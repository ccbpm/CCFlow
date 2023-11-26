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
        public string extattr { get; set; }
        #endregion 属性.


        /// <summary>
        /// 获取指定部门下  指定手机号的人员
        /// </summary>
        /// <param name="deptID">部门编号</param>
        /// <param name="tel">手机号</param>
        /// <returns></returns>
        public UserEntity(string deptID, string tel = null)
        {
            string access_token = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token= " + access_token + "&department_id=" + deptID + "&status=0";

            //读取数据.
            string str = DataType.ReadURLContext(url);
            //获得用户列表.
            UserList users = FormatToJson.ParseFromJson<UserList>(str);

            //从用户列表里找到tel的人员信息，并返回.
            foreach (UserEntity user in users.userlist)
            {
                if (user.mobile.Equals(tel))
                {
                    this.userid = user.userid;
                    this.name = user.name;
                    this.department = user.department;
                    this.position = user.position;
                    this.mobile = user.mobile;
                    this.gender = user.gender;
                    this.email = user.email;
                    this.weixinid = user.weixinid;
                    this.avatar = user.avatar;
                    this.status = user.status;
                    this.extattr = user.extattr;
                }
            }

            throw new Exception("err@该部门下查无此人.");
        }
    }
    /// <summary>
    /// 简写的User
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
}
