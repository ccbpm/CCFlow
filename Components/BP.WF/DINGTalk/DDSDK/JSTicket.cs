﻿
namespace BP.GPM.DTalk.DDSDK
{
    /// <summary>  
    /// JSAPI时用的票据  
    /// </summary>  
    public class JSTicket : ResultPackage
    {
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }
}
