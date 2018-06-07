using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections;
using BP.Tools;
using BP.GPM;
using BP.Sys;
using BP.En;
using BP.EAI.Plugins.DINGTalk;
using BP.EAI.Plugins.DDSDK;
using BP.GPM.DTS;
using BP.DA;


namespace BP.EAI.Plugins
{
    /// <summary>
    /// 钉钉主类
    /// </summary>
    public class DingDing
    {
        private string corpid = BP.Sys.SystemConfig.Ding_CorpID;
        private string corpsecret = BP.Sys.SystemConfig.Ding_CorpSecret;

        public static string getAccessToken()
        {
            if (DataType.IsNullOrEmpty(AccessToken_Ding.Value) || AccessToken_Ding.Begin.AddSeconds(ConstVars.CACHE_TIME) < DateTime.Now)
                UpdateAccessToken(true);
            return AccessToken_Ding.Value;
        }

        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetUserID(string code)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/user/getuserinfo?access_token=" + access_token + "&code=" + code;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                CreateUser_PostVal user = new CreateUser_PostVal();
                user = FormatToJson.ParseFromJson<CreateUser_PostVal>(str);
                //BP.DA.Log.DefaultLogWriteLineError(access_token + "code:" + code + "1." + user.userid + "2." + user.errcode + "3." + user.errmsg);
                if (!DataType.IsNullOrEmpty(user.userid))
                    return user.userid;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
                return ex.Message;
            }
            return "";
        }

        #region 客户端开发

        /// <summary>  
        /// 获取JS票据  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static JSTicket FetchJSTicket()
        {
            SimpleCacheProvider cache = SimpleCacheProvider.GetInstance();
            JSTicket jsTicket = cache.GetCache<JSTicket>(ConstVars.CACHE_JS_TICKET_KEY);
            if (jsTicket == null || DataType.IsNullOrEmpty(jsTicket.ticket))
            {
                String apiurl = FormatApiUrlWithToken(Urls.get_jsapi_ticket);
                jsTicket = Analyze.Get<JSTicket>(apiurl);
                cache.SetCache(ConstVars.CACHE_JS_TICKET_KEY, jsTicket, ConstVars.CACHE_TIME);
            }
            return jsTicket;
        }

        /// <summary>  
        /// 获取签名包  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static SignPackage FetchSignPackage(String url)
        {
            JSTicket jsticket = FetchJSTicket();
            SignPackage signPackage = FetchSignPackage(url, jsticket);
            return signPackage;
        }

        /// <summary>  
        /// 获取签名包  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static SignPackage FetchSignPackage(String url, JSTicket jsticket)
        {
            string timestamp = SignPackageHelper.ConvertToUnixTimeStamp(DateTime.Now);
            string nonceStr = SignPackageHelper.CreateNonceStr();
            if (jsticket == null)
            {
                return null;
            }

            // 这里参数的顺序要按照 key 值 ASCII 码升序排序   
            string rawstring = Keys.jsapi_ticket + "=" + jsticket.ticket;
            rawstring += "&" + Keys.noncestr + "=" + nonceStr;
            rawstring += "&" + Keys.timestamp + "=" + timestamp;
            rawstring += "&" + Keys.url + "=" + url;
            string signature = SignPackageHelper.Sha1Hex(rawstring).ToLower();

            var signPackage = new SignPackage()
            {
                agentId = BP.Sys.SystemConfig.Ding_AgentID,
                corpId = BP.Sys.SystemConfig.Ding_CorpID,
                timeStamp = timestamp,
                nonceStr = nonceStr,
                signature = signature,
                url = url,
                rawstring = rawstring,
                jsticket = jsticket.ticket
            };
            return signPackage;
        }

        /// <summary>  
        ///更新票据  
        /// </summary>  
        /// <param name="forced">true:强制更新.false:按缓存是否到期来更新</param>  
        public static void UpdateAccessToken(bool forced = false)
        {
            if (!forced && AccessToken_Ding.Begin.AddSeconds(ConstVars.CACHE_TIME) >= DateTime.Now)
            {
                //没有强制更新，并且没有超过缓存时间  
                return;
            }
            string CorpID = BP.Sys.SystemConfig.Ding_CorpID;
            string CorpSecret = BP.Sys.SystemConfig.Ding_CorpSecret;
            string TokenUrl = Urls.gettoken;
            string apiurl = TokenUrl + "?" + Keys.corpid + "=" + CorpID + "&" + Keys.corpsecret + "=" + CorpSecret;
            TokenResult tokenResult = Analyze.Get<TokenResult>(apiurl);
            if (tokenResult.ErrCode == ErrCodeEnum.OK)
            {
                AccessToken_Ding.Value = tokenResult.Access_token;
                AccessToken_Ding.Begin = DateTime.Now;
            }
        }

        /// <summary>
        /// 获取拼接Token的url
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static string FormatApiUrlWithToken(string apiUrl)
        {
            //为空或超时
            return apiUrl + "?access_token=" + getAccessToken();
        }
        #endregion

        /// <summary>
        /// 下载多媒体文件
        /// </summary>
        /// <param name="mediaId">多媒体编号</param>
        /// <param name="workID">业务编号</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        public static bool DownLoadMediaById(string mediaId, string workID, string savePath)
        {
            string apiurl = FormatApiUrlWithToken(Urls.get_media) + "&media_id=" + mediaId;
            return Analyze.HttpDownLoadFile(apiurl, savePath);
        }
        /// <summary>
        /// 下载钉钉所有头像
        /// </summary>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public bool DownLoadUserIcon(string savePath)
        {
            if (Directory.Exists(savePath) == false)
                Directory.CreateDirectory(savePath);

            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/list?access_token=" + access_token;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                DepartMent_List departMentList = FormatToJson.ParseFromJson<DepartMent_List>(str);
                //部门集合
                if (departMentList != null && departMentList.department != null && departMentList.department.Count > 0)
                {
                    //部门信息
                    foreach (DepartMentDetailInfo deptMentInfo in departMentList.department)
                    {
                        //部门人员
                        DepartMentUser_List userList = GenerDeptUser_List(access_token, deptMentInfo.id);
                        if (userList != null)
                        {
                            foreach (DepartMentUserInfo userInfo in userList.userlist)
                            {
                                if (DataType.IsNullOrEmpty(userInfo.avatar))
                                {
                                    //大图标
                                    string UserIcon = savePath + "\\" + userInfo.userid + "Biger.png";
                                    File.Copy(savePath + "\\DefaultBiger.png", UserIcon, true);

                                    //小图标
                                    UserIcon = savePath + "\\" + userInfo.userid + "Smaller.png";
                                    File.Copy(savePath + "\\DefaultSmaller.png", UserIcon, true);

                                    //正常图标
                                    UserIcon = savePath + "\\" + userInfo.userid + ".png";
                                    File.Copy(savePath + "\\Default.png", UserIcon, true);
                                }
                                else
                                {
                                    //大图标
                                    string headimgurl = userInfo.avatar;
                                    string UserIcon = savePath + "\\" + userInfo.userid + "Biger.png";
                                    BP.DA.DataType.HttpDownloadFile(headimgurl, UserIcon);

                                    //小图标
                                    UserIcon = savePath + "\\" + userInfo.userid + "Smaller.png";
                                    BP.DA.DataType.HttpDownloadFile(headimgurl, UserIcon);

                                    //正常图标
                                    UserIcon = savePath + "\\" + userInfo.userid + ".png";
                                    BP.DA.DataType.HttpDownloadFile(headimgurl, UserIcon);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /// <summary>
        /// 同步钉钉通讯录到CCGPM
        /// </summary>
        /// <returns></returns>
        public bool AnsyOrgToCCGPM()
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/list?access_token=" + access_token;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                DepartMent_List departMentList = FormatToJson.ParseFromJson<DepartMent_List>(str);
                //部门集合
                if (departMentList != null && departMentList.department != null && departMentList.department.Count > 0)
                {
                    //删除旧数据
                    ClearOrg_Old();
                    //获取根部门
                    DepartMentDetailInfo rootDepartMent = new DepartMentDetailInfo();
                    foreach (DepartMentDetailInfo deptMenInfo in departMentList.department)
                    {
                        if (deptMenInfo.id == "1")
                        {
                            rootDepartMent = deptMenInfo;
                            break;
                        }
                    }
                    //增加跟部门
                    int deptIdx = 0;
                    Dept rootDept = new Dept();
                    rootDept.No = rootDepartMent.id;
                    rootDept.Name = rootDepartMent.name;
                    rootDept.ParentNo = "0";
                    rootDept.Idx = deptIdx;
                    rootDept.DirectInsert();


                    //部门信息
                    foreach (DepartMentDetailInfo deptMentInfo in departMentList.department)
                    {
                        //增加部门,排除根目录
                        if (deptMentInfo.id != "1")
                        {
                            Dept dept = new Dept();
                            dept.No = deptMentInfo.id;
                            dept.Name = deptMentInfo.name;
                            dept.ParentNo = deptMentInfo.parentid;
                            dept.Idx = deptIdx++;
                            dept.DirectInsert();
                        }

                        //部门人员
                        DepartMentUser_List userList = GenerDeptUser_List(access_token, deptMentInfo.id);
                        if (userList != null)
                        {
                            foreach (DepartMentUserInfo userInfo in userList.userlist)
                            {
                                Emp emp = new Emp();
                                DeptEmp deptEmp = new DeptEmp();
                                //如果账户存在则人员信息不添加，添加关联表
                                if (emp.IsExit(EmpAttr.No, userInfo.userid) == true)
                                {
                                    deptEmp.MyPK = deptMentInfo.id + "_" + emp.No;
                                    deptEmp.FK_Dept = deptMentInfo.id;
                                    deptEmp.FK_Emp = emp.No;
                                    deptEmp.DirectInsert();
                                    continue;
                                }

                                //增加人员
                                emp.No = userInfo.userid;
                                emp.EmpNo = userInfo.jobnumber;
                                emp.Name = userInfo.name;
                                emp.FK_Dept = deptMentInfo.id;
                                emp.Tel = userInfo.mobile;
                                emp.Email = userInfo.email;
                                //emp.Idx = DataType.IsNullOrEmpty(userInfo.order) == true ? 0 : Int32.Parse(userInfo.order);
                                emp.DirectInsert();

                                //增加人员与部门对应表
                                deptEmp.MyPK = deptMentInfo.id + "_" + emp.No;
                                deptEmp.FK_Dept = deptMentInfo.id;
                                deptEmp.FK_Emp = emp.No;
                                deptEmp.DirectInsert();
                            }
                        }
                    }

                    #region 处理部门名称全程
                    //OrgInit_NameOfPath nameOfPath = new OrgInit_NameOfPath();
                    //if (nameOfPath.IsCanDo)
                    //    nameOfPath.Do();
                    #endregion

                    return true;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 增量同步组织结构
        /// </summary>
        /// <returns></returns>
        public string AnsyIncrementOrgToGPM()
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/list?access_token=" + access_token;
            try
            {
                StringBuilder append = new StringBuilder();
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                DepartMent_List departMentList = FormatToJson.ParseFromJson<DepartMent_List>(str);
                if (departMentList == null || departMentList.department == null || departMentList.department.Count == 0)
                    return "钉钉获取部门出错。";

                #region 获取钉钉组织结构，进行更新与新增
                //增加跟部门
                int deptIdx = 0;
                bool doSomeThing = false;
                //部门信息
                foreach (DepartMentDetailInfo deptMentInfo in departMentList.department)
                {
                    deptIdx++;
                    doSomeThing = false;
                    //增加部门,排除根目录
                    if (deptMentInfo.id != "1")
                    {
                        Dept dept = new Dept();
                        if (dept.IsExit(DeptAttr.No, deptMentInfo.id) == true)
                        {
                            if (dept.No == deptMentInfo.id && !dept.Name.Equals(deptMentInfo.name))
                            {
                                doSomeThing = true;
                                append.Append("\r\n部门名称发生变化：" + dept.Name + " --> " + deptMentInfo.name);
                            }
                            if (!dept.ParentNo.Equals(deptMentInfo.parentid))
                            {
                                doSomeThing = true;
                                append.Append("\r\n部门父级发生变化：" + dept.ParentNo + " --> " + deptMentInfo.parentid);
                            }
                            //有变化，更新
                            if (doSomeThing == true)
                            {
                                dept.No = deptMentInfo.id;
                                dept.Name = deptMentInfo.name;
                                dept.ParentNo = deptMentInfo.parentid;
                                dept.DirectUpdate();
                            }
                        }
                        else
                        {
                            //不存在则新增
                            dept.No = deptMentInfo.id;
                            dept.Name = deptMentInfo.name;
                            dept.ParentNo = deptMentInfo.parentid;
                            dept.Idx = deptIdx;
                            dept.DirectInsert();
                            append.Append("\r\n新增部门：" + deptMentInfo.id + " - " + deptMentInfo.name);
                        }
                    }
                    //部门人员
                    DepartMentUser_List userList = GenerDeptUser_List(access_token, deptMentInfo.id);
                    if (userList != null)
                    {
                        foreach (DepartMentUserInfo userInfo in userList.userlist)
                        {
                            Emp emp = new Emp();
                            emp.No = userInfo.userid;

                            DeptEmp deptEmp = new DeptEmp();
                            //如果账户存在则人员信息不添加，添加关联表
                            if (emp.RetrieveFromDBSources() > 0)
                            {
                                if (!emp.Name.Equals(userInfo.name))
                                {
                                    emp.Name = userInfo.name;
                                    emp.DirectUpdate();
                                    append.Append("\r\n人员名称发生变化：" + emp.Name + " --> " + userInfo.name);
                                }

                                deptEmp.MyPK = deptMentInfo.id + "_" + emp.No;
                                if (deptEmp.RetrieveFromDBSources() > 0)
                                    continue;

                                //增加人员归属部门
                                deptEmp.FK_Dept = deptMentInfo.id;
                                deptEmp.FK_Emp = emp.No;
                                deptEmp.DirectInsert();
                                append.Append("\r\n增加人员归属部门：" + emp.Name + " - " + deptMentInfo.name);
                                continue;
                            }

                            //增加人员
                            emp.No = userInfo.userid;
                            emp.EmpNo = userInfo.jobnumber;
                            emp.Name = userInfo.name;
                            emp.FK_Dept = deptMentInfo.id;
                            emp.Tel = userInfo.mobile;
                            emp.Email = userInfo.email;
                            //emp.Idx = DataType.IsNullOrEmpty(userInfo.order) == true ? 0 : Int32.Parse(userInfo.order);
                            emp.DirectInsert();
                            append.Append("\r\n增加人员：" + emp.Name + "  所属部门:" + deptMentInfo.name);

                            //增加人员与部门对应表
                            deptEmp.MyPK = deptMentInfo.id + "_" + emp.No;
                            deptEmp.FK_Dept = deptMentInfo.id;
                            deptEmp.FK_Emp = emp.No;
                            deptEmp.DirectInsert();
                        }
                    }
                }
                #endregion

                #region GPM组织结构，在钉钉不存在进行删除部门与人员关系表，人员表不进行删除,删除业务人员表WF_Emp
                Depts gpm_Depts = new Depts();
                gpm_Depts.RetrieveAllFromDBSource();
                foreach (Dept gpm_Dept in gpm_Depts)
                {
                    bool isHave = false;
                    foreach (DepartMentDetailInfo ding_Dept in departMentList.department)
                    {
                        if (gpm_Dept.No.Equals(ding_Dept.id))
                        {
                            isHave = true;
                            break;
                        }
                    }

                    //部门在钉钉不存在则进行删除：部门表、部门人员、部门人员岗位、部门职位、部门岗位
                    if (isHave == false)
                    {
                        //部门岗位
                        DeptStation deptStation = new DeptStation();
                        int iDeptStation = deptStation.Delete(DeptStationAttr.FK_Dept, gpm_Dept.No);
                        //部门职位
                        DeptDuty deptDuty = new DeptDuty();
                        int iDeptDuty = deptDuty.Delete(DeptDutyAttr.FK_Dept, gpm_Dept.No);
                        //部门人员岗位
                        DeptEmpStation deptEmpStation = new DeptEmpStation();
                        int iDeptEmpStation = deptEmpStation.Delete(DeptEmpStationAttr.FK_Dept, gpm_Dept.No);
                        //部门人员
                        DeptEmp deptEmp = new DeptEmp();
                        int iDeptEmp = deptEmp.Delete(DeptEmpAttr.FK_Dept, gpm_Dept.No);
                        //部门表
                        Dept dt = new Dept(gpm_Dept.No);
                        dt.Delete();
                        append.Append("\r\n删除部门：" + gpm_Dept.Name + " 部门全路径：" + gpm_Dept.NameOfPath);
                        append.Append("\r\n        部门岗位 " + iDeptStation + " 条记录");
                        append.Append("\r\n        部门职位 " + iDeptDuty + " 条记录");
                        append.Append("\r\n        部门人员岗位 " + iDeptEmpStation + " 条记录");
                        append.Append("\r\n        部门人员 " + iDeptEmp + " 条记录");
                    }
                    else
                    {
                        //组织结构人员
                        DeptEmps deptEmps = new DeptEmps();
                        QueryObject obj = new QueryObject(deptEmps);
                        obj.AddWhere(DeptEmpAttr.FK_Dept, gpm_Dept.No);
                        obj.addAnd();
                        obj.AddWhereNotIn(DeptEmpAttr.FK_Emp, "'admin'");
                        obj.DoQuery();

                        //部门下没有人员不需要处理
                        if (deptEmps == null || deptEmps.Count == 0)
                            continue;

                        //钉钉部门人员
                        DepartMentUser_List userList = GenerDeptUser_List(access_token, gpm_Dept.No);
                        //部门下没有人员，清除部门下的所有人员
                        if (userList == null || userList.userlist.Count == 0)
                        {
                            append.Append("\r\n删除部门下的人员，部门：" + gpm_Dept.Name + " 部门全路径：" + gpm_Dept.NameOfPath);
                            foreach (DeptEmp dt in deptEmps)
                            {
                                dt.Delete();
                                Emp ep = new Emp();
                                ep.No = dt.FK_Emp;
                                ep.RetrieveFromDBSources();
                                append.Append("\r\n        人员编号：" + dt.FK_Emp + " 姓名:" + ep.Name);
                            }
                            continue;
                        }

                        //判断部门下的人员是否存在
                        foreach (DeptEmp deptEmp in deptEmps)
                        {
                            isHave = false;
                            foreach (DepartMentUserInfo userInfo in userList.userlist)
                            {
                                if (deptEmp.FK_Emp.Equals(userInfo.userid))
                                {
                                    isHave = true;
                                    break;
                                }
                            }

                            //不存在，删除
                            if (isHave == false)
                            {
                                deptEmp.Delete();
                                Emp ep = new Emp();
                                ep.No = deptEmp.FK_Emp;
                                ep.RetrieveFromDBSources();
                                append.Append("\r\n删除部门下的人员，部门：" + gpm_Dept.Name + " 部门全路径：" + gpm_Dept.NameOfPath);
                                append.Append("\r\n        人员编号：" + deptEmp.FK_Emp + " 姓名：" + ep.Name);
                            }
                        }
                    }
                }
                //删除没包含在部门的人员

                #endregion

                #region 处理部门名称全程
                //OrgInit_NameOfPath nameOfPath = new OrgInit_NameOfPath();
                //if (nameOfPath.IsCanDo)
                //    nameOfPath.Do();
                #endregion
                return append.ToString();
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 获取部门下的人员
        /// </summary>
        /// <returns></returns>
        private DepartMentUser_List GenerDeptUser_List(string access_token, string department_id)
        {
            string url = "https://oapi.dingtalk.com/user/list?access_token=" + access_token + "&department_id=" + department_id;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                DepartMentUser_List departMentUserList = FormatToJson.ParseFromJson<DepartMentUser_List>(str);

                //部门人员集合
                if (departMentUserList != null && departMentUserList.userlist != null && departMentUserList.userlist.Count > 0)
                    return departMentUserList;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        #region 组织结构同步
        /// <summary>
        /// 钉钉，新增部门同步钉钉
        /// </summary>
        /// <param name="dept">部门基本信息</param>
        /// <returns></returns>
        public CreateDepartMent_PostVal GPM_Ding_CreateDept(Dept dept)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/create?access_token=" + access_token;
            try
            {
                IDictionary<string, object> list = new Dictionary<string, object>();
                list.Add("name", dept.Name);
                list.Add("parentid", dept.ParentNo);
                //list.Add("order", "1");
                list.Add("createDeptGroup", "true");

                string str = BP.Tools.FormatToJson.ToJson_FromDictionary(list);
                str = new HttpWebResponseUtility().HttpResponsePost_Json(url, str);
                CreateDepartMent_PostVal postVal = FormatToJson.ParseFromJson<CreateDepartMent_PostVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    if (postVal.errcode != "0")
                        BP.DA.Log.DefaultLogWriteLineError("钉钉新增部门失败：" + postVal.errcode + "-" + postVal.errmsg);

                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 钉钉，编辑部门同步钉钉
        /// </summary>
        /// <param name="dept">部门基本信息</param>
        /// <returns></returns>
        public Ding_Post_ReturnVal GPM_Ding_EditDept(Dept dept)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/update?access_token=" + access_token;
            try
            {
                IDictionary<string, object> list = new Dictionary<string, object>();
                list.Add("id", dept.No);
                list.Add("name", dept.Name);
                //根目录不允许修改
                if (dept.No != "1")
                {
                    list.Add("parentid", dept.ParentNo);
                }
                //大于零才可以
                if (dept.Idx > 0)
                {
                    list.Add("order", dept.Idx);
                }
                string str = BP.Tools.FormatToJson.ToJson_FromDictionary(list);
                str = new HttpWebResponseUtility().HttpResponsePost_Json(url, str);
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    if (postVal.errcode != "0")
                        BP.DA.Log.DefaultLogWriteLineError("钉钉修改部门失败：" + postVal.errcode + "-" + postVal.errmsg);

                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 钉钉，删除部门同步钉钉
        /// </summary>
        /// <param name="deptId">部门编号</param>
        /// <returns></returns>
        public Ding_Post_ReturnVal GPM_Ding_DeleteDept(string deptId)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/department/delete?access_token=" + access_token + "&id=" + deptId;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    if (postVal.errcode != "0")
                        BP.DA.Log.DefaultLogWriteLineError("钉钉删除部门失败：" + postVal.errcode + "-" + postVal.errmsg);

                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 钉钉，新增人员同步钉钉
        /// </summary>
        /// <param name="emp">部门基本信息</param>
        /// <returns></returns>
        public CreateUser_PostVal GPM_Ding_CreateEmp(Emp emp)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/user/create?access_token=" + access_token;
            try
            {
                IDictionary<string, object> list = new Dictionary<string, object>();
                //如果用户编号存在则按照此账号进行新建
                if (!(DataType.IsNullOrEmpty(emp.No) || string.IsNullOrWhiteSpace(emp.No)))
                {
                    list.Add("userid", emp.No);
                }
                list.Add("name", emp.Name);
                //部门数组
                List<string> listArrary = new List<string>();
                listArrary.Add(emp.FK_Dept);

                list.Add("department", listArrary);
                list.Add("mobile", emp.Tel);
                list.Add("email", emp.Email);
                list.Add("jobnumber", emp.EmpNo);
                list.Add("position", emp.FK_DutyText);

                string str = BP.Tools.FormatToJson.ToJson_FromDictionary(list);
                str = new HttpWebResponseUtility().HttpResponsePost_Json(url, str);
                CreateUser_PostVal postVal = FormatToJson.ParseFromJson<CreateUser_PostVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    if (postVal.errcode != "0")
                    {
                        //在钉钉通讯录已经存在
                        if (postVal.errcode == "60102") postVal.userid = emp.No;
                        BP.DA.Log.DefaultLogWriteLineError("钉钉新增人员失败：" + postVal.errcode + "-" + postVal.errmsg);
                    }
                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 钉钉，编辑人员同步钉钉
        /// </summary>
        /// <param name="emp">部门基本信息</param>
        /// <returns></returns>
        public Ding_Post_ReturnVal GPM_Ding_EditEmp(Emp emp, List<string> deptIds = null)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/user/update?access_token=" + access_token;
            try
            {
                IDictionary<string, object> list = new Dictionary<string, object>();
                list.Add("userid", emp.No);
                list.Add("name", emp.Name);
                list.Add("email", emp.Email);
                list.Add("jobnumber", emp.EmpNo);
                list.Add("mobile", emp.Tel);
                list.Add("position", emp.FK_DutyText);
                //钉钉根据此从其他部门删除或增加到其他部门
                if (deptIds != null && deptIds.Count > 0)
                {
                    list.Add("department", deptIds);
                }
                string str = BP.Tools.FormatToJson.ToJson_FromDictionary(list);
                str = new HttpWebResponseUtility().HttpResponsePost_Json(url, str);
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    bool create_Ding_user = false;
                    //40022企业中的手机号码和登陆钉钉的手机号码不一致,暂时不支持修改用户信息,可以删除后重新添加
                    if (postVal.errcode == "40022" || postVal.errcode == "40021")
                    {
                        create_Ding_user = true;
                        postVal = GPM_Ding_DeleteEmp(emp.No);
                        //删除失败
                        if (postVal.errcode != "0")
                            create_Ding_user = false;
                    }
                    else if (postVal.errcode == "60121")//60121找不到该用户
                    {
                        create_Ding_user = true;
                    }

                    //需要新增人员
                    if (create_Ding_user == true)
                    {
                        CreateUser_PostVal postUserVal = GPM_Ding_CreateEmp(emp);
                        //消息传递
                        postVal.errcode = postUserVal.errcode;
                        postVal.errmsg = postUserVal.errmsg;
                    }

                    if (postVal.errcode != "0")
                    {
                        BP.DA.Log.DefaultLogWriteLineError("钉钉修改人员失败：" + postVal.errcode + "-" + postVal.errmsg);
                    }
                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 钉钉，删除人员同步钉钉
        /// </summary>
        /// <param name="userid">人员编号</param>
        /// <returns></returns>
        public Ding_Post_ReturnVal GPM_Ding_DeleteEmp(string userid)
        {
            string access_token = getAccessToken();
            string url = "https://oapi.dingtalk.com/user/delete?access_token=" + access_token + "&userid=" + userid;
            try
            {
                string str = new HttpWebResponseUtility().HttpResponseGet(url);
                Ding_Post_ReturnVal postVal = FormatToJson.ParseFromJson<Ding_Post_ReturnVal>(str);

                //请求返回信息
                if (postVal != null)
                {
                    if (postVal.errcode != "0")
                        BP.DA.Log.DefaultLogWriteLineError("钉钉删除人员失败：" + postVal.errcode + "-" + postVal.errmsg);

                    return postVal;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 清空组织结构
        /// </summary>
        private void ClearOrg_Old()
        {
            //人员
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Emp");
            //部门
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Dept");
            //部门人员
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmp");
            //部门人员岗位
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation");
            //admin 是必须存在的
            Emp emp = new Emp();
            emp.No = "admin";
            emp.Pass = "pub";
            emp.Name = "管理员";
            emp.FK_Dept = "1";
            emp.DirectInsert();
            //部门人员关联表
            DeptEmp deptEmp = new DeptEmp();
            deptEmp.FK_Dept = "1";
            deptEmp.FK_Emp = "admin";
            deptEmp.DutyLevel = 0;
            deptEmp.DirectInsert();
        }
    }
}
