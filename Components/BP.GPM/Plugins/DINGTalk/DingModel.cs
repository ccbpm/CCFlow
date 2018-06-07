using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Collections;

namespace BP.EAI.Plugins.DINGTalk
{
    /// <summary>
    /// 获取部门列表
    /// </summary>
    public class DepartMent_List
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
        /// 部门列表数据。以部门的order字段从小到大排列
        /// </summary>
        public List<DepartMentDetailInfo> department { get; set; }
    }

    /// <summary>
    /// 部门列表明细
    /// </summary>
    public class DepartMentDetailInfo
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 父部门id，根部门为1
        /// </summary>
        public string parentid { get; set; }
        /// <summary>
        /// 是否同步创建一个关联此部门的企业群, true表示是, false表示不是
        /// </summary>
        public string createDeptGroup { get; set; }
        /// <summary>
        /// 当群已经创建后，是否有新人加入部门会自动加入该群, true表示是, false表示不是
        /// </summary>
        public string autoAddUser { get; set; }
    }

    /// <summary>
    /// 创建部门后消息
    /// </summary>
    public class CreateDepartMent_PostVal
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
        /// 创建的部门id
        /// </summary>
        public string id { get; set; }
    }

    /// <summary>
    /// 新增人员后消息
    /// </summary>
    public class CreateUser_PostVal
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
        /// 员工唯一标识userid
        /// </summary>
        public string userid { get; set; }
    }

    /// <summary>
    /// 部门人员列表
    /// </summary>
    public class DepartMentUser_List
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
        /// 在分页查询时返回，代表是否还有下一页更多数据
        /// </summary>
        public string hasMore { get; set; }
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<DepartMentUserInfo> userlist { get; set; }
    }

    /// <summary>
    /// 部门人员详细信息
    /// </summary>
    public class DepartMentUserInfo
    {
        /// <summary>
        /// 员工唯一标识ID（不可修改）
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 表示人员在此部门中的排序，列表是按order的倒序排列输出的，即从大到小排列输出的
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 表示人员在此部门中的排序，列表是按order的倒序排列输出的，即从大到小排列输出的
        /// </summary>
        public string orderInDepts { get; set; }
        /// <summary>
        /// 钉钉ID
        /// </summary>
        public string dingId { get; set; }
        /// <summary>
        /// 手机号（ISV不可见）
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 分机号（ISV不可见）
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 办公地点（ISV不可见）
        /// </summary>
        public string workPlace { get; set; }
        /// <summary>
        /// 备注（ISV不可见）
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 是否是企业的管理员, true表示是, false表示不是
        /// </summary>
        public bool isAdmin { get; set; }
        /// <summary>
        /// 是否为企业的老板, true表示是, false表示不是
        /// </summary>
        public bool isBoss { get; set; }
        /// <summary>
        /// 是否隐藏号码, true表示是, false表示不是
        /// </summary>
        public bool isHide { get; set; }
        /// <summary>
        /// 是否是部门的主管, true表示是, false表示不是
        /// </summary>
        public bool isLeader { get; set; }
        /// <summary>
        /// 表示该用户是否激活了钉钉
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// 成员所属部门id列表"department": [1, 2]
        /// </summary>
        public object department { get; set; }
        /// <summary>
        /// 职位信息
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 头像url"./dingtalk/abc.jpg"
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string jobnumber { get; set; }
        /// <summary>
        /// 扩展属性，可以设置多种属性(但手机上最多只能显示10个扩展属性，具体显示哪些属性
        /// 请到OA管理后台->设置->通讯录信息设置和OA管理后台->设置->手机端显示信息设置)
        /// extattr": {"爱好":"旅游","年龄":"24"}
        /// </summary>
        public object extattr { get; set; }
    }

    /// <summary>
    /// 钉钉一般请求返回结果
    /// </summary>
    public class Ding_Post_ReturnVal
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public string errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
    }

    #region 钉钉消息实体类
    public abstract class Ding_Msg
    {
        /// <summary>
        /// 发送的业务编号
        /// </summary>
        public long WorkID { get; set; }
        /// <summary>
        /// 钉钉访问许可
        /// </summary>
        public string Access_Token { get; set; }
        #region 发送普通会话消息
        /// <summary>
        /// 必须：是- 消息发送者员工ID
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 必须：是- 群消息或者个人聊天会话Id，(通过JSAPI之pickConversation接口唤起联系人界面选择之后即可拿到会话ID，之后您可以使用获取到的cid调用此接口）
        /// </summary>
        public string cid { get; set; }
        #endregion

        #region 发送企业会话消息
        /// <summary>
        /// 必须：否- 员工ID列表（消息接收者，多个接收者用’ | '分隔）。特殊情况：指定为@all，则向该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 必须：否- 部门id列表，多个接收者用’ | '分隔。当touser为@all时忽略本参数 touser或者toparty 二者有一个必填
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 必须：是- 企业应用id，这个值代表以哪个应用的名义发送消息
        /// </summary>
        public string agentid { get; set; }
        #endregion
    }
    /// <summary>
    /// 文本和超链接消息
    /// </summary>
    public class Ding_Msg_Text : Ding_Msg
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：text
        /// </summary>
        public string msgtype
        {
            get
            {
                return "text";
            }
        }

        /// <summary>
        /// 必须：是- 消息内容
        /// </summary>
        public string content { get; set; }
    }

    /// <summary>
    /// 超链接消息
    /// </summary>
    public class Ding_Msg_Link : Ding_Msg
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：link
        /// </summary>
        public string msgtype
        {
            get
            {
                return "link";
            }
        }

        /// <summary>
        /// 必须：是- 消息点击链接地址
        /// </summary>
        public string messageUrl { get; set; }

        /// <summary>
        /// 必须：是- 图片媒体文件id，可以调用上传媒体文件接口获取
        /// </summary>
        public string picUrl { get; set; }

        /// <summary>
        /// 必须：是- 消息标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 必须：是- 消息内容
        /// </summary>
        public string text { get; set; }
    }

    /// <summary>
    /// OA消息内容
    /// </summary>
    public class Ding_Msg_OA : Ding_Msg
    {
        /// <summary>
        /// 必须：是- 消息类型，此时固定为：oa
        /// </summary>
        public string msgtype
        {
            get
            {
                return "oa";
            }
        }

        /// <summary>
        /// 必须：是- 客户端点击消息时跳转到的H5地址
        /// </summary>
        public string messageUrl { get; set; }

        /// <summary>
        /// 必须：是- 消息头部的背景颜色。长度限制为8个英文字符，其中前2为表示透明度，后6位表示颜色值。不要添加0x
        /// </summary>
        public string head_bgcolor { get; set; }
        /// <summary>
        /// 必须：是- 消息的头部标题（仅适用于发送普通场景）
        /// </summary>
        public string head_text { get; set; }
        /// <summary>
        /// 必须：否- 消息体的标题
        /// </summary>
        public string body_title { get; set; }
        /// <summary>
        /// 必须：否- 消息体的表单，最多显示6个，超过会被隐藏
        /// </summary>
        public Hashtable body_form { get; set; }
        /// <summary>
        /// 必须：否- 消息体的内容，最多显示3行
        /// </summary>
        public string body_content { get; set; }
        /// <summary>
        /// 必须：否- 消息体中的图片media_id
        /// </summary>
        public string body_image { get; set; }
        /// <summary>
        /// 必须：否- 自定义的附件数目。此数字仅供显示，钉钉不作验证
        /// </summary>
        public string body_file_count { get; set; }
        /// <summary>
        /// 必须：否- 自定义的作者名字
        /// </summary>
        public string body_author { get; set; }
    }

    #endregion 钉钉消息 end
}
