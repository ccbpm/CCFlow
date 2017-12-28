<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncFrm.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.IncFrm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

td
	{border-style: none;
            border-color: inherit;
            border-width: medium;
            padding-top:1px;
	        padding-right:1px;
	        padding-left:1px;
	        color:black;
	        font-size:11.0pt;
	        font-weight:400;
	        font-style:normal;
	        text-decoration:none;
	        font-family:宋体;
	        text-align:general;
	        vertical-align:middle;
	        white-space:nowrap;
	}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table border="0" cellpadding="0" cellspacing="0" style="border-collapse:
 collapse;width:748pt" width="997">
            <colgroup>
                <col span="3" style="width:54pt" width="72" />
                <col style="mso-width-source:userset;mso-width-alt:4544;width:107pt" 
                    width="142" />
                <col style="width:54pt" width="72" />
                <col style="mso-width-source:userset;mso-width-alt:4064;width:95pt" 
                    width="127" />
                <col style="width:54pt" width="72" />
                <col style="mso-width-source:userset;mso-width-alt:6016;width:141pt" 
                    width="188" />
                <col style="width:54pt" width="72" />
                <col style="mso-width-source:userset;mso-width-alt:3456;width:81pt" 
                    width="108" />
            </colgroup>
            <tr height="18" style="height:13.5pt">
                <td height="18" style="height:13.5pt;width:54pt" width="72">
                    岗位</td>
                <td style="width:54pt" width="72">
                    序号</td>
                <td style="width:54pt" width="72">
                    风险点</td>
                <td style="width:107pt" width="142">
                    可能发生的后果</td>
                <td style="width:54pt" width="72">
                    风险等级</td>
                <td style="width:95pt" width="127">
                    隐患排查标准</td>
                <td style="width:54pt" width="72">
                    责任人</td>
                <td style="width:141pt" width="188">
                    排查周期</td>
                <td style="width:54pt" width="72">
                    有无隐患</td>
                <td style="width:81pt" width="108">
                    隐患级别</td>
            </tr>
            <tr height="18" style="height:13.5pt">
                <td height="18" style="height:13.5pt">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                    多选项</td>
                <td>
                </td>
                <td>
                    班检/日检/周检/月检</td>
                <td>
                    有/无</td>
                <td>
                    一般/重大</td>
            </tr>
            <tr height="18" style="height:13.5pt">
                <td height="18" style="height:13.5pt">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
    
    </div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="发送" />
&nbsp;&nbsp;&nbsp;
    <asp:Button ID="Button2" runat="server" Text="Save" />
    </form>
</body>
</html>
