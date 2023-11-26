using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.DA
{
    /// <summary>
    /// 字段
    /// </summary>
    public class KeyVal
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
    }
    /// <summary>
    /// 字段s
    /// </summary>
    public class KeyVals : List<KeyVal>
    {
        public void Add(string _key, string _value, string _type)
        {
            Add(new KeyVal { key = _key, value = _value, type = _type });
        }
    }
}
