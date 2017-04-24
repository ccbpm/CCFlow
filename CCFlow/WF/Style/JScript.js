/* 默认植问题 */
function OpenHelperTBNo( appPath ,  EnsName, ctl  )
{ 
	// alert( EnsName);
	// alert( ctl );
    var url =appPath+ '/Comm/DataHelp.htm?' +appPath+ '/Comm/HelperOfTBNo.aspx?EnsName='+EnsName ;
    var str= window.showModalDialog(url , '','dialogHeight: 550px; dialogWidth:950px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    if (str==undefined )
      return;
    if (str==null)
      return;
      
     ctl.value = str;
     return;
}
function To(url) {
    window.location.href = url;
}
window.onerror=function()
{
  return true;
}
/*打开Opencard*/
function OpenCard(webAppPath, className, url)
{
   var url1=webAppPath+'/Comm/En.htm?EnName='+className+url;
   var newWindow=window.open( url1,'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return;
}
function CardItem3()
{
  if (CurrEnsName==null)
      return ;   
    OpenItme3(WebPath, CurrEnsName , CurrKeys ) ;
}

function OpenItme3(webAppPath, className, url)
{
   var url1=webAppPath+"/Comm/"+'Item3.aspx?EnName='+className+url;
   var newWindow=window.open( url1,'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   newWindow.focus();
   return;
}

function Helper()
{
  // alert('Thanks for you chose bpsoft! more info please visit helper.bpsoft.net. ');
   newWindow=window.open('/Helper/Helper.htm','helper','width=600,top=10,left=10,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false');
   newWindow.focus();  

  // var newWindow=window.open( url,'card', 'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
}
function Print( url )
{
   newWindow=window.open( url ,'p','width=0,top=10,left=10,height=1,scrollbars=yes,resizable=yes,toolbar=yes,location=yes' );
   newWindow.focus();  
}

function WinShowModalDialog( url, winName)
{
    val=window.showModalDialog( url , winName ,'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no'); 
    return;
}
function WinOpen( url, winName  )
{
   var  newWindow=window.open( url , winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
}

/* 鼠标On AND Out. */
function DGTBOn1(ctrl)
{
  ctrl.style.backgroundColor='Silver';
}
function DGTBOut1(ctrl)
{
   ctrl.style.backgroundColor='white';
}
function DGTBOutAlter1(ctrl)
{
   ctrl.style.backgroundColor='lightgoldenrodyellow';
}
function DGTBOutSelected1(ctrl)
{
   ctrl.style.backgroundColor='lightskyblue';
}
/* */ 
function DGTROn1(ctrl)
{
   ctrl.style.backgroundColor='LightSteelBlue';
}

function DGTROut1(ctrl)
{
   ctrl.style.backgroundColor='white';
}

function DGTROutAlter1(ctrl)
{
   ctrl.style.backgroundColor='Lavender';
}
function DGTROutSelected1(ctrl)
{
 // ctrl.style.backgroundColor='lightskyblue';
}
/* */

/* Style1  DGCell 鼠标 On  AND Out */
function DGCellOn1(ctrl)
{
  // ctrl.style.backgroundColor='Silver'
}
function DGCellAlterOut1(ctrl)
{
  // ctrl.style.backgroundColor='lightgoldenrodyellow'
}
function DGCellOut1(ctrl)
{
 // ctrl.style.backgroundColor='white'
}

/*  */
function TBOn1(ctrl)
{
  // ctrl.select();
   //ctrl.style.backgroundColor='LightBlue';
}

function TBOut1(ctrl)
{
  //ctrl.style.backgroundColor='white';
}

/* ESC Key Down  */
function Esc()
{
    if (event.keyCode == 27)     
        window.close();
    return true;
}

/* 用来验证 输入的是不是一个数字． onkeypress */
var appPath = "YSWF";

function VirtyMoney(number) {

    number = number.replace(/\,/g, "");
    if (number == "") return "";
    if (number < 0)
        return '-' + outputDollars(Math.floor(Math.abs(number) - 0) + '') + outputCents(Math.abs(number) - 0);
    else
        return outputDollars(Math.floor(number - 0) + '') + outputCents(number - 0);
} 


function VirtyInt(ctrl)
{
  if (event.keyCode == 190)
      return false; //如果是逗号。

  if (event.keyCode == 13)
        return true;
       
    if (event.keyCode == 46)
        return true;
        
    if (event.keyCode == 45)
        return true;

     if (event.keyCode == 229 || event.keyCode ==190 )
        return true;
        
        
  if (event.keyCode <= 40 && event.keyCode >= 34)
        return true;
 
    if (event.keyCode <= 105 && event.keyCode >= 96)
        return true;

    if (event.keyCode==8 || event.keyCode==190 )
       return true;
  
    if (event.keyCode < 48 || event.keyCode > 57)
        return false;
    else
        return true;
}

function VirtyNum(ctrl) {

    if (obj.value != "") {
        var value = obj.value;
        var pattern = /^\d*[.]{0,1}\d{0,3}$/; --匹配純數字或小數
        if (!pattern.exec(value)) {
            obj.value = value.substr(0, value.length - 1);
        }
    }
    return;
}

function VirtyDateTime(ctrl)
{
    if (event.keyCode == 58)
        return true;
        
    if (event.keyCode == 45)
        return true;
        
    if (event.keyCode == 13)
        return true;
        
    if (event.keyCode < 48 || event.keyCode > 57)
        return false;
    else
        return true;
}

/* 显示日期 */
function ShowDateTime( appPath , ctrl)
{ 
      url =appPath+'/Comm/Pub/CalendarHelp.htm';           
	 if ( event.button != 2) 
	   return; 	
	val=window.showModalDialog( url , '','dialogHeight: 335px; dialogWidth: 340px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no'); 
	if ( val==undefined)
	     return;
	 ctrl.value=val;
}

/* 默认植问题 */
function DefaultVal1( appPath , ctrl, className, attrKey, empId )
{ 
   if ( event.button != 2) 
	 return;    
    url =appPath+ '/Comm/DataHelp.htm?' +appPath+ '/Comm/HelperOfTB.aspx?EnsName='+className+'&AttrKey='+attrKey+'&empId='+empId ;
	str=ctrl.value;
    str= window.showModalDialog(url+'&Key='+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
	if ( str==undefined) 
	     return ;
	ctrl.value=str; 
}



/* 默认植问题　 */
function RefEns( appPath , ctrl, className,attrKey)
{ 
  if ( event.button != 2) 
	 return;
    url =appPath+ '/Comm/DataHelp.htm?' +appPath+ '/Comm/HelperOfTB.aspx?EnsName='+className+'&AttrKey='+attrKey+'&empId='+empId ;
	str=ctrl.value;
    str= window.showModalDialog(url+'&Key='+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
	if ( str==undefined) 
	     return ;
	ctrl.value=str; 
}

/* 字体大小 */
var FontSize='12px' ;
function DoZoom(ctl, size )
{
   fontSize=size;
   document.getElementById( ctl ).style.fontSize=fontSize;
   SetCookie("FontSize",size);
}


/* about cookice */
function GetCookieVal(offset)
{
  var endstr = document.cookie.indexOf (";", offset);
  if (endstr == -1)
      endstr = document.cookie.length;
   return unescape(document.cookie.substring(offset, endstr));
}
// 得到cooke .如果是Null , 返回的val
function GetCookie(name, isNullReVal) 
{
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var I = 0;
    while (I < clen)
    {
      var j = I + alen;
      if (document.cookie.substring(I, j) == arg)
          return GetCookieVal (j);
      I = document.cookie.indexOf(" ", I) + 1;
      if (I == 0)
          break;
    }
    return isNullReVal;
} 
// 设置cook
function SetCookie (name, value)
{
  var argv = SetCookie.arguments;
  var argc = SetCookie.arguments.length;
  var expires = (argc > 2) ?argv[2] : null;
  var path = (argc > 3) ? argv[3] : null;
  var domain = (argc > 4) ? argv[4] : null;
  var secure = (argc > 5) ? argv[5]:false;
  
  document.cookie = name + "=" + escape (value)+((expires == null)?"":("; expires=" + expires.toGMTString())) +
    ((path == null) ? "" : ("; path=" + path))+((domain == null) ? "" : ("; domain=" + domain))+((secure == true) ? "; secure" : "");
    
}

function HalperOfDDL (appPath, EnsName, refkeyvalue,  reftext, ddlID )
{
   var url=''; // appPath+"/Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&RefKey="+refkeyvalue+"&RefText="+reftext;
   if ( appPath=='/')
     url=  "DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&RefKey="+refkeyvalue+"&RefText="+reftext;
     else
     url= appPath+"/Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&RefKey="+refkeyvalue+"&RefText="+reftext;
   
   var str=window.showModalDialog( url  , '','dialogHeight: 500px; dialogWidth:800px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); 
   SetDDLVal(ddlID ,str );
}


function SetDDLVal( ddlID, val )
{

   if (val==undefined)
      return;
      
   var ddl  =   document.getElementById(  ddlID  ); 
   var  mylen  =  ddl.options.length-1  ;  
   while  ( mylen  >=  0 )
   {  
     if ( ddl.options[ mylen ].value== val )
     {
         ddl.options[mylen  ].selected=true;
     }
     mylen  --;  
   }
}

function onDDLSelectedMore( ddlID, MainEns, EnsName, refkeyvalue , reftext )
{
     var url=''; 
     url=  "DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&RefKey="+refkeyvalue+"&RefText="+reftext+"&MainEns="+MainEns+"&DDLID="+ddlID;
     
     
 //  var newWindow=window.open( url,'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
   
   //url=  "DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&DDLID="+ddlID+"&RefKey=No&RefText=Name";
   // alert(url); 
   //url= "DataHelp.htm?HelperOfDDLAdv.aspx?EnsName="+EnsName ;
   
   var str=window.showModalDialog( url  , '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); 
   ddlID = ddlID.replace('DDL_','');
   
   //alert(str);
   
   if (str!=null)
   {
        var hrf =  window.location.href;
        hrf=hrf.replace(ddlID,'s' );
        hrf=hrf+'&'+ddlID+'='+str;
        window.location.href=hrf;
   }
}
function onkeydown()
{
  if (window.event.srcElement.tagName=="TEXTAREA") 
     return false;
  if(event.keyCode==13)
     event.keyCode=9;
}
 
function VirtyMoney(number) 
{ 
  number=number.replace(/\,/g,""); 
  if (number=="") return ""; 
  if(number<0) 
  return '-'+outputDollars(Math.floor(Math.abs(number)-0) + '') + outputCents(Math.abs(number) - 0); 
  else 
  return outputDollars(Math.floor(number-0) + '') + outputCents(number - 0); 
} 
function outputDollars(number) 
{ 
  if (number.length<= 3) 
  return (number == '' ? '0' : number); 
  else 
  { 
    var mod = number.length%3; 
    var output = (mod == 0 ? '' : (number.substring(0,mod))); 
    for (i=0 ; i< Math.floor(number.length/3) ; i++) 
    { 
      if ((mod ==0) && (i ==0)) 
      output+= number.substring(mod+3*i,mod+3*i+3); 
      else 
      output+= ',' + number.substring(mod+3*i,mod+3*i+3); 
    } 
    return (output); 
  } 
} 
function outputCents(amount) 
{ 
  amount = Math.round( ( (amount) - Math.floor(amount) ) *100); 
  return (amount<10 ? '.0' + amount : '.' + amount); 
}
function TBHelp(ctrl, appPath, enName, attrKey) {


  var url = appPath + "/Comm/DataHelp.htm?" + appPath + "/Comm/HelperOfTB.aspx?EnsName=" + enName + "&AttrKey=" + attrKey;
  var  str= window.showModalDialog(url ,'sd', 'dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
  
  if ( str==undefined) 
      return ;
    document.getElementById(  ctrl  ).value=str;
}
