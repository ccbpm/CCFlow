using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Port;
using BP.WF;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 常用语属性
    /// </summary>
    public class FastInputAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 类型
        /// </summary>
        public const string ContrastKey = "ContrastKey";
        /// <summary>
        /// Vals
        /// </summary>
        public const string Vals = "Vals";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Emp = "FK_Emp";
    }
	/// <summary>
	/// 常用语
	/// </summary>
    public class FastInput : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 元素类型.
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.EleType);
            }
            set
            {
                this.SetValByKey(FastInputAttr.EleType, value);
            }
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegularExp
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.RegularExp);
            }
            set
            {
                this.SetValByKey(FastInputAttr.RegularExp, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.Name);
            }
            set
            {
                this.SetValByKey(FastInputAttr.Name, value);
            }
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNotNull
        {
            get
            {
                return this.GetValBooleanByKey(FastInputAttr.IsNotNull);
            }
            set
            {
                this.SetValByKey(FastInputAttr.IsNotNull, value);
            }
        }
        /// <summary>
        /// 是否写入流程数据表
        /// </summary>
        public bool IsWriteToFlowTable
        {
            get
            {
                return this.GetValBooleanByKey(FastInputAttr.IsWriteToFlowTable);
            }
            set
            {
                this.SetValByKey(FastInputAttr.IsWriteToFlowTable, value);
            }
        }
        
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FastInputAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string ContrastKey
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.ContrastKey);
            }
            set
            {
                this.SetValByKey(FastInputAttr.ContrastKey, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(FastInputAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 解决方案
        /// </summary>
        public int Vals
        {
            get
            {
                return this.GetValIntByKey(FastInputAttr.Vals);
            }
            set
            {
                this.SetValByKey(FastInputAttr.Vals, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool UIVisible
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIVisible);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIVisible, value);
            }
        }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool UIIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsEnable);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIIsEnable, value);
            }
        }
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.DefVal);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        /// <summary>
        /// 是否是数字签名?
        /// </summary>
        public bool IsSigan
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsSigan);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSigan, value);
            }
        }

        public override string InitMyPKVals()
        {
            this.MyPK =  this.FK_MapData + "_" + this.FK_Emp + "_" + this.Vals + "_" + this.ContrastKey + "_" + this.EleType;
            return base.InitMyPKVals();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 常用语
        /// </summary>
        public FastInput()
        {
        }
        /// <summary>
        /// 常用语
        /// </summary>
        /// <param name="no"></param>
        public FastInput(string mypk)
            : base(mypk)
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FrmSln", "常用语");
                map.Java_SetCodeStruct("4");

                map.AddMyPK();

                //该表单对应的表单ID
                map.AddTBString(FastInputAttr.ContrastKey, "CYY", "类型CYY", true, false, 0, 20, 20);
                map.AddTBString(FastInputAttr.FK_Emp, null, "人员编号", true, false, 0, 100, 4);
                map.AddTBString(FastInputAttr.Vals, null, "值", true, false, 0, 500, 500);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 常用语s
	/// </summary>
    public class FastInputs : EntitiesMyPK
    {
        public FastInputs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FastInput();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FastInput> ToJavaList()
        {
            return (System.Collections.Generic.IList<FastInput>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FastInput> Tolist()
        {
            System.Collections.Generic.List<FastInput> list = new System.Collections.Generic.List<FastInput>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FastInput)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
