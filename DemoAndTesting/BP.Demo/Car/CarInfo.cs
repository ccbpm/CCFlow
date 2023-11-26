using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: 
   Author:    
   Date:
   Description:  
   ***********************************************************/
namespace BP.OA.Car
{
    public enum CarInfoType
    {
        /// <summary>
        /// ������
        /// </summary>
        XianZhiing,
        /// <summary>
        /// ������
        /// </summary>
        ChuCheing
    }
    /// <summary>
    /// ������Ϣ����
    /// </summary>
    public class CarInfoAttr : EntityNoNameAttr
    {

        /// <summary>
        /// ���ƺ�
        /// </summary>
        public const string FK_CPH = "FK_CPH";
        /// <summary>
        /// ״̬
        /// </summary>
        public const string CarSta = "CarSta";
        /// <summary>
        /// ˾��
        /// </summary>
        public const string Driver = "Driver";
        /// <summary>
        /// ����
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// �Ķ�����
        /// </summary>
        public const string ReadTimes = "ReadTimes";
        /// <summary>
        /// ������
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// ��������
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// ��������
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// �޸�����
        /// </summary>
        public const string EDT = "EDT";
        /// <summary>
        /// ˳���
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public const string IsDownload = "IsDownload";

        /// <summary>
        /// ������
        /// </summary>
        public const string GLS = "GLS";
        /// <summary>
        /// ά�޷�
        /// </summary>
        public const string FY_WSF = "FY_WSF";
        /// <summary>
        /// ����
        /// </summary>
        public const string FY_NianJian = "FY_NianJian";
        /// <summary>
        /// �´��������
        /// </summary>
        public const string NJRQ = "NJRQ";
        /// <summary>
        /// �´α�������
        /// </summary>
        public const string BYRQ = "BYRQ";
        /// <summary>
        /// ��ע 
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// ���ܺ�
        /// </summary>
        public const string CJH = "CJH";
        /// <summary>
        /// ���յ�����
        /// </summary>
        public const string BXDQR = "BXDQR";
        /// <summary>
        /// ��������
        /// </summary>
        public const string GCRQ = "GCRQ";

        public const string FY_Oil = "FY_Oil";
        public const string FY_Stop = "FY_Stop";
        public const string FY_Etc = "FY_Etc";

