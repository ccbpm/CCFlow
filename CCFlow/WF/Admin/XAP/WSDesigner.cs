using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Services;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.Sys;
using BP.Web;
using BP.WF;
using BP.WF.Template;
using FtpSupport;
using Silverlight.DataSetConnector;
using System.Text;

namespace CCFlow.WF.Admin.XAP
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [System.Web.Script.Services.ScriptService]
    public class WSDesigner : System.Web.Services.WebService
    {
        /// <summary>
        /// 流程设计器树控件数据源
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetFlowDesignerTree(params bool[] paras)
        {

            //检查树结构是否符合要求.
            BP.WF.Glo.CheckTreeRoot(); 
              
            string sql = "";
            DataSet myds = new DataSet();

            //加入流程类别.
            sql = "SELECT No,Name,ParentNo FROM WF_FlowSort ORDER BY No,Idx";
            DataTable dtFlowSort = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtFlowSort.TableName = "WF_FlowSort";
            myds.Tables.Add(dtFlowSort);

            //加入流程.
            sql = "SELECT No,Name,FK_FlowSort FROM WF_Flow ORDER BY No,Idx";
            DataTable dtFlow = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtFlow.TableName = "WF_Flow";
            myds.Tables.Add(dtFlow);


            //加入表单树.
            sql = "SELECT No,Name,ParentNo FROM Sys_FormTree ORDER BY Idx ASC,No ASC";
            DataTable dtFormTree = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtFormTree.TableName = "Sys_FormTree";
            myds.Tables.Add(dtFormTree);

            //加入表单.
            sql = "SELECT a.No, a.Name, a.FK_FormTree FROM Sys_MapData a, Sys_FormTree b WHERE a.AppType=" + (int)AppType.Application + " AND a.FK_FormTree=b.No ORDER BY a.Idx ASC , a.No ASC";
            DataTable dtForm = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtForm.TableName = "Sys_MapData";
            myds.Tables.Add(dtForm);

            // 装载组织结构.
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                var ws = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = ws.GetDepts();
                dt.TableName = "Port_Dept";
                myds.Tables.Add(dt);

                DataTable dtEmp = ws.GetEmps();
                dtEmp.TableName = "Port_Emp";
                myds.Tables.Add(dtEmp);
            }
            else
            {
                //加入部门.
                sql = "SELECT No,Name,ParentNo FROM Port_Dept ORDER BY No,Idx";
                DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dtDept.TableName = "Port_Dept";
                myds.Tables.Add(dtDept);

                //加入人员.
                sql = "SELECT No,Name,FK_Dept FROM Port_Emp ORDER BY No,Idx";
                DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dtEmp.TableName = "Port_Emp";
                myds.Tables.Add(dtEmp);
            }
            return Connector.ToXml(myds);
        }

        StringBuilder sbJson = new StringBuilder();

        [WebMethod]
        public string GetFlowTree()
        {
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

        [WebMethod]
        public string GetFormTree()
        {
            string sql = @"SELECT No ,ParentNo,Name, Idx, 1 IsParent FROM Sys_FormTree
union 
SELECT No, FK_FrmSort as ParentNo,Name,Idx,0 IsParent FROM Sys_MapData   where AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree) 
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

                        DataRow [] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
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
                           tmp+= ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower() 
                               + ",\"state\":\"" + jState + "\"";
                        }
                        else
                        {
                            tmp+= ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
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
    

        #region 与共享模版相关的方法。

        /// <summary>
        /// 获得共享模版的目录名称
        /// </summary>
        /// <returns>用@符合分开的文件名.</returns>
        [WebMethod(EnableSession = false)]
        public string GetDirs(string dir,bool FileOrDirecotry )
        {
            string ip = "online.ccflow.org";
            string userNo = "ccflowlover";
            string pass = "ccflowlover";

            List<string> listDir = new List<string>();
            string dirs = "";
            try
            {
               FtpConnection conn = new FtpConnection(ip, userNo, pass);
               List<Win32FindData> sts = getFiles(conn,dir);

                foreach (Win32FindData item in sts)
                {
                    if (FileOrDirecotry)
                    {
                        if (item.FileAttributes == FileAttributes.Directory )
                            listDir.Add(item.FileName);
                    }
                    else if (item.FileAttributes == FileAttributes.Normal)
                    {
                        string tmp = item.FileName;
                        tmp = tmp.Substring(0, tmp.LastIndexOf('.'));

                        if (!listDir.Contains(tmp))
                            listDir.Add(tmp);
                    }
                }

                foreach (string item in listDir)
                {
                    dirs += item + "@";
                }
                if (!string.IsNullOrEmpty(dirs))
                    dirs = dirs.Substring(0, dirs.LastIndexOf('@'));

            }
            catch (Exception e)
            {
                BP.DA.Log.DebugWriteError(e.ToString());
            }
            return dirs;
        }

        public class FtpFile
        {
            public enum FileType
            {
                File, Directory
            }
            public FileType Type = FileType.Directory;
            public string Name;
            public string Ext;
            public string Path;
            public FtpFile Super = null;
            public List<FtpFile> Subs;
            /// <summary>
            /// true标识该级目录一下为资源文件，可预览下载，在下级文件中配置值
            /// </summary>
            public bool CanViewAndDown;


            public void SyncChildren()
            {
                //foreach (var item in this.Subs)
                //{
                //    item.Super = this;
                //}
            }
        }
        /// <summary>
        /// 获得共享模版的目录名称
        /// </summary>
        /// <returns>用@符合分开的文件名.</returns>
        [WebMethod(EnableSession = false)]
        public FtpFile GetDirectory()
        {
            string FlowTemplate = DoPort.FlowTemplate;
            string ip = "online.ccflow.org";
            string userNo = "ccflowlover";
            string pass = "ccflowlover";

            FtpFile Superfile = null;
            FtpSupport.FtpConnection conn = null;
            try
            {
                conn = new FtpSupport.FtpConnection(ip, userNo, pass);

                Superfile = new FtpFile() { Name = FlowTemplate, Path = FlowTemplate, Type = FtpFile.FileType.Directory };
                Superfile.Subs = new List<FtpFile>();

                List<FtpSupport.Win32FindData> sts = getFiles(conn, FlowTemplate);
                foreach (FtpSupport.Win32FindData item in sts)
                {

                    FtpFile file = new FtpFile() { Path = FlowTemplate + "\\" + item.FileName };
                    file.Super = new FtpFile() { Name = Superfile.Name, Path = Superfile.Path };

                    Superfile.Subs.Add(file);

                    if (item.FileAttributes == FileAttributes.Directory)
                    {
                        file.Name = item.FileName;
                        file.Type = FtpFile.FileType.Directory;
                    }
                    else 
                    {
                        file.Type = FtpFile.FileType.File;
                        string tmp = item.FileName;

                        file.Name = tmp.Substring(0, tmp.LastIndexOf('.'));
                        file.Ext = tmp.Substring(tmp.LastIndexOf('.') + 1);

                    }
                }

                Superfile.SyncChildren();
                foreach (FtpFile item in Superfile.Subs)
                {
                    if (item.Type == FtpFile.FileType.Directory)
                    { getSubFile(conn, item); }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                BP.DA.Log.DebugWriteError(e.ToString());
            }
           
            return Superfile;

        }

        void getSubFile(FtpSupport.FtpConnection conn, FtpFile Superfile)
        {
            Superfile.Subs = new List<FtpFile>();
            string path = Superfile.Path;

            List<FtpSupport.Win32FindData> sts = getFiles(conn, path);
            foreach (FtpSupport.Win32FindData item in sts)
            {

                FtpFile file = new FtpFile() { Name = item.FileName, Path = path + "\\" + item.FileName };
                file.Super = new FtpFile() { Name = Superfile.Name, Path = Superfile.Path };
             

                if (item.FileAttributes == FileAttributes.Directory)
                {
                    file.Type = FtpFile.FileType.Directory;
                    Superfile.Subs.Add(file);
                }
                else 
                {
                    file.Type = FtpFile.FileType.File;
                    string tmp = item.FileName;

                    file.Name = tmp.Substring(0, tmp.LastIndexOf('.'));
                    file.Ext = tmp.Substring(tmp.LastIndexOf('.') + 1);


                    if (file.Name.Contains("Flow"))
                    {
                        Superfile.CanViewAndDown = true;
                        Superfile.Type = FtpFile.FileType.File;
                    }
                    bool flag = false;
                    foreach (var f in Superfile.Subs)
                    {
                        if (f.Name.Equals(file.Name))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                        Superfile.Subs.Add(file);                    ;
                }
            }
                          
            Superfile.SyncChildren();

            foreach (FtpFile item in Superfile.Subs)
            {
                if (item.Type == FtpFile.FileType.Directory)
                { 
                    getSubFile(conn, item);
                }
            }
        }

        List<FtpSupport.Win32FindData> getFiles(FtpSupport.FtpConnection conn, string path)
        {
            List<FtpSupport.Win32FindData> sts = new List<Win32FindData>();
            try
            {
                string tmp = conn.GetCurrentDirectory();
                conn.SetCurrentDirectory("/");
                conn.SetCurrentDirectory(path); //设置当前目录.
                FtpSupport.Win32FindData[] f = conn.FindFiles();

                foreach (FtpSupport.Win32FindData item in f)
                {
                    if (".".Equals(item.FileName)
                        || "..".Equals(item.FileName)
                        || string.Empty.Equals(item.FileName))
                        continue;

                    sts.Add(item);
                }
            }
            catch (Exception e) 
            {
                throw new Exception("FTP服务器读取目录出错：" + e.Message, e);
            }
            return sts;
        }

        [WebMethod(EnableSession = true)]
        public byte[] FlowTemplateDown(string[] FlowFileName)
        {
            string path = FlowFileName[0]
               , fileName = FlowFileName[1]
               , fileType = FlowFileName[2]
               , cmd = FlowFileName[3];

            if (string.IsNullOrEmpty(path)
                || string.IsNullOrEmpty(fileName)
                || string.IsNullOrEmpty(fileType)
                || string.IsNullOrEmpty(cmd))
            {
                throw new Exception("FTP服务器文件读取参数出错!" );
            }

            string ip = "online.ccflow.org",
               userNo = "ccflowlover",
               pass = "ccflowlover";
            FtpConnection conn = new FtpConnection(ip, userNo, pass);

            byte[] bytes = null;
            try
            {
                bytes = new byte[] { };

                conn.SetCurrentDirectory(path); //设置当前目录.
                FtpStream fs = conn.OpenFile(fileName, GenericRights.Read);
                if (null != fs )
                {
                    System.IO.MemoryStream ms = new MemoryStream();
                    fs.CopyTo(ms);
                    bytes = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(bytes, 0, bytes.Length);
                }
            }
            catch(Exception e)
            {
                throw new Exception("FTP服务器文件读取出错：" + e.Message, e);
            }
            conn.Close();

            if (null != bytes && 0 < bytes.Length && fileType == "XML")
            {
                if (cmd.Equals("INSTALL"))
                {//在线安装
                    if (fileName.Equals("Flow.xml"))
                    {
                        // 流程安装
                        path = this.FlowTemplateUpload(bytes, fileName);
                        bytes = System.Text.Encoding.UTF8.GetBytes(path);
                    }
                    else
                    {
                        //表单安装
                        //this.UploadfileCCForm(bytes, fileName, "");
                    }
                }
                else if (cmd == "DOWN")
                {   // 保存到本机

                    //HttpContext.Current.Response.BinaryWrite(bytes);
                    //string xml = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    //HttpContext.Current.Response.Write(xml);

                    //fileName = HttpUtility.UrlEncode(fileName);
                    //HttpContext.Current.Response.Charset = "GB2312";
                    //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
                    //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");

                    //HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.End();
                    //HttpContext.Current.Response.Close();
                }
            }
            return bytes;
        }
             
        #endregion 与共享模版相关的方法。

        #region 公用方法
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
        public string UploadFile(byte[] FileByte, String fileName)
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
        public int RunSQL(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@非法的用户.");
            return BP.DA.DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 运行sqls
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        [WebMethod]
        public int RunSQLs(string sqls, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

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
        /// 保存ens
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        [WebMethod]
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
                    en.SetValByKey(key, ap.HisHT[key].ToString().Replace('#', '@'));
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
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTable(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

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
        public string RunSQLReturnString(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            return BP.DA.DBAccess.RunSQLReturnString(sql);
        }
        /// <summary>
        /// 运行sql返回String.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public int RunSQLReturnValInt(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 运行sql返回float.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public float RunSQLReturnValFloat(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            return BP.DA.DBAccess.RunSQLReturnValFloat(sql);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTableS(string sqls, string UserNo, string SID)
        {
            //验证用户未通过
            if (BP.WF.Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            DataSet ds = RunSQLReturnDataSet(sqls);
            return Connector.ToXml(ds);
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
        /// <summary>
        /// 获取自定义表
        /// </summary>
        /// <param name="ensName"></param>
        /// <returns></returns>
        [WebMethod]
        public string RequestSFTable(string ensName)
        {
            if (string.IsNullOrEmpty(ensName))
                throw new Exception("@EnsName值为null.");

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            if (ensName.Contains("."))
            {
                Entities ens = BP.En.ClassFactory.GetEns(ensName);
                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(ensName);

                ens.RetrieveAllFromDBSource();
                dt = ens.ToDataTableField();
                ds.Tables.Add(dt);
            }
            else
            {
                string sql = "SELECT No,Name FROM " + ensName;
                ds.Tables.Add(BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
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
        [WebMethod]
        public string SaveEnum(string enumKey, string enumLab, string cfg)
        {
            SysEnumMain sem = new SysEnumMain();
            sem.No = enumKey;
            if (sem.RetrieveFromDBSources() == 0)
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = WebUser.SysLang;
                sem.Insert();
            }
            else
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = WebUser.SysLang;
                sem.Update();
            }

            string[] strs = cfg.Split('@');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                string[] kvs = str.Split('=');
                SysEnum se = new SysEnum();
                se.EnumKey = enumKey;
                se.Lang = WebUser.SysLang;
                se.IntKey = int.Parse(kvs[0]);
                se.Lab = kvs[1];
                se.Insert();
            }
            return "save ok.";
        }
        #endregion

        [WebMethod(EnableSession = true)]
        public string[]  GetNodeIconFile()
        {
            // XAP/Admin/WF
            string path = Server.MapPath("../../../");
            path += "WF\\Admin\\ClientBin\\NodeIcon";
         
            string[] files = System.IO.Directory.GetFiles(path, "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i];
                item = item.Substring(path.Length, item.Length - path.Length);
                item = item.Substring(item.LastIndexOf('\\')+1,item.IndexOf('.')-1);
                files[i] = item;
            }

            return files;
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
                            BP.Port.Emp emp = new BP.Port.Emp();
                            emp.No = v1;
                            emp.RetrieveFromDBSources();

                            if (emp.CheckPass(v2) == true)
                            {
                                BP.Web.WebUser.SignInOfGener(emp);
                                return BP.WF.Dev2Interface.Port_GetSID(v1);
                            }
                            else
                            {
                                return "error:用户名密码错误.";
                            }
                        }
                        catch (Exception ex)
                        {
                            return "error:" + ex.Message;
                        }
                    case "DeleteFrmSort":
                        SysFormTree fs = new SysFormTree();
                        fs.No = v1;
                        fs.Delete();
                        SysFormTree ft = new SysFormTree();
                        ft.No = v1;
                        ft.Delete();
                        return null;
                    case "DeleteFrm":
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
        /// 根据workID获取工作列表
        /// FK_Node 节点ID
        /// rdt 记录日期，也是工作接受日期。
        /// sdt 应完成日期.
        /// FK_emp 操作员编号。
        /// EmpName 操作员名称.
        /// </summary>
        /// <param name="workid">workid</param>
        /// <returns></returns>
        [WebMethod(EnableSession = false)]
        public string GetDTOfWorkList(string fk_flow, string workid)
        {
            DataSet ds = GetWorkList(fk_flow, workid);
            return Connector.ToXml(ds);
        }
        public DataSet GetWorkList(string fk_flow, string workid)
        {
            try
            {
                string sql = "";
                string table = "ND" + int.Parse(fk_flow) + "Track";
                DataSet ds = new DataSet();
                sql = "SELECT NDFrom, NDTo,ActionType,Msg,RDT,EmpFrom,EmpFromT FROM " + table + " WHERE WorkID=" + workid + "  OR FID=" + workid + " ORDER BY NDFrom ASC,NDTo ASC";
                DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                mydt.TableName = "WF_Track";
                ds.Tables.Add(mydt);
                return ds;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError("GetWorkList发生了错误 paras:" + fk_flow + "\t" + workid + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <returns></returns>
        [WebMethod(EnableSession=false)]
        public string GetFlowTrackJsonData(string fk_flow, string workid)
        {
            var re = new {success = true, msg = string.Empty, datas = new DataSet()};
            DataTable dt = null;

            try
            {
                //获取流程信息
                var sql = "SELECT Paras,ChartType FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "flow";
                re.datas.Tables.Add(dt);

                //获取流程中的节点信息
                sql =
                    "SELECT NodeID,Name,Icon,X,Y,NodePosType,RunModel,HisToNDs,TodolistModel FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "nodes";
                re.datas.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK,Name,X,Y FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "labels";
                re.datas.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node,ToNode,DirType,IsCanBack,Dots FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "dirs";
                re.datas.Tables.Add(dt);

                if (!string.IsNullOrWhiteSpace(workid))
                {
                    //获取工作轨迹信息
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom, NDTo,ActionType,ActionTypeText,Msg,RDT,EmpFrom,EmpFromT,EmpToT FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + "  OR FID=" + workid + " ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "tracks";
                    re.datas.Tables.Add(dt);
                }
            }
            catch(Exception ex)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new {success = false, msg = ex.Message});
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(re);
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

        [WebMethod(EnableSession = true)]
        [Obsolete]
        public string GetFlowBySort(string sort)
        {
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT No,Name,FK_FlowSort FROM WF_Flow");
            ds.Tables.Add(dt);
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }

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
                case "NewSameLevelFrmSort":
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
                case "NewSubLevelFrmSort":
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
                case "NewSameLevelFlowSort":
                    FlowSort fs = null;
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0]);
                        string sameNodeNo = fs.DoCreateSameLevelNode().No;
                        fs = new FlowSort(sameNodeNo);
                        fs.Name = para[1];
                        fs.Update();
                        return fs.No;
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSameLevelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return null;
                    }
                case "NewSubFlowSort":
          
                    try
                    {
                        var para = para1.Split(',');
                        FlowSort fsSub = new FlowSort(para[0]);
                        string subNodeNo = fsSub.DoCreateSubNode().No;
                        FlowSort subFlowSort = new FlowSort(subNodeNo);
                        subFlowSort.Name = para[1];
                        subFlowSort.Update();
                        return subFlowSort.No;
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSubFlowSort Branch has a error , para:\t"  + ex.Message);
                        return null;
                    }
                case "EditFlowSort":
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0]);
                        fs.Name = para[1];
                        fs.Save();
                        return fs.No;
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method EditFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return null;
                    }
                case "NewFlow":
                    Flow fl = new Flow();
                    try
                    {
                        string[] ps = para1.Split(',');
                        if (ps.Length != 5)
                            throw new Exception("@创建流程参数错误");

                        string fk_floSort = ps[0];
                        string flowName = ps[1];
                        DataStoreModel dataSaveModel = (DataStoreModel)int.Parse(ps[2]);
                        string pTable = ps[3];

                        string FlowMark = ps[4];

                        fl.DoNewFlow(fk_floSort, flowName, dataSaveModel, pTable, FlowMark);
                        return fl.No + ";" + fl.Name;
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewFlow Branch has a error , para:\t" + para1 + ex.Message);
                        return ex.Message;
                    }
                case "DelFlow": //删除流程.
                    return WorkflowDefintionManager.DeleteFlowTemplete(para1);
                case "DelLable":
                    LabNote ln = new LabNote(para1);
                    try
                    {
                        ln.Delete();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method DelLable Branch has a error , para:\t" + para1 + ex.Message);
                    }
                    return null;
                case "DelFlowSort":
                    try
                    {
                        FlowSort delfs = new FlowSort(para1);
                        delfs.Delete();
                        //删除文件夹时（流程类别），同时删除下面的流程。  add by zqp 海南 201604
                        Flows fls = new Flows(para1);
                        if(fls.Count>0)//如果存在就删除
                            fls.Delete();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method DelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                    }
                    return null;
                case "NewNode":
                    try
                    {
                        BP.WF.Flow fl11 = new BP.WF.Flow(para1);
                        BP.WF.Node node = new BP.WF.Node();
                        node.FK_Flow = "";
                        node.X = 0;
                        node.Y = 0;
                        node.Insert();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewNode Branch has a error , para:\t" + para1 + ex.Message);
                    }

                    return null;
                case "DelNode":
                    try
                    {
                        if (!string.IsNullOrEmpty(para1))
                        {
                            BP.WF.Node delNode = new BP.WF.Node(int.Parse(para1));
                            delNode.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        return "err:" + ex.Message;

                     //   BP.DA.Log.DefaultLogWriteLineError("Do Method DelNode Branch has a error , para:\t" + para1 + ex.Message);
                    }
                    return null;
                case "NewLab":
                    LabNote lab = new LabNote();
                    try
                    {
                        lab.FK_Flow = para1;
                        lab.MyPK = BP.DA.DBAccess.GenerOID().ToString();
                        lab.Insert();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewLab Branch has a error , para:\t" + para1 + ex.Message);
                    }
                    return lab.MyPK;
                case "DelLab":
                    try
                    {
                        LabNote dellab = new LabNote();
                        dellab.MyPK = para1;
                        dellab.Delete();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method DelLab Branch has a error , para:\t" + para1 + ex.Message);
                    }
                    return null;
                case "GetSettings":
                    return SystemConfig.AppSettings[para1];
             
                case "SaveFlowFrm":
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
                default:
                    throw null;
            }
        }
        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="isLogin"></param>
        /// <param name="param">fk_flow,nodeName,icon,x,y,HisRunModel,ChartType</param>
        /// <returns></returns>
        /// <returns>返回节点编号</returns>
        [WebMethod(EnableSession = true)]
        public int DoNewNode(bool isLogin,  params string[] param)
        {
            return DoNewNode(isLogin, 0, param);
        }
        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="isLogin"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int DoNewNodeWithTemplate(bool isLogin, int fromNodeID, params string[] param)
        {
            return DoNewNode(isLogin, fromNodeID, param);
        }
        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="isLogin"></param>
        /// <param name="param">fk_flow,nodeName,icon,x,y,HisRunModel,ChartType</param>
        /// <returns></returns>
        /// <returns>返回节点编号</returns>
        public int DoNewNode(bool isLogin, int fromNodeID,params string[] param)
        {
            LetAdminLogin("CH", isLogin);

            string fk_flow = param[0];
            if (string.IsNullOrEmpty(fk_flow))
                return 0;

            string nodeName = param[1];
            string icon = param[2];

            int x= (int)double.Parse(param[3]),
                y = (int)double.Parse(param[4]), 
                HisRunModel=int.Parse(param[5]);
         
            Flow fl = new Flow(fk_flow);
            if (param.Length == 7)
            {
                int type = int.Parse(param[6]);
                fl.Retrieve();
                fl.ChartType = type;
                fl.Update();
            }

            try
            {
                BP.WF.Node nf = fl.DoNewNode(x, y);


                int nodeID = nf.NodeID;

                if (fromNodeID != 0)
                {
                    /* 从一个节点上copy回来的. */
                    Node nd = new Node(fromNodeID);

                    nf.Copy(nd);
                    nf.NodeID = nodeID;

                    NodeStations nss = new NodeStations();
                    nss.Retrieve(NodeStationAttr.FK_Node, fromNodeID);
                    foreach (NodeStation item in nss)
                    {
                        item.FK_Node = nf.NodeID;
                        item.Insert();
                    }

                    NodeDepts depts = new NodeDepts();
                    depts.Retrieve(NodeDeptAttr.FK_Node, fromNodeID);
                    foreach (NodeDept item in depts)
                    {
                        item.FK_Node = nf.NodeID;
                        item.Insert();
                    }

                    NodeEmps emps = new NodeEmps();
                    emps.Retrieve(NodeDeptAttr.FK_Node, fromNodeID);
                    foreach (NodeEmp item in emps)
                    {
                        item.FK_Node = nf.NodeID;
                        item.Insert();
                    }

                    //复制条件.
                    Conds conds = new Conds();
                    conds.Retrieve(CondAttr.FK_Node, fromNodeID);
                    foreach (Cond item in conds)
                    {
                        if (item.HisCondType == CondType.Dir)
                            continue; //如果是方向条件就不处理.

                        item.FK_Node = nf.NodeID;
                        item.FK_Flow = nf.FK_Flow;
                        //item.
                        //item.NodeID;
                        // item.Insert();
                    }

                    //复制事件.
                    FrmEvents fevents = new FrmEvents();
                    fevents.Retrieve(FrmEventAttr.FK_MapData, "ND"+fromNodeID);
                    foreach (FrmEvent item in fevents)
                    {

                        item.FK_MapData = item.FK_MapData.Replace("ND" + fromNodeID, "ND" + nf.NodeID);
                        item.DoDoc = item.DoDoc.Replace("ND" + fromNodeID, "ND" + nf.NodeID);

                        item.MyPK = item.FK_MapData + "_" + item.FK_Event;
                        item.Insert();
                    }
                }

                nf.ICON = icon;
                nf.Name = nodeName;
                nf.HisRunModel = (RunModel)HisRunModel;
                nf.Save();
                return nf.NodeID;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除一个连接线
        /// </summary>
        /// <param name="from">从节点</param>
        /// <param name="to">到节点</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool DoDropLine(int from, int to)
        {
            Direction dir = new Direction();
            dir.Node = from;
            dir.ToNode = to;
            dir.Delete();

            // songhonggang (2014-06-15) 删除连线的时候删除表单条件
            Conds conds =new Conds();
            conds.Delete(CondAttr.FK_Node, dir.Node);
            
            return true;
        }

        /// <summary>
        /// 创建一个标签
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>返回标签编号</returns>
        [WebMethod(EnableSession = true)]
        public string DoNewLabel(string fk_flow, int x, int y, string name, string lableId)
        {
            LabNote lab = new LabNote();
            lab.FK_Flow = fk_flow;
            lab.X = x;
            lab.Y = y;
            if (string.IsNullOrEmpty(lableId))
            {
                lab.MyPK = BP.DA.DBAccess.GenerOID().ToString();
            }
            else
            {
                lab.MyPK = lableId;
            }
            lab.Name = name;
            try
            {
                lab.Save();
            }
            catch
            {
            }
            return lab.MyPK;
        }

        [WebMethod]
        public string FlowTemplateUpload(byte[] FileByte, string fileName)
        {
            try
            {
                //文件存放路径
                string filepath = BP.Sys.SystemConfig.PathOfTemp + "\\" + fileName;
                //如果文件已经存在则删除
                if (File.Exists(filepath))
                    File.Delete(filepath);
                //创建文件流实例，用于写入文件
                FileStream stream = new FileStream(filepath, FileMode.CreateNew);
                //写入文件
                stream.Write(FileByte, 0, FileByte.Length);
                stream.Close();

                //保存图片.

                return filepath;
            }
            catch (Exception exception)
            {
                return "Error: Occured on upload the file. Error Message is :\n" + exception.Message;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FK_flowSort">流程类别编号</param>
        /// <param name="Path">模板文件路径</param>
        /// <param name="ImportModel"></param>
        /// <param name="Islogin">0,1,2,3</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string FlowTemplateLoad(string FK_flowSort, string Path,int ImportModel,int SpecialFlowNo)
        {
            try
            {
                ImpFlowTempleteModel model = (ImpFlowTempleteModel)ImportModel;
                LetAdminLogin("CH", true);
                Flow flow = null;
                if (model == ImpFlowTempleteModel.AsSpecFlowNo)
                {
                    if (SpecialFlowNo <= 0)
                    {
                        return "指定流程编号错误";
                    }

                    flow = Flow.DoLoadFlowTemplate(FK_flowSort, Path, model, SpecialFlowNo);
                }
                else
                {
                    flow = Flow.DoLoadFlowTemplate(FK_flowSort, Path, model);
                }

                //执行一下修复view.
                Flow.RepareV_FlowData_View();

                return string.Format("TRUE,{0},{1},{2}", FK_flowSort, flow.No, flow.Name);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="nodes"></param>
        /// <param name="dirs`"></param>
        /// <param name="labes"></param>
        [WebMethod(EnableSession = true)]
        public string FlowSave(string fk_flow, string nodes, string dirs, string labes)
        {
            LetAdminLogin("CH", true);
            return WorkflowDefintionManager.SaveFlow(fk_flow, nodes, dirs, labes);
        }

        [WebMethod]
        public string UploadfileCCForm(byte[] FileByte, string fileName, string fk_frmSort)
        {
            try
            {
                //文件存放路径
                string filepath = BP.Sys.SystemConfig.PathOfTemp + "\\" + fileName;
                //如果文件已经存在则删除
                if (File.Exists(filepath))
                    File.Delete(filepath);

                //创建文件流实例，用于写入文件
                FileStream stream = new FileStream(filepath, FileMode.CreateNew);

                //写入文件
                stream.Write(FileByte, 0, FileByte.Length);
                stream.Close();

                DataSet ds = new DataSet();
                ds.ReadXml(filepath);

                MapData md = MapData.ImpMapData(ds);
                md.FK_FrmSort = fk_frmSort;
                md.Update();
                return null;
            }
            catch (Exception exception)
            {
                return "Error: Occured on upload the file. Error Message is :\n" + exception.Message;
            }

        }

     
    }
}