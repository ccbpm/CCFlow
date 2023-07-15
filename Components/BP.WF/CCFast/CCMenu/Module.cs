using System;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 模块
    /// </summary>
    public class ModuleAttr : EntityTreeAttr
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public const string SystemNo = "SystemNo";
        /// <summary>
        /// 标记
        /// </summary>
        public const string Remark = "Remark";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// Icon
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// 是否启用？
        /// </summary>
        public const string IsEnable = "IsEnable";
    }
    /// <summary>
    /// 模块
    /// </summary>
    public class Module : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(ModuleAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(ModuleAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SystemNo
        {
            get
            {
                return this.GetValStrByKey(ModuleAttr.SystemNo);
            }
            set
            {
                this.SetValByKey(ModuleAttr.SystemNo, value);
            }
        }
        public string Icon
        {
            get
            {
                return this.GetValStrByKey(ModuleAttr.Icon);
            }
            set
            {
                this.SetValByKey(ModuleAttr.Icon, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(ModuleAttr.Idx);
            }
            set
            {
                this.SetValByKey(ModuleAttr.Idx, value);
            }
        }
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(ModuleAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(ModuleAttr.IsEnable, value);
            }
        }
        
        #endregion

        #region 按钮权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = false;
                    return uac;
                }
                else
                {
                    uac.Readonly();
                }
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 模块
        /// </summary>
        public Module()
        {
        }

        /// <summary>
        /// 模块
        /// </summary>
        /// <param name="no"></param>
        public Module(string no)
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
                Map map = new Map("GPM_Module", "模块");
                map.setEnType(EnType.App);
                map.setIsAutoGenerNo(  false);

                map.AddTBStringPK(ModuleAttr.No, null, "编号", true, true, 1, 200, 20);
                map.AddTBString(ModuleAttr.Name, null, "名称", true, false, 0, 300, 20);
                map.AddDDLEntities(ModuleAttr.SystemNo, null, "隶属系统", new MySystems(), true);
                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 500, 50, true);
                map.AddTBInt(ModuleAttr.Idx, 0, "顺序", true, false);
                map.AddTBInt(ModuleAttr.IsEnable, 1, "IsEnable", true, false);
                map.AddTBString(ModuleAttr.OrgNo, null, "组织编号", true, false, 0, 50, 20);

                map.AddTBInt("ChildDisplayModel", 0, "ChildDisplayModel", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 移动方法.
        /// <summary>
        /// 向上移动
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(ModuleAttr.SystemNo, this.SystemNo, ModuleAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(ModuleAttr.SystemNo, this.SystemNo, ModuleAttr.Idx);
        }
        #endregion 移动方法.

        /// <summary>
        /// 业务处理.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            this.OrgNo = BP.Web.WebUser.OrgNo;
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            Menus ens = new Menus();
            ens.Retrieve(MenuAttr.ModuleNo, this.No);
            if (ens.Count != 0)
                throw new Exception("err@该模块下有菜单，您不能删除。");

            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 模块s
    /// </summary>
    public class Modules : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 模块s
        /// </summary>
        public Modules()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Module();
            }
        }
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll("Idx");

            ////集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            //if (BP.Difference.SystemConfig.GroupStationModel == 1)
            //    return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Module> ToJavaList()
        {
            return (System.Collections.Generic.IList<Module>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Module> Tolist()
        {
            System.Collections.Generic.List<Module> list = new System.Collections.Generic.List<Module>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Module)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
