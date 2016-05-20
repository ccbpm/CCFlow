<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>

<%@ Page Language="c#" Inherits="BP.Web.LogininGPM" CodeBehind="Loginin.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>
        <%=BP.Sys.SystemConfig.SysName%>
    </title>
    <meta name="vs_showGrid" content="True" />
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
 <style type="text/css">
       /* css reset */
			body{color:#000;background:#fff;font-size:12px;line-height:166.6%;text-align:center;}
			body,input,select,button{font-family:verdana}
			h1,h2,h3,select,input,button{font-size:100%}
			body,h1,h2,h3,ul,li,form,p,img{margin:0;padding:0;border:0}
			img{margin:0;line-height:normal}
			select{padding:1px}
			select,input,button,button img,label{vertical-align:middle;
                width: 171px;
                height: 36px;
            }
            #IsRemember{vertical-align:middle; width: 15px; height: 15px;}
			header,footer,section,aside,nav,hgroup,figure,figcaption{display:block;margin:0;padding:0;border:none}
			a{text-decoration:none;color:#848585}
			a:hover{color:#000}
			.fontWeight{font-weight:700;}
			/* backgroundImage */
			.headerIntro,
			.themeText li,
			.domain,
			.whatAutologin,
			.ico,
			.ext,
			.headerLogo,
			.headerIntro,
			.headerNav,		
			.footerLogo,
			.footerNav,		
			.domain{position:absolute}
			
			/* header */
			.header{width:800px;height:64px;position:relative;margin:0 auto;z-index:2;overflow:hidden;}
			.headerLogo{top:17px;left:0px}
			.headerIntro{height:28px;width:160px;display:block;background-position:0 -64px;top:17px;left:144px}
			.headerNav{top:30px;right:0px;width:300px;text-align:right}
			.headerNav a{margin-left:13px}
		
			/* main */
			.main{height:440px;margin:0 auto;background:#fff;}
			.main-inner{width:900px;height:440px;overflow:visible;margin:0 auto;position:relative;clear:both}
			#theme
			{height:440px;width:900px;position:absolute;overflow:hidden;z-index:1;background-position:top right;background-repeat:no-repeat;text-align:left;top:0;left:0;}
			.themeLink{height:274px;width:430px;display:block;outline:0;}
			.themeText{margin-left:26px;}
			.themeText li{line-height:22px;-line-height:24px;height:24px;color:#858686;text-indent:12px;background-position:-756px -72px;background-repeat:no-repeat}
			.themeText li a{color:#005590;text-decoration:underline;}
			.unishadow{box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);-webkit-box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);-moz-box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);}
					
			/* footer */
			.footer{height:65px;margin:0 auto;background:#f7f7f7;border-top:1px solid #fff;}
			.footer-inner{width:900px;height:63px;overflow:hidden;margin:0 auto;color:#848585;position:relative;background:#f7f7f7;
                top: 0px;
                left: 0px;
            }
			.footerLogo{top:11px;left:35px}
			.footerNav{top:25px;right:126px;}
			.footerNav a{margin-left:8px}
			.copyright{margin-left:22px}		
			#themeArea{width:240px;height:80px;position:absolute;left:90px;top:134px;}
		
			#theme{-webkit-transition:all 1s ease;-moz-transition:all 1s ease;transition:all 1s ease;background:none;}
			#theme.themeEffect{background:#e7ebe9;}
			.login{width:300px;height:300px;overflow:;float:right;margin-right:70px;margin-top:35px;background:#fff;border:1px solid #afc2af;_display:inline;text-align:left;position:relative;z-index:2;border-radius:2px;}
			.login{box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);-webkit-box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);-moz-box-shadow:0px 1px 3px 0 rgba(0,0,0,0.2);}
		    .tab-1{background:none;}
		    .style1
            {
                width: 67px;
                height: 48px;
                margin-left:10px;
            }
		    .style5
            {
                width: 67px;
                height: 62px;
            }
            .style7
            {
                height: 48px;
            }
            .style8
            {
                height: 77px;
            }
            .style9
            {
                height: 62px;
            }
            #divMain{ display:block;margin:0;padding:0;border:none ;height:440px;margin:0 auto;background:#f7f7f7; 
                      background-image:url('Img/login1.jpg'); background-position:inherit; background-repeat:repeat-x;}
     .style10
     {
         width: 67px;
     }
    </style>
</head>
<body>
       <div style="height:56px; width:900px; margin:0 auto;">
		<div style="float:left; width:40%; display:inline ">
        <a><img src="Img/logo/ccPort.jpg" alt="ccflow"/></a></div>
		<div style="float:right; margin-right:100px; width:30%;height:56px; position:relative; display:inline;">
            <div style="position:absolute; bottom:0px;">        
			<a href="http://ccflow.org/" target="_blank">ccflow5</a>
            <a href="http://ccflow.org/" target="_blank">GPM</a>
            <a href="http://ccflow.org/" target="_blank">SSO</a>
            <a href="http://ccflow.org/" target="_blank">CCIM</a>
            <a href="http://ccflow.org/" target="_blank">CCOA</a>
            </div>
		</div>
        <div style="clear:both;"></div>
	</div>
    <form method="post" runat="server" defaultfocus="TB_No">
    <section class="main" id="mainBg">
     <div id="divMain" style="background-repeat:repeat-x; width:100%; background-position:left; display:block;margin:0;padding:0;border:none" >
		<div class="main-inner" style="  background-repeat:repeat-x; background-position-x: center; background-position-y: top;" >
			<div id="theme">
            <img src="Img/cc.png" style=" height:440px; width:900px;" />
            </div>
			<div id="loginBlock" class="login tab-1">
            <table  style="width:100%; background-color:#F9FBF9; height: 100%;">
            <tr >
             <td colspan="2" class="style8">
              &nbsp;&nbsp;<img src="Img/yaoshi.jpg"  style=" width:29px; height:12px;" />用户登录
              <hr style="width:100%; height:0.5px; color:Black" />
            </td>
            </tr>
               <tr>
                <td class="style1">
                   
										<font size="3"><b>&nbsp;用户名</b></font>
                </td>
                  <td class="style1" >
                  <cc1:TB ID="TB_No"  runat="server" ShowType="TB"
                                Width="190px" Height="40px" Font-Bold="true" Font-Size="25px"></cc1:TB>
                </td>
               </tr>
               <tr>
                <td class="style5">
                  &nbsp;&nbsp;
										<font size="3">&nbsp;&nbsp;<b>密码</b></font>
                </td><td class="style1">
                  <cc1:TB ID="TB_Pass" runat="server"
                                Width="190px" Height="40px" TextMode="Password" Font-Bold="true" Font-Size="25px" ></cc1:TB>
                        
                </td>
               </tr>
               <tr>
                <td colspan="2">
                   &nbsp;&nbsp;<asp:CheckBox ID="IsRemember" runat="server" Checked="true"/>是否记住密码
        
                  <cc1:Btn ID="Btn1" runat="server" Style="border-style: none; border-color: inherit; border-width: medium; background: url('Img/btlogin.png'); width: 108px; 
                                height: 33px; top: 251px; left: 32px; right: 196px;" 
                        OnClick="Btn1_Click">
                  </cc1:Btn>&nbsp;&nbsp;&nbsp;&nbsp;              
                  </td>
               </tr>
               <%--<tr  style=" text-align:center">
                <td colspan="2" class="style8">
                  <cc1:Btn ID="Btn1" runat="server" Style="border-style: none; border-color: inherit; border-width: medium; background: url('Img/btlogin.png'); width: 108px; 
                                height: 33px; top: 251px; left: 32px; right: 196px;" 
                        OnClick="Btn1_Click">
                  </cc1:Btn>&nbsp;&nbsp;&nbsp;&nbsp;              
                  </td>
               </tr>--%>
               <tr>
              
               <td colspan="2">
               <div id="divErr" runat="server">
               
               </div>
               </td>
               </tr>
            </table>
			</div>
		</div>
      </div>
	</section>
    <footer id="footer" class="footer">
		<div class="footer-inner" id="footerInner">			
			<nav class="footerNav">
                <a href="http://ccflow.org/" target="_blank">官方网站</a>&nbsp;
                <a href="http://weibo.com/signup/signup.php?inviteCode=1618274127" target="_blank"><img src="Img/ico_sina_weibo.gif" />@新浪微博</a>
                <a href="http://t.qq.com/hiflow" target="_blank"> <img src="Img/ico_tx_weibo.gif" /> @腾讯微博 </a>
                |<span class="copyright">济南驰骋信息技术有限公司 - 版权所有 &copy; 2003-2015</span>&nbsp;&nbsp;咨询电话:0531-82374939
			</nav>
		</div>
	</footer>
    </form>
</body>
</html>
