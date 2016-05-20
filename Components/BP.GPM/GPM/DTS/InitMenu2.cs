using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.GPM.DTS
{
    /// <summary>
    /// Method ��ժҪ˵��
    /// </summary>
    public class InitMenu : Method
    {
        /// <summary>
        /// �����в����ķ���
        /// </summary>
        public InitMenu()
        {
            this.Title = "Ϊÿ������Ա��ʼ������ϵͳ�˵�";
            this.Help = "�˹�����Ҫ�ڲ˵�Ȩ��,���ܣ����ܵ㣬Ȩ����,ϵͳ�����仯��ִ��.";
            this.Icon = "<img src='/Images/Btn/Delete.gif'  border=0 />";
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
            //ɾ������.
            EmpMenus mymes = new EmpMenus();
            mymes.GetNewEntity.CheckPhysicsTable();
            mymes.ClearTable();

            EmpApps empApps = new EmpApps();
            empApps.GetNewEntity.CheckPhysicsTable();
            empApps.ClearTable();

            //��ѯ�����˵�.
            Menus menus = new Menus();
            menus.RetrieveAllFromDBSource();

            //��ѯ�������е�Ӧ��ϵͳ.
            Apps apps = new Apps();
            apps.RetrieveAllFromDBSource();

            //��ѯ������Ա.
            Emps emps = new Emps();
            emps.RetrieveAllFromDBSource();

            foreach (Emp emp in emps)
            {
                // ɾ������Ա�Ĳ˵�Ȩ��.
                string sql = "";
                BP.DA.DBAccess.RunSQL("DELETE GPM_EmpMenu WHERE FK_Emp='" + emp.No + "'");


                string menuIDs = "";

                #region ���Ƚ����һ����Ա�ĸ��Ի�����.
                //����Ա�˵�������Ϣ���в�ѯ.
                UserMenus ums = new UserMenus();
                ums.Retrieve(UserMenuAttr.FK_Emp, emp.No);
                foreach (UserMenu um in ums)
                {
                    menuIDs += um.FK_Menu + ",";
                }
                #endregion ���Ƚ����һ����Ա�ĸ��Ի�����.
                
                // ������û��ж��ٸ�Ȩ����.
                string groupIDs = ",";
                 sql = "SELECT FK_Group FROM GPM_GroupEmp WHERE FK_Emp='" + emp.No + "'";
                sql += " UNION ";
                sql += "SELECT FK_Group FROM GPM_GroupStation WHERE FK_Station in (SELECT FK_Station from Port_DeptEmpStation where FK_Emp='" + emp.No + "') ";

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (groupIDs.Contains("," + dr[0].ToString() + ","))
                        continue;
                    groupIDs += dr[0].ToString() + ",";
                }

            }

            return "���еĳ�Ա������ʼ���ɹ�.";
        }
    }
}
