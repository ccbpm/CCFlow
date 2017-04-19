$(function () {
	JY.Dict.setSelect("selectisValid","isValid",2,'全部');
	getbaseList();
	//增加回车事件
	$("#baseForm").keydown(function(e){
		 keycode = e.which || e.keyCode;
		 if (keycode==13) {
			 search();
		 }
	});
	//新加
	$('#addBtn').on('click', function(e) {
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();		
		cleanForm();
		JY.Model.edit("auDiv","新增字典",function(){
			 if(JY.Validate.form("auForm")){
				 var that =$(this);
				 var isValid=$("#auForm input[name$='isValid']").val();
				 var dataKey=$("#auForm input[name$='dataKey']").val();
				 var name=$("#auForm input[name$='name']").val();
				 var description=$("#auForm textarea[name$='description']").val();
				 setItemArry();
				 var params ={isValid:isValid,dataKey:dataKey,name:name,description:description,items:itemArry};								 	 
				 $.ajax({type:'POST',url:jypath+'/backstage/dataDict/add',data:JSON.stringify(params),dataType:'json',contentType:"application/json",success:function(data,textStatus){  			        	 
			        	 if(data.res==1){
			        		 that.dialog("close");      
			        		JY.Model.info(data.resMsg,function(){search();});
			        	 }else{
			        		 JY.Model.error(data.resMsg);
			        	 }     	
			         }
			     });  
			 }		
		});
	});
	
	//新加字典值
	$('#itemformAdd').on('click', function(e){
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();
		cleanItemForm();
		JY.Model.edit("itemDiv","新增字典字段",function(){
			 var html="";
			 var name=$("#itemDiv input[name$='name']").val();
			 var value=$("#itemDiv input[name$='value']").val();
			 var sort=$("#itemDiv input[name$='sort']").val();
			 if(JY.Validate.form("itemDiv")){				 
				 html+="<tr id='temtr"+itemNum+"' >";
				 html+="<td>"+name+"</td>";
				 html+="<td>"+value+"</td>";
				 html+="<td>"+sort+"</td>";
				 html+="<td>";
				 html+="<div class='inline position-relative'>";
				 html+="<button class='btn btn-minier btn-primary dropdown-toggle' data-toggle='dropdown'><i class='icon-cog icon-only bigger-110'></i></button>";
				 html+="<ul class='dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close'>";	
				 html+="<li><a class='aBtnNoTD' onclick='editItem(&apos;"+itemNum+"&apos;)' title='修改' href='#'><i class='icon-edit color-blue bigger-120'></i></a></li>";
				 html+="<li><a class='aBtnNoTD' onclick='delItem(&apos;"+itemNum+"&apos;)' title='删除' href='#'><i class='icon-remove-sign color-red bigger-120'></i></a></li>";
				 html+="</ul></div>";	
				 html+="<input type='hidden' name='items'id='item"+itemNum+"' value='"+itemNum+"' itemName='"+name+"' itemValue='"+value+"' itemSort='"+sort+"'  >";			
				 html+="</td>";
				 html+="</tr>";
				 itemNum++;
				 $("#itemsTable").append(html);
				 $(this).dialog("close");
			 }		
		});
	});
	//批量删除
	$('#delBatchBtn').on('click', function(e) {
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();
		var chks =[];    
		$('#baseTable input[name="ids"]:checked').each(function(){chks.push($(this).val());});     
		if(chks.length==0) JY.Model.info("您没有选择任何内容!"); 
		JY.Model.confirm("确认要删除选中的数据吗?",function(){	
			JY.Ajax.doRequest(null,jypath +'/backstage/dataDict/delBatch',{chks:chks.toString()},function(data){
				JY.Model.info(data.resMsg,function(){search();});
			});
		});		
	});
	//增加显示
	$("#itemformAdd").tooltip({hide:{effect:"slideDown",delay:250}});
	//排序显示
	$("#itemformSort").tooltip({hide:{effect:"slideDown",delay:250}});
});
//字典集合
var itemArry =new Array();
var itemNum=1;
function cleanForm(){
	itemArry =new Array();
	itemNum=1;
	JY.Tags.cleanForm("auForm");
	JY.Tags.isValid("auForm","1");
	JY.Tags.cleanForm("itemDiv");
	$("#itemDiv input[name$='sort']").val('1');
	$("#itemsTable tbody").empty();
	$("#auForm input[name$='dataKey']").prop("disabled",false); 
}
function cleanItemForm(){
	JY.Tags.cleanForm("itemDiv");
	$("#itemDiv input[name$='sort']").val('1');
}
function search(){
	$("#searchBtn").trigger("click");
}
function setItemArry(){
	var nums = $("#itemsTable input[name='items']").map(function(){return $(this).val();}).get().join(",");
	if(JY.Object.notNull(nums)){
		var numList=nums.split(",");
		for(var i=0;i<numList.length;i++){
			var num=numList[i];
			var itemName=$("#item"+num).attr("itemName");
			var itemValue=$("#item"+num).attr("itemValue");
			var itemSort=$("#item"+num).attr("itemSort");
			var item={name:itemName,value:itemValue,sort:itemSort};
			itemArry.push(item);
		}
	}	
}
function delItem(id){
	JY.Model.confirm("确认要删除字段吗?",function(){$("#temtr"+id).remove();});	
}
function editItem(id){
	cleanItemForm();
	var itemName=$("#item"+id).attr("itemName");
	var itemValue=$("#item"+id).attr("itemValue");
	var itemSort=$("#item"+id).attr("itemSort");
	$("#itemDiv input[name$='name']").val(itemName);
	$("#itemDiv input[name$='value']").val(itemValue);
	$("#itemDiv input[name$='sort']").val(itemSort);
	JY.Model.edit("itemDiv","修改字典字段",function(){
		 var name=$("#itemDiv input[name$='name']").val();
		 var value=$("#itemDiv input[name$='value']").val();
		 var sort=$("#itemDiv input[name$='sort']").val();
		 if(JY.Validate.form("itemDiv")){		
			 $("#item"+id).attr("itemName",name);
			 $("#item"+id).attr("itemValue",value);
			 $("#item"+id).attr("itemSort",sort);
			 $("#temtr"+id).find("td").eq(0).html(name);
			 $("#temtr"+id).find("td").eq(1).html(value);
			 $("#temtr"+id).find("td").eq(2).html(sort);
			 $(this).dialog("close");
		 }
	});
}
function getbaseList(init){
	if(init==1) $("#baseForm .pageNum").val(1);	
	JY.Model.loading();
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/dataDict/findByPage',null,function(data){
		$("#baseTable tbody").empty();
		 var obj=data.obj;
    	 var list=obj.list;
    	 var results=list.results;
    	 var permitBtn=obj.permitBtn;
     	 var pageNum=list.pageNum,pageSize=list.pageSize,totalRecord=list.totalRecord;
		 var html="";
		 if(results!=null&&results.length>0){
	       	var leng=(pageNum-1)*pageSize;//计算序号
	       	for(var i = 0;i<results.length;i++){
	           	var l=results[i];       
	           	html+="<tr>";
	           	html+="<td class='center'><label> <input type='checkbox' name='ids' value='"+l.id+"' class='ace' /> <span class='lbl'></span></label></td>";
	           	html+="<td class='center hidden-480'>"+(i+leng+1)+"</td>";
	           	html+="<td class='center'>"+l.dataKey+"</td>";	  
	           	html+="<td class='center'>"+l.name+"</td>";
	           	html+="<td class='center hidden-480'>"+l.description+"</td>";
	           	if(l.isValid==1) html+="<td class='center hidden-480'><span class='label label-sm label-success'>有效</span></td>";
	           	else             html+="<td class='center hidden-480'><span class='label label-sm arrowed-in'>无效</span></td>";
	           	html+="<td class='center hidden-480'>"+JY.Date.Default(l.createTime)+"</td>";
	           	html+="<td class='center hidden-480'>"+JY.Date.Default(l.updateTime)+"</td>";
	           	html+=JY.Tags.setFunction(l.id,permitBtn);
	           	html+="</tr>";		 
	          } 
	       	$("#baseTable tbody").append(html);
	        JY.Page.setPage("baseForm","pageing",pageSize,pageNum,totalRecord,"getbaseList");
	    }else{
	       html+="<tr><td colspan='9' class='center'>没有相关数据</td></tr>";
	       $("#baseTable tbody").append(html);
	       $("#pageing ul").empty();//清空分页
	     }		
	    JY.Model.loadingClose();
	});
}
function del(id){
	JY.Model.confirm("确认删除吗？",function(){	
		JY.Ajax.doRequest(null,jypath +'/backstage/dataDict/del',{id:id},function(data){
		 	 JY.Model.info(data.resMsg,function(){search();});
		});
	});
}
function edit(id){
	cleanForm();
	JY.Ajax.doRequest(null,jypath +'/backstage/dataDict/find',{id:id},function(data){
		setForm(data);
		JY.Model.edit("auDiv","修改字典",function(){
			if(JY.Validate.form("auForm")){
				var that =$(this);
				var id=$("#auForm input[name$='id']").val();
				var isValid=$("#auForm input[name$='isValid']").val();
				var dataKey=$("#auForm input[name$='dataKey']").val();
				var name=$("#auForm input[name$='name']").val();
				var description=$("#auForm textarea[name$='description']").val();
				setItemArry();
				var params ={id:id,isValid:isValid,dataKey:dataKey,name:name,description:description,items:itemArry};	
				$.ajax({type:'POST',url:jypath+'/backstage/dataDict/update',data:JSON.stringify(params),dataType:'json',contentType:"application/json",success:function(data,textStatus){  			        	 
					   if(data.res==1){
					      that.dialog("close");
					      JY.Model.info(data.resMsg,function(){search();});
					   }else{
					      JY.Model.error(data.resMsg);
					   } 				        	 
					  }});
			}
	});	
	});
}
function check(id){
	cleanForm();
	JY.Ajax.doRequest(null,jypath +'/backstage/dataDict/find',{id:id},function(data){
		setForm(data);
		JY.Model.check("auDiv","查看字典"); 	
	});
}
function setForm(data){
	var l=data.obj;
	$("#auForm input[name$='id']").val(l.id);
	$("#auForm input[name$='dataKey']").val(JY.Object.notEmpty(l.dataKey));
	$("#auForm input[name$='dataKey']").prop("disabled",true); 
	$("#auForm input[name$='name']").val(JY.Object.notEmpty(l.name));
	JY.Tags.isValid("auForm",(JY.Object.notNull(l.isValid)?l.isValid:"0"));
	$("#auForm textarea[name$='description']").val(JY.Object.notEmpty(l.description));
	var items=l.items;
	if(items!=null){
		var html="";
		for(var i=0;i<items.length;i++){
			var item=items[i];
			html+="<tr id='temtr"+itemNum+"' >";
			html+="<td>"+item.name+"</td>";
			html+="<td>"+item.value+"</td>";
			html+="<td>"+item.sort+"</td>";
			html+="<td>";			
			html+="<div class='inline position-relative'>";
			html+="<button class='btn btn-minier btn-primary dropdown-toggle' data-toggle='dropdown'><i class='icon-cog icon-only bigger-110'></i></button>";
			html+="<ul class='dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close'>";	
			html+="<li><a class='aBtnNoTD' onclick='editItem(&apos;"+itemNum+"&apos;)' title='修改' href='#'><i class='icon-edit color-blue bigger-120'></i></a></li>";
			html+="<li><a class='aBtnNoTD' onclick='delItem(&apos;"+itemNum+"&apos;)' title='删除' href='#'><i class='icon-remove-sign color-red bigger-120'></i></a></li>";	
			html+="</ul></div>";		
			html+="<input type='hidden' name='items'id='item"+itemNum+"' value='"+itemNum+"' itemName='"+item.name+"' itemValue='"+item.value+"' itemSort='"+item.sort+"'  >";			
			html+="</td>";
			html+="</tr>";
			itemNum++;
		}
		$("#itemsTable").append(html);
	}	
}
