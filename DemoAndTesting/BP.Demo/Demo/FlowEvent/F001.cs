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

namespace BP.Demo.FlowEvent
{
    /// <summary>
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
            get { return "001,002,"; }
        }
        #endregion ����.

        #region ����.

        public override string WorkArrive()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            return  "���е��˴�"+this.WorkID+"_"+this.HisNode.NodeID;
           
        }
        public override string CreateWorkID()
        {
            return base.CreateWorkID();

            ////����ɾ������.
            //string dtlID = "ND101Dtl1";
            //DBAccess.RunSQL("DELETE FROM ND101Dtl1 WHERE RefPK='"+this.WorkID+"'");
            ////����entity.
            //GEDtl dtl = new GEDtl("ND101Dtl1");

            ////���ö��ֵ.
            //SysEnums ses = new SysEnums("ABC");
            ////����ö�ٰ����ǰ����в��뵽���ݿ�.
            //foreach (SysEnum item in ses)
            //{
            //    //dtl.OID = DBAccess.GenerOID("ND101Dtl1");
            //    dtl.RefPK = this.WorkID.ToString();
            //    //dtl.SetValByKey("SSss", "2001-12-01");
            //    dtl.InsertAsNew();
            //}
        }
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

            ///  throw new Exception("err@���������̷���������");
            if (SystemConfig.CustomerNo != "CCFlow")
                return null;

            //��صı���,
            // ��ǰ�Ľڵ�, �����ı������ this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int���͵�ID.
            string nodeName = this.HisNode.Name; // ��ǰ�ڵ�����.


            //�ı䷽����ߵ�����Ա.
            if (1 == 2)
            {
                var qingjiaTianshu = this.GetValInt("QingJiaTianshu");
                if (qingjiaTianshu == 1 && 1 == 1)
                {
                    this.JumpToNodeID = 109;
                    this.JumpToEmps = "zhangsan";
                }
            }


            switch (nodeID)
            {
                case 101:  //���ǵ�1���ڵ��ʱ��....
                    //  throw new Exception("���������̷�������,��ֹ��������.");
                    //this.JumpToEmps = "zhangsan,lisi";
                    //this.JumpToNode = new Node(102);

                    //  this.JumpToNodeID = 103;
                    // this.JumpToEmps = "zhoupeng,liping";
                    // this.ND01_SaveAfter();

                    //this.JumpToNodeID = 103;
                    //this.JumpToEmps = "zhoupeng";

                    return "SendWhen�¼��Ѿ�ִ�гɹ���";
                default:
                    break;
            }
            return null;
        }
        #endregion �����¼�.

        /// <summary>
        /// ִ��װ��ǰ���¼�.
        /// </summary>
        /// <returns>return null ��ˢ�£��κ����ݣ�����ˢ������.</returns>
        public override string FrmLoadBefore()
        {
            //throw new Exception("sssssssssssssss");
            return "ִ�гɹ�.";
            //return base.FrmLoadBefore();
        }

        public override string FlowOverAfter()
        {
            //������д�뵽�ӿ�.
            //this.WorkID;
            //this.WorkID;

            //string sql = "UPDATE SSS SET XX=1 WHERE OID=" + this.OID;
            //DBAccess.RunSQL(sql);


            return base.FlowOverAfter();
        }
        /// <summary>
        /// �����ִ�е��¼�
        /// </summary>
        /// <returns></returns>
        public override string SaveAfter()
        {
            switch (this.HisNode.NodeID)
            {
                case 101:
                    this.ND01_SaveAfter();
                    break;
                default:
                    break;
            }
            return base.SaveAfter();
        }
        /// <summary>
        /// �ڵ㱣����¼�
        /// ������������Ϊ:ND+�ڵ���_�¼���.
        /// </summary>
        public void ND01_SaveAfter()
        {

            if (DBAccess.IsExitsObject("ND101Dtl1") == false)
                return;

            if (DBAccess.IsExitsTableCol("ND101Dtl1", "XiaoJi") == false)
                return;

            if (DBAccess.IsExitsObject("ND101") == false)
                return;

            //  string val=this.getva

            //�����ϸ���ĺϼ�.
            float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT SUM(XiaoJi) as Num FROM ND101Dtl1 WHERE RefPK=" + this.OID, 0);

            //���ºϼ�Сд , �Ѻϼ�ת���ɴ�д.
            string sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "',HeJi=" + hj + "  WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);

            sql = "UPDATE ND1Rpt SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "',HeJi=" + hj + "  WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);

            //if (1 == 2)
            //    throw new Exception("@ִ�д���xxxxxx.");
            //�����Ҫ���û���ʾִ�гɹ�����Ϣ���͸�����ֵ������Ͳ��ظ�ֵ��
            //this.SucessInfo = "ִ�гɹ���ʾ.";
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
            catch (Exception ex)
            {
                return base.SendSuccess();
                //  throw new Exception("������ϵͳд�����ʧ�ܣ���ϸ��Ϣ��"+ex.Message);
            }
        }

        /// <summary>
        /// ����ʧ�ܵ�ʱ��
        /// </summary>
        /// <returns></returns>
        public override string SendError()
        {
            return base.SendError();
        }
    }
}