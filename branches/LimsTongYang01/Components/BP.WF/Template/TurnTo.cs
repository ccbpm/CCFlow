using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.Template
{
    /// <summary>
    /// 条件属性
    /// </summary>
    public class TurnToAttr
    {
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string FK_Attr = "FK_Attr";
        /// <summary>
        /// 名称
        /// </summary>
        public const string AttrT = "AttrT";
        /// <summary>
        /// 运算符号
        /// </summary>
        public const string FK_Operator = "FK_Operator";
        /// <summary>
        /// 运算的值
        /// </summary>
        public const string OperatorValue = "OperatorValue";
        /// <summary>
        /// 操作值
        /// </summary>
        public const string OperatorValueT = "OperatorValueT";
        /// <summary>
        /// Node
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 条件类型
        /// </summary>
        public const string TurnToType = "TurnToType";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// TurnToURL
        /// </summary>
        public const string TurnToURL = "TurnToURL";
        /// <summary>
        /// AttrKey
        /// </summary>
        public const string AttrKey = "AttrKey";
    }
    /// <summary>
    /// 条件类型
    /// </summary>
    public enum TurnToType
    {
        /// <summary>
        /// 节点
        /// </summary>
        Node,
        /// <summary>
        /// 流程
        /// </summary>
        Flow
    }
    /// <summary>
    /// 条件
    /// </summary>
    public class TurnTo : EntityMyPK
    {
        /// <summary>
        /// 流程
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(TurnToAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(TurnToAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(TurnToAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 条件类型
        /// </summary>
        public TurnToType HisTurnToType
        {
            get
            {
                return (TurnToType)this.GetValIntByKey(TurnToAttr.TurnToType);
            }
            set
            {
                this.SetValByKey(TurnToAttr.TurnToType, (int)value);
            }
        }
        /// <summary>
        /// 转向URL
        /// </summary>
        public string TurnToURL
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.TurnToURL);
            }
            set
            {
                this.SetValByKey(TurnToAttr.TurnToURL, value);
            }
        }

        #region 实现基本的方方法
        /// <summary>
        /// 属性
        /// </summary>
        public string FK_Attr
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.FK_Attr);
            }
            set
            {
                if (value == null)
                    throw new Exception("FK_Attr不能设置为null");

                value = value.Trim();
                this.SetValByKey(TurnToAttr.FK_Attr, value);
                BP.Sys.MapAttr attr = new BP.Sys.MapAttr(value);
                this.SetValByKey(TurnToAttr.AttrKey, attr.KeyOfEn);
                this.SetValByKey(TurnToAttr.AttrT, attr.Name);
            }
        }
        /// <summary>
        /// 属性Key
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.AttrKey);
            }
        }
        /// <summary>
        /// 属性Text
        /// </summary>
        public string AttrT
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.AttrT);
            }
        }
        /// <summary>
        /// 操作的值
        /// </summary>
        public string OperatorValueT
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.OperatorValueT);
            }
            set
            {
                this.SetValByKey(TurnToAttr.OperatorValueT, value);
            }
        }
        /// <summary>
        /// 运算符号
        /// </summary>
        public string FK_Operator
        {
            get
            {
                string s = this.GetValStringByKey(TurnToAttr.FK_Operator);
                if (s == null || s == "")
                    return "=";
                return s;
            }
            set
            {
                this.SetValByKey(TurnToAttr.FK_Operator, value);
            }
        }
        /// <summary>
        /// 操作符描述
        /// </summary>
        public string FK_OperatorExt
        {
            get
            {
                string s = this.FK_Operator.ToLower();
                switch (s)
                {
                    case "=":
                        return "dengyu";
                    case ">":
                        return "dayu";
                    case ">=":
                        return "dayudengyu";
                    case "<":
                        return "xiaoyu";
                    case "<=":
                        return "xiaoyudengyu";
                    case "!=":
                        return "budengyu";
                    case "like":
                        return "like";
                    default:
                        return s;
                }
            }
        }
        /// <summary>
        /// 运算值
        /// </summary>
        public object OperatorValue
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.OperatorValue);
            }
            set
            {
                this.SetValByKey(TurnToAttr.OperatorValue, value as string);
            }
        }
        /// <summary>
        /// 操作值str
        /// </summary>
        public string OperatorValueStr
        {
            get
            {
                return this.GetValStringByKey(TurnToAttr.OperatorValue);
            }
        }
        /// <summary>
        /// 操作值int
        /// </summary>
        public int OperatorValueInt
        {
            get
            {
                return this.GetValIntByKey(TurnToAttr.OperatorValue);
            }
        }
        /// <summary>
        /// 操作值boolen
        /// </summary>
        public bool OperatorValueBool
        {
            get
            {
                return this.GetValBooleanByKey(TurnToAttr.OperatorValue);
            }
        }
        /// <summary>
        /// workid
        /// </summary>
        public Int64 WorkID = 0;
        /// <summary>
        /// 转向消息
        /// </summary>
        public string MsgOfTurnTo = "";
        #endregion

        #region 构造方法
        /// <summary>
        /// 条件
        /// </summary>
        public TurnTo() { }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="mypk">PK</param>
        public TurnTo(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 它的工作
        /// </summary>
        public Work HisWork = null;
        /// <summary>
        /// 这个条件能不能通过
        /// </summary>
        public virtual bool IsPassed
        {
            get
            {

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
                attr.MyPK = this.FK_Attr;
                attr.RetrieveFromDBSources();

                if (this.HisWork.EnMap.Attrs.Contains(attr.KeyOfEn) == false)
                    throw new Exception("判断条件方向出现错误：实体：" + this.HisWork.EnDesc + " 属性" + this.FK_Attr + "已经不存在.");

                this.MsgOfTurnTo = "@以表单值判断方向，值 " + this.HisWork.EnDesc + "." + this.FK_Attr + " (" + this.HisWork.GetValStringByKey(attr.KeyOfEn) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";

                switch (this.FK_Operator.Trim().ToLower())
                {
                    case "=":  // 如果是 = 
                    case "dengyu":  // 如果是 = 
                        if (this.HisWork.GetValStringByKey(attr.KeyOfEn) == this.OperatorValue.ToString())
                            return true;
                        else
                            return false;
                    case ">":
                    case "dayu":
                        if (this.HisWork.GetValDoubleByKey(attr.KeyOfEn) > Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;

                    case ">=":
                    case "dayudengyu":
                        if (this.HisWork.GetValDoubleByKey(attr.KeyOfEn) >= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;

                    case "<":
                    case "xiaoyu":
                        if (this.HisWork.GetValDoubleByKey(attr.KeyOfEn) < Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;

                    case "<=":
                    case "xiaoyudengyu":
                        if (this.HisWork.GetValDoubleByKey(attr.KeyOfEn) <= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "!=":
                    case "budengyu":
                        if (this.HisWork.GetValDoubleByKey(attr.KeyOfEn) != Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "like":
                        if (this.HisWork.GetValStringByKey(attr.KeyOfEn).IndexOf(this.OperatorValue.ToString()) == -1)
                            return false;
                        else
                            return true;
                    default:
                        throw new Exception("@没有找到操作符号..");
                }
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_TurnTo", "转向条件");

                map.AddMyPK();
                map.AddTBInt(TurnToAttr.TurnToType, 0, "条件类型", true, true);
                map.AddTBString(TurnToAttr.FK_Flow, null, "流程", true, true, 0, 60, 20);
                map.AddTBInt(TurnToAttr.FK_Node, 0, "节点ID", true, true);

                map.AddTBString(TurnToAttr.FK_Attr, null, "属性外键Sys_MapAttr", true, true, 0, 80, 20);
                map.AddTBString(TurnToAttr.AttrKey, null, "键值", true, true, 0, 80, 20);
                map.AddTBString(TurnToAttr.AttrT, null, "属性名称", true, true, 0, 80, 20);

                map.AddTBString(TurnToAttr.FK_Operator, "=", "运算符号", true, true, 0, 60, 20);

                map.AddTBString(TurnToAttr.OperatorValue, "", "要运算的值", true, true, 0, 60, 20);
                map.AddTBString(TurnToAttr.OperatorValueT, "", "要运算的值T", true, true, 0, 60, 20);

                map.AddTBString(TurnToAttr.TurnToURL, null, "要转向的URL", true, true, 0, 700, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 条件s
    /// </summary>
    public class TurnTos : Entities
    {
        #region 属性
        /// <summary>
        /// 条件
        /// </summary>
        public override Entity GetNewEntity
        {
            get { return new TurnTo(); }
        }
        /// <summary>
        /// 条件.
        /// </summary>
        public bool IsAllPassed
        {
            get
            {
                if (this.Count == 0)
                    throw new Exception("@没有要判断的集合.");

                foreach (TurnTo en in this)
                {
                    if (en.IsPassed == false)
                        return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass
        {
            get
            {
                if (this.Count == 1)
                    if (this.IsOneOfTurnToPassed)
                        return true;
                    else
                        return false;
                return false;
            }
        }
        public string MsgOfDesc
        {
            get
            {
                string msg = "";
                foreach (TurnTo c in this)
                {
                    msg += "@" + c.MsgOfTurnTo;
                }
                return msg;
            }
        }
        /// <summary>
        /// 是不是其中的一个passed. 
        /// </summary>
        public bool IsOneOfTurnToPassed
        {
            get
            {
                foreach (TurnTo en in this)
                {
                    if (en.IsPassed == true)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 取出其中一个的完成条件。. 
        /// </summary>
        public TurnTo GetOneOfTurnToPassed
        {
            get
            {
                foreach (TurnTo en in this)
                {
                    if (en.IsPassed == true)
                        return en;
                }
                throw new Exception("@没有完成条件。");
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID = 0;
        #endregion

        #region 构造
        /// <summary>
        /// 条件
        /// </summary>
        public TurnTos()
        {
        }
        /// <summary>
        /// 条件
        /// </summary>
        public TurnTos(string fk_flow)
        {
            this.Retrieve(TurnToAttr.FK_Flow, fk_flow);
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="ct">类型</param>
        /// <param name="nodeID">节点</param>
        public TurnTos(TurnToType ct, int nodeID, Int64 workid)
        {
            this.NodeID = nodeID;
            this.Retrieve(TurnToAttr.FK_Node, nodeID, TurnToAttr.TurnToType, (int)ct);

            foreach (TurnTo en in this)
                en.WorkID = workid;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string TurnToitionDesc
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TurnTo> ToJavaList()
        {
            return (System.Collections.Generic.IList<TurnTo>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TurnTo> Tolist()
        {
            System.Collections.Generic.List<TurnTo> list = new System.Collections.Generic.List<TurnTo>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TurnTo)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
