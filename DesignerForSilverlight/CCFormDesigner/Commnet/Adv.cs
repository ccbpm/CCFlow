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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CCForm
{
    public class AdvTool
    {

        #region attrs
        /// <summary>
        /// 隐藏字段
        /// </summary>
        public const string HiddenField = "HiddenField";
        public const string Mouse = "Mouse";
        public const string Selected = "Selected";
        public const string Line = "Line";
        /// <summary>
        /// 图片附件
        /// </summary>
        public const string ImgAth = "ImgAth";
        /// <summary>
        /// 标签
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// 连接
        /// </summary>
        public const string Link = "Link";
        /// <summary>
        /// 文本框
        /// </summary>
        public const string TextBox = "TextBox";
        /// <summary>
        /// DateCtl
        /// </summary>
        public const string DateCtl = "DateCtl";
        /// <summary>
        /// 下拉框
        /// </summary>
        public const string DDLEnum = "DDLEnum";
        /// <summary>
        /// 数据表
        /// </summary>
        public const string DDLTable = "DDLTable";
        /// <summary>
        /// 单选按钮
        /// </summary>
        public const string RBS = "RBS";
        /// <summary>
        /// 选择框
        /// </summary>
        public const string CheckBox = "CheckBox";
        /// <summary>
        /// 图片
        /// </summary>
        public const string Img = "Img";
        public const string Dtl = "Dtl";
        public const string M2M = "M2M";

        public const string Attachment = "Attachment";
        #endregion

        #region 字段
        string icoName;
        string icoNameText;
        BitmapImage icoImage;
        #endregion

        #region 属性
        /// <summary>
        /// 图标名称
        /// </summary>
        public string IcoName
        {
            get { return icoName; }
            set { icoName = value; }
        }
        /// <summary>
        /// 图标图像
        /// </summary>
        public BitmapImage IcoImage
        {
            get { return icoImage; }
            set { icoImage = value; }
        }
        /// <summary>
        /// 图标文本
        /// </summary>
        public string IcoNameText
        {
            get { return icoNameText; }
            set { icoNameText = value; }
        }
        #endregion

        #region 单一实例

        public static readonly AdvTool instance = new AdvTool();

        #endregion

        #region 公共方法

      
        #endregion
    }
}
