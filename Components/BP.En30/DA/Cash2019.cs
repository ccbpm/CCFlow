using System;
using System.Collections;
using System.IO;
using System.Text;
using BP.En;
using BP.Pub;
using BP.Sys;

namespace BP.DA
{
    public class Cash2019
    {
        #region 对实体的操作.
        /// <summary>
        /// 把实体放入缓存里面
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="ens"></param>
        /// <param name="enPK"></param>
        public static void PutRow(string enName, object pkVal, Row row)
        {
        }
        /// <summary>
        /// 获得实体类
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="pkVal"></param>
        /// <returns></returns>
        public static Row GetRow(string enName, object pkVal)
        {
            return null;
        }
        #endregion 对实体的操作.


        #region 对实体的集合操作.
        /// <summary>
        /// 把集合放入缓存.
        /// </summary>
        /// <param name="ensName">集合实体类名</param>
        /// <param name="ens">实体集合</param>
        public static void PutEns(string ensName, Entities ens)
        {
        }
        /// <summary>
        /// 获取实体集合类
        /// </summary>
        /// <param name="ensName">集合类名</param>
        /// <param name="pkVal">主键</param>
        /// <returns>实体集合</returns>
        public static Entities GetEns(string ensName, object pkVal)
        {
            return null;
        }
        #endregion 对实体的集合操作.

    }
}
