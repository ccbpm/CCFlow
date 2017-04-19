/*
 * JY工具组件
 */
//var JY = JY || {Object:{},Dict:{},Page:{},Tags:{},Model:{},Validate:{},Date:{},Url:{},Ajax:{},File:{}};
var JY = JY || {};
$(function() {
	//复选框
	$('table th input:checkbox').on('click', function(){
		var that = this;
		$(this).closest('table').find('tr > td:first-child input:checkbox').each(function(){
			this.checked = that.checked;
			$(this).closest('tr').toggleClass('selected');
		});				
	});	
	//class是isValidCheckbox的选择框Yes或No，value设为1或0
	$(".isValidCheckbox [sh-isValid]").bind('click', function(){	
		$(".isValidCheckbox [hi-isValid]").val((this.checked)?"1":"0");
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
	JY={Object:{notNull:function(obj) {//判断某对象不为空..返回true 否则 false
							if (obj === null)return false;else if (obj === undefined)return false;else if (obj === "undefined")return false;else if (obj === "")return false;else if (obj === "[]")return false;else if (obj === "{}")return false;else return true;},
				notEmpty:function(obj) {//判断某对象不为空..返回obj 否则 ""
							if (obj === null)return "";else if (obj === undefined)return "";else if (obj === "undefined")return "";else if(obj === "")return "";else if (obj === "[]")return "";else if (obj === "{}")return "";else return obj;},
				serialize:function(form){var o = {};$.each(form.serializeArray(),function (index){if (o[this['name']]){o[this['name']] = o[this['name']] + "," + this['value'];}else{o[this['name']] = this['value'];}});return o;},
				//组合变量传递keys,values,types形式// 转换JSON为字符串
				comVar:function(variables){var keys = "", values = "", types = "",vars={};if (variables) {$.each(variables, function() {if (keys != "") {keys += ",";values += ",";types += ",";}keys += this.key;values += this.value;types += this.type;});}vars={keys:keys,values:values,types:types};return vars;}
		},
		Dict:{//ids 对应id值(多个逗号分隔).keys 对应key值(多个逗号分隔).type(可选)1.请选择，2.自定义数组。默认不填.dfstr (可选)自定义数组
				setSelect:function(ids,keys,type,dfstr){
					$.ajax({type:'POST',url:jypath+'/backstage/dataDict/getDictSelect',data:{ids:ids,keys:keys},dataType:'json',success:function(data,textStatus){
			        	if(data.res==1){var map=data.obj;var idss= ids.split(",");var opts="",name="";
			        		if(type==1){for(var i=0;i<idss.length;i++){name=map[idss[i]].name;opts="<option value=''>请选择</option>";$.each(map[idss[i]].items,function(n,v) {opts+="<option value='"+v.value+"'>"+v.name+"</option>";});$("#"+idss[i]+" select").append(opts);$("#"+idss[i]).trigger("liszt:updated");};}
			        		else if(type==2){var dfstrs= dfstr.split(",");for(var i=0;i<idss.length;i++){name=map[idss[i]].name;$("#"+idss[i]+" label").html(name); opts="<option value=''>"+dfstrs+"</option>";$.each(map[idss[i]].items,function(n,v) {opts+="<option value='"+v.value+"'>"+v.name+"</option>";	});$("#"+idss[i]+" select").append(opts);};}
			        		else{for(var i=0;i<idss.length;i++){var name=map[idss[i]].name;$("#"+idss[i]+" label").html(name);opts="";$.each(map[idss[i]].items,function(n,v) {opts+="<option value='"+v.value+"'>"+v.name+"</option>";});$("#"+idss[i]+" select").append(opts);}}}   
			        	//适应手机
			        	if("ontouchend" in document) {$(".chosen-select").removeClass("chosen-select");}
			        	//下拉框样式
			        	else{$(".chosen-select").chosen();  $(".chosen-select-deselect").chosen({allow_single_deselect:true});}}});
			   }
		},
		Page:{//跳转分页
			  jump:function(formId,num,JpFun){$("#"+formId+" .pageNum").val(num);eval(JpFun+"()");},
			  //设置分页单个显示数量
			  setSize:function(formId,size,JpFun){$("#"+formId+" .pageNum").val(1);$("#"+formId+" .pageSize").val(size);eval(JpFun+"()");},
			  /*自定义跳转分页*/
			  jumpCustom:function(formId,pageId,leng,JpFun){var choseJPage=$("#"+pageId+" .choseJPage").val();if(typeof(choseJPage) == "undefined")return;else if(choseJPage==0)choseJPage=1;else if(choseJPage>leng)choseJPage=leng;$("#"+formId+" .pageNum").val(choseJPage);eval(JpFun+"()");},
			  /*设置分页方法,formId 分页参数Form的Id,pageId 分页位置Id,pagesize 分页显示数量,pagenum 页码,totalCount 数据总数,fun 获得数据方法名*/
			  setPage:function(formId,pageId,pagesize,pagenum,totalCount,fun){
			  	if(totalCount>0){
			  		var pageul = $("#"+pageId+" ul"),html="";
			  		pageul.empty();
			  		var leng = parseInt((totalCount - 1)/pagesize)+1;
			  		if(pagenum - 1 >= 1){html+="<li class='prev'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,1,&apos;"+fun+"&apos;)' href='#'>首页</a></li>";html+="<li class='prev'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum - 1)+",&apos;"+fun+"&apos;)' href='#'>上页</a></li>";}
			  		else{html+="<li class='prev disabled'><a href='##'>首页</a></li>";html+="<li class='prev disabled'><a href='##'>上页</a></li>";}
			  		var all = leng>2?2:leng;//总显示个数,正常为all+1条,现在设2，显示为3条
			  		var start = 1;
			  		//all/2取整后的页数减去当前页数，判断是否为大于0
			  		var before = pagenum - parseInt(all/2);
			  		if(before > 1)start = before;
			  		var end = start + all;
			  		if(end > leng){end = leng;start = leng > all ? (leng - all) : 1;}
			  	    //现在设2,和显示对应
			  		if(pagenum>2&&leng>3){html+="<li class='' ><a href='#' onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum-2)+",&apos;"+fun+"&apos;)' >..</a></li>";}
			  		for(var ii = start ; ii <= end; ii++){
			  			var page = (parseInt(ii));
			  			if(pagenum==page){html+="<li class='active' ><a href='#'>"+page+"</a></li>";}
			  			else{html+="<li><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+ii+",&apos;"+fun+"&apos;)' href='#'>"+page+"</a></li>";}	
			  		}
			  		if(pagenum<=(leng-2)&&leng>3){html+="<li class='' ><a href='#' onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum+2)+",&apos;"+fun+"&apos;)' >..</a></li>";}
			  		if(pagenum + 1 <= leng){html+="<li class='next'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum + 1)+",&apos;"+fun+"&apos;)' href='#'>下页</a></li>";html+="<li class='next'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+leng+",&apos;"+fun+"&apos;)' href='#'>尾页</a></li>";}
			  		else{html+="<li class='next disabled'><a href='##'>下页</a></li>";html+="<li class='next disabled'><a href='##'>尾页</i></a></li>";}
			  		html+="<li class='disabled'><a href='##'>共"+leng+"页<font color='red'>"+totalCount+"</font>条</a></li>";
			  		html+="<li class='next'><input onkeyup='this.value=this.value.replace(/\D/g,&apos;&apos;)' type='number' min='1' max='"+leng+"'  placeholder='页码' class='choseJPage' ></li>";
			  		html+="<li ><a class='btn btn-mini btn-success' onclick='JY.Page.jumpCustom(&apos;"+formId+"&apos;,&apos;"+pageId+"&apos;,"+leng+",&apos;"+fun+"&apos;);' href='##'>跳转</a></li>";
			  		html+="<li class='disabled'><select onchange='JY.Page.setSize(&apos;"+formId+"&apos;,this.value,&apos;"+fun+"&apos;)' style='width:55px;float:left;' title='显示条数'>"+"<option value='5'  "+((pagesize==5)?"selected='selected'":"")+" >5</option>" +"<option value='10' "+((pagesize==10)?"selected='selected'":"")+" >10</option>" +"<option value='15' "+((pagesize==15)?"selected='selected'":"")+" >15</option>"+"</li>";
			  		pageul.append(html);
			  	}	
			  },
			  /*简化版,设置分页方法,formId 分页参数Form的Id,pageId 分页位置Id,pagesize 分页显示数量,pagenum 页码,totalCount 数据总数,fun 获得数据方法名*/
			  setSimPage:function(formId,pageId,pagesize,pagenum,totalCount,fun){
			  	if(totalCount>0){
			  		var pageul = $("#"+pageId+" ul"),html="";
			  		pageul.empty();
			  		var leng = parseInt((totalCount - 1)/pagesize)+1;
			  		if(pagenum - 1 >= 1){html+="<li class='prev'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,1,&apos;"+fun+"&apos;)' href='#'>首</a></li>";}
			  		else{html+="<li class='prev disabled'><a href='##'>首</a></li>";}
			  		var all = leng>2?2:leng;//总显示个数,正常为all+1条,现在设2，显示为3条
			  		var start = 1;
			  		//all/2取整后的页数减去当前页数，判断是否为大于0
			  		var before = pagenum - parseInt(all/2);
			  		if(before > 1)start = before;
			  		var end = start + all;
			  		if(end > leng){end = leng;start = leng > all ? (leng - all) : 1;}
			  	    //现在设2,和显示对应
			  		if(pagenum>2&&leng>3){html+="<li class='' ><a href='#' onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum-2)+",&apos;"+fun+"&apos;)' >..</a></li>";}
			  		for(var ii = start ; ii <= end; ii++){
			  			var page = (parseInt(ii));
			  			if(pagenum==page){html+="<li class='active' ><a href='#'>"+page+"</a></li>";}
			  			else{html+="<li><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+ii+",&apos;"+fun+"&apos;)' href='#'>"+page+"</a></li>";}	
			  		}
			  		if(pagenum<=(leng-2)&&leng>3){html+="<li class='' ><a href='#' onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+(pagenum+2)+",&apos;"+fun+"&apos;)' >..</a></li>";}
			  		if(pagenum + 1 <= leng){html+="<li class='next'><a onclick='JY.Page.jump(&apos;"+formId+"&apos;,"+leng+",&apos;"+fun+"&apos;)' href='#'>尾</a></li>";}
			  		else{html+="<li class='next disabled'><a href='##'>尾</i></a></li>";}
			  		html+="<li class='disabled'><a href='##'>共"+leng+"页</a></li>";
			  		pageul.append(html);
			  	}	
			  }
		},
		Tags:{//设置按钮用的方法,id 这行的id,pBtn 按钮组
			cleanForm:function(formId){$("#"+formId+" input[type$='text']").val("");$("#"+formId+" textarea").val("");},	
			setFunction:function(id,pBtn){
				var h="";
				if(pBtn!=null&&pBtn.length>0){
					h+="<td class='center'>";
					h+="<div class='visible-md visible-lg hidden-sm hidden-xs btn-group'>";
					for(var i=0;i<pBtn.length;i++){h+="<a href='#' title='"+JY.Object.notEmpty(pBtn[i].name)+"' onclick='"+JY.Object.notEmpty(pBtn[i].btnFun)+"(&apos;"+id+"&apos;)' class='aBtnNoTD' ><i class='"+pBtn[i].icon+" bigger-140'></i></a>";}
					h+="</div>";
					h+="<div class='visible-xs visible-sm hidden-md hidden-lg'><div class='inline position-relative'>";
					h+="<button class='btn btn-minier btn-primary dropdown-toggle' data-toggle='dropdown'><i class='icon-cog icon-only bigger-110'></i></button>";
					h+="<ul class='dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close'>";
					for(var i=0;i<pBtn.length;i++){h+="<li><a href='#' title='"+JY.Object.notEmpty(pBtn[i].name)+"' onclick='"+JY.Object.notEmpty(pBtn[i].btnFun)+"(&apos;"+id+"&apos;)' class='aBtnNoTD' ><i class='"+pBtn[i].icon+" bigger-140'></i></a></li>";}
					h+="</ul></div></div>";		
					h+="</td>";
				}else{h+="<td></td>";}
				return h;
			},
			/*class是isValidCheckbox的选择框Yes或No，value设为1或0
			 *formId form的Id
			 */
			isValid:function(formId,val){$("#"+formId+" .isValidCheckbox [hi-isValid]").val(val);if(val==1){$("#"+formId+" .isValidCheckbox [sh-isValid]").prop("checked",true);}else{$("#"+formId+" .isValidCheckbox [sh-isValid]").prop("checked",false);}}
		},
		Model:{//loading框
			loading:function(Str){
				if(JY.Object.notNull(Str))$("#jyLoadingStr").empty().append(Str);
				else                      $("#jyLoadingStr").empty().append("数据读取中...");
				$("#jyLoading").removeClass('hide').dialog({dialogClass: "loading-no-close",minHeight:50,resizable: false,modal: true,show:{effect:"fade"},hide:{effect:"fade"}});
				},
			//关闭loading框
			loadingClose:function(){$("#jyLoading").dialog("close");},
				//Str 询问语句可以是html格式,fn  方法
			confirm:function(Str,fn){
				$("#jyConfirmStr").empty();
				if(JY.Object.notNull(Str)) $("#jyConfirmStr").append(Str);
				else                       $("#jyConfirmStr").append("确认吗？");
				$("#jyConfirm").removeClass('hide').dialog({resizable:false,modal:true,title:"<div class='widget-header'><h4 class='font-bold'>询问</h4></div>",title_html:true,show:{effect:"explode",pieces:9},hide:{effect:"explode",pieces:9},
					buttons: [{html: "<i class='icon-ok bigger-110'></i>&nbsp;确认","class" : "btn btn-primary btn-xs",click: function() {$(this).dialog("close");if(typeof(fn) == 'function'){fn.call(this);}}},
					          {html:"<i class='icon-remove bigger-110'></i>&nbsp;取消","class":"btn btn-xs",click: function() {$(this).dialog("close");}}
					         ]
				});
			},
			info:function(Str,fn){
				if(JY.Object.notNull(Str)){$("#jyInfoStr").empty().append(Str);$("#jyInfo").removeClass('hide').dialog({resizable:false,dialogClass:"title-no-close",modal:true,title: "<div class='widget-header'><h4 class='font-bold'>提示</h4></div>",title_html: true,show:{effect:"explode",pieces:9},hide:{effect:"explode",pieces:9},buttons: [{html:"<i class='icon-ok bigger-110'></i>&nbsp;确认","class":"btn btn-primary btn-xs",click:function(){$(this).dialog("close");if(typeof(fn) == 'function'){fn.call(this);}}}]}); }
			},
			//Str 语句可以是html格式（如：<span>保存成功</span>）,fn 方法
			error:function(Str){
				if(JY.Object.notNull(Str)){$("#jyErrorStr").empty().append(Str);$("#jyError").removeClass('hide').dialog({resizable:false,dialogClass:"title-no-close",modal:true,title:"<div class='widget-header'><h4 class='font-bold red'>错误</h4></div>",title_html: true,show:{effect:"shake",duration:100},hide:{effect:"explode"},buttons: [{html:"<i class='icon-ok bigger-110'></i>&nbsp;确认","class":"btn btn-primary btn-xs",click:function(){$(this).dialog("close");}}]});}
			},
			//divId,fn 方法
			check:function(id,title,fn){
				$("#"+id).removeClass('hide').dialog({resizable:false,modal:true,title:"<div class='widget-header'><h4 class='smaller'>"+(JY.Object.notNull(title)?title:"查看")+"</h4></div>",title_html:true,buttons:[{html:"<i class='icon-remove bigger-110'></i>&nbsp;取消","class":"btn btn-xs",click:function(){$(this).dialog("close");if(typeof(fn) == 'function'){fn.call(this);}}}]});
			},
			edit:function(id,title,savefn,cancelfn){
				$("#"+id).removeClass('hide').dialog({resizable:false,modal:true,title:"<div class='widget-header'><h4 class='smaller'>"+(JY.Object.notNull(title)?title:"修改")+"</h4></div>",title_html:true,buttons:[{html: "<i class='icon-ok bigger-110'></i>&nbsp;保存","class":"btn btn-primary btn-xs",click:function(){if(typeof(savefn) == 'function'){savefn.call(this);}}},{html:"<i class='icon-remove bigger-110'></i>&nbsp;取消","class":"btn btn-xs",click:function(){$(this).dialog("close");if(typeof(cancelfn) == 'function'){cancelfn.call(this);}}}]});
			},
			//审核
			audit:function(id,title,savefn,rejectfn,cancelfn){
				$("#"+id).removeClass('hide').dialog({resizable:false,modal:true,title:"<div class='widget-header'><h4 class='smaller'>"+(JY.Object.notNull(title)?title:"审核")+"</h4></div>",title_html:true,buttons:[{html: "<i class='icon-ok bigger-110'></i>&nbsp;同意","class":"btn btn-primary btn-xs",click:function(){if(typeof(savefn) == 'function'){savefn.call(this);}}},{html: "<i class='icon-undo bigger-110'></i>&nbsp;驳回","class":"btn btn-danger btn-xs",click:function(){if(typeof(rejectfn) == 'function'){rejectfn.call(this);}}},{html:"<i class='icon-remove bigger-110'></i>&nbsp;取消","class":"btn btn-xs",click:function(){$(this).dialog("close");if(typeof(cancelfn) == 'function'){cancelfn.call(this);}}}]});
			},
			//调整
			adjust:function(id,title,againfn,abandonfn,cancelfn){
				$("#"+id).removeClass('hide').dialog({resizable:false,modal:true,title:"<div class='widget-header'><h4 class='smaller'>"+(JY.Object.notNull(title)?title:"调整")+"</h4></div>",title_html:true,buttons:[{html: "<i class='icon-ok bigger-110'></i>&nbsp;提交","class":"btn btn-primary btn-xs",click:function(){if(typeof(againfn) == 'function'){againfn.call(this);}}},{html: "<i class='icon-flag-alt bigger-110'></i>&nbsp;放弃","class":"btn btn-danger btn-xs",click:function(){if(typeof(abandonfn) == 'function'){abandonfn.call(this);}}},{html:"<i class='icon-remove bigger-110'></i>&nbsp;取消","class":"btn btn-xs",click:function(){$(this).dialog("close");if(typeof(cancelfn) == 'function'){cancelfn.call(this);}}}]});
			}
		},
		Validate:{//判断是否是英文数字,是返回true，不是返回false
				  isEnNum:function(str){if(/^[0-9a-zA-Z]+$/.test(str))return true;return false;},
				  //判断是否是英文,是返回true，不是返回false
				  isEn:function(str){if(/^[A-Za-z]+$/.test(str))return true;return false;},
				  //判断是否是电子邮箱,是返回true，不是返回false
				  isEmail:function(email){if(/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(email))return true;return false;},
				  //判断是否是日期,是返回true，不是返回false
				  isDate:function(date){if(date.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/))return true;return false;},		
				  //判断是否是日期时间,是返回true，不是返回false
				  isDatetime:function(datetime){if(datetime.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/))return true;return false;},
				  //判断是否为合法http(s)
				  isUrl:function(str){if(/http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/.test(str))return true;return false;},
				  //判断数值是否在范围内(不包含临界):min 最少值,max 最大值,是返回true，不是返回false
				  numrange:function(v,min,max){v=parseInt(v);min=parseInt(min);max=parseInt(max);if((min<v)&&(v<max))return true;return false;},
				  //判断数值是否在范围内(包含临界),min 最少值,max 最大值,是返回true，不是返回false
				  numrangeth:function(v,min,max){v=parseInt(v);min=parseInt(min);max=parseInt(max);if((min<=v)&&(v<=max))return true;return false;},
				  //表单验证fromId,使用方法 在表单必须使用jyValidate属性
				  form:function(fromId,side) {var res=true;side=JY.Object.notNull(side)?side:1;
				  	$('#'+fromId+" input[jyValidate]").each(function(){
				  		if(res==false)return;var that=$(this);
				  		var jyValidate = $(this).attr("jyValidate").split(",");$.each(jyValidate,function(n,v){
				  			if(res==false)return;
				  			if(v=='required'){if(!JY.Object.notNull(that.val())){that.tips({side:side,msg : "必要字段！",bg:'#FF2D2D',time:1});that.focus();res=false;}}
				  			else if(v=='email'){if(JY.Object.notNull(that.val())){if(!JY.Validate.isEmail(that.val())){that.tips({side:side,msg : "电子邮箱不正确！",bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='date'){if(JY.Object.notNull(that.val())){if(!JY.Validate.isDate(that.val())){that.tips({side:side,msg : "日期格式不正确！",bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='datetime'){if(JY.Object.notNull(that.val())){if(!JY.Validate.isDatetime(that.val())){that.tips({side:side,msg : "日期时间格式不正确！",bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='numrange'){if(JY.Object.notNull(that.val())){var min=that.attr("min");var max=that.attr("max");if(!JY.Validate.numrange(that.val(),min,max)){that.tips({side:side,msg : "数字范围："+min+"~"+max,bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='numrangeth'){if(JY.Object.notNull(that.val())){var min=that.attr("min");var max=that.attr("max");		if(!JY.Validate.numrangeth(that.val(),min,max)){that.tips({side:side,msg :"数字范围："+min+"~"+max,bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='en'){if(JY.Object.notNull(that.val())){if(!JY.Validate.isEn(that.val())){that.tips({side:side,msg:"只能输入英文",bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			else if(v=='ennum'){if(JY.Object.notNull(that.val())){if(!JY.Validate.isEnNum(that.val())){that.tips({side:side,msg:"只能输入英文或数字",bg :'#FF2D2D',time:1});that.focus();res=false;}}}
				  			//extend
				          });});
				  		return res;}
				  },
		Date:{//时间格式化(默认),time 时间
			  Default:function(time){return JY.Object.notNull(time)?(new Date(time).Format("yyyy-MM-dd  hh:mm:ss")):" ";},
			  //时间格式化,time 时间,fmt 格式
			  Format:function(time,fmt){return JY.Object.notNull(time)?(new Date(time).Format(fmt)):"";}
		},
		Url:{//获取url中的参数,name 参数名,当不存在返回空字符串
			 getParam:function(name) {
				    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
				    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
				    if (r != null) return unescape(r[2]); return ""; //返回参数值
				}
		},
		Ajax:{//异步请求, form表单ID,url请求路径,param参数对象,如：{a:'test',b:2},fn回调函数
			doRequest:function(form,url,param,fn){var params = form || param || {};if (typeof form == 'string'){params = $.extend(param || {},JY.Object.serialize($("#" + form)),{menu:JY.Url.getParam("menu")});}$.ajax({type:'POST',url:url,data:params,dataType:'json',success:function(data, textStatus) { if(data.res==1){if (typeof(fn)=='function'){fn.call(this, data);}}else{if(JY.Object.notNull(data.resMsg))JY.Model.error(data.resMsg);}},error:function(){return;},beforeSend:function(){},complete:function(){}});},
		    req:function(form,url,param,fn){var params = form || param || {};if (typeof form == 'string'){params = $.extend(param || {},JY.Object.serialize($("#" + form)),{menu:JY.Url.getParam("menu")});}$.ajax({type:'POST',url:url,data:params,dataType:'json',success:function(data, textStatus) {if (typeof(fn)=='function'){fn.call(this, data);}},error:function(){return;},beforeSend:function(){},complete:function(){}});}
			},
		File:{
			//obj:对象传this, aFmats:允许格式,用"|"分隔
			fileType:function(obj,aFmats){
				if(JY.Object.notNull(aFmats)){var fileType=obj.value.substr(obj.value.lastIndexOf(".")+1).toLowerCase();//获得文件后缀名
					var aFmat=aFmats.split("|");for (f in aFmat){if(aFmat[f]==fileType){return;}}$(obj).tips({side:3,msg:'请上传'+aFmats+'格式的文件',bg:'#FF2D2D',time:3});$(obj).val('');
				}
			}
		}
	};	
/*
 * 对Date的扩展，将 Date 转化为指定格式的String
 *月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
 *年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
 *例子： 
 *(new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
 *(new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18
 */
Date.prototype.Format = function (fmt) { //author: meizz 
 var o = {
     "M+": this.getMonth() + 1, //月份 
     "d+": this.getDate(), //日 
     "h+": this.getHours(), //小时 
     "m+": this.getMinutes(), //分 
     "s+": this.getSeconds(), //秒 
     "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
     "S": this.getMilliseconds() //毫秒 
 };
 if (/(y+)/.test(fmt))fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
 for( var k in o){
	if (new RegExp("(" + k + ")").test(fmt))fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]): (("00" + o[k]).substr(("" + o[k]).length)));	
}		
 return fmt;
};
