using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin
{
    public partial class BatchStartFields : System.Web.UI.Page
    {
        #region 属性.
        public int Step
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["Step"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }


        #endregion 属性.


        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int nodeID = int.Parse(this.FK_Node.ToString());
                BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
                BP.WF.Node nd = new BP.WF.Node(nodeID);

                this.TB_Num.Text = nd.BatchListCount.ToString();

                //动态为 批处理赋值 和默认参数
                string srole = "";
                if (nd.HisBatchRole.ToString() == "None")
                {
                    srole = "0";
                }
                else if (nd.HisBatchRole.ToString() == "Ordinary")
                {
                    srole = "1";
                }
                else
                {
                    srole = "2";
                }
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums(BP.WF.Template.NodeAttr.BatchRole);
                foreach (BP.Sys.SysEnum item in ses)
                {

                    this.DDL_BRole.Items.Add(new ListItem(item.Lab, item.IntKey.ToString()));

                    if (item.IntKey.ToString() == srole)
                    {
                        this.DDL_BRole.Items[int.Parse(srole)].Selected = true;
                    }

                }
            }

        }
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            int nodeID = int.Parse(this.FK_Node.ToString());
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //给变量赋值.
            //批处理的类型
            int selectval = int.Parse(this.DDL_BRole.SelectedValue.ToString());
            switch (selectval)
            {
                case 0:
                    nd.HisBatchRole = BP.WF.BatchRole.None;
                    break;
                case 1:
                    nd.HisBatchRole = BP.WF.BatchRole.Ordinary;
                    break;
                default:
                    nd.HisBatchRole = BP.WF.BatchRole.Group;
                    break;
            }
            //批处理的数量
            nd.BatchListCount = int.Parse(this.TB_Num.Text);
            //批处理的参数 
            string sbatchparas = "";
            if (Request["CB_Node"] != null)
            {
                sbatchparas = Request["CB_Node"].ToString();
            }
            nd.BatchParas = sbatchparas;
            nd.Update();

            BP.Sys.PubClass.Alert("保存成功.");
        }

    }
}