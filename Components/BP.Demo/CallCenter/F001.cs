using System;
using System.Threading;
using System.Collections;

using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.CallCenter
{
    /// <summary>
    /// ��������001
    /// ����������뵽 BP.*.dll ���ܱ��������������
    /// </summary>
    public class F001 : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ��д���̱��
        /// </summary>
        public override string FlowMark
        {
            get { return ",001,015,"; }
        }
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F001()
        {
        }
        #endregion ����.

        #region �����¼�.
        /// <summary>
        /// ��д����ǰ�¼�
        /// </summary>
        /// <returns></returns>
        public override string SendWhen()
        {
            //��صı���,

            //this.JumpToEmps = "zhangsan,lisi";
            //this.JumpToNodeID = 109;

            //this.WorkID;
            //this.HisNode.NodeID;

            //this.HisEn;
            //this.HisNode;
            //this.

            // ��ǰ�Ľڵ�, �����ı������ this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int���͵�ID.
            string nodeName = this.HisNode.Name; // ��ǰ�ڵ�����.
            switch (nodeID)
            {
                case 103:  //�ж��Ƿ�ȫ���Ϲ�,���ϸ���׳��쳣..
                    //string sql = "SELECT COUNT(*) FORM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND WorkSta!= 3 ";
                    //var num = DBAccess.RunSQLReturnValInt(sql);
                    //if (num != 0)
                    //    throw new Exception("err@����Ŀ�С�" + num + "��û�м��ϸ񣬻���û����ɣ������ܷ��͡�");
                    break;
                default:
                    break;
            }
            return null;
        }
        #endregion �����¼�.

        /// <summary>
        /// ���ͳɹ��¼������ͳɹ�ʱ�������̵Ĵ���д������ϵͳ��.
        /// </summary>
        /// <returns>����ִ�н�����������null�Ͳ���ʾ��</returns>
        public override string SendSuccess()
        {
            try
            {
                // ��֯��Ҫ�ı���.
                Int64 workid = this.WorkID; // ����id.
                string flowNo = this.HisNode.FK_Flow; // ���̱��.
                int currNodeID = this.SendReturnObjs.VarCurrNodeID; //��ǰ�ڵ�id
                int toNodeID = this.SendReturnObjs.VarToNodeID;     //����ڵ�id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // ����ڵ����ơ�
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // ������Աid, �����Ա���� ���ŷֿ� ,���� zhangsan,lisi��
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // ������Ա���ƣ������Ա���ö��ŷֿ�����:����,����.

                //ִ��������ϵͳд�����.
                /*
                 * ��������Ҫ��д���ҵ���߼�������������֯�ı���.
                 */

                if (this.HisNode.NodeID == 1503)
                {
                    /* ���ݲ�ͬ�Ľڵ㣬ִ�в�ͬ��ҵ���߼� */
                  //  string sql = "xxxxx  xxxx,xxxx ,";
                  //  DBAccess.RunSQL(sql);

                }

                //����.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("������ϵͳд�����ʧ�ܣ���ϸ��Ϣ��"+ex.Message);
            }
        }

    }
}
