using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	/// <summary>
	/// 属性
	/// </summary>
    public class ContrastAttr
    {
        /// <summary>
        /// 对比项目
        /// </summary>
        public const string ContrastKey = "ContrastKey";
        /// <summary>
        /// KeyVal1
        /// </summary>
        public const string KeyVal1 = "KeyVal1";
        /// <summary>
        /// KeyVal2
        /// </summary>
        public const string KeyVal2 = "KeyVal2";
        /// <summary>
        /// 分类条件
        /// </summary>
        public const string SortBy = "SortBy";
        /// <summary>
        /// 对比的值
        /// </summary>
        public const string KeyOfNum = "KeyOfNum";
        /// <summary>
        /// 分组方式
        /// </summary>
        public const string GroupWay = "GroupWay";
        /// <summary>
        /// 排序
        /// </summary>
        public const string OrderWay = "OrderWay";
    }
	/// <summary>
	/// 对比状态存储
	/// </summary>
    public class Contrast : EntityMyPK
    {
        #region 基本属性
       
        /// <summary>
        /// 属性
        /// </summary>
        public string ContrastKey
        {
            get
            {
                string s= this.GetValStringByKey(ContrastAttr.ContrastKey);
                if (s == null || s == "")
                    s = "FK_NY";

                return s;
            }
            set
            {
                this.SetValByKey(ContrastAttr.ContrastKey, value);
            }
        }

        public string KeyVal1
        {
            get
            {
                return this.GetValStringByKey(ContrastAttr.KeyVal1);
            }
            set
            {
                this.SetValByKey(ContrastAttr.KeyVal1, value);
            }
        }
        public string KeyVal2
        {
            get
            {
                return this.GetValStringByKey(ContrastAttr.KeyVal2);
            }
            set
            {
                this.SetValByKey(ContrastAttr.KeyVal2, value);
            }
        }

        public string SortBy
        {
            get
            {
                return this.GetValStringByKey(ContrastAttr.SortBy);
            }
            set
            {
                this.SetValByKey(ContrastAttr.SortBy, value);
            }
        }

        public string KeyOfNum
        {
            get
            {
                return this.GetValStringByKey(ContrastAttr.KeyOfNum);
            }
            set
            {
                this.SetValByKey(ContrastAttr.KeyOfNum, value);
            }
        }

        public int GroupWay
        {
            get
            {
                return this.GetValIntByKey(ContrastAttr.GroupWay);
            }
            set
            {
                this.SetValByKey(ContrastAttr.GroupWay, value);
            }
        }
        public int OrderWay
        {
            get
            {
                return this.GetValIntByKey(ContrastAttr.OrderWay);
            }
            set
            {
                this.SetValByKey(ContrastAttr.OrderWay, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 对比状态存储
        /// </summary>
        public Contrast()
        {
        }
       
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) 
                    return this._enMap;

                Map map = new Map("Sys_UserRegedit");
                map.EnType = EnType.Sys;
                map.EnDesc = "对比状态存储";
                map.DepositaryOfEntity = Depositary.None;
                map.AddMyPK();

                map.AddTBString(ContrastAttr.ContrastKey, null, "对比项目", false, true, 0, 20, 10);
                map.AddTBString(ContrastAttr.KeyVal1, null, "KeyVal1", false, true, 0, 20, 10);
                map.AddTBString(ContrastAttr.KeyVal2, null, "KeyVal2", false, true, 0, 20, 10);

                map.AddTBString(ContrastAttr.SortBy, null, "SortBy", false, true, 0, 20, 10);
                map.AddTBString(ContrastAttr.KeyOfNum, null, "KeyOfNum", false, true, 0, 20, 10);

                map.AddTBInt(ContrastAttr.GroupWay, 1, "求什么?SumAvg", false, true);
                map.AddTBInt(ContrastAttr.OrderWay, 1, "OrderWay", false, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get {  return new Contrasts(); }
        }
        #endregion
    }
	/// <summary>
	/// 对比状态存储s
	/// </summary>
    public class Contrasts : Entities
    {
        /// <summary>
        /// 对比状态存储s
        /// </summary>
        public Contrasts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Contrast();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Contrast> ToJavaList()
        {
            return (System.Collections.Generic.IList<Contrast>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Contrast> Tolist()
        {
            System.Collections.Generic.List<Contrast> list = new System.Collections.Generic.List<Contrast>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Contrast)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
