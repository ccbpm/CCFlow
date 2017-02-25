using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.Sys;

namespace BP.BPMN
{
    /// <summary>
    ///  网关类型列表
    /// </summary>
    public class GatewayList
    {
        /// <summary>
        /// 唯一网关
        /// </summary>
        public const string ExclusiveGateway = "ExclusiveGateway";
        /// <summary>
        /// 并行网关
        /// </summary>
        public const string ParallelGateway = "ParallelGateway";
        /// <summary>
        /// 事件驱动网关
        /// </summary>
        public const string EventbasedGateway = "EventbasedGateway";
        /// <summary>
        /// 包容性网关
        /// </summary>
        public const string InclusiveGateway = "InclusiveGateway";

        /*
         inclusiveGateway与 exclusiveGateway的区别
       exclusiveGateway 只会寻找唯一一条能走完的flow，也就是说当有一个flow可以走通的情况下，它不会再次去寻找第二条可以走通的flow ，如是没有符合条件的，就走defalut的flow。
       inclusiveGateway 会寻找所有符合条件的 flow，也就是说他会走完所有的符合条件的flow，如果没有符合的，那么就去走defalut的flow
         */
    }
   
}
