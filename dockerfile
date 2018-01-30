FROM microsoft/aspnetcore-build:2.0.0 AS build-env

WORKDIR /app

COPY . .

RUN dotnet restore src/Inshapardaz.Api.sln
RUN dotnet publish src/Inshapardaz.Api/Inshapardaz.Api.csproj -c Release -o /output

# build runtime image
FROM microsoft/aspnetcore:2.0.0 

COPY --from=build-env /output /app

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 5000

ENTRYPOINT [ "dotnet", "Inshapardaz.Api.dll" ]