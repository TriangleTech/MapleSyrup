FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MapleSharp/MapleSharp.csproj", "MapleSharp/"]
RUN dotnet restore "MapleSharp/MapleSharp.csproj"
COPY . .
WORKDIR "/src/MapleSharp"
RUN dotnet build "MapleSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MapleSharp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MapleSharp.dll"]
