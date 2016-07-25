
/** -- ========================= 系统升级SQL (为了方便系统升级代码写入的问题,增加该SQL) 目的是为了方便JFlow CCFlow 的统一版本升级. **/

-- 更新枚举值;
DELETE FROM Sys_Enum WHERE EnumKey ='CodeStruct';
DELETE FROM Sys_Enum WHERE EnumKey ='DBSrcType';
DELETE FROM Sys_Enum WHERE EnumKey ='TBModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmType';
DELETE FROM Sys_Enum WHERE EnumKey ='WebOfficeEnable';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmEnableRole';
DELETE FROM Sys_Enum WHERE EnumKey ='BlockModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FWCType';
DELETE FROM Sys_Enum WHERE EnumKey ='SelectAccepterEnable';
DELETE FROM Sys_Enum WHERE EnumKey ='NodeFormType';
DELETE FROM Sys_Enum WHERE EnumKey ='StartGuideWay';
DELETE FROM Sys_Enum WHERE EnumKey ='StartLimitRole';
DELETE FROM Sys_Enum WHERE EnumKey ='BillFileType';
DELETE FROM Sys_Enum WHERE EnumKey ='EventDoType';
DELETE FROM Sys_Enum WHERE EnumKey ='FormType';
DELETE FROM Sys_Enum WHERE EnumKey ='BatchRole';
DELETE FROM Sys_Enum WHERE EnumKey ='StartGuideWay';
DELETE FROM Sys_Enum WHERE EnumKey ='NodeFormType';
DELETE FROM Sys_Enum WHERE EnumKey ='FormRunType';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmTransferCustomSta';


-- 2016.07.20 升级明细表维护分组;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDtlExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDtlExt','@No=基础信息,基础信息权限信息.@IsExp=数据导入导出,数据导入导出.@FrmThreadLab=超连接,对合流节点有效用于配置与现实子线程运行的情况。@FrmTrackLab=轨迹组件,用于显示流程运行的轨迹图.');


DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FrmNodeComponent';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FrmNodeComponent','@NodeID=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.@SFLab=父子流程组件,在该节点上配置与显示父子流程.@FrmThreadLab=子线程组件,对合流节点有效，用于配置与现实子线程运行的情况。@FrmTrackLab=轨迹组件,用于显示流程运行的轨迹图.@FrmTransferCustomLab=流转自定义,在每个节点上自己控制节点的处理人.');


-- 傻瓜表单属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapFoolForm';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapFoolForm','@No=基础属性,基础属性.@Designer=设计者信息,设计者的单位信息，人员信息，可以上传到表单云.');



--新版本的流程属性,节点属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeExt','@NodeID=基本配置@FWCSta=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.@SendLab=按钮权限,控制工作节点可操作按钮.@RunModel=运行模式,分合流,父子流程@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.@MPhone_WorkModel=移动,与手机平板电脑相关的应用设置.');

DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowExt','@No=基本配置');
 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDataExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDataExt','@No=基本属性@Designer=设计者信息');
UPDATE Sys_MapData SET AppType=0 WHERE No NOT LIKE 'ND%';

-- 旧版本的流程属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeSheet';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeSheet','@NodeID=基本配置@FormType=表单@FWCSta=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.@SFSta=父子流程,对启动，查看父子流程的控件设置.@SendLab=按钮权限,控制工作节点可操作按钮.@RunModel=运行模式,分合流,父子流程@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.@MPhone_WorkModel=移动,与手机平板电脑相关的应用设置.@OfficeOpen=公文按钮,只有当该节点是公文流程时候有效');
 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowSheet';                 
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowSheet','@No=基本配置@FlowRunWay=启动方式,配置工作流程如何自动发起，该选项要与流程服务一起工作才有效.@StartLimitRole=启动限制规则@StartGuideWay=发起前置导航@CFlowWay=延续流程@DTSWay=流程数据与业务数据同步@PStarter=轨迹查看权限');
               
--2016.07 升级数据源;
UPDATE Sys_SFTable SET FK_SFDBSrc='local' WHERE FK_SFDBSrc IS NULL OR FK_SFDBSrc='';
UPDATE Sys_SFTable SET  SrcType=0 WHERE SrcType IS NULL ;

UPDATE Sys_MapAttr SET ColSpan=4 WHERE ColSpan>=3;

