using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.En.FrmUI
{
    /// <summary>
    /// 级联
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 主表字段
        /// </summary>
        public string KeyOfEn = null;
        /// <summary>
        /// 联动字段
        /// </summary>
        public string RelKeyOfEn = null;
        /// <summary>
        /// 联动的数据源
        /// </summary>
        public string SQL = null;
        /// <summary>
        /// 构造
        /// </summary>
        public Connection()
        {
        }
    }
    /// <summary>
    /// 级联s
    /// </summary>
    public class Connections : System.Collections.CollectionBase
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Connections()
        {
        }
        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Connection this[int index]
        {
            get
            {
                return this.InnerList[index] as Connection;
            }
        }
        /// <summary>
        /// 增加级联关系.
        /// </summary>
        /// <param name="keyOfEn"></param>
        /// <param name="refKeyOfEn"></param>
        /// <param name="sql"></param>
        public void Add(string keyOfEn, string refKeyOfEn, string sql)
        {
            Connection en = new Connection();
            en.KeyOfEn=keyOfEn;
            en.RelKeyOfEn = refKeyOfEn;
            en.SQL = sql;

        }
    }
}
