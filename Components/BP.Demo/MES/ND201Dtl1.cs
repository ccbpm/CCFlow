using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;
using ThoughtWorks.QRCode.Codec;
using System.Drawing.Imaging;
using System.Drawing;
using BP.Sys;

namespace BP.MES
{
    /// <summary>
    /// 工单 Attr
    /// </summary>
    public class ND201Dtl1Attr : EntityOIDAttr
    {
        #region 基本属性
        public const string RefPK = "RefPK";

        /// <summary>
        /// 箱体名称
        /// </summary>
        public const string XiangTiMingCheng = "XiangTiMingCheng";
        /// <summary>
        /// 期限限定
        /// </summary>
        public const string QiXianXianDing = "QiXianXianDing";
        /// <summary>
        /// 紧急程度
        /// </summary>
        public const string JJCD = "JJCD";
        public const string JJCDText = "JJCDText";

        /// <summary>
        /// 图纸编号
        /// </summary>
        public const string TuZhiBianHao = "TuZhiBianHao";
        /// <summary>
        /// 图纸制图人
        /// </summary>
        public const string TuZhiZhiTuRen = "TuZhiZhiTuRen";
        /// <summary>
        public const string FaHuoBill = "FaHuoBill";
        public const string FaHuoRen = "FaHuoRen";
        public const string FaHuoRQ = "FaHuoRQ";

        /// <summary>
        /// 0=未处理. 1=已完成. 2=检查合格. 3=打包. 4=发货.
        /// </summary>
        public const string XTSta = "XTSta";

        public const string XTStaText = "XTStaText";
        public const string FaHuoBillOID = "FaHuoBillOID";
        public const string PrjName = "PrjName";
        public const string KeHuMingCheng = "KeHuMingCheng";

        #endregion


