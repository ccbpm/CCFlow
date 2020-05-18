using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 表单元素扩展DB
    /// </summary>
    public class FrmEleDBAttr : EntityMyPKAttr
    {
        /// <summary>
        /// RefPKVal
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// EleID
        /// </summary>
        public const string EleID = "EleID";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// Tag1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// Tag2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// Tag3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// Tag4
        /// </summary>
        public const string Tag4 = "Tag4";
        /// <summary>
        /// Tag5
        /// </summary>
        public const string Tag5 = "Tag5";
    }
    /// <summary>
    /// 表单元素扩展DB
    /// </summary>
    public class FrmEleDB : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// EleID
        /// </summary>
        public string EleID
        {
            get
            {
                return this.GetValStrByKey(FrmEleDBAttr.EleID);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.EleID, value);
            }
        }
        /// <summary>
        /// Tag1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(FrmEleDBAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.Tag1, value);
            }
        }
        /// <summary>
        /// Tag2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(FrmEleDBAttr.Tag2);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.Tag2, value);
            }
        }
        /// <summary>
        /// Tag3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(FrmEleDBAttr.Tag3);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.Tag3, value);
            }
        }
        /// <summary>
        /// Tag4
        /// </summary>
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(FrmEleDBAttr.Tag4);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.Tag4, value);
            }
        }
        /// <summary>
        /// Tag5
        /// </summary>
        public string Tag5
        {
            get
            {
                return this.GetValStringByKey(FrmEleDBAttr.Tag5);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.Tag5, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmEleDBAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// RefPKVal
        /// </summary>
        public string RefPKVal
        {
            get
            {
                return this.GetValStrByKey(FrmEleDBAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.RefPKVal, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(FrmEleDBAttr.FID);
            }
            set
            {
                this.SetValByKey(FrmEleDBAttr.FID, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单元素扩展DB
        /// </summary>
        public FrmEleDB()
        {
        }
        /// <summary>
        /// 表单元素扩展DB
        /// </summary>
        /// <param name="mypk"></param>
        public FrmEleDB(string mypk)
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
                Map map = new Map("Sys_FrmEleDB","表单元素扩展DB");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = FrmEleDBAttr.RefPKVal;

                map.AddMyPK();
                map.AddTBString(FrmEleDBAttr.FK_MapData, null, "FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(FrmEleDBAttr.EleID, null, "EleID", true, false, 0, 50, 20);
                map.AddTBString(FrmEleDBAttr.RefPKVal, null, "RefPKVal", true, false, 0, 50, 20);
                map.AddTBInt(FrmEleDBAttr.FID, 0, "FID", false, true);
                map.AddTBString(FrmEleDBAttr.Tag1, null, "Tag1", true, false, 0, 1000, 20);
                map.AddTBString(FrmEleDBAttr.Tag2, null, "Tag2", true, false, 0, 1000, 20);
                map.AddTBString(FrmEleDBAttr.Tag3, null, "Tag3", true, false, 0, 1000, 20);
                map.AddTBString(FrmEleDBAttr.Tag4, null, "Tag4", true, false, 0, 1000, 20);
                map.AddTBString(FrmEleDBAttr.Tag5, null, "Tag5", true, false, 0, 1000, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //this.MyPK = this.FK_MapData + "_" + this.EleID + "_" + this.RefPKVal;
           // this.GenerPKVal();
            return base.beforeUpdateInsertAction();
        }
        public void GenerPKVal()
        {
            this.MyPK = this.FK_MapData + "_" + this.EleID + "_" + this.RefPKVal;
        }
    }
    /// <summary>
    /// 表单元素扩展DBs
    /// </summary>
    public class FrmEleDBs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 表单元素扩展DBs
        /// </summary>
        public FrmEleDBs()
        {
        }
        /// <summary>
        /// 表单元素扩展DBs
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="pkval"></param>
        public FrmEleDBs(string fk_mapdata, string pkval)
        {
            this.Retrieve(FrmEleDBAttr.FK_MapData, fk_mapdata, FrmEleDBAttr.EleID, pkval);
        }
        /// <summary>
        /// 表单元素扩展DBs
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmEleDBs(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmEleDB();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmEleDB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmEleDB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmEleDB> Tolist()
        {
            System.Collections.Generic.List<FrmEleDB> list = new System.Collections.Generic.List<FrmEleDB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmEleDB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
