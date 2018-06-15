using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using System.IO;

namespace BP.WF
{
    public class PrintJoin
    {
        static Microsoft.Office.Interop.Word.Application objApp = null;
        static Document objDocLast = null;
        static Document objDocBeforeLast = null;
        public PrintJoin()
        {
            objApp = new Application();
        }
        #region 打开文件
        public static void Open(string tempDoc)
        {
            object objTempDoc = tempDoc;
            object objMissing = System.Reflection.Missing.Value;
            objApp = new Application();
            objDocLast = new Document();

            objDocLast = objApp.Documents.Open(
               ref objTempDoc, //FileName 
               ref objMissing, //ConfirmVersions 
               ref objMissing, //ReadOnly 
               ref objMissing, //AddToRecentFiles 
               ref objMissing, //PasswordDocument 
               ref objMissing, //PasswordTemplate 
               ref objMissing, //Revert 
               ref objMissing, //WritePasswordDocument 
               ref objMissing, //WritePasswordTemplate 
               ref objMissing, //Format 
               ref objMissing, //Enconding 
               ref objMissing, //Visible 
               ref objMissing, //OpenAndRepair 
               ref objMissing, //DocumentDirection 
               ref objMissing, //NoEncodingDialog 
               ref objMissing //XMLTransform 
               );
            objDocLast.Activate();
        }
        #endregion

        #region 保存文件到输出模板
        public static void SaveAs(string outDoc)
        {
            object objMissing = System.Reflection.Missing.Value;
            object objOutDoc = outDoc;
            objDocLast.SaveAs(
            ref objOutDoc, //FileName 
            ref objMissing, //FileFormat 
            ref objMissing, //LockComments 
            ref objMissing, //PassWord 
            ref objMissing, //AddToRecentFiles 
            ref objMissing, //WritePassword 
            ref objMissing, //ReadOnlyRecommended 
            ref objMissing, //EmbedTrueTypeFonts 
            ref objMissing, //SaveNativePictureFormat 
            ref objMissing, //SaveFormsData 
            ref objMissing, //SaveAsAOCELetter, 
            ref objMissing, //Encoding 
            ref objMissing, //InsertLineBreaks 
            ref objMissing, //AllowSubstitutions 
            ref objMissing, //LineEnding 
            ref objMissing //AddBiDiMarks 
            );
        }
        #endregion

        #region 循环合并多个文件（插入合并文件）
        /// <summary> 
        /// 循环合并多个文件（插入合并文件） 
        /// </summary> 
        /// <param name="tempDoc">模板文件</param> 
        /// <param name="arrCopies">需要合并的文件</param> 
        /// <param name="outDoc">合并后的输出文件</param> 
        public static void InsertMerge(string tempDoc, List<string> arrCopies, string outDoc)
        {
            object objMissing = Missing.Value;
            object objFalse = false;
            object confirmConversion = false;
            object link = false;
            object attachment = false;
            try
            {
                //打开模板文件 
                Open(tempDoc);
                foreach (string strCopy in arrCopies)
                {
                    objApp.Selection.InsertFile(
                    strCopy,
                    ref objMissing,
                    ref confirmConversion,
                    ref link,
                    ref attachment
                    );
                }
                //保存到输出文件 
                SaveAs(outDoc);
                foreach (Document objDocument in objApp.Documents)
                {
                    objDocument.Close(
                    ref objFalse, //SaveChanges 
                    ref objMissing, //OriginalFormat 
                    ref objMissing //RouteDocument 
                    );
                }
            }
            finally
            {
                objApp.Quit(
                ref objMissing, //SaveChanges 
                ref objMissing, //OriginalFormat 
                ref objMissing //RoutDocument 
                );
                objApp = null;
            }
        }
        /// <summary> 
        /// 循环合并多个文件（插入合并文件） 
        /// </summary> 
        /// <param name="tempDoc">模板文件</param> 
        /// <param name="arrCopies">需要合并的文件</param> 
        /// <param name="outDoc">合并后的输出文件</param> 
        public static void InsertMerge(string tempDoc, string strCopyFolder, string outDoc)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            List<string> files = new List<string>();
            for (int i = 0; i < arrFiles.Count(); i++)
            {
                if (arrFiles[i].Contains("doc"))
                {
                    files.Add(arrFiles[i]);
                }
            }
            InsertMerge(tempDoc, files, outDoc);
        }
        #endregion

        #region 合并文件夹下的所有txt文件

        /// <summary>
        /// 合并多个txt文件
        /// </summary>
        /// <param name="infileName">文件存在的路劲</param>
        /// <param name="outfileName">输出文件名称</param>
        public void CombineFile(string filePath, string outfileName)
        {
            string[] infileName = Directory.GetFiles(filePath, "*.txt");
            int b;
            int n = infileName.Length;
            FileStream[] fileIn = new FileStream[n];
            using (FileStream fileOut = new FileStream(outfileName, FileMode.Create))
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        fileIn[i] = new FileStream(infileName[i], FileMode.Open);
                        while ((b = fileIn[i].ReadByte()) != -1)
                            fileOut.WriteByte((byte)b);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        fileIn[i].Close();
                    }
                }
            }
        }
        #endregion
    }
}
