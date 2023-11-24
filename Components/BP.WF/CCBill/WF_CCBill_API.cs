using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.WF.HttpHandler;
using BP.CCBill.Template;
using BP.CCFast.CCMenu;

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
                userNo = WebUser.UserID;

            string powerSQL = "";

            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
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
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "EntityType";
            dt.Columns[3].ColumnName = "FrmType";
            dt.Columns[4].ColumnName = "PTable";


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

                if (DataType.IsNullOrEmpty(title) == false && en.GetValStrByKey("Title").Equals(title) == false)
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
        public ToolbarBtns CCFrom_FrmPower()
        {
            //获取该表单所有操作按钮的权限
            ToolbarBtns btns = new ToolbarBtns();
            btns.Retrieve(ToolbarBtnAttr.FrmID, this.FrmID, "Idx");
            bool isReadonly = this.GetRequestValBoolen("IsReadonly");
            if (btns.Count == 0)
            {
                MapData md = new MapData(this.FrmID);
                //表单的工具栏权限
                ToolbarBtn btn = new ToolbarBtn();

                btn.FrmID = md.No;
                btn.BtnID = "New";
                btn.BtnLab = "新建";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.SetValByKey("Idx", 0);
                btn.Insert();


                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "Save";
                btn.BtnLab = "保存";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.SetValByKey("Idx", 1);
                btn.Insert();

                if (md.EntityType == EntityType.FrmBill)
                {
                    //单据增加提交的功能
                    btn = new ToolbarBtn();
                    btn.FrmID = md.No;
                    btn.BtnID = "Submit";
                    btn.BtnLab = "归档";
                    btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                    btn.SetValByKey("Idx", 1);
                    btn.Insert();
                }

                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "Delete";
                btn.BtnLab = "删除";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.SetValByKey("Idx", 2);
                btn.Insert();


                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "PrintHtml";
                btn.BtnLab = "打印Html";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 3);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "PrintPDF";
                btn.BtnLab = "打印PDF";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 4);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "PrintRTF";
                btn.BtnLab = "打印RTF";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 5);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "PrintCCWord";
                btn.BtnLab = "打印CCWord";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 6);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = md.No;
                btn.BtnID = "ExpZip";
                btn.BtnLab = "导出Zip包";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 7);
                btn.Insert();

                btns.Retrieve(ToolbarBtnAttr.FrmID, this.FrmID, ToolbarBtnAttr.IsEnable, 1, "Idx");
                return btns;
            }

            //获取针对按钮设置的操作权限
            PowerCenters pcs = new PowerCenters();
            pcs.Retrieve(PowerCenterAttr.CtrlObj, this.FrmID, PowerCenterAttr.CtrlGroup, "FrmBtn");

            string mydepts = "" + WebUser.DeptNo + ","; //我的部门.
            string mystas = ""; //我的角色.

            DataTable mydeptsDT = DBAccess.RunSQLReturnTable("SELECT FK_Dept,FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.UserID + "'");
            foreach (DataRow dr in mydeptsDT.Rows)
            {
                mydepts += dr[0].ToString() + ",";
                mystas += dr[1].ToString() + ",";
            }

            ToolbarBtns newBtns = new ToolbarBtns();
            string empIds = "";
            foreach (ToolbarBtn btn in btns)
            {
                if (btn.ItIsEnable == false)
                    continue;
                if (isReadonly == true && (btn.BtnID.Equals("New") || btn.BtnID.Equals("Save") || btn.BtnID.Equals("Submit") || btn.BtnID.Equals("Delete")))
                    continue;
                //找到关于系统的控制权限集合.
                PowerCenters mypcs = pcs.GetEntitiesByKey(PowerCenterAttr.CtrlPKVal, btn.MyPK) as PowerCenters;
                //如果没有权限控制的描述，就默认有权限.
                if (mypcs == null)
                {
                    newBtns.AddEntity(btn);
                    continue;
                }

                //控制遍历权限.
                foreach (PowerCenter pc in mypcs)
                {
                    if (pc.CtrlModel.Equals("Anyone") == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }
                    if (pc.CtrlModel.Equals("Adminer") == true && BP.Web.WebUser.No.Equals("admin") == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }

                    if (pc.CtrlModel.Equals("AdminerAndAdmin2") == true && BP.Web.WebUser.IsAdmin == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }
                    empIds = "," + pc.IDs + ",";
                    if (pc.CtrlModel.Equals("Emps") == true && empIds.Contains("," + BP.Web.WebUser.No + ",") == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }

                    //是否包含部门？
                    if (pc.CtrlModel.Equals("Depts") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mydepts) == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }

                    //是否包含角色？
                    if (pc.CtrlModel.Equals("Stations") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mystas) == true)
                    {
                        newBtns.AddEntity(btn);
                        break;
                    }

                    //SQL？
                    if (pc.CtrlModel.Equals("SQL") == true)
                    {
                        string sql = BP.WF.Glo.DealExp(pc.IDs, null, "");
                        if (DBAccess.RunSQLReturnValFloat(sql) > 0)
                        {
                            newBtns.AddEntity(btn);
                        }
                        break;
                    }
                }
            }
            return newBtns;
        }
        /// <summary>
        /// 获取单据，实体按钮权限集合
        /// </summary>
        /// <returns></returns>
        public string CCFrom_ToolBar_Init()
        {
            //获取实体单据的权限
            ToolbarBtns btns = CCFrom_FrmPower();
            return BP.Tools.Json.ToJson(btns.ToDataTableField("Frm_ToolbarBtn"));

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
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "EntityType";
            dt.Columns[3].ColumnName = "FrmType";
            dt.Columns[4].ColumnName = "PTable";


            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

    }
}
