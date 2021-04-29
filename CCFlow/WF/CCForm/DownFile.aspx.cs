using System;
using System.IO;
using System.Web;
using BP.Web;
using BP.Sys;
using BP.Tools;
using BP.WF;
using BP;
using BP.En;
using ICSharpCode.SharpZipLib.Zip;

namespace CCFlow.WF.CCForm
{
    public partial class WF_CCForm_DownFile : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close();</script>");
        }
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }

        public string DoWhat
        {
            get
            {
                return this.Request.QueryString["DoWhat"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }

        /// <summary>
        /// ath.
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        public string IsReadonly
        {
            get
            {
                return this.Request.QueryString["IsReadonly"];
            }
        }
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        public string FK_FrmAttachment
        {
            get
            {
                return this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_FrmAttachmentExt
        {
            get
            {
                return "ND" + this.FK_Node + "_DocMultiAth"; // this.Request.QueryString["FK_FrmAttachment"];
            }
        }

        public int _fk_node = 0;
        public int FK_Node
        {
            get
            {
                if (_fk_node == 0 && !string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return int.Parse(this.Request.QueryString["FK_Node"]);

                return _fk_node;
            }
            set
            {
                _fk_node = value;
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = this.Request.QueryString["WorkID"];
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["OID"];

                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["PKVal"];
                return Int64.Parse(str);
            }
        }
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (string.IsNullOrEmpty(str))
                    return 0;
                return Int64.Parse(str);
            }
        }

        public string IsCC
        {
            get
            {
                string paras = this.Request.QueryString["Paras"];
                if (string.IsNullOrEmpty(paras) == false)
                    if (paras.Contains("IsCC=1") == true)
                        return "1";
                return "ssss";
            }
        }
        #endregion 属性.

        //附件文件下载
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            this.Request.ContentEncoding = System.Text.UTF8Encoding.UTF8;

            #region 增加跨域.
            //让其支持跨域访问.
            string origin = this.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin))
            {
                var allAccess_Control_Allow_Origin = System.Web.Configuration.WebConfigurationManager.AppSettings["Access-Control-Allow-Origin"];
                this.Response.Headers["Access-Control-Allow-Origin"] = origin;
                this.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                this.Response.Headers["Access-Control-Allow-Headers"] = "x-requested-with,content-type";
            }
            #endregion


            if (this.DoType == "Down")
            {
                //获取文件是否加密
                bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                FrmAttachmentDB downDB = new FrmAttachmentDB();

                downDB.MyPK = this.MyPK;
                downDB.Retrieve();
                FrmAttachment dbAtt = new FrmAttachment();
                dbAtt.MyPK = downDB.FK_FrmAttachment;
                dbAtt.Retrieve();

                if (dbAtt.ReadRole != 0 && this.FK_Node != 0)
                {
                    //标记已经阅读了.
                    GenerWorkerList gwf = new GenerWorkerList();
                    int count = gwf.Retrieve(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No,
                        GenerWorkerListAttr.FK_Node, this.FK_Node, GenerWorkerListAttr.WorkID, this.WorkID);
                    if (count != 0)
                    {
                        string str = gwf.GetParaString(dbAtt.NoOfObj);
                        str += "," + downDB.MyPK;
                        gwf.SetPara(dbAtt.NoOfObj, str);
                        gwf.Update();
                    }
                }

                bool isEncrypt = downDB.GetParaBoolen("IsEncrypt");

                if (dbAtt.AthSaveWay == AthSaveWay.IISServer)
                {
                    #region 解密下载
                    //1、先解密到本地
                    string filepath = downDB.FileFullName + ".tmp";
                    string tempName = downDB.FileName;

                    if (fileEncrypt == true && isEncrypt == true)
                    {
                        if (File.Exists(filepath) == true)
                            File.Delete(filepath);
                        EncHelper.DecryptDES(downDB.FileFullName, filepath);
                    }
                    else
                    {
                        filepath = downDB.FileFullName;
                    }

                    #region 文件下载（并删除临时明文文件）
                    if (!"firefox".Contains(HttpContext.Current.Request.Browser.Browser.ToLower()))
                        tempName = HttpUtility.UrlEncode(tempName);

                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + tempName);
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.ContentType = "application/octet-stream;charset=utf8";
                    HttpContext.Current.Response.WriteFile(filepath);
                    HttpContext.Current.Response.End();
                    HttpContext.Current.Response.Close();
                    #endregion

                    #endregion
                }

                if (dbAtt.AthSaveWay == AthSaveWay.FTPServer)
                {
                    //下载文件的临时位置
                    string tempFile = downDB.GenerTempFile(dbAtt.AthSaveWay);
                    string tempDescFile = tempFile + ".temp";
                    if (fileEncrypt == true && isEncrypt == true)
                        EncHelper.DecryptDES(tempFile, tempDescFile);
                    else
                        tempDescFile = tempFile;
                    BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(tempDescFile, downDB.FileName);
                }

                if (dbAtt.AthSaveWay == AthSaveWay.DB)
                {
                    string downpath = GetRealPath(downDB.FileFullName);
                    string filepath = downpath + ".tmp";
                    if (fileEncrypt == true && isEncrypt == true)
                    {
                        if (File.Exists(filepath) == true)
                            File.Delete(filepath);
                        EncHelper.DecryptDES(downpath, filepath);
                    }
                    else
                        filepath = downpath;

                    BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(filepath, downDB.FileName);

                }

                this.WinClose();
                return;
            }
            else if (this.DoType == "EntityFile_Load")
            {
                EntityFile_Load(sender, e);

                this.WinClose();
                return;
            }
            else if (this.DoType == "EntityMutliFile_Load")
            {
                EntityMutliFile_Load(sender, e);

                this.WinClose();
                return;

            }

        }

        //实体文件下载
        protected void EntityFile_Load(object sender, EventArgs e)
        {

            //根据EnsName获取Entity
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            en.PKVal = this.DelPKVal;
            int i = en.RetrieveFromDBSources();
            if (i == 0)
                return;
            //获取使用的客户 TianYe集团保存在FTP服务器上
            if (SystemConfig.CustomerNo.Equals("TianYe") || SystemConfig.IsUploadFileToFTP == true)
            {
                string filePath = (string)en.GetValByKey("MyFilePath");
                string fileName = (string)en.GetValByKey("MyFileName");
                string fileExt = (string)en.GetValByKey("MyFileExt");
                //临时存储位置
                string tempFile = SystemConfig.PathOfTemp + System.Guid.NewGuid() + "." + en.GetValByKey("MyFileExt");
                try
                {
                    if (System.IO.File.Exists(tempFile) == true)
                        System.IO.File.Delete(tempFile);
                }
                catch
                {
                    //  tempFile = SystemConfig.PathOfTemp + System.Guid.NewGuid() + this.FileName;
                }

                //连接FTP服务器
                FtpConnection conn = new FtpConnection(SystemConfig.FTPServerIP, SystemConfig.FTPServerPort,
                    SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);
                conn.GetFile(filePath, tempFile, false, System.IO.FileAttributes.Archive);
                conn.Close();

                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(tempFile, fileName + "." + fileExt);
                //删除临时文件
                System.IO.File.Delete(tempFile);
            }
            else
            {
                HttpContext.Current.Response.Charset = "GB2312";
                string fileName = HttpUtility.UrlEncode((string)en.GetValByKey("MyFileName"));
                string fileExt = HttpUtility.UrlEncode((string)en.GetValByKey("MyFileExt"));
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "filename=" + fileName + "." + fileExt);
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.ContentType = "application/octet-stream;charset=utf8";

                HttpContext.Current.Response.WriteFile((string)en.GetValByKey("MyFilePath"));
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Close();
            }
        }

        //实体文件下载
        protected void EntityMutliFile_Load(object sender, EventArgs e)
        {
            string oid = this.Request.QueryString["OID"];
            //根据SysFileManager的OID获取对应的实体
            SysFileManager fileManager = new SysFileManager();
            fileManager.PKVal = oid;
            int i = fileManager.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("没有找到OID=" + oid + "的文件管理数据，请联系管理员");

            //获取使用的客户 TianYe集团保存在FTP服务器上
            if (SystemConfig.CustomerNo.Equals("TianYe") || SystemConfig.IsUploadFileToFTP == true)
            {
                string filePath = fileManager.MyFilePath;
                string fileName = fileManager.MyFileName;
                //临时存储位置
                string tempFile = SystemConfig.PathOfTemp + System.Guid.NewGuid() + "." + fileManager.MyFileExt;
                try
                {
                    if (System.IO.File.Exists(tempFile) == true)
                        System.IO.File.Delete(tempFile);
                }
                catch
                {
                    //  tempFile = SystemConfig.PathOfTemp + System.Guid.NewGuid() + this.FileName;
                }

                //连接FTP服务器
                FtpConnection conn = new FtpConnection(SystemConfig.FTPServerIP, SystemConfig.FTPServerPort,
                    SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);
                conn.GetFile(filePath, tempFile, false, System.IO.FileAttributes.Archive);
                conn.Close();

                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(tempFile, fileName);
                //删除临时文件
                System.IO.File.Delete(tempFile);
            }
            else
            {
                HttpContext.Current.Response.Charset = "GB2312";
                string fileName = HttpUtility.UrlEncode(fileManager.MyFileName);
                string fileExt = HttpUtility.UrlEncode(fileManager.MyFileExt);
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "filename=" + fileName + fileExt);
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.ContentType = "application/octet-stream;charset=utf8";

                HttpContext.Current.Response.WriteFile(fileManager.MyFilePath);
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Close();
            }
        }

        void btn_DownLoad_Zip(object sender, EventArgs e)
        {
            try
            {
                BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment,
                        FrmAttachmentDBAttr.RefPKVal, this.PKVal);

                if (dbs.Count == 0)
                {
                    this.Response.Write("文件不存在，不需打包下载。");
                    return;
                }

                string zipName = this.WorkID + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string basePath = Server.MapPath("//DataUser//Temp");
                string tempPath = basePath + "//" + WebUser.No;
                string zipPath = basePath + "//" + WebUser.No;
                string zipFile = zipPath + "//" + zipName + ".zip";

                //删除临时文件，保证一个用户只能存一份，减少磁盘占用空间
                if (System.IO.Directory.Exists(tempPath) == true)
                    System.IO.Directory.Delete(tempPath, true);
                //根据路径创建文件夹
                if (System.IO.Directory.Exists(zipPath) == false)
                    System.IO.Directory.CreateDirectory(zipPath);
                //copy文件临时文件夹
                tempPath = tempPath + "//" + this.WorkID;
                if (System.IO.Directory.Exists(tempPath) == false)
                    System.IO.Directory.CreateDirectory(tempPath);

                foreach (FrmAttachmentDB db in dbs)
                {
                    string copyToPath = tempPath;
                    if (!File.Exists(db.FileFullName)) continue;

                    if (!string.IsNullOrEmpty(db.Sort))
                    {
                        copyToPath = tempPath + "//" + db.Sort;
                        if (System.IO.Directory.Exists(copyToPath) == false)
                            System.IO.Directory.CreateDirectory(copyToPath);
                    }
                    //新文件目录
                    copyToPath = copyToPath + "//" + db.FileName;
                    File.Copy(db.FileFullName, copyToPath, true);
                }
                //执行压缩
                (new FastZip()).CreateZip(zipFile, tempPath, true, "");
                //删除临时文件夹
                System.IO.Directory.Delete(tempPath, true);

                //显示出下载超链接
                //BP.Web.Controls.BPHyperLink hLink = (BP.Web.Controls.BPHyperLink)this.Pub1.FindControl("H_LINK_Btn");                
                //BP.PubClass.DownloadFile(zipFile, this.WorkID + ".zip");

            }
            catch (Exception ex)
            {
                //this.Alert(ex.Message);
            }
        }


        private string GetRealPath(string fileFullName)
        {
            bool isFile = false;
            string downpath = "";
            try
            {
                //如果相对路径获取不到可能存储的是绝对路径
                FileInfo downInfo = new FileInfo(Server.MapPath("~/" + fileFullName));
                isFile = true;
                downpath = Server.MapPath("~/" + fileFullName);
            }
            catch (Exception)
            {
                FileInfo downInfo = new FileInfo(fileFullName);
                isFile = true;
                downpath = fileFullName;
            }
            if (!isFile)
            {
                throw new Exception("没有找到下载的文件路径！");
            }

            return downpath;
        }


    }

}