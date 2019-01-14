using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BP.DA;
using LitJson;

namespace BP.Sys
{
    /// <summary>
    /// 解析控件并保存.
    /// </summary>
    public class CCFormParse
    {
        /// <summary>
        /// 保存元素
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="eleType">元素类型</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置</param>
        /// <param name="y">位置</param>
        /// <param name="h">高度</param>
        /// <param name="w">宽度</param>
        public static void SaveFrmEle(string fk_mapdata, string eleType, string ctrlID, float x, float y, float h, float w)
        {
            FrmEle en = new FrmEle();

            en.EleType = eleType;
            en.FK_MapData = fk_mapdata;
            en.EleID = ctrlID;
            
            int i = en.Retrieve(FrmEleAttr.FK_MapData, fk_mapdata, FrmEleAttr.EleID, ctrlID);
            en.X = x;
            en.Y = y;
            en.W = w;
            en.H = h;

            if (i == 0)
                en.Insert();
            else
                en.Update();
        }
        /// <summary>
        /// 保存一个rb
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public static string SaveFrmRadioButton(string fk_mapdata, string ctrlID, float x, float y)
        {
            FrmRB en = new FrmRB();
            en.MyPK = fk_mapdata + "_" + ctrlID;
            int i= en.RetrieveFromDBSources();
            if (i == 0)
                return null;

            en.FK_MapData = fk_mapdata;
            en.X = x;
            en.Y = y;
            en.Update();
            return en.KeyOfEn;
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">空间ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="h">高度h</param>
        /// <param name="w">宽度w</param>
        public static void SaveAthImg(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            FrmImgAth en = new FrmImgAth();
            en.MyPK = fk_mapdata + "_" + ctrlID;
            en.FK_MapData = fk_mapdata;
            en.CtrlID = ctrlID;
            en.RetrieveFromDBSources();

            en.X = x;
            en.Y = y;
            en.W = w;
            en.H = h;
            en.Update();
        }
        /// <summary>
        /// 保存多附件
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="h">高度</param>
        /// <param name="w">宽度</param>
        public static void SaveAthMulti(string fk_mapdata,string ctrlID, float x, float y, float h, float w)
        {
            FrmAttachment en = new FrmAttachment();
            en.MyPK = fk_mapdata + "_" + ctrlID;
            en.FK_MapData = fk_mapdata;
            en.NoOfObj = ctrlID;
            en.RetrieveFromDBSources();

            en.X = x;
            en.Y = y;
            en.W = w;
            en.H = h;
            en.Update();
        }
        public static void SaveDtl(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            MapDtl dtl = new MapDtl();
            dtl.No = ctrlID;
            dtl.RetrieveFromDBSources();

            dtl.FK_MapData = fk_mapdata;
            dtl.X = x;
            dtl.Y = y;
            dtl.W = w;
            dtl.H = h;
            dtl.Update();
        }
        public static void SaveiFrame(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            FrmEle en = new FrmEle();
            en.FK_MapData = fk_mapdata;
            en.EleID = ctrlID;
            en.MyPK = en.FK_MapData + "_" + en.EleID;
            if (en.RetrieveFromDBSources() == 0)
                en.Insert();

            en.X = x;
            en.Y = y;
            en.W = w;
            en.H = h;
            en.Update();
        }
        public static void SaveMapAttr(string fk_mapdata, string fieldID, string shape, JsonData control, JsonData properties, string pks)
        {
            MapAttr attr = new MapAttr();
            attr.FK_MapData = fk_mapdata;
            attr.KeyOfEn = fieldID;
            attr.MyPK = fk_mapdata + "_" + fieldID;
            attr.RetrieveFromDBSources();

            //if (attr.KeyOfEn == "BiaoTi")
            //{
            //    int i = 11;
            //}

            //执行一次查询,以防止其他的属性更新错误.
            //if (pks.Contains("@" + attr.KeyOfEn + "@") == true)
            //    attr.RetrieveFromDBSources();

            switch (shape)
            {
                case "TextBoxStr":  //文本类型.
                case "TextBoxSFTable":
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "TextBoxInt": //数值
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.MyDataType = DataType.AppInt;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "TextBoxBoolean":
                    attr.MyDataType = DataType.AppBoolean;
                    attr.UIContralType = En.UIContralType.CheckBok;
                    attr.LGType = En.FieldTypeS.Normal;
                    break;
                case "TextBoxFloat":
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "TextBoxMoney":
                    attr.MyDataType = DataType.AppMoney;
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "TextBoxDate":
                    attr.MyDataType = DataType.AppDate;
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "TextBoxDateTime":
                    attr.MyDataType = DataType.AppDateTime;
                    attr.LGType = En.FieldTypeS.Normal;
                    attr.UIContralType = En.UIContralType.TB;
                    break;
                case "DropDownListEnum": //枚举类型.
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.LGType = En.FieldTypeS.Enum;
                    attr.UIContralType = En.UIContralType.DDL;
                    break;
                case "DropDownListTable": //外键类型.
                    attr.MyDataType = BP.DA.DataType.AppString;
                    if (pks.Contains("@" + attr.KeyOfEn + "@") == false)
                        attr.LGType = En.FieldTypeS.FK;
                    attr.UIContralType = En.UIContralType.DDL;
                    attr.MaxLen = 100;
                    attr.MinLen = 0;
                    break;
                default:
                    break;
            }

            //坐标
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            attr.X = float.Parse(vector[0].ToJson());
            attr.Y = float.Parse(vector[1].ToJson());

            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];  //获得一个属性.
                if (property == null || !property.Keys.Contains("property")
                    || property["property"] == null
                    || property["property"].ToString() == "group")
                    continue;

                string val = null;
                if (property["PropertyValue"] != null)
                    val = property["PropertyValue"].ToString();
                string propertyName = property["property"].ToString();
                switch (propertyName)
                {
                    case "Name":
                        if (attr.Name == "")
                            attr.Name = val;
                        break;
                    case "MinLen":
                    case "MaxLen":
                    case "DefVal":
                        attr.SetValByKey(propertyName, val);
                        break;
                    case "UIIsEnable":
                    case "UIVisible":
                        attr.SetValByKey(propertyName, val);
                        break;
                    case "FieldText":
                        if (attr.Name=="")
                           attr.Name = val;
                        break;
                    case "UIIsInput":
                        if (val == "true")
                            attr.UIIsInput = true;
                        else
                            attr.UIIsInput = false;
                        break;
                    case "UIBindKey":
                        attr.UIBindKey =val;
                        break;
                    default:
                        break;
                }
            }


