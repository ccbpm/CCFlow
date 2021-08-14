﻿using System;
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
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="frmID"></param>
        /// <param name="workid"></param>
        /// <param name="en"></param>
        /// <param name="path"></param>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        public static StringBuilder GenerHtmlOfFree(MapData mapData, string frmID, Int64 workid, Entity en, string path, string flowNo = null, string FK_Node = null, string basePath = null)
        {
            StringBuilder sb = new System.Text.StringBuilder();

            //字段集合.
            MapAttrs mapAttrs = new MapAttrs(frmID);

            Attrs attrs = en.EnMap.Attrs;

            string appPath = "";
            float wtX = MapData.GenerSpanWeiYi(mapData, 1200);
            //float wtX = 0;
            float x = 0;

            #region 输出竖线与标签 & 超连接 Img.
            FrmLabs labs = mapData.FrmLabs;
            foreach (FrmLab lab in labs)
            {
                System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(lab.FontColor);
                x = lab.X + wtX;
                sb.Append("\t\n<DIV name=u2 style='position:absolute;left:" + x + "px;top:" + lab.Y + "px;text-align:left;' >");
                sb.Append("\t\n<span style='color:" + lab.FontColorHtml + ";font-family: " + lab.FontName + ";font-size: " + lab.FontSize + "px;' >" + lab.TextHtml + "</span>");
                sb.Append("\t\n</DIV>");
            }

            FrmLines lines = new FrmLines();
            lines.Retrieve(FrmLineAttr.FK_MapData, mapData.No, FrmLineAttr.Y1);
            foreach (FrmLine line in lines)
            {
                if (line.X1 == line.X2)
                {
                    /* 一道竖线 */
                    float h = line.Y1 - line.Y2;
                    h = Math.Abs(h);
                    if (line.Y1 < line.Y2)
                    {
                        x = line.X1 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "' name='YLine' style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "' name='YLine'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
                else
                {
                    /* 一道横线 */
                    float w = line.X2 - line.X1;

                    if (line.X1 < line.X2)
                    {
                        x = line.X1 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "' name='line'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "' name='line'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
            }

            FrmLinks links = mapData.FrmLinks;
            foreach (FrmLink link in links)
            {
                string url = link.URLExt;
                if (url.Contains("@"))
                {
                    foreach (MapAttr attr in mapAttrs)
                    {
                        if (url.Contains("@") == false)
                            break;
                        url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    }
                }
                x = link.X + wtX;
                sb.Append("\t\n<DIV name=u2 style='position:absolute;left:" + x + "px;top:" + link.Y + "px;text-align:left;' >");
                sb.Append("\t\n<span style='color:" + link.FontColorHtml + ";font-family: " + link.FontName + ";font-size: " + link.FontSize + "px;' > <a href=\"" + url + "\" target='" + link.Target + "'> " + link.Lab + "</a></span>");
                sb.Append("\t\n</DIV>");
            }

            FrmImgs imgs = mapData.FrmImgs;
            foreach (FrmImg img in imgs)
            {
                float y = img.Y;
                string imgSrc = "";

                #region 图片类型
                if (img.HisImgAppType == ImgAppType.Img)
                {
                    //数据来源为本地.
                    if (img.ImgSrcType == 0)
                    {
                        if (img.ImgPath.Contains(";") == false)
                            imgSrc = img.ImgPath;
                    }

                    //数据来源为指定路径.
                    if (img.ImgSrcType == 1)
                    {
                        //图片路径不为默认值
                        imgSrc = img.ImgURL;
                        if (imgSrc.Contains("@"))
                        {
                            /*如果有变量*/
                            imgSrc = BP.WF.Glo.DealExp(imgSrc, en, "");
                        }
                    }

                    x = img.X + wtX;
                    // 由于火狐 不支持onerror 所以 判断图片是否存在
                    imgSrc = "icon.png";

                    sb.Append("\t\n<div id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    if (DataType.IsNullOrEmpty(img.LinkURL) == false)
                        sb.Append("\t\n<a href='" + img.LinkURL + "' target=" + img.LinkTarget + " ><img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.UIWidth + "px;height:" + img.UIHeight + "px;' /></a>");
                    else
                        sb.Append("\t\n<img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.UIWidth + "px;height:" + img.UIHeight + "px;' />");
                    sb.Append("\t\n</div>");
                    continue;
                }
                #endregion 图片类型

                #region 二维码
                if (img.HisImgAppType == ImgAppType.QRCode)
                {
                    x = img.X + wtX;
                    string pk = en.PKVal.ToString();
                    string myPK = frmID + "_" + img.MyPK + "_" + pk;
                    FrmEleDB frmEleDB = new FrmEleDB();
                    frmEleDB.MyPK = myPK;
                    if (frmEleDB.RetrieveFromDBSources() == 0)
                    {
                        //生成二维码
                    }

                    sb.Append("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    sb.Append("\t\n<img src='" + frmEleDB.Tag2 + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.UIWidth + "px;height:" + img.UIHeight + "px;' />");
                    sb.Append("\t\n</DIV>");

                    continue;
                }
                #endregion

                #region 电子签章
                //图片类型
                if (img.HisImgAppType == ImgAppType.Seal)
                {
                    //获取登录人岗位
                    string stationNo = "";
                    //签章对应部门
                    string fk_dept = WebUser.FK_Dept;
                    //部门来源类别
                    string sealType = "0";
                    //签章对应岗位
                    string fk_station = img.Tag0;
                    //表单字段
                    string sealField = "";
                    string sql = "";

                    //重新加载 可能有缓存
                    img.RetrieveFromDBSources();
                    //0.不可以修改，从数据表中取，1可以修改，使用组合获取并保存数据
                    if ((img.IsEdit == 1))
                    {
                        #region 加载签章
                        //如果设置了部门与岗位的集合进行拆分
                        if (!DataType.IsNullOrEmpty(img.Tag0) && img.Tag0.Contains("^") && img.Tag0.Split('^').Length == 4)
                        {
                            fk_dept = img.Tag0.Split('^')[0];
                            fk_station = img.Tag0.Split('^')[1];
                            sealType = img.Tag0.Split('^')[2];
                            sealField = img.Tag0.Split('^')[3];
                            //如果部门没有设定，就获取部门来源
                            if (fk_dept == "all")
                            {
                                //默认当前登陆人
                                fk_dept = WebUser.FK_Dept;
                                //发起人
                                if (sealType == "1")
                                {
                                    sql = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + en.GetValStrByKey("OID");
                                    fk_dept = DBAccess.RunSQLReturnString(sql);
                                }
                                //表单字段
                                if (sealType == "2" && !DataType.IsNullOrEmpty(sealField))
                                {
                                    //判断字段是否存在
                                    foreach (MapAttr attr in mapAttrs)
                                    {
                                        if (attr.KeyOfEn == sealField)
                                        {
                                            fk_dept = en.GetValStrByKey(sealField);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //判断本部门下是否有此人
                        //sql = "SELECT fk_station from port_deptEmpStation where fk_dept='" + fk_dept + "' and fk_emp='" + WebUser.No + "'";
                        sql = string.Format(" select FK_Station from Port_DeptStation where FK_Dept ='{0}' and FK_Station in (select FK_Station from Port_DeptEmpStation where FK_Emp='{1}')", fk_dept, WebUser.No);
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (fk_station.Contains(dr[0].ToString() + ","))
                            {
                                stationNo = dr[0].ToString();
                                break;
                            }
                        }
                        #endregion 加载签章

                        imgSrc = CCFlowAppPath + "DataUser/Seal/" + fk_dept + "_" + stationNo + ".png";
                        //设置主键
                        string myPK = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                        myPK = myPK + "_" + en.GetValStrByKey("OID") + "_" + img.MyPK;

                        FrmEleDB imgDb = new FrmEleDB(myPK);
                      
                        //判断是否存在
                        if (imgDb == null || DataType.IsNullOrEmpty(imgDb.FK_MapData))
                        {
                            imgDb.FK_MapData = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                            imgDb.EleID = en.GetValStrByKey("OID");
                            imgDb.RefPKVal = img.MyPK;
                            imgDb.Tag1 = imgSrc;
                            imgDb.Insert();
                        }

                        //添加控件
                        x = img.X + wtX;
                        sb.Append("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                        sb.Append("\t\n<img src='" + imgSrc + "' onerror=\"javascript:this.src='" + appPath + "DataUser/Seal/Def.png'\" style=\"padding: 0px;margin: 0px;border-width: 0px;width:" + img.UIWidth + "px;height:" + img.UIHeight + "px;\" />");
                        sb.Append("\t\n</DIV>");
                    }
                    else
                    {
                        FrmEleDB realDB = null;
                        FrmEleDB imgDb = new FrmEleDB();
                        QueryObject objQuery = new QueryObject(imgDb);
                        objQuery.AddWhere(FrmEleDBAttr.FK_MapData, img.EnPK);
                        objQuery.addAnd();
                        objQuery.AddWhere(FrmEleDBAttr.EleID, en.GetValStrByKey("OID"));

                        if (objQuery.DoQuery() == 0)
                        {
                            FrmEleDBs imgdbs = new FrmEleDBs();
                            imgdbs.Retrieve(FrmEleDBAttr.EleID, en.GetValStrByKey("OID"));
                            if (imgdbs.Count  > 0)
                            {
                                foreach (FrmEleDB single in imgdbs)
                                {
                                    if (single.FK_MapData.Substring(6, single.FK_MapData.Length - 6).Equals(img.EnPK.Substring(6, img.EnPK.Length - 6)))
                                    {
                                        single.FK_MapData = img.EnPK;
                                        single.MyPK = img.EnPK + "_" + en.GetValStrByKey("OID") + "_" + img.EnPK;
                                        single.RefPKVal = img.EnPK;
                                        //  single.DirectInsert();
                                        //  realDB = single; cut by zhoupeng .没有看明白.
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                realDB = imgDb;
                            }
                        }
                        else
                        {
                            realDB = imgDb;
                        }

                        if (realDB != null)
                        {
                            imgSrc = realDB.Tag1;
                            //如果没有查到记录，控件不显示。说明没有走盖章的一步
                            x = img.X + wtX;
                            sb.Append("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                            sb.Append("\t\n<img src='" + imgSrc + "' onerror='javascript:this.src='" + appPath + "DataUser/ICON/" + SystemConfig.CustomerNo + "/LogBiger.png';' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.UIWidth + "px;height:" + img.UIHeight + "px;' />");
                            sb.Append("\t\n</DIV>");
                        }
                    }
                }
                #endregion
            }


            FrmBtns btns = mapData.FrmBtns;
            foreach (FrmBtn btn in btns)
            {
                x = btn.X + wtX;
                sb.Append("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + btn.Y + "px;text-align:left;' >");
                sb.Append("\t\n<span >");

                string doDoc = BP.WF.Glo.DealExp(btn.EventContext, en, null);
                doDoc = doDoc.Replace("~", "'");
                switch (btn.HisBtnEventType)
                {
                    case BtnEventType.Disable:
                        sb.Append("<input type=button class=Btn value='" + btn.Lab.Replace("&nbsp;", " ") + "' disabled='disabled'/>");
                        break;
                    case BtnEventType.RunExe:
                    case BtnEventType.RunJS:
                        sb.Append("<input type=button class=Btn value=\"" + btn.Lab.Replace("&nbsp;", " ") + "\" enable=true onclick=\"" + doDoc + "\" />");
                        break;
                    default:
                        sb.Append("<input type=button value='" + btn.Lab + "' />");
                        break;
                }
                sb.Append("\t\n</span>");
                sb.Append("\t\n</DIV>");
            }
            #endregion 输出竖线与标签

            #region  输出 rb.
            BP.Sys.FrmRBs myrbs = mapData.FrmRBs;
            MapAttr attrRB = new MapAttr();
            foreach (BP.Sys.FrmRB rb in myrbs)
            {
                x = rb.X + wtX;
                sb.Append("<DIV id='F" + rb.MyPK + "' style='position:absolute; left:" + x + "px; top:" + rb.Y + "px; height:16px;text-align: left;word-break: keep-all;' >");
                sb.Append("<span style='word-break: keep-all;font-size:12px;'>");

                if (rb.IntKey == en.GetValIntByKey(rb.KeyOfEn))
                    sb.Append("<b>" + rb.Lab + "</b>");
                else
                    sb.Append(rb.Lab);

                sb.Append("</span>");
                sb.Append("</DIV>");
            }
            #endregion  输出 rb.

            #region 输出数据控件.
            int fSize = 0;
            foreach (MapAttr attr in mapAttrs)
            {
                //处理隐藏字段，如果是不可见并且是启用的就隐藏.
                if (attr.UIVisible == false && attr.UIIsEnable)
                {
                    sb.Append("<input type=text value='" + en.GetValStrByKey(attr.KeyOfEn) + "' style='display:none;' />");
                    continue;
                }

                if (attr.UIVisible == false)
                    continue;

                x = attr.X + wtX;
                if (attr.LGType == FieldTypeS.Enum || attr.LGType == FieldTypeS.FK)
                    sb.Append("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + x + "px; top:" + attr.Y + "px;  height:16px;text-align: left;word-break: keep-all;' >");
                else
                    sb.Append("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + x + "px; top:" + attr.Y + "px; width:" + attr.UIWidth + "px; height:16px;text-align: left;word-break: keep-all;' >");

                sb.Append("<span>");

                #region add contrals.
                if (attr.MaxLen >= 3999 && attr.TBModel == TBModel.RichText)
                {
                    sb.Append(en.GetValStrByKey(attr.KeyOfEn));

                    sb.Append("</span>");
                    sb.Append("</DIV>");
                    continue;
                }

                #region 通过逻辑类型，输出相关的控件.
                string text = "";
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:  // 输出普通类型字段.
                        if (attr.IsSigan == true)
                        {
                            text = en.GetValStrByKey(attr.KeyOfEn);
                            text = SignPic(text);
                            break;
                        }
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
                            text = en.GetValStrByKey(attr.KeyOfEn);
                        }
                        break;
                    case FieldTypeS.Enum:
                        if (attr.UIContralType == UIContralType.CheckBok)
                        {
                            string s = en.GetValStrByKey(attr.KeyOfEn) + ",";
                            SysEnums enums = new SysEnums(attr.UIBindKey);
                            foreach (SysEnum se in enums)
                            {
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
                #endregion 通过逻辑类型，输出相关的控件.

                #endregion add contrals.


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

                sb.Append(text);

                sb.Append("</span>");
                sb.Append("</DIV>");
            }

            

            #endregion 输出数据控件.

            #region 输出明细.
            int dtlsCount = 0;
            MapDtls dtls = new MapDtls(frmID);
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                dtlsCount++;
                x = dtl.X + wtX;
                float y = dtl.Y;

                sb.Append("<DIV id='Fd" + dtl.No + "' style='position:absolute; left:" + x + "px; top:" + y + "px; width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;' >");
                sb.Append("<span>");

                MapAttrs attrsOfDtls = new MapAttrs(dtl.No);

                sb.Append("<table style='wdith:100%' >");
                sb.Append("<tr>");
                foreach (MapAttr item in attrsOfDtls)
                {
                    if (item.KeyOfEn == "OID")
                        continue;
                    if (item.UIVisible == false)
                        continue;

                    sb.Append("<th class='DtlTh'>" + item.Name + "</th>");
                }
                sb.Append("</tr>");
                //#endregion 输出标题.


                //#region 输出数据.
                GEDtls gedtls = new GEDtls(dtl.No);
                gedtls.Retrieve(GEDtlAttr.RefPK, workid, "OID");
                foreach (GEDtl gedtl in gedtls)
                {
                    sb.Append("<tr>");

                    foreach (MapAttr item in attrsOfDtls)
                    {
                        if (item.KeyOfEn.Equals("OID") || item.UIVisible == false)
                            continue;

                        if (item.UIContralType == UIContralType.DDL)
                        {
                            sb.Append("<td class='DtlTd'>" + gedtl.GetValRefTextByKey(item.KeyOfEn) + "</td>");
                            continue;
                        }

                        if (item.IsNum)
                        {
                            sb.Append("<td class='DtlTd' style='text-align:right' >" + gedtl.GetValStrByKey(item.KeyOfEn) + "</td>");
                            continue;
                        }

                        sb.Append("<td class='DtlTd'>" + gedtl.GetValStrByKey(item.KeyOfEn) + "</td>");
                    }
                    sb.Append("</tr>");
                }
                //#endregion 输出数据.


                sb.Append("</table>");

                //string src = "";
                //if (dtl.HisEditModel == EditModel.TableModel)
                //{
                //    src = SystemConfig.CCFlowWebPath + "WF/CCForm/Dtl.htm?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1";
                //}
                //else
                //{
                //    src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1";
                //}

                //sb.Append("<iframe ID='F" + dtl.No + "' onload= 'F" + dtl.No + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

                sb.Append("</span>");
                sb.Append("</DIV>");
            }
            #endregion 输出明细.

            #region 审核组件
            if (flowNo != null)
            {
                NodeWorkCheck fwc = new NodeWorkCheck(frmID);
                if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Disable)
                {
                    x = fwc.FWC_X + wtX;
                    sb.Append("<DIV id='DIVWC" + fwc.No + "' style='position:absolute; left:" + x + "px; top:" + fwc.FWC_Y + "px; width:" + fwc.FWC_W + "px; height:" + fwc.FWC_H + "px;text-align: left;' >");
                    sb.Append("<table   style='border: 1px outset #C0C0C0;padding: inherit; margin: 0;border-collapse:collapse;width:100%;' >");

                    #region 生成审核信息.
                    if (flowNo != null)
                    {
                        string sql = "SELECT EmpFrom, EmpFromT,RDT,Msg,NDFrom,NDFromT FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);

                        //获得当前待办的人员,把当前审批的人员排除在外,不然就有默认同意的意见可以打印出来.
                        sql = "SELECT FK_Emp, FK_Node FROM WF_GenerWorkerList WHERE IsPass!=1 AND WorkID=" + workid;
                        DataTable dtOfTodo = DBAccess.RunSQLReturnTable(sql);

                        foreach (DataRow dr in dt.Rows)
                        {

                            #region 排除正在审批的人员.
                            string nodeID = dr["NDFrom"].ToString();
                            string empFrom = dr["EmpFrom"].ToString();
                            if (dtOfTodo.Rows.Count != 0)
                            {
                                bool isHave = false;
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
                            #endregion 排除正在审批的人员.

                            sb.Append("<tr>");
                            sb.Append("<td valign=middle style='border-style: solid;padding: 4px;text-align: left;color: #333333;font-size: 12px;border-width: 1px;border-color: #C2D5E3;' >" + dr["NDFromT"] + "</td>");
                            string msg = dr["Msg"].ToString();

                            msg += "<br>";
                            msg += "<br>";
                            msg += "审核人:" + dr["EmpFromT"] + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日期:" + dr["RDT"].ToString();

                            sb.Append("<td colspan=3 valign=middle style='border-style: solid;padding: 4px;text-align: left;color: #333333;font-size: 12px;border-width: 1px;border-color: #C2D5E3;' >" + msg + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                    sb.Append("</table>");
                    #endregion 生成审核信息.
                    sb.Append("</DIV>");
                }
            }
            #endregion 审核组件

            #region 父子流程组件
            if (flowNo != null)
            {
                FrmSubFlow subFlow = new FrmSubFlow(frmID);
                if (subFlow.HisFrmSubFlowSta != FrmSubFlowSta.Disable)
                {
                    x = subFlow.SF_X + wtX;
                    sb.Append("<DIV id='DIVWC" + subFlow.No + "' style='position:absolute; left:" + x + "px; top:" + subFlow.SF_Y + "px; width:" + subFlow.SF_W + "px; height:" + subFlow.SF_H + "px;text-align: left;' >");
                    sb.Append("<span>");

                    string src = appPath + "WF/WorkOpt/SubFlow.htm?s=2";
                    string fwcOnload = "";

                    if (subFlow.HisFrmSubFlowSta == FrmSubFlowSta.Readonly)
                    {
                        src += "&DoType=View";
                    }

                    src += "&r=q";
                    sb.Append("<iframe ID='FSF" + subFlow.No + "' " + fwcOnload + "  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + subFlow.SF_W + "' height='" + subFlow.SF_H + "'   scrolling=auto/></iframe>");

                    sb.Append("</span>");
                    sb.Append("</DIV>");
                }
            }
            #endregion 父子流程组件

            #region 输出附件
            FrmAttachments aths = new FrmAttachments(frmID);
            //FrmAttachmentDBs athDBs = null;
            //if (aths.Count > 0)
            //    athDBs = new FrmAttachmentDBs(frmID, en.PKVal.ToString());

            foreach (FrmAttachment ath in aths)
            {

                if (ath.UploadType == AttachmentUploadType.Single)
                {
                    /* 单个文件 */
                    FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK);
                    FrmAttachmentDB athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, ath.MyPK) as FrmAttachmentDB;
                    x = ath.X + wtX;
                    float y = ath.Y;
                    sb.Append("<DIV id='Fa" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + y + "px; text-align: left;float:left' >");
                    //  sb.Append("<span>");
                    sb.Append("<DIV>");

                    sb.Append("附件没有转化:" + athDB.FileName);


                    sb.Append("</DIV>");
                    sb.Append("</DIV>");
                }

                if (ath.UploadType == AttachmentUploadType.Multi)
                {
                    x = ath.X + wtX;
                    sb.Append("<DIV id='Fd" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + ath.Y + "px; width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;' >");
                    sb.Append("<span>");
                    sb.Append("<ul>");

                    //判断是否有这个目录.
                    if (System.IO.Directory.Exists(path + "\\pdf\\") == false)
                        System.IO.Directory.CreateDirectory(path + "\\pdf\\");

                    //文件加密
                    bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                    FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK);

                    foreach (FrmAttachmentDB item in athDBs)
                    {
                        //获取文件是否加密
                        bool isEncrypt = item.GetParaBoolen("IsEncrypt");
                        if (ath.AthSaveWay == AthSaveWay.FTPServer)
                        {
                            try
                            {
                                string toFile = path + "\\pdf\\" + item.FileName;
                                if (System.IO.File.Exists(toFile) == false)
                                {
                                    //把文件copy到,
                                    ////获取文件是否加密
                                    //bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                                    //bool isEncrypt = item.GetParaBoolen("IsEncrypt");
                                    string file = item.GenerTempFile(ath.AthSaveWay);
                                    string fileTempDecryPath = file;
                                    if (fileEncrypt == true && isEncrypt == true)
                                    {
                                        fileTempDecryPath = file + ".tmp";
                                        BP.Tools.EncHelper.DecryptDES(file, fileTempDecryPath);

                                    }

                                    System.IO.File.Copy(fileTempDecryPath, toFile, true);
                                }

                                sb.Append("<li><a href='" + item.FileName + "'>" + item.FileName + "</a></li>");
                            }
                            catch (Exception ex)
                            {
                                sb.Append("<li>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</li>");
                            }
                        }

                        if (ath.AthSaveWay == AthSaveWay.IISServer)
                        {
                            try
                            {
                                string toFile = path + "\\pdf\\" + item.FileName;
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
                                    System.IO.File.Copy(fileTempDecryPath, path + "\\pdf\\" + item.FileName, true);
                                }
                                sb.Append("<li><a href='" + item.FileName + "'>" + item.FileName + "</a></li>");
                            }
                            catch (Exception ex)
                            {
                                sb.Append("<li>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</li>");
                            }
                        }

                    }
                    sb.Append("</ul>");

                    sb.Append("</span>");
                    sb.Append("</DIV>");
                }
            }
            #endregion 输出附件.

            return sb;
        }

        private static StringBuilder GenerHtmlOfFool(MapData mapData, string frmID, Int64 workid, Entity en, string path, string flowNo = null, string FK_Node = null, string basePath = null, NodeFormType formType = NodeFormType.FoolForm)
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
                    ath.MyPK = gf.CtrlID;
                    if (ath.RetrieveFromDBSources() == 0)
                        continue;
                    if (ath.IsVisable == false)
                        continue;

                    sb.Append(" <tr>");
                    sb.Append("  <th colspan=4><b>" + gf.Lab + "</b></th>");
                    sb.Append(" </tr>");

                    FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK);


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
                        if (System.IO.Directory.Exists(path + "\\pdf\\") == false)
                            System.IO.Directory.CreateDirectory(path + "\\pdf\\");

                        foreach (FrmAttachmentDB item in athDBs)
                        {
                            String fileTo = path + "\\pdf\\" + item.FileName;
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
                    url = url.Replace("@basePath", basePath);
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
                        sql = "SELECT a." + BP.Sys.Glo.UserNo + ", a.SignType FROM Port_Emp a, " + tTable + " b WHERE a." + Glo.UserNo + "=b.EmpFrom AND B.WorkID=" + workid;

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
            int columWidth = (int)width*(100/columNum);

            sb.Append("<table style='wdith:100%' >");
            sb.Append("<tr>");
           
            foreach (MapAttr item in attrsOfDtls)
            {
                if (item.KeyOfEn == "OID")
                    continue;
                if (item.UIVisible == false)
                    continue;
                sb.Append("<th class='DtlTh' style='width:"+ columWidth + "px'>" + item.Name + "</th>");
            }
            sb.Append("</tr>");
            //#endregion 输出标题.

            
            //#region 输出数据.
            GEDtls gedtls = new GEDtls(dtl.No);
            gedtls.Retrieve(GEDtlAttr.RefPK, workid, "OID");
            foreach (GEDtl gedtl in gedtls)
            {
                sb.Append("<tr>");

                foreach (MapAttr item in attrsOfDtls)
                {
                    if (item.KeyOfEn.Equals("OID") || item.UIVisible == false)
                        continue;

                    if (item.UIContralType == UIContralType.DDL)
                    {
                        sb.Append("<td class='DtlTd'>" + gedtl.GetValRefTextByKey(item.KeyOfEn) + "</td>");
                        continue;
                    }

                    if (item.IsNum)
                    {
                        sb.Append("<td class='DtlTd' style='text-align:right;' >" + gedtl.GetValStrByKey(item.KeyOfEn) + "</td>");
                        continue;
                    }

                    sb.Append("<td class='DtlTd' >" + gedtl.GetValStrByKey(item.KeyOfEn) + "</td>");
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
                if (System.IO.Directory.Exists(path + "\\pdf\\") == false)
                    System.IO.Directory.CreateDirectory(path + "\\pdf\\");

                //文件加密
                bool fileEncrypt = SystemConfig.IsEnableAthEncrypt;
                FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK);
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
                            string toFile = path + "\\pdf\\" + item.FileName;
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
                            string toFile = path + "\\pdf\\" + item.FileName;
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
                                System.IO.File.Copy(fileTempDecryPath, path + "\\pdf\\" + item.FileName, true);
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
        /// 树形表单转成PDF.
        /// </summary>
        public static string MakeCCFormToPDF(Node node, Int64 workid, string flowNo, string fileNameFormat, bool urlIsHostUrl, string basePath,string htmlString=null)
        {
            //设置文件名,防止下载错误.
            fileNameFormat = workid.ToString();

            //根据节点信息获取表单方案
            MapData md = new MapData("ND" + node.NodeID);
            string resultMsg = "";
            GenerWorkFlow gwf = null;

            //获取主干流程信息
            if (flowNo != null)
                gwf = new GenerWorkFlow(workid);

            //存放信息地址
            string hostURL = SystemConfig.GetValByKey("HostURL", "");
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + "ND" + node.NodeID + "\\" + workid;
            string frmID = node.NodeFrmID;

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

            if ((int)node.HisFormType == (int)NodeFormType.FoolForm || (int)node.HisFormType == (int)NodeFormType.FreeForm
                || (int)node.HisFormType == (int)NodeFormType.RefOneFrmTree || (int)node.HisFormType == (int)NodeFormType.FoolTruck
                || node.HisFormType == NodeFormType.Develop)
            {
                resultMsg = setPDFPath("ND" + node.NodeID, workid, flowNo, gwf);
                if (resultMsg.IndexOf("err@") != -1)
                    return resultMsg;

                string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + "ND" + node.NodeID + "\\" + workid + "\\index.htm";

                resultMsg = MakeHtmlDocument(frmID, workid, flowNo, fileNameFormat, urlIsHostUrl, path, billUrl, "ND" + node.NodeID, basePath, htmlString);

                if (resultMsg.IndexOf("err@") != -1)
                    return resultMsg;

                ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/index.htm");

                //#region 把所有的文件做成一个zip文件.
                //生成pdf文件
                string pdfPath = path + "\\pdf";

                if (System.IO.Directory.Exists(pdfPath) == false)
                    System.IO.Directory.CreateDirectory(pdfPath);

                string pdfFile = pdfPath + "\\" + fileNameFormat + ".pdf";
                string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
                try
                {
                    Html2Pdf(pdfFileExe, billUrl, pdfFile);
                    if (urlIsHostUrl == false)
                        ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");
                    else
                        ht.Add("pdf", SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");
                }
                catch (Exception ex)
                {
                    throw new Exception("err@html转PDF错误:PDF的路径"+ pdfPath  +"可能抛的异常"+ ex.Message);
                }

                //生成压缩文件
                string zipFile = path + "\\..\\" + fileNameFormat + ".zip";

                System.IO.FileInfo finfo = new FileInfo(zipFile);
                ZipFilePath = finfo.FullName; //文件路径.

                try
                {
                    (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                    ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + "ND" + node.NodeID + "/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".zip");
                }
                catch (Exception ex)
                {
                    ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfFile);
                }

                //把所有的文件做成一个zip文件.

                return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
            }

            if ((int)node.HisFormType == (int)NodeFormType.SheetTree)
            {

                //生成pdf文件
                string pdfPath = path + "\\pdf";
                string pdfTempPath = path + "\\pdfTemp";

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
                    string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + "ND" + node.NodeID + "\\" + workid + "\\" + item.FK_Frm + "index.htm";
                    resultMsg = MakeHtmlDocument(item.FK_Frm, workid, flowNo, fileNameFormat, urlIsHostUrl, path, billUrl, "ND" + node.NodeID, basePath);

                    if (resultMsg.IndexOf("err@") != -1)
                        return resultMsg;

                    ht.Add("htm_" + item.FK_Frm, SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + item.FK_Frm + "index.htm");

                    //#region 把所有的文件做成一个zip文件.
                    if (System.IO.Directory.Exists(pdfTempPath) == false)
                        System.IO.Directory.CreateDirectory(pdfTempPath);

                    fileNameFormat = fileNameFormat.Substring(0, fileNameFormat.Length - 1);
                    string pdfFormFile = pdfTempPath + "\\" + item.FK_Frm + ".pdf";
                    string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
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
                string pdfFile = pdfPath + "\\" + fileNameFormat + ".pdf";
                //开始合并处理
                if (System.IO.Directory.Exists(pdfPath) == false)
                    System.IO.Directory.CreateDirectory(pdfPath);

                MergePDF(pdfTempPath, pdfFile);//合并pdf
                                               //合并完删除文件夹

                System.IO.Directory.Delete(pdfTempPath, true);
                if (urlIsHostUrl == false)
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");
                else
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");

                //生成压缩文件
                string zipFile = path + "\\..\\" + fileNameFormat + ".zip";

                System.IO.FileInfo finfo = new FileInfo(zipFile);
                ZipFilePath = finfo.FullName; //文件路径.

                try
                {
                    (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                    ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + frmID + "/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".zip");
                }
                catch (Exception ex)
                {
                    ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfFile);
                }



                return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
            }

            return "warning@不存在需要打印的表单";

        }

        public static string MakeBillToPDF(string frmId, Int64 workid, string basePath, bool urlIsHostUrl = false,string htmlString=null)
        {
            string resultMsg = "";

            //  获取单据的属性信息
            BP.CCBill.FrmBill bill = new BP.CCBill.FrmBill(frmId);
            string fileNameFormat = null;

            //存放信息地址
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + bill.No + "\\" + workid;

            //处理正确的文件名.
            if (fileNameFormat == null)
                fileNameFormat = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM Frm_GenerBill WHERE WorkID=" + workid, "" + workid.ToString());


            if (DataType.IsNullOrEmpty(fileNameFormat) == true)
                fileNameFormat = workid.ToString();

            fileNameFormat = DataType.PraseStringToFileName(fileNameFormat);

            Hashtable ht = new Hashtable();

            //生成pdf文件
            string pdfPath = path + "\\pdf";


            DataRow dr = null;
            resultMsg = setPDFPath(frmId, workid, null, null);
            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            //获取表单的信息执行打印
            string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + bill.No + "\\" + workid + "\\" + "index.htm";
            resultMsg = MakeHtmlDocument(bill.No, workid, null, fileNameFormat, urlIsHostUrl, path, billUrl, frmId, basePath, htmlString);

            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmId + "/" + workid + "/" + "index.htm");

            //#region 把所有的文件做成一个zip文件.
            if (System.IO.Directory.Exists(pdfPath) == false)
                System.IO.Directory.CreateDirectory(pdfPath);

            fileNameFormat = fileNameFormat.Substring(0, fileNameFormat.Length - 1);
            string pdfFormFile = pdfPath + "\\" + bill.Name + ".pdf";  //生成的路径.
            string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
            try
            {
                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                if (urlIsHostUrl == false)
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "InstancePacketOfData/" + frmId + "/" + workid + "/pdf/" + bill.Name + ".pdf");
                else
                    ht.Add("pdf", SystemConfig.GetValByKey("HostURL", "") + "/DataUser/InstancePacketOfData/" + frmId + "/" + workid + "/pdf/" + bill.Name + ".pdf");
            }
            catch (Exception ex)
            {
                /*有可能是因为文件路径的错误， 用补偿的方法在执行一次, 如果仍然失败，按照异常处理. */
                fileNameFormat = DBAccess.GenerGUID();
                pdfFormFile = pdfPath + "\\" + fileNameFormat + ".pdf";

                Html2Pdf(pdfFileExe, resultMsg, pdfFormFile);
                ht.Add("pdf", SystemConfig.GetValByKey("HostURLOfBS", "") + "/InstancePacketOfData/" + frmId + "/" + workid + "/pdf/" + bill.Name + ".pdf");
            }

            //生成压缩文件
            string zipFile = path + "\\..\\" + fileNameFormat + ".zip";

            System.IO.FileInfo finfo = new FileInfo(zipFile);
            ZipFilePath = finfo.FullName; //文件路径.

            try
            {
                (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                ht.Add("zip", SystemConfig.HostURLOfBS + "/DataUser/InstancePacketOfData/" + frmId + "/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".zip");
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
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + "ND" + node.NodeID + "\\" + workid;

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
            string pdfPath = path + "\\pdf";


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
            string billUrl = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + "ND" + node.NodeID + "\\" + workid + "\\" + frmNode.FK_Frm + "index.htm";
            resultMsg = MakeHtmlDocument(frmNode.FK_Frm, workid, flowNo, fileNameFormat, urlIsHostUrl, path, billUrl, "ND" + node.NodeID, basePath);

            if (resultMsg.IndexOf("err@") != -1)
                return resultMsg;

            // ht.Add("htm", SystemConfig.GetValByKey("HostURLOfBS", "../../DataUser/") + "/InstancePacketOfData/" + "ND" + node.NodeID + "/" + workid + "/" + frmNode.FK_Frm + "index.htm");

            //#region 把所有的文件做成一个zip文件.
            if (System.IO.Directory.Exists(pdfPath) == false)
                System.IO.Directory.CreateDirectory(pdfPath);

            fileNameFormat = fileNameFormat.Substring(0, fileNameFormat.Length - 1);
            string pdfFormFile = pdfPath + "\\" + frmNode.FK_Frm + ".pdf";
            string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
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
                pdfFormFile = pdfPath + "\\" + fileNameFormat + ".pdf";

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
                filelist2.Add(Directorypath + "\\" + temp.Name);
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
            string path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\";
            try
            {

                path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\" + workid;
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                //把模版文件copy过去.
                string templateFilePath = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\";
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
                    if (System.IO.File.Exists(path + "\\" + fl.FullName) == true)
                        System.IO.File.Delete(path + "\\" + fl.FullName);
                    System.IO.File.Copy(fl.FullName, path + "\\" + fl.Name, true);
                }

            }
            catch (Exception ex)
            {
                return "err@读写文件出现权限问题，请联系管理员解决。" + ex.Message;
            }

            string hostURL = SystemConfig.GetValByKey("HostURL", "");
            string billUrl = hostURL + "/DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/index.htm";

            // begin生成二维码.
            string pathQR = path + "\\QR.png"; // key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);
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

        public static string MakeHtmlDocument(string frmID, Int64 workid, string flowNo, string fileNameFormat,
           bool urlIsHostUrl, string path, string indexFile, string nodeID, string basePath,string htmlString=null)
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
                    url = url.Replace("@basePath", basePath);
                    if (url.Contains("http") == false)
                        url = basePath + url;

                    //把URL中的内容转换成流


                    string str = "<iframe style='width:100%;height:auto;' ID='" + mapData.No + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe></div>";
                    string docs1 = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\indexUrl.htm");
                    //docs1 = docs1.Replace("@Docs", DownLoadFielToMemoryStream(url));

                    string url1 = "http://www.baidu.com";
                    StringBuilder sb1 = new StringBuilder();
                    WebClient MyWebClient = new WebClient();
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                    Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                    //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                    string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句





                    docs1 = docs1.Replace("@Width", mapData.FrmW.ToString() + "px");
                    docs1 = docs1.Replace("@Height", mapData.FrmH.ToString() + "px");
                    if (gwf != null)
                        docs1 = docs1.Replace("@Title", gwf.Title);
                    DataType.WriteFile(indexFile, pageHtml);
                    return indexFile;
                }else if(mapData.HisFrmType == FrmType.Develop)
                {
                    string ddocs = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\indexDevelop.htm");

                    //获取附件

                    //获取从表
                    MapDtls dtls = new MapDtls(frmID);
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dtl.IsView == false)
                            continue;
                        string html = GetDtlHtmlByID(dtl, workid, mapData.FrmW);
                        htmlString = htmlString.Replace("@Dtl_Fd" + dtl.No, html);
                    }
                    FrmAttachments aths = new FrmAttachments(frmID);
                    foreach (FrmAttachment ath in aths)
                    {
                        if (ath.IsVisable == false)
                            continue;
                        string html = GetAthHtmlByID(ath, workid,path);
                        htmlString = htmlString.Replace("@Ath_" + ath.MyPK, html);
                    }

                    htmlString = htmlString.Replace("../../DataUser", SystemConfig.HostURLOfBS+ "/DataUser");
                    htmlString = htmlString.Replace("../DataUser", SystemConfig.HostURLOfBS + "/DataUser");
                    ddocs = ddocs.Replace("@Docs", htmlString);

                    ddocs = ddocs.Replace("@Height", mapData.FrmH.ToString() + "px");
                    ddocs = ddocs.Replace("@Title", mapData.Name);
                   
                    DataType.WriteFile(indexFile, ddocs);
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

                    string templateFilePathMy = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\";
                    WaterImageManage wim = new WaterImageManage();
                    wim.DrawWords(templateFilePathMy + "ShuiYin.png", words, float.Parse("0.15"), ImagePosition.Center, path + "\\ShuiYin.png");
                }
               
                #endregion

                //生成 表单的 html.
                StringBuilder sb = new System.Text.StringBuilder();

                #region 替换模版文件..
                //首先判断是否有约定的文件.
                string docs = "";
                string tempFile = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\" + mapData.No + ".htm";
                if (System.IO.File.Exists(tempFile) == false)
                {
                    if (gwf != null)
                    {

                        if (nd.HisFormType == NodeFormType.FreeForm)
                            mapData.HisFrmType = FrmType.FreeFrm;
                        else if (nd.HisFormType == NodeFormType.FoolForm || nd.HisFormType == NodeFormType.FoolTruck)
                            mapData.HisFrmType = FrmType.FoolForm;
                        else if (nd.HisFormType == NodeFormType.SelfForm)
                            mapData.HisFrmType = FrmType.Url;
                    }

                    if (mapData.HisFrmType == FrmType.FoolForm)
                    {
                        docs = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\indexFool.htm");
                        sb = BP.WF.MakeForm2Html.GenerHtmlOfFool(mapData, frmID, workid, en, path, flowNo, nodeID, basePath, nd.HisFormType);
                        docs = docs.Replace("@Width", mapData.FrmW.ToString() + "px");
                    }
                    else if (mapData.HisFrmType == FrmType.FreeFrm)
                    {
                        docs = DataType.ReadTextFile(SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\indexFree.htm");
                        sb = BP.WF.MakeForm2Html.GenerHtmlOfFree(mapData, frmID, workid, en, path, flowNo, nodeID, basePath);
                        docs = docs.Replace("@Width", (mapData.FrmW * 1.5).ToString() + "px");
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
                    String pathInfo = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\EndInfo\\" + flowNo + ".txt";
                    if (System.IO.File.Exists(pathInfo) == false)
                        pathInfo = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\EndInfo\\Default.txt";

                    docs = docs.Replace("@EndInfo", DataType.ReadTextFile(pathInfo));
                }

                //indexFile = SystemConfig.getPathOfDataUser() + "\\InstancePacketOfData\\" + frmID + "\\" + workid + "\\index.htm";
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

                Log.DebugWriteError("@生成PDF错误：" + ex.Message + "  --@pdf=" + pdf + "@htmFile="+htmFile);
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