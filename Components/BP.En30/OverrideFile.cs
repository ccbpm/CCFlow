using System;
using System.Collections.Generic;
using System.Text;

namespace BP.En
{
    /// <summary>
    /// 可以被重写的类
    /// </summary>
    public class OverrideFile
    {
        #region 可以重写的表单事件.
        /// <summary>
        /// 执行的事件
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="en"></param>
        public static void FrmEvent_LoadBefore(string frmID, Entity en)
        {
            
        }
        /// <summary>
        /// 装载填充的事件.
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="en"></param>
        public static void FrmEvent_FrmLoadAfter(string frmID, Entity en)
        {

        }
        /// <summary>
        /// 保存前事件
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="en"></param>
        public static void FrmEvent_SaveBefore(string frmID, Entity en)
        {

        }
        /// <summary>
        /// 保存后事件
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="en"></param>
        public static void FrmEvent_SaveAfter(string frmID, Entity en)
        {
        }
        #endregion 可以重写的表单事件.
    }
}
