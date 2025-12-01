# DevOps Dashboard - Quick Start Guide

## 🚀 Complete Setup Instructions

This guide will walk you through setting up and running both the backend API and frontend application.

---

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **Git** - [Download here](https://git-scm.com/)
- **GitHub Personal Access Token** - [Create one here](https://github.com/settings/tokens)

---

## Backend Setup (ASP.NET Core API)

### Step 1: Navigate to Backend Directory
```bash
cd backend
```

### Step 2: Configure GitHub Integration
Edit `appsettings.json` and add your GitHub Personal Access Token:

```json
{
  "GitHub": {
    "Token": "ghp_your_token_here",
    "Owner": "bholsinger09",
    "Repository": "DevOpsDashboard"
  }
}
```

**To create a GitHub token:**
1. Go to GitHub Settings → Developer settings → Personal access tokens
2. Click "Generate new token (classic)"
3. Select scopes: `repo` (all) and `read:org`
4. Copy the token and paste it in `appsettings.json`

### Step 3: Restore Dependencies
```bash
dotnet restore
```

### Step 4: Run the Backend
```bash
dotnet run
```

The backend will start on:
- **API**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger
- **Hangfire Dashboard**: https://localhost:5001/hangfire

### Backend Features:
- ✅ RESTful API endpoints
- ✅ SQLite database (auto-created)
- ✅ SignalR hub for real-time updates
- ✅ Hangfire background jobs
- ✅ GitHub API integration

---

## Frontend Setup (Ionic Angular PWA)

### Step 1: Navigate to Frontend Directory
```bash
cd frontend
```

### Step 2: Install Dependencies
```bash
npm install
```

This will install:
- Ionic Framework
- Angular 17
- SignalR client
- All required dependencies

### Step 3: Verify API Configuration
Check `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  hubUrl: 'https://localhost:5001/dashboardHub'
};
```

### Step 4: Start the Development Server
```bash
npm start
```

The frontend will open automatically at: **http://localhost:8100**

### Frontend Features:
- ✅ Responsive PWA design
- ✅ Real-time updates via SignalR
- ✅ Offline support with service worker
- ✅ Mobile-friendly interface
- ✅ Dark mode support

---

## 🎯 First Time Usage

### 1. Add Your First Server
1. Navigate to **Servers** page
2. Click the **+** button
3. Enter:
   - Name: `My Server`
   - URL: `https://example.com`
   - Description: Optional

### 2. Sync GitHub Data
1. Navigate to **GitHub** page
2. Click the sync icon
3. Wait for issues and PRs to load

### 3. Create a Deployment
1. Navigate to **Deployments** page
2. Click the **+** button
3. Enter deployment details

### 4. View Dashboard
Check the **Dashboard** page for:
- Server statistics
- GitHub activity
- Recent deployments
- System logs

---

## 📡 Real-time Features

The dashboard automatically updates in real-time when:
- Server status changes (checked every minute)
- GitHub data is synced (every 5 minutes)
- New deployments are created
- Critical logs are recorded

---

## 🔧 Development Commands

### Backend
```bash
# Run with hot reload
dotnet watch run

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Build for production
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
# Start dev server
npm start

# Build for production
npm run build

# Run tests
npm test

# Lint code
npm run lint

# Generate PWA assets
npm run ionic:build
```

---

## 🌐 Building for Production

### Backend Production Build
```bash
cd backend
dotnet publish -c Release -o ./publish
```

### Frontend Production Build
```bash
cd frontend
npm run build
```

The production files will be in `frontend/www/`

---

## 📱 PWA Installation

Once running, you can install the app:

**On Desktop:**
1. Open http://localhost:8100
2. Look for the install icon in the address bar
3. Click "Install"

**On Mobile:**
1. Open http://localhost:8100 in a mobile browser
2. Tap the browser menu
3. Select "Add to Home Screen"

---

## 🔍 API Endpoints Overview

### Servers
- `GET /api/servers` - List all servers
- `POST /api/servers` - Create server
- `PUT /api/servers/{id}` - Update server
- `DELETE /api/servers/{id}` - Delete server
- `GET /api/servers/stats` - Get statistics

### GitHub
- `GET /api/github/issues` - Get issues
- `GET /api/github/pullrequests` - Get PRs
- `POST /api/github/sync` - Manual sync
- `GET /api/github/stats` - Get statistics

### Deployments
- `GET /api/deployments` - List deployments
- `POST /api/deployments` - Create deployment
- `PUT /api/deployments/{id}` - Update deployment
- `GET /api/deployments/stats` - Get statistics

### Logs
- `GET /api/logs` - List logs (with filtering)
- `POST /api/logs` - Create log entry
- `DELETE /api/logs/{id}` - Delete log
- `DELETE /api/logs/cleanup` - Cleanup old logs
- `GET /api/logs/stats` - Get statistics

---

## 🐛 Troubleshooting

### Backend Issues

**Port already in use:**
```bash
# Change port in Properties/launchSettings.json or use:
dotnet run --urls "https://localhost:5002"
```

**Database errors:**
```bash
# Delete and recreate database
rm devopsdashboard.db
dotnet run
```

**CORS errors:**
Ensure your frontend URL is in the CORS policy in `Program.cs`

### Frontend Issues

**npm install fails:**
```bash
# Clear cache and retry
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

**SignalR connection fails:**
- Ensure backend is running
- Check API URL in `environment.ts`
- Check browser console for errors

**PWA not working:**
```bash
# Build for production to enable service worker
npm run build
```

---

## 📊 Monitoring

### Hangfire Dashboard
Access at: https://localhost:5001/hangfire

View:
- Scheduled jobs
- Job history
- Failed jobs
- Server statistics

### SignalR Connection
Check browser console for:
- "SignalR connection started" - ✅ Connected
- Connection errors - ❌ Check backend

---

## 🚢 Deployment Options

### Backend
- Azure App Service
- AWS Elastic Beanstalk
- Docker Container
- Any .NET hosting

### Frontend
- Azure Static Web Apps
- Netlify
- Vercel
- Firebase Hosting
- AWS S3 + CloudFront

---

## 📝 Next Steps

1. **Customize**: Modify themes in `frontend/src/theme/variables.scss`
2. **Add Features**: Create new pages and services
3. **Deploy**: Follow production deployment guides
4. **Monitor**: Use Hangfire dashboard and application logs

---

## 🤝 Contributing

Ready to contribute? Push your changes to GitHub:

```bash
git add .
git commit -m "Your commit message"
git push origin main
```

---

## 📧 Support

For issues or questions:
- Check the README.md
- Review API documentation at /swagger
- Check browser console for errors
- Review backend logs

---

**Happy Monitoring! 🎉**
