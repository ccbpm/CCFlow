using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    public class FlowFormTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 节点类型
        /// </summary>
        public const string NodeType = "NodeType";
        /// <summary>
        /// url
        /// </summary>
        public const string Url = "Url";
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class FlowFormTree : EntityTree
    {
        #region 扩展属性，不做数据操作
        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public string IsEdit { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        #endregion

        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Flow, value);
            }
        }
        #endregion 属性

        #region 构造方法
        /// <summary>
        /// 独立表单树
        /// </summary>
        public FlowFormTree()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        /// <param name="_No"></param>
        public FlowFormTree(string _No) : base(_No) { }
        #endregion

        ///// <summary>
        ///// 分组字段
        ///// </summary>
        //public override string RefObjField
        //{
        //    get { return FlowFormTreeAttr.FK_Flow; }
        //}
        /// <summary>
        /// 独立表单树Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowFormTree", "独立表单树");
                map.Java_SetCodeStruct("2");;
                map.Java_SetDepositaryOfEntity(Depositary.Application);


                map.AddTBStringPK(FlowFormTreeAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(FlowFormTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FlowFormTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBInt(FlowFormTreeAttr.Idx, 0, "Idx", false, false);

                // 隶属的流程编号.
                map.AddTBString(FlowFormTreeAttr.FK_Flow, null, "流程编号", true, true, 1, 20, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        //public void WritToGPM()
        //{
        //    if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
        //        return;

        //    string pMenuNo = "";
        //    #region 检查系统是否存在，并返回系统菜单编号
        //    string sql = "SELECT * FROM GPM_App where No='" + SystemConfig.SysNo + "'";

        //    DataTable dt = DBAccess.RunSQLReturnTable(sql);
        //    if (dt != null && dt.Rows.Count == 0)
        //    {
        //        //系统类别
        //        sql = "SELECT No FROM GPM_Menu WHERE ParentNo=0";
        //        string sysRootNo = DBAccess.RunSQLReturnStringIsNull(sql, "0");
        //        // 取得该功能菜单的主键编号.
        //        pMenuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
        //        string url = Glo.HostURL;
        //        /*如果没有系统，就插入该系统菜单.*/
        //        sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
        //        sql += " VALUES('{0}','{1}','{2}',1,2,'{3}',1,'{4}')";
        //        sql = string.Format(sql, pMenuNo, SystemConfig.SysName, sysRootNo, SystemConfig.SysNo, "FlowFormTree" + SystemConfig.SysNo);
        //        DBAccess.RunSQL(sql);
        //        /*如果没有系统，就插入该系统.*/
        //        sql = "INSERT INTO GPM_App(No,Name,AppModel,FK_AppSort,Url,RefMenuNo,MyFileName)";
        //        sql += " VALUES('{0}','{1}',0,'01','{2}','{3}','admin.gif')";

        //        sql = string.Format(sql, SystemConfig.SysNo, SystemConfig.SysName, url, pMenuNo);
        //        DBAccess.RunSQL(sql);
        //    }
        //    else
        //    {
        //        pMenuNo = dt.Rows[0]["RefMenuNo"].ToString();
        //    }
        //    #endregion

        //    try
        //    {
        //        sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowFormTree" + this.No + "'";
        //        dt = DBAccess.RunSQLReturnTable(sql);
        //        if (dt.Rows.Count >= 1)
        //            DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowFormTree" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "' ");
        //    }
        //    catch
        //    {
        //    }

        //    // 组织数据。
        //    // 获取他的ParentNo
        //    string parentNo = "";//this.ParentNo
        //    if (!DataType.IsNullOrEmpty(this.ParentNo))
        //    {
        //        sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowFormTree" + this.ParentNo + "'";
        //        dt = DBAccess.RunSQLReturnTable(sql);
        //        if (dt.Rows.Count >= 1)
        //        {
        //            pMenuNo = dt.Rows[0]["No"].ToString();
        //        }
        //    }

        //    string menuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
        //    // 执行插入.
        //    sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
        //    sql += " VALUES('{0}','{1}','{2}',{3},{4},'{5}',{6},'{7}')";
        //    sql = string.Format(sql, menuNo, this.Name, pMenuNo, 1, 3, SystemConfig.SysNo, 1, "FlowFormTree" + this.No);
        //    DBAccess.RunSQL(sql);
        //}
        //protected override bool beforeInsert()
        //{
        //    this.WritToGPM();
        //    return base.beforeInsert();
        //}


        //protected override bool beforeDelete()
        //{
        //    try
        //    {
        //        //删除权限管理
        //        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
        //            DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowFormTree" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
        //    }
        //    catch
        //    {
        //    }
        //    return base.beforeDelete();
        //}
        //protected override bool beforeUpdate()
        //{
        //    //修改权限管理
        //    if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
        //    {
        //        DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='FlowFormTree" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
        //    }
        //    return base.beforeUpdate();
        //}
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class FlowFormTrees : EntitiesTree
    {
        /// <summary>
        /// 独立表单树s
        /// </summary>
        public FlowFormTrees()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        public FlowFormTrees(string flowNo)
        {
           int i= this.Retrieve(FlowFormTreeAttr.FK_Flow, flowNo);
           if (i == 0)
           {
               FlowFormTree tree = new FlowFormTree();
               tree.No = "100";
               tree.FK_Flow = flowNo;
               tree.Name = "根目录";
              // tree.IsDir = false;
               tree.ParentNo = "0";
               tree.Insert();

               //创建一个节点.
               tree.DoCreateSubNode();
           }
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowFormTree();
            }

        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowFormTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowFormTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowFormTree> Tolist()
        {
            System.Collections.Generic.List<FlowFormTree> list = new System.Collections.Generic.List<FlowFormTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowFormTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
