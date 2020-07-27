@ECHO off
COLOR E
ECHO   .
ECHO   更新说明:
ECHO     .
ECHO     1. 即将复制D:\CCFlow 下的文件到D:\JFlow    
ECHO     2. 本更新仅仅处理前台页面, 对于后台的文件，需要手工翻译.
ECHO     3. java版本的其他版本更新，需要参考D:\JFlow\*.bat文件.  
ECHO     4. ccflow的前台文件，仅仅向D:\JFlow 版本更新.
ECHO        editor: zhanglei 
PAUSE

COLOR F



--更新GPM
xcopy  /e /k /y D:\ccflow\CCFlow\GPM  D:\JFlow\jflow-web\src\main\webapp\GPM 
xcopy  /e /k /y D:\ccflow\CCFlow\WF  D:\JFlow\jflow-web\src\main\webapp\WF  

-- 删除ccflow的文件.

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
