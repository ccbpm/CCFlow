using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.BPMN;
using BP.Sys;
using BP.En;
using BP.WF.Template;
using System.Collections.Generic;
using BP.WF;
using LitJson;

namespace CCFlow.WF.Admin.CCFormDesigner.common
{
    /// <summary>
    /// FormDesignerController 的摘要说明
    /// by dgq FormDesiner service
    /// </summary>
    public class FormDesignerController : IHttpHandler, IRequiresSessionState
    {
        #region 全局变量IRequiresSessionState
        /// <summary>
        /// 表单编号
        /// </summary>
        private string FK_MapData
        {
            get
            {
                return getUTF8ToString("FK_MapData");
            }
        }
        /// <summary>
        /// http请求
        /// </summary>
        public HttpContext context
        {
            get;
            set;
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext myContext)
        {
            context = myContext;
            string action = string.Empty;
            //返回值
            string msg = string.Empty;
            if (string.IsNullOrEmpty(context.Request["action"]) == false)
                action = context.Request["action"].ToString();

            try
            {

                switch (action)
                {
                    case "loadform"://获取表单数据
                        MapData mapData = new MapData(this.FK_MapData);
                        msg = mapData.FormJson; //要返回的值.
                        break;
                    case "SaveForm": //保存表单数据.
                        try
                        {
                            string diagram = getUTF8ToString("diagram");//表单 H5 格式.
                            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, diagram); //执行保存.
                            msg = "true";
                        }
                        catch (Exception ex)
                        {
                            Log.DebugWriteError(ex.StackTrace);
                            msg = "error:表单格式不正确，保存失败。" + ex.StackTrace;
                        }
                        break;
                    case "ParseStringToPinyin": //转拼音方法.
                        string name = getUTF8ToString("name");
                        string flag = getUTF8ToString("flag");
                        if (flag == "true")
                            msg = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true);
                        else
                            msg = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, false);
                        break;
                    case "LetLogin":    //使管理员登录
                        msg = WebUser.No == "admin" ? string.Empty : LetAdminLogin("CH", true);
                        break;
                    case "DoType"://表单特殊元素保存公共方法
                        msg = DoType();
                        break;
                    case "GetEnumerationList": //获取所有枚举
                    case "GetSFTableList": //获取所有的外键表.
                        string pageNumberStr = getUTF8ToString("pageNumber");
                        int pageNumber = 1;
                        if (string.IsNullOrEmpty(pageNumberStr) == false)
                            pageNumber = int.Parse(pageNumberStr);

                        string pageSizeStr = getUTF8ToString("pageSize");
                        int pageSize = 9999;
                        if (string.IsNullOrEmpty(pageSizeStr) == false)
                            pageSize = int.Parse(pageSizeStr);

                        //调用API获得数据.
                        if (action == "GetSFTableList")
                            msg = BP.Sys.CCFormAPI.DB_SFTableList(pageNumber, pageSize);
                        else
                            msg = BP.Sys.CCFormAPI.DB_EnumerationList(pageNumber, pageSize); //调用API获得数据.

                        // BP.DA.DataType.WriteFile("c:\\sss.txt", msg);
                        break;
                    case "Hiddenfielddata"://获取隐藏字段.
                        msg = BP.Sys.CCFormAPI.DB_Hiddenfielddata(this.FK_MapData);
                        break;
                    case "HiddenFieldDelete": //删除隐藏字段.
                        string records = getUTF8ToString("records");
                        string FK_MapData = getUTF8ToString("FK_MapData");
                        MapAttr mapAttrs = new MapAttr();
                        int result = mapAttrs.Delete(MapAttrAttr.KeyOfEn, records, MapAttrAttr.FK_MapData, FK_MapData);
                        msg = result.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }
            if (string.IsNullOrEmpty(msg))
                msg = "";

