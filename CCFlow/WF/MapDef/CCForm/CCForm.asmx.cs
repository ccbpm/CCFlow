using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Services;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Tools;
using BP.WF;
using BP.WF.Template;
using System.Xml;

namespace BP.Web
{
    /// <summary>
    /// DA 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CCForm : System.Web.Services.WebService
    {
        #region 公用方法

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [WebMethod]
        public string CfgKey(string key, string UserNo, string SID)
        {
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");
            return BP.Sys.SystemConfig.AppSettings[key];
        }
        /// <summary>
        /// 上传多附件
        /// </summary>
        /// <param name="WorkID">业务编号</param>
        /// <param name="FK_Node">节点编号</param>
        /// <param name="FK_FrmAttachment">控件ID</param>
        /// <param name="UserNo">上传人账号</param>
        /// <param name="UserName">上传人中文名</param>
        /// <param name="FileByte">文件大小</param>
        /// <param name="fileName">文件名，不能为空</param>
        /// <param name="MyNote">备注，可以为空</param>
        /// <param name="sort">排序，可以为空</param>
        /// <returns>文件名</returns>
        [WebMethod(EnableSession = true)]
        public string AttachmentFiles(string WorkID, string FK_Node, string FK_FrmAttachment, string UserNo, string UserName, byte[] FileByte, String fileName, string MyNote, string sort)
        {

            try
            {
                string guid = BP.DA.DBAccess.GenerGUID();
                string exts = fileName.Substring(fileName.IndexOf(".") + 1);
                FrmAttachment athDesc = new FrmAttachment(FK_FrmAttachment);
                //如果有上传类型限制，进行判断格式
                if (athDesc.Exts == "*.*" || athDesc.Exts == "")
                {
                    /*任何格式都可以上传*/
                }
                else
                {
                    if (athDesc.Exts.ToLower().Contains(exts) == false)
                    {
                        return "error:您上传的文件，不符合系统的格式要求，要求的文件格式:" + athDesc.Exts + "，您现在上传的文件格式为:" + exts;
                    }
                }

                //保存文件
                string realSaveTo = UploadFile(FileByte, fileName, athDesc.SaveTo);
                FileInfo info = new FileInfo(realSaveTo);

                FrmAttachmentDB dbUpload = new FrmAttachmentDB();

                dbUpload.MyPK = guid;
                dbUpload.NodeID = FK_Node.ToString();

                if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                {
                    /*如果是继承，就让他保持本地的PK. */
                    dbUpload.RefPKVal = WorkID;
                }

                if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                {
                    /*如果是协同，就让他是PWorkID. */
                    string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + WorkID, 0).ToString();
                    if (pWorkID == null || pWorkID == "0")
                        pWorkID = WorkID;

                    dbUpload.RefPKVal = pWorkID;
                }

                dbUpload.FK_MapData = athDesc.FK_MapData;
                dbUpload.FK_FrmAttachment = FK_FrmAttachment;

                dbUpload.FileExts = info.Extension;
                dbUpload.FileFullName = realSaveTo;
                dbUpload.FileName = info.Name;
                dbUpload.FileSize = (float)info.Length;

                dbUpload.RDT = DataType.CurrentDataTimess;
                dbUpload.Rec = UserNo;
                dbUpload.RecName = UserName;
                if (athDesc.IsNote)
                    dbUpload.MyNote = MyNote;

                if (!string.IsNullOrEmpty(sort))
                    dbUpload.Sort = sort;

                dbUpload.UploadGUID = guid;
                dbUpload.Insert();
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
            return fileName;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="FileByte">文件大小</param>
        /// <param name="fileName">文件名</param>
        /// <param name="saveTo">保存路径</param>
        /// <returns></returns>
        public string UploadFile(byte[] FileByte, String fileName, string saveTo)
        {
            //创建目录.

            string pathSave = saveTo;
            if (!System.IO.Directory.Exists(pathSave))
                System.IO.Directory.CreateDirectory(pathSave);

            string filePath = pathSave + fileName;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            //这里使用绝对路径来索引
            FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            stream.Write(FileByte, 0, FileByte.Length);
            stream.Close();

            //DataSet ds = new DataSet();
            //ds.ReadXml(filePath);

            return filePath;
        }
        /// <summary>
        /// 上传文件.
        /// </summary>
        /// <param name="FileByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [WebMethod]
        public string UploadFile(byte[] FileByte, String fileName)
        {
            //创建临时目录.
            string pathTemp = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp";
            if (!System.IO.Directory.Exists(pathTemp))
                System.IO.Directory.CreateDirectory(pathTemp);

            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string filePath = path + "\\" + fileName;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            //这里使用绝对路径来索引
            FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            stream.Write(FileByte, 0, FileByte.Length);
            stream.Close();

            DataSet ds = new DataSet();
            ds.ReadXml(filePath);

            string strs = string.Empty;
            strs = FormatToJson.ToJson(ds);
            //strs =  Silverlight.DataSetConnector.Connector.ToXml(ds);
            return strs;
        }
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int RunSQL(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            return BP.DA.DBAccess.RunSQL(sql);
        }

        [WebMethod(EnableSession = true)]
        public int RunSQLs(string sqls, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            if (string.IsNullOrEmpty(sqls))
                return 0;

            int i = 0;
            string[] strs = sqls.Split('@');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                i += BP.DA.DBAccess.RunSQL(str);
            }
            return i;
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTable(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            DataSet ds = new DataSet();
            ds.Tables.Add(BP.DA.DBAccess.RunSQLReturnTable(sql));
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnJson(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return BP.DA.DataType.ToJson(ds.Tables[0]);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public DataTable RunSQLReturnTable2DataTable(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 运行sql返回String.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnString(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            return BP.DA.DBAccess.RunSQLReturnString(sql);
        }
        /// <summary>
        /// 运行sql返回String.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public int RunSQLReturnValInt(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 运行sql返回float.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public float RunSQLReturnValFloat(string sql, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                return 0;

            return BP.DA.DBAccess.RunSQLReturnValFloat(sql);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [WebMethod]
        public string RunSQLReturnTableS(string sqls, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            string[] strs = sqls.Split('@');
            DataSet ds = new DataSet();
            int i = 0;
            foreach (string sql in strs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "DT" + i;
                ds.Tables.Add(dt);
                i++;
            }
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }
        /// <summary>
        /// 将中文转化成拼音.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flag">true: 全拼，false : 简拼</param>
        /// <returns></returns>
        [WebMethod]
        public string ParseStringToPinyin(string name, bool flag)
        {
            string s = string.Empty; ;
            try
            {
                if (flag)
                {
                    s = BP.DA.DataType.ParseStringToPinyin(name);
                    if (s.Length > 15)
                        s = BP.DA.DataType.ParseStringToPinyinWordFirst(name);
                }
                else
                {
                    s = BP.DA.DataType.ParseStringToPinyinJianXie(name);
                }

                s = s.Trim().Replace(" ", "");
                s = s.Trim().Replace(" ", "");
                //常见符号
                s = s.Replace(",", "").Replace(".", "").Replace("，", "").Replace("。", "").Replace("!", "");
                s = s.Replace("*", "").Replace("@", "").Replace("#", "").Replace("~", "").Replace("|", "");
                s = s.Replace("$", "").Replace("%", "").Replace("&", "").Replace("（", "").Replace("）", "").Replace("【", "").Replace("】", "");
                s = s.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("/", "");
                if (s.Length > 0)
                {
                    //去除开头数字
                    int iHead = 0;
                    string headStr = s.Substring(0, 1);
                    while (int.TryParse(headStr, out iHead) == true)
                    {
                        //替换为空
                        s = s.Substring(1);
                        if (s.Length > 0) headStr = s.Substring(0, 1);
                    }
                }
                return s;
            }
            catch
            {
                return null;
            }
        }

        private string DealPK(string pk, string fromMapdata, string toMapdata)
        {
            if (pk.Contains("*" + fromMapdata))
                return pk.Replace("*" + toMapdata, "*" + toMapdata);
            else
                return pk + "*" + toMapdata;
        }
        public void LetAdminLogin()
        {
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp);
        }
        /// <summary>
        /// 让他登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sid"></param>
        public void LetUserLogin(string user, string sid)
        {
            BP.Port.Emp emp = new BP.Port.Emp(user);
            BP.Web.WebUser.SignInOfGener(emp);
        }

        /// <summary>
        /// 保存Enum.
        /// </summary>
        /// <param name="enumKey"></param>
        /// <param name="enumLab"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [WebMethod]
        public string SaveEnum(string enumKey, string enumLab, string cfg)
        {
            SysEnumMain sem = new SysEnumMain();
            sem.No = enumKey;
            if (sem.RetrieveFromDBSources() == 0)
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = WebUser.SysLang;
                sem.Insert();
            }
            else
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = WebUser.SysLang;
                sem.Update();
            }

            string[] strs = cfg.Split('@');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                string[] kvs = str.Split('=');
                SysEnum se = new SysEnum();
                se.MyPK = enumKey + "_" + WebUser.SysLang + "_" + kvs[0];
                se.EnumKey = enumKey;
                se.Lang = WebUser.SysLang;
                se.IntKey = int.Parse(kvs[0]);
                se.Lab = kvs[1];
                se.Save();
            }
            return "save ok.";
        }

        /// <summary>
        /// 保存枚举字段
        /// </summary>
        /// <param name="fk_mapData">表单ID</param>
        /// <param name="fieldKey">字段值</param>
        /// <param name="fieldName">名</param>
        /// <param name="enumKey">枚举值</param>
        /// <returns>是否保存成功</returns>
        [WebMethod]
        public string SaveEnumField(string fk_mapData, string fieldKey, string fieldName, string enumKey, double x, double y)
        {
            try
            {
                MapAttr attr = new MapAttr();
                attr.MyPK = fk_mapData + "_" + fieldKey;
                if (attr.IsExits == true)
                    return "字段{" + fieldKey + "}已经存在.";

                attr.FK_MapData = fk_mapData;
                attr.KeyOfEn = fieldKey;
                attr.Name = fieldName;
                attr.MyDataType = DataType.AppInt;

                attr.X = float.Parse(x.ToString());
                attr.Y = float.Parse(y.ToString());

                attr.UIBindKey = enumKey;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.Insert();

                //在数据库里增加相应的字段,等到修复表的时候创建报错误.
                MapData md = new MapData(attr.FK_MapData);
                GEEntity ge = md.HisGEEn;
                ge.CheckPhysicsTable(); //创建该字段.

                return "OK";
            }
            catch (Exception ex)
            {
                return "@错误信息:" + ex.Message;
                // +" StackTrace:" + ex.StackTrace;
                //                return "@错误信息:" + ex.Message + " StackTrace:" + ex.StackTrace;
            }
        }
        /// <summary>
        /// 保存外键字段
        /// </summary>
        /// <param name="fk_mapData">表单ID</param>
        /// <param name="fieldKey">字段值</param>
        /// <param name="fieldName">名</param>
        /// <param name="enName">枚举值</param>
        /// <returns>是否保存成功</returns>
        [WebMethod]
        public string SaveFKField(string fk_mapData, string fieldKey, string fieldName, string enName, double x, double y)
        {
            try
            {
                // 插入影子字段.
                SFTable sf = new SFTable(enName);

                //如果是外键字段，插入后返回。
                if (sf.SrcType == SrcType.TableOrView)
                {
                    MapAttr attr = new MapAttr();
                    attr.MyPK = fk_mapData + "_" + fieldKey;
                    if (attr.IsExits == true)
                        return "字段{" + fieldKey + "}已经存在.";

                    attr.FK_MapData = fk_mapData;
                    attr.KeyOfEn = fieldKey;
                    attr.Name = fieldName;
                    attr.MyDataType = DataType.AppString;

                    attr.X = float.Parse(x.ToString());
                    attr.Y = float.Parse(y.ToString());

                    attr.UIBindKey = enName;
                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.FK;
                    attr.Insert(); //执行保存.

                    //在数据库里增加相应的字段,等到修复表的时候创建报错误.
                    MapData md = new MapData(attr.FK_MapData);
                    GEEntity ge = md.HisGEEn;
                    ge.CheckPhysicsTable(); //创建该字段.
                    return "OK";
                }

                
                // 外部数据或者WS数据.
                MapAttr myattr = new MapAttr();
                myattr.MyPK = fk_mapData + "_" + fieldKey;
                if (myattr.IsExits == true)
                    return "字段{" + fieldKey + "}已经存在.";

                myattr.FK_MapData = fk_mapData;
                myattr.KeyOfEn = fieldKey;
                myattr.Name = fieldName;
                myattr.MyDataType = DataType.AppString;

                myattr.X = float.Parse(x.ToString());
                myattr.Y = float.Parse(y.ToString());

                myattr.UIBindKey = enName;
                myattr.UIContralType = UIContralType.DDL;
                myattr.LGType = FieldTypeS.Normal;
                myattr.Insert(); //执行保存.

                //插入影子字段.
                myattr.MyPK = fk_mapData + "_" + fieldKey+"T";
                myattr.KeyOfEn = fieldKey + "T";
                myattr.UIContralType = UIContralType.TB;
                myattr.UIVisible = false;
                myattr.UIIsEnable = false; // 让其不是隐藏字段.
                myattr.Insert();

                //在数据库里增加相应的字段,等到修复表的时候创建报错误.
                MapData mymd = new MapData(myattr.FK_MapData);
                GEEntity myge = mymd.HisGEEn;
                myge.CheckPhysicsTable(); //创建该字段.
                

#warning 被周朋删除.
                ////判断此选中的数据源项是否是WebService数据源项，如果是，则增加一个attr.KeyOfEn + "Text"为名的隐藏字段
                ////added by liuxc,2015.9.14
                //if (isWS)
                //{
                //    attr = new MapAttr();
                //    attr.MyPK = fk_mapData + "_" + fieldKey + "Text";
                //    if (!attr.IsExits)
                //    {
                //        attr.FK_MapData = fk_mapData;
                //        attr.KeyOfEn = fieldKey + "Text";
                //        attr.Name = fieldName + "文本";
                //        attr.MyDataType = DataType.AppString;

                //        attr.X = float.Parse(x.ToString());
                //        attr.Y = float.Parse(y.ToString());

                //        attr.UIContralType = UIContralType.TB;
                //        attr.LGType = FieldTypeS.Normal;

                //        attr.UIVisible = false;
                //        attr.HisEditType = EditType.Edit;
                //        attr.Insert();
                //    }
                //}

                return "OK";
            }
            catch (Exception ex)
            {
                return "@错误信息:" + ex.Message;
            }
        }
        #endregion

        [WebMethod]
        public string SaveImageAsFile(byte[] img, string pkval, string fk_Frm_Ele)
        {
            FrmEle fe = new FrmEle(fk_Frm_Ele);
            System.Drawing.Image newImage;
            using (MemoryStream ms = new MemoryStream(img, 0, img.Length))
            {
                ms.Write(img, 0, img.Length);
                newImage = Image.FromStream(ms, true);
                Bitmap bitmap = new Bitmap(newImage, new Size(fe.WOfInt, fe.HOfInt));

                if (System.IO.Directory.Exists(fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\") == false)
                    System.IO.Directory.CreateDirectory(fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\");

                string saveTo = fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\" + pkval + ".jpg";
                bitmap.Save(saveTo, ImageFormat.Jpeg);

                string pathFile = System.Web.HttpContext.Current.Request.ApplicationPath + fe.HandSiganture_UrlPath + fe.FK_MapData + "/" + pkval + ".jpg";
                FrmEleDB ele = new FrmEleDB();
                ele.FK_MapData = fe.FK_MapData;
                ele.EleID = fe.EleID;
                ele.RefPKVal = pkval;
                ele.Tag1 = pathFile.Replace("\\\\", "\\");
                ele.Tag1 = pathFile.Replace("////", "//");

                ele.Tag2 = saveTo.Replace("\\\\", "\\");
                ele.Tag2 = saveTo.Replace("////", "//");

                ele.GenerPKVal();
                ele.Save();

                return pathFile;
                // return "../DataUser/" + realpath + strFileName + ".png";
            }
            //FrmEleDB db = new FrmEleDB();
            //db.MyPK= 
            //return "error";
        }

        /// <summary>
        /// 装载表单模板
        /// </summary>
        /// <param name="fileByte">字节数</param>
        /// <param name="fk_mapData"></param>
        /// <param name="isClear"></param>
        /// <returns></returns>
        [WebMethod]
        public string LoadFrmTemplete(byte[] fileByte, string fk_mapData, bool isClear)
        {
            try
            {

                string file = "\\Temp\\" + fk_mapData + ".xml";
                UploadFile(fileByte, file);
                string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file;
                this.LoadFrmTempleteFile(path, fk_mapData, isClear, true);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="fk_mapData">表单ID</param>
        /// <param name="isClear">是否清除现有的元素</param>
        /// <param name="isSetReadonly">是否设置的只读？</param>
        /// <returns>导入结果</returns>
        [WebMethod]
        public string LoadFrmTempleteFile(string file, string fk_mapData, bool isClear, bool isSetReadonly)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(file);
                MapData.ImpMapData(fk_mapData, ds, isSetReadonly);
                if (fk_mapData.Contains("ND"))
                {
                    /* 判断是否是节点表单 */
                    int nodeID = 0;
                    try
                    {
                        nodeID = int.Parse(fk_mapData.Replace("ND", ""));
                    }
                    catch
                    {
                        return null;
                    }
                    Node nd = new Node(nodeID);
                    nd.RepareMap();
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取xml数据
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetXmlData(string xmlFileName)
        {
            DataTable dt = new DataTable("o");
            dt.Columns.Add(new DataColumn("DFor"));
            dt.Columns.Add(new DataColumn("Tag1"));
            dt.Columns.Add(new DataColumn("Tag2"));
            dt.Columns.Add(new DataColumn("Tag3"));
            dt.Columns.Add(new DataColumn("Tag4"));

            DataRow dr = dt.NewRow();
            dr["DFor"] = "HandSiganture";
            dr["Tag1"] = "@Label=存储路径@FType=String@DefVal=D:\\ccflow\\VisualFlow\\DataUser\\BPPaint\\";
            dr["Tag2"] = "@Label=窗口打开高度@FType=Int@DefVal=500";
            dr["Tag3"] = "@Label=窗口打开宽度@FType=Int@DefVal=200";
            dr["Tag4"] = "@Label=UrlPath@FType=String@DefVal=/DataUser/BPPaint/";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["DFor"] = "EleSiganture";
            dr["Tag1"] = "@Label=位置@FType=String";
            dr["Tag2"] = "@Label=高度@FType=Int";
            dr["Tag3"] = "@Label=宽度@FType=Int";
            dr["Tag4"] = "";
            dt.Rows.Add(dr);

            DataSet myds = new DataSet();
            myds.Tables.Add(dt);
            return Silverlight.DataSetConnector.Connector.ToXml(myds);
        }
        [WebMethod]
        public string DoType(string dotype, string v1, string v2, string v3, string v4, string v5)
        {
            string sql = "";
            try
            {
                switch (dotype)
                {
                    case "CreateCheckGroup":
                        string gKey = v1;
                        string gName = v2;
                        string enName1 = v3;

                        MapAttr attrN = new MapAttr();
                        int i = attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_Note");
                        i += attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_Checker");
                        i += attrN.Retrieve(MapAttrAttr.FK_MapData, enName1, MapAttrAttr.KeyOfEn, gKey + "_RDT");
                        if (i > 0)
                            return "前缀已经使用：" + gKey + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";

                        GroupField gf = new GroupField();
                        gf.Lab = gName;
                        gf.EnName = enName1;
                        gf.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_Note";
                        attrN.Name = "审核意见";
                        attrN.MyDataType = DataType.AppString;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = true;
                        attrN.MaxLen = 4000;
                        attrN.GroupID = gf.OID;
                        attrN.UIHeight = 23 * 3;
                        attrN.Idx = 1;
                        attrN.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_Checker";
                        attrN.Name = "审核人";// "审核人";
                        attrN.MyDataType = DataType.AppString;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.MaxLen = 50;
                        attrN.MinLen = 0;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = false;
                        attrN.DefVal = "@WebUser.No";
                        attrN.UIIsEnable = false;
                        attrN.GroupID = gf.OID;
                        attrN.IsSigan = true;
                        attrN.Idx = 2;
                        attrN.Insert();

                        attrN = new MapAttr();
                        attrN.FK_MapData = enName1;
                        attrN.KeyOfEn = gKey + "_RDT";
                        attrN.Name = "审核日期"; // "审核日期";
                        attrN.MyDataType = DataType.AppDateTime;
                        attrN.UIContralType = UIContralType.TB;
                        attrN.UIIsEnable = true;
                        attrN.UIIsLine = false;
                        attrN.DefVal = "@RDT";
                        attrN.UIIsEnable = false;
                        attrN.GroupID = gf.OID;
                        attrN.Idx = 3;
                        attrN.Insert();

                        /*
                         * 判断是否是节点设置的审核分组，如果是就为节点设置焦点字段。
                         */

                        string frmID = attrN.FK_MapData;
                        frmID = frmID.Replace("ND", "");
                        int nodeid = 0;
                        try
                        {
                            nodeid = int.Parse(frmID);
                        }
                        catch
                        {
                            //转化不成功就是不是节点表单字段.
                            return null;
                        }

                        Node nd = new Node();
                        nd.NodeID = nodeid;
                        if (nd.RetrieveFromDBSources() != 0)
                        {
                            if (string.IsNullOrEmpty(nd.FocusField) == false)
                                return null;

                            nd.FocusField = "@" + gKey + "_Note";
                            nd.Update();
                        }
                        return null;
                    case "NewDtl":
                        MapDtl dtlN = new MapDtl();
                        dtlN.No = v1;
                        if (dtlN.RetrieveFromDBSources() != 0)
                            return "从表已存在";
                        dtlN.Name = v1;
                        dtlN.FK_MapData = v2;
                        dtlN.PTable = v1;
                        dtlN.Insert();
                        dtlN.IntMapAttrs();
                        return null;
                    case "DelM2M":
                        MapM2M m2mDel = new MapM2M();
                        m2mDel.MyPK = v1;
                        m2mDel.Delete();
                        //M2M m2mData = new M2M();
                        //m2mData.Delete(M2MAttr.FK_MapData, v1);
                        return null;
                    case "NewAthM": // 新建 NewAthM. 
                        string fk_mapdataAth = v1;
                        string athName = v2;

                        BP.Sys.FrmAttachment athM = new FrmAttachment();
                        athM.MyPK = athName;
                        if (athM.IsExits)
                            return "多选名称:" + athName + "，已经存在。";

                        athM.X = float.Parse(v3);
                        athM.Y = float.Parse(v4);
                        athM.Name = "多文件上传";
                        athM.FK_MapData = fk_mapdataAth;
                        athM.Insert();
                        return null;
                    case "NewM2M":
                        string fk_mapdataM2M = v1;
                        string m2mName = v2;
                        MapM2M m2m = new MapM2M();
                        m2m.FK_MapData = v1;
                        m2m.NoOfObj = v2;
                        if (v3 == "0")
                        {
                            m2m.HisM2MType = M2MType.M2M;
                            m2m.Name = "新建一对多";
                        }
                        else
                        {
                            m2m.HisM2MType = M2MType.M2MM;
                            m2m.Name = "新建一对多多";
                        }

                        m2m.X = float.Parse(v4);
                        m2m.Y = float.Parse(v5);
                        m2m.MyPK = m2m.FK_MapData + "_" + m2m.NoOfObj;
                        if (m2m.IsExits)
                            return "多选名称:" + m2mName + "，已经存在。";
                        m2m.Insert();
                        return null;
                    case "DelEnum":

                        //删除空数据.
                        BP.DA.DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData Is null or FK_MapData='' ");

                        // 检查这个物理表是否被使用。
                        sql = "SELECT  * FROM Sys_MapAttr WHERE UIBindKey='" + v1 + "'";
                        DataTable dtEnum = DBAccess.RunSQLReturnTable(sql);
                        string msgDelEnum = "";
                        foreach (DataRow dr in dtEnum.Rows)
                        {
                            msgDelEnum += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                        }

                        if (msgDelEnum != "")
                            return "该枚举已经被如下字段所引用，您不能删除它。" + msgDelEnum;

                        sql = "DELETE FROM Sys_EnumMain WHERE No='" + v1 + "'";
                        sql += "@DELETE FROM Sys_Enum WHERE EnumKey='" + v1 + "' ";
                        DBAccess.RunSQLs(sql);
                        return null;
                    case "DelSFTable": /* 删除自定义的物理表. */
                        // 检查这个物理表是否被使用。
                        sql = "SELECT  * FROM Sys_MapAttr WHERE UIBindKey='" + v1 + "'";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        string msgDel = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            msgDel += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                        }

                        if (msgDel != "")
                            return "该数据表已经被如下字段所引用，您不能删除它。" + msgDel;

                        SFTable sfDel = new SFTable();
                        sfDel.No = v1;
                        sfDel.DirectDelete();
                        return null;
                    case "SaveSFTable":
                        string enName = v2;
                        string chName = v1;
                        if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
                            return "视图中的中英文名称不能为空。";

                        SFTable sf = new SFTable();
                        sf.No = enName;
                        sf.Name = chName;

                        sf.No = enName;
                        sf.Name = chName;

                        sf.FK_Val = enName;
                        sf.Save();
                        if (DBAccess.IsExitsObject(enName) == true)
                        {
                            /*已经存在此对象，检查一下是否有No,Name列。*/
                            sql = "SELECT No,Name FROM " + enName;
                            try
                            {
                                DBAccess.RunSQLReturnTable(sql);
                            }
                            catch (Exception ex)
                            {
                                return "您指定的表或视图(" + enName + ")，不包含No,Name两列，不符合ccflow约定的规则。技术信息:" + ex.Message;
                            }
                            return null;
                        }
                        else
                        {
                            /*创建这个表，并且插入基础数据。*/
                            try
                            {
                                // 如果没有该表或者视图，就要创建它。
                                sql = "CREATE TABLE " + enName + "(No varchar(30) NOT NULL,Name varchar(50) NULL)";
                                DBAccess.RunSQL(sql);
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('001','Item1')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('002','Item2')");
                                DBAccess.RunSQL("INSERT INTO " + enName + " (No,Name) VALUES('003','Item3')");
                            }
                            catch (Exception ex)
                            {
                                sf.DirectDelete();
                                return "创建物理表期间出现错误,可能是非法的物理表名.技术信息:" + ex.Message;
                            }
                        }
                        return null; /*创建成功后返回空值*/
                    case "FrmTempleteExp":  //导出表单.
                        MapData mdfrmtem = new MapData();
                        mdfrmtem.No = v1;
                        if (mdfrmtem.RetrieveFromDBSources() == 0)
                        {
                            if (v1.Contains("ND"))
                            {
                                int nodeId = int.Parse(v1.Replace("ND", ""));
                                Node nd123 = new Node(nodeId);
                                mdfrmtem.Name = nd123.Name;
                                mdfrmtem.PTable = v1;
                                mdfrmtem.EnPK = "OID";
                                mdfrmtem.Insert();
                            }
                        }

                        DataSet ds = mdfrmtem.GenerHisDataSet();
                        string file = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + v1 + ".xml";
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        ds.WriteXml(file);

                        // BP.Sys.PubClass.DownloadFile(file, mdfrmtem.Name + ".xml");
                        //this.DownLoadFile(System.Web.HttpContext.Current, file, mdfrmtem.Name);
                        return null;
                    case "FrmTempleteImp": //导入表单.
                        DataSet dsImp = new DataSet();
                        string fileImp = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + v1 + ".xml";
                        dsImp.ReadXml(fileImp); //读取文件.
                        MapData.ImpMapData(v1, dsImp, true);
                        return null;
                    case "NewHidF":
                        string fk_mapdataHid = v1;
                        string key = v2;
                        string name = v3;
                        int dataType = int.Parse(v4);
                        MapAttr mdHid = new MapAttr();
                        mdHid.MyPK = fk_mapdataHid + "_" + key;
                        mdHid.FK_MapData = fk_mapdataHid;
                        mdHid.KeyOfEn = key;
                        mdHid.Name = name;
                        mdHid.MyDataType = dataType;
                        mdHid.HisEditType = EditType.Edit;
                        mdHid.MaxLen = 100;
                        mdHid.MinLen = 0;
                        mdHid.LGType = FieldTypeS.Normal;
                        mdHid.UIVisible = false;
                        mdHid.UIIsEnable = false;
                        mdHid.Insert();
                        return null;
                    case "DelDtl":
                        MapDtl dtl = new MapDtl(v1);
                        dtl.Delete();
                        return null;
                    case "DelWorkCheck":

                        FrmWorkCheck check = new FrmWorkCheck();
                        check.No = v1;
                        check.Delete();
                        return null;
                    case "DeleteFrm":
                        string delFK_Frm = v1;
                        MapData mdDel = new MapData(delFK_Frm);
                        mdDel.Delete();
                        sql = "@DELETE FROM Sys_MapData WHERE No='" + delFK_Frm + "'";
                        sql = "@DELETE FROM WF_FrmNode WHERE FK_Frm='" + delFK_Frm + "'";
                        DBAccess.RunSQLs(sql);
                        return null;
                    case "FrmUp":
                    case "FrmDown":
                        FrmNode myfn = new FrmNode();
                        myfn.Retrieve(FrmNodeAttr.FK_Node, v1, FrmNodeAttr.FK_Frm, v2);
                        if (dotype == "FrmUp")
                            myfn.DoUp();
                        else
                            myfn.DoDown();
                        return null;
                    case "SaveFlowFrm":
                        // 转化参数意义.
                        string vals = v1;
                        string fk_Node = v2;
                        string fk_flow = v3;
                        bool isPrint = false;
                        if (v5 == "1")
                            isPrint = true;

                        bool isReadonly = false;
                        if (v4 == "1")
                            isReadonly = true;

                        string msg = this.SaveEn(vals);
                        if (msg.Contains("Error"))
                            return msg;

                        string fk_frm = msg;
                        Frm fm = new Frm();
                        fm.No = fk_frm;
                        fm.Retrieve();

                        FrmNode fn = new FrmNode();
                        if (fn.Retrieve(FrmNodeAttr.FK_Frm, fk_frm,
                            FrmNodeAttr.FK_Node, fk_Node) == 1)
                        {
                            fn.IsEdit = !isReadonly;
                            fn.IsPrint = isPrint;
                            fn.FK_Flow = fk_flow;
                            fn.Update();
                            BP.DA.DBAccess.RunSQL("UPDATE Sys_MapData SET FK_FrmSort='01',AppType=1  WHERE No='" + fk_frm + "'");
                            return fk_frm;
                        }

                        fn.FK_Frm = fk_frm;
                        fn.FK_Flow = fk_flow;
                        fn.FK_Node = int.Parse(fk_Node);
                        fn.IsEdit = !isReadonly;
                        fn.IsPrint = isPrint;
                        fn.Idx = 100;
                        fn.FK_Flow = fk_flow;
                        fn.Insert();

                        MapData md = new MapData();
                        md.No = fm.No;
                        if (md.RetrieveFromDBSources() == 0)
                        {
                            md.Name = fm.Name;
                            md.EnPK = "OID";
                            md.Insert();

                        }

                        MapAttr attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "OID";
                        attr.Name = "WorkID";
                        attr.MyDataType = BP.DA.DataType.AppInt;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "0";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();

                        attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "FID";
                        attr.Name = "FID";
                        attr.MyDataType = BP.DA.DataType.AppInt;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "0";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();

                        attr = new MapAttr();
                        attr.FK_MapData = md.No;
                        attr.KeyOfEn = "RDT";
                        attr.Name = "记录日期";
                        attr.MyDataType = BP.DA.DataType.AppDateTime;
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.UIIsEnable = false;
                        attr.DefVal = "@RDT";
                        attr.HisEditType = BP.En.EditType.Readonly;
                        attr.Insert();
                        return fk_frm;
                    default:
                        return "Error:" + dotype + " , 未设置此标记.";
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }
        /// <summary>
        /// 保存entity.
        /// 事例: @EnName=BP.Sys.FrmLabel@PKVal=Lin13b@X=100@Y=299@Text=我的标签
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        [WebMethod]
        public string SaveEn(string vals)
        {
            Entity en = null;
            try
            {
                AtPara ap = new AtPara(vals);
                string enName = ap.GetValStrByKey("EnName");
                string pk = ap.GetValStrByKey("PKVal");
                en = ClassFactory.GetEn(enName);
                en.ResetDefaultVal();

                if (en == null)
                    throw new Exception("无效的类名:" + enName);

                if (string.IsNullOrEmpty(pk) == false)
                {
                    en.PKVal = pk;
                    en.RetrieveFromDBSources();
                }

                foreach (string key in ap.HisHT.Keys)
                {
                    if (key == "PKVal")
                        continue;
                    en.SetValByKey(key, ap.HisHT[key].ToString().Replace('^', '@'));
                }
                en.Save();
                return en.PKVal as string;
            }
            catch (Exception ex)
            {
                if (en != null)
                    en.CheckPhysicsTable();
                return "Error:" + ex.Message;
            }
        }

        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [WebMethod]
        public string FtpMethod(string doType, string v1, string v2, string v3)
        {
            try
            {
                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection("192.168.1.138", "administrator", "jiaozi");
                switch (doType)
                {
                    case "ShareFrm": /*共享模板*/
                        MapData md = new MapData();
                        DataSet ds = md.GenerHisDataSet();
                        string file = BP.Sys.SystemConfig.PathOfTemp + v1 + "_" + v2 + "_" + DateTime.Now.ToString("MM-dd hh-mm") + ".xml";
                        ds.WriteXml(file);
                        conn.SetCurrentDirectory("/");
                        conn.SetCurrentDirectory("/Upload.Form/");
                        conn.SetCurrentDirectory(v3);
                        conn.PutFile(file, md.Name + ".xml");
                        conn.Close();
                        return null;
                    case "GetDirs":
                        //   return "@01.日常办公@02.人力资源@03.其它类";
                        conn.SetCurrentDirectory(v1);
                        FtpSupport.Win32FindData[] dirs = conn.FindFiles();
                        conn.Close();
                        string dirsStr = "";
                        foreach (FtpSupport.Win32FindData dir in dirs)
                        {
                            dirsStr += "@" + dir.FileName;
                        }
                        return dirsStr;
                    case "GetFls":
                        conn.SetCurrentDirectory(v1);
                        FtpSupport.Win32FindData[] fls = conn.FindFiles();
                        conn.Close();
                        string myfls = "";
                        foreach (FtpSupport.Win32FindData fl in fls)
                        {
                            myfls += "@" + fl.FileName;
                        }
                        return myfls;
                    case "LoadTempleteFile":
                        string fileFtpPath = v1;
                        conn.SetCurrentDirectory("/Form.表单模版/");
                        conn.SetCurrentDirectory(v3);

                        /*下载文件到指定的目录: */
                        string tempFile = BP.Sys.SystemConfig.PathOfTemp + "\\" + v2 + ".xml";
                        conn.GetFile(v1, tempFile, false, FileAttributes.Archive, FtpSupport.FtpTransferType.Ascii);
                        return this.LoadFrmTempleteFile(tempFile, v2, true, true);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }
        /// <summary>
        /// 获取外键数据
        /// </summary>
        /// <param name="uiBindKey">外键</param>
        /// <returns>外键数据</returns>
        [WebMethod]
        public string RequestSFTableV1(string uiBindKey)
        {
            if (string.IsNullOrEmpty(uiBindKey))
                throw new Exception("@uiBindKey不能为空值.");

            DataSet ds = new DataSet();
            ds.Tables.Add(PubClass.GetDataTableByUIBineKeyForCCFormDesigner(uiBindKey));
            return Silverlight.DataSetConnector.Connector.ToXml(ds);
        }

        #region 产生 page 菜单
        public void InitFrm(string fk_mapdata)
        {
            try
            {
                BP.Sys.PubClass.InitFrm(fk_mapdata);
            }
            catch (Exception ex)
            {
                FrmLines lines = new FrmLines();
                lines.Delete(FrmLabAttr.FK_MapData, fk_mapdata);
                throw ex;
            }
        }
        private DataSet ds = null;
        /// <summary>
        /// 获取一个Frm
        /// </summary>
        /// <param name="fk_mapdata">map</param>
        /// <param name="workID">可以为0</param>
        /// <returns>form描述</returns>
        [WebMethod]
        public string GenerFrm(string fk_mapdata, int workID)
        {
            //// test
            //string fs = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("ND402.txt"));
            //return fs;

            try
            {
                this.ds = MapData.GenerHisDataSet(fk_mapdata);
                if (this.ds == null || this.ds.Tables.Count <= 0)
                {
                    MapData md = new MapData();
                    md.No = fk_mapdata;
                    if (md.RetrieveFromDBSources() == 0)
                    {
                        MapDtl dtl = new MapDtl();
                        dtl.No = fk_mapdata;
                        if (dtl.RetrieveFromDBSources() == 0)
                            throw new Exception("装载错误，该表单ID=" + fk_mapdata + "丢失，请修复一次流程重新加载一次.");
                        else
                        {
                            md.Copy(dtl);
                            md.DirectInsert();
                        }
                    }
                }

                string json = FormatToJson.ToJson(ds);
                //string xml =  Silverlight.DataSetConnector.Connector.ToXml(ds);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion 产生 frm

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromMapData"></param>
        /// <param name="fk_mapdata"></param>
        /// <param name="isClear">是否清除</param>
        /// <param name="isSetReadonly">是否设置为只读?</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string CopyFrm(string fromMapData, string fk_mapdata, bool isClear, bool isSetReadonly)
        {
            isSetReadonly = true;
            this.LetAdminLogin();

            MapData md = new MapData(fromMapData);
            MapData.ImpMapData(fk_mapdata, md.GenerHisDataSet(), isSetReadonly);

            // 如果是节点表单，就要执行一次修复，以免漏掉应该有的系统字段。
            if (fk_mapdata.Contains("ND") == true)
            {
                try
                {
                    string fk_node = fk_mapdata.Replace("ND", "");
                    Node nd = new Node(int.Parse(fk_node));
                    nd.RepareMap();
                }
                catch
                {
                    // 不处理异常。
                }
            }
            return null;
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="xml">xml串</param>
        /// <param name="sqls">要执行的SQL</param>
        /// <param name="mapAttrKeyName"></param>
        /// <returns></returns>
        [WebMethod]
        public string SaveFrm(string fk_mapdata, string xml, string sqls, string UserNo, string SID)
        {
            //验证用户未通过
            if (Dev2Interface.Port_CheckUserLogin(UserNo, SID) == false)
                throw new Exception("@非法用户.");

            DataSet ds = new DataSet();
            try
            {
                ds = FormatToJson.JsonToDataSet(xml);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            string str = "";
            foreach (DataTable dt in ds.Tables)
            {
                try
                {
                    str += SaveDT(dt);
                }
                catch (Exception ex)
                {
                    str += "@保存" + dt.TableName + "失败:" + ex.Message;
                }
            }

            #region 数据库兼容处理，把要执行的SQL 处理一下.
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sqls = sqls.Replace("LEN(", "LENGTH(");
            }
            else if(BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                sqls = sqls.Replace("LENGTH(", "LEN(");
            }

            sqls += "@UPDATE Sys_MapAttr SET Name='' WHERE FK_MapData='" + fk_mapdata + "'  AND Name IS NULL ";
            sqls += "@UPDATE Sys_MapAttr SET UIVisible=1 WHERE FK_MapData='" + fk_mapdata + "' AND UIVisible is null";
            sqls += "@UPDATE Sys_MapAttr SET UIIsEnable=1 WHERE FK_MapData='" + fk_mapdata + "' AND UIIsEnable is null";
            sqls += "@UPDATE Sys_MapAttr SET UIIsLine=0 WHERE FK_MapData='" + fk_mapdata + "' AND UIIsLine is null";
            #endregion 处理数据库兼容的问题

            //执行它，这些SQL 都是用@符号隔开的。
            RunSQLs(sqls, UserNo, SID);

            MapData md = new MapData();
            md.No = fk_mapdata;
            DataTable mapdata = ds.Tables["Sys_MapData"];
            if (mapdata != null && mapdata.Rows.Count == 1)
            {
                DataRow mdr = mapdata.Rows[0];
                md.RetrieveFromDBSources();

                md.Name = mdr["Name"].ToString();

                float tmp = 0;
                float.TryParse(mdr["FrmH"].ToString(), out tmp);
                md.FrmH = tmp <= 0 ? md.FrmH : tmp;

                float.TryParse(mdr["FrmW"].ToString(), out tmp);
                md.FrmW = tmp <= 0 ? md.FrmW : tmp;

                float.TryParse(mdr["MaxEnd"].ToString(), out tmp);
                md.MaxEnd = tmp <= 0 ? md.MaxEnd : tmp;

                float.TryParse(mdr["MaxLeft"].ToString(), out tmp);
                md.MaxLeft = tmp <= 0 ? md.MaxLeft : tmp;

                float.TryParse(mdr["MaxRight"].ToString(), out tmp);
                md.MaxRight = tmp == 0 ? md.MaxRight : tmp;

                float.TryParse(mdr["MaxTop"].ToString(), out tmp);
                md.MaxTop = tmp == 0 ? md.MaxTop : tmp;

                md.Update();
            }
            //md.ResetMaxMinXY();
            md.UpdateVer();
            //清空缓存，后面优化对一个缓存项进行清除
            BP.Sys.SystemConfig.DoClearCash_del();

            //md.DeleteFromCash();  ////移除缓存.

            // 备份文件
            CCFlow.WF.Admin.XAP.DoPort.WriteToXmlMapData(fk_mapdata, false);

            if (string.IsNullOrEmpty(str))
                return null;

            return str;
        }

        class TableSQL
        {
            public string PK;
            public string INSERTED;
            public string UPDATED;
        }

        const string SYS_MAPATTR = "SYS_MAPATTR";
        const string UIBINDKEY = "UIBINDKEY";

        static Dictionary<string, TableSQL> dicTableSql = new Dictionary<string, TableSQL>();
        TableSQL getTableSql(string tableName, DataColumnCollection columns = null)
        {
            TableSQL tableSql = null;
            if (dicTableSql.ContainsKey(tableName))
            {
                tableSql = dicTableSql[tableName];
            }
            else
            {
                if (columns != null && columns.Count > 0)
                {
                    string igF = "@RowIndex@RowState@";

                    #region gener sql.
                    //生成sqlUpdate
                    string sqlUpdate = "UPDATE " + tableName + " SET ";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName + "@"))
                            continue;

                        switch (dc.ColumnName)
                        {
                            case "MYPK":
                            case "OID":
                            case "NO":
                                continue;
                            default:
                                break;
                        }

                        if (tableName == SYS_MAPATTR && dc.ColumnName == UIBINDKEY)
                            continue;
                        try
                        {
                            sqlUpdate += dc.ColumnName + "=" + BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim() + ",";
                        }
                        catch
                        {
                        }
                    }
                    sqlUpdate = sqlUpdate.Substring(0, sqlUpdate.Length - 1);
                    string pk = "";
                    if (columns.Contains("NODEID"))
                        pk = "NODEID";
                    if (columns.Contains("MYPK"))
                        pk = "MYPK";
                    if (columns.Contains("OID"))
                        pk = "OID";
                    if (columns.Contains("NO"))
                        pk = "NO";

                    sqlUpdate += " WHERE " + pk + "=" + BP.Sys.SystemConfig.AppCenterDBVarStr + pk;

                    //生成sqlInsert
                    string sqlInsert = "INSERT INTO " + tableName + " ( ";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName.Trim() + "@"))
                            continue;

                        if (tableName == SYS_MAPATTR && dc.ColumnName.Trim() == UIBINDKEY)
                            continue;

                        sqlInsert += dc.ColumnName.Trim() + ",";
                    }
                    sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                    sqlInsert += ") VALUES (";
                    foreach (DataColumn dc in columns)
                    {
                        if (igF.Contains("@" + dc.ColumnName + "@"))
                            continue;

                        if (tableName == SYS_MAPATTR && dc.ColumnName.Trim() == UIBINDKEY)
                            continue;

                        sqlInsert += BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim() + ",";
                    }
                    sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                    sqlInsert += ")";
                    #endregion gener sql.

                    tableSql = new TableSQL();
                    tableSql.UPDATED = sqlUpdate;
                    tableSql.INSERTED = sqlInsert;
                    tableSql.PK = pk;

                    dicTableSql.Add(tableName, tableSql);
                }
            }

            return tableSql;
        }
        public string SaveDT(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return "";

            string tableName = dt.TableName.Replace("CopyOf", "").ToUpper();
            //MaxTop,MaxLeft,MaxRight,MaxEnd合并到AtPara
            if (tableName == "SYS_MAPDATA" && dt.Columns.Contains("MaxLeft") && dt.Columns.Contains("MaxTop"))
                return "";

            if (dt.Columns.Count == 0)
                return null;

            TableSQL sqls = getTableSql(tableName, dt.Columns);
            string updataSQL = sqls.UPDATED;
            string pk = sqls.PK;
            string insertSQL = sqls.INSERTED;

            #region save data.
            foreach (DataRow dr in dt.Rows)
            {
                BP.DA.Paras ps = new BP.DA.Paras();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName == pk)
                        continue;

                    if (tableName == SYS_MAPATTR && dc.ColumnName.Trim() == UIBINDKEY)
                        continue;

                    if (updataSQL.Contains(BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim()))
                        ps.Add(dc.ColumnName.Trim(), dr[dc.ColumnName.Trim()]);
                }

                ps.Add(pk, dr[pk]);
                ps.SQL = updataSQL;
                try
                {
                    if (BP.DA.DBAccess.RunSQL(ps) == 0)
                    {
                        ps.Clear();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (tableName == SYS_MAPATTR && dc.ColumnName == UIBINDKEY)
                                continue;

                            if (updataSQL.Contains(BP.Sys.SystemConfig.AppCenterDBVarStr + dc.ColumnName.Trim()))
                                ps.Add(dc.ColumnName.Trim(), dr[dc.ColumnName.Trim()]);
                        }
                        ps.SQL = insertSQL;
                        BP.DA.DBAccess.RunSQL(ps);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    string pastrs = "";
                    foreach (Para p in ps)
                    {
                        pastrs += "\t\n@" + p.ParaName + "=" + p.val;
                    }
                    throw new Exception("@执行sql=" + ps.SQL + "失败." + ex.Message + "\t\n@paras=" + pastrs);
                }
            }
            #endregion
            return null;
        }

    }
}
