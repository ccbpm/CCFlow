using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.JianYu
{
    /// <summary>
    /// �ּ��� ����
    /// </summary>
    public class FenJianQuAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ������
        /// </summary>
        public const string JianYuNo = "BZR";
        public const string Tel = "Tel";
        /// <summary>
        /// �������
        /// </summary>
        public const string PrisonNo = "PrisonNo";
        /// <summary>
        /// �������
        /// </summary>
        public const string JianQuNo = "JianQuNo";


    }
    /// <summary>
    /// �ּ���
    /// </summary>
    public class FenJianQu : BP.En.EntityNoName
    {
        #region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string PrisonNo
        {
            get
            {
                return this.GetValStrByKey(FenJianQuAttr.PrisonNo);
            }
            set
            {
                this.SetValByKey(FenJianQuAttr.PrisonNo, value);
            }
        }
        public string JianQuNo
        {
            get
            {
                return this.GetValStrByKey(FenJianQuAttr.JianQuNo);
            }
            set
            {
                this.SetValByKey(FenJianQuAttr.JianQuNo, value);
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
        /// �ּ���
        /// </summary>		
        public FenJianQu() { }
        public FenJianQu(string no) : base(no)
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

                Map map = new Map("JY_FenJianQu", "�ּ���");

                #region �ֶ� 
                map.AddTBStringPK(FenJianQuAttr.No, null, "���", true, true, 3, 3, 50);
                map.AddTBString(FenJianQuAttr.Name, null, "����", true, false, 0, 50, 200);

                map.AddTBString(FenJianQuAttr.PrisonNo, null, "����", true, false, 0, 50, 200);
                map.AddTBString(FenJianQuAttr.JianQuNo, null, "����", true, false, 0, 50, 200);

               // map.AddDDLEntities(JianQuAttr.PrisonNo, null, "����", new Prisons(), false);
               // map.AddDDLEntities(FenJianQuAttr.JianQuNo, null, "����", new JianQus(), false);

                //	map.AddTBString(FenJianQuAttr.BZR, null, "������", true, false, 0, 50, 200);
                //  map.AddTBString(FenJianQuAttr.Tel, null, "�����ε绰", true, false, 0, 50, 200);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new FenJianQus(); }
        }
        #endregion
    }
    /// <summary>
    /// �ּ���s
    /// </summary>
    public class FenJianQus : BP.En.EntitiesNoName
    {
        #region ��д
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FenJianQu();
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// �ּ���s
        /// </summary>
        public FenJianQus() { }
        #endregion
    }

}
