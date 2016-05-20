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
    public class EleFunc
    {
        #region attrs
        public const string SelectAll = "SelectAll";
        public const string CopyEle = "CopyEle";
        public const string Paste = "Paste";
        public const string Bold = "Bold";
        public const string Italic = "Italic";
        public const string Strike = "Strike";
        public const string FontSizeAdd = "FontSizeAdd";
        public const string FontSizeCut = "FontSizeCut";
        public const string Colorpicker = "Colorpicker";
        #endregion

        #region 字段
        string _No;
        string _Name;
        BitmapImage _Img;
        string _Tooltip;
        #endregion

        #region 属性
        /// <summary>
        /// 图标名称
        /// </summary>
        public string No
        {
            get { return _No; }
            set { _No = value; }
        }
        /// <summary>
        /// 图标图像
        /// </summary>
        public BitmapImage Img
        {
            get { return _Img; }
            set { _Img = value; }
        }
        /// <summary>
        /// 图标文本
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// 提示
        /// </summary>
        public string Tooltip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; }
        }
        #endregion

        #region 单一实例
        public static readonly EleFunc instance = new EleFunc();
        #endregion

        #region 公共方法
        public List<EleFunc> getToolList()
        {
            List<EleFunc> ToolList = new List<EleFunc>()
            {
                //new EleFunc(){ No=EleFunc.SelectAll,  Name="全选", Tooltip="选择全部的元素，可以批量删除、移动等操作, 也可以ctrl+控件的方式实现多选择.",
                  //  Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.SelectAll+".png",UriKind.RelativeOrAbsolute))},
             //   new EleFunc(){ No=EleFunc.CopyEle, Tooltip="复制已经选择的元素", Name="复制",Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.CopyEle+".png",UriKind.RelativeOrAbsolute))},
              //  new EleFunc(){ No=EleFunc.Paste, Tooltip="", Name="粘帖",Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.Paste+".png",UriKind.RelativeOrAbsolute))},

                new EleFunc(){ No=Func.Alignment_Left,Name="左对齐", Tooltip="可对选泽的一个或者多个控件执行", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Alignment_Left+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=Func.Alignment_Center,  Name="居中",Tooltip="可对选泽的一个或者多个控件执行", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Alignment_Center+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=Func.Alignment_Right,Name="右对齐", Tooltip="可对选泽的一个或者多个控件执行", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Alignment_Right+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=Func.Alignment_Top, Name="顶部对齐",Tooltip="可对选泽的一个或者多个控件执行", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Alignment_Top+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=Func.Alignment_Down, Name="底部对齐",Tooltip="可对选泽的一个或者多个控件执行", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Alignment_Down+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=Func.Delete,  Name="删除",Tooltip="删除选择的一个或者多个元素", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Delete+".png",UriKind.RelativeOrAbsolute))},
                //new EleFunc(){ No=Func.Undo,  Name="撤销",Tooltip="此功能尚未实现", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.Undo+".png",UriKind.RelativeOrAbsolute))},
                //new EleFunc(){ No=Func.ForwardDo,  Name="恢复",Tooltip="此功能尚未实现",Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+Func.ForwardDo+".png",UriKind.RelativeOrAbsolute))},
                
                new EleFunc(){ No=EleFunc.Bold, Name="粗体",Tooltip="对于选择的一个或者多个标签元素有效", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.Bold+".png",UriKind.RelativeOrAbsolute))},
                //new EleFunc(){ No=EleFunc.Italic, Name="斜体",Tooltip="对于选择的一个或者多个标签元素有效",  Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.Italic+".png",UriKind.RelativeOrAbsolute))},
                //new EleFunc(){ No=EleFunc.Strike,  Name="删线",Tooltip="对于选择的一个或者多个标签元素有效", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.Strike+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=EleFunc.FontSizeAdd,  Name="增大",Tooltip="对于选择的一个或者多个标签、线元素有效",  Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.FontSizeAdd+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=EleFunc.FontSizeCut,  Name="减小",Tooltip="对于选择的一个或者多个标签、线元素有效", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.FontSizeCut+".png",UriKind.RelativeOrAbsolute))},
                new EleFunc(){ No=EleFunc.Colorpicker,  Name="颜色",Tooltip="对于选择的一个或者多个标签、线元素有效", Img=new BitmapImage(new Uri("/CCFormDesigner;component/Img/"+EleFunc.Colorpicker+".png",UriKind.RelativeOrAbsolute))}
            };
            return ToolList;
        }
        #endregion
    }
}
