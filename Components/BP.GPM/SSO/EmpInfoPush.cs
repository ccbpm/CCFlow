using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 信息推送
    /// </summary>
    public class EmpInfoPushAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 图标
        /// </summary>
        public const string ICON = "ICON";
        /// <summary>
        /// Url
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 获取的sql
        /// </summary>
        public const string GetSQL = "GetSQL";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 消息id
        /// </summary>
        public const string FK_InfoPush = "FK_InfoPush";
    }
    /// <summary>
    /// 信息推送
    /// </summary>
    public class EmpInfoPush : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// Idx
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EmpInfoPushAttr.Idx);
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.Idx, value);
            }
        }
        /// <summary>
        /// GetSQL
        /// </summary>
        public string GetSQL
        {
            get
            {
                string sql = this.GetValStringByKey(EmpInfoPushAttr.GetSQL);
                sql = sql.Replace("@WebUser.No", "'"+Web.WebUser.No+"'");
                return sql;
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.GetSQL, value);
            }
        }
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey("WebPath");
            }
        }
        public string ICON
        {
            get
            {
                return this.WebPath;
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.ICON, value);
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey(EmpInfoPushAttr.Url);
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.Url, value);
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpInfoPushAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.FK_Emp, value);
            }
        }
        public string FK_InfoPush
        {
            get
            {
                return this.GetValStringByKey(EmpInfoPushAttr.FK_InfoPush);
            }
            set
            {
                this.SetValByKey(EmpInfoPushAttr.FK_InfoPush, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 信息推送
        /// </summary>
        public EmpInfoPush()
        {
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
                Map map = new Map("GPM_EmpInfoPush");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "信息推送";
                map.EnType = EnType.Sys;

                map.AddMyPK();

                map.AddTBString(EmpInfoPushAttr.FK_Emp, null, "操作员", true, false, 0, 30, 20);
                map.AddTBString(EmpInfoPushAttr.FK_InfoPush, null, "推送消息ID", true, false, 0, 30, 20);

              //  map.AddTBString(EmpInfoPushAttr.No, null, "编号", true, false, 2, 30, 20);

                map.AddTBString(EmpInfoPushAttr.Name, null, "名称", true, false, 0, 3900, 20);
                map.AddTBString(EmpInfoPushAttr.Url, null, "连接", true, false, 0, 3900, 20, true);
                map.AddDDLSysEnum(BarAttr.OpenWay, 0, "打开方式", true, true,
                BarAttr.OpenWay, "@0=新窗口@1=本窗口@2=覆盖新窗口");
                map.AddTBInt(EmpInfoPushAttr.Idx, 0, "显示顺序", true, false);

              
                 
                map.AddMyFile("图标");
         

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 信息推送s
    /// </summary>
    public class EmpInfoPushs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 信息推送s
        /// </summary>
        public EmpInfoPushs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpInfoPush();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpInfoPush> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpInfoPush>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpInfoPush> Tolist()
        {
            System.Collections.Generic.List<EmpInfoPush> list = new System.Collections.Generic.List<EmpInfoPush>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpInfoPush)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
