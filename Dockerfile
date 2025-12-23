# --------------------------------
# Build + Publish stage
# --------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        clang \
        zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Copy everything
COPY . .

# Publish AOT into /app
WORKDIR /src/source/Scribbly.Cubby.Host
RUN dotnet publish \
    -c $BUILD_CONFIGURATION \
    -r linux-x64 \
    -p:PublishAot=true \
    -p:SelfContained=true \
    -p:StripSymbols=true \
    -o /app

# --------------------------------
# Final runtime stage
# --------------------------------
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final

WORKDIR /app

COPY --from=build /app .

ENV SCRB_CUBBY_HTTPS=true \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["Scribbly.Cubby.Host.dll"]

