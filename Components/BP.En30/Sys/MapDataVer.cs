using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Sys.Base;
using BP.En;
using System.Collections.Generic;
using System.IO;
using BP.Pub;

namespace BP.Sys
{
    /// <summary>
    /// 表单模板版本管理
    /// </summary>
    public class MapDataVerAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 版本号
        /// </summary>
        public const string Ver = "Ver";
        /// <summary>
        /// 日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 备注
        /// </summary>
        public const string RecNote = "RecNote";

        public const string IsRel = "IsRel";
        /// <summary>
        /// 行数
        /// </summary>
        public const string RowNum = "RowNum";


        public const string AttrsNum = "AttrsNum";
        public const string DtlsNum = "DtlsNum";
        public const string AthsNum = "AthsNum";
        public const string ExtsNum = "ExtsNum";
    }
    /// <summary>
    /// 表单模板版本管理
    /// </summary>
    public class MapDataVer : EntityMyPK
    {
        #region 属性
        public int Ver
        {
            get
            {
                return this.GetValIntByKey(MapDataVerAttr.Ver);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.Ver, value);
            }
        }
        public int IsRel
        {
            get
            {
                return this.GetValIntByKey(MapDataVerAttr.IsRel);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.IsRel, value);
            }
        }
        public int RowNum
        {
            get
            {
                return this.GetValIntByKey(MapDataVerAttr.RowNum);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.RowNum, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MapDataVerAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.FrmID, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(MapDataVerAttr.Rec);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.Rec, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(MapDataVerAttr.RecName);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.RecName, value);
            }
        }
        public string RecNote
        {
            get
            {
                return this.GetValStringByKey(MapDataVerAttr.RecNote);
            }
            set
            {
                this.SetValByKey(MapDataVerAttr.RecNote, value);
            }
        }
        public string RDT
        {
            set
            {
                this.SetValByKey(MapDataVerAttr.RDT, value);
            }
        }
        public int AttrsNum
        {
            set
            {
                this.SetValByKey(MapDataVerAttr.AttrsNum, value);
            }
        }
        public int DtlsNum
        {
            set
            {
                this.SetValByKey(MapDataVerAttr.DtlsNum, value);
            }
        }
        public int AthsNum
        {
            set
            {
                this.SetValByKey(MapDataVerAttr.AthsNum, value);
            }
        }
        public int ExtsNum
        {
            set
            {
                this.SetValByKey(MapDataVerAttr.ExtsNum, value);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 模板版本管理
        /// </summary>
        public MapDataVer()
        {
        }
        public MapDataVer(string mypk)
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

                Map map = new Map("Sys_MapDataVer", "表单模板版本管理");

                map.AddMyPK();
                map.AddTBInt(MapDataVerAttr.Ver, 0, "版本号", true, false);
                map.AddTBInt(MapDataVerAttr.IsRel, 0, "是否主版本?", true, false);
                map.AddTBInt(MapDataVerAttr.RowNum, 0, "行数", true, false);

                map.AddTBString(MapDataVerAttr.FrmID, null, "表单ID", true, false, 0, 50, 20);


                map.AddTBInt(MapDataVerAttr.AttrsNum, 0, "字段数", true, true);
                map.AddTBInt(MapDataVerAttr.DtlsNum, 0, "从表数", true, true);
                map.AddTBInt(MapDataVerAttr.AthsNum, 0, "附件数", true, true);
                map.AddTBInt(MapDataVerAttr.ExtsNum, 0, "逻辑数", true, true);



                map.AddTBString(MapDataVerAttr.Rec, null, "记录人ID", true, false, 0, 50, 20);
                map.AddTBString(MapDataVerAttr.RecName, null, "记录人名称", true, false, 0, 50, 20);
                map.AddTBString(MapDataVerAttr.RecNote, null, "备注", true, false, 0, 500, 20);

                map.AddTBDateTime(MapDataVerAttr.RDT, null, "记录时间", true, false);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.SetValByKey("RDT", DataType.CurrentDateTime);
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            //如果改版本的数据已经在存储表中使用，则不能删除
            MapData md = new MapData(this.FrmID);
            int count = DBAccess.RunSQLReturnValInt("SELECT Count(*) From " + md.PTable + " WHERE AtPara like '%@FrmVer=" + this.Ver + "%'");
            if (count > 0)
            {
                throw new Exception("表单" + md.Name + "版本" + this.Ver + "已经被使用，不能删除");
                return false;
            }

            return base.beforeDelete();
        }

        protected override void afterDelete()
        {
            //删除版本需要删除相关的表单信息
            MapData md = new MapData(this.FrmID+"."+this.Ver);
            md.Delete();
            base.afterDelete();
        }

       
    }
    /// <summary>
    /// 表单模板版本管理s
    /// </summary>
    public class MapDataVers : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 模板版本管理s
        /// </summary>
        public MapDataVers()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDataVer();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDataVer> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDataVer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDataVer> Tolist()
        {
            System.Collections.Generic.List<MapDataVer> list = new System.Collections.Generic.List<MapDataVer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDataVer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
