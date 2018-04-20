using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using System.Collections.Generic;

namespace BP.Sys
{
    /// <summary>
    /// 按日期查询方式
    /// </summary>
    public enum DTSearchWay
    {
        /// <summary>
        /// 不设置
        /// </summary>
        None,
        /// <summary>
        /// 按日期
        /// </summary>
        ByDate,
        /// <summary>
        /// 按日期时间
        /// </summary>
        ByDateTime
    }
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 独立表单
        /// </summary>
        Application = 0,
        /// <summary>
        /// 节点表单
        /// </summary>
        Node = 1
    }
    public enum FrmFrom
    {
        Flow,
        Node,
        Dtl
    }
    /// <summary>
    /// 表单类型 0=傻瓜表单@1=自由表单@2=Silverlight表单(已取消)@3=嵌入式表单@4=Word表单@5=Excel表单
    /// </summary>
    public enum FrmType
    {
        /// <summary>
        /// 傻瓜表单
        /// </summary>
        FoolForm = 0,
        /// <summary>
        /// 自由表单
        /// </summary>
        FreeFrm = 1,
        /// <summary>
        /// Silverlight表单(已取消)
        /// </summary>
        Silverlight = 2,
        /// <summary>
        /// URL表单(自定义)
        /// </summary>
        Url = 3,
        /// <summary>
        /// Word类型表单
        /// </summary>
        WordFrm = 4,
        /// <summary>
        /// Excel类型表单
        /// </summary>
        ExcelFrm = 5,
        /// <summary>
        /// VSTOExccel模式.
        /// </summary>
        VSTOForExcel = 6,
        /// <summary>
        /// 实体类组件
        /// </summary>
        Entity
    }
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapDataAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 表单事件实体类
        /// </summary>
        public const string FormEventEntity = "FormEventEntity";
        /// <summary>
        /// 存储表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 表存储格式0=自定义表,1=指定表,可以修改字段2=执行表不可以修改字段.
        /// </summary>
        public const string PTableModel = "PTableModel";
        public const string Dtls = "Dtls";
        public const string EnPK = "EnPK";
        public const string FrmW = "FrmW";
        public const string FrmH = "FrmH";
        /// <summary>
        /// 表格列(对傻瓜表单有效)
        /// </summary>
        public const string TableCol = "TableCol";
        /// <summary>
        /// 表格宽度
        /// </summary>
        public const string TableWidth = "TableWidth";
        /// <summary>
        /// 表格高度
        /// </summary>
        public const string TableHeight = "TableHeight";
        /// <summary>
        /// 来源
        /// </summary>
        public const string FrmFrom = "FrmFrom";
        /// <summary>
        /// 设计者
        /// </summary>
        public const string Designer = "Designer";
        /// <summary>
        /// 设计者单位
        /// </summary>
        public const string DesignerUnit = "DesignerUnit";
        /// <summary>
        /// 设计者联系方式
        /// </summary>
        public const string DesignerContact = "DesignerContact";
        /// <summary>
        /// 设计器
        /// </summary>
        public const string DesignerTool11 = "DesignerTool";
        /// <summary>
        /// 表单类别
        /// </summary>
        public const string FK_FrmSort = "FK_FrmSort";
        /// <summary>
        /// 表单树类别
        /// </summary>
        public const string FK_FormTree = "FK_FormTree";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// Url(对于嵌入式表单有效)
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// Tag
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 版本号
        /// </summary>
        public const string Ver = "Ver";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string AppType = "AppType";
        /// <summary>
        /// 表单body属性.
        /// </summary>
        public const string BodyAttr = "BodyAttr";
        /// <summary>
        /// 流程控件
        /// </summary>
        public const string FlowCtrls = "FlowCtrls";
        

        #region 报表属性(参数的方式存储).
        /// <summary>
        /// 是否关键字查询
        /// </summary>
        public const string RptIsSearchKey = "RptIsSearchKey";
        /// <summary>
        /// 时间段查询方式
        /// </summary>
        public const string RptDTSearchWay = "RptDTSearchWay";
        /// <summary>
        /// 时间字段
        /// </summary>
        public const string RptDTSearchKey = "RptDTSearchKey";
        /// <summary>
        /// 查询外键枚举字段
        /// </summary>
        public const string RptSearchKeys = "RptSearchKeys";
        #endregion 报表属性(参数的方式存储).

        #region 其他计算属性，参数存储.
        /// <summary>
        /// 最左边的值
        /// </summary>
        public const string MaxLeft = "MaxLeft";
        /// <summary>
        /// 最右边的值
        /// </summary>
        public const string MaxRight = "MaxRight";
        /// <summary>
        /// 最头部的值
        /// </summary>
        public const string MaxTop = "MaxTop";
        /// <summary>
        /// 最底部的值
        /// </summary>
        public const string MaxEnd = "MaxEnd";
        #endregion 其他计算属性，参数存储.

        #region weboffice属性。
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public const string IsWoEnableWF = "IsWoEnableWF";
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public const string IsWoEnableSave = "IsWoEnableSave";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string IsWoEnableReadonly = "IsWoEnableReadonly";
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public const string IsWoEnableRevise = "IsWoEnableRevise";
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public const string IsWoEnableViewKeepMark = "IsWoEnableViewKeepMark";
        /// <summary>
        /// 是否打印
        /// </summary>
        public const string IsWoEnablePrint = "IsWoEnablePrint";
        /// <summary>
        /// 是否启用签章
        /// </summary>
        public const string IsWoEnableSeal = "IsWoEnableSeal";
        /// <summary>
        /// 是否启用套红
        /// </summary>
        public const string IsWoEnableOver = "IsWoEnableOver";
        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public const string IsWoEnableTemplete = "IsWoEnableTemplete";
        /// <summary>
        /// 是否自动写入审核信息
        /// </summary>
        public const string IsWoEnableCheck = "IsWoEnableCheck";
        /// <summary>
        /// 是否插入流程
        /// </summary>
        public const string IsWoEnableInsertFlow = "IsWoEnableInsertFlow";
        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public const string IsWoEnableInsertFengXian = "IsWoEnableInsertFengXian";
        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public const string IsWoEnableMarks = "IsWoEnableMarks";
        /// <summary>
        /// 是否启用下载
        /// </summary>
        public const string IsWoEnableDown = "IsWoEnableDown";
        #endregion weboffice属性。

        #region 参数属性.
        public const string EnsName = "EnsName";
        #endregion 参数属性.
    }
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapData : EntityNoName
    {
        #region entity 相关属性(参数属性)
        /// <summary>
        /// 属性ens
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.GetValStringByKey(MapDataAttr.EnsName);
            }
            set
            {
                this.SetPara(MapDataAttr.EnsName, value);
            }
        }
        #endregion entity 相关操作.


        #region weboffice文档属性(参数属性)
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsRowLock, value);
            }
        }

        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool IsWoEnablePrint
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnablePrint);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnablePrint, value);
            }
        }
        /// <summary>
        /// 是否启用只读
        /// </summary>
        public bool IsWoEnableReadonly
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableReadonly);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableReadonly, value);
            }
        }
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public bool IsWoEnableRevise
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableRevise);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableRevise, value);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool IsWoEnableSave
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSave);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSave, value);
            }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool IsWoEnableViewKeepMark
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableViewKeepMark);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableViewKeepMark, value);
            }
        }
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public bool IsWoEnableWF
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableWF);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableWF, value);
            }
        }

        /// <summary>
        /// 是否启用套红
        /// </summary>
        public bool IsWoEnableOver
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableOver);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableOver, value);
            }
        }

        /// <summary>
        /// 是否启用签章
        /// </summary>
        public bool IsWoEnableSeal
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSeal);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSeal, value);
            }
        }

        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public bool IsWoEnableTemplete
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableTemplete);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableTemplete, value);
            }
        }

        /// <summary>
        /// 是否记录节点信息
        /// </summary>
        public bool IsWoEnableCheck
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableCheck, value);
            }
        }

        /// <summary>
        /// 是否插入流程图
        /// </summary>
        public bool IsWoEnableInsertFlow
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFlow);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFlow, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableInsertFengXian
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFengXian);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFengXian, value);
            }
        }

        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public bool IsWoEnableMarks
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableMarks);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableMarks, value);
            }
        }
        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableDown
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableDown);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableDown, value);
            }
        }
        #endregion weboffice文档属性

        #region 自动计算属性.
        public float MaxLeft
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxLeft);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxLeft, value);
            }
        }
        public float MaxRight
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxRight);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxRight, value);
            }
        }
        public float MaxTop
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxTop);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxTop, value);
            }
        }
        public float MaxEnd
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxEnd);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxEnd, value);
            }
        }
        #endregion 自动计算属性.

        #region 报表属性(参数方式存储).
        /// <summary>
        /// 是否关键字查询
        /// </summary>
        public bool RptIsSearchKey
        {
            get
            {
                return this.GetParaBoolen(MapDataAttr.RptIsSearchKey, true);
            }
            set
            {
                this.SetPara(MapDataAttr.RptIsSearchKey, value);
            }
        }
        /// <summary>
        /// 时间段查询方式
        /// </summary>
        public DTSearchWay RptDTSearchWay
        {
            get
            {
                return (DTSearchWay)this.GetParaInt(MapDataAttr.RptDTSearchWay);
            }
            set
            {
                this.SetPara(MapDataAttr.RptDTSearchWay, (int)value);
            }
        }
        /// <summary>
        /// 时间字段
        /// </summary>
        public string RptDTSearchKey
        {
            get
            {
                return this.GetParaString(MapDataAttr.RptDTSearchKey);
            }
            set
            {
                this.SetPara(MapDataAttr.RptDTSearchKey, value);
            }
        }
        /// <summary>
        /// 查询外键枚举字段
        /// </summary>
        public string RptSearchKeys
        {
            get
            {
                return this.GetParaString(MapDataAttr.RptSearchKeys, "*");
            }
            set
            {
                this.SetPara(MapDataAttr.RptSearchKeys, value);
            }
        }
        #endregion 报表属性(参数方式存储).

        #region 外键属性
        /// <summary>
        ///版本号.
        /// </summary>
        public string Ver
        {
            get
            {
                return this.GetValStringByKey(MapDataAttr.Ver);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Ver, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.Idx);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Idx, value);
            }
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        public FrmEles HisFrmEles
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
        /// 枚举值
        /// </summary>
        public SysEnums SysEnums
        {
            get
            {
                SysEnums obj = this.GetRefObject("SysEnums") as SysEnums;
                if (obj == null)
                {
                    obj = new SysEnums();
                    obj.RetrieveInSQL(SysEnumAttr.EnumKey, "SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "'");
                    this.SetRefObject("SysEnums", obj);

                }
                return obj;
            }
        }
        /// <summary>
        /// 报表
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

        #region 基本属性.
        /// <summary>
        /// 事件实体
        /// </summary>
        public string FormEventEntity
        {
            get
            {
                return this.GetValStringByKey(MapDataAttr.FormEventEntity);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FormEventEntity, value);
            }
        }
 
        public static Boolean IsEditDtlModel
        {
            get
            {
                string s = BP.Web.WebUser.GetSessionByKey("IsEditDtlModel", "0");
                if (s == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                BP.Web.WebUser.SetSessionByKey("IsEditDtlModel", "1");
            }
        }
        #endregion 基本属性.

        #region 表单结构数据json
        /// <summary>
        /// 表单图数据
        /// </summary>
        public string FormJson
        {
            get
            {
                // return this.GenerHisFrm();
                string str = this.GetBigTextFromDB("FormJson");
                if (str == null || str == "")
                {
                    return "";
                }
                return str;
            }
            set
            {
                this.SaveBigTxtToDB("FormJson", value);
            }
        }
        /// <summary>
        /// 生成Frm
        /// </summary>
        /// <returns></returns>
        public string GenerHisFrm()
        {
            string body = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfWebApp + "\\WF\\Admin\\CCFormDesigner\\EleTemplate\\Body.txt");

            //替换高度宽度.
            body = body.Replace("@FrmH", this.FrmH.ToString());
            body = body.Replace("@FrmW", this.FrmW.ToString());

            string labTemplate = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfWebApp + "\\WF\\Admin\\CCFormDesigner\\EleTemplate\\Label.txt");
            string myLabs = "";
            FrmLabs labs = new FrmLabs(this.No);
            foreach (FrmLab lab in labs)
            {
                string labTxt = labTemplate.Clone() as string;

                labTxt = labTxt.Replace("@MyPK", lab.MyPK);

                labTxt = labTxt.Replace("@Label", lab.Text);
                labTxt = labTxt.Replace("@X", lab.X.ToString());
                labTxt = labTxt.Replace("@Y", lab.Y.ToString());

                float y1 = lab.Y - 20;
                labTxt = labTxt.Replace("@Y1", y1.ToString());
                myLabs += labTxt + ",";
            }

            if (myLabs == "")
            {
                body = body.Replace("@Labels", myLabs);
            }
            else
            {
                myLabs = myLabs.Substring(0, myLabs.Length - 1);
                body = body.Replace("@Labels", myLabs);
            }
            return body;
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
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// 表存储模式0=自定义表,1,指定的表,2=指定的表不能修改表结构.
        /// @周朋
        /// </summary>
        public int PTableModel
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.PTableModel);
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTableModel, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Url);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Url, value);
            }
        }
        public DBUrlType HisDBUrl
        {
            get
            {
                return DBUrlType.AppCenterDSN;
                // return (DBUrlType)this.GetValIntByKey(MapDataAttr.DBURL);
            }
        }
        public int HisFrmTypeInt
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, value);
            }
        }
        public FrmType HisFrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, (int)value);
            }
        }
        /// <summary>
        /// 表单类型名称
        /// </summary>
        public string HisFrmTypeText
        {
            get
            {
                return this.HisFrmType.ToString();

                //  SysEnum se = new SysEnum("FrmType", this.HisFrmTypeInt);
                // return se.Lab;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Note);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Note, value);
            }
        }
        /// <summary>
        /// 是否有CA.
        /// </summary>
        public bool IsHaveCA
        {
            get
            {
                return this.GetParaBoolen("IsHaveCA", false);

            }
            set
            {
                this.SetPara("IsHaveCA", value);
            }
        }
        /// <summary>
        /// 类别，可以为空.
        /// </summary>
        public string FK_FrmSort
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FrmSort);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FrmSort, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DBSrc
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.DBSrc);
            }
            set
            {
                this.SetValByKey(MapDataAttr.DBSrc, value);
            }
        }

        /// <summary>
        /// 类别，可以为空.
        /// </summary>
        public string FK_FormTree
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FormTree, value);
            }
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string FK_FormTreeText
        {
            get
            {
                return DBAccess.RunSQLReturnStringIsNull("SELECT Name FROM Sys_FormTree WHERE No='" + this.FK_FormTree + "'", "目录错误");
            }
        }
        /// <summary>
        /// 从表集合.
        /// </summary>
        public string Dtls
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Dtls);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Dtls, value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string EnPK
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.EnPK);
                if (DataType.IsNullOrEmpty(s))
                    return "OID";
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.EnPK, value);
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
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmW);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmW, value);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmH);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmH, value);
            }
        }
        /// <summary>
        /// 应用类型.  0独立表单.1节点表单
        /// </summary>
        public string AppType
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.AppType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.AppType, value);
            }
        }
        /// <summary>
        /// 表单body属性.
        /// </summary>
        public string BodyAttr
        {
            get
            {
                string str = this.GetValStrByKey(MapDataAttr.BodyAttr);
                str = str.Replace("~", "'");
                return str;
            }
            set
            {
                this.SetValByKey(MapDataAttr.BodyAttr, value);
            }
        }
        /// <summary>
        /// 流程控件s.
        /// </summary>
        public string FlowCtrls
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FlowCtrls);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FlowCtrls, value);
            }
        }
        #endregion

        #region 构造方法
        public Map GenerHisMap()
        {
            MapAttrs mapAttrs = this.MapAttrs;
            if (mapAttrs.Count == 0)
            {
                this.RepairMap();
                mapAttrs = this.MapAttrs;
            }

            Map map = new Map(this.PTable, this.Name);
            map.EnDBUrl = new DBUrl(this.HisDBUrl);
            map.Java_SetEnType(EnType.App);
            map.Java_SetDepositaryOfEntity(Depositary.None);
            map.Java_SetDepositaryOfMap(Depositary.Application);

            Attrs attrs = new Attrs();
            foreach (MapAttr mapAttr in mapAttrs)
                map.AddAttr(mapAttr.HisAttr);

            // 产生从表。
            MapDtls dtls = this.MapDtls; // new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                GEDtls dtls1 = new GEDtls(dtl.No);
                map.AddDtl(dtls1, "RefPK");
            }

            #region 查询条件.
            map.IsShowSearchKey = this.RptIsSearchKey; //是否启用关键字查询.
            // 按日期查询.
            map.DTSearchWay = this.RptDTSearchWay; //日期查询方式.
            map.DTSearchKey = this.RptDTSearchKey; //日期字段.

            //加入外键查询字段.
            string[] keys = this.RptSearchKeys.Split('*');
            foreach (string key in keys)
            {
                if (DataType.IsNullOrEmpty(key))
                    continue;

                map.AddSearchAttr(key);
            }
            #endregion 查询条件.

            return map;
        }
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
        /// 生成map.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static Map GenerHisMap(string no)
        {
            if (SystemConfig.IsDebug)
            {
                MapData md = new MapData();
                md.No = no;
                md.Retrieve();
                return md.GenerHisMap();
            }
            else
            {
                Map map = BP.DA.Cash.GetMap(no);
                if (map == null)
                {
                    MapData md = new MapData();
                    md.No = no;
                    md.Retrieve();
                    map = md.GenerHisMap();
                    BP.DA.Cash.SetMap(no, map);
                }
                return map;
            }
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        public MapData()
        {
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapData(string no)
            : base(no)
        {
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

                Map map = new Map("Sys_MapData", "表单注册表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基础信息.
                map.AddTBStringPK(MapDataAttr.No, null, "编号", true, false, 1, 200, 100);
                map.AddTBString(MapDataAttr.Name, null, "描述", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.FormEventEntity, null, "事件实体", true, true, 0, 100, 20, true);

                map.AddTBString(MapDataAttr.EnPK, null, "实体主键", true, false, 0, 200, 20);
                map.AddTBString(MapDataAttr.PTable, null, "物理表", true, false, 0, 500, 20);

                //@周朋 表存储格式0=自定义表,1=指定表,可以修改字段2=执行表不可以修改字段.
                map.AddTBInt(MapDataAttr.PTableModel, 0, "表存储模式", true, true);
                

                map.AddTBString(MapDataAttr.Url, null, "连接(对嵌入式表单有效)", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Dtls, null, "从表", true, false, 0, 500, 20);

                //格式为: @1=方案名称1@2=方案名称2@3=方案名称3
                //map.AddTBString(MapDataAttr.Slns, null, "表单控制解决方案", true, false, 0, 500, 20);

                map.AddTBInt(MapDataAttr.FrmW, 900, "FrmW", true, true);
                map.AddTBInt(MapDataAttr.FrmH, 1200, "FrmH", true, true);

                map.AddTBInt(MapDataAttr.TableCol, 4, "傻瓜表单显示的列", true, true);
                map.AddTBInt(MapDataAttr.TableWidth, 800, "表格宽度", true, true);

                //Tag
                map.AddTBString(MapDataAttr.Tag, null, "Tag", true, false, 0, 500, 20);

                // 可以为空这个字段。
                map.AddTBString(MapDataAttr.FK_FrmSort, null, "表单类别", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.FK_FormTree, null, "表单树类别", true, false, 0, 500, 20);


                // enumFrmType  @自由表单，@傻瓜表单，@嵌入式表单.  
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FreeFrm, "表单类型", true, false, MapDataAttr.FrmType);

                // 应用类型.  0独立表单.1节点表单
                map.AddTBInt(MapDataAttr.AppType, 0, "应用类型", true, false);
                map.AddTBString(MapDataAttr.DBSrc, "local", "数据源", true, false, 0, 100, 20);
                map.AddTBString(MapDataAttr.BodyAttr, null, "表单Body属性", true, false, 0, 100, 20);
                #endregion 基础信息.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);

                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", true, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, false, 0, 128, 20);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, false, 0, 30, 20);

                //流程控件.
                map.AddTBString(MapDataAttr.FlowCtrls, null, "流程控件", true, true, 0, 200, 20);

                //增加参数字段.
                map.AddTBAtParas(4000);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 上移
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(MapDataAttr.FK_FormTree, this.FK_FormTree, MapDataAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        public void DoOrderDown()
        {
            this.DoOrderDown(MapDataAttr.FK_FormTree, this.FK_FormTree, MapDataAttr.Idx);
        }

        //检查表单
        public void CheckPTableSaveModel(string filed)
        {
            if (this.PTableModel == 2)
            {
                /*如果是存储格式*/
                if (DBAccess.IsExitsTableCol(this.PTable, filed) == false)
                    throw new Exception("@表单的表存储模式不允许您创建不存在的字段(" + filed + ")，不允许修改表结构.");
            }
        }

        /// <summary>
        /// 获得PTableModel=2模式下的表单，没有被使用的字段集合.
        /// </summary>
        /// <param name="frmID"></param>
        /// <returns></returns>
        public static DataTable GetFieldsOfPTableMode2(string frmID)
        {
            string pTable = "";

            MapDtl dtl = new MapDtl();
            dtl.No = frmID;
            if (dtl.RetrieveFromDBSources() == 1)
            {
                pTable = dtl.PTable;
            }
            else
            {
                MapData md = new MapData();
                md.No = frmID;
                md.RetrieveFromDBSources();
                pTable = md.PTable;
            }

            //获得原始数据.
            DataTable dt = BP.DA.DBAccess.GetTableSchema(pTable, false);

            //创建样本表结构.
            DataTable mydt = BP.DA.DBAccess.GetTableSchema(pTable, false);
            mydt.Rows.Clear();

            //获得现有的列..
            MapAttrs attrs = new MapAttrs(frmID);

            string flowFiels = ",GUID,PRI,PrjNo,PrjName,PEmp,AtPara,FlowNote,WFSta,PNodeID,FK_FlowSort,FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,MyNum,PWorkID,PFlowNo,BillNo,ProjNo,";

            //排除已经存在的列. 把所有的列都输出给前台，让前台根据类型分拣.
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr["FName"].ToString();
                if (attrs.Contains(MapAttrAttr.KeyOfEn, key) == true)
                    continue;

                if (flowFiels.Contains("," + key + ",") == true)
                    continue;

                DataRow mydr = mydt.NewRow();
                mydr["FName"] = dr["FName"];
                mydr["FType"] = dr["FType"];
                mydr["FLen"] = dr["FLen"];
                mydr["FDesc"] = dr["FDesc"];
                mydt.Rows.Add(mydr);
            }
            return mydt;
        }


        #endregion

        #region 常用方法.
        private FormEventBase _HisFEB = null;
        public FormEventBase HisFEB
        {
            get
            {
                if (this.FormEventEntity == "")
                    return null;

                if (_HisFEB == null)
                    _HisFEB = BP.Sys.Glo.GetFormEventBaseByEnName(this.No);

                return _HisFEB;
            }
        }
        /// <summary>
        /// 执行事件.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="en"></param>
        /// <param name="atParas"></param>
        /// <returns></returns>
        public string DoEvent(string eventType, Entity en, string atParas = null)
        {
            string str = this.FrmEvents.DoEventNode(eventType, en);

            string mystrs = null;
            if (this.HisFEB != null)
                mystrs = this.HisFEB.DoIt(eventType, en, atParas);

            if (str == null)
                return mystrs;

            if (mystrs == null)
                return str;

            return str + "@" + mystrs;
        }
        /// <summary>
        /// 升级逻辑.
        /// </summary>
        public void Upgrade()
        {
            string sql = "";
            #region 升级ccform控件.
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmLine") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLine", "Sys_FrmLineBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLineBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Line;
                    //ele.BorderColor = dr["BorderColor"].ToString();
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());
                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmLab") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLab", "Sys_FrmLabBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLabBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Label;

                    ele.EleName = dr[FrmLabAttr.Text].ToString();

                    //ele.FontColor = dr[FrmLabAttr.FontColor].ToString();
                    //ele.FontName = dr[FrmLabAttr.FontName].ToString();
                    //ele.FontSize = int.Parse(dr[FrmLabAttr.FontSize].ToString());
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());

                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmBtn") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLab", "Sys_FrmLabBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLabBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Line;
                    //ele.BorderColor = dr["BorderColor"].ToString();
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());
                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            #endregion 升级ccform控件.
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="isSetReadony"></param>
        /// <returns></returns>
        public static MapData ImpMapData(DataSet ds)
        {
            string errMsg = "";
            if (ds.Tables.Contains("WF_Flow") == true)
                errMsg += "@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";

            if (errMsg != "")
                throw new Exception(errMsg);

            DataTable dt = ds.Tables["Sys_MapData"];
            string fk_mapData = dt.Rows[0]["No"].ToString();
            MapData md = new MapData();
            md.No = fk_mapData;
            if (md.IsExits)
                throw new Exception("@已经存在(" + fk_mapData + ")的表单ID，所以您不能导入。");

            //导入.
            return ImpMapData(fk_mapData, ds);
        }
        /// <summary>
        /// 设置表单为只读属性
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        public static void SetFrmIsReadonly(string fk_mapdata)
        {
            //把主表字段设置为只读.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            foreach (MapAttr attr in attrs)
            {
                if (attr.DefValReal.Contains("@"))
                {
                    attr.UIIsEnable = false;
                    attr.DefValReal = ""; //清空默认值.
                    attr.SetValByKey("ExtDefVal", ""); //设置默认值.
                    attr.Update();
                    continue;
                }

                if (attr.UIIsEnable == true)
                {
                    attr.UIIsEnable = false;
                    attr.Update();
                    continue;
                }
            }

            //把从表字段设置为只读.
            MapDtls dtls = new MapDtls(fk_mapdata);
            foreach (MapDtl dtl in dtls)
            {
                dtl.IsInsert = false;
                dtl.IsUpdate = false;
                dtl.IsDelete = false;
                dtl.Update();

                attrs = new MapAttrs(dtl.No);
                foreach (MapAttr attr in attrs)
                {
                    if (attr.DefValReal.Contains("@"))
                    {
                        attr.UIIsEnable = false;
                        attr.DefValReal = ""; //清空默认值.
                        attr.SetValByKey("ExtDefVal", ""); //设置默认值.
                        attr.Update();
                    }

                    if (attr.UIIsEnable == true)
                    {
                        attr.UIIsEnable = false;
                        attr.Update();
                        continue;
                    }
                }
            }

            //把附件设置为只读.
            FrmAttachments aths = new FrmAttachments(fk_mapdata);
            foreach (FrmAttachment item in aths)
            {
                item.IsUpload = false;
                item.HisDeleteWay = AthDeleteWay.DelSelf;

                //如果是从开始节点表单导入的,就默认为, 按照主键PK的方式显示.
                if (fk_mapdata.IndexOf("ND") == 0)
                {
                    item.HisCtrlWay = AthCtrlWay.PK;
                    item.DataRefNoOfObj = "AttachM1";
                }
                item.Update();
            }
        }
        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ds">表单数据</param>
        /// <param name="isSetReadonly">是否设置只读？</param>
        /// <returns></returns>
        public static MapData ImpMapData(string fk_mapdata, DataSet ds)
        {

            #region 检查导入的数据是否完整.
            string errMsg = "";
            //if (ds.Tables[0].TableName != "Sys_MapData")
            //    errMsg += "@非表单模板。";

            if (ds.Tables.Contains("WF_Flow") == true)
                errMsg += "@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";

            DataTable dtCheck = ds.Tables["Sys_MapAttr"];
            bool isHave = false;
            foreach (DataRow dr in dtCheck.Rows)
            {
                if (dr["KeyOfEn"].ToString() == "OID")
                {
                    isHave = true;
                    break;
                }
            }

            if (isHave == false)
                errMsg += "@表单模版缺少列:OID";

            if (errMsg != "")
                throw new Exception("@以下错误不可导入，可能的原因是非表单模板文件:" + errMsg);
            #endregion

            // 定义在最后执行的sql.
            string endDoSQL = "";

            //检查是否存在OID字段.
            MapData mdOld = new MapData();
            mdOld.No = fk_mapdata;
            mdOld.RetrieveFromDBSources();
            mdOld.Delete();

            // 求出dataset的map.
            string oldMapID = "";
            DataTable dtMap = ds.Tables["Sys_MapData"];
            if (dtMap.Rows.Count == 1)
            {
                oldMapID = dtMap.Rows[0]["No"].ToString();
            }
            else
            {
                // 求旧的表单ID.
                foreach (DataRow dr in dtMap.Rows)
                    oldMapID = dr["No"].ToString();

                if (DataType.IsNullOrEmpty(oldMapID) == true)
                    oldMapID = dtMap.Rows[0]["No"].ToString();
            }

            string timeKey = DateTime.Now.ToString("MMddHHmmss");

            #region 表单元素
            foreach (DataTable dt in ds.Tables)
            {
                int idx = 0;
                switch (dt.TableName)
                {
                    case "Sys_MapDtl":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapDtl dtl = new MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                dtl.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            dtl.Insert();
                        }
                        break;
                    case "Sys_MapData":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapData md = new MapData();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                md.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            //如果物理表为空，则使用编号为物理数据表
                            if (DataType.IsNullOrEmpty(md.PTable.Trim()) == true)
                                md.PTable = md.No;

                            //表单类别编号不为空，则用原表单类别编号
                            if (DataType.IsNullOrEmpty(mdOld.FK_FormTree) == false)
                                md.FK_FormTree = mdOld.FK_FormTree;

                            //表单类别编号不为空，则用原表单类别编号
                            if (DataType.IsNullOrEmpty(mdOld.FK_FrmSort) == false)
                                md.FK_FrmSort = mdOld.FK_FrmSort;

                            if (DataType.IsNullOrEmpty(mdOld.PTable) == false)
                                md.PTable = mdOld.PTable;
                            if (DataType.IsNullOrEmpty(mdOld.Name) == false)
                                md.Name = mdOld.Name;

                            md.HisFrmType = mdOld.HisFrmType;
                            //表单应用类型保持不变
                            md.AppType = mdOld.AppType;

                            md.DirectInsert();
                        }
                        break;
                    case "Sys_FrmBtn":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmBtn en = new FrmBtn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            //en.MyPK = "Btn_" + idx + "_" + fk_mapdata;
                            en.MyPK = DBAccess.GenerGUID();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLine":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLine en = new FrmLine();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //en.MyPK = "LE_" + idx + "_" + fk_mapdata;
                            en.MyPK = DBAccess.GenerGUID();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLab":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLab en = new FrmLab();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //  en.FK_MapData = fk_mapdata; 删除此行解决从表lab的问题。
                            //en.MyPK = "LB_" + idx + "_" + fk_mapdata;
                            en.MyPK = DBAccess.GenerGUID();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLink":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLink en = new FrmLink();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //en.MyPK = "LK_" + idx + "_" + fk_mapdata;
                            en.MyPK = DBAccess.GenerGUID();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmEle":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEle en = new FrmEle();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }


                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImg":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //en.MyPK = "Img_" + idx + "_" + fk_mapdata;
                            en.MyPK = DBAccess.GenerGUID();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImgAth":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImgAth en = new FrmImgAth();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            if (DataType.IsNullOrEmpty(en.CtrlID))
                                en.CtrlID = "ath" + idx;

                            en.Insert();
                        }
                        break;
                    case "Sys_FrmRB":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }


                            try
                            {
                                en.Save();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_FrmAttachment":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Ath_" + idx + "_" + fk_mapdata;


                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_MapM2M":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapM2M en = new MapM2M();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                                en.Update();
                            }
                        }
                        break;
                    case "Sys_MapFrame":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapFrame en = new MapFrame();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;


                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.Insert();
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapExt en = new MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                if (DataType.IsNullOrEmpty(val.ToString()) == true)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            //执行保存，并统一生成PK的规则.
                            en.InitPK();
                            en.Save();
                        }
                        break;
                    case "Sys_MapAttr":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapAttr en = new MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            en.MyPK = en.FK_MapData + "_" + en.KeyOfEn;

                            //直接插入.
                            try
                            {
                                en.DirectInsert();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_GroupField":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            GroupField en = new GroupField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                try
                                {
                                    en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                                }
                                catch
                                {
                                    throw new Exception("val:" + val.ToString() + "oldMapID:" + oldMapID + "fk_mapdata:" + fk_mapdata);
                                }
                            }
                            int beforeID = en.OID;
                            en.OID = 0;
                            en.Insert();
                            endDoSQL += "@UPDATE Sys_MapAttr SET GroupID=" + en.OID + " WHERE FK_MapData='" + fk_mapdata + "' AND GroupID=" + beforeID;
                        }
                        break;
                    case "Sys_Enum":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                            if (se.IsExits)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnumMain sem = new Sys.SysEnumMain();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                sem.SetValByKey(dc.ColumnName, val);
                            }
                            if (sem.IsExits)
                                continue;
                            sem.Insert();
                        }
                        break;
                    case "WF_Node":
                        if (dt.Rows.Count > 0)
                        {
                            endDoSQL += "@UPDATE WF_Node SET FWCSta=2"
                                + ",FWC_X=" + dt.Rows[0]["FWC_X"]
                                + ",FWC_Y=" + dt.Rows[0]["FWC_Y"]
                                + ",FWC_H=" + dt.Rows[0]["FWC_H"]
                                + ",FWC_W=" + dt.Rows[0]["FWC_W"]
                                + ",FWCType=" + dt.Rows[0]["FWCType"]
                                + " WHERE NodeID=" + fk_mapdata.Replace("ND", "");
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion

            //执行最后结束的sql.
            DBAccess.RunSQLs(endDoSQL);

            MapData mdNew = new MapData(fk_mapdata);
            mdNew.RepairMap();
            mdNew.Update();
            return mdNew;
        }
        /// <summary>
        /// 修复map.
        /// </summary>
        public void RepairMap()
        {
            GroupFields gfs = new GroupFields(this.No);
            if (gfs.Count == 0)
            {
                GroupField gf = new GroupField();
                gf.FrmID = this.No;
                gf.Lab = this.Name;
                gf.Insert();

                string sqls = "";
                sqls += "@UPDATE Sys_MapDtl SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                //sqls += "@UPDATE Sys_MapFrame SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapM2M SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                DBAccess.RunSQLs(sqls);
            }
            else
            {
                if (SystemConfig.AppCenterDBType != DBType.Oracle)
                {
                    GroupField gfFirst = gfs[0] as GroupField;

                    string sqls = "";
                    sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gfFirst.OID + "       WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_MapAttr       WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE FrmID='" + this.No + "') or GroupID is null) AS X) AND FK_MapData='" + this.No + "' ";
                    sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gfFirst.OID + " WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_FrmAttachment WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE FrmID='" + this.No + "')) AS X) AND FK_MapData='" + this.No + "' ";

#warning 这些sql 对于Oracle 有问题，但是不影响使用.
                    try
                    {
                        DBAccess.RunSQLs(sqls);
                    }
                    catch
                    {
                    }
                }
            }

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (this.EnPK == "OID")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = "OID";
                    attr.Name = "OID";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }
            if (this.EnPK == "No" || this.EnPK == "MyPK")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, this.EnPK, MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = this.EnPK;
                    attr.Name = this.EnPK;
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, "RDT", MapAttrAttr.FK_MapData, this.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "RDT";
                attr.Name = "更新时间";
                attr.GroupID = 0;
                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }

            //检查特殊UIBindkey丢失的问题.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.No);

            foreach (MapAttr item in attrs)
            {
                if (item.LGType == FieldTypeS.Enum || item.LGType == FieldTypeS.FK)
                {
                    if (DataType.IsNullOrEmpty(item.UIBindKey) == true)
                    {
                        item.LGType = FieldTypeS.Normal;
                        item.UIContralType = UIContralType.TB;
                        item.Update();
                    }
                }
            }

        }
        protected override bool beforeInsert()
        {
            if (this.HisFrmType == FrmType.Url || this.HisFrmType == FrmType.Entity)
            {

            }
            else
            {
                this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            }
            return base.beforeInsert();
        }
        /// <summary>
        /// 设置Para 参数.
        /// </summary>
        public void ResetMaxMinXY()
        {
            if (this.HisFrmType != FrmType.FreeFrm)
                return;

            #region 计算最左边,与最右边的值。
            // 求最左边.
            float i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 == 0) /*没有线，只有图片的情况下。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);

            float i2 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 > i2)
                this.MaxLeft = i2;
            else
                this.MaxLeft = i1;

            // 求最右边.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(X2) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 == 0)
            {
                /*没有线的情况，按照图片来计算。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(X+W) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);
            }
            this.MaxRight = i1;

            // 求最top.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(Y1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            i2 = DBAccess.RunSQLReturnValFloat("SELECT MIN(Y)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);

            if (i1 > i2)
                this.MaxTop = i2;
            else
                this.MaxTop = i1;

            // 求最end.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            /*小周鹏添加2014/10/23-----------------------START*/
            if (i1 == 0) /*没有线，只有图片的情况下。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);

            /*小周鹏添加2014/10/23-----------------------END*/
            i2 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);
            if (i2 == 0)
                i2 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的 附件
            float endFrmAtt = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_FrmAttachment  WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的明细表
            float endFrmDtl = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_MapDtl  WHERE FK_MapData='" + this.No + "'", 0);

            //求出最底部的扩展控件
            float endFrmEle = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_FrmEle  WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的textbox
            float endFrmAttr = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+UIHeight)  FROM  Sys_MapAttr  WHERE FK_MapData='" + this.No + "' and UIVisible='1'", 0);

            if (i1 > i2)
                this.MaxEnd = i1;
            else
                this.MaxEnd = i2;

            this.MaxEnd = this.MaxEnd > endFrmAtt ? this.MaxEnd : endFrmAtt;
            this.MaxEnd = this.MaxEnd > endFrmDtl ? this.MaxEnd : endFrmDtl;
            this.MaxEnd = this.MaxEnd > endFrmEle ? this.MaxEnd : endFrmEle;
            this.MaxEnd = this.MaxEnd > endFrmAtt ? this.MaxEnd : endFrmAttr;

            #endregion

            this.DirectUpdate();
        }
        /// <summary>
        /// 求位移.
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanWeiYi(MapData md, float scrWidth)
        {
            if (scrWidth == 0)
                scrWidth = 900;

            float left = md.MaxLeft;
            if (left == 0)
            {
                md.ResetMaxMinXY();
                md.RetrieveFromDBSources();
                md.Retrieve();

                left = md.MaxLeft;
            }
            //取不到左侧参考值，则不进行位移
            if (left == 0)
                return left;

            float right = md.MaxRight;
            float withFrm = right - left;
            if (withFrm >= scrWidth)
            {
                /* 如果实际表单宽度大于屏幕宽度 */
                return -left;
            }

            //计算位移大小
            float space = (scrWidth - withFrm) / 2; //空白的地方.

            return -(left - space);
        }
        /// <summary>
        /// 求屏幕宽度
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanWidth(MapData md, float scrWidth)
        {
            if (scrWidth == 0)
                scrWidth = 900;
            float left = md.MaxLeft;
            if (left == 0)
            {
                md.ResetMaxMinXY();
                left = md.MaxLeft;
            }

            float right = md.MaxRight;
            float withFrm = right - left;
            if (withFrm >= scrWidth)
            {
                return withFrm;
            }
            return scrWidth;
        }
        /// <summary>
        /// 求屏幕高度
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanHeight(MapData md, float scrHeight)
        {
            if (scrHeight == 0)
                scrHeight = 1200;

            float end = md.MaxEnd;
            if (end > scrHeight)
                return end + 10;
            else
                return scrHeight;
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.HisFrmType == FrmType.Url || this.HisFrmType == FrmType.Entity)
                return base.beforeUpdateInsertAction();

            this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            MapAttrs.Retrieve(MapAttrAttr.FK_MapData, PTable);

            //更新版本号.
            this.Ver = DataType.CurrentDataTimess;

            #region  检查是否有ca认证设置.
            bool isHaveCA = false;
            foreach (MapAttr item in this.MapAttrs)
            {
                if (item.SignType == SignType.CA)
                {
                    isHaveCA = true;
                    break;
                }
            }
            this.IsHaveCA = isHaveCA;
            if (IsHaveCA == true)
            {
                //就增加隐藏字段.
                //MapAttr attr = new BP.Sys.MapAttr();
                // attr.MyPK = this.No + "_SealData";
                // attr.FK_MapData = this.No;
                // attr.HisEditType = BP.En.EditType.UnDel;
                //attr.KeyOfEn = "SealData";
                // attr.Name = "SealData";
                // attr.MyDataType = BP.DA.DataType.AppString;
                // attr.UIContralType = UIContralType.TB;
                //  attr.LGType = FieldTypeS.Normal;
                // attr.UIVisible = false;
                // attr.UIIsEnable = false;
                // attr.MaxLen = 4000;
                // attr.MinLen = 0;
                // attr.Save();
            }
            #endregion  检查是否有ca认证设置.

            return base.beforeUpdateInsertAction();
        }
        /// <summary>
        /// 更新版本
        /// </summary>
        public void UpdateVer()
        {
            string sql = "UPDATE Sys_MapData SET VER='" + BP.DA.DataType.CurrentDataTimess + "' WHERE No='" + this.No + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
        protected override bool beforeDelete()
        {
            string sql = "";
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData ='" + this.No + "'";
            DataTable Sys_MapDtl = DBAccess.RunSQLReturnTable(sql);

            string whereFK_MapData = "FK_MapData= '" + this.No + "' ";
            string whereEnsName = "FrmID= '" + this.No + "' ";
            string whereNo = "No='" + this.No + "' ";

            foreach (DataRow dr in Sys_MapDtl.Rows)
            {
                // ids += ",'" + dr["No"] + "'";
                whereFK_MapData += " OR FK_MapData='" + dr["No"] + "' ";
                whereEnsName += " OR FrmID='" + dr["No"] + "' ";
                whereNo += " OR No='" + dr["No"] + "' ";
            }

            //	string where = " FK_MapData IN (" + ids + ")";

            #region 删除相关的数据。
            sql = "DELETE FROM Sys_MapDtl WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmLine WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmEle WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmEvent WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmBtn WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmLab WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmLink WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmImg WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmImgAth WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmRB WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_FrmAttachment WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_MapM2M WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_MapFrame WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_MapExt WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_MapAttr WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_GroupField WHERE " + whereEnsName;
            sql += "@DELETE FROM Sys_MapData WHERE " + whereNo;
            sql += "@DELETE FROM Sys_MapM2M WHERE " + whereFK_MapData;
            sql += "@DELETE FROM Sys_M2M WHERE " + whereFK_MapData;
            sql += "@DELETE FROM WF_FrmNode WHERE FK_Frm='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmSln WHERE " + whereFK_MapData;
            DBAccess.RunSQLs(sql);
            #endregion 删除相关的数据。

            #region 删除物理表。
            //如果存在物理表.
            if (DBAccess.IsExitsObject(this.PTable) && this.PTable.IndexOf("ND") == 0)
            {
                //如果其他表单引用了该表，就不能删除它.
                sql = "SELECT COUNT(No) AS NUM  FROM Sys_MapData WHERE PTable='" + this.PTable + "' OR ( PTable='' AND No='" + this.PTable + "')";
                if (DBAccess.RunSQLReturnValInt(sql, 0) > 1)
                {
                    /*说明有多个表单在引用.*/
                }
                else
                {
                    // edit by zhoupeng 误删已经有数据的表.
                    if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + this.PTable + " WHERE 1=1 ") == 0)
                        DBAccess.RunSQL("DROP TABLE " + this.PTable);
                }
            }

            MapDtls dtls = new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
                dtl.Delete();

            #endregion

            return base.beforeDelete();
        }
        /// <summary>
        /// 生成自动的ｊｓ程序。
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <param name="tbPer"></param>
        /// <returns></returns>
        public static string GenerAutoFull(string pk, MapAttrs attrs, MapExt me, string tbPer)
        {
            string left = "\n document.forms[0]." + tbPer + "_TB" + me.AttrOfOper + "_" + pk + ".value = ";
            string right = me.Doc;
            foreach (MapAttr mattr in attrs)
            {
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tbPer + "_TB_" + mattr.KeyOfEn + "_" + pk + ".value) ");
            }
            return " alert( document.forms[0]." + tbPer + "_TB" + me.AttrOfOper + "_" + pk + ".value ) ; \t\n " + left + right;
        }
        #endregion 常用方法.

        #region 与Excel相关的操作 .
        /// <summary>
        /// 获得Excel文件流
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool ExcelGenerFile(string pkValue, ref byte[] bytes, string saveTo)
        {
            byte[] by = BP.DA.DBAccess.GetByteFromDB(this.PTable, this.EnPK, pkValue, saveTo);
            if (by != null)
            {
                bytes = by;
                return true;
            }
            else //说明当前excel文件没有生成.
            {
                string tempExcel = BP.Sys.SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\" + this.No + ".xlsx";
                if (System.IO.File.Exists(tempExcel) == true)
                {
                    bytes = BP.DA.DataType.ConvertFileToByte(tempExcel);
                    return false;
                }
                else //模板文件也不存在时
                {
                    throw new Exception("@没有找到模版文件." + tempExcel + " 请确认表单配置.");
                }
            }
        }
        /// <summary>
        /// 保存excel文件
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="bty"></param>
        public void ExcelSaveFile(string pkValue, byte[] bty, string saveTo)
        {
            BP.DA.DBAccess.SaveFileToDB(bty, this.PTable, this.EnPK, pkValue, saveTo);
        }
        #endregion 与Excel相关的操作 .

    }
    /// <summary>
    /// 映射基础s
    /// </summary>
    public class MapDatas : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 映射基础s
        /// </summary>
        public MapDatas()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapData();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapData> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapData>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapData> Tolist()
        {
            System.Collections.Generic.List<MapData> list = new System.Collections.Generic.List<MapData>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapData)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
