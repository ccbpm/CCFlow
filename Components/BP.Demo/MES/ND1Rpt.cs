using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;


namespace BP.MES
{
    /// <summary>
    /// 箱体订单 Attr
    /// </summary>
    public class ND1RptAttr : NDXRptBaseAttr
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
        /// 装配负责人
        /// </summary>
        public const string ZhuangPeiFuZeRen = "ZhuangPeiFuZeRen";
        /// <summary>
        /// 检测负责人
        /// </summary>
        public const string JianCeFuZeRen = "JianCeFuZeRen";
        /// <summary>
        /// 封装人
        /// </summary>
        public const string FengZhuangRen = "FengZhuangRen";
        /// <summary>
        /// 客户信息
        /// </summary>
        public const string KeHuMingCheng = "KeHuMingCheng";
        #endregion
    }
    /// <summary>
    /// 箱体订单
    /// </summary>
    public class ND1Rpt : NDXRptBase
    {
        #region 属性
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string TuZhiBianHao
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.TuZhiBianHao);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.TuZhiBianHao, value);
            }
        }

        public string DingDanHao
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.DingDanHao);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.TuZhiBianHao, DingDanHao);
            }
        }


        public string PrjName
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.PrjName);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.PrjName, value);
            }
        }
        public string XiangTiMingCheng
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.XiangTiMingCheng);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.XiangTiMingCheng, value);
            }
        }

        /// <summary>
        /// 请假人编号
        /// </summary>
        public string TuZhiZhiTuRen
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.TuZhiZhiTuRen);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.TuZhiZhiTuRen, value);
            }
        }
        /// <summary>
        /// 请假人名称
        /// </summary>
        public string ZhuangPeiFuZeRen
        {
            get
            {
                return this.GetValStringByKey(ND1RptAttr.ZhuangPeiFuZeRen);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.ZhuangPeiFuZeRen, value);
            }
        }
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public int JJCD
        {
            get
            {
                return this.GetValIntByKey(ND1RptAttr.JJCD);
            }
            set
            {
                this.SetValByKey(ND1RptAttr.JJCD, value);
            }
        }

        /// <summary>
        /// 总经理意见
        /// </summary>
        public string JJCDText
        {
            get
            {
                return this.GetValRefTextByKey(ND1RptAttr.JJCD);
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
        public ND1Rpt()
        {

        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND1Rpt(Int64 workid)
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

                Map map = new Map("ND1Rpt", "箱体订单");

                #region 流程的基本字段
                map.AddTBIntPKOID();
                map.AddTBString(ND1RptAttr.Title, null, "标题", false, true, 0, 500, 10);
                map.AddTBString(ND1RptAttr.BillNo, null, "编号", false, true, 0, 50, 10);

                //  map.AddDDLSysEnum(ND1RptAttr.WFState, 0, "状态", false, true, "WFState");
                //map.AddTBInt(ND1RptAttr.FID, 0, "FID", false, true);
                //map.AddTBInt(ND1RptAttr.FlowDaySpan, 0, "跨度", false, true);
                //  map.AddTBString(ND1RptAttr.KeHuMingCheng, null, "客户名称", false, true, 0, 50, 10);

                map.AddTBString(ND1RptAttr.DingDanHao, null, "订单号", true, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.PrjName, null, "项目名称", true, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.KeHuMingCheng, null, "客户名称", false, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.QiXianXianDing, null, "期限限定", true, true, 0, 50, 10);
                map.AddDDLSysEnum(ND1RptAttr.JJCD, 0, "紧急程度", true, true, ND1RptAttr.JJCD, "@0=低@1=中@2=高");

                map.AddTBString(ND1RptAttr.TuZhiBianHao, null, "图纸编号", true, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.TuZhiZhiTuRen, null, "图纸制图人", true, true, 0, 50, 10);

                map.AddTBString(ND1RptAttr.XiangTiMingCheng, null, "箱体名称", true, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.ZhuangPeiFuZeRen, null, "装配负责人", true, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.JianCeFuZeRen, null, "检测负责人", true, true, 0, 50, 10);

                //map.AddTBString(ND1RptAttr.FengZhuangRen, null, "封装人", true, true, 0, 50, 10);
                map.AddTBInt(ND1RptAttr.PWorkID, 0, "父流程ID", true, true);
                // map.AddTBString(ND1RptAttr.FaHuoRen, null, "发货人", false, true, 0, 100, 10);
                #endregion 流程的基本字段

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 检查是否可以发送.
        /// </summary>
        /// <returns></returns>
        public string Work_CheckIsOverAndSend()
        {
            BP.WF.GenerWorkFlow gwf = new WF.GenerWorkFlow(this.OID);
            if (gwf.FK_Node != 102)
                return "";

            //获得重做与未完成的数据量： ，如果是0，2则让其发送下去.  0=未检查, 2=重做的. 
            string sql = "SELECT  count(OID) as Num FROM ND101Dtl1 WHERE RefPK = '" + this.OID + "' AND WorkSta in(0,2) ";
            var val = BP.DA.DBAccess.RunSQLReturnValInt(sql);
            if (val != 0)
                return "";

            //开始执行发送.
            string msg = BP.WF.Dev2Interface.Node_SendWork("001", this.OID, 103, null).ToMsgOfText();

            //  string msg = BP.WF.Dev2Interface.Flow_ReSend(this.OID, 103, "zhoupeng", "运动到").ToMsgOfText();
            return msg;
        }
    }
    /// <summary>
    /// 箱体订单s
    /// </summary>
    public class ND1Rpts : NDXRptBases
    {
        public string MyReWork_Init()
        {
            //2 重做
            string sql = "SELECT OID, Title, WorkSta, Worker,count(OID) as Num FROM V_WorkDtl WHERE Worker = '" + BP.Web.WebUser.No + "' AND WorkSta=2 GROUP BY Title,WorkSta,Worker,OID ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }


        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND1Rpt();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND1Rpts() { }
        #endregion
    }
}
