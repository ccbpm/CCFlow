@ECHO off
COLOR E
ECHO   .
ECHO   ����˵��:
ECHO     .
ECHO     1. ��������D:\CCFlow �µ��ļ���D:\JFlow    
ECHO     2. �����½�������ǰ̨ҳ��, ���ں�̨���ļ�����Ҫ�ֹ�����.
ECHO     3. java�汾�������汾���£���Ҫ�ο�D:\JFlow\*.bat�ļ�.  
ECHO     4. ccflow��ǰ̨�ļ���������D:\JFlow �汾����.
ECHO        editor: zhanglei 
PAUSE

COLOR F



--����GPM
xcopy  /e /k /y D:\ccflow\CCFlow\GPM  D:\JFlow\jflow-web\src\main\webapp\GPM 
xcopy  /e /k /y D:\ccflow\CCFlow\WF  D:\JFlow\jflow-web\src\main\webapp\WF  

-- ɾ��ccflow���ļ�.

del D:\JFlow2020\jflow-web\src\main\webapp\WF\CCForm\*.asmx;
del D:\JFlow2020\jflow-web\src\main\webapp\WF\CCForm\*.cs;
del D:\JFlow2020\jflow-web\src\main\webapp\WF\CCForm\*.aspx;

del D:\JFlow2020\jflow-web\src\main\webapp\WF\Comm\*.aspx;
del D:\JFlow2020\jflow-web\src\main\webapp\WF\Comm\*.cs;


del D:\JFlow2020\jflow-web\src\main\webapp\WF\WorkOpt\*.aspx;
del D:\JFlow2020\jflow-web\src\main\webapp\WF\WorkOpt\*.cs;

del D:\JFlow2020\jflow-web\src\main\webapp\WF\Scripts\config.js
ren D:\JFlow2020\jflow-web\src\main\webapp\WF\Scripts\configJFlow.js  config.js

pause;
