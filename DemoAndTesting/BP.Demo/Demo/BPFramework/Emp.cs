using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// ��Ա ����
    /// </summary>
    public class EmployeeAttr : EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// �Ա�
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// ��ַ
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public const string PWD = "PWD";
        /// <summary>
        /// �༶
        /// </summary>
        public const string FK_BanJi = "FK_BanJi";
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
        /// <summary>
        /// ��ע
        /// </summary>
        public const string Note = "Note";
        public const string BDT = "BDT";
        /// <summary>
        /// �Ƿ���������
        /// </summary>
        public const string IsTeKunSheng = "IsTeKunSheng";
        /// <summary>
        /// �Ƿ����ش󼲲�ʷ��
        /// </summary>
        public const string IsJiBing = "IsJiBing";
        /// <summary>
        /// �Ƿ�ƫԶɽ����
        /// </summary>
        public const string IsPianYuanShanQu = "IsPianYuanShanQu";
        /// <summary>
        /// �Ƿ������
        /// </summary>
        public const string IsDuShengZi = "IsDuShengZi";
        /// <summary>
        /// ������ò
        /// </summary>
        public const string ZZMM = "ZZMM";
        /// <summary>
        /// ����
        /// </summary>
        public const string GL = "GL";

        #endregion
    }
    /// <summary>
    /// ��Ա
    /// </summary>
    public class Employee : BP.En.EntityNoName
    {
        #region ����
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.PWD);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.PWD, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(EmployeeAttr.Age);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Age, value);
            }
        }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Addr);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Addr, value);
            }
        }

        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(EmployeeAttr.XB);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.XB, value);
            }
        }
        /// <summary>
        /// �Ա�����
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(EmployeeAttr.XB);
            }
        }
        /// <summary>
        /// �༶���
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// �༶����
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(EmployeeAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Email);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Email, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.Tel, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(EmployeeAttr.RegDate);
            }
            set
            {
                this.SetValByKey(EmployeeAttr.RegDate, value);
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
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                uac.IsView = true;
                return uac;
            }
        }
        /// <summary>
        /// ��Ա
        /// </summary>
        public Employee()
        {
        }
        /// <summary>
        /// ��Ա
        /// </summary>
        /// <param name="no"></param>
        public Employee(string no):base(no)
        {
        }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_Employee", "��Ա");
                //������Ϣ.
                map.IsAllowRepeatName = true; //�Ƿ����������ظ�.
                map.IsAutoGenerNo = true; //�Ƿ��Զ����ɱ��.
                map.Java_SetCodeStruct("4"); // 4λ���ı�ţ��� 0001 ��ʼ���� 9999.

                //��ͨ�ֶ�.
                map.AddTBStringPK(EmployeeAttr.No, null, "ѧ��", true, true, 4, 4, 4); // ��������Զ�����ֶα�����ֻ����.
                map.AddTBString(EmployeeAttr.Name, null, "����", true, false, 0, 200, 70);
                map.AddTBString(EmployeeAttr.PWD, null, "��¼����", true, false, 0, 200, 70);
                //map.AddTBString("shuoming", null, "˵��", true, false, 0, 200, 70);

                map.AddTBString(EmployeeAttr.Addr, null, "��ַ", true, false, 0, 200, 100,false,"http://sina.com.cn");
                map.AddTBInt(EmployeeAttr.Age, 0, "����", true, false);
                map.AddTBInt(EmployeeAttr.GL, 0, "����", true, false);

                map.AddTBString(EmployeeAttr.Tel, null, "�绰", true, false, 0, 200, 60);
                map.AddTBString(EmployeeAttr.Email, null, "�ʼ�", true, false, 0, 200, 50,true);
                
                map.AddTBDateTime(EmployeeAttr.RegDate, null, "ע������", true, true);
                map.AddTBDate(EmployeeAttr.BDT, null, "��������", true, true);

                map.AddTBStringDoc(EmployeeAttr.Note, null, "��ע", true, false, true); //����ı���.


                //ö���ֶ�
                map.AddDDLSysEnum(EmployeeAttr.XB, 0, "�Ա�", true, true, EmployeeAttr.XB, "@0=Ů@1=��");
                //����ֶ�.
                map.AddDDLEntities(EmployeeAttr.FK_BanJi, null, "�༶", new BP.Demo.BPFramework.BanJis(), true);


                //����checkbox����.
                map.AddBoolean(EmployeeAttr.IsDuShengZi, false, "�Ƿ��Ƕ����ӣ�", true, true);
                map.AddBoolean(EmployeeAttr.IsJiBing, false, "�Ƿ����ش󼲲���", true, true);
                map.AddBoolean(EmployeeAttr.IsPianYuanShanQu, false, "�Ƿ�ƫԶɽ����", true, true);
                map.AddBoolean(EmployeeAttr.IsTeKunSheng, false, "�Ƿ�����������", true, true);

                // ö���ֶ� - ������ò.
                map.AddDDLSysEnum(EmployeeAttr.ZZMM, 0, "������ò", true, true, EmployeeAttr.ZZMM,
                    "@0=���ȶ�Ա@1=��Ա@2=��Ա");

                map.AddMyFile("����");

                // map.AddMyFileS("����");

                //���ò�ѯ������
                map.AddSearchAttr(EmployeeAttr.XB);
                map.AddSearchAttr(EmployeeAttr.FK_BanJi);

              

                //���в����ķ���.
                RefMethod rm = new RefMethod();
                rm.Title = "���ɰ��";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "���ɽ��", true, false);
                rm.HisAttrs.AddTBString("Note", null, "��ע", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "ע��ѧ��";
                rm.Warning = "��ȷ��Ҫע����";
                rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
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
        protected override bool beforeUpdateInsertAction()
        {
            if (this.Email.Length == 0)
                throw new Exception("@email ����Ϊ��.");

            return base.beforeUpdateInsertAction();
        }

          
        #endregion ��д���෽��

        #region ����
        /// <summary>
        /// ���в����ķ���:���ɰ��
        /// ˵������Ҫ����string����.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {

            return "ѧ��:"+this.No+",����:"+this.Name+",������:"+jine+"Ԫ,˵��:"+note;
        }
        /// <summary>
        /// �޲����ķ���:ע��ѧ��
        /// ˵������Ҫ����string����.
        /// </summary>
        /// <returns></returns>
        public string DoZhuXiao()
        {
            return "ѧ��:" + this.No + ",����:" + this.Name + ",�Ѿ�ע��.";
        }
        /// <summary>
        /// У������
        /// </summary>
        /// <param name="pass">ԭʼ����</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool CheckPass(string pass)
        {
            return this.PWD.Equals(pass);
        }
        #endregion

    }
    /// <summary>
    /// ��Աs
    /// </summary>
    public class Employees : BP.En.EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ��Աs
        /// </summary>
        public Employees() { }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Employee();
            }
        }
        #endregion ��д���෽��

    }
}
