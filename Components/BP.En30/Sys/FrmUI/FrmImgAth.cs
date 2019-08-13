using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
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

        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                uac.IsUpdate = true;
                return uac;
            }
        }
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
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;

                map.AddMyPK();

                map.AddTBString(FrmImgAthAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(FrmImgAthAttr.CtrlID, null, "控件ID", true, true, 0, 200, 20);
                map.AddTBString(FrmImgAthAttr.Name, null, "中文名称", true, false, 0, 200, 20);

                map.AddTBFloat(FrmImgAthAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmImgAthAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmImgAthAttr.H, 200, "H", true, false);
                map.AddTBFloat(FrmImgAthAttr.W, 160, "W", false, false);

                map.AddBoolean(FrmImgAthAttr.IsEdit, true, "是否可编辑", true, true);
                //map.AddTBInt(FrmImgAthAttr.IsEdit, 1, "是否可编辑", true, true);
                map.AddBoolean(FrmImgAthAttr.IsRequired, false, "是否必填项", true, true);
                //map.AddTBInt(FrmImgAthAttr.IsRequired, 0, "是否必填项", true, true);
                //map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, true, 0, 128, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //this.MyPK = this.FK_MapData + "_" + this.CtrlID;
            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmImgAth imgAth = new BP.Sys.FrmImgAth();
            imgAth.MyPK = this.MyPK;
            imgAth.RetrieveFromDBSources();
            imgAth.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
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
