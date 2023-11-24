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
    public class SFProcedure : EntityNoName
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

            #region WebApi接口
            if (src.DBSrcType.Equals("WebApi") == true)
            {
                //执行POST
                string post = this.GetValStringByKey("RequestMethod");
                //  return BP.Tools.PubGlo.HttpPostConnect_Data(this.FK_SFDBSrc, this.SelectStatement, ht, this.GetValStringByKey("RequestMethod"));
                string postData = BP.Tools.PubGlo.HttpPostConnect_Data(this.FK_SFDBSrc, this.SelectStatement, ht, post);

                string jsonNode = this.GetValStringByKey("JsonNode"); //json的节点.
                JArray jArr = new JArray();
                try
                {
                    JObject json = JObject.Parse(postData);
                    if (!DataType.IsNullOrEmpty(jsonNode))
                        jArr = (JArray)(json.GetValue(jsonNode));
                    else
                        jArr = JArray.Parse(postData);
                }
                catch (Exception ex)
                {
                    throw new Exception("err@转化JSON出现错误Data=【" + postData + "】" + ex.Message);
                }
                return BP.Tools.Json.ToDataTable(jArr.ToString());
            }
            #endregion WebApi接口

            #region SQL接口
            string runObj = this.SelectStatement;
            if (DataType.IsNullOrEmpty(runObj))
                throw new Exception("@外键类型SQL配置错误," + this.No + " " + this.Name + " 是一个(SQL)类型(" + this.GetValStrByKey("SrcType") + ")，但是没有配置sql.");

            if (runObj == null)
                runObj = string.Empty;
            runObj = runObj.Replace("~~", "\"");
            runObj = runObj.Replace("~", "'");
            runObj = runObj.Replace("/#", "+"); //为什么？
            runObj = runObj.Replace("/$", "-"); //为什么？
            if (runObj.Contains("@WebUser.No"))
                runObj = runObj.Replace("@WebUser.No", BP.Web.WebUser.No);

            if (runObj.Contains("@WebUser.Name"))
                runObj = runObj.Replace("@WebUser.Name", BP.Web.WebUser.Name);

            if (runObj.Contains("@WebUser.FK_DeptName"))
                runObj = runObj.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.DeptName);

            if (runObj.Contains("@WebUser.FK_Dept"))
                runObj = runObj.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

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
            if (runObj.Contains("@") && BP.Difference.SystemConfig.isBSsystem == true)
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
                throw new Exception("@外键类型SQL错误," + runObj + "部分过程条件没有被替换.");
            DataTable dt = null;
            try
            {
                dt = src.RunSQLReturnTable(runObj);
            }
            catch (Exception ex)
            {
                throw new Exception("err@获得SFProcedure(" + this.No + "," + this.Name + ")出现错误:SQL[" + runObj + "],数据库异常信息:" + ex.Message);
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
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFProcedure()
        {
        }
        public SFProcedure(string no)
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
                Map map = new Map("Sys_SFProcedure", "过程");

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString("ConnString", null, "ConnString", true, false, 0, 200, 150, true);
                map.AddDDLSysEnum("IsPara", 0, "参数个数", true, true, "IsPara", "@0=无参数@1=有参数");

                map.AddTBString("ExpDoc", null, "表达式内容", true, false, 0, 1000, 600, true);
                map.AddTBString("ExpNote", null, "表达式说明", true, false, 0, 1000, 600, true);
                map.AddDDLStringEnum("RequestMethod", "Get", "请求模式", "@Get=Get@POST=POST", true);
                map.AddTBStringDoc("ColumnsRemark", "", "格式备注", true, false, true);

                // 创建信息
                map.AddGroupAttr("创建信息");
                map.AddTBString("Remark", null, "备注", true, false, 0, 100, 20, true);
                map.AddTBDateTime("RDT", null, "创建日期", true, true);
                map.AddTBString("OrgNo", null, "组织编号", true, true, 0, 100, 20);
                map.AddTBAtParas();
                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>返回执行结果</returns>
        public string ExecSQL(Hashtable ht, SFDBSrc src)
        {
            string sql = this.SelectStatement;
            try
            {
                sql = Glo.DealExp(sql, ht); //处理sql.

                int num = src.RunSQL(sql);
                if (num == 0)
                    return "执行失败:" + this.GetValStrByKey("MsgOfErr") + "，返回结果为0";
                return "执行成功:" + this.GetValStrByKey("MsgOfOK") + "，返回结果为" + num;
            }
            catch (Exception ex)
            {
                return "执行错误:" + this.GetValStrByKey("MsgOfErr") + "，请检查配置的SQL是否正确:[" + sql + "]";
            }
        }
        public string Exec(string paras)
        {
            SFDBSrc src = new SFDBSrc();
            src.No = this.FK_SFDBSrc;
            src.RetrieveFromDBSources();

            Hashtable ht = DataType.ParseParasToHT(paras);
            if (src.DBSrcType.Equals("WebApi") == true)
                return ExecWebApi(ht, src);
            return ExecSQL(ht, src);
        }

        public string ExecWebApi(Hashtable ht, SFDBSrc src)
        {
            try
            {
                string postData = SFTable.Data_WebApi(ht, this.GetValStringByKey("RequestMethod"), this.FK_SFDBSrc, this.SelectStatement);
                return "" + this.GetValByKey("MsgOfOK") + " -:" + postData;
            }
            catch (Exception ex)
            {
                return "err@[" + this.GetValByKey("MsgOfErr") + "]:失败信息" + ex.Message;
            }

            //// 需要替换的参数
            //string fieldNo = this.GetValStringByKey("FieldNo");
            //if (DataType.IsNullOrEmpty(fieldNo))
            //    fieldNo = "No";

            //string fieldName = this.GetValStringByKey("FieldName");
            //if (DataType.IsNullOrEmpty(fieldName))
            //    fieldName = "Name";

            //string fieldParentNo = this.GetValStringByKey("FieldParentNo");
            //if (DataType.IsNullOrEmpty(fieldParentNo))
            //    fieldParentNo = "ParentNo";

            //// 根节点
            //string jsonNode = this.GetValStringByKey("JsonNode");

            //JToken jToken = JToken.Parse(postData);
            //// 如果是JSON数组
            //if (jToken.Type == JTokenType.Array)
            //{
            //    // 新的对象，用来删除原对象无用字段
            //    JArray newJsonArr = new JArray();
            //    JArray arr = (JArray)jToken;
            //    JObject firstItem = (JObject)arr[0];
            //    checkJsonField(firstItem, fieldNo, fieldName, fieldParentNo);
            //    foreach (JObject obj in arr)
            //    {
            //        newJsonArr.Add(getJSONByTargetName(obj, fieldNo, fieldName, fieldParentNo));
            //    }
            //    return BP.Tools.Json.ConvertToDataTable(newJsonArr);
            //}
            //// 如果是JSON对象
            //if (jToken.Type == JTokenType.Object)
            //{
            //    JObject jsonItem = (JObject)jToken;
            //    // 判断是不是有根节点
            //    // 如果有
            //    if (!DataType.IsNullOrEmpty(jsonNode) && jsonItem.ContainsKey(jsonNode))
            //    {
            //        // 新的对象，用来删除原对象无用字段
            //        JArray newJsonArr = new JArray();
            //        JToken jToken1 = jsonItem[jsonNode];
            //        // 判断当前是不是数组或者
            //        if (jToken1.Type == JTokenType.Array)
            //        {
            //            JObject firstItem = (JObject)jToken1[0];
            //            checkJsonField(firstItem, fieldNo, fieldName, fieldParentNo);
            //            foreach (JObject obj in jToken1)
            //            {
            //                newJsonArr.Add(getJSONByTargetName(obj, fieldNo, fieldName, fieldParentNo));
            //            }
            //            return BP.Tools.Json.ToDataTable(newJsonArr.ToString());
            //        }
            //        if (jToken1.Type == JTokenType.Object)
            //        {
            //            JObject targetObj = (JObject)jToken;
            //            checkJsonField(targetObj, fieldNo, fieldName, fieldParentNo);
            //            JObject itemOfRootNode = getJSONByTargetName(targetObj, fieldNo, fieldName, fieldParentNo);
            //            return BP.Tools.Json.ToDataTable(itemOfRootNode.ToString());
            //        }
            //        throw new Exception("指定的RootNode下不是JSON数组 或 JSON对象");

            //    }
            //    // 如果没有配置rootNode，检查他本身有没有所需属性
            //    JObject currObj = (JObject)jToken;
            //    checkJsonField(currObj, fieldNo, fieldName, fieldParentNo);
            //    JObject itemOfResponse = getJSONByTargetName(currObj, fieldNo, fieldName, fieldParentNo);
            //    return BP.Tools.Json.ToDataTable(itemOfResponse.ToString());
            //}
            //throw new Exception("Web_API 没有正确返回JSON字符串");
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
    public class SFProcedures : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFProcedures()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFProcedure();
            }
        }
        /// <summary>
        ///  重写过程全部的方法
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
        public System.Collections.Generic.IList<SFProcedure> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFProcedure>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFProcedure> Tolist()
        {
            System.Collections.Generic.List<SFProcedure> list = new System.Collections.Generic.List<SFProcedure>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFProcedure)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
