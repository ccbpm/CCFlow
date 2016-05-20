using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.En;

namespace BP.BPMN
{
    /// <summary>
    /// 事件属性
    /// </summary>
    public class EventAttr:EleBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        #endregion
    }
    /// <summary>
    /// 事件
    /// </summary>
    public class Event : EleBase
    {
        #region 基本属性
     
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        public override string FlowObjs
        {
            get { return FlowObjects.Events; }
        }
        #endregion

        #region 参数属性.
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public Event()
        {
        }
        public Event(string mypk)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EventAttr.MyPK, mypk);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 Event [" + mypk + "]不存在。");
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

                Map map = new Map("WF_Objs", "事件");


                #region 基础属性.
                map.AddMyPK();
                map.AddTBString(EventAttr.Title, null, "标题", true, false, 0, 500, 10, true);
                map.AddTBString(EventAttr.EleType, null, "类型(Activities/Events/Events)", true, false, 0, 100, 10, true);
                map.AddTBString(EventAttr.EleTypeDtl, null, "明细类型", true, false, 0, 100, 10, true);
                map.AddTBInt(EventAttr.X, 100, "X", true, false);
                map.AddTBInt(EventAttr.Y, 100, "Y", true, false);

                // 关联到ccbpm的信息. 如果 EleTypeDtl=UserEvent就关联到 WF_Node 对象上.
                map.AddTBString(EventAttr.RefFlowNo, null, "流程编号", true, false, 0, 100, 10, true);
                map.AddTBString(EventAttr.RefType, null, "关联类型", true, false, 0, 100, 10, true);
                map.AddTBString(EventAttr.RefPK, null, "关联主键", true, false, 0, 100, 10, true);
                #endregion 基础属性.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行业务重写
        /// <summary>
        /// 更新之前要重写，业务逻辑让其更新到相关的表里去.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 事件s
    /// </summary>
    public class Events : EleBases
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Event();
            }
        }
        /// <summary>
        /// 事件集合
        /// </summary>
        public Events() { }

        /// <summary>
        /// 事件集合
        /// </summary>
        /// <param name="flowNo"></param>
        public Events(string flowNo)
        {
           
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Event> ToJavaList()
        {
            return (System.Collections.Generic.IList<Event>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Event> Tolist()
        {
            System.Collections.Generic.List<Event> list = new System.Collections.Generic.List<Event>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Event)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }

}
