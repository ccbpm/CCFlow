using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 剪切图片附件数据存储 - 属性
    /// </summary>
    public class FrmImgAthDBAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 附件
        /// </summary>
        public const string FK_FrmImgAth = "FK_FrmImgAth";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// RefPKVal
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 文件扩展
        /// </summary>
        public const string FileExts = "FileExts";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string FileSize = "FileSize";
        /// <summary>
        /// 保存到
        /// </summary>
        public const string FileFullName = "FileFullName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名字
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 类别
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 备注
        /// </summary>
        public const string MyNote = "MyNote";
    }
    /// <summary>
    /// 剪切图片附件数据存储
    /// </summary>
    public class FrmImgAthDB : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.Sort, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.RDT);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.RDT, value);
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public string FileFullName
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.FileFullName);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FileFullName, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string FilePathName
        {
            get
            {
                return this.FileFullName.Substring(this.FileFullName.LastIndexOf('\\') + 1);
            }
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.FileName);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FileName, value);
            }
        }
        /// <summary>
        /// 附件扩展名
        /// </summary>
        public string FileExts
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.FileExts);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FileExts, value.Replace(".",""));
            }
        }
        /// <summary>
        /// 相关附件
        /// </summary>
        public string FK_FrmImgAth
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.FK_FrmImgAth);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FK_FrmImgAth, value);
            }
        }
        /// <summary>
        /// 主键值
        /// </summary>
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.RefPKVal, value);
            }
        }
        /// <summary>
        /// MyNote
        /// </summary>
        public string MyNote
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.MyNote);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.MyNote, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.Rec);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录人名称
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.RecName, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthDBAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public float FileSize
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthDBAttr.FileSize);
            }
            set
            {
                this.SetValByKey(FrmImgAthDBAttr.FileSize, value/1024);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 剪切图片附件数据存储
        /// </summary>
        public FrmImgAthDB()
        {
        }
        /// <summary>
        /// 剪切图片附件数据存储
        /// </summary>
        /// <param name="mypk"></param>
        public FrmImgAthDB(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FrmImgAthDB", "剪切图片附件数据存储");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.EnDesc = "剪切图片附件数据存储";
                map.Java_SetEnType(EnType.Sys);

                map.IndexField = FrmImgAthDBAttr.RefPKVal; 


                map.AddMyPK();

                // 以下三个字段组成一个主键. FK_FrmImgAth+"_"+RefPKVal
                map.AddTBString(FrmImgAthDBAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmImgAthDBAttr.FK_FrmImgAth, null, "图片附件编号", true, false, 1, 50, 20);
                map.AddTBString(FrmImgAthDBAttr.RefPKVal, null, "实体主键", true, false, 1, 50, 20);

                map.AddTBString(FrmImgAthDBAttr.FileFullName, null, "文件全路径", true, false, 0, 700, 20);
                map.AddTBString(FrmImgAthDBAttr.FileName, null, "名称", true, false, 0, 500, 20);
                map.AddTBString(FrmImgAthDBAttr.FileExts, null, "扩展名", true, false, 0, 50, 20);
                map.AddTBFloat(FrmImgAthDBAttr.FileSize, 0, "文件大小", true, false);

                map.AddTBDateTime(FrmImgAthDBAttr.RDT, null, "记录日期", true, false);
                map.AddTBString(FrmImgAthDBAttr.Rec, null, "记录人", true, false, 0, 50, 20);
                map.AddTBString(FrmImgAthDBAttr.RecName, null, "记录人名字", true, false, 0, 50, 20);
                map.AddTBStringDoc(FrmImgAthDBAttr.MyNote, null, "备注", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 重写
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_FrmImgAth + "_" + this.RefPKVal;
            return base.beforeInsert();
        }
        /// <summary>
        /// 重写
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            this.MyPK = this.FK_FrmImgAth + "_" + this.RefPKVal;
            return base.beforeUpdate();
        }
        #endregion
    }
    /// <summary>
    /// 剪切图片附件数据存储s
    /// </summary>
    public class FrmImgAthDBs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 剪切图片附件数据存储s
        /// </summary>
        public FrmImgAthDBs()
        {
        }

        public FrmImgAthDBs(string fk_mapdata)
        {
            this.Retrieve(FrmImgAthDBAttr.FK_MapData, fk_mapdata);
        }
        /// <summary>
        /// 剪切图片附件数据存储s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmImgAthDBs(string fk_mapdata,string pkval)
        {
            this.Retrieve(FrmImgAthDBAttr.FK_MapData, fk_mapdata, 
                FrmImgAthDBAttr.RefPKVal, pkval);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmImgAthDB();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmImgAthDB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmImgAthDB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmImgAthDB> Tolist()
        {
            System.Collections.Generic.List<FrmImgAthDB> list = new System.Collections.Generic.List<FrmImgAthDB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmImgAthDB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
