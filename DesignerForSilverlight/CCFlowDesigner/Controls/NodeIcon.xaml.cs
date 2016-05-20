
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Liquid;
using System.Windows.Threading;

namespace BP.Controls
{
  /// <summary>
  /// 流程图标元素
  /// </summary>
  public partial class NodeIcon 
  {
     
    public string Name { get; set; }
      
    public NodeIcon()
    {
      InitializeComponent();
      this.MouseRightButtonDown += BP.Glo.Element_MouseRightButtonDown;

    }

    private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
    {
    }


    private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      txtLink.TextDecorations = null;
    }

  }
}
