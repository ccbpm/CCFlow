using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.BPMN;
using BP.Sys;
using BP.En;
using BP.WF.Template;
using System.Collections.Generic;
using BP.WF;
using LitJson;
using CCFlow.ViewModels;
using Newtonsoft.Json.Utilities;

namespace CCFlow.WF.Admin.CCFormDesigner
{
    /// <summary>
    /// SFTableHandler 的摘要说明
    /// </summary>
    public class SFTableHandler : IHttpHandler
    {
        #region 属性.
        public string FK_SFTable
        {
            get
            {
                return context.Request["FK_SFTable"].ToString();
            }            
        }
        public string DoType
        {
            get
            {
                return context.Request["DoType"].ToString();
            }
        }
        #endregion 


        public HttpContext context=null;
        public void ProcessRequest(HttpContext _context)
        {
            context = _context;

            string json = "";

            switch (this.DoType)
            {
                case "CreateTableDataInit":
                    WriteInfo( CreateTableDataInit()); //输出数据.
                    break;
                case "CreateTableDataSave":
                    {
                        using (StreamReader reader = new System.IO.StreamReader(context.Request.InputStream))
                        {
                            json = reader.ReadToEnd();
                        }

                        WriteInfo( CreateTableDataSave(json)); //输出保存结果..
                        break;
                    }
                case "CreateTreeDataInit":
                    {
                        WriteInfo(CreateTreeDataInit()); //输出数据.
                        break;
                    }
                case "CreateTreeDataSave": 
                    {
                        using (StreamReader reader = new System.IO.StreamReader(context.Request.InputStream))
                        {
                            json = reader.ReadToEnd();
                        }
                        WriteInfo(CreateTreeDataSave(json));
                        break;
                    }
                default:
                    break;
            }
        }

