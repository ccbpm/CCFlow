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

namespace CCForm
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public class DBType
    {
        /// <summary>
        /// MSSQL
        /// </summary>
        public const string MSSQL = "MSSQL";
        /// <summary>
        /// Oracle
        /// </summary>
        public const string Oracle = "Oracle";
        /// <summary>
        /// MySQL
        /// </summary>
        public const string MySQL = "MySQL";
        /// <summary>
        /// Infomix
        /// </summary>
        public const string Infomix = "Infomix";
    }
    /// <summary>
    /// 附件上传类型
    /// </summary>
    public enum AttachmentUploadType
    {
        /// <summary>
        /// 单个的
        /// </summary>
        Single,
        /// <summary>
        /// 多个的
        /// </summary>
        Multi,
        /// <summary>
        /// 指定的
        /// </summary>
        Specifically
    }
    /// <summary>
    /// 类型
    /// </summary>
    public enum TBType
    {
        String,
        Int,
        Float,
        Money,
        DateTime,
        Date
    }
    /// <summary>
    /// 光标状态
    /// </summary>
    public enum MousePosition
    {
        None = 0, //'无      
        SizeRight = 1, //'拉伸右边框      
        SizeLeft = 2, //'拉伸左边框      
        SizeTop = 4, //'拉伸上边框      
        SizeTopLeft = 5, //'拉伸左上角      
        SizeTopRight = 6, //'拉伸右上角   
        SizeBottom = 3, //'拉伸下边框   
        SizeBottomLeft = 7, //'拉伸左下角      
        SizeBottomRight = 8, //'拉伸右下角      
        Drag = 9   // '鼠标拖动.
    }
    public class EEleTableNames
    {
        public const string
            Sys_FrmLine = "SYS_FRMLINE",
            Sys_FrmBtn = "SYS_FRMBTN",
            Sys_FrmLab = "SYS_FRMLAB",
            Sys_FrmLink = "SYS_FRMLINK",
            Sys_FrmImg = "SYS_FRMIMG",
            Sys_FrmEle = "SYS_FRMELE",
            Sys_FrmImgAth = "SYS_FRMIMGATH",
            Sys_FrmRB = "SYS_FRMRB",
            Sys_FrmAttachment = "SYS_FRMATTACHMENT",
            Sys_MapData = "SYS_MAPDATA",
            Sys_MapAttr = "SYS_MAPATTR",
            Sys_MapDtl = "SYS_MAPDTL",
            Sys_MapM2M = "SYS_MAPM2M",
            WF_Node = "WF_NODE";//BPWorkCheck
    }

    // 1）相对于设计器UI,所有新增元素操作状态都为新增
    // 2）若设计器首次加载，加载完成后把状态都改为Default,保存时不做处理
    // 3）若为新增元素,元素状态不变,带保存时遍历生成 insert 语句
    // 4）元素删除时,现在执行元素删除方法,执行 delete 语句,更新 UI
    // 5）元素编辑时,元素状态为Update,保存时遍历生成 update 语句
    //public enum OperateState
    //{
    //    Added,
    //    Updated,
    //    Deleted,
    //    Default
    //}
}
