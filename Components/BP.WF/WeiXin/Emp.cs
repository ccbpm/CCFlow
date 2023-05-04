using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.WeiXin
{
    /// <summary>
    /// 工作人员属性
    /// </summary>
    public class EmpAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// SID
        /// </summary>
        public const string OpenID = "OpenID";
        /// <summary>
        /// 手机号码
        /// </summary>
        public const string Tel = "Tel";
        #endregion
    }
    /// <summary>
    /// Emp 的摘要说明。
    /// </summary>
    public class Emp : EntityNoName
    {
        #region 扩展属性
        public string OpenID
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OpenID);
            }
            set
            {
                this.SetValByKey(EmpAttr.OpenID, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpAttr.FK_Dept, value);
            }
        }
        
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpAttr.Tel, value);
            }
        }

        #endregion

        /// <summary>
        /// 工作人员
        /// </summary>
        public Emp()
        {
        }
       
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
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

                Map map = new Map("Port_Emp", "用户");

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 1, 50, 100);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 100);
                map.AddTBString(EmpAttr.OpenID, null, "OpenID", true, false, 0, 100, 100);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new BP.Port.Depts(), true);
                map.AddTBString(EmpAttr.Tel, null, "手机号码", false, false, 0, 11, 30);
                #endregion 字段


                this._enMap = map;
                return this._enMap;
            }
        }

        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
    }
    /// <summary>
    /// 工作人员
    /// </summary>
    public class Emps : EntitiesNoName
    {
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Emp();
            }
        }
        /// <summary>
        /// 工作人员s
        /// </summary>
        public Emps()
        {
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Emp> ToJavaList()
        {
            return (System.Collections.Generic.IList<Emp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Emp> Tolist()
        {
            System.Collections.Generic.List<Emp> list = new System.Collections.Generic.List<Emp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Emp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.


    }
}
