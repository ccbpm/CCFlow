using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Sys;
using BP.Web.Controls;
using System.Data;

namespace CCFlow.WF.Admin.Org
{
    public partial class Default : System.Web.UI.Page
    {
        #region Property
        private string[] _steps = new[]
                                     {
                                         "第1步 设置组织机构模式",
                                         "第2步 设置组织机构维护方式",
                                         "第3步 选择组织机构来源",
                                         "第4步 配置查询语句"
                                     };

        private string[] _smodels = new[] { "一个用户一个部门模式", "一个用户多个部门模式" };
        private string[] _smkinds = new[] { "由CCBPM组织结构维护", "集成我自己开发框架下的组织结构或者现在已有系统的组织结构" };
        private string[] _ssources = new[] { "使用数据源直接连接", "使用WebServces模式", "使用AD" };

        private Dictionary<string, Dictionary<string, string>> _oneones = null;
        private Dictionary<string, Dictionary<string, string>> _onemores = null;

        public string StepTitle
        {
            get
            {
                return _steps[Step - 1];
            }
        }

        public int Step
        {
            get
            {
                return int.Parse(Request.QueryString["step"] ?? "1");
            }
        }
        #endregion

        /// <summary>
        /// 组织结构模式变量
        /// </summary>
        GloVar gvarSModel = null;
        /// <summary>
        /// 组织结构维护方式变量
        /// </summary>
        GloVar gvarSMKind = null;
        /// <summary>
        /// 组织结构数据来源变量
        /// </summary>
        GloVar gvarSSource = null;
        /// <summary>
        /// 数据源变量
        /// </summary>
        GloVar gvarDBSrc = null;
        /// <summary>
        /// 查询语句是否配置完成，且有效
        /// </summary>
        //GloVar gvarDBOver = null;
        /// <summary>
        /// 关于组织结构设置的全局变量集合
        /// </summary>
        GloVars gvars = null;

        private BP.En.Map[] maps = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            GloVar gvar = new GloVar();

            gvars = gvar.GetNewEntities as GloVars;
            gvars.Retrieve(GloVarAttr.GroupKey, "IntegrationSet");

            gvarSModel = gvars.GetEntityByKey("StructureModel") as GloVar;
            gvarSMKind = gvars.GetEntityByKey("StructureMngKind") as GloVar;
            gvarSSource = gvars.GetEntityByKey("StructureSource") as GloVar;

            GenerateCheckedInfo();

            switch (Step)
            {
                case 1:

                    #region 设置组织结构模式

                    Pub1.Add("选择组织结构模式：");

                    if (gvarSModel == null)
                    {
                        gvarSModel = new GloVar();
                        gvarSModel.No = "StructureModel";
                        gvarSModel.Name = "组织结构模式";
                        gvarSModel.Val = "1";
                        gvarSModel.GroupKey = "IntegrationSet";
                        gvarSModel.Note = GetJoinStringFromArray(_smodels);
                        gvarSModel.Insert();
                    }

                    BPRadioButtonList rads = new BPRadioButtonList();
                    rads.ID = "Rads_StructureModel";
                    AddCheckItems(rads, _smodels);
                    rads.RepeatDirection = RepeatDirection.Horizontal;
                    rads.RepeatLayout = RepeatLayout.Flow;
                    rads.SetSelectItem(gvarSModel.Val);
                    Pub1.Add(rads);
                    Pub1.AddBR();
                    Pub1.AddBR();

                    AddButton(NamesOfBtn.Save, "保存并继续");
                    #endregion

                    break;
                case 2:

                    #region 设置组织结构维护方式

                    Pub1.Add("设置组织结构维护方式：");
                    
                    if (gvarSMKind == null)
                    {
                        gvarSMKind = new GloVar();
                        gvarSMKind.No = "StructureMngKind";
                        gvarSMKind.Name = "组织结构维护方式";
                        gvarSMKind.Val = "1";
                        gvarSMKind.GroupKey = "IntegrationSet";
                        gvarSMKind.Note = GetJoinStringFromArray(_smkinds);
                        gvarSMKind.Insert();
                    }

                    rads = new BPRadioButtonList();
                    rads.ID = "Rads_StructureMngKind";
                    AddCheckItems(rads, _smkinds);
                    rads.RepeatDirection = RepeatDirection.Horizontal;
                    rads.RepeatLayout = RepeatLayout.Flow;
                    rads.SetSelectItem(gvarSMKind.Val);
                    Pub1.Add(rads);
                    Pub1.AddBR();
                    Pub1.AddBR();

                    AddButton(NamesOfBtn.Save, "保存并继续");
                    Pub1.AddSpace(1);
                    AddButton(NamesOfBtn.Back, "上一步");
                    #endregion

                    break;
                case 3:

                    #region 选择组织结构来源

                    Pub1.Add("选择组织结构来源：");
                    
                    if (gvarSSource == null)
                    {
                        gvarSSource = new GloVar();
                        gvarSSource.No = "StructureSource";
                        gvarSSource.Name = "选择组织结构来源";
                        gvarSSource.Val = "1";
                        gvarSSource.GroupKey = "IntegrationSet";
                        gvarSSource.Note = GetJoinStringFromArray(_ssources);
                        gvarSSource.Insert();
                    }

                    rads = new BPRadioButtonList();
                    rads.ID = "Rads_StructureSource";
                    AddCheckItems(rads, _ssources);
                    rads.RepeatDirection = RepeatDirection.Horizontal;
                    rads.RepeatLayout = RepeatLayout.Flow;
                    rads.SetSelectItem(gvarSSource.Val);
                    Pub1.Add(rads);
                    Pub1.AddBR();
                    Pub1.AddBR();

                    AddButton(NamesOfBtn.Save, "保存并继续");
                    Pub1.AddSpace(1);
                    AddButton(NamesOfBtn.Back, "上一步");
                    #endregion

                    break;
                case 4:

                    #region 配置查询语句
                    
                    string msg = string.Empty;

                    if (gvarSModel == null)
                    {
                        msg = "“<a href='?step=1'>组织结构模式</a>”、";
                    }

                    if (gvarSMKind == null)
                    {
                        msg += "“<a href='?step=2'>组织结构维护方式</a>”、";
                    }

                    if (gvarSSource == null)
                    {
                        msg += "“<a href='?step=3'>组织结构来源</a>”";
                    }

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        Pub1.AddEasyUiPanelInfo("信息", "请先配置" + msg.TrimEnd('、') + "，然后配置此项！");
                        Pub1.AddBR();
                        AddButton(NamesOfBtn.Back, "上一步");
                        return;
                    }

