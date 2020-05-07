using BP.En;
using BP.GPM.DTalk;
using BP.GPM.WeiXin;
using BP.Tools;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace BP.WF.DTS
{
    public class WXOrganization : Method
    {
        public WXOrganization()
        {
            this.Title = "同步微信企业号的通讯录 ";
            this.Help = "将微信企业号中的通讯录同步到本地的组织结构";
            this.Help = "同步完成之后，需要配置人员的角色信息和主部门的信息。";

            this.GroupName = "执行";
        }
        public override void Init()
        {
            throw new NotImplementedException();
        }
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        public override object Do()
        {
            #region 读取数据.
            //判断是否配置了企业号
            if (string.IsNullOrWhiteSpace(BP.Sys.SystemConfig.WX_CorpID))
                return "err@没有配置企业号相关信息";

            //获取 AccessToken
            string accessToken = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token=" + accessToken + "";

            //获取部门列表
            DeptMent_GetList DeptMentList = new DeptMent_GetList();
            HttpWebResponse response = new BP.GPM.DTalk.HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();
            DeptMentList = FormatToJson.ParseFromJson<DeptMent_GetList>(str);
            reader.Dispose();
            reader.Close();
            if (response != null)
                response.Close();

            if (DeptMentList == null)
                return "err@没有获得到数据.";

            if (DeptMentList.errcode == "0")
                return "err@获得数据期间出现错误.";
            #endregion 读取数据.

            #region 清楚现有的数据.
            //先删除所有部门
            GPM.Depts depts = new GPM.Depts();
            depts.ClearTable();

            //删除所有人员
            GPM.Emps emps = new GPM.Emps();
            emps.ClearTable();

            //删除部门人员表
            GPM.DeptEmps deptEmps = new GPM.DeptEmps();
            deptEmps.ClearTable();

            //删除部门人员岗位表
            GPM.DeptEmpStations deptEmpStations = new GPM.DeptEmpStations();
            deptEmpStations.ClearTable();
            #endregion 清楚现有的数据.

            #region 写入数据.
            GPM.DeptEmp deptEmp = new GPM.DeptEmp();
            GPM.Emp emp = new GPM.Emp();
            foreach (DeptMentInfo deptMent in DeptMentList.department)
            {
                //先插入部门表
                GPM.Dept dept = new GPM.Dept();
                dept.No = deptMent.id;
                dept.Name = deptMent.name;
                dept.ParentNo = deptMent.parentid;
                dept.Insert();

                //获取部门下的人员
                UsersBelongDept users = GetUserListByDeptID(deptMent.id);
                if (users != null && users.errcode == "0")
                {
                    foreach (UserInfoBelongDept userInfo in users.userlist)
                    {
                        //此处不能同步admin帐号的用户
                        if (userInfo.userid == "admin")
                            continue;

                        //如果有，放入部门人员表
                        if (emps.Retrieve(GPM.EmpAttr.No, userInfo.userid) > 0)
                        {
                            //插入部门人员表
                            deptEmp = new GPM.DeptEmp();
                            deptEmp.MyPK = deptMent.id + "_" + userInfo.userid;
                            deptEmp.FK_Emp = userInfo.userid;
                            deptEmp.FK_Dept = deptMent.id;
                            deptEmp.Insert();
                        }
                        //如果没有，默认主部门是当前第一个
                        else
                        {
                            //插入人员表
                            emp = new GPM.Emp();
                            emp.No = userInfo.userid;
                            emp.Name = userInfo.name;
                            emp.FK_Dept = deptMent.id;
                            emp.Email = userInfo.email;
                            emp.Tel = userInfo.mobile;
                            emp.Insert();

                            //插入部门人员表
                            deptEmp = new GPM.DeptEmp();
                            deptEmp.MyPK = deptMent.id + "_" + userInfo.userid;
                            deptEmp.FK_Emp = userInfo.userid;
                            deptEmp.FK_Dept = deptMent.id;
                            deptEmp.Insert();

                            //没有岗位，不同步，手动分配岗位吧
                            //GPM.DeptEmpStation deptEmpStation = new GPM.DeptEmpStation();
                            //deptEmpStation.MyPK = deptMent.id + "_" + userInfo.userid + "";
                        }
                    }
                }
            }
            #endregion 写入数据.

            #region 增加 admin.
            //不管以上有无人员，都添加admin帐号的信息
            //插入admin帐号
            emp = new GPM.Emp();
            emp.No = "admin";
            emp.Name = "admin";
            emp.FK_Dept = "1";//默认跟部门为1
            emp.Email = "";
            emp.Tel = "";
            emp.Insert();

            //部门人员表加入admin
            deptEmp = new GPM.DeptEmp();
            deptEmp.MyPK = "1_admin";
            deptEmp.FK_Emp = "admin";
            deptEmp.FK_Dept = "1";
            deptEmp.Insert();
            #endregion 增加admin.

            return "同步完成..";
        }
        /// <summary>
        /// 获取指定部门下的人员
        /// </summary>
        /// <param name="FK_Dept">部门编号</param>
        /// <returns></returns>
        public UsersBelongDept GetUserListByDeptID(string FK_Dept)
        {
            //获取token
            string access_token = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=" + access_token + "&department_id=" + FK_Dept + "&status=0";
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                UsersBelongDept users = FormatToJson.ParseFromJson<UsersBelongDept>(str);

                //人员集合
                if (users != null)
                    return users;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }
    }
}
