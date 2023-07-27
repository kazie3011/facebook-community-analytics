@echo off

set projectPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src
set publishScriptPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\publish-scripts

set env=partner
set publishDomain=gdll.vn.pubxml
set password=MQaLY5u0

echo ________________________________
echo *                              *
echo *      -Using partner config      *
echo *______________________________*
cd %publishScriptPath%
call update-appsettings-partner.bat
call update-globalconfigs-partner.bat
rem timeout /t 3

echo ________________________________
echo *                              *
echo *      -Start to publish-      *
echo *      -ENV: %env%             *
echo *______________________________*  


cd %projectPath%\FacebookCommunityAnalytics.Api.WebBackgroundJob\
msbuild /p:Configuration=Release /p:DeployOnBuild=true /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.job.%publishDomain% 

cd %projectPath%\FacebookCommunityAnalytics.Api.HttpApi.Host\
msbuild /p:Configuration=Release /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.api.%publishDomain% 

cd %projectPath%\FacebookCommunityAnalytics.Api.IdentityServer\
msbuild /p:Configuration=Release /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.identity.%publishDomain% 

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