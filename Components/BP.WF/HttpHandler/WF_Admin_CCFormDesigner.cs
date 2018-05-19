using System;
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

namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 初始化表单
        /// </summary>
        /// <returns></returns>
        public string FormDesigner_InitMapData()
        {
            MapData md = new MapData(this.FK_MapData);
            return md.ToJson();
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner(HttpContext mycontext)
        {
            this.context = mycontext;
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
        
        public string HiddenFieldDelete()
        {
            string records = GetRequestVal("records");
            string FK_MapData = GetRequestVal("FK_MapData");
            MapAttr mapAttrs = new MapAttr();
            int result = mapAttrs.Delete(MapAttrAttr.KeyOfEn, records, MapAttrAttr.FK_MapData, FK_MapData);
            return result.ToString();
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
                return "err@"+this.ToString()+" msg:" + ex.Message;
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
        public string NewFrmGuide_Create()
        {
            MapData md = new MapData();
            md.Name = this.GetRequestVal("TB_Name");
            md.No = DataType.ParseStringForNo(this.GetRequestVal("TB_No"), 100);

            //表单的物理表.
            md.PTable = DataType.ParseStringForNo(this.GetRequestVal("TB_PTable"), 100);

            //数据表模式。 @周朋 需要翻译.
            md.PTableModel = this.GetRequestValInt("DDL_PTableModel");

            md.FK_FrmSort = this.GetRequestVal("DDL_FrmTree");
            md.FK_FormTree = this.GetRequestVal("DDL_FrmTree");
            md.AppType = "0";//独立表单
            md.DBSrc = this.GetRequestVal("DDL_DBSrc");
            if (md.IsExits == true)
                return "err@表单ID:" + md.No + "已经存在.";

            md.HisFrmTypeInt = this.GetRequestValInt("DDL_FrmType");

            switch (md.HisFrmType)
            {
                //自由，傻瓜，SL表单不做判断
                case BP.Sys.FrmType.FreeFrm:
                case BP.Sys.FrmType.FoolForm:
                    break;
                case BP.Sys.FrmType.Url:
                case BP.Sys.FrmType.Entity:
                    md.Url = md.PTable;
                    break;
                //如果是以下情况，导入模式
                case BP.Sys.FrmType.WordFrm:
                case BP.Sys.FrmType.ExcelFrm:
                    break;
                default:
                    throw new Exception("未知表单类型.");
            }
            md.Insert();

            //增加上OID字段.
            BP.Sys.CCFormAPI.RepareCCForm(md.No);

            if (md.HisFrmType == BP.Sys.FrmType.WordFrm || md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
            {
                /*把表单模版存储到数据库里 */
                return "url@../../Comm/En.htm?EnName=BP.WF.Template.MapFrmExcel&PKVal=" + md.No;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Entity)
                return "url@../../Comm/En.htm?EnName=" + md.PTable;

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
                return "url@FormDesigner.htm?FK_MapData=" + md.No;


            return "url@../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + md.No;
        }
        #endregion 创建表单.

        public string LetLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            WebUser.SignInOfGener(emp);
            return "登录成功.";
        }
      
        public string GoToFrmDesigner_Init()
        {
            //根据不同的表单类型转入不同的表单设计器上去.
            BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
            if (md.HisFrmType == BP.Sys.FrmType.FoolForm)
            {
                /* 傻瓜表单 需要翻译. */
                return "url@../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + this.FK_MapData;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                /* 自由表单 */
                return "url@FormDesigner.htm?FK_MapData=" + this.FK_MapData + "&IsFirst=1";
            }

            if (md.HisFrmType == BP.Sys.FrmType.VSTOForExcel)
            {
                /* 自由表单 */
                return "url@FormDesigner.htm?FK_MapData=" + this.FK_MapData;
            }

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
                return "err@"+ex.Message;
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
                MapData mapData = new MapData(this.FK_MapData);

                //获取表单元素
                string sqls = "SELECT * FROM Sys_MapAttr WHERE UIVisible=1 AND FK_MapData='" + this.FK_MapData + "';" + Environment.NewLine
                                + "SELECT * FROM Sys_FrmBtn WHERE FK_MapData='" + this.FK_MapData + "';" + Environment.NewLine
                                + "SELECT * FROM Sys_FrmRB WHERE FK_MapData='" + this.FK_MapData + "';" + Environment.NewLine
                                + "SELECT * FROM Sys_FrmLab WHERE FK_MapData='" + this.FK_MapData + "';"
                                + "SELECT * FROM Sys_FrmLink WHERE FK_MapData='" + this.FK_MapData + "';"
                                + "SELECT * FROM Sys_FrmImg WHERE FK_MapData='" + this.FK_MapData + "';"
                                + "SELECT * FROM Sys_FrmImgAth WHERE FK_MapData='" + this.FK_MapData + "';"
                                + "SELECT * FROM Sys_FrmAttachment WHERE FK_MapData='" + this.FK_MapData + "';"
                                 + "SELECT * FROM Sys_MapDtl WHERE FK_MapData='" + this.FK_MapData + "';"
                                 + "SELECT * FROM Sys_FrmLine WHERE FK_MapData='" + this.FK_MapData + "';"
                                 + "select '轨迹图' Name,'FlowChart' No,FrmTrackSta Sta,FrmTrack_X X,FrmTrack_Y Y,FrmTrack_H H,FrmTrack_W  W from WF_Node where nodeid=" + this.FK_Node
+ " union select '审核组件' Name, 'FrmCheck' No,FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node where nodeid=" + this.FK_Node
+ " union select '子流程' Name,'SubFlowDtl' No,SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W from WF_Node  where nodeid=" + this.FK_Node
+ " union select '子线程' Name, 'ThreadDtl' No,FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W W from WF_Node where nodeid=" + this.FK_Node
+ " union select '流转自定义' Name,'FrmTransferCustom' No,FTCSta Sta,FTC_X X,FTC_Y Y,FTC_H H,FTC_W  W FROM WF_Node  where nodeid=" + this.FK_Node + ";";
                ;
                DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);


                ////用列名称进行比对 重新设置
                string mapAttrCols, frmBtnCols, frmRbCols, frmLabCols, sys_FrmLinkCols, sys_FrmImgCols, sys_FrmImgAthCols, sys_FrmAttachmentCols, sys_MapDtlCols, sys_FrmLineCols, figureComCols;
                mapAttrCols = "MyPK,FK_MapData,KeyOfEn,Name,DefVal,UIContralType,MyDataType,LGType,UIWidth,UIHeight,UIBindKey,UIRefKey,UIRefKeyText,UIVisible,UIIsEnable,UIIsLine,UIIsInput,Idx,IsSigan,X,Y,GUID,Tag,EditType,AtPara,ExtDefVal,ExtDefValText,MinLen,MaxLen,ExtRows,IsRichText,IsSupperText,Tip,ColSpan,ColSpanText,GroupID,GroupIDText";
                frmBtnCols = "MyPK,FK_MapData,Text,X,Y,IsView,IsEnable,BtnType,UAC,UACContext,EventType,EventContext,MsgOK,MsgErr,GUID,GroupID";
                frmRbCols = "MyPK,FK_MapData,KeyOfEn,EnumKey,Lab,IntKey,X,Y,GUID,Script,FieldsCfg,Tip";
                frmLabCols = "MyPK,FK_MapData,Text,X,Y,FontSize,FontColor,FontName,FontStyle,FontWeight,IsBold,IsItalic,GUID";
                sys_FrmLinkCols = "MyPK,FK_MapData,Text,URL,Target,X,Y,FontSize,FontColor,FontName,FontStyle,IsBold,IsItalic,GUID";
                sys_FrmImgCols = "MyPK,FK_MapData,ImgAppType,X,Y,H,W,ImgURL,ImgPath,LinkURL,LinkTarget,GUID,Tag0,SrcType,IsEdit,Name,EnPK,ImgSrcType";
                sys_FrmImgAthCols = "MyPK,FK_MapData,CtrlID,X,Y,H,W,IsEdit,GUID,Name,IsRequired";
                sys_FrmAttachmentCols = "MyPK,FK_MapData,NoOfObj,FK_Node,Name,Exts,SaveTo,Sort,X,Y,W,H,IsUpload,IsDelete,IsDownload,IsOrder,IsAutoSize,IsNote,IsShowTitle,UploadType,CtrlWay,AthUploadWay,AtPara,RowIdx,GroupID,GUID,DeleteWay,IsWoEnableWF,IsWoEnableSave,IsWoEnableReadonly,IsWoEnableRevise,IsWoEnableViewKeepMark,IsWoEnablePrint,IsWoEnableOver,IsWoEnableSeal,IsWoEnableTemplete,IsWoEnableCheck,IsWoEnableInsertFlow,IsWoEnableInsertFengXian,IsWoEnableMarks,IsWoEnableDown,IsRowLock,IsToHeLiuHZ,IsHeLiuHuiZong,IsTurn2Html,AthRunModel";
                sys_MapDtlCols = "No,Name,FK_MapData,PTable,GroupField,Model,ImpFixTreeSql,ImpFixDataSql,RowIdx,GroupID,RowsOfList,IsEnableGroupField,IsShowSum,IsShowIdx,IsCopyNDData,IsHLDtl,IsReadonly,IsShowTitle,IsView,IsInsert,IsDelete,IsUpdate,IsEnablePass,IsEnableAthM,IsEnableM2M,IsEnableM2MM,WhenOverSize,DtlOpenType,EditModel,X,Y,H,W,FrmW,FrmH,MTR,GUID,FK_Node,AtPara,IsExp,ImpModel,ImpSQLSearch,ImpSQLInit,ImpSQLFullOneRow,FilterSQLExp,SubThreadWorker,SubThreadWorkerText";
                sys_FrmLineCols = " MyPK,FK_MapData,X,Y,X1,Y1,X2,Y2,BorderWidth,BorderColor,GUID";
                figureComCols = "Name,No,Sta,X,Y,H,W";

                string[] tableCols = new string[11];
                ds.Tables[0].TableName = "Sys_MapAttr";
                tableCols[0] = mapAttrCols;

                ds.Tables[1].TableName = "Sys_FrmBtn";
                tableCols[1] = frmBtnCols;
                ds.Tables[2].TableName = "Sys_FrmRB";
                tableCols[2] = frmRbCols;
                ds.Tables[3].TableName = "Sys_FrmLab";
                tableCols[3] = frmLabCols;

                ds.Tables[4].TableName = "Sys_FrmLink";
                tableCols[4] = sys_FrmLineCols;
                ds.Tables[5].TableName = "Sys_FrmImg";
                tableCols[5] = sys_FrmImgCols;
                ds.Tables[6].TableName = "Sys_FrmImgAth";
                tableCols[6] = sys_FrmImgAthCols;
                ds.Tables[7].TableName = "Sys_FrmAttachment";
                tableCols[7] = sys_FrmAttachmentCols;
                ds.Tables[8].TableName = "Sys_MapDtl";
                tableCols[8] = sys_MapDtlCols;
                ds.Tables[9].TableName = "Sys_FrmLine";
                tableCols[9] = sys_FrmLineCols;
                ds.Tables[10].TableName = "FigureCom";
                tableCols[10] = figureComCols;

                #region 解决oracle大小写问题.
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    Dictionary<string, string> dicCols = new Dictionary<string, string>();
                    //将所有的列名进行转换（适应ORACLE） ORACLE 不区分大小写，都是大写
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        dicCols.Clear();
                        //dicCols = (new List<string>(tableCols[i].Split(','))).ToDictionary(m => m.ToString().Trim().ToLower(), m => m.Trim());
                        for (int m = 0; m < tableCols[i].Split(',').Length; m++)
                        {
                            dicCols.Add(tableCols[i].Split(',')[m].ToLower(), tableCols[i].Split(',')[m]);
                        }
                        DataTable dt = ds.Tables[i];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dicCols.ContainsKey(dc.ColumnName.ToLower()))
                            {
                                dc.ColumnName = dicCols[dc.ColumnName.ToLower()];
                            }
                        }
                    }
                }
                #endregion 解决oracle大小写问题.
              //  ds.WriteXml("c:\\aaaa.xml", XmlWriteMode.DiffGram);
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
            string docs = this.GetRequestVal("diagram");
            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, docs);
            //   BP.DA.DataType.WriteFile("c:\\xxxx.txt", docs);
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
                int refNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(KeyOfEn) FROM Sys_MapAttr WHERE UIBindKey='" + dr["No"] + "'", 0);
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
                return "err@"+ex.Message;
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
    }
}
