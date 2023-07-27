D:
cd D:\Workspace\facebook-community-analytics\db.tools\bin
mongodump "mongodb://fbca.admin.dan:px7hA&N8#9N7$K3g@103.163.214.53:27017/FacebookCommunityAnalytics-DAN?authSource=FacebookCommunityAnalytics-DAN"

@echo off
for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
set ldt=%ldt:~0,4%-%ldt:~4,2%-%ldt:~6,2%_%ldt:~8,2%-%ldt:~10,2%
echo Local date is [%ldt%]
rename dump DbLive-fbca-DAN-%ldt%
