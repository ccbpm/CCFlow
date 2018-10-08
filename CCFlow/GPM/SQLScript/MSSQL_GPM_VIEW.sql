--GO--
--DROP VIEW V_GPM_EmpGroup
--GO-- 
CREATE VIEW V_GPM_EmpGroup
AS
SELECT FK_Group,FK_Emp FROM GPM_GroupEmp
UNION
SELECT a.FK_Group,B.FK_Emp FROM GPM_GroupStation a, Port_DeptEmpStation b 
WHERE a.FK_Station=b.FK_Station
--GO-- 
--DROP VIEW V_GPM_EmpGroupMenu
--GO-- 
CREATE VIEW V_GPM_EmpGroupMenu
AS
SELECT a.FK_Group,a.FK_Emp,b.FK_Menu,b.IsChecked FROM V_GPM_EmpGroup a, GPM_GroupMenu b 
WHERE a.FK_Group=b.FK_Group

--GO-- 
--DROP VIEW V_GPM_EmpStationMenu
--GO-- 
CREATE VIEW V_GPM_EmpStationMenu
AS
SELECT b.FK_Station,a.FK_Emp,b.FK_Menu,b.IsChecked FROM Port_DeptEmpStation a,GPM_StationMenu b
WHERE a.FK_Station = b.FK_Station

--GO-- 
--DROP VIEW V_GPM_EmpMenu
--GO-- 
CREATE VIEW V_GPM_EmpMenu
AS
select distinct c.* from (
SELECT a.FK_Emp+'_'+a.FK_Menu as MyPK, a.FK_Emp, b.No as FK_Menu, b.* FROM 
   GPM_EmpMenu a,GPM_Menu b WHERE a.FK_Menu=b.No AND B.IsEnable=1
UNION
SELECT a.FK_Emp+'_'+a.FK_Menu as MyPK, a.FK_Emp, b.No as FK_Menu, b.* FROM 
  V_GPM_EmpGroupMenu a,GPM_Menu b  WHERE a.FK_Menu=b.No AND B.IsEnable=1
UNION
SELECT a.FK_Emp+'_'+a.FK_Menu as MyPK, a.FK_Emp, b.No as FK_Menu, b.* 
FROM V_GPM_EmpStationMenu a,GPM_Menu b WHERE a.FK_Menu=b.No AND B.IsEnable=1
) c

 