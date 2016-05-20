using System;
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
            this.Icon = "<img src='/WF/Img/Btn/Delete.gif'  border=0 />";
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
            Group g = new Group();
            g.CheckPhysicsTable();

            GroupEmp ge = new GroupEmp();
            ge.CheckPhysicsTable();

            GroupStation gs = new GroupStation();
            gs.CheckPhysicsTable();

            UserMenu um = new UserMenu();
            um.CheckPhysicsTable();

            ////ɾ������.
            //EmpMenus mymes = new EmpMenus();
            //mymes.GetNewEntity.CheckPhysicsTable();
            //mymes.ClearTable();

            //EmpApps empApps = new EmpApps();
            //empApps.GetNewEntity.CheckPhysicsTable();
            //empApps.ClearTable();

            ////��ѯ�����˵�.
            //Menus menus = new Menus();
            //menus.RetrieveAllFromDBSource();

            ////��ѯ�������е�Ӧ��ϵͳ.
            //Apps apps = new Apps();
            //apps.RetrieveAllFromDBSource();

            ////��ѯ������Ա.
            //Emps emps = new Emps();
            //emps.RetrieveAllFromDBSource();
           
            return "���еĳ�Ա������ʼ���ɹ�.";
        }
    }
}
