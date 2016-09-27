using System;
using System.Collections.Generic;
using System.Web;
using BP.Sys;
using System.IO;
using BP.Web;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using BP.DA;
using BP.WF.Template;
using BP.WF;
using CCFlow.WF.CCForm;


namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// JQFileUpload 的摘要说明
    /// </summary>
    public class CCFormHeader : IHttpHandler
    {
        public HttpContext context=null;
        public string FK_MapExt
        {
            get
            {
                return context.Request.QueryString["FK_MapExt"];
            }
        }
        public void ProcessRequest(HttpContext mycontext)
        {
            context= mycontext;
            context.Request.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            string doType = context.Request["DoType"];
            string attachPk = context.Request["AttachPK"];
            string workid = context.Request["WorkID"];
            string fk_node = context.Request["FK_Node"];
            string ensName = context.Request["EnsName"];
            string fk_flow = context.Request["FK_Flow"];
            string pkVal = context.Request["PKVal"];
            string message = "true";
            //判断是否包含附件，包含附件则是上传，否则是功能执行
            if (context.Request.Files.Count > 0)
            {
                switch (doType)
                {
                    case "SingelAttach"://单附件上传
                        SingleAttach(context, attachPk, workid, fk_node, ensName);
                        break;
                    case "MoreAttach"://多附件上传
                        MoreAttach(context, attachPk, workid, fk_node, ensName, fk_flow, pkVal);
                        break;
                }
            }
            else
            {
                switch (doType)
                {
                    case "InitPopSetting":
                        message = InitPopSetting();
                        break;
                    case "InitPopVal":
                        message = InitPopVal();
                        break;
                    case "InitLJZData":
                        message = InitPopValLJZ_Tree();
                        break;
                    case "DelWorkCheckAttach"://删除附件
                        message = DelWorkCheckAttach(pkVal);
                        break;
                    default:
                        break;
                }
            }
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            context.Response.Expires = 0;
            context.Response.Write(message);
            context.Response.End();
        }
        /// <summary>
        /// 获得Pop的设置.
        /// </summary>
        /// <returns></returns>
        public string InitPopSetting()
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapExt;
            me.Retrieve();
            return me.PopValToJson();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string InitPopValLJZ_Tree()
        {
            string mypk = context.Request.QueryString["FK_MapExt"];
            MapExt me = new MapExt();
            me.MyPK = mypk;
            me.Retrieve();

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("");
            return null;
        }

        /// <summary>
        /// 初始化PopVal的值
        /// </summary>
        /// <returns></returns>
        public string InitPopVal()
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapExt;
            me.Retrieve();

            DataSet ds = new DataSet();

            if (me.PopValWorkModel == PopValWorkModel.SelfUrl)
            {
                return BP.Tools.Json.ToJson(ds);
            }

            if (me.PopValWorkModel == PopValWorkModel.TableOnly)
            {
                string sqlObjs = me.PopValEntitySQL;

                sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                dt.TableName = "DTObjs";
                return BP.Tools.Json.ToJson(dt);
            }

            ds.Tables.Add(me.ToDataTableField("Sys_MapExt"));

            if (me.PopValWorkModel == PopValWorkModel.Group)
            {
                string sqlObjs = me.PopValGroupSQL;
                if (sqlObjs.Length > 10)
                {
                    sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                    dt.TableName = "DTGroup";
                    ds.Tables.Add(dt);
                }

                sqlObjs = me.PopValEntitySQL;
                if (sqlObjs.Length > 10)
                {
                    sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
                    dt.TableName = "DTEntity";
                    ds.Tables.Add(dt);
                }
                return BP.Tools.Json.ToJson(ds);
            }
          
            //把配置信息放入进去.
            ds.Tables.Add(me.ToDataTableField("Sys_MapExt"));
            return BP.Tools.Json.ToJson(ds);
        }

        //单附件上传方法
        private void SingleAttach(HttpContext context, string attachPk, string workid, string fk_node, string ensName)
        {
            FrmAttachment frmAth = new FrmAttachment();
            frmAth.MyPK = attachPk;
            frmAth.RetrieveFromDBSources();

            string athDBPK = attachPk + "_" + workid;

            BP.WF.Node currND = new BP.WF.Node(fk_node);
            BP.WF.Work currWK = currND.HisWork;
            currWK.OID = long.Parse(workid);
            currWK.Retrieve();
            //处理保存路径.
            string saveTo = frmAth.SaveTo;

            if (saveTo.Contains("*") || saveTo.Contains("@"))
            {
                /*如果路径里有变量.*/
                saveTo = saveTo.Replace("*", "@");
                saveTo = BP.WF.Glo.DealExp(saveTo, currWK, null);
            }

            try
            {
                saveTo = context.Server.MapPath("~/" + saveTo);
            }
            catch 
            {
                //saveTo = saveTo;
            }

            if (System.IO.Directory.Exists(saveTo) == false)
                System.IO.Directory.CreateDirectory(saveTo);


            saveTo = saveTo + "\\" + athDBPK + "." + context.Request.Files[0].FileName.Substring(context.Request.Files[0].FileName.LastIndexOf('.') + 1);
            context.Request.Files[0].SaveAs(saveTo);

            FileInfo info = new FileInfo(saveTo);


            FrmAttachmentDB dbUpload = new FrmAttachmentDB();
            dbUpload.MyPK = athDBPK;
            dbUpload.FK_FrmAttachment = attachPk;
            dbUpload.RefPKVal = workid;

            dbUpload.FK_MapData = ensName;

            dbUpload.FileExts = info.Extension;

            #region 处理文件路径，如果是保存到数据库，就存储pk.
            if (frmAth.SaveWay == 0)
            {
                //文件方式保存
                dbUpload.FileFullName = saveTo;
            }

            if (frmAth.SaveWay == 1)
            {
                //保存到数据库
                dbUpload.FileFullName = dbUpload.MyPK;
            }
            #endregion 处理文件路径，如果是保存到数据库，就存储pk.


            dbUpload.FileName = context.Request.Files[0].FileName;
            dbUpload.FileSize = (float)info.Length;
            dbUpload.Rec = WebUser.No;
            dbUpload.RecName = WebUser.Name;
            dbUpload.RDT = BP.DA.DataType.CurrentDataTime;

            dbUpload.NodeID = fk_node;
            dbUpload.Save();

            if (frmAth.SaveWay == 1)
            {
                //执行文件保存.
                BP.DA.DBAccess.SaveFileToDB(saveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
            }

        }

        //多附件上传方法
        public void MoreAttach(HttpContext context, string attachPk, string workid, string fk_node, string ensNamestring, string fk_flow, string pkVal)
        {
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                string savePath = athDesc.SaveTo;
                if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                {
                    /*如果有变量*/
                    savePath = savePath.Replace("*", "@");
                    GEEntity en = new GEEntity(athDesc.FK_MapData);
                    en.PKVal = pkVal;
                    en.Retrieve();
                    savePath = BP.WF.Glo.DealExp(savePath, en, null);

                    if (savePath.Contains("@") && fk_node != null)
                    {
                        /*如果包含 @ */
                        BP.WF.Flow flow = new BP.WF.Flow(fk_flow);
                        BP.WF.Data.GERpt myen = flow.HisGERpt;
                        myen.OID = long.Parse(workid);
                        myen.RetrieveFromDBSources();
                        savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                    }
                    if (savePath.Contains("@") == true)
                        throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                }
                else
                {
                    savePath = athDesc.SaveTo + "\\" + pkVal;
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


                string exts = System.IO.Path.GetExtension(context.Request.Files[i].FileName).ToLower().Replace(".", "");


                string guid = BP.DA.DBAccess.GenerGUID();
                string fileName = context.Request.Files[i].FileName.Substring(0, context.Request.Files[i].FileName.LastIndexOf('.'));
                string ext = System.IO.Path.GetExtension(context.Request.Files[i].FileName);
                string realSaveTo = savePath + "/" + guid + "." + fileName + ext;

                realSaveTo = realSaveTo.Replace("~", "-");
                realSaveTo = realSaveTo.Replace("'", "-");
                realSaveTo = realSaveTo.Replace("*", "-");


                context.Request.Files[i].SaveAs(realSaveTo);

                FileInfo info = new FileInfo(realSaveTo);

                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
                dbUpload.NodeID = fk_node.ToString();
                dbUpload.FK_FrmAttachment = attachPk;
                dbUpload.FK_MapData = athDesc.FK_MapData;
                dbUpload.FK_FrmAttachment = attachPk;
                dbUpload.FileExts = info.Extension;

                #region 处理文件路径，如果是保存到数据库，就存储pk.
                if (athDesc.SaveWay == 0)
                {
                    //文件方式保存
                    dbUpload.FileFullName = realSaveTo;
                }

                if (athDesc.SaveWay == 1)
                {
                    //保存到数据库
                    dbUpload.FileFullName = dbUpload.MyPK;
                }
                #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                dbUpload.FileName = context.Request.Files[i].FileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDataTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.RefPKVal = pkVal;
                //if (athDesc.IsNote)
                //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                //if (athDesc.Sort.Contains(","))
                //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

                dbUpload.UploadGUID = guid;
                dbUpload.Insert();

                if (athDesc.SaveWay == 1)
                {
                    //执行文件保存.
                    BP.DA.DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                }
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="MyPK"></param>
        /// <returns></returns>
        private string DelWorkCheckAttach(string MyPK)
        {
            FrmAttachmentDB athDB = new FrmAttachmentDB();
            athDB.RetrieveByAttr(FrmAttachmentDBAttr.MyPK, MyPK);
            //删除文件
            if (athDB.FileFullName != null)
            {
                if (File.Exists(athDB.FileFullName) == true)
                    File.Delete(athDB.FileFullName);
            }
            int i = athDB.Delete(FrmAttachmentDBAttr.MyPK, MyPK);
            if (i > 0)
                return "true";
            return "false";
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}