        public string CreateTreeDataInit()
        {
            if (String.IsNullOrEmpty(this.FK_SFTable))
                return "";

            verifyTable();

            SFTable sf = new SFTable(this.FK_SFTable);
            
            string sql = "SELECT * FROM " + sf.No;
            DataTable dt = sf.RunSQLReturnTable(sql);

            string json = "";
            List<CodeItem> items = new List<CodeItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i] != null)
                    {
                        items.Add(new CodeItem()
                        {
                            ID = dt.Rows[i]["No"].ToString(),
                            Value = dt.Rows[i]["Name"].ToString(),
                            Parent = dt.Columns.Contains("ParentNo") ? dt.Rows[i]["ParentNo"].ToString() : ""
                        });
                    }
                }

                //return BP.Tools.Json.ToJson(models.ToArray());

                CodeItem[] treeItems = items.ToArray();

                treeItems = this.buildTreeItems(treeItems);

                json = Newtonsoft.Json.JsonConvert.SerializeObject(treeItems);

                //return json;
            }
            else
            {
                dt = sf.GenerData();
                //return BP.Tools.Json.ToJson(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i] != null)
                    {
                        items.Add(new CodeItem()
                        {
                            ID = dt.Rows[i]["No"].ToString(),
                            Value = dt.Rows[i]["Name"].ToString(),
                            Parent = dt.Columns.Contains("ParentNo") ? dt.Rows[i]["ParentNo"].ToString() : ""
                        });
                    }
                }

                //return BP.Tools.Json.ToJson(models.ToArray());
                CodeItem[] treeItems = items.ToArray();

                treeItems = this.buildTreeItems(treeItems);

                json = Newtonsoft.Json.JsonConvert.SerializeObject(treeItems);
            }

            return json;

            //string json = "";
            //List<CodeItem> items = new List<CodeItem>();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    if (dt.Rows[i] != null)
            //    {
            //        items.Add(new CodeItem()
            //        {
            //            ID = dt.Rows[i]["No"].ToString(),
            //            Value = dt.Rows[i]["Name"].ToString(),
            //            Parent = dt.Columns.Contains("ParentNo") ? dt.Rows[i]["ParentNo"].ToString() : ""
            //        });
            //    }
            //}
            //CodeItem[] treeItems = this.buildTreeItems(items.ToArray());
            //json = Newtonsoft.Json.JsonConvert.SerializeObject(treeItems);
            //return json;
        }

        private void verifyTable() 
        {
            string connectionString = ConfigurationManager.AppSettings.Get("AppCenterDSN");
            string dbType = ConfigurationManager.AppSettings.Get("AppCenterDBType");

            if (dbType.ToLower() == "mssql")
            {
                string sqlCmdTxtCreateTB = String.Format("CREATE TABLE {0} (No nvarchar(30) NOT NULL, Name nvarchar(60) NULL, ParentNo nvarchar(30) NULL CONSTRAINT {1}pk PRIMARY KEY CLUSTERED (No ASC))", this.FK_SFTable, this.FK_SFTable);

                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    string sqlCmdTxtGetTB = String.Format("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = '{0}' and TABLE_NAME = '{1}'", sqlConn.Database, this.FK_SFTable);

                    System.Data.SqlClient.SqlCommand sqlCmd = new System.Data.SqlClient.SqlCommand(sqlCmdTxtGetTB, sqlConn);

                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }

                    string tableName = "";

                    using (System.Data.SqlClient.SqlDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.Default))
                    {
                        while (reader.Read())
                        {
                            tableName = reader.GetString(0);
                        }
                    }

                    if (String.IsNullOrEmpty(tableName))
                    {
                        sqlCmd = new System.Data.SqlClient.SqlCommand(sqlCmdTxtCreateTB, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private CodeItem[] buildTreeItems(CodeItem[] codeItems) 
        {
            List<CodeItem> nodes = new List<CodeItem>(codeItems);

            List<CodeItem> parentNodes = new List<CodeItem>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID.ToLower() == nodes[i].Parent.ToLower())
                {
                    parentNodes.Add(nodes[i]);
                    nodes.RemoveAt(i);
                    i--;
                }
            }

            Dictionary<string, List<CodeItem>> childNodes = new Dictionary<string, List<CodeItem>>();

            for (int i = 0; i < parentNodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodes[j].Parent.ToLower() == parentNodes[i].ID.ToLower())
                    {
                        if (childNodes.ContainsKey(parentNodes[i].ID.ToLower()))
                        {
                            childNodes.Add(parentNodes[i].ID.ToLower(), new List<CodeItem>(new CodeItem[] { nodes[j] }));
                            nodes.RemoveAt(j);
                            j--;
                        }
                        else
                        {
                            childNodes[parentNodes[i].ID.ToLower()].Add(nodes[j]);
                            j--;
                        }
                    }
                }
            }

            for (int i = 0; i < parentNodes.Count; i++)
            {
                if (childNodes[parentNodes[i].ID.ToLower()] != null)
                {
                    parentNodes[i].Children = childNodes[parentNodes[i].ID.ToLower()].ToArray();
                }
            }

            childNodes = new Dictionary<string, List<CodeItem>>();

            foreach (var parentNode in parentNodes)
            {
                if (parentNode.Children != null)
                {
                    for (int i = 0; i < parentNode.Children.Length; i++)
                    {
                        for (int j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Parent.ToLower() == parentNode.Children[i].ID.ToLower())
                            {
                                if (childNodes.ContainsKey(parentNode.Children[i].ID.ToLower()))
                                {
                                    childNodes.Add(parentNode.Children[i].ID.ToLower(), new List<CodeItem>(new CodeItem[] { nodes[j] }));
                                    nodes.RemoveAt(j);
                                    j--;
                                }
                                else
                                {
                                    childNodes[parentNode.Children[i].ID.ToLower()].Add(nodes[j]);
                                    j--;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var parentNode in parentNodes)
            {
                if (parentNode.Children != null)
                {
                    foreach (var childNode in parentNode.Children)
                    {
                        if (childNodes[childNode.ID.ToLower()] != null)
                        {
                            childNode.Children = childNodes[parentNode.ID.ToLower()].ToArray();
                        }
                    }
                }
            }

            for (int i = 0; i < parentNodes.Count; i++)
            {
                if (parentNodes[i].Children != null)
                {
                    for (int j = 0; j < parentNodes[i].Children.Length; j++)
                    {
                        if (childNodes[parentNodes[i].Children[j].ID.ToLower()] != null)
                        {
                            parentNodes[i].Children[j].Children = childNodes[parentNodes[i].ID.ToLower()].ToArray();
                        }
                    }
                }
            }

            return parentNodes.ToArray();
        }

        public string CreateTreeDataSave(string json) 
        {
            CodeItem[] items = Newtonsoft.Json.JsonConvert.DeserializeObject<CodeItem[]>(json);

            if (items.Length <= 0)
            {
                return "err@数据错误,保存的值为空.";
            }

            //删除原来的数据.
            BP.Sys.SFTable sf = new BP.Sys.SFTable(this.FK_SFTable);
            //sf.RunSQL("DELETE FROM " + sf.No);
            sf.RunSQL("DELETE FROM " + this.FK_SFTable);

            string sql = "";

            foreach (var item in items)
            {
                //saveTreeItem(item);
                sql = String.Format("INSERT INTO {0} (No, Name, {1}) Values ('{2}', '{3}', '{4}')", this.FK_SFTable, sf.ParentValue, item.ID, item.Value, item.Parent);
                sf.RunSQL(sql);

                if (item.Children != null && item.Children.Length > 0)
                {
                    foreach (var child in item.Children)
                    {
                        //saveTreeItem(child);
                        sql = String.Format("INSERT INTO {0} (No, Name, {1}) Values ('{2}', '{3}', '{4}')", this.FK_SFTable, sf.ParentValue, child.ID, child.Value, child.Parent);
                        sf.RunSQL(sql);

                        if (child.Children != null && child.Children.Length> 0)
                        {
                            foreach (var chld in child.Children)
                            {
                                //saveTreeItem(chld);
                                sql = String.Format("INSERT INTO {0} (No, Name, {1}) Values ('{2}', '{3}', '{4}')", this.FK_SFTable, sf.ParentValue, chld.ID, chld.Value, chld.Parent);
                                sf.RunSQL(sql);
                            }
                        }
                    }
                }
            }

            return "";
        }

        //private string saveTreeItem(CodeItem codeItem) 
        //{
        //    return codeItem.ID;
        //}

        public string CreateTableDataInit()
        {
            if (String.IsNullOrEmpty(this.FK_SFTable))
            {
                return "";
            }

            SFTable sf = new SFTable(this.FK_SFTable);
            string sql = "SELECT * FROM "+ sf.No;
            DataTable dt = sf.RunSQLReturnTable(sql);

            string json = "";
            List<CodeItem> items = new List<CodeItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i] != null)
                    {
                        items.Add(new CodeItem()
                        {
                            ID = dt.Rows[i]["No"].ToString(),
                            Value = dt.Rows[i]["Name"].ToString(),
                            Parent = dt.Columns.Contains("ParentNo") ? dt.Rows[i]["ParentNo"].ToString() : ""
                        });
                    }
                }

                //return BP.Tools.Json.ToJson(models.ToArray());

                json = Newtonsoft.Json.JsonConvert.SerializeObject(items.ToArray());

                //return json;
            }
            else//如果表中没有数据，则自动向该表插入3条默认数据
            {
                //items.AddRange(new CodeItem[] 
                //{
                //    new CodeItem()
                //    { 
                //        ID = String.Format("{0}-Item-001", this.FK_SFTable), 
                //        Value = String.Format("{0}-Item-001", this.FK_SFTable)
                //    },
                //    new CodeItem()
                //    {
                //        ID = String.Format("{0}-Item-002", this.FK_SFTable), 
                //        Value = String.Format("{0}-Item-002", this.FK_SFTable)
                //    },
                //    new CodeItem()
                //    {
                //        ID = String.Format("{0}-Item-003", this.FK_SFTable), 
                //        Value = String.Format("{0}-Item-003", this.FK_SFTable)
                //    }
                //});

                for (int i = 0; i < 3; i++)
                {
                    items.Add(new CodeItem()
                    { 
                        ID = String.Format("{0}", (i + 1)),
                        Value = String.Format("{0}-Item-{1}", this.FK_SFTable, (i + 1))
                    });
                }

                json = Newtonsoft.Json.JsonConvert.SerializeObject(items.ToArray());

                foreach (var item in items)
                {
                    sql = "INSERT INTO " + this.FK_SFTable + " (No,Name)Values('" + item.ID + "','" + item.Value + "')";
                    sf.RunSQL(sql);
                }
            }

            return json;

            //return BP.Tools.Json.ToJson(dt);
        }

        public void WriteInfo(string info)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(info);
        }
        /// <summary>
        /// 执行保存.
        /// </summary>
        /// <returns></returns>
        public string CreateTableDataSave(string json= "@001=xxxx@002=xxxxx")
        {
            CodeItem[] items = Newtonsoft.Json.JsonConvert.DeserializeObject<CodeItem[]>(json);
            if (items.Length <= 0)
            {
                return "err@数据错误,保存的值为空.";
            }

            if (String.IsNullOrEmpty(this.FK_SFTable))
            {
                return "err@参数错误.";
            }

            //删除原来的数据.
            BP.Sys.SFTable sf = new BP.Sys.SFTable(this.FK_SFTable);
            sf.RunSQL("DELETE FROM " + sf.No);

            string sql = "";
            foreach (var item in items)
            {
                if (!String.IsNullOrEmpty(sf.ParentValue))
                {
                    sql = String.Format("INSERT INTO {0} (No, Name, {1}) Values ('{2}', '{3}', '{4}')", this.FK_SFTable, sf.ParentValue, item.ID, item.Value, item.Parent);
                }
                else
                {
                    sql = String.Format("INSERT INTO {0} (No, Name) Values ('{1}', '{2}')", this.FK_SFTable, item.ID, item.Value);
                }
                sf.RunSQL(sql);
            }

            return "保存成功";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}


namespace CCFlow.ViewModels
{
    public class CodeItem
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "parent")]
        public string Parent { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "children")]
        public CodeItem[] Children { get; set; }
    }
}