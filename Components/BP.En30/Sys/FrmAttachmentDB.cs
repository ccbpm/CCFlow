﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 附件数据存储 - 属性
    /// </summary>
    public class FrmAttachmentDBAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 附件
        /// </summary>
        public const string FK_FrmAttachment = "FK_FrmAttachment";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// RefPKVal
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
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
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 是否锁定行
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 上传的GUID
        /// </summary>
        public const string UploadGUID = "UploadGUID";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";

    }
    /// <summary>
    /// 附件数据存储
    /// </summary>
    public class FrmAttachmentDB : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Sort, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentDBAttr.RDT);
                return str.Substring(5,11);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RDT, value);
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public string FileFullName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileFullName);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FileFullName, value.Replace("/","\\"));
            }
        }
        /// <summary>
        /// 上传GUID
        /// </summary>
        public string UploadGUID
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.UploadGUID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.UploadGUID, value);
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
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileName);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FileName, value);
            }
        }
        /// <summary>
        /// 附件扩展名
        /// </summary>
        public string FileExts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileExts);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FileExts, value.Replace(".",""));
            }
        }
        /// <summary>
        /// 相关附件
        /// </summary>
        public string FK_FrmAttachment
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_FrmAttachment);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_FrmAttachment, value);
            }
        }
        /// <summary>
        /// 主键值
        /// </summary>
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RefPKVal, value);
            }
        }
        /// <summary>
        /// 工作ID.
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(FrmAttachmentDBAttr.FID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FID, value);
            }
        }
        /// <summary>
        /// MyNote
        /// </summary>
        public string MyNote
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.MyNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.MyNote, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.Rec);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录人名称
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RecName, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public float FileSize
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentDBAttr.FileSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FileSize, value/1024);
            }
        }
        /// <summary>
        /// 是否锁定行?
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentDBAttr.IsRowLock);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.IsRowLock, value);
            }
        }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentDBAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Idx, value);
            }
        }
        /// <summary>
        /// 附件扩展名
        /// </summary>
        public string NodeID
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.NodeID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public AttachmentUploadType HisAttachmentUploadType
        {
            get
            {

                if (this.MyPK.Contains("_") && this.MyPK.Length < 32)
                    return AttachmentUploadType.Single;
                else
                    return AttachmentUploadType.Multi;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 附件数据存储
        /// </summary>
        public FrmAttachmentDB()
        {
        }
        /// <summary>
        /// 附件数据存储
        /// </summary>
        /// <param name="mypk"></param>
        public FrmAttachmentDB(string mypk)
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
                Map map = new Map("Sys_FrmAttachmentDB");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "附件数据存储";
                map.EnType = EnType.Sys;
                map.AddMyPK();
                map.AddTBString(FrmAttachmentDBAttr.FK_MapData, null,"FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(FrmAttachmentDBAttr.FK_FrmAttachment, null, "附件编号", true, false, 1, 500, 20);
                map.AddTBString(FrmAttachmentDBAttr.RefPKVal, null, "实体主键", true, false, 0, 50, 20);
                map.AddTBInt(FrmAttachmentDBAttr.FID, 0, "FID", true, false);


                map.AddTBString(FrmAttachmentDBAttr.Sort, null, "类别", true, false, 0, 200, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileFullName, null, "文件路径", true, false, 0, 700, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileName, null,"名称", true, false, 0, 500, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileExts, null, "扩展", true, false, 0, 50, 20);
                map.AddTBFloat(FrmAttachmentDBAttr.FileSize, 0, "文件大小", true, false);

                map.AddTBDateTime(FrmAttachmentDBAttr.RDT, null, "记录日期", true, false);
                map.AddTBString(FrmAttachmentDBAttr.Rec, null, "记录人", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentDBAttr.RecName, null, "记录人名字", true, false, 0, 50, 20);
                map.AddTBStringDoc(FrmAttachmentDBAttr.MyNote, null, "备注", true, false);
                map.AddTBString(FrmAttachmentDBAttr.NodeID, null, "节点ID", true, false, 0, 50, 20);

                map.AddTBInt(FrmAttachmentDBAttr.IsRowLock, 0, "是否锁定行", true, false);

                //顺序.
                map.AddTBInt(FrmAttachmentDBAttr.Idx, 0, "排序", true, false);


                //这个值在上传时候产生.
                map.AddTBString(FrmAttachmentDBAttr.UploadGUID, null, "上传GUID", true, false, 0, 200, 20);

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
            return base.beforeInsert();
        }
        #endregion
    }
    /// <summary>
    /// 附件数据存储s
    /// </summary>
    public class FrmAttachmentDBs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件数据存储s
        /// </summary>
        public FrmAttachmentDBs()
        {
        }
        /// <summary>
        /// 附件数据存储s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachmentDBs(string fk_mapdata,string pkval)
        {
            this.Retrieve(FrmAttachmentDBAttr.FK_MapData, fk_mapdata, 
                FrmAttachmentDBAttr.RefPKVal, pkval);
        }
        public FrmAttachmentDBs(string fk_mapdata, Int64 pkval)
        {
            this.Retrieve(FrmAttachmentDBAttr.FK_MapData, fk_mapdata,
                FrmAttachmentDBAttr.RefPKVal, pkval);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachmentDB();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachmentDB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachmentDB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachmentDB> Tolist()
        {
            System.Collections.Generic.List<FrmAttachmentDB> list = new System.Collections.Generic.List<FrmAttachmentDB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachmentDB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
