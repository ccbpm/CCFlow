using System;
using System.Data;
using BP.WF;
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using BP.Sys;
using System.IO;
namespace CCFlow.WF.Admin.XAP
{
    public partial class DoPort : BP.Web.WebPage
    {
        #region 属性。
        public new string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public new string PK
        {
            get
            {
                string s = this.Request.QueryString["PK"];
                if (s.Contains("ND0") == true)
                    s = s.Replace("ND00", "ND");
                if (s.Contains("ND0") == true)
                    s = s.Replace("ND00", "ND");
                return s;
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string PassKey
        {
            get
            {
                return this.Request.QueryString["PassKey"];
            }
        }
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Browser.Cookies == false)
            {
                //this.Response.Write("您的浏览器不支持cookies功能，无法使用该系统。");
                //return;
            }

            if (this.PassKey != BP.Sys.SystemConfig.AppSettings["PassKey"])
                return;

            //if (this.Lang == null || this.Lang == "")
              // throw new Exception("语言编号错误。");

         //   BP.Sys.SystemConfig.DoClearCash();
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp, "CH");

            string fk_flow = this.Request.QueryString["FK_Flow"];
            string fk_Node = this.Request.QueryString["FK_Node"];

            string FK_MapData = this.Request.QueryString["FK_MapData"];
            if (string.IsNullOrEmpty(FK_MapData))
                FK_MapData = this.Request.QueryString["PK"];

