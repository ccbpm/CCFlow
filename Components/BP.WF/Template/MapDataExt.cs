﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Sys;
using System.Collections.Generic;

namespace BP.WF.Template
{
    /// <summary>
    /// 表单属性
    /// </summary>
    public class MapDataExt : EntityNoName
    {
        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
                return uac;
            }
        }
        #endregion 权限控制.

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
        /// <summary>
        /// 左边界.
        /// </summary>
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
        /// <summary>
        /// 右边界
        /// </summary>
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
        /// <summary>
        /// 最高top
        /// </summary>
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
        /// <summary>
        /// 最低
        /// </summary>
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
                return this.GetParaString(MapDataAttr.RptSearchKeys,"*");
            }
            set
            {
                this.SetPara(MapDataAttr.RptSearchKeys, value);
            }
        }
        #endregion 报表属性(参数方式存储).

        #region 外键属性
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
                FrmBtns obj = this.GetRefObject("FrmBtns") as FrmBtns;
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
                FrmImgs obj = this.GetRefObject("FrmImgs") as FrmImgs;
                if (obj == null)
                {
                    obj = new FrmImgs(this.No);
                    this.SetRefObject("FrmImgs", obj);
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
            }
        }
        public AppType HisAppType
        {
            get
            {
                return (AppType)this.GetValIntByKey(MapDataAttr.AppType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.AppType, (int)value);
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
                if (string.IsNullOrEmpty(s))
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
        ///// <summary>
        ///// 表单控制方案
        ///// </summary>
        //public string Slns
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(MapDataAttr.Slns);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MapDataAttr.Slns, value);
        //    }
        //}
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
        /// 表格显示的列
        /// </summary>
        public int TableCol
        {
            get
            {
                int i = this.GetValIntByKey(MapDataAttr.TableCol);
                if (i == 0 || i == 1)
                    return 4;
                return i;
            }
            set
            {
                this.SetValByKey(MapDataAttr.TableCol, value);
            }
        }
        public string TableWidth
        {
            get
            {
                //switch (this.TableCol)
                //{
                //    case 2:
                //        return
                //        labCol = 25;
                //        ctrlCol = 75;
                //        break;
                //    case 4:
                //        labCol = 20;
                //        ctrlCol = 30;
                //        break;
                //    case 6:
                //        labCol = 15;
                //        ctrlCol = 30;
                //        break;
                //    case 8:
                //        labCol = 10;
                //        ctrlCol = 15;
                //        break;
                //    default:
                //        break;
                //}


                int i = this.GetValIntByKey(MapDataAttr.TableWidth);
                if (i <= 50)
                    return "100%";
                return i + "px";
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单属性
        /// </summary>
        public MapDataExt()
        {
        }
        /// <summary>
        /// 表单属性
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapDataExt(string no)
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
                Map map = new Map("Sys_MapData", "表单属性");
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, false, 1, 200, 20);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20);

                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, 1, "表单类型",
                    true, false, MapDataAttr.FrmType, 
                    "@0=傻瓜表单@1=自由表单@2=Silverlight表单(已取消)@3=嵌入式表单@4=Word表单@5=Excel表单");

                map.AddTBString(MapDataAttr.Url, null, "URL连接(对嵌入式表单有效)", true, false, 0, 500, 20, true);
                //数据源.
                map.AddDDLEntities(MapDataAttr.DBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20,true);

                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20,false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);

                map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false,true);


                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                //查询条件.
                map.AddSearchAttr(MapDataAttr.DBSrc);

                //RefMethod rm = new RefMethod();
                //rm.Title = "设计自由表单"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoDFrom";
                //rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Form.png";
                //rm.Visable = true;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设计傻瓜表单"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoDFromCol4";
                //rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Form.png";
                //rm.Visable = true;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                #region 方法 - 基本功能.
                RefMethod rm = new RefMethod();
                rm.Title = "装载填充"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/FullData.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "批量设置验证规则";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                
                rm = new RefMethod();
                rm.Title = "手机端表单";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "内置JavaScript脚本"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoInitScript";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单body属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBodyAttr";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "导出XML表单模版"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExp";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Export.png";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "导出到xml";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 方法 - 基本功能.


                //rm = new RefMethod();
                //rm.Title = "扩展设置"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoMapExt";
                //rm.Icon = "/WF/Img/Setting.png";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                #region 方法 - 开发接口.
                rm = new RefMethod();
                rm.Title = "调用查询API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "调用分析API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoGroup";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);
                #endregion 方法 - 开发接口.



                //rm = new RefMethod();
                //rm.Title = "Word表单属性"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoWordFrm";
                //rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Btn/Word.gif";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "Excel表单属性"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoExcelFrm";
                //rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Btn/Excel.gif";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "数据源管理"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                //rm.Icon = "/WF/Img/DB.png";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Visable = true;
                //rm.RefAttrLinkLabel = "数据源管理";
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        /// <summary>
        /// 批量设置正则表达式规则.
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpressionBatch()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/RegularExpressionBatch.aspx?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrNode/SortingMapAttrs.aspx?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
          /// <summary>
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDFrom()
        {
            string url = SystemConfig.CCFlowWebPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            PubClass.WinOpen(url, 800, 650);
            return null;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoDFromCol4()
        {
            string url = SystemConfig.CCFlowWebPath + "WF/MapDef/MapDef.aspx?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            PubClass.WinOpen(url, 800, 650);
            return null;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Search.aspx?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 调用分析API
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Group.aspx?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoDBSrc()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Search.aspx?s=34&FK_MapData=" + this.No + "&EnsName=BP.Sys.SFDBSrcs";
        }
        public string DoWordFrm()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/WordFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=WordFrm&RefNo=";
        }
        public string DoExcelFrm()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/ExcelFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=ExcelFrm&RefNo=";
        }
        public string DoPageLoadFull()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/PageLoadFull.aspx?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        public string DoInitScript()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/InitScript.aspx?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// 表单属性.
        /// </summary>
        /// <returns></returns>
        public string DoBodyAttr()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/BodyAttr.aspx?s=34&FK_MapData=" + this.No + "&ExtType=BodyAttr&RefNo=";
        }
        /// <summary>
        /// 表单事件
        /// </summary>
        /// <returns></returns>
        public string DoEvent()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/Action.aspx?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoMapExt()
        {
            return SystemConfig.CCFlowWebPath + "WF/MapDef/MapExt/List.aspx?FK_MapData=" + this.No + "&T=sd";
        }
        /// <summary>
        /// 导出表单
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            string urlExt = SystemConfig.CCFlowWebPath + "WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + this.No;
            PubClass.WinOpen(urlExt, 900, 1000);
            return null;
        }
        #endregion 方法.

    }
    /// <summary>
    /// 表单属性s
    /// </summary>
    public class MapDataExts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 表单属性s
        /// </summary>
        public MapDataExts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDataExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDataExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDataExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDataExt> Tolist()
        {
            System.Collections.Generic.List<MapDataExt> list = new System.Collections.Generic.List<MapDataExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDataExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
