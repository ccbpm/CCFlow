using System;
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
    public class ClearDB : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ClearDB()
        {
            this.Title = "清除流程运行的数据(此功能要在测试环境里运行)";
            this.Help = "清除所有流程运行的数据，包括待办工作。";
            this.Warning = "此功能要在测试环境里执行，确认是测试环境吗？";
            this.Icon = "<img src='/WF/Img/Btn/Delete.gif'  border=0 />";

            this.GroupName = "流程维护";
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
            if (BP.Web.WebUser.No != "admin")
                return "非法的用户执行。";

            //DBAccess.RunSQL("DELETE FROM WF_CHOfFlow");

            DBAccess.RunSQL("DELETE FROM WF_Bill");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow");
            DBAccess.RunSQL("DELETE FROM WF_ReturnWork");
            DBAccess.RunSQL("DELETE FROM WF_GenerFH");
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper");
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom");
            DBAccess.RunSQL("DELETE FROM WF_RememberMe");
            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB");
            DBAccess.RunSQL("DELETE FROM WF_CCList");
            DBAccess.RunSQL("DELETE FROM WF_CH"); //删除考核.

            Flows fls = new Flows();
            fls.RetrieveAll();
            foreach (Flow item in fls)
            {
                try
                {
                    DBAccess.RunSQL("DELETE FROM ND" + int.Parse(item.No) + "Track");
                }
                catch
                {
                }
            }

            Nodes nds = new Nodes();
            foreach (Node nd in nds)
            {
                try
                {
                    Work wk = nd.HisWork;
                    DBAccess.RunSQL("DELETE FROM " + wk.EnMap.PhysicsTable);
                }
                catch
                {
                }
            }

            MapDatas mds = new MapDatas();
            mds.RetrieveAll();
            foreach (MapData nd in mds)
            {
                try
                {
                    DBAccess.RunSQL("DELETE FROM " + nd.PTable);
                }
                catch
                {
                }
            }

            MapDtls dtls = new MapDtls();
            dtls.RetrieveAll();
            foreach (MapDtl dtl in dtls)
            {
                try
                {
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                }
                catch
                {
                }
            }
            return "执行成功...";
        }
    }
}
