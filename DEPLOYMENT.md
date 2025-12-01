# DevOps Dashboard - Deployment Guide

This guide covers multiple deployment options for your DevOps Dashboard PWA.

---

## 🐳 Option 1: Docker Deployment (Recommended)

### Prerequisites
- Docker Desktop installed
- Docker Compose installed

### Quick Start

1. **Build and run with Docker Compose:**
```bash
docker-compose up -d
```

2. **Access the application:**
- Frontend: http://localhost
- Backend API: http://localhost:5000
- Swagger Docs: http://localhost:5000/swagger

3. **View logs:**
```bash
docker-compose logs -f
```

4. **Stop services:**
```bash
docker-compose down
```

### Production Docker Deployment

Build for production:
```bash
# Build images
docker-compose build

# Push to Docker Hub (optional)
docker tag devops-dashboard-api:latest yourusername/devops-dashboard-api:latest
docker tag devops-dashboard-web:latest yourusername/devops-dashboard-web:latest
docker push yourusername/devops-dashboard-api:latest
docker push yourusername/devops-dashboard-web:latest
```

---

## ☁️ Option 2: Azure Deployment

### A. Azure App Service (Backend)

1. **Create App Service:**
```bash
# Login to Azure
az login

# Create resource group
az group create --name devops-dashboard-rg --location eastus

# Create App Service plan
az appservice plan create \
  --name devops-dashboard-plan \
  --resource-group devops-dashboard-rg \
  --sku B1 --is-linux

# Create web app
az webapp create \
  --name devops-dashboard-api \
  --resource-group devops-dashboard-rg \
  --plan devops-dashboard-plan \
  --runtime "DOTNET|10.0"
```

2. **Deploy Backend:**
```bash
cd backend
dotnet publish -c Release -o ./publish
cd publish
zip -r ../api.zip .
cd ..

az webapp deployment source config-zip \
  --resource-group devops-dashboard-rg \
  --name devops-dashboard-api \
  --src api.zip
```

3. **Configure settings:**
```bash
az webapp config appsettings set \
  --resource-group devops-dashboard-rg \
  --name devops-dashboard-api \
  --settings ASPNETCORE_ENVIRONMENT=Production
```

### B. Azure Static Web Apps (Frontend)

1. **Build frontend:**
```bash
cd frontend
npm run build -- --configuration production
```

2. **Deploy via Azure Portal:**
- Go to Azure Portal → Static Web Apps
- Create new resource
- Connect to GitHub repository
- Set build folder: `frontend`
- Set output folder: `www`

Or use CLI:
```bash
npm install -g @azure/static-web-apps-cli
swa deploy --app-location ./frontend --output-location ./www
```

---

## 🚀 Option 3: AWS Deployment

### A. Elastic Beanstalk (Backend)

1. **Install EB CLI:**
```bash
pip install awsebcli
```

2. **Initialize and deploy:**
```bash
cd backend
eb init -p "64bit Amazon Linux 2 v2.6.0 running .NET 8" devops-dashboard-api
eb create devops-dashboard-env
eb deploy
```

### B. S3 + CloudFront (Frontend)

1. **Build frontend:**
```bash
cd frontend
npm run build -- --configuration production
```

2. **Create S3 bucket:**
```bash
aws s3 mb s3://devops-dashboard-app
aws s3 website s3://devops-dashboard-app --index-document index.html
```

3. **Upload files:**
```bash
aws s3 sync ./www s3://devops-dashboard-app --acl public-read
```

4. **Create CloudFront distribution:**
```bash
aws cloudfront create-distribution \
  --origin-domain-name devops-dashboard-app.s3.amazonaws.com
```

---

## 🌊 Option 4: DigitalOcean App Platform

### Deploy via CLI

1. **Install doctl:**
```bash
brew install doctl
doctl auth init
```

2. **Create app.yaml:**
```yaml
name: devops-dashboard
services:
  - name: api
    github:
      repo: bholsinger09/DevOpsDashboard
      branch: main
      deploy_on_push: true
    source_dir: /backend
    dockerfile_path: backend/Dockerfile
    http_port: 5000
    
  - name: web
    github:
      repo: bholsinger09/DevOpsDashboard
      branch: main
      deploy_on_push: true
    source_dir: /frontend
    dockerfile_path: frontend/Dockerfile
    http_port: 80
    routes:
      - path: /
```

3. **Deploy:**
```bash
doctl apps create --spec app.yaml
```

---

## 🎯 Option 5: Heroku Deployment

