using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
	/// <summary>
	/// 属性
	/// </summary>
    public class SerialAttr
    {
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 工作人员
        /// </summary>
        public const string CfgKey = "CfgKey";
        /// <summary>
        /// 序列号
        /// </summary>
        public const string IntVal = "IntVal";
    }
	/// <summary>
	/// 序列号
	/// </summary>
	public class Serial: Entity
	{
		#region 基本属性
		/// <summary>
		/// 序列号
		/// </summary>
		public string IntVal
		{
			get
			{
				return this.GetValStringByKey(SerialAttr.IntVal ) ; 
			}
			set
			{
				this.SetValByKey(SerialAttr.IntVal,value) ; 
			}
		}
		/// <summary>
		/// 操作员ID
		/// </summary>
		public string CfgKey
		{
			get
			{
				return this.GetValStringByKey(SerialAttr.CfgKey ) ; 
			}
			set
			{
				this.SetValByKey(SerialAttr.CfgKey,value) ; 
			}
		}
		#endregion

		#region 构造方法

		/// <summary>
		/// 序列号
		/// </summary>
		public Serial()
		{
		}
		/// <summary>
		/// map
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) return this._enMap;
				Map map = new Map("Sys_Serial");
				map.EnType=EnType.Sys;
				map.EnDesc="序列号";
				map.DepositaryOfEntity=Depositary.None;
				map.AddTBStringPK(SerialAttr.CfgKey,"OID","CfgKey",false,true,1,100,10);
				map.AddTBInt(SerialAttr.IntVal,0,"属性",true,false);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion 

        public int Gener(string CfgKey)
        {
            Paras ps = new Paras();
            ps.Add("p", CfgKey);

            string sql = "SELECT IntVal Sys_Serial WHERE CfgKey="+SystemConfig.AppCenterDBVarStr+"p";
            int val = DBAccess.RunSQLReturnValInt(sql, 0,ps);
            if (val == 0)
            {
                sql = "INSERT INTO Sys_Serial VALUES(" + SystemConfig.AppCenterDBVarStr + "p,1)";
                DBAccess.RunSQLReturnVal(sql, ps);
                return 1;
            }
            else
            {
                val++;
                ps.Add("intV", val);
                sql = "UPDATE  Sys_Serial SET IntVal="+SystemConfig.AppCenterDBVarStr+"intV WHERE  CfgKey=" + SystemConfig.AppCenterDBVarStr + "p";
                DBAccess.RunSQLReturnVal(sql);
                return val;
            }
        }
	}
	/// <summary>
	/// 序列号s
	/// </summary>
	public class Serials : Entities
	{
		/// <summary>
		/// 序列号s
		/// </summary>
		public Serials()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Serial();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Serial> ToJavaList()
        {
            return (System.Collections.Generic.IList<Serial>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Serial> Tolist()
        {
            System.Collections.Generic.List<Serial> list = new System.Collections.Generic.List<Serial>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Serial)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
