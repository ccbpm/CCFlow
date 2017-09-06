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
        public static string CCFlowAppPath = "/";
        public static string MakeHtmlDocumentOfFreeFrm(string frmID, Int64 workid, string flowNo = null)
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
                else
                {
                    try
                    {
                        System.IO.Directory.Delete(path, true);
                    }
                    catch (Exception ex)
                    {
                    }
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                return "err@读写文件出现权限问题，请联系管理员解决。" + ex.Message;
            }

            //把模版文件copy过去.
            string templateFilePath = SystemConfig.PathOfDataUser + "InstancePacketOfData\\Template\\";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(templateFilePath);
            System.IO.FileInfo[] finfos = dir.GetFiles();
            foreach (System.IO.FileInfo fl in finfos)
                System.IO.File.Copy(fl.FullName, path + "\\" + fl.Name, true);
            #endregion 准备目录文件.

            #region 生成二维码.
            /*说明是图片文件.*/
            string pathQR = path + "\\QR.png"; // key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);
            string billUrl = SystemConfig.HostURL + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + "/index.htm";
            ThoughtWorks.QRCode.Codec.QRCodeEncoder qrc = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            qrc.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            qrc.QRCodeScale = 4;
            qrc.QRCodeVersion = 7;
            qrc.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Bitmap btm = qrc.Encode(billUrl, System.Text.Encoding.UTF8);
            btm.Save(pathQR);
            #endregion

            #region 定义变量做准备.
            StringBuilder sb = new System.Text.StringBuilder();

            //生成表单信息.
            GEEntity en = new GEEntity(frmID, workid);
            MapData md = new MapData(frmID);

            //字段集合.
            MapAttrs attrs = new MapAttrs(frmID);

            MapData mapData = new MapData(frmID);

            string appPath = "";

            float wtX = MapData.GenerSpanWeiYi(md, 1200);
            //float wtX = 0;
            float x = 0;
            #endregion

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
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:  // 输出普通类型字段.
                        sb.Append(en.GetValStrByKey(attr.KeyOfEn));
                        break;
                    case FieldTypeS.Enum:
                    case FieldTypeS.FK:
                        sb.Append(en.GetValRefTextByKey(attr.KeyOfEn));
                        break;
                    default:
                        break;
                }
                #endregion 通过逻辑类型，输出相关的控件.

                #endregion add contrals.

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
                if (dtl.HisDtlShowModel == DtlShowModel.Table)
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
                        string sql = "SELECT * FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid + " AND ActionType=" + (int)ActionType.WorkCheck + " ORDER BY RDT ";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td valign=middle style='border-style: solid;padding: 4px;text-align: left;color: #333333;font-size: 12px;border-width: 1px;border-color: #C2D5E3;' >" + dr["NDFromT"] + "</td>");

                            sb.Append("<br><br>");

                            string msg = "<font color=green>" + dr["Msg"].ToString() + "</font>";

                            msg += "<br>";
                            msg += "<br>";
                            msg += "审核人:" + dr["EmpFromT"] + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;审核日期:" + dr["RDT"].ToString();

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
                    foreach (FrmAttachmentDB item in athDBs)
                    {
                        if (ath.AthSaveWay == AthSaveWay.FTPServer)
                        {
                            try
                            {
                                //把文件copy到,
                                string file = item.MakeFullFileFromFtp();
                                System.IO.File.Copy(file, path + "\\" + item.FileName, true);
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
                                //把文件copy到,
                                System.IO.File.Copy(item.FileFullName, path + "\\" + item.FileName, true);
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

            #region 替换模版文件..
            string docs = BP.DA.DataType.ReadTextFile(SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\Template\\index.htm");
            docs = docs.Replace("@Docs", sb.ToString());

            docs = docs.Replace("@Width", mapData.FrmW.ToString());
            docs = docs.Replace("@Height", mapData.FrmH.ToString());

            if (flowNo != null)
            {
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                gwf.RetrieveFromDBSources();

                docs = docs.Replace("@Title", gwf.Title);
            }


            string indexFile = SystemConfig.PathOfDataUser + "\\InstancePacketOfData\\" + frmID + "\\" + workid + "\\index.htm";
            BP.DA.DataType.WriteFile(indexFile, docs);
            #endregion 替换模版文件..

            #region 把所有的文件做成一个zip文件.
            string zipFile = path + "\\..\\" + workid + ".zip";
            try
            {
                (new FastZip()).CreateZip(zipFile, path, true, "");
            }
            catch (Exception ex)
            {
                return "err@生成zip文件遇到权限问题:" + ex.Message;
            }
            #endregion 把所有的文件做成一个zip文件.

            //生成pdf文件
            string pdfFile = path + "\\..\\" + workid + ".pdf";
            try
            {
                Html2Pdf(billUrl, pdfFile);
            }
            catch (Exception ex)
            {
                return "err@生成pdf文件遇到权限问题:" + ex.Message;
            }

            Hashtable ht = new Hashtable();
            ht.Add("htm", billUrl);
            ht.Add("zip", SystemConfig.HostURL + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + ".zip");
            ht.Add("pdf", SystemConfig.HostURL + "DataUser/InstancePacketOfData/" + frmID + "/" + workid + ".pdf");
            return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);

        }

        public static void Html2Pdf(string htmFile, string pdf)
        {
            //因为Web 是多线程环境，避免甲产生的文件被乙下载去，所以档名都用唯一

            string pdfFileExe = SystemConfig.PathOfDataUser + "\\ThirdpartySoftware\\wkhtmltox\\wkhtmltopdf.exe";
            string fileNameWithOutExtention = System.Guid.NewGuid().ToString();

            //执行wkhtmltopdf.exe
            //Process p = System.Diagnostics.Process.Start(pdfFileExe, @"http://msdn.microsoft.com/zh-cn D:\" + fileNameWithOutExtention + ".pdf");
            Process p = System.Diagnostics.Process.Start(pdfFileExe, htmFile + " " + pdf);

            //若不加这一行，程序就会马上执行下一句而抓不到文件发生意外：System.IO.FileNotFoundException: 找不到文件 ''。
            p.WaitForExit();

            ////把文件读进文件流
            //FileStream fs = new FileStream(pdf, FileMode.Open);
            //byte[] file = new byte[fs.Length];
            //fs.Read(file, 0, file.Length);
            //fs.Close();

            //Response给客户端下载
            //Response.Clear();
            //Response.AddHeader("content-disposition", "attachment; filename=" + fileNameWithOutExtention + ".pdf");//强制下载
            //Response.ContentType = "application/octet-stream";
            //Response.BinaryWrite(file);
        }
    }
}
