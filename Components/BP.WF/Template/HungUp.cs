using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 挂起 属性
	/// </summary>
    public class HungUpAttr:EntityMyPKAttr
    {
        #region 基本属性
        public const string Title = "Title";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 执行人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 通知给
        /// </summary>
        public const string NoticeTo = "NoticeTo";
        /// <summary>
        /// 挂起原因
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 挂起时间
        /// </summary>
        public const string DTOfHungUp = "DTOfHungUp";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 接受人
        /// </summary>
        public const string Accepter = "Accepter";
        /// <summary>
        /// 挂起方式.
        /// </summary>
        public const string HungUpWay = "HungUpWay";
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public const string DTOfUnHungUp = "DTOfUnHungUp";
        /// <summary>
        /// 计划解除挂起时间
        /// </summary>
        public const string DTOfUnHungUpPlan = "DTOfUnHungUpPlan";
        #endregion
    }
	/// <summary>
	/// 挂起
	/// </summary>
    public class HungUp:EntityMyPK
    {
        #region 属性
        public HungUpWay HungUpWay
        {
            get
            {
                return (HungUpWay)this.GetValIntByKey(HungUpAttr.HungUpWay);
            }
            set
            {
                this.SetValByKey(HungUpAttr.HungUpWay, (int)value);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(HungUpAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(HungUpAttr.FK_Node, value);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(HungUpAttr.WorkID);
            }
            set
            {
                this.SetValByKey(HungUpAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 挂起标题
        /// </summary>
        public string Title
        {
            get
            {
                string s= this.GetValStringByKey(HungUpAttr.Title);
                if (DataType.IsNullOrEmpty(s))
                    s = "来自@Rec的挂起信息.";
                return s;
            }
            set
            {
                this.SetValByKey(HungUpAttr.Title, value);
            }
        }
        /// <summary>
        /// 挂起原因
        /// </summary>
        public string Note
        {
            get
            {
               return this.GetValStringByKey(HungUpAttr.Note);
            }
            set
            {
                this.SetValByKey(HungUpAttr.Note, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.Rec);
            }
            set
            {
                this.SetValByKey(HungUpAttr.Rec, value);
            }
        }
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public string DTOfUnHungUp
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfUnHungUp);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfUnHungUp, value);
            }
        }
        /// <summary>
        /// 预计解除挂起时间
        /// </summary>
        public string DTOfUnHungUpPlan
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfUnHungUpPlan);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfUnHungUpPlan, value);
            }
        }
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public string DTOfHungUp
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfHungUp);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfHungUp, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 挂起
        /// </summary>
        public HungUp()
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

                Map map = new Map("WF_HungUp", "挂起");
                map.Java_SetEnType(EnType.Admin);

                map.AddMyPK();
                map.AddTBInt(HungUpAttr.FK_Node, 0, "节点ID", true, true);
                map.AddTBInt(HungUpAttr.WorkID, 0, "WorkID", true, true);
                map.AddDDLSysEnum(HungUpAttr.HungUpWay, 0, "挂起方式", true, true, HungUpAttr.HungUpWay, 
                    "@0=无限挂起@1=按指定的时间解除挂起并通知我自己@2=按指定的时间解除挂起并通知所有人");

                map.AddTBStringDoc(HungUpAttr.Note, null, "挂起原因(标题与内容支持变量)", true, false, true);

                map.AddTBString(HungUpAttr.Rec, null, "挂起人", true, false, 0, 50, 10, true);

                map.AddTBDateTime(HungUpAttr.DTOfHungUp, null, "挂起时间", true, false);
                map.AddTBDateTime(HungUpAttr.DTOfUnHungUp, null, "实际解除挂起时间", true, false);
                map.AddTBDateTime(HungUpAttr.DTOfUnHungUpPlan, null, "预计解除挂起时间", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 执行释放挂起
        /// </summary>
        public void DoRelease()
        {
        }
        #endregion
    }
	/// <summary>
	/// 挂起
	/// </summary>
	public class HungUps: EntitiesMyPK
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new HungUp();
			}
		}
		/// <summary>
        /// 挂起
		/// </summary>
		public HungUps(){} 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<HungUp> ToJavaList()
        {
            return (System.Collections.Generic.IList<HungUp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<HungUp> Tolist()
        {
            System.Collections.Generic.List<HungUp> list = new System.Collections.Generic.List<HungUp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((HungUp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
