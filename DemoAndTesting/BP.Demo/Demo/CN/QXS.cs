using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
	/// <summary>
	/// 区县市
	/// </summary>
    public class QXSAttr : EntityNoNameAttr
    {
        #region 基本属性
        public const string FK_PQ = "FK_PQ";
        public const string FK_SF = "FK_SF";
        public const string NameS = "NameS";
        #endregion
    }
	/// <summary>
    /// 区县市
	/// </summary>
	public class QXS :EntityNoName
	{	
		#region 基本属性
        public string NameS
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.NameS);
            }
        }
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.FK_PQ);
            }
        }
        public string FK_SF
        {
            get
            {
                return this.GetValStrByKey(QXSAttr.FK_SF);
            }
        }
		#endregion 

		#region 构造函数
		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				uac.OpenForSysAdmin();
				return uac;
			}
		}
		/// <summary>
		/// 区县市
		/// </summary>		
		public QXS(){}
		public QXS(string no):base(no)
		{
		}
		/// <summary>
		/// Map
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map();

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.PhysicsTable = "CN_QXS";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.EnDesc = "区县市";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(QXSAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(QXSAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(QXSAttr.NameS, null, "NameS", true, false, 0, 50, 200);


                map.AddDDLEntities(QXSAttr.FK_SF, null, "省份", new SFs(), true);
                map.AddDDLEntities(QXSAttr.FK_PQ, null, "片区", new PQs(), true);

                map.AddSearchAttr(QXSAttr.FK_SF);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion

        /// <summary>
        /// 获取一个字符串中是否包含区县名称，如果包含就返回它的编号，不包含就返回默认的值。
        /// </summary>
        /// <param name="name">字串</param>
        /// <param name="defVal">默认值</param>
        /// <returns>区县代码</returns>
        public static string GenerQXSNoByName(string name, string defVal)
        {
            //进行模糊匹配地区。
            QXSs qxss = new QXSs();
            qxss.RetrieveAll();

            foreach (QXS qxs in qxss)
            {
                if (name.Contains(qxs.NameS))
                    return qxs.No;
            }

            SFs sfs = new SFs();
            sfs.RetrieveAll();
            foreach (SF sf in sfs)
            {
                if (name.Contains(sf.Names))
                    return sf.No;
            }

            return defVal;
        }
	}
	/// <summary>
	/// 区县市
	/// </summary>
	public class QXSs : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new QXS();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 区县市s
		/// </summary>
		public QXSs(){}

        /// <summary>
        /// 区县市s
        /// </summary>
        /// <param name="sf">省份</param>
        public QXSs(string sf)
        {
            this.Retrieve(QXSAttr.FK_SF, sf);
        }
		#endregion
	}
	
}
