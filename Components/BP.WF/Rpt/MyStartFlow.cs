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
    /// 我发起的流程
    /// </summary>
    public class MyStartFlowAttr : EntityNoNameAttr
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
    /// 我发起的流程
    /// </summary>
    public class MyStartFlow : EntityNoName
    {
        #region 报表权限控制方式
        /// <summary>
        /// 报表查看权限控制.
        /// </summary>
        public RightViewWay RightViewWay
        {
            get
            {
                return (RightViewWay)this.GetValIntByKey(MyStartFlowAttr.RightViewWay);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.RightViewWay, (int)value);
            }
        }
        /// <summary>
        /// 报表查看权限控制-数据
        /// </summary>
        public string RightViewTag
        {
            get
            {
                return this.GetValStringByKey(MyStartFlowAttr.RightViewTag);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.RightViewTag, value);
            }
        }
        /// <summary>
        /// 报表部门权限控制.
        /// </summary>
        public RightDeptWay RightDeptWay
        {
            get
            {
                return (RightDeptWay)this.GetValIntByKey(MyStartFlowAttr.RightDeptWay);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.RightDeptWay, (int)value);
            }
        }
        /// <summary>
        /// 报表部门权限控制-数据
        /// </summary>
        public string RightDeptTag
        {
            get
            {
                return this.GetValStringByKey(MyStartFlowAttr.RightDeptTag);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.RightDeptTag, value);
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
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MyStartFlowAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.PTable, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FK_Rpt
        {
            get
            {
                return "ND" + int.Parse( this.No) + "MyRpt";
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
        /// 我发起的流程
        /// </summary>
        public MyStartFlow()
        {
        }
        /// <summary>
        /// 我发起的流程
        /// </summary>
        /// <param name="no">映射编号</param>
        public MyStartFlow(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 访问权限控制.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                uac.IsDelete = false;
                return uac;
            }
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

                Map map = new Map("WF_Flow", "我发起的流程");

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("4");;

                map.AddTBStringPK(MyStartFlowAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(MyStartFlowAttr.Name, null, "名称", true, true, 0, 200, 20);
                map.AddTBString(MyStartFlowAttr.PTable, null, "物理表", true, true, 0, 500, 20);
                map.AddTBString(MyStartFlowAttr.Note, null, "备注", true, true, 0, 500, 20, true);

                #region 我发起的流程.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoS2_ColsChose()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoS4_ColsOrder()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoS5_SearchCond()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                
                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoS8_RptExportTemplate()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.Warning = "您确定要执行吗?";
                rm.ClassMethodName = this.ToString() + ".DoReset()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                #endregion 我发起的流程.

                #region 查询与分析.
                rm = new RefMethod();
                rm.Title = "查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "查询与分析";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自定义查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/SQL.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchAdv()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "查询与分析";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "分组分析";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoGroup()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "../../WF/Img/Group.gif";
                rm.GroupName = "查询与分析";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "交叉报表(实验中)";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/D3.png";
                rm.ClassMethodName = this.ToString() + ".DoD3()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "查询与分析";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "对比分析(实验中)";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Contrast.png";
                rm.ClassMethodName = this.ToString() + ".DoContrast()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "查询与分析";
                map.AddRefMethod(rm);

                #endregion 查询与分析.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 映射方法.
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoS2_ColsChose()
        {
            string url = "/WF/Admin/FoolFormDesigner/Rpt/S2ColsChose.htm?FK_MapData="+this.FK_Rpt+"MyRpt&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
            return url;
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoS4_ColsOrder()
        {
            string url = "/WF/Admin/FoolFormDesigner/Rpt/S3ColsLabel.htm?FK_MapData="+this.FK_Rpt+"MyRpt&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
            return url;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoS5_SearchCond()
        {
            string url = "/WF/Admin/FoolFormDesigner/Rpt/S5SearchCond.htm?FK_MapData="+this.FK_Rpt+"&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
            return url;
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoS8_RptExportTemplate()
        {
            string url = "/WF/Admin/FoolFormDesigner/Rpt/S8_RptExportTemplate.aspx?FK_MapData="+this.FK_Rpt+"&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
            return url;
        }
        #endregion 映射方法.

        #region 查询与分析
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoSearch()
        {
            return "../../Rpt/Search.aspx?FK_MapData="+this.FK_Rpt+"&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
        }
        /// <summary>
        /// 高级查询
        /// </summary>
        /// <returns></returns>
        public string DoSearchAdv()
        {
            return "../../Rpt/SearchAdv.aspx?FK_MapData=ND" + int.Parse( this.No) + "MyRpt&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
        }
        /// <summary>
        /// 高级分析
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            return "../../Rpt/Group.aspx?FK_MapData=ND" + int.Parse( this.No) + "MyRpt&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
        }
        /// <summary>
        /// 交叉分析
        /// </summary>
        /// <returns></returns>
        public string DoD3()
        {
            return "../../Rpt/D3.aspx?FK_MapData="+this.FK_Rpt+"&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
        }
        /// <summary>
        /// 对比分析
        /// </summary>
        /// <returns></returns>
        public string DoContrast()
        {
            return "../../Rpt/Contrast.aspx?FK_MapData=" + this.FK_Rpt + "&FK_Flow=" + this.No + "&RptNo="+this.FK_Rpt;
        }
        #endregion

        #region 其他方法.
        /// <summary>
        /// 显示的列.
        /// </summary>
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
            this.DoReset();
            return base.beforeInsert();
        }
        /// <summary>
        /// 重置设置.
        /// </summary>
        public void DoReset()
        {
            MapData md = new MapData( "ND" + int.Parse( this.No) + "MyRpt");
            md.RptIsSearchKey = true;
            md.RptDTSearchWay = DTSearchWay.None;
            md.RptDTSearchKey = "";
            md.RptSearchKeys = "*FK_Dept*WFSta*FK_NY*";

            Flow fl = new Flow(this.No);
            this.PTable = fl.PTable;
            this.Update();

            string keys = "'OID','FK_Dept','FlowStarter','WFState','Title','FlowStartRDT','FlowEmps','FlowDaySpan','FlowEnder','FlowEnderRDT','FK_NY','FlowEndNode','WFSta'";
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.No) + "Rpt");

            attrs.Delete(MapAttrAttr.FK_MapData, this.No); // 删除已经有的字段。
            foreach (MapAttr attr in attrs)
            {
                if (keys.Contains("'" + attr.KeyOfEn + "'") == false)
                    continue;
                attr.FK_MapData = this.No;
                attr.Insert();
            }
        }
        /// <summary>
        /// 删除之前.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Delete(MapAttrAttr.FK_MapData, this.No);
            return base.beforeDelete();
        }
        #endregion 其他方法.

    }
    /// <summary>
    /// 我发起的流程s
    /// </summary>
    public class MyStartFlows : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 我发起的流程s
        /// </summary>
        public MyStartFlows()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MyStartFlow();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyStartFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyStartFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyStartFlow> Tolist()
        {
            System.Collections.Generic.List<MyStartFlow> list = new System.Collections.Generic.List<MyStartFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyStartFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
