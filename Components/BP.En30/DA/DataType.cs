using System;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using System.Text;
using System.IO;
using BP.Sys;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace BP.DA
{
    /// <summary>
    /// DataType 的摘要说明。
    /// </summary>
    public class DataType
    {
        public static bool IsNullOrEmpty(string s)
        {
            if (s == "null")
                return true;

            return string.IsNullOrEmpty(s);
        }

        #region 与日期相关的操作.
        /// <summary>
        /// 获得指定日期的周1第一天日期.
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns></returns>
        public static DateTime WeekOfMonday(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Monday)
                return DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " 00:01");

            for (int i = 0; i < 7; i++)
            {
                DateTime mydt = dt.AddDays(-i);
                if (mydt.DayOfWeek == DayOfWeek.Monday)
                    return DataType.ParseSysDate2DateTime(mydt.ToString("yyyy-MM-dd") + " 00:01");
            }
            throw new Exception("@系统错误.");
        }
        /// <summary>
        /// 获得指定日期的周7第7天日期.
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns></returns>
        public static DateTime WeekOfSunday(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Sunday)
                return DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " 00:01");

            for (int i = 0; i < 7; i++)
            {
                DateTime mydt = dt.AddDays(i);
                if (mydt.DayOfWeek == DayOfWeek.Sunday)
                    return DataType.ParseSysDate2DateTime(mydt.ToString("yyyy-MM-dd") + " 00:01");
            }
            throw new Exception("@系统错误.");
        }
        /// <summary>
        /// 增加日期去掉周末节假日
        /// </summary>
        /// <param name="dt">日期</param>
        /// <param name="days">增加的天数</param>
        /// <returns></returns>
        public static DateTime AddDays(string dt, int days, TWay tway)
        {
            return AddDays(BP.DA.DataType.ParseSysDate2DateTime(dt), days, tway);
        }
        /// <summary>
        /// 增加日期去掉周末
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="days"></param>
        /// <returns>返回天数</returns>
        public static DateTime AddDays(DateTime dt, int days, TWay tway)
        {
            if (tway == TWay.AllDays)
                return dt.AddDays(days);

            //没有设置节假日.
            if (BP.Sys.GloVar.Holidays == "")
            {
                // 2015年以前的算法.
                dt = dt.AddDays(days);
                if (dt.DayOfWeek == DayOfWeek.Saturday)
                    return dt.AddDays(2);

                if (dt.DayOfWeek == DayOfWeek.Sunday)
                    return dt.AddDays(1);
                return dt;
            }

            /* 设置节假日. */
            while (days > 0)
            {
                if (BP.Sys.GloVar.Holidays.Contains(dt.ToString("MM-dd")))
                {
                    dt = dt.AddDays(1);
                    continue;
                }

                days--;

                if (days == 0)
                    break;

                dt = dt.AddDays(1);
            }
            return dt;
        }
        /// <summary> 
        /// 取指定日期是一年中的第几周 
        /// </summary> 
        /// <param name="dtime">给定的日期</param> 
        /// <returns>数字 一年中的第几周</returns> 
        public static int WeekOfYear(DateTime dtime)
        {
            int weeknum = 0;
            DateTime tmpdate = DateTime.Parse(dtime.Year.ToString() + "-1" + "-1");
            DayOfWeek firstweek = tmpdate.DayOfWeek;
            //if(firstweek) 
            int i = dtime.DayOfYear - 1 + (int)firstweek;
            weeknum = i / 7;
            if (i > 0)
            {
                weeknum++;
            }
            return weeknum;
        }
        public static string TurnToJiDuByDataTime(string dt)
        {
            if (dt.Length <= 6)
                throw new Exception("@要转化季度的日期格式不正确:" + dt);
            string yf = dt.Substring(5, 2);
            switch (yf)
            {
                case "01":
                case "02":
                case "03":
                    return dt.Substring(0, 4) + "-03";
                case "04":
                case "05":
                case "06":
                    return dt.Substring(0, 4) + "-06";
                case "07":
                case "08":
                case "09":
                    return dt.Substring(0, 4) + "-09";
                case "10":
                case "11":
                case "12":
                    return dt.Substring(0, 4) + "-12";
                default:
                    break;
            }
            return null;
        }
        #endregion

        #region 将json转换为DataTable
        /// <summary>
        /// 将json转换为DataTable
        /// </summary>
        /// <param name="json">得到的json</param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string json)
        {
            return BP.Tools.Json.ToDataTable(json);
        }
        #endregion

        #region Datatable转换为Json
        /// <summary>     
        /// Datatable转换为Json     
        /// </summary>    
        /// <param name="table">Datatable对象</param>     
        /// <returns>Json字符串</returns>     
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            if (drc.Count >0)
            {
                for (int i = 0; i < drc.Count; i++)
                {
                    jsonString.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string strKey = dt.Columns[j].ColumnName;
                        /**小周鹏修改-2014/11/11----------------------------START**/
                        // BillNoFormat对应value:{YYYY}-{MM}-{dd}-{LSH4} Format时会产生异常。
                        if (strKey.Equals("BillNoFormat"))
                        {
                            continue;
                        }
                        /**小周鹏修改-2014/11/11----------------------------END**/
                        string strValue = drc[i][j].ToString();
                        Type type = dt.Columns[j].DataType;
                        jsonString.Append("\"" + strKey + "\":");

                        strValue = String.Format(strValue, type);
                        if (j < dt.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + strValue + "\",");
                        }
                        else
                        {
                            jsonString.Append("\"" + strValue + "\"");
                        }
                    }
                    jsonString.Append("},");
                }
                jsonString.Remove(jsonString.Length - 1, 1);
            }
            jsonString.Append("]");
            return jsonString.ToString();
        }
        /// <summary>    
        /// DataTable转换为Json     
        /// </summary>    
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (DataType.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + String.Format(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        #endregion

        /// <summary>
        /// 根据通用的树形结构生成行政机构树形结构
        /// </summary>
        /// <param name="dtTree">通用格式的数据表No,Name,ParentNo列</param>
        /// <param name="dtTree">根目录编号值</param>
        /// <returns></returns>
        public static DataTable PraseParentTree2TreeNo(DataTable dtTree, string parentNo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("IsDtl", typeof(string));

            dt.Columns.Add("RefNo", typeof(string));
            dt.Columns.Add("RefParentNo", typeof(string));

            dt = _PraseParentTree2TreeNo(dtTree, dt, parentNo);
            return dt;
        }
        private static DataTable _PraseParentTree2TreeNo(DataTable dtTree, DataTable newDt, string parentNo)
        {
            //记录已经转换的数据
            System.Collections.Generic.List<DataRow> removeRows = new System.Collections.Generic.List<DataRow>();
            //DataTable newDtTree = dtTree.Copy();

            //newDtTree.DefaultView.RowFilter = " ParentNo=" + parentNo;
            //newDtTree = newDtTree.DefaultView.ToTable();

            foreach (DataRow row in dtTree.Rows)
            {
                if (row["ParentNo"].ToString().Equals(parentNo) || row["No"].ToString().Equals(parentNo))
                {
                    DataRow newRow = newDt.NewRow();

                    newRow["No"] = row["No"].ToString();
                    newRow["Name"] = row["Name"];
                    newRow["IsDtl"] = "0";


                    if (dtTree.Columns.Contains("Idx"))
                        newRow["Grade"] = row["Idx"];
                    if (dtTree.Columns.Contains("RefNo"))
                        newRow["RefNo"] = row["RefNo"];
                    else
                        newRow["RefNo"] = row["No"];

                    newRow["RefParentNo"] = row["ParentNo"];

                    newDt.Rows.Add(newRow);
                    removeRows.Add(row);
                }

            }

            #region 将原结构数据转换到新的datable 中
            //foreach (DataRow row in dtTree.Rows)
            //{
            //    if (newDt.Rows.Count == 0)
            //    {
            //        if (!row["IsRoot"].Equals("1"))
            //            continue;

            //        DataRow newRow = newDt.NewRow();

            //        newRow["No"] = row["No"];
            //        newRow["Name"] = row["Name"];
            //        newRow["Grade"] = row["Idx"];
            //        newRow["IsDtl"] = "";

            //        newRow["RefNo"] = row["RefNo"];
            //        newRow["RefParentNo"] = row["ParentNo"];

            //        newDt.Rows.Add(newRow);
            //        removeRows.Add(row);
            //    }
            //    else
            //    {
            //        foreach (DataRow newDtRow in newDt.Rows)
            //        {
            //            if (row["ParentNo"].Equals(newDtRow["No"]))
            //            {
            //                DataRow newRow = newDt.NewRow();

            //                newRow["No"] = row["No"];
            //                newRow["Name"] = row["Name"];
            //                newRow["Grade"] = row["Idx"];
            //                newRow["IsDtl"] = "";

            //                newRow["RefNo"] = row["RefNo"];
            //                newRow["RefParentNo"] = row["ParentNo"];

            //                newDt.Rows.Add(newRow);
            //                removeRows.Add(row);
            //            }
            //        }
            //    }
            //}
            #endregion 将原结构数据转换到新的datable 中

            //移除已经转换的数据
            foreach (DataRow row in removeRows)
                dtTree.Rows.Remove(row);
            //如果原结构中还有数据就接着转换
            if (dtTree.Rows.Count != 0)
                _PraseParentTree2TreeNo(dtTree, newDt, dtTree.Rows[0]["No"].ToString());
            return newDt;
        }
        public static string PraseGB2312_To_utf8(string text)
        {
            if (DataType.IsNullOrEmpty(text))
                return text;

            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// 转换成MB
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float PraseToMB(long val)
        {
            try
            {
                return float.Parse(String.Format("{0:##.##}", val / 1048576));
            }
            catch
            {
                return 0;
            }
        }
        public static string PraseStringToUrlFileName(string fileName)
        {

            if (fileName.LastIndexOf('\\') == -1)
            {
                fileName = PraseStringToUrlFileNameExt(fileName, "%", "%25");
                fileName = PraseStringToUrlFileNameExt(fileName, "+", "%2B");
                fileName = PraseStringToUrlFileNameExt(fileName, " ", "%20");
                fileName = PraseStringToUrlFileNameExt(fileName, "/", "%2F");
                fileName = PraseStringToUrlFileNameExt(fileName, "?", "%3F");
                fileName = PraseStringToUrlFileNameExt(fileName, "#", "%23");
                fileName = PraseStringToUrlFileNameExt(fileName, "&", "%26");
                fileName = PraseStringToUrlFileNameExt(fileName, "=", "%3D");
                fileName = PraseStringToUrlFileNameExt(fileName, " ", "%20");
                return fileName;
            }

            //HttpUtility.HtmlEncode(fileName);

            string filePath = fileName.Substring(0, fileName.LastIndexOf('\\'));
            string fName = fileName.Substring(fileName.LastIndexOf('\\'));
            // fName = HttpUtility.HtmlEncode(fName);
            //if (1 == 2)
            //{
            fName = PraseStringToUrlFileNameExt(fName, "%", "%25");
            fName = PraseStringToUrlFileNameExt(fName, "+", "%2B");
            fName = PraseStringToUrlFileNameExt(fName, " ", "%20");
            fName = PraseStringToUrlFileNameExt(fName, "/", "%2F");
            fName = PraseStringToUrlFileNameExt(fName, "?", "%3F");
            fName = PraseStringToUrlFileNameExt(fName, "#", "%23");
            fName = PraseStringToUrlFileNameExt(fName, "&", "%26");
            fName = PraseStringToUrlFileNameExt(fName, "=", "%3D");
            fName = PraseStringToUrlFileNameExt(fName, " ", "%20");

            // }
            return filePath + fName;
        }
        private static string PraseStringToUrlFileNameExt(string fileName, string val, string replVal)
        {
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            fileName = fileName.Replace(val, replVal);
            return fileName;
        }
        /// <summary>
        /// 处理文件名称
        /// </summary>
        /// <param name="fileNameFormat">文件格式</param>
        /// <returns>返回合法的文件名</returns>
        public static string PraseStringToFileName(string fileNameFormat)
        {
            char[] strs = "+#?*\"<>/;,-:%~".ToCharArray();
            foreach (char c in strs)
                fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

            strs = "：，。；？".ToCharArray();
            foreach (char c in strs)
                fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

            //去掉空格.
            while (fileNameFormat.Contains(" ") == true)
                fileNameFormat = fileNameFormat.Replace(" ", "");

            //替换特殊字符.
            fileNameFormat = fileNameFormat.Replace("\t\n", "");

            //处理合法的文件名.
            StringBuilder rBuilder = new StringBuilder(fileNameFormat);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);

            fileNameFormat = rBuilder.ToString();

            fileNameFormat = fileNameFormat.Replace("__","_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");

            if (fileNameFormat.Length > 240)
                fileNameFormat = fileNameFormat.Substring(0, 240);

            return fileNameFormat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="isNumber"></param>
        /// <returns></returns>
        public static string PraseAtToInSql(string strs, bool isNumber)
        {
            strs = strs.Replace("@", "','");
            strs = strs + "'";
            strs = strs.Substring(2);
            if (isNumber)
                strs = strs.Replace("'", "");
            return strs;
        }
        /// <summary>
        /// 把内容里面的东西处理成超连接。
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static string DealSuperLink(string doc)
        {
            if (doc == null)
                return null;

            return doc;

            Regex urlregex = new Regex(@"(http:\/\/([\w.]+\/?)\S*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            doc = urlregex.Replace(doc, "<a href='' target='_blank'></a>");
            Regex emailregex = new Regex(@"([a-zA-Z_0-9.-]+@[a-zA-Z_0-9.-]+\.\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            doc = emailregex.Replace(doc, "<a href=mailto:></a>");
            return doc;
        }
        /// <summary>
        /// 将文件转化为二进制
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] ConvertFileToByte(string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open,
                System.IO.FileAccess.Read, FileShare.ReadWrite);

            byte[] nowByte = new byte[(int)fs.Length];
            try
            {
                fs.Read(nowByte, 0, (int)fs.Length);
                return nowByte;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                fs.Close();
            }
        }
      
        

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="file">路径</param>
        /// <param name="Doc">内容</param>
        public static void WriteFile(string file, string Doc)
        {
            System.IO.StreamWriter sr;
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);
            try
            {
                //sr = new System.IO.StreamWriter(file, false, System.Text.Encoding.GetEncoding("GB2312"));
                sr = new System.IO.StreamWriter(file, false, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception("@文件：" + file + ",错误:" + ex.Message);
            }

            sr.Write(Doc);
            sr.Close();
        }
        /// <summary>
        /// 写入一个文件
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        public static string WriteFile(string filePathName, byte[] objData)
        {
            string folder = System.IO.Path.GetDirectoryName(filePathName);
            if (System.IO.Directory.Exists(folder)==false)
                System.IO.Directory.CreateDirectory(folder);

            if (System.IO.File.Exists(filePathName)==true)
                System.IO.File.Delete(filePathName);

            System.IO.FileStream fs = new System.IO.FileStream(filePathName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(fs);
            try
            {
                w.Write(objData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                w.Close();
                fs.Close();
            }
            return filePathName;
        }
        /// <summary>
        /// Http下载文件
        /// </summary>
        public static string HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);

            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }
        /// <summary>
        /// 读取URL内容
        /// </summary>
        /// <param name="url">要读取的url</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encode">text code.</param>
        /// <returns>返回读取内容</returns>
        public static string ReadURLContext(string url, int timeOut, Encoding encode)
        {
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "get";
                webRequest.Timeout = timeOut;
                string str = webRequest.Address.AbsoluteUri;
                str = str.Substring(0, str.LastIndexOf("/"));
            }
            catch (Exception ex)
            {
                try
                {
                    BP.DA.Log.DefaultLogWriteLineWarning("@读取URL出现错误:URL=" + url + "@错误信息：" + ex.Message);
                    return null;
                }
                catch
                {
                    return ex.Message;
                }
            }
            //	因为它返回的实例类型是WebRequest而不是HttpWebRequest,因此记得要进行强制类型转换
            //  接下来建立一个HttpWebResponse以便接收服务器发送的信息，它是调用HttpWebRequest.GetResponse来获取的：
            HttpWebResponse webResponse;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                try
                {
                    // 如果出现死连接。
                    BP.DA.Log.DefaultLogWriteLineWarning("@获取url=" + url + "失败。异常信息:" + ex.Message, true);
                    return null;
                }
                catch
                {
                    return ex.Message;
                }
            }

            //如果webResponse.StatusCode的值为HttpStatusCode.OK，表示成功，那你就可以接着读取接收到的内容了：
            // 获取接收到的流
            Stream stream = webResponse.GetResponseStream();
            System.IO.StreamReader streamReader = new StreamReader(stream, encode);
            string content = streamReader.ReadToEnd();
            webResponse.Close();
            return content;
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="file">路径</param>
        /// <returns>内容</returns>
        public static string ReadTextFile(string file)
        {
            System.IO.StreamReader read = new System.IO.StreamReader(file, System.Text.Encoding.UTF8); // 文件流.
            string doc = read.ReadToEnd();  //读取完毕。
            read.Close(); // 关闭。
            return doc;
        }
        public static bool SaveAsFile(string filePath, string doc)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false);
            sw.Write(doc);
            sw.Close();
            return true;
        }
        public static string ReadTextFile2Html(string file)
        {
            return DataType.ParseText2Html(ReadTextFile(file));
        }
        /// <summary>
        /// 判断是否全部是汉字
        /// </summary>
        /// <param name="htmlstr"></param>
        /// <returns></returns>
        public static bool CheckIsChinese(string htmlstr)
        {
            char[] chs = htmlstr.ToCharArray();
            foreach (char c in chs)
            {
                int i = c.ToString().Length;
                if (i == 1)
                    return false;
            }
            return true;
        }

        #region 元角分
        public static string TurnToFiels(float money)
        {
            string je = money.ToString("0.00");

            string strs = "";

            switch (je.Length)
            {
                case 7: //         千                                百                                  十                              元                                角                              分;
                    strs = "D" + je.Substring(0, 1) + ".TW,THOU.TW,D" + je.Substring(1, 1) + ".TW,HUN.TW,D" + je.Substring(2, 1) + ".TW,TEN.TW,D" + je.Substring(3, 1) + ".TW,YUAN.TW,D" + je.Substring(5, 1) + ".TW,JIAO.TW,D" + je.Substring(6, 1) + ".TW,FEN.TW";
                    break;
                case 6: // 百;
                    strs = "D" + je.Substring(0, 1) + ".TW,HUN.TW,D" + je.Substring(1, 1) + ".TW,TEN.TW,D" + je.Substring(2, 1) + ".TW,YUAN.TW,D" + je.Substring(4, 1) + ".TW,JIAO.TW,D" + je.Substring(5, 1) + ".TW,FEN.TW";
                    break;
                case 5: // 十;
                    strs = "D" + je.Substring(0, 1) + ".TW,TEN.TW,D" + je.Substring(1, 1) + ".TW,YUAN.TW,D" + je.Substring(3, 1) + ".TW,JIAO.TW,D" + je.Substring(4, 1) + ".TW,FEN.TW";
                    break;
                case 4: // 元;
                    if (money > 1)
                        strs = "D" + je.Substring(0, 1) + ".TW,YUAN.TW,D" + je.Substring(2, 1) + ".TW,JIAO.TW,D" + je.Substring(3, 1) + ".TW,FEN.TW";
                    else
                        strs = "D" + je.Substring(2, 1) + ".TW,JIAO.TW,D" + je.Substring(3, 1) + ".TW,FEN.TW";
                    break;
                default:
                    throw new Exception("没有涉及到这么大的金额播出");
            }

            //			strs=strs.Replace(",D0.TW,JIAO.TW,D0.TW,FEN.TW",""); // 替换掉 .0角0分;
            //			strs=strs.Replace("D0.TW,HUN.TW,D0.TW,TEN.TW","D0.TW"); // 替换掉 .0百0十 为 0 ;
            //			strs=strs.Replace("D0.TW,THOU.TW","D0.TW");  // 替换掉零千。
            //			strs=strs.Replace("D0.TW,HUN.TW","D0.TW");
            //			strs=strs.Replace("D0.TW,TEN.TW","D0.TW");
            //			strs=strs.Replace("D0.TW,JIAO.TW","D0.TW");
            //			strs=strs.Replace("D0.TW,FEN.TW","D0.TW");
            return strs;
        }
        #endregion

        public static string Html2Text(string htmlstr)
        {
            htmlstr = htmlstr.Replace("<BR>", "\n");
            return htmlstr.Replace("&nbsp;", " ");
            //	return htmlstr;
        }
        public static string ByteToString(byte[] bye)
        {
            string s = "";
            foreach (byte b in bye)
            {
                s += b.ToString();
            }
            return s;
        }
        public static byte[] StringToByte(string s)
        {
            byte[] bs = new byte[s.Length];
            char[] cs = s.ToCharArray();
            int i = 0;
            foreach (char c in cs)
            {
                bs[i] = Convert.ToByte(c);
                i++;
            }
            return bs;
        }
        /// <summary>
        /// 取道百分比
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string GetPercent(decimal a, decimal b)
        {
            decimal p = a / b;
            return p.ToString("0.00%");
        }
        public static string GetWeek(int weekidx)
        {
            switch (weekidx)
            {
                case 0:
                    return "星期日";
                case 1:
                    return "星期一";
                case 2:
                    return "星期二";
                case 3:
                    return "星期三";
                case 4:
                    return "星期四";
                case 5:
                    return "星期五";
                case 6:
                    return "星期六";
                default:
                    throw new Exception("error weekidx=" + weekidx);
            }
        }

        public static string GetABC(string abc)
        {
            switch (abc)
            {
                case "A":
                    return "B";
                case "B":
                    return "C";
                case "C":
                    return "D";
                case "D":
                    return "E";
                case "E":
                    return "F";
                case "F":
                    return "G";
                case "G":
                    return "H";
                case "H":
                    return "I";
                case "I":
                    return "J";
                case "J":
                    return "K";
                case "K":
                    return "L";
                case "L":
                    return "M";
                case "M":
                    return "N";
                case "N":
                    return "O";
                case "Z":
                    return "A";
                default:
                    throw new Exception("abc error" + abc);
            }
        }
        public static string GetBig5(string text)
        {
            System.Text.Encoding e2312 = System.Text.Encoding.GetEncoding("GB2312");
            byte[] bs = e2312.GetBytes(text);
            System.Text.Encoding e5 = System.Text.Encoding.GetEncoding("Big5");
            byte[] bs5 = System.Text.Encoding.Convert(e2312, e5, bs);
            return e5.GetString(bs5);
        }
        /// <summary>
        /// 返回 data1 - data2 的天数.
        /// </summary>
        /// <param name="data1">fromday</param>
        /// <param name="data2">today</param>
        /// <returns>相隔的天数</returns>
        public static int SpanDays(string fromday, string today)
        {
            try
            {
                TimeSpan span = DateTime.Parse(today.Substring(0, 10)) - DateTime.Parse(fromday.Substring(0, 10));
                return span.Days;
            }
            catch
            {
                //throw new Exception(ex.Message +"" +fromday +"  " +today ) ; 
                return 0;
            }
        }
        /// <summary>
        /// 返回 QuarterFrom - QuarterTo 的季度.
        /// </summary>
        /// <param name="QuarterFrom">QuarterFrom</param>
        /// <param name="QuarterTo">QuarterTo</param>
        /// <returns>相隔的季度</returns>
        public static int SpanQuarter(string _APFrom, string _APTo)
        {
            DateTime fromdate = Convert.ToDateTime(_APFrom + "-01");
            DateTime todate = Convert.ToDateTime(_APTo + "-01");
            int i = 0;
            if (fromdate > todate)
                throw new Exception("选择出错！起始时期" + _APFrom + "不能大于终止时期" + _APTo + "!");

            while (fromdate <= todate)
            {
                i++;
                fromdate = fromdate.AddMonths(1);
            }

            int j = (i + 2) / 3;
            return j;
        }
        /// <summary>
        /// 到现在的天数。
        /// </summary>
        /// <param name="data1"></param>
        /// <returns></returns>
        public static int SpanDays(string data1)
        {
            TimeSpan span = DateTime.Now - DateTime.Parse(data1.Substring(0, 10));
            return span.Days;
        }
        /// <summary>
        /// 检查是否是一个字段或者表名称
        /// </summary>
        /// <param name="str">要检查的字段或者表名称</param>
        /// <returns>是否合法</returns>
        public static bool CheckIsFieldOrTableName(string str)
        {
            string s = str.Substring(0, 1);
            if (DataType.IsNumStr(s))
                return false;

            string chars = "~!@#$%^&*()_+`{}|:'<>?[];',./";
            if (chars.Contains(s) == true)
                return false;
            return true;
        }
        public static string ParseText2Html(string val)
        {
            //val = val.Replace("&", "&amp;");
            //val = val.Replace("<","&lt;");
            //val = val.Replace(">","&gt;");

            //val = val.Replace(char(34), "&quot;");
            //val = val.Replace(char(9), "&nbsp;&nbsp;&nbsp;");
            //val = val.Replace(" ", "&nbsp;");

            return val.Replace("\n", "<BR>").Replace("~", "'");

            //return val.Replace("\n", "<BR>&nbsp;&nbsp;").Replace("~", "'");

        }
        public static string ParseHtmlToText(string val)
        {
            if (val == null)
                return val;

            val = val.Replace("&nbsp;", " ");
            val = val.Replace("  ", " ");

            val = val.Replace("</td>", "");
            val = val.Replace("</TD>", "");

            val = val.Replace("</tr>", "");
            val = val.Replace("</TR>", "");

            val = val.Replace("<tr>", "");
            val = val.Replace("<TR>", "");

            val = val.Replace("</font>", "");
            val = val.Replace("</FONT>", "");

            val = val.Replace("</table>", "");
            val = val.Replace("</TABLE>", "");


            val = val.Replace("<BR>", "\n\t");
            val = val.Replace("<BR>", "\n\t");
            val = val.Replace("&nbsp;", " ");

            val = val.Replace("<BR><BR><BR><BR>", "<BR><BR>");
            val = val.Replace("<BR><BR><BR><BR>", "<BR><BR>");
            val = val.Replace("<BR><BR>", "<BR>");

            char[] chs = val.ToCharArray();

            bool isStartRec = false;
            string recStr = "";
            foreach (char c in chs)
            {
                if (c == '<')
                {
                    recStr = "";
                    isStartRec = true; /* 开始记录 */
                }

                if (isStartRec)
                {
                    recStr += c.ToString();
                }

                if (c == '>')
                {
                    isStartRec = false;

                    if (recStr == "")
                    {
                        isStartRec = false;
                        continue;
                    }

                    /* 开始分析这个标记内的东西。*/
                    string market = recStr.ToLower();
                    if (market.Contains("<img"))
                    {
                        /* 这是一个图片标记 */
                        isStartRec = false;
                        recStr = "";
                        continue;
                    }
                    else
                    {
                        val = val.Replace(recStr, "");
                        isStartRec = false;
                        recStr = "";
                    }
                }
            }


            val = val.Replace("字体：大中小", "");
            val = val.Replace("字体:大中小", "");

            val = val.Replace("  ", " ");
            val = val.Replace("\t", "");
            val = val.Replace("\n", "");
            val = val.Replace("\r", "");
            return val;
        }
        /// <summary>
        /// 将文本转换成可用做Name,Text的文本，文本中仅允许含有汉字、字母、数字、下划线
        /// </summary>
        /// <param name="nameStr">待转换的文本</param>
        /// <param name="maxLen">文本最大长度，0为不限制，超过maxLen，截取前maxLen字符长度</param>
        /// <returns></returns>
        public static string ParseStringForName(string nameStr, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(nameStr))
                return string.Empty;

            string nStr = Regex.Replace(nameStr, RegEx_Replace_OnlyHSZX, "");

            if (maxLen > 0 && nStr.Length > maxLen)
                return nStr.Substring(0, maxLen);

            return nStr;
        }
        /// <summary>
        /// 将文本转换成可用做No的文本，文本中仅允许含有字母、数字、下划线，且开头只能是字母
        /// </summary>
        /// <param name="noStr">待转换的文本</param>
        /// <param name="maxLen">文本最大长度，0为不限制，超过maxLen，截取前maxLen字符长度</param>
        /// <returns></returns>
        public static string ParseStringForNo(string noStr, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(noStr))
                return string.Empty;

            string nStr = Regex.Replace(Regex.Replace(noStr, RegEx_Replace_OnlySZX, ""), RegEx_Replace_FirstXZ, "");

            if (maxLen > 0 && nStr.Length > maxLen)
                return nStr.Substring(0, maxLen);

            return nStr;
        }
        /// <summary>
        /// 去除指定字符串中非数字的字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ParseStringOnlyIntNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return Regex.Replace(str, RegEx_Replace_OnlyIntNum, "");
        }
        /// <summary>
        /// 去除指定字符串中危险字符
        /// <para>注：含有这些字符的参数经过拼接，组成SQL可能包含危险语句</para>
        /// <para>涉及字符：' ; -- / &amp; &gt; &lt;</para>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ParseStringFilterDangerousSymbols(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return Regex.Replace(str, RegEx_Replace_FilterDangerousSymbols, "").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }
        /// <summary>
        /// 将中文转化成拼音
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ParseStringToPinyin(string exp)
        {
            exp = exp.Trim();
            string pinYin = "", str = null;
            char[] chars = exp.ToCharArray();
            foreach (char c in chars)
            {
                try
                {
                    str = CCFormAPI.ChinaMulTonesToPinYin(c.ToString());
                    if (str == null)
                    {
                        str = BP.Tools.chs2py.convert(c.ToString());
                    }
                    pinYin += str.Substring(0, 1).ToUpper() + str.Substring(1);
                }
                catch
                {
                    pinYin += c;
                }
            }
            return pinYin;
        }
        /// <summary>
        /// 转化成拼音第一个字母大字
        /// </summary>
        /// <param name="str">要转化的中文串</param>
        /// <returns>拼音</returns>
        public static string ParseStringToPinyinWordFirst(string str)
        {
            try
            {
                String _Temp = "";
                for (int i = 0; i < str.Length; i++)
                {
                    _Temp = _Temp + BP.DA.DataType.ParseStringToPinyin(str.Substring(i, 1));
                }
                return _Temp;
            }
            catch (Exception ex)
            {
                throw new Exception("@错误：" + str + "，不能转换成拼音。");
            }
        }
        /// <summary>
        /// 转化成拼音第一个字母大字
        /// </summary>
        /// <param name="str">要转化的中文串</param>
        /// <returns>拼音</returns>
        public static string ParseStringToPinyinJianXie(string str)
        {
            try
            {
                String _Temp = null;
                var re = string.Empty;
                for (int i = 0; i < str.Length; i++)
                {
                    re = BP.DA.DataType.ParseStringToPinyin(str.Substring(i, 1));
                    _Temp += re.Length == 0 ? "" : re.Substring(0, 1);
                }
                return _Temp;
            }
            catch (Exception ex)
            {
                throw new Exception("@错误：" + str + "，不能转换成拼音。");
            }
        }
        /// <summary>
        /// 转化成 decimal
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static decimal ParseExpToDecimal(string exp)
        {
            if (exp.Trim() == "")
                throw new Exception("DataType.ParseExpToDecimal要转换的表达式为空。");


            exp = exp.Replace("+-", "-");
            exp = exp.Replace("￥", "");
            //exp=exp.Replace(" ",""); 不能替换，因为有sql表达公式时间，会出现错误。
            exp = exp.Replace("\n", "");
            exp = exp.Replace("\t", "");

            exp = exp.Replace("＋", "+");
            exp = exp.Replace("－", "-");
            exp = exp.Replace("＊", "*");
            exp = exp.Replace("／", "/");
            exp = exp.Replace("）", ")");
            exp = exp.Replace("（", "(");

            exp = exp.Replace(".00.00", "00");

            exp = exp.Replace("--", "- -");


            if (exp.IndexOf("@") != -1)
                return 0;

            string val = exp.Substring(0, 1);
            if (val == "-")
                exp = exp.Substring(1);

            //  exp = exp.Replace("*100%", "*100");

            exp = exp.Replace("*100%", "*1");

            try
            {
                return decimal.Parse(exp);
            }
            catch
            {
            }

            try
            {
                string sql = "SELECT  " + exp + " as Num  ";
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.Access:
                        sql = "SELECT  " + exp + " as Num  ";
                        return DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    case DBType.Oracle:
                        sql = "SELECT  " + exp + " NUM from DUAL ";
                        return DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    case DBType.Informix:
                        sql = "SELECT  " + exp + " NUM from  taa_onerow ";
                        return DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineInfo(ex.Message);
                /* 如果抛出异常就按 0  计算。 */
                return 0;
            }

            exp = exp.Replace("-0", "");


            try
            {
                BP.Tools.StringExpressionCalculate sc = new BP.Tools.StringExpressionCalculate();
                return sc.TurnToDecimal(exp);
            }
            catch (Exception ex)
            {
                if (exp.IndexOf("/") != -1)
                    return 0;
                throw new Exception("表达式(\"" + exp + "\")计算错误：" + ex.Message);
            }
        }
        public static string ParseFloatToCash(float money)
        {
            if (money == 0)
                return "零圆零角零分";
            BP.Tools.DealString d = new BP.Tools.DealString();
            d.InputString = money.ToString();
            d.ConvertToChineseNum();
            return d.OutString;
        }
        public static string ParseFloatToRMB(float money)
        {
            if (money == 0)
                return "零圆零角零分";
            BP.Tools.DealString d = new BP.Tools.DealString();
            d.InputString = money.ToString();
            d.ConvertToChineseNum();
            return d.OutString;
        }
        /// <summary>
        /// 得到一个日期,根据系统
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        public DateTime Parse(string dataStr)
        {
            return DateTime.Parse(dataStr);
        }
        /// <summary>
        /// 系统定义的时间格式 yyyy-MM-dd .
        /// </summary>
        public static string SysDataFormat
        {
            get
            {
                return "yyyy-MM-dd";
            }
        }
        #region 与周相关.
        /// <summary>
        /// 当前周次
        /// </summary>
        public static int CurrentWeek
        {
            get
            {
                System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                int weekOfYear = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                return weekOfYear;
            }
        }
        /// <summary>
        /// 根据一个日期，获得该日期属于一年的第几周.
        /// </summary>
        /// <param name="dataTimeStr">日期时间串，要符合bp格式.</param>
        /// <returns>该日期属于所在年度的第几周</returns>
        public static int CurrentWeekGetWeekByDay(string dataTimeStr)
        {
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(DataType.ParseSysDate2DateTime(dataTimeStr),
                System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
        #endregion
        /// <summary>
        /// 格式化日期类型
        /// </summary>
        /// <param name="dataStr">日期字符串</param>
        /// <returns>标准的日期类型</returns>
        public static string FormatDataTime(string dataStr)
        {

            return dataStr;
        }
        /// <summary>
        /// 当前的日期
        /// </summary>
        public static string CurrentData
        {
            get
            {
                return DateTime.Now.ToString(DataType.SysDataFormat);
            }
        }
        public static string CurrentTime
        {
            get
            {
                return DateTime.Now.ToString("hh:mm");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string CurrentTimeQuarter
        {
            get
            {
                return DateTime.Now.ToString("hh:mm");
            }
        }
        /// <summary>
        /// 给一个时间，返回一个刻种时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ParseTime2TimeQuarter(string time)
        {
            string hh = time.Substring(0, 3);
            int mm = int.Parse(time.Substring(3, 2));
            if (mm == 0)
            {
                return hh + "00";
            }

            if (mm < 15)
            {
                return hh + "00";
            }
            if (mm >= 15 && mm < 30)
            {
                return hh + "15";
            }

            if (mm >= 30 && mm < 45)
            {
                return hh + "30";
            }

            if (mm >= 45 && mm < 60)
            {
                return hh + "45";
            }
            return time;
        }
        public static string CurrentDay
        {
            get
            {
                return DateTime.Now.ToString("dd");
            }
        }

        /// <summary>
        /// 当前的会计期间
        /// </summary>
        public static string CurrentAP
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
        }
        /// <summary>
        /// 当前的会计期间
        /// </summary>
        public static string CurrentYear
        {
            get
            {
                return DateTime.Now.ToString("yyyy");
            }
        }
        public static string CurrentMonth
        {
            get
            {
                return DateTime.Now.ToString("MM");
            }
        }
        /// <summary>
        /// 当前的会计期间 yyyy-MM
        /// </summary>
        public static string CurrentYearMonth
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
        }
        public static string GetJDByMM(string mm)
        {
            string jd = "01";
            switch (mm)
            {
                case "01":
                case "02":
                case "03":
                    jd = "01";
                    break;
                case "04":
                case "05":
                case "06":
                    jd = "04";
                    break;
                case "07":
                case "08":
                case "09":
                    jd = "07";
                    break;
                case "10":
                case "11":
                case "12":
                    jd = "10";
                    break;
                default:
                    throw new Exception("@不是有效的月份格式" + mm);
            }
            return jd;
        }
        /// <summary>
        /// 当前的季度期间yyyy-MM
        /// </summary>
        public static string CurrentAPOfJD
        {
            get
            {
                return DateTime.Now.ToString("yyyy") + "-" + DataType.GetJDByMM(DateTime.Now.ToString("MM"));
            }
        }
        /// <summary>
        /// 当前的季度的前一个季度.
        /// </summary>
        public static string CurrentAPOfJDOfFrontFamily
        {
            get
            {
                DateTime now = DateTime.Now.AddMonths(-3);
                return now.ToString("yyyy") + "-" + DataType.GetJDByMM(now.ToString("MM"));
            }
        }
        /// <summary>
        /// yyyy-JD
        /// </summary>
        public static string CurrentAPOfPrevious
        {
            get
            {
                int m = int.Parse(DateTime.Now.ToString("MM"));
                return DateTime.Now.ToString("yyyy-MM");
            }
        }
        /// <summary>
        /// 取出当前月份的上一个月份
        /// </summary>
        public static string CurrentNYOfPrevious
        {
            get
            {
                DateTime dt = DateTime.Now;
                dt = dt.AddMonths(-1);
                return dt.ToString("yyyy-MM");
            }
        }
        /// <summary>
        /// 取出当前月份的上上一个月份
        /// </summary>
        public static string ShangCurrentNYOfPrevious
        {
            get
            {
                DateTime dt = DateTime.Now;
                dt = dt.AddMonths(-2);
                return dt.ToString("yyyy-MM");
            }
        }
        /// <summary>
        /// 当前的季度期间
        /// </summary>
        public static string CurrentAPOfYear
        {
            get
            {
                return DateTime.Now.ToString("yyyy");
            }
        }
        /// <summary>
        /// 当前的日期时间
        /// </summary>
        public static string CurrentDataTime
        {
            get
            {
                return DateTime.Now.ToString(DataType.SysDataTimeFormat);
            }
        }
        public static string CurrentDataTimeOfDef
        {
            get
            {
                switch (BP.Web.WebUser.SysLang)
                {
                    case "CH":
                    case "B5":
                        return CurrentDataTimeCNOfShort;
                    case "EN":
                        return DateTime.Now.ToString("MM/DD/YYYY");
                    default:
                        break;
                }
                return CurrentDataTimeCNOfShort;
            }
        }
        public static string CurrentDataTimeCNOfShort
        {
            get
            {
                return DateTime.Now.ToString("yy年MM月dd日 HH时mm分");
            }
        }
        public static string CurrentDataTimeCNOfLong
        {
            get
            {
                return DateTime.Now.ToString("yy年MM月dd日 HH时mm分ss秒");
            }
        }
        public static string CurrentDataCNOfShort
        {
            get
            {
                return DateTime.Now.ToString("yy年MM月dd日");
            }
        }
        public static string CurrentDataCNOfLong
        {
            get
            {
                return DateTime.Now.ToString("yyyy年MM月dd日");
            }
        }
        /// <summary>
        /// 当前的日期时间
        /// </summary>
        public static string CurrentDataTimeCN
        {
            get
            {
                return DateTime.Now.ToString(DataType.SysDataFormatCN) + "，" + GetWeekName(DateTime.Now.DayOfWeek);
            }
        }
        private static string GetWeekName(System.DayOfWeek dw)
        {
            switch (dw)
            {
                case DayOfWeek.Monday:
                    return "星期一";
                case DayOfWeek.Thursday:
                    return "星期四";
                case DayOfWeek.Friday:
                    return "星期五";
                case DayOfWeek.Saturday:
                    return "星期六";
                case DayOfWeek.Sunday:
                    return "星期日";
                case DayOfWeek.Tuesday:
                    return "星期二";
                case DayOfWeek.Wednesday:
                    return "星期三";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 当前的日期时间
        /// </summary>
        public static string CurrentDataTimess
        {
            get
            {
                return DateTime.Now.ToString(DataType.SysDataTimeFormat + ":ss");
            }
        }
        public static string ParseSysDateTime2SysDate(string sysDateformat)
        {
            try
            {
                return sysDateformat.Substring(0, 10);
            }
            catch (Exception ex)
            {
                throw new Exception("日期格式错误:" + sysDateformat + " errorMsg=" + ex.Message);
            }
        }
        /// <summary>
        /// 转化为友好的日期
        /// </summary>
        /// <param name="sysDateformat">日期</param>
        /// <returns></returns>
        public static string ParseSysDate2DateTimeFriendly(string sysDateformat)
        {
            BP.DA.DTTemp dt = new DTTemp();
            return dt.DateStringFromNow(sysDateformat);
        }
        /// <summary>
        /// 把chichengsoft本系统日期格式转换为系统日期格式。
        /// </summary>
        /// <param name="sysDateformat">yyyy-MM-dd</param>
        /// <returns>DateTime</returns>
        public static DateTime ParseSysDate2DateTime(string sysDateformat)
        {
            if (sysDateformat == null || sysDateformat.Trim().Length == 0)
                return DateTime.Now;


            try
            {
                if (sysDateformat.Length > 10)
                    return ParseSysDateTime2DateTime(sysDateformat);

                sysDateformat = sysDateformat.Trim();
                //DateTime.Parse(sysDateformat,
                string[] strs = null;
                if (sysDateformat.IndexOf("-") != -1)
                {
                    strs = sysDateformat.Split('-');
                }

                if (sysDateformat.IndexOf("/") != -1)
                {
                    strs = sysDateformat.Split('/');
                }

                int year = int.Parse(strs[0]);
                int month = int.Parse(strs[1]);
                int day = int.Parse(strs[2]);

                //DateTime dt= DateTime.Now;
                return new DateTime(year, month, day, 0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("日期[" + sysDateformat + "]转换出现错误:" + ex.Message + "无效的日期是格式。");
            }
            //return dt;			 
        }
        /// <summary>
        /// 2005-11-04 09:12
        /// </summary>
        /// <param name="sysDateformat"></param>
        /// <returns></returns>
        public static DateTime ParseSysDateTime2DateTime(string sysDateformat)
        {
            try
            {
                return Convert.ToDateTime(sysDateformat);
            }
            catch (Exception ex)
            {
                throw new Exception("@时间格式不正确:" + sysDateformat + "@技术信息:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取两个时间之间的字符串表示形式，如：1天2时34分
        /// <para>added by liuxc,2014-12-4</para>
        /// </summary>
        /// <param name="t1">开始时间</param>
        /// <param name="t2">结束时间</param>
        /// <returns>返回：x天x时x分</returns>
        public static string GetSpanTime(DateTime t1, DateTime t2)
        {
            var span = t2 - t1;
            var days = span.Days;
            var hours = span.Hours;
            var minutes = span.Minutes;

            if (days == 0 && hours == 0 && minutes == 0)
                minutes = span.Seconds > 0 ? 1 : 0;

            var spanStr = string.Empty;

            if (days > 0)
                spanStr += days + "天";

            if (hours > 0)
                spanStr += hours + "时";

            if (minutes > 0)
                spanStr += minutes + "分";

            if (spanStr.Length == 0)
                spanStr = "0分";

            return spanStr;
        }
        /// <summary>
        /// 获得两个日期之间的天数
        /// </summary>
        /// <param name="dtoffrom"></param>
        /// <param name="dtofto"></param>
        /// <returns></returns>
        public static float GeTimeLimits(string dtoffrom, string dtofto)
        {
            DateTime dtfrom = DataType.ParseSysDate2DateTime(dtoffrom);
            DateTime dtto = DataType.ParseSysDate2DateTime(dtofto);

            TimeSpan ts = dtto - dtfrom;
            return  (float)Math.Round(ts.TotalDays, 2);
        }
        /// <summary>
        /// 获得两个时间的参数
        /// </summary>
        /// <param name="dtoffrom">时间从</param>
        /// <returns></returns>
        public static float GeTimeLimits(string dtoffrom )
        {
            return GeTimeLimits(dtoffrom, DataType.CurrentDataTime);
        }
        public static float GetSpanMinute(string fromdatetim, string toDateTime)
        {
            DateTime dtfrom = DataType.ParseSysDateTime2DateTime(fromdatetim);
            DateTime dtto = DataType.ParseSysDateTime2DateTime(toDateTime);

            TimeSpan ts = dtfrom - dtto;
            return (float)ts.TotalMinutes;
        }
        /// <summary>
        /// 到现在的时间
        /// </summary>
        /// <param name="fromdatetim"></param>
        /// <returns>分中数</returns>
        public static int GetSpanMinute(string fromdatetim)
        {
            DateTime dtfrom = DataType.ParseSysDateTime2DateTime(fromdatetim);
            DateTime dtto = DateTime.Now;
            TimeSpan ts = dtfrom - dtto;
            return (int)ts.TotalMinutes + ts.Hours * 60;
        }
        /// <summary>
        /// 系统定义日期时间格式 yyyy-MM-dd hh:mm
        /// </summary>
        public static string SysDataTimeFormat
        {
            get
            {
                return "yyyy-MM-dd HH:mm";
            }
        }
        public static string SysDataFormatCN
        {
            get
            {
                return "yyyy年MM月dd日";
            }
        }
        public static string SysDatatimeFormatCN
        {
            get
            {
                return "yyyy年MM月dd日 HH时mm分";
            }
        }
        public static DBUrlType GetDBUrlByString(string strDBUrl)
        {
            switch (strDBUrl)
            {
                case "AppCenterDSN":
                    return DBUrlType.AppCenterDSN;
                case "DBAccessOfOracle1":
                    return DBUrlType.DBAccessOfOracle1;
                case "DBAccessOfOracle2":
                    return DBUrlType.DBAccessOfOracle2;
                case "DBAccessOfMSSQL1":
                    return DBUrlType.DBAccessOfMSSQL1;
                case "DBAccessOfMSSQL2":
                    return DBUrlType.DBAccessOfMSSQL2;
                case "DBAccessOfOLE":
                    return DBUrlType.DBAccessOfOLE;
                case "DBAccessOfODBC":
                    return DBUrlType.DBAccessOfODBC;
                default:
                    throw new Exception("@没有此类型[" + strDBUrl + "]");
            }
        }
        public static int GetDataTypeByString(string datatype)
        {
            switch (datatype)
            {
                case "AppBoolean":
                    return DataType.AppBoolean;
                case "AppDate":
                    return DataType.AppDate;
                case "AppDateTime":
                    return DataType.AppDateTime;
                case "AppDouble":
                    return DataType.AppDouble;
                case "AppFloat":
                    return DataType.AppFloat;
                case "AppInt":
                    return DataType.AppInt;
                case "AppMoney":
                    return DataType.AppMoney;
                case "AppString":
                    return DataType.AppString;
                default:
                    throw new Exception("@没有此类型" + datatype);
            }
        }
        public static string GetDataTypeDese(int datatype)
        {
            if (Web.WebUser.SysLang == "CH")
            {
                switch (datatype)
                {
                    case DataType.AppBoolean:
                        return "布尔(Int)";
                    case DataType.AppDate:
                        return "日期nvarchar";
                    case DataType.AppDateTime:
                        return "日期时间nvarchar";
                    case DataType.AppDouble:
                        return "双精度(double)";
                    case DataType.AppFloat:
                        return "浮点(float)";
                    case DataType.AppInt:
                        return "整型(int)";
                    case DataType.AppMoney:
                        return "货币(float)";
                    case DataType.AppString:
                        return "字符(nvarchar)";
                    default:
                        throw new Exception("@没有此类型");
                }
            }

            switch (datatype)
            {
                case DataType.AppBoolean:
                    return "Boolen";
                case DataType.AppDate:
                    return "Date";
                case DataType.AppDateTime:
                    return "Datetime";
                case DataType.AppDouble:
                    return "Double";
                case DataType.AppFloat:
                    return "Float";
                case DataType.AppInt:
                    return "Int";
                case DataType.AppMoney:
                    return "Money";
                case DataType.AppString:
                    return "Nvarchar";
                default:
                    throw new Exception("@没有此类型");
            }
        }
        /// <summary>
        /// 产生适应的图片大小
        /// 用途:在固定容器大小的位置，显示固定的图片。
        /// </summary>
        /// <param name="height">容器高度</param>
        /// <param name="width">容器宽度</param>
        /// <param name="AdaptHeight">原始图片高度</param>
        /// <param name="AdaptWidth">原始图片宽度</param>
        /// <param name="isFull">是否填充:是,小图片将会放大填充容器. 否,小图片不放大保留原来的大小</param>
        public static void GenerPictSize(float panelHeight, float panelWidth, ref float AdaptHeight, ref float AdaptWidth, bool isFullPanel)
        {
            if (isFullPanel == false)
            {
                if (panelHeight <= AdaptHeight && panelWidth <= AdaptWidth)
                    return;
            }

            float zoom = 1;
            zoom = System.Math.Min(panelHeight / AdaptHeight, panelWidth / AdaptWidth);
            AdaptHeight = AdaptHeight * zoom;
            AdaptWidth = AdaptWidth * zoom;
        }

        #region 正则表达式
        /// <summary>
        /// (RegEx.Replace操作使用)仅含有汉字、数字、字母、下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("姓名@-._#:：“｜：$?>a:12",RegEx_Replace_OnlyHSZX,""));</para>
        /// <para>   输出：姓名_a12</para>
        /// </summary>
        public const string RegEx_Replace_OnlyHSZX = @"[^0-9a-zA-Z_\u4e00-\u9fa5]";
        /// <summary>
        /// (RegEx.Replace操作使用)仅含有数字、字母、下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("姓名@-._#:：“｜：$?>a:12",RegEx_Replace_OnlySZX,""));</para>
        /// <para>   输出：_a12</para>
        /// </summary>
        public const string RegEx_Replace_OnlySZX = @"[\u4e00-\u9fa5]|[^0-9a-zA-Z_]";
        /// <summary>
        /// (RegEx.Replace操作使用)字符串开头不能为数字或下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("_12_a1",RegEx_Replace_FirstXZ,""));</para>
        /// <para>   输出：a1</para>
        /// </summary>
        public const string RegEx_Replace_FirstXZ = "^(_|[0-9])+";
        /// <summary>
        /// (RegEx.Replace操作使用)仅含有整形数字
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("_12_a1",RegEx_Replace_OnlyIntNum,""));</para>
        /// <para>   输出：121</para>
        /// </summary>
        public const string RegEx_Replace_OnlyIntNum = "[^0-9]";
        /// <summary>
        /// (RegEx.Replace操作使用)字符串不能含有指定危险字符
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("'_1--2/_a1",RegEx_Replace_FilterDangerousSymbols,""));</para>
        /// <para>   输出：_12_a1</para>
        /// </summary>
        public const string RegEx_Replace_FilterDangerousSymbols = "[';/]|[-]{2}";
        #endregion

        #region 数据类型。
        /// <summary>
        /// string
        /// </summary>
        public const int AppString = 1;
        /// <summary>
        /// int
        /// </summary>
        public const int AppInt = 2;
        /// <summary>
        /// float
        /// </summary>
        public const int AppFloat = 3;
        /// <summary>
        /// AppBoolean
        /// </summary>
        public const int AppBoolean = 4;
        /// <summary>
        /// AppDouble
        /// </summary>
        public const int AppDouble = 5;
        /// <summary>
        /// AppDate
        /// </summary>
        public const int AppDate = 6;
        /// <summary>
        /// AppDateTime
        /// </summary>
        public const int AppDateTime = 7;
        /// <summary>
        /// AppMoney
        /// </summary>
        public const int AppMoney = 8;
        #endregion

        #region 其他方法.
        public static string StringToDateStr(string str)
        {
            try
            {
                DateTime dt = DateTime.Parse(str);
                string year = dt.Year.ToString();
                string month = dt.Month.ToString();
                string day = dt.Day.ToString();
                return year + "-" + month.PadLeft(2, '0') + "-" + day.PadLeft(2, '0');
                //return str;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public static string GenerSpace(int spaceNum)
        {
            if (spaceNum <= 0)
                return "";

            string strs = "";
            while (spaceNum != 0)
            {
                strs += "&nbsp;&nbsp;";
                spaceNum--;
            }
            return strs;
        }
        public static string GenerBR(int spaceNum)
        {
            string strs = "";
            while (spaceNum != 0)
            {
                strs += "<BR>";
                spaceNum--;
            }
            return strs;
        }
        public static bool IsImgExt(string ext)
        {
            ext = ext.Replace(".", "").ToLower();
            switch (ext)
            {
                case "gif":
                case "jpg":
                case "jepg":
                case "jpeg":
                case "bmp":
                case "png":
                case "tif":
                case "gsp":
                case "mov":
                case "psd":
                case "tiff":
                case "wmf":
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsVoideExt(string ext)
        {
            ext = ext.Replace(".", "").ToLower();
            switch (ext)
            {
                case "mp3":
                case "mp4":
                case "asf":
                case "wma":
                case "rm":
                case "rmvb":
                case "mpg":
                case "wmv":
                case "quicktime":
                case "avi":
                case "flv":
                case "mpeg":
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 判断是否是Num 字串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumStr(string str)
        {
            try
            {
                decimal d = decimal.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 是不时奇数
        /// </summary>
        /// <param name="num">will judg value</param>
        /// <returns></returns>
        public static bool IsQS(int num)
        {
            int ii = 0;
            for (int i = 0; i < 500; i++)
            {
                if (num == ii)
                    return false;
                ii = ii + 2;
            }
            return true;
        }

        public static bool StringToBoolean(string str)
        {
            if (str == null || str == "" || str == ",nbsp;")
                return false;

            if (str == "0" || str == "1")
            {
                if (str == "0")
                    return false;
                else
                    return true;
            }
            else if (str == "true" || str == "false")
            {
                if (str == "false")
                    return false;
                else
                    return true;

            }
            else if (str == "是" || str == "否")
            {
                if (str == "否")
                    return false;
                else
                    return true;
            }
            else
                throw new Exception("@要转换的[" + str + "]不是bool 类型");

            

             
        }
        #endregion 其他方法.

        #region 与门户相关的.
#if DEBUG
        static TimeSpan ts = new TimeSpan(0, 10, 0);
#else
       static TimeSpan ts = new TimeSpan(0, 1, 0);
#endif

        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static BP.En30.ccportal.PortalInterfaceSoapClient GetPortalInterfaceSoapClientInstance()
        {
            var basicBinding = new BasicHttpBinding()
            {
                //CloseTimeout = ts,
                //OpenTimeout = ts,
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "PortalInterfaceSoapClient"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;

            //url.
            string url =  DataType.BPMHost + "/DataUser/PortalInterface.asmx";

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(BP.En30.ccportal.PortalInterfaceSoapClient).GetConstructor(
                new Type[] { 
                    typeof(Binding),
                    typeof(EndpointAddress) 
                });
            return (BP.En30.ccportal.PortalInterfaceSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;
                _BPMHost = "http://" + System.Web.HttpContext.Current.Request.Url.Authority;
                return _BPMHost;
            }
        }
        #endregion 与门户相关的.
    }
}
