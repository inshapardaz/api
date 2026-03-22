FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "src/Inshapardaz.Api/Inshapardaz.Api.csproj"
RUN dotnet publish "src/Inshapardaz.Api/Inshapardaz.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:5000/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "Inshapardaz.Api.dll"]
