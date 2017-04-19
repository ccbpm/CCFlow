//发送请求
function sendSever(){
	if(JY.Validate.form("interfaceForm")){
		var serverUrl=	$("#interfaceForm input[name$='serverUrl']").val();
		if(!JY.Validate.isUrl(serverUrl)){
			 $("#interfaceForm input[name$='serverUrl']").tips({side:1,msg:'url不合法',bg:'#FF2D2D',time:5});
			return;
		}
		
		var startTime = new Date().getTime(); //请求开始时间  毫秒
		 JY.Ajax.doRequest("interfaceForm",jypath +'/backstage/tool/interfaceTest/severTest',null,function(data){
			 var endTime = new Date().getTime();  
			 var obj=data.obj;
			 $("#clientTime").html(endTime-startTime);
			 $("#serverTime").html(obj.reqTime);
			 $("#resContent").val(obj.result);
			 $("#resContent").tips({side:1,msg:data.resMsg,bg:'#75C117',time:10});
		 });
	}
}
//重置表单
function reSetForm(){
	$("#interfaceForm input[name$='requestMethod'][value='POST']").prop("checked",true);
	JY.Tags.cleanForm("interfaceForm");
	$("#serverTime").val("-");
	$("#clientTime").val("-");
}