#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5107

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NdfcAPIsMongoDB/NdfcAPIsMongoDB.csproj", "NdfcAPIsMongoDB/"]
RUN dotnet restore "NdfcAPIsMongoDB/NdfcAPIsMongoDB.csproj"
COPY . .
WORKDIR "/src/NdfcAPIsMongoDB"
RUN dotnet build "NdfcAPIsMongoDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NdfcAPIsMongoDB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NdfcAPIsMongoDB.dll"]