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
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
RUN powershell -Command \
    wget -Uri https://nodejs.org/dist/v%NODE_VERSION%/node-v%NODE_VERSION%-x64.msi -OutFile node.msi -UseBasicParsing ; \
    if ((Get-FileHash node.msi -Algorithm sha256).Hash -ne $env:NODE_DOWNLOAD_SHA) {exit 1} ; \
    Start-Process -FilePath msiexec -ArgumentList /q, /i, node.msi -Wait ; \
    Remove-Item -Path node.msi

RUN npm install
RUN npm install gulp
RUN gulp build:dist

RUN dotnet build "spydersoft.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "spydersoft.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "spydersoft.Identity.dll"]