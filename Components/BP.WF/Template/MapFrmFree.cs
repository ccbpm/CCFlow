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
    /// 自由表单属性 attr
    /// </summary>
    public class MapFrmFreeAttr : MapDataAttr
    {
        public const string TemplaterVer = "TemplaterVer";
    }
    /// <summary>
    /// 自由表单属性
    /// </summary>
    public class MapFrmFree : EntityNoName
    {
        #region 文件模版属性.
        /// <summary>
        /// 模版版本号
        /// </summary>
        public string TemplaterVer
        {
            get
            {
                return this.GetValStringByKey(MapFrmFreeAttr.TemplaterVer);
            }
            set
            {
                this.SetValByKey(MapFrmFreeAttr.TemplaterVer, value);
            }
        }
        #endregion 文件模版属性.

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

                if (this.No.Substring(0, 2) == "ND")
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 节点ID.
        /// </summary>
        public int NodeID
        {
            get
            {
                return int.Parse(this.No.Replace("ND", ""));
            }
        }
        /// <summary>
        /// 傻瓜表单-宽度
        /// </summary>
        public string TableWidth
        {
            get
            {
                int i = this.GetValIntByKey(MapFrmFreeAttr.TableWidth);
                if (i <= 50)
                    return "900";
                return i.ToString();
            }
        }
        /// <summary>
        /// 傻瓜表单-高度
        /// </summary>
        public string TableHeight
        {
            get
            {
                int i = this.GetValIntByKey(MapFrmFreeAttr.TableHeight);
                if (i <= 500)
                    return "900";
                return i.ToString();
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
                int i = this.GetValIntByKey(MapFrmFreeAttr.TableCol);
                if (i == 0 || i == 1)
                    return 4;
                return i;
            }
            set
            {
                this.SetValByKey(MapFrmFreeAttr.TableCol, value);
            }
        }
       
        #endregion

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

        #region 构造方法
        /// <summary>
        /// 自由表单属性
        /// </summary>
        public MapFrmFree()
        {
        }
        /// <summary>
        /// 自由表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmFree(string no)  : base(no)
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
                Map map = new Map("Sys_MapData", "自由表单属性");
                map.Java_SetEnType(EnType.Sys);

                #region 基本属性.
                map.AddTBStringPK(MapFrmFreeAttr.No, null, "表单编号", true, false, 1, 190, 20);
                map.AddTBString(MapFrmFreeAttr.PTable, null, "存储表", true, false, 0, 100, 20);
                map.AddTBString(MapFrmFreeAttr.Name, null, "表单名称", true, false, 0, 500, 20,true);
                 
                //数据源.
                map.AddDDLEntities(MapFrmFreeAttr.DBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddDDLEntities(MapFrmFreeAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);

                //表单的运行类型.
                map.AddDDLSysEnum(MapFrmFreeAttr.FrmType, (int)BP.Sys.FrmType.FreeFrm, "表单类型", true, false, MapFrmFreeAttr.FrmType);
                #endregion 基本属性.

                #region 模版属性。
                map.AddTBString(MapFrmFreeAttr.TemplaterVer, null, "模版编号", true, false, 0, 30, 20);
                #endregion 模版属性。

                #region 设计者信息.
                map.AddTBString(MapFrmFreeAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapFrmFreeAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapFrmFreeAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapFrmFreeAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapFrmFreeAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapFrmFreeAttr.Note, null, "备注", true, false, true);

                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapFrmFreeAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                map.AddMyFile("表单模版");

                //查询条件.
                map.AddSearchAttr(MapFrmFreeAttr.DBSrc);

                #region 方法 - 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "启动自由表单设计器(SL)";
                rm.ClassMethodName = this.ToString() + ".DoDesignerSL";
              //  rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/FileType/xlsx.gif";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "启动自由表单设计器(H5)";
                rm.ClassMethodName = this.ToString() + ".DoDesignerH5";
                //  rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/FileType/xlsx.gif";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "启动傻瓜表单设计器";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/FileType/xlsx.gif";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段维护";
                rm.ClassMethodName = this.ToString() + ".DoEditFiledsList";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量修改字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
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
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/RegularExpression.png";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
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


                rm = new RefMethod();
                rm.Title = "表单检查";  //"设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCheckFixFrmForUpdateVer";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "表单检查";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Check.png";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "节点表单组件"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFrmCompent";
                //rm.Visable = true;
                //rm.RefAttrLinkLabel = "节点表单组件";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Components.png";
                //map.AddRefMethod(rm);
                #endregion 方法 - 基本功能.

                #region 高级设置.

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "重命名字段";
                rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/ReName.png";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机端表单";
                rm.GroupName = "高级设置";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


            

                #endregion 高级设置.

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

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 节点表单方法.
        /// <summary>
        /// 启动自由表单设计器(SL)
        /// </summary>
        /// <returns></returns>
        public string DoDesignerSL()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCFormDesigner/CCFormDesignerSL.aspx?FK_MapData="+this.No+"&UserNo="+Web.WebUser.No+"&SID="+BP.Web.WebUser.SID;
        }
        /// <summary>
        /// 启动自由表单设计器(h5)
        /// </summary>
        /// <returns></returns>
        public string DoDesignerH5()
        {
            // WF/Admin/CCFormDesigner/FormDesigner.htm?FK_MapData=ND102&UserNo=admin&SID=44a42h5gcbxnwjof2hv2pw5e
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCFormDesigner/FormDesigner.htm?FK_MapData=" + this.No + "&UserNo=" + Web.WebUser.No + "&SID=" + BP.Web.WebUser.SID;
        }
        /// <summary>
        /// 傻瓜表单设计器
        /// </summary>
        /// <returns></returns>
        public string DoDesignerFool()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/Designer.aspx?FK_MapData="+this.No+"&MyPK="+this.No+"&IsEditMapData=True";
        }
        /// <summary>
        /// 编辑excel模版.
        /// </summary>
        /// <returns></returns>
        public string DoEditExcelTemplate()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCFormDesigner/ExcelFrmDesigner/Designer.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 表单字段.
        /// </summary>
        /// <returns></returns>
        public string DoEditFiledsList()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/CCFormDesigner/FiledsList.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 执行旧版本的兼容性检查.
        /// </summary>
        public string DoCheckFixFrmForUpdateVer()
        {
            // 更新状态.
            DBAccess.RunSQL("UPDATE Sys_GroupField SET CtrlType='' WHERE CtrlType IS NULL");
            DBAccess.RunSQL("UPDATE Sys_GroupField SET CtrlID='' WHERE CtrlID IS NULL");

            string str = "";

             // 处理失去分组的字段. 
            string sql = "SELECT MyPK FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "' AND GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "' AND CtrlType='' ) ";
            MapAttrs attrs = new MapAttrs();
            attrs.RetrieveInSQL(sql);
            if (attrs.Count != 0)
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
                    gf.EnName = this.No;
                    gf.Insert();
                }

                //设置GID.
                foreach (MapAttr attr in attrs)
                {
                    attr.Update(MapAttrAttr.GroupID, gf.OID);
                }
            }

            //从表.
            MapDtls dtls = new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                GroupField gf = new GroupField();
                if (gf.IsExit(GroupFieldAttr.CtrlID, dtl.No) == true && !string.IsNullOrEmpty(gf.CtrlType))
                    continue;

                gf.Lab = dtl.Name;
                gf.CtrlID =  dtl.No;
                gf.CtrlType = "Dtl";
                gf.EnName = dtl.FK_MapData;
                gf.DirectSave();

                str += "@为从表" + dtl.Name + " 增加了分组.";
            }

            // 框架.
            MapFrames frams = new MapFrames(this.No);
            foreach (MapFrame fram in frams)
            {
                GroupField gf = new GroupField();
                if (gf.IsExit(GroupFieldAttr.CtrlID, fram.MyPK) == true && !string.IsNullOrEmpty(gf.CtrlType))
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
                GroupField gf = new GroupField();
                if (gf.IsExit(GroupFieldAttr.CtrlID, ath.MyPK) == true && !string.IsNullOrEmpty(gf.CtrlType))
                    continue;

                gf.Lab = ath.Name;
                gf.CtrlID = ath.MyPK;
                gf.CtrlType = "Ath";
                gf.EnName = ath.FK_MapData;
                gf.Insert();

                str += "@为附件 " + ath.Name + " 增加了分组.";
            }

            if (this.IsNodeFrm == true)
            {
                FrmNodeComponent conn = new FrmNodeComponent(this.NodeID);
                conn.Update();
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
            attrOld.KeyOfEn = fieldOld;
            attrOld.FK_MapData = this.No;
            attrOld.MyPK = attrOld.FK_MapData + "_" + attrOld.KeyOfEn;
            if (attrOld.RetrieveFromDBSources() == 0)
                return "@旧字段输入错误[" + attrOld.KeyOfEn + "].";

            //检查是否存在该字段？
            MapAttr attrNew = new MapAttr();
            attrNew.KeyOfEn = newField;
            attrNew.FK_MapData = this.No;
            attrNew.MyPK = attrNew.FK_MapData + "_" + attrNew.KeyOfEn;
            if (attrNew.RetrieveFromDBSources() == 1)
                return "@该字段[" + attrNew.KeyOfEn + "]已经存在.";

            //删除旧数据.
            attrOld.Delete();

            //copy这个数据,增加上它.
            attrNew.Copy(attrOld);
            attrNew.KeyOfEn = newField;
            attrNew.FK_MapData = this.No;

            if (newFieldName != "")
                attrNew.Name = newFieldName;

            attrNew.Insert();

            //更新处理他的相关业务逻辑.
            MapExts exts = new MapExts(this.No);
            foreach (MapExt item in exts)
            {
                item.MyPK = item.MyPK.Replace("_" + fieldOld, "_" + newField);

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
            return "执行成功";
        }
        /// <summary>
        /// 批量设置正则表达式规则.
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpressionBatch()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/RegularExpressionBatch.aspx?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量修改字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchEditAttr()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/BatchEdit.aspx?FK_MapData=" +
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
            string url = SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/CCForm/Frm.aspx?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            PubClass.WinOpen(url, 800, 650);
            return null;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoDFromCol4()
        {
            string url = SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/Designer.aspx?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
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
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/WordFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=WordFrm&RefNo=";
        }
        public string DoExcelFrm()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/ExcelFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=ExcelFrm&RefNo=";
        }
        public string DoPageLoadFull()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/PageLoadFull.aspx?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        public string DoInitScript()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/InitScript.aspx?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// 自由表单属性.
        /// </summary>
        /// <returns></returns>
        public string DoBodyAttr()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/BodyAttr.aspx?s=34&FK_MapData=" + this.No + "&ExtType=BodyAttr&RefNo=";
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
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/MapExt/List.aspx?FK_MapData=" + this.No + "&T=sd";
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
    /// 自由表单属性s
    /// </summary>
    public class MapFrmFrees : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 自由表单属性s
        /// </summary>
        public MapFrmFrees()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmFree();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmFree> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmFree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmFree> Tolist()
        {
            System.Collections.Generic.List<MapFrmFree> list = new System.Collections.Generic.List<MapFrmFree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmFree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
