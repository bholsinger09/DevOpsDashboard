# DevOps Dashboard

A comprehensive DevOps monitoring dashboard built with Ionic (Angular) for the frontend and ASP.NET Core Web API for the backend. The application is a Progressive Web App (PWA) with real-time updates via SignalR.

## Features

- **Server Monitoring**: Track server uptime, status, and response times
- **GitHub Integration**: View issues and pull requests from your repositories
- **Deployment Tracking**: Monitor deployment status and history
- **System Logs**: Centralized logging with filtering and search
- **Real-time Updates**: Live data updates using SignalR
- **Background Jobs**: Automated monitoring with Hangfire
- **PWA Support**: Install on any device, works offline

## Technology Stack

### Backend
- ASP.NET Core 8.0 Web API
- Entity Framework Core with SQLite
- SignalR for real-time communication
- Hangfire for background job processing
- Octokit for GitHub API integration
- Swagger for API documentation

### Frontend
- Ionic 7 with Angular 17
- Progressive Web App (PWA)
- SignalR client for real-time updates
- Responsive design for mobile and desktop

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- Git

## Getting Started

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update `appsettings.json` with your GitHub token:
   ```json
   {
     "GitHub": {
       "Token": "your_github_token_here",
       "Owner": "bholsinger09",
       "Repository": "DevOpsDashboard"
     }
   }
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

   The API will be available at `https://localhost:5001`
   Swagger documentation: `https://localhost:5001/swagger`
   Hangfire dashboard: `https://localhost:5001/hangfire`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Update API endpoints in `src/environments/environment.ts` if needed:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:5001/api',
     hubUrl: 'https://localhost:5001/dashboardHub'
   };
   ```

4. Start the development server:
   ```bash
   npm start
   ```

   The app will be available at `http://localhost:8100`

## Project Structure

```
DevOpsDashboard/
├── backend/
│   ├── Controllers/        # API Controllers
│   ├── Data/              # Database Context
│   ├── Hubs/              # SignalR Hubs
│   ├── Models/            # Domain Models
│   ├── Services/          # Business Logic
│   ├── Program.cs         # Application Entry Point
│   └── appsettings.json   # Configuration
│
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── pages/     # Page Components
│   │   │   ├── services/  # Angular Services
│   │   │   └── app.component.*
│   │   ├── environments/  # Environment Config
│   │   ├── theme/        # Styles
│   │   ├── index.html
│   │   └── manifest.webmanifest
│   ├── angular.json
│   ├── package.json
│   └── ngsw-config.json  # Service Worker Config
│
└── README.md
```

## API Endpoints

### Servers
- `GET /api/servers` - Get all servers
- `POST /api/servers` - Create a server
- `PUT /api/servers/{id}` - Update a server
- `DELETE /api/servers/{id}` - Delete a server
- `GET /api/servers/stats` - Get server statistics

### GitHub
- `GET /api/github/issues` - Get GitHub issues
- `GET /api/github/pullrequests` - Get pull requests
- `POST /api/github/sync` - Sync GitHub data
- `GET /api/github/stats` - Get GitHub statistics

### Deployments
- `GET /api/deployments` - Get deployments
- `POST /api/deployments` - Create a deployment
- `PUT /api/deployments/{id}` - Update deployment
- `GET /api/deployments/stats` - Get deployment statistics

### Logs
- `GET /api/logs` - Get system logs
- `POST /api/logs` - Create a log entry
- `DELETE /api/logs/{id}` - Delete a log
- `DELETE /api/logs/cleanup` - Cleanup old logs
- `GET /api/logs/stats` - Get log statistics

## Real-time Events

The SignalR hub broadcasts these events:
- `ReceiveServerStatusUpdate` - Server status changes
- `ReceiveDeploymentNotification` - Deployment updates
- `ReceiveLogNotification` - Critical log entries
- `ReceiveGitHubUpdate` - GitHub data sync completion

## Background Jobs

Hangfire runs these recurring jobs:
- **Server Uptime Check**: Every minute - checks all servers
- **GitHub Sync**: Every 5 minutes - syncs issues and PRs

## PWA Features

- Offline support with service workers
- App installation on mobile/desktop
- Push notifications (ready for implementation)
- Caching strategies for API responses

## Building for Production

### Backend
```bash
cd backend
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd frontend
npm run build
```

The production build will be in `frontend/www/`

## Deployment

### Backend
Deploy to Azure App Service, AWS, or any hosting that supports ASP.NET Core

### Frontend
Deploy to:
- Azure Static Web Apps
- Netlify
- Vercel
- Firebase Hosting
- Any static hosting service

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

MIT

## Author

DevOps Team
