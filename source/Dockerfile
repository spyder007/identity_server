#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-nanoserver-1809 AS build
WORKDIR /src
COPY ["spydersoft.Identity/spydersoft.Identity.csproj", "spydersoft.Identity/"]

RUN dotnet restore "spydersoft.Identity/spydersoft.Identity.csproj"
COPY . .
WORKDIR "/src/spydersoft.Identity"

## Could try a multi-arch build here too:  https://github.com/aspnet/aspnet-docker/tree/master/2.1
FROM stefanscherer/node-windows:10.15.3-nanoserver-1809 AS node-builder
WORKDIR /src
COPY . .

RUN npm install
RUN npm install gulp-cli -g
RUN gulp build:dist

RUN dotnet build "spydersoft.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "spydersoft.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node-builder /src/spydersoft.Identity/wwwroot .
ENTRYPOINT ["dotnet", "spydersoft.Identity.dll"]