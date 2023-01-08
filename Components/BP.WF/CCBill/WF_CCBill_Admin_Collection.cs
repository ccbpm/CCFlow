using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF;
using BP.WF.HttpHandler;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    /// 实体集合的处理
    /// </summary>
    public class WF_CCBill_Admin_Collection : DirectoryPageBase
    {
        #region 属性.
        /// <summary>
        /// 模块编号
        /// </summary>
        public string ModuleNo
        {
            get
            {
                string str = this.GetRequestVal("ModuleNo");
                return str;
            }
        }
        /// <summary>
        /// 菜单ID.
        /// </summary>
        public string MenuNo
        {
            get
            {
                string str = this.GetRequestVal("MenuNo");
                return str;
            }
        }

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
        public WF_CCBill_Admin_Collection()
        {

        }
        /// <summary>
        /// 新建实体流程
        /// </summary>
        /// <returns></returns>
        public string FlowNewEntity_Save()
        {
            //当前表单的信息
            MapData mapData = new MapData(this.FrmID);
            #region 第1步: 创建一个流程.
            //首先创建流程. 参数都通过 httrp传入了。
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel handler = new WF_Admin_CCBPMDesigner_FlowDevModel();
            string flowNo = handler.FlowDevModel_Save();

            //执行更新. 设置为不能独立启动.
            BP.WF.Flow fl = new BP.WF.Flow(flowNo);
            fl.IsCanStart = false;
            fl.TitleRole = "@WebUser.No 在@RDT 发起【@DictName】";
            fl.Update();

            //更新开始节点.
            BP.WF.Node nd = new BP.WF.Node(int.Parse(flowNo + "01"));
            nd.Name = this.Name;
            if (mapData.HisFrmType == FrmType.Develop)
            {
                nd.FormType = NodeFormType.Develop;
                MapData map = new MapData(nd.NodeFrmID);
                map.HisFrmType = FrmType.Develop;
                map.Update();
            }
            nd.Update();

            #endregion 创建一个流程.

            #region 第2步 把表单导入到流程上去.
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handlerFrm = new BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            string ndFrmID = "ND" + int.Parse(flowNo + "01");
            handlerFrm.Imp_CopyFrm(ndFrmID, this.FrmID);

            MapAttr attr = new MapAttr(ndFrmID + "_Title");
            attr.setUIVisible(false);
            attr.setName("流程标题");
            attr.Update();

            //生成名称字段.
            attr.setKeyOfEn("DictName");
            attr.setName("名称");
            attr.setUIVisible(true);
            attr.setMyPK(attr.FK_MapData + "_" + attr.KeyOfEn);
            attr.DirectInsert();


            #endregion 把表单导入到流程上去.


            //创建查询菜单.放入到与该实体平行的位置.
            BP.CCFast.CCMenu.Menu menu = new BP.CCFast.CCMenu.Menu();
            menu.ModuleNo = this.ModuleNo; //隶属与实体一个模块.
            menu.Name = this.Name;
            menu.Idx = 0;
            menu.MenuModel = MethodModelClass.FlowNewEntity; //新建实体流程..

            //menu.MenuModel = menuModel "FlowEtc"; //其他类型的业务流程.
            menu.Mark = "Search"; //流程查询.
            menu.Tag1 = flowNo; //流程编号.
            menu.No = this.FrmID + "_" + flowNo;
            menu.Icon = "icon-paper-plane";
            menu.Insert();


            //处理启动此流程后与实体的关系设计.
            MethodFlowNewEntity en = new MethodFlowNewEntity(menu.No);
            en.FlowNo = flowNo;
            en.FrmID = this.FrmID;
            en.DTSWhenFlowOver = true; // 是否在流程结束后同步?
            en.DTSDataWay = 1; // 同步所有相同的字段.
            en.UrlExt = "/WF/CCBill/Opt/StartFlowByNewEntity.htm?FlowNo=" + en.FlowNo + "&FrmID=" + this.FrmID + "&MenuNo=" + menu.No;
            en.Update();

            //增加一个集合链接.
            Collection enColl = new Collection();
            enColl.FrmID = this.FrmID;
            enColl.MethodID = MethodModelClass.FlowNewEntity;
            enColl.Mark = MethodModelClass.FlowNewEntity;
            enColl.Name = this.Name;

            enColl.FlowNo = flowNo;
            enColl.Tag1 = flowNo;

            enColl.MethodModel = MethodModelClass.FlowNewEntity; //方法模式.
            //   enColl.UrlExt = "../CCBill/Opt/StartFlowByNewEntity.htm?FlowNo=" + en.FlowNo + "&FrmID=" + this.FrmID + "&MenuNo=" + menu.No;
            enColl.Icon = "icon-drop";
            enColl.Idx = 100;
            enColl.SetPara("EnName", "TS.CCBill.CollectionFlowNewEntity");
            enColl.Insert();

            return menu.No; //返回的方法ID;
        }
        /// <summary>
        /// 单据批量发起流程
        /// </summary>
        /// <returns></returns>
        public string Bill_Save()
        {
            string fromFrmID = this.GetRequestVal("DictFrmID");
            string toFrmID = this.GetRequestVal("BillFrmID");

            //创建从表
            MapDtl mapDtl = new MapDtl();
            mapDtl.setFK_MapData(toFrmID);
            mapDtl.No = toFrmID + "Dtl1";
            mapDtl.FK_Node = 0;
            mapDtl.Name = "从表";
            mapDtl.PTable = mapDtl.No;
            mapDtl.H = 300;
            mapDtl.Insert();
            mapDtl.IntMapAttrs();

            //这里仅仅复制主表的字段.
            MapAttrs attrsFrom = new MapAttrs();
            attrsFrom.Retrieve(MapAttrAttr.FK_MapData, fromFrmID);
            foreach (MapAttr attr in attrsFrom)
            {
                if (attr.IsExit(MapAttrAttr.FK_MapData, mapDtl.No, MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                    continue;

                if (attr.IsExit(MapAttrAttr.FK_MapData, mapDtl.No, MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                    continue;

                attr.setFK_MapData(mapDtl.No);
                attr.setMyPK(attr.FK_MapData + "_" + attr.KeyOfEn);
                attr.Insert();
            }
            //增加一个关联的实体字段的OID
            MapAttr mapAttr = new BP.Sys.MapAttr();
            mapAttr.setFK_MapData(mapDtl.No);
            mapAttr.setEditType(EditType.Readonly);
            mapAttr.setKeyOfEn("DictOID");
            mapAttr.setName("关联实体的OID");
            mapAttr.setMyDataType(DataType.AppInt);
            mapAttr.setUIContralType(UIContralType.TB);
            mapAttr.setLGType(FieldTypeS.Normal);
            mapAttr.setUIVisible(false);
            mapAttr.setUIIsEnable(false);
            mapAttr.DefVal = "0";
            mapAttr.Insert();
            return "复制成功.";


        }
        /// <summary>
        /// 实体批量发起流程
        /// </summary>
        /// <returns></returns>
        public string FlowEntityBatchStart_Save()
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

            #region 第2步 把表单导入到流程的从表中去.
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            string frmID = "ND" + int.Parse(fl.No) + "01";
            MapDtl mapDtl = new MapDtl();
            mapDtl.setFK_MapData(frmID);
            mapDtl.No = frmID + "Dtl1";
            mapDtl.FK_Node = 0;
            mapDtl.Name = "从表";
            mapDtl.PTable = mapDtl.No;
            mapDtl.H = 300;
            mapDtl.Insert();
            mapDtl.IntMapAttrs();

            //这里仅仅复制主表的字段.
            MapAttrs attrsFrom = new MapAttrs();
            attrsFrom.Retrieve(MapAttrAttr.FK_MapData, this.FrmID);
            foreach (MapAttr attr in attrsFrom)
            {
                //是否包含，系统字段？
                if (BP.WF.Glo.FlowFields.Contains("," + attr.KeyOfEn + ",") == true)
                    continue;

                if (attr.IsExit(MapAttrAttr.FK_MapData, mapDtl.No, MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                    continue;

                attr.setFK_MapData(mapDtl.No);
                attr.setMyPK(attr.FK_MapData + "_" + attr.KeyOfEn);
                attr.Insert();
            }
            //增加一个关联的实体字段的OID
            MapAttr mapAttr = new BP.Sys.MapAttr();
            mapAttr.setFK_MapData(mapDtl.No);
            mapAttr.setEditType(EditType.Readonly);
            mapAttr.setKeyOfEn("DictOID");
            mapAttr.setName("关联实体的OID");
            mapAttr.setMyDataType(DataType.AppInt);
            mapAttr.setUIContralType(UIContralType.TB);
            mapAttr.setLGType(FieldTypeS.Normal);
            mapAttr.setUIVisible(false);
            mapAttr.setUIIsEnable(false);
            mapAttr.DefVal = "0";
            mapAttr.Insert();
            //更新开始节点.
            BP.WF.Node nd = new BP.WF.Node(int.Parse(flowNo + "01"));
            nd.Name = this.Name;
            nd.Update();
            #endregion 把表单导入到流程上去.


            //创建方法.
            BP.CCBill.Template.Method en = new BP.CCBill.Template.Method();
            en.FrmID = this.FrmID;
            en.No = en.FrmID + "_" + flowNo;
            en.Name = this.Name;
            en.GroupID = this.GroupID; //分组的编号.
            en.FlowNo = flowNo;
            en.Icon = "icon-paper-plane";
            en.RefMethodType = RefMethodType.LinkeWinOpen; // = 1;
            en.MethodModel = MethodModelClass.FlowEntityBatchStart; //类型.
            en.Mark = "Search"; //发起流程.
            en.Tag1 = flowNo; //标记为空.
            en.SetValByKey("IsCanBatch", 1);
            en.MethodID = flowNo; // 就是流程编号.

            en.FlowNo = flowNo;
            en.Insert();


            //增加一个集合链接.
            Collection enColl = new Collection();
            enColl.FrmID = this.FrmID;
            enColl.MethodID = MethodModelClass.FlowEntityBatchStart;
            enColl.Mark = MethodModelClass.FlowEntityBatchStart;
            enColl.Name = this.Name;

            enColl.FlowNo = flowNo;
            enColl.Tag1 = flowNo;

            enColl.MethodModel = MethodModelClass.FlowEntityBatchStart; //方法模式.
            enColl.Icon = "icon-drop";
            enColl.Insert();
            //返回方法编号。
            return en.No;
        }

    }
}
