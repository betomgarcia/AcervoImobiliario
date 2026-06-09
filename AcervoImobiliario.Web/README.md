# Acervo Imobiliário — Frontend

Interface web do sistema Acervo Imobiliário, consumindo a API .NET real.

## Stack

- React + TypeScript + Vite
- React Router, React Query, React Hook Form, Zod
- Material UI, Axios
- PWA preparado (`vite-plugin-pwa`)

## Pré-requisitos

- Node.js 20+
- API rodando em `http://localhost:5219`
- MongoDB via `docker compose up -d`

## Configuração

```bash
cd AcervoImobiliario.Web
npm install
cp .env.example .env
```

Em desenvolvimento, o Vite faz proxy de `/api` para a API local — `VITE_API_BASE_URL` pode ficar vazio.

Para build de produção, defina:

```env
VITE_API_BASE_URL=http://localhost:5219
```

## Executar

Terminal 1 — API:

```bash
dotnet run --project ../AcervoImobiliario.Api
```

Terminal 2 — Frontend:

```bash
npm run dev
```

Acesse: http://localhost:5173

## Telas

1. Busca progressiva de imóveis com autocomplete
2. Cadastro de imóvel
3. Detalhe do imóvel
4. Histórico em timeline/cards expansíveis
5. Cadastro de evento no histórico

## Build

```bash
npm run build
npm run preview
```
