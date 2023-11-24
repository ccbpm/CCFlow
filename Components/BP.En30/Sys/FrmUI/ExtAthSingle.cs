using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 字段单附件： 附件属性存储在 FrmAttachmentSingle
    /// </summary>
    public class AthSingle : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.Name);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Name, value);
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                if (BP.Web.WebUser.No.Equals("admin") == true)
                {
                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 字段单附件
        /// </summary>
        public AthSingle()
        {
        }
        /// <summary>
        /// 字段单附件
        /// </summary>
        /// <param name="mypk"></param>
        public AthSingle(string mypk)
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
                Map map = new Map("Sys_MapAttr", "字段单附件");

                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "名称", true, false, 0, 500, 20, true);
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                map.AddTBFloat(MapAttrAttr.UIHeight, 1, "高度", true, false);
                map.AddTBFloat(MapAttrAttr.UIWidth, 1, "宽度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FrmID.Contains("ND") == true)
            {
                string fk_mapData = this.FrmID.Substring(0, this.FrmID.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);

                //删除对应的附件属性.
                FrmAttachment ath = new FrmAttachment();
                ath.MyPK = this.FrmID + "_" + this.KeyOfEn;
                ath.Delete();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);
            base.afterDelete();
        }
        #endregion
    }
    /// <summary>
    /// 字段单附件s
    /// </summary>
    public class AthSingles : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 字段单附件s
        /// </summary>
        public AthSingles()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new AthSingle();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AthSingle> ToJavaList()
        {
            return (System.Collections.Generic.IList<AthSingle>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AthSingle> Tolist()
        {
            System.Collections.Generic.List<AthSingle> list = new System.Collections.Generic.List<AthSingle>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AthSingle)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
