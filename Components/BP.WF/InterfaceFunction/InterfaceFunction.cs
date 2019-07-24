using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BP.WF
{
    /// <summary>
    /// 接口功能方法.
    /// </summary>
    public class InterfaceFunction
    {
        /// <summary>
        /// 合成pdf文件
        /// </summary>
        /// <param name="fileList">文件名list</param>
        /// <param name="outMergeFile">输出路径</param>
        public static void MakeForm2Html_mergePDFFiles(List<string> fileList, string outMergeFile)
        {
            PdfReader reader;
            //iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(1403, 991);
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Count; i++)
            {
                reader = new PdfReader(fileList[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }
    }
}
