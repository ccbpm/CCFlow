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
using BP.WF.HttpHandler;
using BP.NetPlatformImpl;

namespace BP.Frm
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_API : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_API()
        {
        }
        #endregion 构造方法.

        #region 常用参数.
        /// <summary>
        /// 目录树编号
        /// </summary>
        public string TreeNo
        {
            get
            {
                return this.GetRequestVal("TreeNo");
            }
        }
        #endregion 常用参数.


        #region 前台的操作 api
        /// <summary>
        /// 获得可以操作的单据列表
        /// </summary>
        /// <returns></returns>
        public string CCFrom_GenerFrmListOfCanOption()
        {
            string sql = "";
            sql = "SELECT No,Name,EntityType,FrmType,PTable FROM Sys_MapData WHERE (EntityType=1 OR EntityType=2) ORDER BY IDX ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "EntityType";
                dt.Columns[3].ColumnName = "FrmType";
                dt.Columns[4].ColumnName = "PTable";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得指定的目录下可以操作的单据列表
        /// </summary>
        /// <returns></returns>
        public string CCFrom_GenerFrmListOfCanOptionBySpecTreeNo()
        {
            string treeNo = this.GetRequestVal("TreeNo");
            return null;
        }
        /// <summary>
        /// 获得一个表单的操作权限
        /// </summary>
        /// <returns></returns>
        public string CCFrom_FrmPower()
        {
            var frmID = this.FrmID;

            //Frm.FrmBill fb = new FrmBill(frmID);

            Hashtable ht = new Hashtable();
            //ht.Add("No", fb.No);
            //ht.Add("Name", fb.Name);
            ht.Add("IsView", 1);
            ht.Add("IsInsert", 1);
            ht.Add("IsUpdate", 1);
            ht.Add("IsDelete", 1);
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 删除实体根据BillNo
        /// </summary>
        /// <returns></returns>
        public string CCFrom_DeleteFrmEntityByBillNo()
        {

            GEEntity en = new GEEntity(this.FrmID);
            int i= en.Retrieve("BillNo", this.GetRequestVal("BillNo"));
            if (i == 0)
                return "err@单据编号为" + this.GetRequestVal("BillNo") + "的数据不存在.";

            en.Delete();
            return "删除成功";
        }
        /// <summary>
        /// 删除实体根据 OID
        /// </summary>
        /// <returns></returns>
        public string CCFrom_DeleteFrmEntityByOID()
        {
            GEEntity en = new GEEntity(this.FrmID,this.OID);
            en.Delete();
            return "删除成功";
        }
        #endregion 前台的操作 api.


        #region 后台操作api.
        /// <summary>
        /// 获得所有的单据、表单 @lizhen 转移代码.
        /// </summary>
        /// <returns></returns>
        public string CCBillAdmin_Admin_GenerAllBills()
        {
            string sql = "";
            sql = "SELECT No,Name,EntityType,FrmType,PTable FROM Sys_MapData WHERE (EntityType=1 OR EntityType=2) ORDER BY IDX ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "EntityType";
                dt.Columns[3].ColumnName = "FrmType";
                dt.Columns[4].ColumnName = "PTable";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

    }
}
