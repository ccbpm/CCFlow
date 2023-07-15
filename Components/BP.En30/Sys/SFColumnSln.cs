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
using BP.Web;
using BP.Difference;
using Newtonsoft.Json.Linq;

namespace BP.Sys
{
    /// <summary>
    /// 查询列s
    /// </summary>
    public class SFColumnSln : EntityMyPK
    { 
        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFColumnSln()
        {
        }
        public SFColumnSln(string no)
        {
            this.MyPK = no;
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
                Map map = new Map("Sys_SFColumnSln", "查询列转换方案");

                map.AddMyPK();
                //对应的是： MapExt的 MyPK
                map.AddTBString("RefPKVal", null, "实体主键", false, false, 1, 200, 20);
                map.AddTBString("FrmID", null, "表单", false, false, 1, 200, 20);
                map.AddTBString("AttrKey", null, "列英文名", true, true, 1, 200, 100);
                map.AddTBString("AttrName", null, "列中文名", true, true, 0, 200, 100);
                map.AddBoolean("IsEnable", true, "启用?", true, true);
                map.AddTBString("DataType", null, "列中文名", true, true, 0, 200, 100);

                // const sql=""
                map.AddTBString("ToField", null, "转换列名", true, true, 0, 200, 100);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 查询列s
    /// </summary>
    public class SFColumnSlns : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 查询列s
        /// </summary>
        public SFColumnSlns()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFColumnSln();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFColumnSln> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFColumnSln>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFColumnSln> Tolist()
        {
            System.Collections.Generic.List<SFColumnSln> list = new System.Collections.Generic.List<SFColumnSln>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFColumnSln)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
