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
    /// Method ��ժҪ˵��
    /// </summary>
    public class RepariDB : Method
    {
        /// <summary>
        /// �����в����ķ���
        /// </summary>
        public RepariDB()
        {
            this.Title = "����������Ŀ������";
            this.Help = "��PRJ_FileDir������������¸���Ŀ¼����ʼ����";
        }
        /// <summary>
        /// ����ִ�б���
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// ��ǰ�Ĳ���Ա�Ƿ����ִ���������
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// ִ��
        /// </summary>
        /// <returns>����ִ�н��</returns>
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
            return "ִ�гɹ�...";
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
