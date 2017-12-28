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
    public class F104:BP.Sys.EventBase
    {
        #region ����.
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F104()
        {
            this.Title = "�������";
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
                case 10401: //��д�������뵥�ڵ�.
                case 10402: //��д�������뵥�ڵ�.
                case 10403: //��д�������뵥�ڵ�.
                    switch (this.EventType)
                    {
                        case EventListOfNode.FrmLoadBefore: //��������¼�.
                            break;
                        case EventListOfNode.SaveBefore: //������ǰ�¼�.
                            SendWhen10401();
                            break;
                        case EventListOfNode.SendWhen: //����ǰ.
                            SendWhen10401();
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
        /// ִ�з���ǰ�¼�
        /// </summary>
        public void SendWhen10401()
        {
            DateTime dtFrom = this.HisEn.GetValDateTime("QingJiaShiJianCong");
            DateTime dtTo = this.HisEn.GetValDateTime("Dao");

            if (dtFrom > dtTo)
                throw new Exception("���ʱ�䵽����С�����ʱ���.");

            //����������.
            TimeSpan ts = dtTo - dtFrom;
            float span = ts.Days;
            this.HisEn.SetValByKey("QingJiaTianShu", span);

            //���õ�����Դ��,����ֱ�Ӹ��µ�����Դ�����YuE�ֶ��ǿ��Ա༭����������û�б�Ҫ.
            string sql = "UPDATE ND10401 SET QingJiaTianShu=" + span + " WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}
