using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;

namespace BP.Sys
{
    /// <summary>
    /// 系统字典表
    /// </summary>
    public class SFTableDict : EntityNoName
    {

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 系统字典表
        /// </summary>
        public SFTableDict()
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

                Map map = new Map("Sys_SFTable", "系统字典表");

                map.AddTBStringPK(SFTableAttr.No, null, "编号", true, true, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "名称", true, false, 0, 200, 20);
                //map.AddBoolean(SFTableAttr.IsAutoGenerNo, true, "是否自动生成编号", true, true);
                map.AddDDLSysEnum(SFTableAttr.NoGenerModel, 1, "编号生成规则", true, true, SFTableAttr.NoGenerModel,
            "@0=自定义@1=流水号@2=标签的全拼@3=标签的简拼@4=按GUID生成");

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "字典表类型", true, false, SFTableAttr.CodeStruct);
                map.AddDDLStringEnum(SFTableAttr.DictSrcType, "SysDict", "数据表类型", SFTableAttr.DictSrcType, false);


                RefMethod rm = new RefMethod();
                rm.Title = "编辑数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            if (this.GetValIntByKey(SFTableAttr.CodeStruct) == 0)
                return  "../../Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + this.No + "&&QueryType=Dict";
            else
            {
                return "../../Admin/FoolFormDesigner/SFTableEditDataTree.htm?FK_SFTable=" + this.No + "&&QueryType=Dict";
            }
        }
        /// <summary>
        /// 删除之前要做的工作
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (mattrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in mattrs)
                    err += " @ " + item.MyPK + " " + item.Name;
                throw new Exception("@如下实体字段在引用:" + err + "。您不能删除该表。");
            }
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 系统字典表s
    /// </summary>
    public class SFTableDicts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 系统字典表s
        /// </summary>
        public SFTableDicts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTableDict();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTableDict> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTableDict>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTableDict> Tolist()
        {
            System.Collections.Generic.List<SFTableDict> list = new System.Collections.Generic.List<SFTableDict>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTableDict)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
