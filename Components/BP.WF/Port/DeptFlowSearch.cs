using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.WF.Port
{
    /// <summary>
    /// 流程部门数据查询权限
    /// </summary>
    public class DeptFlowSearchAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        #endregion
    }
    /// <summary>
    /// 流程部门数据查询权限 的摘要说明。
    /// </summary>
    public class DeptFlowSearch : EntityMyPK
    {
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsView = true;
                    uac.IsDelete = true;
                    uac.IsInsert = true;
                    uac.IsUpdate = true;
                    uac.IsAdjunct = true;
                }
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DeptFlowSearchAttr.FK_Emp);
            }
            set
            {
                SetValByKey(DeptFlowSearchAttr.FK_Emp, value);
            }
        }
        /// <summary>
        ///部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptFlowSearchAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptFlowSearchAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(DeptFlowSearchAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(DeptFlowSearchAttr.FK_Flow, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 流程部门数据查询权限
        /// </summary> 
        public DeptFlowSearch() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_DeptFlowSearch", "流程部门数据查询权限");
                map.AddMyPK();
                map.AddTBString(DeptFlowSearchAttr.FK_Emp, null, "操作员", true, true, 1, 50, 11);
                map.AddTBString(DeptFlowSearchAttr.FK_Flow, null, "流程编号", true, true, 1, 50, 11);
                map.AddTBString(DeptFlowSearchAttr.FK_Dept, null, "部门编号", true, true, 1, 100, 11);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        
    }
    /// <summary>
    /// 流程部门数据查询权限 
    /// </summary>
    public class DeptFlowSearchs : Entities
    {
        #region 构造
        /// <summary>
        /// 流程部门数据查询权限
        /// </summary>
        public DeptFlowSearchs() { }
        /// <summary>
        /// 流程部门数据查询权限
        /// </summary>
        /// <param name="FK_Emp">FK_Emp</param>
        public DeptFlowSearchs(string FK_Emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptFlowSearchAttr.FK_Emp, FK_Emp);
            qo.DoQuery();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptFlowSearch();
            }
        }
        #endregion

        #region 查询方法
        #endregion
    }
}
