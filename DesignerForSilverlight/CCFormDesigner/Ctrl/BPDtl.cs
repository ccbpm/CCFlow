using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Silverlight;

namespace CCForm
{
    public class BPDtl : UCExt, IDelete
    {
     
        public double X
        {
            get
            {
                return Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
            }
            set
            {
                this.SetValue(Canvas.LeftProperty, value);
            }
        }
        public double Y
        {
            get
            {
                return Convert.ToDouble(this.GetValue(Canvas.TopProperty));
            }
            set
            {
                this.SetValue(Canvas.TopProperty, value);
            }
        }

        public DataGrid MyDG = null;

        public BPDtl()
        {
            new Adjust().Bind(this);
            this.BindDrag();
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 400;
            this.Height = 200;
            this.IsSelected = false;
            this.isCanReSize = true;

            this.SizeChanged += new SizeChangedEventHandler((object sender, SizeChangedEventArgs e)=>
            {
                if (MyDG != null)
                {
                    if (!Convert.IsDBNull(this.Width))
                        MyDG.Width = this.Width;
                    if (!Convert.IsDBNull(this.Height))
                        MyDG.Height = this.Height;
                }
            });
        }

        public BPDtl(string name):this()
        {
            this.Name = name;
            this.LoadDtl();
           
        }
        
      
        public void UpdatePos()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                MyDG.Width = this.Width;
                MyDG.Height = this.Height;
            }
        }

      

        #region Method
        public void LoadDtl()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            string sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.Name + "' ORDER BY IDx asc";
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(
                (object sender, FF.RunSQLReturnTableCompletedEventArgs e) =>
                {
                    DataGrid dg = new DataGrid();
                    dg.Name = "DG" + this.Name;
                    DataGridTextColumn mycol = new DataGridTextColumn();
                    mycol.Header = "IDX";
                    dg.Columns.Add(mycol);
                    dg.Width = this.Width;
                    dg.Height = this.Height;
                    this.layout.Child = dg;
                    this.MyDG = dg;

                    if (e.Error != null) return;

                    DataSet ds = new DataSet();
                    ds.FromXml(e.Result);
                    if (ds.Tables[0].Rows.Count > 0)
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (dr["UIVisible"].ToString() == "0")
                                continue;
                            DataGridTextColumn txtColumn = new DataGridTextColumn();
                            txtColumn.Header = dr["Name"];
                            dg.Columns.Add(txtColumn);
                        }
                });
        }

        public void NewDtl()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("NewDtl", this.Name, Glo.FK_MapData, null, null, null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>((object sender, FF.DoTypeCompletedEventArgs e) =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, "提示", MessageBoxButton.OK);
                    return;
                }
                this.LoadDtl();
                Glo.OpenDtl(Glo.FK_MapData, this.Name+ Glo.TimeKey);
            });
        }

        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除明细表[" + this.Name + "]吗？如果确定它相关的字段与设置也将会被删除，物理表也将被删除。", "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;
        

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("DelDtl", this.Name, null, null, null, null, null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(
                (object sender, FF.DoTypeCompletedEventArgs e)=>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(e.Error.Message, "删除错误", MessageBoxButton.OK);
                        return;
                    }
                    Glo.Remove(this);
                });
        }

        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            string sql = "UPDATE Sys_MapDtl SET IsView=0 WHERE No='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
            hidDA.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>((object sender, FF.RunSQLsCompletedEventArgs e)=>
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        #endregion

     
    }
}
