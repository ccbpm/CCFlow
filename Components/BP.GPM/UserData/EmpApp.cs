using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 管理员与系统权限
    /// </summary>
    public class EmpAppAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
    }
    /// <summary>
    /// 管理员与系统权限
    /// </summary>
    public class EmpApp : EntityMyPK
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpAppAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpAppAttr.FK_Emp, value);
            }
        }
        public string FK_App
        {
            get
            {
                return this.GetValStringByKey(EmpAppAttr.FK_App);
            }
            set
            {
                this.SetValByKey(EmpAppAttr.FK_App, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(AppAttr.Name);
            }
            set
            {
                this.SetValByKey(AppAttr.Name, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(AppAttr.Idx);
            }
            set
            {
                this.SetValByKey(AppAttr.Idx, value);
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public string Img
        {
            get
            {
                string s = this.GetValStringByKey("WebPath");
                if (DataType.IsNullOrEmpty(s))
                {
                    return "../../DataUser/BP.GPM.STem/laptop.png";
                }
                else
                {
                    return s;
                }
            }
        }
        /// <summary>
        /// 超链接
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStringByKey(AppAttr.Url);
            }
            set
            {
                this.SetValByKey(AppAttr.Url, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 管理员与系统权限
        /// </summary>
        public EmpApp()
        {
        }
        /// <summary>
        /// 管理员与系统权限
        /// </summary>
        /// <param name="mypk"></param>
        public EmpApp(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 管理员与系统权限
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_EmpApp");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "管理员与系统权限";
                map.EnType = EnType.Sys;

                map.AddMyPK();

                map.AddTBString(EmpAppAttr.FK_Emp, null, "操作员", true, false, 0, 50, 20);
                map.AddTBString(EmpAppAttr.FK_App, null, "系统", true, false, 0, 50, 20);

                map.AddTBString(AppAttr.Name, null, "系统-名称", true, false, 0, 3900, 20);
                
                map.AddTBString(AppAttr.Url, null, "连接", true, false, 0, 3900, 20, true);

                map.AddMyFile("图标");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 管理员与系统权限s
    /// </summary>
    public class EmpApps : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 系统s
        /// </summary>
        public EmpApps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpApp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpApp> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpApp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpApp> Tolist()
        {
            System.Collections.Generic.List<EmpApp> list = new System.Collections.Generic.List<EmpApp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpApp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
