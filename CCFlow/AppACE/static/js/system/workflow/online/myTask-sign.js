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
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/workflow/online/myTask/findSignByPage',null,function(data){
		 $("#baseTable tbody").empty();
	 	 var obj=data.obj;
    	 var list=obj.list;
    	 var results=list.results;
    	 //var permitBtn=obj.permitBtn;
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
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.taskDefinitionKey)+"</td>";
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.name)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.processDefinitionId)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.processInstanceId)+"</td>";
            		 /*html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.priority)+"</td>";*/
            		 html+="<td class='center hidden-480'>"+JY.Date.Default(l.createTime)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Date.Default(l.dueDate)+"</td>";
            		 html+="<td class='left hidden-480'>"+JY.Object.notEmpty(l.description)+"</td>";
            		 html+="<td class='left hidden-480'>"+JY.Object.notEmpty(l.owner)+"</td>";     		 
            		 html+="<td class='left'><button class='btn btn-xs btn-info'  onclick='claimTask(&apos;"+l.id+"&apos;)' ><i class='icon-pencil align-top bigger-125'></i>签收</button></td>";
            		 html+="</tr>";		 
            	 } 
        		 $("#baseTable tbody").append(html);
        		 JY.Page.setPage("baseForm","pageing",pageSize,pageNum,totalRecord,"getbaseList");
        	 }else{
        		html+="<tr><td colspan='12' class='center'>没有相关数据</td></tr>";
        		$("#baseTable tbody").append(html);
        		$("#pageing ul").empty();//清空分页
        	 }	 
    	 JY.Model.loadingClose();
	 });
}
function claimTask(taskId){
	JY.Model.confirm("确认签收吗？",function(){	
		JY.Ajax.doRequest("",jypath +'/backstage/workflow/online/myTask/claim/'+taskId,null,function(data){
			JY.Model.info(data.resMsg,function(){search();});  		
		});
	});
}