                    //如果组织结构来源选择的是WebService和AD，则提示用户编写提供的通用webservice，以联接这两方载体
                    if (gvarSSource.Val == "2" || gvarSSource.Val == "3")
                    {
                        Pub1.AddEasyUiPanelInfo("信息", "您选择的组织结构数据来源是“<span style='font-weight:bold;'>" + _ssources[int.Parse(gvarSSource.Val)-1] + "</span>”，该种方式目前请自行修改位于CCFlow项目下的WebService文件：\\DataUser\\PortalInterface.asmx，此WebService用来提供组织结构的相关数据。");
                        Pub1.AddBR();
                        AddButton(NamesOfBtn.Back, "上一步");
                        return;
                    }

                    if (gvarSModel.Val == "1")
                    {
                        //一个用户一个部门
                        _oneones = new Dictionary<string, Dictionary<string, string>>();
                        maps = new[]
                                   {
                                       new BP.Port.Station().EnMap,
                                       new BP.Port.Dept().EnMap,
                                       new BP.Port.Emp().EnMap,                                       
                                       new BP.Port.EmpStation().EnMap
                                   };
                        GenerateMapAttrs(_oneones, maps);
                    }
                    else
                    {
                        //一个用户多个部门
                        _onemores = new Dictionary<string, Dictionary<string, string>>();
                        maps = new[]
                                       {
                                           new BP.GPM.StationType().EnMap,
                                           new BP.GPM.Station().EnMap,
                                           new BP.GPM.Dept().EnMap,
                                           new BP.GPM.Duty().EnMap,
                                           new BP.GPM.DeptDuty().EnMap,
                                           new BP.GPM.Emp().EnMap,
                                           new BP.GPM.DeptEmp().EnMap,
                                           new BP.GPM.DeptStation().EnMap,
                                           new BP.GPM.DeptEmpStation().EnMap
                                       };
                        GenerateMapAttrs(_onemores, maps);
                    }

