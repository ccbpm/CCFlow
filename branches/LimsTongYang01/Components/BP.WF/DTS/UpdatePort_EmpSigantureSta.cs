using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class UpdatePort_EmpSigantureSta : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public UpdatePort_EmpSigantureSta()
        {
            this.Title = "同步用户签名状态(如果是在/DataUser/Siganture 有图片签名，就设置当前人员为图片签名状态.)";
            this.Help = "执行更新Port_Emp数据表的SignType字段.";
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
                if (BP.Web.WebUser.No == "admin")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string path = BP.Sys.SystemConfig.PathOfDataUser + "Siganture";
            string[] files = System.IO.Directory.GetFiles(path);

            //清空设置为图片签名的记录.
            string sql = "UPDATE Port_Emp SET SignType=0 WHERE SignType=1";
            DBAccess.RunSQL(sql);

            //遍历文件名.
            foreach (string file in files)
            {
                string userName = file.Substring(file.LastIndexOf('\\') + 1);
                userName = userName.Substring(0, userName.LastIndexOf('.'));

                sql = "UPDATE Port_Emp SET SignType=1 WHERE No='" + userName + "'";
                DBAccess.RunSQL(sql);
            }

            return "执行完成，[" + files.Length + "]笔数据.";
        }
    }
}
