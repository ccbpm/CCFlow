using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using System.Collections.Generic;

namespace BP.Sys
{
    
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapDataTemplateAttr : EntityNoNameAttr
    {
        public const string PTable = "PTable";
    }
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapDataTemplate : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// 映射基础
        /// </summary>
        public MapDataTemplate()
        {
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapDataTemplate(string no)
            : base(no)
        {
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

                Map map = new Map("Sys_MapData", "表单模版表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基础信息.
                map.AddTBStringPK(MapDataTemplateAttr.No, null, "编号", true, false, 1, 150, 100);
                map.AddTBString(MapDataTemplateAttr.Name, null, "描述", true, false, 0, 200, 20);
                map.AddTBString(MapDataTemplateAttr.PTable, null, "物理表", true, false, 0, 100, 20);
                map.AddTBInt("IsTemplate", 0, "是否是表单模版", true, false);
                #endregion 基础信息.

                #region 设计者信息.
                //增加参数字段.
                map.AddTBAtParas(4000);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion 构造方法

    }
    /// <summary>
    /// 映射基础s
    /// </summary>
    public class MapDataTemplates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 映射基础s
        /// </summary>
        public MapDataTemplates()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDataTemplate();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDataTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDataTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDataTemplate> Tolist()
        {
            System.Collections.Generic.List<MapDataTemplate> list = new System.Collections.Generic.List<MapDataTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDataTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
