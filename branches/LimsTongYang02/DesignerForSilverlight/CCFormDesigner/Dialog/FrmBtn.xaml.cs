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
using BP.En;

namespace CCForm
{
    public partial class FrmBtn : ChildWindow
    {
        public FrmBtn()
        {
            InitializeComponent();
        }
        private BPBtn _HisBtn = null;
        public BPBtn HisBtn
        {
            get
            {
                _HisBtn.Content = this.TB_Text.Text;
                ListBoxItem lb = (ListBoxItem)this.DDL_AppType.SelectedItem;
                _HisBtn.HisBtnType = (BtnType)int.Parse(lb.Tag.ToString());

                ListBoxItem li = (ListBoxItem)this.DDL_EventType.SelectedItem;
                _HisBtn.HisEventType = (EventType)int.Parse(li.Tag.ToString());
                _HisBtn.EventContext = this.TB_EventDoc.Text;

                _HisBtn.MsgErr = this.TB_MsgErr.Text;
                _HisBtn.MsgOK = this.TB_MsgOK.Text;
                return _HisBtn;
            }
            set
            {
                _HisBtn = value;
                this.TB_Text.Text = _HisBtn.Content.ToString();
                this.TB_EventDoc.Text = _HisBtn.EventContext;

                this.TB_MsgErr.Text = _HisBtn.MsgErr;
                this.TB_MsgOK.Text = _HisBtn.MsgOK;


                foreach (ListBoxItem lb in this.DDL_AppType.Items)
                {
                    int myNum = int.Parse(lb.Tag.ToString());
                    BtnType mybtnType = (BtnType)myNum;
                    if (mybtnType == _HisBtn.HisBtnType)
                    {
                        lb.IsSelected = true; 
                        break;
                    } 
                }

                foreach (ListBoxItem lb in this.DDL_EventType.Items)
                {
                    int myNum = int.Parse(lb.Tag.ToString());
                    EventType mybtnType = (EventType)myNum;
                    if (mybtnType == _HisBtn.HisEventType)
                    {
                        lb.IsSelected = true;
                        break;
                    }
                }
                //选择项
                DDL_EventType.SelectedIndex = (int)_HisBtn.HisEventType;
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            BPBtn btn = this.HisBtn;
            string keys = "@EnName=BP.Sys.FrmBtn@PKVal=" + btn.Name + "@FK_MapData=" + Glo.FK_MapData + "@Text=" + btn.Content.ToString() + "@DDL_AppType=" + (int)btn.HisBtnType + "@EventType=" + (int)btn.HisEventType + "@EventContext=" + this.TB_EventDoc.Text.Replace('@', '#') + "@MsgOK=" + this.TB_MsgOK.Text.Replace('@', '#') + "@MsgErr=" + this.TB_MsgErr.Text.Replace('@', '#');
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveEnAsync(keys);
            da.SaveEnCompleted += new EventHandler<FF.SaveEnCompletedEventArgs>(da_SaveEnCompleted);
        }

        void da_SaveEnCompleted(object sender, FF.SaveEnCompletedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void DDL_AppType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DDL_AppType == null)
                return;
            if (sender == null)
                return;

           // ListBoxItem lb = (ListBoxItem)e.OriginalSource;
            ListBoxItem lb =(ListBoxItem)this.DDL_AppType.SelectedItem;  //(ListBoxItem)e.OriginalSource;
            if (lb == null)
                return;

            if (lb.Tag.ToString() == "0")
            {
                this.TB_Text.Text = "Button1";
                this.TB_EventDoc.IsEnabled = true;
                this.DDL_EventType.IsEnabled = true;

                this.TB_MsgOK.IsEnabled = true;
                this.TB_MsgErr.IsEnabled = true;
                return;
            }

            int mytype=(int)this.HisBtn.HisBtnType;
            if (mytype.ToString() == lb.Tag.ToString())
            {
                this.TB_Text.Text = this.HisBtn.Content.ToString();
            }
            else
            {
                this.TB_Text.Text = lb.Content.ToString();
            }

            this.TB_EventDoc.IsEnabled = false;
            this.DDL_EventType.IsEnabled = false;
            this.TB_MsgOK.IsEnabled = false;
            this.TB_MsgErr.IsEnabled = false;
            return;
        }

        private void TB_EventDoc_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {

            
        }

        private void TB_MsgOK_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

