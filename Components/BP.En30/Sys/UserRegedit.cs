using System;
using System.Collections;
using System.Collections.Generic;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
namespace BP.Sys
{
    /// <summary>
    /// 用户注册表
    /// </summary>
    public class UserRegeditAttr
    {
        /// <summary>
        /// 是否显示图片
        /// </summary>
        public const string IsPic = "IsPic";
        /// <summary>
        /// 名称
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 实体名称
        /// </summary>
        public const string CfgKey = "CfgKey";
        /// <summary>
        /// 属性
        /// </summary> 
        public const string Vals = "Vals";
        /// <summary>
        /// 查询
        /// </summary>
        public const string SearchKey = "SearchKey";
        /// <summary>
        /// MyPK
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// OrderBy
        /// </summary>
        public const string OrderBy = "OrderBy";
        /// <summary>
        /// OrderWay
        /// </summary>
        public const string OrderWay = "OrderWay";
        /// <summary>
        /// 产生的sql
        /// </summary>
        public const string GenerSQL = "GenerSQL";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 数值
        /// </summary>
        public const string NumKey = "NumKey";
        /// <summary>
        /// 查询
        /// </summary>
        public const string MVals = "MVals";
        /// <summary>
        /// 查询时间从
        /// </summary>
        public const string DTFrom = "DTFrom";
        /// <summary>
        /// 查询时间到
        /// </summary>
        public const string DTTo = "DTTo";
    }
    /// <summary>
    /// 用户注册表
    /// </summary>
    public class UserRegedit : EntityMyPK
    {
        #region 用户注册表信息键值列表
        #endregion

        /// <summary>
        /// 是否使用自动的MyPK,即FK_Emp + CfgKey
        /// </summary>
        public bool AutoMyPK { get; set; }

