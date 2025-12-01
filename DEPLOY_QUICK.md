# 🚀 Quick Deployment Reference

## Fastest Option: Docker (5 minutes)

### Prerequisites
- Docker Desktop installed and running

### One-Command Deploy
```bash
cd /Users/benh/Documents/DevOpsDashboard
./deploy.sh
```

That's it! Your app will be running at:
- **Frontend**: http://localhost
- **Backend**: http://localhost:5000
- **API Docs**: http://localhost:5000/swagger

### Docker Commands
```bash
# Start
docker-compose up -d

# Stop
docker-compose down

# View logs
docker-compose logs -f

# Rebuild
docker-compose build

# Check status
docker-compose ps
```

---

## Cloud Deployment Quick Start

### Azure (Recommended for .NET)

**1. Install Azure CLI:**
```bash
brew install azure-cli
az login
```

**2. Deploy:**
```bash
# Backend
cd backend
az webapp up --name devops-dashboard-api --runtime "DOTNET:10.0"

# Frontend
cd frontend
npm run build -- --configuration production
az staticwebapp create --name devops-dashboard-web
```

### Heroku (Easiest)

**1. Install Heroku CLI:**
```bash
brew install heroku/brew/heroku
heroku login
```

**2. Deploy:**
```bash
# Backend
heroku create devops-dashboard-api
git subtree push --prefix backend heroku main

# Frontend
heroku create devops-dashboard-web --buildpack heroku/nodejs
git subtree push --prefix frontend heroku main
```

### DigitalOcean (Best Value)

**1. Install doctl:**
```bash
brew install doctl
doctl auth init
```

**2. Deploy:**
```bash
doctl apps create --spec app.yaml
```

---

## Production Checklist

Before going live:

```bash
# 1. Set environment variables
export ASPNETCORE_ENVIRONMENT=Production
export GITHUB_TOKEN=your_token_here

# 2. Update frontend production URLs
# Edit: frontend/src/environments/environment.prod.ts

# 3. Enable HTTPS (use Let's Encrypt)
# 4. Set up database backups
# 5. Configure monitoring
```

---

## Troubleshooting

**Port already in use:**
```bash
# Find and kill process
lsof -ti:5000 | xargs kill -9
lsof -ti:80 | xargs kill -9
```

**Docker issues:**
```bash
# Reset Docker
docker system prune -a
docker volume prune
```

**Build fails:**
```bash
# Clean and rebuild
rm -rf backend/bin backend/obj
rm -rf frontend/node_modules frontend/www
docker-compose build --no-cache
```

---

## Monitoring Your Deployment

```bash
# Check if services are healthy
curl http://localhost:5000/api/servers
curl http://localhost

# Docker stats
docker stats

# View all logs
docker-compose logs -f
```

---

## Quick Updates

```bash
# Pull latest code and redeploy
git pull origin main
./deploy.sh
```

---

## Need Help?

- **Full Guide**: See [DEPLOYMENT.md](DEPLOYMENT.md)
- **Setup Guide**: See [SETUP.md](SETUP.md)
- **Testing Guide**: See [TDD_GUIDE.md](TDD_GUIDE.md)
- **GitHub**: https://github.com/bholsinger09/DevOpsDashboard

---

**Your DevOps Dashboard is deployment-ready! 🎉**
