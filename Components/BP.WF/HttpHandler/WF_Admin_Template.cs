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
using FtpSupport;
using System.Collections;

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
        public FtpSupport.FtpConnection GenerFTPConn
        {
            get
            {

                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection(Glo.TemplateFTPHost, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
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
                dirName = "/";

            FtpSupport.FtpConnection conn = this.GenerFTPConn;
            conn.SetCurrentDirectory(dirName);

            DataSet ds = new DataSet();
            Win32FindData[] fls;
            try
            {
                fls = conn.FindFiles();
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
            foreach (Win32FindData fl in fls)
            {
                switch (fl.FileAttributes)
                {
                    case System.IO.FileAttributes.Directory:
                        DataRow drDir = dtDir.NewRow(); ;
                        drDir[0] = fl.FileName;
                        drDir[1] = fl.CreationTime.ToString("yyyy-MM-dd HH:mm");
                        drDir[2] = conn.GetCurrentDirectory() + "/" + fl.FileName;
                        dtDir.Rows.Add(drDir);
                        continue;
                    case System.IO.FileAttributes.System:
                    case System.IO.FileAttributes.Hidden:
                        continue;
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0] = fl.FileName;
                dr[1] = fl.CreationTime.ToString("yyyy-MM-dd HH:mm");
                dr[2] = conn.GetCurrentDirectory() + "/" + fl.FileName;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 导入文件
        /// </summary>
        /// <returns></returns>
        public string Flow_Imp()
        {
            //组织变量.
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';'); //获得选择的文件.
            string dirName = this.GetRequestVal("DirName"); //所在的路径.
            string sortNo = GetRequestVal("SortNo"); //流程目录树的编号.

            //构造信息存储.
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Info", typeof(string));

            //执行导入.
            FtpConnection conn = this.GenerFTPConn;
            conn.SetCurrentDirectory(dirName); //设置当前路径.

            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;
                //生成路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + "\\" + str;
                //下载目录下.
                try
                {
                    conn.GetFile(str, tempfile, false, System.IO.FileAttributes.Normal);
                    //执行导入.
                    BP.WF.Flow flow = BP.WF.Flow.DoLoadFlowTemplate(sortNo, tempfile, ImpFlowTempleteModel.AsNewFlow);
                    flow.DoCheck(); //要执行一次检查
                    DataRow dr = dt.NewRow();
                    dr["No"] = flow.No;
                    dr["Name"] = flow.Name;
                    dr["Info"] = str + ",导入成功." ;
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = "000";
                    dr["Name"] = tempfile;
                    dr["Info"] = "导入失败:" + str;
                    dt.Rows.Add(dr);
                }
            }

            //返回执行结果.
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 界面方法.

    }


}
