FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine-amd64 as dotnet-builder

WORKDIR /src
COPY src/Tataru.csproj .
RUN dotnet restore Tataru.csproj -r linux-musl-x64
COPY src/ .
RUN dotnet publish Tataru.csproj -r linux-musl-x64 -c Release -o /build

FROM  mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine3.18-amd64 as dotnet-runner

EXPOSE 8080

WORKDIR /build
COPY --from=dotnet-builder /build .
ENTRYPOINT ["/build/Tataru"]