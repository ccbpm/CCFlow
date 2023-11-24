using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 业务单元Demo
    /// </summary>
    public class DemoBuessUnit : BP.Sys.BuessUnitBase
    {
        public override string Title
        {
            get
            {
                return "业务单元测试:BP.Demo.DemoBuessUnit";
            }
        }
        public override string DoIt()
        {
            //把业务逻辑写到这里,通过workid参数，获得更多的数据.

            //输出结果.
            return "执行成功.WorkID:" + this.WorkID;
            // return base.DoIt();
        }
    }

}
