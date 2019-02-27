using System;
using System.Data;
using System.Collections;
using BP;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.WF.DTS
{
    /// <summary>
    /// 删除空白的字段分组
    /// </summary>
    public class DeleteBlankGroupField : Method
    {
        /// <summary>
        /// 删除空白的字段分组
        /// </summary>
        public DeleteBlankGroupField()
        {
            this.Title = "删除空白的字段分组";
            this.Help = "";
            this.Icon = "<img src='/WF/Img/Btn/Delete.gif'  border=0 />";
        }
        public override void Init()
        {
             
        }
        public override bool IsCanDo
        {
            get { return true; }
        }
        public override object Do()
        {
            GroupFields gfs = new GroupFields();
            gfs.RetrieveAll();

            int delNum = 0;
            foreach (GroupField item in gfs)
            {
                int num = 0;
                num += DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE GroupID='" + item.OID + "' and FK_MapData='" + item.FrmID + "'");
                //num += DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_FrmAttachment WHERE GroupID=" + item.OID + " and FK_MapData='" + item.EnName + "'");
                //num += DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapDtl WHERE GroupID=" + item.OID + " and FK_MapData='" + item.EnName + "'");
                //num += DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapFrame WHERE GroupID=" + item.OID + " and FK_MapData='" + item.EnName + "'");
                if (num == 0)
                {
                    delNum++;
                    item.Delete();
                }
            }
            return "执行成功,删除数量:" + delNum;
        }
    }
}
