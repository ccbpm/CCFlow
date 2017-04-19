/*
 * click:点击推事件
 * view：跳转URL
 * scancode_push：扫码推事件
 * scancode_waitmsg：扫码推事件且弹出“消息接收中”提示框
 * pic_sysphoto：弹出系统拍照发图
 * pic_photo_or_album：弹出拍照或者相册发图
 * pic_weixin：弹出微信相册发图器
 * location_select：弹出地理位置选择器
 * media_id：下发消息（除文本消息）
 * view_limited：跳转图文消息URL
 */
var type_click="click";
var type_view="view";
$(function () {
	refashMenu();
	$("#menuForm input[name$='selectType']").bind("click", function () {
		var v=this.value;
		if(v==0){
			$("#selectMessage").removeClass('hide');
			$("#selectUrl").addClass('hide');
			$("#menuForm input[name$='type']").val(type_click);
		}else{
			$("#selectUrl").removeClass('hide');
			$("#selectMessage").addClass('hide');
			$("#menuForm input[name$='type']").val(type_view);
		}		
    });
	$('#itemformAdd').on('click', function(e){
		e.preventDefault();
		cleanItemForm();
		JY.Model.edit("itemDiv","新增图文",function(){
			 var html="";
			 var title=$("#itemDiv input[name$='title']").val();
			 var sort=$("#itemDiv input[name$='sort']").val();
			 var url=$("#itemDiv input[name$='url']").val();
			 var picUrl=$("#itemDiv input[name$='picUrl']").val();
			 var content=$("#itemDiv input[name$='content']").val();
			 if(JY.Validate.form("itemDiv")){				 
				 html+="<tr id='temtr"+itemNum+"' >";
				 html+="<td>"+title+"</td>";
				 html+="<td>"+content+"</td>";
				 html+="<td>"+url+"</td>";
				 if(JY.Object.notNull(picUrl)){
					 html+="<td class='center' ><img height='30' width='30' src='"+picUrl+"' ></td>";
				 }else{
					 html+="<td></td>";
				 }	
				 html+="<td>"+sort+"</td>";
				 html+="<td class='center'>";
				 html+="<div class='inline position-relative'>";
				 html+="<button class='btn btn-minier btn-primary dropdown-toggle' data-toggle='dropdown'><i class='icon-cog icon-only bigger-110'></i></button>";
				 html+="<ul class='dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close'>";	
				 html+="<li><a class='aBtnNoTD' onclick='editItem(&apos;"+itemNum+"&apos;)' title='修改' href='#'><i class='icon-edit color-blue bigger-120'></i></a></li>";
				 html+="<li><a class='aBtnNoTD' onclick='delItem(&apos;"+itemNum+"&apos;)' title='删除' href='#'><i class='icon-remove-sign color-red bigger-120'></i></a></li>";
				 html+="</ul></div>";	
				 html+="<input type='hidden' name='items'id='item"+itemNum+"' value='"+itemNum+"' itemTitle='"+title+"' itemUrl='"+url+"' itemSort='"+sort+"' itemPicUrl='"+picUrl+"' itemContent='"+content+"'  >";			
				 html+="</td>";
				 html+="</tr>";
				 itemNum++;
				 $("#itemsTable").append(html);
				 $(this).dialog("close");
			 }		
		});
	});
});
function delItem(id){
	JY.Model.confirm("确认要删除该图文吗?",function(){$("#temtr"+id).remove();});	
}
function editItem(id){
	cleanItemForm();
	var itemTitle=$("#item"+id).attr("itemTitle");
	var itemContent=$("#item"+id).attr("itemContent");
	var itemSort=$("#item"+id).attr("itemSort");
	var itemUrl=$("#item"+id).attr("itemUrl");
	var itemPicUrl=$("#item"+id).attr("itemPicUrl");	
	$("#itemDiv input[name$='title']").val(itemTitle);
	$("#itemDiv input[name$='content']").val(itemContent);
	$("#itemDiv input[name$='sort']").val(itemSort);
	$("#itemDiv input[name$='url']").val(itemUrl);
	$("#itemDiv input[name$='picUrl']").val(itemPicUrl);
	JY.Model.edit("itemDiv","修改图文",function(){
		 var title=$("#itemDiv input[name$='title']").val();
		 var content=$("#itemDiv input[name$='content']").val();
		 var sort=$("#itemDiv input[name$='sort']").val();
		 var url=$("#itemDiv input[name$='url']").val();
		 var picUrl=$("#itemDiv input[name$='picUrl']").val();
		 if(JY.Validate.form("itemDiv")){		
			 $("#item"+id).attr("itemTitle",title);
			 $("#item"+id).attr("itemContent",content);
			 $("#item"+id).attr("itemSort",sort);
			 $("#item"+id).attr("itemUrl",url);
			 $("#item"+id).attr("itemPicUrl",picUrl);
			 $("#temtr"+id).find("td").eq(0).html(title);
			 $("#temtr"+id).find("td").eq(1).html(content);
			 $("#temtr"+id).find("td").eq(2).html(url);
			 if(JY.Object.notNull(picUrl)){
				 $("#temtr"+id).find("td").eq(3).html("<img height='30' width='30' src='"+picUrl+"' ></td>");
			 }else{
				 $("#temtr"+id).find("td").eq(3).html("");
			 }			 
			 $("#temtr"+id).find("td").eq(4).html(sort);
			 $(this).dialog("close");
		 }
	});
}
function setItemArry(){
	var nums = $("#itemsTable input[name='items']").map(function(){return $(this).val();}).get().join(",");
	if(JY.Object.notNull(nums)){
		var numList=nums.split(",");
		for(var i=0;i<numList.length;i++){
			var num=numList[i];
			var itemTitle=$("#item"+num).attr("itemTitle");
			var itemContent=$("#item"+num).attr("itemContent");
			var itemUrl=$("#item"+num).attr("itemUrl");
			var itemSort=$("#item"+num).attr("itemSort");
			var itemPicUrl=$("#item"+num).attr("itemPicUrl");	
			var item={title:itemTitle,url:itemUrl,sort:itemSort,picUrl:itemPicUrl,content:itemContent};
			itemArry.push(item);
		}
	}	
}
function refashMenu(){
	JY.Model.loading();
	$("#menuList").empty();
	JY.Ajax.doRequest(null,jypath +'/backstage/weixin/menu/menuTree',null,function(data){
			var obj=data.obj,html="",list=obj.list;
			html=createMenu(html,list);
			$("#menuList").append(html);
			$("#menuList div ul li").bind('click',function(){  
				$("#menuList div ul li").removeClass('current');
				$(this).addClass("current");
			});	
		JY.Model.loadingClose();
	});
}
function createMenu(html,list){
	if(list!=null&&list.length>0){
		var mlength=list.length;
		var size= (mlength<2)?2:3;
		for(var i = 0;i<mlength;i++){
			 var l=list[i];
			 html+="<li class='jsMenu pre_menu_item grid_item jslevel1 ui-sortable ui-sortable-disabled size1of"+size+"'>";
			 html+="<a draggable='false' class='pre_menu_link' href='javascript:void(0);' onclick='findMenu(&apos;"+l.id+"&apos;)' >";
			 html+="<i class='icon_menu_dot js_icon_menu_dot dn'></i>";
			 html+="<i class='icon20_common sort_gray'></i>";
			 html+="<span class='js_l1Title'>"+l.name+"</span>";
			 html+="</a>";	
			 html=createSubMenu(html,l.id,l.nodes);
			 html+="</li>";
		}
		if(mlength<3){
			html+="<li class='js_addMenuBox pre_menu_item grid_item no_extra size1of"+size+"'>";
			html+="<a draggable='false' title='最多添加3个一级菜单' class='pre_menu_link js_addL1Btn' href='javascript:void(0);' onclick='newMenu()' >";
			html+="<i class='icon14_menu_add'></i>";
			html+="</a>";
			html+="</li>";
		}		
	}else{
		html+="<li class='js_addMenuBox pre_menu_item grid_item no_extra size1of1'>";
		html+="<a draggable='false' title='最多添加3个一级菜单' class='pre_menu_link js_addL1Btn' href='javascript:void(0);' onclick='newMenu()' >";
		html+="<i class='icon14_menu_add'></i>";
		html+="</a>";
		html+="</li>";
	}
	return html;
}
function createSubMenu(html,pid,nodes){
	html+="<div class='sub_pre_menu_box js_l2TitleBox'>";
	html+="<ul class='sub_pre_menu_list'>";
	if(nodes!=null&&nodes.length>0){			
		for(var i = 0;i<nodes.length;i++){
			var n=nodes[i];
			html+="<li class='jslevel2'>";
			html+="<a draggable='false' class='jsSubView' href='javascript:void(0);' onclick='findMenu(&apos;"+n.id+"&apos;)' >";
			html+="<span class='sub_pre_menu_inner js_sub_pre_menu_inner'>";
			html+="<i class='icon20_common sort_gray'></i>";
			html+="<span class='js_l2Title'>"+n.name+"</span>";
			html+="</span>";
			html+="</a>";
			html+="</li>";
		}			
		if(nodes.length<5){
			html+="<li class='js_addMenuBox'>";
			html+="<a draggable='false' title='最多添加5个子菜单' class='jsSubView js_addL2Btn' href='javascript:void(0);' onclick='newMenu(&apos;"+pid+"&apos;)'  >";
			html+="<span class='sub_pre_menu_inner js_sub_pre_menu_inner'>";
			html+="<i class='icon14_menu_add'></i>";
			html+="</span>";
			html+="</a>";
			html+="</li>";
		}
	}else{
		html+="<li class='js_addMenuBox'>";
		html+="<a draggable='false' title='最多添加5个子菜单' class='jsSubView js_addL2Btn' href='javascript:void(0);' onclick='newMenu(&apos;"+pid+"&apos;)' >";
		html+="<span class='sub_pre_menu_inner js_sub_pre_menu_inner'>";
		html+="<i class='icon14_menu_add'></i>";
		html+="</span>";
		html+="</a>";
		html+="</li>";
	}
	html+="</ul>";
	html+="<i class='arrow arrow_out'></i> <i class='arrow arrow_in'></i>";
	html+="</div>";	
	return html;
}
var saveBtn="no";
function findMenu(id){
	cleanForm();
	JY.Model.loading();
	JY.Ajax.doRequest(null,jypath +'/backstage/weixin/menu/findMenu',{id:id},function(data){
			setForm(data);
			$("#delMenuBtn").removeClass('hide');
		JY.Model.loadingClose();
	});
	saveBtn="edit";
}
function newMenu(pId){
	cleanForm();
	if(JY.Object.notNull(pId)){
		$("#menuForm input[name$='pId']").val(pId);
		$("#menuTile").html("新增二级菜单");
	}else{
		$("#menuList div ul li").removeClass('current');
		$("#menuTile").html("新增一级菜单");
	}
	saveBtn="add";
}
function setForm(data){
	var l=data.obj;
	$("#menuForm input[name$='id']").val(l.id);
	$("#menuForm input[name$='keyId']").val(l.keyId);
	var pId=l.pId;
	if("0"!=pId){
		$("#menuForm input[name$='pId']").val(pId);
		$("#menuTile").html("编辑二级菜单【"+l.name+"】");
	}else{
		$("#menuList div ul li").removeClass('current');
		$("#menuTile").html("编辑一级菜单【"+l.name+"】");
	}
	$("#menuForm input[name$='name']").val(l.name);
	$("#menuForm input[name$='sort']").val(l.sort);
	var type=l.type;
	if(type=="view"){
		$("#selectUrl").removeClass('hide');
		$("#selectMessage").addClass('hide');
		$("#menuForm input[name$='url']").val(l.url);
		$("#menuForm input[name$='type']").val(type_view);
		$("#menuForm input[name$='selectType'][value='1']").parent("label").trigger("click");
	}else{
		$("#selectMessage").removeClass('hide');
		$("#selectUrl").addClass('hide');
		$("#menuForm input[name$='type']").val(type_click);
		$("#menuForm input[name$='selectType'][value='0']").parent("label").trigger("click");
		var selectType=l.selectType;
		var items=l.items;		
		if("text"==selectType){
			removeTab();
			$("#tabText").addClass("active");
			$("#text").addClass("active");
			if(items!=null && items.length>0){
				$("#menuForm textarea[name$='text']").val(items[0].content);
			}
		}else if("imageText"==selectType){
			removeTab();
			$("#tabimageText").addClass("active");
			$("#imageText").addClass("active");		
			if(items!=null && items.length>0){
				var html="";
				for(var i=0;i<items.length;i++){
					var item=items[i];			 
					html+="<tr id='temtr"+itemNum+"' >";
					html+="<td>"+JY.Object.notEmpty(item.title)+"</td>";
					html+="<td>"+JY.Object.notEmpty(item.content)+"</td>";
					html+="<td>"+JY.Object.notEmpty(item.url)+"</td>";
					if(JY.Object.notNull(item.picUrl)){
						html+="<td class='center' ><img height='30' width='30' src='"+item.picUrl+"' ></td>";
					}else{
						html+="<td></td>";
					}	
					html+="<td>"+item.sort+"</td>";
					html+="<td class='center' >";						
					html+="<div class='inline position-relative'>";
					html+="<button class='btn btn-minier btn-primary dropdown-toggle' data-toggle='dropdown'><i class='icon-cog icon-only bigger-110'></i></button>";
					html+="<ul class='dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close'>";	
					html+="<li><a class='aBtnNoTD' onclick='editItem(&apos;"+itemNum+"&apos;)' title='修改' href='#'><i class='icon-edit color-blue bigger-120'></i></a></li>";
					html+="<li><a class='aBtnNoTD' onclick='delItem(&apos;"+itemNum+"&apos;)' title='删除' href='#'><i class='icon-remove-sign color-red bigger-120'></i></a></li>";	
					html+="</ul></div>";		
					html+="<input type='hidden' name='items'id='item"+itemNum+"' value='"+itemNum+"' itemTitle='"+JY.Object.notEmpty(item.title)+"' itemUrl='"+JY.Object.notEmpty(item.url)+"' itemSort='"+item.sort+"' itemPicUrl='"+JY.Object.notEmpty(item.picUrl)+"' itemContent='"+JY.Object.notEmpty(item.content)+"'  >";			
					html+="</td>";
					html+="</tr>";
					itemNum++;
				}
				$("#itemsTable").append(html);
			}	
		}else if("image"==selectType){
			removeTab();
			$("#tabimage").addClass("active");
			$("#image").addClass("active");
			if(items!=null && items.length>0){
				$("#menuForm input[name$='picUrl']").val(items[0].picUrl);
			}
			
		}
	}
}
var itemArry =new Array();
var itemNum=1;
function cleanForm(){
	$("#menuTile").html("选择菜单位置");
	$("#delMenuBtn").addClass('hide');
	JY.Tags.cleanForm("menuForm");
	$("#menuForm input[name$='id']").val("");
	$("#menuForm input[name$='pId']").val("0");
	$("#menuForm input[name$='sort']").val("1");
	$("#menuForm input[name$='type']").val(type_click);
	$("#menuForm input[name$='keyId']").val("");
	$("#selectMessage").removeClass('hide');
	$("#selectUrl").addClass('hide');
	$("#menuForm input[name$='selectType'][value='0']").parent("label").trigger("click");
	itemArry =new Array();
	itemNum=1;
	$("#itemDiv input[name$='sort']").val('1');
	$("#itemsTable tbody").empty();
	removeTab();
	$("#tabText").addClass("active");
	$("#text").addClass("active");
}

