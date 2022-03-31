using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 超连接
    /// </summary>
    public class ExtLink : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 目标
        /// </summary>
        public string Target
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag1);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag1, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string URL
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag2).Replace("#", "@");
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag2, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
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
                if (BP.Web.WebUser.No.Equals("admin")==true)
                {

                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }

                return uac;
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public ExtLink()
        {
        }
        /// <summary>
        /// 超连接
        /// </summary>
        /// <param name="mypk"></param>
        public ExtLink(string mypk)
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
                Map map = new Map("Sys_MapAttr", "超连接");
                map.IndexField = MapAttrAttr.FK_MapData;

                #region 通用的属性.
                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段", true, true, 1, 100, 20);
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.TextColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");

                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                #endregion 通用的属性.

                #region 个性化属性.
                map.AddTBString(MapAttrAttr.Name, "New Link", "标签", true, false, 0, 500, 20, true);
                map.AddTBString(MapAttrAttr.Tag1, "_blank", "连接目标", true, false, 0, 20, 20);
                map.SetHelperAlert(MapAttrAttr.Tag1, "比如:_blank,_parent,_self");
                map.AddTBString(MapAttrAttr.Tag2, null, "URL", true, false, 0, 500, 20, true);
                #endregion 个性化属性.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 超连接s
    /// </summary>
    public class ExtLinks : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 超连接s
        /// </summary>
        public ExtLinks()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ExtLink();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtLink> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtLink>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtLink> Tolist()
        {
            System.Collections.Generic.List<ExtLink> list = new System.Collections.Generic.List<ExtLink>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtLink)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
