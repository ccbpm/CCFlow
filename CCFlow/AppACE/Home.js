$(function () {
	//加载皮肤设置
	loadSkin($("#userSkin").val());
	//加载菜单

	//getMenu();

	//TabControlAppend('shouye', '首页', '');
	//测试
	/*for(var i=1;i<18;i++){
		TabControlAppend('shouye'+i,'首页'+i, '');
	}*/
	//皮肤设置框-主题事件
	$('#skinSetting input[name=skin]').on('click', function(){
		var skin=$(this).val();
		$('body').removeClass().addClass(skin);
		loadSkin(skin);
	});	
	//加载个性化设置
	loadPro();
	//切换窄屏 
	$('#jy-settings-add-container').on('click', function(){
		var checked=this.checked;
		if(checked)$("#main-container").addClass("container");
		else       $("#main-container").removeClass("container");
	});
	//缩放菜单
	$('#jy-settings-sfMenu').on('click', function(){
		var checked=this.checked;
		if(checked) $("#sidebar").addClass("menu-min");
		else        $("#sidebar").removeClass("menu-min");
	});	
	//切换右菜单
	$('#jy-settings-rtl').on('click', function(){
		var checked=this.checked;
		if(checked) $("#indexBody").addClass("rtl");
		else        $("#indexBody").removeClass("rtl");
	});	
	//设置头像
	$('#headpicRecommend li').click(function(){
		$('#headpicRecommend li').removeClass("visiHot");
		$(this).addClass("visiHot");
		$("#headpicPreviewImg").attr('src',$(this).find('img').attr('src')); 
	});
});
function loadSkin(skin){
	$('body').removeClass().addClass(skin);
	var skinColor="";
	if(skin=="skin-0")skinColor="blue";
	else if(skin=="skin-1")skinColor="dark";
	else if(skin=="skin-2")skinColor="pink";
	else if(skin=="skin-3")skinColor="light-grey";		
	$('#skinSettingIcon1').removeClass().addClass(skinColor+" icon-github-alt bigger-110");
	$('#skinSettingIcon2').removeClass().addClass(skinColor+" icon-magnet bigger-110");
}
function loadPro(){
	var settings=$.cookie('JY.settings');
	if(typeof(settings) != "undefined"){
		var sjson = JSON.parse(settings);	
		//切换窄屏 
		if(typeof(sjson.narWinMenu) != "undefined" &&sjson.narWinMenu=="1"){
			$("#main-container").addClass("container");
			$("#jy-settings-add-container").prop("checked",true);			
		}else{
			$("#main-container").removeClass("container");	
			$("#jy-settings-add-container").prop("checked",false);
		}
		//缩放菜单
		if(typeof(sjson.sfMenu) != "undefined" &&sjson.sfMenu=="1"){
			$("#sidebar").addClass("menu-min");
			$("#jy-settings-sfMenu").prop("checked",true);		
		}else{
			$("#sidebar").removeClass("menu-min");	
			$("#jy-settings-sfMenu").prop("checked",false);		
		}
		//切换右菜单
		if(typeof(sjson.posMenu) != "undefined" &&sjson.posMenu=="1"){
			$("#indexBody").addClass("rtl");
			$("#jy-settings-rtl").prop("checked",true);	
		}else{
			$("#indexBody").removeClass("rtl");	
			$("#jy-settings-rtl").prop("checked",false);	
		}
	}else{
		var sjson={};
		$.cookie('JY.settings', JSON.stringify(sjson));
	}
}
function perSetting(){
	$("#perSetting").removeClass('hide').dialog({
		resizable:false,dialogClass:"title-no-close",modal:true,//设置为true，该dialog将会有遮罩层
		title: "<div class='widget-header'><h4 class='font-bold'>系统设置</h4></div>",title_html: true,
		show:{effect:"fade"},
		buttons: [
		  {  
			html: "<i class='icon-ok bigger-110'></i>&nbsp;确认","class" : "btn btn-primary btn-xs",
			click: function() {	
				var that =$(this);
				 JY.Ajax.doRequest("perSettingFrom",jypath +'/backstage/account/setSetting',null,function(data){
		        	$("#userSkin").val($("#perSettingFrom input:radio[name=skin]:checked").val());
		        	savePro();
		        	that.dialog("close");
		        	JY.Model.info(data.resMsg); 		 
				 }); 	
			}
		  },
		  {
			html: "<i class='icon-remove bigger-110'></i>&nbsp;取消","class" : "btn btn-xs",
			click: function() {		
				//取消恢复皮肤
				var skin=$("#userSkin").val();
				loadSkin(skin);	
				$("#skinSetting input:radio[name=skin][value='"+skin+"']").prop("checked",true);
				//取消恢复个性化
				loadPro();
				$(this).dialog("close");
			}
		  }
		]
	}); 
}
function savePro(){
	var checked=false;
	var sjson = JSON.parse($.cookie('JY.settings'));
	//切换窄屏 
	checked=$("#perSettingFrom input:checkbox[name=narWinMenu]:checked").val();
	if(checked)sjson.narWinMenu="1";
	else       sjson.narWinMenu="0";
	//缩放菜单
	checked=$("#perSettingFrom input:checkbox[name=sfMenu]:checked").val();
	if(checked)sjson.sfMenu="1";
	else       sjson.sfMenu="0";
	//切换右菜单
	checked=$("#perSettingFrom input:checkbox[name=posMenu]:checked").val();
	if(checked)sjson.posMenu="1";
	else       sjson.posMenu="0";
	//保存至cookie
	$.cookie('JY.settings', JSON.stringify(sjson));
}


function openUrl(title, url) {
    openMenu('1', 'menu10', 'menu13', title, url);
    return;
}

function perData() {
    openMenu('1', 'menu10', 'menu13', '设置', '../WF/Tools.aspx');
}

//打开菜单
function openMenu(type, id, parentId, menuName, resUrl) {
    if ('1' != type || "noset" == resUrl) return;
    else if ('/' == resUrl.substring(0, 1)) TabControlAppend(id, menuName, jypath + resUrl + "?menu=" + id);
    else TabControlAppend(id, menuName, resUrl);
}

function getMenu(layer, ref) {

    alert('该功能为二次开发提供的装饰功能.');

    //alert('');
	/*$("#menu_li_id").empty();
	JY.Ajax.doRequest(null,jypath +'/backstage/menu/getMenu',{layer:layer,ref:ref},function(data){
    	$("#menu_li_id").html(data.obj); 
	});*/
}
function logout(){
	JY.Model.confirm("确认要退出吗？",function(){window.location.href="Login.htm?DoType=Logout";});	
}

