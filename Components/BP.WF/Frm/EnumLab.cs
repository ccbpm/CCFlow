using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Frm
{
    /// <summary>
    /// 表单活动类型
    /// </summary>
    public enum FrmActionType
    {
        /// <summary>
        /// 创建
        /// </summary>
        Create = 0,
        /// <summary>
        /// 保存
        /// </summary>
        Save = 1,
        /// <summary>
        /// 评论
        /// </summary>
        BBS,
        /// <summary>
        /// 打开
        /// </summary>
        View
    }
}
