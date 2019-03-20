/****** 对象:  View WF_EmpWorks    脚本日期: 03/12/2011 21:42:50 ******/;

/*  WF_EmpWorks  */;

CREATE VIEW  WF_EmpWorks
(
PRI,
WorkID,
IsRead,
Starter,
StarterName,
WFState, 
FK_Dept,
DeptName,
FK_Flow,
FlowName,
PWorkID,
PFlowNo,
FK_Node,
NodeName,
WorkerDept,
Title,
RDT,
ADT,
SDT,
FK_Emp,
FID,
FK_FlowSort,
SysType,
SDTOfNode,
PressTimes,
GuestNo,
GuestName,
BillNo,
FlowNote,
TodoEmps,
TodoEmpsNum,
TodoSta,
TaskSta,
ListType,
Sender,
AtPara,
MyNum
)
AS

SELECT A.PRI,A.WorkID,B.IsRead, A.Starter,
A.StarterName,
A.WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.FK_Node, B.FK_NodeText AS NodeName, B.FK_Dept WorkerDept, A.Title, A.RDT, B.RDT AS ADT, 
B.SDT, B.FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode,B.PressTimes,
A.GuestNo,
A.GuestName,
A.BillNo,
A.FlowNote,
A.TodoEmps,
A.TodoEmpsNum,
A.TodoSta,
A.TaskSta,
0 as ListType,
A.Sender,
A.AtPara,
1 as MyNum
FROM  WF_GenerWorkFlow A, WF_GenerWorkerlist B
WHERE     (B.IsEnable = 1) AND (B.IsPass = 0)
 AND A.WorkID = B.WorkID AND A.FK_Node = B.FK_Node AND A.WFState!=0
 UNION
SELECT A.PRI,A.WorkID,B.Sta AS IsRead, A.Starter,
A.StarterName,
2 AS WFState,
A.FK_Dept,A.DeptName, A.FK_Flow, A.FlowName,A.PWorkID,
A.PFlowNo,
B.FK_Node, B.NodeName,  B.CCToDept as WorkerDept,A.Title, A.RDT, B.RDT AS ADT, 
B.RDT AS SDT, B.CCTo as FK_Emp,B.FID ,A.FK_FlowSort,A.SysType,A.SDTOfNode, 0 as PressTimes,
A.GuestNo,
A.GuestName,
A.BillNo,
A.FlowNote,
A.TodoEmps,
A.TodoEmpsNum,
0 as TodoSta,
0 AS TaskSta,
1 as ListType,
B.Rec as Sender,
'@IsCC=1'+A.AtPara as AtPara,
1 as MyNum
  FROM WF_GenerWorkFlow A, WF_CCList B WHERE A.WorkID=B.WorkID AND  B.Sta <=1 AND B.InEmpWorks = 1 AND A.WFState!=0;
  
  

/****** 对象:  View V_FlowStarter 脚本日期:  2015-04-10 ******/;
/*  V_FlowStarter 
-- 按绑定岗位.
-- 按绑定部门的人员.
-- 按绑定人员的人员.
  */;
CREATE VIEW V_FlowStarter (FK_Flow,FlowName,FK_Emp)
AS
SELECT A.FK_Flow, a.FlowName, C.FK_Emp FROM WF_Node a, WF_NodeStation b, Port_DeptEmpStation c 
 WHERE a.NodePosType=0 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 ) 
AND  a.NodeID=b.FK_Node AND B.FK_Station=C.FK_Station   AND (A.DeliveryWay=0 OR A.DeliveryWay=14)
UNION 
SELECT A.FK_Flow, a.FlowName, C.No as FK_Emp FROM WF_Node a, WF_NodeDept b, Port_Emp c 
 WHERE a.NodePosType=0 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 ) 
AND  a.NodeID=b.FK_Node AND B.FK_Dept=C.FK_Dept   AND A.DeliveryWay=1 
UNION 
SELECT A.FK_Flow, a.FlowName, B.FK_Emp FROM WF_Node A, WF_NodeEmp B 
 WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) 
AND A.NodeID=B.FK_Node  AND A.DeliveryWay=3
UNION 
SELECT A.FK_Flow, a.FlowName, B.No AS FK_Emp FROM WF_Node A, Port_Emp B 
 WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 )  AND A.DeliveryWay=4 
 UNION   
SELECT A.FK_Flow, a.FlowName, E.FK_Emp FROM WF_Node A, WF_NodeDept B, WF_NodeStation C, Port_Emp D,  Port_DeptEmpStation E
 WHERE a.NodePosType=0 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 ) AND  A.NodeID=B.FK_Node AND A.NodeID=C.FK_Node AND B.FK_Dept=D.FK_Dept
  AND C.FK_Station=E.FK_Station AND A.DeliveryWay=9 ;

/****** 对象:  View V_FlowStarterBPM 脚本日期:  2015-04-10 ******/;
/*  V_FlowStarterBPM 
-- 按绑定岗位.
-- 按绑定部门的人员.
-- 按绑定人员的人员.
  */;

CREATE VIEW V_FlowStarterBPM (FK_Flow,FlowName,FK_Emp)
AS
SELECT A.FK_Flow, a.FlowName, C.FK_Emp FROM WF_Node a, WF_NodeStation b, Port_DeptEmpStation c 
 WHERE a.NodePosType=0 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 ) 
AND  a.NodeID=b.FK_Node AND B.FK_Station=C.FK_Station   AND (A.DeliveryWay=0 OR A.DeliveryWay=14)
  UNION 
SELECT A.FK_Flow, a.FlowName, C.FK_Emp FROM WF_Node a, WF_NodeDept b, Port_DeptEmp c 
 WHERE a.NodePosType=0 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 )
