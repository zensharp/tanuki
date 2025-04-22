# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG VERSION
WORKDIR /staging

COPY Tanuki ./src
RUN dotnet build src/Tanuki.csproj /p:Version="$VERSION" -c release
RUN dotnet pack src/Tanuki.csproj /p:Version="$VERSION" -c release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /app
COPY --from=build /out /app
COPY Tanuki/templates/ /out/

RUN dotnet tool install --global --add-source out Tanuki --prerelease
ENV PATH="$PATH:/root/.dotnet/tools"
