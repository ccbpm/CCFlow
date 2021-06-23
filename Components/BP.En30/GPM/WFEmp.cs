using System;
using System.Data;
using System.Text.RegularExpressions;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Web;

namespace BP.GPM
{
    /// <summary>
    /// 操作员属性
    /// </summary>
    public class WFEmpAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// sid
        /// </summary>
        public const string SID = "SID";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 直属部门领导
        /// </summary>
        public const string Leader = "Leader";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 拼音
        /// </summary>
        public const string PinYin = "PinYin";
        public const string SignType = "SignType";
        #endregion

        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string UserID = "UserID";
        public const string UseSta = "UseSta";
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class WFEmp : EntityNoName
    {
        #region 扩展属性
       

        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpAttr.PinYin, value);
            }
        }
        /// <summary>
        /// 直属部门领导
        /// </summary>
        public string Leader
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Leader);
            }
            set
            {
                this.SetValByKey(EmpAttr.Leader, value);
            }
        }
        /// <summary>
        /// 主要的部门。
        /// </summary>
        public Dept HisDept
        {
            get
            {
                try
                {
                    return new Dept(this.FK_Dept);
                }
                catch (Exception ex)
                {
                    throw new Exception("@获取操作员" + this.No + "部门[" + this.FK_Dept + "]出现错误,可能是系统管理员没有给他维护部门.@" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
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
        /// 组织编号
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
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpAttr.FK_Dept);
            }
        }
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
        public string Email
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Email);
            }
            set
            {
                this.SetValByKey(EmpAttr.Email, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Pass);
            }
            set
            {
                this.SetValByKey(EmpAttr.Pass, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.Idx);
            }
            set
            {
                this.SetValByKey(EmpAttr.Idx, value);
            }
        }

        public int UseSta
        {
            get
            {
                return this.GetValIntByKey(WFEmpAttr.UseSta);
            }
            set
            {
                this.SetValByKey(WFEmpAttr.UseSta, value);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            if (this.Pass == pass)
                return true;
            return false;
        }
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public WFEmp()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public WFEmp(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@用户或者密码错误：[" + no + "]，或者帐号被停用。@技术信息(从内存中查询出现错误)：ex1=" + ex.Message);
            }
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsView=true;
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

                Map map = new Map("WF_Emp", "用户");

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.EnType = EnType.App;
                map.IndexField = EmpAttr.FK_Dept;
                #endregion

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(WFEmpAttr.No, null, "登陆账号", true, false, 1, 50, 90);
                map.AddTBString(WFEmpAttr.Name, null, "名称", true, false, 0, 200, 130);
                map.AddTBString(WFEmpAttr.Pass, "123", "密码", false, false, 0, 100, 10);


                map.AddDDLEntities(WFEmpAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), false);
                string msg = "最后登录的部门:";
                msg += "\t\n 1.常用部门，一个人有多个部门，但是必须有一个主部门。";
                msg += "\t\n 2.在多部门的情况下，切换部门的时候，这个保存的最后登录的部门。";
                map.SetHelperAlert(WFEmpAttr.FK_Dept, msg);


                map.AddTBString(WFEmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(WFEmpAttr.Tel, null, "电话", true, false, 0, 20, 130);

                map.AddTBString(WFEmpAttr.Leader, null, "直属部门领导", true, false, 0, 20, 130);
                map.SetHelperAlert(WFEmpAttr.Leader, "这里是领导的登录帐号，不是中文名字，用于流程的接受人规则中。");


                map.AddTBString(WFEmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(WFEmpAttr.PinYin, null, "拼音", true, false, 0, 500, 132, true);

                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                {
                    map.AddTBString(EmpExtAttr.UserID, null, "用户ID", true, false, 0, 50, 30);
                    map.AddTBString(EmpExtAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 30);
                    map.AddTBString(EmpExtAttr.OrgName, null, "OrgName", true, false, 0, 500, 132, true);
                }

                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(EmpExtAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);
                    map.AddTBString(EmpExtAttr.QQAppID, null, "QQAppID", true, false, 0, 500, 132, true);

                    map.AddTBString(EmpExtAttr.OpenID, null, "OpenID", false, false, 0, 200, 36);//小程序的openID
                    map.AddTBString(EmpExtAttr.OpenID2, null, "OpenID2", false, false, 0, 200, 36);//公众号的openID
                    map.AddTBString(EmpExtAttr.unionid, null, "unionid", false, false, 0, 200, 36);//公众号的openID
                }

                map.AddTBInt(WFEmpAttr.UseSta, 1, "用户状态0禁用,1正常.", true, true);

                map.AddTBInt(WFEmpAttr.Idx, 0, "序号", true, false);
                map.SetHelperAlert(WFEmpAttr.Idx, "显示的顺序");

                #endregion 字段

               
                this._enMap = map;
                return this._enMap;
            }
        }

      
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class WFEmps : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WFEmp();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public WFEmps()
        {
        }
        public override int RetrieveAll()
        {
            return base.RetrieveAll("Name");
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WFEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<WFEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WFEmp> Tolist()
        {
            System.Collections.Generic.List<WFEmp> list = new System.Collections.Generic.List<WFEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WFEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