function removeTab(){
	$("#tabText").removeClass("active");
	$("#text").removeClass("active");
	$("#tabimageText").removeClass("active");
	$("#imageText").removeClass("active");			
	$("#tabimage").removeClass("active");
	$("#image").removeClass("active");
}

function saveMenu(){
	if(saveBtn=="add"){
		addMenu();
	}else if(saveBtn=="edit"){		
		editMenu();
	}else{
		JY.Model.info("请选择菜单位置");
	}
}

function addMenu(){
	itemArry =new Array();
	if(JY.Validate.form("menuForm")){
		 var pId=$("#menuForm input[name$='pId']").val();
		 var name=$("#menuForm input[name$='name']").val();
		 var type=$("#menuForm input[name$='type']").val();
		 var sort=$("#menuForm input[name$='sort']").val();
		 var params ={pId:pId,name:name,type:type,sort:sort,url:url};
		 if(type_view==type){
			 var url=$("#menuForm input[name$='url']").val();
			 params.url=url;
		 }else if(type_click==type){
			 if($("#text").hasClass("active")){
				 	var text=$("#menuForm textarea[name$='text']").val();
					var item={content:text};
					itemArry.push(item);
					params.items=itemArry;
					params.selectType="text";
			 }else if($("#imageText").hasClass("active")){
				 	setItemArry();
				 	params.items=itemArry;
				 	params.selectType="imageText";
			 }else if($("#image").hasClass("active")){
					var picUrl=$("#menuForm input[name$='picUrl']").val();
					var item={picUrl:picUrl};
				    itemArry.push(item);
					params.items=itemArry;
				    params.selectType="image";
			 } 
		 }
		 $.ajax({type:'POST',url:jypath+'/backstage/weixin/menu/addMenu',data:JSON.stringify(params),dataType:'json',contentType:"application/json",success:function(data,textStatus){  			        	 
        		JY.Model.info(data.resMsg,function(){	
        			cleanForm();
        			refashMenu();	
        			saveBtn="no";
        		});	
         }
     });  
	}
}
function editMenu(){
	itemArry =new Array();
	if(JY.Validate.form("menuForm")){
		 var id= $("#menuForm input[name$='id']").val();
		 var pId=$("#menuForm input[name$='pId']").val();
		 var keyId=$("#menuForm input[name$='keyId']").val();	 
		 var name=$("#menuForm input[name$='name']").val();
		 var type=$("#menuForm input[name$='type']").val();
		 var sort=$("#menuForm input[name$='sort']").val();
		 var params ={id:id,pId:pId,keyId:keyId,name:name,type:type,sort:sort,url:url};
		 if(type_view==type){
			 var url=$("#menuForm input[name$='url']").val();
			 params.url=url;
		 }else if(type_click==type){
			 if($("#text").hasClass("active")){
				 	var text=$("#menuForm textarea[name$='text']").val();
					var item={content:text};
					itemArry.push(item);
					params.items=itemArry;
					params.selectType="text";
			 }else if($("#imageText").hasClass("active")){
				 	setItemArry();
				 	params.items=itemArry;
				 	params.selectType="imageText";
			 }else if($("#image").hasClass("active")){
					var picUrl=$("#menuForm input[name$='picUrl']").val();
					var item={picUrl:picUrl};
				    itemArry.push(item);
					params.items=itemArry;
				    params.selectType="image";
			 } 
		 }
		 $.ajax({type:'POST',url:jypath+'/backstage/weixin/menu/updateMenu',data:JSON.stringify(params),dataType:'json',contentType:"application/json",success:function(data,textStatus){  			        	 
        		JY.Model.info(data.resMsg,function(){	
        			cleanForm();
        			refashMenu();	
        			saveBtn="no";
        		}); 	
         }
     });  
	}
}
function delMenu(){
	JY.Model.confirm("确认要删除菜单吗?",function(){	
		var id=$("#menuForm input[name$='id']").val();
		JY.Ajax.doRequest(null,jypath +'/backstage/weixin/menu/delMenu',{id:id},function(data){
				JY.Model.info(data.resMsg,function(){
					cleanForm();
					refashMenu();
					saveBtn="no";
				});
		});
	});
}
function syncMenu(){
	JY.Model.confirm("确认要发布菜单吗?",function(){	
		JY.Ajax.doRequest(null,jypath +'/backstage/weixin/menu/syncMenu',"",function(data){
			JY.Model.info(data.resMsg);
		});
	});
}
function cleanItemForm(){
	JY.Tags.cleanForm("itemDiv");
	$("#itemDiv input[name$='sort']").val('1');
}

