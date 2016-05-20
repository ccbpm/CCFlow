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
    public class SysDocFileAttr
    {
        /// <summary>
        /// 关联的Table
        /// </summary>
        public const string EnName = "EnName";
        /// <summary>
        /// 关联的key
        /// </summary>
        public const string RefKey = "RefKey";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string RefVal = "RefVal";
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string FileSize = "FileSize";
        /// <summary>
        /// 文件类型
        /// </summary>
        public const string FileType = "FileType";
    }
	public class SysDocFile : EntityMyPK
	{
		#region 实现基本属性
		public string EnName
		{
			get
			{
				return this.GetValStringByKey(SysDocFileAttr.EnName);
			}
			set
			{
				this.SetValByKey(SysDocFileAttr.EnName,value);
			}
		}
		public string RefKey
		{
            get
            {
                return this.GetValStringByKey(SysDocFileAttr.RefKey);
            }
			set
			{
				this.SetValByKey(SysDocFileAttr.RefKey,value);
			}
		}
        public string RefVal
        {
            get
            {
                return this.GetValStringByKey(SysDocFileAttr.RefVal);
            }
            set
            {
                this.SetValByKey(SysDocFileAttr.RefVal, value);
            }
        }
        public string FileName
        {
            get
            {
                return this.GetValStringByKey(SysDocFileAttr.FileName);
            }
            set
            {
                this.SetValByKey(SysDocFileAttr.FileName, value);
            }
        }
		public  int  FileSize
		{
			get
			{
				return this.GetValIntByKey(SysDocFileAttr.FileSize);
			}
			set
			{
				this.SetValByKey(SysDocFileAttr.FileSize,value);
			}
		}
		public  string  FileType
		{
			get
			{
				return this.GetValStringByKey(SysDocFileAttr.FileType);
			}
			set
			{
				this.SetValByKey(SysDocFileAttr.FileType,value);
			}
		}
		#endregion 

        #region 字段
        public string DocHtml
        {
            get
            {
                return DataType.ParseText2Html(this.DocText);
            }
        }
        public string DocText1
        {
            get
            {
                return "";
            }
        }
        public string DocText
        {
            get
            {
                return this.D1 + this.D2 + this.D3 + this.D4 + this.D5 + this.D6 + this.D7 + this.D8 + this.D9 +this.D10 +this.D11 +this.D12 +this.D13 +this.D14 +this.D15 +this.D16+this.D17+this.D18 +this.D19 +this.D20;
            }
            set
            {
                int len = value.Length;
                this.FileSize = len;
                int step = 2000;
                int i = 0;
                int idx = -1;
                while (true)
                {
                    i++;
                    idx++;
                    if (len > step * i)
                    {
                        this.SetValByKey("D" + i, value.Substring(step * idx, step));
                    }
                    else
                    {
                        this.SetValByKey("D" + i, value.Substring(step * idx));
                        break;
                    }
                    if (i > 20)
                        throw new Exception("数据太大存储不下。");
                }
            }
        }
        public string D1
        {
            get
            {
                return this.GetValStrByKey("D1");
            }
            set
            {
                this.SetValByKey("D1", value);
            }
        }
        public string D2
        {
            get
            {
                return this.GetValStrByKey("D2");
            }
            set
            {
                this.SetValByKey("D2", value);
            }
        }
        public string D3
        {
            get
            {
                return this.GetValStrByKey("D3");
            }
            set
            {
                this.SetValByKey("D3", value);
            }
        }
        public string D4
        {
            get
            {
                return this.GetValStrByKey("D4");
            }
            set
            {
                this.SetValByKey("D4", value);
            }
        }
        public string D5
        {
            get
            {
                return this.GetValStrByKey("D5");
            }
            set
            {
                this.SetValByKey("D5", value);
            }
        }
        public string D6
        {
            get
            {
                return this.GetValStrByKey("D6");
            }
            set
            {
                this.SetValByKey("D6", value);
            }
        }
        public string D7
        {
            get
            {
                return this.GetValStrByKey("D7");
            }
            set
            {
                this.SetValByKey("D7", value);
            }
        }
        public string D8
        {
            get
            {
                return this.GetValStrByKey("D8");
            }
            set
            {
                this.SetValByKey("D8", value);
            }
        }
        public string D9
        {
            get
            {
                return this.GetValStrByKey("D9");
            }
            set
            {
                this.SetValByKey("D9", value);
            }
        }

        public string D10
        {
            get
            {
                return this.GetValStrByKey("D10");
            }
            set
            {
                this.SetValByKey("D10", value);
            }
        }
        public string D11
        {
            get
            {
                return this.GetValStrByKey("D11");
            }
            set
            {
                this.SetValByKey("D11", value);
            }
        }
        public string D12
        {
            get
            {
                return this.GetValStrByKey("D12");
            }
            set
            {
                this.SetValByKey("D12", value);
            }
        }
        public string D13
        {
            get
            {
                return this.GetValStrByKey("D13");
            }
            set
            {
                this.SetValByKey("D13", value);
            }
        }
        public string D14
        {
            get
            {
                return this.GetValStrByKey("D14");
            }
            set
            {
                this.SetValByKey("D14", value);
            }
        }
        public string D15
        {
            get
            {
                return this.GetValStrByKey("D15");
            }
            set
            {
                this.SetValByKey("D15", value);
            }
        }
        public string D16
        {
            get
            {
                return this.GetValStrByKey("D16");
            }
            set
            {
                this.SetValByKey("D16", value);
            }
        }
        public string D17
        {
            get
            {
                return this.GetValStrByKey("D17");
            }
            set
            {
                this.SetValByKey("D17", value);
            }
        }
        public string D18
        {
            get
            {
                return this.GetValStrByKey("D18");
            }
            set
            {
                this.SetValByKey("D18", value);
            }
        }
        public string D19
        {
            get
            {
                return this.GetValStrByKey("D19");
            }
            set
            {
                this.SetValByKey("D19", value);
            }
        }
        public string D20
        {
            get
            {
                return this.GetValStrByKey("D20");
            }
            set
            {
                this.SetValByKey("D20", value);
            }
        }
        #endregion

        #region 构造方法
        public SysDocFile()
        {
        }
        public SysDocFile(string pk)
            : base(pk)
        {
        }
        /// <summary>
        /// 注意不初始化数据。
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public SysDocFile(string enName, string key, string val)
        {
            this.MyPK = enName + "@" + key + "@" + val;
        }
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_DocFile");
                map.EnDesc = "备注字段文件管理者";
               // map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBString(SysDocFileAttr.FileName, null, "名称", false, true, 0, 200, 30);
                map.AddTBInt(SysDocFileAttr.FileSize, 0, "大小", true, true);
                map.AddTBString(SysDocFileAttr.FileType, null, "文件类型", true, true, 0, 50, 20);

                map.AddTBString("D1", null, "D1", true, true, 0, 4000, 20);
                map.AddTBString("D2", null, "D2", true, true, 0, 4000, 20);
                map.AddTBString("D3", null, "D3", true, true, 0, 4000, 20);


                if (map.EnDBUrl.DBType == DBType.Oracle || map.EnDBUrl.DBType == DBType.MSSQL)
                {
                    map.AddTBString("D4", null, "D4", true, true, 0, 4000, 20);
                    map.AddTBString("D5", null, "D5", true, true, 0, 4000, 20);
                    map.AddTBString("D6", null, "D6", true, true, 0, 4000, 20);

                    map.AddTBString("D7", null, "D7", true, true, 0, 4000, 20);
                    map.AddTBString("D8", null, "D8", true, true, 0, 4000, 20);
                    map.AddTBString("D9", null, "D9", true, true, 0, 4000, 20);

                    map.AddTBString("D10", null, "D10", true, true, 0, 4000, 20);
                    map.AddTBString("D11", null, "D11", true, true, 0, 4000, 20);

                    map.AddTBString("D12", null, "D12", true, true, 0, 4000, 20);
                    map.AddTBString("D13", null, "D13", true, true, 0, 4000, 20);
                    map.AddTBString("D14", null, "D14", true, true, 0, 4000, 20);
                    map.AddTBString("D15", null, "D15", true, true, 0, 4000, 20);

                    map.AddTBString("D16", null, "D16", true, true, 0, 4000, 20);
                    map.AddTBString("D17", null, "D17", true, true, 0, 4000, 20);
                    map.AddTBString("D18", null, "D18", true, true, 0, 4000, 20);
                    map.AddTBString("D19", null, "D19", true, true, 0, 4000, 20);
                    map.AddTBString("D20", null, "D20", true, true, 0, 4000, 20);
                }

                this._enMap = map;
                return this._enMap;
            }
		}		 
		#endregion 

		#region 共用方法 V2.0 
        public static string GetValHtmlV2(string enName, string pkVal)
        {
            try
            {
                return BP.DA.DataType.ReadTextFile(BP.Sys.SystemConfig.PathOfFDB + enName + "\\" + pkVal + ".fdb");
            }
            catch
            {
                return null;
            }
        }
        public static string GetValTextV2(string enName, string pkVal)
        {
              try
            {
            return BP.DA.DataType.ReadTextFile(BP.Sys.SystemConfig.PathOfFDB   + enName + "\\" + pkVal + ".fdb");
            }
              catch
              {
                  return null;
              }
        }
        public static void SetValV2(string enName, string pkVal, string val)
        {
            try
            {
                string dir = BP.Sys.SystemConfig.PathOfFDB + enName + "\\";
                if (System.IO.Directory.Exists(dir) == false)
                    System.IO.Directory.CreateDirectory(dir);

                BP.DA.DataType.SaveAsFile(dir + "\\" + pkVal + ".fdb", val);
            }
            catch (Exception ex)
            {
                throw ex;
                string filePath = BP.Sys.SystemConfig.PathOfFDB + enName;
                if (System.IO.Directory.Exists(filePath) == false)
                    System.IO.Directory.CreateDirectory(filePath);
            }
        }
		#endregion

        #region 共用方法
        public static string GetValHtmlV1(string enName, string pkVal)
        {
            SysDocFile sdf = new SysDocFile();
            sdf.MyPK = enName + "@Doc@" + pkVal;
            sdf.RetrieveFromDBSources();
            return sdf.DocHtml;
        }
        public static string GetValTextV1(string enName, string pkVal)
        {
            SysDocFile sdf = new SysDocFile();
            sdf.MyPK = enName + "@Doc@" + pkVal;
            sdf.RetrieveFromDBSources();
            return sdf.DocText;
        }
        public static void SetValV1(string enName, string pkVal, string val)
        {
            SysDocFile sdf = new SysDocFile();
            sdf.MyPK = enName + "@Doc@" + pkVal;
            sdf.FileSize = val.Length;
            sdf.DocText = val;
            sdf.Save();
        }

        public void UpdateLoadFileOfAccess(string FileName)
        {
            //FileInfo fi = new FileInfo( FileName );// Replace with your file name
            //if (fi.Exists==false)
            //    throw new Exception("文件已经不存在。");

            //this.FileSize=fi.Length.ToString();
            //this.FileName = fi.FullName;
            //this.Name=fi.Name;
            //this.Insert();

            //byte[] bData = null;
            ////int nNewFileID = 0;
            //// Read file data into buffer
            //using ( FileStream fs = fi.OpenRead() )
            //{
            //    bData = new byte[fi.Length];
            //    int nReadLength = fs.Read( bData,0, (int)(fi.Length) );
            //}

            ////			// Add file info into DB
            ////			string strQuery = "INSERT INTO FileInfo " 
            ////				+ " ( FileName, FullName, FileData ) "
            ////				+ " VALUES "
            ////				+ " ( @FileName, @FullName, @FileData ) "
            ////				+ " SELECT @@IDENTITY AS 'Identity'";

            //string strQuery="UPDATE Sys_FileManager SET FileData=@FileData WHERE OID="+this.OID;
            //OleDbConnection conn = (OleDbConnection)BP.DA.DBAccess.GetAppCenterDBConn ;
            //conn.Open();

            //OleDbCommand sqlComm = new OleDbCommand( strQuery, 
            //    conn);

            ////sqlComm.Parameters.Add( "@FileName", fi.Name );
            ////sqlComm.Parameters.Add( "@FullName", fi.FullName );
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
            ////				//ListViewItem itmNew = lsvFileInfo.Items.Add( fi.Name );
            ////				//itmNew.Tag = nNewFileID;
            ////			}
        }
        #endregion
	}
	/// <summary>
	/// 文件管理者 
	/// </summary>
	public class SysDocFiles :Entities
	{
		public SysDocFiles(){}
		public SysDocFiles(string _tableName, string _key)
		{
			QueryObject qo = new QueryObject(this); 
			qo.AddWhere(SysDocFileAttr.EnName,_tableName);
			qo.addAnd();
			qo.AddWhere(SysDocFileAttr.RefKey,_key);
			qo.DoQuery();
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new SysDocFile();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SysDocFile> ToJavaList()
        {
            return (System.Collections.Generic.IList<SysDocFile>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SysDocFile> Tolist()
        {
            System.Collections.Generic.List<SysDocFile> list = new System.Collections.Generic.List<SysDocFile>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SysDocFile)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