        //============================================================
        /// <summary>
        /// ����״̬
        /// </summary>
        public const string CLZT = "CLZT";
        /// <summary>
        /// ����
        /// </summary>
        public const string CX = "CX";
        /// <summary>
        /// ��������
        /// </summary>
        public const string FDJH = "FDJH";
        /// <summary>
        /// ������
        /// </summary>
        public const string ZRR = "ZRR";
        /// <summary>
        /// ר�ò���
        /// </summary>
        public const string ZYBM = "ZYBM";
        /// <summary>
        /// ר����
        /// </summary>
        public const string ZYR = "ZYR";
    }
    /// <summary>
    ///  ������Ϣ
    /// </summary>
    public class CarInfo : EntityNoName
    {
        #region ����
        public string BXDQR
        {
            get { return this.GetValStringByKey(CarInfoAttr.BXDQR); }
            set { this.SetValByKey(CarInfoAttr.BXDQR, value); }
        }
        public string ZRR
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZRR);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZRR, value);
            }
        }
        public string ZYR
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZYR);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZYR, value);
            }
        }
        public string ZYBM
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZYBM);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZYBM, value);
            }
        }
        public string GCRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.GCRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.GCRQ, value);
            }
        }
        public string BYRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.BYRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.BYRQ, value);
            }
        }
        public string NJRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.NJRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.NJRQ, value);
            }
        }
        public string FK_CPH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.FK_CPH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_CPH, value);
            }
        }
        public string FK_CPHText
        {
            get
            {
                return this.GetValRefTextByKey(CarInfoAttr.FK_CPH);
            }
        }
        public decimal FY_Oil
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Oil);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Oil, value);
            }
        }
        public decimal FY_Etc
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Etc);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Etc, value);
            }
        }
        public decimal FY_Stop
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Stop);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Stop, value);
            }
        }
        public string CX
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.CX);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.CX, value);
            }
        }
        public string CJH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.CJH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.CJH, value);
            }
        }
        public string FDJH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.FDJH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FDJH, value);
            }
        }
        #endregion ����

        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.Title);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.Title, value);
            }
        }
        /// <summary>
        /// �Ķ�����
        /// </summary>
        public int ReadTimes
        {
            get
            {
                return this.GetValIntByKey(CarInfoAttr.ReadTimes);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ReadTimes, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// ������
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_Emp, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.RDT);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.RDT, value);
            }
        }
        public string EDT
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.EDT);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.EDT, value);
            }
        }
        public decimal FY_WSF
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_WSF);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_WSF, value);
            }
        }
        public decimal FY_NianJian
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_NianJian);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_NianJian, value);
            }
        }
        #endregion ����

        #region Ȩ�޿�������.
        //public override UAC HisUAC
        //{
        //    get
        //    {
        //        UAC uac = new UAC();
        //        if (Web.WebUser.No == "admin")
        //        {
        //            uac.IsDelete = false;
        //        }
        //        return uac;
        //    }
        //}

        #endregion Ȩ�޿�������.

        #region ���췽��
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public CarInfo()
        {
        }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="_No"></param>
        public CarInfo(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// ������ϢMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarInfo");
                map.EnDesc = "������Ϣ";
                map.CodeStruct = "3";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(CarInfoAttr.No, null, "���", true, true, 3, 3, 3);
                map.AddTBString(CarInfoAttr.Name, null, "", false, true, 2, 100, 30);

                // ���ƺ�
                map.AddDDLEntities(CarInfoAttr.FK_CPH, null, "���ƺ�", new ZhiBiaos(), true);
                map.AddTBString(CarInfoAttr.CX, null, "����", true, false, 2, 100, 30, false);
                map.AddTBString(CarInfoAttr.CJH, null, "���ܺ�", true, false, 2, 100, 30, false);
                map.AddTBString(CarInfoAttr.FDJH, null, "��������", true, false, 2, 100, 30, false);

                map.AddTBFloat(CarInfoAttr.GLS, 0, "��ǰ���", true, false);
                map.AddTBDate(CarInfoAttr.NJRQ, null, "�´��������", true, true);//==============
                map.AddTBDate(CarInfoAttr.BYRQ, null, "�´α�������", true, true);//===============
                map.AddTBDate(CarInfoAttr.BXDQR, null, "���յ�����", true, true);
                map.AddTBDate(CarInfoAttr.GCRQ, null, "��������", true, false);

                //map.AddDDLEntities();
                map.AddTBString(CarInfoAttr.ZYBM, null, "ר�ò���", true, false, 2, 100, 30, false);//=====================
                map.AddTBString(CarInfoAttr.ZYR, null, "ר����", true, false, 2, 100, 30, false);
                //========================
                map.AddTBString(CarInfoAttr.ZRR, null, "������", true, false, 2, 100, 30, false);//========================
                map.AddDDLSysEnum(CarInfoAttr.CLZT, 0, "����״̬", true, false, CarInfoAttr.CLZT, "@0=������@1=������@2=ά����");
                map.AddTBStringDoc(CarInfoAttr.Note, null, "��ע", true, false, true);

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "���Ǽ�";
                rm.ClassMethodName = this.ToString() + ".DoNianJian";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��ʷ����¼";
                rm.ClassMethodName = this.ToString() + ".DoNJHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����Ǽ�";
                rm.ClassMethodName = this.ToString() + ".DoBaoYang";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "��ʷ������¼";
                rm.ClassMethodName = this.ToString() + ".DoBYHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ά�޵Ǽ�";
                rm.ClassMethodName = this.ToString() + ".DoWeiHu";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "��ʷά�޼�¼";
                rm.ClassMethodName = this.ToString() + ".DoWXHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�뱣�Ǽ�";
                rm.ClassMethodName = this.ToString() + ".DoRuBao";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "��ʷ�뱣��¼";
                rm.ClassMethodName = this.ToString() + ".DoRBHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�Ǽǳ���";
                rm.ClassMethodName = this.ToString() + ".DoChuBao";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "��ʷ������¼";
                rm.ClassMethodName = this.ToString() + ".DoCBHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��ֵ����";
                rm.ClassMethodName = this.ToString() + ".DoChongZhi";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "��ʷ��ֵ��¼";
                rm.ClassMethodName = this.ToString() + ".DoCZHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����ۺϲ�ѯ";
                rm.ClassMethodName = this.ToString() + ".DoFeiYongZongHe";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoFeiYongZongHe()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarFeiYongGuanLis&FK_Car=" + this.No;
        }
        /// <summary>
        /// ��ֵ
        /// </summary>
        /// <returns></returns>
        public string DoChongZhi()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.FullMoneys&FK_Car=" + this.No;
        }
        public string DoCZHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.FullMoneys&FK_Car=" + this.No;
        }
        /// <summary>
        /// �뱣
        /// </summary>
        /// <returns></returns>
        public string DoRuBao()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.CarRuBaos&FK_Car=" + this.No;
        }
        public string DoRBHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarRuBaos&FK_Car=" + this.No;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public string DoChuBao()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.CarChuBaos&FK_Car=" + this.No;
        }
        public string DoCBHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarChuBaos&FK_Car=" + this.No;
        }
        /// <summary>
        /// BaoYang
        /// </summary>
        /// <returns></returns>
        public string DoBaoYang()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.BaoYangs&FK_Car=" + this.No;
        }
        public string DoBYHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.BaoYangs&FK_Car=" + this.No;
        }
        /// <summary>
        /// ά��
        /// </summary>
        /// <returns></returns>
        public string DoWeiHu()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.WeiHus&FK_Car=" + this.No;
        }
        public string DoWXHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.WeiHus&FK_Car=" + this.No;
        }
        /// <summary>
        /// ���
        /// </summary>
        /// <returns></returns>
        public string DoNianJian()
        {
            //if (BP.OA.Car.API.Car_NianJian_FlowIsEnable ==true) /*��������.*/
            //    return "/WF/MyFlow.htm?FK_Flow="+API.Car_NianJian_FlowMark+"&FK_Car=" + this.No,"sss", 500, 600);
            //else
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.NianJians&FK_Car=" + this.No;
        }
        public string DoNJHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.NianJians&FK_Car=" + this.No;
        }
        #region ��д����
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_CPH))
            {
                throw new Exception("���ƺŲ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.CX))
            {
                throw new Exception("���Ͳ���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.FDJH))
            {
                throw new Exception("�������Ų���Ϊ�գ�");
            }
            if (string.IsNullOrEmpty(this.CJH))
            {
                throw new Exception("���ܺŲ���Ϊ�գ�");
            }
            // ����Name�복�ƺ���ͬ
            ZhiBiao zb = new ZhiBiao();
            zb.No = this.FK_CPH;
            zb.Retrieve();
            this.Name = zb.Name;
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            try
            {
                // �˴���Ҫ�޸�ָ��ʹ��״̬
                ZhiBiao temp = new ZhiBiao();
                temp.No = this.FK_CPH;
                temp.Retrieve();
                temp.ZBZT = (int)ZBZT.YiYong;
                temp.Update();
            }
            catch (Exception ex)
            {

            }
            base.afterInsertUpdateAction();
        }

        protected override bool beforeUpdate()
        {

            return base.beforeUpdate();
        }
        /// <summary>
        /// ͬ��ά����
        /// </summary>
        public static void DTS_WHF()
        {
            CarInfos infos = new CarInfos();
            infos.RetrieveAll();
            foreach (CarInfo en in infos)
            {
                string sql = "SELECT SUM(WSFY) FROM oa_carweihu  WHERE FK_Car='" + en.No + "'";
                decimal d = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                en.FY_WSF = d;
                en.Update();
            }
        }
        public static void DTS_NianJian()
        {
            CarInfos infos = new CarInfos();
            infos.RetrieveAll();
            foreach (CarInfo en in infos)
            {
                string sql = "SELECT SUM(JE) FROM OA_CarNianJian  WHERE FK_Car='" + en.No + "'";
                decimal d = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                en.FY_NianJian = d;
                en.Update();
            }
        }
        #endregion ��д����
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class CarInfos : EntitiesNoName
    {
        /// <summary>
        /// ������Ϣs
        /// </summary>
        public CarInfos() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarInfo();
            }
        }
    }
}
