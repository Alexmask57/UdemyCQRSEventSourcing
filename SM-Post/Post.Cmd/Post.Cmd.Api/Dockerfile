FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Post.Cmd.Api/Post.Cmd.Api.csproj", "Post.Cmd.Api/"]
RUN dotnet restore "Post.Cmd.Api/Post.Cmd.Api.csproj"
COPY . .
WORKDIR "/src/Post.Cmd.Api"
RUN dotnet build "Post.Cmd.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Post.Cmd.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Cmd.Api.dll"]
