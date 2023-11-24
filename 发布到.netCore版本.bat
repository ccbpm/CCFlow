@ECHO off
COLOR E
ECHO   .
ECHO   !警告 WARNING!
ECHO     .
ECHO     1、即将复制D:\CCFlowCloud 下的文件到D:\CCFlowForNetcore，CCFlowForNetcore 下除NetPlatformImpl、*.NetCore.csproj外，其他所有的文件都会被删除，并用CCFlow中的替换！
ECHO     .
ECHO     2、请关闭所有打开的文件，关闭Visual Studio， 否则会因为文件被占用，导致文件遗漏！
ECHO     .  editor: zhanglei 
PAUSE

COLOR F

echo 更新BP.En30
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\DA
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\En
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\EnTS
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\Port
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\Pub
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\Sys
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\Tools
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\Web
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\DTS
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\GPM
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.En30\References
del /q  D:\内网版本\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\DA              D:\CCFlowForNetcore\Components\BP.En30\DA\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\En             D:\CCFlowForNetcore\Components\BP.En30\En\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\EnTS                D:\CCFlowForNetcore\Components\BP.En30\EnTS\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\Port              D:\CCFlowForNetcore\Components\BP.En30\Port\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\Pub               D:\CCFlowForNetcore\Components\BP.En30\Pub\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\Sys                D:\CCFlowForNetcore\Components\BP.En30\Sys\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\Tools             D:\CCFlowForNetcore\Components\BP.En30\Tools\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.En30\Web              D:\CCFlowForNetcore\Components\BP.En30\Web\
COPY /Y          D:\内网版本\CCFlowCloud\Components\BP.En30\OverrideFile.cs       D:\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

echo 更新BP.WF
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Admin
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCBill
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFast
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCOA
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Data
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\DINGTalk
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\DTS
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\HttpHandler
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Port
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Rpt
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\TA
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Template
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\WeiXin
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\WF
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Xml

del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\AppClass.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\Glo.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\PortalInterface.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\SMS.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\StartWork.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\WF.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs

xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Admin        D:\内网版本\CCFlowForNetcore\Components\BP.WF\Admin\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\CCBill        D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCBill\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\CCFast        D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFast\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\CCOA        D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCOA\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Data         D:\内网版本\CCFlowForNetcore\Components\BP.WF\Data\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\DINGTalk          D:\内网版本\CCFlowForNetcore\Components\BP.WF\DINGTalk\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\DTS          D:\内网版本\CCFlowForNetcore\Components\BP.WF\DTS\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\HttpHandler  D:\内网版本\CCFlowForNetcore\Components\BP.WF\HttpHandler\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Port         D:\内网版本\CCFlowForNetcore\Components\BP.WF\Port\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Rpt          D:\内网版本\CCFlowForNetcore\Components\BP.WF\Rpt\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\TA          D:\内网版本\CCFlowForNetcore\Components\BP.WF\TA\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Template     D:\内网版本\CCFlowForNetcore\Components\BP.WF\Template\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\WeiXin       D:\内网版本\CCFlowForNetcore\Components\BP.WF\WeiXin\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\WF           D:\内网版本\CCFlowForNetcore\Components\BP.WF\WF\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.WF\Xml          D:\内网版本\CCFlowForNetcore\Components\BP.WF\Xml\
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\CCFlowAPI.cs              D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\CCFormAPI.cs              D:\内网版本\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\Dev2Interface.cs          D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\Dev2InterfaceAnonymous.cs D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\Dev2InterfaceGuest.cs     D:\内网版本\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\EnumLib.cs                D:\内网版本\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\Glo.cs                    D:\内网版本\CCFlowForNetcore\Components\BP.WF\Glo.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\HttpWebResponseUtility.cs D:\内网版本\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\SMS.cs                    D:\内网版本\CCFlowForNetcore\Components\BP.WF\SMS.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\WF.cs                     D:\内网版本\CCFlowForNetcore\Components\BP.WF\WF.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.WF\WorkFlowBuessRole.cs      D:\内网版本\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs

