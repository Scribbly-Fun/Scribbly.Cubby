FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Scribbly.Cubby.Hosted/Scribbly.Cubby.Hosted.csproj", "Scribbly.Cubby.Hosted/"]
RUN dotnet restore "Scribbly.Cubby.Hosted/Scribbly.Cubby.Hosted.csproj"
COPY . .
WORKDIR "/src/Scribbly.Cubby.Hosted"
RUN dotnet build "./Scribbly.Cubby.Hosted.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Scribbly.Cubby.Hosted.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scribbly.Cubby.Hosted.dll"]
