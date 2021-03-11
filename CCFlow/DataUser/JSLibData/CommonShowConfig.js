
/*
1. 该页面旨在解决各个项目所需功能显示问题，定义各个‘是否显示’变量.
2. 为了不与其他字段冲突,命名加前缀'Hide_'
3. 根据项目差异，自行定义，自行调用. 
*/
var Hide_IsRead = true; //轨迹中是否显示已阅读: true 显示
var Hide_IsOpenFrm = true; //时间轴中是否显示查看表单:true 显示
var Hide_HastenWork = true; //在途是否显示催办按钮:true 显示
var Hide_IsTodoList = true; //待办列表中是否显示查看授权:true 显示
var UserICon = "@basePath/DataUser/Siganture/"; //默认的用户签名地址
var UserIConExt = ".JPG";
var IsShowDevelopFieldLab = false; //开发者表单设计器设计插入字段时增加的文本

//Pop弹出框是否可以选择，有可以输入，输入是按回车键或者失去焦点的时候默认是输入完一条信息
var IsPopEnableSelfInput = true;

//工作处理器:发送退回成功后,提示信息关闭倒计时设置(单位:秒)
//调用的页面  /WF/Toolbar.js  , /WF/WorkOpt/AccepterOfGener.htm
var WF_WorkOpt_LeftSecond = 30;

var IsRecordUserLog = 0;
