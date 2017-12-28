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
    /// 生成模版的垃圾数据 
    /// </summary>
    public class GenerDBTemplete : Method
    {
        /// <summary>
        /// 生成模版的垃圾数据
        /// </summary>
        public GenerDBTemplete()
        {
            this.Title = "生成模版的垃圾数据";
            this.Help = "可以手工的查看并清除他们.";
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

            MapDatas mds = new MapDatas();
            mds.RetrieveAll();

            string msg = "";
            Node nd = new Node();
            foreach (MapData item in mds)
            {
                if (item.No.Contains("ND") == false)
                    continue;

                string temp = item.No.Replace("ND", "");
                int nodeID = 0;
                try
                {
                    nodeID = int.Parse(temp);
                }
                catch
                {
                    continue;
                }

                nd.NodeID = nodeID;
                if (nd.IsExits == false)
                {
                    msg += "@" + item.No + "," + item.Name;
                    //删除该模版.
                    item.Delete();
                }
            }
            if (msg == "")
                msg = "数据良好.";
            else
                msg = "如下节点已经删除，但是节点表单数据没有被删除." + msg;

            return msg;
        }
    }
}
