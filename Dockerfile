FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine
ARG VERSION

# Build application
WORKDIR /staging
COPY Tanuki /staging/src
COPY Tanuki/templates /data/templates
RUN dotnet build src/Tanuki.csproj /p:Version="$VERSION" -c release
RUN dotnet pack src/Tanuki.csproj /p:Version="$VERSION" -c release -o /staging/out
RUN dotnet tool install --global --add-source /staging/out Tanuki --prerelease
ENV PATH="$PATH:/root/.dotnet/tools"
