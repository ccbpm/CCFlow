using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Sys;
using BP.Web;
using System.Data;
using BP.Web.Controls;
using BP.WF;

namespace CCFlow.WF.Admin
{
    /// <summary>
    /// 手机表单中的字段排序功能，added by liuxc,2016-02-26
    /// </summary>
    public partial class SortingMapAttrs : System.Web.UI.Page
    {
        #region 排序按钮控件ID拆解对象
        private class MoveBtnIds
        {
            /// <summary>
            /// 排序按钮控件ID拆解对象
            /// </summary>
            /// <param name="btnId">排序按钮控件ID，由8部分组成，每部分由_连接，如：Btn_Group_Up_4_1_2_1_186
            /// <para>各部分含义：</para>
            /// <para>1.控件命名前缀，此处为Btn</para>
            /// <para>2.移动对象类型，Group[字段分组]或Attr[字段]</para>
            /// <para>3.移动方向，Up[上移]或Down[下移]</para>
            /// <para>4.所有参与排序的记录总数</para>
            /// <para>5.当前记录所处索引号，从1开始</para>
            /// <para>6.当前记录原记录的索引号，从1开始</para>
            /// <para>7.如果是字段移动，此处为该字段所处分组的索引号；分组移动时，此处为0</para>
            /// <para>8.当前记录的主键值</para>
            /// </param>
            public MoveBtnIds(string btnId)
            {
                ControlId = btnId;

                string[] ids = ControlId.Split('_');
                if (ids.Length < 8)
                {
                    Success = false;
                    return;
                }

                try
                {
                    CtrlPrefix = ids[0];
                    MoveDirection = ids[2];
                    ObjectType = ids[1];
                    AllCount = int.Parse(ids[3]);
                    Idx = int.Parse(ids[4]);
                    OldIdx = int.Parse(ids[5]);
                    GroupIdx = int.Parse(ids[6]);
                    Key = ControlId.Substring(CtrlPrefix.Length + ObjectType.Length + MoveDirection.Length
                                           + AllCount.ToString().Length + Idx.ToString().Length +
                                           OldIdx.ToString().Length + GroupIdx.ToString().Length + 7);
                    Success = true;
                }
                catch
                {
                    Success = false;
                }
            }

            /// <summary>
            /// 获取控件ID
            /// </summary>
            public string ControlId { get; private set; }
            /// <summary>
            /// 获取控件前缀
            /// </summary>
            public string CtrlPrefix { get; private set; }
            /// <summary>
            /// 获取排序移动类型，Group或Attr
            /// </summary>
            public string ObjectType { get; private set; }
            /// <summary>
            /// 获取移动方向，Down或Up
            /// </summary>
            public string MoveDirection { get; private set; }
            /// <summary>
            /// 获取所有参与排序的记录总数
            /// </summary>
            public int AllCount { get; private set; }
            /// <summary>
            /// 获取当前记录所处索引号，从1开始
            /// </summary>
            public int Idx { get; private set; }
            /// <summary>
            /// 获取当前记录原记录的索引号，从1开始
            /// </summary>
            public int OldIdx { get; private set; }
            /// <summary>
            /// 获取字段所处分组的索引号，字段排序时有效
            /// </summary>
            public int GroupIdx { get; private set; }
            /// <summary>
            /// 获取当前记录的主键值
            /// </summary>
            public string Key { get; private set; }
            /// <summary>
            /// 获取本对象是否转换成功
            /// </summary>
            public bool Success { get; private set; }
        }
        #endregion

        public string FK_MapData
        {
            get { return Request.QueryString["FK_MapData"]; }
        }

        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
        }

        private MapDatas mapdatas;
        private MapAttrs attrs;
        private GroupFields groups;
        private MapDtls dtls;
        private FrmAttachments athMents;
        private FrmBtns btns;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检验登录（只有admin能进行排序），与传参
            if ("admin" != WebUser.No)
            {
                BP.WF.Dev2Interface.Port_Login("admin");
                Response.Write("<h3>重新点击左侧超链接！</h3>");
                return;
            }

            if (string.IsNullOrWhiteSpace(FK_MapData))
            {
                Response.Write("<h3>FK_MapData参数错误！</h3>");
                return;
            }
            #endregion

            #region 获取数据
            mapdatas = new MapDatas();
            QueryObject qo = new QueryObject(mapdatas);
            qo.AddWhere(MapDataAttr.No, "Like", FK_MapData + "%");
            qo.addOrderBy(MapDataAttr.Idx);
            qo.DoQuery();

