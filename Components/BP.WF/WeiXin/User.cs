using System;
using System.Collections.Generic;
using System.Text;
using BP.DA;
using BP.Tools;

namespace BP.GPM.WeiXin
{
    /// <summary>
    /// 部门下的人员
    /// </summary>
    public class UserList
    {
        #region 属性.
        /// <summary>
        /// 返回码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<UserEntity> userlist { get; set; }
        #endregion 属性.

        /// <summary>
        /// 初始化部门信息
        /// </summary>
        public UserList()
        {
        }
        /// <summary>
        /// 部门ID
        /// </summary>
        /// <param name="deptID"></param>
        public UserList(string deptID)
        {
            //获取token.
            string access_token = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=" + access_token + "&department_id=" + deptID + "&status=0";

            //获得信息.
            string str = DataType.ReadURLContext(url);
            UserList userList = BP.Tools.FormatToJson.ParseFromJson<UserList>(str);

            //赋值.
            this.errcode = userList.errcode;
            this.errmsg = userList.errmsg;
            this.userlist = userList.userlist;
        }
    }
    /// <summary>
    /// 部门人员详情
    /// </summary>
    public class UserEntity
    {
        #region 属性.
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
        public extattr extattr { get; set; }
        #endregion 属性.


        
    }
    public class extattr
    { 
        public List<attrs> attrs { get; set; }
    }
    public class attrs
    { 
        public string name { get; set; }
        public string value { get; set; }
    }
    /// <summary>
    /// 简写的User
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
    public class UserEntityList
    {
        #region 属性.
        /// <summary>
        /// 返回码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 成员列表
        /// </summary>
        public List<UserEntity> userlist { get; set; }
        #endregion 属性.

        /// <summary>
        /// 初始化部门信息
        /// </summary>
        public UserEntityList()
        {
        }
        /// <summary>
        /// 部门ID
        /// </summary>
        /// <param name="deptID"></param>
        public UserEntityList(string deptID)
        {
            //获取token.
            string access_token = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=" + access_token + "&department_id=" + deptID + "&status=0";

            //获得信息.
            string str = DataType.ReadURLContext(url);
            UserList userList = BP.Tools.FormatToJson.ParseFromJson<UserList>(str);

            //赋值.
            this.errcode = userList.errcode;
            this.errmsg = userList.errmsg;
            this.userlist = userList.userlist;
        }
    }
}
