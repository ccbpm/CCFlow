using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Port;
using Silverlight.DataSetConnector;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;
using BP.Tools;

/// <summary>
/// ccflowAPI 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class CCFlowAPI : CCForm
{
 
    #region 流程api
    /// <summary>
    /// 催办
    /// </summary>
    /// <param name="workid">工作编号</param>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Flow_DoPress(Int64 workid, string msg, string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);
        System.Data.DataSet ds = new DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.Flow_DoPress(workid, msg, true));
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    #endregion

    #region Port API

    /// <summary>
    /// 退出登陆
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public void Port_SigOut(string userNo)
    {
        BP.WF.Dev2Interface.Port_SigOut();
    }
    /// <summary>
    /// 获取菜单
    /// </summary>
    /// <param name="userNo">用户编号</param>
    [WebMethod(EnableSession = true)]
    public string Port_Menu(string userNo)
    {
        BP.WF.XML.Tools xmls = new BP.WF.XML.Tools();
        xmls.RetrieveAll();

        DataSet ds = new DataSet();
        ds.Tables.Add(xmls.ToDataTable());
        //  ds.WriteXml("c:\\Port_Menu获取菜单.xml");
        //return Connector.ToXml(ds);
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userNo">用户名</param>
    /// <param name="oldPass">旧密码</param>
    /// <param name="newPass">新密码</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Port_ChangePassword(string userNo, string oldPass, string newPass)
    {
        Emp emp = new Emp(userNo);
        if (emp.Pass == oldPass)
        {
            emp.Pass = newPass;
            emp.Update();
            return "修改成功，请牢记您的新密码。";
        }
        else
        {
            return "密码修改失败，旧密码错误。";
        }
    }
    /// <summary>
    /// 获取站内信
    /// 返回MsgType, Num 两个列.
    /// MsgType 在 BP.Sys.SMSMsgType中定义. 
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <param name="lastTime">上一次访问的时间</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Port_SMS(string userNo, string lastTime)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        Paras ps = new Paras();
        ps.SQL = "SELECT MsgType , Count(*) as Num FROM Sys_SMS WHERE SendTo='" + userNo + "' AND  RDT >'" + lastTime + "' AND MsgType IS NOT NULL Group By MsgType";
        ps.Add(BP.WF.SMSAttr.SendTo, userNo);

        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
        return BP.DA.DataType.ToJson(dt);
        //string strs = "";
        //foreach (DataRow dr in dt.Rows)
        //    strs += "@" + dr[0].ToString() + "=" + dr[1].ToString();
        ////return strs;
    }
    /// <summary>
    /// 获得当前操作员的系统消息
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="lastTime"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Port_SMS_DB(string userNo, string lastTime)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        Paras ps = new Paras();
        ps.SQL = "SELECT * FROM Sys_SMS WHERE SendTo='" + userNo + "' AND  RDT >'" + lastTime + "' ORDER BY RDT ";
        ps.Add(BP.WF.SMSAttr.SendTo, userNo);

        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    #endregion Port API

    #region 与数据源相关的接口.
    /// <summary>
    /// 获得移动菜单
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>菜单json</returns>
    [WebMethod(EnableSession = true)]
    public string DB_MobileMenu(string userNo)
    {
        DataSet ds = new DataSet();
        ds.ReadXml(BP.Sys.SystemConfig.PathOfDataUser + "\\Xml\\Mobile.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 获取通讯录
    /// </summary>
    /// <param name="DeptNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Address_List()
    {
        /**---小周鹏修改 2014-11-05---START**/
        //string sql = " select A.No, A.Name as UserName,B.Name as DeptName,A.Tel, A.Email from WF_Emp as A,Port_Dept as B where A.FK_Dept=B.No order by B.No ";
        string sql = " select A.No, A.Name as UserName,B.Name as DeptName,A.Tel, A.Email from Port_Emp as A,Port_Dept as B where A.FK_Dept=B.No order by B.No ";
        /**---小周鹏修改 2014-11-05---END**/
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        dt.Columns.Add("Img", typeof(string));
        foreach (DataRow dr in dt.Rows)
        {

            /**---小周鹏修改 2014-09-02---START**/
            // dr["Img"] = "/DataUser/UserIcon/" + dr["No"] + ".png";
            dr["Img"] = "/DataUser/UserIcon/" + dr["No"] + "Smaller.png";
            /**---小周鹏修改 2014-09-02---END**/
        }
        return BP.DA.DataType.ToJson(dt);
    }
    /// <summary>
    /// 获取个人信息
    /// </summary>
    /// <param name="DeptNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string UserInfoByNo(string userNo)
    {
        string sql = " select A.No, A.Name as UserName,B.Name as DeptName,A.Tel,A.Email from Port_Emp as A,Port_Dept as B where A.FK_Dept=B.No and A.NO='" + userNo + "' order by B.No  ";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        dt.Columns.Add("Img", typeof(string));
        foreach (DataRow dr in dt.Rows)
        {
            dr["Img"] = "/DataUser/UserIcon/" + dr["No"] + "Smaller.png";
        }
        return BP.DA.DataType.ToJson(dt);
    }
    /// <summary>
    /// 更改个人信息
    /// </summary>
    /// <param name="DeptNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string UserInfoChange(string userNo, string userName, string tel, string email)
    {

        string sSql = "Update Port_Emp set Name='" + userName + "',Tel='" + tel + "',Email='" + email + "' where No='" + userNo + "'";
        int i = BP.DA.DBAccess.RunSQL(sSql);
        if (i > 0)
        {
            return " 修改成功！ ";
        }
        else
        {
            return " 修改失败！ ";
        }
    }
    /// <summary>
    /// 意见反馈
    /// </summary>
    /// <param name="DeptNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string WriteUserMsg(string userNo, string msg)
    {
        string path = BP.Sys.SystemConfig.PathOfDataUser + "LogOfUser";
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);
        string dd = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        string file = path + "\\" + userNo + "_" + dd + ".txt";
        DataType.WriteFile(file, msg);
        return "反馈成功.";
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string WriteUserLog(string userNo, string msg)
    {
        string path = BP.Sys.SystemConfig.PathOfDataUser + "\\LogOfUser";
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        string dd = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        string file = path + "\\Log_" + userNo + "_" + dd + ".txt";

        DataType.WriteFile(file, msg);

        return "反馈成功.";
    }
    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="workid">工作ID</param>
    /// <param name="bytestr">图片</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string FileUploadImage(string userNo, string bytestr, string smaller, string byImg)
    {
        if (bytestr.Trim() == "")
            return "err:@文件上传失败！";

        try
        {
            string filePath = BP.Sys.SystemConfig.PathOfDataUser + "\\UserIcon\\";
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (!di.Exists)
                di.Create();
            string imgBName = filePath + "" + userNo + "Biger.png";
            string imgSName = filePath + "" + userNo + "Smaller.png";
            string imgName = filePath + "" + userNo + ".png";
            bool imgB = StringToFile(bytestr, imgBName);
            bool imgS = StringToFile(smaller, imgSName);
            bool img = StringToFile(byImg, imgName);
        }
        catch (Exception ex)
        {
            return "err:@" + ex.Message;
        }

        return "操作成功";

    }
    /// <summary> 
    /// 把经过base64编码的字符串保存为文件 
    /// </summary> 
    /// <param name="base64String">经base64加码后的字符串 </param> 
    /// <param name="fileName">保存文件的路径和文件名 </param> 
    /// <returns>保存文件是否成功 </returns> 
    public static bool StringToFile(string base64String, string fileName)
    {
        //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"/beapp/" + fileName; 
        System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
        if (!DataType.IsNullOrEmpty(base64String) && File.Exists(fileName))
        {
            bw.Write(Convert.FromBase64String(base64String));
        }
        bw.Close();
        fs.Close();
        return true;
    }
    /// <summary>
    /// 获取可以退回的节点集合
    /// </summary>
    /// <param name="nodeID">节点ID</param>
    /// <param name="workid">工作ID</param>
    /// <param name="fid">流程ID</param>
    /// <returns>返回退回的信息</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerWillReturnNodes(int nodeID, Int64 workid, Int64 fid, string userNo)
    {
        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            System.Data.DataSet ds = new System.Data.DataSet();
            DataTable table = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(nodeID, workid, fid);
            ds.Tables.Add(table);
            return BP.DA.DataType.ToJson(ds.Tables[0]);
            //return Connector.ToXml(ds);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    /// <summary>
    /// 获得流程树
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="sid"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string DB_FlowTree(string userNo, string sid)
    {
        try
        {

            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            string sql = "SELECT No, Name, ParentNo FROM WF_FlowSort ";
            DataTable sort = DBAccess.RunSQLReturnTable(sql);
            sort.TableName = "WF_FlowSort";

            string sql1 = "SELECT No, Name, FK_FlowSort as ParentNo FROM WF_Flow ";
            DataTable flow = DBAccess.RunSQLReturnTable(sql1);
            flow.TableName = "WF_Flow";

            DataSet ds = new DataSet();
            ds.Tables.Add(sort);
            ds.Tables.Add(flow);

            return BP.Tools.FormatToJson.ToJson(ds);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    /// <summary>
    /// 获得已经完成的流程列表.
    /// </summary>
    /// <param name="userNo">用户编号</param>
    /// <param name="sid">SID</param>
    /// <returns>返回No,Name,Num三个列</returns>
    [WebMethod(EnableSession = true)]
    public string DB_FlowCompleteGroup(string userNo, string sid)
    {
        try
        {
            BP.WF.Dev2Interface.Port_Login(userNo, sid);
            System.Data.DataSet ds = new System.Data.DataSet();
            DataTable table = BP.WF.Dev2Interface.DB_FlowCompleteGroup(userNo);
            ds.Tables.Add(table);
            return BP.DA.DataType.ToJson(ds.Tables[0]);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    /// <summary>
    /// 获得已经完成的流程数据
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="sid"></param>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="pageSize">每页的数量</param>
    /// <param name="pageIdx">第n页</param>
    /// <returns>返回WF_GenerWorklist数据, WorkID,Title,Starter,StarterName,WFState,FK_Node</returns>
    [WebMethod(EnableSession = true)]
    public string DB_FlowComplete(string userNo, string sid, string fk_flow, int pageSize, int pageIdx)
    {
        try
        {

            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            DataTable table = BP.WF.Dev2Interface.DB_FlowComplete(userNo, fk_flow, pageSize, pageIdx);
            return BP.DA.DataType.ToJson(table);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    /// <summary>
    /// 可以退回到的节点
    /// </summary>
    /// <param name="nodeID"></param>
    /// <param name="workid"></param>
    /// <param name="fid"></param>
    /// <param name="userNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string DataTable_DB_GenerWillReturnNodes(int nodeID, Int64 workid,
        Int64 fid, string userNo)
    {
        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);
            System.Data.DataSet ds = new System.Data.DataSet();
            DataTable table = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(nodeID, workid, fid);
            ds.Tables.Add(table);
            return BP.DA.DataType.ToJson(ds.Tables[0]);
            //return Connector.ToXml(ds);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    /// <summary>
    /// 获得任务池的工作列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>获得任务池的工作列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_TaskPool(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_TaskPool());
        ds.WriteXml("c:\\DB_TaskPool获得任务池的工作列表.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    /// <summary>
    /// 获得我从任务池里申请下来的工作列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>获得我从任务池里申请下来的工作列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_TaskPoolOfMyApply(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_TaskPoolOfMyApply());
        //ds.WriteXml("c:\\DB_TaskPoolOfMyApply获得我从任务池里申请下来的工作列表.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
   
    /// <summary>
    /// 获取当前操作员可以发起的流程集合
    /// </summary>
    /// <param name="userNo">用户编号</param>
    /// <param name="sid">SID</param>
    /// <returns>json</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerCanStartFlowsOfDataTable(string userNo, string sid = null)
    {
        BP.WF.Dev2Interface.Port_Login(userNo, sid);
        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(userNo));
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 获取当前操作员可以发起的流程集合
    /// </summary>
    /// <param name="userNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string DataTable_DB_GenerCanStartFlowsOfDataTable(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);
        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(userNo));
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 待办列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>待办列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DataTable_DB_GenerEmpWorksOfDataTable(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);
        DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();

        return BP.DA.DataType.ToJson(dt);
    }
    /// <summary>
    /// 待办列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>待办列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerEmpWorksOfDataTable(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable());
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //// ds.WriteXml("c:\\DB_GenerEmpWorksOfDataTable待办.xml");s
        //string str = Connector.ToXml(ds);
        ////  BP.DA.DataType.WriteFile("c:\\aaa.xml", str);
        //return str;
    }
    /// <summary>
    /// 抄送列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>操送列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_CCList(string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_CCList(userNo));
        // ds.WriteXml("c:\\DB_CCList抄送.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    /// <summary>
    /// 执行抄送已阅
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">流程节点</param>
    /// <param name="workID">工作id</param>
    /// <param name="fid">流程id</param>
    /// <param name="msge">填写意见</param>
    [WebMethod(EnableSession = true)]
    public string Node_DoCCCheckNote(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, Int64 fid, string msge)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        //BP.WF.Dev2Interface.Node_DoCCCheckNote(fk_flow, fk_node, workID, fid, msge);
        return "已阅完成";
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="flowNo"></param>
    /// <param name="WorkID"></param>
    /// <param name="FID"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string DB_Truck(string flowNo, Int64 WorkID, Int64 FID)
    {
        string sqlOfWhere1 = "";

        string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
        Paras prs = new Paras();
        if (FID == 0)
        {
            sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
            prs.Add("WorkID11", WorkID);
            prs.Add("WorkID12", WorkID);
        }
        else
        {
            sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
            prs.Add("FID11", FID);
            prs.Add("FID12", FID);
        }

        string sql = "";
        sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer FROM ND" + int.Parse(flowNo) + "Track " + sqlOfWhere1 + " ORDER BY RDT";
        prs.SQL = sql;

        DataTable dt = DBAccess.RunSQLReturnTable(prs);
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    /// <summary>
    /// 挂起列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>挂起列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerHungUpList(string userNo)
    {
        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerHungUpList(userNo));
        // ds.WriteXml("c:\\DB_GenerCanStartFlowsOfDataTable挂起.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    /// <summary>
    /// 在途列表
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>在途列表xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerRuning(string userNo)
    {

        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerRuning());
        //ds.WriteXml("c:\\DB_GenerRuning在途列表.xml");
        return BP.DA.DataType.ToJson(ds.Tables[0]);
        //return Connector.ToXml(ds);
    }
    #endregion 与数据源相关的接口.

    public CCFlowAPI()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    /// <summary>
    /// 创建空白工作WorkID
    /// </summary>
    /// <param name="flowNo">流程编号</param>
    /// <param name="starter">发起人</param>
    /// <param name="title">标题</param>
    /// <returns>workid</returns>
    [WebMethod(EnableSession = true)]
    public string Node_CreateBlankWork(string flowNo, string starter, string title)
    {
        if (WebUser.No != starter)
        {
            BP.WF.Dev2Interface.Port_Login(starter);
            //throw new Exception("@当前登录用户非(" + WebUser.No + ")");
        }

        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, null, null, starter, title).ToString());
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 执行删除 
    /// </summary>
    /// <param name="mypk"></param>
    /// <returns></returns>
    ///  小周鹏修改 2014-09-02 修改追加返回值
    [WebMethod(EnableSession = true)]
    public string Node_CC_DoDel(string mypk)
    {
        BP.WF.Dev2Interface.Node_CC_DoDel(mypk);
        /**---小周鹏修改 2014-09-02---START**/
        return "删除成功！";
        /**---小周鹏修改 2014-09-02---END**/
    }
    /// <summary>
    /// 设置读取了
    /// </summary>
    /// <param name="mypk"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_CC_SetRead(string mypks)
    {
        string[] strs = mypks.Split(',');
        foreach (string str in strs)
        {
            BP.WF.Dev2Interface.Node_CC_SetRead(str);
        }
        return null;
    }


    /// <summary>
    /// 执行抄送
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="toEmpNo">抄送给人员编号,多个用逗号分开比如 zhangsan,lisi</param>
    /// <param name="msgTitle">消息标题</param>
    /// <param name="msgDoc">消息内容</param>
    /// <param name="pFlowNo">父流程编号(可以为null)</param>
    /// <param name="pWorkID">父流程WorkID(可以为0)</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_CC(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, string toEmpNos, string msgTitle, string msgDoc, string pFlowNo, Int64 pWorkID)
    {
        if (WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);


        toEmpNos = toEmpNos.Replace(";", ",");
        toEmpNos = toEmpNos.Replace("；", ",");
        toEmpNos = toEmpNos.Replace("，", ",");

        string[] toEmps = toEmpNos.Split(',');
        string strs = "";
        foreach (string item in toEmps)
        {
            if (DataType.IsNullOrEmpty(item) == true)
                continue;
            Emp emp = new Emp(item);
            strs += emp.Name + " ";

            BP.WF.Dev2Interface.Node_CC(fk_flow, fk_node, workID, emp.No, emp.Name, msgTitle, msgDoc, pFlowNo, pWorkID);
        }

        return "执行抄送成功,抄送给:" + strs;
    }
    /// <summary>
    /// 设置当前工作状态为草稿,如果启用了草稿,请在开始节点的表单保存按钮下增加上它.
    /// 必须是在开始节点时调用.
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="workID">工作ID</param>
    [WebMethod(EnableSession = true)]
    public void Node_SetDraft(string fk_flow, Int64 workID)
    {
        BP.WF.Dev2Interface.Node_SetDraft(fk_flow, workID);
    }
    /// <summary>
    /// 设置工作已读
    /// </summary>
    /// <param name="nodeID"></param>
    /// <param name="workids"></param>
    /// <returns></returns>
    public string Node_SetWorkRead(int nodeID, string workids)
    {
        string[] strs = workids.Split(',');
        foreach (string str in strs)
        {
            if (DataType.IsNullOrEmpty(str))
                continue;

            BP.WF.Dev2Interface.Node_SetWorkRead(nodeID, Int64.Parse(str));

        }
        return null;
    }

    /// <summary>
    /// 节点工作取消挂起
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="workid">工作ID</param>
    /// <param name="msg">取消挂起原因</param>
    /// <returns>执行信息</returns>
    [WebMethod(EnableSession = true)]
    public void Node_UnHungUpWork(string fk_flow, Int64 workid, string msg)
    {
        BP.WF.Dev2Interface.Node_UnHungUpWork(fk_flow, workid, msg);
    }
    /// <summary>
    /// 撤销发送
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <param name="workid"></param>
    [WebMethod(EnableSession = true)]
    public string Flow_DoUnSend(string fk_flow, Int64 workid, string userNo)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);
        try
        {
            BP.WF.Dev2Interface.Flow_DoUnSend(fk_flow, workid);
            return "撤销成功.";
        }
        catch (Exception ex)
        {
            return "撤销失败:" + ex.Message;
        }
    }
    /// <summary>
    /// 节点工作挂起
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="workid">工作ID</param>
    /// <param name="way">挂起方式</param>
    /// <param name="reldata">解除挂起日期(可以为空)</param>
    /// <param name="hungNote">挂起原因</param>
    /// <returns>返回执行信息</returns>
    [WebMethod(EnableSession = true)]
    public string Node_HungUpWork(string fk_flow, Int64 workid, int wayInt, string reldata, string hungNote)
    {
        return BP.WF.Dev2Interface.Node_HungUpWork(fk_flow, workid, wayInt, reldata, hungNote);
    }
    /// <summary>
    /// 申请共享任务
    /// </summary>
    /// <param name="workid">工作ID</param>
    /// <param name="toEmp">移交到人员(只给移交给一个人)</param>
    /// <param name="msg">移交消息</param>
    [WebMethod(EnableSession = true)]
    public string Node_TaskPoolTakebackOne(Int64 workID, string userNo)
    {
        if (WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(workID);
        return "申请成功！";
    }
    /// <summary>
    /// 申请共享任务
    /// </summary>
    /// <param name="workid">工作ID</param>
    /// <param name="toEmp">移交到人员(只给移交给一个人)</param>
    /// <param name="msg">移交消息</param>
    [WebMethod(EnableSession = true)]
    public string Node_TaskPoolPutOne(Int64 workID, string userNo)
    {
        if (WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        BP.WF.Dev2Interface.Node_TaskPoolPutOne(workID);
        return "申请成功！";
    }
    /// <summary>
    /// 工作移交
    /// </summary>
    /// <param name="workid">工作ID</param>
    /// <param name="toEmp">移交到人员(只给移交给一个人)</param>
    /// <param name="msg">移交消息</param>
    [WebMethod(EnableSession = true)]
    public string Node_Shift(string flowNo, int nodeID, Int64 workID, Int64 fid, string toEmp, string msg, string userNo, string sid)
    {
        if (WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);
        return BP.WF.Dev2Interface.Node_Shift(flowNo, nodeID, workID, fid, toEmp, msg);
    }

    /**---小周鹏添加 2014-09-22---START**/
    /// <summary>
    /// 撤销移交
    /// </summary>
    /// <param name="fk_flow">流程</param>
    /// <param name="workid">工作ID</param>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    [WebMethod(EnableSession = true)]
    public string Un_Node_Shift(string userNo, string sid, string fk_flow, Int64 workID)
    {
        this.LetUserLogin(userNo, sid);

        string resultMsg = null;
        try
        {
            WorkFlow mwf = new WorkFlow(fk_flow, workID);
            string str = mwf.DoUnShift();

            resultMsg = str;
        }
        catch (Exception ex)
        {
            resultMsg = "err: @执行撤消失败，失败信息：" + ex.Message;
        }
        return resultMsg;
    }
    /**---小周鹏添加 2014-09-22---END**/
    /// <summary>
    /// 执行工作退回(退回指定的点)
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">流程ID</param>
    /// <param name="currentNodeID">当前节点ID</param>
    /// <param name="returnToNodeID">退回到的工作ID</param>
    /// <param name="msg">退回原因</param>
    /// <param name="isBackToThisNode">退回后是否要原路返回？</param>
    /// <returns>执行结果，此结果要提示给用户。</returns>
    [WebMethod(EnableSession = true)]
    public string Node_ReturnWork(string fk_flow, Int64 workID, Int64 fid, int currentNodeID,
        int returnToNodeID, string returnToEmp, string msg, bool isBackToThisNode, string userNo, string sid)
    {
        try
        {
            //让用户登录.
            LetUserLogin(userNo, sid);
            return BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workID, fid, currentNodeID, returnToNodeID, returnToEmp, msg, isBackToThisNode);
        }
        catch (Exception ex)
        {
            return "err@" + ex.Message;
        }
    }
    public string DataSetToXml(DataSet ds)
    {
        string strs = "";
        strs += "<DataSet>";
        foreach (DataTable dt in ds.Tables)
        {
            strs += "\t\n<" + dt.TableName + ">";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "\t\n< ";
                foreach (DataColumn dc in dt.Columns)
                {
                    strs += dc.ColumnName + "='" + dr[dc.ColumnName] + "' ";
                }
                strs += "/>";
            }
            strs += "\t\n</" + dt.TableName + ">";
        }
        strs += "\t\n</DataSet>";
        return strs;
    }
    /// <summary>
    /// 待办提示
    /// </summary>
    /// <param name="userNo"></param>
    /// <returns></returns>
    [WebMethod]
    public string AlertString(string userNo)
    {
        return "@EmpWorks=12@CC=34";
    }
    /// <summary>
    /// 用户登录
    /// 0,密码用户名错误
    /// 返回一个长的字符串标识登录成功，标识本地登录的安全验证码.
    /// 2,服务器错误.
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="pass"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Port_Login(string userNo, string pass)
    {
        try
        {
            Emp emp = new Emp();
            emp.No = userNo;

            if (emp.RetrieveFromDBSources() == 0)
                return "0"; //没有查询到。

            if (emp.CheckPass(pass) == false)
                return "1"; // 密码错误.

            return BP.WF.Dev2Interface.Port_GetSIDName(userNo);
        }
        catch (Exception ex)
        {
            // 数据库连接不上.
            Log.DefaultLogWriteLineError(ex.Message);
            return "2";
        }
    }
    /// <summary>
    /// 设置SID
    /// </summary>
    /// <param name="userNo">用户编号</param>
    /// <param name="sid">SID号</param>
    public void Port_SetSID(string userNo, string sid)
    {
        //   BP.WF.Dev2Interface.Port_SetSID(userNo, sid);
    }
    /// <summary>
    /// 信息执行
    /// </summary>
    /// <param name="flag">执行的标记</param>
    /// <param name="val0"></param>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <param name="val3"></param>
    /// <param name="val4"></param>
    /// <param name="val5"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string DoIt(string flag, string userNo, string sid, string fk_flow, string workID, string msg, string delModel, string val4, string val5)
    {
        LetUserLogin(userNo, sid);
        try
        {

            switch (flag)
            {
                case "UnSend": //撤销发送..
                    return BP.WF.Dev2Interface.Flow_DoUnSend(fk_flow, Int64.Parse(workID));
                case "EndWorkFlow": //结束流程.
                    return BP.WF.Dev2Interface.Flow_DoFlowOver(fk_flow, Int64.Parse(workID), msg);
                case "DelWorkFlow": //删除流程.
                    string model = delModel;
                    if (model == "1")
                    {
                        /*逻辑删除*/
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(fk_flow, Int64.Parse(workID), msg, false);
                    }
                    if (model == "2")
                    {
                        /*写入日志方式删除*/
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(fk_flow, Int64.Parse(workID), msg, false);
                    }
                    if (model == "3")
                    {
                        /*彻底删除*/
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, Int64.Parse(workID), false);
                    }
                    throw new Exception("@没有判断的删除模式." + delModel);
                default:
                    throw new Exception("@没有约定的标记:" + flag);
            }
        }
        catch (Exception ex)
        {
            return "err:" + ex.Message;
        }
    }
    /// <summary>
    /// 获取产生流程轨迹流程数据表的track.
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <param name="workID"></param>
    /// <param name="fid"></param>
    /// <param name="userNo"></param>
    /// <param name="sid"></param>
    /// <returns>返回流程需要用的东西</returns>
    [WebMethod(EnableSession = true)]
    public string GenerFlowTrack_Josn(string fk_flow, Int64 workID, Int64 fid, string userNo, string sid)
    {
        DataSet ds = BP.WF.Dev2Interface.DB_GenerTrack(fk_flow, workID, fid);
        return BP.Tools.FormatToJson.ToJson(ds);
    }
    /// <summary>
    /// 获取一条待办工作
    /// </summary>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="userNo">操作员编号</param>
    /// <returns>string的json</returns>
    [WebMethod(EnableSession = true)]
    public string GenerWorkNode_JSON(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo, string sid)
    {
        this.LetUserLogin(userNo, sid);
        DataSet ds = BP.WF.CCFlowAPI.GenerWorkNodeForAndroid(fk_flow, fk_node, workID, fid, userNo);
        return BP.Tools.FormatToJson.ToJson(ds);
    }
    /// <summary>
    /// 获取一条待办工作
    /// </summary>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="userNo">操作员编号</param>
    /// <returns>string的json</returns>
    [WebMethod(EnableSession = true)]
    public string GenerWorkNode_JSONV2(string fk_flow, int fk_node, Int64 workID, Int64 fid, bool isCc, float srcWidth, float srcHeight, string userNo, string sid)
    {
        this.LetUserLogin(userNo, sid);
        DataSet ds = this.GenerWorkNodeV2(fk_flow, fk_node, workID, fid, isCc, srcWidth, srcHeight);
        return BP.Tools.FormatToJson.ToJson(ds);
    }

    private DataSet GenerWorkNodeV2(string fk_flow, int fk_node, Int64 workID, Int64 fid, bool iscc, float srcWidth, float srcHeight)
    {
        if (fk_node == 0)
            fk_node = int.Parse(fk_flow + "01");

        if (workID == 0)
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, WebUser.No, null);

        try
        {
            MapData md = new MapData();
            md.No = "ND" + fk_node;
            if (md.RetrieveFromDBSources() == 0)
                throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

            DataSet myds = new DataSet(); // md.GenerHisDataSet();

            #region 流程设置信息.
            Node nd = new Node(fk_node);

            //流程数据. 计算出来表单的位移.
            string sql = "SELECT  Ver as FlowVer, '" + md.Ver + "' as FormVer, " + MapData.GenerSpanWeiYi(md, srcWidth) + " as WeiYi, " + MapData.GenerSpanHeight(md, srcHeight) + " as SrcH, " + MapData.GenerSpanWidth(md, srcWidth) + " as SrcW  FROM WF_Flow WHERE No='" + fk_flow + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "BaseInfo";

            // 增加参数。
            dt.Columns.Add("WeiYi2", typeof(float));
            dt.Columns.Add("SrcH2", typeof(float));
            dt.Columns.Add("SrcW2", typeof(float));

            dt.Rows[0]["WeiYi2"] = MapData.GenerSpanWeiYi(md, srcHeight);
            dt.Rows[0]["SrcH2"] = MapData.GenerSpanHeight(md, srcWidth);
            dt.Rows[0]["SrcW2"] = MapData.GenerSpanWidth(md, srcHeight);

            myds.Tables.Add(dt);
            #endregion 流程设置信息.

            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = workID;
            wk.RetrieveFromDBSources();

            // 处理传递过来的参数。
            foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
            {
                wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
            }

            // 执行一次装载前填充.
            md.DoEvent(FrmEventList.FrmLoadBefore, wk);



            wk.ResetDefaultVal();
            myds.Tables.Add(wk.ToDataTableField(md.No));

            //把附件的数据放入.
            if (md.FrmAttachments.Count > 0)
            {
                sql = "SELECT * FROM Sys_FrmAttachmentDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_FrmAttachmentDB";
                myds.Tables.Add(dt);
            }
            // 图片附件数据放入
            if (md.FrmImgAths.Count > 0)
            {
                sql = "SELECT * FROM Sys_FrmImgAthDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_FrmImgAthDB";
                myds.Tables.Add(dt);
            }

            //把从表的数据放入.
            if (md.MapDtls.Count > 0)
            {
                foreach (MapDtl dtl in md.MapDtls)
                {
                    GEDtls dtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
                    try
                    {
                        qo = new QueryObject(dtls);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                qo.addAnd();
                                qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, workID);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                    DataTable dtDtl = qo.DoQueryToTable();

                    // 为明细表设置默认值.
                    MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in dtlAttrs)
                    {
                        //处理它的默认值.
                        if (attr.DefValReal.Contains("@") == false)
                            continue;

                        foreach (DataRow dr in dtDtl.Rows)
                            dr[attr.KeyOfEn] = attr.DefVal;
                    }

                    dtDtl.TableName = dtl.No; //修改明细表的名称.
                    myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                }
            }
            #endregion

            #region 把外键表加入 DataSet

            DataTable dtMapAttr =BP.Sys.CCFormAPI.GenerHisDataSet( md.No).Tables["Sys_MapAttr"];
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                if (lgType != "2")
                    continue;

                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    throw new Exception("@属性字段数据不完整，流程:" + fk_flow + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }
            #endregion End把外键表加入DataSet

            #region 把流程信息放入里面.
            //把流程信息表发送过去.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            gwf.RetrieveFromDBSources();

            myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState == WFState.Shift)
            {
                //如果是转发.
                BP.WF.ShiftWorks fws = new ShiftWorks();
                fws.Retrieve(ShiftWorkAttr.WorkID, workID, ShiftWorkAttr.FK_Node, fk_node);
                myds.Tables.Add(fws.ToDataTableField("WF_ShiftWork"));
            }

            if (gwf.WFState == WFState.ReturnSta)
            {
                //如果是退回.
                ReturnWorks rts = new ReturnWorks();
                rts.Retrieve(ReturnWorkAttr.WorkID, workID,
                    ReturnWorkAttr.ReturnToNode, fk_node,
                    ReturnWorkAttr.RDT);
                myds.Tables.Add(rts.ToDataTableField("WF_ReturnWork"));
            }

            if (gwf.WFState == WFState.HungUp)
            {
                //如果是挂起.
                HungUps hups = new HungUps();
                hups.Retrieve(HungUpAttr.WorkID, workID, HungUpAttr.FK_Node, fk_node);
                myds.Tables.Add(hups.ToDataTableField("WF_HungUp"));
            }
            Int64 wfid = workID;
            if (fid != 0)
                wfid = fid;

            //放入track信息.
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", wfid);
            DataTable dtNode = DBAccess.RunSQLReturnTable(ps);
            dtNode.TableName = "Track";
            myds.Tables.Add(dtNode);

            //工作人员列表，用于审核组件.
            ps = new Paras();
            ps.SQL = "SELECT * FROM  WF_GenerWorkerlist WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", wfid);
            DataTable dtGenerWorkerlist = DBAccess.RunSQLReturnTable(ps);
            dtGenerWorkerlist.TableName = "WF_GenerWorkerlist";
            myds.Tables.Add(dtGenerWorkerlist);

            if (dtGenerWorkerlist.Rows.Count != 0 && nd.IsStartNode == false && iscc == false)
            {
                foreach (DataRow dr in dtGenerWorkerlist.Rows)
                {
                    if (dr[GenerWorkerListAttr.IsRead].ToString() == "1"
                        && dr[GenerWorkerListAttr.FK_Emp].ToString() == WebUser.No)
                    {
                        BP.WF.Dev2Interface.Node_SetWorkRead(fk_node, workID);
                        break;
                    }
                }
            }

            //放入CCList信息. 用于审核组件.
            ps = new Paras();
            ps.SQL = "SELECT * FROM WF_CCList WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", wfid);
            DataTable dtCCList = DBAccess.RunSQLReturnTable(ps);
            dtCCList.TableName = "WF_CCList";
            myds.Tables.Add(dtCCList);

            //放入WF_SelectAccper信息. 用于审核组件.
            ps = new Paras();
            ps.SQL = "SELECT * FROM WF_SelectAccper WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", wfid);
            DataTable dtSelectAccper = DBAccess.RunSQLReturnTable(ps);
            dtSelectAccper.TableName = "WF_SelectAccper";
            myds.Tables.Add(dtSelectAccper);
            #endregion 把流程信息放入里面.

           // myds.WriteXml("c:\\22xxx.xml", XmlWriteMode.IgnoreSchema);
            //BP.DA.DataType.WriteFile( "c:\\ss.xml", 

            return myds;
        }
        catch (Exception ex)
        {
            Log.DebugWriteError(ex.StackTrace);
            throw new Exception("Para:FK_Node="+fk_node+",workid:"+workID+",UserNo  Ext:"+ex.Message);
        }
    }
    /*小周鹏修改-------------------------------START*/
    /// <summary>
    /// 获取模板文件
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string GetFlowTemplete(string fk_flow, string fk_node, string ver)
    //  public string GetFlowTemplete(string fk_flow, string ver)
    {

        string resultJson = "";

        BP.WF.Flow fl = new Flow(fk_flow);
        string path = BP.Sys.SystemConfig.PathOfDataUser + "\\FlowDesc\\" + fl.No + "." + fl.Name;

        // string fileName =  path+ "\\Flow.xml";
        BP.WF.Node node = new Node(fk_node);
        string fileName = path + "\\" + fk_node + "." + node.Name + ".xml";
        /*小周鹏修改-------------------------------END*/

        if (System.IO.File.Exists(path) == false)
        {
            /*如果不存在, 就要生成他。 */
            DataSet dstemp = fl.GetFlow(path);
            dstemp.WriteXml(fileName);
            resultJson = FormatToJson.ToJson(dstemp);
        }

        DataSet ds = new DataSet();
        ds.ReadXml(fileName);
        DataTable dtFlow = ds.Tables["WF_Flow"];
        if (dtFlow.Rows[0]["Ver"].ToString() != ver)
        {
            /*如果不存在, 就要生成他。 */
            DataSet dstemp = fl.GetFlow(path);
            dstemp.WriteXml(fileName);
            resultJson = FormatToJson.ToJson(dstemp);
        }
        else
        {
            resultJson = FormatToJson.ToJson(ds);
        }

        return resultJson;
    }




    /**---小周鹏添加 2014-09-13---START**/
    /// <summary>
    /// 获取选择下一节点数据源
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_flow">工作流程</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <returns>string的json</returns>
    [WebMethod(EnableSession = true)]
    public string WorkOpt_GetToNodes(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, Int64 fid)
    {
        this.LetUserLogin(userNo, sid);

        Nodes nodes = BP.WF.Dev2Interface.WorkOpt_GetToNodes(fk_flow, fk_node, workID, fid);

        return BP.Tools.Entitis2Json.ConvertEntities2ListJson(nodes);
    }

    /// <summary>
    /// 选择下一节点发送
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">流程ID</param>
    /// <param name="to_node">到达的节点</param>
    /// <returns>发送结果</returns>
    [WebMethod(EnableSession = true)]
    public string WorkOpt_SendToNodes(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, Int64 fid,
        string to_node)
    {
        this.LetUserLogin(userNo, sid);

        // 执行发送.
        string msg = "";
        try
        {
            msg = BP.WF.Dev2Interface.WorkOpt_SendToNodes(fk_flow, fk_node, workID, fid, to_node).ToMsgOfText();
        }
        catch (Exception ex)
        {
            msg = "发送出现错误:" + ex.Message;
        }

        return msg;
    }

    /**---小周鹏添加 2014-09-13---END**/

    /**---小周鹏添加 2014-09-15---START**/
    /// <summary>
    /// 获得接收人的数据源
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">流程ID</param> 
    /// <returns>接收人的Json数据</returns>
    [WebMethod(EnableSession = true)]
    public string WorkOpt_AccepterDB(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, Int64 fid)
    {
        try
        {
            this.LetUserLogin(userNo, sid);

            // 获取接收人DataSet
            DataSet ds = BP.WF.Dev2Interface.WorkOpt_AccepterDB(fk_node, workID, fid);

            return BP.Tools.FormatToJson.ToJson(ds);
        }
        catch (Exception ex)
        {
            return "err:" + ex.Message;
        }
    }

    /// <summary>
    /// 设置指定的节点接受人
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_node">节点ID</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">流程ID</param>
    /// <param name="emps">指定的人员集合zhangsan,lisi,wangwu</param>
    /// <param name="isNextTime">是否下次自动设置</param>
    [WebMethod(EnableSession = true)]
    public string WorkOpt_SetAccepter(string userNo, string sid, int fk_node, Int64 workID, Int64 fid, string emps, bool isNextTime)
    {
        this.LetUserLogin(userNo, sid);

        BP.WF.Dev2Interface.WorkOpt_SetAccepter(fk_node, workID, fid, emps, isNextTime);

        return "接受人员设置成功！";
    }
    /**---小周鹏添加 2014-09-15---END**/

    /**---小周鹏添加 2014-09-17---START**/
    /// <summary>
    /// 删除流程
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="isDelSubFlow">是否要删除它的子流程</param>
    [WebMethod(EnableSession = true)]
    public string Flow_DoDeleteFlowByReal(string userNo, string sid, string fk_flow, Int64 workID, bool isDelSubFlow)
    {
        this.LetUserLogin(userNo, sid);

        BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workID, isDelSubFlow);

        return "删除成功！";
    }
    /**---小周鹏添加 2014-09-17---END**/

    /**---小周鹏添加 2014-09-19---START**/
    /// <summary>
    /// 回复加签内容
    /// </summary>
    /// <param name="userNo">用户</param>
    /// <param name="sid">安全码</param>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">FID</param>
    /// <param name="replyNote">答复信息</param>
    /// <returns>答复结果</returns>
    [WebMethod(EnableSession = true)]
    public string Node_AskforReply(string userNo, string sid, string fk_flow, int fk_node, Int64 workID, Int64 fid, string replyNote)
    {
        this.LetUserLogin(userNo, sid);

        string info = BP.WF.Dev2Interface.Node_AskforReply(workID, replyNote);

        return info;
    }
    /**---小周鹏添加 2014-09-19---END**/


    private DataSet GenerWorkNode_FlowDataOnly(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo)
    {
        if (fk_node == 0)
            fk_node = int.Parse(fk_flow + "01");

        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            DataSet myds = new DataSet();

            //节点
            Node nd = new Node(fk_node);

            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = workID;
            wk.RetrieveFromDBSources();
            wk.ResetDefaultVal();

            #region 设置默认值
            MapAttrs mattrs = nd.MapData.MapAttrs;
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIIsEnable)
                    continue;

                if (attr.DefValReal.Contains("@") == false)
                    continue;

                wk.SetValByKey(attr.KeyOfEn, attr.DefVal);
            }
            #endregion 设置默认值。

            //描述.
            MapData md = new MapData("ND" + fk_node);

 

            myds.Tables.Add(wk.ToDataTableField(md.No));
            if (md.MapDtls.Count > 0)
            {
                foreach (MapDtl dtl in md.MapDtls)
                {
                    GEDtls dtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
                    try
                    {
                        qo = new QueryObject(dtls);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                qo.addAnd();
                                qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, workID);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                    DataTable dtDtl = qo.DoQueryToTable();

                    // 为明细表设置默认值.
                    MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in dtlAttrs)
                    {
                        //处理它的默认值.
                        if (attr.DefValReal.Contains("@") == false)
                            continue;

                        foreach (DataRow dr in dtDtl.Rows)
                            dr[attr.KeyOfEn] = attr.DefVal;
                    }

                    dtDtl.TableName = dtl.No; //修改明细表的名称.
                    myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                }
            }
            #endregion

            #region 把外键表加入DataSet
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                if (lgType != "2")
                    continue;

                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();

                if (DataType.IsNullOrEmpty(uiBindKey))
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    throw new Exception("@属性字段数据不完整，流程:" + nd.FK_Flow + nd.FlowName + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }
            #endregion End把外键表加入DataSet

            #region 把流程信息放入里面.
            //把流程信息表发送过去.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState == WFState.Shift)
            {
                //如果是转发.
                BP.WF.ShiftWorks fws = new ShiftWorks();
                fws.Retrieve(ShiftWorkAttr.WorkID, workID, ShiftWorkAttr.FK_Node, fk_node);
                myds.Tables.Add(fws.ToDataTableField("WF_ShiftWork"));
            }

            if (gwf.WFState == WFState.ReturnSta)
            {
                //如果是退回.
                ReturnWorks rts = new ReturnWorks();
                rts.Retrieve(ReturnWorkAttr.WorkID, workID, ReturnWorkAttr.ReturnToNode, fk_node);
                myds.Tables.Add(rts.ToDataTableField("WF_ShiftWork"));
            }

            if (gwf.WFState == WFState.HungUp)
            {
                //如果是挂起.
                HungUps hups = new HungUps();
                hups.Retrieve(HungUpAttr.WorkID, workID, HungUpAttr.FK_Node, fk_node);
                myds.Tables.Add(hups.ToDataTableField("WF_HungUp"));
            }

            //放入track信息.
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            DataTable dtNode = DBAccess.RunSQLReturnTable(ps);
            dtNode.TableName = "Track";
            myds.Tables.Add(dtNode);
            #endregion 把流程信息放入里面.

           // myds.WriteXml("c:\\GenerWorkNode_FlowDataOnly.xml");
            return myds;
        }
        catch (Exception ex)
        {
            Log.DebugWriteError(ex.StackTrace);
            throw new Exception(ex.Message);
            //return "@生成工作FK_Flow=" + fk_flow + ",FK_Node=" + fk_node + ",WorkID=" + workID + ",FID=" + fid + "错误,错误信息:" + ex.Message;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <param name="fk_node"></param>
    /// <param name="workID"></param>
    /// <param name="fid"></param>
    /// <param name="userNo"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string GenerFlowTemplete_Json(string fk_flow)
    {
        Flow fl = new Flow(fk_flow);

        string path = BP.Sys.SystemConfig.PathOfDataUser + @"\FlowDesc\";

        DataSet myds = fl.DoExpFlowXmlTemplete(path);
        myds.WriteXml("c:\\GenerFlowTemplete_Json.xml");

        string strs = BP.Tools.FormatToJson.ToJson(myds);
        DataType.WriteFile("c:\\GenerFlowTemplete_Json.txt", strs);
        return strs;
    }
    /// <summary>
    /// 获取一条待办工作
    /// </summary>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="userNo">操作员编号</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_SaveWork(string fk_flow, int fk_node, Int64 workID, string userNo, string dsXml)
    {
        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            DataSet ds = Silverlight.DataSetConnector.Connector.FromXml(dsXml);
            Hashtable htMain = new Hashtable();
            DataTable dtMain = ds.Tables["ND" + fk_node]; //获得约定的主表数据.
            foreach (DataRow dr in dtMain.Rows)
                htMain.Add(dr[0].ToString(), dr[1].ToString());
            return BP.WF.Dev2Interface.Node_SaveWork(fk_flow, fk_node, workID, htMain, null);
        }
        catch (Exception ex)
        {
            return "@保存工作出现错误:" + ex.Message;
        }
    }
    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="FID">FID</param>
    /// <param name="userNo">用户</param>
    /// <param name="sid">验证码</param>
    /// <param name="jsonStr">json</param>
    /// <returns>返回执行结果</returns>
    [WebMethod(EnableSession = true)]
    public string Node_SaveWork_Json(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo, string sid, string jsonStr)
    {
        try
        {
            if (WebUser.No != userNo)
            {
                Emp emp = new Emp(userNo);
                BP.Web.WebUser.SignInOfGener(emp);
            }

            #region 此部分代码与 send 相同.
            // 接受数据.
            DataSet ds = BP.Tools.FormatToJson.JsonToDataSet(jsonStr);

            // 求出主表数据.
            string frm = "ND" + fk_node;
            DataTable dtMain = ds.Tables[frm];
            Hashtable htMain = new Hashtable();
            foreach (DataColumn dc in dtMain.Columns)
                htMain.Add(dc.ColumnName, dtMain.Rows[0][dc.ColumnName].ToString());

            // 判断是否有审核数据表.
            if (ds.Tables.Contains("FrmCheck") == true)
            {
                DataTable dtfrm = ds.Tables["FrmCheck"];
                string note = dtfrm.Rows[0][0] as string;
                string opName = dtfrm.Rows[0][1] as string;
                if (note != null)
                    BP.WF.Dev2Interface.WriteTrackWorkCheck(fk_flow, fk_node, workID, fid, note, opName);
            }
            #endregion 此部分代码与 send 相同.

            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fk_flow, fk_node, workID, htMain, ds);

            //把保存后的主表从表数据返回过去，有可能导致业务计算需要显示新的数据.
            #region 保存数据后仅仅返回主从表数据。
            //节点
            Node nd = new Node(fk_node);

            //定义数据容器.
            DataSet myds = new DataSet();

            // 把主从表数据放入里面.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = workID;
            QueryObject qoEn = new QueryObject(wk);
            qoEn.AddWhere("OID", workID);
            dtMain = qoEn.DoQueryToTable(1); // wk.ToDataTableField("ND" + fk_node);
            dtMain.TableName = "ND" + fk_node;
            myds.Tables.Add(dtMain);

            //描述.n
            MapData md = new MapData("ND" + fk_node);
            if (md.MapDtls.Count > 0)
            {
                foreach (MapDtl dtl in md.MapDtls)
                {
                    GEDtls dtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
                    try
                    {
                        qo = new QueryObject(dtls);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                qo.addAnd();
                                qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, workID);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                    DataTable dtDtl = qo.DoQueryToTable();

                    dtDtl.TableName = dtl.No; //修改明细表的名称.
                    myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                }
            }
            #endregion

            // 返回保存后的数据, 因为保存前后，需要执行事件，执行后就要发生数据的变化。
            //   return BP.DA.DataTableConvertJson.Dataset2Json(myds);
            return FormatToJson.ToJson(myds);
        }
        catch (Exception ex)
        {
            return "@保存工作出现错误:" + ex.Message;
        }
    }
    /// <summary>
    /// 执行发送
    /// </summary>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="fid">流程ID</param>
    /// <param name="userNo">操作员</param>
    /// <param name="sid">sid</param>
    /// <param name="jsonStr">json</param>
    /// <returns>发送执行信息</returns>
    [WebMethod(EnableSession = true)]
    public string Node_SendWork_Json(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo, string sid, string jsonStr)
    {
        this.LetUserLogin(userNo, sid);
        try
        {
            SendReturnObjs objs = null;
            if (jsonStr != null)
            {
                #region 此部分代码与 send 相同.
                // 接受数据.
                DataSet ds = BP.Tools.FormatToJson.JsonToDataSet(jsonStr);

                // 求出主表数据.
                string frm = "ND" + fk_node;
                DataTable dtMain = ds.Tables[frm];
                Hashtable htMain = new Hashtable();
                foreach (DataColumn dc in dtMain.Columns)
                    htMain.Add(dc.ColumnName, dtMain.Rows[0][dc.ColumnName].ToString());

                // 判断是否有审核数据表.
                if (ds.Tables.Contains("FrmCheck") == true)
                {
                    DataTable dtfrm = ds.Tables["FrmCheck"];
                    string note = dtfrm.Rows[0][0] as string;
                    string opName = dtfrm.Rows[0][1] as string;
                    if (note != null)
                        BP.WF.Dev2Interface.WriteTrackWorkCheck(fk_flow, fk_node, workID, fid, note, opName);
                }
                #endregion 此部分代码与 send 相同.

                //执行发送.
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, htMain, ds);
            }
            else
            {
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null);
            }
            return objs.ToMsgOfText();
        }
        catch (Exception ex)
        {
            return "@发送工作出现错误:" + ex.Message;
        }
    }
    /// <summary>
    /// 执行发送
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <param name="fk_node"></param>
    /// <param name="workID"></param>
    /// <param name="dsXml"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_SendWork(string fk_flow, int fk_node, Int64 workID, string dsXml, string currUserNo)
    {
        try
        {
            if (BP.Web.WebUser.No != currUserNo)
                BP.WF.Dev2Interface.Port_Login(currUserNo);

            SendReturnObjs objs = null;
            if (dsXml != null)
            {
                StringReader sr = new StringReader(dsXml);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);
                ds.WriteXml("c:\\GenerSendXml.xml");

                Hashtable htMain = new Hashtable();
                DataTable dtMain = ds.Tables["ND" + fk_node];
                foreach (DataRow dr in dtMain.Rows)
                {
                    htMain.Add(dr[0].ToString(), dr[1].ToString());
                }
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, htMain, ds);
            }
            else
            {
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null);
            }
            return objs.ToMsgOfText();
        }
        catch (Exception ex)
        {
            return "err@发送工作出现错误:" + ex.Message;
        }
    }
    /// <summary>
    /// 执行加签
    /// </summary>
    /// <param name="userNo">当前登录人</param>
    /// <param name="sid">校验码</param>
    /// <param name="workID">工作ID</param>
    /// <param name="_askforHelpSta">@5=加签后直接发送@6=加签后由我直接发送</param>
    /// <param name="toEmpNo">加签人</param>
    /// <param name="note">信息</param>
    /// <returns>执行结果</returns>
    [WebMethod(EnableSession = true)]
    public string Node_Askfor(string userNo, string sid, Int64 workID, int _askforHelpSta, string toEmpNo, string note)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        AskforHelpSta sta = (AskforHelpSta)_askforHelpSta;
        return BP.WF.Dev2Interface.Node_Askfor(workID, sta, toEmpNo, note);
    }

    [WebMethod]
    public string GetNoName(string SQL)
    {
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(SQL);
        DataSet ds = new DataSet();
        ds.Tables.Add(dt);
        return BP.DA.DataType.ToJson(ds.Tables[0]);
    }
    /// <summary>
    /// 大文件上传
    /// </summary>
    /// <param name="fileName">上传文件名</param>
    /// <param name="offSet">偏移</param>
    /// <param name="intoBuffer">每次上传字节数组 单位KB</param>
    /// <returns>上传是否成功</returns>
    [WebMethod]
    public bool Upload(string fileName, long offSet, byte[] intoBuffer)
    {
        //指定上传文件夹+文件名(相对路径)
        string strPath = @"D:\value-added\CCFlow\DataUser\UploadFile\" + fileName;
        //将相对路径转换成服务器的绝对路径
        //strPath = Server.MapPath(strPath);

        if (offSet < 0)
        {
            offSet = 0;
        }

        byte[] buffer = intoBuffer;

        if (buffer != null)
        {
            //读写文件的文件流,支持同步读写也支持异步读写
            FileStream filesStream = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            filesStream.Seek(offSet, SeekOrigin.Begin);
            filesStream.Write(buffer, 0, buffer.Length);
            filesStream.Flush();
            filesStream.Close();
            filesStream.Dispose();
            return true;
        }
        return false;
    }

    [WebMethod]
    public string ParseExp(string strExp)
    {
        DataTable dt = DBAccess.RunSQLReturnTable("select " + strExp);
        if (dt != null && dt.Rows.Count > 0)
        {
            return dt.Rows[0][0].ToString();
        }
        return string.Empty;
    }

    private void InitializeComponent()
    {
    }

    #region 获得审核信息. edity by xiaozhoupeng.  2014-07-26
    /// <summary>
    /// 获得审核信息
    /// </summary>
    /// <param name="userNo">当前操作员编号</param>
    /// <param name="sid">SID</param>
    /// <param name="fk_flow">流程编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <returns>审核信息</returns>
    [WebMethod(EnableSession = true)]
    public string GetCheckInfo(string userNo, string sid, string fk_flow, int fk_node, Int64 workID)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        return BP.WF.Dev2Interface.GetCheckInfo(fk_flow, workID, fk_node);
    }
    /// <summary>
    /// 写入工作审核日志:
    /// </summary>
    /// <param name="flowNo">流程编号</param>
    /// <param name="nodeID">节点从</param>
    /// <param name="workid">工作ID</param>
    /// <param name="FID">FID</param>
    /// <param name="msg">审核信息</param>
    /// <param name="optionName">操作名称(比如:科长审核、部门经理审批),如果为空就是"审核".</param>
    [WebMethod(EnableSession = true)]
    public string WriteTrackWorkCheck(string userNo, string sid, string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, string optionName)
    {
        if (BP.Web.WebUser.No != userNo)
            BP.WF.Dev2Interface.Port_Login(userNo);

        BP.WF.Dev2Interface.WriteTrackWorkCheck(flowNo, nodeFrom, workid, fid, msg, optionName);

        //设置审核完成.
        BP.WF.Dev2Interface.Node_CC_SetSta(nodeFrom, workid, BP.Web.WebUser.No, CCSta.CheckOver);


        return "审核成功！";
    }
    #endregion 获得审核信息.


    #region 与表单相关的api.
    /// <summary>
    /// 删除附件
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="sid"></param>
    /// <param name="mypk"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string CCFrom_DelFrmAttachment(string userNo, string sid, string mypk)
    {
        BP.Sys.FrmAttachmentDB db = new FrmAttachmentDB();
        db.MyPK = mypk;
        db.Delete();
        return "删除成功";
    }
    /// <summary>
    /// 上传附件
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="sid"></param>
    /// <param name="intoBuffer"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string CCForm_UploadFrmAttachment(string userNo, string sid, string fk_frmath, byte[] intoBuffer)
    {

        return "上传成功";
        //BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(fk_frmath);
        //string exts = System.IO.Path.GetExtension(fu.FileName).ToLower().Replace(".", "");

        ////如果有上传类型限制，进行判断格式
        //if (athDesc.Exts == "*.*" || athDesc.Exts == "")
        //{
        //    /*任何格式都可以上传*/
        //}
        //else
        //{
        //    if (athDesc.Exts.ToLower().Contains(exts) == false)
        //    {
        //        this.Alert("您上传的文件，不符合系统的格式要求，要求的文件格式:" + athDesc.Exts + "，您现在上传的文件格式为:" + exts);
        //        return;
        //    }
        //}

        //string savePath = athDesc.SaveTo;

        //if (savePath.Contains("@") == true || savePath.Contains("*") == true)
        //{
        //    /*如果有变量*/
        //    savePath = savePath.Replace("*", "@");
        //    GEEntity en = new GEEntity(athDesc.FK_MapData);
        //    en.PKVal = this.PKVal;
        //    en.Retrieve();
        //    savePath = BP.WF.Glo.DealExp(savePath, en, null);

        //    if (savePath.Contains("@") && this.FK_Node != null)
        //    {
        //        /*如果包含 @ */
        //        BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
        //        BP.WF.Data.GERpt myen = flow.HisGERpt;
        //        myen.OID = this.WorkID;
        //        myen.RetrieveFromDBSources();
        //        savePath = BP.WF.Glo.DealExp(savePath, myen, null);
        //    }
        //    if (savePath.Contains("@") == true)
        //        throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
        //}
        //else
        //{
        //    //savePath = athDesc.SaveTo + "\\" + this.PKVal;
        //}

        ////替换关键的字串.
        //savePath = savePath.Replace("\\\\", "\\");
        //try
        //{

        //    savePath = Server.MapPath("~/" + savePath);

        //}
        //catch (Exception)
        //{
        //    savePath = savePath;

        //}
        //try
        //{

        //    if (System.IO.Directory.Exists(savePath) == false)
        //    {
        //        System.IO.Directory.CreateDirectory(savePath);
        //        //System.IO.Directory.CreateDirectory(athDesc.SaveTo);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + Server.MapPath("~/" + savePath) + "===" + savePath + "@技术问题:" + ex.Message);
        //}

        ////int oid = BP.DA.DBAccess.GenerOID();
        //string guid = BP.DA.DBAccess.GenerGUID();

        //string fileName = fu.FileName.Substring(0, fu.FileName.LastIndexOf('.'));
        ////string ext = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
        //string ext = System.IO.Path.GetExtension(fu.FileName);

        ////string realSaveTo = Server.MapPath("~/" + savePath) + "/" + guid + "." + fileName + "." + ext;

        ////string realSaveTo = Server.MapPath("~/" + savePath) + "\\" + guid + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
        ////string saveTo = savePath + "/" + guid + "." + fileName + "." + ext;



        //string realSaveTo = savePath + "/" + guid + "." + fileName + "." + ext;

        //string saveTo = realSaveTo;

        //try
        //{
        //    fu.SaveAs(realSaveTo);
        //}
        //catch (Exception ex)
        //{
        //    this.Response.Write("@文件存储失败,有可能是路径的表达式出问题,导致是非法的路径名称:" + ex.Message);
        //    return;
        //}

        //FileInfo info = new FileInfo(realSaveTo);
        //FrmAttachmentDB dbUpload = new FrmAttachmentDB();

        //dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
        //dbUpload.NodeID = FK_Node.ToString();
        //dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

        //if (athDesc.AthUploadWay == AthUploadWay.Inherit)
        //{
        //    /*如果是继承，就让他保持本地的PK. */
        //    dbUpload.RefPKVal = this.PKVal.ToString();
        //}

        //if (athDesc.AthUploadWay == AthUploadWay.Interwork)
        //{
        //    /*如果是协同，就让他是PWorkID. */
        //    string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
        //    if (pWorkID == null || pWorkID == "0")
        //        pWorkID = this.PKVal;

        //    dbUpload.RefPKVal = pWorkID;
        //}

        //dbUpload.FK_MapData = athDesc.FK_MapData;
        //dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

        //dbUpload.FileExts = info.Extension;
        //dbUpload.FileFullName = saveTo;
        //dbUpload.FileName = fu.FileName;
        //dbUpload.FileSize = (float)info.Length;

        //dbUpload.RDT = DataType.CurrentDataTimess;
        //dbUpload.Rec = BP.Web.WebUser.No;
        //dbUpload.RecName = BP.Web.WebUser.Name;
        //if (athDesc.IsNote)
        //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

        //if (athDesc.Sort.Contains(","))
        //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

        //dbUpload.UploadGUID = guid;
        //dbUpload.Insert();

        ////   this.Response.Redirect("AttachmentUpload.aspx?FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal, true);
        //this.Response.Redirect(this.Request.RawUrl, true);
    }
    /// <summary>
    /// 表单全部信息
    /// </summary>
    /// <param name="fk_mapdata"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string CCForm_FrmTemplete(string fk_mapdata)
    {
        MapData md = new MapData(fk_mapdata);
        DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet( md.No);
        return BP.Tools.FormatToJson.ToJson(ds);
    }
    /// <summary>
    /// 流程信息
    /// </summary>
    /// <param name="fk_mapdata"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string CCFlow_FlowTemplete(string fk_flow)
    {
        DataSet ds = new DataSet();

        string sql = "";
        sql = "SELECT * FROM WF_Flow WHERE No='" + fk_flow + "'";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        dt.TableName = "WF_Flow";
        ds.Tables.Add(dt);

        sql = "SELECT * FROM WF_Node WHERE FK_Flow='" + fk_flow + "'";
        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        dt.TableName = "WF_Node";
        ds.Tables.Add(dt);

        return BP.Tools.FormatToJson.ToJson(ds);
    }
    #endregion
}
