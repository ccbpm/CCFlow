using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// ע���û� ����
    /// </summary>
    public class BPUserAttr : EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// ����
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// �Ա�
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// ��ַ
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// ����
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// �ʼ�
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// �绰
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// ע��ʱ��
        /// </summary>
        public const string RegDate = "RegDate";
        #endregion
    }
    /// <summary>
    /// ע���û�
    /// </summary>
    public class BPUser : EntityNoName
    {
        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Pass);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Pass, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(BPUserAttr.Age);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Age, value);
            }
        }
        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(BPUserAttr.XB);
            }
            set
            {
                this.SetValByKey(BPUserAttr.XB, value);
            }
        }
        /// <summary>
        /// �Ա�����
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(BPUserAttr.XB);
            }
        }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Addr);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Addr, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(BPUserAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Email);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Email, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.Tel);
            }
            set
            {
                this.SetValByKey(BPUserAttr.Tel, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(BPUserAttr.RegDate);
            }
            set
            {
                this.SetValByKey(BPUserAttr.RegDate, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ע���û�
        /// </summary>
        public BPUser()
        {
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
                Map map = new Map("Demo_BPUser", "ע���û�");
                
                // ��ͨ�ֶ�
                map.AddTBStringPK(BPUserAttr.No, null, "�û���", true, false, 1, 100, 100);
                map.AddTBString(BPUserAttr.Pass, null, "����", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Name, null, "����", true, false, 0, 200, 10);
                map.AddTBInt(BPUserAttr.Age, 0, "����", true, false);
                map.AddTBString(BPUserAttr.Addr, null, "��ַ", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Tel, null, "�绰", true, false, 0, 200, 10);
                map.AddTBString(BPUserAttr.Email, null, "�ʼ�", true, false, 0, 200, 10);
                map.AddTBDateTime(BPUserAttr.RegDate, null, "ע������", true, true);

                //ö���ֶ�
                map.AddDDLSysEnum(BPUserAttr.XB, 0,"�Ա�", false, true, BPUserAttr.XB, "@0=Ů@1=��");

                //����ֶ�.
                map.AddDDLEntities(BPUserAttr.FK_NY, null, "��������", new BP.Pub.NYs(),true);

                //���ò�ѯ������
                map.AddSearchAttr(BPUserAttr.XB);
                map.AddSearchAttr(BPUserAttr.FK_NY);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        /// <summary>
        /// ��д����ķ���.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            //�ڲ���֮ǰ����ע��ʱ��.
            this.RegDate = DataType.CurrentDataTime;

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// ע���û�s
    /// </summary>
    public class BPUsers : EntitiesNoName
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BPUser();
            }
        }
        /// <summary>
        /// ע���û�s
        /// </summary>
        public BPUsers() { }
        #endregion
    }
}
