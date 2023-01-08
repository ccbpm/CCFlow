
namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 控制方式
    /// </summary>
    public enum CtrlWay
    {
        /// <summary>
        /// 游客
        /// </summary>
        Guest,
        /// <summary>
        /// 任何人
        /// </summary>
        AnyOne,
        /// <summary>
        /// 按角色
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按sql
        /// </summary>
        BySQL
    }
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 系统根目录
        /// </summary>
        Root,
        /// <summary>
        /// 系统类别
        /// </summary>
        AppSort,
        /// <summary>
        /// 系统
        /// </summary>
        App,
        /// <summary>
        /// 目录
        /// </summary>
        Dir,
        /// <summary>
        /// 菜单
        /// </summary>
        Menu,
        /// <summary>
        /// 功能控制点
        /// </summary>
        Function
    }
    /// <summary>
    /// 菜单控制类型
    /// </summary>
    public enum MenuCtrlWay
    {
        /// <summary>
        /// 按照设置的控制
        /// </summary>
        BySetting,
        /// <summary>
        /// 任何人
        /// </summary>
        Anyone,
        /// <summary>
        /// 仅仅管理员
        /// </summary>
        AdminOnly
    }
}
