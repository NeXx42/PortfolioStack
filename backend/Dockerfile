FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Portfolio.Api/*.csproj Portfolio.Api/
COPY Portfolio.Core/*.csproj Portfolio.Core/
COPY Portfolio.Data/*.csproj Portfolio.Data/

RUN dotnet restore Portfolio.Api/Portfolio.Api.csproj

COPY . .

RUN dotnet publish Portfolio.Api/Portfolio.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]