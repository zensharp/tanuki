FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine
ARG VERSION

WORKDIR /staging

# Install UV
RUN curl -LsSf https://astral.sh/uv/install.sh | sh
COPY zensical /staging/zensical
RUN uv --version
ENV PATH="$PATH:/root/.local/bin"

# Build application
COPY Tanuki /staging/src
RUN dotnet build src/Tanuki.csproj /p:Version="$VERSION" -c release
RUN dotnet pack src/Tanuki.csproj /p:Version="$VERSION" -c release -o /staging/out
RUN dotnet tool install --global --add-source /staging/out Tanuki --prerelease
ENV PATH="$PATH:/root/.dotnet/tools"
