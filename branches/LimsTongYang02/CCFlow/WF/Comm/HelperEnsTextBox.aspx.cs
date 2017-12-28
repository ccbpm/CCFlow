using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Comm
{
    public partial class HelperEnsTextBox : System.Web.UI.Page
    {
        #region attr
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string Field
        {
            get
            {
                return this.Request.QueryString["Field"];
            }
        }
        #endregion attr

        protected void Page_Load(object sender, EventArgs e)
        {
           // UIContralType.AthShow
            // 求已经填写的默认值.
            Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            BP.En.QueryObject qo = new QueryObject(ens);
            if (en.EnMap.Attrs.Contains("Rec") == true)
            {
                qo.AddWhere("Rec", BP.Web.WebUser.No);
            }
            qo.Top = 12;
            DataTable dt = qo.DoQueryToTable();


            //求设置的默认值。
            string sql = "SELECT a.CurValue, 1 as IsDef FROM Sys_UserRegedit a  WHERE (A.FK_Emp='" + WebUser.No + "' OR A.FK_Emp='admin') AND FK_MapData='" + this.EnsName + "' AND NodeAttrKey='" + this.Field + "' ";
            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                DataRow row = mydt.NewRow();
                row[0] = dr[this.Field];
                row[1] =0;
                mydt.Rows.Add(row);
            }
        }
    }
}