            //Textbox 高、宽.
            decimal minX = decimal.Parse(vector[0].ToJson());
            decimal minY = decimal.Parse(vector[1].ToJson());
            decimal maxX = decimal.Parse(vector[2].ToJson());
            decimal maxY = decimal.Parse(vector[3].ToJson());
            decimal imgWidth = maxX - minX;
            decimal imgHeight = maxY - minY;

            attr.UIWidth = float.Parse(imgWidth.ToString("0.00"));
            attr.UIHeight = float.Parse(imgHeight.ToString("0.00"));

          //  attr.ColSpan

            if (pks.Contains("@" + attr.KeyOfEn + "@") == true)
            {
                attr.Update();
            }
            else
            {
                attr.Insert();
            }
        }

        #region 装饰类控件.
        /// <summary>
        /// 保存线.
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="form_Lines"></param>
        public static void SaveLine(string fk_mapdata,JsonData form_Lines)
        {
            //标签.
            string linePKs = "@";
            FrmLines lines = new FrmLines();
            lines.Retrieve(FrmLabAttr.FK_MapData, fk_mapdata);
            foreach (FrmLine item in lines)
                linePKs += item.MyPK + "@";

            if (form_Lines.IsArray == true && form_Lines.Count > 0)
            {
                for (int idx = 0, jLine = form_Lines.Count; idx < jLine; idx++)
                {
                    JsonData line = form_Lines[idx];
                    if (line.IsObject == false)
                        continue;

                    FrmLine lineEn = new FrmLine();

                    lineEn.MyPK = line["CCForm_MyPK"].ToString();
                    lineEn.FK_MapData = fk_mapdata;

                    JsonData turningPoints = line["turningPoints"];
                    lineEn.X1 = float.Parse(turningPoints[0]["x"].ToString());
                    lineEn.X2 = float.Parse(turningPoints[1]["x"].ToString());
                    lineEn.Y1 = float.Parse(turningPoints[0]["y"].ToString());
                    lineEn.Y2 = float.Parse(turningPoints[1]["y"].ToString());

                    JsonData properties = line["properties"];
                    JsonData borderWidth = properties.GetObjectFromArrary_ByKeyValue("type", "LineWidth");
                    JsonData borderColor = properties.GetObjectFromArrary_ByKeyValue("type", "Color");
                    string strborderWidth = "2";
                    if (borderWidth != null && borderWidth["PropertyValue"] != null && !DataType.IsNullOrEmpty(borderWidth["PropertyValue"].ToString()))
                    {
                        strborderWidth = borderWidth["PropertyValue"].ToString();
                    }
                    string strBorderColor = "Black";
                    if (borderColor != null && borderColor["PropertyValue"] != null && !DataType.IsNullOrEmpty(borderColor["PropertyValue"].ToString()))
                    {
                        strBorderColor = borderColor["PropertyValue"].ToString();
                    }
                    lineEn.BorderWidth = float.Parse(strborderWidth);
                    lineEn.BorderColor = strBorderColor;
                    if (linePKs.Contains("@"+lineEn.MyPK + "@") == true)
                    {
                        linePKs = linePKs.Replace(lineEn.MyPK + "@", "");
                        lineEn.DirectUpdate();
                    }
                    else
                        lineEn.DirectInsert();
                }
            }

            //删除找不到的Line.
            string[] strs = linePKs.Split('@');
            string sqls = "";
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLine WHERE MyPK='" + str + "'";
            }
            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        public static void SaveLabel(string fk_mapdata, JsonData control, JsonData properties, string pks, string ctrlID)
        {
            // New lab 对象.
            FrmLab lab = new FrmLab();
            lab.MyPK = ctrlID;
            lab.FK_MapData = fk_mapdata;

            //坐标.
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            lab.X = float.Parse(vector[0].ToJson());
            lab.Y = float.Parse(vector[1].ToJson());

            StringBuilder fontStyle = new StringBuilder();
            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];
                if (property == null || !property.Keys.Contains("type") || property["type"] == null)
                    continue;

