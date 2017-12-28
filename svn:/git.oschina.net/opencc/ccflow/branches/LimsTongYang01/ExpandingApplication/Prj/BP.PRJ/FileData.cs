using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.Port; 
using BP.En;

namespace BP.PRJ
{
    /// <summary>
    /// 文件数据描述
    /// </summary>
    public class FileDataAttr : EntityNoNameAttr
    {
        /// <summary>
        /// My_PK
        /// </summary>
        public const string My_PK = "My_PK";
        /// <summary>
        /// 编号
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 文件类型
        /// </summary>
        public const string FileType = "FileType";
        /// <summary>
        /// 文件名
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 路径
        /// </summary>
        public const string AbsolutionPath = "AbsolutionPath";
        /// <summary>
        /// 文件格式
        /// </summary>
        public const string FileFormat = "FileFormat";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string FileSize = "FileSize";
        /// <summary>
        /// 上传日期
        /// </summary>
        public const string UpLoadDate = "UpLoadDate";
        /// <summary>
        /// 上传人
        /// </summary>
        public const string UpLoadPerson = "UpLoadPerson";
        /// <summary>
        /// 上传节点名称
        /// </summary>
        public const string NodeAttrbute = "NodeAttrbute";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FlowID = "FlowID";
    }
    /// <summary>
    /// 文件数据表
    /// </summary>
    public class FileData : EntityNoName
    {
        /// <summary>
        /// FileData
        /// </summary>
        public FileData()
        {
        }
        /// <summary>
        /// 文件描述
        /// </summary>
        public FileData(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("PRJ_FileData");
                map.EnDesc = "文件数据表";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.CodeStruct = "2";
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(FileDataAttr.My_PK, null, "My_PK", true, false, 10, 10, 10);
                map.AddTBInt(FileDataAttr.OID, 0, "OID", true, false);
                map.AddTBString(FileDataAttr.FileName, null, "文件名", true, false, 0, 60, 500);
                map.AddTBString(FileDataAttr.AbsolutionPath, null, "路径", true, false, 0, 60, 500);
                map.AddTBString(FileDataAttr.FileFormat, null, "文件格式", true, false, 0, 60, 500);
                map.AddTBString(FileDataAttr.FileSize, null, "文件大小", true, false, 0, 60, 500);
                map.AddTBDate(FileDataAttr.UpLoadDate, null, "上传日期", true, false);
                map.AddTBString(FileDataAttr.UpLoadPerson, null, "上传人", true, false, 0, 60, 500);
                map.AddTBString(FileDataAttr.NodeAttrbute, null, "上传节点名称", true, false, 0, 60, 500);
                map.AddTBInt(FileDataAttr.FlowID, 0, "FlowID", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
}
