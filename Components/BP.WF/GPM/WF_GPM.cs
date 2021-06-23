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
using BP.GPM.Menu2020;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_GPM : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_GPM()
        {

        }

        /// <summary>
        /// 模块移动.
        /// </summary>
        /// <returns></returns>
        public string Module_Move()
        {
            string sortNo = this.GetRequestVal("RootNo");
            string[] EnNos = this.GetRequestVal("EnNos").Split(',');
            for (int i = 0; i < EnNos.Length; i++)
            {
                var enNo = EnNos[i];

                string sql = "UPDATE GPM_Module SET SystemNo ='" + sortNo + "',Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "模块顺序移动成功..";
        }
        public string System_Move()
        {
            string[] EnNos = this.GetRequestVal("EnNos").Split(',');
            for (int i = 0; i < EnNos.Length; i++)
            {
                var enNo = EnNos[i];

                string sql = "UPDATE GPM_System SET Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "模块顺序移动成功..";
        }
        /// <summary>
        /// 菜单移动
        /// </summary>
        /// <returns></returns>
        public string Menu_Move()
        {
            
            string sortNo = this.GetRequestVal("RootNo");
            string[] EnNos = this.GetRequestVal("EnNos").Split(',');
            for (int i = 0; i < EnNos.Length; i++)
            {
                var enNo = EnNos[i];

                string sql = "UPDATE GPM_Menu SET ModuleNo ='" + sortNo + "',Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "菜单顺序移动成功..";
        }


        public string Home_Init()
        {
            string str = SystemConfig.PathOfData+ "\\XML\\BarTemp.xml";
            DataSet ds = new DataSet();
            ds.ReadXml(str);

            DataTable dt = ds.Tables[0];
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 创建系统
        /// </summary>
        /// <returns></returns>
        public string NewSystem_Save()
        {
            string rootNo = "0";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                rootNo = BP.Web.WebUser.OrgNo;

            //创建根目录。
            BP.Sys.FrmTree frmTree = new FrmTree();
            if (rootNo.Equals("0")==true)
            frmTree.Retrieve(FrmTreeAttr.ParentNo, rootNo);
            else
                frmTree.Retrieve(FrmTreeAttr.No, rootNo);
            //类别.
            BP.WF.Template.FlowSort fs = new BP.WF.Template.FlowSort();
            if (rootNo.Equals("0") == true)
                fs.Retrieve(FrmTreeAttr.ParentNo, rootNo);
            else
                fs.Retrieve(FrmTreeAttr.No, rootNo);

            //系统名称
            string name = this.GetRequestVal("TB_Name");

            //创建系统.
            MySystem system = new MySystem();
            system.Name = name;
            system.Icon = "icon-briefcase";
            system.OrgNo = WebUser.OrgNo;
            system.Insert();

            FrmTree frmTee = frmTree.DoCreateSubNode();
            frmTee.Name = name;
            // en.ICON = system.Icon;
            frmTee.OrgNo = WebUser.OrgNo;
            frmTree.Idx = 100;
            frmTee.Update();
            DBAccess.RunSQL("UPDATE Sys_FormTree SET No='" + system.No + "' WHERE No='" + frmTee.No + "'");

            FlowSort myen = fs.DoCreateSubNode() as FlowSort;
            myen.Name = name;
            myen.OrgNo = WebUser.OrgNo;
            myen.Idx = 100;
            myen.Update();
            DBAccess.RunSQL("UPDATE WF_FlowSort SET No='" + system.No + "' WHERE No='" + myen.No + "'");

            //创建模块.
            string modelNo = null;
            for (int i = 0; i < 5; i++)
            {
                name = this.GetRequestVal("TB_" + i);
                if (DataType.IsNullOrEmpty(name) == true)
                    continue;

                Module en = new Module();
                en.SystemNo = system.No;
                en.Name = name;
                en.Icon = "icon-folder";
                en.Insert();
                if (modelNo == null)
                    modelNo = en.No;
            }

            return system.No;
        }
    }
}
