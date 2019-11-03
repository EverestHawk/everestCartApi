FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR .
COPY *.sln .
COPY src/ApplicationCore/ApplicationCore.csproj src/ApplicationCore/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Services/Services.csproj src/Services/
COPY src/Webapi/webapi.csproj src/Webapi/

RUN dotnet restore /src/ApplicationCore/ApplicationCore.csproj
RUN dotnet restore /src/Infrastructure/Infrastructure.csproj
RUN dotnet restore /src/Services/Services.csproj
RUN dotnet restore /src/Webapi/webapi.csproj
COPY . .

WORKDIR /src/ApplicationCore
RUN dotnet build -c Release -o /app
WORKDIR /src/Infrastructure
RUN dotnet build -c Release -o /app
WORKDIR /src/Services
RUN dotnet build -c Release -o /app
WORKDIR /src/Webapi
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webapi.dll"]