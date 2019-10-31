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

            string doType = this.Request.QueryString["DoType"]; //消息类型标记,在节点事件上配置的标记.
            if (doType == "SendToCCMSG")
            {
                byte[] data = new byte[this.Request.InputStream.Length];
                this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                string txt = System.Text.Encoding.UTF8.GetString(data);  //编码.

                //转成json.
                Dictionary<string, object> dictionary = null;
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);

                //获得里面的参数.
                string send = dictionary["sender"].ToString(); //发送人.
                string sendTo = dictionary["sendTo"].ToString(); //发送给 与人员表Port_Emp的No一致.
                string tel = "";
                if (dictionary["tel"] != null) //配置的电话。
                    tel = dictionary["tel"].ToString();

                string title = dictionary["title"].ToString(); //标题
                string content = dictionary["content"].ToString(); //信息内容.
                string openUrl = dictionary["openUrl"].ToString(); //要打开的url.

                //执行相关处理,接受以上参数后，可以发送丁丁，微信，手机短信，或者其他的即时通讯里面去.
            }
        }
    }
}