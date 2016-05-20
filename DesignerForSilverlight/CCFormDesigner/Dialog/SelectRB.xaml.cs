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
using Silverlight;
using CCForm.FF;

namespace CCForm
{
    public partial class SelectRB : ChildWindow
    {
        public SelectRB()
        {
            InitializeComponent();
            //this.listBox1.SelectionMode = SelectionMode.Single;
            //this.listBox2.SelectionMode = SelectionMode.Single;
        }
        protected override void OnOpened()
        {
            if (this.listBox1.Items.Count == 0)
            {
                this.BindData();
                this.tabItem1.IsSelected = true;
                this.tabItem2.Header = "新建";
            }
            base.OnOpened();
        }
        public void BindData()
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();

            string sql = "SELECT NO,NAME,CFGVAL FROM SYS_ENUMMAIN";
            CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
            this.listBox1.SelectionChanged += new SelectionChangedEventHandler(listBox1_SelectionChanged);
        }

        void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBoxItem li = e.AddedItems[0] as ListBoxItem;
                string[] itemName = li.Content.ToString().Split(':');
                this.TB_KeyOfEn.Text = itemName[0];
                this.TB_KeyOfName.Text = itemName[1];

                string s = li.Tag as string;
                string[] strs = s.Split('@');
                this.listBox2.Items.Clear();
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    ListBoxItem dd = new ListBoxItem();
                    dd.Content = str;
                    this.listBox2.Items.Add(dd);
                }
                this.EditItem(li);
            }
            catch
            {
            }
        }
        /// <summary>
        /// da_RunSQLReturnTableCompleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ListBoxItem item = new ListBoxItem();
                item.Tag = dr["CFGVAL"] == null ? "" : dr["CFGVAL"].ToString();
                item.Content = dr["NO"] + ":" + dr["NAME"];
                
                //设置选中项
                if (dr["NO"] == TB_No.Text)
                {
                    string s = item.Tag as string;
                    string[] strs = s.Split('@');
                    this.listBox2.Items.Clear();
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        ListBoxItem dd = new ListBoxItem();
                        dd.Content = str;
                        this.listBox2.Items.Add(dd);
                    }
                    item.IsSelected = true;
                }
                this.listBox1.Items.Add(item);
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            #region 数据检查。
            if (string.IsNullOrEmpty(this.TB_Name.Text)
               || string.IsNullOrEmpty(this.TB_No.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "Note", MessageBoxButton.OK);
                return;
            }
            if (this.TB_No.Text.Length >= 50)
            {
                MessageBox.Show("英文名称太长,不能多于50个字符，并且必须是下划线或者英文字母。", "Note",
                    MessageBoxButton.OK);
                return;
            }
            #endregion 数据检查。
            
            ListBoxItem lbi = this.listBox1.SelectedItem as ListBoxItem;
            if (lbi == null)
            {
                MessageBox.Show("没有选择枚举值：请从左侧选项卡中选择枚举值。", "Note", MessageBoxButton.OK);
                this.tabItem1.IsSelected = true;
                return;
            }
            string enumKey = lbi.Content.ToString();
            enumKey = enumKey.Substring(0, enumKey.IndexOf(':'));


            CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveEnumFieldAsync(Glo.FK_MapData, this.TB_KeyOfEn.Text, this.TB_KeyOfName.Text, enumKey, Glo.X, Glo.Y);
            da.SaveEnumFieldCompleted += new EventHandler<SaveEnumFieldCompletedEventArgs>(da_SaveEnumFieldCompleted);

        }

        void da_SaveEnumFieldCompleted(object sender, SaveEnumFieldCompletedEventArgs e)
        {
            if (e.Result == "OK")
            {
                this.DialogResult = true;
                this.TB_No.Text = "";
                this.TB_Name.Text = "";
                this.TB_KeyOfEn.Text = "";
                this.TB_KeyOfName.Text = "";
                this.listBox2.Items.Clear();
                this.listBox3.Items.Clear();
                this.tabItem2.Header = "新建";
                return;
            }

            MessageBox.Show(e.Result);
        }

        //void da_NewFieldsCompleted(object sender, NewFieldsCompletedEventArgs e)
        //{
        //    if (e.Result == null)
        //    {
        //        this.DialogResult = true;
        //    }
        //    else
        //    {
        //        MessageBox.Show( e.Result, "Note", MessageBoxButton.OK);
        //        return;
        //    }
        //}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            this.TB_No.Text = "";
            this.TB_Name.Text = "";
            this.tabItem2.IsSelected = true;
            this.tabItem2.Header = "新建";

            this.listBox3.Items.Clear();
            int idx = 0;
            for (int i = idx; i < 20; i++)
            {
                TextBox tb = new TextBox();
                tb.Name = "TB_" + i;
                tb.Text = "";
                tb.Width = this.listBox3.Width;

                ListBoxItem lb = new ListBoxItem();
                lb.Name = "LB_" + i;
                lb.Content = tb;
                this.listBox3.Items.Add(lb);
            }
        }
        private void Btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem li = this.listBox1.SelectedItem as ListBoxItem;
            this.EditItem(li);
            this.tabItem2.IsSelected = true;

        }
        public void EditItem(ListBoxItem li)
        {
            string myStr = li.Content as string;
            this.TB_No.Text = myStr.Substring(0, myStr.IndexOf(':'));
            this.TB_Name.Text = myStr.Substring(myStr.IndexOf(':') + 1);
            this.tabItem2.Header = "编辑:" + this.TB_Name.Text;

            this.listBox3.Items.Clear();
            string s = li.Tag as string;
            string[] strs = s.Split('@');
            this.listBox3.Items.Clear();
            int idx = 0;
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string[] mystrs = str.Split('=');

                TextBox tb = new TextBox();
                tb.Name = "TB_" + idx;
                tb.Text = mystrs[1];
                tb.Width = this.listBox3.Width;

                ListBoxItem lb = new ListBoxItem();
                lb.Name = "LB_" + idx;
                lb.Content = tb;
                this.listBox3.Items.Add(lb);
                idx++;
            }

            idx++;
            for (int i = idx; i < 20; i++)
            {
                TextBox tb = new TextBox();
                tb.Name = "TB_" + i;
                tb.Text = "";
                tb.Width = this.listBox3.Width;

                ListBoxItem lb = new ListBoxItem();
                lb.Name = "LB_" + i;
                lb.Content = tb;
                this.listBox3.Items.Add(lb);
            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string keys = "";
            int idx = 0;
            foreach (ListBoxItem li in this.listBox3.Items)
            {
                TextBox tb = li.Content as TextBox;
                if (string.IsNullOrEmpty(tb.Text))
                    continue;

                keys += "@" + idx + "=" + tb.Text;
                idx++;
            }
            //枚举编号
            if (string.IsNullOrEmpty(this.TB_No.Text))
            {
                MessageBox.Show("枚举编号不能为空！");
                return;
            }
            //标签名称
            if (string.IsNullOrEmpty(this.TB_Name.Text))
            {
                MessageBox.Show("标签名称不能为空！");
                return;
            }
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveEnumAsync(this.TB_No.Text, this.TB_Name.Text, keys);
            da.SaveEnumCompleted += new EventHandler<SaveEnumCompletedEventArgs>(da_SaveEnumCompleted);
        }

        void da_SaveEnumCompleted(object sender, SaveEnumCompletedEventArgs e)
        {
            this.BindData();
            MessageBox.Show("保存成功");
            this.tabItem1.IsSelected = true;
        }

        private void Btn_Del_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("are you sure？", "Note",
                MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("DelEnum", this.TB_No.Text.Trim(), null, null, null, null);
            da.DoTypeCompleted += new EventHandler<DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }

        void da_DoTypeCompleted(object sender, DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "错误", MessageBoxButton.OK);
                return;
            }
        
            MessageBox.Show("删除成功", "提示", MessageBoxButton.OK);
            this.tabItem1.IsSelected = true;
            this.TB_No.Text = "";
            this.TB_Name.Text = "";
            this.listBox3.Items.Clear();
            this.listBox1.Items.Clear();
            this.BindData();
        }

        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            Glo.GetKeyOfEn(this.TB_Name.Text, true, this.TB_No);
        }
      
    }
}

