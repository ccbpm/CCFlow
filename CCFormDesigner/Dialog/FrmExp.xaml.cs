using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
namespace CCForm
{
    public partial class FrmExp : ChildWindow
    {
        public FrmExp()
        {
            InitializeComponent();
        }
        public LoadingWindow loading = new LoadingWindow();
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Show();
            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.DoTypeAsync("FrmTempleteExp", Glo.FK_MapData, null, null, null, null);
            ff.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(ff_DoTypeCompleted);
        }
        void ff_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            loading.DialogResult = false;
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "执行失败", MessageBoxButton.OK);
                return;
            }
            //Glo.WinOpen(Glo.BPMHost + "/WF/MapDef/Handler.ashx?DoType=DownTempFrm&FK_MapData=" + Glo.FK_MapData);
            Glo.WinOpen(Glo.BPMHost + "/WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + Glo.FK_MapData,
                100, 100);
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

