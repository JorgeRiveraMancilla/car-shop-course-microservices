FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /Src
EXPOSE 80

# Copy csproj and restore as distinct layers
COPY car-shop-course-microservices.sln car-shop-course-microservices.sln
COPY Src/AuctionService/AuctionService.csproj Src/AuctionService/AuctionService.csproj
COPY Src/SearchService/SearchService.csproj Src/SearchService/SearchService.csproj
COPY Src/GatewayService/GatewayService.csproj Src/GatewayService/GatewayService.csproj
COPY Src/Contracts/Contracts.csproj Src/Contracts/Contracts.csproj
COPY Src/IdentityService/IdentityService.csproj Src/IdentityService/IdentityService.csproj

# Restore packages
RUN dotnet restore car-shop-course-microservices.sln

# Copy the app folders
COPY Src/SearchService Src/SearchService
COPY Src/Contracts Src/Contracts

# Publish the app
RUN dotnet publish "Src/SearchService/SearchService.csproj" -c Release -o /app/Src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Src/out .
ENTRYPOINT [ "dotnet", "SearchService.dll" ]