using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.XML;
using BP.Difference;
using System.Collections;

namespace BP.Cloud.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class Admin : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Admin()
        {
        }

        #region 执行父类的重写方法.
        #endregion 执行父类的重写方法.

        #region 界面 .
        public string Register_Init()
        {
            return "注册页面";
        }
        #endregion xxx 界面方法.

        public string Organization_CreateOrg()
        {
            string orgNo = this.GetRequestVal("OrgNo");
            string orgName = this.GetRequestVal("OrgName");

            return Dev2Interface.Port_CreateOrg(orgNo, orgName);
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public string Login_AdminOnlySaas()
        {
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.SAAS)
                return "err@必须是saas模式才能使用此接口登陆.";

            try
            {
                string userNo = this.GetRequestVal("TB_No");
                if (userNo == null)
                    userNo = this.GetRequestVal("TB_UserNo");
                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");

                //从数据库里查询.
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@丢失了admin用户.";

                if (emp.CheckPass(pass) == false)
                    return "err@用户名密码错误.";
                BP.WF.Dev2Interface.Port_Login("admin", "100");


                string token = BP.WF.Dev2Interface.Port_GenerToken();
                WebUser.Token = token;
                Hashtable ht = new Hashtable();
                ht.Add("Token", token);
                return BP.Tools.Json.ToJson(ht);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }



        /// <summary>
        /// 登录的时候判断.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetRequestVal("TB_Adminer").Trim();
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            string pass = this.GetRequestVal("TB_PassWord2").Trim();
            if (emp.CheckPass(pass) == false)
                return "err@用户名或密码错误.";

            //让其登录.

            BP.Web.GuestUser.Exit();
            BP.WF.Dev2Interface.Port_Login(emp.No);
            string token = BP.WF.Dev2Interface.Port_GenerToken("PC");

            //判断当前管理员有多少个企业.
            Depts depts = new Depts();
            depts.Retrieve("Adminer", emp.No);

            //如果部门为0，就让其注册.
            if (depts.Count == 0)
                return "url@/RegisterOrg.html";

            //如果只有一个部门.
            if (depts.Count == 1)
            {
                var dept = depts[0] as Dept;
                emp.FK_Dept = dept.No;
                emp.Update();
                return "url@/Admin/Portal/Default.htm?Token=" + token + "&UserNo=" + emp.No;
            }

            //转入到选择一个企业的页面.
            return "url@/Admin/Portal/SelectOneInc.htm?Token=" + token + "&UserNo=" + emp.No;
        }

        /// <summary>
        /// 查询可以登录的企业.
        /// </summary>
        /// <returns></returns>
        public string SelectOneInc_Init()
        {
            Depts depts = new Depts();
            depts.Retrieve(DeptAttr.Adminer, WebUser.No);
            return depts.ToJson();
        }

        public string SelectOneInc_SelectIt()
        {
            string no = this.GetRequestVal("No");
            Emp emp = new Emp(WebUser.No);
            emp.FK_Dept = no;
            emp.Update();
            return "url@Admin/Portal/Default.htm?Token=" + BP.Web.WebUser.Token + "&UserNo=" + emp.No;
        }

        /// <summary>
        ///  要返回的数据.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            return "";
        }

        /// <summary>
        /// 流程树
        /// </summary>
        /// <returns></returns>
        public string Default_FlowsTree()
        {
            //组织数据源.
            string sql = "SELECT * FROM (SELECT 'F'+No as NO,'F'+ParentNo PARENTNO, NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE, -1 DTYPE FROM WF_FlowSort WHERE OrgNo='" + WebUser.FK_Dept +
                          "' union  SELECT NO, 'F'+FK_FlowSort as PARENTNO,(NO + '.' + NAME) as NAME,IDX,0 ISPARENT,'FLOW' TTYPE, 0 as DTYPE FROM WF_Flow) A  ORDER BY DTYPE, IDX ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
            if (dt.Rows.Count == 0)
            {
                FlowSort fs = new FlowSort();
                fs.No = "99";
                fs.ParentNo = "0";
                fs.Name = "流程树";
                fs.Insert();

                dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
            }
            else
            {
                DataRow[] drs = dt.Select("NAME='流程树'");
                if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
                    drs[0]["PARENTNO"] = "F0";
            }

            String str = BP.Tools.Json.ToJson(dt);
            return str;
        }

        /// <summary>
        /// 查询表单树
        /// </summary>
        /// <returns></returns>
        public string Default_FrmTree()
        {
            //组织数据源.
            string sqls = "";
            /* sqls = "SELECT No,ParentNo,Name, Idx FROM Sys_FormTree WHERE OrgNo=" + WebUser.FK_Dept + " ORDER BY Idx ASC ";
             sqls += "SELECT No, FK_FormTree as ParentNo,Name,Idx,0 IsParent  FROM Sys_MapData    ";*/
            sqls = "SELECT No,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree WHERE OrgNo='" + WebUser.FK_Dept + "' ORDER BY Idx ASC ; ";
            sqls += "SELECT No, FK_FormTree as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData  WHERE AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree WHERE OrgNo='" + WebUser.FK_Dept + "') ORDER BY Idx ASC";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            DataTable dtSort = ds.Tables[0]; //类别表.
            DataTable dtForm = ds.Tables[1].Clone(); //表单表,这个是最终返回的数据.
            //增加顶级目录.
            DataRow[] rowsOfSort = dtSort.Select("ParentNo='0'");
            DataRow drFormRoot = dtForm.NewRow();
            drFormRoot[0] = rowsOfSort[0]["No"];
            drFormRoot[1] = "0";
            drFormRoot[2] = rowsOfSort[0]["Name"];
            drFormRoot[3] = rowsOfSort[0]["Idx"];
            drFormRoot[4] = rowsOfSort[0]["IsParent"];
            drFormRoot[5] = rowsOfSort[0]["TType"];
            dtForm.Rows.Add(drFormRoot); //增加顶级类别..

            //把类别数据组装到form数据里.
            foreach (DataRow dr in dtSort.Rows)
            {
                DataRow drForm = dtForm.NewRow();
                drForm[0] = dr["No"];
                drForm[1] = dr["ParentNo"];
                drForm[2] = dr["Name"];
                drForm[3] = dr["Idx"];
                drForm[4] = dr["IsParent"];
                drForm[5] = dr["TType"];
                dtForm.Rows.Add(drForm); //类别.
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                dtForm.Rows.Add(row.ItemArray);
            }

            String str = BP.Tools.Json.ToJson(dtForm);
            return str;
        }
        /// <summary>
        /// 获取设计器 - 系统维护菜单数据
        /// 系统维护管理员菜单 需要翻译
        /// </summary>
        /// <returns></returns>
        public string Default_AdminMenu()
        {
            //查询全部.
            AdminMenus groups = new AdminMenus();
            groups.RetrieveAll();

            return BP.Tools.Json.ToJson(groups.ToDataTable());
        }
        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            return "err@没有判断的标记:" + this.DoType;
        }
        #endregion 执行父类的重写方法.
    }
}
