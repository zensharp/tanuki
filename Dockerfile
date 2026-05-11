FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine
ARG VERSION

WORKDIR /staging

# Zensical
COPY zensical /staging/zensical
WORKDIR /staging/zensical
## Install pip
RUN apk add --no-cache python3 py3-pip
## Install zensical
RUN python3 -m venv .venv

# Dotnet
WORKDIR /staging
## Pack dotnet project
COPY Tanuki /staging/src
RUN dotnet build src/Tanuki.csproj /p:Version="$VERSION" -c release
RUN dotnet pack src/Tanuki.csproj /p:Version="$VERSION" -c release -o /staging/out
## Install as global tool
RUN dotnet tool install --global --add-source /staging/out Tanuki --prerelease
ENV PATH="$PATH:/root/.dotnet/tools"