                    //如果组织结构维护方式选择的是由“CCBPM组织结构维护”，则下面的配置查询语句就不需要了
                    if (gvarSMKind.Val == "1")
                    {
                        msg = "您选择的组织结构数据来源是“<span style='font-weight:bold;'>" + _ssources[int.Parse(gvarSSource.Val) - 1] + "</span>”，组织结构维护方式选择的是“<span style='font-weight:bold;'>" + _smkinds[int.Parse(gvarSMKind.Val) - 1] + "</span>”，组织结构模式是“<span style='font-weight:bold;'>" + _smodels[int.Parse(gvarSModel.Val) - 1] + "</span>”，此种模式下，在ccbpm的主库中需要维护的相关表有：<br />";

                        if (gvarSModel.Val == "1")
                        {
                            //一个用户一个部门
                            msg += "1.岗位类型[Sys_Enum]。Sys_Enum枚举表中EnumKey='StaGrade'的枚举。<br />";

                            for (int i = 0; i < maps.Length; i++)
                            {
                                msg += string.Format("{0}.{1}[{2}]。<br />", i + 2, maps[i].EnDesc, maps[i].PhysicsTable);
                            }
                        }
                        else
                        {
                            //一个用户多个部门
                            for (int i = 0; i < maps.Length; i++)
                            {
                                msg += string.Format("{0}.{1}[{2}]。<br />", i + 1, maps[i].EnDesc, maps[i].PhysicsTable);
                            }
                        }

                        msg += " <hr> 在这种运行模式下，ccbpm系统已经为您提供了一套维护组织机构的功能，您可以左边的组织机构树进行维护。";
                        Pub1.AddEasyUiPanelInfo("信息", msg);
                        Pub1.AddBR();
                        AddButton(NamesOfBtn.Back, "上一步");
                    }
                    else
                    {
                        Pub1.Add(
                            "您选择的组织结构数据来源是“<span style='font-weight:bold;'>" + _ssources[int.Parse(gvarSSource.Val) - 1] + "</span>”，组织结构维护方式选择的是“<span style='font-weight:bold;'>" + _smkinds[int.Parse(gvarSMKind.Val) - 1] + "</span>”，组织结构模式是“<span style='font-weight:bold;'>" + _smodels[int.Parse(gvarSModel.Val) - 1] + "</span>”。<br />");

                        gvarDBSrc = gvars.GetEntityByKey("StructureDBSrc") as GloVar;

                        if (gvarDBSrc == null)
                        {
                            gvarDBSrc = new GloVar();
                            gvarDBSrc.No = "StructureDBSrc";
                            gvarDBSrc.Name = "数据源编号";
                            gvarDBSrc.Val = "local";
                            gvarDBSrc.Note = "数据源是Sys_SFDBSrc表";
                            gvarDBSrc.GroupKey = "IntegrationSet";
                            gvarDBSrc.Save();
                        }

                        //gvarDBOver = gvars.GetEntityByKey("StructureDBOver") as GloVar;

                        //if (gvarDBOver == null)
                        //{
                        //    gvarDBOver = new GloVar();
                        //    gvarDBOver.No = "StructureDBOver";
                        //    gvarDBOver.Name = "数据源SQL配置是否完成";
                        //    gvarDBOver.Val = "False";
                        //    gvarDBOver.Note = "若组织结构配置的SQL已经配置完成，且有效，则为True；反之，为False";
                        //    gvarDBOver.GroupKey = "IntegrationSet";
                        //    gvarDBOver.Save();
                        //}

                        SFDBSrcs srcs = new SFDBSrcs();
                        srcs.RetrieveAll(SFDBSrcAttr.DBSrcType);

                        //根据传来的数据源编号，变更当前的数据源
                        if (!string.IsNullOrWhiteSpace(Request.QueryString["src"]) && srcs.IsExits(SFDBSrcAttr.No, Request.QueryString["src"]))
                        {
                            gvarDBSrc.Val = Request.QueryString["src"];
                            gvarDBSrc.Update();
                        }

                        DDL ddl = new DDL();
                        ddl.CssClass = "easyui-combobox";
                        ddl.Attributes["data-options"] = "onSelect:function(rcd){location.href='Integration.aspx?step=4&src=' + rcd.value;}";
                        ddl.ID = "DDL_DBSrcs";
                        ddl.AutoPostBack = true;
                        ddl.BindEntities(srcs);
                        ddl.SetSelectItem(gvarDBSrc.Val);

                        Pub1.AddBR();
                        Pub1.Add("选择数据源：");
                        Pub1.Add(ddl);
                        Pub1.AddSpace(1);
                        Pub1.Add("<a href=\"javascript: OpenEasyUiDialog('../../Comm/Sys/SFDBSrcNewGuide.aspx', 'eudlgframe', '新建数据源', 760, 470, 'icon-add', true, function () {location.href = location.href;})\" class='easyui-linkbutton' data-optinos=\"iconCls:'icon-add'\">新建数据源</a>");
                        Pub1.AddBR();
                        Pub1.AddBR();

                        //if (gvarDBSrc.Val == "local")
                        //{
                        //    Pub1.AddEasyUiPanelInfo("信息", string.Format("“<span style='font-weight:bold;'>{0}</span>”是CCFlow主数据源，不需要进行组织结构的SQL配置。", ddl.SelectedItem.Text));
                        //    Pub1.AddBR();
                        //    AddButton(NamesOfBtn.Back, "上一步");
                        //    return;
                        //}

                        Pub1.Add("选择此数据源，需要配置的相关表的SQL查询语句如下：");
                        Pub1.AddBR();
                        Pub1.AddBR();

                        if (gvarSModel.Val == "1")
                        {
                            #region //一个用户一个部门
                            Pub1.Add("1.<span style='font-weight:bold;'>岗位类型数据</span>。<br />");
                            LinkBtn btn = new LinkBtn(false, NamesOfBtn.Edit, "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;编辑数据");
                            btn.PostBackUrl = "javascript:void(0)";
                            btn.OnClientClick = "OpenEasyUiDialog('../../Comm/Sys/EnumList.aspx?RefNo=StaGrade&t=' + Math.random(), 'euiframe','编辑岗位类型',600,374,'icon-edit')";
                            Pub1.Add(btn);
                            Pub1.AddBR();
                            Pub1.AddBR();
                            #endregion
                        }

                        int idx = gvarSModel.Val == "1" ? 2 : 1;
                        TB tb = null;
                        foreach (BP.En.Map map in maps)
                        {
                            gvar = CheckGloVar(gvars, map.PhysicsTable, map.EnDesc);
                            Pub1.Add(GenerateAttrDescs(map, idx++));
                            tb = new TB();
                            tb.ID = "TB_" + map.PhysicsTable;
                            tb.TextMode = TextBoxMode.MultiLine;
                            tb.Text = (gvar.Val ?? string.Empty).Replace('~', '\'');
                            tb.Wrap = true;
                            tb.Width = 760;
                            tb.Height = 60;
                            Pub1.Add(tb);
                            Pub1.AddBR();
                            AddButton(NamesOfBtn.DataCheck + "_" + map.PhysicsTable, "检查正确性");
                            Pub1.AddSpace(1);
                            AddButton(NamesOfBtn.Open + "_" + map.PhysicsTable, "打开数据源");
                            Pub1.AddBR();
                            Pub1.AddBR();
                        }

                        AddButton(NamesOfBtn.Setting, "设置全部");
                        Pub1.AddSpace(1);
                        AddButton(NamesOfBtn.Back, "上一步");
                        Pub1.AddBR();
                        Pub1.AddBR();
                    }
                    #endregion

