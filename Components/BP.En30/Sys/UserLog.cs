using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// 用户日志
	/// </summary>
    public class UserLogAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 日志标记
        /// </summary>
        public const string LogFlag = "LogFlag";
        /// <summary>
        /// 处理内容
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// IP地址
        /// </summary>
        public const string IP = "IP";
    }
	/// <summary>
	/// 用户日志
	/// </summary>
	public class UserLog: EntityMyPK
	{
        public override UAC HisUAC
        {
            get
            {
                var uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }

		#region 用户日志信息键值列表
		#endregion

		#region 基本属性
        public string IP
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.IP);
            }
            set
            {
                this.SetValByKey(UserLogAttr.IP, value);
            }
        }
        /// <summary>
        /// 日志标记键
        /// </summary>
        public string LogFlag
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.LogFlag);
            }
            set
            {
                this.SetValByKey(UserLogAttr.LogFlag, value);
            }
        }
		/// <summary>
		/// FK_Emp
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(UserLogAttr.FK_Emp) ; 
			}
			set
			{
				this.SetValByKey(UserLogAttr.FK_Emp,value) ; 
			}
		}
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.RDT);
            }
            set
            {
                this.SetValByKey(UserLogAttr.RDT, value);
            }
        }
      
        public string Docs
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.Docs);
            }
            set
            {
                this.SetValByKey(UserLogAttr.Docs, value);
            }
        }
      
		#endregion

		#region 构造方法
		/// <summary>
		/// 用户日志
		/// </summary>
		public UserLog()
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
                Map map = new Map("Sys_UserLogT", "用户日志");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();

                map.AddTBString(UserLogAttr.FK_Emp, null, "用户", true, false, 0, 30, 20);
                map.AddTBString(UserLogAttr.IP, null, "IP", true, false, 0, 200, 20);
                map.AddTBString(UserLogAttr.LogFlag, null, "标识", true, false, 0, 300, 20);
                map.AddTBString(UserLogAttr.Docs, null, "说明", true, false, 0, 300, 20);
                map.AddTBString(UserLogAttr.RDT, null, "记录日期", true, false, 0, 20, 20);

                map.GetAttrByKey(this.PK).UIVisible = false;

                map.DTSearchKey = UserLogAttr.RDT;
                map.DTSearchWay = DTSearchWay.ByDate;

                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new UserLogs(); }
        }
        #endregion 重写
    }
	/// <summary>
	/// 用户日志s
	/// </summary>
    public class UserLogs : EntitiesMyPK
    {
        #region 构造
        public UserLogs()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        public UserLogs(string emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(UserLogAttr.FK_Emp, emp);
            qo.DoQuery();
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserLog();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<UserLog> Tolist()
        {
            System.Collections.Generic.List<UserLog> list = new System.Collections.Generic.List<UserLog>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((UserLog)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<UserLog> ToJavaList()
        {
            return (System.Collections.Generic.IList<UserLog>)this;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
         
    }
}
