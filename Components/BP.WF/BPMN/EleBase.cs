using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.WF;
using BP.Sys;

namespace BP.BPMN
{
    /// <summary>
    ///  元素基类属性
    /// </summary>
    public class EleBaseAttr:BP.En.EntityMyPKAttr
    {
        #region 基础属性.
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// Y
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// X
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 元素类型
        /// </summary>
        public const string EleType = "EleType";
        /// <summary>
        /// 元素明细
        /// </summary>
        public const string EleTypeDtl = "EleTypeDtl";
        /// <summary>
        /// 关联的ccbpm类型
        /// </summary>
        public const string RefType = "RefType";
        /// <summary>
        /// 关联的主键
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string RefFlowNo = "RefFlowNo";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        #endregion 基础属性.
    }
    /// <summary>
    /// 元素基类
    /// </summary>
    abstract public class EleBase : BP.En.EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 元素类型
        /// </summary>
        public abstract string FlowObjs
        {
            get;
        }
        /// <summary>
        /// X
        /// </summary>
        public int Y
        {
            get
            {
                return this.GetValIntByKey(EleBaseAttr.Y);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public int X
        {
            get
            {
                return this.GetValIntByKey(EleBaseAttr.X);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.X, value);
            }
        }
      
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.Title);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.Title, value);
            }
        }
        /// <summary>
        /// 元素类型
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.EleType);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.EleType, value);
            }
        }
        /// <summary>
        /// 元素类型明细
        /// </summary>
        public string EleTypeDtl
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.EleTypeDtl);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.EleTypeDtl, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string RefFlowNo
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.RefFlowNo);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.RefFlowNo, value);
            }
        }
        /// <summary>
        /// 关联到的ccbpm元素类型.
        /// </summary>
        public string RefType
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.RefType);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.RefType, value);
            }
        }
        /// <summary>
        /// 关联到的ccbpm元素值.
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(EleBaseAttr.RefPK);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.RefPK, value);
            }
        }
        public int RefPKInt
        {
            get
            {
                return this.GetValIntByKey(EleBaseAttr.RefPK);
            }
            set
            {
                this.SetValByKey(EleBaseAttr.RefPK, value);
            }
        }
        #endregion attrs

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        protected EleBase()
        {
        }
        /// <summary>
        /// 根据OID构造实体
        /// </summary>
        /// <param name="主键">mypk</param>
        protected EleBase(string mypk)
            : base(mypk)
        {
        }
        #endregion 构造

        #region 重写方法.
        protected override bool beforeUpdateInsertAction()
        {
            #region 更新任务.
            if (this.RefType == ActivityList.UserTask)
            {
                if (this.RefPKInt == 0)
                {
                    BP.WF.Flow fl = new BP.WF.Flow(this.RefFlowNo);
                    Node nd = fl.DoNewNode(this.X, this.Y);
                    nd.Name = this.Title;
                    nd.Update();

                    //节点ID.
                    this.RefPKInt = nd.NodeID;
                }
                else
                {
                    /*人机交互流程*/
                    Node nd = new Node(this.RefPKInt);
                    nd.Name = this.Title;
                    nd.X = this.X;
                    nd.Y = this.Y;
                    nd.Update();
                }
            }

            if (this.RefType == ActivityList.ServiceTask)
            {
                if (this.RefPKInt == 0)
                {
                    BP.WF.Flow fl = new BP.WF.Flow(this.RefFlowNo);
                    Node nd = fl.DoNewNode(this.X, this.Y);
                    nd.Name = this.Title;
                    nd.Update();

                    //节点ID.
                    this.RefPKInt = nd.NodeID;
                }
                else
                {
                    /*人机交互流程*/
                    Node nd = new Node(this.RefPKInt);
                    nd.Name = this.Title;
                    nd.X = this.X;
                    nd.Y = this.Y;
                    nd.Update();
                }
            }
            #endregion 更新任务.

            //更新线
            if (this.RefType == ActivityList.SequenceFlow)
            {
                Direction dir = new Direction();
             //   dir.fk
            }

            return base.beforeUpdateInsertAction();
        }
        #endregion 重写方法.
    }
    /// <summary>
    /// 元素基类s
    /// </summary>
    abstract public class EleBases : BP.En.EntitiesMyPK
    {
        /// <summary>
        /// 元素基类s
        /// </summary>
        public EleBases()
        {
        }
    }
}
