using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// ��Ʒ ����
    /// </summary>
    public class ProductAttr : EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// �Ա�
        /// </summary>
        public const string GuiGe = "GuiGe";
        public const string BeiZhu = "BeiZhu";
        #endregion
    }
    /// <summary>
    /// ��Ʒ
    /// </summary>
    public class Product : BP.En.EntityNoName
    {

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
                if (BP.Web.WebUser.No.Equals("admin")==true)
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
        /// ��Ʒ
        /// </summary>
        public Product()
        {
        }
        /// <summary>
        /// ��Ʒ
        /// </summary>
        /// <param name="no"></param>
        public Product(string no)
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

                Map map = new Map("Demo_Product", "��Ʒ");

                //������Ϣ.
                map.ItIsAllowRepeatName = true; //�Ƿ����������ظ�.
                map.ItIsAutoGenerNo = true; //�Ƿ��Զ����ɱ��.
                map.CodeStruct = "4"; // 4λ���ı�ţ��� 0001 ��ʼ���� 9999.

                #region �ֶ�ӳ�� - ��ͨ�ֶ�.
                map.AddTBStringPK(ProductAttr.No, null, "��Ʒ���", true, true, 4, 4, 90); // ��������Զ�����ֶα�����ֻ����.
                map.AddTBString(ProductAttr.Name, null, "����", true, false, 0, 200, 70);
                map.AddTBString(ProductAttr.GuiGe, null, "���", true, false, 0, 200, 100, true);
                map.AddTBStringDoc(ResumeAttr.BeiZhu, null, "��ע", true, false);
                map.AddTBAtParas(2000);
                #endregion �ֶ�ӳ�� - ��ͨ�ֶ�.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion ��д���෽��
        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }

    }
    /// <summary>
    /// ��Ʒs
    /// </summary>
    public class Products : BP.En.EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ��Ʒs
        /// </summary>
        public Products() { }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Product();
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
