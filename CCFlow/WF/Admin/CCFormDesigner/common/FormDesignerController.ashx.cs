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
        public HttpContext _Context
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
            return HttpUtility.UrlDecode(_Context.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            _Context = context;

            if (_Context == null) return;

            string action = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["action"]))
                action = context.Request["action"].ToString();

            switch (action)
            {
                case "loadform"://获取表单数据
                    s_responsetext = Form_LoadFormJsonData();
                    break;
                case "saveform"://保存表单数据
                    s_responsetext = Form_Save();
                    break;
                case "ParseStringToPinyin":// 转拼音方法
                    s_responsetext = ParseStringToPinyin();
                    break;
                case "LetLogin":    //使管理员登录
                    s_responsetext = WebUser.No == "admin" ? string.Empty : LetAdminLogin("CH", true);
                    break;
                case "DoType"://表单特殊元素保存公共方法
                    s_responsetext = DoType();
                    break;
                case "GetEnumerationList"://获取所有枚举
                    s_responsetext = GetEnumerationList();
                    break;
                case "Hiddenfielddata"://获取隐藏字段
                    s_responsetext = Hiddenfielddata();
                    break;
                case "HiddenFieldDelete"://删除隐藏字段
                    s_responsetext = HiddenFieldDelete();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            context.Response.Expires = 0;
            context.Response.Write(s_responsetext);
            context.Response.End();
        }

        /// <summary>
        /// 删除隐藏字段
        /// </summary>
        /// <returns></returns>
        public string HiddenFieldDelete()
        {
            string StrSuccess = "";
            string records = getUTF8ToString("records");
            string FK_MapData = getUTF8ToString("FK_MapData");
            MapAttr mapAttrs = new MapAttr();
            int Success = mapAttrs.Delete(MapAttrAttr.KeyOfEn, records, MapAttrAttr.FK_MapData, FK_MapData);

            StrSuccess = Success.ToString();
            return StrSuccess;

        }

        /// <summary>
        /// 获取表单所有隐藏字段数据集
        /// </summary>
        /// <returns></returns>
        public string Hiddenfielddata()
        {

            string newsid = getUTF8ToString("FK_MapData");
            int RowCount = 0;
            //当前页
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            MapAttrs mapAttrs = new MapAttrs();
            QueryObject obj = new QueryObject(mapAttrs);
            obj.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.UIVisible, "0");
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.EditType, "0");
            RowCount = obj.GetCount();
            //查询
            obj.DoQuery(MapAttrAttr.MyPK, iPageSize, iPageNumber);

            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(mapAttrs, RowCount);
        }

        /// <summary>
        /// 获取所有枚举
        /// </summary>
        public string GetEnumerationList()
        {

            int RowCount = 0;
            //当前页
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            SysEnumMains enumMains = new SysEnumMains();
            QueryObject obj = new QueryObject(enumMains);
            RowCount = obj.GetCount();
            //查询
            obj.DoQuery(SysEnumMainAttr.No, iPageSize, iPageNumber);

            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(enumMains, RowCount);

        }
        /// <summary>
        /// 获取表单Json数据
        /// </summary>
        /// <returns>string json</returns>
        private string Form_LoadFormJsonData()
        {
            try
            {
                MapData mapData = new MapData(this.FK_MapData);
                return mapData.FormJson;
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.Message);

                MapData mapData = new MapData(this.FK_MapData);
                mapData.CheckPhysicsTable();
                mapData.FormJson = "";
                return "";
            }
        }

        /// <summary>
        /// 保存表单图信息
        /// </summary>
        /// <returns></returns>
        private string Form_Save()
        {
            try
            {
                //表单格式.
                string diagram = getUTF8ToString("diagram");
                //表单图.
                string png = getUTF8ToString("png");
                JsonData jd = JsonMapper.ToObject(diagram);
                if (jd.IsObject == true)
                {
                    JsonData form_MapData = jd["c"];
                    //直接保存表单图信息
                    MapData mapData = new MapData(this.FK_MapData);
                    mapData.FrmW = float.Parse(form_MapData["width"].ToJson());
                    mapData.FrmH = float.Parse(form_MapData["height"].ToJson());
                    mapData.Update();
                    //表单描述文件直接保存到数据库.
                    mapData.FormJson = diagram;
                    //格式化表单串
                    //FormatDiagram2Json(jd);
                    MapData.SaveCCForm20(this.FK_MapData, FormatDiagram2Json(jd));
                    return "true";
                }
                return "error:表单格式不正确，保存失败。";
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// 处理表单事件方法
        /// </summary>
        /// <returns></returns>
        public string DoType()
        {
            string dotype = getUTF8ToString("DoType");
            string v1 = getUTF8ToString("v1");
            string v2 = getUTF8ToString("v2");
            string v3 = getUTF8ToString("v3");
            string v4 = getUTF8ToString("v4");
            string v5 = getUTF8ToString("v5");
            string sql = "";
            try
            {
                switch (dotype)
                {
                    case "CreateCheckGroup":
                        string gKey = v1;
                        string gName = v2;
                        string enName1 = v3;

                        MapAttr attrN = new MapAttr();
                        int i = attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_Note");
                        i += attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_Checker");
                        i += attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_RDT");
                        if (i > 0)
                            return "error:前缀已经使用：" + gKey + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";

                        GroupField gf = new GroupField();
                        gf.Lab = gName;
                        gf.EnName = enName1;
                        gf.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_Note";
                        attrN.Name = "审核意见";
                        attrN.MyDataType = DataType.AppString;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = true;
                        attrN.MaxLen = 4000;
                        attrN.GroupID = gf.OID;
                        attrN.UIHeight = 23 * 3;
                        attrN.Idx = 1;
                        attrN.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_Checker";
                        attrN.Name = "审核人";// "审核人";
                        attrN.MyDataType = DataType.AppString;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.MaxLen = 50;
                        attrN.MinLen = 0;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = false;
                        attrN.DefVal = "@WebUser.No";
                        attrN.UIIsEnable = false;
                        attrN.GroupID = gf.OID;
                        attrN.IsSigan = true;
                        attrN.Idx = 2;
                        attrN.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_RDT";
                        attrN.Name = "审核日期"; // "审核日期";
                        attrN.MyDataType = DataType.AppDateTime;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = false;
                        attrN.DefVal = "@RDT";
                        attrN.UIIsEnable = false;
                        attrN.GroupID = gf.OID;
                        attrN.Idx = 3;
                        attrN.Insert();

                        /*
                         * 判断是否是节点设置的审核分组，如果是就为节点设置焦点字段。
                         */
                        string frmID = attrN.FK_MapData;
                        frmID = frmID.Replace("ND", "");
                        int nodeid = 0;
                        try
                        {
                            nodeid = int.Parse(frmID);
                        }
                        catch
                        {
                            //转化不成功就是不是节点表单字段.
                            return "error:只能节点表单才可以使用审核分组组件。";
                        }

                        Node nd = new Node();
                        nd.NodeID = nodeid;
                        if (nd.RetrieveFromDBSources() != 0 && string.IsNullOrEmpty(nd.FocusField) == true)
                        {
                            nd.FocusField = "@" + gKey + "_Note";
                            nd.Update();
                        }
                        return "true";
                    case "NewDtl":
                        MapDtl dtlN = new MapDtl();
                        dtlN.No = v1;
                        if (dtlN.RetrieveFromDBSources() != 0)
                            return "error:从表已存在";
                        dtlN.Name = v1;
                        dtlN.FK_MapData = v2;
                        dtlN.PTable = v1;
                        dtlN.Insert();
                        dtlN.IntMapAttrs();
                        return "true";
                    case "DelM2M":
                        MapM2M m2mDel = new MapM2M();
                        m2mDel.MyPK = v1;
                        m2mDel.Delete();
                        //M2M m2mData = new M2M();
                        //m2mData.Delete(M2MAttr.FK_MapData, v1);
                        return "true";
                    case "NewAthM": // 新建 NewAthM. 
                        string fk_mapdataAth = v1;
                        string athName = v2;

                        BP.Sys.FrmAttachment athM = new FrmAttachment();
                        athM.MyPK = athName;
                        if (athM.IsExits)
                            return "error:多选名称:" + athName + "，已经存在。";

                        athM.X = float.Parse(v3);
                        athM.Y = float.Parse(v4);
                        athM.Name = "多文件上传";
                        athM.FK_MapData = fk_mapdataAth;
                        athM.Insert();
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
                        BP.DA.DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData Is null or FK_MapData='' ");

                        // 检查这个物理表是否被使用。
                        sql = "SELECT  * FROM Sys_MapAttr WHERE UIBindKey='" + v1 + "'";
                        DataTable dtEnum = DBAccess.RunSQLReturnTable(sql);
                        string msgDelEnum = "";
                        foreach (DataRow dr in dtEnum.Rows)
                        {
                            msgDelEnum += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                        }

                        if (msgDelEnum != "")
                            return "error:该枚举已经被如下字段所引用，您不能删除它。" + msgDelEnum;

                        sql = "DELETE FROM Sys_EnumMain WHERE No='" + v1 + "'";
                        sql += "@DELETE FROM Sys_Enum WHERE EnumKey='" + v1 + "' ";
                        DBAccess.RunSQLs(sql);
                        return "true";
                    case "DelSFTable": /* 删除自定义的物理表. */
                        // 检查这个物理表是否被使用。
                        sql = "SELECT  * FROM Sys_MapAttr WHERE UIBindKey='" + v1 + "'";
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
                                // 如果没有该表或者视图，就要创建它。
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

                        DataSet ds = mdfrmtem.GenerHisDataSet();
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
                        string name = v3;
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
                    case "SaveFlowFrm":
                        // 转化参数意义.
                        string vals = v1;
                        string fk_Node = v2;
                        string fk_flow = v3;
                        bool isPrint = false;
                        if (v5 == "1")
                            isPrint = true;

                        bool isReadonly = false;
                        if (v4 == "1")
                            isReadonly = true;

                        string msg = this.SaveEn(vals);
                        if (msg.Contains("Error"))
                            return msg;

                        string fk_frm = msg;
                        Frm fm = new Frm();
                        fm.No = fk_frm;
                        fm.Retrieve();

                        FrmNode fn = new FrmNode();
                        if (fn.Retrieve(FrmNodeAttr.FK_Frm, fk_frm,
                            FrmNodeAttr.FK_Node, fk_Node) == 1)
                        {
                          //  fn.IsEdit = !isReadonly;
                            fn.IsPrint = isPrint;
                            fn.FK_Flow = fk_flow;
                            fn.Update();
                            BP.DA.DBAccess.RunSQL("UPDATE Sys_MapData SET FK_FrmSort='01',AppType=1  WHERE No='" + fk_frm + "'");
                            return fk_frm;
                        }

                        fn.FK_Frm = fk_frm;
                        fn.FK_Flow = fk_flow;
                        fn.FK_Node = int.Parse(fk_Node);
                       // fn.IsEdit = !isReadonly;
                        fn.IsPrint = isPrint;
                        fn.Idx = 100;
                        fn.FK_Flow = fk_flow;
                        fn.Insert();

                        MapData md = new MapData();
                        md.No = fm.No;
                        if (md.RetrieveFromDBSources() == 0)
                        {
                            md.Name = fm.Name;
                            md.EnPK = "OID";
                            md.Insert();
                        }

                        MapAttr attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "OID";
                        attr.Name = "WorkID";
                        attr.MyDataType = BP.DA.DataType.AppInt;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "0";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();

                        attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "FID";
                        attr.Name = "FID";
                        attr.MyDataType = BP.DA.DataType.AppInt;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "0";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();

                        attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "RDT";
                        attr.Name = "记录日期";
                        attr.MyDataType = BP.DA.DataType.AppDateTime;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "@RDT";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();
                        return fk_frm;
                    default:
                        return "error:" + dotype + " , 未设置此标记.";
                }
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
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
        /// 转拼音方法
        /// </summary>
        /// <returns></returns>
        public string ParseStringToPinyin()
        {
            string name = getUTF8ToString("name");
            string flag = getUTF8ToString("flag");
            string s = string.Empty; ;
            try
            {
                if (flag == "true")
                {
                    s = BP.DA.DataType.ParseStringToPinyin(name);
                    if (s.Length > 15)
                        s = BP.DA.DataType.ParseStringToPinyinWordFirst(name);
                }
                else
                {
                    s = BP.DA.DataType.ParseStringToPinyinJianXie(name);
                }

                s = s.Trim().Replace(" ", "");
                s = s.Trim().Replace(" ", "");
                //常见符号
                s = s.Replace(",", "").Replace(".", "").Replace("，", "").Replace("。", "").Replace("!", "");
                s = s.Replace("*", "").Replace("@", "").Replace("#", "").Replace("~", "").Replace("|", "");
                s = s.Replace("$", "").Replace("%", "").Replace("&", "").Replace("（", "").Replace("）", "").Replace("【", "").Replace("】", "");
                s = s.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("/", "");
                if (s.Length > 0)
                {
                    //去除开头数字
                    int iHead = 0;
                    string headStr = s.Substring(0, 1);
                    while (int.TryParse(headStr, out iHead) == true)
                    {
                        //替换为空
                        s = s.Substring(1);
                        if (s.Length > 0) headStr = s.Substring(0, 1);
                    }
                }
                return s;
            }
            catch
            {
                return "false";
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
                    WebUser.SignInOfGener(emp, lang, "admin", true, false);
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return string.Empty;
        }

        #region 将表单串格式化存入数据库
        /// <summary>
        /// 将表单设计串格式化为Json
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private DataSet FormatDiagram2Json(JsonData formData)
        {
            DataSet form_DS = Form_InitDataSource();

            DataTable
            dtLine = form_DS.Tables[EEleTableNames.Sys_FrmLine],
            dtBtn = form_DS.Tables[EEleTableNames.Sys_FrmBtn],
            dtLabel = form_DS.Tables[EEleTableNames.Sys_FrmLab],
            dtLink = form_DS.Tables[EEleTableNames.Sys_FrmLink],
            dtImg = form_DS.Tables[EEleTableNames.Sys_FrmImg],
            dtEle = form_DS.Tables[EEleTableNames.Sys_FrmEle],
            dtImgAth = form_DS.Tables[EEleTableNames.Sys_FrmImgAth],
            dtMapAttr = form_DS.Tables[EEleTableNames.Sys_MapAttr],
            dtRDB = form_DS.Tables[EEleTableNames.Sys_FrmRB],
            dtlDT = form_DS.Tables[EEleTableNames.Sys_MapDtl],
            dtWorkCheck = form_DS.Tables[EEleTableNames.WF_Node],
            dtM2M = form_DS.Tables[EEleTableNames.Sys_MapM2M],
            dtAth = form_DS.Tables[EEleTableNames.Sys_FrmAttachment];

            //控件线集合 Line
            JsonData form_Lines = formData["m"]["connectors"];
            if (form_Lines.IsArray == true && form_Lines.Count > 0)
            {
                for (int iLine = 0, jLine = form_Lines.Count; iLine < jLine; iLine++)
                {
                    #region 线集合
                    JsonData line = form_Lines[iLine];
                    if (line.IsObject == true)
                    {
                        DataRow drline = dtLine.NewRow();

                        drline["MYPK"] = line["CCForm_MyPK"].ToString();
                        drline["FK_MAPDATA"] = this.FK_MapData;

                        drline["X"] = "0";
                        drline["Y"] = "0";

                        JsonData turningPoints = line["turningPoints"];
                        drline["X1"] = turningPoints[0]["x"].ToString();
                        drline["X2"] = turningPoints[1]["x"].ToString();
                        drline["Y1"] = turningPoints[0]["y"].ToString();
                        drline["Y2"] = turningPoints[1]["y"].ToString();

                        JsonData properties = line["properties"];
                        JsonData borderWidth = properties.GetObjectFromArrary_ByKeyValue("type", "LineWidth");
                        JsonData borderColor = properties.GetObjectFromArrary_ByKeyValue("type", "Color");
                        string strborderWidth = "2";
                        if (borderWidth != null && borderWidth["PropertyValue"] != null && !string.IsNullOrEmpty(borderWidth["PropertyValue"].ToString()))
                        {
                            strborderWidth = borderWidth["PropertyValue"].ToString();
                        }
                        string strBorderColor = "Black";
                        if (borderColor != null && borderColor["PropertyValue"] != null && !string.IsNullOrEmpty(borderColor["PropertyValue"].ToString()))
                        {
                            strBorderColor = borderColor["PropertyValue"].ToString();
                        }
                        drline["BorderWidth"] = strborderWidth;
                        drline["BorderColor"] = strBorderColor;
                        dtLine.Rows.Add(drline);
                    }
                    #endregion
                }
            }

            //其他控件，Label,Img,TextBox
            JsonData form_Controls = formData["s"]["figures"];
            if (form_Controls.IsArray == true && form_Controls.Count > 0)
            {
                for (int iControl = 0, jControl = form_Controls.Count; iControl < jControl; iControl++)
                {
                    JsonData control = form_Controls[iControl];
                    //不存在控件类型不进行处理，继续循环
                    if (control == null || control["CCForm_Shape"] == null)
                        continue;

                    if (control["CCForm_Shape"].ToString() == "Label")
                    {
                        #region 标签
                        DataRow drLab = dtLabel.NewRow();

                        drLab["MYPK"] = control["CCForm_MyPK"];
                        drLab["FK_MAPDATA"] = this.FK_MapData;
                        //坐标
                        JsonData style = control["style"];
                        JsonData vector = style["gradientBounds"];
                        drLab["X"] = vector[0].ToJson();
                        drLab["Y"] = vector[1].ToJson();

                        //属性集合
                        JsonData properties = control["properties"];
                        StringBuilder fontStyle = new StringBuilder();
                        for (int iProperty = 0; iProperty < properties.Count; iProperty++)
                        {
                            JsonData property = properties[iProperty];
                            if (property == null || !property.Keys.Contains("type") || property["type"] == null)
                                continue;

                            if (property["type"].ToString() == "SingleText")
                            {
                                drLab["TEXT"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                            }
                            else if (property["type"].ToString() == "Color")
                            {
                                drLab["FONTCOLOR"] = property["PropertyValue"] == null ? "#FF000000" : property["PropertyValue"].ToString();
                                fontStyle.Append(string.Format("color:{0};", drLab["FONTCOLOR"]));
                            }
                            else if (property["type"].ToString() == "TextFontFamily")
                            {
                                drLab["FontName"] = property["PropertyValue"] == null ? "Portable User Interface" : property["PropertyValue"].ToString();
                                if (property["PropertyValue"] != null)
                                    fontStyle.Append(string.Format("font-family:{0};", property["PropertyValue"].ToJson()));
                            }
                            else if (property["type"].ToString() == "TextFontSize")
                            {
                                drLab["FONTSIZE"] = property["PropertyValue"] == null ? "14" : property["PropertyValue"].ToString();
                                fontStyle.Append(string.Format("font-size:{0};", drLab["FONTSIZE"]));
                            }
                            else if (property["type"].ToString() == "FontWeight")
                            {
                                if (property["PropertyValue"] == null || property["PropertyValue"].ToString() == "normal")
                                {
                                    drLab["IsBold"] = "0";
                                    fontStyle.Append(string.Format("font-weight:normal;"));
                                }
                                else
                                {
                                    drLab["IsBold"] = "1";
                                    fontStyle.Append(string.Format("font-weight:{0};", property["PropertyValue"].ToString()));
                                }
                            }
                        }
                        drLab["FontStyle"] = fontStyle.ToString();
                        drLab["IsItalic"] = "0";//暂不处理斜体

                        dtLabel.Rows.Add(drLab);
                        #endregion end 标签
                    }
                    else if (control["CCForm_Shape"].ToString() == "Button")
                    {
                        #region 按钮 Button
                        DataRow drBtn = dtBtn.NewRow();

                        drBtn["MYPK"] = control["CCForm_MyPK"];
                        drBtn["FK_MAPDATA"] = this.FK_MapData;
                        //坐标
                        JsonData style = control["style"];
                        JsonData vector = style["gradientBounds"];
                        drBtn["X"] = vector[0].ToJson();
                        drBtn["Y"] = vector[1].ToJson();
                        drBtn["ISVIEW"] = "0";
                        drBtn["ISENABLE"] = "0";
                        //属性集合
                        JsonData properties = control["properties"];
                        for (int iProperty = 0; iProperty < properties.Count; iProperty++)
                        {
                            JsonData property = properties[iProperty];
                            if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                                continue;

                            if (property["property"].ToString() == "primitives.1.str")
                            {
                                drBtn["TEXT"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                            }
                            else if (property["property"].ToString() == "ButtonEvent")
                            {
                                drBtn["EVENTTYPE"] = property["PropertyValue"] == null ? "0" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "BtnEventDoc")
                            {
                                drBtn["EVENTCONTEXT"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                            }
                        }
                        dtBtn.Rows.Add(drBtn);
                        #endregion
                    }
                    else if (control["CCForm_Shape"].ToString() == "HyperLink")
                    {
                        #region 超链接 HyperLink
                        DataRow drLink = dtLink.NewRow();

                        drLink["MYPK"] = control["CCForm_MyPK"];
                        drLink["FK_MAPDATA"] = this.FK_MapData;
                        //坐标
                        JsonData vector = control["style"]["gradientBounds"];
                        drLink["X"] = vector[0].ToJson();
                        drLink["Y"] = vector[1].ToJson();

                        //属性集合
                        JsonData properties = control["properties"];
                        StringBuilder fontStyle = new StringBuilder();
                        for (int iProperty = 0; iProperty < properties.Count; iProperty++)
                        {
                            JsonData property = properties[iProperty];
                            if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                                continue;

                            if (property["property"].ToString() == "primitives.0.str")
                            {
                                drLink["TEXT"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "primitives.0.style.fillStyle")
                            {
                                drLink["FONTCOLOR"] = property["PropertyValue"] == null ? "#FF000000" : property["PropertyValue"].ToString();
                                fontStyle.Append(string.Format("color:{0};", drLink["FONTCOLOR"]));
                            }
                            else if (property["property"].ToString() == "primitives.0.font")
                            {
                                drLink["FontName"] = property["PropertyValue"] == null ? "Portable User Interface" : property["PropertyValue"].ToString();
                                if (property["PropertyValue"] != null)
                                    fontStyle.Append(string.Format("font-family:{0};", property["PropertyValue"].ToJson()));
                            }
                            else if (property["property"].ToString() == "primitives.0.size")
                            {
                                drLink["FONTSIZE"] = property["PropertyValue"] == null ? "14" : property["PropertyValue"].ToString();
                                fontStyle.Append(string.Format("font-size:{0};", drLink["FONTSIZE"]));
                            }
                            else if (property["property"].ToString() == "primitives.0.fontWeight")
                            {
                                if (property["PropertyValue"] == null || property["PropertyValue"].ToString() == "normal")
                                {
                                    drLink["IsBold"] = "0";
                                    fontStyle.Append(string.Format("font-weight:normal;"));
                                }
                                else
                                {
                                    drLink["IsBold"] = "1";
                                    fontStyle.Append(string.Format("font-weight:{0};", property["PropertyValue"].ToString()));
                                }
                            }
                            else if (property["property"].ToString() == "URL")
                            {
                                drLink["URL"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "WinOpenModel")
                            {
                                drLink["TARGET"] = property["PropertyValue"] == null ? "_blank" : property["PropertyValue"].ToString();
                            }
                        }
                        drLink["FontStyle"] = fontStyle.ToString();
                        drLink["IsItalic"] = "0";//斜体暂不处理
                        dtLink.Rows.Add(drLink);
                        #endregion
                    }
                    else if (control["CCForm_Shape"].ToString() == "Image")
                    {
                        #region 装饰图片  Image
                        DataRow drImg = dtImg.NewRow();

                        drImg["MYPK"] = control["CCForm_MyPK"];
                        drImg["FK_MAPDATA"] = this.FK_MapData;
                        drImg["IMGAPPTYPE"] = "0";
                        drImg["ISEDIT"] = "1";
                        drImg["NAME"] = drImg["MYPK"];
                        drImg["ENPK"] = drImg["MYPK"];
                        //坐标
                        JsonData vector = control["style"]["gradientBounds"];
                        drImg["X"] = vector[0].ToJson();
                        drImg["Y"] = vector[1].ToJson();
                        //图片高、宽
                        decimal minX = decimal.Parse(vector[0].ToJson());
                        decimal minY = decimal.Parse(vector[1].ToJson());
                        decimal maxX = decimal.Parse(vector[2].ToJson());
                        decimal maxY = decimal.Parse(vector[3].ToJson());
                        decimal imgWidth = maxX - minX;
                        decimal imgHeight = maxY - minY;
                        
                        drImg["W"] = imgWidth.ToString("0.00");
                        drImg["H"] = imgHeight.ToString("0.00");
                        //属性集合
                        JsonData properties = control["properties"];
                        StringBuilder fontStyle = new StringBuilder();
                        for (int iProperty = 0; iProperty < properties.Count; iProperty++)
                        {
                            JsonData property = properties[iProperty];
                            if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                                continue;

                            if (property["property"].ToString() == "LinkURL")
                            {
                                //图片连接到
                                drImg["LINKURL"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "WinOpenModel")
                            {
                                //打开窗口方式
                                drImg["LINKTARGET"] = property["PropertyValue"] == null ? "_blank" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "ImgAppType")
                            {
                                //应用类型：0本地图片，1指定路径
                                drImg["SRCTYPE"] = property["PropertyValue"] == null ? "0" : property["PropertyValue"].ToString();
                            }
                            else if (property["property"].ToString() == "ImgPath")
                            {
                                //指定图片路径
                                drImg["IMGURL"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                            }
                        }

                        //ImageFrame 本地图片路径
                        JsonData primitives = control["primitives"];
                        foreach (JsonData primitive in primitives)
                        {
                            if (primitive["oType"] == null)
                                continue;
                            if (primitive["oType"].ToJson() == "ImageFrame")
                            {
                                drImg["IMGPATH"] = primitive == null ? "" : primitive["url"].ToString();
                            }
                        }

                        dtImg.Rows.Add(drImg);
                        #endregion Image
                    }
                }
            }
            //TempSaveFrm(form_DS);
            return form_DS;
        }

        private void TempSaveFrm(DataSet ds)
        {
            string str = "";
            foreach (DataTable dt in ds.Tables)
            {
                try
                {
                    str += SaveDT(dt);
                    if (dt.TableName.ToUpper() == "WF_NODE" && dt.Columns.Count > 0 && dt.Rows.Count == 1)
                    {
                        /* 更新审核组件状态. */
                        // string nodeid = fk_mapdata.Replace("ND", "");
                        // BP.DA.DBAccess.RunSQL("UPDATE WF_Node SET FWCSta=1 WHERE FWCSta =0 AND NodeID=" + nodeid);
                    }
                }
                catch (Exception ex)
                {
                    str += "@保存" + dt.TableName + "失败:" + ex.Message;
                }
            }
        }

        public string SaveDT(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return "";

            string tableName = dt.TableName.Replace("CopyOf", "").ToUpper();
            //MaxTop,MaxLeft,MaxRight,MaxEnd合并到AtPara
            if (tableName == "SYS_MAPDATA" && dt.Columns.Contains("MaxLeft") && dt.Columns.Contains("MaxTop"))
                return "";

            if (dt.Columns.Count == 0)
                return null;

            TableSQL sqls = getTableSql(tableName, dt.Columns);
            string updataSQL = sqls.UPDATED;
            string pk = sqls.PK;
            string insertSQL = sqls.INSERTED;

            #region save data.
            foreach (DataRow dr in dt.Rows)
            {
                BP.DA.Paras ps = new BP.DA.Paras();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName == pk)
                        continue;

                    if (tableName == "SYS_MAPATTR" && dc.ColumnName.Trim() == "UIBINDKEY")
                        continue;

                    if (updataSQL.Contains(BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim()))
                        ps.Add(dc.ColumnName.Trim(), dr[dc.ColumnName.Trim()]);
                }

                ps.Add(pk, dr[pk]);
                ps.SQL = updataSQL;
                try
                {
                    if (BP.DA.DBAccess.RunSQL(ps) == 0)
                    {
                        ps.Clear();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (tableName == "SYS_MAPATTR" && dc.ColumnName == "UIBINDKEY")
                                continue;

                            if (updataSQL.Contains(BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim()))
                                ps.Add(dc.ColumnName.Trim(), dr[dc.ColumnName.Trim()]);
                        }
                        ps.SQL = insertSQL;
                        BP.DA.DBAccess.RunSQL(ps);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    string pastrs = "";
                    foreach (Para p in ps)
                    {
                        pastrs += "\t\n@" + p.ParaName + "=" + p.val;
                    }
                    throw new Exception("@执行sql=" + ps.SQL + "失败." + ex.Message + "\t\n@paras=" + pastrs);
                }
            }
            #endregion
            return null;
        }

        Dictionary<string, TableSQL> dicTableSql = new Dictionary<string, TableSQL>();
        TableSQL getTableSql(string tableName, DataColumnCollection columns = null)
        {
            TableSQL tableSql = null;
            if (dicTableSql.ContainsKey(tableName))
            {
                tableSql = dicTableSql[tableName];
            }
            else
            {
                if (columns != null && columns.Count > 0)
                {
                    string igF = "@RowIndex@RowState@";

                    #region gener sql.
                    //生成sqlUpdate
                    string sqlUpdate = "UPDATE " + tableName + " SET ";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName + "@"))
                            continue;

                        switch (dc.ColumnName)
                        {
                            case "MYPK":
                            case "OID":
                            case "NO":
                                continue;
                            default:
                                break;
                        }

                        if (tableName == "SYS_MAPATTR" && dc.ColumnName == "UIBINDKEY")
                            continue;
                        try
                        {
                            sqlUpdate += dc.ColumnName + "=" + BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim() + ",";
                        }
                        catch
                        {
                        }
                    }
                    sqlUpdate = sqlUpdate.Substring(0, sqlUpdate.Length - 1);
                    string pk = "";
                    if (columns.Contains("NODEID"))
                        pk = "NODEID";
                    if (columns.Contains("MYPK"))
                        pk = "MYPK";
                    if (columns.Contains("OID"))
                        pk = "OID";
                    if (columns.Contains("NO"))
                        pk = "NO";

                    sqlUpdate += " WHERE " + pk + "=" + BP.Sys.SystemConfig.AppCenterDBVarStr + pk;

                    //生成sqlInsert
                    string sqlInsert = "INSERT INTO " + tableName + " ( ";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName.Trim() + "@"))
                            continue;

                        if (tableName == "SYS_MAPATTR" && dc.ColumnName.Trim() == "UIBINDKEY")
                            continue;

                        sqlInsert += dc.ColumnName.Trim() + ",";
                    }
                    sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                    sqlInsert += ") VALUES (";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName + "@"))
                            continue;

                        if (tableName == "SYS_MAPATTR" && dc.ColumnName.Trim() == "UIBINDKEY")
                            continue;

                        sqlInsert += BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim() + ",";
                    }
                    sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                    sqlInsert += ")";
                    #endregion gener sql.

                    tableSql = new TableSQL();
                    tableSql.UPDATED = sqlUpdate;
                    tableSql.INSERTED = sqlInsert;
                    tableSql.PK = pk;

                    dicTableSql.Add(tableName, tableSql);
                }
            }

            return tableSql;
        }

        /// <summary>
        /// 数据集表结构集合
        /// </summary>
        /// <returns></returns>
        private DataSet Form_InitDataSource()
        {
            DataTable dtMapData = new DataTable();
            dtMapData.TableName = EEleTableNames.Sys_MapData;
            dtMapData.Columns.Add(new DataColumn("NO", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("NAME", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("FrmW", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("FrmH", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxLeft", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxRight", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxTop", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxEnd", typeof(double)));

            #region line
            DataTable dtLine = new DataTable();
            dtLine.TableName = EEleTableNames.Sys_FrmLine;
            dtLine.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLine.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            dtLine.Columns.Add(new DataColumn("X", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y", typeof(double)));

            dtLine.Columns.Add(new DataColumn("X1", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y1", typeof(double)));

            dtLine.Columns.Add(new DataColumn("X2", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y2", typeof(double)));

            dtLine.Columns.Add(new DataColumn("BorderWidth", typeof(string)));
            dtLine.Columns.Add(new DataColumn("BorderColor", typeof(string)));
            // lineDT.Columns.Add(new DataColumn("BorderStyle", typeof(string)));
            #endregion line

            #region btn
            DataTable dtBtn = new DataTable();
            dtBtn.TableName = EEleTableNames.Sys_FrmBtn;
            dtBtn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("X", typeof(double)));
            dtBtn.Columns.Add(new DataColumn("Y", typeof(double)));
            dtBtn.Columns.Add(new DataColumn("ISVIEW", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("ISENABLE", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("EVENTTYPE", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("EVENTCONTEXT", typeof(string)));
            #endregion line

            #region label
            DataTable dtLabel = new DataTable();
            dtLabel.TableName = EEleTableNames.Sys_FrmLab;
            dtLabel.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("X", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("TEXT", typeof(string)));

            dtLabel.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion label

            #region Link
            DataTable dtLikn = new DataTable();
            dtLikn.TableName = EEleTableNames.Sys_FrmLink;
            dtLikn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("X", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("TARGET", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("URL", typeof(string)));

            dtLikn.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));

            dtLikn.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLikn.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion Link

            #region img  ImgSeal
            DataTable dtImg = new DataTable();
            dtImg.TableName = EEleTableNames.Sys_FrmImg;
            dtImg.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtImg.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtImg.Columns.Add(new DataColumn("X", typeof(double)));
            dtImg.Columns.Add(new DataColumn("Y", typeof(double)));
            dtImg.Columns.Add(new DataColumn("W", typeof(double)));
            dtImg.Columns.Add(new DataColumn("H", typeof(double)));

            dtImg.Columns.Add(new DataColumn("IMGURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("IMGPATH", typeof(string))); //应用类型 0=图片，1签章..

            dtImg.Columns.Add(new DataColumn("LINKURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("LINKTARGET", typeof(string)));
            dtImg.Columns.Add(new DataColumn("SRCTYPE", typeof(int))); //图片来源类型.
            dtImg.Columns.Add(new DataColumn("IMGAPPTYPE", typeof(int))); //应用类型 0=图片，1签章..
            dtImg.Columns.Add(new DataColumn("Tag0", typeof(string)));
            dtImg.Columns.Add(new DataColumn("ISEDIT", typeof(int)));
            dtImg.Columns.Add(new DataColumn("NAME", typeof(string)));//中文名
            dtImg.Columns.Add(new DataColumn("ENPK", typeof(string)));//英文名
            #endregion img

            #region eleDT
            DataTable dtEle = new DataTable();
            dtEle.TableName = EEleTableNames.Sys_FrmEle;
            dtEle.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtEle.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            //eleDT.Columns.Add(new DataColumn("EleType", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleID", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleName", typeof(string)));

            dtEle.Columns.Add(new DataColumn("X", typeof(double)));
            dtEle.Columns.Add(new DataColumn("Y", typeof(double)));
            dtEle.Columns.Add(new DataColumn("W", typeof(double)));
            dtEle.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion eleDT

            #region Sys_FrmImgAth
            DataTable imgAthDT = new DataTable();
            imgAthDT.TableName = EEleTableNames.Sys_FrmImgAth;
            imgAthDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("CTRLID", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("ISEDIT", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("X", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("Y", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("W", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion Sys_FrmImgAth

            #region mapAttrDT
            DataTable mapAttrDT = new DataTable();
            mapAttrDT.TableName = EEleTableNames.Sys_MapAttr;
            mapAttrDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UICONTRALTYPE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MYDATATYPE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("LGTYPE", typeof(string)));

            mapAttrDT.Columns.Add(new DataColumn("UIWIDTH", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("UIHEIGHT", typeof(double)));

            mapAttrDT.Columns.Add(new DataColumn("UIBINDKEY", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKey", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKeyText", typeof(string)));
            //   mapAttrDT.Columns.Add(new DataColumn("UIVISIBLE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("X", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion mapAttrDT

            #region frmRBDT
            DataTable dtRdb = new DataTable();
            dtRdb.TableName = EEleTableNames.Sys_FrmRB;
            dtRdb.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("ENUMKEY", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("INTKEY", typeof(int)));
            dtRdb.Columns.Add(new DataColumn("LAB", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("X", typeof(double)));
            dtRdb.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion frmRBDT

            #region Dtl
            DataTable dtlDT = new DataTable();

            dtlDT.TableName = EEleTableNames.Sys_MapDtl;
            dtlDT.Columns.Add(new DataColumn("NO", typeof(string)));
            dtlDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            dtlDT.Columns.Add(new DataColumn("X", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("Y", typeof(double)));

            dtlDT.Columns.Add(new DataColumn("H", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("W", typeof(double)));
            #endregion Dtl

            // BPWorkCheck
            DataTable dtWorkCheck = new DataTable();
            dtWorkCheck.TableName = EEleTableNames.WF_Node;
            dtWorkCheck.Columns.Add(new DataColumn("NodeID", typeof(string)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_W", typeof(double)));

            //子流程属性.
            dtWorkCheck.Columns.Add(new DataColumn("SF_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_W", typeof(double)));


            #region m2mDT
            DataTable m2mDT = new DataTable();
            m2mDT.TableName = EEleTableNames.Sys_MapM2M;
            m2mDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            m2mDT.Columns.Add(new DataColumn("X", typeof(double)));
            m2mDT.Columns.Add(new DataColumn("Y", typeof(double)));

            m2mDT.Columns.Add(new DataColumn("H", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("W", typeof(string)));
            #endregion m2mDT

            #region athDT
            DataTable athDT = new DataTable();
            athDT.TableName = EEleTableNames.Sys_FrmAttachment;
            athDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            athDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            athDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));

            //athDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            //athDT.Columns.Add(new DataColumn("EXTS", typeof(string)));
            //athDT.Columns.Add(new DataColumn("SAVETO", typeof(string)));
            athDT.Columns.Add(new DataColumn("UPLOADTYPE", typeof(int)));

            athDT.Columns.Add(new DataColumn("X", typeof(double)));
            athDT.Columns.Add(new DataColumn("Y", typeof(double)));
            athDT.Columns.Add(new DataColumn("W", typeof(double)));
            athDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion athDT

            DataSet dsLatest = new DataSet();

            dsLatest.Tables.Add(dtWorkCheck);
            dsLatest.Tables.Add(dtLabel);
            dsLatest.Tables.Add(dtLikn);
            dsLatest.Tables.Add(dtImg);
            dsLatest.Tables.Add(dtEle);
            dsLatest.Tables.Add(dtBtn);
            dsLatest.Tables.Add(imgAthDT);
            dsLatest.Tables.Add(mapAttrDT);
            dsLatest.Tables.Add(dtRdb);
            dsLatest.Tables.Add(dtLine);
            dsLatest.Tables.Add(dtlDT);
            dsLatest.Tables.Add(athDT);
            dsLatest.Tables.Add(dtMapData);
            dsLatest.Tables.Add(m2mDT);

            return dsLatest;
        }
        #endregion End

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class EEleTableNames
    {
        public const string
            Sys_FrmLine = "Sys_FrmLine",
            Sys_FrmBtn = "Sys_FrmBtn",
            Sys_FrmLab = "Sys_FrmLab",
            Sys_FrmLink = "Sys_FrmLink",
            Sys_FrmImg = "Sys_FrmImg",
            Sys_FrmEle = "Sys_FrmEle",
            Sys_FrmImgAth = "Sys_FrmImgAth",
            Sys_FrmRB = "Sys_FrmRB",
            Sys_FrmAttachment = "Sys_FrmAttachment",

            Sys_MapData = "Sys_MapData",
            Sys_MapAttr = "Sys_MapAttr",
            Sys_MapDtl = "Sys_MapDtl",
            Sys_MapM2M = "Sys_MapM2M",
            WF_Node = "WF_Node";//BPWorkCheck
    }

    class TableSQL
    {
        public string PK;
        public string INSERTED;
        public string UPDATED;
    }
}