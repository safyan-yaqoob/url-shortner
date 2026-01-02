# Docker Setup and Testing Guide

## Overview
This setup includes:
- **API Instance 1**: Running on port 5001 (internal port 5000)
- **API Instance 2**: Running on port 5002 (internal port 5000)
- **YARP Gateway**: Running on port 8080, load balancing between the two API instances

## Prerequisites
- Docker Desktop installed and running
- Docker Compose installed

## Building and Running

### Build and Start All Services
```bash
docker-compose up --build
```

### Run in Detached Mode
```bash
docker-compose up -d --build
```

### Stop All Services
```bash
docker-compose down
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api1
docker-compose logs -f api2
docker-compose logs -f gateway
```

## Testing Load Balancing

### 1. Test Direct API Access
```bash
# API Instance 1
curl http://localhost:5001/api/ShortUrl

# API Instance 2
curl http://localhost:5002/api/ShortUrl
```

### 2. Test Through Gateway (Load Balanced)
```bash
# Create a short URL through gateway
curl -X POST http://localhost:8080/api/ShortUrl \
  -H "Content-Type: application/json" \
  -d '{"longUrl": "https://example.com"}'

# Get short URL through gateway (will be load balanced)
curl http://localhost:8080/api/ShortUrl/{shortCode}
```

### 3. Verify Load Balancing
Make multiple requests and check the logs to see requests distributed between api1 and api2:
```bash
# Run multiple requests
for i in {1..10}; do
  curl http://localhost:8080/api/ShortUrl/{shortCode}
  sleep 1
done

# Check logs to see which instance handled each request
docker-compose logs api1 | grep "GET"
docker-compose logs api2 | grep "GET"
```

### 4. Test Health Endpoint
```bash
# Health check on individual instances
curl http://localhost:5001/health
curl http://localhost:5002/health

# Health check through gateway
curl http://localhost:8080/health
```

## Load Balancing Policy
The YARP gateway uses **RoundRobin** load balancing, which distributes requests evenly between the two API instances.

## Network Architecture
```
Client Request → Gateway (8080) → Load Balancer → api1 (5000) or api2 (5000)
                                      ↓
                              RoundRobin Policy
```

## Troubleshooting

### Check if containers are running
```bash
docker-compose ps
```

### Restart a specific service
```bash
docker-compose restart api1
docker-compose restart api2
docker-compose restart gateway
```

### Rebuild a specific service
```bash
docker-compose build api1
docker-compose up -d api1
```

### Check container logs for errors
```bash
docker-compose logs api1
docker-compose logs api2
docker-compose logs gateway
```

