using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Sys
{
    /// <summary>
    /// 文本框类型
    /// </summary>
    public enum TBModel
    {
        /// <summary>
        /// 正常的
        /// </summary>
        Normal,
        /// <summary>
        /// 大文本
        /// </summary>
        BigDoc,
        /// <summary>
        /// 富文本
        /// </summary>
        RichText,
        /// <summary>
        /// 超大文本
        /// </summary>
        SupperText
    }
    /// <summary>
    /// 数字签名类型
    /// </summary>
    public enum SignType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 图片
        /// </summary>
        Pic,
        /// <summary>
        /// 山东CA签名.
        /// </summary>
        CA,
        /// <summary>
        /// 广东CA
        /// </summary>
        GDCA,
        /// <summary>
        /// 图片盖章
        /// </summary>
        GZCA
    }
    public enum PicType
    {
        /// <summary>
        /// 自动签名
        /// </summary>
        Auto,
        /// <summary>
        /// 手动签名
        /// </summary>
        ShouDong
    }
}
