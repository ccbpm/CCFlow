using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: NianJian.cs
   Author:�������￣˳
   Date:2014-05-18
   Description:���������
   ***********************************************************/
namespace BP.OA.Car
{
    /// <summary>
    /// ���̨�� ����
    /// </summary>
    public class NianJianAttr : EntityOIDAttr
    {
        /// <summary>
        /// �����
        /// </summary>
        public const string NJR = "NJR";
        /// <summary>
        /// ���ʱ��
        /// </summary>
        public const string NJRQ = "NJRQ";
        /// <summary>
        /// ������
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// �´�����
        /// </summary>
        public const string NextDT = "NextDT";
        /// <summary>
        /// che 
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// 
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// ������
        /// </summary>
        public const string ZRR = "ZRR";
        /// <summary>
        /// ��ע
        /// </summary>
        public const string BZ = "BZ";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";

    }
    /// <summary>
    ///  ���̨��
    /// </summary>
    public class NianJian : EntityOID
    {
        #region ����
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(NianJianAttr.WorkID);
            }
            set
            {
                this.SetValByKey(NianJianAttr.WorkID, value);
            }
        }
        public string NextDT
        {
            get {
                return this.GetValStringByKey(NianJianAttr.NextDT);
            }
            set {
                this.SetValByKey(NianJianAttr.NextDT, value);
            }
        }
        public string FK_Car
        {
            get
            { return this.GetValStrByKey(NianJianAttr.FK_Car); }

            set
            {
                this.SetValByKey(NianJianAttr.FK_Car, value);
            }
        }
        public string FK_CarText 
        {
            get 
            {
                return this.GetValRefTextByKey(NianJianAttr.FK_Car);
            }
        }
        public string NJR
        {
            get
            { return this.GetValStrByKey(NianJianAttr.NJR); }

            set
            {
                this.SetValByKey(NianJianAttr.NJR, value);
            }
        }
        public string ZRR
        {
            get
            { return this.GetValStrByKey(NianJianAttr.ZRR); }

            set
            {
                this.SetValByKey(NianJianAttr.ZRR, value);
            }
        }
        public string BZ
        {
            get
            { return this.GetValStrByKey(NianJianAttr.BZ); }

            set
            {
                this.SetValByKey(NianJianAttr.BZ, value);
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        public string NJRQ
        {
            get
            {
                return this.GetValStrByKey(NianJianAttr.NJRQ); 
            }
            set
            {
                this.SetValByKey(NianJianAttr.NJRQ, value);
            }
        }
        public decimal JE
        {
            get
            { return this.GetValDecimalByKey(NianJianAttr.JE); }

            set
            {
                this.SetValByKey(NianJianAttr.JE, value);
            }
        }
        #endregion ����

        #region Ȩ�޿�������.
        #endregion Ȩ�޿�������.

        #region ���췽��
        /// <summary>
        /// ���̨��
        /// </summary>
        public NianJian()
        {
        }
        /// <summary>
        /// ���̨��
        /// </summary>
        /// <param name="oid"></param>
        public NianJian(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// ���̨��Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarNianJian");
                map.EnDesc = "������";
                map.CodeStruct = "9";
                map.IsAutoGenerNo = true;

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.None;
                //����OID�����ֶΡ�
                map.AddTBIntPKOID();

                map.AddDDLEntities(NianJianAttr.FK_Car, null, "����", new CarInfos(),true);
                map.AddTBMoney(NianJianAttr.JE, 0, "������(�����)", true, false);
                map.AddTBDate(NianJianAttr.NJRQ, null, "�������", true, false);
                map.AddTBDate(NianJianAttr.NextDT, null, "�´��������", true, false);
                map.AddTBString(NianJianAttr.NJR, null, "������", true, false, 2, 100, 30, false);
                map.AddTBString(NianJianAttr.ZRR, null, "������", true, false, 2, 100, 30, false);

                map.AddTBStringDoc(NianJianAttr.BZ,null,"��ע",true,false,true);
                map.AddTBInt(NianJianAttr.WorkID, 0, "�´���WorkID������", false, false);

                //��ѯ����.
                map.AddSearchAttr(NianJianAttr.FK_Car);

                //RefMethod rm = new RefMethod();
                //rm = new RefMethod();
                //rm.Title = "������̼�¼";
                //rm.ClassMethodName = this.ToString() + ".DoNianJian";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        //public string DoNianJian()
        //{
        //    //return "/WF/WFRpt.htm?FK_Flow=" + API.Car_NianJian_FlowMark + "&WorkID=" + this.WorkID, "sd", "s", 500, 600);
        //    return null;
        //}

        #region ��д����
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// �����Զ������´��������.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_Car))
            {
                throw new Exception("���ƺŲ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.NJR))
            {
                throw new Exception("�����˲���Ϊ�գ�");
            } 
            if (string.IsNullOrEmpty(this.JE+""))
            {
                throw new Exception("�����ƺŲ���Ϊ�գ�");
            } 
            // ͬ�����³������´��������
            CarInfo carInfo = new CarInfo();
            
            carInfo.No = this.FK_Car;
            carInfo.Retrieve();// �����ݿ�ȡ������
            carInfo.NJRQ = this.NextDT;
            carInfo.Update();
            CarInfos carInfos = new CarInfos();
            return base.beforeUpdateInsertAction();//��챾���Լ�������߸���
        }
        protected override void afterInsertUpdateAction()
        {
            // ���·��ü�¼
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_Car;
            feiyong.JingBanRen = this.NJR;
            feiyong.FeiYongJE = this.JE;
            feiyong.FeiYongRiQi = this.NJRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.NianJian;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion ��д����
    }
    /// <summary>
    /// ���̨��
    /// </summary>
    public class NianJians : BP.En.EntitiesOID
    {
        /// <summary>
        /// ���̨��s
        /// </summary>
        public NianJians() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NianJian();
            }
        }
    }
}
