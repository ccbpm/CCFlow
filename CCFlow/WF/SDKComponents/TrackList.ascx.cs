using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.SDKComponents
{
    public partial class TrackList : BP.Web.UC.UCBase3
    {
        #region 参数.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                string nodeIDStr = this.Request.QueryString["FK_Node"];
                if (nodeIDStr == null)
                    nodeIDStr = this.Request.QueryString["NodeID"];

                if (nodeIDStr == null)
                    nodeIDStr = this.FK_Flow + "01";
                return int.Parse(nodeIDStr);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.WorkCheck wc = new BP.WF.WorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
            BP.WF.Tracks tks = wc.HisWorkChecks;

            this.AddTable(" width='100%' border='0' ");
            foreach (BP.WF.Track tk in tks)
            {
                switch (tk.HisActionType)
                {
                    case BP.WF.ActionType.Press: //催办.
                    case BP.WF.ActionType.UnSend: //撤销.
                    case BP.WF.ActionType.WorkCheck: //审核.
                        continue;
                    default:
                        break;
                }

                this.AddTR();
                this.AddTDBigDocBegain();
                this.Add("<p style='float:left'>");
                this.Add("<img src='/WF/Img/Action/" + tk.HisActionType + ".png' border=0 width='20px' width='20px' />");
                this.Add(" - " + tk.ActionTypeText);
                this.Add(" - " + BP.DA.DataType.ParseSysDate2DateTimeFriendly(tk.RDT));
                this.Add("</p>");

                this.Add("\t\n");
                this.Add("\t\n");
                this.AddBR();
                this.AddBR();

                switch (tk.HisActionType)
                {
                    case BP.WF.ActionType.Forward: //前进.
                    case BP.WF.ActionType.Start:   //发起.
                        string checkInfo = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, tk.NDFrom,"");
                        if (checkInfo == null)
                            this.Add(tk.MsgHtml);
                        else
                            this.Add(checkInfo);
                        break;
                    case BP.WF.ActionType.Return: //退回信息.
                        this.Add(tk.MsgHtml);
                        break;
                   
                    default:
                        throw new Exception("@没有判断的行为." + tk.HisActionType);
                }

                this.AddBR();
                this.Add("<p style='float:right'>"+tk.RDT +"   " +tk.Exer+"</p>");

                this.AddTDEnd();
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
    }
}