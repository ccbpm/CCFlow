using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silverlight;

namespace CCForm
{
    public partial class Dtl : UserControl
    {
        FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
        public Dtl()
        {
            InitializeComponent();
            BindDtl();
        }
        public void BindDtl()
        {
            string sql = "SELECT * FROM Sys_MapAttr where fk_mapdata='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }

        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];

            //string sql = "SELECT * FROM Sys_MapAttr ";
            //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql); 

            this.grdLayoutRoot.ColumnDefinitions.Clear();
            this.grdLayoutRoot.RowDefinitions.Clear();

            #region 设置好布局
            foreach (DataColumn dc in dt.Columns)
            {
                ColumnDefinition cdf = new ColumnDefinition();
                // cdf.SetValue(ColumnDefinition.WidthProperty, (double)100);
                cdf.MaxWidth = (double)100;
                cdf.MinWidth = (double)20;
                this.grdLayoutRoot.ColumnDefinitions.Add(cdf);
            }
            foreach (DataRow dr in dt.Rows)
            {
                RowDefinition row = new RowDefinition();
                row.MaxHeight = (double)100;
                row.MinHeight = (double)20;
                this.grdLayoutRoot.RowDefinitions.Add(row);
            }
            #endregion 设置好布局

            #region 填充数据.
            int columnIdx = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                TextBlock text = new TextBlock();
                text.Name = dc.ColumnName;
                text.SetValue(Grid.ColumnProperty, columnIdx);
                text.SetValue(Grid.HeightProperty, (double)20);
                text.Text = dc.ColumnName;
                this.grdLayoutRoot.Children.Add(text);
                columnIdx++;
            }

            int rowIdx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                columnIdx = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    TextBlock text = new TextBlock();
                    text.Name = "r" + columnIdx + "_" + rowIdx;
                    text.SetValue(Grid.ColumnProperty, columnIdx);
                    text.SetValue(Grid.RowProperty, rowIdx);
                    text.Text = dr[dc.ColumnName];
                    this.grdLayoutRoot.Children.Add(text);
                    columnIdx++;
                }
                rowIdx++;
            }
            #endregion 填充数据
        }
    }
}
