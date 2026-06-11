# CI/CD — Acervo Imobiliário (DEV e PRD)

Pipeline via **GitHub Actions** com três plataformas:

| Plataforma | Responsabilidade |
|------------|------------------|
| **Vercel** | Frontend (React/Vite/PWA) |
| **Railway** | API .NET 9 (somente backend) |
| **MongoDB Atlas** | Banco de dados |

---

## Arquitetura validada

| Componente | DEV | PRD |
|------------|-----|-----|
| **Frontend** | https://acervo-dev.arlabs.dev.br | https://acervo.arlabs.dev.br |
| **API** | https://api-dev-acervo.arlabs.dev.br | https://api-acervo.arlabs.dev.br |
| **Hospedagem frontend** | Vercel | Vercel |
| **Hospedagem API** | Railway (`development`) | Railway (`production`) |
| **MongoDB** | Atlas — database `AcervoImobiliarioDev` | Atlas — database `AcervoImobiliario` |

DEV e PRD usam o **mesmo cluster Atlas**. A separação é feita por `MongoDb__DatabaseName` (não por instâncias MongoDB no Railway).

```
Push em develop                    Tag v* (ex.: v1.0.0)
        │                                  │
        ▼                                  ▼
   ┌─────────┐                        ┌─────────┐
   │ validate │                        │ validate │
   └────┬────┘                        └────┬────┘
        ▼                                  ▼
   ┌────────────┐                    ┌────────────┐
   │ deploy-dev │ (automático)         │ deploy-prd │ (aprovação manual)
   │ Vercel DEV │                    │ Vercel PRD │
   │ Railway DEV│                    │ Railway PRD│
   └────────────┘                    └────────────┘
```

**O que NÃO dispara deploy:**
- Push em `main` (sem tag) → nenhum deploy
- Tag `v*` → **não** faz deploy DEV
- Push em `develop` → **não** faz deploy PRD

---

## Fluxos de deploy

### Deploy DEV (branch `develop`)

```bash
git checkout develop
git merge main   # ou commits diretos em develop
git push origin develop
```

1. Job **Validate** — build frontend + testes backend
2. Job **Deploy DEV** — Vercel + Railway (environment `development`)

### Deploy PRD (tag `v*`)

```bash
git tag v1.0.0
git push origin v1.0.0
```

1. Job **Validate**
2. Job **Deploy PRD** — aguarda **aprovação manual** no GitHub Environment `production`
3. Após aprovar: Vercel PRD + Railway PRD

---

## GitHub — Environments

Crie em **Settings → Environments**:

### `development`

- **Required reviewers:** nenhum
- **Deployment branches:** restringir a `develop` (recomendado)

| Nome | Tipo | Valor |
|------|------|-------|
| `VERCEL_PROJECT_ID` | Secret | ID do projeto Vercel DEV |
| `RAILWAY_TOKEN` | Secret | Project Token Railway (DEV) |
| `RAILWAY_SERVICE_ID_API` | Secret | ID do serviço API no Railway |
| `VITE_API_BASE_URL` | Variable | `https://api-dev-acervo.arlabs.dev.br` |
| `RAILWAY_ENVIRONMENT` | Variable | `development` |

### `production`

- **Required reviewers:** você (ou time) — **obrigatório**

| Nome | Tipo | Valor |
|------|------|-------|
| `VERCEL_PROJECT_ID` | Secret | ID do projeto Vercel PRD |
| `RAILWAY_TOKEN` | Secret | Project Token Railway (PRD) |
| `RAILWAY_SERVICE_ID_API` | Secret | ID do serviço API no Railway |
| `VITE_API_BASE_URL` | Variable | `https://api-acervo.arlabs.dev.br` |
| `RAILWAY_ENVIRONMENT` | Variable | `production` |

### Repositório (compartilhados)

| Nome | Tipo |
|------|------|
| `VERCEL_TOKEN` | Secret |
| `VERCEL_ORG_ID` | Secret |

> Os environments usam os **mesmos nomes** de secrets (`VERCEL_PROJECT_ID`, `RAILWAY_TOKEN`, etc.). O GitHub resolve o valor correto conforme o environment do job.

---

## Vercel (frontend)

| Projeto | Domínio | Root Directory |
|---------|---------|----------------|
| DEV | acervo-dev.arlabs.dev.br | `AcervoImobiliario.Web` |
| PRD | acervo.arlabs.dev.br | `AcervoImobiliario.Web` |

