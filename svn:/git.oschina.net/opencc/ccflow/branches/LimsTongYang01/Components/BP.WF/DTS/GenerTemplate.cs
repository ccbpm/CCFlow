using System;
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
    public class GenerTemplate : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerTemplate()
        {
            this.Title = "生成流程模板与表单模板";
            this.Help = "把系统中的流程与表单转化成模板放在指定的目录下。";
            this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
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
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string path = this.GetValStrByKey("Path") + "_" + DateTime.Now.ToString("yy年MM月dd日HH时mm分");
            if (System.IO.Directory.Exists(path))
                return "系统正在执行中，请稍后。";

            System.IO.Directory.CreateDirectory(path);
            System.IO.Directory.CreateDirectory(path + "\\Flow.流程模板");
            System.IO.Directory.CreateDirectory(path + "\\Frm.表单模板");

            Flows fls = new Flows();
            fls.RetrieveAll();
            FlowSorts sorts = new FlowSorts();
            sorts.RetrieveAll();

            // 生成流程模板。
            foreach (FlowSort sort in sorts)
            {
                string pathDir = path + "\\Flow.流程模板\\" + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);
                foreach (Flow fl in fls)
                {
                    fl.DoExpFlowXmlTemplete(pathDir);
                }
            }

            // 生成表单模板。
            foreach (FlowSort sort in sorts)
            {
                string pathDir = path + "\\Frm.表单模板\\" + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);
                foreach (Flow fl in fls)
                {
                    string pathFlowDir = pathDir + "\\" + fl.No + "." + fl.Name;
                    System.IO.Directory.CreateDirectory(pathFlowDir);
                    Nodes nds = new Nodes(fl.No);
                    foreach (Node nd in nds)
                    {
                        MapData md = new MapData("ND" + nd.NodeID);
                        System.Data.DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                        ds.WriteXml(pathFlowDir + "\\" + nd.NodeID + "." + nd.Name + ".Frm.xml");
                    }
                }
            }

            // 独立表单模板.
            SysFormTrees frmSorts = new SysFormTrees();
            frmSorts.RetrieveAll();
            foreach (SysFormTree sort in frmSorts)
            {
                string pathDir = path + "\\Frm.表单模板\\" + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);

                MapDatas mds = new MapDatas();
                mds.Retrieve(MapDataAttr.FK_FrmSort, sort.No);
                foreach (MapData md in mds)
                {
                    System.Data.DataSet ds =BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                    ds.WriteXml(pathDir + "\\" + md.No + "." + md.Name + ".Frm.xml");
                }
            }
            return "生成成功，请打开" + path + "。<br>如果您想共享出来请压缩后发送到template＠ccflow.org";
        }
    }
}
