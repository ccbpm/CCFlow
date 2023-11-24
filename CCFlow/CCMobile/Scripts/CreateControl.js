//1、变量 gr_InstallPath 等号后面的参数是插件安装文件的所在的网站目录，一般从网站的根目
//   录开始寻址，插件安装文件一定要存在于指定目录下。
//2、gr_Version 等号后面的参数是插件安装包的版本号，如果有新版本插件安装包，应上传新版
//   本插件安装文件到网站对应目录，并更新这里的版本号。
//3、更多详细信息请参考帮助中“报表插件(WEB报表)->在服务器部署插件安装包”部分
var gr_InstallPath = "WF/Activex"; //实际项目中应该写从根目录寻址的目录，如gr_InstallPath="/myapp/report/grinstall"; 
var gr_Version = "6,0,15,0819";

//以下注册号为本机开发测试注册号，报表访问地址为localhost时可以去掉试用标志
//购买注册后，请用您的注册用户名与注册号替换下面变量中值
var gr_UserName = '锐浪报表插件本机开发测试注册';
var gr_SerialNo = '8PJH495VA61FLI5TG0L4KB2337F1G7AKLD6LNNA9F9T28IKRU6N33P8Z6XX4BUYB5E9NZ6INMD5T8EN47IX63VV7F9BJHB5ZJQQ6MX3J3V12C4XDHU97SXX6X3VA57KCB6';

//报表插件目前只能在32位浏览器中使用
var _gr_platform = window.navigator.platform;
if (_gr_platform.indexOf("64") > 0)
    alert("锐浪Grid++Report报表插件不能运行在64位浏览器中，相关报表与打印功能将无法正常运新，请改用32位浏览器！");

//区分浏览器(IE or not)
var _gr_agent = navigator.userAgent.toLowerCase();
var _gr_isIE = (_gr_agent.indexOf("msie")>0)? true : false;

var gr_CodeBase;
if( _gr_isIE )
    gr_CodeBase = 'codebase="' + gr_InstallPath + '/grbsctl6.cab#Version=' + gr_Version + '"';
else
    gr_CodeBase = '';

//创建报表对象，报表对象是不可见的对象，详细请查看帮助中的 IGridppReport
//Name - 指定插件对象的ID，可以用js代码 document.getElementById("%Name%") 获取报表对象
//EventParams - 指定报表对象的需要响应的事件，如："<param name='OnInitialize' value=OnInitialize> <param name='OnProcessBegin' value=OnProcessBegin>"形式，可以指定多个事件
function CreateReport(PluginID, EventParams)
{
    var typeid;
    if( _gr_isIE )
        typeid = 'classid="clsid:396841CC-FC0F-4989-8182-EBA06AA8CA2F" ';
    else
        typeid = 'type="application/x-grplugin6-report" ';
    typeid += gr_CodeBase;
	document.write('<object id="' + PluginID + '" ' + typeid);
	document.write(' width="0" height="0" VIEWASTEXT>');
	if (EventParams != undefined)
	    document.write(EventParams);
	document.write('</object>');
	
	document.write('<script type="text/javascript">');
	    document.write(PluginID + '.Register("' + gr_UserName + '", "' + gr_SerialNo + '");');
	document.write('</script>');
}

//用更多的参数创建报表打印显示插件，详细请查看帮助中的 IGRPrintViewer
//PluginID - 插件的ID，可以通过 var ReportViewer = document.getElementById("%PluginID%"); 这样的方式获取插件引用变量
//Width - 插件的显示宽度，"100%"为整个显示区域宽度，"500"表示500个屏幕像素点
//Height - 插件的显示高度，"100%"为整个显示区域高度，"500"表示500个屏幕像素点
//ReportURL - 获取报表模板的URL
//DataURL - 获取报表数据的URL
//AutoRun - 指定插件在创建之后是否自动生成并展现报表,值为false或true
//ExParams - 指定更多的插件属性阐述,形如: "<param name="%ParamName%" value="%Value%">"这样的参数串
function CreatePrintViewerEx2(PluginID, Width, Height, ReportURL, DataURL, AutoRun, ExParams)
{
    var typeid;
    if( _gr_isIE )
        typeid = 'classid="clsid:ABB64AAC-D7E8-4733-B052-1B141C92F3CE" ' + gr_CodeBase;
    else
        typeid = 'type="application/x-grplugin6-printviewer"';
	document.write('<object id="' + PluginID + '" ' + typeid);
	document.write(' width="' + Width + '" height="' + Height + '">');
	document.write('<param name="ReportURL" value="' + ReportURL + '">');
	document.write('<param name="DataURL" value="' + DataURL + '">');
	document.write('<param name="AutoRun" value=' + AutoRun + '>');
	document.write('<param name="SerialNo" value="' + gr_SerialNo + '">');
	document.write('<param name="UserName" value="' + gr_UserName + '">');
	document.write(ExParams);
	document.write('</object>');
}

