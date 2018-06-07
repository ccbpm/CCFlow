using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.Tools;
using Newtonsoft.Json;
using BP.DA;


namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>  
    /// 分析器  
    /// </summary>  
    public class Analyze
    {
        #region Get Function
        /// <summary>  
        /// 发起GET请求  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="requestUrl"></param>  
        /// <returns></returns>  
        public static T Get<T>(String requestUrl) where T : ResultPackage, new()
        {
            String resultJson = RequestHelper.Get(requestUrl);
            return AnalyzeResult<T>(resultJson);
        }
        #endregion

        #region Post Function
        /// <summary>  
        /// 发起POST请求  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="requestUrl"></param>  
        /// <param name="requestParamOfJsonStr"></param>  
        /// <returns></returns>  
        public static T Post<T>(String requestUrl, String requestParamOfJsonStr) where T : ResultPackage, new()
        {
            String resultJson = RequestHelper.Post(requestUrl, requestParamOfJsonStr);
            return AnalyzeResult<T>(resultJson);
        }
        #endregion

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downLoadUrl"></param>
        /// <param name="saveFullName"></param>
        /// <returns></returns>
        public static bool HttpDownLoadFile(string downLoadUrl, string saveFullName)
        {
            return RequestHelper.HttpDownLoadFile(downLoadUrl, saveFullName);
        }

        #region AnalyzeResult
        /// <summary>  
        /// 分析结果  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="resultJson"></param>  
        /// <returns></returns>  
        private static T AnalyzeResult<T>(string resultJson) where T : ResultPackage, new()
        {
            ResultPackage tempResult = null;
            if (!DataType.IsNullOrEmpty(resultJson))
            {
                tempResult = JsonConvert.DeserializeObject<ResultPackage>(resultJson);
            }
            T result = null;
            if (tempResult != null && tempResult.IsOK())
            {
                result = JsonConvert.DeserializeObject<T>(resultJson);
            }
            else if (tempResult != null)
            {
                result = tempResult as T;
            }
            else if (tempResult == null)
            {
                result = new T();
            }

            //结果wei
            if (result == null && tempResult != null)
            {
                result = JsonConvert.DeserializeObject<T>(resultJson);
            }
            result.Json = resultJson;
            return result;
        }
        #endregion
    }
}
