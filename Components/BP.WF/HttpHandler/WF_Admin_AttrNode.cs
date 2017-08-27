using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_AttrNode : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_AttrNode(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region  单据模版维护
        public string Bill_Init()
        {
            //增加上单据模版集合.
            BillTemplates bills = new BillTemplates();
            bills.Retrieve(BillTemplateAttr.NodeID, this.FK_Node);
            return bills.ToJson();

            //DataSet ds = new DataSet();
            //DataTable dt = bills.ToDataTableField("WF_BillTemplate");
            //ds.Tables.Add(dt);
            ////传递来的变量.
            //string fk_template = this.GetRequestVal("FK_BillTemplate");
            //return BP.Tools.Json.DataSetToJson(ds);
        }
        public string Bill_Save()
        {
            BillTemplate bt = new BillTemplate();

            //上传附件
            string filepath = "";
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                filepath = HttpContext.Current.Server.MapPath("\\DataUser\\CyclostyleFile\\" + file.FileName);
                file.SaveAs(filepath);
            }

            bt.NodeID = this.FK_Node;
            bt.No = this.GetRequestVal("TB_No");
            if (string.IsNullOrEmpty(bt.No))
            {
                bt.No = DA.DBAccess.GenerOID().ToString(); 
            }
            bt.Name = this.GetRequestVal("TB_Name");

            bt.TempFilePath = filepath;
            bt.HisBillFileType = (BillFileType)this.GetRequestValInt("DDL_BillFileType");
            bt.BillOpenModel = (BillOpenModel)this.GetRequestValInt("DDL_BillOpenModel");
            bt.QRModel = (QRModel)this.GetRequestValInt("DDL_BillOpenModel");

            bt.Save();
            
            return "保存成功.";
        }
        public string Bill_Delete()
        {
            BillTemplate bt = new BillTemplate();
            bt.No = this.GetRequestVal("FK_BillTemplate");
            bt.Delete();

            return "删除成功.";
        }
        public void Bill_Download()
        {
            string no = context.Request["No"].ToString();
            string sql = "select TempFilePath from WF_BillTemplate where No = '" + no + "'";
            string MyFilePath = BP.DA.DBAccess.RunSQLReturnVal(sql).ToString();   
            HttpResponse response = context.Response;

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", MyFilePath.Substring(MyFilePath.LastIndexOf('\\') + 1)));
            response.ContentEncoding = System.Text.Encoding.UTF8;
            //response.ContentType = "application/ms-excel";
            response.BinaryWrite(System.IO.File.ReadAllBytes(MyFilePath));
            response.End();
        }
        #endregion

        #region  节点消息
        public string PushMsg_Init()
        {
            //增加上单据模版集合.
            int nodeID = this.GetRequestValInt("FK_Node");
            BP.WF.Template.PushMsgs ens = new BP.WF.Template.PushMsgs(nodeID);
            return ens.ToJson();
        }
        public string PushMsg_Save()
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = HttpContext.Current.Request.QueryString["MyPK"];
            msg.RetrieveFromDBSources();

            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);

            #region 求出来选择的节点.
            string nodesOfSMS = "";
            string nodesOfEmail = "";
            foreach (BP.WF.Node mynd in nds)
            {
                foreach (string key in HttpContext.Current.Request.Params.AllKeys)
                {
                    if (key.Contains("CB_SMS_" + mynd.NodeID)
                        && nodesOfSMS.Contains(mynd.NodeID + "") == false)
                        nodesOfSMS += mynd.NodeID + ",";

                    if (key.Contains("CB_Email_" + mynd.NodeID)
                        && nodesOfEmail.Contains(mynd.NodeID + "") == false)
                        nodesOfEmail += mynd.NodeID + ",";
                }
            }

            //节点.
            msg.MailNodes = nodesOfEmail;
            msg.SMSNodes = nodesOfSMS;
            #endregion 求出来选择的节点.

            #region 短信保存.
            //短信推送方式。
            msg.SMSPushWay = Convert.ToInt32(HttpContext.Current.Request["RB_SMS"].ToString().Replace("RB_SMS_", ""));

            //短信手机字段.
            msg.SMSField = HttpContext.Current.Request["DDL_SMS_Fields"].ToString();
            //替换变量
            string smsstr = HttpContext.Current.Request["TB_SMS"].ToString();
            //扬玉慧 此处是配置界面  不应该把用户名和用户编号转化掉
            //smsstr = smsstr.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            //smsstr = smsstr.Replace("@WebUser.No", BP.Web.WebUser.No);

            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            // smsstr = smsstr.Replace("@RDT",);
            //短信内容模版.
            msg.SMSDoc_Real = smsstr;
            #endregion 短信保存.

            #region 邮件保存.
            //邮件.
            msg.MailPushWay = Convert.ToInt32(HttpContext.Current.Request["RB_Email"].ToString().Replace("RB_Email_", "")); ;

            //邮件标题与内容.
            msg.MailTitle_Real = HttpContext.Current.Request["TB_Email_Title"].ToString();
            msg.MailDoc_Real = HttpContext.Current.Request["TB_Email_Doc"].ToString();

            //邮件地址.
            msg.MailAddress = HttpContext.Current.Request["DDL_Email_Fields"].ToString(); ;

            #endregion 邮件保存.

            //保存.
            if (string.IsNullOrEmpty(msg.MyPK) == true)
            {
                msg.MyPK = BP.DA.DBAccess.GenerGUID();
                msg.Insert();
            }
            else
            {
                msg.Update();
            }

            return "保存成功.."; 
        }
        public string PushMsg_Delete()
        {
            PushMsg pm = new PushMsg();
            pm.MyPK = this.GetRequestVal("MyPK");
            pm.Delete();

            return "删除成功.";
        }
        public string PushMsgEntity_Init()
        {
            DataSet ds = new DataSet();

            //字段下拉框.
            //select * from Sys_MapAttr where FK_MapData='ND102' and LGType = 0 AND MyDataType =1

            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            attrs.Retrieve(BP.Sys.MapAttrAttr.FK_MapData, "ND" + this.FK_Node, "LGType", 0, "MyDataType", 1);
            ds.Tables.Add(attrs.ToDataTableField("FrmFields"));

            //节点 
            //TODO 数据太多优化一下
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
            ds.Tables.Add(nds.ToDataTableField("Nodes"));

            //mypk
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();
            ds.Tables.Add(msg.ToDataTableField("PushMsgEntity"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        

        #endregion

        #region 表单模式
        /// <summary>
        /// 表单模式
        /// </summary>
        /// <returns></returns>
        public string NodeFromWorkModel_Init()
        {
            //数据容器.
            DataSet ds = new DataSet();

            // 当前节点信息.
            Node nd = new Node(this.FK_Node);

            nd.NodeFrmID = nd.NodeFrmID;
            // nd.FormUrl = nd.FormUrl;

            DataTable mydt = nd.ToDataTableField("WF_Node");
            ds.Tables.Add(mydt);

            BtnLabExtWebOffice mybtn = new BtnLabExtWebOffice(this.FK_Node);
            DataTable mydt2 = mybtn.ToDataTableField("WF_BtnLabExtWebOffice");
            ds.Tables.Add(mydt2);

            BtnLab btn = new BtnLab(this.FK_Node);
            DataTable dtBtn = btn.ToDataTableField("WF_BtnLab");
            ds.Tables.Add(dtBtn);

            //节点s
            Nodes nds = new Nodes(nd.FK_Flow);

            //节点s
            ds.Tables.Add(nds.ToDataTableField("Nodes"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 表单模式
        /// </summary>
        /// <returns></returns>
        public string NodeFromWorkModel_Save()
        {
            Node nd = new Node(this.FK_Node);

            BP.Sys.MapData md = new BP.Sys.MapData("ND" + this.FK_Node);

            //用户选择的表单类型.
            string selectFModel = this.GetValFromFrmByKey("FrmS");

            //使用ccbpm内置的节点表单
            if (selectFModel == "DefFrm")
            {
                string frmModel = this.GetValFromFrmByKey("RB_Frm");
                if (frmModel == "0")
                {
                    nd.FormType = NodeFormType.FreeForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.FixForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FoolForm;
                    md.Update();
                }

                string refFrm = this.GetValFromFrmByKey("RefFrm");

                if (refFrm == "0")
                {
                    nd.NodeFrmID = "";
                    nd.DirectUpdate();
                }

                if (refFrm == "1")
                {
                    nd.NodeFrmID = "ND" + this.GetValFromFrmByKey("DDL_Frm");
                    nd.DirectUpdate();
                }
            }

            //使用傻瓜轨迹表单模式.
            if (selectFModel == "FoolTruck")
            {
                nd.FormType = NodeFormType.FoolTruck;
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.FoolForm;  //同时更新表单表住表.
                md.Update();
            }

            //使用嵌入式表单
            if (selectFModel == "SelfForm")
            {
                nd.FormType = NodeFormType.SelfForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_CustomURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;  //同时更新表单表住表.
                md.Url = this.GetValFromFrmByKey("TB_CustomURL");
                md.Update();

            }
            //使用SDK表单
            if (selectFModel == "SDKForm")
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_FormURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;
                md.Url = this.GetValFromFrmByKey("TB_FormURL");
                md.Update();

            }
            //绑定多表单
            if (selectFModel == "SheetTree")
            {

                string sheetTreeModel = this.GetValFromFrmByKey("SheetTreeModel");

                if (sheetTreeModel == "0")
                {
                    nd.FormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
            }

            //如果公文表单选择了
            if (selectFModel == "WebOffice")
            {
                nd.FormType = NodeFormType.WebOffice;
                nd.Update();

                //按钮标签.
                BtnLabExtWebOffice btn = new BtnLabExtWebOffice(this.FK_Node);

                // tab 页工作风格.
                string WebOfficeStyle = this.GetValFromFrmByKey("WebOfficeStyle");
                if (WebOfficeStyle == "0")
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.FrmFirst;
                else
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.WordFirst;


                string WebOfficeFrmType = this.GetValFromFrmByKey("WebOfficeFrmType");
                //表单工作模式.
                if (WebOfficeFrmType == "0")
                {
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FreeFrm;

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;  //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FoolForm;

                    md.HisFrmType = BP.Sys.FrmType.FoolForm; //同时更新表单表住表.
                    md.Update();
                }

                btn.Update();
            }

            return "保存成功...";
        }
        #endregion 表单模式

        #region 手机表单字段排序
        #region SortingMapAttrs_Init

        public string SortingMapAttrs_Init()
        {
            MapDatas mapdatas;
            MapAttrs attrs;
            GroupFields groups;
            MapDtls dtls;
            FrmAttachments athMents;
            FrmBtns btns;

            Nodes nodes = null;

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
            qo.AddWhere(GroupFieldAttr.EnName, FK_MapData);
            qo.addOrderBy(GroupFieldAttr.Idx);
            qo.DoQuery();
            #endregion


            DataSet ds = new DataSet();

            _BindData4SortingMapAttrs_Init(mapdatas,
            attrs,
            groups,
            dtls,
            athMents,
            btns,
            nodes,
            ds);




            //string s = "";

            return BP.Tools.Json.ToJson(ds);
        }

        private void _BindData4SortingMapAttrs_Init(MapDatas mapdatas,
            MapAttrs attrs,
            GroupFields groups,
            MapDtls dtls,
            FrmAttachments athMents,
            FrmBtns btns,
            Nodes nodes,
            DataSet ds)
        {
            MapData mapdata = mapdatas.GetEntityByKey(FK_MapData) as MapData;
            DataTable dtAttrs = attrs.ToDataTableField("dtAttrs");
            DataTable dtDtls = dtls.ToDataTableField("dtDtls");
            DataTable dtGroups = groups.ToDataTableField("dtGroups");
            DataTable dtNoGroupAttrs = null;
            DataRow[] rows_Attrs = null;
            //LinkBtn btn = null;
            //DDL ddl = null;
            int idx_Attr = 1;
            int gidx = 1;
            GroupField group = null;

            if (mapdata != null)
            {
                #region 一、面板1、 分组数据+未分组数据
                //pub1.AddEasyUiPanelInfoBegin(mapdata.Name + "[" + mapdata.No + "]字段排序", padding: 5);
                //pub1.AddTable("class='Table' border='0' cellpadding='0' cellspacing='0' style='width:100%'");

                #region 标题行常量

                //pub1.AddTR();
                //pub1.AddTDGroupTitle("style='width:40px;text-align:center'", "序");
                //pub1.AddTDGroupTitle("style='width:100px;'", "字段名称");
                //pub1.AddTDGroupTitle("style='width:160px;'", "中文描述");
                //pub1.AddTDGroupTitle("style='width:160px;'", "字段分组");
                //pub1.AddTDGroupTitle("字段排序");
                //pub1.AddTREnd();

                #endregion

                #region A、构建数据dtNoGroupAttrs，这个放在前面
                //检索全部字段，查找出没有分组或分组信息不正确的字段，存入“无分组”集合
                dtNoGroupAttrs = dtAttrs.Clone();

                foreach (DataRow dr in dtAttrs.Rows)
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

                
                #endregion


                #region 三、其他。如果是明细表的字段排序，则增加“返回”按钮；否则增加“复制排序”按钮,2016-03-21

                MapDtl tdtl = new MapDtl();
                tdtl.No = FK_MapData;
                if (tdtl.RetrieveFromDBSources() == 1)
                {
                    //pub1.Add(
                    //        string.Format(
                    //            "<a href='{0}' target='_self' class='easyui-linkbutton' data-options=\"iconCls:'icon-back'\">返回</a>",
                    //            Request.Path + "?FK_Flow=" + (FK_Flow ??
                    //                                          string.Empty) +
                    //            "&FK_MapData=" + tdtl.FK_MapData +
                    //            "&t=" +
                    //            DateTime.Now.ToString("yyyyMMddHHmmssffffff")));
                }
                else
                {
                    //btn = new LinkBtn(false, "Btn_ResetAttr_Idx", "重置顺序");
                    //btn.SetDataOption("iconCls", "'icon-reset'");
                    //btn.Click += btnReSet_Click;
                    //pub1.Add(btn);
                    //pub1.Add("<a href='javascript:void(0)' onclick=\"Form_View('" + this.FK_MapData + "','" + this.FK_Flow + "');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-search'\">预览</a>");
                    //pub1.Add("<a href='javascript:void(0)' onclick=\"$('#nodes').dialog('open');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-copy'\">复制排序</a>");
                    //pub1.Add("&nbsp;<a href='javascript:void(0)' onclick=\"GroupFieldNew('" + this.FK_MapData + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-addfolder'\">新建分组</a>");
                    //pub1.AddBR();
                    //pub1.AddBR();

                    //pub1.Add(
                    //    "<div id='nodes' class='easyui-dialog' data-options=\"title:'选择复制到节点（多选）:',closed:true,buttons:'#btns'\" style='width:280px;height:340px'>");

                    //ListBox lb = new ListBox();
                    //lb.Style.Add("width", "100%");
                    //lb.Style.Add("Height", "100%");
                    //lb.SelectionMode = ListSelectionMode.Multiple;
                    //lb.BorderStyle = BorderStyle.None;
                    //lb.ID = "lbNodes";

                    nodes = new Nodes();
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

                    //ListItem item = null;

                    //foreach (BP.WF.Node node in nodes)
                    //{
                    //    item = new ListItem(string.Format("({0}){1}", node.NodeID, node.Name),
                    //                              node.NodeID.ToString());

                    //    if ("ND" + node.NodeID == FK_MapData)
                    //        item.Attributes.Add("disabled", "disabled");

                    //    lb.Items.Add(item);
                    //}

                    //pub1.Add(lb);
                    //pub1.AddDivEnd();

                    //pub1.Add("<div id='btns'>");

                    //LinkBtn lbtn = new LinkBtn(false, NamesOfBtn.Copy, "复制");
                    //lbtn.OnClientClick = "var v = $('#" + lb.ClientID + "').val(); if(!v) { alert('请选择将此排序复制到的节点！'); return false; } else { $('#" + hidCopyNodes.ClientID + "').val(v); return true; }";
                    //lbtn.Click += new EventHandler(lbtn_Click);
                    //pub1.Add(lbtn);
                    //lbtn = new LinkBtn(false, NamesOfBtn.Cancel, "取消");
                    //lbtn.OnClientClick = "$('#nodes').dialog('close');";
                    //pub1.Add(lbtn);

                    //pub1.AddDivEnd();
                }
                #endregion


                ds.Tables.Add(mapdatas.ToDataTableField("mapdatas"));
                dtGroups.TableName = "dtGroups";
                ds.Tables.Add(dtGroups);
                dtNoGroupAttrs.TableName = "dtNoGroupAttrs";
                ds.Tables.Add(dtNoGroupAttrs);
                dtAttrs.TableName = "dtAttrs";
                ds.Tables.Add(dtAttrs);
                dtDtls.TableName = "dtDtls";
                ds.Tables.Add(dtDtls);
                ds.Tables.Add(athMents.ToDataTableField("athMents"));
                ds.Tables.Add(btns.ToDataTableField("btns"));
                ds.Tables.Add(nodes.ToDataTableField("nodes"));
            }
        }

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

        private int GetGroupID(string ctrlID, GroupFields gfs)
        {
            GroupField gf = gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
            return gf == null ? 0 : gf.OID;
        }

        #endregion
        public string SortingMapAttrs_Save()
        {
            Node nd = new Node(this.FK_Node);

            BP.Sys.MapData md = new BP.Sys.MapData("ND" + this.FK_Node);

            //用户选择的表单类型.
            string selectFModel = this.GetValFromFrmByKey("FrmS");

            //使用ccbpm内置的节点表单
            if (selectFModel == "DefFrm")
            {
                string frmModel = this.GetValFromFrmByKey("RB_Frm");
                if (frmModel == "0")
                {
                    nd.FormType = NodeFormType.FreeForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.FixForm;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FoolForm;
                    md.Update();
                }

                string refFrm = this.GetValFromFrmByKey("RefFrm");

                if (refFrm == "0")
                {
                    nd.NodeFrmID = "";
                    nd.DirectUpdate();
                }

                if (refFrm == "1")
                {
                    nd.NodeFrmID = "ND" + this.GetValFromFrmByKey("DDL_Frm");
                    nd.DirectUpdate();
                }
            }

            //使用傻瓜轨迹表单模式.
            if (selectFModel == "FoolTruck")
            {
                nd.FormType = NodeFormType.FoolTruck;
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.FoolForm;  //同时更新表单表住表.
                md.Update();
            }

            //使用嵌入式表单
            if (selectFModel == "SelfForm")
            {
                nd.FormType = NodeFormType.SelfForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_CustomURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;  //同时更新表单表住表.
                md.Url = this.GetValFromFrmByKey("TB_CustomURL");
                md.Update();

            }
            //使用SDK表单
            if (selectFModel == "SDKForm")
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_FormURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;
                md.Url = this.GetValFromFrmByKey("TB_FormURL");
                md.Update();

            }
            //绑定多表单
            if (selectFModel == "SheetTree")
            {

                string sheetTreeModel = this.GetValFromFrmByKey("SheetTreeModel");

                if (sheetTreeModel == "0")
                {
                    nd.FormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm; //同时更新表单表住表.
                    md.Update();
                }
            }

            //如果公文表单选择了
            if (selectFModel == "WebOffice")
            {
                nd.FormType = NodeFormType.WebOffice;
                nd.Update();

                //按钮标签.
                BtnLabExtWebOffice btn = new BtnLabExtWebOffice(this.FK_Node);

                // tab 页工作风格.
                string WebOfficeStyle = this.GetValFromFrmByKey("WebOfficeStyle");
                if (WebOfficeStyle == "0")
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.FrmFirst;
                else
                    btn.WebOfficeWorkModel = WebOfficeWorkModel.WordFirst;


                string WebOfficeFrmType = this.GetValFromFrmByKey("WebOfficeFrmType");
                //表单工作模式.
                if (WebOfficeFrmType == "0")
                {
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FreeFrm;

                    md.HisFrmType = BP.Sys.FrmType.FreeFrm;  //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    btn.WebOfficeFrmModel = BP.Sys.FrmType.FoolForm;

                    md.HisFrmType = BP.Sys.FrmType.FoolForm; //同时更新表单表住表.
                    md.Update();
                }

                btn.Update();
            }

            return "保存成功...";
        }
        #endregion 表单模式

        #region 事件.
        public string Action_Init()
        {
            return "";
        }
        #endregion 事件.

        #region 考核超时规则.
        /// <summary>
        /// 初始化考核规则.
        /// </summary>
        /// <returns></returns>
        public string CHOvertimeRole_Init()
        {

            BP.WF.Node nd = new Node(this.FK_Node);

            Nodes nds = new Nodes();
            nds.Retrieve(NodeAttr.FK_Flow, nd.FK_Flow);

            //组装json.
            DataSet ds = new DataSet();

            DataTable dtNodes = nds.ToDataTableField("Nodes");
            dtNodes.TableName = "Nodes";
            ds.Tables.Add(dtNodes);

            DataTable dtNode = nds.ToDataTableField("Node");
            dtNode.TableName = "Node";
            ds.Tables.Add(dtNode);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        public string CHOvertimeRole_Save()
        {
            BP.WF.Node nd = new Node(this.FK_Node);

            int val = this.GetRequestValInt("RB_OutTimeDeal");

            var deal = (BP.WF.OutTimeDeal)val;

            nd.HisOutTimeDeal = deal;

            if (nd.HisOutTimeDeal == OutTimeDeal.AutoJumpToSpecNode)
                nd.DoOutTime = this.GetRequestVal("DDL_Nodes");

            if (nd.HisOutTimeDeal == OutTimeDeal.AutoShiftToSpecUser)
                nd.DoOutTime = this.GetRequestVal("TB_Shift");

            if (nd.HisOutTimeDeal == OutTimeDeal.SendMsgToSpecUser)
                nd.DoOutTime = this.GetRequestVal("TB_SendEmps");

            if (nd.HisOutTimeDeal == OutTimeDeal.RunSQL)
                nd.DoOutTime = this.GetRequestVal("TB_SQL");

            //是否质量考核节点.
            if (this.GetRequestValInt("IsEval") == 0)
                nd.IsEval = false;
            else
                nd.IsEval = true;

            //执行更新.
            nd.Update();

            return "@保存成功.";
        }
        #endregion

        #region 多人处理规则.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistModel_Init()
        {
            //nd.TodolistModel = (TodolistModel)this.GetRequestValInt("RB_TodolistModel");  //考核方式.
            //nd.TeamLeaderConfirmRole = (TeamLeaderConfirmRole)this.GetRequestValInt("DDL_TeamLeaderConfirmRole");  //考核方式.
            //nd.TeamLeaderConfirmDoc = this.GetRequestVal("TB_TeamLeaderConfirmDoc");
            //nd.Update();

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            ht.Add("TodolistModel", (int)nd.TodolistModel);
            ht.Add("TeamLeaderConfirmRole", (int)nd.TeamLeaderConfirmRole);
            ht.Add("TeamLeaderConfirmDoc", nd.TeamLeaderConfirmDoc);
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string TodolistModel_Save()
        {
            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            nd.TodolistModel = (TodolistModel)this.GetRequestValInt("RB_TodolistModel");  //考核方式.
            nd.TeamLeaderConfirmRole = (TeamLeaderConfirmRole)this.GetRequestValInt("DDL_TeamLeaderConfirmRole");  //考核方式.
            nd.TeamLeaderConfirmDoc = this.GetRequestVal("TB_TeamLeaderConfirmDoc");

            nd.Update();

            return "保存成功...";
        }

        #endregion 多人处理规则.

        #region 考核规则.
        public string CHRole_Init()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            return nd.ToJson();
        }

        public string CHRole_Save()
        {
            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            nd.HisCHWay = (CHWay)this.GetRequestValInt("RB_CHWay");  //考核方式.

            nd.TimeLimit = this.GetRequestValInt("TB_TimeLimit");
            nd.WarningDay = this.GetRequestValInt("TB_WarningDay");
            nd.TCent = this.GetRequestValFloat("TB_TCent");

            nd.TWay = (BP.DA.TWay)this.GetRequestValInt("DDL_TWay");  //节假日计算方式.

            if (this.GetRequestValInt("CB_IsEval") == 1)
                nd.IsEval = true;
            else
                nd.IsEval = false;

            nd.Update();

            return "保存成功...";
        }
        #endregion 考核规则.

        #region 节点属性（列表）的操作
        /// <summary>
        /// 初始化节点属性列表.
        /// </summary>
        /// <returns></returns>
        public string NodeAttrs_Init()
        {
            var strFlowId = GetRequestVal("FK_Flow");
            if (string.IsNullOrEmpty(strFlowId))
            {
                return "err@参数错误！";
            }
            Nodes nodes = new Nodes();
            nodes.Retrieve("FK_Flow", strFlowId);
            //因直接使用nodes.ToJson()无法获取某些字段（e.g.HisFormTypeText,原因：Node没有自己的Attr类）
            //故此处手动创建前台所需的DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("NodeID");	//节点ID
            dt.Columns.Add("Name");		//节点名称
            dt.Columns.Add("HisFormType");		//表单方案
            dt.Columns.Add("HisFormTypeText");
            dt.Columns.Add("HisRunModel");		//节点类型
            dt.Columns.Add("HisRunModelT");

            dt.Columns.Add("HisDeliveryWay");	//接收方类型
            dt.Columns.Add("HisDeliveryWayText");
            dt.Columns.Add("HisDeliveryWayJsFnPara");
            dt.Columns.Add("HisDeliveryWayCountLabel");
            dt.Columns.Add("HisDeliveryWayCount");	//接收方Count

            dt.Columns.Add("HisCCRole");	//抄送人
            dt.Columns.Add("HisCCRoleText");
            dt.Columns.Add("HisFrmEventsCount");	//消息&事件Count
            dt.Columns.Add("HisFinishCondsCount");	//流程完成条件Count
            DataRow dr;
            foreach (Node node in nodes)
            {
                dr = dt.NewRow();
                dr["NodeID"] = node.NodeID;
                dr["Name"] = node.Name;
                dr["HisFormType"] = node.HisFormType;
                dr["HisFormTypeText"] = node.HisFormTypeText;
                dr["HisRunModel"] = node.HisRunModel;
                dr["HisRunModelT"] = node.HisRunModelT;
                dr["HisDeliveryWay"] = node.HisDeliveryWay;
                dr["HisDeliveryWayText"] = node.HisDeliveryWayText;

                //接收方数量
                var intHisDeliveryWayCount = 0;
                if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByStation)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByStation";
                    dr["HisDeliveryWayCountLabel"] = "岗位";
                    BP.WF.Template.NodeStations nss = new BP.WF.Template.NodeStations();
                    intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
                }
                else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByDept)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByDept";
                    dr["HisDeliveryWayCountLabel"] = "部门";
                    BP.WF.Template.NodeDepts nss = new BP.WF.Template.NodeDepts();
                    intHisDeliveryWayCount = nss.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, node.NodeID);
                }
                else if (node.HisDeliveryWay == BP.WF.DeliveryWay.ByBindEmp)
                {
                    dr["HisDeliveryWayJsFnPara"] = "ByDept";
                    dr["HisDeliveryWayCountLabel"] = "人员";
                    BP.WF.Template.NodeEmps nes = new BP.WF.Template.NodeEmps();
                    intHisDeliveryWayCount = nes.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, node.NodeID);
                }
                dr["HisDeliveryWayCount"] = intHisDeliveryWayCount;

                //抄送
                dr["HisCCRole"] = node.HisCCRole;
                dr["HisCCRoleText"] = node.HisCCRoleText;

                //消息&事件Count
                BP.Sys.FrmEvents fes = new BP.Sys.FrmEvents();
                dr["HisFrmEventsCount"] = fes.Retrieve(BP.Sys.FrmEventAttr.FK_MapData, "ND" + node.NodeID);

                //流程完成条件Count
                BP.WF.Template.Conds conds = new BP.WF.Template.Conds(BP.WF.Template.CondType.Flow, node.NodeID);
                dr["HisFinishCondsCount"] = conds.Count;


                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        #region 发送后转向处理规则
        public string TurnToDeal_Init()
        {

            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();

            Hashtable ht = new Hashtable();
            ht.Add(NodeAttr.TurnToDeal, (int)nd.HisTurnToDeal);
            ht.Add(NodeAttr.TurnToDealDoc, nd.TurnToDealDoc);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion

        #region 发送后转向处理规则Save
        /// <summary>
        /// 前置导航save
        /// </summary>
        /// <returns></returns>
        public string TurnToDeal_Save()
        {
            try
            {
                int nodeID = int.Parse(this.FK_Node.ToString());
                BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
                BP.WF.Node nd = new BP.WF.Node(nodeID);

                int val = this.GetRequestValInt("TurnToDeal");

                //遍历页面radiobutton
                if (0 == val)
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.CCFlowMsg;
                }
                else if (1 == val)
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecMsg;
                    nd.TurnToDealDoc = this.GetRequestVal("TB_SpecMsg");
                }
                else
                {
                    nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecUrl;
                    nd.TurnToDealDoc = this.GetRequestVal("TB_SpecURL");
                }
                //执行保存操作
                nd.Update();

                return "保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 特别控件特别用户权限
        public string SepcFiledsSepcUsers_Init()
        {

            /*string fk_mapdata = this.GetRequestVal("FK_MapData");
            if (string.IsNullOrEmpty(fk_mapdata))
                fk_mapdata = "ND101";

            string fk_node = this.GetRequestVal("FK_Node");
            if (string.IsNullOrEmpty(fk_node))
                fk_mapdata = "101";


            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(fk_mapdata);

            BP.Sys.FrmImgs imgs = new BP.Sys.FrmImgs(fk_mapdata);

            BP.Sys.MapExts exts = new BP.Sys.MapExts();
            int mecount = exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata,
                BP.Sys.MapExtAttr.Tag, this.GetRequestVal("FK_Node"),
                BP.Sys.MapExtAttr.ExtType, "SepcFiledsSepcUsers");

            BP.Sys.FrmAttachments aths = new BP.Sys.FrmAttachments(fk_mapdata);

            exts = new BP.Sys.MapExts();
            exts.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata,
                BP.Sys.MapExtAttr.Tag, this.GetRequestVal("FK_Node"),
                BP.Sys.MapExtAttr.ExtType, "SepcAthSepcUsers");
            */
            return "";//toJson
        }
        #endregion

        #region 批量发起规则设置
        public string BatchStartFields_Init()
        {

            int nodeID = int.Parse(this.FK_Node.ToString());
            //获取节点字段集合
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            //获取节点对象
            BP.WF.Node nd = new BP.WF.Node(nodeID);
            //获取批量发起设置规则
            BP.Sys.SysEnums ses = new BP.Sys.SysEnums(BP.WF.Template.NodeAttr.BatchRole);
            //获取当前节点设置的批处理规则
            string srole = "";
            if (nd.HisBatchRole == BatchRole.None)
                srole = "0";
            else if (nd.HisBatchRole == BatchRole.Ordinary)
                srole = "1";
            else
                srole = "2";
            return "{\"nd\":" + nd.ToJson() + ",\"ses\":" + ses.ToJson() + ",\"attrs\":" + attrs.ToJson() + ",\"BatchRole\":" + srole + "}";
        }
        #endregion

        #region 批量发起规则设置save
        public string BatchStartFields_Save()
        {

            int nodeID = int.Parse(this.FK_Node.ToString());
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //给变量赋值.
            //批处理的类型
            int selectval = int.Parse(this.GetRequestVal("DDL_BRole"));
            switch (selectval)
            {
                case 0:
                    nd.HisBatchRole = BP.WF.BatchRole.None;
                    break;
                case 1:
                    nd.HisBatchRole = BP.WF.BatchRole.Ordinary;
                    break;
                default:
                    nd.HisBatchRole = BP.WF.BatchRole.Group;
                    break;
            }
            //批处理的数量
            nd.BatchListCount = int.Parse(this.GetRequestVal("TB_BatchListCount"));
            //批处理的参数 
            string sbatchparas = "";
            if (this.GetRequestVal("CB_Node") != null)
            {
                sbatchparas = this.GetRequestVal("CB_Node");
            }
            nd.BatchParas = sbatchparas;
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 发送阻塞模式
        public string BlockModel_Init()
        {
            BP.WF.Node nd = new BP.WF.Node();
            nd.NodeID = this.FK_Node;
            nd.RetrieveFromDBSources();
            return nd.ToJson();
        }
        public string BlockModel_Save()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            
            nd.BlockAlert = this.GetRequestVal("TB_Alert"); //提示信息.

            int val = this.GetRequestValInt("RB_BlockModel");
            nd.SetValByKey(BP.WF.Template.NodeAttr.BlockModel, val);
            if (nd.BlockModel == BP.WF.BlockModel.None)
                nd.BlockModel = BP.WF.BlockModel.None;

            if (nd.BlockModel == BP.WF.BlockModel.CurrNodeAll)
                nd.BlockModel = BP.WF.BlockModel.CurrNodeAll;

            if (nd.BlockModel == BP.WF.BlockModel.SpecSubFlow)
            {
                nd.BlockModel = BP.WF.BlockModel.SpecSubFlow;
                nd.BlockExp = this.GetRequestVal("TB_SpecSubFlow");
            }

            if (nd.BlockModel == BP.WF.BlockModel.BySQL)
            {
                nd.BlockModel = BP.WF.BlockModel.BySQL;
                nd.BlockExp = this.GetRequestVal("TB_SQL");
            }

            if (nd.BlockModel == BP.WF.BlockModel.ByExp)
            {
                nd.BlockModel = BP.WF.BlockModel.ByExp;
                nd.BlockExp = this.GetRequestVal("TB_Exp");
            }

            nd.BlockAlert = this.GetRequestVal("TB_Alert");
            nd.Update();

            return "保存成功.";
        }
        #endregion

        #region 可以撤销的节点
        public string CanCancelNodes_Init()
        {

            BP.WF.Node mynd = new BP.WF.Node();
            mynd.NodeID = this.FK_Node;
            mynd.RetrieveFromDBSources();

            BP.WF.Template.NodeCancels rnds = new BP.WF.Template.NodeCancels();
            rnds.Retrieve(NodeCancelAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            return "{\"mynd\":" + mynd.ToJson() + ",\"rnds\":" + rnds.ToJson() + ",\"nds\":" + nds.ToJson() + "}";
        }
        public string CanCancelNodes_Save()
        {
            BP.WF.Template.NodeCancels rnds = new BP.WF.Template.NodeCancels();
            rnds.Delete(BP.WF.Template.NodeCancelAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int i = 0;
            foreach (BP.WF.Node nd in nds)
            {
                string cb = this.GetRequestVal("CB_" + nd.NodeID);
                if (cb == null || cb == "")
                    continue;

                NodeCancel nr = new NodeCancel();
                nr.FK_Node = this.FK_Node;
                nr.CancelTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
            {
                return "请您选择要撤销的节点。";
            }
            return "设置成功.";
        }
        #endregion


        #region 可以退回的节点
        public string CanReturnNodes_Init()
        {

            BP.WF.Node mynd = new BP.WF.Node();
            mynd.NodeID = this.FK_Node;
            mynd.RetrieveFromDBSources();

            BP.WF.Template.NodeReturns rnds = new BP.WF.Template.NodeReturns();
            rnds.Retrieve(NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            return "{\"mynd\":" + mynd.ToJson() + ",\"rnds\":" + rnds.ToJson() + ",\"nds\":" + nds.ToJson() + "}";
        }
        public string CanReturnNodes_Save()
        {
            BP.WF.Template.NodeReturns rnds = new BP.WF.Template.NodeReturns();
            rnds.Delete(BP.WF.Template.NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int i = 0;
            foreach (BP.WF.Node nd in nds)
            {
                string cb = this.GetRequestVal("CB_" + nd.NodeID);
                if (cb == null || cb == "")
                    continue;

                NodeReturn nr = new NodeReturn();
                nr.FK_Node = this.FK_Node;
                nr.ReturnTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
            {
                return "请您选择要撤销的节点。";
            }
            return "设置成功.";
        }
        #endregion

        #region 表单检查(CheckFrm.htm)
        public string CheckFrm_Init()
        {
            if (string.IsNullOrWhiteSpace(this.FK_MapData))
                return "err@参数FK_MapData不能为空！";

            MapData md = new MapData(this.FK_MapData);
            return md.Name;
        }

        public string CheckFrm_Check()
        {
            if (BP.Web.WebUser.No != "admin")
                return "err@只有管理员有权限进行此项操作！";

            if (string.IsNullOrWhiteSpace(this.FK_MapData))
                return "err@参数FK_MapData不能为空！";

            string msg = string.Empty;

            //1.检查字段扩展设置
            MapExts mes = new MapExts(this.FK_MapData);
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            MapDtls dtls = new MapDtls(this.FK_MapData);
            Entity en = null;
            string fieldMsg = string.Empty;

            //1.1主表
            foreach (MapExt me in mes)
            {
                if (!string.IsNullOrWhiteSpace(me.AttrOfOper))
                {
                    en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrOfOper);

                    if (en != null && !string.IsNullOrWhiteSpace(me.AttrsOfActive))
                        en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrsOfActive);
                }

                if (en == null)
                {
                    me.DirectDelete();
                    msg += "删除扩展设置中MyPK=" + me.PKVal + "的设置项；<br />";
                }
            }

            //1.2明细表
            foreach (MapDtl dtl in dtls)
            {
                mes = new MapExts(dtl.No);
                attrs = new MapAttrs(dtl.No);

                foreach (MapExt me in mes)
                {
                    if (!string.IsNullOrWhiteSpace(me.AttrOfOper))
                    {
                        en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrOfOper);

                        if (en != null && !string.IsNullOrWhiteSpace(me.AttrsOfActive))
                            en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, me.AttrsOfActive);
                    }

                    if (en == null)
                    {
                        me.DirectDelete();
                        msg += "删除扩展设置中MyPK=" + me.PKVal + "的设置项；<br />";
                    }
                }
            }

            //2.检查字段权限
            FrmFields ffs = new FrmFields();
            ffs.Retrieve(FrmFieldAttr.FK_MapData, this.FK_MapData);

            //2.1主表
            foreach (FrmField ff in ffs)
            {
                en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ff.KeyOfEn);

                if (en == null)
                {
                    ff.DirectDelete();
                    msg += "删除字段权限中MyPK=" + ff.PKVal + "的设置项；<br />";
                }
            }

            //2.2明细表
            foreach (MapDtl dtl in dtls)
            {
                ffs = new FrmFields();
                ffs.Retrieve(FrmFieldAttr.FK_MapData, dtl.No);
                attrs = new MapAttrs(dtl.No);

                foreach (FrmField ff in ffs)
                {
                    en = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ff.KeyOfEn);

                    if (en == null)
                    {
                        ff.DirectDelete();
                        msg += "删除字段权限中MyPK=" + ff.PKVal + "的设置项；<br />";
                    }
                }
            }

            msg += "检查完成！";

            return msg;
        }
        #endregion

        #region 消息事件
        public string PushMessage_Init()
        {
            BP.WF.Template.PushMsg enDel = new BP.WF.Template.PushMsg();
            enDel.FK_Node = this.FK_Node;
            enDel.RetrieveFromDBSources();
            return enDel.ToJson();
        }

        public string PushMessage_Delete()
        {
            BP.WF.Template.PushMsg enDel = new BP.WF.Template.PushMsg();
            enDel.MyPK = this.MyPK; ;
            enDel.Delete();
            return "删除成功";
        }

        public string PushMessage_ShowHidden()
        {
            BP.WF.XML.EventLists xmls = new BP.WF.XML.EventLists();
            xmls.RetrieveAll();
            foreach (BP.WF.XML.EventList item in xmls)
            {
                if (item.IsHaveMsg == false)
                    continue;

            }
            return BP.Tools.Json.ToJson(xmls);


        }

        public string PushMessageEntity_Init()
        {
            var fk_node = GetRequestVal("FK_Node");
            BP.WF.Template.PushMsg en = new BP.WF.Template.PushMsg();
            en.MyPK = this.MyPK;
            en.FK_Event = this.FK_Event;
            en.RetrieveFromDBSources();
            return en.ToJson();
        }
        public string PushMessageEntity_Save()
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();
            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;
            // msg = BP.Sys.PubClass.CopyFromRequestByPost(msg, context.Request) as BP.WF.Template.PushMsg;
            msg.Save();  //执行保存.

            return "保存成功...";
        }
        #endregion
    }
}