using System;
using System.IO;
using System.Collections;
using BP.DA;
using BP.En;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace BP.Sys
{
    /// <summary>
    /// 文件管理属性
    /// </summary>
    public class SysFileManagerAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 上传日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// EnName
        /// </summary>
        public const string EnName = "EnName";
        /// <summary>
        /// 关联的key
        /// </summary>
        public const string RefVal = "RefVal";
        /// <summary>
        /// 文件路径
        /// </summary>
        public const string MyFilePath = "MyFilePath";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string MyFileSize = "MyFileSize";
        /// <summary>
        /// MyFileExt
        /// </summary>
        public const string MyFileExt = "MyFileExt";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 高度（如果是图片）
        /// </summary>
        public const string MyFileH = "MyFileH";
        /// <summary>
        /// 宽度（如果是图片）
        /// </summary>
        public const string MyFileW = "MyFileW";
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string MyFileName = "MyFileName";
        /// <summary>
        /// 内容
        /// </summary>
        public const string Doc = "Doc";
        public const string AttrFileName = "AttrFileName";
        public const string AttrFileNo = "AttrFileNo";
        public const string WebPath = "WebPath";
    }
    /// <summary>
    /// 文件管理者
    /// </summary>
    public class SysFileManager : EntityOID
    {
        #region 实现基本属性
        /// <summary>
        /// 
        /// </summary>
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.WebPath);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.WebPath, value);
            }
        }
        public string AttrFileNo
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.AttrFileNo);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.AttrFileNo, value);
            }
        }

        public string AttrFileName
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.AttrFileName);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.AttrFileName, value);
            }
        }

        public string MyFileName
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.MyFileName);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFileName, value);
            }
        }
        public string MyFileWebUrl
        {
            get
            {
                return this.WebPath;
            }
        }

        public string MyFileExt
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.MyFileExt);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFileExt, value);
            }
        }
 

        public string Rec
        {
            get
            {
                string s = this.GetValStringByKey(SysFileManagerAttr.Rec);
                if (s == null || s == "")
                    return null;
                return s;
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.Rec, value);
            }
        }

        public string RecText
        {
            get
            {
                return this.GetValRefTextByKey(SysFileManagerAttr.Rec);
            }
        }
        public string EnName
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.EnName);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.EnName, value);
            }
        }
        public object RefVal
        {
            get
            {
                return this.GetValByKey(SysFileManagerAttr.RefVal);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.RefVal, value);
            }
        }
        public string MyFilePath
        {
            get
            {
                return  this.GetValStringByKey(SysFileManagerAttr.MyFilePath);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFilePath, value);
            }
        }
        public int MyFileH
        {
            get
            {
                return this.GetValIntByKey(SysFileManagerAttr.MyFileH);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFileH, value);
            }
        }
        public int MyFileW
        {
            get
            {
                return this.GetValIntByKey(SysFileManagerAttr.MyFileW);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFileW, value);
            }
        }
        public float MyFileSize
        {
            get
            {
                return this.GetValIntByKey(SysFileManagerAttr.MyFileSize);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.MyFileSize, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.RDT);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.RDT, value);
            }
        }
        public string Note
        {
            get
            {
                return this.GetValStringByKey(SysFileManagerAttr.Note);
            }
            set
            {
                this.SetValByKey(SysFileManagerAttr.Note, value);
            }
        }
        #endregion

        #region 构造方法
        public SysFileManager()
        {
        }
        /// <summary>
        /// 文件管理者
        /// </summary>
        /// <param MyFileName="_OID"></param>
        public SysFileManager(int _OID)
            : base(_OID)
        {
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_FileManager", "文件管理者");

                map.AddTBIntPKOID();
                map.AddTBString(SysFileManagerAttr.AttrFileName, null, "指定名称", true, false, 0, 50, 20);
                map.AddTBString(SysFileManagerAttr.AttrFileNo, null, "指定编号", true, false, 0, 50, 20);

                map.AddTBString(SysFileManagerAttr.EnName, null, "关联的表", false, true, 1, 50, 20);
                map.AddTBString(SysFileManagerAttr.RefVal, null, "主键值", false, true, 1, 50, 10);
                map.AddTBString(SysFileManagerAttr.WebPath, null, "Web路径", false, true, 0, 100, 30);

                map.AddMyFile("文件名称");

                //map.AddTBString(SysFileManagerAttr.MyFileName, null, "文件名称", true, false, 1, 50, 20);
                //map.AddTBInt(SysFileManagerAttr.MyFileSize, 0, "文件大小", true, true);
                //map.AddTBInt(SysFileManagerAttr.MyFileH, 0, "Img高度", true, true);
                //map.AddTBInt(SysFileManagerAttr.MyFileW, 0, "Img宽度", true, true);
                //map.AddTBString(SysFileManagerAttr.MyFileExt, null, "文件类型", true, true, 0, 50, 20);

                map.AddTBString(SysFileManagerAttr.RDT, null, "上传时间", true, true, 1, 50, 20);
                map.AddTBString(SysFileManagerAttr.Rec, null, "上传人", true, true, 0, 50, 20);
                map.AddTBStringDoc();
                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeInsert()
        {
           this.Rec = BP.Web.WebUser.No;
           this.RDT = DataType.CurrentDataTime;
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            if (this.Rec == Web.WebUser.No)
            {
                return base.beforeDelete();
            }
            return base.beforeDelete();
        }
        #endregion

        #region　共用方法
        public void UpdateLoadFileOfAccess(string MyFilePath)
        {
            //FileInfo fi = new FileInfo(MyFilePath);// Replace with your file MyFileName
            //if (fi.Exists == false)
            //    throw new Exception("文件已经不存在。");

            //this.MyFileSize =int.Parse( fi.Length.ToString());
            //this.MyFilePath = fi.FullMyFileName;
            //this.MyFileName = fi.MyFileName;
            //this.Insert();

            //byte[] bData = null;
            ////int nNewFileID = 0;
            //// Read file data into buffer
            //using (FileStream fs = fi.OpenRead())
            //{
            //    bData = new byte[fi.Length];
            //    int nReadLength = fs.Read(bData, 0, (int)(fi.Length));
            //}

            ////			// Add file info into DB
            ////			string strQuery = "INSERT INTO FileInfo " 
            ////				+ " ( FileMyFileName, FullMyFileName, FileData ) "
            ////				+ " VALUES "
            ////				+ " ( @FileMyFileName, @FullMyFileName, @FileData ) "
            ////				+ " SELECT @@IDENTITY AS 'Identity'";

            //string strQuery = "UPDATE Sys_FileManager SET FileData=@FileData WHERE OID=" + this.OID;
            //OleDbConnection conn = (OleDbConnection)BP.DA.DBAccess.GetAppCenterDBConn;
            //conn.Open();

            //OleDbCommand sqlComm = new OleDbCommand(strQuery,
            //    conn);

            ////sqlComm.Parameters.Add( "@FileMyFileName", fi.MyFileName );
            ////sqlComm.Parameters.Add( "@FullMyFileName", fi.FullMyFileName );
            //sqlComm.Parameters.AddWithValue("@FileData", bData);
            //sqlComm.ExecuteNonQuery();

            //// Get new file ID
            ////	SqlDataReader sqlReader = sqlComm.ExecuteReader(); 
            ////			if( sqlReader.Read() )
            ////			{
            ////				nNewFileID = int.Parse(sqlReader.GetValue(0).ToString());
            ////			}
            ////
            ////			sqlReader.Close();
            ////			sqlComm.Dispose();
            ////
            ////			if( nNewFileID > 0 )
            ////			{
            ////				// Add new item in list view
            ////				//ListViewItem itmNew = lsvFileInfo.Items.Add( fi.MyFileName );
            ////				//itmNew.Tag = nNewFileID;
            ////			}
        }
        #endregion
    }
	/// <summary>
	/// 文件管理者 
	/// </summary>
    public class SysFileManagers : EntitiesOID
	{
        /// <summary>
        /// 文件管理者
        /// </summary>
		public SysFileManagers()
        {
        }
        /// <summary>
        /// 文件管理者
        /// </summary>
        /// <param name="EnName"></param>
        /// <param name="refval"></param>
        public SysFileManagers(string EnName, string refval)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(SysFileManagerAttr.EnName, EnName);
            qo.addAnd();
            qo.AddWhere(SysFileManagerAttr.RefVal, refval);
            qo.DoQuery();
        }
        /// <summary>
        /// 文件管理者
        /// </summary>
        /// <param name="EnName"></param>
        public SysFileManagers(string EnName)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(SysFileManagerAttr.EnName, EnName);
            qo.DoQuery();
        }
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new SysFileManager();
			}
		}
        public SysFileManager GetSysFileByAttrFileNo(string key)
        {
            foreach (SysFileManager en in this)
            {
                if (en.AttrFileNo == key)
                    return en;
            }
            return null;
        }
	}
}
