using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.En;
using BP.DA;

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
                    CreateOrSaveDtl(fk_mapdata, no, name);
                    break;
                case "AthMulti":
                    CreateOrSaveAthMulti(fk_mapdata, no, name);
                    break;
                case "AthSingle":
                    CreateOrSaveAthSingle(fk_mapdata, no, name, x, y);
                    break;
                case "AthImg":
                    CreateOrSaveAthImg(fk_mapdata, no, name, x, y);
                    break;
                case "iFrame": //框架.
                    MapFrame mapFrame = new MapFrame();
                    mapFrame.setMyPK(fk_mapdata + "_" + no);
                    if (mapFrame.RetrieveFromDBSources() != 0)
                        throw new Exception("@创建失败，已经有同名元素[" + no + "]的控件.");
                    mapFrame.setFK_MapData(fk_mapdata);
                    mapFrame.setEleType("iFrame");
                    mapFrame.setName(name);
                    mapFrame.setFrmID(no);
                    mapFrame.URL = "http://ccflow.org";
                 
                    mapFrame.W = 400;
                    mapFrame.H = 600;
                    mapFrame.Insert();
                    break;

                case "HandSiganture"://签字版
                    //检查是否可以创建字段? 
                    MapData md = new MapData(fk_mapdata);
                    md.CheckPTableSaveModel(no);

                    MapAttr ma = new MapAttr();
                    ma.setFK_MapData(fk_mapdata);
                    ma.setKeyOfEn(no);
                    ma.setName(name);
                    ma.setMyDataType(DataType.AppString);
                    ma.setUIContralType(UIContralType.HandWriting);
                   
                    //frmID设置字段所属的分组
                    GroupField groupField = new GroupField();
                    groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
                    ma.setGroupID(groupField.OID);
                    ma.Insert();
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
            no = no.Trim();
            name = name.Trim();

            FrmImgAth ath = new FrmImgAth();
            ath.setFK_MapData(fk_mapdata);
            ath.CtrlID = no;
            ath.setMyPK(fk_mapdata + "_" + no);

            //ath.X = x;
            //ath.Y = y;
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
            ath.setFK_MapData(fk_mapdata);
            ath.NoOfObj = no;

            ath.setMyPK(ath.FK_MapData + "_" + ath.NoOfObj);
            ath.RetrieveFromDBSources();
            ath.UploadType = AttachmentUploadType.Single;
            ath.Name = name;
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
        public static void CreateOrSaveAthMulti(string fk_mapdata, string no, string name)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.setFK_MapData(fk_mapdata);
            ath.NoOfObj = no;
            ath.setMyPK(ath.FK_MapData + "_" + ath.NoOfObj);
            int i = ath.RetrieveFromDBSources();

            if (i == 0)
            {
                // if (!SystemConfig.CustomerNo.Equals("Factory5_mobile"))
                //ath.SaveTo =  BP.Difference.SystemConfig.PathOfDataUser + "/UploadFile/" + fk_mapdata + "/";
                // ath.SaveTo = "/DataUser/UploadFile/" + fk_mapdata + "/";
                if (fk_mapdata.Contains("ND") == true)
                    ath.HisCtrlWay = AthCtrlWay.WorkID;
            }

            ath.UploadType = AttachmentUploadType.Multi;
            ath.Name = name;
            //默认在移动端显示
            ath.SetPara("IsShowMobile", 1);
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
        public static void CreateOrSaveDtl(string fk_mapdata, string dtlNo, string dtlName)
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

            }

            dtl.Name = dtlName;
            dtl.setFK_MapData(fk_mapdata);

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
            if (md.RetrieveFromDBSources() == 1)
                md.CheckPTableSaveModel(fieldName);

            //外键字段表.
            SFTable sf = new SFTable(fk_SFTable);

            if (DataType.IsNullOrEmpty(fieldDesc) == true)
                fieldDesc = sf.Name;

            MapAttr attr = new MapAttr();
            attr.setMyPK(fk_mapdata + "_" + fieldName);
            attr.RetrieveFromDBSources();

            //基本属性赋值.
            attr.setFK_MapData(fk_mapdata);
            attr.setKeyOfEn(fieldName);
            attr.setName(fieldDesc);
            attr.setMyDataType(DataType.AppString);

            attr.setUIContralType(BP.En.UIContralType.DDL);
            attr.UIBindKey = fk_SFTable; //绑定信息.
            //如果绑定的外键是树形结构的，在AtPara中增加标识
            if (sf.CodeStruct == CodeStruct.Tree)
                attr.SetPara("CodeStruct", 1);
            if (DataType.IsNullOrEmpty(sf.RootVal) == false)
                attr.SetPara("ParentNo", sf.RootVal);
           

            //根据外键表的类型不同，设置它的LGType.
            switch (sf.SrcType)
            {
                case SrcType.CreateTable:
                case SrcType.TableOrView:
                case SrcType.BPClass:
                    attr.setLGType(FieldTypeS.FK);
                    break;
                case SrcType.SQL: //是sql模式.
                default:
                    attr.setLGType(FieldTypeS.Normal);
                    break;
            }

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            attr.setGroupID(groupField.OID);
            if (attr.RetrieveFromDBSources() == 0)
                attr.Insert();
            else
                attr.Update();

            //如果是普通的字段, 这个属于外部数据类型,或者webservices类型. sql 语句类型.
            if (attr.LGType == FieldTypeS.Normal)
            {
                MapAttr attrH = new MapAttr();
                attrH.Copy(attr);

                attrH.SetValByKey(MapAttrAttr.UIBindKey,"");
                attrH.SetPara("CodeStruct", "");
                attrH.SetPara("ParentNo", "");
                attrH.SetValByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn + "T");
                attrH.SetValByKey(MapAttrAttr.Name ,attr.Name);
                attrH.SetValByKey(MapAttrAttr.UIContralType , (int)BP.En.UIContralType.TB);
                attrH.SetValByKey(MapAttrAttr.MinLen ,0);
                attrH.SetValByKey(MapAttrAttr.MaxLen ,500);
                attrH.SetValByKey(MapAttrAttr.MyDataType , DataType.AppString);
                attrH.SetValByKey(MapAttrAttr.UIVisible ,false);
                attrH.SetValByKey(MapAttrAttr.UIIsEnable , false);
                attrH.SetValByKey(MapAttrAttr.MyPK , attrH.FK_MapData + "_" + attrH.KeyOfEn);

                if (attrH.RetrieveFromDBSources() == 0)
                    attrH.Insert();
                else
                    attrH.Update();
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
            ma.setFK_MapData(fk_mapdata);
            ma.setKeyOfEn(fieldName);

            //赋值主键。
            ma.setMyPK(ma.FK_MapData + "_" + ma.KeyOfEn);

            //先查询赋值.
            ma.RetrieveFromDBSources();

            ma.setName(fieldDesc);
            ma.setMyDataType(DataType.AppInt);
           
            ma.setUIIsEnable(true);
            ma.setLGType(FieldTypeS.Enum);

            ma.setUIContralType(ctrlType);
            ma.UIBindKey = enumKey;

            if (ma.UIContralType == UIContralType.RadioBtn)
            {
                SysEnums ses = new SysEnums(ma.UIBindKey);
                int idx = 0;
                foreach (SysEnum item in ses)
                {
                    idx++;
                    FrmRB rb = new FrmRB();
                    rb.setFK_MapData(ma.FK_MapData);
                    rb.setKeyOfEn(ma.KeyOfEn);
                    rb.setIntKey(item.IntKey);
                    rb.setMyPK(rb.FK_MapData + "_" + rb.KeyOfEn + "_" + rb.IntKey);
                    rb.RetrieveFromDBSources();

                    rb.setEnumKey(ma.UIBindKey);
                    rb.setLab(item.Lab);
                    rb.Save();
                }
            }
            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            ma.setGroupID(groupField.OID);

            ma.Save();
        }

        public static void NewImage(string frmID, string keyOfEn, string name, float x, float y)
        {
            //BP.Sys.CCFormParse.SaveImage(frmID, control, properties, imgPKs, ctrlID);
            //imgPKs = imgPKs.Replace(ctrlID + "@", "@");
            FrmImg img = new FrmImg();
            img.setMyPK(keyOfEn);
            img.setFK_MapData(frmID);
            img.Name = name;
            img.IsEdit = 1;
            img.HisImgAppType = ImgAppType.Img;
            img.Insert();
        }

        public static void NewField(string frmID, string field, string fieldDesc, int mydataType, float x, float y, int colSpan = 1)
        {
            //检查是否可以创建字段? 
            MapData md = new MapData(frmID);
            md.CheckPTableSaveModel(field);

            MapAttr ma = new MapAttr();
            ma.setFK_MapData(frmID);
            ma.setKeyOfEn(field);
            ma.setName(fieldDesc);
            ma.setMyDataType(mydataType);
            if (mydataType == 7)
                ma.IsSupperText = 1;

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, frmID, GroupFieldAttr.CtrlType, "");
            ma.setGroupID(groupField.OID);

            ma.Insert();
        }
        public static void NewEnumField(string fk_mapdata, string field, string fieldDesc, string enumKey, UIContralType ctrlType,
            int colSpan = 1)
        {
            //检查是否可以创建字段? 
            MapData md = new MapData(fk_mapdata);
            md.CheckPTableSaveModel(field);

            MapAttr ma = new MapAttr();
            ma.setFK_MapData(fk_mapdata);
            ma.setKeyOfEn(field);
            ma.setName(fieldDesc);
            ma.setMyDataType(DataType.AppInt);
            //ma.X = x;
            //ma.Y = y;
            ma.setUIIsEnable(true);
            ma.setLGType(FieldTypeS.Enum);
            ma.setUIContralType(ctrlType);
            ma.UIBindKey = enumKey;

            //frmID设置字段所属的分组
            GroupField groupField = new GroupField();
            groupField.Retrieve(GroupFieldAttr.FrmID, fk_mapdata, GroupFieldAttr.CtrlType, "");
            ma.setGroupID(groupField.OID);
            ma.Insert();

            if (ma.UIContralType != UIContralType.RadioBtn)
                return;

            //删除可能存在的数据.
            DBAccess.RunSQL("DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + ma.KeyOfEn + "' AND FK_MapData='" + ma.FK_MapData + "'");

            SysEnums ses = new SysEnums(ma.UIBindKey);
            int idx = 0;
            foreach (SysEnum item in ses)
            {
                idx++;
                FrmRB rb = new FrmRB();
                rb.setFK_MapData(ma.FK_MapData);
                rb.setKeyOfEn(ma.KeyOfEn);
                rb.setEnumKey(ma.UIBindKey);

                rb.setLab(item.Lab);
                rb.setIntKey(item.IntKey);
                //rb.X = ma.X;
                ////让其变化y值.
                //rb.Y = ma.Y + idx * 30;
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
            attrN.SetValByKey(MapAttrAttr.FK_MapData, frmID);
            attrN.SetValByKey(MapAttrAttr.KeyOfEn, gKey + "_Note");
            attrN.SetValByKey(MapAttrAttr.Name, "审核意见");
            attrN.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
            attrN.setUIContralType(UIContralType.TB);
            attrN.SetValByKey(MapAttrAttr.UIIsEnable, true);
            attrN.SetValByKey(MapAttrAttr.UIIsLine, false);
            //attrN.SetValByKey(MapAttrAttr.DefVal, "@WebUser.Name");
            attrN.SetValByKey(MapAttrAttr.GroupID, gf.OID);
            attrN.SetValByKey(MapAttrAttr.MaxLen, 4000);
            attrN.SetValByKey(MapAttrAttr.UIHeight, 23 * 3);
            attrN.SetValByKey(MapAttrAttr.Idx, 1);
            attrN.Insert();


            attrN = new MapAttr();
            attrN.SetValByKey(MapAttrAttr.FK_MapData, frmID);
            attrN.SetValByKey(MapAttrAttr.KeyOfEn, gKey + "_Checker");
            attrN.SetValByKey(MapAttrAttr.Name, "审核人");
            attrN.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
            attrN.setUIContralType(UIContralType.TB);
            attrN.SetValByKey(MapAttrAttr.UIIsEnable, true);
            attrN.SetValByKey(MapAttrAttr.UIIsLine, false);
            attrN.SetValByKey(MapAttrAttr.DefVal, "@WebUser.Name");
            attrN.SetValByKey(MapAttrAttr.GroupID, gf.OID);
            attrN.SetValByKey(MapAttrAttr.IsSigan,true);
            attrN.SetValByKey(MapAttrAttr.Idx, 2);
            attrN.Insert();


            attrN = new MapAttr();
            attrN.SetValByKey(MapAttrAttr.FK_MapData,frmID);
            attrN.SetValByKey(MapAttrAttr.KeyOfEn, gKey + "_RDT");
            attrN.SetValByKey(MapAttrAttr.Name,"审核日期");
            attrN.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDateTime);
            attrN.setUIContralType(UIContralType.TB);
            attrN.SetValByKey(MapAttrAttr.UIIsEnable, true);
            attrN.SetValByKey(MapAttrAttr.UIIsLine, false);
            attrN.SetValByKey(MapAttrAttr.DefVal, "@RDT");
            attrN.SetValByKey(MapAttrAttr.GroupID, gf.OID);
            attrN.SetValByKey(MapAttrAttr.Idx, 3);
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
            int i = gf.Retrieve(GroupFieldAttr.Lab, groupName, GroupFieldAttr.FrmID, frmID);
            if (i == 0)
                gf.Insert();

            MapAttr attr = new MapAttr();
            attr.setFK_MapData(frmID);
            attr.setKeyOfEn( prx + "_Note");
            attr.setName("审核意见"); // sta;  // this.ToE("CheckNote", "审核意见");
            attr.setMyDataType(DataType.AppString);
            attr.setUIContralType(UIContralType.TB);
            attr.setUIIsEnable(true);
            attr.setUIIsLine(true);
            attr.setMaxLen(4000);
            attr.SetValByKey(MapAttrAttr.ColSpan, 4);
            // attr.ColSpan = 4;
            attr.setGroupID(gf.OID);
            attr.setUIHeight( 23 * 3);
            attr.setIdx(1);
            attr.Insert();
            attr.Update("Idx", 1);


            attr = new MapAttr();
            attr.setFK_MapData(frmID);
            attr.setKeyOfEn(prx + "_Checker");
            attr.setName("审核人");// "审核人";
            attr.setMyDataType(DataType.AppString);
            attr.setUIContralType(UIContralType.TB);
            attr.setMaxLen(50);
            attr.setMinLen(0);
            attr.setUIIsEnable(true);
            attr.setUIIsLine(false);
            attr.setDefVal("@WebUser.No");
            attr.setUIIsEnable(false);
            attr.setGroupID(gf.OID);
            attr.IsSigan=true;
            attr.Idx = 2;
            attr.Insert();
            attr.Update("Idx", 2);

            attr = new MapAttr();
            attr.setFK_MapData(frmID);
            attr.setKeyOfEn(prx + "_RDT");
            attr.setName("审核日期"); // "审核日期";
            attr.setMyDataType(DataType.AppDateTime);
            attr.setUIContralType(UIContralType.TB);
            attr.setUIIsEnable(true);
            attr.setUIIsLine(false);
            attr.setDefVal("@RDT");
            attr.setUIIsEnable(false);
            attr.setGroupID(gf.OID);
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
        public static void CreateFrm(string frmID, string frmName, string frmTreeID, FrmType frmType = FrmType.FoolForm)
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
        /// 一键设置元素只读
        /// </summary>
        /// <param name="frmID">要设置的表单.</param>
        public static void OneKeySetFrmEleReadonly(string frmID)
        {
            string sql = "UPDATE Sys_MapAttr SET UIIsEnable=0 WHERE FK_MapData='" + frmID + "'";
            DBAccess.RunSQL(sql);

            MapDtls dtls = new MapDtls(frmID);
            foreach (MapDtl dtl in dtls)
            {
                dtl.IsInsert = false;
                dtl.IsUpdate = false;
                dtl.IsDelete = false;
                dtl.Update();

                //sql = "UPDATE Sys_MapAttr SET UIIsEnable=0 WHERE FK_MapData='" + dtl.No + "'";
                //DBAccess.RunSQL(sql);
            }

            FrmAttachments ens = new FrmAttachments(frmID);
            foreach (FrmAttachment en in ens)
            {
                en.IsUpload = false;
                en.DeleteWay = 0;
                en.Update();

                //sql = "UPDATE Sys_MapAttr SET UIIsEnable=0 WHERE FK_MapData='" + dtl.No + "'";
                //DBAccess.RunSQL(sql);
            }

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
                attr.SetValByKey(MapAttrAttr.FK_MapData,   frmID);
                attr.SetValByKey(MapAttrAttr.KeyOfEn , "OID");
                attr.SetValByKey(MapAttrAttr.Name ,"主键");
                attr.SetValByKey(MapAttrAttr.MyDataType , DataType.AppInt);
                attr.SetValByKey(MapAttrAttr.UIContralType,(int)UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.SetValByKey(MapAttrAttr.DefVal ,"0");
                attr.SetValByKey(MapAttrAttr.EditType, (int)EditType.Readonly);
                 
                attr.Insert();
            }
            if (attr.IsExit(MapAttrAttr.KeyOfEn, "AtPara", MapAttrAttr.FK_MapData, frmID) == false)
            {
                attr.setFK_MapData(frmID);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("AtPara");
                attr.setName("参数"); // 单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(4000);
                attr.Idx = -100;
                attr.Insert();
            }

        }
        
         
        /// <summary>
        /// 复制表单
        /// </summary>
        /// <param name="copyToFrmID">源表单ID</param>
        /// <param name="copyFrmID">copy到表单ID</param>
        /// <param name="copyFrmName">新实体表单名称</param>
        public static string CopyFrm(string srcFrmID, string copyToFrmID, string copyFrmName, string fk_frmTree)
        {
            MapData mymd = new MapData();
            mymd.No = copyToFrmID;
            if (mymd.RetrieveFromDBSources() == 1)
                throw new Exception("@目标表单ID:" + copyToFrmID + "已经存在，位于:" + mymd.FK_FormTreeText + "目录下.");

            //获得源文件信息.
            DataSet ds = GenerHisDataSet_AllEleInfo(srcFrmID);

            //导入表单文件.
            ImpFrmTemplate(copyToFrmID, ds, false);

            //复制模版文件.
            MapData mdCopyTo = new MapData(copyToFrmID);

            if (mdCopyTo.HisFrmType == FrmType.ExcelFrm)
            {
                /*如果是excel表单，那就需要复制excel文件.*/
                string srcFile =  BP.Difference.SystemConfig.PathOfDataUser + "FrmVSTOTemplate/" + srcFrmID + ".xls";
                string toFile =  BP.Difference.SystemConfig.PathOfDataUser + "FrmVSTOTemplate/" + copyToFrmID + ".xls";
                if (System.IO.File.Exists(srcFile) == true)
                {
                    if (System.IO.File.Exists(toFile) == false)
                        System.IO.File.Copy(srcFile, toFile, false);
                }

                srcFile =  BP.Difference.SystemConfig.PathOfDataUser + "FrmVSTOTemplate/" + srcFrmID + ".xlsx";
                toFile =  BP.Difference.SystemConfig.PathOfDataUser + "FrmVSTOTemplate/" + copyToFrmID + ".xlsx";
                if (System.IO.File.Exists(srcFile) == true)
                {
                    if (System.IO.File.Exists(toFile) == false)
                        System.IO.File.Copy(srcFile, toFile, false);
                }
            }

            mdCopyTo.Retrieve();

            mdCopyTo.FK_FormTree = fk_frmTree;
            //  md.FK_FrmSort = fk_frmTree;
            mdCopyTo.Name = copyFrmName;
            mdCopyTo.Update();

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
        /// 修改frm的事件
        /// </summary>
        /// <param name="frmID"></param>
        public static void AfterFrmEditAction(string frmID)
        {
            //清除缓存.
            CashFrmTemplate.Remove(frmID);
            Cash.SetMap(frmID, null);

            MapData mapdata = new MapData();
            mapdata.No = frmID;
            mapdata.RetrieveFromDBSources();
            Cash2019.UpdateRow(mapdata.ToString(), frmID, mapdata.Row);
            mapdata.CleanObject();
            return;
        }
        /// <summary>
        /// 获得表单信息.
        /// </summary>
        /// <param name="frmID">表单</param>
        /// <returns></returns>
        public static DataSet GenerHisDataSet(string frmID, string frmName = null, MapData md = null)
        {
            //首先从缓存获取数据.
            DataSet dsFrm = CashFrmTemplate.GetFrmDataSetModel(frmID);
            if (dsFrm != null)
                return dsFrm;

            DataSet ds = new DataSet();

            //创建实体对象.
            if (md == null)
                md = new MapData(frmID);

            if (DataType.IsNullOrEmpty(md.Name) == true && frmName != null)
                md.Name = frmName;

            //加入主表信息.
            DataTable Sys_MapData = md.ToDataTableField("Sys_MapData");
            ds.Tables.Add(Sys_MapData);

            //加入分组表.
            DataTable Sys_GroupField = md.GroupFields.ToDataTableField("Sys_GroupField");
            ds.Tables.Add(Sys_GroupField);

            //加入明细表.
            DataTable Sys_MapDtl = md.OrigMapDtls.ToDataTableField("Sys_MapDtl");
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

            //Sys_FrmRB.
            DataTable Sys_FrmRB = md.FrmRBs.ToDataTableField("Sys_FrmRB");
            ds.Tables.Add(Sys_FrmRB);
             

            //img.
            DataTable Sys_FrmImg = md.FrmImgs.ToDataTableField("Sys_FrmImg");
            ds.Tables.Add(Sys_FrmImg);

            //Sys_MapFrame.
            DataTable Sys_MapFrame = md.MapFrames.ToDataTableField("Sys_MapFrame");
            ds.Tables.Add(Sys_MapFrame);

            //Sys_FrmAttachment.
            DataTable Sys_FrmAttachment = md.FrmAttachments.ToDataTableField("Sys_FrmAttachment");
            ds.Tables.Add(Sys_FrmAttachment);

            //FrmImgAths. 上传图片附件.
            DataTable Sys_FrmImgAth = md.FrmImgAths.ToDataTableField("Sys_FrmImgAth");
            ds.Tables.Add(Sys_FrmImgAth);

            //放入缓存.
            CashFrmTemplate.Put(frmID, ds);

            return ds;
        }
        /// <summary>
        /// 获得表单字段信息字段.
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <returns></returns>
        public static DataSet GenerHisDataSet_AllEleInfo(string fk_mapdata)
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

            //如果是开发者表单, 就把html信息放入到字段.
            if (md.HisFrmType == FrmType.Develop)
            {
                Sys_MapData.Columns.Add("HtmlTemplateFile", typeof(string));
                string text = DBAccess.GetBigTextFromDB("Sys_MapData", "No", md.No, "HtmlTemplateFile");
                Sys_MapData.Rows[0]["HtmlTemplateFile"] = text;
            }


            ds.Tables.Add(Sys_MapData);

            //加入分组表.
            GroupFields gfs = new GroupFields();
            gfs.RetrieveIn(GroupFieldAttr.FrmID, frmIDs);
            DataTable Sys_GroupField = gfs.ToDataTableField("Sys_GroupField");
            ds.Tables.Add(Sys_GroupField);

            //加入明细表.
            DataTable Sys_MapDtl = md.OrigMapDtls.ToDataTableField("Sys_MapDtl");
            ds.Tables.Add(Sys_MapDtl);

            //加入枚举表.
            SysEnums ses = new SysEnums();
            ses.RetrieveInSQL(SysEnumAttr.EnumKey, "SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + frmIDs + ") ");
            DataTable Sys_Menu = ses.ToDataTableField("Sys_Enum");
            ds.Tables.Add(Sys_Menu);

            //加入字段属性.
            MapAttrs attrs = new MapAttrs();
            attrs.RetrieveIn(MapAttrAttr.FK_MapData, frmIDs);
            DataTable Sys_MapAttr = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(Sys_MapAttr);

            //加入扩展属性.
            MapExts exts = new MapExts();
            exts.RetrieveIn(MapAttrAttr.FK_MapData, frmIDs);
            DataTable Sys_MapExt = exts.ToDataTableField("Sys_MapExt");
            ds.Tables.Add(Sys_MapExt);
 
            //img.
            //Sys_FrmLab.
            FrmImgs frmImgs = new FrmImgs();
            frmImgs.RetrieveIn(MapAttrAttr.FK_MapData, frmIDs);
            ds.Tables.Add(frmImgs.ToDataTableField("Sys_FrmImg"));

            //Sys_FrmRB.
            DataTable Sys_FrmRB = md.FrmRBs.ToDataTableField("Sys_FrmRB");
            ds.Tables.Add(Sys_FrmRB);


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
        public static DataSet GenerHisDataSet_AllEleInfo2017(string fk_mapdata, bool isCheckFrmType = false)
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
          //  listNames.Add("Sys_FrmLine");
         //   sql = "@SELECT * FROM Sys_FrmLine WHERE " + where;
           // sqls += sql;

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
                    DataTable dt = DBAccess.RunSQLReturnTable(s);
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

        #region 模版操作 2020. 
        /// <summary>
        /// A:从一个表单导入另外一个表单模版:
        /// 1.向已经存在的表单上导入模版.
        /// 2.用于节点表单的导入,设计表单的时候，新建一个表单后在导入的情况.
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="specImpFrmID"></param>
        /// <returns></returns>
        public static MapData Template_ImpFromSpecFrmID(string frmID, string specImpFrmID)
        {
            return null;
        }

        /// <summary>
        /// B:复制表单模版到指定的表单ID.
        /// 用于复制一个表单，到另外一个表单ID上去.用于表单树的上的表单Copy.
        /// </summary>
        /// <param name="fromFrmID">要copy的表单ID</param>
        /// <param name="copyToFrmID">copy到的表单ID</param>
        /// <param name="copyToFrmName">表单名称</param>
        /// <returns></returns>
        public static MapData Template_CopyFrmToFrmIDAsNewFrm(string fromFrmID, string copyToFrmID, string copyToFrmName)
        {

            return null;
        }
        /// <summary>
        /// C:导入模版xml文件..
        /// 导入一个已经存在的表单,如果这个表单ID已经存在就提示错误..
        /// </summary>
        /// <param name="">表单元素</param>
        /// <param name="">表单类别</param>
        /// <returns></returns>
        public static MapData Template_LoadXmlTemplateAsNewFrm(DataSet ds, string frmSort)
        {
            MapData md = MapData.ImpMapData(ds);
            md.OrgNo = DBAccess.RunSQLReturnString("SELECT OrgNo FROM sys_formtree WHERE NO='" + frmSort + "'");
            md.FK_FormTree = frmSort;
            md.Update();
            return md;
        }
        public static MapData Template_LoadXmlTemplateAsSpecFrmID(string newFrmID, DataSet ds, string frmSort)
        {
            MapData md = MapData.ImpMapData(newFrmID, ds);
            md.OrgNo = DBAccess.RunSQLReturnString("SELECT OrgNo FROM sys_formtree WHERE NO='" + frmSort + "'");
            md.FK_FormTree = frmSort;
            md.Update();
            return md;
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
                se.setMyPK(se.EnumKey + "_" + se.Lang + "_" + se.IntKey);
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
            if (DataType.IsNullOrEmpty(name) == true)
                return "";

            string s = string.Empty;
            try
            {
                if (isQuanPin == true)
                {
                    s = DataType.ParseStringToPinyin(name);
                    if (s.Length > 15)
                        s = DataType.ParseStringToPinyinJianXie(name);
                }
                else
                {
                    s = DataType.ParseStringToPinyinJianXie(name);
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
            if (DataType.IsNullOrEmpty(name) == true)
                return "";

            string s = string.Empty;

            if (removeSpecialSymbols)
                name = DataType.ParseStringForName(name, maxLen);

            //单.
            name = name.Replace("单", "Dan");

            try
            {
                if (isQuanPin == true)
                    s = DataType.ParseStringToPinyin(name);
                else
                    s = DataType.ParseStringToPinyinJianXie(name);

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
