using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.MS
{
    /// <summary>
    /// 制度编号 部门
    /// </summary>
    public class ZhiDuDeptAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 制度编号
        /// </summary>
        public const string ZDNo = "ZDNo";
        /// <summary>
        /// 编号最大值
        /// </summary>
        public const string ZDMax = "ZDMax";

        #endregion
    }
    /// <summary>
    /// 制度
    /// </summary>
    public class ZhiDuDept : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(ZhiDuDeptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(ZhiDuDeptAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 制度编号
        /// </summary>
        public string ZDNo
        {
            get
            {
                return this.GetValStringByKey(ZhiDuDeptAttr.ZDNo);
            }
            set
            {
                this.SetValByKey(ZhiDuDeptAttr.ZDNo, value);
            }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public int ZDMax
        {
            get
            {
                return this.GetValIntByKey(ZhiDuDeptAttr.ZDMax);
            }
            set
            {
                this.SetValByKey(ZhiDuDeptAttr.ZDMax, value);
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// Main
        /// </summary>
        public ZhiDuDept()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("MS_ZhiDuDept", "制度");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBStringPK(ZhiDuDeptAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(ZhiDuDeptAttr.Name, null, "名称", true, true, 0, 200, 5);
                map.AddTBString(ZhiDuDeptAttr.FK_Dept, null, "部门", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuDeptAttr.ZDMax, null, "最大值", true, true, 0, 400, 4);
                map.AddTBString(ZhiDuDeptAttr.ZDNo, null, "制度编号", true, true, 0, 200, 4);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 制度s
    /// </summary>
    public class ZhiDuDepts : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ZhiDuDept();
            }
        }
        /// <summary>
        /// 制度
        /// </summary>
        public ZhiDuDepts() { }
        #endregion
    }
}
