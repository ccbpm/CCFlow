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
    public class ND2RptAttr : NDXRptBaseAttr
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
    public class ND2Rpt : NDXRptBase
    {
        #region 属性
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string TuZhiBianHao
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.TuZhiBianHao);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.TuZhiBianHao, value);
            }
        }
        public string DingDanHao
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.DingDanHao);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.DingDanHao, value);
            }
        }
      
        /// <summary>
        /// 请假人编号
        /// </summary>
        public string TuZhiZhiTuRen
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.TuZhiZhiTuRen);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.TuZhiZhiTuRen, value);
            }
        }
        /// <summary>
        /// 请假人名称
        /// </summary>
        //public string ZhuangPeiFuZeRen
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(ND2RptAttr.ZhuangPeiFuZeRen);
        //    }
        //    set
        //    {
        //        this.SetValByKey(ND2RptAttr.ZhuangPeiFuZeRen, value);
        //    }
        //}
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public int JJCD
        {
            get
            {
                return this.GetValIntByKey(ND2RptAttr.JJCD);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.JJCD, value);
            }
        }

        /// <summary>
        /// 总经理意见
        /// </summary>
        public string JJCDText
        {
            get
            {
                return this.GetValRefTextByKey(ND2RptAttr.JJCD);
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
        public ND2Rpt()
        {

        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND2Rpt(Int64 workid)
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
                map.AddTBString(ND2RptAttr.Title, null, "标题", true, true, 0, 500, 10);
                map.AddTBString(ND2RptAttr.BillNo, null, "编号", true, true, 0, 50, 10);

                map.AddTBString(ND2RptAttr.DingDanHao, null, "订单号", true, true, 0, 50, 10);
                 map.AddTBString(ND2RptAttr.KeHuMingCheng, null, "客户名称", false, true, 0, 50, 10);
                map.AddTBString(ND2RptAttr.PrjName, null, "项目名称", true, true, 0, 50, 10);
                map.AddTBString(ND2RptAttr.QiXianXianDing, null, "期限限定", true, true, 0, 50, 10);
                map.AddDDLSysEnum(ND2RptAttr.JJCD, 0, "紧急程度", true, true, ND2RptAttr.JJCD, "@0=低，@1=中，@2=高");

                map.AddTBInt(ND2RptAttr.FlowEndNode, 0, "订单状态(运行的节点)", false, true);
                #endregion 流程的基本字段

                //箱体信息.
              //  map.AddDtl(new ND201Dtl1s(), ND201Dtl1Attr.RefPK);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 订单s
    /// </summary>
    public class ND2Rpts : NDXRptBases
    {

        /// <summary>
        /// 打包
        /// </summary>
        /// <returns></returns>
        public string DaBao_Init()
        {
            ///ND1Rpt rpts = new ND1Rpt();
            string sql = "SELECT B.OID, B.XiangTiMingCheng AS Name, A.Title FROM ND2Rpt A, ND201Dtl1 B WHERE B.XTSta=3 AND A.OID=B.RefPK ORDER BY A.PWorkID";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 物流单
        /// </summary>
        /// <param name="XiangTiDtls">箱体的IDs</param>
        /// <param name="relEmpNo">发货人</param>
        /// <param name="dtRel">发货日期</param>
        ///  <param name="fhBill">发货单号</param>
        /// <returns></returns>
        public string DaBao_Save(string XiangTiDtls, string relEmpNo, string dtRel, string fhBill,string fhName)
        {
            try
            {
                WuLiuBill en = new WuLiuBill();
                en.RelEmpNo = relEmpNo;
                en.XiangTiDtls = XiangTiDtls;
                en.DTRel = dtRel;
                en.FaHuoBill = fhBill;
                en.KuaiDiName = fhName;


                en.Insert();

                //遍历箱体.
                string[] strs = XiangTiDtls.Split(',');
                foreach (string mystr in strs)
                {
                    if (DataType.IsNullOrEmpty(mystr) == true)
                        continue;

                    DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta = 4, FaHuoRen = '" + relEmpNo + "', FaHuoRQ = '" + dtRel + "', FaHuoBill ='" + fhBill + "',FaHuoBillOID=" + en.OID + "  WHERE OID = " + mystr);
                }

                //获得该物流单，包含的 订单. 检查每个订单是否都已经发货，如果发货，就让其结束。
                string sql = "select distinct RefPK from ND201Dtl1 where FaHuoBillOID=" + en.OID;
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Int64 workIDOf002 = Int64.Parse(dr[0].ToString()); //订单的workID.

                    sql = "select * from ND201Dtl1 where XTSta !=4   and refpk=" + workIDOf002;
                    DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (mydt.Rows.Count == 0)
                    {
                        //所有的订单的下的箱子 已经发货了。
                        BP.WF.Dev2Interface.Flow_DoFlowOver(workIDOf002, "已经发货完毕，执行订单结束");
                    }
                }

                return "执行成功..";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND2Rpt();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND2Rpts() { }
        #endregion
    }
}
