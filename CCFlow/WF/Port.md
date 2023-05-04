### URL接口说明.
- url接口是通过使用http协议的模式进行调用流程的功能页面与实现执行流程的功能.
- 这中模式的下的做法是,驰骋bpm独立部署,各个业务系统如果使用流程服务,通过url调用的模式出现.
- 调用方式: http://xxxx.xxx.xxx/WF/Port.htm?DoWhat=xxxx&UserNo=xxxx&Token=xxxxx&其他参数Key=参数值.
- 调用页面功能页面，比如:发起、待办、在途、工作处理器,可以嵌入到自己的系统中来,让两个系统看起来是一个系统.

#### 必选参数.
- DoWhat:是约定的执行内容, 比如:StartFlow,Todolist,Runing
- Token: 是执行ccbpm登录后返回的校验字符串.
- 身份校验方式开发人员可以自行修改,详见:页面类: BP.WF.HttpHandler.WF 方法 Port_Init 校验安全性部分.
- 为了方便测试开发,ccbpm的原始版本允许传入UserNo,直接让用户登录,以下的说明都是使用UserNo进行说明.

#### 页面功能调用
- 说明:页面调用
- 所谓的页面调用就是输入参数返回url页面,开发人员可以把这个页面嵌入到自己的系统中来.
#### ******* StartFlow 发起指定的流程. ************************************************************************************
- 发起流程进入工作处理器页面.
###### 必选参数:
- 执行标记:DoWhat=StartFlow
- 身份:UserNo=xxxx
- 流程模板编号,FK_Flow

  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=StartFlow&UserNo=admin&FK_Flow=001

###### 可选参数1
- 工作ID:WorkID, 如果不传入默认为发起新的流程,传入就打开指定的流程
- 字段值:Field1,Filed2. 比如:&Tel=18660153393&Addr=山东.济南 默认就在表单的Tel,Addr字段赋值.

###### 可选参数2 : 发起流程后直接跳转到指定的节点上去.
- 跳转的节点: JumpToNodeID
- 跳转的人员:JumpToEmpNo

###### 可选参数2 : 防止发起流程重复..
- 实体主键字段名: EntityPK
- 实体主键值:ZhuangZiBianHao=xxxxx
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=DealWork&UserNo=admin&EntityPK=ZhuangZiBianHao&ZhuangZiBianHao=xxxx

 说明: 传入在高速公路施工流程中，一个桩子发起一个流程. 如果该桩子编号已经存在就获取出来WorkID，不存在就按这个主键存储.


#### ******* DealWork 处理工作. ******************************************************************************************
- 定义:流程在运转的过程中，需要查看流程的表单信息，通过参数来查看指定的流程实例的WorkID的参数。
- 根据需求场景不同,如果当前人员可以操作当前节点的工作，系统直接转入MyFlow 功能页面.

###### 必选参数
- 执行标记:DoWhat=DealWork
- 身份:UserNo=xxxx
- 流程实例ID:WorkID
- 实例:打开指定WorkID的工作.
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=DealWork&UserNo=admin&WorkID=xxxx 

###### 可选参数
 - NodeID: 要查看的指定节点的表单，系统转到MyFrm.htm功能页面,该功能页面没有工具栏.
 - 实例
- /WF/Port.htm?DoWhat=MyView&UserNo=admin&WorkID=xxxx


#### ******* MyView 查看流程表单. ******************************************************************************************
- 定义:流程在运转的过程中，需要查看流程的表单信息，通过参数来查看指定的流程实例的WorkID的参数。
- 根据需求场景不同,如果当前人员可以操作当前节点的工作，系统直接转入MyFlow 功能页面.

###### 必选参数
- 执行标记:DoWhat=MyView
- 身份:UserNo=xxxx
- 流程实例,WorkID
- 实例:打开表单系统自动定位到最后一个节点上,显示MyView的工具栏,有打印关闭按钮操作.
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=MyView&UserNo=admin&WorkID=xxxx 

###### 可选参数
 - NodeID: 要查看的指定节点的表单，系统转到MyFrm.htm功能页面,该功能页面没有工具栏.
 - 实例
- /WF/Port.htm?DoWhat=MyView&UserNo=admin&WorkID=xxxx

#### ******* 打开待办/在途/草稿. *********
- 待办:待办就是等待我要处理的问题，包括退回、移交、撤销的工作。
- 在途:我参与的流程但是该流程没有完成。
- 草稿:开始节点的，保存下来的，没有发送出去的工作。

###### 必选参数
- 执行标记:DoWhat=Todolist/Runing/Draft
- 身份:UserNo=xxxx
- 流程实例,WorkID
- 实例:
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=Todolist&UserNo=admin 

###### 可选参数1
 - NodeID: 要查看的指定节点的待办工作
 - 实例
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=Todolist&UserNo=admin&NodeID=106 打开停留在106节点的待办.

  ###### 可选参数2
 - FlowNo: 要查看的指定流程的待办
 - 实例
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=Todolist&UserNo=admin&FlowNo=009 打开流程009的所有待办.
  
#### ******* StartFrm表单  ******************************************************************************************
- 定义:流程在运转的过程中，需要查看流程的表单信息，通过参数来查看指定的流程实例的WorkID的参数。
- 根据需求场景不同,如果当前人员可以操作当前节点的工作，系统直接转入MyFlow 功能页面.

###### 新建一个表单
- 执行标记:DoWhat=StartFrm
- 身份:UserNo=xxxx
- 表单ID:FrmID=xxxxxx
- 实例:打开表单系统自动定位到最后一个节点上,显示MyView的工具栏,有打印关闭按钮操作.
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=StartFrm&UserNo=admin&FrmID=xxxxx

###### 打开表单
- 执行标记:DoWhat=StartFrm
- 身份:UserNo=xxxx
- 表单ID:FrmID=xxxxxx
- 实例:打开表单系统自动定位到最后一个节点上,显示MyView的工具栏,有打印关闭按钮操作.
  http://e.tjzzjt.cn:8080/WF/Port.htm?DoWhat=StartFrm&UserNo=admin&FrmID=xxxxx&OID=xxxxx
