using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// 找领导类型
    /// </summary>
    public enum FindLeaderType
    {
        /// <summary>
        /// 提交人
        /// </summary>
        Submiter,
        /// <summary>
        /// 指定节点的提交人
        /// </summary>
        SpecNodeSubmiter,
        /// <summary>
        /// 特定字段的提交人
        /// </summary>
        BySpecField
    }
    /// <summary>
    /// 寻找领导模式
    /// </summary>
    public enum FindLeaderModel
    {
        /// <summary>
        /// 直接领导
        /// </summary>
        DirLeader,
        /// <summary>
        /// 指定职务级别的领导
        /// </summary>
        SpecDutyLevelLeader,
        /// <summary>
        /// 特定职务领导
        /// </summary>
        DutyLeader,
        /// <summary>
        /// 特定岗位
        /// </summary>
        SpecStation
    }
}