//用更多的参数创建报表打印显示插件，详细请查看帮助中的 IGRDisplayViewer
//PluginID - 插件的ID，可以通过 var ReportViewer = document.getElementById("%PluginID%"); 这样的方式获取插件引用变量
//Width - 插件的显示宽度，"100%"为整个显示区域宽度，"500"表示500个屏幕像素点
//Height - 插件的显示高度，"100%"为整个显示区域高度，"500"表示500个屏幕像素点
//ReportURL - 获取报表模板的URL
//DataURL - 获取报表数据的URL
//AutoRun - 指定插件在创建之后是否自动生成并展现报表,值为false或true
//ExParams - 指定更多的插件属性阐述,形如: "<param name="%ParamName%" value="%Value%">"这样的参数串
function CreateDisplayViewerEx2(PluginID, Width, Height, ReportURL, DataURL, AutoRun, ExParams)
{
    var typeid;
    if( _gr_isIE )
        typeid = 'classid="clsid:600CD6D9-EBE1-42cb-B8DF-DFB81977122E" ' + gr_CodeBase;
    else
        typeid = 'type="application/x-grplugin6-displayviewer"';
	document.write('<object id="' + PluginID + '" ' + typeid);
	document.write(' width="' + Width + '" height="' + Height + '">');
	document.write('<param name="ReportURL" value="' + ReportURL + '">');
	document.write('<param name="DataURL" value="' + DataURL + '">');
	document.write('<param name="AutoRun" value=' + AutoRun + '>');
	document.write('<param name="SerialNo" value="' + gr_SerialNo + '">');
	document.write('<param name="UserName" value="' + gr_UserName + '">');
	document.write(ExParams);
	document.write('</object>');
}

//以 ReportDesigner 为 ID 创建报表设计器插件(Designer)，详细请查看帮助中的 IGRDesigner
//Width - 插件的显示宽度，"100%"为整个显示区域宽度，"500"表示500个屏幕像素点
//Height - 插件的显示高度，"100%"为整个显示区域高度，"500"表示500个屏幕像素点
//LoadReportURL - 读取报表模板的URL，运行时从此URL读入报表模板数据并加载到设计器插件
//SaveReportURL - 保存报表模板的URL，保存设计后的结果数据，由此URL的服务在WEB服务端将报表模板持久保存
//DataURL - 获取报表运行时数据的URL，在设计器中进入打印视图与查询视图时从此URL获取报表数据
//ExParams - 指定更多的插件属性阐述,形如: "<param name="%ParamName%" value="%Value%">"这样的参数串
function CreateDesignerEx(Width, Height, LoadReportURL, SaveReportURL, DataURL, ExParams)
{
    var typeid;
    if( _gr_isIE )
        typeid = 'classid="clsid:CE666189-5D7C-42ee-AAA4-E5CB375ED3C7" ' + gr_CodeBase;
    else
        typeid = 'type="application/x-grplugin6-designer"';


	document.write('<object id="ReportDesigner" ' + typeid);
	document.write(' width="' + Width + '" height="' + Height + '">');
	document.write('<param name="LoadReportURL" value="' + LoadReportURL + '">');
	document.write('<param name="SaveReportURL" value="' + SaveReportURL + '">');
	document.write('<param name="DataURL" value="' + DataURL + '">');
	document.write('<param name="SerialNo" value="' + gr_SerialNo + '">');
	document.write('<param name="UserName" value="' + gr_UserName + '">');
	document.write(ExParams);
	document.write('</object>');
}

//以 ReportViewer 为 ID 创建报表打印显示器插件(PrintViewer)，参数说明参考 CreatePrintViewerEx2
function CreatePrintViewerEx(Width, Height, ReportURL, DataURL, AutoRun, ExParams)
{
    CreatePrintViewerEx2("ReportViewer", Width, Height, ReportURL, DataURL, AutoRun, ExParams)
}

//以 ReportViewer 为 ID 创建报表查询显示器插件(DisplayViewer)，参数说明参考 CreateDisplayViewerEx2
function CreateDisplayViewerEx(Width, Height, ReportURL, DataURL, AutoRun, ExParams)
{
    CreateDisplayViewerEx2("ReportViewer", Width, Height, ReportURL, DataURL, AutoRun, ExParams)
}

//以 ReportViewer 为 ID 创建报表打印显示器插件(PrintViewer)，插件大小为100%充满位置区域，插件创建后会自动运行，参数说明参考 CreatePrintViewerEx2
function CreatePrintViewer(ReportURL, DataURL)
{
    CreatePrintViewerEx("100%", "100%", ReportURL, DataURL, true, "");
}

//以 ReportViewer 为 ID 创建报表查询显示器插件(DisplayViewer)，插件大小为100%充满位置区域，插件创建后会自动运行，参数说明参考 CreateDisplayViewerEx2
function CreateDisplayViewer(ReportURL, DataURL)
{
    CreateDisplayViewerEx("100%", "100%", ReportURL, DataURL, true, "");
}

//以 ReportDesigner 为 ID 创建报表设计器插件(Designer)，插件大小为100%充满位置区域，参数说明参考 CreateDesignerEx
function CreateDesigner(LoadReportURL, SaveReportURL, DataURL)
{
    CreateDesignerEx("100%", "100%", LoadReportURL, SaveReportURL, DataURL, "");
}