                string type = property["type"].ToString().Trim();
                string val = null;
                if (property["PropertyValue"] != null)
                    val = property["PropertyValue"].ToString();

                switch (type)
                {
                    case "SingleText":
                        lab.Text = val == null ? "" : val.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                        break;
                    case "Color":
                        // lab.FontColor = val == null ? "#FF000000" : val.ToString();
                        lab.FontColor = val == null ? "#000000" : val.ToString();
                        fontStyle.Append(string.Format("color:{0};", lab.FontColor));
                        break;
                    case "TextFontFamily":
                        lab.FontName = val == null ? "Portable User Interface" : val.ToString();
                        if (val != null)
                            fontStyle.Append(string.Format("font-family:{0};", property["PropertyValue"].ToJson()));
                        break;
                    case "TextFontSize":
                        lab.FontSize = val == null ? 14 : int.Parse(val.ToString());
                        fontStyle.Append(string.Format("font-size:{0};", lab.FontSize));
                        break;
                    case "FontWeight":
                        if (val == null || val.ToString() == "normal")
                        {
                            lab.IsBold = false;
                            fontStyle.Append(string.Format("font-weight:normal;"));
                        }
                        else
                        {
                            lab.IsBold = true;
                            fontStyle.Append(string.Format("font-weight:{0};", val));
                        }
                        break;
                    default:
                        break;
                }
            }

            if (lab.Text == null || lab.Text == "")
            {
                /*如果没有取到标签， 从这里获取，系统有一个. */
                JsonData primitives = control["primitives"][0];
                lab.Text = primitives["str"].ToString().Trim();
                lab.FontName = primitives["font"].ToString().Trim();
                lab.FontSize = int.Parse(primitives["size"].ToString().Trim());
            }

            lab.FontStyle = fontStyle.ToString();
            if (pks.Contains(lab.MyPK + "@") == true)
            {
                lab.DirectUpdate();
            }
            else
            {
                lab.DirectInsert();
            }
        }
        public static void SaveButton(string fk_mapdata, JsonData control, JsonData properties, string pks,string ctrlID)
        {
            FrmBtn btn = new FrmBtn(ctrlID);
            btn.MyPK = ctrlID;
            btn.FK_MapData = fk_mapdata;

            //坐标
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            btn.X = float.Parse( vector[0].ToJson());
            btn.Y = float.Parse(vector[1].ToJson());
            btn.IsEnable = true;
            /*for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];
                if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                    continue;

                string val = null;
                if (property["PropertyValue"] != null)
                    val = property["PropertyValue"].ToString();

                string propertyBtn = property["property"].ToString();
                switch (propertyBtn)
                {
                    case "primitives.1.str":
                        btn.Text = val == null ? "" : val.Replace(" ", "&nbsp;").Replace("\n", "@");
                        break;
                    case "ButtonEvent":
                        btn.EventType = val == null ? 0 : int.Parse(val);
                        break;
                    case "BtnEventDoc":
                        btn.EventContext = val == null ? "" : val;
                        break;
                    default:
                        break;
                }
            }*/
            if (pks.Contains("@" + btn.MyPK + "@") == true)
                btn.DirectUpdate();
            else
                btn.DirectInsert();
        }

        public static void SaveHyperLink(string fk_mapdata,JsonData control, JsonData properties, string pks, string ctrlID)
        {
            FrmLink link = new FrmLink(ctrlID);
            link.MyPK = ctrlID;
            link.FK_MapData = fk_mapdata;
            //坐标
            JsonData vector = control["style"]["gradientBounds"];
            link.X = float.Parse( vector[0].ToJson());
            link.Y= float.Parse( vector[1].ToJson());

            //属性集合
            StringBuilder fontStyle = new StringBuilder();
            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];
                if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                    continue;

                string propertyLink = property["property"].ToString();
                var valLink = property["PropertyValue"];

                switch (propertyLink)
                {
                    //case "primitives.0.str":
                    //case "SingleText":
                    //    link.Text  = valLink == null ? "" : valLink.ToString();
                    //    break;
                    case "primitives.0.style.fillStyle":
                        link.FontColor = valLink == null ? "#FF000000" : valLink.ToString();
                        fontStyle.Append(string.Format("color:{0};", link.FontColor));
                        break;
                    case "FontName":
                        link.FontName= valLink == null ? "Portable User Interface" : valLink.ToString();
                        if (valLink != null)
                            fontStyle.Append(string.Format("font-family:{0};", valLink.ToJson()));
                        continue;
                    case "FontSize":
                       link.FontSize = valLink == null ? 14 : int.Parse(  valLink.ToString());
                       fontStyle.Append(string.Format("font-size:{0};", link.FontSize));
                        continue;
                    case "primitives.0.fontWeight":
                        if (valLink == null || valLink.ToString() == "normal")
                        {
                            link.IsBold = false;
                            fontStyle.Append(string.Format("font-weight:normal;"));
                        }
                        else
                        {
                            link.IsBold = true;
                            fontStyle.Append(string.Format("font-weight:{0};", valLink.ToString()));
                        }
                        continue;
                    case "FontColor":
                        link.FontColor = valLink == null ? "" : valLink.ToString();
                        continue;
                    //case "URL":
                     //   link.URL = valLink == null ? "" : valLink.ToString();
                     //   continue;
                    //case "WinOpenModel":
                     ///   link.Target = valLink == null ? "_blank" : valLink.ToString();
                    //    continue;
                    default:
                        break;
                }
            }
            link.FontStyle = fontStyle.ToString();

            if (link.Text == null || link.Text == "")
            {
                /*如果没有取到标签， 从这里获取，系统有一个. */
                JsonData primitives = control["primitives"][0];
                link.Text = primitives["str"].ToString().Trim();
                link.FontName = primitives["font"].ToString().Trim();
                link.FontSize = int.Parse(primitives["size"].ToString().Trim());
            }

            //执行保存.
            if (pks.Contains("@" + link.MyPK + "@") == true)
            {
                link.DirectUpdate();
            }
            else
            {
                link.DirectInsert();
            }
        }
        public static void SaveImage(string fk_mapdata,  JsonData control, JsonData properties, string pks, string ctrlID)
        {
            FrmImg img = new FrmImg();
            img.MyPK = ctrlID;
            img.FK_MapData = fk_mapdata;
            img.IsEdit = 1;
            img.HisImgAppType = ImgAppType.Img;
        
            //坐标
            JsonData vector = control["style"]["gradientBounds"];
            img.X = float.Parse( vector[0].ToJson());
            img.Y = float.Parse( vector[1].ToJson());
            //图片高、宽
            decimal minX = decimal.Parse(vector[0].ToJson());
            decimal minY = decimal.Parse(vector[1].ToJson());
            decimal maxX = decimal.Parse(vector[2].ToJson());
            decimal maxY = decimal.Parse(vector[3].ToJson());
            decimal imgWidth = maxX - minX;
            decimal imgHeight = maxY - minY;

            img.W = float.Parse( imgWidth.ToString("0.00"));
            img.H = float.Parse(imgHeight.ToString("0.00"));

            StringBuilder fontStyle = new StringBuilder();
            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];
                if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                    continue;

                if (property["property"].ToString() == "LinkURL")
                {
                    //图片连接到
                    img.LinkURL = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                }
                else if (property["property"].ToString() == "WinOpenModel")
                {
                    //打开窗口方式
                    img.LinkTarget = property["PropertyValue"] == null ? "_blank" : property["PropertyValue"].ToString();
                }
                else if (property["property"].ToString() == "ImgAppType")
                {
                    //应用类型：0本地图片，1指定路径.
                    img.ImgSrcType = property["PropertyValue"] == null ? 0 : int.Parse(property["PropertyValue"].ToString());
                }
                else if (property["property"].ToString() == "ImgPath")
                {
                    //指定图片路径
                    img.ImgURL = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                }
            }

            //ImageFrame 本地图片路径
            JsonData primitives = control["primitives"];
            foreach (JsonData primitive in primitives)
            {
                if (primitive["oType"] == null)
                    continue;
                if (primitive["oType"].ToJson() == "ImageFrame")
                {
                    img.ImgPath= primitive == null ? "" : primitive["url"].ToString();
                }
            }

            //执行保存.
            if (pks.Contains(img.MyPK + "@") == true)
                img.DirectUpdate();
            else
                img.DirectInsert();
        }
        #endregion 装饰类控件.

    }
}
