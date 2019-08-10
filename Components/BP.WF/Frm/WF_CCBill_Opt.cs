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

namespace BP.Frm
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Opt : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Opt()
        {
        }
        #endregion 构造方法.

        #region 关联单据.
        /// <summary>
        /// 设置父子关系.
        /// </summary>
        /// <returns></returns>
        public string RefBill_Done()
        {
            try
            {
                string frmID = this.GetRequestVal("FrmID");
                Int64 workID = this.GetRequestValInt64("WorkID");
                GERpt rpt = new GERpt(frmID, workID);

                string pFrmID = this.GetRequestVal("PFrmID");
                Int64 pWorkID = this.GetRequestValInt64("PWorkID");

                //把数据copy到当前的子表单里.
                GERpt rptP = new GERpt(pFrmID, pWorkID);
                rpt.Copy(rptP);
                rpt.PWorkID = pWorkID;
                rpt.SetValByKey("PFrmID", pFrmID);
                rpt.Update();

                //更新控制表,设置父子关系.
                GenerBill gbill = new GenerBill(workID);
                gbill.PFrmID = pFrmID;
                gbill.PWorkID = pWorkID;
                gbill.Update();
                return "执行成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 单据初始化
        /// </summary>
        /// <returns></returns>
        public string RefBill_Init()
        {
            DataSet ds = new DataSet();

            #region 查询显示的列
            MapAttrs mapattrs = new MapAttrs();
            mapattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);

            DataRow row = null;
            DataTable dt = new DataTable("Attrs");
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            dt.Columns.Add("LGType", typeof(int));

            //设置标题、单据号位于开始位置


            foreach (MapAttr attr in mapattrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if (attr.UIVisible == false)
                    continue;
                row = dt.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;
                row["LGType"] = attr.LGType;
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            #endregion 查询显示的列

            #region 查询语句

            MapData md = new MapData(this.FrmID);

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);

            #region 关键字字段.
            string keyWord = this.GetRequestVal("SearchKey");

            if (DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
            {
                qo.addLeftBracket();
                if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere("Title", " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere("Title", " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                qo.addOr();
                if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere("BillNo", " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere("BillNo", " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");

                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();

            }
            else
            {
                qo.AddHD();
            }
            #endregion 关键字段查询

            #region 时间段的查询
            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            if (DataType.IsNullOrEmpty(dtFrom) == false)
            {
               
                //取前一天的24：00
                if (dtFrom.Trim().Length == 10) //2017-09-30
                    dtFrom += " 00:00:00";
                if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                    dtFrom += ":00";

                dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                    dtTo += " 24:00";

                qo.addAnd();
                qo.addLeftBracket();
                qo.SQL = " RDT>= '" + dtFrom + "'";
                qo.addAnd();
                qo.SQL = "RDT <= '" + dtTo + "'";
                qo.addRightBracket();
            }
            #endregion 时间段的查询

            qo.DoQuery("OID", this.PageSize, this.PageIdx);

            #endregion

            DataTable mydt = rpts.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 关联单据.

    }
}
