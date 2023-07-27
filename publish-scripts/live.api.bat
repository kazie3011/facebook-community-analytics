@echo off

set projectPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src
set publishScriptPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\publish-scripts

set env=live
set publishDomain=gdll.vn.pubxml
set password=k23VQbw9P

echo ________________________________
echo *                              *
echo *      -Using LIVE config      *
echo *______________________________*
cd %publishScriptPath%
call update-appsettings-live.bat
call update-globalconfigs-live.bat
rem timeout /t 3

echo ________________________________
echo *                              *
echo *      -Start to publish-      *
echo *      -ENV: %env%             *
echo *______________________________*  

cd %projectPath%\FacebookCommunityAnalytics.Api.HttpApi.Host\
msbuild /p:Configuration=Release /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.api.%publishDomain% 

cd %projectPath%\FacebookCommunityAnalytics.Api.Blazor\
msbuild /p:Configuration=Release /p:DeployOnBuild=true /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.%publishDomain% 

echo ________________________________
echo *                              *
echo *      -Done publishing-       *
echo *______________________________*

echo ________________________________
echo *                              *
echo *    -Revert to DEV config     *
echo *______________________________*
cd %publishScriptPath%
call update-appsettings-dev.bat
call update-globalconfigs-dev.bat
Pause