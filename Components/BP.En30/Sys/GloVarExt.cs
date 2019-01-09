 using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class GloVarExt : EntityNoName
    {
        #region 属性
        public object ValOfObject
        {
            get
            {
                return this.GetValByKey(GloVarAttr.Val);
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }
        public string Val
        {
            get
            {
                return this.GetValStringByKey(GloVarAttr.Val);
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }
        public float ValOfFloat
        {
            get
            {
                try
                {
                    return this.GetValFloatByKey(GloVarAttr.Val);
                }
                catch
                {
                    return 0;
                    throw new Exception("@" + this.Name + ", 没有设置默认值." + this.Val);
                }
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }
        public int ValOfInt
        {
            get
            {
                try
                {
                    return this.GetValIntByKey(GloVarAttr.Val);
                }
                catch(Exception ex)
                {
                    return 0;
                    throw new Exception("@" + this.Name + ", 没有设置默认值." + this.Val);
                }
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }
        public decimal ValOfDecimal
        {
            get
            {
                try
                {
                    return this.GetValDecimalByKey(GloVarAttr.Val);
                  }
                catch
                {
                    return 0;
                    throw new Exception("@" + this.Name + ", 没有设置默认值."+ this.Val);
                }
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }
        public bool ValOfBoolen
        {
            get
            {
                return this.GetValBooleanByKey(GloVarAttr.Val);
            }
            set
            {
                this.SetValByKey(GloVarAttr.Val, value);
            }
        }	
        /// <summary>
        /// note
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey(GloVarAttr.Note);
            }
            set
            {
                this.SetValByKey(GloVarAttr.Note, value);
            }
        }
        /// <summary>
        /// 分组值
        /// </summary>
        public string GroupKey
        {
            get
            {
                return this.GetValStringByKey(GloVarAttr.GroupKey);
            }
            set
            {
                this.SetValByKey(GloVarAttr.GroupKey, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 全局变量
        /// </summary>
        public GloVarExt()
        {
        }
        /// <summary>
        /// 全局变量
        /// </summary>
        /// <param name="mypk"></param>
        public GloVarExt(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 全局变量s
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_GloVar", "全局变量");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(GloVarAttr.No, null, "键", true, false, 1, 50, 20);
                map.AddTBString(GloVarAttr.Name, null, "名称", true, false, 0, 120, 20);
                map.AddTBString(GloVarAttr.Val, null, "值/表达式", true, false, 0, 2000, 20,true);
                map.AddTBString(GloVarAttr.GroupKey, null, "分组值", false, false, 0, 120, 20, false);
                map.AddTBStringDoc(GloVarAttr.Note, null, "备注", true, false,true);
                map.AddTBInt(GloVarAttr.Idx, 0, "顺序号", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 全局变量s
    /// </summary>
    public class GloVarExts : EntitiesNoName
    {
         
        #region 构造
        /// <summary>
        /// 全局变量s
        /// </summary>
        public GloVarExts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GloVarExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GloVarExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<GloVarExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GloVarExt> Tolist()
        {
            System.Collections.Generic.List<GloVarExt> list = new System.Collections.Generic.List<GloVarExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GloVarExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
 