using System;
using System.Threading;
using System.Collections;

using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.MES
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
            get { return ",001,"; }
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

            // ��ǰ�Ľڵ�, �����ı������ this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int���͵�ID.
            string nodeName = this.HisNode.Name; // ��ǰ�ڵ�����.
            switch (nodeID)
            {
                case 102:  //�ж��Ƿ�ȫ���Ϲ�,���ϸ���׳��쳣..
                    string sql = "SELECT COUNT(*) FROM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND  WorkSta!= 1 AND  WorkSta!=4  ";
                  
                   DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=1    WHERE REFPK=(SELECT PWORKID FROM ND1Rpt WHERE OID=" + this.WorkID +")  AND XiangTiMingCheng = (SELECT XiangTiMingCheng FROM ND1Rpt WHERE OID = "+ this.WorkID + ")");

                    var num = DBAccess.RunSQLReturnValInt(sql);
                    if (num != 0)
                        throw new Exception("err@����Ŀ�С�" + num + "��û�м��ϸ񣬻���û����ɣ������ܷ��͡�");
                    break;
                case 103:

                    // 0=δ����. 1=�����. 2=����. 3=���ϸ�. 4=��ɴ����. 5=����ϸ� 
                    /* 1. ������ WorkSta=4�� �޸�δ5  === ����ɴ������޸�δ������ϸ�.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=4 AND RefPK=" + this.WorkID);

                    /* 2. ������ WorkSta=2 �� �޸�δ5 === �������ģ��޸�δ����ϸ� */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=2 AND RefPK=" + this.WorkID);

                    /* 3. ������ WorkSta=1 �� �޸�δ3  ====  ������ɵģ�����δ���ϸ�.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=1 AND RefPK=" + this.WorkID);


                    //�ж��Ƿ�ȫ���Ϲ�,���ϸ���׳��쳣..
                    string sql3 = "SELECT COUNT(*) FROM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND WorkSta!= 3 ";
                    var num3 = DBAccess.RunSQLReturnValInt(sql3);
                    if (num3 != 0)
                        throw new Exception("err@����Ŀ�С�" + num3 + "��û�м��ϸ񣬻���û����ɣ������ܷ��͡�");
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
                Int64 workid = this.WorkID; // ����id.w
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

                //������ڵ㣬���·���.
                if (this.HisNode.NodeID == 103)
                {
                    // 0=δ����. 1=�����. 2=����. 3=���ϸ�. 4=��ɴ����. 5=����ϸ� 
                    /* 1. ������ WorkSta=4�� �޸�δ5  === ����ɴ������޸�δ������ϸ�.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=4 AND RefPK="+this.WorkID);

                    /* 2. ������ WorkSta=2 �� �޸�δ5 === �������ģ��޸�δ����ϸ� */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=2 AND RefPK=" + this.WorkID);

                    /* 3. ������ WorkSta=1 �� �޸�δ3  ====  ������ɵģ�����δ���ϸ�.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=1 AND RefPK=" + this.WorkID);
               

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

        /// <summary>
        /// ���̽���֮��
        /// </summary>
        /// <returns></returns>
        public override string FlowOverAfter()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            //���ÿ���ö�����ÿ�������̣��Ƿ���ɣ������ɣ����ö������̣��˶��������װ�ڵ㡣
            string sql = "SELECT COUNT(*) from WF_GenerWorkFlow WHERE PWorkID=" + gwf.PWorkID + " AND WFState!=3 ";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num > 0)
               
           return ""; //˵���ö���δ��ɵ�����.

           // �����������װ��ļල��Ա.
            sql = "SELECT FK_Emp from WF_GenerWorkerList WHERE WorkID=" + gwf.PWorkID + " AND FK_Node=202 AND IsPass=0 ";
            string worker = DBAccess.RunSQLReturnStringIsNull(sql, null);
            if (worker == null)
                throw new Exception("err@��Ӧ�ò�ѯ��������Ա��");

            string currEmpNo = BP.Web.WebUser.No;

            BP.WF.Dev2Interface.Port_Login(worker);

            //�ö������̣��˶�����װ�ڵ�.
            BP.WF.Dev2Interface.Node_SendWork("002", gwf.PWorkID);

            //�л�����
            BP.WF.Dev2Interface.Port_Login(currEmpNo);


            return base.FlowOverAfter();
        }

    }
}
