using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: FullMoney.cs
   Author:�������￣˳
   Date:2014-05-18
   Description:��ֵ��
   ***********************************************************/
namespace BP.OA.Car
{
    /// <summary>
    /// ��ֵ������
    /// </summary>
    public enum FullMoneyCarCardType
    {
        /// <summary>
        /// ͣ����
        /// </summary>
        STOP,
        /// <summary>
        /// �Ϳ�
        /// </summary>
        OIL,
        /// <summary>
        /// ETC��ֵ��
        /// </summary>
        ETC,
    }
    /// <summary>
    /// ��ֵ̨�� ����
    /// </summary>
    public class FullMoneyAttr : EntityOIDAttr
    {
        /// <summary>
        /// ��ֵ��
        /// </summary>
        public const string JBR = "JBR";
        /// <summary>
        /// ��ֵʱ��
        /// </summary>
        public const string CZRQ = "CZRQ";
        /// <summary>
        /// ����
        /// </summary>
        public const string CardNo = "CardNo";
        /// <summary>
        /// ������λ
        /// </summary>
        public const string HZDW = "HZDW";
        /// <summary>
        /// ����ص�
        /// </summary>
        public const string BLDD = "BLDD";
        /// <summary>
        /// ��д����
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// ��ֵ��˾
        /// </summary>
        public const string WXGS = "WXGS";
        /// <summary>
        /// �����Ŀ�
        /// </summary>
        public const string FK_Card = "FK_Card";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// ����
        /// </summary>
        public const string CarCardType = "CarCardType";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// ��ֵ���
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// ��ֵǰ���
        /// </summary>
        public const string CZQJE = "CZQJE";
        /// <summary>
        /// ��ֵ������
        /// </summary>
        public const string CZGLS = "CZGLS";
    }
    /// <summary>
    ///  ��ֵ̨��
    /// </summary>
    public class FullMoney : EntityOID
    {
        #region ����
        public string FK_NY
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_NY); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_NY, value);
            }
        }
        public string FK_Car
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_Car); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_Car, value);
            }
        }
        public string FK_Card
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_Card); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_Card, value);
            }
        }
        public string CZRQ
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.CZRQ); }

            set
            {
                this.SetValByKey(FullMoneyAttr.CZRQ, value);
            }
        }
        public float JE
        {
            get
            { return this.GetValFloatByKey(FullMoneyAttr.JE); }

            set
            {
                this.SetValByKey(FullMoneyAttr.JE, value);
            }
        }
        public string JBR 
        {
            get { return this.GetValStringByKey(FullMoneyAttr.JBR); }
            set { this.SetValByKey(FullMoneyAttr.JBR,value); }
        }
        #endregion ����

        #region Ȩ�޿�������.
        #endregion Ȩ�޿�������.

        #region ���췽��
        /// <summary>
        /// ��ֵ̨��
        /// </summary>
        public FullMoney()
        {
        }
        /// <summary>
        /// ��ֵ̨��
        /// </summary>
        /// <param name="oid"></param>
        public FullMoney(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// ��ֵ̨��Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarFullMoney");
                map.EnDesc = "��ֵ̨��";
                map.CodeStruct = "9";

                map.DepositaryOfEntity = Depositary.None;

                //����OID�����ֶΡ�
                map.AddTBIntPKOID();

                //map.AddDDLEntities(FullMoneyAttr.FK_Card, null, "��", new Cards(),false);
                //map.AddDDLEntities(FullMoneyAttr.FK_NY, null, "��������", new BP.Pub.NYs(), false);

                map.AddDDLEntities(FullMoneyAttr.FK_Car, null, "����", new CarInfos(), true);
                map.AddDDLSysEnum(FullMoneyAttr.CarCardType, 0, "����", true, true, FullMoneyAttr.CarCardType,
                 "@0=ͣ����@1=�Ϳ�@2=Etc��");
                map.AddTBString(FullMoneyAttr.CardNo, null, "����", true, false, 2, 100, 30);
                map.AddTBString(FullMoneyAttr.HZDW, null, "������λ", true, false, 2, 100, 30);
                map.AddTBString(FullMoneyAttr.BLDD, null, "����ص�", true, false, 2, 100, 30);
                map.AddTBMoney(FullMoneyAttr.CZQJE, 0, "��ֵǰ���", true, false);
                map.AddTBMoney(FullMoneyAttr.JE, 0, "���ý�Ԫ��", true, false);

                map.AddTBDate(FullMoneyAttr.CZRQ, null, "��ֵ����", true, false);
                map.AddTBInt(FullMoneyAttr.CZGLS, 0, "��ֵ������", true, false);
                map.AddTBString(FullMoneyAttr.JBR, null, "������", true, false, 2, 100, 30, false);
                map.AddTBStringDoc(CarInfoAttr.Note, null, "��ע", true, false, true);

                //��ѯ����.
                //map.AddSearchAttr(FullMoneyAttr.FK_NY);
                //map.AddSearchAttr(FullMoneyAttr.FK_Card);
                map.AddSearchAttr(FullMoneyAttr.CarCardType);
                map.AddSearchAttr(FullMoneyAttr.FK_Car);
                this._enMap = map;
                return this._enMap;
            }
        }

        #region ��д����
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// �����Զ�����ά����.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_Car))
            {
                throw new Exception("���ƺŲ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.CZRQ))
            {
                throw new Exception("��ֵ���ڲ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.JE+""))
            {
                throw new Exception("���ò���Ϊ�գ�");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            // ���·��ü�¼
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_Car;
            feiyong.JingBanRen = this.JBR;
            feiyong.FeiYongJE = Decimal.Parse(this.JE+"");
            feiyong.FeiYongRiQi = this.CZRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.FullMoney;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion ��д����
    }
    /// <summary>
    /// ��ֵ̨��
    /// </summary>
    public class FullMoneys : BP.En.EntitiesOID
    {
        /// <summary>
        /// ��ֵ̨��s
        /// </summary>
        public FullMoneys() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FullMoney();
            }
        }
    }
}
