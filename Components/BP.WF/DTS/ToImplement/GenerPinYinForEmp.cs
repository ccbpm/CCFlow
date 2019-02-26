using System;
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
    /// 修改人员编号 的摘要说明
    /// </summary>
    public class GenerPinYinForEmp : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerPinYinForEmp()
        {
            this.Title = "为人员生成拼音，放入到 Port_Emp.PinYin 字段里.";
            this.Help = "为了检索方便，为所有的人员生成拼音, 方便在会签，移交，接受人查询。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原用户名", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新用户名", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.IsAdmin == true)
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
            if (BP.DA.DBAccess.IsView("Port_Emp", SystemConfig.AppCenterDBType) == true)
                return "port_emp 是一个视图无法生成拼音.";

            if (BP.DA.DBAccess.IsExitsTableCol("Port_Emp", BP.GPM.EmpAttr.PinYin) == false)
                return "port_emp 不包含PinYin 这一列,无法生成拼音.";

            BP.GPM.Emps emps = new BP.GPM.Emps();
            emps.RetrieveAll();
            foreach (BP.GPM.Emp item in emps)
            {
                if (item.PinYin.Contains("/") == true)
                    continue;
                item.Update();
            }
            return "执行成功...";
        }
    }
}
