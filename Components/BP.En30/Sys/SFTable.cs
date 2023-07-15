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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using NetTaste;

namespace BP.Sys
{
    /// <summary>
    /// 表数据来源类型
    /// </summary>
    public class DictSrcType
    {
        /// <summary>
        /// 本地的类
        /// </summary>
        public const string BPClass = "BPClass";
        /// <summary>
        /// 通过ccform创建表
        /// </summary>
        public const string CreateTable = "CreateTable";
        /// <summary>
        /// 表或视图
        /// </summary>
        public const string TableOrView = "TableOrView";
        /// <summary>
        /// SQL查询数据
        /// </summary>
        public const string SQL = "SQL";
        /// <summary>
        /// WebServices
        /// </summary>
        public const string WebServices = "WebServices";
        /// <summary>
        /// hand
        /// </summary>
        public const string Handler = "Handler";
        /// <summary>
        /// JS请求数据.
        /// </summary>
        public const string JQuery = "JQuery";
        /// <summary>
        /// 系统字典表
        /// </summary>
        public const string SysDict = "SysDict";
        /// <summary>
        /// WebApi接口
        /// </summary>
        public const string WebApi = "WebApi";
    }
    /// <summary>
    /// 编码表类型
    /// </summary>
    public enum CodeStruct
    {
        /// <summary>
        /// 普通的编码表
        /// </summary>
        NoName,
        /// <summary>
        /// 树编码表(No,Name,ParentNo)
        /// </summary>
        Tree,
        /// <summary>
        /// 行政机构编码表
        /// </summary>
        GradeNoName
    }

    /// <summary>
    /// 编号生成规则
    /// </summary>
    public enum NoGenerModel
    {
        /// <summary>
        /// 自定义
        /// </summary>
        None,
        /// <summary>
        /// 流水号
        /// </summary>
        ByLSH,
        /// <summary>
        /// 标签的全拼
        /// </summary>
        ByQuanPin,
        /// <summary>
        /// 标签的简拼
        /// </summary>
        ByJianPin,
        /// <summary>
        /// 按GUID生成
        /// </summary>
        ByGUID
    }
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTableAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 字段
        /// </summary>
        public const string FK_Val = "FK_Val";
        /// <summary>
        /// 数据表描述
        /// </summary>
        public const string TableDesc = "TableDesc";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 是否是树
        /// </summary>
        public const string IsTree = "IsTree";
        /// <summary>
        /// 表类型
        /// </summary>
        public const string DictSrcType = "DBSrcType";
        /// <summary>
        /// 字典表类型
        /// </summary>
        public const string CodeStruct = "CodeStruct";
        /// <summary>
        /// 是否自动生成编号
        /// </summary>
        public const string IsAutoGenerNo = "IsAutoGenerNo";
        /// <summary>
        /// 编号生成规则
        /// </summary>
        public const string NoGenerModel = "NoGenerModel";

