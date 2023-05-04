/****** 对象:  View WF_EmpWorks    脚本日期: 03/12/2011 21:42:50 ******/;

/*  WF_EmpWorks  */;
 CREATE  VIEW  WF_EmpWorks
(PRI,WorkID,IsRead,Starter,StarterName,WFState, FK_Dept,DeptName,
FK_Flow,FlowName,PWorkID,PFlowNo,FK_Node,NodeName,Title,
RDT,ADT,SDT,FK_Emp,FID,FK_FlowSort,SysType,SDTOfNode,PressTimes,
GuestNo,GuestName,BillNo,TodoEmps,TodoEmpsNum,TodoSta,TaskSta,FlowNote,
ListType,Sender,AtPara,Domain,OrgNo,FlowIdx,FlowSortIdx)
AS

SELECT A.PRI,A.WorkID,B.IsRead, A.Starter,
A.StarterName,
A.WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.FK_Node, B.FK_NodeText AS NodeName,  A.Title, A.RDT, B.RDT AS ADT, 
B.SDT, B.FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode,B.PressTimes,
A.GuestNo,
A.GuestName,A.BillNo,A.TodoEmps,A.TodoEmpsNum,A.TodoSta,
A.TaskSta,A.FlowNote,0 as ListType,A.Sender,A.AtPara,
A.Domain,A.OrgNo,C.Idx AS FlowIdx, D.Idx AS FlowSortIdx
FROM  WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Flow C,WF_FlowSort D
WHERE     (B.IsEnable = 1) AND (B.IsPass = 0)
 AND A.WorkID = B.WorkID AND A.FK_Node = B.FK_Node AND A.WFState!=0 AND WhoExeIt!=1 AND A.FK_Flow=C.No AND A.FK_FlowSort=D.No AND C.FK_FlowSort=D.No
 UNION
SELECT A.PRI,A.WorkID,B.Sta AS IsRead, A.Starter,
A.StarterName,
2 AS WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.FK_Node, B.NodeName,   A.Title, A.RDT, B.RDT AS ADT, 
B.RDT AS SDT, B.CCTo as FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode, 0 as PressTimes,
A.GuestNo,
A.GuestName,A.BillNo,A.TodoEmps,A.TodoEmpsNum,
0 as TodoSta,
0 AS TaskSta,
A.FlowNote,
1 as ListType,
B.Rec as Sender,
'@IsCC=1'+A.AtPara as AtPara,
A.Domain,A.OrgNo,C.Idx AS FlowIdx, D.Idx AS FlowSortIdx
  FROM WF_GenerWorkFlow A, WF_CCList B,WF_Flow C,WF_FlowSort D 
  WHERE A.WorkID=B.WorkID AND  B.Sta <=1 AND B.InEmpWorks = 1 AND A.WFState!=0 AND A.FK_Flow=C.No AND A.FK_FlowSort=D.No AND C.FK_FlowSort=D.No ;
  
/****** 对象:  View V_FlowStarterBPM 脚本日期:  2015-04-10 ******/;
/*  V_FlowStarterBPM 
-- 按绑定岗位.
-- 按绑定部门的人员.
-- 按绑定人员的人员.
  */;

CREATE VIEW V_FlowStarterBPM (FK_Flow,FlowName,FK_Emp,OrgNo)
AS 
SELECT
	A.FK_Flow,
	a.FlowName,
	C.FK_Emp,
	C.OrgNo 
FROM
	WF_Node a,
	WF_NodeStation b,
	Port_DeptEmpStation c 
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND B.FK_Station= C.FK_Station 	 
	AND ( A.DeliveryWay= 0 OR A.DeliveryWay= 14 ) UNION
SELECT
	A.FK_Flow,
	a.FlowName,
	C.FK_Emp,
	C.OrgNo 
FROM
	WF_Node a,
	WF_NodeDept b,
	Port_DeptEmp c 
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND B.FK_Dept= C.FK_Dept 
	AND A.DeliveryWay= 1 UNION
SELECT
	A.FK_Flow,
	a.FlowName,
	B.FK_Emp,
	C.OrgNo
FROM
	WF_Node A,
	WF_NodeEmp B,
	Port_Emp C 
WHERE
	A.NodePosType= 0 
	AND A.NodeID= B.FK_Node 
	AND A.DeliveryWay= 3 
	AND B.FK_Emp=C.No 
	UNION
SELECT
	A.FK_Flow,
	A.FlowName,
	B.No AS FK_Emp,
	B.OrgNo 
FROM
	WF_Node A,
	Port_Emp B,
	WF_Flow C
WHERE
	A.NodePosType= 0 	AND A.DeliveryWay= 4 
			AND A.FK_Flow= C.No
		AND ((B.OrgNo = C.OrgNo) OR ((B.OrgNo IS NULL) AND (C.OrgNo IS NULL)))

	UNION
SELECT
	A.FK_Flow,
	a.FlowName,
	E.FK_Emp,
	E.OrgNo 
FROM
	WF_Node A,
	WF_NodeDept B,
	WF_NodeStation C,
	Port_DeptEmpStation E 
WHERE
	a.NodePosType= 0 
	AND A.NodeID= B.FK_Node 
	AND A.NodeID= C.FK_Node 
	AND B.FK_Dept= E.FK_Dept 
	AND C.FK_Station= E.FK_Station 
	AND A.DeliveryWay= 9 UNION
SELECT
	A.FK_Flow,
	A.FlowName,
	C.No AS FK_Emp,
	B.OrgNo 
FROM
	WF_Node A,
	WF_FlowOrg B,
	Port_Emp C 
WHERE
	A.FK_Flow= B.FlowNo 
	 AND ((B.OrgNo = C.OrgNo) OR ((B.OrgNo IS NULL) AND (C.OrgNo IS NULL)))
	AND (
	A.DeliveryWay= 22 
	OR A.DeliveryWay= 51);