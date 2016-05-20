using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.Web;
using BP.DA;
using BP.Sys;
using BP;

public partial class WF_MapDef_Rpt_Home : BP.Web.WebPage
{
    #region 属性.
    public string FK_Flow
    {
        get
        {
            string s= this.Request.QueryString["FK_Flow"];
            if (s == null)
                s ="007";
            return s;
        }
    }
    public int Idx
    {
        get
        {
            string s = this.Request.QueryString["Idx"];
            if (s == null)
                s = "0";
            return int.Parse(s);
        }
    }
    public string FK_MapAttr
    {
        get
        {
            string s = this.Request.QueryString["FK_MapAttr"];
            return s;
        }
    }
    public string FK_MapData
    {
        get
        {
            string s = this.Request.QueryString["FK_MapData"];
            if (s == null)
                s = "ND"+int.Parse(this.FK_Flow)+"Rpt";
            return s;
        }
    }
    public string RptNo
    {
        get
        {
            string s = this.Request.QueryString["RptNo"];
            if (s == null)
                s = "ND"+int.Parse(this.FK_Flow)+"MyRpt";
            return s;
        }
    }
    #endregion 属性.

    protected void Page_Load(object sender, EventArgs e)
    {
        var fl = new Flow(FK_Flow);
        Title = "报表设计：" + fl.Name;

        // 清除缓存数据.
        Cash.EnsData_Cash.Clear();
    }

    public void BindHome()
    {
        //this.Pub2.AddH2("欢迎使用ccflow报表设计器.");
        //this.Pub2.AddHR();

        //this.Pub2.AddFieldSet("什么是流程数据？");
        //this.Pub2.AddUL();
        //this.Pub2.AddLi("流程数据查询");
        //this.Pub2.AddLi("流程数据统计分析");
        //this.Pub2.AddLi("流程数对比分析");
        //this.Pub2.AddULEnd();
        //this.Pub2.AddFieldSetEnd();

        //this.Pub2.AddFieldSet("设计者必读？");
        //string info = "";
        //info += "<b>关于流程数据表:</b><br>";
        //info += "流程数据是一个流程上所有节点表单字段合集组成的表，是以NDxxxRpt命名的，流程发起后就向这个表中增加一条数据。";
        //info += "<br><b>如何进行权限控制:</b><br>";
        //info += "数据权限是以查询与分析的部门条件进行控制的，一个操作员能够查询那些部门的数据是管理员在系统中维护的，存放在WF_DeptFlowSearch表中。";
        //this.Pub2.Add(info);
        //this.Pub2.AddULEnd();
        //this.Pub2.AddFieldSetEnd();
    }
}