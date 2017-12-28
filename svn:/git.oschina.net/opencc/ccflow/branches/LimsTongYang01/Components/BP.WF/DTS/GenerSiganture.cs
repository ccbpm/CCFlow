using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerSiganture : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerSiganture()
        {
            this.Title = "为没有设置数字签名的用户设置默认的数字签名";
            this.Help = "此功能需要用户对 "+ BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\ 有读写权限，否则会执行失败。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            try
            {
                BP.Port.Emps emps = new Emps();
                emps.RetrieveAllFromDBSource();
                string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\T.JPG";
                string fontName = "宋体";
                string empOKs = "";
                string empErrs = "";
                foreach (Emp emp in emps)
                {
                    string pathMe = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.No + ".JPG";
                    if (System.IO.File.Exists(pathMe))
                        continue;

                    File.Copy(BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\Templete.JPG",
                        path, true);

                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                    Font font = new Font(fontName, 15);
                    Graphics g = Graphics.FromImage(img);
                    System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat(StringFormatFlags.DirectionVertical);//文本
                    g.DrawString(emp.Name, font, drawBrush, 3, 3);
                    img.Save(pathMe);
                    img.Dispose();
                    g.Dispose();

                    File.Copy(pathMe,
                    BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + emp.Name + ".JPG", true);
                }
                return "执行成功...";
            }
            catch(Exception ex)
            {
                return "执行失败，请确认对 " + BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\ 目录有访问权限？异常信息:"+ex.Message;
            }
        }
    }
}
