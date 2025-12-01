#!/bin/bash

echo "🚀 DevOps Dashboard - Quick Deploy Script"
echo "=========================================="
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker Desktop first."
    echo "   Download from: https://www.docker.com/products/docker-desktop"
    exit 1
fi

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo "⚠️  Docker Desktop is not running. Starting it now..."
    open -a Docker
    echo "⏳ Waiting for Docker to start (this may take 30-60 seconds)..."
    
    # Wait for Docker daemon to be ready
    for i in {1..60}; do
        if docker info &> /dev/null; then
            echo "✅ Docker is ready!"
            break
        fi
        if [ $i -eq 60 ]; then
            echo "❌ Docker failed to start. Please start Docker Desktop manually and try again."
            exit 1
        fi
        sleep 2
        echo -n "."
    done
    echo ""
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed."
    exit 1
fi

echo "✅ Docker and Docker Compose are ready"
echo ""

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Build images
echo "🔨 Building Docker images..."
docker-compose build

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ Build successful!"
echo ""

# Start containers
echo "🚀 Starting containers..."
docker-compose up -d

if [ $? -ne 0 ]; then
    echo "❌ Failed to start containers!"
    exit 1
fi

echo ""
echo "✅ Deployment successful!"
echo ""
echo "📊 Your DevOps Dashboard is now running:"
echo "   Frontend:  http://localhost"
echo "   Backend:   http://localhost:5000"
echo "   Swagger:   http://localhost:5000/swagger"
echo ""
echo "📝 View logs with: docker-compose logs -f"
echo "🛑 Stop with: docker-compose down"
echo ""

# Show container status
echo "Container Status:"
docker-compose ps
