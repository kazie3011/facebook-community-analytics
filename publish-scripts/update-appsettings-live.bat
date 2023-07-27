@echo off
echo FacebookCommunityAnalytics scripts - copying appsettings.dev.json to appsettings.json
echo -----------------------------------------------------
set env=live
echo Current Environment: %env%
set sourcePath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src

set webPath=%sourcePath%\FacebookCommunityAnalytics.Api.Blazor
echo Update path: %webPath%
COPY /Y "%webPath%\appsettings.%env%.json" "%webPath%\appsettings.json"

set webPath=%sourcePath%\FacebookCommunityAnalytics.Api.IdentityServer
echo Update path: %webPath%
COPY /Y "%webPath%\appsettings.%env%.json" "%webPath%\appsettings.json"

set webPath=%sourcePath%\FacebookCommunityAnalytics.Api.HttpApi.Host
echo Update path: %webPath%
COPY /Y "%webPath%\appsettings.%env%.json" "%webPath%\appsettings.json"

set webPath=%sourcePath%\FacebookCommunityAnalytics.Api.WebBackgroundJob
echo Update path: %webPath%
COPY /Y "%webPath%\appsettings.%env%.json" "%webPath%\appsettings.json"