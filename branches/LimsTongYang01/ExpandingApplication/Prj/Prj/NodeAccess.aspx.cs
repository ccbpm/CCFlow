using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;
using BP.En;
using BP.PRJ;
using BP.Web;

public partial class ExpandingApplication_PRJ_NodeRuleUI : WebPage
{
    public string FK_Prj
    {
        get
        {
            string s = this.Request.QueryString["FK_Prj"];
            if (s == null)
                s = "0001";
            return s;
        }
    }
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    public string FK_NodeStr
    {
        get
        {
            return this.Request.QueryString["FK_Node"];
        }
    }
    public int FK_Node
    {
        get
        {
            return int.Parse(this.Request.QueryString["FK_Node"]);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (this.FK_NodeStr != null)
        {
            this.BindNodeRule();
            return;
        }

        if (this.FK_Flow == null)
        {
            Flows fls = new Flows();
            fls.Retrieve(FlowAttr.FlowType, (int)FlowAppType.PRJ);
            Flow fl = fls[0] as Flow;
            this.Response.Redirect("NodeAccess.aspx?FK_Flow=" + fl.No + "&FK_Prj=" + this.FK_Prj, true);
            return;
        }
        this.BindLeft();
        this.BindRight();
    }
    public void BindLeft()
    {
        FlowSorts sorts = new FlowSorts();
        sorts.RetrieveAll();
        Flows fls = new Flows();
        fls.Retrieve(FlowAttr.FlowType, (int)FlowAppType.PRJ);

        foreach (FlowSort fs in sorts)
        {
            this.Pub1.AddB(fs.Name);
            this.Pub1.AddBR();
            this.Pub1.AddUL();
            foreach (Flow fl in fls)
            {
                if (fl.FK_FlowSort != fs.No)
                    continue;

                if (this.FK_Flow == fl.No)
                    this.Pub1.AddLi("<a href='NodeAccess.aspx?FK_Flow=" + fl.No + "&FK_Prj=" + this.FK_Prj + "' ><b>" + fl.Name + "</b></a>");
                else
                    this.Pub1.AddLi("<a href='NodeAccess.aspx?FK_Flow=" + fl.No + "&FK_Prj=" + this.FK_Prj + "' >" + fl.Name + "</a>");
            }
            this.Pub1.AddULEnd();
        }
    }
    public void BindRight()
    {
        this.PubTitle.AddCaptionLeft("资料树节点访问控制");
        Nodes nds = new Nodes(this.FK_Flow);
        this.Pub2.AddUL();
        foreach (Node nd in nds)
        {
            this.Pub2.AddLi("<a href=\"javascript:window.showModalDialog('NodeAccess.aspx?FK_Node=" + nd.NodeID + "&FK_Prj=" + this.FK_Prj + "', 'ds', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');\" >" + nd.Name + "</a>");
        }
        this.Pub2.AddULEnd();
    }
    public void BindNodeRule()
    {
        NodeAccesss nrs = new NodeAccesss();
        nrs.Retrieve(NodeAccessAttr.FK_Node, this.FK_Node, NodeAccessAttr.FK_Prj, this.FK_Prj);

        string root = BP.SystemConfig.PathOfDataUser + "\\PrjData\\Templete\\" + this.FK_Prj;

        this.Pub3.AddTable("width=100%");
        this.Pub3.AddCaptionLeft("节点与资料树访问权限 - <a href=\"javascript:AddDir('" + this.FK_Flow + "','"+this.FK_Node+"')\" >增加文件夹</a>");
        string[] dirs = System.IO.Directory.GetDirectories(root);
        int idx = 0;
        foreach (string dir in dirs)
        {
            idx++;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            this.Pub3.AddTR();
            this.Pub3.AddTDIdx(idx);
            this.Pub3.AddTD("colspan=5", "<img src='./../Images/Btn/open.gif'>" + dirInfo.Name);
            this.Pub3.AddTD("<a href=\"javascript:AddFile('" + idx + "','" + this.FK_Flow + "','" + this.FK_Node + "')\" >增加模板</a>");
            this.Pub3.AddTREnd();
            FileInfo[] fls = dirInfo.GetFiles();
            foreach (FileInfo fl in fls)
            {
                idx++;
                this.Pub3.AddTR();
                this.Pub3.AddTDIdx(idx);
                this.Pub3.AddTD("<img src='./../Images/FileType/" + fl.Extension.Replace(".", "") + ".gif'>" + fl.Name);
                NodeAccess rl = nrs.GetEntityByKey(NodeAccessAttr.FileFullName, fl.FullName) as NodeAccess;
                if (rl == null)
                {
                    rl = new NodeAccess();
                    rl.FileFullName = fl.FullName;
                    rl.FileName = fl.Name;
                    rl.FK_Node = this.FK_Node;
                }

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + NodeAccessAttr.IsView + "_" + idx;
                cb.Text = "可见";
                cb.Checked = rl.IsView;
                this.Pub3.AddTD(cb);

                cb = new CheckBox();
                cb.ID = "CB_" + NodeAccessAttr.IsDown + "_" + idx;
                cb.Text = "可下载";
                cb.Checked = rl.IsDown;
                this.Pub3.AddTD(cb);

                cb = new CheckBox();
                cb.ID = "CB_" + NodeAccessAttr.IsUpload + "_" + idx;
                cb.Text = "可上传";
                cb.Checked = rl.IsUpload;
                this.Pub3.AddTD(cb);

                cb = new CheckBox();
                cb.ID = "CB_" + NodeAccessAttr.IsDelete + "_" + idx;
                cb.Text = "可删除";
                cb.Checked = rl.IsDelete;
                this.Pub3.AddTD(cb);

                this.Pub3.AddTD("<a href=\"javascript:DelFile('" + idx + "','" + this.FK_Flow + "','" + this.FK_Node + "')\" >删除</a>");
                this.Pub3.AddTREnd();
            }
        }
        this.Pub3.AddTableEnd();

        this.Pub3.AddHR();
        Button btn = new Button();
        btn.Text = " Save  ";
        btn.ID = "Btn_Save";
        btn.Click += new EventHandler(btn_Templete_Click);
        this.Pub3.Add(btn);
    }
    void btn_Templete_Click(object sender, EventArgs e)
    {
        NodeAccesss nrs = new NodeAccesss();
        nrs.Retrieve(NodeAccessAttr.FK_Node, this.FK_Node, NodeAccessAttr.FK_Prj, this.FK_Prj);

        string root = BP.SystemConfig.PathOfDataUser + "\\PrjData\\Templete\\" + this.FK_Prj;
        this.Pub3.AddTable();
        this.Pub3.AddCaptionLeft("节点与资料树访问权限");
        string[] dirs = System.IO.Directory.GetDirectories(root);
        int idx = 0;
        foreach (string dir in dirs)
        {
            idx++;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileInfo[] fls = dirInfo.GetFiles();
            foreach (FileInfo fl in fls)
            {
                idx++;
                NodeAccess rl = new NodeAccess();
                rl = new NodeAccess();
                rl.FileFullName = fl.FullName;
                rl.FileName = fl.Name;
                rl.FK_Node = this.FK_Node;
                rl.FK_Prj = this.FK_Prj;

                rl.IsView = this.Pub3.GetCBByID("CB_" + NodeAccessAttr.IsView + "_" + idx).Checked;
                rl.IsDown = this.Pub3.GetCBByID("CB_" + NodeAccessAttr.IsDown + "_" + idx).Checked;
                rl.IsUpload = this.Pub3.GetCBByID("CB_" + NodeAccessAttr.IsUpload + "_" + idx).Checked;
                rl.IsDelete = this.Pub3.GetCBByID("CB_" + NodeAccessAttr.IsDelete + "_" + idx).Checked;
                rl.MyPK =DataType.ParseStringToPinyin( rl.FileName) + "_" + this.FK_Node + "_" + this.FK_Prj;
                rl.Save();
            }
        }
        this.WinCloseWithMsg("Save OK.");
    }
}