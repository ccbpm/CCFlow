<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=BP.Sys.SystemConfig.SysName %></title>
    <%--<style type="text/css">
      dds  
       * { margin:0; padding:0;}
        html, body, form { width:100%; height:100%; font-family:"微软雅黑"; }
        body { background:#efefef;}
        .bg { width:980px; height:567px; top:20px; position:relative; background:url(Img/LoginBJ.jpg) no-repeat 0px 0px; margin:auto; border-left:1px solid #333; border-right:1px solid #333;}
        .login { position:absolute; left:590px; top:250px; height:200px; width:350px; overflow:hidden;}
    </style>--%>
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
                height: 19px;
                margin-left:10px;
            }
		    .style5
            {
                width: 102px;
                height: 11px;
            }
            #divMain{ display:block;margin:0;padding:0;border:none ;height:440px;margin:0 auto;background:#f7f7f7;
                       background-image:url('Img/repeatimage.jpg'); background-position:inherit; background-repeat:repeat-x;}
        .style10
        {
            height: 42px;
        }
        .style12
        {
            height: 44px;
        }
        .style13
        {
            height: 20px;
        }
        .style14
        {
            height: 19px;
        }
        .style15
        {
            height: 11px;
        }
    </style>
</head>
<body>
 <div style="height:56px; width:900px; margin:0 auto;">
		<div style="float:left; width:40%; display:inline "><a><img  src="/DataUser/Icon/LogBig.png" alt="ccflow" style=" height:60px;" /></a></div>
		<div style="float:right; margin-right:100px; width:30%;height:56px; position:relative; display:inline;">
            <div style="position:absolute; bottom:0px;">        
			 
            </div>
		</div>
        <div style="clear:both;"></div>
	</div>
    <form id="Form1" method="post" runat="server">
    <section class="main" id="mainBg">
     <div id="divMain" style="background-repeat:repeat-x; width:100%; background-position:left; display:block;margin:0;padding:0;border:none" >
		<div class="main-inner" style="  background-repeat:repeat-x; background-position-x: center; background-position-y: top;" >
			<div id="theme">
            <img src="/DataUser/ICON/Login.jpg" style=" height:440px; width:900px;" />
            </div>
			<div id="loginBlock" class="login tab-1">
            <table  style="width:100%; background-color:#F1F3F8;  height: 120%;">
            <tr >
             <td colspan="2" class="style12">
             &nbsp;&nbsp;<img src="Img/yaoshi.jpg"  style=" width:29px; height:12px;" /><strong style="font-size:16px;"> 用户登录</strong>

              <hr style="width:90%; height:0.5px; color:Black" />
            </td>
            </tr>
               <tr>
                <td >
                 &nbsp;&nbsp;&nbsp;&nbsp;用户名:
                </td>
                  <td class="style14">
                   <asp:TextBox ID="txtUserName" runat="server"  Font-Bold="true"  Font-Size="14"
                            Style="line-height: 26px;
                            border: 1px solid #C8E0F8;" 
                          Width="168px"></asp:TextBox>
                </td>
               </tr>
               <tr>
                <td >
                 &nbsp;&nbsp; &nbsp;&nbsp;密&nbsp;&nbsp;码:
                </td><td class="style15">
                  <asp:TextBox ID="txtPassword" runat="server"  Font-Bold="true"  Font-Size="14"
                           Style="line-height: 26px;
                            border: 1px solid #C8E0F8;  "
                            TextMode="Password" Width="169px"></asp:TextBox>
                </td>
               </tr>
              <tr>
                <td  colspan='2' align="center" class="style13" >
                   <asp:CheckBox ID="IsRemember" runat="server" Checked="true"/>是否记住密码
                </td>
               </tr>
               <tr  style=" text-align:center">
                <td colspan="2" class="style10">
                   <asp:LinkButton ID="lbtnSubmit" runat="server" Text="登陆" 
                        OnClick="btnSubmit_Click" Style="position: absolute; width: 95px;
                height: 30px; top: 289px; left: 100px; background-image:url('Img/btlogin.png');"><strong style="font-size:16px;"></strong></asp:LinkButton>
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
                <span class="copyright"> <% =BP.Sys.SystemConfig.SysName %>   </span>&nbsp;&nbsp;
			</nav>
		</div>
	</footer>
    </form>
</body>
</html>
