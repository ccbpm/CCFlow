﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BP.DA
{
    /// <summary>
    /// 时间计算方式
    /// </summary>
    public enum TWay
    {
        /// <summary>
        /// 计算节假日
        /// </summary>
        Holiday,
        /// <summary>
        /// 不计算节假日
        /// </summary>
        AllDays
    }
    /// <summary>
    /// 数据库部署类型
    /// </summary>
    public enum DBModel
    {
        /// <summary>
        /// 独立（集中模式）
        /// </summary>
        Single=0,
        /// <summary>
        /// 域模式
        /// </summary>
        Domain=1
    }
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// sqlserver
        /// </summary>
        MSSQL=0,
        /// <summary>
        /// oracle  
        /// </summary>
        Oracle=1,
        /// <summary>
        /// Access
        /// </summary>
        Access=2,
        /// <summary>
        /// PostgreSQL 
        /// </summary>
        PostgreSQL=3,
        /// <summary>
        /// DB2
        /// </summary>
        DB2=4,
        /// <summary>
        /// MySQL
        /// </summary>
        MySQL=5,
        /// <summary>
        /// Informix
        /// </summary>
        Informix=6,
        /// <summary>
        /// 达梦
        /// </summary>
        DM=7,
        /// <summary>
        /// 人大金仓
        /// </summary>
        KingBase=8
    }
    /// <summary>
    /// 保管位置
    /// </summary>
    public enum Depositary
    {
         /// <summary>
        /// 不保管
        /// </summary>
        None,
        /// <summary>
        /// 全体
        /// </summary>
        Application        
    }
    /// <summary>
    /// 图表类型
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// 柱状图
        /// </summary>
        Histogram,
        /// <summary>
        /// 丙状图
        /// </summary>
        Pie,
        /// <summary>
        /// 折线图
        /// </summary>
        Line
    }
    /// <summary>
    /// 分组方式
    /// </summary>
    public enum GroupWay
    {
        /// <summary>
        /// 求合
        /// </summary>
        BySum,
        /// <summary>
        /// 求平均
        /// </summary>
        ByAvg
    }
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderWay
    {
        /// <summary>
        /// 升序
        /// </summary>
        OrderByUp,
        /// <summary>
        /// 降序
        /// </summary>
        OrderByDown
    }
    /// <summary>
    /// 数据检查级别
    /// </summary>
    public enum DBCheckLevel
    {
        /// <summary>
        /// 低,只出报告,不操作任何数据
        /// </summary>
        Low = 1,
        /// <summary>
        /// 中,出检查报告,删除外键的左右空格.
        /// </summary>
        Middle = 2,
        /// <summary>
        /// 高,删除对应不上的数据.
        /// </summary>
        High = 3,
    }
}
