
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Liquid;
using FileType = global::WF.WS.FileType;

namespace BP.Controls
{
  /// <summary>
  /// Navigation link visual control.
  /// </summary>
  public partial class FtpFileLink 
  {

      //public int Id { get; set; }
      //public string Name { get; set; }
      //public string IconSrc { get; set; }
      //public string ImageUri { get; set; }

      //bool selected = true;
      ///// <summary>
      ///// 客户端使用，服务不使用
      ///// </summary>
      //public bool Selected
      //{
      //    get { return selected; }
      //    set { selected = value; }
      //}

      FileType type;

      public FileType Type
      {
          get { return type; }
          set 
          {
            type = value;
            if (type == FileType.File)
            {
                this.img.Visibility = System.Windows.Visibility.Collapsed;
                this.imgFile.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.img.Visibility = System.Windows.Visibility.Visible;
                this.imgFile.Visibility = System.Windows.Visibility.Collapsed;
            }
              
          }
      }

     
    public MouseButtonEventHandler MouseLeftButtonDown;
    public FtpFileLink()
    {
      InitializeComponent();
      this.LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
    }

    void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.LayoutRoot.Tag = this.txtLink.Text;
        if (null != MouseLeftButtonDown)
            MouseLeftButtonDown(sender, e);
    }

     
    private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      txtLink.TextDecorations = TextDecorations.Underline;
    }


    private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      txtLink.TextDecorations = null;
    }

  }
}
