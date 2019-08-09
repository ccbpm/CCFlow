using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 系统类别
    /// </summary>
    public class AppSortAttr : EntityNoNameAttr
    {
        public const string Idx = "Idx";
        /// <summary>
        /// 关联的菜单编号
        /// </summary>
        public const string RefMenuNo = "RefMenuNo";
    }
    /// <summary>
    /// 系统类别
    /// </summary>
    public class AppSort : EntityNoName
    {
        #region 属性
        /// <summary>
        /// RefMenuNo
        /// </summary>
        public string RefMenuNo
        {
            get
            {
                return this.GetValStrByKey(AppSortAttr.RefMenuNo);
            }
            set
            {
                this.SetValByKey(AppSortAttr.RefMenuNo, value);
            }
        }
        #endregion

        #region 按钮权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
        }
        #endregion
        #region 构造方法
        /// <summary>
        /// 系统类别
        /// </summary>
        public AppSort()
        {
        }
        /// <summary>
        /// 系统类别
        /// </summary>
        /// <param name="mypk"></param>
        public AppSort(string no)
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
                Map map = new Map("GPM_AppSort");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "系统类别";
                map.EnType = EnType.App;
                map.IsAutoGenerNo = true;


                map.AddTBStringPK(AppSortAttr.No, null, "编号", true, true, 2, 2, 20);
                map.AddTBString(AppSortAttr.Name, null, "名称", true, false, 0, 300, 20);
                map.AddTBInt(AppSortAttr.Idx, 0, "显示顺序", true, false);
                map.AddTBString(AppSortAttr.RefMenuNo, null, "关联的菜单编号", false, false, 0, 300, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            Apps pps = new Apps();
            pps.Retrieve(AppAttr.FK_AppSort, this.No);
            if (pps.Count != 0)
                throw new Exception("err@该类别下有系统，您不能删除，请把该系统类别下的系统移除或者删除，您才能删除该类别。");

            Menu root = new Menu();
            root.No = this.RefMenuNo;
            if (root.RetrieveFromDBSources() > 0)
                root.Delete();
            return base.beforeDelete();
        }

        protected override bool beforeUpdate()
        {
            Menu root = new Menu();
            root.No = this.RefMenuNo;
            if (root.RetrieveFromDBSources() > 0)
            {
                root.Name = this.Name;
                root.Update();
            }
            return base.beforeUpdate();
        }

        protected override bool beforeInsert()
        {
            base.beforeInsert();

            // 求root.
            Menu root = new Menu();
            root.No = "1000";
            if (root.RetrieveFromDBSources() == 0)
            {
                /*如果没有root.*/
                root.ParentNo = "0";
                root.Name = BP.Sys.SystemConfig.SysName;
                root.FK_App = BP.Sys.SystemConfig.SysNo;
                root.HisMenuType = MenuType.Root;
                root.Idx = 0;
                root.Insert();
            }

            // 创建系统类别做为二级菜单.
            Menu sort1 = root.DoCreateSubNode() as Menu;
            sort1.Name = this.Name;
            sort1.HisMenuType = MenuType.AppSort;
            sort1.FK_App = "AppSort";
            sort1.Update();

            this.RefMenuNo = sort1.No;
            return true;
        }
    }
    /// <summary>
    /// 系统类别s
    /// </summary>
    public class AppSorts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 系统类别s
        /// </summary>
        public AppSorts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new AppSort();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AppSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<AppSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AppSort> Tolist()
        {
            System.Collections.Generic.List<AppSort> list = new System.Collections.Generic.List<AppSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AppSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
