using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.JianYu
{
    /// <summary>
    /// ��������069
    /// ����������뵽 BP.*.dll ���ܱ��������������
    /// </summary>
    public class F069 : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ��д���̱��
        /// </summary>
        public override string FlowMark
        {
            get { return ",069,"; }
        }
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F069()
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

                // ��ǰ�Ľڵ�, �����ı������ this.HisNode .
                int nodeID = this.HisNode.NodeID;    // int���͵�ID.
                if (this.HisNode.NodeID == 6901)
                {
                    // ��������еĲ�Ʒ..
                    string sql = "SELECT * FROM ND6901Dtl1 WHERE RefPK=" + this.WorkID;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);

                    //��ʼ����������.
                    string intfos = "��Ʒ�з�������������Ϣ���£�<br/>";
                    foreach (DataRow dr in dt.Rows)
                    {
                        //����������.WorkID.
                        Int64 workidSubFlow = BP.WF.Dev2Interface.Node_CreateBlankWork("070");

                        //���������Ʒ��������ˣ�����,���ֿ�. zhangsan,lisi
                        string fuzren = dr["FuZeRen"].ToString();

                        //�������̵��������ò�Ʒ��Ϣ����.
                        GEEntity rpt070 = new GEEntity("ND70Rpt", workidSubFlow);
                        // rpt070.Copy(dr);
                     //   rpt070.SetValByKey("ChanPinMingCheng", dr["ChanPinMingCheng"].ToString());
                        rpt070.SetValByKey("Tel", dr["Tel"].ToString());
                        rpt070.SetValByKey("FuZeRen", dr["FuZeRen"].ToString());
                        rpt070.Update();

                        //���ø��ӹ�ϵ.
                        BP.WF.Dev2Interface.SetParentInfo("070", workidSubFlow, this.WorkID, BP.Web.WebUser.No, 6902, false);
                        //ִ�з��ͣ�����2���ڵ���ȥ. 
                        intfos +=  "<br>������: "+ BP.WF.Dev2Interface.Node_SendWork("070", workidSubFlow, 7002, fuzren).ToMsgOfText();
                    }

                    return intfos;
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
