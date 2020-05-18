using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Port
{
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class TeamTypeAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    ///  用户组类型
    /// </summary>
    public class TeamType : EntityNoName
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
        public TeamType()
        {
        }
        /// <summary>
        /// 用户组类型
        /// </summary>
        /// <param name="_No"></param>
        public TeamType(string _No) : base(_No) { }
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
                Map map = new Map("Port_TeamType", "用户组类型");
                map.Java_SetCodeStruct("2");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);

                map.AddTBStringPK(TeamTypeAttr.No, null, "编号", true, true, 1, 5, 5);
                map.AddTBString(TeamTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
                map.AddTBInt(TeamTypeAttr.Idx, 0, "顺序", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 用户组类型
    /// </summary>
    public class TeamTypes : EntitiesNoName
    {
        /// <summary>
        /// 用户组类型s
        /// </summary>
        public TeamTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TeamType();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TeamType> ToJavaList()
        {
            return (System.Collections.Generic.IList<TeamType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TeamType> Tolist()
        {
            System.Collections.Generic.List<TeamType> list = new System.Collections.Generic.List<TeamType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TeamType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
