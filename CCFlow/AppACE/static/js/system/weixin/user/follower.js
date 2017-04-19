$(function () {
	JY.Dict.setSelect("selectSubscribe","wxSubscribe",2,'全部');
	getbaseList();
	//增加回车事件
	$("#baseForm").keydown(function(e){
		 keycode = e.which || e.keyCode;
		 if(keycode==13){
			 search();
		 }
	});
	//新加
	$('#syncFollowerBtn').on('click', function(e) {
		//通知浏览器不要执行与事件关联的默认动作		
		e.preventDefault();
		JY.Model.confirm("确认要同步关注者数据吗?",function(){	
			JY.Model.loading();
			JY.Ajax.doRequest(null,jypath +'/backstage/weixin/follower/syncFollower',"",function(data){
				JY.Model.loadingClose();
				JY.Model.info(data.resMsg,function(){search();});
			});
		});
		
	});
});
function search(){
	$("#searchBtn").trigger("click");
}

function getbaseList(init){
	if(init==1) $("#baseForm .pageNum").val(1);	
	JY.Model.loading();
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/weixin/follower/findByPage',null,function(data){
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
            		 html+="<td class='center'><label> <input type='checkbox' name='ids' value='"+l.openid+"' class='ace' /> <span class='lbl'></span></label></td>";
            		 html+="<td class='center hidden-480'>"+(i+leng+1)+"</td>";          		 
            		 html+="<td class='center'><img width='30' height='30' src='"+JY.Object.notEmpty(l.headimgurl)+"' ></td>";
            		 html+="<td class='center'>"+JY.Object.notEmpty(l.nickname)+"</td>";
            		 var sex="未知";if(l.sex==1)sex="男";else if(l.sex==2)sex="女";
            		 html+="<td class='center hidden-480' >"+sex+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.country)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.province)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.city)+"</td>";   
            		 if(l.subscribe==1) html+="<td class='center hidden-480'><span class='label label-sm label-success'>订阅</span></td>";
            		 else if(l.subscribe==0)html+="<td class='center hidden-480'><span class='label label-sm arrowed-in'>退订</span></td>";
            		 else             html+="<td class='center hidden-480'><span class='label label-danger label-sm arrowed-in'>未知</span></td>";
            		 html+="<td class='center hidden-480'>"+JY.Date.Default(l.subscribeTime)+"</td>";
            		 html+="<td class='center hidden-480'>"+JY.Object.notEmpty(l.remark)+"</td>";
            		 html+=JY.Tags.setFunction(l.openid,permitBtn);
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
function check(openid){
	cleanForm();
	JY.Ajax.doRequest(null,jypath +'/backstage/weixin/follower/find',{openid:openid},function(data){
    		setForm(data);
    		JY.Model.check("auDiv");      	
	});
}
function cleanForm(){
	JY.Tags.cleanForm("auForm");
}
function setForm(data){
	var l=data.obj;
	$("#auForm input[name$='openid']").val(l.openid);
	$("#auForm input[name$='nickname']").val(JY.Object.notEmpty(l.nickname));
	$("#auForm input[name$='country']").val(JY.Object.notEmpty(l.country));
	$("#auForm input[name$='province']").val(JY.Object.notEmpty(l.province));
	$("#auForm input[name$='city']").val(JY.Object.notEmpty(l.city));
	$("#auForm textarea[name$='remark']").val(JY.Object.notEmpty(l.remark));
}