AND  a.NodeID=b.FK_Node AND B.FK_Dept=C.FK_Dept   AND A.DeliveryWay=1
  UNION 
SELECT A.FK_Flow, a.FlowName, B.FK_Emp FROM WF_Node A, WF_NodeEmp B 
 WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) 
AND A.NodeID=B.FK_Node  AND A.DeliveryWay=3
  UNION 
SELECT A.FK_Flow, a.FlowName, B.No AS FK_Emp FROM WF_Node A, Port_Emp B 
 WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 )  AND A.DeliveryWay=4
  UNION  
SELECT A.FK_Flow, a.FlowName, E.FK_Emp FROM WF_Node A, WF_NodeDept B, WF_NodeStation C,  Port_DeptEmpStation E
 WHERE a.NodePosType=0 
 AND ( a.WhoExeIt=0 OR a.WhoExeIt=2 ) 
 AND  A.NodeID=B.FK_Node 
 AND A.NodeID=C.FK_Node 
 AND B.FK_Dept=E.FK_Dept 
 AND C.FK_Station=E.FK_Station AND A.DeliveryWay=9 ;
 

/****** 考核:  View V_TOTALCH    脚本日期:  2015-09-10 ******/;
/*  V_TOTALCH */;
CREATE VIEW V_TOTALCH
(FK_Emp,AllNum,ASNum,CSNum,JiShi,ANQI,YuQi,ChaoQi,WCRate)
AS
SELECT FK_Emp, (SELECT count(MyPK) AS Num FROM WF_CH A WHERE A.FK_Emp=WF_CH.FK_Emp) as  AllNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp) as  ASNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta >=2 AND A.FK_Emp=WF_CH.FK_Emp) as  CSNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =0 AND A.FK_Emp=WF_CH.FK_Emp) as  JiShi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =1 AND A.FK_Emp=WF_CH.FK_Emp) as  AnQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =2 AND A.FK_Emp=WF_CH.FK_Emp) as  YuQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =3 AND A.FK_Emp=WF_CH.FK_Emp) as  ChaoQi,
ROUND( (SELECT  convert(float,count(MyPK) ) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp)/(SELECT count(MyPK) AS Num FROM WF_CH A WHERE  A.FK_Emp=WF_CH.FK_Emp)*100,2) 
AS WCRate
FROM WF_CH GROUP BY FK_Emp;


/****** 考核:  View 总的考核 月份   脚本日期:  2015-09-10 ******/;
/*  V_TOTALCHYF */;
CREATE VIEW V_TOTALCHYF
(FK_Emp,FK_NY,AllNum,ASNum,CSNum,JiShi,AnQi,YuQi,ChaoQi,WCRate)
AS
SELECT FK_Emp,FK_NY, (SELECT count(MyPK) AS Num FROM WF_CH A WHERE A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  AllNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  ASNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta >=2 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  CSNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =0 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  JiShi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =1 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  AnQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =2 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  YuQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =3 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY) as  ChaoQi,
ROUND( (SELECT  convert(float,count(MyPK) ) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY )/(SELECT count(MyPK) AS Num FROM WF_CH A WHERE  A.FK_Emp=WF_CH.FK_Emp AND A.FK_NY=WF_CH.FK_NY)*100,2) 
AS WCRate
FROM WF_CH GROUP BY FK_Emp,FK_NY;

/****** 考核:  V_TotalCHWeek 总的考核 周 脚本日期:  2015-09-10 ******/;
/*  V_TotalCHWeek */;
CREATE VIEW V_TotalCHWeek
(FK_Emp,WeekNum,FK_NY,AllNum,ASNum,CSNum,JiShi,AnQi,YuQi,ChaoQi,WCRate)
AS
SELECT FK_Emp,WeekNum,FK_NY, (SELECT count(MyPK) AS Num FROM WF_CH A WHERE A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  AllNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  ASNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta >=2 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  CSNum,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =0 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  JiShi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =1 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  AnQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =2 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  YuQi,
(SELECT count(MyPK) AS Num FROM WF_CH A WHERE CHSta =3 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum) as  ChaoQi,
ROUND( (SELECT  convert(float,count(MyPK) ) AS Num FROM WF_CH A WHERE CHSta <=1 AND A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum )/(SELECT count(MyPK) AS Num FROM WF_CH A WHERE  A.FK_Emp=WF_CH.FK_Emp AND A.WeekNum=WF_CH.WeekNum)*100,2) 
AS WCRate
FROM WF_CH GROUP BY FK_Emp,WeekNum,FK_NY;

/****** 考核:  V_WF_Delay 逾期流程 脚本日期:  2019-03-19 ******/;
/*  V_WF_Delay */;
CREATE VIEW V_WF_Delay
AS
SELECT     CONVERT(varchar, WorkID) + '_' + CONVERT(varchar, FK_Emp) + '_' + CONVERT(varchar, FK_Node) AS MyPK, PRI, WorkID, IsRead, Starter, StarterName, WFState, FK_Dept, DeptName, FK_Flow, 
                      FlowName, PWorkID, PFlowNo, FK_Node, NodeName, WorkerDept, Title, RDT, ADT, SDT, FK_Emp, FID, FK_FlowSort, SysType, SDTOfNode, PressTimes, GuestNo, GuestName, BillNo, FlowNote, 
                      TodoEmps, TodoEmpsNum, TodoSta, TaskSta, ListType, Sender, AtPara, MyNum
FROM         dbo.WF_EmpWorks where CONVERT(datetime,SDT, 20) > GETDATE() AND SDT<>'无'
GO