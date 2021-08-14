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
    public class EmpExtAttr : EntityNoNameAttr
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

        /// QQAppID
        /// </summary>
        public const string QQAppID = "QQAppID";
        /// <summary>
        /// QQ号
        /// </summary>
        public const string QQ = "QQ";
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public const string OrgName = "OrgName";
        public const string OpenID = "OpenID";
        public const string OpenID2 = "OpenID2";
        /// <summary>
        /// 连接的ID
        /// </summary>
        public const string unionid = "unionid";

    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class EmpExt : EntityNoName
    {
        public string UserID
        {
            get
            {
                return this.GetValStringByKey(EmpExtAttr.UserID);
            }
        }
       
        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public EmpExt()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public EmpExt(string no)
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

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.EnType = EnType.App;
                map.IndexField = EmpAttr.FK_Dept;
                #endregion

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpExtAttr.No, null, "登陆账号", true, false, 1, 50, 90);
                map.AddTBString(EmpExtAttr.Name, null, "名称", true, false, 0, 200, 130);
                map.AddTBString(EmpExtAttr.Pass, "123", "密码", false, false, 0, 100, 10);


                map.AddDDLEntities(EmpExtAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), false);
                string msg = "最后登录的部门:";
                msg += "\t\n 1.常用部门，一个人有多个部门，但是必须有一个主部门。";
                msg += "\t\n 2.在多部门的情况下，切换部门的时候，这个保存的最后登录的部门。";
                map.SetHelperAlert(EmpExtAttr.FK_Dept, msg);


                map.AddTBString(EmpExtAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpExtAttr.Tel, null, "电话", true, false, 0, 20, 130);

                map.AddTBString(EmpExtAttr.Leader, null, "直属部门领导", true, false, 0, 20, 130);
                map.SetHelperAlert(EmpExtAttr.Leader, "这里是领导的登录帐号，不是中文名字，用于流程的接受人规则中。");


                map.AddTBString(EmpExtAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpExtAttr.PinYin, null, "拼音", true, false, 0, 500, 132, true);

                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                {
                    map.AddTBString(EmpExtAttr.UserID, null, "用户ID", true, false, 0, 50, 30);
                    map.AddTBString(EmpExtAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 30);
                    map.AddTBString(EmpExtAttr.OrgName, null, "OrgName", true, false, 0, 500, 132, true);
                }

                if(SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(EmpExtAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);
                    map.AddTBString(EmpExtAttr.QQAppID, null, "QQAppID", true, false, 0, 500, 132, true);

                    map.AddTBString(EmpExtAttr.OpenID, null, "OpenID", false, false, 0, 200, 36);//小程序的openID
                    map.AddTBString(EmpExtAttr.OpenID2, null, "OpenID2", false, false, 0, 200, 36);//公众号的openID
                    map.AddTBString(EmpExtAttr.unionid, null, "unionid", false, false, 0, 200, 36);//公众号的openID
                }
               


                map.AddTBInt(EmpExtAttr.Idx, 0, "序号", true, false);
                map.SetHelperAlert(EmpExtAttr.Idx, "显示的顺序");

                #endregion 字段

               
                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeDelete()
        {
            if (this.No.Equals("admin") == true)
                throw new Exception("err@管理员账号不能删除.");

            return base.beforeDelete();
        }


        public string DoUnEnable()
        {
            string userNo = this.No;
            //判断当前人员是否有待办
            string wfSql = "";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                wfSql = " AND OrgNo='" + WebUser.OrgNo + "'";
                userNo = this.UserID;
            }
            string sql = "";
            /*不是授权状态*/
            if (SystemConfig.GetValByKeyBoolen("IsEnableTaskPool", false) == true)
                sql = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE FK_Emp='" + userNo + "' AND TaskSta!=1 " + wfSql;
            else
                sql = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE  FK_Emp='" + userNo + "' " + wfSql;

            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count != 0)
                return this.Name + "还存在待办，不能禁用该用户";

            BP.GPM.WFEmp wfEmp = new BP.GPM.WFEmp();
            wfEmp.Copy(this);
            wfEmp.UseSta = 0;//禁用
            wfEmp.Save();
            this.Delete();

            return this.Name + "已禁用";

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
    public class EmpExts : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpExt();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public EmpExts()
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
        public System.Collections.Generic.IList<EmpExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpExt> Tolist()
        {
            System.Collections.Generic.List<EmpExt> list = new System.Collections.Generic.List<EmpExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