        #region 链接到其他系统获取数据的属性。
        /// <summary>
        /// 数据源
        /// </summary>
        public const string FK_SFDBSrc = "FK_SFDBSrc";
        /// <summary>
        /// 数据源表
        /// </summary>
        public const string SrcTable = "SrcTable";
        /// <summary>
        /// 显示的值
        /// </summary>
        public const string ColumnValue = "ColumnValue";
        /// <summary>
        /// 显示的文字
        /// </summary>
        public const string ColumnText = "ColumnText";
        /// <summary>
        /// 父结点值
        /// </summary>
        public const string ParentValue = "ParentValue";
        /// <summary>
        /// 查询语句
        /// </summary>
        public const string SelectStatement = "SelectStatement";
        /// <summary>
        /// 缓存分钟数
        /// </summary>
        public const string CashMinute = "CashMinute";
        /// <summary>
        /// 最近缓存的时间
        /// </summary>
        public const string RootVal = "RootVal";
        /// <summary>
        /// 加入日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        #endregion 链接到其他系统获取数据的属性。
        /// <summary>
        /// AtPara
        /// </summary>
        public const string AtPara = "AtPara";
    }
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTable : EntityNoName
    {
        #region 数据源属性.
        /// <summary>
        /// 判断是否存在 @honyan. 
        /// </summary>
        public override bool IsExits
        {
            get
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    return base.IsExits;

                this.No = BP.Web.WebUser.OrgNo + "_" + this.No;
                return base.IsExits;
            }
        }

        /// <summary>
        /// 获得webApi数据
        /// </summary>
        /// <param name="ht">参数</param>
        /// <param name="requestMethod">POST/GET</param>
        /// <param name="sfDBSrcNo">数据源</param>
        /// <param name="SelectStatement">执行部分</param>
        /// <returns>返回的数据</returns>
        public static string Data_WebApi(Hashtable ht, string requestMethod, string sfDBSrcNo, string SelectStatement)
        {
            //返回值
            //用户输入的webAPI地址
            string apiUrl = SelectStatement;
            if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                apiUrl = apiUrl.Replace("@WebApiHost", BP.Difference.SystemConfig.AppSettings["WebApiHost"]);

            var mysrc = new SFDBSrc(sfDBSrcNo);

            #region  GET 解析路径变量 /{xxx}/{yyy} ? xxx=xxx
            if (requestMethod.ToUpper().Equals("GET") == true)
            {
                if (apiUrl.Contains("{") == true)
                {
                    if (ht != null)
                    {
                        foreach (string key in ht.Keys)
                        {
                            apiUrl = apiUrl.Replace("{" + key + "}", ht[key].ToString());
                        }
                    }
                    apiUrl = mysrc.ConnString + apiUrl;
                }
                else
                {
                    apiUrl = mysrc.ConnString + apiUrl;
                }
                return BP.Tools.PubGlo.HttpPostConnect(apiUrl, "", requestMethod);
            }
            #endregion

            if(apiUrl.StartsWith("http")==false)
                apiUrl = mysrc.ConnString + apiUrl;

            return BP.Tools.PubGlo.HttpPostConnect(apiUrl, "", requestMethod);
        }
        /// <summary>
        /// 判断json 是否符合要求
        /// </summary>
        /// <param name="jsonItem"></param>
        /// <param name="fieldNo"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldParentNo"></param>
        /// <exception cref="Exception"></exception>
        private void checkJsonField(JObject jsonItem, string fieldNo, string fieldName, string fieldParentNo)
        {
            // 判断是否为数组嵌套
            if (jsonItem.Type != JTokenType.Object)
                throw new Exception("API返回格式错误，不为标准json数组格式");
            // 判断是否包含所匹配的项
            if (!DataType.IsNullOrEmpty(fieldNo) && !jsonItem.ContainsKey(fieldNo))
                throw new Exception("字典[" + this.No + "]API不包含定义的No列：" + fieldNo);
            if (!DataType.IsNullOrEmpty(fieldName) && !jsonItem.ContainsKey(fieldName))
                throw new Exception("字典[" + this.No + "]API不包含定义的Name列：" + fieldName);

            if (this.CodeStruct == CodeStruct.Tree)
            {
                if (!DataType.IsNullOrEmpty(fieldParentNo) && !jsonItem.ContainsKey(fieldParentNo))
                    throw new Exception("字典[" + this.No + "]API不包含定义的ParentNo列：" + fieldParentNo);
            }
        }
        /// <summary>
        /// 过滤出新的json对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fieldNo"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldParentNo"></param>
        /// <returns></returns>
        private JObject getJSONByTargetName(JObject source, string fieldNo, string fieldName, string fieldParentNo)
        {
            JObject nObj = new JObject();
            if (!DataType.IsNullOrEmpty(fieldNo))
            {
                nObj["No"] = source[fieldNo];
            }

            if (!DataType.IsNullOrEmpty(fieldName))
            {
                nObj["Name"] = source[fieldName];
            }
            if (this.CodeStruct == CodeStruct.Tree)
            {
                if (!DataType.IsNullOrEmpty(fieldParentNo))
                    nObj["ParentNo"] = source[fieldParentNo];
            }
            return nObj;
        }

        /// <summary>
        /// 获得外部数据表
        /// </summary>
        public DataTable GenerHisDataTable(Hashtable ht = null)
        {
            //创建数据源.
            SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

            #region BP类
            if (this.SrcType == BP.Sys.DictSrcType.BPClass)
            {
                Entities ens = ClassFactory.GetEns(this.No);
                return ens.RetrieveAllToTable();
            }
            #endregion

            #region  WebServices
            // this.SrcType == BP.Sys.SrcType.WebServices，by liuxc 
            //暂只考虑No,Name结构的数据源，2015.10.04，added by liuxc
            if (this.SrcType == DictSrcType.WebServices)
            {
                var td = this.TableDesc.Split(','); //接口名称,返回类型
                var ps = (this.SelectStatement ?? string.Empty).Split('&');
                var args = new ArrayList();
                string[] pa = null;

                foreach (var p in ps)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;

                    pa = p.Split('=');
                    if (pa.Length != 2) continue;

                    //此处要 SL 中显示表单时，会有问题
                    try
                    {
                        if (pa[1].Contains("@WebUser.No"))
                            pa[1] = pa[1].Replace("@WebUser.No", BP.Web.WebUser.No);
                        if (pa[1].Contains("@WebUser.Name"))
                            pa[1] = pa[1].Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        if (pa[1].Contains("@WebUser.FK_DeptName"))
                            pa[1] = pa[1].Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                        if (pa[1].Contains("@WebUser.FK_Dept"))
                            pa[1] = pa[1].Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    }
                    catch
                    {
                    }

                    if (pa[1].Contains("@WorkID"))
                        pa[1] = pa[1].Replace("@WorkID", HttpContextHelper.RequestParams("WorkID") ?? "");
                    if (pa[1].Contains("@NodeID"))
                        pa[1] = pa[1].Replace("@NodeID", HttpContextHelper.RequestParams("NodeID") ?? "");
                    if (pa[1].Contains("@FK_Node"))
                        pa[1] = pa[1].Replace("@FK_Node", HttpContextHelper.RequestParams("FK_Node") ?? "");
                    if (pa[1].Contains("@FK_Flow"))
                        pa[1] = pa[1].Replace("@FK_Flow", HttpContextHelper.RequestParams("FK_Flow") ?? "");
                    if (pa[1].Contains("@FID"))
                        pa[1] = pa[1].Replace("@FID", HttpContextHelper.RequestParams("FID") ?? "");

                    args.Add(pa[1]);
                }

                var result = InvokeWebService(src.IP, td[0], args.ToArray());

                if (DataType.IsNullOrEmpty(result as string) == true)
                    throw new Exception("err@获得结果错误.");

                switch (td[1])
                {
                    case "DataSet":
                        return result == null ? new DataTable() : (result as DataSet).Tables[0];
                    case "DataTable":
                        return result as DataTable;
                    case "Json":
                        return BP.Tools.Json.ToDataTable(result as string);
                    //  return dt;
                    case "Xml":
                        if (result == null || string.IsNullOrWhiteSpace(result.ToString()))
                            throw new Exception("@返回的XML格式字符串不正确。");

                        var xml = new XmlDocument();
                        xml.LoadXml(result as string);

                        XmlNode root = null;

                        if (xml.ChildNodes.Count < 2)
                            root = xml.ChildNodes[0];
                        else
                            root = xml.ChildNodes[1];

                        DataTable dt = new DataTable();
                        dt.Columns.Add("No", typeof(string));
                        dt.Columns.Add("Name", typeof(string));

                        foreach (XmlNode node in root.ChildNodes)
                        {
                            dt.Rows.Add(node.SelectSingleNode("No").InnerText,
                                        node.SelectSingleNode("Name").InnerText);
                        }
                        return dt;
                    default:
                        throw new Exception("@不支持的返回类型" + td[1]);
                }
            }
            #endregion

            #region WebApi接口
            if (this.SrcType == DictSrcType.WebApi)
            {
                string postData = Data_WebApi(ht, this.GetValStringByKey("RequestMethod"), this.FK_SFDBSrc, this.SelectStatement);

                // 需要替换的参数
                string fieldNo = this.GetValStringByKey("FieldNo");
                if (DataType.IsNullOrEmpty(fieldNo))
                    fieldNo = "No";

                string fieldName = this.GetValStringByKey("FieldName");
                if (DataType.IsNullOrEmpty(fieldName))
                    fieldName = "Name";

                string fieldParentNo = this.GetValStringByKey("FieldParentNo");
                if (DataType.IsNullOrEmpty(fieldParentNo))
                    fieldParentNo = "ParentNo";

                // 根节点
                string jsonNode = this.GetValStringByKey("JsonNode");

                JToken jToken = JToken.Parse(postData);
                // 如果是JSON数组
                if (jToken.Type == JTokenType.Array)
                {
                    // 新的对象，用来删除原对象无用字段
                    JArray newJsonArr = new JArray();
                    JArray arr = (JArray)jToken;
                    if(arr.Count > 0)
                    {
                        JObject firstItem = (JObject)arr[0];
                        checkJsonField(firstItem, fieldNo, fieldName, fieldParentNo);
                    }
                    foreach (JObject obj in arr)
                    {
                        newJsonArr.Add(getJSONByTargetName(obj, fieldNo, fieldName, fieldParentNo));
                    }
                    return BP.Tools.Json.ConvertToDataTable(newJsonArr);
                }
                // 如果是JSON对象
                if (jToken.Type == JTokenType.Object)
                {
                    JObject jsonItem = (JObject)jToken;
                    // 判断是不是有根节点
                    // 如果有
                    if (!DataType.IsNullOrEmpty(jsonNode) && jsonItem.ContainsKey(jsonNode))
                    {
                        // 新的对象，用来删除原对象无用字段
                        JArray newJsonArr = new JArray();
                        JToken jToken1 = jsonItem[jsonNode];
                        // 判断当前是不是数组或者
                        if (jToken1.Type == JTokenType.Array)
                        {
                            JArray arr = (JArray)jToken1;
                            if (arr.Count > 0)
                            {
                                JObject firstItem = (JObject)arr[0];
                                checkJsonField(firstItem, fieldNo, fieldName, fieldParentNo);
                            }
                            
                            foreach (JObject obj in arr)
                            {
                                newJsonArr.Add(getJSONByTargetName(obj, fieldNo, fieldName, fieldParentNo));
                            }
                            return BP.Tools.Json.ToDataTable(newJsonArr.ToString());
                        }
                        if (jToken1.Type == JTokenType.Object)
                        {
                            JObject targetObj = (JObject)jToken;
                            checkJsonField(targetObj, fieldNo, fieldName, fieldParentNo);
                            JObject itemOfRootNode = getJSONByTargetName(targetObj, fieldNo, fieldName, fieldParentNo);
                            return BP.Tools.Json.ToDataTable(itemOfRootNode.ToString());
                        }
                        throw new Exception("指定的RootNode下不是JSON数组 或 JSON对象");

                    }
                    // 如果没有配置rootNode，检查他本身有没有所需属性
                    JObject currObj = (JObject)jToken;
                    checkJsonField(currObj, fieldNo, fieldName, fieldParentNo);
                    JObject itemOfResponse = getJSONByTargetName(currObj, fieldNo, fieldName, fieldParentNo);
                    return BP.Tools.Json.ToDataTable(itemOfResponse.ToString());
                }
                throw new Exception("Web_API 没有正确返回JSON字符串");
                #endregion
            }
            #endregion WebApi接口

            #region SQL查询.外键表/视图，edited by liuxc,2016-12-29
            if (this.SrcType == DictSrcType.TableOrView)
            {
                string sql = "SELECT " + this.ColumnValue + " No, " + this.ColumnText + " Name";
                if (this.CodeStruct == BP.Sys.CodeStruct.Tree)
                    sql += ", " + this.ParentValue + " ParentNo";

                sql += " FROM " + this.SrcTable;
                DataTable dt = src.RunSQLReturnTable(sql);

                if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                {
                    dt.Columns[0].ColumnName = "No";
                    dt.Columns[1].ColumnName = "Name";
                    if (dt.Columns.Count == 3)
                        dt.Columns[2].ColumnName = "ParentNo";
                }
                return dt;
            }
            #endregion SQL查询.外键表/视图，edited by liuxc,2016-12-29

            #region 动态SQL，edited by liuxc,2016-12-29
            if (this.SrcType == DictSrcType.SQL)
            {
                string runObj = this.SelectStatement;

                if (DataType.IsNullOrEmpty(runObj))
                    throw new Exception("@外键类型SQL配置错误," + this.No + " " + this.Name + " 是一个(SQL)类型(" + this.GetValStrByKey("SrcType") + ")，但是没有配置sql.");

                if (runObj == null)
                    runObj = string.Empty;

                runObj = Glo.DealExp(runObj, ht);
                if (runObj.Contains("@") == true)
                    throw new Exception("@外键类型SQL错误," + runObj + "部分查询条件没有被替换.");

                DataTable dt = null;
                try
                {
                    dt = src.RunSQLReturnTable(runObj);
                }
                catch (Exception ex)
                {
                    throw new Exception("err@获得SFTable(" + this.No + "," + this.Name + ")出现错误:SQL[" + runObj + "],数据库异常信息:" + ex.Message);
                }
                if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                {
                    dt.Columns[0].ColumnName = "No";
                    dt.Columns[1].ColumnName = "Name";
                    if (dt.Columns.Count == 3)
                        dt.Columns[2].ColumnName = "ParentNo";
                }
                return dt;
            }
            #endregion

            #region 自定义表.
            //if (this.SrcType == DictSrcType.CreateTable)
            //{
            //    string sql = "SELECT No, Name FROM " + this.No;
            //    DataTable dt = src.RunSQLReturnTable(sql);
            //    if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            //    {
            //        dt.Columns[0].ColumnName = "No";
            //        dt.Columns[1].ColumnName = "Name";
            //        if (dt.Columns.Count == 3)
            //            dt.Columns[2].ColumnName = "ParentNo";
            //    }
            //    return dt;
            //}

            #endregion

            #region 内置字典表.
            if (this.SrcType == DictSrcType.SysDict || this.SrcType == DictSrcType.CreateTable)
            {
                string sql = "";
                if (this.CodeStruct == CodeStruct.NoName)
                    sql = "SELECT MyPK, BH AS No, Name FROM Sys_SFTableDtl WHERE FK_SFTable='" + this.No + "'";
                else
                    sql = "SELECT MyPK, BH AS No, Name,ParentNo FROM Sys_SFTableDtl WHERE FK_SFTable='" + this.No + "'";
                DataTable dt = src.RunSQLReturnTable(sql);

                if (dt!=null && dt.Rows.Count == 0)
                {
                    if (this.CodeStruct == CodeStruct.NoName)
                    {
                        SFTableDtl dtl = new SFTableDtl();
                        dtl.BH = "001";
                        dtl.Name = "Name 001";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.Insert();

                        dtl = new SFTableDtl();
                        dtl.BH = "002";
                        dtl.Name = "Name 002";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.Insert();

                        dtl = new SFTableDtl();
                        dtl.BH = "003";
                        dtl.Name = "Name 002";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.Insert();
                    }
                    //如果是tree.
                    if (this.CodeStruct == CodeStruct.Tree)
                    {
                        SFTableDtl dtl = new SFTableDtl();
                        dtl.BH = "001";
                        dtl.Name = "根目录";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.ParentNo = "0"; // root 树结构编号.
                        dtl.Insert();

                        dtl = new SFTableDtl();
                        dtl.BH = "002";
                        dtl.Name = "Node 001";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.ParentNo = "001";
                        dtl.Insert();

                        dtl = new SFTableDtl();
                        dtl.BH = "003";
                        dtl.Name = "Node 002";
                        dtl.MyPK = this.No + "_" + dtl.BH;
                        dtl.FK_SFTable = this.No;
                        dtl.ParentNo = "001";
                        dtl.Insert();
                    }
                    dt = src.RunSQLReturnTable(sql);
                }

                if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
                {
                    dt.Columns[0].ColumnName = "No";
                    dt.Columns[1].ColumnName = "Name";
                    if (dt.Columns.Count == 3)
                        dt.Columns[2].ColumnName = "ParentNo";
                }
                return dt;
            }
            #endregion
            throw new Exception("err@没有判断的类型." + this.SrcTable);
        }
        /// <summary>
        /// 修改外键数据
        /// </summary>
        /// <returns></returns>
        public void UpdateData(string No, string Name, string FK_SFTable)
        {
            var sql = "";
            if (this.SrcType == DictSrcType.SysDict)
                sql = "UPDATE Sys_SFTableDtl SET Name = '" + Name + "' WHERE MyPK='" + FK_SFTable + "_" + No + "'";
            else
                sql = "UPDATE " + FK_SFTable + " SET Name = '" + Name + "' WHERE No = '" + No + "'";
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 新增外键数据
        /// </summary>
        /// <returns></returns>
        public void InsertData(string No, string Name, string FK_SFTable)
        {
            var sql = "";
            if (this.SrcType == BP.Sys.DictSrcType.SysDict)
                sql = "insert into  Sys_SFTableDtl(MyPK,FK_SFTable,BH,Name) values('" + FK_SFTable + "_" + No + "','" + FK_SFTable + "','" + No + "','" + Name + "')";
            else
                sql = "insert into  " + FK_SFTable + "(No,Name) values('" + No + "','" + Name + "')";
            DBAccess.RunSQL(sql);
        }

        /// <summary>
        /// 修改了名字.
        /// </summary>
        /// <returns></returns>
        public string GenerJson()
        {
            return BP.Tools.Json.ToJson(this.GenerHisDataTable());
        }

        /// <summary>
        /// 自动生成编号
        /// </summary>
        /// <returns></returns>
        public string GenerSFTableNewNo()
        {
            string table = this.SrcTable;
            NoGenerModel NoGenerModel = this.NoGenerModel;
            if (NoGenerModel == NoGenerModel.ByGUID)//编号按guid生成
            {
                string guid = DBAccess.GenerGUID();
                return guid;
            }
            else if (NoGenerModel == NoGenerModel.ByLSH)//编号按流水号生成
            {
                if (this.SrcType == DictSrcType.SysDict)//如果是按系统字典表
                {
                    try
                    {
                        string sql = null;
                        string field = "BH";
                        switch (this.EnMap.EnDBUrl.DBType)
                        {
                            case DBType.MSSQL:
                                sql = "SELECT CONVERT(INT, MAX(CAST(" + field + " as int)) )+1 AS No FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            case DBType.PostgreSQL:
                            case DBType.UX:
                                sql = "SELECT to_number( MAX(" + field + ") ,'99999999')+1   FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            case DBType.Oracle:
                            case DBType.KingBaseR3:
                            case DBType.KingBaseR6:
                                sql = "SELECT MAX(" + field + ") +1 AS No FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            case DBType.MySQL:
                                sql = "SELECT CONVERT(MAX(CAST(" + field + " AS SIGNED INTEGER)),SIGNED) +1 AS No FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            case DBType.Informix:
                                sql = "SELECT MAX(" + field + ") +1 AS No FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            case DBType.Access:
                                sql = "SELECT MAX( [" + field + "]) +1 AS  No FROM Sys_SFTableDtl where FK_SFTable='" + table + "'";
                                break;
                            default:
                                throw new Exception("error");
                        }
                        string str = DBAccess.RunSQLReturnValInt(sql, 1).ToString();
                        if (str == "0" || str == "")
                            str = "1";
                        return str.PadLeft(3, '0');
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                try
                {
                    string sql = null;
                    string field = "No";
                    switch (this.EnMap.EnDBUrl.DBType)
                    {
                        case DBType.MSSQL:
                            sql = "SELECT CONVERT(INT, MAX(CAST(" + field + " as int)) )+1 AS No FROM " + table;
                            break;
                        case DBType.PostgreSQL:
                        case DBType.UX:
                            sql = "SELECT to_number( MAX(" + field + ") ,'99999999')+1   FROM " + table;
                            break;
                        case DBType.Oracle:
                        case DBType.KingBaseR3:
                        case DBType.KingBaseR6:
                            sql = "SELECT MAX(" + field + ") +1 AS No FROM " + table;
                            break;
                        case DBType.MySQL:
                            sql = "SELECT CONVERT(MAX(CAST(" + field + " AS SIGNED INTEGER)),SIGNED) +1 AS No FROM " + table;
                            break;
                        case DBType.Informix:
                            sql = "SELECT MAX(" + field + ") +1 AS No FROM " + table;
                            break;
                        case DBType.Access:
                            sql = "SELECT MAX( [" + field + "]) +1 AS  No FROM " + table;
                            break;
                        default:
                            throw new Exception("error");
                    }
                    string str = DBAccess.RunSQLReturnValInt(sql, 1).ToString();
                    if (str == "0" || str == "")
                        str = "1";
                    return str.PadLeft(3, '0');
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else   //其他的生成编号默认为空
            {
                return "";
            }
        }
        /// <summary>
        /// 实例化 WebServices
        /// </summary>
        /// <param name="url">WebServices地址</param>
        /// <param name="methodname">调用的方法</param>
        /// <param name="args">把webservices里需要的参数按顺序放到这个object[]里</param>
        public object InvokeWebService(string url, string methodname, object[] args)
        {
            return null;
            /* TODO 2019-07-25 为了合并core，注释掉
                        //这里的namespace是需引用的webservices的命名空间，在这里是写死的，大家可以加一个参数从外面传进来。
                        string @namespace = "BP.RefServices";
                        try
                        {
                            if (url.EndsWith(".asmx"))
                                url += "?wsdl";
                            else if (url.EndsWith(".svc"))
                                url += "?singleWsdl";

                            //获取WSDL
                            WebClient wc = new WebClient();
                            Stream stream = wc.OpenRead(url);
                            ServiceDescription sd = ServiceDescription.Read(stream);
                            string classname = sd.Services[0].Name;
                            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                            sdi.AddServiceDescription(sd, "", "");
                            CodeNamespace cn = new CodeNamespace(@namespace);

                            //生成客户端代理类代码
                            CodeCompileUnit ccu = new CodeCompileUnit();
                            ccu.Namespaces.Add(cn);
                            sdi.Import(cn, ccu);
                            CSharpCodeProvider csc = new CSharpCodeProvider();
                            ICodeCompiler icc = csc.CreateCompiler();

                            //设定编译参数
                            CompilerParameters cplist = new CompilerParameters();
                            cplist.GenerateExecutable = false;
                            cplist.GenerateInMemory = true;
                            cplist.ReferencedAssemblies.Add("System.dll");
                            cplist.ReferencedAssemblies.Add("System.XML.dll");
                            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                            cplist.ReferencedAssemblies.Add("System.Data.dll");

                            //编译代理类
                            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                            if (true == cr.Errors.HasErrors)
                            {
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                                {
                                    sb.Append(ce.ToString());
                                    sb.Append(System.Environment.NewLine);
                                }
                                throw new Exception(sb.ToString());
                            }

                            //生成代理实例，并调用方法
                            System.Reflection.Assembly assembly = cr.CompiledAssembly;
                            Type t = assembly.GetType(@namespace + "." + classname, true, true);
                            object obj = Activator.CreateInstance(t);
                            System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                            return mi.Invoke(obj, args);
                        }
                        catch
                        {
                            return null;
                        }
            */
        }

        #region 链接到其他系统获取数据的属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(SFTableAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string FK_SFDBSrc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_SFDBSrc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_SFDBSrc, value);
            }
        }
        public string FK_SFDBSrcT
        {
            get
            {
                return this.GetValRefTextByKey(SFTableAttr.FK_SFDBSrc);
            }
        }
        /// <summary>
        /// 数据缓存时间
        /// </summary>
        public string RootVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.RootVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.RootVal, value);
            }
        }
        /// <summary>
        /// 同步间隔
        /// </summary>
        public int CashMinute
        {
            get
            {
                return this.GetValIntByKey(SFTableAttr.CashMinute);
            }
            set
            {
                this.SetValByKey(SFTableAttr.CashMinute, value);
            }
        }

        /// <summary>
        /// 物理表名称
        /// </summary>
        public string SrcTable
        {
            get
            {
                string str = this.GetValStringByKey(SFTableAttr.SrcTable);
                if (DataType.IsNullOrEmpty(str) == true)
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(SFTableAttr.SrcTable, value);
            }
        }
        /// <summary>
        /// 值/主键字段名
        /// </summary>
        public string ColumnValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnValue, value);
            }
        }
        /// <summary>
        /// 显示字段/显示字段名
        /// </summary>
        public string ColumnText
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnText);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnText, value);
            }
        }
        /// <summary>
        /// 父结点字段名
        /// </summary>
        public string ParentValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ParentValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ParentValue, value);
            }
        }
        /// <summary>
        /// 查询语句
        /// </summary>
        public string SelectStatement
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.SelectStatement);
            }
            set
            {
                this.SetValByKey(SFTableAttr.SelectStatement, value);
            }
        }
        /// <summary>
        /// 加入日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.RDT);
            }
            set
            {
                this.SetValByKey(SFTableAttr.RDT, value);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否是类
        /// </summary>
        public bool IsClass
        {
            get
            {
                if (this.No.Contains("."))
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否是树形实体?
        /// </summary>
        public bool IsTree
        {
            get
            {
                if (this.CodeStruct == BP.Sys.CodeStruct.NoName)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 数据源类型
        /// </summary>
        public string SrcType
        {
            get
            {
                if (this.No.ToUpper().Contains("BP.") == true)
                    return DictSrcType.BPClass;
                else
                {
                    string src = this.GetValStringByKey(SFTableAttr.DictSrcType);
                    if (src.Equals(BP.Sys.DictSrcType.BPClass))
                        return Sys.DictSrcType.CreateTable;
                    return src;
                }
            }
            set
            {
                this.SetValByKey(SFTableAttr.DictSrcType, value);
            }
        }
        /// <summary>
        /// 数据源类型名称
        /// </summary>
        public string SrcTypeText
        {
            get
            {
                switch (this.SrcType)
                {
                    case DictSrcType.TableOrView:
                        if (this.IsClass)
                            return "<img src='/WF/Img/Class.png' width='16px' broder='0' />实体类";
                        else
                            return "<img src='/WF/Img/Table.gif' width='16px' broder='0' />表/视图";
                    case DictSrcType.SQL:
                        return "<img src='/WF/Img/SQL.png' width='16px' broder='0' />SQL表达式";
                    case DictSrcType.WebServices:
                        return "<img src='/WF/Img/WebServices.gif' width='16px' broder='0' />WebServices";
                    case DictSrcType.WebApi:
                        return "WebApi接口";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 字典表类型
        /// <para>0：NoName类型</para>
        /// <para>1：NoNameTree类型</para>
        /// <para>2：NoName行政区划类型</para>
        /// </summary>
        public CodeStruct CodeStruct
        {
            get
            {
                return (CodeStruct)this.GetValIntByKey(SFTableAttr.CodeStruct);
            }
            set
            {
                this.SetValByKey(SFTableAttr.CodeStruct, (int)value);
            }
        }
        /// <summary>
        ///编号生成规则
        /// </summary>
        public NoGenerModel NoGenerModel
        {
            get
            {
                return (NoGenerModel)this.GetValIntByKey(SFTableAttr.NoGenerModel);
            }
            set
            {
                this.SetValByKey(SFTableAttr.NoGenerModel, (int)value);
            }
        }
        /// <summary>
        /// 编码类型
        /// </summary>
        public string CodeStructT
        {
            get
            {
                return this.GetValRefTextByKey(SFTableAttr.CodeStruct);
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public string FK_Val
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_Val);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_Val, value);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string TableDesc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.TableDesc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.TableDesc, value);
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.DefVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.DefVal, value);
            }
        }
        public EntitiesNoName HisEns
        {
            get
            {
                if (this.IsClass)
                {
                    EntitiesNoName ens = (EntitiesNoName)BP.En.ClassFactory.GetEns(this.No);
                    ens.RetrieveAll();
                    return ens;
                }

                BP.En.GENoNames ges = new GENoNames(this.No, this.Name);
                ges.RetrieveAll();
                return ges;
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                //   uac.OpenForSysAdmin();
                uac.Readonly(); //@hongyan.
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTable()
        {
        }
        public SFTable(string mypk)
        {
            this.No = mypk;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                switch (this.No)
                {
                    case "BP.Pub.NYs":
                        this.Name = "年月";
                        this.FK_Val = "FK_NY";
                        this.Insert();
                        break;
                    case "BP.Pub.YFs":
                        this.Name = "月";
                        this.FK_Val = "FK_YF";
                        this.Insert();
                        break;
                    case "BP.Pub.Days":
                        this.Name = "天";
                        this.FK_Val = "FK_Day";
                        this.Insert();
                        break;
                    case "BP.Pub.NDs":
                        this.Name = "年";
                        this.FK_Val = "FK_ND";
                        this.Insert();
                        break;
                    default:
                        throw new Exception(ex.Message);
                }
            }
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
                Map map = new Map("Sys_SFTable", "字典表");


                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLStringEnum(SFTableAttr.DictSrcType, "BPClass", "数据表类型", SFTableAttr.DictSrcType, false);
                //map.AddDDLSysEnum(SFTableAttr.DictSrcType, 0, "数据表类型", true, false, SFTableAttr.DictSrcType);

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "字典表类型", true, false, SFTableAttr.CodeStruct);
                map.AddTBString(SFTableAttr.RootVal, null, "根节点值", false, false, 0, 200, 20);

                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);
                map.AddDDLSysEnum(SFTableAttr.NoGenerModel, 1, "编号生成规则", true, true, SFTableAttr.NoGenerModel,
            "@0=自定义@1=流水号@2=标签的全拼@3=标签的简拼@4=按GUID生成");
                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(SFTableAttr.SrcTable, null, "数据源表", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnValue, null, "显示的值(编号列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "显示的文字(名称列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "父级值(父级列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.SelectStatement, null, "查询语句", true, false, 0, 1000, 600, true);

                map.AddTBString("RequestMethod", "Get", "RequestMethod", true, false, 0, 100, 600, true);
                map.AddTBString("FieldNo", "", "FieldNo", true, false, 0, 200, 600, true);
                map.AddTBString("FieldName", "", "FieldName", true, false, 0, 200, 600, true);
                map.AddTBString("FieldParentNo", "", "FieldParentNo", true, false, 0, 200, 600, true);
                map.AddTBString("JsonNode", "", "根节点", true, false, 0, 200, 600, true);

                //是否有参数
                map.AddTBInt("IsPara", 0, "IsPara", false, false);
                map.AddTBDateTime(SFTableAttr.RDT, null, "加入日期", false, false);
                map.AddTBString(SFTableAttr.OrgNo, null, "组织编号", false, false, 0, 100, 20);
                map.AddTBString(SFTableAttr.AtPara, null, "AtPara", false, false, 0, 50, 20);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

                RefMethod rm = new RefMethod();
                rm.Title = "查看数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "修改属性";
                rm.ClassMethodName = this.ToString() + ".DoAttr";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "新建字典";
                rm.ClassMethodName = this.ToString() + ".DoNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "创建Table向导";
                //rm.ClassMethodName = this.ToString() + ".DoGuide";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.IsForEns = false;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "数据源管理";
                //rm.ClassMethodName = this.ToString() + ".DoMangDBSrc";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.IsForEns = false;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 映射方法.
        public string DoAttr()
        {
            string projectName = HttpContextHelper.Request.Url.Segments[1];
            if (projectName.Equals("WF/"))
                projectName = "";
            else
                projectName = "/" + projectName;
            return SystemConfig.HostURLOfBS + projectName + "/WF/Comm/EnOnly.htm?EnName=BP.Sys.SFTable&No=" + this.No;
        }
        public string DoNew()
        {
            string projectName = HttpContextHelper.Request.Url.Segments[1];
            if (projectName.Equals("WF/"))
                projectName = "";
            else
                projectName = "/" + projectName;
            return SystemConfig.HostURLOfBS + projectName + "/WF/Admin/FoolFormDesigner/SFTable/Default.htm?DoType=New&FromApp=SL&s=0.3256071044807922";
        }
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoMangDBSrc()
        {
            return "../../Comm/Sys/SFDBSrcNewGuide.htm";
        }
        /// <summary>
        /// 创建表向导
        /// </summary>
        /// <returns></returns>
        public string DoGuide()
        {
            return "../../Admin/FoolFormDesigner/CreateSFGuide.htm";
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            if (this.IsClass)
            {

                return "../../Comm/Ens.htm?EnsName=" + this.No;
            }
            else
            {
                if (this.GetValIntByKey(SFTableAttr.CodeStruct) == 0)
                    return "../../Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + this.No;
                else
                    return "../../Admin/FoolFormDesigner/SFTableEditDataTree.htm?FK_SFTable=" + this.No;
            }
        }
        #endregion


        #region 重写方法.
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
        protected override bool beforeInsert()
        {

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                this.OrgNo = BP.Web.WebUser.OrgNo;
                this.No = this.OrgNo + "_" + this.No;
            }
            //利用这个时间串进行排序.
            this.RDT = DataType.CurrentDateTime;

            #region 如果是本地类. 
            if (this.SrcType == BP.Sys.DictSrcType.BPClass)
            {
                Entities ens = ClassFactory.GetEns(this.No);
                Entity en = ens.GetNewEntity;
                this.Name = en.EnDesc;

                //检查是否是树结构.
                if (en.IsTreeEntity == true)
                    this.CodeStruct = BP.Sys.CodeStruct.Tree;
                else
                    this.CodeStruct = BP.Sys.CodeStruct.NoName;
            }
            #endregion 如果是本地类.

            #region 本地类，物理表..
            if (this.SrcType == BP.Sys.DictSrcType.CreateTable)
            {
                if (DBAccess.IsExitsObject(this.No) == true)
                    return base.beforeInsert();

                string sql = "";
                if (this.CodeStruct == BP.Sys.CodeStruct.NoName || this.CodeStruct == BP.Sys.CodeStruct.GradeNoName)
                {
                    sql = "CREATE TABLE " + this.No + " (";
                    sql += "No varchar(30) NOT NULL,";
                    sql += "Name varchar(3900) NULL";
                    sql += ")";
                }

                if (this.CodeStruct == BP.Sys.CodeStruct.Tree)
                {
                    sql = "CREATE TABLE " + this.No + " (";
                    sql += "No varchar(30) NOT NULL,";
                    sql += "Name varchar(3900)  NULL,";
                    sql += "ParentNo varchar(3900)  NULL";
                    sql += ")";
                }

                this.RunSQL(sql);

                //初始化数据.
                this.InitDataTable();
            }
            #endregion 如果是本地类.

            return base.beforeInsert();
        }
        protected override void afterInsert()
        {
            try
            {
                if (this.SrcType == DictSrcType.TableOrView)
                {
                    //暂时这样处理
                    string sql = "CREATE VIEW " + this.No + " (";
                    sql += "[No],";
                    sql += "[Name]";
                    sql += (this.CodeStruct == BP.Sys.CodeStruct.Tree ? ",[ParentNo])" : ")");
                    sql += " AS ";
                    sql += "SELECT " + this.ColumnValue + " No," + this.ColumnText + " Name" + (this.CodeStruct == BP.Sys.CodeStruct.Tree ? ("," + this.ParentValue + " ParentNo") : "") + " FROM " + this.SrcTable + (string.IsNullOrWhiteSpace(this.SelectStatement) ? "" : (" WHERE " + this.SelectStatement));

                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                    {
                        sql = sql.Replace("[", "`").Replace("]", "`");
                    }
                    else
                    {
                        sql = sql.Replace("[", "").Replace("]", "");
                    }
                    this.RunSQL(sql);
                }
            }
            catch (Exception ex)
            {
                //创建视图失败时，删除此记录，并提示错误
                this.DirectDelete();
                throw ex;
            }
            base.afterInsert();
        }
        #endregion 重写方法.

        #region 执行方法.
        public string GenerDataOfJsonFromWebApi(string paras)
        {
            var isPara = this.GetValIntByKey("IsPara");
            //if (isPara == 0)
            //    return "err@无参字典，不能调用这个方法.";
            if (isPara == 1 && paras.Contains("=") == false)
                paras = "@Key=" + paras;
            if (isPara == 2 && paras.Contains("=") == false)
                return "err@多个参字典,正确的格式为:@para1=val1@para2=val2.";
            //把参数转化为 ht.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPKVal", this.No);

            //获得ht.
            Hashtable ht = SFTable.GenerHT(paras, ens);
            DataTable dt = this.GenerHisDataTable(ht);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 根据参数获得json.
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string GenerJsonByPara(string paras)
        {
            var isPara = this.GetValIntByKey("IsPara");
            //if (isPara == 0)
            //    return "err@无参字典，不能调用这个方法.";
            if (isPara == 1 && paras.Contains("=") == false)
                paras = "@Key=" + paras;
            if (isPara == 2 && paras.Contains("=") == false)
                return "err@多个参字典,正确的格式为:@para1=val1@para2=val2.";

            //把参数转化为 ht.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPKVal", this.No);

            //获得ht.
            Hashtable ht = SFTable.GenerHT(paras, ens);
            try
            {
                DataTable dt = this.GenerHisDataTable(ht);
                string json = BP.Tools.Json.ToJson(dt);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 获得原始的数据
        /// </summary>
        /// <returns></returns>
        public string TS_YuanShi_Data_WebApi()
        {
            return Data_WebApi(null, this.GetValStringByKey("RequestMethod"), this.FK_SFDBSrc, this.SelectStatement);
        }
        /// <summary>
        /// 获得原始的数据
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string TS_YuanShi_Data_WebApi_Para(string paras)
        {
            //把参数转化为ht.
            Hashtable ht = DataType.ParseParasToHT(paras);

            //获取参数.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPK", this.No);
            //遍历它.
            foreach (SFPara item in ens)
            {
                //内部参数.
                if (item.IsSys == 0)
                {
                    if (ht.ContainsKey(item.ParaKey) == false)
                        ht.Add(item.ParaKey, item.ExpVal);
                    continue;
                }

                //如果是外部参数.
                if (ht.ContainsKey(item.ParaKey) == true)
                    continue;

                string key = "";
                if (ht.ContainsKey("Key") == true)
                    key = ht["Key"].ToString();
                ht.Add(item.ParaKey, key);
            }
            return Data_WebApi(ht, this.GetValStringByKey("RequestMethod"), this.FK_SFDBSrc, this.SelectStatement);
        }
        /// <summary>
        /// 返回json.
        /// </summary>
        /// <returns></returns>
        public string GenerDataOfJson()
        {
            DataTable dt = this.GenerHisDataTable();
            string json = BP.Tools.Json.ToJson(dt);
            return json;
        }
        /// <summary>
        /// 初始化数据.
        /// </summary>
        public void InitDataTable()
        {
            DataTable dt = this.GenerHisDataTable();

            string sql = "";
            if (dt.Rows.Count == 0)
            {
                /*初始化数据.*/
                if (this.CodeStruct == BP.Sys.CodeStruct.Tree)
                {
                    sql = "INSERT INTO " + this.SrcTable + " (No,Name,ParentNo) VALUES('1','" + this.Name + "','0') ";
                    this.RunSQL(sql);

                    for (int i = 1; i < 4; i++)
                    {
                        string no = i.ToString();
                        no = no.PadLeft(3, '0');

                        sql = "INSERT INTO " + this.SrcTable + " (No,Name,ParentNo) VALUES('" + no + "','Item" + no + "','1') ";
                        this.RunSQL(sql);
                    }
                }

                if (this.CodeStruct == BP.Sys.CodeStruct.NoName)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        string no = i.ToString();
                        no = no.PadLeft(3, '0');
                        sql = "INSERT INTO " + this.SrcTable + " (No,Name) VALUES('" + no + "','Item" + no + "') ";
                        this.RunSQL(sql);
                    }
                }
            }
        }
        #endregion 执行方法.

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="paras">@Key=xxx@xxx=xxx</param>
        /// <returns></returns>
        public string Vue3_GenerJsonByParas(string paras)
        {
            //获取参数,.
            SFParas ens = new SFParas();
            ens.Retrieve("RefPK", this.No);

            //通过公共的方法生成参数.
            Hashtable ht = SFTable.GenerHT(paras, ens);

            DataTable dt = this.GenerHisDataTable(ht);
            return BP.Tools.Json.ToJson(dt);
        }

        public static Hashtable GenerHT(string paras, SFParas ens)
        {
            //把参数转化为ht.
            Hashtable ht = DataType.ParseParasToHT(paras);

            //遍历它.
            foreach (SFPara item in ens)
            {
                //内部参数.
                if (item.IsSys == 0)
                {
                    if (ht.ContainsKey(item.ParaKey) == false)
                        ht.Add(item.ParaKey, item.ExpVal);
                    continue;
                }

                //如果是外部参数.
                if (ht.ContainsKey(item.ParaKey) == true)
                    continue;

                string key = "";
                if (ht.ContainsKey("Key") == true)
                    key = ht["Key"].ToString();
                ht.Add(item.ParaKey, key);
            }
            return ht;
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTables : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTables()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTable();
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
        public System.Collections.Generic.IList<SFTable> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTable>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTable> Tolist()
        {
            System.Collections.Generic.List<SFTable> list = new System.Collections.Generic.List<SFTable>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTable)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
