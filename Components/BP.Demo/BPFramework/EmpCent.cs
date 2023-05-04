using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// Ա�����˵÷� ����
    /// </summary>
    public class EmpCentAttr : EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// Ա��
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// �÷�
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        #endregion
    }
    /// <summary>
    /// Ա�����˵÷�
    /// </summary>
    public class EmpCent : EntityMyPK
    {
        #region ����
        /// <summary>
        /// Ա��
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// �÷�
        /// </summary>
        public float Cent
        {
            get
            {
                return this.GetValFloatByKey(EmpCentAttr.Cent);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.Cent, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(EmpCentAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpCentAttr.FK_Dept, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// Ա�����˵÷�
        /// </summary>
        public EmpCent()
        {
        }
        /// <summary>
        /// Ա�����˵÷�
        /// </summary>
        /// <param name="mypk"></param>
        public EmpCent(string mypk):base(mypk)
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
                Map map = new Map("Demo_EmpCent", "Ա�����˵÷�");
                
                // ��ͨ�ֶ�
                map.AddMyPK();
                map.AddTBString(EmpCentAttr.FK_Emp, null, "Ա��", true, false, 0, 200, 10);
                map.AddTBString(EmpCentAttr.FK_Dept, null, "��������(������)", true, false, 0, 200, 10);
                map.AddTBString(EmpCentAttr.FK_NY, null, "�·�", true, false, 0, 200, 10);
                map.AddTBFloat(EmpCentAttr.Cent, 0, "�÷�", true, false);

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
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// Ա�����˵÷�s
    /// </summary>
    public class EmpCents : EntitiesMyPK
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpCent();
            }
        }
        /// <summary>
        /// Ա�����˵÷�s
        /// </summary>
        public EmpCents() { }
        #endregion
    }
}
