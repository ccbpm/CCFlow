CREATE VIEW v_wf_authtodolist as 
SELECT
	b.FK_Emp AS Auther,
	b.FK_EmpText AS AuthName,
	a.PWorkID AS PWorkID,
	a.FK_Node AS FK_Node,
	a.FID AS FID,
	a.WorkID AS WorkID,
	c.AutherToEmpNo AS AutherToEmpNo,
	c.TakeBackDT AS TakeBackDT,
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	a.Title AS Title 
FROM
	wf_generworkflow a , wf_generworkerlist b , wf_auth c 
WHERE

			a.WorkID = b.WorkID 
			
		AND  c.AuthType = 1 
		AND b.FK_Emp = c.Auther 
		AND b.IsPass = 0 
		AND b.IsEnable = 1 
		AND a.FK_Node = b.FK_Node 
	AND a.WFState >= 2  UNION all 
SELECT
	b.FK_Emp AS Auther,
	b.FK_EmpText AS AuthName,
	a.PWorkID AS PWorkID,
	a.FK_Node AS FK_Node,
	a.FID AS FID,
	a.WorkID AS WorkID,
	c.AutherToEmpNo AS AutherToEmpNo,
	c.TakeBackDT AS TakeBackDT,
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	a.Title AS Title 
FROM
	wf_generworkflow a , wf_generworkerlist b , wf_auth c  
WHERE
	
			a.WorkID = b.WorkID 
			 
		AND  c.AuthType = 2 
		AND  b.FK_Emp = c.Auther  
		AND b.IsPass = 0 
		AND  b.IsEnable = 1  
		AND a.FK_Node = b.FK_Node  
	AND  a.WFState >= 2  
	AND  a.FK_Flow = c.FlowNo ;

CREATE VIEW v_myflowdata 		
as 
SELECT
	a.WorkID AS WorkID,
	a.FID AS FID,
	a.FK_FlowSort AS FK_FlowSort,
	a.SysType AS SysType,
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	a.Title AS Title,
	a.WFSta AS WFSta,
	a.WFState AS WFState,
	a.Starter AS Starter,
	a.StarterName AS StarterName,
	a.Sender AS Sender,
	a.RDT AS RDT,
	a.HungupTime AS HungupTime,
	a.SendDT AS SendDT,
	a.FK_Node AS FK_Node,
	a.NodeName AS NodeName,
	a.FK_Dept AS FK_Dept,
	a.DeptName AS DeptName,
	a.PRI AS PRI,
	a.SDTOfNode AS SDTOfNode,
	a.SDTOfFlow AS SDTOfFlow,
	a.SDTOfFlowWarning AS SDTOfFlowWarning,
	a.PFlowNo AS PFlowNo,
	a.PWorkID AS PWorkID,
	a.PNodeID AS PNodeID,
	a.PFID AS PFID,
	a.PEmp AS PEmp,
	a.GuestNo AS GuestNo,
	a.GuestName AS GuestName,
	a.BillNo AS BillNo,
	a.FlowNote AS FlowNote,
	a.TodoEmps AS TodoEmps,
	a.TodoEmpsNum AS TodoEmpsNum,
	a.TaskSta AS TaskSta,
	a.AtPara AS AtPara,
	a.Emps AS Emps,
	a.GUID AS GUID,
	a.FK_NY AS FK_NY,
	a.WeekNum AS WeekNum,
	a.TSpan AS TSpan,
	a.TodoSta AS TodoSta,
	a."Domain",
	a.PrjNo AS PrjNo,
	a.PrjName AS PrjName,
	a.OrgNo AS OrgNo,
	b.EmpNo AS MyEmpNo 
FROM
	wf_generworkflow a , wf_powermodel b  
WHERE
	
			a.FK_Flow = b.FlowNo 
			
		AND  b.PowerCtrlType = 1 
	AND a.WFState >= 2  UNION ALL 
SELECT
	a.WorkID AS WorkID,
	a.FID AS FID,
	a.FK_FlowSort AS FK_FlowSort,
	a.SysType AS SysType,
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	a.Title AS Title,
	a.WFSta AS WFSta,
	a.WFState AS WFState,
	a.Starter AS Starter,
	a.StarterName AS StarterName,
	a.Sender AS Sender,
	a.RDT AS RDT,
	a.HungupTime AS HungupTime,
	a.SendDT AS SendDT,
	a.FK_Node AS FK_Node,
	a.NodeName AS NodeName,
	a.FK_Dept AS FK_Dept,
	a.DeptName AS DeptName,
	a.PRI AS PRI,
	a.SDTOfNode AS SDTOfNode,
	a.SDTOfFlow AS SDTOfFlow,
	a.SDTOfFlowWarning AS SDTOfFlowWarning,
	a.PFlowNo AS PFlowNo,
	a.PWorkID AS PWorkID,
	a.PNodeID AS PNodeID,
	a.PFID AS PFID,
	a.PEmp AS PEmp,
	a.GuestNo AS GuestNo,
	a.GuestName AS GuestName,
	a.BillNo AS BillNo,
	a.FlowNote AS FlowNote,
	a.TodoEmps AS TodoEmps,
	a.TodoEmpsNum AS TodoEmpsNum,
	a.TaskSta AS TaskSta,
	a.AtPara AS AtPara,
	a.Emps AS Emps,
	a.GUID AS GUID,
	a.FK_NY AS FK_NY,
	a.WeekNum AS WeekNum,
	a.TSpan AS TSpan,
	a.TodoSta AS TodoSta,
	a."Domain",
	a.PrjNo AS PrjNo,
	a.PrjName AS PrjName,
	a.OrgNo AS OrgNo,
	c.No AS MyEmpNo 
