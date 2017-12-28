using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.Web;

namespace BP.Port
{
    public class Emp
    {
        public Emp()
        {
        }
        /// <summary>
        /// 人员
        /// </summary>
        /// <param name="fk_no"></param>
        public Emp(string fk_no)
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM Port_Emp WHERE No='" + fk_no + "'");
            if (dt.Rows.Count == 0)
            {
                throw new Exception("@用户名不存在。");
            }

            this.No = dt.Rows[0]["No"].ToString();
            this.Name = dt.Rows[0]["Name"].ToString();
            this.FK_Dept = dt.Rows[0]["FK_Dept"].ToString();
            this.Pass = dt.Rows[0]["Pass"].ToString();
            //this.SID = dt.Rows[0]["SID"].ToString();
        }
        public void Update()
        {
            BP.DA.DBAccess.RunSQL("UPDATE Port_Emp set SID='" + WebUser.SID + "' WHERE No='" + WebUser.No + "'");
        }
        public string No = null;
        public string Name = null;
        public string FK_Dept = null;
        public string Pass = null;
        //public string SID = null;
    }
}
