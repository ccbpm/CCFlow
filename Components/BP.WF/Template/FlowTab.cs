using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 抄送属性
    /// </summary>
    public class FlowTabAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 标记
        /// </summary>
        public const string Mark = "Mark";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        public const string UrlExt = "UrlExt";
        public const string Icon = "Icon";
        public const string OrgNo = "OrgNo";
        public const string Tip = "Tip";
        public const string Idx = "Idx";
        #endregion
    }
    /// <summary>
    /// 抄送
    /// </summary>
    public class FlowTab : EntityMyPK
    {
        #region 属性
        public string Name
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.Name);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.Name, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.FK_Flow, value);
            }
        }
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowTabAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.IsEnable, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.OrgNo, value);
            }
        }
        public string Tip
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.Tip);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.Tip, value);
            }
        }
        public string UrlExt
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.UrlExt);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.UrlExt, value);
            }
        }
        public string Mark
        {
            get
            {
                return this.GetValStrByKey(FlowTabAttr.Mark);
            }
            set
            {
                this.SetValByKey(FlowTabAttr.Mark, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 抄送设置
        /// </summary>
        public FlowTab()
        {
        }
        /// <summary>
        /// 抄送设置
        /// </summary>
        /// <param name="mypk"></param>
        public FlowTab(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
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

                Map map = new Map("WF_FlowTab", "流程功能");

                map.AddMyPK();
                map.AddTBString(FlowTabAttr.Name, null, "标签", true, true, 0, 100, 10, false);
                map.AddTBString(FlowTabAttr.FK_Flow, null, "流程编号", false, false, 0, 4, 10);
                map.AddTBString(FlowTabAttr.Mark, null, "标记", false, false, 0, 50, 10);
                map.AddTBString(FlowTabAttr.Tip, null, "Tip", false, false, 0, 200, 10);

                map.AddTBString(FlowTabAttr.UrlExt, null, "url链接", false, false, 0, 300, 10);
                map.AddTBString(FlowTabAttr.Icon, null, "Icon", false, false, 0, 50, 10);
                map.AddTBString(FlowTabAttr.OrgNo, null, "OrgNo", false, false, 0, 50, 10);
                map.AddTBInt(FlowTabAttr.Idx, 0, "Idx", true, true);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
            }
            else
            {
                this.OrgNo = BP.Web.WebUser.OrgNo;
            }
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 抄送s
    /// </summary>
    public class FlowTabs : EntitiesMyPK
    {
        #region 获得方法.
        public string Default_Mover(string flowNo, string myks)
        {
            string[] ens = myks.Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                var enNo = ens[i];
                if (enNo.Contains("Default") == true)
                    continue;

                string sql = "UPDATE WF_FlowTab SET Idx=" + (i + 1) + " WHERE MyPK='" + enNo + "' AND FK_Flow='" + flowNo + "' ";
                DBAccess.RunSQL(sql);
            }
            return "移动成功..";
        }

        /// <summary>
        /// 给主页初始化数据.
        /// </summary>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        public string Default_Init_bak(string flowNo)
        {

            string rptNo = "ND" + int.Parse(flowNo) + "Rpt";

            Flow fl = new Flow(flowNo);

            BP.WF.GERpts rpts = new BP.WF.GERpts();

        //    GEEntitys ens = new GEEntitys(rptNo);

            GenerWorkFlows ens = new GenerWorkFlows();
            BP.En.QueryObject qo = new QueryObject(ens);
            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, flowNo);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", "%" + BP.Web.WebUser.No + "%");
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.OrgNo, BP.Web.WebUser.OrgNo);
            qo.addOrderBy("RDT");
            qo.Top = 100;
            qo.DoQuery();
            return ens.ToJson();
        }

        public string Default_Init(string flowNo)
        {
            //需要判断当前流程是绑定表单库的表单还是节点表单
            if (DataType.IsNullOrEmpty(flowNo) == true)
                return "err@流程编号不能为空";

            DataSet ds = new DataSet();
            string rptNo = "ND" + int.Parse(flowNo) + "Rpt";
            GEEntitys ens = new GEEntitys(rptNo);
            BP.En.QueryObject qo = new QueryObject(ens);
            qo.AddWhere(BP.WF.GERptAttr.FlowEmps, " LIKE ", "%" + BP.Web.WebUser.No + "%");
            qo.addOr();
            qo.AddWhere(BP.WF.GERptAttr.FlowStarter, BP.Web.WebUser.No );
            qo.addOrderBy("RDT");
            qo.Top = 100;
            qo.DoQuery();
            ds.Tables.Add(ens.ToDataTableField("DT"));


           
            //表单的ID
            string frmID = "ND" + int.Parse(flowNo) + "Rpt";
            Flow flow = new Flow(flowNo);
            if (flow.FlowDevModel == FlowDevModel.RefOneFrmTree)
                frmID = flow.FrmUrl;
            if (flow.FlowDevModel == FlowDevModel.Prefessional)
            {
                //获取第一个节点的表单方案
                Node nd = new Node(int.Parse(flowNo) + "01");
                if (nd.FormType == NodeFormType.RefOneFrmTree)
                    frmID = nd.NodeFrmID;
            }

            //查询出单流程的所有字段
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.Idx);

            //默认显示的系统字段 标题、创建人、创建时间、部门、状态.
            MapAttrs mattrsOfSystem = new MapAttrs();
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.Title));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FlowStarter));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FK_Dept));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.WFState));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FlowEmps));
            ds.Tables.Add(mattrsOfSystem.ToDataTableField("Sys_MapAttrOfSystem"));

            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            return BP.Tools.Json.ToJson(ds);
        }


        /**
         * 获取通用系统字段和表单字段
         */
        public string FlowTab_Search_MapAttrs(string flowNo)
        {
            //需要判断当前流程是绑定表单库的表单还是节点表单
            if (DataType.IsNullOrEmpty(flowNo) == true)
                return "err@流程编号不能为空";
            //表单的ID
            string frmID = "ND" + int.Parse(flowNo) + "Rpt";
            Flow flow = new Flow(flowNo);
            if (flow.FlowDevModel == FlowDevModel.RefOneFrmTree)
                frmID = flow.FrmUrl;
            if(flow.FlowDevModel == FlowDevModel.Prefessional)
            {
                //获取第一个节点的表单方案
                Node nd = new Node(int.Parse(flowNo) + "01");
                if (nd.FormType == NodeFormType.RefOneFrmTree)
                    frmID = nd.NodeFrmID;
            }

            DataSet ds = new DataSet();
            //查询出单流程的所有字段
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.Idx);

            //默认显示的系统字段 标题、创建人、创建时间、部门、状态.
            MapAttrs mattrsOfSystem = new MapAttrs();
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.Title));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FlowStarter));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FK_Dept));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.WFState));
            mattrsOfSystem.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, BP.WF.GERptAttr.FlowEmps));
            ds.Tables.Add(mattrsOfSystem.ToDataTableField("Sys_MapAttrOfSystem"));

            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            return BP.Tools.Json.ToJson(ds);
        }


        public string Search_SearchData(string flowNo,int pageIdx,int pageSize)
        {
            //表单编号
            string rptNo = "ND" + int.Parse(flowNo) + "Rpt";

            //当前用户查询信息表
            UserRegedit ur = new UserRegedit(BP.Web.WebUser.No, rptNo + "_SearchAttrs");

            //表单属性
            MapData mapData = new MapData(rptNo);

            //流程表单对应的所有字段
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);

            //流程表单对应的流程数据
            GEEntitys ens = new GEEntitys(rptNo);
            QueryObject qo = new QueryObject(ens);
            qo.addLeftBracket();
            qo.AddWhere(BP.WF.GERptAttr.FlowStarter, BP.Web.WebUser.No);
            qo.addOr();
            qo.AddWhere(BP.WF.GERptAttr.FlowEmps, " LIKE ", "'%@" + BP.Web.WebUser.No + ",%'");
            qo.addRightBracket();

            #region 关键字查询
            string searchKey = ""; //关键字查询
            if (mapData.IsSearchKey)
                searchKey = ur.SearchKey;

            if (mapData.IsSearchKey && DataType.IsNullOrEmpty(searchKey) == false && searchKey.Length >= 1)
            {
                int i = 0;

                foreach (MapAttr myattr in attrs)
                {
                    Attr attr = myattr.HisAttr;
                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                        case FieldType.FK:
                        case FieldType.PKFK:
                            continue;
                        default:
                            break;
                    }

                    if (attr.MyDataType != DataType.AppString)
                        continue;

                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.Key == "FK_Dept")
                        continue;

                    i++;

                    if (i == 1)
                    {
                        qo.addLeftBracket();
                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }

                    qo.addOr();

                    if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                    else
                        qo.AddWhere(attr.Key, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                }

                qo.MyParas.Add("SKey", searchKey);
                qo.addRightBracket();
            }
            else if (DataType.IsNullOrEmpty(mapData.GetParaString("StringSearchKeys")) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFields = mapData.GetParaString("StringSearchKeys").Split('*');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    string[] items = str.Split(',');
                    if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                        continue;
                    field = items[0];
                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    idx++;
                    if (idx == 1)
                    {
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                        qo.MyParas.Add(field, fieldValue);
                        continue;
                    }
                    qo.addAnd();

                    if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                    else
                        qo.AddWhere(field, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                    qo.MyParas.Add(field, fieldValue);


                }
                if (idx != 0)
                    qo.addRightBracket();
            }

            #endregion 关键字查询



            #region 外键或者枚举的查询

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);
            foreach (string str in ap.HisHT.Keys)
            {
                var val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                    continue;
                qo.addAnd();
                qo.AddWhere(str, ap.GetValStrByKey(str));
            }
            #endregion 外键或者枚举的查询


           

            #region 日期处理
            if (mapData.DTSearchWay != DTSearchWay.None)
            {
                string dtKey = mapData.DTSearchKey;
                string dtFrom = ur.GetValStringByKey(UserRegeditAttr.DTFrom).Trim();
                string dtTo = ur.GetValStringByKey(UserRegeditAttr.DTTo).Trim();

                if (DataType.IsNullOrEmpty(dtFrom) == true)
                {
                    if (mapData.DTSearchWay == DTSearchWay.ByDate)
                        dtFrom = "1900-01-01";
                    else
                        dtFrom = "1900-01-01 00:00";
                }

                if (DataType.IsNullOrEmpty(dtTo) == true)
                {
                    if (mapData.DTSearchWay == DTSearchWay.ByDate)
                        dtTo = "2999-01-01";
                    else
                        dtTo = "2999-12-31 23:59";
                }

                if (mapData.DTSearchWay == DTSearchWay.ByDate)
                {

                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }

                if (mapData.DTSearchWay == DTSearchWay.ByDateTime)
                {

                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + " 00:00'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + " 23:59'";
                    qo.addRightBracket();
                }
            }
            #endregion 日期处理

            qo.AddWhere(" AND  WFState > 1 ");
            qo.AddWhere(" AND FID = 0 ");
            if (DataType.IsNullOrEmpty(ur.OrderBy) == false)
                if (ur.OrderWay.ToUpper().Equals("DESC") == true)
                    qo.addOrderByDesc(ur.OrderBy);
                else
                    qo.addOrderBy(ur.OrderBy);
            ur.SetPara("Count", qo.GetCount());
            ur.Update();
            qo.DoQuery("OID", pageSize, pageIdx);

            return BP.Tools.Json.ToJson(ens.ToDataTableField("Search_Data"));

        }



        #endregion


        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowTab();
            }
        }
        /// <summary>
        /// 抄送
        /// </summary>
        public FlowTabs() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowTab> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowTab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowTab> Tolist()
        {
            System.Collections.Generic.List<FlowTab> list = new System.Collections.Generic.List<FlowTab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowTab)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
