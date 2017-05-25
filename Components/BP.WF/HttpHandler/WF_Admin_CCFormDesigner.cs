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
using System.Linq;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {

        #region 执行父类的重写方法.
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner(HttpContext mycontext)
        {
            this.context = mycontext;
        }

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
                    case "loadform"://获取表单数据
                        MapData mapData = new MapData(this.FK_MapData);
                        return mapData.FormJson; //要返回的值.
                    case "ParseStringToPinyin": //转拼音方法.
                        string name = getUTF8ToString("name");
                        string flag = getUTF8ToString("flag");
                        if (flag == "true")
                            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true);
                        else
                            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, false);
                    case "DoType"://表单特殊元素保存公共方法
                        return DoTypeFunc();
                    case "Hiddenfielddata"://获取隐藏字段.
                        return BP.Sys.CCFormAPI.DB_Hiddenfielddata(this.FK_MapData);
                    case "HiddenFieldDelete": //删除隐藏字段.
                        string records = getUTF8ToString("records");
                        string FK_MapData = getUTF8ToString("FK_MapData");
                        MapAttr mapAttrs = new MapAttr();
                        int result = mapAttrs.Delete(MapAttrAttr.KeyOfEn, records, MapAttrAttr.FK_MapData, FK_MapData);
                        return result.ToString();
                    case "CcformElements"://杨玉慧 获取表单元素的JSON 字符串
                        return CCForm_AllElements_ResponseJson();
                    case "PublicNoNameCtrlCreate": //创建通用的控件.
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
                    case "NewField": //创建一个字段. 对应 FigureCreateCommand.js  里的方法.
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
                            return ex.Message;
                        }
                    case "CreateCheckGroup": //创建审核分组，暂时未实现.
                        BP.Sys.CCFormAPI.NewCheckGroup(this.FK_MapData, null, null);
                        return "true";
                    
                    case "SaveSFTable":
                        string enName = this.GetRequestVal("v2");
                        string chName = this.GetRequestVal("v1");
                        if (string.IsNullOrEmpty(chName) || string.IsNullOrEmpty(enName))
                            return "error:视图中的中英文名称不能为空。";

                        SFTable sf = new SFTable();
                        sf.No = enName;
                        sf.Name = chName;

                        sf.No = enName;
                        sf.Name = chName;

                        sf.FK_Val = enName;
                        sf.Save();
                        if (DBAccess.IsExitsObject(enName) == true)
                        {
                            /*已经存在此对象，检查一下是否有No,Name列。*/
                            sql = "SELECT No,Name FROM " + enName;
                            try
                            {
                                DBAccess.RunSQLReturnTable(sql);
                            }
                            catch (Exception ex)
                            {
                                return "您指定的表或视图(" + enName + ")，不包含No,Name两列，不符合ccflow约定的规则。技术信息:" + ex.Message;
                            }
                            return "true";
                        }
                        else
                        {
                            /*创建这个表，并且插入基础数据。*/
                            try
                            {
                                // 如果没有该表或视图，就要创建它。
                                sql = "CREATE TABLE " + enName + "(No varchar(30) NOT NULL,Name varchar(50) NULL)";
                                DBAccess.RunSQL(sql);
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('001','Item1')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('002','Item2')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('003','Item3')");
                            }
                            catch (Exception ex)
                            {
                                sf.DirectDelete();
                                return "error:创建物理表期间出现错误,可能是非法的物理表名.技术信息:" + ex.Message;
                            }
                        }
                        return "true"; /*创建成功后返回空值*/
                    case "FrmTempleteExp":  //导出表单.
                        MapData mdfrmtem = new MapData();
                        mdfrmtem.No = this.GetRequestVal("v1");
                        if (mdfrmtem.RetrieveFromDBSources() == 0)
                        {
                            if (this.GetRequestVal("v1").Contains("ND"))
                            {
                                int nodeId = int.Parse(this.GetRequestVal("v1").Replace("ND", ""));
                                Node nd123 = new Node(nodeId);
                                mdfrmtem.Name = nd123.Name;
                                mdfrmtem.PTable = this.GetRequestVal("v1");
                                mdfrmtem.EnPK = "OID";
                                mdfrmtem.Insert();
                            }
                        }

                        DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(mdfrmtem.No);
                        string file = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + this.GetRequestVal("v1") + ".xml";
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        ds.WriteXml(file);
                        // BP.Sys.PubClass.DownloadFile(file, mdfrmtem.Name + ".xml");
                        //this.DownLoadFile(System.Web.HttpContext.Current, file, mdfrmtem.Name);
                        return null;
                    case "FrmTempleteImp": //导入表单.
                        DataSet dsImp = new DataSet();
                        string fileImp = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + this.GetRequestVal("v1") + ".xml";
                        dsImp.ReadXml(fileImp); //读取文件.
                        MapData.ImpMapData(this.GetRequestVal("v1"), dsImp, true);
                        return "true";
                    case "NewHidF":
                        string fk_mapdataHid = this.GetRequestVal("v1");
                        string key = this.GetRequestVal("v2");
                        string myname = this.GetRequestVal("v3");
                        int dataType = int.Parse(this.GetRequestVal("v4"));
                        MapAttr mdHid = new MapAttr();
                        mdHid.MyPK = fk_mapdataHid + "_" + key;
                        mdHid.FK_MapData = fk_mapdataHid;
                        mdHid.KeyOfEn = key;
                        mdHid.Name = myname;
                        mdHid.MyDataType = dataType;
                        mdHid.HisEditType = EditType.Edit;
                        mdHid.MaxLen = 100;
                        mdHid.MinLen = 0;
                        mdHid.LGType = FieldTypeS.Normal;
                        mdHid.UIVisible = false;
                        mdHid.UIIsEnable = false;
                        mdHid.Insert();
                        return "true";
                    case "DelDtl":
                        MapDtl dtl = new MapDtl(this.GetRequestVal("v1"));
                        dtl.Delete();
                        return "true";
                    case "DelWorkCheck":
                        FrmWorkCheck check = new FrmWorkCheck();
                        check.No = this.GetRequestVal("v1");
                        check.Delete();
                        return "true";
                    case "DeleteFrm":
                        string delFK_Frm = this.GetRequestVal("v1");
                        MapData mdDel = new MapData(delFK_Frm);
                        mdDel.Delete();
                        sql = "@DELETE FROM Sys_MapData WHERE No='" + delFK_Frm + "'";
                        sql = "@DELETE FROM WF_FrmNode WHERE FK_Frm='" + delFK_Frm + "'";
                        DBAccess.RunSQLs(sql);
                        return "true";
                    case "FrmUp":
                    case "FrmDown":
                        FrmNode myfn = new FrmNode();
                        myfn.Retrieve(FrmNodeAttr.FK_Node, this.GetRequestVal("v1"), FrmNodeAttr.FK_Frm, this.GetRequestVal("v2"));
                        if (this.DoType == "FrmUp")
                            myfn.DoUp();
                        else
                            myfn.DoDown();
                        return "true";
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

            string str = "";
            if (isQuanPin == "1")
                str= BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true);
            else
                str = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, false);

            MapData md = new MapData();
            md.No = str;
            if (md.RetrieveFromDBSources() == 0)
                return str;
                
            return "err@表单ID:" + str + "已经被使用.";
        }
        public string NewFrmGuide_Create()
        {
            MapData md = new MapData();
            md.Name = this.GetRequestVal("TB_Name");
            md.No = this.GetRequestVal("TB_No");
            md.PTable = this.GetRequestVal("TB_PTable");

            md.No = this.Pub1.GetTextBoxByID("TB_No").Text;
            md.PTable = this.Pub1.GetTextBoxByID("TB_PTable").Text;
            md.FK_FrmSort = this.Pub1.GetDDLByID("DDL_FrmTree").SelectedValue;
            md.FK_FormTree = this.Pub1.GetDDLByID("DDL_FrmTree").SelectedValue;
            md.AppType = "0";//独立表单
            md.DBSrc = this.DBSrc;

            if (md.Name.Length == 0 || md.No.Length == 0 || md.PTable.Length == 0)
            {
                BP.Sys.PubClass.Alert("必填项不能为空.");
                return;
            }

            if (md.IsExits == true)
            {
                BP.Sys.PubClass.Alert("表单ID:" + md.No + "已经存在.");
                return;
            }

            md.HisFrmTypeInt = this.FrmType; //表单类型.

            switch ((BP.Sys.FrmType)(this.FrmType))
            {
                //自由，傻瓜，SL表单不做判断
                case BP.Sys.FrmType.FreeFrm:
                case BP.Sys.FrmType.FoolForm:
                    break;
                case BP.Sys.FrmType.Url:
                    string url = this.Pub1.GetTextBoxByID("TB_Url").Text;
                    if (string.IsNullOrEmpty(url))
                    {
                        BP.Sys.PubClass.Alert("必填项不可以为空");
                        return;
                    }
                    md.Url = url;
                    break;
                //如果是以下情况，导入模式
                case BP.Sys.FrmType.WordFrm:
                case BP.Sys.FrmType.ExcelFrm:
                    var file = Request.Files[0];
                    string savePath = null;
                    var ext = Path.GetExtension(file.FileName).ToLower(); //后缀

                    ext = Path.GetExtension(file.FileName).ToLower();

                    if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.ExcelFrm &&
                        ext != ".xls" && ext != ".xlsx")
                    {
                        BP.Sys.PubClass.Alert("上传的Excel文件格式错误.");
                        return;
                    }

                    if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.WordFrm &&
                        ext != ".doc" && ext != ".docx")
                    {
                        BP.Sys.PubClass.Alert("上传的Word文件格式错误.");
                        return;
                    }
                    savePath = BP.Sys.SystemConfig.PathOfDataUser + "FrmOfficeTemplate\\";
                    if (Directory.Exists(savePath) == false)
                        Directory.CreateDirectory(savePath);
                    file.SaveAs(savePath + this.Pub1.GetTextBoxByID("TB_No").Text + ext);

                    break;
                default:
                    throw new Exception("未知表单类型.");
            }
            md.Insert();

            if (md.HisFrmType == BP.Sys.FrmType.WordFrm || md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
            {
                /*把表单模版存储到数据库里 */
                this.Response.Redirect("/WF/Comm/En.htm?EnsName=BP.WF.Template.MapFrmExcels&PK=" + md.No, true);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm && this.Pub1.GetRadioButtonByID("RB_FrmGenerMode_2").Checked)
            {
                this.Response.Redirect("../FoolFormDesigner/ImpTableField.htm?DoType=New&FK_MapData=" + md.No);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                this.Response.Redirect("FormDesigner.htm?FK_MapData=" + md.No);
            }

            if (md.HisFrmType == BP.Sys.FrmType.FoolForm)
            {
                this.Response.Redirect("../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + md.No);
            }
        }
        #endregion 创建表单.


        public string LetLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            WebUser.SignInOfGener(emp);
            return null;
        }

        public string CCFormDesignerSL_Init()
        {
            return BP.WF.Glo.SilverlightDownloadUrl;
        }
        public string DesignerFrm_Init()
        {
            //根据不同的表单类型转入不同的表单设计器上去.
            BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
            if (md.HisFrmType == BP.Sys.FrmType.FoolForm)
            {
                /*傻瓜表单*/
                return "url@./FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + this.FK_MapData;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                /*自由表单*/
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
        /// 处理表单事件方法
        /// </summary>
        /// <returns></returns>
        public string DoFunc()
        {
            string sql = "";
            try
            {
                switch (this.DoType)
                {
                    case "CreateCheckGroup": //创建审核分组，暂时未实现.
                        BP.Sys.CCFormAPI.NewCheckGroup(FK_MapData, null, null);
                        return "true";
                    case "SaveSFTable":
                        string enName = this.GetRequestVal("v2");
                        string chName = this.GetRequestVal("v1");
                        if (string.IsNullOrEmpty(chName) || string.IsNullOrEmpty(enName))
                            return "error:视图中的中英文名称不能为空。";

                        SFTable sf = new SFTable();
                        sf.No = enName;
                        sf.Name = chName;

                        sf.No = enName;
                        sf.Name = chName;

                        sf.FK_Val = enName;
                        sf.Save();
                        if (DBAccess.IsExitsObject(enName) == true)
                        {
                            /*已经存在此对象，检查一下是否有No,Name列。*/
                            sql = "SELECT No,Name FROM " + enName;
                            try
                            {
                                DBAccess.RunSQLReturnTable(sql);
                            }
                            catch (Exception ex)
                            {
                                return "您指定的表或视图(" + enName + ")，不包含No,Name两列，不符合ccflow约定的规则。技术信息:" + ex.Message;
                            }
                            return "true";
                        }
                        else
                        {
                            /*创建这个表，并且插入基础数据。*/
                            try
                            {
                                // 如果没有该表或视图，就要创建它。
                                sql = "CREATE TABLE " + enName + "(No varchar(30) NOT NULL,Name varchar(50) NULL)";
                                DBAccess.RunSQL(sql);
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('001','Item1')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('002','Item2')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('003','Item3')");
                            }
                            catch (Exception ex)
                            {
                                sf.DirectDelete();
                                return "error:创建物理表期间出现错误,可能是非法的物理表名.技术信息:" + ex.Message;
                            }
                        }
                        return "true"; /*创建成功后返回空值*/
                    case "FrmTempleteExp":  //导出表单.
                        MapData mdfrmtem = new MapData();
                        mdfrmtem.No = this.GetRequestVal("v1");
                        if (mdfrmtem.RetrieveFromDBSources() == 0)
                        {
                            if (this.GetRequestVal("v1").Contains("ND"))
                            {
                                int nodeId = int.Parse(this.GetRequestVal("v1").Replace("ND", ""));
                                Node nd123 = new Node(nodeId);
                                mdfrmtem.Name = nd123.Name;
                                mdfrmtem.PTable = this.GetRequestVal("v1");
                                mdfrmtem.EnPK = "OID";
                                mdfrmtem.Insert();
                            }
                        }

                        DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(mdfrmtem.No);
                        string file = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + this.GetRequestVal("v1") + ".xml";
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        ds.WriteXml(file);
                        // BP.Sys.PubClass.DownloadFile(file, mdfrmtem.Name + ".xml");
                        //this.DownLoadFile(System.Web.HttpContext.Current, file, mdfrmtem.Name);
                        return null;
                    case "FrmTempleteImp": //导入表单.
                        DataSet dsImp = new DataSet();
                        string fileImp = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + this.GetRequestVal("v1") + ".xml";
                        dsImp.ReadXml(fileImp); //读取文件.
                        MapData.ImpMapData(this.GetRequestVal("v1"), dsImp, true);
                        return "true";
                    case "NewHidF":
                        string fk_mapdataHid = this.GetRequestVal("v1");
                        string key = this.GetRequestVal("v2");
                        string name = this.GetRequestVal("v3");
                        int dataType = int.Parse(this.GetRequestVal("v4"));
                        MapAttr mdHid = new MapAttr();
                        mdHid.MyPK = fk_mapdataHid + "_" + key;
                        mdHid.FK_MapData = fk_mapdataHid;
                        mdHid.KeyOfEn = key;
                        mdHid.Name = name;
                        mdHid.MyDataType = dataType;
                        mdHid.HisEditType = EditType.Edit;
                        mdHid.MaxLen = 100;
                        mdHid.MinLen = 0;
                        mdHid.LGType = FieldTypeS.Normal;
                        mdHid.UIVisible = false;
                        mdHid.UIIsEnable = false;
                        mdHid.Insert();
                        return "true";
                    case "DelDtl":
                        MapDtl dtl = new MapDtl(this.GetRequestVal("v1"));
                        dtl.Delete();
                        return "true";
                    case "DelWorkCheck":
                        FrmWorkCheck check = new FrmWorkCheck();
                        check.No = this.GetRequestVal("v1");
                        check.Delete();
                        return "true";
                    case "DeleteFrm":
                        string delFK_Frm = this.GetRequestVal("v1");
                        MapData mdDel = new MapData(delFK_Frm);
                        mdDel.Delete();
                        sql = "@DELETE FROM Sys_MapData WHERE No='" + delFK_Frm + "'";
                        sql = "@DELETE FROM WF_FrmNode WHERE FK_Frm='" + delFK_Frm + "'";
                        DBAccess.RunSQLs(sql);
                        return "true";
                    case "FrmUp":
                    case "FrmDown":
                        FrmNode myfn = new FrmNode();
                        myfn.Retrieve(FrmNodeAttr.FK_Node, this.GetRequestVal("v1"), FrmNodeAttr.FK_Frm, this.GetRequestVal("v2"));
                        if (this.DoType == "FrmUp")
                            myfn.DoUp();
                        else
                            myfn.DoDown();
                        return "true";
                    default:
                        return "err@" + this.DoType + " , 未设置此标记.";
                }
            }
            catch (Exception ex)
            {
                return "err@DoType 异常信息" + ex.Message;
            }
        }

        #region 功能界面 .
        /// <summary>
        /// 处理表单事件方法
        /// </summary>
        /// <returns></returns>
        public string DoTypeFunc()
        {
            string dotype = getUTF8ToString("DoType");
            string frmID = getUTF8ToString("FK_MapData");
            if (frmID == null)
                frmID = getUTF8ToString("FrmID");

            float x = 0;
            float y = 0;
            string no = "";
            string name = "";

            string v1 = getUTF8ToString("v1");
            string v2 = getUTF8ToString("v2");
            string v3 = getUTF8ToString("v3");
            string v4 = getUTF8ToString("v4");
            string v5 = getUTF8ToString("v5");
            string resutlStr = string.Empty;
            string sql = "";
            try
            {
                switch (dotype.Trim())
                {

                    case "PublicNoNameCtrlCreate": //创建通用的控件.
                        string ctrlType = getUTF8ToString("CtrlType");
                        try
                        {
                            no = getUTF8ToString("No");
                            name = getUTF8ToString("Name");
                            x = float.Parse(getUTF8ToString("x"));
                            y = float.Parse(getUTF8ToString("y"));
                            BP.Sys.CCFormAPI.CreatePublicNoNameCtrl(frmID, ctrlType, no, name, x, y);
                            return "true";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    case "NewField": //创建一个字段. 对应 FigureCreateCommand.js  里的方法.
                        try
                        {
                            BP.Sys.CCFormAPI.NewField(getUTF8ToString("FrmID"), getUTF8ToString("KeyOfEn"), getUTF8ToString("Name"),
                                int.Parse(getUTF8ToString("FieldType")),
                                float.Parse(getUTF8ToString("x")),
                               float.Parse(getUTF8ToString("y"))
                               );
                            return "true";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    case "CreateCheckGroup": //创建审核分组，暂时未实现.
                        BP.Sys.CCFormAPI.NewCheckGroup(FK_MapData, null, null);
                        return "true";
                    case "NewM2M":
                        string fk_mapdataM2M = v1;
                        string m2mName = v2;
                        MapM2M m2m = new MapM2M();
                        m2m.FK_MapData = v1;
                        m2m.NoOfObj = v2;
                        if (v3 == "0")
                        {
                            m2m.HisM2MType = M2MType.M2M;
                            m2m.Name = "新建一对多";
                        }
                        else
                        {
                            m2m.HisM2MType = M2MType.M2MM;
                            m2m.Name = "新建一对多多";
                        }

                        m2m.X = float.Parse(v4);
                        m2m.Y = float.Parse(v5);
                        m2m.MyPK = m2m.FK_MapData + "_" + m2m.NoOfObj;
                        if (m2m.IsExits)
                            return "error:多选名称:" + m2mName + "，已经存在。";
                        m2m.Insert();
                        return "true";
                    case "DelEnum":
                        //删除空数据.
                        BP.DA.DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData IS NULL OR FK_MapData='' ");

                        //获得要删除的枚举值.
                        string enumKey = getUTF8ToString("EnumKey");

                        // 检查这个物理表是否被使用.
                        sql = "SELECT  FK_MapData,KeyOfEn,Name FROM Sys_MapAttr WHERE UIBindKey='" + enumKey + "'";
                        DataTable dtEnum = DBAccess.RunSQLReturnTable(sql);
                        string msgDelEnum = "";
                        foreach (DataRow dr in dtEnum.Rows)
                        {
                            msgDelEnum += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                        }

                        if (msgDelEnum != "")
                            return "error:该枚举已经被如下字段所引用，您不能删除它。" + msgDelEnum;

                        sql = "DELETE FROM Sys_EnumMain WHERE No='" + enumKey + "'";
                        sql += "@DELETE FROM Sys_Enum WHERE EnumKey='" + enumKey + "' ";
                        DBAccess.RunSQLs(sql);
                        return "true";
                     
                    case "SaveSFTable":
                        string enName = v2;
                        string chName = v1;
                        if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
                            return "error:视图中的中英文名称不能为空。";

                        SFTable sf = new SFTable();
                        sf.No = enName;
                        sf.Name = chName;

                        sf.No = enName;
                        sf.Name = chName;

                        sf.FK_Val = enName;
                        sf.Save();
                        if (DBAccess.IsExitsObject(enName) == true)
                        {
                            /*已经存在此对象，检查一下是否有No,Name列。*/
                            sql = "SELECT No,Name FROM " + enName;
                            try
                            {
                                DBAccess.RunSQLReturnTable(sql);
                            }
                            catch (Exception ex)
                            {
                                return "您指定的表或视图(" + enName + ")，不包含No,Name两列，不符合ccflow约定的规则。技术信息:" + ex.Message;
                            }
                            return "true";
                        }
                        else
                        {
                            /*创建这个表，并且插入基础数据。*/
                            try
                            {
                                // 如果没有该表或视图，就要创建它。
                                sql = "CREATE TABLE " + enName + "(No varchar(30) NOT NULL,Name varchar(50) NULL)";
                                DBAccess.RunSQL(sql);
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('001','Item1')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('002','Item2')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('003','Item3')");
                            }
                            catch (Exception ex)
                            {
                                sf.DirectDelete();
                                return "error:创建物理表期间出现错误,可能是非法的物理表名.技术信息:" + ex.Message;
                            }
                        }
                        return "true"; /*创建成功后返回空值*/
                    case "FrmTempleteExp":  //导出表单.
                        MapData mdfrmtem = new MapData();
                        mdfrmtem.No = v1;
                        if (mdfrmtem.RetrieveFromDBSources() == 0)
                        {
                            if (v1.Contains("ND"))
                            {
                                int nodeId = int.Parse(v1.Replace("ND", ""));
                                Node nd123 = new Node(nodeId);
                                mdfrmtem.Name = nd123.Name;
                                mdfrmtem.PTable = v1;
                                mdfrmtem.EnPK = "OID";
                                mdfrmtem.Insert();
                            }
                        }

                        DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(mdfrmtem.No);
                        string file = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + v1 + ".xml";
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        ds.WriteXml(file);
                        // BP.Sys.PubClass.DownloadFile(file, mdfrmtem.Name + ".xml");
                        //this.DownLoadFile(System.Web.HttpContext.Current, file, mdfrmtem.Name);
                        return null;
                    case "FrmTempleteImp": //导入表单.
                        DataSet dsImp = new DataSet();
                        string fileImp = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + v1 + ".xml";
                        dsImp.ReadXml(fileImp); //读取文件.
                        MapData.ImpMapData(v1, dsImp, true);
                        return "true";
                    case "NewHidF":
                        string fk_mapdataHid = v1;
                        string key = v2;
                        name = v3;
                        int dataType = int.Parse(v4);
                        MapAttr mdHid = new MapAttr();
                        mdHid.MyPK = fk_mapdataHid + "_" + key;
                        mdHid.FK_MapData = fk_mapdataHid;
                        mdHid.KeyOfEn = key;
                        mdHid.Name = name;
                        mdHid.MyDataType = dataType;
                        mdHid.HisEditType = EditType.Edit;
                        mdHid.MaxLen = 100;
                        mdHid.MinLen = 0;
                        mdHid.LGType = FieldTypeS.Normal;
                        mdHid.UIVisible = false;
                        mdHid.UIIsEnable = false;
                        mdHid.Insert();
                        return "true";
                    case "DelDtl":
                        MapDtl dtl = new MapDtl(v1);
                        dtl.Delete();
                        return "true";
                    case "DelWorkCheck":
                        FrmWorkCheck check = new FrmWorkCheck();
                        check.No = v1;
                        check.Delete();
                        return "true";
                    case "DeleteFrm":
                        string delFK_Frm = v1;
                        MapData mdDel = new MapData(delFK_Frm);
                        mdDel.Delete();
                        sql = "@DELETE FROM Sys_MapData WHERE No='" + delFK_Frm + "'";
                        sql = "@DELETE FROM WF_FrmNode WHERE FK_Frm='" + delFK_Frm + "'";
                        DBAccess.RunSQLs(sql);
                        return "true";
                    case "FrmUp":
                    case "FrmDown":
                        FrmNode myfn = new FrmNode();
                        myfn.Retrieve(FrmNodeAttr.FK_Node, v1, FrmNodeAttr.FK_Frm, v2);
                        if (dotype == "FrmUp")
                            myfn.DoUp();
                        else
                            myfn.DoDown();
                        return "true";
                    default:
                        return "error:" + dotype + " , 后台执行错误，未设置此标记.";
                }
            }
            catch (Exception ex)
            {
                return "err@DoType 异常信息" + ex.Message;
            }
        }
        #endregion 功能界面方法.

        /// <summary>
        /// 获取自由表单所有元素
        /// </summary>
        /// <returns>json data</returns>
        private string CCForm_AllElements_ResponseJson()
        {
            try
            {
                MapData mapData = new MapData(this.FK_MapData);
                mapData.RetrieveFromDBSources();

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
                #region
                mapAttrCols = "MyPK,FK_MapData,KeyOfEn,Name,DefVal,UIContralType,MyDataType,LGType,UIWidth,UIHeight,UIBindKey,UIRefKey,UIRefKeyText,UIVisible,UIIsEnable,UIIsLine,UIIsInput,Idx,IsSigan,X,Y,GUID,Tag,EditType,AtPara,ExtDefVal,ExtDefValText,MinLen,MaxLen,ExtRows,IsRichText,IsSupperText,Tip,ColSpan,ColSpanText,GroupID,GroupIDText";
                frmBtnCols = "MyPK,FK_MapData,Text,X,Y,IsView,IsEnable,BtnType,UAC,UACContext,EventType,EventContext,MsgOK,MsgErr,GUID,GroupID";
                frmRbCols = "MyPK,FK_MapData,KeyOfEn,EnumKey,Lab,IntKey,X,Y,GUID,Script,FieldsCfg,Tip";
                frmLabCols = " MyPK,FK_MapData,Text,X,Y,FontSize,FontColor,FontName,FontStyle,FontWeight,IsBold,IsItalic,GUID";
                sys_FrmLinkCols = "MyPK,FK_MapData,Text,URL,Target,X,Y,FontSize,FontColor,FontName,FontStyle,IsBold,IsItalic,GUID";
                sys_FrmImgCols = "MyPK,FK_MapData,ImgAppType,X,Y,H,W,ImgURL,ImgPath,LinkURL,LinkTarget,GUID,Tag0,SrcType,IsEdit,Name,EnPK,ImgSrcType";
                sys_FrmImgAthCols = "MyPK,FK_MapData,CtrlID,X,Y,H,W,IsEdit,GUID,Name,IsRequired";
                sys_FrmAttachmentCols = "MyPK,FK_MapData,NoOfObj,FK_Node,Name,Exts,SaveTo,Sort,X,Y,W,H,IsUpload,IsDelete,IsDownload,IsOrder,IsAutoSize,IsNote,IsShowTitle,UploadType,CtrlWay,AthUploadWay,AtPara,RowIdx,GroupID,GUID,DeleteWay,IsWoEnableWF,IsWoEnableSave,IsWoEnableReadonly,IsWoEnableRevise,IsWoEnableViewKeepMark,IsWoEnablePrint,IsWoEnableOver,IsWoEnableSeal,IsWoEnableTemplete,IsWoEnableCheck,IsWoEnableInsertFlow,IsWoEnableInsertFengXian,IsWoEnableMarks,IsWoEnableDown,IsRowLock,IsToHeLiuHZ,IsHeLiuHuiZong,IsTurn2Html,AthRunModel";
                sys_MapDtlCols = "No,Name,FK_MapData,PTable,GroupField,Model,ImpFixTreeSql,ImpFixDataSql,RowIdx,GroupID,RowsOfList,IsEnableGroupField,IsShowSum,IsShowIdx,IsCopyNDData,IsHLDtl,IsReadonly,IsShowTitle,IsView,IsInsert,IsDelete,IsUpdate,IsEnablePass,IsEnableAthM,IsEnableM2M,IsEnableM2MM,WhenOverSize,DtlOpenType,DtlShowModel,X,Y,H,W,FrmW,FrmH,MTR,GUID,FK_Node,AtPara,IsExp,IsImp,IsEnableSelectImp,ImpSQLSearch,ImpSQLInit,ImpSQLFull,FilterSQLExp,SubThreadWorker,SubThreadWorkerText";
                sys_FrmLineCols = " MyPK,FK_MapData,X,Y,X1,Y1,X2,Y2,BorderWidth,BorderColor,GUID";
                figureComCols = "Name,No,Sta,X,Y,H,W";
                #endregion
                string[] tableCols = new string[11];

                ds.Tables[0].TableName = "MapAttr";
                tableCols[0] = mapAttrCols;
                ds.Tables[1].TableName = "FrmBtn";
                tableCols[1] = frmBtnCols;
                ds.Tables[2].TableName = "FrmRb";
                tableCols[2] = frmRbCols;
                ds.Tables[3].TableName = "FrmLab";
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

                Dictionary<string, string> dicCols = new Dictionary<string, string>();
                //将所有的列名进行转换（适应ORACLE） ORACLE 不区分大小写，都是大写
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    dicCols = (new List<string>(tableCols[i].Split(','))).ToDictionary(m => m.ToString().Trim().ToLower(), m => m.Trim());
                    DataTable dt = ds.Tables[i];
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dicCols.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            dc.ColumnName = dicCols[dc.ColumnName.ToLower()];
                        }
                    }
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
            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, this.GetRequestVal("diagram"));
            return "保存成功.";
        }

        #region tables
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
                return ex.Message;
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

        #region 方法 Home
        public string Home_Init()
        {
            string no = this.GetRequestVal("No");

            MapData md = new MapData(no);

            // 基础信息.
            Hashtable ht = new Hashtable();
            ht.Add("No", no);
            ht.Add("Name", md.Name);
            ht.Add("PTable", md.PTable);
            ht.Add("FrmTypeT", md.HisFrmTypeText);
            ht.Add("FrmTreeName", md.FK_FormTreeText);

            //统计信息.
            if (DBAccess.IsExitsObject(md.PTable) == true)
                ht.Add("SumDataNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + md.PTable)); //数据量.
            else
                ht.Add("SumDataNum", 0); //数据量.

            ht.Add("SumAttrNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "'")); //字段数量.
            ht.Add("SumAttrFK", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "' AND LGType=2 ")); //外键.
            ht.Add("SumAttrEnum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "' AND LGType=1 ")); //外键.

            ht.Add("MapFrmFrees", "../../Comm/En.htm?EnsName=BP.WF.Template.MapFrmFrees&PK=" + no); //自由表单属性.
            ht.Add("MapFrmFools", "../../Comm/En.htm?EnsName=BP.WF.Template.MapFrmFools&PK=" + no); //傻瓜表单属性.
            ht.Add("MapFrmExcels", "../../Comm/En.htm?EnsName=BP.WF.Template.MapFrmExcels&PK=" + no); //Excel表单属性.
            ht.Add("MapDataURLs", "../../Comm/En.htm?EnsName=BP.WF.Template.MapDataURLs&PK=" + no);  //嵌入式表单属性.

            return BP.DA.DataType.ToJsonEntityModel(ht);
        }
        #endregion 方法 Home

        #region 字段列表 的操作
        /// <summary>
        /// 初始化字段列表.
        /// </summary>
        /// <returns></returns>
        public string FiledsList_Init()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData);
            foreach (MapAttr item in attrs)
            {
                if (item.LGType == FieldTypeS.Enum)
                {
                    SysEnumMain se = new SysEnumMain(item.UIBindKey);
                    item.UIRefKey = se.CfgVal;
                    continue;
                }

                if (item.LGType == FieldTypeS.FK)
                {
                    item.UIRefKey = item.UIBindKey;
                    continue;
                }

                item.UIRefKey = "无";
            }
            return attrs.ToJson();
        }

        /// <summary>
        /// 删除字段
        /// </summary>
        /// <returns></returns>
        public string FiledsList_Delete()
        {
            MapAttr attr = new MapAttr(this.MyPK);
            if (attr.Delete() == 1)
                return "删除成功！";

            return "err@删除成功！";
        }
        #endregion 字段列表 的操作
    }
}
