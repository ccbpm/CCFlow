using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.MES
{
	/// <summary>
	/// 工作人员属性
	/// </summary>
	public class EmpAttr: BP.En.EntityNoNameAttr
	{
		#region 基本属性
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// SID
        /// </summary>
        public const string SID = "SID";
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
         
        /// <summary>
        /// 工作人员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 工作人员编号
        /// </summary>
        /// <param name="_No">No</param>
        public Emp(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex1)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@用户或者密码错误：[" + no + "]，或者帐号被停用。@技术信息(从内存中查询出现错误)：ex1=" + ex1.Message);
            }
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
                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 1, 20, 100);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 100);
                map.AddTBString(EmpAttr.Pass, "123", "密码", false, false, 0, 20, 10);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new BP.Port.Depts(), true);
                map.AddTBString(EmpAttr.SID, null, "SID", false, false, 0, 20, 10);
                map.AddTBString(EmpAttr.Tel, null, "手机号码", false, false, 0, 11, 30);

                map.AddBoolean("IsDiaoDu", false, "调度.", true, false);
                map.AddBoolean("IsZhuangPei", false, "装配人员.", true, false);
                map.AddBoolean("IsDaBao", false, "是否打包", true, false);
                map.AddBoolean("IsChecker", false, "检测人员.", true, false);
                map.AddBoolean("IsFaHuo", false, "发货人员", true, false);

                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept); //查询条件.
 


                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 重置当前用户编号
        /// </summary>
        /// <param name="userNo">当前用户编号</param>
        /// <returns>返回重置信息</returns>
        public string DoChangeUserNo(string userNo)
        {
            if (BP.Web.WebUser.No != "admin")
                return "非超级管理员，不能执行。";

            string msg = "";
            int i = 0;
            //更新待办.
            string sql = "update wf_generworkerlist set fk_emp='"+userNo+"' where fk_emp='"+this.No+"'";
            i= DBAccess.RunSQL(sql);
            if (i != 0)
                msg += "@待办更新[" + i + "]个。";

            sql = "UPDATE WF_GENERWORKFLOW  SET STARTER='"+userNo+"'  WHERE STARTER='"+this.No+"'";
            i = DBAccess.RunSQL(sql);
            if (i != 0)
                msg += "@流程注册更新[" + i + "]个。";


            //更换流程信息的数据表
            BP.WF.Flows fls = new Flows();
            fls.RetrieveAll();
            foreach (Flow fl in fls)
            {
                sql = "UPDATE " + fl.PTable + " SET FlowEnder='" + userNo + "' WHERE FlowEnder='" + this.No + "'";
                i = DBAccess.RunSQL(sql);

                if (i != 0)
                    msg += "@流程注册更新[" + i + "]个。";

                sql = "UPDATE  " + fl.PTable + "  SET FlowStarter='" + userNo + "' WHERE FlowStarter='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@流程业务表发起人，更新了[" + i + "]个。";


                sql = "UPDATE  " + fl.PTable + "  SET Rec='" + userNo + "' WHERE Rec='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@流程业务表记录人，更新了[" + i + "]个。";

                string trackTable = "ND" + int.Parse(fl.No) + "Track";
                sql = "UPDATE  " + trackTable + "  SET EmpFrom='" + userNo + "' WHERE EmpFrom='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@轨迹表 EmpFrom，更新了[" + i + "]个。";


                sql = "UPDATE  " + trackTable + "  SET EmpTo='" + userNo + "' WHERE EmpTo='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@轨迹表 EmpTo，更新了[" + i + "]个。";


                sql = "UPDATE  " + trackTable + "  SET Exer='" + userNo + "' WHERE Exer='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@轨迹表 Exer，更新了[" + i + "]个。";
            }


            //更新其他字段.
            BP.Sys.MapAttrs attrs = new Sys.MapAttrs();
            attrs.RetrieveAll();
            foreach (BP.Sys.MapAttr attr in attrs)
            {
                if (attr.DefValReal.Contains("@WebUser.No") == true)
                {
                    try
                    {
                        BP.Sys.MapData md = new Sys.MapData(attr.FK_MapData);
                        sql = "UPDATE " + md.PTable + " SET " + attr.KeyOfEn + "='" + userNo + "' WHERE " + attr.KeyOfEn + "='" + this.No + "'";
                        i = DBAccess.RunSQL(sql);
                        if (i != 0)
                            msg += "@表[" + md.Name + "],[" + md.PTable + "] [" + attr.KeyOfEn + "]，更新了[" + i + "]个。";
                    }
                    catch
                    {

                    }
                }
            }
            //人员主表信息-手动修改

            return msg;
        }
        /// <summary>
        /// 执行禁用
        /// </summary>
        public string DoDisableIt()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.UseSta = 0;
            emp.Update();
            return "已经执行(禁用)成功";
        }
        /// <summary>
        /// 执行启用
        /// </summary>
        public string DoEnableIt()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.UseSta = 1;
            emp.Update();
            return "已经执行(启用)成功";
        }

        protected override bool beforeDelete()
        {
            //if (BP.Web.WebUser.IsAdmin == false)
            //    throw new Exception("err@非管理员不能删除.");

            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.Update();
            return base.beforeUpdate();
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
 