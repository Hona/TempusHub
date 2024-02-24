#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Empedo.Discord/Empedo.Discord.csproj", "src/Empedo.Discord/"]
RUN dotnet restore "src/Empedo.Discord/Empedo.Discord.csproj"
COPY . .
WORKDIR "/src/src/Empedo.Discord"
RUN dotnet build "Empedo.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Empedo.Discord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Empedo.Discord.dll"]