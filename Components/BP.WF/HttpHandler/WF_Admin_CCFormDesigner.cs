﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.CCBill;
namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 执行父类的重写方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCFormDesigner()
        {
        }

        /// <summary>
        /// 创建枚举类型字段
        /// </summary>
        /// <returns></returns>
        public string FrmEnumeration_NewEnumField()
        {
            UIContralType ctrl = UIContralType.RadioBtn;
            string ctrlDoType = GetRequestVal("ctrlDoType");
            if (ctrlDoType == "DDL")
                ctrl = UIContralType.DDL;
            else
                ctrl = UIContralType.RadioBtn;

            string fk_mapdata = this.GetRequestVal("FK_MapData");
            string keyOfEn = this.GetRequestVal("KeyOfEn");
            string fieldDesc = this.GetRequestVal("Name");
            string enumKeyOfBind = this.GetRequestVal("UIBindKey"); //要绑定的enumKey.
            float x = this.GetRequestValFloat("x");
            float y = this.GetRequestValFloat("y");

            BP.Sys.CCFormAPI.NewEnumField(fk_mapdata, keyOfEn, fieldDesc, enumKeyOfBind, ctrl, x, y);
            return "绑定成功.";
        }
        /// <summary>
        /// 创建外键字段.
        /// </summary>
        /// <returns></returns>
        public string NewSFTableField()
        {
            try
            {
                string fk_mapdata = this.GetRequestVal("FK_MapData");
                string keyOfEn = this.GetRequestVal("KeyOfEn");
                string fieldDesc = this.GetRequestVal("Name");
                string sftable = this.GetRequestVal("UIBindKey");
                float x = float.Parse(this.GetRequestVal("x"));
                float y = float.Parse(this.GetRequestVal("y"));

                //调用接口,执行保存.
                BP.Sys.CCFormAPI.SaveFieldSFTable(fk_mapdata, keyOfEn, fieldDesc, sftable, x, y);
                return "设置成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 转换拼音
        /// </summary>
        /// <returns></returns>
        public string ParseStringToPinyin()
        {
            string name = GetRequestVal("name");
            string flag = GetRequestVal("flag");
            //暂时没发现此方法在哪里有调用，edited by liuxc,2017-9-25
            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(flag, "true"), true, 20);
        }

        /// <summary>
        /// 创建隐藏字段.
        /// </summary>
        /// <returns></returns>
        public string NewHidF()
        {
            MapAttr mdHid = new MapAttr();
            mdHid.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            mdHid.FK_MapData = this.FK_MapData;
            mdHid.KeyOfEn = this.KeyOfEn;
            mdHid.Name = this.Name;
            mdHid.MyDataType = int.Parse(this.GetRequestVal("FieldType"));
            mdHid.HisEditType = EditType.Edit;
            mdHid.MaxLen = 100;
            mdHid.MinLen = 0;
            mdHid.LGType = FieldTypeS.Normal;
            mdHid.UIVisible = false;
            mdHid.UIIsEnable = false;
            mdHid.Insert();

            return "创建成功..";
        }
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string sql = "";
            //返回值
            try
            {
                switch (this.DoType)
                {
                    default:
                        throw new Exception("没有判断的执行标记:" + this.DoType);
                }
            }
            catch (Exception ex)
            {
                return "err@" + this.ToString() + " msg:" + ex.Message;
            }
        }
        #endregion 执行父类的重写方法.

        #region 创建表单.
        public string NewFrmGuide_GenerPinYin()
        {
            string isQuanPin = this.GetRequestVal("IsQuanPin");
            string name = this.GetRequestVal("TB_Name");


            //表单No长度最大100，因有前缀CCFrm_，因此此处设置最大94，added by liuxc,2017-9-25
            string str = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(isQuanPin, "1"), true, 94);

            MapData md = new MapData();
            md.No = str;
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                md.No = str + "_" + WebUser.OrgNo;
            if (md.RetrieveFromDBSources() == 0)
                return str;

            return "err@表单ID:" + str + "已经被使用.";
        }
        /// <summary>
        /// 获得系统的表
        /// </summary>
        /// <returns></returns>
        public string NewFrmGuide_Init()
        {
            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc("local");
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            DataTable tables = src.GetTables(true);
            tables.TableName = "Tables";
            ds.Tables.Add(tables);
            return BP.Tools.Json.ToJson(ds);

        }
        /// <summary>
        /// 创建一个DBList.
        /// </summary>
        /// <returns></returns>
        public string NewFrmGuide_Create_DBList()
        {
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.Name = this.GetRequestVal("TB_Name");

            md.No = DataType.ParseStringForNo(this.GetRequestVal("TB_No"), 100);
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                md.No = md.No + "_" + BP.Web.WebUser.OrgNo;

            //表单类型.
            md.HisFrmTypeInt = this.GetRequestValInt("DDL_FrmType");
            md.EntityType = EntityType.DBList;
            string sort = this.GetRequestVal("FK_FrmSort");
            if (DataType.IsNullOrEmpty(sort) == true)
                sort = this.GetRequestVal("DDL_FrmTree");

            //    md.FK_FrmSort = sort;
            md.FK_FormTree = sort;

            //类型.
            md.AppType = "100";//独立表单.
            md.DBSrc = this.GetRequestVal("DDL_DBSrc");
            if (md.IsExits == true)
                return "err@表单ID:" + md.No + "已经存在.";

            //没有设置表，保存不上.
            md.PTable = this.No;
            md.Insert();

            //增加上OID字段.
            BP.Sys.CCFormAPI.RepareCCForm(md.No);

            //数据源实体，修改OID的属性为字符型
            MapAttr mapAttr = new MapAttr(md.No + "_OID");
            mapAttr.MyDataType = DataType.AppString;
            mapAttr.Update();


            BP.CCBill.FrmDict entityDict = new FrmDict(md.No);

            BP.CCBill.Template.CtrlModel ctrl = new BP.CCBill.Template.CtrlModel();
            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnSearch";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            if (ctrl.RetrieveFromDBSources() == 0)
                ctrl.Insert();
            entityDict.CheckEnityTypeAttrsFor_EntityNoName();


            #region 初始化数据.
            DBList db = new DBList(md.No);
            db.MainTable = "A";
            db.MainTablePK = "No";

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                db.ExpEn = "SELECT A.No as OID, A.No as BillNo, a.Name AS Title, A.Tel,A.Email , A.FK_Dept as DeptNo, B.Name AS DeptT FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.No='@Key' ";
                db.ExpList = "SELECT A.No as OID, A.No as BillNo, a.Name AS Title, A.Tel, A.PinYin,A.Email, B.No as DeptNo, B.Name AS DeptT FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No ";
                db.ExpCount = "SELECT  count(a.No) as Num FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No ";
            }
            else
            {
                db.ExpEn = "SELECT A.No as OID, A.No as BillNo, a.Name AS Title, A.Tel ,A.Email , A.FK_Dept as DeptNo, B.Name AS DeptT FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.No='@Key' AND A.OrgNo='@WebUser.OrgNo' ";
                db.ExpList = "SELECT A.No as OID, A.No as BillNo, a.Name AS Title, A.Tel, A.PinYin,A.Email, B.No as DeptNo, B.Name AS DeptT FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.OrgNo='@WebUser.OrgNo'  ";
                db.ExpCount = "SELECT  count(a.No) as Num FROM Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.OrgNo='@WebUser.OrgNo'  ";
            }

            db.Update();
            #endregion  初始化数据.


            return "创建成功...";
        }
        /// <summary>
        /// 创建表单
        /// </summary>
        /// <returns></returns>
        public string NewFrmGuide_Create()
        {
            MapData md = new MapData();

            try
            {
                md.Name = this.GetRequestVal("TB_Name");
                md.No = DataType.ParseStringForNo(this.GetRequestVal("TB_No"), 100);
                md.HisFrmTypeInt = this.GetRequestValInt("DDL_FrmType");
                string ptable = this.GetRequestVal("TB_PTable");

              

                md.PTable = ptable;
                //   md.PTable = DataType.ParseStringForNo(this.GetRequestVal("TB_PTable"), 100);

                //数据表模式。  需要翻译.
                md.PTableModel = this.GetRequestValInt("DDL_PTableModel");

                string sort = this.GetRequestVal("FK_FrmSort");
                if (DataType.IsNullOrEmpty(sort) == true)
                    sort = this.GetRequestVal("DDL_FrmTree");

                //    md.FK_FrmSort = sort;
                md.FK_FormTree = sort;

                md.AppType = "0";//独立表单
                md.DBSrc = this.GetRequestVal("DDL_DBSrc");
                if (md.IsExits == true)
                    return "err@表单ID:" + md.No + "已经存在.";

                switch (md.HisFrmType)
                {
                    //自由，傻瓜，SL表单不做判断
                    case BP.Sys.FrmType.FreeFrm:
                    case BP.Sys.FrmType.FoolForm:
                    case BP.Sys.FrmType.Develop:
                        break;
                    case BP.Sys.FrmType.Url:
                    case BP.Sys.FrmType.Entity:
                        md.UrlExt = md.PTable;
                        md.PTable = "";
                        break;
                    //如果是以下情况，导入模式
                    case BP.Sys.FrmType.WordFrm:
                    case BP.Sys.FrmType.ExcelFrm:
                    case BP.Sys.FrmType.VSTOForExcel:
                        break;
                    default:
                        throw new Exception("未知表单类型." + md.HisFrmType.ToString());
                }
                md.Insert();

                //增加上OID字段.
                BP.Sys.CCFormAPI.RepareCCForm(md.No);

                BP.Sys.EntityType entityType = (EntityType)this.GetRequestValInt("EntityType");

                #region 如果是单据.
                if (entityType == EntityType.FrmBill)
                {
                    BP.CCBill.FrmBill bill = new FrmBill(md.No);
                    bill.EntityType = EntityType.FrmBill;
                    bill.BillNoFormat = "ccbpm{yyyy}-{MM}-{dd}-{LSH4}";

                    //设置默认的查询条件.
                    bill.SetPara("IsSearchKey", 1);
                    bill.SetPara("DTSearchWay", 0);

                    bill.Update();
                    bool isHavePFrmID = false;
                    if (DataType.IsNullOrEmpty(this.GetRequestVal("FrmID")) == false)
                        isHavePFrmID = true;
                    bill.CheckEnityTypeAttrsFor_Bill(isHavePFrmID);
                }
                #endregion 如果是单据.

                #region 如果是实体 EnityNoName .
                if (entityType == EntityType.FrmDict)
                {
                    BP.CCBill.FrmDict entityDict = new FrmDict(md.No);
                    entityDict.BillNoFormat = "3"; //编码格式.001,002,003.
                    entityDict.BtnNewModel = 0;

                    //设置默认的查询条件.
                    entityDict.SetPara("IsSearchKey", 1);
                    entityDict.SetPara("DTSearchWay", 0);

                    entityDict.EntityType = EntityType.FrmDict;

                    entityDict.Update();
                    entityDict.CheckEnityTypeAttrsFor_EntityNoName();
                }
                #endregion 如果是实体 EnityNoName .

                //创建表与字段.
                GEEntity en = new GEEntity(md.No);
                en.CheckPhysicsTable();

                if (md.HisFrmType == BP.Sys.FrmType.WordFrm || md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
                {
                    /*把表单模版存储到数据库里 */
                    return "url@../../Comm/RefFunc/En.htm?EnName=BP.WF.Template.MapFrmExcel&PKVal=" + md.No;
                }

                if (md.HisFrmType == BP.Sys.FrmType.Entity)
                    return "url@../../Comm/Ens.htm?EnsName=" + md.PTable;

                if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
                    return "url@FormDesigner.htm?FK_MapData=" + md.No + "&EntityType=" + this.GetRequestVal("EntityType");

                if (md.HisFrmType == BP.Sys.FrmType.Develop)
                    return "url@../DevelopDesigner/Designer.htm?FK_MapData=" + md.No + "&FrmID=" + md.No + "&EntityType=" + this.GetRequestVal("EntityType");

                return "url@../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + md.No + "&EntityType=" + this.GetRequestVal("EntityType");
            }
            catch (Exception ex)
            {
                md.Delete();
                return "err@" + ex.Message;
            }
        }
        #endregion 创建表单.

        public string LetLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            WebUser.SignInOfGener(emp);
            return "登录成功.";
        }

        /// <summary>
        /// 表单属性
        /// </summary>
        /// <returns></returns>
        public string GoToFrmAttr_Init()
        {
            //根据不同的表单类型转入不同的表单设计器上去.
            BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
            if (md.EntityType == EntityType.SingleFrm)
            {
                if (md.HisFrmType == FrmType.FreeFrm)
                    return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapFrmFree&No=" + this.FK_MapData;

                //、、f (md.HisFrmType == FrmType.FoolForm)
                //     return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapFrmFool&No=" + this.FK_MapData;

                if (md.HisFrmType == FrmType.ExcelFrm)
                    return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapFrmExcel&No=" + this.FK_MapData;

                if (md.HisFrmType == FrmType.Url)
                    return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapDataURL&No=" + this.FK_MapData;


                return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapFrmFool&No=" + this.FK_MapData;
            }

            if (md.EntityType == EntityType.FrmDict)
                return "url@../../Comm/En.htm?EnName=BP.CCBill.FrmDict&No=" + this.FK_MapData;

            if (md.EntityType == EntityType.EntityTree)
                return "url@../../Comm/En.htm?EnName=BP.CCBill.FrmBill&No=" + this.FK_MapData;


            return "err@没有判断的表单转入类型" + md.HisFrmType.ToString();
        }

        public string PublicNoNameCtrlCreate()
        {
            try
            {
                float x = float.Parse(this.GetRequestVal("x"));
                float y = float.Parse(this.GetRequestVal("y"));
                BP.Sys.CCFormAPI.CreatePublicNoNameCtrl(this.FrmID, this.GetRequestVal("CtrlType"),
                    this.GetRequestVal("No"),
                    this.GetRequestVal("Name"), x, y);
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string NewImage()
        {

            try
            {
                BP.Sys.CCFormAPI.NewImage(this.GetRequestVal("FrmID"),
                    this.GetRequestVal("KeyOfEn"), this.GetRequestVal("Name"),
                    //int.Parse(this.GetRequestVal("FieldType")),
                    float.Parse(this.GetRequestVal("x")),
                   float.Parse(this.GetRequestVal("y"))
                   );
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }


        }
        public string NewField()
        {
            try
            {
                BP.Sys.CCFormAPI.NewField(this.GetRequestVal("FrmID"),
                    this.GetRequestVal("KeyOfEn"), this.GetRequestVal("Name"),
                    int.Parse(this.GetRequestVal("FieldType")),
                    float.Parse(this.GetRequestVal("x")),
                   float.Parse(this.GetRequestVal("y"))
                   );
                return "true";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 生成所有表单元素.
        /// </summary>
        /// <returns></returns>
        public string CCForm_AllElements_ResponseJson()
        {
            try
            {
                DataSet ds = new DataSet();

                MapData mapData = new MapData(this.FK_MapData);

                //属性.
                MapAttrs attrs = new MapAttrs(this.FK_MapData);
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.UIVisible, 1);
                ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

                FrmBtns btns = new FrmBtns(this.FK_MapData);
                ds.Tables.Add(btns.ToDataTableField("Sys_FrmBtn"));

                FrmRBs rbs = new FrmRBs(this.FK_MapData);
                ds.Tables.Add(rbs.ToDataTableField("Sys_FrmRB"));

                FrmLabs labs = new FrmLabs(this.FK_MapData);
                ds.Tables.Add(labs.ToDataTableField("Sys_FrmLab"));

                FrmLinks links = new FrmLinks(this.FK_MapData);
                ds.Tables.Add(links.ToDataTableField("Sys_FrmLink"));

                FrmImgs imgs = new FrmImgs(this.FK_MapData);
                ds.Tables.Add(imgs.ToDataTableField("Sys_FrmImg"));

                FrmImgAths imgAths = new FrmImgAths(this.FK_MapData);
                ds.Tables.Add(imgAths.ToDataTableField("Sys_FrmImgAth"));

                FrmAttachments aths = new FrmAttachments(this.FK_MapData);
                ds.Tables.Add(aths.ToDataTableField("Sys_FrmAttachment"));

                MapDtls dtls = new MapDtls();
                dtls.Retrieve(MapDtlAttr.FK_MapData, this.FK_MapData, MapDtlAttr.FK_Node, 0);
                ds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));

                FrmLines lines = new FrmLines(this.FK_MapData);
                ds.Tables.Add(lines.ToDataTableField("Sys_FrmLine"));

                BP.Sys.FrmUI.MapFrameExts mapFrameExts = new BP.Sys.FrmUI.MapFrameExts(this.FK_MapData);
                ds.Tables.Add(mapFrameExts.ToDataTableField("Sys_MapFrame"));

                //组织节点组件信息.
                string sql = "";
                if (this.FK_Node > 100)
                {
                    sql += "select '轨迹图' AS Name,'FlowChart' AS No,FrmTrackSta Sta,FrmTrack_X X,FrmTrack_Y Y,FrmTrack_H H,FrmTrack_W  W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '审核组件'AS Name, 'FrmCheck'AS No,FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '子流程' AS Name,'SubFlowDtl'AS  No,SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W from WF_Node  WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '子线程' AS Name, 'ThreadDtl'AS  No,FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W W from WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    sql += " union select '流转自定义' AS Name,'FrmTransferCustom' AS  No,FTCSta Sta,FTC_X X,FTC_Y Y,FTC_H H,FTC_W  W FROM WF_Node WHERE nodeid=" + SystemConfig.AppCenterDBVarStr + "nodeid";
                    Paras ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("nodeid", this.FK_Node);
                    DataTable dt = null;

                    try
                    {
                        dt = DBAccess.RunSQLReturnTable(ps);
                    }
                    catch (Exception ex)
                    {
                        FrmSubFlow sb = new FrmSubFlow();
                        sb.CheckPhysicsTable();

                        TransferCustom tc = new TransferCustom();
                        tc.CheckPhysicsTable();

                        FrmThread ft = new FrmThread();
                        ft.CheckPhysicsTable();

                        FrmTrack ftd = new FrmTrack();
                        ftd.CheckPhysicsTable();

                        FrmTransferCustom ftd1 = new FrmTransferCustom();
                        ftd1.CheckPhysicsTable();

                        throw ex;
                    }

                    dt.TableName = "FigureCom";

                    if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        //  figureComCols = "Name,No,Sta,X,Y,H,W";
                        dt.Columns[0].ColumnName = "Name";
                        dt.Columns[1].ColumnName = "No";
                        dt.Columns[2].ColumnName = "Sta";
                        dt.Columns[3].ColumnName = "X";
                        dt.Columns[4].ColumnName = "Y";
                        dt.Columns[5].ColumnName = "H";
                        dt.Columns[6].ColumnName = "W";
                    }
                    ds.Tables.Add(dt);
                }

                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public string SaveForm()
        {
            //清缓存
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            string docs = this.GetRequestVal("diagram");
            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, docs);

            return "保存成功.";
        }

        #region 表格处理.
        public string Tables_Init()
        {
            BP.Sys.SFTables tabs = new BP.Sys.SFTables();
            tabs.RetrieveAll();
            DataTable dt = tabs.ToDataTableField();
            dt.Columns.Add("RefNum", typeof(int));

            foreach (DataRow dr in dt.Rows)
            {
                //求引用数量.
                int refNum = DBAccess.RunSQLReturnValInt("SELECT COUNT(KeyOfEn) FROM Sys_MapAttr WHERE UIBindKey='" + dr["No"] + "'", 0);
                dr["RefNum"] = refNum;
            }
            return BP.Tools.Json.ToJson(dt);
        }
        public string Tables_Delete()
        {
            try
            {
                BP.Sys.SFTable tab = new BP.Sys.SFTable();
                tab.No = this.No;
                tab.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string TableRef_Init()
        {
            BP.Sys.MapAttrs mapAttrs = new BP.Sys.MapAttrs();
            mapAttrs.RetrieveByAttr(BP.Sys.MapAttrAttr.UIBindKey, this.FK_SFTable);

            DataTable dt = mapAttrs.ToDataTableField();
            return BP.Tools.Json.ToJson(dt);
        }


        #endregion

        /// <summary>
        /// 表单重置
        /// </summary>
        /// <returns></returns>
        public string ResetFrm_Init()
        {
            MapData md = new MapData(this.FK_MapData);
            md.ResetMaxMinXY();
            md.Update();
            return "重置成功.";
        }

        #region 复制表单
        /// <summary>
        /// 复制表单属性和表单内容
        /// </summary>
        /// <param name="frmId">新表单ID</param>
        /// <param name="frmName">新表单内容</param>
        public void DoCopyFrm()
        {
            string fromFrmID = GetRequestVal("FromFrmID");
            string toFrmID = GetRequestVal("ToFrmID");
            string toFrmName = GetRequestVal("ToFrmName");
            MapData toMapData = new MapData(fromFrmID);
            toMapData.No = toFrmID;
            toMapData.Name = toFrmName;
            toMapData.Insert();
            //导入表单信息
            MapData.ImpMapData(toFrmID, BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(fromFrmID));

            if (toMapData.HisEntityType == (int)EntityType.FrmBill)
            {
                FrmBill frmBill = new FrmBill(fromFrmID);
                frmBill.No = toFrmID;
                frmBill.Name = toFrmName;
                frmBill.Update();
            }
            if (toMapData.HisEntityType == (int)EntityType.FrmDict)
            {
                FrmDict frmDict = new FrmDict(fromFrmID);
                frmDict.No = toFrmID;
                frmDict.Name = toFrmName;
                frmDict.Update();
            }

            //清空缓存

            toMapData.RepairMap();
            SystemConfig.DoClearCash();


        }
        #endregion 复制表单

    }
}
