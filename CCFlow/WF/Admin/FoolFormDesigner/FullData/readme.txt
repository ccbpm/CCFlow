=== 说明.

1. 传入了一个MapExt的MyPK . 
2. 在MyPK+"_FullData", 作为该填充数据的主键.
3. 三种类型的数据，主表，从表，下拉框 ， 所有的数据，都存入了一行记录。

********* 关于数据存储格式.

4. 文本框的自动完成填充，与下拉框的自动填充，都是存储在 Sys_MapExt 的一行数据上.
   4.1 文本框的主键为 格式为:TBFullCtrl_"+frmID+"_"+keOfEn+"_FullData
   4.1 下拉框的主键为 格式为:DDLFullCtrl_"+frmID+"_"+keOfEn+"_FullData
 
   例如： TBFullCtrl_Frm_KeHuDingShan_KeHuBianHao_FullData

   4.2 决定文本框的填充模式是在 Sys_MapAttr的 AtPara字段.  TBFullCtrl (None/Simple/Table)

5. pop 的设置存储在一行记录上。格式为:
         "PopBranchesAndLeaf_" +frmID + "_" +keyOfEn;

5.1 填充的数据格式为.  "PopBranchesAndLeaf_" +frmID + "_" +keyOfEn+"_FullData";

5.2 决定文本框的pop模式是在 Sys_MapAttr的 AtPara 字段. 
     PopModel (None/PopBranchesAndLeaf/PopBranches ....)

