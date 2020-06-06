﻿using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Port;
using System.Security.Cryptography;
using System.Text;
using BP.WF.XML;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 系统约定字段列表
    /// </summary>
    public class WorkSysFieldAttr
    {
        /// <summary>
        /// 发送人员字段
        /// 用在节点发送时确定下一个节点接受人员, 类似与发送邮件来选择接受人.
        /// 并且在下一个节点属性的 访问规则中选择【按表单SysSendEmps字段计算】有效。
        /// </summary>
        public const string SysSendEmps = "SysSendEmps";
        /// <summary>
        /// 抄送人员字段
        /// 当前的工作需要抄送时, 就需要在当前节点表单中，增加此字段。
        /// 并且在节点属性的抄送规则中选择【按表单SysCCEmps字段计算】有效。
        /// 如果有多个操作人员，字段的接受值用逗号分开。比如: zhangsan,lisi,wangwu
        /// </summary>
        public const string SysCCEmps = "SysCCEmps";
        /// <summary>
        /// 流程应完成日期
        /// 说明：在开始节点表单中增加此字段，用来标记此流程应当完成的日期.
        /// 用户在发送后就会把此值记录在WF_GenerWorkFlow 的 SDTOfFlow 中.
        /// 此字段显示在待办，发起，在途，删除，挂起列表里.
        /// </summary>
        public const string SysSDTOfFlow = "SysSDTOfFlow";
        /// <summary>
        /// 节点应完成时间
        /// 说明：在开始节点表单中增加此字段，用来标记此节点的下一个节点应该完成的日期.
        /// </summary>
        public const string SysSDTOfNode = "SysSDTOfNode";
        /// <summary>
        /// PWorkID 调用
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// FromNode
        /// </summary>
        public const string FromNode = "FromNode";
        /// <summary>
        /// 是否需要已读回执
        /// </summary>
        public const string SysIsReadReceipts = "SysIsReadReceipts";

        #region 与质量考核相关的字段
        /// <summary>
        /// 编号
        /// </summary>
        public const string EvalEmpNo = "EvalEmpNo";
        /// <summary>
        /// 名称
        /// </summary>
        public const string EvalEmpName = "EvalEmpName";
        /// <summary>
        /// 分值
        /// </summary>
        public const string EvalCent = "EvalCent";
        /// <summary>
        /// 内容
        /// </summary>
        public const string EvalNote = "EvalNote";
        #endregion 与质量考核相关的字段
    }
    /// <summary>
    /// 工作属性
    /// </summary>
    public class WorkAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string OID = "OID";      
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人Text
        /// </summary>
        public const string RecText = "RecText";
        /// <summary>
        /// Emps
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// MD5
        /// </summary>
        public const string MD5 = "MD5";
        #endregion
    }
    public class SubThreadWorkAttr: WorkAttr
    {
        #region 基本属性
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        #endregion
    }
    /// <summary>
    /// WorkBase 的摘要说明。
    /// 工作
    /// </summary>
    abstract public class Work : Entity
    {
        /// <summary>
        /// 检查MD5值是否通过
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsPassCheckMD5()
        {
            string md51 = this.GetValStringByKey(WorkAttr.MD5);
            string md52 = Glo.GenerMD5(this);
            if (md51 != md52)
                return false;
            return true;
        }

        #region 基本属性(必须的属性)
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "OID";
            }
        }
        /// <summary>
        /// classID
        /// </summary>
        public override string ClassID
        {
            get
            {
                return "ND"+this.HisNode.NodeID;
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public virtual Int64 FID
        {
            get
            {
                if (this.HisNode.HisRunModel != RunModel.SubThread)
                    return 0;
               
                return this.GetValInt64ByKey(WorkAttr.FID);
               
            }
            set
            {
                if (this.HisNode.HisRunModel != RunModel.SubThread)
                    this.SetValByKey(WorkAttr.FID, 0);
                else
                    this.SetValByKey(WorkAttr.FID, value);
            }
        }
        /// <summary>
        /// workid,如果是空的就返回 0 . 
        /// </summary>
        public virtual Int64 OID
        {
            get
            {
                return this.GetValInt64ByKey(WorkAttr.OID);
            }
            set
            {
                this.SetValByKey(WorkAttr.OID, value);
            }
        }
        /// <summary>
        /// 人员emps
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStringByKey(WorkAttr.Emps);
            }
            set
            {
                this.SetValByKey(WorkAttr.Emps, value);
            }
        }
        public int RetrieveFID()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereIn(WorkAttr.OID, "(" + this.FID + "," + this.OID + ")");
            int i = qo.DoQuery();
            if (i == 0)
            {
                if (SystemConfig.IsDebug == false)
                {
                    this.CheckPhysicsTable();
                    throw new Exception("@节点[" + this.EnDesc + "]数据丢失：WorkID=" + this.OID + " FID=" + this.FID + " sql=" + qo.SQL);
                }
            }
            return i;
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                string str = this.GetValStringByKey(WorkAttr.Rec);
                if (str == "")
                    this.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);

                return this.GetValStringByKey(WorkAttr.Rec);
            }
            set
            {
                this.SetValByKey(WorkAttr.Rec, value);
            }
        }
        /// <summary>
        /// 工作人员
        /// </summary>
        public Emp RecOfEmp
        {
            get
            {
                return new Emp(this.Rec);
            }
        }
   
        private Node _HisNode = null;
        /// <summary>
        /// 工作的节点.
        /// </summary>
        public Node HisNode
        {
            get
            {
                if (this._HisNode == null)
                {
                    this._HisNode = new Node(this.NodeID);
                }
                return _HisNode;
            }
            set
            {
                _HisNode = value;
            }
        }
        /// <summary>
        /// 从表.
        /// </summary>
        public MapDtls HisMapDtls
        {
            get
            {
                return this.HisNode.MapData.MapDtls;
            }
        }
        /// <summary>
        /// 从表.
        /// </summary>
        public FrmAttachments HisFrmAttachments
        {
            get
            {
                return this.HisNode.MapData.FrmAttachments;
            }
        }
        #endregion

        

        #region 构造函数
        /// <summary>
        /// 工作
        /// </summary>
        protected Work()
        {
        }
        /// <summary>
        /// 工作
        /// </summary>
        /// <param name="oid">WFOID</param>		 
        protected Work(Int64 oid)
        {
            this.SetValByKey(EntityOIDAttr.OID, oid);
            this.Retrieve();
        }
        #endregion

        #region  重写基类的方法。
        /// <summary>
        /// 按照指定的OID Insert.
        /// </summary>
        public void InsertAsOID(Int64 oid)
        {
            this.SetValByKey("OID", oid);
            this.RunSQL(SqlBuilder.Insert(this));
        }
        /// <summary>
        /// 按照指定的OID 保存
        /// </summary>
        /// <param name="oid"></param>
        public void SaveAsOID(Int64 oid)
        {
            this.SetValByKey("OID", oid);
            if (this.RetrieveNotSetValues().Rows.Count == 0)
                this.InsertAsOID(oid);
            this.Update();
        }
        /// <summary>
        /// 保存实体信息
        /// </summary>
        public new int Save()
        {
            if (this.OID <= 10)
                throw new Exception("@没有给WorkID赋值,不能保存.");
            if (this.Update() == 0)
            {
                this.InsertAsOID(this.OID);
                return 0;
            }
            return 1;
        }
        public override void Copy(DataRow dr)  {
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (  attr.Key == WorkAttr.Rec
                   || attr.Key == WorkAttr.FID
                   || attr.Key == WorkAttr.OID
                   || attr.Key == "No"
                   || attr.Key == "Name")
                    continue;

                try
                {
                    this.SetValByKey(attr.Key, dr[attr.Key]);
                }
                catch
                {
                }
            }
        }
        public override void Copy(Entity fromEn)
        {
            if (fromEn == null)
                return;
            Attrs attrs = fromEn.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (  attr.Key == WorkAttr.Rec
                    || attr.Key == WorkAttr.FID
                    || attr.Key == WorkAttr.OID
                    || attr.Key == WorkAttr.Emps
                    || attr.Key == "No"
                    || attr.Key == "Name")
                    continue;
                this.SetValByKey(attr.Key, fromEn.GetValByKey(attr.Key));
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 直接的保存前要做的工作
        /// </summary>
        public virtual void BeforeSave()
        {
            // 执行保存前的事件。
            this.HisNode.HisFlow.DoFlowEventEntity(EventListOfNode.SaveBefore, this.HisNode, this.HisNode.HisWork, "@WorkID=" + this.OID + "@FID=" + this.FID);
        }
        /// <summary>
        /// 直接的保存
        /// </summary>
        public new void DirectSave()
        {
            this.beforeUpdateInsertAction();
            if (this.DirectUpdate() == 0)
                this.DirectInsert();
        }
        public string NodeFrmID = "";
        protected int _nodeID = 0;
        public int NodeID
        {
            get
            {
                if (_nodeID == 0)
                    throw new Exception("您没有给_Node给值。");
                return this._nodeID;
            }
            set
            {
                if (this._nodeID != value)
                {
                    this._nodeID = value;
                    this._enMap = null;
                }
                this._nodeID = value;
            }
        }
        /// <summary>
        /// 已经路过的节点
        /// </summary>
        public string HisPassedFrmIDs = "";
        #endregion
    }
    /// <summary>
    /// 工作 集合
    /// </summary>
    abstract public class Works : EntitiesOID
    {
        #region 构造方法
        /// <summary>
        /// 信息采集基类
        /// </summary>
        public Works()
        {
        }
        #endregion

        
    }
}
