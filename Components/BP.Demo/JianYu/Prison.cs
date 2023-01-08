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
    public class PrisonAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ������
        /// </summary>
        public const string BZR = "BZR";
        public const string Tel = "Tel";
    }
    /// <summary>
    /// ����
    /// </summary>
    public class Prison : BP.En.EntityNoName
    {
        #region ��������
        /// <summary>
        /// ������
        /// </summary>
        public string BZR
        {
            get
            {
                return this.GetValStrByKey(PrisonAttr.BZR);
            }
            set
            {
                this.SetValByKey(PrisonAttr.BZR, value);
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
        public Prison() { }
        public Prison(string no) : base(no)
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

                Map map = new Map("JY_Prison", "����");

                #region �ֶ� 
                map.AddTBStringPK(PrisonAttr.No, null, "���", true, true, 3, 3, 50);
                map.AddTBString(PrisonAttr.Name, null, "����", true, false, 0, 50, 200);
                // map.AddTBString(PrisonAttr.BZR, null, "������", true, false, 0, 50, 200);
                //  map.AddTBString(PrisonAttr.Tel, null, "�����ε绰", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new Prisons(); }
        }
        #endregion
    }
    /// <summary>
    /// ����s
    /// </summary>
    public class Prisons : BP.En.EntitiesNoName
    {
        #region ��д
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Prison();
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ����s
        /// </summary>
        public Prisons() { }
        #endregion
    }

}
