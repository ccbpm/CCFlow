using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Bill : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Bill(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Bill()
        {
        }

        /// <summary>
        /// 列表初始化.
        /// </summary>
        /// <returns></returns>
        public string List_Init()
        {
            //定义容器.
            DataSet ds = new DataSet();

            //单据类别.
            BP.Sys.FrmTrees ens = new BP.Sys.FrmTrees();
            ens.RetrieveAll();

            DataTable dtSort = ens.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //查询出来单据运行模式的.
            FrmBills bills = new FrmBills();
            bills.Retrieve(FrmBillAttr.FrmBillWorkModel, 1); 

            DataTable dtStart = bills.ToDataTableField(); 
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }


        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.



    }
}
