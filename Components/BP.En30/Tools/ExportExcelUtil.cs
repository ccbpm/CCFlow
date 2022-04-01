using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Tools
{
    public class ExportExcelUtil
    {
        public static string ExportGroupExcel(DataSet ds, string title, string paras)
        {
            DataTable dt = ds.Tables["GroupSearch"];
            DataTable AttrsOfNum = ds.Tables["AttrsOfNum"];
            DataTable AttrsOfGroup = ds.Tables["AttrsOfGroup"];

            title = title.Trim();
            string filename = title + "Ep" + title + ".xls";
            string file = filename;
            bool flag = true;
            string filepath = SystemConfig.PathOfTemp;

            #region 参数及变量设置

            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);



            filename = filepath + filename;

            //filename = HttpUtility.UrlEncode(filename);

            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            try
            {
                objStreamWriter.WriteLine(Convert.ToChar(9) + title + Convert.ToChar(9));
                string strLine = "序号" + Convert.ToChar(9);
                //生成文件标题
                foreach (DataRow attr in AttrsOfGroup.Rows)
                {
                    strLine = strLine + attr["Name"] + Convert.ToChar(9);
                }
                foreach (DataRow attr in AttrsOfNum.Rows)
                {
                    strLine = strLine + attr["Name"] + Convert.ToChar(9);
                }

                objStreamWriter.WriteLine(strLine);
                strLine = "";
                foreach (DataRow dr in dt.Rows)
                {
                    strLine = strLine + dr["IDX"] + Convert.ToChar(9);
                    foreach (DataRow attr in AttrsOfGroup.Rows)
                    {
                        strLine = strLine + dr[attr["KeyOfEn"] + "T"] + Convert.ToChar(9);

                    }
                    foreach (DataRow attr in AttrsOfNum.Rows)
                    {

                        strLine = strLine + dr[attr["KeyOfEn"].ToString()] + Convert.ToChar(9);
                    }

                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }

                strLine = "汇总" + Convert.ToChar(9);
                foreach (DataRow attr in AttrsOfGroup.Rows)
                {

                    strLine = strLine + "" + Convert.ToChar(9);
                }

                foreach (DataRow attr in AttrsOfNum.Rows)
                {
                    double d = 0;
                    foreach (DataRow dtr in dt.Rows)
                    {
                        d += Double.Parse(dtr[attr["KeyOfEn"].ToString()].ToString());
                    }
                    if (paras.Contains(attr["KeyOfEn"] + "=AVG"))
                    {
                        if (dt.Rows.Count != 0)
                        {
                            d = Double.Parse((d / dt.Rows.Count).ToString("0.0000"));
                        }

                    }

                    if (Int32.Parse(attr["MyDataType"].ToString()) == DataType.AppInt)
                    {
                        if (paras.Contains(attr["KeyOfEn"] + "=AVG"))
                            strLine = strLine + d + Convert.ToChar(9);
                        else
                            strLine = strLine + (Int32)d + Convert.ToChar(9);
                    }
                    else
                    {
                        strLine = strLine + d + Convert.ToChar(9); ;
                    }

                }

                objStreamWriter.WriteLine(strLine);
                strLine = "";
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
            //DelExportedTempFile(filepath);
            #endregion

            if (flag)
            {
                file = "/DataUser/Temp/" + file;

            }

            return file;
        }
        public static string ExportDGToExcel(DataTable dt, Entity en, string title, Attrs mapAttrs = null, string filename = null)
        {
            if (filename == null)
                filename = title + "_" + DataType.CurrentDateCNOfLong + "_" + WebUser.No + ".xls";
            string file = filename;
            bool flag = true;
            string filepath = SystemConfig.PathOfTemp;

            #region 参数及变量设置


            //如果导出目录没有建立，则建立.
            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);

            filename = filepath + filename;

            if (File.Exists(filename))
                File.Delete(filename);

            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            try
            {
                Attrs attrs = null;
                if (mapAttrs != null)
                    attrs = mapAttrs;
                else
                    attrs = en.EnMap.Attrs;

                Attrs selectedAttrs = null;
                BP.Sys.UIConfig cfg = new UIConfig(en);

                if (cfg.ShowColumns.Length == 0)
                    selectedAttrs = attrs;
                else
                {
                    selectedAttrs = new Attrs();

                    foreach (Attr attr in attrs)
                    {
                        bool contain = false;

                        foreach (string col in cfg.ShowColumns)
                        {
                            if (col == attr.Key)
                            {
                                contain = true;
                                break;
                            }
                        }

                        if (contain)
                            selectedAttrs.Add(attr);
                    }
                }

                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + title + Convert.ToChar(9));
                objStreamWriter.WriteLine();
                string strLine = "";

                //添加标签，解决数字在excel中变为科学计数法问题
                strLine = "<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\"> ";
                strLine += "<tr>";
                //生成文件标题
                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.Key.Equals("OID"))
                        continue;

                    if (attr.Key.Equals("WorkID"))
                        continue;

                    if (attr.Key.Equals("MyNum"))
                        continue;

                    if (attr.IsFKorEnum)
                        continue;

                    if (attr.UIVisible == false && attr.MyFieldType != FieldType.RefText)
                        continue;

                    if (attr.Key.Equals("MyFilePath") || attr.Key.Equals("MyFileExt")
                        || attr.Key.Equals("WebPath") || attr.Key.Equals("MyFileH")
                        || attr.Key.Equals("MyFileW") || attr.Key.Equals("MyFileSize")
                        || attr.Key.Equals("RefPK"))
                        continue;

                    if (attr.MyFieldType == FieldType.RefText)
                        strLine += "<td>" + attr.Desc.Replace("名称", "") + Convert.ToChar(9) + "</td>";
                    else
                        strLine += "<td>" + attr.Desc + Convert.ToChar(9) + "</td>";
                }
                strLine += "</tr>";
                objStreamWriter.WriteLine(strLine);
                strLine = "";

                foreach (DataRow dr in dt.Rows)
                {
                    strLine = "</tr>";
                    foreach (Attr attr in selectedAttrs)
                    {
                        if (attr.IsFKorEnum)
                            continue;

                        if (attr.UIVisible == false && attr.MyFieldType != FieldType.RefText)
                            continue;

                        if (attr.Key.Equals("OID"))
                            continue;

                        if (attr.Key.Equals("MyNum"))
                            continue;

                        if (attr.Key.Equals("WorkID") == true)
                            continue;

                        if (attr.Key.Equals("MyFilePath") || attr.Key.Equals("MyFileExt")
                            || attr.Key.Equals("WebPath") || attr.Key.Equals("MyFileH")
                            || attr.Key.Equals("MyFileW") || attr.Key.Equals("MyFileSize")
                            || attr.Key.Equals("RefPK"))
                            continue;
                        if (dt.Columns.Contains(attr.Key) == false)
                            continue;


                        if (attr.MyDataType == DataType.AppBoolean)
                        {
                            strLine += "<td>" + (dr[attr.Key].Equals(1) ? "是" : "否") + Convert.ToChar(9) + "</td>";
                        }
                        else
                        {
                            string text = "";
                            if (attr.IsFKorEnum || attr.IsFK)
                                text = dr[attr.Key + "Text"].ToString();
                            else if (dt.Columns.Contains(attr.Key + "T") == true)
                                text = dr[attr.Key + "T"].ToString();
                            else
                                text = dr[attr.Key].ToString();

                            if (attr.Key == "FK_NY" && DataType.IsNullOrEmpty(text) == true)
                            {
                                text = dr[attr.Key].ToString();
                            }
                            if (DataType.IsNullOrEmpty(text) == false && (text.Contains("\n") == true || text.Contains("\r") == true))
                            {
                                text = text.Replace("\n", " ");
                                text = text.Replace("\r", " ");
                            }
                            strLine += "<td style=\"vnd.ms-excel.numberformat:@\">" + text + " " + Convert.ToChar(9) + "</td>";
                        }
                    }
                    strLine += "</tr>";
                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }


                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + " 制表人：" + Convert.ToChar(9) + WebUser.Name + Convert.ToChar(9) + "日期：" + Convert.ToChar(9) + DateTime.Now.ToShortDateString());

            }
            catch (Exception e)
            {
                flag = false;
                throw new Exception("数据导出有问题," + e.Message);
            }
            finally
            {
                objStreamWriter.Close();
                objFileStream.Close();
            }
            #endregion

            #region 删除掉旧的文件
            //DelExportedTempFile(filepath);
            #endregion

            if (flag)
            {
                file = "/DataUser/Temp/" + file;
                //this.Write_Javascript(" window.open('"+ Request.ApplicationPath + @"/Report/Exported/" + filename +"'); " );
                //this.Write_Javascript(" window.open('"+Request.ApplicationPath+"/Temp/" + file +"'); " );
            }

            return file;
        }

    }
}
