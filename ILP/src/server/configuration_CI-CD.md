Вот полная конфигурация CI/CD для проекта с ASP.NET Core (бэкенд) и Vue.js (фронтенд) с использованием GitLab CI/CD:

## Структура проекта
```
project/
├── backend/               # ASP.NET Core проект
│   ├── Api/
│   ├── Services/
│   ├── Tests/
│   └── Dockerfile
├── frontend/              # Vue.js проект
│   ├── src/
│   ├── tests/
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
└── .gitlab-ci.yml
```

## 1. GitLab CI/CD (.gitlab-ci.yml)

```yaml
stages:
  - test
  - build
  - docker-build
  - deploy

variables:
  DOCKER_REGISTRY: registry.gitlab.com
  BACKEND_IMAGE: $DOCKER_REGISTRY/$CI_PROJECT_PATH/backend
  FRONTEND_IMAGE: $DOCKER_REGISTRY/$CI_PROJECT_PATH/frontend
  NUGET_CACHE_FOLDER: "$CI_PROJECT_DIR/backend/.nuget"

cache:
  paths:
    - backend/.nuget/
    - frontend/node_modules/

# ==================== BACKEND ====================
backend-test:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:7.0
  script:
    - cd backend
    - dotnet restore --packages $NUGET_CACHE_FOLDER
    - dotnet build --no-restore
    - dotnet test --no-build --collect:"XPlat Code Coverage"
  artifacts:
    paths:
      - backend/TestResults/
    reports:
      coverage_report:
        coverage_format: cobertura
        path: backend/TestResults/*/coverage.cobertura.xml
  coverage: '/Total\s+\|\s+\d+\.\d+/'
  only:
    - merge_requests
    - main
    - develop

backend-build:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:7.0
  script:
    - cd backend
    - dotnet restore --packages $NUGET_CACHE_FOLDER
    - dotnet publish -c Release -o publish
  artifacts:
    paths:
      - backend/publish/
    expire_in: 1 hour
  only:
    - main
    - develop

# ==================== FRONTEND ====================
frontend-test:
  stage: test
  image: node:18-alpine
  before_script:
    - cd frontend
    - npm ci --cache .npm --prefer-offline
  script:
    - npm run test:unit
    - npm run lint
  artifacts:
    paths:
      - frontend/coverage/
    reports:
      coverage_report:
        coverage_format: cobertura
        path: frontend/coverage/cobertura-coverage.xml
  coverage: '/All files[^|]*\|[^|]*\s+([\d\.]+)/'
  only:
    - merge_requests
    - main
    - develop

frontend-build:
  stage: build
  image: node:18-alpine
  script:
    - cd frontend
    - npm ci --cache .npm --prefer-offline
    - npm run build
  artifacts:
    paths:
      - frontend/dist/
    expire_in: 1 hour
  only:
    - main
    - develop

# ==================== DOCKER BUILD ====================
docker-backend:
  stage: docker-build
  image: docker:24
  services:
    - docker:24-dind
  variables:
    DOCKER_TLS_CERTDIR: "/certs"
  before_script:
    - docker login -u $CI_REGISTRY_USER -p $CI_REGISTRY_PASSWORD $DOCKER_REGISTRY
  script:
    - docker pull $BACKEND_IMAGE:latest || true
    - docker build
        --cache-from $BACKEND_IMAGE:latest
        --tag $BACKEND_IMAGE:$CI_COMMIT_SHA
        --tag $BACKEND_IMAGE:latest
        -f backend/Dockerfile
        ./backend
    - docker push $BACKEND_IMAGE:$CI_COMMIT_SHA
    - docker push $BACKEND_IMAGE:latest
  only:
    - main
    - develop

docker-frontend:
  stage: docker-build
  image: docker:24
  services:
    - docker:24-dind
  variables:
    DOCKER_TLS_CERTDIR: "/certs"
  before_script:
    - docker login -u $CI_REGISTRY_USER -p $CI_REGISTRY_PASSWORD $DOCKER_REGISTRY
  script:
    - docker pull $FRONTEND_IMAGE:latest || true
    - docker build
        --cache-from $FRONTEND_IMAGE:latest
        --tag $FRONTEND_IMAGE:$CI_COMMIT_SHA
        --tag $FRONTEND_IMAGE:latest
        -f frontend/Dockerfile
        ./frontend
    - docker push $FRONTEND_IMAGE:$CI_COMMIT_SHA
    - docker push $FRONTEND_IMAGE:latest
  only:
    - main
    - develop

# ==================== DEPLOY ====================
deploy-production:
  stage: deploy
  image: alpine:latest
  before_script:
    - apk add --no-cache openssh-client
    - eval $(ssh-agent -s)
    - echo "$SSH_PRIVATE_KEY" | tr -d '\r' | ssh-add - > /dev/null
    - mkdir -p ~/.ssh
    - chmod 700 ~/.ssh
  script:
    - ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_HOST "
        cd /app &&
        docker-compose pull &&
        docker-compose up -d --force-recreate &&
        docker system prune -f
      "
  environment:
    name: production
    url: https://your-domain.com
  only:
    - main
  when: manual

deploy-staging:
  stage: deploy
  image: alpine:latest
  before_script:
    - apk add --no-cache openssh-client
    - eval $(ssh-agent -s)
    - echo "$SSH_PRIVATE_KEY" | tr -d '\r' | ssh-add - > /dev/null
  script:
    - ssh -o StrictHostKeyChecking=no $STAGING_USER@$STAGING_HOST "
        cd /app &&
        docker-compose pull &&
        docker-compose up -d --force-recreate
      "
  environment:
    name: staging
    url: https://staging.your-domain.com
  only:
    - develop
```

