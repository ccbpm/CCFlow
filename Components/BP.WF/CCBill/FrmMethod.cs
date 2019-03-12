﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.CCBill
{
    
	/// <summary>
	/// 表单方法属性
	/// </summary>
    public class FrmMethodAttr:EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string MethodID = "MethodID";
        /// <summary>
        /// 方法名
        /// </summary>
        public const string MethodName = "MethodName";
        /// <summary>
        /// 方法类型
        /// </summary>
        public const string RefMethodType = "RefMethodType";
        /// <summary>
        /// 处理内容
        /// </summary>
        public const string MethodDoc = "MethodDoc";
        /// <summary>
        /// 方法的内容类型
        /// </summary>
        public const string MethodDocTypeOfFunc = "MethodDocTypeOfFunc";
        /// <summary>
        /// 处理内容 tag.
        /// </summary>
        public const string Idx = "Idx";
    }
	/// <summary>
	/// 表单方法
	/// </summary>
    public class FrmMethod : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodID, value);
            }
        }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.MethodName);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodName, value);
            }
        }
        public string MethodDoc
        {
            get
            {
                string s = this.GetValStringByKey(FrmMethodAttr.MethodDoc);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodDoc, value);
            }
        }
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(FrmMethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.RefMethodType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单方法
        /// </summary>
        public FrmMethod()
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

                Map map = new Map("WF_FrmMethod", "表单方法");

                map.AddMyPK();

                map.AddTBString(FrmMethodAttr.FrmID, null, "表单ID", true, false, 0, 300, 10);
                map.AddTBString(FrmMethodAttr.MethodID, null, "方法ID", true, false, 0, 300, 10);
                map.AddTBString(FrmMethodAttr.MethodName, null, "方法名", true, false, 0, 300, 10);

                map.AddDDLSysEnum(FrmMethodAttr.RefMethodType, 0, "方法类型", true, false, FrmMethodAttr.RefMethodType,
                    "@0=功能@1=模态窗口打开@2=新窗口打开@3=右侧窗口打开@4=实体集合的功能");

                map.AddTBStringDoc(FrmMethodAttr.MethodDoc, null, "方法内容", true, false);
                map.AddTBInt(FrmMethodAttr.Idx, 0, "Idx", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public void DoUp()
        {
            this.DoOrderUp(FrmMethodAttr.FrmID, this.FrmID, FrmMethodAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(FrmMethodAttr.FrmID, this.FrmID, FrmMethodAttr.Idx);
        }

       
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
         
    }
	/// <summary>
	/// 表单方法
	/// </summary>
    public class FrmMethods : EntitiesMyPK
    {
        /// <summary>
        /// 表单方法
        /// </summary>
        public FrmMethods() { }
        /// <summary>
        /// 表单方法
        /// </summary>
        /// <param name="FrmID"></param>
        public FrmMethods(string FrmID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(FrmMethodAttr.MethodID, "SELECT NodeID FROM WF_Node WHERE FrmID='" + FrmID + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 表单方法
        /// </summary>
        /// <param name="nodeid">方法IDID</param>
        public FrmMethods(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmMethodAttr.MethodID, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmMethod();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmMethod> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmMethod>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmMethod> Tolist()
        {
            System.Collections.Generic.List<FrmMethod> list = new System.Collections.Generic.List<FrmMethod>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmMethod)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
