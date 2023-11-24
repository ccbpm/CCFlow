using BP.DA;


namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_GPM_Window : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_GPM_Window()
        {
        }
        public string Default_Mover()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enNo = ens[i];
                string sql = "UPDATE GPM_WindowTemplate SET Idx=" + i + " WHERE No='" + enNo + "'  ";
                DBAccess.RunSQL(sql);
                // BP.CCFast.Portal.Window en = new BP.CCFast.Portal.Window(); 
            }
            return "移动成功..";
        }
        public string Tabs_Default_Mover()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enNo = ens[i];
                string sql = "UPDATE GPM_MenuDtl SET Idx=" + i + " WHERE No='" + enNo + "'  ";
                DBAccess.RunSQL(sql);
                // BP.CCFast.Portal.Window en = new BP.CCFast.Portal.Window(); 
            }
            return "移动成功..";
        }

    }
}
