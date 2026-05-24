# ── Stage 1: build React app ─────────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# Install deps first (better layer caching when only source changes)
COPY package.json package-lock.json ./
RUN npm ci --prefer-offline

# Build
COPY . .
RUN npm run build          # tsc -b && vite build → dist/

# ── Stage 2: serve with nginx ────────────────────────────────────────────────
FROM nginx:1.27-alpine AS runtime

# Replace default config with our SPA config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built assets
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
