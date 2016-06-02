using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using BP.Sys;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using System.Web;

namespace BP.Web.Controls
{
    public class Glo
    {
        #region ddl 的操作.
        public static void DDL_BindDataTable(DropDownList ddl, DataTable dt, string selectVal=null, 
            string valCol="No", string textVal="Name")
        {
            ddl.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr[textVal].ToString(), dr[valCol].ToString());
                ddl.Items.Add(li);
                if (selectVal != null && selectVal == dr[valCol].ToString())
                    li.Selected = true;
            }
        }
        public static void DDL_BindEns(DropDownList ddl, DataTable dt, string selectVal, 
            string valAttr="No", string textAttr="Name")
        {
            ddl.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr[textAttr].ToString(), dr[valAttr].ToString());
                ddl.Items.Add(li);
                if (selectVal == dr[valAttr].ToString())
                    li.Selected = true;
            }
        }
        public static void DDL_BindEns(DropDownList ddl, Entities ens, string selectVal, 
            string valAttr = "No", string textAttr = "Name")
        {
            ddl.Items.Clear();
            foreach (Entity en in ens)
            {
                ListItem li = new ListItem(en.GetValStrByKey(textAttr), en.GetValStrByKey(valAttr));
                ddl.Items.Add(li);
                if (selectVal == en.GetValStrByKey(valAttr ))
                    li.Selected = true;
            }
        }
        public static void DDL_BindEns(DropDownList ddl, string ensName, string selectVal)
        {
            ddl.Items.Clear();

            Entities ens = BP.En.ClassFactory.GetEns(ensName) as Entities;
            ens.RetrieveAll();

            foreach (Entity en in ens)
            {
                ListItem li = new ListItem(en.GetValStrByKey("Name"), en.GetValStrByKey("No"));
                ddl.Items.Add(li);
                if (selectVal == en.GetValStrByKey("No"))
                    li.Selected = true;
            }
        }
        public static void DDL_BindEnum(DropDownList ddl, string enumKey, int selectedVal)
        {
            ddl.Items.Clear();
            SysEnums ens = new SysEnums(enumKey);
            foreach (SysEnum en in ens)
            {
                ListItem li = new ListItem(en.Lab, en.IntKey.ToString());
                ddl.Items.Add(li);
                if (selectedVal == en.IntKey)
                    li.Selected = true;
            }
        }
        public static void DDL_SetSelectVal(DropDownList ddl, string val, string sql)
        {
            ddl.Items.Clear();
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(dr["Name"].ToString(), dr["No"].ToString()));
            }
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == val)
                {
                    li.Selected = true;
                    break;
                }
            }
        }

        public static void DDL_SetSelectVal2(DropDownList ddl, string val, string sql)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("请选择", "-1"));
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(dr["Name"].ToString(), dr["No"].ToString()));
            }
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == val)
                {
                    li.Selected = true;
                    break;
                }
            }
        }

        public static void DDL_SetSelectVal(DropDownList ddl, string val)
        {
            foreach (ListItem li in ddl.Items)
            {
                li.Selected = false;
            }

            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == val)
                {
                    li.Selected = true;
                    break;
                }
            }
        }
        #endregion

        public void DownFileByPath(string filepath, string filename)
        {
            if (null == filepath || filepath.Trim().Length < 1) return;
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            if (null == filename || filename.Trim().Length < 1) filename = Path.GetFileName(filepath);

            System.IO.Stream iStream = null;

            // Buffer to read 10K bytes in chunk:
            byte[] buffer = new Byte[10240];

            // Length of the file:
            int length;

            // Total bytes to read:
            long dataToRead;

            // Identify the file to download including its path.
            if (!File.Exists(filepath))
            {
                response.Write("要下载的文件不存在!" + filepath);
                return;
            }
            try
            {

                // Open the file.
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                response.Clear();

                // Total bytes to read:
                dataToRead = iStream.Length;

                long p = 0;
                if (request.Headers["Range"] != null)
                {
                    response.StatusCode = 206;
                    p = long.Parse(request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                response.ContentType = "application/octet-stream";
                //response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(request.ContentEncoding.GetBytes(filename)));
                //response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(filename, Encoding.Unicode));
                //response.Charset = "gb2312";
                response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(filename, Encoding.UTF8));  //Encoding.GetEncoding("GB2312")
                iStream.Position = p;
                dataToRead = dataToRead - p;
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, 10240);

                        // Write the data to the current output stream.
                        response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
                response.End();
            }
        }

        public void DownFileByURL(string url, string filename)
        {
            if (null == url || url.Trim().Length < 1) return;

            string filepath = HttpContext.Current.Server.MapPath(url);
            DownFileByPath(filepath, filename);
        }

        public static string DealStrLength(string str, int maxLen)
        {
            if (str.Length > maxLen)
                return str.Substring(0, maxLen);
            return str;
        }
        public static string NetDiskFtpIP
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpIP"];
            }
        }

        public static string NetDiskFtpUser
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpUser"];
            }
        }

        public static string NetDiskFtpPass
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpPass"];
            }
        }


        public static void DownFile(string filepath, string fileName, HttpRequest request, HttpResponse response)
        {
            System.IO.Stream iStream = null;
            // Buffer to read 10K bytes in chunk:string
            byte[] buffer = new Byte[10240];
            // Length of the file:
            int length;
            // Total bytes to read:
            long dataToRead;

            // Identify the file to download including its path.
            if (!File.Exists(filepath)) return;

            // Identify the file name.
            string filename = Path.GetFileName(filepath);

            try
            {
                // Open the file.
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                response.Clear();

                // Total bytes to read:
                dataToRead = iStream.Length;

                long p = 0;
                if (request.Headers["Range"] != null)
                {
                    response.StatusCode = 206;
                    p = long.Parse(request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                response.ContentType = "application/octet-stream";
                response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                iStream.Position = p;
                dataToRead = dataToRead - p;
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, 10240);

                        // Write the data to the current output stream.
                        response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
                response.End();
            }
        }
    }
}
