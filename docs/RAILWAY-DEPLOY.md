# Deploy da API no Railway — Acervo Imobiliário

> **CI/CD automatizado:** [`deploy.md`](deploy.md)  
> Este guia cobre configuração manual da **API** no Railway.

---

## Arquitetura

| Componente | Onde roda |
|------------|-----------|
| **Frontend** | Vercel (`acervo-dev.arlabs.dev.br` / `acervo.arlabs.dev.br`) |
| **API** | Railway (este guia) |
| **MongoDB** | MongoDB Atlas (não no Railway) |

---

## Pré-requisitos

- Conta [Railway](https://railway.app)
- Cluster [MongoDB Atlas](https://cloud.mongodb.com) configurado
- Repositório GitHub conectado

---

## MongoDB Atlas

1. **Database Access** — usuário com read/write.
2. **Network Access** — liberar IPs do Railway (`0.0.0.0/0` inicialmente).
3. Copiar connection string `mongodb+srv://usuario:senha@cluster...`.

A mesma connection string pode ser usada em DEV e PRD. A separação é por `MongoDb__DatabaseName`:

| Ambiente Railway | `MongoDb__DatabaseName` |
|------------------|-------------------------|
| `development` | `AcervoImobiliarioDev` |
| `production` | `AcervoImobiliario` |

> **Não** use serviço MongoDB no Railway neste projeto.

---

## Projeto Railway

Projeto: **Acervo Imobiliário**

Dois **environments**, cada um com **apenas o serviço da API**:

| Environment | Domínio customizado |
|-------------|---------------------|
| `development` | https://api-dev-acervo.arlabs.dev.br |
| `production` | https://api-acervo.arlabs.dev.br |

### Settings do serviço API

| Campo | Valor |
|-------|-------|
| Root Directory | `/` |
| Builder | Dockerfile |
| Health check | `/health` (via `railway.toml`) |

### Variáveis — environment `development`

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:${PORT}
MongoDb__ConnectionString=<sua-connection-string-atlas>
MongoDb__DatabaseName=AcervoImobiliarioDev
Cors__AllowedOrigins__0=https://acervo-dev.arlabs.dev.br
Cors__AllowedOrigins__1=http://localhost:5173
```

### Variáveis — environment `production`

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:${PORT}
MongoDb__ConnectionString=<sua-connection-string-atlas>
MongoDb__DatabaseName=AcervoImobiliario
Cors__AllowedOrigins__0=https://acervo.arlabs.dev.br
```

Marque `MongoDb__ConnectionString` como secret. Não commite no Git.

### Domínio customizado

**Settings → Networking → Custom Domain** → configure `api-dev-acervo.arlabs.dev.br` ou `api-acervo.arlabs.dev.br` conforme o environment.

---

## Validar API

```bash
# DEV
curl https://api-dev-acervo.arlabs.dev.br/health
curl https://api-dev-acervo.arlabs.dev.br/health/ready
curl https://api-dev-acervo.arlabs.dev.br/api/cities

# PRD
curl https://api-acervo.arlabs.dev.br/health
curl https://api-acervo.arlabs.dev.br/health/ready
curl https://api-acervo.arlabs.dev.br/api/cities
```

`/health/ready` retorna `"database":"Connected"` quando o Atlas está acessível.

---

## Frontend (Vercel)

O frontend **não** é publicado no Railway. Configure na Vercel:

| Projeto | Domínio | `VITE_API_BASE_URL` (via CI) |
|---------|---------|------------------------------|
| DEV | acervo-dev.arlabs.dev.br | `https://api-dev-acervo.arlabs.dev.br` |
| PRD | acervo.arlabs.dev.br | `https://api-acervo.arlabs.dev.br` |

---

## Problemas comuns

| Problema | Solução |
|----------|---------|
| API não sobe | `ASPNETCORE_URLS=http://0.0.0.0:${PORT}` |
| `/health/ready` → 503 | Connection string Atlas incorreta ou IP bloqueado no Atlas |
| Erro CORS | `Cors__AllowedOrigins__0` com URL exata do frontend Vercel |
| Dados DEV em PRD | Verificar `MongoDb__DatabaseName` diferente em cada environment |
| Swagger 404 em PRD | Esperado — Swagger só em `Development` |

---

## Arquivos no repositório

| Arquivo | Função |
|---------|--------|
| `Dockerfile` | Build multi-stage .NET 9 |
| `railway.toml` | Health check `/health` |
| `.dockerignore` | Otimização do build |
