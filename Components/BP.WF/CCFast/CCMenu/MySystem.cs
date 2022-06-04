using System;
using System.Data;
using System.IO;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.CCBill.Template;
using BP.WF;
using BP.WF.Template;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 系统
    /// </summary>
    public class MySystemAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string MySystemModel = "MySystemModel";
        /// <summary>
        /// UrlExt
        /// </summary>
        public const string UrlExt = "UrlExt";
        /// <summary>
        /// SubUrl
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 是否启用.
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 关联菜单编号
        /// </summary>
        public const string RefMenuNo = "RefMenuNo";
        public const string Icon = "Icon";
    }
    /// <summary>
    /// 系统
    /// </summary>
    public class MySystem : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 打开方式
        /// </summary>
        public string OpenWay
        {
            get
            {
                int openWay = 0;

                switch (openWay)
                {
                    case 0:
                        return "_blank";
                    case 1:
                        return this.No;
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey("WebPath");
            }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MySystemAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(MySystemAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(MySystemAttr.Idx);
            }
            set
            {
                this.SetValByKey(MySystemAttr.Idx, value);
            }
        }
        /// <summary>
        /// Icon
        /// </summary>
        public string Icon
        {
            get
            {
                return this.GetValStrByKey(MySystemAttr.Icon);
            }
            set
            {
                this.SetValByKey(MySystemAttr.Icon, value);
            }
        }


        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(MySystemAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(MySystemAttr.OrgNo, value);
            }
        }
        public string RefMenuNo
        {
            get
            {
                return this.GetValStringByKey(MySystemAttr.RefMenuNo);
            }
            set
            {
                this.SetValByKey(MySystemAttr.RefMenuNo, value);
            }
        }
        #endregion

        #region 按钮权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 系统
        /// </summary>
        public MySystem()
        {
        }
        /// <summary>
        /// 系统
        /// </summary>
        /// <param name="no"></param>
        public MySystem(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_System", "系统"); 
                map.DepositaryOfEntity = Depositary.None;

                map.AddTBStringPK(MySystemAttr.No, null, "编号", true, false, 2, 100, 100);
                map.AddTBString(MySystemAttr.Name, null, "名称", true, false, 0, 300, 150, true);
                map.AddBoolean(MySystemAttr.IsEnable, true, "启用?", true, true);
                map.AddTBString(MySystemAttr.Icon, null, "图标", true, false, 0, 50, 150, true);

                map.AddTBString(MenuAttr.OrgNo, null, "组织编号", true, false, 0, 50, 20);
                map.AddTBInt(MySystemAttr.Idx, 0, "显示顺序", true, false);

                RefMethod rm = new RefMethod();
                rm.Title = "导出应用模板";
                rm.ClassMethodName = this.ToString() + ".DoExpAppModel";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            string path = BP.Difference.SystemConfig.PathOfWebApp + "CCFast/SystemTemplete/" + this.Name + "/";
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            //系统属性.
            DataSet ds = new DataSet();
            ds.Tables.Add(this.ToDataTableField("MySystem"));

            //模块.
            Modules ens = new Modules();
            ens.Retrieve(ModuleAttr.SystemNo, this.No);
            ds.Tables.Add(ens.ToDataTableField("Modules"));

            //菜单.
            Menus menus = new Menus();
            menus.Retrieve(MenuAttr.SystemNo, this.No);
            ds.Tables.Add(menus.ToDataTableField("Menus"));

            string file = path + "Menus.xml"; //默认的页面.
            ds.WriteXml(file);

            //遍历菜单.
            foreach (Menu en in menus)
            {
                ////常规的功能，不需要备份.
                //if (en.Mark.Equals("WorkRec") == true
                //    || en.Mark.Equals("Calendar") == true
                //    || en.Mark.Equals("Notepad") == true)
                //    continue;
                switch (en.MenuModel)
                {
                    case "WorkRec":
                    case "Calendar":
                    case "Notepad":
                    case "Task":
                    case "KnowledgeManagement":
                        break;
                    case "Dict": //如果是实体.
                        Dict(en, path);
                        break;
                    case "DictTable":  //如果是字典.
                        DictTable(en, path);
                        break;
                    default:
                        //    throw new Exception("err@没有判断的应用类型:" + en.Mark);
                        break;
                }
            }

            return "执行成功. 导出到：" + path;
        }
        public string DictTable(Menu en, string path)
        {
            DataSet ds = new DataSet();

            SFTable sf = new SFTable(en.UrlExt);

            ds.Tables.Add(sf.ToDataTableField("SFTable"));

            DataTable dt = sf.GenerHisDataTable();
            dt.TableName = "Data";
            ds.Tables.Add(dt);

            ds.WriteXml(path + en.UrlExt + ".xml");
            return "";
        }
        /// <summary>
        /// 导出字典.
        /// </summary>
        /// <returns></returns>
        public string Dict(Menu en, string path)
        {
            //获得表单的ID.
            string frmID = en.UrlExt;

            DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(frmID);
            string file = path + "/" + frmID + ".xml"; //实体方法.
            ds.WriteXml(file);

            #region 导出实体的方法 .
            //获得方法分组
            BP.CCBill.Template.GroupMethods ensGroup = new BP.CCBill.Template.GroupMethods();
            ensGroup.Retrieve(MethodAttr.FrmID, frmID);

            //获得方法.
            BP.CCBill.Template.Methods ens = new BP.CCBill.Template.Methods();
            ens.Retrieve(MethodAttr.FrmID, frmID);

            //保存方法.
            ds = new DataSet();
            ds.Tables.Add(ensGroup.ToDataTableField("GroupMethods"));
            ds.Tables.Add(ens.ToDataTableField("Methods"));

            file = path + frmID + "_GroupMethods.xml"; //实体方法.
            ds.WriteXml(file);

            //循环单实体方法集合.
            foreach (BP.CCBill.Template.Method method in ens)
            {
                switch (method.MethodModel)
                {
                    case "FlowEtc": //流程
                        BP.WF.Flow f2l1 = new BP.WF.Flow(method.MethodID);
                        f2l1.DoExpFlowXmlTemplete(path + method.MethodID + "_Flow");
                        break;
                    case "FlowBaseData": //流程
                        BP.WF.Flow fl1 = new BP.WF.Flow(method.MethodID);
                        fl1.DoExpFlowXmlTemplete(path + method.MethodID + "_Flow");
                        break;
                    case "Func": //功能导出？
                        break;
                    default:
                        break;
                }
            }
            #endregion 导出实体的方法 .

            #region 导出集合 .
            //获得方法分组
            CCBill.Template.Collections ensCollts = new BP.CCBill.Template.Collections();
            ensCollts.Retrieve(CollectionAttr.FrmID, frmID);

            //保存方法.
            ds = new DataSet();
            ds.Tables.Add(ensCollts.ToDataTableField("Collections"));

            file = path + "/" + frmID + "_Collections.xml"; //实体方法.
            ds.WriteXml(file);

            //循环单实体方法集合.
            foreach (BP.CCBill.Template.Collection method in ensCollts)
            {
                switch (method.MethodModel)
                {
                    case "FlowEntityBatchStart": //流程
                        BP.WF.Flow fC1 = new BP.WF.Flow(method.FlowNo);
                        fC1.DoExpFlowXmlTemplete(path + method.FlowNo + "_Flow");
                        break;
                    case "FlowNewEntity": //流程
                        BP.WF.Flow fc2 = new BP.WF.Flow(method.FlowNo);
                        fc2.DoExpFlowXmlTemplete(path + method.FlowNo + "_Flow");
                        break;
                    default:
                        break;
                }
            }
            #endregion 导出实体的方法 .

            return "实体导出成功";
        }
        /// <summary>
        /// 导出应用模板
        /// </summary>
        /// <returns></returns>
        public string DoExpAppModel()
        {
            return "../../GPM/PowerCenter.htm?CtrlObj=System&CtrlPKVal=" + this.No + "&CtrlGroup=System";
        }

        /// <summary>
        /// 业务处理.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID(10);

            this.OrgNo = BP.Web.WebUser.OrgNo;
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {
            Modules ens = new Modules();
            ens.Retrieve(ModuleAttr.SystemNo, this.No);
            if (ens.Count != 0)
                throw new Exception("err@该系统下有子模块，您不能删除。");

            //看看这个类别下是否有表单，如果有就删除掉.
            string sql = "SELECT COUNT(No) AS No FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                DBAccess.RunSQL("DELETE FROM Sys_FormTree WHERE No='" + this.No + "' ");

            //看看这个类别下是否有流程，如果有就删除掉.
            sql = "SELECT COUNT(No) AS No FROM WF_Flow WHERE FK_FlowSort='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                DBAccess.RunSQL("DELETE FROM WF_FlowSort WHERE No='" + this.No + "' ");

            return base.beforeDelete();
        }

        #region 移动方法.
        /// <summary>
        /// 向上移动
        /// </summary>
        public void DoUp()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.DoOrderUp(MySystemAttr.OrgNo, this.OrgNo, MySystemAttr.Idx);
            else
                this.DoOrderUp(MySystemAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.DoOrderDown(MySystemAttr.OrgNo, this.OrgNo, MySystemAttr.Idx);
            else
                this.DoOrderDown(MySystemAttr.Idx);
        }
        #endregion 移动方法.

    }
    /// <summary>
    /// 系统s
    /// </summary>
    public class MySystems : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 系统s
        /// </summary>
        public MySystems()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MySystem();
            }
        }
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                var i = base.RetrieveAll("Idx");
                if (i != 0)
                    return i;

                #region 初始化菜单.

                string file =  BP.Difference.SystemConfig.PathOfData + "XML/AppFlowMenu.xml";
                DataSet ds = new DataSet();
                ds.ReadXml(file);

                //增加系统.
                foreach (DataRow dr in ds.Tables["MySystem"].Rows)
                {
                    MySystem en = new MySystem();
                    en.No = dr["No"].ToString();
                    en.Name = dr["Name"].ToString();
                    en.Icon = dr["Icon"].ToString();
                    en.IsEnable = true;
                    en.Insert();
                }

                //增加模块.
                foreach (DataRow dr in ds.Tables["Module"].Rows)
                {
                    Module en = new Module();
                    en.No = dr["No"].ToString();
                    en.Name = dr["Name"].ToString();
                    en.SystemNo = dr["SystemNo"].ToString();
                    en.Icon = dr["Icon"].ToString();

                    // en.MenuCtrlWay = 1; 
                    //en.IsEnable = true;
                    en.Insert();
                }

                //增加连接.
                foreach (DataRow dr in ds.Tables["Item"].Rows)
                {
                    Menu en = new Menu();
                    en.No = dr["No"].ToString();
                    en.Name = dr["Name"].ToString();
                    //   en.SystemNo = dr["SystemNo"].ToString();
                    en.ModuleNo = dr["ModuleNo"].ToString();
                    en.UrlExt = dr["Url"].ToString();
                    en.Icon = dr["Icon"].ToString();
                    en.Insert();
                }

                #endregion 初始化菜单.

                return RetrieveAll();
            }

            ////集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            //if (BP.Difference.SystemConfig.GroupStationModel == 1)
            //    return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        #endregion

        /// <summary>
        /// 获得系统列表
        /// </summary>
        /// <returns></returns>
        public string ImpSystem_Init()
        {
            string path = BP.Difference.SystemConfig.PathOfWebApp + "CCFast/SystemTemplete/";
            string[] strs = System.IO.Directory.GetDirectories(path);

            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");

            foreach (string str in strs)
            {
                System.IO.DirectoryInfo en = new System.IO.DirectoryInfo(str);
                DataRow dc = dt.NewRow();
                dc[0] = en.Name;
                dc[1] = en.Name;
                dt.Rows.Add(dc);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        public string DealGUIDNo(string no)
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return no;

            if (no.Contains("_") == true)
                no = no.Substring(no.IndexOf('_'));

            return BP.Web.WebUser.OrgNo + "_" + no;
        }
        /// <summary>
        /// 导入系统
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ImpSystem_Imp(string name)
        {
            string path = BP.Difference.SystemConfig.PathOfWebApp + "CCFast/SystemTemplete/" + name;
            string pathOfMenus = path + "/Menus.xml";
            if (File.Exists(pathOfMenus) == false)
                return "err@系统错误，目录里缺少文件:" + pathOfMenus;

            DataSet ds = new DataSet();
            ds.ReadXml(pathOfMenus);

            //创建系统.
            DataTable dt = ds.Tables["MySystem"];
            MySystem system = new MySystem();
            Row row = system.Row;
            row.LoadDataTable(dt, dt.Rows[0]);

            //旧的orgNo.
            string oldOrgNo = system.OrgNo;

            system.No = this.DealGUIDNo(system.No);
            if (system.IsExits == true)
                return "err@系统:" + name + ",已经存在.您不能在导入.";

            system.OrgNo = BP.Web.WebUser.OrgNo;
            system.DirectInsert();

            //创建流程目录..
            FlowSort fs = new FlowSort();
            fs.No = system.No;
            fs.Name = system.Name;
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                fs.ParentNo = "100";
            else
            {
                fs.OrgNo = BP.Web.WebUser.OrgNo;
                fs.ParentNo = system.No;
            }
            if (fs.IsExits == true)
                fs.DirectUpdate();
            else
                fs.DirectInsert();

            //创建model.
            dt = ds.Tables["Modules"];
            Modules modules = new Modules();

            foreach (DataRow dr in dt.Rows)
            {
                Module en = new Module();
                en.Row.LoadDataTable(dt, dr);
                en.OrgNo = BP.Web.WebUser.OrgNo;
                en.SystemNo = system.No; //重新赋值，有可能这个编号有变化。
                en.No = this.DealGUIDNo(en.No);  //修改编号格式，防止重复导入，在saas模式下，可以重复导入。
                en.DirectInsert();
                modules.AddEntity(en);
            }

            //创建menus.
            dt = ds.Tables["Menus"];

            foreach (DataRow dr in dt.Rows)
            {
                BP.CCFast.CCMenu.Menu en = new BP.CCFast.CCMenu.Menu();
                en.Row.LoadDataTable(dt, dr);
                en.OrgNo = BP.Web.WebUser.OrgNo;

                en.SystemNo = system.No; //重新赋值，有可能这个编号有变化。
                                         //en.ModuleNo = "";

                int idx = en.ModuleNo.IndexOf('_');
                if (idx > 0)
                    en.ModuleNo = en.ModuleNo.Substring(idx);

                Module myModule = null;

                //解决对应的模块编号变化的问题.
                foreach (Module item in modules)
                {
                    if (en.ModuleNo.Contains(item.No) == true)
                    {
                        en.ModuleNo = item.No;
                        myModule = item;
                        continue;
                    }
                }

                //设置模块编号.
                en.ModuleNo = myModule.No; //

                en.No = this.DealGUIDNo(en.No);  //修改编号格式，防止重复导入，在saas模式下，可以重复导入。

                switch (en.MenuModel)
                {
                    case "Dict":
                        ImpSystem_Imp_Dict(en, path, system, myModule, oldOrgNo);
                        break;
                    case "DictTable":
                        ImpSystem_Imp_DictTable(en, path);
                        break;
                    default:
                        break;
                }
                en.DirectInsert();
            }

            return "执行成功.";
        }
        private void ImpSystem_Imp_DictTable(Menu en, string path)
        {

        }
        /// <summary>
        /// 导入实体
        /// </summary>
        /// <param name="en"></param>
        /// <param name="path"></param>
        private void ImpSystem_Imp_Dict(Menu en, string path, MySystem system, Module module, string oldOrgNo)
        {
            string frmID = en.UrlExt;

            //导入表单.
            string file = path + "/" + frmID + ".xml";
            DataSet ds = new DataSet();
            ds.ReadXml(file);

            //旧的OrgNo 替换为新的orgNo.
            string realFrmID = en.UrlExt;
            if (DataType.IsNullOrEmpty(oldOrgNo) == false)
                realFrmID = frmID.Replace(oldOrgNo, BP.Web.WebUser.OrgNo);

            MapData.ImpMapData(realFrmID, ds);
            MapData md = new MapData(realFrmID);

            if (DataType.IsNullOrEmpty(oldOrgNo) == false)
                md.PTable = md.PTable.Replace(oldOrgNo, BP.Web.WebUser.OrgNo);

            md.Update();

            file = path + "/" + frmID + "_GroupMethods.xml";

            //导入单个实体的方法分组.
            ds = new DataSet();
            ds.ReadXml(file);
            DataTable dt = ds.Tables["GroupMethods"];
            foreach (DataRow dr in dt.Rows)
            {
                GroupMethod gm = new GroupMethod();
                gm.Row.LoadDataTable(dt, dr);
                gm.OrgNo = Web.WebUser.OrgNo;
                gm.FrmID = realFrmID;
                gm.No = DBAccess.GenerGUID();
                gm.DirectInsert();
            }

            dt = ds.Tables["Methods"];
            foreach (DataRow dr in dt.Rows)
            {
                BP.CCBill.Template.Method myen = new BP.CCBill.Template.Method();
                myen.Row.LoadDataTable(dt, dr);

                myen.FrmID = realFrmID;

                switch (myen.MethodModel)
                {
                    case "FlowEtc": //其他业务流程.
                        myen.FlowNo = ImpSystem_Imp_Dict_FlowEtc(myen.FlowNo, myen.Name, path, system);
                        break;
                    case "FlowBaseData": //修改基础资料流程
                        myen.FlowNo = ImpSystem_Imp_Dict_FlowEtc(myen.FlowNo, myen.Name, path, system);
                        break;
                    case "Func": //功能.
                        break;
                    default:
                        break;
                }
                //    en.OrgNo = Web.WebUser.OrgNo;
                myen.No = DBAccess.GenerGUID();
                myen.DirectInsert();
            }

            //导入实体集合.
            file = path + "/" + frmID + "_Collections.xml";
            ds.ReadXml(file);
            dt = ds.Tables["GroupMethods"];
            foreach (DataRow dr in dt.Rows)
            {
                BP.CCBill.Template.Collection myen = new BP.CCBill.Template.Collection();
                myen.Row.LoadDataTable(dt, dr);
                myen.FrmID = realFrmID;

                switch (myen.MethodModel)
                {
                    case "FlowEntityBatchStart": //批量发起流程.
                        ImpSystem_Imp_Dict_FlowEtc(myen.FlowNo, myen.Name, path, system);
                        break;
                    case "FlowNewEntity": //新建流程
                        ImpSystem_Imp_Dict_FlowEtc(myen.FlowNo, myen.Name, path, system);
                        break;
                    case "Func": //功能.
                        break;
                    default:
                        break;
                }
                myen.DirectInsert();
            }
        }
        /// <summary>
        /// 导入流程.
        /// </summary>
        /// <param name="en"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ImpSystem_Imp_Dict_FlowEtc(string tempFlowNo, string tempFlowName, string path, MySystem mysystem)
        {
            //导入模式
            BP.WF.ImpFlowTempleteModel model = ImpFlowTempleteModel.AsNewFlow;

            path = path + "/" + tempFlowNo + "_Flow/" + tempFlowName + ".xml";
            //   if (model == ImpFlowTempleteModel.AsSpecFlowNo)
            //     flowNo = this.GetRequestVal("SpecFlowNo");

            //执行导入
            BP.WF.Flow flow = BP.WF.Template.TemplateGlo.LoadFlowTemplate(mysystem.No, path, model, null);
            flow.FK_FlowSort = mysystem.No;
            flow.DoCheck(); //要执行一次检查.

            return flow.No;


            //Hashtable ht = new Hashtable();
            //ht.Add("FK_Flow", flow.No); //流程编号.
            //ht.Add("FlowName", flow.Name); //名字.
            //ht.Add("FK_FlowSort", flow.FK_FlowSort); //类别.
            //ht.Add("Msg", "导入成功,流程编号为:" + flow.No + "名称为:" + flow.Name);
            //return BP.Tools.Json.ToJson(ht);
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MySystem> ToJavaList()
        {
            return (System.Collections.Generic.IList<MySystem>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MySystem> Tolist()
        {
            System.Collections.Generic.List<MySystem> list = new System.Collections.Generic.List<MySystem>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MySystem)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
