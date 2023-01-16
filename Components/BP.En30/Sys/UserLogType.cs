using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Sys
{
    /// <summary>
    ///  类型
    /// </summary>
    public class UserLogType : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// 类型
        /// </summary>
        public UserLogType()
        {
        }
        /// <summary>
        /// 类型
        /// </summary>
        /// <param name="_No"></param>
        public UserLogType(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();//@hongyan.
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 类型
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_UserLogType", "类型");

                map.AddTBStringPK("No", null, "编号", true, true, 1, 100, 20);
                map.AddTBString("Name", null, "名称", true, false, 0, 100, 300);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 类型
    /// </summary>
    public class UserLogTypes : EntitiesNoName
    {
        #region 构造.
        /// <summary>
        /// 表单目录s
        /// </summary>
        public UserLogTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserLogType();
            }
        }
        #endregion 构造.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<UserLogType> ToJavaList()
        {
            return (System.Collections.Generic.IList<UserLogType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<UserLogType> Tolist()
        {
            System.Collections.Generic.List<UserLogType> list = new System.Collections.Generic.List<UserLogType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((UserLogType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
