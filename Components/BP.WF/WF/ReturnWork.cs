using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 

namespace BP.WF
{
	/// <summary>
	/// 退回轨迹
	/// </summary>
	public class ReturnWorkAttr 
	{
		#region 基本属性
		/// <summary>
		/// 工作ID
		/// </summary>
		public const  string WorkID="WorkID";
		/// <summary>
		/// 工作人员
		/// </summary>
		public const  string Worker="Worker";
		/// <summary>
		/// 退回原因
		/// </summary>
		public const  string BeiZhu="BeiZhu";
        /// <summary>
        /// 退回日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 退回人
        /// </summary>
        public const string Returner = "Returner";
        /// <summary>
        /// 退回人名称
        /// </summary>
        public const string ReturnerName = "ReturnerName";
        /// <summary>
        /// 退回到节点
        /// </summary>
        public const string ReturnToNode = "ReturnToNode";
        /// <summary>
        /// 退回节点
        /// </summary>
        public const string ReturnNode = "ReturnNode";
        /// <summary>
        /// 退回节点名称
        /// </summary>
        public const string ReturnNodeName = "ReturnNodeName";
        /// <summary>
        /// 退回给
        /// </summary>
        public const string ReturnToEmp = "ReturnToEmp";
        /// <summary>
        /// 退回后是否需要原路返回？
        /// </summary>
        public const string IsBackTracking = "IsBackTracking";
		#endregion
	}
	/// <summary>
	/// 退回轨迹
	/// </summary>
    public class ReturnWork : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(ReturnWorkAttr.WorkID);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 退回到节点
        /// </summary>
        public int ReturnToNode
        {
            get
            {
                return this.GetValIntByKey(ReturnWorkAttr.ReturnToNode);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.ReturnToNode, value);
            }
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        public int ReturnNode
        {
            get
            {
                return this.GetValIntByKey(ReturnWorkAttr.ReturnNode);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.ReturnNode, value);
            }
        }
        public string ReturnNodeName
        {
            get
            {
                return this.GetValStrByKey(ReturnWorkAttr.ReturnNodeName);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.ReturnNodeName, value);
            }
        }
        /// <summary>
        /// 退回人
        /// </summary>
        public string Returner
        {
            get
            {
                return this.GetValStringByKey(ReturnWorkAttr.Returner);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.Returner, value);
            }
        }
        public string ReturnerName
        {
            get
            {
                return this.GetValStringByKey(ReturnWorkAttr.ReturnerName);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.ReturnerName, value);
            }
        }
        /// <summary>
        /// 退回给
        /// </summary>
        public string ReturnToEmp
        {
            get
            {
                return this.GetValStringByKey(ReturnWorkAttr.ReturnToEmp);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.ReturnToEmp, value);
            }
        }
        public string BeiZhu
        {
            get
            {
                return this.GetValStringByKey(ReturnWorkAttr.BeiZhu);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.BeiZhu, value);
            }
        }
        public string BeiZhuHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(ReturnWorkAttr.BeiZhu);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(ReturnWorkAttr.RDT);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.RDT, value);
            }
        }
        /// <summary>
        /// 是否要原路返回？
        /// </summary>
        public bool IsBackTracking
        {
            get
            {
                return this.GetValBooleanByKey(ReturnWorkAttr.IsBackTracking);
            }
            set
            {
                SetValByKey(ReturnWorkAttr.IsBackTracking, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 退回轨迹
        /// </summary>
        public ReturnWork() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_ReturnWork", "退回轨迹");

                map.AddMyPK();

                map.AddTBInt(ReturnWorkAttr.WorkID, 0, "WorkID", true, true);

                map.AddTBInt(ReturnWorkAttr.ReturnNode, 0, "退回节点", true, true);
                map.AddTBString(ReturnWorkAttr.ReturnNodeName, null, "退回节点名称", true, true, 0, 100, 10);

                map.AddTBString(ReturnWorkAttr.Returner, null, "退回人", true, true, 0, 50, 10);
                map.AddTBString(ReturnWorkAttr.ReturnerName, null, "退回人名称", true, true, 0, 100, 10);

                map.AddTBInt(ReturnWorkAttr.ReturnToNode, 0, "ReturnToNode", true, true);
                map.AddTBString(ReturnWorkAttr.ReturnToEmp, null, "退回给", true, true, 0, 4000, 10);

                map.AddTBString(ReturnWorkAttr.BeiZhu, null, "退回原因", true, true, 0, 4000, 10);
                map.AddTBDateTime(ReturnWorkAttr.RDT, null, "退回日期", true, true);

                map.AddTBInt(ReturnWorkAttr.IsBackTracking, 0, "是否要原路返回?", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeInsert()
        {
            this.Returner = BP.Web.WebUser.No;
            this.ReturnerName = BP.Web.WebUser.Name;

            this.RDT =DataType.CurrentDataTime;
            return base.beforeInsert();
        }
    }
	/// <summary>
	/// 退回轨迹s 
	/// </summary>
	public class ReturnWorks : Entities
	{	 
		#region 构造
		/// <summary>
		/// 退回轨迹s
		/// </summary>
		public ReturnWorks()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ReturnWork();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ReturnWork> ToJavaList()
        {
            return (System.Collections.Generic.IList<ReturnWork>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ReturnWork> Tolist()
        {
            System.Collections.Generic.List<ReturnWork> list = new System.Collections.Generic.List<ReturnWork>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ReturnWork)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
