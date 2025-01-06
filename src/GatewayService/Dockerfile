FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80

# Copy csproj and restore as distinct layers
COPY car-shop-course-microservices.sln car-shop-course-microservices.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj

# Restore packages
RUN dotnet restore car-shop-course-microservices.sln

# Copy the app folders
COPY src/GatewayService src/GatewayService

# Publish the app
RUN dotnet publish "src/GatewayService/GatewayService.csproj" -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "GatewayService.dll" ]