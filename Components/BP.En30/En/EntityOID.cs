using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 属性列表
    /// </summary>
    public class EntityOIDAttr
    {
        /// <summary>
        /// OID
        /// </summary>
        public const string OID = "OID";
    }
	/// <summary>
	/// 属性列表
	/// </summary>
    public class EntityOIDMyFileAttr : EntityOIDAttr
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
    }
	/// <summary>
	/// OID实体,只有一个实体这个实体只有一个主键属性。
	/// </summary>
	abstract public class EntityOID : Entity
	{		 
		#region 属性
        /// <summary>
        /// 是否是自动增长列
        /// </summary>
        public virtual bool IsInnKey
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "OID";
            }
        }
		/// <summary>
		/// OID, 如果是空的就返回 0 . 
		/// </summary>
        public int OID
        {
            get
            {
                try
                {
                    return this.GetValIntByKey(EntityOIDAttr.OID);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                this.SetValByKey(EntityOIDAttr.OID, value);
            }
        }
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造一个空实例
		/// </summary>
		protected EntityOID()
		{
		}
		/// <summary>
		/// 根据OID构造实体
		/// </summary>
		/// <param name="oid">oid</param>
		protected EntityOID(int oid)  
		{
			this.SetValByKey(EntityOIDAttr.OID,oid);
			this.Retrieve();
		}
		#endregion
	 
		#region override 方法
		public override int DirectInsert()
		{
            this.OID =DBAccess.GenerOID();
			return base.DirectInsert ();
		}
		public void InsertAsNew()
		{
			this.OID=0;
			this.Insert();
		}
        public override bool IsExits
        {
            get
            {
                if (this.OID == 0)
                    return false;

                try
                {
                    // 生成数据库判断语句。
                    string selectSQL = "SELECT " + this.PKField + " FROM " + this.EnMap.PhysicsTable + " WHERE OID=" + this.HisDBVarStr + "v";
                    Paras ens = new Paras();
                    ens.Add("v", this.OID);

                    // 从数据库里面查询，判断有没有。
                    switch (this.EnMap.EnDBUrl.DBUrlType)
                    {
                        case DBUrlType.AppCenterDSN:
                            return DBAccess.IsExits(selectSQL, ens);
                        default:
                            throw new Exception("没有设计到。" + this.EnMap.EnDBUrl.DBType);
                    }
                }
                catch(Exception ex)
                {
                    this.CheckPhysicsTable();
                    throw ex;
                }

                /* DEL BY PENG 2008-04-27
				// 生成数据库判断语句。
				string selectSQL="SELECT "+this.PKField + " FROM "+ this.EnMap.PhysicsTable + " WHERE " ;
				switch(this.EnMap.EnDBUrl.DBType )
				{
					case DBType.MSSQL:
						selectSQL +=SqlBuilder.GetKeyConditionOfMS(this);
						break;
					case DBType.Access:
						selectSQL +=SqlBuilder.GetKeyConditionOfOLE(this);
						break;
					case DBType.Oracle:
						selectSQL +=SqlBuilder.GetKeyConditionOfOracle(this);
						break; 
					default:
						throw new Exception("没有设计到。"+this.EnMap.EnDBUrl.DBType);
				}

				// 从数据库里面查询，判断有没有。
				switch(this.EnMap.EnDBUrl.DBUrlType )
				{
					case DBUrlType.AppCenterDSN:
						return DBAccess.IsExits( selectSQL) ;
					case DBUrlType.DBAccessOfMSSQL:
						return DBAccessOfMSSQL.IsExits( selectSQL) ;
					case DBUrlType.DBAccessOfOLE:
						return DBAccessOfOLE.IsExits( selectSQL) ;
					case DBUrlType.DBAccessOfOracle:
						return DBAccessOfOracle.IsExits( selectSQL) ;
					default:
						throw new Exception("没有设计到。"+this.EnMap.EnDBUrl.DBType);				
				}
                */
            }
        }
		/// <summary>
		/// 删除之前的操作。
		/// </summary>
		/// <returns></returns>
		protected override bool beforeDelete() 
		{
			if (base.beforeDelete()==false)
				return false;			
			try 
			{				
				if (this.OID < 0 )
					throw new Exception("@实体["+this.EnDesc+"]没有被实例化，不能Delete().");
				return true;
			} 
			catch (Exception ex) 
			{
				throw new Exception("@["+this.EnDesc+"].beforeDelete err:"+ex.Message);
			}
		}
		protected override bool beforeUpdateInsertAction()
		{
			return base.beforeUpdateInsertAction ();
		}

		/// <summary>
		/// beforeInsert 之前的操作。
		/// </summary>
		/// <returns></returns>
        protected override bool beforeInsert()
        {
            if (this.OID == -999)
                return base.beforeInsert();

            if (this.OID > 0)
                throw new Exception("@[" + this.EnDesc + "], 实体已经被实例化 oid=[" + this.OID + "]，不能Insert.");

            if (this.IsInnKey)
                this.OID = -1;
            else
                this.OID = BP.DA.DBAccess.GenerOID();

            return base.beforeInsert();
         
        }
		/// <summary>
		/// beforeUpdate
		/// </summary>
		/// <returns></returns>
		protected override bool beforeUpdate()
		{
			if (base.beforeUpdate()==false)
				return false;

			/*
			if (this.OID <= 0 )
				throw new Exception("@实体["+this.EnDesc+"]没有被实例化，不能Update().");
				*/
			return true;
		}
        protected virtual string SerialKey
        {
            get
            {
                return "OID";
            }
        }
       
		#endregion

		#region public 方法
		/// <summary>
		/// 作为一个新的实体保存。
		/// </summary>
        public void SaveAsNew()
        {
            try
            {
                this.OID = DBAccess.GenerOIDByKey32(this.SerialKey);
                this.RunSQL(SqlBuilder.Insert(this));
            }
            catch (System.Exception ex)
            {
                this.CheckPhysicsTable();
                throw ex;
            }
        }
		/// <summary>
		/// 按照指定的OID Insert.
		/// </summary>
        public void InsertAsOID(int oid)
        {
            this.SetValByKey("OID", oid);
            try
            {
                this.RunSQL(SqlBuilder.Insert(this));
            }
            catch (Exception ex)
            {
                this.CheckPhysicsTable();
                throw ex;
            }
        }
        public void InsertAsOID(Int64 oid)
        {
            try
            {
                //先设置一个标记值，为的是不让其在[beforeInsert]产生oid.
                this.SetValByKey("OID", -999);

                //调用方法.
                this.beforeInsert();

                //设置主键.
                this.SetValByKey("OID", oid);

                this.RunSQL(SqlBuilder.Insert(this));

                this.afterInsert();
            }
            catch (Exception ex)
            {
                this.CheckPhysicsTable();
                throw ex;
            }
        }
		/// <summary>
		/// 按照指定的OID 保存
		/// </summary>
		/// <param name="oid"></param>
		public void SaveAsOID(int oid)
		{
			this.SetValByKey("OID",oid);
			if (this.IsExits==false)
				this.InsertAsOID(oid);
			this.Update();
		}
		#endregion
	}
	abstract public class EntitiesOID : Entities
	{
        /// <summary>
        /// 构造
        /// </summary>
		public EntitiesOID()
		{

		}
		 
		#region 查询方法, 专用于与语言有关的实体
		/// <summary>
		/// 查询出来, 所有中文的实例 . 
		/// </summary>
		public void RetrieveAllCNEntities()
		{
			this.RetrieveByLanguageNo("CH") ; 
		}
		/// <summary>
		/// 按语言查询。 
		/// </summary>
		public void RetrieveByLanguageNo(string LanguageNo )
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere( "LanguageNo", LanguageNo );
			qo.DoQuery();
		}
		#endregion
	}
}
