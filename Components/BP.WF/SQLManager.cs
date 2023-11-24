using BP.Difference;
using BP.Sys;
using BP.Web;
using BP.WF.Admin;
using BP.WF.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static NPOI.HSSF.Util.HSSFColor;

namespace BP.WF
{
    public class SQLManager
    {

        public static string GenerSQLByMark(string mark)
        {
            if (mark.Equals("srcDepts"))
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single) return " SELECT No, Name, ParentNo FROM Port_Dept Order by Idx ";
                return "SELECT No, Name, ParentNo FROM Port_Dept WHERE OrgNo = '"+WebUser.OrgNo+"' Order by Idx ";
            }

            if (mark.Equals("srcStations"))
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single) return "SELECT No, Name, FK_StationType GroupNo FROM Port_Station Order By Idx ";
                else return "SELECT No,Name,FK_StationType GroupNo FROM Port_Station WHERE OrgNo = '@WebUser.OrgNo' Order By Idx ";
            }

            if (mark.Equals("srcEmps"))
            {
                return " SELECT No,Name,FK_Dept GroupNo FROM Port_Emp Order By Idx ";
            }

            if (mark.Equals("srcStationTypes"))
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single) return "SELECT No, Name FROM Port_StationType Order by Idx ";
                else return "SELECT No,Name FROM Port_StationType WHERE OrgNo = '@WebUser.OrgNo' Order by Idx ";
            }

            throw new Exception("没有判断的标记:" + mark);
        }
    }
}
