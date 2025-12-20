FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["source/Scribbly.Cubby.Host/Scribbly.Cubby.Host.csproj", "source/Scribbly.Cubby.Host/"]
RUN dotnet restore "source/Scribbly.Cubby.Host/Scribbly.Cubby.Host.csproj"
COPY . .
WORKDIR "/src/source/Scribbly.Cubby.Host"
RUN dotnet build "./Scribbly.Cubby.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Scribbly.Cubby.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scribbly.Cubby.Host.dll"]
