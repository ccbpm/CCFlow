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

                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection(Glo.TemplateFTPHost, Glo.TemplateFTPUser, Glo.TemplateFTPPassword );
                return conn;
            }
        }
        public string Flow_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";


            return null;
        }
        #endregion 界面方法.

    }
}
