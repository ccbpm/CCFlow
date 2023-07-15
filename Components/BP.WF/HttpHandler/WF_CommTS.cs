using System;
using System.Collections.Generic;
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
using BP.Difference;
using System.IO;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CommTS : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 参数
        public string Paras
        {
            get
            {
                return this.GetRequestVal("Paras");
            }
        }
        public string OrderBy
        {
            get
            {
                return this.GetRequestVal("OrderBy");
            }
        }
        public string MyPK
        {
            get
            {
                return this.GetRequestVal("MyPK");
            }
        }
        public string No
        {
            get
            {
                return this.GetRequestVal("No");
            }
        }
        public int OID
        {
            get
            {
                string str = this.GetRequestVal("OID");
                if (DataType.IsNullOrEmpty(str))
                    str = this.GetRequestVal("WorkID");

                if (DataType.IsNullOrEmpty(str))
                    str = this.GetRequestVal("PKVal");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = "0";
                return int.Parse(str);
            }
        }
        public string ClassID
        {
            get
            {
                return this.GetRequestVal("ClassID");
            }
        }
        /// <summary>
        /// 关联服务器端的实体类.
        /// </summary>
        public string RefEnName
        {
            get
            {
                string str = this.GetRequestVal("RefEnName");
                if (DataType.IsNullOrEmpty(str) == false)
                    str = str.Replace("TS.", "BP.");
                return str;
            }
        }
        public string KVs
        {
            get
            {
                return this.GetRequestVal("KVs");
            }
        }
        public string Map
        {
            get
            {
                return this.GetRequestVal("Map");
            }
        }
        public string PK
        {
            get
            {
                return this.GetRequestVal("PK");
            }
        }
        public string PKVal
        {
            get
            {
                return this.GetRequestVal("PKVal");
            }
        }
        public int PKValInt
        {
            get
            {
                string str = this.GetRequestVal("PKVal");


                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("WorkID");
                else
                    return int.Parse(str);

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("NodeID");
                else
                    return int.Parse(str);

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("OID");
                else
                    return int.Parse(str);

                if (DataType.IsNullOrEmpty(str) == true)
                    str = "0";

                return int.Parse(str);
            }
        }
        #endregion 参数

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CommTS()
        {
        }

        #region 页面类.
        /// <summary>
        /// 从表移动
        /// </summary>
        /// <returns></returns>
        public string DtlSearch_UpdatIdx()
        {
            Map map = BP.EnTS.Glo.GenerMap(this.ClassID);

            string pk = "No";
            if (map.Attrs.Contains("No") == true)
                pk = "No";
            else if (map.Attrs.Contains("OID") == true)
                pk = "OID";
            else if (map.Attrs.Contains("MyPK") == true)
                pk = "MyPK";
            else if (map.Attrs.Contains("NodeID") == true)
                pk = "NodeID";
            else if (map.Attrs.Contains("WorkID") == true)
                pk = "WorkID";

            string[] pks = this.GetRequestVal("PKs").Split(',');
            int idx = 0;
            foreach (string str in pks)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;
                idx++;
                string sql = "UPDATE " + map.PhysicsTable + " SET Idx=" + idx + " WHERE " + pk + "='" + str + "'";
                DBAccess.RunSQL(sql);
            }

            #region 特殊业务处理.
            if (this.ClassID.Equals("TS.WF.Cond") == true)
            {
                //判断设置的顺序是否合理？
                Cond cond = new Cond();
                string pkval = pks[0];
                cond.MyPK = pkval;
                cond.Retrieve();

                return WF_Admin_Cond2020.List_DoCheckExt(cond.CondTypeInt, cond.FK_Node, cond.ToNodeID);
            }
            #endregion 特殊业务处理.


            return "移动成功.";
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        /// <returns></returns>
        public string TreeEns_UpdatIdx()
        {
            string[] pks = this.GetRequestVal("PKs").Split(',');
            string ptable = this.GetRequestVal("PTable");
            string pk = this.GetRequestVal("PK");
            int idx = 0;
            foreach (string str in pks)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;
                idx++;
                string sql = "UPDATE " + ptable + " SET Idx='" + idx + "' WHERE " + pk + "='" + str + "'";
                DBAccess.RunSQL(sql);
            }
            return "执行成功.";
        }
        #endregion 页面类.

        /// <summary>
        /// 加入map到缓存.
        /// </summary>
        /// <returns></returns>
        public string Entity_SetMap()
        {
            BP.EnTS.Glo.SetMap(this.ClassID, this.Map);
            return "1";
        }
        /// <summary>
        /// 检查是否存在Map
        /// </summary>
        /// <returns></returns>
        public string Entity_IsExitMap()
        {
            //缓存map.
            if (BP.EnTS.Glo.IsExitMap(this.ClassID) == true)
                return "1";

            return "0";
        }
        public string Entity_IsExits()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);
                en.No = this.PKVal;
                if (en.IsExits == true)
                    return "1";
                return "0";
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);
                en.MyPK = this.PKVal;
                if (en.IsExits == true)
                    return "1";
                return "0";
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID);
                en.OID = int.Parse(this.PKVal);
                if (en.IsExits == true)
                    return "1";
                return "0";
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);
                en.WorkID = int.Parse(this.PKVal);
                if (en.IsExits == true)
                    return "1";
                return "0";
            }
            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);
                en.NodeID = int.Parse(this.PKVal);
                if (en.IsExits == true)
                    return "1";
                return "0";
            }

            throw new Exception("err@没有判断的entity类型.");
        }
        /// <summary>
        /// 执行insert方法.
        /// </summary>
        /// <returns></returns>
        public string Entity_Insert()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.Insert();
                return en.ToJson();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.Insert();
                return en.ToJson();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.Insert();
                return en.ToJson();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.Insert();
                return en.ToJson();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.Insert();
                return en.ToJson();
            }

            throw new Exception("err@没有判断的entity类型.");
        }
        /// <summary>
        /// 根据
        /// </summary>
        /// <returns></returns>
        public string Entity_GenerSQLAttrDB()
        {
            // PK  PKVal ClassID Row
            string attrKey = this.GetRequestVal("AttrKey"); //  "SELECT * FROM WHERE XX=@SortNo ";
            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }

                Attr attr = en.EnMap.GetAttrByKey(attrKey);
                string sql = attr.UIBindKey;
                sql = BP.WF.Glo.DealExp(sql, en);
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return BP.Tools.Json.ToJson(dt);
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }

                Attr attr = en.EnMap.GetAttrByKey(attrKey);
                string sql = attr.UIBindKey;
                sql = BP.WF.Glo.DealExp(sql, en);
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return BP.Tools.Json.ToJson(dt);
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }

                Attr attr = en.EnMap.GetAttrByKey(attrKey);
                string sql = attr.UIBindKey;
                sql = BP.WF.Glo.DealExp(sql, en);
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return BP.Tools.Json.ToJson(dt);
            }

            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }

                Attr attr = en.EnMap.GetAttrByKey(attrKey);
                string sql = attr.UIBindKey;
                sql = BP.WF.Glo.DealExp(sql, en);
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return BP.Tools.Json.ToJson(dt);
            }

            return "";
        }
        public string Entity_Save()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                var num = en.Save();

                return num.ToString();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                var num = en.Save();
                return num.ToString();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                var num = en.Save();
                return num.ToString();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                var num = en.Save();
                return num.ToString();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                var num = en.Save();
                return num.ToString();
            }

            throw new Exception("err@没有判断的entity类型.");

        }
        public bool checkPower(string classID, string pkval)
        {
            if (classID.Contains("BP.WF.Admin") == true && BP.Web.WebUser.IsAdmin == false)
                throw new Exception("非法用户.");

            return true;
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <returns></returns>
        public string Entity_Update()
        {
            // checkPower(this.ClassID, this.PKVal);
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID, this.PKVal);

                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.No = this.PKVal;

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    foreach (var item in json)
                    {
                        enServ.SetValByKey(item.Key, item.Value);
                    }
                    enServ.Update();
                }

                return en.Update().ToString();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID, this.PKVal);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                {
                    en.SetValByKey(item.Key, item.Value);
                }
                en.MyPK = this.PKVal;

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    foreach (var item in json)
                    {
                        enServ.SetValByKey(item.Key, item.Value);
                    }
                    enServ.Update();

                    //把变更后的值给,TS实体.
                    Row row = enServ.Row;
                    foreach (var key in row.Keys)
                        en.Row[key] = row[key];
                }
                return en.Update().ToString();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID, this.PKValInt);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                    en.SetValByKey(item.Key, item.Value);

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKValInt;
                    enServ.RetrieveFromDBSources();
                    foreach (var item in json)
                    {
                        enServ.SetValByKey(item.Key, item.Value);
                    }
                    enServ.Update();
                }

                en.OID = this.PKValInt;
                return en.Update().ToString();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID, this.PKValInt);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                    en.SetValByKey(item.Key, item.Value);

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    foreach (var item in json)
                    {
                        enServ.SetValByKey(item.Key, item.Value);
                    }
                    enServ.Update();
                }

                en.NodeID = this.PKValInt;
                return en.Update().ToString();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID, this.PKValInt);
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(this.GetRequestVal("Row"));
                foreach (var item in json)
                    en.SetValByKey(item.Key, item.Value);

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKValInt;
                    enServ.RetrieveFromDBSources();
                    foreach (var item in json)
                    {
                        enServ.SetValByKey(item.Key, item.Value);
                    }
                    enServ.Update();
                }

                en.WorkID = this.PKValInt;
                return en.Update().ToString();
            }

            throw new Exception("err@没有判断的entity类型. Entity_Update ");
        }
        /// <summary>
        /// 查询 
        /// </summary>
        /// <returns></returns>
        public string Entity_Retrieve()
        {

            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID, this.PKVal);
                return en.ToJson();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID, this.PKVal);
                return en.ToJson();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID, PKValInt);
                return en.ToJson();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID, this.PKValInt);
                return en.ToJson();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID, this.PKValInt);
                return en.ToJson();
            }

            throw new Exception("err@没有判断的entity类型. Entity_Retrieve ");
        }
        /// <summary>
        /// 从数据库里查询.
        /// </summary>
        /// <returns></returns>
        public string Entity_RetrieveFromDBSources()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);
                en.No = this.PKVal;
                int val = en.RetrieveFromDBSources();
                if (val == 0) return "0";
                return en.ToJson();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);
                en.MyPK = this.PKVal;
                int val = en.RetrieveFromDBSources();
                if (val == 0) return "0";
                return en.ToJson();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID);
                en.OID = this.PKValInt;
                int val = en.RetrieveFromDBSources();
                if (val == 0)
                    return "0";
                return en.ToJson();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);
                en.WorkID = this.PKValInt;
                int val = en.RetrieveFromDBSources();
                if (val == 0) return "0";
                return en.ToJson();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);
                en.NodeID = this.PKValInt;
                int val = en.RetrieveFromDBSources();
                if (val == 0) return "0";
                return en.ToJson();
            }

            throw new Exception("err@没有判断的entity类型. Entity_Retrieve ");
        }
        public string Entities_RetrieveAllFromDBSource()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntitiesNoName ens = new TSEntitiesNoName(this.ClassID);
                ens.RetrieveAllFromDBSource();
                return ens.ToJson();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntitiesOID ens = new TSEntitiesOID(this.ClassID);
                ens.RetrieveAllFromDBSource();
                return ens.ToJson();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntitiesMyPK ens = new TSEntitiesMyPK(this.ClassID);
                ens.RetrieveAllFromDBSource();
                return ens.ToJson();
            }
            if (this.PK.Equals("WorkID") == true)
            {
                TSEntitiesWorkID ens = new TSEntitiesWorkID(this.ClassID);
                ens.RetrieveAllFromDBSource();
                return ens.ToJson();
            }
            if (this.PK.Equals("NodeID") == true)
            {
                TSEntitiesNodeID ens = new TSEntitiesNodeID(this.ClassID);
                ens.RetrieveAllFromDBSource();
                return ens.ToJson();
            }
            throw new Exception("err@没有判断的entity类型. Entities_RetrieveAllFromDBSource ");
        }
        public string Entities_RetrieveAll()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntitiesNoName ens = new TSEntitiesNoName(this.ClassID);
                ens.RetrieveAll(this.OrderBy);
                return ens.ToJson();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntitiesOID ens = new TSEntitiesOID(this.ClassID);
                ens.RetrieveAll(this.OrderBy);
                return ens.ToJson();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntitiesMyPK ens = new TSEntitiesMyPK(this.ClassID);
                ens.RetrieveAll(this.OrderBy);
                return ens.ToJson();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntitiesWorkID ens = new TSEntitiesWorkID(this.ClassID);
                ens.RetrieveAll(this.OrderBy);
                return ens.ToJson();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntitiesNodeID ens = new TSEntitiesNodeID(this.ClassID);
                ens.RetrieveAll(this.OrderBy);
                return ens.ToJson();
            }

            throw new Exception("err@没有判断的entity类型. Entities_RetrieveAll ");
        }
        public string Entities_Retrieve()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntitiesNoName ens = new TSEntitiesNoName(this.ClassID);
                BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
                return hand.Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntitiesMyPK ens = new TSEntitiesMyPK(this.ClassID);
                BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
                return hand.Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntitiesNodeID ens = new TSEntitiesNodeID(this.ClassID);
                BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
                return hand.Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }


            if (this.PK.Equals("WorkID") == true)
            {
                TSEntitiesWorkID ens = new TSEntitiesWorkID(this.ClassID);
                BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
                return hand.Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntitiesOID ens = new TSEntitiesOID(this.ClassID);
                BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
                return hand.Entities_Init_Ext(ens, ens.GetNewEntity, this.Paras);
            }


            throw new Exception("err@没有判断的 entity 类型. Entities_Retrieve ");
        }

        public string Entities_Delete()
        {
            TSEntitiesNoName ens = new TSEntitiesNoName(this.ClassID);
            BP.WF.HttpHandler.WF_Comm hand = new WF_Comm();
            return hand.Entities_Delete_Ext(ens);
        }

        public string Entities_RetrieveLikeKey()
        {
            string searchKey = this.GetRequestVal("SearchKey");
            string attrsScop = this.GetRequestVal("AttrsScop");
            string condAttr = this.GetRequestVal("CondAttr");
            string condVal = this.GetRequestVal("CondVal");
            string orderBy = this.GetRequestVal("OrderBy");

            if (this.PK.Equals("No") == true)
            {
                TSEntitiesNoName ens = new TSEntitiesNoName(this.ClassID);
                QueryObject qo = new QueryObject(ens);

                string[] strs = attrsScop.Split(',');

                qo.addLeftBracket();
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    qo.AddWhere(str, " LIKE ", "'%" + searchKey + "%'");
                    qo.addOr();
                }
                qo.AddWhere(" 1=2 ");
                qo.addRightBracket();

                if (DataType.IsNullOrEmpty(condAttr) == false)
                {
                    qo.addAnd();
                    qo.AddWhere(condAttr, "=", condVal);
                }
                if (DataType.IsNullOrEmpty(orderBy) == false)
                    qo.addOrderBy(orderBy);
                qo.DoQuery();
                return ens.ToJson();
            }

            return "err@没有判断的类型Entities_RetrieveLikeKey:" + this.PKVal;
        }
        
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string Entity_Delete()
        {
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID);
                en.No = this.PKVal;
                en.RetrieveFromDBSources();

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    var i = enServ.Delete();

                    en.Delete(); //执行本实体的删除.
                    return i.ToString();
                }

                return en.Delete().ToString();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID);
                en.MyPK = this.PKVal;
                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    var i = enServ.Delete();

                    en.Delete(); //执行本实体的删除.
                    return i.ToString();
                }

                return en.Delete().ToString();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID);
                en.OID = this.PKValInt;
                en.RetrieveFromDBSources();

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    var i = enServ.Delete();

                    en.Delete(); //执行本实体的删除. 
                    return i.ToString();
                }

                return en.Delete().ToString();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID);
                en.WorkID = this.PKValInt;

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    var i = enServ.Delete();

                    en.Delete(); //执行本实体的删除.
                    return i.ToString();
                }


                return en.Delete().ToString();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID);
                en.NodeID = this.PKValInt;

                //判断是否有对应的后端实体类，如果有则要执行更新.
                if (DataType.IsNullOrEmpty(this.RefEnName) == false)
                {
                    Entity enServ = ClassFactory.GetEn(this.RefEnName);
                    if (enServ == null)
                        throw new Exception("err@TS实体类[" + this.ClassID + "]关联的[" + this.RefEnName + "]拼写错误,");

                    enServ.PKVal = this.PKVal;
                    enServ.RetrieveFromDBSources();
                    var i = enServ.Delete();

                    en.Delete(); //执行本实体的删除.
                    return i.ToString();
                }

                return en.Delete().ToString();
            }

            throw new Exception("err@没有判断的entity类型. Entity_Delete ");
        }

        public string Entity_Upload()
        {
            var files = HttpContextHelper.RequestFiles();
            if (files.Count == 0)
                return "err@请选择要上传的文件。";
            //获取保存文件信息的实体
            string saveTo = this.GetRequestVal("SaveTo");
            string realSaveTo = "";
            if (DataType.IsNullOrEmpty(saveTo) == true)
            {
                realSaveTo = SystemConfig.PathOfDataUser + "UploadFile/";
                saveTo = "/DataUser/UploadFile/";
            }
            else
            {
                if (saveTo.StartsWith("/DataUser"))
                    realSaveTo = SystemConfig.PathOfWebApp + saveTo;
            }
            //获取文件的名称
            string fileName = files[0].FileName;
            if (fileName.IndexOf("/") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            //文件后缀
            string ext = System.IO.Path.GetExtension(files[0].FileName);

            //文件大小
            float size = HttpContextHelper.RequestFileLength(files[0]) / 1024;

            FileInfo info = new FileInfo(saveTo);
            HttpContextHelper.UploadFile(files[0], realSaveTo + fileName+ext);
            AtPara para = new AtPara();
            para.SetVal("FileName", fileName);
            para.SetVal("FileExt", ext);
            para.SetVal("FileSize", size.ToString());
            para.SetVal("FilePath", saveTo + fileName + ext);
            string saveInfo = para.GenerAtParaStrs();
            if (this.PK.Equals("No") == true)
            {
                TSEntityNoName en = new TSEntityNoName(this.ClassID, this.PKVal);
                en.No = this.PKVal;
                en.SetValByKey(this.KeyOfEn, saveInfo);
                return en.DirectUpdate().ToString();
            }

            if (this.PK.Equals("MyPK") == true)
            {
                TSEntityMyPK en = new TSEntityMyPK(this.ClassID, this.PKVal);
                en.MyPK = this.PKVal;
                en.SetValByKey(this.KeyOfEn, saveInfo);
                return en.DirectUpdate().ToString();
            }

            if (this.PK.Equals("OID") == true)
            {
                TSEntityOID en = new TSEntityOID(this.ClassID, this.PKValInt);
                en.OID = this.PKValInt;
                en.SetValByKey(this.KeyOfEn, saveInfo);
                return en.DirectUpdate().ToString();
            }

            if (this.PK.Equals("NodeID") == true)
            {
                TSEntityNodeID en = new TSEntityNodeID(this.ClassID, this.PKValInt);
                en.NodeID = this.PKValInt;
                en.SetValByKey(this.KeyOfEn, saveInfo);
                return en.DirectUpdate().ToString();
            }

            if (this.PK.Equals("WorkID") == true)
            {
                TSEntityWorkID en = new TSEntityWorkID(this.ClassID, this.PKValInt);
                en.WorkID = this.PKValInt;
                en.SetValByKey(this.KeyOfEn, saveInfo);
                return en.DirectUpdate().ToString();
            }
            return "上传成功";
        }
    }
}
