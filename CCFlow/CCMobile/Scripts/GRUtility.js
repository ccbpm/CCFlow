//由三原色值合成颜色整数值
function ColorFromRGB(red, green, blue)
{
    return red + green*256 + blue*256*256;
}

//获取颜色中的红色值，传入参数为整数表示的RGB值
function ColorGetR(intColor)
{
    return intColor & 255;
}

//获取颜色中的绿色值，传入参数为整数表示的RGB值
function ColorGetG(intColor)
{
    //return intColor & 255*256;
    return (intColor>>8) & 255;
}

//获取颜色中的蓝色值，传入参数为整数表示的RGB值
function ColorGetB(intColor)
{
    //return intColor & 255*256*256;
    return (intColor>>16) & 255;
}

//创建 XMLHttpRequest 对象
function CreateXMLHttpRequest() 
{
    var xmlhttp;
    if (window.XMLHttpRequest)
        xmlhttp = new XMLHttpRequest(); // code for IE7+, Firefox, Chrome, Opera, Safari
    else
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP"); // code for IE6, IE5
    return xmlhttp;
}

//按异步方式请求报表数据，在响应事件中将数据加载进报表，然后执行后续任务函数
function AjaxReportRun(Report, DataUrl, RunFun) 
{
    var xmlhttp = CreateXMLHttpRequest();
    xmlhttp.onreadystatechange=function()
    {
        if (xmlhttp.readyState==4 && xmlhttp.status==200)
        {
            Report.LoadDataFromAjaxRequest(xmlhttp.responseText, xmlhttp.getAllResponseHeaders()); //加载报表数据
            RunFun(); //数据加载后需要执行的任务
        }
    }
    xmlhttp.open("POST", DataUrl, true);
    xmlhttp.send();
}

//按异步方式请求报表数据，在响应事件中将数据加载进报表，并启动报表查看器的运行
function AjaxReportViewerStart(ReportViewer, DataUrl) 
{
    ReportViewer.Stop(); //首先停止报表的运行

    var xmlhttp = CreateXMLHttpRequest();
    xmlhttp.onreadystatechange=function()
    {
        if (xmlhttp.readyState==4 && xmlhttp.status==200)
        {
            ReportViewer.Report.LoadDataFromAjaxRequest(xmlhttp.responseText, xmlhttp.getAllResponseHeaders()); //加载报表数据
            ReportViewer.Start(); //启动报表运行
        }
    }
    xmlhttp.open("POST", DataUrl, true);
    xmlhttp.send();
}

//按同步方式请求报表数据，数据请求方法调用后紧接着调用报表载入数据的方法
//用 Ajax 载入子报表数据必须用 HTTP 同步数据请求，即采用本函数
function AjaxSyncLoadReportData(Report, DataUrl) 
{
    var xmlhttp = CreateXMLHttpRequest();
    xmlhttp.open("POST", DataUrl, false);
    xmlhttp.send();
    Report.LoadDataFromAjaxRequest(xmlhttp.responseText, xmlhttp.getAllResponseHeaders()); //加载报表数据
}

