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
using BP.Difference;
using BP.CCBill.Template;
using Newtonsoft.Json.Linq;

namespace BP.CCBill
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
            string userNo = GetRequestVal("UserNo");
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.No;

            string powerSQL = "";

            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                powerSQL = "SELECT FrmID," +
              "(CASE WHEN IsEnableAll=1 THEN true " +
              "ELSE(CASE WHEN IsEnableUser=1 AND INSTR(IDOfUsers,'," + userNo + ",')>0 THEN true " +
              "ELSE(CASE WHEN IsEnableStation=1 AND (SELECT COUNT(*) From Port_DeptEmpStation D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND INSTR(IDOfStations,D.FK_Station))>0 THEN true " +
              "ELSE(CASE WHEN IsEnableDept=1 AND (SELECT COUNT(*) From Port_DeptEmp D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND INSTR(IDOfDepts,D.FK_Dept))>0 THEN true " +
              "ELSE false END)" +
              "END)" +
              "END)" +
              "END) AS IsView   FROM Frm_CtrlModel WHERE CtrlObj='BtnSearch'";

                sql = "SELECT No,Name,EntityType,FrmType,PTable FROM Sys_MapData M, (" + powerSQL + ") AS B WHERE M.No=B.FrmID AND (M.EntityType=1 OR M.EntityType=2) AND B.IsView=1 ORDER BY M.IDX ";

            }
            else 
            {
                powerSQL = "SELECT FrmID," +
             "(CASE WHEN IsEnableAll=1 THEN true " +
             "ELSE(CASE WHEN IsEnableUser=1 AND 1=1 THEN true " +
             "ELSE(CASE WHEN IsEnableStation=1 AND (SELECT COUNT(*) From Port_DeptEmpStation D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND  1=1 THEN true " +
             "ELSE(CASE WHEN IsEnableDept=1 AND (SELECT COUNT(*) From Port_DeptEmp D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND  1=1 THEN true " +
             "ELSE false END)" +
             "END)" +
             "END)" +
             "END) AS IsView   FROM Frm_CtrlModel WHERE CtrlObj='BtnSearch'";

                sql = "SELECT No,Name,EntityType,FrmType,PTable FROM Sys_MapData M  WHERE  (M.EntityType=1 OR M.EntityType=2)   ORDER BY M.IDX ";

            }

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
        /// 根据单据编号创建或者更新实体信息.
        /// </summary>
        /// <returns>返回url：打开该实体的url.</returns>
        public string CCFrom_NewFrmBillAsSpecBillNo()
        {

            string billNo = this.GetRequestVal("BillNo");
            string title = this.GetRequestVal("Title");
            string paras = this.GetRequestVal("Paras");

            if (DataType.IsNullOrEmpty(paras) == true)
                paras = "";
            AtPara ap = new AtPara(paras);

            GEEntity en = new GEEntity(this.FrmID);
            int i = en.Retrieve("BillNo", billNo);
            if (i == 0)
            {
                Int64 workid = BP.CCBill.Dev2Interface.CreateBlankBillID(this.FrmID, WebUser.No, ap.HisHT, billNo);
                en = new GEEntity(this.FrmID, workid);
                if (DataType.IsNullOrEmpty(paras) == false)
                {
                    en.Copy(ap.HisHT);
                    en.Update();
                }
                if (DataType.IsNullOrEmpty(title) == false)
                {
                    en.SetValByKey("Title", title);
                    en.Update();
                }
                return "url@../../WF/CCBill/MyBill.htm?FrmID=" + this.FrmID + "&OID=" + workid;
            }
            else
            {
                if (DataType.IsNullOrEmpty(paras) == false)
                {
                    en.Copy(ap.HisHT);
                    en.Update();
                }

                if (DataType.IsNullOrEmpty(title) == false && en.GetValStrByKey("Title").Equals(title)==false)
                {
                    en.SetValByKey("Title", title);
                    en.Update();
                }
            }
            return "url@../../WF/CCBill/MyBill.htm?FrmID=" + this.FrmID + "&OID=" + en.OID;
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
            Hashtable ht = new Hashtable();
            string frmID = this.FrmID;
            CtrlModels ctrlMs = new CtrlModels();

            ctrlMs.Retrieve(CtrlModelAttr.FrmID, frmID);
            string userNo = GetRequestVal("UserNo");
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.No;
            foreach (CtrlModel ctrlM in ctrlMs)
            {
                int isTrue = 0;
                if (ctrlM.IsEnableAll == true)
                    isTrue = 1;
                else
                {
                    //根据设置的权限来判断
                    if (ctrlM.IsEnableStation == true)
                    {
                        string stations = ctrlM.IDOfStations;
                        stations = stations.Trim(',');
                        stations = stations.Replace(",", "','");
                        stations = "'" + stations + "'";
                        string sql = "SELECT * From Port_DeptEmpStation DES,Port_Emp E WHERE  E.No=DES.FK_Emp AND E.No='" + userNo + "' AND DES.FK_Station IN(" + stations + ")";
                        if (DBAccess.RunSQLReturnCOUNT(sql) > 1)
                            isTrue = 1;
                    }

                    if (ctrlM.IsEnableUser == true && isTrue == 0)
                    {
                        string emps = ctrlM.IDOfUsers;
                        if (emps.Contains("," + userNo + ",") == true)
                            isTrue = 1;
                    }

                    if (ctrlM.IsEnableDept == true && isTrue == 0)
                    {
                        string depts = ctrlM.IDOfDepts;
                        depts = depts.Trim(',');
                        depts = depts.Replace(",", "','");
                        depts = "'" + depts + "'";
                        string sql = "SELECT * From Port_DeptEmp D,Port_Emp E WHERE  E.No=D.FK_Emp AND E.No='" + userNo + "' AND D.FK_Dept IN(" + depts + ")";
                        if (DBAccess.RunSQLReturnCOUNT(sql) > 1)
                            isTrue = 1;
                    }
                   
                }

                if (ctrlM.CtrlObj.Equals("BtnNew") == true)
                    ht.Add("IsInsert", isTrue);
                if (ctrlM.CtrlObj.Equals("BtnSave") == true)
                    ht.Add("IsSave", isTrue);
                if (ctrlM.CtrlObj.Equals("BtnSubmit") == true)
                    ht.Add("IsSubmit", isTrue);
                if (ctrlM.CtrlObj.Equals("BtnSearch") == true)
                    ht.Add("IsView", isTrue);
                if (ctrlM.CtrlObj.Equals("BtnDelete") == true)
                    ht.Add("IsDelete", isTrue);
               
            }
            ht.Add("IsShowDataVer", 1);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 获取单据，实体按钮权限集合
        /// </summary>
        /// <returns></returns>
        public string CCFrom_ToolBar_Init()
        {
            FrmDict frmDict = new FrmDict(this.FrmID);
            bool isReadonly = this.GetRequestValBoolen("IsReadonly");
            DataTable dt = new DataTable("ToolBar");
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Role", typeof(int));
            dt.Columns.Add("Icon");
            DataRow dr = dt.NewRow();
            //获取实体单据的权限
            string jsonString = CCFrom_FrmPower();
            //把json转换成实体
            JObject jObject = JObject.Parse(jsonString);
            //新建
            if(jObject.GetValue("IsInsert")!=null && jObject.GetValue("IsInsert").ToString().Equals("1") && isReadonly == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Add";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnNewLable) == true ? "新建" : frmDict.BtnNewLable;
                dr["Icon"] = "xinjian";
                dt.Rows.Add(dr);
            }
            //保存
            if (jObject.GetValue("IsSave")!=null && jObject.GetValue("IsSave").ToString().Equals("1") && isReadonly == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Save";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnSaveLable) == true ? "保存" : frmDict.BtnSaveLable;
                dr["Icon"] = "baocun";
                dt.Rows.Add(dr);
            }
            //提交
            if (jObject.GetValue("IsSubmit")!=null && jObject.GetValue("IsSubmit").ToString().Equals("1") && isReadonly == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Submit";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnSubmitLable) == true ? "提交" : frmDict.BtnSubmitLable;
                dr["Icon"] = "baocun";
                dt.Rows.Add(dr);
            }
            //删除
            if (jObject.GetValue("IsDelete")!=null && jObject.GetValue("IsDelete").ToString().Equals("1") && isReadonly == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Delete";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnDelLable) == true ? "删除" : frmDict.BtnDelLable;
                dr["Icon"] = "shanchu1";
                dt.Rows.Add(dr);
            }
            ////数据快照
            //if (jObject.GetValue("IsShowDataVer")!=null && jObject.GetValue("IsShowDataVer").ToString().Equals("1") && isReadonly == false)
            //{
            //    dr = dt.NewRow();
            //    dr["No"] = "DataVer";
            //    dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnDataVer) == true ? "数据快照" : frmDict.BtnDataVer;
            //    dr["Icon"] = "book";
            //    dt.Rows.Add(dr);
            //}

            //查询
            if (jObject.GetValue("IsView")!=null && jObject.GetValue("IsView").ToString().Equals("1") && isReadonly == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Search";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnSearchLabel) == true ? "查询" : frmDict.BtnSearchLabel;
                dr["Icon"] = "Search";
                dt.Rows.Add(dr);
            }
            if (frmDict.BtnGroupEnable == true)
            {
                //分组
                dr = dt.NewRow();
                dr["No"] = "Group";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnGroupLabel) == true ? "分析" : frmDict.BtnGroupLabel;
                dr["Icon"] = "fenzu";
                dt.Rows.Add(dr);
            }

            if (frmDict.BtnPrintHtmlEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Print";
                dr["Type"] = "HTML";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnPrintHtml) == true ? "打印HTML" : frmDict.BtnPrintHtml;
                dr["Icon"] = "print";
                dt.Rows.Add(dr);
            }

            if (frmDict.BtnPrintPDFEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Print";
                dr["Type"] = "PDF";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnPrintPDF) == true ? "打印PDF" : frmDict.BtnPrintPDF;
                dr["Icon"] = "print";
                dt.Rows.Add(dr);
            }

            if (frmDict.BtnPrintRTFEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Print";
                dr["Type"] = "RTF";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnPrintRTF) == true ? "打印RTF" : frmDict.BtnPrintRTF;
                dr["Icon"] = "print";
                dt.Rows.Add(dr);
            }

            if (frmDict.BtnPrintCCWordEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Print";
                dr["Type"] = "CCWord";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnPrintCCWord) == true ? "打印CCWord" : frmDict.BtnPrintCCWord;
                dr["Icon"] = "print";
                dt.Rows.Add(dr);
            }
            if (frmDict.BtnExpZipEnable ==true)
            {
                dr = dt.NewRow();
                dr["No"] = "Print";
                dr["Type"] = "ZIP";
                dr["Name"] = DataType.IsNullOrEmpty(frmDict.BtnExpZip) == true ? "打包ZIP" : frmDict.BtnExpZip;
                dr["Icon"] = "print";
                dt.Rows.Add(dr);
            }

            //关联流程
            DictFlows dictFlows = new BP.CCBill.Template.DictFlows();
            dictFlows.Retrieve("FrmID", this.FrmID);
            foreach(DictFlow dict in dictFlows)
            {
                dr = dt.NewRow();
                dr["No"] = "dictFlow";
                dr["Type"] = dict.FlowNo;
                dr["Name"] = dict.Label;
                dr["Icon"] = "shezhi";
                dt.Rows.Add(dr);
            }
            if (WebUser.No.Equals("admin") && this.IsMobile == false)
            {
                dr = dt.NewRow();
                dr["No"] = "Setting";
                dr["Name"] = "设置";
                dr["Icon"] = "shezhi";
                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }


        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        public string CCForm_Power_ViewList()
        {
            string userNo = GetRequestVal("UserNo");
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.No;
            string sql = "SELECT FrmID," +
                "(CASE WHEN IsEnableAll=1 THEN true " +
                "ELSE(CASE WHEN IsEnableUser=1 AND INSTR(IDOfUsers,'," + userNo + ",')>0 THEN true " +
                "ELSE(CASE WHEN IsEnableStation=1 AND (SELECT COUNT(*) From Port_DeptEmpStation D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND INSTR(IDOfStations,D.FK_Station))>0 THEN true " +
                "ELSE(CASE WHEN IsEnableDept=1 AND (SELECT COUNT(*) From Port_DeptEmp D,Port_Emp E WHERE D.FK_Emp = E.No AND E.No='" + userNo + "' AND INSTR(IDOfDepts,D.FK_Dept))>0 THEN true " +
                "ELSE false END)" +
                "END)" +
                "END)" +
                "END) AS IsView   FROM Frm_CtrlModel WHERE CtrlObj='BtnSearch'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "FrmView";
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 删除实体根据BillNo
        /// </summary>
        /// <returns></returns>
        public string CCFrom_DeleteFrmEntityByBillNo()
        {

            GEEntity en = new GEEntity(this.FrmID);
            int i = en.Retrieve("BillNo", this.GetRequestVal("BillNo"));
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
            GEEntity en = new GEEntity(this.FrmID, this.OID);
            en.Delete();
            return "删除成功";
        }
        #endregion 前台的操作 api.


        #region 后台操作api.
        /// <summary>
        /// 获得所有的单据、表单
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
