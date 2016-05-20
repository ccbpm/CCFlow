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

namespace CCForm
{
    public partial class FrmOp : ChildWindow
    {
        public FrmOp()
        {
            InitializeComponent();
        }
        protected override void OnOpened()
        {
            this.TB_Name.Text = Glo.HisMapData.Name;
            this.TB_FrmH.Text = Glo.HisMapData.FrmH.ToString("0.00");
            this.TB_FrmW.Text = Glo.HisMapData.FrmW.ToString("0.00");
            this.TB_FrmH.TextAlignment = TextAlignment.Right;
            this.TB_FrmW.TextAlignment = TextAlignment.Right;
            base.OnOpened();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Glo.HisMapData.FrmH = double.Parse(this.TB_FrmH.Text);
            Glo.HisMapData.FrmW = double.Parse(this.TB_FrmW.Text);
            Glo.HisMapData.Name = this.TB_Name.Text;

            // Glo.FK_Flow→→→ Glo.FK_MapData  qin
            string sql = "UPDATE Sys_MapData SET NAME='" + this.TB_Name.Text + "', FrmW=" + Glo.HisMapData.FrmW + ", FrmH=" + Glo.HisMapData.FrmH + " WHERE No='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient fa = Glo.GetCCFormSoapClientServiceInstance();
            fa.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
            fa.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(fa_RunSQLsCompleted);
        }
        void fa_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

