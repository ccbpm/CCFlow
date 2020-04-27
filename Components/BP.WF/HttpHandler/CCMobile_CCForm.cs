using System;
using System.IO;
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
using System.Net.Http;
using System.Collections;
using NPOI.SS.Formula.Functions;
using LitJson;
using System.Net;
using BP.WF.WeiXin;
using System.Security.Cryptography;
using BP.Tools;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_CCForm : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_CCForm()
        {
        }
        public string HandlerMapExt()
        {
            WF_CCForm en = new WF_CCForm();
            return en.HandlerMapExt();
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_Down();
        }
        /// <summary>
        /// 表单初始化.
        /// </summary>
        /// <returns></returns>
        public string Frm_Init()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.Frm_Init();
        }

        public string Dtl_Init()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.Dtl_Init();
        }

        //保存从表数据
        public string Dtl_SaveRow()
        {
            #region  查询出来从表数据.
            GEDtls dtls = new GEDtls(this.EnsName);
            GEDtl dtl = dtls.GetNewEntity as GEDtl;
            dtls.Retrieve("RefPK", this.GetRequestVal("RefPKVal"));
            MapDtl mdtl = new MapDtl(this.EnsName);
            Map map = dtl.EnMap;
            foreach (GEDtl item in dtls)
            {
                string pkval = item.GetValStringByKey(dtl.PK);
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.IsRefAttr == true)
                        continue;

                    if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                    {
                        if (attr.UIIsReadonly == true)
                            continue;

                        string val = this.GetValFromFrmByKey("TB_" + attr.Key + "_" + pkval, null);
                        item.SetValByKey(attr.Key, val);
                        continue;
                    }


                    if (attr.UIContralType == UIContralType.TB)
                    {
                       
                        string val = this.GetValFromFrmByKey("TB_" + attr.Key + "_" + pkval, null);
                        item.SetValByKey(attr.Key, HttpUtility.UrlDecode(val, Encoding.UTF8));
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        string val = this.GetValFromFrmByKey("DDL_" + attr.Key + "_" + pkval);
                        item.SetValByKey(attr.Key, HttpUtility.UrlDecode(val, Encoding.UTF8));
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.CheckBok)
                    {
                        string val = this.GetValFromFrmByKey("CB_" + attr.Key + "_" + pkval, "-1");
                        if (val == "0")
                            item.SetValByKey(attr.Key, 0);
                        else
                            item.SetValByKey(attr.Key, 1);
                        continue;
                    }
                }
                item.SetValByKey("OID",pkval);
                //关联主赋值.
                item.RefPK = this.RefPKVal;
                switch (mdtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        item.RefPK = this.RefPKVal;
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        item.RefPK = this.RefPKVal;
                        item.FID = long.Parse(this.RefPKVal);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.
                        item.RefPK = this.RefPKVal;
                        item.FID = this.FID;
                        break;
                }
                item.Rec = WebUser.No;
                item.Update(); //执行更新.
            }
            return "保存成功.";
            #endregion  查询出来从表数据.

            //#region 保存新加行.
          
            //string keyVal = "";
            //foreach (Attr attr in map.Attrs)
            //{

            //    if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
            //    {
            //        if (attr.UIIsReadonly == true)
            //            continue;

            //        keyVal = this.GetValFromFrmByKey("TB_" + attr.Key + "_0", null);
            //        dtl.SetValByKey(attr.Key, keyVal);
            //        continue;
            //    }


            //    if (attr.UIContralType == UIContralType.TB && attr.UIIsReadonly == false)
            //    {
            //        keyVal = this.GetValFromFrmByKey("TB_" + attr.Key + "_0");
            //        if (attr.IsNum && keyVal == "")
            //            keyVal = "0";
            //        dtl.SetValByKey(attr.Key, keyVal);
            //        continue;
            //    }

            //    if (attr.UIContralType == UIContralType.DDL && attr.UIIsReadonly == true)
            //    {
            //        keyVal = this.GetValFromFrmByKey("DDL_" + attr.Key + "_0");
            //        dtl.SetValByKey(attr.Key, keyVal);
            //        continue;
            //    }

            //    if (attr.UIContralType == UIContralType.CheckBok && attr.UIIsReadonly == true)
            //    {
            //        keyVal = this.GetValFromFrmByKey("CB_" + attr.Key + "_0", "-1");
            //        if (keyVal == "-1")
            //            dtl.SetValByKey(attr.Key, 0);
            //        else
            //            dtl.SetValByKey(attr.Key, 1);
            //        continue;
            //    }
            //}

            //dtl.SetValByKey("RefPK", this.GetRequestVal("RefPKVal"));
            //dtl.PKVal = "0";
            //dtl.Insert();
            
            //#endregion 保存新加行.

            //return "保存成功.";
        }

        //多附件上传方法
        public void MoreAttach()
        {
            string PKVal = this.GetRequestVal("PKVal");
            string attachPk = this.GetRequestVal("AttachPK");
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);
            MapData mapData = new MapData(athDesc.FK_MapData);
            string msg = null;
            GEEntity en = new GEEntity(athDesc.FK_MapData);
            en.PKVal = PKVal;
            en.Retrieve();

            var files = HttpContextHelper.RequestFiles();
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                #region 文件上传的iis服务器上 or db数据库里.
                if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                {

                    string savePath = athDesc.SaveTo;
                    if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                    {
                        /*如果有变量*/
                        savePath = savePath.Replace("*", "@");
                        savePath = BP.WF.Glo.DealExp(savePath, en, null);

                        if (savePath.Contains("@") && this.FK_Node != 0)
                        {
                            /*如果包含 @ */
                            BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                            BP.WF.Data.GERpt myen = flow.HisGERpt;
                            myen.OID = this.WorkID;
                            myen.RetrieveFromDBSources();
                            savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                        }
                        if (savePath.Contains("@") == true)
                            throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                    }
                    else
                    {
                        savePath = athDesc.SaveTo + "\\" + PKVal;
                    }

                    //替换关键的字串.
                    savePath = savePath.Replace("\\\\", "\\");
                    try
                    {
                        savePath = SystemConfig.PathOfWebApp + savePath;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (System.IO.Directory.Exists(savePath) == false)
                            System.IO.Directory.CreateDirectory(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + SystemConfig.PathOfWebApp + savePath + "===" + savePath + "@技术问题:" + ex.Message);
                    }

                    string exts = System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "");
                    string guid = BP.DA.DBAccess.GenerGUID();

                    string fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                    if (fileName.LastIndexOf("\\") > 0)
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    string ext = System.IO.Path.GetExtension(file.FileName);
                    string realSaveTo = savePath + "\\" + guid + "." + fileName + ext;

                    realSaveTo = realSaveTo.Replace("~", "-");
                    realSaveTo = realSaveTo.Replace("'", "-");
                    realSaveTo = realSaveTo.Replace("*", "-");

                    HttpContextHelper.UploadFile(file, realSaveTo);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + realSaveTo);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

                        try
                        {
                            File.Delete(realSaveTo);
                        }
                        catch
                        {
                        }
                        //note:此处如何向前uploadify传递失败信息，有待研究
                        //this.Alert("上传附件错误：" + msg, true);
                        return;
                    }

                    FileInfo info = new FileInfo(realSaveTo);

                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
                    dbUpload.NodeID = this.FK_Node;
                    dbUpload.FK_FrmAttachment = attachPk;
                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = attachPk;
                    dbUpload.FileExts = info.Extension;

                    #region 处理文件路径，如果是保存到数据库，就存储pk.
                    if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                    {
                        //文件方式保存
                        dbUpload.FileFullName = realSaveTo;
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        //保存到数据库
                        dbUpload.FileFullName = dbUpload.MyPK;
                    }
                    #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                    dbUpload.FileName = fileName + ext;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.RDT = DataType.CurrentDataTimess;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    dbUpload.FK_Dept = WebUser.FK_Dept;
                    dbUpload.FK_DeptName = WebUser.FK_DeptName;
                    dbUpload.RefPKVal = PKVal;
                    dbUpload.FID = this.FID;

                    //if (athDesc.IsNote)
                    //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                    //if (athDesc.Sort.Contains(","))
                    //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

                    dbUpload.UploadGUID = guid;
                    dbUpload.Insert();

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        //执行文件保存.
                        BP.DA.DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + dbUpload.FileFullName);
                    if (!string.IsNullOrEmpty(msg))
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
                }
                #endregion 文件上传的iis服务器上 or db数据库里.

                #region 保存到数据库 / FTP服务器上.
                if (athDesc.AthSaveWay == AthSaveWay.DB || athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    string guid = DBAccess.GenerGUID();

                    //把文件临时保存到一个位置.
                    string temp = SystemConfig.PathOfTemp + "" + guid + ".tmp";
                    try
                    {
                        HttpContextHelper.UploadFile(file, temp);
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.Delete(temp);
                        HttpContextHelper.UploadFile(file, temp);
                    }

                    //  fu.SaveAs(temp);

                    //执行附件上传前事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + temp);
                    if (string.IsNullOrEmpty(msg) == false)
                    {
                        BP.Sys.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + file.FileName + "，" + msg);

                        try
                        {
                            File.Delete(temp);
                        }
                        catch
                        {
                        }

                        throw new Exception("err@上传附件错误：" + msg);
                    }

                    FileInfo info = new FileInfo(temp);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = BP.DA.DBAccess.GenerGUID();
                    dbUpload.NodeID = FK_Node;
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                    dbUpload.FID = this.FID; //流程id.
                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /*如果是继承，就让他保持本地的PK. */
                        dbUpload.RefPKVal = PKVal.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*如果是协同，就让他是PWorkID. */
                        Paras ps = new Paras();
                        ps.SQL = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add("WorkID", PKVal);
                        string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0).ToString();
                        if (pWorkID == null || pWorkID == "0")
                            pWorkID = PKVal;
                        dbUpload.RefPKVal = pWorkID;
                    }

                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = athDesc.MyPK;
                    dbUpload.FileName = file.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.RDT = DataType.CurrentDataTimess;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    dbUpload.FK_Dept = WebUser.FK_Dept;
                    dbUpload.FK_DeptName = WebUser.FK_DeptName;
                    //if (athDesc.IsNote)
                    //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

                    //if (athDesc.Sort.Contains(","))
                    //{
                    //    string[] strs = athDesc.Sort.Contains("@") == true ? athDesc.Sort.Substring(athDesc.Sort.LastIndexOf("@") + 1).Split(',') : athDesc.Sort.Split(',');
                    //    BP.Web.Controls.DDL ddl = this.Pub1.GetDDLByID("ddl");
                    //    dbUpload.Sort = strs[0];
                    //    if (ddl != null)
                    //    {
                    //        int selectedIndex = string.IsNullOrEmpty(ddl.SelectedItemStringVal) ? 0 : int.Parse(ddl.SelectedItemStringVal);
                    //        dbUpload.Sort = strs[selectedIndex];
                    //    }
                    //}

                    dbUpload.UploadGUID = guid;

                    if (athDesc.AthSaveWay == AthSaveWay.DB)
                    {
                        dbUpload.Insert();
                        //把文件保存到指定的字段里.
                        dbUpload.SaveFileToDB("FileDB", temp);
                    }

                    if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        /*保存到fpt服务器上.*/
                        FtpConnection ftpconn = new FtpConnection(SystemConfig.FTPServerIP, SystemConfig.FTPPort,
                            SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);

                        string ny = DateTime.Now.ToString("yyyy_MM");

                        //判断目录年月是否存在.
                        if (ftpconn.DirectoryExist(ny) == false)
                            ftpconn.CreateDirectory(ny);
                        ftpconn.SetCurrentDirectory(ny);

                        //判断目录是否存在.
                        if (ftpconn.DirectoryExist(athDesc.FK_MapData) == false)
                            ftpconn.CreateDirectory(athDesc.FK_MapData);

                        //设置当前目录，为操作的目录。
                        ftpconn.SetCurrentDirectory(athDesc.FK_MapData);

                        //把文件放上去.
                        ftpconn.PutFile(temp, guid + "." + dbUpload.FileExts);
                        ftpconn.Close();

                        //设置路径.
                        dbUpload.FileFullName = ny + "//" + athDesc.FK_MapData + "//" + guid + "." + dbUpload.FileExts;
                        dbUpload.Insert();
                    }

                    //执行附件上传后事件，added by liuxc,2017-7-15
                    msg = mapData.DoEvent(FrmEventList.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + temp);
                    if (!string.IsNullOrEmpty(msg))
                        BP.Sys.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
                }
                #endregion 保存到数据库.
            }
        }
		/// <summary>
        /// 获取百度云token
        /// </summary>
        /// <returns></returns>
        public string getAccessToken()
        {
            string ak = Sys.SystemConfig.APIKey;
            string sk = Sys.SystemConfig.SecretKey;

            //百度云应用获取token
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", ak));
            paraList.Add(new KeyValuePair<string, string>("client_secret", sk));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);

            return result;
        }
        /// <summary>
        /// 调用企业号获取地理位置
        /// </summary>
        /// <returns></returns>
        public string GetWXConfigSetting()
        {
            //必须是当前页面，如果在CCMobile/Home.htm调用，则传入Home.htm
            string htmlPage = this.GetRequestVal("htmlPage");
            Hashtable ht = new Hashtable();

            //生成签名的时间戳
            string timestamp = DateTime.Now.ToString("yyyyMMDDHHddss");
            //生成签名的随机串
            string nonceStr = BP.DA.DBAccess.GenerGUID();
            //企业号jsapi_ticket
            string jsapi_ticket = "";
            string url1 =  htmlPage;
            //获取 AccessToken
            string accessToken = new BP.WF.WeiXin.WeiXin().GenerAccessToken();
            
            string url = "https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + accessToken;


            HttpWebResponse response = new HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();

            //权限签名算法
            Ticket ticket = new Ticket();
            ticket = FormatToJson.ParseFromJson<Ticket>(str);

            if (ticket.errcode == "0")
                jsapi_ticket = ticket.ticket;
            else
                return "err:@获取jsapi_ticket失败+accessToken="+ accessToken;

            ht.Add("timestamp", timestamp);
            ht.Add("nonceStr", nonceStr);
            //企业微信的corpID
            ht.Add("AppID", BP.Sys.SystemConfig.WX_CorpID);

            //生成签名算法
            string str1 = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url1 + "";
            string Signature = Sha1Signature(str1);
            ht.Add("signature", Signature);

            return BP.Tools.Json.ToJson(ht);
        }
        public static string Sha1Signature(string str)
        {
            string s = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToString();
            return s.ToLower();
        }

        public string GetIDCardInfo()
        {
            string token = getAccessToken();
            JsonData jd = JsonMapper.ToObject(token);
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/idcard?access_token=" + jd["access_token"].ToString();
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            var files = HttpContextHelper.RequestFiles();  //context.Request.Files;
            if (files.Count == 0)
                return "err@请选择要上传的身份证件。";
            Stream stream = files[0].InputStream;//new MemoryStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            string base64 = Convert.ToBase64String(bytes);
            stream.Close();
            String str = "id_card_side=" + "front" + "&image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
    }
}
