using System;
using System.Collections.Generic;
using System.Text;
using BP.DA;
using BP.GPM.WeiXin.Msg;
using System.Runtime.Serialization.Json;

namespace BP.GPM.WeiXin
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
    /// <summary>
    /// 获取企业的jsapi_ticket
    /// </summary>
    public class Ticket
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public string expires_in { get; set; }
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
    public enum MsgType
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
    /// 微信-文本消息
    /// </summary>
    public class MsgText : MsgBase
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

       
    }
    /// <summary>
    /// 微信-图片消息
    /// </summary>
    public class MsgImage : MsgBase
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
    }
    /// <summary>
    /// 微信-voice消息
    /// </summary>
    public class MsgVoice : MsgBase
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
    }
    /// <summary>
    /// 微信-video消息
    /// </summary>
    public class MsgVideo : MsgBase
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
    }
    /// <summary>
    /// 微信-file消息
    /// </summary>
    public class MsgFile : MsgBase
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
    }

    #region news消息

    /// <summary>
    /// 微信-news消息
    /// </summary>
    public class MsgNews : MsgBase
    {
        /// <summary>
        /// 必须：是- 息类型，此时固定为：news （不支持主页型应用）
        /// </summary>
        public string msgtype
        {
            get { return "news"; }
        }

        private List<NewsArticles> _Articles = new List<NewsArticles>();
        /// <summary>
        /// 必须：是- 图文消息，一个图文消息支持1到8条图文
        /// </summary>
        public List<NewsArticles> articles
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
    public class NewsArticles
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
    public class MsgMpNews : MsgBase
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
        public List<MpNewsArticles> articles { get; set; }
    }

    /// <summary>
    /// mpnews消息内容
    /// </summary>
    public class MpNewsArticles
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
