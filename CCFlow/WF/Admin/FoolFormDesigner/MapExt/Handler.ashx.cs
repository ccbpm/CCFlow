using System;
using System.Web;
using System.Data;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    /// <summary>
    /// MapExt 的摘要说明
    /// </summary>
    public class MapExtHandler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_MapExt);
            }
        }

        #region 属性.
        ///// <summary>
        ///// 字段
        ///// </summary>
        //public string KeyOfEn
        //{
        //    get
        //    {
        //        string str = context.Request.QueryString["KeyOfEn"];
        //        if (str == null || str == "")
        //            str = context.Request.QueryString["RefNo"].ToString();
        //        return str;
        //    }
        //}
        //public string DoType
        //{
        //    get
        //    {
        //        string str = context.Request.QueryString["DoType"].ToString();
        //        if (str == null || str == "")
        //            str = context.Request.QueryString["DoType"].ToString();
        //        return str;
        //    }
        //}
        ///// <summary>
        ///// extMap
        ///// </summary>
        //public string FK_MapExt
        //{
        //    get
        //    {
        //        string fk_mapExt = context.Request.QueryString["MyPK"] as string;
        //        if (fk_mapExt == null || fk_mapExt == "")
        //            fk_mapExt = context.Request.QueryString["FK_MapExt"] as string;
        //        return fk_mapExt;
        //    }
        //}
        ///// <summary>
        ///// 表单ID
        ///// </summary>
        //public string FK_MapData
        //{
        //    get
        //    {
        //        string str = context.Request.QueryString["FK_MapData"] as string;
        //        if (str == null || str == "")
        //            str = context.Request.QueryString["MyPK"] as string;
        //        return str;
        //    }
        //}
        //string no;
        //string name;
        //string fk_dept;
        //string oid;
        //string kvs;
        #endregion 属性.
        
    }
}