### Backend Deployment

1. **Create Heroku app:**
```bash
heroku login
heroku create devops-dashboard-api
```

2. **Add buildpack:**
```bash
heroku buildpacks:set heroku/dotnet
```

3. **Deploy:**
```bash
git subtree push --prefix backend heroku main
```

### Frontend Deployment

1. **Create app:**
```bash
heroku create devops-dashboard-web --buildpack heroku/nodejs
```

2. **Add nginx buildpack:**
```bash
heroku buildpacks:add https://github.com/heroku/heroku-buildpack-nginx
```

3. **Deploy:**
```bash
git subtree push --prefix frontend heroku main
```

---

## 🔧 Option 6: Manual VPS Deployment

### Prerequisites
- Ubuntu 22.04 server
- Domain name (optional)

### Setup Script

```bash
#!/bin/bash

# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 10
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

# Install Node.js
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Install Nginx
sudo apt install -y nginx

# Install PM2 for process management
sudo npm install -g pm2

# Clone repository
git clone https://github.com/bholsinger09/DevOpsDashboard.git
cd DevOpsDashboard

# Build backend
cd backend
dotnet publish -c Release -o /var/www/devops-api

# Build frontend
cd ../frontend
npm install --legacy-peer-deps
npm run build -- --configuration production
sudo cp -r www/* /var/www/devops-web/

# Configure Nginx
sudo tee /etc/nginx/sites-available/devops-dashboard > /dev/null <<EOF
server {
    listen 80;
    server_name your-domain.com;

    location / {
        root /var/www/devops-web;
        try_files \$uri \$uri/ /index.html;
    }

    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOF

sudo ln -s /etc/nginx/sites-available/devops-dashboard /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx

# Start backend with PM2
cd /var/www/devops-api
pm2 start dotnet --name devops-api -- DevOpsDashboard.API.dll
pm2 startup
pm2 save
```

---

## 🔒 Production Checklist

Before deploying to production:

- [ ] **Environment Variables**
  - [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
  - [ ] Configure database connection string
  - [ ] Set GitHub token if using GitHub integration
  - [ ] Configure any API keys

- [ ] **Security**
  - [ ] Enable HTTPS/SSL
  - [ ] Configure CORS properly
  - [ ] Set secure authentication
  - [ ] Review exposed endpoints
  - [ ] Enable rate limiting

- [ ] **Database**
  - [ ] Migrate from SQLite to PostgreSQL/SQL Server
  - [ ] Set up database backups
  - [ ] Configure connection pooling

- [ ] **Monitoring**
  - [ ] Set up application logging
  - [ ] Configure error tracking (Sentry, Application Insights)
  - [ ] Set up uptime monitoring
  - [ ] Configure alerts

- [ ] **Performance**
  - [ ] Enable response caching
  - [ ] Configure CDN for static assets
  - [ ] Optimize images
  - [ ] Enable compression

- [ ] **Testing**
  - [ ] Run all tests
  - [ ] Load testing
  - [ ] Security scanning
  - [ ] Cross-browser testing

---

## 📊 Monitoring Your Deployment

### Health Checks

Backend health endpoint:
```bash
curl http://your-domain.com/api/servers
```

Frontend availability:
```bash
curl -I http://your-domain.com
```

### Docker monitoring:
```bash
docker stats
docker-compose ps
docker-compose logs -f --tail=100
```

### Database backup:
```bash
# If using Docker volume
docker run --rm -v devopsdashboard_backend-data:/data -v $(pwd):/backup alpine tar czf /backup/backup.tar.gz /data
```

---

## 🆘 Troubleshooting

### Backend not starting:
- Check logs: `docker-compose logs backend`
- Verify database connection
- Check port 5000 is available

### Frontend 404 errors:
- Verify nginx configuration
- Check frontend build completed successfully
- Ensure API URL is correct in environment files

### SignalR connection issues:
- Enable WebSocket support in reverse proxy
- Check CORS configuration
- Verify firewall rules

---

## 🔄 Updates and Rollbacks

### Update deployment:
```bash
git pull origin main
docker-compose down
docker-compose build
docker-compose up -d
```

### Rollback:
```bash
docker-compose down
git checkout <previous-commit>
docker-compose build
docker-compose up -d
```

---

## 📞 Support

For deployment issues:
1. Check logs: `docker-compose logs`
2. Review this guide
3. Check GitHub Issues
4. Contact repository maintainer

---

**Choose the deployment option that best fits your needs and infrastructure!**
