/*!
 * Copyright (c) 2013-Now http://jeesite.com All rights reserved.
 * 
 * @author ThinkGem
 * @version 2019-1-6
 */
$("#username, #password").on("focus blur",function(){var a=this;setTimeout(function(){var b=$(a).css("borderColor");if(b!=""){$(a).prev().css("color",b)}},100)}).blur();$("#loginForm").validate({submitHandler:function(c){var d=$("#username").val(),a=$("#password").val(),b=$("#validCode").val();if(secretKey!=""){$("#username").val(DesUtils.encode(d,secretKey));$("#password").val(DesUtils.encode(a,secretKey));$("#validCode").val(DesUtils.encode(b,secretKey))}js.ajaxSubmitForm($(c),function(f,e,g){if(f.isValidCodeLogin==true){$("#isValidCodeLogin").show();$("#validCodeRefresh").click()}if(f.result=="false"&&f.message.length>0){js.showMessage(f.message)}else{js.loading($("#btnSubmit").data("loading"));if(f.__url&&f.__url!=""){location=f.__url}else{location=ctx+"/index"}}},"json",true,$("#btnSubmit").data("loginValid"));$("#username").val(d);$("#password").val(a).select().focus();$("#validCode").val(b)}});