$(function () {
	//上传
	$('#upload2code').ace_file_input({
		no_file:'请选择二维码图 ...',
		btn_choose:'选择',
		btn_change:'更改',
		droppable:false,
		onchange:null,
		thumbnail:false, //| true | large
		whitelist:'gif|png|jpg|jpeg',
		blacklist:'xls|txt'
		//onchange:''
		//
	});
});

//识别中文,生成二维码前就要把字符串转换成UTF-8，然后再生成二维码
function toUtf8(str) {    
    var out, i, len, c;    
    out = "";    
    len = str.length;    
    for(i = 0; i < len; i++) {    
        c = str.charCodeAt(i);    
        if ((c >= 0x0001) && (c <= 0x007F)) {    
            out += str.charAt(i);    
        } else if (c > 0x07FF) {    
            out += String.fromCharCode(0xE0 | ((c >> 12) & 0x0F));    
            out += String.fromCharCode(0x80 | ((c >>  6) & 0x3F));    
            out += String.fromCharCode(0x80 | ((c >>  0) & 0x3F));    
        } else {    
            out += String.fromCharCode(0xC0 | ((c >>  6) & 0x1F));    
            out += String.fromCharCode(0x80 | ((c >>  0) & 0x3F));    
        }    
    }    
    return out;    
} 

function createTwoD(){	
	var content=$("#encoderContent").val();
	if(JY.Object.notNull(content)){
		var contentutf8=toUtf8(content);
		//先清空
		$("#code").empty();
		$("#code").qrcode({ 
		    render: "table", //table方式 
		    width: 235, //宽度 
		    height:235, //高度 
		    text: contentutf8 //任意内容 
		}); 
	}else{
		$("#encoderContent").tips({side:1,msg:"请输入！",bg:'#FF2D2D',time:2});	
	}	
}

function analyze2code(){
	var upVal=$("#upload2code").val();
	if(JY.Object.notNull(upVal)){
		$("#uploadForm").ajaxSubmit({
            type:'post',
            url:jypath +'/backstage/tool/qrCode/upload2Code',
            success:function(data){
            	var o=JSON.parse(data);
            	var ores=o.res;
            	var oresMsg=o.resMsg;
            	if(ores==1){
            		$("#analyzeContent").val(o.obj);
            	}else{
            		JY.Model.error(oresMsg);  	
            	}
            }
        });
	}else{
		JY.Model.error("请选择文件");  	
	}			
}