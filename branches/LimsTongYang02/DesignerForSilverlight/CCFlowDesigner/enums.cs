using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace BP
{
    public struct SEnum
    {
        public string Name;
        public int Value;
    }
    /// <summary>
    /// 组织机构/模式 
    /// </summary>
    public enum OSModel
    {
        /// <summary>
        /// 集成(WorkFlow)嵌入模式
        /// </summary>
        OneOne = 0,
        /// <summary>
        /// 独立运行(BPM)模式
        /// </summary>
        OneMore =1
    }
    /// <summary>
    /// 应用类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 独立表单
        /// </summary>
        Application = 0,
        /// <summary>
        /// 节点表单
        /// </summary>
        Node = 1
    }
    public enum FlowChartType
    {
        Shape = 0,
        UserIcon = 1,  
        UnKnown

    }
    /// <summary>
    /// 节点位置
    /// </summary>
    public enum NodePosType
    {
        Start,
        Mid,
        End
    }
    public enum MergePictureRepeatDirection
    {
        Vertical = 0,
        Horizontal,
        None
    }
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum FlowNodeType_del
    {
        INITIAL,
        INTERACTION,
        COMPLETION,
        //AND_MERGE, ccflow 没有这些类型。
        //AND_BRANCH,
        STATIONODE,
        AUTOMATION,
        DUMMY,
        //OR_BRANCH,
        //OR_MERGE,
        SUBPROCESS,
        VOTE_MERGE,
    }
    public enum FlowNodeType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL,
        /// <summary>
        /// 子线程.
        /// </summary>
        SubThread,
        /// <summary>
        /// 虚节点
        /// </summary>
        VirtualStart,
        VirtualEnd,
        //INITIAL,
        //INTERACTION,
        //COMPLETION,
        ////AND_MERGE, ccflow 没有这些类型。
        ////AND_BRANCH,
        //STATIONODE,
        //AUTOMATION,
        //DUMMY,
        ////OR_BRANCH,
        ////OR_MERGE,
        //SUBPROCESS,
        //VOTE_MERGE,

        UnKnown
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL,
        /// <summary>
        /// 子线程.
        /// </summary>
        SubThread
    }
    /// <summary>
    /// 节点位置类型
    /// </summary>
    public enum FlowNodePosType
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        Start,
        /// <summary>
        /// 中间点
        /// </summary>
        Mid,
        /// <summary>
        /// 结束点
        /// </summary>
        End
    }
    public enum WorkFlowElementType
    {
        FlowNode = 0,
        Direction,
        Label
    }

    public enum PageEditType
    {
        Add = 0,
        Modify,
        None
    }

    public enum DirectionLineType
    {
        Line = 0,
        Polyline
    }

    public enum HistoryType
    {
        New,
        Next,
        Previous
    } 

    public enum DirectionMove { Begin = 0, Line, End }

    public enum DirType
    {
        //前进线
        Forward = 0,
        //回退线
        Backward = 1,
        //虚拟线
        Virtual = 2
    }

    /// <summary>
    /// 普通工作节点处理模式
    /// </summary>
    public enum TodolistModel
    {
        /// <summary>
        /// 抢办(谁抢到谁来办理,办理完后其他人就不能办理.)
        /// </summary>
        QiangBan,
        /// <summary>
        /// 协作(没有处理顺序，接受的人都要去处理,由最后一个人发送到下一个节点)
        /// </summary>
        Teamup,
        /// <summary>
        /// 队列(按照顺序处理，有最后一个人发送到下一个节点)
        /// </summary>
        Order,
        /// <summary>
        /// 共享模式(需要申请，申请后才能执行)
        /// </summary>
        Sharing
    }

    public enum BPFormType
    {
       
        FormNode,
        FormFlow
    }
    //public enum FrmType
    //{
    //    傻瓜表单 = 0,
    //    自由表单 = 1,
    //    SL表单 = 2,
    //    自定义表单 = 3,
    //    Word表单 = 4,
    //    Excel表单 = 5
    //}


    //public class EnumConverter : IValueConverter 
    //{ 
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
    //    { 
    //        if (value == null || parameter == null)   
    //            return value; 
    //        return value.ToString() == parameter.ToString(); 
    //    } 
    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    { 
    //        if (value == null || parameter == null)  
    //            return value; 
    //        return Enum.Parse(targetType, (String)parameter, true); 
    //    } 
    //}
}
