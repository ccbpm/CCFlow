using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Demo.BuessUnit
{
    /// <summary>
    /// 该类为业务单元子类.
    /// 1. 该子类必须放入到BP.*.dll 里面.
    /// 2. 
    /// </summary>
    public class BuessUnitDemo : BP.Sys.BuessUnitBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        public override string Title
        {
            get {
                return "业务单元测试";
            }
        }
        public override string DoIt()
        {


            // this.WorkID;
            BP.DA.Log.DebugWriteInfo("BuessUnitDemo业务单元已经被执行:WorkID=" + this.WorkID);
            return base.DoIt();
        }
    }
}
