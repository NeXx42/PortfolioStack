#!/bin/bash

API_IMAGE="ghcr.io/nexx42/backend:$1"
export API_IMAGE=$API_IMAGE

FRONTEND_IMAGE="ghcr.io/nexx42/frontend:$1"
export FRONTEND_IMAGE=$FRONTEND_IMAGE

docker pull $API_IMAGE
docker pull $FRONTEND_IMAGE

docker compose up -d --no-deps --build

TIMESTAMP=$(date "+%Y-%m-%d %H:%M:%S")
echo "[$TIMESTAMP] Deployed version - $1" >> ./log.txt