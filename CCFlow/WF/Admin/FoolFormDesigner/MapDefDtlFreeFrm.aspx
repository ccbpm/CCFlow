<%@ Page Title="ccform从表设计器" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_MapDefDtlCCForm" Codebehind="MapDefDtlFreeFrm.aspx.cs" %>
<%@ Import Namespace="BP.Sys" %>
 <%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<base target="_self" />
<script type="text/javascript" language="javascript">
	function HelpGroup()
	{
	   var msg='字段分组：就是把类似的字段放在一起，让用户操作更友好。\t\n比如：我们纳税人设计一个基础信息采集节点。';
	   msg+='在登记纳税人基础信息时，我们可以把基础信息、车船信息、房产信息、投资人信息分组。\t\n \t\n分组的格式为:@从字段名称1=分组名称1@从字段名称2=分组名称2 ,\t\n比如：节点信息设置，@NodeID=基本信息@LitData=考核信息。';
       alert( msg);
	}
	function DoGroupF( enName)
	{
	    var b=window.showModalDialog( 'GroupTitle.aspx?EnName='+enName , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
	}
	function Insert(fk_mapdata,idx)
    {
        var url = 'FieldTypeList.aspx?FK_MapData=' + fk_mapdata + '&IDX=' + idx;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function AddF(fk_mapdata) {

        var url = 'FieldTypeList.aspx?FK_MapData=' + fk_mapdata;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 

        window.location.href = window.location.href;
    }
    function AddTable(mypk)
    {
        var url='EditCells.aspx?MyPK='+mypk;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function MapExt(mypk) {
        var url = 'MapExt.aspx?FK_MapData=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 800px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function CopyFieldFromNode(mypk)
    {
        var url='CopyFieldFromNode.aspx?DoType=AddF&FK_Node='+mypk;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 700px; dialogWidth: 900px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function HidAttr(mypk) {
        alert(mypk);
        var url = 'HidAttr.aspx?FK_MapData=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 700px; dialogWidth: 900px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function GroupFieldNew(mypk)
    {
        var url='GroupField.aspx?RefNo='+mypk+"&RefOID=0&DoType=FunList";
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function GroupField(mypk, OID )
    {
        var url='GroupField.aspx?RefNo='+mypk+"&RefOID="+OID ;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function GroupFieldDel(mypk,refoid)
    {
        var url='GroupField.aspx?RefNo='+mypk+'&DoType=DelIt&RefOID='+refoid ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }

    function Edit(fk_mapdata, mypk, ftype)
    {
        var url = 'EditF.aspx?DoType=Edit&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&FType=' + ftype;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function EditEnum(mypk,refno)
    {
        var url='EditEnum.aspx?DoType=Edit&MyPK='+mypk+'&RefNo='+refno;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
     function EditTable(mypk,refno)
    {
        var url='EditTable.aspx?DoType=Edit&MyPK='+mypk+'&RefNo='+refno;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    
	function Up(mypk,refoid,idx)
    {
        var url='Do.aspx?DoType=Up&MyPK='+mypk+'&RefNo='+refoid+'&ToIdx='+idx;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        //window.location.href ='Designer.aspx?PK='+mypk+'&IsOpen=1';
        window.location.href = window.location.href ;
    }
    function Down(mypk, myfield, idx)
    {
       // alert(mypk );
        var url='Do.aspx?DoType=Down&MyPK='+mypk+'&RefNo='+myfield +'&ToIdx='+idx;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function GFDoUp(refoid)
    {
        var url='Do.aspx?DoType=GFDoUp&RefOID='+refoid ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href ;
    }
    function GFDoDown(refoid)
    {
        var url='Do.aspx?DoType=GFDoDown&RefOID='+refoid ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }

    function FrameDoUp(MyPK) {
        var url = 'Do.aspx?DoType=FrameDoUp&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function FrameDoDown(MyPK) {
        var url = 'Do.aspx?DoType=FrameDoDown&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }

    function DtlDoUp(MyPK)
    {
        var url='Do.aspx?DoType=DtlDoUp&MyPK='+MyPK ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href ;
    }
    function DtlDoDown(MyPK)
    {
        var url='Do.aspx?DoType=DtlDoDown&MyPK='+MyPK;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }

    function M2MDoUp(MyPK) {
        var url = 'Do.aspx?DoType=M2MDoUp&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function M2MDoDown(MyPK) {
        var url = 'Do.aspx?DoType=M2MDoDown&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }

    function Del(mypk,refoid)
    {
        if (window.confirm('您确定要删除吗？') ==false)
            return ;
    
        var url='Do.aspx?DoType=Del&MyPK='+mypk+'&RefOID='+refoid;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
	function Esc()
    {
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

  function EditDtl(mypk, dtlKey) {
      var url = 'MapDtl.aspx?DoType=Edit&FK_MapData=' + mypk + '&FK_MapDtl=' + dtlKey;
      var b = window.showModalDialog(url, 'ass', 'dialogHeight: 600px; dialogWidth: 700px;center: yes; help:no;resizable:yes');
     // var b = window.showModalDialog(url, 'ass', 'dialogHeight: 700px; dialogWidth: 800px;center: yes; help:no;resizable:yes');
      window.location.href = window.location.href;
  }

  function EditM2M(mypk, dtlKey) {
      var url = 'MapM2M.aspx?DoType=Edit&FK_MapData=' + mypk + '&NoOfObj=' + dtlKey;
      var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
      window.location.href = window.location.href;
  }
  
  function MapDtl( mypk  )
  {
      var url='MapDtl.aspx?DoType=DtlList&FK_MapData=' + mypk   ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    
    /// 多选.
    function MapM2M(mypk) {
        var url = 'MapM2M.aspx?DoType=List&FK_MapData=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
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
    //然浏览器最大化.
    function ResizeWindow() {
        if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
            var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
            var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
            window.moveTo(0, 0);           //把window放在左上角     
            window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
        }
    }
    window.onload = ResizeWindow;
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server"  />
    <%
        MapDtl dtl = new MapDtl();
        dtl.No = this.FK_MapDtl;
        if (dtl.RetrieveFromDBSources() == 0)
        {
            dtl.FK_MapData = this.FK_MapData;
            dtl.Name = this.FK_MapData;
            dtl.Insert();
            dtl.IntMapAttrs();
        }

        if (this.FK_Node != 0)
        {
            /* 如果传递来了节点信息, 就是说明了独立表单的节点方案处理, 现在就要做如下判断.
             * 1, 如果已经有了.
             */
            dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
            if (dtl.RetrieveFromDBSources() == 0)
            {

                // 开始复制它的属性.
                MapAttrs attrs = new MapAttrs(this.FK_MapDtl); 

                //让其直接保存.
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                dtl.FK_MapData = "Temp";
                dtl.DirectInsert(); //生成一个明细表属性的主表.
                
                //循环保存字段.
                int idx = 0;
                foreach (MapAttr item in attrs)
                {
                    item.FK_MapData = this.FK_MapDtl + "_" + this.FK_Node;
                    item.MyPK = item.FK_MapData + "_" + item.KeyOfEn;
                    item.Save();
                    idx++;
                    item.Idx = idx;
                    item.DirectUpdate();
                }

                MapData md = new MapData();
                md.No = "Temp";
                if (md.IsExits == false)
                {
                    md.Name = "为权限方案设置的临时的数据";
                    md.Insert();
                }
            }

            this.Response.Redirect("MapDefDtlFreeFrm.aspx?FK_MapDtl="+dtl.No+"&FK_MapData=Temp", true);
            return;
        }
        
         %>
    <div class='easyui-layout' data-options='fit:true'>
        <div data-options="region:'north',noheader:true,split:false,border:false" style='height:30px;overflow-y:hidden'>
            <div style='float:left'>
                <a href="javascript:EditDtl('<%=this.FK_MapData %>','<%=dtl.No %>')" class='easyui-linkbutton' data-options="iconCls:'icon-edit',plain:true"><%=dtl.Name %></a>
            </div>
            <div style='float:right'>
                <a href="javascript:document.getElementById('F<%=dtl.No %>').contentWindow.AddF('<%=dtl.No %>');" class='easyui-linkbutton' data-options="iconCls:'icon-new',plain:true">插入列</a>
                <a href="javascript:document.getElementById('F<%=dtl.No %>').contentWindow.AddFGroup('<%=dtl.No %>');" class='easyui-linkbutton' data-options="iconCls:'icon-new',plain:true">插入列组</a>
                <a href="javascript:document.getElementById('F<%=dtl.No %>').contentWindow.CopyF('<%=dtl.No %>');" class='easyui-linkbutton' data-options="iconCls:'icon-add',plain:true">复制列</a>
                <a href="javascript:document.getElementById('F<%=dtl.No %>').contentWindow.HidAttr('<%=dtl.No %>');" class='easyui-linkbutton' data-options="iconCls:'icon-add',plain:true">隐藏列</a>
                <a href="javascript:document.getElementById('F<%=dtl.No %>').contentWindow.DtlMTR('<%=dtl.No %>');" class='easyui-linkbutton' data-options="iconCls:'icon-add',plain:true">多表头</a>
                <a href='Action.aspx?FK_MapData=<%=this.FK_MapDtl %>' class='easyui-linkbutton' data-options="iconCls:'icon-add',plain:true">从表事件</a>
                <a href="javascript:DtlDoUp('<%=dtl.No %>')" class='easyui-linkbutton' data-options="iconCls:'icon-up',plain:true"></a>
                <a href="javascript:DtlDoDown('<%=dtl.No %>')" class='easyui-linkbutton' data-options="iconCls:'icon-down',plain:true"></a>
            </div>
            <div style='clear:both'></div>
        </div>
        <div data-options="region:'center',noheader:true,border:false" style="overflow-y:hidden">
            <iframe ID='F<%=dtl.No %>' frameborder='0' scrolling="auto" style='width:100%;height:100%' src='MapDtlDe.aspx?DoType=Edit&FK_MapData=<%=this.FK_MapData %>&FK_MapDtl=<%=dtl.No %>'></iframe>
        </div>
    </div>
</asp:Content>

