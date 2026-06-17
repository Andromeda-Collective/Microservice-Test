# ──────────────────────────────────────────────
# cart-service / Django — Dockerfile
# ──────────────────────────────────────────────
FROM python:3.12-slim

# Prevent Python from writing .pyc files and enable unbuffered stdout
ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

WORKDIR /app

# Install dependencies first (layer caching)
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy project source
COPY . .

# Collect static files (optional — safe to fail if no static)
RUN python manage.py collectstatic --noinput 2>/dev/null || true

# Default port for Django via gunicorn
EXPOSE 8000

# Gunicorn entrypoint — adjust worker count as needed
CMD ["gunicorn", "--bind", "0.0.0.0:8000", "--workers", "3", "config.wsgi:application"]
