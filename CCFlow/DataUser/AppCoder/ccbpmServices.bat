
@echo off 
echo 正在关闭未关闭的IE进程，请稍等...... 
taskkill /f /im iexplore.exe 

echo -------------开始运行指定的网页页面---------- 
::echo.&pause 

start "" "C:\Program Files\Internet Explorer\iexplore.exe" 

http://127.0.0.1:80/DataUser/AppCoder/ccbpmServices.htm

echo IE打开完成！ 
::echo.&pause 

ping 127.0.0.1 -n 2 

taskkill /f /im iexplore.exe
