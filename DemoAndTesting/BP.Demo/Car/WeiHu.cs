using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: WeiHu.cs
   Author:�������￣˳
   Date:2014-05-18
   Description:����ά����
   ***********************************************************/
namespace BP.OA.Car
{
    
    /// <summary>
    /// ά��̨�� ����
    /// </summary>
    public class WeiHuAttr : EntityOIDAttr
    {
        /// <summary>
        /// ά����
        /// </summary>
        public const string WXR = "WXR";
        /// <summary>
        /// ά��ʱ��
        /// </summary>
        public const string WXRQ = "WXRQ";
        /// <summary>
        /// ά�޷���
        /// </summary>
        public const string WSFY = "WSFY";
        /// <summary>
        /// ��д����
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// ά�޹�˾
        /// </summary>
        public const string WXGS = "WXGS";
        /// <summary>
        /// �����ĳ���
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_NY = "FK_NY";
    }
    /// <summary>
    ///  ά��̨��
    /// </summary>
    public class WeiHu : EntityOID
    {
        #region ����
        public string WXR
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.WXR); }

            set
            {
                this.SetValByKey(WeiHuAttr.WXR, value);
            }
        }
         public string WXRQ
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.WXRQ); }

            set
            {
                this.SetValByKey(WeiHuAttr.WXRQ, value);
            }
        }
         public string FK_NY
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.FK_NY); }

            set
            {
                this.SetValByKey(WeiHuAttr.FK_NY, value);
            }
        }
         public string FK_Car 
         {
             get { return this.GetValStringByKey(WeiHuAttr.FK_Car); }
             set { this.SetValRefTextByKey(WeiHuAttr.FK_Car,value); }
         }
        public decimal WSFY
        {
            get
            { return this.GetValDecimalByKey(WeiHuAttr.WSFY); }

            set
            {
                this.SetValByKey(WeiHuAttr.WSFY, value);
            }
        }
        #endregion ����

        #region Ȩ�޿�������.
        #endregion Ȩ�޿�������.

        #region ���췽��
        /// <summary>
        /// ά��̨��
        /// </summary>
        public WeiHu()
        {
        }
        /// <summary>
        /// ά��̨��
        /// </summary>
        /// <param name="oid"></param>
        public WeiHu(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// ά��̨��Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarWeiXiu");
                map.EnDesc = "ά�޹���";
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.None;

                //����OID�����ֶΡ�
                map.AddTBIntPKOID();

                map.AddDDLEntities(WeiHuAttr.FK_Car, null, "����", new CarInfos(),true);
                map.AddTBString(WeiHuAttr.WXR, null, "ά����", true, false, 2, 100, 30, false);
                map.AddTBDate(WeiHuAttr.WXRQ, null, "ά������", true, false);
                map.AddTBMoney(WeiHuAttr.WSFY, 0, "ά�޷���(�����)", true, false);
                map.AddTBString(WeiHuAttr.WXGS, null, "ά�޹�˾", true, false, 2, 100, 30, true);
                map.AddTBStringDoc(CarInfoAttr.Note, null, "��ע", true, false, true);

                //map.AddDDLEntities(WeiHuAttr.FK_NY, null, "��������", new BP.Pub.NYs(), false);

                //��ѯ����.
                map.AddSearchAttr(WeiHuAttr.FK_Car);
                //map.AddSearchAttr(WeiHuAttr.FK_NY);


                this._enMap = map;
                return this._enMap;
            }
        }

        #region ��д����
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
            if (string.IsNullOrEmpty(this.WXRQ))
            {
                throw new Exception("ά�����ڲ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.WSFY+""))
            {
                throw new Exception("ά�޷��ò���Ϊ�գ�");
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
            feiyong.JingBanRen = this.WXR;
            feiyong.FeiYongJE = this.WSFY;
            feiyong.FeiYongRiQi = this.WXRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.WeiXiu;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion ��д����
    }
    /// <summary>
    /// ά��̨��
    /// </summary>
    public class WeiHus : BP.En.EntitiesOID
    {
        /// <summary>
        /// ά��̨��s
        /// </summary>
        public WeiHus() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WeiHu();
            }
        }
    }
}
