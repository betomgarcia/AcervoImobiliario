# Frontend — Acervo Imobiliário (React + Vite)
FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

FROM node:20-alpine AS final
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/dist ./dist

ENV NODE_ENV=production

CMD ["sh", "-c", "serve -s dist -l tcp://0.0.0.0:${PORT:-3000}"]
