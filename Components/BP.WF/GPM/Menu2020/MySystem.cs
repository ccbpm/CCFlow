using System;
using System.Collections;
using System.Data;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.GPM.Menu2020
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
        /// <param name="mypk"></param>
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

                //RefMethod rm = new RefMethod();
                //rm.Title = "权限控制";
                //rm.ClassMethodName = this.ToString() + ".DoPowerCenter";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 权限控制
        /// </summary>
        /// <returns></returns>
        public string DoPowerCenter()
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
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.DoOrderUp(MySystemAttr.OrgNo, this.OrgNo, MySystemAttr.Idx);
            else
                this.DoOrderUp(MySystemAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
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
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                var i = base.RetrieveAll("Idx");
                if (i != 0)
                    return i;

                #region 初始化菜单.

                string file = SystemConfig.PathOfData + "\\XML\\AppFlowMenu.xml";
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

            //集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            if (BP.Sys.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        #endregion


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