        #region 基本属性
        /// <summary>
        /// 是否显示图片
        /// </summary>
        public bool IsPic
        {
            get
            {
                return this.GetValBooleanByKey(UserRegeditAttr.IsPic);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.IsPic, value);
            }
        }
        /// <summary>
        /// 数值键
        /// </summary>
        public string NumKey
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.NumKey);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.NumKey, value);
            }
        }
        /// <summary>
        /// 参数
        /// </summary>
        public string Paras
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.Paras);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.Paras, value);
            }
        }
        /// <summary>
        /// 产生的sql
        /// </summary>
        public string GenerSQL
        {
            get
            {
                string GenerSQL = this.GetValStringByKey(UserRegeditAttr.GenerSQL);
                GenerSQL = GenerSQL.Replace("~", "'");
                return GenerSQL;
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.GenerSQL, value);
            }
        }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string OrderWay
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.OrderWay);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.OrderWay, value);
            }
        }
        public string OrderBy
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.OrderBy);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.OrderBy, value);
            }
        }
        /// <summary>
        /// FK_Emp
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 查询时间从
        /// </summary>
        public string DTFrom_Data
        {
            get
            {
                string s = this.GetValStringByKey(UserRegeditAttr.DTFrom);
                if (string.IsNullOrEmpty(s) || 1 == 1)
                {
                    DateTime dt = DateTime.Now.AddDays(-14);
                    return dt.ToString(DataType.SysDataFormat);
                }
                return s.Substring(0, 10);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTFrom, value);
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public string DTTo_Data
        {
            get
            {
                string s = this.GetValStringByKey(UserRegeditAttr.DTTo);
                if (string.IsNullOrEmpty(s) || 1 == 1)
                {
                    DateTime dt = DateTime.Now;
                    return dt.ToString(DataType.SysDataFormat);
                }
                return s.Substring(0, 10);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTTo, value);
            }
        }

        /// <summary>
        /// 查询时间从
        /// </summary>
        public string DTFrom
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.DTFrom);
                //string s = this.GetValStringByKey(UserRegeditAttr.DTFrom);
                //if (string.IsNullOrEmpty(s) || 1==1)
                //{
                //    DateTime dt = DateTime.Now.AddDays(-14);
                //    return dt.ToString(DataType.SysDataFormat);
                //}
                //return s.Substring(0, 10);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTFrom, value);
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public string DTTo
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.DTTo);
                //string s = this.GetValStringByKey(UserRegeditAttr.DTTo);
                //if (string.IsNullOrEmpty(s) || 1 == 1 )
                //{
                //    DateTime dt = DateTime.Now;
                //    return dt.ToString(DataType.SysDataFormat);
                //}
                //return s.Substring(0, 10);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTTo, value);
            }
        }
        public string DTFrom_Datatime
        {
            get
            {
                string s = this.GetValStringByKey(UserRegeditAttr.DTFrom);
                if (string.IsNullOrEmpty(s))
                {
                    DateTime dt = DateTime.Now.AddDays(-14);
                    return dt.ToString(DataType.SysDataTimeFormat);
                }
                return s;
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTFrom, value);
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public string DTTo_Datatime
        {
            get
            {
                string s = this.GetValStringByKey(UserRegeditAttr.DTTo);
                if (string.IsNullOrEmpty(s))
                {
                    DateTime dt = DateTime.Now;
                    return dt.ToString(DataType.SysDataTimeFormat);
                }
                return s;
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.DTTo, value);
            }
        }
        /// <summary>
        /// CfgKey
        /// </summary>
        public string CfgKey
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.CfgKey);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.CfgKey, value);
            }
        }
        public string SearchKey
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.SearchKey);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.SearchKey, value);
            }
        }
        /// <summary>
        /// Vals
        /// </summary>
        public string Vals
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.Vals);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.Vals, value);
            }
        }
        public string MVals
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.MVals);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.MVals, value);
            }
        }
        public string MyPK
        {
            get
            {
                return this.GetValStringByKey(UserRegeditAttr.MyPK);
            }
            set
            {
                this.SetValByKey(UserRegeditAttr.MyPK, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 用户注册表
        /// </summary>
        public UserRegedit()
        {
            AutoMyPK = true;
        }
        /// <summary>
        /// 用户注册表
        /// </summary>
        /// <param name="fk_emp">人员</param>
        /// <param name="cfgkey">配置</param>
        public UserRegedit(string fk_emp, string cfgkey)
            : this()
        {
            this.MyPK = fk_emp + cfgkey;
            this.CfgKey = cfgkey;
            this.FK_Emp = fk_emp;
            int i = this.RetrieveFromDBSources();
            if (i == 0)
            {
                this.CfgKey = cfgkey;
                this.FK_Emp = fk_emp;
                this.DirectInsert();
                // this.DirectInsert();
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
                Map map = new Map("Sys_UserRegedit", "用户注册表");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(UserRegeditAttr.FK_Emp, null, "用户", false, false, 0, 30, 20);
                map.AddTBString(UserRegeditAttr.CfgKey, null, "键", true, false, 0, 200, 20);
                map.AddTBString(UserRegeditAttr.Vals, null, "值", true, false, 0, 2000, 20);
                map.AddTBString(UserRegeditAttr.GenerSQL, null, "GenerSQL", true, false, 0, 2000, 20);
                map.AddTBString(UserRegeditAttr.Paras, null, "Paras", true, false, 0, 2000, 20);
                map.AddTBString(UserRegeditAttr.NumKey, null, "分析的Key", true, false, 0, 300, 20);
                map.AddTBString(UserRegeditAttr.OrderBy, null, "OrderBy", true, false, 0, 300, 20);
                map.AddTBString(UserRegeditAttr.OrderWay, null, "OrderWay", true, false, 0, 300, 20);
                map.AddTBString(UserRegeditAttr.SearchKey, null, "SearchKey", true, false, 0, 300, 20);
                map.AddTBString(UserRegeditAttr.MVals, null, "MVals", true, false, 0, 2000, 20);
                map.AddBoolean(UserRegeditAttr.IsPic, false, "是否图片", true, false);

                map.AddTBString(UserRegeditAttr.DTFrom, null, "查询时间从", true, false, 0, 20, 20);
                map.AddTBString(UserRegeditAttr.DTTo, null, "到", true, false, 0, 20, 20);

                //增加属性.
                map.AddTBAtParas(4000);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new UserRegedits(); }
        }
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
        #endregion 重写

        /// <summary>
        /// 获取键/值对集合
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetVals()
        {
            if (string.IsNullOrWhiteSpace(this.Vals))
                return new Dictionary<string, string>();

            string[] arr = null;
            string[] strs = this.Vals.Split("@".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int idx = -1;
            Dictionary<string, string> kvs = new Dictionary<string, string>();

            foreach (string str in strs)
            {
                idx = str.IndexOf('=');

                if (idx == -1)
                    continue;

                kvs.Add(str.Substring(0, idx), idx == str.Length - 1 ? "" : str.Substring(idx + 1));
            }

            return kvs;
        }

        /// <summary>
        /// 获取当前用户是否具有导入数据的权限
        /// <para>added by liuxc,2017-04-30</para>
        /// <remarks>注意：此权限数据保存于Sys_Regedit.Paras字段中，为@ImpEmpNos=liyan,liping,ligen格式</remarks>
        /// </summary>
        /// <param name="ensName">集合类全名，如BP.Port.Emps</param>
        /// <returns></returns>
        public static bool HaveRoleForImp(string ensName)
        {
            //获取可导入权限
            UserRegedit ur = new UserRegedit("admin", ensName + "_SearchAttrs");
            string impEmps = new AtPara(ur.Paras).GetValStrByKey("ImpEmpNos");

            if (string.IsNullOrWhiteSpace(impEmps))
            {
                return true;
            }
            else
            {
                return WebUser.No == "admin" || ("," + impEmps + ",").IndexOf("," + WebUser.No + ",") != -1;
            }
        }

        /// <summary>
        /// 获取当前用户是否具有导出数据的权限
        /// <para>added by liuxc,2017-04-30</para>
        /// <remarks>注意：此权限数据保存于Sys_Regedit.Paras字段中，为@ExpEmpNos=liyan,liping,ligen格式</remarks>
        /// </summary>
        /// <param name="ensName">集合类全名，如BP.Port.Emps</param>
        /// <returns></returns>
        public static bool HaveRoleForExp(string ensName)
        {
            //获取可导入权限
            UserRegedit ur = new UserRegedit("admin", ensName + "_SearchAttrs");
            string impEmps = new AtPara(ur.Paras).GetValStrByKey("ExpEmpNos");
            if (string.IsNullOrWhiteSpace(impEmps))
            {
                return true;
            }
            else
            {
                return WebUser.No == "admin" || ("," + impEmps + ",").IndexOf("," + WebUser.No + ",") != -1;
            }
        }
    }
    /// <summary>
    /// 用户注册表s
    /// </summary>
    public class UserRegedits : EntitiesMyPK
    {
        #region 构造
        public UserRegedits()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        public UserRegedits(string emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(UserRegeditAttr.FK_Emp, emp);
            qo.DoQuery();
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserRegedit();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<UserRegedit> ToJavaList()
        {
            return (System.Collections.Generic.IList<UserRegedit>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<UserRegedit> Tolist()
        {
            System.Collections.Generic.List<UserRegedit> list = new System.Collections.Generic.List<UserRegedit>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((UserRegedit)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
