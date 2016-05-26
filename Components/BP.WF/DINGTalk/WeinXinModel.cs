using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.WXin
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
    public class User
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
    public class MessageErrorModel
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string invaliduser { get; set; }
        public string invalidparty { get; set; }
        public string invalidtag { get; set; }
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum WX_MsgType
    {
        text = 0,
        image = 1,
        voice = 2,
        video = 3,
        file = 4,
        news = 5,
        mapnews = 6
    }
    /// <summary>
    /// 微信-消息公共类
    /// </summary>
    abstract public class WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 调用接口凭证
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 必须：否- 成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送 
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 必须：否- 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为@all时忽略本参数 
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 必须：否- 标签ID列表，多个接收者用‘|’分隔。当touser为@all时忽略本参数 
        /// </summary>
        public string totag { get; set; }
        /// <summary>
        /// 必须：是- 企业应用的id，整型。可在应用的设置页面查看 
        /// </summary>
        public string agentid { get; set; }
        /// <summary>
        /// 必须：否- ccflow 业务ID
        /// </summary>
        public string WorkID { get; set; }
    }

    /// <summary>
    /// 微信-文本消息
    /// </summary>
    public class WX_Msg_Text : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：text （支持消息型应用跟主页型应用） 
        /// </summary>
        public string msgtype
        {
            get { return "text"; }
        }
        /// <summary>
        /// 必须：是- 消息内容，最长不超过2048个字节，注意：主页型应用推送的文本消息在微信端最多只显示20个字（包含中英文）
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    /// <summary>
    /// 微信-图片消息
    /// </summary>
    public class WX_Msg_Image : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：image（不支持主页型应用）
        /// </summary>
        public string msgtype
        {
            get { return "image"; }
        }
        /// <summary>
        /// 必须：是- 图片媒体文件id，可以调用上传临时素材或者永久素材接口获取,永久素材media_id必须由发消息的应用创建
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    /// <summary>
    /// 微信-voice消息
    /// </summary>
    public class WX_Msg_Voice : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：voice （不支持主页型应用）
        /// </summary>
        public string msgtype
        {
            get { return "voice"; }
        }
        /// <summary>
        /// 必须：是- 语音文件id，可以调用上传临时素材或者永久素材接口获取
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    /// <summary>
    /// 微信-video消息
    /// </summary>
    public class WX_Msg_Video : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：video （不支持主页型应用）
        /// </summary>
        public string msgtype
        {
            get { return "video"; }
        }
        /// <summary>
        /// 必须：是- 视频媒体文件id，可以调用上传临时素材或者永久素材接口获取 
        /// </summary>
        public string media_id { get; set; }
        /// <summary>
        /// 必须：否- 视频消息的标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 必须：否- 视频消息的描述，不超过512个字节，超过会自动截断 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    /// <summary>
    /// 微信-file消息
    /// </summary>
    public class WX_Msg_File : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：file （不支持主页型应用） 
        /// </summary>
        public string msgtype
        {
            get { return "file"; }
        }
        /// <summary>
        /// 必须：是- 媒体文件id，可以调用上传临时素材或者永久素材接口获取
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    #region news消息

    /// <summary>
    /// 微信-news消息
    /// </summary>
    public class WX_Msg_News : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 息类型，此时固定为：news （不支持主页型应用）
        /// </summary>
        public string msgtype
        {
            get { return "news"; }
        }

        private List<News_Articles> _Articles = new List<News_Articles>();
        /// <summary>
        /// 必须：是- 图文消息，一个图文消息支持1到8条图文
        /// </summary>
        public List<News_Articles> articles
        {
            get
            {
                return _Articles;
            }
        }
    }

    /// <summary>
    /// News消息内容
    /// </summary>
    public class News_Articles
    {
        /// <summary>
        /// 必须：否- 标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 必须：否- 描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 必须：否- 点击后跳转的链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 必须：否- 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80。如不填，在客户端不显示图片
        /// </summary>
        public string picurl { get; set; }
    }
    #endregion

    #region mpnews消息

    /// <summary>
    /// 微信-mpnews消息-mpnews消息与news消息类似，不同的是图文消息内容存储在微信后台，并且支持保密选项。每个应用每天最多可以发送100次
    /// </summary>
    public class WX_Msg_MpNews : WX_MsgBase
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：mpnews （不支持主页型应用） 
        /// </summary>
        public string msgtype
        {
            get { return "mpnews"; }
        }
        /// <summary>
        /// 必须：是- 图文消息，一个图文消息支持1到8条图文
        /// </summary>
        public List<MpNews_Articles> articles { get; set; }

        /// <summary>
        /// 表示是否是保密消息
        /// </summary>
        private string _Safe = "0";
        /// <summary>
        /// 必须：否- 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe
        {
            get
            {
                return this._Safe;
            }
            set
            {
                this._Safe = value;
            }
        }
    }

    /// <summary>
    /// mpnews消息内容
    /// </summary>
    public class MpNews_Articles
    {
        /// <summary>
        /// 必须：是- 图文消息的标题，不超过128个字节，超过会自动截断 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 必须：是- 描述，图文消息缩略图的media_id, 可以在上传多媒体文件接口中获得。此处thumb_media_id即上传接口返回的media_id 
        /// </summary>
        public string thumb_media_id { get; set; }
        /// <summary>
        /// 必须：否- 描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// 必须：否- 图文消息点击“阅读原文”之后的页面链接
        /// </summary>
        public string content_source_url { get; set; }
        /// <summary>
        /// 必须：是- 图文消息的内容，支持html标签，不超过666 K个字节
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 必须：否- 图文消息的描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string digest { get; set; }
        /// <summary>
        /// 必须：否- 否显示封面，1为显示，0为不显示
        /// </summary>
        public string show_cover_pic { get; set; }
    }
    #endregion
}
