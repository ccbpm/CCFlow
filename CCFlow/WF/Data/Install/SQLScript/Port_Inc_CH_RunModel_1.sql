-- 执行删除.
DELETE FROM Port_Org;
DELETE FROM Port_OrgAdminer;
DELETE FROM Port_Dept;
DELETE FROM Port_Station;
DELETE FROM Port_Emp;
DELETE FROM Port_DeptEmpStation;
DELETE FROM Port_DeptEmp;
DELETE FROM Port_StationType;
 
-- Port_Org ;
DELETE FROM Port_Org;
INSERT INTO Port_Org (No,Name,adminer,adminername) VALUES('100','BPM服务','admin','admin');
INSERT INTO Port_Org (No,Name,adminer,adminername) VALUES('ccs','驰骋公司','yuwen','钰雯');
INSERT INTO Port_Org (No,Name,adminer,adminername) VALUES('quanyi','泉亿公司','yuwen','钰雯');


-- Port_Org ;
DELETE FROM Port_OrgAdminer;
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('100_admin','100','admin','admin');
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('ccs_yuwen','ccs','yuwen','钰雯');
INSERT INTO Port_OrgAdminer (MyPK,OrgNo,FK_Emp,EmpName) VALUES('quanyi_yuwen','quanyi','yuwen','钰雯');

 
-- Port_Dept ;
DELETE FROM Port_Dept;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('100','集团BPM服务','0','100');
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('ccs','驰骋公司','100','ccs') ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1001','市场部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1002','研发部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1003','服务部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1004','财务部','ccs','ccs')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('1005','人力资源部','ccs','ccs') ; 


INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('quanyi','泉亿公司','100','quanyi') ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('101','总经理室','quanyi','quanyi')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('102','市场部','quanyi','quanyi')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('103','研发部','quanyi','quanyi')  ;
INSERT INTO Port_Dept (No,Name,ParentNo,OrgNo) VALUES('104','服务部','quanyi','quanyi')  ;


-- Port_StationType ;
DELETE FROM Port_StationType;
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('1','高层','ccs');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('2','中层','ccs');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('3','基层','ccs');

INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('10','高层','quanyi');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('20','中层','quanyi');
INSERT INTO Port_StationType (No,Name,OrgNo) VALUES('30','基层','quanyi');
 
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

--  quanyi ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('A01','总经理','10','quanyi') ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('A02','市场部经理','10','quanyi') ;


-- Port_Emp ;
-- 总经理部 ;
DELETE FROM Port_Emp;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('admin','管理员','123','ccs','0531-82374939','admin@ccflow.org',0,'100') ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('yuwen','钰雯','123','ccs','0531-82374939','yuwen@ccflow.org',0,'ccs')  ;

