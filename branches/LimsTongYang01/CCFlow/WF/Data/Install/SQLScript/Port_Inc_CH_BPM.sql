-- 执行删除.
DELETE FROM Port_Dept;
DELETE FROM Port_Station;
DELETE FROM Port_Emp;
DELETE FROM Port_DeptEmpStation;
DELETE FROM Port_DeptEmp;
DELETE FROM Port_StationType;

-- 职务 ;
DELETE FROM Port_Duty;
INSERT INTO Port_Duty (No,Name) VALUES('01','董事长') ;
INSERT INTO Port_Duty (No,Name) VALUES('02','总经理');
INSERT INTO Port_Duty (No,Name) VALUES('03','科长');
INSERT INTO Port_Duty (No,Name) VALUES('04','科员');
INSERT INTO Port_Duty (No,Name) VALUES('05','分公司总经理');
INSERT INTO Port_Duty (No,Name) VALUES('20','其他');

 
 
-- Port_Dept ;
DELETE FROM Port_Dept;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('100','集团总部','0','zhoupeng',0)   ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1001','集团市场部','100','zhanghaicheng',1)  ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1002','集团研发部','100','qifenglin',1)  ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1003','集团服务部','100','zhanghaicheng',1)  ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1004','集团财务部','100','yangyilei',1)  ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1005','集团人力资源部','100','liping',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1060','南方分公司','100','wangwenying',0) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1061',   '市场部','1060','ranqingxin',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1062',    '财务部','1060','randun',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1063',    '销售部','1060','randun',1) ;

INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1070','北方分公司','100','lining',0) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1071','市场部','1070','lichao',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1072','财务部','1070','linyangyang',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1073','销售部','1070','tianyi',1) ;
INSERT INTO Port_Dept (No,Name,ParentNo,Leader,IsDir) VALUES('1099','外来单位','100','Guest',1)  ;


-- Port_StationType ;
DELETE FROM Port_StationType;
INSERT INTO Port_StationType (No,Name) VALUES('1','高层');
INSERT INTO Port_StationType (No,Name) VALUES('2','中层');
INSERT INTO Port_StationType (No,Name) VALUES('3','基层');
 
-- Port_Station ;
DELETE FROM Port_Station;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('01','总经理','1') ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('02','市场部经理','2');
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('03','研发部经理','2')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('04','客服部经理','2')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('05','财务部经理','2')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('06','人力资源部经理','2')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('07','销售人员岗','3')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('08','程序员岗','3')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('09','技术服务岗','3')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('10','出纳岗','3')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('11','人力资源助理岗','3')  ;
INSERT INTO Port_Station (No,Name,FK_StationType) VALUES('12','外来人员岗','3')  ;



