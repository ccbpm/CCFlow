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
        /// <summary>
        /// 获得附件信息.
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="pk">主键</param>
        /// <returns>附件信息.</returns>
        public static string GetAthInfos(string fk_mapdata, string pk)
        {
            int num = BP.DA.DBAccess.RunSQL("SELECT COUNT(MYPK) FROM Sys_FrmAttachmentDB WHERE FK_MapData='" + fk_mapdata + "' AND RefPKVal=" + pk);
            return "附件(" + num + ")";
        }

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
            FrmEle fe = null;
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
                    fe = new FrmEle();
                    fe.MyPK = fk_mapdata + "_" + no;
                    if (fe.RetrieveFromDBSources() != 0)
                        throw new Exception("@创建失败，已经有同名元素[" + no + "]的控件.");
                    fe.FK_MapData = fk_mapdata;
                    fe.EleType = "Fieldset";
                    fe.EleName = name;
                    fe.EleID = no;
                    fe.X = x;
                    fe.Y = y;
                    fe.Insert();
                    //CreateOrSaveAthImg(fk_mapdata, no, name, x, y);
                    break;

                case "iFrame": //框架.
                     fe = new FrmEle();
                    fe.MyPK = fk_mapdata + "_" + no;
                    if (fe.RetrieveFromDBSources() != 0)
                        throw new Exception("@创建失败，已经有同名元素[" + no + "]的控件.");
                    fe.FK_MapData = fk_mapdata;
                    fe.EleType = "iFrame";
                    fe.EleName = name;
                    fe.EleID = no;
                    fe.Tag1 = "http://ccflow.org";
                    fe.X = x;
                    fe.Y = y;
                    fe.W = 400;
                    fe.H = 600;
                    fe.Insert();
                    break;
                default:
                    throw new Exception("@没有判断的存储控件:" + ctrlType + ",存储该控件前,需要做判断.");
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
            int i = ath.RetrieveFromDBSources();
            if (i == 0)
            {
                ath.SaveTo = SystemConfig.PathOfDataUser + "\\UploadFile\\" + fk_mapdata + "\\";
            }

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

            if (dtl.RetrieveFromDBSources() == 0)
            {
                if (dtlName == null)
                    dtlName = dtlNo;

                //把他的模式复制过来.
                MapData md = new MapData(fk_mapdata);
                dtl.PTableModel = md.PTableModel;


                dtl.W = 500;
            }

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
            //检查是否可以创建字段? 
            MapData md = new MapData();
            md.No = fk_mapdata;
            if(md.RetrieveFromDBSources() == 1)
                md.CheckPTableSaveModel(fieldName);

            //外键字段表.
            SFTable sf = new SFTable(fk_SFTable);

            if (DataType.IsNullOrEmpty(fieldDesc) == true)
                fieldDesc = sf.Name;

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
                case SrcType.CreateTable:
                case SrcType.TableOrView:
                case SrcType.BPClass:
                case SrcType.SQL: //是sql模式.
                    attr.LGType = FieldTypeS.FK;
                    break;
                default:
                    attr.LGType = FieldTypeS.Normal;
                    break;
            }

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            attr.GroupID = groupField.OID;
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
            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            ma.GroupID = groupField.OID;

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
        public static void NewField(string frmID, string field, string fieldDesc, int mydataType, float x, float y, int colSpan = 1)
        {
            //检查是否可以创建字段? 
            MapData md = new MapData(frmID);
            md.CheckPTableSaveModel(field);

            MapAttr ma = new MapAttr();
            ma.FK_MapData = frmID;
            ma.KeyOfEn = field;
            ma.Name = fieldDesc;
            ma.MyDataType = mydataType;
            ma.X = x;
            ma.Y = y;

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, frmID, GroupFieldAttr.CtrlType, "");
            ma.GroupID = groupField.OID;

            ma.Insert();
        }
        public static void NewEnumField(string fk_mapdata, string field, string fieldDesc, string enumKey, UIContralType ctrlType,
           float x, float y, int colSpan = 1)
        {
            //检查是否可以创建字段? 
            MapData md = new MapData(fk_mapdata);
            md.CheckPTableSaveModel(field);

            MapAttr ma = new MapAttr();
            ma.FK_MapData = fk_mapdata;
            ma.KeyOfEn = field;
            ma.Name = fieldDesc;
            ma.MyDataType = DataType.AppInt;
            ma.X = x;
            ma.Y = y;
            ma.UIIsEnable = true;
            ma.LGType = FieldTypeS.Enum;
            ma.UIContralType = ctrlType;
            ma.UIBindKey = enumKey;

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            ma.GroupID = groupField.OID;

            ma.Insert();

            if (ma.UIContralType != UIContralType.RadioBtn)
                return;

            //删除可能存在的数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + ma.KeyOfEn + "' AND FK_MapData='" + ma.FK_MapData + "'");

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
                rb.IntKey = item.IntKey;
                rb.X = ma.X;

                //让其变化y值.
                rb.Y = ma.Y + idx * 30;
                rb.Insert();
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
                return "err@前缀已经使用：" + gKey + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";

            GroupField gf = new GroupField();
            gf.Lab = gName;
            gf.FrmID = frmID;
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
            attrN.DefVal = "@WebUser.Name";
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
            if (nd.RetrieveFromDBSources() != 0 && DataType.IsNullOrEmpty(nd.FocusField) == true)
            {
                nd.FocusField = "@" + gKey + "_Note";
                nd.Update();
            }
             * */
        }
        #endregion 创建修改字段.

        #region 模版操作.
        /// <summary>
        /// 创建一个审核分组
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="prx">前缀</param>
        public static void CreateCheckGroup(string frmID, string groupName, string prx)
        {
            GroupField gf = new GroupField();
            gf.Lab = groupName;
            gf.FrmID = frmID;
            int i=gf.Retrieve(GroupFieldAttr.Lab, groupName, GroupFieldAttr.FrmID, frmID);
            if (i ==0)
                gf.Insert();

            MapAttr attr = new MapAttr();
            attr.FK_MapData = frmID;
            attr.KeyOfEn = prx + "_Note";
            attr.Name = "审核意见"; // sta;  // this.ToE("CheckNote", "审核意见");
            attr.MyDataType = DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.UIIsEnable = true;
            attr.UIIsLine = true;
            attr.MaxLen = 4000;
            attr.SetValByKey(MapAttrAttr.ColSpan, 4);
           // attr.ColSpan = 4;
            attr.GroupID = gf.OID;
            attr.UIHeight = 23 * 3;
            attr.Idx = 1;
            attr.Insert();
            attr.Update("Idx", 1);


            attr = new MapAttr();
            attr.FK_MapData = frmID;
            attr.KeyOfEn = prx + "_Checker";
            attr.Name = "审核人";// "审核人";
            attr.MyDataType = DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.MaxLen = 50;
            attr.MinLen = 0;
            attr.UIIsEnable = true;
            attr.UIIsLine = false;
            attr.DefVal = "@WebUser.No";
            attr.UIIsEnable = false;
            attr.GroupID = gf.OID;
            attr.IsSigan = true;
            attr.Idx = 2;
            attr.Insert();
            attr.Update("Idx", 2);

            attr = new MapAttr();
            attr.FK_MapData = frmID;
            attr.KeyOfEn = prx + "_RDT";
            attr.Name = "审核日期"; // "审核日期";
            attr.MyDataType = DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.UIIsEnable = true;
            attr.UIIsLine = false;
            attr.DefVal = "@RDT";
            attr.UIIsEnable = false;
            attr.GroupID = gf.OID;
            attr.Idx = 3;
            attr.Insert();
            attr.Update("Idx", 3);
        }
        /// <summary>
        /// 创建表单
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="frmName">表单名称</param>
        /// <param name="frmTreeID">表单类别编号（表单树ID）</param>
        /// <param name="frmType">表单类型</param>
        public static void CreateFrm(string frmID, string frmName, string frmTreeID, FrmType frmType = FrmType.FreeFrm)
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
        /// 修复表单.
        /// </summary>
        /// <param name="frmID"></param>
        public static void RepareCCForm(string frmID)
        {
            MapAttr attr = new MapAttr();
            if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, frmID) == false)
            {
                attr.FK_MapData = frmID;
                attr.KeyOfEn = "OID";
                attr.Name = "主键";
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
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="jsonStrOfH5Frm"></param>
        public static void SaveFrm(string fk_mapdata, string jsonStrOfH5Frm)
        {
            // BP.DA.DataType.WriteFile("D:\\AAAAAA.JSON", jsonStrOfH5Frm);
            //return;
            JsonData jd = JsonMapper.ToObject(jsonStrOfH5Frm);
            if (jd.IsObject == false)
                throw new Exception("err@表单格式不正确，保存失败。");

            JsonData form_MapData = jd["c"];

            //直接保存表单图信息.
            MapData mapData = new MapData(fk_mapdata);
            mapData.FrmW = float.Parse(form_MapData["width"].ToJson());
            mapData.FrmH = float.Parse(form_MapData["height"].ToJson());
         //   mapData.DesignerTool = "Html5";
            mapData.Update();

            //执行保存.
            SaveFrm(fk_mapdata, jd);

            //一直没有找到设置3列，自动回到四列的情况.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET ColSpan=3 WHERE  UIHeight<=23 AND ColSpan=4");
        }
        /// <summary>
        /// 将表单设计串格式化为Json.
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
                labelPKs += item.MyPK + "@";

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
            {
                if (item.KeyOfEn == "OID")
                    continue;
                if (item.UIVisible == false)
                    continue;

                attrPKs += item.KeyOfEn + "@";
            }
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
            {
                athMultis += item.NoOfObj + "@";
            }
            athMultis += "@";

            //图片附件.
            string athImgs = "@";
            FrmImgAths fias = new FrmImgAths(); ;
            fias.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
            foreach (FrmImgAth item in fias)
            {
                athImgs += item.CtrlID + "@";
            }
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
                delSqls += "@DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapdata + "' AND KeyOfEn NOT IN ('OID')";
                delSqls += "@DELETE FROM Sys_FrmRB WHERE FK_MapData='" + fk_mapdata + "'"; //枚举值的相关rb. 
                delSqls += "@DELETE FROM Sys_MapDtl WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmBtn WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmLine WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmLab WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmLink WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmImg WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmEle WHERE FK_MapData='" + fk_mapdata + "'";
                delSqls += "@DELETE FROM Sys_FrmImgAth WHERE FK_MapData='" + fk_mapdata + "'";

                BP.DA.DBAccess.RunSQLs(delSqls);
                return;
            }

            string flowEle = "";
            string sqls = "";

            string nodeIDStr = fk_mapdata.Replace("ND", "");
            int nodeID = 0;
            if (BP.DA.DataType.IsNumStr(nodeIDStr) == true)
                nodeID = int.Parse(nodeIDStr);

            //流程控件.
            string flowCtrls = "";

            //循环元素.
            for (int idx = 0, jControl = form_Controls.Count; idx < jControl; idx++)
            {
                JsonData control = form_Controls[idx];  //不存在控件类型不进行处理，继续循环.
                if (control == null || control["CCForm_Shape"] == null)
                    continue;

                string shape = control["CCForm_Shape"].ToString();

                if (control.Keys.Contains("CCForm_MyPK") == false)
                    continue;

                if (control["CCForm_MyPK"] == null)
                    continue;

                string ctrlID = control["CCForm_MyPK"].ToString();

                JsonData properties = control["properties"];  //属性集合.

                #region 装饰类控件.
                switch (shape)
                {
                    case "Label": //保存标签.
                        if (ctrlID.IndexOf("RB_") == 0)
                        {
                            /*让其向下运行.*/
                            shape = "RadioButtonItem";
                        }
                        else
                        {
                            BP.Sys.CCFormParse.SaveLabel(fk_mapdata, control, properties, labelPKs, ctrlID);
                            labelPKs = labelPKs.Replace(ctrlID + "@", "@");
                        }
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
                if (shape.Contains("TextBox") == true
                    || shape.Contains("DropDownList") == true)
                {
                    BP.Sys.CCFormParse.SaveMapAttr(fk_mapdata, ctrlID, shape, control, properties, attrPKs);
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

                if (shape == "Dtl")
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

                //存储到FrmEle 类的控件，都可以使用该方法保存.
                if (shape == "Fieldset"
                    || shape == FrmEle.iFrame
                    || shape == FrmEle.Fieldset
                    || shape == FrmEle.HandSiganture)
                {
                    //记录已经存在的ID， 需要当时保存.
                    BP.Sys.CCFormParse.SaveFrmEle(fk_mapdata, shape, ctrlID, x, y, height, width);
                    eleIDs = eleIDs.Replace(ctrlID + "@", "@");
                    continue;
                }

                if (shape == "RadioButton")
                {
                    if (ctrlID.Contains("=") == true)
                        continue;

                    //记录已经存在的ID， 需要当时保存.
                    if (ctrlID.Contains("RB_") == true)
                        ctrlID = ctrlID.Substring(3);

                    string str = BP.Sys.CCFormParse.SaveFrmRadioButton(fk_mapdata, ctrlID, x, y);
                    if (str == null)
                        continue;

                    attrPKs = attrPKs.Replace(str + "@", "@");
                    continue;
                }

                if (shape == "RadioButton")
                {
                    continue;
                }
                #endregion 附件.

                #region 处理流程组件, 如果已经传来节点ID,说明是节点表单.
                //流程类的组件,都记录下来放入到Sys_MapData.FlowCtrls 字段里. 记录控件的位置，原来记录到节点里的都要取消掉.
                //@zhoupeng
                if (shape == "FlowChart" || shape == "FrmCheck" || shape == "SubFlowDtl" || shape == "ThreadDtl")
                {
                    if (flowCtrls.Contains(shape) == false)
                        flowCtrls += "@Ctrl=" + shape + ",X=" + x + ",Y=" + y + ",H=" + height + ",W=" + width;
                }

                if (nodeID != 0)
                {
                    sqls = "";
                    switch (shape)
                    {
                        case "FlowChart":
                            if (DBAccess.RunSQLReturnString("SELECT FrmTrackSta FROM WF_Node WHERE NodeID=" + nodeID) == "0")
                            {
                                /*状态是 0 就把他启用起来. */
                                sqls += "@UPDATE WF_Node SET FrmTrackSta=1,FrmTrack_X=" + x + ",FrmTrack_Y=" + y + ",FrmTrack_H=" + height + ", FrmTrack_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            else
                            {
                                /* 仅仅更新位置与高度。*/
                                sqls += "@UPDATE WF_Node SET FrmTrack_X=" + x + ",FrmTrack_Y=" + y + ",FrmTrack_H=" + height + ", FrmTrack_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            flowEle += shape + ",";
                            continue;
                        case "FrmCheck":
                            if (DBAccess.RunSQLReturnString("SELECT FWCSta FROM WF_Node WHERE NodeID=" + nodeID) == "0")
                            {
                                /*状态是 0 就把他启用起来. */
                                sqls += "@UPDATE WF_Node SET FWCSta=1,FWC_X=" + x + ",FWC_Y=" + y + ",FWC_H=" + height + ", FWC_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            else
                            {
                                /* 仅仅更新位置与高度。*/
                                sqls += "@UPDATE WF_Node SET FWC_X=" + x + ",FWC_Y=" + y + ",FWC_H=" + height + ", FWC_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            flowEle += shape + ",";
                            continue;
                        case "SubFlowDtl": //子流程
                            if (DBAccess.RunSQLReturnString("SELECT SFSta FROM WF_Node WHERE NodeID=" + nodeID) == "0")
                            {
                                /*状态是 0 就把他启用起来. */
                                sqls += "@UPDATE WF_Node SET SFSta=1,SF_X=" + x + ",SF_Y=" + y + ",SF_H=" + height + ", SF_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            else
                            {
                                /* 仅仅更新位置与高度。*/
                                sqls += "@UPDATE WF_Node SET SF_X=" + x + ",SF_Y=" + y + ",SF_H=" + height + ", SF_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            flowEle += shape + ",";
                            continue;
                        case "ThreadDtl": //子线程
                            if (DBAccess.RunSQLReturnString("SELECT FrmThreadSta FROM WF_Node WHERE NodeID=" + nodeID) == "0")
                            {
                                /*状态是 0 就把他启用起来. */
                                sqls += "@UPDATE WF_Node SET FrmThreadSta=1,FrmThread_X=" + x + ",FrmThread_Y=" + y + ",FrmThread_H=" + height + ",FrmThread_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            else
                            {
                                /* 仅仅更新位置与高度。*/
                                sqls += "@UPDATE WF_Node SET FrmThread_X=" + x + ",FrmThread_Y=" + y + ",FrmThread_H=" + height + ", FrmThread_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            flowEle += shape + ",";
                            continue;
                        case "FrmTransferCustom": //流转自定义
                            if (DBAccess.RunSQLReturnString("SELECT FTCSta FROM WF_Node WHERE NodeID=" + nodeID) == "0")
                            {
                                /*状态是 0 就把他启用起来. */
                                sqls += "@UPDATE WF_Node SET FTCSta=1,FTC_X=" + x + ",FTC_Y=" + y + ",FTC_H=" + height + ",FTC_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            else
                            {
                                /* 仅仅更新位置与高度。*/
                                sqls += "@UPDATE WF_Node SET FTC_X=" + x + ",FTC_Y=" + y + ",FrmThread_H=" + height + ",FTC_W=" + width + " WHERE NodeID=" + nodeIDStr;
                            }
                            flowEle += shape + ",";
                            continue;
                        default:
                            break;
                    }
                }
                #endregion 处理流程组件.

                if (shape == "FlowChart" || shape == "FrmCheck" || shape == "SubFlowDtl" || shape == "ThreadDtl")
                    continue;

                throw new Exception("@没有判断的ccform保存控件的类型:shape = " + shape);
            }


            #region 处理节点表单。
            if (nodeID != 0)
            {
                //轨迹组件.
                if (flowEle.Contains("FlowChart") == false)
                    sqls += "@UPDATE WF_Node SET FrmTrackSta=0 WHERE NodeID=" + nodeID;

                //审核组件.
                if (flowEle.Contains("FrmCheck") == false)
                    sqls += "@UPDATE WF_Node SET FWCSta=0 WHERE NodeID=" + nodeID;

                //子流程组件.
                if (flowEle.Contains("SubFlowDtl") == false)
                    sqls += "@UPDATE WF_Node SET SFSta=0 WHERE NodeID=" + nodeID;

                //子线城组件.
                if (flowEle.Contains("ThreadDtl") == false)
                    sqls += "@UPDATE WF_Node SET FrmThreadSta=0 WHERE NodeID=" + nodeID;

                //自定义流程组件.
                if (flowEle.Contains("FrmTransferCustom") == false)
                    sqls += "@UPDATE WF_Node SET FTCSta=0 WHERE NodeID=" + nodeID;
            }

            //执行要更新的sql.
            if (sqls != "")
            {
                BP.DA.DBAccess.RunSQLs(sqls);
                sqls = "";
            }

            //更新组件. @zhoupeng.
            DBAccess.RunSQL("UPDATE Sys_MapData SET FlowCtrls='"+flowCtrls+"' WHERE No='"+fk_mapdata+"'");
            #endregion 处理节点表单。

            #region 删除没有替换下来的 PKs, 说明这些都已经被删除了.
            string[] pks = labelPKs.Split('@');
            sqls = "";
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmLab WHERE MyPK='" + pk + "'";
            }

            pks = btnsPKs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmBtn WHERE MyPK='" + pk + "'";
            }

            pks = linkPKs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmLink WHERE MyPK='" + pk + "'";
            }

            pks = imgPKs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmImg WHERE MyPK='" + pk + "'";
            }

            pks = attrPKs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;

                if (pk == "OID")
                    continue;

                sqls += "@DELETE FROM Sys_MapAttr WHERE KeyOfEn='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
                sqls += "@DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }

            pks = dtlPKs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;

                //调用删除逻辑.
                MapDtl dtl = new MapDtl();
                dtl.No = pk;
                dtl.RetrieveFromDBSources();
                dtl.Delete();

                // sqls += "@DELETE FROM Sys_MapDtl WHERE No='" + pk + "'";
            }


            pks = athMultis.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;
                sqls += "@DELETE FROM Sys_FrmAttachment WHERE NoOfObj='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }

            //删除图片附件.
            pks = athImgs.Split('@');
            foreach (string pk in pks)
            {
                if (DataType.IsNullOrEmpty(pk))
                    continue;

                sqls += "@DELETE FROM Sys_FrmImgAth WHERE CtrlID='" + pk + "' AND FK_MapData='" + fk_mapdata + "'";
            }

            //删除这些，没有替换下来的数据.
            BP.DA.DBAccess.RunSQLs(sqls);
            #endregion 删除没有替换下来的 PKs, 说明这些都已经被删除了.

            //清空缓存
            MapData mymd = new MapData(fk_mapdata);
            mymd.RepairMap();
            BP.Sys.SystemConfig.DoClearCash();
        }

        /// <summary>
        /// 复制表单
        /// </summary>
        /// <param name="srcFrmID">源表单ID</param>
        /// <param name="copyFrmID">copy到表单ID</param>
        /// <param name="copyFrmName">表单名称</param>
        public static string CopyFrm(string srcFrmID, string copyFrmID, string copyFrmName, string fk_frmTree)
        {
            MapData mymd = new MapData();
            mymd.No = copyFrmID;
            if (mymd.RetrieveFromDBSources() == 1)
                throw new Exception("@目标表单ID:"+copyFrmID+"已经存在，位于:"+mymd.FK_FormTreeText+"目录下.");

            //获得源文件信息.
            DataSet ds = GenerHisDataSet(srcFrmID);

            //导入表单文件.
            ImpFrmTemplate(copyFrmID, ds, false);

            //复制模版文件.
            MapData md = new MapData(copyFrmID);

            if (md.HisFrmType == FrmType.ExcelFrm)
            {
                /*如果是excel表单，那就需要复制excel文件.*/
                string srcFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate/" + srcFrmID + ".xls";
                string toFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate/" + copyFrmID + ".xls";
                if (System.IO.File.Exists(srcFile) == true)
                {
                    if (System.IO.File.Exists(toFile) == false)
                        System.IO.File.Copy(srcFile, toFile, false);
                }

                srcFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate/" + srcFrmID + ".xlsx";
                toFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate/" + copyFrmID + ".xlsx";
                if (System.IO.File.Exists(srcFile) == true)
                {
                    if (System.IO.File.Exists(toFile) == false)
                        System.IO.File.Copy(srcFile, toFile, false);
                }
            }

            md.Retrieve();

            md.FK_FormTree = fk_frmTree;
            md.FK_FrmSort = fk_frmTree;
            md.Name = copyFrmName;
            md.Update();
           
            return "表单复制成功,您需要重新登录，或者刷新才能看到。";
        }
        /// <summary>
        /// 导入表单API
        /// </summary>
        /// <param name="toFrmID">要导入的表单ID</param>
        /// <param name="fromds">数据源</param>
        /// <param name="isSetReadonly">是否把空间设置只读？</param>
        public static void ImpFrmTemplate(string toFrmID, DataSet fromds, bool isSetReadonly)
        {
            MapData.ImpMapData(toFrmID, fromds);
        }
        /// <summary>
        /// 获得表单信息.
        /// </summary>
        /// <param name="frmID">表单</param>
        /// <returns></returns>
        public static System.Data.DataSet GenerHisDataSet(string frmID, string frmName=null)
        {
            DataSet ds = new DataSet();

            //创建实体对象.
            MapData md = new MapData(frmID);

            if (frmName != null)
                md.Name = frmName;

            //加入主表信息.
            DataTable Sys_MapData = md.ToDataTableField("Sys_MapData");
            ds.Tables.Add(Sys_MapData);
             
            //加入分组表.
            DataTable Sys_GroupField = md.GroupFields.ToDataTableField("Sys_GroupField");
            ds.Tables.Add(Sys_GroupField);
             
            //加入明细表.
            DataTable Sys_MapDtl = md.MapDtls.ToDataTableField("Sys_MapDtl");
            ds.Tables.Add(Sys_MapDtl);

            //加入枚举表.
            DataTable Sys_Menu = md.SysEnums.ToDataTableField("Sys_Enum");
            ds.Tables.Add(Sys_Menu);

            //加入外键属性.
            DataTable Sys_MapAttr = md.MapAttrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(Sys_MapAttr);

            //加入扩展属性.
            DataTable Sys_MapExt = md.MapExts.ToDataTableField("Sys_MapExt");
            ds.Tables.Add(Sys_MapExt);

            //线.
            DataTable Sys_FrmLine = md.FrmLines.ToDataTableField("Sys_FrmLine");
            ds.Tables.Add(Sys_FrmLine);

            //link.
            DataTable Sys_FrmLink = md.FrmLinks.ToDataTableField("Sys_FrmLink");
            ds.Tables.Add(Sys_FrmLink);

            //btn.
            DataTable Sys_FrmBtn = md.FrmBtns.ToDataTableField("Sys_FrmBtn");
            ds.Tables.Add(Sys_FrmBtn);

            //Sys_FrmLab.
            DataTable Sys_FrmLab = md.FrmLabs.ToDataTableField("Sys_FrmLab");
            ds.Tables.Add(Sys_FrmLab);

            //img.
            DataTable Sys_FrmImg = md.FrmImgs.ToDataTableField("Sys_FrmImg");
            ds.Tables.Add(Sys_FrmImg);

            //Sys_FrmRB.
            DataTable Sys_FrmRB = md.FrmRBs.ToDataTableField("Sys_FrmRB");
            ds.Tables.Add(Sys_FrmRB);

            //Sys_FrmEle.
            DataTable Sys_FrmEle = md.FrmEles.ToDataTableField("Sys_FrmEle");
            ds.Tables.Add(Sys_FrmEle);

            //Sys_MapFrame.
            DataTable Sys_MapFrame = md.MapFrames.ToDataTableField("Sys_MapFrame");
            ds.Tables.Add(Sys_MapFrame);

            //Sys_FrmAttachment.
            DataTable Sys_FrmAttachment = md.FrmAttachments.ToDataTableField("Sys_FrmAttachment");
            ds.Tables.Add(Sys_FrmAttachment);

            //FrmImgAths. 上传图片附件.
            DataTable Sys_FrmImgAth = md.FrmImgAths.ToDataTableField("Sys_FrmImgAth");
            ds.Tables.Add(Sys_FrmImgAth);

            //FrmImgAthDBs 上传图片信息
            DataTable Sys_FrmImgAthDB = md.FrmImgAthDB.ToDataTableField("Sys_FrmImgAthDB");
            ds.Tables.Add(Sys_FrmImgAthDB);

            return ds;
        }
        public static System.Data.DataSet GenerHisDataSet_AllEleInfo(string fk_mapdata)
        {

            MapData md = new MapData(fk_mapdata);

            //求出 frmIDs 
            string frmIDs = "'" + fk_mapdata + "'";
            MapDtls mdtls = new MapDtls(md.No);
            foreach (MapDtl item in mdtls)
                frmIDs += ",'" + item.No + "'";
            
            DataSet ds = new DataSet();

            //加入主表信息.
            DataTable Sys_MapData = md.ToDataTableField("Sys_MapData");
            ds.Tables.Add(Sys_MapData);

            //加入分组表.
            GroupFields gfs = new GroupFields();
            gfs.RetrieveIn(GroupFieldAttr.FrmID, frmIDs);
            DataTable Sys_GroupField = gfs.ToDataTableField("Sys_GroupField");
            ds.Tables.Add(Sys_GroupField);

            //加入明细表.
            DataTable Sys_MapDtl = md.MapDtls.ToDataTableField("Sys_MapDtl");
            ds.Tables.Add(Sys_MapDtl);

            //加入枚举表.
            SysEnums ses = new SysEnums();
            ses.RetrieveInSQL(SysEnumAttr.EnumKey, "SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + frmIDs + ") ");
            DataTable Sys_Menu = ses.ToDataTableField("Sys_Enum");
            ds.Tables.Add(Sys_Menu);

            //加入字段属性.
            MapAttrs attrs = new MapAttrs();
            attrs.RetrieveIn(MapAttrAttr.FK_MapData,   frmIDs);
            DataTable Sys_MapAttr = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(Sys_MapAttr);

            //加入扩展属性.
            MapExts exts = new MapExts();
            exts.RetrieveIn(MapAttrAttr.FK_MapData, frmIDs);
            DataTable Sys_MapExt = exts.ToDataTableField("Sys_MapExt");
            ds.Tables.Add(Sys_MapExt);

            //线.
            DataTable Sys_FrmLine = md.FrmLines.ToDataTableField("Sys_FrmLine");
            ds.Tables.Add(Sys_FrmLine);

            //link.
            DataTable Sys_FrmLink = md.FrmLinks.ToDataTableField("Sys_FrmLink");
            ds.Tables.Add(Sys_FrmLink);

            //btn.
            DataTable Sys_FrmBtn = md.FrmBtns.ToDataTableField("Sys_FrmBtn");
            ds.Tables.Add(Sys_FrmBtn);

            //Sys_FrmLab.
            DataTable Sys_FrmLab = md.FrmLabs.ToDataTableField("Sys_FrmLab");
            ds.Tables.Add(Sys_FrmLab);

            //img.
            DataTable Sys_FrmImg = md.FrmImgs.ToDataTableField("Sys_FrmImg");
            ds.Tables.Add(Sys_FrmImg);

            //Sys_FrmRB.
            DataTable Sys_FrmRB = md.FrmRBs.ToDataTableField("Sys_FrmRB");
            ds.Tables.Add(Sys_FrmRB);

            //Sys_FrmEle.
            DataTable Sys_FrmEle = md.FrmEles.ToDataTableField("Sys_FrmEle");
            ds.Tables.Add(Sys_FrmEle);

            //Sys_MapFrame.
            DataTable Sys_MapFrame = md.MapFrames.ToDataTableField("Sys_MapFrame");
            ds.Tables.Add(Sys_MapFrame);

            //Sys_FrmAttachment.
            DataTable Sys_FrmAttachment = md.FrmAttachments.ToDataTableField("Sys_FrmAttachment");
            ds.Tables.Add(Sys_FrmAttachment);

            //FrmImgAths. 上传图片附件.
            DataTable Sys_FrmImgAth = md.FrmImgAths.ToDataTableField("Sys_FrmImgAth");
            ds.Tables.Add(Sys_FrmImgAth);

            return ds;
        }
        /// <summary>
        /// 获得表单模版dataSet格式.
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="isCheckFrmType">是否检查表单类型</param>
        /// <returns>DataSet</returns>
        public static System.Data.DataSet GenerHisDataSet_AllEleInfo2017(string fk_mapdata, bool isCheckFrmType = false)
        {
            MapData md = new MapData(fk_mapdata);

            //从表.
            string sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
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
            sql = "SELECT * FROM Sys_GroupField WHERE  FrmID IN (" + ids + ")";
            sqls += sql;

            // Sys_Enum
            listNames.Add("Sys_Enum");
            sql = "@SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + ids + ") ) order By EnumKey,IntKey";
            sqls += sql;

            // 审核组件
            string nodeIDstr = fk_mapdata.Replace("ND", "");
            if (DataType.IsNumStr(nodeIDstr))
            {
                // 审核组件状态:0 禁用;1 启用;2 只读;
                listNames.Add("WF_Node");
                sql = "@SELECT * FROM WF_Node WHERE NodeID=" + nodeIDstr + " AND  ( FWCSta >0  OR SFSta >0 )";
                sqls += sql;
            }

            string where = " FK_MapData IN (" + ids + ")";

            // Sys_MapData.
            listNames.Add("Sys_MapData");
            sql = "@SELECT * FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
            sqls += sql;

            // Sys_MapAttr.
            listNames.Add("Sys_MapAttr");

            sql = "@SELECT * FROM Sys_MapAttr WHERE " + where + " AND KeyOfEn NOT IN('WFState') ORDER BY FK_MapData, IDX  ";
            sqls += sql;


            // Sys_MapExt.
            listNames.Add("Sys_MapExt");
            sql = "@SELECT * FROM Sys_MapExt WHERE " + where;
            sqls += sql;

            //if (isCheckFrmType == true && md.HisFrmType == FrmType.FreeFrm)
            //{
            // line.
            listNames.Add("Sys_FrmLine");
            sql = "@SELECT * FROM Sys_FrmLine WHERE " + where;
            sqls += sql;

            // link.
            listNames.Add("Sys_FrmLink");
            sql = "@SELECT * FROM Sys_FrmLink WHERE " + where;
            sqls += sql;

            // btn.
            listNames.Add("Sys_FrmBtn");
            sql = "@SELECT * FROM Sys_FrmBtn WHERE " + where;
            sqls += sql;

            // Sys_FrmImg.
            listNames.Add("Sys_FrmImg");
            sql = "@SELECT * FROM Sys_FrmImg WHERE " + where;
            sqls += sql;

            // Sys_FrmLab.
            listNames.Add("Sys_FrmLab");
            sql = "@SELECT * FROM Sys_FrmLab WHERE " + where;
            sqls += sql;
            //}

            // Sys_FrmRB.
            listNames.Add("Sys_FrmRB");
            sql = "@SELECT * FROM Sys_FrmRB WHERE " + where;
            sqls += sql;

            // ele.
            listNames.Add("Sys_FrmEle");
            sql = "@SELECT * FROM Sys_FrmEle WHERE " + where;
            sqls += sql;

            //Sys_MapFrame.
            listNames.Add("Sys_MapFrame");
            sql = "@SELECT * FROM Sys_MapFrame WHERE " + where;
            sqls += sql;

            // Sys_FrmAttachment. 
            listNames.Add("Sys_FrmAttachment");
            /* 20150730 小周鹏修改 添加AtPara 参数 START */
            //sql = "@SELECT  MyPK,FK_MapData,UploadType,X,Y,W,H,NoOfObj,Name,Exts,SaveTo,IsUpload,IsDelete,IsDownload "
            // + " FROM Sys_FrmAttachment WHERE " + where + " AND FK_Node=0";
            sql = "@SELECT * "
                + " FROM Sys_FrmAttachment WHERE " + where + "";

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
            {
                for (int i = 0; i < listNames.Count; i++)
                {
                    string s = strs[i];
                    if (DataType.IsNullOrEmpty(s))
                        continue;
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(s);
                    dt.TableName = listNames[i];
                    ds.Tables.Add(dt);
                }
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
        public static string SaveEnum(string enumKey, string enumLab, string cfg, bool isNew, string lang = "CH")
        {
            SysEnumMain sem = new SysEnumMain();
            sem.No = enumKey;
            int dataCount = sem.RetrieveFromDBSources();
            if (dataCount > 0 && isNew)
                return "err@已存在枚举" + enumKey + ",请修改枚举名字";

            if (dataCount == 0)
            {
                sem.Name = enumLab;
                sem.CfgVal = cfg;
                sem.Lang = lang;
                sem.DirectInsert();
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
                if (DataType.IsNullOrEmpty(str))
                    continue;
                string[] kvs = str.Split('=');
                SysEnum se = new SysEnum();
                se.EnumKey = enumKey;
                se.Lang = lang;
                se.IntKey = int.Parse(kvs[0]);
                //杨玉慧
                //解决当  枚举值含有 ‘=’号时，保存不进去的方法
                string[] kvsValues = new string[kvs.Length - 1];
                for (int i = 0; i < kvsValues.Length; i++)
                {
                    kvsValues[i] = kvs[i + 1];
                }
                se.Lab = string.Join("=", kvsValues);
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
            string s = string.Empty;
            try
            {
                if (isQuanPin == true)
                {
                    s = BP.DA.DataType.ParseStringToPinyin(name);
                    if (s.Length > 15)
                        s = BP.DA.DataType.ParseStringToPinyinJianXie(name);
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
                    string headStr = s.Substring(0, 1);
                    if (DataType.IsNumStr(headStr) == true)
                        s = "F" + s;
                }
                //去掉空格，去掉点.
                s = s.Replace(" ", "");
                s = s.Replace(".", "");
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 转拼音全拼/简写方法(若转换后以数字开头，则前面加F)
        /// <para>added by liuxc,2017-9-25</para>
        /// </summary>
        /// <param name="name">中文字符串</param>
        /// <param name="isQuanPin">是否转换全拼</param>
        /// <param name="removeSpecialSymbols">是否去除特殊符号，仅保留汉字、数字、字母、下划线</param>
        /// <param name="maxLen">转化后字符串最大长度，0为不限制</param>
        /// <returns>转化后的拼音，不成功则抛出异常.</returns>
        public static string ParseStringToPinyinField(string name, bool isQuanPin, bool removeSpecialSymbols, int maxLen)
        {
            string s = string.Empty;

            if (removeSpecialSymbols)
                name = DataType.ParseStringForName(name, maxLen);

            try
            {
                if (isQuanPin == true)
                    s = BP.DA.DataType.ParseStringToPinyin(name);
                else
                    s = BP.DA.DataType.ParseStringToPinyinJianXie(name);


                //如果全拼长度超过maxLen，则取前maxLen长度的字符
                if (maxLen > 0 && s.Length > maxLen)
                    s = s.Substring(0, maxLen);

                if (s.Length > 0)
                {
                    //去除开头数字
                    string headStr = s.Substring(0, 1);
                    if (DataType.IsNumStr(headStr) == true)
                        s = "F" + (s.Length > maxLen - 1 ? s.Substring(0, maxLen - 1) : s);
                }

                return s;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 多音字转拼音
        /// </summary>
        /// <param name="charT">单个汉字</param>
        /// <returns>包含返回拼音，否则返回null</returns>
        public static string ChinaMulTonesToPinYin(string charT)
        {
            try
            {
                ChMulToneXmls mulChs = new ChMulToneXmls();
                mulChs.RetrieveAll();
                foreach (ChMulToneXml en in mulChs)
                {
                    if (en.No.Equals(charT))
                    {
                        return en.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
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

            //转化成json.
            return BP.Tools.Json.ToJson(sftables.ToDataTableField());
            // return BP.Tools.Entitis2Json.ConvertEntitis2GridJsonOnlyData(sftables, RowCount);
        }
        /// <summary>
        /// 获得隐藏字段集合.
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <returns></returns>
        public static string DB_Hiddenfielddata(string fk_mapdata)
        {
            MapAttrs mapAttrs = new MapAttrs();
            QueryObject obj = new QueryObject(mapAttrs);
            obj.AddWhere(MapAttrAttr.FK_MapData, fk_mapdata);
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.UIVisible, "0");
            obj.addAnd();
            obj.AddWhere(MapAttrAttr.EditType, "0");
            //查询
            obj.DoQuery();

            return mapAttrs.ToJson();
        }
        #endregion 其他功能.

    }
}
