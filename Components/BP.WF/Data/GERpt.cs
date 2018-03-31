using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    ///  属性
    /// </summary>
    public class GERptAttr
    {
        /// <summary>
        /// 工作实例ID
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 参与人员
        /// </summary>
        public const string FlowEmps = "FlowEmps";
        /// <summary>
        /// 发起年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 发起人编号
        /// </summary>
        public const string FlowStarter = "FlowStarter";
        /// <summary>
        /// 发起时间
        /// </summary>
        public const string FlowStartRDT = "FlowStartRDT";
        /// <summary>
        /// 发起人部门编号
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程状态(详细状态)
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 流程状态(概要状态)
        /// </summary>
        public const string WFSta = "WFSta";       
        /// <summary>
        /// 结束人
        /// </summary>
        public const string FlowEnder = "FlowEnder";
        /// <summary>
        /// 最后处理时间
        /// </summary>
        public const string FlowEnderRDT = "FlowEnderRDT";
        /// <summary>
        /// 跨度
        /// </summary>
        public const string FlowDaySpan = "FlowDaySpan";
        /// <summary>
        /// 停留节点
        /// </summary>
        public const string FlowEndNode = "FlowEndNode";
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// PFID
        /// </summary>
        public const string PFID = "PFID";
        /// <summary>
        /// 父流程节点发送
        /// </summary>
        public const string PNodeID = "PNodeID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 调用父流程的工作人员
        /// </summary>
        public const string PEmp = "PEmp";
        /// <summary>
        /// 客户编号
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 客户名称
        /// </summary>
        public const string GuestName = "GuestName";
        /// <summary>
        /// 单据编号
        /// </summary>
        public const string BillNo = "BillNo";
        /// <summary>
        /// 流程备注
        /// </summary>
        public const string FlowNote = "FlowNote";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 项目编号
        /// </summary>
        public const string PrjNo = "PrjNo";
        /// <summary>
        /// 项目名称
        /// </summary>
        public const string PrjName = "PrjName";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 数量
        /// </summary>
        public const string MyNum = "MyNum";
    }
    /// <summary>
    /// 报表
    /// </summary>
    public class GERpt : BP.En.EntityOID
    {
        #region attrs
        public new Int64 OID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.OID);
            }
            set
            {
                this.SetValByKey(GERptAttr.OID, value);
            }
        }
        /// <summary>
        /// 流程时间跨度
        /// </summary>
        public float FlowDaySpan
        {
            get
            {
                return this.GetValFloatByKey(GERptAttr.FlowDaySpan);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowDaySpan, value);
            }
        }
        public int MyNum
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.MyNum);
            }
            set
            {
                this.SetValByKey(GERptAttr.MyNum, value);
            }
        }
        /// <summary>
        /// 主流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.FID);
            }
            set
            {
                this.SetValByKey(GERptAttr.FID, value);
            }
        }
        public string GUID
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.GUID);
            }
            set
            {
                this.SetValByKey(GERptAttr.GUID, value);
            }
        }
        /// <summary>
        /// 流程参与人员
        /// </summary>
        public string FlowEmps
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEmps);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEmps, value);
            }
        }
        /// <summary>
        /// 流程备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowNote);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.GuestNo);
            }
            set
            {
                this.SetValByKey(GERptAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.GuestName);
            }
            set
            {
                this.SetValByKey(GERptAttr.GuestName, value);
            }
        }
        public string BillNo
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.BillNo);
            }
            set
            {
                this.SetValByKey(GERptAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程发起人
        /// </summary>
        public string FlowStarter
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowStarter);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowStarter, value);
            }
        }
        /// <summary>
        /// 流程发起时间
        /// </summary>
        public string FlowStartRDT
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowStartRDT);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowStartRDT, value);
            }
        }
        /// <summary>
        /// 流程结束者
        /// </summary>
        public string FlowEnder
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEnder);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEnder, value);
            }
        }
        /// <summary>
        /// 流程最后处理时间
        /// </summary>
        public string FlowEnderRDT
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEnderRDT);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEnderRDT, value);
            }
        }
        public string FlowEndNodeText
        {
            get
            {
                Node nd =new Node(this.FlowEndNode);
                return nd.Name;
            }
        }
        public int FlowEndNode
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.FlowEndNode);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEndNode, value);
            }
        }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.Title);
            }
            set
            {
                this.SetValByKey(GERptAttr.Title, value);
            }
        }
        /// <summary>
        /// 隶属年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(GERptAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 发起人部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(GERptAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(GERptAttr.WFState);
            }
            set
            {
                //设置他的值.
                this.SetValByKey(GERptAttr.WFState,(int)value);

                // 设置 WFSta 的值.
                switch (value)
                {
                    case WF.WFState.Complete:
                        SetValByKey(BP.WF.GenerWorkFlowAttr.WFSta, (int)WFSta.Complete);
                        break;
                    case WF.WFState.Delete:
                    case WF.WFState.Blank:
                        SetValByKey(BP.WF.GenerWorkFlowAttr.WFSta, (int)WFSta.Etc);
                        break;
                    default:
                        SetValByKey(BP.WF.GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                        break;
                }

            }
        }
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(GERptAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 发出的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.PNodeID);
            }
            set
            {
                this.SetValByKey(GERptAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// 父流程流程编号
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(GERptAttr.PFlowNo, value);
            }
        }
        public string PEmp
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.PEmp);
            }
            set
            {
                this.SetValByKey(GERptAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 项目编号
        /// </summary>
        public string PrjNo
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.PrjNo);
            }
            set
            {
                this.SetValByKey(GERptAttr.PrjNo, value);
            }
        }
        public string PrjName
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.PrjName);
            }
            set
            {
                this.SetValByKey(GERptAttr.PrjName, value);
            }
        }
        #endregion attrs

        #region 重写。

        public override void Copy(System.Data. DataRow dr)
        {
            foreach (Attr attr in this.EnMap.Attrs)
            {
                if (attr.Key == WorkAttr.CDT
                      || attr.Key == WorkAttr.RDT
                      || attr.Key == WorkAttr.Rec
                      || attr.Key == WorkAttr.FID
                      || attr.Key == WorkAttr.OID
                      || attr.Key == WorkAttr.Emps
                      || attr.Key == GERptAttr.AtPara
                      || attr.Key == GERptAttr.BillNo
                      //|| attr.Key == GERptAttr.CFlowNo
                      //|| attr.Key == GERptAttr.CWorkID
                      || attr.Key == GERptAttr.FID
                      || attr.Key == GERptAttr.FK_Dept
                      || attr.Key == GERptAttr.FK_NY
                      || attr.Key == GERptAttr.FlowDaySpan
                      || attr.Key == GERptAttr.FlowEmps
                      || attr.Key == GERptAttr.FlowEnder
                      || attr.Key == GERptAttr.FlowEnderRDT
                      || attr.Key == GERptAttr.FlowEndNode
                      || attr.Key == GERptAttr.FlowNote
                      || attr.Key == GERptAttr.FlowStarter
                      || attr.Key == GERptAttr.GuestName
                      || attr.Key == GERptAttr.GuestNo
                      || attr.Key == GERptAttr.GUID
                      || attr.Key == GERptAttr.PEmp
                      || attr.Key == GERptAttr.PFlowNo
                      || attr.Key == GERptAttr.PNodeID
                      || attr.Key == GERptAttr.PWorkID
                      || attr.Key == GERptAttr.Title
                      || attr.Key == GERptAttr.PrjNo
                      || attr.Key == GERptAttr.PrjName
                      || attr.Key.Equals("No")
                      || attr.Key.Equals("Name"))
                    continue;

                if (attr.Key == GERptAttr.MyNum)
                {
                    this.SetValByKey(attr.Key, 1);
                    continue;
                }

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
                    || attr.Key == GERptAttr.AtPara
                    || attr.Key == GERptAttr.BillNo
                    //|| attr.Key == GERptAttr.CFlowNo
                    //|| attr.Key == GERptAttr.CWorkID
                    || attr.Key == GERptAttr.FID
                    || attr.Key == GERptAttr.FK_Dept
                    || attr.Key == GERptAttr.FK_NY
                    || attr.Key == GERptAttr.FlowDaySpan
                    || attr.Key == GERptAttr.FlowEmps
                    || attr.Key == GERptAttr.FlowEnder
                    || attr.Key == GERptAttr.FlowEnderRDT
                    || attr.Key == GERptAttr.FlowEndNode
                    || attr.Key == GERptAttr.FlowNote
                    || attr.Key == GERptAttr.FlowStarter
                    || attr.Key == GERptAttr.GuestName
                    || attr.Key == GERptAttr.GuestNo
                    || attr.Key == GERptAttr.GUID
                    || attr.Key == GERptAttr.PEmp
                    || attr.Key == GERptAttr.PFlowNo
                    || attr.Key == GERptAttr.PNodeID
                    || attr.Key == GERptAttr.PWorkID
                    || attr.Key == GERptAttr.Title
                    || attr.Key == GERptAttr.PrjNo
                    || attr.Key == GERptAttr.PrjName
                    || attr.Key == "No"
                    || attr.Key == "Name")
                    continue;


                if (attr.Key== GERptAttr.MyNum)
                {
                    this.SetValByKey(attr.Key, 1);
                    continue;
                }
                this.SetValByKey(attr.Key, fromEn.GetValByKey(attr.Key));
            }
        }
        #endregion

        #region 属性.
        private string _RptName = null;
        public string RptName
        {
            get
            {
                return _RptName;
            }
            set
            {
                this._RptName = value;

                this._SQLCash = null;
                this._enMap = null;
                this.Row = null;
            }
        }
        public override string ToString()
        {
            return RptName;
        }
        public override string PK
        {
            get
            {
                return "OID";
            }
        }
        public override string PKField
        {
            get
            {
                return "OID";
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this.RptName == null)
                {
                    BP.Port.Emp emp = new BP.Port.Emp();
                    return emp.EnMap;
                }

                if (this._enMap == null)
                    this._enMap = MapData.GenerHisMap(this.RptName);

                return this._enMap;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="rptName"></param>
        public GERpt(string rptName)
        {
            this.RptName = rptName;
        }
        public GERpt()
        {
        }
        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="rptName"></param>
        /// <param name="oid"></param>
        public GERpt(string rptName, Int64 oid)
        {
            this.RptName = rptName;
            this.OID = (int)oid;
            this.Retrieve();
        }
        #endregion attrs
    }
    /// <summary>
    /// 报表s
    /// </summary>
    public class GERpts : BP.En.EntitiesOID
    {
        /// <summary>
        /// 报表s
        /// </summary>
        public GERpts()
        {
        }

        /// <summary>
        /// 获得一个实例.
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GERpt();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GERpt> ToJavaList()
        {
            return (System.Collections.Generic.IList<GERpt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GERpt> Tolist()
        {
            System.Collections.Generic.List<GERpt> list = new System.Collections.Generic.List<GERpt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GERpt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
