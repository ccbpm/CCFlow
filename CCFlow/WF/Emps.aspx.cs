using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.IO;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_Emps : BP.Web.WebPage
    {
        public void GenerAllImg()
        {
            BP.WF.Port.WFEmps empWFs = new BP.WF.Port.WFEmps();
            empWFs.RetrieveAll();

            foreach (BP.WF.Port.WFEmp emp in empWFs)
            {
                if (System.IO.File.Exists(BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.No + ".JPG")
                    || System.IO.File.Exists(BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.Name + ".JPG"))
                {
                    continue;
                }

                try
                {
                    string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\T.JPG";
                    string pathMe = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.No + ".JPG";
                    File.Copy(BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\Templete.JPG",
                        path, true);


                    string fontName = "宋体";
                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                    Font font = new Font(fontName, 15);
                    Graphics g = Graphics.FromImage(img);
                    System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat(StringFormatFlags.DirectionVertical);//文本
                    g.DrawString(emp.Name, font, drawBrush, 3, 3);

                    try
                    {
                        File.Delete(pathMe);
                    }
                    catch
                    {
                    }
                    img.Save(pathMe);
                    img.Dispose();
                    g.Dispose();

                    File.Copy(pathMe,
                    BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.Name + ".JPG", true);
                }
                catch
                {

                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "Empleyes";
            if (WebUser.IsWap)
            {
                this.BindWap();
                return;
            }

            string sql = "SELECT a.No,a.Name, b.Name as DeptName FROM Port_Emp a, Port_Dept b WHERE a.FK_Dept=b.No ORDER BY a.FK_Dept ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            BP.WF.Port.WFEmps emps = new BP.WF.Port.WFEmps();
            if (this.DoType != null)
                emps.RetrieveAllFromDBSource();
            else
                emps.RetrieveAllFromDBSource();

            this.Pub1.AddTable("width=100% align=left");
            this.Pub1.AddCaptionMsg("通讯录");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("部门");
            this.Pub1.AddTDTitle("人员");
            this.Pub1.AddTDTitle("Tel");
            this.Pub1.AddTDTitle("Email");
            this.Pub1.AddTDTitle("岗位"); // <a href=Emps.aspx?DoType=1>刷新</a> ");
            this.Pub1.AddTDTitle("签名");
            if (WebUser.No == "admin")
                this.Pub1.AddTDTitle("顺序");

            if (this.DoType != null)
            {
                BP.WF.Port.WFEmp.DTSData();
                this.GenerAllImg();
            }
            this.Pub1.AddTREnd();

            string keys = DateTime.Now.ToString("MMddhhmmss");
            string deptName = null;
            int idx = 0;

            EmpStations ess = new EmpStations();
            ess.RetrieveAll();

            foreach (DataRow dr in dt.Rows)
            {
                string fk_emp = dr["No"].ToString();
                if (fk_emp == "admin")
                    continue;

                idx++;
                if (dr["DeptName"].ToString() != deptName)
                {
                    deptName = dr["DeptName"].ToString();
                    this.Pub1.AddTRSum();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(deptName);
                }
                else
                {
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD();
                }

                this.Pub1.AddTD(fk_emp + "-" + dr["Name"]);

                BP.WF.Port.WFEmp emp = emps.GetEntityByKey(fk_emp) as BP.WF.Port.WFEmp;
                if (emp != null)
                {
                    //this.Pub1.AddTD(emp.TelHtml);
                    //this.Pub1.AddTD(emp.EmailHtml);

                    this.Pub1.AddTD();
                    this.Pub1.AddTD();

                    string stas = "";
                    foreach (EmpStation es in ess)
                    {
                        if (es.FK_Emp != emp.No)
                            continue;
                        stas += es.FK_StationT + ",";
                    }
                    this.Pub1.AddTD(stas);
                }
                else
                {
                    this.Pub1.AddTD("");
                    this.Pub1.AddTD("");
                    this.Pub1.AddTD("");
                    //break;
                }

                this.Pub1.AddTD("<img src='../DataUser/Siganture/" + fk_emp + ".jpg' border=1 onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"/>");
                if (WebUser.No == "admin" && emp!=null)
                {
                    this.Pub1.AddTD("<a href=\"javascript:DoUp('" + emp.No + "','" + keys + "')\" ><img src='Img/Btn/Up.gif' border=0 /></a>-<a href=\"javascript:DoDown('" + emp.No + "','" + keys + "')\" ><img src='Img/Btn/Down.gif' border=0 /></a>");
                }
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        public void BindWap()
        {
            this.Pub1.AddTable("align=left width=100%");
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=4 align=left class=FDesc", "<a href='Home.aspx'><img src='Img/Home.gif' border=0/>Home</a> - 成员");
            this.Pub1.AddTREnd();

            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAllFromDBSource();

            BP.WF.Port.WFEmps emps = new BP.WF.Port.WFEmps();
            emps.RetrieveAllFromDBSource();

            int idx = 0;
            foreach (BP.Port.Dept dept in depts)
            {
                this.Pub1.AddTRSum();
                this.Pub1.AddTD("colspan=4", dept.Name);
                this.Pub1.AddTREnd();
                foreach (BP.WF.Port.WFEmp emp in emps)
                {
                    if (emp.FK_Dept != dept.No)
                        continue;

                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(emp.Name);
                    this.Pub1.AddTD(emp.Tel);
                    this.Pub1.AddTD("");
                    //this.Pub1.AddTD(emp.Stas);
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }
    }
}