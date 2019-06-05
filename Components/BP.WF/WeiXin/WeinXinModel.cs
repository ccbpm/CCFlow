using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.WeiXin
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

    #region 组织结构

    /// <summary>
    /// 部门列表
    /// </summary>
    public class DeptMent_GetList
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public string errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 部门列表数据
        /// </summary>
        public List<DeptMentInfo> department { get; set; }
    }

    /// <summary>
    /// 部门信息
    /// </summary>
    public class DeptMentInfo
    {
        /// <summary>
        ///  部门id 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 部门名称 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///  父亲部门id。根部门为1 
        /// </summary>
        public string parentid { get; set; }
        /// <summary>
        ///  在父部门中的次序值。order值小的排序靠前
        /// </summary>
        public string order { get; set; }
    }

    /// <summary>
    /// 部门下的人员
    /// </summary>
    public class UsersBelongDept
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public string errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<UserInfoBelongDept> userlist { get; set; }
    }
    /// <summary>
    /// 部门人员详情
    /// </summary>
    public class UserInfoBelongDept
    {
        /// <summary>
        /// 成员UserID。对应管理端的帐号
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 成员所属部门
        /// </summary>
        public object department { get; set; }
        /// <summary>
        /// 职位信息
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 性别。0表示未定义，1表示男性，2表示女性
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string weixinid { get; set; }
        /// <summary>
        /// 头像url。注：如果要获取小图将url最后的"/0"改成"/64"即可
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 关注状态: 1=已关注，2=已冻结，4=未关注 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public string extattr { get; set; }
    }
    #endregion
}
