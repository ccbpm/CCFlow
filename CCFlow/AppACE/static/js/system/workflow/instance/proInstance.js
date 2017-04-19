$(function () {
	getbaseList();
	//增加回车事件
	$("#baseForm").keydown(function(e){
		 keycode = e.which || e.keyCode;
		 if (keycode==13) {
			 search();
		 } 
	});
});
function search(){
	$("#searchBtn").trigger("click");
}
function getbaseList(init){
	if(init==1)$("#baseForm .pageNum").val(1);	
	JY.Model.loading();
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/workflow/instance/pro/findByPage',null,function(data){
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
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.id)+"</td>";
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.processInstanceId)+"</td>";
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.processDefinitionId)+"</td>";
            		 var task=l.task;
            		 if(JY.Object.notNull(task)){
            			 html+="<td class='center'><a href='javascritp:void(0);' onclick='currentNode(&apos;"+l.processDefinitionId+"&apos;,&apos;"+l.processInstanceId+"&apos;);return false' >"+JY.Object.notEmpty(task.name)+"</a></td>";
            		 }else{
            			 html+="<td class='center'></td>";
            		 }  	
            		 html+=JY.Tags.setFunction(l.id,permitBtn);
            		 html+="</tr>";		 
            	 } 
        		 $("#baseTable tbody").append(html);
        		 JY.Page.setPage("baseForm","pageing",pageSize,pageNum,totalRecord,"getbaseList");
        	 }else{
        		html+="<tr><td colspan='8' class='center'>没有相关数据</td></tr>";
        		$("#baseTable tbody").append(html);
        		$("#pageing ul").empty();//清空分页
        	 }		 
    	 JY.Model.loadingClose();
	 });
}

function currentNode(pdId,pIId){
	var width=document.documentElement.clientWidth * 0.85+"px";
	var height=document.documentElement.clientHeight * 0.85+"px";
	layer.open({
	    type: 2,
	    title: '当前节点',
	    shadeClose: true,
	    maxmin: true,
	    area: [width, height],
	    content: jypath+"/act-process-editor/diagram-viewer/index.html?processDefinitionId="+pdId+"&processInstanceId="+pIId //iframe的url
	});
}