            attrs = new MapAttrs();
            qo = new QueryObject(attrs);
            qo.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.UIVisible, true);
            qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
            qo.DoQuery();

            btns = new FrmBtns(this.FK_MapData);
            athMents = new FrmAttachments(this.FK_MapData);
            dtls = new MapDtls(this.FK_MapData);

            groups = new GroupFields();
            qo = new QueryObject(groups);
            qo.AddWhere(GroupFieldAttr.FrmID, FK_MapData);
            qo.addOrderBy(GroupFieldAttr.Idx);
            qo.DoQuery();
            #endregion

            this.BindData();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void BindData()
        {
            MapData mapdata = mapdatas.GetEntityByKey(FK_MapData) as MapData;
            DataTable dt_Attr = attrs.ToDataTableField("dtAttrs");
            DataTable dt_Dtls = dtls.ToDataTableField("dtDtls");
            DataTable dtGroups = groups.ToDataTableField("dtGroups");
            DataTable dtNoGroupAttrs = null;
            DataRow[] rows_Attrs = null;
            LinkBtn btn = null;
            DDL ddl = null;
            CheckBox cb = null;
            int idx_Attr = 1;
            int gidx = 1;
            GroupField group = null;

            if (mapdata != null)
            {
                #region 一、面板1、 分组数据+未分组数据

             //   pub1.AddEasyUiPanelInfoBegin(mapdata.Name + "[" + mapdata.No + "]字段排序", padding: 5);
                pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

                #region 标题行常量

                pub1.AddTR();
                pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
                pub1.AddTDGroupTitle("style='width:100px;'", "字段名称");
                pub1.AddTDGroupTitle("style='width:160px;'", "中文描述");
                pub1.AddTDGroupTitle("style='width:160px;'", "字段分组");
                pub1.AddTDGroupTitle("字段排序");
                pub1.AddTDGroupTitle("是否显示");
                pub1.AddTREnd();

                #endregion

                #region A、构建数据dtNoGroupAttrs，这个放在前面
                //检索全部字段，查找出没有分组或分组信息不正确的字段，存入“无分组”集合
                dtNoGroupAttrs = dt_Attr.Clone();

                foreach (DataRow dr in dt_Attr.Rows)
                {
                    if (IsExistInDataRowArray(dtGroups.Rows, GroupFieldAttr.OID, dr[MapAttrAttr.GroupID]) == false)
                        dtNoGroupAttrs.Rows.Add(dr.ItemArray);
                }
                #endregion

                #region B、构建数据dtGroups，这个放在后面(！！涉及更新数据库)
                #region 如果没有，则创建分组（1.明细2.多附件3.按钮）
                //01、未分组明细表,自动创建一个
                foreach (MapDtl mapDtl in dtls)
                {
                    if (GetGroupID(mapDtl.No, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = mapDtl.Name;
                        group.EnName = mapDtl.FK_MapData;
                        group.CtrlType = GroupCtrlType.Dtl;
                        group.CtrlID = mapDtl.No;
                        group.Insert();

                        groups.AddEntity(group);
                    }
                }
                //02、未分组多附件自动分配一个
                foreach (FrmAttachment athMent in athMents)
                {
                    if (GetGroupID(athMent.MyPK, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = athMent.Name;
                        group.EnName = athMent.FK_MapData;
                        group.CtrlType = GroupCtrlType.Ath;
                        group.CtrlID = athMent.MyPK;
                        group.Insert();

                        athMent.GroupID = group.OID;
                        athMent.Update();

                        groups.AddEntity(group);
                    }
                }

                //03、未分组按钮自动创建一个
                foreach (FrmBtn fbtn in btns)
                {
                    if (GetGroupID(fbtn.MyPK, groups) == 0)
                    {
                        group = new GroupField();
                        group.Lab = fbtn.Text;
                        group.EnName = fbtn.FK_MapData;
                        group.CtrlType = GroupCtrlType.Btn;
                        group.CtrlID = fbtn.MyPK;
                        group.Insert();

                        fbtn.GroupID = group.OID;
                        fbtn.Update();

                        groups.AddEntity(group);
                    }
                }
                #endregion

                dtGroups = groups.ToDataTableField("dtGroups");
                #endregion

                foreach (DataRow drGrp in dtGroups.Rows)
                {
                    #region 01、当前分组标题行tr

                    pub1.AddTR();
                    pub1.AddTDBegin("colspan='6' class='GroupTitle'");

                    if (gidx > 1)
                    {
                        btn = new LinkBtn(false, "Btn_Group_Up_" + dtGroups.Rows.Count + "_" + gidx + "_" + drGrp["Idx"] + "_0_" + drGrp["OID"], "上移");
                        btn.SetDataOption("iconCls", "'icon-up'");
                        btn.Click += btn_Click;
                        pub1.Add(btn);
                    }

                    if (gidx < dtGroups.Rows.Count)
                    {
                        btn = new LinkBtn(false, "Btn_Group_Down_" + dtGroups.Rows.Count + "_" + gidx + "_" + drGrp["Idx"] + "_0_" + drGrp["OID"], "下移");
                        btn.SetDataOption("iconCls", "'icon-down'");
                        btn.Click += btn_Click;
                        pub1.Add(btn);
                    }

                    pub1.AddSpace(1);
                    pub1.Add("<a href=\"javascript:GroupField('" + this.FK_MapData + "','" + drGrp["OID"] + "')\" >分组：" + drGrp["Lab"] + "</a>");

                    pub1.AddTDEnd();
                    pub1.AddTREnd();

                    #endregion

                    #region 02、当前分组每个字段一行（循环）
                    //分组中的字段
                    rows_Attrs = dt_Attr.Select(string.Format("FK_MapData = '{0}' AND GroupID = {1}", FK_MapData, drGrp["OID"]));

                    idx_Attr = 1;

                    foreach (DataRow row in rows_Attrs)
                    {
                        #region 字段行tr

                        ddl = new DDL();
                        ddl.ID = "DDL_Group_" + drGrp[GroupFieldAttr.OID] + "_" + row[MapAttrAttr.KeyOfEn];

                        foreach (DataRow dr in dtGroups.Rows)
                        {
                            string type = dr["CtrlType"].ToString();
                            if (type != "")
                                continue;

                            string lab = dr[GroupFieldAttr.Lab].ToString();
                            string oid = dr[GroupFieldAttr.OID].ToString();

                            ListItem li = new ListItem();
                            li.Text = lab;
                            li.Value = oid;
                            ddl.Items.Add(li);
                        }

                      //  foreach (DataRow rowGroup in dtGroups.Select("CtrlType = ''"))
                         //   ddl.Items.Add(new ListItem(rowGroup[GroupFieldAttr.Lab].ToString(), rowGroup[GroupFieldAttr.OID].ToString()));

                        ddl.AutoPostBack = true;
                        ddl.SelectedIndexChanged += ddl_SelectedIndexChanged;
                        ddl.SetSelectItem((int)drGrp[GroupFieldAttr.OID]);

                        cb = new CheckBox();
                        cb.ID = "CB_" + row[MapAttrAttr.KeyOfEn];
                        cb.Checked = Convert.ToInt32(row[MapAttrAttr.IsEnableInAPP]) == 1;

                        pub1.AddTR();
                        pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                        pub1.AddTD(string.Format("<a href='javascript:ShowEditWindow(\"{0}\",\"{1}\")'>{2}</a>", row[MapAttrAttr.Name], GenerateEditUrl(row), row[MapAttrAttr.KeyOfEn]));
                        pub1.AddTD(row[MapAttrAttr.Name].ToString());
                        pub1.AddTD(ddl);
                        pub1.AddTDBegin();

                        if (idx_Attr > 1)
                        {
                            btn = new LinkBtn(false,
                                              "Btn_Attr_Up_" + rows_Attrs.Length + "_" + idx_Attr + "_" + row["Idx"] + "_" + gidx + "_" +
                                              row["KeyOfEn"], "上移");
                            btn.SetDataOption("iconCls", "'icon-up'");
                            btn.Click += btn_Click;
                            pub1.Add(btn);
                        }

                        if (idx_Attr < rows_Attrs.Length)
                        {
                            btn = new LinkBtn(false,
                                              "Btn_Attr_Down_" + rows_Attrs.Length + "_" + idx_Attr + "_" + row["Idx"] + "_" + gidx + "_" +
                                              row["KeyOfEn"], "下移");
                            btn.SetDataOption("iconCls", "'icon-down'");
                            btn.Click += btn_Click;
                            pub1.Add(btn);
                        }

                        pub1.AddTDEnd();
                        pub1.AddTD(cb);
                        pub1.AddTREnd();

                        #endregion

                        idx_Attr++;
                    }
                    #endregion

                    #region 03、当前分组多附件行（循环）
                    #region 03-1.先构建数据
                    List<FrmAttachment> groupOfAthMents = new List<FrmAttachment>();
                    foreach (FrmAttachment athMent in athMents)
                    {
                        if (athMent.IsVisable == false)
                            continue;
                        if (GetGroupID(athMent.MyPK, groups).ToString() != drGrp["OID"].ToString())
                            continue;
                        groupOfAthMents.Add(athMent);
                    }
                    #endregion
                    #region 03-2 构建行tr
                    //此分组存在多附件
                    if (groupOfAthMents.Count > 0)
                    {
                        GroupAddAthMent(groupOfAthMents);
                    }
                    #endregion
                    #endregion

                    #region 04、明细表行
                    List<MapDtl> groupOfDtls = new List<MapDtl>();
                    foreach (MapDtl mapDtl in dtls)
                    {
                        if (GetGroupID(mapDtl.No, groups).ToString() != drGrp["OID"].ToString())
                            continue;
                        groupOfDtls.Add(mapDtl);
                    }
                    //此分组存在明细表
                    if (groupOfDtls.Count > 0)
                    {
                        GroupAddDtl(groupOfDtls);
                    }
                    #endregion

                    #region 05、按钮行
                    List<FrmBtn> groupOfBtns = new List<FrmBtn>();
                    foreach (FrmBtn fbtn in btns)
                    {
                        if (GetGroupID(fbtn.MyPK, groups).ToString() != drGrp["OID"].ToString())
                            continue;
                        groupOfBtns.Add(fbtn);
                    }
                    //此分组存在按钮
                    if (groupOfBtns.Count > 0)
                    {
                        GroupAddBtn(groupOfBtns);
                    }
                    #endregion

                    #region 00、如果此分组下没有字段，则显示无字段消息
                    if (rows_Attrs.Length == 0 && groupOfAthMents.Count == 0 && groupOfDtls.Count == 0 && groupOfBtns.Count == 0)
                    {
                        #region 该分组下面没有任何字段
                        pub1.AddTR();
                        pub1.AddTDBegin("colspan='6' style='color:red'");
                        pub1.AddSpace(1);
                        pub1.Add("@该分组下面没有任何字段或控件");
                        pub1.AddTDEnd();
                        pub1.AddTREnd();
                        #endregion
                    }
                    #endregion

                    gidx++;
                }

                //如果含有未分组字段，则显示在下方
                if (dtNoGroupAttrs.Rows.Count > 0)
                {
                    #region 分组行
                    pub1.AddTR();
                    pub1.AddTDBegin("colspan='6' class='GroupTitle'");
                    pub1.AddSpace(1);
                    pub1.Add("未分组字段");
                    pub1.AddTDEnd();
                    pub1.AddTREnd();
                    #endregion

                    idx_Attr = 1;

                    foreach (DataRow row in dtNoGroupAttrs.Rows)
                    {
                        #region 字段行

                        ddl = new DDL();
                        ddl.ID = "DDL_Group_" + (row[MapAttrAttr.GroupID] ?? string.Empty) + "_" + row[MapAttrAttr.KeyOfEn];
                        ddl.Items.Add(new ListItem("请选择分组", ""));

                        foreach (DataRow rowGroup in dtGroups.Select("CtrlType=''"))
                            ddl.Items.Add(new ListItem(rowGroup[GroupFieldAttr.Lab].ToString(), rowGroup[GroupFieldAttr.OID].ToString()));

                        ddl.AutoPostBack = true;
                        ddl.SelectedIndexChanged += ddl_SelectedIndexChanged;

                        cb = new CheckBox();
                        cb.ID = "CB_" + row[MapAttrAttr.KeyOfEn];
                        cb.Checked = Convert.ToInt32(row[MapAttrAttr.IsEnableInAPP]) == 1;

                        pub1.AddTR();
                        pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                        pub1.AddTD(string.Format("<a href='javascript:ShowEditWindow(\"{0}\",\"{1}\")'>{2}</a>", row[MapAttrAttr.Name], GenerateEditUrl(row), row[MapAttrAttr.KeyOfEn]));
                        pub1.AddTD(row[MapAttrAttr.Name].ToString());
                        pub1.AddTD(ddl);
                        pub1.AddTD("&nbsp;");
                        pub1.AddTD(cb);
                        pub1.AddTREnd();

                        #endregion

                        idx_Attr++;
                    }
                }

                pub1.AddTableEnd();
              //  pub1.AddEasyUiPanelInfoEnd();
                pub1.AddBR(); 
                #endregion

                #region 二、面板2、检测是否含有明细表，与分组不对应的明细表做排序
                if (dtls.Count < 0)
                {
                    pub1.AddEasyUiPanelInfoBegin("未分组明细表排序", padding: 5);
                    pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

                    #region 标题行

                    pub1.AddTR();
                    pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
                    pub1.AddTDGroupTitle("style='width:100px;'", "明细表编号");
                    pub1.AddTDGroupTitle("style='width:160px;'", "中文名称");
                    pub1.AddTDGroupTitle("排序");
                    pub1.AddTDGroupTitle("是否显示");
                    pub1.AddTREnd();

                    #endregion

                    idx_Attr = 1;

                    foreach (MapDtl dtl in dtls)
                    {
                        #region 明细表排序

                        pub1.AddTR();
                        pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                        pub1.AddTD("<a href=\"javascript:EditDtl('" + this.FK_MapData + "','" + dtl.No + "')\" >" + dtl.No + "</a>");
                        pub1.AddTD(dtl.Name);
                        pub1.AddTDBegin();

                        if (idx_Attr > 1)
                        {
                            btn = new LinkBtn(false, "Btn_Dtl_Up_" + dtls.Count + "_" + idx_Attr + "_" + dtl.RowIdx + "_0_" + dtl.No, "上移");
                            btn.SetDataOption("iconCls", "'icon-up'");
                            btn.Click += btn_Click;
                            pub1.Add(btn);
                        }

                        if (idx_Attr < dtls.Count)
                        {
                            btn = new LinkBtn(false, "Btn_Dtl_Down_" + dtls.Count + "_" + idx_Attr + "_" + dtl.RowIdx + "_0_" + dtl.No, "下移");
                            btn.SetDataOption("iconCls", "'icon-down'");
                            btn.Click += btn_Click;
                            pub1.Add(btn);
                        }

                        pub1.AddSpace(1);
                        pub1.Add(
                            string.Format("<a href='{0}' target='_self' class='easyui-linkbutton' data-options=\"iconCls:'icon-sheet'\">字段排序</a>",
                                Request.Path + "?FK_Flow=" + (FK_Flow ?? string.Empty) + "&FK_MapData=" + dtl.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff")));


                        pub1.AddTDEnd();
                        pub1.AddTREnd();

                        #endregion

                        idx_Attr++;
                    }

                    pub1.AddTableEnd();
                    pub1.AddEasyUiPanelInfoEnd();
                    pub1.AddBR();
                }
                #endregion

                #region 三、其他。如果是明细表的字段排序，则增加“返回”按钮；否则增加“复制排序”按钮,2016-03-21

                MapDtl tdtl = new MapDtl();
                tdtl.No = FK_MapData;
                if (tdtl.RetrieveFromDBSources() == 1)
                {
                    pub1.Add(
                            string.Format(
                                "<a href='{0}' target='_self' class='easyui-linkbutton' data-options=\"iconCls:'icon-back'\">返回</a>",
                                Request.Path + "?FK_Flow=" + (FK_Flow ??
                                                              string.Empty) +
                                "&FK_MapData=" + tdtl.FK_MapData +
                                "&t=" +
                                DateTime.Now.ToString("yyyyMMddHHmmssffffff")));
                }
                else
                {
                    btn = new LinkBtn(false, "Btn_ResetAttr_Idx", "智能重置顺序");
                    btn.SetDataOption("iconCls", "'icon-reset'");
                    btn.Click += btnReSet_Click;
                    pub1.Add(btn);
                    pub1.Add("<a href='javascript:void(0)' onclick=\"Form_View('" + this.FK_MapData + "','" + this.FK_Flow + "');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-search'\">预览</a>");
                    pub1.Add("<a href='javascript:void(0)' onclick=\"$('#nodes').dialog('open');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-copy'\">复制排序</a>");
                    pub1.Add("&nbsp;<a href='javascript:void(0)' onclick=\"GroupFieldNew('" + this.FK_MapData + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-addfolder'\">新建分组</a>");
                    btn = new LinkBtn(false,"Btn_Save_InApp","保存");
                    btn.SetDataOption("iconCls","'icon-save'");
                    btn.Click += btnSaveInApp_Click;
                    pub1.Add(btn);
                    pub1.AddBR();
                    pub1.AddBR();

                    pub1.Add(
                        "<div id='nodes' class='easyui-dialog' data-options=\"title:'选择复制到节点（多选）:',closed:true,buttons:'#btns'\" style='width:280px;height:340px'>");

                    ListBox lb = new ListBox();
                    lb.Style.Add("width", "100%");
                    lb.Style.Add("Height", "100%");
                    lb.SelectionMode = ListSelectionMode.Multiple;
                    lb.BorderStyle = BorderStyle.None;
                    lb.ID = "lbNodes";

                    Nodes nodes = new Nodes();
                    nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, FK_Flow, BP.WF.Template.NodeAttr.Step);

                    if (nodes.Count == 0)
                    {
                        string nodeid = FK_MapData.Replace("ND", "");
                        string flowno = string.Empty;

                        if (nodeid.Length > 2)
                        {
                            flowno = nodeid.Substring(0, nodeid.Length - 2).PadLeft(3, '0');
                            nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, flowno, BP.WF.Template.NodeAttr.Step);
                        }
                    }

                    ListItem item = null;

                    foreach (BP.WF.Node node in nodes)
                    {
                        item = new ListItem(string.Format("({0}){1}", node.NodeID, node.Name),
                                                  node.NodeID.ToString());

                        if ("ND" + node.NodeID == FK_MapData)
                            item.Attributes.Add("disabled", "disabled");

                        lb.Items.Add(item);
                    }

                    pub1.Add(lb);
                    pub1.AddDivEnd();

                    pub1.Add("<div id='btns'>");

                    LinkBtn lbtn = new LinkBtn(false, NamesOfBtn.Copy, "复制");
                    lbtn.OnClientClick = "var v = $('#" + lb.ClientID + "').val(); if(!v) { alert('请选择将此排序复制到的节点！'); return false; } else { $('#" + hidCopyNodes.ClientID + "').val(v); return true; }";
                    lbtn.Click += new EventHandler(lbtn_Click);
                    pub1.Add(lbtn);
                    lbtn = new LinkBtn(false, NamesOfBtn.Cancel, "取消");
                    lbtn.OnClientClick = "$('#nodes').dialog('close');";
                    pub1.Add(lbtn);

                    pub1.AddDivEnd();
                }
                #endregion
            }
        }

        #region 排序复制功能

        void lbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hidCopyNodes.Value)) return;

            string[] nodeids = hidCopyNodes.Value.Split(',');
            string tmd = null;
            GroupField group = null;
            MapDatas tmapdatas = null;
            MapAttrs tattrs = null, oattrs = null, tarattrs = null;
            GroupFields tgroups = null, ogroups = null, targroups = null;
            MapDtls tdtls = null;
            MapData tmapdata = null;
            MapAttr tattr = null;
            GroupField tgrp = null;
            MapDtl tdtl = null;
            int maxGrpIdx = 0;  //当前最大分组排序号
            int maxAttrIdx = 0; //当前最大字段排序号
            int maxDtlIdx = 0;  //当前最大明细表排序号
            List<string> idxGrps = new List<string>();  //复制过的分组名称集合
            List<string> idxAttrs = new List<string>(); //复制过的字段编号集合
            List<string> idxDtls = new List<string>();  //复制过的明细表编号集合

            foreach (string nodeid in nodeids)
            {
                tmd = "ND" + nodeid;

                #region 获取数据
                tmapdatas = new MapDatas();
                QueryObject qo = new QueryObject(tmapdatas);
                qo.AddWhere(MapDataAttr.No, "Like", tmd + "%");
                qo.addOrderBy(MapDataAttr.Idx);
                qo.DoQuery();

                tattrs = new MapAttrs();
                qo = new QueryObject(tattrs);
                qo.AddWhere(MapAttrAttr.FK_MapData, tmd);
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.UIVisible, true);
                qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                qo.DoQuery();

                tgroups = new GroupFields();
                qo = new QueryObject(tgroups);
                qo.AddWhere(GroupFieldAttr.FrmID, tmd);
                qo.addOrderBy(GroupFieldAttr.Idx);
                qo.DoQuery();

                tdtls = new MapDtls();
                qo = new QueryObject(tdtls);
                qo.AddWhere(MapDtlAttr.FK_MapData, tmd);
                qo.addAnd();
                qo.AddWhere(MapDtlAttr.IsView, true);
                //qo.addOrderBy(MapDtlAttr.RowIdx);
                qo.DoQuery();
                #endregion

                #region 复制排序逻辑

                #region //分组排序复制
                foreach (GroupField grp in groups)
                {
                    //通过分组名称来确定是同一个组，同一个组在不同的节点分组编号是不一样的
                    tgrp = tgroups.GetEntityByKey(GroupFieldAttr.Lab, grp.Lab) as GroupField;
                    if (tgrp == null) continue;

                    tgrp.Idx = grp.Idx;
                    tgrp.DirectUpdate();

                    maxGrpIdx = Math.Max(grp.Idx, maxGrpIdx);
                    idxGrps.Add(grp.Lab);
                }

                foreach (GroupField grp in tgroups)
                {
                    if (idxGrps.Contains(grp.Lab))
                        continue;

                    grp.Idx = maxGrpIdx = maxGrpIdx + 1;
                    grp.DirectUpdate();
                }
                #endregion

                #region //字段排序复制
                foreach (MapAttr attr in attrs)
                {
                    //排除主键
                    if (attr.IsPK == true)
                        continue;

                    tattr = tattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                    if (tattr == null) continue;

                    group = groups.GetEntityByKey(GroupFieldAttr.OID, attr.GroupID) as GroupField;

                    //比对字段的分组是否一致，不一致则更新一致
                    if (group == null)
                    {
                        //源字段分组为空，则目标字段分组置为0
                        tattr.GroupID = 0;
                    }
                    else
                    {
                        //此处要判断目标节点中是否已经创建了这个源字段所属分组，如果没有创建，则要自动创建
                        tgrp = tgroups.GetEntityByKey(GroupFieldAttr.Lab, group.Lab) as GroupField;

                        if (tgrp == null)
                        {
                            tgrp = new GroupField();
                            tgrp.Lab = group.Lab;
                            tgrp.EnName = tmd;
                            tgrp.Idx = group.Idx;
                            tgrp.Insert();
                            tgroups.AddEntity(tgrp);

                            tattr.GroupID = tgrp.OID;
                        }
                        else
                        {
                            if (tgrp.OID != tattr.GroupID)
                                tattr.GroupID = tgrp.OID;
                        }
                    }

                    tattr.Idx = attr.Idx;
                    tattr.DirectUpdate();
                    maxAttrIdx = Math.Max(attr.Idx, maxAttrIdx);
                    idxAttrs.Add(attr.KeyOfEn);
                }

                foreach (MapAttr attr in tattrs)
                {
                    //排除主键
                    if (attr.IsPK == true)
                        continue;
                    if (idxAttrs.Contains(attr.KeyOfEn))
                        continue;

                    attr.Idx = maxAttrIdx = maxAttrIdx + 1;
                    attr.DirectUpdate();
                }
                #endregion

                #region //明细表排序复制
                string dtlIdx = string.Empty;
                GroupField tgroup = null;
                int groupidx = 0;
                int tgroupidx = 0;

                foreach (MapDtl dtl in dtls)
                {
                    dtlIdx = dtl.No.Replace(dtl.FK_MapData + "Dtl", "");
                    tdtl = tdtls.GetEntityByKey(MapDtlAttr.No, tmd + "Dtl" + dtlIdx) as MapDtl;

                    if (tdtl == null)
                        continue;

                    //判断目标明细表是否有分组，没有分组，则创建分组
                    tgroup = GetGroup(tdtl.No, tgroups);
                    tgroupidx = tgroup == null ? 0 : tgroup.Idx;
                    group = GetGroup(dtl.No, groups);
                    groupidx = group == null ? 0 : group.Idx;

                    if (tgroup == null)
                    {
                        group = new GroupField();
                        group.Lab = tdtl.Name;
                        group.EnName = tdtl.FK_MapData;
                        group.CtrlType = GroupCtrlType.Dtl;
                        group.CtrlID = tdtl.No;
                        group.Idx = groupidx;
                        group.Insert();

                        tgroupidx = groupidx;
                        tgroups.AddEntity(group);
                    }

                    #region 1.明细表排序
                    if (tgroupidx != groupidx && group != null)
                    {
                        tgroup.Idx = groupidx;
                        tgroup.DirectUpdate();

                        tgroupidx = groupidx;
                        tmapdata = tmapdatas.GetEntityByKey(MapDataAttr.No, tdtl.No) as MapData;
                        if (tmapdata != null)
                        {
                            tmapdata.Idx = tgroup.Idx;
                            tmapdata.DirectUpdate();
                        }
                    }

                    maxDtlIdx = Math.Max(tgroupidx, maxDtlIdx);
                    idxDtls.Add(dtl.No);
                    #endregion

                    #region 2.获取源节点明细表中的字段分组、字段信息
                    oattrs = new MapAttrs();
                    qo = new QueryObject(oattrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, dtl.No);
                    qo.addAnd();
                    qo.AddWhere(MapAttrAttr.UIVisible, true);
                    qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                    qo.DoQuery();

                    ogroups = new GroupFields();
                    qo = new QueryObject(ogroups);
                    qo.AddWhere(GroupFieldAttr.FrmID, dtl.No);
                    qo.addOrderBy(GroupFieldAttr.Idx);
                    qo.DoQuery();
                    #endregion

                    #region 3.获取目标节点明细表中的字段分组、字段信息
                    tarattrs = new MapAttrs();
                    qo = new QueryObject(tarattrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, tdtl.No);
                    qo.addAnd();
                    qo.AddWhere(MapAttrAttr.UIVisible, true);
                    qo.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                    qo.DoQuery();

                    targroups = new GroupFields();
                    qo = new QueryObject(targroups);
                    qo.AddWhere(GroupFieldAttr.FrmID, tdtl.No);
                    qo.addOrderBy(GroupFieldAttr.Idx);
                    qo.DoQuery();
                    #endregion

                    #region 4.明细表字段分组排序
                    maxGrpIdx = 0;
                    idxGrps = new List<string>();

                    foreach (GroupField grp in ogroups)
                    {
                        //通过分组名称来确定是同一个组，同一个组在不同的节点分组编号是不一样的
                        tgrp = targroups.GetEntityByKey(GroupFieldAttr.Lab, grp.Lab) as GroupField;
                        if (tgrp == null) continue;

                        tgrp.Idx = grp.Idx;
                        tgrp.DirectUpdate();

                        maxGrpIdx = Math.Max(grp.Idx, maxGrpIdx);
                        idxGrps.Add(grp.Lab);
                    }

                    foreach (GroupField grp in targroups)
                    {
                        if (idxGrps.Contains(grp.Lab))
                            continue;

                        grp.Idx = maxGrpIdx = maxGrpIdx + 1;
                        grp.DirectUpdate();
                    }
                    #endregion

                    #region 5.明细表字段排序
                    maxAttrIdx = 0;
                    idxAttrs = new List<string>();

                    foreach (MapAttr attr in oattrs)
                    {
                        tattr = tarattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                        if (tattr == null) continue;

                        group = ogroups.GetEntityByKey(GroupFieldAttr.OID, attr.GroupID) as GroupField;

                        //比对字段的分组是否一致，不一致则更新一致
                        if (group == null)
                        {
                            //源字段分组为空，则目标字段分组置为0
                            tattr.GroupID = 0;
                        }
                        else
                        {
                            //此处要判断目标节点中是否已经创建了这个源字段所属分组，如果没有创建，则要自动创建
                            tgrp = targroups.GetEntityByKey(GroupFieldAttr.Lab, group.Lab) as GroupField;

                            if (tgrp == null)
                            {
                                tgrp = new GroupField();
                                tgrp.Lab = group.Lab;
                                tgrp.EnName = tdtl.No;
                                tgrp.Idx = group.Idx;
                                tgrp.Insert();
                                targroups.AddEntity(tgrp);

                                tattr.GroupID = tgrp.OID;
                            }
                            else
                            {
                                if (tgrp.OID != tattr.GroupID)
                                    tattr.GroupID = tgrp.OID;
                            }
                        }

                        tattr.Idx = attr.Idx;
                        tattr.DirectUpdate();
                        maxAttrIdx = Math.Max(attr.Idx, maxAttrIdx);
                        idxAttrs.Add(attr.KeyOfEn);
                    }

                    foreach (MapAttr attr in tarattrs)
                    {
                        if (idxAttrs.Contains(attr.KeyOfEn))
                            continue;

                        attr.Idx = maxAttrIdx = maxAttrIdx + 1;
                        attr.DirectUpdate();
                    }
                    #endregion
                }

                //确定目标节点中，源节点没有的明细表的排序
                foreach (MapDtl dtl in tdtls)
                {
                    if (idxDtls.Contains(dtl.No))
                        continue;

                    maxDtlIdx = maxDtlIdx + 1;
                    tgroup = GetGroup(dtl.No, tgroups);

                    if(tgroup == null)
                    {
                        tgroup = new GroupField();
                        tgroup.Lab = tdtl.Name;
                        tgroup.EnName = tdtl.FK_MapData;
                        tgroup.CtrlType = GroupCtrlType.Dtl;
                        tgroup.CtrlID = tdtl.No;
                        tgroup.Idx = maxDtlIdx;
                        tgroup.Insert();

                        tgroups.AddEntity(group);
                    }

                    if(tgroup.Idx != maxDtlIdx)
                    {
                        tgroup.Idx = maxDtlIdx;
                        tgroup.DirectUpdate();
                    }

                    tmapdata = tmapdatas.GetEntityByKey(MapDataAttr.No, dtl.No) as MapData;
                    if (tmapdata != null)
                    {
                        tmapdata.Idx = maxDtlIdx;
                        tmapdata.DirectUpdate();
                    }
                }
                #endregion

                #endregion
            }

            //重新加载本页
            Response.Redirect(Request.Path + "?FK_Flow=" + (FK_Flow ??
                              string.Empty) + "&FK_MapData=" + FK_MapData + "&t=" +
                              DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }
        #endregion

        #region 字段分组调整

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            MapAttr attr = null;
            string key = null;
            int newGrpId = 0;
            DataTable dt = null;
            DataRow[] rows = null;
            string[] ids = ddl.ID.Split('_');   //如：DDL_Group_102_XingMing

            if (ids.Length < 4) return;

            key = ddl.ID.Substring(ids[0].Length + ids[1].Length + ids[2].Length + 3);
            newGrpId = ddl.SelectedItemIntVal;
            //如果是明细表
            if (ids[1] == "Dtl")
            {
                //MapDtl mapDtl = dtls.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapDtlAttr.No, key) as MapDtl;
                //if (mapDtl == null) return;

                //mapDtl.GroupID = newGrpId;
                //mapDtl.DirectUpdate();
            }
            else if (ids[1] == "AthMent")
            {
                //多附件排序
                //FrmAttachment athMent = athMents.GetEntityByKey(FrmAttachmentAttr.FK_MapData, FK_MapData, FrmAttachmentAttr.NoOfObj, key) as FrmAttachment;
                //if (athMent == null) return;

                //athMent.GroupID = newGrpId;
                //athMent.DirectUpdate();
            }
            else
            {
                attr = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, key) as MapAttr;

                if (attr == null) return;

                newGrpId = ddl.SelectedItemIntVal;
                dt = attrs.ToDataTableField("dtAttrs");
                rows = dt.Select(string.Format("FK_MapData = '{0}' AND GroupID = {1}", FK_MapData, newGrpId), "Idx DESC");

                attr.GroupID = newGrpId;
                attr.Idx = (rows.Length > 0 ? (int)rows[0]["Idx"] : 0) + 1;
                attr.DirectUpdate();
            }
            //重新加载本页
            Response.Redirect(Request.Path + "?FK_Flow=" + (FK_Flow ??
                              string.Empty) + "&FK_MapData=" + FK_MapData + "&t=" +
                              DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }
        #endregion

        #region 重置表单字段
        void btnReSet_Click(object sender, EventArgs e)
        {
            attrs = new MapAttrs();
            QueryObject qo = new QueryObject(attrs);
            qo.AddWhere(MapAttrAttr.FK_MapData, FK_MapData);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.UIVisible, true);
            qo.addOrderBy(MapAttrAttr.Y, MapAttrAttr.X);
            qo.DoQuery();
            int rowIdx = 0;
            foreach (MapAttr mapAttr in attrs)
            {
                mapAttr.Idx = rowIdx;
                mapAttr.DirectUpdate();
                rowIdx++;
            }
            //重新加载本页
            Response.Redirect(Request.Path + "?FK_Flow=" + (FK_Flow ??
                              string.Empty) + "&FK_MapData=" + FK_MapData + "&t=" +
                              DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }
        #endregion
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSaveInApp_Click(object sender, EventArgs e)
        {
            MapAttr att = null;
            foreach (Control ctrl in pub1.Controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox cb = ctrl as CheckBox;
                    bool val=cb.Checked;
                    string keyOfEn = cb.ID.Replace("CB_","");

                     att = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, keyOfEn) as MapAttr;
                     att.IsEnableInAPP = val;
                     att.Update();
                }
            }
        }

        #region 字段排序

        void btn_Click(object sender, EventArgs e)
        {
            LinkBtn btn = sender as LinkBtn;
            MoveBtnIds ids = new MoveBtnIds(btn.ID);
            MoveBtnIds tids = null;
            LinkBtn tbtn = null;
            GroupField group = null;
            MapAttr attr = null;
            MapData mapdata = null;
            MapDtl dtl = null;
            FrmAttachment athMent = null;
            FrmBtn fbtn = null;
            List<string> keys = new List<string>();
            int targetIdx;

            if (!ids.Success) return;

            targetIdx = ids.MoveDirection == "Up" ? ids.Idx - 1 : ids.Idx + 1;

            switch (ids.ObjectType)
            {
                case "Group":
                    #region 字段分组
                    //检测所有的分组，判断原有idx与现在idx是否匹配，不匹配的都进行更新，第一次进行排序时，将原有的idx都更新一遍
                    foreach (Control ctrl in pub1.Controls)
                    {
                        tbtn = ctrl as LinkBtn;
                        if (tbtn == null || !tbtn.ID.StartsWith("Btn_Group_")) continue;

                        tids = new MoveBtnIds(ctrl.ID);
                        if (!tids.Success || keys.Contains(tids.Key)) continue;

                        keys.Add(tids.Key);

                        if (tids.Idx == targetIdx)
                        {
                            //受影响的组索引号更改为当前移动组的索引号
                            if (tids.OldIdx != ids.Idx)
                            {
                                //更新GroupField中的索引
                                group = groups.GetEntityByKey(int.Parse(tids.Key)) as GroupField;
                                group.Idx = ids.Idx;
                                group.Update();
                            }
                        }
                        else if (tids.Idx == ids.Idx)
                        {
                            //当前移动组的索引号改为受影响的组索引号
                            if (tids.OldIdx != targetIdx)
                            {
                                //更新GroupField中的索引
                                group = groups.GetEntityByKey(int.Parse(tids.Key)) as GroupField;
                                group.Idx = targetIdx;
                                group.Update();
                            }
                        }
                        else
                        {
                            //检索其余未受影响的组，将与之对应的索引号不一样的均更新
                            if (tids.OldIdx != tids.Idx)
                            {
                                //更新GroupField中的索引
                                group = groups.GetEntityByKey(int.Parse(tids.Key)) as GroupField;
                                group.Idx = tids.Idx;
                                group.Update();
                            }
                        }
                    }
                    #endregion
                    break;
                case "Attr":
                    #region 字段
                    //检测所有的字段，判断原有idx与现在idx是否匹配，不匹配的都进行更新，第一次进行排序时，将原有的idx都更新一遍
                    foreach (Control ctrl in pub1.Controls)
                    {
                        tbtn = ctrl as LinkBtn;
                        if (tbtn == null || !tbtn.ID.StartsWith("Btn_Attr_")) continue;

                        tids = new MoveBtnIds(ctrl.ID);
                        if (!tids.Success || keys.Contains(tids.Key)) continue;

                        keys.Add(tids.Key);

                        if (tids.Idx == targetIdx && tids.GroupIdx == ids.GroupIdx)
                        {
                            //受影响的字段索引号更改为当前移动字段的索引号
                            if (tids.OldIdx != ids.Idx)
                            {
                                //更新MapAttr中的索引
                                attr = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, tids.Key) as MapAttr;
                                attr.Idx = ids.Idx;
                                attr.Update();
                            }
                        }
                        else if (tids.Idx == ids.Idx && tids.GroupIdx == ids.GroupIdx)
                        {
                            //当前移动字段的索引号改为受影响的字段索引号
                            if (tids.OldIdx != targetIdx)
                            {
                                //更新MapAttr中的索引
                                attr = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, tids.Key) as MapAttr;
                                attr.Idx = targetIdx;
                                attr.Update();
                            }
                        }
                        else
                        {
                            //检索其余未受影响的字段，将与之对应的索引号不一样的均更新
                            if (tids.OldIdx != tids.Idx)
                            {
                                //更新MapAttr中的索引
                                attr = attrs.GetEntityByKey(MapAttrAttr.FK_MapData, FK_MapData, MapAttrAttr.KeyOfEn, tids.Key) as MapAttr;
                                attr.Idx = tids.Idx;
                                attr.Update();
                            }
                        }
                    }
                    #endregion
                    break;
                case "Dtl":
                    #region 明细表
                    //检测所有的明细表，判断原有idx与现在idx是否匹配，不匹配的都进行更新，第一次进行排序时，将原有的idx都更新一遍
                    foreach (Control ctrl in pub1.Controls)
                    {
                        tbtn = ctrl as LinkBtn;
                        if (tbtn == null || !tbtn.ID.StartsWith("Btn_Dtl_")) continue;

                        tids = new MoveBtnIds(ctrl.ID);
                        if (!tids.Success || keys.Contains(tids.Key)) continue;

                        keys.Add(tids.Key);

                        if (tids.Idx == targetIdx)
                        {
                            //受影响的明细表索引号更改为当前移动明细表的索引号
                            if (tids.OldIdx != ids.Idx)
                            {
                                //更新MapDtl中的明细表索引
                                //dtl = dtls.GetEntityByKey(tids.Key) as MapDtl;
                                //dtl.RowIdx = ids.Idx;
                                //dtl.Update();
                                group = GetGroup(tids.Key, groups);
                                group.Idx = ids.Idx;
                                group.Update();

                                //更新MapData中的索引
                                mapdata = mapdatas.GetEntityByKey(dtl.No) as MapData;
                                mapdata.Idx = ids.Idx;
                                mapdata.Update();
                            }
                        }
                        else if (tids.Idx == ids.Idx)
                        {
                            //当前移动明细表的索引号改为受影响的明细表索引号
                            if (tids.OldIdx != targetIdx)
                            {
                                //更新MapDtl中的明细表索引
                                //dtl = dtls.GetEntityByKey(tids.Key) as MapDtl;
                                //dtl.RowIdx = targetIdx;
                                //dtl.Update();
                                group = GetGroup(tids.Key, groups);
                                group.Idx = targetIdx;
                                group.Update();

                                //更新MapData中的索引
                                mapdata = mapdatas.GetEntityByKey(dtl.No) as MapData;
                                mapdata.Idx = targetIdx;
                                mapdata.Update();
                            }
                        }
                        else
                        {
                            //检索其余未受影响的明细表，将与之对应的索引号不一样的均更新
                            if (tids.OldIdx != tids.Idx)
                            {
                                //更新MapDtl中的明细表索引
                                //dtl = dtls.GetEntityByKey(tids.Key) as MapDtl;
                                //dtl.RowIdx = tids.Idx;
                                //dtl.Update();
                                group = GetGroup(tids.Key, groups);
                                group.Idx = tids.Idx;
                                group.Update();

                                //更新MapData中的索引
                                mapdata = mapdatas.GetEntityByKey(dtl.No) as MapData;
                                mapdata.Idx = tids.Idx;
                                mapdata.Update();
                            }
                        }
                    }
                    #endregion
                    break;
                case "AthMent":
                    #region 多附件
                    //检测所有的多附件，判断原有idx与现在idx是否匹配，不匹配的都进行更新，第一次进行排序时，将原有的idx都更新一遍
                    foreach (Control ctrl in pub1.Controls)
                    {
                        tbtn = ctrl as LinkBtn;
                        if (tbtn == null || !tbtn.ID.StartsWith("Btn_AthMent")) continue;

                        tids = new MoveBtnIds(ctrl.ID);
                        if (!tids.Success || keys.Contains(tids.Key)) continue;

                        keys.Add(tids.Key);

                        athMent = athMents.GetEntityByKey(FrmAttachmentAttr.FK_MapData, FK_MapData, FrmAttachmentAttr.NoOfObj, tids.Key) as FrmAttachment;
                        if (athMent == null)
                            continue;
                        if (tids.Idx == targetIdx)
                        {
                            //受影响的组索引号更改为当前移动组的索引号
                            if (tids.OldIdx != ids.Idx)
                            {
                                //更新FrmAttachment中的索引
                           //     athMent.RowIdx = ids.Idx;
                                athMent.Update();
                            }
                        }
                        else if (tids.Idx == ids.Idx)
                        {
                            //当前移动组的索引号改为受影响的组索引号
                            if (tids.OldIdx != targetIdx)
                            {
                                //更新FrmAttachment中的索引
                            //    athMent.RowIdx = targetIdx;
                                athMent.Update();
                            }
                        }
                        else
                        {
                            //检索其余未受影响的组，将与之对应的索引号不一样的均更新
                            if (tids.OldIdx != tids.Idx)
                            {
                                //更新FrmAttachment中的索引
                             //   athMent.RowIdx = tids.Idx;
                                athMent.Update();
                            }
                        }
                    }
                    #endregion
                    break;
                case "Btn":
                    #region 按钮
                    //检测所有的按钮，判断原有idx与现在idx是否匹配，不匹配的都进行更新，第一次进行排序时，将原有的idx都更新一遍
                    foreach (Control ctrl in pub1.Controls)
                    {
                        tbtn = ctrl as LinkBtn;
                        if (tbtn == null || !tbtn.ID.StartsWith("Btn_Btn")) continue;

                        tids = new MoveBtnIds(ctrl.ID);
                        if (!tids.Success || keys.Contains(tids.Key)) continue;

                        keys.Add(tids.Key);

                        fbtn = btns.GetEntityByKey(FrmBtnAttr.FK_MapData, FK_MapData, FrmBtnAttr.MyPK, tids.Key) as FrmBtn;
                        if (fbtn == null)
                            continue;
                        if (tids.Idx == targetIdx)
                        {
                            //受影响的组索引号更改为当前移动组的索引号
                            if (tids.OldIdx != ids.Idx)
                            {
                                //更新FrmBtn中的索引
                                //fbtn.RowIdx = ids.Idx;
                                fbtn.Update();
                            }
                        }
                        else if (tids.Idx == ids.Idx)
                        {
                            //当前移动组的索引号改为受影响的组索引号
                            if (tids.OldIdx != targetIdx)
                            {
                                //更新FrmBtn中的索引
                                //    fbtn.RowIdx = targetIdx;
                                fbtn.Update();
                            }
                        }
                        else
                        {
                            //检索其余未受影响的组，将与之对应的索引号不一样的均更新
                            if (tids.OldIdx != tids.Idx)
                            {
                                //更新FrmBtn中的索引
                                //   fbtn.RowIdx = tids.Idx;
                                fbtn.Update();
                            }
                        }
                    }
                    #endregion
                    break;
            }

            //重新加载本页
            Response.Redirect(Request.Path + "?FK_Flow=" + (FK_Flow ??
                              string.Empty) + "&FK_MapData=" + FK_MapData + "&t=" +
                              DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }
        #endregion

        /// <summary>
        /// 判断在DataRow数组中，是否存在指定列指定值的行
        /// </summary>
        /// <param name="rows">DataRow数组</param>
        /// <param name="field">指定列名</param>
        /// <param name="value">指定值</param>
        /// <returns></returns>
        private bool IsExistInDataRowArray(DataRowCollection rows, string field, object value)
        {
            foreach (DataRow row in rows)
            {
                if (Equals(row[field], value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 根据MapAttr字段行数据，获取该字段的编辑链接
        /// </summary>
        /// <param name="drAttr">MapAttr字段行数据</param>
        /// <returns></returns>
        private string GenerateEditUrl(DataRow drAttr)
        {
            string url = "../FoolFormDesigner/";

            switch ((FieldTypeS)drAttr[MapAttrAttr.LGType])
            {
                case BP.En.FieldTypeS.Enum:
                    url = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&FK_MapData=" + drAttr[MapAttrAttr.FK_MapData] + "&MyPK=" + drAttr[MapAttrAttr.MyPK] + "&FType=" + drAttr[MapAttrAttr.MyDataType] + "&GroupField=0";
                    break;
                case BP.En.FieldTypeS.Normal:
                    url += "EditFieldGuide.htm?DoType=Edit&FK_MapData=" + drAttr[MapAttrAttr.FK_MapData] + "&MyPK=" + drAttr[MapAttrAttr.MyPK] + "&FType=" + drAttr[MapAttrAttr.MyDataType] + "&GroupField=0";
                    break;
                case BP.En.FieldTypeS.FK:
                    url = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&FK_MapData=" + drAttr[MapAttrAttr.FK_MapData] + "&MyPK=" + drAttr[MapAttrAttr.MyPK] + "&FType=" +
                           drAttr[MapAttrAttr.MyDataType] + "&GroupField=0";
                    break;
                default:
                    url = "javascript:alert('未涉及的字段类型！');void(0);";
                    break;
            }

            return url;
        }

        /// <summary>
        /// 给分组添加明细表
        /// </summary>
        /// <param name="groupOfDtls"></param>
        private void GroupAddDtl(List<MapDtl> groupOfDtls)
        {
            pub1.AddTR();
            pub1.AddTDBegin("colspan='5'");
            pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

            #region 标题行
            pub1.AddTR();
            pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
            pub1.AddTDGroupTitle("style='width:100px;'", "明细表编号");
            pub1.AddTDGroupTitle("style='width:160px;'", "中文名称");
            pub1.AddTDGroupTitle("style='width:120px;'", "字段分组");
            pub1.AddTDGroupTitle("排序");
            pub1.AddTREnd();

            #endregion

            int idx_Attr = 1;
            LinkBtn btn = null;
            foreach (MapDtl dtl in groupOfDtls)
            {
                #region 明细表排序

                pub1.AddTR();
                pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                pub1.AddTD("<a href=\"javascript:EditDtl('" + this.FK_MapData + "','" + dtl.No + "')\" >" + dtl.No + "</a>");
                pub1.AddTD(dtl.Name);
                //DDL ddl = new DDL();
                //ddl.ID = "DDL_Dtl_" + dtl.GroupID + "_" + dtl.No;

                //foreach (GroupField groupField in groups)
                //    ddl.Items.Add(new ListItem(groupField.Lab, groupField.OID.ToString()));

                //ddl.AutoPostBack = true;
                //ddl.SelectedIndexChanged += ddl_SelectedIndexChanged;
                //ddl.SetSelectItem(dtl.GroupID);
                //pub1.AddTD(ddl);
                pub1.AddTD("&nbsp;");

                pub1.AddTDBegin();

                if (idx_Attr > 1)
                {
                    btn = new LinkBtn(false, "Btn_Dtl_Up_" + dtls.Count + "_" + idx_Attr + "_" + dtl.RowIdx + "_0_" + dtl.No, "上移");
                    btn.SetDataOption("iconCls", "'icon-up'");
                    btn.Click += btn_Click;
                    pub1.Add(btn);
                }

                if (idx_Attr < groupOfDtls.Count)
                {
                    btn = new LinkBtn(false, "Btn_Dtl_Down_" + dtls.Count + "_" + idx_Attr + "_" + dtl.RowIdx + "_0_" + dtl.No, "下移");
                    btn.SetDataOption("iconCls", "'icon-down'");
                    btn.Click += btn_Click;
                    pub1.Add(btn);
                }

                pub1.AddSpace(1);
                pub1.Add(
                    string.Format("<a href='{0}' target='_self' class='easyui-linkbutton' data-options=\"iconCls:'icon-sheet'\">字段排序</a>",
                        Request.Path + "?FK_Flow=" + (FK_Flow ?? string.Empty) + "&FK_MapData=" + dtl.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff")));

                pub1.AddTDEnd();
                pub1.AddTREnd();

                #endregion

                idx_Attr++;
            }
            pub1.AddTableEnd();

            pub1.AddBR();
            pub1.AddTDEnd();
            pub1.AddTREnd();
        }

        /// <summary>
        /// 给分组添加多附件
        /// </summary>
        /// <param name="groupOfDtls"></param>
        private void GroupAddAthMent(List<FrmAttachment> groupOfAthMents)
        {
            pub1.AddTR();
            pub1.AddTDBegin("colspan='5'");
            pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

            #region 标题行
            pub1.AddTR();
            pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
            pub1.AddTDGroupTitle("style='width:100px;'", "多附件编号");
            pub1.AddTDGroupTitle("style='width:160px;'", "中文名称");
            pub1.AddTDGroupTitle("style='width:120px;'", "字段分组");
            pub1.AddTDGroupTitle("排序");
            pub1.AddTREnd();

            #endregion

            int idx_Attr = 1;
            LinkBtn btn = null;
            foreach (FrmAttachment athMent in groupOfAthMents)
            {
                #region 多附件排序

                pub1.AddTR();
                pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                pub1.AddTD("<a href=\"javascript:EditAthMent('" + this.FK_MapData + "','" + athMent.NoOfObj + "')\" >" + athMent.NoOfObj + "</a>");
                pub1.AddTD(athMent.Name);
                //DDL ddl = new DDL();
                //ddl.ID = "DDL_AthMent_" + athMent.GroupID + "_" + athMent.NoOfObj;

                //foreach (GroupField groupField in groups)
                //    ddl.Items.Add(new ListItem(groupField.Lab, groupField.OID.ToString()));

                //ddl.AutoPostBack = true;
                //ddl.SelectedIndexChanged += ddl_SelectedIndexChanged;
                //ddl.SetSelectItem(athMent.GroupID);
                //pub1.AddTD(ddl);
                pub1.AddTD("&nbsp;");

                pub1.AddTDBegin();

                //if (idx_Attr > 1)
                //{
                //    btn = new LinkBtn(false, "Btn_AthMent_Up_" + athMents.Count + "_" + idx_Attr + "_" + athMent.RowIdx + "_0_" + athMent.NoOfObj, "上移");
                //    btn.SetDataOption("iconCls", "'icon-up'");
                //    btn.Click += btn_Click;
                //    pub1.Add(btn);
                //}
                //if (idx_Attr < groupOfAthMents.Count)
                //{
                //    btn = new LinkBtn(false, "Btn_AthMent_Down_" + athMents.Count + "_" + idx_Attr + "_" + athMent.RowIdx + "_0_" + athMent.NoOfObj, "下移");
                //    btn.SetDataOption("iconCls", "'icon-down'");
                //    btn.Click += btn_Click;
                //    pub1.Add(btn);
                //}

                pub1.AddTDEnd();
                pub1.AddTREnd();

                #endregion

                idx_Attr++;
            }
            pub1.AddTableEnd();

            pub1.AddBR();
            pub1.AddTDEnd();
            pub1.AddTREnd();
        }

        /// <summary>
        /// 给分组添加按钮
        /// </summary>
        /// <param name="groupOfBtns"></param>
        private void GroupAddBtn(List<FrmBtn> groupOfBtns)
        {
            pub1.AddTR();
            pub1.AddTDBegin("colspan='5'");
            pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

            #region 标题行
            pub1.AddTR();
            pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
            pub1.AddTDGroupTitle("style='width:100px;'", "按钮编号");
            pub1.AddTDGroupTitle("style='width:160px;'", "按钮文本");
            pub1.AddTDGroupTitle("style='width:120px;'", "按钮分组");
            pub1.AddTDGroupTitle("排序");
            pub1.AddTREnd();

            #endregion

            int idx_Attr = 1;
            LinkBtn btn = null;
            foreach (FrmBtn fbtn in groupOfBtns)
            {
                #region 多附件排序

                pub1.AddTR();
                pub1.AddTD("style='text-align:center'", idx_Attr.ToString());
                pub1.AddTD(fbtn.MyPK);
                pub1.AddTD(fbtn.Text);
                //DDL ddl = new DDL();
                //ddl.ID = "DDL_Btn_" + fbtn.GroupID + "_" + fbtn.MyPK;

                //foreach (GroupField groupField in groups)
                //    ddl.Items.Add(new ListItem(groupField.Lab, groupField.OID.ToString()));

                //ddl.AutoPostBack = true;
                //ddl.SelectedIndexChanged += ddl_SelectedIndexChanged;
                //ddl.SetSelectItem(fbtn.GroupID);
                //pub1.AddTD(ddl);
                pub1.AddTD("&nbsp;");

                pub1.AddTDBegin();

                //if (idx_Attr > 1)
                //{
                //    btn = new LinkBtn(false, "Btn_Btn_Up_" + btns.Count + "_" + idx_Attr + "_" + fbtn.RowIdx + "_0_" + fbtn.MyPK, "上移");
                //    btn.SetDataOption("iconCls", "'icon-up'");
                //    btn.Click += btn_Click;
                //    pub1.Add(btn);
                //}
                //if (idx_Attr < groupOfBtns.Count)
                //{
                //    btn = new LinkBtn(false, "Btn_Btn_Down_" + btns.Count + "_" + idx_Attr + "_" + fbtn.RowIdx + "_0_" + fbtn.MyPK, "下移");
                //    btn.SetDataOption("iconCls", "'icon-down'");
                //    btn.Click += btn_Click;
                //    pub1.Add(btn);
                //}

                pub1.AddTDEnd();
                pub1.AddTREnd();

                #endregion

                idx_Attr++;
            }
            pub1.AddTableEnd();

            pub1.AddBR();
            pub1.AddTDEnd();
            pub1.AddTREnd();
        }

        private int GetGroupID(string ctrlID, GroupFields gfs)
        {
            GroupField gf = gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
            return gf == null ? 0 : gf.OID;
        }

        private GroupField GetGroup(string ctrlID, GroupFields gfs)
        {
            return gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
        }
    }
}