#!/bin/bash

echo "🚀 DevOps Dashboard - Heroku Deployment"
echo "========================================"
echo ""

# Check if Heroku CLI is installed
if ! command -v heroku &> /dev/null; then
    echo "❌ Heroku CLI is not installed."
    echo ""
    echo "Install with: brew tap heroku/brew && brew install heroku"
    exit 1
fi

echo "✅ Heroku CLI is installed"
echo ""

# Check if logged in
if ! heroku auth:whoami &> /dev/null; then
    echo "🔐 Please login to Heroku..."
    heroku login
fi

echo "✅ Logged in to Heroku"
echo ""

# Create backend app
echo "📦 Creating backend app..."
BACKEND_APP="devops-dashboard-api-$(date +%s)"
heroku create $BACKEND_APP --region us

if [ $? -ne 0 ]; then
    echo "⚠️  App name might be taken, using auto-generated name..."
    BACKEND_APP=$(heroku create --region us --json | jq -r '.name')
fi

echo "✅ Backend app created: $BACKEND_APP"

# Add .NET buildpack
echo "🔨 Adding .NET buildpack..."
heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack -a $BACKEND_APP

# Set config vars
heroku config:set ASPNETCORE_ENVIRONMENT=Production -a $BACKEND_APP

# Deploy backend
echo "🚀 Deploying backend..."
git subtree push --prefix backend https://git.heroku.com/$BACKEND_APP.git main || \
  git push https://git.heroku.com/$BACKEND_APP.git `git subtree split --prefix backend main`:main --force

BACKEND_URL=$(heroku apps:info -a $BACKEND_APP --json | jq -r '.app.web_url')
echo "✅ Backend deployed to: $BACKEND_URL"
echo ""

# Create frontend app
echo "📦 Creating frontend app..."
FRONTEND_APP="devops-dashboard-web-$(date +%s)"
heroku create $FRONTEND_APP --region us

if [ $? -ne 0 ]; then
    echo "⚠️  App name might be taken, using auto-generated name..."
    FRONTEND_APP=$(heroku create --region us --json | jq -r '.name')
fi

echo "✅ Frontend app created: $FRONTEND_APP"

# Update frontend environment with backend URL
echo "🔧 Updating frontend configuration..."
cat > frontend/src/environments/environment.heroku.ts << EOF
export const environment = {
  production: true,
  apiUrl: '${BACKEND_URL}api',
  hubUrl: '${BACKEND_URL}dashboardHub'
};
EOF

# Update angular.json to add heroku configuration
echo "🔧 Configuring frontend build..."

# Deploy frontend
echo "🚀 Deploying frontend..."
cd frontend

# Create temporary package.json with express
cat > package.json.tmp << 'EOF'
{
  "name": "devops-dashboard-web",
  "version": "1.0.0",
  "engines": {
    "node": "18.x",
    "npm": "9.x"
  },
  "scripts": {
    "postinstall": "npm install --legacy-peer-deps && npm run build",
    "build": "ng build --configuration production",
    "start": "node server.js"
  },
  "dependencies": {
    "express": "^4.18.2"
  }
}
EOF

mv package.json package.json.backup
mv package.json.tmp package.json

cd ..
git add frontend/src/environments/environment.heroku.ts frontend/package.json
git commit -m "Configure for Heroku deployment"

git subtree push --prefix frontend https://git.heroku.com/$FRONTEND_APP.git main || \
  git push https://git.heroku.com/$FRONTEND_APP.git `git subtree split --prefix frontend main`:main --force

cd frontend
mv package.json.backup package.json
cd ..

FRONTEND_URL=$(heroku apps:info -a $FRONTEND_APP --json | jq -r '.app.web_url')

echo ""
echo "=============================================="
echo "✅ Deployment Complete!"
echo "=============================================="
echo ""
echo "Backend API:  $BACKEND_URL"
echo "Frontend App: $FRONTEND_URL"
echo ""
echo "📱 You can now access from your iPhone!"
echo ""
echo "Useful commands:"
echo "  heroku logs --tail -a $BACKEND_APP   # View backend logs"
echo "  heroku logs --tail -a $FRONTEND_APP  # View frontend logs"
echo "  heroku open -a $FRONTEND_APP         # Open in browser"
echo ""
