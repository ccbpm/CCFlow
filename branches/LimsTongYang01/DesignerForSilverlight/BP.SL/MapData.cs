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
using BP.En;

namespace BP.Sys.SL
{
    public class MapDataAttr
    {
        public const string No = "No";
        public const string Name = "Name";
    }
    public class MapData : BP.En.Entity
    {
        public string No = null;
        public string Name = null;
        public double FrmH = 1200;
        public double FrmW = 900;
        public MapData()
        {
        }
    }
}
