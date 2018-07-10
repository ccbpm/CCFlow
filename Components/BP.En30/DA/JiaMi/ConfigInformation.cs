using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DecryptAndEncryptionHelper
{
    public class ConfigInformation
    {

        private static ConfigInformation _configInformation;

        public ConfigInformation Instance
        {
            get
            {
                if (_configInformation == null)
                {
                    _configInformation = new ConfigInformation();
                }
                return _configInformation;
            }
        }
        // 数据库链接字符串加解密 Key Value
        public static String Key = "27e167e9-2008-33c1-bea0-c8781a9f01cb";
        public static String Vector = "8280d587-33bf-4127-2001-5e0b4b672958";
    }
}
