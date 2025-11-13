FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy everything
COPY . .
RUN dotnet restore
# Build and publish a release
RUN dotnet publish Bonjour --no-restore -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 5259 7255

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Bonjour.dll"]