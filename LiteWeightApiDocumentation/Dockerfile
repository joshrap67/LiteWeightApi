FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LiteWeightApiDocumentation/LiteWeightApiDocumentation.csproj", "LiteWeightApiDocumentation/"]
RUN dotnet restore "LiteWeightApiDocumentation/LiteWeightApiDocumentation.csproj"
COPY . .
WORKDIR "/src/LiteWeightApiDocumentation"
RUN dotnet build "LiteWeightApiDocumentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiteWeightApiDocumentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiteWeightApiDocumentation.dll"]
