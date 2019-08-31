using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 评分控件
    /// </summary>
    public class ExtScore : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 目标
        /// </summary>
        //public string Target
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(MapAttrAttr.Tag1);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MapAttrAttr.Tag1, value);
        //    }
        //}
        /// <summary>
        /// URL
        /// </summary>
        public string URL
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag2);
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
                if (BP.Web.WebUser.No == "admin")
                {

                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }

                return uac;
            }
        }
        /// <summary>
        /// 评分控件
        /// </summary>
        public ExtScore()
        {
        }
        /// <summary>
        /// 评分控件
        /// </summary>
        /// <param name="mypk"></param>
        public ExtScore(string mypk)
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
                Map map = new Map("Sys_MapAttr", "评分控件");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 通用的属性.
                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段", true, true, 1, 100, 20);
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");                
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                #endregion 通用的属性.


                #region 个性化属性.
                map.AddTBString(MapAttrAttr.Name, null, "评分事项", true, false, 0, 500, 20, true);
                map.AddTBInt(MapAttrAttr.Tag2, 5, "总分", true, false);
                #endregion 个性化属性.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 评分控件s
    /// </summary>
    public class ExtScores : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 评分控件s
        /// </summary>
        public ExtScores()
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
