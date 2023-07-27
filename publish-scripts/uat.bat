@echo off

set projectPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src
set publishScriptPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\publish-scripts

set env=uat
set publishDomain=gdll.vn.pubxml
set password=TYgC@PzmS6L*

echo ________________________________
echo *                              *
echo *      -Using UAT config      *
echo *______________________________*
cd %publishScriptPath%
call update-appsettings-uat.bat
call update-globalconfigs-uat.bat
rem timeout /t 3

echo ________________________________
echo *                              *
echo *      -Start to publish-      *
echo *      -ENV: %env%             *
echo *______________________________*  

cd %projectPath%\FacebookCommunityAnalytics.Api.HttpApi.Host\
msbuild /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.api.%publishDomain% 

cd %projectPath%\FacebookCommunityAnalytics.Api.IdentityServer\
msbuild /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.identity.%publishDomain% 

cd %projectPath%\FacebookCommunityAnalytics.Api.Blazor\
msbuild /p:DeployOnBuild=true /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.%publishDomain% 

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