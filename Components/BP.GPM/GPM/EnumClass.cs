﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.GPM
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
        /// 按岗位
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
}
