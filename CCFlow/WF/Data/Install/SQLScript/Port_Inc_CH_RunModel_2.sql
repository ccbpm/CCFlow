-- 执行删除.
DELETE FROM Port_Org;
DELETE FROM Port_OrgAdminer;
DELETE FROM Port_Dept;
DELETE FROM Port_Station;
DELETE FROM Port_Emp;
DELETE FROM Port_DeptEmpStation;
DELETE FROM Port_DeptEmp;
DELETE FROM Port_StationType;
DELETE FROM WF_FlowSort;
DELETE FROM Sys_FormTree;
 
 
-- Port_Org ;
DELETE FROM Port_Org;
INSERT INTO Port_Org (No,Name,adminer,adminername) VALUES('100','Admin组织','admin','admin');
INSERT INTO Port_Org (No,Name,adminer,adminername) VALUES('ccs','驰骋公司','ccs','驰骋管理员');


-- Port_Org ;
DELETE FROM Port_OrgAdminer;
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('100_100_admin','100','admin','admin');
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('ccs_ccs_ccs','ccs','ccs_ccs','驰骋管理员');
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('ccs_ccs_yuwen','ccs','ccs_yuwen','钰雯');

 
-- Port_Dept ;
DELETE FROM Port_Dept;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('100','BPM服务','0','100');
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('ccs','驰骋公司','100','ccs') ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1001','市场部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1002','研发部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1003','服务部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1004','财务部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1005','人力资源部','ccs','ccs') ; 


-- Port_StationType ;
DELETE FROM Port_StationType;
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('1','高层','ccs');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('2','中层','ccs');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('3','基层','ccs');
 
