using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    /// 方法类型模式
    /// </summary>
    public class MethodModelClass
    {
        /// <summary>
        /// 链接或者按钮
        /// </summary>
        public const string LinkButton = "LinkButton";
        /// <summary>
        /// 方法
        /// </summary>
        public const string Func = "Func";
        /// <summary>
        /// 修改实体数据流程
        /// </summary>
        public const string FlowBaseData = "FlowBaseData";
        /// <summary>
        /// 新建实体流程
        /// </summary>
        public const string FlowNewEntity = "FlowNewEntity";
        /// <summary>
        /// 实体批量发起流程
        /// </summary>
        public const string FlowEntityBatchStart = "FlowEntityBatchStart";
        /// <summary>
        /// 新建实体流程
        /// </summary>
        public const string FlowEtc = "FlowEtc";
    }

    public enum SearchDataRole
    {
        /// <summary>
        /// 只查询自己创建的
        /// </summary>
        ByOnlySelf = 0,
        /// <summary>
        /// 查询本部门创建的包含兼职部门
        /// </summary>
        ByDept = 1,
        /// <summary>
        /// 查询本部门（包含兼职部门）及子级部门
        /// </summary>
        ByDeptAndSSubLevel = 2,
        /// <summary>
        /// 根据岗位设定的部门的集合
        /// </summary>
        ByStationDept = 3,
        /// <summary>
        /// 查询所有用户创建的数据信息
        /// </summary>
        SearchAll = 4

    }
    public enum SearchDataRoleByDeptStation
    {
        /// <summary>
        /// 只查询自己创建的
        /// </summary>
        ByOnlySelf = 0,
        /// <summary>
        /// 查询本部门创建的包含兼职部门
        /// </summary>
        ByDept = 1
    }
    /// <summary>
    /// 表单活动类型
    /// </summary>
    public class FrmActionType
    {
        /// <summary>
        /// 创建
        /// </summary>
        public const string Create = "Create";
        /// <summary>
        ///保存 
        /// </summary>
        public const string Save = "Save";
        /// <summary>
        /// 评论
        /// </summary>
        public const string BBS = "BBS";
        /// <summary>
        /// 打开
        /// </summary>
        public const string View = "View";
        /// <summary>
        /// 回滚数据
        /// </summary>
        public const string DataVerReback = "DataVerReback";
        /// <summary>
        /// 发起流程
        /// </summary>
        public const string StartFlow = "StartFlow";
        /// <summary>
        /// 发起注册流程
        /// </summary>
        public const string StartRegFlow = "StartRegFlow";
        /// <summary>
        /// 其他
        /// </summary>
        public const string Etc = "Etc";
    }
}
