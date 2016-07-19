<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_MapDef"
    Title="ccform傻瓜表单设计器" CodeBehind="Designer.aspx.cs" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        body
        {
            font-size: .80em;
            font-family: "Helvetica Neue" , "Lucida Grande" , "Segoe UI" , Arial, Helvetica, Verdana, sans-serif;
            margin: 0px;
            padding: 0px;
            color: #696969;
        }
    </style>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript" src="../../Comm/JScript.js"></script>
    <script language="JavaScript" type="text/javascript" src="MapDef.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript" src="../../Style/Verify.js"></script>
    <script language="JavaScript" type="text/javascript" src="../../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"
        defer="defer"></script>
    <script language="javascript" type="text/javascript" >
        function FrmEvent(mypk) {
            var url = 'FrmEvent.aspx?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        }
        function HelpGroup() {
            var msg = '字段分组：就是把类似的字段放在一起，让用户操作更友好。\t\n比如：我们纳税人设计一个基础信息采集节点。';
            msg += '在登记纳税人基础信息时，我们可以把基础信息、车船信息、房产信息、投资人信息分组。\t\n \t\n分组的格式为:@从字段名称1=分组名称1@从字段名称2=分组名称2 ,\t\n比如：节点信息设置，@NodeID=基本信息@LitData=考核信息。';
            alert(msg);
        }
        function DoGroupF(enName) {
            var b = window.showModalDialog('GroupTitle.aspx?EnName=' + enName, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Insert(mypk, IDX) {
            var url = 'FieldTypeList.aspx?DoType=AddF&MyPK=' + mypk + '&IDX=' + IDX;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function AddF(fk_mapdata) {
            //var url = 'Do.aspx?DoType=AddF&MyPK=' + mypk;
            var url = 'FieldTypeList.aspx?DoType=AddF&FK_MapData=' + fk_mapdata;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function AddField(fk_mapdata, groupID) {
            var url = 'FieldTypeList.aspx?DoType=AddF&FK_MapData=' + fk_mapdata+'&GroupField='+groupID;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function AddTable(mypk) {
            var url = 'EditCells.aspx?MyPK=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function MapExt(mypk) {
            var url = 'MapExt.aspx?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 800px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function BatchEdit(mypk) {
            var url = 'BatchEdit.aspx?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 800px;center: yes; help: no');
            window.location.href = window.location.href;
        }
       
        function CopyFieldFromNode(mypk) {
            var url = 'CopyFieldFromNode.aspx?DoType=AddF&FK_Node=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 700px; dialogWidth: 900px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function GroupFieldNew(mypk) {
            var url = 'GroupField.aspx?FK_MapData=' + mypk + "&RefOID=0&DoType=FunList";
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function ExpImp(mypk, fk_flow) {
            var url = 'ExpImp.aspx?RefNo=' + mypk + "&RefOID=0&DoType=FunList&FK_Flow=" + fk_flow;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function GroupField(mypk, OID) {
            var url = 'GroupField.aspx?FK_MapData=' + mypk + "&GroupField=" + OID;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function GroupFieldDel(mypk, refoid) {
            var url = 'GroupField.aspx?RefNo=' + mypk + '&DoType=DelIt&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function Edit(fk_mapdata, mypk, ftype) {
            var url = 'EditF.aspx?DoType=Edit&MyPK=' + mypk + '&FType=' + ftype + '&FK_MapData='+fk_mapdata;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EditEnum(mypk, refno) {
            var url = 'EditEnum.aspx?DoType=Edit&MyPK=' + mypk + '&RefNo=' + refno;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EditTable(mypk, refno) {
            var url = 'EditTable.aspx?DoType=Edit&MyPK=' + mypk + '&RefNo=' + refno;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function Up(mypk, refoid, idx, t) {
            var url = 'Do.aspx?DoType=Up&MyPK=' + mypk + '&RefNo=' + refoid + '&ToIdx=' + idx + '&T=' + t;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no');
            //window.location.href ='Designer.aspx?PK='+mypk+'&IsOpen=1';
            window.location.href = window.location.href;
        }
        function Down(mypk, refoid, idx, t) {
            var url = 'Do.aspx?DoType=Down&MyPK=' + mypk + '&RefNo=' + refoid + '&ToIdx=' + idx + '&T=' + t;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function GFDoUp(refoid) {
            var url = 'Do.aspx?DoType=GFDoUp&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function GFDoDown(refoid) {
            var url = 'Do.aspx?DoType=GFDoDown&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no');
            window.location.href = window.location.href;
        }
       

        function Del(mypk, refoid) {
            if (window.confirm('您确定要删除吗？') == false)
                return;

            var url = 'Do.aspx?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }

        function GroupBarClick(rowIdx) {

            var alt = document.getElementById('Img' + rowIdx).alert;
            var sta = 'block';
            if (alt == 'Max') {
                sta = 'block';
                alt = 'Min';
            } else {
                sta = 'none';
                alt = 'Max';
            }

            document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
            document.getElementById('Img' + rowIdx).alert = alt;
            var i = 0
            for (i = 0; i <= 40; i++) {
                if (document.getElementById(rowIdx + '_' + i) == null)
                    continue;
                if (sta == 'block') {
                    document.getElementById(rowIdx + '_' + i).style.display = '';
                } else {
                    document.getElementById(rowIdx + '_' + i).style.display = sta;
                }
            }
        }

        var isInser = "";

        function CopyFieldFromNode(mypk) {
            var url = 'CopyFieldFromNode.aspx?FK_Node=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 700px; dialogWidth: 900px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        // 子线程.
        function EditFrmThread(mypk) {
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmTrack&PK=' + mypk
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }

        // 轨迹图.
        function EditTrack(mypk) {
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmTrack&PK=' + mypk
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }

     
        /// 审核组件.
        function EditFWC(mypk) {
            //http: //localhost:41466/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.FrmNodeComponents&PK=7901&EnName=BP.WF.Template.FrmNodeComponent&tab=%E7%88%B6%E5%AD%90%E6%B5%81%E7%A8%8B%E7%BB%84%E4%BB%B6
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=审核组件';
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }
        //子流程.
        function EditSubFlow(mypk) {
            // var url = '../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=父子流程组件';
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }

        //子线程.
        function EditThread(mypk) {
            // var url = '../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=子线程组件';
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }

        //轨迹组件.
        function EditTrack(mypk) {
            // var url = '../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=轨迹组件';
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            window.location.href = window.location.href;
        }


        function MapDataEdit(mypk) {
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.MapFoolForm&PK=' + mypk
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 850px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function FrmNodeComponent(mypk) {
            var url = '../../Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 850px;center: yes; help: no');
            window.location.href = window.location.href;
        }


        function EditDtl(mypk, dtlKey) {
            var url = 'MapDtl.aspx?DoType=Edit&FK_MapData=' + mypk + '&FK_MapDtl=' + dtlKey;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 600px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
            // var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function EditM2M(mypk, dtlKey) {
            var url = 'MapM2M.aspx?DoType=Edit&FK_MapData=' + mypk + '&NoOfObj=' + dtlKey;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function MapDtl(mypk) {
            var url = 'MapDtl.aspx?DoType=DtlList&FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        /// 多选.
        function MapM2M(mypk) {
            var url = 'MapM2M.aspx?DoType=List&FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function MapM2MM(mypk) {
            var url = 'MapM2MM.aspx?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EditAth(fk_mapdata, ath) {
            var url = 'Attachment.aspx?FK_MapData=' + fk_mapdata + '&Ath=' + ath;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Ath(mypk) {
            var url = 'Attachment.aspx?DoType=List&FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 800px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function EditFrame(mypk, dtlKey) {
            var url = 'MapFrame.aspx?DoType=Edit&FK_MapData=' + mypk + '&FK_MapFrame=' + dtlKey;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function MapFrame(mypk) {
            var url = 'MapFrame.aspx?DoType=DtlList&FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function HidAttr(fk_mapData) {
            var url = 'HidAttr.aspx?FK_MapData=' + fk_mapData;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EnableAthM(fk_MapDtl) {
            var url = '../CCForm/AttachmentUpload.aspx?IsBTitle=1&PKVal=0&Ath=AthMDtl&FK_MapData=' + fk_MapDtl + '&FK_FrmAttachment=' + fk_MapDtl + '_AthMDtl';
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            //  window.location.href = window.location.href;
        }
        function Sln(fk_mapdata) {
            var url = 'Sln.aspx?IsBTitle=1&PKVal=0&Ath=AthM&FK_MapData=' + fk_mapdata + '&FK_FrmAttachment=' + fk_mapdata + '_AthM';
            WinOpen(url);
            //var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        }

        //然浏览器最大化.
        function ResizeWindow() {
            if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
                var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
                var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
                window.moveTo(0, 0);           //把window放在左上角     
                window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
            }
        }
        window.onload = ResizeWindow();
    </script>
    <base target="_self" />
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-panel" style="padding: 5px;">
    <%
        string fk_mapdata = this.Request.QueryString["FK_MapData"];
        string fk_flow = this.Request.QueryString["FK_Flow"];
       // int nodeID = 0  this.Request.QueryString["FK_Flow"];
        string  nodeID = this.Request.QueryString["NodeID"];
         %>
        <a href="javascript:ExpImp('<%=fk_mapdata %>','<%=fk_flow%>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-copy'">导入/导出</a>
        <a href="javascript:AddF('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-new'">新建字段</a>
        <a href="javascript:HidAttr('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-hidden'">隐藏字段</a>
        <a href="javascript:GroupFieldNew('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-groupbar'">新建字段分组</a>
        <a href="javascript:MapDtl('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-dtl'">新建从表</a>
        <a href="javascript:Ath('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-attachment'">新建附件组件</a>
        <a href="javascript:MapFrame('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-frame'">新建框架</a>
        
        <%--<a href="javascript:MapExt('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-dts'">扩展设置</a>
--%>
        <a href="javascript:MapDataEdit('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-property'">表单属性</a>

        <% if ( fk_flow != null) { %>
        <a href="javascript:FrmNodeComponent('<%=this.NodeID %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-Components'"> 节点表单组件</a>
        <% } %>

       <uc1:Pub ID="UCCaption" runat="server" />
    </div>

    <div align="center"  style=" vertical-align:middle;background-color:#d0d0d0;height:100%;padding-top:10px;padding-bottom:10px;" >
     <uc1:Pub ID="Pub1" runat="server" />
    </div>

</asp:Content>
