using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// ���� ����
    /// </summary>
    public class FanrenAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ������
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";

        public const string WorkIDOfBatch = "WorkIDOfBatch";
        public const string WorkIDOfSubthread = "WorkIDOfSubthread";
        /// <summary>
        /// �ּ������
        /// </summary>
        public const string FenJianQuNo = "FenJianQuNo";
        /// <summary>
        /// �������
        /// </summary>
        public const string JianQuNo = "JianQuNo";
        /// <summary>
        /// �������
        /// </summary>
        public const string PrisonNo = "PrisonNo";
    }
    /// <summary>
    /// ����
    /// </summary>
    public class Fanren : BP.En.EntityNoName
    {
        #region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string FenJianQuNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.FenJianQuNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.FenJianQuNo, value);
            }
        }
        public string JianQuNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.JianQuNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.JianQuNo, value);
            }
        }
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(FanrenAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(FanrenAttr.PrisonNo, value);
            }
        }
        /// <summary>
        /// ���߳�ID.
        /// </summary>
        public int WorkIDOfSubthread
        {
            get
            {
                return this.GetValIntByKey(FanrenAttr.WorkIDOfSubthread);
            }
            set
            {
                this.SetValByKey(FanrenAttr.WorkIDOfSubthread, value);
            }
        }
        /// <summary>
        /// ����ID
        /// </summary>
        public int WorkIDOfBatch
        {
            get
            {
                return this.GetValIntByKey(FanrenAttr.WorkIDOfBatch);
            }
            set
            {
                this.SetValByKey(FanrenAttr.WorkIDOfBatch, value);
            }
        }

        #endregion

        #region ���캯��
        /// <summary>
        /// ʵ���Ȩ�޿���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (BP.Web.WebUser.No == "zhoupeng" || BP.Web.WebUser.No.Equals("admin") == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                }
                else
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = false;
                    uac.IsInsert = false;
                }
                return uac;
            }
        }
        /// <summary>
        /// ����
        /// </summary>		
        public Fanren() { }
        public Fanren(string no) : base(no)
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("JY_Fanren", "����");

                #region �ֶ� 
                map.AddTBStringPK(FanrenAttr.No, null, "���", true, true, 3, 3, 50);
                map.AddTBString(FanrenAttr.Name, null, "����", true, false, 0, 50, 200);

                map.AddDDLEntities(FanrenAttr.PrisonNo, null, "����", new Prisons(), false);
                map.AddTBInt(FanrenAttr.WorkIDOfBatch, 0, "���ε�WorkID", true, false);

                map.AddDDLEntities(FanrenAttr.JianQuNo, null, "����", new JianQus(), false);
                map.AddTBInt(FanrenAttr.WorkIDOfSubthread, 0, "���߳�WorkID", true, false);

                map.AddDDLEntities(FanrenAttr.FenJianQuNo, null, "�ּ���", new FenJianQus(), false);
                #endregion

                //���ռ�������ѯ.
                map.AddSearchAttr(JianQuAttr.PrisonNo);

                //�����в����ķ���.
                RefMethod rm = new RefMethod();
                rm.Title = "��������";
                rm.ClassMethodName = this.ToString() + ".DoFlow";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsCanBatch = false; //�Ƿ����������
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoFlow()
        {
            return "/WF/MyView.htm?WorkID=" + this.WorkIDOfSubthread+"&FID="+this.WorkIDOfBatch+"&FK_Flow=001";
        }
        protected override bool beforeInsert()
        {

            FenJianQu en = new FenJianQu(this.FenJianQuNo);
            this.PrisonNo = en.PrisonNo;
            this.JianQuNo = en.JianQuNo;

            return base.beforeInsert();
        }
        public override Entities GetNewEntities
        {
            get { return new Fanrens(); }
        }
        #endregion
    }
    /// <summary>
    /// ����s
    /// </summary>
    public class Fanrens : BP.En.EntitiesNoName
    {
        #region ��д
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Fanren();
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ����s
        /// </summary>
        public Fanrens() { }
        #endregion
    }

}
