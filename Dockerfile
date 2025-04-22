FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG VERSION

# Build application
WORKDIR /staging
COPY Tanuki /staging/src
RUN dotnet build src/Tanuki.csproj /p:Version="$VERSION" -c release
RUN dotnet pack src/Tanuki.csproj /p:Version="$VERSION" -c release -o /staging/out
RUN dotnet tool install --global --add-source /staging/out Tanuki --prerelease
ENV PATH="$PATH:/root/.dotnet/tools"

# Change PWD to app folder
WORKDIR /app
COPY Tanuki/templates /app/templates
