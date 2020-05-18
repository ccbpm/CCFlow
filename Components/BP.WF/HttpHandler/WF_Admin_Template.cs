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
        /// <summary>
        /// 导入本机模版
        /// </summary>
        /// <returns></returns>
        public string ImpFrmLocal_Done()
        {
            try
            {
                ///表单类型.
                string frmSort = this.GetRequestVal("FrmSort");

                //创建临时文件.
                string temp = SystemConfig.PathOfTemp + "\\" + Guid.NewGuid() + ".xml";
                HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), temp);

                //获得数据类型.
                DataSet ds = new DataSet();
                ds.ReadXml(temp);

                //执行装载.
                MapData.ImpMapData(ds);

                return "导入成功.";
            }catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
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
            dtInfo.Columns.Add("Name");   //文件名.
            dtInfo.Columns.Add("Info");   //导入信息。
            dtInfo.Columns.Add("Result"); //执行结果.

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

                #region 下载文件.
                //设置要到的路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + "\\" + str;
                FtpStatus fs;
                try
                {
                    //下载目录下.
                    fs = conn.DownloadFile(tempfile, "/Flow" + remotePath + "/" + str, FtpLocalExists.Overwrite);
                }
                catch (Exception ex)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str,  ex.Message, "失败.");
                    continue;
                }

                if (fs.ToString().Equals("Success") == false)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, "模板未下载成", "失败.");
                    continue;
                }
                #endregion 下载文件.

                #region 执行导入.
                BP.WF.Flow flow = new BP.WF.Flow();
                try
                {
                    //执行导入.
                    flow = BP.WF.Flow.DoLoadFlowTemplate(sortNo, tempfile, ImpFlowTempleteModel.AsNewFlow);
                    flow.DoCheck(); //要执行一次检查.

                    dtInfo = this.ImpAddInfo(dtInfo, str, "执行成功:新流程编号:" + flow.No + " - " + flow.Name, "成功.");
                }
                catch (Exception ex)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, ex.Message, "导入失败.");
                }
                #endregion 执行导入.
            }

            return BP.Tools.Json.ToJson(dtInfo);
        }
        public DataTable ImpAddInfo(DataTable dtInfo, string fileName, string info, string result)
        {
            DataRow dr = dtInfo.NewRow();
            dr[0] = fileName;
            dr[1] = info;
            dr[2] = result;
            dtInfo.Rows.Add(dr);
            return dtInfo;
        }
        /// <summary>
        /// 导入表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_Imp()
        {
            //构造返回数据.
            DataTable dtInfo = new DataTable();
            dtInfo.Columns.Add("Name");   //文件名.
            dtInfo.Columns.Add("Info");   //导入信息.
            dtInfo.Columns.Add("Result"); //执行结果.

            //获得变量.
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');
            string sortNo = GetRequestVal("SortNo");
            string dirName = GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";

            FtpClient conn = this.GenerFTPConn;
            string remotePath = conn.GetWorkingDirectory() + dirName;

            ///遍历选择的文件.
            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;
                //设置要到的路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + "\\" + str;

                //下载目录下
                FtpStatus fs = conn.DownloadFile(tempfile, "/Form" + remotePath + "/" + str, FtpLocalExists.Overwrite);
                if (fs.ToString().Equals("Success") == false)
                {
                    DataRow dr = dtInfo.NewRow();
                    dr[0] = str;
                    dr[1] = "文件下载失败.";
                    dr[2] = "导入失败";
                    dtInfo.Rows.Add(dr);
                    continue;
                }

                //读取文件.
                DataSet ds = new DataSet();
                ds.ReadXml(tempfile);

                if (ds.Tables.Contains("Sys_MapData") == false)
                {
                    DataRow dr = dtInfo.NewRow();
                    dr[0] = str;
                    dr[1] = "模版不存在Sys_MapData表.";
                    dr[2] = "导入失败";
                    dtInfo.Rows.Add(dr);
                    continue;
                }

                //获得模版里的编号，检查是否存在.
                string no = ds.Tables["Sys_MapData"].Rows[0]["No"].ToString();
                MapData md = new MapData();
                md.No = no;
                if (md.RetrieveFromDBSources() == 1)
                {
                    md.No = no + "" + WebUser.OrgNo;
                    ds.Tables["Sys_MapData"].Rows[0]["No"] = md.No;
                    if (md.RetrieveFromDBSources() == 1)
                    {
                        DataRow dr = dtInfo.NewRow();
                        dr[0] = str;
                        dr[1] = "模版编号为:" + no + "，已经存在.";
                        dr[2] = "导入失败";
                        dtInfo.Rows.Add(dr);
                        continue;
                    }
                }

                try
                {
                    //执行装载.
                    MapData.ImpMapData(md.No,ds);
                    DataRow dr = dtInfo.NewRow();
                    dr[0] = str;
                    dr[1] = "执行成功:新模板编号:" + md.No + " -名称 " + md.Name;
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
                    continue;
                }
            }
            //返回执行结果.
            return BP.Tools.Json.ToJson(dtInfo);
        }
        #endregion 界面方法.

    }


}
