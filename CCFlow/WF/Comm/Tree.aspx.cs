using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using BP.En;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;

namespace CCOA.Comm
{
    public partial class Tree : System.Web.UI.Page
    {
        public string EnsDesc
        {
            get
            {
                if (this.EnsName == null || this.HisEn == null)
                    return "树形结构";
                return this.HisEn.EnDesc;
            }
        }
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EnName
        {
            get
            {
                if (this._HisEn != null)
                    return this.HisEn.ToString();
                return "";
            }
        }
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EnsName
        {
            get
            {
                return getUTF8ToString("EnsName");
            }
        }
        /// <summary>
        /// 实体集合
        /// </summary>
        public Entities _HisEns = null;
        public Entities HisEns
        {
            get
            {
                if (this.EnsName != null)
                {
                    if (this._HisEns == null)
                    {
                        _HisEns = BP.En.ClassFactory.GetEns(this.EnsName.Replace("#", ""));
                    }
                }
                return _HisEns;
            }
        }
        /// <summary>
        /// 单个实体
        /// </summary>
        private Entity _HisEn = null;
        public Entity HisEn
        {
            get
            {
                if (_HisEn == null)
                {
                    if (this.HisEns == null)
                    {
                        _HisEn = BP.En.ClassFactory.GetEn(this.EnsName.Replace("#", ""));
                        if (this._HisEn == null)
                            throw new Exception("在此项目中没有找到命名空间及符合的类：" + this.EnsName);
                    }
                    else
                        _HisEn = this.HisEns.GetNewEntity;

                    EntityTree enTree = _HisEn as EntityTree;
                    if (enTree == null)
                    {
                        if (_HisEn.IsNoEntity)
                            throw new Exception("传入的实体必须继承于EntityTree;本次传入" + this.EnsName + "继承于：NoEntity。");
                        else if (_HisEn.IsOIDEntity)
                            throw new Exception("传入的实体必须继承于EntityTree;本次传入" + this.EnsName + "继承于：OIDEntity。");
                        else if (_HisEn.IsMIDEntity)
                            throw new Exception("传入的实体必须继承于EntityTree;本次传入" + this.EnsName + "继承于：MIDEntity。");
                        else
                            throw new Exception("传入的实体必须继承于EntityTree。");
                    }
                }
                return _HisEn;
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
            //if (BP.Web.WebUser.No == null)
            //    return;

            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (DataType.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "gettreenodes"://获取树节点
                    s_responsetext = GetTreeNodes();
                    break;
                case "treesortmanage"://知识树操作
                    s_responsetext = TreeNodeManage();
                    break;
                case "updatetreenodename"://修改知识类别名称
                    s_responsetext = UpdateTreeNodeName();
                    break;
                case "gettreenodename"://获取节点文本
                    s_responsetext = GetTreeNodeName();
                    break;
            }
            if (DataType.IsNullOrEmpty(s_responsetext))
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
        /// 获取树节点
        /// </summary>
        /// <returns></returns>
        private string GetTreeNodes()
        {
            try
            {
                string parentNo = getUTF8ToString("ParentNo");

                Entities ens = this.HisEn.GetNewEntities;
                ens.RetrieveAll(EntityTreeAttr.Idx);

                TansEntitiesToGenerTree(ens, parentNo);
                return appendMenus.ToString();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("调用的目标发生了异常"))
                    return "error:" + ex.Message + " 或用户没有登录。";

                return "error:" + ex.Message;
            }
        }

        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo)
        {
            EntityTree root = ens.GetEntityByKey(EntityTreeAttr.ParentNo, rootNo) as EntityTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            if(root.No.Equals(rootNo))
                throw new Exception("@根节点编号不能与父节点编号相同：No=" + rootNo);
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");

            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        public void AddChildren(EntityTree parentEn, Entities ens)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"state\":\"closed\"");
                EntityTree treeNode = item as EntityTree;
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }

        /// <summary>
        /// 树节点操作
        /// </summary>
        /// <returns></returns>
        private string TreeNodeManage()
        {
            string nodeNo = getUTF8ToString("nodeNo");
            string dowhat = getUTF8ToString("dowhat");
            string returnVal = "";

            EntityTree treeNode = this.HisEn as EntityTree;
            treeNode.RetrieveByAttr(EntityTreeAttr.No, nodeNo);

            switch (dowhat.ToLower())
            {
                case "sample"://新增同级节点   
                    EntityTree enTree = treeNode.DoCreateSameLevelNode();
                    returnVal = "{No:'" + enTree.No + "',Name:'" + enTree.Name + "'}";
                    break;
                case "children"://新增子节点
                    enTree = treeNode.DoCreateSubNode();
                    returnVal = "{No:'" + enTree.No + "',Name:'" + enTree.Name + "'}";
                    break;
                case "doup"://上移
                    treeNode.DoUp();
                    break;
                case "dodown"://下移
                    treeNode.DoDown();
                    break;
                case "delete"://删除
                    treeNode.Delete();
                    break;
            }
            //返回
            return returnVal;
        }

        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        private string UpdateTreeNodeName()
        {
            string nodeNo = getUTF8ToString("nodeNo");
            string nodeName = getUTF8ToString("nodeName");

            EntityTree treeNode = this.HisEn as EntityTree;
            treeNode.RetrieveByAttr(EntityTreeAttr.No, nodeNo);
            treeNode.Name = nodeName;
            int i = treeNode.Update();
            if (i > 0)
            {
                return "true";
            }

            return "false";
        }

        /// <summary>
        /// 根据编号获取节点文本
        /// </summary>
        /// <returns></returns>
        private string GetTreeNodeName()
        {
            string nodeNo = getUTF8ToString("nodeNo");
            EntityTree treeNode = this.HisEn as EntityTree;
            treeNode.RetrieveByAttr(EntityTreeAttr.No, nodeNo);

            return treeNode.Name;
        }
    }
}