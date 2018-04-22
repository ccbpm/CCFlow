using System;
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
    public class MapFrmNodeAttr : MapDataAttr
    {
        /// <summary>
        /// 保存标签
        /// </summary>
        public const string BtnSaveLab = "BtnSaveLab";
        /// <summary>
        /// 保存是否启用
        /// </summary>
        public const string BtnSaveEnable = "BtnSaveEnable";
        /// <summary>
        /// 删除标签
        /// </summary>
        public const string BtnDelLab = "BtnDelLab";
        /// <summary>
        /// 删除是否启用
        /// </summary>
        public const string BtnDelEnable = "BtnDelEnable";

        /// <summary>
        /// 打印标签
        /// </summary>
        public const string BtnPrintLab = "BtnPrintLab";
        /// <summary>
        /// 打印是否启用
        /// </summary>
        public const string BtnPrintEnable = "BtnPrintEnable";
    }
    /// <summary>
    /// 自由表单属性
    /// </summary>
    public class MapFrmNode : EntityNoName
    {
        #region 文件模版属性.
       
        #endregion 文件模版属性.

        #region 属性
        /// <summary>
        /// 表单事件实体
        /// </summary>
        public string FromEventEntity
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FormEventEntity);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FormEventEntity,value);
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
                int i = this.GetValIntByKey(MapFrmNodeAttr.TableWidth);
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
                int i = this.GetValIntByKey(MapFrmNodeAttr.TableHeight);
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
                int i = this.GetValIntByKey(MapFrmNodeAttr.TableCol);
                if (i == 0 || i == 1)
                    return 4;
                return i;
            }
            set
            {
                this.SetValByKey(MapFrmNodeAttr.TableCol, value);
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
        public MapFrmNode()
        {
        }
        /// <summary>
        /// 自由表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmNode(string no)  : base(no)
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
                map.AddTBStringPK(MapFrmNodeAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.AddTBString(MapFrmNodeAttr.PTable, null, "存储表", true, false, 0, 100, 20);
                map.AddTBString(MapFrmNodeAttr.Name, null, "表单名称", true, false, 0, 200, 20,true);
                map.AddTBString(MapDataAttr.FormEventEntity, null, "事件实体", true, true, 0, 100, 20, true);
                 
                //数据源.
                map.AddDDLEntities(MapFrmNodeAttr.DBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                //宽度高度.
                map.AddTBInt(MapFrmNodeAttr.FrmW, 900, "宽度", true, false);
                map.AddTBInt(MapFrmNodeAttr.FrmH, 1200, "高度", true, false);

                //表单的运行类型.
                map.AddDDLSysEnum(MapFrmNodeAttr.FrmType, (int)BP.Sys.FrmType.FreeFrm, "表单类型", true, false, MapFrmNodeAttr.FrmType);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapFrmNodeAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapFrmNodeAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapFrmNodeAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, false);
                map.AddTBString(MapFrmNodeAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapFrmNodeAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapFrmNodeAttr.Note, null, "备注", true, false, true);

                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapFrmNodeAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                map.AddMyFile("表单模版");

                //查询条件.
                map.AddSearchAttr(MapFrmNodeAttr.DBSrc);

                #region 方法 - 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "启动傻瓜表单设计器";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                rm.Icon = "../../WF/Img/FileType/xlsx.gif";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段维护";
                rm.ClassMethodName = this.ToString() + ".DoEditFiledsList";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "批量修改字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "装载填充"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
                rm.Icon = "../../WF/Img/FullData.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量设置验证规则";
                rm.Icon = "../../WF/Img/RegularExpression.png";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
               // map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "JS编程"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoInitScript";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单body属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBodyAttr";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "导出XML表单模版"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExp";
                rm.Icon = "../../WF/Img/Export.png";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "导出到xml";
                rm.RefMethodType = RefMethodType.RightFrameOpen;           
                map.AddRefMethod(rm);

              

                rm = new RefMethod();
                rm.Title = "重置表单";  //"设计表单";
                rm.ClassMethodName = this.ToString() + ".DoReset";
                rm.Warning = "重置就是重新让设计器还原原来的设置. \t\n注意:执行重置有可能会导致部分的界面元素不能按照原始的方式还原上来.";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "重置表单";
                rm.Icon = "../../WF/Img/Check.png";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "Tab顺序键"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoTabIdx";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);




                //rm = new RefMethod();
                //rm.Title = "节点表单组件"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFrmCompent";
                //rm.Visable = true;
                //rm.RefAttrLinkLabel = "节点表单组件";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.Icon = ../../Img/Components.png";
                //map.AddRefMethod(rm);
                #endregion 方法 - 基本功能.

                #region 高级设置.

                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "重命名字段";
             //   rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                rm.Icon = "../../WF/Img/ReName.png";
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重命表单ID";
              //  rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("NewFrmID1", null, "新表单ID名称", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("NewFrmID2", null, "确认表单ID名称", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmID";
                rm.Icon = "../../WF/Img/ReName.png";
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "复制表单";
                //  rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FrmID", null, "要复制新表单ID", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FrmName", null, "表单名称", true, false, 0, 100, 100);
                rm.HisAttrs.AddDDLEntities("FrmTree", null, "复制到表单目录", new FrmTrees(),true);

                rm.ClassMethodName = this.ToString() + ".DoCopyFrm";
                rm.Icon = "../../WF/Img/Btn/Copy.GIF";
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "重置表单";
                rm.ClassMethodName = this.ToString() + ".DoResetFrm";
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "手机端表单";
                rm.GroupName = "高级设置";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);
                #endregion 高级设置.

                #region 方法 - 开发接口.
                rm = new RefMethod();
                rm.Title = "调用查询API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                rm.Icon = "../../WF/Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "调用分析API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoGroup";
                rm.Icon = "../../WF/Img/Table.gif";
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

        protected override bool beforeUpdate()
        {
            //注册事件表单实体.
            BP.Sys.FormEventBase feb = BP.Sys.Glo.GetFormEventBaseByEnName(this.No);
            if (feb == null)
                this.FromEventEntity = "";
            else

                this.FromEventEntity = feb.ToString();

            return base.beforeUpdate();
        }
        public string DoTabIdx()
        {
            return SystemConfig.CCFlowWebPath +"WF/Admin/FoolFormDesigner/TabIdx.htm?FK_MapData=" + this.No;
        }

        public string DoResetFrm()
        {
            string sql = "UPDATE Sys_MapData SET FormJson= null WHERE No='"+this.No+"'";
            DBAccess.RunSQL(sql);
            return "表单已经重置成功, 请关闭表单设计器重新打开.";
        }
        
        /// <summary>
        /// 复制表单
        /// </summary>
        /// <returns></returns>
        public string DoCopyFrm(string frmID, string frmName, string fk_frmTree)
        {
            return BP.Sys.CCFormAPI.CopyFrm(this.No, frmID, frmName, fk_frmTree);
        }

        #region 节点表单方法.
        public string DoReset()
        {
            string sql = "UPDATE Sys_MapData SET FormJson = null WHERE No='"+this.No+"'";
            BP.DA.DBAccess.RunSQL(sql);
            return "重置成功,您需要关闭当前H5的表单设计器然后重新打开.";
        }
       
        /// <summary>
        /// 傻瓜表单设计器
        /// </summary>
        /// <returns></returns>
        public string DoDesignerFool()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsEditMapData=True&DoDesignerFool&IsFirst=1";
        }
        /// <summary>
        /// 编辑excel模版.
        /// </summary>
        /// <returns></returns>
        public string DoEditExcelTemplate()
        {
            return "../../Admin/CCFormDesigner/ExcelFrmDesigner/Designer.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 表单字段.
        /// </summary>
        /// <returns></returns>
        public string DoEditFiledsList()
        {
            return "../../Admin/FoolFormDesigner/BatchEdit.htm?FK_MapData=" + this.No;
        }
        #endregion

        #region 通用方法.
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="frmID1"></param>
        /// <param name="frmID2"></param>
        /// <returns></returns>
        public string DoChangeFrmID(string frmID1, string frmID2)
        {
            MapData md = new MapData();
            md.No = frmID1;
            if (md.IsExits == true)
                return "表单ID【" + frmID1 + "】已经存在";

            if (frmID1 != frmID2)
                return "两次输入的ID不一致.";


            string frmIDOld = this.No;

            string sqls = "";
            sqls += "@UPDATE Sys_MapData SET No='" + frmID1 + "' WHERE No='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmLine SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmLab SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmBtn SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapAttr SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapExt SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmImg SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmImgAth SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmRB SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapDtl SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapFrame SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmEle SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmEvent SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            BP.DA.DBAccess.RunSQLs(sqls);

            return "重命名成功，你需要关闭窗口重新刷新。";
        }
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
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionBatch.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量修改字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchEditAttr()
        {
            return "../../Admin/FoolFormDesigner/BatchEdit.htm?FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/AttrNode/SortingMapAttrs.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDFrom()
        {
            string url = "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            PubClass.WinOpen(url, 800, 650);
            return null;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoDFromCol4()
        {
            string url = "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&IsFirst=1&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
            PubClass.WinOpen(url, 800, 650);
            return null;
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
        /// 调用分析API
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            return "../../Comm/Group.aspx?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoDBSrc()
        {
            return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=BP.Sys.SFDBSrcs";
        }
        public string DoWordFrm()
        {
            return "../../Admin/FoolFormDesigner/MapExt/WordFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=WordFrm&RefNo=";
        }
        public string DoExcelFrm()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ExcelFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=ExcelFrm&RefNo=";
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
        /// 自由表单属性.
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
            return "../../Admin/CCFormDesigner/FrmEvent.htm?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoMapExt()
        {
            return "../../Admin/FoolFormDesigner/MapExt/List.aspx?FK_MapData=" + this.No + "&T=sd";
        }
        /// <summary>
        /// 导出表单
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            return "../../Admin/FoolFormDesigner/Exp.htm?FK_MapData=" + this.No;
 
        }
        #endregion 方法.
    }
    /// <summary>
    /// 自由表单属性s
    /// </summary>
    public class MapFrmNodes : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 自由表单属性s
        /// </summary>
        public MapFrmNodes()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmNode();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmNode> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmNode>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmNode> Tolist()
        {
            System.Collections.Generic.List<MapFrmNode> list = new System.Collections.Generic.List<MapFrmNode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmNode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
