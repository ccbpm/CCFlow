using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;

namespace CCFlow.WF.Comm.RefFunc
{
    public partial class RefLeft : BP.Web.UC.UCBase3
    {
        #region 属性

        /// <summary>
        /// CCFlow主目录Url
        /// <para></para>
        /// <para>added by liuxc,2014-10-23</para>
        /// </summary>
        public string CCFlowPath
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority,
                                     Request.ApplicationPath.EndsWith("/")
                                         ? Request.ApplicationPath
                                         : Request.ApplicationPath + "/");
            }
        }

        public string PK
        {
            get
            {
                if (ViewState["PK"] == null)
                {
                    string pk = this.Request.QueryString["PK"];
                    if (pk == null)
                        pk = this.Request.QueryString["No"];

                    if (pk == null)
                        pk = this.Request.QueryString["RefNo"];

                    if (pk == null)
                        pk = this.Request.QueryString["OID"];

                    if (pk == null)
                        pk = this.Request.QueryString["MyPK"];


                    if (pk != null)
                    {
                        ViewState["PK"] = pk;
                    }
                    else
                    {
                        Entity mainEn = BP.En.ClassFactory.GetEn(this.EnName);
                        ViewState["PK"] = this.Request.QueryString[mainEn.PK];
                    }
                }
                return ViewState["PK"] as string;
            }
        }

        public string AttrKey
        {
            get
            {
                return this.Request.QueryString["AttrKey"];
            }
        }

        public new string EnName
        {
            get
            {
                string enName = this.Request.QueryString["EnName"];
                string ensName = this.Request.QueryString["EnsName"];
                if (enName == null && ensName == null)
                    throw new Exception("@缺少参数");

                if (enName == null)
                    enName = this.ViewState["EnName"] as string;

                if (enName == null)
                {
                    Entities ens = ClassFactory.GetEns(this.EnsName);
                    this.ViewState["EnName"] = ens.GetNewEntity.ToString();
                    enName = this.ViewState["EnName"].ToString();
                }
                return enName;
            }
        }

        public new string EnsName
        {
            get
            {
                string enName = this.Request.QueryString["EnName"];
                string ensName = this.Request.QueryString["EnsName"];
                if (enName == null && ensName == null)
                    throw new Exception("@缺少参数");


                if (ensName == null)
                    ensName = this.ViewState["EnsName"] as string;
                if (ensName == null)
                {
                    Entity en = ClassFactory.GetEn(this.EnName);
                    this.ViewState["EnsName"] = en.GetNewEntities.ToString();
                    ensName = this.ViewState["EnsName"].ToString();
                }
                return ensName;
            }
        }

        /// <summary>
        /// 功能个数
        /// </summary>
        public int ItemCount { get; set; }
        #endregion

        #region Private Property,added by liuxc,2014-10-23

        /// <summary>
        /// 结点属性左侧功能菜单第一项的默认图标
        /// </summary>
        private const string IconFirstDefault = "WF/Img/Home.gif";

        /// <summary>
        /// 结点属性左侧功能菜单多对多的默认图标
        /// </summary>
        private const string IconM2MDefault = "WF/Img/M2M.png";

        /// <summary>
        /// 结点属性左侧功能菜单明细的默认图标
        /// </summary>
        private const string IconDtlDefault = "WF/Img/Btn/Dtl.gif";

        /// <summary>
        /// 是否显示结点属性左侧功能菜单默认图标
        /// </summary>
        private const bool ShowIconDefault = true;
        #endregion

        #region 左侧功能菜单项对象，added by liuxc,2016-3-7
        /// <summary>
        /// 左侧功能菜单项对象
        /// </summary>
        private class LeftMenuItem
        {
            /// <summary>
            /// 左侧功能菜单项对象
            /// </summary>
            /// <param name="ccflowPath">CCFlow的根Url路径，如http://192.168.1.1/</param>
            /// <param name="text">文本</param>
            /// <param name="url">链接URL</param>
            /// <param name="iconImg">图标的路径</param>
            /// <param name="isSelection">是否处于选中状态</param>
            /// <param name="tooltip">鼠标移动到功能项上的鼠标显示信息</param>
            public LeftMenuItem(string ccflowPath, string text, string url, string iconImg, bool isSelection, string tooltip = null)
            {
                CCFlowPath = ccflowPath;
                Text = text;
                Url = url;
                IconImg = iconImg;
                IsSelection = isSelection;
            }

            /// <summary>
            /// CCFlow的根Url路径，如http://192.168.1.1/
            /// </summary>
            public string CCFlowPath { get; set; }
            /// <summary>
            /// 文本
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// 链接URL
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 图标的路径
            /// </summary>
            public string IconImg { get; set; }
            /// <summary>
            /// 是否处于选中状态
            /// </summary>
            public bool IsSelection { get; set; }
            /// <summary>
            /// 鼠标移动到功能项上的鼠标显示信息
            /// </summary>
            public string ToolTip { get; set; }

            /// <summary>
            /// 获取增加li的字符串
            /// </summary>
            public string LiString
            {
                get
                {
                    return string.Format("<div{4}><a href=\"{0}\"{5}>{3}<span class='nav'>{1}</span></a></div>{2}", Url,
                                         Text,
                                         Environment.NewLine, GetIcon(IconImg),
                                         IsSelection ? " style='font-weight:bold'" : "",
                                         string.IsNullOrWhiteSpace(ToolTip) ? "" : (" title='" + ToolTip + "'"));
                }
            }

            /// <summary>
            /// 获取结点属性左侧功能菜单项默认前置图标
            /// <para></para>
            /// <para>根据本页中设置的ShowIconDefault与IconXXXDefault来生成</para>
            /// </summary>
            /// <param name="imgPath">图标的相对路径，空则为默认明细的图标</param>
            /// <returns></returns>
            private string GetIcon(string imgPath)
            {
                if (ShowIconDefault == false)
                    return string.Empty;

                return "<img src='" + CCFlowPath + (string.IsNullOrWhiteSpace(imgPath) ? IconDtlDefault : imgPath).Replace("//", "/").TrimStart('/') + "' width='16' border='0' />";
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Entity en = BP.En.ClassFactory.GetEn(this.EnName);
            if (this.PK == null)
                return;

            if (en == null)
                throw new Exception(this.EnsName + " " + this.EnName);

            if (en.EnMap.AttrsOfOneVSM.Count + en.EnMap.Dtls.Count + en.EnMap.HisRefMethods.Count == 0)
                return;

            en.PKVal = this.PK;
            string keys = "&" + en.PK + "=" + this.PK + "&r=" + DateTime.Now.ToString("MMddhhmmss");

            string titleKey = "";

            if (en.EnMap.Attrs.Contains("Name"))
                titleKey = "Name";
            else if (en.EnMap.Attrs.Contains("Title"))
                titleKey = "Title";
            string desc = en.EnDesc;
            if (titleKey != "")
            {
                en.RetrieveFromDBSources();
                desc = en.GetValStrByKey(titleKey);
                if (desc.Length > 30)
                    desc = en.EnDesc;
            }

            //edited by liuxc,2016-3-3,修改左侧功能列表导航的表现形式，RefMethod中增加分组的概念，2016-3-7修改完成
            Dictionary<string, List<LeftMenuItem>> dictDefs = new Dictionary<string, List<LeftMenuItem>>();

            //AddUL("class='navlist'");
            //AddLi(
            //    string.Format("<div><a href='UIEn.aspx?EnName={0}&PK={1}'>{4}<span class='nav'>{2}</span></a></div>{3}", EnName, PK, titleKey == "Title" ? "主页" : desc, Environment.NewLine, GetIcon(IconFirstDefault)));
            AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, (titleKey == "Title" ? "主页" : desc),
                                                                 string.Format("UIEn.aspx?EnName={0}&PK={1}", EnName, PK),
                                                                 IconFirstDefault, false));

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            string sql = "";
            int i = 0;

            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "Dot2Dot.aspx?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    try
                    {
                        sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                        i = DBAccess.RunSQLReturnValInt(sql);
                    }
                    catch
                    {
                        sql = "SELECT COUNT(*) as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal;
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt(sql);
                        }
                        catch
                        {
                            vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                        }
                    }
                    if (i == 0)
                    {
                        if (this.AttrKey == vsM.EnsOfMM.ToString())
                        {
                            //AddLi(string.Format(
                            //    "<div style='font-weight:bold'><a href='{0}'>{3}<span class='nav'>{1}</span></a></div>{2}",
                            //    url, vsM.Desc, Environment.NewLine, GetIcon(IconM2MDefault)));
                            AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc, url, IconM2MDefault, true));
                            ItemCount++;
                        }
                        else
                        {
                            //AddLi(string.Format("<div><a href='{0}'>{3}<span class='nav'>{1}</span></a></div>{2}", url, vsM.Desc, Environment.NewLine, GetIcon(IconM2MDefault)));
                            AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc, url, IconM2MDefault, false));
                            ItemCount++;
                        }
                    }
                    else
                    {
                        if (this.AttrKey == vsM.EnsOfMM.ToString())
                        {
                            //AddLi(string.Format(
                            //    "<div style='font-weight:bold'><a href='{0}'>{4}<span class='nav'>{1}[{2}]</span></a></div>{3}",
                            //    url, vsM.Desc, i, Environment.NewLine, GetIcon(IconM2MDefault)));
                            AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc + "[" + i + "]", url, IconM2MDefault, true));
                            ItemCount++;
                        }
                        else
                        {
                            //AddLi(string.Format("<div><a href='{0}'>{4}<span class='nav'>{1}[{2}]</span></a></div>{3}", url, vsM.Desc, i, Environment.NewLine, GetIcon(IconM2MDefault)));
                            AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, vsM.Desc + "[" + i + "]", url, IconM2MDefault, false));
                            ItemCount++;
                        }
                    }
                }
            }
            #endregion

            #region 加入他门的 方法
            Dictionary<string, List<LeftMenuItem>> dictGrps = new Dictionary<string, List<LeftMenuItem>>();

            RefMethods myreffuncs = en.EnMap.HisRefMethods;
            string path = this.Request.ApplicationPath;
            bool haveGroup = false;
            foreach (RefMethod func in myreffuncs)
            {
                if (func.Visable == false || func.RefAttrKey != null)
                    continue;

                haveGroup = !string.IsNullOrWhiteSpace(func.GroupName);

                if (func.RefMethodType != RefMethodType.Func)
                {
                    string myurl = func.Do(null) as string;
                    int h = func.Height;

                    if (func.RefMethodType == RefMethodType.RightFrameOpen)
                    {
                        //AddLi(string.Format(
                        //        "<div><a href='javascript:void(0)' onclick=\"javascript:OpenUrlInRightFrame(this,'{0}')\" title='{1}'>{4}<span class='nav'>{2}</span></a></div>{3}",
                        //        myurl, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title, "javascript:OpenUrlInRightFrame(this,'" + myurl + "')", func.Icon, false,
                                                            func.ToolTip));
                        ItemCount++;
                        continue;
                    }

                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href='" + myurl + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format("<div><a href='{0}' title='{1}'>{4}<span class='nav'>{2}</span></a></div>{3}", myurl, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title, myurl, func.Icon, false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + myurl + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format("<div><a href=\"javascript:WinOpen('{0}','{1}')\" title='{2}'>{5}<span class='nav'>{3}</span></a></div>{4}", myurl, func.Target, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title,
                                                            "javascript:WinOpen('" + myurl + "', '" + func.Target + "')",
                                                            func.Icon, false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                    continue;
                }

                // string url = path + "/Comm/RefMethod.aspx?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                string url = "../RefMethod.aspx?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + keys;

                //  string urlRefFunc = "RefMethod.aspx?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + keys;
                if (func.Warning == null)
                {
                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format("<div><a href='{0}' title='{1}'>{4}<span class='nav'>{2}</span></a></div>{3}", url, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title, url, func.Icon, false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format("<div><a href=\"javascript:WinOpen('{0}','{1}')\" title='{2}'>{5}<span class='nav'>{3}</span></a></div>{4}", url, func.Target, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title,
                                                            "javascript:WinOpen('" + url + "', '" + func.Target + "')",
                                                            func.Icon, false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                }
                else
                {
                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format(
                        //    "<div><a href=\"javascript: if ( confirm('{0}')){{ window.location.href='{1}' }}\" title='{2}'>{5}<span class='nav'>{3}</span></a></div>{4}",
                        //    func.Warning, url, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title,
                                                            "javascript: if ( confirm('" + func.Warning +
                                                            "')){{ window.location.href='" + url + "' }}", func.Icon,
                                                            false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        //AddLi(string.Format(
                        //    "<div><a href=\"javascript: if ( confirm('{0}')){{ WinOpen('{1}','{2}') }}\" title='{3}'>{6}<span class='nav'>{4}</span></a></div>{5}",
                        //    func.Warning, url, func.Target, func.ToolTip, func.Title, Environment.NewLine, GetIcon(func.Icon)));
                        AddGroupedLeftItem(haveGroup ? dictGrps : dictDefs, haveGroup ? func.GroupName : "默认组",
                                           new LeftMenuItem(CCFlowPath, func.Title,
                                                            "javascript: if ( confirm('" + func.Warning +
                                                            "')){{ WinOpen('" + url + "', '" + func.Target + "') }}",
                                                            func.Icon,
                                                            false,
                                                            func.ToolTip));
                        ItemCount++;
                    }
                }
            }
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            foreach (EnDtl enDtl in enDtls)
            {
                string url = "Dtl.aspx?EnName=" + this.EnName + "&PK=" + this.PK + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;

                try
                {
                    i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                }
                catch
                {
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }
                    catch
                    {
                        enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                    }
                }

                if (i == 0)
                {
                    //this.AddLi("<a href=\"" + url + "\"  >" + enDtl.Desc + "</a>");
                    //AddLi(string.Format("<div><a href='{0}'>{3}<span class='nav'>{1}</span></a></div>{2}", url, enDtl.Desc, Environment.NewLine, GetIcon(IconDtlDefault)));
                    AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, enDtl.Desc, url, IconDtlDefault, false));
                    ItemCount++;
                }
                else
                {
                    //this.AddLi("<a href=\"" + url + "\"   >" + enDtl.Desc + "-" + i + "</a>");
                    //AddLi(string.Format("<div><a href='{0}'>{4}<span class='nav'>{1} [{2}]</span></a></div>{3}", url, enDtl.Desc, i, Environment.NewLine, GetIcon(IconDtlDefault)));
                    AddGroupedLeftItem(dictDefs, "默认组", new LeftMenuItem(CCFlowPath, enDtl.Desc + "[" + i + "]", url, IconDtlDefault, false));
                    ItemCount++;
                }
            }
            #endregion

            //AddULEnd();

            //added by liuxc,2016-3-7
            if(dictGrps.Count == 0)
            {
                AddUL("class='navlist'");

                foreach(LeftMenuItem item in dictDefs["默认组"])
                    AddLi(item.LiString);

                AddULEnd();
            }
            else
            {
                Add("<div class='easyui-accordion' data-options='fit:true'>");

                //增加默认组
                Add("<div title='基本功能' style='overflow:auto;'>");
                AddUL("class='navlist'");

                foreach (LeftMenuItem item in dictDefs["默认组"])
                    AddLi(item.LiString);

                AddULEnd();
                AddDivEnd();

                //增加RefMethod分组
                foreach (KeyValuePair<string, List<LeftMenuItem>> grp in dictGrps)
                {
                    Add("<div title='"+grp.Key+"' style='overflow:auto;'>");
                    AddUL("class='navlist'");

                    foreach (LeftMenuItem item in grp.Value)
                        AddLi(item.LiString);

                    AddULEnd();
                    AddDivEnd();
                }

                AddDivEnd();
            }
        }

        /// <summary>
        /// 增加分组中的项
        /// </summary>
        /// <param name="dictGrps">分组集合</param>
        /// <param name="group">所加项的组名</param>
        /// <param name="item">所加项</param>
        private void AddGroupedLeftItem(Dictionary<string, List<LeftMenuItem>> dictGrps, string group, LeftMenuItem item)
        {
            if (string.IsNullOrWhiteSpace(group))
                group = "默认组";

            if (!dictGrps.ContainsKey(group))
                dictGrps.Add(group, new List<LeftMenuItem>());

            dictGrps[group].Add(item);
        }

        /// <summary>
        /// 获取结点属性左侧功能菜单项默认前置图标
        /// <para></para>
        /// <para>根据本页中设置的ShowIconDefault与IconXXXDefault来生成</para>
        /// </summary>
        /// <param name="imgPath">图标的相对路径，空则为默认明细的图标</param>
        /// <returns></returns>
        private string GetIcon(string imgPath)
        {
            if (ShowIconDefault == false)
                return string.Empty;

            return "<img src='" + CCFlowPath + (string.IsNullOrWhiteSpace(imgPath) ? IconDtlDefault : imgPath).Replace("//", "/").TrimStart('/') + "' width='16' border='0' />";
        }
    }
}