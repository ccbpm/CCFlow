using System;
using System.IO;
using System.Drawing;
using BP.Port;
using BP.En;

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
            this.Help = "此功能需要用户对 "+ BP.Difference.SystemConfig.PathOfDataUser + "Siganture/ 有读写权限，否则会执行失败。";
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
        /// 生成一个
        /// </summary>
        /// <param name="empID">人员ID</param>
        /// <param name="empName">人员名称</param>
        public static void GenerIt(string empID, string empName)
        {
            string path =  BP.Difference.SystemConfig.PathOfDataUser + "Siganture/T.jpg";
            string fontName = "宋体";

            string pathMe =  BP.Difference.SystemConfig.PathOfDataUser + "Siganture/" + empID + ".jpg";
            if (System.IO.File.Exists(pathMe))
                return;

            File.Copy(BP.Difference.SystemConfig.PathOfDataUser + "Siganture/Templete.jpg",
                path, true);

            System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            Font font = new Font(fontName, 15);
            Graphics g = Graphics.FromImage(img);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat(StringFormatFlags.DirectionVertical);//文本
            g.DrawString(empName, font, drawBrush, 3, 3);
            img.Save(pathMe);
            img.Dispose();
            g.Dispose();

            File.Copy(pathMe,
            BP.Difference.SystemConfig.PathOfDataUser + "Siganture/" + empName + ".jpg", true);
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
                foreach (Emp emp in emps)
                {
                    GenerIt(emp.No, emp.Name);
                }
                return "执行成功...";
            }
            catch(Exception ex)
            {
                return "执行失败，请确认对 " + BP.Difference.SystemConfig.PathOfDataUser + "Siganture/ 目录有访问权限？异常信息:"+ex.Message;
            }
        }
    }
}
