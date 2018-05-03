using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_CCForm : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_CCForm(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        public string HandlerMapExt()
        {
            WF_CCForm en = new WF_CCForm(this.context);
            return en.HandlerMapExt();
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.AttachmentUpload_Down();
        }
        /// <summary>
        /// 表单初始化.
        /// </summary>
        /// <returns></returns>
        public string Frm_Init()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.Frm_Init();
        }

        public string Dtl_Init()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.Dtl_Init();
        }

        //多附件上传方法
        public void MoreAttach()
        {
            string PKVal = this.GetRequestVal("PKVal");
            string attachPk = this.GetRequestVal("AttachPK");
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);
            MapData mapData = new MapData(athDesc.FK_MapData);
            string msg = null;
            GEEntity en = new GEEntity(athDesc.FK_MapData);
            en.PKVal = PKVal;
            en.Retrieve();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile file = context.Request.Files[i];

                #region 文件上传的iis服务器上 or db数据库里.
                if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                {

                    string savePath = athDesc.SaveTo;
                    if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                    {
                        /*如果有变量*/
                        savePath = savePath.Replace("*", "@");
                        savePath = BP.WF.Glo.DealExp(savePath, en, null);

                        if (savePath.Contains("@") && this.FK_Node != 0)
                        {
                            /*如果包含 @ */
                            BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                            BP.WF.Data.GERpt myen = flow.HisGERpt;
                            myen.OID = this.WorkID;
                            myen.RetrieveFromDBSources();
                            savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                        }
                        if (savePath.Contains("@") == true)
                            throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                    }
                    else
                    {
                        savePath = athDesc.SaveTo + "\\" + PKVal;
                    }

                    //替换关键的字串.
                    savePath = savePath.Replace("\\\\", "\\");
                    try
                    {
                        savePath = context.Server.MapPath("~/" + savePath);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (System.IO.Directory.Exists(savePath) == false)
                            System.IO.Directory.CreateDirectory(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + context.Server.MapPath("~/" + savePath) + "===" + savePath + "@技术问题:" + ex.Message);
                    }

                    string exts = System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "");
                    string guid = BP.DA.DBAccess.GenerGUID();
                    
                    string fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                    if (fileName.LastIndexOf("\\") > 0)
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    string ext = System.IO.Path.GetExtension(file.FileName);
                    string realSaveTo = savePath + "\\" + guid + "." + fileName + ext;

                    realSaveTo = realSaveTo.Replace("~", "-");
                    realSaveTo = realSaveTo.Replace("'", "-");
                    realSaveTo = realSaveTo.Replace("*", "-");

                    file.SaveAs(realSaveTo);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + realSaveTo);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

                        try
                        {
                            File.Delete(realSaveTo);
                        }
                        catch { }

                        //note:此处如何向前uploadify传递失败信息，有待研究
                        //this.Alert("上传附件错误：" + msg, true);
                        return;
                    }

                    FileInfo info = new FileInfo(realSaveTo);

                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
                    dbUpload.NodeID = this.FK_Node.ToString();
                    dbUpload.FK_FrmAttachment = attachPk;
                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = attachPk;
                    dbUpload.FileExts = info.Extension;

                    #region 处理文件路径，如果是保存到数据库，就存储pk.
                    if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                    {
                        //文件方式保存
                        dbUpload.FileFullName = realSaveTo;
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        //保存到数据库
                        dbUpload.FileFullName = dbUpload.MyPK;
                    }
                    #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                    dbUpload.FileName = fileName + ext;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.RDT = DataType.CurrentDataTimess;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    dbUpload.RefPKVal = PKVal;
                    dbUpload.FID = this.FID;

                    //if (athDesc.IsNote)
                    //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                    //if (athDesc.Sort.Contains(","))
                    //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

                    dbUpload.UploadGUID = guid;
                    dbUpload.Insert();

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        //执行文件保存.
                        BP.DA.DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + dbUpload.FileFullName);
                    if (!string.IsNullOrEmpty(msg))
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
                }
                #endregion 文件上传的iis服务器上 or db数据库里.

                #region 保存到数据库 / FTP服务器上.
                if (athDesc.AthSaveWay == AthSaveWay.DB || athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    string guid = DBAccess.GenerGUID();

                    //把文件临时保存到一个位置.
                    string temp = SystemConfig.PathOfTemp + "" + guid + ".tmp";
                    try
                    {
                        file.SaveAs(temp);
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.Delete(temp);
                        file.SaveAs(temp);
                    }

                    //  fu.SaveAs(temp);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + temp);
                    if (string.IsNullOrEmpty(msg) == false)
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

                        try
                        {
                            File.Delete(temp);
                        }
                        catch
                        {
                        }

                        throw new Exception("err@上传附件错误：" + msg);
                    }

                    FileInfo info = new FileInfo(temp);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = BP.DA.DBAccess.GenerGUID();
                    dbUpload.NodeID = FK_Node.ToString();
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                    dbUpload.FID = this.FID; //流程id.
                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /*如果是继承，就让他保持本地的PK. */
                        dbUpload.RefPKVal = PKVal.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*如果是协同，就让他是PWorkID. */
                        string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + PKVal, 0).ToString();
                        if (pWorkID == null || pWorkID == "0")
                            pWorkID = PKVal;
                        dbUpload.RefPKVal = pWorkID;
                    }

                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                    dbUpload.FileName = file.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.RDT = DataType.CurrentDataTimess;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    //if (athDesc.IsNote)
                    //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                    //if (athDesc.Sort.Contains(","))
                    //{
                    //    string[] strs = athDesc.Sort.Contains("@") == true ? athDesc.Sort.Substring(athDesc.Sort.LastIndexOf("@") + 1).Split(',') : athDesc.Sort.Split(',');
                    //    BP.Web.Controls.DDL ddl = this.Pub1.GetDDLByID("ddl");
                    //    dbUpload.Sort = strs[0];
                    //    if (ddl != null)
                    //    {
                    //        int selectedIndex = string.IsNullOrEmpty(ddl.SelectedItemStringVal) ? 0 : int.Parse(ddl.SelectedItemStringVal);
                    //        dbUpload.Sort = strs[selectedIndex];
                    //    }
                    //}

                    dbUpload.UploadGUID = guid;

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        dbUpload.Insert();
                        //把文件保存到指定的字段里.
                        dbUpload.SaveFileToDB("FileDB", temp);
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        /*保存到fpt服务器上.*/
                        FtpSupport.FtpConnection ftpconn = new FtpSupport.FtpConnection(SystemConfig.FTPServerIP,
                            SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);

                        string ny = DateTime.Now.ToString("yyyy_MM");

                        //判断目录年月是否存在.
                        if (ftpconn.DirectoryExist(ny) == false)
                            ftpconn.CreateDirectory(ny);
                        ftpconn.SetCurrentDirectory(ny);

                        //判断目录是否存在.
                        if (ftpconn.DirectoryExist(athDesc.FK_MapData) == false)
                            ftpconn.CreateDirectory(athDesc.FK_MapData);

                        //设置当前目录，为操作的目录。
                        ftpconn.SetCurrentDirectory(athDesc.FK_MapData);

                        //把文件放上去.
                        ftpconn.PutFile(temp, guid + "." + dbUpload.FileExts);
                        ftpconn.Close();

                        //设置路径.
                        dbUpload.FileFullName = ny + "//" + athDesc.FK_MapData + "//" + guid + "." + dbUpload.FileExts;
                        dbUpload.Insert();
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + temp);
                    if (!string.IsNullOrEmpty(msg))
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
                }
                #endregion 保存到数据库.
            }
        }
    }
}
