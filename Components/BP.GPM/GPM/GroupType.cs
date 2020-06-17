using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class GroupTypeAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    ///  用户组类型
    /// </summary>
    public class GroupType : EntityNoName
    {
        #region 属性
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 用户组类型
        /// </summary>
        public GroupType()
        {
        }
        /// <summary>
        /// 用户组类型
        /// </summary>
        /// <param name="_No"></param>
        public GroupType(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 用户组类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_GroupType", "用户组类型");
                map.CodeStruct = "2";

                map.DepositaryOfEntity= Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(GroupTypeAttr.No, null, "编号", true, true, 1, 5, 5);
                map.AddTBString(GroupTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
                map.AddTBInt(GroupTypeAttr.Idx, 0, "顺序", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class GroupTypes : EntitiesNoName
    {
        /// <summary>
        /// 用户组类型s
        /// </summary>
        public GroupTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupType();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupType> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupType> Tolist()
        {
            System.Collections.Generic.List<GroupType> list = new System.Collections.Generic.List<GroupType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
