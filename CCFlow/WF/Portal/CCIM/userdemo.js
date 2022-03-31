var webUser = {
  No: '18600000000',
  Name: '张三',
  FG_DeptNo: '123456-1',
  FG_DeptName: '123456-部门名',
}
var FG_OrgNo = '1638841445388-2'
var FG_OrgName = '1638841445388-组织名'
var badgeName = '#iconIM'

window.onload = () => {
  initSDK({
    No: webUser.No,
    Name: webUser.Name,
    Password: webUser.Password || undefined,
    FG_DeptNo: webUser.FG_DeptNo,
    FG_DeptName: webUser.FG_DeptName,
    FG_OrgNo,
    FG_OrgName
  }, badgeName)
}

/**
 * 初始化UMSDK
 * @param {Object} options
 * @param {String} badge 红泡标记的class或者id名
 */
function initSDK(options, badge=undefined) {
  if (umclient) {
    umclient.init(options, badge)
  }
}
 
/*
 * 打开消息面板.
 * */
function OpenMsgPanel() {
  umclient.OpenMsgPanel()
}