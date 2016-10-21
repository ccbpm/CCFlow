using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.En;
using BP.DA;
using LitJson;

namespace BP.Sys
{
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
        public static void CreatePublicNoNameCtrl(string fk_mapdata, string ctrlType, string no, string name, float x, float y)
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
                case "Fieldset": //分组.
                    FrmEle fe = new FrmEle();
                    fe.MyPK = fk_mapdata + "_" + no;
                    if (fe.RetrieveFromDBSources() != 0)
                        throw new Exception("@创建失败，已经有同名元素["+no+"]的控件.");
                    fe.FK_MapData = fk_mapdata;
                    fe.EleType = "Fieldset";
                    fe.EleName = name;
                    fe.EleID = no;
                    fe.X = x;
                    fe.Y = y;
                    fe.Insert();
                    //CreateOrSaveAthImg(fk_mapdata, no, name, x, y);
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
            FrmImgAth ath = new FrmImgAth();
            ath.FK_MapData = fk_mapdata;
            ath.CtrlID = no;
            ath.MyPK = fk_mapdata + "_" + no;

            ath.X = x;
            ath.Y = y;
            ath.Insert();

            //ath.MyPK = ath.FK_MapData + "_" + ath.NoOfObj;
            //ath.RetrieveFromDBSources();
            //ath.UploadType = AttachmentUploadType.Single;
            //ath.Name = name;
            //ath.X = x;
            //ath.Y = y;
            //ath.Save();
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

