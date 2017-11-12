CREATE VIEW V_KM_DocTree AS 
select KM_Tree.No MyPK,ParentNo,TreeNo,KM_Tree.Name Title,'' Doc,'' KeyWords,FK_Emp,Port_Emp.Name as FK_EmpText,
'' FK_Dept,'' FK_DeptText,RDT,'' EDTER,'' EDT,KM_Tree.Idx,'' ReadTimes,'' DownLoadTimes,'' IsDownload,
0 DocType,IsEnable,'' UploadPath,'' FileExt,FileStatus,FileSize,KMTreeCtrlWay,NumOfInfos,IsShare
from KM_Tree left join Port_Emp on Port_Emp.No = KM_Tree.FK_Emp
union
select MyPK,RefTreeNo ParentNo,KM_FileInfo.TreeNo,Title,Doc,KeyWords,FK_Emp,Port_Emp.Name as FK_EmpText,
KM_FileInfo.FK_Dept,Port_Dept.Name FK_DeptText,RDT,EDTER,EDT,KM_FileInfo.Idx,ReadTimes,DownLoadTimes,IsDownload,
1 DocType,IsEnable,UploadPath,FileExt,FileStatus,FileSize,KMTreeCtrlWay,1 NumOfInfos,IsShare
from KM_FileInfo 
inner join Port_Emp on Port_Emp.No = KM_FileInfo.FK_Emp
inner join Port_Dept on Port_Dept.No = KM_FileInfo.FK_Dept

GO
----------------------------------------------------------------------------------------------------------------
CREATE VIEW V_KM_EmpDeptTree AS 
SELECT b.FK_Dept,a.No,b.RefTreeNo FROM Port_Emp a,KM_TreeDept b
WHERE a.FK_Dept = b.FK_Dept

GO
----------------------------------------------------------------------------------------------------------------
CREATE VIEW V_KM_EmpDocTree AS 
select distinct c.* from (
SELECT a.FK_Emp+'_'+a.RefTreeNo+'_'+b.MyPK as MyTreeNO, a.FK_Emp as FK_REmp, b.* FROM 
   KM_TreeEmp a,V_KM_DocTree b WHERE a.RefTreeNo=b.MyPK AND B.IsEnable=1
UNION
SELECT a.No+'_'+a.FK_Dept+'_'+b.MyPK as MyTreeNO, a.No as FK_REmp, b.* FROM 
   Port_Emp a,V_KM_DocTree b WHERE b.IsShare=1 AND b.FileStatus =1
UNION
SELECT a.FK_Emp+'_'+a.RefTreeNo+'_'+a.FK_Dept as MyTreeNO, a.FK_Emp as FK_REmp, b.* FROM 
  V_KM_EmpDeptTree a,V_KM_DocTree b  WHERE a.RefTreeNo=b.MyPK AND B.IsEnable=1
UNION
SELECT a.FK_Emp+'_'+a.RefTreeNo+'_'+a.FK_Station as MyTreeNO, a.FK_Emp as FK_REmp, b.* 
FROM V_KM_EmpStationTree a,V_KM_DocTree b WHERE a.RefTreeNo=b.MyPK AND B.IsEnable=1
) c

GO
----------------------------------------------------------------------------------------------------------------
CREATE VIEW V_KM_EmpStationTree AS 
SELECT b.FK_Station,a.FK_Emp,b.RefTreeNo FROM Port_EmpStation a,KM_TreeStation b
WHERE a.FK_Station = b.FK_Station

GO
