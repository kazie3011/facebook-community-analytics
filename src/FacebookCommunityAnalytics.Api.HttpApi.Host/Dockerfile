#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
#COPY ["nuget.config", "."]
COPY NuGet.Config /
COPY ["src/FacebookCommunityAnalytics.Api.HttpApi.Host/FacebookCommunityAnalytics.Api.HttpApi.Host.csproj", "src/FacebookCommunityAnalytics.Api.HttpApi.Host/"]
COPY ["src/FacebookCommunityAnalytics.Api.MongoDB/FacebookCommunityAnalytics.Api.MongoDB.csproj", "src/FacebookCommunityAnalytics.Api.MongoDB/"]
COPY ["src/FacebookCommunityAnalytics.Api.Domain/FacebookCommunityAnalytics.Api.Domain.csproj", "src/FacebookCommunityAnalytics.Api.Domain/"]
COPY ["src/FacebookCommunityAnalytics.Api.Domain.Shared/FacebookCommunityAnalytics.Api.Domain.Shared.csproj", "src/FacebookCommunityAnalytics.Api.Domain.Shared/"]
COPY ["src/FacebookCommunityAnalytics.Api.Core/FacebookCommunityAnalytics.Api.Core.csproj", "src/FacebookCommunityAnalytics.Api.Core/"]
COPY ["src/FacebookCommunityAnalytics.Api.HttpApi/FacebookCommunityAnalytics.Api.HttpApi.csproj", "src/FacebookCommunityAnalytics.Api.HttpApi/"]
COPY ["src/FacebookCommunityAnalytics.Api.Application.Contracts/FacebookCommunityAnalytics.Api.Application.Contracts.csproj", "src/FacebookCommunityAnalytics.Api.Application.Contracts/"]
COPY ["src/FacebookCommunityAnalytics.Api.Application/FacebookCommunityAnalytics.Api.Application.csproj", "src/FacebookCommunityAnalytics.Api.Application/"]
RUN dotnet restore "src/FacebookCommunityAnalytics.Api.HttpApi.Host/FacebookCommunityAnalytics.Api.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/FacebookCommunityAnalytics.Api.HttpApi.Host"
RUN dotnet build "FacebookCommunityAnalytics.Api.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacebookCommunityAnalytics.Api.HttpApi.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacebookCommunityAnalytics.Api.HttpApi.Host.dll"]
