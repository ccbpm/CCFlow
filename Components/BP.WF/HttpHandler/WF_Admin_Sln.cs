﻿using System;
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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Sln : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_Sln(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 绑定流程表单
        /// <summary>
        /// 获取流程所有节点
        /// </summary>
        /// <returns></returns>
        public string BindForm_GenderFlowNode()
        {
            Node nd = new Node(this.FK_Node);

            //规范做法.
            Nodes nds = new Nodes(nd.FK_Flow);
            return nds.ToJson();

        }

        /// <summary>
        /// 获取所有节点，复制表单
        /// </summary>
        /// <returns></returns>
        public string BindForm_GetFlowNodeDropList()
        {
            Nodes nodes = new Nodes();
            nodes.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, FK_Flow, BP.WF.Template.NodeAttr.Step);

            if (nodes.Count == 0)
                return "";

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("<select id = \"copynodesdll\"  multiple = \"multiple\" style = \"border - style:None; width: 100%; Height: 100%; \">");

            foreach (Node node in nodes)
                sBuilder.Append("<option " + (FK_Node == node.NodeID ? "disabled = \"disabled\"" : "") + " value = \"" + node.NodeID + "\" >" + "[" + node.NodeID + "]" + node.Name + "</ option >");

            sBuilder.Append("</select>");

            return sBuilder.ToString();
        }

        /// <summary>
        /// 复制表单到节点
        /// </summary>
        /// <returns></returns>
        public string BindForm_DoCopyFrmToNodes()
        {
            string nodeStr = this.GetRequestVal("NodeStr");//节点string,
            string frmStr = this.GetRequestVal("frmStr");//表单string,

            string[] nodeList = nodeStr.Split(',');
            string[] frmList = frmStr.Split(',');

            foreach (string node in nodeList)
            {
                if (string.IsNullOrWhiteSpace(node))
                    continue;

                int nodeid = int.Parse(node);

                DBAccess.RunSQL("DELETE FROM WF_FRMNODE WHERE FK_NODE=" + nodeid);

                foreach (string frm in frmList)
                {
                    if (string.IsNullOrWhiteSpace(frm))
                        continue;

                    FrmNode fn = new FrmNode();
                    if (!fn.IsExit("mypk", frm + "_" + nodeid + "_" + this.FK_Flow))
                    {
                        fn.FK_Frm = frm;
                        fn.FK_Node = nodeid;
                        fn.FK_Flow = this.FK_Flow;

                        fn.Insert();
                    }
                }
            }

            return "操作成功！";
        }        

        /// <summary>
        /// 保存流程表单
        /// </summary>
        /// <returns></returns>
        public string BindForm_SaveBindForms()
        {
            try
            {
                string formNos = this.context.Request["formNos"];

                FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
                //删除已经删除的。
                foreach (FrmNode fn in fns)
                {
                    if (formNos.Contains("," + fn.FK_Frm + ",") == false)
                    {
                        fn.Delete();
                        continue;
                    }
                }

                // 增加集合中没有的。
                string[] strs = formNos.Split(',');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    if (fns.Contains(FrmNodeAttr.FK_Frm, s))
                        continue;

                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = s;
                    fn.FK_Flow = this.FK_Flow;
                    fn.FK_Node = this.FK_Node;
                    fn.Save();
                }
                return "true";
            }
            catch (Exception ex)
            {
                return "err:保存失败。";
            }
        }

        /// <summary>
        /// 获取表单库所有表单
        /// </summary>
        /// <returns></returns>
        public string BindForm_GenerForms()
        {
            //形成树
            FlowFormTrees appendFormTrees = new FlowFormTrees();
            //节点绑定表单
            FrmNodes frmNodes = new FrmNodes(this.FK_Flow, this.FK_Node);
            //所有表单类别
            SysFormTrees formTrees = new SysFormTrees();
            formTrees.RetrieveAll(SysFormTreeAttr.Idx);

            //根节点
            BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
            root.No = "00";
            root.ParentNo = "0";
            root.Name = "表单库";
            root.NodeType = "root";
            appendFormTrees.AddEntity(root);

            foreach (SysFormTree formTree in formTrees)
            {
                //已经添加排除
                if (appendFormTrees.Contains("No", formTree.No) == true)
                    continue;
                //根节点排除
                if (formTree.ParentNo.Equals("0"))
                {
                    root.No = formTree.No;
                    continue;
                }
                //文件夹
                BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                nodeFolder.No = formTree.No;
                nodeFolder.ParentNo = formTree.ParentNo;
                nodeFolder.Name = formTree.Name;
                nodeFolder.NodeType = "folder";
                if (formTree.ParentNo.Equals("0"))
                    nodeFolder.ParentNo = root.No;
                appendFormTrees.AddEntity(nodeFolder);

                //表单
                MapDatas mapS = new MapDatas();
                mapS.RetrieveByAttr(MapDataAttr.FK_FormTree, formTree.No);
                if (mapS != null && mapS.Count > 0)
                {
                    foreach (MapData map in mapS)
                    {
                        BP.WF.Template.FlowFormTree formFolder = new BP.WF.Template.FlowFormTree();
                        formFolder.No = map.No;
                        formFolder.ParentNo = map.FK_FormTree;
                        formFolder.Name = map.Name + "[" + map.No + "]";
                        formFolder.NodeType = "form";
                        appendFormTrees.AddEntity(formFolder);
                    }
                }
            }

            string strCheckedNos = "";
            //设置选中
            foreach (FrmNode frmNode in frmNodes)
            {
                strCheckedNos += "," + frmNode.FK_Frm + ",";
            }
            //重置
            appendMenus.Clear();
            //生成数据
            TansEntitiesToGenerTree(appendFormTrees, root.No, strCheckedNos);
            return appendMenus.ToString();
        }

        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityMultiTree root = ens.GetEntityByKey(rootNo) as EntityMultiTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");
            appendMenus.Append(",\"state\":\"open\"");

            //attributes
            BP.WF.Template.FlowFormTree formTree = root as BP.WF.Template.FlowFormTree;
            if (formTree != null)
            {
                string url = formTree.Url == null ? "" : formTree.Url;
                url = url.Replace("/", "|");
                appendMenus.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"Url\":\"" + url + "\"}");
            }
            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens, checkIds);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        public void AddChildren(EntityMultiTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityMultiTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");


                //attributes
                BP.WF.Template.FlowFormTree formTree = item as BP.WF.Template.FlowFormTree;
                if (formTree != null)
                {
                    string url = formTree.Url == null ? "" : formTree.Url;
                    string ico = "icon-tree_folder";
                    string treeState = "closed";
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form")
                    {
                        ico = "icon-sheet";
                    }
                    appendMenuSb.Append(",\"state\":\"" + treeState + "\"");
                    appendMenuSb.Append(",iconCls:\"");
                    appendMenuSb.Append(ico);
                    appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
        #endregion

        #region 表单方案.
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string BindFrms_Init()
        {
            //注册这个枚举，防止第一次运行出错.
            BP.Sys.SysEnums ses = new SysEnums("FrmEnableRole");

            string text = "";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            //FrmNodeExt fns = new FrmNodeExt(this.FK_Flow, this.FK_Node);

            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);

            #region 如果没有ndFrm 就增加上.
            bool isHaveNDFrm = false;
            foreach (FrmNode fn in fns)
            {
                if (fn.FK_Frm == "ND" + this.FK_Node)
                {
                    isHaveNDFrm = true;
                    break;
                }
            }

            if (isHaveNDFrm == false)
            {
                FrmNode fn = new FrmNode();
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Frm = "ND" + this.FK_Node;
                fn.FK_Node = this.FK_Node;

                fn.FrmEnableRole = FrmEnableRole.Disable; //就是默认不启用.
                fn.FrmSln = 0;
                //  fn.IsEdit = true;
                fn.IsEnableLoadData = true;
                fn.Insert();
                fns.AddEntity(fn);
            }
            #endregion 如果没有ndFrm 就增加上.

            //组合这个实体才有外键信息.
            FrmNodeExts fnes = new FrmNodeExts();
            foreach (FrmNode fn in fns)
            {
                MapData md = new MapData();
                md.No = fn.FK_Frm;
                if (md.IsExits == false)
                {
                    fn.Delete();  //说明该表单不存在了，就需要把这个删除掉.
                    continue;
                }

                FrmNodeExt myen = new FrmNodeExt(fn.MyPK);
                fnes.AddEntity(myen);
            }

            //把json数据返回过去.
            return fnes.ToJson();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string BindFrms_Delete()
        {
            FrmNodeExt myen = new FrmNodeExt(this.MyPK);
            myen.Delete();
            return "删除成功.";
        }

        public string BindFrms_DoOrder()
        {
            FrmNode myen = new FrmNode(this.MyPK);

            if (this.GetRequestVal("OrderType") == "Up")
                myen.DoUp();
            else
                myen.DoDown();

            return "执行成功...";
        }

        #endregion 表单方案.
    }
}