            //初始化他的map.
            dtl.IntMapAttrs();
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
        /// <summary>
        /// 创建字段
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="field"></param>
        /// <param name="fieldDesc"></param>
        /// <param name="mydataType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colSpan"></param>
        public static void NewField(string frmID, string field, string fieldDesc,int mydataType, float x, float y, int colSpan = 1)
        {
            MapAttr ma = new MapAttr();
            ma.FK_MapData = frmID;
            ma.KeyOfEn = field;
            ma.Name = fieldDesc;
            ma.MyDataType = mydataType;
            ma.X = x;
            ma.Y =y;
            ma.Insert();

        }
        public static void NewEnumField(string fk_mapdata, string field, string fieldDesc, string enumKey, UIContralType ctrlType,
           float x, float y, int colSpan = 1)
        {

            MapAttr ma = new MapAttr();
            ma.FK_MapData = fk_mapdata; 
            ma.KeyOfEn =field;
            ma.Name = fieldDesc; 
            ma.MyDataType = DataType.AppInt;
            ma.X =x;
            ma.Y = y;
            ma.UIIsEnable = true;
            ma.LGType = FieldTypeS.Enum;
                ma.UIContralType = ctrlType ;
                ma.UIBindKey = enumKey;
            ma.Insert();

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
                    rb.EnumKey = ma.UIBindKey;
                    rb.Lab = item.Lab;
                    rb.X = ma.X;

                    //让其变化y值.
                    rb.Y = ma.Y + idx * 30;
                    rb.Insert();
                }
            }
        }
        /// <summary>
        /// 创建字段分组
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="gKey"></param>
        /// <param name="gName"></param>
        /// <returns></returns>
        public static string NewCheckGroup(string frmID, string gKey, string gName)
        {
            //string gKey = v1;
            //string gName = v2;
            //string enName1 = v3;

            MapAttr attrN = new MapAttr();
            int i = attrN.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, gKey + "_Note");
            i += attrN.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, gKey + "_Checker");
            i += attrN.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, gKey + "_RDT");
            if (i > 0)
                return "error:前缀已经使用：" + gKey + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";

            GroupField gf = new GroupField();
            gf.Lab = gName;
            gf.EnName = frmID;
            gf.Insert();

            attrN = new MapAttr();
            attrN.FK_MapData = frmID;
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
            attrN.FK_MapData = frmID;
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
            attrN.FK_MapData = frmID;
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
            frmID = frmID.Replace("ND", "");
            int nodeid = 0;
            try
            {
                nodeid = int.Parse(frmID);
            }
            catch
            {
                //转化不成功就是不是节点表单字段.
                return "error:只能节点表单才可以使用审核分组组件。";
            }
            return null;

            /*
            Node nd = new Node();
            nd.NodeID = nodeid;
            if (nd.RetrieveFromDBSources() != 0 && string.IsNullOrEmpty(nd.FocusField) == true)
            {
                nd.FocusField = "@" + gKey + "_Note";
                nd.Update();
            }
             * */
        }
        #endregion 创建修改字段.

        #region 模版操作.
        /// <summary>
        /// 创建表单
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="frmName">表单名称</param>
        /// <param name="frmTreeID">表单类别编号（表单树ID）</param>
        /// <param name="frmType">表单类型</param>
        public static void CreateFrm(string frmID, string frmName, string frmTreeID, FrmType frmType= FrmType.FreeFrm)
        {
            MapData md = new MapData();
            md.No = frmID;
            if (md.IsExits == true)
                throw new Exception("@表单ID为:" + frmID + " 已经存在.");

            md.Name = frmName;
            md.HisFrmType = frmType;
            md.Insert();
        }
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

            //执行保存.
            SaveFrm(fk_mapdata, jd);
        }
        /// <summary>
        /// 将表单设计串格式化为Json
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private static void SaveFrm(string fk_mapdata, LitJson.JsonData formData)
        {
            #region 求 PKs.
            //标签.
            string labelPKs = "@";
            FrmLabs labs = new FrmLabs();
            labs.Retrieve(FrmLabAttr.FK_MapData, fk_mapdata);
            foreach (FrmLab item in labs)
                labelPKs +=  item.MyPK+"@";

            //超链接.
            string linkPKs = "@";
            FrmLinks links = new FrmLinks();
            links.Retrieve(FrmLabAttr.FK_MapData, fk_mapdata);
            foreach (FrmLink item in links)
                linkPKs += item.MyPK + "@";

            //按钮.
            string btnsPKs = "@";
            FrmBtns btns = new FrmBtns();
            btns.Retrieve(FrmLabAttr.FK_MapData, fk_mapdata);
            foreach (FrmBtn item in btns)
                btnsPKs += item.MyPK + "@";

            //图片.
            string imgPKs = "@";
            FrmImgs imgs = new FrmImgs();
            imgs.Retrieve(FrmLabAttr.FK_MapData, fk_mapdata);
            foreach (FrmImg item in imgs)
                imgPKs += item.MyPK + "@";

            //求已经存在的字段.
            string attrPKs = "@";
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (MapAttr item in attrs)
                attrPKs += item.KeyOfEn + "@";
            attrPKs += "@";


            //求明细表.
            string dtlPKs = "@";
            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (MapDtl item in dtls)
                dtlPKs += item.No + "@";
            dtlPKs += "@";

            //求附件.
            string athMultis = "@";
            FrmAttachments aths = new FrmAttachments();
            aths.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (FrmAttachment item in aths)
                athMultis += item.NoOfObj + "@";
            athMultis += "@";

            //图片附件.
            string athImgs = "@";
            FrmImgAths fias = new FrmImgAths();;
            fias.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (FrmImgAth item in fias)
                athImgs += item.CtrlID + "@";
            athImgs += "@";


            //附加元素..
            string eleIDs = "@";
            FrmEles feles = new FrmEles(); ;
            feles.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (FrmEle item in feles)
                eleIDs += item.EleID + "@";
            eleIDs += "@";
            #endregion 求PKs.

            // 保存线.
            JsonData form_Lines = formData["m"]["connectors"];
            BP.Sys.CCFormParse.SaveLine(fk_mapdata, form_Lines);
                
            //其他控件，Label,Img,TextBox
            JsonData form_Controls = formData["s"]["figures"];
            if (form_Controls.IsArray == false || form_Controls.Count == 0)
            {
                /*画布里没有任何东西, 清楚所有的元素.*/
                string delSqls = "";
                delSqls += "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapdata + "' AND KeyOfEn NOT IN ('OID')";
                delSqls += "DELETE FROM Sys_MapDtl WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmBtn WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmLine WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmLab WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmLink WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmImg WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmEle WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "DELETE FROM Sys_FrmImgAth WHERE FK_MapData='" + fk_mapdata + "'";

                BP.DA.DBAccess.RunSQLs(delSqls);
                return;
            }

            //循环元素.
            for (int idx = 0, jControl = form_Controls.Count; idx < jControl; idx++)
            {
                JsonData control = form_Controls[idx];  //不存在控件类型不进行处理，继续循环.
                if (control == null || control["CCForm_Shape"] == null)
                    continue;

                string shape = control["CCForm_Shape"].ToString();
                if (control["CCForm_MyPK"] == null)
                    continue;

                string ctrlID = control["CCForm_MyPK"].ToString();

                JsonData properties = control["properties"];  //属性集合.

                #region 装饰类控件.
                switch (shape)
                {
                    case "Label": //保存标签.
                        BP.Sys.CCFormParse.SaveLabel(fk_mapdata, control, properties, labelPKs, ctrlID);
                        labelPKs = labelPKs.Replace(ctrlID + "@", "@");
                        continue;
                    case "Button": //保存Button.
                        BP.Sys.CCFormParse.SaveButton(fk_mapdata, control, properties, btnsPKs, ctrlID);
                        btnsPKs = btnsPKs.Replace(ctrlID + "@", "@");
                        continue;
                    case "HyperLink": //保存link.
                        BP.Sys.CCFormParse.SaveHyperLink(fk_mapdata, control, properties, linkPKs, ctrlID);
                        linkPKs = linkPKs.Replace(ctrlID + "@", "@");
                        continue;
                    case "Image": //保存Img.
                        BP.Sys.CCFormParse.SaveImage(fk_mapdata, control, properties, imgPKs, ctrlID);
                        imgPKs = imgPKs.Replace(ctrlID + "@", "@");
                        continue;
                  
                    default:
                        break;
                }
                #endregion 装饰类控件.

                #region 数据类控件.
                if (shape.Contains("TextBox")==true
                    || shape.Contains("DropDownList")==true)
                {
                    BP.Sys.CCFormParse.SaveMapAttr(fk_mapdata,ctrlID,shape,control, properties,attrPKs);
                    attrPKs = attrPKs.Replace(ctrlID + "@", "@");
                    continue;
                }

                //求出公共的属性-坐标.
                JsonData style = control["style"];
                JsonData vector = style["gradientBounds"];
                float x = float.Parse(vector[0].ToJson());
                float y = float.Parse(vector[1].ToJson());
                float maxX = float.Parse(vector[2].ToJson());
                float maxY = float.Parse(vector[3].ToJson());
                float width = maxX - x;
                float height = maxY - y;

                if (shape =="Dtl")
                {
                    //记录已经存在的ID， 需要当时保存.
                    BP.Sys.CCFormParse.SaveDtl(fk_mapdata, ctrlID, x, y, height, width);
                    dtlPKs = dtlPKs.Replace(ctrlID + "@", "@");
                    continue;
                }
                #endregion 数据类控件.

                #region 附件.
                if (shape == "AthMulti" || shape == "AthSingle")
                {
                    //记录已经存在的ID， 需要当时保存.
                    BP.Sys.CCFormParse.SaveAthMulti(fk_mapdata, ctrlID, x, y, height, width);
                    athMultis = athMultis.Replace(ctrlID + "@", "@");
                    continue;
                }
                if (shape == "AthImg")
                {
                    //记录已经存在的ID， 需要当时保存.
                    BP.Sys.CCFormParse.SaveAthImg(fk_mapdata, ctrlID, x, y, height, width);
                    athImgs = athImgs.Replace(ctrlID + "@", "@");
                    continue;
                }

                if (shape == "Fieldset")
                {
                    //记录已经存在的ID， 需要当时保存.
                    BP.Sys.CCFormParse.SaveFrmEle(fk_mapdata, shape, ctrlID, x, y, height, width);
                    eleIDs = eleIDs.Replace(ctrlID + "@", "@");
                    continue;
                }

                
            

                
                #endregion 附件.

                throw new Exception("@没有判断的类型:shape = " + shape);
            }

            #region 删除没有替换下来的 PKs, 说明这些都已经被删除了.
            string[] pks = labelPKs.Split('@');
            string sqls="";
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLab WHERE MyPK='" + pk + "'";
            }

            pks = btnsPKs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmBtn WHERE MyPK='" + pk + "'";
            }

            pks = linkPKs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmLink WHERE MyPK='" + pk + "'";
            }

            pks = imgPKs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmImg WHERE MyPK='" + pk + "'";
            }

            pks = attrPKs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_MapAttr WHERE KeyOfEn='" + pk + "' AND FK_MapData='"+fk_mapdata+"'";
                sqls += "@DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }

            pks = dtlPKs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_MapDtl WHERE No='" + pk + "'";
            }


            pks = athMultis.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmAttachment WHERE NoOfObj='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }

            //删除图片附件.
            pks = athImgs.Split('@');
            foreach (string pk in pks)
            {
                if (string.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmImgAth WHERE CtrlID='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }
            

            //删除这些，没有替换下来的数据.
            BP.DA.DBAccess.RunSQLs(sqls);
            #endregion 删除没有替换下来的 PKs, 说明这些都已经被删除了.

        }
        /// <summary>
        /// 获得表单模版dataSet格式.
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="isCheckFrmType">是否检查表单类型</param>
        /// <returns>DataSet</returns>
        public static System.Data.DataSet GenerHisDataSet(string fk_mapdata, bool isCheckFrmType = false)
        {
            MapData md = new MapData(fk_mapdata);

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
            //杨玉慧  加上TableWidth,TableHeight,TableCol 获取傻瓜表单的宽度
            //sql = "@SELECT No,Name,FrmW,FrmH FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
            sql = "@SELECT No,Name,FrmW,FrmH,TableWidth,TableHeight,TableCol FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
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

            //if (isCheckFrmType == true && md.HisFrmType == FrmType.FreeFrm)
            //{
                // line.
                listNames.Add("Sys_FrmLine");
                sql = "@SELECT MyPK,FK_MapData, X1,X2, Y1,Y2,BorderColor,BorderWidth from Sys_FrmLine WHERE " + where;
                sqls += sql;

                // link.
                listNames.Add("Sys_FrmLink");
                sql = "@SELECT FK_MapData,MyPK,Text,URL,Target,FontSize,FontColor,X,Y from Sys_FrmLink WHERE " + where;
                sqls += sql;

                // btn.
                listNames.Add("Sys_FrmBtn");
                sql = "@SELECT FK_MapData,MyPK,Text,EventType,EventContext,MsgErr,MsgOK,X,Y FROM Sys_FrmBtn WHERE " + where;
                sqls += sql;

                // Sys_FrmImg.
                listNames.Add("Sys_FrmImg");
                sql = "@SELECT * FROM Sys_FrmImg WHERE " + where;
                sqls += sql;

                // Sys_FrmLab.
                listNames.Add("Sys_FrmLab");
                sql = "@SELECT MyPK,FK_MapData,Text,X,Y,FontColor,FontName,FontSize,FontStyle,FontWeight,IsBold,IsItalic FROM Sys_FrmLab WHERE " + where;
                sqls += sql;
            //}

            // Sys_FrmRB.
            listNames.Add("Sys_FrmRB");
            sql = "@SELECT * FROM Sys_FrmRB WHERE " + where;
            sqls += sql;

            // ele.
            listNames.Add("Sys_FrmEle");
            sql = "@SELECT FK_MapData,MyPK,EleType,EleID,EleName,X,Y,W,H FROM Sys_FrmEle WHERE " + where;
            sqls += sql;

            //Sys_MapFrame.
            listNames.Add("Sys_MapFrame");
            sql = "@SELECT MyPK,FK_MapData,Name,URL,W,H,GroupID FROM Sys_MapFrame WHERE " + where;
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
                    md.RepairMap();
            }

            ds.Tables.Add(dtMapDtl);
            return ds;
        }
        #endregion 模版操作.

        #region 其他功能.
        /// <summary>
        /// 保存枚举
        /// </summary>
        /// <param name="enumKey">键值对</param>
        /// <param name="enumLab">标签</param>
        /// <param name="cfg">配置 @0=xxx@1=yyyy@n=xxxxxc</param>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        public static string SaveEnum(string enumKey, string enumLab, string cfg, string lang="CH")
        {
            SysEnumMain sem = new SysEnumMain();
            sem.No = enumKey;
            if (sem.RetrieveFromDBSources() == 0)
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = lang;
                sem.Insert();
            }
            else
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = lang;
                sem.Update();
            }

            string[] strs = cfg.Split('@');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                string[] kvs = str.Split('=');
                SysEnum se = new SysEnum();
                se.EnumKey = enumKey;
                se.Lang = lang;
                se.IntKey = int.Parse(kvs[0]);
                se.Lab = kvs[1];
                se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                se.Save();
            }
            return "保存成功.";
        }
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
        public static string DB_SFTableList(int pageNumber, int pageSize)
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
        public static string DB_EnumerationList(int pageNumber, int pageSize)
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
        public static string DB_Hiddenfielddata(string fk_mapdata)
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
