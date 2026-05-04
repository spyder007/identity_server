FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM node:20-slim AS node-builder
WORKDIR /src
COPY . .
WORKDIR /src/Spydersoft.Identity
RUN npm install && npm install gulp-cli -g
RUN npm run production


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG GITHUB_TOKEN
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Directory.Packages.props", "."]
COPY ["Spydersoft.Identity/Spydersoft.Identity.csproj", "Spydersoft.Identity/"]
RUN if [ -n "$GITHUB_TOKEN" ]; then \
        dotnet nuget update source spydersoft \
            --username "docker" \
            --password "$GITHUB_TOKEN" \
            --store-password-in-clear-text; \
    fi
RUN dotnet restore "Spydersoft.Identity/Spydersoft.Identity.csproj"
COPY . .
WORKDIR /src/Spydersoft.Identity
RUN dotnet build "Spydersoft.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Spydersoft.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node-builder /src/Spydersoft.Identity/wwwroot ./wwwroot
USER $APP_UID
ENTRYPOINT ["dotnet", "Spydersoft.Identity.dll"]
