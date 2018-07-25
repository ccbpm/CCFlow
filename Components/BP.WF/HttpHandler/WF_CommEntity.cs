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

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CommEntity()
        {
        }

        #region 从表.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Dtl_Save()
        {
            try
            {
                #region  查询出来从表数据.
                Entities dtls = ClassFactory.GetEns(this.EnsName);
                Entity dtl = dtls.GetNewEntity;
                dtls.Retrieve(this.GetRequestVal("RefKey"), this.GetRequestVal("RefVal"));
                Map map = dtl.EnMap;
                foreach (Entity item in dtls)
                {
                    string pkval = item.GetValStringByKey(dtl.PK);
                    foreach (Attr attr in map.Attrs)
                    {
                        if (attr.IsRefAttr == true)
                            continue;

                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {
                            if (attr.UIIsReadonly == true)
                                continue;

                            string val = this.GetValFromFrmByKey("TB_" + pkval + "_" + attr.Key, null);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }


                        if (attr.UIContralType == UIContralType.TB && attr.UIIsReadonly == false)
                        {
                            string val = this.GetValFromFrmByKey("TB_" + pkval + "_" + attr.Key, null);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == true)
                        {
                            string val = this.GetValFromFrmByKey("DDL_" + pkval + "_" + attr.Key);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == true)
                        {
                            string val = this.GetValFromFrmByKey("CB_" + pkval + "_" + attr.Key, "-1");
                            if (val == "-1")
                                item.SetValByKey(attr.Key, 0);
                            else
                                item.SetValByKey(attr.Key, 1);
                            continue;
                        }
                    }

                    item.Update(); //执行更新.
                }
                #endregion  查询出来从表数据.

                #region 保存新加行.
                for (int i = 0; i < 50; i++)
                {
                    string pkval = "TB_"+i+"_"+dtl.PK;
                    var val=this.GetValFromFrmByKey(pkval, "");
                    if (val.Equals(""))
                        continue;

                    foreach (Attr attr in map.Attrs)
                    {

                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {
                            if (attr.UIIsReadonly == true)
                                continue;

                            val = this.GetValFromFrmByKey("TB_" + i + "_" + attr.Key, null);
                            dtl.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.TB && attr.UIIsReadonly == false)
                        {
                            val = this.GetValFromFrmByKey("TB_" + i + "_" + attr.Key);
                            dtl.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == true)
                        {
                            val = this.GetValFromFrmByKey("DDL_" + i + "_" + attr.Key);
                            dtl.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == true)
                        {
                            val = this.GetValFromFrmByKey("CB_" + i + "_" + attr.Key, "-1");
                            if (val == "-1")
                                dtl.SetValByKey(attr.Key, 0);
                            else
                                dtl.SetValByKey(attr.Key, 1);
                            continue;
                        }
                    }
                    dtl.SetValByKey(pkval, 0);
                    dtl.SetValByKey(this.GetRequestVal("RefKey"), this.GetRequestVal("RefVal"));
                    dtl.PKVal = "0";
                    dtl.Insert();
                }
                #endregion 保存新加行.

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string Dtl_Init()
        {
            //定义容器.
            DataSet ds = new DataSet();

            //查询出来从表数据.
            Entities dtls = ClassFactory.GetEns(this.EnsName);
            dtls.Retrieve(this.GetRequestVal("RefKey"), this.GetRequestVal("RefVal"));
            ds.Tables.Add(dtls.ToDataTableField("Dtls"));

            //实体.
            Entity dtl = dtls.GetNewEntity;
            //定义Sys_MapData.
            MapData md = new MapData();
            md.No = this.EnName;
            md.Name = dtl.EnDesc;

            #region 加入权限信息.
            //把权限加入参数里面.
            if (dtl.HisUAC.IsInsert)
                md.SetPara("IsInsert", "1");
            if (dtl.HisUAC.IsUpdate)
                md.SetPara("IsUpdate", "1");
            if (dtl.HisUAC.IsDelete)
                md.SetPara("IsDelete", "1");
            #endregion 加入权限信息.

            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            #region 字段属性.
            MapAttrs attrs = dtl.EnMap.Attrs.ToMapAttrs;
            DataTable sys_MapAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(sys_MapAttrs);
            #endregion 字段属性.

            #region 把外键与枚举放入里面去.
            foreach (DataRow dr in sys_MapAttrs.Rows)
            {
                string uiBindKey = dr["UIBindKey"].ToString();
                string lgType = dr["LGType"].ToString();
                if (lgType.Equals("2")==false)
                    continue;

                string UIIsEnable = dr["UIVisible"].ToString();
                if (UIIsEnable == "0")
                    continue;

                if (DataType.IsNullOrEmpty(uiBindKey) == true)
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
            foreach (Attr attr in dtl.EnMap.Attrs)
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

                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    dtEnum.Columns["MYPK"].ColumnName = "MyPK";
                    dtEnum.Columns["LAB"].ColumnName = "Lab";
                    dtEnum.Columns["ENUMKEY"].ColumnName = "EnumKey";
                    dtEnum.Columns["INTKEY"].ColumnName = "IntKey";
                    dtEnum.Columns["LANG"].ColumnName = "Lang";
                }
                ds.Tables.Add(dtEnum);
            }
            #endregion 把外键与枚举放入里面去.

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 从表.

        #region 实体的操作.
        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <returns></returns>
        public string EntityOnly_Init()
        {
            try
            {
                //是否是空白记录.
                bool isBlank = DataType.IsNullOrEmpty(this.PKVal);

                //初始化entity.
                string enName = this.EnName;
                Entity en = null;
                if (isBlank == true)
                {
                    if (DataType.IsNullOrEmpty(this.EnsName) == true)
                        return "err@类名没有传递过来";
                    Entities ens = ClassFactory.GetEns(this.EnsName);
                    en = ens.GetNewEntity;
                }
                else
                {
                    en = ClassFactory.GetEn(this.EnName);
                }

                if (en == null)
                    return "err@参数类名不正确.";

                //获得描述.
                Map map = en.EnMap;
                string pkVal = this.PKVal;
                if (isBlank == false)
                {
                    en.PKVal = pkVal;
                    int i = en.RetrieveFromDBSources();
                    if (i == 0)
                        return "err@数据[" + map.EnDesc + "]主键为[" + pkVal + "]不存在，或者没有保存。";
                }
                else
                {
                    foreach (Attr attr in en.EnMap.Attrs)
                        en.SetValByKey(attr.Key, attr.DefaultVal);

                    //设置默认的数据.
                    en.ResetDefaultVal();

                    en.SetValByKey("RefPKVal", this.RefPKVal);

                    //自动生成一个编号.
                    if (en.IsNoEntity == true && en.EnMap.IsAutoGenerNo == true)
                        en.SetValByKey("No", en.GenerNewNoByKey("No"));
                }


                //定义容器.
                DataSet ds = new DataSet();

                //定义Sys_MapData.
                MapData md = new MapData();
                md.No = this.EnName;
                md.Name = map.EnDesc;

                //附件类型.
                md.SetPara("BPEntityAthType", (int)map.HisBPEntityAthType);

                #region 加入权限信息.
                //把权限加入参数里面.
                if (en.HisUAC.IsInsert)
                    md.SetPara("IsInsert", "1");
                if (en.HisUAC.IsUpdate)
                    md.SetPara("IsUpdate", "1");
                if (isBlank == true)
                {                   
                    if (en.HisUAC.IsDelete)
                        md.SetPara("IsDelete", "0");
                }
                else
                {
                    if (en.HisUAC.IsDelete)
                        md.SetPara("IsDelete", "1");
                }
                #endregion 加入权限信息.


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
                        if (keyOfEn.Equals(field))
                        {
                            currGroupID = field;
                        }
                    }
                    drAttr[MapAttrAttr.GroupID] = currGroupID;
                }
                ds.Tables.Add(sys_MapAttrs);
                #endregion 字段属性.

                #region 加入扩展属性.
                MapExts mapExts = new MapExts(this.EnName+"s");
                DataTable Sys_MapExt = mapExts.ToDataTableField("Sys_MapExt");
                ds.Tables.Add(Sys_MapExt);
                #endregion 加入扩展属性.

                #region 把外键与枚举放入里面去.

                //加入外键.
                foreach (DataRow dr in sys_MapAttrs.Rows)
                {
                    string uiBindKey = dr["UIBindKey"].ToString();
                    string lgType = dr["LGType"].ToString();
                    if (lgType.Equals("2")==false)
                        continue;

                    string UIIsEnable = dr["UIVisible"].ToString();

                    if (UIIsEnable.Equals("0") == true)
                        continue;

                    if (DataType.IsNullOrEmpty(uiBindKey) == true)
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

                    DataTable dt = BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey);
                    dt.TableName = keyOfEn;

                    ds.Tables.Add(dt);
                }

                //加入sql模式的外键.
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.IsRefAttr == true)
                        continue;

                    if (DataType.IsNullOrEmpty(attr.UIBindKey) || attr.UIBindKey.Length <= 10)
                        continue;

                    if (attr.UIIsReadonly == true)
                        continue;

                    if (attr.UIBindKey.Contains("SELECT") == true || attr.UIBindKey.Contains("select") == true)
                    {
                        /*是一个sql*/
                        string sqlBindKey = attr.UIBindKey.Clone() as string;
                        sqlBindKey = BP.WF.Glo.DealExp(sqlBindKey, en, null);

                        DataTable dt = DBAccess.RunSQLReturnTable(sqlBindKey);
                        dt.TableName = attr.Key;

                        //@杜. 翻译当前部分.
                        if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                        }

                        ds.Tables.Add(dt);
                    }
                }

                //加入枚举的外键.
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

                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    {
                        dtEnum.Columns["MYPK"].ColumnName = "MyPK";
                        dtEnum.Columns["LAB"].ColumnName = "Lab";
                        dtEnum.Columns["ENUMKEY"].ColumnName = "EnumKey";
                        dtEnum.Columns["INTKEY"].ColumnName = "IntKey";
                        dtEnum.Columns["LANG"].ColumnName = "Lang";
                    }

                    ds.Tables.Add(dtEnum);
                }
                #endregion 把外键与枚举放入里面去.


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
                        myurl = "../RefMethod.htm?Index=" + item.Index + "&EnName=" + en.ToString() + "&EnsName=" + en.GetNewEntities.ToString() + "&PKVal=" + this.PKVal;
                    }

                    DataRow dr = dtM.NewRow();

                    dr["No"] = item.Index;
                    dr["Title"] = item.Title;
                    dr["Tip"] = item.ToolTip;
                    dr["Visable"] = item.Visable;
                    dr["Warning"] = item.Warning;

                    dr["RefMethodType"] = (int)item.RefMethodType;
                    dr["RefAttrKey"] = item.RefAttrKey;
                    dr["Url"] = myurl;
                    dr["W"] = item.Width;
                    dr["H"] = item.Height;
                    dr["Icon"] = item.Icon;
                    dr["IsCanBatch"] = item.IsCanBatch;
                    dr["GroupName"] = item.GroupName;

                    dtM.Rows.Add(dr); //增加到rows.
                }
                #endregion 增加 上方法.

                //增加方法。
                ds.Tables.Add(dtM);

                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <returns></returns>
        public string Entity_Init()
        {
            try
            {
                //是否是空白记录.
                bool isBlank = DataType.IsNullOrEmpty(this.PKVal);
                //if (DataType.IsNullOrEmpty(this.PKVal) == true)
                //    return "err@主键数据丢失，不能初始化En.htm";

                //初始化entity.
                string enName = this.EnName;
                Entity en = null;
                if (DataType.IsNullOrEmpty(enName) == true)
                {
                    if (DataType.IsNullOrEmpty(this.EnsName) == true)
                        return "err@类名没有传递过来";
                    Entities ens = ClassFactory.GetEns(this.EnsName);
                    en = ens.GetNewEntity;
                }
                else
                {
                    en = ClassFactory.GetEn(this.EnName);
                }

                if (en == null)
                    return "err@参数类名不正确.";

                //获得描述.
                Map map = en.EnMap;

                string pkVal = this.PKVal;

                if (isBlank == false)
                {
                    en.PKVal = pkVal;
                    en.RetrieveFromDBSources();
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

                //附件类型.
                md.SetPara("BPEntityAthType", (int)map.HisBPEntityAthType );

                if (isBlank == true)
                {
                    if (en.HisUAC.IsUpdate)
                        md.SetPara("IsUpdate", "0");
                    if (en.HisUAC.IsDelete)
                        md.SetPara("IsDelete", "0");
                }
                else
                {
                    if (en.HisUAC.IsUpdate)
                        md.SetPara("IsUpdate", "1");
                    if (en.HisUAC.IsDelete)
                        md.SetPara("IsDelete", "1");
                }
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
                //判断Func是否有参数
                dtM.Columns.Add("FunPara");
               

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
                        myurl = "../RefMethod.htm?Index=" + item.Index + "&EnName=" + en.ToString() + "&EnsName=" + en.GetNewEntities.ToString() + "&PKVal=" + this.PKVal;
                    }

                    DataRow dr = dtM.NewRow();

                    dr["No"] = item.Index;
                    dr["Title"] = item.Title;
                    dr["Tip"] = item.ToolTip;
                    dr["Visable"] = item.Visable;
                    dr["Warning"] = item.Warning;


                    dr["RefMethodType"] = (int)item.RefMethodType;
                    dr["RefAttrKey"] = item.RefAttrKey;
                    dr["Url"] = myurl;
                    dr["W"] = item.Width;
                    dr["H"] = item.Height;
                    dr["Icon"] = item.Icon;
                    dr["IsCanBatch"] = item.IsCanBatch;
                    dr["GroupName"] = item.GroupName;
                    Attrs attrs = item.HisAttrs;
                    if(attrs.Count ==0)
                        dr["FunPara"] = "false";
                    else
                        dr["FunPara"] = "true";

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
                        dr["No"] = enMM.ToString();
                        // dr["GroupName"] = vsM.GroupName;
                        if (en.PKVal != null)
                        {
                            //判断模式.
                            string url = "";
                            if (vsM.Dot2DotModel == Dot2DotModel.TreeDept)
                            {
                                //url = "Dot2DotTreeDeptModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString();
                                //  url = "Branches.htm?EnName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString();

                                url = "Branches.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString();
                                // url += "&PKVal=" + en.PKVal;
                                url += "&Dot2DotEnName=" + vsM.EnsOfMM.GetNewEntity.ToString(); //存储实体类.
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM; //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体分组依据.  比如:FK_Station.
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  

                            }
                            else if (vsM.Dot2DotModel == Dot2DotModel.TreeDeptEmp)
                            {
                                //   url = "Dot2DotTreeDeptEmpModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString();
                                // url = "Dot2Dot.aspx?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString();
                                url = "BranchesAndLeaf.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString();
                                //   url += "&PKVal=" + en.PKVal;
                                url += "&Dot2DotEnName=" + vsM.EnsOfMM.GetNewEntity.ToString(); //存储实体类.
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM; //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体分组依据.  比如:FK_Station.
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  比如:FK_Station.
                                //url += "&RootNo=" + vsM.RootNo; //默认的B实体分组依据.  比如:FK_Station.
                            }
                            else
                            {
                                // url = "Dot2Dot.aspx?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString();
                                url = "Dot2Dot.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString(); //比如:BP.WF.Template.NodeStations
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM;  //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体.   //比如:BP.Port.Stations
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  比如:FK_Station.

                                //+"&RefAttrEnsName=" + vsM.EnsOfM.ToString();
                                //url += "&RefAttrKey=" + vsM.AttrOfOneInMM + "&RefAttrEnsName=" + vsM.EnsOfM.ToString();
                            }

                            dr["Url"] = url + "&" + en.PK + "=" + en.PKVal + "&PKVal=" + en.PKVal;
                            dr["Icon"] = "../Img/M2M.png";

                        }

                        dr["W"] = "900";
                        dr["H"] = "500";
                        dr["RefMethodType"] = (int)RefMethodType.RightFrameOpen;


                        // 获得选择的数量.
                        try
                        {
                            sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                            i = DBAccess.RunSQLReturnValInt(sql);
                        }
                        catch
                        {
                            sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal;
                            try
                            {
                                i = DBAccess.RunSQLReturnValInt(sql);
                            }
                            catch
                            {
                                vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                            }
                        }
                        dr["Title"] = vsM.Desc + "(" + i + ")";
                        dtM.Rows.Add(dr);
                    }
                }
                #endregion 增加 一对多.

                #region 从表
                EnDtls enDtls = en.EnMap.Dtls;
                foreach (EnDtl enDtl in enDtls)
                {
                    //判断该dtl是否要显示?
                    Entity myEnDtl = enDtl.Ens.GetNewEntity; //获取他的en
                    myEnDtl.SetValByKey(enDtl.RefKey, this.PKVal);  //给refpk赋值
                    if (myEnDtl.HisUAC.IsView == false)
                        continue;

                    DataRow dr = dtM.NewRow();
                    //string url = "Dtl.aspx?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() ;
                    string url = "Dtl.htm?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                        }
                        catch
                        {
                            enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                        }
                    }

                    dr["No"] = enDtl.EnsName;
                    dr["Title"] = enDtl.Desc + "(" + i + ")";
                    dr["Url"] = url;
                    dr["GroupName"] = enDtl.GroupName;

                    dr["RefMethodType"] = (int)RefMethodType.RightFrameOpen;

                    dtM.Rows.Add(dr);
                }
                #endregion 增加 从表.

                ds.Tables.Add(dtM); //


                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 实体的操作.

        #region 部门人员模式.
        public string BranchesAndLeaf_SearchByNodeID()
        {
            string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");
            string key = this.GetRequestVal("Key"); //查询关键字.

            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            QueryObject qo = new QueryObject(ensMen); //集合.
            qo.AddWhere(defaultGroupAttrKey, key);
            qo.DoQuery();


            return ensMen.ToJson();
        }
        public string BranchesAndLeaf_SearchByKey()
        {
            string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");

            string key = this.GetRequestVal("Key"); //查询关键字.

            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            QueryObject qo = new QueryObject(ensMen); //集合.
            qo.AddWhere("No", " LIKE ", "%" + key + "%");
            qo.addOr();
            qo.AddWhere("Name", " LIKE ", "%" + key + "%");
            qo.DoQuery();

            return ensMen.ToJson();
        }
        public string BranchesAndLeaf_Delete()
        {
            try
            {
                string dot2DotEnName = this.GetRequestVal("Dot2DotEnName");
                string AttrOfOneInMM = this.GetRequestVal("AttrOfOneInMM");
                string AttrOfMInMM = this.GetRequestVal("AttrOfMInMM");
                Entity mm = ClassFactory.GetEn(dot2DotEnName);
                mm.Delete(AttrOfOneInMM, this.PKVal, AttrOfMInMM, this.GetRequestVal("Key"));
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string BranchesAndLeaf_Init()
        {
            string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");

            //string enName = this.GetRequestVal("EnName");
            Entity en = ClassFactory.GetEn(this.EnName);
            en.PKVal = this.PKVal;
            en.Retrieve();

            //找到映射.
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            AttrOfOneVSM vsM = null;
            foreach (AttrOfOneVSM item in oneVsM)
            {
                if (item.Dot2DotModel == Dot2DotModel.TreeDeptEmp
                    && item.EnsOfMM.ToString().Equals(dot2DotEnsName)
                    && item.DefaultGroupAttrKey.Equals(defaultGroupAttrKey))
                {
                    vsM = item;
                    break;
                }
            }
            if (vsM == null)
                return "err@参数错误,没有找到VSM";

            //组织数据.
            DataSet ds = new DataSet();
            string rootNo = vsM.RootNo;
            if (rootNo.Equals("@WebUser.FK_Dept") || rootNo.Equals("WebUser.FK_Dept"))
                rootNo = WebUser.FK_Dept;

            #region 生成树目录.
            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            Entity enMen = ensMen.GetNewEntity;

            Attr attr = enMen.EnMap.GetAttrByKey(defaultGroupAttrKey);
            if (attr == null)
                return "err@在实体[" + ensOfM + "]指定的分树的属性[" + defaultGroupAttrKey + "]不存在，请确认是否删除了该属性?";

            if (attr.MyFieldType == FieldType.Normal)
                return "err@在实体[" + ensOfM + "]指定的分树的属性[" + defaultGroupAttrKey + "]不能是普通字段，必须是外键或者枚举.";

            Entities trees = attr.HisFKEns;
            trees.RetrieveAll();

            DataTable dt = trees.ToDataTableField("DBTrees");
            //如果没有parnetNo 列，就增加上, 有可能是分组显示使用这个模式.
            if (dt.Columns.Contains("ParentNo") == false)
            {
                dt.Columns.Add("ParentNo");
                foreach (DataRow dr in dt.Rows)
                    dr["ParentNo"] = rootNo;
            }
            ds.Tables.Add(dt);
            #endregion 生成树目录.

            #region 生成选择的数据.
            Entities dot2Dots = ClassFactory.GetEns(dot2DotEnsName);
            dot2Dots.Retrieve(vsM.AttrOfOneInMM, this.PKVal);

            DataTable dtSelected = dot2Dots.ToDataTableField("DBMMs");

            string attrOfMInMM = this.GetRequestVal("AttrOfMInMM");
            string AttrOfOneInMM = this.GetRequestVal("AttrOfOneInMM");

            dtSelected.Columns[attrOfMInMM].ColumnName = "No";

            if (dtSelected.Columns.Contains(attrOfMInMM + "Text") == false)
                return "err@MM实体类字段属性需要按照外键属性编写:" + dot2DotEnsName + " - " + attrOfMInMM;

            dtSelected.Columns[attrOfMInMM + "Text"].ColumnName = "Name";

            dtSelected.Columns.Remove(AttrOfOneInMM);
            ds.Tables.Add(dtSelected); //已经选择的数据.
            #endregion 生成选择的数据.

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 部门人员模式.

        #region 分组数据.
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string Dot2Dot_Save()
        {

            try
            {
                string eles = this.GetRequestVal("ElesAAA");

                //实体集合.
                string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
                string attrOfOneInMM = this.GetRequestVal("AttrOfOneInMM");
                string attrOfMInMM = this.GetRequestVal("AttrOfMInMM");

                //获得点对点的实体.
                Entity en = ClassFactory.GetEns(dot2DotEnsName).GetNewEntity;
                en.Delete(attrOfOneInMM, this.PKVal); //首先删除.

                string[] strs = eles.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    en.SetValByKey(attrOfOneInMM, this.PKVal);
                    en.SetValByKey(attrOfMInMM, str);
                    en.Insert();
                }
                return "数据保存成功.";
            }
            catch (Exception ex)
            {
                return "err@"+ex.Message;
            }
        }
        /// <summary>
        /// 获得分组的数据源
        /// </summary>
        /// <returns></returns>
        public string Dot2Dot_GenerGroupEntitis()
        {
            string key = this.GetRequestVal("DefaultGroupAttrKey");

            //实体集合.
            string ensName = this.GetRequestVal("EnsOfM");
            Entities ens = ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;

            Attrs attrs = en.EnMap.Attrs;
            Attr attr = attrs.GetAttrByKey(key);

            if (attr == null)
                return "err@设置的分组外键错误[" + key + "],不存在[" + ensName + "]或者已经被删除.";

            if (attr.MyFieldType == FieldType.Normal)
                return "err@设置的默认分组["+key+"]不能是普通字段.";

            if (attr.MyFieldType == FieldType.FK)
            {
                Entities ensFK = attr.HisFKEns;
                ensFK.Clear();
                ensFK.RetrieveAll();
                return ensFK.ToJson();
            }

            if (attr.MyFieldType == FieldType.Enum)
            {
                /* 如果是枚举 */
                SysEnums ses = new SysEnums();
                ses.Retrieve(SysEnumAttr.IntKey, attr.UIBindKey);

                //ses.ToStringOfSQLModelByKey

                BP.Pub.NYs nys = new Pub.NYs();
                foreach (SysEnum item in ses)
                {
                    BP.Pub.NY ny =new Pub.NY();
                    ny.No = item.IntKey.ToString();
                    ny.Name = item.Lab;
                    nys.AddEntity(ny);
                }
                return nys.ToJson();
            }

            return "err@设置的默认分组[" + key + "]不能是普通字段.";
        }
        #endregion 分组数据.


    }
}
