FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY car-shop-course-microservices.sln .
COPY Src/IdentityService/IdentityService.csproj ./Src/IdentityService/

# Restore packages
RUN dotnet restore Src/IdentityService/IdentityService.csproj

# Copy source code
COPY Src/IdentityService/ ./Src/IdentityService/

# Build and publish
RUN dotnet publish Src/IdentityService/IdentityService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]