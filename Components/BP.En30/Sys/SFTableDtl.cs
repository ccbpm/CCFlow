using System;
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
	public class SFTableDtlAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 关联的外键
        /// </summary>
        public const string FK_SFTable = "FK_SFTable";
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
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
    }

    /// <summary>
    /// 系统字典表
    /// </summary>
    public class SFTableDtl : EntityMyPK
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        //public string OrgNo
        //{
        //    get
        //    {
        //        return this.GetValStrByKey(SFTableDtlAttr.OrgNo);
        //    }
        //    set
        //    {
        //        this.SetValByKey(SFTableDtlAttr.OrgNo, value);
        //    }
        //}
        public string FK_SFTable
        {
            get
            {
                return this.GetValStrByKey(SFTableDtlAttr.FK_SFTable);
            }
            set
            {
                this.SetValByKey(SFTableDtlAttr.FK_SFTable, value);
            }
        }
        public string BH
        {
            get
            {
                return this.GetValStrByKey(SFTableDtlAttr.BH);
            }
            set
            {
                this.SetValByKey(SFTableDtlAttr.BH, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStrByKey(SFTableDtlAttr.Name);
            }
            set
            {
                this.SetValByKey(SFTableDtlAttr.Name, value);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(SFTableDtlAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(SFTableDtlAttr.ParentNo, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(SFTableDtlAttr.Idx);
            }
            set
            {
                this.SetValByKey(SFTableDtlAttr.Idx, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 访问权限
        /// </summary>
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
        public SFTableDtl()
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

                Map map = new Map("Sys_SFTableDtl", "系统字典表");

                //FK_SFTable+"_"+BH
                map.AddMyPK();

                map.AddTBString(SFTableDtlAttr.FK_SFTable, null, "外键表ID", true, false, 0, 200, 20);
                //map.AddTBString(SFTableDtlAttr.TableID, null, "TableID", true, false, 0, 200, 20);
                map.AddTBString(SFTableDtlAttr.BH, null, "BH", true, false, 0, 200, 20);
                map.AddTBString(SFTableDtlAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(SFTableDtlAttr.ParentNo, null, "父节点ID", true, false, 0, 200, 20);

                //用户注销组织的时候，方便删除数据.
                map.AddTBString(SFTableDtlAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 20);
                map.AddTBInt(SFTableDtlAttr.Idx, 0, "顺序号", false, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            //@hongyan 加标记的翻译.
            if (DataType.IsNullOrEmpty(this.GetValStringByKey("OrgNo")) == true)
                this.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);

            if (DataType.IsNullOrEmpty(this.GetValStringByKey("BH")) == true)
                this.SetValByKey("BH",  DBAccess.GenerGUID());

            this.setMyPK(this.GetValStringByKey("FK_SFTable") + "_" + this.GetValStringByKey("BH"));

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 系统字典表s
    /// </summary>
    public class SFTableDtls : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 系统字典表s
        /// </summary>
        public SFTableDtls()
        {
        }

        public SFTableDtls(string fk_sftable)
        {
            this.Retrieve(SFTableDtlAttr.FK_SFTable, fk_sftable);
        }


        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTableDtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTableDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTableDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTableDtl> Tolist()
        {
            System.Collections.Generic.List<SFTableDtl> list = new System.Collections.Generic.List<SFTableDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTableDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

