using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.Web;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;

namespace CCFlow.WF.WebOffice
{
    public partial class PrintOffice : System.Web.UI.Page
    {
        #region 属性
        /// <summary>
        /// MyPK编号
        /// </summary>
        public string MyPK
        {
            get
            {
                return this.Request["MyPK"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request["action"];
            if (!string.IsNullOrEmpty(type))
            {
                try
                {
                    Bill bill = new Bill();
                    bill.RetrieveByAttr(BillAttr.MyPK,this.MyPK);
                    if (!string.IsNullOrEmpty(bill.Url))
                    {
                        string path = Server.MapPath(bill.Url);
                        var result = File.ReadAllBytes(path);

                        Response.Clear();
                        Response.BinaryWrite(result);
                    }
                    else
                    {
                        throw new Exception("文件不存在!");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("文件打开失败!");
                }
            }
        }
    }
}