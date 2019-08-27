using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 授权属性
    /// </summary>
    public class AuthAttr
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// 类型(0=全部流程1=指定流程)
        /// </summary>
        public const string AuthType = "AuthType";
        /// <summary>
        /// 授权人
        /// </summary>
        public const string Auther = "Auther";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 取回日期
        /// </summary>
        public const string TakeBackDT = "TakeBackDT";
        /// <summary>
        /// 人员编号.
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 人员名称
        /// </summary>
        public const string EmpName = "EmpName";

        public const string RDT = "RDT";

        
    }
    /// <summary>
    /// 授权
    /// </summary>
    public class Auth : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(AuthAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(AuthAttr.FlowNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 授权
        /// </summary>
        public Auth()
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

                Map map = new Map("WF_Auth", "授权");

                map.AddMyPK();

                map.AddTBInt(AuthAttr.AuthType, 0, "类型(0=全部流程1=指定流程)", true, false);
                map.AddTBString(AuthAttr.Auther, null, "授权人", true, false, 0, 100, 10);

                map.AddTBString(AuthAttr.FlowNo, null, "流程编号", true, false, 0, 100, 10);
                map.AddTBString(AuthAttr.FlowName, null, "流程名称", true, false, 0, 100, 10);
                map.AddTBDate(AuthAttr.TakeBackDT, null, "取回日期", true, false);
                 
                map.AddTBString(AuthAttr.EmpNo, null, "人员编号", true, false, 0, 100, 10);
                map.AddTBString(AuthAttr.EmpName, null, "人员名称", true, false, 0, 100, 10);
                map.AddTBDate(AuthAttr.RDT, null, "记录日期", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerGUID();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 授权
    /// </summary>
    public class Auths : EntitiesMyPK
    {
        /// <summary>
        /// 授权
        /// </summary>
        public Auths() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Auth();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Auth> ToJavaList()
        {
            return (System.Collections.Generic.IList<Auth>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Auth> Tolist()
        {
            System.Collections.Generic.List<Auth> list = new System.Collections.Generic.List<Auth>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Auth)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
