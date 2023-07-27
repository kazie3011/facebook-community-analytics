@echo off

set projectPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\src
set publishScriptPath=D:\Workspace\facebook-community-analytics\FacebookCommunityAnalytics.Api\publish-scripts

set env=partner-test
set publishDomain=gdll.vn.pubxml
set password=xdQQaBq8x!9h

echo ________________________________
echo *                              *
echo *      -Using Partner-test config      *
echo *______________________________*
cd %publishScriptPath%
call update-appsettings-partner-test.bat
call update-globalconfigs-partner-test.bat
rem timeout /t 3

echo ________________________________
echo *                              *
echo *      -Start to publish-      *
echo *      -ENV: %env%             *
echo *______________________________*  

cd %projectPath%\FacebookCommunityAnalytics.Api.Web\
msbuild /p:Configuration=Debug /p:DeployOnBuild=true /p:AllowUntrustedCertificate=True /p:PublishProfile=%env%.%publishDomain%  

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