using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.WF.HttpHandler;
using BP.CCBill.Template;
using BP.Difference;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin : DirectoryPageBase
    {

        /// <summary>
        /// 工具栏按钮
        /// </summary>
        /// <returns></returns>
        public string ToolbarSetting_Init()
        {
            ToolbarBtns btns = new ToolbarBtns(); 
            int i = btns.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            if (i == 0)
            {
                FrmBill bill = new FrmBill(this.FrmID);
                ToolbarBtn btn = new ToolbarBtn();
                if (bill.EntityType != EntityType.DBList)
                {
                    btn.FrmID = this.FrmID;
                    btn.BtnID = "New";
                    btn.BtnLab = "新建";
                    btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                    btn.SetValByKey("Idx", 0);
                    btn.Insert();


                    btn = new ToolbarBtn();
                    btn.FrmID = this.FrmID;
                    btn.BtnID = "Save";
                    btn.BtnLab = "保存";
                    btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                    btn.SetValByKey("Idx", 1);
                    btn.Insert();


                    if (bill.EntityType == EntityType.FrmBill)
                    {
                        //单据增加提交的功能
                        btn = new ToolbarBtn();
                        btn.FrmID = this.FrmID;
                        btn.BtnID = "Submit";
                        btn.BtnLab = "提交";
                        btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                        btn.SetValByKey("Idx", 1);
                        btn.Insert();
                    }

                    btn = new ToolbarBtn();
                    btn.FrmID = this.FrmID;
                    btn.BtnID = "Delete";
                    btn.BtnLab = "删除";
                    btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                    btn.SetValByKey("Idx", 2);
                    btn.Insert();
                }
                
              
                btn = new ToolbarBtn();
                btn.FrmID = this.FrmID;
                btn.BtnID = "PrintHtml";
                btn.BtnLab = "打印Html";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 3);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = this.FrmID;
                btn.BtnID = "PrintPDF";
                btn.BtnLab = "打印PDF";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 4);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = this.FrmID;
                btn.BtnID = "PrintRTF";
                btn.BtnLab = "打印RTF";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 5);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = this.FrmID;
                btn.BtnID = "PrintCCWord";
                btn.BtnLab = "打印CCWord";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 6);
                btn.Insert();

                btn = new ToolbarBtn();
                btn.FrmID = this.FrmID;
                btn.BtnID = "ExpZip";
                btn.BtnLab = "导出Zip包";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.ItIsEnable = false;
                btn.SetValByKey("Idx", 7);
                btn.Insert();

                btns.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            }
            return btns.ToJson(); 
        }
        /// <summary>
        /// 实体、单据工具栏操作按钮的顺序移动
        /// </summary>
        /// <returns></returns>
        public string ToolbarSetting_Mover()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enMyPK = ens[i];
                if (DataType.IsNullOrEmpty(enMyPK) == true)
                    continue;
                string sql = "UPDATE Frm_ToolbarBtn SET Idx=" + i + " WHERE MyPK='" + enMyPK + "'";
                DBAccess.RunSQL(sql);
            }
            return "顺序移动成功..";
        }


        #region 方法的操作.
        /// <summary>
        /// 方法的初始化
        /// </summary>
        /// <returns></returns>
        public string Method_Init()
        {
            BP.CCBill.Template.GroupMethod gnn = new GroupMethod();

            GroupMethods gms = new GroupMethods();
            int i = gms.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            if (i == 0)
            {
                GroupMethod gm = new GroupMethod();
                gm.FrmID = this.FrmID;
                gm.Name = "相关操作";
                gm.MethodType = "Home";
                gm.Icon = "icon-home";
                gm.Insert();
                gms.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            }

            DataTable dtGroups = gms.ToDataTableField("Groups");
            dtGroups.TableName = "Groups";

            Methods methods = new Methods();
         //   methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsEnable, 1, "Idx");
            methods.Retrieve(MethodAttr.FrmID, this.FrmID,  "Idx");

            DataTable dtMethods = methods.ToDataTableField("Methods");
            dtMethods.TableName = "Methods";

            DataSet ds = new DataSet();
            ds.Tables.Add(dtGroups);
            ds.Tables.Add(dtMethods);

            return BP.Tools.Json.ToJson(ds);

        }
        /// <summary>
        /// 移动分组
        /// </summary>
        /// <returns></returns>
        public string Method_MoverGroup()
        {
            string[] ens = this.GetRequestVal("GroupIDs").Split(',');
            string frmID = this.FrmID;
            for (int i = 0; i < ens.Length; i++)
            {
                string en = ens[i];

                string sql = "UPDATE Frm_GroupMethod SET Idx=" + i + " WHERE No='" + en + "' AND FrmID='" + frmID + "'";
                DBAccess.RunSQL(sql);
            }
            return "目录移动成功..";
        }
        /// <summary>
        /// 移动方法.
        /// </summary>
        /// <returns></returns>
        public string Method_MoverMethod()
        {
            string sortNo = this.GetRequestVal("GroupID");

            string[] ens = this.GetRequestVal("MethodIDs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enNo = ens[i];
                string sql = "UPDATE Frm_Method SET GroupID ='" + sortNo + "',Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "方法顺序移动成功..";
        }
        #endregion 方法的操作.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin()
        {

        }
        /// <summary>
        /// 列表集合初始化
        /// </summary>
        /// <returns></returns>
        public string Collection_Init()
        {
            Collections collections = new Collections();
            int i = collections.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            if (i == 0)
            {
                FrmBill bill = new FrmBill(this.FrmID);
                //查询
                Collection collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "Search";
                collection.Name = "查询";
                collection.MethodModel = "Search";
                collection.Mark = "Search";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 0);
                collection.Insert();

                if (bill.EntityType != EntityType.DBList)
                {
                    //新建
                    collection = new Collection();
                    collection.FrmID = this.FrmID;
                    collection.MethodID = "New";
                    collection.Name = "新建";
                    collection.MethodModel = "New";
                    collection.Mark = "New";
                    collection.No = collection.FrmID + "_" + collection.MethodID;
                    collection.SetValByKey("Idx", 1);
                    collection.Insert();

                    //删除
                    collection = new Collection();
                    collection.FrmID = this.FrmID;
                    collection.MethodID = "Delete";
                    collection.Name = "删除";
                    collection.MethodModel = "Delete";
                    collection.Mark = "Delete";
                    collection.No = collection.FrmID + "_" + collection.MethodID;
                    collection.SetValByKey("Idx", 2);
                    collection.Insert();

                    //导入
                    collection = new Collection();
                    collection.FrmID = this.FrmID;
                    collection.MethodID = "ImpExcel";
                    collection.Name = "导入Excel";
                    collection.MethodModel = "ImpExcel";
                    collection.Mark = "ImpExcel";
                    collection.No = collection.FrmID + "_" + collection.MethodID;
                    collection.SetValByKey("Idx", 5);
                    collection.Insert();
                }
                

                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "Group";
                collection.Name = "分析";
                collection.MethodModel = "Group";
                collection.Mark = "Group";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 3);
                collection.SetValByKey("IsEnable", false);
                collection.Insert();

                //导出
                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "ExpExcel";
                collection.Name = "导出Excel";
                collection.MethodModel = "ExpExcel";
                collection.Mark = "ExpExcel";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 4);
                collection.Insert();

               
                collections.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            }
            return collections.ToJson();
        }

        /// <summary>
        /// 集合方法的移动.
        /// </summary>
        /// <returns></returns>
        public string Collection_Mover()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enNo = ens[i];
                if (DataType.IsNullOrEmpty(enNo) == true)
                    continue;
                string sql = "UPDATE Frm_Collection SET Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "顺序移动成功..";
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
            throw new Exception("@标记[" + this.DoType + "]DoMethod=[" + this.GetRequestVal("DoMethod") + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.
    }
}
