﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.HttpHandler;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin_CreateFunc : DirectoryPageBase
    {
        #region 属性.
        public string GroupID
        {
            get
            {
                string str = this.GetRequestVal("GroupID");
                return str;
            }
        }
        public string Name
        {
            get
            {
                string str = this.GetRequestVal("Name");
                return str;
            }
        }
        #endregion 属性.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin_CreateFunc()
        {

        }
        /// <summary>
        /// 其他业务流程
        /// </summary>
        /// <returns></returns>
        public string FlowEtc_Save()
        {
            #region 第1步: 创建一个流程.
            //首先创建流程. 参数都通过 httrp传入了。
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel handler = new WF_Admin_CCBPMDesigner_FlowDevModel();
            string flowNo = handler.FlowDevModel_Save();

            //执行更新. 设置为不能独立启动.
            BP.WF.Flow fl = new WF.Flow(flowNo);
            fl.IsCanStart = false;
            fl.Update();
            #endregion 创建一个流程.

            #region 第2步 把表单导入到流程上去.
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handlerFrm = new WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            handlerFrm.Imp_CopyFrm("ND" + int.Parse(flowNo + "01"), this.FrmID);

            //更新开始节点.
            BP.WF.Node nd = new WF.Node(int.Parse(flowNo + "01"));
            nd.Name = this.Name;
            nd.Update();
            #endregion 把表单导入到流程上去.


            // 第4步: 创建实体分组的方法.
            CrateFlowMenu_4_GroupMethod(MethodModelClass.FlowEtc, flowNo);

            //创建流程目录与流程菜单.
            CrateFlow_5_Module(MethodModelClass.FlowEtc, flowNo);

            return this.FrmID + "_" + flowNo; //返回的方法ID;
        }
        /// <summary>
        /// 新建实体流程
        /// </summary>
        /// <returns></returns>
        public string FlowNewEntity_Save()
        {
            #region 第1步: 创建一个流程.
            //首先创建流程. 参数都通过 httrp传入了。
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel handler = new WF_Admin_CCBPMDesigner_FlowDevModel();
            string flowNo = handler.FlowDevModel_Save();

            //执行更新. 设置为不能独立启动.
            BP.WF.Flow fl = new WF.Flow(flowNo);
            fl.IsCanStart = false;
            fl.Update();
            #endregion 创建一个流程.

            #region 第2步 把表单导入到流程上去.
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handlerFrm = new WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            handlerFrm.Imp_CopyFrm("ND" + int.Parse(flowNo + "01"), this.FrmID);

            //更新开始节点.
            BP.WF.Node nd = new WF.Node(int.Parse(flowNo + "01"));
            nd.Name = this.Name;
            nd.Update();
            #endregion 把表单导入到流程上去.

            // 第4步: 创建实体分组的方法.
            //   CrateFlowMenu_4_GroupMethod(MethodModelClass.FlowNewEntity, flowNo);

            //创建流程目录与流程菜单.
            CrateFlow_5_Module(MethodModelClass.FlowNewEntity, flowNo);

            string pkval = this.FrmID + "_" + flowNo;

            //处理启动此流程后与实体的关系设计.
            MethodFlowNewEntity en = new MethodFlowNewEntity(pkval);
            en.FlowNo = flowNo;
            en.FrmID = this.FrmID;
            en.DTSWhenFlowOver = true; // 是否在流程结束后同步?
            en.DTSDataWay = 1; // 同步所有相同的字段.
            en.UrlExt = "../CCBill/Opt/StartFlowByNewEntity.htm?FlowNo=" + en.FlowNo + "&FrmID=" + this.FrmID+"&MenuNo="+pkval;
            en.Update();

            return pkval; //返回的方法ID;
        }
        /// <summary>
        /// 创建基础信息变更流程
        /// </summary>
        /// <returns></returns>
        public string FlowBaseData_Save()
        {

            #region 第1步: 创建一个流程.
            //首先创建流程. 参数都通过 httrp传入了。
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel handler = new WF_Admin_CCBPMDesigner_FlowDevModel();
            string flowNo = handler.FlowDevModel_Save();

            //执行更新. 设置为不能独立启动.
            BP.WF.Flow fl = new WF.Flow(flowNo);
            fl.IsCanStart = false;
            fl.Update();
            #endregion 创建一个流程.

            #region 第2步 把表单导入到流程上去.
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handlerFrm = new WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            handlerFrm.Imp_CopyFrm("ND" + int.Parse(flowNo + "01"), this.FrmID);

            //更新开始节点.
            BP.WF.Node nd = new WF.Node(int.Parse(flowNo + "01"));
            nd.Name = this.Name;
            nd.Update();
            #endregion 把表单导入到流程上去.

            #region 第3步： 处理流程的业务表单 - 字段增加一个影子字段.
            //处理字段数据.增加一个列.
            string frmID = "ND" + int.Parse(fl.No + "01");
            MapData md = new MapData(frmID);
            if (md.TableCol != 0)
            {
                md.TableCol = 0; //设置为4列.
                md.Update();
            }

            //查询出来数据.
            MapAttrs mapAttrs = new MapAttrs(md.No);
            GroupFields gfs = new GroupFields(md.No);

            //遍历分组.
            foreach (GroupField gs in gfs)
            {
                //遍历字段.
                int idx = 0;
                foreach (MapAttr mapAttr in mapAttrs)
                {
                    if (gs.OID != mapAttr.GroupID)
                        continue;

                    //是否包含，系统字段？
                    if (BP.WF.Glo.FlowFields.Contains("," + mapAttr.KeyOfEn + ",") == true)
                        continue;

                    //其他类型的控件，就排除.
                    if ((int)mapAttr.UIContralType >= 5)
                        continue;

                    if (mapAttr.UIVisible == false)
                        continue;

                    idx++;
                    idx++;
                    mapAttr.Idx = idx;
                    mapAttr.Update();
                    //  DBAccess.RunSQL("UP")

                    //复制一个影子字段.
                    mapAttr.KeyOfEn = "bak" + mapAttr.KeyOfEn;
                    mapAttr.Name = "(原)" + mapAttr.Name;

                    mapAttr.MyPK = mapAttr.FK_MapData + "_" + mapAttr.KeyOfEn;
                    mapAttr.UIIsEnable = false;
                    mapAttr.Idx = idx - 1;
                    mapAttr.DirectInsert();
                }
            }
            #endregion 处理流程的业务表单 - 字段增加一个影子字段..

            // 第4步: 创建实体分组的方法.
            CrateFlowMenu_4_GroupMethod(MethodModelClass.FlowBaseData, flowNo);


            // 创建流程目录与流程菜单.
            CrateFlow_5_Module(MethodModelClass.FlowBaseData, flowNo);



            return this.FrmID + "_" + flowNo; //返回的方法ID;
        }
        /// <summary>
        /// 创建方法分组.
        /// </summary>
        /// <param name="menuModel"></param>
        /// <param name="flowNo"></param>
        private void CrateFlowMenu_4_GroupMethod(string menuModel, string flowNo)
        {
            #region 第4步: 创建实体分组/方法.

            GroupMethod gm = new GroupMethod();
            gm.Name = this.Name;
            gm.FrmID = this.FrmID;
            gm.MethodType = menuModel; //类型.
            gm.MethodID = flowNo;
            gm.Insert();


            //创建 - 方法
            BP.CCBill.Template.Method en = new Template.Method();
            en.FrmID = this.FrmID;
            en.No = en.FrmID + "_" + flowNo;
            en.Name = this.Name;
            en.GroupID = gm.No; //分组的编号.
            en.FlowNo = flowNo;
            en.Icon = "icon-plane";

            en.RefMethodType = RefMethodType.LinkeWinOpen; // = 1;
            en.MethodModel = menuModel; //类型.
            en.Mark = "StartFlow"; //发起流程.
            en.Tag1 = flowNo; //标记为空.
            en.MethodID = flowNo; // 就是流程编号.
            en.FlowNo = flowNo;
            en.Insert();

            // 增加内置流程方法:发起查询.
            en.Name = "流程查询";
            en.Icon = "icon-grid";

            en.MethodModel = menuModel; //类型.
            en.Mark = "Search"; //流程查询.
            en.Tag1 = flowNo; //标记为空.
            en.MethodID = flowNo; // 就是流程编号.
            en.FlowNo = flowNo;
            en.No = DBAccess.GenerGUID();
            en.Insert();


            // 增加内置流程方法:流程分析.
            en.Name = "流程分析";
            en.Icon = "icon-chart";
            en.MethodModel = menuModel; //类型.
            en.Mark = "Group"; //流程分析.
            en.Tag1 = flowNo; //标记为空.
            en.MethodID = flowNo; // 就是流程编号.
            en.FlowNo = flowNo;
            en.No = DBAccess.GenerGUID();
            en.Insert();
            #endregion 第4步 创建方法.
        }
        /// <summary>
        /// 创建菜单分组
        /// </summary>
        /// <param name="menuModel"></param>
        /// <param name="flowNo"></param>
        private void CrateFlow_5_Module(string menuModel, string flowNo)
        {
            #region 第5步: 创建菜单目录与菜单-分组
            //创建该模块下的 菜单:分组.
            BP.GPM.Menu2020.Module mmodule = new GPM.Menu2020.Module();
            mmodule.Name = this.Name;
            mmodule.SystemNo = this.GetRequestVal("SortNo"); // md.FK_FormTree; //设置类别.
            mmodule.Idx = 100;
            mmodule.Insert();

            //创建菜单.
            BP.GPM.Menu2020.Menu menu = new GPM.Menu2020.Menu();

            //流程查询.
            menu = new BP.GPM.Menu2020.Menu();
            menu.ModuleNo = mmodule.No;
            menu.MenuCtrlWay = GPM.Menu2020.MenuCtrlWay.Anyone;
            menu.Name = "发起流程";
            menu.Idx = 0;

            menu.MenuModel = menuModel; //模式.
            menu.Mark = "StartFlow"; //发起流程.
            menu.Tag1 = flowNo; //流程编号.
            menu.UrlExt = "../MyFlow.htm?FK_Flow=" + flowNo;
            menu.No = this.FrmID + "_" + flowNo;
            menu.Icon = "icon-paper-plane";
            menu.Insert();

            //待办.
            menu = new BP.GPM.Menu2020.Menu();
            menu.ModuleNo = mmodule.No;
            menu.MenuCtrlWay = GPM.Menu2020.MenuCtrlWay.Anyone;
            menu.Name = "待办";

            menu.MenuModel = menuModel;
            menu.Mark = "Todolist";
            menu.Tag1 = flowNo;
            menu.UrlExt = "../Todolist.htm?FK_Flow=" + flowNo;
            menu.Icon = "icon-bell";
            menu.Idx = 1;
            menu.Insert();

            //未完成.
            menu = new BP.GPM.Menu2020.Menu();
            menu.MenuModel = menuModel; //模式.
            menu.ModuleNo = mmodule.No;
            menu.MenuCtrlWay = GPM.Menu2020.MenuCtrlWay.Anyone;
            menu.Name = "未完成(在途)";
            menu.Mark = "Runing"; //未完成.
            menu.Tag1 = flowNo; //流程编号.
            menu.Idx = 2;
            menu.UrlExt = "../Runing.htm?FK_Flow=" + flowNo;
            menu.Icon = "icon-clock";
            menu.Insert();

            //流程查询.
            menu = new BP.GPM.Menu2020.Menu();
            menu.ModuleNo = mmodule.No;
            menu.MenuCtrlWay = GPM.Menu2020.MenuCtrlWay.Anyone;
            menu.Name = "流程查询";

            menu.MenuModel = menuModel; //模式.
            menu.Mark = "FlowSearch"; //流程查询.
            menu.Tag1 = flowNo; //流程编号.
            menu.UrlExt = "/App/OneFlow/RptSearch.htm?FK_Flow=" + flowNo;
            menu.Idx = 3;
            menu.Icon = "icon-magnifier";
            menu.Insert();

            //流程查询.
            menu = new BP.GPM.Menu2020.Menu();
            menu.MenuModel = menuModel; //模式.
            menu.ModuleNo = mmodule.No;
            menu.MenuCtrlWay = GPM.Menu2020.MenuCtrlWay.Anyone;
            menu.Name = "流程分析";

            menu.MenuModel = menuModel; //模式.
            menu.Mark = "FlowGroup"; //流程查询.
            menu.Tag1 = flowNo; //流程编号.
            menu.Idx = 4;
            menu.UrlExt = "/App/OneFlow/RptGroup.htm?FK_Flow=" + flowNo;
            menu.Icon = "icon-chart";
            menu.Insert();
            #endregion 第5步 创建目录.
        }

    }
}
