using System;
using System.IO;
using System.Collections;
using BP.DA;
using BP.En;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace BP.Sys
{
    /// <summary>
    /// 语言模块
    /// </summary>
    public class LangueModel
    {
        /// <summary>
        /// 表单
        /// </summary>
        public const string CCForm = "CCForm";
        /// <summary>
        /// 查询组件
        /// </summary>
        public const string Search = "Search";
        /// <summary>
        /// 分组组件
        /// </summary>
        public const string Group = "Group";
        /// <summary>
        /// 菜单
        /// </summary>
        public const string Menu = "Menu";
    }
    /// <summary>
    /// 语言Attr
    /// </summary>
    public class LangueAttr
    {
        /// <summary>
        /// 模块
        /// </summary>
        public const string Model = "Model";
        /// <summary>
        /// 分类
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 关联的键
        /// </summary>
        public const string SortKey = "SortKey";
        /// <summary>
        /// 模块
        /// </summary>
        public const string Langue = "Langue";
        /// <summary>
        /// 值
        /// </summary>
        public const string Val = "Val";
        /// <summary>
        /// 模块键值 FrmID, Or FlowNo
        /// </summary>
        public const string ModelKey = "ModelKey";
    }
    /// <summary>
    /// 语言
    /// </summary>
    public class Langue : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 模块：比如 ccform.
        /// </summary>
        public string Model
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.Model);
            }
            set
            {
                this.SetValByKey(LangueAttr.Model, value);
            }
        }
        /// <summary>
        /// 类别：比如Label,Field
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.Sort);
            }
            set
            {
                this.SetValByKey(LangueAttr.Sort, value);
            }
        }
        /// <summary>
        /// 关联的主键: 比如:LabelID, KeyOfEn
        /// </summary>
        public string SortKey
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.SortKey);
            }
            set
            {
                this.SetValByKey(LangueAttr.SortKey, value);
            }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public string Lang
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.Langue);
            }
            set
            {
                this.SetValByKey(LangueAttr.Langue, value);
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public string Val
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.Val);
            }
            set
            {
                this.SetValByKey(LangueAttr.Val, value);
            }
        }
        public string ModelKey
        {
            get
            {
                return this.GetValStringByKey(LangueAttr.ModelKey);
            }
            set
            {
                this.SetValByKey(LangueAttr.ModelKey, value);
            }
        }
        #endregion


        #region 构造方法
        public Langue()
        {
        }
        public Langue(string pk)
            : base(pk)
        {
        }
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_Langue", "语言定义");
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.AddMyPK();

                map.AddTBString(LangueAttr.Langue, null, "语言ID", true, true, 0, 20, 20);

                map.AddTBString(LangueAttr.Model, null, "模块", true, true, 0, 20, 20);
                map.AddTBString(LangueAttr.ModelKey, null, "模块实例", true, true, 0, 200, 20);

                map.AddTBString(LangueAttr.Sort, null, "类别", true, true, 0, 20, 20);
                map.AddTBString(LangueAttr.SortKey, null, "类别PK", true, true, 0, 100, 20);

                map.AddTBString(LangueAttr.Val, null, "语言值", true, true, 0, 3999, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.Lang + "_" + this.Model + "_" + this.ModelKey + "_" + this.Sort + "_" + this.SortKey;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 语言 
    /// </summary>
    public class Langues : EntitiesMyPK
    {
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Langue();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Langue> ToJavaList()
        {
            return (System.Collections.Generic.IList<Langue>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Langue> Tolist()
        {
            System.Collections.Generic.List<Langue> list = new System.Collections.Generic.List<Langue>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Langue)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
