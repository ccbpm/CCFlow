var  CurrEnsName ; // 类名称．　实体名称．
var  CurrKeys; // 外键，主键，枚举类型形成的 string.
var  WebPath; // 虚拟当前的虚拟目录.

function TROver(ctrl,path,id,clsName,keys)
{
   ctrl.style.backgroundColor='LightSteelBlue';
   //OnDGMousedown(path,id,clsName,keys);
    CurrEnsName = clsName;
    CurrKeys = keys;
    WebPath = path;
}

/*在Datagirn */
function OnDGMousedown(path, id, clsName, keys)
{
   if ( event.button != 2)
      return true;
    CurrEnsName = clsName;
    CurrKeys = keys;
    WebPath = path;
    ShowMenu( id,clsName,keys);
    document.oncontextmenu=new Function("event.returnValue=false;");
}

/* 打开方法影射. */
function RefMethod( index , warning, target, ensName )
{
 if (CurrEnsName==null)
      return;

 if (warning==null ||  warning=='' )
 {
   
 }
 else
 {
 
  if ( confirm(  warning ) ==false)   
      return false;
 }
  var url=WebPath+"/Comm/RefMethod.htm?Index="+index+"&EnsName="+ ensName + CurrKeys;
  //alert( url );
    if (target==null)
      var a=window.location.href=url; 
    else
      var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no'); 
   return true;
}

/* 打开方法影射. */
function RefMethodExt(  index , warning, target, ensName, key , path)
{

 if (warning==null ||  warning==''  )
 {
 }
 else
 {
   if ( confirm(  warning ) ==false)   
      return ;
 }
 
  var url=path+"/Comm/RefMethod.htm?Index="+index+"&EnsName="+ ensName +"&"+ key;
 // alert (url);
    if (target==null)
      var a=window.location.href=url; 
    else
      var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no'); 
   return ;
}


/*编辑一对多的关系*/
function EditOneVsM( vsMName , attrKey )
{
 
 if (CurrEnsName==null)
      return ;
       
   var url=WebPath+'/Comm/'+'UIEn1ToM.aspx?EnsName='+CurrEnsName+"&AttrKey="+attrKey+CurrKeys;
   //alert (url );
   var a=window.showModalDialog( url , 'OneVs' ,'dialogHeight: 600px; dialogWidth: 800px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no'); 
   
  // WinShowModalDialog(url,'onevsM');
  //EnsName=BP.Port.Emp&PK=1&AttrKey=BP.Port.EmpDepts
  //var newWindow=window.open( WebPath+'/Comm/'+'UIEn1ToM.aspx?EnsName='+CurrEnsName+"&AttrKey="+attrKey+CurrKeys,'chosecol', 'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
  // newWindow.focus();
   return true;
}

