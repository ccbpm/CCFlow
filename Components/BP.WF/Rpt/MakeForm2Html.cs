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
using Microsoft.Win32;
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

namespace BP.WF
{
    public class MakeForm2Html
    {

        public static StringBuilder GenerHtmlOfFree(MapData mapData, string frmID, Int64 workid, Entity en, string path, string flowNo = null)
        {
            StringBuilder sb = new System.Text.StringBuilder();

            //字段集合.
            MapAttrs attrs = new MapAttrs(frmID);

            string appPath = "";
            float wtX = MapData.GenerSpanWeiYi(mapData, 1200);
            //float wtX = 0;
            float x = 0;

            #region 输出Ele
            FrmEles eles = mapData.FrmEles;
            if (eles.Count >= 1)
            {
                FrmEleDBs dbs = new FrmEleDBs(frmID, workid.ToString());
                foreach (FrmEle ele in eles)
                {
                    float y = ele.Y;
                    x = ele.X + wtX;
                    sb.Append("\t\n<DIV id=" + ele.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    switch (ele.EleType)
                    {
                        case FrmEle.HandSiganture:
                            FrmEleDB db = dbs.GetEntityByKey(FrmEleDBAttr.EleID, ele.EleID) as FrmEleDB;
                            string dbFile = appPath + "DataUser/BPPaint/Def.png";
                            if (db != null)
                                dbFile = db.Tag1;

                            //if (this.IsReadonly || ele.IsEnable == false)
                            //{
                            //    sb.Append("\t\n<img src='" + dbFile + "' onerror=\"this.src='" + appPath + "DataUser/BPPaint/Def.png'\" style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            //}
                            //else
                            //{
                            //    string url = appPath + "WF/CCForm/BPPaint.aspx?W=" + ele.HandSiganture_WinOpenW + "&H=" + ele.HandSiganture_WinOpenH + "&MyPK=" + ele.PKVal + "&PKVal=" + en.PKVal;
                            //    myjs = "javascript:BPPaint(this,'" + url + "','" + ele.HandSiganture_WinOpenW + "','" + ele.HandSiganture_WinOpenH + "','" + ele.MyPK + "');";
                            //    //string myjs = "javascript:window.open('" + appPath + "WF/CCForm/BPPaint.aspx?PKVal=" + en.PKVal + "&MyPK=" + ele.MyPK + "&H=" + ele.HandSiganture_WinOpenH + "&W=" + ele.HandSiganture_WinOpenW + "', 'sdf', 'dialogHeight: " + ele.HandSiganture_WinOpenH + "px; dialogWidth: " + ele.HandSiganture_WinOpenW + "px;center: yes; help: no');";
                            //    sb.Append("\t\n<img id='Ele" + ele.MyPK + "' onclick=\"" + myjs + "\" onerror=\"this.src='" + appPath + "DataUser/BPPaint/Def.png'\" src='" + dbFile + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            //}
                            break;
                        case FrmEle.iFrame: //输出框架.
                        //    string paras = this.RequestParas;
                        //    if (paras.Contains("FID=") == false && this.HisEn.Row.ContainsKey("FID"))
                        //    {
                        //        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                        //    }

                        //    if (paras.Contains("WorkID=") == false && this.HisEn.Row.ContainsKey("OID"))
                        //    {
                        //        paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");
                        //    }

                        //    string src = ele.Tag1.Clone() as string; // url 
                        //    if (src.Contains("?"))
                        //        src += "&r=q" + paras;
                        //    else
                        //        src += "?r=q" + paras;

                        //    if (src.Contains("UserNo") == false)
                        //        src += "&UserNo=" + WebUser.No;
                        //    if (src.Contains("SID") == false)
                        //        src += "&SID=" + WebUser.SID;
                        //    if (src.Contains("@"))
                        //    {
                        //        foreach (Attr m in ge.EnMap.Attrs)
                        //        {
                        //            if (src.Contains("@") == false)
                        //                break;
                        //            src = src.Replace("@" + m.Key, en.GetValStrByKey(m.Key));
                        //        }
                        //    }

                        //    if (this.IsReadonly == true)
                        //    {
                        //        sb.Append("<iframe ID='F" + ele.EleID + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ele.W + "' height='" + ele.H + "' scrolling=auto /></iframe>");
                        //    }
                        //    else
                        //    {
                        //        AddLoadFunction(ele.EleID, "blur", "SaveDtl");
                        //        sb.Append("<iframe ID='F" + ele.EleID + "' onload= '" + ele.EleID + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + ele.W + "px; height:" + ele.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                        //    }
                        //    break;
                        //case FrmEle.EleSiganture:
                        //    sb.Append("未处理");
                        //    break;
                        //case FrmEle.SubThread: //子线程.
                        //    paras = this.RequestParas;
                        //    if (paras.Contains("FID=") == false && this.HisEn.Row.ContainsKey("FID"))
                        //    {
                        //        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                        //    }

                        //    if (paras.Contains("WorkID=") == false && this.HisEn.Row.ContainsKey("OID"))
                        //    {
                        //        paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");
                        //    }

                        //    src = "/WF/WorkOpt/ThreadDtl.aspx?1=2" + paras;
                        //    if (src.Contains("UserNo") == false)
                        //        src += "&UserNo=" + WebUser.No;
                        //    if (src.Contains("SID") == false)
                        //        src += "&SID=" + WebUser.SID;
                        //    if (src.Contains("@"))
                        //    {
                        //        foreach (Attr m in en.EnMap.Attrs)
                        //        {
                        //            if (src.Contains("@") == false)
                        //                break;
                        //            src = src.Replace("@" + m.Key, en.GetValStrByKey(m.Key));
                        //        }
                        //    }
                        //    sb.Append("<iframe ID='F" + ele.EleID + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ele.W + "' height='" + ele.H + "' scrolling=auto /></iframe>");
                        //    break;
                        default:
                            break;
                    }
                    sb.Append("\t\n</DIV>");
                }
            }
            #endregion 输出Ele

            #region 输出竖线与标签 & 超连接 Img.
            FrmLabs labs = mapData.FrmLabs;
            foreach (FrmLab lab in labs)
            {
                Color col = ColorTranslator.FromHtml(lab.FontColor);
                x = lab.X + wtX;
                sb.Append("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + lab.Y + "px;text-align:left;' >");
                sb.Append("\t\n<span style='color:" + lab.FontColorHtml + ";font-family: " + lab.FontName + ";font-size: " + lab.FontSize + "px;' >" + lab.TextHtml + "</span>");
                sb.Append("\t\n</DIV>");
            }

            FrmLines lines = mapData.FrmLines;
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
                        sb.Append("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
                else
                {
                    /* 一道横线 */
                    float w = line.X2 - line.X1;

                    if (line.X1 < line.X2)
                    {
                        x = line.X1 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        sb.Append("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
            }

            FrmLinks links = mapData.FrmLinks;
            foreach (FrmLink link in links)
            {
                string url = link.URL;
                if (url.Contains("@"))
                {
                    foreach (MapAttr attr in attrs)
                    {
                        if (url.Contains("@") == false)
                            break;
                        url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    }
                }
                x = link.X + wtX;
                sb.Append("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + link.Y + "px;text-align:left;' >");
                sb.Append("\t\n<span style='color:" + link.FontColorHtml + ";font-family: " + link.FontName + ";font-size: " + link.FontSize + "px;' > <a href=\"" + url + "\" target='" + link.Target + "'> " + link.Text + "</a></span>");
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
                    if (string.IsNullOrEmpty(img.LinkURL) == false)
                        sb.Append("\t\n<a href='" + img.LinkURL + "' target=" + img.LinkTarget + " ><img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' /></a>");
                    else
                        sb.Append("\t\n<img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
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
                    sb.Append("\t\n<img src='" + frmEleDB.Tag2 + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
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
                        if (!string.IsNullOrEmpty(img.Tag0) && img.Tag0.Contains("^") && img.Tag0.Split('^').Length == 4)
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
                                    fk_dept = BP.DA.DBAccess.RunSQLReturnString(sql);
                                }
                                //表单字段
                                if (sealType == "2" && !string.IsNullOrEmpty(sealField))
                                {
                                    //判断字段是否存在
                                    foreach (MapAttr attr in attrs)
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
                        sql = string.Format(" select FK_Station from Port_DeptStation where FK_Dept ='{0}' and FK_Station in (select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Emp='{1}')", fk_dept, WebUser.No);
                        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
                        string myPK = string.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                        myPK = myPK + "_" + en.GetValStrByKey("OID") + "_" + img.MyPK;

                        FrmEleDB imgDb = new FrmEleDB();
                        QueryObject queryInfo = new QueryObject(imgDb);
                        queryInfo.AddWhere(FrmEleAttr.MyPK, myPK);
                        queryInfo.DoQuery();
                        //判断是否存在
                        if (imgDb == null || string.IsNullOrEmpty(imgDb.FK_MapData))
                        {
                            imgDb.FK_MapData = string.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                            imgDb.EleID = en.GetValStrByKey("OID");
                            imgDb.RefPKVal = img.MyPK;
                            imgDb.Tag1 = imgSrc;
                            imgDb.Insert();
                        }

                        //添加控件
                        x = img.X + wtX;
                        sb.Append("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                        sb.Append("\t\n<img src='" + imgSrc + "' onerror=\"javascript:this.src='" + appPath + "DataUser/Seal/Def.png'\" style=\"padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;\" />");
                        sb.Append("\t\n</DIV>");
                    }
                    else
                    {
                        FrmEleDB realDB = null;
                        FrmEleDB imgDb = new FrmEleDB();
                        QueryObject objQuery = new QueryObject(imgDb);
                        objQuery.AddWhere(FrmEleAttr.FK_MapData, img.EnPK);
                        objQuery.addAnd();
                        objQuery.AddWhere(FrmEleAttr.EleID, en.GetValStrByKey("OID"));

                        if (objQuery.DoQuery() == 0)
                        {
                            FrmEleDBs imgdbs = new FrmEleDBs();
                            QueryObject objQuerys = new QueryObject(imgdbs);
                            objQuerys.AddWhere(FrmEleAttr.EleID, en.GetValStrByKey("OID"));
                            if (objQuerys.DoQuery() > 0)
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
                            sb.Append("\t\n<img src='" + imgSrc + "' onerror='javascript:this.src='" + appPath + "DataUser/ICON/" + BP.Sys.SystemConfig.CustomerNo + "/LogBiger.png';' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
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
                        sb.Append("<input type=button class=Btn value='" + btn.Text.Replace("&nbsp;", " ") + "' disabled='disabled'/>");
                        break;
                    case BtnEventType.RunExe:
                    case BtnEventType.RunJS:
                        sb.Append("<input type=button class=Btn value=\"" + btn.Text.Replace("&nbsp;", " ") + "\" enable=true onclick=\"" + doDoc + "\" />");
                        break;
                    default:
                        sb.Append("<input type=button value='" + btn.Text + "' />");
                        break;
                }
                sb.Append("\t\n</span>");
                sb.Append("\t\n</DIV>");
            }
            #endregion 输出竖线与标签

            #region 输出数据控件.
            int fSize = 0;
            foreach (MapAttr attr in attrs)
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
                    sb.Append("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + x + "px; top:" + attr.Y + "px; width:auto" + attr.UIWidth + "px; height:16px;text-align: left;word-break: keep-all;' >");

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
                       text = en.GetValStrByKey(attr.KeyOfEn);
                        break;
                    case FieldTypeS.Enum:
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
                    if (text == "0")
                        text = "[&#10005]";
                    else
                        text = "[&#10004]";
                }

                 sb.Append(text);

                sb.Append("</span>");
                sb.Append("</DIV>");
            }

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

                //string paras = this.RequestParas;
                //string strs = "";
                //foreach (string str in Glo.Request.QueryString.Keys)
                //{
                //    if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
                //        continue;
                //    strs += "&" + str + "=" + this.Request.QueryString[str];
                //}

                string src = "";
                if (dtl.HisRowShowModel == RowShowModel.Table)
                {
                    src = appPath + "WF/CCForm/Dtl.htm?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1";
                }
                else
                {
                    src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1";
                }

                sb.Append("<iframe ID='F" + dtl.No + "' onload= 'F" + dtl.No + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

                sb.Append("</span>");
                sb.Append("</DIV>");
            }
            #endregion 输出明细.

            #region 审核组件
            if (flowNo != null)
            {
                FrmWorkCheck fwc = new FrmWorkCheck(frmID);
                if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Disable)
                {
                    x = fwc.FWC_X + wtX;
                    sb.Append("<DIV id='DIVWC" + fwc.No + "' style='position:absolute; left:" + x + "px; top:" + fwc.FWC_Y + "px; width:" + fwc.FWC_W + "px; height:" + fwc.FWC_H + "px;text-align: left;' >");
                    sb.Append("<span>");

                    sb.Append("<table   style='border: 1px outset #C0C0C0;padding: inherit; margin: 0;border-collapse:collapse;width:100%;' >");

                    #region 生成审核信息.
                    if (flowNo != null)
                    {
                        string sql = "SELECT EmpFrom, EmpFromT,RDT,Msg,NDFrom FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);

                        //获得当前待办的人员,把当前审批的人员排除在外,不然就有默认同意的意见可以打印出来.
                        sql = "SELECT FK_Emp, FK_Node FROM WF_GenerWorkerList WHERE IsPass!=1 AND WorkID="+workid;
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

                            sb.Append("<br><br>");

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

                    sb.Append("</span>");
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

                    string src = appPath + "WF/WorkOpt/SubFlow.aspx?s=2";
                    string fwcOnload = "";
                    //string paras = this.RequestParas;
                    //if (paras.Contains("FID=") == false && en.EnMap.Attrs.Contains("FID") == true)
                    //    paras += "&FID=" + en.GetValStrByKey("FID");
                    //if (paras.Contains("OID=") == false)
                    //    paras += "&OID=" + en.GetValStrByKey("OID");
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
            FrmAttachmentDBs athDBs = null;
            if (aths.Count > 0)
                athDBs = new FrmAttachmentDBs(frmID, en.PKVal.ToString());

            foreach (FrmAttachment ath in aths)
            {

                if (ath.UploadType == AttachmentUploadType.Single)
                {
                    /* 单个文件 */
                    FrmAttachmentDB athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, ath.MyPK) as FrmAttachmentDB;
                    x = ath.X + wtX;
                    float y = ath.Y;
                    sb.Append("<DIV id='Fa" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + y + "px; text-align: left;float:left' >");
                    //  sb.Append("<span>");
                    sb.Append("<DIV>");

                    sb.Append("附件没有转化:" + athDB.FileName);

                    //string node = this.Request.QueryString["FK_Node"];
                    //if (string.IsNullOrEmpty(node) && en.EnMap.Attrs.Contains("FK_Node"))
                    //{
                    //    node = en.GetValStrByKey("FK_Node");
                    //    if (node == "0" || node == "")
                    //        node = ((Work)en).NodeID.ToString();
                    //}
                    //if (athDB != null)
                    //{
                    //    if (ath.IsWoEnableWF)
                    //        lab.Text = "<a  href=\"javascript:OpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.HisEn.GetValStrByKey("OID") + "','" + athDB.MyPK + "','" + this.FK_MapData + "','" + ath.NoOfObj + "','" + node + "','" + athDB.FileExts + "')\"><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName + "</a>";
                    //    else
                    //        lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName;
                    //}
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

                    foreach (FrmAttachmentDB item in athDBs)
                    {
                        if (ath.AthSaveWay == AthSaveWay.FTPServer)
                        {
                            try
                            {
                                string toFile = path + "\\pdf\\" + item.FileName;
                                if (System.IO.File.Exists(toFile) == false)
                                {
                                    //把文件copy到,
                                    string file = item.GenerTempFile(ath.AthSaveWay);
                                    System.IO.File.Copy(file, toFile, true);
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
                                    System.IO.File.Copy(item.FileFullName, path + "\\pdf\\" + item.FileName, true);
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

        public static StringBuilder GenerHtmlOfFool(MapData mapData, string frmID, Int64 workid, Entity en, string path, string flowNo = null)
        {
            StringBuilder sb = new System.Text.StringBuilder();

            //字段集合.
            MapAttrs attrs = new MapAttrs(frmID);

            string appPath = "";
            float wtX = MapData.GenerSpanWeiYi(mapData, 1200);
            //float wtX = 0;
            float x = 0;

            //生成表头.

            string frmName = mapData.Name;
            if (SystemConfig.AppSettings["CustomerNo"] == "TianYe")
                frmName = "";
            sb.Append("\t\n <table style='width:950px;height:auto;' >");
            sb.Append("\t\n <tr style='border-style:none;border-width:0px;' >");
            sb.Append("\t\n <td colspan=1 style='border-style:none;border-width:0px;float:left' ><img src='icon.png' style='height:60px;'  /></td>");
            sb.Append("\t\n <td colspan=2 style='border-style:none;border-width:0px;' ><div  style='padding-left:10px;' ><br><h2><b>" + frmName + "</b></h2></div></td>");
            sb.Append("\t\n <td colspan=1 style='border-style:none;border-width:0px;float:right' ><img src='QR.png' style='height:60px;'  /></td>");
            sb.Append("\t\n </tr>");

            GroupFields gfs = new GroupFields(frmID);
            foreach (GroupField gf in gfs)
            {
                //输出标题.
                sb.Append("\t\n <tr>");
                sb.Append("\t\n  <th colspan=4><b>" + gf.Lab + "</b></th>");
                sb.Append("\t\n </tr>");

                #region 输出字段.
                if (gf.CtrlID == "" && gf.CtrlType == "")
                {
                    var isDropTR = true;
                    string html = "";
                    foreach (MapAttr attr in attrs)
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
                                text = en.GetValStrByKey(attr.KeyOfEn);
                                break;
                            case FieldTypeS.Enum:
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
                            if (text == "0")
                                text = "[&#10005]";
                            else
                                text = "[&#10004]";
                        }

                        // text = text.Replace("font-family: 楷体", "font-family: 宋体");
                        // text = text.Replace("font-family: 隶书", "font-family: 宋体");
                        //text = System.Web.HttpUtility.UrlDecode(text, System.Text.Encoding.UTF8);
                        // text = System.Web.HttpUtility.UrlDecode(text, System.Text.Encoding.GetEncoding("gb2312"));


                        //  System.Text.UTF8Encoding utf = new System.Text.UTF8Encoding();
                        // utf.GetChars(

                        //线性展示并且colspan=3
                        if (attr.ColSpan == 3)
                        {
                            isDropTR = true;
                            html += "\t\n <tr>";
                            html += "\t\n <td  class='FDesc'  >" + attr.Name + "</td>";
                            html += "\t\n <td ColSpan=3>";
                            html += text;
                            html += "\t\n </td>";
                            html += "\t\n </tr>";
                            continue;
                        }

                        //线性展示并且colspan=4
                        if (attr.ColSpan == 4)
                        {
                            isDropTR = true;
                            html += "\t\n <tr>";
                            html += "\t\n <td ColSpan=4 style='width:100%' >" + attr.Name + "</br>";
                            html += text;
                            html += "\t\n </td>";
                            html += "\t\n </tr>";
                            continue;
                        }

                        if (isDropTR == true)
                        {
                            html += "\t\n <tr>";
                            html += "\t\n <td class='FDesc' style='width:80px;' >" + attr.Name + "</td>";
                            html += "\t\n <td class='FContext'  >";
                            html += text;
                            html += "\t\n </td>";
                            isDropTR = !isDropTR;
                            continue;
                        }

                        if (isDropTR == false)
                        {
                            html += "\t\n <td  class='FDesc'>" + attr.Name + "</td>";
                            html += "\t\n <td class='FContext'  >";
                            html += text;
                            html += "\t\n </td>";
                            html += "\t\n </tr>";
                            isDropTR = !isDropTR;
                            continue;
                        }
                    }
                    sb.Append(html); //增加到里面.
                    continue;
                }
                #endregion 输出字段.

                #region 如果是附件.
                if (gf.CtrlType == "Ath")
                {
                    FrmAttachments aths = new FrmAttachments(frmID);

                    foreach (FrmAttachment ath in aths)
                    {
                        if (ath.MyPK != gf.CtrlID)
                            continue;

                        BP.Sys.FrmAttachmentDBs athDBs = BP.WF.Glo.GenerFrmAttachmentDBs(ath, workid.ToString(), ath.MyPK);


                        if (ath.UploadType == AttachmentUploadType.Single)
                        {
                            /* 单个文件 */
                            sb.Append("<tr><td colspan=4>单附件没有转化:" + ath.MyPK + "</td></td>");
                            continue;
                        }

                        if (ath.UploadType == AttachmentUploadType.Multi)
                        {
                            sb.Append("\t\n<tr><td valign=top colspan=4 >");
                            sb.Append("\t\n<ul>");

                            //判断是否有这个目录.
                            if (System.IO.Directory.Exists(path + "\\pdf\\") == false)
                                System.IO.Directory.CreateDirectory(path + "\\pdf\\");

                            foreach (FrmAttachmentDB item in athDBs)
                            {
                                string fileTo = path + "\\pdf\\" + item.FileName;

                                #region 从ftp服务器上下载.
                                if (ath.AthSaveWay == AthSaveWay.FTPServer)
                                {
                                    try
                                    {
                                        //把文件copy到,
                                        if (System.IO.File.Exists(fileTo) == false)
                                        {
                                            string file = item.GenerTempFile(ath.AthSaveWay);


                                            System.IO.File.Copy(file, fileTo, true);
                                        }

                                        sb.Append("<li><a href='" + item.FileName + "'>" + item.FileName + "</a></li>");
                                    }
                                    catch (Exception ex)
                                    {
                                        sb.Append("<li>" + item.FileName + "(<font color=red>文件未从ftp下载成功{" + ex.Message + "}</font>)</li>");
                                    }
                                }
                                #endregion 从ftp服务器上下载.

                                #region 从iis服务器上下载.
                                if (ath.AthSaveWay == AthSaveWay.IISServer)
                                {
                                    try
                                    {
                                        //把文件copy到,
                                        if (System.IO.File.Exists(fileTo) == false)
                                            System.IO.File.Copy(item.FileFullName, fileTo, true);

                                        sb.Append("<li><a href='" + item.FileName + "'>" + item.FileName + "</a></li>");
                                    }
                                    catch (Exception ex)
                                    {
                                        sb.Append("<li>" + item.FileName + "(<font color=red>文件未从iis下载成功{" + ex.Message + "}</font>)</li>");
                                    }
                                }
                                #endregion 从iis服务器上下载.

                            }
                            sb.Append("\t\n</ul>");
                            sb.Append("\t\n</td></tr>");
                        }
                    }
                }
                #endregion 如果是附件.

                #region 审核组件
                if (gf.CtrlType == "FWC" && flowNo != null)
                {
                    //sb.Append(" \t\n <tr><td colspan=4 valign=top style='width:100%;valign:middle;height:auto;'  >");
                    FrmWorkCheck fwc = new FrmWorkCheck(frmID);


                    string sql = "";
                    DataTable dtTrack = null;

                    if (DBAccess.IsExitsTableCol("Port_Emp", "SignType") == true)
                    {
                        string tTable = "ND" + int.Parse(flowNo) + "Track";
                        sql = "SELECT a.No, a.SignType FROM Port_Emp a, " + tTable + " b WHERE a.No=b.EmpFrom AND B.WorkID=" + workid;

                        dtTrack = DBAccess.RunSQLReturnTable(sql);
                        dtTrack.TableName = "SignType";

                        dtTrack.Columns[0].ColumnName = "No";
                        dtTrack.Columns[1].ColumnName = "SignType";
                    }

                    string html = ""; // "<table style='width:100%;valign:middle;height:auto;' >";

                    #region 生成审核信息.
                    sql = "SELECT NDFromT,Msg,RDT,EmpFromT,EmpFrom,NDFrom FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
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



                        html += "<tr>";
                        html += " <td valign=middle >" + dr["NDFromT"] + "</td>";

                        string msg = dr["Msg"].ToString();

                        msg += "<br>";
                        msg += "<br>";

                        string empStrs = "";
                        if (dtTrack == null)
                        {
                            empStrs = dr["EmpFromT"].ToString();
                        }
                        else
                        {
                            string singType = "0";
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
                                empStrs = "<img src='../../../../../DataUser/Siganture/" + dr["EmpFrom"] + ".jpg' title='" + dr["EmpFromT"] + "' style='height:60px;' border=0 onerror=\"src='../../../../../DataUser/Siganture/UnName.JPG'\" />";
                            }

                        }

                        msg += "审核人:" + empStrs + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日期:" + dr["RDT"].ToString();

                        html += " <td colspan=3 valign=middle >" + msg + "</td>";
                        html += "\t\n </tr>";
                    }
                    #endregion 生成审核信息.

                    sb.Append("\t\n " + html);
                }
                #endregion 审核组件
            }

            sb.Append("\t\n</table>");
            return sb;
        }
        /// <summary>
        /// zip文件路径.
        /// </summary>
        public static string ZipFilePath = "";

        public static string CCFlowAppPath = "/";
        public static string MakeHtmlDocument(string frmID, Int64 workid, string flowNo = null, string fileNameFormat = null)
        {
            try
            {
                #region 准备目录文件.
                string path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\";
                try
                {
                    path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\";
                    if (System.IO.Directory.Exists(path) == false)
                        System.IO.Directory.CreateDirectory(path);

                    path = SystemConfig.PathOfDataUser + "InstancePacketOfData\\" + frmID + "\\" + workid;
                    if (System.IO.Directory.Exists(path) == false)
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }


                    //把模版文件copy过去.
                    string templateFilePath = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\";
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(templateFilePath);
                    System.IO.FileInfo[] finfos = dir.GetFiles();
                    foreach (System.IO.FileInfo fl in finfos)
                    {
                        if (fl.Name.Contains("ShuiYin"))
                            continue;

                        if (fl.Name.Contains("htm"))
                            continue;

                        System.IO.File.Copy(fl.FullName, path + "\\" + fl.Name, true);
                    }

                    //把ccs文件copy过去.
                    System.IO.File.Copy(SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\ccbpm.css", path + "\\ccbpm.css", true);

                }
                catch (Exception ex)
                {
                    return "err@读写文件出现权限问题，请联系管理员解决。" + ex.Message;
                }

                #endregion 准备目录文件.

                #region 生成二维码.
                /*说明是图片文件.*/
                string pathQR = path + "\\QR.png"; // key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);
                string billUrl = SystemConfig.HostURLOfBS + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/index.htm";
                ThoughtWorks.QRCode.Codec.QRCodeEncoder qrc = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
                qrc.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
                qrc.QRCodeScale = 4;
                qrc.QRCodeVersion = 7;
                qrc.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
                System.Drawing.Bitmap btm = qrc.Encode(billUrl, System.Text.Encoding.UTF8);
                btm.Save(pathQR);
                #endregion

                #region 定义变量做准备.
                //生成表单信息.
                GEEntity en = new GEEntity(frmID, workid);

                MapData mapData = new MapData(frmID);
                #endregion

                #region 生成水文.

                string rdt = "";
                if (en.EnMap.Attrs.Contains("RDT"))
                {
                    rdt = en.GetValStringByKey("RDT");
                    if (rdt.Length > 10)
                        rdt = rdt.Substring(0, 10);
                }
                string words = Glo.PrintBackgroundWord;
                words = words.Replace("@RDT", rdt);

                if (words.Contains("@") == true)
                    words = Glo.DealExp(words, en, null);

                string templateFilePathMy = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\";
                WaterImageManage wim = new WaterImageManage();
                wim.DrawWords(templateFilePathMy + "ShuiYin.png", words, float.Parse("0.15"), ImagePosition.Center, path + "\\ShuiYin.png");
                #endregion

                //生成 表单的 html.
                StringBuilder sb = new System.Text.StringBuilder();
                if (mapData.HisFrmType == FrmType.FoolForm)
                    sb = BP.WF.MakeForm2Html.GenerHtmlOfFool(mapData, frmID, workid, en, path, flowNo);
                else
                    sb = BP.WF.MakeForm2Html.GenerHtmlOfFree(mapData, frmID, workid, en, path, flowNo);

                #region 替换模版文件..

                string docs = "";

                if (mapData.HisFrmType == FrmType.FoolForm)
                    docs = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\Template\\indexFool.htm");
                else
                    docs = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\Template\\indexFree.htm");

                docs = docs.Replace("@Docs", sb.ToString());
                docs = docs.Replace("@Width", mapData.FrmW.ToString());
                docs = docs.Replace("@Height", mapData.FrmH.ToString());

                if (flowNo != null)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = workid;
                    gwf.RetrieveFromDBSources();

                    docs = docs.Replace("@Title", gwf.Title);

                    //替换模版尾部的打印说明信息.
                    string pathInfo = SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\Template\\EndInfo\\" + flowNo + ".txt";
                    if (System.IO.File.Exists(pathInfo) == false)
                        pathInfo = SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\Template\\EndInfo\\Default.txt";

                    docs = docs.Replace("@EndInfo", DataType.ReadTextFile(pathInfo));
                }

                string indexFile = SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\" + frmID + "\\" + workid + "\\index.htm";
                BP.DA.DataType.WriteFile(indexFile, docs);
                #endregion 替换模版文件..

                #region 处理正确的文件名.

                if (fileNameFormat == null)
                {
                    if (flowNo != null)
                        fileNameFormat = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + workid, "" + workid.ToString());
                    else
                        fileNameFormat = workid.ToString();
                }

                if (string.IsNullOrEmpty(fileNameFormat) == true)
                    fileNameFormat = workid.ToString();

                fileNameFormat = BP.DA.DataType.PraseStringToFileName(fileNameFormat);
                #endregion

                Hashtable ht = new Hashtable();
                ht.Add("htm", billUrl);

                #region 把所有的文件做成一个zip文件.
                //生成pdf文件
                string pdfPath = path + "\\pdf";
                if (System.IO.Directory.Exists(pdfPath) == false)
                    System.IO.Directory.CreateDirectory(pdfPath);

                string pdfFile = pdfPath + "\\" + fileNameFormat + ".pdf";
                string pdfFileExe = SystemConfig.PathOfDataUser + "ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
                try
                {
                    Html2Pdf(pdfFileExe, billUrl, pdfFile);
                    ht.Add("pdf", SystemConfig.HostURLOfBS + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");
                }
                catch (Exception ex)
                {
                    /*有可能是因为文件路径的错误， 用补偿的方法在执行一次, 如果仍然失败，按照异常处理. */
                    fileNameFormat = DBAccess.RunSQLReturnStringIsNull("SELECT BillNo FROM WF_GenerWorkFlow WHERE WorkID=" + workid, workid.ToString());
                    pdfFile = pdfPath + "\\" + fileNameFormat + ".pdf";

                    try
                    {
                        Html2Pdf(pdfFileExe, billUrl, pdfFile);
                        ht.Add("pdf", SystemConfig.HostURLOfBS + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/pdf/" + DataType.PraseStringToUrlFileName(fileNameFormat) + ".pdf");
                    }
                    catch
                    {
                        ht.Add("pdf", "err@生成pdf错误:" + ex.Message + "@路径变量: pdfFileExe=" + pdfFileExe + " pdf " + pdfFile + " ,  html url:" + billUrl);
                    }
                }

                string zipFile = path + "\\..\\" + fileNameFormat + ".zip";

                System.IO.FileInfo finfo = new FileInfo(zipFile);
                ZipFilePath = finfo.FullName; //文件路径.

                try
                {
                    (new FastZip()).CreateZip(finfo.FullName, pdfPath, true, "");

                    ht.Add("zip", SystemConfig.HostURLOfBS + "DataUser/InstancePacketOfData/" + frmID + "/" + DataType.PraseStringToUrlFileName( fileNameFormat) + ".zip");
                }
                catch (Exception ex)
                {
                    ht.Add("zip", "err@生成zip文件遇到权限问题:" + ex.Message + " @Path:" + pdfFile);
                }
                #endregion 把所有的文件做成一个zip文件.

                return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
            }
            catch (Exception ex)
            {
                return "err@报表生成错误:" + ex.Message;
            }
        }
        public static void Html2Pdf(string pdfFileExe, string htmFile, string pdf)
        {
            BP.DA.Log.DebugWriteInfo("@开始生成PDF" + pdfFileExe + "@pdf=" + pdf + "@htmFile=" + htmFile);
            try
            {
                string fileNameWithOutExtention = System.Guid.NewGuid().ToString();
                Process p = System.Diagnostics.Process.Start(pdfFileExe, " --disable-external-links " + htmFile + " " + pdf);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                //BP.DA.Log.DebugWriteError("@生成PDF错误" + ex.Message + "@pdf=" + pdf + "@htmFile="+htmFile);
                throw ex;
            }
        }
    }
}
