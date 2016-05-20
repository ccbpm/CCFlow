<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exp.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Exp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程模版导出</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <fieldset>
    <legend> <asp:RadioButton ID="RB0" Text="导出到本地xml" Checked="true" GroupName="xxx" runat="server" />
        </legend>
    </fieldset>

    <fieldset>
    <legend> <asp:RadioButton ID="RB1" Text="导出我的私有云服务器上" GroupName="xxx" runat="server" />
        </legend>
    </fieldset>


<fieldset>
    <legend> <asp:RadioButton ID="RB2" Text="导出公用有云服务器上" GroupName="xxx" runat="server" />
        </legend>
    </fieldset>


    </div>
    <asp:Button ID="Button1" runat="server" Text=" 执 行 " />
    </form>
</body>
</html>
