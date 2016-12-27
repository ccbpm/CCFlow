using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 抄送控制方式
    /// </summary>
    public enum CtrlWay
    {
        /// <summary>
        /// 按照岗位
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按SQL
        /// </summary>
        BySQL
    }
	/// <summary>
	/// 抄送属性
	/// </summary>
    public class CCAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string CCTitle = "CCTitle";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string CCDoc = "CCDoc";
        /// <summary>
        /// 抄送控制方式
        /// </summary>
        public const string CCCtrlWay = "CCCtrlWay";
        /// <summary>
        /// 抄送对象
        /// </summary>
        public const string CCSQL = "CCSQL";

        public const string CCIsStations = "CCIsStations";
        public const string CCIsDepts = "CCIsDepts";
        public const string CCIsEmps = "CCIsEmps";
        public const string CCIsSQLs = "CCIsSQLs";
        #endregion
    }
	/// <summary>
	/// 抄送
	/// </summary>
    public class CC : Entity
    {
        #region 属性
        /// <summary>
        /// 抄送
        /// </summary>
        /// <param name="rpt"></param>
        /// <returns></returns>
        public DataTable GenerCCers(Entity rpt)
        {
            string sql = "";
            string ccSql = "";
            ccSql += this.CCSQL.Clone() as string;
            ccSql = ccSql.Replace("@WebUser.No", BP.Web.WebUser.No);
            ccSql = ccSql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            ccSql = ccSql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            switch (this.CCCtrlWay)
            {
                case CtrlWay.ByDept:
                    sql += "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_CCDept WHERE FK_Node=" + this.NodeID + "))";
                    break;
                case CtrlWay.ByEmp:
                    sql += "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_CCEmp WHERE FK_Node=" + this.NodeID + ")";
                    break;
                case CtrlWay.ByStation:
                    sql += "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ( SELECT FK_Station FROM WF_CCStation WHERE FK_Node=" + this.NodeID + "))";
                    break;
                case CtrlWay.BySQL:
                    if (this.CCIsStations == true)
                    {
                        sql = "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + ""
                        + " WHERE FK_Station IN ( SELECT FK_Station FROM WF_CCStation WHERE FK_Node=" + this.NodeID + "))"
                        + " AND NO IN(" + ccSql + ") ";
                    }
                    if (this.CCIsEmps == true)
                    {
                        sql += "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_CCEmp WHERE FK_Node=" + this.NodeID + ")"
                            + " AND NO IN(" + ccSql + ")";
                    }

                    if (this.CCIsDepts == true)
                    {
                        sql += "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_CCDept WHERE FK_Node=" + this.NodeID + "))"
                        + " AND NO IN(" + ccSql + ")";
                    }
                    if (this.CCIsSQLs == true)
                    {
                        sql = ccSql;
                    }
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                BP.DA.Log.DebugWriteWarning("@流程节点设计错误，未找到抄送人员，NodeID=[" + this.NodeID + "]。 SQL:" + sql);
                return dt;
            }
            return dt;
            //string sql = "";
            //if (this.CCIsSQLs == true)
            //{
            //    sql = "\t\n UNION    \t\n   ";
            //    sql += this.CCSQL.Clone() as string;
            //    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
            //    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            //    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            //    if (sql.Contains("@"))
            //    {
            //        foreach (Attr attr in rpt.EnMap.Attrs)
            //        {
            //            if (sql.Contains("@") == false)
            //                break;
            //            sql = sql.Replace("@" + attr.Key, rpt.GetValStrByKey(attr.Key));
            //        }
            //    }
            //}
            //if (this.CCIsEmps == true)
            //{
            //    sql += "\t\n UNION \t\n      SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_CCEmp WHERE FK_Node=" + this.NodeID + ")";
            //}

            //if (this.CCIsDepts == true)
            //{
            //    if (Glo.OSModel == Sys.OSModel.OneMore)
            //        sql += "\t\n UNION \t\n      SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_CCDept WHERE FK_Node=" + this.NodeID + "))";
            //    else
            //        sql += "\t\n UNION \t\n      SELECT No,Name FROM Port_Emp WHERE No IN (SELECT No FROM Port_Emp WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_CCDept WHERE FK_Node=" + this.NodeID + "))";
            //}

            //if (this.CCIsStations == true)
            //{
            //    sql += "\t\n UNION \t\n      SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ( SELECT FK_Station FROM WF_CCStation WHERE FK_Node=" + this.NodeID + "))";
            //}

            //if (sql.Length > 20)

            //    sql = sql.Substring("\t\n UNION  \t\n  ".Length );

            //DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count == 0)
            //{
            //    BP.DA.Log.DebugWriteWarning("@流程节点设计错误，未找到抄送人员，NodeID=[" + this.NodeID + "]。 SQL:" + sql);
            //    return dt;
            //}
            //return dt;
        }
        /// <summary>
        ///节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
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
                if (BP.Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 抄送标题
        /// </summary>
        public string CCTitle
        {
            get
            {
                string s= this.GetValStringByKey(CCAttr.CCTitle);
                if (string.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCTitle, value);
            }
        }
        /// <summary>
        /// 抄送内容
        /// </summary>
        public string CCDoc
        {
            get
            {
                string s = this.GetValStringByKey(CCAttr.CCDoc);
                if (string.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCDoc, value);
            }
        }
        /// <summary>
        /// 抄送对象
        /// </summary>
        public string CCSQL
        {
            get
            {
                string sql= this.GetValStringByKey(CCAttr.CCSQL);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("‘", "'");
                sql = sql.Replace("’", "'");
                sql = sql.Replace("''", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(CCAttr.CCSQL, value);
            }
        }
        /// <summary>
        /// 是否启用按照岗位抄送
        /// </summary>
        public bool CCIsStations
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsStations);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsStations, value);
            }
        }
        /// <summary>
        /// 是否启用按照部门抄送
        /// </summary>
        public bool CCIsDepts
        {
            get
            {
                return  this.GetValBooleanByKey(CCAttr.CCIsDepts);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsDepts, value);
            }
        }
        /// <summary>
        /// 是否启用按照人员抄送
        /// </summary>
        public bool CCIsEmps
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsEmps);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsEmps, value);
            }
        }
        /// <summary>
        /// 是否启用按照SQL抄送
        /// </summary>
        public bool CCIsSQLs
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsSQLs);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsSQLs, value);
            }
        }
        /// <summary>
        /// 抄送方式
        /// </summary>
        public CtrlWay CCCtrlWay
        {
            get
            {
                return (CtrlWay)this.GetValIntByKey(CCAttr.CCCtrlWay);
            }
            set
            {
                this.SetValByKey(CCAttr.CCCtrlWay, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// CC
        /// </summary>
        public CC()
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
                Map map = new Map("WF_Node", "抄送规则");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10, false);

                map.AddBoolean(CCAttr.CCIsStations, false, "按照岗位抄送", true, true, true);
                map.AddBoolean(CCAttr.CCIsDepts, false, "按照部门抄送", true, true, true);
                map.AddBoolean(CCAttr.CCIsEmps, false, "按照人员抄送", true, true, true);
                map.AddBoolean(CCAttr.CCIsSQLs, false, "按照SQL抄送", true, true, true);

                map.AddDDLSysEnum(CCAttr.CCCtrlWay, 0, "控制方式",true, true,"CtrlWay");

                map.AddTBString(CCAttr.CCSQL, null, "SQL表达式", true, false, 0, 500, 10, true);
                map.AddTBString(CCAttr.CCTitle, null, "抄送标题", true, false, 0, 500, 10,true);
                map.AddTBStringDoc(CCAttr.CCDoc, null, "抄送内容(标题与内容支持变量)", true, false,true);

                map.AddSearchAttr(CCAttr.CCCtrlWay);

                // 相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.Template.CCStations(), new BP.WF.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "抄送岗位");

                map.AttrsOfOneVSM.Add(new BP.WF.Template.CCDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                DeptAttr.No,  "抄送部门" );

                map.AttrsOfOneVSM.Add(new BP.WF.Template.CCEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, NodeEmpAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "抄送人员");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 抄送s
	/// </summary>
	public class CCs: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CC();
			}
		}
		/// <summary>
        /// 抄送
		/// </summary>
		public CCs(){} 		 
		#endregion


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CC> ToJavaList()
        {
            return (System.Collections.Generic.IList<CC>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CC> Tolist()
        {
            System.Collections.Generic.List<CC> list = new System.Collections.Generic.List<CC>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CC)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
