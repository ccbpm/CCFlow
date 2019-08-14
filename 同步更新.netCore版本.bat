@ECHO off
COLOR E
ECHO   .
ECHO   !警告 WARNING!
ECHO     .
ECHO     1、即将复制D:\CCFlow 下的文件到D:\CCFlowForNetcore，CCFlowForNetcore 下除NetPlatformImpl、*.NetCore.csproj外，其他所有的文件都会被删除，并用CCFlow中的替换！
ECHO     .
ECHO     2、请关闭所有打开的文件，关闭Visual Studio， 否则会因为文件被占用，导致文件遗漏！
ECHO     .  editer: zhanlei 
PAUSE

COLOR F

echo 更新BP.En30
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\DA
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\DTS
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\En
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\GPM
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\Port
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\Pub
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\References
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\Sys
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\Tools
rd /s/q D:\CCFlowForNetcore\Components\BP.En30\Web
del /q D:\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

xcopy /e /k /y D:\ccflow\Components\BP.En30\DA              D:\CCFlowForNetcore\Components\BP.En30\DA\
xcopy /e /k /y D:\ccflow\Components\BP.En30\DTS             D:\CCFlowForNetcore\Components\BP.En30\DTS\
xcopy /e /k /y D:\ccflow\Components\BP.En30\En              D:\CCFlowForNetcore\Components\BP.En30\En\
xcopy /e /k /y D:\ccflow\Components\BP.En30\GPM             D:\CCFlowForNetcore\Components\BP.En30\GPM\
xcopy /e /k /y D:\ccflow\Components\BP.En30\Port            D:\CCFlowForNetcore\Components\BP.En30\Port\
xcopy /e /k /y D:\ccflow\Components\BP.En30\Pub             D:\CCFlowForNetcore\Components\BP.En30\Pub\
xcopy /e /k /y D:\ccflow\Components\BP.En30\References      D:\CCFlowForNetcore\Components\BP.En30\References\
xcopy /e /k /y D:\ccflow\Components\BP.En30\Sys             D:\CCFlowForNetcore\Components\BP.En30\Sys\
xcopy /e /k /y D:\ccflow\Components\BP.En30\Tools           D:\CCFlowForNetcore\Components\BP.En30\Tools\
xcopy /e /k /y D:\ccflow\Components\BP.En30\Web             D:\CCFlowForNetcore\Components\BP.En30\Web\
COPY /Y D:\ccflow\Components\BP.En30\OverrideFile.cs           D:\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

echo 更新BP.WF
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Cloud
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Data
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\DTS
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Frm
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\HttpHandler
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Port
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Rpt
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Template
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\UnitTesting
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\WeiXin
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\WF
rd /s/q D:\CCFlowForNetcore\Components\BP.WF\Xml
del /q D:\CCFlowForNetcore\Components\BP.WF\AppClass.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceHtml.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Glo.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\Msg.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\OverrideFile.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\PortalInterface.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\SMS.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\StartWork.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\WF.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\WFEnum.cs
del /q D:\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs

