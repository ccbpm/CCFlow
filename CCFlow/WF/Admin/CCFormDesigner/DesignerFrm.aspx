<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DesignerFrm.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.DesignerFrm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     
    <%
        //根据不同的表单类型转入不同的表单设计器上去.
        string fk_mapdata = this.Request.QueryString["FK_MapData"];
        BP.Sys.MapData md = new BP.Sys.MapData(fk_mapdata);

        if (md.HisFrmType == BP.Sys.FrmType.Column4Frm)
        {
            /*傻瓜表单*/
            this.Response.Redirect("../MapDef/MapDef.aspx?IsFirst=1&FK_MapData=" + fk_mapdata, true);
            return;
        }

        if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
        {
            /*自由表单*/
            this.Response.Redirect("FormDesigner.aspx?FK_MapData=" + fk_mapdata, true);
            return;
        }
        
         
       %>
    </div>
    </form>
</body>
</html>
