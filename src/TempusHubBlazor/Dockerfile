FROM mcr.microsoft.com/dotnet/core/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /src
COPY ["TempusHubBlazor.csproj", ""]
RUN dotnet restore "./TempusHubBlazor.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TempusHubBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TempusHubBlazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "TempusHubBlazor.dll"]