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

            ens.RetrieveAll(EntityTreeAttr.Idx);
            return ens.ToJsonOfTree();
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

                if (attr.HisAttr.IsFKorEnum)
                    row["field"] = attr.KeyOfEn + "Text";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 部门-人员关系

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Comm()
        {
        }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
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

                if (this.FK_Dept == null)
                {
                    this.FK_Dept = value;
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

            MapAttrs attrs = map.Attrs.ToMapAttrs;

            //属性集合.
            DataTable dtAttrs = attrs.ToDataTableField();
            dtAttrs.TableName = "Sys_MapAttrs";

            DataSet ds = new DataSet();
            ds.Tables.Add(dtAttrs); //把描述加入.

            //查询结果
            QueryObject qo = new QueryObject(ens);

            string[] strs = this.context.Request.Form.ToString().Split('&');
            foreach (string str in strs)
            {
                if (str.IndexOf("EnsName") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }
                bool isExist = false;
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.Key.Equals(key))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist == false)
                    continue;

                if (mykey[1] == "mvals")
                {
                    //如果用户多项选择了，就要找到它的选择项目.

                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.EnsName + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    string val = "";
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                                val = WebUser.FK_Dept;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);
                        qo.AddWhereIn(mykey[0], instr);
                    }
                }
                else
                {
                    qo.AddWhere(mykey[0], mykey[1]);
                }
                qo.addAnd();
            }

            if (this.FK_Dept != null && (this.GetRequestVal("FK_Emp") == null
                || this.GetRequestVal("FK_Emp") == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }

            qo.AddHD();
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
        public string GroupDtl_Exp()
        {
            //获得.
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;

            //查询结果
            QueryObject qo = new QueryObject(ens);
            string[] strs = this.context.Request.Form.ToString().Split('&');
            foreach (string str in strs)
            {
                if (str.IndexOf("EnsName") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }

                if (en.EnMap.Attrs.Contains(key) == false)
                    continue;

                if (mykey[1] == "mvals")
                {
                    //如果用户多项选择了，就要找到它的选择项目.

                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.EnsName + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    string val = "";
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                                val = WebUser.FK_Dept;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);
                        qo.AddWhereIn(mykey[0], instr);
                    }
                }
                else
                {
                    qo.AddWhere(mykey[0], mykey[1]);
                }
                qo.addAnd();
            }

            if (this.FK_Dept != null && (this.GetRequestVal("FK_Emp") == null
                || this.GetRequestVal("FK_Emp") == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }

            qo.AddHD();

            DataTable dt = qo.DoQueryToTable();

            string filePath = ExportDGToExcel(dt, en, en.EnDesc);


            return filePath;
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
                Entity en = ClassFactory.GetEn(this.EnName);

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

                    //int i=en.RetrieveFromDBSources();
                    //if (i == 0)
                    //  return "err@实体:["+"]";
                }

                return en.ToJson(false);
            }
            catch (Exception ex)
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

                //if (en.HisUAC.IsDelete == false)
                //    return "err@xx";


                #region 首先判断参数删除.
                string key1 = this.GetRequestVal("Key1");
                string key2 = this.GetRequestVal("Key2");
                string val1 = this.GetRequestVal("Val1");
                string val2 = this.GetRequestVal("Val2");

                if (key1.Equals("undefined") == false)
                {
                    int num = 0;
                    if (key2.Equals("undefined") == false)
                    {
                        num = en.Delete(key1, val1, key2, val2);
                    }
                    else
                    {
                        num = en.Delete(key1, val1);
                    }
                    return num.ToString();
                }
                #endregion 首先判断参数删除.


                if (en.PKCount != 1)
                {
                    /*多个主键的情况. 遍历属性，循环赋值.*/
                    foreach (Attr attr in en.EnMap.Attrs)
                        en.SetValByKey(attr.Key, this.GetRequestVal(attr.Key));
                }
                else
                {
                    en.PKVal = this.PKVal;
                }

                int i = en.RetrieveFromDBSources(); //查询出来再删除.
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
                //return en.ToJson(false);

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
                    return "err@实体类名错误[" + this.EnName + "].";

                en.PKVal = this.PKVal;
                en.RetrieveFromDBSources();

                //遍历属性，循环赋值.
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    en.SetValByKey(attr.Key, this.GetValFromFrmByKey(attr.Key));
                }

                //保存参数属性.
                string frmParas = this.GetValFromFrmByKey("frmParas", "");
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
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
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
                    if (DataType.IsNullOrEmpty(para) || para.Contains("=") == false)
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

        /// <summary>
        /// 获得实体集合s
        /// </summary>
        /// <returns></returns>
        public string Entities_RetrieveCond()
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

                    if (idx == 0)
                    {
                        qo.AddWhere(key, oper, val);
                    }
                    else
                    {
                        qo.addAnd();
                        qo.AddWhere(key, oper, val);
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
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        public string Entities_DoMethodReturnString()
        {
            //创建类实体.
            BP.En.Entities ens = ClassFactory.GetEns(this.EnsName); // Activator.CreateInstance(System.Type.GetType("BP.En.Entity")) as BP.En.Entity;

            string methodName = this.GetRequestVal("MethodName");

            Type tp = ens.GetType();
            System.Reflection.MethodInfo mp = tp.GetMethod(methodName);
            if (mp == null)
                return "err@没有找到类[" + this.EnsName + "]方法[" + methodName + "].";

            string paras = this.GetRequestVal("paras");

            //执行该方法.
            object[] myparas = new object[0];

            if (DataType.IsNullOrEmpty(paras) == false)
                myparas = paras.Split(',');

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
                                    int myInt = this.GetValIntFromFrmByKey(attr.Key);  //int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.Row[idx] = myInt;
                                    rm.SetValByKey(attr.Key, myInt);
                                    break;
                                case BP.DA.DataType.AppFloat:
                                    float myFloat = this.GetValFloatFromFrmByKey(attr.Key); // float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myFloat);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppMoney:
                                    decimal myDoub = this.GetValDecimalFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myDoub);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    bool myBool = this.GetValBoolenFromFrmByKey(attr.Key); // decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
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
            if (ens == null)
                return "err@类名:" + this.EnsName + "错误";

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
            ht.Add("EnName", en.ToString()); //类名?


            //把map信息放入
            ht.Add("PhysicsTable", map.PhysicsTable);
            ht.Add("CodeStruct", map.CodeStruct);
            ht.Add("CodeLength", map.CodeLength);

            //查询条件.
            if (map.IsShowSearchKey == true)
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
            dt.TableName = "Attrs";

            AttrSearchs attrs = map.SearchAttrs;
            foreach (AttrSearch item in attrs)
            {
                DataRow dr = dt.NewRow();
                dr["Field"] = item.Key;
                dr["Name"] = item.HisAttr.Desc;
                dr["Width"] = item.Width; //下拉框显示的宽度.
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
                //绑定SQL的外键
                if (item.HisAttr.UIDDLShowType == BP.Web.Controls.DDLShowType.BindSQL)
                {
                    //获取SQl
                    string sql = item.HisAttr.UIBindKey;
                    sql = sql.Replace("~", "'");
                    if (sql.Contains("@WebUser.No"))
                        sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);

                    if (sql.Contains("@WebUser.Name"))
                        sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);

                    if (sql.Contains("@WebUser.FK_Dept"))
                        sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

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

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行查询 - 初始化查找数据
        /// </summary>
        /// <returns></returns>
        public string Search_SearchIt()
        {
            return BP.Tools.Json.ToJson(Search_Search());
        }
        /// <summary>
        /// 执行查询.这个方法也会被导出调用.
        /// </summary>
        /// <returns></returns>
        public DataSet Search_Search()
        {
            //获得.
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;

            MapAttrs attrs = new MapAttrs(); ;
            //DataTable dtAttrs = attrs.ToDataTableField();
            //dtAttrs.TableName = "Attrs";


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
            foreach (MapAttr attr in attrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if ((count != 0 && DataType.IsNullOrEmpty(searchVisable)) || attr.UIVisible == false)
                    continue;
                row = dtAttrs.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;

                dtAttrs.Rows.Add(row);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dtAttrs); //把描述加入.

            md.Name = map.EnDesc;

            //附件类型.
            md.SetPara("BPEntityAthType", (int)map.HisBPEntityAthType);

            //获取实体类的主键
            md.SetPara("PK", en.PK);

            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

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
            if (en.EnMap.IsShowSearchKey && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
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
                string enumKey = ","; //求出枚举值外键.
                foreach (Attr attr in map.Attrs)
                {
                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                            enumKey = "," + attr.Key + "Text,";
                            break;
                        case FieldType.FK:
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

                    i++;
                    if (i == 1)
                    {
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }
                    qo.addOr();

                    if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
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

            if (map.DTSearchWay != DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                //按日期查询
                if (map.DTSearchWay == DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    dtTo += " 23:59:59";
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
                        string y = ap.GetValStrByKey("DDL_" + attr.Key + "_Year");
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
            ur.SetPara("RecCount", qo.GetCount());
            ur.Save();

            //获取配置信息
            EnCfg encfg = new EnCfg(this.EnsName);
            //增加排序
            if (encfg != null)
            {
                string orderBy = encfg.GetParaString("OrderBy");
                bool isDesc = encfg.GetParaBoolen("IsDeSc");

                if (DataType.IsNullOrEmpty(orderBy) == false)
                {
                    if (isDesc)
                        qo.addOrderByDesc(encfg.GetParaString("OrderBy"));
                    else
                        qo.addOrderBy(encfg.GetParaString("OrderBy"));
                }

            }

            if (GetRequestVal("DoWhat") != null && GetRequestVal("DoWhat").Equals("Batch"))
                qo.DoQuery(en.PK, 500, 1);
            else
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

            RefMethods rms = map.HisRefMethods;
            foreach (RefMethod item in rms)
            {
                if (item.IsForEns == false)
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
                dr["IsCanBatch"] = item.IsCanBatch;
                dr["GroupName"] = item.GroupName;
                dr["ClassMethodName"] = item.ClassMethodName;

                dtM.Rows.Add(dr); //增加到rows.
            }
            ds.Tables.Add(dtM); //把数据加入里面.
            #endregion

            return ds;

        }

        private DataTable Search_Data(Entities ens, Entity en)
        {
            //获得.
            Map map = en.EnMapInTime;

            MapAttrs attrs = map.Attrs.ToMapAttrs;
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

            if (map.DTSearchWay != DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                if (map.DTSearchWay == DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    //设置开始查询时间
                    dtFrom += " 00:00:00";
                    //结束查询时间
                    dtTo += " 23:59:59";
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
                        string y = ap.GetValStrByKey("DDL_" + attr.Key + "_Year");
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

            #endregion 获得查询数据.
            return qo.DoQueryToTable();


        }
        public string Search_GenerPageIdx()
        {

            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.EnsName + "_SearchAttrs";
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
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            string name = "数据导出";
            string filename = name + "_" + BP.DA.DataType.CurrentDataTimeCNOfLong + "_" + WebUser.Name + ".xls";
            string filePath = ExportDGToExcel(Search_Data(ens, en), en, name);
            //DataTableToExcel(Search_Data(ens, en),en, filename, name,
            //                                                  BP.Web.WebUser.Name, true, true, true);

            return filePath;
        }
        #endregion 查询.

        #region Refmethod.htm 相关功能.
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
                string pk = this.PKVal;
                if (pk == null)
                    pk = this.GetRequestVal(en.PK);

                if (pk == null)
                    pk = this.PKVal;

                if (pk == null)
                    return "err@错误pkval 没有值。";

                string infos = "";

                string[] pks = pk.Split(',');

                foreach (string mypk in pks)
                {

                    en.PKVal = mypk;
                    en.Retrieve();
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

                    result = result.Replace("@", "\t\n");
                    infos += "close@" + result;
                }

                return infos;
            }
            #endregion 处理无参数的方法.

            en.PKVal = this.PKVal;
            en.Retrieve();

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
                string v = item.DefValReal;
                if (v.IndexOf('@') == -1)
                    mydrMain[item.KeyOfEn] = item.DefValReal;
                //替换默认值的@的
                else
                {
                    if (v.Equals("@WebUser.No"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.No;
                    else if (v.Equals("@WebUser.Name"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.Name;
                    else if (v.Equals("@WebUser.FK_Dept"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.FK_Dept;
                    else if (v.Equals("@WebUser.FK_DeptName"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.FK_DeptName;
                    else if (v.Equals("@WebUser.FK_DeptNameOfFull") || v.Equals("@WebUser.FK_DeptFullName"))
                        mydrMain[item.KeyOfEn] = Web.WebUser.FK_DeptNameOfFull;
                    else if (v.Equals("@RDT"))
                    {
                        if (item.MyDataType == DataType.AppDate)
                            mydrMain[item.KeyOfEn] = DataType.CurrentData;
                        if (item.MyDataType == DataType.AppDateTime)
                            mydrMain[item.KeyOfEn] = DataType.CurrentDataTime;
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
            foreach (DataRow dr in mapAttrs.Rows)
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
                    ds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }

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

        public string Entitys_Init()
        {
            //定义容器.
            DataSet ds = new DataSet();

            //查询出来从表数据.
            Entities dtls = ClassFactory.GetEns(this.EnsName);
            Entity en = dtls.GetNewEntity;
            QueryObject qo = new QueryObject(dtls);
            qo.addOrderBy(en.PK);
            qo.DoQuery();
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

            if (en.IsNoEntity == true && en.EnMap.IsAutoGenerNo)
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
            foreach (DataRow dr in sys_MapAttrs.Rows)
            {
                string uiBindKey = dr["UIBindKey"].ToString();
                string lgType = dr["LGType"].ToString();
                if (lgType.Equals("2") == false)
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


        #region 实体集合的保存.
        /// <summary>
        /// 实体集合的删除
        /// </summary>
        /// <returns></returns>
        public string Entities_Delete()
        {
            return "err@该方法没有完成，请使用Entiy_Delete. 可以按照条件删除.";
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
                Entity en = dtls.GetNewEntity;
                QueryObject qo = new QueryObject(dtls);
                //qo.DoQuery(en.PK, BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
                qo.DoQuery();
                Map map = en.EnMap;
                foreach (Entity item in dtls)
                {
                    string pkval = item.PKVal.ToString();

                    foreach (Attr attr in map.Attrs)
                    {
                        if (attr.IsRefAttr == true)
                            continue;

                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {
                            if (attr.UIIsReadonly == false)
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
                #endregion  查询出来实体数据.

                #region 保存新加行.
                //没有新增行
                if (this.GetRequestValBoolen("InsertFlag") == false)
                    return "保存成功.";

                string valValue = "";


                foreach (Attr attr in map.Attrs)
                {

                    if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                    {
                        if (attr.UIIsReadonly == false)
                            continue;

                        valValue = this.GetValFromFrmByKey("TB_" + 0 + "_" + attr.Key, null);
                        en.SetValByKey(attr.Key, valValue);
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.TB && attr.UIIsReadonly == false)
                    {
                        valValue = this.GetValFromFrmByKey("TB_" + 0 + "_" + attr.Key);
                        en.SetValByKey(attr.Key, valValue);
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == true)
                    {
                        valValue = this.GetValFromFrmByKey("DDL_" + 0 + "_" + attr.Key);
                        en.SetValByKey(attr.Key, valValue);
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == true)
                    {
                        valValue = this.GetValFromFrmByKey("CB_" + 0 + "_" + attr.Key, "-1");
                        if (valValue == "-1")
                            en.SetValByKey(attr.Key, 0);
                        else
                            en.SetValByKey(attr.Key, 1);
                        continue;
                    }
                }

                if (en.IsNoEntity)
                {
                    if (en.EnMap.IsAutoGenerNo)
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
                    Log.DebugWriteInfo(ex.Message);
                    //msg += "<hr>" + ex.Message;
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
            foreach (RefMethod item in rms)
            {
                if (item.IsCanBatch == false)
                    continue;
                DataRow mydr = dt.NewRow();
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
                dr["IsCanBatch"] = item.IsCanBatch;
                dr["GroupName"] = item.GroupName;
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
                if (DataType.IsNullOrEmpty(mypk) == true)
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
                string err = "@执行[" + this.EnsName + "][" + rm.ClassMethodName + "]期间出现错误：" + ex.Message + " InnerException= " + ex.InnerException + "[参数为：]" + msg;
                return "<font color=red>" + err + "</font>";
            }
        }
        #endregion 相关功能.

        #region  公共方法。
        public string SFTable()
        {
            SFTable sftable = new SFTable(this.GetRequestVal("SFTable"));
            DataTable dt = sftable.GenerHisDataTable;
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
        public string Exec(string clsName, string methodName, string paras = null)
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
            sql = sql.Replace("~", "'");
            sql = sql.Replace("[%]", "%");  //防止URL编码

            return DBAccess.RunSQL(sql).ToString();
        }
        /// <summary>
        /// 运行SQL返回DataTable
        /// </summary>
        /// <returns>DataTable转换的json</returns>
        public string DBAccess_RunSQLReturnTable()
        {
            string sql = this.GetRequestVal("SQL");
            sql = sql.Replace("~", "'");
            sql = sql.Replace("[%]", "%");  //防止URL编码


#warning zhoupeng把这个去掉了. 2018.10.24
            // sql = sql.Replace("-", "%"); //为什么？

            sql = sql.Replace("/#", "+"); //为什么？
            sql = sql.Replace("/$", "-"); //为什么？
            // sql = sql.Replace('"', '\"'); //为什么？

            if (null == sql || "" == sql)
            {
                return "err@查询sql为空";
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //暂定
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
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
                    var realkey = key.Replace("Case", "").Replace("case", "").Replace("CASE", "");
                    indexAs = realkey.ToUpper().IndexOf("AS");
                    if (indexAs != -1)
                        realkey = realkey.Substring(indexAs + 2);
                    if (dt.Columns[realkey.ToUpper()] != null)
                        dt.Columns[realkey.ToUpper()].ColumnName = realkey;
                }

            }

            return BP.Tools.Json.ToJson(dt);
        }
        public string RunUrlCrossReturnString()
        {
            string url = this.GetRequestVal("url");
            string strs = DataType.ReadURLContext(url, 9999, System.Text.Encoding.UTF8);
            return strs;
        }
        #endregion

        //执行方法.
        public string HttpHandler()
        {
            //@樊雷伟 , 这个方法需要同步.

            //获得两个参数.
            string httpHandlerName = this.GetRequestVal("HttpHandlerName");
            string methodName = this.GetRequestVal("DoMethod");

            var type = System.Type.GetType(httpHandlerName);
            if (type == null)
            {
                BP.WF.HttpHandler.DirectoryPageBase obj = ClassFactory.GetHandlerPage(httpHandlerName) as BP.WF.HttpHandler.DirectoryPageBase;
                if (obj == null)
                    return "err@页面处理类名[" + httpHandlerName + "],没有获取到，请检查拼写错误？";
                obj.context = this.context;
                return obj.DoMethod(obj, methodName);
            }
            else
            {
                BP.WF.HttpHandler.DirectoryPageBase en = Activator.CreateInstance(type, this.context)
                    as BP.WF.HttpHandler.DirectoryPageBase;
                en.context = this.context;
                return en.DoMethod(en, methodName);
            }
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

            ht.Add("CustomerNo", BP.Sys.SystemConfig.CustomerNo);
            ht.Add("CustomerName", BP.Sys.SystemConfig.CustomerName);

            ht.Add("GroupNo", WebUser.GroupNo);


            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 当前登录人员信息
        /// </summary>
        /// <returns></returns>
        public string GuestUser_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("No", GuestUser.No);
            ht.Add("Name", GuestUser.Name);
            ht.Add("DeptNo", GuestUser.DeptNo);
            ht.Add("DeptName", GuestUser.DeptName);
            return BP.Tools.Json.ToJson(ht);
        }


        /// <summary>
        /// 实体Entity 文件上传
        /// </summary>
        /// <returns></returns>

        public string EntityAth_Upload()
        {
            HttpFileCollection files = context.Request.Files;
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
            if (fileName.IndexOf("\\") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            //文件后缀
            string ext = System.IO.Path.GetExtension(files[0].FileName);
            ext = ext.Replace(".", ""); //去掉点 @李国文

            //文件大小
            float size = files[0].ContentLength / 1024;

            //保存位置
            string filepath = "";


            //如果是天业集团则保存在ftp服务器上
            if (SystemConfig.CustomerNo.Equals("TianYe") || SystemConfig.IsUploadFileToFTP == true)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp = SystemConfig.PathOfTemp + "" + guid + ".tmp";
                try
                {
                    files[0].SaveAs(temp);
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(temp);
                    files[0].SaveAs(temp);
                }

                /*保存到fpt服务器上.*/
                FtpSupport.FtpConnection ftpconn = new FtpSupport.FtpConnection(SystemConfig.FTPServerIP,
                    SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);

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
                filepath = ny + "//Helper//" + guid + ext;

            }
            else
            {
                string fileSavePath = en.EnMap.FJSavePath;

                if (DataType.IsNullOrEmpty(fileSavePath) == true)
                    fileSavePath = BP.Sys.SystemConfig.PathOfDataUser + enName;

                if (System.IO.Directory.Exists(fileSavePath) == false)
                    System.IO.Directory.CreateDirectory(fileSavePath);

                filepath = fileSavePath + "\\" + this.PKVal + "." + ext;

                //存在文件则删除
                if (System.IO.File.Exists(filepath) == true)
                    System.IO.File.Delete(filepath);

                FileInfo info = new FileInfo(filepath);
                files[0].SaveAs(filepath);
            }

            //需要这样写 @李国文.
            en.SetValByKey("MyFileName", fileName);
            en.SetValByKey("MyFilePath", filepath);
            en.SetValByKey("MyFileExt", ext);
            en.SetValByKey("MyFileSize", size);
            en.SetValByKey("WebPath", filepath);

            en.Update();
            return "文件保存成功";
        }


        public string EntityMultiAth_Upload()
        {
            HttpFileCollection files = context.Request.Files;
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
            if (fileName.IndexOf("\\") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            //文件后缀
            string ext = System.IO.Path.GetExtension(files[0].FileName);

            //文件大小
            float size = files[0].ContentLength / 1024;

            //保存位置
            string filepath = "";

            //如果是天业集团则保存在ftp服务器上
            if (SystemConfig.CustomerNo.Equals("TianYe") || SystemConfig.IsUploadFileToFTP == true)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp = SystemConfig.PathOfTemp + "" + guid + ".tmp";
                try
                {
                    files[0].SaveAs(temp);
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(temp);
                    files[0].SaveAs(temp);
                }

                /*保存到fpt服务器上.*/
                FtpSupport.FtpConnection ftpconn = new FtpSupport.FtpConnection(SystemConfig.FTPServerIP,
                    SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);

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
                filepath = ny + "//Helper//" + guid + ext;

            }
            else
            {

                string savePath = BP.Sys.SystemConfig.PathOfDataUser + enName + this.PKVal;

                if (System.IO.Directory.Exists(savePath) == false)
                    System.IO.Directory.CreateDirectory(savePath);
                filepath = savePath + "\\" + fileName + ext;
                //存在文件则删除
                if (System.IO.Directory.Exists(filepath) == true)
                    System.IO.Directory.Delete(filepath);

                FileInfo info = new FileInfo(filepath);

                files[0].SaveAs(filepath);
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


            //把map信息放入
            ht.Add("PhysicsTable", map.PhysicsTable);
            ht.Add("CodeStruct", map.CodeStruct);
            ht.Add("CodeLength", map.CodeLength);

            //查询条件.
            if (map.IsShowSearchKey == true)
                ht.Add("IsShowSearchKey", 1);
            else
                ht.Add("IsShowSearchKey", 0);

            //按日期查询.
            ht.Add("DTSearchWay", (int)map.DTSearchWay);
            ht.Add("DTSearchLable", map.DTSearchLable);
            ht.Add("DTSearchKey", map.DTSearchKey);

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

            AttrSearchs attrs = map.SearchAttrs;
            foreach (AttrSearch item in attrs)
            {
                DataRow dr = dt.NewRow();
                dr["Field"] = item.Key;
                dr["Name"] = item.HisAttr.Desc;
                dr["MyFieldType"] = item.HisAttr.MyFieldType;
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
            string reAttrs = this.GetRequestVal("Attrs");

            //判断是否已经选择分组
            bool contentFlag = false;
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType == UIContralType.DDL)
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
            //// 查询出来关于它的活动列配置。
            //ActiveAttrs aas = new ActiveAttrs();
            //aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            //获取注册信息表
            UserRegedit ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");

            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.Columns.Add("Name");
            dt.Columns.Add("Checked");

            dt.TableName = "Attrs";

            //如果不存在分析项手动添加一个分析项
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
            ds.Tables.Add(ddlDt);

            foreach (Attr attr in map.Attrs)
            {
                if (attr.IsPK || attr.IsNum == false)
                    continue;
                if (attr.UIContralType == UIContralType.TB == false)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                if (attr.MyFieldType == FieldType.FK)
                    continue;
                if (attr.MyFieldType == FieldType.Enum)
                    continue;
                if (attr.Key == "OID" || attr.Key == "WorkID" || attr.Key == "MID")
                    continue;


                //bool isHave = false;
                //// 有没有配置抵消它的属性。
                //foreach (ActiveAttr aa in aas)
                //{
                //    if (aa.AttrKey != attr.Key)
                //        continue;
                //    DataRow dr = dt.NewRow();
                //    dr["Field"] = attr.Key;
                //    dr["Name"] = attr.Desc;

                //    // 根据状态 设置信息.
                //    if (ur.Vals.IndexOf(attr.Key) != -1)
                //        dr["Checked"] = "true";

                //    dt.Rows.Add(dr);

                //    isHave = true;
                //}

                //if (isHave)
                //    continue;


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

                if (this.IsContainsNDYF)
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
            UserRegedit ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");

            // 查询出来关于它的活动列配置.
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            ds = GroupSearchSet(ens, en, map, ur, ds, aas);
            if (ds == null)
                return "info@<img src='../Img/Warning.gif' /><b><font color=red> 您没有选择显示内容/分析项目</font></b>";

            //不显示合计列。
            string NoShowSum = SystemConfig.GetConfigXmlEns("NoShowSum", this.EnsName);
            DataTable showSum = new DataTable("NoShowSum");
            showSum.Columns.Add("NoShowSum");
            DataRow sumdr = showSum.NewRow();
            sumdr["NoShowSum"] = NoShowSum;
            showSum.Rows.Add(sumdr);

            DataTable activeAttr = aas.ToDataTable();
            activeAttr.TableName = "ActiveAttr";
            ds.Tables.Add(activeAttr);
            ds.Tables.Add(showSum);

            return BP.Tools.Json.ToJson(ds);
        }

        private DataSet GroupSearchSet(Entities ens, Entity en, Map map, UserRegedit ur, DataSet ds, ActiveAttrs aas)
        {

            //查询条件
            //分组
            string groupKey = "";
            Attrs AttrsOfNum = new Attrs();//列
            string Condition = ""; //处理特殊字段的条件问题。


            //根据注册表信息获取里面的分组信息
            string StateNumKey = ur.Vals.Substring(ur.Vals.IndexOf("@StateNumKey") + 1);

            string[] statNumKeys = StateNumKey.Split('@');
            foreach (string ct in statNumKeys)
            {
                if (ct.Split('=').Length != 2)
                    continue;
                string[] paras = ct.Split('=');

                //判断paras[0]的类型
                int dataType = 2;
                if (paras[0].Equals("Group_Number"))
                {
                    AttrsOfNum.Add(new Attr("Group_Number", "Group_Number", 1, DataType.AppInt, false, "数量"));
                }
                else
                {
                    Attr attr = map.GetAttrByKey(paras[0]);
                    AttrsOfNum.Add(attr);
                    dataType = attr.MyDataType;
                }

                if (this.GetRequestVal("DDL_" + paras[0]) == null)
                {
                    ActiveAttr aa = (ActiveAttr)aas.GetEnByKey(ActiveAttrAttr.AttrKey, paras[0]);
                    if (aa == null)
                        continue;

                    Condition += aa.Condition;
                    groupKey += " round (" + aa.Exp + ", 4)  \"" + paras[0] + "\",";
                    StateNumKey += paras[0] + "=Checked@"; // 记录状态
                    continue;
                }

                if (paras[0].Equals("Group_Number"))
                {
                    groupKey += " count(*) \"" + paras[0] + "\",";
                }
                else
                {
                    switch (paras[1])
                    {
                        case "SUM":
                            if (dataType == 2)
                                groupKey += " SUM(" + paras[0] + ") \"" + paras[0] + "\",";
                            else
                                groupKey += " round ( SUM(" + paras[0] + "), 4) \"" + paras[0] + "\",";
                            break;
                        case "AVG":
                            groupKey += " round (AVG(" + paras[0] + "), 4)  \"" + paras[0] + "\",";
                            break;
                        case "AMOUNT":
                            if (dataType == 2)
                                groupKey += " SUM(" + paras[0] + ") \"" + paras[0] + "\",";
                            else
                                groupKey += " round ( SUM(" + paras[0] + "), 4) \"" + paras[0] + "\",";
                            break;
                        default:
                            throw new Exception("没有判断的情况.");
                    }

                }

            }
            bool isHaveLJ = false; // 是否有累计字段。
            if (StateNumKey.IndexOf("AMOUNT@") != -1)
                isHaveLJ = true;


            if (groupKey == "")
            {
                return null;
            }

            /* 如果包含累计数据，那它一定需要一个月份字段。业务逻辑错误。*/
            groupKey = groupKey.Substring(0, groupKey.Length - 1);
            BP.DA.Paras ps = new Paras();
            // 生成 sql.
            string selectSQL = "SELECT ";
            string groupBy = " GROUP BY ";
            Attrs AttrsOfGroup = new Attrs();

            string SelectedGroupKey = ur.Vals.Substring(0, ur.Vals.IndexOf("@StateNumKey")); // 为保存操作状态的需要。
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                bool isSelected = false;
                string[] SelectedGroupKeys = SelectedGroupKey.Split('@');
                foreach (string key in SelectedGroupKeys)
                {
                    if (key.Contains("=") == true)
                        continue;
                    //if (key.Equals("Group_Number"))

                    selectSQL += key + " \"" + key + "\",";
                    groupBy += key + ",";
                    // 加入组里面。
                    AttrsOfGroup.Add(map.GetAttrByKey(key), false, false);

                }
            }

            string groupList = this.GetRequestVal("GroupList");
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                /* 如果是年月 分组， 并且如果内部有 累计属性，就强制选择。*/
                if (groupList.IndexOf("FK_NY") != -1 && isHaveLJ)
                {
                    selectSQL += "FK_NY,";
                    groupBy += "FK_NY,";
                    SelectedGroupKey += "@FK_NY";
                    // 加入组里面。
                    AttrsOfGroup.Add(map.GetAttrByKey("FK_NY"), false, false);
                }
            }

            groupBy = groupBy.Substring(0, groupBy.Length - 1);

            if (groupBy.Trim().Equals("GROUP BY"))
            {
                return null;
            }

            // 查询语句的生成
            string where = " WHERE ";
            string whereOfLJ = " WHERE "; // 累计的where.
            string url = "";
            Paras myps = new Paras();
            //获取查询的注册表
            BP.Sys.UserRegedit searchUr = new UserRegedit();
            searchUr.MyPK = WebUser.No + "_" + this.EnsName + "_SearchAttrs";
            searchUr.RetrieveFromDBSources();

            #region 查询条件
            //关键字查询
            string keyWord = searchUr.SearchKey;
            AtPara ap = new AtPara(searchUr.Vals);
            if (en.EnMap.IsShowSearchKey && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
            {
                string whereLike = "";
                bool isAddAnd = false;
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.IsNum)
                        continue;
                    if (attr.IsRefAttr)
                        continue;

                    switch (attr.Field)
                    {
                        case "MyFileExt":
                        case "MyFilePath":
                        case "WebPath":
                            continue;
                        default:
                            break;
                    }
                    if (isAddAnd == false)
                    {
                        isAddAnd = true;
                        whereLike += "      " + attr.Field + " LIKE '%" + keyWord + "%' ";
                    }
                    else
                    {
                        whereLike += "   AND   " + attr.Field + " LIKE '%" + keyWord + "%'";
                    }
                }
                whereLike += "          ";
                where += whereLike;
            }

            //其余查询条件
            //时间
            if (map.DTSearchWay != DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                //按日期查询
                if (map.DTSearchWay == DTSearchWay.ByDate)
                {

                    dtTo += " 23:59:59";
                    where += " and (" + map.DTSearchKey + " >= '" + dtFrom + "'";
                    where += " and " + map.DTSearchKey + " <= '" + dtTo + "'";
                    where += ")";
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

                    where += " and (" + map.DTSearchKey + " >= '" + dtFrom + "'";
                    where += " and " + map.DTSearchKey + " <= '" + dtTo + "'";
                    where += ")";
                }
            }
            /// #region 获得查询数据.
            foreach (string str in ap.HisHT.Keys)
            {
                Object val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                {
                    continue;
                }
                where += " " + str + "=" + SystemConfig.AppCenterDBVarStr + str + "   AND ";
                if (str != "FK_NY")
                    whereOfLJ += " " + str + " =" + SystemConfig.AppCenterDBVarStr + str + "   AND ";

                myps.Add(str, val);

            }

            #endregion

            if (where == " WHERE ")
            {
                where = "" + Condition.Replace("and", "");
                whereOfLJ = "" + Condition.Replace("and", "");
            }
            else
            {
                where = where.Substring(0, where.Length - " AND ".Length) + Condition;
                whereOfLJ = whereOfLJ.Substring(0, whereOfLJ.Length - " AND ".Length) + Condition;
            }

            string orderByReq = this.GetRequestVal("OrderBy");

            string orderby = "";

            if (orderByReq != null && (selectSQL.Contains(orderByReq) || groupKey.Contains(orderByReq)))
            {
                orderby = " ORDER BY " + orderByReq;
                string orderWay = this.GetRequestVal("OrderWay");
                if (!DataType.IsNullOrEmpty(orderWay) && !orderWay.Equals("Up"))
                    orderby += " DESC ";
            }

            // 组装成需要的 sql 
            string sql = selectSQL + groupKey + " FROM " + map.PhysicsTable + where + groupBy + orderby;


            myps.SQL = sql;
            //  物理表。

            DataTable dt2 = DBAccess.RunSQLReturnTable(myps);

            DataTable dt1 = dt2.Clone();

            dt1.Columns.Add("IDX", typeof(int));

            #region 对他进行分页面

            int myIdx = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                myIdx++;
                DataRow mydr = dt1.NewRow();
                mydr["IDX"] = myIdx;
                foreach (DataColumn dc in dt2.Columns)
                {
                    mydr[dc.ColumnName] = dr[dc.ColumnName];
                }
                dt1.Rows.Add(mydr);
            }
            #endregion

            #region 处理 Int 类型的分组列。
            DataTable dt = dt1.Clone();
            dt.TableName = "GroupSearch";
            dt.Rows.Clear();
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns[attr.Key].DataType = typeof(string);
            }
            foreach (DataRow dr in dt1.Rows)
            {
                dt.ImportRow(dr);
            }
            #endregion

            // 处理这个物理表 , 如果有累计字段, 就扩展它的列。
            if (isHaveLJ)
            {
                // 首先扩充列.
                foreach (Attr attr in AttrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppInt:
                            dt.Columns.Add(attr.Key + "Amount", typeof(int));
                            break;
                        default:
                            dt.Columns.Add(attr.Key + "Amount", typeof(decimal));
                            break;
                    }
                }

                // 添加累计汇总数据.
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (Attr attr in AttrsOfNum)
                    {
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                            continue;

                        //形成查询sql.
                        if (whereOfLJ.Length > 10)
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + map.PhysicsTable + whereOfLJ + " AND ";
                        else
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + map.PhysicsTable + " WHERE ";

                        foreach (Attr attr1 in AttrsOfGroup)
                        {
                            switch (attr1.Key)
                            {
                                case "FK_NY":
                                    sql += " FK_NY <= '" + dr["FK_NY"] + "' AND FK_ND='" + dr["FK_NY"].ToString().Substring(0, 4) + "' AND ";
                                    break;
                                case "FK_Dept":
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                                case "FK_SJ":
                                case "FK_XJ":
                                    sql += attr1.Key + " LIKE '" + dr[attr1.Key] + "%' AND ";
                                    break;
                                default:
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                            }
                        }

                        sql = sql.Substring(0, sql.Length - "AND ".Length);
                        if (attr.MyDataType == DataType.AppInt)
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValInt(sql, 0);
                        else
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    }
                }
            }

            // 为表扩充外键
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns.Add(attr.Key + "T", typeof(string));
            }
            foreach (Attr attr in AttrsOfGroup)
            {
                if (attr.UIBindKey.IndexOf(".") == -1)
                {
                    /* 说明它是枚举类型 */
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int val = 0;
                        try
                        {
                            val = int.Parse(dr[attr.Key].ToString());
                        }
                        catch
                        {
                            dr[attr.Key + "T"] = " ";
                            continue;
                        }

                        foreach (SysEnum se in ses)
                        {
                            if (se.IntKey == val)
                                dr[attr.Key + "T"] = se.Lab;
                        }
                    }
                    continue;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    Entity myen = attr.HisFKEn;
                    string val = dr[attr.Key].ToString();
                    myen.SetValByKey(attr.UIRefKeyValue, val);
                    try
                    {
                        myen.Retrieve();
                        dr[attr.Key + "T"] = myen.GetValStrByKey(attr.UIRefKeyText);
                    }
                    catch
                    {
                        if (val == null || val.Length <= 1)
                        {
                            dr[attr.Key + "T"] = val;
                        }
                        else if (val.Substring(0, 2) == "63")
                        {
                            try
                            {
                                BP.Port.Dept Dept = new BP.Port.Dept(val);
                                dr[attr.Key + "T"] = Dept.Name;
                            }
                            catch
                            {
                                dr[attr.Key + "T"] = val;
                            }
                        }
                        else
                        {
                            dr[attr.Key + "T"] = val;
                        }
                    }
                }
            }
            ds.Tables.Add(dt);
            ds.Tables.Add(AttrsOfNum.ToMapAttrs.ToDataTableField("AttrsOfNum"));
            ds.Tables.Add(AttrsOfGroup.ToMapAttrs.ToDataTableField("AttrsOfGroup"));
            return ds;
        }
        public string ParseExpToDecimal()
        {
            string exp = this.GetRequestVal("Exp");

            decimal d = DataType.ParseExpToDecimal(exp);
            return d.ToString();
        }

        /// <summary>
        /// 执行导出
        /// </summary>
        /// <returns></returns>
        public string Group_Exp()
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

            // 查询出来关于它的活动列配置.
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            ds = GroupSearchSet(ens, en, map, ur, ds, aas);
            if (ds == null)
                return "info@<img src='../Img/Warning.gif' /><b><font color=red> 您没有选择分析的数据</font></b>";

            string filePath = ExportGroupExcel(ds, en.EnDesc, ur.Vals);


            return filePath;
        }

        public bool IsContainsNDYF
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
                qo.AddWhere("FK_MapData", "=", FK_MapData);
                qo.addAnd();
                qo.AddWhere("AttrKey", "=", AttrKey);
                qo.addAnd();
                qo.AddWhere("FK_Emp", "=", WebUser.No);
                qo.addAnd();
                qo.AddWhere("LB", "=", "1");

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

                ds.Tables.Add(dvs.ToDataTableField("MainTable")); //把描述加入.
            }
            if (lb == "hisWords")
            {
                Node nd = new Node(this.FK_Node);
                string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt";
                if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    MapData mapData = new MapData(this.FK_MapData);
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
            return BP.Tools.Json.ToJson(ds);


        }

        /// <summary>
        /// 注意特殊字符的处理
        /// </summary>
        /// <returns></returns>
        private string readTxt()
        {
            try
            {
                string path = BP.Sys.SystemConfig.PathOfDataUser + "Fastenter\\" + FK_MapData + "\\" + GetRequestVal("AttrKey"); ;
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
                        dt.Rows[count]["MyPk"] = BP.DA.DBAccess.GenerGUID();

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

    }

}
