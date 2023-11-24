using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Sys.XML;
using BP.Web;
using BP.En;
using BP.Difference;
using System.Text;
using BP.Tools;
using System.Reflection;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Tsp;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Comm : DirectoryPageBase
    {
        #region 树的实体.
        /// <summary>
        /// 获得树的结构
        /// </summary>
        /// <returns></returns>
        public string Tree_Init()
        {
            EntitiesTree ens = ClassFactory.GetEns(this.EnsName) as EntitiesTree;
            if (ens == null)
                return "err@该实体[" + this.EnsName + "]不是一个树形实体.";
            //获取ParentNo
            ens.RetrieveAll(EntityTreeAttr.Idx);

            return BP.Tools.Json.ToJson(ens.ToDataTableField("TreeTable"));
        }
        #endregion 树的实体

        #region 部门-人员关系.

        public string Tree_MapBaseInfo()
        {
            EntitiesTree enTrees = ClassFactory.GetEns(this.TreeEnsName) as EntitiesTree;
            EntityTree enenTree = enTrees.GetNewEntity as EntityTree;
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Hashtable ht = new Hashtable();
            ht.Add("TreeEnsDesc", enenTree.EnDesc);
            ht.Add("EnsDesc", en.EnDesc);
            ht.Add("EnPK", en.PK);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 获得树的结构
        /// </summary>
        /// <returns></returns>
        public string TreeEn_Init()
        {
            EntitiesTree ens = ClassFactory.GetEns(this.TreeEnsName) as EntitiesTree;
            ens.RetrieveAll(EntityTreeAttr.Idx);
            return ens.ToJsonOfTree();
        }

        /// <summary>
        /// 获取树关联的集合
        /// </summary>
        /// <returns></returns>
        public string TreeEmp_Init()
        {
            DataSet ds = new DataSet();
            string RefPK = this.GetRequestVal("RefPK");
            string FK = this.GetRequestVal("FK");
            //获取关联的信息集合
            Entities ens = ClassFactory.GetEns(this.EnsName);
            ens.RetrieveByAttr(RefPK, FK);
            DataTable dt = ens.ToDataTableField("GridData");
            ds.Tables.Add(dt);

            //获取实体对应的列明
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;
            MapAttrs attrs = map.Attrs.ToMapAttrs;
            //属性集合.
            DataTable dtAttrs = attrs.ToDataTableField();
            dtAttrs.TableName = "Sys_MapAttrs";

            dt = new DataTable("Sys_MapAttr");
            dt.Columns.Add("field", typeof(string));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            DataRow row = null;
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.KeyOfEn == this.RefPK)
                    continue;

                row = dt.NewRow();
                row["field"] = attr.KeyOfEn;
                row["title"] = attr.Name;
                row["Width"] = attr.UIWidthInt * 2;
                row["UIContralType"] = attr.UIContralType;

                if (attr.HisAttr.ItIsFKorEnum)
                    row["field"] = attr.KeyOfEn + "Text";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 部门-人员关系
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Comm()
        {
        }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string DeptNo
        {
            get
            {
                string str = this.GetRequestVal("FK_Dept");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
            set
            {
                string val = value;
                if (val == "all")
                    return;

                if (this.DeptNo == null)
                {
                    this.DeptNo = value;
                    return;
                }
            }

        }

        #region 统计分析组件.
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string ContrastDtl_Init()
        {
            //获得.
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;

            MapAttrs attrs = new MapAttrs();
            MapData md = new MapData();
            md.No = this.EnsName;
            int count = md.RetrieveFromDBSources();
            if (count == 0)
                attrs = map.Attrs.ToMapAttrs;
            else
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.EnsName, MapAttrAttr.Idx);

            //属性集合.
            DataTable dtAttrs = attrs.ToDataTableField();
            dtAttrs.TableName = "Sys_MapAttrs";

            DataSet ds = new DataSet();
            ds.Tables.Add(dtAttrs); //把描述加入.

            //增加分组的查询条件
            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.setMyPK(WebUser.No + "_" + this.EnsName + "_SearchAttrs");
            ur.RetrieveFromDBSources();
            AtPara ap = new AtPara(ur.Vals);
            string vals = "";
            foreach (string str in ap.HisHT.Keys)
            {
                string val = this.GetRequestVal(str);
                if (DataType.IsNullOrEmpty(val) == false)
                    vals += "@" + str + "=" + val;
                else
                    vals += "@" + str + "=" + ap.HisHT[str];
            }
            ur.SetValByKey(UserRegeditAttr.Vals, vals);
            //查询结果
            QueryObject qo = Search_Data(ens, en, map, ur);
            //获取配置信息
            EnCfg encfg = new EnCfg();
            encfg.No = this.EnsName;
            encfg.RetrieveFromDBSources();

            //增加排序
            string orderBy = "";
            bool isDesc = false;
            if (DataType.IsNullOrEmpty(ur.OrderBy) == false)
            {
                orderBy = ur.OrderBy;
                isDesc = ur.OrderWay.Equals("desc") == true ? true : false;
            }

            if (DataType.IsNullOrEmpty(ur.OrderBy) == true && encfg != null)
            {
                orderBy = encfg.GetValStrByKey("OrderBy");
                if (orderBy.IndexOf(",") != -1)
                {
                    string[] str = orderBy.Split(',');
                    orderBy = str[0];
                }
                isDesc = encfg.GetValBooleanByKey("IsDeSc");
            }

            if (DataType.IsNullOrEmpty(orderBy) == false)
            {
                try
                {
                    if (isDesc)
                        qo.addOrderByDesc(orderBy);
                    else
                        qo.addOrderBy(orderBy);
                }
                catch (Exception ex)
                {
                    encfg.SetValByKey("OrderBy", orderBy);
                }
            }


            qo.DoQuery();

            DataTable dt = ens.ToDataTableField();
            dt.TableName = "Group_Dtls";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 执行导出
        /// </summary>
        /// <returns></returns>
        //public string GroupDtl_Exp()
        //{
        //    //获得.
        //    Entities ens = ClassFactory.GetEns(this.EnsName);
        //    Entity en = ens.GetNewEntity;

        //    //查询结果
        //    QueryObject qo = new QueryObject(ens);
        //    string[] strs = HttpContextHelper.Request.Form.ToString().Split('&');
        //    foreach (string str in strs)
        //    {
        //        if (str.IndexOf("EnsName") != -1)
        //            continue;

        //        string[] mykey = str.Split('=');
        //        string key = mykey[0];

        //        if (key == "OID" || key == "MyPK")
        //            continue;

        //        if (key == "FK_Dept")
        //        {
        //            this.DeptNo = mykey[1];
        //            continue;
        //        }

        //        bool isExist = false;
        //        bool IsInt = false;
        //        bool IsDouble = false;
        //        bool IsFloat = false;
        //        bool IsMoney = false;
        //        foreach (Attr attr in en.EnMap.Attrs)
        //        {
        //            if (attr.Key.Equals(key))
        //            {
        //                isExist = true;
        //                if (attr.MyDataType == DataType.AppInt)
        //                    IsInt = true;
        //                if (attr.MyDataType == DataType.AppDouble)
        //                    IsDouble = true;
        //                if (attr.MyDataType == DataType.AppFloat)
        //                    IsFloat = true;
        //                if (attr.MyDataType == DataType.AppMoney)
        //                    IsMoney = true;
        //                break;
        //            }
        //        }

        //        if (isExist == false)
        //            continue;

        //        if (mykey[1] == "mvals")
        //        {
        //            //如果用户多项选择了，就要找到它的选择项目.

        //            UserRegedit sUr = new UserRegedit();
        //            sUr.setMyPK(WebUser.No + this.EnsName + "_SearchAttrs";
        //            sUr.RetrieveFromDBSources();

        //            /* 如果是多选值 */
        //            string cfgVal = sUr.MVals;
        //            AtPara ap = new AtPara(cfgVal);
        //            string instr = ap.GetValStrByKey(key);
        //            string val = "";
        //            if (instr == null || instr == "")
        //            {
        //                if (key == "FK_Dept" || key == "FK_Unit")
        //                {
        //                    if (key == "FK_Dept")
        //                        val = WebUser.DeptNo;
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                instr = instr.Replace("..", ".");
        //                instr = instr.Replace(".", "','");
        //                instr = instr.Substring(2);
        //                instr = instr.Substring(0, instr.Length - 2);
        //                qo.AddWhereIn(mykey[0], instr);
        //            }
        //        }
        //        else
        //        {
        //            if (IsInt == true && DataType.IsNullOrEmpty(mykey[1]) == false)
        //                qo.AddWhere(mykey[0], Int32.Parse(mykey[1]));
        //            else if (IsDouble == true && DataType.IsNullOrEmpty(mykey[1]) == false)
        //                qo.AddWhere(mykey[0], double.Parse(mykey[1]));
        //            else if (IsFloat == true && DataType.IsNullOrEmpty(mykey[1]) == false)
        //                qo.AddWhere(mykey[0], float.Parse(mykey[1]));
        //            else if (IsMoney == true && DataType.IsNullOrEmpty(mykey[1]) == false)
        //                qo.AddWhere(mykey[0], decimal.Parse(mykey[1]));
        //            else
        //                qo.AddWhere(mykey[0], mykey[1]);
        //        }
        //        qo.addAnd();
        //    }

        //    if (this.DeptNo != null && (this.GetRequestVal("FK_Emp") == null
        //        || this.GetRequestVal("FK_Emp") == "all"))
        //    {
        //        if (this.DeptNo.Length == 2)
        //        {
        //            qo.AddWhere("FK_Dept", " = ", "all");
        //            qo.addAnd();
        //        }
        //        else
        //        {
        //            if (this.DeptNo.Length == 8)
        //            {
        //                qo.AddWhere("FK_Dept", " = ", this.DeptNo);
        //            }
        //            else
        //            {
        //                qo.AddWhere("FK_Dept", " like ", this.DeptNo + "%");
        //            }

        //            qo.addAnd();
        //        }
        //    }

        //    qo.AddHD();

        //    DataTable dt = qo.DoQueryToTable();

        //    string filePath = ExportDGToExcel(dt, en, en.EnDesc);


        //    return filePath;
        //}
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
            Entity en = ClassFactory.GetEn(this.EnName);
            try
            {
                string pkval = this.PKVal;

                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
                if (DataType.IsNullOrEmpty(pkval) == true || pkval.Equals("0") || pkval.Equals("undefined"))
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
                    //int i=en.RetrieveFromDBSources();
                    //if (i == 0)
                    //  return "err@实体:["+"]";
                }

                return en.ToJson(false);
            }
            catch (Exception ex)
            {
                en.CheckPhysicsTable();
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
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
                #region 首先判断参数删除.
                string key1 = this.GetRequestVal("Key1");
                string val1 = this.GetRequestVal("Val1");

                string key2 = this.GetRequestVal("Key2");
                string val2 = this.GetRequestVal("Val2");
                Attrs attrs = en.EnMap.Attrs;

                if (DataType.IsNullOrEmpty(key1) == false && key1.Equals("undefined") == false)
                {
                    int num = 0;
                    if (DataType.IsNullOrEmpty(key2) == false && key2.Equals("undefined") == false)
                    {
                        if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB)
                            num = en.Delete(key1, BP.Sys.Base.Glo.GenerRealType(attrs, key1, val1), key2, BP.Sys.Base.Glo.GenerRealType(attrs, key2, val2));
                        else
                            num = en.Delete(key1, val1, key2, val2);
                    }
                    else
                    {
                        if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB)
                            num = en.Delete(key1, BP.Sys.Base.Glo.GenerRealType(attrs, key1, val1));
                        else
                            num = en.Delete(key1, val1);
                    }
                    return num.ToString();
                }
                #endregion 首先判断参数删除.

                /* 不管是个主键，还是单个主键，都需要循环赋值。*/
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));

                if (en.PKCount != 1)
                {
                    int i = en.RetrieveFromDBSources(); //查询出来再删除.
                    return en.Delete().ToString(); //返回影响行数.
                }
                else
                {
                    string pkval = en.PKVal.ToString();
                    if (DataType.IsNullOrEmpty(pkval) == true)
                        en.PKVal = this.PKVal;

                    int num = en.RetrieveFromDBSources();
                    en.Delete();

                    return "删除成功.";
                    // int i = en.RetrieveFromDBSources(); //查询出来再删除.
                    //return en.Delete().ToString(); //返回影响行数.
                }


                // int i = en.RetrieveFromDBSources(); //查询出来再删除.
                //return en.Delete().ToString(); //返回影响行数.
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
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));

                //返回数据.
                //return en.ToJson(false);
                en.PKVal = this.PKVal;

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
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
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
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";

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
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";

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
                    return "err@实体类名错误[" + this.EnName + "].";

                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));
                }

                //保存参数属性.
                string frmParas = HttpUtility.UrlDecode(this.GetValFromFrmByKey("frmParas", ""));
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
                {
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));
                }

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

        /// <summary>
        /// 执行插入.
        /// </summary>
        /// <returns></returns>
        public string Entity_DirectInsert()
        {
            try
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                if (en == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));
                }

                //插入数据库.
                int i = en.DirectInsert();
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
        public static int isRuning = 0;
        public static string getMemory(object o) // 获取引用类型的内存地址方法  
        {
            GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);

            IntPtr addr = GCHandle.ToIntPtr(h);

            return "0x" + addr.ToString("X");
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Entity_DoMethodReturnString()
        {
            try
            {
                /**
                while (isRuning == 1)
                {
                    System.Threading.Thread.Sleep(100);
                }
                isRuning = 1;*/
                Entity currentThreadEn = ClassFactory.GetEn(this.EnName);
                /**
                Entity currentThreadEn = null;
                if(this.EnName == "BP.Sys.SFTable")
                {
                    currentThreadEn = new SFTable();
                }
                else
                {
                    currentThreadEn = ClassFactory.GetEn(this.EnName);
                } */
                System.Diagnostics.Debug.WriteLine("EnAddr:" + getMemory(currentThreadEn));
                //System.Diagnostics.Debug.WriteLine("EnInfo:", JsonConvert.SerializeObject(currentThreadEn));

                if (currentThreadEn == null)
                    return "err@类" + this.EnName + "不存在,请检查是不是拼写错误";
                currentThreadEn.PKVal = this.PKVal;
                currentThreadEn.RetrieveFromDBSources();

                string methodName = this.GetRequestVal("MethodName");

                //if ("GenerDataOfJson".Equals(methodName) == true)
                //{
                //  BP.DA.Log.DebugWriteInfo("ENS:" + en.ToJson() + ":Entity_DoMethodReturnString(methodName): " + methodName);
                //}


                Type tp = currentThreadEn.GetType();

                System.Reflection.MethodInfo mp = tp.GetMethod(methodName);
                if (mp == null)
                    return "err@没有找到类[" + this.EnName + "]方法[" + methodName + "].";
                string paras = this.GetRequestVal("paras");
                //执行该方法.
                object[] myparas = new object[0];

                if (DataType.IsNullOrEmpty(paras) == false)
                {
                    string[] str = paras.Split('~');
                    myparas = new object[str.Length];

                    int idx = 0;
                    ParameterInfo[] paramInfos = mp.GetParameters();
                    foreach (ParameterInfo paramInfo in paramInfos)
                    {
                        myparas[idx] = str[idx].Contains("`") == true ? str[idx].Replace("`", "~") : str[idx];
                        try
                        {
                            if (paramInfo.ParameterType.Name.Equals("Single"))
                                myparas[idx] = float.Parse(str[idx]);
                            if (paramInfo.ParameterType.Name.Equals("Double"))
                                myparas[idx] = double.Parse(str[idx]);
                            if (paramInfo.ParameterType.Name.Equals("Int32"))
                                myparas[idx] = Int32.Parse(str[idx]);
                            if (paramInfo.ParameterType.Name.Equals("Int64"))
                                myparas[idx] = Int64.Parse(str[idx]);
                            if (paramInfo.ParameterType.Name.Equals("Decimal"))
                                myparas[idx] = new Decimal(double.Parse(str[idx]));
                            if (paramInfo.ParameterType.Name.Equals("Boolean"))
                            {
                                if (str[idx].ToLower().Equals("true") || str[idx].Equals("1"))
                                    myparas[idx] = true;
                                else
                                    myparas[idx] = false;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("err@类[" + this.EnName + "]方法[" + methodName + "]值" + str[idx] + "转换成" + paramInfo.ParameterType.Name + "失败");
                        }
                        idx++;
                    }
                }

                string result = mp.Invoke(currentThreadEn, myparas) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。

                /** isRuning = 0; */
                return result;


            }
            catch (Exception ex)
            {
                /**isRuning = 0;*/
                throw ex;
            }
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
            try
            {
                Entities ens = ClassFactory.GetEns(this.EnsName);
                if (ens == null)
                    return "err@类" + this.EnsName + "不存在,请检查是不是拼写错误";
                ens.RetrieveAll();
                return ens.ToJson();
            }
            catch (Exception e)
            {
                return "err@[Entities_RetrieveAll][" + this.EnsName + "]类名错误，或者其他异常:" + e.Message;
            }
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
                if (ens == null)
                    return "err@类" + this.EnsName + "不存在,请检查是不是拼写错误";
                if (this.Paras == null)
                    return "0";
                return Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string Entities_Init_Ext(Entities ens, Entity en, string paras)
        {

            QueryObject qo = new QueryObject(ens);
            string[] myparas = this.Paras.Split('@');

            Attrs attrs = en.EnMap.Attrs;

            int idx = 0;
            for (int i = 0; i < myparas.Length; i++)
            {
                string para = myparas[i];
                if (DataType.IsNullOrEmpty(para) || para.Contains("=") == false)
                    continue;

                string[] strs = para.Split('=');
                string key = strs[0];
                string val = strs[1];


                if (key.ToLower().Equals("orderby") == true)
                {
                    //多重排序
                    if (val.IndexOf(",") != -1)
                    {
                        string[] strs1 = val.Split(',');
                        foreach (string str in strs1)
                        {
                            if (DataType.IsNullOrEmpty(str) == true)
                                continue;
                            if (str.ToUpper().IndexOf("DESC") != -1)
                            {
                                string str1 = str.Replace("DESC", "").Replace("desc", "");
                                qo.addOrderByDesc(str1.Trim());
                            }
                            else
                            {
                                if (str.ToUpper().IndexOf("ASC") != -1)
                                {
                                    string str1 = str.Replace("ASC", "").Replace("asc", "");
                                    qo.addOrderBy(str1.Trim());
                                }
                                else
                                {
                                    qo.addOrderBy(str.Trim());
                                }
                            }

                        }
                    }
                    else
                    {
                        qo.addOrderBy(val);
                    }

                    continue;
                }

                if (attrs.Contains(key) == false)
                    continue;

                object valObj = val;

                if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
                    valObj = BP.Sys.Base.Glo.GenerRealType(en.EnMap.Attrs, key, val);

                if (idx == 0)
                {
                    qo.AddWhere(key, valObj);
                }
                else
                {
                    qo.addAnd();
                    qo.AddWhere(key, valObj);
                }
                idx++;
            }
            try
            {
                qo.DoQuery();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("exist"))
                    qo.DoQuery();
            }
            return ens.ToJson();
        }

        /// <summary>
        /// 获得实体集合s
        /// </summary>
        /// <returns></returns>
        public string Entities_RetrieveCond()
        {
            try
            {
                Entities ens = ClassFactory.GetEns(this.EnsName);
                if (ens == null)
                    return "err@类" + this.EnsName + "不存在,请检查是不是拼写错误";
                if (this.Paras == null)
                    return "0";

                return Entities_RetrieveCond_Ext(ens, this.Paras);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string Entities_RetrieveCond_Ext(Entities ens, string paras)
        {

            QueryObject qo = new QueryObject(ens);
            string[] myparas = paras.Replace("[%]", "%").Split('@');

            Attrs attrs = ens.GetNewEntity.EnMap.Attrs;

            int idx = 0;
            for (int i = 0; i < myparas.Length; i++)
            {
                string para = myparas[i];
                if (DataType.IsNullOrEmpty(para))
                    continue;

                string[] strs = para.Split('|');
                string key = strs[0];
                string oper = strs[1];
                string val = strs[2];

                if (key.ToLower().Equals("orderby") == true)
                {
                    qo.addOrderBy(val);
                    continue;
                }

                //获得真实的数据类型.
                object typeVal = val;
                if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
                    typeVal = BP.Sys.Base.Glo.GenerRealType(attrs, key, val);

                string[] keys = key.Trim().Split(',');
                int count = 0;
                foreach (string str in keys)
                {
                    count++;
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    if (idx == 0 && count == 1)
                    {
                        qo.AddWhere(str, oper, typeVal);
                    }
                    else
                    {
                        if (count != 1)
                            qo.addOr();
                        else
                            qo.addAnd();
                        qo.AddWhere(str, oper, typeVal);
                    }

                }
                idx++;
            }

            qo.DoQuery();
            return ens.ToJson();
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        public string Entities_DoMethodReturnString()
        {

            //创建类实体.
            BP.En.Entities ens = ClassFactory.GetEns(this.EnsName);
            // Activator.CreateInstance(System.Type.GetType("BP.En.Entity")) as BP.En.Entity;

            string methodName = this.GetRequestVal("MethodName");
            if (ens == null)
                return "err@没有找到实体类";
            Type tp = ens.GetType();
            System.Reflection.MethodInfo mp = tp.GetMethod(methodName);
            if (mp == null)
                return "err@没有找到类[" + this.EnsName + "]方法[" + methodName + "].";

            string paras = this.GetRequestVal("paras");
            if ("un".Equals(paras) == true || "undefined".Equals(paras) == true)
                paras = "";

            //执行该方法.
            object[] myparas = new object[0];
            string atPara = GetRequestVal("atPara");

            if (DataType.IsNullOrEmpty(paras) == false)
            {
                string[] str = paras.Split('~');
                if (DataType.IsNullOrEmpty(atPara) == true)
                    myparas = new object[str.Length];
                else
                    myparas = new object[str.Length + 1];

                int idx = 0;
                ParameterInfo[] paramInfos = mp.GetParameters();
                foreach (ParameterInfo paramInfo in paramInfos)
                {
                    myparas[idx] = str[idx];
                    try
                    {
                        if (paramInfo.ParameterType.Name.Equals("Single"))
                            myparas[idx] = float.Parse(str[idx]);
                        if (paramInfo.ParameterType.Name.Equals("Double"))
                            myparas[idx] = double.Parse(str[idx]);
                        if (paramInfo.ParameterType.Name.Equals("Int32"))
                            myparas[idx] = Int32.Parse(str[idx]);
                        if (paramInfo.ParameterType.Name.Equals("Int64"))
                            myparas[idx] = Int64.Parse(str[idx]);
                        if (paramInfo.ParameterType.Name.Equals("Decimal"))
                            myparas[idx] = new Decimal(double.Parse(str[idx]));
                        if (paramInfo.ParameterType.Name.Equals("Boolean"))
                        {
                            if (str[idx].ToLower().Equals("true") || str[idx].Equals("1"))
                                myparas[idx] = true;
                            else
                                myparas[idx] = false;
                        }

                    }
                    catch (Exception e)
                    {
                        throw new Exception("err@类[" + this.EnName + "]方法[" + methodName + "]值" + str[idx] + "转换成" + paramInfo.ParameterType.Name + "失败");
                    }

                    idx++;
                }


            }

            if (DataType.IsNullOrEmpty(atPara) == false)
            {
                if (myparas.Length == 0)
                {
                    myparas = new object[1];
                    myparas[0] = atPara;
                }
                else
                    myparas[myparas.Length - 1] = atPara;
            }


            string result = mp.Invoke(ens, myparas) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。
            return result;

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
                ht.Add("Warning", rm.Warning == null ? "" : rm.Warning);
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
                                case DataType.AppString:
                                case DataType.AppDate:
                                case DataType.AppDateTime:
                                    string str1 = this.GetValFromFrmByKey(attr.Key);
                                    rm.SetValByKey(attr.Key, str1);
                                    break;
                                case DataType.AppInt:
                                    int myInt = this.GetValIntFromFrmByKey(attr.Key);  //int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.Row[idx] = myInt;
                                    rm.SetValByKey(attr.Key, myInt);
                                    break;
                                case DataType.AppFloat:
                                    float myFloat = this.GetValFloatFromFrmByKey(attr.Key); // float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myFloat);
                                    break;
                                case DataType.AppDouble:
                                case DataType.AppMoney:
                                    decimal myDoub = this.GetValDecimalFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myDoub);
                                    break;
                                case DataType.AppBoolean:
                                    bool myBool = this.GetValBoolenFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myBool);
                                    break;
                                default:
                                    return "err@没有判断的字段数据类型．";
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
                            bool myBoolval = this.GetValBoolenFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
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

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("GroupName", typeof(string));
            dt.Columns.Add("Icon", typeof(string));
            dt.Columns.Add("Note", typeof(string));

            DataRow dr;
            foreach (BP.En.Method en in al)
            {
                if (en.IsCanDo == false
                    || en.ItIsVisable == false)
                    continue;

                dr = dt.NewRow();
                dr["Name"] = en.ToString();
                dr["Title"] = en.Title;
                dr["GroupName"] = en.GroupName;
                dr["Icon"] = en.Icon;
                dr["Note"] = en.Help;
                dt.Rows.Add(dr);

            }

            return BP.Tools.Json.ToJson(dt);
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
            if (ens == null)
                return "err@类名:" + this.EnsName + "错误";

            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;

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
            ht.Add("EnName", en.ToString()); //类名?


            //把map信息放入
            ht.Add("PhysicsTable", map.PhysicsTable);
            ht.Add("CodeStruct", map.CodeStruct);
            //ht.Add("CodeLength", map.CodeLength);

            //查询条件.
            if (map.ItIsShowSearchKey == true)
                ht.Add("IsShowSearchKey", 1);
            else
                ht.Add("IsShowSearchKey", 0);

            ht.Add("SearchFields", map.SearchFields);
            ht.Add("SearchFieldsOfNum", map.SearchFieldsOfNum);

            //按日期查询.
            ht.Add("DTSearchWay", (int)map.DTSearchWay);
            ht.Add("DTSearchLabel", map.DTSearchLabel);
            ht.Add("DTSearchKey", map.DTSearchKey);

            //把实体类中的主键放在hashtable中
            ht.Add("EntityPK", en.PKField);

            //#region 把配置的信息增加里面去.
            //EnCfg cfg = new EnCfg();
            //cfg.No = this.EnsName;
            //if (cfg.RetrieveFromDBSources() == 0)
            //{
            //    cfg.Insert();
            //}
            //foreach (string key in cfg.Row.Keys)
            //{
            //    if (ht.ContainsKey(key) == true)
            //        continue;
            //    //设置值.
            //    ht.Add(key, cfg.GetValByKey(key));
            //}
            //#endregion 把配置的信息增加里面去. 

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
            if (ens == null)
                return "err@类名错误:" + this.EnsName;

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;

            DataSet ds = new DataSet();

            //构造查询条件集合.
            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.Columns.Add("Width");
            dt.Columns.Add("UIContralType");
            dt.Columns.Add("IsTree");
            dt.TableName = "Attrs";
            SearchFKEnums attrs = map.SearchFKEnums;
            Attr attr = null;
            foreach (SearchFKEnum item in attrs)
            {
                attr = item.HisAttr;
                DataRow dr = dt.NewRow();
                dr["Field"] = item.Key;
                dr["Name"] = item.HisAttr.Desc;
                dr["Width"] = item.Width; //下拉框显示的宽度.
                dr["UIContralType"] = (int)item.HisAttr.UIContralType;
                if (attr.ItIsFK && attr.HisFKEn.ItIsTreeEntity == true)
                {
                    if (attr.Key.Equals("FK_Dept") && WebUser.IsAdmin == false)
                        dr["IsTree"] = 0;
                    else
                        dr["IsTree"] = 1;
                }
                else
                    dr["IsTree"] = 0;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            //把外键枚举增加到里面.
            foreach (SearchFKEnum item in attrs)
            {
                attr = item.HisAttr;
                if (attr.ItIsEnum == true)
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    DataTable dtEnum = ses.ToDataTableField();
                    dtEnum.TableName = item.Key;
                    ds.Tables.Add(dtEnum);
                    continue;
                }

                if (attr.ItIsFK == true)
                {
                    Entities ensFK = attr.HisFKEns;
                    ensFK.RetrieveAll();

                    DataTable dtEn = ensFK.ToDataTableField();
                    dtEn.TableName = item.Key;
                    ds.Tables.Add(dtEn);
                    continue;
                }
                //绑定SQL的外键
                if (DataType.IsNullOrEmpty(attr.UIBindKey) == false
                    && ds.Tables.Contains(attr.Key) == false)
                {
                    //获取SQL
                    string sql = attr.UIBindKey;
                    if (attr.UIBindKey.Contains("SELECT") == false)
                    {
                        SFTable sf = new SFTable(attr.UIBindKey);
                        sql = sf.SelectStatement;
                    }

                    sql = BP.WF.Glo.DealExp(sql, null, null);
                    DataTable dtSQl = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataColumn col in dtSQl.Columns)
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
                            case "parentno":
                                col.ColumnName = "ParentNo";
                                break;
                            default:
                                break;
                        }
                    }
                    dtSQl.TableName = item.Key;
                    ds.Tables.Add(dtSQl);
                }

            }

            //获取查询条件的扩展属性
            MapExts exts = new MapExts(this.EnsName);
            if (exts.Count != 0)
                ds.Tables.Add(exts.ToDataTableField("Sys_MapExt"));


            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行查询 - 初始化查找数据
        /// </summary>
        /// <returns></returns>
        public string Search_SearchIt()
        {
            //取出来查询条件.
            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.Row = null;
            ur.setMyPK(WebUser.No + "_" + this.EnsName + "_SearchAttrs");
            ur.RetrieveFromDBSources();

            DataSet ds = new DataSet();
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = null;
            if (this.EnsName.IndexOf("TS.") == 0)
                map = en.EnMap;
            else
                map = en.EnMapInTime;


            MapAttrs attrs = new MapAttrs();

            MapData md = new MapData();
            md.No = this.EnsName;
            int count = md.RetrieveFromDBSources();
            if (count == 0)
                attrs = map.Attrs.ToMapAttrs;
            else
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.EnsName, MapAttrAttr.Idx);

            //根据设置的显示列显示字段
            DataRow row = null;
            DataTable dtAttrs = new DataTable("Attrs");
            dtAttrs.Columns.Add("KeyOfEn", typeof(string));
            dtAttrs.Columns.Add("Name", typeof(string));
            dtAttrs.Columns.Add("Width", typeof(int));
            dtAttrs.Columns.Add("UIContralType", typeof(int));
            dtAttrs.Columns.Add("IsRichText", typeof(int));
            dtAttrs.Columns.Add("MyDataType", typeof(int));
            foreach (MapAttr attr in attrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if ((count != 0 && DataType.IsNullOrEmpty(searchVisable)) || (count == 0 && attr.UIVisible == false))
                    continue;
                row = dtAttrs.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;
                row["IsRichText"] = attr.TextModel == 3 ? 1 : 0;
                row["MyDataType"] = attr.MyDataType;
                dtAttrs.Rows.Add(row);
            }

            ds.Tables.Add(dtAttrs); //把描述加入.

            md.Name = map.EnDesc;

            //附件类型.
            md.SetPara("BPEntityAthType", (int)map.HisBPEntityAthType);

            //获取实体类的主键
            md.SetPara("PK", en.PK);

            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            QueryObject qo = Search_Data(ens, en, map, ur);
            //获得行数.
            ur.SetPara("RecCount", qo.GetCount());
            ur.Save();

            //获取配置信息
            EnCfg encfg = new EnCfg();
            encfg.No = this.EnsName;
            encfg.RetrieveFromDBSources();

            string fieldSet = encfg.FieldSet;
            string oper = "";
            if (DataType.IsNullOrEmpty(fieldSet) == false)
            {
                string ptable = en.EnMap.PhysicsTable;
                DataTable dt = new DataTable("Search_HeJi");
                dt.Columns.Add("Field");
                dt.Columns.Add("Type");
                dt.Columns.Add("Value");
                DataRow dr;
                string[] strs = fieldSet.Split('@');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    string[] item = str.Split('=');
                    if (item.Length == 2)
                    {
                        if (item[1].Contains(",") == true)
                        {
                            string[] ss = item[1].Split(',');
                            foreach (string s in ss)
                            {
                                dr = dt.NewRow();
                                dr["Field"] = ((MapAttr)attrs.GetEntityByKey("KeyOfEn", s)).Name;
                                dr["Type"] = item[0];
                                dt.Rows.Add(dr);

                                oper += item[0] + "(" + ptable + "." + s + ")" + ",";
                            }
                        }
                        else
                        {
                            dr = dt.NewRow();
                            dr["Field"] = ((MapAttr)attrs.GetEntityByKey("KeyOfEn", item[1])).Name;
                            dr["Type"] = item[0];
                            dt.Rows.Add(dr);

                            oper += item[0] + "(" + ptable + "." + item[1] + ")" + ",";
                        }
                    }
                }
                oper = oper.Substring(0, oper.Length - 1);
                DataTable dd = qo.GetSumOrAvg(oper);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow ddr = dt.Rows[i];
                    ddr["Value"] = dd.Rows[0][i];
                }
                ds.Tables.Add(dt);
            }
            //增加排序
            string orderBy = "";
            bool isDesc = false;
            if (DataType.IsNullOrEmpty(ur.OrderBy) == false)
            {
                orderBy = ur.OrderBy;
                isDesc = ur.OrderWay.Equals("desc") == true ? true : false;
            }

            if (DataType.IsNullOrEmpty(ur.OrderBy) == true && encfg != null)
            {
                orderBy = encfg.GetValStrByKey("OrderBy");
                if (orderBy.IndexOf(",") != -1)
                {
                    string[] str = orderBy.Split(',');
                    orderBy = str[0];
                }
                isDesc = encfg.GetValBooleanByKey("IsDeSc");
            }

            if (DataType.IsNullOrEmpty(orderBy) == false)
            {
                try
                {
                    if (isDesc)
                        qo.addOrderByDesc(orderBy);
                    else
                        qo.addOrderBy(orderBy);
                }
                catch (Exception ex)
                {
                    encfg.SetValByKey("OrderBy", orderBy);
                }
            }

            //if (GetRequestVal("DoWhat") != null && GetRequestVal("DoWhat").Equals("Batch"))
            //    qo.DoQuery(en.PK, 500, 1);
            // else
            qo.DoQuery(en.PK, this.PageSize, this.PageIdx);
            #endregion 获得查询数据.

            DataTable mydt = ens.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.

            #region 获得方法的集合
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
            dtM.Columns.Add("ClassMethodName");
            dtM.Columns.Add("IsShowForEnsCondtion");
            dtM.Columns.Add("IsHaveFuncPara");

            RefMethods rms = map.HisRefMethods;
            foreach (RefMethod item in rms)
            {
                if (item.ItIsForEns == false)
                    continue;

                if (item.Visable == false)
                    continue;

                string myurl = "";

                myurl = "RefMethod.htm?Index=" + item.Index + "&EnName=" + en.ToString() + "&EnsName=" + en.GetNewEntities.ToString() + "&PKVal=";

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
                dr["IsCanBatch"] = item.ItIsCanBatch;
                dr["GroupName"] = item.GroupName;
                dr["ClassMethodName"] = item.ClassMethodName;
                dr["IsShowForEnsCondtion"] = item.IsShowForEnsCondtion;
                dr["IsHaveFuncPara"] = item.HisAttrs.Count == 0 ? 0 : 1;

                dtM.Rows.Add(dr); //增加到rows.
            }
            ds.Tables.Add(dtM); //把数据加入里面.
            #endregion

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行查询.这个方法也会被导出调用.
        /// </summary>
        /// <returns></returns>
        public QueryObject Search_Data(Entities ens, Entity en, Map map, UserRegedit ur)
        {

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);

            //关键字.
            string keyWord = ur.SearchKey;
            QueryObject qo = new QueryObject(ens);
            bool isFirst = true; //是否第一次拼接SQL
            Attrs attrs = map.Attrs;
            
            #region 关键字字段.
            if (DataType.IsNullOrEmpty(map.SearchFields) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFields = map.SearchFields.Split('@');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    field = str.Split('=')[1];
                    if (DataType.IsNullOrEmpty(field) == true)
                        continue;

                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    fieldValue = fieldValue.Trim();
                    fieldValue = fieldValue.Replace(",", ";").Replace(" ", ";");
                    string[] fieldValues = fieldValue.Split(';');
                    int valIdx = 0;
                    idx++;
                    foreach (String val in fieldValues)
                    {
                        valIdx++;

                        if (idx == 1 && valIdx == 1)
                        {
                            isFirst = false;
                            /* 第一次进来。 */
                            qo.addLeftBracket();
                            if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + ",'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + "+'%'"));
                            else
                                qo.AddWhere(field, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + "||'%'");
                            qo.MyParas.Add(field + valIdx, val);

                            if (valIdx == fieldValues.Length)
                                qo.addRightBracket();

                            continue;
                        }
                        if (valIdx == 1 && idx != 1)
                        {
                            qo.addAnd();
                            qo.addLeftBracket();
                        }
                        else
                            qo.addOr();

                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + ",'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + valIdx + "||'%'");
                        qo.MyParas.Add(field + valIdx, val);

                        if (valIdx == fieldValues.Length)
                            qo.addRightBracket();
                    }
                }

            }
            else
            {

                if (en.EnMap.ItIsShowSearchKey && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
                {
                    Attr attrPK = new Attr();
                    foreach (Attr attr in attrs)
                    {
                        if (attr.ItIsPK)
                        {
                            attrPK = attr;
                            break;
                        }
                    }
                    int i = 0;
                    string enumKey = ","; //求出枚举值外键.
                    keyWord = keyWord.Replace(",", ";").Replace(" ", ";");
                    string[] strVals = keyWord.Split(';');
                    if (strVals.Length > 1)
                    {
                        //判断是否存在SKeWord
                        Attr keyAttr = attrs.GetAttrByKeyOfEn("SKeyWords");
                        if (keyAttr == null)
                            throw new Exception("err@没有关键字SKeyWords不能按照多关键字查询");
                        foreach (string val in strVals)
                        {
                            i++;
                            if (i == 1)
                            {
                                isFirst = false;
                                /* 第一次进来。 */
                                qo.addLeftBracket();
                                if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                    qo.AddWhere("SKeyWords", " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + ", '%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + "+'%'"));
                                else
                                    qo.AddWhere("SKeyWords", " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + "|| '%'");

                                qo.MyParas.Add("SKeyWords" + i, val);

                                continue;
                            }
                            qo.addAnd();

                            if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                qo.AddWhere("SKeyWords", " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + ", '%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + "+'%'"));
                            else
                                qo.AddWhere("SKeyWords", " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKeyWords" + i + "|| '%'");

                            qo.MyParas.Add("SKeyWords" + i, val);
                        }
                    }
                    else
                    {
                        foreach (Attr attr in map.Attrs)
                        {
                            switch (attr.MyFieldType)
                            {
                                case FieldType.Enum:
                                    enumKey = "," + attr.Key + "Text,";
                                    break;
                                case FieldType.FK:
                                    //enumKey = "," + attr.Key + "Text,";
                                    // case FieldType.PKFK:
                                    continue;
                                default:
                                    break;
                            }

                            if (attr.MyDataType != DataType.AppString)
                                continue;

                            //排除枚举值关联refText.
                            if (attr.MyFieldType == FieldType.RefText)
                            {
                                if (enumKey.Contains("," + attr.Key + ",") == true)
                                    continue;
                            }

                            if (attr.Key == "FK_Dept")
                                continue;
                            int valIdx = 0;
                            foreach (string val in strVals)
                            {
                                i++;
                                valIdx++;
                                if (i == 1)
                                {
                                    isFirst = false;
                                    /* 第一次进来。 */
                                    qo.addLeftBracket();
                                    if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                        qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + ", '%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + "+'%'"));
                                    else
                                        qo.AddWhere(attr.Key, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + "|| '%'");

                                    qo.MyParas.Add("SKey" + valIdx, val);

                                    continue;
                                }
                                qo.addOr();

                                if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                    qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + ", '%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + "+'%'"));
                                else
                                    qo.AddWhere(attr.Key, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey" + valIdx + "|| '%'");

                                qo.MyParas.Add("SKey" + valIdx, val);
                            }

                        }
                    }
                    qo.addRightBracket();

                }

            }

            #endregion

            #region 增加数值型字段的查询
            if (DataType.IsNullOrEmpty(map.SearchFieldsOfNum) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFieldsOfNum = map.SearchFieldsOfNum.Split('@');
                foreach (String str in searchFieldsOfNum)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    field = str.Split('=')[1];
                    if (DataType.IsNullOrEmpty(field) == true)
                        continue;

                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    string[] strVals = fieldValue.Split(',');

                    //判断是否是第一次进入
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();
                    if (DataType.IsNullOrEmpty(strVals[0]) == false)
                    {

                        if (DataType.IsNullOrEmpty(strVals[1]) == true)
                            qo.AddWhere(field, ">=", strVals[0]);
                        else
                        {
                            qo.AddWhere(field, ">=", strVals[0], field + "1");
                            qo.addAnd();
                            qo.AddWhere(field, "<=", strVals[1], field + "2");
                        }

                    }
                    else
                    {
                        qo.AddWhere(field, "<=", strVals[1]);
                    }

                    qo.addRightBracket();

                }


            }
            #endregion

            if (map.DTSearchWay != DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                if (map.DTSearchWay == DTSearchWay.ByYearMonth || map.DTSearchWay == DTSearchWay.ByYear)
                {
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.AddWhere(map.DTSearchKey, dtFrom);
                }


                if (DataType.IsNullOrEmpty(dtTo) == true)
                    dtTo = DataType.CurrentDate;


                //按日期查询
                if (map.DTSearchWay == DTSearchWay.ByDate)
                {
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    if (DataType.IsNullOrEmpty(dtFrom) == true)
                    {
                        qo.addLeftBracket();
                        qo.SQL = map.PhysicsTable + "." + map.DTSearchKey + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }
                    else
                    {

                        qo.addLeftBracket();
                        dtTo += " 23:59:59";
                        qo.SQL = map.PhysicsTable + "." + map.DTSearchKey + " >= '" + dtFrom + "'";
                        qo.addAnd();
                        qo.SQL = map.PhysicsTable + "." + map.DTSearchKey + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }
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
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    if (DataType.IsNullOrEmpty(dtFrom) == true)
                    {
                        qo.addLeftBracket();
                        qo.SQL = map.DTSearchKey + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }
                    else
                    {
                        qo.addLeftBracket();
                        qo.SQL = map.DTSearchKey + " >= '" + dtFrom + "'";
                        qo.addAnd();
                        qo.SQL = map.DTSearchKey + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }

                }
            }

            List<string> keys = new List<string>();

            #region 普通属性
            string opkey = ""; // 操作符号。
            foreach (SearchNormal attr in en.EnMap.SearchNormals)
            {
                if (attr.ItIsHidden)
                {
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();
                    if (attr.DefaultSymbol.Equals("exp") == true)
                    {
                        qo.addSQL(BP.WF.Glo.DealExp(attr.RefAttrKey, null));
                        qo.addRightBracket();
                        continue;

                    }
                    //如果传参上有这个值的查询
                    string val = this.GetRequestVal(attr.RefAttrKey);
                    if (DataType.IsNullOrEmpty(val) == false)
                    {
                        attr.DefaultSymbol = "=";
                        attr.DefaultVal = val;
                    }

                    //获得真实的数据类型.
                    if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true && (attr.DefaultSymbol.Equals("=") || attr.DefaultSymbol.Equals("!=")))
                    {
                        object valType = BP.Sys.Base.Glo.GenerRealType(en.EnMap.Attrs,
                            attr.RefAttrKey, attr.DefaultValRun);
                        qo.AddWhere(attr.RefAttrKey, attr.DefaultSymbol, valType);
                    }
                    else
                    {
                        qo.AddWhere(attr.RefAttrKey, attr.DefaultSymbol, attr.DefaultValRun);
                    }
                    qo.addRightBracket();
                    if (keys.Contains(attr.RefAttrKey) == false)
                        keys.Add(attr.RefAttrKey);
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

                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;
                qo.addLeftBracket();

                if (attr.DefaultVal.Length >= 8)
                {
                    string date = "2005-09-01";
                    try
                    {
                        /* 就可能是年月日。 */
                        string y = ap.GetValStrByKey("DDL_" + attr.Key + "_Year");
                        string m = ap.GetValStrByKey("DDL_" + attr.Key + "_Month");
                        string d = ap.GetValStrByKey("DDL_" + attr.Key + "_Day");
                        date = y + "-" + m + "-" + d;

                        if (opkey == "<=")
                        {
                            DateTime dt = DataType.ParseSysDate2DateTime(date).AddDays(1);
                            date = DataType.SysDataFormat(dt);
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
                if (keys.Contains(attr.RefAttrKey) == false)
                    keys.Add(attr.RefAttrKey);
            }
            #endregion

            #region 获得查询数据.
            foreach (string str in ap.HisHT.Keys)
            {
                if (keys.Contains(str) == false)
                    keys.Add(str);

                string val = ap.GetValStrByKey(str);
                if (DataType.IsNullOrEmpty(val) == true || val.Equals("null") == true)
                    val = "all";
                if (val.Equals("all"))
                    continue;

                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;
                isFirst = false;
                qo.addLeftBracket();
                Attr attr = attrs.GetAttrByKeyOfEn(str);
                if (attr != null && attr.ItIsFK && attr.UIBindKey.Contains(",TS.") == false
                    && attr.HisFKEn.ItIsTreeEntity == true && !(attr.Key.Equals("FK_Dept") && WebUser.IsAdmin == false))
                {
                    //需要获取当前数据选中的数据和子级(先阶段只处理部门信息)
                    DataTable dt = null;
                    try
                    {
                        dt = DBAccess.RunSQLReturnTable(BP.WF.Dev2Interface.GetDeptNoSQLByParentNo(val, attr.HisFKEn.EnMap.PhysicsTable));
                    }
                    catch (Exception ex)
                    {
                        if (SystemConfig.AppCenterDBType == DBType.MySQL)
                            throw new Exception("err@请在web.config中数据库连接配置中增加Allow User Variables=True;");
                        throw new Exception(ex.Message);
                    }
                    if (dt.Rows.Count == 0)
                        qo.AddWhere(attr.Key, val);
                    else
                        qo.AddWhereIn(attr.Key, dt);
                    qo.addRightBracket();
                    continue;
                }
                //多选
                if (val.IndexOf(",") != -1)
                {
                    if (attr.ItIsNum == true)
                    {
                        qo.AddWhere(str, "IN", "(" + val + ")");
                        qo.addRightBracket();
                        continue;
                    }
                    val = "('" + val.Replace(",", "','") + "')";
                    qo.AddWhere(str, "IN", val);
                    qo.addRightBracket();
                    continue;
                }

                object valType = BP.Sys.Base.Glo.GenerRealType(attrs,
                    str, val);
                qo.AddWhere(str, valType);
                qo.addRightBracket();
            }

            foreach (Attr attr in map.Attrs)
            {
                /*if (1 == 1)
                    continue;*/

                string val = HttpContextHelper.RequestParams(attr.Field);
                if (DataType.IsNullOrEmpty(val))
                    continue;
                if (keys.Contains(attr.Field))
                    continue;
                if (attr.Field.Equals("Token"))
                    continue;
                if (attr.Field.Equals("No"))
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppBoolean:
                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addLeftBracket();
                        qo.AddWhere(attr.Field, Convert.ToBoolean(int.Parse(val)));
                        qo.addRightBracket();
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                    case DataType.AppString:
                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addLeftBracket();
                        qo.AddWhere(attr.Field, val);
                        qo.addRightBracket();
                        break;
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addLeftBracket();
                        qo.AddWhere(attr.Field, double.Parse(val));
                        qo.addRightBracket();
                        break;
                    case DataType.AppInt:
                        if (val == "all" || val == "-1")
                            continue;
                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false; ;
                        qo.addLeftBracket();
                        qo.AddWhere(attr.Field, int.Parse(val));
                        qo.addRightBracket();
                        break;
                    default:
                        break;
                }
                if (keys.Contains(attr.Field) == false)
                    keys.Add(attr.Field);
            }

            return qo;

        }
        private DataTable SearchDtl_Data(Entities ens, Entity en, string workId, string fid)
        {
            //获得.
            Map map = en.EnMapInTime;

            MapAttrs attrs = map.Attrs.ToMapAttrs;

            QueryObject qo = new QueryObject(ens);

            qo.AddWhere("RefPK", "=", workId);
            //qo.addAnd();
            //qo.AddWhere("FID", "=", fid);

            #endregion 获得查询数据.

            return qo.DoQueryToTable();
        }

        public string Search_GenerPageIdx()
        {

            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.setMyPK(WebUser.No + "_" + this.EnsName + "_SearchAttrs");
            ur.RetrieveFromDBSources();

            string url = "?EnsName=" + this.EnsName;
            int pageSpan = 10;
            int recNum = ur.GetParaInt("RecCount"); //获得查询数量.
            int pageSize = 12;
            if (recNum <= pageSize)
                return "1";

            string html = "";
            html += "<ul class='pagination'>";

            string appPath = ""; // this.Request.ApplicationPath;
            int myidx = 0;
            if (PageIdx <= 1)
            {
                html += "<li><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/LeftEnd.png' border=0/><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Left.png' border=0/></li>";
            }
            else
            {
                myidx = PageIdx - 1;
                //this.Add("<a href='" + url + "&PageIdx=1' >《-</a> <a href='" + url + "&PageIdx=" + myidx + "'>《-</a>");
                html += "<li><a href='" + url + "&PageIdx=1' ><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/LeftEnd.png' border=0/></a><a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Left.png' border=0/></a></li>";
            }


            //分页采用java默认方式分页，采用bigdecimal分页报错
            int pageNum = 0;
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
            to = from + pageSpan;
            for (int i = 1; i <= pageNum; i++)
            {
                if (i >= from && i <= to)
                {
                    if (PageIdx == i)
                        html += "<li class='active' ><a href='#'><b>" + i + "</b></a></li>";
                    else
                        html += "<li><a href='" + url + "&PageIdx=" + i + "'>" + i + "</a></li>";
                }
            }

            if (PageIdx != pageNum)
            {
                myidx = PageIdx + 1;
                html += "<li><a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Right.png' border=0/></a>&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/RightEnd.png' border=0/></a> &nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum + "</li>";
            }
            else
            {
                html += "<li><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/Right.png' border=0/></li>";
                html += "<li><img style='vertical-align:middle' src='" + Glo.CCFlowAppPath + "WF/Img/Arr/RightEnd.png' border=0/>&nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum + "</li>";
            }
            html += "</ul>";
            return html;
        }
        /// <summary>
        /// 执行导出
        /// </summary>
        /// <returns></returns>
        public string Search_Exp()
        {
            //取出来查询条件.
            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.setMyPK(WebUser.No + "_" + this.EnsName + "_SearchAttrs");
            ur.RetrieveFromDBSources();
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            QueryObject qo = Search_Data(ens, en, en.EnMap, ur);
            EnCfg encfg = new EnCfg();
            encfg.No = this.EnsName;
            //增加排序
            if (encfg.RetrieveFromDBSources() != 0)
            {
                string orderBy = encfg.GetValStrByKey("OrderBy");
                bool isDesc = encfg.GetValBooleanByKey("IsDeSc");

                if (DataType.IsNullOrEmpty(orderBy) == false)
                {
                    try
                    {
                        if (isDesc)
                            qo.addOrderByDesc(orderBy);
                        else
                            qo.addOrderBy(orderBy);
                    }
                    catch (Exception ex)
                    {
                        encfg.SetValByKey("OrderBy", orderBy);
                    }
                }
            }
            if (encfg.RetrieveFromDBSources() != 0)
                qo.addOrderBy(en.PK);
            qo.DoQuery();
            return BP.Tools.Json.ToJson(ens.ToDataTableField());

        }
        /// <summary>
        /// 从表执行导出
        /// </summary>
        /// <returns></returns>
        public string SearchDtl_Exp()
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;

            string workId = this.GetRequestVal("WorkId");
            string fid = this.GetRequestVal("FID");
            string name = "从表数据导出";
            string filename = name + "_" + DataType.CurrentDateTimeCNOfLong + "_" + WebUser.Name + ".xls";
            string filePath = BP.Tools.ExportExcelUtil.ExportDGToExcel(SearchDtl_Data(ens, en, workId, fid), en, name);

            return filePath;
        }


        #region Refmethod.htm 相关功能.
        public string Refmethod_Init()
        {
            string ensName = this.EnsName;
            int index = this.Index;
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];

            string pk = this.PKVal;
            if (pk == null)
                pk = this.GetRequestVal(en.PK);

            if (pk == null)
                pk = this.PKVal;

            if (pk == null)
                return "err@错误pkval 没有值。";

            en.PKVal = pk;
            en.RetrieveFromDBSources();

            //获取主键集合
            string[] pks = pk.Split(',');

            #region 处理无参数的方法.
            if (rm.HisAttrs == null || rm.HisAttrs.Count == 0)
            {
                string infos = "";
                int count = 0;
                int sucCount = 0;
                int errCount = 0;
                if (pks.Length == 1)
                {
                    rm.HisEn = en;

                    // 如果是link.
                    if (rm.RefMethodType == RefMethodType.LinkModel
                        || rm.RefMethodType == RefMethodType.LinkeWinOpen
                        || rm.RefMethodType == RefMethodType.RightFrameOpen)
                    {
                        string url = rm.Do(null) as string;
                        if (DataType.IsNullOrEmpty(url))
                            return "err@应该返回的url.";
                        return "url@" + url;
                    }

                    object obj = rm.Do(null);
                    if (obj == null)
                        return "close@info";

                    string result = obj.ToString();
                    if (result.IndexOf("url@") != -1 || result.IndexOf("err@") != -1)
                        return result;

                    result = "info@" + result;
                    return result;
                }
                foreach (string mypk in pks)
                {
                    if (DataType.IsNullOrEmpty(mypk) == true)
                        continue;
                    count++;
                    en.PKVal = mypk;
                    en.RetrieveFromDBSources();
                    rm.HisEn = en;

                    // 如果是link.
                    if (rm.RefMethodType == RefMethodType.LinkModel
                        || rm.RefMethodType == RefMethodType.LinkeWinOpen
                        || rm.RefMethodType == RefMethodType.RightFrameOpen)
                    {
                        string url = rm.Do(null) as string;
                        if (DataType.IsNullOrEmpty(url))
                        {
                            infos += "err@应该返回的url.";
                            break;
                        }

                        infos += "url@" + url;
                        break;
                    }

                    object obj = rm.Do(null);
                    if (obj == null)
                    {
                        infos += "close@info";
                        break;
                    }

                    string result = obj.ToString();
                    if (result.IndexOf("url@") != -1)
                    {
                        infos += result;
                        break;
                    }
                    if (result.IndexOf("err@") != -1)
                        errCount++;
                    else
                        sucCount++;
                    result = result.Replace("err@", "");
                    infos += "close@" + result + "<br/>";
                }
                if (pk.IndexOf(",") != -1)
                    infos = "一共选择" + count + "笔数据,其中[" + sucCount + "]执行成功,[" + errCount + "]执行失败.<br/>" + infos;
                return infos;
            }
            #endregion 处理无参数的方法.

            DataSet ds = new DataSet();

            //转化为json 返回到前台解析. 处理有参数的方法.
            Attrs attrs = rm.HisAttrs;
            MapAttrs mapAttrs = rm.HisAttrs.ToMapAttrs;

            //属性.
            DataTable attrDt = mapAttrs.ToDataTableField("Sys_MapAttrs");
            ds.Tables.Add(attrDt);

            #region 该方法的默认值.
            DataTable dtMain = new DataTable();
            dtMain.TableName = "MainTable";
            foreach (MapAttr attr in mapAttrs)
            {
                dtMain.Columns.Add(attr.KeyOfEn, typeof(string));
            }

            DataRow mydrMain = dtMain.NewRow();
            foreach (MapAttr item in mapAttrs)
            {
                string v = item.DefValReal;
                if (v.IndexOf('@') == -1)
                {
                    if (en.Row.ContainsKey(item.KeyOfEn) == true)
                        mydrMain[item.KeyOfEn] = en.GetValByKey(item.KeyOfEn);
                    else
                        mydrMain[item.KeyOfEn] = item.DefValReal;
                }

                //替换默认值的@的
                else
                {
                    if (v.Equals("@WebUser.No"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.No;
                    else if (v.Equals("@WebUser.Name"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.Name;
                    else if (v.Equals("@WebUser.FK_Dept"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.DeptNo;
                    else if (v.Equals("@WebUser.FK_DeptName"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.DeptName;
                    else if (v.Equals("@WebUser.FK_DeptNameOfFull") || v.Equals("@WebUser.FK_DeptFullName"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.DeptNameOfFull;
                    else if (v.Equals("@RDT"))
                    {
                        if (item.MyDataType == DataType.AppDate)
                            mydrMain[item.KeyOfEn] = DataType.CurrentDate;
                        if (item.MyDataType == DataType.AppDateTime)
                            mydrMain[item.KeyOfEn] = DataType.CurrentDateTime;
                    }
                    else
                    {
                        //如果是EnsName中字段
                        if (en.GetValByKey(v.Replace("@", "")) != null)
                            mydrMain[item.KeyOfEn] = en.GetValByKey(v.Replace("@", "")).ToString();

                    }


                }

            }
            dtMain.Rows.Add(mydrMain);
            ds.Tables.Add(dtMain);
            #endregion 该方法的默认值.

            #region 加入该方法的外键.
            foreach (DataRow dr in attrDt.Rows)
            {
                string lgType = dr["LGType"].ToString();
                if (lgType.Equals("2") == false)
                    continue;

                string UIIsEnable = dr["UIVisible"].ToString();
                if (UIIsEnable == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();
                if (ds.Tables.Contains(uiBindKey) == false)
                {
                    ds.Tables.Add(BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }

            }

            //加入sql模式的外键.
            foreach (Attr attr in attrs)
            {
                if (attr.ItIsRefAttr == true)
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

                    DataTable dt1 = DBAccess.RunSQLReturnTable(sqlBindKey);
                    dt1.TableName = attr.Key;

                    //@杜. 翻译当前部分.
                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                    {
                        dt1.Columns["NO"].ColumnName = "No";
                        dt1.Columns["NAME"].ColumnName = "Name";
                    }
                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                    {
                        dt1.Columns["no"].ColumnName = "No";
                        dt1.Columns["name"].ColumnName = "Name";
                    }

                    if (ds.Tables.Contains(attr.Key) == false)
                    {
                        ds.Tables.Add(dt1);
                    }

                }
            }

            #endregion 加入该方法的外键.

            #region 加入该方法的枚举.
            DataTable dtEnum = new DataTable();
            dtEnum.Columns.Add("Lab", typeof(string));
            dtEnum.Columns.Add("EnumKey", typeof(string));
            dtEnum.Columns.Add("IntKey", typeof(string));
            dtEnum.TableName = "Sys_Enum";

            foreach (Attr item in attrs)
            {
                if (item.ItIsEnum == false)
                    continue;

                SysEnums ses = new SysEnums(item.UIBindKey, item.UITag);
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
            dt.Columns.Add("Title", typeof(string));
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

        public string Ens_Init()
        {
            //定义容器.
            DataSet ds = new DataSet();

            //查询出来从表数据.
            Entities dtls = ClassFactory.GetEns(this.EnsName);
            dtls.RetrieveAll();
            Entity en = dtls.GetNewEntity;
            //QueryObject qo = new QueryObject(dtls);
            //qo.addOrderBy(en.PK);
            //qo.DoQuery();
            ds.Tables.Add(dtls.ToDataTableField("Ens"));

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

            #region 判断主键是否为自增长
            if (en.ItIsNoEntity == true && en.EnMap.ItIsAutoGenerNo)
                md.SetPara("IsNewRow", "0");
            else
                md.SetPara("IsNewRow", "1");
            #endregion

            #region 添加EN的主键
            md.SetPara("PK", en.PK);

            #endregion

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

                if (DataType.IsNullOrEmpty(uiBindKey) == true || mapAttr.UIIsEnable == false)
                    continue;

                // 判断是否存在.
                if (ds.Tables.Contains(uiBindKey) == true)
                    continue;
                if (uiBindKey.ToUpper().Trim().StartsWith("SELECT") == true)
                {
                    string sqlBindKey = BP.WF.Glo.DealExp(uiBindKey, en, null);

                    DataTable dt = DBAccess.RunSQLReturnTable(sqlBindKey);
                    dt.TableName = mapAttr.KeyOfEn;
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
                    continue;
                }

                if (mapAttr.LGType != FieldTypeS.FK)
                    continue;

                ds.Tables.Add(BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey));
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

                string sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ")";
                DataTable dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);

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

            return BP.Tools.Json.ToJson(ds);
        }

        #region 实体集合的保存.
        /// <summary>
        /// 实体集合的删除
        /// </summary>
        /// <returns></returns>
        public string Entities_Delete()
        {

            if (this.Paras == null)
                return "err@删除实体，参数不能为空";
            string[] myparas = this.Paras.Split('@');

            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类" + this.EnsName + "不存在,请检查是不是拼写错误";
            return Entities_Delete_Ext(ens);
        }
        public string Entities_Delete_Ext(Entities ens)
        {
            try
            {
                string[] myparas = this.Paras.Split('@');
                List<string[]> paras = new List<string[]>();
                int idx = 0;
                for (int i = 0; i < myparas.Length; i++)
                {
                    string para = myparas[i];
                    if (DataType.IsNullOrEmpty(para) || para.Contains("=") == false)
                        continue;

                    string[] strs = para.Split('=');
                    paras.Add(strs);
                }

                if (paras.Count == 1)
                    ens.Delete(paras[0][0], paras[0][1]);

                if (paras.Count == 2)
                    ens.Delete(paras[0][0], paras[0][1], paras[1][0], paras[1][1]);

                if (paras.Count == 3)
                    ens.Delete(paras[0][0], paras[0][1], paras[1][0], paras[1][1], paras[2][0], paras[2][1]);

                if (paras.Count == 4)
                    ens.Delete(paras[0][0], paras[0][1], paras[1][0], paras[1][1], paras[2][0], paras[2][1], paras[3][0], paras[3][1]);

                if (paras.Count > 4)
                    return "err@实体类的删除，条件不能大于4个";

                return "删除成功";
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
        public string Entities_Save()
        {
            try
            {
                #region  查询出来s实体数据.
                Entities dtls = BP.En.ClassFactory.GetEns(this.EnsName);
                if (dtls == null)
                    return "err@类" + this.EnsName + "不存在,请检查是不是拼写错误";

                dtls.RetrieveAll();
                Entity en = dtls.GetNewEntity;

                Map map = en.EnMap;
                foreach (Entity item in dtls)
                {
                    string pkval = item.PKVal.ToString();

                    foreach (Attr attr in map.Attrs)
                    {
                        if (attr.ItIsRefAttr == true)
                            continue;
                        if (attr.UIVisible == false || attr.UIIsReadonly == true)
                            continue;
                        string key = pkval + "_" + attr.Key;
                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {
                            string val = this.GetValFromFrmByKey("TB_" + key, null);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }


                        if (attr.UIContralType == UIContralType.TB)
                        {
                            string val = this.GetValFromFrmByKey("TB_" + key, null);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.DDL)
                        {
                            string val = this.GetValFromFrmByKey("DDL_" + key);
                            item.SetValByKey(attr.Key, val);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == false)
                        {
                            string val = this.GetValFromFrmByKey("CB_" + key, "-1");
                            if (val == "-1")
                                item.SetValByKey(attr.Key, 0);
                            else
                                item.SetValByKey(attr.Key, 1);
                            continue;
                        }
                    }

                    item.Update(); //执行更新.
                }
                #endregion  查询出来实体数据.

                #region 保存新加行.
                string strs = this.GetRequestVal("NewPKVals");
                //没有新增行
                if (this.GetRequestValBoolen("InsertFlag") == false || (en.EnMap.ItIsAutoGenerNo == true && DataType.IsNullOrEmpty(strs) == true))
                    return "更新成功.";

                string valValue = "";
                string[] pkVals = strs.Split(',');
                foreach (string pkval in pkVals)
                {
                    if (DataType.IsNullOrEmpty(pkval) == true)
                        continue;
                    foreach (Attr attr in map.Attrs)
                    {

                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {
                            if (attr.UIIsReadonly == false)
                                continue;

                            valValue = this.GetValFromFrmByKey("TB_" + pkval + "_" + attr.Key, null);
                            en.SetValByKey(attr.Key, valValue);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.TB && attr.UIIsReadonly == false)
                        {
                            valValue = this.GetValFromFrmByKey("TB_" + pkval + "_" + attr.Key);
                            en.SetValByKey(attr.Key, valValue);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == false)
                        {
                            valValue = this.GetValFromFrmByKey("DDL_" + pkval + "_" + attr.Key);
                            en.SetValByKey(attr.Key, valValue);
                            continue;
                        }

                        if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == false)
                        {
                            valValue = this.GetValFromFrmByKey("CB_" + pkval + "_" + attr.Key, "-1");
                            if (valValue == "-1")
                                en.SetValByKey(attr.Key, 0);
                            else
                                en.SetValByKey(attr.Key, 1);
                            continue;
                        }
                    }

                    if (en.ItIsNoEntity)
                    {
                        if (en.EnMap.ItIsAutoGenerNo)
                            en.SetValByKey("No", en.GenerNewNoByKey("No"));
                    }

                    try
                    {
                        if (en.PKVal.ToString() == "0")
                        {
                        }
                        else
                        {
                            en.Insert();
                        }
                    }
                    catch (Exception ex)
                    {
                        //异常处理..
                        BP.DA.Log.DebugWriteError(ex.Message);
                        return ex.Message;
                    }

                }



                #endregion 保存新加行.

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 获取批处理的方法.
        public string Refmethod_BatchInt()
        {
            string ensName = this.EnsName;
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            BP.En.RefMethods rms = en.EnMap.HisRefMethods;
            DataTable dt = new DataTable();
            dt.TableName = "RM";
            dt.Columns.Add("No");
            dt.Columns.Add("Title");
            dt.Columns.Add("Tip");
            dt.Columns.Add("Visable");

            dt.Columns.Add("Url");
            dt.Columns.Add("Target");
            dt.Columns.Add("Warning");
            dt.Columns.Add("RefMethodType");
            dt.Columns.Add("GroupName");
            dt.Columns.Add("W");
            dt.Columns.Add("H");
            dt.Columns.Add("Icon");
            dt.Columns.Add("IsCanBatch");
            dt.Columns.Add("RefAttrKey");
            dt.Columns.Add("IsHaveFuncPara");
            foreach (RefMethod item in rms)
            {
                if (item.ItIsCanBatch == false)
                    continue;
                DataRow mydr = dt.NewRow();
                item.HisEn = en; // 增加上.
                string myurl = "";
                if (item.RefMethodType != RefMethodType.Func)
                {
                    myurl = item.Do(null) as string;
                    if (myurl == null)
                        continue;
                }
                else
                {
                    myurl = "../Comm/RefMethod.htm?Index=" + item.Index + "&EnName=" + en.ToString() + "&EnsName=" + en.GetNewEntities.ToString() + "&PKVal=" + this.PKVal;
                }

                DataRow dr = dt.NewRow();

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
                dr["IsCanBatch"] = item.ItIsCanBatch;
                dr["GroupName"] = item.GroupName;
                dr["IsHaveFuncPara"] = item.HisAttrs.Count == 0 ? 0 : 1;
                dt.Rows.Add(dr);
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        public string Refmethod_Done()
        {
            Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            string msg = "";

            string pk = this.PKVal;

            if (pk.Contains(",") == false)
            {
                /*批处理的方式.*/
                en.PKVal = pk;

                en.RetrieveFromDBSources();
                msg = DoOneEntity(en, this.Index);
                if (msg == null)
                    return "close@info";
                else if (msg.IndexOf("@") != -1)
                    return msg;
                else
                    return "info@" + msg;
            }

            //如果是批处理.
            string[] pks = pk.Split(',');
            int count = 0;
            int sucCount = 0;
            int errCount = 0;
            foreach (string mypk in pks)
            {
                if (DataType.IsNullOrEmpty(mypk) == true)
                    continue;
                count++;
                en.PKVal = mypk;
                en.RetrieveFromDBSources();

                string s = DoOneEntity(en, this.Index);
                if (DataType.IsNullOrEmpty(s) == false)
                {
                    if (s.IndexOf("err@") != -1)
                        errCount++;
                    else
                        sucCount++;
                    if (en.ItIsNoEntity)
                        msg += "编号:" + en.GetValByKey("No") + ",名称:" + en.GetValByKey("Name") + ",执行结果:" + s + "<br/>";
                    else if (en.ItIsOIDEntity)
                    {
                        if (DataType.IsNullOrEmpty(en.GetValStringByKey("PrjNo")) == false)
                            msg += "编号:" + en.GetValStringByKey("PrjNo") + " 名称:" + en.GetValStringByKey("PrjName") + " 执行结果:" + s + "<br/>";
                        else
                            msg += "编号:" + en.GetValByKey("OID") + " 名称:" + en.GetValByKey("Title") + " 执行结果:" + s + "<br/>";

                    }
                    else
                        msg += "主键:" + en.GetValStringByKey(en.PK) + s + "<br/>";
                }

            }

            if (DataType.IsNullOrEmpty(msg) == true)
                return "close@info";
            if (pk.IndexOf(",") != -1)
                msg = "一共选择" + count + "笔数据,其中[" + sucCount + "]执行成功,[" + errCount + "]执行失败.<br/>" + msg;
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
                            case DataType.AppString:
                            case DataType.AppDate:
                            case DataType.AppDateTime:
                                string str1 = this.GetValFromFrmByKey(attr.Key);
                                objs[idx] = str1;
                                //attr.DefaultVal=str1;
                                break;
                            case DataType.AppInt:
                                int myInt = this.GetValIntFromFrmByKey(attr.Key);
                                objs[idx] = myInt;
                                //attr.DefaultVal=myInt;
                                break;
                            case DataType.AppFloat:
                                float myFloat = this.GetValFloatFromFrmByKey(attr.Key);
                                objs[idx] = myFloat;
                                //attr.DefaultVal=myFloat;
                                break;
                            case DataType.AppDouble:
                            case DataType.AppMoney:
                                decimal myDoub = this.GetValDecimalFromFrmByKey(attr.Key);
                                objs[idx] = myDoub;
                                //attr.DefaultVal=myDoub;
                                break;
                            case DataType.AppBoolean:
                                objs[idx] = this.GetValBoolenFromFrmByKey(attr.Key);
                                attr.DefaultVal = false;
                                break;
                            default:
                                throw new Exception("没有判断的字段 - 数据类型．");

                        }
                        break;
                    case UIContralType.DDL:
                        try
                        {
                            if (attr.MyDataType == DataType.AppString)
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
                string err = "@执行[" + this.EnsName + "][" + rm.ClassMethodName + "]期间出现错误：" + ex.Message + " InnerException= " + ex.InnerException + "[参数为：]" + msg;
                return "<font color=red>" + err + "</font>";
            }
        }
        #endregion 相关功能.

        #region  公共方法。
        public string SFTable()
        {
            SFTable sftable = new SFTable(this.GetRequestVal("SFTable"));
            DataTable dt = sftable.GenerHisDataTable();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得一个实体的数据
        /// </summary>
        /// <returns></returns>
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
            string sqlKey = this.GetRequestVal("SQLKey"); //SQL的key.
            string paras = this.GetRequestVal("Paras"); //参数. 格式为 @para1=paraVal@para2=val2

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
            SysEnums ses = new SysEnums();
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                ses.Retrieve(SysEnumAttr.EnumKey, this.EnumKey, SysEnumAttr.OrgNo, WebUser.OrgNo);
                if (ses.Count == 0)
                {
                    BP.Sys.XML.EnumInfoXml xml = new BP.Sys.XML.EnumInfoXml(this.EnumKey);
                    ses.RegIt(this.EnumKey, xml.Vals);
                }
            }
            else
                ses = new SysEnums(this.EnumKey);
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
        public string Exec(string clsName, string methodName, string paras = null)
        {
            #region 处理 HttpHandler 类.
            if (clsName.ToUpper().Contains(".HttpHandler.") == true)
            {
                //创建类实体.
                string baseName = BP.Sys.Base.Glo.DealClassEntityName("BP.WF.HttpHandler.DirectoryPageBase");

                DirectoryPageBase ctrl = Activator.CreateInstance(System.Type.GetType(baseName)) as DirectoryPageBase;
                //ctrl.context = this.context;

                try
                {
                    //执行方法返回json.
                    string data = ctrl.DoMethod(ctrl, methodName);
                    return data;
                }
                catch (Exception ex)
                {
                    string parasStr = "";
                    foreach (string key in HttpContextHelper.RequestParamKeys)
                    {
                        parasStr += "@" + key + "=" + HttpContextHelper.RequestParams(key);
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
                string result = mp.Invoke(this, myparas) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。
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
            sql = HttpUtility.UrlDecode(sql, Encoding.UTF8);

            string dbSrc = this.GetRequestVal("DBSrc");
            sql = sql.Replace("~~", "\"");
            sql = sql.Replace("~", "'");
            sql = sql.Replace("[%]", "%");  //防止URL编码
            if (DataType.IsNullOrEmpty(dbSrc) == false && dbSrc.Equals("local") == false)
            {
                SFDBSrc sfdb = new SFDBSrc(dbSrc);
                return sfdb.RunSQL(sql).ToString();
            }

            return DBAccess.RunSQL(sql).ToString();
        }
        /// <summary>
        /// 运行SQL返回DataTable
        /// </summary>
        /// <returns>DataTable转换的json</returns>
        public string DBAccess_RunSQLReturnTable()
        {
            string sql = this.GetRequestVal("SQL");

            //判断是否是标记,没有空格.
            if (sql.Contains(" ") == false)
                sql = SQLManager.GenerSQLByMark(sql);


            sql = HttpUtility.UrlDecode(sql, Encoding.UTF8);
            string dbSrc = this.GetRequestVal("DBSrc");
            sql = sql.Replace("~", "'");
            sql = sql.Replace("[%]", "%");  //防止URL编码

            sql = sql.Replace("@WebUser.No", WebUser.No);  //替换变量.
            sql = sql.Replace("@WebUser.Name", WebUser.Name);  //替换变量.
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);  //替换变量.
            sql = sql.Replace("@WebUser.DeptParentNo", WebUser.DeptParentNo);  //替换变量.
            sql = sql.Replace("@WebUser.OrgNo", WebUser.OrgNo);  //替换变量.

#warning zhoupeng把这个去掉了. 2018.10.24

            // sql = sql.Replace("-", "%"); //为什么？

            sql = sql.Replace("/#", "+"); //为什么？
            sql = sql.Replace("/$", "-"); //为什么？
            sql = sql.Replace("‘", "'");
            sql = sql.Replace("’", "'");

            if (null == sql || "" == sql)
                return "err@查询sql为空";
            DataTable dt = null;
            if (DataType.IsNullOrEmpty(dbSrc) == false && dbSrc.Equals("local") == false)
            {
                SFDBSrc sfdb = new SFDBSrc(dbSrc);
                dt = sfdb.RunSQLReturnTable(sql);
            }
            else
                dt = DBAccess.RunSQLReturnTable(sql);

            //暂定
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                //获取SQL的字段
                //获取 from 的位置
                sql = sql.Replace(" ", "");
                int index = sql.ToUpper().IndexOf("FROM");
                int indexAs = 0;
                sql = sql.Substring(6, index - 6);
                string[] keys = sql.Split(',');
                foreach (string key in keys)
                {
                    string realkey = key.Replace("Case", "").Replace("case", "").Replace("CASE", "");
                    indexAs = realkey.ToUpper().IndexOf("AS");
                    if (indexAs != -1)
                        realkey = realkey.Substring(indexAs + 2);
                    if (dt.Columns[realkey.ToUpper()] != null)
                        dt.Columns[realkey.ToUpper()].ColumnName = realkey;
                }

            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                //获取SQL的字段
                //获取 from 的位置
                sql = sql.Replace(" ", "");
                int index = sql.ToUpper().IndexOf("FROM");
                int indexAs = 0;
                sql = sql.Substring(6, index - 6);
                string[] keys = sql.Split(',');
                foreach (string key in keys)
                {
                    string realkey = key.Replace("Case", "").Replace("case", "").Replace("CASE", "");
                    indexAs = realkey.ToUpper().IndexOf("AS");
                    if (indexAs != -1)
                        realkey = realkey.Substring(indexAs + 2);
                    if (dt.Columns[realkey.ToLower()] != null)
                        dt.Columns[realkey.ToLower()].ColumnName = realkey;
                }

            }

            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
            {
                if (dt.Columns.Contains("NO"))
                    dt.Columns["NO"].ColumnName = "No";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        public string RunUrlCrossReturnString()
        {
            string url = this.GetRequestVal("urlExt");
            string strs = DataType.ReadURLContext(url, 9999, System.Text.Encoding.UTF8);
            return strs;
        }
        /// <summary>
        /// 通过接口返回JSON数据
        /// </summary>
        /// <returns></returns>
        public string RunWebAPIReturnString()
        {
            //设置请求头
            Hashtable headerMap = new Hashtable();

            //设置返回值格式
            headerMap.Add("Content-Type", "application/json");
            //设置token，用于接口校验
            headerMap.Add("Authorization", WebUser.Token);

            string url = this.GetRequestVal("url");
            string postData = BP.Tools.PubGlo.HttpPostConnect(url, headerMap, null);

            JObject res = postData.ToJObject();
            if (res["code"].ToString() == "200")
                return res["data"].ToString();
            else
                return "[]";
        }
        #endregion

        //执行方法.
        public string HttpHandler()
        {
            //@樊雷伟 , 这个方法需要同步.

            //获得两个参数.
            string httpHandlerName = this.GetRequestVal("HttpHandlerName");
            string methodName = this.GetRequestVal("DoMethod");

            Type type = System.Type.GetType(httpHandlerName);
            if (type == null)
            {
                BP.WF.HttpHandler.DirectoryPageBase obj = ClassFactory.GetHandlerPage(httpHandlerName) as BP.WF.HttpHandler.DirectoryPageBase;
                if (obj == null)
                    return "err@页面处理类名[" + httpHandlerName + "],没有获取到，请检查拼写错误？";
                // obj.context = this.context;
                return obj.DoMethod(obj, methodName);
            }
            else
            {
                BP.WF.HttpHandler.DirectoryPageBase en = Activator.CreateInstance(type)
                    as BP.WF.HttpHandler.DirectoryPageBase;
                //en.context = this.context;
                return en.DoMethod(en, methodName);
            }
        }
        /// <summary>
        /// 当前登录人员信息
        /// </summary>
        /// <returns></returns>
        public string GuestUser_Init()
        {
            Hashtable ht = new Hashtable();

            string userNo = Web.GuestUser.No;
            if (DataType.IsNullOrEmpty(userNo) == true)
            {
                ht.Add("No", "");
                ht.Add("Name", "");
                return BP.Tools.Json.ToJson(ht);
            }

            ht.Add("No", GuestUser.No);
            ht.Add("Name", GuestUser.Name);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 当前登录人员信息
        /// </summary>
        /// <returns></returns>
        public string WebUser_Init()
        {
            Hashtable ht = new Hashtable();
            string token = this.GetRequestVal("Token");
            if (DataType.IsNullOrEmpty(token) == false)
            {
                if (DataType.IsNullOrEmpty(WebUser.Token) == false && token.Equals(WebUser.Token) == true)
                {

                }
                else
                {
                    BP.WF.Dev2Interface.Port_LoginByToken(token);
                }
            }
            if (DataType.IsNullOrEmpty(token) == true)
            {
                string userNo = Web.WebUser.No;
                if (DataType.IsNullOrEmpty(userNo) == true)
                {
                    token = Web.WebUser.Token;
                    if (DataType.IsNullOrEmpty(token) == true)
                        throw new Exception("err@ 登录已过期，请重新登录!");

                    BP.WF.Dev2Interface.Port_LoginByToken(token);
                }
            }
            //需要同步.
            ht.Add("No", WebUser.No);
            ht.Add("Name", WebUser.Name);
            ht.Add("FK_Dept", WebUser.DeptNo);
            ht.Add("FK_DeptName", WebUser.DeptName);
            ht.Add("FK_DeptNameOfFull", WebUser.DeptNameOfFull);
            ht.Add("CustomerNo", BP.Difference.SystemConfig.CustomerNo);
            ht.Add("CustomerName", BP.Difference.SystemConfig.CustomerName);
            ht.Add("IsAdmin", WebUser.IsAdmin == true ? 1 : 0);
            ht.Add("Token", WebUser.Token); //token.

            ht.Add("Tel", WebUser.Tel);
            ht.Add("OrgNo", WebUser.OrgNo);
            ht.Add("OrgName", WebUser.OrgName);

            //检查是否是授权状态.
            if (WebUser.IsAuthorize == true)
            {
                ht.Add("IsAuthorize", "1");
                ht.Add("Auth", WebUser.Auth);
                ht.Add("AuthName", WebUser.AuthName);
            }
            else
            {
                ht.Add("IsAuthorize", "0");
            }

            //每次访问表很消耗资源.
            //Port.WFEmp emp = new BP.Port.WFEmp(WebUser.No);
            //ht.Add("Theme", emp.GetParaString("Theme"));


            //增加运行模式. add by zhoupeng 2020.03.10 适应saas模式.
            ht.Add("CCBPMRunModel", BP.Difference.SystemConfig.GetValByKey("CCBPMRunModel", "0"));

            return BP.Tools.Json.ToJson(ht);
        }

        public string WebUser_BackToAuthorize()
        {
            BP.WF.Dev2Interface.Port_Login(WebUser.Auth);
            return "登录成功";
        }


        #region 分组统计.
        /// <summary>
        /// 获得分组统计的查询条件.
        /// </summary>
        /// <returns></returns>
        public string Group_MapBaseInfo()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类名:" + this.EnsName + "错误";

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;

            Hashtable ht = new Hashtable();

            //把权限信息放入.
            UAC uac = en.HisUAC;
            if (this.GetRequestValBoolen("IsReadonly"))
            {
                ht.Add("IsUpdata", false);

                ht.Add("IsInsert", false);
                ht.Add("IsDelete", false);
            }
            else
            {
                ht.Add("IsUpdata", uac.IsUpdate);

                ht.Add("IsInsert", uac.IsInsert);
                ht.Add("IsDelete", uac.IsDelete);
                ht.Add("IsView", uac.IsView);
            }

            ht.Add("IsExp", uac.IsExp); //是否可以导出?
            ht.Add("IsImp", uac.IsImp); //是否可以导入?

            ht.Add("EnDesc", en.EnDesc); //描述?
            ht.Add("EnName", en.ToString()); //类名?

            MapData mapData = new MapData();
            mapData.No = this.EnsName;

            #region 查询条件
            //单据，实体单据
            if (mapData.RetrieveFromDBSources() != 0 && DataType.IsNullOrEmpty(mapData.FormTreeNo) == false)
            {
                //查询条件.
                ht.Add("IsShowSearchKey", mapData.GetParaInt("IsSearchKey"));
                ht.Add("SearchFields", mapData.GetParaString("StringSearchKeys"));

                //按日期查询.
                ht.Add("DTSearchWay", mapData.GetParaInt("DTSearchWay"));
                ht.Add("DTSearchLabel", mapData.GetParaString("DTSearchLabel"));

            }
            else
            {
                if (map.ItIsShowSearchKey == true)
                    ht.Add("IsShowSearchKey", 1);
                else
                    ht.Add("IsShowSearchKey", 0);

                ht.Add("SearchFields", map.SearchFields);
                ht.Add("SearchFieldsOfNum", map.SearchFieldsOfNum);

                //按日期查询.
                ht.Add("DTSearchWay", (int)map.DTSearchWay);
                ht.Add("DTSearchLabel", map.DTSearchLabel);
                ht.Add("DTSearchKey", map.DTSearchKey);
            }
            #endregion  查询条件

            //把map信息放入
            ht.Add("PhysicsTable", map.PhysicsTable);
            ht.Add("CodeStruct", map.CodeStruct);
            // ht.Add("CodeLength", map.CodeLength);
            return BP.Tools.Json.ToJson(ht);
        }
        #endregion

        /// <summary>
        /// 外键或者枚举的分组查询条件.
        /// </summary>
        /// <returns></returns>
        public string Group_SearchAttrs()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类名错误:" + this.EnsName;

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;

            DataSet ds = new DataSet();

            //构造查询条件集合.
            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.Columns.Add("MyFieldType");
            dt.TableName = "Attrs";

            SearchFKEnums attrs = map.SearchFKEnums;
            foreach (SearchFKEnum item in attrs)
            {
                DataRow dr = dt.NewRow();
                dr["Field"] = item.Key;
                dr["Name"] = item.HisAttr.Desc;
                dr["MyFieldType"] = item.HisAttr.MyFieldType;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            //把外键枚举增加到里面.
            foreach (SearchFKEnum item in attrs)
            {
                Attr attr = item.HisAttr;
                if (attr.ItIsEnum == true)
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    DataTable dtEnum = ses.ToDataTableField();
                    dtEnum.TableName = item.Key;
                    ds.Tables.Add(dtEnum);
                    continue;
                }

                if (attr.ItIsFK == true)
                {
                    Entities ensFK = item.HisAttr.HisFKEns;
                    ensFK.RetrieveAll();

                    DataTable dtEn = ensFK.ToDataTableField();
                    dtEn.TableName = item.Key;

                    ds.Tables.Add(dtEn);
                }
                //绑定SQL的外键
                if (DataType.IsNullOrEmpty(attr.UIBindKey) == false
                    && ds.Tables.Contains(attr.Key) == false)
                {
                    string sql = attr.UIBindKey;
                    DataTable dtSQl = null;
                    //说明是实体类绑定的外部数据源
                    if (sql.ToUpper().Contains("SELECT") == true)
                    {
                        //sql数据
                        sql = BP.WF.Glo.DealExp(attr.UIBindKey, null, null);
                        dtSQl = DBAccess.RunSQLReturnTable(sql);
                    }
                    else
                    {
                        dtSQl = BP.Pub.PubClass.GetDataTableByUIBineKey(attr.UIBindKey);
                    }
                    foreach (DataColumn col in dtSQl.Columns)
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
                            case "parentno":
                                col.ColumnName = "ParentNo";
                                break;
                            default:
                                break;
                        }
                    }
                    dtSQl.TableName = item.Key;
                    ds.Tables.Add(dtSQl);
                }


            }

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        ///获取分组的外键、枚举
        /// </summary>
        /// <returns></returns>
        public string Group_ContentAttrs()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类名错误:" + this.EnsName;

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;
            Attrs attrs = map.Attrs;
            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.Columns.Add("Checked");
            dt.TableName = "Attrs";

            //获取注册信心表
            UserRegedit ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");

            //判断是否已经选择分组
            bool contentFlag = false;
            foreach (Attr attr in attrs)
            {
                if (attr.ItIsEnum == true || attr.UIContralType == UIContralType.DDL || attr.UIContralType == UIContralType.RadioBtn)
                {
                    DataRow dr = dt.NewRow();
                    dr["Field"] = attr.Key;
                    dr["Name"] = attr.Desc;

                    // 根据状态 设置信息.
                    if (ur.Vals.IndexOf(attr.Key) != -1)
                    {
                        dr["Checked"] = "true";
                        contentFlag = true;
                    }
                    dt.Rows.Add(dr);
                }

            }

            if (contentFlag == false && dt.Rows.Count != 0)
                dt.Rows[0]["Checked"] = "true";

            return BP.Tools.Json.ToJson(dt);
        }

        public string Group_Analysis()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类名错误:" + this.EnsName;

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;
            DataSet ds = new DataSet();


            //获取注册信息表
            UserRegedit ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");

            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.Columns.Add("Checked");

            dt.TableName = "Attrs";

            //默认手动添加一个求数量的分析项
            DataRow dtr = dt.NewRow();
            dtr["Field"] = "Group_Number";
            dtr["Name"] = "数量";
            dtr["Checked"] = "true";
            dt.Rows.Add(dtr);

            DataTable ddlDt = new DataTable();
            ddlDt.TableName = "Group_Number";
            ddlDt.Columns.Add("No");
            ddlDt.Columns.Add("Name");
            ddlDt.Columns.Add("Selected");
            DataRow ddlDr = ddlDt.NewRow();
            ddlDr["No"] = "SUM";
            ddlDr["Name"] = "求和";
            ddlDr["Selected"] = "true";
            ddlDt.Rows.Add(ddlDr);
            ddlDr = ddlDt.NewRow();
            ddlDr["No"] = "AVG";
            ddlDr["Name"] = "求平均";
            if (ur.Vals.IndexOf("@Group_Number=AVG") != -1)
                ddlDr["Selected"] = "true";
            ddlDt.Rows.Add(ddlDr);
            ds.Tables.Add(ddlDt);

            foreach (Attr attr in map.Attrs)
            {
                if (attr.ItIsPK || attr.ItIsNum == false)
                    continue;
                if (attr.UIContralType != UIContralType.TB)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                if (attr.MyFieldType == FieldType.FK)
                    continue;
                if (attr.MyFieldType == FieldType.Enum)
                    continue;
                if (attr.Key.Equals("OID") || attr.Key.Equals("WorkID") || attr.Key.Equals("FID") || attr.Key.Equals("FK_Node") || attr.Key.Equals("WFState") || attr.Key.Equals("MID"))
                    continue;



                dtr = dt.NewRow();
                dtr["Field"] = attr.Key;
                dtr["Name"] = attr.Desc;


                // 根据状态 设置信息.
                if (ur.Vals.IndexOf(attr.Key) != -1)
                    dtr["Checked"] = "true";

                dt.Rows.Add(dtr);

                ddlDt = new DataTable();
                ddlDt.Columns.Add("No");
                ddlDt.Columns.Add("Name");
                ddlDt.Columns.Add("Selected");
                ddlDt.TableName = attr.Key;

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "SUM";
                ddlDr["Name"] = "求和";
                if (ur.Vals.IndexOf("@" + attr.Key + "=SUM") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "AVG";
                ddlDr["Name"] = "求平均";
                if (ur.Vals.IndexOf("@" + attr.Key + "=AVG") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);

                if (this.ItIsContainsNDYF)
                {
                    ddlDr = ddlDt.NewRow();
                    ddlDr["No"] = "AMOUNT";
                    ddlDr["Name"] = "求累计";
                    if (ur.Vals.IndexOf("@" + attr.Key + "=AMOUNT") != -1)
                        ddlDr["Selected"] = "true";
                    ddlDt.Rows.Add(ddlDr);
                }

                ds.Tables.Add(ddlDt);


            }

            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        public string Group_Search()
        {
            //获得
            Entities ens = ClassFactory.GetEns(this.EnsName);
            if (ens == null)
                return "err@类名错误:" + this.EnsName;

            Entity en = ens.GetNewEntity;
            Map map = ens.GetNewEntity.EnMapInTime;
            DataSet ds = new DataSet();

            //获取注册信息表
            UserRegedit ur = new UserRegedit();
            ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");
            // 查询出来关于它的活动列配置.
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            ds = GroupSearchSet(ens, en, map, ur, ds, aas);
            if (ds == null)
                return "info@<img src='../Img/Warning.gif' /><b><font color=red> 您没有选择显示内容/分析项目</font></b>";

            ////不显示合计列。
            /*string NoShowSum =  BP.Difference.SystemConfig.GetConfigXmlEns("NoShowSum", this.EnsName);
            DataTable showSum = new DataTable("NoShowSum");
            showSum.Columns.Add("NoShowSum");
            DataRow sumdr = showSum.NewRow();
            sumdr["NoShowSum"] = NoShowSum;
            showSum.Rows.Add(sumdr);

            DataTable activeAttr = aas.ToDataTable();
            activeAttr.TableName = "ActiveAttr";
            ds.Tables.Add(activeAttr);
            ds.Tables.Add(showSum);*/

            return BP.Tools.Json.ToJson(ds);
        }

        private DataSet GroupSearchSet(Entities ens, Entity en, Map map, UserRegedit ur, DataSet ds, ActiveAttrs aas)
        {

            //查询条件
            //分组
            string Condition = ""; //处理特殊字段的条件问题。

            AtPara atPara = new AtPara(ur.Vals);
            //获取分组的条件
            string groupKey = atPara.GetValStrByKey("SelectedGroupKey");
            //分析项
            string analyKey = atPara.GetValStrByKey("StateNumKey");

            //设置显示的列
            Attrs mapAttrOfShows = new Attrs();

            //查询语句定义
            string sql = "";
            string selectSQL = "SELECT ";  //select部分的组合
            string groupBySQL = " GROUP BY "; //分组的组合

            #region SelectSQL语句的组合

            #region 分组条件的整合
            if (DataType.IsNullOrEmpty(groupKey) == false)
            {
                bool isSelected = false;
                string[] SelectedGroupKeys = groupKey.Split(',');
                foreach (string key in SelectedGroupKeys)
                {
                    if (DataType.IsNullOrEmpty(key) == true)
                        continue;
                    Attr attr = map.GetAttrByKey(key);
                    // 加入组里面。
                    mapAttrOfShows.Add(map.GetAttrByKey(key));

                    selectSQL += map.PhysicsTable + "." + key + " \"" + key + "\",";

                    groupBySQL += map.PhysicsTable + "." + key + ",";

                    if (attr.MyFieldType == FieldType.FK)
                    {
                        Map fkMap = attr.HisFKEn.EnMap;
                        string refText = fkMap.PhysicsTable + "_" + attr.Key + "." + fkMap.GetFieldByKey(attr.UIRefKeyText);
                        selectSQL += refText + "  AS " + key + "Text" + ",";
                        groupBySQL += refText + ",";
                        continue;
                    }

                    if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                    {
                        //增加枚举字段
                        if (DataType.IsNullOrEmpty(attr.UIBindKey))
                            throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag + "");

                        Sys.SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                        selectSQL += ses.GenerCaseWhenForOracle(en.EnMap.PhysicsTable + ".", attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString().Equals("") == true ? "0" : attr.DefaultVal.ToString()) + ",";
                        continue;
                    }

                    //不是外键、枚举，就是外部数据源
                    selectSQL += map.PhysicsTable + "." + key + "T" + " \"" + key + "T\",";
                    groupBySQL += map.PhysicsTable + "." + key + "T,";



                }
            }
            #endregion 分组条件的整合

            #region 分析项的整合
            Attrs AttrsOfNum = new Attrs();
            string[] analyKeys = analyKey.Split(',');
            foreach (string key in analyKeys)
            {
                if (DataType.IsNullOrEmpty(key) == true)
                    continue;
                string[] strs = key.Split('=');
                if (strs.Length != 2)
                    continue;

                //求数据的总和
                if (strs[0].Equals("Group_Number"))
                {
                    selectSQL += " count(*) \"" + strs[0] + "\",";
                    mapAttrOfShows.Add(new Attr("Group_Number", "Group_Number", 1, DataType.AppInt, false, "数量(合计)"));
                    AttrsOfNum.Add(new Attr("Group_Number", "Group_Number", 1, DataType.AppInt, false, "数量"));
                    continue;
                }

                //判断分析项的数据类型
                Attr attr = map.GetAttrByKey(strs[0]);
                AttrsOfNum.Add(attr);

                int dataType = attr.MyDataType;
                switch (strs[1])
                {
                    case "SUM":
                        if (dataType == 2)
                            selectSQL += " SUM(" + map.PhysicsTable + "." + strs[0] + ") \"" + strs[0] + "\",";
                        else
                        {
                            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                                selectSQL += " round ( cast (SUM(" + map.PhysicsTable + "." + strs[0] + ") as  numeric), 4)  \"" + strs[0] + "\",";
                            else
                                selectSQL += " round ( SUM(" + map.PhysicsTable + "." + strs[0] + "), 4) \"" + strs[0] + "\",";
                        }
                        attr.Desc = attr.Desc + "(合计)";

                        break;
                    case "AVG":
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                            selectSQL += " round ( cast (AVG(" + map.PhysicsTable + "." + strs[0] + ") as  numeric), 4)  \"" + strs[0] + "\",";
                        else
                            selectSQL += " round (AVG(" + map.PhysicsTable + "." + strs[0] + "), 4)  \"" + strs[0] + "\",";
                        attr.Desc = attr.Desc + "(平均)";
                        break;
                    case "AMOUNT":
                        if (dataType == 2)
                            selectSQL += " SUM(" + map.PhysicsTable + "." + strs[0] + ") \"" + strs[0] + "\",";
                        else
                        {
                            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                                selectSQL += " round ( cast (SUM(" + map.PhysicsTable + "." + strs[0] + ") as  numeric), 4)  \"" + strs[0] + "\",";
                            else
                                selectSQL += " round ( SUM(" + map.PhysicsTable + "." + strs[0] + "), 4) \"" + strs[0] + "\",";
                        }
                        attr.Desc = attr.Desc + "(累计)";
                        break;
                    default:
                        throw new Exception("没有判断的情况.");
                }
                mapAttrOfShows.Add(attr);

            }
            #endregion 分析项的整合
            if (DataType.IsNullOrEmpty(selectSQL) == true || selectSQL.Equals("SELECT ") == true)
                return null;
            selectSQL = selectSQL.Substring(0, selectSQL.Length - 1);

            #endregion SelectSQL语句的组合

            #region WhereSQL语句的组合

            //获取查询的注册表
            BP.Sys.UserRegedit searchUr = new UserRegedit();
            searchUr.setMyPK(WebUser.No + "_" + this.EnsName + "_SearchAttrs");
            searchUr.RetrieveFromDBSources();

            QueryObject qo = Search_Data(ens, en, map, searchUr);

            string whereSQL = " " + qo.SQL.Substring(qo.SQL.IndexOf("FROM "));

            #endregion WhereSQL语句的组合

            #region OrderBy语句组合

            string orderbySQL = "";
            string orderByKey = this.GetRequestVal("OrderBy");
            if (DataType.IsNullOrEmpty(orderByKey) == false && selectSQL.Contains(orderByKey) == true)
            {
                orderbySQL = " ORDER BY" + orderByKey;
                string orderWay = this.GetRequestVal("OrderWay");
                if (DataType.IsNullOrEmpty(orderWay) == false && orderWay.Equals("Up") == false)
                    orderbySQL += " DESC ";

            }

            #endregion OrderBy语句组合

            sql = selectSQL + whereSQL + groupBySQL.Substring(0, groupBySQL.Length - 1) + orderbySQL;

            DataTable dt = DBAccess.RunSQLReturnTable(sql, qo.MyParas);
            dt.TableName = "MainData";

            ds.Tables.Add(dt);
            ds.Tables.Add(mapAttrOfShows.ToMapAttrs.ToDataTableField("Sys_MapAttr"));
            ds.Tables.Add(AttrsOfNum.ToMapAttrs.ToDataTableField("AttrsOfNum"));

            return ds;
        }
        public string ParseExpToDecimal()
        {
            string exp = this.GetRequestVal("Exp");

            decimal d = DataType.ParseExpToDecimal(exp);
            return d.ToString();
        }


        public bool ItIsContainsNDYF
        {
            get
            {
                if (this.GetValFromFrmByKey("IsContainsNDYF").ToString().ToUpper() == "TRUE")
                    return true;
                else
                    return false;
            }
        }

        #region 常用词汇功能开始
        /// <summary>
        /// 常用词汇
        /// </summary>
        /// <returns></returns>
        public string HelperWordsData()
        {

            string FK_MapData = this.GetRequestVal("FK_MapData");
            string AttrKey = this.GetRequestVal("AttrKey");
            string lb = this.GetRequestVal("lb");

            //读取txt文件
            if (lb == "readWords")
                return readTxt();

            //读取其他常用词汇
            DataSet ds = new DataSet();
            //我的词汇
            if (lb == "myWords")
            {
                DefVals dvs = new DefVals();
                QueryObject qo = new QueryObject(dvs);
                qo.AddHD();

                qo.addAnd();
                qo.AddWhere(DefValAttr.FrmID, "=", FK_MapData);
                qo.addAnd();
                qo.AddWhere(DefValAttr.AttrKey, "=", AttrKey);
                qo.addAnd();
                qo.AddWhere(DefValAttr.EmpNo, "=", WebUser.No);
                qo.addAnd();
                qo.AddWhere(DefValAttr.LB, "=", "1");

                string pageNumber = GetRequestVal("pageNumber");
                int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                //每页多少行
                string pageSize = GetRequestVal("pageSize");
                int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

                DataTable dt = new DataTable("DataCount");
                dt.Columns.Add("DataCount", typeof(int));
                DataRow dr = dt.NewRow();
                dr["DataCount"] = qo.GetCount();
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);

                qo.DoQuery("MyPK", iPageSize, iPageNumber);


                // string gg = BP.Tools.Json.ToJson(dvs.ToDataTableField("MainTable"));
                ds.Tables.Add(dvs.ToDataTableField("MainTable")); //把描述加入.
            }
            if (lb == "hisWords")
            {
                Node nd = new Node(this.NodeID);
                string rptNo = "ND" + int.Parse(this.FlowNo) + "Rpt";
                if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    MapData mapData = new MapData(this.FrmID);
                    rptNo = mapData.PTable;
                }


                GEEntitys ges = new GEEntitys(rptNo);
                QueryObject qo = new QueryObject(ges);
                string fk_emp = this.GetRequestVal("FK_Emp");
                qo.AddWhere(fk_emp, "=", WebUser.No);
                qo.addAnd();
                qo.AddWhere(AttrKey, "!=", "");
                string pageNumber = GetRequestVal("pageNumber");
                int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                //每页多少行
                string pageSize = GetRequestVal("pageSize");
                int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

                DataTable dt = new DataTable("DataCount");
                dt.Columns.Add("DataCount", typeof(int));
                DataRow dr = dt.NewRow();
                dr["DataCount"] = qo.GetCount();
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);

                qo.DoQuery("OID", iPageSize, iPageNumber);

                dt = ges.ToDataTableField();
                DataTable newDt = new DataTable("MainTable");
                newDt.Columns.Add("CurValue");
                newDt.Columns.Add("MyPk");
                foreach (DataRow drs in dt.Rows)
                {
                    if (DataType.IsNullOrEmpty(drs[AttrKey].ToString()))
                        continue;
                    dr = newDt.NewRow();
                    dr["CurValue"] = drs[AttrKey];
                    dr["MyPK"] = drs["OID"];
                    newDt.Rows.Add(dr);
                }

                ds.Tables.Add(newDt); //把描述加入.


            }

            return DataTableConvertJson.DataTable2Json(ds.Tables["MainTable"], int.Parse(ds.Tables["DataCount"].Rows[0][0].ToString()));
            //return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 注意特殊字符的处理
        /// </summary>
        /// <returns></returns>
        private string readTxt()
        {
            try
            {
                string path = BP.Difference.SystemConfig.PathOfDataUser + "Fastenter/" + this.FrmID + "/" + GetRequestVal("AttrKey"); ;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string[] folderArray = Directory.GetFiles(path);
                if (folderArray.Length == 0)
                    return "";

                string fileName;
                string[] strArray;

                string pageNumber = GetRequestVal("pageNumber");
                int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                string pageSize = GetRequestVal("pageSize");
                int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable("MainTable");
                dt.Columns.Add("MyPk", typeof(string));
                dt.Columns.Add("TxtStr", typeof(string));
                dt.Columns.Add("CurValue", typeof(string));

                string liStr = "";
                int count = 0;
                int index = iPageSize * (iPageNumber - 1);
                foreach (string folder in folderArray)
                {
                    dt.Rows.Add("", "", "");
                    if (count >= index && count < iPageSize * iPageNumber)
                    {
                        dt.Rows[count]["MyPk"] = DBAccess.GenerGUID();

                        strArray = folder.Split('\\');
                        fileName = strArray[strArray.Length - 1].Replace("\"", "").Replace("'", "");
                        liStr += string.Format("{{id:\"{0}\",value:\"{1}\"}},", DataTableConvertJson.GetFilteredStrForJSON(fileName, true),
                            DataTableConvertJson.GetFilteredStrForJSON(File.ReadAllText(folder, System.Text.Encoding.Default), false));

                        dt.Rows[count]["CurValue"] = DataTableConvertJson.GetFilteredStrForJSON(fileName, true);
                        dt.Rows[count]["TxtStr"] = DataTableConvertJson.GetFilteredStrForJSON(File.ReadAllText(folder, System.Text.Encoding.Default), false);
                    }
                    count += 1;
                }

                ds.Tables.Add(dt);
                dt = new DataTable("DataCount");
                dt.Columns.Add("DataCount", typeof(int));
                DataRow dr = dt.NewRow();
                dr["DataCount"] = folderArray.Length;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion 常用词汇结束

        #region 前台SQL转移处理
        public string RunSQL_Init()
        {
            string sql = GetRequestVal("SQL");
            string dbSrc = this.GetRequestVal("DBSrc");
            DataTable dt = null;
            if (DataType.IsNullOrEmpty(dbSrc) == false && dbSrc.Equals("local") == false)
            {
                SFDBSrc sfdb = new SFDBSrc(dbSrc);
                dt = sfdb.RunSQLReturnTable(sql);
            }
            else
            {
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        // 您的应用ID
        private static string APP_KEY = "447d8b671ee948b8";
        // 您的应用密钥
        private static string APP_SECRET = "rF1HBr3QjtPD1gXVFfIAGKtDRF6Q2HuB";

        public string ToLang()
        {
            // 添加请求参数
            Dictionary<String, String[]> paramsMap = createRequestParams();
            // 添加鉴权相关参数
            BP.Tools.AuthV3Util.addAuthParams(APP_KEY, APP_SECRET, paramsMap);
            Dictionary<String, String[]> header = new Dictionary<string, string[]>() { { "Content-Type", new String[] { "application/x-www-form-urlencoded" } } };
            // 请求api服务
            byte[] result = HttpUtil.doPost("https://openapi.youdao.com/api", header, paramsMap, "application/json");
            // 打印返回结果
            if (result != null)
            {
                string resStr = System.Text.Encoding.UTF8.GetString(result);
                return resStr;
            }
            return "";
        }

        private Dictionary<String, String[]> createRequestParams()
        {
            // note: 将下列变量替换为需要请求的参数
            string q = this.GetRequestVal("Txt");//待翻译文本
            string from = "zh-CHS";//源语言语种
            string to = this.GetRequestVal("ToLang");//目标语言语种
            string vocabId = "";//非必填项，用户指定的词典 out_id，目前支持英译中

            return new Dictionary<string, string[]>() {
                { "q", new string[]{q}},
                {"from", new string[]{from}},
                {"to", new string[]{to}},
                {"vocabId", new string[]{vocabId}}
            };
        }
    }

}
