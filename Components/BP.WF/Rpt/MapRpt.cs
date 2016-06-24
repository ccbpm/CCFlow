using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.WF;
using BP.Sys;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表查看权限控制方式
    /// </summary>
    public enum RightViewWay
    {
        /// <summary>
        /// 任何人都可以查看
        /// </summary>
        AnyOne,
        /// <summary>
        /// 按照组织结构权限
        /// </summary>
        ByPort,
        /// <summary>
        /// 按照SQL控制
        /// </summary>
        BySQL
    }
    /// <summary>
    /// 部门数据权限控制方式
    /// </summary>
    public enum RightDeptWay
    {
        /// <summary>
        /// 所有部门的数据.
        /// </summary>
        All,
        /// <summary>
        /// 本部门的数据.
        /// </summary>
        SelfDept,
        /// <summary>
        /// 本部门与子部门的数据.
        /// </summary>
        SelfDeptAndSubDepts,
        /// <summary>
        /// 指定部门的数据.
        /// </summary>
        SpecDepts
    }

    /// <summary>
    /// 报表设计
    /// </summary>
    public class MapRptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 查询的物理表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";

        #region 权限控制 2014-12-18
        /// <summary>
        /// 报表查看权限控制方式
        /// </summary>
        public const string RightViewWay = "RightViewWay";
        /// <summary>
        /// 数据存储
        /// </summary>
        public const string RightViewTag = "RightViewTag";
        /// <summary>
        /// 部门数据权限控制方式
        /// </summary>
        public const string RightDeptWay = "RightDeptWay";
        /// <summary>
        /// 数据存储
        /// </summary>
        public const string RightDeptTag = "RightDeptTag";
        #endregion 权限控制
    }
    /// <summary>
    /// 报表设计
    /// </summary>
    public class MapRpt : EntityNoName
    {
        #region 报表权限控制方式
        /// <summary>
        /// 报表查看权限控制.
        /// </summary>
        public RightViewWay RightViewWay
        {
            get
            {
                return (RightViewWay)this.GetValIntByKey(MapRptAttr.RightViewWay);
            }
            set
            {
                this.SetValByKey(MapRptAttr.RightViewWay, (int)value);
            }
        }
        /// <summary>
        /// 报表查看权限控制-数据
        /// </summary>
        public string RightViewTag
        {
            get
            {
                return this.GetValStringByKey(MapRptAttr.RightViewTag);
            }
            set
            {
                this.SetValByKey(MapRptAttr.RightViewTag, value);
            }
        }
        /// <summary>
        /// 报表部门权限控制.
        /// </summary>
        public RightDeptWay RightDeptWay
        {
            get
            {
                return (RightDeptWay)this.GetValIntByKey(MapRptAttr.RightDeptWay);
            }
            set
            {
                this.SetValByKey(MapRptAttr.RightDeptWay, (int)value);
            }
        }
        /// <summary>
        /// 报表部门权限控制-数据
        /// </summary>
        public string RightDeptTag
        {
            get
            {
                return this.GetValStringByKey(MapRptAttr.RightDeptTag);
            }
            set
            {
                this.SetValByKey(MapRptAttr.RightDeptTag, value);
            }
        }
        #endregion 报表权限控制方式

        #region 外键属性
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrames MapFrames
        {
            get
            {
                MapFrames obj = this.GetRefObject("MapFrames") as MapFrames;
                if (obj == null)
                {
                    obj = new MapFrames(this.No);
                    this.SetRefObject("MapFrames", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public GroupFields GroupFields
        {
            get
            {
                GroupFields obj = this.GetRefObject("GroupFields") as GroupFields;
                if (obj == null)
                {
                    obj = new GroupFields(this.No);
                    this.SetRefObject("GroupFields", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 逻辑扩展
        /// </summary>
        public MapExts MapExts
        {
            get
            {
                MapExts obj = this.GetRefObject("MapExts") as MapExts;
                if (obj == null)
                {
                    obj = new MapExts(this.No);
                    this.SetRefObject("MapExts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                FrmEvents obj = this.GetRefObject("FrmEvents") as FrmEvents;
                if (obj == null)
                {
                    obj = new FrmEvents(this.No);
                    this.SetRefObject("FrmEvents", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 一对多
        /// </summary>
        public MapM2Ms MapM2Ms
        {
            get
            {
                MapM2Ms obj = this.GetRefObject("MapM2Ms") as MapM2Ms;
                if (obj == null)
                {
                    obj = new MapM2Ms(this.No);
                    this.SetRefObject("MapM2Ms", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 从表
        /// </summary>
        public MapDtls MapDtls
        {
            get
            {
                MapDtls obj = this.GetRefObject("MapDtls") as MapDtls;
                if (obj == null)
                {
                    obj = new MapDtls(this.No);
                    this.SetRefObject("MapDtls", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public FrmLinks FrmLinks
        {
            get
            {
                FrmLinks obj = this.GetRefObject("FrmLinks") as FrmLinks;
                if (obj == null)
                {
                    obj = new FrmLinks(this.No);
                    this.SetRefObject("FrmLinks", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtns FrmBtns
        {
            get
            {
                FrmBtns obj = this.GetRefObject("FrmLinks") as FrmBtns;
                if (obj == null)
                {
                    obj = new FrmBtns(this.No);
                    this.SetRefObject("FrmBtns", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 元素
        /// </summary>
        public FrmEles FrmEles
        {
            get
            {
                FrmEles obj = this.GetRefObject("FrmEles") as FrmEles;
                if (obj == null)
                {
                    obj = new FrmEles(this.No);
                    this.SetRefObject("FrmEles", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 线
        /// </summary>
        public FrmLines FrmLines
        {
            get
            {
                FrmLines obj = this.GetRefObject("FrmLines") as FrmLines;
                if (obj == null)
                {
                    obj = new FrmLines(this.No);
                    this.SetRefObject("FrmLines", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public FrmLabs FrmLabs
        {
            get
            {
                FrmLabs obj = this.GetRefObject("FrmLabs") as FrmLabs;
                if (obj == null)
                {
                    obj = new FrmLabs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public FrmImgs FrmImgs
        {
            get
            {
                FrmImgs obj = this.GetRefObject("FrmLabs") as FrmImgs;
                if (obj == null)
                {
                    obj = new FrmImgs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachments FrmAttachments
        {
            get
            {
                FrmAttachments obj = this.GetRefObject("FrmAttachments") as FrmAttachments;
                if (obj == null)
                {
                    obj = new FrmAttachments(this.No);
                    this.SetRefObject("FrmAttachments", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAths FrmImgAths
        {
            get
            {
                FrmImgAths obj = this.GetRefObject("FrmImgAths") as FrmImgAths;
                if (obj == null)
                {
                    obj = new FrmImgAths(this.No);
                    this.SetRefObject("FrmImgAths", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 单选按钮
        /// </summary>
        public FrmRBs FrmRBs
        {
            get
            {
                FrmRBs obj = this.GetRefObject("FrmRBs") as FrmRBs;
                if (obj == null)
                {
                    obj = new FrmRBs(this.No);
                    this.SetRefObject("FrmRBs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public MapAttrs MapAttrs
        {
            get
            {
                MapAttrs obj = this.GetRefObject("MapAttrs") as MapAttrs;
                if (obj == null)
                {
                    obj = new MapAttrs(this.No);
                    this.SetRefObject("MapAttrs", obj);
                }
                return obj;
            }
        }
        #endregion

        #region 属性
        public string FK_Flow
        {
            get
            {
               return this.GetValStrByKey(MapRptAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(MapRptAttr.FK_Flow, value);
            }
        }
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapRptAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapRptAttr.PTable, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(MapRptAttr.Note);
            }
            set
            {
                this.SetValByKey(MapRptAttr.Note, value);
            }
        }
        public Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    _HisEns = BP.En.ClassFactory.GetEns(this.No);
                }
                return _HisEns;
            }
        }
        public Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        #endregion

        #region 构造方法
        private GEEntity _HisEn = null;
        public GEEntity HisGEEn
        {
            get
            {
                if (this._HisEn == null)
                    _HisEn = new GEEntity(this.No);
                return _HisEn;
            }
        }
        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public GEEntity GenerGEEntityByDataSet(DataSet ds)
        {
            // New 它的实例.
            GEEntity en = this.HisGEEn;

            // 它的table.
            DataTable dt = ds.Tables[this.No];

            //装载数据.
            en.Row.LoadDataTable(dt, dt.Rows[0]);

            // dtls.
            MapDtls dtls = this.MapDtls;
            foreach (MapDtl item in dtls)
            {
                DataTable dtDtls = ds.Tables[item.No];
                GEDtls dtlsEn = new GEDtls(item.No);
                foreach (DataRow dr in dtDtls.Rows)
                {
                    // 产生它的Entity data.
                    GEDtl dtl = (GEDtl)dtlsEn.GetNewEntity;
                    dtl.Row.LoadDataTable(dtDtls, dr);

                    //加入这个集合.
                    dtlsEn.AddEntity(dtl);
                }

                //加入到他的集合里.
                en.Dtls.Add(dtDtls);
            }
            return en;
        }
        /// <summary>
        /// 报表设计
        /// </summary>
        public MapRpt()
        {
        }
        /// <summary>
        /// 报表设计
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapRpt(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_MapData", "报表设计");

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("4");;

                map.AddTBStringPK(MapRptAttr.No, null, "编号", true, false, 1, 200, 20);
                map.AddTBString(MapRptAttr.Name, null, "描述", true, false, 0, 500, 20);
           //     map.AddTBString(MapRptAttr.SearchKeys, null, "查询键", true, false, 0, 500, 20);
                map.AddTBString(MapRptAttr.PTable, null, "物理表", true, false, 0, 500, 20);
                map.AddTBString(MapRptAttr.FK_Flow, null, "流程编号", true, false, 0, 3, 3);

                //Tag
             //   map.AddTBString(MapRptAttr.Tag, null, "Tag", true, false, 0, 500, 20);

                //时间查询:用于报表查询.
              //  map.AddTBInt(MapRptAttr.IsSearchKey, 0, "是否需要关键字查询", true, false);
             //   map.AddTBInt(MapRptAttr.DTSearchWay, 0, "时间查询方式", true, false);
             //   map.AddTBString(MapRptAttr.DTSearchKey, null, "时间查询字段", true, false, 0, 200, 20);
                map.AddTBString(MapRptAttr.Note, null, "备注", true, false, 0, 500, 20);
            

                #region 权限控制. 2014-12-18
                map.AddTBInt(MapRptAttr.RightViewWay, 0, "报表查看权限控制方式", true, false);
                map.AddTBString(MapRptAttr.RightViewTag, null, "报表查看权限控制Tag", true, false, 0, 4000, 20);

                map.AddTBInt(MapRptAttr.RightDeptWay, 0, "部门数据查看控制方式", true, false);
                map.AddTBString(MapRptAttr.RightDeptTag, null, "部门数据查看控制Tag", true, false, 0, 4000, 20);

                map.AttrsOfOneVSM.Add(new RptStations(), new Stations(), RptStationAttr.FK_Rpt, RptStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "岗位权限");
                map.AttrsOfOneVSM.Add(new RptDepts(), new Depts(), RptDeptAttr.FK_Rpt, RptDeptAttr.FK_Dept,
                    DeptAttr.Name, DeptAttr.No, "部门权限");
                map.AttrsOfOneVSM.Add(new RptEmps(), new Emps(), RptEmpAttr.FK_Rpt, RptEmpAttr.FK_Emp,
                 DeptAttr.Name, DeptAttr.No, "人员权限");
                #endregion 权限控制.


                this._enMap = map;
                return this._enMap;
            }
        }

         
        #endregion

        public MapAttrs HisShowColsAttrs
        {
            get
            {
                MapAttrs mattrs = new MapAttrs(this.No);
                return mattrs;
            }
        }
        protected override bool beforeInsert()
        {
            this.ResetIt();
            return base.beforeInsert();
        }

        public void ResetIt()
        {
            MapData md = new MapData(this.No);
            md.RptIsSearchKey = true;
            md.RptDTSearchWay = DTSearchWay.None;
            md.RptDTSearchKey = "";
            md.RptSearchKeys = "*FK_Dept*WFSta*FK_NY*";

            Flow fl = new Flow(this.FK_Flow);
            this.PTable = fl.PTable;
            this.Update();

            string keys = "'OID','FK_Dept','FlowStarter','WFState','Title','FlowStartRDT','FlowEmps','FlowDaySpan','FlowEnder','FlowEnderRDT','FK_NY','FlowEndNode','WFSta'";
            MapAttrs attrs = new MapAttrs("ND"+int.Parse(this.FK_Flow)+"Rpt" );

            attrs.Delete(MapAttrAttr.FK_MapData, this.No); // 删除已经有的字段。
            foreach (MapAttr attr in attrs)
            {
                if (keys.Contains("'" + attr.KeyOfEn + "'") == false)
                    continue;
                attr.FK_MapData = this.No;
                attr.Insert();
            }
        }
        
        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Delete(MapAttrAttr.FK_MapData, this.No);
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 报表设计s
    /// </summary>
    public class MapRpts : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 报表设计s
        /// </summary>
        public MapRpts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapRpt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapRpt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapRpt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapRpt> Tolist()
        {
            System.Collections.Generic.List<MapRpt> list = new System.Collections.Generic.List<MapRpt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapRpt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
