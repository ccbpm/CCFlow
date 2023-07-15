using BP.DA;
using BP.En;
using System;
using System.Data;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRun_WhoExeIt : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRun_WhoExeIt()
        {
            this.Title = "执行节点的自动任务.";
            this.Help = "对于节点属性里配置的自动执行或者机器执行的节点上的任务自动发送下去。";
            this.GroupName = "流程自动执行定时任务";
        }
      
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string info = "";
            string sql = "SELECT WorkID,FID,FK_Emp,FK_Node,FK_Flow,AtPara FROM WF_GenerWorkerlist WHERE WhoExeIt!=0 AND IsPass=0 AND IsEnable=1 ORDER BY FK_Emp";
            DataTable dt = null;

            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return "无任务";

            #region 自动启动流程 whoExIt. 
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr[0].ToString());
                Int64 fid = Int64.Parse(dr[1].ToString());
                string empNo = dr[2].ToString();
                int nodeID = int.Parse(dr[3].ToString());
                string fk_flow = dr[4].ToString();
                string paras = dr[5].ToString();
                int toNodeID = 0;
                string toEmps = null;
                //判断AtPara中是否存在设置的时间
                if (DataType.IsNullOrEmpty(paras) == false && paras.Contains("DelayedData") == true)
                {
                    AtPara atPara = new AtPara(paras);
                    string delayedData = atPara.GetValStrByKey("DelayedData");
                    int day = atPara.GetValIntByKey("Day");
                    int hour = atPara.GetValIntByKey("Hour");
                    int minute = atPara.GetValIntByKey("Minute");
                    toNodeID = atPara.GetValIntByKey("ToNodeID");
                    toEmps = atPara.GetValStrByKey("ToEmps");
                    DateTime dtime = DataType.ParseSysDate2DateTime(delayedData);
                    //获取延期的小时
                    int hours = day * 24 + hour + minute / 60;
                    string newTime = dtime.AddDays(day).AddHours(hour).AddMinutes(minute).ToString(DataType.SysDateTimeFormat);
                    string currTime = DateTime.Now.ToString(DataType.SysDateTimeFormat);

                    if (DataType.GetSpanMinute(newTime, currTime) > 0)
                        continue;

                }

                if (BP.Web.WebUser.No.Equals(empNo) == false)
                    BP.WF.Dev2Interface.Port_Login(empNo);

                try
                {
                    
                    info += "发送成功:" + BP.Web.WebUser.No + BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid,toNodeID, toEmps).ToMsgOfText();
                }
                catch (Exception ex)
                {
                    info += "err@发送错误:" + ex.Message.ToString();
                }
            }
            #endregion 自动启动流程

            if (BP.Web.WebUser.No.Equals("admin") == false)
                BP.WF.Dev2Interface.Port_Login("admin");

            return info;
        }

        #region 重写。
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.IsAdmin == true)
                    return true;
                return false;
            }
        }
        #endregion 重写。

    }
}
