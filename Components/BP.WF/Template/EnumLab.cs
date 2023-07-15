using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程设计模式-
    /// </summary>
    public enum FlowDevModel
    {
        /// <summary>
        /// 专业模式
        /// </summary>
        Prefessional = 0,
        /// <summary>
        /// 极简模式
        /// </summary>
        JiJian=1,
        /// <summary>
        /// 累加模式
        /// </summary>
        FoolTruck = 2,
        /// <summary>
        /// 绑定单表单
        /// </summary>
        RefOneFrmTree = 3,
        /// <summary>
        /// 绑定多表单
        /// </summary>
        FrmTree = 4,
        /// <summary>
        /// SDK表单-WorkID模式
        /// </summary>
        SDKFrmWorkID = 5,
        /// <summary>
        /// SDK表单-自定义主键模式.
        /// </summary>
        SDKFrmSelfPK = 7,
        /// <summary>
        /// 嵌入式表单
        /// </summary>
        SelfFrm = 6,
        /// <summary>
        /// 物联网流程
        /// </summary>
        InternetOfThings = 10
    }
}
