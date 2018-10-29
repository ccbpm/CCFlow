using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class MyFlow : Form
    {
        public MyFlow()
        {
            InitializeComponent();
        }

        private void MyFlow_Load(object sender, EventArgs e)
        {
            string flowNo = "001";
            int fk_node = 0;
            Int64 workid = 0;

            ServiceReference.WindowsFormsApplicationDemoSoapClient clent = new ServiceReference.WindowsFormsApplicationDemoSoapClient();
            clent.HelloWorld();


        }
    }
}
