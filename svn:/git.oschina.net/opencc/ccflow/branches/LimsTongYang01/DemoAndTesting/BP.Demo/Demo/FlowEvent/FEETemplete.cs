using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;
using BP.Port;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// xxxxxx �����¼�ʵ��
    /// </summary>
    public class FEETemplete : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// xxxxxx �����¼�ʵ��
        /// </summary>
        public FEETemplete()
        {
        }
        #endregion ����.

        #region ��д����.
        public override string FlowMark
        {
            get { return "Templete"; }
        }
        #endregion ��д����.

        #region ��д�����˶��¼�.
        /// <summary>
        /// ɾ����
        /// </summary>
        /// <returns></returns>
        public override string AfterFlowDel()
        {
            return null;
        }
        /// <summary>
        /// ɾ��ǰ
        /// </summary>
        /// <returns></returns>
        public override string BeforeFlowDel()
        {
            return null;
        }
        
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        public override string FlowOverAfter()
        {
            throw new Exception("@�Ѿ����õ��˽������¼���.");
            return null;
        }
        /// <summary>
        /// ����ǰ
        /// </summary>
        /// <returns></returns>
        public override string FlowOverBefore()
        {
            return null;
        }
        #endregion ��д�����˶��¼�

        #region �ڵ���¼�
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
        #endregion

        #region ��д�ڵ��˶��¼�.

        public override string SaveBefore()
        {
            return null;
        }
        public override string SaveAfter()
        {
            return null;
        }
        public override string SendWhen()
        {
            return null;
        }
        public override string SendSuccess()
        {
            return null;
        }
        public override string SendError()
        {
            return null;
        }
        public override string ReturnAfter()
        {
            return null;
        }
        public override string ReturnBefore()
        {
            return null;
        }
        public override string UndoneAfter()
        {
            return null;
        }
        public override string UndoneBefore()
        {
            return null;
        }
        #endregion ��д�ڵ��˶��¼�
    }
}
