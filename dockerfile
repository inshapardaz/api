FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app

RUN dotnet restore src/Inshapardaz.Api.sln
RUN dotnet publish src/Inshapardaz.Api/Inshapardaz.Api.csproj -c Release -o ../../bin

EXPOSE 5000

ENTRYPOINT [ "dotnet", "bin/Inshapardaz.Api.dll" ]