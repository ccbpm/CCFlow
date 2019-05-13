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
using System.Web.Services.Description;

namespace BP.Sys
{
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTableSQL : EntityNoName
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
        /// 用户自定义表
        /// </summary>
        public SFTableSQL()
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
                Map map = new Map("Sys_SFTable", "字典表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "数据表类型", true, false, SFTableAttr.SrcType,
                    "@0=本地的类@1=创建表@2=表或视图@3=SQL查询表@4=WebServices@5=微服务Handler外部数据源@6=JavaScript外部数据源@7=动态Json");


                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);


                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(SFTableAttr.ColumnValue, null, "显示的值(编号列)", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "显示的文字(名称列)", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "父级值(父级列)", true, false, 0, 200, 20);
                map.AddTBStringDoc(SFTableAttr.SelectStatement, null, "查询语句", true, false);
                map.AddTBDateTime(SFTableAttr.RDT, null, "加入日期", false, false);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

                RefMethod rm = new RefMethod();
                rm.Title = "查看数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
             //   map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (attrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in attrs)
                    err += " @ " + item.MyPK + " " + item.Name;
                throw new Exception("@如下实体字段在引用:" + err + "。您不能删除该表。");
            }
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTableSQLs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTableSQLs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTableSQL();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTableSQL> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTableSQL>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTableSQL> Tolist()
        {
            System.Collections.Generic.List<SFTableSQL> list = new System.Collections.Generic.List<SFTableSQL>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTableSQL)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
