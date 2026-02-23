# Stage 1: build
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
COPY tsconfig*.json ./
COPY vite.config.ts ./
COPY public ./public
COPY src ./src
COPY index.html ./

COPY entrypoint.sh /entrypoint.sh

RUN npm ci
RUN npm run build

# server
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

EXPOSE 80
ENTRYPOINT ["/entrypoint.sh"]