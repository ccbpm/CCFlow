using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Web.Comm;
using BP;

namespace CCFlow.Web.Comm
{
    /// <summary>
    /// ��ѯͨ�ý���
    /// </summary>
    public partial class Search : BP.Web.WebPage
    {
        #region ����.
        public int PageIdxOfSeach
        {
            get
            {
                if (ViewState["PageIdxOfSeach"] == null)
                    return this.PageIdx;
                else
                    return 1;
            }
            set
            {
                ViewState["PageIdxOfSeach"] = value;
            }
        }
        public new Entities HisEns
        {
            get
            {
                if (this.EnsName != null)
                {
                    if (this._HisEns == null)
                        _HisEns = BP.En.ClassFactory.GetEns(this.EnsName);
                }
                return _HisEns;
            }
        }
        public new string Key
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    str = this.Request.QueryString["EnsName"];
                if (str == null)
                    throw new Exception("������Ч��");
                return str;
            }
        }
        /// <summary>
        /// _HisEns
        /// </summary>
        public new  Entities _HisEns = null;
        private  Entity _HisEn = null;
        public new Entity HisEn
        {
            get
            {
                if (_HisEn == null)
                    _HisEn = this.HisEns.GetNewEntity;
                return _HisEn;
            }
        }
        /// <summary>
        /// ��ʾ��ʽ.
        /// </summary>
        public ShowWay ShowWay
        {
            get
            {
                if (Session["ShowWay"] == null)
                {
                    if (this.Request.QueryString["ShowWay"] == null)
                    {
                        Session["ShowWay"] = "2";
                    }
                    else
                    {
                        Session["ShowWay"] = this.Request.QueryString["ShowWay"];
                    }
                }
                return (ShowWay)int.Parse(Session["ShowWay"].ToString());
            }
            set
            {
                Session["ShowWay"] = (int)value;
            }
        }

        public bool IsReadonly
        {
            get
            {
                string i = this.Request.QueryString["IsReadonly"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }

        public TB TB_Key
        {
            get
            {
                return this.ToolBar1.GetTBByID("TB_Key");
            }
        }

        /// <summary>
        /// ��ǰѡ��de En
        /// </summary>
        public Entity CurrentSelectEnDa
        {
            get
            {
                Entity en = this.HisEn;
                en.Retrieve();
                return en;
            }
        }
        public bool IsShowGroup
        {
            get
            {
                if (this.Request.QueryString["IsShowGroup"] == null)
                {
                    return true;
                }
                else
                {
                    if (this.Request.QueryString["IsShowGroup"] == "0")
                        return false;
                    else
                        return true;
                }
            }
        }
        public bool IsShowToolBar1
        {
            get
            {
                string str = this.Request.QueryString["IsShowToolBar1"];
                if (str == null || str == "1")
                    return true;
                return false;
            }
        }
        #endregion ����.

        #region װ�ط���. Page_Load
        protected void Page_Load(object sender, System.EventArgs e)
        {
            UAC uac = this.HisEn.HisUAC;
            if (uac.IsView == false)
                throw new Exception("��û�в鿴[" + this.HisEn.EnDesc + "]���ݵ�Ȩ��.");

            if (this.IsReadonly)
            {
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
            }

            if (this.Request.QueryString["PageIdx"] == null)
                this.PageIdx = 1;
            else
                this.PageIdx = int.Parse(this.Request.QueryString["PageIdx"]);

            Entity en = this.HisEn;
            UIConfig cfg = new UIConfig(en);

            // edit by stone : ������ʵʩ�Ļ�ȡmap, �������û���̬�����ò�ѯ����.
            Map map = en.EnMapInTime;
            this.ShowWay = ShowWay.Dtl;
            if (uac.IsView == false)
                throw new Exception("@�Բ�����û�в鿴��Ȩ�ޣ�");

            #region ����toolbar2 �� contral  ���ò�Ѱ����.
            this.ToolBar1.InitByMapV2(map, 1);
            
            bool isEdit = true;
            if (this.IsReadonly)
                isEdit = false;
            if (uac.IsInsert == false)
                isEdit = false;

            string js = "javascript:ShowEn('./RefFunc/UIEn.aspx?EnsName=" + this.EnsName + "','cd','" + cfg.WinCardH + "' , '" + cfg.WinCardW + "');";
            if (isEdit)
                this.ToolBar1.AddLinkBtn(NamesOfBtn.New, "�½�", js);

            js = "javascript:OpenAttrs('" + this.EnsName + "');";

            if (WebUser.No == "admin")
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Setting, "����", js);

            js = "javascript:DoExp();";
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Excel, "����", js);

            #endregion

            #region ����ѡ��� Ĭ��ֵ
            AttrSearchs searchs = map.SearchAttrs;
            bool isChange = false;
            foreach (AttrSearch attr in searchs)
            {
                string mykey = this.Request.QueryString[attr.Key];
                if (mykey == "" || mykey == null)
                    continue;
                else
                {
                    this.ToolBar1.GetDDLByKey("DDL_" + attr.Key).SetSelectItem(mykey, attr.HisAttr);
                    isChange = true;
                }
            }

            if (this.Request.QueryString["Key"] != null)
            {
                this.ToolBar1.GetTBByID("TB_Key").Text = this.Request.QueryString["Key"];
                isChange = true;
            }

            if (isChange == true)
            {
                /*����Ǳ�����*/
                this.ToolBar1.SaveSearchState(this.EnsName, null);
            }
            #endregion

            this.SetDGData();
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Search).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.Label1.Text = this.GenerCaption(this.HisEn.EnMap.EnDesc + "" + this.HisEn.EnMap.TitleExt);
            //��ʱ�ļ���
            this.expFileName.Value = this.HisEns.GetNewEntity.EnDesc + "���ݵ���" + "_" + BP.DA.DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";
        }
        #endregion װ�ط���. Page_Load

        #region ����
        public Entities SetDGData()
        {
            return this.SetDGData(this.PageIdx);
        }
        public Entities SetDGData(int pageIdx)
        {
            Entities ens = this.HisEns;
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;
            QueryObject qo = new QueryObject(ens);
            qo = this.ToolBar1.GetnQueryObject(ens, en);

            if (this.DoType == "Exp")
            {
                /*����ǵ������Ͱ���������excel.*/
                string filePath = this.ExportDGToExcel(qo.DoQueryToTable(), en.EnMap, en.EnDesc + "���ݵ���");
                this.WinClose(filePath);
                return null;
            }

            int maxPageNum = 0;
            try
            {
                this.UCSys2.Clear();
                  maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(),
                      SystemConfig.PageSize, pageIdx, "Search.aspx?EnsName=" + this.EnsName);
                if (maxPageNum > 1)
                    this.UCSys2.Add("��ҳ��:�� �� PageUp PageDown");
            }
            catch
            {
                try
                {
                    en.CheckPhysicsTable();
                }
                catch(Exception wx)
                {
                    BP.DA.Log.DefaultLogWriteLineError(wx.Message);
                }
                maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(), SystemConfig.PageSize, pageIdx, "Search.aspx?EnsName=" + this.EnsName);
            }

            qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx);

            if (map.IsShowSearchKey)
            {
                string keyVal = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();
                if (keyVal.Length >= 1)
                {
                    Attrs attrs = map.Attrs;
                    foreach (Entity myen in ens)
                    {
                        foreach (Attr attr in attrs)
                        {
                            if (attr.IsFKorEnum)
                                continue;

                            if (attr.IsPK)
                                continue;

                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                case DataType.AppInt:
                                case DataType.AppFloat:
                                case DataType.AppDouble:
                                case DataType.AppBoolean:
                                    continue;
                                default:
                                    break;
                            }
                            myen.SetValByKey(attr.Key, myen.GetValStrByKey(attr.Key).Replace(keyVal, "<font color=red>" + keyVal + "</font>"));
                        }
                    }
                }
            }

            this.UCSys1.DataPanelDtl(ens, null);

            int ToPageIdx = this.PageIdx + 1;
            int PPageIdx = this.PageIdx - 1;

            this.UCSys1.Add("<SCRIPT language=javascript>");
            this.UCSys1.Add("\t\n document.onkeydown = chang_page;");
            this.UCSys1.Add("\t\n function chang_page() { ");
            //  this.UCSys3.Add("\t\n  alert(event.keyCode); ");
            if (this.PageIdx == 1)
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 37 || event.keyCode == 33) alert('�Ѿ��ǵ�һҳ');");
            }
            else
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 37  || event.keyCode == 38 || event.keyCode == 33) ");
                this.UCSys1.Add("\t\n     location='Search.aspx?EnsName=" + this.EnsName + "&PageIdx=" + PPageIdx + "';");
            }

            if (this.PageIdx == maxPageNum)
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) alert('�Ѿ������һҳ');");
            }
            else
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) ");
                this.UCSys1.Add("\t\n     location='Search.aspx?EnsName=" + this.EnsName + "&PageIdx=" + ToPageIdx + "';");
            }

            this.UCSys1.Add("\t\n } ");
            this.UCSys1.Add("</SCRIPT>");
            return ens;
        }

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        #endregion

        #region �¼�.
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            try
            {
                var btn = sender as LinkBtn;
                switch (btn.ID)
                {
                    case NamesOfBtn.Insert: //���ݵ���
                        this.Response.Redirect("UIEn.aspx?EnName=" + this.HisEn.ToString(), true);
                        return;
                    case NamesOfBtn.Excel: //���ݵ���
                        Entities ens = this.HisEns;
                        Entity en = ens.GetNewEntity;
                        QueryObject qo = new QueryObject(ens);
                        qo = this.ToolBar1.GetnQueryObject(ens, en);
                        qo.DoQuery();
                        string file = "";
                        try
                        {
                            file = this.ExportDGToExcel(ens.ToDataTableDesc(), this.HisEn.EnDesc);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                file = this.ExportDGToExcel(ens.ToDataTableDescField(), this.HisEn.EnDesc);
                            }
                            catch
                            {
                                throw new Exception("����û����ȷ�������ܵ�ԭ��֮һ��:ϵͳ����Աû��ȷ�İ�װExcel�������֪ͨ�����ο���װ˵��������@ϵͳ�쳣��Ϣ��" + ex.Message);
                            }
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Excel_S: //���ݵ���.
                        Entities ens1 = this.SetDGData();
                        try
                        {
                            this.ExportDGToExcel(ens1.ToDataTableDesc(), this.HisEn.EnDesc);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("����û����ȷ�������ܵ�ԭ��֮һ��:ϵͳ����Աû��ȷ�İ�װExcel�������֪ͨ�����ο���װ˵��������@ϵͳ�쳣��Ϣ��" + ex.Message);
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Xml: //���ݵ���
                        return;
                    case "Btn_Print":  //���ݵ���.
                        return;
                    default:
                        this.PageIdx = 1;
                        this.SetDGData(1);
                        this.ToolBar1.SaveSearchState(this.EnsName, null);
                        return;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    this.ResponseWriteRedMsg(ex);
                }
            }
        }
        private bool Btn_New_ButtonClick(object sender, EventArgs e)
        {
            this.WinOpen("./RefFunc/UIEn.aspx?EnName=" + this.HisEn.ToString());
            this.SetDGData();
            return false;
        }
        #endregion �¼�.
    }
}
