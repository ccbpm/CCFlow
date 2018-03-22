using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// ���� ����
    /// </summary>
    public class ResumeAttr
    {
        #region ��������
        /// <summary>
        /// ѧ�����
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// ������λ
        /// </summary>
        public const string GongZuoDanWei = "GongZuoDanWei";
        /// <summary>
        /// ֤����
        /// </summary>
        public const string ZhengMingRen = "ZhengMingRen";
        /// <summary>
        /// ��ע
        /// </summary>
        public const string BeiZhu = "BeiZhu";
        /// <summary>
        /// ����
        /// </summary>
        public const string NianYue = "NianYue";
        #endregion
    }
    /// <summary>
    /// ����
    /// </summary>
    public class Resume : BP.En.EntityOID
    {
        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public string NianYue
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.NianYue);
            }
            set
            {
                this.SetValByKey(ResumeAttr.NianYue, value);
            }
        }
        /// <summary>
        /// ��Ա
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.RefPK);
            }
            set
            {
                this.SetValByKey(ResumeAttr.RefPK, value);
            }
        }
        /// <summary>
        /// ������λ
        /// </summary>
        public string GongZuoDanWei
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.GongZuoDanWei);
            }
            set
            {
                this.SetValByKey(ResumeAttr.GongZuoDanWei, value);
            }
        }
        /// <summary>
        /// ֤����
        /// </summary>
        public string ZhengMingRen
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.ZhengMingRen);
            }
            set
            {
                this.SetValByKey(ResumeAttr.ZhengMingRen, value);
            }
        }
        /// <summary>
        /// ��ע
        /// </summary>
        public string BeiZhu
        {
            get
            {
                return this.GetValStringByKey(ResumeAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(ResumeAttr.BeiZhu, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ����
        /// </summary>
        public Resume()
        {
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="oid">ʵ����</param>
        public Resume(int oid):base(oid)
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
                Map map = new Map("Demo_Resume", "����");

                map.AddTBIntPKOID();
                map.AddTBString(ResumeAttr.RefPK, null, "ѧ�����", false, false, 0, 200, 10);
                map.AddTBString(ResumeAttr.NianYue, null, "����", true, false, 0, 200, 50);
                map.AddTBString(ResumeAttr.GongZuoDanWei, null, "������λ", true, false, 0, 200, 70);
                map.AddTBString(ResumeAttr.ZhengMingRen, "", "֤����", true, false, 1, 200, 50);
                map.AddTBString(ResumeAttr.BeiZhu, null, "��ע", true, false, 0, 200, 150);
                map.AddTBString("QT", null, "����", true, false, 0, 200, 150);
                map.AddTBString("QT2", null, "����11", true, false, 0, 200, 150);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (this.ZhengMingRen.Length == 0)
                throw new Exception("@֤������Ϣ����Ϊ��.");

            return base.beforeUpdateInsertAction();
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = true;
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;
            }
        }
    }
    /// <summary>
    /// ����s
    /// </summary>
    public class Resumes : BP.En.EntitiesOID
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Resume();
            }
        }
        /// <summary>
        /// ����s
        /// </summary>
        public Resumes() { }
        #endregion
    }
}
