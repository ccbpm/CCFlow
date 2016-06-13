<%@ Page Language="C#" AutoEventWireup="true" Inherits="AppClassic_Left" Codebehind="Left.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导航</title>
    <style type="text/css">
*{  margin:0; padding:0; font-size:12px; font-family:"微软雅黑","宋体";}
html,body,form
{
    margin:0 auto;
    height:100%;  
    font-family:"微软雅黑","宋体";
}

.over{ }
pre{ margin:0; padding:0;}
a:visited{ color:#333;}
a:link{ color:#333;}
a:hover{ color: Blue;}
a { }
li { list-style:none;}
img { border:none;}
.JS_zhut{width:100%;height: 99%; overflow:hidden; margin:0px; background: #e1e7ec; margin:0 auto;}

.JS_zhut_n
{
    width:100%;
    height: 99%; 
    overflow:hidden;
    margin:0 auto; 
    padding:1px;
}

.JS_zhut_n .left { width:100%; height:99%; margin-top:2px; background:#f1f1f3; border:1px solid #FFF; overflow:hidden;  }
.JS_zhut_n .left ul li
 { 
     width:158px; 
     height:41px;
     line-height:20px;
}
.JS_zhut_n .left ul li a { color:#333; text-decoration:none; font-size:14px; float:left; margin-left:30px;}

 </style>
    <script language="JavaScript">
        function myrefresh() {
            window.location.reload();
        }
        setTimeout('myrefresh()', 100000); //指定1秒刷新一次 
   </script> 
</head>
<body>
<%
    if (BP.Web.WebUser.No == null)
    {
        %>
         登录信息丢失，请重新登录。
        <%
            return;
    }
     %>
    <div class="JS_zhut"  >
        <div class="JS_zhut_n" >
            <div  class="left" style="overflow-y:auto" >
            <br />
                 <!-- 菜单的配置在: \\DataUser\\XML\\BarOfTop.xml -->
                <ul>
                <% 
                    BP.WF.XML.ClassicMenus ens = new BP.WF.XML.ClassicMenus();
                    ens.RetrieveAll();
                    string strs = "";
                    foreach (BP.WF.XML.ClassicMenu en in ens)
                    {
                        if (en.Enable == false)
                            continue;
                        strs += "<li style='background:url(" + en.Img + ") no-repeat;background-position-x:1%' ><a  href='"+en.Url+"' target='main'>"+en.Name+"</a></li>";
                    }
                    %>
                    <%=strs %>
                </ul>
            </div>
        </div>
    </div>
</body>
</html>

