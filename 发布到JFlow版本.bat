@ECHO off
COLOR E
ECHO   .
ECHO   ����˵��:
ECHO     .
ECHO     1. ��������D:\CCFlowCloud �µ��ļ���D:\JFlow    
ECHO     2. �����½�������ǰ̨ҳ��, ���ں�̨���ļ�����Ҫ�ֹ�����.
ECHO     3. java�汾�������汾���£���Ҫ�ο�D:\JFlow\*.bat�ļ�.  
ECHO     4. ccflow��ǰ̨�ļ���������D:\JFlow �汾����.
ECHO        editor: zhanglei 
PAUSE

COLOR F

--����GPM

rd  D:\JFlow\jflow-web\src\main\webapp\GPM  /s /Q
rd  D:\JFlow\jflow-web\src\main\webapp\WF   /s /Q
rd  D:\JFlow\jflow-web\src\main\webapp\CCFast   /s /Q
rd  D:\JFlow\jflow-web\src\main\webapp\Portal   /s /Q

 xcopy  /e /k /y D:\CCFlowCloud\CCFlow\GPM\  D:\JFlow\jflow-web\src\main\webapp\GPM\
 xcopy  /e /k /y D:\CCFlowCloud\CCFlow\WF\     D:\JFlow\jflow-web\src\main\webapp\WF\
 xcopy  /e /k /y D:\CCFlowCloud\CCFlow\CCFast\     D:\JFlow\jflow-web\src\main\webapp\CCFast\
 xcopy  /e /k /y D:\CCFlowCloud\CCFlow\Portal\     D:\JFlow\jflow-web\src\main\webapp\Portal\


del D:\JFlow\jflow-web\src\main\webapp\WF\CCForm\*.asmx;
del D:\JFlow\jflow-web\src\main\webapp\WF\CCForm\*.cs;
del D:\JFlow\jflow-web\src\main\webapp\WF\CCForm\*.aspx;

del D:\JFlow\jflow-web\src\main\webapp\WF\Comm\*.aspx;
del D:\JFlow\jflow-web\src\main\webapp\WF\Comm\*.cs;

del D:\JFlow\jflow-web\src\main\webapp\WF\WorkOpt\*.aspx;
del D:\JFlow\jflow-web\src\main\webapp\WF\WorkOpt\*.cs;

del D:\JFlow\jflow-web\src\main\webapp\WF\Scripts\config.js
ren D:\JFlow\jflow-web\src\main\webapp\WF\Scripts\configJFlow.js  config.js

pause;
