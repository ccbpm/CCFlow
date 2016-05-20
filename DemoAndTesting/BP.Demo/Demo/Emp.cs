using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 员工 属性
    /// </summary>
    public class EmpAttr:EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 年龄
        /// </summary>
        public const string Age = "Age";
        #endregion
    }
    /// <summary>
    /// 员工
    /// </summary>
    public class Emp : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {  return this.GetValStringByKey(EmpAttr.Email); }
            set
            {  this.SetValByKey(EmpAttr.Email, value); }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            { return this.GetValIntByKey(EmpAttr.XB);    }
            set
            {  this.SetValByKey(EmpAttr.XB, value);      }
        }
        /// <summary>
        /// 性别标签
        /// </summary>
        public string XB_Text
        {
            get
            { return this.GetValRefTextByKey(EmpAttr.XB); }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.Addr);
            }
            set
            {
                this.SetValByKey(EmpAttr.Addr, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpAttr.Tel, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            { return this.GetValStringByKey(EmpAttr.FK_Dept);  }
            set
            {  this.SetValByKey(EmpAttr.FK_Dept, value); }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string FK_Dept_Text
        {
            get
            { return this.GetValStringByKey(EmpAttr.FK_Dept);   }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 员工
        /// </summary>
        public Emp()
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
                Map map = new Map("Demo_Emp");
                map.EnDesc = "员工";
                map.DepositaryOfEntity= Depositary.None;
                //字段映射.
                map.AddTBStringPK(EmpAttr.No,null,"编号",true,false,5,40,4);
                map.AddTBString(EmpAttr.Name, null, "name", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Email, null, "Email", true, false, 0, 200, 10);
                map.AddTBString(EmpAttr.Addr, null, "Addr", true, false, 0, 200, 10);
                map.AddBoolean(EmpAttr.IsEnable, true, "是否启用", true, true);
                map.AddDDLSysEnum(EmpAttr.XB, 0, "性别", true,true,"XB","@0=女@1=男");
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new BP.Port.Depts(), true);
                map.AddTBInt(EmpAttr.Age, 18, "年龄", true, false);

                //查询条件
                map.AddSearchAttr(EmpAttr.XB);
                map.AddSearchAttr(EmpAttr.FK_Dept);

                //带有参数的方法映射.
                RefMethod rm = new RefMethod();
                rm.Title = "调动";
                rm.Warning = "您确定要执行调动吗？";
                //增加两个参数.
                rm.HisAttrs.AddDDLEntities("FK_Dept", null, "要调动到的部门",  new BP.Port.Depts(), true);
                rm.HisAttrs.AddTBString("Note", null, "调用原因", true, false, 0, 1000, 100);
                rm.ClassMethodName = this.ToString() + ".DoMove";
                map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 执行调动
        /// </summary>
        /// <param name="fk_dept"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoMove(string fk_dept, string note)
        {
            //编写您的逻辑.
            Dept dept = new Dept(fk_dept);
            return "已经成功的把改人员调动到:"+dept.Name+" , 调用原因是:"+note;
        }
        #endregion
    }
    /// <summary>
    /// 员工s
    /// </summary>
    public class Emps : EntitiesNoName
    {
        #region 方法
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
        /// 员工s
        /// </summary>
        public Emps() 
        {
        }
        /// <summary>
        /// 查询全部(可以重写这个方法)
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }
        #endregion
    }
}