using System;
using System.Data;
using BP.DA;
using BP.WF;

namespace BP.Demo
{
    /// <summary>
    /// ��������001
    /// 0. ����������뵽 BP.*.dll ���ܱ������������.
    /// 1. һ�������ֺ����������.
    /// </summary>
    public class F001 : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ��д���̱��,����������.
        /// </summary>
        public override string FlowMark
        {
            get { return ",001,002,003,004,006,005,"; }
        }
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F001()
        {
        }
        public override string FrmLoadAfter()
        {

            return base.FrmLoadAfter();
        }
        #endregion ����.

        #region �����¼�.
        /// <summary>
        /// ��д����ǰ�¼�
        /// </summary>
        /// <returns></returns>
        public override string SendWhen()
        {
            //if (this.HisNode.NodeID == 107)
            //{
            //    //�ж��Ƿ���bomcode �����������ݣ����û�У��Ͳ������·�.
            //    string sql = "SELECT COUNT(*) AS SNU FROM ND101Dtl1 WHERE RefPK="+this.OID;
            //    if (DBAccess.RunSQLReturnValInt(sql) == 0)
            //        throw new Exception("@������û�е����������BOM��Ϣ��");
            //}
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
                if (1 == 1)
                { 
                }

                //// ��֯��Ҫ�ı���.
                //Int64 workid = this.WorkID; // ����id.w
                //string flowNo = this.HisNode.FK_Flow; // ���̱��.
                //int currNodeID = this.SendReturnObjs.VarCurrNodeID; //��ǰ�ڵ�id
                //int toNodeID = this.SendReturnObjs.VarToNodeID;     //����ڵ�id.
                //string toNodeName = this.SendReturnObjs.VarToNodeName; // ����ڵ����ơ�
                //string acceptersID = this.SendReturnObjs.VarAcceptersID; // ������Աid, �����Ա���� ���ŷֿ� ,���� zhangsan,lisi��
                //string acceptersName = this.SendReturnObjs.VarAcceptersName; // ������Ա���ƣ������Ա���ö��ŷֿ�����:����,����.

                //DBAccess.RunSQL("UPDATE ND1Rpt SET TodoEmps='" + this.HisGenerWorkFlow.TodoEmps + "' WHERE OID=" + this.WorkID);

                //// ʵʱ������Ŀ������.
                ////ND2RptExt ext = new ND2RptExt(this.HisGenerWorkFlow.PWorkID);
                ////ext.ResetNum();
                //DBAccess.RunSQL("UPDATE nd1rpt set  CCRQ=QiXianXianDing WHERE CCRQ='' OR  CCRQ IS NULL ");

                //����.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("������ϵͳд�����ʧ�ܣ���ϸ��Ϣ��"+ex.Message);
            }
        }

        /// <summary>
        /// ���̽���֮��
        /// </summary>
        /// <returns></returns>
        public override string FlowOverAfter()
        {

            ////�������״̬.
            //ND1Rpt rpt = new ND1Rpt(this.WorkID);
            //if (rpt.QiXianXianDingDT >= DateTime.Now)
            //{
            //    rpt.SetValByKey("SXSta", 3);
            //}
            //else
            //{
            //    rpt.SetValByKey("SXSta", 4);
            //}
            //rpt.Update();

            //GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            ////���ÿ���ö�����ÿ�������̣��Ƿ���ɣ������ɣ����ö������̣��˶��������װ�ڵ㡣
            //string sql = "SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE PWorkID=" + gwf.PWorkID + " AND WFState!=3 ";
            //int num = DBAccess.RunSQLReturnValInt(sql);
            //if (num > 0)
            //    return ""; //˵���ö���δ��ɵĵ�����.

            //// �����������װ��ļල��Ա.
            //sql = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + gwf.PWorkID + " AND FK_Node=202 AND IsPass=0 ";
            //string worker = DBAccess.RunSQLReturnStringIsNull(sql, null);
            //if (worker == null)
            //    throw new Exception("err@��Ӧ�ò�ѯ��������Ա��");

            //string currEmpNo = BP.Web.WebUser.No;

            //BP.WF.Dev2Interface.Port_Login(worker);

            ////�ö������̣��˶�����װ�ڵ�.
            //BP.WF.Dev2Interface.Node_SendWork("002", gwf.PWorkID);

            ////�л�����
            //BP.WF.Dev2Interface.Port_Login(currEmpNo);


            return base.FlowOverAfter();
        }

    }
}