-- 市场部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('zhanghaicheng','张海成','123','1001','0531-82374939','zhanghaicheng@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('zhangyifan','张一帆','123','1001','0531-82374939','zhangyifan@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('zhoushengyu','周升雨','123','1001','0531-82374939','zhoushengyu@ccflow.org',0,'ccs')  ;

-- 研发部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('qifenglin','祁凤林','123','1002','0531-82374939','qifenglin@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('zhoutianjiao','周天娇','123','1002','0531-82374939','zhoutianjiao@ccflow.org',0,'ccs')  ;

-- 服务部经理 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('guoxiangbin','郭祥斌','123','1003','0531-82374939','guoxiangbin@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('fuhui','福惠','123','1003','0531-82374939','fuhui@ccflow.org',0,'ccs')  ;

-- 财务部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('yangyilei','杨依雷','123','1004','0531-82374939','yangyilei@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('guobaogeng','郭宝庚','123','1004','0531-82374939','guobaogeng@ccflow.org',0,'ccs') ;

-- 人力资源部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('liping','李萍','123','1005','0531-82374939','liping@ccflow.org',0,'ccs')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('liyan','李言','123','1005','0531-82374939','liyan@ccflow.org',0,'ccs')  ;

-- 外来单位人员
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email,EmpSta,OrgNo) VALUES('Guest','外来人员','123','1099','0531-82374939','Guest@ccflow.org',0,'ccs')  ;

 
-- Port_DeptEmp 人员与部门的对应 yuwen担任两个公司的总经理,一人多职. ;
DELETE FROM Port_DeptEmp;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('ccs_yuwen','yuwen','ccs','驰骋公司','ccs','驰骋公司','ccs');
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_yuwen','yuwen','1001','市场部','01','总经理室','ccs');

INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('quanyi_yuwen','yuwen','quanyi','泉亿公司','quanyi','泉亿公司','quanyi');
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('101_yuwen','yuwen','101','总经理室','A01','总经理','quanyi');

-- 市场部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_zhanghaicheng','zhanghaicheng','1001','集团市场部','02','市场部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_zhangyifan','zhangyifan','1001','集团市场部','07','销售人员岗','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1001_zhoushengyu','zhoushengyu','1001','集团市场部','07','销售人员岗','ccs') ;

-- 研发部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1002_qifenglin','qifenglin','1002','集团研发部','03','研发部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1002_zhoutianjiao','zhoutianjiao','1002','集团研发部','08','程序员岗','ccs') ;

-- 服务部经理 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1003_guoxiangbin','guoxiangbin','1003','客服部经理','04','客服部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1003_fuhui',            'fuhui','1003','客服部经理','09','技术服务岗','ccs') ;

-- 财务部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1004_yangyilei','yangyilei','1004','财务部','05','财务部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1004_guobaogeng','guobaogeng','1004','财务部','10','出纳岗','ccs') ;

-- 人力资源部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1005_liping','liping','1005','集团总部','06','人力资源部经理','ccs') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,DeptName,StationNo,StationNoT,OrgNo) VALUES('1005_liyan','liyan','1005','集团总部','11','人力资源助理岗','ccs') ;

-- Port_DeptEmpStation 人员与岗位的对应 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('100_yuwen_01','100','yuwen','01','ccs')  ;

-- 市场部; 
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_zhanghaicheng_02','1001','zhanghaicheng','02','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_zhangyifan_07','1001','zhangyifan','07','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1001_zhoushengyu_07','1001','zhoushengyu','07','ccs')  ;

-- 研发部 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1002_qifenglin_03','1002','qifenglin','03','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1002_zhoutianjiao_08','1002','zhoutianjiao','08','ccs')  ;

-- 服务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1003_guoxiangbin_04','1003','guoxiangbin','04','ccs');
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1003_fuhui_09',      '1003','fuhui','09','ccs') ; 

-- 财务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1004_yangyilei_05','1004','yangyilei','05','ccs')   ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1004_guobaogeng_10','1004','guobaogeng','10','ccs')  ;

-- 人力资源部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1005_liping_06','1005','liping','06','ccs')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1005_liyan_11','1005','liyan','11','ccs')  ;

-- 外来单位人员;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station,OrgNo) VALUES('1099_Guest_12','1005','Guest','12','ccs') ;

-- ccs流程树;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('100','SAAS流程树','0','100',0) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('ccs','驰骋公司','100','ccs',1) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1001','行政办公','ccs','ccs',2) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1002','财务流程','ccs','ccs',3) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1003','项目管理','ccs','ccs',4) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('1004','其他流程','ccs','ccs',5) ;

-- ccs表单树;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('100','SAAS表单树','0','100',0) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('ccs','驰骋公司','100','ccs',1) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1001','行政办公','ccs','ccs',2) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1002','财务流程','ccs','ccs',3) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1003','项目管理','ccs','ccs',4) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('1004','其他流程','ccs','ccs',5) ;


-- quanyi 流程树;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi','泉亿公司','100','quanyi',1) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1001','行政办公','quanyi','quanyi',2) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1002','财务流程','quanyi','quanyi',3) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1003','项目管理','quanyi','quanyi',4) ;
INSERT INTO WF_FlowSort (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1004','其他流程','quanyi','quanyi',5) ;

-- quanyi 表单树;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi','泉亿公司','100','quanyi',1) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1001','行政办公','quanyi','quanyi',2) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1002','财务流程','quanyi','quanyi',3) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1003','项目管理','quanyi','quanyi',4) ;
INSERT INTO Sys_FormTree (No,Name,ParentNo,OrgNo,Idx) VALUES('quanyi1004','其他流程','quanyi','quanyi',5) ;
