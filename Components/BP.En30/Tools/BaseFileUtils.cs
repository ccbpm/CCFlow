using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Tools
{
    public class BaseFileUtils
    {
        /// <summary>
        /// 为了java容易翻译,与java的语法保持一致.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] getFiles(string path)
        {
            return System.IO.Directory.GetFiles(path);
        }
    }
}
