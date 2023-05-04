using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Sys
{
    /// <summary>
    ///  级别
    /// </summary>
    public class UserLogLevel : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// 级别
        /// </summary>
        public UserLogLevel()
        {
        }
        /// <summary>
        /// 表单目录
        /// </summary>
        /// <param name="_No"></param>
        public UserLogLevel(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();

                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 级别
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_UserLogLevel", "级别");

                map.AddTBStringPK("No", null, "编号", true, true, 1, 100, 300);
                map.AddTBString("Name", null, "名称", true, false, 0, 100, 600);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 级别
    /// </summary>
    public class UserLogLevels : EntitiesNoName
    {
        #region 构造.
        /// <summary>
        /// 级别s
        /// </summary>
        public UserLogLevels() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserLogLevel();
            }
        }
        #endregion 构造.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<UserLogLevel> ToJavaList()
        {
            return (System.Collections.Generic.IList<UserLogLevel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<UserLogLevel> Tolist()
        {
            System.Collections.Generic.List<UserLogLevel> list = new System.Collections.Generic.List<UserLogLevel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((UserLogLevel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
