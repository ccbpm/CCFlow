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
    /// </summary>
    public class F025 : BP.Sys.EventBase
    {
        #region ����.
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F025()
        {
            this.Title = "��������";
        }
        #endregion ����.

        /// <summary>
        /// ִ���¼�
        /// 1���������������׳��쳣��Ϣ��ǰ̨����ͻ���ʾ���󲢲�����ִ�С�
        /// 2��ִ�гɹ�����ִ�еĽ������SucessInfo�������������Ҫ��ʾ�͸�ֵΪ�ջ���Ϊnull��
        /// 3�����еĲ��������Դ�  this.SysPara.GetValByKey �л�ȡ��
        /// </summary>
        public override void Do()
        {
            switch (this.FK_Node)
            {
                case 2501: //��д�������뵥�ڵ�.
                    switch (this.EventType)
                    {
                        case EventListOfNode.FrmLoadBefore: //��������¼�.
                            this.ND2501_FrmLoadBefore();
                            break;
                        case EventListOfNode.SendWhen: //����ǰ.
                            this.ND2501_SendWhen();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// װ��ǰ�¼�
        /// ������������Ϊ:ND+�ڵ���_�¼���.
        /// </summary>
        public void ND2501_FrmLoadBefore()
        {
            // �����. 
            int dtlOID = this.GetValInt("ItemNo");
            int Totalmoney = this.GetValInt("Totalmoney");

            string sql = "select sum(BXJE) as Num from ND25Rpt WHERE ItemNo='" + dtlOID + "' AND WFState!=0";
            decimal bxjeSum = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 1);

            decimal ye = Totalmoney - bxjeSum;

            /*���������Ҫ����������.*/

            //���õ�ʵ���У����������ʾ������.
            this.HisEn.SetValByKey("YuE", ye);

            //���õ�����Դ��,����ֱ�Ӹ��µ�����Դ�����YuE�ֶ��ǿ��Ա༭����������û�б�Ҫ.
            sql = "UPDATE ND2501 SET YuE=" + ye + " WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// ����ǰ
        /// ������������Ϊ:ND+�ڵ���_�¼���.
        /// </summary>
        public void ND2501_SendWhen()
        {
            throw new Exception("@����������Ѿ�������Ԥ�㡣");
        }
    }
}
