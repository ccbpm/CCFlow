using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.WF;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 工作流定义管理
    /// </summary>
    public class WorkflowDefintionManager
    {
        /// <summary>
        /// 保存流程, 用 ~ 隔开。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodes">节点信息，格式为:@NodeID=xxxx@X=xxx@Y=xxx@Name=xxxx@RunModel=1</param>
        /// <param name="dirs">方向信息，格式为: @Node=xxxx@ToNode=xxx@X=xxx@Y=xxx@Name=xxxx   </param>
        /// <param name="labes">标签信息，格式为:@MyPK=xxxxx@Label=xxx@X=xxx@Y=xxxx</param>
        public static string SaveFlow(string fk_flow, string nodes, string dirs, string labes)
        {
            try
            {
                #region 处理方向.
                string sql = "DELETE FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                DBAccess.RunSQL(sql);

                string[] mydirs = dirs.Split('~');
                foreach (string dir in mydirs)
                {
                    if (DataType.IsNullOrEmpty(dir))
                        continue;

                    AtPara ap = new AtPara(dir);

                    string dots = ap.GetValStrByKey("Dots").Replace('#', '@');
                    if (DataType.IsNullOrEmpty(dots) == true)
                        dots = "";

                    Direction enDir = new Direction();
                    enDir.Node = ap.GetValIntByKey(DirectionAttr.Node);
                    enDir.ToNode = ap.GetValIntByKey(DirectionAttr.ToNode);
                    enDir.IsCanBack = ap.GetValBoolenByKey(DirectionAttr.IsCanBack);
                    enDir.DirType = ap.GetValIntByKey(DirectionAttr.DirType);
                    enDir.FK_Flow = fk_flow;
                    enDir.Dots = dots;
                    try
                    {
                        enDir.Insert();
                    }
                    catch
                    {
                        // enDir.Update();
                    }
                }
                #endregion 处理方向.

                #region 保存节点
                string[] nds = nodes.Split('~');
                foreach (string nd in nds)
                {
                    if (DataType.IsNullOrEmpty(nd))
                        continue;

                    AtPara ap = new AtPara(nd);
                    sql = "UPDATE WF_Node SET X=" + ap.GetValStrByKey("X") + ",Y=" + ap.GetValStrByKey("Y") + ", Name='" + ap.GetValStrByKey("Name") + "' WHERE NodeID=" + ap.GetValIntByKey("NodeID");
                    DBAccess.RunSQL(sql);
                }
                Flow.UpdateVer(fk_flow);
                #endregion 保存节点

                #region 处理标签。
                sql = "DELETE FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                DBAccess.RunSQL(sql);

                string[] mylabs = labes.Split('~');
                foreach (string lab in mylabs)
                {
                    if (DataType.IsNullOrEmpty(lab))
                        continue;

                    AtPara ap = new AtPara(lab);
                    LabNote ln = new LabNote();
                    ln.MyPK = BP.DA.DBAccess.GenerGUID();  // ap.GetValStrByKey("MyPK");
                    ln.FK_Flow = fk_flow;
                    ln.Name = ap.GetValStrByKey("Label");
                    ln.X = ap.GetValIntByKey("X");
                    ln.Y = ap.GetValIntByKey("Y");
                    ln.Insert();
                }
                #endregion 处理标签。


                // 备份文件
                //f1.WriteToXml();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
        /// <summary>
        /// 导出流程模版
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="saveToPath"></param>
        public static void ExpWorkFlowTemplete(string flowNo,string saveToPath)
        {
        }
        /// <summary>
        /// 导入流程模版
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="filePath">文件路径</param>
        public static void ImpWorkFlowTemplete(string flowNo, string filePath)
        {
        }
        /// <summary>
        /// 执行新建一个流程模版
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        public static void CreateFlowTemplete(string flowSort)
        {
        }
        /// <summary>
        /// 删除一个流程模版
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        public static string DeleteFlowTemplete(string flowNo)
        {
            BP.WF.Flow fl = new BP.WF.Flow(flowNo);
            try
            {
                fl.DoDelete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError("Do Method DelFlow Branch has a error , para:\t" + flowNo + ex.Message);
                return "err@" + ex.Message;
            }
        }
    }
}
