# ──────────────────────────────────────────────
# nginx — Dockerfile
# ──────────────────────────────────────────────
FROM nginx:alpine

# Remove default config
RUN rm /etc/nginx/conf.d/default.conf

# Copy our custom config
COPY conf.d/ /etc/nginx/conf.d/

# SSL certs and DH params are mounted at runtime via volumes,
# so we don't bake them into the image.

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
