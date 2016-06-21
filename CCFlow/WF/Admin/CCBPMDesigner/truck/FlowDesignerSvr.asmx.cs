using System;
using System.Data;
using System.IO;
using System.Web.Services;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.WF;
using BP.WF.Template;
using Silverlight.DataSetConnector;
using System.Text;
using System.Xml;
using System.Linq;

namespace CCFlow.WF.Admin.CCBPMDesigner
{
    /// <summary>
    /// FlowDesignerSvr 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class FlowDesignerSvr : System.Web.Services.WebService
    {
        OSModel model = BP.Sys.OSModel.OneOne;
       

        StringBuilder sbJson = new StringBuilder();
        [WebMethod]
        public string GetFlowTree()
        {
            
            BP.WF.Glo.CheckTreeRoot();

            string sql = @"
SELECT No ,ParentNo,Name, Idx, 1 IsParent FROM WF_FlowSort
union 
SELECT No, FK_FlowSort as ParentNo,Name,Idx,0 IsParent FROM WF_Flow 
";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            sbJson.Clear();

            string sTmp = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                GetTreeJsonByTable(dt, "0");
            }
            sTmp += sbJson.ToString();

            return sTmp;
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        public string GetTreeJsonByTable(DataTable tabel, object pId, string rela = "ParentNo", string idCol = "No", string txtCol = "Name", string IsParent = "IsParent", string sChecked = "")
        {
            string treeJson = string.Empty;

            if (tabel.Rows.Count > 0)
            {
                sbJson.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }


                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow row = rows[i];


                        string jNo = row[idCol] as string;
                        string jText = row[txtCol] as string;
                        if (jText.Length > 25)
                            jText = jText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + jNo + ")'/>";

                        string jIsParent = row[IsParent].ToString();
                        string jState = "1".Equals(jIsParent) ? "open" : "closed";
                        jState = "open".Equals(jState) && i == 0 ? "open" : "closed";

                        DataRow[] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
                        string tmp = "{\"id\":\"" + jNo + "\",\"text\":\"" + jText;

                        if ("0".Equals(pId.ToString()))
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\",\"IsRoot\":\"1\"}";
                        }
                        else
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\"}";
                        }

