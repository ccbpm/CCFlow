using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template.Frm;


namespace BP.WF.Admin
{
    public class FlowAttr : BP.WF.Template.FlowAttr
    {

    }
    /// <summary>
    /// 流程
    /// </summary>
    public class Flow : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 存储表
        /// </summary>
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.PTable);
            }
            set
            {
                this.SetValByKey(FlowAttr.PTable, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(FlowAttr.FK_FlowSort, value);
            }
        }

        /// <summary>
        /// 是否可以独立启动
        /// </summary>
        public bool IsCanStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsCanStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsCanStart, value);
            }
        }
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin == false)
                    throw new Exception("err@管理员登录用户信息丢失,当前会话[" + WebUser.No + "," + WebUser.Name + "]");
                uac.IsUpdate = true;
                uac.IsDelete = true;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public Flow()
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

                Map map = new Map("WF_Flow", "流程模版");

                #region 基本属性。
                //处理流程类别.
                string sql = "";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    map.AddDDLEntities(FlowAttr.FK_FlowSort, null, "类别", new FlowSorts(), true);
                }
                else
                {
                    sql = "SELECT No,Name FROM WF_FlowSort WHERE OrgNo='@WebUser.OrgNo' ORDER BY No,Idx";
                    map.AddDDLSQL(FlowAttr.FK_FlowSort, null, "类别", sql, true);
                    map.AddTBString(FlowAttr.OrgNo, null, "组织编号", false, false, 0, 50, 10, false);
                    map.AddHidden(FlowAttr.OrgNo, " = ", BP.Web.WebUser.OrgNo);
                }

                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 4, 3);
                map.SetHelperUrl(FlowAttr.No, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661868&doc_id=31094"); //使用alert的方式显示帮助信息.
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 50, 300);

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "单号格式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3953012&doc_id=31094");


                map.AddTBString(FlowAttr.FlowEventEntity, null, "事件实体", true, true, 0, 150, 30);
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661871&doc_id=31094");

                map.AddTBString(FlowAttr.PTable, null, "存储表", true, false, 0, 30, 10);
                map.SetHelperUrl(FlowAttr.PTable, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=4000827&doc_id=31094");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661872&doc_id=31094");

                // map.AddBoolean(FlowAttr.IsCanStart, true, "独立启动？", true, true);

                map.AddDDLSysEnum(FlowAttr.IsCanStart, 1, "发布状态", true, false, "IsCanStart", "@0=不启用@1=独立启动");

                //map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？(独立启动的流程可以显示在发起流程列表里)", true, true, true);
                //map.SetHelperUrl(FlowAttr.IsCanStart, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661874&doc_id=31094");

                // // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, 0, "草稿规则", true, true, FlowAttr.Draft, "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱");
                map.SetHelperUrl(FlowAttr.Draft, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661878&doc_id=31094");
                #endregion 基本属性。

                map.AddTBString(FlowAttr.OrgNo, null, "组织编号", false, false, 0, 50, 10, false);

                map.AddTBString("Creater", "admin", "创建人", true, false, 0, 150, 10, true);
                map.AddTBDateTime(FlowAttr.CreateDate, null, "创建日期", true, false);


                //查询.
                map.AddSearchAttr(FlowAttr.FK_FlowSort);
                map.AddSearchAttr(FlowAttr.IsCanStart);



                #region 流程模版管理.
                RefMethod rm = new RefMethod();
                rm.Title = "流程模版";
                rm.Icon = "../../WF/Img/undo.png";
                rm.ClassMethodName = this.ToString() + ".DoExps()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-paper-plane";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Title = "重生成报表数据"; // "删除数据";
                rm.Warning = "您确定要执行吗? 注意:此方法耗费资源。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                rm.GroupName = "流程维护";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重生成流程标题";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                //设置相关字段.
                //rm.RefAttrKey = FlowAttr.TitleRole;
                rm.RefAttrLinkLabel = "重新生成流程标题";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要根据新的规则重新产生标题吗？";
                rm.GroupName = "流程维护";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重生成FlowEmps字段";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerFlowEmps()";
                rm.RefAttrLinkLabel = "补充修复emps字段，包括wf_generworkflow,NDxxxRpt字段.";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "您确定要重新生成吗？";
                rm.GroupName = "流程维护";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "流程维护";
                rm.Title = "删除指定日期范围内的流程";
                rm.Warning = "您确定要删除吗？";
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.HisAttrs.AddTBDateTime("DTFrom", null, "时间从", true, false);
                rm.HisAttrs.AddTBDateTime("DTTo", null, "时间到", true, false);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "仅仅当前流程");
                rm.Icon = "icon-briefcase";
                rm.ClassMethodName = this.ToString() + ".DoDelFlows";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "按工作ID删除"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.GroupName = "流程维护";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "删除备注", true, false, 0, 100, 100);
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "流程维护";
                rm.Title = "强制设置接收人";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.HisAttrs.AddTBInt("NodeID", 0, "节点ID", true, false);
                rm.HisAttrs.AddTBString("Worker", null, "接受人编号", true, false, 0, 100, 100);
                rm.Icon = "icon-briefcase";
                rm.ClassMethodName = this.ToString() + ".DoSetTodoEmps";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "按工作ID强制结束"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.GroupName = "流程维护";
                rm.ClassMethodName = this.ToString() + ".DoStopFlow";
                rm.HisAttrs.AddTBInt("WorkID", 0, "输入工作ID", true, false);
                rm.Icon = "icon-briefcase";
                rm.HisAttrs.AddTBString("beizhu", null, "备注", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "回滚流程";
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "您确定要回滚它吗？";
                rm.HisAttrs.AddTBInt("workid", 0, "请输入要会滚WorkID", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "回滚到的节点ID", true, false);
                rm.HisAttrs.AddTBString("note", null, "回滚原因", true, false, 0, 600, 200);
                rm.Icon = "icon-briefcase";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);


                //@hongyan.
                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Title = "删除模板"; // "删除数据";
                rm.IsCanBatch = true;
                rm.ClassMethodName = this.ToString() + ".DeleteIt";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                #endregion 流程运行维护.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        public string DeleteIt()
        {
            try
            {
                BP.WF.Flow fl = new BP.WF.Flow(this.No);
                fl.DoDelete();
                return "删除成功...";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string DoExps()
        {
            return "../../Admin/AttrFlow/Exp.htm?FK_Flow=" + this.No + "&Lang=CH";
        }

    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class Flows : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flows() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public Flows(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Flow();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Flow> ToJavaList()
        {
            return (System.Collections.Generic.IList<Flow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Flow> Tolist()
        {
            System.Collections.Generic.List<Flow> list = new System.Collections.Generic.List<Flow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Flow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

