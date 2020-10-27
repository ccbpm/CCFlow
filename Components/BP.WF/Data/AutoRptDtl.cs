using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Data
{
    /// <summary>
    /// 自动报表 属性
    /// </summary>
    public class AutoRptDtlAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 发起人
        /// </summary>
        public const string AutoRptNo = "AutoRptNo";
        /// <summary>
        /// 插入日期
        /// </summary>
        public const string SQLExp = "SQLExp";
        public const string BeiZhu = "BeiZhu";
        public const string UrlExp = "UrlExp";

        #endregion
    }
    /// <summary>
    /// 自动报表-数据项
    /// </summary>
    public class AutoRptDtl : EntityOIDName
    {
        #region 属性
        /// <summary>
        /// 对应的任务.
        /// </summary>
        public string AutoRptNo
        {
            get
            {
                return this.GetValStringByKey(AutoRptDtlAttr.AutoRptNo);
            }
            set
            {
                this.SetValByKey(AutoRptDtlAttr.AutoRptNo, value);
            }
        }
        /// <summary>
        /// 到达的人员
        /// </summary>
        public string SQLExp
        {
            get
            {
                return this.GetValStringByKey(AutoRptDtlAttr.SQLExp);
            }
            set
            {
                this.SetValByKey(AutoRptDtlAttr.SQLExp, value);
            }
        }
        public string UrlExp
        {
            get
            {
                return this.GetValStringByKey(AutoRptDtlAttr.UrlExp);
            }
            set
            {
                this.SetValByKey(AutoRptDtlAttr.UrlExp, value);
            }
        }
        
        /// <summary>
        /// 发起时间（可以为空）
        /// </summary>
        public string BeiZhu
        {
            get
            {
                return this.GetValStringByKey(AutoRptDtlAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(AutoRptDtlAttr.BeiZhu, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// AutoRptDtl
        /// </summary>
        public AutoRptDtl()
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

                Map map = new Map("WF_AutoRptDtl", "自动报表-数据项");
                map.CodeStruct = "3";


                //主键.
                map.AddTBIntPKOID();
                map.AddTBString(AutoRptDtlAttr.Name, null, "标题", true, false, 0, 300, 150);

                map.AddTBString(AutoRptDtlAttr.SQLExp, null, "SQL表达式(返回一个数字)", true, false, 0, 300, 300);
                //map.AddTBStringDoc(AutoRptDtlAttr.SQLExp, null, "SQL表达式(返回一个数字)", true, false, true);
                string msg = "返回的一行一列:";
                msg += "\t\n比如:SELECT COUNT(*) FROM WF_EmpWorks WHERE FK_Emp ='@WebUser.No'";
                msg += "\t\n支持ccbpm表达式。";
                map.SetHelperAlert(AutoRptDtlAttr.SQLExp, msg);

                map.AddTBString(AutoRptDtlAttr.UrlExp, null, "Url表达式", true, false, 0, 300, 300);
                  msg = "具有绝对路径的Url表达式:";
                msg += "\t\n比如:http:/128.1.1.1:9090/myurl.htm?UserNo=@WebUser.No";
                msg += "\t\n支持ccbpm表达式。";
                map.SetHelperAlert(AutoRptDtlAttr.UrlExp, msg);

                map.AddTBString(AutoRptDtlAttr.BeiZhu, null, "备注", true, false, 0, 500, 150);

                map.AddTBString(AutoRptDtlAttr.AutoRptNo, null, "任务ID", false, false, 0, 20, 10);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 自动报表-数据项
    /// </summary>
    public class AutoRptDtls : EntitiesOIDName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new AutoRptDtl();
            }
        }
        /// <summary>
        /// 自动报表
        /// </summary>
        public AutoRptDtls() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AutoRptDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<AutoRptDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AutoRptDtl> Tolist()
        {
            System.Collections.Generic.List<AutoRptDtl> list = new System.Collections.Generic.List<AutoRptDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AutoRptDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
