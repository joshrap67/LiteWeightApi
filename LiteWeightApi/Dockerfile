FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /LiteWeightApi

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /LiteWeightApi
COPY --from=build-env /LiteWeightApi/out .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LiteWeightApi.dll"]
