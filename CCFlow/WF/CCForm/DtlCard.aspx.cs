using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web.Controls;
namespace CCFlow.WF.CCForm
{
    public partial class WF_DtlFrm : BP.Web.WebPage
    {
        public int addRowNum
        {
            get
            {
                try
                {
                    int i = int.Parse(this.Request.QueryString["addRowNum"]);
                    if (this.Request.QueryString["IsCut"] == null)
                        return i;
                    else
                        return i;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.Request.QueryString["RefPKVal"];
                if (str == null)
                    return "1";
                return str;
            }
        }
        public bool IsReadonly
        {
            get
            {
                if (this.Request.QueryString["IsReadonly"] == "1")
                    return true;
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 载入相关文件.
            this.Page.RegisterClientScriptBlock("sguw",
    "<link href='./Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");

            this.Page.RegisterClientScriptBlock("s2g4uh",
     "<script language='JavaScript' src='./Style/Frm/jquery.min.js' ></script>");

            this.Page.RegisterClientScriptBlock("sdfuy24j",
    "<script language='JavaScript' src='./Style/Frm/jquery.idTabs.min.js' ></script>");
            #endregion 载入相关文件.

            #region 查询出来从表.
            MapDtl mdtl = new MapDtl(this.EnsName);
            GEDtls dtls = new GEDtls(this.EnsName);
            QueryObject qo = null;
            try
            {
                qo = new QueryObject(dtls);
                switch (mdtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        break;
                    case DtlOpenType.ForWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        break;
                    case DtlOpenType.ForFID:
                        qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                        break;
                }
                qo.DoQuery();
            }
            catch (Exception ex)
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                throw ex;

                //#region 解决Access 不刷新的问题。
                //string rowUrl = this.Request.RawUrl;
                //if (rowUrl.IndexOf("rowUrl") > 1)
                //{
                //    throw ex;
                //}
                //else
                //{
                //    //this.Response.Redirect(rowUrl + "&rowUrl=1&IsWap=" + this.IsWap, true);
                //    return;
                //}
                //#endregion
            }
            #endregion 查询出来从表.

            #region 初始化空白行
            if (this.IsReadonly == false)
            {
                mdtl.RowsOfList = mdtl.RowsOfList + this.addRowNum;
                int num = dtls.Count;
                if (mdtl.IsInsert)
                {
                    int dtlCount = dtls.Count;
                    for (int i = 0; i < mdtl.RowsOfList - dtlCount; i++)
                    {
                        BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                        dt.ResetDefaultVal();
                        dt.OID = i;
                        dtls.AddEntity(dt);
                    }

                    if (num == mdtl.RowsOfList)
                    {
                        BP.Sys.GEDtl dt1 = new GEDtl(this.EnsName);
                        dt1.ResetDefaultVal();
                        dt1.OID = mdtl.RowsOfList + 1;
                        dtls.AddEntity(dt1);
                    }
                }
            }
            #endregion 初始化空白行

            MapData md = new MapData(mdtl.No);
            this.UCEn1.Clear();

            this.UCEn1.Add("\t\n<div class=\"easyui-tabs\" fit=\"true\" border=\"false\" style='width:" + md.FrmW + "px;height:" + md.FrmH + "px;' data-options=\"tools:'#tab-tools'\">");  //begain.

            #region 输出标签.
            int idx = 0;
            int dtlsNum = dtls.Count;
            
            foreach (GEDtl dtl in dtls)
            {
                idx++;
                this.UCEn1.Add("\t\n<div id=" + idx + " title='第" + idx + "条' style='overflow: auto;'>");
                string src = "";
                src = "FrmDtl.aspx?FK_MapData=" + this.EnsName + "&WorkID=" + this.RefPKVal + "&OID=" + dtl.OID + "&IsReadonly=" + this.IsReadonly;
                this.UCEn1.Add("\t\n<iframe id='IF" + idx + "' Onblur=\"SaveDtlData('" + idx + "');\" frameborder='0' style='width:" + md.FrmW + "px;height:" + md.FrmH + "px;' src=\"" + src + "\"></iframe>");
                this.UCEn1.Add("\t\n</div>");
            }
            this.UCEn1.Add("\t\n </div>");
            if (this.IsReadonly == false && mdtl.IsInsert)
            {
                int addNum = addRowNum + 1;
                int cutNum = addRowNum - 1;

                this.UCEn1.Add("\t\n<div id=\"tab-tools\">");
                if (cutNum >= 0)
                {
                    this.UCEn1.Add("\t\n<a href='DtlCard.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&addRowNum=" + cutNum + "' class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-reload'\">移除</a>");
                    this.UCEn1.Add("\t\n<a href='DtlCard.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&addRowNum=" + addNum + "' class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-reload'\">插入</a>");
                }
                else
                    this.UCEn1.Add("\t\n<a href='DtlCard.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&addRowNum=" + addNum + "' >插入</a>");
                this.UCEn1.Add("\t\n</div>");
            }
            #endregion 输出标签.


            if (this.IsReadonly == false)
            {

            }

            #region 处理iFrom SaveDtlData。
            //string js = "";
            //js = "\t\n<script type='text/javascript' >";
            //js += "\t\n function SaveDtl(dtl) { ";
            //js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData();";
            //js += "\t\n } ";
            //js += "\t\n</script>";
            //this.UCEn1.Add(js);
            #endregion 处理iFrom SaveDtlData。
        }
    }
}