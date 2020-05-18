using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: Drivers.cs
   Author:�������￣˳
   Date:2014-05-18
   Description:˾����Ϣ������
   ***********************************************************/
namespace BP.OA.Car
{
    
    /// <summary>
    /// ˾������
    /// </summary>
    public class DriverAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ״̬
        /// </summary>
        public const string CarSta = "CarSta";
        /// <summary>
        /// ���εı��
        /// </summary>
        public const string TreeNo = "TreeNo";
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
        /// ˾������
        /// </summary>
       /// public const string SiJiMingCheng = "SiJiMingCheng";
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

        //============================================================
        /// <summary>
        /// ��ע
        /// </summary>
        public const string BZ = "BZ";
        /// <summary>
        /// ��֤������
        /// </summary>
        public const string JZDQR = "JZDQR";
        /// <summary>
        /// ��֤����
        /// </summary>
        public const string LZRQ = "LZRQ";
        /// <summary>
        /// ׼�ݳ���
        /// </summary>
        public const string ZJCX = "ZJCX";
        /// <summary>
        /// ��������
        /// </summary>
        public const string CCCS = "CCCS";
        /// <summary>
        /// Υ�´���
        /// </summary>
        public const string WZCS = "WZCS";
        /// <summary>
        /// �������ۼ�
        /// </summary>
        public const string FKZJE = "FKZJE";
    }
    /// <summary>
    ///  ˾��
    /// </summary>
    public class Driver : EntityNoName
    {
        #region ����
        /// <summary>
        /// ��������
        /// </summary>
        public Int64 CCCS {
            get {
                return this.GetValInt64ByKey(DriverAttr.CCCS);
            }
            set {
                this.SetValByKey(DriverAttr.CCCS,value);
            }
        }
        /// <summary>
        /// Υ�´���
        /// </summary>
        public Int64 WZCS {
            get {
                return this.GetValInt64ByKey(DriverAttr.WZCS);
            }
            set {
                this.SetValByKey(DriverAttr.WZCS,value);
            }
        }
        /// <summary>
        /// �����ܽ��
        /// </summary>
        public decimal FKZJE {
            get {
                return this.GetValDecimalByKey(DriverAttr.FKZJE);
            }
            set {
                this.SetValByKey(DriverAttr.FKZJE,value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        /// 
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(DriverAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// ������
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DriverAttr.FK_Emp, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.RDT);
            }
            set
            {
                this.SetValByKey(DriverAttr.RDT, value);
            }
        }
        public string EDT
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.EDT);
            }
            set
            {
                this.SetValByKey(DriverAttr.EDT, value);
            }
        }
        public string ZJCX
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.ZJCX);
            }
            set
            {
                this.SetValByKey(DriverAttr.ZJCX, value);
            }
        }
        public string LZRQ
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.LZRQ);
            }
            set
            {
                this.SetValByKey(DriverAttr.LZRQ, value);
            }
        }
        public string JZDQR
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.JZDQR);
            }
            set
            {
                this.SetValByKey(DriverAttr.JZDQR, value);
            }
        }

        #endregion ����

        #region Ȩ�޿�������.
        #endregion Ȩ�޿�������.

        #region ���췽��
        /// <summary>
        /// ˾��
        /// </summary>
        public Driver()
        {
        }
        /// <summary>
        /// ˾��
        /// </summary>
        /// <param name="_No"></param>
        public Driver(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// ˾��Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarDriver");
                map.EnDesc = "˾��";
                map.CodeStruct = "3";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(DriverAttr.No, null, "���", true, true, 1, 100, 100);
                map.AddTBString(DriverAttr.Name, null, "��ʻԱ����", true, false, 0, 100, 120, false);
                map.AddTBString(DriverAttr.ZJCX, null, "׼�ݳ���", true, false, 0, 100, 100, false);
                map.AddTBDate(DriverAttr.LZRQ, null, "��֤����", true, false);
                map.AddTBDate(DriverAttr.JZDQR, null, "��֤������", true, false);
                map.AddTBInt(DriverAttr.CCCS,0,"��������",true,true);
                map.AddTBInt(DriverAttr.WZCS,0,"Υ�´���",true,true);
                map.AddTBDecimal(DriverAttr.FKZJE, 0, "�������ۼ�", true, true);
                map.AddTBStringDoc(DriverAttr.BZ, null, "��ע", true, false,true);

                //map.AddDDLSysEnum(DriverAttr.CarSta, 0, "״̬", true, true, DriverAttr.CarSta,
                //    "@0=������@1=������@2=ά����");
                //map.AddTBString(DriverAttr.FK_Emp, null, "������", true, false, 0, 100, 30);
                //map.AddTBString(DriverAttr.FK_Dept, null, "����", true, false, 0, 100, 30);
                //map.AddTBInt(DriverAttr.Idx, 0, "Idx", false, false);
                //map.AddTBInt(DriverAttr.ReadTimes, 0, "�Ķ�����", false, false);
                //map.AddTBInt(DriverAttr.IsDownload, 0, "�Ƿ��������?", false, false);

                // ���ò�ѯ����

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "�����Ǽ�";
                rm.ClassMethodName = this.ToString() + ".DoChuCheHistory";                
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "Υ�µǼ�";
                rm.ClassMethodName = this.ToString() + ".DoWeiZhangHistory";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoChuCheHistory() 
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarRecords&FK_JiaShiYuan=" + this.No;
        }
        public string DoWeiZhangHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarWeiZhangs&FK_JiaShiYuan=" + this.No;
        }

        #region ��д����
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.Name))
                 throw new Exception("��ʻԱ��������Ϊ�գ�");
            
            if (string.IsNullOrEmpty(this.ZJCX))
                           throw new Exception("׼�ٳ��Ͳ���Ϊ�գ�");
            
            if (string.IsNullOrEmpty(this.LZRQ))
                            throw new Exception("��֤������������Ϊ�գ�");
            
            if (string.IsNullOrEmpty(this.JZDQR))
                            throw new Exception("��֤�����ղ���Ϊ�գ�");
            
            return base.beforeInsert();
        }

       
        #endregion ��д����
    }
    /// <summary>
    /// ˾��
    /// </summary>
    public class Drivers : BP.En.EntitiesNoName
    {
        /// <summary>
        /// ˾��s
        /// </summary>
        public Drivers() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Driver();
            }
        }
    }
}
