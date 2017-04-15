<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConditionLine.aspx.cs"
    Inherits="CCFlow.WF.Admin.ConditionLine" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>条件设置</title>
    <link href="../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="javascript">
        var currShow;

        //在右侧框架中显示指定url的页面
        function OpenUrlInRightFrame(ele, url) {
            if (ele != null && ele != undefined) {
                //if (currShow == $(ele).text()) return;

                currShow = $(ele).parents('li').text(); //有回车符

                $.each($('ul.navlist'), function () {
                    $.each($(this).children('li'), function () {
                        $(this).children('div').css('font-weight', $(this).text() == currShow ? 'bold' : 'normal');
                    });
                });

                $('#context').attr('src', url + '&s=' + Math.random());
            }
        }

        $(document).ready(function () {
            $('ul.navlist').find("a[id='a<%=this.ToNodeId %>']").click();
        });
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',border:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'west',split:true,title:'节点方向'" style="width: 300px;">
                <ul class="navlist">
                    <asp:Repeater ID="rptLines" runat="server">
                        <ItemTemplate>
                            <li>
                                <div>
                                    <a id='a<%# Eval("ToNode") %>' href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, 'Condition.htm?CondType=<%=this.CondType %>&FK_Flow=<%=this.FK_Flow %>&FK_MainNode=<%# Eval("Node") %>&FK_Node=<%# Eval("Node") %>&FK_Attr=<%=this.FK_Attr %>&DirType=<%# Eval("DirType") %>&ToNodeID=<%# Eval("ToNode") %>')">
                                        <span class="nav">到:<%# Eval("ToNodeName") %></span></a></div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <li>
                        <div>
                            <a href="javascript:void(0)" onclick="OpenUrlInRightFrame(this, 'CondPRI.aspx?CondType=<%=this.CondType %>&FK_Flow=<%=this.FK_Flow %>&FK_MainNode=<%=FK_MainNode %>&FK_Node=<%=this.FK_Node %>&FK_Attr=<%=this.FK_Attr %>&DirType=<%=this.DirType %>&ToNodeID=<%=this.ToNodeId %>')">
                                <span class="nav">优先级设置</span></a></div>
                    </li>
                </ul>
            </div>
            <div data-options="region:'center',noheader:true" style="overflow-y: hidden">
                <iframe id="context" scrolling="auto" frameborder="0" src="" style="width: 100%;
                    height: 100%;"></iframe>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
