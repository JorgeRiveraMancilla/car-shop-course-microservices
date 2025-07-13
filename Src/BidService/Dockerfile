FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY car-shop-course-microservices.sln .
COPY Src/BidService/BidService.csproj ./Src/BidService/
COPY Src/Contracts/Contracts.csproj ./Src/Contracts/

# Restore packages
RUN dotnet restore Src/BidService/BidService.csproj

# Copy source code
COPY Src/BidService/ ./Src/BidService/
COPY Src/Contracts/ ./Src/Contracts/

# Build and publish
RUN dotnet publish Src/BidService/BidService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BidService.dll"]