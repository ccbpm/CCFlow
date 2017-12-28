using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 调Handler，返回值data的通用对象类
    /// </summary>
    public class JsonResultInnerData
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据对象
        /// </summary>
        public object InnerData { get; set; }
    }
}
