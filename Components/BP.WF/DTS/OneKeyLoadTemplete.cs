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
    public class OneKeyLoadTemplete : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public OneKeyLoadTemplete()
        {
            this.Title = "一键恢复流程模版目录";
            this.Help = "此功能是一键备份流程的逆向操作.";
            this.Help += "@执行时请注意";
            this.Help += "@1,系统所有的流程数据、模版数据、组织解构数据、将会被删除。";
            this.Help += "@2,重新装载C:\\CCFlowTemplete 的数据。";
            this.Help += "@3,此功能一般提供给ccflow的开发者用于不同的数据库之间的移植。";
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
                if (BP.Web.WebUser.No != "admin")
                    return false;

                return true;
            }
        }
        public override object Do()
        {
            string msg = "";

            #region 检查数据文件是否完整.
            string path = "C:\\CCFlowTemplete";
            if (System.IO.Directory.Exists(path) == false)
                msg += "@错误：约定的目录不存在服务器" + path + ",请把从ccflow备份的文件放入" + path;

            //PortTables.
            string file = path + "\\PortTables.xml";
            if (System.IO.File.Exists(file) == false)
                msg += "@错误：约定的文件不存在，" + file;

            //SysTables.
            file = path + "\\SysTables.xml";
            if (System.IO.File.Exists(file) == false)
                msg += "@错误：约定的文件不存在，" + file;

            //FlowTables.
            file = path + "\\FlowTables.xml";
            if (System.IO.File.Exists(file) == false)
                msg += "@错误：约定的文件不存在，" + file;
            #endregion 检查数据文件是否完整.

            #region 1 装载流程基础表数据.
            DataSet ds = new DataSet();
            ds.ReadXml(path + "\\FlowTables.xml");

            //流程类别.
            FlowSorts sorts = new FlowSorts();
            sorts.ClearTable();
            DataTable dt = ds.Tables["WF_FlowSort"];
           // sorts = QueryObject.InitEntitiesByDataTable(sorts, dt, null) as FlowSorts;
            foreach (FlowSort item in sorts)
            {
                item.DirectInsert(); //插入数据.
            }
            #endregion 1 装载流程基础表数据.

            #region 2 组织结构.
            ds = new DataSet();
            ds.ReadXml(path + "\\PortTables.xml");

            //Port_Emp.
            Emps emps = new Emps();
            emps.ClearTable();
            dt = ds.Tables["Port_Emp"];
            emps = QueryObject.InitEntitiesByDataTable(emps, dt, null) as Emps;
            foreach (Emp item in emps)
            {
                item.DirectInsert(); //插入数据.
            }

            //Depts.
            Depts depts = new Depts();
            depts.ClearTable();
            dt = ds.Tables["Port_Dept"];
            depts = QueryObject.InitEntitiesByDataTable(depts, dt, null) as Depts;
            foreach (Dept item in depts)
            {
                item.DirectInsert(); //插入数据.
            }

            //Stations.
            Stations stas = new Stations();
            stas.ClearTable();
            dt = ds.Tables["Port_Station"];
            stas = QueryObject.InitEntitiesByDataTable(stas, dt, null) as Stations;
            foreach (Station item in stas)
            {
                item.DirectInsert(); //插入数据.
            }


            if (BP.Sys.SystemConfig.OSModel == OSModel.OneMore)
            {
                //EmpDepts.
                BP.GPM.DeptEmps eds = new BP.GPM.DeptEmps();
                eds.ClearTable();
                dt = ds.Tables["Port_DeptEmp"];
                eds = QueryObject.InitEntitiesByDataTable(eds, dt, null) as BP.GPM.DeptEmps;
                foreach (BP.GPM.DeptEmp item in eds)
                {
                    item.DirectInsert(); //插入数据.
                }
            }
            #endregion 2 组织结构.

            #region 3 恢复系统数据.
            ds = new DataSet();
            ds.ReadXml(path + "\\SysTables.xml");

            //枚举Main.
            SysEnumMains sems = new SysEnumMains();
            sems.ClearTable();
            dt = ds.Tables["Sys_EnumMain"];
            sems = QueryObject.InitEntitiesByDataTable(sems, dt, null) as SysEnumMains;
            foreach (SysEnumMain item in sems)
            {
                item.DirectInsert(); //插入数据.
            }

            //枚举.
            SysEnums ses = new SysEnums();
            ses.ClearTable();
            dt = ds.Tables["Sys_Enum"];
            ses = QueryObject.InitEntitiesByDataTable(ses, dt, null) as SysEnums;
            foreach (SysEnum item in ses)
            {
                item.DirectInsert(); //插入数据.
            }

            ////Sys_FormTree.
            //BP.Sys.SysFormTrees sfts = new SysFormTrees();
            //sfts.ClearTable();
            //dt = ds.Tables["Sys_FormTree"];
            //sfts = QueryObject.InitEntitiesByDataTable(sfts, dt, null) as SysFormTrees;
            //foreach (SysFormTree item in sfts)
            //{
            //    try
            //    {
            //       item.DirectInsert(); //插入数据.
            //    }
            //    catch
            //    {
            //    }
            //}
            #endregion 3 恢复系统数据.

            #region 4.备份表单相关数据.
            if (1 == 2)
            {
                string pathOfTables = path + "\\SFTables";
                System.IO.Directory.CreateDirectory(pathOfTables);
                SFTables tabs = new SFTables();
                tabs.RetrieveAll();
                foreach (SFTable item in tabs)
                {
                    if (item.No.Contains("."))
                        continue;

                    string sql = "SELECT * FROM " + item.No;
                    ds = new DataSet();
                    ds.Tables.Add(BP.DA.DBAccess.RunSQLReturnTable(sql));
                    ds.WriteXml(pathOfTables + "\\" + item.No + ".xml");
                }
            }
            #endregion 4 备份表单相关数据.

            #region 5.恢复表单数据.
            //删除所有的流程数据.
            MapDatas mds = new MapDatas();
            mds.RetrieveAll();
            foreach (MapData fl in mds)
            {
                //if (fl.FK_FormTree.Length > 1 || fl.FK_FrmSort.Length > 1)
                //    continue;
                fl.Delete(); //删除流程.
            }

            //清除数据.
            SysFormTrees fss = new SysFormTrees();
            fss.ClearTable();

            // 调度表单文件。         
            string frmPath = path + "\\Form";
            DirectoryInfo dirInfo = new DirectoryInfo(frmPath);
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo item in dirs)
            {
                if (item.FullName.Contains(".svn"))
                    continue;

                string[] fls = System.IO.Directory.GetFiles(item.FullName);
                if (fls.Length == 0)
                    continue;

                SysFormTree fs = new SysFormTree();
                fs.No = item.Name.Substring(0, item.Name.IndexOf('.') );
                fs.Name = item.Name.Substring(item.Name.IndexOf('.'));
                fs.ParentNo = "0";
                fs.Insert();

                foreach (string f in fls)
                {
                    try
                    {
                        msg += "@开始调度表单模板文件:" + f;
                        System.IO.FileInfo info = new System.IO.FileInfo(f);
                        if (info.Extension != ".xml")
                            continue;

                        ds = new DataSet();
                        ds.ReadXml(f);

                        MapData md = MapData.ImpMapData(ds);
                        md.FK_FrmSort = fs.No;
                        md.Update();
                    }
                    catch (Exception ex)
                    {
                        msg += "@调度失败,文件:"+f+",异常信息:" + ex.Message;
                    }
                }
            }
            #endregion 5.恢复表单数据.

            #region 6.恢复流程数据.
            //删除所有的流程数据.
            Flows flsEns = new Flows();
            flsEns.RetrieveAll();
            foreach (Flow fl in flsEns)
            {
                fl.DoDelete(); //删除流程.
            }

            dirInfo = new DirectoryInfo(path + "\\Flow\\");
            dirs = dirInfo.GetDirectories();

            //删除数据.
            FlowSorts fsRoots = new FlowSorts();
            fsRoots.ClearTable();

            //生成流程树.
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
                fs.No = dir.Name.Substring(0, dir.Name.IndexOf('.') );
                fs.Name = dir.Name.Substring(3);
                fs.ParentNo = fsRoot.No;
                fs.Insert();

                foreach (string filePath in fls)
                {
                    msg += "\t\n@开始调度流程模板文件:" + filePath;
                    Flow myflow = BP.WF.Flow.DoLoadFlowTemplate(fs.No, filePath, ImpFlowTempleteModel.AsTempleteFlowNo);
                    msg += "\t\n@流程:" + myflow.Name + "装载成功。";

                    System.IO.FileInfo info = new System.IO.FileInfo(filePath);
                    myflow.Name = info.Name.Replace(".xml", "");
                    if (myflow.Name.Substring(2, 1) == ".")
                        myflow.Name = myflow.Name.Substring(3);

                    myflow.DirectUpdate();
                }
            }
            #endregion 6.恢复流程数据.

            BP.DA.Log.DefaultLogWriteLineInfo(msg);

            //删除多余的空格.
            BP.WF.DTS.DeleteBlankGroupField dts = new DeleteBlankGroupField();
            dts.Do();

            //执行生成签名.
            GenerSiganture gs = new GenerSiganture();
            gs.Do();

            return msg;
        }
    }
}
