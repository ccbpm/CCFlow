using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using BP.Web;
using BP.Difference;
using Newtonsoft.Json.Linq;

namespace BP.Sys
{

    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFSearch : EntityNoName
    {
        public string FK_SFDBSrc
        {
            get
            {
                return this.GetValStringByKey("FK_SFDBSrc");
            }
        }
        public string SelectStatement
        {
            get
            {
                return this.GetValStringByKey("SelectStatement");
            }
        }

        #region 数据源属性.
        /// <summary>
        /// 获得外部数据表
        /// </summary>
        public DataTable GenerHisDataTable(Hashtable ht = null)
        {
            //创建数据源.
            SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);
            DataTable dt = null;

            #region WebApi接口
            if (src.DBSrcType.Equals("WebApi") == true)
            {
                //执行POST
                string postData = Data_WebApi(ht);
                string jsonNode = this.GetValStringByKey("JsonNode"); //json的节点.
                JToken jToken = JToken.Parse(postData);

                // 如果是JSON数组
                if (jToken.Type == JTokenType.Array)
                {
                    JArray arr = (JArray)jToken;
                    if (arr.Count == 0)
                        dt = new DataTable();
                    else
                        dt = BP.Tools.Json.ConvertToDataTable(arr);
                }
                // 如果是JSON对象
                if (jToken.Type == JTokenType.Object)
                {
                    JObject jsonItem = (JObject)jToken;
                    if (!DataType.IsNullOrEmpty(jsonNode) && jsonItem.ContainsKey(jsonNode))
                    {
                        JArray newJsonArr = new JArray();
                        JToken jToken1 = jsonItem[jsonNode];
                        // 判断当前是不是数组或者
                        if (jToken1.Type == JTokenType.Array)
                        {
                            newJsonArr = (JArray)jToken1;
                            return BP.Tools.Json.ToDataTable(newJsonArr.ToString());
                        }
                        if (jToken1.Type == JTokenType.Object)
                        {
                            JObject targetObj = (JObject)jToken;
                            return BP.Tools.Json.ToDataTable(targetObj.ToString());
                        }
                        throw new Exception("指定的RootNode下不是JSON数组 或 JSON对象");
                    } 

                    dt = BP.Tools.Json.ToDataTable(jsonItem.ToString());
                }

                if (dt == null)
                    throw new Exception("Web_API 没有正确返回JSON字符串");
                return dt;
            }
            #endregion WebApi接口

            #region SQL接口
            string runObj = this.SelectStatement;
            if (DataType.IsNullOrEmpty(runObj))
                throw new Exception("@外键类型SQL配置错误," + this.No + " " + this.Name + " 是一个(SQL)类型(" + this.GetValStrByKey("SrcType") + ")，但是没有配置sql.");

            if (runObj == null)
                runObj = string.Empty;

            runObj = runObj.Replace("~", "'");
            runObj = runObj.Replace("/#", "+"); //为什么？
            runObj = runObj.Replace("/$", "-"); //为什么？
            if (runObj.Contains("@WebUser.No"))
                runObj = runObj.Replace("@WebUser.No", BP.Web.WebUser.No);

            if (runObj.Contains("@WebUser.Name"))
                runObj = runObj.Replace("@WebUser.Name", BP.Web.WebUser.Name);

            if (runObj.Contains("@WebUser.FK_DeptName"))
                runObj = runObj.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);

            if (runObj.Contains("@WebUser.FK_Dept"))
                runObj = runObj.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

