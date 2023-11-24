using System;

namespace BP.Port.DINGTalk.DDSDK
{
    public class ResultPackage
    {
        /// <summary>  
        /// 错误码  
        /// </summary>  
        public ErrCodeEnum ErrCode { get; set; }

        /// <summary>  
        /// 错误消息  
        /// </summary>  
        public string ErrMsg { get; set; }

        /// <summary>  
        /// 结果的json形式  
        /// </summary>  
        public String Json { get; set; }


        #region IsOK Function
        public bool ItIsOK()
        {
            return ErrCode == ErrCodeEnum.OK;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            String info = "{ErrCode:" + this.ErrCode + ",ErrMsg:" + this.ErrMsg + "}";
            return info;
        }
        #endregion
    }
}
