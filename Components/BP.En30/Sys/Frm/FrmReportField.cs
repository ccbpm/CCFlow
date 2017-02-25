using System;
using System.Collections.Generic;
using System.Text;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 表单报表设置属性
    /// </summary>
    public class FrmReportFieldAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 表单编号
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 字段名
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
        /// <summary>
        /// 显示中文名
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 列宽
        /// </summary>
        public const string UIWidth = "UIWidth";
        /// <summary>
        /// 是否显示
        /// </summary>
        public const string UIVisible = "UIVisible";
        /// <summary>
        /// 显示顺序
        /// </summary>
        public const string Idx = "Idx";
    }

    /// <summary>
    /// 表单报表设置数据存储表
    /// </summary>
    public class FrmReportField : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 表单编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmReportFieldAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStrByKey(FrmReportFieldAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 显示中文名
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(FrmReportFieldAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.Name, value);
            }
        }
        /// <summary>
        /// 列宽
        /// </summary>
        public string UIWidth
        {
            get
            {
                return this.GetValStrByKey(FrmReportFieldAttr.UIWidth);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.UIWidth, value);
            }
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool UIVisible
        {
            get
            {
                return this.GetValBooleanByKey(FrmReportFieldAttr.UIVisible);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.UIVisible, value);
            }
        }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmReportFieldAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmReportFieldAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单报表
        /// </summary>
        public FrmReportField()
        {
        }
        /// <summary>
        /// 表单报表
        /// </summary>
        /// <param name="mypk"></param>
        public FrmReportField(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        #endregion

        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_FrmRePortField");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "表单报表";
                map.EnType = EnType.Sys;
                map.AddMyPK();
                map.AddTBString(FrmReportFieldAttr.FK_MapData, null, "表单编号", true, false, 1, 100, 80);
                map.AddTBString(FrmReportFieldAttr.KeyOfEn, null, "字段名", true, false,1,100,100);
                map.AddTBString(FrmReportFieldAttr.Name, null, "显示中文名", true, false, 1, 200, 200);
                map.AddTBString(FrmReportFieldAttr.UIWidth, "0", "宽度", true, false, 1, 100, 100);
                map.AddBoolean(FrmReportFieldAttr.UIVisible, true, "是否显示", true, true);
                map.AddTBInt(FrmReportFieldAttr.Idx, 0, "显示顺序", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            return base.beforeUpdateInsertAction();
        }
    }

    /// <summary>
    /// 表单报表设置数据存储表s
    /// </summary>
    public class FrmReportFields : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 表单报表s
        /// </summary>
        public FrmReportFields()
        {
        }
        /// <summary>
        /// 表单报表s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmReportFields(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmReportFieldAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmReportFieldAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmReportField();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmReportField> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmReportField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmReportField> Tolist()
        {
            System.Collections.Generic.List<FrmReportField> list = new System.Collections.Generic.List<FrmReportField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmReportField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
