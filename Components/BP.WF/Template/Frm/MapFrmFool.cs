using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
    /// <summary>
    /// 傻瓜表单属性
    /// </summary>
    public class MapFrmFool : EntityNoName
    {
        #region 属性
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
                if (BP.Web.WebUser.IsAdmin == true)
                {
                    uac.OpenForAppAdmin();//2020.6.22 zsy 修改.
                    if (this.No.StartsWith("ND") == true)
                        uac.IsDelete = false;
                    uac.IsInsert = false;
                }
                else
                {
                    throw new Exception("err@非法用户,只有管理员才可以操作.");
                }
                return uac;
            }
        }
        #endregion 权限控制.

        #region 构造方法
        /// <summary>
        /// 傻瓜表单属性
        /// </summary>
        public MapFrmFool()
        {
        }
        /// <summary>
        /// 傻瓜表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmFool(string no)
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

                Map map = new Map("Sys_MapData", "傻瓜表单属性");

                #region 基本属性.
                map.AddGroupAttr("基本属性");
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
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
                map.AddTBString(MapDataAttr.Name, null, "名称", true, false, 0, 500, 20, true);
                map.AddTBInt(MapDataAttr.TableCol, 0, "显示列数", false, false);
                map.AddTBInt(MapDataAttr.FrmW, 900, "表单宽度", true, false);
                if (BP.WF.Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                }
                else
                {
                    map.AddTBString(MapDataAttr.DBSrc, null, "数据源", false, false, 0, 500, 20);
                    map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                }

                //表单解析 0 普通 1 页签展示
                map.AddDDLSysEnum(MapDataAttr.FrmShowType, 0, "展示方式", true, true, "表单展示方式",
                    "@0=普通方式@1=页签方式");

                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型",
                    true, true, MapDataAttr.FrmType);

                map.AddTBString(MapDataAttr.UrlExt, null, "自定义URL", true, false, 0, 300, 20, true);
                map.AddTBString(MapDataAttr.Icon, "icon-doc", "图标", true, false, 0, 100, 100, true);

                map.AddBoolean("IsEnableJs", false, "是否启用自定义js函数？", true, true, true);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddGroupAttr("设计者信息");
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 400, 100, true);
                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                #region 基本功能.
                map.AddGroupMethod("基本功能");

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
                map.AddGroupMethod("高级功能");
                rm = new RefMethod();
                rm.Title = "版本管理"; // "设计表单";
               // rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoMapDataVer";
                //  rm.Icon = "../../WF/Img/Ver.png";
                rm.Icon = "icon-social-dropbox";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "改变表单类型";
               // rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "修改表单类型", true, true);
                rm.Icon = "icon-refresh";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "傻瓜表单设计";
               // rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                //rm.Icon = "../../WF/Img/FileType/xlsx.gif";
                rm.Icon = "icon-note";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "JS编程"; // "设计表单";
             //   rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoInitScript";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Icon = "icon-social-dropbox";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                //平铺模式.
                if (BP.WF.Glo.CCBPMRunModel != CCBPMRunModel.Single)
                {
                    map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.FrmOrgs(),
                        new BP.WF.Port.Admin2Group.Orgs(),
                        BP.WF.Template.FrmOrgAttr.FrmID,
                        BP.WF.Template.FrmOrgAttr.OrgNo, "适用组织");
                }

                rm = new RefMethod();
                rm.Title = "特别控件特别用户权限";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/SpecUserSpecFields.png";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-note";
                map.AddRefMethod(rm);
                #endregion

                #region 开发接口.
                map.AddGroupMethod("开发接口");
                rm = new RefMethod();
                rm.Title = "调用查询API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                //rm.Icon = "../../WF/Img/Table.gif";
                rm.Icon = "icon-magnifier";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
             //   rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "调用分析API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoGroup";
                rm.Icon = "icon-chart";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
             //   rm.GroupName = "开发接口";
                map.AddRefMethod(rm);
                #endregion 方法 - 开发接口.

                #region 实验中的功能
                map.AddGroupMethod("实验中的功能");
                //rm = new RefMethod();
                //rm.Title = "批量设置验证规则";
                //rm.GroupName = "实验中的功能";
                ////rm.Icon = "../../WF/Img/RegularExpression.png";
                //rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-settings";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "一键设置表单元素只读";
                rm.Warning = "您确定要设置吗？所有的元素，包括字段、从表、附件以及其它组件都将会被设置为只读的.";
              //  rm.GroupName = "实验中的功能";
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
            //注册事件表单实体.
            //BP.Sys.Base.FormEventBase feb = BP.Sys.Base.Glo.GetFormEventBaseByEnName(this.No);
            //if (feb == null)
            //    this.FromEventEntity = "";
            //else
            //    this.FromEventEntity = feb.ToString();

            if (this.NodeID != 0)
                this.FK_FormTree = "";

            return base.beforeUpdate();
        }
        protected override void afterUpdate()
        {
            //修改关联明细表
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
        /// <summary>
        /// 版本管理
        /// </summary>
        /// <returns></returns>
        public string DoMapDataVer()
        {
            return BP.Difference.SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapDataVer.htm?FK_MapData=" + this.No + "&FrmID=" + this.No;
        }
        /// <summary>
        /// 顺序
        /// </summary>
        /// <returns></returns>
        public string DoTabIdx()
        {
            return BP.Difference.SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/TabIdx.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/FoolFormDesigner/PrintTemplate/Default.htm?FK_MapData=" + this.No + "&FrmID=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;

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
        /// 傻瓜表单设计器
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
        /// <summary>
        /// 执行旧版本的兼容性检查.
        /// </summary>
        public string DoCheckFixFrmForUpdateVer()
        {
            // 更新状态.
            DBAccess.RunSQL("UPDATE Sys_GroupField SET CtrlType='' WHERE CtrlType IS NULL");
            DBAccess.RunSQL("UPDATE Sys_GroupField SET CtrlID='' WHERE CtrlID IS NULL");
            DBAccess.RunSQL("UPDATE Sys_GroupField SET CtrlID='' WHERE CtrlID IS NULL");


            //更新GroupFieldID
            string sql = "";
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                    sql = "UPDATE Sys_MapDtl, Sys_GroupField B SET Sys_MapDtl.GroupField=B.OID WHERE Sys_MapDtl.No=B.CtrlID AND Sys_MapDtl.GroupField=''";
                    break;
                case DBType.MSSQL:
                default:
                    sql = "UPDATE Sys_MapDtl SET Sys_MapDtl.GroupField=Sys_GroupField.OID FROM Sys_GroupField WHERE Sys_MapDtl.No=Sys_GroupField.CtrlID AND Sys_MapDtl.GroupField=''";
                    break;
            }
            DBAccess.RunSQL(sql);


            //删除重影数据.
            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE CtrlType='FWC' and CTRLID is null");

            //一直遇到遇到自动变长的问题, 强制其修复过来.
            DBAccess.RunSQL("UPDATE Sys_Mapattr SET colspan=3 WHERE UIHeight<=38 AND colspan=4");

            string str = "";

            //处理失去分组的字段. 
            string sqlOfOID = " CAST(OID as VARCHAR(50)) ";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                sqlOfOID = " CAST(OID as CHAR) ";

            sql = "SELECT MyPK FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "' AND GroupID NOT IN (SELECT " + sqlOfOID + " FROM Sys_GroupField WHERE FrmID='" + this.No + "' AND ( CtrlType='' OR CtrlType IS NULL)  )  OR GroupID IS NULL ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
            {
                GroupField gf = null;
                GroupFields gfs = new GroupFields(this.No);
                foreach (GroupField mygf in gfs)
                {
                    if (mygf.CtrlID == "")
                        gf = mygf;
                }

                if (gf == null)
                {
                    gf = new GroupField();
                    gf.Lab = "基本信息";
                    gf.FrmID = this.No;
                    gf.Insert();
                }

                //设置 GroupID
                foreach (DataRow dr in dt.Rows)
                {
                    DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE MyPK='" + dr[0].ToString() + "'");
                }
            }

            //从表.
            MapDtls dtls = new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                GroupField gf = new GroupField();
                int i = gf.Retrieve(GroupFieldAttr.CtrlID, dtl.No, GroupFieldAttr.FrmID, this.No);
                if (i == 1)
                    continue;

                //GroupField gf = new GroupField();
                //if (gf.IsExit(GroupFieldAttr.CtrlID, dtl.No) == true && !DataType.IsNullOrEmpty(gf.CtrlType))
                //    continue;

                gf.Lab = dtl.Name;
                gf.CtrlID = dtl.No;
                gf.CtrlType = "Dtl";
                gf.FrmID = dtl.FK_MapData;
                gf.Save();
                str += "@为从表" + dtl.Name + " 增加了分组.";
            }

            // 框架.
            MapFrames frams = new MapFrames(this.No);
            foreach (MapFrame fram in frams)
            {
                GroupField gf = new GroupField();
                int i = gf.Retrieve(GroupFieldAttr.CtrlID, fram.MyPK, GroupFieldAttr.FrmID, this.No);
                if (i == 1)
                    continue;

                gf.Lab = fram.Name;
                gf.CtrlID = fram.MyPK;
                gf.CtrlType = "Frame";
                gf.EnName = fram.FK_MapData;
                gf.Insert();

                str += "@为框架 " + fram.Name + " 增加了分组.";
            }

            // 附件.
            FrmAttachments aths = new FrmAttachments(this.No);
            foreach (FrmAttachment ath in aths)
            {
                //单附件、不可见的附件，都不需要增加分组.
                if (ath.IsVisable == false || ath.UploadType == AttachmentUploadType.Single)
                    continue;


                GroupField gf = new GroupField();
                int i = gf.Retrieve(GroupFieldAttr.CtrlID, ath.MyPK, GroupFieldAttr.FrmID, this.No);
                if (i == 1)
                    continue;

                gf.Lab = ath.Name;
                gf.CtrlID = ath.MyPK;
                gf.CtrlType = "Ath";
                gf.FrmID = ath.FK_MapData;
                gf.Insert();

                str += "@为附件 " + ath.Name + " 增加了分组.";
            }

            if (this.IsNodeFrm == true)
            {
                //提高执行效率.
                // FrmNodeComponent conn = new FrmNodeComponent(this.NodeID);
                //  conn.InitGroupField();
                //conn.Update();
            }


            //删除重复的数据, 比如一个从表显示了多个分组里. 增加此部分.
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                sql = "SELECT * FROM (SELECT FrmID,CtrlID,CtrlType, count(*) as Num FROM sys_groupfield WHERE CtrlID!='' GROUP BY FrmID,CtrlID,CtrlType ) WHERE Num > 1";
            else
                sql = "SELECT * FROM (SELECT FrmID,CtrlID,CtrlType, count(*) as Num FROM sys_groupfield WHERE CtrlID!='' GROUP BY FrmID,CtrlID,CtrlType ) AS A WHERE A.Num > 1";

            dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string enName = dr[0].ToString();
                string ctrlID = dr[1].ToString();
                string ctrlType = dr[2].ToString();

                GroupFields gfs = new GroupFields();
                gfs.Retrieve(GroupFieldAttr.FrmID, enName, GroupFieldAttr.CtrlID, ctrlID, GroupFieldAttr.CtrlType, ctrlType);

                if (gfs.Count <= 1)
                    continue;
                foreach (GroupField gf in gfs)
                {
                    gf.Delete(); //删除其中的一个.
                    break;
                }
            }

            if (str == "")
                return "检查成功.";

            return str + ", @@@ 检查成功。";
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
            MapAttr attrOld = new MapAttr();
            attrOld.setKeyOfEn(fieldOld);
            attrOld.setFK_MapData(this.No);
            attrOld.setMyPK(attrOld.FK_MapData + "_" + attrOld.KeyOfEn);
            if (attrOld.RetrieveFromDBSources() == 0)
                return "@旧字段输入错误[" + attrOld.KeyOfEn + "].";

            //检查是否存在该字段？
            MapAttr attrNew = new MapAttr();
            attrNew.setKeyOfEn(newField);
            attrNew.setFK_MapData(this.No);
            attrNew.setMyPK(attrNew.FK_MapData + "_" + attrNew.KeyOfEn);
            if (attrNew.RetrieveFromDBSources() == 1)
                return "@该字段[" + attrNew.KeyOfEn + "]已经存在.";

            //删除旧数据.
            attrOld.Delete();

            //copy这个数据,增加上它.
            attrNew.Copy(attrOld);
            attrNew.setKeyOfEn(newField);
            attrNew.setFK_MapData(this.No);

            if (newFieldName != "")
                attrNew.Name = newFieldName;

            attrNew.Insert();
            if (this.No.StartsWith("ND") == false)
            {
                //修改对应的数据库字段名
                DBAccess.RenameTableField(this.PTable, fieldOld, newField);
            }
            else
            {
                string strs = this.No.Replace("ND", "");
                strs = strs.Substring(0, strs.Length - 2);
                string rptTable = "ND" + strs + "Rpt";
                MapDatas mds = new MapDatas();
                mds.Retrieve(MapDataAttr.No, rptTable);
                if (mds.Count == 0)
                {
                    string sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "',  MyPK='" + rptTable + "_" + newField + "' WHERE KeyOfEn='" + fieldOld + "' AND FK_MapData='" + rptTable + "'";
                    DBAccess.RenameTableField(rptTable, fieldOld, newField);
                }

                foreach (MapData item in mds)
                {
                    string sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "',  MyPK='" + item.No + "_" + newField + "' WHERE KeyOfEn='" + fieldOld + "' AND FK_MapData='" + item.No + "'";
                    DBAccess.RunSQL(sql);
                    DBAccess.RenameTableField(item.PTable, fieldOld, newField);
                }

            }
            //更新处理他的相关业务逻辑.
            MapExts exts = new MapExts(this.No);
            foreach (MapExt item in exts)
            {
                item.setMyPK(item.MyPK.Replace("_" + fieldOld, "_" + newField));

                if (item.AttrOfOper == fieldOld)
                    item.AttrOfOper = newField;

                if (item.AttrsOfActive == fieldOld)
                    item.AttrsOfActive = newField;

                item.Tag = item.Tag.Replace(fieldOld, newField);
                item.Tag1 = item.Tag1.Replace(fieldOld, newField);
                item.Tag2 = item.Tag2.Replace(fieldOld, newField);
                item.Tag3 = item.Tag3.Replace(fieldOld, newField);

                item.AtPara = item.AtPara.Replace(fieldOld, newField);
                item.Doc = item.Doc.Replace(fieldOld, newField);
                item.Save();
            }

            //如果是开发者表单需要替换开发者表单的Html
            if (this.HisFrmType == FrmType.Develop)
            {
                string devHtml = DBAccess.GetBigTextFromDB("Sys_MapData", "No", this.No, "HtmlTemplateFile");
                if (DataType.IsNullOrEmpty(devHtml) == true)
                    return "执行成功";
                string prefix = "TB_";
                //外部数据源、外键、枚举下拉框
                if ((attrNew.LGType == FieldTypeS.Normal && attrNew.MyDataType == DataType.AppString && attrNew.UIContralType == UIContralType.DDL)
                    || (attrNew.LGType == FieldTypeS.FK && attrNew.MyDataType == DataType.AppString)
                    || (attrNew.LGType == FieldTypeS.Enum && attrNew.UIContralType == UIContralType.DDL))
                {
                    devHtml = devHtml.Replace("id=\"DDL_" + attrOld.KeyOfEn + "\"", "id=\"DDL_" + attrNew.KeyOfEn + "\"")
                                .Replace("id=\"SS_" + attrOld.KeyOfEn + "\"", "id=\"SS_" + attrNew.KeyOfEn + "\"")
                               .Replace("name=\"DDL_" + attrOld.KeyOfEn + "\"", "name=\"DDL_" + attrNew.KeyOfEn + "\"")
                               .Replace("data-key=\"" + attrOld.KeyOfEn + "\"", "data-key=\"" + attrNew.KeyOfEn + "\"")
                               .Replace(">" + attrOld.KeyOfEn + "</option>", ">" + attrNew.KeyOfEn + "</option>");

                    //保存开发者表单数据
                    BP.WF.Dev2Interface.SaveDevelopForm(devHtml, this.No);
                    return "执行成功";
                }
                //枚举
                if (attrNew.LGType == FieldTypeS.Enum)
                {
                    if (DataType.IsNullOrEmpty(attrNew.UIBindKey) == true)
                        throw new Exception("err@" + attrNew.Name + "枚举字段绑定的枚举为空,请检查该字段信息是否发生变更");
                    //根据绑定的枚举获取枚举值
                    SysEnums enums = new SysEnums(attrNew.UIBindKey);
                    if (attrNew.UIContralType == UIContralType.CheckBok)
                    {
                        prefix = "CB_";
                        devHtml = devHtml.Replace("id=\"SC_" + attrOld.KeyOfEn + "\"", "id=\"SC_" + attrNew.KeyOfEn + "\"");
                    }
                    if (attrNew.UIContralType == UIContralType.RadioBtn)
                    {
                        prefix = "RB_";
                        devHtml = devHtml.Replace("id=\"SR_" + attrOld.KeyOfEn + "\"", "id=\"SR_" + attrNew.KeyOfEn + "\"");
                    }
                    foreach (SysEnum item in enums)
                    {
                        devHtml = devHtml.Replace("id=\"" + prefix + attrOld.KeyOfEn + "_" + item.IntKey + "\"", "id=\"" + prefix + attrNew.KeyOfEn + "_" + item.IntKey + "\"")
                            .Replace("name=\"" + prefix + attrOld.KeyOfEn + "\"", "name=\"" + prefix + attrNew.KeyOfEn + "\"")
                            .Replace("data-key=\"" + attrOld.KeyOfEn + "\"", "data-key=\"" + attrNew.KeyOfEn + "\"");
                    }
                    //保存开发者表单数据
                    BP.WF.Dev2Interface.SaveDevelopForm(devHtml, this.No);
                    return "执行成功";
                }
                //普通字段
                if (attrNew.LGType == FieldTypeS.Normal)
                {
                    prefix = "TB_";
                    if (attrNew.MyDataType == DataType.AppBoolean)
                        prefix = "CB_";
                    devHtml = devHtml.Replace("id=\"" + prefix + attrOld.KeyOfEn + "\"", "id=\"" + prefix + attrNew.KeyOfEn + "\"")
                                .Replace("name=\"" + prefix + attrOld.KeyOfEn + "\"", "name=\"" + prefix + attrNew.KeyOfEn + "\"")
                                .Replace("data-key=\"" + attrOld.KeyOfEn + "\"", "data-key=\"" + attrNew.KeyOfEn + "\"")
                                .Replace("data-name=\"" + attrOld.Name + "\"", "data-name=\"" + attrNew.Name + "\"");
                }
                //保存开发者表单数据
                BP.WF.Dev2Interface.SaveDevelopForm(devHtml, this.No);
                return "执行成功";
            }
            return "执行成功";
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
        /// 设计傻瓜表单
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

        public string DoSpecFieldsSpecUsers()
        {
            return "../../Admin/FoolFormDesigner/SepcFiledsSepcUsers.htm?FrmID=" +
                   this.No + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 傻瓜表单属性.
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
    /// 傻瓜表单属性s
    /// </summary>
    public class MapFrmFools : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 傻瓜表单属性s
        /// </summary>
        public MapFrmFools()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmFool();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmFool> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmFool>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmFool> Tolist()
        {
            System.Collections.Generic.List<MapFrmFool> list = new System.Collections.Generic.List<MapFrmFool>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmFool)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
