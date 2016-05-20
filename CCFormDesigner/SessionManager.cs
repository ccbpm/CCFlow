using System;
using System.Collections.Generic;

namespace BP
{
    public class SessionManager
    {
        private static Dictionary<string, string> session = new Dictionary<string, string>();
        public static Dictionary<string, string> Session
        {
            get { return session; }
            set { session = value; }
        }
    }
}