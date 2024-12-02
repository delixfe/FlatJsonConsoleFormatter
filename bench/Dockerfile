
FROM mcr.microsoft.com/dotnet/sdk:8.0
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["bench/Benchmarks.csproj", "bench/"]
COPY ["src/FlatJsonConsoleFormatter/FlatJsonConsoleFormatter.csproj", "src/FlatJsonConsoleFormatter/"]
COPY ["src/JsonConsoleFormatters/JsonConsoleFormatters.csproj", "src/JsonConsoleFormatters/"]
RUN dotnet restore "./bench/Benchmarks.csproj"
COPY . .
WORKDIR "/src/bench"
RUN dotnet build "./Benchmarks.csproj" -c $BUILD_CONFIGURATION 
ENTRYPOINT dotnet run "./Benchmarks.csproj" -c $BUILD_CONFIGURATION --no-build
