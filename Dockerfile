FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/ShopSphere.Api/ShopSphere.Api.csproj backend/ShopSphere.Api/
RUN dotnet restore backend/ShopSphere.Api/ShopSphere.Api.csproj

COPY backend/ShopSphere.Api/ backend/ShopSphere.Api/
RUN dotnet publish backend/ShopSphere.Api/ShopSphere.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ShopSphere.Api.dll"]