using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF
{
	/// <summary>
	/// 开始工作基类属性
	/// </summary>
    public class StartWorkAttr : WorkAttr
    {
        #region 属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 工作内容标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// PRI
        /// </summary>
        public const string PRI = "PRI";
        #endregion

        #region 父子流程属性
        public const string PFlowNo = "PFlowNo";
        public const string PWorkID = "PWorkID";
        public const string PNodeID = "PNodeID";
        #endregion 父子流程属性
    }
	/// <summary>	 
	/// 开始工作基类,所有开始工作都要从这里继承
	/// </summary>
	abstract public class StartWork : Work 
	{
        #region 与_SQLCash 操作有关
        private SQLCash _SQLCash = null;
        public override SQLCash SQLCash
        {
            get
            {
                if (_SQLCash == null)
                {
                    _SQLCash = BP.DA.Cash.GetSQL("ND" + this.NodeID.ToString());
                    if (_SQLCash == null)
                    {
                        _SQLCash = new SQLCash(this);
                        BP.DA.Cash.SetSQL("ND" + this.NodeID.ToString(), _SQLCash);
                    }
                }
                return _SQLCash;
            }
            set
            {
                _SQLCash = value;
            }
        }
        #endregion

		#region  单据属性
		/// <summary>
		/// FK_Dept
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(StartWorkAttr.FK_Dept);
			}
            set
            {
                this.SetValByKey(StartWorkAttr.FK_Dept, value);
            } 
		}
        //public string FK_DeptOf2Code
        //{
        //    get
        //    {
        //        return this.FK_Dept.Substring(6);
        //    } 
        //}
		/// <summary>
		/// FK_XJ
		/// </summary>
        //public string FK_XJ
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(StartWorkAttr.FK_Dept);
        //    }
        //    set
        //    {
        //        this.SetValByKey(StartWorkAttr.FK_Dept, value);
        //    }
        //}
		#endregion

		#region 基本属性
		/// <summary>
		/// 工作内容标题
		/// </summary>
		public string Title
		{
			get
			{
				return this.GetValStringByKey(StartWorkAttr.Title);
			}
			set
			{
				this.SetValByKey(StartWorkAttr.Title,value);
			} 
		}
		#endregion

		#region 构造函数
		/// <summary>
		/// 工作流程
		/// </summary>
		protected StartWork()
		{
		}
        protected StartWork(Int64 oid):base(oid)
        {
        }
		#endregion
		
		#region  重写基类的方法。
		/// <summary>
		/// 删除之前的操作。
		/// </summary>
		/// <returns></returns>
		protected override bool beforeDelete() 
		{
			if (base.beforeDelete()==false)
				return false;			 
			if (this.OID < 0 )
				throw new Exception("@实体["+this.EnDesc+"]没有被实例化，不能Delete().");
			return true;
		}
		/// <summary>
		/// 插入之前的操作。
		/// </summary>
		/// <returns></returns>
        protected override bool beforeInsert()
        {
            if (this.OID > 0)
                throw new Exception("@实体[" + this.EnDesc + "], 已经被实例化，不能Insert.");

            this.SetValByKey("OID", DBAccess.GenerOID());
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.Emps = BP.Web.WebUser.No;
            return base.beforeUpdateInsertAction();
        }
		/// <summary>
		/// 更新操作
		/// </summary>
		/// <returns></returns>
		protected override bool beforeUpdate()
		{
			if (base.beforeUpdate()==false)
				return false;
			if (this.OID < 0 )			
				throw new Exception("@实体["+this.EnDesc+"]没有被实例化，不能Update().");
			return base.beforeUpdate();
		}
		#endregion
	}
	/// <summary>
	/// 工作流程采集信息的基类 集合
	/// </summary>
	abstract public class StartWorks : Works
	{
		#region 构造方法
		/// <summary>
		/// 信息采集基类
		/// </summary>
		public StartWorks()
		{
		}
		#endregion 

		#region 公共查询方法
		/// <summary>
		/// 查询到我的任务.
		/// </summary>		 
		/// <returns></returns>
		public DataTable RetrieveMyTask_del(string flow)
		{
			QueryObject qo = new QueryObject(this);
			//qo.Top=50;
            qo.AddWhere(StartWorkAttr.OID, " in ", " ( SELECT WorkID FROM V_WF_Msg  WHERE  FK_Flow='" + flow + "' AND FK_Emp='" + BP.Web.WebUser.No + "' )");
			return qo.DoQueryToTable();
		}
		/// <summary>
		/// 查询到我的任务.
		/// </summary>		 
		/// <returns></returns>
		public DataTable RetrieveMyTask(string flow)
		{
			//string sql="SELECT OID AS WORKID, TITLE, ";
			QueryObject qo = new QueryObject(this);
			//qo.Top=50;
			if (BP.Sys.SystemConfig.AppCenterDBType==DBType.Oracle)
                qo.AddWhere(StartWorkAttr.OID, " in ", " (  SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Node IN ( SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND FK_Flow='" + flow + "' AND WORKID=WF_GenerWorkFlow.WORKID ) )");
			else
                qo.AddWhere(StartWorkAttr.OID, " in ", " (  SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Node IN ( SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND FK_Flow='" + flow + "' AND WORKID=WF_GenerWorkFlow.WORKID ) )");
			return qo.DoQueryToTable();
		}
		/// <summary>
		/// 查询到我的任务.
		/// </summary>		 
		/// <returns></returns>
		public DataTable RetrieveMyTaskV2(string flow)
		{
            string sql = "SELECT OID, TITLE, RDT, Rec FROM  " + this.GetNewEntity.EnMap.PhysicsTable + " WHERE OID IN (   SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Node IN ( SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND FK_Flow='" + flow + "' AND WORKID=WF_GenerWorkFlow.WORKID )  )";
			return DBAccess.RunSQLReturnTable(sql) ;
			/*
			QueryObject qo = new QueryObject(this);
			//qo.Top=50;
			qo.AddWhere(StartWorkAttr.OID," in ", " ( SELECT WorkID FROM V_WF_Msg  WHERE  FK_Flow='"+flow+"' AND FK_Emp="+Web.WebUser.No+")" );
			return qo.DoQueryToTable();
			*/
		}
		/// <summary>
		/// 查询到我的任务.
		/// </summary>		 
		/// <returns></returns>
		public DataTable RetrieveMyTask(string flow,string flowSort)
		{
			QueryObject qo = new QueryObject(this);
			//qo.Top=50;
            qo.AddWhere(StartWorkAttr.OID, " IN ", " ( SELECT WorkID FROM V_WF_Msg  WHERE  (FK_Flow='" + flow + "' AND FK_Emp='" + BP.Web.WebUser.No + "' ) AND ( FK_Flow in ( SELECT No FROM WF_Flow WHERE FK_FlowSort='" + flowSort + "' )) )");
			return qo.DoQueryToTable();			 
		}
		#endregion 
	}
}
