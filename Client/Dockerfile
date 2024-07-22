FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MSClient/MSClient.csproj", "MSClient/"]
RUN dotnet restore "MSClient/MSClient.csproj"
COPY . .
WORKDIR "/src/MSClient"
RUN dotnet build "MSClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSClient.dll"]