| Campo | Valor |
|-------|-------|
| Framework | Vite |
| Build Command | `npm run build` |
| Output Directory | `dist` |
| Install Command | `npm ci` |

`VITE_API_BASE_URL` é injetada pelo GitHub Actions no `vercel build` — não fixe no painel Vercel para deploys via CI.

---

## Railway (API apenas)

Projeto **Acervo Imobiliário** com dois environments:

| Environment | Domínio API | `ASPNETCORE_ENVIRONMENT` |
|-------------|-------------|--------------------------|
| `development` | api-dev-acervo.arlabs.dev.br | `Development` |
| `production` | api-acervo.arlabs.dev.br | `Production` |

| Campo do serviço API | Valor |
|----------------------|-------|
| Root Directory | `/` |
| Builder | Dockerfile |

**O Railway não hospeda MongoDB neste projeto.**

### Variáveis da API — DEV

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:${PORT}
MongoDb__ConnectionString=<connection-string-atlas>
MongoDb__DatabaseName=AcervoImobiliarioDev
Cors__AllowedOrigins__0=https://acervo-dev.arlabs.dev.br
Cors__AllowedOrigins__1=http://localhost:5173
```

### Variáveis da API — PRD

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:${PORT}
MongoDb__ConnectionString=<connection-string-atlas>
MongoDb__DatabaseName=AcervoImobiliario
Cors__AllowedOrigins__0=https://acervo.arlabs.dev.br
```

> A `MongoDb__ConnectionString` pode ser **a mesma** nos dois environments (mesmo cluster Atlas). O isolamento DEV/PRD é garantido por `MongoDb__DatabaseName`.

Configure a connection string como **secret** no Railway. **Nunca** commite no Git.

---

## MongoDB Atlas

1. Um cluster compartilhado entre DEV e PRD.
2. **Network Access:** liberar IPs do Railway (`0.0.0.0/0` para começar).
3. **Database Access:** usuário com read/write.
4. Connection string `mongodb+srv://...` configurada em cada serviço API no Railway.

| Ambiente | `MongoDb__DatabaseName` |
|----------|-------------------------|
| DEV | `AcervoImobiliarioDev` |
| PRD | `AcervoImobiliario` |

Na primeira subida da API, o `MongoDbInitializer` cria índices e seed de cidades. O database é criado automaticamente pelo MongoDB no primeiro uso.

Coleções: `cities`, `properties`, `property_histories`.

---

## CORS

| Ambiente | Origem |
|----------|--------|
| DEV | `https://acervo-dev.arlabs.dev.br` |
| PRD | `https://acervo.arlabs.dev.br` |

Configurado na API do Railway via `Cors__AllowedOrigins__N`.

---

## Validar após deploy

### DEV

```bash
curl https://api-dev-acervo.arlabs.dev.br/health
curl https://api-dev-acervo.arlabs.dev.br/health/ready
```

Abrir https://acervo-dev.arlabs.dev.br — testar busca/cadastro sem erro CORS.

### PRD

```bash
curl https://api-acervo.arlabs.dev.br/health
curl https://api-acervo.arlabs.dev.br/health/ready
```

Abrir https://acervo.arlabs.dev.br.

---

## Rollback

| Componente | Ação |
|------------|------|
| Frontend | Vercel → Deployments → Promote anterior |
| API | Railway → Deployments → Redeploy anterior |
| Release | Nova tag ou re-push de tag após revert |

---

## Pontos de atenção

| Tópico | Detalhe |
|--------|---------|
| **Banco isolado** | Mesmo cluster Atlas, databases diferentes |
| **VITE no build** | `VITE_API_BASE_URL` vem do GitHub Variable por environment |
| **Tag ≠ DEV** | Tags `v*` só disparam PRD |
| **main ≠ deploy** | Push em `main` sem tag não publica |
| **Swagger** | Ativo na API DEV; desabilitado em PRD |
| **Health** | `/health` (liveness) e `/health/ready` (MongoDB) |

---

## Arquivos do repositório

| Arquivo | Função |
|---------|--------|
| `.github/workflows/deploy.yml` | Pipeline CI/CD |
| `AcervoImobiliario.Web/vercel.json` | SPA rewrites Vercel |
| `Dockerfile` | Build da API |
| `railway.toml` | Health check `/health` |

---

## Comandos úteis

```bash
# Validar localmente
cd AcervoImobiliario.Web && npm ci && npm run build
dotnet restore && dotnet build && dotnet test

# Deploy DEV
git push origin develop

# Deploy PRD
git tag v1.0.0 && git push origin v1.0.0
```
