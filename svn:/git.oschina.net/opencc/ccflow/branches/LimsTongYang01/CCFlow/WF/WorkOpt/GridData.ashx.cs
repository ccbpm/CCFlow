using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;
using CCFlow.WF.UC;
using Newtonsoft.Json;
using System.Data;

namespace CCFlow.WF.WorkOpt
{
    /// <summary>
    /// GridData1 的摘要说明
    /// </summary>
    public class GridData1 : IHttpHandler
    {
        private string _FK_MapData = null;
        private string FK_MapData
        {
            get
            {
                if (string.IsNullOrEmpty(this._FK_MapData))
                    return null;

                return this._FK_MapData;
            }
            set
            {
                this._FK_MapData = value;
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            string WorkID = context.Request.QueryString["WorkID"];
            string FK_Flow = context.Request.QueryString["FK_Flow"];
            string FK_Node = context.Request.QueryString["FK_Node"];
            string doType = context.Request.QueryString["DoType"];
            FK_MapData = context.Request.QueryString["FK_MapData"];

            string jsonResult = "";
            switch (doType)
            {
                case "0":
                    jsonResult = GetDtlCount(WorkID, FK_Flow, FK_Node);
                    break;
                case "1":

                    string getName = context.Request.QueryString["Name"];
                    if (!string.IsNullOrEmpty(getName))
                    {
                        jsonResult = GetMainPage(WorkID, FK_Flow, FK_Node, getName);
                    }
                    break;
                case "2":
                    jsonResult = GetDtlCountFlow(FK_Flow, FK_Node);
                    break;
                case "3":
                    jsonResult = GetChildDtlCount(FK_Flow, FK_Node);
                    break;
                case "4":
                    string getType = context.Request.QueryString["GetType"];
                    jsonResult = GetDataByType(FK_Flow, FK_Node, WorkID, getType);
                    break;
                case "5"://获取所有的图片信息

                    jsonResult = GetFlowPhoto(WorkID, FK_Flow, FK_Node);
                    break;
                case "6"://获取所有的图片信息
                    jsonResult = GetPhoto(WorkID, FK_Flow, FK_Node);
                    break;
                default:
                    break;
            }
            context.Response.Clear();
            context.Response.Write(jsonResult);
        }

        private string GetFlowPhoto(string workID, string flow, string node)
        {
            string json = null;
            node = "ND" + node;
            string sql = string.Format("select EnPk,ImgPath from Sys_FrmImg where FK_MapData ='{0}' and ImgPath is not null UNION ALL  select v1.EnPK ,v2.Tag1 as ImgPath  from Sys_FrmImg v1 left join Sys_FrmEleDB v2 on v1.MyPK = v2.RefPKVal  where v1.FK_MapData='{0}' and EleID='{1}'", node, workID);

            System.Data.DataTable table = DBAccess.RunSQLReturnTable(sql);

            List<PhotoEntity> photoEntities = new List<PhotoEntity>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    photoEntities.Add(new PhotoEntity() { Name = row[0].ToString(), Value = row[1].ToString() });
                }
            }
            json = JsonConvert.SerializeObject(photoEntities);
            return json;
        }

