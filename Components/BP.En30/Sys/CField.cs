using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	/// <summary>
	/// 属性
	/// </summary>
	public class CFieldAttr
	{
		/// <summary>
		/// 属性Key
		/// </summary>
		public const string EnsName="EnsName";
		/// <summary>
		/// 工作人员
		/// </summary>
		public const string FK_Emp="FK_Emp";
		/// <summary>
		/// 列选择
		/// </summary>
		public const string Attrs="Attrs";
	}
	/// <summary>
	/// 列选择
	/// </summary>
	public class CField: Entity
	{
		#region 基本属性
		/// <summary>
		/// 列选择
		/// </summary>
		public string Attrs
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.Attrs ) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.Attrs,value) ; 
			}
		}
		/// <summary>
		/// 操作员ID
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.FK_Emp ) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.FK_Emp,value) ; 
			}
		}
		/// <summary>
		/// 属性
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.EnsName ) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.EnsName,value) ; 
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 列选择
		/// </summary>
		public CField()
		{
		}
		/// <summary>
		/// 列选择
		/// </summary>
		/// <param name="FK_Emp">工作人员ID</param>
		/// <param name="className">类名称</param>
		/// <param name="attrKey">属性</param>
		/// <param name="Attrs">值</param>
        public CField(string FK_Emp, string className)
        {
            int i = this.Retrieve(CFieldAttr.FK_Emp, FK_Emp,
                   CFieldAttr.EnsName, className);
            if (i == 0)
            {
                this.EnsName = className;
                this.FK_Emp = FK_Emp;
                this.Insert();
            }
        }
		/// <summary>
		/// map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) return this._enMap;
                Map map = new Map("Sys_CField");
                map.EnType = EnType.Sys;
                map.EnDesc = "列选择";
                map.DepositaryOfEntity = Depositary.None;
                map.AddTBStringPK(CFieldAttr.EnsName, null, "实体类名称", false, true, 1, 100, 10);
                map.AddTBStringPK(CFieldAttr.FK_Emp, Web.WebUser.No, "工作人员", false, true, 1, 100, 10);
                map.AddTBStringDoc(CFieldAttr.Attrs, null, "属性s", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 
	
        public static Attrs GetMyAttrs(Entities ens, Map map)
        {
            string vals = SystemConfig.GetConfigXmlEns("ListAttrs", ens.ToString());
            if (vals == null)
                return map.Attrs;
            Attrs attrs=new Attrs();
            foreach (Attr attr in map.Attrs)
            {
                if ( vals.Contains(","+attr.Key+",") )
                    attrs.Add(attr);
            }
            return attrs;

            //string no = Web.WebUser.No;
            //if (no == null)
            //    throw new Exception("@您的登陆时间太长。。。");

            //CField cf = new CField(no, ens.ToString());
            //if (cf.Attrs == "")
            //    return ens.GetNewEntity.EnMap.Attrs;

            //Attrs myattrs = new Attrs();
            //Attrs attrs = ens.GetNewEntity.EnMap.Attrs;
            //foreach (Attr attr in attrs)
            //{
            //    if (attr.IsPK)
            //    {
            //        myattrs.Add(attr);
            //        continue;
            //    }
            //    if (cf.Attrs.IndexOf("@" + attr.Key + "@") >= 0)
            //        myattrs.Add(attr);
            //}
            //return myattrs;
        }
	}
	/// <summary>
	/// 列选择s
	/// </summary>
	public class CFields : Entities
	{
		/// <summary>
		/// 列选择s
		/// </summary>
		public CFields()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CField();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CField> ToJavaList()
        {
            return (System.Collections.Generic.IList<CField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CField> Tolist()
        {
            System.Collections.Generic.List<CField> list = new System.Collections.Generic.List<CField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
