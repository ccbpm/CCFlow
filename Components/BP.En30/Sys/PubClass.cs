using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.IO.Compression;
using System.Text;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.Text.RegularExpressions;
using BP.Port;
using System.Collections.Generic;

namespace BP.Sys
{
    /// <summary>
    /// PageBase 的摘要说明。
    /// </summary>
    public class PubClass
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="maillAddr">地址</param>
        /// <param name="title">标题</param>
        /// <param name="doc">内容</param>
        public static void SendMail(string maillAddr, string title, string doc)
        {
            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
            myEmail.From = new System.Net.Mail.MailAddress("ccflow.cn@gmail.com", "ccflow", System.Text.Encoding.UTF8);

            myEmail.To.Add(maillAddr);
            myEmail.Subject = title;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

            myEmail.Body = doc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件

            myEmail.Priority = MailPriority.High;//邮件优先级

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(SystemConfig.GetValByKey("SendEmailAddress", "ccflow.cn@gmail.com"),
                SystemConfig.GetValByKey("SendEmailPass", "ccflow123"));

            //上述写你的邮箱和密码
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 587); //使用的端口
            client.Host = SystemConfig.GetValByKey("SendEmailHost", "smtp.gmail.com");
            client.EnableSsl = true; //经过ssl加密.
            object userState = myEmail;
            try
            {
                client.Send(myEmail);

                //   client.SendAsync(myEmail, userState);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
        }
        public static string ToHtmlColor(string colorName)
        {
            try
            {
                if (colorName.StartsWith("#"))
                {
                    colorName = colorName.Replace("#", string.Empty);
                    //update by dgq 六位颜色不需要转换
                    if (colorName.Length == 6)
                        return "#" + colorName;
                }
                int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);

                Color col = Color.FromArgb
               (
                     Convert.ToByte((v >> 24) & 255),
                     Convert.ToByte((v >> 16) & 255),
                     Convert.ToByte((v >> 8) & 255),
                     Convert.ToByte((v >> 0) & 255)
                );

                int alpha = col.A;
                var red = Convert.ToString(col.R, 16); ;
                var green = Convert.ToString(col.G, 16);
                var blue = Convert.ToString(col.B, 16);
                return string.Format("#{0}{1}{2}", red, green, blue);
            }
            catch
            {
                return "black";
            }
        }
        public static void InitFrm(string fk_mapdata)
        {
            // 删除数据.
            FrmLabs labs = new FrmLabs();
            labs.Delete(FrmLabAttr.FK_MapData, fk_mapdata);

            FrmLines lines = new FrmLines();
            lines.Delete(FrmLabAttr.FK_MapData, fk_mapdata);

            MapData md = new MapData();
            md.No = fk_mapdata;
            if (md.RetrieveFromDBSources() == 0)
            {
                MapDtl mdtl = new MapDtl();
                mdtl.No = fk_mapdata;
                if (mdtl.RetrieveFromDBSources() == 0)
                {
                    throw new Exception("@对:" + fk_mapdata + "的映射信息不存在.");
                }
                else
                {
                    md.Copy(mdtl);
                }
            }

            MapAttrs mattrs = new MapAttrs(fk_mapdata);
            GroupFields gfs = new GroupFields(fk_mapdata);

            int tableW = 700;
            int padingLeft = 3;
            int leftCtrlX = 700 / 100 * 20;
            int rightCtrlX = 700 / 100 * 60;

            string keyID = DateTime.Now.ToString("yyMMddhhmmss");
            // table 标题。
            int currX = 0;
            int currY = 0;
            FrmLab lab = new FrmLab();
            lab.Text = md.Name;
            lab.FontSize = 20;
            lab.X = 200;
            currY += 30;
            lab.Y = currY;
            lab.FK_MapData = fk_mapdata;
            lab.FontWeight = "Bold";
            lab.MyPK = "Lab" + keyID + "1";
            lab.Insert();

            // 表格头部的横线.
            currY += 20;
            FrmLine lin = new FrmLine();
            lin.X1 = 0;
            lin.X2 = tableW;
            lin.Y1 = currY;
            lin.Y2 = currY;
            lin.BorderWidth = 2;
            lin.FK_MapData = fk_mapdata;
            lin.MyPK = "Lin" + keyID + "1";
            lin.Insert();
            currY += 5;

            bool isLeft = false;
            int i = 2;
            foreach (GroupField gf in gfs)
            {
                i++;
                lab = new FrmLab();
                lab.X = 0;
                lab.Y = currY;
                lab.Text = gf.Lab;
                lab.FK_MapData = fk_mapdata;
                lab.FontWeight = "Bold";
                lab.MyPK = "Lab" + keyID + i.ToString();
                lab.Insert();

                currY += 15;
                lin = new FrmLine();
                lin.X1 = padingLeft;
                lin.X2 = tableW;
                lin.Y1 = currY;
                lin.Y2 = currY;
                lin.FK_MapData = fk_mapdata;
                lin.BorderWidth = 3;
                lin.MyPK = "Lin" + keyID + i.ToString();
                lin.Insert();

                isLeft = true;
                int idx = 0;
                foreach (MapAttr attr in mattrs)
                {
                    if (gf.OID != attr.GroupID || attr.UIVisible == false)
                        continue;

                    idx++;
                    if (isLeft)
                    {
                        lin = new FrmLine();
                        lin.X1 = 0;
                        lin.X2 = tableW;
                        lin.Y1 = currY;
                        lin.Y2 = currY;
                        lin.FK_MapData = fk_mapdata;
                        lin.MyPK = "Lin" + keyID + i.ToString() + idx;
                        lin.Insert();
                        currY += 14; /* 画一横线 .*/

                        lab = new FrmLab();
                        lab.X = lin.X1 + padingLeft;
                        lab.Y = currY;
                        lab.Text = attr.Name;
                        lab.FK_MapData = fk_mapdata;
                        lab.MyPK = "Lab" + keyID + i.ToString() + idx;
                        lab.Insert();

                        lin = new FrmLine();
                        lin.X1 = leftCtrlX;
                        lin.Y1 = currY - 14;

                        lin.X2 = leftCtrlX;
                        lin.Y2 = currY;
                        lin.FK_MapData = fk_mapdata;
                        lin.MyPK = "Lin" + keyID + i.ToString() + idx + "R";
                        lin.Insert(); /*画一 竖线 */

                        attr.X = leftCtrlX + padingLeft;
                        attr.Y = currY - 3;
                        attr.UIWidth = 150;
                        attr.Update();
                        currY += 14;
                    }
                    else
                    {
                        currY = currY - 14;
                        lab = new FrmLab();
                        lab.X = tableW / 2 + padingLeft;
                        lab.Y = currY;
                        lab.Text = attr.Name;
                        lab.FK_MapData = fk_mapdata;
                        lab.MyPK = "Lab" + keyID + i.ToString() + idx;
                        lab.Insert();

                        lin = new FrmLine();
                        lin.X1 = tableW / 2;
                        lin.Y1 = currY - 14;

                        lin.X2 = tableW / 2;
                        lin.Y2 = currY;
                        lin.FK_MapData = fk_mapdata;
                        lin.MyPK = "Lin" + keyID + i.ToString() + idx;
                        lin.Insert(); /*画一 竖线 */

                        lin = new FrmLine();
                        lin.X1 = rightCtrlX;
                        lin.Y1 = currY - 14;
                        lin.X2 = rightCtrlX;
                        lin.Y2 = currY;
                        lin.FK_MapData = fk_mapdata;
                        lin.MyPK = "Lin" + keyID + i.ToString() + idx + "R";
                        lin.Insert(); /*画一 竖线 */

                        attr.X = rightCtrlX + padingLeft;
                        attr.Y = currY - 3;
                        attr.UIWidth = 150;
                        attr.Update();
                        currY += 14;
                    }
                    isLeft = !isLeft;
                }
            }
            // table bottom line.
            lin = new FrmLine();
            lin.X1 = 0;
            lin.Y1 = currY;

            lin.X2 = tableW;
            lin.Y2 = currY;
            lin.FK_MapData = fk_mapdata;
            lin.BorderWidth = 3;
            lin.MyPK = "Lin" + keyID + "eR";
            lin.Insert();

            currY = currY - 28 - 18;
            // 处理结尾. table left line
            lin = new FrmLine();
            lin.X1 = 0;
            lin.Y1 = 50;
            lin.X2 = 0;
            lin.Y2 = currY;
            lin.FK_MapData = fk_mapdata;
            lin.BorderWidth = 3;
            lin.MyPK = "Lin" + keyID + "eRr";
            lin.Insert();

            // table right line.
            lin = new FrmLine();
            lin.X1 = tableW;
            lin.Y1 = 50;
            lin.X2 = tableW;
            lin.Y2 = currY;
            lin.FK_MapData = fk_mapdata;
            lin.BorderWidth = 3;
            lin.MyPK = "Lin" + keyID + "eRr4";
            lin.Insert();
        }
        public static String ColorToStr(System.Drawing.Color color)
        {
            try
            {
                string color_s = System.Drawing.ColorTranslator.ToHtml(color);
                color_s = color_s.Substring(1, color_s.Length - 1);
                return "#" + Convert.ToString(Convert.ToInt32(color_s, 16) + 40000, 16);
            }
            catch
            {
                return "black";
            }
        }
        /// <summary>
        /// 处理字段
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        public static string DealToFieldOrTableNames(string fd)
        {
            string keys = "~!@#$%^&*()+{}|:<>?`=[];,./～！＠＃￥％……＆×（）——＋｛｝｜：“《》？｀－＝［］；＇，．／";
            char[] cc = keys.ToCharArray();
            foreach (char c in cc)
                fd = fd.Replace(c.ToString(), "");
            string s = fd.Substring(0, 1);
            try
            {
                int a = int.Parse(s);
                fd = "F" + fd;
            }
            catch
            {
            }
            return fd;
        }
        private static string _KeyFields = null;
        public static string KeyFields
        {
            get
            {
                if (_KeyFields == null)
                    _KeyFields = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfWebApp + SystemConfig.CCFlowWebPath + "WF\\Data\\Sys\\FieldKeys.txt");
                return _KeyFields;
            }
        }
        public static bool IsNum(string str)
        {
            Boolean strResult;
            String cn_Regex = @"^[\u4e00-\u9fa5]+$";
            if (Regex.IsMatch(str, cn_Regex))
            {
                strResult = true;
            }
            else
            {
                strResult = false;
            }
            return strResult;
        }

        public static bool IsCN(string str)
        {
            Boolean strResult;
            String cn_Regex = @"^[\u4e00-\u9fa5]+$";
            if (Regex.IsMatch(str, cn_Regex))
            {
                strResult = true;
            }
            else
            {
                strResult = false;
            }
            return strResult;
        }

        public static bool IsImg(string ext)
        {
            ext = ext.Replace(".", "").ToLower();
            switch (ext)
            {
                case "gif":
                    return true;
                case "jpg":
                    return true;
                case "bmp":
                    return true;
                case "png":
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 按照比例数小
        /// </summary>
        /// <param name="ObjH">目标高度</param>
        /// <param name="factH">实际高度</param>
        /// <param name="factW">实际宽度</param>
        /// <returns>目标宽度</returns>
        public static int GenerImgW_del(int ObjH, int factH, int factW, int isZeroAsWith)
        {
            if (factH == 0 || factW == 0)
                return isZeroAsWith;

            decimal d = decimal.Parse(ObjH.ToString()) / decimal.Parse(factH.ToString()) * decimal.Parse(factW.ToString());

            try
            {
                return int.Parse(d.ToString("0"));
            }
            catch (Exception ex)
            {
                throw new Exception(d.ToString() + ex.Message);
            }
        }

        /// <summary>
        /// 按照比例数小
        /// </summary>
        /// <param name="ObjH">目标高度</param>
        /// <param name="factH">实际高度</param>
        /// <param name="factW">实际宽度</param>
        /// <returns>目标宽度</returns>
        public static int GenerImgH(int ObjW, int factH, int factW, int isZeroAsWith)
        {
            if (factH == 0 || factW == 0)
                return isZeroAsWith;

            decimal d = decimal.Parse(ObjW.ToString()) / decimal.Parse(factW.ToString()) * decimal.Parse(factH.ToString());

            try
            {
                return int.Parse(d.ToString("0"));
            }
            catch (Exception ex)
            {
                throw new Exception(d.ToString() + ex.Message);
            }
        }


        public static string FilesViewStr(string enName, object pk)
        {
            string url = SystemConfig.CCFlowWebPath + "WF/Comm/FileManager.aspx?EnsName=" + enName + "&PK=" + pk.ToString();
            string strs = "";
            //SysFileManagers ens = new SysFileManagers(enName, pk.ToString());
            //string path = BP.Sys.Glo.Request.ApplicationPath;

            //foreach (SysFileManager file in ens)
            //{
            //    strs += "<img src='" + SystemConfig.CCFlowWebPath + "WF/Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='" + path + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a>&nbsp;";
            //    if (file.Rec == WebUser.No)
            //    {
            //        strs += "<a title='打开它' href=\"javascript:DoAction('" + SystemConfig.CCFlowWebPath + "WF/Comm/Do.aspx?ActionType=1&OID=" + file.OID + "&EnsName=" + enName + "&PK=" + pk + "','删除文件《" + file.MyFileName + file.MyFileExt + "》')\" ><img src='" + SystemConfig.CCFlowWebPath + "WF/Img/Btn/delete.gif' border=0 alt='删除此附件' /></a>&nbsp;";
            //    }
            //}
            return strs;
        }
        public static string GenerLabelStr(string title)
        {
            string path = BP.Sys.Glo.Request.ApplicationPath;
            if (path == "" || path == "/")
                path = "..";

            string str = "";
            str += "<TABLE  height='100%' cellPadding='0' background='" + path + "/Images/DG_bgright.gif'>";
            str += "<TBODY>";
            str += "<TR   >";
            str += "<TD  >";
            str += "<IMG src='" + path + "/Images/DG_Title_Left.gif' border='0'></TD>";
            str += "<TD style='font-size:14px'  vAlign='bottom' noWrap background='" + path + "/Images/DG_Title_BG.gif'>&nbsp;";
            str += " &nbsp;<b>" + title + "</b>&nbsp;&nbsp;";
            str += "</TD>";
            str += "<TD>";
            str += "<IMG src='" + path + "/Images/DG_Title_Right.gif' border='0'></TD>";
            str += "</TR>";
            str += "</TBODY>";
            str += "</TABLE>";
            return str;
            //return str;
        }
        
        public static string GenerTablePage(DataTable dt, string title)
        {

            string str = "<Table id='tb' class=Table >";

            str += "<caption>" + title + "</caption>";


            // 标题
            str += "<TR>";
            foreach (DataColumn dc in dt.Columns)
            {
                str += "<TD class='DGCellOfHeader" + BP.Web.WebUser.Style + "' nowrap >" + dc.ColumnName + "</TD>";
            }
            str += "</TR>";

            //内容
            foreach (DataRow dr in dt.Rows)
            {
                str += "<TR>";


                foreach (DataColumn dc in dt.Columns)
                {
                    //string doc=dr[dc.ColumnName];
                    str += "<TD nowrap=true >&nbsp;" + dr[dc.ColumnName] + "</TD>";
                }
                str += "</TR>";
            }
            str += "</Table>";
            return str;
        }
        /// <summary>
        /// 产生临时文件名称
        /// </summary>
        /// <param name="hz"></param>
        /// <returns></returns>
        public static string GenerTempFileName(string hz)
        {
            return Web.WebUser.No + DateTime.Now.ToString("MMddhhmmss") + "." + hz;
        }
        public static void DeleteTempFiles()
        {
            //string[] strs = System.IO.Directory.GetFiles( MapPath( SystemConfig.TempFilePath )) ;
            string[] strs = System.IO.Directory.GetFiles(SystemConfig.PathOfTemp);

            foreach (string s in strs)
            {
                System.IO.File.Delete(s);
            }
        }
        /// <summary>
        /// 重新建立索引
        /// </summary>
        public static void ReCreateIndex()
        {
            ArrayList als = ClassFactory.GetObjects("BP.En.Entity");
            string sql = "";
            foreach (object obj in als)
            {
                Entity en = (Entity)obj;
                if (en.EnMap.EnType == EnType.View)
                    continue;
                sql += "IF EXISTS( SELECT name  FROM  sysobjects WHERE  name='" + en.EnMap.PhysicsTable + "') <BR> DROP TABLE " + en.EnMap.PhysicsTable + "<BR>";
                sql += "CREATE TABLE " + en.EnMap.PhysicsTable + " ( <BR>";
                sql += "";
            }


        }
        public static void DBIOToAccess()
        {
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.En.Entities");
            PubClass.DBIO(DBType.Access, al, false);
        }
        /// <summary>
        /// 检查所有的物理表
        /// </summary>
        public static void CheckAllPTable(string nameS)
        {
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.En.Entities");
            foreach (Entities ens in al)
            {
                if (ens.ToString().Contains(nameS) == false)
                    continue;


                try
                {
                    Entity en = ens.GetNewEntity;
                    en.CheckPhysicsTable();
                }
                catch
                {

                }

            }

        }

        /// <summary>
        /// 数据传输
        /// </summary>
        /// <param name="dbtype">对象</param>
        /// <returns></returns>
        public static void DBIO(DA.DBType dbtype, ArrayList als, bool creatTableOnly)
        {
            foreach (Entities ens in als)
            {
                Entity myen = ens.GetNewEntity;
                if (myen.EnMap.EnType == EnType.View)
                    continue;

                #region create table
                switch (dbtype)
                {

                    case DBType.Oracle:
                        try
                        {

                            DBAccessOfOracle1.RunSQL("drop table " + myen.EnMap.PhysicsTable);
                        }
                        catch
                        {
                        }
                        try
                        {
                            DBAccessOfOracle1.RunSQL(SqlBuilder.GenerCreateTableSQLOfOra_OK(myen));
                        }
                        catch
                        {

                        }
                        break;
                    case DBType.MSSQL:
                        try
                        {
                            if (myen.EnMap.PhysicsTable.Contains("."))
                                continue;

                            if (DBAccessOfMSSQL1.IsExitsObject(myen.EnMap.PhysicsTable))
                                continue;

                            DBAccessOfMSSQL1.RunSQL("drop table " + myen.EnMap.PhysicsTable);
                        }
                        catch
                        {
                        }
                        DBAccessOfMSSQL1.RunSQL(SqlBuilder.GenerCreateTableSQLOfMS(myen));
                        break;
                    case DBType.Informix:
                        try
                        {
                            if (myen.EnMap.PhysicsTable.Contains("."))
                                continue;

                            if (DBAccessOfMSSQL1.IsExitsObject(myen.EnMap.PhysicsTable))
                                continue;

                            DBAccessOfMSSQL1.RunSQL("drop table " + myen.EnMap.PhysicsTable);
                        }
                        catch
                        {
                        }
                        DBAccessOfMSSQL1.RunSQL(SqlBuilder.GenerCreateTableSQLOfInfoMix(myen));
                        break;
                    case DBType.Access:
                        try
                        {
                            DBAccessOfOLE.RunSQL("drop table " + myen.EnMap.PhysicsTable);
                        }
                        catch
                        {
                        }
                        DBAccessOfOLE.RunSQL(SqlBuilder.GenerCreateTableSQLOf_OLE(myen));
                        break;
                    default:
                        throw new Exception("error :");

                }
                #endregion

                if (creatTableOnly)
                    return;

                try
                {
                    QueryObject qo = new QueryObject(ens);
                    qo.DoQuery();
                    // ens.RetrieveAll(1000);
                }
                catch
                {
                    continue;
                }

                #region insert data
                foreach (Entity en in ens)
                {
                    try
                    {
                        switch (dbtype)
                        {
                            case DBType.Oracle:
                            case DBType.Informix:
                                DBAccessOfOracle1.RunSQL(SqlBuilder.Insert(en));
                                break;
                            case DBType.MSSQL:
                                DBAccessOfOracle1.RunSQL(SqlBuilder.Insert(en));
                                break;
                            case DBType.Access:
                                DBAccessOfOLE.RunSQL(SqlBuilder.InsertOFOLE(en));
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.DefaultLogWriteLineError(dbtype.ToString() + "bak出现错误：" + ex.Message);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 获取datatable.
        /// </summary>
        /// <param name="uiBindKey"></param>
        /// <returns></returns>
        public static System.Data.DataTable GetDataTableByUIBineKey(string uiBindKey)
        {
            DataTable dt = new DataTable();
            if (uiBindKey.Contains("."))
            {
                Entities ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);

                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    throw new Exception("类名错误:" + uiBindKey + ",不能转化成ens.");

                ens.RetrieveAllFromDBSource();
                dt = ens.ToDataTableField(uiBindKey);
                return dt;
            }

            //added by liuxc,2017-09-11,增加动态SQL查询类型的处理，此种类型没有固定的数据表或视图
            SFTable sf = new SFTable();
            sf.No = uiBindKey;
            if (sf.RetrieveFromDBSources() != 0)
            {
                if (sf.SrcType == SrcType.Handler || sf.SrcType == SrcType.JQuery)
                    return null;
                dt = sf.GenerHisDataTable;
            }

            if (dt == null)
                dt = new DataTable();

            #region 把列名做成标准的.
            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName.ToLower();
                switch (colName)
                {
                    case "no":
                        col.ColumnName = "No";
                        break;
                    case "name":
                        col.ColumnName = "Name";
                        break;
                    case "parentno":
                        col.ColumnName = "ParentNo";
                        break;
                    default:
                        break;
                }
            }
            #endregion 把列名做成标准的.

            dt.TableName = uiBindKey;
            return dt;
        }
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="uiBindKey">绑定的外键或者枚举</param>
        /// <returns></returns>
        public static System.Data.DataTable GetDataTableByUIBineKeyForCCFormDesigner(string uiBindKey)
        {
            DataTable mydt = new DataTable();
            mydt.Columns.Add("No");
            mydt.Columns.Add("Name");

            DataRow dr = mydt.NewRow();
            dr[0] = "数据1";
            dr[1] = "数据1";
            mydt.Rows.Add(dr);

            dr = mydt.NewRow();
            dr[0] = "数据2";
            dr[1] = "数据2";
            mydt.Rows.Add(dr);

            dr = mydt.NewRow();
            dr[0] = "数据3";
            dr[1] = "数据3";
            mydt.Rows.Add(dr);
            return mydt;



            int topNum = 40;
            DataTable dt = new DataTable();
            if (uiBindKey.Contains("."))
            {
                Entities ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);

                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    throw new Exception("类名错误:" + uiBindKey + ",不能转化成ens.");

                BP.En.QueryObject qo = new QueryObject(ens);
                return qo.DoQueryToTable(topNum);
            }
            else
            {
                SFTable sf = new SFTable(uiBindKey);
                if (sf.SrcType == SrcType.TableOrView)
                {
                    string sql = "";
                    switch (BP.Sys.SystemConfig.AppCenterDBType)
                    {
                        case DBType.Oracle:
                            sql = "SELECT No,Name FROM " + uiBindKey + " where rowNum <= " + topNum;
                            break;
                        case DBType.MSSQL:
                            sql = "SELECT top " + topNum + " No,Name FROM " + uiBindKey;
                            break;
                        default:
                            sql = "SELECT  No,Name FROM " + uiBindKey;
                            break;
                    }
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = uiBindKey;
                    return dt;
                }

                //返回数据.
                return sf.GenerHisDataTable;
            }
        }

        #region 系统调度
        public static string GenerDBOfOreacle()
        {
            ArrayList als = ClassFactory.GetObjects("BP.En.Entity");
            string sql = "";
            foreach (object obj in als)
            {
                Entity en = (Entity)obj;
                sql += "IF EXISTS( SELECT name  FROM  sysobjects WHERE  name='" + en.EnMap.PhysicsTable + "') <BR> DROP TABLE " + en.EnMap.PhysicsTable + "<BR>";
                sql += "CREATE TABLE " + en.EnMap.PhysicsTable + " ( <BR>";
                sql += "";
            }
            //DA.Log.DefaultLogWriteLine(LogType.Error,msg.Replace("<br>@","\n") ); // 
            return sql;
        }
        public static string DBRpt(DBCheckLevel level)
        {
            // 取出全部的实体
            ArrayList als = ClassFactory.GetObjects("BP.En.Entities");
            string msg = "";
            foreach (object obj in als)
            {
                Entities ens = (Entities)obj;
                try
                {
                    msg += DBRpt1(level, ens);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + ens.ToString() + "体检失败:" + ex.Message;
                }
            }

            MapDatas mds = new MapDatas();
            mds.RetrieveAllFromDBSource();
            foreach (MapData md in mds)
            {
                try
                {
                    md.HisGEEn.CheckPhysicsTable();
                    PubClass.AddComment(md.HisGEEn);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + md.No + "体检失败:" + ex.Message;
                }
            }

            MapDtls dtls = new MapDtls();
            dtls.RetrieveAllFromDBSource();
            foreach (MapDtl dtl in dtls)
            {
                try
                {
                    dtl.HisGEDtl.CheckPhysicsTable();
                    PubClass.AddComment(dtl.HisGEDtl);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + dtl.No + "体检失败:" + ex.Message;
                }
            }

            #region 检查处理必要的基础数据 Pub_Day .
            string sql = "";
            string sqls = "";
            sql = "SELECT count(*) Num FROM Pub_Day";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 1; i <= 31; i++)
                    {
                        string d = i.ToString().PadLeft(2, '0');
                        sqls += "@INSERT INTO Pub_Day(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {
            }

            sql = "SELECT count(*) Num FROM Pub_YF";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        string d = i.ToString().PadLeft(2, '0');
                        sqls += "@INSERT INTO Pub_YF(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {
            }

            sql = "SELECT count(*) Num FROM Pub_ND";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 2010; i < 2015; i++)
                    {
                        string d = i.ToString();
                        sqls += "@INSERT INTO Pub_ND(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {

            }
            sql = "SELECT count(*) Num FROM Pub_NY";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 2010; i < 2015; i++)
                    {

                        for (int yf = 1; yf <= 12; yf++)
                        {
                            string d = i.ToString() + "-" + yf.ToString().PadLeft(2, '0');
                            sqls += "@INSERT INTO Pub_NY(No,Name)VALUES('" + d + "','" + d + "')";
                        }
                    }
                }
            }
            catch
            {
            }

            DBAccess.RunSQLs(sqls);
            #endregion 检查处理必要的基础数据。
            return msg;
        }
        private static void RepleaceFieldDesc(Entity en)
        {
            string tableId = DBAccess.RunSQLReturnVal("select ID from sysobjects WHERE name='" + en.EnMap.PhysicsTable + "' AND xtype='U'").ToString();

            if (tableId == null || tableId == "")
                return;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

            }
        }
        /// <summary>
        /// 为表增加注释
        /// </summary>
        /// <returns></returns>
        public static string AddComment()
        {
            // 取出全部的实体
            ArrayList als = ClassFactory.GetObjects("BP.En.Entities");
            string msg = "";
            Entity en = null;
            Entities ens = null;
            foreach (object obj in als)
            {
                try
                {
                    ens = (Entities)obj;
                    en = ens.GetNewEntity;
                    if (en.EnMap.EnType == EnType.View || en.EnMap.EnType == EnType.ThirdPartApp)
                        continue;
                }
                catch
                {
                    continue;
                }

                msg += AddComment(en);
            }
            return msg;
        }
        public static string AddComment(Entity en)
        {
            if (en == null)
                return null;

            if (en.EnMap == null)
                return null;

            if (en.EnMap.PhysicsTable == null)
                return null;


            if (DBAccess.IsExitsObject(en.EnMap.PhysicsTable) == false)
                return "实体表不存在.";

            try
            {
                switch (en.EnMap.EnDBUrl.DBType)
                {
                    case DBType.Oracle:
                        AddCommentForTable_Ora(en);
                        break;
                    case DBType.MySQL:
                        AddCommentForTable_MySql(en);
                        break;
                    default:
                        AddCommentForTable_MS(en);
                        break;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "<hr>" + en.ToString() + "体检失败:" + ex.Message;
            }
        }
        public static void AddCommentForTable_Ora(Entity en)
        {
           

            en.RunSQL("comment on table " + en.EnMap.PhysicsTable + " IS '" + en.EnDesc + "'");
            SysEnums ses = new SysEnums();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (DBAccess.IsExitsTableCol(en.EnMap.PhysicsTable, attr.Field) == false)
                    continue;


                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + "-主键'");
                        break;
                    case FieldType.Normal:
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + "'");
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ",枚举类型:" + ses.ToDesc() + "'");
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ", 主键:枚举类型:" + ses.ToDesc() + "'");
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS " + attr.Desc + ", 外键:对应物理表:" + myen.EnMap.PhysicsTable + ",表描述:" + myen.EnDesc);
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ", 主外键:对应物理表:" + myen1.EnMap.PhysicsTable + ",表描述:" + myen1.EnDesc + "'");
                        break;
                    default:
                        break;
                }
            }
        }
        public static void AddCommentForTable_MySql(Entity en)
        {
            MySql.Data.MySqlClient.MySqlConnection conn =
                new MySql.Data.MySqlClient.MySqlConnection(BP.Sys.SystemConfig.AppCenterDSN);
            en.RunSQL("alter table " + conn.Database + "." + en.EnMap.PhysicsTable + " comment = '" + en.EnDesc + "'");


            //获取当前实体对应表的所有字段结构信息
            DataTable cols =
                DBAccess.RunSQLReturnTable(
                    "select column_name,column_default,is_nullable,character_set_name,column_type from information_schema.columns where table_schema = '" +
                    conn.Database + "' and table_name='" + en.EnMap.PhysicsTable + "'");
            SysEnums ses = new SysEnums();
            string sql = string.Empty;
            DataRow row = null;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                row = cols.Select(string.Format("column_name='{0}'", attr.Field))[0];
                sql = string.Format("ALTER TABLE {0}.{1} CHANGE COLUMN {2} {2} {3}{4}{5}{6} COMMENT ",
                                    conn.Database,
                                    en.EnMap.PhysicsTable,
                                    attr.Field,
                                    row["column_type"].ToString().ToUpper(),
                                    Equals(row["character_set_name"], "utf8") ? " CHARACTER SET 'utf8'" : "",
                                    Equals(row["is_nullable"], "YES") ? " NULL" : " NOT NULL",
                                    Equals(row["column_default"], "NULL")
                                        ? " DEFAULT NULL"
                                        : (Equals(row[""], "") ? "" : " DEFAULT " + row[""]));

                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        en.RunSQL(sql + string.Format("'{0} - 主键'", attr.Desc));
                        break;
                    case FieldType.Normal:
                        en.RunSQL(sql + string.Format("'{0}'", attr.Desc));
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL(sql + string.Format("'{0},枚举类型:{1}'", attr.Desc, ses.ToDesc()));
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL(sql + string.Format("'{0},主键:枚举类型:{1}'", attr.Desc, ses.ToDesc()));
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL(sql +
                                  string.Format("'{0},外键:对应物理表:{1},表描述:{2}'", attr.Desc, myen.EnMap.PhysicsTable,
                                                myen.EnDesc));
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL(sql +
                                  string.Format("'{0},主外键:对应物理表:{1},表描述:{2}'", attr.Desc, myen1.EnMap.PhysicsTable,
                                                myen1.EnDesc));
                        break;
                    default:
                        break;
                }
            }
        }
        private static void AddColNote(Entity en, string table, string col, string note)
        {
            try
            {
                string sql = "execute  sp_dropextendedproperty 'MS_Description','user',dbo,'table','" + table + "','column'," + col;
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {

            }

            try
            {
                string sql = "execute  sp_addextendedproperty 'MS_Description', '" + note + "', 'user', dbo, 'table', '" + table + "', 'column', '" + col + "'";
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 为表增加解释
        /// </summary>
        /// <param name="en"></param>
        public static void AddCommentForTable_MS(Entity en)
        {

            if (en.EnMap.EnType == EnType.View || en.EnMap.EnType == EnType.ThirdPartApp)
                return;

            try
            {
                string sql = "execute  sp_dropextendedproperty 'MS_Description','user',dbo,'table','" + en.EnMap.PhysicsTable + "'";
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {
            }

            try
            {
                string sql = "execute  sp_addextendedproperty 'MS_Description', '" + en.EnDesc + "', 'user', dbo, 'table', '" + en.EnMap.PhysicsTable + "'";
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {
            }


            SysEnums ses = new SysEnums();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.Key == attr.Desc)
                    continue;
                if (attr.Field == attr.Desc)
                    continue;

                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc+"(主键)");
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+"'");
                        break;
                    case FieldType.Normal:
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+"'");
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        //	en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"++"'" );
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ",枚举类型:" + ses.ToDesc());
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ",主键:枚举类型:" + ses.ToDesc());
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+", 主键:枚举类型:"+ses.ToDesc()+"'" );
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ", 外键:对应物理表:" + myen.EnMap.PhysicsTable + ",表描述:" + myen.EnDesc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS "+  );
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ", 主外键:对应物理表:" + myen1.EnMap.PhysicsTable + ",表描述:" + myen1.EnDesc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+  );
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 产程系统报表，如果出现问题，就写入日志里面。
        /// </summary>
        /// <returns></returns>
        public static string DBRpt1(DBCheckLevel level, Entities ens)
        {
            Entity en = ens.GetNewEntity;
            if (en.EnMap.EnDBUrl.DBUrlType != DBUrlType.AppCenterDSN)
                return null;

            if (en.EnMap.EnType == EnType.ThirdPartApp)
                return null;

            if (en.EnMap.EnType == EnType.View)
                return null;

            if (en.EnMap.EnType == EnType.Ext)
                return null;

            // 检测物理表的字段。
            en.CheckPhysicsTable();

            PubClass.AddComment(en);

            string msg = "";
          
            string table = en.EnMap.PhysicsTable;
            Attrs fkAttrs = en.EnMap.HisFKAttrs;
            if (fkAttrs.Count == 0)
                return msg;
            int num = 0;
            string sql;
            //string msg="";
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                string enMsg = "";
                try
                {
                    #region 更新他们，去掉左右空格，因为外键不能包含左右空格。
                    if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                    {
                        /*如果是高中级别,就去掉左右空格*/
                        if (attr.MyDataType == DataType.AppString)
                        {
                            DBAccess.RunSQL("UPDATE " + en.EnMap.PhysicsTable + " SET " + attr.Field + " = rtrim( ltrim(" + attr.Field + ") )");
                        }
                    }
                    #endregion

                    #region 处理关联表的情况.
                    Entities refEns = attr.HisFKEns; // ClassFactory.GetEns(attr.UIBindKey);
                    Entity refEn = refEns.GetNewEntity;

                    //取出关联的表。
                    string reftable = refEn.EnMap.PhysicsTable;
                    //sql="SELECT COUNT(*) FROM "+en.EnMap.PhysicsTable+" WHERE "+attr.Key+" is null or len("+attr.Key+") < 1 ";
                    // 判断外键表是否存在。

                    sql = "SELECT COUNT(*) FROM  sysobjects  WHERE  name = '" + reftable + "'";
                    //num=DA.DBAccess.RunSQLReturnValInt(sql,0);
                    if (DBAccess.IsExitsObject(new DBUrl(DBUrlType.AppCenterDSN), reftable) == false)
                    {
                        //报告错误信息
                        enMsg += "<br>@检查实体：" + en.EnDesc + ",字段 " + attr.Key + " , 字段描述:" + attr.Desc + " , 外键物理表:" + reftable + "不存在:" + sql;
                    }
                    else
                    {
                        Attr attrRefKey = refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue); // 去掉主键的左右 空格．
                        if (attrRefKey.MyDataType == DataType.AppString)
                        {
                            if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                            {
                                /*如果是高中级别,就去掉左右空格*/
                                DBAccess.RunSQL("UPDATE " + reftable + " SET " + attrRefKey.Field + " = rtrim( ltrim(" + attrRefKey.Field + ") )");
                            }
                        }

                        Attr attrRefText = refEn.EnMap.GetAttrByKey(attr.UIRefKeyText);  // 去掉主键 Text 的左右 空格．

                        if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                        {
                            /*如果是高中级别,就去掉左右空格*/
                            DBAccess.RunSQL("UPDATE " + reftable + " SET " + attrRefText.Field + " = rtrim( ltrim(" + attrRefText.Field + ") )");
                        }

                    }
                    #endregion

                    #region 外键的实体是否为空
                    switch (en.EnMap.EnDBUrl.DBType)
                    {
                        case DBType.Oracle:
                            sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " is null or length(" + attr.Field + ") < 1 ";
                            break;
                        default:
                            sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " is null or len(" + attr.Field + ") < 1 ";
                            break;
                    }

                    num = DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    if (num == 0)
                    {
                    }
                    else
                    {
                        enMsg += "<br>@检查实体：" + en.EnDesc + ",物理表:" + en.EnMap.PhysicsTable + "出现" + attr.Key + "," + attr.Desc + "不正确,共有[" + num + "]行记录没有数据。" + sql;
                    }
                    #endregion

                    #region 是否能够对应到外键
                    //是否能够对应到外键。
                    sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( SELECT " + refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue).Field + " FROM " + reftable + "	 ) ";
                    num = DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    if (num == 0)
                    {
                    }
                    else
                    {
                        /*如果是高中级别.*/
                        string delsql = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( SELECT " + refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue).Field + " FROM " + reftable + "	 ) ";
                        //int i =DBAccess.RunSQL(delsql);
                        enMsg += "<br>@" + en.EnDesc + ",物理表:" + en.EnMap.PhysicsTable + "出现" + attr.Key + "," + attr.Desc + "不正确,共有[" + num + "]行记录没有关联到数据，请检查物理表与外键表。" + sql + "如果您想删除这些对应不上的数据请运行如下SQL: " + delsql + " 请慎重执行.";
                    }
                    #endregion

                    #region 判断 主键
                    //DBAccess.IsExits("");
                    #endregion
                }
                catch (Exception ex)
                {
                    enMsg += "<br>@" + ex.Message;
                }

                if (enMsg != "")
                {
                    msg += "<BR><b>-- 检查[" + en.EnDesc + "," + en.EnMap.PhysicsTable + "]出现如下问题,类名称:" + en.ToString() + "</b>";
                    msg += enMsg;
                }
            }
            return msg;
        }
        #endregion

        #region 转化格式  chen
        /// <summary>
        /// 将某控件中的数据转化为Excel文件
        /// </summary>
        /// <param name="ctl"></param>
        public static void ToExcel(System.Web.UI.Control ctl, string filename)
        {
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";//"application/ms-excel";
            //image/JPEG;text/HTML;image/GIF;application/ms-msword
            ctl.Page.EnableViewState = false;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            ctl.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 将某控件中的数据转化为Word文件
        /// </summary>
        /// <param name="ctl"></param>
        public static void ToWord(System.Web.UI.Control ctl, string filename)
        {
            filename = HttpUtility.UrlEncode(filename);
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename + ".doc");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-msword";//image/JPEG;text/HTML;image/GIF;application/ms-excel
            ctl.Page.EnableViewState = false;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            ctl.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
        }

        public static void OpenExcel(string filepath, string tempName)
        {
            tempName = HttpUtility.UrlEncode(tempName);
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + tempName);
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.WriteFile(filepath);
            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Close();
        }
        public static void DownloadFile(string filepath, string tempName)
        {
            if (!"firefox".Contains(HttpContext.Current.Request.Browser.Browser.ToLower()))
                tempName = HttpUtility.UrlEncode(tempName);

            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + tempName);
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/octet-stream;charset=utf8";
            //HttpContext.Current.Response.ContentType = "application/ms-msword";  //image/JPEG;text/HTML;image/GIF;application/ms-excel
            //HttpContext.Current.EnableViewState =false;

            HttpContext.Current.Response.WriteFile(filepath);
            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Close();
        }

        /// <summary>
        /// 从别的网站服务器上下载文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="tempName"></param>
        public static void DownloadHttpFile(string filepath, string tempName)
        {
            List<byte> byteList = new List<byte>();

            //打开网络连接
            string filePth = filepath.Replace("\\", "/").Trim();
            if (filepath.IndexOf("/") == 0) {
                filepath = filepath.Remove(1, filepath.Length - 1);
            }
            if (!SystemConfig.AttachWebSite.Trim().EndsWith("/"))
            {
                filepath = SystemConfig.AttachWebSite.Trim() + "/" + filepath;
            }
            else
            {
                filepath = SystemConfig.AttachWebSite.Trim() + filepath;
            }

            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(filepath);
            //向服务器请求,获得服务器的回应数据流
            using (Stream myStream = myRequest.GetResponse().GetResponseStream())
            {
                //定义一个字节数据
                byte[] btContent = new byte[512];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 512);
                while (intSize > 0)
                {
                    if (intSize == 512)
                        byteList.AddRange(btContent);
                    else
                    {
                        byte[] btContent2 = new byte[intSize];
                        for (int i = 0; i < btContent2.Length; i++)
                        {
                            btContent2[i] = btContent[i];
                        }
                        byteList.AddRange(btContent2);
                    }
                    intSize = myStream.Read(btContent, 0, 512);
                }

                tempName = HttpUtility.UrlEncode(tempName);
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + tempName);
                HttpContext.Current.Response.ContentType = "application/octet-stream;charset=gb2312";

                HttpContext.Current.Response.BinaryWrite(byteList.ToArray());
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Close();

                myStream.Close();
            }
        }
        public static void DownloadFileV4(string filepath, string tempName)
        {
            //File exportFile = null;
            //FileInputStream fis = null;
            //try
            //{
            //    FileInfo oldFile = new FileInfo(filepath);
            //   // exportFile = FileEncrypt.dencryptNoDenc(oldFile); // 生成临时文件

            //    var httpServletResponse = HttpContext.Current.Response;

            //    FileStream servletOutputStream = File.OpenRead(oldFile.FullName);

            //    string fileName= oldFile.Name;


            //    httpServletResponse.AddHeader("Content-Disposition","attachment;filename="+fileName);
            //    httpServletResponse.ContentType="application/x-download"; 


            //    // 此处只写文件名exportFile.getName()，不需绝对路径

            //    httpServletResponse.setHeader(
            //                    "Content-Disposition",
            //                    );
            //    httpServletResponse.setContentLength((int)exportFile.length());
            //    httpServletResponse.setContentType("application/x-download");
            //    byte[] b = new byte[4096];
            //    int i = 0;
            //    fis = new java.io.FileInputStream(exportFile);
            //    while ((i = fis.read(b)) > 0)
            //    {
            //        servletOutputStream.write(b, 0, i);
            //    }
            //}
            //catch (Exception e)
            //{
            //    logger.error(e);
            //}
            //finally
            //{
            //    try
            //    {
            //        if (fis != null)
            //        {
            //            fis.close();
            //        }
            //        if (exportFile != null && exportFile.exists())
            //        {
            //            exportFile.delete();
            //        }
            //    }
            //    catch (Exception ce)
            //    {
            //        logger.error(ce);
            //    }
            //}
            //FacesContext.getCurrentInstance().responseComplete();

        }
        public static void DownloadFileV1(string filepath, string tempName)
        {
            HttpContext.Current.Response.ContentType = "application/x-zip-compressed";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename="+tempName);

            //指定编码 防止中文文件名乱码
            HttpContext.Current.Response.HeaderEncoding = System.Text.Encoding.GetEncoding("gb2312");
            HttpContext.Current.Response.TransmitFile(filepath);            

        }
        public static void DownloadFileV3(string filepath, string tempName)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + tempName);
            HttpContext.Current.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            HttpContext.Current.Response.WriteFile(fileInfo.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
             
        }
        public static void DownloadFileV2(string filepath, string tempName)
        {

            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Exists == false)
                return;
            byte[] buffer = new byte[102400];
            HttpContext.Current.Response.Clear();
            using (FileStream iStream = File.OpenRead(fileInfo.FullName))
            {
                long dataLengthToRead = iStream.Length; //获取下载的文件总大小
                //HttpContext.Current.Response.ContentType = "application/octet-stream"; //此方法不可以.
                HttpContext.Current.Response.ContentType = "application/x-download";

                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;  filename=" +
                                   HttpUtility.UrlEncode(tempName, System.Text.Encoding.UTF8));
                while (dataLengthToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(102400));//'读取的大小

                    HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                    HttpContext.Current.Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                HttpContext.Current.Response.Close();
                HttpContext.Current.Response.End();
            }
        }
        public static void OpenWordDoc(string filepath, string tempName)
        {
            tempName = HttpUtility.UrlEncode(tempName);

            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + tempName);
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-msword";  //image/JPEG;text/HTML;image/GIF;application/ms-excel
            //HttpContext.Current.EnableViewState =false;
            HttpContext.Current.Response.WriteFile(filepath);
            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Close();
        }
        public static void OpenWordDocV2(string filepath, string tempName)
        {
            //tempName = HttpUtility.UrlEncode(tempName);

            FileInfo fileInfo = new FileInfo(filepath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(tempName, System.Text.Encoding.UTF8));
            HttpContext.Current.Response.AppendHeader("Content-Length", fileInfo.Length.ToString());
            HttpContext.Current.Response.WriteFile(fileInfo.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        #endregion

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static DataTable HashtableToDataTable(Hashtable ht)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Hashtable";
            foreach (string key in ht.Keys)
            {
                dt.Columns.Add(key, typeof(string));
            }

            DataRow dr = dt.NewRow();
            foreach (string key in ht.Keys)
            {
                dr[key] = ht[key] as string;
            }
            dt.Rows.Add(dr);
            return dt;
        }
        #region

        #region
        public static void To(string url)
        {
            System.Web.HttpContext.Current.Response.Redirect(url, true);
        }
        public static void Print(string url)
        {
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> var newWindow =window.open('" + url + "','p','width=0,top=10,left=10,height=1,scrollbars=yes,resizable=yes,toolbar=yes,location=yes,menubar=yes') ; newWindow.focus(); </script> ");
        }
        public static BP.En.Entity CopyFromRequest(BP.En.Entity en)
        {
            return CopyFromRequest(en, BP.Sys.Glo.Request);
        }
        public static BP.En.Entity CopyFromRequest(BP.En.Entity en, HttpRequest reqest)
        {
            //获取传递来的所有的checkbox ids 用于设置该属性为falsse.
            string checkBoxIDs = reqest.QueryString["CheckBoxIDs"];
            if (checkBoxIDs != null)
            {
                string[] strs = checkBoxIDs.Split(',');
                foreach (string str in strs)
                {
                    if (str == null || str == "")
                        continue;

                    //设置该属性为false.
                    en.Row[str.Replace("CB_", "")] = 0;
                }
            }


            //如果不使用clone 就会导致 “集合已修改;可能无法执行枚举操作。”的错误。
            Hashtable ht = en.Row.Clone() as Hashtable;

            /*说明已经找到了这个字段信息。*/
            foreach (string key in reqest.Params.Keys)
            {
                if (key == null || key == "")
                    continue;

                //获得实际的值, 具有特殊标记的，系统才赋值.
                string attrKey = key.Clone() as string;
                if (key.StartsWith("TB_"))
                    attrKey = attrKey.Replace("TB_", "");
                else if (key.StartsWith("CB_"))
                    attrKey = attrKey.Replace("CB_", "");
                else if (key.StartsWith("DDL_"))
                    attrKey = attrKey.Replace("DDL_", "");
                else if (key.StartsWith("RB_"))
                    attrKey = attrKey.Replace("RB_", "");
                else
                    continue;

                string val = reqest.Params[key];
                //if (key.IndexOf("CB_") == 0)
                //{
                //    en.Row[attrKey] = 1;
                //    continue;
                //}

                //其他的属性.
                en.Row[attrKey] = val;
            }
            return en;
        }

        public static BP.En.Entity CopyFromRequestByPost(BP.En.Entity en, HttpRequest reqest)
        {
            //获取传递来的所有的checkbox ids 用于设置该属性为falsse.
            string checkBoxIDs = reqest.QueryString["CheckBoxIDs"];
            if (checkBoxIDs != null)
            {
                string[] strs = checkBoxIDs.Split(',');
                foreach (string str in strs)
                {
                    if (str == null || str == "")
                        continue;

                    if (str.Contains("CBPara"))
                    {
                        /*如果是参数字段.*/
                        en.Row[str.Replace("CBPara_", "")]= 0;
                    }
                    else
                    {
                        //设置该属性为false.
                        en.Row[str.Replace("CB_", "")] = 0;
                    }

                }
            }


            //如果不使用clone 就会导致 “集合已修改;可能无法执行枚举操作。”的错误。
            Hashtable ht = en.Row.Clone() as Hashtable;

            /*说明已经找到了这个字段信息。*/
            foreach (string key in reqest.Form.Keys)
            {
                if (key == null || key == "")
                    continue;

                //获得实际的值, 具有特殊标记的，系统才赋值.
                string attrKey = key.Clone() as string;
                if (key.StartsWith("TB_"))
                    attrKey = attrKey.Replace("TB_", "");
                else if (key.StartsWith("CB_"))
                    attrKey = attrKey.Replace("CB_", "");
                else if (key.StartsWith("DDL_"))
                    attrKey = attrKey.Replace("DDL_", "");
                else if (key.StartsWith("RB_"))
                    attrKey = attrKey.Replace("RB_", "");
                else
                {
                    //给参数赋值.

                   
                    if (key.StartsWith("TBPara_"))
                        attrKey = attrKey.Replace("TBPara_", "");
                    else if (key.StartsWith("DDLPara_"))
                        attrKey = attrKey.Replace("DDLPara_", "");
                    else if (key.StartsWith("RBPara_"))
                        attrKey = attrKey.Replace("RBPara_", "");
                    else if (key.StartsWith("CBPara_"))
                        attrKey = attrKey.Replace("CBPara_", "");
                    else
                        continue;
                    
                }

                string val = reqest.Form[key];
                if (key.IndexOf("CB_") == 0 || key.IndexOf("CBPara_") == 0)
                {
                    en.Row[attrKey] = 1;
                    continue;
                }

                //其他的属性.
                en.Row[attrKey] = val;
            }
            return en;
        }

        /// <summary>
        /// 明细表传参保存
        /// </summary>
        /// <param name="en"></param>
        /// <param name="pk"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Entity CopyDtlFromRequests(Entity en, string pk, Map map)
        {
            string allKeys = ";";
            if (pk == null || pk == "")
                pk = "";
            else
                pk = "_" + pk;

            HttpRequest reqest = BP.Sys.Glo.Request;
            foreach (string myK in reqest.Params.Keys)
                allKeys += myK + ";";

            Attrs attrs = map.Attrs;
            foreach (Attr attr in attrs)
            {
                string relKey = null;
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        relKey = "TB_" + attr.Key + pk;
                        break;
                    case UIContralType.CheckBok:
                        relKey = "CB_" + attr.Key + pk;
                        break;
                    case UIContralType.DDL:
                        relKey = "DDL_" + attr.Key + pk;
                        break;
                    case UIContralType.RadioBtn:
                        relKey = "RB_" + attr.Key + pk;
                        break;
                    case UIContralType.MapPin:
                        relKey = "TB_" + attr.Key + pk;
                        break;
                    default:
                        break;
                }

                if (relKey == null)
                    continue;

                if (allKeys.Contains(relKey + ";"))
                {
                    /*说明已经找到了这个字段信息。*/
                    foreach (string myK in BP.Sys.Glo.Request.Params.Keys)
                    {
                        if (myK == null || myK == "")
                            continue;

                        if (myK.EndsWith(relKey))
                        {
                            if (attr.UIContralType == UIContralType.CheckBok)
                            {
                                string val = BP.Sys.Glo.Request.Params[myK];
                                if (val == "on" || val == "1" || val.Contains(",on"))
                                    en.SetValByKey(attr.Key, 1);
                                else
                                    en.SetValByKey(attr.Key, 0);
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, BP.Sys.Glo.Request.Params[myK]);
                            }
                        }
                    }
                    continue;
                }
            }
            if (map.IsHaveAutoFull == false)
                return en;
            en.AutoFull();
            return en;
        }
        public static void WinClose(string returnVal)
        {
            string clientscript = "<script language='javascript'> window.returnValue = '" + returnVal + "'; window.close(); </script>";
            System.Web.HttpContext.Current.Response.Write(clientscript);
        }
        public static void WinCloseAndReloadParent(string returnVal)
        {
            string clientscript = "<script language='javascript'> window.opener.location.reload(); window.close(); </script>";
            System.Web.HttpContext.Current.Response.Write(clientscript);
        }
        public static void WinClose()
        {
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>  window.close(); </script> ");
        }
        public static void WinCloseAndAlertMsg(string message)
        {
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('" + message + "'); window.close(); </script> ");
        }
        public static void Open(string url)
        {
            //  System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> newWindow =window.open('" + url + "','" + winName + "','width=" + width + ",top=" + top + ",scrollbars=yes,resizable=yes,toolbar=false,location=false') ; newWindow.focus(); </script> ");
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> var newWindow =window.open('" + url + "','p' ) ; newWindow.focus(); </script> ");
        }
        public static void WinReload()
        {
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>window.parent.main.document.location.reload(); </script> ");
        }
        public static void WinOpen(string url)
        {
            PubClass.WinOpen(url, "", "msg" + DateTime.Now.ToString("MMddHHmmss"), 300, 300);
        }
        public static void WinOpen(string url, int w, int h)
        {
            PubClass.WinOpen(url, "", "msg" + DateTime.Now.ToString("MMddHHmmss"), w, h);
        }
        public static void WinOpen(string url, string title, string winName, int width, int height)
        {
            PubClass.WinOpen(url, title, winName, width, height, 100, 200);
        }
        public static void WinOpen(string url, string title, int width, int height)
        {
            PubClass.WinOpen(url, title, "ActivePage", width, height, 100, 200);
        }
        public static void WinOpen(string url, string title, string winName, int width, int height, int top, int left)
        {
            url = url.Replace("<", "[");
            url = url.Replace(">", "]");
            url = url.Trim();
            title = title.Replace("<", "[");
            title = title.Replace(">", "]");
            title = title.Replace("\"", "‘");
            if (top == 0 && left == 0)
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> var newWindow =window.open('" + url + "','" + winName + "','width=" + width + ",top=" + top + ",scrollbars=yes,resizable=yes,toolbar=false,location=false') ; </script> ");
            else
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> var newWindow =window.open('" + url + "','" + winName + "','width=" + width + ",top=" + top + ",left=" + left + ",height=" + height + ",scrollbars=yes,resizable=yes,toolbar=false,location=false');</script>");
        }
        /// <summary>
        /// 输出到页面上红色的警告。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteRedMsg(string msg)
        {
            //this.Response.Write("<BR><font color='red' size='"+MsgFontSize.ToString()+"' > <b>"+msg+"</b></font>");
            //if (msg.Length < 200)
            //	return ;
            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/ErrorPage.aspx";
            WinOpen(url, "警告", msg + DateTime.Now.ToString("mmss"), 500, 400, 150, 270);
        }
        /// <summary>
        /// 输出到页面上蓝色的信息。
        /// </summary>
        /// <param name="msg">消息</param>
        public static void ResponseWriteBlueMsg(string msg)
        {

            if (SystemConfig.IsBSsystem)
            {
                msg = msg.Replace("@", "<BR>@");
                System.Web.HttpContext.Current.Session["info"] = msg;
                string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx";
                WinOpen(url, "信息", "sysmsg", 500, 400, 150, 270);
            }
            else
            {
                Log.DebugWriteInfo(msg);
            }
        }
        /// <summary>
        /// 保存成功
        /// </summary>
        public static void ResponseWriteBlueMsg_SaveOK()
        {
            //this.Alert("保存成功!");

            ResponseWriteBlueMsg("保存成功!");
        }
        /// <summary>
        /// ResponseWriteBlueMsg_DeleteOK
        /// </summary>
        public static void ResponseWriteBlueMsg_DeleteOK()
        {
            //this.Alert("删除成功!");
            ResponseWriteBlueMsg("删除成功!");
        }
        /// <summary>
        /// ResponseWriteBlueMsg_UpdataOK
        /// </summary>
        public static void ResponseWriteBlueMsg_UpdataOK()
        {
            // this.Alert("更新成功!");
            ResponseWriteBlueMsg("更新成功!");
        }
        /// <summary>
        /// 输出到页面上黑色的信息。
        /// </summary>
        /// <param name="msg">消息</param>
        public static void ResponseWriteBlackMsg(string msg)
        {
            System.Web.HttpContext.Current.Response.Write("<font color='Black' size=5 ><b>" + msg + "</b></font>");
        }
        public static void ResponseSript(string Sript)
        {
            System.Web.HttpContext.Current.Response.Write(Sript);
        }
        public static void ToSignInPage()
        {
            System.Web.HttpContext.Current.Response.Redirect(BP.Sys.Glo.Request.ApplicationPath + "/SignIn.aspx?url=/Wel.aspx");
        }
        public static void ToWelPage()
        {
            System.Web.HttpContext.Current.Response.Redirect(BP.Sys.Glo.Request.ApplicationPath + "/Wel.aspx");
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        public static void ToErrorPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            string path = BP.Sys.Glo.Request.ApplicationPath;
            if (path == "/" || path == "")
                path = "";

            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx");
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        public static void ToMsgPage(string mess)
        {
            mess = mess.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx?d=" + DateTime.Now.ToString(), false);

            //System.Web.HttpContext.Current.Session["info"]=mess;
            //System.Web.HttpContext.Current.Response.Redirect(BP.Sys.Glo.Request.ApplicationPath+"/Port/InfoPage.aspx",true);
        }
        #endregion

        /// <summary>
        ///转到一个页面上。 '_top'
        /// </summary>
        /// <param name="mess"></param>
        /// <param name="target">'_top'</param>
        public static void ToErrorPage(string mess, string target)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;

            string path = BP.Sys.Glo.Request.ApplicationPath;
            if (path == "/" || path == "")
                path = "";

            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx target='_top'");
        }
        //public static void AlertSaveOK()
        //{
        //    "保存成功";
        //}
        /// <summary>
        /// 外部参数.
        /// </summary>
        public static string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
        /// <summary>
        /// 不用page 参数，show message
        /// </summary>
        /// <param name="mess"></param>
        public static void Alert(string mess)
        {
            //string msg1 = "<script language=javascript>alert('" + msg + "');</script>";
            //if (! System.Web.HttpContext.Current.ClientScript.IsClientScriptBlockRegistered("a "))
            //    ClientScript.RegisterClientScriptBlock(this.GetType(), "a ", msg1);


            string script = "<script language=JavaScript>alert('" + mess + "');</script>";
            System.Web.HttpContext.Current.Response.Write(script);



            //	System.Web.HttpContext.Current.Response.aps ( script );
            //  System.Web.HttpContext.Current.Response.Write(script);
        }

        public static void ResponseWriteScript(string script)
        {
            script = "<script language=JavaScript> " + script + "</script>";
            System.Web.HttpContext.Current.Response.Write(script);
        }
        #endregion

    }
}