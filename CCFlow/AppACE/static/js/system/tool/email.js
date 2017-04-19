$(function () {
	getbaseList();
	//增加回车事件
	$("#baseForm").keydown(function(e){
		 keycode = e.which || e.keyCode;
		 if(keycode==13){
			 search();
		 }
	});
	//全选或全取消
	$('#id-toggle-all').removeAttr('checked').on('click', function(){
		if(this.checked) {
			select_all();
		} else select_none();
	});
	var toolbars=[['source', '|','undo', 'redo', '|','simpleupload','insertimage','scrawl', 'insertframe','|',,'bold','italic',
	               'underline', 'fontborder', 'forecolor', 'backcolor', 'fontsize', 'fontfamily', 'paragraph',
	               'imagecenter','justifyleft', 'justifyright', 'justifycenter', 'justifyjustify', 
	               'strikethrough', 'superscript', 'subscript', 'removeformat', 'formatmatch', 
	               'autotypeset','pasteplain', '|','touppercase', 'tolowercase','|',
	               'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', 'link', 
	               'unlink','spechars','emotion','map','inserttable',
	               'edittd','edittable','date','time','print','preview','insertcode','searchreplace']];
	
	ue = UE.getEditor('editor',{toolbars:toolbars});
});
var ue;
function sendMail(){
	var toListi=$("#mailform input[name$='toList']");
	var toList=toListi.val();
	var subjecti=$("#mailform input[name$='subject']");
	var subject=subjecti.val();
	var body=ue.getContent();
	
	if(JY.Object.notNull(toList)){
		var tos=toList.split(";");
		for(var i=0;i<tos.length;i++){
			if(JY.Object.notNull(tos[i].trim())&&!JY.Validate.isEmail(tos[i])){
				toListi.tips({side:1,msg:"电子邮箱不正确！",bg:'#FF2D2D',time:1});
				toListi.focus();
				return false;
			}
		}	
	}else{
		toListi.tips({side:1,msg:"请填写收件人",bg:'#FF2D2D',time:1});
		toListi.focus();
		return false;
	}

	if(!JY.Object.notNull(subject)){
		subjecti.tips({side:1,msg:"请填写主题",bg:'#FF2D2D',time:1});
		subjecti.focus();
		return false;
	}
	var params={toList:toList,subject:subject,body:body};
	JY.Model.loading();
	JY.Ajax.doRequest(null,jypath +'/backstage/tool/email/sendMail',params,function(data){
		JY.Model.loadingClose();
	        	JY.Model.info(data.resMsg,function(){
	        		toListi.val("");
	        		subjecti.val("");
		        	ue.execCommand('clearlocaldata');//清空内容
		        	ue.execCommand('cleardoc');//清空草稿箱
		        	writeClose();
		        });		 
	});
}

function search(){
	$("#searchBtn").trigger("click");
}

function getbaseList(init){
	if(init==1) $("#baseForm .pageNum").val(1);	
	JY.Model.loading();
	JY.Ajax.doRequest("baseForm",jypath +'/backstage/tool/email/findByPage',null,function(data){
		 $("#sentTable").empty();
    		 var obj=data.obj;
        	 var list=obj.list;
        	 var results=list.results;
        	 var pageNum=list.pageNum,pageSize=list.pageSize,totalRecord=list.totalRecord;
        	 var html="";
    		 if(results!=null&&results.length>0){
        		 for(var i = 0;i<results.length;i++){
            		 var l=results[i];
            		 html+="<div class='message-item message-unread'>";
            		 html+="<label class='inline'><input type='checkbox' class='ace'> <span class='lbl'></span></label>";
            		 html+="<i class='message-star icon-twitter orange2'></i>";
            		 html+="<span title='"+JY.Object.notEmpty(l.toList)+"' class='sender'>"+JY.Object.notEmpty(l.toList)+"</span>";
            		 html+="<span class='time'>"+comtime(l.createTime)+"</span>";
            		 html+="<span class='summary'>";
            		 html+="<span class='text'>"+JY.Object.notEmpty(l.subject)+"</span>";  
            		 html+="</div>";  
            	 } 
        		 $("#sentTable").append(html);
        		 JY.Page.setPage("baseForm","pageing",pageSize,pageNum,totalRecord,"getbaseList");
        	 }else{
        		html+="<div class='message-item message-unread '><div class='center'>没有相关数据</div></div>";
        		$("#sentTable").append(html);
        		$("#pageing ul").empty();//清空分页
        	 }	
        	 JY.Model.loadingClose(); 
	});
}
function writeClose(){
	$("#sentA").trigger("click");
	search();
}
function select_all(){
	var count = 0;
	$('.message-item input[type=checkbox]').each(function(){
		this.checked = true;
		$(this).closest('.message-item').addClass('selected');
		count++;
	});
	$('#id-toggle-all').get(0).checked = true;
}
function select_none(){
	$('.message-item input[type=checkbox]').removeAttr('checked').closest('.message-item').removeClass('selected');
	$('#id-toggle-all').get(0).checked = false;
}

function comtime(time){
	var resTime="";
	if(JY.Object.notNull(time)){
		var cToday=new Date();
		var cTTime=cToday.getTime();//获取当前时间
		var cDate=cToday.getDate();//获取当前日
		var cMonth=cToday.getMonth()+1;//当前月份
		var cYear=cToday.getFullYear();//当前年份
		
		var dTime=new Date(time);
		var dTTime=dTime.getTime();//时间
		var dDate=dTime.getDate();//日
		var dMonth=dTime.getMonth()+1;//月份
		var dYear=dTime.getFullYear();//年份		
		
		if((cYear-dYear)>0){
			resTime=dTime.Format("yyyy/dd/MM");
		}else if(cMonth==dMonth && cDate==dDate){
			var difTime=cTTime-dTTime;
			if(difTime<60000){
				resTime= Math.floor(difTime/1000)+"秒前";
			}else if(difTime<3600000){
				resTime=Math.floor(difTime/60000)+"分钟前";
			}else{
				resTime=Math.floor(difTime/3600000)+"小时前";
			}	
		}else{
			resTime=dTime.Format("dd/MM");
		}
	}
	return resTime;
}
function getConfig(){
	JY.Tags.cleanForm("configForm");
	JY.Model.loading();
	JY.Ajax.doRequest(null,jypath +'/backstage/tool/email/getConfig',null,function(data){
			var mc=data.obj;
			$("#configForm input[name$='smtp']").val(mc.smtp);
			$("#configForm input[name$='port']").val(mc.port);
			$("#configForm input[name$='email']").val(mc.email);
			$("#configForm input[name$='emailName']").val(mc.emailName);
			$("#configForm input[name$='userName']").val(mc.userName);
			$("#configForm input[name$='password']").val(mc.password);
		JY.Model.loadingClose();
	});
}
function showPW(o){
	var checked=o.checked;
	if(checked){
		$("#configForm input[name$='password']").attr("type","text");
	}else{
		$("#configForm input[name$='password']").attr("type","password");
	}
}
