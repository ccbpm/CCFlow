using System;
using System.IO;
using System.Drawing;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using BP.Sys;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.Security.Cryptography;
using System.Text;
using BP.Port;
using BP.WF.Rpt;
using BP.WF.Data;
using BP.WF.Template;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BP.WF
{
    public class MakeForm2Html
    {
        private static StringBuilder GenerHtmlOfFool(MapData mapData, string frmID, Int64 workid, Entity en, string path, string flowNo = null, string FK_Node = null, NodeFormType formType = NodeFormType.FoolForm)
        {
            StringBuilder sb = new StringBuilder();
            //字段集合.
            MapAttrs mapAttrs = new MapAttrs(frmID);
            Attrs attrs = null;
            GroupFields gfs = null;
            if (formType == NodeFormType.FoolTruck && DataType.IsNullOrEmpty(FK_Node) == false)
            {
                Node nd = new Node(FK_Node);
                Work wk = nd.HisWork;
                wk.OID = workid;
                wk.RetrieveFromDBSources();

                /* 求出来走过的表单集合 */
                string sql = "SELECT NDFrom FROM ND" + int.Parse(flowNo) + "Track A, WF_Node B ";
                sql += " WHERE A.NDFrom=B.NodeID  ";
                sql += "  AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.Start + "  OR ActionType=" + (int)ActionType.Skip + ")  ";
                sql += "  AND B.FormType=" + (int)NodeFormType.FoolTruck + " "; // 仅仅找累加表单.
                sql += "  AND NDFrom!=" + Int32.Parse(FK_Node.Replace("ND", "")) + " "; //排除当前的表单.


                sql += "  AND (A.WorkID=" + workid + ") ";
                sql += " ORDER BY A.RDT ";

                // 获得已经走过的节点IDs.
                DataTable dtNodeIDs = DBAccess.RunSQLReturnTable(sql);
                string frmIDs = "";
                if (dtNodeIDs.Rows.Count > 0)
                {
                    //把所有的节点字段.
                    foreach (DataRow dr in dtNodeIDs.Rows)
                    {
                        if (frmIDs.Contains("ND" + dr[0].ToString()) == true)
                            continue;
                        frmIDs += "'ND" + dr[0].ToString() + "',";
                    }
                }

                if (frmIDs == "")
                    frmIDs = "'" + mapData.No + "'";
                else
                    frmIDs = frmIDs.Substring(0, frmIDs.Length - 1);

                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                if (gwf.WFState == WFState.Complete)
                    frmIDs = frmIDs + ",'" + FK_Node + "'";
                gfs = new GroupFields();
                gfs.RetrieveIn(GroupFieldAttr.FrmID, "(" + frmIDs + ")");

                mapAttrs = new MapAttrs();
                mapAttrs.RetrieveIn(MapAttrAttr.FK_MapData, "(" + frmIDs + ")", "GroupID, Idx");
            }
            else
            {
                gfs = new GroupFields(frmID);
                attrs = en.EnMap.Attrs;
            }

            //生成表头.
            String frmName = mapData.Name;
            if (SystemConfig.AppSettings["CustomerNo"] == "TianYe")
                frmName = "";

            sb.Append(" <table style='width:950px;height:auto;' >");

            //#region 生成头部信息.
            sb.Append("<tr>");

            sb.Append("<td colspan=4 >");

            sb.Append("<table border=0 style='width:950px;'>");

            sb.Append("<tr  style='border:0px;' >");

            //二维码显示
            bool IsHaveQrcode = true;
            if (SystemConfig.GetValByKeyBoolen("IsShowQrCode", false) == false)
                IsHaveQrcode = false;

            //判断当前文件是否存在图片
            bool IsHaveImg = false;
            String IconPath = path + "/icon.png";
            if (System.IO.File.Exists(IconPath) == true)
                IsHaveImg = true;
            if (IsHaveImg == true)
            {
                sb.Append("<td>");
                sb.Append("<img src='icon.png' style='height:100px;border:0px;' />");
                sb.Append("</td>");
            }
            if (IsHaveImg == false && IsHaveQrcode == false)
                sb.Append("<td  colspan=6>");
            else if ((IsHaveImg == true && IsHaveQrcode == false) || (IsHaveImg == false && IsHaveQrcode == true))
                sb.Append("<td  colspan=5>");
            else
                sb.Append("<td  colspan=4>");

            sb.Append("<br><h2><b>" + frmName + "</b></h2>");
            sb.Append("</td>");

            if (IsHaveQrcode == true)
            {
                sb.Append("<td>");
                sb.Append(" <img src='QR.png' style='height:100px;'  />");
                sb.Append("</td>");
            }

            sb.Append("</tr>");
            sb.Append("</table>");

            sb.Append("</td>");
            //#endregion 生成头部信息.

            if (DataType.IsNullOrEmpty(FK_Node) == false && DataType.IsNullOrEmpty(flowNo) == false)
            {
                Node nd = new Node(Int32.Parse(FK_Node.Replace("ND","")));
                if (frmID.StartsWith("ND")==true && nd.FrmWorkCheckSta != FrmWorkCheckSta.Disable)
                {
                    GroupField gf = gfs.GetEntityByKey(GroupFieldAttr.CtrlType, "FWC") as GroupField;
                    if (gf == null)
                    {
                        gf = new GroupField();
                        gf.OID = 100;
                        gf.FrmID = nd.NodeFrmID;
                        gf.CtrlType = "FWC";
                        gf.CtrlID = "FWCND" + nd.NodeID;
                        gf.Idx = 100;
                        gf.Lab = "审核信息";
                        gfs.AddEntity(gf);
                    }
                }
            }


            foreach (GroupField gf in gfs)
            {
                //输出标题.
                if (gf.CtrlType != "Ath")
                {
                    sb.Append(" <tr>");
                    sb.Append("  <th colspan=4><b>" + gf.Lab + "</b></th>");
                    sb.Append(" </tr>");
                }

                //#region 输出字段.
                if (gf.CtrlID == "" && gf.CtrlType == "")
                {
                    Boolean isDropTR = true;
                    String html = "";
                    foreach (MapAttr attr in mapAttrs)
                    {
                        //处理隐藏字段，如果是不可见并且是启用的就隐藏.
                        if (attr.UIVisible == false)
                            continue;
                        if (attr.GroupID != attr.GroupID)
                            continue;
                        //处理分组数据，非当前分组的数据不输出
                        if (attr.GroupID != gf.OID)
                            continue;

                        string text = "";

                        switch (attr.LGType)
                        {
                            case FieldTypeS.Normal:  // 输出普通类型字段.
                                if (attr.MyDataType == 1 && (int)attr.UIContralType == DataType.AppString)
                                {

                                    if (attrs.Contains(attr.KeyOfEn + "Text") == true)
                                        text = en.GetValRefTextByKey(attr.KeyOfEn);
                                    if (DataType.IsNullOrEmpty(text))
                                        if (attrs.Contains(attr.KeyOfEn + "T") == true)
                                            text = en.GetValStrByKey(attr.KeyOfEn + "T");
                                }
                                else
                                {
                                    //判断是不是图片签名
                                    if (attr.IsSigan == true)
                                    {
                                        String SigantureNO = en.GetValStrByKey(attr.KeyOfEn);
                                        String src = SystemConfig.HostURL + "/DataUser/Siganture/";
                                        text = "<img src='" + src + SigantureNO + ".JPG' title='" + SigantureNO + "' onerror='this.src=\""+src+ "Siganture.JPG\"' style='height:50px;'  alt='图片丢失' /> ";
                                    }
                                    else
                                    {
                                        text = en.GetValStrByKey(attr.KeyOfEn);
                                    }
                                    if (attr.IsRichText == true)
                                    {
                                        text = text.Replace("white-space: nowrap;", "");
                                    }
                                }

                                break;
                            case FieldTypeS.Enum:
                                if(attr.UIContralType == UIContralType.CheckBok)
                                {
                                    string s = en.GetValStrByKey(attr.KeyOfEn)+",";
                                    SysEnums enums = new SysEnums(attr.UIBindKey);
                                    foreach(SysEnum se in enums){
                                        if (s.IndexOf(se.IntKey + ",") != -1)
                                            text += se.Lab + " ";
                                    }

                                } 
                                else
                                    text = en.GetValRefTextByKey(attr.KeyOfEn);
                                break;
                            case FieldTypeS.FK:
                                text = en.GetValRefTextByKey(attr.KeyOfEn);
                                break;
                            default:
                                break;
                        }

                        if (attr.IsBigDoc)
                        {
                            //这几种字体生成 pdf都乱码
                            text = text.Replace("仿宋,", "宋体,");
                            text = text.Replace("仿宋;", "宋体;");
                            text = text.Replace("仿宋\"", "宋体\"");
                            text = text.Replace("黑体,", "宋体,");
                            text = text.Replace("黑体;", "宋体;");
                            text = text.Replace("黑体\"", "宋体\"");
                            text = text.Replace("楷体,", "宋体,");
                            text = text.Replace("楷体;", "宋体;");
                            text = text.Replace("楷体\"", "宋体\"");
                            text = text.Replace("隶书,", "宋体,");
                            text = text.Replace("隶书;", "宋体;");
                            text = text.Replace("隶书\"", "宋体\"");
                        }

                        if (attr.MyDataType == DataType.AppBoolean)
                        {
                            if (DataType.IsNullOrEmpty(text) || text == "0")
                                text = "[&#10005]" + attr.Name;
                            else
                                text = "[&#10004]" + attr.Name;
                        }

                        //线性展示并且colspan=3
                        if (attr.ColSpan == 3 || (attr.ColSpan == 4 && attr.UIHeightInt < 30))
                        {
                            isDropTR = true;
                            html += " <tr>";
                            html += " <td  class='FoolFrmFieldCtrl' style='width:143px' >" + attr.Name + "</td>";
                            html += " <td  ColSpan=3 style='width:712.5px' class='FContext'>";
                            html += text;
                            html += " </td>";
                            html += " </tr>";
                            continue;
                        }

                        //线性展示并且colspan=4
                        if (attr.ColSpan == 4)
                        {
                            isDropTR = true;
                            html += " <tr>";
                            html += " <td ColSpan=4 class='FoolFrmFieldCtrl' >" + attr.Name + "</td>";
                            html += " </tr>";
                            html += " <tr>";
                            html += " <td ColSpan=4 class='FContext'>";
                            html += text;
                            html += " </td>";
                            html += " </tr>";
                            continue;
                        }

                        if (isDropTR == true)
                        {
                            html += " <tr>";
                            html += " <td class='FoolFrmFieldCtrl' style='width:143px'>" + attr.Name + "</td>";
                            html += " <td class='FContext' style='width:332px'>";
                            html += text;
                            html += " </td>";
                            isDropTR = !isDropTR;
                            continue;
                        }

                        if (isDropTR == false)
                        {
                            html += " <td  class='FoolFrmFieldCtrl'style='width:143px'>" + attr.Name + "</td>";
                            html += " <td class='FContext' style='width:332px'>";
                            html += text;
                            html += " </td>";
                            html += " </tr>";
                            isDropTR = !isDropTR;
                            continue;
                        }
                    }
                    sb.Append(html); //增加到里面.
                    continue;
                }
                //#endregion 输出字段.

                //#region 如果是从表.
                if (gf.CtrlType == "Dtl")
                {
                    if (DataType.IsNullOrEmpty(gf.CtrlID) == true)
                        continue;
                    /* 如果是从表 */
                    MapAttrs attrsOfDtls = null;
                    try
                    {
                        attrsOfDtls = new MapAttrs(gf.CtrlID);
                    }
                    catch (Exception ex) { }

                    //#region 输出标题.
                    sb.Append("<tr><td valign=top colspan=4 >");

                    sb.Append("<table style='wdith:100%' >");
                    sb.Append("<tr>");
                    foreach (MapAttr item in attrsOfDtls)
                    {
                        if (item.KeyOfEn == "OID")
                            continue;
                        if (item.UIVisible == false)
                            continue;

                        sb.Append("<th stylle='width:" + item.UIWidthInt + "px;'>" + item.Name + "</th>");
                    }
                    sb.Append("</tr>");
                    //#endregion 输出标题.


                    //#region 输出数据.
                    GEDtls dtls = new GEDtls(gf.CtrlID);
                    dtls.Retrieve(GEDtlAttr.RefPK, workid,"OID");
                    foreach (GEDtl dtl in dtls)
                    {
                        sb.Append("<tr>");

                        foreach (MapAttr item in attrsOfDtls)
                        {

                            if (item.KeyOfEn == "OID" || item.UIVisible == false)
                                continue;
                            string text = "";
                            switch (item.LGType)
                            {
                                case FieldTypeS.Normal:  // 输出普通类型字段.
                                    if (item.MyDataType == 1 && (int)item.UIContralType == DataType.AppString)
                                    {

                                        if (attrs.Contains(item.KeyOfEn + "Text") == true)
                                            text = dtl.GetValRefTextByKey(item.KeyOfEn);
                                        if (DataType.IsNullOrEmpty(text))
                                            if (attrs.Contains(item.KeyOfEn + "T") == true)
                                                text = dtl.GetValStrByKey(item.KeyOfEn + "T");
                                    }
                                    else
                                    {
                                       
                                        text = dtl.GetValStrByKey(item.KeyOfEn);
                                        
                                        if (item.IsRichText == true)
                                        {
                                            text = text.Replace("white-space: nowrap;", "");
                                        }
                                    }

                                    break;
                                case FieldTypeS.Enum:
                                    if (item.UIContralType == UIContralType.CheckBok)
                                    {
                                        string s = en.GetValStrByKey(item.KeyOfEn) + ",";
                                        SysEnums enums = new SysEnums(item.UIBindKey);
                                        foreach (SysEnum se in enums)
                                        {
                                            if (s.IndexOf(se.IntKey + ",") != -1)
                                                text += se.Lab + " ";
                                        }

                                    }
                                    else
                                        text = dtl.GetValRefTextByKey(item.KeyOfEn);
                                    break;
                                case FieldTypeS.FK:
                                    text = dtl.GetValRefTextByKey(item.KeyOfEn);
                                    break;
                                default:
                                    break;
                            }

                            if (item.UIContralType == UIContralType.DDL)
                            {
                                sb.Append("<td>" + text + "</td>");
                                continue;
                            }

                            if (item.IsNum)
                            {
                                sb.Append("<td style='text-align:right' >" + text+ "</td>");
                                continue;
                            }

                            sb.Append("<td>" + text + "</td>");
                        }
                        sb.Append("</tr>");
                    }
                    //#endregion 输出数据.


                    sb.Append("</table>");

                    sb.Append(" </td>");
                    sb.Append(" </tr>");
                }
                //#endregion 如果是从表.

                //#region 如果是附件.
                if (gf.CtrlType == "Ath")
                {
                    if (DataType.IsNullOrEmpty(gf.CtrlID) == true)
                        continue;
                    FrmAttachment ath = new FrmAttachment();
                    ath.setMyPK(gf.CtrlID);
                    if (ath.RetrieveFromDBSources() == 0)
                        continue;
                    if (ath.IsVisable == false)
                        continue;

                    sb.Append(" <tr>");
                    sb.Append("  <th colspan=4><b>" + gf.Lab + "</b></th>");
                    sb.Append(" </tr>");

                    FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK, workid);


                    if (ath.UploadType == AttachmentUploadType.Single)
                    {
                        /* 单个文件 */
                        sb.Append("<tr><td colspan=4>单附件没有转化:" + ath.MyPK + "</td></td>");
                        continue;
                    }

                    if (ath.UploadType == AttachmentUploadType.Multi)
                    {
                        sb.Append("<tr><td valign=top colspan=4 >");
                        sb.Append("<ul>");

                        //判断是否有这个目录.
                        if (System.IO.Directory.Exists(path + "/pdf/") == false)
                            System.IO.Directory.CreateDirectory(path + "/pdf/");

                        foreach (FrmAttachmentDB item in athDBs)
                        {
                            String fileTo = path + "/pdf/" + item.FileName;
                            //加密信息
                            bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                            bool isEncrypt = item.GetParaBoolen("IsEncrypt");
                            //#region 从ftp服务器上下载.
                            if (ath.AthSaveWay == AthSaveWay.FTPServer)
                            {
                                try
                                {
                                    if (System.IO.File.Exists(fileTo) == true)
                                        System.IO.File.Delete(fileTo);//rn "err@删除已经存在的文件错误,请检查iis的权限:" + ex.getMessage();

                                    //把文件copy到,                                  
                                    String file = item.GenerTempFile(ath.AthSaveWay);

                                    String fileTempDecryPath = file;
                                    if (fileEncrypt == true && isEncrypt == true)
                                    {
                                        fileTempDecryPath = file + ".tmp";
                                        BP.Tools.EncHelper.DecryptDES(file, fileTempDecryPath);

                                    }
                                    System.IO.File.Copy(fileTempDecryPath, fileTo, true);

                                    sb.Append("<li><a href='" + SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + FK_Node + "/" + workid + "/" + "pdf/" + item.FileName + "'>" + item.FileName + "</a></li>");
                                }
                                catch (Exception ex)
                                {
                                    sb.Append("<li>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</li>");
                                }
                            }
                            //#endregion 从ftp服务器上下载.


                            //#region 从iis服务器上下载.
                            if (ath.AthSaveWay == AthSaveWay.IISServer)
                            {
                                try
                                {

                                    String fileTempDecryPath = item.FileFullName;
                                    if (fileEncrypt == true && isEncrypt == true)
                                    {
                                        fileTempDecryPath = item.FileFullName + ".tmp";
                                        BP.Tools.EncHelper.DecryptDES(item.FileFullName, fileTempDecryPath);

                                    }

                                    //把文件copy到,
                                    System.IO.File.Copy(fileTempDecryPath, fileTo, true);

                                    sb.Append("<li><a href='" + SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/" + "pdf/" + item.FileName + "'>" + item.FileName + "</a></li>");
                                }
                                catch (Exception ex)
                                {
                                    sb.Append("<li>" + item.FileName + "(<font color=red>文件未从web下载成功{" + ex.Message + "}</font>)</li>");
                                }
                            }

                        }
                        sb.Append("</ul>");
                        sb.Append("</td></tr>");
                    }

                }
                //#endregion 如果是附件.

                //如果是IFrame页面
                if (gf.CtrlType == "Frame" && flowNo != null)
                {
                    if (DataType.IsNullOrEmpty(gf.CtrlID) == true)
                        continue;
                    sb.Append("<tr>");
                    sb.Append("  <td colspan='4' >");

                    //根据GroupID获取对应的
                    MapFrame frame = new MapFrame(gf.CtrlID);
                    //获取URL
                    String url = frame.URL;

                    //替换URL的
                    url = url.Replace("@basePath", SystemConfig.HostURLOfBS);
                    //替换系统参数
                    url = url.Replace("@WebUser.No", WebUser.No);
                    url = url.Replace("@WebUser.Name;", WebUser.Name);
                    url = url.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);
                    url = url.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);

                    //替换参数
                    if (url.IndexOf("?") > 0)
                    {
                        //获取url中的参数
                        url = url.Substring(url.IndexOf('?'));
                        String[] paramss = url.Split('&');
                        foreach (String param in paramss)
                        {
                            if (DataType.IsNullOrEmpty(param) || param.IndexOf("@") == -1)
                                continue;
                            String[] paramArr = param.Split('=');
                            if (paramArr.Length == 2 && paramArr[1].IndexOf('@') == 0)
                            {
                                if (paramArr[1].IndexOf("@WebUser.") == 0)
                                    continue;
                                url = url.Replace(paramArr[1], en.GetValStrByKey(paramArr[1].Substring(1)));
                            }
                        }

                    }
                    sb.Append("<iframe style='width:100%;height:auto;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe></div>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }


                //#region 审核组件
                if (gf.CtrlType == "FWC" && flowNo != null)
                {
                    NodeWorkCheck fwc = new NodeWorkCheck(frmID);

                    String sql = "";
                    DataTable dtTrack = null;
                    Boolean bl = false;
                    try
                    {
                        bl = DBAccess.IsExitsTableCol("Port_Emp", "SignType");
                    }
                    catch (Exception ex)
                    {

                    }
                    if (bl)
                    {
                        String tTable = "ND" + int.Parse(flowNo) + "Track";
                        sql = "SELECT a." + BP.Sys.Base.Glo.UserNo + ", a.SignType FROM Port_Emp a, " + tTable + " b WHERE a." + Glo.UserNo + "=b.EmpFrom AND B.WorkID=" + workid;

                        dtTrack = DBAccess.RunSQLReturnTable(sql);
                        dtTrack.TableName = "SignType";
                        if (dtTrack.Columns.Contains("No") == false)
                            dtTrack.Columns.Add("No");
                        if (dtTrack.Columns.Contains("SignType") == false)
                            dtTrack.Columns.Add("SignType");
                    }

                    String html = ""; // "<table style='width:100%;valign:middle;height:auto;' >";

                    //#region 生成审核信息.
                    sql = "SELECT NDFromT,Msg,RDT,EmpFromT,EmpFrom,NDFrom FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);

                    //获得当前待办的人员,把当前审批的人员排除在外,不然就有默认同意的意见可以打印出来.
                    sql = "SELECT FK_Emp, FK_Node FROM WF_GenerWorkerList WHERE IsPass!=1 AND WorkID=" + workid;
                    DataTable dtOfTodo = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //#region 排除正在审批的人员.
                        string nodeID = dr["NDFrom"].ToString();
                        string empFrom = dr["EmpFrom"].ToString();
                        if (dtOfTodo.Rows.Count != 0)
                        {
                            Boolean isHave = false;
                            foreach (DataRow mydr in dtOfTodo.Rows)
                            {
                                if (mydr["FK_Node"].ToString() != nodeID)
                                    continue;

                                if (mydr["FK_Emp"].ToString() != empFrom)
                                    continue;
                                isHave = true;
                            }

                            if (isHave == true)
                                continue;
                        }
                        //#endregion 排除正在审批的人员.


                        html += "<tr>";
                        html += " <td valign=middle class='FContext'>" + dr["NDFromT"] + "</td>";

                        String msg = dr["Msg"].ToString();

                        msg += "<br>";
                        msg += "<br>";

                        String empStrs = "";
                        if (dtTrack == null)
                        {
                            empStrs = dr["EmpFromT"].ToString();
                        }
                        else
                        {
                            String singType = "0";
                            foreach (DataRow drTrack in dtTrack.Rows)
                            {
                                if (drTrack["No"].ToString() == dr["EmpFrom"].ToString())
                                {
                                    singType = drTrack["SignType"].ToString();
                                    break;
                                }
                            }

                            if (singType == "0" || singType == "2")
                            {
                                empStrs = dr["EmpFromT"].ToString();
                            }


                            if (singType == "1")
                            {
                                String src = SystemConfig.HostURL + "/DataUser/Siganture/";
                                empStrs = "<img src='"+src + dr["EmpFrom"] + ".JPG' title='" + dr["EmpFromT"] + "' style='height:60px;'  alt='图片丢失' /> ";
                            }

                        }
                        msg += "审核人:" + empStrs + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日期:" + dr["RDT"].ToString();

                        html += " <td colspan=3 valign=middle style='font-size:18px'>" + msg + "</td>";
                        html += " </tr>";
                    }
                    //#endregion 生成审核信息.

                    sb.Append(" " + html);
                }
            }

            sb.Append("</table>");
            return sb;
        }

        private static void GenerHtmlOfDevelop(MapData mapData, string frmID, Int64 workid, Entity en, string path,string indexFile, string flowNo = null, string FK_Node = null)
        {
            string htmlString = DataType.ReadTextFile(indexFile);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            //用于创建新节点
            HtmlAgilityPack.HtmlNode createnode = doc.DocumentNode.SelectSingleNode("/p");

            //将字符串转换成 HtmlDocument
            doc.LoadHtml(htmlString);
            //字段集合.
            MapAttrs mapAttrs = new MapAttrs(frmID);

            //获取审核组件的信息
            string sql = "SELECT NDFromT,Msg,RDT,EmpFromT,EmpFrom,NDFrom FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            HtmlAgilityPack.HtmlNode node = null;
            foreach (MapAttr attr in mapAttrs)
            {
                //处理隐藏字段，如果是不可见并且是启用的就隐藏.
                if (attr.UIVisible == false)
                    continue;
                string text =  en.GetValStrByKey(attr.KeyOfEn);
                //外键或者外部数据源
                if ((attr.LGType == FieldTypeS.Normal && attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL)
                    || (attr.LGType == FieldTypeS.FK && attr.MyDataType == DataType.AppString))
                {
                    if (mapAttrs.Contains(attr.KeyOfEn + "Text") == true)
                        text = en.GetValRefTextByKey(attr.KeyOfEn);
                    if (DataType.IsNullOrEmpty(text))
                        if (mapAttrs.Contains(attr.KeyOfEn + "T") == true)
                            text = en.GetValStrByKey(attr.KeyOfEn + "T");
                    node = doc.GetElementbyId("DDL_"+attr.KeyOfEn);
                    HtmlAgilityPack.HtmlNode parentNode = node.ParentNode;
                    HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode("<span>"+ text+"</span>");
                    parentNode.AppendChild(newNode);
                    node.Remove();
                    continue;
                }
                //枚举、枚举下拉框
                if (attr.MyDataType == DataType.AppInt && attr.LGType == FieldTypeS.Enum)
                {
                    text = en.GetValStrByKey(attr.KeyOfEn);
                    //如果是下拉框
                    if(attr.UIContralType== UIContralType.DDL)
                    {
                        text = en.GetValRefTextByKey(attr.KeyOfEn+"Text");
                        node = doc.GetElementbyId("DDL_" + attr.KeyOfEn);
                        HtmlAgilityPack.HtmlNode parentNode = node.ParentNode;
                        HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode("<span>" + text + "</span>");
                        parentNode.AppendChild(newNode);
                        node.Remove();
                        continue;
                    }
                    doc.GetElementbyId("RB_" + attr.KeyOfEn+"_"+text).SetAttributeValue("checked", "checked");
                    continue;
                }
                //枚举复选框
                if (attr.MyDataType == DataType.AppString && attr.LGType == FieldTypeS.Enum)
                {
                    text = en.GetValStrByKey(attr.KeyOfEn);
                    string s = en.GetValStrByKey(attr.KeyOfEn) + ",";
                    SysEnums enums = new SysEnums(attr.UIBindKey);
                    foreach (SysEnum se in enums)
                    {
                        if (s.IndexOf(se.IntKey + ",") != -1)
                            doc.GetElementbyId("CB_" + attr.KeyOfEn+"_"+ se.IntKey).SetAttributeValue("checked", "checked");
                        doc.GetElementbyId("CB_" + attr.KeyOfEn + "_" + se.IntKey).SetAttributeValue("disabled", "disabled");
                    }
                   
                    continue;
                }

                if (attr.MyDataType == DataType.AppBoolean)
                {
                    if (DataType.IsNullOrEmpty(text) || text == "0")
                        doc.GetElementbyId("CB_" + attr.KeyOfEn).SetAttributeValue("checked", "");
                    else
                        doc.GetElementbyId("CB_" + attr.KeyOfEn).SetAttributeValue("checked", "checked");

                    doc.GetElementbyId("CB_" + attr.KeyOfEn).SetAttributeValue("disabled", "disabled");
                    continue;

                }
                if(attr.MyDataType == DataType.AppString)
                {
                    //签批字段
                    if(attr.UIContralType == UIContralType.SignCheck)
                    {
                        node = doc.GetElementbyId("TB_" + attr.KeyOfEn);
                        DataTable mydt = GetWorkcheckInfoByNodeIDs(dt, text);
                        if (mydt.Rows.Count == 0)
                        {
                            node.Remove();
                            continue;
                        }
                        string _html = "<div style='min-height:17px;'>";
                        _html += "<table style='width:100%'><tbody>";
                        foreach (DataRow dr in mydt.Rows)
                        {
                            _html += "<tr><td style='border: 1px solid #D6DDE6;'>";
                            _html += "<div style='word-wrap: break-word;line-height:20px;padding:5px;padding-left:50px;'><font color='#999'>" + dr[1].ToString() + "</font></div>";
                            _html += "<div style='text-align:right;padding-right:5px'>" + dr[3].ToString() + "(" + dr[2].ToString() + ")</div>";
                            _html += "</td></tr>";
                        }
                        _html += "</tbody></table></div>";
                        HtmlAgilityPack.HtmlNode parentNode = node.ParentNode;
                        HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode(_html);
                        parentNode.AppendChild(newNode);
                        node.Remove();
                        continue;
                    }
                    //字段附件
                    if (attr.UIContralType == UIContralType.AthShow)
                    {
                        continue;
                    }
                    //签名
                    if (attr.IsSigan == true)
                    {
                        continue;
                    }

                }

                if (attr.IsBigDoc)
                {
                    //这几种字体生成 pdf都乱码
                    text = text.Replace("仿宋,", "宋体,");
                    text = text.Replace("仿宋;", "宋体;");
                    text = text.Replace("仿宋\"", "宋体\"");
                    text = text.Replace("黑体,", "宋体,");
                    text = text.Replace("黑体;", "宋体;");
                    text = text.Replace("黑体\"", "宋体\"");
                    text = text.Replace("楷体,", "宋体,");
                    text = text.Replace("楷体;", "宋体;");
                    text = text.Replace("楷体\"", "宋体\"");
                    text = text.Replace("隶书,", "宋体,");
                    text = text.Replace("隶书;", "宋体;");
                    text = text.Replace("隶书\"", "宋体\"");
                    doc.GetElementbyId("TB_" + attr.KeyOfEn).InnerHtml = text;
                    doc.GetElementbyId("TB_" + attr.KeyOfEn).SetAttributeValue("disabled", "disabled");
                    continue;
                }

                
                //如果是字符串
                doc.GetElementbyId("TB_" + attr.KeyOfEn).SetAttributeValue("value", text); 
                doc.GetElementbyId("TB_" + attr.KeyOfEn).SetAttributeValue("disabled", "disabled");
            }
            //获取从表
            MapDtls dtls = new MapDtls(frmID);
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;
                string html = GetDtlHtmlByID(dtl, workid, mapData.FrmW);
                node = doc.DocumentNode.SelectSingleNode("//img[@data-key='" + dtl.No + "']");
                HtmlAgilityPack.HtmlNode parentNode = node.ParentNode;
                HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode(html);
                parentNode.AppendChild(newNode);
                node.Remove();
            }
            //获取附件
            FrmAttachments aths = new FrmAttachments(frmID);
            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsVisable == false)
                    continue;
                node = doc.DocumentNode.SelectSingleNode("//img[@data-key='" + ath.MyPK+"']");
                string html = GetAthHtmlByID(ath, workid, path);
                HtmlAgilityPack.HtmlNode parentNode = node.ParentNode;
                HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode(html);
                parentNode.AppendChild(newNode);
                node.Remove();
            }
            doc.Save(indexFile, Encoding.UTF8);
            return ;
        }

        private static DataTable GetWorkcheckInfoByNodeIDs(DataTable dt,string nodeId)
        {
            DataTable mydt = dt.Clone();
            if (DataType.IsNullOrEmpty(nodeId) == true)
                return mydt;
            string[] nodeIds = nodeId.Split(',');
            for(int i= 0;i < nodeIds.Length; i++)
            {
                if (DataType.IsNullOrEmpty(nodeIds[i]) == true)
                    continue;
                //获取到值
                DataRow[] rows = dt.Select("NDFrom=" + nodeIds[i]);
                foreach(DataRow dr in rows)
                {
                    DataRow myrow = mydt.NewRow();
                    myrow.ItemArray = dr.ItemArray;
                    mydt.Rows.Add(myrow);

                }
            }
            return mydt;
        }

        private static  string GetDtlHtmlByID(MapDtl dtl,Int64 workid,float width)
        {
            StringBuilder sb = new System.Text.StringBuilder();
            MapAttrs attrsOfDtls = new MapAttrs(dtl.No);
            int columNum = 0;
            foreach (MapAttr item in attrsOfDtls)
            {
                if (item.KeyOfEn == "OID")
                    continue;
                if (item.UIVisible == false)
                    continue;
                columNum++;
            }
            if (columNum == 0)
                return "";
            int columWidth = (int)100/ columNum;

            sb.Append("<table style='width:100%' >");
            sb.Append("<tr>");
           
            foreach (MapAttr item in attrsOfDtls)
            {
                if (item.KeyOfEn == "OID")
                    continue;
                if (item.UIVisible == false)
                    continue;
                sb.Append("<th class='DtlTh' style='width:"+ columWidth + "%'>" + item.Name + "</th>");
            }
            sb.Append("</tr>");
            //#endregion 输出标题.

            
            //#region 输出数据.
            GEDtls gedtls = new GEDtls(dtl.No);
            gedtls.Retrieve(GEDtlAttr.RefPK, workid, "OID");
            foreach (GEDtl gedtl in gedtls)
            {
                sb.Append("<tr>");

                foreach (MapAttr attr in attrsOfDtls)
                {
                    //处理隐藏字段，如果是不可见并且是启用的就隐藏.
                    if (attr.KeyOfEn.Equals("OID") || attr.UIVisible == false)
                        continue;
                  
                    string text = "";

                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:  // 输出普通类型字段.
                            if (attr.MyDataType == 1 && (int)attr.UIContralType == DataType.AppString)
                            {

                                if (attrsOfDtls.Contains(attr.KeyOfEn + "Text") == true)
                                    text = gedtl.GetValRefTextByKey(attr.KeyOfEn);
                                if (DataType.IsNullOrEmpty(text))
                                    if (attrsOfDtls.Contains(attr.KeyOfEn + "T") == true)
                                        text = gedtl.GetValStrByKey(attr.KeyOfEn + "T");
                            }
                            else
                            {
                                //判断是不是图片签名
                                if (attr.IsSigan == true)
                                {
                                    String SigantureNO = gedtl.GetValStrByKey(attr.KeyOfEn);
                                    String src = SystemConfig.HostURL + "/DataUser/Siganture/";
                                    text = "<img src='" + src + SigantureNO + ".JPG' title='" + SigantureNO + "' onerror='this.src=\"" + src + "Siganture.JPG\"' style='height:50px;'  alt='图片丢失' /> ";
                                }
                                else
                                {
                                    text = gedtl.GetValStrByKey(attr.KeyOfEn);
                                }
                                if (attr.IsRichText == true)
                                {
                                    text = text.Replace("white-space: nowrap;", "");
                                }
                            }

                            break;
                        case FieldTypeS.Enum:
                            if (attr.UIContralType == UIContralType.CheckBok)
                            {
                                string s = gedtl.GetValStrByKey(attr.KeyOfEn) + ",";
                                SysEnums enums = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum se in enums)
                                {
                                    if (s.IndexOf(se.IntKey + ",") != -1)
                                        text += se.Lab + " ";
                                }

                            }
                            else
                                text = gedtl.GetValRefTextByKey(attr.KeyOfEn);
                            break;
                        case FieldTypeS.FK:
                            text = gedtl.GetValRefTextByKey(attr.KeyOfEn);
                            break;
                        default:
                            break;
                    }

                    if (attr.IsBigDoc)
                    {
                        //这几种字体生成 pdf都乱码
                        text = text.Replace("仿宋,", "宋体,");
                        text = text.Replace("仿宋;", "宋体;");
                        text = text.Replace("仿宋\"", "宋体\"");
                        text = text.Replace("黑体,", "宋体,");
                        text = text.Replace("黑体;", "宋体;");
                        text = text.Replace("黑体\"", "宋体\"");
                        text = text.Replace("楷体,", "宋体,");
                        text = text.Replace("楷体;", "宋体;");
                        text = text.Replace("楷体\"", "宋体\"");
                        text = text.Replace("隶书,", "宋体,");
                        text = text.Replace("隶书;", "宋体;");
                        text = text.Replace("隶书\"", "宋体\"");
                    }

                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (DataType.IsNullOrEmpty(text) || text == "0")
                            text = "否";
                        else
                            text = "是";
                    }
                    if (attr.IsNum)
                        sb.Append("<td class='DtlTd' style='text-align:right;' >" + text + "</td>");
                    else
                        sb.Append("<td class='DtlTd' >" +text + "</td>");
                }
                 
                sb.Append("</tr>");
            }
            //#endregion 输出数据.


            sb.Append("</table>");

         
            sb.Append("</span>");
            return sb.ToString();
        }

        private static string GetAthHtmlByID(FrmAttachment ath, Int64 workid, string path)
        {
            StringBuilder sb = new System.Text.StringBuilder();

            if (ath.UploadType == AttachmentUploadType.Multi)
            {


                //判断是否有这个目录.
                if (System.IO.Directory.Exists(path + "/pdf/") == false)
                    System.IO.Directory.CreateDirectory(path + "/pdf/");

                //文件加密
                bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK, workid);
                sb.Append("<table id = 'ShowTable' class='table' style='width:100%'>");
                sb.Append("<thead><tr style = 'border:0px;'>");
                sb.Append("<th style='width:50px; border: 1px solid #ddd;padding:8px;background-color:white' nowrap='true'>序</th>");
                sb.Append("<th style = 'min -width:200px; border: 1px solid #ddd;padding:8px;background-color:white' nowrap='true'>文件名</th>");
                sb.Append("<th style = 'width:50px; border: 1px solid #ddd;padding:8px;background-color:white' nowrap='true'>大小KB</th>");
                sb.Append("<th style = 'width:120px; border: 1px solid #ddd;padding:8px;background-color:white' nowrap='true'>上传时间</th>");
                sb.Append("<th style = 'width:80px; border: 1px solid #ddd;padding:8px;background-color:white' nowrap='true'>上传人</th>");
                sb.Append("</thead>");
                sb.Append("<tbody>");
                int idx = 0;
                foreach (FrmAttachmentDB item in athDBs)
                {
                    idx++;
                    sb.Append("<tr>");
                    sb.Append("<td class='Idx'>" + idx + "</td>");
                    //获取文件是否加密
                    bool isEncrypt = item.GetParaBoolen("IsEncrypt");
                    if (ath.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        try
                        {
                            string toFile = path + "/pdf/" + item.FileName;
                            if (System.IO.File.Exists(toFile) == false)
                            {
                                //获取文件是否加密
                                string file = item.GenerTempFile(ath.AthSaveWay);
                                string fileTempDecryPath = file;
                                if (fileEncrypt == true && isEncrypt == true)
                                {
                                    fileTempDecryPath = file + ".tmp";
                                    BP.Tools.EncHelper.DecryptDES(file, fileTempDecryPath);

                                }

                                System.IO.File.Copy(fileTempDecryPath, toFile, true);
                            }

                            sb.Append("<td  title='" + item.FileName + "'>" + item.FileName + "</td>");
                        }
                        catch (Exception ex)
                        {
                            sb.Append("<td>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</td>");
                        }
                    }

                    if (ath.AthSaveWay == AthSaveWay.IISServer)
                    {
                        try
                        {
                            string toFile = path + "/pdf/" + item.FileName;
                            if (System.IO.File.Exists(toFile) == false)
                            {
                                //把文件copy到,
                                string fileTempDecryPath = item.FileFullName;
                                if (fileEncrypt == true && isEncrypt == true)
                                {
                                    fileTempDecryPath = item.FileFullName + ".tmp";
                                    BP.Tools.EncHelper.DecryptDES(item.FileFullName, fileTempDecryPath);

                                }

                                //把文件copy到,
                                System.IO.File.Copy(fileTempDecryPath, path + "/pdf/" + item.FileName, true);
                            }
                            sb.Append("<td>"+item.FileName + "</td>");
                        }
                        catch (Exception ex)
                        {
                            sb.Append("<td>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</td>");
                        }
                    }
                    sb.Append("<td>" + item.FileSize + "KB</td>");
                    sb.Append("<td>" + item.RDT+ "</td>");
                    sb.Append("<td>" + item.RecName + "</td>");
                    sb.Append("</tr>");


                }
                sb.Append("</tbody>");

                sb.Append("</table>");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 单表单，多表单打印PDF
        /// </summary>
        /// <param name="node">节点属性</param>
        /// <param name="workid">流程实例WorkID</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="pdfName">生成PDF的名称</param>
        /// <param name="filePath">生成PDF的路径</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string MakeCCFormToPDF(Node node, Int64 workid, string flowNo, string pdfName, string filePath)
        {
            //根据节点信息获取表单方案
            MapData md = new MapData("ND" + node.NodeID);
            string resultMsg = "";
            GenerWorkFlow gwf = null;

            //获取主干流程信息
            if (flowNo != null)
                gwf = new GenerWorkFlow(workid);

            //存放信息地址
            string hostURL = SystemConfig.GetValByKey("HostURL", "");
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid;
            string frmID = node.NodeFrmID;

            //处理正确的文件名.
            if (DataType.IsNullOrEmpty(pdfName) == true)
            {
                if (DataType.IsNullOrEmpty(flowNo)==false)
                    pdfName = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + workid,  workid.ToString());
                else
                    pdfName = workid.ToString();
            }

            pdfName = DataType.PraseStringToFileName(pdfName);

            Hashtable ht = new Hashtable();
            #region 单表单打印
            if ((int)node.HisFormType == (int)NodeFormType.FoolForm 
                || (int)node.HisFormType == (int)NodeFormType.RefOneFrmTree || (int)node.HisFormType == (int)NodeFormType.FoolTruck
                || node.HisFormType == NodeFormType.Develop)
            {
                resultMsg = setPDFPath("ND" + node.NodeID, workid, flowNo, gwf);
                if (resultMsg.IndexOf("err@") != -1)
                    return resultMsg;

                string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/index.htm";

                resultMsg = MakeHtmlDocument(frmID, workid, flowNo,path, billUrl, "ND" + node.NodeID);

                if (resultMsg.IndexOf("err@") != -1)
                    return resultMsg;

                ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/index.htm");
                //生成pdf文件
                string pdfPath = filePath;
                if(DataType.IsNullOrEmpty(pdfPath)==true)
                    pdfPath = path + "/pdf";

                if (System.IO.Directory.Exists(pdfPath) == false)
                    System.IO.Directory.CreateDirectory(pdfPath);

                string pdfFile = pdfPath + "/" + pdfName + ".pdf";
                string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware/wkhtmltox/wkhtmltopdf.exe";
                try
                {
                    Html2Pdf(pdfFileExe, billUrl, pdfFile);
                    if (DataType.IsNullOrEmpty(filePath) ==true)
                        ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + pdfName + ".pdf");
                    else
                        ht.Add("pdf", pdfPath + "/" + DataType.PraseStringToUrlFileName(pdfName) + ".pdf");
                }
                catch (Exception ex)
                {
                    throw new Exception("err@html转PDF错误:PDF的路径"+ pdfPath  +"可能抛的异常"+ ex.Message);
                }

                //生成压缩文件
                string zipFile = path + "/../" + pdfName + ".zip";

                System.IO.FileInfo finfo = new FileInfo(zipFile);
                ZipFilePath = finfo.FullName; //文件路径.

                try
                {
                    (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                    ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + "ND" + node.NodeID + "/" + pdfName + ".zip");
                }
                catch (Exception ex)
                {
                    ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfFile);
                }

                //把所有的文件做成一个zip文件.

                return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
            }
            #endregion 单表单打印
            #region 多表单合并PDF打印
            if ((int)node.HisFormType == (int)NodeFormType.SheetTree)
            {
                string pdfPath = filePath;
                //生成pdf文件
                //生成pdf文件
                if (DataType.IsNullOrEmpty(pdfPath) == true)
                    pdfPath = path + "/pdf";
                string pdfTempPath = path + "/pdfTemp";

                DataRow dr = null;
                resultMsg = setPDFPath("ND" + node.NodeID, workid, flowNo, gwf);
                if (resultMsg.IndexOf("err@") != -1)
                    return resultMsg;

                //获取绑定的表单
                FrmNodes nds = new FrmNodes(node.FK_Flow, node.NodeID);
                foreach (FrmNode item in nds)
                {
                    //判断当前绑定的表单是否启用
                    if (item.FrmEnableRoleInt == (int)FrmEnableRole.Disable)
                        continue;

                    //判断 who is pk
                    if (flowNo != null && item.WhoIsPK == WhoIsPK.PWorkID) //如果是父子流程
                        workid = gwf.PWorkID;
                    //获取表单的信息执行打印
                    string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + item.FK_Frm + "index.htm";
                    resultMsg = MakeHtmlDocument(item.FK_Frm, workid, flowNo, path, billUrl, "ND" + node.NodeID);

                    if (resultMsg.IndexOf("err@") != -1)
                        return resultMsg;

                    ht.Add("htm_" + item.FK_Frm, SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + item.FK_Frm + "index.htm");

                    //#region 把所有的文件做成一个zip文件.
                    if (System.IO.Directory.Exists(pdfTempPath) == false)
                        System.IO.Directory.CreateDirectory(pdfTempPath);

                    string pdfFormFile = pdfTempPath + "/" + item.FK_Frm + ".pdf";
                    string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware/wkhtmltox/wkhtmltopdf.exe";
                    try
                    {
                        Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);

                    }
                    catch (Exception ex)
                    {
                        /*有可能是因为文件路径的错误， 用补偿的方法在执行一次, 如果仍然失败，按照异常处理. */
                        Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                    }

                }

                //pdf合并
                string pdfFile = pdfPath + "/" + pdfName + ".pdf";
                //开始合并处理
                if (System.IO.Directory.Exists(pdfPath) == false)
                    System.IO.Directory.CreateDirectory(pdfPath);

                MergePDF(pdfTempPath, pdfFile);//合并pdf
                                               //合并完删除文件夹

                System.IO.Directory.Delete(pdfTempPath, true);
                if (DataType.IsNullOrEmpty(filePath) == true)
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmID + "/" + workid + "/pdf/" + pdfName + ".pdf");
                else
                    ht.Add("pdf", pdfPath + "/"+ pdfName + ".pdf");

                //生成压缩文件
                string zipFile = path + "/../" + pdfName + ".zip";

                System.IO.FileInfo finfo = new FileInfo(zipFile);
                ZipFilePath = finfo.FullName; //文件路径.

                try
                {
                    (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                    ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + frmID + "/" + pdfName + ".zip");
                }
                catch (Exception ex)
                {
                    ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfFile);
                }



                return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
            }
            #endregion 多表单合并PDF打印

            return "warning@不存在需要打印的表单";

        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <param name="frmId">表单ID</param>
        /// <param name="workid">数据ID</param>
        /// <param name="filePath">PDF路径</param>
        /// <param name="pdfName">PDF名称</param>
        /// <returns></returns>
        public static string MakeBillToPDF(string frmId, Int64 workid,string filePath, string pdfName)
        {
            string resultMsg = "";

            //  获取单据的属性信息
            BP.CCBill.FrmBill bill = new BP.CCBill.FrmBill(frmId);

            //存放信息地址
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + bill.No + "/" + workid;

            if (DataType.IsNullOrEmpty(pdfName) == true)
                pdfName = workid.ToString();

            pdfName = DataType.PraseStringToFileName(pdfName);

            Hashtable ht = new Hashtable();
            string pdfPath = filePath;
            //生成pdf文件
            if(DataType.IsNullOrEmpty(pdfPath) ==true)
                pdfPath = path + "/pdf";
            DataRow dr = null;
            resultMsg = setPDFPath(frmId, workid, null, null);
            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            //获取表单的信息执行打印
            string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + bill.No + "/" + workid + "/" + "index.htm";
            resultMsg = MakeHtmlDocument(bill.No, workid, null, path, billUrl, frmId);

            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmId + "/" + workid + "/" + "index.htm");

            //#region 把所有的文件做成一个zip文件.
            if (System.IO.Directory.Exists(pdfPath) == false)
                System.IO.Directory.CreateDirectory(pdfPath);

            string pdfFormFile = pdfPath + "/" + pdfName + ".pdf";  //生成的路径.
            string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware/wkhtmltox/wkhtmltopdf.exe";
            try
            {
                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                if (DataType.IsNullOrEmpty(filePath) == true)
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmId + "/" + workid + "/pdf/" + pdfName + ".pdf");
                else
                    ht.Add("pdf", pdfPath +"/" + pdfName + ".pdf");
            }
            catch (Exception ex)
            {

                pdfFormFile = pdfPath + "/" + pdfName + ".pdf";

                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "") + "/InstancePacketOfData/" + frmId + "/" + workid + "/pdf/" + bill.Name + ".pdf");
            }

            //生成压缩文件
            string zipFile = path + "/../" + pdfName + ".zip";

            System.IO.FileInfo finfo = new FileInfo(zipFile);
            ZipFilePath = finfo.FullName; //文件路径.

            try
            {
                (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + frmId + "/" + pdfName + ".zip");
            }
            catch (Exception ex)
            {
                ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfPath);
            }
            return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
        }

        public static string MakeFormToPDF(string frmId, string frmName, Node node, 
            Int64 workid, string flowNo, string fileNameFormat, bool urlIsHostUrl, string basePath)
        {

            string resultMsg = "";
            GenerWorkFlow gwf = null;

            //获取主干流程信息
            if (flowNo != null)
                gwf = new GenerWorkFlow(workid);

            //存放信息地址
            string hostURL = SystemConfig.GetValByKey("HostURL", "");
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid;

            //处理正确的文件名.
            if (fileNameFormat == null)
            {
                if (flowNo != null)
                    fileNameFormat = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + workid, "" + workid.ToString());
                else
                    fileNameFormat = workid.ToString();
            }

            if (DataType.IsNullOrEmpty(fileNameFormat) == true)
                fileNameFormat = workid.ToString();

            fileNameFormat = DataType.PraseStringToFileName(fileNameFormat);

            Hashtable ht = new Hashtable();

            //生成pdf文件
            string pdfPath = path + "/pdf";


            DataRow dr = null;
            resultMsg = setPDFPath("ND" + node.NodeID, workid, flowNo, gwf);
            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            //获取绑定的表单
            FrmNode frmNode = new FrmNode();
            frmNode.Retrieve(FrmNodeAttr.FK_Frm, frmId);

            //判断当前绑定的表单是否启用
            if (frmNode.FrmEnableRoleInt == (int)FrmEnableRole.Disable)
                return "warning@" + frmName + "没有被启用";

            //判断 who is pk
            if (flowNo != null && frmNode.WhoIsPK == WhoIsPK.PWorkID) //如果是父子流程
                workid = gwf.PWorkID;

            //获取表单的信息执行打印
            string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + frmNode.FK_Frm + "index.htm";
            resultMsg = MakeHtmlDocument(frmNode.FK_Frm, workid, flowNo,path, billUrl, "ND" + node.NodeID);

            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            // ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + frmNode.FK_Frm + "index.htm");

            //#region 把所有的文件做成一个zip文件.
            if (System.IO.Directory.Exists(pdfPath) == false)
                System.IO.Directory.CreateDirectory(pdfPath);

            fileNameFormat = fileNameFormat.Substring(0, fileNameFormat.Length - 1);
            string pdfFormFile = pdfPath + "/" + frmNode.FK_Frm + ".pdf";
            string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware/wkhtmltox/wkhtmltopdf.exe";
            try
            {
                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                if (urlIsHostUrl == false)
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + frmNode.FK_Frm + ".pdf");
                else
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + frmNode.FK_Frm + ".pdf");


            }
            catch (Exception ex)
            {
                /*有可能是因为文件路径的错误， 用补偿的方法在执行一次, 如果仍然失败，按照异常处理. */
                fileNameFormat = DBAccess.GenerGUID();
                pdfFormFile = pdfPath + "/" + fileNameFormat + ".pdf";

                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + frmNode.FK_Frm + ".pdf");
            }

            return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);


        }

        /// <summary>
        /// 读取合并的pdf文件名称
        /// </summary>
        /// <param name="Directorypath">目录</param>
        /// <param name="outpath">导出的路径</param>
        public static void MergePDF(string Directorypath, string outpath)
        {
            List<string> filelist2 = new List<string>();
            System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(Directorypath);
            FileInfo[] ff2 = di2.GetFiles("*.pdf");
            BubbleSort(ff2);
            foreach (FileInfo temp in ff2)
            {
                filelist2.Add(Directorypath + "/" + temp.Name);
            }

            PdfReader reader;
            //iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(1403, 991);
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outpath, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < filelist2.Count; i++)
            {
                reader = new PdfReader(filelist2[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }
        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="arr">文件名数组</param>
        public static void BubbleSort(FileInfo[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i; j < arr.Length; j++)
                {
                    if (arr[i].LastWriteTime > arr[j].LastWriteTime)//按创建时间（升序）
                    {
                        FileInfo temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }
            }
        }


        //前期文件的准备
        private static string setPDFPath(string frmID, long workid, string flowNo, GenerWorkFlow gwf)
        {
            //准备目录文件.
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + frmID + "/";
            try
            {

                path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + frmID + "/";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                path = SystemConfig.PathOfDataUser + "InstancePacketOfData/" + frmID + "/" + workid;
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                //把模版文件copy过去.
                string templateFilePath = SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/";
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(templateFilePath);
                System.IO.FileInfo[] finfos = dir.GetFiles();
                if (finfos.Length == 0)
                    return "err@不存在模板文件";
                foreach (System.IO.FileInfo fl in finfos)
                {
                    if (fl.Name.Contains("ShuiYin"))
                        continue;

                    if (fl.Name.Contains("htm"))
                        continue;
                    if (System.IO.File.Exists(path + "/" + fl.FullName) == true)
                        System.IO.File.Delete(path + "/" + fl.FullName);
                    System.IO.File.Copy(fl.FullName, path + "/" + fl.Name, true);
                }

            }
            catch (Exception ex)
            {
                return "err@读写文件出现权限问题，请联系管理员解决。" + ex.Message;
            }

            string hostURL = SystemConfig.GetValByKey("HostURL", "");
            string billUrl = hostURL + "/DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/index.htm";

            // begin生成二维码.
            string pathQR = path + "/QR.png"; // key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);
            if (SystemConfig.GetValByKeyBoolen("IsShowQrCode", false) == true)
            {
                /*说明是图片文件.*/
                string qrUrl = hostURL + "/WF/WorkOpt/PrintDocQRGuide.htm?FrmID=" + frmID + "&WorkID=" + workid + "&FlowNo=" + flowNo;
                if (flowNo != null)
                {
                    gwf = new GenerWorkFlow(workid);
                    qrUrl = hostURL + "/WF/WorkOpt/PrintDocQRGuide.htm?AP=" + frmID + "$" + workid + "_" + flowNo + "_" + gwf.FK_Node + "_" + gwf.Starter + "_" + gwf.FK_Dept;
                }

                //二维码的生成
                ThoughtWorks.QRCode.Codec.QRCodeEncoder qrc = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
                qrc.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
                qrc.QRCodeScale = 4;
                qrc.QRCodeVersion = 7;
                qrc.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
                System.Drawing.Bitmap btm = qrc.Encode(qrUrl, System.Text.Encoding.UTF8);
                btm.Save(pathQR);
                //QrCodeUtil.createQrCode(qrUrl,path,"QR.png");
            }
            //end生成二维码.
            return "";
        }

        private static string DownLoadFielToMemoryStream(string url)
        {
            var wreq = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            System.Net.HttpWebResponse response = wreq.GetResponse() as System.Net.HttpWebResponse;
            MemoryStream ms = null;
            using (var stream = response.GetResponseStream())
            {
                Byte[] buffer = new Byte[response.ContentLength];
                int offset = 0, actuallyRead = 0;
                do
                {
                    actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                    offset += actuallyRead;
                }
                while (actuallyRead > 0);
                ms = new MemoryStream(buffer);
            }
            response.Close();
            return Convert.ToBase64String(ms.ToArray());

        }

        /// <summary>
        /// zip文件路径.
        /// </summary>
        public static string ZipFilePath = "";

        public static string CCFlowAppPath = "/";

        public static string GetHtml(string url)
        {
            string html = String.Empty;
            HttpWebRequest rt = null;
            HttpWebResponse rs = null;
            Stream stream = null;
            StreamReader sr = null;

            try
            {
                rt = (HttpWebRequest)WebRequest.Create(url);
                rs = (HttpWebResponse)rt.GetResponse();
                stream = rs.GetResponseStream();
                sr = new StreamReader(stream, System.Text.Encoding.Default);
                html = sr.ReadToEnd();

            }
            catch (Exception ee)
            {
                new  Exception("发生异常:"+ee.Message);
            }
            finally
            {
                sr.Close();
                stream.Close();
                rs.Close();
            }
            return html;
        }


       
        public static string MakeHtmlDocument(string frmID, Int64 workid, string flowNo,string path, string indexFile, string nodeID)
        {
            try
            {
                GenerWorkFlow gwf = null;
                if (flowNo != null)
                    gwf = new GenerWorkFlow(workid);

                //#region 定义变量做准备.
                //生成表单信息.
                MapData mapData = new MapData(frmID);

                if (mapData.HisFrmType == FrmType.Url)
                {
                    string url = mapData.UrlExt;

                    //替换系统参数
                    url = url.Replace("@WebUser.No", WebUser.No);
                    url = url.Replace("@WebUser.Name;", WebUser.Name);
                    url = url.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);
                    url = url.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);

                    //替换参数
                    if (url.IndexOf("?") > 0)
                    {
                        //获取url中的参数
                        var urlParam = url.Substring(url.IndexOf('?'));
                        string[] paramss = url.Split('&');
                        foreach (string param in paramss)
                        {
                            if (DataType.IsNullOrEmpty(param) || param.IndexOf("@") == -1)
                                continue;
                            string[] paramArr = param.Split('=');
                            if (paramArr.Length == 2 && paramArr[1].IndexOf('@') == 0)
                            {
                                if (paramArr[1].IndexOf("@WebUser.") == 0)
                                    continue;
                                url = url.Replace(paramArr[1], gwf.GetValStrByKey(paramArr[1].Substring(1)));
                            }
                        }

                    }
                    url = url.Replace("@basePath", SystemConfig.HostURLOfBS);
                    if (url.Contains("http") == false)
                        url = SystemConfig.HostURLOfBS + url;

                    string str = "<iframe style='width:100%;height:auto;' ID='" + mapData.No + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe></div>";
                    string docs1 = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/indexUrl.htm");
                    StringBuilder sb1 = new StringBuilder();
                    WebClient MyWebClient = new WebClient();
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据

                    Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                    string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                   
                    docs1 = docs1.Replace("@Width", mapData.FrmW.ToString() + "px");
                    docs1 = docs1.Replace("@Height", mapData.FrmH.ToString() + "px");
                    if (gwf != null)
                        docs1 = docs1.Replace("@Title", gwf.Title);
                    DataType.WriteFile(indexFile, pageHtml);
                    return indexFile;
                }
                else if(mapData.HisFrmType == FrmType.Develop)
                {
                    GEEntity enn = new GEEntity(frmID, workid);
                    string ddocs = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/indexDevelop.htm");
                    string htmlString = DBAccess.GetBigTextFromDB("Sys_MapData", "No", mapData.No, "HtmlTemplateFile");
                   

                    htmlString = htmlString.Replace("../../DataUser", SystemConfig.HostURLOfBS + "/DataUser");
                    htmlString = htmlString.Replace("../DataUser", SystemConfig.HostURLOfBS + "/DataUser");
                    ddocs = ddocs.Replace("@Docs", htmlString);

                    ddocs = ddocs.Replace("@Height", mapData.FrmH.ToString() + "px");
                    ddocs = ddocs.Replace("@Title", mapData.Name);

                    DataType.WriteFile(indexFile, ddocs);
                   GenerHtmlOfDevelop(mapData, mapData.No, workid, enn, path, indexFile, flowNo, nodeID);

                    return indexFile;
                }
                 GEEntity en = new GEEntity(frmID, workid);


                #region 生成水文.

                string rdt = "";
                if (en.EnMap.Attrs.Contains("RDT"))
                {
                    rdt = en.GetValStringByKey("RDT");
                    if (rdt.Length > 10)
                        rdt = rdt.Substring(0, 10);
                }
                //先判断节点中水印的设置
                //判断是否打印水印
                bool isPrintShuiYin = SystemConfig.GetValByKeyBoolen("IsPrintBackgroundWord", false);
                Node nd = null;
                if (gwf != null)
                    nd = new Node(gwf.FK_Node);
                if (isPrintShuiYin == true)
                {
                    string words = "";
                    if (nd.NodeID != 0)
                        words = nd.ShuiYinModle;

                    if (DataType.IsNullOrEmpty(words) == true)
                        words = Glo.PrintBackgroundWord;
                    words = words.Replace("@RDT", rdt);

                    if (words.Contains("@") == true)
                        words = Glo.DealExp(words, en);

                    string templateFilePathMy = SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/";
                    WaterImageManage wim = new WaterImageManage();
                    wim.DrawWords(templateFilePathMy + "ShuiYin.png", words, float.Parse("0.15"), ImagePosition.Center, path + "/ShuiYin.png");
                }
               
                #endregion

                //生成 表单的 html.
                StringBuilder sb = new System.Text.StringBuilder();

                #region 替换模版文件..
                //首先判断是否有约定的文件.
                string docs = "";
                string tempFile = SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/" + mapData.No + ".htm";
                if (System.IO.File.Exists(tempFile) == false)
                {
                    if (gwf != null)
                    {

                        if (nd.HisFormType == NodeFormType.Develop)
                            mapData.HisFrmType = FrmType.Develop;
                        else if (nd.HisFormType == NodeFormType.FoolForm || nd.HisFormType == NodeFormType.FoolTruck)
                            mapData.HisFrmType = FrmType.FoolForm;
                        else if (nd.HisFormType == NodeFormType.SelfForm)
                            mapData.HisFrmType = FrmType.Url;
                    }

                    if (mapData.HisFrmType == FrmType.FoolForm)
                    {
                        docs = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/indexFool.htm");
                        sb = BP.WF.MakeForm2Html.GenerHtmlOfFool(mapData, frmID, workid, en, path, flowNo, nodeID, nd.HisFormType);
                        docs = docs.Replace("@Width", mapData.FrmW.ToString() + "px");
                    }
                     
                }




                docs = docs.Replace("@Docs", sb.ToString());

                docs = docs.Replace("@Height", mapData.FrmH.ToString() + "px");

                String dateFormat = DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                docs = docs.Replace("@PrintDT", dateFormat);

                if (flowNo != null)
                {
                    gwf = new GenerWorkFlow(workid);
                    gwf.WorkID = workid;
                    gwf.RetrieveFromDBSources();

                    docs = docs.Replace("@Title", gwf.Title);

                    if (gwf.WFState == WFState.Runing)
                    {
                        if (SystemConfig.CustomerNo == "TianYe" && gwf.NodeName.Contains("反馈") == true)
                        {
                            nd = new Node(gwf.FK_Node);
                            if (nd.IsEndNode == true)
                            {
                                //让流程自动结束.
                                BP.WF.Dev2Interface.Flow_DoFlowOver(workid, "打印并自动结束", 0);
                            }
                        }
                    }

                    //替换模版尾部的打印说明信息.
                    String pathInfo = SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/EndInfo/" + flowNo + ".txt";
                    if (System.IO.File.Exists(pathInfo) == false)
                        pathInfo = SystemConfig.PathOfDataUser + "InstancePacketOfData/Template/EndInfo/Default.txt";

                    docs = docs.Replace("@EndInfo", DataType.ReadTextFile(pathInfo));
                }

                //indexFile = SystemConfig.getPathOfDataUser() + "/InstancePacketOfData/" + frmID + "/" + workid + "/index.htm";
                DataType.WriteFile(indexFile, docs);

                return indexFile;
            }
            catch (Exception ex)
            {
                return "err@报表生成错误:" + ex.Message;
            }
        }

        public static void Html2Pdf(string pdfFileExe, string htmFile, string pdf)
        {

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            try
            {

                startInfo.FileName = pdfFileExe;//设定需要执行的命令  
                startInfo.Arguments = " --disable-external-links " + htmFile + " " + pdf;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = true; //重定向输出  
                startInfo.CreateNoWindow = true;//不创建窗口  
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                Process p = Process.Start(startInfo);
                p.WaitForExit();
                p.Close();

            }
            catch (Exception ex)
            {
                process.Dispose();

                BP.DA.Log.DebugWriteError("@生成PDF错误：" + ex.Message + "  --@pdf=" + pdf + "@htmFile="+htmFile);
                throw ex;
            }
        }
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        private static string SignPic(string userNo)
        {

            if (string.IsNullOrWhiteSpace(userNo))
            {
                return "";
            }
            //如果文件存在
            String path = SystemConfig.PathOfDataUser + "Siganture/" + userNo + ".jpg";

            if (File.Exists(path) == false)
            {
                path = SystemConfig.PathOfDataUser + "Siganture/" + userNo + ".JPG";
                if (File.Exists(path) == true)
                    return "<img src='" + path + "' style='border:0px;width:100px;height:30px;'/>";
                else
                {
                    Emp emp = new Emp(userNo);
                    return emp.Name;
                }
            }
            else
            {
                return "<img src='" + path + "' style='border:0px;width:100px;height:30px;'/>";
            }

        }

    }

}
#endregion