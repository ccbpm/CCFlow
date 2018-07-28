using System;
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
        /// 完成任务时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 记录时间
        /// </summary>
        public const string RDT = "RDT";
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
        /// MyNum
        /// </summary>
        public const string MyNum = "MyNum";
        /// <summary>
        /// MD5
        /// </summary>
        public const string MD5 = "MD5";
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
        /// 完成时间
        /// </summary>
        public string CDT
        {
            get
            {
                string str = this.GetValStringByKey(WorkAttr.CDT);
                if (str.Length < 5)
                    this.SetValByKey(WorkAttr.CDT, DataType.CurrentDataTime);

                return this.GetValStringByKey(WorkAttr.CDT);
            }
        }
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
        /// 记录时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(WorkAttr.RDT);
            }
        }
        public string RDT_Date
        {
            get
            {
                try
                {
                    return DataType.ParseSysDate2DateTime(this.RDT).ToString(DataType.SysDataFormat);
                }
                catch
                {
                    return DataType.CurrentData;
                }
            }
        }
        public DateTime RDT_DateTime
        {
            get
            {
                try
                {
                    return DataType.ParseSysDate2DateTime(this.RDT_Date);
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }
        public string Record_FK_NY
        {
            get
            {
                return this.RDT.Substring(0, 7);
            }
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
        /// <summary>
        /// 记录人名称
        /// </summary>
        public string RecText
        {
            get
            {
                try
                {
                    return this.HisRec.Name;
                }
                catch
                {
                    return this.Rec;
                }
            }
            set
            {
                this.SetValByKey("RecText", value);
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

        #region 扩展属性
        /// <summary>
        /// 跨度天数
        /// </summary>
        public int SpanDays
        {
            get
            {
                if (this.CDT == this.RDT)
                    return 0;
                return DataType.SpanDays(this.RDT, this.CDT);
            }
        }
        /// <summary>
        /// 得到从工作完成到现在的日期
        /// </summary>
        /// <returns></returns>
        public int GetCDTimeLimits(string todata)
        {
            return DataType.SpanDays(this.CDT, todata);
        }
        /// <summary>
        /// 他的记录人
        /// </summary>
        public Emp HisRec
        {
            get
            {
              //  return new Emp(this.Rec);
                Emp emp = this.GetValByKey("HisRec"+this.Rec) as Emp;
                if (emp == null)
                {
                    emp = new Emp(this.Rec);
                    this.SetValByKey("HisRec" + this.Rec, emp);
                }
                return emp;
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

        #region 需要子类写的方法
        /// <summary>
        /// 自动填充
        /// </summary>
        /// <param name="attr"></param>
        public void DoAutoFull(Attr attr)
        {
            if (this.OID == 0)
                return;

            if (attr.AutoFullDoc == null || attr.AutoFullDoc.Length == 0)
                return;

            string objval = null;

            // 这个代码需要提纯到基类中去。
            switch (attr.AutoFullWay)
            {
                case BP.En.AutoFullWay.Way0:
                    return;
                case BP.En.AutoFullWay.Way1_JS:
                    break;
                case BP.En.AutoFullWay.Way2_SQL:
                    string sql = attr.AutoFullDoc;
                    Attrs attrs1 = this.EnMap.Attrs;
                    foreach (Attr a1 in attrs1)
                    {
                        if (a1.IsNum)
                            sql = sql.Replace("@" + a1.Key, this.GetValStringByKey(a1.Key));
                        else
                            sql = sql.Replace("@" + a1.Key, "'" + this.GetValStringByKey(a1.Key) + "'");
                    }

                    objval = DBAccess.RunSQLReturnString(sql);
                    break;
                case BP.En.AutoFullWay.Way3_FK:
                    try
                    {
                        string sqlfk = "SELECT @Field FROM @Table WHERE No=@AttrKey";
                        string[] strsFK = attr.AutoFullDoc.Split('@');
                        foreach (string str in strsFK)
                        {
                            if (str == null || str.Length == 0)
                                continue;

                            string[] ss = str.Split('=');
                            if (ss[0] == "AttrKey")
                                sqlfk = sqlfk.Replace('@' + ss[0], "'" + this.GetValStringByKey(ss[1]) + "'");
                            else
                                sqlfk = sqlfk.Replace('@' + ss[0], ss[1]);
                        }
                        sqlfk = sqlfk.Replace("''", "'");

                        objval = DBAccess.RunSQLReturnString(sqlfk);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@在处理自动完成：外键[" + attr.Key + ";" + attr.Desc + "],时出现错误。异常信息：" + ex.Message);
                    }
                    break;
                case BP.En.AutoFullWay.Way4_Dtl:
                    string mysql = "SELECT @Way(@Field) FROM @Table WHERE RefPK='"+ this.OID+"'";
                    string[] strs = attr.AutoFullDoc.Split('@');
                    foreach (string str in strs)
                    {
                        if (str == null || str.Length == 0)
                            continue;

                        string[] ss = str.Split('=');
                        mysql = mysql.Replace('@' + ss[0], ss[1]);
                    }
                    objval = DBAccess.RunSQLReturnString(mysql);
                    break;
                default:
                    throw new Exception("未涉及到的类型。");
            }
            if (objval == null)
                return;

            if (attr.IsNum)
            {
                try
                {
                    decimal d = decimal.Parse(objval);
                    this.SetValByKey(attr.Key, objval);
                }
                catch
                {
                }
            }
            else
            {
                this.SetValByKey(attr.Key, objval);
            }
            return;
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
                if (attr.Key == WorkAttr.CDT
                   || attr.Key == WorkAttr.RDT
                   || attr.Key == WorkAttr.Rec
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
                if (attr.Key == WorkAttr.CDT
                    || attr.Key == WorkAttr.RDT
                    || attr.Key == WorkAttr.Rec
                    || attr.Key == WorkAttr.FID
                    || attr.Key == WorkAttr.OID
                    || attr.Key == WorkAttr.Emps
                    || attr.Key == "No"
                    || attr.Key == "Name")
                    continue;
                this.SetValByKey(attr.Key, fromEn.GetValByKey(attr.Key));
            }
        }
        /// <summary>
        /// 删除主表数据也要删除它的明细数据
        /// </summary>
        protected override void afterDelete()
        {
            #warning 删除了明细，有可能造成其他的影响.
            //MapDtls dtls = this.HisNode.MapData.MapDtls;
            //foreach (MapDtl dtl in dtls)。
            //    DBAccess.RunSQL("DELETE FROM  " + dtl.PTable + " WHERE RefPK=" + this.OID);

            base.afterDelete();
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 更新之前
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            #region 特殊处理
            try
            {
                if (this.GetValStrByKey("WFState") == "Runing")
                {
                    this.SetValByKey("WFState", (int)WFState.Runing);
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            return base.beforeUpdate();
        }
        /// <summary>
        /// 直接的保存前要做的工作
        /// </summary>
        public virtual void BeforeSave()
        {
            //执行自动计算.
            this.AutoFull();
            // 执行保存前的事件。

            this.HisNode.HisFlow.DoFlowEventEntity(EventListOfNode.SaveBefore, this.HisNode, this.HisNode.HisWork, null);
        }
        /// <summary>
        /// 直接的保存
        /// </summary>
        public new void DirectSave()
        {
            this.beforeUpdateInsertAction();
            if (this.DirectUpdate() == 0)
            {
                this.SetValByKey(WorkAttr.RDT, DateTime.Now.ToString("yyyy-MM-dd"));
                this.DirectInsert();
            }
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

        #region 查询方法
        /// <summary>
        /// 查询工作(不适合审核节点查询)
        /// </summary>
        /// <param name="empId">工作人员</param>
        /// <param name="nodeStat">节点状态</param>
        /// <param name="fromdate">记录日期从</param>
        /// <param name="todate">记录日期到</param>
        /// <returns></returns>
        public int Retrieve(string key, string empId, string fromdate, string todate)
        {
            QueryObject qo = new QueryObject(this);
                qo.AddWhere(WorkAttr.Rec, empId);

            qo.addAnd();
            qo.AddWhere(WorkAttr.RDT, ">=", fromdate);
            qo.addAnd();
            qo.AddWhere(WorkAttr.RDT, "<=", todate);

            if (key.Trim().Length == 0)
                return qo.DoQuery();
            else
            {
                if (key.IndexOf("%") == -1)
                    key = "%" + key + "%";
                Entity en = this.GetNewEntity;
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(en.PK, " LIKE ", key);
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;
                    if (attr.UIContralType == UIContralType.DDL || attr.UIContralType == UIContralType.CheckBok)
                        continue;
                    qo.addOr();
                    qo.AddWhere(attr.Key, " LIKE ", key);
                }
                qo.addRightBracket();
                return qo.DoQuery();
            }
        }
        public int Retrieve(string fromDataTime, string toDataTime)
        {
            QueryObject qo = new QueryObject(this);
            qo.Top = 90000;
            qo.AddWhere(WorkAttr.RDT, " >=", fromDataTime);
            qo.addAnd();
            qo.AddWhere(WorkAttr.RDT, " <= ", toDataTime);
            return qo.DoQuery();
        }
        #endregion
    }
}
