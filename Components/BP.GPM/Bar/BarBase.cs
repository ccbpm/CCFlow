using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
using BP.WF.XML;

namespace BP.GPM
{
    /// <summary>
    /// 流程事件基类
    /// 0,集成该基类的子类,可以重写事件的方法与基类交互.
    /// 1,一个子类必须与一个流程模版绑定.
    /// 2,基类里有很多流程运行过程中的变量，这些变量可以辅助开发者在编写复杂的业务逻辑的时候使用.
    /// 3,该基类有一个子类模版，位于:\CCFlow\WF\Admin\AttrFlow\F001Templepte.cs .
    /// </summary>
    abstract public class BarBase
    {
        #region 系统属性.
        /// <summary>
        /// 流程编号/流程标记.
        /// </summary>
        abstract public string No
        {
            get;
        }
        /// <summary>
        /// 名称
        /// </summary>
        abstract public string Name
        {
            get;
        }
        /// <summary>
        /// 权限控制-是否可以查看
        /// </summary>
        abstract public bool IsCanView
        {
            get;
        }
        #endregion 系统属性.

        #region 外观行为.
        /// <summary>
        /// 标题
        /// </summary>
        abstract public string Title
        {
            get;
        }
        /// <summary>
        /// 更多连接
        /// </summary>
        abstract public string More
        {
            get;
        }
        /// <summary>
        /// 内容信息
        /// </summary>
        abstract public string Documents
        {
            get;
        }
        /// <summary>
        /// 宽度
        /// </summary>
        abstract public string Width
        {
            get;
        }
        /// <summary>
        /// 高度
        /// </summary>
        abstract public string Height
        {
            get;
        }
        #endregion 外观行为.

    }
}
