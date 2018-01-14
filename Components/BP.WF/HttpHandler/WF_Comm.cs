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
    public class WF_Comm : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 统计分析组件.
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string ContrastDtl_Init()
        {
            return "";
        }
        #endregion 统计分析组件.

        #region Entity 公共类库.
        /// <summary>
        /// 实体类名
        /// </summary>
        public string EnName
        {
            get
            {
                return this.GetRequestVal("EnName");
            }
        }
        /// <summary>
        /// 获得实体
        /// </summary>
        /// <returns></returns>
        public string Entity_Init()
        {
            try
            {
                string pkval = this.PKVal;
                Entity en = ClassFactory.GetEn(this.EnName) ;
                
                if (pkval == "0" || pkval == "" || pkval == null || pkval == "undefined")
                {
                    Map map = en.EnMap;
                    foreach (Attr attr in en.EnMap.Attrs)
                        en.SetValByKey(attr.Key, attr.DefaultVal);
                    //设置默认的数据.
                    en.ResetDefaultVal();

                }
                else
                {
                    en.PKVal = pkval;
                    en.Retrieve();
                }

                return en.ToJson(false);
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string Entity_Delete()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                en.PKVal = this.PKVal;
                int i= en.RetrieveFromDBSources(); //查询出来再删除.
                if (i == 0)
                    return "err@无此记录，无法删除."+this.EnName+" - "+en.PKVal;

               return en.Delete().ToString(); //返回影响行数.
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public string Entity_Update()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));


                //返回数据.
               // return en.ToJson(false);

                return en.Update().ToString(); //返回影响行数.
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 从数据源查询.
        /// </summary>
        /// <returns></returns>
        public string Entity_RetrieveFromDBSources()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                en.PKVal = this.PKVal;
                int i = en.RetrieveFromDBSources();

                if (i == 0)
                {
                    en.ResetDefaultVal();
                    en.PKVal = this.PKVal;
                }

                if (en.Row.ContainsKey("RetrieveFromDBSources") == true)
                    en.Row["RetrieveFromDBSources"] = i;
                else
                    en.Row.Add("RetrieveFromDBSources", i);

                return en.ToJson(false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 从数据源查询.
        /// </summary>
        /// <returns></returns>
        public string Entity_Retrieve()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);

                en = en.CreateInstance();
                en.PKVal = this.PKVal;
                en.Retrieve();

                if (en.Row.ContainsKey("Retrieve") == true)
                    en.Row["Retrieve"] = "1";
                else
                    en.Row.Add("Retrieve", "1");

                return en.ToJson(false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <returns></returns>
        public string Entity_IsExits()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                en.PKVal = this.PKVal;
                bool isExit = en.IsExits;
                if (isExit == true)
                    return "1";
                return "0";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns>返回保存影响的行数</returns>
        public string Entity_Save()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                if (en == null)
                    return "err@实体类名错误["+this.EnName+"].";

                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, this.GetValFromFrmByKey(attr.Key));

                //保存参数属性.
                string frmParas = this.GetValFromFrmByKey("frmParas","");
                if (DataType.IsNullOrEmpty(frmParas) == false)
                {
                    AtPara ap = new AtPara(frmParas);
                    foreach (string key in ap.HisHT.Keys)
                    {
                        en.SetPara(key, ap.GetValStrByKey(key));
                    }
                }

                return en.Save().ToString();
            }
            catch (Exception ex)
            {
                return "err@保存错误:" + ex.Message;
            }
        }
        /// <summary>
        /// 执行插入.
        /// </summary>
        /// <returns></returns>
        public string Entity_Insert()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, this.GetValFromFrmByKey(attr.Key));

                //插入数据库.
                int i = en.Insert();
                if (i == 1)
                    en.Retrieve();//执行查询.

                //返回数据.
                return en.ToJson(false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        
        public string Entity_DoMethodReturnString()
        {
            //创建类实体.
            BP.En.Entity en = ClassFactory.GetEn(this.EnName); // Activator.CreateInstance(System.Type.GetType("BP.En.Entity")) as BP.En.Entity;
            en.PKVal = this.PKVal;
            en.RetrieveFromDBSources();

            string methodName = this.GetRequestVal("MethodName");

            Type tp = en.GetType();
            System.Reflection.MethodInfo mp = tp.GetMethod(methodName);
            if (mp == null)
                return "err@没有找到类[" + this.EnName + "]方法[" + methodName + "].";

            string paras = this.GetRequestVal("paras");

            //执行该方法.
            object[] myparas = new object[0];

            if (DataType.IsNullOrEmpty(paras) == false)
                myparas = paras.Split(',');

            string result = mp.Invoke(en, myparas) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。
            return result;
        }


        #endregion 
       
        #region Entities 公共类库.
        /// <summary>
        /// 调用参数.
        /// </summary>
        public string Paras
        {
            get
            {
                return this.GetRequestVal("Paras");
            }
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public string Entities_RetrieveAll()
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            ens.RetrieveAll();
            return ens.ToJson();
        }
        /// <summary>
        /// 获得实体集合s
        /// </summary>
        /// <returns></returns>
        public string Entities_Init()
        {
            try
            {
                Entities ens = ClassFactory.GetEns(this.EnsName);
                if (this.Paras == null)
                    return "0";

                QueryObject qo = new QueryObject(ens);
                string[] myparas = this.Paras.Split('@');

                int idx = 0;
                for (int i = 0; i < myparas.Length; i++)
                {
                    string para = myparas[i];
                    if (DataType.IsNullOrEmpty(para) || para.Contains("=")==false)
                        continue;

                    string[] strs = para.Split('=');
                    string key = strs[0];
                    string val = strs[1];

                    if (key.ToLower().Equals("orderby") == true)
                    {
                        qo.addOrderBy(val);
                        continue;
                    }

                    if (idx == 0)
                    {
                        qo.AddWhere(key, val);
                    }
                    else
                    {
                        qo.addAnd();
                        qo.AddWhere(key, val);
                    }
                    idx++;
                }

                qo.DoQuery();
                return ens.ToJson();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 

        #region 功能执行.
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string Method_Init()
        {
            string ensName = this.GetRequestVal("M");
            Method rm = BP.En.ClassFactory.GetMethod(ensName);
            if (rm == null)
                return "err@方法名错误或者该方法已经不存在" + ensName;

            if (rm.HisAttrs.Count == 0)
            {
                Hashtable ht = new Hashtable();
                ht.Add("No", ensName);
                ht.Add("Title", rm.Title);
                ht.Add("Help", rm.Help);
                ht.Add("Warning", rm.Warning);
                return BP.Tools.Json.ToJson(ht);
            }

            DataTable dt = new DataTable();

            //转化为集合.
            MapAttrs attrs = rm.HisAttrs.ToMapAttrs;

            return "";
        }
        public string Method_Done()
        {
            string ensName = this.GetRequestVal("M");
            Method rm = BP.En.ClassFactory.GetMethod(ensName);
            // rm.Init();
            int mynum = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                mynum++;
            }
            int idx = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                try
                {
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDate:
                                case BP.DA.DataType.AppDateTime:
                                    string str1 = this.GetValFromFrmByKey(attr.Key);
                                    rm.SetValByKey(attr.Key, str1);
                                    break;
                                case BP.DA.DataType.AppInt:
                                    int myInt =  this.GetValIntFromFrmByKey(attr.Key);  //int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.Row[idx] = myInt;
                                    rm.SetValByKey(attr.Key, myInt);
                                    break;
                                case BP.DA.DataType.AppFloat:
                                    float myFloat = this.GetValFloatFromFrmByKey(attr.Key); // float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myFloat);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppMoney:
                                    decimal myDoub =this.GetValDecimalFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myDoub);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    bool myBool =this.GetValBoolenFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myBool);
                                    break;
                                default:
                                    return "err@没有判断的数据类型．";
                            }
                            break;
                        case UIContralType.DDL:
                            try
                            {
                                string str = this.GetValFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                               // string str = this.UCEn1.GetDDLByKey("DDL_" + attr.Key).SelectedItemStringVal;
                                rm.SetValByKey(attr.Key, str);
                            }
                            catch
                            {
                                rm.SetValByKey(attr.Key, "");
                            }
                            break;
                        case UIContralType.CheckBok:
                                    bool myBoolval =this.GetValBoolenFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                rm.SetValByKey(attr.Key, myBoolval);
                            break;
                        default:
                            break;
                    }
                    idx++;
                }
                catch (Exception ex)
                {
                    return "err@获得参数错误" + "attr=" + attr.Key + " attr = " + attr.Key + ex.Message;
                }
            }

            try
            {
                object obj = rm.Do();
                if (obj != null)
                    return obj.ToString();
                else
                    return "err@执行完成没有返回信息.";
            }
            catch (Exception ex)
            {
                return "err@执行错误:" + ex.Message;
            }
        }
        public string MethodLink_Init()
        {
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.En.Method");
            int i = 1;
            string html = "";

            //DataTable dt = new DataTable();
            //dt.Columns.Add("No", typeof(string));
            //dt.Columns.Add("Name", typeof(string));
            //dt.Columns.Add("Icon", typeof(string));
            //dt.Columns.Add("Note", typeof(string));

            foreach (BP.En.Method en in al)
            {
                if (en.IsCanDo == false
                    || en.IsVisable == false)
                    continue;

              //DataRow dr = dt.NewRow();

                html += "<li><a href=\"javascript:ShowIt('" + en.ToString() + "');\"  >" + en.GetIcon("/") + en.Title + "</a><br><font size=2 color=Green>" + en.Help + "</font><br><br></li>";
            }

            return html;
        }
        #endregion

        #region 查询.
        /// <summary>
        /// 获得查询的基本信息.
        /// </summary>
        /// <returns></returns>
        public string Search_MapBaseInfo()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;

            Hashtable ht = new Hashtable();

            //把权限信息放入.
            UAC uac = en.HisUAC;
            ht.Add("IsUpdata", uac.IsUpdate);
            ht.Add("IsInsert", uac.IsInsert);
            ht.Add("IsDelete", uac.IsDelete);
            ht.Add("IsView", uac.IsView);
            ht.Add("IsExp", uac.IsExp); //是否可以导出?
            ht.Add("IsImp", uac.IsImp); //是否可以导入?

            ht.Add("EnDesc", en.EnDesc); //描述?
            ht.Add("EnName", en.ToString() ); //类名?

            //把map信息放入
            ht.Add("PhysicsTable", map.PhysicsTable);
            ht.Add("CodeStruct", map.CodeStruct);
            ht.Add("CodeLength", map.CodeLength);

            //查询条件.
            if (map.IsShowSearchKey==true)
               ht.Add("IsShowSearchKey", 1);
            else
                ht.Add("IsShowSearchKey", 0);

            //按日期查询.
            ht.Add("DTSearchWay", (int)map.DTSearchWay);
            ht.Add("DTSearchLable", map.DTSearchLable);
            ht.Add("DTSearchKey", map.DTSearchKey);

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 外键或者枚举的查询.
        /// </summary>
        /// <returns></returns>
        public string Search_SearchAttrs()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;

            DataSet ds = new DataSet();

            //构造查询条件集合.
            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.TableName = "Attrs";

            AttrSearchs attrs = map.SearchAttrs;
            foreach (AttrSearch item in attrs)
            {
                DataRow dr = dt.NewRow();
                dr["Field"] = item.Key;
                dr["Name"] = item.HisAttr.Desc;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            //把外键枚举增加到里面.
            foreach (AttrSearch item in attrs)
            {
                if (item.HisAttr.IsEnum == true)
                {
                    SysEnums ses = new SysEnums(item.HisAttr.UIBindKey);
                    DataTable dtEnum = ses.ToDataTableField();
                    dtEnum.TableName = item.Key;
                    ds.Tables.Add(dtEnum);
                    continue;
                }

                if (item.HisAttr.IsFK == true)
                {
                    Entities ensFK = item.HisAttr.HisFKEns;
                    ensFK.RetrieveAll();

                    DataTable dtEn = ensFK.ToDataTableField();
                    dtEn.TableName = item.Key;

                    ds.Tables.Add(dtEn);
                }
            }

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行查询.
        /// </summary>
        /// <returns></returns>
        public string Search_SearchIt()
        {
            //获得.
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;

            MapAttrs attrs = map.Attrs.ToMapAttrs;

            //属性集合.
            DataTable dtAttrs = attrs.ToDataTableField();
            dtAttrs.TableName = "Attrs";

            DataSet ds = new DataSet();
            ds.Tables.Add(dtAttrs); //把描述加入.

            //取出来查询条件.
            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.EnsName + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);

            //关键字.
            string keyWord = ur.SearchKey;
            QueryObject qo = new QueryObject(ens);

            #region 关键字字段.
            if (en.EnMap.IsShowSearchKey && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length > 1)
            {
                Attr attrPK = new Attr();
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.IsPK)
                    {
                        attrPK = attr;
                        break;
                    }
                }
                int i = 0;
                foreach (Attr attr in map.Attrs)
                {
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
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (SystemConfig.AppCenterDBVarStr == "@")
                            qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }
                    qo.addOr();

                    if (SystemConfig.AppCenterDBVarStr == "@")
                        qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                    else
                        qo.AddWhere(attr.Key, " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");

                }
                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();

            }
            else
            {
                qo.AddHD();
            }
            #endregion


            if (map.DTSearchWay != DTSearchWay.None && DataType.IsNullOrEmpty( ur.DTFrom) ==false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                if (map.DTSearchWay == DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = map.DTSearchKey + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = map.DTSearchKey + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }

                if (map.DTSearchWay == DTSearchWay.ByDateTime)
                {
                    //取前一天的24：00
                    if (dtFrom.Trim().Length == 10) //2017-09-30
                        dtFrom += " 00:00:00";
                    if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                        dtFrom += ":00";

                    dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                    if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                        dtTo += " 24:00";

                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = map.DTSearchKey + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = map.DTSearchKey + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }
            }


            #region 普通属性
            string opkey = ""; // 操作符号。
            foreach (AttrOfSearch attr in en.EnMap.AttrsOfSearch)
            {
                if (attr.IsHidden)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.AddWhere(attr.RefAttrKey, attr.DefaultSymbol, attr.DefaultValRun);
                    qo.addRightBracket();
                    continue;
                }

                if (attr.SymbolEnable == true)
                {
                    opkey = ap.GetValStrByKey("DDL_" + attr.Key);
                    if (opkey == "all")
                        continue;
                }
                else
                {
                    opkey = attr.DefaultSymbol;
                }

                qo.addAnd();
                qo.addLeftBracket();

                if (attr.DefaultVal.Length >= 8)
                {
                    string date = "2005-09-01";
                    try
                    {
                        /* 就可能是年月日。 */
                        string y =ap.GetValStrByKey("DDL_" + attr.Key + "_Year");
                        string m = ap.GetValStrByKey("DDL_" + attr.Key + "_Month");
                        string d = ap.GetValStrByKey("DDL_" + attr.Key + "_Day");
                        date = y + "-" + m + "-" + d;

                        if (opkey == "<=")
                        {
                            DateTime dt = DataType.ParseSysDate2DateTime(date).AddDays(1);
                            date = dt.ToString(DataType.SysDataFormat);
                        }
                    }
                    catch
                    {
                    }

                    qo.AddWhere(attr.RefAttrKey, opkey, date);
                }
                else
                {
                    qo.AddWhere(attr.RefAttrKey, opkey, ap.GetValStrByKey("TB_" + attr.Key));
                }
                qo.addRightBracket();
            }
            #endregion

            #region 获得查询数据.
            foreach (string str in ap.HisHT.Keys)
            {
                var val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                    continue;
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(str, ap.GetValStrByKey(str));
                qo.addRightBracket();
            }

            //获得行数.
            if (this.PageIdx == 1)
            {
                ur.SetPara("RecCount", qo.GetCount());
                ur.Update();
            }

            qo.DoQuery(en.PK,12,this.PageIdx);
            #endregion 获得查询数据.

            DataTable mydt = ens.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.

            return BP.Tools.Json.ToJson(ds);
        }
        public string Search_GenerPageIdx()
        {

            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.EnsName + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            string url = "?EnsName="+this.EnsName;
            int pageSpan = 20;
            int recNum = ur.GetParaInt("RecCount"); //获得查询数量.
            int pageSize = 12;
            if (recNum <= pageSize)
                return "1";

            string html = "";
            html += "<div style='text-align:center;'>";

            string appPath = ""; // this.Request.ApplicationPath;
            int myidx = 0;
            if (PageIdx <= 1)
            {
                //this.Add("《- 《-");
                html += "<img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/LeftEnd.png' border=0/><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Left.png' border=0/>";
            }
            else
            {
                myidx = PageIdx - 1;
                //this.Add("<a href='" + url + "&PageIdx=1' >《-</a> <a href='" + url + "&PageIdx=" + myidx + "'>《-</a>");
                html += "<a href='" + url + "&PageIdx=1' ><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/LeftEnd.png' border=0/></a><a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Left.png' border=0/></a>";
            }

            /*int pageNum = 0;
            decimal pageCountD = decimal.Parse(recNum.ToString()) / decimal.Parse(pageSize.ToString()); // 页面个数。
            string[] strs = pageCountD.ToString("0.0000").Split('.');
            if (int.Parse(strs[1]) > 0)
                pageNum = int.Parse(strs[0]) + 1;
            else
                pageNum = int.Parse(strs[0]);

            int from = 0;
            int to = 0;

            decimal spanTemp = decimal.Parse(PageIdx.ToString()) / decimal.Parse(pageSpan.ToString()); // 页面个数。

            strs = spanTemp.ToString("0.0000").Split('.');
            from = int.Parse(strs[0]) * pageSpan;
            to = from + pageSpan;*/
            //分页采用java默认方式分页，采用bigdecimal分页报错
            int pageNum = (recNum + pageSize - 1) / pageSize;// 页面个数。

            int from = PageIdx < 1 ? 0 : (PageIdx - 1) * pageSize + 1;//从

            int to = PageIdx < 1 ? pageSize : PageIdx * pageSize;//到

            for (int i = 1; i <= pageNum; i++)
            {
                if (i >= from && i < to)
                {
                }
                else
                {
                    continue;
                }

                if (PageIdx == i)
                    html += "&nbsp;<font style='font-weight:bloder;color:#f00'>" + i + "</font>&nbsp;";
                else
                    html += "&nbsp;<a href='" + url + "&PageIdx=" + i + "'>" + i + "</a>";
            }

            if (PageIdx != pageNum)
            {
                myidx = PageIdx + 1;
                //this.Add("&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'>-》</a>&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'>-》</a>&nbsp;&nbsp;Page:" + PageIdx + "/" + pageNum + " Total:" + recNum + ".");
                html += "&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Right.png' border=0/></a>&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/RightEnd.png' border=0/></a>&nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum;
            }
            else
            {
                //this.Add("&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'> -》》</a>&nbsp;&nbsp;Page:" + PageIdx + "/" + pageNum + " Totlal:" + recNum + ".");
                html += "&nbsp;<img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Right.png' border=0/>&nbsp;&nbsp;";
                html += "&nbsp;<img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/RightEnd.png' border=0/>&nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum;
                //this.Add("<img src='/WF/Img/Page_Down.gif' border=1 />");
            }
            html += "</div>";
            return html;
        }

        #endregion 查询.

        #region Refmethod.htm 相关功能.
        public string RefEnKey
        {
            get
            {
                return this.PKVal;
            }
        }
        public string Refmethod_Init()
        {
            string ensName = this.EnsName;
            int index = this.Index;
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];

            #region 处理无参数的方法.
            if (rm.HisAttrs == null || rm.HisAttrs.Count == 0)
            {
                string pk = this.RefEnKey;
                if (pk == null)
                    pk = this.GetRequestVal(en.PK);

                en.PKVal = pk;
                en.Retrieve();
                rm.HisEn = en;

                // 如果是link.
                if (rm.RefMethodType == RefMethodType.LinkModel)
                {
                    string url = rm.Do(null) as string;
                    if (string.IsNullOrEmpty(url))
                        return "err@应该返回的url.";

                    return "url@" + url;
                }

                object obj = rm.Do(null);
                if (obj == null)
                {
                    return "close@info";
                }

                string info = obj.ToString();
                info = info.Replace("@", "\t\n");
                return "close@" + info;
            }
            #endregion 处理无参数的方法.

            //转化为json 返回到前台解析. 处理有参数的方法.
            DataSet ds = new DataSet();
            MapAttrs attrs = rm.HisAttrs.ToMapAttrs;

            //属性.
            DataTable mapAttrs = attrs.ToDataTableField("Sys_MapAttrs");
            ds.Tables.Add(mapAttrs);

            #region 该方法的默认值.
            DataTable dtMain = new DataTable();
            dtMain.TableName = "MainTable";
            foreach (MapAttr attr in attrs)
            {
                dtMain.Columns.Add(attr.KeyOfEn, typeof(string));
            }

            DataRow mydrMain = dtMain.NewRow();
            foreach (MapAttr item in attrs)
            {
                mydrMain[item.KeyOfEn] = item.DefValReal;
            }
            dtMain.Rows.Add(mydrMain);
            ds.Tables.Add(dtMain);
            #endregion 该方法的默认值.

            #region 加入该方法的外键.
            foreach (DataRow dr in mapAttrs.Rows)
            {
                string lgType = dr["LGType"].ToString();
                if (lgType != "2")
                    continue;

                string UIIsEnable = dr["UIVisible"].ToString();
                if (UIIsEnable == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                if (string.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                ds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }
            #endregion 加入该方法的外键.

            #region 加入该方法的枚举.
            DataTable dtEnum = new DataTable();
            dtEnum.Columns.Add("Lab", typeof(string));
            dtEnum.Columns.Add("EnumKey", typeof(string));
            dtEnum.Columns.Add("IntKey", typeof(string));
            dtEnum.TableName = "Sys_Enum";

            foreach (MapAttr item in attrs)
            {
                if (item.LGType != FieldTypeS.Enum)
                    continue;

                SysEnums ses = new SysEnums(item.UIBindKey);
                foreach (SysEnum se in ses)
                {
                    DataRow drEnum = dtEnum.NewRow();
                    drEnum["Lab"] = se.Lab;
                    drEnum["EnumKey"] = se.EnumKey;
                    drEnum["IntKey"] = se.IntKey;
                    dtEnum.Rows.Add(drEnum);
                }

            }

            ds.Tables.Add(dtEnum);
            #endregion 加入该方法的枚举.

            #region 增加该方法的信息
            DataTable dt = new DataTable();
            dt.TableName = "RM";
            dt.Columns.Add("Title",typeof(string));
            dt.Columns.Add("Warning", typeof(string));

            DataRow mydr = dt.NewRow();
            mydr["Title"] = rm.Title;
            mydr["Warning"] = rm.Warning;
            dt.Rows.Add(mydr);
            #endregion 增加该方法的信息

            //增加到里面.
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        public string Refmethod_Done()
        {
            Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            string msg = "";

            string pk = this.RefEnKey;
            if (pk.Contains(",") == false)
            {
                /*批处理的方式.*/
                en.PKVal = pk;
                en.Retrieve();
                msg = DoOneEntity(en, this.Index);
                if (msg == null)
                    return "close@info";
                else
                    return "info@" + msg;
            }

            //如果是批处理.
            string[] pks = pk.Split(',');
            foreach (string mypk in pks)
            {
                if (string.IsNullOrEmpty(mypk) == true)
                    continue;
                en.PKVal = mypk;
                en.Retrieve();

                string s = DoOneEntity(en, this.Index);
                if (s != null)
                    msg += "@" + s;
            }
            if (msg == "")
                return "close@info";
            else
                return "info@" + msg;
        }
        public string DoOneEntity(Entity en, int rmIdx)
        {
            BP.En.RefMethod rm = en.EnMap.HisRefMethods[rmIdx];
            rm.HisEn = en;
            int mynum = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                mynum++;
            }

            object[] objs = new object[mynum];

            int idx = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                            case BP.DA.DataType.AppDate:
                            case BP.DA.DataType.AppDateTime:
                                string str1 = this.GetValFromFrmByKey(attr.Key);
                                objs[idx] = str1;
                                //attr.DefaultVal=str1;
                                break;
                            case BP.DA.DataType.AppInt:
                                int myInt = this.GetValIntFromFrmByKey(attr.Key);
                                objs[idx] = myInt;
                                //attr.DefaultVal=myInt;
                                break;
                            case BP.DA.DataType.AppFloat:
                                float myFloat = this.GetValFloatFromFrmByKey(attr.Key);
                                objs[idx] = myFloat;
                                //attr.DefaultVal=myFloat;
                                break;
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppMoney:
                                decimal myDoub = this.GetValDecimalFromFrmByKey(attr.Key);
                                objs[idx] = myDoub;
                                //attr.DefaultVal=myDoub;
                                break;
                            case BP.DA.DataType.AppBoolean:
                                objs[idx] = this.GetValBoolenFromFrmByKey(attr.Key);
                                attr.DefaultVal = false;
                                break;
                            default:
                                throw new Exception("没有判断的数据类型．");

                        }
                        break;
                    case UIContralType.DDL:
                        try
                        {
                            if (attr.MyFieldType == FieldType.FK)
                            {
                                string str = this.GetValFromFrmByKey(attr.Key);
                                objs[idx] = str;
                                attr.DefaultVal = str;
                            }
                            else
                            {
                                int enumVal = this.GetValIntFromFrmByKey(attr.Key);
                                objs[idx] = enumVal;
                                attr.DefaultVal = enumVal;
                            }

                        }
                        catch
                        {
                            objs[idx] = null;
                        }
                        break;
                    case UIContralType.CheckBok:
                        objs[idx] = this.GetValBoolenFromFrmByKey(attr.Key);

                        attr.DefaultVal = objs[idx].ToString();

                        break;
                    default:
                        break;
                }
                idx++;
            }

            try
            {
                object obj = rm.Do(objs);
                if (obj != null)
                    return obj.ToString();

                return null;
            }
            catch (Exception ex)
            {
                string msg = "";
                foreach (object obj in objs)
                    msg += "@" + obj.ToString();
                string err = "@执行[" + this.EnsName + "]["+rm.ClassMethodName+"]期间出现错误：" + ex.Message + " InnerException= " + ex.InnerException + "[参数为：]" + msg;
                return "<font color=red>" + err + "</font>";
            }
        }
        #endregion 相关功能.

        #region  公共方法。
        public string SFTable()
        {
            SFTable sftable = new SFTable(this.GetRequestVal("SFTable") );
            DataTable dt = sftable.GenerData();
            return BP.Tools.Json.ToJson(dt);
        }
        public string EnsData()
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);

            string filter = this.GetRequestVal("Filter");

            if (filter == null || filter == "" || filter.Contains("=") == false)
            {
                ens.RetrieveAll();
            }
            else
            {
                QueryObject qo = new QueryObject(ens);
                string[] strs = filter.Split('=');
                qo.AddWhere(strs[0], strs[1]);
                qo.DoQuery();
            }
            return ens.ToJson();
        }
        /// <summary>
        /// 执行一个SQL，然后返回一个列表.
        /// 用于gener.js 的公共方法.
        /// </summary>
        /// <returns></returns>
        public string SQLList()
        {
            string sqlKey = context.Request.QueryString["SQLKey"]; //SQL的key.
            string paras = context.Request.QueryString["Paras"]; //参数. 格式为 @para1=paraVal@para2=val2

            BP.Sys.XML.SQLList sqlXml = new BP.Sys.XML.SQLList(sqlKey);

            //获得SQL
            string sql = sqlXml.SQL;
            string[] strs = paras.Split('@');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                //参数.
                string[] p = str.Split('=');
                sql = sql.Replace("@" + p[0], p[1]);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string EnumList()
        {
            SysEnums ses = new SysEnums(this.EnumKey);
            return ses.ToJson();
        }
        #endregion  公共方法。

        #region 执行方法.
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="clsName">类名称</param>
        /// <param name="monthodName">方法名称</param>
        /// <param name="paras">参数，可以为空.</param>
        /// <returns>执行结果</returns>
        public string Exec(string clsName, string methodName, string paras=null)
        {
            #region 处理 HttpHandler 类.
            if (clsName.Contains(".HttpHandler.") == true)
            {
                //创建类实体.
                DirectoryPageBase ctrl = Activator.CreateInstance(System.Type.GetType("BP.WF.HttpHandler.DirectoryPageBase"),
                    this.context) as DirectoryPageBase;
                ctrl.context = this.context;

                try
                {
                    //执行方法返回json.
                    string data = ctrl.DoMethod(ctrl, methodName);
                    return data;
                }
                catch (Exception ex)
                {
                    string parasStr = "";
                    foreach (string key in context.Request.QueryString.Keys)
                    {
                        parasStr += "@" + key + "=" + context.Request.QueryString[key];
                    }
                    return "err@" + ex.Message + " 参数:" + parasStr;
                }
            }
            #endregion 处理 page 类.

            #region 执行entity类的方法.
            try
            {
                //创建类实体.
                BP.En.Entity en = Activator.CreateInstance(System.Type.GetType("BP.En.Entity")) as BP.En.Entity;
                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                Type tp = en.GetType();
                System.Reflection.MethodInfo mp = tp.GetMethod(methodName);
                if (mp == null)
                    return "err@没有找到类[" + clsName + "]方法[" + methodName + "].";

                //执行该方法.
                object[] myparas = null;
                string result= mp.Invoke(this, myparas) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。
                return result;
            }
            catch (Exception ex)
            {
                return "err@执行实体类的方法错误:" + ex.Message;
            }
            #endregion 执行entity类的方法.
        }
        #endregion

        #region 数据库相关.
        /// <summary>
        /// 运行SQL
        /// </summary>
        /// <returns>返回影响行数</returns>
        public string DBAccess_RunSQL()
        {
            string sql = this.GetRequestVal("SQL");
            sql = sql.Replace("~", "'");
            return DBAccess.RunSQL(sql).ToString();
        }
        /// <summary>
        /// 运行SQL返回DataTable
        /// </summary>
        /// <returns>DataTable转换的json</returns>
        public string DBAccess_RunSQLReturnTable()
        {
            string sql = this.GetRequestVal("SQL");
            sql = sql.Replace("~","'");
            sql = sql.Replace("-", "%");
            if (null == sql || "" == sql)
            {
                return "err@查询sql为空";
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 运行Url返回string.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string RunUrlCrossReturnString(string url)
        {
            string strs = DataType.ReadURLContext(url, 9999, System.Text.Encoding.UTF8);
            return strs;
        }
        #endregion

        //执行方法.
        public string HttpHandler()
        {
            //获得两个参数.
            string httpHandlerName = this.GetRequestVal("HttpHandlerName");
            string methodName = this.GetRequestVal("DoMethod");

            BP.WF.HttpHandler.DirectoryPageBase en = Activator.CreateInstance(System.Type.GetType(httpHandlerName),this.context) 
                as BP.WF.HttpHandler.DirectoryPageBase;

            en.context = this.context;
            return en.DoMethod(en, methodName);
        }
        /// <summary>
        /// 当前登录人员信息
        /// </summary>
        /// <returns></returns>
        public string WebUser_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("No", WebUser.No);
            ht.Add("Name", WebUser.Name);
            ht.Add("FK_Dept", WebUser.FK_Dept);
            ht.Add("FK_DeptName", WebUser.FK_DeptName);
            ht.Add("FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
            return BP.Tools.Json.ToJson(ht);
        }
    }
}
