<%@ Page Language="C#" AutoEventWireup="true" Inherits="App_Login" CodeBehind="Login.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <title> <%=BP.Sys.SystemConfig.SysName %></title>
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
                width: 102px;
                height: 48px;
                margin-left:10px;
            }
		    .style2
            {
                width: 102px;
                height: 25px;
            }
            .style3
            {
                height: 25px;
            }
            .style5
            {
                width: 102px;
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
                      background-image:url('Img/repeatimage.jpg'); background-position:inherit; background-repeat:repeat-x;}
    </style>
    <script type="text/javascript">
        $(function () {
            $('#<%=TB_Pass.ClientID %>').change(function () {
                $('#hidIsRememberedPass').val('0');
            });
        });
    </script>
</head>
<body> 
    <div style="height:56px; width:800px; margin:0 auto;">
		<div style="float:left; width:40%; display:inline ">
        <a><img src="/DataUser/Icon/LogBig.png" alt="ccflow" style=" height:60px;" /></a></div>
		<div style="float:right; margin-right:100px; width:30%;height:56px; position:relative; display:inline;">
            <div style="position:absolute; bottom:0px;">
			 
            </div>
		</div>
        <div style="clear:both;"></div>
	</div>
    <form id="Form1" method="post" runat="server" defaultfocus="TB_No">
    <section class="main" id="mainBg">
     <div id="divMain" style="background-repeat:repeat-x; width:100%; background-position:left; display:block;margin:0;padding:0;border:none" >
		<div class="main-inner" style="background-position: center top; background-repeat:repeat-x; top: 0px; left: 0px;" >
			<div id="theme">
            <img src="/DataUser/ICON/Login.jpg" style=" height:440px; width:900px;" />
            </div>
			<div id="loginBlock" class="login tab-1">
            <table  style="width:100%;background-color:#F1F3F8;height: 120%;">
            <tr >
             <td colspan="2" class="style8">
              &nbsp;&nbsp;<img src="Img/Key.png"   style="width:25px; height:25px;" />
              <strong style="font-size:16px;"> 用户登录</strong>
              <hr style="width:80%; height:0.5px; color:Black" />
            </td>
            </tr>
               <tr>
                <td >
                 &nbsp;&nbsp;<font size="2">&nbsp;&nbsp;<b>用户名</b></font>
                </td>
                  <td >
                  <cc1:TB ID="TB_No"  runat="server" ShowType="TB"  Font-Bold="true"
                                Width="194px" Height="28px" BorderStyle="None" Font-Size="14px"></cc1:TB>
                </td>
               </tr>
               <tr>
                <td >
                  &nbsp;&nbsp;<cc1:lab ID="Lab2" runat="server">
										<font size="2">&nbsp;&nbsp;<b>密&nbsp;码</b></font></cc1:lab>
                </td><td>
                  <cc1:TB ID="TB_Pass" runat="server" Font-Bold="true" Font-Size="18px"
                                Width="194px" Height="28px" BorderStyle="none" TextMode="Password" ></cc1:TB>
                </td>
               </tr>
               <tr>
                <td colspan='2' align="center">
                   <asp:CheckBox ID="IsRemember" runat="server" Checked="true"/>是否记住密码
                   <asp:HiddenField ID="hidIsRememberedPass" runat="server"></asp:HiddenField>
                </td>
               </tr>
               <tr  style=" text-align:center">
                <td colspan="2" class="style8">
                  <cc1:Btn ID="Btn1" runat="server" Style="border-style: none; border-color: inherit; border-width: medium; background: url('Img/btlogin.png'); width: 108px; 
                                height: 33px; top: 251px; left: 32px; right: 196px;" 
                        OnClick="Btn1_Click">
                  </cc1:Btn>&nbsp;&nbsp;&nbsp;&nbsp;        
                  </td>      
                </tr>
               <tr>
               <td></td><td></td>
               </tr>
            </table>
			</div>
		</div>
      </div>
	</section>
    <footer id="footer" class="footer">
		<div class="footer-inner" id="footerInner">			
			<nav class="footerNav">
                 <%=BP.Sys.SystemConfig.SysName %>
			</nav>
		</div>
	</footer>
    </form>
</body>
</html>