echo 更新BP.GPM
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\AD
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Bar
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\GPM
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\UserData

del /q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Crypto.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Dev2Interface.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Glo.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.GPM\HttpHandler.cs

xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.GPM\AD       D:\内网版本\CCFlowForNetcore\Components\BP.GPM\AD\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.GPM\Bar        D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Bar\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.GPM\GPM       D:\内网版本\CCFlowForNetcore\Components\BP.GPM\GPM\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.GPM\UserData        D:\内网版本\CCFlowForNetcore\Components\BP.GPM\UserData\

copy /Y D:\内网版本\CCFlowCloud\Components\BP.GPM\Crypto.cs              D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Crypto.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.GPM\Dev2Interface.cs              D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Dev2Interface.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.GPM\Glo.cs          D:\内网版本\CCFlowForNetcore\Components\BP.GPM\Glo.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.GPM\HttpHandler.cs D:\内网版本\CCFlowForNetcore\Components\BP.GPM\HttpHandler.cs

echo 更新BP.Cloud
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Adminer
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Data
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\DTS
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\HttpHandler
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\OrgSetting
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Port
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Sys
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Template
rd /s/q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\WeXinAPI

del /q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Dev2Interface.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Dev2InterfaceGuest.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\DTSClearTestData.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Glo.cs
del /q D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\QRCodeHelper.cs

xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Adminer       D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Adminer\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Data        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Data\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\DTS       D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\DTS\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\HttpHandler        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\HttpHandler\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\OrgSetting        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\OrgSetting\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Port        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Port\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Sys        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Sys\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Template        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Template\
xcopy /e /k /y D:\内网版本\CCFlowCloud\Components\BP.Cloud\WeXinAPI        D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\WeXinAPI\


copy /Y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Dev2Interface.cs              D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Dev2Interface.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Dev2InterfaceGuest.cs              D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Dev2InterfaceGuest.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.Cloud\DTSClearTestData.cs              D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\DTSClearTestData.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.Cloud\Glo.cs          D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\Glo.cs
copy /Y D:\内网版本\CCFlowCloud\Components\BP.Cloud\QRCodeHelper.cs D:\内网版本\CCFlowForNetcore\Components\BP.Cloud\QRCodeHelper.cs 
 
echo 更新 ccflow前台
RD /q /s D:\内网版本\CCFlowForNetcore\CCFlow\DataUser\
RD /q /s D:\内网版本\CCFlowForNetcore\CCFlow\GPM\
RD /q /s D:\内网版本\CCFlowForNetcore\CCFlow\WF\
RD /q /s D:\内网版本\CCFlowForNetcore\CCFlow\SDKFlowDemo\
RD /q /s D:\内网版本\CCFlowForNetcore\CCFlow\Portal\

XCOPY /e /k /y D:\内网版本\CCFlowCloud\CCFlow\DataUser      D:\内网版本\CCFlowForNetcore\CCFlow\DataUser\
XCOPY /e /k /y D:\内网版本\CCFlowCloud\CCFlow\GPM           D:\内网版本\CCFlowForNetcore\CCFlow\GPM\
XCOPY /e /k /y D:\内网版本\CCFlowCloud\CCFlow\WF            D:\内网版本\CCFlowForNetcore\CCFlow\WF\
XCOPY /e /k /y D:\内网版本\CCFlowCloud\CCFlow\SDKFlowDemo   D:\内网版本\CCFlowForNetcore\CCFlow\SDKFlowDemo\
XCOPY /e /k /y D:\内网版本\CCFlowCloud\CCFlow\Portal   D:\内网版本\CCFlowForNetcore\CCFlow\Portal\

if %ERRORLEVEL% NEQ 0 goto errors
GOTO success

:errors
ECHO 文件复制过程中出现错误，请对照上面的错误提示进行处理！
PAUSE
@ECHO ON

:success
ECHO 文件复制成功！
@ECHO ON
