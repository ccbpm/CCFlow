using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo.SDK
{
    /// <summary>
    /// ��� ����
    /// </summary>
    public class QingJiaAttr
    {
        #region ��������
        /// <summary>
        /// ����˱��
        /// </summary>
        public const string QingJiaRenNo = "QingJiaRenNo";
        /// <summary>
        /// ���������
        /// </summary>
        public const string QingJiaRenName = "QingJiaRenName";
        /// <summary>
        /// ���ű��
        /// </summary>
        public const string QingJiaRenDeptNo = "QingJiaRenDeptNo";
        /// <summary>
        /// ��������
        /// </summary>
        public const string QingJiaRenDeptName = "QingJiaRenDeptName";
        /// <summary>
        /// �������
        /// </summary>
        public const string QingJiaTianShu = "QingJiaTianShu";
        /// <summary>
        /// ���ԭ��
        /// </summary>
        public const string QingJiaYuanYin = "QingJiaYuanYin";
        #endregion

        #region �������.
        public const string NoteBM = "NoteBM";
        public const string NoteZJL = "NoteZJL";
        public const string NoteRL = "NoteRL";
        #endregion �������.
    }
    /// <summary>
    /// ���
    /// </summary>
    public class QingJia : EntityOID
    {
        #region ����
        /// <summary>
        /// ����˲�������
        /// </summary>
        public string QingJiaRenDeptName
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenDeptName);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenDeptName, value);
            }
        }
        /// <summary>
        /// ����˱��
        /// </summary>
        public string QingJiaRenNo
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenNo);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenNo, value);
            }
        }
        /// <summary>
        /// ���������
        /// </summary>
        public string QingJiaRenName
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenName);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenName, value);
            }
        }
        /// <summary>
        /// ����˲��ű��
        /// </summary>
        public string QingJiaRenDeptNo
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenDeptNo);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenDeptNo, value);
            }
        }
        /// <summary>
        /// ���ԭ��
        /// </summary>
        public string QingJiaYuanYin
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaYuanYin);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaYuanYin, value);
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        public float QingJiaTianShu
        {
            get
            {
                return this.GetValIntByKey(QingJiaAttr.QingJiaTianShu);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaTianShu, value);
            }
        }
        /// <summary>
        /// �����������
        /// </summary>
        public string NoteBM
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteBM);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteBM, value);
            }
        }
        /// <summary>
        /// �ܾ������
        /// </summary>
        public string NoteZJL
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteZJL);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteZJL, value);
            }
        }
        /// <summary>
        /// ������Դ���
        /// </summary>
        public string NoteRL
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteRL);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteRL, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ���
        /// </summary>
        public QingJia()
        {
        }
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="oid">ʵ����</param>
        public QingJia(int oid):base(oid)
        {
        }
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="oid">ʵ����</param>
        public QingJia(Int64 oid)
        {
            this.OID = (int)oid;
            this.Retrieve();
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

                Map map = new Map("Demo_QingJia", "���");

                map.AddTBIntPKOID();
                map.AddTBString(QingJiaAttr.QingJiaRenNo, null, "����˱��", false, false, 0, 200, 10);
                map.AddTBString(QingJiaAttr.QingJiaRenName, null, "���������", true, false, 0, 200, 70);
                map.AddTBString(QingJiaAttr.QingJiaRenDeptNo, "", "����˲��ű��", true, false, 0, 200, 50);
                map.AddTBString(QingJiaAttr.QingJiaRenDeptName, null, "����˲�������", true, false, 0, 200, 50);
                map.AddTBString(QingJiaAttr.QingJiaYuanYin, null, "���ԭ��", true, false, 0, 200, 150);
                map.AddTBFloat(QingJiaAttr.QingJiaTianShu, 0, "�������", true, false);

                // �����Ϣ.
                map.AddTBString(QingJiaAttr.NoteBM, null, "���ž������", true, false, 0, 200, 150);
                map.AddTBString(QingJiaAttr.NoteZJL, null, "�ܾ������", true, false, 0, 200, 150);
                map.AddTBString(QingJiaAttr.NoteRL, null, "������Դ���", true, false, 0, 200, 150);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ���s
    /// </summary>
    public class QingJias : EntitiesOID
    {
        #region ����
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new QingJia();
            }
        }
        /// <summary>
        /// ���s
        /// </summary>
        public QingJias() { }
        #endregion
    }
}
