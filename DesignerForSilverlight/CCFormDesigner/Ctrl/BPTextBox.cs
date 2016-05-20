using System;
using System.Windows.Input;
using BP.En;
namespace CCForm
{

    public class BPTextBox : TextBoxExt, IRouteEvent, IDelete
    {
      
        public override bool IsCanReSize
        {
            get
            {
                if (this.HisTBType == TBType.DateTime|| this.HisTBType == TBType.Date)
                    return false;

                return true;
            }
        }
        public override bool IsCanDel
        {
            get
            {
                switch (this.Name)
                {
                    case "Title":
                        return false;
                    default:
                        return true;
                }
            }
        }
       
     
        public string NameOfReal = string.Empty;

        private string TBVal = string.Empty;
       
        /// <summary>
        /// 类型
        /// </summary>
        public TBType HisTBType = TBType.String;
        public string HisDataType
        {
            get
            {
                switch (this.HisTBType)
                {
                    case TBType.Float:
                        return DataType.AppFloat;
                    case TBType.Money:
                        return DataType.AppMoney;
                    case TBType.Int:
                        return DataType.AppInt;
                    case TBType.Date:
                        return DataType.AppDate;
                    case TBType.DateTime:
                        return DataType.AppDateTime;
                    case TBType.String:
                    default:
                        return DataType.AppString;
                }
            }
        }
       
        /// <summary>
        /// BPTextBox
        /// </summary>
        public BPTextBox()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
        }
        public BPTextBox(TBType ty):this()
        {
            this.HisTBType = ty;
            this.InitType();
        }
        public BPTextBox(TBType ty, string tbName)
            : this(ty)
        {
            this.NameOfReal = tbName;
            this.Name = tbName;
        }

       
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Text = this.NameOfReal;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.TBVal == null)
                this.Text = "";
            else
                this.Text = this.TBVal;
        }

      

        public void HidIt()
        {
            string sql = "UPDATE Sys_MapAttr SET UIVisible=0 WHERE KeyOfEn='" + this.Name + "' AND FK_MapData='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
            hidDA.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs e)=>
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                };
        }
       
        public void DoCopy()
        {
        }

        public void InitType()
        {
            this.IsReadOnly = true;

            switch (this.HisTBType)
            {
                case TBType.Date:
                    this.Width = 80;
                    this.Height = 23;
                    break;
                case TBType.DateTime:
                    this.Width = 120;
                    this.Height = 23;
                    break;
                case TBType.String:
                    this.Width = 100;
                    this.Height = 23;
                    break;
                case TBType.Money:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0.00";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                case TBType.Int:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                case TBType.Float:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                default:
                    break;
            }
        }
     
    }
}
