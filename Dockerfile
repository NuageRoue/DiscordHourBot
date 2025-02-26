# Étape de base : utiliser l'image .NET 6 pour l'exécution
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Étape de build : utiliser l'image .NET 6 SDK pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DiscordBot/DiscordBot.csproj", "DiscordBot/"]
RUN dotnet restore "DiscordBot/DiscordBot.csproj"
COPY . .
WORKDIR "/src/DiscordBot"
RUN dotnet build "DiscordBot.csproj" -c Release -o /app/build

# Étape de publication : publier l'application
FROM build AS publish
RUN dotnet publish "DiscordBot.csproj" -c Release -o /app/publish

# Étape finale : utiliser l'image de base et copier les fichiers publiés
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.dll"]
