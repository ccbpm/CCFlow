using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.Web;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BP.Sys;
using System.Text.RegularExpressions;
using BP.En30.Utility;
using BP.En30.Utility.Web;
using System.Web.UI.WebControls;
using BP.En;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BP.Difference;

namespace BP.Cloud.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class App_Org : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public App_Org()
        {
        }
        public string CheckAuth()
        {
            try
            {
                string deptNo = this.GetRequestVal("deptNo");
                Dept dept = new Dept(deptNo);
                var authRes = BP.Tools.Json.ToJson(GetAuth(dept.OrgNo));

                return authRes;
            }
            catch (Exception ex)
            {
                return BP.Tools.Json.ToJson(new BaseResponse<bool>()
                {
                    code = ResponseCode.exception,
                    msg = ex.Message
                });
            }
        }
        public BaseResponse<string> GetAuth(string orgNo)
        {
            BaseResponse<string> res = new BaseResponse<string>();
            try
            {
                int count = DBAccess.RunSQLReturnValInt("select count(*) num from Port_OrgAdminer where fk_emp='" + WebUser.No + "' and orgno='" + orgNo + "'");
                if (count > 0)//是否本组织的管理员
                {
                    res.code = ResponseCode.success;
                }
                else
                {
                    res.code = ResponseCode.fail;
                    res.msg = "没有操作权限";
                }
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.Message);

                res.code = ResponseCode.exception;
                res.msg = ex.Message;
            }

            return res;
        }
        public string DeleteDeptByNo()
        {
            BaseResponse<string> res = new BaseResponse<string>();
            try
            {
                string deptNo = this.GetRequestVal("deptNo");
                Dept dept = new Dept(deptNo);

                var auth = GetAuth(dept.OrgNo);
                if (auth.code == ResponseCode.success)
                {
                    var deptRes = BP.Cloud.Dev2Interface.CheckDeptIsUsed(deptNo);

                    if (deptRes.code != ResponseCode.success)
                    {
                        res.code = deptRes.code;
                        res.msg = deptRes.msg;
                    }
                    else
                    {
                        DBAccess.RunSQL("delete from port_dept where no='" + deptNo + "'");

                        res.msg = "操作成功";
                        res.code = ResponseCode.success;
                    }
                }
                else
                {
                    res.code = ResponseCode.fail;
                    res.msg = "没有操作权限";
                }
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.Message);

                res.msg = ex.Message;
                res.code = ResponseCode.exception;

            }
            return BP.Tools.Json.ToJson(res);
        }

        public string RemoveDeptEmp()
        {
            string userID = this.GetRequestVal("userID");
            string deptNo = this.GetRequestVal("deptNo");

            Dept dept = new Dept(deptNo);
            var auth = GetAuth(dept.OrgNo);
            if (auth.code != ResponseCode.success)
                return "err@" + auth.msg;

            string empNo = this.GetRequestVal("empNo");
            BP.Cloud.Emp emp = new BP.Cloud.Emp(empNo);
            if (emp.FK_Dept == deptNo)
            {
                return "err@不可以直接移除主部门";
            }

            DBAccess.RunSQL("delete from Port_DeptEmp where mypk='" + deptNo + "_" + userID + "'");
            DBAccess.RunSQL("delete from Port_DeptEmpStation where FK_Dept='" + deptNo + "' and fk_emp='" + userID + "' and orgNo='" + emp.OrgNo + "'");


            return "成功移除子部门";
        }
        public string SetMainDept()
        {
            string deptNo = this.GetRequestVal("deptNo");
            Dept dept = new Dept(deptNo);
            var auth = GetAuth(dept.OrgNo);
            if (auth.code != ResponseCode.success)
                return "err@" + auth.msg;

            string empNo = this.GetRequestVal("empNo");
            string userID = this.GetRequestVal("userID");
            string empOrgNo = this.GetRequestVal("empOrgNo");

            DBAccess.RunSQL("update port_emp set fk_dept='" + deptNo + "' where no='" + empNo + "'");
            DBAccess.RunSQL("update Port_DeptEmp set ismaindept=0 where orgNo='" + empOrgNo + "' and fk_emp='" + userID + "'");
            DBAccess.RunSQL("update Port_DeptEmp set ismaindept=1 where mypk='" + deptNo + "_" + userID + "'");

            return "成功设置主部门";
        }
        public string DoEnable()
        {
            string userID = this.GetRequestVal("userID");
            string deptNo = this.GetRequestVal("deptNo");
            Dept dept = new Dept(deptNo);
            var authRes = new App_Org().GetAuth(dept.OrgNo);
            if (authRes.code != ResponseCode.success)
                throw new Exception(authRes.msg);


            DBAccess.RunSQL("update wf_emp set UseSta=1 where No='" + userID + "'");

            return "已启用";
        }

        public string DoUnEnable()
        {
            string deptNo = this.GetRequestVal("deptNo");
            Dept dept = new Dept(deptNo);
            var authRes = new App_Org().GetAuth(dept.OrgNo);
            if (authRes.code != ResponseCode.success)
                throw new Exception(authRes.msg);

            string userID = this.GetRequestVal("userID");
            if (userID.Equals("admin") == true)
                return "err@" + this.Name + "是管理员不能禁用.";

            string sql = "";
            //SAAS版本. 集团版 @hongyan 需要翻译.
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                sql = "SELECT FK_Emp FROM Port_OrgAdminer WHERE FK_Emp='" + userID + "' AND OrgNo='" + WebUser.OrgNo + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                    return "err@" + this.Name + "是管理员不能禁用.";
            }

            Paras ps = new Paras();
            ps.Add("No", userID);
            DBAccess.RunSQL("update wf_emp set UseSta=0 where No='" + userID + "'");

            return "已禁用";

        }
        public string Template_Save()
        {

            var files = HttpContextHelper.RequestFiles();
            string ext = ".xls";
            string fileName = System.IO.Path.GetFileName(files[0].FileName);
            if (fileName.Contains(".xlsx"))
                ext = ".xlsx";

            //设置文件名
            string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ext;

            //文件存放路径
            string filePath = SystemConfig.PathOfTemp + "\\" + fileNewName;
            HttpContextHelper.UploadFile(files[0], filePath);

            #region 获得数据源.
            var sheetNameList = BP.DA.DBLoad.GenerTableNames(filePath).ToList();
            if (sheetNameList.Count < 3 || sheetNameList.Contains("部门$") == false || sheetNameList.Contains("岗位$") == false || sheetNameList.Contains("人员$") == false)
                throw new Exception("excel不符合要求");

            //获得部门数据.
            DataTable dtDept = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("部门$"));

            //获得岗位数据.
            DataTable dtStation = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("岗位$"));

            //获得人员数据.
            DataTable dtEmp = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("人员$"));
            #endregion 获得数据源.

            #region 检查是否有根目录为 0 的数据?
            //检查数据的完整性.
            //1.检查是否有根目录为0的数据?
            var num = 0;
            bool isHave = false;
            foreach (DataRow dr in dtDept.Rows)
            {
                string str1 = dr[0] as string;
                if (DataType.IsNullOrEmpty(str1) == true)
                    continue;

                num++;
                string str = dr[1] as string;
                if (str == null || str.Equals(DBNull.Value))
                    return "err@导入出现数据错误:" + str1 + "的.上级部门名称-不能用空行的数据， 第[" + num + "]行数据.";

                if (str.Equals("0") == true)
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                return "err@导入数据没有找到部门根目录节点.";
            #endregion 检查是否有根目录为0的数据

            #region 检查部门名称是否重复?
            string deptStrs = "";
            foreach (DataRow dr in dtDept.Rows)
            {
                string deptName = dr[0] as string;
                if (DataType.IsNullOrEmpty(deptName) == true)
                    continue;

                if (deptStrs.Contains("," + deptName + ",") == true)
                    return "err@部门名称:" + deptName + "重复.";

                //加起来..
                deptStrs += "," + deptName + ",";
            }
            #endregion 检查部门名称是否重复?

            #region 检查人员帐号是否重复?
            string emps = "";
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr[0] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                if (emps.Contains("," + empNo + ",") == true)
                    return "err@人员帐号:" + empNo + "重复.";

                //加起来..
                emps += "," + empNo + ",";
            }
            #endregion 检查人员帐号是否重复?

            #region 检查岗位名称是否重复?
            string staStrs = "";
            foreach (DataRow dr in dtStation.Rows)
            {
                string staName = dr[0] as string;
                if (DataType.IsNullOrEmpty(staName) == true)
                    continue;

                if (staStrs.Contains("," + staName + ",") == true)
                    return "err@岗位名称:" + staName + "重复.";

                //加起来..
                staStrs += "," + staName + ",";
            }
            #endregion 检查岗位名称是否重复?

            #region 检查人员的部门名称是否存在于部门数据里?
            int idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string emp = dr[0] as string;
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;

                idx++;
                //去的部门编号.
                string strs = dr["部门名称"] as string;
                if (DataType.IsNullOrEmpty(strs) == true)
                    return "err@第[" + idx + "]行,人员[" + emp + "]部门不能为空:" + strs + ".";

                string[] mystrs = strs.Split(',');
                foreach (string str in mystrs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //先看看数据是否有?
                    Dept dept = new Dept();
                    if (dept.Retrieve("Name", str, "OrgNo", WebUser.OrgNo) == 1)
                        continue;

                    //从xls里面判断.
                    isHave = false;
                    foreach (DataRow drDept in dtDept.Rows)
                    {
                        if (str.Equals(drDept[0].ToString()) == true)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (isHave == false)
                        return "err@第[" + idx + "]行,人员[" + emp + "]部门名[" + str + "]，不存在模版里。";
                }
            }
            #endregion 检查人员的部门名称是否存在于部门数据里

            #region 检查人员的岗位名称是否存在于岗位数据里?
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string emp = dr[0] as string;
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;

                idx++;

                //岗位名称..
                string strs = dr["岗位名称"] as string;
                if (DataType.IsNullOrEmpty(strs) == true)
                    continue;

                // return "err@第[" + idx + "]行,人员[" + emp + "]岗位名称不能为空:" + strs + ".";

                //判断岗位.
                string[] mystrs = strs.Split(',');
                foreach (string str in mystrs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //先看看数据是否有?
                    Station stationEn = new Station();
                    if (stationEn.Retrieve("Name", str, "OrgNo", WebUser.OrgNo) == 1)
                        continue;

                    //从 xls 判断.
                    isHave = false;
                    foreach (DataRow drSta in dtStation.Rows)
                    {
                        if (str.Equals(drSta[0].ToString()) == true)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (isHave == false)
                        return "err@第[" + idx + "]行,人员[" + emp + "]岗位名称[" + str + "]，不存在模版里。";
                }
            }
            #endregion 检查人员的部门名称是否存在于部门数据里

            #region 检查部门负责人是否存在于人员列表里?
            string empStrs = ",";
            foreach (DataRow item in dtEmp.Rows)
            {
                empStrs += item[0].ToString() + ",";
            }
            idx = 0;
            foreach (DataRow dr in dtDept.Rows)
            {
                string empNo = dr[2] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                idx++;
                if (empStrs.Contains("," + empNo + ",") == false)
                    return "err@部门负责人[" + empNo + "]不存在与人员表里，第[" + idx + "]行.";
            }
            #endregion 检查部门负责人是否存在于人员列表里

            #region 检查直属领导帐号是否存在于人员列表里?
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr[6] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                idx++;
                if (empStrs.Contains("," + empNo + ",") == false)
                    return "err@部门负责人[" + empNo + "]不存在与人员表里，第[" + idx + "]行.";
            }
            #endregion 检查部门负责人是否存在于人员列表里


            #region 插入数据到 Port_StationType. 
            idx = -1;
            foreach (DataRow dr in dtStation.Rows)
            {
                idx++;
                string str = dr[1] as string;

                //判断是否是空.
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                if (str.Equals("岗位类型") == true)
                    continue;

                str = str.Trim();

                //看看数据库是否存在.
                StationType st = new StationType();
                if (st.IsExit("Name", str, "OrgNo", WebUser.OrgNo) == false)
                {
                    st.Name = str;
                    st.OrgNo = BP.Web.WebUser.OrgNo;
                    st.No = DBAccess.GenerGUID();
                    st.Insert();
                }
            }
            #endregion 插入数据到 Port_StationType. 

            #region 插入数据到 Port_Station. 
            idx = -1;
            foreach (DataRow dr in dtStation.Rows)
            {
                idx++;
                string str = dr[0] as string;

                //判断是否是空.
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                if (str.Equals("岗位名称") == true)
                    continue;


                //获得类型的外键的编号.
                string stationTypeName = dr[1].ToString().Trim();
                StationType st = new StationType();
                if (st.Retrieve("Name", stationTypeName, "OrgNo", WebUser.OrgNo) == 0)
                    return "err@系统出现错误,没有找到岗位类型[" + stationTypeName + "]的数据.";

                //看看数据库是否存在.
                Station sta = new Station();
                sta.Name = str;
                sta.Idx = idx;

                //不存在就插入.
                if (sta.IsExit("Name", str, "OrgNo", WebUser.OrgNo) == false)
                {
                    sta.OrgNo = BP.Web.WebUser.OrgNo;
                    sta.FK_StationType = st.No;
                    sta.No = DBAccess.GenerGUID();
                    sta.Insert();
                }
                else
                {
                    //存在就更新.
                    sta.FK_StationType = st.No;
                    sta.Update();
                }
            }
            #endregion 插入数据到 Port_Station. 

            #region 插入数据到 Port_Dept.
            idx = -1;
            foreach (DataRow dr in dtDept.Rows)
            {
                //获得部门名称.
                string deptName = dr[0] as string;
                if (deptName.Equals("部门名称") == true)
                    continue;

                string parentDeptName = dr[1] as string;
                string leader = dr[2] as string;

                //说明是根目录.
                if (parentDeptName.Equals("0") == true)
                {
                    Dept root = new Dept();
                    root.No = BP.Web.WebUser.OrgNo;
                    if (root.RetrieveFromDBSources() == 0)
                        return "err@没有找到，根目录节点，请联系管理员。";

                    root.Name = deptName;
                    root.Update();
                    continue;
                }


                //先求出来父节点.
                Dept parentDept = new Dept();
                int i = parentDept.Retrieve("Name", parentDeptName, "OrgNo", BP.Web.WebUser.OrgNo);
                if (i == 0)
                    return "err@没有找到当前部门[" + deptName + "]的上一级部门[" + parentDeptName + "]";

                Dept myDept = new Dept();

                //如果数据库存在.
                i = parentDept.Retrieve("Name", deptName, "OrgNo", BP.Web.WebUser.OrgNo);
                if (i >= 1)
                    continue;

                //插入部门.
                myDept.Name = deptName;
                myDept.OrgNo = BP.Web.WebUser.OrgNo;
                myDept.No = DBAccess.GenerGUID();
                myDept.ParentNo = parentDept.No;
                myDept.Leader = leader; //领导.
                myDept.Idx = idx;
                myDept.Insert();
            }
            #endregion 插入数据到 Port_Dept.

            #region 插入到 Port_Emp.
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr[0].ToString();
                string empName = dr[1].ToString();
                string deptNames = dr[2].ToString();
                string stationNames = dr[3].ToString();
                string tel = dr[4].ToString();
                string email = dr[5].ToString();
                string leader = dr[6].ToString(); //部门领导.

                Emp emp = new Emp();
                int i = emp.Retrieve("UserID", empNo, "OrgNo", BP.Web.WebUser.OrgNo);
                if (i >= 1)
                {
                    emp.Tel = tel;
                    emp.Name = empName;
                    emp.Email = email;
                    emp.Leader = leader;
                    emp.Update();
                    continue;
                }

                //找到人员的部门.
                string[] depts = deptNames.Split(',');
                Dept dept = new Dept();
                foreach (var deptName in depts)
                {
                    if (DataType.IsNullOrEmpty(deptName) == true)
                        continue;

                    i = dept.Retrieve("Name", deptName, "OrgNo", WebUser.OrgNo);
                    if (i <= 0)
                        return "err@部门名称不存在." + deptName;

                    DeptEmp de = new DeptEmp();
                    de.FK_Dept = dept.No;
                    de.FK_Emp = empNo;
                    de.OrgNo = WebUser.OrgNo;
                    de.MyPK = de.FK_Dept + "_" + de.FK_Emp;
                    de.Delete();
                    de.Insert();
                }

                //插入岗位.
                string[] staNames = stationNames.Split(',');
                Station sta = new Station();
                foreach (var staName in staNames)
                {
                    if (DataType.IsNullOrEmpty(staName) == true)
                        continue;

                    i = sta.Retrieve("Name", staName, "OrgNo", WebUser.OrgNo);
                    if (i == 0)
                        return "err@岗位名称不存在." + staName;

                    DeptEmpStation des = new DeptEmpStation();
                    des.FK_Dept = dept.No;
                    des.FK_Emp = empNo;
                    des.FK_Station = sta.No;
                    des.OrgNo = WebUser.OrgNo;
                    des.MyPK = des.FK_Dept + "_" + des.FK_Emp + "_" + des.FK_Station;
                    des.Delete();
                    des.Insert();
                }

                //插入到数据库.
                emp.No = BP.Web.WebUser.OrgNo + "_" + empNo;
                emp.UserID = empNo;
                emp.Name = empName;
                emp.FK_Dept = dept.No;
                emp.OrgNo = WebUser.OrgNo;
                emp.Tel = tel;
                emp.Email = email;
                emp.Leader = leader;
                emp.Idx = idx;

                emp.Insert();
            }
            #endregion 插入到 Port_Emp.


            //删除临时文件
            System.IO.File.Delete(filePath);

            return "执行完成.";
        }




        //httppost请求
        BP.WF.HttpWebResponseUtility httpWebResponseUtility = new BP.WF.HttpWebResponseUtility();
        // 上传模板到微信
        public string UploadToWeCom(string filePath, string token)
        {

            string url = "https://qyapi.weixin.qq.com/cgi-bin/service/media/upload?type=file&attachment_type=&provider_access_token=" + token;

            var boundary = DateTime.Now.Ticks.ToString("X");
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent(boundary))
            using (var contentByte = new ByteArrayContent(File.ReadAllBytes(filePath)))
            {
                // 移除客户端请求头
                client.DefaultRequestHeaders.Remove("Expect");
                client.DefaultRequestHeaders.Remove("Connection");

                content.Headers.Remove("Content-Type");
                content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

                FileInfo fileInfo = new FileInfo(filePath);
                var fileName = Path.GetFileName(filePath);
                content.Add(contentByte);
                contentByte.Headers.Remove("Content-Disposition");
                contentByte.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"media\";filename=\"{fileName}\"" + "");
                contentByte.Headers.Remove("Content-Type");
                contentByte.Headers.TryAddWithoutValidation("Content-Type", "application/octet-stream");


                var response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    BP.DA.Log.DebugWriteInfo(response.ToString());
                    JObject obj = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    return obj.Value<string>("media_id");
                }
                else
                {
                    BP.DA.Log.DebugWriteError($"上传失败：{response.StatusCode}");
                    return null;
                }
            }
        }

        public void RequestTranslate(string token, string media_id, string corp_id)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/service/contact/id_translate?provider_access_token=" + token;
            JObject obj = new JObject();
            JArray idList = new JArray();
            idList.Add(media_id);
            obj["auth_corpid"] = corp_id;
            obj["media_id_list"] = idList;

            string requestBody = obj.ToString();
            using (var client = new HttpClient())
            using (var content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                var response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    JObject res = JObject.Parse(responseContent);
                    string jobid = res.Value<string>("jobid");
                    BP.Cloud.Org org = new BP.Cloud.Org(WebUser.OrgNo);
                    org.AsyncTaskID = jobid;
                    org.Update();
                    BP.DA.Log.DebugWriteInfo("申请通讯录转义成功，id = " + jobid);
                }
                else
                {
                    BP.DA.Log.DebugWriteError($"请求失败，状态码：{response.StatusCode}");
                }
            }

        }

        public string QueryTranslateResult()
        {
            string provider_access_token = BP.Cloud.WeXinAPI.Glo.getProviderAccessToken();
            BP.Cloud.Org org = new BP.Cloud.Org(WebUser.OrgNo);
            string asyncTaskID = org.AsyncTaskID;
            string url = "https://qyapi.weixin.qq.com/cgi-bin/service/batch/getresult?provider_access_token=" + provider_access_token + "&jobid=" + asyncTaskID;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                var resultInfo = new JObject();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    JObject obj = JObject.Parse(responseContent);
                    return obj.ToString(); ;
                    
                }
                else
                {
                    BP.DA.Log.DebugWriteError($"请求失败，状态码：{response.StatusCode}");
                    return "err@请求失败";
                }
            }
        }

        public string UploadAndSyncOrg()
        {
            var file = HttpContext.Current.Request.Files[0];
            int fileLength = file.ContentLength;
            byte[] input = new byte[fileLength];
            Stream inputStream = file.InputStream;
            inputStream.Read(input, 0, fileLength);
            inputStream.Position = 0;
            StreamReader reader = new StreamReader(inputStream, System.Text.Encoding.UTF8);
            string fileContent = reader.ReadToEnd();
            string[] items = fileContent.Split(';');
            string failItems = "";

            BP.Cloud.Depts depts = new BP.Cloud.Depts();
            depts.RetrieveAll();

            foreach (string item in items)
            {
                if(item.Trim().Length == 0)
                {
                    continue;
                }
                string[] info = item.Split(',');
                if (info.Length != 4)
                {
                    failItems += item + "|";
                    continue;
                }
                string no = info[0].Trim();
                string user_id = info[1].Trim();
                string username = info[2].Trim();
                string deptName = info[3].Trim();

                BP.Cloud.Emp emp = new BP.Cloud.Emp(no);
                emp.Name = username;
                emp.Update();
                BP.Cloud.Dept targetDept = null;
                foreach (BP.Cloud.Dept dept in depts)
                {
                    if (dept.No == emp.FK_Dept)
                    {
                        targetDept = dept;
                        break;
                    }
                }
                if (targetDept == null || targetDept.Name.Equals(deptName))
                {
                    continue;
                }
                targetDept.Name = deptName;
                targetDept.Update();
            }
            JObject res = new JObject();
            if(failItems.Length > 0)
            {
                res["msg"] = "部分更新成功，以下列更新失败:" + failItems;
            }
            else
            {
                res["msg"] = "更新成功，请重新登录";

            }
            return res.ToString();
        }

        public string ApplyWeComExport()
        {
            string folderPath = @"C:\AppTemp";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = folderPath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_tanslate_file.txt";            
            BP.Cloud.Emps targetEmps = new BP.Cloud.Emps();
            BP.Cloud.Depts depts = new BP.Cloud.Depts();
            depts.RetrieveAll();
            targetEmps.Retrieve("OrgNo", WebUser.OrgNo);
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                foreach (BP.Cloud.Emp emp in targetEmps)
                {
                    BP.Cloud.Dept targetDept = null;
                    foreach (BP.Cloud.Dept dept in depts)
                    {
                        if (dept.No == emp.FK_Dept)
                        {
                            targetDept = dept;
                            break;
                        }
                    }
                    if (targetDept == null)
                    {
                        continue;
                    }
                    string reocrd = emp.No + ',' + emp.UserID + "," + "$userName=" + emp.UserID + "$,$departmentName=" + targetDept.RefID + "$;";
                    outputFile.WriteLine(reocrd);
                }
            }
            
            string provider_access_token = BP.Cloud.WeXinAPI.Glo.getProviderAccessToken();
            BP.Cloud.Org org = new BP.Cloud.Org(WebUser.OrgNo);
            string mediaID = UploadToWeCom(filePath, provider_access_token);
            JObject response = new JObject();
            if (mediaID == null)
            {
                response["msg"] = "请求导出失败，请重试";
                return response.ToString();
            }
            RequestTranslate(provider_access_token, mediaID, org.CorpID);
            response["msg"] = "提交成功";
            return response.ToString();
        }
        /// <summary>
        /// 微信初始化数据.
        /// </summary>
        /// <returns></returns>
        public string OrganizationWX_Init()
        {
            string urlx = this.GetRequestVal("url");
            if(urlx.Contains(","))
            {
                urlx = urlx.Split(',')[0];
            }
            /*return WeixinUtil.getWxConfig(request, url, jsapi);*/
            BP.Cloud.Org org = new Org(WebUser.OrgNo);

            #region 获取应用的jsapi_ticket
            //如果jsapi_ticket接近失效，要重新获取，更新.
            String jsapi = "";
            if (HttpContext.Current.Cache.Get("jsapi_expires_in") != null
                && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                Convert.ToDateTime(HttpContext.Current.Cache.Get("jsapi_expires_in"))) > 0)

                //如果失效了，就直接更新一下.
                jsapi = HttpContext.Current.Cache.Get("jsapi").ToString();
            else
                jsapi = getjssdk(org);

            #endregion

            #region JS-SDK使用权限签名算法 参与签名的参数有四个: noncestr（随机字符串）, jsapi_ticket(如何获取参考“获取企业jsapi_ticket”以及“获取应用的jsapi_ticket接口”), timestamp（时间戳）, url（当前网页的URL， 不包含#及其后面部分）

            String requestUrl = urlx;

            // String timestamp = DateTime.Now.ToString(); // 必填，生成签名的时间戳
            long timestamp = (long)Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            
            String nonceStr = BP.DA.DBAccess.GenerGUID();  // 必填，生成签名的随机串

            // 注意这里参数名必须全部小写，且必须有序
            String sign = "jsapi_ticket=" + jsapi + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + requestUrl;

            //sha1签名
            String signature = GetHash(sign);
            string corpId = org.CorpID;
            #endregion

            Hashtable ht = new Hashtable();
            ht.Add("corpId", BP.Cloud.WeXinAPI.Glo.CorpID);
            ht.Add("appId", corpId);
            ht.Add("timestamp", timestamp);
            ht.Add("nonceStr", nonceStr);
            ht.Add("signature", signature);
            ht.Add("agentid", org.AgentId);
            ht.Add("js_ticket", jsapi);
            ht.Add("url", urlx);

            return BP.Tools.Json.ToJson(ht);

        }
        /// <summary>
        /// 获取应用的jsapi_ticket
        /// </summary>
        /// <returns></returns>
        public String getjssdk(BP.Cloud.Org org)
        {
            //如果AccessToken接近失效，要重新获取，更新
            if (DataType.IsNullOrEmpty(org.AccessTokenExpiresIn) == false
                && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                Convert.ToDateTime(org.AccessTokenExpiresIn)) > 0)
            {
                //如果失效了，就直接更新一下.
                org.AccessToken = getAccessToken(org);//获取企业凭证,更新失效时间
            }

            string url = "https://qyapi.weixin.qq.com/cgi-bin/ticket/get?access_token=" + org.AccessToken + "&type=agent_config&debug=1";
            string res = httpWebResponseUtility.HttpResponseGet(url);
            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string ticket = (string)dd["ticket"];//生成签名所需的jsapi_ticket，最长为512字节
            string expires_in = (string)dd["expires_in"];//凭证的有效时间（秒）
            if (string.IsNullOrEmpty(ticket) == true)
                return "";

            HttpContext.Current.Cache.Insert("jsapi", ticket);
            DateTime ss = DateTime.Now.AddSeconds(double.Parse(expires_in));

            HttpContext.Current.Cache.Insert("jsapi_expires_in", ss.ToString("yyyy-MM-dd HH:mm:ss"));
            return ticket;
        }
        /// <summary>
        /// 获取企业凭证 第三方服务商在取得企业的永久授权码后，通过此接口可以获取到企业的access_token。
        ///获取后可通过通讯录、应用、消息等企业接口来运营这些应用。
        /// </summary>
        /// <returns></returns>
        public string getAccessToken(BP.Cloud.Org org)
        {
            //获取第三方应用凭证
            string suitAccessToken = BP.Cloud.WeXinAPI.Glo.getSuitAccessToken();

            // string permanentCode = CreateOrg();//获取永久授权码
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("auth_corpid", org.CorpID);//授权方corpid
            parameters.Add("permanent_code", org.PermanentCode);//永久授权码，通过get_permanent_code获取
            string accessTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/service/get_corp_token?suite_access_token=" + suitAccessToken;

            string res = httpWebResponseUtility.HttpResponsePost_Json(accessTokenUrl, JsonConvert.SerializeObject(parameters));

            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string accessToken = (string)dd["access_token"];//授权方（企业）access_token,最长为512字节
            string expires_in = (string)dd["expires_in"];
            DateTime ss = DateTime.Now.AddSeconds(double.Parse(expires_in));
            //更新accessToken到org表中
            //BP.Cloud.Org org = new BP.Cloud.Org(corpid);

            org.AccessToken = accessToken;
            org.AccessTokenExpiresIn = ss.ToString("yyyy-MM-dd HH:mm:ss");

            org.Update();
            return accessToken;
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String GetHash(String input)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[]
            UTF8Encoding enc = new UTF8Encoding();
            byte[] dataToHash = enc.GetBytes(input);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash.ToLower();
        }

        /// <summary>
        /// 主动发送应用消息：企业后台调用接口通过应用向指定成员发送单聊消息
        /// </summary>
        public void sendMessageQywx(string toUserIds, string sender, string title, string docs, string url)
        {
            //根据发送人ID取得组织信息
            BP.Cloud.Emp emp = new BP.Cloud.Emp();
            emp.No = sender;
            if (emp.RetrieveFromDBSources() == 0)
                return;
            string orgNo = emp.OrgNo;

            //根据orgNo取得AccessToken
            BP.Cloud.Org org = new BP.Cloud.Org();
            org.No = orgNo;
            if (emp.RetrieveFromDBSources() == 0)
                return;

            //如果AccessToken接近失效，要重新获取，更新
            string accessToken = "";
            if (DataType.IsNullOrEmpty(org.AccessTokenExpiresIn) == false
                && DateTime.Compare(Convert.ToDateTime(DateTime.Now),
                Convert.ToDateTime(org.AccessTokenExpiresIn)) > 0)
            {
                //如果失效了，就直接更新一下.
                accessToken = getAccessToken(org);//获取企业凭证,更新失效时间
            }
            else
            {
                accessToken = org.AccessToken;
            }

            //组织发送信息的参数
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("touser", toUserIds.Replace(",", "|"));//指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为”@all”，则向该企业应用的全部成员发送
            parameters.Add("toparty", toUserIds.Replace(",", "|"));//指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。 当touser为”@all”时忽略本参数
            parameters.Add("totag", toUserIds.Replace(",", "|"));//指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。 当touser为”@all”时忽略本参数
            parameters.Add("msgtype", "text");//消息类型，此时固定为：text
            parameters.Add("msgtype", org.AgentId);//企业应用的id，整型。企业内部开发，可在应用的设置页面查看；第三方服务商，可通过接口 获取企业授权信息 获取该参数值
            parameters.Add("text", "{\"content\" : \"你有待办流程，请及时处理。\n<a href=\"http://work.weixin.qq.com\">XXXX</a>\"}");//消息内容，最长不超过2048个字节，超过将截断（支持id转译）
            parameters.Add("safe", "0");//表示是否是保密消息，0表示否，1表示是，默认0
            parameters.Add("enable_id_trans", "0");//表示是否开启id转译，0表示否，1表示是，默认0
            parameters.Add("enable_duplicate_check", "0");//表示是否开启重复消息检查，0表示否，1表示是，默认0
            parameters.Add("duplicate_check_interval", "1800");//表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时

            string sendUrl = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken;

            //获得返回的数据.
            string res = httpWebResponseUtility.HttpResponsePost_Json(sendUrl, JsonConvert.SerializeObject(parameters));

            //获取企业新信息，插入数据库
            //解析返回的json串
            Dictionary<string, object> dd = res.Trim(new char[] { '{', '}' }).Split(',').ToDictionary(s => s.Split(':')[0].Trim('"'), s => (object)s.Split(':')[1].Trim('"'));
            string errcode = (string)dd["errcode"];
            if (errcode.Equals("0"))
            {
                /*
                 * 如果部分接收人无权限或不存在，发送仍然执行，但会返回无效的部分（即invaliduser或invalidparty或invalidtag），
                 * 常见的原因是接收人不在应用的可见范围内。
                 * 如果全部接收人无权限或不存在，则本次调用返回失败，errcode为81013。
                 * 返回包中的userid，不区分大小写，统一转为小写
                 */
                string invaliduser = (string)dd["invaliduser"];//发送的接收人中无效的用户名
                string invalidparty = (string)dd["invalidparty"];//发送的接收人中无效的部门
                string invalidtag = (string)dd["invalidtag"];//发送的接收人中无效的标签
                return;
            }
            //如果全部接收人无权限或不存在，则本次调用返回失败，errcode为81013。
            if (errcode.Equals("81013"))
            {
                return;
            }
        }

        #region 组织结构维护.
        /// <summary>
        /// 创建人员
        /// </summary>
        /// <returns></returns>
        public string Organization_NewEmp()
        {
            Dept dept = new Dept(this.FK_Dept);
            var authRes = new App_Org().GetAuth(dept.OrgNo);
            if (authRes.code != ResponseCode.success)
                throw new Exception(authRes.msg);

            if (RegexUtil.IsNumLetter(this.No) == false)
                return "err@账号[" + this.No + "]不可以包含特殊字符";

            BP.Cloud.Emp emp = new Emp();
            emp.Retrieve(BP.Cloud.EmpAttr.UserID, this.No);
            if (emp.Retrieve(BP.Cloud.EmpAttr.UserID, this.No, EmpAttr.OrgNo, WebUser.OrgNo) != 0)
                return "err@组织:"+ WebUser.OrgName + ",已经存在此账号";

            emp = new Emp();
            emp.No = BP.Web.WebUser.OrgNo + "_" + this.No;
            if (emp.RetrieveFromDBSources() == 1)
            {
                return "err@当前组织已经存在此账号";
                //插入数据.
                DeptEmp myde = new DeptEmp();
                myde.MyPK = myde.FK_Dept + "_" + emp.UserID;
                int i = myde.RetrieveFromDBSources();
                if (i == 0)
                {
                    myde.FK_Dept = this.FK_Dept;
                    myde.FK_Emp = emp.UserID;
                    myde.OrgNo = emp.OrgNo;
                    myde.IsMainDept = false;
                    myde.Insert();
                    return "info@该人员的ID隶属于[" + emp.FK_DeptText + "],已经被关联到本部门作为兼职.";
                }
                else
                {
                    myde.FK_Dept = this.FK_Dept;
                    myde.FK_Emp = emp.UserID;
                    myde.OrgNo = emp.OrgNo;
                    myde.IsMainDept = false;
                    myde.Update();
                    return "info@该人员的ID隶属于[" + emp.FK_DeptText + "],已经被关联到本部门作为兼职.";
                }
            }

            emp.Name = "新同事";
            emp.FK_Dept = this.FK_Dept;
            emp.UserID = this.No; //设置userID.
            if (DataType.IsNumStr(this.No) == true)
                emp.Tel = this.No;
            emp.OrgNo = BP.Web.WebUser.OrgNo;
            emp.Insert();

            //插入数据,设置他的部门.
            DeptEmp de = new DeptEmp();
            de.FK_Dept = emp.FK_Dept;
            de.FK_Emp = emp.UserID;
            de.MyPK = de.FK_Dept + "_" + de.FK_Emp;
            de.OrgNo = emp.OrgNo;
            de.IsMainDept = true;
            de.Save();

            return "创建成功.";
        }
        public string DeptEmpNodeUncheck()
        {
            string empNo = this.GetRequestVal("EmpNo");
            BP.Cloud.Emp emp = new BP.Cloud.Emp(empNo);

            string deptNo = this.GetRequestVal("DeptNo");
            string pk = deptNo + "_" + emp.UserID;

            BP.Cloud.DeptEmp deptEmp = new BP.Cloud.DeptEmp();
            if (deptEmp.IsExit("MyPK", pk))
                deptEmp.Delete();


            return deptEmp.IsExit("MyPK", pk) ? "err@操作失败" : "操作成功";
        }
        public string DelEmp()
        {
            string empNo = this.GetRequestVal("empNo");
            string userId = this.GetRequestVal("userId");
            string deptNo = this.GetRequestVal("deptNo");
            string orgNo = this.GetRequestVal("orgNo");

            string str = "select COUNT(*) num from WF_EmpWorks where FK_Emp='" + userId + "' and OrgNo='" + OrgNo + "'";
            int count = DBAccess.RunSQLReturnValInt(str);
            if (count > 0)
                return "err@此账号有待办工作需要处理,不可以删除.";

            str = "select COUNT(*) num from Port_DeptEmp where FK_Emp='" + userId + "' and OrgNo='" + orgNo + "' and IsMainDept=0";
            count = DBAccess.RunSQLReturnValInt(str);
            if (count > 0)
                return "err@此账号有关联子部门,不可以删除.";

            str = "delete from Port_Emp where no = '" + empNo + "'";
            DBAccess.RunSQL(str);

            str = "delete from wf_emp where no = '" + empNo + "'";
            DBAccess.RunSQL(str);

            str = "delete FROM Port_DeptEmp WHERE FK_EMP = '" + userId + "' AND OrgNo = '" + orgNo + "'";
            DBAccess.RunSQL(str);

            str = "delete FROM Port_DeptEmpStation WHERE  FK_EMP = '" + userId + "' AND OrgNo = '" + orgNo + "'";
            DBAccess.RunSQL(str);


            return "操作成功";
        }
        /// <summary>
        /// 初始化组织结构部门表维护.
        /// </summary>
        /// <returns></returns>
        public string Organization_Init()
        {
            BP.Cloud.Depts depts = new BP.Cloud.Depts();
            depts.Retrieve("OrgNo", WebUser.OrgNo);
            return depts.ToJson();
        }
        /// <summary>
        /// 获取该部门的所有人员
        /// </summary>
        /// <returns></returns>
        public string LoadDatagridDeptEmp_Init()
        {
            string deptNo = this.GetRequestVal("deptNo");
            if (string.IsNullOrEmpty(deptNo))
            {
                return "{ total: 0, rows: [] }";
            }
            string orderBy = this.GetRequestVal("orderBy");

            string searchText = this.GetRequestVal("searchText");
            if (!DataType.IsNullOrEmpty(searchText))
            {
                searchText.Trim();
            }
            string addQue = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                addQue = "  AND (pe.No like '%" + searchText + "%' or pe.Name like '%" + searchText + "%') ";
            }

            string pageNumber = this.GetRequestVal("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = this.GetRequestVal("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = "(select pe.*,pd.name FK_DutyText from port_emp pe left join port_duty pd on pd.no = pe.fk_duty where pe.no in (select fk_emp from Port_DeptEmp where fk_dept='" + deptNo + "') "
                + addQue + " ) dbSo ";

            return DBPaging(sql, iPageNumber, iPageSize, "No", orderBy);
        }
        /// <summary>
        /// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        /// </summary>
        /// <param name="dataSource">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">当前页数据条数</param>
        /// <param name="key">计算总行数需要</param>
        /// <param name="orderKey">排序字段</param>
        /// <returns></returns>
        public string DBPaging(string dataSource, int pageNumber, int pageSize, string key, string orderKey)
        {
            string sql = "";
            string orderByStr = "";

            if (!string.IsNullOrEmpty(orderKey))
                orderByStr = " ORDER BY " + orderKey;

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    int beginIndex = (pageNumber - 1) * pageSize + 1;
                    int endIndex = pageNumber * pageSize;

                    sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
                        "FROM (SELECT * FROM  " + dataSource + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT TOP " + pageSize + " * FROM " + dataSource + " WHERE " + key + " NOT IN  ("
                    + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + dataSource + " )" + orderByStr;
                    break;
                case DBType.MySQL:
                    pageNumber -= 1;
                    sql = "select * from  " + dataSource + orderByStr + " limit " + pageNumber + "," + pageSize;
                    break;
                default:
                    throw new Exception("暂不支持您的数据库类型.");
            }

            DataTable DTable = DBAccess.RunSQLReturnTable(sql);

            int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + dataSource);

            return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        }
        #endregion

    }
}
