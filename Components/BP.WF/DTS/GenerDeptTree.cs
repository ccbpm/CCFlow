using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerDeptTree : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerDeptTree()
        {
            this.Title = "为部门Port_Dept表生成TreeNo字段,跟节点为00。";
            this.Help = "该字段仅仅为了用于LIKE查询，不能作为关联主键，因为该字段是变化的，随着部门的增加而变化.";
            this.Help += "执行此功能要求. 1. Port_Dept, 必须有 TreeNo 字段。 2. Port_Dept 必须有DeptTreeNo 字段. 3. Port_DeptEmp 必须有 DeptTreeNo 字段. 4. Port_DeptEmpStation 必须有 DeptTreeNo 字段.";
            //  this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
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
            if (DBAccess.IsExitsTableCol("Port_Dept", "TreeNo") == false)
                return "err@ Port_Dept 没有找到 TreeNo 的列.";

            BP.GPM.Dept dept = new GPM.Dept();
            int i = dept.Retrieve(DeptAttr.ParentNo, "0");
            if (i == 0)
                return "err@没有找到ParentNo=0的根节点.";

            //更新跟节点的TreeNo. 
            string sql = "UPDATE Port_Dept SET TreeNo='01' WHERE No='" + dept.No + "'";
            DBAccess.RunSQL(sql);

            BP.Port.Depts depts = new Depts();
            depts.Retrieve(BP.Port.DeptAttr.ParentNo, dept.No);

            int idx = 0;
            foreach (BP.Port.Dept item in depts)
            {
                idx++;
                string subNo = idx.ToString().PadLeft(2, '0');
                sql = "UPDATE Port_Dept SET TreeNo='01" + subNo + "' WHERE No='" + item.No + "'";
                DBAccess.RunSQL(sql);


                sql = "UPDATE Port_DeptEmp SET DeptTreeNo='01" + subNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);
                sql = "UPDATE Port_DeptEmpStation SET DeptTreeNo='01" + subNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);
            }

            return "执行成功.";
        }

        public void SetDeptTreeNo(Dept dept, string pTreeNo)
        {
            BP.Port.Depts depts = new Depts();
            depts.Retrieve(BP.Port.DeptAttr.ParentNo, dept.No);

            int idx = 0;
            foreach (BP.Port.Dept item in depts)
            {
                idx++;
                string subNo = idx.ToString().PadLeft(2, '0');
                string sql = "UPDATE Port_Dept SET TreeNo='" + pTreeNo + "" + subNo + "' WHERE No='" + item.No + "'";
                DBAccess.RunSQL(sql);

                //更新其他的表字段.
                sql = "UPDATE Port_DeptEmp SET DeptTreeNo='" + pTreeNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);
                sql = "UPDATE Port_DeptEmpStation SET DeptTreeNo='" + pTreeNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);

                //递归调用.
                SetDeptTreeNo(item, pTreeNo + subNo);
            }
        }
    }
}
