#!/bin/bash
# Generate self-signed SSL certificates for local development.
# DO NOT use these certificates in production.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SSL_DIR="$SCRIPT_DIR"

echo "Generating self-signed SSL certificate..."

openssl req -x509 -nodes \
    -days 365 \
    -newkey rsa:2048 \
    -keyout "$SSL_DIR/privkey.pem" \
    -out "$SSL_DIR/fullchain.pem" \
    -subj "/C=IR/ST=Tehran/L=Tehran/O=Microservice-Test/OU=Dev/CN=localhost" \
    -addext "subjectAltName=DNS:localhost,DNS:www.localhost,DNS:api.localhost,IP:127.0.0.1"

echo "Done. Files generated:"
echo "  $SSL_DIR/privkey.pem"
echo "  $SSL_DIR/fullchain.pem"
