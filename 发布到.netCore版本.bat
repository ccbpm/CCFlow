@ECHO off
COLOR E
ECHO   .
ECHO   !���� WARNING!
ECHO     .
ECHO     1����������D:\CCFlowCloud �µ��ļ���D:\CCFlowForNetcore��CCFlowForNetcore �³�NetPlatformImpl��*.NetCore.csproj�⣬�������е��ļ����ᱻɾ��������CCFlow�е��滻��
ECHO     .
ECHO     2����ر����д򿪵��ļ����ر�Visual Studio�� �������Ϊ�ļ���ռ�ã������ļ���©��
ECHO     .  editor: zhanglei 
PAUSE

COLOR F

echo ����BP.En30
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\DA
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\En
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\EnTS
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\Port
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\Pub
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\Sys
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\Tools
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\Web
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\DTS
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\GPM
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.En30\References
del /q  D:\�����汾\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\DA              D:\CCFlowForNetcore\Components\BP.En30\DA\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\En             D:\CCFlowForNetcore\Components\BP.En30\En\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\EnTS                D:\CCFlowForNetcore\Components\BP.En30\EnTS\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\Port              D:\CCFlowForNetcore\Components\BP.En30\Port\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\Pub               D:\CCFlowForNetcore\Components\BP.En30\Pub\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\Sys                D:\CCFlowForNetcore\Components\BP.En30\Sys\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\Tools             D:\CCFlowForNetcore\Components\BP.En30\Tools\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.En30\Web              D:\CCFlowForNetcore\Components\BP.En30\Web\
COPY /Y          D:\�����汾\CCFlowCloud\Components\BP.En30\OverrideFile.cs       D:\CCFlowForNetcore\Components\BP.En30\OverrideFile.cs

echo ����BP.WF
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Admin
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCBill
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFast
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCOA
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Data
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\DINGTalk
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\DTS
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\HttpHandler
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Port
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Rpt
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\TA
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Template
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\WeiXin
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\WF
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Xml

del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\AppClass.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\Glo.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\PortalInterface.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\SMS.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\StartWork.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\WF.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs

xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Admin        D:\�����汾\CCFlowForNetcore\Components\BP.WF\Admin\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\CCBill        D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCBill\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\CCFast        D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFast\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\CCOA        D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCOA\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Data         D:\�����汾\CCFlowForNetcore\Components\BP.WF\Data\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\DINGTalk          D:\�����汾\CCFlowForNetcore\Components\BP.WF\DINGTalk\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\DTS          D:\�����汾\CCFlowForNetcore\Components\BP.WF\DTS\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\HttpHandler  D:\�����汾\CCFlowForNetcore\Components\BP.WF\HttpHandler\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Port         D:\�����汾\CCFlowForNetcore\Components\BP.WF\Port\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Rpt          D:\�����汾\CCFlowForNetcore\Components\BP.WF\Rpt\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\TA          D:\�����汾\CCFlowForNetcore\Components\BP.WF\TA\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Template     D:\�����汾\CCFlowForNetcore\Components\BP.WF\Template\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\WeiXin       D:\�����汾\CCFlowForNetcore\Components\BP.WF\WeiXin\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\WF           D:\�����汾\CCFlowForNetcore\Components\BP.WF\WF\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.WF\Xml          D:\�����汾\CCFlowForNetcore\Components\BP.WF\Xml\
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\CCFlowAPI.cs              D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFlowAPI.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\CCFormAPI.cs              D:\�����汾\CCFlowForNetcore\Components\BP.WF\CCFormAPI.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\Dev2Interface.cs          D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2Interface.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\Dev2InterfaceAnonymous.cs D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceAnonymous.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\Dev2InterfaceGuest.cs     D:\�����汾\CCFlowForNetcore\Components\BP.WF\Dev2InterfaceGuest.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\EnumLib.cs                D:\�����汾\CCFlowForNetcore\Components\BP.WF\EnumLib.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\Glo.cs                    D:\�����汾\CCFlowForNetcore\Components\BP.WF\Glo.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\HttpWebResponseUtility.cs D:\�����汾\CCFlowForNetcore\Components\BP.WF\HttpWebResponseUtility.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\SMS.cs                    D:\�����汾\CCFlowForNetcore\Components\BP.WF\SMS.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\WF.cs                     D:\�����汾\CCFlowForNetcore\Components\BP.WF\WF.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.WF\WorkFlowBuessRole.cs      D:\�����汾\CCFlowForNetcore\Components\BP.WF\WorkFlowBuessRole.cs