                    break;
            }
        }

        /// <summary>
        /// 拼接字符数组中的所有项为一个长字符串
        /// </summary>
        /// <param name="arr">字符数组</param>
        /// <returns></returns>
        private string GetJoinStringFromArray(string[] arr)
        {
            string str = string.Empty;

            for (var i = 0; i < arr.Length; i++)
            {
                str += (i + 1) + "." + arr[i] + (i == arr.Length - 1 ? ";" : ".");
            }

            return str;
        }

        /// <summary>
        /// 通过一个字符数组增加BPRadioButtonList中的各选择项
        /// </summary>
        /// <param name="rad">BPRadioButtonList</param>
        /// <param name="items">字符数组</param>
        private void AddCheckItems(BPRadioButtonList rad, string[] items)
        {
            for (var i = 1; i <= items.Length; i++)
            {
                rad.Items.Add(new ListItem(i + "." + items[i - 1], i.ToString()));
            }
        }

        /// <summary>
        /// 生成组织结构信息
        /// </summary>
        private void GenerateCheckedInfo()
        {
            Pub3.AddUL();
            Pub3.AddLi("组织结构模式：<span style='font-weight:bold;'>" + (gvarSModel == null ? "未选" : _smodels[int.Parse(gvarSModel.Val) - 1]) + "</span>");
            Pub3.AddLi("组织结构维护方式：<span style='font-weight:bold;'>" + (gvarSMKind == null ? "未选" : _smkinds[int.Parse(gvarSMKind.Val) - 1]) + "</span>");
            Pub3.AddLi("组织结构来源：<span style='font-weight:bold;'>" + (gvarSSource == null ? "未选" : _ssources[int.Parse(gvarSSource.Val) - 1]) + "</span>");
            Pub3.AddULEnd();
        }

        /// <summary>
        /// 检查SQL语句配置的正确性
        /// </summary>
        /// <param name="src">数据源对象</param>
        /// <param name="sModel">数据结构模式，“1”=oneone模式，“2”=onemore模式</param>
        /// <param name="table">检查所依据的表名</param>
        /// <param name="sql">组建表的SQL语句</param>
        /// <returns></returns>
        private string CheckSQL(SFDBSrc src, string sModel, string table, string sql)
        {
            DataTable dt = null;
            string error = string.Empty;

            //检查连接及SQL语法正确性
            try
            {
                dt = src.RunSQLReturnTable(sql, 0, 1);
            }
            catch (Exception ex)
            {
                error += ex.Message;
                return error;
            }

            //检查返回的列及各列类型是否正确
            Dictionary<string, string> coldefs = sModel == "1" ? _oneones[table] : _onemores[table];
            DataColumn col = null;
            string errorFields = string.Empty;
            int idx = 0;

            foreach (KeyValuePair<string, string> coldef in coldefs)
            {
                error = string.Empty;

                if (!dt.Columns.Contains(coldef.Key))
                {
                    error += (++idx) + ". 不包含" + coldef.Key + "列，";
                }
                else
                {
                    col = dt.Columns[coldef.Key];
                    if (!Equals(col.DataType.Name, coldef.Value))
                    {
                        error += (error.EndsWith("，") ? "" : (++idx) + ". ") + coldef.Key + "列类型应为" + coldef.Value + "，现在是" + col.DataType.Name + "<br />";
                    }
                    else if (error.EndsWith("，"))
                    {
                        error = error.TrimEnd('，') + "<br />";
                    }
                }

                errorFields += error;
            }

            return errorFields;
        }

        /// <summary>
        /// 根据EnMap生成属性信息字符串
        /// </summary>
        /// <param name="map">实体EnMap</param>
        /// <param name="idx">字符串前缀序号</param>
        /// <returns></returns>
        private string GenerateAttrDescs(BP.En.Map map, int idx)
        {
            string descs = string.Format("{0}.<span style='font-weight:bold;'>{1}SQL</span>[列：", idx, map.EnDesc);

            foreach (BP.En.Attr attr in map.Attrs)
            {
                if (attr.IsRefAttr) continue;

                descs += string.Format("{0}({1}，{2})、", attr.Key, attr.Desc, GetDataType(attr.MyDataType));
            }

            descs = descs.TrimEnd('、') + "]。<br />";

            return descs;
        }

        /// <summary>
        /// 根据EnMaps生成各表列信息
        /// </summary>
        /// <param name="mattrs">存储各表外信息的字典</param>
        /// <param name="maps">EnMaps</param>
        private void GenerateMapAttrs(Dictionary<string, Dictionary<string, string>> mattrs, BP.En.Map[] maps)
        {
            KeyValuePair<string, Dictionary<string, string>> kv;

            foreach (BP.En.Map map in maps)
            {
                kv = new KeyValuePair<string, Dictionary<string, string>>(map.PhysicsTable,
                                                                          new Dictionary<string, string>());

                foreach (BP.En.Attr attr in map.Attrs)
                {
                    if (attr.IsRefAttr) continue;

                    kv.Value.Add(attr.Key, GetDataType(attr.MyDataType));
                }

                mattrs.Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// 根据Attr.MyDataType获取对应的C#数据类型名称
        /// </summary>
        /// <param name="myDataType">Attr.MyDataType</param>
        /// <returns></returns>
        private string GetDataType(int myDataType)
        {
            switch (myDataType)
            {
                case DataType.AppBoolean:
                    return "Int32";
                case DataType.AppDate:
                    return "String";
                case DataType.AppDateTime:
                    return "String";
                case DataType.AppDouble:
                    return "Double";
                case DataType.AppFloat:
                    return "Single";
                case DataType.AppInt:
                    return "Int32";
                case DataType.AppMoney:
                    return "Single";
                case DataType.AppString:
                    return "String";
                default:
                    throw new Exception("@没有此类型");
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            LinkBtn btn = sender as LinkBtn;

            if (btn.ID.StartsWith(NamesOfBtn.DataCheck + "_"))
            {
                #region //检查正确性
                string table = btn.ID.Substring((NamesOfBtn.DataCheck + "_").Length);
                string sql = Pub1.GetTBByID("TB_" + table).Text;
                string srcno = Pub1.GetDDLByID("DDL_DBSrcs").SelectedItemStringVal;
                SFDBSrc src = new SFDBSrc(srcno);
                string error = CheckSQL(src, gvarSModel.Val, table, sql);

                if (string.IsNullOrWhiteSpace(error))
                {
                    //如果配置正确，则把此次的SQL语句存储上
                    GloVar gvar = gvars.GetEntityByKey(table + "_Temp") as GloVar;
                    gvar.Val = sql;
                    gvar.Update();

                    Alert("配置正确！");
                }
                else
                {
                    Alert("成功获取数据，但有如下错误：<br />" + error.TrimEnd('，'), "error");
                }
                #endregion
            }
            else if (btn.ID.StartsWith(NamesOfBtn.Open + "_"))
            {
                #region //打开数据源
                string item = btn.ID.Substring((NamesOfBtn.Open + "_").Length);
                string value = Pub1.GetTBByID("TB_" + item).Text;
                string srcno = Pub1.GetDDLByID("DDL_DBSrcs").SelectedItemStringVal;
                SFDBSrc src = new SFDBSrc(srcno);
                DataTable dt = null;

                try
                {
                    dt = src.RunSQLReturnTable(value, 0, 100);
                }
                catch (Exception ex)
                {
                    Alert(ex.Message, "error");
                    return;
                }

                Dictionary<string, string> coldefs = gvarSModel.Val == "1" ? _oneones[item] : _onemores[item];

                Pub2.Add("<table class='easyui-datagrid' data-options='fit:true'>");

                Pub2.Add("  <thead>");
                Pub2.Add("      <tr>");

                foreach (KeyValuePair<string, string> coldef in coldefs)
                {
                    Pub2.Add(string.Format("          <th data-options=\"field:'{0}'\">{0}</th>", coldef.Key));
                }

                Pub2.Add("      </tr>");
                Pub2.Add("  </thead>");

                Pub2.Add("  <tbody>");

                foreach (DataRow row in dt.Rows)
                {
                    Pub2.Add("      <tr>");

                    foreach (KeyValuePair<string, string> coldef in coldefs)
                    {
                        if (dt.Columns.Contains(coldef.Key))
                        {
                            Pub2.Add(string.Format("          <td>{0}</td>",
                                                   row[coldef.Key] == null || row[coldef.Key] == DBNull.Value
                                                       ? ""
                                                       : row[coldef.Key]));
                        }
                        else
                        {
                            Pub2.Add("          <td></td>");
                        }
                    }

                    Pub2.Add("      </tr>");
                }

                Pub2.Add("  </tbody>");

                Pub2.Add("</table>");
                ClientScript.RegisterClientScriptBlock(this.GetType(), "showwindow", "$(function(){$('#datawin').window('open');});",
                                                       true);
                #endregion
            }
            else
            {
                switch (btn.ID)
                {
                    case NamesOfBtn.Save:
                        #region //保存并继续
                        switch (Step)
                        {
                            case 1:
                                GloVar gvar = new GloVar("StructureModel");
                                gvar.Val = Pub1.GetRadioButtonListByID("Rads_StructureModel").SelectedValue;
                                gvar.Update();
                                Response.Redirect("Integration.aspx?step=2", true);
                                break;
                            case 2:
                                gvar = new GloVar("StructureMngKind");
                                gvar.Val = Pub1.GetRadioButtonListByID("Rads_StructureMngKind").SelectedValue;
                                gvar.Update();
                                Response.Redirect("Integration.aspx?step=3", true);
                                break;
                            case 3:
                                gvar = new GloVar("StructureSource");
                                gvar.Val = Pub1.GetRadioButtonListByID("Rads_StructureSource").SelectedValue;
                                gvar.Update();
                                Response.Redirect("Integration.aspx?step=4", true);
                                break;
                        }
                        #endregion
                        break;
                    case NamesOfBtn.Back:
                        #region //上一步
                        switch (Step)
                        {
                            case 2:
                                Response.Redirect("Integration.aspx?step=1", true);
                                break;
                            case 3:
                                Response.Redirect("Integration.aspx?step=2", true);
                                break;
                            case 4:
                                Response.Redirect("Integration.aspx?step=3", true);
                                break;
                        }
                        #endregion
                        break;
                    case NamesOfBtn.Setting:
                        #region //设置全部
                        //循环文本控件，保存所有SQL
                        Dictionary<string, Dictionary<string, string>> coldefs = gvarSModel.Val == "1" ? _oneones : _onemores;
                        SFDBSrc src = new SFDBSrc(gvarDBSrc.Val);
                        SFDBSrc srcLocal = new SFDBSrc("local");

                        TB tb = null;
                        GloVar tvar = null;
                        string sql = string.Empty;
                        string exists = string.Empty;
                        string successViews = string.Empty;
                        string errorViews = string.Empty;
                        string error = string.Empty;
                        int successIdx = 0;
                        int errorIdx = 0;
                        List<string> existsObjs = new List<string>();

                        #region checkSqls
                        Dictionary<string, string> checkSqlsOneOne = new Dictionary<string, string>
                                                                         {
                                                                             {
                                                                                 "Port_Emp,Port_Dept",
                                                                                 "SELECT * FROM {Port_Emp} t1 WHERE t1.FK_Dept NOT IN (SELECT t2.No FROM {Port_Dept} t2)"
                                                                                 },
                                                                             {
                                                                                 "Port_Station,Sys_Enum",
                                                                                 "SELECT * FROM {Port_Station} t1 WHERE t1.StaGrade NOT IN ({Sys_Enum})"
                                                                                 },
                                                                             {
                                                                                 "Port_EmpStation,Port_Emp",
                                                                                 "SELECT * FROM {Port_EmpStation} t1 WHERE t1.FK_Emp NOT IN (SELECT t2.No FROM {Port_Emp} t2)"
                                                                                 },
                                                                             {
                                                                                 "Port_EmpStation,Port_Station",
                                                                                 "SELECT * FROM {Port_EmpStation} t1 WHERE t1.FK_Station NOT IN (SELECT t2.No FROM {Port_Station} t2)"
                                                                                 },
                                                                             {
                                                                                 "Port_EmpDept,Port_Emp",
                                                                                 "SELECT * FROM {Port_EmpDept} t1 WHERE t1.FK_Emp NOT IN (SELECT t2.No FROM {Port_Emp} t2)"
                                                                                 },
                                                                             {
                                                                                 "Port_EmpDept,Port_Dept",
                                                                                 "SELECT * FROM {Port_EmpDept} t1 WHERE t1.FK_Dept NOT IN (SELECT t2.No FROM {Port_Dept} t2)"
                                                                                 }
                                                                         };
                        Dictionary<string, string> checkSqlsOneMore = new Dictionary<string, string>
                                                                          {
                                                                              {
                                                                                  "Port_Station,Port_StationType",
                                                                                  "SELECT * FROM {Port_Station} ps WHERE ps.FK_StationType NOT IN (SELECT pst.No FROM {Port_StationType} pst)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptDuty,Port_Dept",
                                                                                  "SELECT * FROM {Port_DeptDuty} pdd WHERE pdd.FK_Dept NOT IN (SELECT pd.No FROM {Port_Dept} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptDuty,Port_Duty",
                                                                                  "SELECT * FROM {Port_DeptDuty} pdd WHERE pdd.FK_Duty NOT IN (SELECT pd.No FROM {Port_Duty} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptStation,Port_Dept",
                                                                                  "SELECT * FROM {Port_DeptStation} pds WHERE pds.FK_Dept NOT IN (SELECT pd.No FROM {Port_Dept} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptStation,Port_Station",
                                                                                  "SELECT * FROM {Port_DeptStation} pds WHERE pds.FK_Station NOT IN (SELECT ps.No FROM {Port_Station} ps)"
                                                                                  },
                                                                              {
                                                                                  "Port_Emp,Port_Dept",
                                                                                  "SELECT * FROM {Port_Emp} pe WHERE pe.FK_Dept NOT IN (SELECT pd.No FROM {Port_Dept} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_Emp,Port_Duty",
                                                                                  "SELECT * FROM {Port_Emp} pe WHERE pe.FK_Duty NOT IN (SELECT pd.No FROM {Port_Duty} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmpStation,Port_Dept",
                                                                                  "SELECT * FROM {Port_DeptEmpStation} pdes WHERE pdes.FK_Dept NOT IN (SELECT pd.No FROM {Port_Dept} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmpStation,Port_Station",
                                                                                  "SELECT * FROM {Port_DeptEmpStation} pdes WHERE pdes.FK_Station NOT IN (SELECT ps.No FROM {Port_Station} ps)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmpStation,Port_Emp",
                                                                                  "SELECT * FROM {Port_DeptEmpStation} pdes WHERE pdes.FK_Emp NOT IN (SELECT pe.No FROM {Port_Emp} pe)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmp,Port_Dept",
                                                                                  "SELECT * FROM {Port_DeptEmp} pde WHERE pde.FK_Dept NOT IN (SELECT pd.No FROM {Port_Dept} pd)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmp,Port_Emp",
                                                                                  "SELECT * FROM {Port_DeptEmp} pde WHERE pde.FK_Emp NOT IN (SELECT pe.No FROM {Port_Emp} pe)"
                                                                                  },
                                                                              {
                                                                                  "Port_DeptEmp,Port_Duty",
                                                                                  "SELECT * FROM {Port_DeptEmp} pde WHERE pde.FK_Duty NOT IN (SELECT pd.No FROM {Port_Duty} pd)"
                                                                                  }
                                                                          };
                        #endregion

                        Dictionary<string, string> checkSqls = gvarSModel.Val == "1" ? checkSqlsOneOne : checkSqlsOneMore;

                        try
                        {
                            #region //保存配置，并初步检查各数据表返回数据的有效性
                            foreach (KeyValuePair<string, Dictionary<string, string>> def in coldefs)
                            {
                                tb = Pub1.GetTBByID("TB_" + def.Key);
                                tvar = gvars.GetEntityByKey(def.Key + "_Temp") as GloVar;
                                tvar.Val = tb.Text;
                                tvar.Update();

                                error = CheckSQL(src, gvarSModel.Val, def.Key, tvar.Val);

                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    successViews += (++successIdx) + ". " + def.Key + "<br />";
                                }
                                else
                                {
                                    errorViews += (++errorIdx) + ". " + def.Key + "<br />&nbsp;&nbsp;&nbsp;错误信息：" + error + "<br />";
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(errorViews))
                            {
                                Alert("配置SQL检查结果：<br />配置正确的表有：<br />"
                                      + successViews
                                      + "<br />配置错误的表有：<br />"
                                      + errorViews
                                      + "<br />请检查后重新设置！", "error");
                                return;
                            }
                            #endregion

                            #region //检查admin，如果没有admin则允许创建视图；如果admin不属于顶级部门，则也不允许创建视图
                            sql = "SELECT * FROM {Port_Emp} pe WHERE pe.No = 'admin'".Replace("{Port_Emp}",
                                                      "(" + (gvars.GetEntityByKey("Port_Emp_Temp") as GloVar).Val + ")");

                            DataTable dt = src.RunSQLReturnTable(sql);
                            if (dt.Rows.Count == 0)
                            {
                                Alert("配置的Port_Emp中，必须含有No='admin'的超级管理员数据！", "error");
                                return;
                            }

                            if (dt.Rows[0]["FK_Dept"] == null || dt.Rows[0]["FK_Dept"] == DBNull.Value || string.IsNullOrWhiteSpace(dt.Rows[0]["FK_Dept"].ToString()))
                            {
                                Alert("配置的Port_Emp中，No='admin'的超级管理员数据中，FK_Dept部门字段必须有值！", "error");
                                return;
                            }

                            sql = "SELECT pd.ParentNo FROM {Port_Dept} pd WHERE pd.No = '" + dt.Rows[0]["FK_Dept"] + "'";
                            dt = src.RunSQLReturnTable(sql.Replace("{Port_Dept}",
                                                      "(" + (gvars.GetEntityByKey("Port_Dept_Temp") as GloVar).Val + ")"));

                            if (dt.Rows.Count == 0)
                            {
                                Alert("配置的Port_Emp中，No='admin'的超级管理员数据中，FK_Dept部门不存在！", "error");
                                return;
                            }

                            if (dt.Rows[0][0] != null && dt.Rows[0][0] != DBNull.Value)
                            {
                                sql = "SELECT * FROM {Port_Dept} pd WHERE pd.No = '" + dt.Rows[0][0] + "'";
                                dt = src.RunSQLReturnTable(sql.Replace("{Port_Dept}",
                                                      "(" + (gvars.GetEntityByKey("Port_Dept_Temp") as GloVar).Val + ")"));

                                if (dt.Rows.Count > 0)
                                {
                                    Alert("配置的Port_Emp中，No='admin'的超级管理员数据中，FK_Dept部门必须是顶级部门！", "error");
                                    return;
                                }
                            }
                            #endregion

                            #region //根据SQL,检查数据完整性，有错误数据，给出提示
                            string[] tables = null;
                            foreach (KeyValuePair<string, string> cs in checkSqls)
                            {
                                tables = cs.Key.Split(',');
                                sql = cs.Value;

                                foreach (string table in tables)
                                {
                                    if (!sql.Contains("{" + table + "}")) continue;

                                    if (table == "Sys_Enum")
                                    {
                                        SysEnums enums = new SysEnums("StaGrade");
                                        string grades = string.Empty;

                                        foreach (SysEnum en in enums)
                                        {
                                            grades += en.IntKey + ",";
                                        }

                                        sql = sql.Replace("{" + table + "}", grades.TrimEnd(','));
                                    }
                                    else
                                    {
                                        sql = sql.Replace("{" + table + "}",
                                                      "(" + (gvars.GetEntityByKey(table + "_Temp") as GloVar).Val + ")");
                                    }
                                }

                                dt = src.RunSQLReturnTable(sql);

                                if (dt.Rows.Count > 0)
                                {
                                    errorViews += (++errorIdx) + ". " + sql + "<br />";
                                }
                            }
                            #endregion

                            #region //创建两方数据库的组织结构视图

                            GloVar gvar = null;
                            foreach (KeyValuePair<string, Dictionary<string, string>> def in coldefs)
                            {
                                tvar = gvars.GetEntityByKey(def.Key + "_Temp") as GloVar;
                                gvar = gvars.GetEntityByKey(def.Key) as GloVar;
                                gvar.Val = tvar.Val;
                                gvar.Update();

                                //判断数据源上是否已经存在同名的表或视图
                                exists = src.IsExistsObj(def.Key);
                                if (!string.IsNullOrEmpty(exists))
                                {
                                    if (exists == "TABLE" || exists == "VIEW")
                                        src.Rename(exists, def.Key,
                                                   def.Key + "_Bak" + DateTime.Now.ToString("MMddHHmmss"));
                                    existsObjs.Add(def.Key);
                                }

                                //在数据源所在数据库上建立视图
                                sql = "CREATE VIEW " + def.Key + Environment.NewLine +
                                      "AS" + Environment.NewLine + tvar.Val;
                                src.RunSQL(sql);

                                //在CCFlow主库上建立与数据源库视图的联接，也建一个视图
                                //判断主数据库里是否已经存在同名的表或视图，如果有，则改名
                                exists = srcLocal.IsExistsObj(def.Key);
                                if (!string.IsNullOrEmpty(exists))
                                {
                                    if (exists == "TABLE" || exists == "VIEW")
                                        srcLocal.Rename(exists, def.Key, def.Key + "_Bak" + DateTime.Now.ToString("MMddHHmmss"));
                                }

                                sql = string.Format("CREATE VIEW {0} AS SELECT * FROM {1}", def.Key, src.GetLinkedServerObjName(def.Key));
                                BP.DA.DBAccess.RunSQL(sql);
                            }
                            #endregion

                            //todo:oneone下部门人员表SQL自动写入，待处理
                            //tvar = gvars.GetEntityByKey("StructureDBOver") as GloVar;
                            //tvar.Val = true.ToString();
                            //tvar.Update();

                            if (!string.IsNullOrWhiteSpace(errorViews))
                            {
                                Alert("创建成功，但检查到数据完整性有以下错误：<br />"
                                      + errorViews, "error");
                            }
                            else
                            {
                                Alert("创建成功！");
                            }
                        }
                        catch (Exception ex)
                        {
                            Alert(ex.Message, "error");
                        }
                        #endregion
                        break;
                }
            }
        }

        /// <summary>
        /// 获取或创建全局变量
        /// <remarks>如果已经存在要检查的全局变量，则获取此变量；否则创建此变量，并获取</remarks>
        /// </summary>
        /// <param name="vars">全局变量集合</param>
        /// <param name="varno">要检查的全局变量编号</param>
        /// <param name="varname">要检查的全局变量名称</param>
        /// <returns></returns>
        private GloVar CheckGloVar(GloVars vars, string varno, string varname)
        {
            //正式的变量
            GloVar gvar = vars.GetEntityByKey(varno) as GloVar;

            if (gvar == null)
            {
                gvar = new GloVar();
                gvar.No = varno;
                gvar.Name = varname;
                gvar.GroupKey = "IntegrationSet";
                gvar.Val = "";
                gvar.Save();
            }

            //临时的变量
            gvar = vars.GetEntityByKey(varno + "_Temp") as GloVar;

            if (gvar == null)
            {
                gvar = new GloVar();
                gvar.No = varno + "_Temp";
                gvar.Name = varname;
                gvar.GroupKey = "IntegrationSet";
                gvar.Val = "";
                gvar.Save();
            }

            return gvar;
        }

        /// <summary>
        /// Pub1中添加LinkBtn按钮
        /// </summary>
        /// <param name="btnId">按钮的id</param>
        /// <param name="btnText">按钮的文字</param>
        private void AddButton(string btnId, string btnText)
        {
            LinkBtn btn = new LinkBtn(false, btnId, "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + btnText);
            btn.Click += new EventHandler(btn_Click);
            Pub1.Add(btn);
        }

        /// <summary>
        /// 前端弹出easyui-messager
        /// </summary>
        /// <param name="msg">包含的信息</param>
        /// <param name="type">图标类型，info/error/warning/question</param>
        private void Alert(string msg, string type = "info")
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "msginfo", "$.messager.alert('提示', '" + msg.Replace("\'", "\\\'") + "','" + type + "');", true);
        }
    }
}