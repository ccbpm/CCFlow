using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// �̶��ʲ� ����
    /// </summary>
    public class GDZCAttr : EntityNoNameAttr
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
        public const string jinE = "jinE";
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
    /// �̶��ʲ�
    /// </summary>
    public class GDZC : BP.En.EntityNoName
    {
        #region ����
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.PWD);
            }
            set
            {
                this.SetValByKey(GDZCAttr.PWD, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int jinE
        {
            get
            {
                return this.GetValIntByKey(GDZCAttr.jinE);
            }
            set
            {
                this.SetValByKey(GDZCAttr.jinE, value);
            }
        }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Addr);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Addr, value);
            }
        }
        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(GDZCAttr.XB);
            }
            set
            {
                this.SetValByKey(GDZCAttr.XB, value);
            }
        }
        /// <summary>
        /// �Ա�����
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(GDZCAttr.XB);
            }
        }
        /// <summary>
        /// �༶���
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(GDZCAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// �༶����
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(GDZCAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Email);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Email, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.Tel);
            }
            set
            {
                this.SetValByKey(GDZCAttr.Tel, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(GDZCAttr.RegDate);
            }
            set
            {
                this.SetValByKey(GDZCAttr.RegDate, value);
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
        /// �̶��ʲ�
        /// </summary>
        public GDZC()
        {
        }
        /// <summary>
        /// �̶��ʲ�
        /// </summary>
        /// <param name="no"></param>
        public GDZC(string no)
            : base(no)
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

                Map map = new Map("Demo_GDZC", "�̶��ʲ�");

                //������Ϣ.
                map.IsAllowRepeatName = true; //�Ƿ����������ظ�.
                map.IsAutoGenerNo = true; //�Ƿ��Զ����ɱ��.
                map.Java_SetCodeStruct("4"); // 4λ���ı�ţ��� 0001 ��ʼ���� 9999.

                #region �ֶ�ӳ�� - ��ͨ�ֶ�.
                map.AddTBStringPK(GDZCAttr.No, null, "�̶��ʲ����", true, true, 4, 4, 90); // ��������Զ�����ֶα�����ֻ����.
                map.AddTBString(GDZCAttr.Name, null, "����", true, false, 0, 200, 70);

                map.AddTBString(GDZCAttr.Addr, null, "��ַ", true, false, 0, 200, 100, true);
                map.AddTBInt(GDZCAttr.jinE, 18, "���", true, false);
                map.AddTBInt("Yuanzhi", 18, "ԭֵ", true, false);


                #endregion �ֶ�ӳ�� - ��ͨ�ֶ�.

                map.AddMyFile("��Ƭ");
                //map.AddMyFileS("����");

                //#region ���ò�ѯ����.
                ////string���ͣ���ؼ��ֲ�ѯ����.
                //map.SearchFields = "@��ַ=" + GDZCAttr.Addr;
                //map.SearchFields += "@�绰=" + GDZCAttr.Tel;

                ////��ֵ���ͣ���Χ��ѯ����.
                //map.SearchFieldsOfNum = "@����=" + GDZCAttr.jinE;

                ////�������ڲ�ѯ����.
                //map.DTSearchKey = GDZCAttr.RegDate;
                //map.DTSearchLable = "ע������";
                //map.DTSearchWay = Sys.DTSearchWay.ByDate; 

                ////�����ö�١�
                //map.AddSearchAttr(GDZCAttr.XB,1001); //��ȴ���1000���ǻ���.
                //map.AddSearchAttr(GDZCAttr.FK_BanJi);

                ////���صĲ�ѯ����.
                ////map.AddHidden("XB", " = ", "0");
                //#endregion ���ò�ѯ����.


                //#region ������ʵ���ӳ��.
                ////��Զ��ӳ��.
                //map.AttrsOfOneVSM.Add(new GDZCKeMus(), new KeMus(), GDZCKeMuAttr.FK_GDZC,
                //  GDZCKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "ѡ�޵Ŀ�Ŀ");

                ////��ϸ��ӳ��.
                //map.AddDtl(new Resumes(), ResumeAttr.RefPK);
                //#endregion ������ʵ���ӳ��.

               // #region ����ӳ��.

               // //���в����ķ���.
               // RefMethod rm = new RefMethod();
               // rm.Title = "���ɰ��";
               // rm.HisAttrs.AddTBDecimal("JinE", 100, "���ɽ��", true, false);
               // rm.HisAttrs.AddTBString("Note", null, "��ע", true, false, 0, 100, 100);
               //// rm.HisAttrs.AddTBString("Nowete", null, "22��ע", true, false, 0, 100, 100);
               // rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
               // rm.GroupName = "����ִ�в���";
               // //  rm.IsCanBatch = false; //�Ƿ����������
               // map.AddRefMethod(rm);

               // //�����в����ķ���.
               // rm = new RefMethod();
               // rm.Title = "ע��ѧ��";
               // rm.Warning = "��ȷ��Ҫע����";
               // rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
               // rm.IsForEns = true;
               // rm.IsCanBatch = true; //�Ƿ����������
               // map.AddRefMethod(rm);

               // //�����в����ķ���.
               // rm = new RefMethod();
               // rm.Title = "����xx����";
               // rm.ClassMethodName = this.ToString() + ".DoStartFlow";
               // rm.RefMethodType = RefMethodType.LinkeWinOpen;
               // rm.IsCanBatch = false; //�Ƿ����������
               // map.AddRefMethod(rm);

               // //�����в����ķ���.
               // rm = new RefMethod();
               // rm.Title = "��ӡ�̶��ʲ�֤";
               // rm.ClassMethodName = this.ToString() + ".DoPrintStuLicence";
               // rm.IsCanBatch = true; //�Ƿ����������
               // map.AddRefMethod(rm);
               // #endregion ����ӳ��.

                ////�����в����ķ���.
                //rm = new RefMethod();
                //rm.Title = "������ӡ�̶��ʲ�֤";
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
            return base.beforeUpdateInsertAction();
        }

        #endregion ��д���෽��

        #region ����
        public string DoPrintStuLicence()
        {

            BP.Pub.RTFEngine en = new Pub.RTFEngine();

            GDZC stu = new GDZC(this.No);

            en.HisGEEntity = stu; //��ǰ��ʵ��.

            //���Ӵӱ�.
            BP.Demo.Resumes dtls = new Resumes();
            dtls.Retrieve(ResumeAttr.RefPK, stu.No);
            en.AddDtlEns(dtls);

            string saveTo = BP.Sys.SystemConfig.PathOfTemp; // \\DataUser\\Temp\\
            string billFileName = this.No + "StuTest.doc";

            //Ҫ���ɵ�����.
            en.MakeDoc(BP.Sys.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\GDZCDemo.rtf", saveTo, billFileName, false);

            string url = "/DataUser/Temp/" + billFileName;

            string info = "�������ɳɹ�:<a href='" + url + "' >��ӡ</a>��<a href='/SDKFlowDemo/App/PrintJoin.aspx'>ƴ�Ӵ�ӡ</a>";
            return info;
        }
        public string DoStartFlow()
        {
            return "/WF/MyFlow.htm?FK_Flow=001&FK_Studept=" + this.No + "&StuName=" + this.Name;
        }
        /// <summary>
        /// ���в����ķ���:���ɰ��
        /// ˵������Ҫ����string����.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {
            return "ѧ��:" + this.No + ",����:" + this.Name + ",������:" + jine + "Ԫ,˵��:" + note;
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

        protected override bool beforeDelete()
        {

            return base.beforeDelete();
        }

    }
    /// <summary>
    /// �̶��ʲ�s
    /// </summary>
    public class GDZCs : BP.En.EntitiesNoName
    {
        #region ����
        /// <summary>
        /// �̶��ʲ�s
        /// </summary>
        public GDZCs() { }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GDZC();
            }
        }
        #endregion ��д���෽��

        #region ���Է���.
        public string EnsMothed()
        {
            return "EnsMothed@ִ�гɹ�.";
        }
        public string EnsMothedParas(string para1, string para2)
        {
            return "EnsMothedParas@ִ�гɹ�." + para1 + " - " + para2;
        }
        #endregion

    }
}
