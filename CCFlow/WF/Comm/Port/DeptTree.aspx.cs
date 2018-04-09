using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Text;
using System.Data;
using BP.En;
using BP.DA;
using BP.WF.Template;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.Comm.Port
{
    public partial class DeptTree : System.Web.UI.Page
    {
        public string FK_Node
        {
            get
            {
                try
                {
                    string nodeid = this.Request.QueryString["NodeID"];
                    if (nodeid == null)
                        nodeid = this.Request.QueryString["FK_Node"];
                    return nodeid;
                }
                catch
                {
                    return "101"; // 0; 有可能是流程调用独立表单。
                }
            }
        }

        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;

            if (DataType.IsNullOrEmpty(WebUser.No) || DataType.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getTreeDateMet"://获取部门数据
                    s_responsetext = getTreeDateMet();
                    break;
                case "insertMet"://保存数据
                    s_responsetext = insertMet();
                    break;
            }
            if (DataType.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端 树型
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <returns></returns>
        private string getTreeDateMet()
        {
            string sql = "";
            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                sql = "select NO,NAME,ParentNo from port_dept ORDER BY Idx";
            else
                sql = "select NO,NAME,ParentNo from port_dept  ";

            DataTable dt_dept = DBAccess.RunSQLReturnTable(sql);
            string s_responsetext = string.Empty;
            string s_checkded = string.Empty;
            //节点部门集合
            NodeDepts nodeDepts = new NodeDepts();
            QueryObject obj = new QueryObject(nodeDepts);
            obj.AddWhere(NodeDeptAttr.FK_Node, this.FK_Node);
            obj.DoQuery();
            //将已有部门，拼接字符串
            if (nodeDepts != null && nodeDepts.Count > 0)
            {
                foreach (NodeDept item in nodeDepts)
                {
                    s_checkded += "," + item.FK_Dept + ",";
                }
            }
            s_responsetext = GetTreeJsonByTable(dt_dept, "NO", "NAME", "ParentNo", "0", s_checkded);
            if (DataType.IsNullOrEmpty(s_responsetext) || s_responsetext == "[]")//如果为空，使用另一种查询
            {
                treeResult.Clear();
                s_responsetext = GetTreeJsonByTable(dt_dept, "NO", "NAME", "ParentNo", "O0", s_checkded);
            }
            return s_responsetext;
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        private string insertMet()
        {
            string getId = getUTF8ToString("getId");
            NodeDept nodeDept = new NodeDept();
            nodeDept.Delete(NodeDeptAttr.FK_Node, this.FK_Node);

            if (!DataType.IsNullOrEmpty(getId))
            {
                string[] Depts = getId.Split(',');
                for (int i = 0; i < Depts.Length; i++)
                {
                    nodeDept = new NodeDept();
                    nodeDept.FK_Node = int.Parse(this.FK_Node);
                    nodeDept.FK_Dept = Depts[i];
                    nodeDept.Insert();
                }
            }

            return "true";
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        /// <param name="tabel">数据源</param>
        /// <param name="idCol">ID列</param>
        /// <param name="txtCol">Text列</param>
        /// <param name="rela">关系字段</param>
        /// <param name="pId">父ID</param>
        ///<returns>easyui tree json格式</returns>
        StringBuilder treeResult = new StringBuilder();
        StringBuilder treesb = new StringBuilder();
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId, string CheckedString)
        {
            string treeJson = string.Empty;
            treeResult.Append(treesb.ToString());

            treesb.Clear();
            if (tabel.Rows.Count > 0)
            {
                treesb.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        string deptNo = row[idCol].ToString();

                        if (treeResult.Length == 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                 + "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                 + "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"closed\"");
                        }
                        else
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                 + "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower());
                        }


                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol], CheckedString);
                            treeResult.Append(treesb.ToString());
                            treesb.Clear();
                        }
                        treeResult.Append(treesb.ToString());
                        treesb.Clear();
                        treesb.Append("},");
                    }
                    treesb = treesb.Remove(treesb.Length - 1, 1);
                }
                treesb.Append("]");
                treeResult.Append(treesb.ToString());
                treeJson = treeResult.ToString();
                treesb.Clear();
            }
            return treeJson;
        }
    }
}