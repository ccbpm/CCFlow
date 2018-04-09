using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;


namespace CCFlow.WF.App.Comm
{
    public partial class FrmCheck : System.Web.UI.UserControl
    {
        #region 属性
        /// <summary>
        /// 是否隐藏审核输入框
        /// </summary>
        public bool IsHidden
        {
            get
            {
                string _isHidden = this.Request.QueryString["IsHidden"];
                if (DataType.IsNullOrEmpty(_isHidden))
                    return false;
                else
                    return bool.Parse(_isHidden);
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                string workid = this.Request.QueryString["OID"];
                if (workid == null)
                    workid = this.Request.QueryString["WorkID"];
                return Int64.Parse(workid);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 执行操作.
        /// </summary>
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

}