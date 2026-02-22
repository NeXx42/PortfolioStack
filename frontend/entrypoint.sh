#!/bin/sh
# Replace placeholders with actual env vars at runtime
echo "Injecting runtime environment variables..."
envsubst < /usr/share/nginx/html/index.html > /usr/share/nginx/html/index.tmp
mv /usr/share/nginx/html/index.tmp /usr/share/nginx/html/index.html

# Start Nginx
nginx -g 'daemon off;'