# Deploy no Railway — Acervo Imobiliário

Guia para publicar **API** e **Frontend** no Railway, usando **MongoDB Atlas** como banco.

---

## Pré-requisitos

- Conta no [Railway](https://railway.app)
- Conta no [MongoDB Atlas](https://cloud.mongodb.com)
- Repositório no GitHub com o código commitado
- Node.js 20+ e .NET 9 SDK (apenas para validação local)

---

## Parte 1 — MongoDB Atlas

1. Acesse o Atlas → seu cluster.
2. **Database Access** → crie ou use um usuário com leitura/escrita.
3. **Network Access** → adicione `0.0.0.0/0` (permite Railway; restrinja depois se quiser).
4. **Connect** → **Drivers** → copie a connection string `mongodb+srv://...`.
5. Substitua `<password>` pela senha real (URL-encode caracteres especiais).
6. Inclua o nome do banco na URL:

```
mongodb+srv://usuario:senha@cluster0.xxxxx.mongodb.net/AcervoImobiliario?retryWrites=true&w=majority
```

> **Não commite** a connection string no Git. Configure apenas como variável no Railway.

---

## Parte 2 — Projeto no Railway

1. Acesse [railway.app](https://railway.app) → **New Project**.
2. Escolha **Deploy from GitHub repo**.
3. Autorize o GitHub e selecione o repositório `AcervoImobiliario`.
4. O Railway criará um serviço inicial — você vai configurar **dois serviços** no mesmo projeto.

---

## Parte 3 — Serviço da API (Backend)

### Criar / configurar o serviço

1. Se o Railway criou um serviço automático, renomeie para `acervo-api`.
2. Ou clique **+ New** → **GitHub Repo** → mesmo repositório (segundo serviço).

### Settings

| Campo | Valor |
|-------|-------|
| **Root Directory** | `/` (raiz do repositório) |
| **Builder** | Dockerfile |
| **Dockerfile Path** | `Dockerfile` |

O arquivo `railway.toml` na raiz já define health check em `/health`.

### Variables (aba Variables)

| Variável | Valor |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:${PORT}` |
| `MongoDb__ConnectionString` | Sua connection string do Atlas |
| `MongoDb__DatabaseName` | `AcervoImobiliario` |

Marque `MongoDb__ConnectionString` como **secret**.

### Domínio público

1. Aba **Settings** → **Networking** → **Generate Domain**.
2. Anote a URL, ex.: `https://acervo-api-production.up.railway.app`.

### Validar API

```bash
curl https://<sua-api>.up.railway.app/health
```

Resposta esperada:

```json
{"status":"Healthy","service":"Acervo Imobiliário API","checkedAt":"..."}
```

Teste MongoDB:

```bash
curl https://<sua-api>.up.railway.app/health/ready
```

Deve retornar `"database":"Connected"`.

Teste funcional:

```bash
curl https://<sua-api>.up.railway.app/api/cities
```

---

## Parte 4 — Serviço do Frontend (Web)

### Criar o serviço

1. No mesmo projeto Railway: **+ New** → **GitHub Repo** → mesmo repositório.
2. Renomeie para `acervo-web`.

### Settings

| Campo | Valor |
|-------|-------|
| **Root Directory** | `AcervoImobiliario.Web` |
| **Builder** | Dockerfile |
| **Dockerfile Path** | `Dockerfile` |

### Variables

| Variável | Valor | Build time? |
|----------|-------|-------------|
| `VITE_API_BASE_URL` | `https://<dominio-da-api>` | **Sim** ✅ |

Exemplo com referência Railway (substitua o nome do serviço da API):

```
VITE_API_BASE_URL=https://${{acervo-api.RAILWAY_PUBLIC_DOMAIN}}
```

> **Importante:** `VITE_*` é embutida no build. Marque como disponível no **Build**. Se mudar a URL da API, faça **redeploy** do frontend.

A URL **não** deve terminar com `/api` — o frontend já chama `/api/properties/...`.

### Domínio público

1. **Settings** → **Networking** → **Generate Domain**.
2. Anote a URL, ex.: `https://acervo-web-production.up.railway.app`.

---

## Parte 5 — CORS (ligar frontend à API)

Após publicar o frontend, adicione a URL dele nas variáveis do serviço **acervo-api**:

| Variável | Valor |
|----------|-------|
| `Cors__AllowedOrigins__0` | `https://<dominio-do-frontend>` |
| `Cors__AllowedOrigins__1` | `http://localhost:5173` |

Com referência Railway:

```
Cors__AllowedOrigins__0=https://${{acervo-web.RAILWAY_PUBLIC_DOMAIN}}
Cors__AllowedOrigins__1=http://localhost:5173
```

Salve e aguarde o **redeploy automático** da API.

---

## Parte 6 — Ordem recomendada de deploy

```
1. Atlas configurado (usuário + IP + connection string)
2. Deploy da API com MongoDb__ConnectionString
3. Validar /health e /health/ready
4. Deploy do frontend com VITE_API_BASE_URL apontando para a API
5. Configurar CORS com URL do frontend
6. Testar fluxo completo no navegador
```

---

## Checklist pós-deploy

- [ ] `GET /health` → 200
- [ ] `GET /health/ready` → 200 e `database: Connected`
- [ ] `GET /api/cities` → lista de cidades (seed na primeira execução)
- [ ] Frontend abre sem erro
- [ ] Busca de imóveis funciona (sem erro CORS no console)
- [ ] Cadastro de cidade funciona
- [ ] Cadastro de imóvel funciona
- [ ] Histórico funciona
- [ ] Connection string **não** está no Git

---

## Problemas comuns

| Problema | Solução |
|----------|---------|
| API não sobe | Verifique logs; confirme `ASPNETCORE_URLS=http://0.0.0.0:${PORT}` |
| `/health/ready` → 503 | Connection string incorreta ou IP não liberado no Atlas |
| Frontend chama API errada | `VITE_API_BASE_URL` incorreta ou não marcada no build |
| Erro CORS | Adicione URL do frontend em `Cors__AllowedOrigins__0` |
| Build frontend falha | Root Directory deve ser `AcervoImobiliario.Web` |
| Build API falha | Root Directory deve ser `/` (raiz) |
| Swagger 404 em produção | Esperado — Swagger só em `Development` |

---

## Arquivos de deploy no repositório

| Arquivo | Serviço |
|---------|---------|
| `Dockerfile` | API |
| `railway.toml` | API |
| `.dockerignore` | API |
| `AcervoImobiliario.Web/Dockerfile` | Frontend |
| `AcervoImobiliario.Web/railway.toml` | Frontend |
| `AcervoImobiliario.Web/.dockerignore` | Frontend |
