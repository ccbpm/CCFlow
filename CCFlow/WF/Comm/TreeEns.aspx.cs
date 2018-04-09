using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class TreeEns : System.Web.UI.Page
    {
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 树形实体名称
        /// </summary>
        public string TreeEnsName
        {
            get
            {
                return getUTF8ToString("TreeEnsName");
            }
        }
        public string RefPK
        {
            get
            {
                return getUTF8ToString("RefPK");
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
        /// 树形实体名称
        /// </summary>
        public string TreeEnName
        {
            get
            {
                Entity en = GetEntityByEnName(TreeEnsName);
                if (en != null)
                    return en.ClassID;
                return "";
            }
        }
        /// <summary>
        /// 树形解释
        /// </summary>
        public string TreeEnsDesc
        {
            get
            {
                Entity en = GetEntityByEnName(TreeEnsName);
                if (this.TreeEnsName == null || en == null)
                    return "树形结构";
                return en.EnDesc;
            }
        }
        /// <summary>
        ///实体名称
        /// </summary>
        public string EnName
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (en != null)
                    return en.ClassID;
                return "";
            }
        }
        /// <summary>
        /// 解释
        /// </summary>
        public string EnsDesc
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (this.EnsName == null || en == null)
                    return "实体名";
                return en.EnDesc;
            }
        }
        /// <summary>
        /// 实体主键
        /// </summary>
        public string EnPK
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (this.EnsName == null || en == null)
                    return "No";
                return en.PK;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
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
                case "getensgriddata"://获取列表数据
                    s_responsetext = GetEnsGridData();
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
                Entity en = GetEntityByEnName(TreeEnsName);

                Entities ens = en.GetNewEntities;
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
        /// 获取ens数据
        /// </summary>
        /// <returns></returns>
        private string GetEnsGridData()
        {
            string RefPK = getUTF8ToString("RefPK");
            string FK = getUTF8ToString("FK");

            Entity en = GetEntityByEnName(EnsName);
            Entities ens = en.GetNewEntities;
            ens.RetrieveByAttr(RefPK, FK);

            return TranslateEntitiesToGridJsonColAndData(ens);
        }

        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <param name="EnName"></param>
        /// <returns></returns>
        private Entity GetEntityByEnName(string EnName)
        {
            Entities _HisEns = null;
            Entity _HisEn = null;
            if (EnName != null)
            {
                _HisEns = BP.En.ClassFactory.GetEns(EnName.Replace("#", ""));
                if (_HisEns == null)
                {
                    _HisEn = BP.En.ClassFactory.GetEn(EnName.Replace("#", ""));
                    if (_HisEn == null)
                        throw new Exception("在此项目中没有找到命名空间及符合的类：" + EnName);
                }
                else
                    _HisEn = _HisEns.GetNewEntity;
            }
            return _HisEn;
        }

        /// <summary>
        /// 将实体类转为json格式 包含列名和数据
        /// </summary>
        /// <param name="ens"></param>
        /// <returns></returns>
        public string TranslateEntitiesToGridJsonColAndData(BP.En.Entities ens)
        {
            Attrs attrs = ens.GetNewEntity.EnMap.Attrs;
            StringBuilder append = new StringBuilder();
            append.Append("{");
            //整理列名
            append.Append("columns:[");
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr || attr.UIVisible == false)
                    continue;

                if (attr.Key == this.RefPK)
                    continue;

                append.Append("{");
                append.Append(string.Format("field:'{0}',title:'{1}',width:{2},sortable:true", attr.Key, attr.Desc, attr.UIWidth * 2));
                append.Append("},");
            }
            if (append.Length > 10)
                append = append.Remove(append.Length - 1, 1);
            append.Append("]");

            //整理数据
            bool bHaveData = false;
            append.Append(",data:[");
            foreach (Entity en in ens)
            {
                bHaveData = true;
                append.Append("{");
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr || attr.UIVisible == false)
                        continue;
                    append.Append(attr.Key + ":\"" + en.GetValStrByKey(attr.Key) + "\",");
                }
                append = append.Remove(append.Length - 1, 1);
                append.Append("},");
            }
            if (append.Length > 11 && bHaveData)
                append = append.Remove(append.Length - 1, 1);
            append.Append("]");
            append.Append("}");
            return append.ToString();
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
            if (root.No.Equals(rootNo))
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
    }
}