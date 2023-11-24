using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.Cloud
{
    /// <summary>
    /// 部门编号人员
    /// </summary>
    public class OrgEmpAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 部门编号人员
    /// </summary>
    public class OrgEmp : EntityMM
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(OrgEmpAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(OrgEmpAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(EmpAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 部门编号人员
        /// </summary>
        public OrgEmp()
        {
        }
        /// <summary>
        /// 部门编号人员
        /// </summary>
        /// <param name="mypk"></param>
        public OrgEmp(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 部门编号人员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_OrgEmp","");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "公司人员";
                map.EnType = EnType.App;

                map.AddTBStringPK(OrgEmpAttr.FK_Emp, null, "人员", false, false, 0, 36, 36);
                map.AddTBStringPK(EmpAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

       
    }
    /// <summary>
    /// 部门编号人员s
    /// </summary>
    public class OrgEmps : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 部门编号s
        /// </summary>
        public OrgEmps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgEmp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgEmp> Tolist()
        {
            System.Collections.Generic.List<OrgEmp> list = new System.Collections.Generic.List<OrgEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
