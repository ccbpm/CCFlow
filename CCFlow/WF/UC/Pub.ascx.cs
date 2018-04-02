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
using BP.WF;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.UC
{
    public partial class Pub : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        public void BindWorkDtl(Node nd, Works wks)
        {
            Attrs attrs = wks.GetNewEntity.EnMap.Attrs;
            this.AddTable("width='100%' align=center class=Table");
            this.AddTR();
            this.AddTDTitle("序");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                this.AddTDTitle(attr.Desc);
            }
            this.AddTDTitle("报告");
            this.AddTDTitle("轨迹");
            this.AddTREnd();

            int idx = 0;
            bool is1 = false;
            foreach (Entity en in wks)
            {
                idx++;
                is1 = this.AddTR(is1);
                this.AddTD(idx);
                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;
                    switch (attr.MyDataType)
                    {
                        case DataType.AppBoolean:
                            this.AddTD(en.GetValBoolStrByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                        case DataType.AppInt:
                        case DataType.AppDouble:
                            this.AddTD(en.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.AddTDMoney(en.GetValDecimalByKey(attr.Key));
                            break;
                        default:
                            this.AddTD(en.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.AddTD("<a href=\"./../WF/WFRpt.htm?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >报告</a>");
                this.AddTD("<a href=\"./../WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >轨迹</a>");
                this.AddTREnd();
            }

            this.AddTRSum();
            this.AddTD("");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppDouble:
                        this.AddTD("align=right class=TD", wks.GetSumDecimalByKey(attr.Key));
                        break;
                    case DataType.AppMoney:
                        this.AddTD("align=right class=TD", wks.GetSumDecimalByKey(attr.Key).ToString("0.00"));
                        break;
                    default:
                        this.AddTD();
                        break;
                }
            }
            this.AddTD();
            this.AddTD();
            this.AddTREnd();
            this.AddTableEnd();
        }
       
        protected void AddContral(string desc, string text)
        {
            this.Add("<td  class='FDesc' nowrap> " + desc + "</td>");
            this.Add("<td width='40%' class=TD>");
            if (text == "")
                text = "&nbsp;";
            this.Add(text);
            this.AddTDEnd();
        }
        /// <summary>
        /// 增加一个工作
        /// </summary>
        /// <param name="en"></param>
        /// <param name="rws"></param>
        /// <param name="fws"></param>
        /// <param name="nodeId"></param>
        public void ADDWork(Work en, ReturnWorks rws,  int nodeId)
        {
            this.BindViewEn(en, "width=90%");

            foreach (BP.WF.ReturnWork rw in rws)
            {
                if (rw.ReturnToNode != nodeId)
                    continue;

                this.AddBR();
                this.AddMsgOfInfo("退回信息：", rw.BeiZhuHtml);
            }

            //foreach (ShiftWork fw in fws)
            //{
            //    if (fw.FK_Node != nodeId)
            //        continue;

            //    this.AddBR();
            //    this.AddMsgOfInfo("转发信息：", fw.NoteHtml);
            //}


            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStrByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            #region 加入他的明细

            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "WFRptDtl.aspx?RefPK=" + en.PKVal.ToString() + "&EnName=" + enDtl.Ens.GetNewEntity.ToString();
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a  href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  as NUM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  as NUM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入他门的相关功能
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					//int i=DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM "+vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable+" WHERE "+vsM.AttrOfMInMM+"='"+en.PKVal+"'");
            //					refstrs+="[<a href='"+url+"' target='_blank' >"+en1.Name+"</a>]";
            //					//refstrs+="编辑: <a href=\"javascript:window.open(RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+ens.ToString()+"'> )\" > "+en1.Name+"</a>";
            //					//var newWindow= window.open( this.Request.ApplicationPath+'/Comm/'+'RefFuncLink.aspx?RefFuncOID='+OID+'&MainEnsName='+ CurrEnsName +CurrKeys,'chosecol', 'width=100,top=400,left=400,height=50,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
            //					//refstrs+="编辑: <a href=\"javascript:EnsRefFunc('"+en1.OID.ToString()+"')\" > "+en1.Name+"</a>";
            //					//refstrs+="编辑:"+en1.Name+"javascript: EnsRefFunc('"+en1.OID.ToString()+"',)";
            //					//this.AddItem(en1.Name,"EnsRefFunc('"+en1.OID.ToString()+"')",en1.Icon);
            //				}
            //			}
            #endregion

            // 不知道为什么去掉。
            this.Add(refstrs);
        }
    }

}