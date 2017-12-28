using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BP.WF
{
    public class FrmNode
    {
        public string MyPK = null;
        public string No = null;
        public string Name = null;
        public string PTable = null;
        public string URL = null;
        public int FK_Node = 0;
        public int FormType = 0;
        public bool IsReadonly = false;
        public string FK_Frm = null;
        public FrmNode(string strs)
        {
            string[] kvs = strs.Split('@');
            foreach (string k in kvs)
            {
                if (k == null || k == "")
                    continue;
                string[] fv = k.Split('=');
                switch (fv[0])
                {
                    case "FK_Node":
                        this.FK_Node = int.Parse(fv[1]);
                        break;
                    case "FK_Frm":
                        this.FK_Frm = fv[1];
                        break;
                    case "No":
                        this.No = fv[1];
                        break;
                    case "Name":
                        this.Name = fv[1];
                        break;
                    case "PTable":
                        this.PTable = fv[1];
                        break;
                    case "URL":
                        this.URL = fv[1];
                        break;
                    case "IsReadonly":

                        if (fv[1] == "1")
                            this.IsReadonly = true;
                        else
                            this.IsReadonly = false;
                        break;
                    case "FormType":
                        this.FormType = int.Parse(fv[1]);
                        break;
                }
            }
        }

    }
}
