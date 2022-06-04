using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF.Template;
using System.Net.Http;
using System.Collections;
using LitJson;
using System.Net;
using BP.Tools;
using System.Drawing;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 表单
    /// </summary>
    public class CCMobile_CCForm : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_CCForm()
        {
            BP.Web.WebUser.SheBei = "Mobile";
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

        public string FrmGener_Init()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.FrmGener_Init();
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
                        item.SetValByKey(attr.Key, HttpUtility.UrlDecode(val, Encoding.UTF8));
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
                        item.FID = this.FID;
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

        }

        //多附件上传方法
        public string UploadIOSAttach()
        {

            string uploadFileM = ""; //上传附件数据的MyPK,用逗号分开
            string pkVal = this.GetRequestVal("PKVal");
            string attachPk = this.GetRequestVal("FK_FrmAttachment");
            string sort = this.GetRequestVal("Sort");
            string fileSoruce = this.GetRequestVal("fileSource");
            string fileName = this.GetRequestVal("fileName");
            string ext = this.GetRequestVal("Ext");
            
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(attachPk);
            MapData mapData = new MapData(athDesc.FK_MapData);
            string msg = "";
            //求出来实体记录，方便执行事件.
            GEEntity en = new GEEntity(athDesc.FK_MapData);
            en.PKVal = pkVal;
            if (en.RetrieveFromDBSources() == 0)
            {
                en.PKVal = this.FID;
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.PKVal = this.PWorkID;
                    en.RetrieveFromDBSources();
                }
            }

            //求主键. 如果该表单挂接到流程上.
            if (this.FK_Node != 0)
            {
                //判断表单方案。
                FrmNode fn = new FrmNode(this.FK_Node, this.FK_MapData);
                if (fn.FrmSln == FrmSln.Self)
                {
                    BP.Sys.FrmAttachment myathDesc = new FrmAttachment();
                    myathDesc.setMyPK(attachPk + "_" + this.FK_Node);
                    if (myathDesc.RetrieveFromDBSources() != 0)
                        athDesc.HisCtrlWay = myathDesc.HisCtrlWay;
                }
                pkVal = BP.WF.Dev2Interface.GetAthRefPKVal(this.WorkID, this.PWorkID, this.FID, this.FK_Node, this.FK_MapData, athDesc);
            }

            //获取上传文件是否需要加密
            bool fileEncrypt =  BP.Difference.SystemConfig.IsEnableAthEncrypt;

            #region 文件上传的iis服务器上 or db数据库里.
            if (athDesc.AthSaveWay == AthSaveWay.IISServer || athDesc.AthSaveWay == AthSaveWay.DB)
            {
                string savePath = athDesc.SaveTo;
                if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                {
                    /*如果有变量*/
                    savePath = savePath.Replace("*", "@");

                    if (savePath.Contains("@") && this.FK_Node != 0)
                    {
                        /*如果包含 @ */
                        BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                        BP.WF.GERpt myen = flow.HisGERpt;
                        myen.OID = this.WorkID;
                        myen.RetrieveFromDBSources();
                        savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                    }
                    if (savePath.Contains("@") == true)
                        throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                }
                else
                {
                    savePath = athDesc.SaveTo + "/" + pkVal;
                }

                //替换关键的字串.
                savePath = savePath.Replace("\\\\", "/");
                try
                {
                    if (savePath.Contains(BP.Difference.SystemConfig.PathOfWebApp) == false)
                        savePath =  BP.Difference.SystemConfig.PathOfWebApp + savePath;
                }
                catch (Exception ex)
                {
                    savePath =  BP.Difference.SystemConfig.PathOfDataUser + "UploadFile/" + mapData.No + "/";
                    //return "err@获取路径错误" + ex.Message + ",配置的路径是:" + savePath + ",您需要在附件属性上修改该附件的存储路径.";
                }

                try
                {
                    if (System.IO.Directory.Exists(savePath) == false)
                        System.IO.Directory.CreateDirectory(savePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("err@创建路径出现错误，可能是没有权限或者路径配置有问题:" + savePath + "@异常信息:" + ex.Message);
                }

                
                string guid = DBAccess.GenerGUID();



                string realSaveTo = savePath + "/" + guid + "." + fileName;

                realSaveTo = realSaveTo.Replace("~", "-");
                realSaveTo = realSaveTo.Replace("'", "-");
                realSaveTo = realSaveTo.Replace("*", "-");
                
                if (fileEncrypt == true)
                { 
                    string strtmp = realSaveTo + ".tmp";
                    Base64StrToImage(fileSoruce, strtmp);
                    EncHelper.EncryptDES(strtmp, strtmp.Replace(".tmp", ""));//加密
                    File.Delete(strtmp);//删除临时文件
                }
                else
                {
                    //文件保存的路径
                    Base64StrToImage(fileSoruce, realSaveTo);
                }

                //执行附件上传前事件，added by liuxc,2017-7-15
                msg = ExecEvent.DoFrm(mapData, EventListFrm.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + realSaveTo);
                if (!DataType.IsNullOrEmpty(msg))
                {
                    BP.Sys.Base.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" + fileName + "，" + msg);
                    File.Delete(realSaveTo);
                   
                }

                FileInfo info = new FileInfo(realSaveTo);
                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.setMyPK(guid); 
                dbUpload.NodeID = this.FK_Node;
                dbUpload.Sort = sort;
                dbUpload.setFK_MapData(athDesc.FK_MapData);
                dbUpload.FK_FrmAttachment = attachPk;
                dbUpload.FileExts = info.Extension;
                dbUpload.FID = this.FID;
                if (fileEncrypt == true)
                    dbUpload.SetPara("IsEncrypt", 1);
                dbUpload.FileFullName = realSaveTo;
                dbUpload.FileName = fileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDateTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.FK_Dept = WebUser.FK_Dept;
                dbUpload.FK_DeptName = WebUser.FK_DeptName;
                dbUpload.RefPKVal = pkVal;
                dbUpload.FID = this.FID;

                dbUpload.UploadGUID = guid;
                dbUpload.Insert();
                //执行附件上传后事件，added by liuxc,2017-7-15
                msg = ExecEvent.DoFrm(mapData, EventListFrm.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + dbUpload.FileFullName);
                if (!DataType.IsNullOrEmpty(msg))
                    BP.Sys.Base.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);
            }
            #endregion 文件上传的iis服务器上 or db数据库里.

            #region 保存到数据库 / FTP服务器上.
            if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp =  BP.Difference.SystemConfig.PathOfTemp + "" + guid + ".tmp";

                if (fileEncrypt == true)
                {
                    string strtmp =  BP.Difference.SystemConfig.PathOfTemp + "" + guid + "_Desc" + ".tmp";
                    Base64StrToImage(fileSoruce, strtmp);
                    EncHelper.EncryptDES(strtmp, temp);//加密
                    File.Delete(strtmp);//删除临时文件
                }
                else
                {
                    //文件保存的路径
                    Base64StrToImage(fileSoruce, temp);
                }

                //执行附件上传前事件，added by liuxc,2017-7-15
                msg = ExecEvent.DoFrm(mapData, EventListFrm.AthUploadeBefore, en, "@FK_FrmAttachment=" + athDesc.MyPK + "@FileFullName=" + temp);
                if (DataType.IsNullOrEmpty(msg) == false)
                {
                    BP.Sys.Base.Glo.WriteLineError("@AthUploadeBefore事件返回信息，文件：" +fileName + "，" + msg);
                    File.Delete(temp);

                    throw new Exception("err@上传附件错误：" + msg);
                }

                FileInfo info = new FileInfo(temp);
                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.setMyPK(DBAccess.GenerGUID());
                dbUpload.Sort = sort;
                dbUpload.NodeID = FK_Node;
                dbUpload.setFK_MapData(athDesc.FK_MapData);
                dbUpload.FK_FrmAttachment = athDesc.MyPK;
                dbUpload.FID = this.FID; //流程id.
                if (fileEncrypt == true)
                    dbUpload.SetPara("IsEncrypt", 1);

                dbUpload.RefPKVal = pkVal.ToString();
                dbUpload.setFK_MapData(athDesc.FK_MapData);
                dbUpload.FK_FrmAttachment = athDesc.MyPK;
                dbUpload.FileName = fileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDateTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.FK_Dept = WebUser.FK_Dept;
                dbUpload.FK_DeptName = WebUser.FK_DeptName;
                
                dbUpload.UploadGUID = guid;

                
                if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    /*保存到fpt服务器上.*/
                    BP.FtpConnection ftpconn = new BP.FtpConnection(BP.Difference.SystemConfig.FTPServerIP,
                        SystemConfig.FTPServerPort,
                        SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

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
                    try
                    {
                        ftpconn.PutFile(temp, guid + "." + dbUpload.FileExts);
                    }
                    catch
                    {
                        throw new Exception("err@FTP端口号受限或者防火墙未关闭");
                    }
                    ftpconn.Close();

                    //设置路径.
                    dbUpload.FileFullName = ny + "//" + athDesc.FK_MapData + "//" + guid + "." + dbUpload.FileExts;
                    dbUpload.Insert();
                    File.Delete(temp);
                }


                //执行附件上传后事件，added by liuxc,2017-7-15
                msg = ExecEvent.DoFrm(mapData, EventListFrm.AthUploadeAfter, en, "@FK_FrmAttachment=" + dbUpload.FK_FrmAttachment + "@FK_FrmAttachmentDB=" + dbUpload.MyPK + "@FileFullName=" + temp);
                if (DataType.IsNullOrEmpty(msg) == false)
                    BP.Sys.Base.Glo.WriteLineError("@AthUploadeAfter事件返回信息，文件：" + dbUpload.FileName + "，" + msg);

            }
            #endregion 保存到数据库.
            return "上传成功";
        }


        public  void Base64StrToImage(string base64Str, string savePath)
        {
            var ret = true;
            base64Str = base64Str.Replace(" ", "+");
            string[] str = base64Str.Split(',');  //base64Str为base64完整的字符串，先处理一下得到我们所需要的字符串
            byte[] imageBytes = Convert.FromBase64String(str[1]);
            //读入MemoryStream对象
            MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            memoryStream.Write(imageBytes, 0, imageBytes.Length);
            //  转成图片
            Image image = Image.FromStream(memoryStream);
            //   图片名称
            string iname = DateTime.Now.ToString("yyMMddhhmmss");
            image.Save(savePath);  // 将图片存到本地

        }

            /// <summary>
            /// 获取百度云token
            /// </summary>
            /// <returns></returns>
            public string getAccessToken()
        {
            string ak =  BP.Difference.SystemConfig.APIKey;
            string sk =  BP.Difference.SystemConfig.SecretKey;

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

        public string GetAccessTokenImgs()
        {
            //获取 AccessToken
            return BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
        }

        /// <summary>
        /// 下载微信服务器图片，上传到应用服务器
        /// </summary>
        /// <param name="mideaid"></param>
        public string MyFlowGener_SaveUploadeImg()
        {
            try
            {
                string media_id = this.GetRequestVal("IDs");
                string athMyPK = this.GetRequestVal("AthMyPK"); //图片组件.

                FrmAttachment athDesc = new FrmAttachment(athMyPK);

                if (string.IsNullOrEmpty(media_id))
                {
                    return "media_id为空";
                }
                string accessToken = GetAccessTokenImgs();
                //BP.DA.Log.DebugWriteError("accessToken:" + accessToken);
                Bitmap img = null;
                HttpWebRequest req;
                HttpWebResponse res = null;
               
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", accessToken, media_id);

                System.Uri httpUrl = new System.Uri(url);
                req = (HttpWebRequest)(WebRequest.Create(httpUrl));
                req.Timeout = 180000; //设置超时值10秒
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                req.Method = "GET";
                res = (HttpWebResponse)(req.GetResponse());
                img = new Bitmap(res.GetResponseStream());//获取图片流    
                //BP.DA.Log.DebugWriteError(res.GetResponseHeader("Content-Type"));
                string fileName = res.GetResponseHeader("Content-Disposition").Replace("attachment; filename=", "").Replace("\"", "");
                  
                img.Save(BP.Difference.SystemConfig.PathOfTemp + fileName);
                BP.DA.Log.DebugWriteError(this.FK_Node+":"+this.FK_Flow + ":" + this.WorkID + ":" +
                    athDesc.NoOfObj + ":" + athDesc.FK_MapData + ":" + BP.Difference.SystemConfig.PathOfTemp + fileName + ":" + fileName);
                BP.WF.CCFormAPI.CCForm_AddAth(this.FK_Node,this.FK_Flow, this.WorkID,
                    athMyPK, athDesc.FK_MapData, BP.Difference.SystemConfig.PathOfTemp + fileName, fileName);

                return "执行成功";
            }
            catch (Exception ex)
            {
                string msg = "err@GetMedia:" + ex.Message + " -- " + ex.StackTrace;
                BP.DA.Log.DebugWriteError(msg);
                return msg;

            }
        }
        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
        /// <summary>
        /// 调用企业号获取地理位置
        /// </summary>
        /// <returns></returns>
        public string GetWXConfigSetting()
        {
            string htmlPage = this.GetRequestVal("htmlPage");
            Hashtable ht = new Hashtable();

            //生成签名的时间戳
            string timestamp = DateTime.Now.ToString("yyyyMMDDHHddss");
            //生成签名的随机串
            string nonceStr = BP.DA.DBAccess.GenerGUID();
            //企业号jsapi_ticket
            string jsapi_ticket = "";
            string url1 = htmlPage;
            //获取 AccessToken
            string accessToken =  BP.GPM.WeiXin.WeiXinEntity.getAccessToken();

            string url = "https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + accessToken;


            HttpWebResponse response = new HttpWebResponseUtility().CreateGetHttpResponse(url, 10000, null, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();

            //权限签名算法
            BP.GPM.WeiXin.Ticket ticket = new BP.GPM.WeiXin.Ticket();
            ticket = FormatToJson.ParseFromJson<BP.GPM.WeiXin.Ticket>(str);

            if (ticket.errcode == "0")
                jsapi_ticket = ticket.ticket;
            else
                return "err:@获取jsapi_ticket失败+accessToken=" + accessToken;

            ht.Add("timestamp", timestamp);
            ht.Add("nonceStr", nonceStr);
            //企业微信的corpID
            ht.Add("AppID", BP.Difference.SystemConfig.WX_CorpID);

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
