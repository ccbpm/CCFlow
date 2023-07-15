using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using ICSharpCode.SharpZipLib.Zip;
using BP.Difference;
using System.IO;
using System.Web;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CommEntity : DirectoryPageBase
    {

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

                        if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == false)
                        {
                            string val = this.GetValFromFrmByKey("DDL_" + pkval + "_" + attr.Key);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == false)
                        {
                            string val = this.GetValFromFrmByKey("CB_" + pkval + "_" + attr.Key, "-1");
                            if (val == "-1")
                                item.SetValByKey(attr.Key, 0);
                            else
                                item.SetValByKey(attr.Key, val);
                            continue;
                        }
                    }

                    item.Update(); //执行更新.
                }
                #endregion  查询出来从表数据.

                #region 保存新加行.
                int newRowCount = this.GetRequestValInt("NewRowCount");
                bool isEntityOID = dtl.IsOIDEntity;  //已同步数据.
                bool isEntityNo = dtl.IsNoEntity;
                for (int i = 0; i < newRowCount; i++)
                {
                    string val = "";
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
                            if (attr.IsNum && val == "")
                                val = "0";
                            dtl.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == false)
                        {
                            val = this.GetValFromFrmByKey("DDL_" + i + "_" + attr.Key);
                            dtl.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == false)
                        {
                            val = this.GetValFromFrmByKey("CB_" + i + "_" + attr.Key, "-1");
                            if (val == "-1")
                                dtl.SetValByKey(attr.Key, 0);
                            else
                                dtl.SetValByKey(attr.Key, val);
                            continue;
                        }
                    }
                    //dtl.SetValByKey(pkval, 0);
                    dtl.SetValByKey(this.GetRequestVal("RefKey"), this.GetRequestVal("RefVal"));

                    //已同步数据.
                    if (isEntityOID == true)
                    {
                        dtl.PKVal = "0";
                        dtl.Insert();
                        continue;
                    }

                    if (isEntityNo == true && dtl.EnMap.IsAutoGenerNo == true)
                    {
                        dtl.PKVal = dtl.GenerNewNoByKey("No");
                        dtl.Insert();
                        continue;
                    }

                    //直接执行保存.
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
            if (dtl.HisUAC.IsImp)
                md.SetPara("IsImp", "1");
            if (dtl.HisUAC.IsExp)
                md.SetPara("IsImp", "1");
            md.SetPara("EntityPK", dtl.PKField);

            #endregion 加入权限信息.

            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            #region 字段属性.
            MapAttrs attrs = dtl.EnMap.Attrs.ToMapAttrs;
            DataTable sys_MapAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(sys_MapAttrs);
            #endregion 字段属性.

            #region 把外键与枚举放入里面去.
            foreach (MapAttr mapAttr in attrs)
            {
                string uiBindKey = mapAttr.UIBindKey;
                if (mapAttr.LGType != FieldTypeS.FK)
                    continue;
                if (mapAttr.UIIsEnable == false)
                    continue;

                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue;
                // 判断是否存在.
                if (ds.Tables.Contains(uiBindKey) == true)
                    continue;

                ds.Tables.Add(BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }

            foreach (Attr attr in dtl.EnMap.Attrs)
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

                    // 判断是否存在.
                    if (ds.Tables.Contains(sqlBindKey) == true)
                        continue;

                    sqlBindKey = BP.WF.Glo.DealExp(sqlBindKey, null, null);

                    DataTable dt = DBAccess.RunSQLReturnTable(sqlBindKey);
                    dt.TableName = attr.Key;

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            string colName = col.ColumnName.ToLower();
                            switch (colName)
                            {
                                case "no":
                                    col.ColumnName = "No";
                                    break;
                                case "name":
                                    col.ColumnName = "Name";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    ds.Tables.Add(dt);
                }
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
                string sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ")";
                DataTable dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                dtEnum.TableName = "Sys_Enum";
                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                {
                    foreach (DataColumn col in dtEnum.Columns)
                    {
                        string colName = col.ColumnName.ToLower();
                        switch (colName)
                        {
                            case "mypk":
                                col.ColumnName = "MyPK";
                                break;
                            case "lab":
                                col.ColumnName = "Lab";
                                break;
                            case "enumkey":
                                col.ColumnName = "EnumKey";
                                break;
                            case "intkey":
                                col.ColumnName = "IntKey";
                                break;
                            case "lang":
                                col.ColumnName = "Lang";
                                break;
                            default:
                                break;
                        }
                    }
                }
                ds.Tables.Add(dtEnum);
            }
            #endregion 把外键与枚举放入里面去.

            return BP.Tools.Json.ToJson(ds);
        }

        public string Dtl_Exp()
        {
            string refPKVal = this.GetRequestVal("RefVal");
            Entities dtls = ClassFactory.GetEns(this.EnsName);
            dtls.Retrieve(this.GetRequestVal("RefKey"), this.GetRequestVal("RefVal"));
            Entity en = dtls.GetNewEntity;
            string name = "数据导出";
            if (refPKVal.Contains("/") == true)
                refPKVal = refPKVal.Replace("/", "_");
            string filename = refPKVal + "_" + en.ToString() + "_" + DataType.CurrentDate + "_" + name + ".xls";
            string filePath = BP.Tools.ExportExcelUtil.ExportDGToExcel(dtls.ToDataTableField(), en, name, null, filename);

            filePath = BP.Difference.SystemConfig.PathOfTemp + filename;

            string tempPath = BP.Difference.SystemConfig.PathOfTemp + refPKVal + "/";
            if (System.IO.Directory.Exists(tempPath) == false)
                System.IO.Directory.CreateDirectory(tempPath);

            string myFilePath = BP.Difference.SystemConfig.PathOfDataUser + this.EnsName.Substring(0, this.EnsName.Length - 1);

            foreach (Entity dt in dtls)
            {
                string pkval = dt.PKVal.ToString();
                string ext = string.IsNullOrWhiteSpace(dt.GetValByKey("MyFileExt") as string) ? "" : dt.GetValByKey("MyFileExt").ToString();
                if (DataType.IsNullOrEmpty(ext) == true)
                    continue;
                myFilePath = myFilePath + "/" + pkval + "." + ext;
                if (System.IO.File.Exists(myFilePath) == true)
                    System.IO.File.Copy(myFilePath, tempPath + pkval + "." + ext, true);
            }
            System.IO.File.Copy(filePath, tempPath + filename, true);

            //生成压缩文件
            string zipFile = BP.Difference.SystemConfig.PathOfTemp + refPKVal + "_" + en.ToString() + "_" + DataType.CurrentDate + "_" + name + ".zip";

            System.IO.FileInfo finfo = new System.IO.FileInfo(zipFile);

            (new FastZip()).CreateZip(finfo.FullName, tempPath, true, "");

            return "/DataUser/Temp/" + refPKVal + "_" + en.ToString() + "_" + DataType.CurrentDate + "_" + name + ".zip";
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

                    if (ens == null)
                        return "err@类名错误" + this.EnsName;  

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

                //多附件上传
                if ((int)map.HisBPEntityAthType == 2)
                {
                    //增加附件分类
                    DataTable attrFiles = new DataTable("AttrFiles");
                    attrFiles.Columns.Add("FileNo");
                    attrFiles.Columns.Add("FileName");
                    foreach (AttrFile attrFile in map.HisAttrFiles)
                    {
                        DataRow dr = attrFiles.NewRow();
                        dr["FileNo"] = attrFile.FileNo;
                        dr["FileName"] = attrFile.FileName;
                        attrFiles.Rows.Add(dr);
                    }
                    ds.Tables.Add(attrFiles);

                    //增加附件列表
                    SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
                    ds.Tables.Add(sfs.ToDataTableField("Sys_FileManager"));
                }

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

                Attrs attrs = en.EnMap.Attrs;
                MapAttrs mapAttrs = new MapAttrs();
                //获取Map中的分组
                DataTable dtGroups = new DataTable("Sys_GroupField");
                dtGroups.Columns.Add("OID");
                dtGroups.Columns.Add("Lab");
                string groupName = "";
                string groupNames = "";
                foreach(Attr attr in attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;
                    groupName = attr.GroupName;
                    if (groupNames.Contains(groupName + ",") == false)
                    {
                        DataRow dr = dtGroups.NewRow();
                        groupNames += groupName + ",";
                        dr["OID"] = groupName;
                        dr["Lab"] = groupName;
                        dtGroups.Rows.Add(dr);
                    }
                   
                    MapAttr mapAttr = attr.ToMapAttr;
                    mapAttr.SetPara("GroupName", attr.GroupName);
                    mapAttrs.AddEntity(mapAttr);
                }
                ds.Tables.Add(dtGroups);
                DataTable sys_MapAttrs = mapAttrs.ToDataTableField("Sys_MapAttr");
                ds.Tables.Add(sys_MapAttrs);



                #region 加入扩展属性.
                MapExts mapExts = new MapExts(this.EnName + "s");
                DataTable Sys_MapExt = mapExts.ToDataTableField("Sys_MapExt");
                ds.Tables.Add(Sys_MapExt);
                #endregion 加入扩展属性.

                #region 把外键与枚举放入里面去.

                //加入外键.
                foreach (DataRow dr in sys_MapAttrs.Rows)
                {
                    string uiBindKey = dr["UIBindKey"].ToString();
                    string lgType = dr["LGType"].ToString();
                    if (lgType.Equals("2") == false)
                        continue;

                    string UIVisible = dr["UIVisible"].ToString();
                    string uiIsEnable = dr["UIIsEnable"].ToString();


                    if (UIVisible.Equals("0") == true || uiIsEnable.Equals("0") == true)
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

                    DataTable dt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
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

                    if (attr.UIBindKey.ToUpper().Contains("SELECT") == true)
                    {
                        /*是一个sql*/
                        string sqlBindKey = attr.UIBindKey.Clone() as string;
                        sqlBindKey = BP.WF.Glo.DealExp(sqlBindKey, en, null);

                        DataTable dt = DBAccess.RunSQLReturnTable(sqlBindKey);
                        dt.TableName = attr.Key;

                        //@杜. 翻译当前部分.
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                        }
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                        {
                            dt.Columns["no"].ColumnName = "No";
                            dt.Columns["name"].ColumnName = "Name";
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
                    DataTable dtEnum = new DataTable();
                    string sqlEnum = "";
                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    {
                        string sqlWhere = " EnumKey IN (" + enumKeys + ") AND OrgNo='" + WebUser.OrgNo + "'";

                        sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE " + sqlWhere;
                        sqlEnum += " UNION ";
                        sqlEnum += "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ") AND EnumKey NOT IN (SELECT EnumKey FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE " + sqlWhere + ") AND (OrgNo Is Null Or OrgNo='')";
                        dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                    }
                    else
                    {
                        sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ")";
                        dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                    }
                    dtEnum.TableName = "Sys_Enum";

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                    {
                        dtEnum.Columns["MYPK"].ColumnName = "MyPK";
                        dtEnum.Columns["LAB"].ColumnName = "Lab";
                        dtEnum.Columns["ENUMKEY"].ColumnName = "EnumKey";
                        dtEnum.Columns["INTKEY"].ColumnName = "IntKey";
                        dtEnum.Columns["LANG"].ColumnName = "Lang";
                    }

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                    {
                        dtEnum.Columns["mypk"].ColumnName = "MyPK";
                        dtEnum.Columns["lab"].ColumnName = "Lab";
                        dtEnum.Columns["enumkey"].ColumnName = "EnumKey";
                        dtEnum.Columns["intkey"].ColumnName = "IntKey";
                        dtEnum.Columns["lang"].ColumnName = "Lang";
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
                    item.HisEn = en;
                    //item.HisAttrs = en.EnMap.Attrs;B
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
                //增加方法。
                ds.Tables.Add(dtM);
                #endregion 增加 上方法.
                #region 从表
                DataTable dtl = new DataTable("Dtl");
                dtl.Columns.Add("No");
                dtl.Columns.Add("Title");
                dtl.Columns.Add("Url");
                dtl.Columns.Add("GroupName");
                EnDtls enDtls = en.EnMapInTime.Dtls;
                foreach (EnDtl enDtl in enDtls)
                {
                    if (enDtl.DtlEditerModel == DtlEditerModel.DtlBatch 
                        || enDtl.DtlEditerModel == DtlEditerModel.DtlSearch
                        || enDtl.DtlEditerModel == DtlEditerModel.DtlURL)
                        continue;
                    //判断该dtl是否要显示?
                    string url = "";
                    if(enDtl.DtlEditerModel != DtlEditerModel.DtlURLEnonly)
                    {
                        Entity myEnDtl = enDtl.Ens.GetNewEntity; //获取他的en
                        myEnDtl.SetValByKey(enDtl.RefKey, this.PKVal);  //给refpk赋值.
                        if (myEnDtl.HisUAC.IsView == false)
                            continue;
                        if (enDtl.DtlEditerModel == DtlEditerModel.DtlBatch)
                            url = "DtlBatch.htm?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                        else
                            url = "DtlSearch.htm?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    }
                    else
                    {
                        url = enDtl.UrlExt;
                        url = BP.WF.Glo.DealExp(url, en);
                    }
                    DataRow dr = dtl.NewRow();
                    dr["No"] = enDtl.DtlEditerModel == DtlEditerModel.DtlURLEnonly? enDtl.Desc:enDtl.EnsName;
                    dr["Title"] = enDtl.Desc;
                    dr["Url"] = url;
                    dr["GroupName"] = enDtl.GroupName;
                    dtl.Rows.Add(dr);
                }
                #endregion 增加 从表.

                ds.Tables.Add(dtl);

                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 实体Entity 单文件上传
        /// </summary>
        /// <returns></returns>
        public string EntityAth_Upload()
        {
            var files = HttpContextHelper.RequestFiles();
            if (files.Count == 0)
                return "err@请选择要上传的文件。";
            //获取保存文件信息的实体

            string enName = this.EnName;
            Entity en = null;

            //是否是空白记录.
            bool isBlank = DataType.IsNullOrEmpty(this.PKVal);
            if (isBlank == true)
                return "err@请先保存实体信息然后再上传文件";
            else
                en = ClassFactory.GetEn(this.EnName);

            if (en == null)
                return "err@参数类名不正确.";
            en.PKVal = this.PKVal;
            int i = en.RetrieveFromDBSources();
            if (i == 0)
                return "err@数据[" + this.EnName + "]主键为[" + en.PKVal + "]不存在，或者没有保存。";

            //获取文件的名称
            string fileName = files[0].FileName;
            if (fileName.IndexOf("/") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            //文件后缀
            string ext = System.IO.Path.GetExtension(files[0].FileName);
            ext = ext.Replace(".", ""); //去掉点 @李国文

            //文件大小
            float size = HttpContextHelper.RequestFileLength(files[0]) / 1024;

            //保存位置
            string filepath = "";
            string relativePath = "";


            //如果是天业集团则保存在ftp服务器上
            if (BP.Difference.SystemConfig.CustomerNo.Equals("TianYe") || BP.Difference.SystemConfig.IsUploadFileToFTP == true)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp = BP.Difference.SystemConfig.PathOfTemp + "" + guid + ".tmp";
                try
                {
                    //files[0].SaveAs(temp);
                    HttpContextHelper.UploadFile(files[0], temp);
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(temp);
                    //files[0].SaveAs(temp);
                    HttpContextHelper.UploadFile(files[0], temp);
                }

                /*保存到fpt服务器上.*/
                FtpConnection ftpconn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                    SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

                if (ftpconn == null)
                    return "err@FTP服务器连接失败";

                string ny = DateTime.Now.ToString("yyyy_MM");

                //判断目录年月是否存在.
                if (ftpconn.DirectoryExist(ny) == false)
                    ftpconn.CreateDirectory(ny);
                ftpconn.SetCurrentDirectory(ny);

                //判断目录是否存在.
                if (ftpconn.DirectoryExist("Helper") == false)
                    ftpconn.CreateDirectory("Helper");

                //设置当前目录，为操作的目录。
                ftpconn.SetCurrentDirectory("Helper");

                //把文件放上去.
                ftpconn.PutFile(temp, guid + "." + ext);
                ftpconn.Close();

                //删除临时文件
                System.IO.File.Delete(temp);

                //设置路径.
                filepath = ny + "/Helper/" + guid + "." + ext;

            }
            else
            {
                string fileSavePath = en.EnMap.FJSavePath;

                if (DataType.IsNullOrEmpty(fileSavePath) == true)
                    fileSavePath = BP.Difference.SystemConfig.PathOfDataUser + enName;

                if (System.IO.Directory.Exists(fileSavePath) == false)
                    System.IO.Directory.CreateDirectory(fileSavePath);

                filepath = fileSavePath + "/" + this.PKVal + "." + ext;
                relativePath = "/DataUser/" + enName + '/' + this.PKVal + "." + ext;
                //存在文件则删除
                if (System.IO.File.Exists(filepath) == true)
                    System.IO.File.Delete(filepath);

                FileInfo info = new FileInfo(filepath);
                //files[0].SaveAs(filepath);
                HttpContextHelper.UploadFile(files[0], filepath);
            }

            //需要这样写 @李国文.
            en.SetValByKey("MyFileName", fileName);
            en.SetValByKey("MyFilePath", filepath);
            en.SetValByKey("MyFileExt", ext);
            en.SetValByKey("MyFileSize", size);
            en.SetValByKey("WebPath", relativePath);

            en.Update();
            return "文件保存成功";
        }

        /// <summary>
        /// 实体单附件删除
        /// </summary>
        /// <returns></returns>
        public string EntityAth_Delete()
        {
            string pkval = this.PKVal;
            string enName = this.EnName;
            if(DataType.IsNullOrEmpty(pkval) || DataType.IsNullOrEmpty(enName))
            {
                return "err@删除实体附件需要传入PKVal和EnName";
            }
            Entity en = ClassFactory.GetEn(this.EnName);
            en.PKVal = pkval;
            en.RetrieveFromDBSources();
                
            string filePath = en.GetValStringByKey("MyFilePath");
            try
            {
                System.IO.File.Delete(filePath);
                return "附件删除成功";
            }
            catch (Exception e)
            {
                string errMsg = "err@删除失败," + "filePath: " + filePath + e.ToString();
                BP.DA.Log.DebugWriteError(errMsg);
                return errMsg;
            }

        }
        public void EntityFile_Load()
        {
            //根据EnsName获取Entity
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            en.PKVal = this.GetRequestVal("DelPKVal");
            int i = en.RetrieveFromDBSources();
            if (i == 0)
                return;
            string filePath = en.GetValStringByKey("MyFilePath");
            string fileName = en.GetValStringByKey("MyFileName");
            string fileExt = en.GetValStringByKey("MyFileExt");
            //获取使用的客存在FTP服务器上
            if ( BP.Difference.SystemConfig.IsUploadFileToFTP == true)
            {
               
                //临时存储位置
                string tempFile = BP.Difference.SystemConfig.PathOfTemp + System.Guid.NewGuid() + "." + en.GetValByKey("MyFileExt");

                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //连接FTP服务器
                FtpConnection conn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                    BP.Difference.SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);
                conn.GetFile(filePath, tempFile, false, System.IO.FileAttributes.Archive);
                conn.Close();

                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(tempFile, fileName + "." + fileExt);
                //删除临时文件
                System.IO.File.Delete(tempFile);
            }
            else
            {
                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(filePath, fileName + "." + fileExt);
            }
        }

        /// <summary>
        /// 实体多附件上传
        /// </summary>
        /// <returns></returns>
        public string EntityMultiAth_Upload()
        {
            //HttpFileCollection files = context.Request.Files;
            var files = HttpContextHelper.RequestFiles();
            if (files.Count == 0)
                return "err@请选择要上传的文件。";
            //获取保存文件信息的实体

            string enName = this.EnName;
            Entity en = null;

            //是否是空白记录.
            bool isBlank = DataType.IsNullOrEmpty(this.PKVal);
            if (isBlank == true)
                return "err@请先保存实体信息然后再上传文件";
            else
                en = ClassFactory.GetEn(this.EnName);

            if (en == null)
                return "err@参数类名不正确.";
            en.PKVal = this.PKVal;
            int i = en.RetrieveFromDBSources();
            if (i == 0)
                return "err@数据[" + this.EnName + "]主键为[" + en.PKVal + "]不存在，或者没有保存。";

            //获取文件的名称
            string fileName = files[0].FileName;
            if (fileName.IndexOf("/") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

            SysFileManagers fileManagers = new SysFileManagers();
            fileManagers.Retrieve(SysFileManagerAttr.RefVal, this.PKVal, SysFileManagerAttr.MyFileName, fileName);
            if (fileManagers.Count != 0)
                return "err@文件" + fileName + "已经存在";
            //文件后缀
            string ext = System.IO.Path.GetExtension(files[0].FileName);

            //文件大小
            float size = HttpContextHelper.RequestFileLength(files[0]) / 1024;

            //保存位置
            string filepath = "";

            //如果是天业集团则保存在ftp服务器上
            if (BP.Difference.SystemConfig.CustomerNo.Equals("TianYe") || BP.Difference.SystemConfig.IsUploadFileToFTP == true)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp = BP.Difference.SystemConfig.PathOfTemp + "" + guid + ".tmp";
                try
                {
                    //files[0].SaveAs(temp);
                    HttpContextHelper.UploadFile(files[0], temp);
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(temp);
                    //files[0].SaveAs(temp);
                    HttpContextHelper.UploadFile(files[0], temp);
                }

                /*保存到fpt服务器上.*/
                FtpConnection ftpconn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                    SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

                if (ftpconn == null)
                    return "err@FTP服务器连接失败";

                string ny = DateTime.Now.ToString("yyyy_MM");

                //判断目录年月是否存在.
                if (ftpconn.DirectoryExist(ny) == false)
                    ftpconn.CreateDirectory(ny);
                ftpconn.SetCurrentDirectory(ny);

                //判断目录是否存在.
                if (ftpconn.DirectoryExist("Helper") == false)
                    ftpconn.CreateDirectory("Helper");

                //设置当前目录，为操作的目录。
                ftpconn.SetCurrentDirectory("Helper");

                //把文件放上去.
                ftpconn.PutFile(temp, guid + ext);
                ftpconn.Close();

                //删除临时文件
                System.IO.File.Delete(temp);

                //设置路径.
                filepath = ny + "/Helper/" + guid + ext;

            }
            else
            {

                string savePath = BP.Difference.SystemConfig.PathOfDataUser + enName + "/" + this.PKVal;

                if (System.IO.Directory.Exists(savePath) == false)
                    System.IO.Directory.CreateDirectory(savePath);
                savePath = savePath + "/" + fileName + ext;
                //存在文件则删除
                if (System.IO.Directory.Exists(savePath) == true)
                    System.IO.Directory.Delete(savePath);

                FileInfo info = new FileInfo(savePath);

                //files[0].SaveAs(filepath);
                HttpContextHelper.UploadFile(files[0], savePath);
                filepath = "/DataUser/" + enName + "/" + this.PKVal + "/" + fileName + ext;
            }
            //保存上传的文件
            SysFileManager fileManager = new SysFileManager();
            fileManager.AttrFileNo = this.GetRequestVal("FileNo");
            fileManager.AttrFileName = HttpUtility.UrlDecode(this.GetRequestVal("FileName"), System.Text.Encoding.UTF8);
            fileManager.EnName = this.EnName;
            fileManager.RefVal = this.PKVal;
            fileManager.MyFileName = fileName;
            fileManager.MyFilePath = filepath;
            fileManager.MyFileExt = ext;
            fileManager.MyFileSize = size;
            fileManager.WebPath = filepath;
            fileManager.Insert();
            return fileManager.ToJson();
        }

        public void EntityMutliFile_Load()
        {
            string oid = this.GetRequestVal("OID");
            //根据SysFileManager的OID获取对应的实体
            SysFileManager fileManager = new SysFileManager();
            fileManager.PKVal = oid;
            int i = fileManager.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("没有找到OID=" + oid + "的文件管理数据，请联系管理员");

            //获取使用的客户保存在FTP服务器上
            if (BP.Difference.SystemConfig.IsUploadFileToFTP == true)
            {
                string filePath = fileManager.MyFilePath;
                string fileName = fileManager.MyFileName;
                //临时存储位置
                string tempFile = BP.Difference.SystemConfig.PathOfTemp + System.Guid.NewGuid() + "." + fileManager.MyFileExt;
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);
                //连接FTP服务器
                FtpConnection conn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                    BP.Difference.SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);
                conn.GetFile(filePath, tempFile, false, System.IO.FileAttributes.Archive);
                conn.Close();

                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(tempFile, fileName);
                //删除临时文件
                System.IO.File.Delete(tempFile);
            }
            else
            {
                BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(fileManager.MyFilePath, fileManager.MyFileName+"."+ fileManager.MyFileExt);
            }
        }
        /// <summary>
        /// 删除实体多附件上传的信息
        /// </summary>
        /// <returns></returns>
        public string EntityMultiFile_Delete()
        {
            int oid = this.OID;
            SysFileManager fileManager = new SysFileManager(OID);
            //获取上传的附件路径，删除附件
            string filepath = fileManager.MyFilePath;
            if (BP.Difference.SystemConfig.IsUploadFileToFTP == false)
            {
                if (System.IO.File.Exists(filepath) == true)
                    System.IO.File.Delete(filepath);
            }
            else
            {
                /*保存到fpt服务器上.*/
                FtpConnection ftpconn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                    BP.Difference.SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

                if (ftpconn == null)
                    return "err@FTP服务器连接失败";

                if (ftpconn.FileExist(filepath) == true)
                    ftpconn.DeleteFile(filepath);
            }
            fileManager.Delete();
            return fileManager.MyFileName + "删除成功";
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

                //把主数据放入里面去.
                DataTable dtMain = en.ToDataTableField("MainTable");
                ds.Tables.Add(dtMain);

                #region 增加 上方法.
                DataTable dtM = new DataTable("dtM");
                dtM.Columns.Add("No");
                dtM.Columns.Add("Title");
                dtM.Columns.Add("Tip");
                dtM.Columns.Add("Visable", System.Type.GetType("System.Boolean"));

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
                dtM.Columns.Add("ClassMethodName");

                RefMethods rms = en.EnMapInTime.HisRefMethods;
                foreach (RefMethod item in rms)
                {
                    item.HisEn = en;

                    string myurl = "";
                    if (item.RefMethodType == RefMethodType.LinkeWinOpen
                         || item.RefMethodType == RefMethodType.RightFrameOpen
                        || item.RefMethodType == RefMethodType.LinkModel)
                    {
                        try
                        {
                            myurl = item.Do(null) as string;
                            if (myurl == null)
                                continue;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("err@系统错误:根据方法名生成url出现错误:@" + ex.Message + "@" + ex.InnerException + " @方法名:" + item.Title + " - 方法:" + item.ClassMethodName);
                        }
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
                    if (attrs.Count == 0)
                        dr["FunPara"] = "false";
                    else
                        dr["FunPara"] = "true";
                    dr["ClassMethodName"] = item.ClassMethodName;
                    dtM.Rows.Add(dr); //增加到rows.
                }
                #endregion 增加 上方法.

                #region 加入一对多的实体编辑
                AttrsOfOneVSM oneVsM = en.EnMapInTime.AttrsOfOneVSM;
                string sql = "";
                int i = 0;
                if (oneVsM.Count > 0)
                {

                    foreach (AttrOfOneVSM vsM in oneVsM)
                    {
                        string rootNo = vsM.RootNo;
                        if (rootNo != null && rootNo.Contains("@") == true)
                        {
                            rootNo = rootNo.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                            rootNo = rootNo.Replace("@WebUser.OrgNo", WebUser.OrgNo);
                        }

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
                                url = "Branches.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString();
                                url += "&Dot2DotEnName=" + vsM.EnsOfMM.GetNewEntity.ToString(); //存储实体类.
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM; //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体分组依据.  比如:FK_Station.
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  
                                url += "&RootNo=" + rootNo;

                            }
                            else if (vsM.Dot2DotModel == Dot2DotModel.TreeDeptEmp)
                            {
                                url = "BranchesAndLeaf.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString();
                                url += "&Dot2DotEnName=" + vsM.EnsOfMM.GetNewEntity.ToString(); //存储实体类.
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM; //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体分组依据.  比如:FK_Station.
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  比如:FK_Station.
                                url += "&RootNo=" + rootNo;
                            }
                            else
                            {
                                url = "Dot2Dot.htm?EnName=" + this.EnName + "&Dot2DotEnsName=" + vsM.EnsOfMM.ToString(); //比如:BP.WF.Template.NodeStations
                                url += "&AttrOfOneInMM=" + vsM.AttrOfOneInMM; //存储表那个与主表关联. 比如: FK_Node
                                url += "&AttrOfMInMM=" + vsM.AttrOfMInMM;  //dot2dot存储表那个与实体表.  比如:FK_Station.
                                url += "&EnsOfM=" + vsM.EnsOfM.ToString(); //默认的B实体.   //比如:BP.Port.Stations
                                url += "&DefaultGroupAttrKey=" + vsM.DefaultGroupAttrKey; //默认的B实体分组依据.  比如:FK_Station.

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
                        dr["GroupName"] = vsM.GroupName;
                        dtM.Rows.Add(dr);
                    }
                }
                #endregion 增加 一对多.

                #region 从表
                EnDtls enDtls = en.EnMapInTime.Dtls;
                foreach (EnDtl enDtl in enDtls)
                {
                    if (enDtl.DtlEditerModel == DtlEditerModel.DtlBatchEnonly 
                        || enDtl.DtlEditerModel == DtlEditerModel.DtlSearchEnonly
                        || enDtl.DtlEditerModel == DtlEditerModel.DtlURLEnonly)
                        continue;
                    //判断该dtl是否要显示?
                    Entity myEnDtl = enDtl.Ens.GetNewEntity; //获取他的en
                    myEnDtl.SetValByKey(enDtl.RefKey, this.PKVal);  //给refpk赋值.
                    if (myEnDtl.HisUAC.IsView == false)
                        continue;

                    
                    DataRow dr = dtM.NewRow();
                    string url = "";
                    if (enDtl.DtlEditerModel == DtlEditerModel.DtlBatch)
                        url = "DtlBatch.htm?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    else
                        url = "DtlSearch.htm?EnName=" + this.EnName + "&PK=" + this.PKVal + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();

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
                    dr["Icon"] = enDtl.Icon;
                    dr["RefMethodType"] = (int)RefMethodType.RightFrameOpen;
                    dtM.Rows.Add(dr);
                }
                #endregion 增加 从表.

                ds.Tables.Add(dtM);
                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@Entity_Init错误:" + ex.Message;
            }
        }
        #endregion 实体的操作.

        public string Branches_SearchByKey()
        {

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

        #region 部门人员模式.
        public string BranchesAndLeaf_SearchByNodeID()
        {
            string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");
            string key = this.GetRequestVal("Key"); //查询关键字.
            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.

            //如果是部门人员信息，关联的有兼职部门.
            string emp1s = BP.Sys.Base.Glo.DealClassEntityName("BP.Port.Emps");
            string emp2s = BP.Sys.Base.Glo.DealClassEntityName("BP.Port.Emps");

            if ((ensOfM.Equals(emp1s) == true || ensOfM.Equals(emp2s) == true)
                && defaultGroupAttrKey.Equals("FK_Dept") == true)
            {
                string sql = "Select  E." + BP.Sys.Base.Glo.UserNo + " , E.Name ,D.Name AS FK_DeptText,-1 AS TYPE  From Port_DeptEmp DE, Port_Emp E,Port_Dept D Where DE.FK_Emp = E.No And DE.FK_Dept = D.No AND  D.No='" + key + "'";

                sql += " union ";
                sql += "select  E." + BP.Sys.Base.Glo.UserNo + " , E.Name ,D.Name AS FK_DeptText,0 AS TYPE From Port_Emp E,Port_Dept D Where E.Fk_Dept = D.No AND  D.No='" + key + "' ORDER BY TYPE DESC";
                DataTable dtt = DBAccess.RunSQLReturnTable(sql);
                DataTable dt = dtt.Clone();
                string emps = "";
                foreach (DataRow drr in dtt.Rows)
                {
                    if (emps.Contains(drr[0].ToString() + ",") == true)
                        continue;
                    emps += drr[0].ToString() + ",";

                    DataRow dr = dt.NewRow();
                    dr["No"] = drr[0];
                    dr["Name"] = drr[1];
                    dr["FK_DeptText"] = drr[2];
                    dr["type"] = drr[3];
                    dt.Rows.Add(dr);
                }
                foreach (DataColumn col in dt.Columns)
                {
                    string colName = col.ColumnName.ToLower();
                    switch (colName)
                    {
                        case "no":
                            col.ColumnName = "No";
                            break;
                        case "name":
                            col.ColumnName = "Name";
                            break;
                        case "fk_depttext":
                            col.ColumnName = "FK_DeptText";
                            break;
                        case "type":
                            col.ColumnName = "TYPE";
                            break;
                        default:
                            break;
                    }
                }
                return BP.Tools.Json.ToJson(dt);
            }



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
            string rootno = this.GetRequestVal("RootNo"); //查询根节点.

            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            QueryObject qo = new QueryObject(ensMen); //集合.
            qo.addLeftBracket();
            qo.AddWhere("No", " LIKE ", "%" + key + "%");
            qo.addOr();
            qo.AddWhere("Name", " LIKE ", "%" + key + "%");
            qo.addRightBracket();
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                qo.addAnd();
                qo.AddWhere("OrgNo", WebUser.OrgNo);
            }
            qo.DoQuery();
            DataTable dt = ensMen.ToDataTableField();
            Entity en = ensMen.GetNewEntity;
            string tableName = en.EnMap.PhysicsTable;
            if (tableName.Equals("Port_Emp") == true 
                && dt.Columns.Contains("UserID")==true)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["No"] = dr["UserID"];
                }
            }
            return BP.Tools.Json.ToJson(dt);
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
        /// 初始化		dt.Rows[3]	error CS0103: 当前上下文中不存在名称“dt”	

        /// </summary>
        /// <returns></returns>
        public string BranchesAndLeaf_Init()
        {
            string dot2DotEnsName = this.GetRequestVal("Dot2DotEnsName");
            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");
            dot2DotEnsName = BP.Sys.Base.Glo.DealClassEntityName(dot2DotEnsName);
            //string enName = this.GetRequestVal("EnName");
            Entity en = ClassFactory.GetEn(this.EnName);
            en.PKVal = this.PKVal;
            en.Retrieve();

            //找到映射.
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            AttrOfOneVSM vsM = null;
            foreach (AttrOfOneVSM item in oneVsM)
            {
                //if (item.Dot2DotModel != Dot2DotModel.TreeDeptEmp)
                //    continue;

                //if (item.EnsOfMM.ToString().Equals(dot2DotEnsName) == false)
                //    continue;

                //if (item.DefaultGroupAttrKey == null)
                //    continue;

                //if (item.DefaultGroupAttrKey.Equals(dot2DotEnsName) == false)
                //    continue;

                //vsM = item;
                //break;


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
            string rootNo = GetRequestVal("RootNo");
            if (DataType.IsNullOrEmpty(rootNo) == true)
                rootNo = vsM.RootNo;
            if (rootNo.Equals("@WebUser.FK_Dept") || rootNo.Equals("WebUser.FK_Dept"))
                rootNo = WebUser.FK_Dept;
            if (rootNo.Equals("@WebUser.OrgNo") || rootNo.Equals("WebUser.OrgNo"))
                rootNo = WebUser.OrgNo;

            if (DataType.IsNullOrEmpty(rootNo) == true)
                rootNo = "0";

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
            Entity tree = trees.GetNewEntity;

            int IsExitParentNo = 0; //是否存在ParentNo

            int IsExitIdx = 0; //判断改类是否存在Idx
            if (DBAccess.IsExitsTableCol(tree.EnMap.PhysicsTable, "Idx") == true
              && tree.EnMap.Attrs.Contains("Idx") == true)
                IsExitIdx = 1;

            if (DBAccess.IsExitsTableCol(tree.EnMap.PhysicsTable, "ParentNo") == true
               && tree.EnMap.Attrs.Contains("ParentNo") == true)
                IsExitParentNo = 1;

            if (IsExitParentNo == 1)
            {
                if (IsExitIdx == 1)
                {
                    if (rootNo.Equals("0"))
                    {
                        Entities ens = attr.HisFKEns;
                        ens.Retrieve("ParentNo", rootNo, "Idx");
                        string vals = "";
                        foreach (Entity item in ens)
                        {
                            vals += "'" + item.GetValStringByKey("No") + "'" + ",";
                        }
                        vals = vals + "'0'";
                        trees.RetrieveInOrderBy("ParentNo", vals, "Idx");


                    }

                    else
                        trees.Retrieve("No", rootNo, "Idx");
                }
                else
                {
                    if (rootNo.Equals("0"))
                    {
                        Entities ens = trees;
                        ens.Retrieve("ParentNo", rootNo, "Idx");
                        string vals = "";
                        foreach (Entity item in ens)
                        {
                            vals += "'" + item.GetValStringByKey("No") + "'" + ",";
                        }
                        vals = vals + "'0'";
                        trees.RetrieveIn("ParentNo", vals);
                    }
                    else
                        trees.Retrieve("No", rootNo);
                }
            }
            else
            {
                if (IsExitIdx == 1)
                    trees.RetrieveAll("Idx");
                else
                    trees.RetrieveAll();
            }


            DataTable dt = trees.ToDataTableField("DBTrees");
            //如果没有parnetNo 列，就增加上, 有可能是分组显示使用这个模式.
            if (dt.Columns.Contains("ParentNo") == false)
            {
                dt.Columns.Add("ParentNo");
                foreach (DataRow dr in dt.Rows)
                    dr["ParentNo"] = rootNo;
            }
            ds.Tables.Add(dt);
            dt = new DataTable();
            dt.TableName = "Base_Info";
            dt.Columns.Add("IsExitParentNo", typeof(int));
            dt.Columns.Add("ExtShowCols", typeof(string));
            DataRow drr = dt.NewRow();
            drr["IsExitParentNo"] = IsExitParentNo;
            drr["ExtShowCols"] = vsM.ExtShowCols;
            dt.Rows.Add(drr);
            ds.Tables.Add(dt);

            #endregion 生成树目录.

            #region 生成选择的数据.
            bool saveType = this.GetRequestValBoolen("SaveType");
            Entities dot2Dots = ClassFactory.GetEns(dot2DotEnsName);
            DataTable dtSelected = null;
            Entity dot2Dot = dot2Dots.GetNewEntity;
            //选择的值保存在一个字段中
            string para = this.GetRequestVal("Para");
            string paraVal = this.GetRequestVal("ParaVal");

            string para1 = this.GetRequestVal("Para1");
            string paraVal1 = this.GetRequestVal("ParaVal1");

            string pkval = this.PKVal;
            //是SAAS版并且Dot2DotEnName含有FK_Emp字段
            bool isHaveSAASEmp = BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS && dot2Dot.EnMap.Attrs.Contains("FK_Emp") == true ? true : false;
            if (isHaveSAASEmp == true)
            {
                string sql = "SELECT A.*,B.Name AS FK_EmpText FROM " + dot2Dot.EnMap.PhysicsTable + " A,Port_Emp B WHERE A.FK_Emp=B.UserID AND B.OrgNo='" + WebUser.OrgNo + "'";
                if (saveType == true)
                {
                    if (DataType.IsNullOrEmpty(para) == true)
                        sql += " AND A." + vsM.AttrOfOneInMM + "='" + pkval + "'";
                    if (DataType.IsNullOrEmpty(para1) == true)
                    {
                        pkval = pkval.Replace("_" + paraVal, "");
                        sql += " AND A." + vsM.AttrOfOneInMM + "='" + pkval + "' AND " + para + "='" + paraVal + "'";
                    }

                    else if (DataType.IsNullOrEmpty(para) == false && DataType.IsNullOrEmpty(para1) == false)
                    {
                        pkval = pkval.Replace("_" + paraVal, "");
                        sql += " AND A." + vsM.AttrOfOneInMM + "='" + pkval + "' AND " + para + "='" + paraVal + "' AND " + para1 + "='" + paraVal1 + "'";
                    }
                }
                else
                {
                    sql += " AND A." + vsM.AttrOfOneInMM + "='" + pkval + "'";
                }
                dtSelected = DBAccess.RunSQLReturnTable(sql);
                dtSelected.TableName = "DBMMs";

            }
            else
            {
                if (saveType == true)
                {
                    if (DataType.IsNullOrEmpty(para) == true)
                        dot2Dots.Retrieve(vsM.AttrOfOneInMM, this.PKVal);
                    else if (DataType.IsNullOrEmpty(para1) == true)
                    {
                        pkval = pkval.Replace("_" + paraVal, "");
                        dot2Dots.Retrieve(vsM.AttrOfOneInMM, pkval, para, paraVal);
                    }

                    else if (DataType.IsNullOrEmpty(para) == false && DataType.IsNullOrEmpty(para1) == false)
                    {
                        pkval = pkval.Replace("_" + paraVal, "");
                        dot2Dots.Retrieve(vsM.AttrOfOneInMM, pkval, para, paraVal, para1, paraVal1);
                    }


                }
                else
                {
                    dot2Dots.Retrieve(vsM.AttrOfOneInMM, this.PKVal);
                }
                dtSelected = dot2Dots.ToDataTableField("DBMMs");
            }



            string attrOfMInMM = this.GetRequestVal("AttrOfMInMM");
            string AttrOfOneInMM = this.GetRequestVal("AttrOfOneInMM");

            dtSelected.Columns[attrOfMInMM].ColumnName = "No";

            if (dtSelected.Columns.Contains(attrOfMInMM + "Text") == false && saveType == false)
                return "err@MM实体类字段属性需要按照外键属性编写:" + dot2DotEnsName + " - " + attrOfMInMM;

            if (saveType == false)
                dtSelected.Columns[attrOfMInMM + "Text"].ColumnName = "Name";

            if (DataType.IsNullOrEmpty(vsM.ExtShowCols) == false && vsM.ExtShowCols.Contains("@" + defaultGroupAttrKey + "=") == true)
            {
                if (dtSelected.Columns.Contains(defaultGroupAttrKey + "Text") == false)
                {
                    dtSelected.Columns.Add(defaultGroupAttrKey + "Text", typeof(string));
                }
                foreach (DataRow dr in dtSelected.Rows)
                {
                    enMen.PKVal = dr["No"].ToString();
                    if (isHaveSAASEmp == true)
                        enMen.PKVal = WebUser.OrgNo + "_" + enMen.PKVal;
                    enMen.RetrieveFromDBSources();
                    dr[defaultGroupAttrKey + "Text"] = enMen.Row[defaultGroupAttrKey + "Text"];
                }
            }

            dtSelected.Columns.Remove(AttrOfOneInMM);
            ds.Tables.Add(dtSelected); //已经选择的数据.
            #endregion 生成选择的数据.

            return BP.Tools.Json.ToJson(ds);
        }


        public string BranchesAndLeaf_GetTreesByParentNo()
        {
            string rootNo = GetRequestVal("RootNo");
            if (DataType.IsNullOrEmpty(rootNo))
                rootNo = "0";

            string defaultGroupAttrKey = this.GetRequestVal("DefaultGroupAttrKey");
            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            Entity enMen = ensMen.GetNewEntity;

            Attr attr = enMen.EnMap.GetAttrByKey(defaultGroupAttrKey);
            if (attr == null)
                return "err@在实体[" + ensOfM + "]指定的分树的属性[" + defaultGroupAttrKey + "]不存在，请确认是否删除了该属性?";

            if (attr.MyFieldType == FieldType.Normal)
                return "err@在实体[" + ensOfM + "]指定的分树的属性[" + defaultGroupAttrKey + "]不能是普通字段，必须是外键或者枚举.";

            Entities trees = attr.HisFKEns;
            //判断改类是否存在Idx
            Entity tree = trees.GetNewEntity;
            if (DBAccess.IsExitsTableCol(tree.EnMap.PhysicsTable, "Idx") == true
                && tree.EnMap.Attrs.Contains("Idx") == true)
                trees.Retrieve("ParentNo", rootNo, "Idx");
            else
                trees.Retrieve("ParentNo", rootNo);

            DataTable dt = trees.ToDataTableField("DBTrees");
            //如果没有parnetNo 列，就增加上, 有可能是分组显示使用这个模式.
            if (dt.Columns.Contains("ParentNo") == false)
            {
                dt.Columns.Add("ParentNo");
                foreach (DataRow dr in dt.Rows)
                    dr["ParentNo"] = rootNo;
            }
            return BP.Tools.Json.ToJson(dt); ;
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
                bool saveType = this.GetRequestValBoolen("SaveType");
                //获得点对点的实体.
                Entity en = ClassFactory.GetEns(dot2DotEnsName).GetNewEntity;
                if (saveType == true)
                {
                    //选择的值保存在一个字段中
                    string para = this.GetRequestVal("Para");
                    string paraVal = this.GetRequestVal("ParaVal");

                    string para1 = this.GetRequestVal("Para1");
                    string paraVal1 = this.GetRequestVal("ParaVal1");

                    //首先删除.
                    if (DataType.IsNullOrEmpty(para) == true)
                        en.Delete(attrOfOneInMM, this.PKVal);
                    else if (DataType.IsNullOrEmpty(para1) == true)
                        en.Delete(attrOfOneInMM, this.PKVal, para, paraVal);
                    else if (DataType.IsNullOrEmpty(para) == false && DataType.IsNullOrEmpty(para1) == false)
                        en.Delete(attrOfOneInMM, this.PKVal, para, paraVal, para1, paraVal1);

                    if (DataType.IsNullOrEmpty(eles) == true)
                        return "没有选择值";
                    en.SetValByKey(attrOfOneInMM, this.PKVal);
                    en.SetValByKey(attrOfMInMM, eles);
                    if (en.Row.ContainsKey(para))
                        en.SetValByKey(para, paraVal);
                    if (en.Row.ContainsKey(para1))
                        en.SetValByKey(para1, paraVal1);

                    en.Insert();
                    return "数据保存成功.";

                }



                en.Delete(attrOfOneInMM, this.PKVal);

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
                return "err@" + ex.Message;
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

            if (attr.MyFieldType == FieldType.Normal && attr.UIContralType != UIContralType.DDL)
                return "err@设置的默认分组[" + key + "]不能是普通字段.";

            if (attr.MyFieldType == FieldType.FK)
            {
                Entities ensFK = attr.HisFKEns;
                ensFK.Clear();
                ensFK.RetrieveAll();
                return ensFK.ToJson();
            }

            if (attr.UIContralType == UIContralType.DDL && DataType.IsNullOrEmpty(attr.UIBindKey) == false
         && attr.UIBindKey.ToUpper().StartsWith("SELECT"))
            {
                String sqlBindKey = Glo.DealExp(attr.UIBindKey, en, null);

                DataTable dt = DBAccess.RunSQLReturnTable(sqlBindKey);
                if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                {
                    dt.Columns["NO"].ColumnName = "No";
                    dt.Columns["NAME"].ColumnName = "Name";
                }
                if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                {
                    dt.Columns["NO"].ColumnName = "No";
                    dt.Columns["NAME"].ColumnName = "Name";
                }
                return BP.Tools.Json.ToJson(dt);
            }

            if (attr.MyFieldType == FieldType.Enum)
            {
                /* 如果是枚举 */
                SysEnums ses = new SysEnums();
                ses.Retrieve(SysEnumAttr.IntKey, attr.UIBindKey);
            }

            return "err@设置的默认分组[" + key + "]不能是普通字段.";
        }
        #endregion 分组数据.


    }
}
