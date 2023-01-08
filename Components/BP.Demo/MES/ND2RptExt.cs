using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;
using BP.DA;

namespace BP.MES
{
    /// <summary>
    /// 订单 Attr
    /// </summary>
    public class ND2RptExtAttr : NDXRptBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 箱体名称
        /// </summary>
        public const string XiangTiMingCheng = "XiangTiMingCheng";
        /// <summary>
        /// 订单号
        /// </summary>
        public const string DingDanHao = "DingDanHao";
        /// <summary>
        /// 项目名称
        /// </summary>
        public const string PrjName = "PrjName";
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
        /// 发货人
        /// </summary>
        public const string KeHuMingCheng = "KeHuMingCheng";

        #endregion
    }
    /// <summary>
    /// 订单
    /// </summary>
    public class ND2RptExt : NDXRptBase
    {
        #region 属性
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string TuZhiBianHao
        {
            get
            {
                return this.GetValStringByKey(ND2RptExtAttr.TuZhiBianHao);
            }
            set
            {
                this.SetValByKey(ND2RptExtAttr.TuZhiBianHao, value);
            }
        }
        /// <summary>
        /// 请假人编号
        /// </summary>
        public string TuZhiZhiTuRen
        {
            get
            {
                return this.GetValStringByKey(ND2RptExtAttr.TuZhiZhiTuRen);
            }
            set
            {
                this.SetValByKey(ND2RptExtAttr.TuZhiZhiTuRen, value);
            }
        }
        /// <summary>
        /// 请假人名称
        /// </summary>
        //public string ZhuangPeiFuZeRen
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(ND2RptExtAttr.ZhuangPeiFuZeRen);
        //    }
        //    set
        //    {
        //        this.SetValByKey(ND2RptExtAttr.ZhuangPeiFuZeRen, value);
        //    }
        //}
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public int JJCD
        {
            get
            {
                return this.GetValIntByKey(ND2RptExtAttr.JJCD);
            }
            set
            {
                this.SetValByKey(ND2RptExtAttr.JJCD, value);
            }
        }

        /// <summary>
        /// 总经理意见
        /// </summary>
        public string JJCDText
        {
            get
            {
                return this.GetValRefTextByKey(ND2RptExtAttr.JJCD);
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
        public ND2RptExt()
        {

        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND2RptExt(Int64 workid)
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

                Map map = new Map("ND2Rpt", "订单信息");

                #region 流程的基本字段
                map.AddTBIntPKOID();
                map.AddTBString(ND2RptExtAttr.Title, null, "标题", true, true, 0, 500, 10);
                map.AddTBString(ND2RptExtAttr.BillNo, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(ND2RptExtAttr.KeHuMingCheng, null, "客户名称", false, true, 0, 50, 10);
                map.AddTBString(ND2RptExtAttr.DingDanHao, null, "订单号", true, true, 0, 50, 10);
                map.AddTBString(ND2RptExtAttr.PrjName, null, "项目名称", true, true, 0, 50, 10);
                map.AddTBString(ND2RptExtAttr.QiXianXianDing, null, "期限限定", true, true, 0, 50, 10);
                map.AddDDLSysEnum(ND2RptExtAttr.JJCD, 0, "紧急程度", true, true, ND2RptExtAttr.JJCD, "@0=低，@1=中，@2=高");
                map.AddTBInt(ND2RptExtAttr.FlowEndNode, 0, "订单状态(运行的节点)", false, true);

                #endregion 流程的基本字段


                //箱体信息.
                map.AddDtl(new ND201Dtl1s(), ND201Dtl1Attr.RefPK);



                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 订单s
    /// </summary>
    public class ND2RptExts : NDXRptBases
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND2RptExt();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND2RptExts() { }
        #endregion
    }
}
