using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.WF;
using BP.Sys;
using BP.WF.Data;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表定义
    /// </summary>
    public class RptDfineAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 查询的物理表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 部门查询权限控制规则
        /// </summary>
        public const string MyDeptRole = "MyDeptRole";
    }
    /// <summary>
    /// 报表定义
    /// </summary>
    public class RptDfine : EntityNoName
    {

        #region 属性
        /// <summary>
        /// 本部门流程查询权限定义
        /// </summary>
        public int MyDeptRole
        {
            get
            {
                return this.GetValIntByKey(RptDfineAttr.MyDeptRole);
            }
            set
            {
                this.SetValByKey(RptDfineAttr.MyDeptRole, value);
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    uac.IsDelete = false;
                    uac.IsView = true;
                    uac.IsInsert = false;
                }
                else
                {
                    uac.IsView = false;
                }
                return uac;
            }
        }
        /// <summary>
        /// 报表定义
        /// </summary>
        public RptDfine()
        {
        }
        /// <summary>
        /// 报表定义
        /// </summary>
        /// <param name="no">映射编号</param>
        public RptDfine(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "报表定义");

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("4"); ;

                #region 基本属性.
                map.AddTBStringPK(RptDfineAttr.No, null, "编号", true, false, 1, 200, 20);
                map.AddTBString(RptDfineAttr.Name, null, "流程名称", true, false, 0, 500, 20);

                map.AddDDLSysEnum(RptDfineAttr.MyDeptRole, 0, "本部门发起的流程", true,
                    true, RptDfineAttr.MyDeptRole,
                    "@0=仅部门领导可以查看@1=部门下所有的人都可以查看@2=本部门里指定岗位的人可以查看", true);
                
                //map.AddTBString(RptDfineAttr.PTable, null, "物理表", true, false, 0, 500, 20);
                //map.AddTBString(RptDfineAttr.Note, null, "备注", true, false, 0, 500, 20);
                #endregion 基本属性.

                #region 绑定的关联关系.
                map.AttrsOfOneVSM.Add(new RptStations(), new Stations(), RptStationAttr.FK_Rpt, RptStationAttr.FK_Station,
                DeptAttr.Name, DeptAttr.No, "岗位权限");
                map.AttrsOfOneVSM.Add(new RptDepts(), new Depts(), RptDeptAttr.FK_Rpt, RptDeptAttr.FK_Dept,
                    DeptAttr.Name, DeptAttr.No, "部门权限");
                map.AttrsOfOneVSM.Add(new RptEmps(), new Emps(), RptEmpAttr.FK_Rpt, RptEmpAttr.FK_Emp,
                 DeptAttr.Name, DeptAttr.No, "人员权限");
                #endregion

                #region 我发起的流程.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SelectCols.png";
                rm.ClassMethodName = this.ToString() + ".DoColsChoseOf_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Order.png";
                rm.ClassMethodName = this.ToString() + ".DoColsOrder_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SearchCond.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchCond_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoRptExportTemplate_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "执行查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Reset.png";
                rm.Warning = "您确定要执行吗?如果确定，以前配置将清空。";
                rm.ClassMethodName = this.ToString() + ".DoReset_MyStartFlow()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);
                #endregion 我发起的流程.

                #region 我审批的流程.
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SelectCols.png";
                rm.ClassMethodName = this.ToString() + ".DoColsChoseOf_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Order.png";
                rm.ClassMethodName = this.ToString() + ".DoColsOrder_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SearchCond.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchCond_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoRptExportTemplate_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我参与的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "执行查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "执行分析";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoGroup_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Reset.png";
                rm.Warning = "您确定要执行吗?";
                rm.ClassMethodName = this.ToString() + ".DoReset_MyJoinFlow()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "我审批的流程";
                map.AddRefMethod(rm);
                #endregion 我发起的流程.

                #region 我部门发起的流程.
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SelectCols.png";
                rm.ClassMethodName = this.ToString() + ".DoColsChoseOf_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Order.png";
                rm.ClassMethodName = this.ToString() + ".DoColsOrder_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SearchCond.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchCond_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoRptExportTemplate_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "执行查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Reset.png";
                rm.Warning = "您确定要执行吗?";
                rm.ClassMethodName = this.ToString() + ".DoReset_MyDeptFlow()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "本部门发起的流程";
                map.AddRefMethod(rm);
                #endregion 我部门发起的流程.

                #region 高级查询.
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SelectCols.png";
                rm.ClassMethodName = this.ToString() + ".DoColsChoseOf_AdminerFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Order.png";
                rm.ClassMethodName = this.ToString() + ".DoColsOrder_AdminerFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SearchCond.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchCond_AdminerFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoRptExportTemplate_AdminerFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "执行查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch_AdminerFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查询权限";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoReset_AdminerFlowRight()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Reset.png";
                rm.Warning = "您确定要执行吗?";
                rm.ClassMethodName = this.ToString() + ".DoReset_AdminerFlow()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "高级查询";
                map.AddRefMethod(rm);
                #endregion 高级查询.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 通用方法.
        /// <summary>
        /// 选择的列
        /// </summary>
        /// <param name="rptMark"></param>
        /// <returns></returns>
        public string DoColsChose(string rptMark)
        {
            return "../../Admin/RptDfine/S2ColsChose.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) +"Rpt"+ rptMark ;
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <param name="rptMark"></param>
        /// <returns></returns>
        public string DoColsOrder(string rptMark)
        {
            return "../../Admin/RptDfine/S3ColsLabel.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) +"Rpt"+ rptMark ;
        }
        /// <summary>
        /// 查询条件设置
        /// </summary>
        /// <param name="rptMark"></param>
        /// <returns></returns>
        public string DoSearchCond(string rptMark)
        {
            return "../../Admin/RptDfine/S5SearchCond.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) +"Rpt" + rptMark ;
        }
        /// <summary>
        /// 导出模版设置
        /// </summary>
        /// <param name="rptMark"></param>
        /// <returns></returns>
        public string DoRptExportTemplate(string rptMark)
        {
            return "../../Admin/RptDfine/S8_RptExportTemplate.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No)+"Rpt" + rptMark;
        }
        /// <summary>
        /// 重置设置.
        /// </summary>
        public string DoReset(string rptMark, string rptName)
        {
            MapData md = new MapData();
            md.No = "ND" + int.Parse(this.No) + "Rpt" + rptMark;
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = rptName;
                md.Insert();
            }

            md.RptIsSearchKey = true; //按关键查询.
            md.RptDTSearchWay = DTSearchWay.None; //按日期查询.
            md.RptDTSearchKey = "";

            //设置查询条件.
            switch (rptMark)
            {
                case "My":
                case "MyJoin":
                case "MyDept":
                    md.RptSearchKeys = "*WFSta*FK_NY*"; //查询条件.
                    break;
                case "Adminer":
                    md.RptSearchKeys = "*WFSta*FK_NY*"; //查询条件.
                    break;
                default:
                    break;
            }

            Flow fl = new Flow(this.No);
            md.PTable = fl.PTable;
            md.Update();

            string keys = ",OID,FK_Dept,FlowStarter,WFState,Title,FlowStarter,FlowStartRDT,FlowEmps,FlowDaySpan,FlowEnder,FlowEnderRDT,FK_NY,FlowEndNode,WFSta,";

            //查询出来所有的字段.
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.No) + "Rpt");
            attrs.Delete(MapAttrAttr.FK_MapData, md.No); // 删除已经有的字段。
            foreach (MapAttr attr in attrs)
            {
                if (keys.Contains("," + attr.KeyOfEn + ",") == false)
                    continue;

                attr.FK_MapData = md.No;
                attr.UIIsEnable = false;

                #region 判断特殊的字段.
                switch (attr.KeyOfEn)
                {
                    case GERptAttr.FK_Dept:
                        attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case GERptAttr.FK_NY:
                        attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        //attr.GroupID = groupID;
                        attr.Update();
                        break;
                    case GERptAttr.Title:
                        attr.UIWidth = 120;
                        break;
                    case GERptAttr.FlowStarter:
                        attr.UIIsEnable = false;
                        //attr.LGType = FieldTypeS.FK;
                        //attr.UIBindKey = "BP.Port.Emps";
                        //attr.UIContralType = UIContralType.DDL;
                        //attr.UIWidth = 120;
                        break;
                    case GERptAttr.FlowEndNode:
                        //attr.LGType = FieldTypeS.FK;
                        //attr.UIBindKey = "BP.WF.Template.NodeExts";
                        //attr.UIContralType = UIContralType.DDL;
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
                #endregion

                attr.Insert();
            }
            return "标记为: "+rptMark + "的报表，重置成功...";
        }
        #endregion

        #region 我发起的流程
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoColsChoseOf_MyStartFlow()
        {
            return this.DoColsChose("My");
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoColsOrder_MyStartFlow()
        {
            return DoColsOrder("My");
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoSearchCond_MyStartFlow()
        {
            return DoSearchCond("My");
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoRptExportTemplate_MyStartFlow()
        {
            return DoRptExportTemplate("My");
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public string DoReset_MyStartFlow()
        {
            return DoReset("My","我发起的流程");
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch_MyStartFlow()
        {
            return "../../RptDfine/Search.htm?SearchType=My&FK_Flow=" + this.No;
        }
        #endregion

        #region 我参与的流程
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoColsChoseOf_MyJoinFlow()
        {
            return this.DoColsChose("MyJoin");
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoColsOrder_MyJoinFlow()
        {
            return DoColsOrder("MyJoin");
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoSearchCond_MyJoinFlow()
        {
            return DoSearchCond("MyJoin");
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoRptExportTemplate_MyJoinFlow()
        {
            return DoRptExportTemplate("MyJoin");
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public string DoReset_MyJoinFlow()
        {
            return DoReset("MyJoin", "我审批的流程");
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch_MyJoinFlow()
        {
            return "../../RptDfine/Search.htm?SearchType=MyJoin&FK_Flow=" + this.No;
        }

        public string DoGroup_MyJoinFlow()
        {
            return "../../RptDfine/Group.htm?SearchType=MyJoin&FK_Flow=" + this.No;
        }
        #endregion 我审批的流程

        #region 本部门发起的流程
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoColsChoseOf_MyDeptFlow()
        {
            return this.DoColsChose("MyDept");
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoColsOrder_MyDeptFlow()
        {
            return DoColsOrder("MyDept");
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoSearchCond_MyDeptFlow()
        {
            return DoSearchCond("MyDept");
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoRptExportTemplate_MyDeptFlow()
        {
            return DoRptExportTemplate("MyDept");
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public string DoReset_MyDeptFlow()
        {
            return DoReset("MyDept", "本部门发起的流程");
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch_MyDeptFlow()
        {
            return "../../RptDfine/Search.htm?SearchType=MyDept&FK_Flow=" + this.No;
        }
        #endregion 本部门发起的流程


        #region 高级查询
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoColsChoseOf_AdminerFlow()
        {
            return this.DoColsChose("Adminer");
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoColsOrder_AdminerFlow()
        {
            return DoColsOrder("Adminer");
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoSearchCond_AdminerFlow()
        {
            return DoSearchCond("Adminer");
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoRptExportTemplate_AdminerFlow()
        {
            return DoRptExportTemplate("Adminer");
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public string DoReset_AdminerFlow()
        {
            return DoReset("Adminer", "本部门发起的流程");
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch_AdminerFlow()
        {
            return "../../RptDfine/Search.htm?SearchType=Adminer&FK_Flow=" + this.No;
        }

        public string DoReset_AdminerFlowRight()
        {
            return "../../Admin/RptDfine/AdvSearchRight.htm?FK_Flow=" + this.No;
        }
        #endregion 高级查询

    }
    /// <summary>
    /// 报表定义s
    /// </summary>
    public class RptDfines : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 报表定义s
        /// </summary>
        public RptDfines()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new RptDfine();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<RptDfine> ToJavaList()
        {
            return (System.Collections.Generic.IList<RptDfine>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RptDfine> Tolist()
        {
            System.Collections.Generic.List<RptDfine> list = new System.Collections.Generic.List<RptDfine>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RptDfine)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
