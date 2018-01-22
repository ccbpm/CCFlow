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
    public class RelFlowTemplateToSubInc : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public RelFlowTemplateToSubInc()
        {
            this.Title = "发布流程模版到子公司";
            this.Help = "集团公司的流程模版发布到子公司里面去.";
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
            //找到根目录.
            string sql = "SELECT No FROM WF_FlowSort where ParentNo='0'";
            string rootNo = DBAccess.RunSQLReturnString(sql, null);
            if (rootNo == null)
                return "没有找到根目录节点"+sql;

            //求出分公司集合(组织结构集合)
            sql = "SELECT No,Name FROM Port_Dept where xxx=000";
            DataTable dtInc = DBAccess.RunSQLReturnTable(sql);

            //取得所有根目录下的流程模版.
            Flows fls = new Flows(rootNo);
            
            string infos = "";
            foreach (Flow fl in fls)
            {
                //不是模版.
                if (fl.FlowMark.Equals("") ==true)
                    continue;

                infos += "@开始发布流程模版:" + fl.Name +" 标记:"+fl.FlowMark;

                //把该模版发布到子公司里面去.
                foreach (DataRow dr in dtInc.Rows)
                {
                    string incNo = dr["No"].ToString();
                    string incName = dr["Name"].ToString();

                    //检查该公司是否创建了树节点, 如果没有就插入一个.
                    BP.WF.Template.FlowSort fs = new Template.FlowSort();
                    fs.No = incNo;
                    if (fs.RetrieveFromDBSources() == 0)
                    {
                        fs.Name = incName;
                        fs.OrgNo = incNo;
                        fs.ParentNo = rootNo;
                        fs.Insert();
                    }

                    //开始把该模版发布到该公司下.
                    Flow flInc = new Flow();
                    int num= flInc.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, incNo,
                        BP.WF.Template.FlowAttr.FlowMark, fl.FlowMark);
                    if (num == 1)
                        continue; //模版已经存在.

                    string filePath = fl.GenerFlowXmlTemplete();

                    //作为一个新的流程编号，导入该流程树的子节点下.
                    BP.WF.Flow.DoLoadFlowTemplate(incNo,filePath, ImpFlowTempleteModel.AsNewFlow);

                    infos += "@模版:" + fl.Name + " 标记:" + fl.FlowMark+" 已经发布到子公司:"+incName;
                }
            }

            return infos;
        }
    }
}
