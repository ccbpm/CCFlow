-- 删除; 
DELETE FROM Port_Dept;
DELETE FROM Port_Station;
DELETE FROM Port_Emp;
DELETE FROM Port_EmpStation;
DELETE FROM Port_DeptEmp;


-- Port_Dept ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1','驰骋软件','0');
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('2','市场部','1')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('3','研发部','1')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('4','服务部','1')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('5','财务部','1')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('6','人力资源部','1');


-- Port_StationType ;
INSERT INTO Port_StationType (No,Name) VALUES('1','基层')  ;
INSERT INTO Port_StationType (No,Name) VALUES('2','中层')  ;
INSERT INTO Port_StationType (No,Name) VALUES('3','基层')  ;


-- Port_Station ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('01','总经理','1','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('02','市场部经理','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('03','研发部经理','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('04','客服部经理','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('05','财务部经理','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('06','人力资源部经理','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('07','销售人员岗','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('08','程序员岗','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('09','技术服务岗','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('10','出纳岗','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('11','人力资源助理岗','3','0')  ;

-- Port_Emp ;
-- 总经理部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('admin','admin','123','1')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoupeng','周朋','123','1')  ;

-- 市场部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhanghaicheng','张海成','123','2')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhangyifan','张一帆','123','2')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoushengyu','周升雨','123','2')  ;

-- 研发部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('qifenglin','祁凤林','123','3')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoutianjiao','周天娇','123','3')  ;

-- 服务部经理 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('guoxiangbin','郭祥斌','123','4')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('fuhui','福惠','123','4')  ;

-- 财务部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('yangyilei','杨依雷','123','5')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('guobaogeng','郭宝庚','123','5') ;

-- 人力资源部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('liping','李萍','123','6')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('liyan','李言','123','6')  ;

 
-- Port_DeptEmp 人员与部门的对应 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1_zhoupeng','zhoupeng','1') ;

-- 市场部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('2_zhanghaicheng','zhanghaicheng','2') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('2_zhangyifan','zhangyifan','2') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('2_zhoushengyu','zhoushengyu','2') ;

-- 研发部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('3_qifenglin','qifenglin','3') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('3_zhoutianjiao','zhoutianjiao','3') ;

-- 服务部经理 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('4_guoxiangbin','guoxiangbin','4') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('4_fuhui','fuhui','4') ;

-- 财务部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('5_yangyilei','yangyilei','5') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('5_guobaogeng','guobaogeng','5') ;

-- 人力资源部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('6_liping','liping','6') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('6_liyan','liyan','6') ;


-- Port_EmpStation 人员与岗位的对应 ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoupeng','01')  ;

-- 市场部;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhanghaicheng','02')  ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhangyifan','07')  ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoushengyu','07')  ;

-- 研发部 ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('qifenglin','03')  ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoutianjiao','08')  ;

--服务部;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('guoxiangbin','04');
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('fuhui','09') ; 

-- 财务部;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('yangyilei','05')   ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('guobaogeng','10')  ;

-- 人和资源部;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('liping','06')  ;
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('liyan','11')  ;


-- 部门与岗位的对应关系.
DELETE FROM  Port_DeptStation;

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('100','01');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1001','02');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1001','07');

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1002','03');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1002','08');

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1003','04');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1003','09');

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1004','05');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1004','10');

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1005','06');
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES('1005','11');