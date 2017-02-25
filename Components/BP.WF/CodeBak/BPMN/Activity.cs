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
    /// 活动属性
    /// </summary>
    public class ActivityAttr:EleBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        #endregion
    }
    /// <summary>
    /// 活动
    /// </summary>
    public class Activity : EleBase
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
            get { return FlowObjects.Activities; }
        }
        #endregion

        #region 参数属性.
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public Activity()
        {
        }
        public Activity(string mypk)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(ActivityAttr.MyPK, mypk);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 Activity [" + mypk + "]不存在。");
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

                Map map = new Map("WF_Objs", "活动");

                #region 基础属性.
                map.AddMyPK();
                map.AddTBString(ActivityAttr.Title, null, "标题", true, false, 0, 500, 10, true);
                map.AddTBString(ActivityAttr.EleType, null, "类型(Activities/GetWays/Events)", true, false, 0, 100, 10, true);
                map.AddTBString(ActivityAttr.EleTypeDtl, null, "明细类型", true, false, 0, 100, 10, true);
                map.AddTBInt(ActivityAttr.X, 100, "X", true, false);
                map.AddTBInt(ActivityAttr.Y, 100, "Y", true, false);

                // 关联到ccbpm的信息. 如果 EleTypeDtl=UserActivity就关联到 WF_Node 对象上.
                map.AddTBString(ActivityAttr.RefFlowNo, null, "流程编号", true, false, 0, 100, 10, true);
                map.AddTBString(ActivityAttr.RefType, null, "关联类型", true, false, 0, 100, 10, true);
                map.AddTBString(ActivityAttr.RefPK, null, "关联主键", true, false, 0, 100, 10, true);
                #endregion 基础属性.

                this._enMap= map;
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
    /// 活动s
    /// </summary>
    public class Activitys : EleBases
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Activity();
            }
        }
        /// <summary>
        /// 活动集合
        /// </summary>
        public Activitys() { }

        /// <summary>
        /// 活动集合
        /// </summary>
        /// <param name="flowNo"></param>
        public Activitys(string flowNo)
        {
           
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Activity> ToJavaList()
        {
            return (System.Collections.Generic.IList<Activity>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Activity> Tolist()
        {
            System.Collections.Generic.List<Activity> list = new System.Collections.Generic.List<Activity>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Activity)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }

}
