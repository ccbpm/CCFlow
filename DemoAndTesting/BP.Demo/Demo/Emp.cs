using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// Ա�� ����
    /// </summary>
    public class EmpAttr:EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// �绰
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// �ʼ�
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// �Ա�
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// ��ַ
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// ����
        /// </summary>
        public const string Age = "Age";
        #endregion
    }
    /// <summary>
    /// Ա��
    /// </summary>
    public class Emp : EntityNoName
    {
        #region ����
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {  return this.GetValStringByKey(EmpAttr.Email); }
            set
            {  this.SetValByKey(EmpAttr.Email, value); }
        }
        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            { return this.GetValIntByKey(EmpAttr.XB);    }
            set
            {  this.SetValByKey(EmpAttr.XB, value);      }
        }
        /// <summary>
        /// �Ա��ǩ
        /// </summary>
        public string XB_Text
        {
            get
            { return this.GetValRefTextByKey(EmpAttr.XB); }
        }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.Addr);
            }
            set
            {
                this.SetValByKey(EmpAttr.Addr, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpAttr.Tel, value);
            }
        }
        /// <summary>
        /// ���ű��
        /// </summary>
        public string FK_Dept
        {
            get
            { return this.GetValStringByKey(EmpAttr.FK_Dept);  }
            set
            {  this.SetValByKey(EmpAttr.FK_Dept, value); }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string FK_Dept_Text
        {
            get
            { return this.GetValStringByKey(EmpAttr.FK_Dept);   }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// Ա��
        /// </summary>
        public Emp()
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

                Map map = new Map("Demo_Emp","Ա��");
                map.DepositaryOfEntity= Depositary.None;
                //�ֶ�ӳ��.
                map.AddTBStringPK(EmpAttr.No,null,"���",true,false,5,40,4);
                map.AddTBString(EmpAttr.Name, null, "name", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Tel, null, "�绰", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Email, null, "Email", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Addr, null, "Addr", true, false, 0, 200, 10);
                map.AddBoolean(EmpAttr.IsEnable, true, "�Ƿ�����", true, true);
                map.AddDDLSysEnum(EmpAttr.XB, 0, "�Ա�", true,true,"XB","@0=Ů@1=��");
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "����", new BP.Port.Depts(), true);
                map.AddTBInt(EmpAttr.Age, 18, "����", true, false);

                //��ѯ����
                map.AddSearchAttr(EmpAttr.XB);
                map.AddSearchAttr(EmpAttr.FK_Dept);

                //���в����ķ���ӳ��.
                RefMethod rm = new RefMethod();
                rm.Title = "����";
                rm.Warning = "��ȷ��Ҫִ�е�����";
                //������������.
                rm.HisAttrs.AddDDLEntities("FK_Dept", null, "Ҫ�������Ĳ���",  new BP.Port.Depts(), true);
                rm.HisAttrs.AddTBString("Note", null, "����ԭ��", true, false, 0, 1000, 100);
                rm.ClassMethodName = this.ToString() + ".DoMove";
                map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// ִ�е���
        /// </summary>
        /// <param name="fk_dept"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoMove(string fk_dept, string note)
        {
            //��д�����߼�.
            Dept dept = new Dept(fk_dept);
            return "�Ѿ��ɹ��İѸ���Ա������:"+dept.Name+" , ����ԭ����:"+note;
        }
        #endregion
    }
    /// <summary>
    /// Ա��s
    /// </summary>
    public class Emps : EntitiesNoName
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Emp();
            }
        }
        /// <summary>
        /// Ա��s
        /// </summary>
        public Emps() 
        {
        }
        /// <summary>
        /// ��ѯȫ��(������д�������)
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }
        #endregion
    }
}