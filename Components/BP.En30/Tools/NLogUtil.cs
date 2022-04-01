using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Tools
{
    public class NLogUtil
    {
        public static NLog.Logger _logger
        {
            get
            {
                return NLog.LogManager.GetCurrentClassLogger();
            }
        }
    }
}
