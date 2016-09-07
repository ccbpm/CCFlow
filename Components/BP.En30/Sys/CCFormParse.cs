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

        public static void SaveDtl(string fk_mapdata, DataTable dtlDT, string ctrlID, float x, float y)
        {
            //dtlDT
            DataRow dr = dtlDT.NewRow();
            dr["No"] =  ctrlID;

            //增加到集合里.
            dtlDT.Rows.Add(dr);

            MapDtl dtl = new MapDtl();
            dtl.No = ctrlID;
            dtl.RetrieveFromDBSources();

            dtl.X = x;
            dtl.Y = y;

            dtl.Update();
        }
        public static void SaveMapAttr(string fk_mapdata, string ctrlID, string shape, DataTable dtMapAttr, JsonData control, JsonData properties)
        {
            DataRow drAttr = dtMapAttr.NewRow();

            drAttr["KeyOfEn"] = ctrlID;
            drAttr["MYPK"] = fk_mapdata + "_" + ctrlID;
            drAttr["FK_MAPDATA"] = fk_mapdata;

            switch (shape)
            {
                case "TextBoxStr":
                case "TextBoxSFTable":
                    drAttr["MyDataType"] = DataType.AppString;
                    break;
                case "TextBoxInt":
                case "TextBoxEnum":
                    drAttr["MyDataType"] = DataType.AppInt;
                    break;
                case "TextBoxFloat":
                    drAttr["MyDataType"] = DataType.AppFloat;
                    break;
                case "TextBoxMoney":
                    drAttr["MyDataType"] = DataType.AppMoney;
                    break;
                case "TextBoxDate":
                    drAttr["MyDataType"] = DataType.AppDate;
                    break;
                case "TextBoxDateTime":
                    drAttr["MyDataType"] = DataType.AppDateTime;
                    break;
                default:
                    break;
            }

            //坐标
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            drAttr["X"] = vector[0].ToJson();
            drAttr["Y"] = vector[1].ToJson();

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
                    case "MinLen":
                    case "MaxLen":
                    case "DefVal":
                        //case "UIWidth":
                        //case "UIHeight":
                        drAttr[propertyName] = val;
                        break;
                    case "UIIsEnable":
                    case "UIVisible":
                        string type = property["type"].ToString();
                        drAttr[propertyName] = val;
                        break;
                    case "FieldText":
                        drAttr["Name"] = val;
                        break;
                    case "UIIsInput":
                        if (val == "true")
                            drAttr["UIIsInput"] = "1";
                        else
                            drAttr["UIIsInput"] = "0";
                        break;
                    default:
                        break;
                }

                //Textbox 高、宽.
                decimal minX = decimal.Parse(vector[0].ToJson());
                decimal minY = decimal.Parse(vector[1].ToJson());
                decimal maxX = decimal.Parse(vector[2].ToJson());
                decimal maxY = decimal.Parse(vector[3].ToJson());
                decimal imgWidth = maxX - minX;
                decimal imgHeight = maxY - minY;
                drAttr["UIWidth"] = imgWidth.ToString("0.00");
                drAttr["UIHeight"] = imgHeight.ToString("0.00");

                //控件类型.
                switch (shape)
                {
                    case "TextBoxStr":
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppString;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        break;
                    case "TextBoxInt":
                    case "TextBoxBoolean":
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppInt;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "TextBoxFloat":
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppFloat;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "TextBoxDouble":
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppDouble;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "TextBoxMoney": //金额类型.
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppMoney;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "TextBoxDate": //日期.
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppDate;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "TextBoxDateTime": //金额类型.
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppDateTime;
                        drAttr["LGTYPE"] = "0";
                        drAttr["UICONTRALTYPE"] = "0";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "DropDownListEnum": //枚举类型.
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppInt;
                        drAttr["LGTYPE"] = "1";
                        drAttr["UICONTRALTYPE"] = "1";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "16";
                        break;
                    case "DropDownListTable": //外键类型.
                        drAttr["MYDATATYPE"] = BP.DA.DataType.AppString;
                        drAttr["LGTYPE"] = "2";
                        drAttr["UICONTRALTYPE"] = "1";
                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "100";
                        break;
                    default:
                        break;
                }
            }

            //增加到集合.
            dtMapAttr.Rows.Add(drAttr);
        }

        #region 装饰类控件.
        public static void SaveLabel(string fk_mapdata, DataTable dtLabel,JsonData control, JsonData properties )
        {
            DataRow drLab = dtLabel.NewRow();
            drLab["MYPK"] = control["CCForm_MyPK"];
            drLab["FK_MAPDATA"] = fk_mapdata;

            //坐标.
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            drLab["X"] = vector[0].ToJson();
            drLab["Y"] = vector[1].ToJson();

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
                        drLab["TEXT"] = val == null ? "" : val.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                        break;
                    case "Color":
                        drLab["FONTCOLOR"] = val == null ? "#FF000000" : val.ToString();
                        fontStyle.Append(string.Format("color:{0};", drLab["FONTCOLOR"]));
                        break;
                    case "TextFontFamily":
                        drLab["FontName"] = val == null ? "Portable User Interface" : val.ToString();
                        if (val != null)
                            fontStyle.Append(string.Format("font-family:{0};", property["PropertyValue"].ToJson()));
                        break;
                    case "TextFontSize":
                        drLab["FONTSIZE"] = val == null ? "14" : val.ToString();
                        fontStyle.Append(string.Format("font-size:{0};", drLab["FONTSIZE"]));
                        break;
                    case "FontWeight":
                        if (val == null || val.ToString() == "normal")
                        {
                            drLab["IsBold"] = "0";
                            fontStyle.Append(string.Format("font-weight:normal;"));
                        }
                        else
                        {
                            drLab["IsBold"] = "1";
                            fontStyle.Append(string.Format("font-weight:{0};", val));
                        }
                        break;
                    default:
                        break;
                }
            }

            if (drLab["TEXT"] == null || drLab["TEXT"] == "")
            {
                /*如果没有取到标签， 从这里获取，系统有一个. */
                JsonData primitives = control["primitives"][0];
                drLab["TEXT"] = primitives["str"].ToString().Trim();
                drLab["FontName"] = primitives["font"].ToString().Trim();
                drLab["FontSize"] = primitives["size"].ToString().Trim();
            }

            drLab["FontStyle"] = fontStyle.ToString();
            drLab["IsItalic"] = "0";//暂不处理斜体
            dtLabel.Rows.Add(drLab);
        }

        public static void SaveButton(string fk_mapdata, DataTable dtBtn, JsonData control, JsonData properties)
        {
            DataRow drBtn = dtBtn.NewRow();

            drBtn["MYPK"] = control["CCForm_MyPK"];
            drBtn["FK_MAPDATA"] = fk_mapdata;
            //坐标
            JsonData style = control["style"];
            JsonData vector = style["gradientBounds"];
            drBtn["X"] = vector[0].ToJson();
            drBtn["Y"] = vector[1].ToJson();
            drBtn["ISVIEW"] = "1";
            drBtn["ISENABLE"] = "1";
            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
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
                        drBtn["TEXT"] = val == null ? "" : val.Replace(" ", "&nbsp;").Replace("\n", "@");
                        break;
                    case "ButtonEvent":
                        drBtn["EVENTTYPE"] = val == null ? "0" : val;
                        break;
                    case "BtnEventDoc":
                        drBtn["EVENTCONTEXT"] = val == null ? "" : val;
                        break;
                    default:
                        break;
                }
            }
            dtBtn.Rows.Add(drBtn);
        }

        public static void SaveHyperLink(string fk_mapdata, DataTable dtLink, JsonData control, JsonData properties)
        {
            DataRow drLink = dtLink.NewRow();

            drLink["MYPK"] = control["CCForm_MyPK"];
            drLink["FK_MAPDATA"] = fk_mapdata;
            //坐标
            JsonData vector = control["style"]["gradientBounds"];
            drLink["X"] = vector[0].ToJson();
            drLink["Y"] = vector[1].ToJson();

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
                    case "primitives.0.str":
                    case "SingleText":
                        drLink["TEXT"] = valLink == null ? "" : valLink.ToString();
                        break;
                    case "primitives.0.style.fillStyle":
                        drLink["FONTCOLOR"] = valLink == null ? "#FF000000" : valLink.ToString();
                        fontStyle.Append(string.Format("color:{0};", drLink["FONTCOLOR"]));
                        break;
                    case "FontName":
                        drLink["FontName"] = valLink == null ? "Portable User Interface" : valLink.ToString();
                        if (valLink != null)
                            fontStyle.Append(string.Format("font-family:{0};", valLink.ToJson()));
                        continue;
                    case "FontSize":
                        drLink["FONTSIZE"] = valLink == null ? "14" : valLink.ToString();
                        fontStyle.Append(string.Format("font-size:{0};", drLink["FONTSIZE"]));
                        continue;
                    case "primitives.0.fontWeight":
                        if (valLink == null || valLink.ToString() == "normal")
                        {
                            drLink["IsBold"] = "0";
                            fontStyle.Append(string.Format("font-weight:normal;"));
                        }
                        else
                        {
                            drLink["IsBold"] = "1";
                            fontStyle.Append(string.Format("font-weight:{0};", valLink.ToString()));
                        }
                        continue;
                    case "FontColor":
                        drLink["FontColor"] = valLink == null ? "" : valLink.ToString();
                        continue;
                    case "URL":
                        drLink["URL"] = valLink == null ? "" : valLink.ToString();
                        continue;
                    case "WinOpenModel":
                        drLink["TARGET"] = valLink == null ? "_blank" : valLink.ToString();
                        continue;
                    default:
                        break;
                }
            }
            drLink["FontStyle"] = fontStyle.ToString();
            drLink["IsItalic"] = "0"; //斜体暂不处理.
            dtLink.Rows.Add(drLink);
        }
        public static void SaveImage(string fk_mapdata, DataTable dtImg, JsonData control, JsonData properties)
        {
            DataRow drImg = dtImg.NewRow();

            drImg["MYPK"] = control["CCForm_MyPK"];
            drImg["FK_MAPDATA"] = fk_mapdata;
            drImg["IMGAPPTYPE"] = "0";
            drImg["ISEDIT"] = "1";
            drImg["NAME"] = drImg["MYPK"];
            drImg["ENPK"] = drImg["MYPK"];
            //坐标
            JsonData vector = control["style"]["gradientBounds"];
            drImg["X"] = vector[0].ToJson();
            drImg["Y"] = vector[1].ToJson();
            //图片高、宽
            decimal minX = decimal.Parse(vector[0].ToJson());
            decimal minY = decimal.Parse(vector[1].ToJson());
            decimal maxX = decimal.Parse(vector[2].ToJson());
            decimal maxY = decimal.Parse(vector[3].ToJson());
            decimal imgWidth = maxX - minX;
            decimal imgHeight = maxY - minY;

            drImg["W"] = imgWidth.ToString("0.00");
            drImg["H"] = imgHeight.ToString("0.00");

            StringBuilder fontStyle = new StringBuilder();
            for (int iProperty = 0; iProperty < properties.Count; iProperty++)
            {
                JsonData property = properties[iProperty];
                if (property == null || !property.Keys.Contains("property") || property["property"] == null)
                    continue;

                if (property["property"].ToString() == "LinkURL")
                {
                    //图片连接到
                    drImg["LINKURL"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
                }
                else if (property["property"].ToString() == "WinOpenModel")
                {
                    //打开窗口方式
                    drImg["LINKTARGET"] = property["PropertyValue"] == null ? "_blank" : property["PropertyValue"].ToString();
                }
                else if (property["property"].ToString() == "ImgAppType")
                {
                    //应用类型：0本地图片，1指定路径
                    drImg["SRCTYPE"] = property["PropertyValue"] == null ? "0" : property["PropertyValue"].ToString();
                }
                else if (property["property"].ToString() == "ImgPath")
                {
                    //指定图片路径
                    drImg["IMGURL"] = property["PropertyValue"] == null ? "" : property["PropertyValue"].ToString();
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
                    drImg["IMGPATH"] = primitive == null ? "" : primitive["url"].ToString();
                }
            }
            dtImg.Rows.Add(drImg);
        }
        #endregion 装饰类控件.

    }
}
