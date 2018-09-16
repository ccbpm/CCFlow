using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting
{
    /// <summary>
    /// 根据datatable 生成单据
    /// </summary>
    public class GenerBillByDataTable : TestBase
    {
        /// <summary>
        /// 根据datatable 生成单据
        /// </summary>
        public GenerBillByDataTable()
        {
            this.Title = "根据 datatable 生成单据";
            this.DescIt = "流程: 财务报销流程.";
            this.EditState = EditState.Passed;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1, .
        /// 2, .
        /// 3，.
        /// </summary>
        public override void Do()
        {
            #region 初始化数据。
            //初始化变量.
            fk_flow = "001";
            userNo = "zhangyifan";
            fl = new Flow(fk_flow);
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白的流程.
            this.workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, null, null);

            //创建主表数据. （费用主表）。
            Hashtable ht = new Hashtable();
            ht.Add("ZhaiYaoShuoMing", "摘要说明:ZhaiYaoShuoMing.");
            ht.Add("RPI", 1);

            // 创建从表数据。（费用明细）
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.TableName = "ND101Dtl1";
            dt.Columns.Add("RefPK", typeof(int)); //关联的主键，这里是workid.
            dt.Columns.Add("FYLX", typeof(int)); // 费用类型.
            dt.Columns.Add("JinE", typeof(decimal)); // 金额
            dt.Columns.Add("ShuLiang", typeof(decimal)); //数量.
            dt.Columns.Add("XiaoJi", typeof(decimal)); // 小计。

            DataRow dr = dt.NewRow();
            dr["RefPK"] = this.workID;
            dr["FYLX"] = 1;
            dr["JinE"] = 150;
            dr["ShuLiang"] = 2;
            dr["XiaoJi"] = 300;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["RefPK"] = this.workID;
            dr["FYLX"] = 2;
            dr["JinE"] = 250;
            dr["ShuLiang"] = 3;
            dr["XiaoJi"] = 750;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);

            //执行发送.
            BP.WF.Dev2Interface.Node_SendWork(fk_flow, this.workID, ht, ds, 0, null);
            #endregion 初始化数据。

            #region 生成测试数据。
            //查询到保存数据库的数据源。
            string sql = "SELECT * FROM ND101 WHERE OID=" + this.workID;
            DataTable dtMain = DBAccess.RunSQLReturnTable(sql);

            sql = "SELECT * FROM ND101Dtl1 WHERE RefPK=" + this.workID;
            DataTable dtDtl = DBAccess.RunSQLReturnTable(sql);
            dtDtl.TableName = "ND101Dtl1";

            DataSet myds = new DataSet();
            myds.Tables.Add(dtDtl);
            #endregion 生成测试数据。

            string templeteFilePath = @"D:\ccflow\trunk\CCFlow\DataUser\CyclostyleFile\单据打印样本.rtf";
            BP.Pub.RTFEngine rpt = new BP.Pub.RTFEngine();

            rpt.MakeDocByDataSet(templeteFilePath, "C:\\", this.workID + ".doc", dtMain, myds);
        }
    }
}
