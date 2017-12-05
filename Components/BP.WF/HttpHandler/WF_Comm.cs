using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
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
        #region Refmethod.htm 相关功能.
        public string RefEnKey
        {
            get
            {
                string str = this.GetRequestVal("No");
                if (str == null)
                    str = this.GetRequestVal("OID");

                if (str == null)
                    str = this.GetRequestVal("MyPK");

                if (str == null)
                    str = this.GetRequestVal("PK");

                return str;
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
                            string str = this.GetValFromFrmByKey(attr.Key); 
                            objs[idx] = str;
                            attr.DefaultVal = str;
                        }
                        catch
                        {
                            objs[idx] = null;
                        }
                        break;
                    case UIContralType.CheckBok:
                        bool val = this.GetValBoolenFromFrmByKey(attr.Key);
                        if (val==true)
                            objs[idx] = "1";
                        else
                            objs[idx] = "0";
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

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 界面 .
        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <returns></returns>
        public string Entity_Init()
        {
            return "";
        }
        public string Entity_Save()
        {
            return "";
        }
        public string Entity_Delete()
        {
            return "";
        }
        #endregion 界面方法.

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

    }
}
