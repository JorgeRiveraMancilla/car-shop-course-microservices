FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY car-shop-course-microservices.sln .
COPY Src/GatewayService/GatewayService.csproj ./Src/GatewayService/

# Restore packages
RUN dotnet restore Src/GatewayService/GatewayService.csproj

# Copy source code
COPY Src/GatewayService/ ./Src/GatewayService/

# Build and publish
RUN dotnet publish Src/GatewayService/GatewayService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.dll"]