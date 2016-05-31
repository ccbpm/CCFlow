 using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class GloVarAttr : EntityNoNameAttr
    {
        /// <summary>
        /// Val
        /// </summary>
        public const string Val = "Val";
        /// <summary>
        /// Note
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// GroupKey
        /// </summary>
        public const string GroupKey = "GroupKey";
    }
    /// <summary>
    /// 全局变量
    /// </summary>
    public class GloVar : EntityNoName
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
        public GloVar()
        {
        }
        /// <summary>
        /// 全局变量
        /// </summary>
        /// <param name="mypk"></param>
        public GloVar(string no)
        {
            this.No = no;
            this.Retrieve();
        }
         /// <summary>
		/// 键值
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="isNullAsVal"></param> 
        public GloVar(string key, object isNullAsVal)
		{
			try
			{
				this.No=key;
				this.Retrieve(); 
			}
			catch
			{				
				if (this.RetrieveFromDBSources()==0)
				{
					this.Val = isNullAsVal.ToString();
					this.Insert();
				}
			}
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

                Map map = new Map("Sys_GloVar");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "全局变量";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(GloVarAttr.No, null, "键", true, false, 1, 50, 20);
                map.AddTBString(GloVarAttr.Name, null, "名称", true, false, 0, 120, 20);
                map.AddTBString(GloVarAttr.Val, null, "值", true, false, 0, 4000, 20,true);
                map.AddTBString(GloVarAttr.GroupKey, null, "分组值", true, false, 0, 120, 20, true);
                map.AddTBStringDoc(GloVarAttr.Note, null, "说明", true, false,true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        #region 公共属性.
        /// <summary>
        /// 公共假期
        /// </summary>
        private static string _Holidays = null;
        /// <summary>
        /// 一个月份的假期.
        /// </summary>
        public static string Holidays
        {
            get
            {
                if (_Holidays != null)
                    return _Holidays; 
                GloVar en = new GloVar();
                en.No ="Holiday";
                int i= en.RetrieveFromDBSources();
                if (i==0)
                    _Holidays="";
                else
                    _Holidays = en.Val;
                return _Holidays;
            }
            set
            {
                _Holidays = value;
            }
        }
        #endregion



    }
    /// <summary>
    /// 全局变量s
    /// </summary>
    public class GloVars : EntitiesNoName
    {
        #region get value by key
        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">val</param>
        public static int SetValByKey(string key, object val)
        {
            GloVar en = new GloVar(key, val);
            en.ValOfObject = val;
            return en.Update();
        }
        /// <summary>
        /// 获取html数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValByKeyHtml(string key)
        {
            return DataType.ParseText2Html(GloVars.GetValByKey(key));
        }
        public static string GetValByKey(string key)
        {
            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.Val;
            }   

            throw new Exception("error key=" + key);
        }
        /// <summary>
        /// 得到，一个key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValByKey(string key, string isNullAs)
        {
            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.Val;
            }

            GloVar en = new GloVar(key, isNullAs);
            //GloVar en = new GloVar(key);
            return en.Val;
        }
        public static int GetValByKeyInt(string key, int isNullAs)
        {
            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfInt;
            }

            GloVar en = new GloVar(key, isNullAs);
            //GloVar en = new GloVar(key);
            return en.ValOfInt;
        }
        public static int GetValByKeyDecimal(string key, int isNullAs)
        {
            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfInt;
            }

            GloVar en = new GloVar(key, isNullAs);
            //GloVar en = new GloVar(key);
            return en.ValOfInt;
        }
        public static bool GetValByKeyBoolen(string key, bool isNullAs)
        {

            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfBoolen;
            }


            int val = 0;
            if (isNullAs)
                val = 1;

            GloVar en = new GloVar(key, val);

            return en.ValOfBoolen;
        }
        public static float GetValByKeyFloat(string key, float isNullAs)
        {
            foreach (GloVar cfg in GloVars.MyGloVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfFloat;
            }

            GloVar en = new GloVar(key, isNullAs);
            return en.ValOfFloat;
        }
        private static GloVars _MyGloVars = null;
        public static GloVars MyGloVars
        {
            get
            {
                if (_MyGloVars == null)
                {
                    _MyGloVars = new GloVars();
                    _MyGloVars.RetrieveAll();
                }
                return _MyGloVars;
            }
        }
        public static void ReSetVal()
        {
            _MyGloVars = null;
        }
        #endregion

        #region 构造
        /// <summary>
        /// 全局变量s
        /// </summary>
        public GloVars()
        {
        }
        /// <summary>
        /// 全局变量s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public GloVars(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GloVar();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GloVar> ToJavaList()
        {
            return (System.Collections.Generic.IList<GloVar>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GloVar> Tolist()
        {
            System.Collections.Generic.List<GloVar> list = new System.Collections.Generic.List<GloVar>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GloVar)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
 