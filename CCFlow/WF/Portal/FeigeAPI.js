/**
 *  操作说明：
 *  1. 需要在您的 page 页面里增加一个 div  id= iconIM
 *  2. 需要在飞鸽的官网上注册一个企业(组织),并获得官网提供的组织编号 OrgNo，与密钥 OrgKey.
 *  3. 修改本js文件的如下配置.
 * */

// 全局配置文件.
var feiGe_Host = "http://dev.ccc.com"; //飞鸽的服务器地址. 请登录 http://111.111.111 注册企业账号.
var feiGe_OrgNo = "CCFlowDemo"; //组织账号.
var feiGe_OrgName = "驰骋软件"; //组织名称.
var feiGe_OrgKey = "324324234234"; //密钥.
var feiGe_IsOpenMsgWhenHaveMsg = false; 

// 小， 中，大. 
var feiGe_MsgShowModel = 0; //消息ICON的风格.

//页面启动函数.
$(function () {

    var msg = $("#iconIM");
    if (msg == null) {
        alert("没有找到 divID=iconIM  的标记，无法初始化im系统。");
        return;
    }

    // 开发者需要提供的接口: 给当前登录人员的信息赋值.
    var webUser = new WebUser();

    //执行登录.
    LoginIM(webUser.No, webUser.Name, webUser.FK_Dept, webUser.DeptName);

});

/**
 * 发送消息
 * @param {any} userIDs
 * @param {any} docs
 * @param {any} url
 */
function SendMsgTo(userIDs, docs, url) {

}

/**
 * 
 * 登录要处理的业务逻辑
 * 
 * 1. 客户端，把org的信息 ， 把当前登录信息 都传递了给 后台。 
 * 2. 判断 key 与OrgNo 是否匹配。
 * 2. 判断 OrgName 与名称是否相等，不相等，就按照 客户端的执行更新。
 * * 
 * 3. 判断人员账号是否存在？ 不存在就插入一条记录， 
 * 1  判断人员账号信息是否与数据库一致？ 不一致，按照客户端的参数计算。 包括：人员名称，部门名称。
 * 1. 判断部门是否存在？ 不存在，就执行插入。
 * 1. 判断部门名称是否与客户端一致？ 不一致，就按照客户端更新。
 *  
 * */


//当前登录信息变量.
var currUserNo = null; //当前登录用户的账号.
var currUserName = null; //当前登录的用户名.
var currUserDeptNo = null; //当前登录部门编号》
var currUserDeptName = null; //当前登录的部门名称.

function LoginIM(userNo, userName, deptNo, deptName) {

    currUserNo = userNo;
    currUserName = userName;
    currUserDeptNo = deptNo;
    currUserDeptName = deptName;

    // 一下代码官方提供.
    //1. 加载指定飞鸽远程的js,ccs文件. 
    var url = feiGe_Host + "xxxx.js";

    // 2. 让当前人员登录，把curr*的变量传递过去, 并返回当前人员信息的数量. 
    var msgNum = 0;

    // 3. 初始化消息提示风格. 把消息内容绑定到 div = iconIM 上.

}

function DemoIt() {
    //创建工作.
    CreateGroupTalk("xxxx", "zhangsa,lisi,wangwu", "关于:张三请假的讨论",
        " 请各位发表意见。", "xxxx.htm?WorkID=xxx*SDID==xxx");
}

/*
 * 打开消息面板.
 * 
 * */
function OpenMsgPanel() {

    alert("尚未完成...");
    return;

    var url = "https://open.umnet.cn/#/sdk3?col=F67C01";
    window.open(url);
}

/**
 * 创建一个多会话： 从一个工作上打开会话.
 * @param {密钥} orgSDI
 * @param {群聊的主键} orgSgroupPKDI
 * @param {多个人用都好分开} userIds ， "@zhangsan,张三@lisi,李四"
 * @param {标题} title
 * @param {创建后的文字内容:可选} docs
 * @param {url链接：可选} url
 *
 */
function CreateGroupTalk(orgSDI, groupPK, userIds, title, docs, url) {

}

/**
 * 创建单人聊天
 * @param {any} orgSDI
 * @param {any} userID
 * @param {any} title
 * @param {any} docs
 * @param {any} url
 */
function CreateTalk(orgSDI, userID, title, docs, url) {


}