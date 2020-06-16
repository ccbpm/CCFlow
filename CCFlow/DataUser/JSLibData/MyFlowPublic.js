/*
1. 该JS文件被嵌入到了MyFlowGener.htm 的工作处理器中. 
2. 开发者可以重写该文件处理通用的应用,比如通用的函数.

*/
 

//转化拼音的方法
function StrToPinYin(str) {

 
	var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
	handler.AddPara("name", str);
	handler.AddPara("flag", "false");
	data = handler.DoMethodReturnString("ParseStringToPinyin");
	return data;
}

       

/*

1. beforeSave、beforeSend、 beforeReturn、 beforeDelete 
2 .MyFlowGener、MyFlowTree的固定方法，禁止删除
3.主要写保存前、发送前、退回前、删除前事件
4.返回值为 true、false

*/


//保存前事件
function beforeSave(saveType) {
 
	return true;

}


//发生前事件
function beforeSend() {
 
	return true;
}

//退回前事件
function beforeReturn() {
	return true;
}

//删除前事件
function beforeDelete() {
	return true;
}


//抄送阅读页面增加关闭前事件
function beforeCCClose() {
	return true;
}

//关闭弹出窗刷新页面
function WindowCloseReloadPage(msg) {

	//ReloadGxjtList(msg);

} 