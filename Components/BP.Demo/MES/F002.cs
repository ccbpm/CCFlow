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
                DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=1 WHERE  RefPK=(select PWorkID from ND1Rpt  WHERE OID=" + this.WorkID + ")");


            }

            if (this.HisNode.NodeID == 202)
            {
                    DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=2 WHERE  RefPK=(select PWorkID from ND1Rpt  WHERE OID=" + this.WorkID + ")");


            }
            if (this.HisNode.NodeID == 203)
            {
                DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=3 WHERE  RefPK=" + this.WorkID );


            }

            if (this.HisNode.NodeID == 204)
            {
              //  DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=4 WHERE XTSta=3 AND RefPK=" + this.WorkID);


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

                if (this.HisNode.NodeID == 201)
                {
                    StartSubFlows();
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsEnable=0 WHERE  FK_Node !=201 and WorkID= " + workid);
                }

                if (this.HisNode.NodeID == 203)
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsEnable = 0 WHERE FK_Node = 204 and WorkID= " + workid);
            
                //����.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("������ϵͳд�����ʧ�ܣ���ϸ��Ϣ��"+ex.Message);
            }
        }

        public void StartSubFlows()
        {
            //����֮ǰ��ɾ���п��ܴ��ڵ���������.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
            foreach (GenerWorkFlow item in gwfs)
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.WorkID);
            }

            //������ö������ж��ٸ�����.
            ND201Dtl1s dtls = new ND201Dtl1s();
            dtls.Retrieve(ND201Dtl1Attr.RefPK, this.WorkID);

            //��ǰʵ�������.
            ND2Rpt rpt002 = new ND2Rpt(this.WorkID);


            //�������壬.
            foreach (ND201Dtl1 item in dtls)
            {
                 int i=item.Row.Count;
                //����һ���հ׵�workid.
                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001", BP.Web.WebUser.No);

                //ִ�б�������.
                ND1Rpt rpt001 = new ND1Rpt(workid);

                //�Ѷ���������ֶΣ����뵽��������������ȥ.
                rpt001.SetValByKey("DingDanHao", rpt002.GetValByKey("DingDanHao")); //�������
                rpt001.SetValByKey("PrjName", rpt002.GetValByKey("PrjName")); //��Ŀ����
                rpt001.SetValByKey("KeHuMingCheng", rpt002.GetValByKey("KeHuMingCheng")); //�ͻ�����
                rpt001.SetValByKey("QiXianXianDing", rpt002.GetValByKey("QiXianXianDing")); //�����޶�
                rpt001.SetValByKey("JJCD", rpt002.GetValByKey("JJCD")); //�����̶�
                

                //��������Ϣ��д�뵽 �������� ������ȥ.
                rpt001.SetValByKey("XiangTiMingCheng", item.GetValByKey("XiangTiMingCheng")); //��������.
                rpt001.SetValByKey("TuZhiBianHao", item.GetValByKey("TuZhiBianHao")); //ͼֽ���.
                rpt001.SetValByKey("TuZhiZhiTuRen", item.GetValByKey("TuZhiZhiTuRen")); //ͼֽ��ͼ��.
                //rpt001.SetValByKey("QiXianXianDing", item.GetValByKey("QiXianXianDing")); //�����޶�.
                //rpt001.SetValByKey("JJCD", item.GetValByKey("JJCD")); //�����̶�.
                rpt001.Update();
                
            

                //������������װ�ϵ�.
                ND201Dtl1Dtl1s dtl1s = new ND201Dtl1Dtl1s();
                dtl1s.Retrieve(ND201Dtl1Dtl1Attr.RefPK, item.OID);

                //��װ�ϵ���copy����������ȥ.
                ND101Dtl1 dtl101 = new ND101Dtl1();

                foreach (ND201Dtl1Dtl1 dtl1 in dtl1s)
                {
                    dtl101.Row = dtl1.Row;
                    dtl101.RefPK = workid;
                    dtl101.InsertAsOID(dtl1.OID);
                }

                //ִ�з���.
                BP.WF.Dev2Interface.Node_SendWork("001", workid);

                //���ø��ӹ�ϵ.
                BP.WF.Dev2Interface.SetParentInfo("001", workid, this.WorkID, BP.Web.WebUser.No, 202, false);


             
                //BP.WF.Dev2Interface.Node_SetDraft2Todolist("001", workid);
            }
        }

    }
}
