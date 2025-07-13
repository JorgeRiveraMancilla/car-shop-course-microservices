FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY car-shop-course-microservices.sln .
COPY Src/SearchService/SearchService.csproj ./Src/SearchService/
COPY Src/Contracts/Contracts.csproj ./Src/Contracts/

# Restore packages
RUN dotnet restore Src/SearchService/SearchService.csproj

# Copy source code
COPY Src/SearchService/ ./Src/SearchService/
COPY Src/Contracts/ ./Src/Contracts/

# Build and publish
RUN dotnet publish Src/SearchService/SearchService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SearchService.dll"]