## 2. Dockerfile для ASP.NET Core (backend/Dockerfile)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
```

## 3. Dockerfile для Vue.js (frontend/Dockerfile)

```dockerfile
# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy source and build
COPY . .
RUN npm run build

# Production stage with nginx
FROM nginx:stable-alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

## 4. Конфигурация Nginx (frontend/nginx.conf)

```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript application/javascript application/json application/xml+rss;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    location / {
        try_files $uri $uri/ /index.html;
    }

    # Proxy API requests to backend
    location /api/ {
        proxy_pass http://backend:80/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Health check endpoint
    location /health {
        access_log off;
        return 200 "healthy\n";
    }
}
```

## 5. Docker Compose (docker-compose.yml)

```yaml
version: '3.8'

services:
  backend:
    image: ${BACKEND_IMAGE:-registry.gitlab.com/your-project/backend:latest}
    container_name: backend
    restart: always
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
      - JWT__Key=${JWT_KEY}
      - JWT__Issuer=${JWT_ISSUER}
    expose:
      - "80"
    networks:
      - app-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  frontend:
    image: ${FRONTEND_IMAGE:-registry.gitlab.com/your-project/frontend:latest}
    container_name: frontend
    restart: always
    ports:
      - "80:80"
    depends_on:
      - backend
    networks:
      - app-network

networks:
  app-network:
    driver: bridge
```

## 6. Конфигурация для GitHub Actions (альтернатива)

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '7.0.x'
  NODE_VERSION: '18'

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      # Backend tests
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Backend tests
        run: |
          cd backend
          dotnet test --collect:"XPlat Code Coverage"

      # Frontend tests
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: ${{ env.NODE_VERSION }}

      - name: Frontend tests
        run: |
          cd frontend
          npm ci
          npm run test:unit

  build-and-push:
    needs: test
    if: github.event_name == 'push' && (github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop')
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Login to DockerHub
        uses: docker/login-action@v2
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}

      - name: Build and push backend
        uses: docker/build-push-action@v4
        with:
          context: ./backend
          push: true
          tags: ${{ secrets.DOCKER_USERNAME }}/backend:latest

      - name: Build and push frontend
        uses: docker/build-push-action@v4
        with:
          context: ./frontend
          push: true
          tags: ${{ secrets.DOCKER_USERNAME }}/frontend:latest

  deploy:
    needs: build-and-push
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to server
        uses: appleboy/ssh-action@v0.1.5
        with:
          host: ${{ secrets.SERVER_HOST }}
          username: ${{ secrets.SERVER_USER }}
          key: ${{ secrets.SSH_PRIVATE_KEY }}
          script: |
            cd /app
            docker-compose pull
            docker-compose up -d --force-recreate
            docker system prune -f
```

## 7. Переменные окружения (настройка в GitLab/GitHub)

**GitLab CI/CD Variables:**
- `SSH_PRIVATE_KEY` - приватный ключ SSH для деплоя
- `SERVER_USER` - пользователь сервера
- `SERVER_HOST` - хост сервера
- `STAGING_USER` - пользователь staging сервера
- `STAGING_HOST` - хост staging сервера
- `DB_CONNECTION_STRING` - строка подключения к БД
- `JWT_KEY` - ключ для JWT
- `JWT_ISSUER` - issuer для JWT

## 8. Скрипт для локального запуска

```bash
#!/bin/bash
# deploy.sh

# Build and run locally
docker-compose up -d --build

# Check logs
docker-compose logs -f

# Stop
docker-compose down
```

## Особенности конфигурации:

1. **Многоступенчатая сборка** - оптимизация размера образов
2. **Кэширование зависимостей** - ускорение сборки
3. **Параллельное выполнение** - тесты и сборка выполняются одновременно
4. **Health checks** - проверка работоспособности
5. **Безопасность** - использование переменных окружения для секретов
6. **Окружения** - разделение staging и production
7. **Code Coverage** - сбор отчетов о покрытии кода

Эта конфигурация обеспечивает полный цикл CI/CD с автоматическим тестированием, сборкой Docker образов и деплоем на сервер.
