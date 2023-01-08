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
    public class JianQuAttr : EntityNoNameAttr
    {
        /// <summary>
        /// �������
        /// </summary>
        public const string PrisonNo = "PrisonNo";
        public const string Tel = "Tel";
    }
    /// <summary>
    /// ����
    /// </summary>
    public class JianQu : BP.En.EntityNoName
    {
        #region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(JianQuAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(JianQuAttr.PrisonNo, value);
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
        public JianQu() { }
        public JianQu(string no) : base(no)
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

                Map map = new Map("JY_JianQu", "����");

                #region �ֶ� 
                map.AddTBStringPK(JianQuAttr.No, null, "���", true, true, 3, 3, 50);
                map.AddTBString(JianQuAttr.Name, null, "����", true, false, 0, 50, 200);

                map.AddTBString(JianQuAttr.PrisonNo, null, "�������", true, false, 0, 50, 200);

              //  map.AddDDLEntities(JianQuAttr.PrisonNo, null, "����", new Prisons(), false);

                //map.AddTBString(JianQuAttr.PrisonNo, null, "������", true, false, 0, 50, 200);
                // map.AddTBString(JianQuAttr.Tel, null, "�����ε绰", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new JianQus(); }
        }
        #endregion
    }
    /// <summary>
    /// ����s
    /// </summary>
    public class JianQus : BP.En.EntitiesNoName
    {
        #region ��д
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new JianQu();
            }
        }
        /// <summary>
        /// ��ѯȫ��
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            string no = BP.Web.WebUser.FK_Dept;

            no = no.Substring(0, 4);

            return this.Retrieve("PrisonNo", no);

           // return base.RetrieveAll();
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ����s
        /// </summary>
        public JianQus() { }
        #endregion
    }

}