FROM
	wf_generworkflow a , wf_powermodel b , port_emp c , port_deptempstation d 
WHERE
	
			a.FK_Flow = b.FlowNo 
			
		AND b.PowerCtrlType = 0 
		AND c.No = d.FK_Emp 
	AND b.StaNo = d.FK_Station 
	AND  a.WFState >= 2;
	
CREATE VIEW v_flowstarterbpm 
as 
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	c.FK_Emp AS FK_Emp,
	c.OrgNo AS OrgNo 
FROM
	wf_node a ,
	 wf_nodestation b,
	  port_deptempstation c 
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND b.FK_Station= c.FK_Station 	 
	AND ( a.DeliveryWay= 0 OR a.DeliveryWay= 14 ) UNION
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	c.FK_Emp AS FK_Emp,
	c.OrgNo AS OrgNo 
FROM
	wf_node a , 
	wf_nodedept b , 
	port_deptemp c 
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND b.FK_Dept= c.FK_Dept 
	AND a.DeliveryWay= 1 UNION
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	b.FK_Emp AS FK_Emp,
	c.OrgNo AS OrgNo 
FROM
	wf_node a ,
	 wf_nodeemp b ,
	  port_emp c 
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND a.DeliveryWay= 3 
	AND b.FK_Emp=c.No  UNION
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	b.No AS FK_Emp,
	b.OrgNo AS OrgNo 
FROM
	wf_node a ,
	 port_emp b ,
	  wf_flow c  
WHERE
	a.NodePosType= 0 	AND a.DeliveryWay= 4 
			AND a.FK_Flow= c.No
  AND ((b.OrgNo = c.OrgNo) OR ((b.OrgNo IS NULL) AND (c.OrgNo IS NULL))) UNION
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	e.FK_Emp AS FK_Emp,
	e.OrgNo AS OrgNo 
FROM
	wf_node a ,
	 wf_nodedept b ,
	  wf_nodestation c ,
	   port_deptempstation e  
WHERE
	a.NodePosType= 0 
	AND a.NodeID= b.FK_Node 
	AND a.NodeID= c.FK_Node 
	AND b.FK_Dept= e.FK_Dept 
	AND c.FK_Station= e.FK_Station 
	AND a.DeliveryWay= 9 UNION
SELECT
	a.FK_Flow AS FK_Flow,
	a.FlowName AS FlowName,
	c.No AS FK_Emp,
	b.OrgNo AS OrgNo 
FROM
	wf_node a ,
	 wf_floworg b ,
	  port_emp c  
WHERE
	a.FK_Flow= b.FlowNo 
    AND ((b.OrgNo = c.OrgNo) OR ((b.OrgNo IS NULL) AND (c.OrgNo IS NULL)))
	AND (
	a.DeliveryWay= 22 
	OR a.DeliveryWay= 51);

CREATE VIEW WF_EmpWorks 
AS

SELECT A.PRI,A.WorkID,B.IsRead, A.Starter,
A.StarterName,
A.WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.FK_Node, B.NodeName AS NodeName,  A.Title, A.RDT, B.RDT AS ADT, 
B.SDT, B.FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode,
A.GuestNo,
A.GuestName,
A.BillNo,
A.TodoEmps,
A.TodoEmpsNum,
A.TodoSta,
A.TaskSta,
A.FlowNote,
0 as ListType,
A.Sender,
A.AtPara,
A."Domain",A.OrgNo,
C.Idx AS FlowIdx, D.Idx AS FlowSortIdx
FROM  WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Flow C,WF_FlowSort D
WHERE     (B.IsEnable = 1) AND (B.IsPass = 0)
 AND A.WorkID = B.WorkID AND A.FK_Node = B.FK_Node AND A.WFState<>0 AND WhoExeIt<>1 AND A.FK_Flow=C.No AND A.FK_FlowSort=D.No AND C.FK_FlowSort=D.No
 UNION all
SELECT A.PRI,A.WorkID,B.Sta AS IsRead, A.Starter,
A.StarterName,
2 AS WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.NodeIDWork FK_Node, B.NodeName,  A.Title, A.RDT, B.RDT AS ADT, 
B.RDT AS SDT, B.CCTo as FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode,
A.GuestNo,
A.GuestName,
A.BillNo,
A.TodoEmps,
A.TodoEmpsNum,
0 as TodoSta,
0 AS TaskSta,
A.FlowNote,
1 as ListType,
B.RecEmpNo as Sender,
'@IsCC=1'||A.AtPara as AtPara,
A."Domain",A.OrgNo,C.Idx AS FlowIdx, D.Idx AS FlowSortIdx
  FROM WF_GenerWorkFlow A, WF_CCList B ,WF_Flow C,WF_FlowSort D 
  WHERE A.WorkID=B.WorkID AND  
  B.Sta <=1 AND B.InEmpWorks = 1
   AND A.WFState<>0 AND A.FK_Flow=C.No AND A.FK_FlowSort=D.No AND C.FK_FlowSort=D.No;