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
    /// 网关属性
    /// </summary>
    public class GatewayAttr:EleBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        #endregion
    }
    /// <summary>
    /// 网关
    /// </summary>
    public class Gateway : EleBase
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
        /// <summary>
        /// 流程对象
        /// </summary>
        public override string FlowObjs
        {
            get { return FlowObjects.GatWays; }
        }
        #endregion

        #region 参数属性.
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public Gateway()
        {
        }
        public Gateway(string mypk)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GatewayAttr.MyPK, mypk);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 Gateway [" + mypk + "]不存在。");
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

                Map map = new Map("WF_Objs", "网关");

                #region 基础属性.
                map.AddMyPK();
                map.AddTBString(GatewayAttr.Title, null, "标题", true, false, 0, 500, 10, true);
                map.AddTBString(GatewayAttr.EleType, null, "类型(Activities/Gateways/Events)", true, false, 0, 100, 10, true);
                map.AddTBString(GatewayAttr.EleTypeDtl, null, "明细类型", true, false, 0, 100, 10, true);
                map.AddTBInt(GatewayAttr.X, 100, "X", true, false);
                map.AddTBInt(GatewayAttr.Y, 100, "Y", true, false);

                // 关联到ccbpm的信息. 如果 EleTypeDtl=UserGateway就关联到 WF_Node 对象上.
                map.AddTBString(GatewayAttr.RefFlowNo, null, "流程编号", true, false, 0, 100, 10, true);
                map.AddTBString(GatewayAttr.RefType, null, "关联类型", true, false, 0, 100, 10, true);
                map.AddTBString(GatewayAttr.RefPK, null, "关联主键", true, false, 0, 100, 10, true);
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
    /// 网关s
    /// </summary>
    public class Gateways : EleBases
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Gateway();
            }
        }
        /// <summary>
        /// 网关集合
        /// </summary>
        public Gateways() { }

        /// <summary>
        /// 网关集合
        /// </summary>
        /// <param name="flowNo"></param>
        public Gateways(string flowNo)
        {
           
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Gateway> ToJavaList()
        {
            return (System.Collections.Generic.IList<Gateway>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Gateway> Tolist()
        {
            System.Collections.Generic.List<Gateway> list = new System.Collections.Generic.List<Gateway>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Gateway)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
