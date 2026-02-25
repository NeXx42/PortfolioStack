# ---------- Stage 1: Build ----------
FROM node:20-alpine AS builder
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm ci

# Copy source
COPY app/ ./app/
COPY public/ ./public/
COPY next.config.ts ./
COPY tsconfig.json ./

ARG ENVIRONMENT=docker
COPY .env.$ENVIRONMENT .env

# Build the app
RUN npm run build

# ---------- Stage 2: Production ----------
FROM node:20-alpine AS runner
WORKDIR /app

COPY --from=builder /app/.next/standalone ./

COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

EXPOSE 3000
CMD ["node", "server.js"]