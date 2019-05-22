
/** -- ========================= 系统升级SQL (为了方便系统升级代码写入的问题,增加该SQL) 目的是为了方便JFlow CCFlow 的统一版本升级. **/
update sys_groupfield set frmID=enName where frmid is  null;

UPDATE Sys_MapData SET FK_FormTree='' WHERE No LIKE 'ND%';

DELETE FROM Sys_Enum WHERE EnumKey ='SelectorModel';
DELETE FROM Sys_Enum WHERE EnumKey ='SrcType';
DELETE FROM Sys_Enum WHERE EnumKey ='CondModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmTrackSta'; 
DELETE FROM Sys_Enum WHERE EnumKey ='EventDoType'; 
DELETE FROM Sys_Enum WHERE EnumKey ='EditModel';
DELETE FROM Sys_Enum WHERE EnumKey ='AthRunModel'; 
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
DELETE FROM Sys_Enum WHERE EnumKey ='FrmType';
DELETE FROM Sys_Enum WHERE EnumKey ='FTCSta';
DELETE FROM Sys_Enum WHERE EnumKey ='SrcType';
DELETE FROM Sys_Enum WHERE EnumKey IN ('TodolistModel','CCStaWay','TWay' );

DELETE FROM Sys_GloVar WHERE GroupKey='DefVal';

INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('0','选择系统约定默认值','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.No','登陆人员账号','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.Name','登陆人员名称','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_Dept','登陆人员部门编号','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_DeptName','登陆人员部门名称','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_DeptFullName','登陆人员部门全称','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yyyy年mm月dd日','当前日期(yyyy年MM月dd日)','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yyyy年mm月dd日HH时mm分','当前日期(yyyy年MM月dd日HH时mm分)','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yy年mm月dd日','当前日期(yy年MM月dd日)','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yy年mm月dd日HH时mm分','当前日期(yy年MM月dd日HH时mm分)','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@FK_ND','当前年度','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@FK_YF','当前月份','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@CurrWorker','当前工作可处理人员','DefVal');
 
-- 升级数据源 2016.0
UPDATE Sys_SFTable SET SrcType=0 WHERE No LIKE '%.%';
UPDATE Sys_SFTable SET SrcType=1 WHERE No NOT LIKE '%.%' AND SrcType=0;

--更新日期长度.
UPDATE SYS_MAPATTR SET UIWidth=125 WHERE MYDATATYPE=6
UPDATE SYS_MAPATTR SET UIWidth=145 WHERE MYDATATYPE=7


-- 2016.11.18 升级维护附件属性.;
DELETE FROM Sys_EnCfg WHERE No='BP.Sys.FrmUI.FrmAttachmentExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Sys.FrmUI.FrmAttachmentExt',
'@MyPK=基础信息,附件的基本配置.
@DeleteWay=权限控制,控制附件的下载与上传权限.@IsRowLock=WebOffice属性,设置与公文有关系的属性配置.
@IsToHeLiuHZ=流程相关,控制节点附件的分合流.');

-- 2016.07.20 升级明细表维护分组;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDtlExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDtlExt','@No=基础信息,基础信息权限信息.@IsExp=数据导入导出,数据导入导出.@MTR=多表头,实现多表头.@IsEnableLink=超链接,显示在从表的右边.@IsCopyNDData=流程相关,与流程相关的配置非流程可以忽略.');


DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FrmNodeComponent';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FrmNodeComponent','@NodeID=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.@SFLab=父子流程组件,在该节点上配置与显示父子流程.@FrmThreadLab=子线程组件,对合流节点有效，用于配置与现实子线程运行的情况。@FrmTrackLab=轨迹组件,用于显示流程运行的轨迹图.@FTCLab=流转自定义,在每个节点上自己控制节点的处理人.');

-- 傻瓜表单属性; 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapFrmFool';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapFrmFool','@No=基础属性,基础属性.@Designer=设计者信息,设计者的单位信息，人员信息，可以上传到表单云.');


-- 2018.07.24 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowExt','@No=基础信息,基础信息权限信息.@IsBatchStart=数据&表单,数据导入导出.@DesignerNo=设计者,流程开发设计者信息');


--新版本的流程属性,节点属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeExt','@NodeID=基本配置@SendLab=按钮权限,控制工作节点可操作按钮.@RunModel=运行模式,分合流,父子流程@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.');
  
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
--UPDATE Sys_MapAttr SET ColSpan=4 WHERE ColSpan>=3;

-- 2019.03.10 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.Frm.FrmBill';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Frm.FrmBill','@No=基础信息,单据基础配置信息.@BtnNewLable=单据按钮权限,用于控制每个功能按钮启用规则.@BtnImpExcel=列表按钮,列表按钮控制@Designer=设计者,流程开发设计者信息');

-- 2019.05.15 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.Frm.FrmDict';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Frm.FrmDict','@No=基础信息,单据基础配置信息.@BtnNewLable=单据按钮权限,用于控制每个功能按钮启用规则.@BtnImpExcel=列表按钮,列表按钮控制@Designer=设计者,流程开发设计者信息');
 

