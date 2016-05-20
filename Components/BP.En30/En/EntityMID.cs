using System;
using System.Collections;
using BP.DA;

namespace BP.En
{

	/// <summary>
	/// 属性列表
	/// </summary>
	public class EntityMIDAttr
	{
		public static string MID="MID";
	}
	/// <summary>
	/// MID实体,只有一个实体这个实体只有一个主键属性。
	/// </summary>
	abstract public class EntityMID : Entity
	{

		public override int Save()
		{
			
			if ( this.Update()==0 )
			{
				this.MID=BP.DA.DBAccess.GenerOID();
				this.Insert();
				this.Retrieve();
			}
			return this.MID;
		}


//		public override int Save()
//		{
//
//			if (this.IsExits)
//				return this.Update();
//			else
//			{
//				this.Insert();
//				return 1;
//			}
//
//			//			if (this.Update()==0)
//			//				this.Insert();
//			//
//			//			return base.Save ();
//		}


		/// <summary>
		/// 是否存在
		/// </summary>
		/// <returns></returns>
		public bool IsExitCheckByPKs()
		{
			return false;
		}


		#region 属性
		/// <summary>
		/// MID, 如果是空的就返回 0 . 
		/// </summary>
		public int MID
		{
			get
			{
				try
				{
					return this.GetValIntByKey(EntityMIDAttr.MID);
				}
				catch
				{
					return 0; 
				}
			}

			set
			{
				this.SetValByKey(EntityMIDAttr.MID,value);
			}
		}
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造一个空实例
		/// </summary>
		protected EntityMID()
		{
		}
		/// <summary>
		/// 根据MID构造实体
		/// </summary>
		/// <param name="MID">MID</param>
		protected EntityMID(int mid)  
		{
			this.SetValByKey(EntityMIDAttr.MID,MID);
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EntityMIDAttr.MID,mid);
			if (qo.DoQuery()==0)
				throw new Exception("没有查询到MID="+mid+"的实例。");
			//this.Retrieve();
		}
		#endregion
	 
		#region override 方法
	
		public override int Retrieve()
		{
			if (this.MID==0)
			{
				return base.Retrieve();
			}
			else
			{
				QueryObject qo = new QueryObject(this);
				qo.AddWhere("MID", this.MID);
				if (qo.DoQuery()==0)
					throw new Exception("没有此记录:MID="+this.MID);
				else
					return 1;
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
				if (this.MID < 0 )
					throw new Exception("@实体["+this.EnDesc+"]没有被实例化，不能Delete().");
				return true;
				 
			} 
			catch (Exception ex) 
			{
				throw new Exception("@["+this.EnDesc+"].beforeDelete err:"+ex.Message);
			}
		}
		public override int DirectInsert()
		{
			this.MID=DBAccess.GenerOID();
			//EnDA.Insert(this);
			return this.RunSQL( SqlBuilder.Insert(this) );

		}
		#endregion

		#region public 方法
		 
		#endregion

	}
	/// <summary>
	/// MID实体集合
	/// </summary>
	abstract public class EntitiesMID : Entities
	{
		public EntitiesMID()
		{
		}		 
	}
}