-- Port_Emp ;
-- 总经理部 ;
DELETE FROM Port_Emp;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('admin','admin','123','100','01','admin','0531-82374939','zhoupeng@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('zhoupeng','周朋','123','100','02','admin','0531-82374939','zhoupeng@ccflow.org',1)  ;

-- 市场部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('zhanghaicheng','张海成','123','1001','03','zhoupeng','0531-82374939','zhanghaicheng@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('zhangyifan','张一帆','123','1001','04','zhanghaicheng','0531-82374939','zhangyifan@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('zhoushengyu','周升雨','123','1001','04','zhanghaicheng','0531-82374939','zhoushengyu@ccflow.org',1)  ;

-- 研发部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('qifenglin','祁凤林','123','1002','03','zhoupeng','0531-82374939','qifenglin@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('zhoutianjiao','周天娇','123','1002','04','qifenglin','0531-82374939','zhoutianjiao@ccflow.org',1)  ;

-- 服务部经理 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('guoxiangbin','郭祥斌','123','1003','03','zhoupeng','0531-82374939','guoxiangbin@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('fuhui','福惠','123','1003','04','guoxiangbin','0531-82374939','fuhui@ccflow.org',1)  ;

-- 财务部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('yangyilei','杨依雷','123','1004','03','zhoupeng','0531-82374939','yangyilei@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('guobaogeng','郭宝庚','123','1004','04','yangyilei','0531-82374939','guobaogeng@ccflow.org',1) ;

-- 人力资源部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('liping','李萍','123','1005','03','zhoupeng','0531-82374939','liping@ccflow.org',1)  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('liyan','李言','123','1005','04','liping','0531-82374939','liyan@ccflow.org',1)  ;

-- 外来单位人员
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,FK_Duty,Leader,Tel,Email,NumOfDept) VALUES('Guest','外来人员','123','1099','20','Guest','0531-82374939','Guest@ccflow.org',1)  ;


-- 预制部门与职务的对应.;

DELETE FROM Port_DeptDuty;

INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('100', '01');
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('100', '02');
   -- 市场部
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1001','03');  
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1001','04');  
   -- 研发部
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1002','03');  
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1002','04'); 
   -- 客服部
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1003','03');  
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1003','04');  
   -- 财务部
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1004','03');  
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1004','04');  
   -- 人力资源
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1005','03');  
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1005','04');
   -- 外来单位
INSERT INTO Port_DeptDuty (FK_Dept,FK_Duty) VALUES ('1099','20');


--==== 增加部门与岗位对应.;

DELETE FROM Port_DeptStation;

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('100', '01');
   -- 市场部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1001','02');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1001','07');  
   -- 研发部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1002','03');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1002','08'); 
   -- 客服部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1003','04');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1003','09');  
   -- 财务部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1004','05');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1004','10');  
   -- 人力资源
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1005','06');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1005','11');

-- 外来单位
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1099','12');
  
 
-- Port_DeptEmp 人员与部门的对应 ;
DELETE FROM Port_DeptEmp;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('100_zhoupeng','zhoupeng','100','02',10,'zhoupeng') ;

-- 市场部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1001_zhanghaicheng','zhanghaicheng','1001','03',20,'zhoupeng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1001_zhangyifan','zhangyifan','1001','04',20,'zhanghaicheng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1001_zhoushengyu','zhoushengyu','1001','04',20,'zhanghaicheng') ;

-- 研发部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1002_qifenglin','qifenglin','1002','03',20,'zhoupeng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1002_zhoutianjiao','zhoutianjiao','1002','04',20,'qifenglin') ;

-- 服务部经理 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1003_guoxiangbin','guoxiangbin','1003','03',20,'zhoupeng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1003_fuhui',            'fuhui','1003','04',20,'guoxiangbin') ;

-- 财务部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1004_yangyilei','yangyilei','1004','03',20,'zhoupeng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1004_guobaogeng','guobaogeng','1004','04',20,'yangyilei') ;

-- 人力资源部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1005_liping','liping','1005','03',20,'zhoupeng') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept,FK_Duty,DutyLevel,Leader) VALUES('1005_liyan','liyan','1005','04',20,'liping') ;

-- Port_DeptEmpStation 人员与岗位的对应 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('100_zhoupeng_01','100','zhoupeng','01')  ;

-- 市场部; 
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhanghaicheng_02','1001','zhanghaicheng','02')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhangyifan_07','1001','zhangyifan','07')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhoushengyu_07','1001','zhoushengyu','07')  ;

-- 研发部 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1002_qifenglin_03','1002','qifenglin','03')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1002_zhoutianjiao_08','1002','zhoutianjiao','08')  ;

--服务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1003_guoxiangbin_04','1003','guoxiangbin','04');
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1003_fuhui_09',      '1003','fuhui','09') ; 


-- 财务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1004_yangyilei_05','1004','yangyilei','05')   ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1004_guobaogeng_10','1004','guobaogeng','10')  ;

-- 人和资源部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1005_liping_06','1005','liping','06')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1005_liyan_11','1005','liyan','11')  ;

-- 外来单位人员;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1099_Guest_12','1005','Guest','12') ;

 

 