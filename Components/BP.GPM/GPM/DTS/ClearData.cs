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
    public class ClearData : Method
    {
        /// <summary>
        /// �����в����ķ���
        /// </summary>
        public ClearData()
        {
            this.Title = "�������ά��������";
            this.Help = "ϵͳ���ϵͳ���˵����û��飬�û�Ȩ�ޡ�";
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
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_AppSort");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_App");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_Menu");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_Group");
            return "����ɹ�.";
        }
    }
}
