using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 单选框
    /// </summary>
    public class FrmRBAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// KeyOfEn
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
        /// <summary>
        /// IntKey
        /// </summary>
        public const string IntKey = "IntKey";
        /// <summary>
        /// EnumKey
        /// </summary>
        public const string EnumKey = "EnumKey";
        /// <summary>
        /// 标签
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 单选框
    /// </summary>
    public class FrmRB : EntityMyPK
    {
        #region 属性
        public string Lab
        {
            get
            {
                return this.GetValStringByKey(FrmRBAttr.Lab);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.Lab, value);
            }
        }
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(FrmRBAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.KeyOfEn, value);
            }
        }
        public int IntKey
        {
            get
            {
                return this.GetValIntByKey(FrmRBAttr.IntKey);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.IntKey, value);
            }
        }
        /// <summary>
        ///  Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmRBAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.Y, value);
            }
        }
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmRBAttr.X);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.X, value);
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmRBAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.FK_MapData, value);
            }
        }
        public string EnumKey
        {
            get
            {
                return this.GetValStrByKey(FrmRBAttr.EnumKey);
            }
            set
            {
                this.SetValByKey(FrmRBAttr.EnumKey, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单选框
        /// </summary>
        public FrmRB()
        {
        }
        public FrmRB(string mypk)
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
                Map map = new Map("Sys_FrmRB");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "单选框";
                map.EnType = EnType.Sys;

                map.AddMyPK();
                map.AddTBString(FrmRBAttr.FK_MapData, null, "FK_MapData", true, false, 0, 100, 20);
                map.AddTBString(FrmRBAttr.KeyOfEn, null, "KeyOfEn", true, false, 0, 30, 20);
                map.AddTBString(FrmRBAttr.EnumKey, null, "EnumKey", true, false, 0, 30, 20);
                map.AddTBString(FrmRBAttr.Lab, null, "Lab", true, false, 0, 90, 20);
                map.AddTBInt(FrmRBAttr.IntKey, 0, "IntKey", true, false);

                map.AddTBFloat(FrmRBAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmRBAttr.Y, 5, "Y", false, false);
                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, false, 0, 128, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_MapData + "_" + this.KeyOfEn + "_" + this.IntKey;
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.KeyOfEn + "_" + this.IntKey;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 单选框s
    /// </summary>
    public class FrmRBs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 单选框s
        /// </summary>
        public FrmRBs()
        {
        }
        /// <summary>
        /// 单选框s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmRBs(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 单选框s
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="keyOfEn">字段</param>
        public FrmRBs(string fk_mapdata, string keyOfEn)
        {
            this.Retrieve(FrmRBAttr.FK_MapData, fk_mapdata, FrmRBAttr.KeyOfEn, keyOfEn);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmRB();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmRB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmRB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmRB> Tolist()
        {
            System.Collections.Generic.List<FrmRB> list = new System.Collections.Generic.List<FrmRB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmRB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
