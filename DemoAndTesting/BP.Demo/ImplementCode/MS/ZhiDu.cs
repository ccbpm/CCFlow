using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.MS
{
	/// <summary>
	/// 制度属性
	/// </summary>
    public class ZhiDuAttr:EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 类别
        /// </summary>
        public const string FK_Sort = "FK_Sort";
        /// <summary>
        /// 制度内容
        /// </summary>
        public const string MainDoc = "MainDoc";
        /// <summary>
        /// 制度控制方式
        /// </summary>
        public const string RelDept = "RelDept";
        /// <summary>
        /// 制度对象
        /// </summary>
        public const string MainSQL = "MainSQL";
        /// <summary>
        /// 原存储路径
        /// </summary>
        public const string WebPath = "WebPath";
        /// <summary>
        /// 制度编号
        /// </summary>
        public const string ZDNo = "ZDNo";
        /// <summary>
        /// 制度版本号
        /// </summary>
        public const string ZDVersion = "ZDVersion";
        /// <summary>
        /// 文件属性
        /// </summary>
        public const string ZDProperty = "ZDProperty";
        /// <summary>
        /// 文件作废
        /// </summary>
        public const string IsDelete = "IsDelete";
       /// <summary>
       /// 外部系统编号
       /// </summary>
        public const string ExternalNo = "ExternalNo";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string OID = "OID";
        #endregion
    }
	/// <summary>
	/// 制度
	/// </summary>
    public class ZhiDu : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 存储路径
        /// </summary>
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.WebPath);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.WebPath, value);
            }
        }
        public string FK_Sort
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.FK_Sort);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.FK_Sort, value);
            }
        }
        /// <summary>
        /// 发布单位
        /// </summary>
        public string RelDept
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.RelDept);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.RelDept, value);
            }
        }
        /// <summary>
        /// 制度版本号
        /// </summary>
        public string ZDVersion
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.ZDVersion);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.ZDVersion, value);
            
            }
        }
        /// <summary>
        /// 文件属性
        /// </summary>
        public string ZDProperty
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.ZDProperty);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.ZDProperty, value);
            }
        }
        /// <summary>
        /// 制度编号
        /// </summary>
        public string ZDNo
        {
            get
            {
                return this.GetValStringByKey(ZhiDuAttr.ZDNo);
            }
            set
            {
                this.SetValByKey(ZhiDuAttr.ZDNo, value);
            }
        }
        /// <summary>
        /// 判断子表是否包含记录
        /// </summary>
        public bool HisHaveDtl
        {
            get
            {
                ZhiDuDtl dtl = new ZhiDuDtl();
                return dtl.RetrieveByAttr("FK_Main", this.No);
            }
        }
        //外部平台编号
        public string ExternalNo
        {
            get { return this.GetValStringByKey(ZhiDuAttr.ExternalNo); }
            set { this.SetValByKey(ZhiDuAttr.ExternalNo, value); }
        }

        //流程编号
        public string  OID
        {
            get { return this.GetValStringByKey(ZhiDuAttr.OID); }
            set { this.SetValByKey(ZhiDuAttr.OID, value); }
        }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete
        {
            get { return this.GetValBooleanByKey(ZhiDuAttr.IsDelete); }
            set { this.SetValByKey(ZhiDuAttr.IsDelete, value); }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }

      
        #endregion

        #region 构造函数
        /// <summary>
        /// Main
        /// </summary>
        public ZhiDu()
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
                Map map = new Map("MS_ZhiDu", "制度");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBStringPK(ZhiDuAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(ZhiDuAttr.Name, null, "名称", true, true, 0, 200, 5);
                map.AddTBString(ZhiDuAttr.FK_Sort, null, "类别", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.WebPath, null, "路径", true, true, 0, 400, 4);
                map.AddTBString(ZhiDuAttr.RelDept, null, "发布单位", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.ZDNo, null, "制度编号", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.ZDVersion, null, "制度版本号", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.ZDProperty, null, "文件属性", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.ExternalNo, null, "外部系统编号", true, true, 0, 200, 4);
                map.AddTBString(ZhiDuAttr.OID, null, "流程编号", true, true, 0, 200, 4);
                map.AddBoolean(ZhiDuAttr.IsDelete,false,"是否作废",true,false);


                // map.AddTBString(MainAttr.FK_Sort, null, "制度类别", true, false, 0, 500, 10,true);
                // map.AddTBStringDoc(MainAttr.MainDoc, null, "制度内容(类别与内容支持变量)", true, false,true);
                // map.AddSearchAttr(MainAttr.RelDept);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 制度s
	/// </summary>
	public class ZhiDus: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ZhiDu();
			}
		}
		/// <summary>
        /// 制度
		/// </summary>
        public ZhiDus() { } 		 
		#endregion
	}
}
