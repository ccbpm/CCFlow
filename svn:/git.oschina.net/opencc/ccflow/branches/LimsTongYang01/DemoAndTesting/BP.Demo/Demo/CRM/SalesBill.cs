using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// �����ʵ� ����
    /// </summary>
    public class SaleBillAttr
    {
        #region ��������
        /// <summary>
        /// �����ڵ�
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// ���
        /// </summary>
        public const string FK_ND = "FK_ND";
        /// <summary>
        /// ���
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// ��Ʒ
        /// </summary>
        public const string FK_Product = "FK_Product";
        #endregion
    }
    /// <summary>
    /// �����ʵ�
    /// </summary>
    public class SaleBill : EntityOID
    {
        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public string FK_Product
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Product);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Product, value);
            }
        }
        /// <summary>
        /// ��Ա
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// ������λ
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// ���
        /// </summary>
        public string FK_ND
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_ND);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_ND, value);
            }
        }
        /// <summary>
        /// ���
        /// </summary>
        public decimal JE
        {
            get
            {
                return this.GetValDecimalByKey(SaleBillAttr.JE);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.JE, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// �����ʵ�
        /// </summary>
        public SaleBill()
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
                Map map = new Map("Demo_SaleBill","�����ʵ�");

                map.AddTBIntPKOID();
                map.AddDDLEntities(SaleBillAttr.FK_Dept, null, "����", new BP.Port.Depts(), false);
                map.AddDDLEntities(SaleBillAttr.FK_Emp, null, "��Ա", new BP.Port.Emps(),false);
                
                map.AddDDLEntities(SaleBillAttr.FK_ND, null, "���", new BP.Pub.NDs(), false);
                map.AddDDLEntities(SaleBillAttr.FK_NY, null, "����", new BP.Pub.NYs(), false);

                map.AddDDLEntities(SaleBillAttr.FK_Product, null, "��Ʒ", new BP.Demo.CRM.Products(), false);
                map.AddTBDecimal(SaleBillAttr.JE, 0, "���۽��", true, false);
                map.AddTBString(BP.Demo.CRM.ProductAttr.Addr, null, "������ַ", true, false, 0, 50, 200);

                //��ѯ����ӳ��.
                map.AddSearchAttr(SaleBillAttr.FK_Dept);
                map.AddSearchAttr(SaleBillAttr.FK_NY);
                map.AddSearchAttr(SaleBillAttr.FK_Product);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// �����ʵ�s
    /// </summary>
    public class SaleBills : EntitiesOID
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SaleBill();
            }
        }
        /// <summary>
        /// �����ʵ�s
        /// </summary>
        public SaleBills() { }
        #endregion
    }
}
