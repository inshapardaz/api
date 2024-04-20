#BACKEND SERVICE
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

ARG asp_environment
ARG build_number
ENV ASPNETCORE_ENVIRONMENT=$asp_environment

COPY *.sln  ./
COPY src/**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# COPY tests/**/*.csproj  ./
# RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

RUN ls
RUN dotnet restore

COPY . .

# run the test
#RUN dotnet test --results-directory /testresults --logger "trx;LogFileName=test_results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/testresults/coverage/ /p:Exclude="[Tests.*]" "tests/Dashboards.Api.Tests/Dashboards.Api.Tests.csproj" 

RUN dotnet publish src/Inshapardaz.Api/Inshapardaz.Api.csproj -r linux-x64 -c Release

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS pubish
WORKDIR /app

RUN apt-get update
RUN apt-get install -y libunwind-dev

COPY --from=build /app/src/Inshapardaz.Api/bin/Release/net7.0/linux-x64/publish/ .
RUN echo "1.0.0.$build_number" >> version.txt
# Expose the API port
EXPOSE 80

# Set the entry point for the API
ENTRYPOINT ["dotnet", "Inshapardaz.Api.dll"]
