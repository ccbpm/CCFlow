//JavaScript Document
var box_view_btn="loginBtn";//初始化按钮值
$(function () {
	getVerifyCode();
	//监听docuemnt的onkeydown事件看是不是按了回车键
	$(document).keydown(function(event){
		event = event ? event : window.event;
		if (event.keyCode === 13){
			$("#"+box_view_btn).trigger("click");
		}
	});
	//登录
	$("#loginBtn").click(function () {
		if("" == $("#accountNameId").val()){
			$("#accountNameId").tips({side : 3,msg : "用户名不能为空！",bg : '#FF2D2D',time : 2});
			$("#accountNameId").focus();
		}else if("" == $("#passwordId").val()){
			$("#passwordId").tips({side : 3,msg : "密码不能为空！",bg : '#FF2D2D',time : 2});
			$("#passwordId").focus();
		}else if("" == $("#verifyCodeId").val()){
			$("#verifyCodeId").tips({side : 3,msg : "验证码不能为空！",bg : '#FF2D2D',time : 2});
			$("#verifyCodeId").focus();
		}else{
			dialogloading();
			var loginname = $("#accountNameId").val();
			var password = $("#passwordId").val();
			var verifyCode=$("#verifyCodeId").val();
			var code = loginname+",jy,"+$.md5(password)+",jy,"+verifyCode;
			$.ajax({type:'POST',url:jypath +'/system_login',data:{KEYDATA:code,tm:new Date().getTime()},
		            dataType:'json',success:function(data, textStatus) {
		            	dialogloadingClose();	            
		            	var result=data.result;
		            	if ("success" != result) {  //如果登录不成功，则再次刷新验证码
		            		dialogloadingClose();
		            		clearLoginForm();//清除信息
							loginAlert(result);		
						}else{
							window.location.href=jypath+"/backstage/index";
						}
		            }
		     });
		}
	});
	//注册
	$("#registerBtn").click(function () {
		if("" == $("#registerEmail").val()){
			//alert("Email不能为空！");
			$("#registerEmail").focus();
			return;
		}	
			var params =$("#registerForm").serializeArray();
			alert("暂停注册");
	});
	//忘记密码
	$("#forgotBtn").click(function () {
		if("" == $("#forgotEmail").val()){
			alert("找回要邮箱");
			return;
		}			
	});
	$("#vimg").click(function () {
		getVerifyCode();
	});
	//改写dialog是title可以使用html
	$.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
		_title: function(title) {			
			var $title = this.options.title || '&nbsp;';
			if(("title_html" in this.options)&&(this.options.title_html == true))title.html($title);
			else title.text($title);
		}
	}));
});
function clearLoginForm(){	
	$("#verifyCodeId").val("");
	$("#passwordId").val("");
	getVerifyCode();
}
//切换窗口
function show_box(id) {
	 var strs= new Array(); //定义一数组 
	 strs=id.split("-"); //字符分割 
	 box_view_btn=strs[0]+"Btn";
	 jQuery('.widget-box.visible').removeClass('visible');
	 jQuery('#'+id).addClass('visible');
}	
//刷新验证码
function getVerifyCode() {
	$("#vimg").attr("src", "verifyCode/slogin.do?random=" + Math.random());
}
function loginAlert(msg) {
	if("codeerror" == msg){
		$("#verifyCodeId").tips({side : 3,msg : "验证码不正确！",bg : '#FF2D2D',time : 2});
		$("#verifyCodeId").focus();
	}else if("nullup" == msg){
		$("#accountNameId").tips({side : 3,msg : "用户名或密码不能为空！",bg : '#FF2D2D',time : 2});
		$("#accountNameId").focus();
	}else if("nullcode" == msg){
		$("#verifyCodeId").tips({side : 3,msg : "验证码不能为空！",bg : '#FF2D2D',time : 2});
		$("#verifyCodeId").focus();
	}else if("usererror" == msg){
		$("#accountNameId").tips({side : 3,msg : "用户名或密码有误！",bg : '#FF2D2D',time : 2});
		$("#accountNameId").focus();
	}else if("attemptserror" == msg){
		dialogerror("错误次数过多！");
	}else if("error" == msg){
		dialogerror("输入有误！");
	}else if("inactive" == msg){
		dialogerror("未激活！");
	}
}

function dialogerror(Str){
	$("#jyErrorStr").empty().append(Str);
	$("#jyError").removeClass('hide').dialog({
		resizable: false,
		dialogClass: "title-no-close",
		modal: true,//设置为true，该dialog将会有遮罩层
		title: "<div class='widget-header'><h4 class='font-bold red' >错误</h4></div>",
		title_html: true,
		show:{effect:"shake",duration: 100},
		hide:{effect:"explode"},
		buttons: [{  
			html: "<i class='icon-ok bigger-110'></i>&nbsp;确认","class" : "btn btn-primary btn-xs",
			click: function() {						 
				$(this).dialog("close");
			}
		}]
	}); 
};
function dialogloading(){
	$("#jyLoadingStr").empty().append("正在登录 , 请稍后 ...");
	$("#jyLoading").removeClass('hide').dialog({
		dialogClass: "loading-no-close",
		minHeight: 50,
		resizable: false,
		modal: true,
		show:{effect:"fade"},hide:{effect:"fade"}
	});
};
function dialogloadingClose(){
	$("#jyLoading").dialog("close");
};