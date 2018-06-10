using System;
using System.Collections;
using BP.En;
using BP.DA;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class EntityNoAttr
	{
		/// <summary>
		/// 编号
		/// </summary>
		public const string No="No";
	}
    public class EntityNoMyFileAttr : EntityNoAttr
    {
        /// <summary>
        /// MyFileName
        /// </summary>
        public const string MyFileName = "MyFileName";
        /// <summary>
        /// MyFilePath
        /// </summary>
        public const string MyFilePath = "MyFilePath";
        /// <summary>
        /// MyFileExt
        /// </summary>
        public const string MyFileExt = "MyFileExt";
        public const string WebPath = "WebPath";
        public const string MyFileH = "MyFileH";
        public const string MyFileW = "MyFileW";
        public const string MyFileNum = "MyFileNum";
    }
	/// <summary>
	/// NoEntity 的摘要说明。
	/// </summary>
	abstract public class EntityNo: Entity
	{
		#region 提供的属性
        public override string PK
        {
            get
            {
                return "No";
            }
        }
		/// <summary>
		/// 编号
		/// </summary>
		public string No
		{
            get
            {
                return this.GetValStringByKey(EntityNoNameAttr.No);
            }
            set
            {
                this.SetValByKey(EntityNoNameAttr.No, value);
            }
		}
		#endregion

		#region 与编号有关的逻辑操作(这个属性只与dict EntityNo, 基类有关系。)
		/// <summary>
		/// Insert 之前的操作。
		/// </summary>
		/// <returns></returns>
        protected override bool beforeInsert()
        {

            Attr attr = this.EnMap.GetAttrByKey("No");
            if (attr.UIVisible == true && attr.UIIsReadonly && this.EnMap.IsAutoGenerNo && this.No.Length==0)
                this.No = this.GenerNewNo;

            return base.beforeInsert();
            ////if (this.EnMap.IsAutoGenerNo == true && (this.No == "" || this.No == null || this.No == "自动生成"))
            ////{
            ////    this.No = this.GenerNewNo;
            ////}
            //if (this.EnMap.IsAllowRepeatNo == false)
            //{
            //    string field = attr.Field;

            //    Paras ps = new Paras();
            //    ps.Add("no", No);
            //    string sql = "SELECT " + field + " FROM " + this.EnMap.PhysicsTable + " WHERE " + field + "=:no";
            //    if (DBAccess.IsExits(sql, ps))
            //        throw new Exception("@[" + this.EnMap.EnDesc + " , " + this.EnMap.PhysicsTable + "] 编号[" + No + "]重复。");
            //}

            //// 是不是检查编号的长度。
            //if (this.EnMap.IsCheckNoLength)
            //{
            //    if (this.No.Length!=this.EnMap.CodeLength )
            //        throw new Exception("@ ["+this.EnMap.EnDesc+"]编号["+this.No+"]错误，长度不符合系统要求，必须是["+this.EnMap.CodeLength.ToString()+"]位，而现在有长度是["+this.No.Length.ToString()+"]位。");
            //}
            //return base.beforeInsert();
        }
		#endregion 
		 
		#region 构造涵数
		/// <summary>
		/// 事例化一个实体
		/// </summary>
		public EntityNo()
		{
		}
		/// <summary>
		/// 通过编号得到实体。
		/// </summary>
		/// <param name="_no">编号</param>
		public EntityNo(string _no)  
		{
			if (_no==null || _no=="")
				throw new Exception( this.EnDesc+"@对表["+this.EnDesc+"]进行查询前必须指定编号。");

			this.No = _no ;
			if (this.Retrieve()==0) 
			{				
				throw new Exception("@没有"+this._enMap.PhysicsTable+", No = "+No+"的记录。");
			}
		}
        public override int Save()
        {
            /*如果包含编号。 */
            if (this.IsExits)
            {
                return this.Update();
            }
            else
            {
                if (this.EnMap.IsAutoGenerNo
                    && this.EnMap.GetAttrByKey("No").UIIsReadonly)
                    this.No = this.GenerNewNo;

                this.Insert();
                return 0;
            }

           // return base.Save();
        }
		#endregion		

		#region 提供的查寻方法
		/// <summary>
		/// 生成一个编号
		/// </summary>
		public string GenerNewNo
		{
            get
            {
                return this.GenerNewNoByKey("No");
            }
		}
        /// <summary>
        /// 生成编号
        /// </summary>
        /// <returns></returns>
        public string GenerNewEntityNo()
        {
            return this.GenerNewNoByKey("No");
        }
		/// <summary>
		/// 按 No 查询。
		/// </summary>
		/// <returns></returns>
        public int RetrieveByNo()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EntityNoAttr.No, this.No);
            return qo.DoQuery();
        }
		/// <summary>
		/// 按 No 查询。
		/// </summary>
		/// <param name="_No">No</param>
		/// <returns></returns>
		public int RetrieveByNo(string _No) 
		{
			this.No = _No ;
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EntityNoAttr.No,this.No);
			return qo.DoQuery();
		}
		#endregion
	}
	/// <summary>
	/// 编号实体集合。
	/// </summary>
	abstract public class EntitiesNo : Entities
	{
        public override int RetrieveAllFromDBSource()
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy("No");
            return qo.DoQuery();
        }
	}
}
