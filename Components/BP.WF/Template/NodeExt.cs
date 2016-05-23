using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// �ڵ�����.
    /// </summary>
    public class NodeExt : Entity
    {
        #region ����
        /// <summary>
        /// ��ȡ�ڵ�İ�����Ϣurl
        /// <para></para>
        /// <para>added by liuxc,2014-8-19</para> 
        /// </summary>
        /// <param name="sysNo">������վ������ϵͳNo</param>
        /// <param name="searchTitle">�����������</param>
        /// <returns></returns>
        private string this[string sysNo, string searchTitle]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sysNo) || string.IsNullOrWhiteSpace(searchTitle))
                    return "javascript:alert('�˴���û�а�����Ϣ��')";

                return string.Format("http://online.ccflow.org/KM/Tree.aspx?no={0}&st={1}", sysNo, Uri.EscapeDataString(searchTitle));
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// CCFlow��������
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm������
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region ����.
        /// <summary>
        /// ��ʱ����ʽ
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// ���ʹ���
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// ���ʹ���
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        /// <summary>
        /// �ڵ�ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        /// <summary>
        /// ���̱��
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// ������sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// �Ƿ�����˻�
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        #endregion ����.

        #region ���Ի�ȫ�ֵ� Node
        /// <summary>
        /// ���ʿ���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                Flow fl = new Flow(this.FK_Flow);
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// �ڵ�
        /// </summary>
        public NodeExt() { }
        /// <summary>
        /// �ڵ�
        /// </summary>
        /// <param name="nodeid">�ڵ�ID</param>
        public NodeExt(int nodeid)
        {
            this.NodeID = nodeid;
            this.Retrieve();
        }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                //map �Ļ� ����Ϣ.
                map.PhysicsTable = "WF_Node";
                map.EnDesc = "�ڵ�";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                #region  ��������
                map.AddTBIntPK(NodeAttr.NodeID, 0, "�ڵ�ID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");
                map.AddTBInt(NodeAttr.Step, 0, "����(�޼�������)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");
                //map.SetHelperAlert(NodeAttr.Step, "�����ڽڵ��������ȷ�����ò���������������׶�д."); //ʹ��alert�ķ�ʽ��ʾ������Ϣ.
                map.AddTBString(NodeAttr.FK_Flow, null, "���̱��", false, false, 3, 3, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");
                map.AddTBString(NodeAttr.Name, null, "����", true, true, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                map.AddTBString(NodeAttr.Tip, null, "������ʾ", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");


                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "˭ִ����",true, true, NodeAttr.WhoExeIt, "@0=����Աִ��@1=����ִ��@2=���ִ��");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");

                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "���ͺ�ת��",
                 true, true, NodeAttr.TurnToDeal, "@0=��ʾccflowĬ����Ϣ@1=��ʾָ����Ϣ@2=ת��ָ����url@3=��������ת��");
                map.SetHelperUrl(NodeAttr.TurnToDeal, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "ת��������", true, false, 0, 1000, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "�Ѷ���ִ", true, true, NodeAttr.ReadReceipts,
                    "@0=����ִ@1=�Զ���ִ@2=����һ�ڵ���ֶξ���@3=��SDK�����߲�������");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");

                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "�����������ƹ���", true, true, NodeAttr.CondModel,
                 "@0=����������������@1=���û��ֹ�ѡ��");
                map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //���Ӱ���

                // ��������.
                map.AddDDLSysEnum(NodeAttr.CancelRole,(int)CancelRole.OnlyNextStep, "��������", true, true,
                    NodeAttr.CancelRole,"@0=��һ�����Գ���@1=���ܳ���@2=��һ���뿪ʼ�ڵ���Գ���@3=ָ���Ľڵ���Գ���");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                // �ڵ㹤��������. edit by peng, 2014-01-24.    by huangzhimin ���ù���ר�ⷽʽ����������б�
                //map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "����������", true, true, NodeAttr.BatchRole, "@0=������������@1=�������@2=�����������");
                //map.AddTBInt(NodeAttr.BatchListCount, 12, "����������", true, false);
                ////map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "�ڵ㹤��������"]); //���Ӱ���
                //map.SetHelperUrl(NodeAttr.BatchRole, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.SetHelperUrl(NodeAttr.BatchListCount, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.AddTBString(NodeAttr.BatchParas, null, "���������", true, false, 0, 300, 10, true);
                //map.SetHelperUrl(NodeAttr.BatchParas, "http://ccbpm.mydoc.io/?v=5404&t=17920");


                map.AddBoolean(NodeAttr.IsTask, true, "������乤����?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsRM, true, "�Ƿ�����Ͷ��·���Զ����书��?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");

                map.AddTBDateTime("DTFrom", "�������ڴ�", true, true);
                map.AddTBDateTime("DTTo", "�������ڵ�", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "�Ƿ��ǽڵ�ģ�棨ҵ��Ԫ��?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                
                map.AddTBString(NodeAttr.FocusField, null, "�����ֶ�", true, false, 0, 50, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17932");
                map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "���淽ʽ", true, true);
                map.SetHelperUrl(NodeAttr.SaveModel, "http://ccbpm.mydoc.io/?v=5404&t=17934");

                map.AddBoolean(NodeAttr.IsGuestNode, false, "�Ƿ����ⲿ�û�ִ�еĽڵ�(����֯�ṹ��Ա���봦�����Ľڵ�)?", true, true, true);
                #endregion  ��������
                 
                #region �ֺ������߳�����
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "�ڵ�����",
                    true, true, NodeAttr.RunModel, "@0=��ͨ@1=����@2=����@3=�ֺ���@4=���߳�");

                map.SetHelperUrl(NodeAttr.RunModel, "http://ccbpm.mydoc.io/?v=5404&t=17940"); //���Ӱ���.
        
                //���߳�����.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "���߳�����", true, true, NodeAttr.SubThreadType, "@0=ͬ��@1=���");
                map.SetHelperUrl(NodeAttr.SubThreadType, "http://ccbpm.mydoc.io/?v=5404&t=17944"); //���Ӱ���


                map.AddTBDecimal(NodeAttr.PassRate, 100, "���ͨ����", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "http://ccbpm.mydoc.io/?v=5404&t=17945"); //���Ӱ���.

                // �������̲߳��� 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "���߳�������ʽ", true, true,
                    NodeAttr.SubFlowStartWay, "@0=������@1=ָ�����ֶ�����@2=����ϸ������");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "��������", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, "http://ccbpm.mydoc.io/?v=5404&t=17946"); //���Ӱ���

                //���촦��ģʽ.
                map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "���촦��ģʽ", true, true, NodeAttr.TodolistModel,
                    "@0=����ģʽ@1=Э��ģʽ@2=����ģʽ@3=����ģʽ");
                map.SetHelperUrl(NodeAttr.TodolistModel, "http://ccbpm.mydoc.io/?v=5404&t=17947"); //���Ӱ���.
                

                //��������ģʽ.
                //map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "��������ģʽ", true, true, NodeAttr.BlockModel,
                //    "@0=������@1=��ǰ�ڵ���δ��ɵ�������ʱ@2=��Լ����ʽ����δ���������@3=����SQL����@4=���ձ��ʽ����");
                //map.SetHelperUrl(NodeAttr.BlockModel, "http://ccbpm.mydoc.io/?v=5404&t=17948"); //���Ӱ���.

                //map.AddTBString(NodeAttr.BlockExp, null, "�������ʽ", true, false, 0, 700, 10,true);
                //map.SetHelperUrl(NodeAttr.BlockExp, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                //map.AddTBString(NodeAttr.BlockAlert, null, "������ʱ��ʾ��Ϣ", true, false, 0, 700, 10, true);
                //map.SetHelperUrl(NodeAttr.BlockAlert, "http://ccbpm.mydoc.io/?v=5404&t=17948");


                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "�Ƿ��������߳̽�����Ա�ظ�(���������������̷߳���ʱ��Ч)?", true, true, true);

                #endregion �ֺ������߳�����

                #region �Զ���ת����
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "�����˾��Ƿ�����", true, true, false);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "http://ccbpm.mydoc.io/?v=5404&t=17949"); //���Ӱ���

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "�������Ѿ����ֹ�", true, true, false);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "����������һ����ͬ", true, true, false);
                map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "�Ҳ��������˴������",
       true, true, NodeAttr.WhenNoWorker, "@0=��ʾ����@1=�Զ�ת����һ��");
                #endregion

                #region  ���ܰ�ť״̬
                map.AddTBString(BtnAttr.SendLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SendLab, "http://ccbpm.mydoc.io/?v=5404&t=16219");
                map.AddTBString(BtnAttr.SendJS, "", "��ťJS����", true, false, 0, 999, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow ����ǰ�����������ж�"); //���Ӱ���.
                map.SetHelperUrl(BtnAttr.SendJS, "http://ccbpm.mydoc.io/?v=5404&t=17967");

                map.AddTBString(BtnAttr.SaveLab, "����", "���水ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, "http://ccbpm.mydoc.io/?v=5404&t=24366"); //���Ӱ���

                map.AddTBString(BtnAttr.ThreadLab, "���߳�", "���̰߳�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "http://ccbpm.mydoc.io/?v=5404&t=16263"); //���Ӱ���

                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "���߳�ɾ����ʽ", true, true,
           NodeAttr.ThreadKillRole, "@0=����ɾ��@1=�ֹ�ɾ��@2=�Զ�ɾ��",true);
                //map.SetHelperUrl(NodeAttr.ThreadKillRole, ""); //���Ӱ���
               

                map.AddTBString(BtnAttr.SubFlowLab, "������", "�����̰�ť��ǩ", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SubFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=16262");
                map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "���ƹ���", true, true, BtnAttr.SubFlowCtrlRole, "@0=��@1=������ɾ��������@2=����ɾ��������");

                map.AddTBString(BtnAttr.JumpWayLab, "��ת", "��ת��ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "��ת����", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "����ת�Ľڵ�", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, "http://ccbpm.mydoc.io/?v=5404&t=16261"); //���Ӱ���.

                map.AddTBString(BtnAttr.ReturnLab, "�˻�", "�˻ذ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0,"�˻ع���",true, true, NodeAttr.ReturnRole);
              //  map.AddTBString(NodeAttr.ReturnToNodes, null, "���˻ؽڵ�", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.ReturnRole, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //���Ӱ���.

                map.AddBoolean(NodeAttr.IsBackTracking, false, "�Ƿ����ԭ·����(�����˻ع��ܲ���Ч)", true, true, false);
                map.AddTBString(BtnAttr.ReturnField, "", "�˻���Ϣ��д�ֶ�", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //���Ӱ���.

                map.AddTBString(BtnAttr.CCLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "���͹���", true, true, NodeAttr.CCRole,
                    "@@0=���ܳ���@1=�ֹ�����@2=�Զ�����@3=�ֹ����Զ�@4=����SysCCEmps�ֶμ���@5=�ڷ���ǰ�򿪳��ʹ���");
                map.SetHelperUrl(BtnAttr.CCLab, "http://ccbpm.mydoc.io/?v=5404&t=16259"); //���Ӱ���.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "����д�����",
             true, true, NodeAttr.CCWriteTo, "@0=д�볭���б�@1=д�����@2=д������볭���б�", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //���Ӱ���

                map.AddTBString(BtnAttr.ShiftLab, "�ƽ�", "�ƽ���ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, "http://ccbpm.mydoc.io/?v=5404&t=16257"); //���Ӱ���.note:none

                map.AddTBString(BtnAttr.DelLab, "ɾ��", "ɾ����ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "ɾ������", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "http://ccbpm.mydoc.io/?v=5404&t=17992"); //���Ӱ���.

                map.AddTBString(BtnAttr.EndFlowLab, "��������", "�������̰�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17989"); //���Ӱ���

                map.AddTBString(BtnAttr.PrintDocLab, "��ӡ����", "��ӡ���ݰ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "��ӡ��ʽ", true,
                    true, BtnAttr.PrintDocEnable, "@0=����ӡ@1=��ӡ��ҳ@2=��ӡRTFģ��@3=��ӡWordģ��");
                map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //���Ӱ���

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "�Ƿ�����", true, true);
                //map.AddTBString(BtnAttr.AthLab, "����", "������ť��ǩ", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "����Ȩ��"), true, true, 
                //    NodeAttr.FJOpen, "@0=�رո���@1=����Ա@2=����ID@3=����ID");

                map.AddTBString(BtnAttr.TrackLab, "�켣", "�켣��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "�Ƿ�����", true, true);
                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "�켣"]); //���Ӱ���
                map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "����", "����ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, "http://ccbpm.mydoc.io/?v=5404&t=16267"); //���Ӱ���.

                map.AddTBString(BtnAttr.SelectAccepterLab, "������", "�����˰�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "������ʽ",
          true, true, BtnAttr.SelectAccepterEnable);
                map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //���Ӱ���


                map.AddTBString(BtnAttr.SearchLab, "��ѯ", "��ѯ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "�Ƿ�����", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "��ѯ"]); //���Ӱ���
                map.SetHelperUrl(BtnAttr.SearchLab, "http://ccbpm.mydoc.io/?v=5404&t=24373");

                map.AddTBString(BtnAttr.WorkCheckLab, "���", "��˰�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.BatchLab, "������", "������ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //���Ӱ���

                map.AddTBString(BtnAttr.AskforLab, "��ǩ", "��ǩ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");

                // add by ���� 2014-11-21. ���û������Լ�������ת.
                map.AddTBString(BtnAttr.TCLab, "��ת�Զ���", "��ת�Զ���", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.TCEnable, "http://ccbpm.mydoc.io/?v=5404&t=17978");

                //map.AddTBString(BtnAttr.AskforLabRe, "ִ��", "��ǩ��ť��ǩ", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "�Ƿ�����", true, true);

               // map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "��ǩ"]); //���Ӱ���


                // ɾ�������ģʽ,�ñ��������п�����,�����������ֶ��Լ���.
                map.AddTBString(BtnAttr.WebOfficeLab, "����", "�ĵ���ť��ǩ", false, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.WebOfficeEnable, 0, "�ĵ����÷�ʽ", false, false);
                
                //cut bye zhoupeng.
                //map.AddTBString(BtnAttr.WebOfficeLab, "����", "�ĵ���ť��ǩ", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "�ĵ����÷�ʽ", true, true, BtnAttr.WebOfficeEnable,
                //  "@0=������@1=��ť��ʽ@2=��ǩҳ�ú�ʽ@3=��ǩҳ��ǰ��ʽ");//edited by liuxc,2016-01-18,from xc
                //map.SetHelperUrl(BtnAttr.WebOfficeLab, "http://ccbpm.mydoc.io/?v=5404&t=17993");

                // add by ���� 2015-08-06. ��Ҫ��.
                map.AddTBString(BtnAttr.PRILab, "��Ҫ��", "��Ҫ��", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PRIEnable, false, "�Ƿ�����", true, true);

                // add by ���� 2015-08-06. �ڵ�ʱ��.
                map.AddTBString(BtnAttr.CHLab, "�ڵ�ʱ��", "�ڵ�ʱ��", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.CHEnable, false, "�Ƿ�����", true, true);


                // add by ���� 2015-12-24. �ڵ�ʱ��.
                map.AddTBString(BtnAttr.FocusLab, "��ע", "��ע", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, true, "�Ƿ�����", true, true);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "�Ƿ�����", true, true);
                #endregion  ���ܰ�ť״̬

                #region ��������
               // // ��������
               // map.AddTBFloat(NodeAttr.TSpanDay, 0, "����(��)", true, false); //"����(��)".
               // map.AddTBFloat(NodeAttr.TSpanHour, 8, "Сʱ", true, false); //"����(��)".

               // map.AddTBFloat(NodeAttr.WarningDay, 0, "����Ԥ��(��)", true, false);    // "��������(0������)"
               // map.AddTBFloat(NodeAttr.WarningHour, 0, "����Ԥ��(Сʱ)", true, false); // "��������(0������)"
               // map.SetHelperUrl(NodeAttr.WarningHour, "http://ccbpm.mydoc.io/?v=5404&t=17999");
               // map.AddTBFloat(NodeAttr.TCent, 1, "�۷�(ÿ����1Сʱ)", true, false); //"�۷�(ÿ����1���)"
                
               // //map.AddTBFloat(NodeAttr.MaxDeductCent, 0, "��߿۷�", true, false);   //"��߿۷�"
               // //map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"), "�����÷�", true, false); //"�����÷�".
               // //map.AddTBString(NodeAttr.FK_Flows, null, "flow", false, false, 0, 100, 10);

               // map.AddDDLSysEnum(NodeAttr.CHWay, 0, "���˷�ʽ", true, true, NodeAttr.CHWay,"@0=������@1=��ʱЧ@2=��������");

               //// map.AddTBFloat(NodeAttr.Workload, 0, "������(��λ:Сʱ)", true, false);

               // // �Ƿ��������˵㣿
               // map.AddBoolean(NodeAttr.IsEval, false, "�Ƿ��������˵�", true, true, true);


               // // ȥ����, �ƶ��� ���⹦�ܽ��洦����.
               // map.AddDDLSysEnum(NodeAttr.OutTimeDeal, 0, "��ʱ����", true, true, NodeAttr.OutTimeDeal,
               // "@0=������@1=�Զ������˶�@2=�Զ���תָ���Ľڵ�@3=�Զ��ƽ���ָ������Ա@4=��ָ������Ա����Ϣ@5=ɾ������@6=ִ��SQL");
               // map.AddTBString(NodeAttr.DoOutTime, null, "��������", true, false, 0, 300, 10, true);
               // map.AddTBString(NodeAttr.DoOutTimeCond, null, "ִ�г�ʱ����", true, false, 0, 100, 10, true);
               // map.SetHelperUrl(NodeAttr.OutTimeDeal, "http://ccbpm.mydoc.io/?v=5404&t=18001");
                #endregion ��������

                #region ����������, �˴������BP.Sys.FrmWorkCheck ҲҪ���.
                // BP.Sys.FrmWorkCheck
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "������״̬",
                    true, true, FrmWorkCheckAttr.FWCSta, "@0=����@1=����@2=ֻ��");
                map.SetHelperUrl(FrmWorkCheckAttr.FWCSta, "http://ccbpm.mydoc.io/?v=5404&t=17936");
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "��ʾ��ʽ",
                    true, true, FrmWorkCheckAttr.FWCShowModel, "@0=���ʽ@1=����ģʽ"); //��������ʱû����.
                map.SetHelperUrl(FrmWorkCheckAttr.FWCShowModel, "http://ccbpm.mydoc.io/?v=5404&t=17937");
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCType, (int)FWCType.Check, "������ʽ", true, true,
                    FrmWorkCheckAttr.FWCType, "@0=������@1=��־���@2=�ܱ����@3=�±����");
                map.SetHelperUrl(FrmWorkCheckAttr.FWCType, "http://ccbpm.mydoc.io/?v=5404&t=17938");
                // add by stone 2015-03-19. ���Ϊ�գ���ȥ�ڵ�������ʾ��������.
                map.AddTBString(FrmWorkCheckAttr.FWCNodeName, null, "�ڵ��������", true, false, 0, 100, 10);

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCAth, (int)FWCAth.None, "�����ϴ�", true, true,
                   FrmWorkCheckAttr.FWCAth, "@0=������@1=�฽��@2=������(�ݲ�֧��)@3=ͼƬ����(�ݲ�֧��)");
                map.SetHelperAlert(FrmWorkCheckAttr.FWCAth,
                    "������ڼ䣬�Ƿ������ϴ�����������ʲô���ĸ�����ע�⣺�����������ڽڵ�������á�"); //ʹ��alert�ķ�ʽ��ʾ������Ϣ.

                map.AddBoolean(FrmWorkCheckAttr.FWCTrackEnable, true, "�켣ͼ�Ƿ���ʾ��", true, true, false);
                map.AddBoolean(FrmWorkCheckAttr.FWCListEnable, true, "��ʷ�����Ϣ�Ƿ���ʾ��(��,�����������)", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsShowAllStep, false, "�ڹ켣�����Ƿ���ʾ���еĲ��裿", true, true,true);
                map.AddBoolean(FrmWorkCheckAttr.SigantureEnabel, false, "ʹ��ͼƬǩ��(����Ϣ��д�ײ���ʾ����OrͼƬǩ��)��", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsFullInfo, true, "����û�δ����Ƿ���Ĭ�������䣿", true, true, true);


                map.AddTBString(FrmWorkCheckAttr.FWCOpLabel, "���", "��������(���/����/��ʾ)", true, false, 0, 50, 10);
                map.AddTBString(FrmWorkCheckAttr.FWCDefInfo, "ͬ��", "Ĭ�������Ϣ", true, false, 0, 50, 10);

                //map.AddTBFloat(FrmWorkCheckAttr.FWC_X, 5, "λ��X", true, false);
                //map.AddTBFloat(FrmWorkCheckAttr.FWC_Y, 5, "λ��Y", true, false);
                
                // �߶�����, ��������ɱ��Ͳ�Ҫ�仯������.
                map.AddTBFloat(FrmWorkCheckAttr.FWC_H, 300, "�߶�", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_H, "��������ɱ��Ͳ�Ҫ�仯������,Ϊ0�����ʶΪ100%,Ӧ�õ����ģʽ."); //���Ӱ���
                map.AddTBFloat(FrmWorkCheckAttr.FWC_W, 400, "���", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_W, "��������ɱ��Ͳ�Ҫ�仯������,Ϊ0�����ʶΪ100%,Ӧ�õ����ģʽ."); //���Ӱ���
                
                map.AddTBStringDoc(FrmWorkCheckAttr.FWCFields, null, "������ʽ���ֶ�", true, false,true);
                #endregion ����������.

                #region ���İ�ť del by zhoupeng. �����²��ı�׼�޸�.
                //map.AddTBString(BtnAttr.OfficeOpenLab, "�򿪱���", "�򿪱��ر�ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOpenLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "��ģ��", "��ģ���ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOpenTemplateLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeSaveLab, "����", "�����ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeSaveLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeAcceptLab, "�����޶�", "�����޶���ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeAcceptLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeRefuseLab, "�ܾ��޶�", "�ܾ��޶���ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeRefuseLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeOverLab, "�׺�", "�׺찴ť��ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeOverLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeOverEnable, false, "�Ƿ�����", true, true);
               

                //map.AddTBString(BtnAttr.OfficePrintLab, "��ӡ", "��ӡ��ť��ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficePrintLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficePrintEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeSealLab, "ǩ��", "ǩ�°�ť��ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeSealLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeSealEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeDownLab, "����", "���ذ�ť��ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeDownLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeDownEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.OfficeInsertFlowLab, "��������", "�������̱�ǩ", true, false, 0, 50, 10);
                //map.SetHelperUrl(BtnAttr.OfficeInsertFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17998");
                //map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "�Ƿ�����", true, true);

                //map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "�Ƿ��¼�ڵ���Ϣ", true, true);
                //map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "�Ƿ���Զ�����ΪPDF", true, true);


                //map.AddBoolean(BtnAttr.OfficeIsMarks, true, "�Ƿ��������ģʽ", true, true);
                //map.AddTBString(BtnAttr.OfficeTemplate, "", "ָ���ĵ�ģ��", true, false, 0, 100, 10);
                //map.SetHelperUrl(BtnAttr.OfficeTemplate, "http://ccbpm.mydoc.io/?v=5404&t=17998");

                //map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "�Ƿ�鿴�û�����", true, true, true);

                //map.AddBoolean(BtnAttr.OfficeIsParent, true, "�Ƿ�ʹ�ø����̵��ĵ�", true, true);

                //map.AddBoolean(BtnAttr.OfficeTHEnable, false, "�Ƿ��Զ��׺�", true, true);
                //map.AddTBString(BtnAttr.OfficeTHTemplate, "", "�Զ��׺�ģ��", true, false, 0, 200, 10);
                //map.SetHelperUrl(BtnAttr.OfficeTHTemplate, "http://ccbpm.mydoc.io/?v=5404&t=17998");

                //if (Glo.IsEnableZhiDu)
                //{
                //    map.AddTBString(BtnAttr.OfficeFengXianTemplate, "", "���յ�ģ��", true, false, 0, 100, 10);
                //    map.AddTBString(BtnAttr.OfficeInsertFengXian, "������յ�", "������յ��ǩ", true, false, 0, 50, 10);
                //    map.AddBoolean(BtnAttr.OfficeInsertFengXianEnabel, false, "�Ƿ�����", true, true);
                //}

                //map.AddTBString(BtnAttr.OfficeDownLab, "����", "���ذ�ť��ǩ", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OfficeIsDown, false, "�Ƿ�����", true, true);
                #endregion

                #region �ƶ�����.
                map.AddDDLSysEnum(NodeAttr.MPhone_WorkModel, 0, "�ֻ�����ģʽ", true, true, NodeAttr.MPhone_WorkModel, "@0=ԭ��̬@1=�����@2=����");
                map.AddDDLSysEnum(NodeAttr.MPhone_SrcModel, 0, "�ֻ���Ļģʽ", true, true, NodeAttr.MPhone_SrcModel, "@0=ǿ�ƺ���@1=ǿ������@2=��������Ӧ����");

                map.AddDDLSysEnum(NodeAttr.MPad_WorkModel, 0, "ƽ�幤��ģʽ", true, true, NodeAttr.MPad_WorkModel, "@0=ԭ��̬@1=�����@2=����");
                map.AddDDLSysEnum(NodeAttr.MPad_SrcModel, 0, "ƽ����Ļģʽ", true, true, NodeAttr.MPad_SrcModel, "@0=ǿ�ƺ���@1=ǿ������@2=��������Ӧ����");
                map.SetHelperUrl(NodeAttr.MPhone_WorkModel, "http://bbs.ccflow.org/showtopic-2866.aspx");
                #endregion �ƶ�����.

                //�ڵ㹤����, ���ӱ�ӳ��.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region ��������.
                RefMethod rm = null;

                rm = new RefMethod();
                rm.Title = "�����˹���";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Menu/Sender.png";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����˹���";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Menu/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoCCer";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "������";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/Form.png";

                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                 rm = new RefMethod();
                rm.Title = "�ڵ��¼�"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ڵ���Ϣ"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

             

                rm = new RefMethod();
                rm.Title = "��������";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/SubFlows.png";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ֻ����ֶ�˳��";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����������"; // "�����������";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���ͺ�ת��"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "������������"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

              


                rm = new RefMethod();
                rm.Title = "��Ϣ����"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Msg.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                if (Glo.IsEnableZhiDu)
                {
                    rm = new RefMethod();
                    rm.Title = "��Ӧ�ƶ��½�"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoZhiDu";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "���յ�"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoFengXianDian";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "��λְ��"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoGangWeiZhiZe";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);
                }

                #endregion ��������.

                #region �ֶ���ع��ܣ�����ʾ�ڲ˵��
                rm = new RefMethod();
                rm.Title = "���˻صĽڵ�(���˻ع������ÿ��˻�ָ���Ľڵ�ʱ,��������Ч.)"; // "��Ʊ�";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "���ÿ��˻صĽڵ�";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ɳ����Ľڵ�"; // "�ɳ������͵Ľڵ�";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.CancelRole;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���ͳɹ�ת������"; // "ת������";
                rm.ClassMethodName = this.ToString() + ".DoTurn";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";

                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.TurnToDealDoc;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��rtf��ӡ��ʽģ��(����ӡ��ʽΪ��ӡRTF��ʽģ��ʱ,��������Ч)"; //"����&����";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.PrintDocEnable;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                {
                    /* Ϊ���ɰ�����õĸ��Ի�����. */
                    rm = new RefMethod();
                    rm.Title = "DXReport����";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "�����Զ����͹���(���ڵ�Ϊ�Զ�����ʱ,��������Ч.)"; // "���͹���";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "�Զ���������";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion �ֶ���ع��ܣ�����ʾ�ڲ˵��

                #region ����.
                rm = new RefMethod();
                rm.Title = "���ÿ��˹���";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "���˹���";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��ʱ�������";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "���˹���";
                map.AddRefMethod(rm);
                #endregion ����.

                #region ʵ���еĹ���
                rm = new RefMethod();
                rm.Title = "�������ýڵ�����";
                rm.Icon = Glo.CCFlowAppPath + "WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "ʵ���еĹ���";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���ö�������Ȩ��";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "ʵ���еĹ���";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�������������";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "ʵ���еĹ���";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ڵ�����ģʽ(������)"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoRunModel";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "ʵ���еĹ���";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ر��ֶ������û�Ȩ��";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "ʵ���еĹ���";
                map.AddRefMethod(rm);

                #endregion ʵ���еĹ���

                this._enMap = map;
                return this._enMap;
            }
        }

        #region ���˹���.
        /// <summary>
        /// ���˹���
        /// </summary>
        /// <returns></returns>
        public string DoCHRole()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/CHRole.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// ��ʱ�������
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/CHOvertimeRole.aspx?FK_Node=" + this.NodeID;
        }
        #endregion ���˹���.

        #region ��������.
        /// <summary>
        /// ���������
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrNode/BatchStartFields.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node="+this.NodeID;
        }
        /// <summary>
        /// �����޸Ľڵ�����
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/AttrFlow/NodeAttrs.aspx?NodeID=0&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        public string DoSheet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/NodeFromWorkModel.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SubFlows.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// �����˹���
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeAccepterRole.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// ������������
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/BlockModel.aspx?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// ���ͺ�ת�����
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/TurnToDeal.aspx?FK_Node=" + this.NodeID;
        }
        
        /// <summary>
        /// �����˹���
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeCCRole.aspx?FK_Node=" + this.NodeID;
        }
        #endregion 

        /// <summary>
        /// �ر��û������ֶ�Ȩ��.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SepcFiledsSepcUsers.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node="+this.NodeID+"&t=" + DataType.CurrentDataTime;
        }

        /// <summary>
        /// �ڵ�����ģʽ.
        /// </summary>
        /// <returns></returns>
        public string DoRunModel()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/NodeRunModel.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// �����ֶ�˳��
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/SortingMapAttrs.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// ���Ų�����
        /// </summary>
        /// <returns></returns>
        public string DoDepts()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Comm/Port/DeptTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&RefNo=" + DataType.CurrentDataTime, 500, 550);
            return null;
        }
        /// <summary>
        /// ���ö�������Ȩ��
        /// </summary>
        /// <returns></returns>
        public string DoNodeFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" +
                   this.NodeID + "&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// �ƶ�
        /// </summary>
        /// <returns></returns>
        public string DoZhiDu()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeZhiDuDtl.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "�ƶ�", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// ���յ�
        /// </summary>
        /// <returns></returns>
        public string DoFengXianDian()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeFengXianDian.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "�ƶ�", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        public string DoSelectAccepter()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/NodeAccepterRole.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// ���˹���
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRole()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);

            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
            //    return "�ڵ���ʹ�����û�����ð���bpmģʽ����������ִ�иò�����Ҫ��ִ�иò�����ѡ��ڵ������нڵ�������Ȼ��ѡ����bpmģʽ���㣬�㱣�水ť��";

            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
         //   return null;
        }
        public string DoTurn()
        {
            return Glo.CCFlowAppPath + "WF/Admin/TurnTo.aspx?FK_Node=" + this.NodeID;
            //, "�ڵ����ת����", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// ���͹���
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.CC&PK=" + this.NodeID; 
            //PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.CC&PK=" + this.NodeID, "���͹���", "Bill", 800, 500, 200, 300);
            //return null;
        }
        /// <summary>
        /// ���Ի������˴���
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
        }
        /// <summary>
        /// �ɴ�����������
        /// </summary>
        /// <returns></returns>
        public string DoActiveFlows()
        {
            return Glo.CCFlowAppPath + "WF/Admin/ConditionSubFlow.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// �˻ؽڵ�
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanReturnNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// �������͵Ľڵ�
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanCancelNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            return Glo.CCFlowAppPath + "WF/Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoPush2Current()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoPush2Spec()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        /// <summary>
        /// ִ����Ϣ����
        /// </summary>
        /// <returns></returns>
        public string DoListen()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoFeatureSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FeatureSetUI.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoShowSheets()
        {
            return Glo.CCFlowAppPath + "WF/Admin/ShowSheets.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoCond()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Condition.aspx?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// ���ɵ�ϱ�
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// ������ɱ�
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// �󶨶�����
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/BindFrms.aspx?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }
        
        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // ����.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "��Ʊ�", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case NodeFormType.FixForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID, "��Ʊ�", "sheet", 800, 500, 210, 300);
                    break;
            }
            return null;
        }

        /// <summary>
        /// ��Ϣ
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return Glo.CCFlowAppPath + "WF/Admin/AttrNode/PushMessage.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// �¼�
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Action.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// ���ݴ�ӡ
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Bill.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public string DoFAppSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        
        protected override bool beforeUpdate()
        {
            //�������̰汾
            Flow.UpdateVer(this.FK_Flow);

            //�ѹ����������÷��� sys_mapdata��.
            ToolbarExcel te = new ToolbarExcel("ND" + this.NodeID);
            te.Copy(this);
            try
            {
                te.Update();
            }
            catch
            {
            }
           
            #region  ��鿼�����ڴ�������õ�������.
            //string doOutTime = this.GetValStrByKey(NodeAttr.DoOutTime);
            //switch (this.HisOutTimeDeal)
            //{
            //    case OutTimeDeal.AutoJumpToSpecNode:
            //        string[] jumps = doOutTime.Split(',');
            //        if (jumps.Length  > 2)
            //        {
            //            string msg = "�Զ���ת����Ӧ�ڵ�,���õ����ݲ���ȷ,��ʽӦ��Ϊ: Node,EmpNo , ����: 101,zhoupeng  �������õĸ�ʽΪ:" + doOutTime;
            //            throw new Exception(msg);
            //        }
            //        break;
            //    case OutTimeDeal.AutoShiftToSpecUser:
            //    case OutTimeDeal.RunSQL:
            //    case OutTimeDeal.SendMsgToSpecUser:
            //        if (string.IsNullOrEmpty(doOutTime) == false)
            //            throw new Exception("@�ڿ������ڴ���ʽ�ϣ���ѡ�����:" + this.HisOutTimeDeal + " ,������û��Ϊ�ù����������ݡ�");
            //        break;
            //    default:
            //        break;
            //}
            #endregion ��鿼�����ڴ�������õ�������

            #region ����ڵ�����.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                /*����ť������*/
                //�����˻�, ��ǩ���ƽ����˻�, ���߳�.
                this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //���߳�.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*����Ǻ�����*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //���߳�.
            }
            #endregion ����ڵ�����.

            #region ������Ϣ�����ֶ�.
            //this.SetPara(NodeAttr.MsgCtrl, this.GetValIntByKey(NodeAttr.MsgCtrl));
            //this.SetPara(NodeAttr.MsgIsSend, this.GetValIntByKey(NodeAttr.MsgIsSend));
            //this.SetPara(NodeAttr.MsgIsReturn, this.GetValIntByKey(NodeAttr.MsgIsReturn));
            //this.SetPara(NodeAttr.MsgIsShift, this.GetValIntByKey(NodeAttr.MsgIsShift));
            //this.SetPara(NodeAttr.MsgIsCC, this.GetValIntByKey(NodeAttr.MsgIsCC));

            //this.SetPara(NodeAttr.MailEnable, this.GetValIntByKey(NodeAttr.MailEnable));
            //this.SetPara(NodeAttr.MsgMailTitle, this.GetValStrByKey(NodeAttr.MsgMailTitle));
            //this.SetPara(NodeAttr.MsgMailDoc, this.GetValStrByKey(NodeAttr.MsgMailDoc));

            //this.SetPara(NodeAttr.MsgSMSEnable, this.GetValIntByKey(NodeAttr.MsgSMSEnable));
            //this.SetPara(NodeAttr.MsgSMSDoc, this.GetValStrByKey(NodeAttr.MsgSMSDoc));
            #endregion

            //��������������
            FrmAttachment workCheckAth = new FrmAttachment();
            bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK, this.NodeID + "_FrmWorkCheck");
            //������������
            if (isHave == false)
            {
                workCheckAth = new FrmAttachment();
                /*���û�в�ѯ����,���п�����û�д���.*/
                workCheckAth.MyPK = this.NodeID + "_FrmWorkCheck";
                workCheckAth.FK_MapData = this.NodeID.ToString();
                workCheckAth.NoOfObj = this.NodeID + "_FrmWorkCheck";
                workCheckAth.Exts = "*.*";

                //�洢·��.
                workCheckAth.SaveTo = "/DataUser/UploadFile/";
                workCheckAth.IsNote = false; //����ʾnote�ֶ�.
                workCheckAth.IsVisable = false; // ������form �ϲ��ɼ�.

                //λ��.
                workCheckAth.X = (float)94.09;
                workCheckAth.Y = (float)333.18;
                workCheckAth.W = (float)626.36;
                workCheckAth.H = (float)150;

                //�฽��.
                workCheckAth.UploadType = AttachmentUploadType.Multi;
                workCheckAth.Name = "������";
                workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                workCheckAth.Insert();
            }   

            //������еĻ���.
            BP.DA.CashEntity.DCash.Clear();

            return base.beforeUpdate();
        }
        #endregion
    }
    /// <summary>
    /// �ڵ㼯��
    /// </summary>
    public class NodeExts : Entities
    {
        #region ���췽��
        /// <summary>
        /// �ڵ㼯��
        /// </summary>
        public NodeExts()
        {
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
