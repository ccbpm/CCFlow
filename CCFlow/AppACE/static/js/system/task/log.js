$(function () {
	JY.Dict.setSelect("selectlogType","logType",2,'全部');
	$("input[name='beginTime']").datetimepicker({
		format:'yyyy-mm-dd',language:'zh-CN',weekStart:1,todayBtn:1,autoclose: 1,todayHighlight: 1,startView: 2,minView:2,
      }).on('changeDate', function(ev){
    	  	var beginTime=$("input[name='beginTime']").val();
    	  	$("input[name='endTime']").datetimepicker('setStartDate',beginTime);
    	 });
	$("input[name='endTime']").datetimepicker({
		format: 'yyyy-mm-dd',language:'zh-CN',weekStart: 1,todayBtn:1,autoclose:1,todayHighlight:1,startView:2,minView:2,
	}).on('changeDate', function(ev){
	  	var endTime=$("input[name='endTime']").val();
	  	$("input[name='beginTime']").datetimepicker('setEndDate',endTime);
	 });
	getbaseList();
	//增加回车事件
	$("#baseForm").keydown(function(e){
		 keycode = e.which || e.keyCode;
		 if(keycode==13){
			 search();
		 }
	});
	//批量删除
	$('#delBatchBtn').on('click', function(e) {
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();
		var chks =[];    
		$('#baseTable input[name="ids"]:checked').each(function(){    
			chks.push($(this).val());    
		});     
		if(chks.length==0) {
			 JY.Model.info("您没有选择任何内容!"); 
		}else{
			JY.Model.confirm("确认要删除选中的数据吗?",function(){	
				JY.Ajax.doRequest(null,jypath +'/backstage/task/taskLog/delBatch',{chks:chks.toString()},function(data){
					JY.Model.info(data.resMsg,function(){search();});
				});
			});		
		}		
	});
	$('#exportXls').on('click', function(e) {
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();
		JY.Model.confirm("确认要导出数据吗?",function(){	
			window.location.href=jypath+"/backstage/task/taskLog/exportXls";
		});	
	});
});
function search(){
	$("#searchBtn").trigger("click");
}

function getbaseList(init){
	if(init==1) $("#baseForm .pageNum").val(1);	
	JY.Model.loading();
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/task/taskLog/findByPage',null,function(data){
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
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.name)+"</td>";
            		 html+="<td class='center hidden-480' >"+JY.Object.notEmpty(l.className)+"</td>";
            		 if(l.type==1) html+="<td class='center'><span class='label label-sm label-success'>正常</span></td>";
            		 else             html+="<td class='center'><span class='label label-sm label-danger arrowed-in'>异常</span></td>"; 	
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.description)+"</td>";
            		 html+="<td class='center '>"+JY.Date.Default(l.createTime)+"</td>";
            		 html+="</tr>";		 
            	 } 
        		 $("#baseTable tbody").append(html);
        		 JY.Page.setPage("baseForm","pageing",pageSize,pageNum,totalRecord,"getbaseList");
        	 }else{
        		html+="<tr><td colspan='7' class='center'>没有相关数据</td></tr>";
        		$("#baseTable tbody").append(html);
        		$("#pageing ul").empty();//清空分页
        	 }	
        	 JY.Model.loadingClose();
	});
}