                        if (rowChild.Length > 0)
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower()
                                + ",\"state\":\"" + jState + "\"";
                        }
                        else
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
                        }

                        sbJson.Append(tmp);
                        if (rowChild.Length > 0)
                        {
                            sbJson.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, jNo, rela, idCol, txtCol, IsParent, sChecked);
                        }

                        sbJson.Append("},");
                    }
                    sbJson = sbJson.Remove(sbJson.Length - 1, 1);
                }
                sbJson.Append("]");
                treeJson = sbJson.ToString();
            }
            return treeJson;
        }


        public DataSet TurnXmlDataSet2SLDataSet(DataSet ds)
        {
            DataSet myds = new DataSet();
            foreach (DataTable dtXml in ds.Tables)
            {
                DataTable dt = new DataTable(dtXml.TableName);
                foreach (DataColumn dc in dtXml.Columns)
                {
                    DataColumn mydc = new DataColumn(dc.ColumnName, typeof(string));
                    dt.Columns.Add(mydc);
                }
                foreach (DataRow dr in dtXml.Rows)
                {
                    DataRow drNew = dt.NewRow();
                    foreach (DataColumn dc in dtXml.Columns)
                    {
                        drNew[dc.ColumnName] = dr[dc.ColumnName];
                    }
                    dt.Rows.Add(drNew);
                }
                myds.Tables.Add(dt);
            }
            return myds;
        }

        /// <summary>
        /// 获得web.config配置信息.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetConfig(string key)
        {
            switch (key)
            {
                case "SendEmailPass":
                case "AppCenterDSN":
                case "FtpPass":
                    throw new Exception("@非法的访问");
                default:
                    break;
            }

            string tmp = BP.Sys.SystemConfig.AppSettings[key];
            return tmp;
        }

        /// <summary>
        /// 上传文件.
        /// </summary>
        /// <param name="FileByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [WebMethod]
        public string UploadFile_del(byte[] FileByte, String fileName)
        {
            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

            string filePath = path + "\\" + fileName;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            //这里使用绝对路径来索引
            FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            stream.Write(FileByte, 0, FileByte.Length);
            stream.Close();

            DataSet ds = new DataSet();
            ds.ReadXml(filePath);

            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }
        [WebMethod(EnableSession = true)]
        public int RunSQL(string sql)
        {
            return BP.DA.DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 运行sqls
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        [WebMethod]
        public int RunSQLs(string sqls)
        {
            if (string.IsNullOrEmpty(sqls))
                return 0;

            int i = 0;
            string[] strs = sqls.Split('@');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                i += BP.DA.DBAccess.RunSQL(str);
            }
            return i;
        }

        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTable(string sql)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(BP.DA.DBAccess.RunSQLReturnTable(sql));
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }
        /// <summary>
        /// 运行sql返回String.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnString(string sql)
        {
            return BP.DA.DBAccess.RunSQLReturnString(sql);
        }
        /// <summary>
        /// 运行sql返回String.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public int RunSQLReturnValInt(string sql)
        {
            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 运行sql返回float.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public float RunSQLReturnValFloat(string sql)
        {
            return BP.DA.DBAccess.RunSQLReturnValFloat(sql);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTableS(string sqls)
        {
            string xml = string.Empty;

            //string file = System.Web.HttpContext.Current.Server.MapPath("./data/002.xml");
            //using (System.IO.StreamReader sr = new System.IO.StreamReader(file))
            //{
            //    file = sr.ReadToEnd();
            //}

            //return file;

            DataSet ds = RunSQLReturnDataSet(sqls);

            xml = Connector.ToXml(ds);
            return xml;
        }

        //将DataSet转换为xml对象字符串
        public static string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法写入文件.
                xmlDS.WriteXml(writer, XmlWriteMode.WriteSchema);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        public DataSet RunSQLReturnDataSet(string sqls)
        {
            string[] strs = sqls.Split('@');
            DataSet ds = new DataSet();
            int i = 0;
            foreach (string sql in strs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "DT" + i;
                ds.Tables.Add(dt);
                i++;
            }
            return ds;
        }

        /// <summary>
        /// 将中文转化成拼音.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [WebMethod]
        public string ParseStringToPinyin(string name)
        {
            try
            {
                string s = BP.DA.DataType.ParseStringToPinyin(name);
                if (s.Length > 15)
                    s = BP.DA.DataType.ParseStringToPinyinWordFirst(name);
                return s;
            }
            catch
            {
                return null;
            }
        }
        private string DealPK(string pk, string fromMapdata, string toMapdata)
        {
            if (pk.Contains("*" + fromMapdata))
                return pk.Replace("*" + toMapdata, "*" + toMapdata);
            else
                return pk + "*" + toMapdata;
        }
        public void LetAdminLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp);
        }

        /// <summary>
        /// 获得所有图标文件路径.
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetNodeIconFile()
        {
            // CCBPMDesigner/Admin/WF
            string path = Server.MapPath("../../../");
            path += "WF\\Admin\\ClientBin\\NodeIcon";
            string[] files = System.IO.Directory.GetFiles(path, "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i];
                item = item.Substring(path.Length, item.Length - path.Length);
                item = item.Substring(item.LastIndexOf('\\') + 1, item.IndexOf('.') - 1);
                files[i] = item;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = string.Empty, data = files });
        }
        /// <summary>
        /// 执行功能返回信息
        /// </summary>
        /// <param name="doType"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = false)]
        public string DoType(string doType, string v1, string v2, string v3, string v4, string v5)
        {
            try
            {
                switch (doType)
                {
                    case "FrmTreeUp": // 表单树
                        SysFormTree sft = new SysFormTree();
                        sft.DoUp();
                        return null;
                    case "FrmTreeDown": // 表单树
                        SysFormTree sft1 = new SysFormTree();
                        sft1.DoDown();
                        return null;
                    case "FrmUp":
                        MapData md1 = new MapData(v1);
                        md1.DoOrderDown();
                        return null;
                    case "FrmDown":
                        MapData md = new MapData(v1);
                        md.DoOrderDown();
                        return null;
                    case "AdminLogin":
                        try
                        {
                            //if (BP.Sys.SystemConfig.IsDebug == true)
                            //    return null;

                            BP.Port.Emp emp = new BP.Port.Emp();
                            emp.No = v1;
                            emp.RetrieveFromDBSources();

                            if (emp.Pass == v2)
                            {
                                BP.Web.WebUser.SignInOfGener(emp);

                                return
                                    Newtonsoft.Json.JsonConvert.SerializeObject(
                                        new
                                            {
                                                success = true,
                                                msg = string.Empty,
                                                data = new { no = emp.No, name = emp.Name, sid = BP.Web.WebUser.SID }
                                            });
                            }

                            return Newtonsoft.Json.JsonConvert.SerializeObject(
                                        new { success = false, msg = "用户名或密码错误" });
                        }
                        catch (Exception ex)
                        {
                            return Newtonsoft.Json.JsonConvert.SerializeObject(
                                        new { success = false, msg = ex.Message });
                        }
                    case "DeleteFrmSort":
                        SysFormTree fs = new SysFormTree();
                        fs.No = v1;
                        fs.Delete();
                        SysFormTree ft = new SysFormTree();
                        ft.No = v1;
                        ft.Delete();
                        return null;
                    case "DeleteFrm": //删除表单.
                    case "DelFrm":
                        MapData md4 = new MapData();
                        md4.No = v1;
                        md4.Delete();
                        return null;
                    case "InitDesignerXml":
                        string path = BP.Sys.SystemConfig.PathOfData + "\\Xml\\Designer.xml";
                        DataSet ds = new DataSet();
                        ds.ReadXml(path);
                        ds = this.TurnXmlDataSet2SLDataSet(ds);
                        return Silverlight.DataSetConnector.Connector.ToXml(ds);
                    default:
                        throw new Exception("没有判断的，功能编号" + doType);
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError("执行错误，功能编号" + doType + " error:" + ex.Message);
                throw new Exception("执行错误，功能编号" + doType + " error:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <param name="fid">父流程编号</param>
        /// <returns></returns>
        [WebMethod(EnableSession = false)]
        public string GetFlowTrackJsonData(string fk_flow, string workid, string fid)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
            try
            {
                //获取流程信息
                var sql = "SELECT NO,Name,Paras,ChartType FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_FLOW";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID ID,Name,Icon,X,Y,NodePosType,RunModel,HisToNDs,TodolistModel FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_NODE";
                ds.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK,Name,X,Y FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LABNOTE";
                ds.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node,ToNode,DirType,IsCanBack,Dots FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_DIRECTION";
                ds.Tables.Add(dt);

                if (!string.IsNullOrWhiteSpace(workid))
                {
                    //获取工作轨迹信息
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom, NDTo,ActionType,ActionTypeText,Msg,RDT,EmpFrom,EmpFromT,EmpToT,EmpTo FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + (string.IsNullOrWhiteSpace(fid) || fid == "0" ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);

                    //判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                    if (dt.Rows.Count > 0)
                    {
                        if (Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.Return) || Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.UnSend))
                        {
                            if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(0);
                                dt.Rows.RemoveAt(0);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(0);
                            }
                        }
                    }

                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);

                    //获取预先计算的节点处理人，以及处理时间,added by liuxc,2016-4-15
                    sql = "SELECT wsa.FK_Node,wsa.FK_Emp,wsa.EmpName,wsa.TSpanDay,wsa.TSpanHour,wsa.ADT,wsa.SDT FROM WF_SelectAccper AS wsa WHERE wsa.WorkID = " + workid;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "POSSIBLE";
                    ds.Tables.Add(dt);

                    //获取节点处理人数据，及处理/查看信息
                    sql = "SELECT wgw.FK_Emp,wgw.FK_Node,wgw.FK_EmpText,wgw.RDT,wgw.IsRead,wgw.IsPass FROM WF_GenerWorkerlist AS wgw WHERE wgw.WorkID = " +
                          workid + (string.IsNullOrWhiteSpace(fid) || fid == "0" ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);
                }
                else
                {
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom, NDTo,ActionType,ActionTypeText,Msg,RDT,EmpFrom,EmpFromT,EmpToT,EmpTo FROM " + trackTable +
                          " WHERE WorkID=0 ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);
                }
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    dt = ds.Tables[i];
                    dt.TableName = dt.TableName.ToUpper();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.ToUpper();
                    }
                }

                var re = new { success = true, msg = string.Empty, ds = ds };
                json = Newtonsoft.Json.JsonConvert.SerializeObject(re);
            }
            catch (Exception ex)
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
            }
            return json;
        }


        /// <summary>
        /// 让admin 登陆
        /// </summary>
        /// <param name="lang">当前的语言</param>
        /// <returns>成功则为空，有异常时返回异常信息</returns>
        [WebMethod(EnableSession = true)]
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

        /// <summary>
        /// 执行功能操作.
        /// </summary>
        /// <param name="doWhat">执行的类型</param>
        /// <param name="para1">参数</param>
        /// <param name="isLogin"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string Do(string doWhat, string para1, bool isLogin)
        {
            // 如果admin账户登陆时有错误发生，则返回错误信息
            var result = LetAdminLogin("CH", isLogin);

            if (string.IsNullOrEmpty(result) == false)
                return result;

            switch (doWhat)
            {
                case "GenerFlowTemplete":
                    Flow temp = new BP.WF.Flow(para1);
                    return null;
                case "GetFlowSorts":    //获取所有流程类型
                    DataTable dtSorts = null;

                    try
                    {
                        dtSorts = BP.DA.DBAccess.RunSQLReturnTable("SELECT no,name FROM WF_FlowSort");
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }

                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = string.Empty, data = dtSorts.Rows.Cast<DataRow>().Select(dr => new { Key = dr["no"].ToString(), Value = dr["name"].ToString() }) });
                case "NewSameLevelFrmSort": //创建同级别的 表单树 目录.
                    SysFormTree frmSort = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSort = new SysFormTree(para[0]);
                        string sameNodeNo = frmSort.DoCreateSameLevelNode().No;
                        frmSort = new SysFormTree(sameNodeNo);
                        frmSort.Name = para[1];
                        frmSort.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewFormSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "NewSubLevelFrmSort": //创建子级别的 表单树 目录.
                    SysFormTree frmSortSub = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSortSub = new SysFormTree(para[0]);
                        string sameNodeNo = frmSortSub.DoCreateSubNode().No;
                        frmSortSub = new SysFormTree(sameNodeNo);
                        frmSortSub.Name = para[1];
                        frmSortSub.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewSubLevelFrmSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "NewSameLevelFlowSort":  //创建同级别的 流程树 目录.
                    FlowSort fs = null;
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0]);
                        string sameNodeNo = fs.DoCreateSameLevelNode().No;
                        fs = new FlowSort(sameNodeNo);
                        fs.Name = para[1];
                        fs.Update();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = fs.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSameLevelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "NewSubFlowSort": //创建子级别的 表单树 目录.
                    try
                    {
                        var para = para1.Split(',');
                        FlowSort fsSub = new FlowSort(para[0]);
                        string subNodeNo = fsSub.DoCreateSubNode().No;
                        FlowSort subFlowSort = new FlowSort(subNodeNo);
                        subFlowSort.Name = para[1];
                        subFlowSort.Update();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = subFlowSort.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSubFlowSort Branch has a error , para:\t" + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "EditFlowSort": //编辑表单树.
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0]);
                        fs.Name = para[1];
                        fs.Save();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = fs.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method EditFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "NewFlow": //创建新流程.
                    Flow fl = new Flow();
                    try
                    {
                        string[] ps = para1.Split(',');
                        if (ps.Length != 5)
                            throw new Exception("@创建流程参数错误");

                        string fk_floSort = ps[0]; //类别编号.
                        string flowName = ps[1]; // 流程名称.
                        DataStoreModel dataSaveModel = (DataStoreModel)int.Parse(ps[2]); //数据保存方式。
                        string pTable = ps[3]; // 物理表名。
                        string FlowMark = ps[4]; // 流程标记.

                        fl.DoNewFlow(fk_floSort, flowName, dataSaveModel, pTable, FlowMark);
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = new { no = fl.No, name = fl.Name } });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewFlow Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelFlow": //删除流程.
                    try
                    {
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = WorkflowDefintionManager.DeleteFlowTemplete(para1) });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelFlowSort":
                    try
                    {
                        FlowSort delfs = new FlowSort();
                        delfs.No = para1;
                        delfs.Delete();
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method DelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelNode":
                    try
                    {
                        if (!string.IsNullOrEmpty(para1))
                        {
                            BP.WF.Node delNode = new BP.WF.Node(int.Parse(para1));
                            delNode.Delete();
                        }
                        else
                        {
                            throw new Exception("@参数错误:" + para1);
                        }
                    }
                    catch (Exception ex)
                    {
                        return "err:" + ex.Message;
                    }
                    return null;
                case "GetSettings":
                    return SystemConfig.AppSettings[para1];
                case "SaveFlowFrm":  //保存独立表单.
                    Entity en = null;
                    try
                    {
                        AtPara ap = new AtPara(para1);
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
                case "ChangeNodeType":
                    var p = para1.Split(',');

                    try
                    {
                        if (p.Length != 3)
                            throw new Exception("@修改节点类型参数错误");

                        //var sql = "UPDATE WF_Node SET Icon='{0}' WHERE FK_Flow='{1}' AND NodeID='{2}'";
                        var sql = "UPDATE WF_Node SET RunModel={0} WHERE FK_Flow='{1}' AND NodeID={2}";
                        DBAccess.RunSQL(string.Format(sql, p[0], p[1], p[2]));
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "ChangeNodeIcon":
                    p = para1.Split(',');

                    try
                    {
                        if (p.Length != 3)
                            throw new Exception("@修改节点图标参数错误");

                        var sql = "UPDATE WF_Node SET Icon='{0}' WHERE FK_Flow='{1}' AND NodeID={2}";
                        DBAccess.RunSQL(string.Format(sql, p[0], p[1], p[2]));
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                default:
                    throw new Exception("@没有约定的执行标记:" + doWhat);
            }
        }

        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="isLogin"></param>
        /// <param name="param">fk_flow,nodeName,icon,x,y</param>
        /// <returns></returns>
        /// <returns>返回节点编号</returns>
        [WebMethod(EnableSession = true)]
        public int DoNewNode(bool isLogin, params string[] param)
        {
            LetAdminLogin("CH", isLogin);

            string fk_flow = param[0];
            if (string.IsNullOrEmpty(fk_flow))
                return 0;

            string nodeName = param[1];
            string icon = param[2];

            int x = (int)double.Parse(param[3]),
                y = (int)double.Parse(param[4]),
                HisRunModel = int.Parse(param[5]);

            Flow fl = new Flow(fk_flow);
            BP.WF.Node nf = fl.DoNewNode(x, y);
            nf.ICON = icon;
            nf.Name = nodeName;
            nf.HisRunModel = (RunModel)HisRunModel;
            nf.Update();
            return nf.NodeID;
        }

        /// <summary>
        /// 执行导入
        /// </summary>
        /// <param name="fk_flowSort">流程类别编号</param>
        /// <param name="templatePath">模板文件路径</param>
        /// <param name="importModel">导入的类型</param>
        /// <param name="specialFlowNo">指定的流程编号</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string FlowTemplateLoad(string fk_flowSort, byte[] FileByte, int importModel, int specialFlowNo)
        {
            LetAdminLogin("CH", true);

            string templatePath = BP.Sys.SystemConfig.PathOfTemp + "\\Temp.xml";

            #region 保存上传的文件。
            try
            {
                //文件存放路径
                //如果文件已经存在则删除
                if (File.Exists(templatePath))
                    File.Delete(templatePath);
                //创建文件流实例，用于写入文件
                FileStream stream = new FileStream(templatePath, FileMode.CreateNew);
                //写入文件
                stream.Write(FileByte, 0, FileByte.Length);
                stream.Close();
            }
            catch (Exception exception)
            {
                return "Error: 保存流程模版文件出现错误，请修改服务器对该目录(" + templatePath + ")的读写权限 :\n" + exception.Message;
            }
            #endregion

            try
            {
                ImpFlowTempleteModel model = (ImpFlowTempleteModel)importModel;
                Flow flow = null;
                if (model == ImpFlowTempleteModel.AsSpecFlowNo)
                {
                    if (specialFlowNo <= 0)
                        return "指定流程编号错误";

                    flow = Flow.DoLoadFlowTemplate(fk_flowSort, templatePath, model, specialFlowNo);
                }
                else
                {
                    flow = Flow.DoLoadFlowTemplate(fk_flowSort, templatePath, model);
                }

                //执行一下修复view.
                Flow.RepareV_FlowData_View();
                return string.Format("TRUE,{0},{1},{2}", fk_flowSort, flow.No, flow.Name);
            }
            catch (Exception ex)
            {
                return "Error: 导入流程模版错误:" + ex.Message;
            }
        }

        /// <summary>
        /// 保存流程, 用 ~ 隔开。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodes">节点信息，格式为:@NodeID=xxxx@X=xxx@Y=xxx@Name=xxxx@RunModel=1</param>
        /// <param name="dirs">方向信息，格式为: @Node=xxxx@ToNode=xxx@X=xxx@Y=xxx@Name=xxxx   </param>
        /// <param name="labes">标签信息，格式为:@MyPK=xxxxx@Label=xxx@X=xxx@Y=xxxx</param>
        [WebMethod(EnableSession = true)]
        public string FlowSave(string fk_flow, string nodes, string dirs, string labes)
        {
            LetAdminLogin("CH", true);
            string result =  WorkflowDefintionManager.SaveFlow(fk_flow, nodes, dirs, labes);
            if( string.IsNullOrEmpty(result))
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = "" });

            }else{
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = result });
            }
        }

        /// <summary>
        /// 将datatable表名以及列名转换为大写形式，适应oracle数据库查询的列均为大写的情况
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable ColumnName2UpperCase(DataTable dt)
        {
            if (!string.IsNullOrEmpty(dt.TableName))
                dt.TableName = dt.TableName.ToUpper();

            foreach(DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.ToUpper();
            }

            return dt;
        }
    }
}
