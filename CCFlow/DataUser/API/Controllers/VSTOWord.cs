using BP.DA;
using BP.Difference;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.WF;
using BP.WF.HttpHandler;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CCFlow.DataUser.API.Controllers
{
    [EnableCors("*", "*", "*")]
    public class VSTOWord : ApiController
    {
        #region 0. 公文组件.
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="token">登陆人员的信息</param>
        /// <param name="workID">工作ID</param>
        /// <returns>执行结果,IsHaveIt=0没有公文，=1有公文. , 没有公文的时候，提示转入选择模板.
        ///  IsCanDoCurrentWork=0，不能查看. IsCanDoCurrentWork=0 不能执行当前工作.</returns>
        [HttpGet, HttpPost]
        public Object VSTOWord_GongWen_Init(string token, Int64 workID)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                Flow fl = new Flow(gwf.FlowNo);

                //是否可以处理当前工作?
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(workID, WebUser.No))
                    gwf.Row.Add("IsCanDoCurrentWork", 1);
                else
                    gwf.Row.Add("IsCanDoCurrentWork", 0);

                //是否可查看?
                if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(gwf.FlowNo, workID))
                    gwf.Row.Add("IsCanView", 1);
                else
                    gwf.Row.Add("IsCanView", 0);

                //判断是否有公文文件？
                string fileName = DBAccess.RunSQLReturnStringIsNull("SELECT DocFile FROM " + fl.PTable + " WHERE OID=" + workID, null);
                if (fileName == null)
                {
                    gwf.Row.Add("IsHaveIt", 0);
                    gwf.Row.Add("Msg", "没有公文数据，请让用户选择模板.");
                }
                else
                {
                    gwf.Row.Add("IsHaveIt", 1);
                    gwf.Row.Add("Msg", "有公文数据，调用公文 VSTOWord_GongWen_GetFile 方法获取公文.");
                }

                //返回执行结果.
                return Return_Info(200, "执行成功", gwf.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        /// <summary>
        /// 创建公文
        /// </summary>
        /// <param name="token"></param>
        /// <param name="workID">工作ID</param>
        /// <param name="templateFileName">模板名称</param>
        /// <returns>返回创建信息</returns>
        [HttpGet, HttpPost]
        public Object VSTOWord_GongWen_Create(string token, Int64 workID, string templateFileName = null)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                //1.根据模板生成公文文件,处理模板文件.
                if (templateFileName == null)
                    templateFileName = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\GovDefalut.docx";

                //2. 
                GenerWorkFlow gwf = new GenerWorkFlow(workID);


                return Return_Info(200, "执行成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }

        /// <summary>
        /// 获得文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public object VSTOWord_GongWen_GetDocFile(string token, Int64 workID)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                //0. 创建gwf
                GenerWorkFlow gwf = new GenerWorkFlow(workID);

                //1. 判断权限。
                if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(gwf.FlowNo, gwf.WorkID) == false)
                    return Return_Info(500, "失败", "没有查看的权限.");

                //2. 获得文件.
                Flow fl = new Flow(gwf.FlowNo);
                string sql = "";
                byte[] file = DBAccess.GetFileFromDB("xxx", fl.PTable, "OID", workID.ToString(), "GovFile");


                return Return_Info(200, "执行成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        /// <summary>
        /// 获得pdf文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public object VSTOWord_GongWen_GetPDFFile(string token, Int64 workID)
        {

            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                return Return_Info(200, "执行成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public object VSTOWord_GongWen_SaveFile(string token, Int64 workID, byte[] file)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                //0. 创建gwf
                GenerWorkFlow gwf = new GenerWorkFlow(workID);

                //1. 判断权限。
                if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(gwf.FlowNo, gwf.WorkID) == false)
                    return Return_Info(500, "失败", "没有查看的权限.");

                //2. 判断权限。
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.WorkID, WebUser.No) == false)
                    return Return_Info(500, "失败", "没有操作工作的权限.");

                //3. 执行保存.
                Flow fl = new Flow(gwf.FlowNo);
                DBAccess.SaveBytesToDB(file, fl.PTable, "OID", workID.ToString(), "GovFile");

                return Return_Info(200, "执行成功", "1");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        #endregion  0.公文组件.

        #region 1. 表单设计器. TemplateDesinger
        /// <summary>
        /// 初始化：获得表单模板.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object TemplateDesinger_Init(string token, string frmID)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                MapData md = new MapData(frmID);

                //获得数据. 
                byte[] dd = DBAccess.GetByteFromDB(md.PTable, "No", md.No, "ExcelFile", false);

                //需要完善. 原来是什么？ ExcelFile就叫什么名字.
                return Return_Info(200, "执行成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="byt">二级制文件</param>
        /// <returns>是否成功</returns>
        [HttpGet, HttpPost]
        public Object TemplateDesinger_Save(string token, string frmID, byte[] byt)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                return Return_Info(200, "执行成功", "");
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        #endregion 1. 表单设计器. TemplateDesinger

        #region 2. 表单组件.
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Frm_Init(string token, string frmID, Int64 workID)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                return Return_Info(200, "执行成功", gwf.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }

        /// <summary>
        /// 保存方法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="val">二级制文件</param>
        /// <param name="ht">主表数据</param>
        /// <param name="ds">从表数据</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object Frm_Save(string token, string frmID, Int64 workID, byte[] val, Hashtable ht, DataSet ds)
        {
            //根据token登录.
            BP.WF.Dev2Interface.Port_LoginByToken(token);
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                return Return_Info(200, "执行成功", gwf.ToJson());
            }
            catch (Exception ex)
            {
                return Return_Info(500, "失败", ex.Message);
            }
        }
        #endregion 2. 表单组件.

        #region 3. 打印组件.
        /// <summary>
        /// 打印组件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object FrmPrinter_Init(string token, string frmID, Int64 workID)
        {
            return Frm_Init(token, frmID, workID);
        }
        /// <summary>
        /// 保存打印，没有主从表的数据.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object FrmPrinter_Save(string token, string frmID, Int64 workID, byte[] val)
        {
            return Frm_Save(token, frmID, workID, val, null, null);
        }
        #endregion  3. 打印组件.

        #region 4. 工作处理器.
        /// <summary>
        /// 工具栏初始化
        /// </summary>
        /// <param name="token"></param>
        /// <param name="flowNo"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object MyFlow_Init_Toolbar(string token, string flowNo, Int64 workID)
        {
            return null;
        }
        /// <summary>
        /// 工作初始化
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object MyFlow_Init_Work(string token, string frmID, Int64 workID)
        {
            return null;
        }
        /// <summary>
        /// 工作保存:与vsto的保存类似
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="mybyte"></param>
        /// <param name="ht"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object MyFlow_Save(string token, string frmID, Int64 workID, byte[] mybyte, Hashtable ht, DataSet ds)
        {
            return null;
        }
        #endregion  4. 工作处理器.

        #region 5. 附件助手.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="token"></param>
        /// <param name="flowNo"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object AthHelper_Init(string token, string frmID, Int64 workID, string athNo, string athDBID)
        {
            return null;
        }
        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public Object AthHelper_Save(string token, string frmID, Int64 workID, string athNo, string athDBID)
        {
            return null;
        }
        #endregion  5. 附件助手.

        #region 公共方法.
        /// <summary>
        /// 返回信息格式
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Object Return_Info(int code, string msg, string data)
        {
            Hashtable ht = new Hashtable();
            ht.Add("code", code);
            ht.Add("message", msg);
            ht.Add("data", data);
            return ht;
        }
        #endregion 公共方法.

    }
}
