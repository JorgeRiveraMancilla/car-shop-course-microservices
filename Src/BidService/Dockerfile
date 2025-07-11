FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY Carsties.sln ./
COPY Src/BidService/BidService.csproj ./Src/BidService/
COPY Src/Contracts/Contracts.csproj ./Src/Contracts/

RUN dotnet restore ./Src/BidService/BidService.csproj

COPY . . 
RUN dotnet publish ./Src/BidService/BidService.csproj -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BidService.dll"]