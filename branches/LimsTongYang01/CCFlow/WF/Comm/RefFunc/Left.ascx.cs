using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Port;
using BP.DA;
using BP.En;
namespace CCFlow.WF.Comm.RefFunc
{
    public partial class Left : BP.Web.UC.UCBase3
    {
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

            AddUL("class='navlist'");
            AddLi(
                string.Format("<div><a href='UIEn.aspx?EnName={0}&PK={1}'><span class='nav'>{2}</span></a></div>{3}", EnName, PK, titleKey == "Title" ? "主页" : desc + " - 主页", Environment.NewLine));

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            string sql = "";
            int i = 0;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    //判断该dot2dot是否显示？
                    Entity enMM = vsM.EnsOfMM.GetNewEntity;
                    enMM.SetValByKey(vsM.AttrOfOneInMM, this.PK);
                    if (enMM.HisUAC.IsView == false)
                        continue;


                    string url = "";

                    if (vsM.Dot2DotModel== Dot2DotModel.Default)
                      url = "Dot2Dot.aspx?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;

                    if (vsM.Dot2DotModel == Dot2DotModel.TreeDept)
                        url = "Dot2DotTreeDeptModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;

                    if (vsM.Dot2DotModel == Dot2DotModel.TreeDept)
                        url = "Dot2DotTreeDeptEmpModel.htm?EnsName=" + en.GetNewEntities.ToString() + "&EnName=" + this.EnName + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;

                    try
                    {
                        sql = "SELECT COUNT(" + vsM.AttrOfOneInMM + ") as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                        i = DBAccess.RunSQLReturnValInt(sql);
                    }
                    catch
                    {
                        sql = "SELECT COUNT(" + vsM.AttrOfOneInMM + ") as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal;
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
                            AddLi(string.Format(
                                "<div style='font-weight:bold'><a href='{0}'><span class='nav'>{1}</span></a></div>{2}",
                                url, vsM.Desc, Environment.NewLine));
                        }
                        else
                        {
                            AddLi(string.Format("<div><a href='{0}'><span class='nav'>{1}</span></a></div>{2}", url, vsM.Desc, Environment.NewLine));
                        }
                    }
                    else
                    {
                        if (this.AttrKey == vsM.EnsOfMM.ToString())
                        {
                            AddLi(string.Format(
                                "<div style='font-weight:bold'><a href='{0}'><span class='nav'>{1} [{2}]</span></a></div>{3}",
                                url, vsM.Desc, i, Environment.NewLine));
                        }
                        else
                        {
                            AddLi(string.Format("<div><a href='{0}'><span class='nav'>{1} [{2}]</span></a></div>{3}", url, vsM.Desc, i, Environment.NewLine));
                        }
                    }
                }
            }
            #endregion

            #region 加入方法
            RefMethods myreffuncs = en.EnMap.HisRefMethods;
            string path = this.Request.ApplicationPath;
            foreach (RefMethod func in myreffuncs)
            {
                if (func.Visable == false || func.RefAttrKey!=null)
                    continue;
                if (func.IsCanBatch == true)
                    continue;

                if (func.RefMethodType != RefMethodType.Func)
                {
                    string myurl = func.Do(null) as string;
                    int h = func.Height;

                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href='" + myurl + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format("<div><a href='{0}' title='{1}'><span class='nav'>{2}</span></a></div>{3}", myurl, func.ToolTip, func.Title, Environment.NewLine));
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + myurl + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format("<div><a href=\"javascript:WinOpen('{0}','{1}')\" title='{2}'><span class='nav'>{3}</span></a></div>{4}", myurl, func.Target, func.ToolTip, func.Title, Environment.NewLine));
                    }
                    continue;
                }

              //  string url = "../RefMethod.aspx?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + keys;
                string url = "../RefMethod.htm?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + keys;

                if (func.Warning == null)
                {
                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format("<div><a href='{0}' title='{1}'><span class='nav'>{2}</span></a></div>{3}", url, func.ToolTip, func.Title, Environment.NewLine));
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format("<div><a href=\"javascript:WinOpen('{0}','{1}')\" title='{2}'><span class='nav'>{3}</span></a></div>{4}", url, func.Target, func.ToolTip, func.Title, Environment.NewLine));
                    }
                }
                else
                {
                    if (func.Target == null)
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format(
                            "<div><a href=\"javascript: if ( confirm('{0}')){{ window.location.href='{1}' }}\" title='{2}'><span class='nav'>{3}</span></a></div>{4}",
                            func.Warning, url, func.ToolTip, func.Title, Environment.NewLine));
                    }
                    else
                    {
                        //this.AddLi(func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>");
                        AddLi(string.Format(
                            "<div><a href=\"javascript: if ( confirm('{0}')){{ WinOpen('{1}','{2}') }}\" title='{3}'><span class='nav'>{4}</span></a></div>{5}",
                            func.Warning, url, func.Target, func.ToolTip, func.Title, Environment.NewLine));
                    }
                }
            }
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            foreach (EnDtl enDtl in enDtls)
            {
                //判断该dtl是否要显示?
                Entity myEnDtl = enDtl.Ens.GetNewEntity; //获取他的en
                myEnDtl.SetValByKey(enDtl.RefKey, this.PK);  //给refpk赋值
                if (myEnDtl.HisUAC.IsView == false)
                    continue;

                string url = "Dtl.aspx?EnName=" + this.EnName + "&PK=" + this.PK + "&EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                try
                {
                    i = DBAccess.RunSQLReturnValInt("SELECT COUNT(" + enDtl.RefKey + ") FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                }
                catch
                {
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(" + enDtl.RefKey + ") FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }
                    catch
                    {
                        enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                    }
                }

                if (i == 0)
                {
                    //this.AddLi("<a href=\"" + url + "\"  >" + enDtl.Desc + "</a>");
                    AddLi(string.Format("<div><a href='{0}'><span class='nav'>{1}</span></a></div>{2}", url, enDtl.Desc, Environment.NewLine));
                }
                else
                {
                    //this.AddLi("<a href=\"" + url + "\"   >" + enDtl.Desc + "-" + i + "</a>");
                    AddLi(string.Format("<div><a href='{0}'><span class='nav'>{1} [{2}]</span></a></div>{3}", url, enDtl.Desc, i, Environment.NewLine));
                }
            }
            #endregion

            AddULEnd();
        }
    }
}