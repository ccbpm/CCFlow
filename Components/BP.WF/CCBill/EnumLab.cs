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
