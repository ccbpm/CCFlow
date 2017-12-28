﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_WorkOpt : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_WorkOpt(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 打包下载
        /// </summary>
        /// <returns></returns>
        public string Packup_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Packup_Init();
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        /// <returns></returns>
        public string HuiQian_SelectEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_SelectEmps();
        }

        #region 审核组件.
        public string WorkCheck_GetNewUploadedAths()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_GetNewUploadedAths();
        }
        public string WorkCheck_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_Init();
        }
        public string WorkCheck_Save()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.WorkCheck_Save();
        }
        #endregion 审核组件

        #region 会签.
        public string HuiQian_AddEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_AddEmps();
        }
        public string HuiQian_Delete()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_Delete();
        }
        public string HuiQian_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_Init();
        }
        public string HuiQian_SaveAndClose()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.HuiQian_SaveAndClose();
        }
        #endregion 会签

        #region 接收人选择器(限定接受人范围的).
        public string Accepter_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Init();
        }
        public string Accepter_Save()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Save();
        }
        public string Accepter_Send()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Accepter_Send();
        }
        #endregion 接收人选择器(限定接受人范围的).

        #region 接收人选择器(通用).
        public string AccepterOfGener_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Init();
        }
        public string AccepterOfGener_AddEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_AddEmps();
        }
        public string AccepterOfGener_Delete()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Delete();
        }
        public string AccepterOfGener_Send()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.AccepterOfGener_Send();
        }
        #endregion 接收人选择器(通用).

        #region 选择人员(通用).
        /// <summary>
        /// 将要去掉.
        /// </summary>
        /// <returns></returns>
        public string SelectEmps()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.SelectEmps_Init();
        }
        public string SelectEmps_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.SelectEmps_Init();
        }
        #endregion 选择人员(通用).


        #region 退回.
        public string Return_Init()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.Return_Init();
        }
        //执行退回.
        public string DoReturnWork()
        {
            WF_WorkOpt en = new WF_WorkOpt(this.context);
            return en.DoReturnWork();
        }
        #endregion 退回.

        #region xxx 界面 .
        #endregion xxx 界面方法.
    }
}
