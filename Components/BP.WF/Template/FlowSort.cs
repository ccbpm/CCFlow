using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程类别属性
    /// </summary>
    public class FlowSortAttr : EntityTreeAttr
    {
    }
    /// <summary>
    ///  流程类别
    /// </summary>
    public class FlowSort : EntityTree
    {
        #region 构造方法
        /// <summary>
        /// 流程类别
        /// </summary>
        public FlowSort()
        {
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        /// <param name="_No"></param>
        public FlowSort(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 流程类别Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowSort", "流程类别");

                map.AddTBStringPK(FlowSortAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.TreeNo, null, "TreeNo", false, false, 0, 100, 30);

                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);
                map.AddTBInt(FlowSortAttr.IsDir, 0, "IsDir", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 重写方法
        public void WritToGPM()
        {
            return;

            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                return;

            string pMenuNo = "";

            #region 检查系统是否存在，并返回系统菜单编号
            string sql = "SELECT * FROM GPM_App where No='" + SystemConfig.SysNo + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count == 0)
            {
                //系统类别
                sql = "SELECT No FROM GPM_Menu WHERE ParentNo=0";
                string sysRootNo = DBAccess.RunSQLReturnStringIsNull(sql, "0");
                // 取得该功能菜单的主键编号.
                pMenuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
                string url = Glo.HostURL;
                /*如果没有系统，就插入该系统菜单.*/
                sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
                sql += " VALUES('{0}','{1}','{2}',1,2,'{3}',1,'{4}')";
                sql = string.Format(sql, pMenuNo, SystemConfig.SysName, sysRootNo, SystemConfig.SysNo, "FlowSort" + SystemConfig.SysNo);
                DBAccess.RunSQL(sql);
                /*如果没有系统，就插入该系统.*/
                sql = "INSERT INTO GPM_App(No,Name,AppModel,FK_AppSort,Url,RefMenuNo,MyFileName)";
                sql += " VALUES('{0}','{1}',0,'01','{2}','{3}','admin.gif')";

                sql = string.Format(sql, SystemConfig.SysNo, SystemConfig.SysName, url, pMenuNo);
                sql = sql.Replace("//", "/");
                DBAccess.RunSQL(sql);
            }
            else
            {
                pMenuNo = dt.Rows[0]["RefMenuNo"].ToString();
            }
            #endregion

            try
            {
                sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count >= 1)
                    DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "' ");
            }
            catch
            {
            }

            // 组织数据。
            // 获取他的ParentNo
            string parentNo = "";//this.ParentNo
            if (!string.IsNullOrEmpty(this.ParentNo))
            {
                sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowSort" + this.ParentNo + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count >= 1)
                {
                    pMenuNo = dt.Rows[0]["No"].ToString();
                }
            }

            string menuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
            // 执行插入.
            sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
            sql += " VALUES('{0}','{1}','{2}',{3},{4},'{5}',{6},'{7}')";
            sql = string.Format(sql, menuNo, this.Name, pMenuNo, 1, 3, SystemConfig.SysNo, 1, "FlowSort" + this.No);
            DBAccess.RunSQL(sql);
        }

        protected override bool beforeInsert()
        {
            this.WritToGPM();
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {
            try
            {
                //删除权限管理
                if (BP.WF.Glo.OSModel == OSModel.OneMore)
                    DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
            catch
            {
            }
            return base.beforeDelete();
        }

        protected override bool beforeUpdate()
        {
            //修改权限管理
            if (BP.WF.Glo.OSModel == OSModel.OneMore)
            {
              //  DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
            return base.beforeUpdate();
        }
        #endregion 重写方法

        #region 扩展方法
        /// <summary>
        /// 子文件夹
        /// </summary>
        public FlowSorts HisSubFlowSorts
        {
            get
            {
                try
                {
                    FlowSorts flowSorts = new FlowSorts();
                    flowSorts.RetrieveByAttr(FlowSortAttr.ParentNo, this.No);
                    return flowSorts;
                }
                catch
                {
                }
                return null;
            }
        }

        /// <summary>
        /// 类别下所包含流程
        /// </summary>
        public Flows HisFlows
        {
            get
            {
                try
                {
                    Flows flows = new Flows();
                    flows.RetrieveByAttr(Template.FlowAttr.FK_FlowSort, this.No);
                    return flows;
                }
                catch
                {
                }
                return null;
            }
        }
        /// <summary>
        /// 强制删除该流程类别下子项，递归
        /// </summary>
        /// <returns></returns>
        public bool DeleteFlowSortSubNode_Force()
        {
            try
            {
                //删除流程
                foreach (Flow flow in this.HisFlows)
                    flow.DoDelete();
                //删除类别
                foreach (FlowSort flowSort in this.HisSubFlowSorts)
                    Delete_FlowSort_SubNodes(flowSort);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 删除类别下所有项，子流程类别、流程
        /// </summary>
        /// <param name="FK_FlowSort">流程类别编号</param>
        /// <returns>true false</returns>
        private bool Delete_FlowSort_SubNodes(FlowSort sub_FlowSort)
        {
            try
            {
                //删除流程
                foreach (Flow flow in sub_FlowSort.HisFlows)
                    flow.DoDelete();
                //删除类别
                foreach (FlowSort flowSort in sub_FlowSort.HisSubFlowSorts)
                    Delete_FlowSort_SubNodes(flowSort);

                sub_FlowSort.Delete();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }
        #endregion
    }
    /// <summary>
    /// 流程类别
    /// </summary>
    public class FlowSorts : EntitiesTree
    {
        /// <summary>
        /// 流程类别s
        /// </summary>
        public FlowSorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSort();
            }

        }
        /// <summary>
        /// 流程类别s
        /// </summary>
        /// <param name="no">ss</param>
        /// <param name="name">anme</param>
        public void AddByNoName(string no, string name)
        {
            FlowSort en = new FlowSort();
            en.No = no;
            en.Name = name;
            this.AddEntity(en);
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                FlowSort fs = new FlowSort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSort> Tolist()
        {
            System.Collections.Generic.List<FlowSort> list = new System.Collections.Generic.List<FlowSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
