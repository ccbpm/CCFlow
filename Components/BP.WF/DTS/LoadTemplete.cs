using System;
using System.IO;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class LoadTemplete : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public LoadTemplete()
        {
            this.Title = "装载流程演示模板";
            this.Help = "为了帮助各位爱好者学习与掌握ccflow, 特提供一些流程模板与表单模板以方便学习。";
            this.Help += "@这些模板的位于" + SystemConfig.PathOfData + "\\FlowDemo\\";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No == "admin")
                    return true;
                else
                    return false;
            }
        }
        public override object Do()
        {
            string msg = "";

            #region 处理表单.
            // 调度表单文件。
            SysFormTrees fss = new SysFormTrees();
            fss.ClearTable();

            //创建root.
            SysFormTree root = new SysFormTree();
            root.No = "0";
            root.Name = "表单库";
            root.ParentNo = "-1";
            root.Insert();

            string frmPath = SystemConfig.PathOfWebApp + "\\SDKFlowDemo\\FlowDemo\\Form\\";
            DirectoryInfo dirInfo = new DirectoryInfo(frmPath);
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            int i = 0;
            foreach (DirectoryInfo item in dirs)
            {
                if (item.FullName.Contains(".svn"))
                    continue;

                string[] fls = System.IO.Directory.GetFiles(item.FullName);
                if (fls.Length == 0)
                    continue;

                SysFormTree fs = new SysFormTree();
                fs.No = item.Name.Substring(0, 2);
                fs.Name = item.Name.Substring(3);
                fs.ParentNo = "0";
                fs.Idx = i++;
                fs.Insert();

                foreach (string f in fls)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(f);
                    if (info.Extension != ".xml")
                        continue;

                    msg += "@开始调度表单模板文件:" + f;
                    BP.DA.Log.DefaultLogWriteLineInfo("@开始调度表单模板文件:" + f);

                    DataSet ds = new DataSet();
                    ds.ReadXml(f);

                    try
                    {
                        MapData md = MapData.ImpMapData(ds);
                        md.FK_FrmSort = fs.No;
                        md.FK_FormTree = fs.No;
                        md.AppType = "0";
                        md.Update();
                    }
                    catch(Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineInfo("@装载表单模版文件:" + f + "出现错误," + ex.Message + " <br> " + ex.StackTrace);

                        throw new Exception("@装载模版文件:"+f+"出现错误,"+ex.Message+" <br> "+ex.StackTrace);
                    }
                }
            }
            #endregion 处理表单.

            #region 处理流程.
            FlowSorts sorts = new FlowSorts();
            sorts.ClearTable();
            dirInfo = new DirectoryInfo(SystemConfig.PathOfWebApp + "\\SDKFlowDemo\\FlowDemo\\Flow\\");
            dirs = dirInfo.GetDirectories();

            FlowSort fsRoot = new FlowSort();
            fsRoot.No = "99";
            fsRoot.Name = "流程树";
            fsRoot.ParentNo = "0";
            fsRoot.DirectInsert();

            foreach (DirectoryInfo dir in dirs)
            {
                if (dir.FullName.Contains(".svn"))
                    continue;

                string[] fls = System.IO.Directory.GetFiles(dir.FullName);
                if (fls.Length == 0)
                    continue;

                FlowSort fs = new FlowSort();
                fs.No = dir.Name.Substring(0, 3);
                fs.Name = dir.Name.Substring(3);
                fs.ParentNo = fsRoot.No;
                fs.Insert();

                foreach (string filePath in fls)
                {
                    msg += "\t\n@开始调度流程模板文件:" + filePath;
                    BP.DA.Log.DefaultLogWriteLineInfo("@开始调度流程模板文件:" + filePath);

                    Flow myflow = BP.WF.Flow.DoLoadFlowTemplate(fs.No, filePath, ImpFlowTempleteModel.AsTempleteFlowNo);
                    msg += "\t\n@流程:[" + myflow.Name + "]装载成功。";

                    System.IO.FileInfo info = new System.IO.FileInfo(filePath);
                    myflow.Name = info.Name.Replace(".xml", "");
                    if (myflow.Name.Substring(2, 1) == ".")
                        myflow.Name = myflow.Name.Substring(3);
                    myflow.DirectUpdate();
                }


                //调度它的下一级目录.
                DirectoryInfo dirSubInfo = new DirectoryInfo(SystemConfig.PathOfWebApp + "\\SDKFlowDemo\\FlowDemo\\Flow\\" + dir.Name);
                DirectoryInfo[] myDirs = dirSubInfo.GetDirectories();
                foreach (DirectoryInfo mydir in myDirs)
                {
                    if (mydir.FullName.Contains(".svn"))
                        continue;

                    string[] myfls = System.IO.Directory.GetFiles(mydir.FullName);
                    if (myfls.Length == 0)
                        continue;

                    // 流程类别.
                    FlowSort subFlowSort = fs.DoCreateSubNode() as FlowSort;
                    subFlowSort.Name = mydir.Name.Substring(3);
                    subFlowSort.Update();

                    foreach (string filePath in myfls)
                    {
                        msg += "\t\n@开始调度流程模板文件:" + filePath;
                        BP.DA.Log.DefaultLogWriteLineInfo("@开始调度流程模板文件:" + filePath);

                        Flow myflow = BP.WF.Flow.DoLoadFlowTemplate(subFlowSort.No, filePath, ImpFlowTempleteModel.AsTempleteFlowNo);
                        msg += "\t\n@流程:" + myflow.Name + "装载成功。";

                        System.IO.FileInfo info = new System.IO.FileInfo(filePath);
                        myflow.Name = info.Name.Replace(".xml", "");
                        if (myflow.Name.Substring(2, 1) == ".")
                            myflow.Name = myflow.Name.Substring(3);
                        myflow.DirectUpdate();
                    }
                }
            }

            //执行流程检查.
            Flows flsEns = new Flows();
            flsEns.RetrieveAll();
            foreach (Flow fl in flsEns)
            {
                fl.DoCheck();
            }
            #endregion 处理流程.



            BP.DA.Log.DefaultLogWriteLineInfo(msg);

            //删除多余的空格.
            BP.WF.DTS.DeleteBlankGroupField dts = new DeleteBlankGroupField();
            dts.Do();

            return msg;
        }
    }
}
