﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using BP.En;
using BP.DA;
using BP.Sys;

namespace BP.Web
{
    /// <summary>
    /// PageBase 的摘要说明。
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {
        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinCloseWithMsg(string mess)
        {
            //this.ResponseWriteRedMsg(mess);
            //return;
            mess = mess.Replace("'", "＇");

            mess = mess.Replace("\"", "＂");

            mess = mess.Replace(";", "；");
            mess = mess.Replace(")", "）");
            mess = mess.Replace("(", "（");

            mess = mess.Replace(",", "，");
            mess = mess.Replace(":", "：");


            mess = mess.Replace("<", "［");
            mess = mess.Replace(">", "］");

            mess = mess.Replace("[", "［");
            mess = mess.Replace("]", "］");


            mess = mess.Replace("@", "\\n@");

            mess = mess.Replace("\r\n", "");

            this.Response.Write("<script language='JavaScript'>alert('" + mess + "'); window.close()</script>");
        }
        public string RefEnKey
        {
            get
            {
                string str = this.Request.QueryString["No"];
                if (str == null)
                    str = this.Request.QueryString["OID"];

                if (str == null)
                    str = this.Request.QueryString["MyPK"];

                if (str == null)
                    str = this.Request.QueryString["PK"];


                return str;
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        public int RefOID
        {
            get
            {
                string s = this.Request.QueryString["RefOID"];
                if (s == null)
                    s = this.Request.QueryString["OID"];
                if (s == null)
                    return 0;
                return int.Parse(s);
            }
        }
        public string GenerTableStr(DataTable dt)
        {
            string str = "<Table id='tb' border=1 >";
            // 标题
            str += "<TR>";
            foreach (DataColumn dc in dt.Columns)
            {
                str += "<TD class='DGCellOfHeader" + BP.Web.WebUser.Style + "' >" + dc.ColumnName + "</TD>";
            }
            str += "</TR>";

            //内容
            foreach (DataRow dr in dt.Rows)
            {
                str += "<TR>";

                foreach (DataColumn dc in dt.Columns)
                {
                    str += "<TD >" + dr[dc.ColumnName] + "</TD>";
                }
                str += "</TR>";
            }
            str += "</Table>";
            return str;
        }
        public string GenerTablePage(DataTable dt, string title)
        {
            return PubClass.GenerTablePage(dt, title);
        }
        public string GenerLabelStr(string title)
        {
            return PubClass.GenerLabelStr(title);
            //return str;
        }

        public Control GenerLabel(string title)
        {
            string path = this.Request.ApplicationPath;
            string str = "";
            str += "<TABLE style='font-size:14px' cellpadding='0' cellspacing='0' background='" + SystemConfig.CCFlowWebPath + "WF/Img/DG_bgright.gif'>";
            str += "<TR>";
            str += "<TD>";
            str += "<IMG src='" + SystemConfig.CCFlowWebPath + "WF/Img/DG_Title_Left.gif' border='0' width='30' height='25'></TD>";

            str += "<TD  valign=bottom noWrap background='" + SystemConfig.CCFlowWebPath + "WF/Img/DG_Title_BG.gif'   height='25' border=0>&nbsp;";
            str += " &nbsp;<b>" + title + "</b>&nbsp;&nbsp;";
            str += "</TD>";
            str += "<TD >";
            str += "<IMG src='" + SystemConfig.CCFlowWebPath + "WF/Img/DG_Title_Right.gif' border='0' width='25' height='25'></TD>";
            str += "</TR>";
            str += "</TABLE>";
            return this.ParseControl(str);
        }
        public Control GenerLabel_bak(string title)
        {
            // return this.ParseControl(title);

            string path = SystemConfig.CCFlowWebPath;//this.Request.ApplicationPath;
            string str = "";

            str += "<TABLE style='font-size:14px'  cellpadding='0' cellspacing='0' background='" + path + "/Images/DG_bgright.gif'>";
            str += "<TBODY>";
            str += "<TR>";
            str += "<TD>";
            str += "<IMG src='" + path + "/Images/DG_Title_Left.gif' border='0' width='30' height='20'></TD>";
            str += "<TD  class=TD  vAlign='center' noWrap background='" + path + "/Images/DG_Title_BG.gif'>&nbsp;";
            str += " &nbsp;" + title + "&nbsp;&nbsp;";
            str += "</TD>";
            str += "<TD>";
            str += "<IMG src='" + path + "WF/Img/DG_Title_Right.gif' border='0' width='25' height='20'></TD>";
            str += "</TR>";
            str += "</TBODY>";
            str += "</TABLE>";
            return this.ParseControl(str);
            //return str;
        }
        public void GenerLabel(Label lab, Entity en, string msg)
        {
            lab.Controls.Clear();
            lab.Controls.Add(this.GenerLabel("<img src='" + en.EnMap.Icon + "' border=0 />" + msg));
        }
        public void GenerLabel(Label lab, string msg)
        {
            lab.Controls.Clear();
            lab.Controls.Add(this.GenerLabel(msg));
        }
        //public void GenerLabel(Label lab, Entity en)
        //{
        //    this.GenerLabel(lab, en.EnDesc + en.EnMap.TitleExt);
        //    return;

        //    lab.Controls.Clear();
        //    if (en.EnMap.Icon == null)
        //        lab.Controls.Add(this.GenerLabel(en.EnMap.EnDesc));
        //    else
        //        lab.Controls.Add(this.GenerLabel("<img src='" + en.EnMap.Icon + "' border=0 />" + en.EnMap.EnDesc + en.EnMap.TitleExt));
        //}
        public string GenerCaption(string title)
        {
            if (BP.Web.WebUser.Style == "2")
                return "<div class=Table_Title ><span>" + title + "</span></div>";

            return "<b>" + title + "</b>";
        }
        protected override void OnLoad(EventArgs e)
        {
            //if (Web.WebUser.No == null)
            //    this.ToSignInPage();
            base.OnLoad(e);
        }
        /// <summary>
        /// 导出到一个excel,文件用于，数据导入。
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        protected void ExportEnToExcelModel_OpenWin(Attrs attrs, string sheetName)
        {
            string filename = sheetName + ".xls";
            string file = filename;
            //SystemConfig.PathOfTemp
            string filepath = SystemConfig.CCFlowWebPath + "\\Temp\\";

            #region 参数及变量设置
            //如果导出目录没有建立，则建立.
            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);

            filename = filepath + filename;
            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            string strLine = "";
            foreach (Attr attr in attrs)
            {
                strLine += attr.Desc + Convert.ToChar(9);
            }

            objStreamWriter.WriteLine(strLine);
            objStreamWriter.Close();
            objFileStream.Close();
            #endregion

            //this.WinOpen(Request.ApplicationPath+"/Temp/" + file,"sss", 500,800);

            this.Write_Javascript(" window.open('" + SystemConfig.CCFlowWebPath + "/Temp/" + file + "'); ");
        }

        #region 用户的访问权限
        /// <summary>
        /// 谁能使用这个页面,他是编号组成的字串。
        /// such as ,admin,jww,002, 
        /// if return value is null, It's mean all emps can visit it . 
        /// </summary>
        protected virtual string WhoCanUseIt()
        {
            return null;
        }
        #endregion

        private void RP(string msg)
        {
            this.Response.Write(msg);
        }
        private void RPBR(string msg)
        {
            this.Response.Write(msg + "<br>");
        }
        public void TableShow(DataTable dt, string title)
        {

            this.RPBR(title);
            this.RPBR("<table border='1' width='100%'>");

        }
        public string GenerCreateTableSQL(string className)
        {
            ArrayList als = ClassFactory.GetObjects(className);
            int u = 0;
            string sql = "";
            foreach (Object obj in als)
            {
                u++;
                try
                {
                    Entity en = (Entity)obj;
                    switch (en.EnMap.EnDBUrl.DBType)
                    {
                        case DBType.Oracle:
                            sql += SqlBuilder.GenerCreateTableSQLOfOra_OK(en) + " \n GO \n";
                            break;
                        case DBType.Informix:
                            sql += SqlBuilder.GenerCreateTableSQLOfInfoMix(en) + " \n GO \n";
                            break;
                        default:
                            sql += SqlBuilder.GenerCreateTableSQLOfMS(en) + "\n GO \n";
                            break;
                    }
                }
                catch
                {
                    continue;
                }
                //Map map=en.EnMap;
                //objStreamWriter.WriteLine(Convert.ToChar(9)+"No:"+u.ToString()+Convert.ToChar(9) +map.EnDesc +Convert.ToChar(9) +map.PhysicsTable+Convert.ToChar(9) +map.EnType);
            }
            Log.DefaultLogWriteLineInfo(sql);
            return sql;
        }


        public void ExportEntityToExcel(string classbaseName)
        {
            #region 文件
            string filename = "DatabaseDesign.xls";
            string file = filename;
            //bool flag = true;
            string filepath = Request.PhysicalApplicationPath + "\\Temp\\";

            //如果导出目录没有建立，则建立.
            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);

            filename = filepath + filename;
            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            //string str="";
            ArrayList als = ClassFactory.GetObjects(classbaseName);
            int i = 0;
            objStreamWriter.WriteLine();
            objStreamWriter.WriteLine(Convert.ToChar(9) + "系统实体[" + classbaseName + "]" + Convert.ToChar(9));
            objStreamWriter.WriteLine();
            //objStreamWriter.WriteLine(Convert.ToChar(9)+"感谢使用系统实体结构自动生成器"+Convert.ToChar(9)+"调用日期"+Convert.ToChar(9)+DateTime.Now.ToString("yyyy年MM月dd日"));
            objStreamWriter.WriteLine(Convert.ToChar(9) + "从" + classbaseName + "继承下来的实体有[" + als.Count + "]个");


            #region 处理目录
            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "系统实体目录");
            objStreamWriter.WriteLine(Convert.ToChar(9) + "序号" + Convert.ToChar(9) + "实体名称" + Convert.ToChar(9) + "物理表/视图" + Convert.ToChar(9) + "类型");
            int u = 0;
            foreach (Object obj in als)
            {
                try
                {
                    u++;
                    Entity en = (Entity)obj;
                    Map map = en.EnMap;
                    objStreamWriter.WriteLine(Convert.ToChar(9) + "No:" + u.ToString() + Convert.ToChar(9) + map.EnDesc + Convert.ToChar(9) + map.PhysicsTable + Convert.ToChar(9) + map.EnType);
                }
                catch
                {
                }
            }
            objStreamWriter.WriteLine();
            #endregion

            foreach (Object obj in als)
            {
                try
                {

                    i++;
                    Entity en = (Entity)obj;
                    Map map = en.EnMap;

                    #region 生成导出文件
                    objStreamWriter.WriteLine("序号" + i);
                    objStreamWriter.WriteLine(Convert.ToChar(9) + "实体名称" + Convert.ToChar(9) + map.EnDesc + Convert.ToChar(9) + "物理表/视图" + Convert.ToChar(9) + map.PhysicsTable + Convert.ToChar(9) + "实体类型" + Convert.ToChar(9) + map.EnType);
                    if (map.CodeStruct == null)
                    {
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "编码结构信息:无");
                    }
                    else
                    {
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "编码结构" + Convert.ToChar(9) + map.CodeStruct + "是否检查编号的长度" + Convert.ToChar(9) + map.IsCheckNoLength);
                    }
                    //objStreamWriter.WriteLine(Convert.ToChar(9)+"物理存放位置"+map.EnDBUrl+Convert.ToChar(9)+"实体内存存放位置"+Convert.ToChar(9)+map.DepositaryOfEntity+Convert.ToChar(9)+"Map 内存存放位置"+Convert.ToChar(9)+map.DepositaryOfMap);
                    objStreamWriter.WriteLine(Convert.ToChar(9) + "物理存放位置" + map.EnDBUrl + Convert.ToChar(9) + "Map 内存存放位置" + Convert.ToChar(9) + map.DepositaryOfMap);
                    objStreamWriter.WriteLine(Convert.ToChar(9) + "访问权限" + Convert.ToChar(9) + "是否查看" + en.HisUAC.IsView + Convert.ToChar(9) + "是否新建" + en.HisUAC.IsInsert + Convert.ToChar(9) + "是否删除" + en.HisUAC.IsDelete + "是否更新" + en.HisUAC.IsUpdate + Convert.ToChar(9) + "是否附件" + en.HisUAC.IsAdjunct);
                    if (map.Dtls.Count > 0)
                    {
                        /* output dtls */
                        EnDtls dtls = map.Dtls;
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "明细/从表信息:个数" + dtls.Count);
                        int ii = 0;
                        foreach (EnDtl dtl in dtls)
                        {
                            ii++;
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "编号:" + ii + "描述:" + dtl.Desc + "关系到的实体类" + dtl.EnsName + "外键" + dtl.RefKey);
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "物理表:" + dtl.Ens.GetNewEntity.EnMap.PhysicsTable);
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "备注:关于" + dtl.Desc + "更详细的信息,请参考" + dtl.EnsName);
                        }
                    }
                    else
                    {
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "明细/从表信息:无");
                    }

                    if (map.AttrsOfOneVSM.Count > 0)
                    {
                        /* output dtls */
                        AttrsOfOneVSM dtls = map.AttrsOfOneVSM;
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "多对多关系:个数" + dtls.Count);
                        int ii = 0;
                        foreach (AttrOfOneVSM dtl in dtls)
                        {
                            ii++;
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "编号:" + ii + "描述:" + dtl.Desc);
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "多对多实体类" + dtl.EnsOfMM.ToString() + "外键" + dtl.AttrOfOneInMM);
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "此实体关联到的外键" + dtl.AttrOfOneInMM);
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "多实体类" + dtl.EnsOfMM.ToString() + "外键" + dtl.AttrOfMValue);
                        }
                    }
                    else
                    {
                        objStreamWriter.WriteLine(Convert.ToChar(9) + "多对多关系:无");
                    }

                    objStreamWriter.WriteLine(Convert.ToChar(9) + "表/视图结构");
                    int iii = 0;
                    objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + "属性序号" + Convert.ToChar(9) + "属性描述" + Convert.ToChar(9) + "属性" + Convert.ToChar(9) + "物理字段" + Convert.ToChar(9) + "数据类型" + Convert.ToChar(9) + "默认值" + Convert.ToChar(9) + "关系类型" + Convert.ToChar(9) + "备注");

                    foreach (Attr attr in map.Attrs)
                    {
                        iii++;
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + iii + Convert.ToChar(9) + attr.Desc + Convert.ToChar(9) + attr.Key + Convert.ToChar(9) + attr.Field + Convert.ToChar(9) + attr.MyDataTypeStr + Convert.ToChar(9) + attr.DefaultVal + Convert.ToChar(9) + "枚举" + Convert.ToChar(9) + "枚举Key" + attr.UIBindKey + " 关于枚举的信息请到Sys_Enum表里找到更详细的信息.");
                            continue;
                        }
                        if (attr.MyFieldType == FieldType.PKEnum)
                        {
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + iii + Convert.ToChar(9) + attr.Desc + Convert.ToChar(9) + attr.Key + Convert.ToChar(9) + attr.Field + Convert.ToChar(9) + attr.MyDataTypeStr + Convert.ToChar(9) + attr.DefaultVal + Convert.ToChar(9) + "主键枚举" + Convert.ToChar(9) + "枚举Key" + attr.UIBindKey + " 关于枚举的信息请到Sys_Enum表里找到更详细的信息.");

                            //objStreamWriter.WriteLine(Convert.ToChar(9)+" "+Convert.ToChar(9)+"No:"+iii+Convert.ToChar(9)+"描述"+Convert.ToChar(9)+attr.Desc+Convert.ToChar(9)+"属性"+Convert.ToChar(9)+attr.Key+Convert.ToChar(9)+"属性默认值"+Convert.ToChar(9)+attr.DefaultVal+Convert.ToChar(9)+"物理字段"+Convert.ToChar(9)+attr.Field+Convert.ToChar(9)+"字段关系类型"+Convert.ToChar(9)+"枚举主键"+Convert.ToChar(9)+"字段数据类型 "+Convert.ToChar(9)+attr.MyDataTypeStr+"");
                            continue;
                        }
                        if (attr.MyFieldType == FieldType.FK)
                        {
                            Entity tmp = attr.HisFKEn;
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + iii + Convert.ToChar(9) + attr.Desc + Convert.ToChar(9) + attr.Key + Convert.ToChar(9) + attr.Field + Convert.ToChar(9) + attr.MyDataTypeStr + Convert.ToChar(9) + attr.DefaultVal + Convert.ToChar(9) + "外键" + Convert.ToChar(9) + "关连的实体:" + tmp.EnDesc + "物理表:" + tmp.EnMap.PhysicsTable + " 关于" + tmp.EnDesc + "信息请到此实体信息里面去找.");

                            //objStreamWriter.WriteLine(Convert.ToChar(9)+" "+Convert.ToChar(9)+"No:"+iii+Convert.ToChar(9)+"描述"+Convert.ToChar(9)+attr.Desc+Convert.ToChar(9)+"属性"+Convert.ToChar(9)+attr.Key+Convert.ToChar(9)+"属性默认值"+Convert.ToChar(9)+attr.DefaultVal+Convert.ToChar(9)+"物理字段"+Convert.ToChar(9)+attr.Field+Convert.ToChar(9)+"字段关系类型"+Convert.ToChar(9)+"外键"+Convert.ToChar(9)+"字段数据类型 "+Convert.ToChar(9)+attr.MyDataTypeStr+""+"关系到的实体名称"+Convert.ToChar(9)+tmp.EnDesc+"物理表"+Convert.ToChar(9)+tmp.EnMap.PhysicsTable+Convert.ToChar(9)+"更详细的信息请参考"+Convert.ToChar(9));
                            continue;
                        }
                        if (attr.MyFieldType == FieldType.PKFK)
                        {
                            Entity tmp = attr.HisFKEn;
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + iii + Convert.ToChar(9) + attr.Desc + Convert.ToChar(9) + attr.Key + Convert.ToChar(9) + attr.Field + Convert.ToChar(9) + attr.MyDataTypeStr + Convert.ToChar(9) + attr.DefaultVal + Convert.ToChar(9) + "外主键" + Convert.ToChar(9) + "关连的实体:" + tmp.EnDesc + "物理表:" + tmp.EnMap.PhysicsTable + " 关于" + tmp.EnDesc + "信息请到此实体信息里面去找.");
                            continue;
                        }

                        //其他的情况.
                        if (attr.MyFieldType == FieldType.Normal || attr.MyFieldType == FieldType.PK)
                        {
                            objStreamWriter.WriteLine(Convert.ToChar(9) + " " + Convert.ToChar(9) + iii + Convert.ToChar(9) + attr.Desc + Convert.ToChar(9) + attr.Key + Convert.ToChar(9) + attr.Field + Convert.ToChar(9) + attr.MyDataTypeStr + Convert.ToChar(9) + attr.DefaultVal + Convert.ToChar(9) + "普通" + Convert.ToChar(9) + attr.EnterDesc);
                            //objStreamWriter.WriteLine(Convert.ToChar(9)+" "+Convert.ToChar(9)+"No:"+iii+Convert.ToChar(9)+"描述"+Convert.ToChar(9)+attr.Desc+Convert.ToChar(9)+"属性"+Convert.ToChar(9)+attr.Key+Convert.ToChar(9)+"属性默认值"+Convert.ToChar(9)+attr.DefaultVal+Convert.ToChar(9)+"物理字段"+Convert.ToChar(9)+attr.Field+Convert.ToChar(9)+"字段关系类型"+Convert.ToChar(9)+"字符"+Convert.ToChar(9)+"字段数据类型"+Convert.ToChar(9)+attr.MyDataTypeStr+""+Convert.ToChar(9)+"输入要求"+Convert.ToChar(9)+attr.EnterDesc);
                            continue;
                        }
                        //objStreamWriter.WriteLine("属性序号:"+iii+Convert.ToChar(9)+"描述"+Convert.ToChar(9)+attr.Desc+Convert.ToChar(9)+"属性"+Convert.ToChar(9)+attr.Key+Convert.ToChar(9)+"属性默认值"+Convert.ToChar(9)+attr.DefaultVal+Convert.ToChar(9)+"物理字段"+Convert.ToChar(9)+attr.Field+"字段关系类型"+Convert.ToChar(9)+"字符"+Convert.ToChar(9)+"字段数据类型"+Convert.ToChar(9)+attr.MyDataTypeStr+Convert.ToChar(9)+""+"输入要求"+Convert.ToChar(9)+attr.EnterDesc+Convert.ToChar(9));
                    }
                }
                catch
                {
                }
            }
            objStreamWriter.WriteLine();
            objStreamWriter.WriteLine(Convert.ToChar(9) + Convert.ToChar(9) + " " + Convert.ToChar(9) + Convert.ToChar(9) + " 制表人：" + Convert.ToChar(9) + WebUser.Name + Convert.ToChar(9) + "日期：" + Convert.ToChar(9) + DateTime.Now.ToShortDateString());

            objStreamWriter.Close();
            objFileStream.Close();

            this.Write_Javascript(" window.open('" + SystemConfig.CCFlowWebPath + "/Temp/" + file + "'); ");


                    #endregion



        }
        public void Helper(string htmlFile)
        {
            this.WinOpen(htmlFile);
        }

        public void Helper()
        {
            this.WinOpen(SystemConfig.CCFlowWebPath + "/" + SystemConfig.AppSettings["PageOfHelper"]);
        }
        /// <summary>
        /// 取得属性by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetRequestStrByKey(string key)
        {
            return this.Request.QueryString[key];
        }

        #region 操作方法
        /// <summary>
        /// showmodaldialog 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        protected void ShowModalDialog(string url, string title, int Height, int Width)
        {
            string script = "<script language='JavaScript'>window.showModalDialog('" + url + "','','dialogHeight: " + Height.ToString() + "px; dialogWidth: " + Width.ToString() + "px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); </script> ";

            //this.RegisterStartupScript("key1s",script); // old .
            ClientScript.RegisterStartupScript(this.GetType(), "K1", script); // new 

            //this.Response.Write( script );
            //this.RegisterClientScriptBlock("Dia",script);
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close();</script>");
        }
        protected void WinClose(string val)
        {
            //经测试谷歌,IE都走window.top.returnValue 方法
            string clientscript = "<script language='javascript'> if(window.opener != undefined){window.top.returnValue = '" + val + "';} else { window.returnValue = '" + val + "';} window.close(); </script>";

            if (Request.QueryString["inlayer"] == "1")
            {
                clientscript = "<script type='text/javascript'>var reval = '" + val + "';window.parent.closeDlg();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "csg", clientscript);
                return;
            }
            //string clientscript = "<script language='javascript'>  window.returnValue = '" + val + "'; window.close(); </script>";
            this.Page.Response.Write(clientscript);
        }
        /// <summary>
        /// 打开一个新的窗口
        /// </summary>
        /// <param name="msg"></param>
        protected void WinOpen(string url)
        {
            this.WinOpen(url, "", "msg", 900, 500);
        }
        protected string dealUrl(string url)
        {
            if (url.IndexOf("?") == -1)
            {
                //url=url.Substring(0,url.IndexOf("",""));
                return url;
            }
            else
            {
                return url;
            }
        }
        protected void WinOpen(string url, string title, string winName, int width, int height)
        {
            this.WinOpen(url, title, winName, width, height, 0, 0);
        }
        protected void WinOpen(string url, string title, int width, int height)
        {
            this.WinOpen(url, title, "ActivePage", width, height, 0, 0);
        }
        protected void WinOpen(string url, string title, string winName, int width, int height, int top, int left)
        {
            WinOpen(url, title, winName, width, height, top, left, false, false);
        }
        protected void WinOpen(string url, string title, string winName, int width, int height, int top, int left, bool _isShowToolBar, bool _isShowAddress)
        {
            url = url.Replace("<", "[");
            url = url.Replace(">", "]");
            url = url.Trim();
            title = title.Replace("<", "[");
            title = title.Replace(">", "]");
            title = title.Replace("\"", "‘");
            string isShowAddress = "no", isShowToolBar = "no";
            if (_isShowAddress)
                isShowAddress = "yes";
            if (_isShowToolBar)
                isShowToolBar = "yes";
            // this.Response.Write("<script language='JavaScript'> var newWindow =window.showModelessDialog('" + url + "','" + winName + "','width=" + width + "px,top=" + top + "px,left=" + left + "px,height=" + height + "px,scrollbars=yes,resizable=yes,toolbar=" + isShowToolBar + ",location=" + isShowAddress + "'); newWindow.focus(); </script> ");
            this.Response.Write("<script language='JavaScript'> var newWindow =window.open('" + url + "','" + winName + "','width=" + width + "px,top=" + top + "px,left=" + left + "px,height=" + height + "px,scrollbars=yes,resizable=yes,toolbar=" + isShowToolBar + ",location=" + isShowAddress + "'); newWindow.focus(); </script> ");

        }
        //private int MsgFontSize=1;
        /// <summary>
        /// 输出到页面上红色的警告。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteRedMsg(string msg)
        {
            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + WebUser.No] = msg;

            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/ErrorPage.aspx?d=" + DateTime.Now.ToString();
            this.WinOpen(url, "警告", "errmsg", 500, 400, 150, 270);
        }
        protected void ResponseWriteShowModalDialogRedMsg(string msg)
        {
            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;

            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/ErrorPage.aspx?d=" + DateTime.Now.ToString();
            this.WinOpenShowModalDialog(url, "警告", "msg", 500, 400, 120, 270);
        }
        protected void ResponseWriteShowModalDialogBlueMsg(string msg)
        {
            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;

            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString();
            this.WinOpenShowModalDialog(url, "提示", "msg", 500, 400, 120, 270);
        }

        protected void WinOpenShowModalDialog(string url, string title, string key, int width, int height, int top, int left)
        {
            //url=this.Request.ApplicationPath+"Comm/ShowModalDialog.htm?"+url;
            //this.RegisterStartupScript(key,"<script language='JavaScript'>window.showModalDialog('"+url+"','"+key+"' ,'dialogHeight: 500px; dialogWidth:"+width+"px; dialogTop: "+top+"px; dialogLeft: "+left+"px; center: yes; help: no' ) ;  </script> ");

            this.ClientScript.RegisterStartupScript(this.GetType(), key, "<script language='JavaScript'>window.showModalDialog('" + url + "','" + key + "' ,'dialogHeight: 500px; dialogWidth:" + width + "px; dialogTop: " + top + "px; dialogLeft: " + left + "px; center: yes; help: no' ) ;  </script> ");

        }
        protected void WinOpenShowModalDialogResponse(string url, string title, string key, int width, int height, int top, int left)
        {
            url = this.Request.ApplicationPath + "Comm/ShowModalDialog.htm?" + url;
            this.Response.Write("<script language='JavaScript'>window.showModalDialog('" + url + "','" + key + "' ,'dialogHeight: 500px; dialogWidth:" + width + "px; dialogTop: " + top + "px; dialogLeft: " + left + "px; center: yes; help: no' ) ;  </script> ");
        }

        protected void ResponseWriteRedMsg(Exception ex)
        {
            this.ResponseWriteRedMsg(ex.Message);
        }
        /// <summary>
        /// 输出到页面上蓝色的信息。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteBlueMsg(string msg)
        {
            msg = msg.Replace("@", "<br>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + WebUser.No] = msg;
            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString();
            this.WinOpen(url, "信息", "d" + this.Session.SessionID, 500, 300, 150, 270);
        }

        protected void AlertHtmlMsg(string msg)
        {
            if (DataType.IsNullOrEmpty(msg))
                return;

            msg = msg.Replace("@", "<br>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            string url = "MsgPage.aspx?d=" + DateTime.Now.ToString();
            this.WinOpen(url, "信息", this.Session.SessionID, 500, 400, 150, 270);
        }
        /// <summary>
        /// 保存成功
        /// </summary>
        protected void ResponseWriteBlueMsg_SaveOK()
        {
            Alert("保存成功！", false);
        }
        /// <summary>
        /// 保存成功
        /// </summary>
        /// <param name="num">记录个数。</param>
        protected void ResponseWriteBlueMsg_SaveOK(int num)
        {
            Alert("共计[" + num + "]条记录保存成功！", false);
        }
        /// <summary>
        /// ResponseWriteBlueMsg_DeleteOK
        /// </summary>
        protected void ResponseWriteBlueMsg_DeleteOK()
        {

            this.Alert("删除成功！", false);
            //
            //更新成功
            //			//this.Alert("删除成功!");
            //			ResponseWriteBlueMsg("删除成功!");
        }
        /// <summary>
        /// "共计["+delNum+"]条记录删除成功！"
        /// </summary>
        /// <param name="delNum">delNum</param>
        protected void ResponseWriteBlueMsg_DeleteOK(int delNum)
        {
            //this.Alert("删除成功!");
            this.Alert("共计[" + delNum + "]条记录删除成功！", false);

        }
        /// <summary>
        /// ResponseWriteBlueMsg_UpdataOK
        /// </summary>
        protected void ResponseWriteBlueMsg_UpdataOK()
        {
            //this.ResponseWriteBlueMsg("更新成功",false);
            this.Alert("更新成功!");
            // ResponseWriteBlueMsg("更新成功!");
        }
        protected void ToSignInPage()
        {
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.PageOfLostSession);
        }
        protected void ToWelPage()
        {
            System.Web.HttpContext.Current.Response.Redirect(BP.Sys.Glo.Request.ApplicationPath + "/Wel.aspx");
        }
        protected void ToErrorPage(Exception mess)
        {
            this.ToErrorPage(mess.Message);
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        protected void ToErrorPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/ToErrorPage.aspx?d=" + DateTime.Now.ToString(), false);
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        protected void ToCommMsgPage(string mess)
        {
            mess = mess.Replace("@", "<BR>@");
            mess = mess.Replace("~", "@");

            System.Web.HttpContext.Current.Session["info"] = mess;
            if (SystemConfig.AppSettings["PageMsg"] == null)
                System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString(), false);
            else
                System.Web.HttpContext.Current.Response.Redirect(SystemConfig.AppSettings["PageMsg"] + "?d=" + DateTime.Now.ToString(), false);
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        protected void ToWFMsgPage(string mess)
        {
            mess = mess.Replace("@", "<BR>@");
            mess = mess.Replace("~", "@");

            System.Web.HttpContext.Current.Session["info"] = mess;
            if (SystemConfig.AppSettings["PageMsg"] == null)
                System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString(), false);
            else
                System.Web.HttpContext.Current.Response.Redirect(SystemConfig.AppSettings["PageMsg"] + "?d=" + DateTime.Now.ToString(), false);
        }
        protected void ToMsgPage_Do(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString(), false);
        }
        #endregion

        /// <summary>
        ///转到一个页面上。 '_top'
        /// </summary>
        /// <param name="mess"></param>
        /// <param name="target">'_top'</param>
        protected void ToErrorPage(string mess, string target)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx target='_top'");
        }

        /// <summary>
        /// 窗口的OnInit事件，自动在页面上加一下记录当前行的Hidden
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //ShowRuning();
            base.OnInit(e);

            if (this.WhoCanUseIt() != null)
            {
                if (this.WhoCanUseIt() == WebUser.No)
                    return;
                if (this.WhoCanUseIt().IndexOf("," + WebUser.No + ",") == -1)
                    this.ToErrorPage("您没有权限访问这个页面。");
            }




        }

        #region 与控件有关系的操作
        public void ShowDataTable(DataTable dt)
        {
            this.Response.Write(this.DataTable2Html(dt, true));
        }
        /// <summary>
        /// 显示DataTable.
        /// </summary>
        public string DataTable2Html(DataTable dt, bool isShowTitle)
        {
            string str = "";
            if (isShowTitle)
            {
                str = dt.TableName + " 合计:" + dt.Rows.Count + "记录.";
            }
            str += "<Table>";
            str += "<TR>";
            foreach (DataColumn dc in dt.Columns)
            {
                str += "  <TD warp=false >";
                str += dc.ColumnName;
                str += "  </TD>";
            }
            str += "</TR>";


            foreach (DataRow dr in dt.Rows)
            {
                str += "<TR>";

                foreach (DataColumn dc in dt.Columns)
                {
                    str += "  <TD>";
                    str += dr[dc.ColumnName];
                    str += "  </TD>";
                }
                str += "</TR>";
            }

            str += "</Table>";
            return str;

            //this.ResponseWriteBlueMsg(str);


        }
        /// <summary>
        /// 显示运行
        /// </summary>
        public void ShowRuning()
        {
            //if (this.IsPostBack==false)
            //	return ;		


            string str = "<script language=javascript><!-- function showRuning() {	sending.style.visibility='visible' } --> </script>";


            // if (!this.IsClientScriptBlockRegistered("ClientProxyScript"))
            //   this.RegisterClientScriptBlock("ClientProxyScript", str);

            if (!this.ClientScript.IsClientScriptBlockRegistered("ClientProxyScript"))
                this.ClientScript.RegisterStartupScript(this.GetType(), "ClientProxyScript", str);

            if (this.IsPostBack == false)
            {
                str = "<div id='sending' style='position: absolute; top: 126; left: -25; z-index: 10; visibility: hidden; width: 903; height: 74'><TABLE WIDTH=100% BORDER=0 CELLSPACING=0 CELLPADDING=0><TR><td width=30%></td><TD bgcolor=#ff9900><TABLE WIDTH=100% height=70 BORDER=0 CELLSPACING=2 CELLPADDING=0><TR><td bgcolor=#eeeeee align=center>系统正在相应您的请求, 请稍候...</td></tr></table></td><td width=30%></td></tr></table></div> ";
                this.Response.Write(str);
            }
        }

        #endregion

        #region 图片属性

        /// <summary>
        /// 是否要检查功能
        /// </summary>
        protected bool IsCheckFunc
        {
            get
            {
                //if (this.SubPageMessage==null || this.SubPageTitle==null) 
                //return false;

                if (ViewState["IsCheckFunc"] != null)
                    return (bool)ViewState["IsCheckFunc"];
                else
                    return true;

            }
            set { ViewState["IsCheckFunc"] = value; }
        }


        #endregion

        #region 关于session 操作。

        public static object GetSessionObjByKey(string key)
        {
            object val = System.Web.HttpContext.Current.Session[key];
            return val;
        }
        public static string GetSessionByKey(string key)
        {
            return (string)GetSessionObjByKey(key);
        }
        /// <summary>
        /// 取出来字符串中的 Key1:val1;Key2:val2;  值. 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public static string GetSessionByKey(string key1, string key2)
        {
            string str = GetSessionByKey(key1);
            if (str == null)
                throw new Exception("没有取到" + key1 + "的值.");

            string[] strs = str.Split(';');
            foreach (string s in strs)
            {
                string[] ss = s.Split(':');
                if (ss[0] == key2)
                    return ss[1];
            }
            return null;
        }
        public static void SetSessionByKey(string key, object obj)
        {
            System.Web.HttpContext.Current.Session[key] = obj;
        }
        public static void SetSessionByKey(string key1, string key2, object obj)
        {
            string str = GetSessionByKey(key1);
            string KV = key2 + ":" + obj.ToString() + ";";
            if (str == null)
            {
                SetSessionByKey(key1, KV);
                return;
            }



            string[] strs = str.Split(';');
            foreach (string s in strs)
            {
                string[] ss = s.Split(':');
                if (ss[0] == key2)
                {
                    SetSessionByKey(key1, str.Replace(s + ";", KV));
                    return;
                }
            }

            SetSessionByKey(key1, str + KV);
        }
        #endregion

        #region 对于 ViewState 的操作。
        /// <summary>
        /// 设置 ViewState Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="DefaultVal"></param>
        public void SetValueByKey(string key, object val, object DefaultVal)
        {
            if (val == null)
                ViewState[key] = DefaultVal;
            else
                ViewState[key] = val;
        }
        public void SetValueByKey(string key, object val)
        {
            ViewState[key] = val;
        }
        /// <summary>
        /// 取出Val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByKey(string key)
        {
            try
            {
                return ViewState[key].ToString();
            }
            catch
            {
                return null;
            }
        }
        public bool GetValueByKeyBool(string key)
        {
            if (this.GetValueByKey(key) == "1")
                return true;
            return false;
        }
        /// <summary>
        /// ss
        /// </summary>
        /// <param name="key">ss</param>
        /// <param name="DefaultVal">ss</param>
        /// <returns></returns>
        public string GetValueByKey_del(string key, string DefaultVal)
        {
            try
            {
                return ViewState[key].ToString();
            }
            catch
            {
                return DefaultVal;
            }
        }
        /// <summary>
        /// 按照key 取出来,bool 的植. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public bool GetBoolValusByKey_del(string key, bool DefaultValue)
        {
            try
            {
                return bool.Parse(this.GetValueByKey(key));
            }
            catch
            {
                return DefaultValue;
            }
        }
        /// <summary>
        /// 取出int valus , 如果没有就返回 DefaultValue ;
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetIntValueByKey_del(string key, int DefaultValue)
        {
            try
            {
                return int.Parse(ViewState[key].ToString());
            }
            catch
            {
                return DefaultValue;
            }
        }
        #endregion



        /// <summary>
        /// 这个table 是用来处理页面上的DataGride. 
        /// </summary>
        protected System.Data.DataTable Table
        {
            get
            {
                //DataTable dt = (System.Data.DataTable)ViewState["Table"];
                DataTable dt = (System.Data.DataTable)ViewState["Table"];
                if (dt == null)
                    dt = new DataTable();
                return dt;
            }
            set
            {
                ViewState["Table"] = value;
            }
        }
        protected System.Data.DataTable Table_bak
        {
            get
            {
                //DataTable dt = (System.Data.DataTable)ViewState["Table"];
                DataTable dt = this.Session["Table"] as DataTable;
                if (dt == null)
                    dt = new DataTable();
                return dt;
            }
            set
            {
                this.Session["Table"] = value;
            }
        }
        protected System.Data.DataTable Table1
        {
            get
            {

                DataTable dt = (System.Data.DataTable)ViewState["Table1"];
                if (dt == null)
                    dt = new DataTable();
                return dt;
            }
            set
            {
                ViewState["Table1"] = value;
            }
        }
        /// <summary>
        /// 应用程序主键
        /// </summary>
        protected string PK
        {
            get
            {
                try
                {
                    return ViewState["PK"].ToString();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                ViewState["PK"] = value;
            }
        }
        /// <summary>
        /// 用来保存状态。
        /// </summary>
        protected bool IsNew_del
        {
            get
            {
                try
                {
                    return (bool)ViewState["IsNew"];
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                ViewState["IsNew"] = value;
            }
        }
        /// <summary>
        /// PKOID if is null return 0 
        /// </summary>
        protected int PKint
        {
            get
            {
                try
                {
                    return int.Parse(ViewState["PKint"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                ViewState["PKint"] = value;
            }
        }
        //		protected void ShowMessage(string msg)
        //		{
        //			PubClass.ShowMessage(msg);
        //		}		
        //		protected void ShowMessage_SaveOK()
        //		{
        //			PubClass.ShowMessageMSG_SaveOK();
        //		}
        protected void ShowMessage_SaveUnsuccessful()
        {
            //PubClass.ShowMessage(msg);
        }

        //		protected void ShowMessage_UpdateSuccessful()
        //		{
        //			PubClass.ShowMessage("更新成功！");
        //		}
        protected void ShowMessage_UpdateUnsuccessful()
        {
            //PubClass.ShowMessage(msg);
        }
        protected void Write_Javascript(string script)
        {
            script = script.Replace("<", "[");
            script = script.Replace(">", "]");
            Response.Write("<script language=javascript> " + script + " </script>");
        }
        protected void ShowMessageWin(string url)
        {
            this.Response.Write("<script language='JavaScript'> window.open('" + url + "')</script>");
        }
        protected void Alert(string mess)
        {
            if (DataType.IsNullOrEmpty(mess))
                return;

            this.Alert(mess, false);
        }
        /// <summary>
        /// 不用page 参数，show message
        /// </summary>
        /// <param name="mess"></param>
        protected void Alert(string mess, bool isClent)
        {
            if (DataType.IsNullOrEmpty(mess))
                return;

            //this.ResponseWriteRedMsg(mess);
            //return;
            mess = mess.Replace("'", "＇");

            mess = mess.Replace("\"", "＂");

            mess = mess.Replace(";", "；");
            mess = mess.Replace(")", "）");
            mess = mess.Replace("(", "（");

            mess = mess.Replace(",", "，");
            mess = mess.Replace(":", "：");


            mess = mess.Replace("<", "［");
            mess = mess.Replace(">", "］");

            mess = mess.Replace("[", "［");
            mess = mess.Replace("]", "］");


            mess = mess.Replace("@", "\\n@");

            mess = mess.Replace("\r\n", "");

            string script = "<script language=JavaScript>alert('" + mess + "');</script>";
            if (isClent)
                System.Web.HttpContext.Current.Response.Write(script);
            else
                this.ClientScript.RegisterStartupScript(this.GetType(), "kesy", script);
            //this.RegisterStartupScript("key1", script);
        }

        protected void Alert(Exception ex)
        {
            this.Alert(ex.Message, false);
        }
        #region 公共的方法

        #region 报表导出的问题
        /// <summary>
        /// 根据DataTable内容导出到Excel中  
        /// </summary>
        /// <param name="dt">要导出内容的DataTable</param>
        /// <param name="filepath">要产生的文件路径</param>
        /// <param name="filename">要产生的文件</param>
        /// <returns></returns>
        protected bool ExportDataTableToExcel_OpenWin_del(System.Data.DataTable dt, string title)
        {
            string filename = "Ep" + this.Session.SessionID + ".xls";
            string file = filename;
            bool flag = true;
            string filepath = SystemConfig.PathOfTemp;

            #region 处理 datatable
            foreach (DataColumn dc in dt.Columns)
            {
                switch (dc.ColumnName)
                {
                    case "No":
                        dc.Caption = "编号";
                        break;
                    case "Name":
                        dc.Caption = "名称";
                        break;
                    case "Total":
                        dc.Caption = "合计";
                        break;
                    case "FK_Dept":
                        dc.Caption = "部门编号";
                        break;
                    case "ZSJGName":
                        dc.Caption = "部门名称";
                        break;
                    case "IncNo":
                        dc.Caption = "纳税人编号";
                        break;
                    case "IncName":
                        dc.Caption = "纳税人名称";
                        break;
                    case "TaxpayerNo":
                        dc.Caption = "纳税人编号";
                        break;
                    case "TaxpayerName":
                        dc.Caption = "纳税人名称";
                        break;
                    case "byrk":
                        dc.Caption = "本月入库";
                        break;
                    case "ljrk":
                        dc.Caption = "累计入库";
                        break;
                    case "qntq":
                        dc.Caption = "去年同期";
                        break;
                    case "jtqzje":
                        dc.Caption = "较去年增减额";
                        break;
                    case "jtqzjl":
                        dc.Caption = "较去年增减率";
                        break;
                    case "BenYueYiJiao":
                        dc.Caption = "本月已缴";
                        break;
                    case "BenYueYingJiao":
                        dc.Caption = "本月应缴";
                        break;
                    case "BenYueWeiJiao":
                        dc.Caption = "本月未缴";
                        break;
                    case "LeiJiWeiJiao":
                        dc.Caption = "累计未缴";
                        break;
                    case "QuNianTongQiLeiJiYiJiao":
                        dc.Caption = "去年同期未缴";
                        break;

                    case "QianNianTongQiLeiJiYiJiao":
                        dc.Caption = "前年同期累计已缴";
                        break;
                    case "QianNianTongQiLeiJiYingJiao":
                        dc.Caption = "前年同期累计应缴";
                        break;

                    case "JiaoQuNianTongQiZhengJian":
                        dc.Caption = "较去年同期增减";
                        break;
                    case "JiaoQuNianTongQiZhengJianLv":
                        dc.Caption = "较去年同期增减率";
                        break;

                    case "JiaoQianNianTongQiZhengJian":
                        dc.Caption = "较去年同期增减";
                        break;
                    case "JiaoQianNianTongQiZhengJianLv":
                        dc.Caption = "较前年同期增减率";
                        break;
                    case "LeiJiYiJiao":
                        dc.Caption = "累计已缴";
                        break;
                    case "LeiJiYingJiao":
                        dc.Caption = "累计应缴";
                        break;
                    case "QuNianBenYueYiJiao":
                        dc.Caption = "去年本月已缴";
                        break;
                    case "QuNianBenYueYingJiao":
                        dc.Caption = "去年本月应缴";
                        break;
                    case "QuNianLeiJiYiJiao":
                        dc.Caption = "去年累计已缴";
                        break;
                    case "QuNianLeiJiYingJiao":
                        dc.Caption = "去年累计应缴";
                        break;
                    case "QianNianBenYueYiJiao":
                        dc.Caption = "前年本月已缴";
                        break;
                    case "QianNianBenYueYingJiao":
                        dc.Caption = "前年本月应缴";
                        break;
                    case "QianNianLeiJiYiJiao":
                        dc.Caption = "前年同期累计已缴";
                        break;
                    case "QianNianLeiJiYingJiao":
                        dc.Caption = "前年同期累计应缴";
                        break;
                    case "JiaoQuNianZhengJian":
                        dc.Caption = "较去年同期增减";
                        break;
                    case "JiaoQuNianZhengJianLv":
                        dc.Caption = "较去年同期增减率";
                        break;
                    case "JiaoQianNianZhengJian":
                        dc.Caption = "较前年同期增减";
                        break;
                    case "JiaoQianNianZhengJianLv":
                        dc.Caption = "较前年同期增减率";
                        break;
                    case "level":
                        dc.Caption = "级次";
                        break;
                }
            }
            #endregion

            #region 参数及变量设置
            //参数校验
            if (dt == null || dt.Rows.Count <= 0 || filename == null || filename == "" || filepath == null || filepath == "")
                return false;

            //如果导出目录没有建立，则建立
            if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

            filename = filepath + filename;

            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            try
            {
                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + title + Convert.ToChar(9));
                objStreamWriter.WriteLine();
                string strLine = "";
                //生成文件标题
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strLine = strLine + dt.Columns[i].Caption + Convert.ToChar(9);
                }

                objStreamWriter.WriteLine(strLine);

                strLine = "";

                //生成文件内容
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        strLine = strLine + dt.Rows[row][col] + Convert.ToChar(9);
                    }
                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }
                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + "制表人：" + Convert.ToChar(9) + WebUser.Name + Convert.ToChar(9) + "日期：" + Convert.ToChar(9) + DateTime.Now.ToShortDateString());

            }
            catch
            {
                flag = false;
            }
            finally
            {
                objStreamWriter.Close();
                objFileStream.Close();
            }
            #endregion

            #region 删除掉旧的文件
            DelExportedTempFile(filepath);
            #endregion


            if (flag)
            {
                this.WinOpen("../Temp/" + file);
                //this.Write_Javascript(" window.open( ); " );
            }

            return flag;
        }
        /// <summary>
        /// 删除掉导出时产生的临时文件 2002.11.09 create by bluesky 
        /// </summary>
        /// <param name="filepath">临时文件路径</param>
        /// <returns></returns>
        public bool DelExportedTempFile(string filepath)
        {
            bool flag = true;
            try
            {
                string[] files = Directory.GetFiles(filepath);

                for (int i = 0; i < files.Length; i++)
                {
                    DateTime lastTime = File.GetLastWriteTime(files[i]);
                    TimeSpan span = DateTime.Now - lastTime;

                    if (span.Hours >= 1)
                        File.Delete(files[i]);
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        #endregion 报表导出


        #endregion

    }
}
