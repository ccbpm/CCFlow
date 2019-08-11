using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.ComponentModel;
using System.Data;
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

namespace BP.WF.HttpHandler
{
    public class HttpHandlerGlo
    {
        #region 转化格式  chen

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
            if (filepath.IndexOf("/") == 0)
            {
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
    }
}
