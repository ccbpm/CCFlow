using System;
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
using System.Collections;
using FluentFTP;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Template : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Template()
        {
        }

        #region  界面 .
        public FtpClient GenerFTPConn
        {
            get
            {
                FtpClient conn = new FtpClient(Glo.TemplateFTPHost, Glo.TemplateFTPPort, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
                conn.Encoding = Encoding.GetEncoding("GB2312");
                //FtpClient conn = new FtpClient(Glo.TemplateFTPHost, Glo.TemplateFTPPort, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
                return conn;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Flow_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/Flow/";
            if (dirName.IndexOf("/Flow/") == -1)
                dirName = "/Flow/" + dirName;
            FtpClient conn = this.GenerFTPConn;
            DataSet ds = new DataSet();
            FtpListItem[] fls;
            try
            {
                fls = conn.GetListing(dirName);
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("该目录无文件");
                return "err@该目录无文件";
            }
            DataTable dtDir = new DataTable();
            dtDir.TableName = "Dir";
            dtDir.Columns.Add("FileName", typeof(string));
            dtDir.Columns.Add("RDT", typeof(string));
            dtDir.Columns.Add("Path", typeof(string));
            ds.Tables.Add(dtDir);

            //把文件加里面.
            DataTable dtFile = new DataTable();
            dtFile.TableName = "File";
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("RDT", typeof(string));
            dtFile.Columns.Add("Path", typeof(string));
            foreach (FtpListItem fl in fls)
            {

                switch (fl.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        {
                            DataRow drDir = dtDir.NewRow();
                            drDir[0] = fl.Name;
                            drDir[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                            drDir[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                            dtDir.Rows.Add(drDir);
                            continue;
                        }
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0] = fl.Name;
                dr[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                dr[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/Form/";
            if (dirName.IndexOf("/Form/") == -1)
                dirName = "/Form/" + dirName;
            FtpClient conn = this.GenerFTPConn;
            DataSet ds = new DataSet();
            FtpListItem[] fls;
            try
            {
                fls = conn.GetListing(dirName);
            }
            catch
            {

                //System.Windows.Forms.MessageBox.Show("该目录无文件");
                return "err@该目录无文件";
            }

            DataTable dtDir = new DataTable();
            dtDir.TableName = "Dir";
            dtDir.Columns.Add("FileName", typeof(string));
            dtDir.Columns.Add("RDT", typeof(string));
            dtDir.Columns.Add("Path", typeof(string));
            ds.Tables.Add(dtDir);

            //把文件加里面.
            DataTable dtFile = new DataTable();
            dtFile.TableName = "File";
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("RDT", typeof(string));
            dtFile.Columns.Add("Path", typeof(string));
            foreach (FtpListItem fl in fls)
            {

                switch (fl.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        {
                            DataRow drDir = dtDir.NewRow();
                            drDir[0] = fl.Name;
                            drDir[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                            drDir[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                            dtDir.Rows.Add(drDir);
                            continue;
                        }
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0] = fl.Name;
                dr[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                dr[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 导入流程模板
        /// </summary>
        /// <returns></returns>
        public string Flow_Imp()
        {
            //构造返回数据.
            DataTable dtInfo = new DataTable();
            dtInfo.Columns.Add("Name");
            dtInfo.Columns.Add("Info");
            dtInfo.Columns.Add("Result");

            //获得下载的文件名.
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');

            string sortNo = GetRequestVal("SortNo");//流程类别.

            string dirName = GetRequestVal("DirName"); //目录名称.
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";

            FtpClient conn = this.GenerFTPConn;
            string remotePath = conn.GetWorkingDirectory() + dirName;

            string err = "";
            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;

                //设置要到的路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + "\\" + str;

                //下载目录下.
                FtpStatus fs = conn.DownloadFile(tempfile, "/Flow" + remotePath + "/" + str, FtpLocalExists.Overwrite);

                if (fs.ToString().Equals("Success") == false)
                    return "err@模板未下载成功" + fs.ToString();

                try
                {
                    //执行导入.
                    BP.WF.Flow flow = new BP.WF.Flow();
                    flow = BP.WF.Flow.DoLoadFlowTemplate(sortNo, tempfile, ImpFlowTempleteModel.AsNewFlow);
                    flow.DoCheck(); //要执行一次检查

                    DataRow dr = dtInfo.NewRow();
                    dr[0] = str;
                    dr[1] = "执行成功:新流程编号:" + flow.No;
                    dr[2] = "导入成功";
                    dtInfo.Rows.Add(dr);

                }
                catch (Exception ex)
                {
                    DataRow dr = dtInfo.NewRow();
                    dr[0] = str;
                    dr[1] = ex.Message;
                    dr[2] = "导入失败";
                    dtInfo.Rows.Add(dr);
                }
            }

            return BP.Tools.Json.ToJson(dtInfo);
        }
        /// <summary>
        /// 导入表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_Imp()
        {
            string Msg = "";
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');
            string sortNo = GetRequestVal("SortNo");
            string dirName = GetRequestVal("DirName");

            FtpClient conn = this.GenerFTPConn;
            string remotePath = conn.GetWorkingDirectory() + dirName;

            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;
                //设置要到的路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + "\\" + str;
                try
                {
                    //下载目录下
                    FtpStatus fs = conn.DownloadFile(tempfile, "/Form" + remotePath + "/" + str, FtpLocalExists.Overwrite);
                    if (fs.ToString() == "Success")
                    {
                        //执行导入
                        DataSet ds = new DataSet();
                        ds.ReadXml(tempfile);
                        try
                        {
                            //执行装载.
                            MapData.ImpMapData(ds);
                            if (this.FK_Node != 0)
                            {
                                Node nd = new Node(this.FK_Node);
                                nd.RepareMap(nd.HisFlow);
                            }
                            Msg = "导入完毕";
                        }
                        catch
                        {
                            Msg = "导入失败";
                        }
                    }
                    else
                    {
                        return "模板下载未成功";
                    }
                }
                catch (Exception ex)
                {
                    return "模板下载未成功-" + ex.Message;
                }
            }

            return Msg;
        }
        #endregion 界面方法.

    }


}
