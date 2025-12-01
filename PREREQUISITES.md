# Prerequisites Installation Guide

This guide will help you install all the required tools to run the DevOps Dashboard.

---

## Required Tools

### 1. .NET 8.0 SDK (for Backend)

**macOS Installation:**

```bash
# Using Homebrew (recommended)
brew install dotnet-sdk

# Or download directly from Microsoft
# Visit: https://dotnet.microsoft.com/download/dotnet/8.0
```

**Verify Installation:**
```bash
dotnet --version
# Should show: 8.0.x
```

---

### 2. Node.js 18+ (for Frontend)

**macOS Installation:**

```bash
# Using Homebrew (recommended)
brew install node

# Or using nvm (Node Version Manager)
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
nvm install 18
nvm use 18
```

**Verify Installation:**
```bash
node --version
# Should show: v18.x.x or higher

npm --version
# Should show: 9.x.x or higher
```

---

### 3. Git (Already Installed ✅)

You already have Git installed since you've been using it.

---

## Installation Steps

### Step 1: Install Homebrew (if not installed)

```bash
# Check if Homebrew is installed
brew --version

# If not installed, install it:
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

### Step 2: Install .NET SDK

```bash
# Install .NET 8 SDK
brew install dotnet-sdk

# Verify installation
dotnet --version

# If you see a version number, you're good to go!
```

### Step 3: Install Node.js (if not already installed)

```bash
# Check current version
node --version

# If not installed or older than v18:
brew install node

# Verify
node --version
npm --version
```

---

## Post-Installation: Test Your Setup

### Backend Test

```bash
cd /Users/benh/Documents/DevOpsDashboard/backend
dotnet restore
dotnet build
dotnet run
```

If successful, you should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### Backend Tests

```bash
cd /Users/benh/Documents/DevOpsDashboard/backend.tests
dotnet test
```

### Frontend Test

```bash
cd /Users/benh/Documents/DevOpsDashboard/frontend
npm install
npm start
```

If successful, your browser will open to `http://localhost:8100`

---

## Quick Start Script

Save this as a script and run it:

```bash
#!/bin/bash

echo "🚀 Installing DevOps Dashboard Prerequisites..."

# Check Homebrew
if ! command -v brew &> /dev/null; then
    echo "📦 Installing Homebrew..."
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
fi

# Install .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "🔧 Installing .NET SDK..."
    brew install dotnet-sdk
else
    echo "✅ .NET SDK already installed: $(dotnet --version)"
fi

# Install Node.js
if ! command -v node &> /dev/null; then
    echo "📗 Installing Node.js..."
    brew install node
else
    echo "✅ Node.js already installed: $(node --version)"
fi

echo ""
echo "✅ All prerequisites installed!"
echo ""
echo "Next steps:"
echo "1. cd backend && dotnet run"
echo "2. cd frontend && npm install && npm start"
```

---

## Troubleshooting

### .NET SDK Installation Issues

**Error: "dotnet: command not found"**
- Restart your terminal after installation
- Check PATH: `echo $PATH`
- Manually add to PATH if needed:
  ```bash
  export PATH="$PATH:/usr/local/share/dotnet"
  ```

**Error: "The framework 'Microsoft.NETCore.App', version '8.0.0' was not found"**
- Install the correct SDK version:
  ```bash
  brew reinstall dotnet-sdk
  ```

### Node.js Installation Issues

**Error: "npm: command not found"**
- Node.js installation includes npm
- Reinstall: `brew reinstall node`

**Error: Permission denied**
- Fix npm permissions:
  ```bash
  sudo chown -R $(whoami) ~/.npm
  ```

---

## Alternative: Using Docker (No Installation Required)

If you prefer not to install SDKs locally, you can use Docker:

### Backend (Docker)
```bash
cd backend
docker build -t devops-dashboard-api .
docker run -p 5001:5001 devops-dashboard-api
```

### Frontend (Docker)
```bash
cd frontend
docker build -t devops-dashboard-web .
docker run -p 8100:8100 devops-dashboard-web
```

---

## Verify Everything Works

Run this verification script:

```bash
#!/bin/bash

echo "🔍 Checking installations..."
echo ""

# Check .NET
if command -v dotnet &> /dev/null; then
    echo "✅ .NET SDK: $(dotnet --version)"
else
    echo "❌ .NET SDK: Not installed"
fi

# Check Node.js
if command -v node &> /dev/null; then
    echo "✅ Node.js: $(node --version)"
else
    echo "❌ Node.js: Not installed"
fi

# Check npm
if command -v npm &> /dev/null; then
    echo "✅ npm: $(npm --version)"
else
    echo "❌ npm: Not installed"
fi

# Check Git
if command -v git &> /dev/null; then
    echo "✅ Git: $(git --version)"
else
    echo "❌ Git: Not installed"
fi

echo ""
echo "Installation complete! 🎉"
```

---

## Need Help?

- **.NET SDK Issues**: https://dotnet.microsoft.com/download/dotnet/8.0
- **Node.js Issues**: https://nodejs.org/
- **Homebrew Issues**: https://brew.sh/

---

**Once installed, return to SETUP.md for running the application!**