        private string GetPhoto(string workID, string fk_flow, string fk_node)
        {
            string json = null;
            FrmNodes fns = new FrmNodes(fk_flow, int.Parse(fk_node));

            List<PhotoEntity> photoEntities = new List<PhotoEntity>();
            foreach (FrmNode fn in fns)
            {
                string sql = string.Format("select EnPk,ImgPath from Sys_FrmImg where FK_MapData ='{0}' and ImgPath is not null UNION ALL  select v1.EnPK ,v2.Tag1 as ImgPath  from Sys_FrmImg v1 left join Sys_FrmEleDB v2 on v1.MyPK = v2.RefPKVal  where v1.FK_MapData='{0}' and EleID='{1}'", fn.FK_Frm, workID);

                System.Data.DataTable table = DBAccess.RunSQLReturnTable(sql);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        photoEntities.Add(new PhotoEntity() { Name = row[0].ToString(), Value = row[1].ToString() });
                    }
                }
            }
            json = JsonConvert.SerializeObject(photoEntities);
            return json;
        }

        private string GetChildDtlCount(string fk_flow, string fk_node)
        {
            FrmNodes fns = new FrmNodes(fk_flow, int.Parse(fk_node));
            List<TempEntity> list = new List<TempEntity>();
            foreach (FrmNode fn in fns)
            {
                if (this.FK_MapData != null && this.FK_MapData.Equals(fn.FK_Frm) == false)
                    continue;

                MapDtls mdtls = new MapDtls(fn.FK_Frm);

                foreach (MapDtl single in mdtls)
                {
                    list.Add(new TempEntity() { Name = single.No });
                }
            }
            return JsonConvert.SerializeObject(list);
        }

        private string GetDataByType(string fk_flow, string fk_node, string workID, string getType)
        {
            FrmNodes fns = new FrmNodes(fk_flow, int.Parse(fk_node));

            string result = "";
            if (getType != "MainPage")
            {
                result = "{\"jsonDtl\":";
                foreach (FrmNode fn in fns)
                {
                    if (this.FK_MapData != null && this.FK_MapData.Equals(fn.FK_Frm) == false)
                        continue;

                    MapDtls mdtls = new MapDtls(fn.FK_Frm);

                    foreach (MapDtl dtl in mdtls)
                    {
                        if (dtl.No.Equals(getType))
                        {
                            GEDtls ens = new GEDtls(dtl.No);
                            ens.Retrieve(GEDtlAttr.RefPK, workID);
                            result += JsonConvert.SerializeObject(ens.ToDataTableField());
                            break;
                        }
                    }
                }
                result += "}";
            }
            else
            {
                result = "{\"MainPage\":[";
                foreach (FrmNode fn in fns)
                {
                    if (this.FK_MapData != null && this.FK_MapData.Equals(fn.FK_Frm) == false)
                        continue;

                    GEEntity ge = new GEEntity(fn.FK_Frm, workID);
                    result += JsonConvert.SerializeObject(ge.Row);
                }
                result += "]}";
            }
            return result;
        }

        private string GetDtlCountFlow(string flow, string node)
        {
            FrmNodes fns = new FrmNodes(flow, int.Parse(node));
            List<TempEntity> list = new List<TempEntity>();
            foreach (FrmNode fn in fns)
            {
                list.Add(new TempEntity() { Name = fn.FK_Frm });
            }
            return JsonConvert.SerializeObject(list);
        }


        private string GetDtlCount(string workID, string flow, string node)
        {
            Flow myFlow = new Flow(flow);

            string pTable = myFlow.PTable;

            if (string.IsNullOrEmpty(pTable))
            {
                pTable = "ND" + int.Parse(flow) + "Rpt";
            }

            Node nd = new Node(node);
            Work wk = nd.HisWork;
            wk.OID = int.Parse(workID);
            wk.Retrieve();
            wk.ResetDefaultVal();

            GEEntity ndxxRpt = new GEEntity(pTable);
            ndxxRpt.PKVal = workID;
            ndxxRpt.Retrieve();
            ndxxRpt.Copy(wk);

            //把数据赋值给wk.
            wk.Row = ndxxRpt.Row;

            //string jsonData = null;

            //执行序列化
            // string jsonData = JsonConvert.SerializeObject(wk.Row);

            // jsonData = jsonData.Substring(1, jsonData.Length - 2);
            //加入他的明细表.
            List<Entities> al = wk.GetDtlsDatasOfList();

            List<TempEntity> tempName = new List<TempEntity>();

            foreach (Entities singleEntities in al)
            {
                tempName.Add(new TempEntity() { Name = singleEntities.GetNewEntity.ToString() });
            }
            return JsonConvert.SerializeObject(tempName);
        }

        /// <summary>
        /// 节点表单数据
        /// </summary>
        /// <param name="workID">业务编号</param>
        /// <param name="flow">流程编号</param>
        /// <param name="node">节点编号</param>
        /// <param name="name">明细表编号</param>
        /// <returns></returns>
        private string GetMainPage(string workID, string flow, string node, string name)
        {
            string jsonData = "{";

            Flow myFlow = new Flow(flow);

            string pTable = myFlow.PTable;

            if (string.IsNullOrEmpty(pTable))
            {
                pTable = "ND" + int.Parse(flow) + "Rpt";
            }

            Node nd = new Node(node);
            Work wk = nd.HisWork;
            wk.OID = int.Parse(workID);
            wk.Retrieve();
            wk.ResetDefaultVal();

            GEEntity ndxxRpt = new GEEntity(pTable);
            ndxxRpt.PKVal = workID;
            ndxxRpt.Retrieve();
            ndxxRpt.Copy(wk);


            //执行序列化
            // string jsonData = JsonConvert.SerializeObject(wk.Row);

            // jsonData = jsonData.Substring(1, jsonData.Length - 2);
            //加入他的明细表.
            List<Entities> al = wk.GetDtlsDatasOfList();
            if (!name.Equals("MainPage"))
            {
                foreach (Entities singlEntities in al)
                {
                    if (singlEntities.GetNewEntity.ToString() == name)
                    {
                        jsonData += "\"jsonDtl\":" + JsonConvert.SerializeObject(singlEntities.ToDataTableField()) + "}";
                        break;
                    }
                }
            }
            else
            {
                //把数据赋值给wk.
                wk.Row = ndxxRpt.Row;
                jsonData = "{\"MainPage\":[";
                jsonData += JsonConvert.SerializeObject(wk.Row);
                jsonData += "]}";
            }
            return jsonData;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class TempEntity
    {
        public string Name { get; set; }
    }

    public class PhotoEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}