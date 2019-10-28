using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.DataUser
{
    public partial class HandlerOfMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string sql = "";

            string doType = this.Request.QueryString["DoType"];
            if (doType == "SendToCCMSG")
            {

                byte[] data = new byte[this.Request.InputStream.Length];
                this.Request.InputStream.Read(data, 0, data.Length);
                string txt = System.Text.Encoding.UTF8.GetString(data);
                try
                {
                    Dictionary<string, object> dictionary = null;
                    dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);
                    string send = dictionary["sender"].ToString();
                    string sendTo = dictionary["sendTo"].ToString();
                    string tel = "";
                    if(dictionary["tel"]!=null)
                        tel = dictionary["tel"].ToString();
                    string title = dictionary["title"].ToString();
                    string content = dictionary["content"].ToString();
                    string openUrl = dictionary["openUrl"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}