@echo off 
echo ���ڹر�δ�رյ�IE���̣����Ե�...... 
taskkill /f /im iexplore.exe  

echo -------------��ʼ����ָ������ҳҳ��---------- 
::echo.&pause 

start "" "C:\Program Files\Internet Explorer\iexplore.exe" localhost:2207/App/AutoExec.htm

echo IE����ɣ� 
::echo.&pause 

ping 127.0.0.1 -n 2 

taskkill /f /im iexplore.exe 