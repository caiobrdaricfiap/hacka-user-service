# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY . /source
WORKDIR /source

RUN dotnet restore user-service/
RUN dotnet publish user-service/ -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y curl dpkg

COPY --from=build /source/out .

ENV ASPNETCORE_URLS=http://+:5230
EXPOSE 5230

# Entrypoint
ENTRYPOINT ["dotnet", "user-service.dll"]