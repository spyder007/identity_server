#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM node:12.19-slim AS node-builder
WORKDIR /src
COPY . .

WORKDIR /src/spydersoft.Identity
RUN npm install && npm install gulp-cli -g
RUN gulp build:dist


FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["spydersoft.Identity/spydersoft.Identity.csproj", "spydersoft.Identity/"]

RUN dotnet restore "spydersoft.Identity/spydersoft.Identity.csproj"
COPY . .
WORKDIR /src/spydersoft.Identity

RUN dotnet build "spydersoft.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "spydersoft.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node-builder /src/spydersoft.Identity/wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "spydersoft.Identity.dll"]