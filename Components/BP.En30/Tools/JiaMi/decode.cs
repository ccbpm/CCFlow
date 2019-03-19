using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DecryptAndEncryptionHelper
{
    public  class decode
    {
        DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
        public string decode_exe(string conString_b)
        {
            return  helper.Decrypto(conString_b);
        }
    


    }
}
