using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Silverlight;

namespace CCForm
{
    public partial class FrmEleMapPin : ChildWindow
    {
        private string _MyPK = null;
        private string _Name = null;

        public FrmEleMapPin()
        {
            InitializeComponent();
        }
        public void InitForm()
        {
            _MyPK = null;
            _Name = null;
            this.RB_0.IsChecked = true;
        }

        public BPMapPin HisEle = null;

        public void BindData(string MyPK)
        {
            _MyPK = MyPK;
            this.RB_0.IsChecked = true;
            string sql = "SELECT * FROM Sys_FrmEle WHERE MyPK='" + MyPK + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }

        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                _Name = dt.Rows[0]["EleID"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["ISENABLE"]) && dt.Rows[0]["ISENABLE"].ToString() == "0")
                {
                    this.RB_1.IsChecked = true;
                }
                if (HisEle == null)
                {
                    HisEle = new BPMapPin();
                }
                HisEle.Name = _Name;
                HisEle.MyPK = _MyPK;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //新增
            if (string.IsNullOrEmpty(_Name))
            {
                BPMapPin mapPin = new BPMapPin();
                mapPin.MyPK = Glo.FK_MapData + "_MapPin_" + mapPin.Name;
                this.HisEle = mapPin;
                _MyPK = mapPin.MyPK;
                _Name = mapPin.Name;
            }

            string strs = "@EnName=BP.Sys.FrmEle@PKVal=" + _MyPK;
            strs += "@EleType=MapPin";
            strs += "@EleName=MapPin";
            strs += "@EleID=" + _Name;
            strs += "@FK_MapData=" + Glo.FK_MapData;

            if (this.RB_0.IsChecked == true)
                strs += "@IsEnable=1";
            else
                strs += "@IsEnable=0";

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveEnAsync(strs);
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
    }
}

