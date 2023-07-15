using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
    /// <summary>
    /// Wps表单 属性
    /// </summary>
    public class MapFrmWps : EntityNoName
    {
        #region 属性
        public string MyFileExt
        {
            get
            {
                return this.GetValStringByKey("MyFileExt");
            }
        }
        public string MyFilePath
        {
            get
            {
                return this.GetValStringByKey("MyFilePath");
            }
        }
        /// <summary>
        /// 是否是节点表单?
        /// </summary>
        public bool IsNodeFrm
        {
            get
            {
                if (this.No.Contains("ND") == false)
                    return false;

                if (this.No.Contains("Rpt") == true)
                    return false;

                if (this.No.Substring(0, 2) == "ND" && this.No.Contains("Dtl") == false)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 物理存储表
        /// </summary>
        public string PTable
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.PTable);
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// 节点ID.
        /// </summary>
        public int NodeID
        {
            get
            {
                if (this.No.IndexOf("ND") != 0)
                    return 0;
                return int.Parse(this.No.Replace("ND", ""));
            }
        }

        /// <summary>
        /// 表格显示的列
        /// </summary>
        public int TableCol
        {
            get
            {
                return 4;
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

        public string FK_FormTree
        {
            get
            {
                return this.GetValStringByKey(MapDataAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FormTree, value);
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

        #endregion

        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                //uac.OpenForSysAdmin();
                uac.OpenForAppAdmin();//2020.6.22zsy修改.
                uac.IsInsert = false;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 构造方法
        /// <summary>
        /// Wps表单属性
        /// </summary>
        public MapFrmWps()
        {
        }
        /// <summary>
        /// Wps表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmWps(string no)
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

                Map map = new Map("Sys_MapData", "Wps表单属性");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.SetHelperUrl(MapDataAttr.No, "xxxx");

                if (BP.WF.Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(MapDataAttr.PTable, null, "存储表", false, false, 0, 100, 20);
                }
                else
                {
                    map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 100, 20);
                    string msg = "提示:";
                    msg += "\t\n1. 该表单把数据存储到那个表里.";
                    msg += "\t\n2. 该表必须有一个int64未的OID列作为主键..";
                    msg += "\t\n3. 如果指定了一个不存在的表,系统就会自动创建上.";
                    map.SetHelperAlert(MapDataAttr.PTable, msg);
                }

                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20, true);
                map.AddTBInt(MapDataAttr.TableCol, 0, "显示列数", false, false);

                map.AddTBInt(MapDataAttr.FrmW, 900, "表单宽度", true, false);

                if (BP.WF.Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                }
                else
                {
                    map.AddTBString(MapDataAttr.DBSrc, null, "数据源", false, false, 0, 500, 20);
                    // map.AddDDLEntities(MapDataAttr.DBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                    map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                }

                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型",
                    true, true, MapDataAttr.FrmType);

                //表单解析 0 普通 1 页签展示
                map.AddDDLSysEnum(MapDataAttr.FrmShowType, 0, "表单展示方式", true, true, "表单展示方式",
                    "@0=普通方式@1=页签方式");
                map.AddBoolean("IsEnableJs", false, "是否启用自定义js函数？", true, true, true);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 400, 100, true);
                //增加参数字段.
                map.AddTBAtParas(4000);
                #endregion 设计者信息.

                map.AddMyFile("wps文件模板", null, BP.Difference.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\");


                #region 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "装载填充"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
                // rm.Icon = "../../WF/Img/FullData.png";
                rm.Icon = "icon-reload";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Icon = "icon-energy";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量修改字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-calculator";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机端表单";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.ClassMethodName = this.ToString() + ".MobileFrmDesigner";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-screen-smartphone";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "隐藏字段";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.Icon = "icon-list";
                rm.ClassMethodName = this.ToString() + ".FrmHiddenField";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单body属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBodyAttr";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Icon = "icon-social-spotify";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "导出模版"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExp";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "../../WF/Img/Export.png";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "导出到xml";
                rm.Target = "_blank";
                rm.Icon = "icon-social-spotify";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "重命名字段";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                rm.Icon = "../../WF/Img/ReName.png";
                rm.Icon = "icon-refresh";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单检查"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCheckFixFrmForUpdateVer";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "表单检查";
                rm.Icon = "../../WF/Img/Check.png";
                rm.Target = "_blank";
                rm.Icon = "icon-eye";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "Tab顺序键"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoTabIdx";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-list";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "模板打印";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-printer";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "模板打印2019";
                rm.ClassMethodName = this.ToString() + ".DoBill2019";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-printer";
                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "参考面板";
                rm.ClassMethodName = this.ToString() + ".DoRefPanel";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-grid";
                map.AddRefMethod(rm);
                #endregion 方法 - 基本功能.

                #region 高级功能.
                rm = new RefMethod();
                rm.Title = "改变表单类型";
                rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "修改表单类型", true, true);
                rm.Icon = "icon-refresh";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "Wps表单设计";
                rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                //rm.Icon = "../../WF/Img/FileType/xlsx.gif";
                rm.Icon = "icon-note";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);


                //平铺模式.
                if (BP.WF.Glo.CCBPMRunModel != CCBPMRunModel.Single)
                {
                    map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.FrmOrgs(),
                        new BP.WF.Port.Admin2Group.Orgs(),
                        BP.WF.Template.FrmOrgAttr.FrmID,
                        BP.WF.Template.FrmOrgAttr.OrgNo, "适用组织");
                }
                #endregion

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "一键设置表单元素只读";
                rm.Warning = "您确定要设置吗？所有的元素，包括字段、从表、附件以及其它组件都将会被设置为只读的.";
                rm.GroupName = "实验中的功能";
                //rm.Icon = "../../WF/Img/RegularExpression.png";
                rm.ClassMethodName = this.ToString() + ".DoOneKeySetReadonly";
                rm.RefMethodType = RefMethodType.Func;
                rm.Icon = "icon-settings";
                map.AddRefMethod(rm);

                #endregion 实验中的功能

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.No);
            base.afterDelete();
        }
        #endregion

        #region 高级设置.
        /// <summary>
        /// 一键设置为只读.
        /// </summary>
        /// <returns></returns>
        public string DoOneKeySetReadonly()
        {
            BP.Sys.CCFormAPI.OneKeySetFrmEleReadonly(this.No);
            return "设置成功.";
        }
        /// <summary>
        /// 改变表单类型 @李国文 ，需要搬到jflow.并测试.
        /// </summary>
        /// <param name="val">要改变的类型</param>
        /// <returns></returns>
        public string DoChangeFrmType(int val)
        {
            MapData md = new MapData(this.No);
            string str = "原来的是:" + md.HisFrmTypeText + "类型，";
            md.HisFrmTypeInt = val;
            str += "现在修改为：" + md.HisFrmTypeText + "类型";
            md.Update();

            return str;
        }
        #endregion 高级设置.

        protected override bool beforeUpdate()
        {
            if (this.NodeID != 0)
                this.FK_FormTree = "";

            return base.beforeUpdate();
        }
        protected override void afterUpdate()
        {
            //修改关联明细表,, 如果是从表.
            MapDtl dtl = new MapDtl();
            dtl.No = this.No;
            if (dtl.RetrieveFromDBSources() == 1)
            {
                dtl.Name = this.Name;
                dtl.PTable = this.PTable;
                dtl.DirectUpdate();

                MapData map = new MapData(this.No);
                //避免显示在表单库中
                // map.FK_FrmSort = "";
                map.FK_FormTree = "";
                map.DirectUpdate();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.No);

            base.afterUpdate();
        }
        #region 节点表单方法.
        public string DoTabIdx()
        {
            return "../../Admin/FoolFormDesigner/TabIdx.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/FoolFormDesigner/PrintTemplate/Default.htm?FK_MapData=" + this.No + "&FrmID=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;

           // return "../../Admin/AttrNode/Bill.htm?FK_MapData=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 隐藏字段.
        /// </summary>
        /// <returns></returns>
        public string FrmHiddenField()
        {
            return "../../Admin/CCFormDesigner/DialogCtr/FrmHiddenField.htm?FK_MapData=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill2019()
        {
            return "../../Admin/AttrNode/Bill2019.htm?FK_MapData=" + this.No + "&FrmID=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;
        }

        /// <summary>
        /// Wps表单设计器
        /// </summary>
        /// <returns></returns>
        public string DoDesignerFool()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsFirst=1&IsEditMapData=True";
        }

        /// <summary>
        /// 节点表单组件
        /// </summary>
        /// <returns></returns>
        public string DoNodeFrmCompent()
        {
            if (this.No.Contains("ND") == true)
                return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=" + this.No.Replace("ND", "") + "&t=" + DataType.CurrentDateTime;
            else
                return "../../Admin/FoolFormDesigner/Do.htm&DoType=FWCShowError";
        }
        #endregion

        #region 通用方法.
        /// <summary>
        /// 替换名称
        /// </summary>
        /// <param name="fieldOldName">旧名称</param>
        /// <param name="newField">新字段</param>
        /// <param name="newFieldName">新字段名称(可以为空)</param>
        /// <returns></returns>
        public string DoChangeFieldName(string fieldOld, string newField, string newFieldName)
        {
            MapFrmFool en = new MapFrmFool(this.No);
            return en.DoChangeFieldName(fieldOld, newField, newFieldName);
        }
        /// <summary>
        /// 批量设置正则表达式规则.
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpressionBatch()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionBatch.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 批量修改字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchEditAttr()
        {
            return "../../Admin/FoolFormDesigner/FieldTypeListBatch.htm?FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string MobileFrmDesigner()
        {
            return "../../Admin/MobileFrmDesigner/Default.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDFrom()
        {
            string url = "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&Token=" + Web.WebUser.Token + "&AppCenterDBType=" + DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Difference.SystemConfig.CustomerNo;
            return url;
        }
        /// <summary>
        /// 设计Wps表单
        /// </summary>
        /// <returns></returns>
        public string DoDFromCol4()
        {
            string url = "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&Token=" + Web.WebUser.Token + "&IsFirst=1&AppCenterDBType=" + DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Difference.SystemConfig.CustomerNo;
            return url;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch()
        {
            return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 参考面板
        /// </summary>
        /// <returns></returns>
        public string DoRefPanel()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.Frm.MapFrmReferencePanel&PKVal=" + this.No;
        }
        /// <summary>
        /// 调用分析API
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            return "../../Comm/Group.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoDBSrc()
        {
            return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=BP.Sys.SFDBSrcs";
        }

        public string DoPageLoadFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PageLoadFull.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        public string DoInitScript()
        {
            return "../../Admin/FoolFormDesigner/MapExt/InitScript.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// Wps表单属性.
        /// </summary>
        /// <returns></returns>
        public string DoBodyAttr()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BodyAttr.htm?s=34&FK_MapData=" + this.No + "&ExtType=BodyAttr&RefNo=";
        }
        /// <summary>
        /// 表单事件
        /// </summary>
        /// <returns></returns>
        public string DoEvent()
        {
            return "../../Admin/CCFormDesigner/Action.htm?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
        }

        /// <summary>
        /// 导出表单
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            return "../../Admin/FoolFormDesigner/ImpExp/Exp.htm?FK_MapData=" + this.No;
        }
        #endregion 方法.
    }
    /// <summary>
    /// Wps表单属性s
    /// </summary>
    public class MapFrmWpss : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// Wps表单属性s
        /// </summary>
        public MapFrmWpss()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmWps();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmWps> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmWps>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmWps> Tolist()
        {
            System.Collections.Generic.List<MapFrmWps> list = new System.Collections.Generic.List<MapFrmWps>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmWps)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
