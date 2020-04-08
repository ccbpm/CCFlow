﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using BP.Sys;

namespace BP.Sys
{
    /// <summary>
	/// 属性
	/// </summary>
	public class DictDtlAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 关联的外键
        /// </summary>
        public const string DictMyPK = "DictMyPK";
        public const string BH = "BH";
        public const string Name = "Name";
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string TableID = "TableID";
        /// <summary>
        /// 列选择
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 系统字典表
    /// </summary>
    public class DictDtl : EntityMyPK
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DictDtlAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DictDtlAttr.OrgNo, value);
            }
        }
        public string DictMyPK
        {
            get
            {
                return this.GetValStrByKey(DictDtlAttr.DictMyPK);
            }
            set
            {
                this.SetValByKey(DictDtlAttr.DictMyPK, value);
            }
        }
        public string BH
        {
            get
            {
                return this.GetValStrByKey(DictDtlAttr.BH);
            }
            set
            {
                this.SetValByKey(DictDtlAttr.BH, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStrByKey(DictDtlAttr.Name);
            }
            set
            {
                this.SetValByKey(DictDtlAttr.Name, value);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DictDtlAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DictDtlAttr.ParentNo, value);
            }
        }
        #endregion 属性.


        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 系统字典表
        /// </summary>
        public DictDtl()
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

                Map map = new Map("Sys_DictDtl", "系统字典表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                //DictMyPK+"_"+BH
                map.AddMyPK();

                map.AddTBString(DictDtlAttr.DictMyPK, null, "外键表ID", true, false, 0, 200, 20);
                //map.AddTBString(DictDtlAttr.TableID, null, "TableID", true, false, 0, 200, 20);

                map.AddTBString(DictDtlAttr.BH, null, "BH", true, false, 0, 200, 20);
                map.AddTBString(DictDtlAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(DictDtlAttr.ParentNo, null, "父节点ID", true, false, 0, 200, 20);

                //用户注销组织的时候，方便删除数据.
                map.AddTBString(DictDtlAttr.OrgNo, null, "OrgNo", true, false, 0, 200, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 更新的操作
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }

        protected override void afterInsertUpdateAction()
        {
            base.afterInsertUpdateAction();
        }
      
        protected override bool beforeInsert()
        {
            if (BP.Sys.SystemConfig.CCBPMRunModel != 0)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 系统字典表s
    /// </summary>
    public class DictDtls : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 系统字典表s
        /// </summary>
        public DictDtls()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DictDtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DictDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<DictDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DictDtl> Tolist()
        {
            System.Collections.Generic.List<DictDtl> list = new System.Collections.Generic.List<DictDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DictDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}