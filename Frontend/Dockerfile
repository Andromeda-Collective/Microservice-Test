# ──────────────────────────────────────────────
# Frontend / React — Dockerfile (multi-stage)
# ──────────────────────────────────────────────

# ── Stage 1: Build ──
FROM node:20-slim AS build
WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build

# ── Stage 2: Serve with nginx ──
# We use a minimal nginx to serve the built static files.
# In production, the main nginx container reverse-proxies to this,
# or you can copy the build output into the main nginx container.
FROM nginx:alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy our built static files
COPY --from=build /app/dist /usr/share/nginx/html

# Simple static-serving config (only used if container is accessed directly)
COPY nginx-frontend.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
