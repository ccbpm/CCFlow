using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
    /// <summary>
    /// ʡ��
    /// </summary>
    public class SFAttr : EntityNoNameAttr
    {
        /// <summary>
        /// Ƭ��
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// ����
        /// </summary>
        public const string Names = "Names";
        /// <summary>
        /// ����
        /// </summary>
        public const string JC = "JC";
    }
    /// <summary>
    /// ʡ��
    /// </summary>
    public class SF : EntityNoName
    {
        #region ��������
        /// <summary>
        /// Ƭ�����
        /// </summary>
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(SFAttr.FK_PQ);
            }
            set
            {
                this.SetValByKey(SFAttr.FK_PQ, value);
            }
        }
        /// <summary>
        /// Ƭ������
        /// </summary>
        public string FK_PQT
        {
            get
            {
                return this.GetValRefTextByKey(SFAttr.FK_PQ);
            }
        }
        /// <summary>
        /// С����
        /// </summary>
        public string Names
        {
            get
            {
                return this.GetValStrByKey(SFAttr.Names);
            }
            set
            {
                this.SetValByKey(SFAttr.Names, value);
            }
        }
        /// <summary>
        /// ���
        /// </summary>
        public string JC
        {
            get
            {
                return this.GetValStrByKey(SFAttr.JC);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ����Ȩ��.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// ʡ��
        /// </summary>		
        public SF() { }
        /// <summary>
        /// ʡ��
        /// </summary>
        /// <param name="no"></param>
        public SF(string no)
            : base(no)
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
                Map map = new Map( "CN_SF", "ʡ��");

                #region ��������
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.IsCheckNoLength = false;
                map.Java_SetEnType(EnType.App);
                map.Java_SetCodeStruct("4");
                #endregion

                #region �ֶ�
                map.AddTBStringPK(SFAttr.No, null, "���", true, false, 2, 2, 2);
                map.AddTBString(SFAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(SFAttr.Names, null, "С����", true, false, 0, 50, 200);
                map.AddTBString(SFAttr.JC, null, "���", true, false, 0, 50, 200);
                map.AddDDLEntities(SFAttr.FK_PQ, null, "Ƭ��", new PQs(), true);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ʡ��s
    /// </summary>
    public class SFs : EntitiesNoName
    {
        #region ʡ��.
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SF();
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ʡ��s
        /// </summary>
        public SFs() { }
        #endregion
    }
}
