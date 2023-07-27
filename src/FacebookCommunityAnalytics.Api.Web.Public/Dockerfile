#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
#COPY ["nuget.config", "."]
COPY NuGet.Config /
COPY ["src/FacebookCommunityAnalytics.Api.Web.Public/FacebookCommunityAnalytics.Api.Web.Public.csproj", "src/FacebookCommunityAnalytics.Api.Web.Public/"]
COPY ["src/FacebookCommunityAnalytics.Api.HttpApi/FacebookCommunityAnalytics.Api.HttpApi.csproj", "src/FacebookCommunityAnalytics.Api.HttpApi/"]
COPY ["src/FacebookCommunityAnalytics.Api.Application.Contracts/FacebookCommunityAnalytics.Api.Application.Contracts.csproj", "src/FacebookCommunityAnalytics.Api.Application.Contracts/"]
COPY ["src/FacebookCommunityAnalytics.Api.Domain.Shared/FacebookCommunityAnalytics.Api.Domain.Shared.csproj", "src/FacebookCommunityAnalytics.Api.Domain.Shared/"]
COPY ["src/FacebookCommunityAnalytics.Api.Core/FacebookCommunityAnalytics.Api.Core.csproj", "src/FacebookCommunityAnalytics.Api.Core/"]
COPY ["src/FacebookCommunityAnalytics.Api.HttpApi.Client/FacebookCommunityAnalytics.Api.HttpApi.Client.csproj", "src/FacebookCommunityAnalytics.Api.HttpApi.Client/"]
RUN dotnet restore "src/FacebookCommunityAnalytics.Api.Web.Public/FacebookCommunityAnalytics.Api.Web.Public.csproj"
COPY . .
WORKDIR "/src/src/FacebookCommunityAnalytics.Api.Web.Public"
RUN dotnet build "FacebookCommunityAnalytics.Api.Web.Public.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacebookCommunityAnalytics.Api.Web.Public.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacebookCommunityAnalytics.Api.Web.Public.dll"]
