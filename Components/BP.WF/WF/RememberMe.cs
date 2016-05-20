
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 记忆我 属性
	/// </summary>
    public class RememberMeAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 当前节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 可执行人员
        /// </summary>
        public const string Objs = "Objs";
        /// <summary>
        /// 可执行人员
        /// </summary>
        public const string ObjsExt = "ObjsExt";
        /// <summary>
        /// 可执行人员数据量
        /// </summary>
        public const string NumOfObjs = "NumOfObjs";
        /// <summary>
        /// 工作人员（候选)
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// 工作人员个数（候选)
        /// </summary>
        public const string NumOfEmps = "NumOfEmps";
        /// <summary>
        /// 工作人员（候选)
        /// </summary>
        public const string EmpsExt = "EmpsExt";
        #endregion
    }
	/// <summary>
	/// 记忆我
	/// </summary>
    public class RememberMe : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 操作员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.FK_Emp, value);
                this.MyPK = this.FK_Node + "_" + BP.Web.WebUser.No;
            }
        }
        /// <summary>
        /// 当前节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(RememberMeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.FK_Node, value);
                this.MyPK = this.FK_Node + "_" + BP.Web.WebUser.No;
            }
        }
        /// <summary>
        /// 有效的工作人员
        /// </summary>
        public string Objs
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.Objs);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.Objs, value);
            }
        }
        /// <summary>
        /// 有效的操作人员ext
        /// </summary>
        public string ObjsExt
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.ObjsExt);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.ObjsExt, value);
            }
        }
        /// <summary>
        /// 所有的人员数量.
        /// </summary>
        public int NumOfEmps
        {
            get
            {
                return this.Emps.Split('@').Length - 2;
            }
        }
        /// <summary>
        /// 可以处理的人员数量
        /// </summary>
        public int NumOfObjs
        {
            get
            {
                return this.Objs.Split('@').Length - 2;
            }
        }
        /// <summary>
        /// 所有的工作人员
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.Emps);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.Emps, value);
            }
        }
        /// <summary>
        /// 所有的工作人员ext
        /// </summary>
        public string EmpsExt
        {
            get
            {
                string str = this.GetValStringByKey(RememberMeAttr.EmpsExt).Trim();
                if (str.Length == 0)
                    return str;

                if (str.Substring(str.Length - 1) == "、")
                    return str.Substring(0, str.Length - 1);
                else
                    return str;
            }
            set
            {
                this.SetValByKey(RememberMeAttr.EmpsExt, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// RememberMe
        /// </summary>
        public RememberMe()
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
                Map map = new Map("WF_RememberMe", "记忆我");

                map.AddMyPK();

                map.AddTBInt(RememberMeAttr.FK_Node, 0, "节点", false, false);
                map.AddTBString(RememberMeAttr.FK_Emp, "", "当前操作人员", true, false, 1, 30, 10);

                map.AddTBString(RememberMeAttr.Objs, "", "分配人员", true, false, 0, 4000, 10);
                map.AddTBString(RememberMeAttr.ObjsExt, "", "分配人员Ext", true, false, 0, 4000, 10);

                map.AddTBString(RememberMeAttr.Emps, "", "所有的工作人员", true, false, 0, 4000, 10);
                map.AddTBString(RememberMeAttr.EmpsExt, "", "工作人员Ext", true, false, 0, 4000, 10);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.FK_Emp = BP.Web.WebUser.No;
            this.MyPK = this.FK_Node + "_" + this.FK_Emp;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
	/// 记忆我
	/// </summary>
	public class RememberMes: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new RememberMe();
			}
		}
		/// <summary>
		/// RememberMe
		/// </summary>
		public RememberMes(){} 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<RememberMe> ToJavaList()
        {
            return (System.Collections.Generic.IList<RememberMe>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RememberMe> Tolist()
        {
            System.Collections.Generic.List<RememberMe> list = new System.Collections.Generic.List<RememberMe>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RememberMe)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