            //组装ajax字符串格式,返回调用客户端
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            context.Response.Expires = 0;
            context.Response.Write(msg);
            context.Response.End();
        }
        /// <summary>
        /// 处理表单事件方法
        /// </summary>
        /// <returns></returns>
        public string DoType()
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
            string sql = "";
            try
            {
                switch (dotype.Trim())
                {
                    case "SaveEnum":
                    case "NewEnum":
                        string enumName = getUTF8ToString("EnumName");
                        string enumKey1 = getUTF8ToString("EnumKey");
                        string cfgVal = getUTF8ToString("Vals");

                        //调用接口执行保存.
                        BP.Sys.CCFormAPI.SaveEnum(enumKey1, enumName, cfgVal);
                        return "true";
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
                    case "NewSFTableField": //创建一个SFTable字段.
                        try
                        {
                            string fk_mapdata = getUTF8ToString("FK_MapData");
                            string keyOfEn = getUTF8ToString("KeyOfEn");
                            string fieldDesc = getUTF8ToString("Name");
                            string sftable = getUTF8ToString("UIBindKey");
                            x = float.Parse(getUTF8ToString("x"));
                            y = float.Parse(getUTF8ToString("y"));

                            //调用接口,执行保存.
                            BP.Sys.CCFormAPI.SaveFieldSFTable(fk_mapdata, keyOfEn, fieldDesc, sftable, x, y);
                            return "true";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    case "NewEnumField": //创建一个字段. 对应 FigureCreateCommand.js  里的方法.
                        try
                        {
                            UIContralType ctrl = UIContralType.RadioBtn;
                            string ctrlDoType = getUTF8ToString("ctrlDoType");
                            if (ctrlDoType == "DDL")
                                ctrl = UIContralType.DDL;
                            else
                                ctrl = UIContralType.RadioBtn;

                            string fk_mapdata = getUTF8ToString("FK_MapData");
                            string keyOfEn = getUTF8ToString("KeyOfEn");
                            string fieldDesc = getUTF8ToString("Name");
                            string enumKeyOfBind = getUTF8ToString("UIBindKey"); //要绑定的enumKey.
                            x = float.Parse(getUTF8ToString("x"));
                            y = float.Parse(getUTF8ToString("y"));

                            BP.Sys.CCFormAPI.NewEnumField(frmID, keyOfEn, fieldDesc, enumKeyOfBind, ctrl, x, y);
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
                    case "DelSFTable": /* 删除自定义的物理表. */
                        // 检查这个物理表是否被使用。
                        sql = "SELECT FK_MapData,KeyOfEn,Name FROM Sys_MapAttr WHERE UIBindKey='" + v1 + "'";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        string msgDel = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            msgDel += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                        }
                        if (msgDel != "")
                            return "error:该数据表已经被如下字段所引用，您不能删除它。" + msgDel;

                        SFTable sfDel = new SFTable();
                        sfDel.No = v1;
                        sfDel.DirectDelete();
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
        public string SaveEn(string vals)
        {
            Entity en = null;
            try
            {
                AtPara ap = new AtPara(vals);
                string enName = ap.GetValStrByKey("EnName");
                string pk = ap.GetValStrByKey("PKVal");
                en = ClassFactory.GetEn(enName);
                en.ResetDefaultVal();

                if (en == null)
                    throw new Exception("无效的类名:" + enName);

                if (string.IsNullOrEmpty(pk) == false)
                {
                    en.PKVal = pk;
                    en.RetrieveFromDBSources();
                }

                foreach (string key in ap.HisHT.Keys)
                {
                    if (key == "PKVal")
                        continue;
                    en.SetValByKey(key, ap.HisHT[key].ToString().Replace('^', '@'));
                }
                en.Save();
                return en.PKVal as string;
            }
            catch (Exception ex)
            {
                if (en != null)
                    en.CheckPhysicsTable();
                return "Error:" + ex.Message;
            }
        }
        /// <summary>
        /// 让admin 登陆
        /// </summary>
        /// <param name="lang">当前的语言</param>
        /// <returns>成功则为空，有异常时返回异常信息</returns>
        public string LetAdminLogin(string lang, bool islogin)
        {
            try
            {
                if (islogin)
                {
                    BP.Port.Emp emp = new BP.Port.Emp("admin");
                    WebUser.SignInOfGener(emp);
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return string.Empty;
        }

        #region 将表单串格式化存入数据库
      
        #endregion End

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}