using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
    /// <summary>
    /// 数据批阅
    /// </summary>
    public class FrmDBRemarkAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 字段
        /// </summary>
        public const string Field = "Field";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 记录人姓名
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 轨迹ID
        /// </summary>
        public const string Remark = "Remark";
    }
    /// <summary>
    /// 数据批阅
    /// </summary>
    public class FrmDBRemark : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 权限控制
        /// </summary>
        public override  UAC HisUAC
        {
            get
            {
                var uac = new UAC();
                uac.Readonly();
                uac.IsView = false;
                return uac;
            }
        }
        public string Remark
        {
            get
            {
                return this.GetValStringByKey(FrmDBRemarkAttr.Remark);
            }
            set
            {
                this.SetValByKey(FrmDBRemarkAttr.Remark, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmDBRemarkAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmDBRemarkAttr.FrmID, value);
            }
        }
        /// <summary>
        /// FK_Emp
        /// </summary>
        public string RecNo
        {
            get
            {
                return this.GetValStringByKey(FrmDBRemarkAttr.RecNo);
            }
            set
            {
                this.SetValByKey(FrmDBRemarkAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmDBRemarkAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmDBRemarkAttr.RecName, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(FrmDBRemarkAttr.RDT);
            }
            set
            {
                this.SetValByKey(FrmDBRemarkAttr.RDT, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 数据批阅
        /// </summary>
        public FrmDBRemark()
        {
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
                Map map = new Map("Sys_FrmDBRemark", "数据批阅");
                map.AddMyPK();

                map.AddTBString(FrmDBRemarkAttr.FrmID, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(FrmDBRemarkAttr.RefPKVal, null, "PKVal", true, true, 0, 40, 20);
                map.AddTBString(FrmDBRemarkAttr.Field, null, "字段", true, true, 0, 60, 20);


                map.AddTBString(FrmDBRemarkAttr.Remark, null, "备注", true, true, 0, 500, 20);

                map.AddTBString(FrmDBRemarkAttr.RecNo, null, "记录人", true, true, 0, 50, 20);
                map.AddTBString(FrmDBRemarkAttr.RecName, null, "字段", true, true, 0, 60, 20);
                map.AddTBDateTime(FrmDBRemarkAttr.RDT, null, "记录时间", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            this.RDT = DataType.CurrentDateTime;
            if (DataType.IsNullOrEmpty(this.RecNo) == true)
            {
                this.RecNo = BP.Web.WebUser.No;
                this.RecName = BP.Web.WebUser.Name;
            }
            return base.beforeInsert();
        }

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new FrmDBRemarks(); }
        }
        #endregion 重写
    }
    /// <summary>
    /// 数据批阅s
    /// </summary>
    public class FrmDBRemarks : EntitiesMyPK
    {
        #region 构造
        public FrmDBRemarks()
        {
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmDBRemark();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmDBRemark> Tolist()
        {
            System.Collections.Generic.List<FrmDBRemark> list = new System.Collections.Generic.List<FrmDBRemark>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmDBRemark)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmDBRemark> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmDBRemark>)this;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。

    }
}
