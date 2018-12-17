using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 图片附件 属性
    /// </summary>
    public class FrmImgAthAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
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
        /// W
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// H
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 控件ID
        /// </summary>
        public const string CtrlID = "CtrlID";
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public const string IsEdit = "IsEdit";
        /// <summary>
        /// 是否必填项
        /// </summary>
        public const string IsRequired = "IsRequired";
    }
    /// <summary>
    /// 图片附件
    /// </summary>
    public class FrmImgAth : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.Name, value);
            }
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string CtrlID
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthAttr.CtrlID);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.CtrlID, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.X);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.X, value);
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.H);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.H, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.W);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.W, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmImgAthAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEdit
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsEdit);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.IsEdit, value);
            }
        }
        /// <summary>
        /// 是否必填，2016-11-1
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsRequired);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.IsRequired, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAth()
        {
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        /// <param name="mypk"></param>
        public FrmImgAth(string mypk)
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
                Map map = new Map("Sys_FrmImgAth", "图片附件");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.AddMyPK();

                map.AddTBString(FrmImgAthAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmImgAthAttr.CtrlID, null, "控件ID", true, false, 0, 200, 20);
                map.AddTBString(FrmImgAthAttr.Name, null, "中文名称", true, false, 0, 200, 20);


                map.AddTBFloat(FrmImgAthAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmImgAthAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmImgAthAttr.H, 200, "H", true, false);
                map.AddTBFloat(FrmImgAthAttr.W, 160, "W", false, false);

                map.AddTBInt(FrmImgAthAttr.IsEdit, 1, "是否可编辑", true, true);
                map.AddTBInt(FrmImgAthAttr.IsRequired, 0, "是否必填项", true, true);
                map.AddTBString(MapAttrAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData +"_"+ this.CtrlID;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 图片附件s
    /// </summary>
    public class FrmImgAths : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 图片附件s
        /// </summary>
        public FrmImgAths()
        {
        }
        /// <summary>
        /// 图片附件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmImgAths(string fk_mapdata)
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
                return new FrmImgAth();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmImgAth> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmImgAth>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmImgAth> Tolist()
        {
            System.Collections.Generic.List<FrmImgAth> list = new System.Collections.Generic.List<FrmImgAth>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmImgAth)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