echo ����BP.GPM
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\AD
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Bar
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\GPM
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\UserData

del /q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Crypto.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Dev2Interface.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Glo.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.GPM\HttpHandler.cs

xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.GPM\AD       D:\�����汾\CCFlowForNetcore\Components\BP.GPM\AD\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.GPM\Bar        D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Bar\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.GPM\GPM       D:\�����汾\CCFlowForNetcore\Components\BP.GPM\GPM\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.GPM\UserData        D:\�����汾\CCFlowForNetcore\Components\BP.GPM\UserData\

copy /Y D:\�����汾\CCFlowCloud\Components\BP.GPM\Crypto.cs              D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Crypto.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.GPM\Dev2Interface.cs              D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Dev2Interface.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.GPM\Glo.cs          D:\�����汾\CCFlowForNetcore\Components\BP.GPM\Glo.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.GPM\HttpHandler.cs D:\�����汾\CCFlowForNetcore\Components\BP.GPM\HttpHandler.cs

echo ����BP.Cloud
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Adminer
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Data
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\DTS
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\HttpHandler
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\OrgSetting
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Port
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Sys
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Template
rd /s/q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\WeXinAPI

del /q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Dev2Interface.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Dev2InterfaceGuest.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\DTSClearTestData.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Glo.cs
del /q D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\QRCodeHelper.cs

xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Adminer       D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Adminer\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Data        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Data\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\DTS       D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\DTS\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\HttpHandler        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\HttpHandler\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\OrgSetting        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\OrgSetting\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Port        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Port\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Sys        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Sys\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Template        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Template\
xcopy /e /k /y D:\�����汾\CCFlowCloud\Components\BP.Cloud\WeXinAPI        D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\WeXinAPI\


copy /Y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Dev2Interface.cs              D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Dev2Interface.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Dev2InterfaceGuest.cs              D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Dev2InterfaceGuest.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.Cloud\DTSClearTestData.cs              D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\DTSClearTestData.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.Cloud\Glo.cs          D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\Glo.cs
copy /Y D:\�����汾\CCFlowCloud\Components\BP.Cloud\QRCodeHelper.cs D:\�����汾\CCFlowForNetcore\Components\BP.Cloud\QRCodeHelper.cs 
 
echo ���� ccflowǰ̨
RD /q /s D:\�����汾\CCFlowForNetcore\CCFlow\DataUser\
RD /q /s D:\�����汾\CCFlowForNetcore\CCFlow\GPM\
RD /q /s D:\�����汾\CCFlowForNetcore\CCFlow\WF\
RD /q /s D:\�����汾\CCFlowForNetcore\CCFlow\SDKFlowDemo\
RD /q /s D:\�����汾\CCFlowForNetcore\CCFlow\Portal\

XCOPY /e /k /y D:\�����汾\CCFlowCloud\CCFlow\DataUser      D:\�����汾\CCFlowForNetcore\CCFlow\DataUser\
XCOPY /e /k /y D:\�����汾\CCFlowCloud\CCFlow\GPM           D:\�����汾\CCFlowForNetcore\CCFlow\GPM\
XCOPY /e /k /y D:\�����汾\CCFlowCloud\CCFlow\WF            D:\�����汾\CCFlowForNetcore\CCFlow\WF\
XCOPY /e /k /y D:\�����汾\CCFlowCloud\CCFlow\SDKFlowDemo   D:\�����汾\CCFlowForNetcore\CCFlow\SDKFlowDemo\
XCOPY /e /k /y D:\�����汾\CCFlowCloud\CCFlow\Portal   D:\�����汾\CCFlowForNetcore\CCFlow\Portal\

if %ERRORLEVEL% NEQ 0 goto errors
GOTO success

:errors
ECHO �ļ����ƹ����г��ִ������������Ĵ�����ʾ���д���
PAUSE
@ECHO ON

:success
ECHO �ļ����Ƴɹ���
@ECHO ON
