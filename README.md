# Car Shop Course - Backend Microservices

Microservices architecture for the Car Shop Course application built with .NET 8, featuring authentication, auctions, search, and gateway services.

## 📚 Course Credits

This project is based on the excellent course by **Neil Cummings**:
**[Build a microservices app with .Net and NextJS from scratch](https://www.udemy.com/course/build-a-microservices-app-with-dotnet-and-nextjs-from-scratch)**

## 🛠️ Services Overview

### 🔐 Identity Service (Port 5001)
- **Technology:** ASP.NET Core + Duende IdentityServer 7
- **Database:** PostgreSQL
- **Purpose:** OAuth2/OIDC authentication and authorization

### 🚗 Auction Service (Port 7001)
- **Technology:** ASP.NET Core Web API
- **Database:** PostgreSQL
- **Purpose:** Manage vehicle auctions

### 🔍 Search Service (Port 7002)
- **Technology:** ASP.NET Core Web API
- **Database:** MongoDB
- **Purpose:** Search and filter auctions

### 🌐 Gateway Service (Port 6001)
- **Technology:** ASP.NET Core + YARP (Yet Another Reverse Proxy)
- **Purpose:** API Gateway and routing

## 📦 Infrastructure Components

### 🐘 PostgreSQL (Port 5432)
- **Purpose:** Primary database for Identity and Auction services
- **Credentials:**
  - Username: `postgres`
  - Password: `postgrespw`

### 🍃 MongoDB (Port 27018)
- **Purpose:** Search database with flexible schema
- **Credentials:**
  - Username: `root`
  - Password: `mongopw`

### 🐰 RabbitMQ (Port 5672/15672)
- **Purpose:** Message broker for inter-service communication
- **Management UI:** `http://localhost:15672`
- **Credentials:**
  - Username: `guest`
  - Password: `guest`

## 💻 Prerequisites

Before starting, make sure you have the following software installed:

- **Docker Desktop** - [Download from docker.com](https://www.docker.com/products/docker-desktop/)
- **Git** - [Download from git-scm.com](https://git-scm.com/)
- **.NET 8 SDK** (for local development) - [Download from dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **VS Code** - [Download from code.visualstudio.com](https://code.visualstudio.com/)

## 🚀 Getting Started

### 1. Clone Repository and Navigate

```bash
git clone <repository-url>
cd car-shop-course-microservices
```

### 2. Run with Docker Compose

```bash
# Start all services
docker-compose up -d
```

### 3. Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (reset databases)
docker-compose down -v
```