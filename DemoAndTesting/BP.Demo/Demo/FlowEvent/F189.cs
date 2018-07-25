using System;
using System.Threading;
using System.Collections;
using BP.Web.Controls;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// ��������001
    /// ����������뵽 BP.*.dll ���ܱ��������������
    /// </summary>
    public class F189: BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ��д���̱��
        /// </summary>
        public override string FlowMark
        {
            get { return ",189,";   }
        }
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F189()
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

            // ��ǰ�Ľڵ�, �����ı������ this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int���͵�ID.
            string nodeName = this.HisNode.Name; // ��ǰ�ڵ�����.
            switch (nodeID)
            {
                case 18901:  //���ǵ�1���ڵ��ʱ��.
                    //  throw new Exception("���������̷�������,��ֹ��������.");
                    //this.JumpToEmps = "zhangsan,lisi";
                    //this.JumpToNode = new Node(102);
                    //this.WorkID;

                    return "SendWhen�¼��Ѿ�ִ�гɹ���";
                default:
                    break;
            }
            return null;
        }
        #endregion �����¼�.

        /// <summary>
        /// �����ִ�е��¼�
        /// </summary>
        /// <returns></returns>
        public override string SaveAfter()
        {
            switch (this.HisNode.NodeID)
            {
                case 18901:
                    //string sql = "UPDATE ND18901 SET  QingJiaTianShu=666666 WHERE OID="+this.WorkID;
                    //BP.DA.DBAccess.RunSQL(sql);
                    break;
                default:
                    break;
            }

            return base.SaveAfter();
        }
        
        /// <summary>
        /// ���ͳɹ��¼������ͳɹ�ʱ�������̵Ĵ���д������ϵͳ��.
        /// </summary>
        /// <returns>����ִ�н�����������null�Ͳ���ʾ��</returns>
        public override string SendSuccess()
        {
            try
            {
                // ��֯��Ҫ�ı���.
                Int64 workid = this.WorkID; // ����id.w
                string flowNo = this.HisNode.FK_Flow; // ���̱��.
                int currNodeID = this.SendReturnObjs.VarCurrNodeID; //��ǰ�ڵ�id
                int toNodeID = this.SendReturnObjs.VarToNodeID; // ����ڵ�id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // ����ڵ����ơ�
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // ������Աid, �����Ա���� ���ŷֿ� ,���� zhangsan,lisi��
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // ������Ա���ƣ������Ա���ö��ŷֿ�����:����,����.

                //ִ��������ϵͳд�����.
                /*
                 * ��������Ҫ��д���ҵ���߼�������������֯�ı���.
                 */

                if (this.HisNode.NodeID == 102)
                {
                    /*���ݲ�ͬ�Ľڵ㣬ִ�в�ͬ��ҵ���߼�*/
                }

                //����.
                return base.SendSuccess();
            }
            catch(Exception ex)
            {
                throw new Exception("������ϵͳд�����ʧ�ܣ���ϸ��Ϣ��"+ex.Message);
            }
        }
         
    }
}
