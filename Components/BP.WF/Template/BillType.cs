using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.Template
{
	/// <summary>
    ///  单据类型
	/// </summary>
    public class BillType : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey("FK_Flow");
            }
            set
            {
                this.SetValByKey("FK_Flow", value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 单据类型
        /// </summary>
        public BillType()
        {
        }
        /// <summary>
        /// 单据类型
        /// </summary>
        /// <param name="_No"></param>
        public BillType(string _No) : base(_No) { }
        /// <summary>
        /// 单据类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_BillType", "单据类型");
                map.Java_SetCodeStruct("2");;

                map.AddTBStringPK(SimpleNoNameAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(SimpleNoNameAttr.Name, null, "名称", true, false, 1, 50, 50);
                map.AddTBString("FK_Flow", null, "流程", true, false, 1, 50, 50);

                map.AddTBInt("IDX", 0, "IDX", false, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 单据类型
	/// </summary>
    public class BillTypes : EntitiesNoName
    {
        /// <summary>
        /// 单据类型s
        /// </summary>
        public BillTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BillType();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BillType> ToJavaList()
        {
            return (System.Collections.Generic.IList<BillType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BillType> Tolist()
        {
            System.Collections.Generic.List<BillType> list = new System.Collections.Generic.List<BillType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BillType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
