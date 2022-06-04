using System;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 权限中心
    /// </summary>
    public class PowerCenterAttr : EntityTreeAttr
    {
        /// <summary>
        /// 控制对象
        /// </summary>
        public const string CtrlObj = "CtrlObj";
        /// <summary>
        /// 分组
        /// </summary>
        public const string CtrlGroup = "CtrlGroup";
        /// <summary>
        /// 控制模式
        /// </summary>
        public const string CtrlModel = "CtrlModel";
        /// <summary>
        /// IDs
        /// </summary>
        public const string IDs = "IDs";
        /// <summary>
        /// 名称
        /// </summary>
        public const string IDNames = "IDNames";
        /// <summary>
        /// 控制对象Val
        /// </summary>
        public const string CtrlPKVal = "CtrlPKVal";
        /// <summary>
        /// 编号
        /// </summary>
        public const string OrgNo = "OrgNo";

        public const string Idx = "Idx";

    }
    /// <summary>
    /// 权限中心
    /// </summary>
    public class PowerCenter : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 控制主键
        /// </summary>
        public string CtrlPKVal
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.CtrlPKVal);
            }
        }
        public string IDNames
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.IDNames);
            }
        }
        public string IDs
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.IDs);
            }
        }
        public string CtrlModel
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.CtrlModel);
            }
        }
        public string CtrlGroup
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.CtrlGroup);
            }
        }
        public string CtrlObj
        {
            get
            {
                return this.GetValStrByKey(PowerCenterAttr.CtrlObj);
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
        /// 权限中心
        /// </summary>
        public PowerCenter()
        {
        }
        /// <summary>
        /// 权限中心
        /// </summary>
        /// <param name="mypk"></param>
        public PowerCenter(string mypk)
        {
            this.setMyPK(mypk);
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

                Map map = new Map("GPM_PowerCenter", "权限中心");

                map.AddMyPK();

                // System,Module,Menus
                map.AddTBString(PowerCenterAttr.CtrlObj, null, "控制对象(SystemMenus)", true, false, 0, 300, 20);
                map.AddTBString(PowerCenterAttr.CtrlPKVal, null, "控制对象ID", true, false, 0, 300, 20);
                //Menus, Frm 
                map.AddTBString(PowerCenterAttr.CtrlGroup, null, "隶属分组(可为空)", true, false, 0, 300, 20);

                //AnyOne,Adminer,Depts
                map.AddTBString(PowerCenterAttr.CtrlModel, null, "控制模式", true, false, 0, 300, 20);

                map.AddTBStringDoc(PowerCenterAttr.IDs, null, "主键s(Stas,Depts等)", true, false);
                map.AddTBStringDoc(PowerCenterAttr.IDNames, null, "IDNames", true, false);

                map.AddTBString(PowerCenterAttr.OrgNo, null, "编号", true, false, 0, 100, 20);
                map.AddTBString(PowerCenterAttr.Idx, null, "Idx", true, false, 0, 100, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (BP.Web.WebUser.IsAdmin == false)
                throw new Exception("err@非管理员不能操作...");

            return base.beforeUpdateInsertAction();
        }

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 权限中心s
    /// </summary>
    public class PowerCenters : EntitiesMyPK
    {
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.SAAS)
                return base.RetrieveAll();

            //集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll();

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo);
        }

        #region 构造
        /// <summary>
        /// 权限中心s
        /// </summary>
        public PowerCenters()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PowerCenter();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<PowerCenter> ToJavaList()
        {
            return (System.Collections.Generic.IList<PowerCenter>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<PowerCenter> Tolist()
        {
            System.Collections.Generic.List<PowerCenter> list = new System.Collections.Generic.List<PowerCenter>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((PowerCenter)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
