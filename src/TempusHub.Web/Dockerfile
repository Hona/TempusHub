#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TempusHub.Web/TempusHub.Web.csproj", "src/TempusHub.Web/"]
COPY ["src/TempusHub.Application/TempusHub.Application.csproj", "src/TempusHub.Application/"]
COPY ["src/TempusHub.Core/TempusHub.Core.csproj", "src/TempusHub.Core/"]
COPY ["src/TempusHub.Infrastructure/TempusHub.Infrastructure.csproj", "src/TempusHub.Infrastructure/"]
RUN dotnet restore "src/TempusHub.Web/TempusHub.Web.csproj"
COPY . .
WORKDIR "/src/src/TempusHub.Web"
RUN dotnet build "TempusHub.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TempusHub.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TempusHub.Web.dll"]