/*编辑一对多的关系*/
function EditOneVsM1(path, enName, vsMName, attrKey, ensName, keys) {
    var url = 'UIEn1ToM.aspx?EnName=' + enName + '&EnsName=' + ensName + "&AttrKey=" + attrKey + keys;
    var a = window.showModalDialog(url, 'OneVs', 'dialogHeight: 600px; dialogWidth: 800px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
    return true;
}
/*编辑明晰 */
function EditDtl( dtlName, refKey)
{
 if (CurrEnsName==null)
      return ;
   var newWindow=window.open( WebPath+'/Comm/UIEnDtl.aspx?EnsName='+dtlName+"&RefKey="+refKey+"&MainEnsName="+ CurrEnsName +CurrKeys,'chosecol', 'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return true;
}

/*编辑明晰 */
function EditDtl1(path, ensName, enName, dtlName, refKey, enKeys)
{
 if (ensName==null)
     return;
 var newWindow = window.open(path + 'WF/Comm/RefFunc/Dtl.aspx?EnsName=' + dtlName + "&EnName=" + enName + "&RefKey=" + refKey + "&MainEnsName=" + ensName + enKeys, 'chosecol', 'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false');
   newWindow.focus();
   return true;
}

/*　相关功能 */
function EnsRefFunc( OID )
{
 if (CurrEnsName==null)
      return ;

   var newWindow=window.open( WebPath+'/Comm/RefFuncLink.aspx?RefFuncOID='+OID+'&MainEnsName='+ CurrEnsName +CurrKeys,'chosecol', 'width=100,top=400,left=400,height=50,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
  return true;
}
/*　相关功能 */
function Delete()
{
 if (CurrEnsName==null)
      return ;      
      
 if (confirm('Will delete it , are you sure? ')==false)
     return;
     
   var url=WebPath+'/Comm/'+'FuncLink.aspx?Flag=DeleteEn&MainEnsName='+ CurrEnsName +CurrKeys;
   var a=window.showModalDialog( url, 'OneVs' ,'dialogHeight: 400px; dialogWidth: 500px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no'); 
   window.localhost.reload();
   //width="+width+",top="+top+",left="+left+",height="+height+"
}
function Update()
{
 if (CurrEnsName==null)
      return ;
  return true;
}
function New()
{
   if (CurrEnsName==null)
      return ;
   var url=WebPath+"/Comm/"+'UIEn.aspx?EnName='+CurrEnsName; 
   var newWindow=window.open( url,'card', 'width=850,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return;
}

function Card()
{
  if (CurrEnsName==null)
      return ;   
    OpenCard(WebPath, CurrEnsName , CurrKeys ) ;
}
function Adjunct( className )
{
  if (CurrEnsName==null)
      return ; 
      
   var newWindow=window.open( WebPath+'/Comm/'+'FileManager.aspx?EnsName='+className+CurrKeys,'files', 'width=900,top=60,left=60,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return;
}

function GroupBy(className, key)
{
   var newWindow=window.open( 'Search.aspx?EnsName='+className+'&GroupKey='+key, 'mainfrm', 'width=900,top=60,left=60,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
}

function GroupEns( className )
{
 if (CurrEnsName==null)
      return ;
   //document.location='UIEnsCols.aspx?EnsName='+className;
   var newWindow=window.open( WebPath+'/Comm/Group.htm?EnsName='+className ,'mainfrm', 'width=900,top=60,left=60,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   return true;
}


function DataCheck( className )
{
  if (CurrEnsName==null)
      return ; 
   
   //alert('提示：\\n 数据体检是为系统管理员或者高级用户设置的，用来监测数据质量。');
   var newWindow=window.open( WebPath+'/Comm/Sys/SystemClassDtl.aspx?Type=Check&EnsName='+className+CurrKeys,'files', 'width=900,top=60,left=60,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return;
}
 

function UIEnsCols( className )
{
 if (CurrEnsName==null)
      return ;
   //document.location='UIEnsCols.aspx?EnsName='+className;
   var newWindow=window.open( WebPath+'/Comm/UIEnsCols.aspx?EnsName='+className ,'mainfrm', 'width=900,top=60,left=60,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   return true;   
}

/* 隐藏 Menum */
function HideMenu( id )
{
   document.getElementById( id ).style.visibility='hidden';
}

/* 显示 Menum */
function ShowMenu( id ,className, keys)
{
   var rightedge=document.body.clientWidth-event.clientX;
   var bottomedge=document.body.clientHeight-event.clientY;
   
   //菜单定位
   if (rightedge < document.getElementById( id ).offsetWidth )
      document.getElementById( id ).style.left=document.body.scrollLeft+event.clientX-document.getElementById( id ).offsetWidth;
   else
      document.getElementById( id ).style.left=document.body.scrollLeft+event.clientX;

   if (bottomedge< document.getElementById( id ).offsetHeight)
      document.getElementById( id ).style.top=document.body.scrollTop+event.clientY-document.getElementById( id ).offsetHeight;
   else
      document.getElementById( id ).style.top=document.body.scrollTop+event.clientY;
   document.getElementById( id ).style.visibility="visible";
//   document.body.onclick=HideMenu(id);
   return false;
}

/* */ 
function MTROn1(ctrl)
{
   ctrl.style.backgroundColor='royalblue';
}
function MTROut1(ctrl)
{
   ctrl.style.backgroundColor='Menu';
}




 


