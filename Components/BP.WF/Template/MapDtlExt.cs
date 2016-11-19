using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 明细
    /// </summary>
    public class MapDtlExt : EntityNoName
    {
        #region 导入导出属性.
        /// <summary>
        /// 用户访问.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 是否可以导出
        /// </summary>
        public bool IsExp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsExp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsExp, value);
            }
        }
        /// <summary>
        /// 是否可以导入？
        /// </summary>
        public bool IsImp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsImp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsImp, value);
            }
        }
        /// <summary>
        /// 是否启用选择数据项目导入？
        /// </summary>
        public bool IsEnableSelectImp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableSelectImp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableSelectImp, value);
            }
        }
        /// <summary>
        /// 查询sql
        /// </summary>
        public string ImpSQLInit
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLInit).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLInit, value);
            }
        }
        /// <summary>
        /// 搜索sql
        /// </summary>
        public string ImpSQLSearch
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLSearch).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLSearch, value);
            }
        }
        /// <summary>
        /// 填充数据
        /// </summary>
        public string ImpSQLFull
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLFull).Replace("~","'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLFull, value);
            }
        }
        #endregion

        #region 基本设置
        /// <summary>
        /// 工作模式
        /// </summary>
        public DtlModel DtlModel
        {
            get
            {
                return (DtlModel)this.GetValIntByKey(MapDtlAttr.Model);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.Model, (int)value);
            }
        }
        /// <summary>
        /// 是否启用行锁定.
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsRowLock, value);
            }
        }
        #endregion 基本设置 

        #region 参数属性
        /// <summary>
        /// 记录增加模式
        /// </summary>
        public DtlAddRecModel DtlAddRecModel
        {
            get
            {
                return (DtlAddRecModel)this.GetParaInt(MapDtlAttr.DtlAddRecModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlAddRecModel, (int)value);
            }
        }
        /// <summary>
        /// 保存方式
        /// </summary>
        public DtlSaveModel DtlSaveModel
        {
            get
            {
                return (DtlSaveModel)this.GetParaInt(MapDtlAttr.DtlSaveModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlSaveModel, (int)value);
            }
        }
        /// <summary>
        /// 是否启用Link,在记录的右边.
        /// </summary>
        public bool IsEnableLink
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsEnableLink,false);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsEnableLink, value);
            }
        }
        public string LinkLabel
        {
            get
            {
                string s= this.GetParaString(MapDtlAttr.LinkLabel);
                if (string.IsNullOrEmpty(s))
                    return "详细";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.LinkLabel, value);
            }
        }
        public string LinkUrl
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.LinkUrl);
                if (string.IsNullOrEmpty(s))
                    return "http://ccport.org";

                s = s.Replace("*", "@");
                return s;
            }
            set
            {
                string val = value;
                val = val.Replace("@", "*");
                this.SetPara(MapDtlAttr.LinkUrl, val);
            }
        }
        public string LinkTarget
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.LinkTarget);
                if (string.IsNullOrEmpty(s))
                    return "_blank";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.LinkTarget, value);
            }
        }
        /// <summary>
        /// 子线程处理人字段(用于分流节点的明细表分配子线程任务).
        /// </summary>
        public string SubThreadWorker
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadWorker);
                if (string.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadWorker, value);
            }
        }
        /// <summary>
        /// 子线程分组字段(用于分流节点的明细表分配子线程任务)
        /// </summary>
        public string SubThreadGroupMark
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadGroupMark);
                if (string.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadGroupMark, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MapDtlAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_Node, value);
            }
        }
        #endregion 参数属性

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
        public GroupFields GroupFields_del
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
        public GEDtls HisGEDtls_temp = null;
        public DtlShowModel HisDtlShowModel
        {
            get
            {
                return (DtlShowModel)this.GetValIntByKey(MapDtlAttr.DtlShowModel);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.DtlShowModel, (int)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public WhenOverSize HisWhenOverSize
        {
            get
            {
                return (WhenOverSize)this.GetValIntByKey(MapDtlAttr.WhenOverSize);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.WhenOverSize, (int)value);
            }
        }
        /// <summary>
        /// 是否显示数量
        /// </summary>
        public bool IsShowSum
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowSum);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowSum, value);
            }
        }
        /// <summary>
        /// 是否显示Idx
        /// </summary>
        public bool IsShowIdx
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowIdx);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowIdx, value);
            }
        }
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool IsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public bool IsHLDtl
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsHLDtl);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsHLDtl, value);
            }
        }
        /// <summary>
        /// 是否是分流
        /// </summary>
        public bool IsFLDtl
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsFLDtl);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsFLDtl, value);
            }
        }
        public int _IsReadonly = 2;
        public bool IsReadonly
        {
            get
            {
                if (_IsReadonly != 2)
                {
                    if (_IsReadonly == 1)
                        return true;
                    else
                        return false;
                }

                if (this.IsDelete || this.IsInsert || this.IsUpdate)
                {
                    _IsReadonly = 0;
                    return false;
                }
                _IsReadonly = 1;
                return true;
            }
        }
        /// <summary>
        /// 是否可以删除？
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsDelete, value);
            }
        }
        /// <summary>
        /// 是否可以新增？
        /// </summary>
        public bool IsInsert
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsInsert);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsInsert, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsView
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsView);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsView, value);
            }
        }
        /// <summary>
        /// 是否可以更新？
        /// </summary>
        public bool IsUpdate
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsUpdate);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsUpdate, value);
            }
        }
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public bool IsEnableAthM
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableAthM);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableAthM, value);
            }
        }
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        public bool IsEnableGroupField
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableGroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableGroupField, value);
            }
        }
        /// <summary>
        /// 是否起用审核连接
        /// </summary>
        public bool IsEnablePass
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnablePass);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnablePass, value);
            }
        }
      
        /// <summary>
        /// 是否copy数据？
        /// </summary>
        public bool IsCopyNDData
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsCopyNDData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsCopyNDData, value);
            }
        }
        /// <summary>
        /// 是否启用一对多
        /// </summary>
        public bool IsEnableM2M
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableM2M);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableM2M, value);
            }
        }
        /// <summary>
        /// 是否启用一对多多
        /// </summary>
        public bool IsEnableM2MM
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableM2MM);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableM2MM, value);
            }
        }

        public bool IsUse = false;
        /// <summary>
        /// 是否检查人员的权限
        /// </summary>
        public DtlOpenType DtlOpenType
        {
            get
            {
                return (DtlOpenType)this.GetValIntByKey(MapDtlAttr.DtlOpenType);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.DtlOpenType, (int)value);
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupField
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.GroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.GroupField, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int RowsOfList
        {
            get
            {
                //如果不可以插入，就让其返回0.
                if (this.IsInsert == false)
                    return 0;

                return this.GetValIntByKey(MapDtlAttr.RowsOfList);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.RowsOfList, value);
            }
        }
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.PTable);
                if (s == "" || s == null)
                {
                    s = this.No;
                    if (s.Substring(0, 1) == "0")
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
                else
                {
                    if (s.Substring(0, 1) == "0")
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
            }
            set
            {
                this.SetValByKey(MapDtlAttr.PTable, value);
            }
        }
        /// <summary>
        /// 多表头
        /// </summary>
        public string MTR
        {
            get
            {
                string s= this.GetValStrByKey(MapDtlAttr.MTR);
                s = s.Replace("《","<");
                s = s.Replace( "》",">");
                s = s.Replace("‘","'");
                return s;
            }
            set
            {
                string s = value;
                s = s.Replace("<","《");
                s = s.Replace(">", "》");
                s = s.Replace("'", "‘");
                this.SetValByKey(MapDtlAttr.MTR, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 明细
        /// </summary>
        public MapDtlExt()
        {
        }
        public MapDtlExt(string mypk)
        {
            this.No = mypk;
            this._IsReadonly = 2;
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
                Map map = new Map("Sys_MapDtl");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "明细";
                map.EnType = EnType.Sys;

                #region 基础信息.

                map.AddTBStringPK(MapDtlAttr.No, null, "编号", true, false, 1, 100, 20);
                map.AddTBString(MapDtlAttr.Name, null, "名称", true, false, 1, 200, 20);
                map.AddTBString(MapDtlAttr.FK_MapData, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(MapDtlAttr.PTable, null, "存储表", true, false, 0, 200, 20);

                map.AddDDLSysEnum(MapDtlAttr.Model, 0, "工作模式", true, true,MapDtlAttr.Model, "@0=普通@1=固定行");

                //map.AddTBString(MapDtlAttr.ImpFixTreeSql, null, "固定列树形SQL", true, false, 0, 500, 20);
                //map.AddTBString(MapDtlAttr.ImpFixDataSql, null, "固定列数据SQL", true, false, 0, 500, 20);

                map.AddTBInt(MapDtlAttr.RowsOfList, 6, "初始化行数", false, false);

                map.AddBoolean(MapDtlAttr.IsEnableGroupField, false, "是否启用分组字段", false, false);

                map.AddBoolean(MapDtlAttr.IsShowSum, true, "是否显示合计？", false, false);
                map.AddBoolean(MapDtlAttr.IsShowIdx, true, "是否显示序号？", false, false);
            
                map.AddBoolean(MapDtlAttr.IsReadonly, false, "是否只读？", false, false);
                map.AddBoolean(MapDtlAttr.IsShowTitle, true, "是否显示标题？", false, false);
                map.AddBoolean(MapDtlAttr.IsView, true, "是否可见？", false, false);


                map.AddBoolean(MapDtlAttr.IsInsert, true, "是否可以插入行？", false, false);
                map.AddBoolean(MapDtlAttr.IsDelete, true, "是否可以删除行？", false, false);
                map.AddBoolean(MapDtlAttr.IsUpdate, true, "是否可以更新？", false, false);

                map.AddBoolean(MapDtlAttr.IsEnablePass, false, "是否启用通过审核功能?", false, false);
                map.AddBoolean(MapDtlAttr.IsEnableAthM, false, "是否启用多附件", false, false);

                map.AddBoolean(MapDtlAttr.IsEnableM2M, false, "是否启用M2M", false, false);
                map.AddBoolean(MapDtlAttr.IsEnableM2MM, false, "是否启用M2M", false, false);

                map.AddDDLSysEnum(MapDtlAttr.WhenOverSize, 0, "WhenOverSize", true, true,
                 MapDtlAttr.WhenOverSize, "@0=不处理@1=向下顺增行@2=次页显示");

                map.AddDDLSysEnum(MapDtlAttr.DtlOpenType, 1, "数据开放类型", true, true,
                    MapDtlAttr.DtlOpenType, "@0=操作员@1=工作ID@2=流程ID");

                map.AddDDLSysEnum(MapDtlAttr.DtlShowModel, 0, "显示格式", true, true,
               MapDtlAttr.DtlShowModel, "@0=表格@1=卡片");

                map.AddTBFloat(MapDtlAttr.X, 5, "X", true, false);
                map.AddTBFloat(MapDtlAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(MapDtlAttr.H, 150, "H", true, false);
                map.AddTBFloat(MapDtlAttr.W, 200, "W", false, false);

                map.AddTBFloat(MapDtlAttr.FrmW, 900, "FrmW", true, true);
                map.AddTBFloat(MapDtlAttr.FrmH, 1200, "FrmH", true, true);

               
                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, false, 0, 128, 20);
                #endregion 基础信息.

                #region 导入导出填充.
                // 2014-07-17 for xinchang bank.
                map.AddBoolean(MapDtlAttr.IsExp, true, "是否可以导出？", false, false);
                map.AddBoolean(MapDtlAttr.IsImp, true, "是否可以导入？", false, false);
                map.AddBoolean(MapDtlAttr.IsEnableSelectImp, false, "是否启用选择数据导入?", false, false);
                map.AddTBString(MapDtlAttr.ImpSQLSearch, null, "查询SQL", true, false, 0, 500, 20,true);
                map.AddTBString(MapDtlAttr.ImpSQLInit, null, "初始化SQL", true, false, 0, 500, 20,true);
                map.AddTBString(MapDtlAttr.ImpSQLFull, null, "数据填充SQL", true, false, 0, 500, 20,true);
                #endregion 导入导出填充.


                #region 多表头.
                //MTR 多表头列.
                map.AddTBStringDoc(MapDtlAttr.MTR, null, "请书写html标记,以《TR》开头，以《/TR》结尾。", true, false, true);
                #endregion 多表头.


                #region 工作流相关.
                //add 2014-02-21.
                map.AddTBInt(MapDtlAttr.FK_Node, 0, "节点(用户独立表单权限控制)", false, false);
                map.AddBoolean(MapDtlAttr.IsCopyNDData, true, "是否允许copy节点数据", false, false);
                map.AddBoolean(MapDtlAttr.IsHLDtl, false, "是否是合流汇总", false, false);

                string sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='@No' AND  ( (MyDataType =1 and UIVisible=1 ) or (UIContralType=1))";
                map.AddDDLSQL(MapDtlAttr.SubThreadWorker, null, "子线程处理人字段", sql, true);
                #endregion 工作流相关.

                RefMethod  rm = new RefMethod();
                rm.Title = "高级设置"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAdvSetting";
                rm.Icon = "/WF/Img/Setting.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoAdvSetting()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapDtl.htm?DoType=Edit&FK_MapDtl=" + this.No + "&t=" + DataType.CurrentDataTime;
        }

        #region 基本属性.
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.X);
            }
        }
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.Y);
            }
        }
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.W);
            }
        }
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAttr.H);
            }
        }
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmW);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmH);
            }
        }
        #endregion 基本属性.


        protected override bool beforeUpdate()
        {
            MapDtl dtl = new MapDtl(this.No);
            dtl.IsEnablePass = this.IsEnableAthM;
            dtl.Update();


            return base.beforeUpdate();
        }

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <param name="fk_val"></param>
        /// <returns></returns>
        public int GetCountByFK(int workID)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE WorkID=" + workID);
        }
        public int GetCountByFK(string field, string val)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "='" + val + "'");
        }
        public int GetCountByFK(string field, Int64 val)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "=" + val);
        }
        public int GetCountByFK(string f1, Int64 val1, string f2, string val2)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(OID) from " + this.PTable + " WHERE " + f1 + "=" + val1 + " AND " + f2 + "='" + val2 + "'");
        }
        #endregion
    }
    /// <summary>
    /// 明细s
    /// </summary>
    public class MapDtlExts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 明细s
        /// </summary>
        public MapDtlExts()
        {
        }
        /// <summary>
        /// 明细s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapDtlExts(string fk_mapdata)
        {
            this.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata, MapDtlAttr.FK_Node, 0, MapDtlAttr.No);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDtlExt();
            }
        }
                
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDtlExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDtlExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDtlExt> Tolist()
        {
            System.Collections.Generic.List<MapDtlExt> list = new System.Collections.Generic.List<MapDtlExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDtlExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
