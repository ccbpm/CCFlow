using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web;
using BP.DA;

namespace CCFlow.WF.Comm
{
    public partial class HelperOfSiganture : System.Web.UI.Page
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!DataType.IsNullOrEmpty(Request["method"]))
                method = Request["method"].ToString();

            switch (method)
            {
                case "sigantureact"://数字签名
                    s_responsetext = SigantureAction();
                    break;
            }
            if (DataType.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        /// <summary>
        /// 数字签名
        /// </summary>
        /// <returns></returns>
        private string SigantureAction()
        {
            string imgSrc = getUTF8ToString("imgSrc");
            string UserNo = getUTF8ToString("UserNo");
            string FK_MapData = getUTF8ToString("FK_MapData");
            string KeyOfEn = getUTF8ToString("KeyOfEn");
            string WorkID = getUTF8ToString("WorkID");
            //修改表数据
            MapData md = new MapData(FK_MapData);
            if (imgSrc.Contains(UserNo) || imgSrc.Contains("UnName"))
            {
                DBAccess.RunSQL("UPDATE " + md.PTable + " SET " + KeyOfEn + "='' WHERE OID=" + WorkID);
                return "siganture";
            }
            else
            {
                DBAccess.RunSQL("UPDATE " + md.PTable + " SET " + KeyOfEn + "='" + UserNo + "' WHERE OID=" + WorkID);
                return UserNo;                
            }
        }
    }
}