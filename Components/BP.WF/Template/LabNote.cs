using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF.Template;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 标签属性
    /// </summary>
    public class LabNoteAttr:BP.En.EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// x
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// y
        /// </summary>
        public const string Y = "Y";
        #endregion
    }
    /// <summary>
    /// 标签.	 
    /// </summary>
    public class LabNote : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                return uac;
            }
        }

        /// <summary>
        /// x
        /// </summary>
        public int X
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.X);
            }
            set
            {
                this.SetValByKey(NodeAttr.X, value);
            }
        }

        /// <summary>
        /// y
        /// </summary>
        public int Y
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Y);
            }
            set
            {
                this.SetValByKey(NodeAttr.Y, value);
            }
        }
        /// <summary>
        /// 标签的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                SetValByKey(NodeAttr.Name, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 标签
        /// </summary>
        public LabNote() { }
        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="_oid">标签ID</param>	
        public LabNote(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
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

                Map map = new Map("WF_LabNote", "标签");
                map.IndexField = NodeAttr.FK_Flow;

                map.AddMyPK();

                map.AddTBString(NodeAttr.Name, null, null, true, false, 0, 3000, 10, true);
                map.AddTBString(NodeAttr.FK_Flow, null, "流程", false, true, 0, 100, 10);

                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerOID().ToString();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 标签集合
    /// </summary>
    public class LabNotes : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new LabNote();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 标签集合
        /// </summary>
        public LabNotes()
        {
        }
        /// <summary>
        /// 标签集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public LabNotes(string fk_flow)
        {
            this.Retrieve(NodeAttr.FK_Flow, fk_flow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<LabNote> ToJavaList()
        {
            return (System.Collections.Generic.IList<LabNote>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<LabNote> Tolist()
        {
            System.Collections.Generic.List<LabNote> list = new System.Collections.Generic.List<LabNote>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((LabNote)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
