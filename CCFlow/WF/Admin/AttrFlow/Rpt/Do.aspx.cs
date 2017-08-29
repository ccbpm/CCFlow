using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF.Rpt;
namespace CCFlow.WF.MapDef.Rpt
{
    public partial class Do : BP.Web.WebPage
    {
        #region 参数.
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];
            }
        }
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "AddFlowMenu": // 增加流程菜单, 并且让所有人都可以使用.
                    //BP.GPM.Menu m = new BP.GPM.Menu(this.RefNo);
                    //BP.GPM.Menus ens = new BP.GPM.Menus();
                    //ens.Retrieve(BP.GPM.MenuAttr.FK_App, m.FK_App, BP.GPM.MenuAttr.Flag, "BPMDir");
                    //if (ens.Count != 0)
                    //{
                    //    this.Response.Write("该系统中已经存在流程菜单，所以您不能增加");
                    //    return;
                    //}

                    //BP.GPM.Menu dir = null;

                    //#region 找到父级节点。
                    //if (m.HisMenuType == BP.GPM.MenuType.App)
                    //    dir = m.DoCreateSubNode() as BP.GPM.Menu;
                    //else
                    //    m = new BP.GPM.Menu(m.ParentNo);

                    //if (dir == null)
                    //{
                    //    if (m.HisMenuType == BP.GPM.MenuType.App)
                    //        dir = m.DoCreateSubNode() as BP.GPM.Menu;
                    //    else
                    //        m = new BP.GPM.Menu(m.ParentNo);
                    //}
                    //if (dir == null)
                    //{
                    //    if (m.HisMenuType == BP.GPM.MenuType.App)
                    //        dir = m.DoCreateSubNode() as BP.GPM.Menu;
                    //    else
                    //        m = new BP.GPM.Menu(m.ParentNo);
                    //}
                    //if (dir == null)
                    //{
                    //    if (m.HisMenuType == BP.GPM.MenuType.App)
                    //        dir = m.DoCreateSubNode() as BP.GPM.Menu;
                    //    else
                    //        m = new BP.GPM.Menu(m.ParentNo);
                    //}
                    //#endregion 找到父级节点。

                    //if (dir == null)
                    //{
                    //    /*没有找到应用程序的根目录.*/
                    //    this.Response.Write("没有找到应用程序的根目录,系统错误...");
                    //    return;
                    //}

                    //dir.Name = "工作流程";
                    //dir.HisMenuType = BP.GPM.MenuType.Dir;
                    //dir.Flag = "BPMDir";
                    //dir.FK_App = m.FK_App;
                    //dir.Update();

                    //BP.WF.XML.ClassicMenus cms = new BP.WF.XML.ClassicMenus();
                    //cms.RetrieveAll();

                    //BP.Port.Stations stas=new BP.Port.Stations();
                    //stas.RetrieveAll();
                    //foreach (BP.WF.XML.ClassicMenu en in cms)
                    //{
                    //    BP.GPM.Menu func = dir.DoCreateSubNode() as BP.GPM.Menu;
                    //    func.Name = en.Name;
                    //    func.Url = en.Url;
                    //    func.FK_App = m.FK_App;
                    //    func.HisMenuType = BP.GPM.MenuType.Menu;
                    //    func.Update();

                    //    // 把权限分配到各个岗位上去.
                    //    foreach (BP.Port.Station item in stas)
                    //    {
                    //        BP.GPM.ByStation bysta = new BP.GPM.ByStation();
                    //        bysta.FK_Station =item.No;
                    //        bysta.RefObj = func.No;
                    //        try
                    //        {
                    //            bysta.Insert();
                    //        }
                    //        catch
                    //        {
                    //        }
                    //    }
                    //}
                    this.WinClose();
                    break;
                case "Del":
                    MapRpt rpt = new MapRpt();
                    rpt.No = this.RptNo;
                    rpt.Delete();
                    this.WinClose("ok");
                    break;
                case "EnableKeySearch":
                    MapData rpt1 = new MapData(this.RptNo);
                    rpt1.RptIsSearchKey = !rpt1.RptIsSearchKey;
                    rpt1.Update();
                    this.WinClose("ok");
                    break;
                default:
                    break;
            }
        }
    }
}