using System;
using System.IO;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.PRJ
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class RepariDB : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public RepariDB()
        {
            this.Title = "重新生成项目资料树";
            this.Help = "把PRJ_FileDir数据清除，重新根据目录来初始化。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
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
            //Dirs dirs = new Dirs();
            //dirs.ClearTable();

            //string path = @"D:\ccflow\VisualFlow\Data\PrjData\Templete";
            //string[] strs = Directory.GetDirectories(path);
            //foreach (string str in strs)
            //{
            //    Dir dir = new Dir();
            //    dir.No = str.Substring(0, 2);
            //    dir.Name = str.Substring(3);
            //    dir.DirPath = str;
            //    dir.Insert();
            //}
            return "执行成功...";
        }
        private void GetFolder(string pPath)
        {
            //string[] str_Directorys;
            //str_Directorys = Directory.GetDirectories(pPath);
            //foreach (string pstr in str_Directorys)
            //{
            //    Dir dir = new Dir();
            //    //dir.No = str.Substring(0, 2);
            //}
        }

    }
}
