using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.WF;

namespace CCFlow.WF.Admin
{
    public partial class FlowFormTree : System.Web.UI.Page
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.Web.WebUser.No != "admin")
                throw new Exception("@非法的用户必须由admin才能操作，现在登录用户是：" + BP.Web.WebUser.No);
            
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(Request["method"]))
                method = Request["method"].ToString();

            switch (method)
            {
                case "getflowformtree":
                    s_responsetext = GetFlowFormTree();
                    break;
                case "getnodeformtree":
                    s_responsetext = GetNodeFormTree();
                    break;
                case "saveflowformtree":
                    s_responsetext = SaveFlowFormTree();
                    break;
                case "savenodeformtree":
                    s_responsetext = SaveNodeFormTree();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 获取独立表单树
        /// </summary>
        /// <returns></returns>
        private string GetFlowFormTree()
        {
            string flowId = getUTF8ToString("flowId");
            string parentNo = getUTF8ToString("parentno");
            string isFirstLoad = getUTF8ToString("isFirstLoad");

            //获取子节点内容
            SysFormTrees flowFormTrees = new SysFormTrees();
            QueryObject objInfo = new QueryObject(flowFormTrees);
            objInfo.AddWhere("ParentNo", parentNo);
            objInfo.addOrderBy("Idx");
            objInfo.DoQuery();
            
            if (isFirstLoad == "true")
            {
                SysFormTree formTree = new SysFormTree("0");
                StringBuilder appSend = new StringBuilder();
                appSend.Append("[");
                appSend.Append("{");
                appSend.Append("\"id\":\"0\"");
                appSend.Append(",\"text\":\"" + formTree.Name + "\"");

                appSend.Append(",iconCls:\"icon-0\"");
                appSend.Append(",\"children\":");
                appSend.Append("[");
                //获取节点下的表单
                SysForms sysForms = new SysForms();
                QueryObject objFlowForms = new QueryObject(sysForms);
                objFlowForms.AddWhere(SysFormAttr.FK_FormTree, parentNo);
                objFlowForms.addOrderBy(SysFormAttr.Name);
                objFlowForms.DoQuery();
                
                //添加子项文件夹
                foreach (SysFormTree item in flowFormTrees)
                {
                    //获取已选择项
                    FrmNodes flowForms = new FrmNodes();
                    QueryObject objFlowForm = new QueryObject(flowForms);
                    objFlowForm.AddWhere("FK_Flow", flowId);
                    objFlowForm.addAnd();
                    objFlowForm.AddWhere("FK_FlowFormTree", item.No);
                    objFlowForm.DoQuery();

                    if (flowForms != null && flowForms.Count > 0)
                    {

                    }
                }
                //添加表单
                foreach (SysForm sysForm in sysForms)
                {
                    appSend.Append("{");
                    appSend.Append("\"id\":\"0\"");
                    appSend.Append(",\"text\":\"" + formTree.Name + "\"");

                    appSend.Append(",iconCls:\"icon-3\"");
                    appSend.Append("},");
                }
                appSend.Append("]");
                appSend.Append("}");
                appSend.Append("]");
                return appSend.ToString();
            }

            return "";
        }
        /// <summary>
        /// 获取节点表单树
        /// </summary>
        /// <returns></returns>
        private string GetNodeFormTree()
        {
            return "";
        }
        /// <summary>
        /// 保存独立表单树
        /// </summary>
        /// <returns></returns>
        private string SaveFlowFormTree()
        {
            return "";
        }
        /// <summary>
        /// 保存节点表单树
        /// </summary>
        /// <returns></returns>
        private string SaveNodeFormTree()
        {
            return "";
        }

        /// <summary>
        /// 获取树节点列表
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="checkIds"></param>
        /// <returns></returns>
        public string GetTreeList(EntitiesSimpleTree ens, string checkIds, string unCheckIds)
        {
            StringBuilder appSend = new StringBuilder();
            appSend.Append("[");
            foreach (EntitySimpleTree item in ens)
            {
                if (appSend.Length > 1) appSend.Append(",{"); else appSend.Append("{");

                appSend.Append("\"id\":\"" + item.No + "\"");
                appSend.Append(",\"text\":\"" + item.Name + "\"");

                SysFormTree node = item as SysFormTree;

                //文件夹节点图标
                string ico = "icon-tree_folder";
                //判断未完全选中
                if (unCheckIds.Contains("," + item.No + ","))
                    ico = "collaboration";

                appSend.Append(",iconCls:\"");
                appSend.Append(ico);
                appSend.Append("\"");

                if (checkIds.Contains("," + item.No + ","))
                    appSend.Append(",\"checked\":true");

                //判断是否还有子节点icon-3
                //BP.GPM.Menus menus = new BP.GPM.Menus();
                //menus.RetrieveByAttr("ParentNo", item.No);
                //if (menus != null && menus.Count > 0)
                //{
                //    appSend.Append(",state:\"closed\"");
                //    appSend.Append(",\"children\":");
                //    appSend.Append("[{");
                //    appSend.Append(string.Format("\"id\":\"{0}\",\"text\":\"{1}\"", item.No + "01", "加载中..."));
                //    appSend.Append("}]");
                //}
                appSend.Append("}");
            }
            appSend.Append("]");

            return appSend.ToString();
        }
    }
}