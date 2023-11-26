using System;
using System.Collections;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
    public class AttrFile
    {
        public string FileNo = null;
        public string FileName = null;
        public AttrFile(string fileno,string filename)
        {
            this.FileNo = fileno;
            this.FileName = filename;
        }
        public AttrFile()
        {
        }
    }
	/// <summary>
	/// 属性集合
	/// </summary>
    [Serializable]
    public class AttrFiles : CollectionBase
    {
        public AttrFiles()
        {
        }
        /// <summary>
        /// 增加文件
        /// </summary>
        /// <param name="fileNo"></param>
        /// <param name="fileName"></param>
        public void Add(string fileNo, string fileName)
        {
            this.InnerList.Add(new AttrFile(fileNo, fileName));
        }
    }	
}
