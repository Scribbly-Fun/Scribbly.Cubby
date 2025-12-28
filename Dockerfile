## --------------------------------
## Build Application
## --------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# NativeAOT prerequisites
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /repo

COPY . .

RUN dotnet publish source/Scribbly.Cubby.Host/Scribbly.Cubby.Host.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishAot=true \
    -p:StripSymbols=true \
    -p:InvariantGlobalization=true \
    -o /app

## --------------------------------
## Run Application
## --------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080
EXPOSE 8081

RUN ls -l /app
RUN chmod +x /app/Scribbly.Cubby.Host.dll

ENTRYPOINT ["dotnet", "/app/Scribbly.Cubby.Host.dll"]