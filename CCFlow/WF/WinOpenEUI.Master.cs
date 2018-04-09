using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF
{
    public partial class WinOpenEUI : System.Web.UI.MasterPage
    {
        #region 属性
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (DataType.IsNullOrEmpty(s))
                    throw new Exception("@流程编号参数错误...");

                s = DataType.ParseStringOnlyIntNumber(s);   //规避注入风险，added by liuxc
                return s;
            }
        }
        public string FromNode
        {
            get
            {
                return this.Request.QueryString["FromNode"];
            }
        }
        public string DoFunc
        {
            get
            {
                return this.Request.QueryString["DoFunc"];
            }
        }
        public string CFlowNo
        {
            get
            {
                return this.Request.QueryString["CFlowNo"];
            }
        }
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        public string Nos
        {
            get
            {
                return this.Request.QueryString["Nos"];
            }
        }

        public bool IsCC
        {
            get
            {

                if (DataType.IsNullOrEmpty(this.Request.QueryString["Paras"]) == false)
                {
                    string myps = this.Request.QueryString["Paras"];
                    if (myps.Contains("IsCC") == true)
                        return true;
                }
                return false;
            }
        }


        private Int64 _workid = 0;
        /// <summary>
        /// 当前的工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {

                if (this.Request.QueryString["WorkID"] == null)
                    return _workid;
                else
                    return Int64.Parse(this.Request.QueryString["WorkID"]);

            }
            set
            {
                _workid = value;
            }
        }
        public Int64 CWorkID
        {
            get
            {
                if (ViewState["CWorkID"] == null)
                {
                    if (this.Request.QueryString["CWorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.QueryString["CWorkID"]);
                }
                else
                    return Int64.Parse(ViewState["CWorkID"].ToString());
            }
            set
            {
                ViewState["CWorkID"] = value;
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public int FK_Node
        {
            get
            {
                string fk_nodeReq = this.Request.QueryString["FK_Node"];
                if (DataType.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.Request.QueryString["NodeID"];

                if (DataType.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.Request.QueryString["WorkID"] != null)
                    {
                        string sql = "SELECT FK_Node from  WF_GenerWorkFlow where WorkID=" + this.WorkID;
                        _FK_Node = DBAccess.RunSQLReturnValInt(sql);
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
            }
        }
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int PWorkID
        {
            get
            {
                try
                {
                    string s = this.Request.QueryString["PWorkID"];
                    if (DataType.IsNullOrEmpty(s) == true)
                        s = this.Request.QueryString["PWorkID"];
                    if (DataType.IsNullOrEmpty(s) == true)
                        s = "0";
                    return int.Parse(s);
                }
                catch
                {
                    return 0;
                }
            }
        }

        private bool isTab = false;

        public bool IsTab
        {
            get { return isTab; }
            set { isTab = value; }
        }

        private string officeTabName = "正文";

        public string OfficeTabName
        {
            get { return officeTabName; }
            set { officeTabName = value; }
        }

        private bool _IsWorkdTab = false;
        public bool IsWordTab
        {
            set { _IsWorkdTab = value; }
            get { return _IsWorkdTab; }
        }

        private bool _isReWord = false;
        //是否已完成
        public bool IsReWord
        {
            get { return _isReWord; }
            set { _isReWord = value; }
        }

        /// <summary>
        /// 是否公文标签置前模式
        /// </summary>
        public bool IsOfficeTabFront { get; set; }

        public string DoType
        {
            get { return this.Request.QueryString["DoType"]; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                BtnLab btnLab = new BtnLab(FK_Node);

                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.FrmFirst
                    || btnLab.WebOfficeWorkModel == WebOfficeWorkModel.WordFirst)
                {
                    IsTab = true;
                    OfficeTabName = btnLab.WebOfficeLab;
                    IsOfficeTabFront = btnLab.WebOfficeWorkModel == WebOfficeWorkModel.WordFirst;
                    if (WorkID == 0)
                    {
                        Flow currFlow = new Flow(this.FK_Flow);
                        WorkID = currFlow.NewWork().OID;
                    }
                    Node node = new Node(this.FK_Node);
                    if (!node.IsStartNode)
                    {
                        string path = Server.MapPath("~/DataUser/OfficeFile/" + this.FK_Flow + "/");

                        foreach (string file in System.IO.Directory.GetFiles(path))
                        {
                            System.IO.FileInfo info = new System.IO.FileInfo(file);
                            if (info.Name.StartsWith(this.WorkID.ToString()))
                            {
                                IsWordTab = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        IsWordTab = true;
                    }
                }
                try
                {
                    WorkFlow workflow = new WorkFlow(this.FK_Flow, this.WorkID);
                    bool IsComplate = workflow.HisGenerWorkFlow.WFState == WFState.Complete ? true : false;
                    if (IsComplate)
                    {
                        if (OfficeTabName.Equals("公文") || BP.Sys.SystemConfig.CustomerNo=="XCBANK")
                            IsReWord = true;
                    }
                }
                catch
                {
                    IsReWord = false;
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
            }
        }
    }
}