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
    public class ToolBox
    {
        #region attrs
        ///// <summary>
        ///// 隐藏字段
        ///// </summary>
        //public const string HiddenField = "HiddenField";
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
        /// <summary>
        /// 签章图片
        /// </summary>
        public const string SealImg = "SealImg";

        public const string Dtl = "Dtl";
        public const string M2M = "M2M";
        public const string M2MM = "M2MM";
        /// <summary>
        /// 审核组件
        /// </summary>
        public const string WorkCheck = "WorkCheck";
        /// <summary>
        /// 父子流程
        /// </summary>
        public const string SubFlow = "SubFlow";
        /// <summary>
        /// 子线程
        /// </summary>
        public const string SubThread = "SubThread";

        public const string FrmEle = "FrmEle";
        public const string Btn = "Btn";
        public const string Attachment = "Attachment";
        /// <summary>
        /// 多文件上传控件
        /// </summary>
        public const string AttachmentM = "AttachmentM";
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
    }

    public class ToolBoxes
    {
        #region 单一实例
        public static readonly ToolBoxes instance = new ToolBoxes();
        #endregion

        public List<ToolBox> GetToolBoxList()
        {
            if (Glo.IsNodeFrm == true)
            {
                List<ToolBox> ToolBoxList = new List<ToolBox>()
            {
                new ToolBox(){ IcoName=ToolBox.Mouse, IcoNameText="鼠标", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Mouse.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Selected,  IcoNameText="选择",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Selected.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Line,  IcoNameText="画线",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Line.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Label,  IcoNameText="标签",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Label.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Link,  IcoNameText="超连接",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Link.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Btn, IcoNameText="按钮",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Btn.png",UriKind.RelativeOrAbsolute))},

                new ToolBox(){ IcoName=ToolBox.Img, IcoNameText="装饰图片",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Img.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.SealImg, IcoNameText="签章",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/seal.png",UriKind.RelativeOrAbsolute))},

                new ToolBox(){ IcoName=ToolBox.TextBox,IcoNameText="文本框", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/TextBox.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DateCtl,IcoNameText="日期/时间", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Calendar.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.CheckBox,IcoNameText="选择框", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/CheckBox.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DDLEnum, IcoNameText="下拉框(枚举)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/DDLEnum.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DDLTable, IcoNameText="下拉框(表/视图)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/DDLEnum.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.RBS,  IcoNameText="单选按钮",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/RB.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.FrmEle, IcoNameText="扩展控件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/FrmEle.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Attachment, IcoNameText="单附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Attachment.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.AttachmentM, IcoNameText="多附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/AttachmentM.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.ImgAth, IcoNameText="图片附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/ImgAth.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Dtl, IcoNameText="明细表(从表)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Dtl.png",UriKind.RelativeOrAbsolute))},
              //  new ToolBox(){ IcoName=ToolBox.M2M, IcoNameText="一对多",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
               // new ToolBox(){ IcoName=ToolBox.M2MM, IcoNameText="一对多多",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.WorkCheck, IcoNameText="审核组件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
                //new ToolBox(){ IcoName=ToolBox.SubThread, IcoNameText="子线程",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.SubFlow, IcoNameText="父子流程",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/SubFlow.png",UriKind.RelativeOrAbsolute))}

                //, new ToolBox(){ IcoName=ToolBox.HiddenField, IcoNameText="隐藏字段",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/HiddenField.png",UriKind.RelativeOrAbsolute))}
            };

                return ToolBoxList;

            }
            else
            {

                List<ToolBox> ToolBoxList = new List<ToolBox>()
            {
                new ToolBox(){ IcoName=ToolBox.Mouse, IcoNameText="鼠标", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Mouse.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Selected,  IcoNameText="选择",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Selected.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Line,  IcoNameText="画线",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Line.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Label,  IcoNameText="标签",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Label.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Link,  IcoNameText="超连接",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Link.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Btn, IcoNameText="按钮",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Btn.png",UriKind.RelativeOrAbsolute))},

                new ToolBox(){ IcoName=ToolBox.Img, IcoNameText="装饰图片",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Img.png",UriKind.RelativeOrAbsolute))},
               new ToolBox(){ IcoName=ToolBox.SealImg, IcoNameText="签章",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/seal.png",UriKind.RelativeOrAbsolute))},

                new ToolBox(){ IcoName=ToolBox.TextBox,IcoNameText="文本框", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/TextBox.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DateCtl,IcoNameText="日期/时间", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Calendar.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.CheckBox,IcoNameText="选择框", IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/CheckBox.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DDLEnum, IcoNameText="下拉框(枚举)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/DDLEnum.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.DDLTable, IcoNameText="下拉框(表/视图)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/DDLEnum.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.RBS,  IcoNameText="单选按钮",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/RB.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.FrmEle, IcoNameText="扩展控件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/FrmEle.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Attachment, IcoNameText="单附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Attachment.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.AttachmentM, IcoNameText="多附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/AttachmentM.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.ImgAth, IcoNameText="图片附件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/ImgAth.png",UriKind.RelativeOrAbsolute))},
                new ToolBox(){ IcoName=ToolBox.Dtl, IcoNameText="明细表(从表)",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/Dtl.png",UriKind.RelativeOrAbsolute))}
                //new ToolBox(){ IcoName=ToolBox.M2M, IcoNameText="一对多",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
               // new ToolBox(){ IcoName=ToolBox.M2MM, IcoNameText="一对多多",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
              //  new ToolBox(){ IcoName=ToolBox.WorkCheck, IcoNameText="审核组件",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/M2M.png",UriKind.RelativeOrAbsolute))},
            //    new ToolBox(){ IcoName=ToolBox.SubFlow, IcoNameText="父子流程",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/SubFlow.png",UriKind.RelativeOrAbsolute))}
                //, new ToolBox(){ IcoName=ToolBox.HiddenField, IcoNameText="隐藏字段",IcoImage=new BitmapImage(new Uri("/CCFormDesigner;component/Img/HiddenField.png",UriKind.RelativeOrAbsolute))}
            };
                
                return ToolBoxList;
            }

            // 为什么把隐藏字段放入了toolbar里了？
        }
      
    }
}
