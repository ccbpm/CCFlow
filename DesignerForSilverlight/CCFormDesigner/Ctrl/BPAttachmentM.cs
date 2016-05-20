using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCForm
{
    public class BPAttachmentM : UCExt, IDelete
    {
        public string Label = null;
        public string SaveTo = null;
        public bool IsDelete = false;
        public bool IsDownload = false;
        public bool IsUpload = false;

        public BPAttachmentM()
        {
            new Adjust().Bind(this);
            this.BindDrag();
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 500;
            this.Height = 200;
            this.IsSelected = false;
            this.isCanReSize = true;

            // 填充
            Grid grid = CreateControls();
            grid.Width = this.Width;
            grid.Height = this.Height;
            grid.Name = "DG" + this.Name;
            this.layout.Child = grid;

            this.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                this.Width = this.Width;
                this.Height = this.Height;
                if (grid != null)
                {
                    if (!Convert.IsDBNull(this.Width))
                        grid.Width = this.Width;
                    if (!Convert.IsDBNull(this.Height))
                        grid.Height = this.Height;
                }
            };
        }

        private Grid CreateControls()
        {
            string xaml =
@"<Grid Background='Transparent' ShowGridLines='True'
    xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'  
    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
    xmlns:sdk='http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk'>
    <Grid.RowDefinitions>
        <RowDefinition Height='20'></RowDefinition>
        <RowDefinition Height='*'></RowDefinition>
    </Grid.RowDefinitions>
 
    <Border Background='LightGray' Grid.Row='0'>
          <TextBlock >多附件</TextBlock>
    </Border>
    <TextBlock Grid.Row='1' TextWrapping='Wrap' VerticalAlignment='Center' HorizontalAlignment='Center'>提示:aswd键改变位置，鼠标移动到边框，按住左键调整大小。</TextBlock>
</Grid>";


            Grid grid = (Grid)System.Windows.Markup.XamlReader.Load(xaml);
            return grid;
        }


        public void UpdatePos()
        {
            Glo.ViewNeedSave = true;
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
            }
        }

        public double X = 0;
        public double Y = 0;
        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除[" + this.Name + "]吗？如果确定以前产生的历史数据也会被删除。",
                "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            string sql = "DELETE FROM Sys_FrmAttachment WHERE NoOfObj='" + this.Name + "' AND FK_MapData='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs e)=>
            {
                if (e.Result != 1)
                    return;
                Glo.Remove(this);
            };
        }
     
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            TrackingMouseMove = true;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            TrackingMouseMove = false;
        }
        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }      
    }
}
