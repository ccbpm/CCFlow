using System.IO;
using BP.DA;
using BP.En;

namespace BP.Port.WeiXin.DTS
{
    /// <summary>
    /// 微信人员头像同步
    /// </summary>
    public class DTSUserIcon : Method
    {
        /// <summary>
        /// 微信人员头像同步
        /// </summary>
        public DTSUserIcon()
        {
            this.Title = "微信人员头像同步到DataUser/Icon";
            this.Help = "本功能将微信企业号中所有人员的头像下载到本地，包括一张大图，一张小图";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.WF.Glo.IsEnable_WeiXin == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            BP.Port.WeiXin.WeiXinEntity weixin = new BP.Port.WeiXin.WeiXinEntity();
            string savePath =  BP.Difference.SystemConfig.PathOfDataUser + "UserIcon";

            //检查目录.
            if (Directory.Exists(savePath) == false)
                Directory.CreateDirectory(savePath);

            //获得部门信息.
            DeptList deptList = new DeptList();
            deptList.RetrieveAll(); //查询所有的部门信息.

            //遍历部门.
            foreach (DeptEntity deptMent in deptList.department)
            {
                //获得部门下的人员信息.
                UserList ul = new UserList(deptMent.id);
                if (ul.errcode != 0)
                    continue;

                //遍历用户信息.
                foreach (UserEntity userInfo in ul.userlist)
                {
                    if (userInfo.avatar == null)
                        continue;

                    //大图标
                    string headimgurl = userInfo.avatar;
                    string UserIcon = savePath + "/" + userInfo.userid + "Biger.png";
                    DataType.HttpDownloadFile(headimgurl, UserIcon);

                    //小图标
                    string iconSize = userInfo.avatar.Substring(headimgurl.LastIndexOf('/'));
                    if (iconSize == "/")
                        headimgurl = userInfo.avatar + "64";
                    else
                        headimgurl = userInfo.avatar.Substring(0, headimgurl.LastIndexOf('/')) + "64";
                    UserIcon = savePath + "/" + userInfo.userid + "Smaller.png";
                    DataType.HttpDownloadFile(headimgurl, UserIcon);
                }
            }
            return "执行成功.";
        }
    }
}
