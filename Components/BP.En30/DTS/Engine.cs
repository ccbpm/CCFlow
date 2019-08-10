using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Data;
using BP.En;
using BP.DA;
using BP.Port;
using BP.Sys;
using BP.Web;

namespace BP.Pub
{
    /// <summary>
    /// WebRtfReport 的摘要说明。
    /// </summary>
    public class RTFEngine
    {
        #region 数据实体
        private Entities _HisEns = null;
        public Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                    _HisEns = new Emps();

                return _HisEns;
            }
        }
        #endregion 数据实体

        #region 数据明细实体
        private System.Text.Encoding _encoder = System.Text.Encoding.GetEncoding("GB2312");

        public string GetCode(string str)
        {
            if (str == null || str == "")
                return "";

            string rtn = "";
            byte[] rr = _encoder.GetBytes(str);
            foreach (byte b in rr)
            {
                if (b > 122)
                    rtn += "\\'" + b.ToString("x");
                else
                    rtn += (char)b;
            }
            return rtn.Replace("\n", " \\par ");
        }
        //明细表数据
        private ArrayList _EnsDataDtls = null;
        public ArrayList EnsDataDtls
        {
            get
            {
                if (_EnsDataDtls == null)
                    _EnsDataDtls = new ArrayList();
                return _EnsDataDtls;
            }
        }
        //多附件数据
        private Hashtable _EnsDataAths = null;
        public Hashtable EnsDataAths
        {
            get
            {
                if (_EnsDataAths == null)
                    _EnsDataAths = new Hashtable();
                return _EnsDataAths;
            }
        }

        #endregion 数据明细实体

        /// <summary>
        /// 增加一个数据实体
        /// </summary>
        /// <param name="en"></param>
        public void AddEn(Entity en)
        {
            this.HisEns.AddEntity(en);
        }
        /// <summary>
        /// 增加一个Ens
        /// </summary>
        /// <param name="ens"></param>
        public void AddDtlEns(Entities dtlEns)
        {
            this.EnsDataDtls.Add(dtlEns);
        }
        public string CyclostyleFilePath = "";
        public string TempFilePath = "";

        #region 获取特殊要处理的流程节点信息.
        public string GetValueByKeyOfCheckNode(string[] strs)
        {
            foreach (Entity en in this.HisEns)
            {
                string val = en.GetValStringByKey(strs[2]);
                switch (strs.Length)
                {
                    case 1:
                    case 2:
                        throw new Exception("step1参数设置错误" + strs.ToString());
                    case 3: // S.9001002.Rec
                        return val;
                    case 4: // S.9001002.RDT.Year
                        switch (strs[3])
                        {
                            case "Text":
                                if (val == "0")
                                    return "否";
                                else
                                    return "是";
                            case "YesNo":
                                if (val == "1")
                                    return "[√]";
                                else
                                    return "[×]";
                            case "Year":
                                return val.Substring(0, 4);
                            case "Month":
                                return val.Substring(5, 2);
                            case "Day":
                                return val.Substring(8, 2);
                            case "NYR":
                                return DA.DataType.ParseSysDate2DateTime(val).ToString("yyyy年MM月dd日");
                            case "RMB":
                                return float.Parse(val).ToString("0.00");
                            case "RMBDX":
                                return DA.DataType.ParseFloatToCash(float.Parse(val));
                            default:
                                throw new Exception("step2参数设置错误" + strs);
                        }
                    default:
                        throw new Exception("step3参数设置错误" + strs);
                }
            }
            throw new Exception("step4参数设置错误" + strs);
        }
        public static string GetImgHexString(System.Drawing.Image img, System.Drawing.Imaging.ImageFormat ext)
        {
            StringBuilder imgs = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ext);
            stream.Close();

            byte[] buffer = stream.ToArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                if ((i % 32) == 0)
                {
                    imgs.AppendLine();
                }
                //else if ((i % 8) == 0)
                //{
                //    imgs.Append(" ");
                //}
                byte num2 = buffer[i];
                int num3 = (num2 & 240) >> 4;
                int num4 = num2 & 15;
                imgs.Append("0123456789abcdef"[num3]);
                imgs.Append("0123456789abcdef"[num4]);
            }
            return imgs.ToString();
        }
        public Entity HisGEEntity = null;
        /// <summary>
        /// 获取ICON图片的数据。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueImgStrs(string key)
        {
            key = key.Replace(" ", "");
            key = key.Replace("\r\n", "");

            /*说明是图片文件.*/
            string path = key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);

            //定义rtf中图片字符串
            StringBuilder pict = new StringBuilder();
            //获取要插入的图片
            System.Drawing.Image img = System.Drawing.Image.FromFile(path);

            //将要插入的图片转换为16进制字符串
            string imgHexString;
            key = key.ToLower();

            if (key.Contains(".png"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Png);
            else if (key.Contains(".jp"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);
            else if (key.Contains(".gif"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Gif);
            else if (key.Contains(".ico"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Icon);
            else
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);

            //生成rtf中图片字符串
            pict.AppendLine();
            pict.Append(@"{\pict");
            pict.Append(@"\jpegblip");
            pict.Append(@"\picscalex100");
            pict.Append(@"\picscaley100");
            pict.Append(@"\picwgoal" + img.Size.Width * 15);
            pict.Append(@"\pichgoal" + img.Size.Height * 15);
            pict.Append(imgHexString + "}");
            pict.AppendLine();
            return pict.ToString();
        }
        /// <summary>
        /// 获取ICON图片的数据。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueImgStrsOfQR(string billUrl)
        {
            /*说明是图片文件.*/
            string path = SystemConfig.PathOfTemp + Guid.NewGuid() + ".png"; // key.Replace("OID.Img@AppPath", SystemConfig.PathOfWebApp);

            #region 生成二维码.
            ThoughtWorks.QRCode.Codec.QRCodeEncoder qrc = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            qrc.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            qrc.QRCodeScale = 4;
            qrc.QRCodeVersion = 7;
            qrc.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Bitmap btm = qrc.Encode(billUrl, System.Text.Encoding.UTF8);
            btm.Save(path);
            #endregion

            //定义rtf中图片字符串
            StringBuilder pict = new StringBuilder();
            //获取要插入的图片
            System.Drawing.Image img = System.Drawing.Image.FromFile(path);

            //将要插入的图片转换为16进制字符串.
            string imgHexString;
            imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Png);

            //生成rtf中图片字符串
            pict.AppendLine();
            pict.Append(@"{\pict");
            pict.Append(@"\jpegblip");
            pict.Append(@"\picscalex100");
            pict.Append(@"\picscaley100");
            pict.Append(@"\picwgoal" + img.Size.Width * 15);
            pict.Append(@"\pichgoal" + img.Size.Height * 15);
            pict.Append(imgHexString + "}");
            pict.AppendLine();
            return pict.ToString();
        }
        /// <summary>
        /// 获取M2M数据并输出
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueM2MStrs(string key)
        {
            return "";

            //string[] strs = key.Split('.');
            //string sql = "SELECT ValsName FROM SYS_M2M WHERE FK_MapData='" + strs[0] + "' AND M2MNo='" + strs[2] + "' AND EnOID='" + this.HisGEEntity.PKVal + "'";
            //string vals = DBAccess.RunSQLReturnStringIsNull(sql, null);
            //if (vals == null)
            //    return "无数据";

            //vals = vals.Replace("@", "  ");
            //vals = vals.Replace("<font color=green>", "");
            //vals = vals.Replace("</font>", "");
            //return vals;

            //string val = "";
            //string[] objs = vals.Split('@');
            //foreach (string obj in objs)
            //{
            //    string[] noName = obj.Split(',');
            //    val += noName[1];
            //}
            //return val;
        }
        /// <summary>
        /// 获取写字版的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueBPPaintStrs(string key)
        {
            key = key.Replace(" ", "");
            key = key.Replace("\r\n", "");

            string[] strs = key.Split('.');
            string filePath = "";
            try
            {
                filePath = DBAccess.RunSQLReturnString("SELECT Tag2 From Sys_FrmEleDB WHERE RefPKVal=" + this.HisGEEntity.PKVal + " AND EleID='" + strs[2].Trim() + "'");
                if (filePath == null)
                    return "";
            }
            catch
            {
                return "";
            }

            //定义rtf中图片字符串
            StringBuilder pict = new StringBuilder();
            //获取要插入的图片
            System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);

            //将要插入的图片转换为16进制字符串
            string imgHexString;
            filePath = filePath.ToLower();

            if (filePath.Contains(".png"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Png);
            else if (filePath.Contains(".jp"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);
            else if (filePath.Contains(".gif"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Gif);
            else if (filePath.Contains(".ico"))
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Icon);
            else
                imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);

            //生成rtf中图片字符串
            pict.AppendLine();
            pict.Append(@"{\pict");
            pict.Append(@"\jpegblip");
            pict.Append(@"\picscalex100");
            pict.Append(@"\picscaley100");
            pict.Append(@"\picwgoal" + img.Size.Width * 15);
            pict.Append(@"\pichgoal" + img.Size.Height * 15);
            pict.Append(imgHexString + "}");
            pict.AppendLine();
            return pict.ToString();
        }
        /// <summary>
        /// 获取类名+@+字段格式的数据.
        /// 比如：
        /// Demo_Inc@ABC
        /// Emp@Name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByAtKey(string key)
        {
            foreach (Entity en in this.HisEns)
            {
                string enKey = en.ToString();

                //有可能是 BP.Port.Emp
                if (enKey.Contains("."))
                    enKey = en.GetType().Name;

                //如果不包含.
                if (key.Contains(enKey + "@") == false)
                    continue;

                // 如果不包含 . 就说明，不需要转意。
                if (key.Contains(".") == false)
                    return en.GetValStringByKey(key.Substring(key.IndexOf('@') + 1));

                //把实体名去掉
                key = key.Replace(enKey + "@", "");
                //把数据破开.
                string[] strs = key.Split('.');
                if (strs.Length == 2)
                {
                    if (strs[1].Trim() == "ImgAth")
                    {
                        string path1 = BP.Sys.SystemConfig.PathOfDataUser + "\\ImgAth\\Data\\" + strs[0].Trim() + "_" + en.PKVal + ".png";
                        //定义rtf中图片字符串.
                        StringBuilder mypict = new StringBuilder();
                        //获取要插入的图片
                        System.Drawing.Image imgAth = System.Drawing.Image.FromFile(path1);

                        //将要插入的图片转换为16进制字符串
                        string imgHexStringImgAth = GetImgHexString(imgAth, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //生成rtf中图片字符串
                        mypict.AppendLine();
                        mypict.Append(@"{\pict");
                        mypict.Append(@"\jpegblip");
                        mypict.Append(@"\picscalex100");
                        mypict.Append(@"\picscaley100");
                        mypict.Append(@"\picwgoal" + imgAth.Size.Width * 15);
                        mypict.Append(@"\pichgoal" + imgAth.Size.Height * 15);
                        mypict.Append(imgHexStringImgAth + "}");
                        mypict.AppendLine();
                        return mypict.ToString();
                    }

                    string val = en.GetValStringByKey(strs[0].Trim());
                    switch (strs[1].Trim())
                    {
                        case "Text":
                            if (val == "0")
                                return "否";
                            else
                                return "是";
                        case "Year":
                            return val.Substring(0, 4);
                        case "Month":
                            return val.Substring(5, 2);
                        case "Day":
                            return val.Substring(8, 2);
                        case "NYR":
                            return DA.DataType.ParseSysDate2DateTime(val).ToString("yyyy年MM月dd日");
                        case "RMB":
                            return float.Parse(val).ToString("0.00");
                        case "RMBDX":
                            return DA.DataType.ParseFloatToCash(float.Parse(val));
                        case "ImgAth":
                            string path1 = BP.Sys.SystemConfig.PathOfDataUser + "\\ImgAth\\Data\\" + strs[0].Trim() + "_" + this.HisGEEntity.PKVal + ".png";

                            //定义rtf中图片字符串.
                            StringBuilder mypict = new StringBuilder();
                            //获取要插入的图片
                            System.Drawing.Image imgAth = System.Drawing.Image.FromFile(path1);

                            //将要插入的图片转换为16进制字符串
                            string imgHexStringImgAth = GetImgHexString(imgAth, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //生成rtf中图片字符串
                            mypict.AppendLine();
                            mypict.Append(@"{\pict");
                            mypict.Append(@"\jpegblip");
                            mypict.Append(@"\picscalex100");
                            mypict.Append(@"\picscaley100");
                            mypict.Append(@"\picwgoal" + imgAth.Size.Width * 15);
                            mypict.Append(@"\pichgoal" + imgAth.Size.Height * 15);
                            mypict.Append(imgHexStringImgAth + "}");
                            mypict.AppendLine();
                            return mypict.ToString();
                        case "Siganture":
                            string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + val + ".jpg";
                            //定义rtf中图片字符串.
                            StringBuilder pict = new StringBuilder();
                            //获取要插入的图片
                            System.Drawing.Image img = System.Drawing.Image.FromFile(path);

                            //将要插入的图片转换为16进制字符串
                            string imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //生成rtf中图片字符串
                            pict.AppendLine();
                            pict.Append(@"{\pict");
                            pict.Append(@"\jpegblip");
                            pict.Append(@"\picscalex100");
                            pict.Append(@"\picscaley100");
                            pict.Append(@"\picwgoal" + img.Size.Width * 15);
                            pict.Append(@"\pichgoal" + img.Size.Height * 15);
                            pict.Append(imgHexString + "}");
                            pict.AppendLine();
                            return pict.ToString();
                        //替换rtf模板文件中的签名图片标识为图片字符串
                        // str = str.Replace(imgMark, pict.ToString());
                        default:
                            throw new Exception("参数设置错误，特殊方式取值错误：" + key);
                    }
                }
            } // 实体循环。

            throw new Exception("参数设置错误 GetValueByKey ：" + key);
        }
        /// <summary>
        /// 获得所所有的审核人员信息.
        /// </summary>
        /// <returns></returns>
        public string GetValueCheckWorks()
        {
            string html = "";

            //获得当前待办的人员,把当前审批的人员排除在外,不然就有默认同意的意见可以打印出来.
           string sql = "SELECT FK_Emp, FK_Node FROM WF_GenerWorkerList WHERE IsPass!=1 AND WorkID=" + this.HisGEEntity.PKVal;
            DataTable dtOfTodo = DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow dr in dtTrack.Rows)
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
                        empStrs = "<img src='../../../../../DataUser/Siganture/" + dr["EmpFrom"] + ".jpg' title='" + dr["EmpFromT"] + "' style='height:60px;' border=0 onerror=\"src='../../../../../DataUser/Siganture/UnName.JPG'\" /> " + dr["EmpFromT"];
                    }

                }

                msg += "审核人:" + empStrs + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日期:" + dr["RDT"].ToString();

                html += " <td colspan=3 valign=middle >" + msg + "</td>";
                html += " </tr>";
            }

            return html;
        }
        /// <summary>
        /// 获得审核组件的信息.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueCheckWorkByKey(string key)
        {
            key = key.Replace(" ", "");
            key = key.Replace("\r\n", "");

            string[] strs = key.Split('.');
            if (strs.Length == 3)
            {
                /*
                 *  是一个节点一个审核人的模式. <WorkCheck.RDT.101>
                 */
                if (dtTrack == null)
                    throw new Exception("@您设置了获取审核组件里的规则，但是你没有给审核组件数据源dtTrack赋值。");

                string nodeID = strs[2];
                foreach (DataRow dr in dtTrack.Rows)
                {
                    if (dr["NDFrom"].ToString() != nodeID)
                        continue;

                    switch (strs[1])
                    {
                        case "RDT":
                            return dr["RDT"].ToString(); //审核日期.
                        case "RDT-NYR":
                            string rdt = dr["RDT"].ToString(); //审核日期.
                            return BP.DA.DataType.ParseSysDate2DateTimeFriendly(rdt);
                        case "Rec":
                            return dr["EmpFrom"].ToString(); //记录人.
                        case "RecName":
                            string recName = dr["EmpFromT"].ToString(); //审核人.
                            recName = this.GetCode(recName);
                            return recName;
                        case "Msg":
                        case "Note":
                            string text=dr["Msg"].ToString();
                            text = text.Replace("\\", "\\\\");
                            text = this.GetCode(text);
                            //return Encoding.GetEncoding("GB2312").GetString(Encoding.UTF8.GetBytes(dr["Msg"].ToString()));
                            return text;

                        //return System.Text.Encoder  //审核信息.
                        default:
                            break;
                    }
                }
            }

            return "无";
        }

        private string GetValueCheckWorkByKey(DataRow row, string key)
        {
            key = key.Replace(" ", "");
            key = key.Replace("\r\n", "");

            switch (key)
            {
                case "RDT":
                    return row["RDT"].ToString(); //审核日期.
                case "RDT-NYR":
                    string rdt = row["RDT"].ToString(); //审核日期.
                    return BP.DA.DataType.ParseSysDate2DateTimeFriendly(rdt);
                case "Rec":
                    return row["EmpFrom"].ToString(); //记录人.
                case "RecName":
                    return row["EmpFromT"].ToString(); //审核人.
                case "Msg":
                case "Note":
                    return row["Msg"].ToString();
                default:
                    return row[key] as string;
            }
        }
        /// <summary>
        /// 审核节点的表示方法是 节点ID.Attr.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByKey(string key)
        {
            key = key.Replace(" ", "");
            key = key.Replace("\r\n", "");

            //获取参数代码.
            if (key.Contains("@"))
                return GetValueByAtKey(key);

            string[] strs = key.Split('.');

            // 如果不包含 . 就说明他是从Rpt中取数据。
            //if (this.HisGEEntity != null && key.Contains("ND") == false)
            if (this.HisGEEntity != null)
            {
                if (strs.Length == 1)
                    return this.HisGEEntity.GetValStringByKey(key);

                if (strs[1].Trim() == "Editor")
                {
                    //获取富文本的内容
                    string content = this.HisGEEntity.GetValStringByKey(strs[0]);
                    content = content.Replace("img+", "img ");
                    string contentHtml = "<html><head></head><body>" + content + "</body></html>";
                    string StrNohtml = System.Text.RegularExpressions.Regex.Replace(contentHtml, "<[^>]+>", "");
                    StrNohtml = System.Text.RegularExpressions.Regex.Replace(StrNohtml, "&[^;]+;", "");

                    return this.GetCode(StrNohtml); 


                    string htmlpath = BP.Sys.SystemConfig.PathOfDataUser + "Bill\\Temp\\EditorHtm.html";
                    if (File.Exists(htmlpath) == false)
                        File.Create(htmlpath);
                    using (StreamWriter sw = new StreamWriter(htmlpath))
                    {
                        sw.Write(contentHtml);
                    }

                    //如何写入到word
                    string html = File.ReadAllText(htmlpath, Encoding.UTF8);

                    //byte[] array = Encoding.ASCII.GetBytes(content);
                    //StringBuilder editors = new StringBuilder();
                    //for (int i = 0; i < array.Length; i++)
                    //{

                    //    editors.Append(array[i]);

                    //}
                    //MemoryStream stream = new MemoryStream(array);             //convert stream 2 string      

                    //System.IO.StreamReader readStream = new System.IO.StreamReader(contentHtml, Encoding.UTF8);
                    return html;

                }

                if (strs[1].Trim() == "ImgAth")
                {
                    string path1 = BP.Sys.SystemConfig.PathOfDataUser + "ImgAth\\Data\\" + strs[0].Trim() + "_" + this.HisGEEntity.PKVal + ".png";
                    if (!File.Exists(path1))
                    {
                        FrmImgAthDB dbImgAth = new FrmImgAthDB();
                        dbImgAth.MyPK = strs[0].Trim() + "_" + this.HisGEEntity.PKVal;
                        int count = dbImgAth.RetrieveFromDBSources();
                        if (count == 1)
                        {
                            path1 = BP.Sys.SystemConfig.PathOfDataUser + "ImgAth\\Data\\" + dbImgAth.FileName + ".png";
                            if (!File.Exists(path1))
                                return this.GetCode(key);
                        }
                        return "";
                    }
                    //定义rtf中图片字符串.
                    StringBuilder mypict = new StringBuilder();
                    //获取要插入的图片
                    System.Drawing.Image imgAth = System.Drawing.Image.FromFile(path1);
                    //图片附件描述属性
                    FrmImgAth frmImgAth = new FrmImgAth();
                    frmImgAth.RetrieveByAttr(FrmImgAthAttr.MyPK, strs[0].Trim());
                    //图片高宽
                    float iWidth = imgAth.Size.Width * 15;
                    float iHeight = imgAth.Size.Height * 15;
                    if (frmImgAth != null && !DataType.IsNullOrEmpty(frmImgAth.FK_MapData))
                    {
                        iWidth = frmImgAth.W * 15;
                        iHeight = frmImgAth.H * 15;
                    }

                    //将要插入的图片转换为16进制字符串
                    string imgHexStringImgAth = GetImgHexString(imgAth, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //生成rtf中图片字符串
                    mypict.AppendLine();
                    mypict.Append(@"{\pict");
                    mypict.Append(@"\jpegblip");
                    mypict.Append(@"\picscalex100");
                    mypict.Append(@"\picscaley100");
                    mypict.Append(@"\picwgoal" + iWidth);
                    mypict.Append(@"\pichgoal" + iHeight);
                    mypict.Append(imgHexStringImgAth + "}");
                    mypict.AppendLine();
                    return mypict.ToString();
                }

                if (strs[1].Trim() == "BPPaint")
                {
                    string path1 = DBAccess.RunSQLReturnString("SELECT  Tag2 FROM Sys_FrmEleDB WHERE REFPKVAL=" + this.HisGEEntity.PKVal + " AND EleID='" + strs[0].Trim() + "'");
                    //  string path1 = BP.Sys.SystemConfig.PathOfDataUser + "\\BPPaint\\" + this.HisGEEntity.ToString().Trim() + "\\" + this.HisGEEntity.PKVal + ".png";
                    //定义rtf中图片字符串.
                    StringBuilder mypict = new StringBuilder();
                    //获取要插入的图片
                    System.Drawing.Image myBPPaint = System.Drawing.Image.FromFile(path1);

                    //将要插入的图片转换为16进制字符串
                    string imgHexStringImgAth = GetImgHexString(myBPPaint, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //生成rtf中图片字符串
                    mypict.AppendLine();
                    mypict.Append(@"{\pict");
                    mypict.Append(@"\jpegblip");
                    mypict.Append(@"\picscalex100");
                    mypict.Append(@"\picscaley100");
                    mypict.Append(@"\picwgoal" + myBPPaint.Size.Width * 15);
                    mypict.Append(@"\pichgoal" + myBPPaint.Size.Height * 15);
                    mypict.Append(imgHexStringImgAth + "}");
                    mypict.AppendLine();
                    return mypict.ToString();
                }

                //根据枚举值返回选中符号
                if (strs[1].Contains("-EnumYes") == true)
                {
                    string relVal = this.HisGEEntity.GetValStringByKey(strs[0]);
                    string[] checkVal = strs[1].Split('-');
                    if (checkVal.Length == 1)
                        return relVal;
                    if (relVal.Equals(checkVal[0]))
                        return "[√]";
                    else
                        return "[×]";
                }

                if (strs.Length == 2)
                {
                    string val = this.HisGEEntity.GetValStringByKey(strs[0].Trim());
                    switch (strs[1].Trim())
                    {

                        case "Text":
                            if (val == "0")
                                return "否";
                            else
                                return "是";
                        case "Year":
                            return val.Substring(0, 4);
                        case "Month":
                            return val.Substring(5, 2);
                        case "Day":
                            return val.Substring(8, 2);
                        case "NYR":
                            return DA.DataType.ParseSysDate2DateTime(val).ToString("yyyy年MM月dd日");
                        case "RMB":
                            decimal md= Math.Round( decimal.Parse(val) ,2);
                            return md.ToString();
                        case "RMBDX":
                            return this.GetCode(DA.DataType.ParseFloatToCash(float.Parse(val)));
                        case "Siganture":
                            string path = BP.Sys.SystemConfig.PathOfDataUser + "Siganture\\" + val + ".jpg";
                            //获取要插入的图片
                            if (File.Exists(path) == true)
                            {
                                //定义rtf中图片字符串
                                StringBuilder pict = new StringBuilder();
                                System.Drawing.Image img = System.Drawing.Image.FromFile(path);

                                //将要插入的图片转换为16进制字符串
                                string imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                                //生成rtf中图片字符串
                                pict.AppendLine();
                                pict.Append(@"{\pict");
                                pict.Append(@"\jpegblip");
                                pict.Append(@"\picscalex100");
                                pict.Append(@"\picscaley100");
                                pict.Append(@"\picwgoal" + img.Size.Width * 15);
                                pict.Append(@"\pichgoal" + img.Size.Height * 15);
                                pict.Append(imgHexString + "}");
                                pict.AppendLine();
                                return pict.ToString();
                            }
                            //图片不存在显示中文名，否则显示原值
                            string empName = DBAccess.RunSQLReturnStringIsNull("SELECT Name FROM Port_Emp WHERE No='" + val + "'", val);
                            return this.GetCode(empName);
                        //替换rtf模板文件中的签名图片标识为图片字符串
                        // str = str.Replace(imgMark, pict.ToString());
                        case "BoolenText":
                            if (val == "0")
                                return "否";
                            else
                                return "是";
                        case "Boolen":
                            if (val == "1")
                                return "[√]";
                            else
                                return "[×]";
                            break;
                        case "YesNo":
                            if (val == "1")
                                return "[√]";
                            else
                                return "[×]";
                            break;
                        case "Yes":
                            if (val == "1")
                                return "[√]";
                            else
                                return "[×]";
                        case "No":
                            if (val == "0")
                                return "[√]";
                            else
                                return "[×]";
                        default:
                            throw new Exception("参数设置错误，特殊方式取值错误：" + key);
                    }
                }
                else
                {
                    throw new Exception("参数设置错误，特殊方式取值错误：" + key);
                }
            }

            foreach (Entity en in this.HisEns)
            {
                string enKey = en.ToString();
                if (enKey.Contains("."))
                    enKey = en.GetType().Name;
                if (key.Contains(en.ToString() + ".") == false)
                    continue;

                /*说明就在这个字段内*/
                if (strs.Length == 1)
                    throw new Exception("参数设置错误，strs.length=1 。" + key);

                if (strs.Length == 2)
                    return en.GetValStringByKey(strs[1].Trim());

                if (strs.Length == 3)
                {
                    if (strs[2].Trim() == "ImgAth")
                    {
                        string path1 = SystemConfig.PathOfDataUser + "\\ImgAth\\Data\\" + strs[1].Trim() + "_" + en.PKVal + ".png";
                        //定义rtf中图片字符串.
                        StringBuilder mypict = new StringBuilder();
                        //获取要插入的图片
                        System.Drawing.Image imgAth = System.Drawing.Image.FromFile(path1);

                        //将要插入的图片转换为16进制字符串
                        string imgHexStringImgAth = GetImgHexString(imgAth, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //生成rtf中图片字符串
                        mypict.AppendLine();
                        mypict.Append(@"{\pict");
                        mypict.Append(@"\jpegblip");
                        mypict.Append(@"\picscalex100");
                        mypict.Append(@"\picscaley100");
                        mypict.Append(@"\picwgoal" + imgAth.Size.Width * 15);
                        mypict.Append(@"\pichgoal" + imgAth.Size.Height * 15);
                        mypict.Append(imgHexStringImgAth + "}");
                        mypict.AppendLine();
                        return mypict.ToString();
                    }


                    string val = en.GetValStringByKey(strs[1].Trim());
                    switch (strs[2].Trim())
                    {
                        case "Text":
                            if (val == "0")
                                return "否";
                            else
                                return "是";
                        case "Year":
                            return val.Substring(0, 4);
                        case "Month":
                            return val.Substring(5, 2);
                        case "Day":
                            return val.Substring(8, 2);
                        case "NYR":
                            return DA.DataType.ParseSysDate2DateTime(val).ToString("yyyy年MM月dd日");
                        case "RMB":
                            return float.Parse(val).ToString("0.00");
                        case "RMBDX":
                            return DA.DataType.ParseFloatToCash(float.Parse(val));
                        case "ImgAth":
                            string path1 = SystemConfig.PathOfDataUser + "\\ImgAth\\Data\\" + strs[0].Trim() + "_" + this.HisGEEntity.PKVal + ".png";

                            //定义rtf中图片字符串.
                            StringBuilder mypict = new StringBuilder();
                            //获取要插入的图片
                            System.Drawing.Image imgAth = System.Drawing.Image.FromFile(path1);

                            //将要插入的图片转换为16进制字符串
                            string imgHexStringImgAth = GetImgHexString(imgAth, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //生成rtf中图片字符串
                            mypict.AppendLine();
                            mypict.Append(@"{\pict");
                            mypict.Append(@"\jpegblip");
                            mypict.Append(@"\picscalex100");
                            mypict.Append(@"\picscaley100");
                            mypict.Append(@"\picwgoal" + imgAth.Size.Width * 15);
                            mypict.Append(@"\pichgoal" + imgAth.Size.Height * 15);
                            mypict.Append(imgHexStringImgAth + "}");
                            mypict.AppendLine();
                            return mypict.ToString();
                        case "Siganture":
                            string path = SystemConfig.PathOfDataUser + "\\Siganture\\" + val + ".jpg";
                            //定义rtf中图片字符串.
                            StringBuilder pict = new StringBuilder();
                            //获取要插入的图片
                            System.Drawing.Image img = System.Drawing.Image.FromFile(path);

                            //将要插入的图片转换为16进制字符串
                            string imgHexString = GetImgHexString(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //生成rtf中图片字符串
                            pict.AppendLine();
                            pict.Append(@"{\pict");
                            pict.Append(@"\jpegblip");
                            pict.Append(@"\picscalex100");
                            pict.Append(@"\picscaley100");
                            pict.Append(@"\picwgoal" + img.Size.Width * 15);
                            pict.Append(@"\pichgoal" + img.Size.Height * 15);
                            pict.Append(imgHexString + "}");
                            pict.AppendLine();
                            return pict.ToString();
                        //替换rtf模板文件中的签名图片标识为图片字符串
                        // str = str.Replace(imgMark, pict.ToString());
                        default:
                            throw new Exception("参数设置错误，特殊方式取值错误：" + key);
                    }
                }
            }

            throw new Exception("参数设置错误 GetValueByKey ：" + key);
        }
        #endregion

        #region 生成单据
        /// <summary>
        /// 生成单据
        /// </summary>
        /// <param name="cfile">模板文件</param>
        public void MakeDoc(string cfile)
        {
            string file = PubClass.GenerTempFileName("doc");
            this.MakeDoc(cfile, SystemConfig.PathOfTemp, file, true);
        }
        public string ensStrs = "";
        /// <summary>
        /// 轨迹表（用于输出打印审核轨迹,审核信息.）
        /// </summary>
        public DataTable dtTrack = null;
        /// <summary>
        /// 单据生成 
        /// </summary>
        /// <param name="cfile">模板文件</param>
        /// <param name="path">生成路径</param>
        /// <param name="file">生成文件</param>
        /// <param name="isOpen">是否用IE打开？</param>
        /// <param name="isOpen">要打开的url用于生成二维码</param>
        public void MakeDoc(string templateRtfFile, string path, string file,bool isOpen, string billUrl=null)
        {
            templateRtfFile = templateRtfFile.Replace(".rtf.rtf", ".rtf");
            
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            string str = Cash.GetBillStr(templateRtfFile, false).Substring(0);
            if (this.HisEns.Count == 0)
                if (this.HisGEEntity == null)
                    throw new Exception("@您没有为报表设置数据源...");

            this.ensStrs = "";
            if (this.HisEns.Count != 0)
            {
                foreach (Entity en in this.HisEns)
                    ensStrs += en.ToString();
            }
            else
            {
                ensStrs = this.HisGEEntity.ToString();
            }

            string error = "";
            string[] paras = null;
            if (this.HisGEEntity != null)
                paras = Cash.GetBillParas(templateRtfFile, ensStrs, this.HisGEEntity);
            else
                paras = Cash.GetBillParas(templateRtfFile, ensStrs, this.HisEns);

            this.TempFilePath = path + file;
            try
            {
                string key = "";
                string ss = "";

                #region 替换主表标记
                foreach (string para in paras)
                {
                    if (para == null || para == "")
                        continue;

                    try
                    {
                        if(para.Contains("Editor"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains("ImgAth"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains("Siganture"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains("Img@AppPath"))
                            str = str.Replace("<" + para + ">", this.GetValueImgStrs(para));
                        else if (para.Contains("Img@QR"))
                            str = str.Replace("<" + para + ">", this.GetValueImgStrsOfQR(billUrl));
                        else if (para.Contains(".BPPaint"))
                            str = str.Replace("<" + para + ">", this.GetValueBPPaintStrs(para));
                        else if (para.Contains(".M2M"))
                            str = str.Replace("<" + para + ">", this.GetValueM2MStrs(para));
                        else if (para.Contains(".RMBDX"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".RMB"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".Boolen"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".BoolenText"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".NYR"))
                            str = str.Replace("<" + para + ">", this.GetCode(this.GetValueByKey(para)));
                        else if (para.Contains(".Year"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".Month"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".Day"))
                            str = str.Replace("<" + para + ">", this.GetValueByKey(para));
                        else if (para.Contains(".Yes") == true)
                            str = str.Replace("<" + para + ">", this.GetCode(this.GetValueByKey(para)));
                        else if (para.Contains("-EnumYes") == true)
                            str = str.Replace("<" + para + ">", this.GetCode(this.GetValueByKey(para)));
                        else if (para.Contains("WorkCheck.RDT")
                            || para.Contains("WorkCheck.Rec")
                            || para.Contains("WorkCheck.RecName")
                            || para.Contains("WorkCheck.Note"))   // 审核组件的审核日期.
                            str = str.Replace("<" + para + ">", this.GetValueCheckWorkByKey(para));
                        else if (para.Contains("WorkChecks") == true) //为烟台增加审核人员的信息,把所有的审核人员信息都输入到这里.
                            str = str.Replace("<" + para + ">", this.GetValueCheckWorks());
                        else if (para.Contains(".") == true)
                            continue; /*有可能是明细表数据.*/
                        else
                        {
                            string val = this.GetValueByKey(para);
                            val = val.Replace("\\", "\\\\");
                            val = this.GetCode(val);
                            str = str.Replace("<" + para + ">", val);
                        }
                    }
                    catch (Exception ex)
                    { 
                        error += "@替换主表标记取参数[" + para + "]出现错误：有以下情况导致此错误;1你用Text取值时间，此属性不是外键。2,类无此属性。3,该字段是明细表字段但是丢失了明细表标记.<br>更详细的信息：<br>" + ex.Message;
                        if (SystemConfig.IsDebug)
                            throw new Exception(error);
                        Log.DebugWriteError(error);
                    }
                }
                #endregion 替换主表标记

                #region 从表
                string shortName = "";
                ArrayList al = this.EnsDataDtls;
                foreach (Entities dtls in al)
                {
                    Entity dtl = dtls.GetNewEntity;
                    string dtlEnName = dtl.ToString();
                    shortName = dtlEnName.Substring(dtlEnName.LastIndexOf(".") + 1);

                    if (str.IndexOf(shortName) == -1)
                        continue;

                    int pos_rowKey = str.IndexOf(shortName);
                    int row_start = -1, row_end = -1;
                    if (pos_rowKey != -1)
                    {
                        row_start = str.Substring(0, pos_rowKey).LastIndexOf("\\row");
                       
                        row_end = str.Substring(pos_rowKey).IndexOf("\\row");
                    }

                    if (row_start != -1 && row_end != -1)
                    {
                        string row = str.Substring(row_start, (pos_rowKey - row_start) + row_end);
                        str = str.Replace(row, "");

                        Map map = dtls.GetNewEntity.EnMap;
                        int i = dtls.Count;
                        while (i > 0)
                        {
                            i--;
                            string rowData = row.Clone() as string;
                            dtl = dtls[i];
                            //替换序号  
                            int rowIdx = i + 1;
                            rowData = rowData.Replace("<IDX>", rowIdx.ToString());

                            foreach (Attr attr in map.Attrs)
                            {
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppDouble:
                                    case DataType.AppFloat:
                                        rowData = rowData.Replace("<" + shortName + "." + attr.Key + ">", dtl.GetValStringByKey(attr.Key));
                                        break;
                                    case DataType.AppMoney:
                                        rowData = rowData.Replace("<" + shortName + "." + attr.Key + ">", dtl.GetValDecimalByKey(attr.Key).ToString("0.00"));
                                        break;
                                    case DataType.AppInt:

                                        if (attr.MyDataType == DataType.AppBoolean)
                                        {
                                            rowData = rowData.Replace("<" + shortName + "." + attr.Key + ">", dtl.GetValStrByKey(attr.Key));
                                            int v = dtl.GetValIntByKey(attr.Key);
                                            if (v == 1)
                                                rowData = rowData.Replace("<" + shortName + "." + attr.Key + "Text>", "是");
                                            else
                                                rowData = rowData.Replace("<" + shortName + "." + attr.Key + "Text>", "否");
                                        }
                                        else
                                        {
                                            if (attr.IsEnum)
                                                rowData = rowData.Replace("<" + shortName + "." + attr.Key + "Text>", GetCode(dtl.GetValRefTextByKey(attr.Key)));
                                            else
                                                rowData = rowData.Replace("<" + shortName + "." + attr.Key + ">", dtl.GetValStrByKey(attr.Key));
                                        }
                                        break;
                                    default:
                                        rowData = rowData.Replace("<" + shortName + "." + attr.Key + ">", GetCode(dtl.GetValStrByKey(attr.Key)));
                                        break;
                                }
                            }

                            str = str.Insert(row_start, rowData);
                        }
                    }
                }
                #endregion 从表

                #region 明细 合计信息。
                al = this.EnsDataDtls;
                foreach (Entities dtls in al)
                {
                    Entity dtl = dtls.GetNewEntity;
                    string dtlEnName = dtl.ToString();
                    shortName = dtlEnName.Substring(dtlEnName.LastIndexOf(".") + 1);
                    //shortName = dtls.ToString().Substring(dtls.ToString().LastIndexOf(".") + 1);
                    Map map = dtl.EnMap;
                    foreach (Attr attr in map.Attrs)
                    {
                        switch (attr.MyDataType)
                        {
                            case DataType.AppDouble:
                            case DataType.AppFloat:
                            case DataType.AppMoney:
                                key = "<" + shortName + "." + attr.Key + ".SUM>";
                                if (str.IndexOf(key) != -1)
                                    str = str.Replace(key, dtls.GetSumFloatByKey(attr.Key).ToString());

                                key = "<" + shortName + "." + attr.Key + ".SUM.RMB>";
                                if (str.IndexOf(key) != -1)
                                    str = str.Replace(key, dtls.GetSumFloatByKey(attr.Key).ToString("0.00"));

                                key = "<" + shortName + "." + attr.Key + ".SUM.RMBDX>";
                                if (str.IndexOf(key) != -1)
                                    str = str.Replace(key,
                                        GetCode(DA.DataType.ParseFloatToCash(dtls.GetSumFloatByKey(attr.Key))));
                                break;
                            case DataType.AppInt:
                                key = "<" + shortName + "." + attr.Key + ".SUM>";
                                if (str.IndexOf(key) != -1)
                                    str = str.Replace(key, dtls.GetSumIntByKey(attr.Key).ToString());
                                break;
                            default:
                                break;
                        }
                    }
                }
                #endregion 从表合计

                #region 审核组件组合信息，added by liuxc,2016-12-16

                //节点单个审核人
		       if (dtTrack != null && str.Contains("<WorkCheckBegin>")== false && str.Contains("<WorkCheckEnd>") ==false){
                   foreach (DataRow row in dtTrack.Rows) //此处的22是ActionType.WorkCheck的值，此枚举位于BP.WF项目中，此处暂写死此值
	                {
                        int acType = int.Parse(row["ACTIONTYPE"].ToString());
	                    if (acType != 22)
	                        continue;
	                    str = str.Replace(
							    "<WorkCheck.Msg." + row["NDFrom"] + ">", this.GetCode(this.GetValueCheckWorkByKey(row, "Msg")));
	                    str = str.Replace(
							    "<WorkCheck.Rec." + row["NDFrom"] + ">", this.GetCode(this.GetValueCheckWorkByKey(row, "EmpFromT")));
	                    str = str.Replace(
							    "<WorkCheck.RDT." + row["NDFrom"] + ">",this.GetCode(this.GetValueCheckWorkByKey(row, "RDT")));
	                
	                
	                }
		       }

                if (dtTrack != null && str.Contains("<WorkCheckBegin>") && str.Contains("<WorkCheckEnd>"))
                {
                    int beginIdx = str.IndexOf("<WorkCheckBegin>"); //len:16
                    int endIdx = str.IndexOf("<WorkCheckEnd>"); //len:14
                    string moduleStr = str.Substring(beginIdx + 16, endIdx - beginIdx - 16);
                    ArrayList tags = new ArrayList();
                    string val = string.Empty;
                    string field = string.Empty;
                    string checkStr = string.Empty;
                    string[] ps = null;


                    foreach (string para in paras)
                    {
                        if (string.IsNullOrWhiteSpace(para) || para.Contains("WorkCheckList.")==false)
                            continue;

                        ps = para.Split('.');
                        tags.Add(ps[1]);
                    }

                    foreach (DataRow row in dtTrack.Rows) //此处的22是ActionType.WorkCheck的值，此枚举位于BP.WF项目中，此处暂写死此值
                    {
                        int acType = int.Parse(row["ACTIONTYPE"].ToString());
                        if (acType != 22)
                            continue;
                        checkStr = moduleStr;
                        foreach (string tag in tags)
                        {
                            checkStr = checkStr.Replace("<WorkCheckList." + tag + ">",
                                                         this.GetCode(this.GetValueCheckWorkByKey(row, tag)));
                        }

                        str = str.Insert(beginIdx, checkStr);
                        beginIdx += checkStr.Length;
                        endIdx += checkStr.Length;
                    }

                    str = str.Substring(0, beginIdx) + (endIdx < str.Length - 1 ? str.Substring(endIdx + 14) : "");

                }
                #endregion

                #region 多附件
                foreach (string athObjEnsName in this.EnsDataAths.Keys)
                {
                    string athName = "Ath." + athObjEnsName;
                    string athFilesName = "";
                    if (str.IndexOf(athName) == -1)
                        continue;

                    FrmAttachmentDBs athDbs = this.EnsDataAths[athObjEnsName] as FrmAttachmentDBs;
                    if (athDbs == null)
                        continue;
                    foreach (FrmAttachmentDB athDb in athDbs)
                    {
                        if (athFilesName.Length > 0)
                            athFilesName += " ， ";

                        athFilesName += athDb.FileName;
                    }
                    str = str.Replace("<" + athName + ">", this.GetCode(athFilesName));
                }
                #endregion

                #region 要替换的字段
                //if (replaceVals != null && replaceVals.Contains("@"))
                //{
                //    string[] vals = replaceVals.Split('@');
                //    foreach (string val in vals)
                //    {
                //        if (val == null || val == "")
                //            continue;

                //        if (val.Contains("=") == false)
                //            continue;

                //        string myRep = val.Clone() as string;

                //        myRep = myRep.Trim();
                //        myRep = myRep.Replace("null", "");
                //        string[] myvals = myRep.Split('=');
                //        str = str.Replace("<" + myvals[0] + ">", "<" + myvals[1] + ">");
                //    }
                //}
                #endregion

                StreamWriter wr = new StreamWriter(this.TempFilePath, false, Encoding.ASCII);
                str = str.Replace("<", "");
                str = str.Replace(">", "");
                wr.Write(str);
                wr.Close();
            }
            catch (Exception ex)
            {
                string msg = "";
                if (SystemConfig.IsDebug)
                {  // 异常可能与单据的配置有关系。
                    try
                    {
                        this.CyclostyleFilePath = SystemConfig.PathOfDataUser + "\\CyclostyleFile\\" + templateRtfFile;
                        str = Cash.GetBillStr(templateRtfFile, false);
                        msg = "@已经成功的执行修复线  RepairLineV2，您重新发送一次或者，退后重新在发送一次，是否可以解决此问题";
                    }
                    catch (Exception ex1)
                    {
                        msg = "执行修复线失败.  RepairLineV2 " + ex1.Message;
                    }
                }
                throw new Exception("生成文档失败：单据名称[" + this.CyclostyleFilePath + "] 异常信息：" + ex.Message + " @自动修复单据信息：" + msg);
            }
            if (isOpen)
                PubClass.Print(HttpContextHelper.RequestApplicationPath + "Temp/" + file);
        }
        #endregion


        #region 生成单据
        #region 生成单据
        /// <summary>
        /// 生成单据根据
        /// </summary>
        /// <param name="templeteFile">模板文件</param>
        /// <param name="saveToFile"></param>
        /// <param name="mainDT"></param>
        /// <param name="dtls"></param>
        public void MakeDocByDataSet(string templeteFile, string saveToPath,
            string saveToFileName, DataTable mainDT, DataSet dtlsDS)
        {
            string valMain = DBAccess.RunSQLReturnString("SELECT NO FROM SYS_MapData");
            this.HisGEEntity = new GEEntity(valMain);
            this.HisGEEntity.Row.LoadDataTable(mainDT, mainDT.Rows[0]);
            this.AddEn(this.HisGEEntity); //增加一个主表。
            if (dtlsDS != null)
            {
                foreach (DataTable dt in dtlsDS.Tables)
                {
                    string dtlID = DBAccess.RunSQLReturnString("SELECT NO FROM SYS_MapDtl ");
                    BP.Sys.GEDtls dtls = new BP.Sys.GEDtls(dtlID);
                    foreach (DataRow dr in dt.Rows)
                    {
                        BP.Sys.GEDtl dtl = dtls.GetNewEntity as BP.Sys.GEDtl;
                        dtl.Row.LoadDataTable(dt, dr);
                        dtls.AddEntity(dtl);
                    }
                    this.AddDtlEns(dtls); //增加一个明晰。
                }
            }

            this.MakeDoc(templeteFile, saveToPath, saveToFileName,  false);
        }
        #endregion
        #endregion

        #region 方法
        /// <summary>
        /// RTFEngine
        /// </summary>
        public RTFEngine()
        {
            this._EnsDataDtls = null;
            this._HisEns = null;
        }
        /// <summary>
        /// 传入的是单个实体
        /// </summary>
        /// <param name="en"></param>
        public RTFEngine(Entity en)
        {
            this._EnsDataDtls = null;
            this._HisEns = null;
            this.HisGEEntity = en;
        }
        #endregion
    }


}
