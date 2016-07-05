<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" 
Inherits="CCFlow.WF.MapDef.WF_MapDef_MapDef" Title="ccflow傻瓜表单设计器" Codebehind="MapDef.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
body
{
    font-size: .80em;
    font-family: "Helvetica Neue", "Lucida Grande", "Segoe UI", Arial, Helvetica, Verdana, sans-serif;
    margin: 0px;
    padding: 0px;
    color: #696969;
}
</style>
    <script src="./../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="../Comm/JScript.js"></script>
    <script language="JavaScript" src="MapDef.js" type="text/javascript" ></script>
    <script language="JavaScript" src="./../Style/Verify.js"></script>
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript" defer="defer" ></script>
    <script language="javascript"  >
        function FrmEvent(mypk) {
            var url = 'FrmEvent.aspx?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        }
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
	function Insert(mypk,IDX)
    {
        var url='Do.aspx?DoType=AddF&MyPK='+mypk+'&IDX=' +IDX ;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
	function AddF(mypk) {
        var url='Do.aspx?DoType=AddF&MyPK='+mypk;
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
    function BatchEdit(mypk) {
        var url = 'BatchEdit.aspx?FK_MapData=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 800px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function MapDataEdit(mypk) {
        var url = 'EditMapData.aspx?FK_MapData=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 350px; dialogWidth: 500px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function CopyFieldFromNode(mypk) {
        var url = 'CopyFieldFromNode.aspx?DoType=AddF&FK_Node=' + mypk;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 700px; dialogWidth: 900px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function GroupFieldNew(mypk)
    {
        var url='GroupField.aspx?RefNo='+mypk+"&RefOID=0&DoType=FunList";
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    function ExpImp(mypk, fk_flow) {
        var url = 'ExpImp.aspx?RefNo=' + mypk + "&RefOID=0&DoType=FunList&FK_Flow=" + fk_flow;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
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
   
    function Edit(mypk,refno, ftype)
    {
        var url='EditF.aspx?DoType=Edit&MyPK='+mypk+'&RefNo='+refno +'&FType=' + ftype;
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
    
	function Up(mypk,refoid,idx,t) {
        var url='Do.aspx?DoType=Up&MyPK='+mypk+'&RefNo='+refoid+'&ToIdx='+idx+'&T='+t;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no'); 
        //window.location.href ='MapDef.aspx?PK='+mypk+'&IsOpen=1';
        window.location.href = window.location.href ;
    }
    function Down(mypk,refoid,idx,t)
    {
        var url = 'Do.aspx?DoType=Down&MyPK=' + mypk + '&RefNo=' + refoid + '&ToIdx=' + idx + '&T=' + t;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 50px; dialogWidth: 50px;center: yes; help: no'); 
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
     // var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
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
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
        window.location.href = window.location.href;
    }

    function AthDoUp(MyPK) {
        var url = 'Do.aspx?DoType=AthDoUp&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function AthDoDown(MyPK) {
        var url = 'Do.aspx?DoType=AthDoDown&MyPK=' + MyPK;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
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
    <style type="text/css">
div#nv
{
    top: 0;
    left: 0;
    width: auto;
    text-align: center;
    padding: 0px;
}

div.wrapper
{
    padding: 3px;
    margin: 1px;
}
 
div#nv ul
{
    width: 100%;
    margin: 3;
    padding: 3;
    text-align: center;
}
div#nv li
{
    float: left;
    display: inline;
    list-style: none;
    margin: 0;
    padding: 0 6px;
    line-height: 1em;
    background-position: right center;
    background-repeat: no-repeat;
    margin: 3;
    padding: 3;
    text-align: center;
}

div#nv li.last
{
    background: none;
}

div#nv a,
div#nv a:link,
div#nv a:active,
div#nv a:visited {
  display: inline-block;
  /* hide from ie/mac \*/
  display: block;
  /* end hide */
  font-weight: bold;
  text-decoration: none;
  margin: 0;
}

div#nv a:hover, .S
{
    background-position: right center;
    background-repeat: no-repeat;
    margin: 3;
    padding: 3;
}

body
{
    font-size:small;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
                <table style="width:100%;height:auto;"  border="1px" >
                <caption  >  <uc1:Pub ID="UCCaption" runat="server" /> </caption>
                 <tr>
                  <td valign="top" align=left  bgcolor="#ffffff">
                   <uc1:Pub ID="Left" runat="server" />
                 </td>

                  <td valign=top  style="width:90%;" >
                   <uc1:Pub ID="Pub1" runat="server" />
                 </td>

                 </tr>
                </table>
</asp:Content>
