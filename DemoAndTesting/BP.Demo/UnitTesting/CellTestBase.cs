using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.UnitTesting
{
    public enum EditState
    {
        /// <summary>
        /// �Ѿ����
        /// </summary>
        Passed,
        /// <summary>
        /// �༭��
        /// </summary>
        Editing,
        /// <summary>
        /// δ���
        /// </summary>
        UnOK
    }
	/// <summary>
	/// ���Ի���
	/// </summary>
    abstract public class TestBase
    {
        public EditState EditState = EditState.Editing;
        /// <summary>
        /// ִ�в�����Ϣ
        /// </summary>
        public int TestStep = 0;
        public string Note = "";
        /// <summary>
        /// ���Ӳ�������.
        /// </summary>
        /// <param name="note">�������ݵ���ϸ����.</param>
        public void AddNote(string note)
        {
            TestStep++;
            if (Note == "")
            {
                Note += "\t\n ����:" + TestStep + "�����";
                Note += "\t\n" + note;

            }
            else
            {
                Note += "\t\n����ͨ��.";
                Note += "\t\n ����:" + TestStep + "�����";
                Note += "\t\n" + note;
            }

            
        }
        public string sql = "";
        public DataTable dt = null;
        /// <summary>
        /// ��������д
        /// </summary>
        public virtual void Do()
        {
        }

        #region ��������.
        /// <summary>
        /// ����
        /// </summary>
        public string Title = "δ�����ĵ�Ԫ����";
        public string DescIt = "����";
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string ErrInfo = "";
        #endregion
        /// <summary>
        /// ���Ի���
        /// </summary>
        public TestBase() { }
    }

}