xcopy /e /k /y D:\ccflow\Components\BP.WF\Cloud        D:\CCFlowForNetcore\Components\BP.WF\Cloud\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Data         D:\CCFlowForNetcore\Components\BP.WF\Data\
xcopy /e /k /y D:\ccflow\Components\BP.WF\DTS          D:\CCFlowForNetcore\Components\BP.WF\DTS\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Frm          D:\CCFlowForNetcore\Components\BP.WF\Frm\
xcopy /e /k /y D:\ccflow\Components\BP.WF\HttpHandler  D:\CCFlowForNetcore\Components\BP.WF\HttpHandler\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Port         D:\CCFlowForNetcore\Components\BP.WF\Port\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Rpt          D:\CCFlowForNetcore\Components\BP.WF\Rpt\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Template     D:\CCFlowForNetcore\Components\BP.WF\Template\
xcopy /e /k /y D:\ccflow\Components\BP.WF\UnitTesting  D:\CCFlowForNetcore\Components\BP.WF\UnitTesting\
xcopy /e /k /y D:\ccflow\Components\BP.WF\WeiXin       D:\CCFlowForNetcore\Components\BP.WF\WeiXin\
xcopy /e /k /y D:\ccflow\Components\BP.WF\WF           D:\CCFlowForNetcore\Components\BP.WF\WF\
xcopy /e /k /y D:\ccflow\Components\BP.WF\Xml          D:\CCFlowForNetcore\Components\BP.WF\Xml\
COPY /Y D:\ccflow\Components\BP.WF\AppClass.cs               D:\CCFlowForNetcore\Components\BP.WF\AppClass.cs
copy /Y D:\ccflow\Components\BP.WF\CCFlowAPI.cs              D:\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
copy /Y D:\ccflow\Components\BP.WF\CCFormAPI.cs              D:\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
copy /Y D:\ccflow\Components\BP.WF\Dev2Interface.cs          D:\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
copy /Y D:\ccflow\Components\BP.WF\Dev2InterfaceAnonymous.cs D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
copy /Y D:\ccflow\Components\BP.WF\Dev2InterfaceGuest.cs     D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
copy /Y D:\ccflow\Components\BP.WF\Dev2InterfaceHtml.cs      D:\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceHtml.cs
copy /Y D:\ccflow\Components\BP.WF\EnumLib.cs                D:\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
copy /Y D:\ccflow\Components\BP.WF\Glo.cs                    D:\CCFlowForNetcore\Components\BP.WF\Glo.cs
copy /Y D:\ccflow\Components\BP.WF\HttpWebResponseUtility.cs D:\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
copy /Y D:\ccflow\Components\BP.WF\Msg.cs                    D:\CCFlowForNetcore\Components\BP.WF\Msg.cs
copy /Y D:\ccflow\Components\BP.WF\OverrideFile.cs           D:\CCFlowForNetcore\Components\BP.WF\OverrideFile.cs
copy /Y D:\ccflow\Components\BP.WF\PortalInterface.cs        D:\CCFlowForNetcore\Components\BP.WF\PortalInterface.cs
copy /Y D:\ccflow\Components\BP.WF\SMS.cs                    D:\CCFlowForNetcore\Components\BP.WF\SMS.cs
copy /Y D:\ccflow\Components\BP.WF\StartWork.cs              D:\CCFlowForNetcore\Components\BP.WF\StartWork.cs
copy /Y D:\ccflow\Components\BP.WF\WF.cs                     D:\CCFlowForNetcore\Components\BP.WF\WF.cs
copy /Y D:\ccflow\Components\BP.WF\WFEnum.cs                 D:\CCFlowForNetcore\Components\BP.WF\WFEnum.cs
copy /Y D:\ccflow\Components\BP.WF\WorkFlowBuessRole.cs      D:\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs
 
echo 更新 ccflow前台
RD /q /s D:\CCFlowForNetcore\CCFlow\DataUser\
RD /q /s D:\CCFlowForNetcore\CCFlow\GPM\
RD /q /s D:\CCFlowForNetcore\CCFlow\WF\
RD /q /s D:\CCFlowForNetcore\CCFlow\SDKFlowDemo\

XCOPY /e /k /y D:\ccflow\CCFlow\DataUser      D:\CCFlowForNetcore\CCFlow\DataUser\
XCOPY /e /k /y D:\ccflow\CCFlow\GPM           D:\CCFlowForNetcore\CCFlow\GPM\
XCOPY /e /k /y D:\ccflow\CCFlow\WF            D:\CCFlowForNetcore\CCFlow\WF\
XCOPY /e /k /y D:\ccflow\CCFlow\SDKFlowDemo   D:\CCFlowForNetcore\CCFlow\SDKFlowDemo\

if %ERRORLEVEL% NEQ 0 goto errors
GOTO success

:errors
ECHO 文件复制过程中出现错误，请对照上面的错误提示进行处理！
PAUSE
@ECHO ON

:success
ECHO 文件复制成功！
@ECHO ON

