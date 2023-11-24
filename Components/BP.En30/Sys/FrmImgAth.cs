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
        public const string FrmID = "FK_MapData";
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
        /// <summary>
        /// 显示分组
        /// </summary>
        public const string GroupID = "GroupID";
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
        }
        public void setName(bool val)
        {
            this.SetValByKey(FrmImgAthAttr.Name, val);
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
        public void setCtrlID(string val)
        {
            this.SetValByKey(FrmImgAthAttr.CtrlID, val);
        }
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
        public void setH(float val)
        {
            this.SetValByKey(FrmImgAthAttr.H, val);
        }

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
        public void setW(float val)
        {
            this.SetValByKey(FrmImgAthAttr.W, val);
        }

        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(FrmImgAthAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.FrmID, value);
            }
        }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool ItIsEdit
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsEdit);
            }
        }
        public void setIsEdit(bool val)
        {
            this.SetValByKey(FrmImgAthAttr.IsEdit, val);
        }
        /// <summary>
        /// 是否必填，2016-11-1
        /// </summary>
        public bool ItIsRequired
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsRequired);
            }
        }
        public void setIsRequired(bool val)
        {
            this.SetValByKey(FrmImgAthAttr.IsRequired, val);
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
            this.setMyPK(mypk);
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
                map.IndexField = FrmImgAttr.FrmID;

                map.AddMyPK();

                map.AddTBString(FrmImgAthAttr.FrmID, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmImgAthAttr.CtrlID, null, "控件ID", true, false, 0, 200, 20);
                map.AddTBString(FrmImgAthAttr.Name, null, "中文名称", true, false, 0, 200, 20);

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
            this.MyPK = this.FrmID + "_" + this.CtrlID;
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            if (attr.RetrieveFromDBSources() == 1)
            {
                attr.setName(this.Name);
                attr.Update();
            }

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
            this.Retrieve(MapAttrAttr.FK_MapData, fk_mapdata);
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
