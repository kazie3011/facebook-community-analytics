@echo off
echo FacebookCommunityAnalytics scripts - copying globalconfigs.live.json to globalconfigs.json
echo -----------------------------------------------------
set env=live
echo Current Environment: %env%
set sourcePath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src

set webPath=%sourcePath%\Configs\
echo Update path: %webPath%
COPY /Y "%webPath%\globalconfigs.%env%.json" "%webPath%\globalconfigs.json"