        #region 订单信息.
        public const string DingDanHao = "DingDanHao";
        #endregion



    }
    /// <summary>
    /// 工单
    /// </summary>
    public class ND201Dtl1 : EntityOID
    {
        #region 属性
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.RefPK);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.RefPK, value);
            }
        }
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string TuZhiBianHao
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.TuZhiBianHao);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.TuZhiBianHao, value);
            }
        }

        public string PrjName
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.PrjName);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.PrjName, value);
            }
        }
        public string KeHuMingCheng
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.KeHuMingCheng);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.KeHuMingCheng, value);
            }
        }

        /// <summary>
        /// 请假人编号
        /// </summary>
        public string TuZhiZhiTuRen
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.TuZhiZhiTuRen);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.TuZhiZhiTuRen, value);
            }
        }
        public string FaHuoRQ
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.FaHuoRQ);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.FaHuoRQ, value);
            }
        }

        public string FaHuoBill
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.FaHuoBill);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.FaHuoBill, value);
            }
        }

        public string FaHuoRen
        {
            get
            {
                return this.GetValStringByKey(ND201Dtl1Attr.FaHuoRen);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.FaHuoRen, value);
            }
        }
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public int JJCD
        {
            get
            {
                return this.GetValIntByKey(ND201Dtl1Attr.JJCD);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.JJCD, value);
            }
        }

        public int XTSta
        {
            get
            {
                return this.GetValIntByKey(ND201Dtl1Attr.XTSta);
            }
            set
            {
                this.SetValByKey(ND201Dtl1Attr.XTSta, value);
            }
        }
        public string XTStaText
        {

            get
            {
                return this.GetValRefTextByKey(ND201Dtl1Attr.XTSta);
            }
        }

        /// <summary>
        /// 人力资源意见
        /// </summary>

        #endregion

        #region 构造函数
        /// <summary>
        /// 请假
        /// </summary>
        public ND201Dtl1()
        {

        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND201Dtl1(int workid)
        {
            this.OID = workid;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ND201Dtl1", "箱体信息");

                #region 基本信息
                map.AddTBIntPKOID();
                map.AddTBString(ND201Dtl1Attr.RefPK, null, "主键", false, true, 0, 500, 10);

                map.AddTBString(ND201Dtl1Attr.XiangTiMingCheng, null, "箱体名称", true, true, 0, 50, 10);
                map.AddDDLSysEnum(ND201Dtl1Attr.XTSta, 0, "状态", true, true, ND201Dtl1Attr.XTSta, "@0=未处理@1=安装完成@2=检查合格@3=打包@4=发货");
                map.AddTBString(ND201Dtl1Attr.TuZhiBianHao, null, "图纸编号", true, true, 0, 50, 10);
                map.AddTBString(ND201Dtl1Attr.TuZhiZhiTuRen, null, "图纸制图人", true, true, 0, 50, 10);

                map.AddTBString(ND201Dtl1Attr.QiXianXianDing, null, "期限限定", true, true, 0, 50, 10);
                map.AddDDLSysEnum(ND201Dtl1Attr.JJCD, 0, "紧急程度", true, true, ND201Dtl1Attr.JJCD, "@0=低@1=中@2=高");

                map.AddTBString(ND201Dtl1Attr.FaHuoBill, null, "发货单号", true, true, 0, 50, 10);
                map.AddTBString(ND201Dtl1Attr.FaHuoRen, null, "发货人", true, true, 0, 50, 10);
                map.AddTBInt(ND201Dtl1Attr.FaHuoBillOID, 0, "发货单OID", true, true);

                map.AddTBDate(ND201Dtl1Attr.FaHuoRQ, "发货日期", true, true);
                #endregion 基本信息

                #region 订单信息.
                map.AddTBString(ND201Dtl1Attr.DingDanHao, null, "订单号", true, true, 0, 50, 10);
                map.AddTBString(ND201Dtl1Attr.PrjName, null, "项目名称", true, true, 0, 50, 10);
                map.AddTBString(ND201Dtl1Attr.KeHuMingCheng, null, "客户名称", false, true, 0, 50, 10);
                #endregion 订单信息

                map.AddSearchAttr(ND201Dtl1Attr.XTSta);

                RefMethod rm = new RefMethod();
                rm.Title = "打印二维码";
                rm.ClassMethodName = this.ToString() + ".CreateCode()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //装料信息.
                map.AddDtl(new ND201Dtl1Dtl1s(), ND201Dtl1Attr.RefPK);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 打印二维码
        /// </summary>
        /// <returns></returns>
        public string CreateCode()
        {
            return "/App/MES/PrintQRCode.htm?WorkIDs=" + this.OID;
        }

        public string DoFaHuo(string fhd, string fhr, string fhrq)
        {

            this.FaHuoBill = fhd;
            this.FaHuoRen = fhr;
            this.FaHuoRQ = fhrq;

            this.Update();

            return "设置发货单号：{" + this.FaHuoBill + "}成功";
        }
    }
    /// <summary>
    /// 工单s
    /// </summary>
    public class ND201Dtl1s : EntitiesOID
    {


        #region 二维码.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CreateBoxQRCode(string workIDStr)
        {
            string url = "http://81.69.38.157/App/MES/BoxInfo.htm?BoxID=" + workIDStr;

            // string url = SystemConfig.HostURL + "/WF/CCBill/OptComponents/QRCode.htm?DoType=MyDict&WorkID=" + workIDStr + "&FrmID=" + this.FrmID + "&MethodNo=" + this.GetRequestVal("MethodNo");
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = 4; //大小(值越大生成的二维码图片像素越高).
            encoder.QRCodeVersion = 0; //版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;//错误效验、错误更正(有4个等级)
            encoder.QRCodeBackgroundColor = Color.White;
            encoder.QRCodeForegroundColor = Color.Black;

            //生成临时文件.
            System.Drawing.Image image = encoder.Encode(url, Encoding.UTF8);
            string tempPath = SystemConfig.PathOfTemp + "/" + workIDStr + ".png";
            image.Save(tempPath, ImageFormat.Png);
            image.Dispose();

            //返回url.
            return url;
        }
        /// <summary>
        /// 扫描要做的工作
        /// </summary>
        /// <returns></returns>
        public string BoxInfo_Init(string workID)
        {
            return "";
            //string url = SystemConfig.HostURL + "/WF/CCBill/OptComponents/QRCodeScan.htm?DoType=MyDict&WorkID=" + workID + "&FrmID=" + this.FrmID + "&MethodNo=" + this.GetRequestVal("MethodNo");
            //QRCodeEncoder encoder = new QRCodeEncoder();
            //encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            //encoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            //encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            //encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;//错误效验、错误更正(有4个等级)
            //encoder.QRCodeBackgroundColor = Color.White;
            //encoder.QRCodeForegroundColor = Color.Black;

            ////生成临时文件.
            //System.Drawing.Image image = encoder.Encode(url, Encoding.UTF8);
            //string tempPath = SystemConfig.PathOfTemp + "/" + this.WorkID + ".png";
            //image.Save(tempPath, ImageFormat.Png);
            //image.Dispose();

            ////返回url.
            //return url;
        }
        #endregion 二维码.

        /// <summary>
        /// 打印二维码
        /// </summary>
        /// <returns></returns>
        public string PrintQRCode_Init(string workIDs)
        {
            string[] strs = workIDs.Split(',');
            foreach (var str in strs)
            {
                CreateBoxQRCode(str);
            }
            return "生成成功.";
        }


        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND201Dtl1();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND201Dtl1s() { }
        #endregion
    }
}
