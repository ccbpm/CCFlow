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

namespace BP.LIMS
{
    /// <summary>
    /// ��������002
    /// ����������뵽 BP.*.dll ���ܱ��������������
    /// </summary>
    public class F002 : BP.WF.FlowEventBase
    {
        #region ����.
        /// <summary>
        /// ��д���̱��
        /// </summary>
        public override string FlowMark
        {
            get { return ",002,"; }
        }
        #endregion ����.

        #region ����.
        /// <summary>
        /// ���������¼�
        /// </summary>
        public F002()
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
            if (this.HisNode.NodeID == 201)
            {
                // StartSubFlows();
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

                if (this.HisNode.NodeID == 203)
                {
                    //����������״̬Ϊ�������.
                    DBAccess.RunSQL("UPDATE YB_YBFenXi SET YBSta=3 WHERE RefPK=" + this.WorkID);

                    //����������Ҳ�����״̬.
                    YBFenXis ens = new YBFenXis();
                    ens.Retrieve(YBFenXiAttr.RefPK, this.WorkID);
                    foreach (YBFenXi en in ens)
                    {
                        DBAccess.RunSQL("UPDATE YB_Pool SET YBSta=3 WHERE OID=" + en.OID);
                    }

                    //����Ƿ���ͬ������
                    Let001Run();
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
        /// �ø��������е���һ���ڵ���ȥ.
        /// </summary>
        public void Let001Run()
        {
            //���Ȼ�ñ��η������ж��ٸ�ί�У�
            string sql = "SELECT DISTINCT WorkIDOfWT FROM YB_YBFenXi WHERE RefPK=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);


            string webUserNo = BP.Web.WebUser.No;

            //����ί�У�ִ�м�飬��ί���£��Ƿ��Ѿ�ȫ������ˣ������ص�״̬.
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workidOfWT = Int64.Parse(dr[0].ToString());
                sql = "SELECT count(*) as Num FROM YB_Pool WHERE YBSta!=3 where refpk=" + workidOfWT;

                //���еĶ��ѷ������. 
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                }
                else
                {
                    continue;
                }


                //�� ί�����̣��ӵȴ������ڵ㣬�˶��� ��Ȩ��ǩ�ֽڵ�.

                //���������ǰ�˵Ĵ��� �� �����¼.
                sql = "select fk_emp from wf_generworkerlist where ispass=0 and workid="+workidOfWT +" and fk_node=103";
                string empNo = DBAccess.RunSQLReturnString(sql);
                BP.WF.Dev2Interface.Port_Login(empNo);

                // ִ�з��͡�
                BP.WF.Dev2Interface.Node_SendWork("001", workidOfWT, 0, null);

            }
        }

    }
}
