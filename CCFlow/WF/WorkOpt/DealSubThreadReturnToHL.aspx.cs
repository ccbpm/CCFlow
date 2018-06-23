using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;


namespace CCFlow.WF.WorkOpt
{
    public partial class DealSubThreadReturnToHL : BP.Web.WebPage
    {
        #region 变量.
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 变量.

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            BP.WF.ReturnWork rw = new BP.WF.ReturnWork();
            rw.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                     BP.WF.ReturnWorkAttr.WorkID, this.WorkID);

            string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                this.FK_Node, rw.ReturnNode, this.TB_Doc.Text, false);

            //提示信息.
            BP.WF.Glo.ToMsg(info);
        }

        protected void Btn_Del_Click(object sender, EventArgs e)
        {
           string msg= BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID,"手工删除");
            //提示信息.
           if (msg == "" || msg == null)
               msg = "该工作删除成功...";

           BP.WF.Glo.ToMsg(msg);
        }

        

        protected void Btn_Shift_Click(object sender, EventArgs e)
        {
            string shiftNo = TB_ShiftNo.Value;
            string message = TB_Doc.Text;
            if (DataType.IsNullOrEmpty(shiftNo))
            {
                Alert("请选择移交人");
                return;
            }
            if(DataType.IsNullOrEmpty(message))
            {
                Alert("请填写处理人信息");
                return;
            }

          string result =   BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, shiftNo,message);

          BP.WF.Glo.ToMsg(result);
        }

        protected void Btn_UnSend_Click(object sender, EventArgs e)
        {
            try
            {
               string str= BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.FID,this.FK_Node);
               BP.WF.Glo.ToMsg(str);
            }
            catch(Exception ex)
            {
                BP.WF.Glo.ToMsg(ex.Message);
            }
        }
    }
}