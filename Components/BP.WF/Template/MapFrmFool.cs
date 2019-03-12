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

                if (this.No.Substring(0, 2) == "ND" && this.No.Contains("Dtl") ==false)
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
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.

                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);

                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 100, 20);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20, true);

                map.AddTBInt(MapDataAttr.TableCol, 4, "表单显示列数", true, true);

              //  map.AddTBInt(MapDataAttr.TableWidth, 900, "傻瓜表单宽度", true, false);
               // map.AddTBInt(MapDataAttr.TableHeight, 900, "傻瓜表单高度", true, false);

                map.AddTBInt(MapDataAttr.FrmW, 900, "傻瓜表单宽度", true, false);
                map.AddTBInt(MapDataAttr.FrmH, 900, "傻瓜表单高度", true, false);

                //数据源.
                map.AddDDLEntities(MapDataAttr.DBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FreeFrm, "表单类型",
                    true, false, MapDataAttr.FrmType);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
               // map.AddTBString(MapFrmFreeAttr.DesignerTool, null, "表单设计器", true, true, 0, 30, 20);

                map.AddTBString(MapDataAttr.Note, null, "备注", true, false,0,400,100, true);
                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                //查询条件.
                map.AddSearchAttr(MapDataAttr.DBSrc);

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
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量修改字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
              //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机端表单";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

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
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "../../WF/Img/Export.png";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "导出到xml";
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "重命名字段";
                rm.HisAttrs.AddTBString("FieldOld", null, "旧字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新字段英文名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新字段中文名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                rm.Icon = "../../WF/Img/ReName.png";
                map.AddRefMethod(rm);
              
                rm = new RefMethod();
                rm.Title = "表单检查"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCheckFixFrmForUpdateVer";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "表单检查";
                rm.Icon = "../../WF/Img/Check.png";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
             
                #endregion 方法 - 基本功能.

                #region 高级功能.
                //@李国文.
                rm = new RefMethod();
                rm.Title = "改变表单类型";
                rm.GroupName = "高级功能";
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "修改表单类型", true, true);
                map.AddRefMethod(rm);
                #endregion

                #region 方法 - 开发接口.
                rm = new RefMethod();
                rm.Title = "调用查询API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                rm.Icon = "../../WF/Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

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

        #region 高级设置.
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
            //BP.Sys.FormEventBase feb = BP.Sys.Glo.GetFormEventBaseByEnName(this.No);
            //if (feb == null)
            //    this.FromEventEntity = "";
            //else

            //    this.FromEventEntity = feb.ToString();

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
                map.FK_FrmSort = "";
                map.FK_FormTree = "";
                map.DirectUpdate();
            }
            base.afterUpdate();
        }
        #region 节点表单方法.

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
                return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=" + this.No.Replace("ND", "") + "&t=" + DataType.CurrentDataTime;
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

            //删除重影数据.
            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE CtrlType='FWC' and CTRLID is null");



            //一直遇到遇到自动变长的问题, 强制其修复过来.
            DBAccess.RunSQL("UPDATE Sys_Mapattr SET colspan=3 WHERE UIHeight<=38 AND colspan=4");

            string str = "";

             // 处理失去分组的字段. 
            string sql = "SELECT MyPK FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "' AND GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE FrmID='" + this.No + "' AND ( CtrlType='' OR CtrlType IS NULL)  )  OR GroupID IS NULL ";
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
                    gf.FrmID = this.No;
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
                gf.DirectSave();
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
                FrmNodeComponent conn = new FrmNodeComponent(this.NodeID);
                conn.Update();
            }


            //删除重复的数据, 比如一个从表显示了多个分组里. 增加此部分.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT * FROM (SELECT FrmID,CtrlID,CtrlType, count(*) as Num FROM sys_groupfield WHERE CtrlID!='' GROUP BY FrmID,CtrlID,CtrlType ) WHERE Num > 1";
            else
                sql = "SELECT * FROM (SELECT FrmID,CtrlID,CtrlType, count(*) as Num FROM sys_groupfield WHERE CtrlID!='' GROUP BY FrmID,CtrlID,CtrlType ) AS A WHERE A.Num > 1";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
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
            string url = "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&IsFirst=1&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
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
        public string DoWordFrm()
        {
            return "../../Admin/FoolFormDesigner/MapExt/WordFrm.aspx?s=34&FK_MapData=" + this.No + "&ExtType=WordFrm&RefNo=";
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
