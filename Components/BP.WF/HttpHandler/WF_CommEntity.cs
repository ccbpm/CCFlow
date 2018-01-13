using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CommEntity : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_CommEntity(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 实体的操作.
        public string Entity_Init()
        {
            string enName = this.EnName;
            if (DataType.IsNullOrEmpty(enName) == true)
                return "err@类名没有传递过来";

            //初始化entity.
            Entity en = ClassFactory.GetEn(this.EnName);

            //获得描述.
            Map map = en.EnMap;
             
            string pkVal = this.PKVal;
            if (DataType.IsNullOrEmpty(pkVal) == false)
            {
                en.PKVal = pkVal;
                en.RetrieveFromDBSources();
            }
            else
            {
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, attr.DefaultVal);
                //设置默认的数据.
                en.ResetDefaultVal();
            }

            //定义容器.
            DataSet ds = new DataSet();

            //定义Sys_MapData.
            MapData md = new MapData();
            md.No = this.EnName;
            md.Name = map.EnDesc;

            #region 加入权限信息.
            //把权限加入参数里面.
            if (en.HisUAC.IsInsert)
                md.SetPara("IsInsert", "1");
            if (en.HisUAC.IsUpdate)
                md.SetPara("IsUpdate", "1");
            if (en.HisUAC.IsDelete)
                md.SetPara("IsDelete", "1");
            #endregion 加入权限信息.

            #region 增加 上方法.
            DataTable dtM = new DataTable("dtM");
            dtM.Columns.Add("No");
            dtM.Columns.Add("Title");
            dtM.Columns.Add("Tip");
            dtM.Columns.Add("Visable");

            dtM.Columns.Add("Url");
            dtM.Columns.Add("Target");
            dtM.Columns.Add("Warning");
            dtM.Columns.Add("RefMethodType");
            dtM.Columns.Add("GroupName");
            dtM.Columns.Add("W");
            dtM.Columns.Add("H");
            dtM.Columns.Add("Icon");
            dtM.Columns.Add("IsCanBatch");
            dtM.Columns.Add("RefAttrKey");

            RefMethods rms = map.HisRefMethods;
            foreach (RefMethod item in rms)
            {
                string myurl = "";
                if (item.RefMethodType != RefMethodType.Func)
                {
                    myurl = item.Do(null) as string;
                    if (myurl == null)
                        continue;
                }
                else
                {
                    myurl = "../RefMethod.htm?Index=" + item.Index + "&EnsName=" + en.GetNewEntities.ToString() + "&PK=" + this.PKVal;
                }

                DataRow dr = dtM.NewRow();

                dr["No"] = item.Index;
                dr["Title"] = item.Title;
                dr["Tip"] = item.ToolTip;
                dr["Visable"] = item.Visable;
                dr["Warning"] = item.Warning;
                dr["RefMethodType"] = (int)item.RefMethodType;
                dr["RefAttrKey"] = item.RefAttrKey;
                dr["URL"] = myurl;
                dr["W"] = item.Width;
                dr["H"] = item.Height;
                dr["Icon"] = item.Icon;
                dr["IsCanBatch"] = item.IsCanBatch;
                dr["GroupName"] = item.GroupName;

                dtM.Rows.Add(dr); //增加到rows.
            }
            #endregion 增加 上方法.

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            string sql = "";
            int i = 0;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    //判断该dot2dot是否显示？
                    Entity enMM = vsM.EnsOfMM.GetNewEntity;
                    enMM.SetValByKey(vsM.AttrOfOneInMM, this.PKVal);
                    if (enMM.HisUAC.IsView == false)
                        continue;
                    DataRow dr = dtM.NewRow();

                    ////判断模式.
                    //string url = "";
                    //if (vsM.Dot2DotModel == Dot2DotModel.TreeDept)
                    //    url = "Dot2DotTreeDeptModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    //else if (vsM.Dot2DotModel == Dot2DotModel.TreeDeptEmp)
                    //    url = "Dot2DotTreeDeptEmpModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    //else
                    //    url = "Dot2Dot.aspx?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    //try
                    //{
                    //    sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                    //    i = DBAccess.RunSQLReturnValInt(sql);
                    //}
                    //catch
                    //{
                    //    sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal;
                    //    try
                    //    {
                    //        i = DBAccess.RunSQLReturnValInt(sql);
                    //    }
                    //    catch
                    //    {
                    //        vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                    //    }
                    //}
                    //if (i == 0)
                    //{
                    //    if (this.AttrKey == vsM.EnsOfMM.ToString())
                    //    {
                    //        //AddLi(string.Format(
                    //        //    "<div style='font-weight:bold'><a href='{0}'>{3}<span class='nav'>{1}</span></a></div>{2}",
                    //        //    url, vsM.Desc, Environment.NewLine, GetIcon(IconM2MDefault)));
                    //        AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc, url, IconM2MDefault, true));
                    //        ItemCount++;
                    //    }
                    //    else
                    //    {
                    //        //AddLi(string.Format("<div><a href='{0}'>{3}<span class='nav'>{1}</span></a></div>{2}", url, vsM.Desc, Environment.NewLine, GetIcon(IconM2MDefault)));
                    //        AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc, url, IconM2MDefault, false));
                    //        ItemCount++;
                    //    }
                    //}
                    //else
                    //{
                    //    if (this.AttrKey == vsM.EnsOfMM.ToString())
                    //    {
                    //        //AddLi(string.Format(
                    //        //    "<div style='font-weight:bold'><a href='{0}'>{4}<span class='nav'>{1}[{2}]</span></a></div>{3}",
                    //        //    url, vsM.Desc, i, Environment.NewLine, GetIcon(IconM2MDefault)));
                    //        AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc + "[" + i + "]", url, IconM2MDefault, true));
                    //        ItemCount++;
                    //    }
                    //    else
                    //    {
                    //        //AddLi(string.Format("<div><a href='{0}'>{4}<span class='nav'>{1}[{2}]</span></a></div>{3}", url, vsM.Desc, i, Environment.NewLine, GetIcon(IconM2MDefault)));
                    //        AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc + "[" + i + "]", url, IconM2MDefault, false));
                    //        ItemCount++;
                    //    }
                    //}
                }
            }
            ds.Tables.Add(dtM);
            #endregion 增加 一对多.



            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            //把主数据放入里面去.
            DataTable dtMain = en.ToDataTableField("MainTable");
            ds.Tables.Add(dtMain);

            #region 增加上分组信息.
            EnCfg ec = new EnCfg(this.EnName);
            string groupTitle = ec.GroupTitle;
            if (DataType.IsNullOrEmpty(groupTitle) == true)
                groupTitle = "@" + en.PK + ",基本信息," + map.EnDesc + "";

            //增加上.
            DataTable dtGroups = new DataTable("Sys_GroupField");
            dtGroups.Columns.Add("OID");
            dtGroups.Columns.Add("Lab");
            dtGroups.Columns.Add("Tip");
            dtGroups.Columns.Add("CtrlType");
            dtGroups.Columns.Add("CtrlID");

            string[] strs = groupTitle.Split('@');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;

                string[] vals = str.Split('=');
                if (vals.Length == 1)
                    vals = str.Split(',');

                if (vals.Length == 0)
                    continue;

                DataRow dr = dtGroups.NewRow();
                dr["OID"] = vals[0];
                dr["Lab"] = vals[1];
                if (vals.Length == 3)
                    dr["Tip"] = vals[2];
                dtGroups.Rows.Add(dr);
            }
            ds.Tables.Add(dtGroups);

            #endregion 增加上分组信息.

            #region 字段属性.
            MapAttrs attrs = en.EnMap.Attrs.ToMapAttrs;
            DataTable sys_MapAttrs = attrs.ToDataTableField("Sys_MapAttr");
            sys_MapAttrs.Columns.Remove(MapAttrAttr.GroupID);
            sys_MapAttrs.Columns.Add("GroupID");

            //sys_MapAttrs.Columns[MapAttrAttr.GroupID].DataType = typeof(string); //改变列类型.

            //给字段增加分组.
            string currGroupID = "";
            foreach (DataRow drAttr in sys_MapAttrs.Rows)
            {
                if (currGroupID.Equals("") == true)
                    currGroupID = dtGroups.Rows[0]["OID"].ToString();

                string keyOfEn = drAttr[MapAttrAttr.KeyOfEn].ToString();
                foreach (DataRow drGroup in dtGroups.Rows)
                {
                    string field = drGroup["OID"].ToString();
                    if (keyOfEn.Equals( field) )
                    {
                        currGroupID = field;
                    }
                }
                drAttr[MapAttrAttr.GroupID] = currGroupID;
            }
            ds.Tables.Add(sys_MapAttrs);
            #endregion 字段属性.

            #region 把外键与枚举放入里面去.
            foreach (DataRow dr in sys_MapAttrs.Rows)
            {
                string uiBindKey = dr["UIBindKey"].ToString();
                string lgType = dr["LGType"].ToString();
                if (lgType != "2")
                    continue;

                string UIIsEnable = dr["UIVisible"].ToString();
                if (UIIsEnable == "0")
                    continue;

                if (string.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();


                // 判断是否存在.
                if (ds.Tables.Contains(uiBindKey) == true)
                    continue;

                ds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }

            string enumKeys = "";
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum)
                {
                    enumKeys += "'" + attr.UIBindKey + "',";
                }
            }

            if (enumKeys.Length > 2)
            {
                enumKeys = enumKeys.Substring(0, enumKeys.Length - 1);
                // Sys_Enum
                string sqlEnum = "SELECT * FROM Sys_Enum WHERE EnumKey IN (" + enumKeys + ")";
                DataTable dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                dtEnum.TableName = "Sys_Enum";
                ds.Tables.Add(dtEnum);
            }

            #endregion 把外键与枚举放入里面去.

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 实体的操作.
    }
}
