using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;
using BP.Port;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// �������� - ��ʼ�ڵ�.
    /// </summary>
    public class ND101 : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public ND101()
        {
        }
        #endregion ����.

        #region ��д����.
        public override string FlowMark
        {
            get { return "QingJia"; }
        }
        #endregion ��д����.

        #region ��д�ڵ���¼�.
        /// <summary>
        /// ������ǰ
        /// </summary>
        public override string FrmLoadAfter()
        {
            return null;
        }
        /// <summary>
        /// �������
        /// </summary>
        public override string FrmLoadBefore()
        {
            return null;
        }
        /// <summary>
        /// �������
        /// </summary>
        public override string SaveAfter()
        {
            return null;
        }
        /// <summary>
        /// ������ǰ
        /// </summary>
        public override string SaveBefore()
        {
            if (this.HisNode.NodeID == 101)
            {
                //���ºϼ�Сд.
                string sql = "UPDATE ND101 SET HeJi=(SELECT SUM(XiaoJi) FROM ND101Dtl1 WHERE RefPK=" + this.OID + ") WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                //�Ѻϼ�ת���ɴ�д.
                float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT HeJi FROM ND101 WHERE OID=" + this.OID, 0);

                sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "' WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                return null;
            }
            return null;
        }
        #endregion ��д�ڵ���¼�

        #region ��д�ڵ��˶��¼�.
        /// <summary>
        /// ����ǰ:���ڼ��ҵ���߼��Ƿ����ִ�з��ͣ�����ִ�з��;��׳��쳣.
        /// </summary>
        public override string SendWhen()
        {
            if (this.HisNode.NodeID == 101)
            {
                //���ºϼ�Сд.
                string sql = "UPDATE ND101 SET HeJi=(SELECT SUM(XiaoJi) FROM ND101Dtl1 WHERE RefPK=" + this.OID + ") WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                //�Ѻϼ�ת���ɴ�д.
                float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT HeJi FROM ND101 WHERE OID=" + this.OID, 0);
                if (hj == 0)
                    throw new Exception("@����Ҫ���������ϸ��Ŀ.");

                sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "' WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                return "�ϼ��Ѿ��ڷ���ǰ�¼����.";
            }

            return null;
        }
        /// <summary>
        /// ���ͳɹ���
        /// </summary>
        public override string SendSuccess()
        {
            return null;
        }
        /// <summary>
        /// ����ʧ�ܺ�
        /// </summary>
        public override string SendError()
        {
            return null;
        }
        /// <summary>
        /// �˻�ǰ
        /// </summary>
        public override string ReturnBefore()
        {
            return null;
        }
        /// <summary>
        /// �˻غ�
        /// </summary>
        public override string ReturnAfter()
        {
            return null;
        }
        #endregion ��д�¼������ҵ���߼�.
    }
}
