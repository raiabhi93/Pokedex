#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src


# copy csproj and restore as distinct layers
COPY ["PokemonAPI/PokemonAPI.csproj", "PokemonAPI/"]
RUN dotnet restore "PokemonAPI/PokemonAPI.csproj"

# copy everything else and build app
COPY . .
WORKDIR "/src/PokemonAPI"
RUN dotnet build "PokemonAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PokemonAPI.csproj" -c Release -o /app/publish

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokemonAPI.dll"]