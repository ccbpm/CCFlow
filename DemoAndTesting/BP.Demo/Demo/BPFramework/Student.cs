using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// ѧ�� ����
    /// </summary>
    public class StudentAttr : EntityNoNameAttr
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
        #endregion

        /// <summary>
        /// Ƭ��
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// ʡ��
        /// </summary>
        public const string FK_SF = "FK_SF";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_City = "FK_City";
    }
    /// <summary>
    /// ѧ��
    /// </summary>
    public class Student : BP.En.EntityNoName
    {
        #region ����
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.PWD);
            }
            set
            {
                this.SetValByKey(StudentAttr.PWD, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.Age);
            }
            set
            {
                this.SetValByKey(StudentAttr.Age, value);
            }
        }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Addr);
            }
            set
            {
                this.SetValByKey(StudentAttr.Addr, value);
            }
        }
        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.XB);
            }
            set
            {
                this.SetValByKey(StudentAttr.XB, value);
            }
        }
        /// <summary>
        /// �Ա�����
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.XB);
            }
        }
        /// <summary>
        /// �༶���
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(StudentAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// �༶����
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Email);
            }
            set
            {
                this.SetValByKey(StudentAttr.Email, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Tel);
            }
            set
            {
                this.SetValByKey(StudentAttr.Tel, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.RegDate);
            }
            set
            {
                this.SetValByKey(StudentAttr.RegDate, value);
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
                //  uac.LoadRightFromCCGPM(this); //��GPM����װ��.
                // return uac;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    uac.IsView = true;
                }
                else
                {
                    uac.IsView = true;
                }
                uac.IsImp = true;
                return uac;
            }
        }
        /// <summary>
        /// ѧ��
        /// </summary>
        public Student()
        {
        }
        /// <summary>
        /// ѧ��
        /// </summary>
        /// <param name="no"></param>
        public Student(string no):base(no)
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

                Map map = new Map("Demo_Student", "ѧ��");
                //������Ϣ.
                map.IsAllowRepeatName = true; //�Ƿ����������ظ�.
                map.IsAutoGenerNo = true; //�Ƿ��Զ����ɱ��.
                map.Java_SetCodeStruct("4"); // 4λ���ı�ţ��� 0001 ��ʼ���� 9999.

                //��ͨ�ֶ�.
                map.AddTBStringPK(StudentAttr.No, null, "ѧ��", true, true, 4, 4, 4); // ��������Զ�����ֶα�����ֻ����.
                map.AddTBString(StudentAttr.Name, null, "����", true, false, 0, 200, 70);
                map.AddTBString(StudentAttr.PWD, null, "��¼����", true, false, 0, 200, 70);
                //map.AddTBString("shuoming", null, "˵��", true, false, 0, 200, 70);

                map.AddTBString(StudentAttr.Addr, null, "��ַ", true, false, 0, 200, 100, false);
                map.AddTBInt(StudentAttr.Age, 0, "����", true, false);
                map.AddTBString(StudentAttr.Tel, null, "�绰", true, false, 0, 200, 60);
                map.AddTBString(StudentAttr.Email, null, "�ʼ�", true, false, 0, 200, 50);
                map.AddTBDateTime(StudentAttr.RegDate, null, "ע������", true, true);

                map.AddDDLEntities(StudentAttr.FK_PQ, null, "Ƭ��",new BP.CN.PQs(),true);
                map.AddDDLEntities(StudentAttr.FK_SF, null, "ʡ��",new BP.CN.SFs(),true);
                map.AddDDLEntities(StudentAttr.FK_City, null, "����",new BP.CN.Citys(),true);

                //map.AddTBString(StudentAttr.FK_SF, null, "��ַ", true, false, 0, 200, 100, true);
                //map.AddTBString(StudentAttr, null, "��ַ", true, false, 0, 200, 100, true);
                //map.AddTBString("LXR", null, "��ϵ��", true, false, 0, 200, 100, true);
                //map.AddTBStringDoc(StudentAttr.Note, null, "��ע", true, false, true); //����ı���.

                //ö���ֶ�
                map.AddDDLSysEnum(StudentAttr.XB, 0, "�Ա�", true, true, StudentAttr.XB, "@0=Ů@1=��");
                //����ֶ�.
                map.AddDDLEntities(StudentAttr.FK_BanJi, null, "�༶", new BP.Demo.BPFramework.BanJis(), true);

                //����checkbox����.
                map.AddBoolean(StudentAttr.IsDuShengZi, false, "�Ƿ��Ƕ����ӣ�", true, true);
                map.AddBoolean(StudentAttr.IsJiBing, false, "�Ƿ����ش󼲲���", true, true);
                map.AddBoolean(StudentAttr.IsPianYuanShanQu, false, "�Ƿ�ƫԶɽ����", true, true);
                map.AddBoolean(StudentAttr.IsTeKunSheng, false, "�Ƿ�����������", true, true);

                //ö���ֶ� - ������ò.
                map.AddDDLSysEnum(StudentAttr.ZZMM, 0, "������ò", true, true, StudentAttr.ZZMM, "@0=���ȶ�Ա@1=��Ա@2=��Ա");

               //map.AddHidden("XB", " = ", "0");

                map.AddMyFile("����");

                //map.AddMyFileS("����");

                //���ò�ѯ������
                map.AddSearchAttr(StudentAttr.XB);
                map.AddSearchAttr(StudentAttr.FK_BanJi);
                map.AddSearchAttr(StudentAttr.ZZMM);


                //��Զ��ӳ��.
                map.AttrsOfOneVSM.Add(new StudentKeMus(), new KeMus(), StudentKeMuAttr.FK_Student,
                  StudentKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "ѧϰ�Ŀ�Ŀ");

                //��ϸ��ӳ��.
                map.AddDtl(new Resumes(), ResumeAttr.RefPK);

                //���в����ķ���.
                RefMethod rm = new RefMethod();
                rm.Title = "���ɰ��";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "���ɽ��", true, false);
                rm.HisAttrs.AddTBString("Note", null, "��ע", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
                rm.GroupName = "����ִ�в���";
                rm.IsCanBatch = false; //�Ƿ����������
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "ע��ѧ��";
                rm.Warning = "��ȷ��Ҫע����";
                rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
                rm.IsForEns = true;
                rm.IsCanBatch = true; //�Ƿ����������
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "����xx����";
                rm.ClassMethodName = this.ToString() + ".DoStartFlow";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsCanBatch = false; //�Ƿ����������
                map.AddRefMethod(rm);


                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "��ӡѧ��֤";
                rm.ClassMethodName = this.ToString() + ".DoPrintStuLicence";
                rm.IsCanBatch = true; //�Ƿ����������
                map.AddRefMethod(rm);


                ////�����в����ķ���.
                //rm = new RefMethod();
                //rm.Title = "������ӡѧ��֤";
                //rm.ClassMethodName = this.ToString() + ".EnsMothed";
                ////rm.IsForEns = true; //�Ƿ����������
                //rm.RefMethodType = RefMethodType.FuncBacthEntities; //�Ƿ����������
                //map.AddRefMethod(rm);

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
        public string DoPrintStuLicence()
        {

            BP.Pub.RTFEngine en = new Pub.RTFEngine();


            Student stu = new Student(this.No);

            en.HisGEEntity = stu; //��ǰ��ʵ��.

            //���Ӵӱ�.
            BP.Demo.Resumes dtls=new Resumes();
            dtls.Retrieve(ResumeAttr.RefPK,stu.No);
            en.AddDtlEns(dtls);


            string saveTo=BP.Sys.SystemConfig.PathOfTemp; // \\DataUser\\Temp\\
            string billFileName = this.No+"StuTest.doc";

            //Ҫ���ɵ�����.
            en.MakeDoc(BP.Sys.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\StudentDemo.rtf", saveTo, billFileName, null, false);

            string url = "/DataUser/Temp/" + billFileName;

            string info = "�������ɳɹ�:<a href='" + url + "' >��ӡ</a>��<a href='/SDKFlowDemo/App/PrintJoin.aspx'>ƴ�Ӵ�ӡ</a>";
            return info;
        }
        public string DoStartFlow()
        {
            return "/WF/MyFlow.htm?FK_Flow=001&FK_Studept="+this.No+"&StuName="+this.Name;

        }
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
    /// ѧ��s
    /// </summary>
    public class Students : BP.En.EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ѧ��s
        /// </summary>
        public Students() { }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Student();
            }
        }
        #endregion ��д���෽��

        #region ���Է���.
        public string EnsMothed()
        {
            return "EnsMothed@ִ�гɹ�.";
        }
        public string EnsMothedParas(string para1,string para2)
        {
            return "EnsMothedParas@ִ�гɹ�." + para1 + " - " + para2;
        }
        #endregion

    }
}
