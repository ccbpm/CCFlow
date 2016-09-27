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
using Newtonsoft.Json.Utilities;
using CCFlow.ViewModels;

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

            switch (this.DoType)
            {
                case "TreeInit":
                    WriteInfo(TreeInit()); //输出数据.
                    break;
                case "CreateTableDataInit":
                    WriteInfo( CreateTableDataInit()); //输出数据.
                    break;
                case "CreateTableDataSave":
                    string json = "";
                    using (StreamReader reader = new System.IO.StreamReader(context.Request.InputStream))
                    {
                        json = reader.ReadToEnd();
                    }
                    WriteInfo( CreateTableDataSave(json)); //输出保存结果..
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 初始化树数据
        /// </summary>
        /// <returns></returns>
        public string TreeInit()
        {
            SFTable sf = new SFTable(this.FK_SFTable);
            DataTable dt = sf.RunSQLReturnTable("SELECT * FROM "+sf.No);
            if (dt.Rows.Count == 0)
            {
                string sql = "INSERT INTO "+sf.No+" (No,Name,ParentNo) VALUES ('001','根节点','0')";
            }
            return BP.Tools.Json.ToJson(dt);
        }

        public string CreateTableDataInit()
        {
            if (String.IsNullOrEmpty(this.FK_SFTable))
                return "";

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
                            Value = dt.Rows[i]["Name"].ToString()
                        });
                    }
                }

                json = Newtonsoft.Json.JsonConvert.SerializeObject(items.ToArray());
            }
            else//如果表中没有数据，则自动向该表插入3条默认数据
            {
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
            //DataTable dt = null; //BP.Tools.Json.ToDataTable(json);
            //if (dt.Rows.Count == 0)
            //    return "err@数据错误,保存的值为空.";

<<<<<<< .mine
           // DictionaryItemViewModel[] items = Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryItemViewModel[]>(json);
=======
            CodeItem[] items = Newtonsoft.Json.JsonConvert.DeserializeObject<CodeItem[]>(json);
>>>>>>> .r734

            //if (items.Length <= 0)
            //{
            //    return "err@数据错误,保存的值为空.";
            //}

            //if (String.IsNullOrEmpty(this.FK_SFTable))
            //{
            //    return "";
            //}

            ////删除原来的数据.
            //BP.Sys.SFTable sf = new BP.Sys.SFTable(this.FK_SFTable);
            //sf.RunSQL("DELETE FROM " + sf.No);

            ////把新数据插入到数据库.
            ////foreach (DataRow dr in dt.Rows)
            ////{
            ////    string sql = "INSERT INTO "+sf.SrcTable+" (No,Name)Values('"+dr[0]+"','"+dr[1]+"')";
            ////    sf.RunSQL(sql);
            ////}

            //string sql = "";

            //foreach (var item in items)
            //{
            //   sql = "INSERT INTO " + sf.SrcTable + " (No,Name)Values('" + item.ID + "','" + item.Value + "')";
            //   sf.RunSQL(sql);
            //}

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