            if (runObj.Contains("@") == true && ht != null)
            {
                foreach (string key in ht.Keys)
                {
                    //值为空或者null不替换
                    if (ht[key] == null || ht[key].Equals("") == true)
                        continue;

                    if (runObj.Contains("@" + key))
                        runObj = runObj.Replace("@" + key, ht[key].ToString());
                    //不包含@则返回SQL语句
                    if (runObj.Contains("@") == false)
                        break;
                }
            }
            if (runObj.Contains("@") && BP.Difference.SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    runObj = runObj.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }
            }
            if (runObj.Contains("@") == true)
                throw new Exception("@外键类型SQL错误," + runObj + "部分查询条件没有被替换.");
            try
            {
                dt = src.RunSQLReturnTable(runObj);
            }
            catch (Exception ex)
            {
                throw new Exception("err@获得SFSearch(" + this.No + "," + this.Name + ")出现错误:SQL[" + runObj + "],数据库异常信息:" + ex.Message);
            }
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                if (dt.Columns.Count == 3)
                    dt.Columns[2].ColumnName = "ParentNo";
            }
            return dt;
            #endregion SQL接口
        }

        private string Data_WebApi(Hashtable ht)
        {
            //返回值
            //用户输入的webAPI地址
            string apiUrl = this.SelectStatement;

            #region 解析路径变量 /{xxx}/{yyy} ? xxx=xxx
            if (apiUrl.Contains("{") == true)
            {
                if (ht != null)
                {
                    foreach (string key in ht.Keys)
                    {
                        apiUrl = apiUrl.Replace("{" + key + "}", ht[key].ToString());
                    }
                }
            }
            if (apiUrl.StartsWith("htt") == false)
            {
                var mysrc = new SFDBSrc(this.FK_SFDBSrc);
                apiUrl = mysrc.ConnString + apiUrl;
            }
            #endregion

            //执行POST
            return BP.Tools.PubGlo.HttpPostConnect(apiUrl, "", this.GetValStringByKey("RequestMethod"));
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFSearch()
        {
        }
        public SFSearch(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFSearch", "查询");

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddDDLStringEnum(SFTableAttr.DictSrcType, "SQL", "数据源类型", "@SQL=SQL@WebApi=WebApi", false);
                map.AddTBString(SFTableAttr.SelectStatement, null, "表达式", true, false, 0, 1000, 600, true);
                map.AddDDLStringEnum("RequestMethod", "Get", "请求模式", "@Get=Get@POST=POST", true);
                map.AddTBString("JsonNode", "", "Json根节点", true, false, 0, 200, 600, true);

                map.AddTBDateTime(SFTableAttr.RDT, null, "创建日期", false, false);
                map.AddTBString(SFTableAttr.OrgNo, null, "组织编号", false, false, 0, 100, 20);
                map.AddTBString(SFTableAttr.AtPara, null, "AtPara", false, false, 0, 50, 20);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 通过webapi获得json数据.
        /// </summary>
        /// <param name="paras">参数:@Key=val@Key2=Val2</param>
        /// <returns>json</returns>
        /// <exception cref="Exception"></exception>
        public string GenerDataOfJson(string paras)
        {
            //把参数转化为 ht.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPKVal", this.No);

            //获得ht.
            Hashtable ht = SFTable.GenerHT(paras, ens); // DataType.ParseParasToHT(paras);

            DataTable dt = this.GenerHisDataTable(ht);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="paras">参数</param>
        /// <param name="slnNo">方案编号：也就是mapExt的MyPK</param>
        /// <returns>转换的json</returns>
        public string GenerDataOfJsonUesingSln(string paras, string slnNo)
        {
            //把参数转化为 ht.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPKVal", this.No);

            //获得ht.参数.
            Hashtable ht = SFTable.GenerHT(paras, ens); // DataType.ParseParasToHT(paras);
            //获得原始数据.
            DataTable dt = this.GenerHisDataTable(ht);

            //获得转换方案.
            SFColumnSlns colSlns = new SFColumnSlns();
            colSlns.Retrieve("RefPKVal", slnNo, "IsEnable", 1);

            foreach (SFColumnSln sln in colSlns)
            {
                string attrKey = sln.Row["AttrKey"].ToString();
                string toField = sln.Row["ToField"].ToString();
                if (DataType.IsNullOrEmpty(toField) == true)
                    continue;

                if (dt.Columns.Contains(attrKey) == true)
                    dt.Columns[attrKey].ColumnName = toField; //修改列名.
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 检查是否有依赖的引用？
        /// </summary>
        /// <returns></returns>
        public string IsCanDelete()
        {
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (mattrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in mattrs)
                    err += " @ " + item.MyPK + " " + item.Name;
                return "err@如下实体字段在引用:" + err + "。您不能删除该表。";
            }
            return null;
        }
        protected override bool beforeDelete()
        {
            string delMsg = this.IsCanDelete();
            if (delMsg != null)
                throw new Exception(delMsg);

            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFSearchs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFSearchs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFSearch();
            }
        }
        /// <summary>
        ///  重写查询全部的方法
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll("RDT");
            return this.Retrieve("OrgNo", WebUser.OrgNo, "RDT");
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFSearch> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFSearch>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFSearch> Tolist()
        {
            System.Collections.Generic.List<SFSearch> list = new System.Collections.Generic.List<SFSearch>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFSearch)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
