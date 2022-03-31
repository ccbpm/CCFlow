# 联信网格嵌入式即时通讯tinyIM
## 概况
联信网格嵌入式即时通讯tinyIM是一款纯JavaScript原生开发的带UI界面的即时通讯系统，一行代码嵌入第三方应用中，即可拥有即时通讯能力。
## 快速开始
在页面中引入umgrid.tinyim.js
``` js
<script type="text/javascript"src="umgrid.tinyim.js"></script>
```
## 使用说明
### 　初始化
 　首先，需要初始化即时通讯系统，需要传入第三方应用中，登录者的帐号、密码等信息。

``` js
umclient.init({
  No: '用户ID', 
  name: '用户名字',
  Password: '用户密码',
  FG_OrgNo: '组织ID',
  FG_OrgName: '组织名字',
  FG_DeptNo: '部门ID',
  FG_DeptName: '部门名字' 
}, '红泡标记的class或者id名')
```
### 　打开消息面板
``` js
umclient.OpenMsgPanel()
```
### 　发送消息进行对话
``` js
umclient.OpenChat({
  FG_OrgNo: '组织ID', 
  No: '用户ID',
  toNo: '对方ID', 
  toName: '对方名字',
  msg: '消息内容'
})
```