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
    public class VSTOExcel : ApiController
    {
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
            if (WebUser.IsAdmin == false)
                return "err@没有权限.";
            try
            {
                MapData md = new MapData(frmID);

                //获得数据. 
                byte[] dd = DBAccess.GetByteFromDB(md.PTable, "No", md.No, frmID + "ExcelFile", false);

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
                MapData md = new MapData(frmID);

                DBAccess.SaveBytesToDB(byt,md.PTable, "No", md.No, frmID + "ExcelFile");
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
        public Object Frm_Save(string token, string frmID, Int64 workID, byte[] val,Hashtable ht, DataSet ds)
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
            return  Frm_Save(token, frmID, workID, val,null,null);
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
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="athNo"></param>
        /// <param name="athDBID"></param>
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

        #region 0. 公共方法.
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
        #endregion  0.  公共方法.  
    }
}
