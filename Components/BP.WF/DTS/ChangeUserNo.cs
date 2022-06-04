using BP.En;
using BP.Sys;

namespace BP.WF.DTS
{
    /// <summary>
    /// 修改人员编号 的摘要说明
    /// </summary>
    public class ChangeUserNo : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ChangeUserNo()
        {
            this.Title = "修改人员编号（原来一个操作中编号叫A,现在修改成B）";
            this.Help = "请慎重执行，执行前请先备份数据库，系统会把生成的SQL放在日志里，打开日志文件(" + BP.Difference.SystemConfig.PathOfDataUser + "\\Log)，然后找到这些sql.";
            this.GroupName = "系统维护";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            this.Warning = "您确定要执行吗？";
            HisAttrs.AddTBString("P1", null, "原用户名", true, false, 0, 10, 10);
            HisAttrs.AddTBString("P2", null, "新用户名", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No.Equals("admin")==true)
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
            string oldNo = this.GetValStrByKey("P1");
            string newNo = this.GetValStrByKey("P2");

            string sqls = "";

            sqls += "UPDATE Port_Emp Set No='" + newNo + "' WHERE No='" + oldNo + "'";
            sqls += "\t\n UPDATE Port_DeptEmpStation Set FK_Emp='" + newNo + "' WHERE FK_Emp='" + oldNo + "'";

            MapDatas mds = new MapDatas();
            mds.RetrieveAll();

            foreach (MapData md in mds)
            {
                MapAttrs mattrs = new MapAttrs(md.No);
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.UIIsEnable == false && attr.DefValReal == "@WebUser.No")
                    {
                        sqls += "\t\n UPDATE " + md.PTable + " SET ";
                    }
                    continue;

                }
                sqls += "UPDATE";

            }

            return "执行成功..." + sqls;
        }
    }
}
