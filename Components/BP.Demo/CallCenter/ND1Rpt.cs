using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;

namespace BP.CallCenter
{
    /// <summary>
    /// 工单 Attr
    /// </summary>
    public class ND1RptAttr : NDXRptBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 房间号
        /// </summary>
        public const string FangJianHao = "FangJianHao";
        /// <summary>
        /// 故障描述
        /// </summary>
        public const string GuZhangMiaoShu = "GuZhangMiaoShu";
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
        /// 发货人
        /// </summary>
        public const string FaHuoRen = "FaHuoRen";
        #endregion
    }
    /// <summary>
    /// 工单
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

                Map map = new Map("ND1Rpt", "工单信息");

                #region 流程的基本字段
                map.AddTBIntPKOID();
                map.AddTBString(ND1RptAttr.Title, null, "标题", false, true, 0, 500, 10);
                map.AddTBString(ND1RptAttr.BillNo, null, "工单编号", false, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.FangJianHao, null, "房间号", false, true, 0, 50, 10);
                map.AddTBString(ND1RptAttr.GuZhangMiaoShu, null, "故障描述", false, true, 0, 50, 10);

                //  map.AddDDLSysEnum(ND1RptAttr.WFState, 0, "状态", false, true, "WFState");
                //map.AddTBInt(ND1RptAttr.FID, 0, "FID", false, true);
                //map.AddTBInt(ND1RptAttr.FlowDaySpan, 0, "跨度", false, true);
                //map.AddTBInt(ND1RptAttr.FlowEndNode, 0, "结束点", false, true);

                //map.AddTBString(ND1RptAttr.PrjName, null, "项目名称", false, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.QiXianXianDing, null, "期限限定", false, true, 0, 50, 10);
                //map.AddDDLSysEnum(ND1RptAttr.JJCD, 0, "紧急程度", true, true, ND1RptAttr.JJCD, "@0=低，@1=中，@2=高");

                //map.AddTBString(ND1RptAttr.TuZhiBianHao, null, "图纸编号", false, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.TuZhiZhiTuRen, null, "图纸制图人", false, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.ZhuangPeiFuZeRen, null, "装配负责人", false, true, 0, 50, 10);
                //map.AddTBString(ND1RptAttr.JianCeFuZeRen, null, "检测负责人", false, true, 0, 50, 10);

                //map.AddTBString(ND1RptAttr.FengZhuangRen, null, "封装人", false, true, 0, 50, 10);
                //map.AddTBInt(ND1RptAttr.PWorkID, 0, "父流程ID", false, true);
                //map.AddTBString(ND1RptAttr.FaHuoRen, null, "发货人", false, true, 0, 100, 10);
                #endregion 流程的基本字段





                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 工单s
    /// </summary>
    public class ND1Rpts : NDXRptBases
    {
        #region 微信客户。
        /// <summary>
        /// 让客户直接登录.
        /// </summary>
        /// <param name="paras"></param>
        /// <returns>执行结果</returns>
        public string WeiXinStartByCustomer_Init()
        {
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            return "登录成功.";
        }
        /// <summary>
        /// 接受表单的参数.
        /// </summary>
        /// <param name="paras">@key1=val1@key2=val2 </param>
        /// <returns>执行结果</returns>
        public string WeiXinStartByCustomer_Save(string paras)
        {
            //放到对象.
            BP.DA.AtPara ap = new DA.AtPara(paras);
            Hashtable ht = ap.HisHT; //获得存储数据.

            ht.Add("BaoXiuRen", BP.Web.WebUser.No);
            ht.Add("BaoXiuRenName", BP.Web.WebUser.Name);

            //创建workid.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001");

            //发送给客服,让客服分配任务.
            BP.WF.Dev2Interface.Node_SendWork("001", workid, ht, 110, null);
            return "流程发起成功.";
        }
        #endregion 微信客户。

        #region 工程师。
        /// <summary>
        /// 接受表单的参数.
        /// </summary>
        /// <param name="paras">@key1=val1@key2=val2 </param>
        /// <returns>执行结果</returns>
        public string WeiXinStartByWorker_Save(string paras)
        {
            //放到对象.
            BP.DA.AtPara ap = new DA.AtPara(paras);
            Hashtable ht = ap.HisHT; //获得存储数据.

            ht.Add("BaoXiuRen", BP.Web.WebUser.No);
            ht.Add("BaoXiuRenName", BP.Web.WebUser.Name);

            //创建workid.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("001");

            //求出自己的组长.
            string sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Dept='" + BP.Web.WebUser.FK_Dept + "' AND FK_Station='1122'";
            string leaderNo = BP.DA.DBAccess.RunSQLReturnString(sql);

            //发送给本部门的领导去分配工作.
            BP.WF.Dev2Interface.Node_SendWork("001", workid, ht, 102, leaderNo);

            BP.Port.Emp emp = new Port.Emp(leaderNo);
            return "流程发起成功,已经报审给：." + emp.Name + "。";
        }
        #endregion 微信客户。

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
