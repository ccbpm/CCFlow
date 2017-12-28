function CalcDateDiff(s) {
    var prefix = '';
	var idx = s.id.indexOf("TB_" + fromDateField);
	
	if(idx == -1){
		idx = s.id.indexOf("TB_" + toDateField);
	}
	
	prefix = s.id.substr(0,idx + "TB_".length);
	
	var from = ReqTB(fromDateField);
	
	if(!from){
		return;
	}
	
	var to = ReqTB(toDateField);	
	
	if(!to){
		return;
	}

	document.getElementById(prefix + targetField).value = (new Date(to.replace(/-/g,'/')).getTime() - new Date(from.replace(/-/g,'/')).getTime())/(24*60*60*1000);
} 
//以Demo中的请假流程为例子编写
var fromDateField = "QingJiaShiJianCong";	//开始时间字段ID
var toDateField = "QingJiaShiJianDao";	//结束时间字段ID
var targetField = "QingJiaTianShu";	//计算值写入的字段ID