            switch (this.DoType)
            {
                case "DownFormTemplete":
                    DataSet ds =BP.Sys.CCFormAPI.GenerHisDataSet(FK_MapData);
                    BP.Sys.MapData md = new BP.Sys.MapData(FK_MapData);
                    string file = BP.Sys.SystemConfig.PathOfTemp + md.No + ".xml";
                    ds.WriteXml(file);
                    System.IO.FileInfo f = new System.IO.FileInfo(file);
                    BP.Sys.PubClass.DownloadFile(f.FullName, md.Name + ".xml");
                    this.Pub1.AddFieldSet("下载提示");

                    string url = "../../../Temp/" + md.No + ".xml";
                    this.Pub1.AddH2("ccflow 已经完成模板的生成了，正在执行下载如果您的浏览器没有反应请<a href='" + url + "' >点这里进行下载</a>。");
                    this.Pub1.Add("如果该xml文件是在ie里直接打开的，请把鼠标放在连接上右键目标另存为，保存该模板。");
                    this.Pub1.AddFieldSetEnd();
                    return;
                case "Ens": // 实体编辑.
                    this.Response.Redirect("../../Comm/Batch.aspx?EnsName=" + this.EnsName, true);
                    break;
                case "En": // 单个实体编辑. 
                    switch (this.EnName)
                    {
                        case "BP.WF.Flow":
                            Flow fl = new Flow(this.PK);
                            if (fl.FlowAppType == FlowAppType.DocFlow)
                                this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.FlowDocs&No=" + this.PK, true);
                            else
                                this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.FlowSheets&No=" + this.PK, true);
                            return;
                        case "BP.WF.Template.FlowSheet":
                        case "BP.WF.Template.Ext.FlowSheet":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.FlowSheets&No=" + this.PK, true);
                            return;
                        case "BP.WF.Template.Ext.NodeExts":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.NodeExts&No=" + this.PK, true);
                            return;
                        case "BP.WF.Rpt.MapRptExts":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Rpt.MapRptExts&PK="+this.PK, true);
                            return;
                        case "BP.WF.Node":
                            Node nd = new Node(this.PK);
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.NodeSheets&PK=" + this.PK, true);
                            return;
                        case "BP.WF.Template.FlowExt":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.FlowExts&PK=" + this.PK, true);
                            return;
                        case "BP.WF.Template.NodeExt":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.NodeExts&PK=" + this.PK, true);
                            return;
                        case "BP.WF.Template.FlowSort":
                            this.Response.Redirect("../../Comm/En.htm?EnsName=BP.WF.Template.FlowSorts&PK=" + this.PK, true);
                            return;
                        default:
                            throw new Exception("err");
                            //this.Response.Redirect("./Comm/En.htm?EnsName=" + this.EnsName + "&No=" + this.PK, true);
                    }
                case "FrmLib": //"表单库"
                    this.Response.Redirect("../Sln/BindFrms.htm?ShowType=SelectedFrm&FK_Flow=" + fk_flow + "&FK_Node=" + fk_Node + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "FlowFrms": //"独立表单"
                    this.Response.Redirect("../Sln/BindFrms.htm?ShowType=FlowFrms&FK_Flow=" + fk_flow + "&FK_Node=" + fk_Node + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "StaDef": // 节点岗位.
                    this.Response.Redirect("../../Comm/RefFunc/Dot2Dot.aspx?EnName=BP.WF.Template.NodeSheet&AttrKey=BP.WF.Template.NodeStations&PK=" + this.PK + "&NodeID=" + this.PK + "&RunModel=0&FLRole=0&FJOpen=0&r=" + this.PK, true);
                    break;
                case "StaDefNew": // 新版本.. http://localhost:13432/WF/Comm/RefFunc/Dot2DotSingle.aspx
                    this.Response.Redirect("../../Comm/RefFunc/Dot2DotSingle.aspx?EnsName=BP.WF.Template.NodeSheets&EnName=BP.WF.Template.NodeSheet&AttrKey=BP.WF.Template.NodeStations&NodeID=" + this.PK + "&r=0319061642&1=FK_StationType&ShowWay=None" + this.PK, true);
                    break;
                case "WFRpt": // 报表设计.r
                    //杨玉慧 点击报表设计改成打开该流程的报表列表
                    //this.Response.Redirect("../../Rpt/OneFlow.htm?FK_MapData=ND" + int.Parse(this.PK) + "Rpt&FK_Flow=" + this.PK, true);
                    string rptUrl = "../../Comm/En.htm?EnsName=BP.WF.Rpt.RptDfines&No="+this.PK;
                    this.Response.Redirect(rptUrl, true);
                    break;
                case "MapDef": //表单定义.
                    int nodeid = int.Parse(this.PK.Replace("ND", ""));
                    Node nd1 = new Node();
                    nd1.NodeID = nodeid;
                    nd1.RetrieveFromDBSources();
                    if (nd1.HisFormType == NodeFormType.FreeForm)
                        this.Response.Redirect("../FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.PK + "&FK_Flow=" + nd1.FK_Flow + "&FK_Node=" + this.FK_Node, true);
                    else
                        this.Response.Redirect("../FoolFormDesigner/Designer.htm?PK=" + this.PK + "&FK_Flow=" + nd1.FK_Flow + "&FK_Node=" + this.FK_Node, true);
                    break;
                case "MapDefFixModel": // 表单定义.
                case "FormFixModel":
                    this.Response.Redirect("../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + FK_MapData + "&FK_Flow=" + this.FK_Flow+"&FK_Node="+this.FK_Node, true);
                    break;
                case "MapDefFreeModel": // 表单定义.
                case "FormFreeModel":
                    this.Response.Redirect("../FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + FK_MapData + "&FK_Flow=" + this.FK_Flow+"&FK_Node="+this.FK_Node, true);
                    break;
                case "MapDefFree": //表单定义.
                    int nodeidFree = int.Parse(this.PK.Replace("ND", ""));
                    Node ndFree = new Node(nodeidFree);
                    this.Response.Redirect("../FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.PK + "&FK_Flow=" + ndFree.FK_Flow + "&FK_Node=" + ndFree.NodeID, true);
                    break;
                case "MapDefF4": //表单定义.
                    int nodeidF4 = int.Parse(this.PK.Replace("ND", ""));
                    Node ndF4 = new Node(nodeidF4);
                    this.Response.Redirect("../FoolFormDesigner/Designer.htm?PK=" + this.PK + "&FK_Flow=" + ndF4.FK_Flow + "&FK_Node=" + nodeidF4, true);
                    break;
                case "Dir": // 方向。
                    this.Response.Redirect("../Admin/Cond/ConditionLine.htm?CondType=" + this.Request.QueryString["CondType"] + "&FK_Flow=" + this.Request.QueryString["FK_Flow"] + "&FK_MainNode=" + this.Request.QueryString["FK_MainNode"] + "&FK_Node=" + this.Request.QueryString["FK_Node"] + "&FK_Attr=" + this.Request.QueryString["FK_Attr"] + "&DirType=" + this.Request.QueryString["DirType"] + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case "RunFlow": //运行流程
                    this.Response.Redirect("../Admin/StartFlow.aspx?FK_Flow=" + fk_flow + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "FlowCheck": // 流程设计
                    this.Response.Redirect("../Admin/DoType.aspx?RefNo=" + this.Request.QueryString["RefNo"] + "&DoType=" + this.DoType, true);
                    break;
                case "ExpFlowTemplete": //流程设计.
                    Flow flow = new Flow(FK_Flow);
                    string fileXml = flow.GenerFlowXmlTemplete();

                    BP.Sys.PubClass.DownloadFile(fileXml, flow.Name + ".xml");
                    BP.Sys.PubClass.WinClose();
                    break;

                case "UploadShare":
                    upload();
                    break;
                case "ShareToFtp":
                   // BP.Web.Common.ShareToFtp();
                    break;
                default:
                    throw new Exception("Error:" + this.DoType);
            }
        }


        /// <summary>
        /// 备份当前流程到用户xml文件
        /// 用户每次保存时调用
        /// </summary>
        public static void WriteToXmlMapData(string FK_MapData, bool savePrtSC)
        {
            try
            {
                string path = string.Empty, xmlName = string.Empty, ext = ".xml";
                if (!string.IsNullOrEmpty(FK_MapData))
                {
                    if (FK_MapData.StartsWith("ND"))
                    {// 节点表单
                        int nodeNo = int.Parse(FK_MapData.Substring(2, FK_MapData.Length - 2));

                        string nodeName = "", FlowNo = "", FlowName = "";
                        string sql = "SELECT n.Name NodeName,FK_Flow FlowNo,f.Name FlowName FROM WF_Node n,WF_Flow f where NodeID ='{0}' and n.FK_Flow = f.No";
                        sql = string.Format(sql, nodeNo);
                        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            nodeName = dt.Rows[0]["NodeName"].ToString();
                            FlowNo = dt.Rows[0]["FlowNo"].ToString();
                            FlowName = dt.Rows[0]["FlowName"].ToString();
                        }

                        nodeName = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(nodeName);
                        FlowName = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(FlowName);

                        path = BP.Sys.SystemConfig.PathOfDataUser + @"FlowDesc\"
                            + FlowNo + "." + FlowName + "\\";
                        xmlName = nodeNo + "." + nodeName;
                    }
                    else
                    { // 独立表单
                        string sql = "SELECT Name from sys_MapData WHERE No ='{0}'";
                        sql = string.Format(sql, FK_MapData);
                        xmlName = BP.DA.DBAccess.RunSQLReturnString(sql);
                        xmlName = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(xmlName);
                        path = BP.Sys.SystemConfig.PathOfDataUser + @"FlowDesc\FlowForm\" + xmlName + "\\";// 独立表单
                    }
                }

                if (!string.IsNullOrEmpty(path))
                {
                    string file = path + xmlName + ext;

                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    else if (System.IO.File.Exists(file))
                    {
                        DateTime time = System.IO.File.GetLastWriteTime(file);
                        string xmlNameOld = path + xmlName + time.ToString("@yyyyMMddHHmmss") + ext;

                        //把文件重命名.
                        if (File.Exists(xmlNameOld))
                            File.Delete(xmlNameOld);
                        System.IO.File.Move(file, xmlNameOld);

                    }

                    if (savePrtSC)
                    {
                        file = path + xmlName + ".png";
                        uploadPng(file);
                    }
                    else
                    {
                        DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(FK_MapData);
                        if (!string.IsNullOrEmpty(file) && null != ds)
                            ds.WriteXml(file);
                    }
                }
            }
            catch (Exception e)
            {
                BP.DA.Log.DefaultLogWriteLineError("表单文件备份错误:"+e.Message);
            }
        }


        public const string FlowTemplate = "FlowTemplate";
        public static void ShareToFtp()
        {
            string flowNo = HttpContext.Current.Request.QueryString["FK_Flow"];
            string bbsNo = HttpContext.Current.Request.QueryString["BBS"];
            string sharTo = HttpContext.Current.Request.QueryString["ShareTo"];
            Flow flow = new Flow(flowNo);
            string ip = "online.ccflow.org";
            string userNo = "ccflowlover";
            string pass = "ccflowlover";
            try
            {
                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection(ip, userNo, pass);
                //conn.SetCurrentDirectory("/");
                conn.SetCurrentDirectory(FlowTemplate + "\\" + sharTo + "\\");

                string createDir = bbsNo + "." + flow.No + "." + flow.Name;
                if (!conn.DirectoryExist(createDir))
                {
                    conn.CreateDirectory(createDir);
                }
                conn.SetCurrentDirectory(createDir + "\\");

                HttpContext.Current.Response.ContentType = "text/plain";

                string dir = BP.Sys.SystemConfig.PathOfDataUser + @"\FlowDesc\" + flow.No + "." + flow.Name + "\\";
                if (System.IO.Directory.Exists(dir))
                {

                    string[] fls = System.IO.Directory.GetFiles(dir);
                    foreach (string fff in fls)
                    {
                        string fileName = fff.Substring(fff.LastIndexOf("\\") + 1);
                        if (fileName.Contains("@"))// 历史数据不上传
                            continue;

                        conn.PutFile(fff, fileName);
                        //conn.DeleteFile(fileName);
                    }

                    //上传成功
                    HttpContext.Current.Response.Write("上传成功");
                }
                else
                {
                    HttpContext.Current.Response.Write("该流程暂没有可发布文件");
                }
                conn.Close();
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write("流程模板发布失败：" + e.Message);
            }
        }

        public static void upload()
        {
            string path = string.Empty, pngName = string.Empty;

            string FK_Flow = HttpContext.Current.Request["FK_Flow"];
            string FK_MapData = HttpContext.Current.Request["FK_MapData"];
            if (!string.IsNullOrEmpty(FK_Flow))
            {// 共享流程
                Flow flow = new Flow(FK_Flow);

                path = BP.Sys.SystemConfig.PathOfDataUser + @"FlowDesc\" + flow.No + "." + flow.Name + "\\";
                pngName = path + "Flow.png";

                if (!string.IsNullOrEmpty(path) && !System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                if (!string.IsNullOrEmpty(pngName))
                    uploadPng(pngName);
            }
            else if (!string.IsNullOrEmpty(FK_MapData))
            {// 共享表单
                WriteToXmlMapData(FK_MapData, true);
            }
        }

        /// <summary>
        /// 图片压缩后保存
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool uploadPng(string fileName)
        {
            //获取上传的数据流
            ImageCodecInfo ici = null;
            Bitmap bmp = null;
            Encoder ecd = null;
            EncoderParameter ept = null;
            EncoderParameters eptS = null;
            try
            {
                // 获取图片编码类型信息
                string coderType = @"image/png";
                ImageCodecInfo[] iciS = ImageCodecInfo.GetImageEncoders();
                foreach (ImageCodecInfo cc in iciS)
                {
                    if (cc.MimeType.Equals(coderType))
                    { ici = cc; break; }
                }

                ecd = System.Drawing.Imaging.Encoder.Quality;
                eptS = new EncoderParameters(1);
                ept = new EncoderParameter(ecd, 10L);
                eptS.Param[0] = ept;
                bmp = new Bitmap(HttpContext.Current.Request.InputStream);
                bmp.Save(fileName, ici, eptS);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
        }
    }

}