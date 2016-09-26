﻿using System;
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
using System.IO;

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

        public string CreateTableDataInit()
        {
            if (String.IsNullOrEmpty(this.FK_SFTable))
            {
                return "";
            }

            SFTable sf = new SFTable(this.FK_SFTable);
            string sql = "SELECT * FROM "+sf.No;
            DataTable dt = sf.RunSQLReturnTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<DictionaryItemViewModel> models = new List<DictionaryItemViewModel>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i] != null)
                    {
                        models.Add(new DictionaryItemViewModel()
                        {
                            ID = dt.Rows[i]["No"].ToString(),
                            Value = dt.Rows[i]["Name"].ToString()
                        });
                    }
                }

                //return BP.Tools.Json.ToJson(models.ToArray());

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(models.ToArray());

                return json;
            }

            return BP.Tools.Json.ToJson(dt);
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

            DictionaryItemViewModel[] items = Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryItemViewModel[]>(json);

            if (items.Length <= 0)
            {
                return "err@数据错误,保存的值为空.";
            }

            if (String.IsNullOrEmpty(this.FK_SFTable))
            {
                return "";
            }

            //删除原来的数据.
            BP.Sys.SFTable sf = new BP.Sys.SFTable(this.FK_SFTable);
            sf.RunSQL("DELETE FROM " + sf.No);

            //把新数据插入到数据库.
            //foreach (DataRow dr in dt.Rows)
            //{
            //    string sql = "INSERT INTO "+sf.SrcTable+" (No,Name)Values('"+dr[0]+"','"+dr[1]+"')";
            //    sf.RunSQL(sql);
            //}

            string sql = "";

            foreach (var item in items)
            {
               sql = "INSERT INTO " + this.FK_SFTable + " (No,Name)Values('" + item.ID + "','" + item.Value + "')";
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
    public class DictionaryItemViewModel
    {
        public string ID { get; set; }

        public string Value { get; set; }

        public string Parent { get; set; }
    }
}