/*
 Navicat Premium Data Transfer

 Source Server         : 127.0.0.1
 Source Server Type    : MySQL
 Source Server Version : 80027
 Source Host           : localhost:3306
 Source Schema         : dev011

 Target Server Type    : MySQL
 Target Server Version : 80027
 File Encoding         : 65001

 Date: 15/02/2023 11:42:35
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for frm_bbs
-- ----------------------------
DROP TABLE IF EXISTS `frm_bbs`;
CREATE TABLE `frm_bbs`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'No',
  `Name` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '标题',
  `ParentNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点',
  `WorkID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '工作ID/OID',
  `Docs` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '内容',
  `Rec` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期时间',
  `DeptNo` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门编号',
  `DeptName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FrmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FrmName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单名称(可以为空)',
  `MyFileName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附件或图片',
  `MyFilePath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFilePath',
  `MyFileExt` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFileExt',
  `WebPath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'WebPath',
  `MyFileH` int(0) NULL DEFAULT 0 COMMENT 'MyFileH',
  `MyFileW` int(0) NULL DEFAULT 0 COMMENT 'MyFileW',
  `MyFileSize` float NULL DEFAULT NULL COMMENT 'MyFileSize',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Frm_BBS_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单评论组件表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_collection
-- ----------------------------
DROP TABLE IF EXISTS `frm_collection`;
CREATE TABLE `frm_collection`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法名称',
  `MethodID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法ID',
  `MethodModel` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法模式',
  `Tag1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Mark` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FrmID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `GroupID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '分组ID',
  `WarningMsg` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '功能执行警告信息',
  `MethodDocTypeOfFunc` int(0) NULL DEFAULT 0 COMMENT '内容类型',
  `MsgSuccess` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '成功提示信息',
  `MsgErr` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '失败提示信息',
  `Docs` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '方法内容',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用？',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `RefMethodType` int(0) NULL DEFAULT 0 COMMENT '页面打开方式',
  `PopHeight` int(0) NULL DEFAULT 0 COMMENT '弹窗高度',
  `PopWidth` int(0) NULL DEFAULT 0 COMMENT '弹窗宽度',
  `WhatAreYouTodo` int(0) NULL DEFAULT 0 COMMENT '执行完毕后干啥？',
  `ShowModel` int(0) NULL DEFAULT 0 COMMENT '显示方式',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Frm_Collection_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '列表连接' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_deptcreate
-- ----------------------------
DROP TABLE IF EXISTS `frm_deptcreate`;
CREATE TABLE `frm_deptcreate`  (
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '表单',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '可以创建部门',
  PRIMARY KEY (`FrmID`, `FK_Dept`) USING BTREE,
  INDEX `Frm_DeptCreateID`(`FrmID`, `FK_Dept`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单据部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_dictflow
-- ----------------------------
DROP TABLE IF EXISTS `frm_dictflow`;
CREATE TABLE `frm_dictflow`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FlowNo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Label` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '功能标签',
  `IsShowListRight` int(0) NULL DEFAULT 0 COMMENT '是否显示在列表右边',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Frm_DictFlow_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '台账子流程' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_empcreate
-- ----------------------------
DROP TABLE IF EXISTS `frm_empcreate`;
CREATE TABLE `frm_empcreate`  (
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '表单',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  PRIMARY KEY (`FrmID`, `FK_Emp`) USING BTREE,
  INDEX `Frm_EmpCreateID`(`FrmID`, `FK_Emp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单据可创建的人员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_func
-- ----------------------------
DROP TABLE IF EXISTS `frm_func`;
CREATE TABLE `frm_func`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法名',
  `FuncID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法ID',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `FuncSrc` int(0) NULL DEFAULT 0 COMMENT '功能来源',
  `DTSName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '功能内容',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '功能说明',
  `WarningMsg` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '独立方法警告信息',
  `MethodDocTypeOfFunc` int(0) NULL DEFAULT 0 COMMENT '内容类型',
  `MethodDoc_Url` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'URL执行内容',
  `MsgSuccess` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '成功提示信息',
  `MsgErr` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '失败提示信息',
  `IsHavePara` int(0) NULL DEFAULT 0 COMMENT '是否含有参数?',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Frm_Func_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '功能' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_generbill
-- ----------------------------
DROP TABLE IF EXISTS `frm_generbill`;
CREATE TABLE `frm_generbill`  (
  `WorkID` int(0) NOT NULL DEFAULT 0 COMMENT 'WorkID',
  `FK_FrmTree` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据类别',
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据ID',
  `FrmName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据名称',
  `BillNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号',
  `Title` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `BillSta` int(0) NULL DEFAULT 0 COMMENT '状态(简)',
  `BillState` int(0) NULL DEFAULT 0 COMMENT '单据状态',
  `Starter` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '创建人',
  `StarterName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '创建人名称',
  `Sender` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `SendDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据活动时间',
  `NDStep` int(0) NULL DEFAULT 0 COMMENT '步骤',
  `NDStepName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '步骤名称',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `DeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门名称',
  `PRI` int(0) NULL DEFAULT 1 COMMENT '优先级',
  `SDTOfNode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点应完成时间',
  `SDTOfFlow` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据应完成时间',
  `PFrmID` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父单据编号',
  `PWorkID` int(0) NULL DEFAULT 0 COMMENT '父单据ID',
  `TSpan` int(0) NULL DEFAULT 0 COMMENT '时间段',
  `AtPara` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参数(单据运行设置临时存储的参数)',
  `Emps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参与人',
  `GUID` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `FK_NY` varchar(7) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年月',
  PRIMARY KEY (`WorkID`) USING BTREE,
  INDEX `Frm_GenerBill_WorkID`(`WorkID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单据控制表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_groupmethod
-- ----------------------------
DROP TABLE IF EXISTS `frm_groupmethod`;
CREATE TABLE `frm_groupmethod`  (
  `No` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `FrmID` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `Icon` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `OrgNo` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  `AtPara` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Frm_GroupMethod_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '方法分组' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_method
-- ----------------------------
DROP TABLE IF EXISTS `frm_method`;
CREATE TABLE `frm_method`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法名',
  `MethodID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法ID',
  `GroupID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分组ID',
  `MethodModel` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '方法模式',
  `Tag1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Mark` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FrmID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `ShowModel` int(0) NULL DEFAULT 0 COMMENT '显示方式',
  `PopHeight` int(0) NULL DEFAULT 100 COMMENT '弹窗高度',
  `PopWidth` int(0) NULL DEFAULT 260 COMMENT '弹窗宽度',
  `IsMyBillToolBar` int(0) NULL DEFAULT 1 COMMENT '是否显示在MyBill.htm工具栏上',
  `IsMyBillToolExt` int(0) NULL DEFAULT 0 COMMENT '是否显示在MyBill.htm工具栏右边的更多按钮里',
  `IsSearchBar` int(0) NULL DEFAULT 0 COMMENT '是否显示在Search.htm工具栏上(用于批处理)',
  `DTSDataWay` int(0) NULL DEFAULT 0 COMMENT '同步相同字段数据方式',
  `DTSSpecFiels` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '要同步的字段',
  `DTSWhenFlowOver` int(0) NULL DEFAULT 0 COMMENT '流程结束后同步？',
  `DTSWhenNodeOver` int(0) NULL DEFAULT 0 COMMENT '节点发送成功后同步？',
  `WhatAreYouTodo` int(0) NULL DEFAULT 0 COMMENT '执行完毕后干啥？',
  `MethodDoc_Url` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '连接URL',
  `Docs` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '方法内容',
  `RefMethodType` int(0) NULL DEFAULT 0 COMMENT '方法类型',
  `WarningMsg` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '功能执行警告信息',
  `MsgSuccess` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '成功提示信息',
  `MsgErr` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '失败提示信息',
  `MethodDocTypeOfFunc` int(0) NULL DEFAULT 0 COMMENT '内容类型',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用？',
  `IsList` int(0) NULL DEFAULT 0 COMMENT '是否显示在列表?',
  `IsHavePara` int(0) NULL DEFAULT 0 COMMENT '是否含有参数?',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Frm_Method_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '功能方法' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_stationcreate
-- ----------------------------
DROP TABLE IF EXISTS `frm_stationcreate`;
CREATE TABLE `frm_stationcreate`  (
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '表单',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '可以创建角色',
  PRIMARY KEY (`FrmID`, `FK_Station`) USING BTREE,
  INDEX `Frm_StationCreateID`(`FrmID`, `FK_Station`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单据角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_stationdept
-- ----------------------------
DROP TABLE IF EXISTS `frm_stationdept`;
CREATE TABLE `frm_stationdept`  (
  `FK_Frm` varchar(190) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '单据编号',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '工作角色',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '部门',
  PRIMARY KEY (`FK_Frm`, `FK_Station`, `FK_Dept`) USING BTREE,
  INDEX `Frm_StationDeptID`(`FK_Frm`, `FK_Station`, `FK_Dept`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单据角色部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_toolbarbtn
-- ----------------------------
DROP TABLE IF EXISTS `frm_toolbarbtn`;
CREATE TABLE `frm_toolbarbtn`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `BtnID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '按钮ID',
  `BtnLab` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `FrmID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `PopHeight` int(0) NULL DEFAULT 0 COMMENT '弹窗高度',
  `PopWidth` int(0) NULL DEFAULT 0 COMMENT '弹窗宽度',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用？',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Frm_ToolbarBtn_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '实体工具栏按钮' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for frm_track
-- ----------------------------
DROP TABLE IF EXISTS `frm_track`;
CREATE TABLE `frm_track`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FrmName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单名称(可以为空)',
  `ActionType` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `ActionTypeText` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型(名称)',
  `WorkID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '工作ID/OID',
  `Msg` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '消息',
  `Rec` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期时间',
  `DeptNo` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门编号',
  `DeptName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `FlowNo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程ID',
  `FlowName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程名称',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `NodeName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `WorkIDOfFlow` int(0) NULL DEFAULT 0 COMMENT '工作ID/OID',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Frm_Track_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单轨迹表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_deptmenu
-- ----------------------------
DROP TABLE IF EXISTS `gpm_deptmenu`;
CREATE TABLE `gpm_deptmenu`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Menu` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '菜单',
  `FK_Dept` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `IsChecked` int(0) NULL DEFAULT 1 COMMENT '是否选中',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_DeptMenu_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '部门菜单' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_empmenu
-- ----------------------------
DROP TABLE IF EXISTS `gpm_empmenu`;
CREATE TABLE `gpm_empmenu`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Menu` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '菜单',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员',
  `FK_App` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '系统编号',
  `IsChecked` int(0) NULL DEFAULT 1 COMMENT '是否选中',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_EmpMenu_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '人员菜单对应' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_group
-- ----------------------------
DROP TABLE IF EXISTS `gpm_group`;
CREATE TABLE `gpm_group`  (
  `No` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_Group_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限组' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_groupdept
-- ----------------------------
DROP TABLE IF EXISTS `gpm_groupdept`;
CREATE TABLE `gpm_groupdept`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '权限组',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_GroupDept_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限组部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_groupemp
-- ----------------------------
DROP TABLE IF EXISTS `gpm_groupemp`;
CREATE TABLE `gpm_groupemp`  (
  `FK_Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '权限组',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  PRIMARY KEY (`FK_Group`, `FK_Emp`) USING BTREE,
  INDEX `GPM_GroupEmpID`(`FK_Group`, `FK_Emp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限组人员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_groupmenu
-- ----------------------------
DROP TABLE IF EXISTS `gpm_groupmenu`;
CREATE TABLE `gpm_groupmenu`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '权限组',
  `FK_Menu` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '菜单',
  `IsChecked` int(0) NULL DEFAULT 1 COMMENT '是否选中',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_GroupMenu_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限组菜单' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_groupstation
-- ----------------------------
DROP TABLE IF EXISTS `gpm_groupstation`;
CREATE TABLE `gpm_groupstation`  (
  `FK_Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '权限组',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '角色',
  PRIMARY KEY (`FK_Group`, `FK_Station`) USING BTREE,
  INDEX `GPM_GroupStationID`(`FK_Group`, `FK_Station`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限组角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_menu
-- ----------------------------
DROP TABLE IF EXISTS `gpm_menu`;
CREATE TABLE `gpm_menu`  (
  `No` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '报表标题',
  `Tag4` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分析项目名称',
  `ListModel` int(0) NULL DEFAULT 0 COMMENT '维度显示格式',
  `TagInt1` int(0) NULL DEFAULT 0 COMMENT '合计位置?',
  `Tag0` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '数据源SQL',
  `Tag1` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '维度1SQL',
  `Tag2` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '维度2SQL',
  `Tag3` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '维度3SQL',
  `MenuModel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '菜单模式',
  `Mark` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '标记',
  `FrmID` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'FrmID',
  `FlowNo` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'FlowNo',
  `OpenWay` int(0) NULL DEFAULT 1 COMMENT '打开方式',
  `UrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'PC端连接',
  `MobileUrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '移动端连接',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用?',
  `SystemNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SystemNo',
  `ModuleNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '隶属模块编号',
  `ModuleNoText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '隶属模块编号',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OrgNo',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  `IsMyBillToolBar` int(0) NULL DEFAULT 1 COMMENT '是否显示在MyDict.htm工具栏上',
  `IsMyBillToolExt` int(0) NULL DEFAULT 0 COMMENT '是否显示在MyDict.htm工具栏右边的更多按钮里',
  `IsSearchBar` int(0) NULL DEFAULT 0 COMMENT '是否显示在 SearchDict.htm工具栏上(用于批处理)',
  `DTSDataWay` int(0) NULL DEFAULT 0 COMMENT '同步相同字段数据方式',
  `DTSSpecFiels` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '要同步的字段',
  `DTSWhenFlowOver` int(0) NULL DEFAULT 0 COMMENT '流程结束后同步？',
  `DTSWhenNodeOver` int(0) NULL DEFAULT 0 COMMENT '节点发送成功后同步？',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_Menu_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '新建实体' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of gpm_menu
-- ----------------------------
INSERT INTO `gpm_menu` VALUES ('1101', 'icon-share-alt', '最近发起', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/StartEarlyer.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1102', 'icon-paper-plane', '发起', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Start.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1103', 'icon-bell', '待办', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Todolist.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1104', 'icon-clock', '在途', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/RuningDataGrid.htm?IsContainFuture=1', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1105', 'fa-hourglass-end', '已完成', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Complete.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1106', 'icon-layers', '批处理', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Batch.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1107', 'icon-people', '抄送', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/CC.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1108', 'icon-note', '我的草稿', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Draft.htm', '', 1, 'AppFlow', 'FlowGener', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1201', 'icon-target', '监控', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Watchdog.htm?EnsName=BP.WF.Data.MyStartFlows', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1202', 'icon-flag', '我发起的', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Comm/Search.htm?EnsName=BP.WF.Data.MyStartFlows', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1203', 'icon-user-following', '我审批的', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Comm/Search.htm?EnsName=BP.WF.Data.MyJoinFlows', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1204', 'icon-calendar', '我的流程分布', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/RptSearch/DistributedOfMy.htm', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1205', 'icon-speech', '我的流程', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/SearchDataGrid.htm', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1207', 'icon-magnifier', '综合查询', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/RptSearch/Default.htm', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1208', 'icon-magnifier', '流程监控', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Watchdog.htm', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1209', 'icon-magnifier', '逾期流程', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Comm/Search.htm?EnsName=BP.WF.Data.Delays', '', 1, 'AppFlow', 'Search', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1301', 'icon-organization', '会签主持人待办', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/HuiQianList.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1303', 'icon-organization', '协作待办', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/TeamupList.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1304', 'icon-briefcase', '授权待办', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/AuthorList.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1305', 'icon-direction', '工作委托2019', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/AuthorTodolist2019.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1307', 'icon-share', '共享任务', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/TaskPoolSharing.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1308', 'icon-folder-alt', '申请下来的任务', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/TaskPoolApply.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1309', 'icon-paper-plane', '未来任务', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/FutureTodolist.htm', '', 1, 'AppFlow', 'TodolistType', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1401', 'icon-share', '共享任务', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/TaskPoolSharing.htm', '', 1, 'AppFlow', 'FlowAdv', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1402', 'icon-folder-alt', '申请下来的任务', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/TaskPoolApply.htm', '', 1, 'AppFlow', 'FlowAdv', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1403', 'icon-star', '我的关注', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Focus.htm', '', 1, 'AppFlow', 'FlowAdv', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1405', 'icon-loop', '挂起的工作', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/HungupList.htm', '', 1, 'AppFlow', 'FlowAdv', '', '', 0, 1, 0, 0, 0, '', 0, 0);
INSERT INTO `gpm_menu` VALUES ('1406', 'icon-settings', '我的设置', NULL, NULL, 0, 0, NULL, '', NULL, NULL, '', '', '', '', 1, '../../WF/Setting/Default.htm', '', 1, 'AppFlow', 'FlowAdv', '', '', 0, 1, 0, 0, 0, '', 0, 0);

-- ----------------------------
-- Table structure for gpm_menudtl
-- ----------------------------
DROP TABLE IF EXISTS `gpm_menudtl`;
CREATE TABLE `gpm_menudtl`  (
  `No` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `RefMenuNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '菜单编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '条件标签',
  `Tag1` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标识',
  `UrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SQL/标记/枚举值',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Icon',
  `Model` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '模式',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_MenuDtl_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '标签' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_module
-- ----------------------------
DROP TABLE IF EXISTS `gpm_module`;
CREATE TABLE `gpm_module`  (
  `No` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `SystemNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属系统',
  `Icon` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT 'IsEnable',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_Module_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '模块' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of gpm_module
-- ----------------------------
INSERT INTO `gpm_module` VALUES ('FlowAdv', '高级功能', 'AppFlow', 'icon-emotsmile', 0, 1, '');
INSERT INTO `gpm_module` VALUES ('FlowGener', '基础功能', 'AppFlow', 'fa-cubes', 0, 1, '');
INSERT INTO `gpm_module` VALUES ('Search', '流程查询', 'AppFlow', 'icon-magnifier', 0, 1, '');
INSERT INTO `gpm_module` VALUES ('TodolistType', '分类待办', 'AppFlow', 'icon-emotsmile', 0, 1, '');

-- ----------------------------
-- Table structure for gpm_powercenter
-- ----------------------------
DROP TABLE IF EXISTS `gpm_powercenter`;
CREATE TABLE `gpm_powercenter`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `CtrlObj` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控制对象(SystemMenus)',
  `CtrlPKVal` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控制对象ID',
  `CtrlGroup` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属分组(可为空)',
  `CtrlModel` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控制模式',
  `IDs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '主键s(Stas,Depts等)',
  `IDNames` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'IDNames',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '编号',
  `Idx` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Idx',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_PowerCenter_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限中心' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_stationmenu
-- ----------------------------
DROP TABLE IF EXISTS `gpm_stationmenu`;
CREATE TABLE `gpm_stationmenu`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Menu` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '菜单',
  `FK_Station` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色',
  `IsChecked` int(0) NULL DEFAULT 1 COMMENT '是否选中',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_StationMenu_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色菜单' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_system
-- ----------------------------
DROP TABLE IF EXISTS `gpm_system`;
CREATE TABLE `gpm_system`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '启用?',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_System_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '系统' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of gpm_system
-- ----------------------------
INSERT INTO `gpm_system` VALUES ('AppFlow', '业务流程', 1, 'icon-emotsmile', '', 0);

-- ----------------------------
-- Table structure for gpm_window
-- ----------------------------
DROP TABLE IF EXISTS `gpm_window`;
CREATE TABLE `gpm_window`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EmpNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户编号',
  `WindowTemplateNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '模板编号',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `IsEnable` int(0) NULL DEFAULT 0 COMMENT '是否可见?',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '排序',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_Window_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '信息块' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_windowtemplate
-- ----------------------------
DROP TABLE IF EXISTS `gpm_windowtemplate`;
CREATE TABLE `gpm_windowtemplate`  (
  `No` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `ColSpan` int(0) NULL DEFAULT 1 COMMENT '占的列数',
  `WinDocModel` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '内容类型',
  `Icon` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `PageID` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '页面ID',
  `MoreLab` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '更多标签',
  `MoreUrl` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '更多链接',
  `MoreLinkModel` int(0) NULL DEFAULT 0 COMMENT '打开方式',
  `PopW` int(0) NULL DEFAULT 500 COMMENT 'Pop宽度',
  `PopH` int(0) NULL DEFAULT 400 COMMENT 'Pop高度',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容表达式',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '默认的排序',
  `IsDel` int(0) NULL DEFAULT 1 COMMENT '用户是否可删除',
  `IsEnable` int(0) NULL DEFAULT 0 COMMENT '是否禁用?',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `LabOfFZ` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分子标签',
  `SQLOfFZ` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '分子表达式',
  `LabOfFM` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分母标签',
  `SQLOfFM` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '分子表达式',
  `LabOfRate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '率标签',
  `IsPie` int(0) NULL DEFAULT 0 COMMENT '饼图?',
  `IsLine` int(0) NULL DEFAULT 0 COMMENT '折线图?',
  `IsZZT` int(0) NULL DEFAULT 0 COMMENT '柱状图?',
  `IsRing` int(0) NULL DEFAULT 0 COMMENT '显示环形图?',
  `IsRate` int(0) NULL DEFAULT 0 COMMENT '百分比扇形图?',
  `DefaultChart` int(0) NULL DEFAULT 0 COMMENT '默认显示图形',
  `DBType` int(0) NULL DEFAULT 0 COMMENT '数据源类型',
  `C1Ens` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '列1外键数据(可选)',
  `C2Ens` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '列2外键数据(可选)',
  `C3Ens` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '列3外键数据(可选)',
  `DBSrc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '数据源',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `GPM_WindowTemplate_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = 'Html信息块' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gpm_windowtemplatedtl
-- ----------------------------
DROP TABLE IF EXISTS `gpm_windowtemplatedtl`;
CREATE TABLE `gpm_windowtemplatedtl`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `RefPK` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'RefPK',
  `DBType` int(0) NULL DEFAULT 0 COMMENT '数据源类型',
  `DBSrc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据源',
  `WindowsShowType` int(0) NULL DEFAULT 0 COMMENT '显示类型',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `FontColor` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字体风格',
  `Exp0` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表达式',
  `Exp1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '表达式1',
  `UrlExt` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '链接/函数',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `GPM_WindowTemplateDtl_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '数据项' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for nd1rpt
-- ----------------------------
DROP TABLE IF EXISTS `nd1rpt`;
CREATE TABLE `nd1rpt`  (
  `PWorkID` int(0) NULL DEFAULT 0 COMMENT '父流程WorkID',
  `PNodeID` int(0) NULL DEFAULT 0 COMMENT '父流程启动的节点',
  `FlowDaySpan` float NULL DEFAULT NULL COMMENT '流程时长(天)',
  `FlowEnderRDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '结束时间',
  `FlowEndNode` int(0) NULL DEFAULT 0 COMMENT '结束节点',
  `FlowStartRDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起时间',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参数',
  `BillNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号',
  `PEmp` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '调起子流程的人员',
  `PrjNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '项目编号',
  `PrjName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '项目名称',
  `PFlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父流程编号',
  `FlowEmps` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参与人',
  `GUID` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `FlowEnder` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '结束人',
  `FlowStarter` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人',
  `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `WFSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `WFState` int(0) NULL DEFAULT 0 COMMENT '流程状态',
  `BMJLSP_Note` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '审核意见',
  `ShenQingRen` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '申请人',
  `ZJLSP_Note` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '审核意见',
  `BMJLSP_Checker` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '审核人',
  `ShenQingRiJi` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '申请日期',
  `ZJLSP_Checker` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '审核人',
  `BMJLSP_RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '审核日期',
  `ZJLSP_RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '审核日期',
  `ShenQingRenBuMen` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '申请人部门',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '更新时间',
  `OID` int(0) NOT NULL COMMENT 'OID',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `CDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '活动时间',
  `Rec` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人',
  `Emps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参与者',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员部门',
  `FK_NY` varchar(7) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年月',
  `MyNum` int(0) NULL DEFAULT 1 COMMENT '个数',
  `QingJiaRiQiCong` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '请假日期从',
  `Dao` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到',
  `QingJiaTianShu` float NULL DEFAULT NULL COMMENT '请假天数',
  `QingJiaYuanYin` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '请假原因',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `ND1Rpt_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for nd1track
-- ----------------------------
DROP TABLE IF EXISTS `nd1track`;
CREATE TABLE `nd1track`  (
  `MyPK` int(0) NOT NULL DEFAULT 0 COMMENT 'MyPK',
  `ActionType` int(0) NULL DEFAULT 0 COMMENT '类型',
  `ActionTypeText` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型(名称)',
  `FID` int(0) NULL DEFAULT 0 COMMENT '流程ID',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT '工作ID',
  `NDFrom` int(0) NULL DEFAULT 0 COMMENT '从节点',
  `NDFromT` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '从节点(名称)',
  `NDTo` int(0) NULL DEFAULT 0 COMMENT '到节点',
  `NDToT` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到节点(名称)',
  `EmpFrom` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '从人员',
  `EmpFromT` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '从人员(名称)',
  `EmpTo` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到人员',
  `EmpToT` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到人员(名称)',
  `RDT` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '日期',
  `WorkTimeSpan` float NULL DEFAULT NULL COMMENT '时间流程时长(天)',
  `Msg` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '消息',
  `NodeData` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '节点数据(日志信息)',
  `Tag` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参数',
  `Exer` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '执行人',
  INDEX `WF_Track_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_bbs
-- ----------------------------
DROP TABLE IF EXISTS `oa_bbs`;
CREATE TABLE `oa_bbs`  (
  `No` varchar(59) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `BBSPRI` int(0) NULL DEFAULT 0 COMMENT '重要性',
  `BBSType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecDeptNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人部门',
  `RelerName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布人',
  `RelDeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布单位',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布日期',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属年月',
  `MyFileNum` int(0) NULL DEFAULT 0 COMMENT '附件',
  `BBSSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `ReadTimes` int(0) NULL DEFAULT 0 COMMENT '读取次数',
  `Reader` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '读取人',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_BBS_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_bbstype
-- ----------------------------
DROP TABLE IF EXISTS `oa_bbstype`;
CREATE TABLE `oa_bbstype`  (
  `No` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_BBSType_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '类型类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_info
-- ----------------------------
DROP TABLE IF EXISTS `oa_info`;
CREATE TABLE `oa_info`  (
  `No` varchar(59) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `InfoPRI` int(0) NULL DEFAULT 0 COMMENT '重要性',
  `InfoType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `InfoSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecDeptNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人部门',
  `RelerName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布人',
  `RelDeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布单位',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发布日期',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属年月',
  `ReadTimes` int(0) NULL DEFAULT 0 COMMENT '读取次数',
  `Reader` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '读取人',
  `MyFileNum` int(0) NULL DEFAULT 0 COMMENT '附件',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_Info_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_infotype
-- ----------------------------
DROP TABLE IF EXISTS `oa_infotype`;
CREATE TABLE `oa_infotype`  (
  `No` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_InfoType_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '类型类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_kmdtl
-- ----------------------------
DROP TABLE IF EXISTS `oa_kmdtl`;
CREATE TABLE `oa_kmdtl`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `RefTreeNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联树编号',
  `KnowledgeNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '知识编号',
  `Foucs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '关注者(多个人用都好分开)',
  `IsDel` int(0) NULL DEFAULT 0 COMMENT 'IsDel',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `MyFileNum` int(0) NULL DEFAULT 0 COMMENT '附件',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_KMDtl_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '知识点' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_kmtree
-- ----------------------------
DROP TABLE IF EXISTS `oa_kmtree`;
CREATE TABLE `oa_kmtree`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ParentNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点编号',
  `KnowledgeNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '知识编号',
  `FileType` int(0) NULL DEFAULT 1 COMMENT '文件类型',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `IsDel` int(0) NULL DEFAULT 0 COMMENT 'IsDel',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_KMTree_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '知识树' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_knowledge
-- ----------------------------
DROP TABLE IF EXISTS `oa_knowledge`;
CREATE TABLE `oa_knowledge`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ImgUrl` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '路径',
  `Title` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '标题',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '描述',
  `KnowledgeSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `Emps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参与人',
  `Foucs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '关注的人(多个人用逗号分开)',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `OA_Knowledge_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '知识管理' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_notepad
-- ----------------------------
DROP TABLE IF EXISTS `oa_notepad`;
CREATE TABLE `oa_notepad`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'NianYue',
  `IsStar` int(0) NULL DEFAULT 0 COMMENT '是否标星',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_Notepad_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '记事本' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_schedule
-- ----------------------------
DROP TABLE IF EXISTS `oa_schedule`;
CREATE TABLE `oa_schedule`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `DTStart` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '开始时间',
  `DTEnd` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '结束时间',
  `TimeStart` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TimeStart',
  `TimeEnd` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TimeEnd',
  `ChiXuTime` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '持续时间',
  `DTAlert` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '提醒时间',
  `Repeats` int(0) NULL DEFAULT 0 COMMENT '重复',
  `Local` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '位置',
  `MiaoShu` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '描述',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属年月',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_Schedule_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '日程' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_task
-- ----------------------------
DROP TABLE IF EXISTS `oa_task`;
CREATE TABLE `oa_task`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Title` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `ParentNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点ID',
  `IsSubTask` int(0) NULL DEFAULT 0 COMMENT '是否是子任务',
  `TaskPRI` int(0) NULL DEFAULT 0 COMMENT '优先级',
  `TaskSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `DTFrom` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '日期从',
  `DTTo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到',
  `ManagerEmpNo` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '负责人',
  `ManagerEmpName` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '负责人名称',
  `RefEmpsNo` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参与人编号',
  `RefEmpsName` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参与人名称',
  `RefLabelNo` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签标号',
  `RefLabelName` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签名称',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_Task_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '任务' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_workchecker
-- ----------------------------
DROP TABLE IF EXISTS `oa_workchecker`;
CREATE TABLE `oa_workchecker`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `RefPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'RefPK',
  `Doc` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Doc',
  `Cent` int(0) NULL DEFAULT 0 COMMENT '评分',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_WorkChecker_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '日志审核' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_workrec
-- ----------------------------
DROP TABLE IF EXISTS `oa_workrec`;
CREATE TABLE `oa_workrec`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `WorkRecModel` int(0) NULL DEFAULT 0 COMMENT '模式',
  `Doc1` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '本日内容',
  `Doc2` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '明日内容',
  `Doc3` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '遇到的问题',
  `HeiJiHour` float NULL DEFAULT NULL COMMENT '合计小时',
  `NumOfItem` int(0) NULL DEFAULT 0 COMMENT '项目数',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `RiQi` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属日期',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年月',
  `NianDu` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年度',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_WorkRec_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作汇报' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_workrecdtl
-- ----------------------------
DROP TABLE IF EXISTS `oa_workrecdtl`;
CREATE TABLE `oa_workrecdtl`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `RefPK` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'RefPK',
  `PrjName` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '项目名称',
  `Doc` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '内容',
  `Result` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '结果',
  `Hour` int(0) NULL DEFAULT 0 COMMENT '小时',
  `Minute` int(0) NULL DEFAULT 0 COMMENT '分钟',
  `HeiJiHour` float NULL DEFAULT NULL COMMENT '合计小时',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Rec` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `RiQi` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属日期',
  `NianYue` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年月',
  `NianDu` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年度',
  `WeekNum` int(0) NULL DEFAULT 0 COMMENT '周次',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_WorkRecDtl_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作内容' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for oa_workshare
-- ----------------------------
DROP TABLE IF EXISTS `oa_workshare`;
CREATE TABLE `oa_workshare`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `EmpName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `ShareToEmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `ShareToEmpName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `ShareState` int(0) NULL DEFAULT 0 COMMENT '状态0=关闭,1=分享',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `OA_WorkShare_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '日志共享' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_dept
-- ----------------------------
DROP TABLE IF EXISTS `port_dept`;
CREATE TABLE `port_dept`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `NameOfPath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门路径',
  `ParentNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点编号',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Leader` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门领导',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '序号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_Dept_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_dept
-- ----------------------------
INSERT INTO `port_dept` VALUES ('100', '集团总部', NULL, '0', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1001', '集团市场部', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1002', '集团研发部', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1003', '集团服务部', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1004', '集团财务部', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1005', '集团人力资源部', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1060', '南方分公司', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1061', '市场部', NULL, '1060', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1062', '财务部', NULL, '1060', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1063', '销售部', NULL, '1060', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1070', '北方分公司', NULL, '100', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1071', '市场部', NULL, '1070', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1072', '财务部', NULL, '1070', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1073', '销售部', NULL, '1070', NULL, NULL, 0);
INSERT INTO `port_dept` VALUES ('1099', '外来单位', NULL, '100', NULL, NULL, 0);

-- ----------------------------
-- Table structure for port_deptemp
-- ----------------------------
DROP TABLE IF EXISTS `port_deptemp`;
CREATE TABLE `port_deptemp`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Dept` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编码',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_DeptEmp_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '部门人员信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_deptemp
-- ----------------------------
INSERT INTO `port_deptemp` VALUES ('1001_zhanghaicheng', '1001', 'zhanghaicheng', NULL);
INSERT INTO `port_deptemp` VALUES ('1001_zhangyifan', '1001', 'zhangyifan', NULL);
INSERT INTO `port_deptemp` VALUES ('1001_zhoushengyu', '1001', 'zhoushengyu', NULL);
INSERT INTO `port_deptemp` VALUES ('1002_qifenglin', '1002', 'qifenglin', NULL);
INSERT INTO `port_deptemp` VALUES ('1002_zhoutianjiao', '1002', 'zhoutianjiao', NULL);
INSERT INTO `port_deptemp` VALUES ('1003_fuhui', '1003', 'fuhui', NULL);
INSERT INTO `port_deptemp` VALUES ('1003_guoxiangbin', '1003', 'guoxiangbin', NULL);
INSERT INTO `port_deptemp` VALUES ('1004_guobaogeng', '1004', 'guobaogeng', NULL);
INSERT INTO `port_deptemp` VALUES ('1004_yangyilei', '1004', 'yangyilei', NULL);
INSERT INTO `port_deptemp` VALUES ('1005_liping', '1005', 'liping', NULL);
INSERT INTO `port_deptemp` VALUES ('1005_liyan', '1005', 'liyan', NULL);
INSERT INTO `port_deptemp` VALUES ('100_admin', '100', 'admin', '');
INSERT INTO `port_deptemp` VALUES ('100_zhoupeng', '100', 'zhoupeng', NULL);

-- ----------------------------
-- Table structure for port_deptempstation
-- ----------------------------
DROP TABLE IF EXISTS `port_deptempstation`;
CREATE TABLE `port_deptempstation`  (
  `MyPK` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Dept` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `FK_Station` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色',
  `FK_Emp` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编码',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_DeptEmpStation_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '部门角色人员对应' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_deptempstation
-- ----------------------------
INSERT INTO `port_deptempstation` VALUES ('1001_zhanghaicheng_02', '1001', '02', 'zhanghaicheng', NULL);
INSERT INTO `port_deptempstation` VALUES ('1001_zhangyifan_07', '1001', '07', 'zhangyifan', NULL);
INSERT INTO `port_deptempstation` VALUES ('1001_zhoushengyu_07', '1001', '07', 'zhoushengyu', NULL);
INSERT INTO `port_deptempstation` VALUES ('1002_qifenglin_03', '1002', '03', 'qifenglin', NULL);
INSERT INTO `port_deptempstation` VALUES ('1002_zhoutianjiao_08', '1002', '08', 'zhoutianjiao', NULL);
INSERT INTO `port_deptempstation` VALUES ('1003_fuhui_09', '1003', '09', 'fuhui', NULL);
INSERT INTO `port_deptempstation` VALUES ('1003_guoxiangbin_04', '1003', '04', 'guoxiangbin', NULL);
INSERT INTO `port_deptempstation` VALUES ('1004_guobaogeng_10', '1004', '10', 'guobaogeng', NULL);
INSERT INTO `port_deptempstation` VALUES ('1004_yangyilei_05', '1004', '05', 'yangyilei', NULL);
INSERT INTO `port_deptempstation` VALUES ('1005_liping_06', '1005', '06', 'liping', NULL);
INSERT INTO `port_deptempstation` VALUES ('1005_liyan_11', '1005', '11', 'liyan', NULL);
INSERT INTO `port_deptempstation` VALUES ('100_zhoupeng_01', '100', '01', 'zhoupeng', NULL);
INSERT INTO `port_deptempstation` VALUES ('1099_Guest_12', '1005', '12', 'Guest', NULL);

-- ----------------------------
-- Table structure for port_emp
-- ----------------------------
DROP TABLE IF EXISTS `port_emp`;
CREATE TABLE `port_emp`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `PinYin` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '拼音',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `Tel` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '手机号',
  `Email` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮箱',
  `Leader` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '直属部门领导',
  `LeaderName` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '领导名',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `Pass` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '',
  `OpenID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OpenID',
  `EmpSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_Emp_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_emp
-- ----------------------------
INSERT INTO `port_emp` VALUES ('admin', 'admin', ',admin,admin,', '100', '0531-82374939', 'zhoupeng@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('fuhui', '福惠', ',fuhui,fh,fh/jtfwb,', '1003', '0531-82374939', 'fuhui@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('guobaogeng', '郭宝庚', ',guobaogeng,gbg,gbg/jtcwb,', '1004', '0531-82374939', 'guobaogeng@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('guoxiangbin', '郭祥斌', ',guoxiangbin,gxb,gxb/jtfwb,', '1003', '0531-82374939', 'guoxiangbin@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('liping', '李萍', ',liping,lp,lp/jtrlzyb,', '1005', '0531-82374939', 'liping@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('liyan', '李言', ',liyan,ly,ly/jtrlzyb,', '1005', '0531-82374939', 'liyan@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('qifenglin', '祁凤林', ',qifenglin,qfl,qfl/jtyfb,', '1002', '0531-82374939', 'qifenglin@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('yangyilei', '杨依雷', ',yangyilei,yyl,yyl/jtcwb,', '1004', '0531-82374939', 'yangyilei@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('zhanghaicheng', '张海成', ',zhanghaicheng,zhc,zhc/jtscb,', '1001', '0531-82374939', 'zhanghaicheng@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('zhangyifan', '张一帆', ',zhangyifan,zyf,zyf/jtscb,', '1001', '0531-82374939', 'zhangyifan@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('zhoupeng', '周朋', ',zhoupeng,zp,zp/jtzb,', '100', '0531-82374939', 'zhoupeng@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('zhoushengyu', '周升雨', ',zhoushengyu,zsy,zsy/jtscb,', '1001', '0531-82374939', 'zhoushengyu@ccflow.org', '', '', '', 0, '123', '', 0);
INSERT INTO `port_emp` VALUES ('zhoutianjiao', '周天娇', ',zhoutianjiao,ztj,ztj/jtyfb,', '1002', '0531-82374939', 'zhoutianjiao@ccflow.org', '', '', '', 0, '123', '', 0);

-- ----------------------------
-- Table structure for port_org
-- ----------------------------
DROP TABLE IF EXISTS `port_org`;
CREATE TABLE `port_org`  (
  `No` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号(与部门编号相同)',
  `Name` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织名称',
  `ParentNo` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父级组织编号',
  `ParentName` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父级组织名称',
  `Adminer` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主要管理员(创始人)',
  `AdminerName` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '管理员名称',
  `FlowNums` int(0) NULL DEFAULT 0 COMMENT '流程数',
  `FrmNums` int(0) NULL DEFAULT 0 COMMENT '表单数',
  `Users` int(0) NULL DEFAULT 0 COMMENT '用户数',
  `Depts` int(0) NULL DEFAULT 0 COMMENT '部门数',
  `GWFS` int(0) NULL DEFAULT 0 COMMENT '运行中流程',
  `GWFSOver` int(0) NULL DEFAULT 0 COMMENT '结束的流程',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '排序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_Org_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '系统信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_orgadminer
-- ----------------------------
DROP TABLE IF EXISTS `port_orgadminer`;
CREATE TABLE `port_orgadminer`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织',
  `FK_Emp` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '管理员账号',
  `EmpName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '管理员名称',
  `FlowSorts` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '管理的流程目录',
  `FrmTrees` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '管理的表单目录',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_OrgAdminer_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '组织管理员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_orgadminerflowsort
-- ----------------------------
DROP TABLE IF EXISTS `port_orgadminerflowsort`;
CREATE TABLE `port_orgadminerflowsort`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织',
  `FK_Emp` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '管理员',
  `RefOrgAdminer` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织管理员',
  `FlowSortNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程目录',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_OrgAdminerFlowSort_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '组织管理员流程目录权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_orgadminerfrmtree
-- ----------------------------
DROP TABLE IF EXISTS `port_orgadminerfrmtree`;
CREATE TABLE `port_orgadminerfrmtree`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织',
  `FK_Emp` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '管理员',
  `RefOrgAdminer` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织管理员',
  `FrmTreeNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单目录',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_OrgAdminerFrmTree_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单目录权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_station
-- ----------------------------
DROP TABLE IF EXISTS `port_station`;
CREATE TABLE `port_station`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FK_StationType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隶属组织',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_Station_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_station
-- ----------------------------
INSERT INTO `port_station` VALUES ('01', '总经理', '1', NULL, 0);
INSERT INTO `port_station` VALUES ('02', '市场部经理', '2', NULL, 0);
INSERT INTO `port_station` VALUES ('03', '研发部经理', '2', NULL, 0);
INSERT INTO `port_station` VALUES ('04', '客服部经理', '2', NULL, 0);
INSERT INTO `port_station` VALUES ('05', '财务部经理', '2', NULL, 0);
INSERT INTO `port_station` VALUES ('06', '人力资源部经理', '2', NULL, 0);
INSERT INTO `port_station` VALUES ('07', '销售人员岗', '3', NULL, 0);
INSERT INTO `port_station` VALUES ('08', '程序员岗', '3', NULL, 0);
INSERT INTO `port_station` VALUES ('09', '技术服务岗', '3', NULL, 0);
INSERT INTO `port_station` VALUES ('10', '出纳岗', '3', NULL, 0);
INSERT INTO `port_station` VALUES ('11', '人力资源助理岗', '3', NULL, 0);
INSERT INTO `port_station` VALUES ('12', '外来人员岗', '3', NULL, 0);

-- ----------------------------
-- Table structure for port_stationtype
-- ----------------------------
DROP TABLE IF EXISTS `port_stationtype`;
CREATE TABLE `port_stationtype`  (
  `No` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_StationType_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '角色类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_stationtype
-- ----------------------------
INSERT INTO `port_stationtype` VALUES ('1', '高层', 0);
INSERT INTO `port_stationtype` VALUES ('2', '中层', 0);
INSERT INTO `port_stationtype` VALUES ('3', '基层', 0);

-- ----------------------------
-- Table structure for port_team
-- ----------------------------
DROP TABLE IF EXISTS `port_team`;
CREATE TABLE `port_team`  (
  `No` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FK_TeamType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_Team_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '标签' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_teamemp
-- ----------------------------
DROP TABLE IF EXISTS `port_teamemp`;
CREATE TABLE `port_teamemp`  (
  `FK_Team` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '用户组',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  PRIMARY KEY (`FK_Team`, `FK_Emp`) USING BTREE,
  INDEX `Port_TeamEmpID`(`FK_Team`, `FK_Emp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户组人员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_teamtype
-- ----------------------------
DROP TABLE IF EXISTS `port_teamtype`;
CREATE TABLE `port_teamtype`  (
  `No` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_TeamType_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '标签类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for port_token
-- ----------------------------
DROP TABLE IF EXISTS `port_token`;
CREATE TABLE `port_token`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员编号',
  `EmpName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员名称',
  `DeptNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门编号',
  `DeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门名称',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号',
  `OrgName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `SheBei` int(0) NULL DEFAULT 0 COMMENT '0=PC,1=移动',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Port_Token_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '登录记录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of port_token
-- ----------------------------
INSERT INTO `port_token` VALUES ('d89ed722-12fd-4230-88cf-2286be43327e', 'admin', 'admin', '100', '集团总部', '', '', '2023-02-15 11:35', 0);

-- ----------------------------
-- Table structure for port_user
-- ----------------------------
DROP TABLE IF EXISTS `port_user`;
CREATE TABLE `port_user`  (
  `No` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '手机号/ID',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '姓名',
  `Pass` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '密码',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `Token` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Token',
  `Tel` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '电话',
  `Email` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮箱',
  `PinYin` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '拼音',
  `OrgNo` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `OrgName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgName',
  `unionid` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'unionid',
  `OpenID` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '小程序的OpenID',
  `OpenID2` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '公众号的OpenID',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '序号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Port_User_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for pub_ny
-- ----------------------------
DROP TABLE IF EXISTS `pub_ny`;
CREATE TABLE `pub_ny`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Pub_NY_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '月份' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_docfile
-- ----------------------------
DROP TABLE IF EXISTS `sys_docfile`;
CREATE TABLE `sys_docfile`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FileName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FileSize` int(0) NULL DEFAULT 0 COMMENT '大小',
  `FileType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件类型',
  `D1` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'D1',
  `D2` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'D2',
  `D3` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'D3',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_DocFile_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '备注字段文件管理者' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_encfg
-- ----------------------------
DROP TABLE IF EXISTS `sys_encfg`;
CREATE TABLE `sys_encfg`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '实体名称',
  `UIRowStyleGlo` int(0) NULL DEFAULT 0 COMMENT '表格数据行风格(应用全局)',
  `IsEnableDouclickGlo` int(0) NULL DEFAULT 1 COMMENT '是否启动双击打开(应用全局)?',
  `IsEnableFocusField` int(0) NULL DEFAULT 0 COMMENT '是否启用焦点字段?',
  `FocusField` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '焦点字段',
  `IsEnableRefFunc` int(0) NULL DEFAULT 0 COMMENT '是否启用相关功能列?',
  `IsEnableOpenICON` int(0) NULL DEFAULT 0 COMMENT '是否启用打开图标?',
  `IsJM` int(0) NULL DEFAULT 0 COMMENT '是否是加密存储?',
  `IsSelectMore` int(0) NULL DEFAULT 0 COMMENT '是否下拉查询条件多选?',
  `MoveToShowWay` int(0) NULL DEFAULT 0 COMMENT '移动到显示方式',
  `TableCol` int(0) NULL DEFAULT 0 COMMENT '实体表单显示列数',
  `IsShowIcon` int(0) NULL DEFAULT 0 COMMENT '是否显示项目图标',
  `KeyLabel` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关键字Label',
  `KeyPlaceholder` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关键字提示',
  `PageSize` int(0) NULL DEFAULT 10 COMMENT '页面显示的条数',
  `FontSize` int(0) NULL DEFAULT 14 COMMENT '页面字体大小',
  `EditerType` int(0) NULL DEFAULT 0 COMMENT '大块文本编辑器',
  `IsCond` int(0) NULL DEFAULT 0 COMMENT '退出后是否清空查询条件?',
  `OrderBy` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '查询排序字段',
  `IsDeSc` int(0) NULL DEFAULT 1 COMMENT '是否降序排序?',
  `FJSavePath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附加保存路径',
  `FJWebPath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附件Web路径',
  `Datan` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段数据分析方式',
  `UI` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'UI设置',
  `ColorSet` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '颜色设置',
  `FieldSet` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段求和/平均设置',
  `ForamtFunc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段格式化函数',
  `Drill` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据钻取',
  `MobileFieldShowModel` int(0) NULL DEFAULT 0 COMMENT '移动端列表字段显示方式',
  `MobileShowContent` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '移动端列表字段设置',
  `BtnsShowLeft` int(0) NULL DEFAULT 0 COMMENT '按钮显示到左边?',
  `IsImp` int(0) NULL DEFAULT 1 COMMENT '是否显示导入?',
  `ImpFuncUrl` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '导入功能Url',
  `IsExp` int(0) NULL DEFAULT 0 COMMENT '是否显示导出',
  `IsGroup` int(0) NULL DEFAULT 1 COMMENT '是否显示分析按钮（在查询工具栏里）?',
  `IsEnableLazyload` int(0) NULL DEFAULT 1 COMMENT '是否启用懒加载？（对树结构实体有效）?',
  `BtnLab1` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:自定义按钮标签1',
  `BtnJS1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:Url/Javasccript',
  `BtnLab2` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:自定义按钮标签2',
  `BtnJS2` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:Url/Javasccript',
  `BtnLab3` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:自定义按钮标签3',
  `BtnJS3` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '集合:Url/Javasccript',
  `EnBtnLab1` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体:自定义按钮标签1',
  `EnBtnJS1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体:Url/Javasccript',
  `EnBtnLab2` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体:自定义按钮标签2',
  `EnBtnJS2` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体:Url/Javasccript',
  `SearchUrlOpenType` int(0) NULL DEFAULT 0 COMMENT '双击/单击行打开内容',
  `IsRefreshParentPage` int(0) NULL DEFAULT 1 COMMENT '关闭后是否刷新本页面',
  `UrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '要打开的Url',
  `DoubleOrClickModel` int(0) NULL DEFAULT 0 COMMENT '双击/单击行弹窗模式',
  `OpenModel` int(0) NULL DEFAULT 0 COMMENT '打开方式',
  `OpenModelFunc` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '弹窗方法',
  `WinCardW` int(0) NULL DEFAULT 0 COMMENT '宽度',
  `WinCardH` int(0) NULL DEFAULT 2 COMMENT '高度',
  `AtPara` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_EnCfg_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '实体配置' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_enum
-- ----------------------------
DROP TABLE IF EXISTS `sys_enum`;
CREATE TABLE `sys_enum`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Lab` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Lab',
  `EnumKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `IntKey` int(0) NULL DEFAULT 0 COMMENT 'Val',
  `Lang` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '语言',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `StrKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'StrKey',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_Enum_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '枚举数据' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_enum
-- ----------------------------
INSERT INTO `sys_enum` VALUES ('AlertType_CH_0', '短信', 'AlertType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertType_CH_1', '邮件', 'AlertType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertType_CH_2', '邮件与短信', 'AlertType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertType_CH_3', '系统(内部)消息', 'AlertType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_0', '不接收', 'AlertWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_1', '短信', 'AlertWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_2', '邮件', 'AlertWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_3', '内部消息', 'AlertWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_4', 'QQ消息', 'AlertWay', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_5', 'RTX消息', 'AlertWay', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AlertWay_CH_6', 'MSN消息', 'AlertWay', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AllSubFlowOverRole_CH_0', '不处理', 'AllSubFlowOverRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AllSubFlowOverRole_CH_1', '当前流程自动运行下一步', 'AllSubFlowOverRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AllSubFlowOverRole_CH_2', '结束当前流程', 'AllSubFlowOverRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AppType_CH_0', '外部Url连接', 'AppType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AppType_CH_1', '本地可执行文件', 'AppType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_0', 'PK-主键', 'AthCtrlWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_1', 'FID-干流程ID', 'AthCtrlWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_2', 'PWorkID-父流程ID', 'AthCtrlWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_3', '仅能查看自己上传的字段单附件', 'AthCtrlWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_4', 'WorkID-按照WorkID计算(对流程节点表单有效)', 'AthCtrlWay', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_5', 'P2WorkID', 'AthCtrlWay', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthCtrlWay_CH_6', 'P3WorkID', 'AthCtrlWay', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthEditModel_CH_0', '只读', 'AthEditModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthEditModel_CH_1', '可编辑全部区域', 'AthEditModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthEditModel_CH_2', '可编辑非数据标签区域', 'AthEditModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSaveWay_CH_0', '保存到web服务器', 'AthSaveWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSaveWay_CH_1', '保存到数据库', 'AthSaveWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSaveWay_CH_2', 'ftp服务器', 'AthSaveWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSingleRole_CH_0', '不使用模板', 'AthSingleRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSingleRole_CH_1', '使用上传模板', 'AthSingleRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthSingleRole_CH_2', '使用上传模板自动加载数据标签', 'AthSingleRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthUploadWay_CH_0', '继承模式', 'AthUploadWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AthUploadWay_CH_1', '协作模式', 'AthUploadWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AuthorWay_CH_0', '不授权', 'AuthorWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AuthorWay_CH_1', '全部流程授权', 'AuthorWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('AuthorWay_CH_2', '指定流程授权', 'AuthorWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BackCopyRole_CH_0', '不反填', 'BackCopyRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BackCopyRole_CH_1', '字段自动匹配', 'BackCopyRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BackCopyRole_CH_2', '按照设置的格式', 'BackCopyRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BackCopyRole_CH_3', '混合模式', 'BackCopyRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BBSPRI_CH_0', '普通', 'BBSPRI', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BBSPRI_CH_1', '紧急', 'BBSPRI', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BBSPRI_CH_2', '火急', 'BBSPRI', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BBSSta_CH_0', '发布中', 'BBSSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BBSSta_CH_1', '禁用', 'BBSSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillFrmType_CH_0', '傻瓜表单', 'BillFrmType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillFrmType_CH_1', '自由表单', 'BillFrmType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillFrmType_CH_8', '开发者表单', 'BillFrmType', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillState_CH_0', '空白', 'BillState', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillState_CH_1', '草稿', 'BillState', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillState_CH_100', '归档', 'BillState', 100, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillState_CH_2', '编辑中', 'BillState', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillSta_CH_0', '运行中', 'BillSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillSta_CH_1', '已完成', 'BillSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('BillSta_CH_2', '其他', 'BillSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CancelRole_CH_0', '上一步可以撤销', 'CancelRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CancelRole_CH_1', '不能撤销', 'CancelRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CancelRole_CH_2', '上一步与开始节点可以撤销', 'CancelRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CancelRole_CH_3', '指定的节点可以撤销', 'CancelRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRoleExcType_CH_0', '按表单字段计算', 'CCRoleExcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRoleExcType_CH_1', '按人员计算', 'CCRoleExcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRoleExcType_CH_2', '按角色计算', 'CCRoleExcType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRoleExcType_CH_3', '按部门计算', 'CCRoleExcType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRoleExcType_CH_4', '按SQL计算', 'CCRoleExcType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_0', '不能抄送', 'CCRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_1', '手工抄送', 'CCRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_2', '自动抄送', 'CCRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_3', '手工与自动', 'CCRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_4', '按表单SysCCEmps字段计算', 'CCRole', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCRole_CH_5', '在发送前打开抄送窗口', 'CCRole', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_0', '仅按角色计算', 'CCStaWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_1', '按角色智能计算(当前节点)', 'CCStaWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_2', '按角色智能计算(发送到节点)', 'CCStaWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_3', '按角色与部门的交集', 'CCStaWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_4', '按直线上级部门找角色下的人员(当前节点)', 'CCStaWay', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCStaWay_CH_5', '按直线上级部门找角色下的人员(接受节点)', 'CCStaWay', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCSta_CH_0', '未读', 'CCSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCSta_CH_1', '已读', 'CCSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCSta_CH_2', '已回复', 'CCSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCSta_CH_3', '删除', 'CCSta', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCWriteTo_CH_0', '写入抄送列表', 'CCWriteTo', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCWriteTo_CH_1', '写入待办', 'CCWriteTo', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CCWriteTo_CH_2', '写入待办与抄送列表', 'CCWriteTo', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertRole_CH_0', '不提示', 'CHAlertRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertRole_CH_1', '每天1次', 'CHAlertRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertRole_CH_2', '每天2次', 'CHAlertRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertWay_CH_0', '邮件', 'CHAlertWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertWay_CH_1', '短信', 'CHAlertWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHAlertWay_CH_2', 'CCIM即时通讯', 'CHAlertWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ChartType_CH_0', '几何图形', 'ChartType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ChartType_CH_1', '肖像图片', 'ChartType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHRole_CH_0', '禁用', 'CHRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHRole_CH_1', '启用', 'CHRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHRole_CH_2', '只读', 'CHRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHRole_CH_3', '启用并可以调整流程应完成时间', 'CHRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHSta_CH_0', '及时完成', 'CHSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHSta_CH_1', '按期完成', 'CHSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHSta_CH_2', '逾期完成', 'CHSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CHSta_CH_3', '超期完成', 'CHSta', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_1', '跨1个单元格', 'ColSpanAttrDT', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_2', '跨2个单元格', 'ColSpanAttrDT', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_3', '跨3个单元格', 'ColSpanAttrDT', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_4', '跨4个单元格', 'ColSpanAttrDT', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_5', '跨5个单元格', 'ColSpanAttrDT', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrDT_CH_6', '跨6个单元格', 'ColSpanAttrDT', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrString_CH_1', '跨1个单元格', 'ColSpanAttrString', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrString_CH_2', '跨2个单元格', 'ColSpanAttrString', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrString_CH_3', '跨3个单元格', 'ColSpanAttrString', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ColSpanAttrString_CH_4', '跨4个单元格', 'ColSpanAttrString', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CondModel_CH_0', '由连接线条件控制', 'CondModel', 0, 'CH', NULL, NULL);
INSERT INTO `sys_enum` VALUES ('CondModel_CH_1', '按照用户选择计算', 'CondModel', 1, 'CH', NULL, NULL);
INSERT INTO `sys_enum` VALUES ('CondModel_CH_2', '发送按钮旁下拉框选择', 'CondModel', 2, 'CH', NULL, NULL);
INSERT INTO `sys_enum` VALUES ('CtrlEnableType_CH_0', '禁用(隐藏)', 'CtrlEnableType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CtrlEnableType_CH_1', '启用', 'CtrlEnableType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CtrlEnableType_CH_2', '只读', 'CtrlEnableType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CtrlWay_CH_0', '单个', 'CtrlWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CtrlWay_CH_1', '多个', 'CtrlWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('CtrlWay_CH_2', '指定', 'CtrlWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DBListDBType_CH_0', '数据库查询SQL', 'DBListDBType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DBListDBType_CH_1', '执行Url返回Json', 'DBListDBType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DBListDBType_CH_2', '执行存储过程', 'DBListDBType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefaultChart_CH_0', '饼图', 'DefaultChart', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefaultChart_CH_1', '折线图', 'DefaultChart', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefaultChart_CH_2', '柱状图', 'DefaultChart', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefaultChart_CH_3', '显示环形图', 'DefaultChart', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefValType_CH_0', '默认值为空', 'DefValType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DefValType_CH_1', '按照设置的默认值设置', 'DefValType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DelEnable_CH_0', '不能删除', 'DelEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DelEnable_CH_1', '逻辑删除', 'DelEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DelEnable_CH_2', '记录日志方式删除', 'DelEnable', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DelEnable_CH_3', '彻底删除', 'DelEnable', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DelEnable_CH_4', '让用户决定删除方式', 'DelEnable', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DeleteWay_CH_0', '不能删除', 'DeleteWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DeleteWay_CH_1', '删除所有', 'DeleteWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DeleteWay_CH_2', '只能删除自己上传的', 'DeleteWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DocType_CH_0', '正式公文', 'DocType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DocType_CH_1', '便函', 'DocType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DoubleOrClickModel_CH_0', '双击行弹窗', 'DoubleOrClickModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DoubleOrClickModel_CH_1', '单击行弹窗', 'DoubleOrClickModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Draft_CH_0', '无(不设草稿)', 'Draft', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Draft_CH_1', '保存到待办', 'Draft', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Draft_CH_2', '保存到草稿箱', 'Draft', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlAddRecModel_CH_0', '按设置的数量初始化空白行', 'DtlAddRecModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlAddRecModel_CH_1', '用按钮增加空白行', 'DtlAddRecModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlOpenType_CH_0', '操作员', 'DtlOpenType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlOpenType_CH_1', 'WorkID-流程ID', 'DtlOpenType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlOpenType_CH_2', 'FID-干流程ID', 'DtlOpenType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlOpenType_CH_3', 'PWorkID-父流程WorkID', 'DtlOpenType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlSaveModel_CH_0', '自动存盘(失去焦点自动存盘)', 'DtlSaveModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlSaveModel_CH_1', '手动存盘(保存按钮触发存盘)', 'DtlSaveModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DtlVer_CH_0', '2017传统版', 'DtlVer', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSDataWay_CH_0', '不同步', 'DTSDataWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSDataWay_CH_1', '同步全部的相同字段的数据', 'DTSDataWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSDataWay_CH_2', '同步指定字段的数据', 'DTSDataWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSearchWay_CH_0', '不启用', 'DTSearchWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSearchWay_CH_1', '按日期', 'DTSearchWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSearchWay_CH_2', '按日期时间', 'DTSearchWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSWay_CH_0', '不考核', 'DTSWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSWay_CH_1', '按照时效考核', 'DTSWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('DTSWay_CH_2', '按照工作量考核', 'DTSWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditerType_CH_0', '无编辑器', 'EditerType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditerType_CH_1', 'Sina编辑器0', 'EditerType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditerType_CH_2', 'FKEditer', 'EditerType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditerType_CH_3', 'KindEditor', 'EditerType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditerType_CH_4', '百度的UEditor', 'EditerType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditModel_CH_0', '表格模式', 'EditModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditModel_CH_1', '傻瓜表单', 'EditModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EditModel_CH_2', '开发者表单', 'EditModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityEditModel_CH_0', '表格', 'EntityEditModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityEditModel_CH_1', '行编辑', 'EntityEditModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityShowModel_CH_0', '表格', 'EntityShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityShowModel_CH_1', '树干模式', 'EntityShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityType_CH_0', '独立表单', 'EntityType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityType_CH_1', '单据', 'EntityType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityType_CH_2', '编号名称实体', 'EntityType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EntityType_CH_3', '树结构实体', 'EntityType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EnumUIContralType_CH_1', '下拉框', 'EnumUIContralType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EnumUIContralType_CH_2', '复选框', 'EnumUIContralType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EnumUIContralType_CH_3', '单选按钮', 'EnumUIContralType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EventType_CH_0', '禁用', 'EventType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EventType_CH_1', '执行URL', 'EventType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('EventType_CH_2', '执行CCFromRef.js', 'EventType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ExcType_CH_0', '超链接', 'ExcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ExcType_CH_1', '函数', 'ExcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ExpType_CH_3', '按照SQL计算', 'ExpType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ExpType_CH_4', '按照参数计算', 'ExpType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FileType_CH_0', '普通附件', 'FileType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FileType_CH_1', '图片文件', 'FileType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FJOpen_CH_0', '关闭附件', 'FJOpen', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FJOpen_CH_1', '操作员', 'FJOpen', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FJOpen_CH_2', '工作ID', 'FJOpen', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FJOpen_CH_3', '流程ID', 'FJOpen', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowBBSRole_CH_0', '禁用', 'FlowBBSRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowBBSRole_CH_1', '启用', 'FlowBBSRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowBBSRole_CH_2', '只读', 'FlowBBSRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowRunWay_CH_0', '手工启动', 'FlowRunWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowRunWay_CH_1', '指定人员按时启动', 'FlowRunWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowRunWay_CH_2', '数据集按时启动', 'FlowRunWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FlowRunWay_CH_3', '触发式启动', 'FlowRunWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FLRole_CH_0', '按接受人', 'FLRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FLRole_CH_1', '按部门', 'FLRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FLRole_CH_2', '按岗位', 'FLRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmDBRemarkEnable_CH_0', '禁用', 'FrmDBRemarkEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmDBRemarkEnable_CH_1', '可编辑', 'FrmDBRemarkEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmDBRemarkEnable_CH_2', '不可编辑', 'FrmDBRemarkEnable', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmSln_CH_0', '默认方案', 'FrmSln', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmSln_CH_1', '只读方案', 'FrmSln', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmSln_CH_2', '自定义方案', 'FrmSln', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmUrlShowWay_CH_0', '不显示', 'FrmUrlShowWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmUrlShowWay_CH_1', '自动大小', 'FrmUrlShowWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmUrlShowWay_CH_2', '指定大小', 'FrmUrlShowWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FrmUrlShowWay_CH_3', '新窗口', 'FrmUrlShowWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FTCWorkModel_CH_0', '简洁模式', 'FTCWorkModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FTCWorkModel_CH_1', '高级模式', 'FTCWorkModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FuncSrc_CH_0', '自定义', 'FuncSrc', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FuncSrc_CH_1', '系统内置', 'FuncSrc', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCAth_CH_0', '不启用', 'FWCAth', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCAth_CH_1', '多附件', 'FWCAth', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCAth_CH_2', '单附件(暂不支持)', 'FWCAth', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCAth_CH_3', '图片附件(暂不支持)', 'FWCAth', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCMsgShow_CH_0', '都显示', 'FWCMsgShow', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCMsgShow_CH_1', '仅显示自己的意见', 'FWCMsgShow', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCOrderModel_CH_0', '按审批时间先后排序', 'FWCOrderModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCOrderModel_CH_1', '按照接受人员列表先后顺序(官职大小)', 'FWCOrderModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCShowModel_CH_0', '表格方式', 'FWCShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCShowModel_CH_1', '自由模式', 'FWCShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCSta_CH_0', '禁用', 'FWCSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCSta_CH_1', '启用', 'FWCSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCSta_CH_2', '只读', 'FWCSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCType_CH_0', '审核组件', 'FWCType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCType_CH_1', '日志组件', 'FWCType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCType_CH_2', '周报组件', 'FWCType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCType_CH_3', '月报组件', 'FWCType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCVer_CH_0', '1个节点1个人保留1个意见', 'FWCVer', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('FWCVer_CH_1', '保留节点历史意见(默认)', 'FWCVer', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GovDocType_CH_0', 'RTF模板', 'GovDocType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GovDocType_CH_1', 'HTML模板', 'GovDocType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GovDocType_CH_2', 'Weboffice组件', 'GovDocType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GovDocType_CH_3', 'WPS组件', 'GovDocType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GovDocType_CH_4', '金格组件', 'GovDocType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GuestFlowRole_CH_0', '不参与', 'GuestFlowRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GuestFlowRole_CH_1', '开始节点参与', 'GuestFlowRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('GuestFlowRole_CH_2', '中间节点参与', 'GuestFlowRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HelpRole_CH_0', '禁用', 'HelpRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HelpRole_CH_1', '启用', 'HelpRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HelpRole_CH_2', '强制提示', 'HelpRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HelpRole_CH_3', '选择性提示', 'HelpRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianLeaderRole_CH_0', '只有一个组长', 'HuiQianLeaderRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianLeaderRole_CH_1', '最后一个组长发送', 'HuiQianLeaderRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianLeaderRole_CH_2', '任意组长可以发送', 'HuiQianLeaderRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianRole_CH_0', '不启用', 'HuiQianRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianRole_CH_1', '协作(同事)模式', 'HuiQianRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('HuiQianRole_CH_4', '组长(领导)模式', 'HuiQianRole', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IMEnable_CH_0', '禁用', 'IMEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IMEnable_CH_1', '启用', 'IMEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ImgSrcType_CH_0', '本地', 'ImgSrcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ImgSrcType_CH_1', 'URL', 'ImgSrcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ImpModel_CH_0', '不导入', 'ImpModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ImpModel_CH_1', '按配置模式导入', 'ImpModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ImpModel_CH_2', '按照xls文件模版导入', 'ImpModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InfoPRI_CH_0', '普通', 'InfoPRI', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InfoPRI_CH_1', '紧急', 'InfoPRI', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InfoPRI_CH_2', '火急', 'InfoPRI', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InfoSta_CH_0', '发布中', 'InfoSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InfoSta_CH_1', '禁用', 'InfoSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InvokeTime_CH_0', '发送时', 'InvokeTime', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('InvokeTime_CH_1', '工作到达时', 'InvokeTime', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsAutoSendSLSubFlowOver_CH_0', '不处理', 'IsAutoSendSLSubFlowOver', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsAutoSendSLSubFlowOver_CH_1', '让同级子流程自动运行下一步', 'IsAutoSendSLSubFlowOver', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsAutoSendSLSubFlowOver_CH_2', '结束同级子流程', 'IsAutoSendSLSubFlowOver', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsBackTracking_CH_0', '不允许原路返回', 'IsBackTracking', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsBackTracking_CH_1', '由退回人决定', 'IsBackTracking', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsBackTracking_CH_2', '强制原路返回', 'IsBackTracking', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsCanStart_CH_0', '不启用', 'IsCanStart', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsCanStart_CH_1', '独立启动', 'IsCanStart', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsKillEtcThread_CH_0', '不删除其它的子线程', 'IsKillEtcThread', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsKillEtcThread_CH_1', '删除其它的子线程', 'IsKillEtcThread', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsKillEtcThread_CH_2', '由子线程退回人决定是否删除', 'IsKillEtcThread', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSigan_CH_0', '无', 'IsSigan', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSigan_CH_1', '图片签名', 'IsSigan', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSigan_CH_2', '山东CA', 'IsSigan', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSigan_CH_3', '广东CA', 'IsSigan', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSigan_CH_4', '图片盖章', 'IsSigan', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_0', 'yyyy-MM-dd', 'IsSupperText', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_1', 'yyyy-MM-dd HH:mm', 'IsSupperText', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_2', 'yyyy-MM-dd HH:mm:ss', 'IsSupperText', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_3', 'yyyy-MM', 'IsSupperText', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_4', 'HH:mm', 'IsSupperText', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_5', 'HH:mm:ss', 'IsSupperText', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_6', 'MM-dd', 'IsSupperText', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_7', 'yyyy', 'IsSupperText', 7, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('IsSupperText_CH_8', 'MM', 'IsSupperText', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JMCD_CH_0', '一般', 'JMCD', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JMCD_CH_1', '保密', 'JMCD', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JMCD_CH_2', '秘密', 'JMCD', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JMCD_CH_3', '机密', 'JMCD', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JumpWay_CH_0', '不能跳转', 'JumpWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JumpWay_CH_1', '只能向后跳转', 'JumpWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JumpWay_CH_2', '只能向前跳转', 'JumpWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JumpWay_CH_3', '任意节点跳转', 'JumpWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('JumpWay_CH_4', '按指定规则跳转', 'JumpWay', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('KnowledgeSta_CH_0', '公开', 'KnowledgeSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('KnowledgeSta_CH_1', '私有', 'KnowledgeSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('LGType_CH_0', '普通', 'LGType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('LGType_CH_1', '枚举', 'LGType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('LGType_CH_2', '外键', 'LGType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('LGType_CH_3', '打开系统页面', 'LGType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ListModel_CH_0', '编辑模式', 'ListModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ListModel_CH_1', '视图模式', 'ListModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ListShowModel_CH_0', '表格', 'ListShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ListShowModel_CH_1', '卡片', 'ListShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ListShowModel_CH_2', '自定义Url', 'ListShowModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MethodDocTypeOfFunc_CH_0', 'SQL', 'MethodDocTypeOfFunc', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MethodDocTypeOfFunc_CH_1', 'URL', 'MethodDocTypeOfFunc', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MethodDocTypeOfFunc_CH_2', 'JavaScript', 'MethodDocTypeOfFunc', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MethodDocTypeOfFunc_CH_3', '业务单元', 'MethodDocTypeOfFunc', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MobileFieldShowModel_CH_0', '默认设置', 'MobileFieldShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MobileFieldShowModel_CH_1', '设置显示字段', 'MobileFieldShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MobileFieldShowModel_CH_2', '设置模板', 'MobileFieldShowModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MobileShowModel_CH_0', '新页面显示模式', 'MobileShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MobileShowModel_CH_1', '列表模式', 'MobileShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Model_CH_0', '普通', 'Model', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Model_CH_1', '固定行', 'Model', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoreLinkModel_CH_0', '新窗口', 'MoreLinkModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoreLinkModel_CH_1', '本窗口', 'MoreLinkModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoreLinkModel_CH_2', '覆盖新窗口', 'MoreLinkModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoveToShowWay_CH_0', '不显示', 'MoveToShowWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoveToShowWay_CH_1', '下拉列表0', 'MoveToShowWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MoveToShowWay_CH_2', '平铺', 'MoveToShowWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MsgCtrl_CH_0', '不发送', 'MsgCtrl', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MsgCtrl_CH_1', '按设置的下一步接受人自动发送（默认）', 'MsgCtrl', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MsgCtrl_CH_2', '由本节点表单系统字段(IsSendEmail,IsSendSMS)来决定', 'MsgCtrl', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MsgCtrl_CH_3', '由SDK开发者参数(IsSendEmail,IsSendSMS)来决定', 'MsgCtrl', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_1', '字符串String', 'MyDataType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_2', '整数类型Int', 'MyDataType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_3', '浮点类型AppFloat', 'MyDataType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_4', '判断类型Boolean', 'MyDataType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_5', '双精度类型Double', 'MyDataType', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_6', '日期型Date', 'MyDataType', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_7', '时间类型Datetime', 'MyDataType', 7, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDataType_CH_8', '金额类型AppMoney', 'MyDataType', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDeptRole_CH_0', '仅部门领导可以查看', 'MyDeptRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDeptRole_CH_1', '部门下所有的人都可以查看', 'MyDeptRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('MyDeptRole_CH_2', '本部门里指定角色的人可以查看', 'MyDeptRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NextRole_CH_0', '禁用', 'NextRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NextRole_CH_1', '相同节点', 'NextRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NextRole_CH_2', '相同流程', 'NextRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NextRole_CH_3', '相同的人', 'NextRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NextRole_CH_4', '不限流程', 'NextRole', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoGenerModel_CH_0', '自定义', 'NoGenerModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoGenerModel_CH_1', '流水号', 'NoGenerModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoGenerModel_CH_2', '标签的全拼', 'NoGenerModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoGenerModel_CH_3', '标签的简拼', 'NoGenerModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoGenerModel_CH_4', '按GUID生成', 'NoGenerModel', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoteEnable_CH_0', '禁用', 'NoteEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoteEnable_CH_1', '启用', 'NoteEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('NoteEnable_CH_2', '只读', 'NoteEnable', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeBtnEnable_CH_0', '不可用', 'OfficeBtnEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeBtnEnable_CH_1', '可编辑', 'OfficeBtnEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeBtnEnable_CH_2', '不可编辑', 'OfficeBtnEnable', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeBtnLocal_CH_0', '工具栏上', 'OfficeBtnLocal', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeBtnLocal_CH_1', '表单标签(divID=GovDocFile)', 'OfficeBtnLocal', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeFileType_CH_0', 'word文件', 'OfficeFileType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OfficeFileType_CH_1', 'WPS文件', 'OfficeFileType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenModel_CH_0', '弹窗-强制关闭', 'OpenModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenModel_CH_1', '新窗口打开-winopen模式', 'OpenModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenModel_CH_2', '弹窗-非强制关闭', 'OpenModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenModel_CH_3', '执行指定的方法.', 'OpenModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenModel_CH_4', '流程设计器打开模式', 'OpenModel', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenWay_CH_0', '新窗口', 'OpenWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenWay_CH_1', '本窗口', 'OpenWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('OpenWay_CH_2', '覆盖新窗口', 'OpenWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ParentFlowSendNextStepRole_CH_0', '不处理', 'ParentFlowSendNextStepRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ParentFlowSendNextStepRole_CH_1', '该子流程运行结束', 'ParentFlowSendNextStepRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ParentFlowSendNextStepRole_CH_2', '该子流程运行到指定节点', 'ParentFlowSendNextStepRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PicUploadType_CH_0', '拍照上传或者相册上传', 'PicUploadType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PicUploadType_CH_1', '只能拍照上传', 'PicUploadType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PopValFormat_CH_0', 'No(仅编号)', 'PopValFormat', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PopValFormat_CH_1', 'Name(仅名称)', 'PopValFormat', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PopValFormat_CH_2', 'No,Name(编号与名称,比如zhangsan,张三;lisi,李四;)', 'PopValFormat', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PowerCtrlType_CH_0', '角色', 'PowerCtrlType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PowerCtrlType_CH_1', '人员', 'PowerCtrlType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRIEnable_CH_0', '不启用', 'PRIEnable', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRIEnable_CH_1', '只读', 'PRIEnable', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRIEnable_CH_2', '编辑', 'PRIEnable', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PrintOpenModel_CH_0', '下载本地', 'PrintOpenModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PrintOpenModel_CH_1', '在线打开', 'PrintOpenModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PrintPDFModle_CH_0', '全部打印', 'PrintPDFModle', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PrintPDFModle_CH_1', '单个表单打印(针对树形表单)', 'PrintPDFModle', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRI_CH_0', '低', 'PRI', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRI_CH_1', '中', 'PRI', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PRI_CH_2', '高', 'PRI', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_0', '按照指定节点的工作人员', 'PushWay', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_1', '按照指定的工作人员', 'PushWay', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_2', '按照指定的工作角色', 'PushWay', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_3', '按照指定的部门', 'PushWay', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_4', '按照指定的SQL', 'PushWay', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('PushWay_CH_5', '按照系统指定的字段', 'PushWay', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRCodeRole_CH_0', '无', 'QRCodeRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRCodeRole_CH_1', '查看流程表单-无需权限', 'QRCodeRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRCodeRole_CH_2', '查看流程表单-需要登录', 'QRCodeRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRCodeRole_CH_3', '外部账户协作模式处理工作', 'QRCodeRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRModel_CH_0', '不生成', 'QRModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('QRModel_CH_1', '生成二维码', 'QRModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RBShowModel_CH_0', '竖向', 'RBShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RBShowModel_CH_3', '横向', 'RBShowModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadReceipts_CH_0', '不回执', 'ReadReceipts', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadReceipts_CH_1', '自动回执', 'ReadReceipts', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadReceipts_CH_2', '由上一节点表单字段决定', 'ReadReceipts', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadReceipts_CH_3', '由SDK开发者参数决定', 'ReadReceipts', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadRole_CH_0', '不控制', 'ReadRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadRole_CH_1', '未阅读阻止发送', 'ReadRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReadRole_CH_2', '未阅读做记录', 'ReadRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefBillRole_CH_0', '不启用', 'RefBillRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefBillRole_CH_1', '非必须选择关联单据', 'RefBillRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefBillRole_CH_2', '必须选择关联单据', 'RefBillRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefMethodTypeLink_CH_0', '模态窗口打开', 'RefMethodTypeLink', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefMethodTypeLink_CH_1', '新窗口打开', 'RefMethodTypeLink', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefMethodTypeLink_CH_2', '右侧窗口打开', 'RefMethodTypeLink', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefMethodTypeLink_CH_4', '转到新页面', 'RefMethodTypeLink', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefWorkModel_CH_0', '禁用', 'RefWorkModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefWorkModel_CH_1', '静态Html脚本', 'RefWorkModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefWorkModel_CH_2', '静态框架Url', 'RefWorkModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefWorkModel_CH_3', '动态Url', 'RefWorkModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RefWorkModel_CH_4', '动态Html脚本', 'RefWorkModel', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Repeat_CH_0', '永不', 'Repeat', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Repeat_CH_1', '每年', 'Repeat', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Repeat_CH_2', '每月', 'Repeat', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnOneNodeRole_CH_0', '不启用', 'ReturnOneNodeRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnOneNodeRole_CH_1', '按照[退回信息填写字段]作为退回意见直接退回', 'ReturnOneNodeRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnOneNodeRole_CH_2', '按照[审核组件]填写的信息作为退回意见直接退回', 'ReturnOneNodeRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnRole_CH_0', '不能退回', 'ReturnRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnRole_CH_1', '只能退回上一个节点', 'ReturnRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnRole_CH_2', '可以退回任意节点', 'ReturnRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnRole_CH_3', '退回指定的节点', 'ReturnRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnSendModel_CH_0', '从退回节点正常执行', 'ReturnSendModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnSendModel_CH_1', '直接发送到当前节点', 'ReturnSendModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ReturnSendModel_CH_2', '直接发送到当前节点的下一个节点', 'ReturnSendModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenModel_CH_0', '新窗口打开', 'RowOpenModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenModel_CH_1', '弹出窗口打开,关闭后刷新列表', 'RowOpenModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenModel_CH_2', '弹出窗口打开,关闭后不刷新列表', 'RowOpenModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenMode_CH_0', '新窗口打开', 'RowOpenMode', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenMode_CH_1', '在本窗口打开', 'RowOpenMode', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenMode_CH_2', '弹出窗口打开,关闭后不刷新列表', 'RowOpenMode', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowOpenMode_CH_3', '弹出窗口打开,关闭后刷新列表', 'RowOpenMode', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowSpanAttrString_CH_1', '跨1个行', 'RowSpanAttrString', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowSpanAttrString_CH_2', '跨2行', 'RowSpanAttrString', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RowSpanAttrString_CH_3', '跨3行', 'RowSpanAttrString', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Rpt3SumModel_CH_0', '不显示', 'Rpt3SumModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Rpt3SumModel_CH_1', '底部', 'Rpt3SumModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Rpt3SumModel_CH_2', '头部', 'Rpt3SumModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RptModel_CH_0', '左边', 'RptModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('RptModel_CH_1', '顶部', 'RptModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SaveModel_CH_0', '仅节点表', 'SaveModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SaveModel_CH_1', '节点表与Rpt表', 'SaveModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ScripRole_CH_0', '禁用', 'ScripRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ScripRole_CH_1', '按钮启用', 'ScripRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ScripRole_CH_2', '发送启用', 'ScripRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchDictOpenType_CH_0', 'MyDictFrameWork.htm 实体与实体相关功能编辑器', 'SearchDictOpenType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchDictOpenType_CH_1', 'MyDict.htm 实体编辑器', 'SearchDictOpenType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchDictOpenType_CH_2', 'MyBill.htm 单据编辑器', 'SearchDictOpenType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchDictOpenType_CH_9', '自定义URL', 'SearchDictOpenType', 9, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchUrlOpenType_CH_0', 'En.htm 实体与实体相关功能编辑器', 'SearchUrlOpenType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchUrlOpenType_CH_1', 'EnOnly.htm 实体编辑器', 'SearchUrlOpenType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchUrlOpenType_CH_2', '/CCForm/FrmGener.htm 傻瓜表单解析器', 'SearchUrlOpenType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchUrlOpenType_CH_3', '/CCForm/FrmGener.htm 自由表单解析器', 'SearchUrlOpenType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SearchUrlOpenType_CH_9', '自定义URL', 'SearchUrlOpenType', 9, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SendModel_CH_0', '给当前人员设置开始节点待办', 'SendModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SendModel_CH_1', '发送到下一个节点', 'SendModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFOpenType_CH_0', '工作查看器', 'SFOpenType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFOpenType_CH_1', '流程轨迹', 'SFOpenType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFShowCtrl_CH_0', '可以看所有的子流程', 'SFShowCtrl', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFShowCtrl_CH_1', '仅仅可以看自己发起的子流程', 'SFShowCtrl', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFShowModel_CH_0', '表格方式', 'SFShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFShowModel_CH_1', '自由模式', 'SFShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFSta_CH_0', '禁用', 'SFSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFSta_CH_1', '启用', 'SFSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SFSta_CH_2', '只读', 'SFSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SharingType_CH_0', '共享', 'SharingType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SharingType_CH_1', '私有', 'SharingType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ShowModel_CH_0', '按钮', 'ShowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ShowModel_CH_1', '超链接', 'ShowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ShowType_CH_0', '显示', 'ShowType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ShowType_CH_1', 'PC折叠', 'ShowType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ShowType_CH_2', '隐藏', 'ShowType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_0', '不签名', 'SigantureEnabel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_1', '图片签名', 'SigantureEnabel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_2', '写字板', 'SigantureEnabel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_3', '电子签名', 'SigantureEnabel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_4', '电子盖章', 'SigantureEnabel', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SigantureEnabel_CH_5', '电子签名+盖章', 'SigantureEnabel', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SkipTime_CH_0', '上一个节点发送时', 'SkipTime', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SkipTime_CH_1', '当前节点工作打开时', 'SkipTime', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_0', '方向条件', 'SQLType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_1', '接受人规则', 'SQLType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_2', '下拉框数据过滤', 'SQLType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_3', '级联下拉框', 'SQLType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_4', 'PopVal开窗返回值', 'SQLType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SQLType_CH_5', '人员选择器人员选择范围', 'SQLType', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_0', '本地的类', 'SrcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_1', '创建表', 'SrcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_2', '表或视图', 'SrcType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_3', 'SQL查询表', 'SrcType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_4', 'WebServices', 'SrcType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_5', '微服务Handler外部数据源', 'SrcType', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_6', 'JavaScript外部数据源', 'SrcType', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_7', '系统字典表', 'SrcType', 7, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SrcType_CH_8', 'WebApi接口', 'SrcType', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowModel_CH_0', '下级子流程', 'SubFlowModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowModel_CH_1', '同级子流程', 'SubFlowModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowShowType_CH_0', '平铺模式显示', 'SubFlowShowType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowShowType_CH_1', '合并模式显示', 'SubFlowShowType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowSta_CH_0', '禁用', 'SubFlowSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowSta_CH_1', '启用', 'SubFlowSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowSta_CH_2', '只读', 'SubFlowSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowType_CH_0', '手动启动子流程', 'SubFlowType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowType_CH_1', '触发启动子流程', 'SubFlowType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('SubFlowType_CH_2', '延续子流程', 'SubFlowType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TabType_CH_0', '本地表或视图', 'TabType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TabType_CH_1', '通过一个SQL确定的一个外部数据源', 'TabType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TabType_CH_2', '通过WebServices获得的一个数据源', 'TabType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Target_CH_0', '新窗口', 'Target', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Target_CH_1', '本窗口', 'Target', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('Target_CH_2', '父窗口', 'Target', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TaskPRI_CH_0', '高', 'TaskPRI', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TaskPRI_CH_1', '中', 'TaskPRI', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TaskPRI_CH_2', '低', 'TaskPRI', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TaskSta_CH_0', '未完成', 'TaskSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TaskSta_CH_1', '已完成', 'TaskSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TemplateFileModel_CH_0', 'rtf模版', 'TemplateFileModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TemplateFileModel_CH_1', 'VSTO模式的word模版', 'TemplateFileModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TemplateFileModel_CH_2', 'VSTO模式的Excel模版', 'TemplateFileModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TemplateFileModel_CH_3', 'Wps模板', 'TemplateFileModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TextModel_CH_0', '普通文本', 'TextModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TextModel_CH_1', '密码框', 'TextModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TextModel_CH_2', '大文本', 'TextModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TextModel_CH_3', '富文本', 'TextModel', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ThreadKillRole_CH_0', '不能删除', 'ThreadKillRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ThreadKillRole_CH_1', '手工删除', 'ThreadKillRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ThreadKillRole_CH_2', '自动删除', 'ThreadKillRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ToobarExcType_CH_0', '超链接', 'ToobarExcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('ToobarExcType_CH_1', '函数', 'ToobarExcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TrackOrderBy_CH_0', '按照时间先后顺序', 'TrackOrderBy', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TrackOrderBy_CH_1', '倒序(新发生的在前面)', 'TrackOrderBy', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TSpan_CH_0', '本周', 'TSpan', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TSpan_CH_1', '上周', 'TSpan', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TSpan_CH_2', '上上周', 'TSpan', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('TSpan_CH_3', '更早', 'TSpan', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UIRowStyleGlo_CH_0', '无风格', 'UIRowStyleGlo', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UIRowStyleGlo_CH_1', '交替风格', 'UIRowStyleGlo', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UIRowStyleGlo_CH_2', '鼠标移动', 'UIRowStyleGlo', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UIRowStyleGlo_CH_3', '交替并鼠标移动', 'UIRowStyleGlo', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileCheck_CH_0', '不控制', 'UploadFileCheck', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileCheck_CH_1', '上传附件个数不能为0', 'UploadFileCheck', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileCheck_CH_2', '每个类别下面的个数不能为0', 'UploadFileCheck', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileNumCheck_CH_0', '不用校验', 'UploadFileNumCheck', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileNumCheck_CH_1', '不能为空', 'UploadFileNumCheck', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UploadFileNumCheck_CH_2', '每个类别下不能为空', 'UploadFileNumCheck', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UrlSrcType_CH_0', '自定义', 'UrlSrcType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UrlSrcType_CH_1', '地图', 'UrlSrcType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UrlSrcType_CH_2', '流程轨迹表', 'UrlSrcType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('UrlSrcType_CH_3', '流程轨迹图', 'UrlSrcType', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('USSWorkIDRole_CH_0', '仅生成一个WorkID', 'USSWorkIDRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('USSWorkIDRole_CH_1', '按接受人生成WorkID', 'USSWorkIDRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_10', '批处理', 'WFStateApp', 10, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_2', '运行中', 'WFStateApp', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_3', '已完成', 'WFStateApp', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_4', '挂起', 'WFStateApp', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_5', '退回', 'WFStateApp', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_6', '转发', 'WFStateApp', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_7', '删除', 'WFStateApp', 7, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_8', '加签', 'WFStateApp', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFStateApp_CH_9', '冻结', 'WFStateApp', 9, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_0', '空白', 'WFState', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_1', '草稿', 'WFState', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_10', '批处理', 'WFState', 10, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_11', '加签回复状态', 'WFState', 11, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_2', '运行中', 'WFState', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_3', '已完成', 'WFState', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_4', '挂起', 'WFState', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_5', '退回', 'WFState', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_6', '转发', 'WFState', 6, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_7', '删除', 'WFState', 7, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_8', '加签', 'WFState', 8, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFState_CH_9', '冻结', 'WFState', 9, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFSta_CH_0', '运行中', 'WFSta', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFSta_CH_1', '已完成', 'WFSta', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WFSta_CH_2', '其他', 'WFSta', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhatAreYouTodo_CH_0', '关闭提示窗口', 'WhatAreYouTodo', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhatAreYouTodo_CH_1', '关闭提示窗口并刷新', 'WhatAreYouTodo', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhatAreYouTodo_CH_2', '转入到Search.htm页面上去', 'WhatAreYouTodo', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhenOverSize_CH_0', '不处理', 'WhenOverSize', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhenOverSize_CH_1', '向下顺增行', 'WhenOverSize', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhenOverSize_CH_2', '次页显示', 'WhenOverSize', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoExeIt_CH_0', '操作员执行', 'WhoExeIt', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoExeIt_CH_1', '机器执行', 'WhoExeIt', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoExeIt_CH_2', '混合执行', 'WhoExeIt', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_0', 'WorkID是主键', 'WhoIsPK', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_1', 'FID是主键(干流程的WorkID)', 'WhoIsPK', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_2', '父流程ID是主键', 'WhoIsPK', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_3', '延续流程ID是主键', 'WhoIsPK', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_4', 'P2WorkID是主键', 'WhoIsPK', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WhoIsPK_CH_5', 'P3WorkID是主键', 'WhoIsPK', 5, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardH_CH_0', '75%', 'WinCardH', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardH_CH_1', '50%', 'WinCardH', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardH_CH_2', '100%', 'WinCardH', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardH_CH_3', '85%', 'WinCardH', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardH_CH_4', '25%', 'WinCardH', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardW_CH_0', '75%', 'WinCardW', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardW_CH_1', '50%', 'WinCardW', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardW_CH_2', '100%', 'WinCardW', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WinCardW_CH_3', '25%', 'WinCardW', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsDBType_CH_0', '数据库查询SQL', 'WindowsDBType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsDBType_CH_1', '执行Url返回Json', 'WindowsDBType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsDBType_CH_2', '执行\\DataUser\\JSLab\\Windows.js的函数.', 'WindowsDBType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsShowType_CH_0', '饼图', 'WindowsShowType', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsShowType_CH_1', '柱图', 'WindowsShowType', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsShowType_CH_2', '折线图', 'WindowsShowType', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WindowsShowType_CH_4', '简单Table', 'WindowsShowType', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WorkRecModel_CH_0', '日志', 'WorkRecModel', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WorkRecModel_CH_1', '周报', 'WorkRecModel', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('WorkRecModel_CH_2', '月报', 'WorkRecModel', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('YBFlowReturnRole_CH_0', '不能退回', 'YBFlowReturnRole', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('YBFlowReturnRole_CH_1', '退回到父流程的开始节点', 'YBFlowReturnRole', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('YBFlowReturnRole_CH_2', '退回到父流程的任何节点', 'YBFlowReturnRole', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('YBFlowReturnRole_CH_3', '退回父流程的启动节点', 'YBFlowReturnRole', 3, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('YBFlowReturnRole_CH_4', '可退回到指定的节点', 'YBFlowReturnRole', 4, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('傻瓜表单显示方式_CH_0', '4列', '傻瓜表单显示方式', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('傻瓜表单显示方式_CH_1', '6列', '傻瓜表单显示方式', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('傻瓜表单显示方式_CH_2', '上下模式3列', '傻瓜表单显示方式', 2, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('实体表单显示列数_CH_0', '4列', '实体表单显示列数', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('实体表单显示列数_CH_1', '6列', '实体表单显示列数', 1, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('表单展示方式_CH_0', '普通方式', '表单展示方式', 0, 'CH', '', '');
INSERT INTO `sys_enum` VALUES ('表单展示方式_CH_1', '页签方式', '表单展示方式', 1, 'CH', '', '');

-- ----------------------------
-- Table structure for sys_enummain
-- ----------------------------
DROP TABLE IF EXISTS `sys_enummain`;
CREATE TABLE `sys_enummain`  (
  `No` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `CfgVal` varchar(1500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '配置信息',
  `Lang` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '语言',
  `EnumKey` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `IsHaveDtl` int(0) NULL DEFAULT 0 COMMENT '是否有子集?',
  `AtPara` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  `Idx0` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val0` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx1` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val1` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx2` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val2` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx3` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val3` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx4` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val4` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx5` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val5` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx6` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val6` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx7` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val7` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx8` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val8` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx9` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val9` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx10` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val10` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx11` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val11` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx12` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val12` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx13` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val13` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx14` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val14` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx15` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val15` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx16` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val16` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx17` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val17` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx18` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val18` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx19` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val19` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx20` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val20` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx21` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val21` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx22` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val22` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx23` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val23` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx24` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val24` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx25` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val25` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx26` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val26` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx27` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val27` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx28` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val28` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Idx29` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnumKey',
  `Val29` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_EnumMain_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '枚举注册' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_enver
-- ----------------------------
DROP TABLE IF EXISTS `sys_enver`;
CREATE TABLE `sys_enver`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体类',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体名',
  `EnPKValue` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主键值',
  `EnVer` int(0) NULL DEFAULT 0 COMMENT '版本号',
  `RecNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '修改人账号',
  `RecName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '修改人名称',
  `MyNote` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '创建日期',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_EnVer_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '实体版本号' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_enverdtl
-- ----------------------------
DROP TABLE IF EXISTS `sys_enverdtl`;
CREATE TABLE `sys_enverdtl`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `RefPK` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联版本主键',
  `FrmID` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FrmID',
  `EnPKValue` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EnPKValue',
  `AttrKey` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `AttrName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段名',
  `LGType` int(0) NULL DEFAULT 0 COMMENT '逻辑类型',
  `BindKey` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '外部数据源',
  `MyVal` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '数据值',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_EnVerDtl_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '版本明细' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_filemanager
-- ----------------------------
DROP TABLE IF EXISTS `sys_filemanager`;
CREATE TABLE `sys_filemanager`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `AttrFileName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '指定名称',
  `AttrFileNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '指定编号',
  `EnName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联的表',
  `RefVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主键值',
  `WebPath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Web路径',
  `MyFileName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件名称',
  `MyFilePath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFilePath',
  `MyFileExt` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFileExt',
  `MyFileH` int(0) NULL DEFAULT 0 COMMENT 'MyFileH',
  `MyFileW` int(0) NULL DEFAULT 0 COMMENT 'MyFileW',
  `MyFileSize` float NULL DEFAULT NULL COMMENT 'MyFileSize',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '上传时间',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '上传人',
  `Doc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `Sys_FileManager_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '文件管理者' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_formtree
-- ----------------------------
DROP TABLE IF EXISTS `sys_formtree`;
CREATE TABLE `sys_formtree`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ParentNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点编号',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  `OrgNo` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ShortName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '简称',
  `Domain` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '域/系统编号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_FormTree_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单目录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_formtree
-- ----------------------------
INSERT INTO `sys_formtree` VALUES ('1', '表单库', '0', 0, '', '', '');
INSERT INTO `sys_formtree` VALUES ('100', '表单树', '0', 0, '', '', '');
INSERT INTO `sys_formtree` VALUES ('102', '流程独立表单', '0', 0, '', '', '');
INSERT INTO `sys_formtree` VALUES ('104', '常用信息管理', '0', 0, '', '', '');
INSERT INTO `sys_formtree` VALUES ('106', '常用单据', '0', 0, '', '', '');

-- ----------------------------
-- Table structure for sys_frmattachment
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmattachment`;
CREATE TABLE `sys_frmattachment`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `NoOfObj` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附件编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点控制(对sln有效)',
  `AthRunModel` int(0) NULL DEFAULT 0 COMMENT '运行模式',
  `AthSaveWay` int(0) NULL DEFAULT 0 COMMENT '保存方式',
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Exts` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '要求上传的格式',
  `NumOfUpload` int(0) NULL DEFAULT 0 COMMENT '最小上传数量',
  `TopNumOfUpload` int(0) NULL DEFAULT 99 COMMENT '最大上传数量',
  `FileMaxSize` int(0) NULL DEFAULT 10240 COMMENT '附件最大限制(KB)',
  `UploadFileNumCheck` int(0) NULL DEFAULT 0 COMMENT '上传校验方式',
  `Sort` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类别(可为空)',
  `H` float NULL DEFAULT NULL COMMENT 'H',
  `IsUpload` int(0) NULL DEFAULT 1 COMMENT '是否可以上传',
  `IsVisable` int(0) NULL DEFAULT 1 COMMENT '是否可见',
  `FileType` int(0) NULL DEFAULT 0 COMMENT '附件类型',
  `ReadRole` int(0) NULL DEFAULT 0 COMMENT '阅读规则',
  `PicUploadType` int(0) NULL DEFAULT 0 COMMENT '图片附件上传方式',
  `DeleteWay` int(0) NULL DEFAULT 1 COMMENT '附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的',
  `IsDownload` int(0) NULL DEFAULT 1 COMMENT '是否可以下载',
  `IsAutoSize` int(0) NULL DEFAULT 1 COMMENT '自动控制大小',
  `IsNote` int(0) NULL DEFAULT 1 COMMENT '是否增加备注',
  `IsExpCol` int(0) NULL DEFAULT 0 COMMENT '是否启用扩展列',
  `IsShowTitle` int(0) NULL DEFAULT 1 COMMENT '是否显示标题列',
  `UploadType` int(0) NULL DEFAULT 0 COMMENT '上传类型0单个1多个2指定',
  `IsIdx` int(0) NULL DEFAULT 0 COMMENT '是否排序',
  `CtrlWay` int(0) NULL DEFAULT 0 COMMENT '控制呈现控制方式0=PK,1=FID,2=ParentID',
  `AthUploadWay` int(0) NULL DEFAULT 0 COMMENT '控制上传控制方式0=继承模式,1=协作模式.',
  `DataRefNoOfObj` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据引用组件ID',
  `AtPara` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  `GroupID` int(0) NULL DEFAULT 0 COMMENT 'GroupID',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `IsEnableTemplate` int(0) NULL DEFAULT 0 COMMENT '是否启用模板下载?',
  `AthSingleRole` int(0) NULL DEFAULT 0 COMMENT '模板规则',
  `AthEditModel` int(0) NULL DEFAULT 0 COMMENT '在线编辑模式',
  `IsToHeLiuHZ` int(0) NULL DEFAULT 1 COMMENT '该字段单附件是否要汇总到合流节点上去？(对子线程节点有效)',
  `IsHeLiuHuiZong` int(0) NULL DEFAULT 1 COMMENT '是否是合流节点的汇总字段单附件组件？(对合流节点有效)',
  `IsTurn2Html` int(0) NULL DEFAULT 0 COMMENT '是否转换成html(方便手机浏览)',
  `MyFileNum` int(0) NULL DEFAULT 0 COMMENT '附件模板',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmAttachment_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '附件' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmattachmentdb
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmattachmentdb`;
CREATE TABLE `sys_frmattachmentdb`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FK_MapData',
  `FK_FrmAttachment` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附件主键',
  `NoOfObj` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '附件标识',
  `RefPKVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体主键',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `Sort` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类别',
  `FileFullName` varchar(700) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件路径',
  `FileName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FileExts` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '扩展',
  `FileSize` float NULL DEFAULT NULL COMMENT '文件大小',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名字',
  `FK_Dept` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '所在部门',
  `FK_DeptName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '所在部门名称',
  `MyNote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '备注',
  `IsRowLock` int(0) NULL DEFAULT 0 COMMENT '是否锁定行',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '排序',
  `UploadGUID` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '上传GUID',
  `AtPara` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmAttachmentDB_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '附件数据存储' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmbtn
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmbtn`;
CREATE TABLE `sys_frmbtn`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `Lab` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '标签',
  `X` float NULL DEFAULT NULL COMMENT 'X',
  `Y` float NULL DEFAULT NULL COMMENT 'Y',
  `IsView` int(0) NULL DEFAULT 0 COMMENT '是否可见',
  `IsEnable` int(0) NULL DEFAULT 0 COMMENT '是否起用',
  `UAC` int(0) NULL DEFAULT 0 COMMENT '控制类型',
  `UACContext` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '控制内容',
  `EventType` int(0) NULL DEFAULT 0 COMMENT '事件类型',
  `EventContext` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '事件内容',
  `MsgOK` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '运行成功提示',
  `MsgErr` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '运行失败提示',
  `BtnID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '按钮ID',
  `GroupID` int(0) NULL DEFAULT 0 COMMENT '所在分组',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmBtn_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '按钮' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmdbremark
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmdbremark`;
CREATE TABLE `sys_frmdbremark`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `RefPKVal` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'PKVal',
  `Field` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `Remark` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注',
  `RecNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmDBRemark_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '数据批阅' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmdbver
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmdbver`;
CREATE TABLE `sys_frmdbver`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `RefPKVal` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主键值',
  `ChangeFields` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '修改的字段',
  `ChangeNum` int(0) NULL DEFAULT 0 COMMENT '修改的字段数量',
  `TrackID` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TrackID',
  `RecNo` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户名',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  `Ver` int(0) NULL DEFAULT 0 COMMENT '版本号',
  `KeyOfEn` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '章节字段有效',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmDBVer_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '数据版本' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmeledb
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmeledb`;
CREATE TABLE `sys_frmeledb`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FK_MapData',
  `EleID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EleID',
  `RefPKVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'RefPKVal',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `Tag1` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Tag2` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag2',
  `Tag3` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag3',
  `Tag4` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag4',
  `Tag5` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag5',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmEleDB_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单元素扩展DB' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmevent
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmevent`;
CREATE TABLE `sys_frmevent`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EventSource` int(0) NULL DEFAULT 0 COMMENT '事件类型',
  `FK_Event` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '事件标记',
  `RefFlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联的流程编号',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID(包含Dtl表)',
  `FK_Flow` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `EventDoType` int(0) NULL DEFAULT 0 COMMENT '事件执行类型',
  `FK_DBSrc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据源',
  `DoDoc` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '执行内容',
  `MsgOK` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '成功执行提示',
  `MsgError` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '异常信息提示',
  `MsgCtrl` int(0) NULL DEFAULT 0 COMMENT '消息发送控制',
  `MailEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用邮件发送？(如果启用就要设置邮件模版，支持ccflow表达式。)',
  `MailTitle` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮件标题模版',
  `MailDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '邮件内容模版',
  `SMSEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用短信发送？(如果启用就要设置短信模版，支持ccflow表达式。)',
  `SMSDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '短信内容模版',
  `MobilePushEnable` int(0) NULL DEFAULT 1 COMMENT '是否推送到手机、pad端。',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmEvent_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '外部自定义事件(表单,从表,流程,节点)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmimg
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmimg`;
CREATE TABLE `sys_frmimg`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `KeyOfEn` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ImgAppType` int(0) NULL DEFAULT 0 COMMENT '应用类型',
  `UIWidth` float NULL DEFAULT NULL COMMENT 'H',
  `UIHeight` float NULL DEFAULT NULL COMMENT 'H',
  `ImgURL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'ImgURL',
  `ImgPath` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'ImgPath',
  `LinkURL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'LinkURL',
  `LinkTarget` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'LinkTarget',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `Tag0` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参数',
  `ImgSrcType` int(0) NULL DEFAULT 0 COMMENT '图片来源0=本地,1=URL',
  `IsEdit` int(0) NULL DEFAULT 0 COMMENT '是否可以编辑',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '中文名称',
  `EnPK` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '英文名称',
  `ColSpan` int(0) NULL DEFAULT 0 COMMENT '单元格数量',
  `LabelColSpan` int(0) NULL DEFAULT 1 COMMENT '文本单元格数量',
  `RowSpan` int(0) NULL DEFAULT 1 COMMENT '行数',
  `GroupID` int(0) NULL DEFAULT 0 COMMENT '显示的分组',
  `GroupIDText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '显示的分组',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmImg_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '图片' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmimgath
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmimgath`;
CREATE TABLE `sys_frmimgath`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `CtrlID` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控件ID',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '中文名称',
  `IsEdit` int(0) NULL DEFAULT 1 COMMENT '是否可编辑',
  `IsRequired` int(0) NULL DEFAULT 0 COMMENT '是否必填项',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `H` float(11, 2) NULL DEFAULT 200.00 COMMENT 'H',
  `W` float(11, 2) NULL DEFAULT 160.00 COMMENT 'W',
  `GroupID` int(0) NULL DEFAULT 0 COMMENT '显示的分组',
  `GroupIDText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '显示的分组',
  `ColSpan` int(0) NULL DEFAULT 0 COMMENT '单元格数量',
  `LabelColSpan` int(0) NULL DEFAULT 1 COMMENT '文本单元格数量',
  `RowSpan` int(0) NULL DEFAULT 1 COMMENT '行数',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmImgAth_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '图片附件' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmimgathdb
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmimgathdb`;
CREATE TABLE `sys_frmimgathdb`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `FK_FrmImgAth` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图片附件编号',
  `RefPKVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体主键',
  `FileFullName` varchar(700) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件全路径',
  `FileName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FileExts` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '扩展名',
  `FileSize` float NULL DEFAULT NULL COMMENT '文件大小',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `RecName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名字',
  `MyNote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '备注',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmImgAthDB_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '剪切图片附件数据存储' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmprinttemplate
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmprinttemplate`;
CREATE TABLE `sys_frmprinttemplate`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `TempFilePath` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '模板路径',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `FlowNo` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FrmID` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `TemplateFileModel` int(0) NULL DEFAULT 0 COMMENT '模版模式',
  `PrintFileType` int(0) NULL DEFAULT 0 COMMENT '生成的文件类型',
  `PrintOpenModel` int(0) NULL DEFAULT 0 COMMENT '生成的文件打开方式',
  `AthSaveWay` int(0) NULL DEFAULT 0 COMMENT '实例的保存方式',
  `QRModel` int(0) NULL DEFAULT 0 COMMENT '二维码生成方式',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmPrintTemplate_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '打印模板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmrb
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmrb`;
CREATE TABLE `sys_frmrb`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `KeyOfEn` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `EnumKey` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '枚举值',
  `Lab` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `IntKey` int(0) NULL DEFAULT 0 COMMENT 'IntKey',
  `UIIsEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `Script` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '要执行的脚本',
  `FieldsCfg` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '配置信息@FieldName=Sta',
  `SetVal` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '设置的值',
  `Tip` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '选择后提示的信息',
  `AtPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmRB_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '单选框' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_frmsln
-- ----------------------------
DROP TABLE IF EXISTS `sys_frmsln`;
CREATE TABLE `sys_frmsln`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `KeyOfEn` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段名',
  `EleType` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `UIIsEnable` int(0) NULL DEFAULT 1 COMMENT '是否可用',
  `UIVisible` int(0) NULL DEFAULT 1 COMMENT '是否可见',
  `IsSigan` int(0) NULL DEFAULT 0 COMMENT '是否签名',
  `IsNotNull` int(0) NULL DEFAULT 0 COMMENT '是否为空',
  `RegularExp` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '正则表达式',
  `IsWriteToFlowTable` int(0) NULL DEFAULT 0 COMMENT '是否写入流程表',
  `IsWriteToGenerWorkFlow` int(0) NULL DEFAULT 0 COMMENT '是否写入流程注册表',
  `DefVal` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '默认值',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_FrmSln_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单字段方案' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_glovar
-- ----------------------------
DROP TABLE IF EXISTS `sys_glovar`;
CREATE TABLE `sys_glovar`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '键',
  `Name` varchar(120) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `Val` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `GroupKey` varchar(120) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分组值',
  `Note` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '备注',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_GloVar_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '全局变量' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_glovar
-- ----------------------------
INSERT INTO `sys_glovar` VALUES ('0', '选择系统约定默认值', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@FK_ND', '当前年度', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@FK_YF', '当前月份', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.FK_Dept', '登陆人员部门编号', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.FK_DeptFullName', '登陆人员部门全称', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.FK_DeptName', '登陆人员部门名称', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.Name', '登陆人员名称', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.No', '登陆人员账号', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.OrgName', '登录人员组织名称', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@WebUser.OrgNo', '登录人员组织', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@yyyy年MM月dd日', '当前日期(yyyy年MM月dd日)', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@yyyy年MM月dd日HH时mm分', '当前日期(yyyy年MM月dd日HH时mm分)', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@yy年MM月dd日', '当前日期(yy年MM月dd日)', NULL, 'DefVal', NULL, 0);
INSERT INTO `sys_glovar` VALUES ('@yy年MM月dd日HH时mm分', '当前日期(yy年MM月dd日HH时mm分)', NULL, 'DefVal', NULL, 0);

-- ----------------------------
-- Table structure for sys_groupenstemplate
-- ----------------------------
DROP TABLE IF EXISTS `sys_groupenstemplate`;
CREATE TABLE `sys_groupenstemplate`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `EnName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表称',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '报表名',
  `EnsName` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '报表类名',
  `OperateCol` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作属性',
  `Attrs` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '运算属性',
  `Rec` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `Sys_GroupEnsTemplate_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表模板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_groupfield
-- ----------------------------
DROP TABLE IF EXISTS `sys_groupfield`;
CREATE TABLE `sys_groupfield`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `Lab` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `FrmID` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `CtrlType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控件类型',
  `CtrlID` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '控件ID',
  `IsZDMobile` int(0) NULL DEFAULT 0 COMMENT '是否折叠(Mobile)',
  `ShowType` int(0) NULL DEFAULT 0 COMMENT '分组显示模式',
  `Idx` int(0) NULL DEFAULT 99 COMMENT '顺序号',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `AtPara` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `Sys_GroupField_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '傻瓜表单分组' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_groupfield
-- ----------------------------
INSERT INTO `sys_groupfield` VALUES (101, '填写请假申请单', 'ND101', '', '', 0, 0, 1, '', '');
INSERT INTO `sys_groupfield` VALUES (103, '流程信息', 'ND1Rpt', '', '', 0, 0, 2, '', '');
INSERT INTO `sys_groupfield` VALUES (105, '填写请假申请单', 'ND102', '', '', 0, 0, 1, '', '');
INSERT INTO `sys_groupfield` VALUES (107, '部门经理审批', 'ND102', '', '', 0, 0, 2, '', '');
INSERT INTO `sys_groupfield` VALUES (109, '填写请假申请单', 'ND103', '', '', 0, 0, 1, '', '');
INSERT INTO `sys_groupfield` VALUES (111, '部门经理审批', 'ND103', '', '', 0, 0, 2, '', '');
INSERT INTO `sys_groupfield` VALUES (113, '总经理审批', 'ND103', '', '', 0, 0, 3, '', '');
INSERT INTO `sys_groupfield` VALUES (115, '填写请假申请单', 'ND104', '', '', 0, 0, 1, '', '');
INSERT INTO `sys_groupfield` VALUES (117, '部门经理审批', 'ND104', '', '', 0, 0, 2, '', '');
INSERT INTO `sys_groupfield` VALUES (119, '总经理审批', 'ND104', '', '', 0, 0, 3, '', '');

-- ----------------------------
-- Table structure for sys_mapattr
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapattr`;
CREATE TABLE `sys_mapattr`  (
  `MyPK` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体标识',
  `KeyOfEn` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '属性',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DefVal` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `DefValType` int(0) NULL DEFAULT 1 COMMENT '默认值类型',
  `UIContralType` int(0) NULL DEFAULT 0 COMMENT '控件',
  `MyDataType` int(0) NULL DEFAULT 1 COMMENT '数据类型',
  `LGType` int(0) NULL DEFAULT 0 COMMENT '逻辑类型',
  `UIWidth` float NULL DEFAULT NULL COMMENT '宽度',
  `UIHeight` float NULL DEFAULT NULL COMMENT '高度',
  `MinLen` int(0) NULL DEFAULT 0 COMMENT '最小长度',
  `MaxLen` int(0) NULL DEFAULT 300 COMMENT '最大长度',
  `UIBindKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '绑定的信息',
  `UIRefKey` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '绑定的Key',
  `UIRefKeyText` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '绑定的Text',
  `ExtIsSum` int(0) NULL DEFAULT 0 COMMENT '是否显示合计(对从表有效)',
  `UIVisible` int(0) NULL DEFAULT 1 COMMENT '是否可见',
  `UIIsEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `UIIsLine` int(0) NULL DEFAULT 0 COMMENT '是否单独栏显示',
  `UIIsInput` int(0) NULL DEFAULT 0 COMMENT '是否必填字段',
  `TextModel` int(0) NULL DEFAULT 0 COMMENT 'TextModel',
  `IsSupperText` int(0) NULL DEFAULT 0 COMMENT '是否是大文本',
  `FontSize` int(0) NULL DEFAULT 0 COMMENT '字体大小',
  `IsSigan` int(0) NULL DEFAULT 0 COMMENT '签字？',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `EditType` int(0) NULL DEFAULT 0 COMMENT '编辑类型',
  `Tag` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `Tag1` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标识1',
  `Tag2` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Tag3` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标识3',
  `Tip` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `ColSpan` int(0) NULL DEFAULT 1 COMMENT '单元格数量',
  `LabelColSpan` int(0) NULL DEFAULT 1 COMMENT '文本单元格数量',
  `RowSpan` int(0) NULL DEFAULT 1 COMMENT '行数',
  `GroupID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsEnableInAPP` int(0) NULL DEFAULT 1 COMMENT '是否在移动端中显示',
  `CSSCtrl` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'CSSCtrl自定义样式',
  `CSSLabel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'CSSLabel标签样式',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '序号',
  `ICON` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'ICON',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  `GroupIDText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '显示的分组',
  `CSSCtrlText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '自定义样式',
  `MyFileName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '模板',
  `MyFilePath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'MyFilePath',
  `MyFileExt` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'MyFileExt',
  `WebPath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'WebPath',
  `MyFileH` int(0) NULL DEFAULT 0 COMMENT 'MyFileH',
  `MyFileW` int(0) NULL DEFAULT 0 COMMENT 'MyFileW',
  `MyFileSize` float(11, 2) NULL DEFAULT 0.00 COMMENT 'MyFileSize',
  `ExtDefVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '系统默认值',
  `ExtDefValText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '系统默认值',
  `CSSLabelText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '标签样式',
  `DefValText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '默认值(选中)',
  `RBShowModel` int(0) NULL DEFAULT 3 COMMENT '单选按钮的展现方式',
  `NumMin` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '最小值',
  `NumMax` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '最大值',
  `NumStepLength` float(11, 2) NULL DEFAULT 1.00 COMMENT '步长',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_MapAttr_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程进度图' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_mapattr
-- ----------------------------
INSERT INTO `sys_mapattr` VALUES ('ND101_AtPara', 'ND101', 'AtPara', '参数', '', 1, 0, 1, 0, 100, 23, 0, 4000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_CDT', 'ND101', 'CDT', '发起时间', '@RDT', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1001, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_Dao', 'ND101', 'Dao', '到', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 1, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1008, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_Emps', 'ND101', 'Emps', 'Emps', '', 0, 0, 1, 0, 100, 23, 0, 8000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1003, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_FID', 'ND101', 'FID', 'FID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1000, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_FK_Dept', 'ND101', 'FK_Dept', '操作员部门', '', 0, 1, 1, 2, 100, 23, 0, 100, 'BP.Port.Depts', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1004, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_FK_NY', 'ND101', 'FK_NY', '年月', '', 0, 0, 1, 0, 100, 23, 0, 7, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1005, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_OID', 'ND101', 'OID', 'OID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 2, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_QingJiaRiQiCong', 'ND101', 'QingJiaRiQiCong', '请假日期从', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 1, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1007, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_QingJiaTianShu', 'ND101', 'QingJiaTianShu', '请假天数', '0', 0, 0, 3, 0, 100, 23, 0, 50, '', '', '', 0, 1, 1, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1009, '0', '@IsSum=0@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_QingJiaYuanYin', 'ND101', 'QingJiaYuanYin', '请假原因', '', 0, 0, 1, 0, 100, 123, 0, 50, '', '', '', 0, 1, 1, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '101', 1, '0', '0', 1010, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_RDT', 'ND101', 'RDT', '更新时间', '@RDT', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_Rec', 'ND101', 'Rec', '发起人', '@WebUser.No', 0, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1002, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_ShenQingRen', 'ND101', 'ShenQingRen', '申请人', '@WebUser.Name', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 1, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_ShenQingRenBuMen', 'ND101', 'ShenQingRenBuMen', '申请人部门', '@WebUser.FK_DeptName', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 3, 1, 1, '101', 1, '0', '0', 3, '0', '@FontSize=12@IsRichText=0@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_ShenQingRiJi', 'ND101', 'ShenQingRiJi', '申请日期', '@RDT', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', 2, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND101_Title', 'ND101', 'Title', '标题', '', 0, 0, 1, 0, 251, 23, 0, 200, '', '', '', 0, 0, 1, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '101', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_AtPara', 'ND102', 'AtPara', '参数', '', 1, 0, 1, 0, 100, 23, 0, 4000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_BMJLSP_Checker', 'ND102', 'BMJLSP_Checker', '审核人', '@WebUser.No', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '107', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_BMJLSP_Note', 'ND102', 'BMJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 1, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '107', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_BMJLSP_RDT', 'ND102', 'BMJLSP_RDT', '审核日期', '@RDT', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '107', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_CDT', 'ND102', 'CDT', '发起时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1001, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_Dao', 'ND102', 'Dao', '到', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1008, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_Emps', 'ND102', 'Emps', 'Emps', '', 0, 0, 1, 0, 100, 23, 0, 8000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1003, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_FID', 'ND102', 'FID', 'FID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1000, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_FK_Dept', 'ND102', 'FK_Dept', '操作员部门', '', 0, 1, 1, 2, 100, 23, 0, 100, 'BP.Port.Depts', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1004, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_FK_NY', 'ND102', 'FK_NY', '年月', '', 0, 0, 1, 0, 100, 23, 0, 7, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1005, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_OID', 'ND102', 'OID', 'OID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 2, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_QingJiaRiQiCong', 'ND102', 'QingJiaRiQiCong', '请假日期从', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1007, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_QingJiaTianShu', 'ND102', 'QingJiaTianShu', '请假天数', '0', 0, 0, 3, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1009, '0', '@IsSum=0@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_QingJiaYuanYin', 'ND102', 'QingJiaYuanYin', '请假原因', '', 0, 0, 1, 0, 100, 123, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '105', 1, '0', '0', 1010, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_RDT', 'ND102', 'RDT', '更新时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_Rec', 'ND102', 'Rec', '发起人', '', 0, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1002, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_ShenQingRen', 'ND102', 'ShenQingRen', '申请人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 1, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_ShenQingRenBuMen', 'ND102', 'ShenQingRenBuMen', '申请人部门', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 3, 1, 1, '105', 1, '0', '0', 3, '0', '@FontSize=12@IsRichText=0@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_ShenQingRiJi', 'ND102', 'ShenQingRiJi', '申请日期', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', 2, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND102_Title', 'ND102', 'Title', '标题', '', 0, 0, 1, 0, 251, 23, 0, 200, '', '', '', 0, 0, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '105', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_AtPara', 'ND103', 'AtPara', '参数', '', 1, 0, 1, 0, 100, 23, 0, 4000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_BMJLSP_Checker', 'ND103', 'BMJLSP_Checker', '审核人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '111', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_BMJLSP_Note', 'ND103', 'BMJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '111', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_BMJLSP_RDT', 'ND103', 'BMJLSP_RDT', '审核日期', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '111', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_CDT', 'ND103', 'CDT', '发起时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1001, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_Dao', 'ND103', 'Dao', '到', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1008, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_Emps', 'ND103', 'Emps', 'Emps', '', 0, 0, 1, 0, 100, 23, 0, 8000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1003, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_FID', 'ND103', 'FID', 'FID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1000, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_FK_Dept', 'ND103', 'FK_Dept', '操作员部门', '', 0, 1, 1, 2, 100, 23, 0, 100, 'BP.Port.Depts', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1004, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_FK_NY', 'ND103', 'FK_NY', '年月', '', 0, 0, 1, 0, 100, 23, 0, 7, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1005, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_OID', 'ND103', 'OID', 'OID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 2, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_QingJiaRiQiCong', 'ND103', 'QingJiaRiQiCong', '请假日期从', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1007, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_QingJiaTianShu', 'ND103', 'QingJiaTianShu', '请假天数', '0', 0, 0, 3, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1009, '0', '@IsSum=0@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_QingJiaYuanYin', 'ND103', 'QingJiaYuanYin', '请假原因', '', 0, 0, 1, 0, 100, 123, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '109', 1, '0', '0', 1010, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_RDT', 'ND103', 'RDT', '更新时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_Rec', 'ND103', 'Rec', '发起人', '', 0, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1002, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ShenQingRen', 'ND103', 'ShenQingRen', '申请人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 1, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ShenQingRenBuMen', 'ND103', 'ShenQingRenBuMen', '申请人部门', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 3, 1, 1, '109', 1, '0', '0', 3, '0', '@FontSize=12@IsRichText=0@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ShenQingRiJi', 'ND103', 'ShenQingRiJi', '申请日期', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', 2, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_Title', 'ND103', 'Title', '标题', '', 0, 0, 1, 0, 251, 23, 0, 200, '', '', '', 0, 0, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '109', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ZJLSP_Checker', 'ND103', 'ZJLSP_Checker', '审核人', '@WebUser.No', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '113', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ZJLSP_Note', 'ND103', 'ZJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 1, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '113', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND103_ZJLSP_RDT', 'ND103', 'ZJLSP_RDT', '审核日期', '@RDT', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '113', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_AtPara', 'ND104', 'AtPara', '参数', '', 1, 0, 1, 0, 100, 23, 0, 4000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_BMJLSP_Checker', 'ND104', 'BMJLSP_Checker', '审核人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '117', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_BMJLSP_Note', 'ND104', 'BMJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '117', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_BMJLSP_RDT', 'ND104', 'BMJLSP_RDT', '审核日期', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '117', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_CDT', 'ND104', 'CDT', '发起时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1001, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_Dao', 'ND104', 'Dao', '到', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1008, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_Emps', 'ND104', 'Emps', 'Emps', '', 0, 0, 1, 0, 100, 23, 0, 8000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1003, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_FID', 'ND104', 'FID', 'FID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1000, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_FK_Dept', 'ND104', 'FK_Dept', '操作员部门', '', 0, 1, 1, 2, 100, 23, 0, 100, 'BP.Port.Depts', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1004, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_FK_NY', 'ND104', 'FK_NY', '年月', '', 0, 0, 1, 0, 100, 23, 0, 7, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1005, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_OID', 'ND104', 'OID', 'OID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 2, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_QingJiaRiQiCong', 'ND104', 'QingJiaRiQiCong', '请假日期从', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1007, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_QingJiaTianShu', 'ND104', 'QingJiaTianShu', '请假天数', '0', 0, 0, 3, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1009, '0', '@IsSum=0@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_QingJiaYuanYin', 'ND104', 'QingJiaYuanYin', '请假原因', '', 0, 0, 1, 0, 100, 123, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '115', 1, '0', '0', 1010, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_RDT', 'ND104', 'RDT', '更新时间', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_Rec', 'ND104', 'Rec', '发起人', '', 0, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1002, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ShenQingRen', 'ND104', 'ShenQingRen', '申请人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 1, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ShenQingRenBuMen', 'ND104', 'ShenQingRenBuMen', '申请人部门', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 3, 1, 1, '115', 1, '0', '0', 3, '0', '@FontSize=12@IsRichText=0@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ShenQingRiJi', 'ND104', 'ShenQingRiJi', '申请日期', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', 2, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_Title', 'ND104', 'Title', '标题', '', 0, 0, 1, 0, 251, 23, 0, 200, '', '', '', 0, 0, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '115', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ZJLSP_Checker', 'ND104', 'ZJLSP_Checker', '审核人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '119', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ZJLSP_Note', 'ND104', 'ZJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '119', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND104_ZJLSP_RDT', 'ND104', 'ZJLSP_RDT', '审核日期', '', 0, 0, 7, 0, 145, 23, 0, 20, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '119', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_AtPara', 'ND1Rpt', 'AtPara', '参数', '', 0, 0, 1, 0, 100, 23, 0, 4000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_BillNo', 'ND1Rpt', 'BillNo', '单据编号', '', 1, 0, 1, 0, 100, 23, 0, 100, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_BMJLSP_Checker', 'ND1Rpt', 'BMJLSP_Checker', '审核人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '107', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_BMJLSP_Note', 'ND1Rpt', 'BMJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '107', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_BMJLSP_RDT', 'ND1Rpt', 'BMJLSP_RDT', '审核日期', '', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '107', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_CDT', 'ND1Rpt', 'CDT', '活动时间', '', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1001, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_Dao', 'ND1Rpt', 'Dao', '到', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1008, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_Emps', 'ND1Rpt', 'Emps', '参与者', '', 0, 0, 1, 0, 100, 23, 0, 8000, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1003, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FID', 'ND1Rpt', 'FID', 'FID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1000, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FK_Dept', 'ND1Rpt', 'FK_Dept', '操作员部门', '', 0, 0, 1, 0, 100, 23, 0, 100, 'BP.Port.Depts', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', 1004, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FK_NY', 'ND1Rpt', 'FK_NY', '年月', '', 0, 0, 1, 0, 100, 23, 0, 7, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', 1005, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowDaySpan', 'ND1Rpt', 'FlowDaySpan', '流程时长(天)', '', 1, 0, 3, 0, 100, 23, 0, 300, '', '', '', 0, 1, 1, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowEmps', 'ND1Rpt', 'FlowEmps', '参与人', '', 1, 0, 1, 0, 100, 23, 0, 1000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowEnder', 'ND1Rpt', 'FlowEnder', '结束人', '', 1, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowEnderRDT', 'ND1Rpt', 'FlowEnderRDT', '结束时间', '', 1, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowEndNode', 'ND1Rpt', 'FlowEndNode', '结束节点', '', 1, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowStarter', 'ND1Rpt', 'FlowStarter', '发起人', '', 1, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_FlowStartRDT', 'ND1Rpt', 'FlowStartRDT', '发起时间', '', 1, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_GUID', 'ND1Rpt', 'GUID', 'GUID', '', 1, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_OID', 'ND1Rpt', 'OID', 'OID', '0', 0, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 2, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PEmp', 'ND1Rpt', 'PEmp', '调起子流程的人员', '', 1, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PFlowNo', 'ND1Rpt', 'PFlowNo', '父流程编号', '', 1, 0, 1, 0, 100, 23, 0, 100, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PNodeID', 'ND1Rpt', 'PNodeID', '父流程启动的节点', '', 1, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PrjName', 'ND1Rpt', 'PrjName', '项目名称', '', 1, 0, 1, 0, 100, 23, 0, 100, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PrjNo', 'ND1Rpt', 'PrjNo', '项目编号', '', 1, 0, 1, 0, 100, 23, 0, 100, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -100, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_PWorkID', 'ND1Rpt', 'PWorkID', '父流程WorkID', '', 1, 0, 2, 0, 100, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -101, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_QingJiaRiQiCong', 'ND1Rpt', 'QingJiaRiQiCong', '请假日期从', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1007, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_QingJiaTianShu', 'ND1Rpt', 'QingJiaTianShu', '请假天数', '0', 0, 0, 3, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1009, '0', '@IsSum=0@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_QingJiaYuanYin', 'ND1Rpt', 'QingJiaYuanYin', '请假原因', '', 0, 0, 1, 0, 100, 123, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '100', 1, '0', '0', 1010, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_RDT', 'ND1Rpt', 'RDT', '更新时间', '', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 0, 0, 0, 0, 0, 1, 0, 0, '', 1, '1', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 999, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_Rec', 'ND1Rpt', 'Rec', '发起人', '', 0, 0, 1, 0, 100, 23, 0, 32, '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1002, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ShenQingRen', 'ND1Rpt', 'ShenQingRen', '申请人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 1, '0', '@IsRichText=0@FontSize=12@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ShenQingRenBuMen', 'ND1Rpt', 'ShenQingRenBuMen', '申请人部门', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 3, 1, 1, '100', 1, '0', '0', 3, '0', '@FontSize=12@IsRichText=0@IsSupperText=0', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ShenQingRiJi', 'ND1Rpt', 'ShenQingRiJi', '申请日期', '', 0, 0, 6, 0, 125, 23, 0, 10, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', 2, '0', '@FontSize=12', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_Title', 'ND1Rpt', 'Title', '标题', '', 0, 0, 1, 0, 251, 23, 0, 200, '', '', '', 0, 0, 0, 1, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '100', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_WFSta', 'ND1Rpt', 'WFSta', '状态', '', 1, 1, 2, 1, 100, 23, 0, 1000, 'WFSta', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_WFState', 'ND1Rpt', 'WFState', '流程状态', '', 1, 1, 2, 1, 100, 23, 0, 1000, 'WFState', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 1, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', -1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ZJLSP_Checker', 'ND1Rpt', 'ZJLSP_Checker', '审核人', '', 0, 0, 1, 0, 100, 23, 0, 50, '', '', '', 0, 1, 0, 0, 0, 0, 0, 0, 1, '', 0, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', 2, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ZJLSP_Note', 'ND1Rpt', 'ZJLSP_Note', '审核意见', '', 0, 0, 1, 0, 100, 69, 0, 4000, '', '', '', 0, 1, 0, 1, 0, 0, 0, 0, 0, '', 0, '', '', '', '', '', 4, 1, 1, '103', 1, '0', '0', 1, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);
INSERT INTO `sys_mapattr` VALUES ('ND1Rpt_ZJLSP_RDT', 'ND1Rpt', 'ZJLSP_RDT', '审核日期', '', 0, 0, 7, 0, 145, 23, 0, 300, '', '', '', 0, 1, 0, 0, 0, 0, 1, 0, 0, '', 0, '', '', '', '', '', 1, 1, 1, '103', 1, '0', '0', 3, '0', '', '0', '0', '', '', '', '', 0, 0, 0.00, '0', '0', '0', '0', 3, '', '', 1.00);

-- ----------------------------
-- Table structure for sys_mapdata
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapdata`;
CREATE TABLE `sys_mapdata`  (
  `No` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '描述',
  `FormEventEntity` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '事件实体',
  `EnPK` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体主键',
  `PTable` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '物理表',
  `PTableModel` int(0) NULL DEFAULT 0 COMMENT '表存储模式',
  `UrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '连接(对嵌入式表单有效)',
  `Dtls` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '从表',
  `FrmW` int(0) NULL DEFAULT 900 COMMENT 'FrmW',
  `TableCol` int(0) NULL DEFAULT 0 COMMENT '傻瓜表单显示的列',
  `Tag` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag',
  `FK_FormTree` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单树类别',
  `FrmType` int(0) NULL DEFAULT 0 COMMENT '表单类型',
  `FrmShowType` int(0) NULL DEFAULT 0 COMMENT '表单展示方式',
  `EntityType` int(0) NULL DEFAULT 0 COMMENT '业务类型',
  `IsEnableJs` int(0) NULL DEFAULT 0 COMMENT '是否启用自定义js函数？',
  `AppType` int(0) NULL DEFAULT 0 COMMENT '应用类型',
  `DBSrc` varchar(600) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `BodyAttr` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单Body属性',
  `Note` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `Designer` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '设计者',
  `DesignerUnit` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单位',
  `DesignerContact` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '联系方式',
  `Idx` int(0) NULL DEFAULT 100 COMMENT '顺序号',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `Ver` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '版本号',
  `Icon` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `FlowCtrls` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程控件',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `IsTemplate` int(0) NULL DEFAULT 0 COMMENT '是否是表单模版',
  `DBType` int(0) NULL DEFAULT 0 COMMENT '数据源类型',
  `ExpEn` varchar(600) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '实体数据源',
  `ExpList` varchar(600) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '列表数据源',
  `MainTable` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '列表数据源主表',
  `MainTablePK` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '列表数据源主表主键',
  `ExpCount` varchar(600) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '列表总数',
  `RowOpenModel` int(0) NULL DEFAULT 2 COMMENT '行记录打开模式',
  `SearchDictOpenType` int(0) NULL DEFAULT 0 COMMENT '双击行打开内容',
  `PopHeight` int(0) NULL DEFAULT 500 COMMENT '弹窗高度',
  `PopWidth` int(0) NULL DEFAULT 760 COMMENT '弹窗宽度',
  `EntityEditModel` int(0) NULL DEFAULT 0 COMMENT '编辑模式',
  `BillNoFormat` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '实体编号规则',
  `SortColumns` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '排序字段',
  `ColorSet` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '颜色设置',
  `FieldSet` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '字段求和求平均设置',
  `ForamtFunc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '字段格式化函数',
  `IsSelectMore` int(0) NULL DEFAULT 1 COMMENT '是否下拉查询条件多选?',
  `TitleRole` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '标题生成规则',
  `RowColorSet` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '表格行颜色设置',
  `RefDict` varchar(190) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '单据关联的实体',
  `BtnRefBill` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '关联单据' COMMENT '关联单据',
  `RefBillRole` int(0) NULL DEFAULT 0 COMMENT '关联单据工作模式',
  `RefBill` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '关联单据ID',
  `Tag0` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Tag0',
  `Tag1` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'Tag1',
  `Tag2` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Tag2',
  `EntityShowModel` int(0) NULL DEFAULT 0 COMMENT '展示模式',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '独立表单属性:FK_Flow',
  `DBURL` int(0) NULL DEFAULT 0 COMMENT 'DBURL',
  `URL` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Url',
  `TemplaterVer` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '模版编号',
  `DBSave` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Excel数据文件存储',
  `MyFileName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '表单模版',
  `MyFilePath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'MyFilePath',
  `MyFileExt` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'MyFileExt',
  `WebPath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'WebPath',
  `MyFileH` int(0) NULL DEFAULT 0 COMMENT 'MyFileH',
  `MyFileW` int(0) NULL DEFAULT 0 COMMENT 'MyFileW',
  `MyFileSize` float(11, 2) NULL DEFAULT 0.00 COMMENT 'MyFileSize',
  `RefWorkModel` int(0) NULL DEFAULT 0 COMMENT '工作模式',
  `RefBlurField` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '失去焦点字段',
  `RefUrl` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '连接',
  `RefHtml` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '静态Html脚本',
  `RightViewWay` int(0) NULL DEFAULT 0 COMMENT '报表查看权限控制方式',
  `RightViewTag` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '报表查看权限控制Tag',
  `RightDeptWay` int(0) NULL DEFAULT 0 COMMENT '部门数据查看控制方式',
  `RightDeptTag` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '部门数据查看控制Tag',
  `HtmlTemplateFile` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_MapData_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '傻瓜表单属性' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_mapdata
-- ----------------------------
INSERT INTO `sys_mapdata` VALUES ('ND101', '填写请假申请单', '', '', 'ND1Rpt', 0, '', '', 900, 0, '', '', 0, 0, 0, 0, 0, 'local', '', '', '', '', '', 100, '', '2020-04-05 18:22:56', '', '', '@IsHaveCA=0@Ver=1@MapDtls_AutoNum=0', '', 0, 0, '', '', '', '', '', 2, 0, 500, 760, 0, '', '', '', '', '', 1, '', '', '', '关联单据', 0, '', '', NULL, '', 0, '', 0, '', '', '', '', '', '', '', 0, 0, 0.00, 0, '', '', NULL, 0, NULL, 0, NULL, NULL);
INSERT INTO `sys_mapdata` VALUES ('ND102', '部门经理审批', '', '', 'ND1Rpt', 0, '', '', 900, 0, '', '', 0, 0, 0, 0, 0, 'local', '', '', '', '', '', 100, '', '2020-04-05 20:44:37', '', '', '@IsHaveCA=0@Ver=1', '', 0, 0, '', '', '', '', '', 2, 0, 500, 760, 0, '', '', '', '', '', 1, '', '', '', '关联单据', 0, '', '', NULL, '', 0, '', 0, '', '', '', '', '', '', '', 0, 0, 0.00, 0, '', '', NULL, 0, NULL, 0, NULL, NULL);
INSERT INTO `sys_mapdata` VALUES ('ND103', '总经理审批', '', '', 'ND1Rpt', 0, '', '', 900, 0, '', '', 0, 0, 0, 0, 0, 'local', '', '', '', '', '', 100, '', '2020-04-05 20:45:03', '', '', '@IsHaveCA=0@Ver=1', '', 0, 0, '', '', '', '', '', 2, 0, 500, 760, 0, '', '', '', '', '', 1, '', '', '', '关联单据', 0, '', '', NULL, '', 0, '', 0, '', '', '', '', '', '', '', 0, 0, 0.00, 0, '', '', NULL, 0, NULL, 0, NULL, NULL);
INSERT INTO `sys_mapdata` VALUES ('ND104', '反馈给申请人', '', '', 'ND1Rpt', 0, '', '', 900, 0, '', '', 0, 0, 0, 0, 0, 'local', '', '', '', '', '', 100, '', '2020-04-05 20:45:19', '', '', '@IsHaveCA=0@Ver=1@MapDtls_AutoNum=0', '', 0, 0, '', '', '', '', '', 2, 0, 500, 760, 0, '', '', '', '', '', 1, '', '', '', '关联单据', 0, '', '', NULL, '', 0, '', 0, '', '', '', '', '', '', '', 0, 0, 0.00, 0, '', '', NULL, 0, NULL, 0, NULL, NULL);
INSERT INTO `sys_mapdata` VALUES ('ND1Rpt', '请假流程-经典表单-演示', '', '', 'ND1Rpt', 0, '', '', 900, 0, '', '', 1, 0, 0, 0, 0, 'local', '', '', '', '', '', 100, '', '2020-04-05 20:52:19', '', '', '@IsHaveCA=0@Ver=1@MapDtls_AutoNum=0', '', 0, 0, '', '', '', '', '', 2, 0, 500, 760, 0, '', '', '', '', '', 1, '', '', '', '关联单据', 0, '', '', NULL, '', 0, '', 0, '', '', '', '', '', '', '', 0, 0, 0.00, 0, '', '', NULL, 0, NULL, 0, NULL, NULL);

-- ----------------------------
-- Table structure for sys_mapdataver
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapdataver`;
CREATE TABLE `sys_mapdataver`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Ver` int(0) NULL DEFAULT 0 COMMENT '版本号',
  `IsRel` int(0) NULL DEFAULT 0 COMMENT '是否主版本?',
  `RowNumExt` int(0) NULL DEFAULT 0 COMMENT '行数',
  `FrmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `AttrsNum` int(0) NULL DEFAULT 0 COMMENT '字段数',
  `DtlsNum` int(0) NULL DEFAULT 0 COMMENT '从表数',
  `AthsNum` int(0) NULL DEFAULT 0 COMMENT '附件数',
  `ExtsNum` int(0) NULL DEFAULT 0 COMMENT '逻辑数',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人ID',
  `RecName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人名称',
  `RecNote` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_MapDataVer_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单模板版本管理' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_mapdtl
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapdtl`;
CREATE TABLE `sys_mapdtl`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Alias` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '别名',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主表',
  `PTable` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '物理表',
  `GroupField` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分组字段',
  `RefPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联的主键',
  `FEBD` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '映射的事件实体类',
  `Model` int(0) NULL DEFAULT 0 COMMENT '工作模式',
  `DtlVer` int(0) NULL DEFAULT 0 COMMENT '使用版本',
  `RowsOfList` int(0) NULL DEFAULT 6 COMMENT '初始化行数',
  `IsEnableGroupField` int(0) NULL DEFAULT 0 COMMENT '是否启用分组字段',
  `IsShowSum` int(0) NULL DEFAULT 1 COMMENT '是否显示合计？',
  `IsShowIdx` int(0) NULL DEFAULT 1 COMMENT '是否显示序号？',
  `IsCopyNDData` int(0) NULL DEFAULT 1 COMMENT '是否允许Copy数据',
  `IsHLDtl` int(0) NULL DEFAULT 0 COMMENT '是否是合流汇总',
  `IsReadonly` int(0) NULL DEFAULT 0 COMMENT '是否只读？',
  `IsShowTitle` int(0) NULL DEFAULT 1 COMMENT '是否显示标题？',
  `IsView` int(0) NULL DEFAULT 1 COMMENT '是否可见',
  `IsInsert` int(0) NULL DEFAULT 1 COMMENT '是否可以插入行？',
  `IsDelete` int(0) NULL DEFAULT 1 COMMENT '是否可以删除行',
  `IsUpdate` int(0) NULL DEFAULT 1 COMMENT '是否可以更新？',
  `IsEnablePass` int(0) NULL DEFAULT 0 COMMENT '是否启用通过审核功能?',
  `IsEnableAthM` int(0) NULL DEFAULT 0 COMMENT '是否启用多附件',
  `IsCopyFirstData` int(0) NULL DEFAULT 0 COMMENT '是否复制第一行数据？',
  `InitDBAttrs` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '行初始化字段',
  `WhenOverSize` int(0) NULL DEFAULT 0 COMMENT '列表数据显示格式',
  `DtlOpenType` int(0) NULL DEFAULT 1 COMMENT '数据开放类型',
  `ListShowModel` int(0) NULL DEFAULT 0 COMMENT '列表数据显示格式',
  `EditModel` int(0) NULL DEFAULT 0 COMMENT '行数据显示格式',
  `UrlDtl` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '自定义Url',
  `ColAutoExp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '列字段计算',
  `MobileShowModel` int(0) NULL DEFAULT 0 COMMENT '移动端数据显示格式',
  `MobileShowField` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '移动端列表显示字段',
  `H` float NULL DEFAULT NULL COMMENT '高度',
  `FrmW` float NULL DEFAULT NULL COMMENT '表单宽度',
  `FrmH` float NULL DEFAULT NULL COMMENT '表单高度',
  `NumOfDtl` int(0) NULL DEFAULT 0 COMMENT '最小从表集合',
  `IsEnableLink` int(0) NULL DEFAULT 0 COMMENT '是否启用超链接',
  `LinkLabel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '超连接标签',
  `LinkTarget` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '连接目标',
  `LinkUrl` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '连接URL',
  `FilterSQLExp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `OrderBySQLExp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点(用户独立表单权限控制)',
  `ShowCols` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '显示的列',
  `IsExp` int(0) NULL DEFAULT 1 COMMENT 'IsExp',
  `ImpModel` int(0) NULL DEFAULT 0 COMMENT '导入规则',
  `ImpSQLSearch` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `ImpSQLInit` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `ImpSQLFullOneRow` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `ImpSQLNames` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段中文名',
  `IsImp` int(0) NULL DEFAULT 0 COMMENT 'IsImp',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `AtPara` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  `ExcType` int(0) NULL DEFAULT 0 COMMENT '执行类型',
  `IsEnableLink2` int(0) NULL DEFAULT 0 COMMENT '相关功能2',
  `LinkLabel2` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '超连接/功能标签',
  `ExcType2` int(0) NULL DEFAULT 0 COMMENT '执行类型',
  `LinkTarget2` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'LinkTarget',
  `LinkUrl2` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '连接/函数',
  `SubThreadWorker` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '子线程处理人字段',
  `SubThreadWorkerText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '子线程处理人字段',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_MapDtl_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '明细' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_mapext
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapext`;
CREATE TABLE `sys_mapext`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `ExtModel` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型1',
  `ExtType` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型2',
  `DoWay` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '执行方式',
  `AttrOfOper` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作的Attr',
  `AttrsOfActive` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '激活的字段',
  `Doc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `Tag` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag',
  `Tag1` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Tag2` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag2',
  `Tag3` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag3',
  `Tag4` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag4',
  `Tag5` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag5',
  `H` int(0) NULL DEFAULT 500 COMMENT '高度',
  `W` int(0) NULL DEFAULT 400 COMMENT '宽度',
  `DBType` int(0) NULL DEFAULT 0 COMMENT '数据类型',
  `FK_DBSrc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据源',
  `PRI` int(0) NULL DEFAULT 0 COMMENT 'PRI/顺序号',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参数',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_MapExt_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '业务逻辑' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_mapframe
-- ----------------------------
DROP TABLE IF EXISTS `sys_mapframe`;
CREATE TABLE `sys_mapframe`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `UrlSrcType` int(0) NULL DEFAULT 0 COMMENT 'URL来源',
  `FrameURL` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FrameURL',
  `URL` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'URL',
  `W` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '宽度',
  `H` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '高度',
  `IsAutoSize` int(0) NULL DEFAULT 1 COMMENT '是否自动设置大小',
  `EleType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `GUID` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_MapFrame_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '框架' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_rptdept
-- ----------------------------
DROP TABLE IF EXISTS `sys_rptdept`;
CREATE TABLE `sys_rptdept`  (
  `FK_Rpt` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '部门',
  PRIMARY KEY (`FK_Rpt`, `FK_Dept`) USING BTREE,
  INDEX `Sys_RptDeptID`(`FK_Rpt`, `FK_Dept`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表部门对应信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_rptemp
-- ----------------------------
DROP TABLE IF EXISTS `sys_rptemp`;
CREATE TABLE `sys_rptemp`  (
  `FK_Rpt` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  PRIMARY KEY (`FK_Rpt`, `FK_Emp`) USING BTREE,
  INDEX `Sys_RptEmpID`(`FK_Rpt`, `FK_Emp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表人员对应信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_rptstation
-- ----------------------------
DROP TABLE IF EXISTS `sys_rptstation`;
CREATE TABLE `sys_rptstation`  (
  `FK_Rpt` varchar(15) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '报表',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '角色',
  PRIMARY KEY (`FK_Rpt`, `FK_Station`) USING BTREE,
  INDEX `Sys_RptStationID`(`FK_Rpt`, `FK_Station`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表角色对应信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_rpttemplate
-- ----------------------------
DROP TABLE IF EXISTS `sys_rpttemplate`;
CREATE TABLE `sys_rpttemplate`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EnsName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类名',
  `FK_Emp` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员',
  `D1` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'D1',
  `D2` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'D2',
  `D3` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'D3',
  `AlObjs` varchar(90) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '要分析的对象',
  `Height` int(0) NULL DEFAULT 600 COMMENT 'Height',
  `Width` int(0) NULL DEFAULT 800 COMMENT 'Width',
  `IsSumBig` int(0) NULL DEFAULT 0 COMMENT '是否显示大合计',
  `IsSumLittle` int(0) NULL DEFAULT 0 COMMENT '是否显示小合计',
  `IsSumRight` int(0) NULL DEFAULT 0 COMMENT '是否显示右合计',
  `PercentModel` int(0) NULL DEFAULT 0 COMMENT '比率显示方式',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_RptTemplate_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '报表模板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_serial
-- ----------------------------
DROP TABLE IF EXISTS `sys_serial`;
CREATE TABLE `sys_serial`  (
  `CfgKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'CfgKey',
  `IntVal` int(0) NULL DEFAULT 0 COMMENT '属性',
  PRIMARY KEY (`CfgKey`) USING BTREE,
  INDEX `Sys_Serial_CfgKey`(`CfgKey`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '序列号' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_serial
-- ----------------------------
INSERT INTO `sys_serial` VALUES ('BP.WF.Template.FlowSort', 102);
INSERT INTO `sys_serial` VALUES ('BP.WF.Template.SysFormTree', 106);
INSERT INTO `sys_serial` VALUES ('OID', 119);
INSERT INTO `sys_serial` VALUES ('UpdataCCFlowVer', 2023021511);
INSERT INTO `sys_serial` VALUES ('Ver', 20221023);

-- ----------------------------
-- Table structure for sys_sfdbsrc
-- ----------------------------
DROP TABLE IF EXISTS `sys_sfdbsrc`;
CREATE TABLE `sys_sfdbsrc`  (
  `No` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `DBSrcType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `DBSrcTypeT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `DBName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据库名称/Oracle保持为空',
  `ConnString` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '连接串/URL',
  `AtPara` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_SFDBSrc_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '数据源' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_sfdbsrc
-- ----------------------------
INSERT INTO `sys_sfdbsrc` VALUES ('CCFromRef', '本机JavaScript数据源', 'CCFromRef', 'local', '', '', '@EditType=1');
INSERT INTO `sys_sfdbsrc` VALUES ('local', '本机数据库', 'local', 'local', '', '', '@EditType=1');
INSERT INTO `sys_sfdbsrc` VALUES ('LocalHandler', '内置Handler', 'LocalHandler', 'local', '', '', '@EditType=1');
INSERT INTO `sys_sfdbsrc` VALUES ('LocalWS', '内置WebServies', 'LocalWS', 'local', '', '', '@EditType=1');

-- ----------------------------
-- Table structure for sys_sftable
-- ----------------------------
DROP TABLE IF EXISTS `sys_sftable`;
CREATE TABLE `sys_sftable`  (
  `No` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `NoGenerModel` int(0) NULL DEFAULT 1 COMMENT '编号生成规则',
  `CodeStruct` int(0) NULL DEFAULT 0 COMMENT '字典表类型',
  `SrcType` int(0) NULL DEFAULT 0 COMMENT '数据表类型',
  `FK_Val` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '默认创建的字段名',
  `TableDesc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '表描述',
  `DefVal` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '默认值',
  `FK_SFDBSrc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT 'local' COMMENT '数据源',
  `ParentValue` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Root节点的值(对树结构有效)',
  `SelectStatement` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '查询语句',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `RootVal` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '根节点值',
  `SrcTable` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '数据源表',
  `ColumnValue` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '显示的值(编号列)',
  `ColumnText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '显示的文字(名称列)',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '组织编号',
  `AtPara` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'AtPara',
  `BH0` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name0` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH1` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name1` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH2` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name2` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH3` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name3` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH4` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name4` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH5` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name5` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH6` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name6` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH7` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name7` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH8` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name8` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH9` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name9` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH10` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name10` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH11` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name11` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH12` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name12` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH13` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name13` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH14` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name14` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH15` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name15` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH16` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name16` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH17` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name17` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH18` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name18` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH19` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name19` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH20` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name20` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH21` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name21` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH22` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name22` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH23` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name23` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH24` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name24` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH25` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name25` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH26` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name26` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH27` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name27` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH28` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name28` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH29` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name29` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH30` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name30` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH31` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name31` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH32` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name32` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH33` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name33` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH34` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name34` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH35` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name35` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH36` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name36` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH37` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name37` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH38` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name38` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH39` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name39` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH40` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name40` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH41` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name41` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH42` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name42` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH43` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name43` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH44` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name44` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH45` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name45` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH46` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name46` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH47` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name47` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH48` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name48` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  `BH49` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '编号',
  `Name49` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '名称',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_SFTable_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '字典表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_sftabledtl
-- ----------------------------
DROP TABLE IF EXISTS `sys_sftabledtl`;
CREATE TABLE `sys_sftabledtl`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_SFTable` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '外键表ID',
  `BH` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'BH',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ParentNo` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点ID',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_SFTableDtl_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '系统字典表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_sms
-- ----------------------------
DROP TABLE IF EXISTS `sys_sms`;
CREATE TABLE `sys_sms`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Sender` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人(可以为空)',
  `SendTo` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送给(可以为空)',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '写入时间',
  `Mobile` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '手机号(可以为空)',
  `MobileSta` int(0) NULL DEFAULT 0 COMMENT '消息状态',
  `MobileInfo` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '短信信息',
  `Email` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Email(可以为空)',
  `EmailSta` int(0) NULL DEFAULT 0 COMMENT 'EmaiSta消息状态',
  `EmailTitle` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `EmailDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `SendDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送时间',
  `IsRead` int(0) NULL DEFAULT 0 COMMENT '是否读取?',
  `IsAlert` int(0) NULL DEFAULT 0 COMMENT '是否提示?',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT 'WorkID',
  `MsgFlag` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '消息标记(用于防止发送重复)',
  `MsgType` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '消息类型(CC抄送,Todolist待办,Return退回,Etc其他消息...)',
  `AtPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_SMS_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '消息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_userloglevel
-- ----------------------------
DROP TABLE IF EXISTS `sys_userloglevel`;
CREATE TABLE `sys_userloglevel`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_UserLogLevel_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '级别' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_userlogt
-- ----------------------------
DROP TABLE IF EXISTS `sys_userlogt`;
CREATE TABLE `sys_userlogt`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `EmpNo` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户账号',
  `EmpName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户名',
  `RDT` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `IP` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'IP',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '说明',
  `LogFlag` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `Level` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '级别',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_UserLogT_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户日志' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_userlogtype
-- ----------------------------
DROP TABLE IF EXISTS `sys_userlogtype`;
CREATE TABLE `sys_userlogtype`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `Sys_UserLogType_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sys_userregedit
-- ----------------------------
DROP TABLE IF EXISTS `sys_userregedit`;
CREATE TABLE `sys_userregedit`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `CfgKey` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `EnsName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `AttrKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员',
  `Vals` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Attrs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '属性s',
  `FK_MapData` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '实体',
  `LB` int(0) NULL DEFAULT 0 COMMENT '类别',
  `CurValue` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '文本',
  `GenerSQL` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'GenerSQL',
  `Paras` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'Paras',
  `NumKey` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '分析的Key',
  `OrderBy` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OrderBy',
  `OrderWay` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OrderWay',
  `SearchKey` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SearchKey',
  `MVals` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'MVals',
  `IsPic` int(0) NULL DEFAULT 0 COMMENT '是否图片',
  `DTFrom` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '查询时间从',
  `DTTo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '到',
  `OrgNo` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OrgNo',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `Sys_UserRegedit_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '用户注册表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_accepterrole
-- ----------------------------
DROP TABLE IF EXISTS `wf_accepterrole`;
CREATE TABLE `wf_accepterrole`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_Node` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点',
  `FK_Mode` int(0) NULL DEFAULT 0 COMMENT '模式类型',
  `Tag0` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag0',
  `Tag1` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Tag2` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag2',
  `Tag3` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag3',
  `Tag4` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag4',
  `Tag5` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag5',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `WF_AccepterRole_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '接受人规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_auth
-- ----------------------------
DROP TABLE IF EXISTS `wf_auth`;
CREATE TABLE `wf_auth`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Auther` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '授权人',
  `AuthType` int(0) NULL DEFAULT 0 COMMENT '类型(0=全部流程1=指定流程2=取消)',
  `AutherToEmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '授权给谁?',
  `AutherToEmpName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '授权给谁?',
  `FlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FlowName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程名称',
  `TakeBackDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '取回日期',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_Auth_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '授权' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_autorpt
-- ----------------------------
DROP TABLE IF EXISTS `wf_autorpt`;
CREATE TABLE `wf_autorpt`  (
  `No` varchar(2) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `DTOfExe` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '最近执行时间',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `StartDT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起时间点',
  `ToEmpOfSQLs` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '通知的人员SQL',
  `Dots` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '执行的时间点(系统写入)',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_AutoRpt_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '自动报表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_autorptdtl
-- ----------------------------
DROP TABLE IF EXISTS `wf_autorptdtl`;
CREATE TABLE `wf_autorptdtl`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `Name` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `SQLExp` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SQL表达式(返回一个数字)',
  `UrlExp` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Url表达式',
  `BeiZhu` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注',
  `AutoRptNo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '任务ID',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `WF_AutoRptDtl_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '自动报表-数据项' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_ccdept
-- ----------------------------
DROP TABLE IF EXISTS `wf_ccdept`;
CREATE TABLE `wf_ccdept`  (
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '部门',
  PRIMARY KEY (`FK_Node`, `FK_Dept`) USING BTREE,
  INDEX `WF_CCDeptID`(`FK_Node`, `FK_Dept`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_ccemp
-- ----------------------------
DROP TABLE IF EXISTS `wf_ccemp`;
CREATE TABLE `wf_ccemp`  (
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  PRIMARY KEY (`FK_Node`, `FK_Emp`) USING BTREE,
  INDEX `WF_CCEmpID`(`FK_Node`, `FK_Emp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送人员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_cclist
-- ----------------------------
DROP TABLE IF EXISTS `wf_cclist`;
CREATE TABLE `wf_cclist`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Title` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `Doc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '内容',
  `Sta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FlowName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `NodeName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT '工作ID',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送人员',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送日期',
  `CCTo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送给',
  `CCToName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送给(人员名称)',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织',
  `CDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '打开时间',
  `ReadDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '阅读时间',
  `PFlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父流程编号',
  `PWorkID` int(0) NULL DEFAULT 0 COMMENT '父流程WorkID',
  `InEmpWorks` int(0) NULL DEFAULT 0 COMMENT '是否加入待办列表',
  `Domain` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Domain',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_CCList_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_ccrole
-- ----------------------------
DROP TABLE IF EXISTS `wf_ccrole`;
CREATE TABLE `wf_ccrole`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `CCRoleExcType` int(0) NULL DEFAULT 0 COMMENT '执行类型',
  `Tag1` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '执行内容1',
  `Tag2` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '执行内容2',
  `CCStaWay` int(0) NULL DEFAULT 0 COMMENT 'CCStaWay',
  `AtPara` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_CCRole_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_ccstation
-- ----------------------------
DROP TABLE IF EXISTS `wf_ccstation`;
CREATE TABLE `wf_ccstation`  (
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '工作角色',
  PRIMARY KEY (`FK_Node`, `FK_Station`) USING BTREE,
  INDEX `WF_CCStationID`(`FK_Node`, `FK_Station`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_ch
-- ----------------------------
DROP TABLE IF EXISTS `wf_ch`;
CREATE TABLE `wf_ch`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT '工作ID',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `Title` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `FK_Flow` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_FlowT` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_NodeT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `Sender` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人',
  `SenderT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人名称',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_EmpT` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `GroupEmps` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '相关当事人',
  `GroupEmpsNames` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '相关当事人名称',
  `GroupEmpsNum` int(0) NULL DEFAULT 1 COMMENT '相关当事人数量',
  `DTFrom` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '任务下达时间',
  `DTTo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '任务处理时间',
  `SDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '应完成日期',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_DeptT` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_NY` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DTSWay` int(0) NULL DEFAULT 0 COMMENT '考核方式',
  `TimeLimit` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '规定限期',
  `OverMinutes` float NULL DEFAULT NULL COMMENT '逾期分钟',
  `UseDays` float NULL DEFAULT NULL COMMENT '实际使用天',
  `OverDays` float NULL DEFAULT NULL COMMENT '逾期天',
  `CHSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `WeekNum` int(0) NULL DEFAULT 0 COMMENT '第几周',
  `Points` float NULL DEFAULT NULL COMMENT '总扣分',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_CH_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '时效考核' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_cheval
-- ----------------------------
DROP TABLE IF EXISTS `wf_cheval`;
CREATE TABLE `wf_cheval`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Title` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FlowName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程名称',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT '工作ID',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '评价节点',
  `NodeName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '停留节点',
  `Rec` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评价人',
  `RecName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评价人名称',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评价日期',
  `EvalEmpNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '被考核的人员编号',
  `EvalEmpName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '被考核的人员名称',
  `EvalCent` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评价分值',
  `EvalNote` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评价内容',
  `FK_Dept` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `DeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门名称',
  `FK_NY` varchar(7) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '年月',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_CHEval_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作质量评价' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_chnode
-- ----------------------------
DROP TABLE IF EXISTS `wf_chnode`;
CREATE TABLE `wf_chnode`  (
  `WorkID` int(0) NULL DEFAULT 0 COMMENT 'WorkID',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `NodeName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `FK_Emp` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '处理人',
  `FK_EmpT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '处理人名称',
  `StartDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '计划开始时间',
  `EndDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '计划结束时间',
  `GT` int(0) NULL DEFAULT 0 COMMENT '工天',
  `Scale` float NULL DEFAULT NULL COMMENT '阶段占比',
  `TotalScale` float NULL DEFAULT NULL COMMENT '总进度',
  `ChanZhi` float NULL DEFAULT NULL COMMENT '产值',
  `AtPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  `MyPK` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'MyPK',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_CHNode_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点时限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_cond
-- ----------------------------
DROP TABLE IF EXISTS `wf_cond`;
CREATE TABLE `wf_cond`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `RefPKVal` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关联主键',
  `RefFlowNo` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `CondType` int(0) NULL DEFAULT 0 COMMENT '条件类型',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `SubFlowNo` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子流程编号',
  `ToNodeID` int(0) NULL DEFAULT 0 COMMENT 'ToNodeID（对方向条件有效）',
  `DataFrom` int(0) NULL DEFAULT 0 COMMENT '条件数据来源',
  `DataFromText` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '条件数据来源T',
  `FK_Attr` varchar(80) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '属性',
  `AttrKey` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '属性键',
  `AttrName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '中文名称',
  `FK_Operator` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '运算符号',
  `OperatorValue` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '要运算的值',
  `OperatorValueT` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '要运算的值T',
  `Note` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注',
  `FK_DBSrc` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SQL的数据来源',
  `AtPara` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '优先级',
  `Tag1` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_Cond_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '条件' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_deptflowsearch
-- ----------------------------
DROP TABLE IF EXISTS `wf_deptflowsearch`;
CREATE TABLE `wf_deptflowsearch`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Emp` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门编号',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_DeptFlowSearch_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程部门数据查询权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_direction
-- ----------------------------
DROP TABLE IF EXISTS `wf_direction`;
CREATE TABLE `wf_direction`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `Node` int(0) NULL DEFAULT 0 COMMENT '从节点',
  `ToNode` int(0) NULL DEFAULT 0 COMMENT '到节点',
  `ToNodeName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到达节点名称',
  `GateWay` int(0) NULL DEFAULT 0 COMMENT '网关显示?',
  `Des` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '描述',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '计算优先级顺序',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_Direction_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点方向信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_direction
-- ----------------------------
INSERT INTO `wf_direction` VALUES ('001_101_102', '001', 101, 102, '部门经理审批', 0, '', 0);
INSERT INTO `wf_direction` VALUES ('001_102_103', '001', 102, 103, '总经理审批', 0, '', 0);
INSERT INTO `wf_direction` VALUES ('001_103_104', '001', 103, 104, '反馈给申请人', 0, '', 0);

-- ----------------------------
-- Table structure for wf_directionstation
-- ----------------------------
DROP TABLE IF EXISTS `wf_directionstation`;
CREATE TABLE `wf_directionstation`  (
  `FK_Direction` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '工作角色',
  PRIMARY KEY (`FK_Direction`, `FK_Station`) USING BTREE,
  INDEX `WF_DirectionStationID`(`FK_Direction`, `FK_Station`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_doctemplate
-- ----------------------------
DROP TABLE IF EXISTS `wf_doctemplate`;
CREATE TABLE `wf_doctemplate`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'No',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `FilePath` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '模板路径',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_DocTemplate_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '公文模板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_emp
-- ----------------------------
DROP TABLE IF EXISTS `wf_emp`;
CREATE TABLE `wf_emp`  (
  `No` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'No',
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Name',
  `UseSta` int(0) NULL DEFAULT 1 COMMENT '用户状态0禁用,1正常.',
  `Tel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tel',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FK_Dept',
  `Email` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Email',
  `Stas` varchar(3000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色s',
  `Depts` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门s',
  `Msg` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '消息',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `SPass` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图片签名密码',
  `Author` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '授权人',
  `AuthorDate` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '授权日期',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  `StartFlows` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '可以发起的流程',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_Emp_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '操作员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_emp
-- ----------------------------
INSERT INTO `wf_emp` VALUES ('admin', 'admin', 1, NULL, NULL, 'zhoupeng@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('fuhui', '福惠', 1, NULL, NULL, 'fuhui@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('guobaogeng', '郭宝庚', 1, NULL, NULL, 'guobaogeng@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('guoxiangbin', '郭祥斌', 1, NULL, NULL, 'guoxiangbin@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('liping', '李萍', 1, NULL, NULL, 'liping@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('liyan', '李言', 1, NULL, NULL, 'liyan@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('qifenglin', '祁凤林', 1, NULL, NULL, 'qifenglin@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('yangyilei', '杨依雷', 1, NULL, NULL, 'yangyilei@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('zhanghaicheng', '张海成', 1, NULL, NULL, 'zhanghaicheng@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('zhangyifan', '张一帆', 1, NULL, NULL, 'zhangyifan@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('zhoupeng', '周朋', 1, NULL, NULL, 'zhoupeng@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('zhoushengyu', '周升雨', 1, NULL, NULL, 'zhoushengyu@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wf_emp` VALUES ('zhoutianjiao', '周天娇', 1, NULL, NULL, 'zhoutianjiao@ccflow.org', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for wf_findworkerrole
-- ----------------------------
DROP TABLE IF EXISTS `wf_findworkerrole`;
CREATE TABLE `wf_findworkerrole`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Name',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `SortVal0` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortVal0',
  `SortText0` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortText0',
  `SortVal1` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortVal1',
  `SortText1` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortText1',
  `SortVal2` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortText2',
  `SortText2` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortText2',
  `SortVal3` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortVal3',
  `SortText3` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SortText3',
  `TagVal0` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagVal0',
  `TagVal1` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagVal1',
  `TagVal2` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagVal2',
  `TagVal3` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagVal3',
  `TagText0` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagText0',
  `TagText1` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagText1',
  `TagText2` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagText2',
  `TagText3` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'TagText3',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否可用',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'IDX',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `WF_FindWorkerRole_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '找人规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_flow
-- ----------------------------
DROP TABLE IF EXISTS `wf_flow`;
CREATE TABLE `wf_flow`  (
  `No` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FK_FlowSort` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类别',
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FlowMark` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程标记',
  `FlowEventEntity` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程事件实体',
  `TitleRole` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题生成规则',
  `TitleRoleNodes` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '生成标题的节点',
  `IsCanStart` int(0) NULL DEFAULT 1 COMMENT '可以独立启动否？(独立启动的流程可以显示在发起流程列表里)',
  `IsFullSA` int(0) NULL DEFAULT 0 COMMENT '是否自动计算未来的处理人？',
  `GuestFlowRole` int(0) NULL DEFAULT 0 COMMENT '外部用户参与流程规则',
  `Draft` int(0) NULL DEFAULT 0 COMMENT '草稿规则',
  `SysType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '系统类型',
  `Tester` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起测试人',
  `ChartType` int(0) NULL DEFAULT 1 COMMENT '节点图形类型',
  `CreateDate` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '创建日期',
  `Creater` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsBatchStart` int(0) NULL DEFAULT 0 COMMENT '是否可以批量发起流程？(如果是就要设置发起的需要填写的字段,多个用逗号分开)',
  `BatchStartFields` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsResetData` int(0) NULL DEFAULT 0 COMMENT '是否启用开始节点数据重置按钮？已经取消)',
  `IsLoadPriData` int(0) NULL DEFAULT 0 COMMENT '是否自动装载上一笔数据？',
  `IsDBTemplate` int(0) NULL DEFAULT 1 COMMENT '是否启用数据模版？',
  `IsStartInMobile` int(0) NULL DEFAULT 1 COMMENT '是否可以在手机里启用？(如果发起表单特别复杂就不要在手机里启用了)',
  `IsMD5` int(0) NULL DEFAULT 0 COMMENT '是否是数据防止篡改(MD5数据加密防篡改)',
  `IsJM` int(0) NULL DEFAULT 0 COMMENT '是否是数据加密流程(把所有字段加密存储)',
  `IsEnableDBVer` int(0) NULL DEFAULT 0 COMMENT '是否是启用数据版本控制',
  `PTable` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程数据存储表',
  `BillNoFormat` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号格式',
  `BuessFields` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关键业务字段',
  `AdvEmps` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '高级查询人员',
  `IsFrmEnable` int(0) NULL DEFAULT 0 COMMENT '是否显示表单',
  `IsTruckEnable` int(0) NULL DEFAULT 1 COMMENT '是否显示轨迹图',
  `IsTimeBaseEnable` int(0) NULL DEFAULT 1 COMMENT '是否显示时间轴',
  `IsTableEnable` int(0) NULL DEFAULT 1 COMMENT '是否显示时间表',
  `IsOPEnable` int(0) NULL DEFAULT 0 COMMENT '是否显示操作',
  `SubFlowShowType` int(0) NULL DEFAULT 0 COMMENT '子流程轨迹图显示模式',
  `TrackOrderBy` int(0) NULL DEFAULT 0 COMMENT '排序方式',
  `FlowRunWay` int(0) NULL DEFAULT 0 COMMENT '运行方式',
  `WorkModel` int(0) NULL DEFAULT 0 COMMENT 'WorkModel',
  `RunObj` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '运行内容',
  `Note` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '备注',
  `RunSQL` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '流程结束执行后执行的SQL',
  `NumOfBill` int(0) NULL DEFAULT 0 COMMENT '是否有单据',
  `NumOfDtl` int(0) NULL DEFAULT 0 COMMENT 'NumOfDtl',
  `FlowAppType` int(0) NULL DEFAULT 0 COMMENT '流程类型',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '显示顺序号(在发起列表中)',
  `SDTOfFlowRole` int(0) NULL DEFAULT 0 COMMENT '流程计划完成日期计算规则',
  `SDTOfFlowRoleSQL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '流程计划完成日期计算规则SQL',
  `FrmUrl` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '表单Url',
  `DRCtrlType` int(0) NULL DEFAULT 0 COMMENT '部门查询权限控制方式',
  `IsToParentNextNode` int(0) NULL DEFAULT 0 COMMENT '子流程运行到该节点时，让父流程自动运行到下一步',
  `StartLimitRole` int(0) NULL DEFAULT 0 COMMENT '启动限制规则',
  `StartLimitPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '规则内容',
  `StartLimitAlert` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '限制提示',
  `StartLimitWhen` int(0) NULL DEFAULT 0 COMMENT '提示时间',
  `StartGuideWay` int(0) NULL DEFAULT 0 COMMENT '前置导航方式',
  `StartGuideLink` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '右侧的连接',
  `StartGuideLab` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '连接标签',
  `StartGuidePara1` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '参数1',
  `StartGuidePara2` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '参数2',
  `StartGuidePara3` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '参数3',
  `Ver` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '版本号',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'OrgNo',
  `DevModelType` int(0) NULL DEFAULT 0 COMMENT '设计模式',
  `DevModelPara` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '设计模式参数',
  `AtPara` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'AtPara',
  `DataDTSWay` int(0) NULL DEFAULT 0 COMMENT '同步方式',
  `DTSDBSrc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '数据源',
  `DTSBTable` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '业务表名',
  `DTSBTablePK` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '业务表主键',
  `DTSSpecNodes` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '同步字段',
  `DTSTime` int(0) NULL DEFAULT 0 COMMENT '执行同步时间点',
  `DTSFields` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '要同步的字段s,中间用逗号分开.',
  `FlowDevModel` int(0) NULL DEFAULT 0 COMMENT '设计模式',
  `PStarter` int(0) NULL DEFAULT 1 COMMENT '发起人可看(必选)',
  `PWorker` int(0) NULL DEFAULT 1 COMMENT '参与人可看(必选)',
  `PCCer` int(0) NULL DEFAULT 1 COMMENT '被抄送人可看(必选)',
  `PAnyOne` int(0) NULL DEFAULT 0 COMMENT '任何人可见',
  `PMyDept` int(0) NULL DEFAULT 1 COMMENT '本部门人可看',
  `PPMyDept` int(0) NULL DEFAULT 1 COMMENT '直属上级部门可看',
  `PPDept` int(0) NULL DEFAULT 1 COMMENT '上级部门可看',
  `PSameDept` int(0) NULL DEFAULT 1 COMMENT '平级部门可看',
  `PSpecDept` int(0) NULL DEFAULT 1 COMMENT '指定部门可看',
  `PSpecDeptExt` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '部门编号',
  `PSpecSta` int(0) NULL DEFAULT 1 COMMENT '指定的角色可看',
  `PSpecStaExt` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '角色编号',
  `PSpecGroup` int(0) NULL DEFAULT 1 COMMENT '指定的权限组可看',
  `PSpecGroupExt` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '权限组',
  `PSpecEmp` int(0) NULL DEFAULT 1 COMMENT '指定的人员可看',
  `PSpecEmpExt` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '指定的人员编号',
  `MyDeptRole` int(0) NULL DEFAULT 0 COMMENT '本部门发起的流程',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_Flow_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程模版' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_flow
-- ----------------------------
INSERT INTO `wf_flow` VALUES ('001', '100', '请假流程-经典表单-演示', '', '', '', '', 1, 0, 0, 0, '', NULL, 1, '', '', 0, '', 0, 0, 0, 1, 0, 0, 0, '', '', NULL, NULL, 0, 1, 1, 1, 0, 0, 0, 0, 0, '', '', '', 0, 0, 0, 0, 0, '', '', 0, 0, 0, '', '', 0, 0, '', '', '', '', '', '2023-02-15 11:34:21', '', 0, '', '', 0, '', '', '', '', 0, '', 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, '', 1, '', 1, '', 1, '', 0);

-- ----------------------------
-- Table structure for wf_floworg
-- ----------------------------
DROP TABLE IF EXISTS `wf_floworg`;
CREATE TABLE `wf_floworg`  (
  `FlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '流程',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '到组织',
  PRIMARY KEY (`FlowNo`, `OrgNo`) USING BTREE,
  INDEX `WF_FlowOrgID`(`FlowNo`, `OrgNo`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程对应组织' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_flowsort
-- ----------------------------
DROP TABLE IF EXISTS `wf_flowsort`;
CREATE TABLE `wf_flowsort`  (
  `No` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `ParentNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父节点No',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ShortName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '简称',
  `OrgNo` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '组织编号(0为系统组织)',
  `Domain` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '域/系统编号',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_FlowSort_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程目录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_flowsort
-- ----------------------------
INSERT INTO `wf_flowsort` VALUES ('1', '0', '流程树', '', '0', '', 0);
INSERT INTO `wf_flowsort` VALUES ('100', '1', '日常办公类', '', '0', '', 0);
INSERT INTO `wf_flowsort` VALUES ('101', '1', '财务类', '', '0', '', 0);
INSERT INTO `wf_flowsort` VALUES ('102', '1', '人力资源类', '', '0', '', 0);

-- ----------------------------
-- Table structure for wf_flowtab
-- ----------------------------
DROP TABLE IF EXISTS `wf_flowtab`;
CREATE TABLE `wf_flowtab`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签',
  `FK_Flow` varchar(4) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Mark` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标记',
  `Tip` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tip',
  `UrlExt` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'url链接',
  `Icon` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Icon',
  `OrgNo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'OrgNo',
  `Idx` int(0) NULL DEFAULT 0 COMMENT 'Idx',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_FlowTab_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程功能' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_frmnode
-- ----------------------------
DROP TABLE IF EXISTS `wf_frmnode`;
CREATE TABLE `wf_frmnode`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Frm` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `IsPrint` int(0) NULL DEFAULT 0 COMMENT '是否可以打印',
  `IsEnableLoadData` int(0) NULL DEFAULT 0 COMMENT '是否启用装载填充事件',
  `IsCloseEtcFrm` int(0) NULL DEFAULT 0 COMMENT '打开时是否关闭其它的页面？',
  `WhoIsPK` int(0) NULL DEFAULT 0 COMMENT '谁是主键?',
  `FrmSln` int(0) NULL DEFAULT 0 COMMENT '控制方案',
  `BillNoField` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号对应字段',
  `BillNoFieldText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号对应字段',
  `FrmNameShow` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单显示名字',
  `Is1ToN` int(0) NULL DEFAULT 0 COMMENT '是否1变N？(分流节点有效)',
  `HuiZong` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '汇总的数据表名',
  `TempleteFile` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '模版文件',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  `IsEnableFWC` int(0) NULL DEFAULT 0 COMMENT '审核组件状态',
  `CheckField` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '签批字段',
  `CheckFieldText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '签批字段',
  `SFSta` int(0) NULL DEFAULT 0 COMMENT '父子流程组件状态',
  `FrmEnableRole` int(0) NULL DEFAULT 0 COMMENT '启用规则',
  `FrmEnableExp` varchar(900) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '启用的表达式',
  `FK_Flow` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FrmType` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '表单类型',
  `IsDefaultOpen` int(0) NULL DEFAULT 0 COMMENT '是否默认打开',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否显示',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_FrmNode_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点表单' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_frmnodefieldremove
-- ----------------------------
DROP TABLE IF EXISTS `wf_frmnodefieldremove`;
CREATE TABLE `wf_frmnodefieldremove`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FrmID` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点编号',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Fields` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段',
  `ExpType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表达式类型',
  `Exp` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表达式',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_FrmNodeFieldRemove_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单节点排除规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_frmorg
-- ----------------------------
DROP TABLE IF EXISTS `wf_frmorg`;
CREATE TABLE `wf_frmorg`  (
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '表单',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '到组织',
  PRIMARY KEY (`FrmID`, `OrgNo`) USING BTREE,
  INDEX `WF_FrmOrgID`(`FrmID`, `OrgNo`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '表单对应组织' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_generworkerlist
-- ----------------------------
DROP TABLE IF EXISTS `wf_generworkerlist`;
CREATE TABLE `wf_generworkerlist`  (
  `WorkID` int(0) NOT NULL DEFAULT 0 COMMENT '工作ID',
  `FK_Emp` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人员',
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点ID',
  `FID` int(0) NULL DEFAULT 0 COMMENT '流程ID',
  `FK_EmpText` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员名称',
  `FK_NodeText` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员所在部门',
  `SDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '应完成日期',
  `DTOfWarning` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '警告日期',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录时间(接受工作日期)',
  `CDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '完成时间',
  `IsEnable` int(0) NULL DEFAULT 1 COMMENT '是否可用',
  `IsRead` int(0) NULL DEFAULT 0 COMMENT '是否读取',
  `IsPass` int(0) NULL DEFAULT 0 COMMENT '是否通过(对合流节点有效)',
  `WhoExeIt` int(0) NULL DEFAULT 0 COMMENT '谁执行它',
  `Sender` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人',
  `PRI` int(0) NULL DEFAULT 1 COMMENT '优先级',
  `PressTimes` int(0) NULL DEFAULT 0 COMMENT '催办次数',
  `DTOfHungup` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '挂起时间',
  `DTOfUnHungup` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '预计解除挂起时间',
  `HungupTimes` int(0) NULL DEFAULT 0 COMMENT '挂起次数',
  `GuestNo` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '外部用户编号',
  `GuestName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '外部用户名称',
  `Idx` int(0) NULL DEFAULT 1 COMMENT '顺序号',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  PRIMARY KEY (`WorkID`, `FK_Emp`, `FK_Node`) USING BTREE,
  INDEX `WF_GenerWorkerlistID`(`WorkID`, `FK_Emp`, `FK_Node`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作者' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_generworkflow
-- ----------------------------
DROP TABLE IF EXISTS `wf_generworkflow`;
CREATE TABLE `wf_generworkflow`  (
  `WorkID` int(0) NOT NULL DEFAULT 0 COMMENT 'WorkID',
  `FID` int(0) NULL DEFAULT 0 COMMENT '流程ID',
  `FK_FlowSort` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `SysType` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '系统类别',
  `FK_Flow` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FlowName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程名称',
  `Title` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `WFSta` int(0) NULL DEFAULT 0 COMMENT '状态',
  `WFState` int(0) NULL DEFAULT 0 COMMENT '状态',
  `Starter` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人',
  `StarterName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人名称',
  `Sender` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录日期',
  `HungupTime` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '挂起日期',
  `SendDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程活动时间',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `NodeName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `DeptName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门名称',
  `PRI` int(0) NULL DEFAULT 1 COMMENT '优先级',
  `SDTOfNode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点应完成时间',
  `SDTOfFlow` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程应完成时间',
  `SDTOfFlowWarning` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程预警时间',
  `PFlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '父流程编号',
  `PWorkID` int(0) NULL DEFAULT 0 COMMENT '父流程ID',
  `PNodeID` int(0) NULL DEFAULT 0 COMMENT '父流程调用节点',
  `PFID` int(0) NULL DEFAULT 0 COMMENT '父流程调用的PFID',
  `PEmp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子流程的调用人',
  `GuestNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户编号',
  `GuestName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户名称',
  `BillNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '单据编号',
  `TodoEmps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '待办人员',
  `TodoEmpsNum` int(0) NULL DEFAULT 0 COMMENT '待办人员数量',
  `TaskSta` int(0) NULL DEFAULT 0 COMMENT '共享状态',
  `AtPara` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参数',
  `Emps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参与人',
  `GUID` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'GUID',
  `FK_NY` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `WeekNum` int(0) NULL DEFAULT 0 COMMENT '周次',
  `TSpan` int(0) NULL DEFAULT 0 COMMENT '时间间隔',
  `TodoSta` int(0) NULL DEFAULT 0 COMMENT '待办状态',
  `Domain` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '域/系统编号',
  `PrjNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'PrjNo',
  `PrjName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'PrjNo',
  `OrgNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FlowNote` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程备注',
  `LostTimeHH` float NULL DEFAULT NULL COMMENT '耗时',
  PRIMARY KEY (`WorkID`) USING BTREE,
  INDEX `WF_GenerWorkFlow_WorkID`(`WorkID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程查询' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_labnote
-- ----------------------------
DROP TABLE IF EXISTS `wf_labnote`;
CREATE TABLE `wf_labnote`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Name` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `FK_Flow` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `X` int(0) NULL DEFAULT 0 COMMENT 'X坐标',
  `Y` int(0) NULL DEFAULT 0 COMMENT 'Y坐标',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_LabNote_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '标签' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_node
-- ----------------------------
DROP TABLE IF EXISTS `wf_node`;
CREATE TABLE `wf_node`  (
  `NodeID` int(0) NOT NULL DEFAULT 0 COMMENT '节点ID',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FlowName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程名',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `WhoExeIt` int(0) NULL DEFAULT 0 COMMENT '谁执行它',
  `ReadReceipts` int(0) NULL DEFAULT 0 COMMENT '已读回执',
  `CancelRole` int(0) NULL DEFAULT 0 COMMENT '撤销规则',
  `CancelDisWhenRead` int(0) NULL DEFAULT 0 COMMENT '对方已经打开就不能撤销',
  `IsOpenOver` int(0) NULL DEFAULT 0 COMMENT '已阅即完成?',
  `IsSendDraftSubFlow` int(0) NULL DEFAULT 0 COMMENT '是否发送草稿子流程?',
  `IsResetAccepter` int(0) NULL DEFAULT 0 COMMENT '可逆节点时重新计算接收人?',
  `IsGuestNode` int(0) NULL DEFAULT 0 COMMENT '是否是外部用户执行的节点(非组织结构人员参与处理工作的节点)?',
  `IsYouLiTai` int(0) NULL DEFAULT 0 COMMENT '该节点是否是游离态',
  `FocusField` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '焦点字段',
  `FWCSta` int(0) NULL DEFAULT 0 COMMENT '节点状态',
  `FWCAth` int(0) NULL DEFAULT 0 COMMENT '审核附件是否启用',
  `DeliveryWay` int(0) NULL DEFAULT 0 COMMENT '接受人规则',
  `SelfParas` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `Step` int(0) NULL DEFAULT 0 COMMENT '步骤(无计算意义)',
  `Tip` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作提示',
  `RunModel` int(0) NULL DEFAULT 0 COMMENT '节点类型',
  `PassRate` decimal(20, 4) NULL DEFAULT NULL COMMENT '完成通过率',
  `ThreadIsCanDel` int(0) NULL DEFAULT 1 COMMENT '是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？',
  `ThreadIsCanAdd` int(0) NULL DEFAULT 1 COMMENT '是否可以增加子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效)？',
  `ThreadIsCanShift` int(0) NULL DEFAULT 0 COMMENT '是否可以移交子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？',
  `USSWorkIDRole` int(0) NULL DEFAULT 0 COMMENT '异表单子线程WorkID生成规则',
  `IsSendBackNode` int(0) NULL DEFAULT 0 COMMENT '是否是发送返回节点(发送当前节点,自动发送给该节点的发送人,发送节点.)?',
  `AutoJumpRole0` int(0) NULL DEFAULT 0 COMMENT '处理人就是发起人',
  `AutoJumpRole1` int(0) NULL DEFAULT 0 COMMENT '处理人已经出现过',
  `AutoJumpRole2` int(0) NULL DEFAULT 0 COMMENT '处理人与上一步相同',
  `WhenNoWorker` int(0) NULL DEFAULT 0 COMMENT '(是)找不到人就跳转,(否)提示错误.',
  `AutoJumpExp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表达式',
  `SkipTime` int(0) NULL DEFAULT 0 COMMENT '执行跳转事件',
  `SendLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送按钮标签',
  `SendJS` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '按钮JS函数',
  `SaveLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '保存按钮标签',
  `SaveEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `CCLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送按钮标签',
  `CCRole` int(0) NULL DEFAULT 0 COMMENT '抄送规则',
  `QRCodeLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '二维码标签',
  `QRCodeRole` int(0) NULL DEFAULT 0 COMMENT '二维码规则',
  `ShiftLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '移交按钮标签',
  `ShiftEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `DelLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '删除按钮标签',
  `DelEnable` int(0) NULL DEFAULT 0 COMMENT '删除规则',
  `EndFlowLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '结束流程按钮标签',
  `EndFlowEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `ShowParentFormLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '查看父流程按钮标签',
  `ShowParentFormEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `OfficeBtnLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '公文按钮标签',
  `OfficeBtnEnable` int(0) NULL DEFAULT 0 COMMENT '文件状态',
  `OfficeFileType` int(0) NULL DEFAULT 0 COMMENT '文件类型',
  `OfficeBtnLocal` int(0) NULL DEFAULT 0 COMMENT '按钮位置',
  `TrackLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '轨迹按钮标签',
  `TrackEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `HungLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '挂起按钮标签',
  `HungEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `SearchLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '查询按钮标签',
  `SearchEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `TCLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流转自定义',
  `TCEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `FrmDBVerLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据版本',
  `FrmDBVerEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `FrmDBRemarkLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据批阅',
  `FrmDBRemarkEnable` int(0) NULL DEFAULT 0 COMMENT '数据批阅',
  `PRILab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '重要性',
  `PRIEnable` int(0) NULL DEFAULT 0 COMMENT '重要性规则',
  `CHLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点时限',
  `CHRole` int(0) NULL DEFAULT 0 COMMENT '时限规则',
  `AllotLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分配按钮标签',
  `AllotEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `FocusLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关注',
  `FocusEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `ConfirmLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '确认按钮标签',
  `ConfirmEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `ListLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '列表按钮标签',
  `ListEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `BatchLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '批量审核标签',
  `BatchEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `NoteLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备注标签',
  `NoteEnable` int(0) NULL DEFAULT 0 COMMENT '启用规则',
  `HelpLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '帮助标签',
  `HelpRole` int(0) NULL DEFAULT 0 COMMENT '帮助显示规则',
  `ScripLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '小纸条标签',
  `ScripRole` int(0) NULL DEFAULT 0 COMMENT '小纸条显示规则',
  `FlowBBSLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评论标签',
  `FlowBBSRole` int(0) NULL DEFAULT 0 COMMENT '评论规则',
  `IMLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '即时通讯标签',
  `IMEnable` int(0) NULL DEFAULT 0 COMMENT '即时通讯规则',
  `NextLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '下一条',
  `NextRole` int(0) NULL DEFAULT 0 COMMENT '获得规则',
  `ThreadLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子线程按钮标签',
  `ThreadEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `ThreadKillRole` int(0) NULL DEFAULT 0 COMMENT '子线程删除方式',
  `JumpWayLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '跳转按钮标签',
  `JumpWay` int(0) NULL DEFAULT 0 COMMENT '跳转规则',
  `JumpToNodes` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '可跳转的节点',
  `ShowParentFormEnableMyView` int(0) NULL DEFAULT 0 COMMENT '查看父流程是否启用',
  `TrackEnableMyView` int(0) NULL DEFAULT 1 COMMENT '轨迹是否启用',
  `FrmDBVerMyView` int(0) NULL DEFAULT 0 COMMENT '数据版本是否启用',
  `FrmDBRemarkEnableMyView` int(0) NULL DEFAULT 0 COMMENT '数据批阅',
  `PressLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '催办',
  `PressEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `RollbackLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '回滚',
  `RollbackEnable` int(0) NULL DEFAULT 1 COMMENT '是否启用',
  `ShowParentFormEnableMyCC` int(0) NULL DEFAULT 0 COMMENT '查看父流程是否启用',
  `TrackEnableMyCC` int(0) NULL DEFAULT 1 COMMENT '轨迹是否启用',
  `FrmDBVerMyCC` int(0) NULL DEFAULT 0 COMMENT '数据版本是否启用',
  `ReturnLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回按钮标签',
  `ReturnRole` int(0) NULL DEFAULT 0 COMMENT '退回规则',
  `ReturnAlert` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '被退回后信息提示',
  `IsBackTracking` int(0) NULL DEFAULT 0 COMMENT '是否可以原路返回',
  `IsBackResetAccepter` int(0) NULL DEFAULT 0 COMMENT '原路返回后是否自动计算接收人.',
  `IsKillEtcThread` int(0) NULL DEFAULT 0 COMMENT '子线程退回,其它子线程删除规则',
  `ReturnCHEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用退回考核规则',
  `ReturnOneNodeRole` int(0) NULL DEFAULT 0 COMMENT '单节点退回规则',
  `ReturnField` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回信息填写字段',
  `ReturnReasonsItems` varchar(999) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回原因',
  `PrintHtmlLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '打印Html标签',
  `PrintHtmlEnable` int(0) NULL DEFAULT 0 COMMENT '(打印Html)是否启用',
  `PrintHtmlMyView` int(0) NULL DEFAULT 0 COMMENT '(打印Html)显示在查看器工具栏?',
  `PrintHtmlMyCC` int(0) NULL DEFAULT 0 COMMENT '(打印Html)显示在抄送工具栏?',
  `PrintPDFLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '打印pdf标签',
  `PrintPDFEnable` int(0) NULL DEFAULT 0 COMMENT '(打印pdf)是否启用',
  `PrintPDFMyView` int(0) NULL DEFAULT 0 COMMENT '(打印pdf)显示在查看器工具栏?',
  `PrintPDFMyCC` int(0) NULL DEFAULT 0 COMMENT '(打印pdf)显示在抄送工具栏?',
  `PrintPDFModle` int(0) NULL DEFAULT 0 COMMENT 'PDF打印规则',
  `ShuiYinModle` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'PDF水印内容',
  `PrintZipLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '打包下载zip标签',
  `PrintZipEnable` int(0) NULL DEFAULT 0 COMMENT '(打包下载zip)是否启用',
  `PrintZipMyView` int(0) NULL DEFAULT 0 COMMENT '(打包下载zip)显示在查看器工具栏?',
  `PrintZipMyCC` int(0) NULL DEFAULT 0 COMMENT '(打包下载zip)显示在抄送工具栏?',
  `PrintDocLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '打印rtf按钮标签',
  `PrintDocEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `HuiQianLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '按钮标签',
  `HuiQianRole` int(0) NULL DEFAULT 0 COMMENT '会签模式',
  `AddLeaderLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '加主持人',
  `AddLeaderEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `HuiQianLeaderRole` int(0) NULL DEFAULT 0 COMMENT '组长会签规则',
  `TAlertRole` int(0) NULL DEFAULT 0 COMMENT '逾期提醒规则',
  `TAlertWay` int(0) NULL DEFAULT 0 COMMENT '逾期提醒方式',
  `WAlertRole` int(0) NULL DEFAULT 0 COMMENT '预警提醒规则',
  `WAlertWay` int(0) NULL DEFAULT 0 COMMENT '预警提醒方式',
  `TCent` float NULL DEFAULT NULL COMMENT '扣分(每延期1小时)',
  `CHWay` int(0) NULL DEFAULT 0 COMMENT '考核方式',
  `IsEval` int(0) NULL DEFAULT 0 COMMENT '是否工作质量考核',
  `OutTimeDeal` int(0) NULL DEFAULT 0 COMMENT '超时处理方式',
  `CCWriteTo` int(0) NULL DEFAULT 0 COMMENT '抄送数据写入规则',
  `CCIsAttr` int(0) NULL DEFAULT 0 COMMENT '按表单字段抄送',
  `CCFormAttr` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '抄送人员字段',
  `CCIsStations` int(0) NULL DEFAULT 0 COMMENT '是否启用？-按照角色抄送',
  `CCStaWay` int(0) NULL DEFAULT 0 COMMENT '抄送角色计算方式',
  `CCIsDepts` int(0) NULL DEFAULT 0 COMMENT '是否启用？-按照部门抄送',
  `CCIsEmps` int(0) NULL DEFAULT 0 COMMENT '是否启用？-按照人员抄送',
  `CCIsSQLs` int(0) NULL DEFAULT 0 COMMENT '是否启用？-按照SQL抄送',
  `CCSQL` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SQL表达式',
  `CCTitle` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '抄送标题',
  `CCDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '抄送内容(标题与内容支持变量)',
  `FWCLab` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '审核信息' COMMENT '显示标签',
  `FWCType` int(0) NULL DEFAULT 0 COMMENT '审核组件',
  `FWCNodeName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '节点意见名称',
  `FWCOpLabel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '审核' COMMENT '操作名词(审核/审阅/批示)',
  `FWCDefInfo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '默认审核信息',
  `SigantureEnabel` int(0) NULL DEFAULT 0 COMMENT '签名方式',
  `FWCIsFullInfo` int(0) NULL DEFAULT 1 COMMENT '如果用户未审核是否按照默认意见填充？',
  `FWCFields` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '审批格式字段',
  `FWCVer` int(0) NULL DEFAULT 1 COMMENT '审核意见保存规则',
  `NodeFrmID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '节点表单ID',
  `CheckField` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '签批字段',
  `CheckFieldText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '签批字段',
  `FWCView` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '审核意见立场',
  `FWCShowModel` int(0) NULL DEFAULT 1 COMMENT '显示方式',
  `FWCOrderModel` int(0) NULL DEFAULT 0 COMMENT '协作模式下操作员显示顺序',
  `FWCMsgShow` int(0) NULL DEFAULT 0 COMMENT '审核意见显示方式',
  `FWC_H` float(11, 2) NULL DEFAULT 300.00 COMMENT '高度(0=100%)',
  `FWCTrackEnable` int(0) NULL DEFAULT 1 COMMENT '轨迹图是否显示？',
  `FWCListEnable` int(0) NULL DEFAULT 1 COMMENT '历史审核信息是否显示？(否,仅出现意见框)',
  `FWCIsShowAllStep` int(0) NULL DEFAULT 0 COMMENT '在轨迹表里是否显示所有的步骤？',
  `FWCIsShowTruck` int(0) NULL DEFAULT 0 COMMENT '是否显示未审核的轨迹？',
  `FWCIsShowReturnMsg` int(0) NULL DEFAULT 0 COMMENT '是否显示退回信息？',
  `ICON` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '节点ICON图片路径',
  `NodeWorkType` int(0) NULL DEFAULT 0 COMMENT '节点类型',
  `FrmAttr` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'FrmAttr',
  `SFSta` int(0) NULL DEFAULT 0 COMMENT '父子流程组件',
  `SubFlowX` int(0) NULL DEFAULT 0 COMMENT '子流程设计器位置X',
  `SubFlowY` int(0) NULL DEFAULT 0 COMMENT '子流程设计器位置Y',
  `TimeLimit` float(11, 2) NULL DEFAULT 2.00 COMMENT '限期(天)',
  `TWay` int(0) NULL DEFAULT 0 COMMENT '时间计算方式',
  `WarningDay` float(11, 2) NULL DEFAULT 1.00 COMMENT '工作预警(天)',
  `DoOutTime` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '超时处理内容',
  `DoOutTimeCond` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '执行超时的条件',
  `Doc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '描述',
  `IsTask` int(0) NULL DEFAULT 1 COMMENT '允许分配工作否?',
  `IsExpSender` int(0) NULL DEFAULT 1 COMMENT '本节点接收人不允许包含上一步发送人',
  `DeliveryParas` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '访问规则设置',
  `SaveModel` int(0) NULL DEFAULT 0 COMMENT '保存模式',
  `IsCanDelFlow` int(0) NULL DEFAULT 0 COMMENT '是否可以删除流程',
  `TodolistModel` int(0) NULL DEFAULT 0 COMMENT '多人处理规则',
  `TeamLeaderConfirmRole` int(0) NULL DEFAULT 0 COMMENT '组长确认规则',
  `TeamLeaderConfirmDoc` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '组长确认设置内容',
  `IsRM` int(0) NULL DEFAULT 1 COMMENT '是否启用投递路径自动记忆功能?',
  `IsHandOver` int(0) NULL DEFAULT 0 COMMENT '是否可以移交',
  `BlockModel` int(0) NULL DEFAULT 0 COMMENT '阻塞模式',
  `BlockExp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '阻塞表达式',
  `BlockAlert` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '被阻塞提示信息',
  `CondModel` int(0) NULL DEFAULT 0 COMMENT '方向条件控制规则',
  `BatchRole` int(0) NULL DEFAULT 0 COMMENT '批处理',
  `FormType` int(0) NULL DEFAULT 1 COMMENT '表单类型',
  `FormUrl` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT 'http://' COMMENT '表单URL',
  `TurnToDeal` int(0) NULL DEFAULT 0 COMMENT '转向处理',
  `TurnToDealDoc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '发送后提示信息',
  `NodePosType` int(0) NULL DEFAULT 0 COMMENT '位置',
  `HisStas` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '角色',
  `HisDeptStrs` varchar(600) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '部门',
  `HisToNDs` varchar(80) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '转到的节点',
  `HisBillIDs` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '单据IDs',
  `HisSubFlows` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'HisSubFlows',
  `PTable` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '物理表',
  `GroupStaNDs` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '角色分组节点',
  `X` int(0) NULL DEFAULT 0 COMMENT 'X坐标',
  `Y` int(0) NULL DEFAULT 0 COMMENT 'Y坐标',
  `RefOneFrmTreeType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '独立表单类型',
  `AtPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'AtPara',
  `SF_H` float(11, 2) NULL DEFAULT 300.00 COMMENT '高度',
  `NodeStations` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的岗位',
  `NodeStationsT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的岗位t',
  `NodeEmps` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的人员',
  `NodeEmpsT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的人员t',
  `NodeDepts` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的部门',
  `NodeDeptsT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '绑定的部门t',
  `FrmTrackLab` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '轨迹' COMMENT '显示标签',
  `FrmTrackSta` int(0) NULL DEFAULT 0 COMMENT '组件状态',
  `FrmTrack_H` float(11, 2) NULL DEFAULT 300.00 COMMENT '高度',
  `SFLab` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '子流程' COMMENT '显示标签',
  `SFShowModel` int(0) NULL DEFAULT 0 COMMENT '显示方式',
  `SFShowCtrl` int(0) NULL DEFAULT 0 COMMENT '显示控制方式',
  `AllSubFlowOverRole` int(0) NULL DEFAULT 0 COMMENT '所有子流程结束规则',
  `SFCaption` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '启动子流程' COMMENT '连接标题',
  `SFDefInfo` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '可启动的子流程编号(多个用逗号分开)',
  `SFFields` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '审批格式字段',
  `SFOpenType` int(0) NULL DEFAULT 0 COMMENT '打开子流程显示',
  `FTCLab` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '流转自定义' COMMENT '显示标签',
  `FTCSta` int(0) NULL DEFAULT 0 COMMENT '组件状态',
  `FTCWorkModel` int(0) NULL DEFAULT 0 COMMENT '工作模式',
  `FTC_H` float(11, 2) NULL DEFAULT 300.00 COMMENT '高度',
  `SelectorModel` int(0) NULL DEFAULT 5 COMMENT '显示方式',
  `FK_SQLTemplate` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SQL模版',
  `FK_SQLTemplateText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SQL模版',
  `IsAutoLoadEmps` int(0) NULL DEFAULT 1 COMMENT '是否自动加载上一次选择的人员？',
  `IsSimpleSelector` int(0) NULL DEFAULT 0 COMMENT '是否单项选择(只能选择一个人)？',
  `IsEnableDeptRange` int(0) NULL DEFAULT 0 COMMENT '是否启用部门搜索范围限定(对使用通用人员选择器有效)？',
  `IsEnableStaRange` int(0) NULL DEFAULT 0 COMMENT '是否启用角色搜索范围限定(对使用通用人员选择器有效)？',
  `SelectorP1` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '分组参数:可以为空,比如:SELECT No,Name,ParentNo FROM  Port_Dept',
  `SelectorP2` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '操作员数据源:比如:SELECT No,Name,FK_Dept FROM  Port_Emp',
  `SelectorP3` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '默认选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID',
  `SelectorP4` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '强制选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID',
  PRIMARY KEY (`NodeID`) USING BTREE,
  INDEX `WF_Node_NodeID`(`NodeID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '抄送规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_node
-- ----------------------------
INSERT INTO `wf_node` VALUES (101, '001', '请假流程-经典表单-演示', '填写请假申请单', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 0, 0, 4, '', 1, '', 0, 100.0000, 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '发送', '', '保存', 1, '抄送', 0, '二维码', 0, '移交', 0, '删除', 0, '结束流程', 0, '查看父流程', 0, '打开公文', 0, 0, 0, '轨迹', 1, '挂起', 0, '查询', 0, '流转自定义', 0, '数据版本', 0, '数据批阅', 0, '重要性', 0, '节点时限', 0, '分配', 0, '关注', 0, '确认', 0, '列表', 1, '批量审核', 0, '备注', 0, '帮助提示', 0, '小纸条', 0, '评论', 0, '即时通讯', 0, '下一条', 0, '子线程', 0, 0, '跳转', 0, '', 0, 1, 0, 0, '催办', 1, '回滚', 1, 0, 1, 0, '退回', 0, '', 1, 0, 0, 0, 0, '', '', '打印Html', 0, 0, 0, '打印pdf', 0, 0, 0, 0, '', '打包下载', 0, 0, 0, '打印单据', 0, '会签', 0, '加主持人', 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, '', '', '', '审核信息', 0, '', '审核', '', 0, 1, '', 0, '', '', '', '', 1, 0, 0, 0.00, 1, 1, 0, 0, 0, '前台', 1, '', 0, 0, 0, 2.00, 0, 1.00, '', '', '', 1, 1, '', 0, 0, 0, 0, '', 1, 0, 0, '', '', 2, 0, 0, 'http://', 0, '', 0, '', '', '@102', '', '', '', '@101@102@104', 52, 46, '', '@IsYouLiTai=0', 300.00, '', '', '', '', '', '', '轨迹', 0, 300.00, '子流程', 0, 0, 0, '启动子流程', '', '', 0, '流转自定义', 0, 0, 300.00, 5, '', '', 1, 0, 0, 0, '', '', '', '');
INSERT INTO `wf_node` VALUES (102, '001', '请假流程-经典表单-演示', '部门经理审批', 0, 0, 0, 0, 0, 0, 0, 0, 0, '审核意见:@BMJLSP_Note', 0, 0, 4, '', 2, '', 0, 100.0000, 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '发送', '', '保存', 1, '抄送', 0, '二维码', 0, '移交', 0, '删除', 0, '结束流程', 0, '查看父流程', 0, '打开公文', 0, 0, 0, '轨迹', 1, '挂起', 0, '查询', 0, '流转自定义', 0, '数据版本', 0, '数据批阅', 0, '重要性', 0, '节点时限', 0, '分配', 0, '关注', 0, '确认', 0, '列表', 1, '批量审核', 0, '备注', 0, '帮助提示', 0, '小纸条', 0, '评论', 0, '即时通讯', 0, '下一条', 0, '子线程', 0, 0, '跳转', 0, '', 0, 1, 0, 0, '催办', 1, '回滚', 1, 0, 1, 0, '退回', 1, '', 1, 0, 0, 0, 0, '', '', '打印Html', 0, 0, 0, '打印pdf', 0, 0, 0, 0, '', '打包下载', 0, 0, 0, '打印单据', 0, '会签', 0, '加主持人', 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, '', '', '', '审核信息', 0, '', '审核', '', 0, 1, '', 0, '', '', '', '', 1, 0, 0, 0.00, 1, 1, 0, 0, 0, '审核', 0, '', 0, 0, 0, 1.00, 1, 1.00, '', '', '', 1, 1, '', 0, 0, 0, 0, '', 1, 0, 0, '', '', 2, 0, 0, 'http://', 0, '', 2, '', '', '@103', '', '', '', '@101@102@104', 244, 221, '', '@IsYouLiTai=0', 300.00, '', '', '', '', '', '', '轨迹', 0, 300.00, '子流程', 0, 0, 0, '启动子流程', '', '', 0, '流转自定义', 0, 0, 300.00, 5, '', '', 1, 0, 0, 0, '', '', '', '');
INSERT INTO `wf_node` VALUES (103, '001', '请假流程-经典表单-演示', '总经理审批', 0, 0, 0, 0, 0, 0, 0, 0, 0, '审核意见:@ZJLSP_Note', 0, 0, 14, '', 3, '', 0, 100.0000, 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '发送', '', '保存', 1, '抄送', 0, '二维码', 0, '移交', 0, '删除', 0, '结束流程', 0, '查看父流程', 0, '打开公文', 0, 0, 0, '轨迹', 1, '挂起', 0, '查询', 0, '流转自定义', 0, '数据版本', 0, '数据批阅', 0, '重要性', 0, '节点时限', 0, '分配', 0, '关注', 0, '确认', 0, '列表', 1, '批量审核', 0, '备注', 0, '帮助提示', 0, '小纸条', 0, '评论', 0, '即时通讯', 0, '下一条', 0, '子线程', 0, 0, '跳转', 0, '', 0, 1, 0, 0, '催办', 1, '回滚', 1, 0, 1, 0, '退回', 1, '', 1, 0, 0, 0, 0, '', '', '打印Html', 0, 0, 0, '打印pdf', 0, 0, 0, 0, '', '打包下载', 0, 0, 0, '打印单据', 0, '会签', 0, '加主持人', 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, '', '', '', '审核信息', 0, '', '审核', '', 0, 1, '', 1, '', '', '', '', 1, 0, 0, 0.00, 1, 1, 0, 0, 0, '审核.png', 0, '', 0, 0, 0, 1.00, 1, 1.00, '', '', '', 1, 1, '', 0, 0, 0, 0, '', 1, 0, 0, '', '', 2, 0, 0, 'http://', 0, '', 2, '@01', '@01', '@104', '', '', '', '@103', 421, 84, '', '@IsYouLiTai=0', 300.00, '01,', '总经理,', '', '', '', '', '轨迹', 0, 300.00, '子流程', 0, 0, 0, '启动子流程', '', '', 0, '流转自定义', 0, 0, 300.00, 5, '', '', 1, 0, 0, 0, '', '', '', '');
INSERT INTO `wf_node` VALUES (104, '001', '请假流程-经典表单-演示', '反馈给申请人', 0, 0, 0, 0, 0, 0, 0, 0, 0, '', 0, 0, 7, '', 4, '', 0, 100.0000, 0, 1, 0, 0, 0, 0, 0, 0, 0, '', 0, '发送', '', '保存', 1, '抄送', 0, '二维码', 0, '移交', 0, '删除', 0, '结束流程', 0, '查看父流程', 0, '打开公文', 0, 0, 0, '轨迹', 1, '挂起', 0, '查询', 0, '流转自定义', 0, '数据版本', 0, '数据批阅', 0, '重要性', 0, '节点时限', 0, '分配', 0, '关注', 0, '确认', 0, '列表', 1, '批量审核', 0, '备注', 0, '帮助提示', 0, '小纸条', 0, '评论', 0, '即时通讯', 0, '下一条', 0, '子线程', 0, 0, '跳转', 0, '', 0, 1, 0, 0, '催办', 1, '回滚', 1, 0, 1, 0, '退回', 1, '', 1, 0, 0, 0, 0, '', '', '打印Html', 0, 0, 0, '打印pdf', 0, 0, 0, 0, '', '打包下载', 0, 0, 0, '打印单据', 0, '会签', 0, '加主持人', 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, '', '', '', '审核信息', 0, '', '审核', '', 0, 1, '', 1, '', '', '', '', 1, 0, 0, 0.00, 1, 1, 0, 0, 0, '审核.png', 0, '', 0, 0, 0, 1.00, 1, 1.00, '', '', '', 1, 1, '', 0, 0, 0, 0, '', 1, 0, 0, '', '', 2, 0, 0, 'http://', 0, '', 2, '', '', '', '', '', '', '@101@102@104', 580, 190, '', '@IsYouLiTai=0', 300.00, '', '', '', '', '', '', '轨迹', 0, 300.00, '子流程', 0, 0, 0, '启动子流程', '', '', 0, '流转自定义', 0, 0, 300.00, 5, '', '', 1, 0, 0, 0, '', '', '', '');

-- ----------------------------
-- Table structure for wf_nodecancel
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodecancel`;
CREATE TABLE `wf_nodecancel`  (
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `CancelTo` int(0) NOT NULL DEFAULT 0 COMMENT '撤销到',
  PRIMARY KEY (`FK_Node`, `CancelTo`) USING BTREE,
  INDEX `WF_NodeCancelID`(`FK_Node`, `CancelTo`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '可撤销的节点' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodedept
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodedept`;
CREATE TABLE `wf_nodedept`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_NodeDept_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodeemp
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodeemp`;
CREATE TABLE `wf_nodeemp`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `FK_Emp` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到人员',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_NodeEmp_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点人员' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodereturn
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodereturn`;
CREATE TABLE `wf_nodereturn`  (
  `FK_Node` int(0) NOT NULL DEFAULT 0 COMMENT '节点',
  `ReturnTo` int(0) NOT NULL DEFAULT 0 COMMENT '退回到',
  PRIMARY KEY (`FK_Node`, `ReturnTo`) USING BTREE,
  INDEX `WF_NodeReturnID`(`FK_Node`, `ReturnTo`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '可退回的节点' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodestation
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodestation`;
CREATE TABLE `wf_nodestation`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_Station` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '工作角色',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_NodeStation_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点角色' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wf_nodestation
-- ----------------------------
INSERT INTO `wf_nodestation` VALUES ('103_01', 103, '01');

-- ----------------------------
-- Table structure for wf_nodesubflow
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodesubflow`;
CREATE TABLE `wf_nodesubflow`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '主流程编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `SubFlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子流程编号',
  `SubFlowName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子流程名称',
  `IsSubFlowGuide` int(0) NULL DEFAULT 0 COMMENT '是否启用子流程批量发起前置导航',
  `SubFlowGuideSQL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '子流程前置导航配置SQL',
  `SubFlowGuideGroup` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '分组的SQL',
  `SubFlowGuideEnNoFiled` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体No字段',
  `SubFlowGuideEnNameFiled` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '实体Name字段',
  `SubFlowStartModel` int(0) NULL DEFAULT 0 COMMENT '启动模式',
  `SubFlowShowModel` int(0) NULL DEFAULT 0 COMMENT '展现模式',
  `SubFlowHidTodolist` int(0) NULL DEFAULT 0 COMMENT '发送后是否隐藏父流程待办',
  `SubFlowType` int(0) NULL DEFAULT 2 COMMENT '子流程类型',
  `SubFlowModel` int(0) NULL DEFAULT 0 COMMENT '子流程模式',
  `SubFlowSta` int(0) NULL DEFAULT 1 COMMENT '状态',
  `ExpType` int(0) NULL DEFAULT 3 COMMENT '表达式类型',
  `CondExp` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '条件表达式',
  `YanXuToNode` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '延续到的节点',
  `YBFlowReturnRole` int(0) NULL DEFAULT 0 COMMENT '退回方式',
  `ReturnToNode` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ReturnToNodeText` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '0' COMMENT '要退回的节点',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  `SubFlowLab` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '启动文字标签',
  `ParentFlowSendNextStepRole` int(0) NULL DEFAULT 0 COMMENT '父流程自动运行到下一步规则',
  `ParentFlowOverRole` int(0) NULL DEFAULT 0 COMMENT '父流程结束规则',
  `SubFlowNodeID` int(0) NULL DEFAULT 0 COMMENT '指定子流程节点ID',
  `IsAutoSendSLSubFlowOver` int(0) NULL DEFAULT 0 COMMENT '同级子流程结束规则',
  `StartOnceOnly` int(0) NULL DEFAULT 0 COMMENT '仅能被调用1次(不能被重复调用).',
  `CompleteReStart` int(0) NULL DEFAULT 0 COMMENT '该子流程运行结束后才可以重新发起.',
  `IsEnableSpecFlowStart` int(0) NULL DEFAULT 0 COMMENT '指定的流程启动后,才能启动该子流程(请在文本框配置子流程).',
  `SpecFlowStart` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '子流程编号',
  `SpecFlowStartNote` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '备注',
  `IsEnableSpecFlowOver` int(0) NULL DEFAULT 0 COMMENT '指定的流程结束后,才能启动该子流程(请在文本框配置子流程).',
  `SpecFlowOver` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '子流程编号',
  `SpecFlowOverNote` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '备注',
  `SubFlowCopyFields` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '父流程字段对应子流程字段',
  `BackCopyRole` int(0) NULL DEFAULT 0 COMMENT '子流程结束后数据字段反填规则',
  `ParentFlowCopyFields` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '子流程字段对应父流程字段',
  `InvokeTime` int(0) NULL DEFAULT 0 COMMENT '调用时间',
  `IsEnableSQL` int(0) NULL DEFAULT 0 COMMENT '按照指定的SQL配置.',
  `SpecSQL` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT 'SQL语句',
  `IsEnableSameLevelNode` int(0) NULL DEFAULT 0 COMMENT '按照指定平级子流程节点完成后启动.',
  `SameLevelNode` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '平级子流程节点',
  `SendModel` int(0) NULL DEFAULT 0 COMMENT '自动发送方式',
  `X` int(0) NULL DEFAULT 0 COMMENT 'X',
  `Y` int(0) NULL DEFAULT 0 COMMENT 'Y',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_NodeSubFlow_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '子流程(所有类型子流程属性)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodeteam
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodeteam`;
CREATE TABLE `wf_nodeteam`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_Team` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户组',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_NodeTeam_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '节点权限组' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_nodetoolbar
-- ----------------------------
DROP TABLE IF EXISTS `wf_nodetoolbar`;
CREATE TABLE `wf_nodetoolbar`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `Title` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `ExcType` int(0) NULL DEFAULT 0 COMMENT '执行类型',
  `UrlExt` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '连接/函数',
  `Target` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '目标',
  `IsMyFlow` int(0) NULL DEFAULT 0 COMMENT '工作处理器',
  `IsMyTree` int(0) NULL DEFAULT 0 COMMENT '流程树',
  `IsMyView` int(0) NULL DEFAULT 0 COMMENT '工作查看器',
  `IsMyCC` int(0) NULL DEFAULT 0 COMMENT '抄送工具栏',
  `IconPath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'ICON路径',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序',
  `MyFileName` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '图标',
  `MyFilePath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFilePath',
  `MyFileExt` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'MyFileExt',
  `WebPath` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'WebPath',
  `MyFileH` int(0) NULL DEFAULT 0 COMMENT 'MyFileH',
  `MyFileW` int(0) NULL DEFAULT 0 COMMENT 'MyFileW',
  `MyFileSize` float NULL DEFAULT NULL COMMENT 'MyFileSize',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `WF_NodeToolbar_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '自定义工具栏' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_part
-- ----------------------------
DROP TABLE IF EXISTS `wf_part`;
CREATE TABLE `wf_part`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `PartType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `Tag0` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag0',
  `Tag1` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag1',
  `Tag2` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag2',
  `Tag3` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag3',
  `Tag4` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag4',
  `Tag5` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag5',
  `Tag6` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag6',
  `Tag7` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag7',
  `Tag8` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag8',
  `Tag9` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag9',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_Part_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '配件' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_powermodel
-- ----------------------------
DROP TABLE IF EXISTS `wf_powermodel`;
CREATE TABLE `wf_powermodel`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `Model` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '模块',
  `PowerFlag` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '权限标识',
  `PowerFlagName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '权限标记名称',
  `PowerCtrlType` int(0) NULL DEFAULT 0 COMMENT '控制类型',
  `EmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员编号',
  `EmpName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '人员名称',
  `StaNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色编号',
  `StaName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '角色名称',
  `FlowNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `FrmID` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '表单ID',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_PowerModel_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '权限模型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_pushmsg
-- ----------------------------
DROP TABLE IF EXISTS `wf_pushmsg`;
CREATE TABLE `wf_pushmsg`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_Event` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '事件类型',
  `PushWay` int(0) NULL DEFAULT 0 COMMENT '推送方式',
  `PushDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '推送保存内容',
  `Tag` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Tag',
  `SMSPushWay` int(0) NULL DEFAULT 0 COMMENT '短消息发送方式',
  `SMSField` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '短消息字段',
  `SMSDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '短消息内容模版',
  `SMSNodes` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SMS节点s',
  `SMSPushModel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '短消息发送设置',
  `MailPushWay` int(0) NULL DEFAULT 0 COMMENT '邮件发送方式',
  `MailAddress` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮件字段',
  `MailTitle` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮件标题模版',
  `MailDoc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '邮件内容模版',
  `MailNodes` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Mail节点s',
  `BySQL` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '按照SQL计算',
  `ByEmps` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送给指定的人员',
  `AtPara` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_PushMsg_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '消息推送' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_rememberme
-- ----------------------------
DROP TABLE IF EXISTS `wf_rememberme`;
CREATE TABLE `wf_rememberme`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点',
  `FK_Emp` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '当前操作人员',
  `Objs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '分配人员',
  `ObjsExt` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '分配人员Ext',
  `Emps` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '所有的工作人员',
  `EmpsExt` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '工作人员Ext',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_RememberMe_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '记忆我' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_returnwork
-- ----------------------------
DROP TABLE IF EXISTS `wf_returnwork`;
CREATE TABLE `wf_returnwork`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT 'WorkID',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `ReturnNode` int(0) NULL DEFAULT 0 COMMENT '退回节点',
  `ReturnNodeName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回节点名称',
  `Returner` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回人',
  `ReturnerName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回人名称',
  `ReturnToNode` int(0) NULL DEFAULT 0 COMMENT '退回到的节点',
  `ReturnToEmp` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '退回给',
  `BeiZhu` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '退回原因',
  `RDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '退回日期',
  `IsBackTracking` int(0) NULL DEFAULT 0 COMMENT '是否要原路返回',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_ReturnWork_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '退回轨迹' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_selectaccper
-- ----------------------------
DROP TABLE IF EXISTS `wf_selectaccper`;
CREATE TABLE `wf_selectaccper`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '接受人节点',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT 'WorkID',
  `FK_Emp` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FK_Emp',
  `EmpName` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'EmpName',
  `DeptName` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门名称',
  `AccType` int(0) NULL DEFAULT 0 COMMENT '类型(@0=接受人@1=抄送人)',
  `Rec` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '记录人',
  `Info` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '办理意见信息',
  `IsRemember` int(0) NULL DEFAULT 0 COMMENT '以后发送是否按本次计算',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号(可以用于流程队列审核模式)',
  `Tag` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '维度信息Tag',
  `TimeLimit` int(0) NULL DEFAULT 0 COMMENT '时限-天',
  `TSpanHour` float NULL DEFAULT NULL COMMENT '时限-小时',
  `ADT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到达日期(计划)',
  `SDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '应完成日期(计划)',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_SelectAccper_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '选择接受/抄送人信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_sqltemplate
-- ----------------------------
DROP TABLE IF EXISTS `wf_sqltemplate`;
CREATE TABLE `wf_sqltemplate`  (
  `No` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '编号',
  `SQLType` int(0) NULL DEFAULT 0 COMMENT '模版SQL类型',
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'SQL说明',
  `Docs` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'SQL模版',
  PRIMARY KEY (`No`) USING BTREE,
  INDEX `WF_SQLTemplate_No`(`No`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = 'SQL模板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_syncdata
-- ----------------------------
DROP TABLE IF EXISTS `wf_syncdata`;
CREATE TABLE `wf_syncdata`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `SyncType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '同步类型',
  `SyncTypeT` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '同步类型',
  `Note` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '备注/说明',
  `APIUrl` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'APIUrl',
  `DBSrc` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '数据库链接ID',
  `SQLTables` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '查询表的集合SQL',
  `SQLFields` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '查询表字段的SQL',
  `AtPara` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT 'AtPara',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_SyncData_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程数据同步' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_syncdatafield
-- ----------------------------
DROP TABLE IF EXISTS `wf_syncdatafield`;
CREATE TABLE `wf_syncdatafield`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `RefPKVal` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '关键内容',
  `AttrKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '业务字段',
  `AttrName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段名',
  `AttrType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `ToFieldKey` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '同步到字段',
  `ToFieldName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '字段名',
  `ToFieldType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型',
  `IsSync` int(0) NULL DEFAULT 0 COMMENT '同步?',
  `TurnFunc` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '转换函数',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_SyncDataField_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程数据同步' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_task
-- ----------------------------
DROP TABLE IF EXISTS `wf_task`;
CREATE TABLE `wf_task`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `FK_Flow` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程编号',
  `Starter` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人',
  `ToNode` int(0) NULL DEFAULT 0 COMMENT '到达的节点',
  `ToEmps` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到达人员',
  `Paras` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '参数',
  `TaskSta` int(0) NULL DEFAULT 0 COMMENT '任务状态',
  `Msg` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '消息',
  `StartDT` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起时间',
  `RDT` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '插入数据时间',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_Task_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '任务' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_transfercustom
-- ----------------------------
DROP TABLE IF EXISTS `wf_transfercustom`;
CREATE TABLE `wf_transfercustom`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT 'WorkID',
  `FK_Node` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `NodeName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `Worker` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '处理人(多个人用逗号分开)',
  `WorkerName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '处理人(多个人用逗号分开)',
  `SubFlowNo` varchar(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '要经过的子流程编号',
  `PlanDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '计划完成日期',
  `TodolistModel` int(0) NULL DEFAULT 0 COMMENT '多人工作处理模式',
  `IsEnable` int(0) NULL DEFAULT 0 COMMENT '是否启用',
  `Idx` int(0) NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_TransferCustom_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '自定义运行路径' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_workflowdeletelog
-- ----------------------------
DROP TABLE IF EXISTS `wf_workflowdeletelog`;
CREATE TABLE `wf_workflowdeletelog`  (
  `OID` int(0) NOT NULL COMMENT 'OID',
  `FID` int(0) NULL DEFAULT 0 COMMENT 'FID',
  `FK_Dept` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '部门',
  `Title` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `FlowStarter` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起人',
  `FlowStartRDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发起时间',
  `FK_Flow` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '流程',
  `FlowEnderRDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '最后处理时间',
  `FlowEndNode` int(0) NULL DEFAULT 0 COMMENT '停留节点',
  `FlowDaySpan` float NULL DEFAULT NULL COMMENT '流程时长(天)',
  `FlowEmps` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '参与人',
  `Oper` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '删除人员',
  `OperDept` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '删除人员部门',
  `OperDeptName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '删除人员名称',
  `DeleteNote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '删除原因',
  `DeleteDT` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '删除日期',
  PRIMARY KEY (`OID`) USING BTREE,
  INDEX `WF_WorkFlowDeleteLog_OID`(`OID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '流程删除日志' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wf_workopt
-- ----------------------------
DROP TABLE IF EXISTS `wf_workopt`;
CREATE TABLE `wf_workopt`  (
  `MyPK` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '主键MyPK',
  `WorkID` int(0) NULL DEFAULT 0 COMMENT '工作ID',
  `NodeID` int(0) NULL DEFAULT 0 COMMENT '节点ID',
  `ToNodeID` int(0) NULL DEFAULT 0 COMMENT '到达的节点ID',
  `EmpNo` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '操作员',
  `FlowNo` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'FlowNo',
  `SendEmps` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到人员',
  `SendEmpsT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到人员',
  `SendDepts` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到部门',
  `SendDeptsT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到部门',
  `SendStas` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到角色',
  `SendStasT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送到角色',
  `SendNote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '小纸条',
  `CCEmps` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到人员',
  `CCEmpsT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到人员',
  `CCDepts` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到部门',
  `CCDeptsT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到部门',
  `CCStas` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到角色',
  `CCStasT` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '抄送到部门',
  `CCNote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '抄送说明',
  `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标题',
  `NodeName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '当前节点',
  `ToNodeName` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '到达节点',
  `TodoEmps` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '当前处理人',
  `SenderName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送人',
  `SendRDT` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '发送日期',
  `SendSDT` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '限期',
  PRIMARY KEY (`MyPK`) USING BTREE,
  INDEX `WF_WorkOpt_MyPK`(`MyPK`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '工作处理器' ROW_FORMAT = Dynamic;

-- ----------------------------
-- View structure for v_flowstarterbpm
-- ----------------------------
DROP VIEW IF EXISTS `v_flowstarterbpm`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_flowstarterbpm` AS select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`c`.`FK_Emp` AS `FK_Emp`,`c`.`OrgNo` AS `OrgNo` from ((`wf_node` `a` join `wf_nodestation` `b`) join `port_deptempstation` `c`) where ((`a`.`NodePosType` = 0) and (`a`.`NodeID` = `b`.`FK_Node`) and (`b`.`FK_Station` = `c`.`FK_Station`) and ((`a`.`DeliveryWay` = 0) or (`a`.`DeliveryWay` = 14))) union select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`c`.`FK_Emp` AS `FK_Emp`,`c`.`OrgNo` AS `OrgNo` from ((`wf_node` `a` join `wf_nodedept` `b`) join `port_deptemp` `c`) where ((`a`.`NodePosType` = 0) and (`a`.`NodeID` = `b`.`FK_Node`) and (`b`.`FK_Dept` = `c`.`FK_Dept`) and (`a`.`DeliveryWay` = 1)) union select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`b`.`FK_Emp` AS `FK_Emp`,`c`.`OrgNo` AS `OrgNo` from ((`wf_node` `a` join `wf_nodeemp` `b`) join `port_emp` `c`) where ((`a`.`NodePosType` = 0) and (`a`.`NodeID` = `b`.`FK_Node`) and (`a`.`DeliveryWay` = 3) and (`b`.`FK_Emp` = `c`.`No`)) union select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`b`.`No` AS `FK_Emp`,`b`.`OrgNo` AS `OrgNo` from ((`wf_node` `a` join `port_emp` `b`) join `wf_flow` `c`) where ((`a`.`NodePosType` = 0) and (`a`.`DeliveryWay` = 4) and (`a`.`FK_Flow` = `c`.`No`) and ((`b`.`OrgNo` = `c`.`OrgNo`) or ((`b`.`OrgNo` is null) and (`c`.`OrgNo` is null)))) union select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`e`.`FK_Emp` AS `FK_Emp`,`e`.`OrgNo` AS `OrgNo` from (((`wf_node` `a` join `wf_nodedept` `b`) join `wf_nodestation` `c`) join `port_deptempstation` `e`) where ((`a`.`NodePosType` = 0) and (`a`.`NodeID` = `b`.`FK_Node`) and (`a`.`NodeID` = `c`.`FK_Node`) and (`b`.`FK_Dept` = `e`.`FK_Dept`) and (`c`.`FK_Station` = `e`.`FK_Station`) and (`a`.`DeliveryWay` = 9)) union select `a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`c`.`No` AS `FK_Emp`,`b`.`OrgNo` AS `OrgNo` from ((`wf_node` `a` join `wf_floworg` `b`) join `port_emp` `c`) where ((`a`.`FK_Flow` = `b`.`FlowNo`) and ((`b`.`OrgNo` = `c`.`OrgNo`) or ((`b`.`OrgNo` is null) and (`c`.`OrgNo` is null))) and ((`a`.`DeliveryWay` = 22) or (`a`.`DeliveryWay` = 51)));

-- ----------------------------
-- View structure for v_myflowdata
-- ----------------------------
DROP VIEW IF EXISTS `v_myflowdata`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_myflowdata` AS select `a`.`WorkID` AS `WorkID`,`a`.`FID` AS `FID`,`a`.`FK_FlowSort` AS `FK_FlowSort`,`a`.`SysType` AS `SysType`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`Title` AS `Title`,`a`.`WFSta` AS `WFSta`,`a`.`WFState` AS `WFState`,`a`.`Starter` AS `Starter`,`a`.`StarterName` AS `StarterName`,`a`.`Sender` AS `Sender`,`a`.`RDT` AS `RDT`,`a`.`HungupTime` AS `HungupTime`,`a`.`SendDT` AS `SendDT`,`a`.`FK_Node` AS `FK_Node`,`a`.`NodeName` AS `NodeName`,`a`.`FK_Dept` AS `FK_Dept`,`a`.`DeptName` AS `DeptName`,`a`.`PRI` AS `PRI`,`a`.`SDTOfNode` AS `SDTOfNode`,`a`.`SDTOfFlow` AS `SDTOfFlow`,`a`.`SDTOfFlowWarning` AS `SDTOfFlowWarning`,`a`.`PFlowNo` AS `PFlowNo`,`a`.`PWorkID` AS `PWorkID`,`a`.`PNodeID` AS `PNodeID`,`a`.`PFID` AS `PFID`,`a`.`PEmp` AS `PEmp`,`a`.`GuestNo` AS `GuestNo`,`a`.`GuestName` AS `GuestName`,`a`.`BillNo` AS `BillNo`,`a`.`TodoEmps` AS `TodoEmps`,`a`.`TodoEmpsNum` AS `TodoEmpsNum`,`a`.`TaskSta` AS `TaskSta`,`a`.`AtPara` AS `AtPara`,`a`.`Emps` AS `Emps`,`a`.`GUID` AS `GUID`,`a`.`FK_NY` AS `FK_NY`,`a`.`WeekNum` AS `WeekNum`,`a`.`TSpan` AS `TSpan`,`a`.`TodoSta` AS `TodoSta`,`a`.`Domain` AS `Domain`,`a`.`PrjNo` AS `PrjNo`,`a`.`PrjName` AS `PrjName`,`a`.`OrgNo` AS `OrgNo`,`a`.`FlowNote` AS `FlowNote`,`a`.`LostTimeHH` AS `LostTimeHH`,`b`.`EmpNo` AS `MyEmpNo` from (`wf_generworkflow` `a` join `wf_powermodel` `b`) where ((`a`.`FK_Flow` = `b`.`FlowNo`) and (`b`.`PowerCtrlType` = 1) and (`a`.`WFState` >= 2)) union select `a`.`WorkID` AS `WorkID`,`a`.`FID` AS `FID`,`a`.`FK_FlowSort` AS `FK_FlowSort`,`a`.`SysType` AS `SysType`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`Title` AS `Title`,`a`.`WFSta` AS `WFSta`,`a`.`WFState` AS `WFState`,`a`.`Starter` AS `Starter`,`a`.`StarterName` AS `StarterName`,`a`.`Sender` AS `Sender`,`a`.`RDT` AS `RDT`,`a`.`HungupTime` AS `HungupTime`,`a`.`SendDT` AS `SendDT`,`a`.`FK_Node` AS `FK_Node`,`a`.`NodeName` AS `NodeName`,`a`.`FK_Dept` AS `FK_Dept`,`a`.`DeptName` AS `DeptName`,`a`.`PRI` AS `PRI`,`a`.`SDTOfNode` AS `SDTOfNode`,`a`.`SDTOfFlow` AS `SDTOfFlow`,`a`.`SDTOfFlowWarning` AS `SDTOfFlowWarning`,`a`.`PFlowNo` AS `PFlowNo`,`a`.`PWorkID` AS `PWorkID`,`a`.`PNodeID` AS `PNodeID`,`a`.`PFID` AS `PFID`,`a`.`PEmp` AS `PEmp`,`a`.`GuestNo` AS `GuestNo`,`a`.`GuestName` AS `GuestName`,`a`.`BillNo` AS `BillNo`,`a`.`TodoEmps` AS `TodoEmps`,`a`.`TodoEmpsNum` AS `TodoEmpsNum`,`a`.`TaskSta` AS `TaskSta`,`a`.`AtPara` AS `AtPara`,`a`.`Emps` AS `Emps`,`a`.`GUID` AS `GUID`,`a`.`FK_NY` AS `FK_NY`,`a`.`WeekNum` AS `WeekNum`,`a`.`TSpan` AS `TSpan`,`a`.`TodoSta` AS `TodoSta`,`a`.`Domain` AS `Domain`,`a`.`PrjNo` AS `PrjNo`,`a`.`PrjName` AS `PrjName`,`a`.`OrgNo` AS `OrgNo`,`a`.`FlowNote` AS `FlowNote`,`a`.`LostTimeHH` AS `LostTimeHH`,`c`.`No` AS `MyEmpNo` from (((`wf_generworkflow` `a` join `wf_powermodel` `b`) join `port_emp` `c`) join `port_deptempstation` `d`) where ((`a`.`FK_Flow` = `b`.`FlowNo`) and (`b`.`PowerCtrlType` = 0) and (`c`.`No` = `d`.`FK_Emp`) and (`b`.`StaNo` = `d`.`FK_Station`) and (`a`.`WFState` >= 2));

-- ----------------------------
-- View structure for v_wf_authtodolist
-- ----------------------------
DROP VIEW IF EXISTS `v_wf_authtodolist`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_wf_authtodolist` AS select `b`.`FK_Emp` AS `Auther`,`b`.`FK_EmpText` AS `AuthName`,`a`.`PWorkID` AS `PWorkID`,`a`.`FK_Node` AS `FK_Node`,`a`.`FID` AS `FID`,`a`.`WorkID` AS `WorkID`,`c`.`AutherToEmpNo` AS `AutherToEmpNo`,`c`.`TakeBackDT` AS `TakeBackDT`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`Title` AS `Title` from ((`wf_generworkflow` `a` join `wf_generworkerlist` `b`) join `wf_auth` `c`) where ((`a`.`WorkID` = `b`.`WorkID`) and (`c`.`AuthType` = 1) and (`b`.`FK_Emp` = `c`.`Auther`) and (`b`.`IsPass` = 0) and (`b`.`IsEnable` = 1) and (`a`.`FK_Node` = `b`.`FK_Node`) and (`a`.`WFState` >= 2)) union select `b`.`FK_Emp` AS `Auther`,`b`.`FK_EmpText` AS `AuthName`,`a`.`PWorkID` AS `PWorkID`,`a`.`FK_Node` AS `FK_Node`,`a`.`FID` AS `FID`,`a`.`WorkID` AS `WorkID`,`c`.`AutherToEmpNo` AS `AutherToEmpNo`,`c`.`TakeBackDT` AS `TakeBackDT`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`Title` AS `Title` from ((`wf_generworkflow` `a` join `wf_generworkerlist` `b`) join `wf_auth` `c`) where ((`a`.`WorkID` = `b`.`WorkID`) and (`c`.`AuthType` = 2) and (`b`.`FK_Emp` = `c`.`Auther`) and (`b`.`IsPass` = 0) and (`b`.`IsEnable` = 1) and (`a`.`FK_Node` = `b`.`FK_Node`) and (`a`.`WFState` >= 2) and (`a`.`FK_Flow` = `c`.`FlowNo`));

-- ----------------------------
-- View structure for wf_empworks
-- ----------------------------
DROP VIEW IF EXISTS `wf_empworks`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `wf_empworks` AS select `a`.`PRI` AS `PRI`,`a`.`WorkID` AS `WorkID`,`b`.`IsRead` AS `IsRead`,`a`.`Starter` AS `Starter`,`a`.`StarterName` AS `StarterName`,`a`.`WFState` AS `WFState`,`a`.`FK_Dept` AS `FK_Dept`,`a`.`DeptName` AS `DeptName`,`b`.`FK_Dept` AS `TodoEmpDeptNo`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`PWorkID` AS `PWorkID`,`a`.`PFlowNo` AS `PFlowNo`,`b`.`FK_Node` AS `FK_Node`,`b`.`FK_NodeText` AS `NodeName`,`a`.`Title` AS `Title`,`a`.`RDT` AS `RDT`,`b`.`RDT` AS `ADT`,`b`.`SDT` AS `SDT`,`b`.`FK_Emp` AS `FK_Emp`,`b`.`FID` AS `FID`,`a`.`FK_FlowSort` AS `FK_FlowSort`,`a`.`SysType` AS `SysType`,`a`.`SDTOfNode` AS `SDTOfNode`,`b`.`PressTimes` AS `PressTimes`,`a`.`GuestNo` AS `GuestNo`,`a`.`GuestName` AS `GuestName`,`a`.`BillNo` AS `BillNo`,`a`.`TodoEmps` AS `TodoEmps`,`a`.`TodoEmpsNum` AS `TodoEmpsNum`,`a`.`TodoSta` AS `TodoSta`,`a`.`TaskSta` AS `TaskSta`,0 AS `ListType`,`a`.`Sender` AS `Sender`,`a`.`AtPara` AS `AtPara`,`a`.`Domain` AS `Domain`,`a`.`OrgNo` AS `OrgNo`,`c`.`Idx` AS `FlowIdx`,`d`.`Idx` AS `FlowSortIdx` from (((`wf_generworkflow` `a` join `wf_generworkerlist` `b`) join `wf_flow` `c`) join `wf_flowsort` `d`) where ((`b`.`IsEnable` = 1) and (`b`.`IsPass` = 0) and (`a`.`WorkID` = `b`.`WorkID`) and (`a`.`FK_Node` = `b`.`FK_Node`) and (`a`.`WFState` <> 0) and (`b`.`WhoExeIt` <> 1) and (`a`.`FK_Flow` = `c`.`No`) and (`a`.`FK_FlowSort` = `d`.`No`) and (`c`.`FK_FlowSort` = `d`.`No`)) union select `a`.`PRI` AS `PRI`,`a`.`WorkID` AS `WorkID`,`b`.`Sta` AS `IsRead`,`a`.`Starter` AS `Starter`,`a`.`StarterName` AS `StarterName`,2 AS `WFState`,`a`.`FK_Dept` AS `FK_Dept`,`a`.`DeptName` AS `DeptName`,'' AS `TodoEmpDeptNo`,`a`.`FK_Flow` AS `FK_Flow`,`a`.`FlowName` AS `FlowName`,`a`.`PWorkID` AS `PWorkID`,`a`.`PFlowNo` AS `PFlowNo`,`b`.`FK_Node` AS `FK_Node`,`b`.`NodeName` AS `NodeName`,`a`.`Title` AS `Title`,`a`.`RDT` AS `RDT`,`b`.`RDT` AS `ADT`,`b`.`RDT` AS `SDT`,`b`.`CCTo` AS `FK_Emp`,`b`.`FID` AS `FID`,`a`.`FK_FlowSort` AS `FK_FlowSort`,`a`.`SysType` AS `SysType`,`a`.`SDTOfNode` AS `SDTOfNode`,0 AS `PressTimes`,`a`.`GuestNo` AS `GuestNo`,`a`.`GuestName` AS `GuestName`,`a`.`BillNo` AS `BillNo`,`a`.`TodoEmps` AS `TodoEmps`,`a`.`TodoEmpsNum` AS `TodoEmpsNum`,0 AS `TodoSta`,0 AS `TaskSta`,1 AS `ListType`,`b`.`Rec` AS `Sender`,((0 <> '@IsCC=1') or (0 <> `a`.`AtPara`)) AS `AtPara`,`a`.`Domain` AS `Domain`,`a`.`OrgNo` AS `OrgNo`,`c`.`Idx` AS `FlowIdx`,`d`.`Idx` AS `FlowSortIdx` from (((`wf_generworkflow` `a` join `wf_cclist` `b`) join `wf_flow` `c`) join `wf_flowsort` `d`) where ((`a`.`WorkID` = `b`.`WorkID`) and (`b`.`Sta` <= 1) and (`b`.`InEmpWorks` = 1) and (`a`.`WFState` <> 0) and (`a`.`FK_Flow` = `c`.`No`) and (`a`.`FK_FlowSort` = `d`.`No`) and (`c`.`FK_FlowSort` = `d`.`No`));

SET FOREIGN_KEY_CHECKS = 1;
