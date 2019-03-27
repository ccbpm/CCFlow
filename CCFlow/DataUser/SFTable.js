/*
外部数据源接口文件使用说明:
0, 该文件会被引用到\CCForm\FrmGener.htm 与 /WF/MyFlowGener.htm 文件里.
1, 该文件放在\DataUser\ 下面，用于与外部系统进行字典表的数据获取.
2, 该文件可以被流程设计人员重写.
3, 在外键数据源配置中存储到 Sys_SFTable 表里.
4, 函数名与 Sys_SFTable.No 保持一致.
5, 每个方法返回的是一个json, 它至少有No,Name两个列. 如果是树形结构就是 No,Name,ParentNo 三个列.
*/

function Demo_Emps() {

}

function Demo_Depts() {

}


function XingChengLeiXing() {
	return [{
		"No" : "1",
		"Name" : "测试A"
	}, {
		"No" : "2",
		"Name" : "测试B"
	}];
}