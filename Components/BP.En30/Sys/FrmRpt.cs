using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 纬度报表
    /// </summary>
    public class FrmRptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// PTable
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// DtlOpenType
        /// </summary>
        public const string DtlOpenType = "DtlOpenType";
        /// <summary>
        /// 插入表单的位置
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// 行数量
        /// </summary>
        public const string RowsOfList = "RowsOfList";
        /// <summary>
        /// 是否显示合计
        /// </summary>
        public const string IsShowSum = "IsShowSum";
        /// <summary>
        /// 是否显示idx
        /// </summary>
        public const string IsShowIdx = "IsShowIdx";
        /// <summary>
        /// 是否允许copy数据
        /// </summary>
        public const string IsCopyNDData = "IsCopyNDData";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string IsReadonly = "IsReadonly";
        /// <summary>
        /// WhenOverSize
        /// </summary>
        public const string WhenOverSize = "WhenOverSize";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDelete = "IsDelete";
        /// <summary>
        /// 是否可以插入
        /// </summary>
        public const string IsInsert = "IsInsert";
        /// <summary>
        /// 是否可以更新
        /// </summary>
        public const string IsUpdate = "IsUpdate";
        /// <summary>
        /// 是否启用通过
        /// </summary>
        public const string IsEnablePass = "IsEnablePass";
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public const string IsHLDtl = "IsHLDtl";
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public const string IsShowTitle = "IsShowTitle";
        /// <summary>
        /// 显示格式
        /// </summary>
        public const string EditModel = "EditModel";
        /// <summary>
        /// 是否可见
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// x
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// H高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// w宽度
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 
        /// </summary>
        public const string FrmW = "FrmW";
        /// <summary>
        /// 
        /// </summary>
        public const string FrmH = "FrmH";
        /// <summary>
        /// 是否可以导出
        /// </summary>
        public const string IsExp = "IsExp";
        /// <summary>
        /// 是否可以导入？
        /// </summary>
        public const string IsImp = "IsImp";
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public const string IsEnableAthM = "IsEnableAthM";
        /// <summary>
        /// 是否启用一对多
        /// </summary>
        public const string IsEnableM2M = "IsEnableM2M";
        /// <summary>
        /// 是否启用一对多多
        /// </summary>
        public const string IsEnableM2MM = "IsEnableM2MM";
        /// <summary>
        /// 多表头列
        /// </summary>
        public const string MTR = "MTR";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupField = "GroupField";
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        /// 
        public const string IsEnableGroupField = "IsEnableGroupField";
        public const string SQLOfColumn = "SQLOfColumn";
        public const string SQLOfRow = "SQLOfRow";
    }
    /// <summary>
    /// 纬度报表
    /// </summary>
    public class FrmRpt : EntityNoName
    {
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
        /// 从表
        /// </summary>
        public FrmRpts FrmRpts
        {
            get
            {
                FrmRpts obj = this.GetRefObject("FrmRpts") as FrmRpts;
                if (obj == null)
                {
                    obj = new FrmRpts(this.No);
                    this.SetRefObject("FrmRpts", obj);
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
        public string SQLOfRow
        {
            get
            {
                return this.GetValStringByKey(FrmRptAttr.SQLOfRow);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.SQLOfRow, value);
            }
        }
        public string SQLOfColumn
        {
            get
            {
                return this.GetValStringByKey(FrmRptAttr.SQLOfColumn);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.SQLOfColumn, value);
            }
        }
        public GEDtls HisGEDtls_temp = null;
        public EditModel HisEditModel
        {
            get
            {
                return (EditModel)this.GetValIntByKey(FrmRptAttr.EditModel);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.EditModel, (int)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public WhenOverSize HisWhenOverSize
        {
            get
            {
                return (WhenOverSize)this.GetValIntByKey(FrmRptAttr.WhenOverSize);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.WhenOverSize, (int)value);
            }
        }
        public bool IsExp
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsExp);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsExp, value);
            }
        }
        public bool IsImp
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsImp);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsImp, value);
            }
        }
        public bool IsShowSum
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsShowSum);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsShowSum, value);
            }
        }
        public bool IsShowIdx
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsShowIdx);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsShowIdx, value);
            }
        }
        public bool IsReadonly_del
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsReadonly);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsReadonly, value);
            }
        }
        public bool IsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public bool IsHLDtl
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsHLDtl);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsHLDtl, value);
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
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsDelete, value);
            }
        }
        public bool IsInsert
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsInsert);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsInsert, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsView
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsView);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsView, value);
            }
        }
        public bool IsUpdate
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsUpdate);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsUpdate, value);
            }
        }
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public bool IsEnableAthM
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsEnableAthM);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsEnableAthM, value);
            }
        }
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        public bool IsEnableGroupField
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsEnableGroupField);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsEnableGroupField, value);
            }
        }
        /// <summary>
        /// 是否起用审核连接
        /// </summary>
        public bool IsEnablePass
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsEnablePass);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsEnablePass, value);
            }
        }
        public bool IsCopyNDData
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsCopyNDData);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsCopyNDData, value);
            }
        }
        /// <summary>
        /// 是否启用一对多
        /// </summary>
        public bool IsEnableM2M
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsEnableM2M);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsEnableM2M, value);
            }
        }
        /// <summary>
        /// 是否启用一对多多
        /// </summary>
        public bool IsEnableM2MM
        {
            get
            {
                return this.GetValBooleanByKey(FrmRptAttr.IsEnableM2MM);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.IsEnableM2MM, value);
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
                return (DtlOpenType)this.GetValIntByKey(FrmRptAttr.DtlOpenType);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.DtlOpenType, (int)value);
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupField
        {
            get
            {
                return this.GetValStrByKey(FrmRptAttr.GroupField);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.GroupField, value);
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmRptAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.FK_MapData, value);
            }
        }
        public int RowsOfList
        {
            get
            {
                return this.GetValIntByKey(FrmRptAttr.RowsOfList);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.RowsOfList, value);
            }
        }
        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(FrmRptAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.RowIdx, value);
            }
        }
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(FrmRptAttr.GroupID);
            }
            set
            {
                this.SetValByKey(FrmRptAttr.GroupID, value);
            }
        }
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(FrmRptAttr.PTable);
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
                this.SetValByKey(FrmRptAttr.PTable, value);
            }
        }
        /// <summary>
        /// 多表头
        /// </summary>
        public string MTR
        {
            get
            {
                string s= this.GetValStrByKey(FrmRptAttr.MTR);
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
                this.SetValByKey(FrmRptAttr.MTR, value);
            }
        }
        #endregion

        #region 构造方法
        public Map GenerMap()
        {
            bool isdebug = SystemConfig.IsDebug;

            if (isdebug == false)
            {
                Map m = BP.DA.Cash.GetMap(this.No);
                if (m != null)
                    return m;
            }

            MapAttrs mapAttrs = this.MapAttrs;
            Map map = new Map(this.PTable,this.Name);
            map.Java_SetEnType(EnType.App);
            map.Java_SetDepositaryOfEntity(Depositary.None);
            map.Java_SetDepositaryOfMap( Depositary.Application);

            Attrs attrs = new Attrs();
            foreach (MapAttr mapAttr in mapAttrs)
                map.AddAttr(mapAttr.HisAttr);

            BP.DA.Cash.SetMap(this.No, map);
            return map;
        }
        public GEDtl HisGEDtl
        {
            get
            {
                GEDtl dtl = new GEDtl(this.No);
                return dtl;
            }
        }
        public GEEntity GenerGEMainEntity(string mainPK)
        {
            GEEntity en = new GEEntity(this.FK_MapData, mainPK);
            return en;
        }
        /// <summary>
        /// 纬度报表
        /// </summary>
        public FrmRpt()
        {
        }
        public FrmRpt(string mypk)
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
                Map map = new Map("Sys_FrmRpt", "纬度报表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = FrmImgAthDBAttr.FK_MapData; 

               
                map.AddTBStringPK(FrmRptAttr.No, null, "编号", true, false, 1, 20, 20);
                map.AddTBString(FrmRptAttr.Name, null, "描述", true, false, 1, 50, 20);
                map.AddTBString(FrmRptAttr.FK_MapData, null, "主表", true, false, 0, 100, 20);
                map.AddTBString(FrmRptAttr.PTable, null, "物理表", true, false, 0, 30, 20);

                map.AddTBString(FrmRptAttr.SQLOfColumn, null, "列的数据源", true, false, 0, 300, 20);
                map.AddTBString(FrmRptAttr.SQLOfRow, null, "行数据源", true, false, 0, 300, 20);

                map.AddTBInt(FrmRptAttr.RowIdx, 99, "位置", false, false);
                map.AddTBInt(FrmRptAttr.GroupID, 0, "GroupID", false, false);

                map.AddBoolean(FrmRptAttr.IsShowSum, true, "IsShowSum", false, false);
                map.AddBoolean(FrmRptAttr.IsShowIdx, true, "IsShowIdx", false, false);
                map.AddBoolean(FrmRptAttr.IsCopyNDData, true, "IsCopyNDData", false, false);
                map.AddBoolean(FrmRptAttr.IsHLDtl, false, "是否是合流汇总", false, false);

                map.AddBoolean(FrmRptAttr.IsReadonly, false, "IsReadonly", false, false);
                map.AddBoolean(FrmRptAttr.IsShowTitle, true, "IsShowTitle", false, false);
                map.AddBoolean(FrmRptAttr.IsView, true, "是否可见", false, false);

                map.AddBoolean(FrmRptAttr.IsExp, true, "IsExp", false, false);
                map.AddBoolean(FrmRptAttr.IsImp, true, "IsImp", false, false);

                map.AddBoolean(FrmRptAttr.IsInsert, true, "IsInsert", false, false);
                map.AddBoolean(FrmRptAttr.IsDelete, true, "IsDelete", false, false);
                map.AddBoolean(FrmRptAttr.IsUpdate, true, "IsUpdate", false, false);

                map.AddBoolean(FrmRptAttr.IsEnablePass, false, "是否启用通过审核功能?", false, false);
                map.AddBoolean(FrmRptAttr.IsEnableAthM, false, "是否启用多附件", false, false);

                map.AddBoolean(FrmRptAttr.IsEnableM2M, false, "是否启用M2M", false, false);
                map.AddBoolean(FrmRptAttr.IsEnableM2MM, false, "是否启用M2M", false, false);

                map.AddDDLSysEnum(FrmRptAttr.WhenOverSize, 0, "WhenOverSize", true, true,
                 FrmRptAttr.WhenOverSize, "@0=不处理@1=向下顺增行@2=次页显示");

                map.AddDDLSysEnum(FrmRptAttr.DtlOpenType, 1, "数据开放类型", true, true,
                    FrmRptAttr.DtlOpenType, "@0=操作员@1=工作ID@2=流程ID");

                map.AddDDLSysEnum(FrmRptAttr.EditModel, 0, "显示格式", true, true,
               FrmRptAttr.EditModel, "@0=表格@1=卡片");

                map.AddTBFloat(FrmRptAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmRptAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmRptAttr.H, 150, "H", true, false);
                map.AddTBFloat(FrmRptAttr.W, 200, "W", false, false);

                map.AddTBFloat(FrmRptAttr.FrmW, 900, "FrmW", true, true);
                map.AddTBFloat(FrmRptAttr.FrmH, 1200, "FrmH", true, true);

                //MTR 多表头列.
                map.AddTBString(FrmRptAttr.MTR, null, "多表头列", true, false, 0, 3000, 20);
                map.AddTBString(FrmRptAttr.GUID, null, "GUID", true, false, 0, 128, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
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
                return this.GetValFloatByKey(FrmRptAttr.FrmW);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(FrmRptAttr.FrmH);
            }
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

        public void IntMapAttrs()
        {
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = this.No;
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = this.Name;
                md.Insert();
            }

            MapAttrs attrs = new MapAttrs(this.No);
            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (attrs.Contains(MapAttrAttr.KeyOfEn, "OID") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.Readonly;

                attr.KeyOfEn = "OID";
                attr.Name = "主键";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "RefPK") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.Readonly;

                attr.KeyOfEn = "RefPK";
                attr.Name = "关联ID";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.Insert();
            }


            if (attrs.Contains(MapAttrAttr.KeyOfEn, "FID") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.Readonly;

                attr.KeyOfEn = "FID";
                attr.Name = "FID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "RDT") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;

                attr.KeyOfEn = "RDT";
                attr.Name = "记录时间";
                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "Rec") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.Readonly;

                attr.KeyOfEn = "Rec";
                attr.Name = "记录人";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MaxLen = 20;
                attr.MinLen = 0;
                attr.DefVal = "@WebUser.No";
                attr.Tag = "@WebUser.No";
                attr.Insert();
            }
        }
        private void InitExtMembers()
        {
            /* 如果启用了多附件*/
            if (this.IsEnableAthM)
            {
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                athDesc.MyPK = this.No + "_AthM";
                if (athDesc.RetrieveFromDBSources() == 0)
                {
                    athDesc.FK_MapData = this.No;
                    athDesc.NoOfObj = "AthM";
                    athDesc.Name = this.Name;
                    athDesc.Insert();
                }
            }
        }
        protected override bool beforeInsert()
        {
            this.InitExtMembers();
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.IsEnablePass)
            {
                /*判断是否有IsPass 字段。*/
                MapAttrs attrs = new MapAttrs(this.No);
                if (attrs.Contains(MapAttrAttr.KeyOfEn, "IsPass") == false)
                    throw new Exception("您启用了从表单(" + this.Name + ")条数据审核选项，但是该从表里没IsPass字段，请参考帮助文档。");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeUpdate()
        {
            MapAttrs attrs = new MapAttrs(this.No);
            bool isHaveEnable = false;
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIIsEnable && attr.UIContralType == UIContralType.TB)
                    isHaveEnable = true;
            }
            this.InitExtMembers();
            return base.beforeUpdate();
        }
        protected override bool beforeDelete()
        {
            string sql = "";
            sql += "@DELETE FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmLab WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmLink WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmImgAth WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmRB WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_MapFrame WHERE FK_MapData='" + this.No + "'";

            if (this.No.Contains("BP.") == false)
            sql += "@DELETE FROM Sys_MapExt WHERE FK_MapData='" + this.No + "'";

            sql += "@DELETE FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_MapData WHERE No='" + this.No + "'";
            sql += "@DELETE FROM Sys_GroupField WHERE FrmID='" + this.No + "'";
            DBAccess.RunSQLs(sql);
            try
            {
                if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) as Num FROM " + this.PTable + " WHERE 1=1 ") == 0)
                    BP.DA.DBAccess.RunSQL("DROP TABLE " + this.PTable);
            }
            catch
            {
            }
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 纬度报表s
    /// </summary>
    public class FrmRpts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 纬度报表s
        /// </summary>
        public FrmRpts()
        {
        }
        /// <summary>
        /// 纬度报表s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmRpts(string fk_mapdata)
        {
            this.Retrieve(FrmRptAttr.FK_MapData, fk_mapdata, FrmRptAttr.No);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmRpt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmRpt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmRpt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmRpt> Tolist()
        {
            System.Collections.Generic.List<FrmRpt> list = new System.Collections.Generic.List<FrmRpt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmRpt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
