using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.TA;

namespace BP.WF.Template
{
	/// <summary>
	/// 任务 属性
	/// </summary>
    public class SubSystemAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// WebHost
        /// </summary>
        public const string WebHost = "WebHost";
        /// <summary>
        /// 流程
        /// </summary>
        public const string TokenPiv = "TokenPiv";
        /// <summary>
        /// 参数
        /// </summary>
        public const string TokenPublie = "TokenPublie";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string CallBack = "CallBack";
        /// <summary>
        /// Msg
        /// </summary>
        public const string RequestMethod = "RequestMethod";
        /// <summary>
        /// 发起时间
        /// </summary>
        public const string CallMaxNum = "CallMaxNum";
        /// <summary>
        /// 插入日期
        /// </summary>
        public const string ApiParas = "ApiParas";

        /// <summary>
        /// 到达节点（可以为0）
        /// </summary>
        public const string ApiNote = "ApiNote";
        public const string ParaDTModel = "ParaDTModel";
        #endregion
    }
	/// <summary>
	/// 任务
	/// </summary>
    public class SubSystem : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 参数
        /// </summary>
        public string ApiParas
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.ApiParas);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public int CallMaxNum
        {
            get
            {
                return this.GetValIntByKey(SubSystemAttr.CallMaxNum);
            }
        }
        /// <summary>
        /// 到达的人员
        /// </summary>
        public string RequestMethod
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.RequestMethod);
            }
        }
        public string CallBack
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.CallBack);
            }
        }
        public string TokenPublie
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.TokenPublie);
            }
        }
        public string TokenPiv
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.TokenPiv);
            }
        }
        public string WebHost
        {
            get
            {
                return this.GetValStringByKey(SubSystemAttr.WebHost);
            }
        }
        public bool IsJson
        {
            get
            {
                string str= this.GetValStringByKey(SubSystemAttr.ParaDTModel);
                if (str.Equals("1") == true)
                    return true;
                return false;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// SubSystem
        /// </summary>
        public SubSystem()
        {
        }
        public SubSystem(string no)
        {
            this.No = no;
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
                Map map = new Map("WF_FlowSort", "子系统");
                
                map.AddTBStringPK("No", null, "编号", true, false, 0, 5, 10);
                map.AddTBString("Name", null, "名称", true, false, 0, 200, 10);

                map.AddTBString("WebHost", null, "系统根路径", true, false, 0, 200, 30, true);
                map.AddTBString("TokenPiv", null, "系统私钥", true, false, 0, 200, 30, true);
                map.AddTBString("TokenPublie", null, "系统公钥", true, false, 0, 200, 30, true);
                map.AddTBString("CallBack", null, "系统回调审批态的url全路径", true, false, 0, 200, 30, true);
                map.AddDDLStringEnum("RequestMethod", "POST","请求模式", "@POST=POST@Get=Get", true);
                map.AddDDLStringEnum("ParaDTModel", "1", "数据格式", "@0=From格式@1=JSON格式", true);

                map.AddTBInt("CallMaxNum", 5, "最大回调次数", true, false);
                map.AddTBStringDoc("ApiParas", null, "参数格式", true, false, true);
                map.AddTBStringDoc("ApiNote", null, "备注", true, false, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	 
}
