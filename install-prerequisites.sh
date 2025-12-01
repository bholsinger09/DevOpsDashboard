#!/bin/bash

echo "🚀 DevOps Dashboard - Prerequisites Installer"
echo "=============================================="
echo ""

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo -e "${RED}❌ This script is designed for macOS${NC}"
    echo "Please install prerequisites manually:"
    echo "  - .NET 8 SDK: https://dotnet.microsoft.com/download"
    echo "  - Node.js 18+: https://nodejs.org/"
    exit 1
fi

# Function to check if command exists
command_exists() {
    command -v "$1" &> /dev/null
}

# Check and install Homebrew
echo "📦 Checking Homebrew..."
if ! command_exists brew; then
    echo -e "${YELLOW}Homebrew not found. Installing...${NC}"
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    
    # Add Homebrew to PATH for Apple Silicon Macs
    if [[ $(uname -m) == 'arm64' ]]; then
        echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
        eval "$(/opt/homebrew/bin/brew shellenv)"
    fi
else
    echo -e "${GREEN}✅ Homebrew is installed: $(brew --version | head -n1)${NC}"
fi

echo ""

# Check and install .NET SDK
echo "🔧 Checking .NET SDK..."
if ! command_exists dotnet; then
    echo -e "${YELLOW}Installing .NET SDK 8.0...${NC}"
    brew install dotnet-sdk
    
    # Verify installation
    if command_exists dotnet; then
        echo -e "${GREEN}✅ .NET SDK installed successfully: $(dotnet --version)${NC}"
    else
        echo -e "${RED}❌ Failed to install .NET SDK${NC}"
        echo "Please install manually from: https://dotnet.microsoft.com/download/dotnet/8.0"
    fi
else
    VERSION=$(dotnet --version)
    echo -e "${GREEN}✅ .NET SDK is already installed: $VERSION${NC}"
    
    # Check if version is 8.x
    MAJOR_VERSION=$(echo $VERSION | cut -d. -f1)
    if [[ $MAJOR_VERSION -lt 8 ]]; then
        echo -e "${YELLOW}⚠️  Warning: .NET SDK version is older than 8.0${NC}"
        echo "Consider upgrading: brew upgrade dotnet-sdk"
    fi
fi

echo ""

# Check and install Node.js
echo "📗 Checking Node.js..."
if ! command_exists node; then
    echo -e "${YELLOW}Installing Node.js...${NC}"
    brew install node
    
    # Verify installation
    if command_exists node; then
        echo -e "${GREEN}✅ Node.js installed successfully: $(node --version)${NC}"
        echo -e "${GREEN}✅ npm installed: $(npm --version)${NC}"
    else
        echo -e "${RED}❌ Failed to install Node.js${NC}"
        echo "Please install manually from: https://nodejs.org/"
    fi
else
    NODE_VERSION=$(node --version | cut -d'v' -f2 | cut -d'.' -f1)
    echo -e "${GREEN}✅ Node.js is already installed: $(node --version)${NC}"
    echo -e "${GREEN}✅ npm version: $(npm --version)${NC}"
    
    # Check if version is 18+
    if [[ $NODE_VERSION -lt 18 ]]; then
        echo -e "${YELLOW}⚠️  Warning: Node.js version is older than 18${NC}"
        echo "Consider upgrading: brew upgrade node"
    fi
fi

echo ""

# Check Git
echo "🔀 Checking Git..."
if command_exists git; then
    echo -e "${GREEN}✅ Git is installed: $(git --version)${NC}"
else
    echo -e "${RED}❌ Git is not installed${NC}"
    echo "Installing Git..."
    brew install git
fi

echo ""
echo "=============================================="
echo "📊 Installation Summary:"
echo "=============================================="
echo ""

# Summary
ALL_GOOD=true

if command_exists dotnet; then
    echo -e "${GREEN}✅ .NET SDK: $(dotnet --version)${NC}"
else
    echo -e "${RED}❌ .NET SDK: Not installed${NC}"
    ALL_GOOD=false
fi

if command_exists node; then
    echo -e "${GREEN}✅ Node.js: $(node --version)${NC}"
else
    echo -e "${RED}❌ Node.js: Not installed${NC}"
    ALL_GOOD=false
fi

if command_exists npm; then
    echo -e "${GREEN}✅ npm: $(npm --version)${NC}"
else
    echo -e "${RED}❌ npm: Not installed${NC}"
    ALL_GOOD=false
fi

if command_exists git; then
    echo -e "${GREEN}✅ Git: $(git --version)${NC}"
else
    echo -e "${RED}❌ Git: Not installed${NC}"
    ALL_GOOD=false
fi

echo ""

if [ "$ALL_GOOD" = true ]; then
    echo -e "${GREEN}🎉 All prerequisites are installed!${NC}"
    echo ""
    echo "Next steps:"
    echo "  1. Backend: cd backend && dotnet restore && dotnet run"
    echo "  2. Frontend: cd frontend && npm install && npm start"
    echo "  3. Tests: cd backend.tests && dotnet test"
    echo ""
    echo "For detailed instructions, see SETUP.md"
else
    echo -e "${RED}⚠️  Some prerequisites are missing${NC}"
    echo "Please check the errors above and install manually if needed"
    echo ""
    echo "Resources:"
    echo "  - .NET SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo "  - Node.js: https://nodejs.org/"
    echo "  - See PREREQUISITES.md for detailed instructions"
fi

echo ""