-- Port_Station ;
DELETE FROM Port_Station;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('01','总经理','1','ccs') ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('02','市场部经理','2','ccs');
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('03','研发部经理','2','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('04','客服部经理','2','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('05','财务部经理','2','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('06','人力资源部经理','2','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('07','销售人员岗','3','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('08','程序员岗','3','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('09','技术服务岗','3','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('10','出纳岗','3','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('11','人力资源助理岗','3','ccs')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('12','外来人员岗','3','ccs')  ;

-- Port_Emp ;
-- 总经理部 ;
DELETE FROM Port_Emp;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('admin','admin','管理员','123','100','0531-82374939','admin@ccflow.org',0,'100')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('100_admin','admin','管理员','123','100','0531-82374939','admin@ccflow.org',0,'100')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_ccs','ccs','驰骋管理员','123','ccs','0531-82374939','ccs@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_yuwen','yuwen','钰雯','123','ccs','0531-82374939','yuwen@ccflow.org',0,'ccs')  ;

-- 市场部 ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_zhanghaicheng','zhanghaicheng','张海成','123','1001','0531-82374939','zhanghaicheng@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_zhangyifan','zhangyifan','张一帆','123','1001','0531-82374939','zhangyifan@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_zhoushengyu','zhoushengyu','周升雨','123','1001','0531-82374939','zhoushengyu@ccflow.org',0,'ccs')  ;

-- 研发部 ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_qifenglin','qifenglin','祁凤林','123','1002','0531-82374939','qifenglin@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_zhoutianjiao','zhoutianjiao','周天娇','123','1002','0531-82374939','zhoutianjiao@ccflow.org',0,'ccs')  ;

-- 服务部经理 ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_guoxiangbin','guoxiangbin','郭祥斌','123','1003','0531-82374939','guoxiangbin@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_fuhui','fuhui','福惠','123','1003','0531-82374939','fuhui@ccflow.org',0,'ccs')  ;

-- 财务部 ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_yangyilei','yangyilei','杨依雷','123','1004','0531-82374939','yangyilei@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_guobaogeng','guobaogeng','郭宝庚','123','1004','0531-82374939','guobaogeng@ccflow.org',0,'ccs') ;

-- 人力资源部 ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_liping','liping','李萍','123','1005','0531-82374939','liping@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_liyan','liyan','李言','123','1005','0531-82374939','liyan@ccflow.org',0,'ccs')  ;

-- 外来单位人员
INSERT INTO Port_Emp (No,UserID,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('ccs_Guest','Guest','外来人员','123','1099','0531-82374939','Guest@ccflow.org',0,'ccs')  ;

 
-- Port_DeptEmp 人员与部门的对应 ;
DELETE FROM Port_DeptEmp;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('100_yuwen','ccs_yuwen','100','集团总部','01','总经理','ccs');

-- 市场部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_ccs_zhanghaicheng','ccs_zhanghaicheng','1001','集团市场部','02','市场部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_ccs_zhangyifan','ccs_zhangyifan','1001','集团市场部','07','销售人员岗','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_ccs_zhoushengyu','ccs_zhoushengyu','1001','集团市场部','07','销售人员岗','ccs') ;

-- 研发部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1002_ccs_qifenglin','ccs_qifenglin','1002','集团研发部','03','研发部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1002_ccs_zhoutianjiao','ccs_zhoutianjiao','1002','集团研发部','08','程序员岗','ccs') ;

-- 服务部经理 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1003_ccs_guoxiangbin','ccs_guoxiangbin','1003','客服部经理','04','客服部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1003_ccs_fuhui',            'ccs_fuhui','1003','客服部经理','09','技术服务岗','ccs') ;

-- 财务部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1004_ccs_yangyilei','ccs_yangyilei','1004','财务部','05','财务部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1004_ccs_guobaogeng','ccs_guobaogeng','1004','财务部','10','出纳岗','ccs') ;

-- 人力资源部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1005_ccs_liping','ccs_liping','1005','集团总部','06','人力资源部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1005_ccs_liyan','ccs_liyan','1005','集团总部','11','人力资源助理岗','ccs') ;

-- Port_DeptEmpStation 人员与岗位的对应 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('100_ccs_yuwen_01','ccs_100','ccs_yuwen','01','ccs')  ;

-- 市场部; 
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_ccs_zhanghaicheng_02','1001','ccs_zhanghaicheng','02','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_ccs_zhangyifan_07','1001','ccs_zhangyifan','07','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_ccs_zhoushengyu_07','1001','ccs_zhoushengyu','07','ccs')  ;

-- 研发部 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1002_ccs_qifenglin_03','1002','ccs_qifenglin','03','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1002_ccs_zhoutianjiao_08','1002','ccs_zhoutianjiao','08','ccs')  ;

-- 服务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1003_ccs_guoxiangbin_04','1003','ccs_guoxiangbin','04','ccs');
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1003_ccs_fuhui_09',      '1003','ccs_fuhui','09','ccs') ; 

-- 财务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1004_ccs_yangyilei_05','1004','ccs_yangyilei','05','ccs')   ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1004_ccs_guobaogeng_10','1004','ccs_guobaogeng','10','ccs')  ;

-- 人力资源部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1005_ccs_liping_06','1005','ccs_liping','06','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1005_ccs_liyan_11','1005','ccs_liyan','11','ccs')  ;

-- 外来单位人员;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1099_ccs_Guest_12','1005','ccs_Guest','12','ccs') ;

-- 流程树;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('100','SAAS流程树','0','100',0) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('ccs','驰骋公司','100','ccs',1) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1001','行政办公','ccs','ccs',2) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1002','财务流程','ccs','ccs',3) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1003','项目管理','ccs','ccs',4) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1004','其他流程','ccs','ccs',5) ;

-- 表单树;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('100','SAAS表单树','0','100',0) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('ccs','驰骋公司','100','ccs',1) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1001','行政办公','ccs','ccs',2) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1002','财务流程','ccs','ccs',3) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1003','项目管理','ccs','ccs',4) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1004','其他流程','ccs','ccs',5) ;






