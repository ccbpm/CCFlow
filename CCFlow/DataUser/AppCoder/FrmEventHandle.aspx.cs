﻿using System;

public partial class DataUser_AppCoder_FrmEventHandle : System.Web.UI.Page
{
    #region 属性
    public string SID
    {
        get
        {
            return this.Request.QueryString["SID"];
        }
    }
    public string WebUserNo
    {
        get
        {
            return this.Request.QueryString["WebUserNo"];
        }
    }
    public string FK_MapData
    {
        get
        {
            return this.Request.QueryString["FK_MapData"];
        }
    }
    public string FK_Event
    {
        get
        {
            return this.Request.QueryString["FK_Event"];
        }
    }
    public int OID
    {
        get
        {
            return int.Parse(this.Request.QueryString["OID"]);
        }
    }
    #endregion 属性

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            switch (this.FK_MapData)
            {
                case "ND101":
                    this.ND101();
                    break;
                default:
                    throw new Exception("没有对(" + this.FK_MapData + ")处理业务逻辑.");
            }
        }
        catch (Exception ex)
        {
            this.OutInfoErrMsg(ex.Message);
        }
    }

    #region 业务处理演示案例
    /// <summary>
    /// 在表单ID=ND101事件.
    /// </summary>
    public void ND101()
    {
        #region 当表单保存前.
        if (this.FK_Event == BP.Sys.EventListFrm.FrmLoadBefore)
        {
            /* 当表单装载前. */
        }
        #endregion 当表单保存前.

        #region 当表单装载后.  
        if (this.FK_Event == BP.Sys.EventListFrm.FrmLoadAfter)
        {
            /* 当表单装载后. */
        }
        #endregion 当表单装载后.

        #region 当表单保存前.
        if (this.FK_Event == BP.Sys.EventListFrm.SaveBefore)
        {
            /* 当表单保存前. */

        }
        #endregion 当表单保存前.

        #region 当表单保存后.
        if (this.FK_Event == BP.Sys.EventListFrm.SaveAfter)
        {
            throw new Exception("");
            /* 当表单保存后. */
            //GEEntity en = new GEEntity(this.FK_MapData, this.OID);
            //string rdt = en.GetValStrByKey("RDT");
            //string fid = this.Request.QueryString["FID"];
            //string rec = en.GetValStrByKey("Rec");

            ////访问数据库案例。
            //int result = DBAccess.RunSQL("DELETE FROM  WF_Emp WHERE 1=2 ");
            //DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_Emp");
        }
        #endregion 当表单保存后.
    }
    #endregion 业务处理演示案例

    #region 公用方法
    public void OutMsg(string msg)
    {
        this.Response.Write(msg);
    }
    public void OutInfoErrMsg(string msg)
    {
        this.Response.Write("Error:" + msg);
    }
    #endregion 公用方法

}