using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 手机表单设计器
    /// </summary>
    public class WF_Admin_MobileFrmDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {
        public string Default_Init()
        {
            //分组.
            GroupFields gfs = new GroupFields();
            gfs.Retrieve(GroupFieldAttr.FrmID, this.FrmID, GroupFieldAttr.Idx);

            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, GroupFieldAttr.Idx);

            MapDtls mapDtls = new MapDtls();
            mapDtls.Retrieve(MapDtlAttr.FK_MapData, this.FrmID);

            FrmAttachments aths = new FrmAttachments(this.FrmID);

            DataSet ds = new DataSet();

            //分组.
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupFields"));

            //字段.
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttrs"));

            //从表.
            ds.Tables.Add(mapDtls.ToDataTableField("Sys_MapDtls"));

            //附件.
            ds.Tables.Add(aths.ToDataTableField("Sys_FrmAttachments"));

            return BP.Tools.Json.ToJson(ds);
        }

        public string Default_Init_bak()
        {
            Nodes nodes = null;

            #region 获取数据
            MapDatas mapdatas = new MapDatas();
            QueryObject qo = new QueryObject(mapdatas);
            qo.AddWhere(MapDataAttr.No, "Like", this.FrmID + "%");
            qo.addOrderBy(MapDataAttr.Idx);
            qo.DoQuery();

            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapDtlAttr.FK_MapData, this.FrmID, MapAttrAttr.EditType, 0,
                GroupFieldAttr.Idx);

            FrmBtns btns = new FrmBtns(this.FrmID);

            FrmAttachments athMents = new FrmAttachments(this.FrmID);

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, this.FrmID, GroupFieldAttr.Idx);

            GroupFields groups = new GroupFields();
            groups.Retrieve(GroupFieldAttr.FrmID, this.FrmID, GroupFieldAttr.Idx);
            #endregion

            DataSet ds = new DataSet();

            BindData4Default_Init(mapdatas,
            attrs,
            groups,
            dtls,
            athMents,
            btns,
            nodes,
            ds);

            //控制页面按钮需要的
            MapDtl tdtl = new MapDtl();
            tdtl.No = this.FrmID;
            if (tdtl.RetrieveFromDBSources() == 1)
            {
                ds.Tables.Add(tdtl.ToDataTableField("tdtl"));
            }

            return BP.Tools.Json.ToJson(ds);
        }

        private void BindData4Default_Init(MapDatas mapdatas,
            MapAttrs attrs,
            GroupFields groups,
            MapDtls dtls,
            FrmAttachments athMents,
            FrmBtns btns,
            Nodes nodes,
            DataSet ds)
        {
            MapData mapdata = mapdatas.GetEntityByKey(this.FrmID) as MapData;
            DataTable dtAttrs = attrs.ToDataTableField("dtAttrs");
            DataTable dtDtls = dtls.ToDataTableField("dtDtls");
            DataTable dtGroups = groups.ToDataTableField("dtGroups");
            DataTable dtNoGroupAttrs = null;
            DataRow[] rows_Attrs = null;
            int idx_Attr = 1;
            int gidx = 1;
            GroupField group = null;

            if (mapdata != null)
            {
                #region 一、面板1、 分组数据+未分组数据

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
                        group.FrmID = mapDtl.FrmID;
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
                        group.EnName = athMent.FrmID;
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
                        group.Lab = fbtn.Lab;
                        group.FrmID = fbtn.FrmID;
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
                DataTable isDtl = new DataTable();
                isDtl.Columns.Add("tdDtl", typeof(int));
                isDtl.Columns.Add("FK_MapData", typeof(string));
                isDtl.Columns.Add("No", typeof(string));
                isDtl.TableName = "TRDtl";

                DataRow tddr = isDtl.NewRow();

                MapDtl tdtl = new MapDtl();
                tdtl.No = this.FrmID;
                if (tdtl.RetrieveFromDBSources() == 1)
                {
                    tddr["tdDtl"] = 1;
                    tddr["FK_MapData"] = tdtl.FrmID;
                    tddr["No"] = tdtl.No;
                }
                else
                {
                    tddr["tdDtl"] = 0;
                    tddr["FK_MapData"] = this.FrmID;
                    tddr["No"] = tdtl.No;
                }
                isDtl.Rows.Add(tddr.ItemArray);
                #endregion

                #region 增加节点信息
                if (DataType.IsNullOrEmpty(this.FlowNo) == false)
                {
                    nodes = new Nodes();
                    nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FlowNo, BP.WF.Template.NodeAttr.Step);

                    if (nodes.Count == 0)
                    {
                        string nodeid = this.FrmID.Replace("ND", "");
                        string flowno = string.Empty;

                        if (nodeid.Length > 2)
                        {
                            flowno = nodeid.Substring(0, nodeid.Length - 2).PadLeft(3, '0');
                            nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, flowno, BP.WF.Template.NodeAttr.Step);
                        }
                    }
                    DataTable dtNodes = nodes.ToDataTableField("dtNodes");
                    dtNodes.TableName = "dtNodes";
                    ds.Tables.Add(dtNodes);
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
                ds.Tables.Add(isDtl);
                //ds.Tables.Add(nodes.ToDataTableField("nodes"));
            }
        }
        private int GetGroupID(string ctrlID, GroupFields gfs)
        {
            GroupField gf = gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
            return gf == null ? 0 : gf.OID;
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
                int rw = int.Parse(row[field].ToString());
                if (rw == int.Parse(value.ToString()))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 手机表单设计器
        /// </summary>
        public WF_Admin_MobileFrmDesigner()
        {
        }
        /// <summary>
        /// 保存需要在手机端表单显示的字段
        /// </summary>
        /// <returns></returns>
        public string Default_From_Save()
        {
            //获取需要显示的字段集合
            string atts = this.GetRequestVal("attrs");
            try
            {
                MapAttrs mattrs = new MapAttrs(this.FrmID);
                MapAttr att = null;
                //更新每个字段的显示属性
                foreach (MapAttr attr in mattrs)
                {
                    att = mattrs.GetEntityByKey(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                    if (atts.Contains("," + attr.KeyOfEn + ","))
                        att.ItIsEnableInAPP = true;
                    else
                        att.ItIsEnableInAPP = false;
                    att.Update();
                }
                //获取附件
                FrmAttachments aths = new FrmAttachments();
                aths.Retrieve(FrmAttachmentAttr.FK_MapData, this.FrmID, FrmAttachmentAttr.FK_Node, 0);
                foreach (FrmAttachment ath in aths)
                {
                    if (atts.Contains("," + ath.MyPK + ",") == true)
                        ath.SetPara("IsShowMobile", 1);
                    else
                        ath.SetPara("IsShowMobile", 0);
                    ath.Update();
                }
                return "保存成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }
        /// <summary>
        /// 将分组、字段排序复制到其他节点
        /// </summary>
        /// <returns></returns>
        public string Default_Copy()
        {
            try
            {
                string[] nodeids = this.GetRequestVal("NodeIDs").Split(',');

                MapDatas mapdatas = new MapDatas();
                QueryObject obj = new QueryObject(mapdatas);
                obj.AddWhere(MapDataAttr.No, "Like", this.FrmID + "%");
                obj.addOrderBy(MapDataAttr.Idx);
                obj.DoQuery();

                MapAttrs mattrs = new MapAttrs();
                obj = new QueryObject(mattrs);
                obj.AddWhere(MapAttrAttr.FK_MapData, this.FrmID);
                obj.addAnd();
                obj.AddWhere(MapAttrAttr.UIVisible, true);
                obj.addOrderBy(MapAttrAttr.GroupID, MapAttrAttr.Idx);
                obj.DoQuery();

                FrmBtns btns = new FrmBtns(this.FrmID);
                FrmAttachments athMents = new FrmAttachments(this.FrmID);
                MapDtls dtls = new MapDtls(this.FrmID);

                GroupFields groups = new GroupFields();
                obj = new QueryObject(groups);
                obj.AddWhere(GroupFieldAttr.FrmID, this.FrmID);
                obj.addOrderBy(GroupFieldAttr.Idx);
                obj.DoQuery();

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
                    foreach (MapAttr attr in mattrs)
                    {
                        //排除主键
                        if (attr.ItIsPK == true)
                            continue;

                        tattr = tattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;
                        if (tattr == null) continue;

                        group = groups.GetEntityByKey(GroupFieldAttr.OID, attr.GroupID) as GroupField;

                        //比对字段的分组是否一致，不一致则更新一致
                        if (group == null)
                        {
                            //源字段分组为空，则目标字段分组置为0
                            tattr.setGroupID(0);
                        }
                        else
                        {
                            //此处要判断目标节点中是否已经创建了这个源字段所属分组，如果没有创建，则要自动创建
                            tgrp = tgroups.GetEntityByKey(GroupFieldAttr.Lab, group.Lab) as GroupField;

                            if (tgrp == null)
                            {
                                tgrp = new GroupField();
                                tgrp.Lab = group.Lab;
                                tgrp.FrmID = tmd;
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

                    foreach (MapAttr attr in mattrs)
                    {
                        //排除主键
                        if (attr.ItIsPK == true)
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
                        dtlIdx = dtl.No.Replace(dtl.FrmID + "Dtl", "");
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
                            group.FrmID = tdtl.FrmID;
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
                                tattr.setGroupID(0);
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

                        if (tgroup == null)
                        {
                            tgroup = new GroupField();
                            tgroup.Lab = tdtl.Name;
                            tgroup.FrmID = tdtl.FrmID;
                            tgroup.CtrlType = GroupCtrlType.Dtl;
                            tgroup.CtrlID = tdtl.No;
                            tgroup.Idx = maxDtlIdx;
                            tgroup.Insert();

                            tgroups.AddEntity(group);
                        }

                        if (tgroup.Idx != maxDtlIdx)
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
                return "复制成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }

        private GroupField GetGroup(string ctrlID, GroupFields gfs)
        {
            return gfs.GetEntityByKey(GroupFieldAttr.CtrlID, ctrlID) as GroupField;
        }
        public string Default_Save()
        {
            Node nd = new Node(this.NodeID);

            BP.Sys.MapData md = new BP.Sys.MapData("ND" + this.NodeID);

            //用户选择的表单类型.
            string selectFModel = this.GetValFromFrmByKey("FrmS");

            //使用ccbpm内置的节点表单
            if (selectFModel == "DefFrm")
            {
                string frmModel = this.GetValFromFrmByKey("RB_Frm");
                if (frmModel == "0")
                {
                    nd.FormType = NodeFormType.Develop;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.Develop;
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.FoolForm;
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
                md.UrlExt = this.GetValFromFrmByKey("TB_CustomURL");
                md.Update();
            }
            //使用SDK表单
            if (selectFModel == "SDKForm")
            {
                nd.FormType = NodeFormType.SDKForm;
                nd.FormUrl = this.GetValFromFrmByKey("TB_FormURL");
                nd.DirectUpdate();

                md.HisFrmType = BP.Sys.FrmType.Url;
                md.UrlExt = this.GetValFromFrmByKey("TB_FormURL");
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

                    md.HisFrmType = BP.Sys.FrmType.FoolForm; //同时更新表单表住表.
                    md.Update();
                }
                else
                {
                    nd.FormType = NodeFormType.DisableIt;
                    nd.DirectUpdate();

                    md.HisFrmType = BP.Sys.FrmType.FoolForm; //同时更新表单表住表.
                    md.Update();
                }
            }
            return "保存成功...";
        }
        /// <summary>
        /// 重置字段顺序
        /// </summary>
        /// <returns></returns>
        public string Default_ReSet()
        {
            try
            {
                MapAttrs mattrs = new MapAttrs();
                QueryObject qo = new QueryObject(mattrs);
                qo.AddWhere(MapAttrAttr.FK_MapData, this.FrmID);//添加查询条件
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.UIVisible, true);
                //qo.addOrderBy(MapAttrAttr.Y, MapAttrAttr.X);
                qo.DoQuery();//执行查询
                int rowIdx = 0;
                //执行更新
                foreach (MapAttr mapAttr in mattrs)
                {
                    mapAttr.Idx = rowIdx;
                    mapAttr.DirectUpdate();
                    rowIdx++;
                }

                return "重置成功！";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message.ToString();
            }
        }
    }
}
