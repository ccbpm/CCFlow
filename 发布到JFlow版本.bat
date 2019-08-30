-- 更新GPM
xcopy D:\ccflow\CCFlow\GPM  D:\JFlow\jflow-web\src\main\webapp\GPM /e

xcopy D:\ccflow\CCFlow\WF  D:\JFlow\jflow-web\src\main\webapp\WF  /e



-- 删除ccflow的文件.

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
