using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Silverlight;

namespace CCForm
{
    public class BPM2M : UCExt, IDelete
    {
        public Grid MyDG = null;
        public int IsM2M = 0;
        void init()
        {
            new Adjust().Bind(this);
            this.BindDrag();

            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.isCanReSize = true;

            this.IsSelected = false;
            this.SizeChanged += new SizeChangedEventHandler((object sender, SizeChangedEventArgs e)=>
            {
                this.Width = this.Width;
                this.Height = this.Height;
                if (MyDG != null)
                {
                    if (!Convert.IsDBNull(this.Width))
                        MyDG.Width = this.Width;
                    if (!Convert.IsDBNull(this.Height))
                        MyDG.Height = this.Height;
                }
            });
            this.Width = 500;
            this.Height = 200;
        }
        public BPM2M(int IsM2M)
        {
            this.IsM2M = IsM2M;
            init();
            AddGridUI();
        }
        void AddGridUI()
        {
            Grid dg = CreateControls();
            dg.Name = "DG" + this.Name;
            dg.Width = this.Width;
            dg.Height = this.Height;
            this.layout.Child = dg;
            this.MyDG = dg;
        }

        /// <summary>
        /// Dtl
        /// </summary>
        public BPM2M(string name,int identity)
        {
            this.Name = name;
       
            this.init();
            this.LoadDtl( identity);
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
          <TextBlock >{0}</TextBlock>
    </Border>
    <TextBlock Grid.Row='1' TextWrapping='Wrap' VerticalAlignment='Center' HorizontalAlignment='Center'>提示:aswd键改变位置，鼠标移动到边框，按住左键调整大小，双击或右键修改属性。</TextBlock>
</Grid>";
           
            if (this.IsM2M == 0)
            {
                xaml = string.Format(xaml, "一对多控件");
            }
            else
            {
                xaml = string.Format(xaml, "一对多多控件");
            }

            Grid grid = (Grid)System.Windows.Markup.XamlReader.Load(xaml);

            return grid;
        }


        public void LoadDtl(int M2MType)
        {
            IsM2M = M2MType;
            AddGridUI();
        }
        public void NewM2M(double x, double y)
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("NewM2M", Glo.FK_MapData, this.Name, x.ToString(), y.ToString(), null);
            da.DoTypeCompleted += (object sender, FF.DoTypeCompletedEventArgs e)=>
            {
                if (e.Result != null)
                {
                    MessageBox.Show(e.Result, "新建错误", MessageBoxButton.OK);
                    return;
                }
                Glo.OpenM2M(Glo.FK_MapData, this.Name + Glo.TimeKey);
            };
        }

        public void UpdatePos()
        {
        }

        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除[" + this.Name + "]吗？如果确定它相关的字段与设置也将会被删除，以前产生的历史数据也会被删除。", "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("DelM2M", this.Name, null, null, null, null, null);
            da.DoTypeCompleted +=(object sender, FF.DoTypeCompletedEventArgs e)=>
            {
                if (e.Result != null)
                {
                    MessageBox.Show(e.Result, "删除错误", MessageBoxButton.OK);
                    return;
                }

                Glo.Remove(this);
            };
        }

        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            string sql = "UPDATE Sys_MapDtl SET IsView=0 WHERE No='" + Glo.FK_MapData + "'";
            this.Visibility = System.Windows.Visibility.Collapsed;
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
            hidDA.RunSQLsCompleted +=(object sender, FF.RunSQLsCompletedEventArgs e)=>
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            };
        }
    }
}
