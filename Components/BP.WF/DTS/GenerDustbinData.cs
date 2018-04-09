using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerDustbinData : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerDustbinData()
        {
            this.Title = "找出因为ccbpm内部的错误而产生的垃圾数据";
            this.Help = "系统不去自动修复它，需要手工的确定原因。";
            this.Icon = "<img src='/WF/Img/Btn/Card.gif'  border=0 />";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {

            string msg = "";
            Flows fls = new Flows();
            fls.RetrieveAll();
            foreach (Flow fl in fls)
            {
                string rptTable = "ND" + int.Parse(fl.No) + "Rpt";
                string fk_mapdata = "ND" + int.Parse(fl.No) + "01";
                MapData md = new MapData(fk_mapdata);

                string sql = "SELECT OID,Title,Rec,WFState,WFState FROM " + md.PTable + " WHERE WFState=" + (int)WFState.Runing + " AND OID IN (SELECT OID FROM " + rptTable + " WHERE WFState!=0 )";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    continue;

                msg += "@" + sql;
                //msg += "修复sql: UPDATE " + ndTable"  " ;
            }
            if (DataType.IsNullOrEmpty(msg))
                return "@能检测到的数据正常.";

            BP.DA.Log.DefaultLogWriteLineInfo(msg);
            return  "如下数据产生异常，开始节点的标示的草稿状态在实际工作中标示出来已经完成了:"+msg+" 以上的数据写入了Log文件中，请打开查看。";
        }
    }
}
