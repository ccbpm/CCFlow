using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.En;
using BP.DA;
using LitJson;

namespace BP.Sys
{
      public class EEleTableNames
      {
        public const string
            Sys_FrmLine = "Sys_FrmLine",
            Sys_FrmBtn = "Sys_FrmBtn",
            Sys_FrmLab = "Sys_FrmLab",
            Sys_FrmLink = "Sys_FrmLink",
            Sys_FrmImg = "Sys_FrmImg",
            Sys_FrmEle = "Sys_FrmEle",
            Sys_FrmImgAth = "Sys_FrmImgAth",
            Sys_FrmRB = "Sys_FrmRB",
            Sys_FrmAttachment = "Sys_FrmAttachment",
            Sys_MapData = "Sys_MapData",
            Sys_MapAttr = "Sys_MapAttr",
            Sys_MapDtl = "Sys_MapDtl",
            Sys_MapM2M = "Sys_MapM2M",
            WF_Node = "WF_Node"; 
      }
    /// <summary>
    /// 表单API
    /// </summary>
    public class CCFormAPI
    {
        #region 创建修改字段.
        /// <summary>
        /// 创建通用组件入口
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlType">控件类型</param>
        /// <param name="no">编号</param>
        /// <param name="name">名称</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public static void PublicNoNameCtrlCreate(string fk_mapdata, string ctrlType, string no, string name, float x, float y)
        {
            switch (ctrlType)
            {
                case "Dtl":
                    CreateOrSaveDtl(fk_mapdata, no, name, x, y);
                    break;
                case "AthMulti":
                    CreateOrSaveAthMulti(fk_mapdata, no, name, x, y);
                    break;
                case "AthSingle":
                    CreateOrSaveAthSingle(fk_mapdata, no, name, x, y);
                    break;
                case "AthImg":
                    CreateOrSaveAthImg(fk_mapdata, no, name, x, y);
                    break;
                default:
                    throw new Exception("@没有判断的存储控件:"+ctrlType+",存储该控件前,需要做判断.");
            }
        }
        /// <summary>
        /// 创建/修改-图片附件
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dtlNo">明细表编号</param>
        /// <param name="dtlName">名称</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public static void CreateOrSaveAthImg(string fk_mapdata, string no, string name, float x, float y)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = fk_mapdata;
            ath.NoOfObj = no;

            ath.MyPK = ath.FK_MapData + "_" + ath.NoOfObj;
            ath.RetrieveFromDBSources();
            ath.UploadType = AttachmentUploadType.Single;
            ath.Name = name;
            ath.X = x;
            ath.Y = y;
            ath.Save();
        }
        /// <summary>
        /// 创建/修改-多附件
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dtlNo">明细表编号</param>
        /// <param name="dtlName">名称</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public static void CreateOrSaveAthSingle(string fk_mapdata, string no, string name, float x, float y)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = fk_mapdata;
            ath.NoOfObj = no;

