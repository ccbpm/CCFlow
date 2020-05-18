using System;



namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// JQFileUpload 的摘要说明
    /// </summary>
    public class CCFormHeader : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 返回子类
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_CCForm);
            }
        }
    }
}