/**
 * 该文件是在独立页面打开直接调用暴露的飞鸽API时需要引入的文件
 * 
 *  操作说明：
 *  第1步:. 需要在您的 page 页面里增加一个 div  id= iconIM
 *  第2步:. 增加一个div click的时间激活 OpenMsgPanel(). 打开消息面板.
 *  第3步:. 重写加载方法, 调用 LoginIM() 让其登录 .
 *  第4步:. 修改  FG_DeptName 组织名称 与其他的设置. .
 * */

// 全局配置文件.
// var FG_OrgNo = ""; //组织名称.
// var FG_DeptName = "未命名组织(请修改\WF\Portal\FeigeAPI.js文件)"; //组织名称.
var feiGe_IsOpenMsgWhenHaveMsg = false; //当有消息的时候，是否打开？

// 小， 中，大. 
var feiGe_MsgShowModel = 0; //消息ICON的风格.
var FG_OrgNo = '', FG_OrgName = ''; //全局变量.
var userID = "", userName = "", deptNo, DeptName; //当前用户登录信息.

  // 开发者需要提供的接口: 给当前登录人员的信息赋值.
  var webUser = new WebUser();
  userID = webUser.No;
  userName = webUser.Name;
  deptNo = webUser.FK_Dept;
  deptName = webUser.FK_DeptName;

  //如果是saas版本.
  if (webUser.CCBPMRunModel == 2) {
    FG_OrgNo = webUser.OrgNo;      //为全局变量赋值.
    FG_OrgName = webUser.OrgName;  //为全局变量赋值.
  }

  //如果.
  if (FG_OrgNo == "") {
    FG_OrgNo = window.location.host;
  }

  /**
   * 
   * 登录要处理的业务逻辑
   * 
   * 1. 客户端，把org的信息 ， 把当前登录信息 都传递了给 后台。 
   * 2. 判断 key 与 OrgNo 是否匹配。
   * 2. 判断 OrgName 与名称是否相等，不相等，就按照 客户端的执行更新。
   * * 
   * 3. 判断人员账号是否存在？ 不存在就插入一条记录， 
   * 1  判断人员账号信息是否与数据库一致？ 不一致，按照客户端的参数计算。 包括：人员名称，部门名称。
   * 1. 判断部门是否存在？ 不存在，就执行插入。
   * 1. 判断部门名称是否与客户端一致？ 不一致，就按照客户端更新。
   *  
   * */

  /*
  * 打开消息面板.
  * */
  function OpenMsgPanel() {
    umclient.OpenMsgPanel()
  }

  /**
   * 打开/创建：群组会话.
   * @param {唯一的群聊主键,在一个组织里不能重复.} groupPK
   * @param {组的名称} groupName
   * @param {组员格式： zhangsan,张三;lisi,李四; } users
   * @param {初始化消息，可以为空.} docs
   * @param {链接} url
   * @param {是否删除其他人员, 默认不删除.} isDeleteEtcUsers
   */
  function OpenGroupTalk(groupPK, groupName, users, docs, isDeleteEtcUsers=false,  url=undefined) {
    var usersOfJson = Turn2Json(users);

      umclient.CreateGroupTalk({
        orgid: FG_OrgNo,
        umid: userID,
        roomid: groupPK,
        roomname: groupName,
        phonelist: usersOfJson,
        msg: docs,
        url,
        isDeleteEtcUsers,
        issendmsg: true
      })
  }

  function Turn2Json(users) {
    users = users.replace(/\s/g, '')
    let newusers = users.split(';').map(item => item.split(',')).map(v => ({
      userid: v[0],
      username: v[1]
    }))
    return newusers;
  }

  /**
   * 向指定的人发送消息，不弹窗.
   * @param {any} toUserID
   * @param {any} toUserName
   * @param {any} docs
   * @param {any} url
   */
  function SendMessage(toUserID, toUserName, docs, url) {
    umclient.CreateTalk({
      orgid: FG_OrgNo,
      umid: userID,
      toumid: toUserID,
      toUserName,
      msg: docs,
      url
    })
  }

  /**
   * 打开对话框，
   * @param {发送到的人员ID} toUserID
   * @param {发送到的人员名称} toUserName
   * @param {消息内容} docs
   * @param {是否发送消息？} isSendMsg
   * @param {是否填充待发送区域?} backfill
   */
  function OpenChat(toUserID, toUserName, docs, isSendMsg, backfill) {
    umclient.OpenChat({
      FG_OrgNo: FG_OrgNo,
      No: userID,
      toNo: toUserID,
      toName: toUserName,
      msg: docs,
      issendmsg: isSendMsg,
      backfill: backfill
    })
  }

  function addScript(url) {
    var script = document.createElement('script')
    script.setAttribute('type', 'text/javascript')
    script.setAttribute('src', url)
    document.getElementsByTagName('head')[0].appendChild(script)
  }

  function addStyle(url) {
    var script = document.createElement('link')
    script.setAttribute('rel', 'stylesheet')
    script.setAttribute('href', url)
    document.getElementsByTagName('head')[0].appendChild(script)
  }

  /**
   * 初始化UMSDK
   * @param {Object} options
   * @param {String} badge 红泡标记父级的class或者id名
   */
  function initSDK(options, badge = undefined) {
    return new Promise(async resolve => {
      await umclient.init(options, badge)
      resolve()
    })
  }