            ath.MyPK = ath.FK_MapData + "_" + ath.NoOfObj;
            ath.RetrieveFromDBSources();
            ath.UploadType = AttachmentUploadType.Single;
            ath.Name = name;
            ath.X = x;
            ath.Y = y;
            ath.Save();
        }
        /// <summary>
        /// 创建/修改-多附件
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dtlNo">明细表编号</param>
        /// <param name="dtlName">名称</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void CreateOrSaveAthMulti(string fk_mapdata, string no, string name, float x, float y)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = fk_mapdata;
            ath.NoOfObj = no;

            ath.MyPK = ath.FK_MapData + "_" + ath.NoOfObj;
            ath.RetrieveFromDBSources();
            ath.UploadType = AttachmentUploadType.Multi;
            ath.Name = name;
            ath.X = x;
            ath.Y = y;
            ath.Save();
        }
        /// <summary>
        /// 创建/修改一个明细表
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dtlNo">明细表编号</param>
        /// <param name="dtlName">名称</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void CreateOrSaveDtl(string fk_mapdata, string dtlNo, string dtlName, float x, float y)
        {
            MapDtl dtl = new MapDtl();
            dtl.No = dtlNo;
            dtl.RetrieveFromDBSources();

            dtl.X = x;
            dtl.Y = y;
            dtl.Name = dtlName;
            dtl.FK_MapData = fk_mapdata;
            dtl.Save();

            #region 检查必要的字段是否存在.
            MapAttr attr = new MapAttr();
            if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, dtl.No) == false)
            {
                attr.FK_MapData = dtl.No;
                attr.KeyOfEn = "OID";
                attr.Name = "关联主键";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, "RefPK", MapAttrAttr.FK_MapData, dtl.No) == false)
            {
                attr.FK_MapData = dtl.No;
                attr.KeyOfEn = "RefPK";
                attr.Name = "关联主键";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }
            #endregion 检查必要的字段是否存在.



        }
        /// <summary>
        /// 创建一个外部数据字段
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDesc">字段中文名</param>
        /// <param name="fk_SFTable">外键表</param>
        /// <param name="x">位置</param>
        /// <param name="y">位置</param>
        /// <param name="colSpan">跨的列数</param>
        public static void SaveFieldSFTable(string fk_mapdata, string fieldName, string fieldDesc, string fk_SFTable, float x, float y, int colSpan = 1)
        {
            //外键字段表.
            SFTable sf = new SFTable(fk_SFTable);

            MapAttr attr = new MapAttr();
            attr.MyPK = fk_mapdata + "_" + fieldName;
            attr.RetrieveFromDBSources();

            //基本属性赋值.
            attr.FK_MapData = fk_mapdata;
            attr.KeyOfEn = fieldName;
            attr.Name = fieldDesc;
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = BP.En.UIContralType.DDL;
            attr.UIBindKey = fk_SFTable; //绑定信息.
            attr.X = x;
            attr.Y = y;

            //根据外键表的类型不同，设置它的LGType.
            switch (sf.SrcType)
            {
                case SrcType.BPClass:
                case SrcType.CreateTable:
                case SrcType.TableOrView:
                    attr.LGType = FieldTypeS.FK;
                    break;
                default:
                    attr.LGType = FieldTypeS.Normal;
                    break;
            }
            attr.Save();
           
            //如果是普通的字段, 这个属于外部数据类型,或者webservices类型.
            if (attr.LGType == FieldTypeS.Normal)
            {
                MapAttr attrH = new MapAttr();
                attrH.Copy(attr);
                attrH.KeyOfEn = attr.KeyOfEn + "T";
                attrH.Name = attr.Name;
                attrH.UIContralType = BP.En.UIContralType.TB;
                attrH.MinLen = 0;
                attrH.MaxLen = 60;
                attrH.MyDataType = BP.DA.DataType.AppString;
                attrH.UIVisible = false;
                attrH.UIIsEnable = false;
                attrH.MyPK = attrH.FK_MapData + "_" + attrH.KeyOfEn;
                attrH.Save();
            }
        }
        /// <summary>
        /// 保存枚举字段
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDesc">字段描述</param>
        /// <param name="enumKey">枚举值</param>
        /// <param name="ctrlType">显示的控件类型</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="colSpan">横跨的行数</param>
        public static void SaveFieldEnum(string fk_mapdata, string fieldName, string fieldDesc, string enumKey, UIContralType ctrlType,
            float x, float y, int colSpan = 1)
        {
            MapAttr ma = new MapAttr();
            ma.FK_MapData = fk_mapdata;
            ma.KeyOfEn = fieldName;

            //赋值主键。
            ma.MyPK = ma.FK_MapData + "_" + ma.KeyOfEn;

            //先查询赋值.
            ma.RetrieveFromDBSources();

            ma.Name = fieldDesc;
            ma.MyDataType = DataType.AppInt;
            ma.X = x;
            ma.Y = y;
            ma.UIIsEnable = true;
            ma.LGType = FieldTypeS.Enum;

            ma.UIContralType = ctrlType;
            ma.UIBindKey = enumKey;

            if (ma.UIContralType == UIContralType.RadioBtn)
            {
                SysEnums ses = new SysEnums(ma.UIBindKey);
                int idx = 0;
                foreach (SysEnum item in ses)
                {
                    idx++;
                    FrmRB rb = new FrmRB();
                    rb.FK_MapData = ma.FK_MapData;
                    rb.KeyOfEn = ma.KeyOfEn;
                    rb.IntKey = item.IntKey;
                    rb.MyPK = rb.FK_MapData + "_" + rb.KeyOfEn + "_" + rb.IntKey;
                    rb.RetrieveFromDBSources();
                   
                    rb.EnumKey = ma.UIBindKey;
                    rb.Lab = item.Lab;
                    rb.X = ma.X;

                    //让其变化y值.
                    rb.Y = ma.Y + idx * 30;
                    rb.Save();
                }
            }
            ma.Save();
        }
        #endregion 创建修改字段.

        #region 模版操作.
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="jsonStrOfH5Frm"></param>
        public static void SaveFrm(string fk_mapdata, string jsonStrOfH5Frm)
        {
            JsonData jd = JsonMapper.ToObject(jsonStrOfH5Frm);
            if (jd.IsObject == false)
                throw new Exception("error:表单格式不正确，保存失败。");

            JsonData form_MapData = jd["c"];

            //直接保存表单图信息.
            MapData mapData = new MapData(fk_mapdata);
            mapData.FrmW = float.Parse(form_MapData["width"].ToJson());
            mapData.FrmH = float.Parse(form_MapData["height"].ToJson());
            mapData.DesignerTool = "H5";
            mapData.Update();

            //表单描述文件直接保存到数据库.
            mapData.FormJson = jsonStrOfH5Frm;

            //格式化表单串
            //FormatDiagram2Json(jd);

            //第1步：构造一个空的数据结构.
            DataSet dsCCForm =  GenerBlankCCFormDataSet();

            //第2步：解析json数据到 dsCCForm.
            dsCCForm = SaveFrm_FullJsonToDataSet(fk_mapdata,dsCCForm, jd);

            //第3步：执行存盘.
            CCFormAPI.SaveCCForm20(fk_mapdata, dsCCForm);
        }
        /// <summary>
        /// 将表单设计串格式化为Json
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private static DataSet SaveFrm_FullJsonToDataSet(string fk_mapdata, DataSet form_DS, LitJson.JsonData formData)
        {
            //装饰类元素.
            DataTable dtLine = form_DS.Tables[EEleTableNames.Sys_FrmLine];
            DataTable dtBtn = form_DS.Tables[EEleTableNames.Sys_FrmBtn];
            DataTable dtLabel = form_DS.Tables[EEleTableNames.Sys_FrmLab];
            DataTable dtLink = form_DS.Tables[EEleTableNames.Sys_FrmLink];
            DataTable dtImg = form_DS.Tables[EEleTableNames.Sys_FrmImg];

            //数据类.
            DataTable dtEle = form_DS.Tables[EEleTableNames.Sys_FrmEle];
            DataTable dtMapAttr = form_DS.Tables[EEleTableNames.Sys_MapAttr];
            DataTable dtRDB = form_DS.Tables[EEleTableNames.Sys_FrmRB];
            DataTable dtM2M = form_DS.Tables[EEleTableNames.Sys_MapM2M];

            //附件类.
            DataTable dtAth = form_DS.Tables[EEleTableNames.Sys_FrmAttachment];
            DataTable dtImgAth = form_DS.Tables[EEleTableNames.Sys_FrmImgAth];

            //组件类.
            DataTable dtlDT = form_DS.Tables[EEleTableNames.Sys_MapDtl];
            DataTable dtWorkCheck = form_DS.Tables[EEleTableNames.WF_Node];

            JsonData form_Lines = formData["m"]["connectors"];
            if (form_Lines.IsArray == true && form_Lines.Count > 0)
            {
                for (int idx = 0, jLine = form_Lines.Count; idx < jLine; idx++)
                {
                    #region 线集合
                    JsonData line = form_Lines[idx];
                    if (line.IsObject == false)
                        continue;

                    DataRow drline = dtLine.NewRow();

                    drline["MYPK"] = line["CCForm_MyPK"].ToString();
                    drline["FK_MAPDATA"] = fk_mapdata;

                    JsonData turningPoints = line["turningPoints"];
                    drline["X1"] = turningPoints[0]["x"].ToString();
                    drline["X2"] = turningPoints[1]["x"].ToString();
                    drline["Y1"] = turningPoints[0]["y"].ToString();
                    drline["Y2"] = turningPoints[1]["y"].ToString();

                    JsonData properties = line["properties"];
                    JsonData borderWidth = properties.GetObjectFromArrary_ByKeyValue("type", "LineWidth");
                    JsonData borderColor = properties.GetObjectFromArrary_ByKeyValue("type", "Color");
                    string strborderWidth = "2";
                    if (borderWidth != null && borderWidth["PropertyValue"] != null && !string.IsNullOrEmpty(borderWidth["PropertyValue"].ToString()))
                    {
                        strborderWidth = borderWidth["PropertyValue"].ToString();
                    }
                    string strBorderColor = "Black";
                    if (borderColor != null && borderColor["PropertyValue"] != null && !string.IsNullOrEmpty(borderColor["PropertyValue"].ToString()))
                    {
                        strBorderColor = borderColor["PropertyValue"].ToString();
                    }
                    drline["W"] = strborderWidth;
                    drline["Color"] = strBorderColor;

                    dtLine.Rows.Add(drline); //增加到里面.
                }
            }

            //其他控件，Label,Img,TextBox
            JsonData form_Controls = formData["s"]["figures"];
            if (form_Controls.IsArray == false)
                return form_DS;
            if (form_Controls.Count == 0)
                return form_DS;

            for (int idx = 0, jControl = form_Controls.Count; idx < jControl; idx++)
            {
                JsonData control = form_Controls[idx];  //不存在控件类型不进行处理，继续循环.
                if (control == null || control["CCForm_Shape"] == null)
                    continue;

                string shape = control["CCForm_Shape"].ToString();
                string ctrlID = control["CCForm_MyPK"].ToString();
                JsonData properties = control["properties"];  //属性集合

                #region 装饰类控件.
                if (shape == "Label")
                {
                    #region 标签
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

                        string type=property["type"].ToString().Trim();

                        string val=null;
                        if (property["PropertyValue"]!=null)
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
                    #endregion end 标签
                }

                if (shape == "Button")
                {
                    #region 按钮 Button
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

                        string propertyBtn=property["property"].ToString();
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
                    #endregion
                }

                if (shape == "HyperLink")
                {
                    #region 超链接 HyperLink
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
                    #endregion
                }

                if (shape == "Image")
                {
                    #region 装饰图片  Image
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
                    #endregion Image
                }
                #endregion 装饰类控件.


                #region 数据类控件.
                if (shape.Contains("TextBox"))
                {
                    DataRow drAttr = dtMapAttr.NewRow();

                    drAttr["KeyOfEn"] = ctrlID;
                    drAttr["MYPK"] = fk_mapdata + "_" + ctrlID;
                    drAttr["FK_MAPDATA"] = fk_mapdata;

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
                            case "DefVal":
                            case "UIIsEnable":
                            case "MinLen":
                            case "MaxLen":
                            case "UIWidth":
                            case "UIHeight":
                            case "":
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

                        drAttr["MinLen"] = "0";
                        drAttr["MaxLen"] = "50";

                        //控件类型.
                        switch (shape)
                        {
                            case "TextBoxStr":
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppString;
                                drAttr["LGTYPE"] ="0" ;
                                drAttr["UICONTRALTYPE"] = "0";
                                break;
                            case "TextBoxInt":
                            case "TextBoxBoolean":
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppInt;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                break;
                            case "TextBoxFloat":
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppFloat;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                break;
                            case "TextBoxDouble":
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppDouble;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                break;
                            case "TextBoxMoney": //金额类型.
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppMoney;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                break;
                            case "TextBoxData": //金额类型.
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppDate;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                drAttr["MaxLen"] = "16";
                                break;
                            case "TextBoxDataTime": //金额类型.
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppDateTime;
                                drAttr["LGTYPE"] = "0";
                                drAttr["UICONTRALTYPE"] = "0";
                                drAttr["MaxLen"] = "16";
                                break;
                            case "DropDownListEnum": //枚举类型.
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppInt;
                                drAttr["LGTYPE"] = "1";
                                drAttr["UICONTRALTYPE"] = "1";
                                break;
                            case "DropDownListTable": //外键类型.
                                drAttr["MYDATATYPE"] = BP.DA.DataType.AppString;
                                drAttr["LGTYPE"] = "2";
                                drAttr["UICONTRALTYPE"] = "1";
                                break;
                            default:
                                break;
                        }
                    }

                    //增加到集合.
                    dtMapAttr.Rows.Add(drAttr);
                }
                #endregion 数据类控件

            }

            //TempSaveFrm(form_DS);
            return form_DS;
        }
        /// <summary>
        /// 生成一个空白的结构 DataSet.
        /// </summary>
        /// <returns></returns>
        public static DataSet GenerBlankCCFormDataSet()
        {
            DataTable dtMapData = new DataTable();
            dtMapData.TableName = EEleTableNames.Sys_MapData;
            dtMapData.Columns.Add(new DataColumn("NO", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("NAME", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("FrmW", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("FrmH", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxLeft", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxRight", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxTop", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxEnd", typeof(double)));

            #region line

            DataTable dtLine = new DataTable();
            dtLine.TableName = EEleTableNames.Sys_FrmLine;
            dtLine.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLine.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            dtLine.Columns.Add(new DataColumn("X", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y", typeof(double)));

            dtLine.Columns.Add(new DataColumn("X1", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y1", typeof(double)));

            dtLine.Columns.Add(new DataColumn("X2", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y2", typeof(double)));

            dtLine.Columns.Add(new DataColumn("W", typeof(string)));
            dtLine.Columns.Add(new DataColumn("Color", typeof(string)));
            // lineDT.Columns.Add(new DataColumn("BorderStyle", typeof(string)));
            #endregion line

            #region btn
            DataTable dtBtn = new DataTable();
            dtBtn.TableName = EEleTableNames.Sys_FrmBtn;
            dtBtn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("X", typeof(double)));
            dtBtn.Columns.Add(new DataColumn("Y", typeof(double)));
            dtBtn.Columns.Add(new DataColumn("ISVIEW", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("ISENABLE", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("EVENTTYPE", typeof(int)));
            dtBtn.Columns.Add(new DataColumn("EVENTCONTEXT", typeof(string)));
            #endregion line

            #region label .
            DataTable dtLabel = new DataTable();
            dtLabel.TableName = EEleTableNames.Sys_FrmLab;
            dtLabel.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("X", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("TEXT", typeof(string)));

            dtLabel.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion label

            #region Link
            DataTable dtLikn = new DataTable();
            dtLikn.TableName = EEleTableNames.Sys_FrmLink;
            dtLikn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("X", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("TARGET", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("URL", typeof(string)));

            dtLikn.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));

            dtLikn.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLikn.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion Link

            #region img  ImgSeal
            DataTable dtImg = new DataTable();
            dtImg.TableName = EEleTableNames.Sys_FrmImg;
            dtImg.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtImg.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtImg.Columns.Add(new DataColumn("X", typeof(double)));
            dtImg.Columns.Add(new DataColumn("Y", typeof(double)));
            dtImg.Columns.Add(new DataColumn("W", typeof(double)));
            dtImg.Columns.Add(new DataColumn("H", typeof(double)));

            dtImg.Columns.Add(new DataColumn("IMGURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("IMGPATH", typeof(string))); //应用类型 0=图片，1签章..

            dtImg.Columns.Add(new DataColumn("LINKURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("LINKTARGET", typeof(string)));
            dtImg.Columns.Add(new DataColumn("SRCTYPE", typeof(int))); //图片来源类型.
            dtImg.Columns.Add(new DataColumn("IMGAPPTYPE", typeof(int))); //应用类型 0=图片，1签章..
            dtImg.Columns.Add(new DataColumn("Tag0", typeof(string)));
            dtImg.Columns.Add(new DataColumn("ISEDIT", typeof(int)));
            dtImg.Columns.Add(new DataColumn("NAME", typeof(string)));//中文名
            dtImg.Columns.Add(new DataColumn("ENPK", typeof(string)));//英文名
            #endregion img

            #region 公共元素存储表
            DataTable dtEle = new DataTable();
            dtEle.TableName = EEleTableNames.Sys_FrmEle;
            dtEle.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtEle.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            //eleDT.Columns.Add(new DataColumn("EleType", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleID", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleName", typeof(string)));

            dtEle.Columns.Add(new DataColumn("X", typeof(double)));
            dtEle.Columns.Add(new DataColumn("Y", typeof(double)));
            dtEle.Columns.Add(new DataColumn("W", typeof(double)));
            dtEle.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion eleDT

            #region Sys_FrmImgAth
            DataTable imgAthDT = new DataTable();
            imgAthDT.TableName = EEleTableNames.Sys_FrmImgAth;
            imgAthDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("CTRLID", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("ISEDIT", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("X", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("Y", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("W", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion Sys_FrmImgAth

            #region mapAttrDT
            DataTable mapAttrDT = new DataTable();
            mapAttrDT.TableName = EEleTableNames.Sys_MapAttr;
            mapAttrDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("DEFVAL", typeof(string))); //默认值.
            mapAttrDT.Columns.Add(new DataColumn("UIIsEnable", typeof(string))); //默认值.
            mapAttrDT.Columns.Add(new DataColumn("MAXLEN", typeof(int))); //最大长度.
            mapAttrDT.Columns.Add(new DataColumn("MINLEN", typeof(int))); //最小长度.
            mapAttrDT.Columns.Add(new DataColumn("UIIsInput", typeof(int))); //是否必填项目?

            mapAttrDT.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UICONTRALTYPE", typeof(int)));
            mapAttrDT.Columns.Add(new DataColumn("MYDATATYPE", typeof(int)));
            mapAttrDT.Columns.Add(new DataColumn("LGTYPE", typeof(int)));

            mapAttrDT.Columns.Add(new DataColumn("UIWIDTH", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("UIHEIGHT", typeof(double)));

            mapAttrDT.Columns.Add(new DataColumn("UIBINDKEY", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKey", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKeyText", typeof(string)));
            //   mapAttrDT.Columns.Add(new DataColumn("UIVISIBLE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("X", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion mapAttrDT

            #region frmRBDT
            DataTable dtRdb = new DataTable();
            dtRdb.TableName = EEleTableNames.Sys_FrmRB;
            dtRdb.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("ENUMKEY", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("INTKEY", typeof(int)));
            dtRdb.Columns.Add(new DataColumn("LAB", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("X", typeof(double)));
            dtRdb.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion frmRBDT

            #region Dtl
            DataTable dtlDT = new DataTable();

            dtlDT.TableName = EEleTableNames.Sys_MapDtl;
            dtlDT.Columns.Add(new DataColumn("NO", typeof(string)));
            dtlDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtlDT.Columns.Add(new DataColumn("Name", typeof(string)));

            dtlDT.Columns.Add(new DataColumn("X", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("Y", typeof(double)));

            dtlDT.Columns.Add(new DataColumn("H", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("W", typeof(double)));
            #endregion Dtl

            // BPWorkCheck
            DataTable dtWorkCheck = new DataTable();
            dtWorkCheck.TableName = EEleTableNames.WF_Node;
            dtWorkCheck.Columns.Add(new DataColumn("NodeID", typeof(string)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_W", typeof(double)));

            //子流程属性.
            dtWorkCheck.Columns.Add(new DataColumn("SF_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_W", typeof(double)));


            #region m2mDT
            DataTable m2mDT = new DataTable();
            m2mDT.TableName = EEleTableNames.Sys_MapM2M;
            m2mDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            m2mDT.Columns.Add(new DataColumn("X", typeof(double)));
            m2mDT.Columns.Add(new DataColumn("Y", typeof(double)));

            m2mDT.Columns.Add(new DataColumn("H", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("W", typeof(string)));
            #endregion m2mDT

            #region athDT
            DataTable athDT = new DataTable();
            athDT.TableName = EEleTableNames.Sys_FrmAttachment;
            athDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            athDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            athDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));

            //athDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            //athDT.Columns.Add(new DataColumn("EXTS", typeof(string)));
            //athDT.Columns.Add(new DataColumn("SAVETO", typeof(string)));
            athDT.Columns.Add(new DataColumn("UPLOADTYPE", typeof(int)));

            athDT.Columns.Add(new DataColumn("X", typeof(double)));
            athDT.Columns.Add(new DataColumn("Y", typeof(double)));
            athDT.Columns.Add(new DataColumn("W", typeof(double)));
            athDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion athDT

            DataSet dsLatest = new DataSet();

            dsLatest.Tables.Add(dtWorkCheck);
            dsLatest.Tables.Add(dtLabel);
            dsLatest.Tables.Add(dtLikn);
            dsLatest.Tables.Add(dtImg);
            dsLatest.Tables.Add(dtEle);
            dsLatest.Tables.Add(dtBtn);
            dsLatest.Tables.Add(imgAthDT);
            dsLatest.Tables.Add(mapAttrDT);
            dsLatest.Tables.Add(dtRdb);
            dsLatest.Tables.Add(dtLine);
            dsLatest.Tables.Add(dtlDT);
            dsLatest.Tables.Add(athDT);
            dsLatest.Tables.Add(dtMapData);
            dsLatest.Tables.Add(m2mDT);

            return dsLatest;

        }

        /// <summary>
        /// 保存表单信息
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ds">控件集合</param>
        public static void SaveCCForm20(string fk_mapdata, DataSet ds)
        {
            //元素集合.
            FrmEles eles = new FrmEles();
            eles.Retrieve(FrmEleAttr.FK_MapData, fk_mapdata);

            #region 生成组合PK, 为了处理删除的元素使用.
            string lines = "";
            FrmLines linEns = new FrmLines();
            linEns.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            foreach (FrmLine lin in linEns)
            {
                lines += lin.MyPK + ",";
            }

            string labs = "";
            FrmLabs labEns = new FrmLabs();
            labEns.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            foreach (FrmLab ele in labEns)
            {
                labs += ele.MyPK + ",";
            }


            string links = "";
            FrmLinks linkEns = new FrmLinks();
            linkEns.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            foreach (FrmLink ele in linkEns)
            {
                links += ele.MyPK + ",";
            }

            string btns = "";
            FrmBtns btnEns = new FrmBtns();
            btnEns.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            foreach (FrmBtn ele in btnEns)
            {
                btns += ele.MyPK + ",";
            }

            string imgs = "";
            FrmImgs imgEns = new FrmImgs();
            imgEns.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            foreach (FrmImg ele in imgEns)
            {
                imgs += ele.MyPK + ",";
            }
            #endregion 生成组合PK.

            //字段集合.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(FrmEleAttr.FK_MapData, fk_mapdata);

            //循环这个集合,开始执行保存.
            foreach (DataTable dt in ds.Tables)
            {
                switch (dt.TableName)
                {
                    case "Sys_FrmLine": //线.
                        SaveCCForm20_Line(fk_mapdata, dt, lines);
                        break;
                    case "Sys_FrmBtn": //按钮.
                        SaveCCForm20_Btn(fk_mapdata, dt, btns);
                        break;
                    case "Sys_FrmLab": //标签.
                        SaveCCForm20_Lab(fk_mapdata, dt, labs);
                        break;
                    case "Sys_FrmLink": //链接.
                        SaveCCForm20_Link(fk_mapdata, dt, links);
                        break;
                    case "Sys_FrmImg": //图片.
                        SaveCCForm20_Img(fk_mapdata, dt, imgs);
                        break;
                    case "Sys_FrmEle": //链接.
                        break;
                    case "Sys_FrmImgAth": //图片附件.
                        break;
                    case "Sys_FrmRB": //单选按钮.
                        break;
                    case "Sys_MapData": //MapData.
                        break;
                    case "Sys_MapAttr": //字段属性.
                        SaveCCForm20_MapAttr(fk_mapdata, dt);
                        break;
                    case "Sys_MapDtl": //明细表.
                        break;
                    case "Sys_MapM2M": //一对多.
                        break;
                    case "WF_Node": //节点.
                        break;
                    //default:
                    //    throw new Exception("@没有约定的表结构.");
                }
            }
        }
        /// <summary>
        /// 保存字段属性.
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="dt"></param>
        public static void SaveCCForm20_MapAttr(string fk_mapdata, DataTable dt)
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, fk_mapdata);

            //求出已经有的属性.
            string pks = "";
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                if (attr.IsPK == true)
                    continue;
                pks += attr.KeyOfEn + ",";
            }

            foreach (DataRow dr in dt.Rows)
            {
                string keyOfEn = dr[MapAttrAttr.KeyOfEn].ToString();
                string name = dr[MapAttrAttr.Name].ToString();

                MapAttr attr = (MapAttr)attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, keyOfEn);
                attr.Name = dr["Name"].ToString();
                attr.MyDataType = int.Parse(dr["MyDataType"].ToString());
                foreach (DataColumn dc in dt.Columns)
                {
                    attr.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                }
                try
                {
                    attr.Save(); //执行保存.
                }
                catch (Exception ex)
                {
                    throw new Exception("@保存"+attr.Name+" 失败："+ex.Message);
                }

                //去掉字段.
                pks = pks.Replace(attr.KeyOfEn+",", "");
            }


            //删除已经从界面上删除的.  没有去掉的都是需要删除的.
            string[] strs = pks.Split(',');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str) == true)
                    continue;
                DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE MyPK='"+fk_mapdata+"_"+str+"'");
            }

        }
        #region 保存 frmEle控件。
        /// <summary>
        /// 保存line
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Line(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmLine ele = new FrmLine();
            foreach (DataRow dr in dt.Rows)
            {
                ele.MyPK = dr["MyPK"].ToString();
                ele.FK_MapData = fk_mapdata;

                ele.X1 = float.Parse(dr["X1"].ToString());
                ele.Y1 = float.Parse(dr["Y1"].ToString());

                ele.X2 = float.Parse(dr["X2"].ToString());
                ele.Y2 = float.Parse(dr["Y2"].ToString());

                ele.BorderColor = dr["Color"].ToString();
                ele.BorderWidth = float.Parse(dr["W"].ToString());

                if (string.IsNullOrEmpty(ele.MyPK))
                {
                    ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.GUID = ele.MyPK;
                }

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLine WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存img
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Img(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmImg ele = new FrmImg();
            foreach (DataRow dr in dt.Rows)
            {
                ele.Copy(dr);
                ele.MyPK = dr["MyPK"].ToString();
                ele.FK_MapData = fk_mapdata;

                //位置.
                ele.X = float.Parse(dr["X"].ToString());
                ele.Y = float.Parse(dr["Y"].ToString());

                ele.W = float.Parse(dr["W"].ToString());
                ele.H = float.Parse(dr["H"].ToString());

                ele.ImgPath = dr["IMGURL"].ToString();
                ele.LinkURL = dr["LINKURL"].ToString();
                ele.LinkTarget = dr["LINKTARGET"].ToString();

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                {
                    ele.DirectInsert();
                }
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmImg WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Lab
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Lab(string fk_mapdata, DataTable dt, string elePKs)
        {
            string pks = "";
            FrmLab ele = new FrmLab();
            foreach (DataRow dr in dt.Rows)
            {
                ele.MyPK = dr["MyPK"].ToString();
                ele.FK_MapData = fk_mapdata;
                ele.X = float.Parse(dr["X"].ToString());
                ele.Y = float.Parse(dr["Y"].ToString());
                ele.Text = dr["Text"].ToString();

                ele.FontStyle = dr["FontStyle"].ToString();
                ele.FontColor = dr["FONTCOLOR"].ToString();
                ele.FontName = dr["FontName"].ToString();
                ele.FontSize = int.Parse(dr["FontSize"].ToString());

                if (dr["IsBold"].ToString() == "1")
                    ele.IsBold = true;
                else
                    ele.IsBold = false;

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                {
                    ele.DirectInsert();
                }
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLab WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Btn
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Btn(string fk_mapdata, DataTable dt, string elePKs)
        {
            FrmBtn ele = new FrmBtn();
            foreach (DataRow dr in dt.Rows)
            {
                ele.MyPK = dr["MyPK"].ToString();
                ele.FK_MapData = fk_mapdata;

                ele.Text = dr["TEXT"].ToString();

                ele.EventType = int.Parse(dr["EVENTTYPE"].ToString());
                ele.EventContext = dr["EVENTCONTEXT"].ToString(); //执行内容.

                ele.X = int.Parse(dr["X"].ToString());
                ele.Y = int.Parse(dr["Y"].ToString());

                if (dr["IsEnable"].ToString() == "1")
                    ele.IsEnable = true;
                else
                    ele.IsEnable = false;

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                {
                    ele.DirectInsert();
                }
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmBtn WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Link
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Link(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmLink ele = new FrmLink();
            foreach (DataRow dr in dt.Rows)
            {
                ele.MyPK = dr["MyPK"].ToString();
                ele.FK_MapData = fk_mapdata;

                ele.X = float.Parse(dr["X"].ToString());
                ele.Y = float.Parse(dr["Y"].ToString());

                ele.Text = dr["Text"].ToString();

                if (ele.Text == "" || ele.Text == null)
                    ele.Text = "无连接标题..";

                ele.FontName = dr["FontName"].ToString();
                ele.FontColor = dr["FontColor"].ToString();

                ele.URL = dr["URL"].ToString();
                ele.Target = dr["Target"].ToString();

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                {
                    ele.DirectInsert();
                }
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLink WHERE MyPK='" + str + "'";
            }
            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        #endregion 保存 frmEle控件。

        #endregion 执行 ccform2.0 的保存.

        /// <summary>
        /// 获得表单模版dataSet格式.
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <returns>DataSet</returns>
        public static System.Data.DataSet GenerHisDataSet(string fk_mapdata)
        {
            // 20150513 小周鹏修改，原因：手机端无法显示 dtl Start
            // string sql = "SELECT FK_MapData,No,X,Y,W,H  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            string sql = "SELECT *  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            // 20150513 小周鹏修改 End

            sql = string.Format(sql, fk_mapdata);
            DataTable dtMapDtl = DBAccess.RunSQLReturnTable(sql);
            dtMapDtl.TableName = "Sys_MapDtl";

            string ids = string.Format("'{0}'", fk_mapdata);
            foreach (DataRow dr in dtMapDtl.Rows)
            {
                ids += ",'" + dr["No"] + "'";
            }
            string sqls = string.Empty;
            List<string> listNames = new List<string>();
            // Sys_GroupField.
            listNames.Add("Sys_GroupField");
            sql = "SELECT * FROM Sys_GroupField WHERE  EnName IN (" + ids + ")";
            sqls += sql;

            // Sys_Enum
            listNames.Add("Sys_Enum");
            sql = "@SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + ids + ") )";
            sqls += sql;

            // 审核组件
            string nodeIDstr = fk_mapdata.Replace("ND", "");
            if (DataType.IsNumStr(nodeIDstr))
            {
                // 审核组件状态:0 禁用;1 启用;2 只读;
                listNames.Add("WF_Node");
                sql = "@SELECT NodeID,FWC_X,FWC_Y,FWC_W,FWC_H,FWCSTA,FWCTYPE,SFSTA,SF_X,SF_Y,SF_H,SF_W FROM WF_Node WHERE NodeID=" + nodeIDstr + " AND  ( FWCSta >0  OR SFSta >0 )";
                sqls += sql;
            }

            string where = " FK_MapData IN (" + ids + ")";

            // Sys_MapData.
            listNames.Add("Sys_MapData");
            sql = "@SELECT No,Name,FrmW,FrmH FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
            sqls += sql;

            // Sys_MapAttr.
            listNames.Add("Sys_MapAttr");

            sql = "@SELECT * FROM Sys_MapAttr WHERE " + where + " AND KeyOfEn NOT IN('WFState') ORDER BY FK_MapData, IDX  ";
            sqls += sql;

            //// Sys_MapM2M.
            //listNames.Add("Sys_MapM2M");
            //sql = "@SELECT MyPK,FK_MapData,NoOfObj,M2MTYPE,X,Y,W,H FROM Sys_MapM2M WHERE " + where;
            //sqls += sql;

            // Sys_MapExt.
            listNames.Add("Sys_MapExt");
            sql = "@SELECT * FROM Sys_MapExt WHERE " + where;
            sqls += sql;

            // line.
            listNames.Add("Sys_FrmLine");
            sql = "@SELECT MyPK,FK_MapData,X,X1,X2,Y,Y1,Y2,BorderColor,BorderWidth from Sys_FrmLine WHERE " + where;
            sqls += sql;

            // ele.
            listNames.Add("Sys_FrmEle");
            sql = "@SELECT FK_MapData,MyPK,EleType,EleID,EleName,X,Y,W,H FROM Sys_FrmEle WHERE " + where;
            sqls += sql;

            // link.
            listNames.Add("Sys_FrmLink");
            sql = "@SELECT FK_MapData,MyPK,Text,URL,Target,FontSize,FontColor,X,Y from Sys_FrmLink WHERE " + where;
            sqls += sql;

            // btn.
            listNames.Add("Sys_FrmBtn");
            sql = "@SELECT FK_MapData,MyPK,Text,BtnType,EventType,EventContext,MsgErr,MsgOK,X,Y FROM Sys_FrmBtn WHERE " + where;
            sqls += sql;

            // Sys_FrmImg.
            listNames.Add("Sys_FrmImg");
            sql = "@SELECT * FROM Sys_FrmImg WHERE " + where;
            sqls += sql;

            // Sys_FrmLab.
            listNames.Add("Sys_FrmLab");
            sql = "@SELECT MyPK,FK_MapData,Text,X,Y,FontColor,FontName,FontSize,FontStyle,FontWeight,IsBold,IsItalic from Sys_FrmLab WHERE " + where;
            sqls += sql;

            // Sys_FrmRB.
            listNames.Add("Sys_FrmRB");
            sql = "@SELECT * FROM Sys_FrmRB WHERE " + where;
            sqls += sql;

            // Sys_FrmAttachment. 
            listNames.Add("Sys_FrmAttachment");
            /* 20150730 小周鹏修改 添加AtPara 参数 START */
            //sql = "@SELECT  MyPK,FK_MapData,UploadType,X,Y,W,H,NoOfObj,Name,Exts,SaveTo,IsUpload,IsDelete,IsDownload "
            // + " FROM Sys_FrmAttachment WHERE " + where + " AND FK_Node=0";
            sql = "@SELECT MyPK,FK_MapData,UploadType,X,Y,W,H,NoOfObj,Name,Exts,SaveTo,IsUpload,IsDelete,IsDownload ,AtPara"
                + " FROM Sys_FrmAttachment WHERE " + where + " AND FK_Node=0";
            /* 20150730 小周鹏修改 添加AtPara 参数 END */
            sqls += sql;

            // Sys_FrmImgAth.
            listNames.Add("Sys_FrmImgAth");
            sql = "@SELECT * FROM Sys_FrmImgAth WHERE " + where;
            sqls += sql;

            //// sqls.Replace(";", ";" + Environment.NewLine);
            // DataSet ds = DA.DBAccess.RunSQLReturnDataSet(sqls);
            // if (ds != null && ds.Tables.Count == listNames.Count)
            //     for (int i = 0; i < listNames.Count; i++)
            //     {
            //         ds.Tables[i].TableName = listNames[i];
            //     }

            string[] strs = sqls.Split('@');
            DataSet ds = new DataSet();

            if (strs != null && strs.Length == listNames.Count)
                for (int i = 0; i < listNames.Count; i++)
                {
                    string s = strs[i];
                    if (string.IsNullOrEmpty(s))
                        continue;
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(s);
                    dt.TableName = listNames[i];
                    ds.Tables.Add(dt);
                }

            foreach (DataTable item in ds.Tables)
            {
                if (item.TableName == "Sys_MapAttr" && item.Rows.Count == 0)
                {
                    BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = fk_mapdata;
                    attr.KeyOfEn = "OID";
                    attr.Name = "OID";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }

            ds.Tables.Add(dtMapDtl);
            return ds;
        }

        #endregion 模版操作.

        #region 其他功能.
        /// <summary>
        /// 转拼音方法
        /// </summary>
        /// <param name="name">字段中文名称</param>
        /// <param name="isQuanPin">是否转换全拼</param>
        /// <returns>转化后的拼音，不成功则抛出异常.</returns>
        public static string ParseStringToPinyinField(string name, bool isQuanPin)
        {
            string s = string.Empty; ;
            try
            {
                if (isQuanPin == true)
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
                //去掉空格，去掉点.
                s = s.Replace(" ", "");
                s = s.Replace(".", "");
                return s;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 获得外键表
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>json</returns>
        public static string GetSFTableList(int pageNumber, int pageSize)
        {
            //获得查询.
            SFTables sftables = new SFTables();
            QueryObject obj = new QueryObject(sftables);
            int RowCount = obj.GetCount();

            //查询
            obj.DoQuery(SysEnumMainAttr.No, pageSize, pageNumber);

            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(sftables, RowCount);
        }
        /// <summary>
        /// 获取所有枚举
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>json</returns>
        public static string GetEnumerationList(int pageNumber, int pageSize)
        {
            SysEnumMains enumMains = new SysEnumMains();
            QueryObject obj = new QueryObject(enumMains);
            int RowCount = obj.GetCount();
            //查询
            obj.DoQuery(SysEnumMainAttr.No, pageSize, pageNumber);

            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(enumMains, RowCount);
        }
        /// <summary>
        /// 获得隐藏字段集合.
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <returns></returns>
        public static string GetHiddenfielddata(string fk_mapdata)
        {
            int RowCount = 0;
            MapAttrs mapAttrs = new MapAttrs();
            QueryObject obj = new QueryObject(mapAttrs);
            obj.AddWhere(MapAttrAttr.FK_MapData, fk_mapdata);
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.UIVisible, "0");
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.EditType, "0");
            RowCount = obj.GetCount();
            //查询
            obj.DoQuery();

            return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(mapAttrs);
        }
        #endregion 其他功能.

    }
}
