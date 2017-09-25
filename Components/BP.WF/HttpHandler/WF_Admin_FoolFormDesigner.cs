using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Services.Description;
using System.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Tools;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 处理页面的业务逻辑
    /// </summary>
    public class WF_Admin_FoolFormDesigner : DirectoryPageBase
    {
        #region 表单设计器.
        /// <summary>
        /// 是不是第一次进来.
        /// </summary>
        public bool IsFirst
        {
            get
            {
                if (this.GetRequestVal("IsFirst") == null || this.GetRequestVal("IsFirst") == "" || this.GetRequestVal("IsFirst") == "null")
                    return false;
                return true;
            }
        }
        /// <summary>
        ///  设计器初始化.
        /// </summary>
        /// <returns></returns>
        public string Designer_Init()
        {
            DataSet ds = new DataSet();
            //如果是第一次进入，就执行旧版本的升级检查.
            if (this.IsFirst == true)
            {
                MapFrmFool cols = new MapFrmFool(this.FK_MapData);
                cols.DoCheckFixFrmForUpdateVer();
                return "url@Designer.htm?FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            // 字段属性.
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            foreach (MapAttr item in attrs)
                item.DefVal = item.DefValReal;

            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            GroupFields gfs = new GroupFields(this.FK_MapData);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupField"));

            MapDtls dtls = new MapDtls(this.FK_MapData);
            ds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));

            MapFrames frms = new MapFrames(this.FK_MapData);
            ds.Tables.Add(frms.ToDataTableField("Sys_MapFrame"));

            //把表单属性放入里面去.
            MapData md = new MapData(this.FK_MapData);
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));


            //附件表.
            FrmAttachments aths = new FrmAttachments(this.FK_MapData);
            ds.Tables.Add(aths.ToDataTableField("Sys_FrmAttachment"));

            if (this.FK_MapData.IndexOf("ND") == 0)
            {
                string nodeStr = this.FK_MapData.Replace("ND", "");

                if (DataType.IsNumStr(nodeStr) == true)
                {
                    FrmNodeComponent fnc = new FrmNodeComponent(int.Parse(nodeStr));
                    //   var f = fnc.GetValFloatByKey("FWC_H");
                    ds.Tables.Add(aths.ToDataTableField("WF_Node"));
                }
            }


            //把dataet转化成json 对象.
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string MapDefDtlFreeFrm_Init()
        {
            MapDtl dtl = new MapDtl();

            //如果传递来了节点信息, 就是说明了独立表单的节点方案处理, 现在就要做如下判断
            if(this.FK_Node != 0)
            {
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;

                if (dtl.RetrieveFromDBSources() == 0)
                {
                    // 开始复制它的属性.
                    MapAttrs attrs = new MapAttrs(this.FK_MapDtl);
                    MapDtl odtl = new Sys.MapDtl(this.FK_MapDtl);
                    //存储表要与原明细表一致
                    if (string.IsNullOrWhiteSpace(odtl.PTable))
                        dtl.PTable = odtl.No;
                    else
                        dtl.PTable = odtl.PTable;

                    //让其直接保存.
                    dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                    dtl.FK_MapData = "Temp";
                    dtl.DirectInsert(); //生成一个明细表属性的主表.

                    //字段的分组也要一同复制
                    Dictionary<int, int> groupids = new Dictionary<int, int>();

                    //循环保存字段.
                    int idx = 0;
                    foreach (MapAttr item in attrs)
                    {
                        if(item.GroupID != 0)
                        {
                            if (groupids.ContainsKey(item.GroupID))
                            {
                                item.GroupID = groupids[item.GroupID];
                            }
                            else
                            {
                                GroupField gf = new Sys.GroupField();
                                gf.OID = item.GroupID;

                                if (gf.RetrieveFromDBSources() == 0)
                                {
                                    gf.Lab = "默认分组";
                                }

                                gf.EnName = dtl.No;
                                gf.InsertAsNew();

                                if (groupids.ContainsKey(item.GroupID) == false)
                                    groupids.Add(item.GroupID, gf.OID);

                                item.GroupID = gf.OID;
                            }
                        }

                        item.FK_MapData = this.FK_MapDtl + "_" + this.FK_Node;
                        item.MyPK = item.FK_MapData + "_" + item.KeyOfEn;
                        item.Save();
                        idx++;
                        item.Idx = idx;
                        item.DirectUpdate();
                    }

                    MapData md = new MapData();
                    md.No = "Temp";
                    if (md.IsExits == false)
                    {
                        md.Name = "为权限方案设置的临时的数据";
                        md.Insert();
                    }
                }

                return "sln@" + dtl.No;
            }

            dtl.No = this.FK_MapDtl;
            if (dtl.RetrieveFromDBSources() == 0)
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, dtl.Name, 100, 200);
            else
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, this.FK_MapDtl, dtl.X, dtl.Y);

            return "创建成功.";
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_FoolFormDesigner(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 执行默认的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string msg = "";

            //通用局部变量定义
            string resultString = string.Empty;
            
            switch (this.DoType)
            {
                case "ParseStringToPinyin": //转拼音方法.
                    string name = GetRequestVal("name");
                    string flag = GetRequestVal("flag");
                    //此处为字段中文转拼音，设置为最大20个字符，edited by liuxc,2017-9-25
                    return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(flag, "true"), true, 20);
                case "DtlFieldUp": //字段上移
                    MapAttr attrU = new MapAttr(this.MyPK);
                    attrU.DoUpForMapDtl();
                    msg = "";
                    break;
                case "DtlFieldDown": //字段下移.
                    MapAttr attrD = new MapAttr(this.MyPK);
                    attrD.DoDownForMapDtl();
                    msg = "";
                    break;
                case "HidAttr": //获得隐藏的字段.
                    MapAttrs attrs = new MapAttrs();
                    attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData,
                        MapAttrAttr.UIVisible, 0);
                    msg = attrs.ToJson();
                    break;
                case "Up": //移动位置..
                    MapAttr attr = new MapAttr(this.MyPK);
                    attr.DoUp();
                    break;
                case "Down": //移动位置.
                    MapAttr attrDown = new MapAttr(this.MyPK);
                    attrDown.DoDown();
                    break;
                case "GFDoUp":
                    GroupField gf = new GroupField(this.RefOID);
                    gf.DoUp();
                    gf.Retrieve();
                    if (gf.Idx == 0)
                        return "";

                    int oidIdx = gf.Idx;
                    gf.Idx = gf.Idx - 1;
                    GroupField gfUp = new GroupField();
                    if (gfUp.Retrieve(GroupFieldAttr.EnName, gf.EnName, GroupFieldAttr.Idx, gf.Idx) == 1)
                    {
                        gfUp.Idx = oidIdx;
                        gfUp.Update();
                    }
                    gf.Update();
                    break;
                case "GFDoDown":
                    GroupField mygf = new GroupField(this.RefOID);
                    mygf.DoDown();
                    mygf.Retrieve();
                    int oidIdx1 = mygf.Idx;
                    mygf.Idx = mygf.Idx + 1;
                    GroupField gfDown = new GroupField();
                    if (gfDown.Retrieve(GroupFieldAttr.EnName, mygf.EnName, GroupFieldAttr.Idx, mygf.Idx) == 1)
                    {
                        gfDown.Idx = oidIdx1;
                        gfDown.Update();
                    }
                    mygf.Update();
                    break;
                case "M2MDoUp":
                    MapM2M ddtl1 = new MapM2M(this.MyPK);
                    if (ddtl1.RowIdx > 0)
                    {
                        ddtl1.RowIdx = ddtl1.RowIdx - 1;
                        ddtl1.Update();
                    }
                    break;
                case "M2MDoDown":
                    MapM2M ddtl2 = new MapM2M(this.MyPK);
                    if (ddtl2.RowIdx < 10)
                    {
                        ddtl2.RowIdx = ddtl2.RowIdx + 1;
                        ddtl2.Update();
                    }
                    break;
                case "FrameDoUp":
                    //MapFrame frame1 = new MapFrame(this.MyPK);
                    //if (frame1.RowIdx > 0)
                    //{
                    //    frame1.RowIdx = frame1.RowIdx - 1;
                    //    frame1.Update();
                    //}
                    break;
                case "FrameDoDown":
                    //MapFrame frame2 = new MapFrame(this.MyPK);
                    //if (frame2.RowIdx < 10)
                    //{
                    //    frame2.RowIdx = frame2.RowIdx + 1;
                    //    frame2.Update();
                    //}
                    break;
                default:
                    throw new Exception("没有判断的执行类型：" + this.DoType);
                    break;
            }
            return msg;
        }

        public string SFList_SaveSFField()
        {
            MapAttr attr = new Sys.MapAttr();
            attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            if (attr.RetrieveFromDBSources() != 0)
                return "err@字段名[" + this.KeyOfEn + "]已经存在.";

            attr.FK_MapData = this.FK_MapData;
            attr.KeyOfEn = this.KeyOfEn;

            //设置string类型.
            attr.MyDataType = DataType.AppString;

            //关键字.
            attr.UIBindKey = this.GetRequestVal("SFTable");

            //分组ID.
            attr.GroupID = this.GetRequestValInt("GroupID");
            attr.UIContralType = En.UIContralType.DDL;

            //外键.
            attr.LGType = En.FieldTypeS.FK;

            SFTable sf = new Sys.SFTable();
            sf.No = attr.UIBindKey;
            if (sf.RetrieveFromDBSources() != 0)
                attr.Name = sf.Name;

            attr.Insert();
            return attr.MyPK;
        }
        /// <summary>
        /// 增加一个枚举类型
        /// </summary>
        /// <returns></returns>
        public string SysEnumList_SaveEnumField()
        {
            MapAttr attr = new Sys.MapAttr();
            attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            if (attr.RetrieveFromDBSources() != 0)
                return "err@字段名[" + this.KeyOfEn + "]已经存在.";

            attr.FK_MapData = this.FK_MapData;
            attr.KeyOfEn = this.KeyOfEn;
            attr.UIBindKey = this.GetRequestVal("EnumKey");

            attr.GroupID = this.GetRequestValInt("GroupFeid");

            attr.UIContralType = En.UIContralType.DDL;

            attr.MyDataType = DataType.AppInt;
            attr.LGType = En.FieldTypeS.Enum;

            SysEnumMain sem = new Sys.SysEnumMain();
            sem.No = attr.UIBindKey;
            if (sem.RetrieveFromDBSources() != 0)
                attr.Name = sem.Name;
            else
                attr.Name = "枚举"+attr.UIBindKey;

            attr.Insert();

            return attr.MyPK;
        }

        public string Designer_NewMapDtl()
        {
            MapDtl en = new MapDtl();
            en.FK_MapData = this.FK_MapData;
            en.No = this.GetRequestVal("DtlNo");

            if (en.RetrieveFromDBSources() == 1)
            {
                return "err@从表ID:" + en.No + "已经存在.";
            }
            else
            {
                en.Name = "我的从表" + en.No;
                en.PTable = en.No;
                en.Insert();
                en.IntMapAttrs();
            }

            //返回字串.
            return en.No;
        }
        /// <summary>
        /// 新建框架
        /// </summary>
        /// <returns></returns>
        public string Designer_NewFrame()
        {
            MapFrame frm = new MapFrame();
            frm.FK_MapData = this.FK_MapData;
            frm.MyPK = frm.FK_MapData + "_" + this.GetRequestVal("FrameNo");
            if (frm.RetrieveFromDBSources() == 1)
                return "err@框架ID:" + this.GetRequestVal("FrameNo") + "已经存在.";
            else
            {
                frm.URL = "http://ccport.org/About.aspx";
                frm.Name = "我的框架" + this.GetRequestVal("FrameNo");
                frm.Insert();
            }

            //BP.Sys.CCFormAPI.CreateOrSaveAthMulti(this.FK_MapData, this.GetRequestVal("FrameNo"), "我的附件", 100, 200);
            return frm.MyPK;
        }
        /// <summary>
        /// 创建一个多附件
        /// </summary>
        /// <returns></returns>
        public string Designer_AthNew()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = this.FK_MapData;
            ath.NoOfObj = this.GetRequestVal("AthNo");
            ath.MyPK = ath.FK_MapData + "_" + ath.NoOfObj;
            if (ath.RetrieveFromDBSources() == 1)
                return "err@附件ID:" + ath.NoOfObj + "已经存在.";
            BP.Sys.CCFormAPI.CreateOrSaveAthMulti(this.FK_MapData, this.GetRequestVal("AthNo"), "我的附件", 100, 200);
            return ath.MyPK;
        }
        /// <summary>
        /// 返回信息.
        /// </summary>
        /// <returns></returns>
        public string GroupField_Init()
        {
            GroupField gf = new GroupField();
            gf.OID = this.GetRequestValInt("GroupField");
            if (gf.OID != 0)
                gf.Retrieve();

            return gf.ToJson();
        }

        /// <summary>
        /// 保存空白的分组.
        /// </summary>
        /// <returns></returns>
        public string GroupField_SaveBlank()
        {
            string no = this.GetValFromFrmByKey("TB_Blank_No");
            string name = this.GetValFromFrmByKey("TB_Blank_Name");

            GroupField gf = new GroupField();
            gf.OID = this.GetRequestValInt("GroupField");
            if (gf.OID != 0)
                gf.Retrieve();

            gf.CtrlID = no;
            gf.EnName = this.FK_MapData;
            gf.Lab = name;
            gf.Save();
            return "保存成功.";
        }

        /// <summary>
        /// 审核分组保存
        /// </summary>
        /// <returns></returns>
        public string CheckGroup_Save()
        {
            string sta = this.GetValFromFrmByKey("TB_Check_Name");
            if (sta.Length == 0)
            {
                return "审核岗位不能为空";
            }

            string prx = this.GetValFromFrmByKey("TB_Check_No");
            if (prx.Length == 0)
            {
                prx = chs2py.convert(sta);
            }

            MapAttr attr = new MapAttr();
            int i = attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Note");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Checker");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_RDT");

            if (i > 0)
            {
                return "前缀已经使用：" + prx + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";
            }
            return "保存成功";
        }
        /// <summary>
        /// 保存分组
        /// </summary>
        /// <returns></returns>
        public string GroupField_SaveCheck()
        {
            string lab = this.GetValFromFrmByKey("TB_Check_Name");
            string prx = this.GetValFromFrmByKey("TB_Check_No");
            BP.Sys.CCFormAPI.CreateCheckGroup(this.FK_MapData, lab, prx);
            return "创建成功...";
        }
        /// <summary>
        /// 
        /// 删除分组
        /// </summary>
        /// <returns></returns>
        public string GroupField_DeleteCheck()
        {
            GroupField gf = new GroupField();
            gf.OID = this.GetRequestValInt("GroupField");
            gf.Delete();

            BP.WF.Template.MapFrmFool md = new BP.WF.Template.MapFrmFool(this.FK_MapData);
            md.DoCheckFixFrmForUpdateVer();

            return "删除成功...";
        }

        /// <summary>
        /// 
        /// 删除并删除该分组下的字段
        /// </summary>
        /// <returns></returns>
        public string GroupField_DeleteAllCheck()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.GroupID, this.GetRequestValInt("GroupField"));
            foreach (MapAttr attr in attrs)
            {
                if (attr.HisEditType != EditType.Edit)
                    continue;
                if (attr.KeyOfEn == "FID")
                    continue;

                attr.Delete();
            }

            GroupField gf = new GroupField();
            gf.OID = this.GetRequestValInt("GroupField");
            gf.Delete();

            return "删除并删除该分组下的字段成功...";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string EditFExtContral_Init()
        {
            ExtContral en = new ExtContral();
            en.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            en.RetrieveFromDBSources();
            return en.ToJson();

        }
        public string EditFExtContral_Save()
        {
            ExtContral en = new ExtContral();
            en.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            en.RetrieveFromDBSources();

            en.UIContralType = (UIContralType)int.Parse(this.GetValFromFrmByKey("Model"));

            switch (en.UIContralType)
            {
                case UIContralType.AthShow:
                    en.AthRefObj = this.GetValFromFrmByKey("DDL_Ath");
                    en.AthShowModel = (AthShowModel)int.Parse(this.GetValFromFrmByKey("DDL_AthShowModel"));

                    //让附件不可见.
                    FrmAttachment ath = new FrmAttachment(en.AthRefObj);
                    ath.IsVisable = false;
                    ath.Update();
                    BP.DA.DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE EnName='" + this.FK_MapData + "' AND CtrlID='" + en.AthRefObj + "'");

                    FrmAttachments aths = new FrmAttachments(this.FK_MapData);
                    foreach (FrmAttachment item in aths)
                    {
                        string sql = "SELECT count(*) FROM Sys_MapAttr WHERE AtPara LIKE '%" + item.MyPK + "@%' AND FK_MapData='" + this.FK_MapData + "'";
                        int num = DBAccess.RunSQLReturnValInt(sql);
                        if (num == 0)
                        {
                            // 没有被引用.
                            item.IsVisable = true;
                            item.Update();
                        }
                    }
                    break;
                default:
                    break;
            }

            en.Update();

            return "保存成功.";
        }
        /// <summary>
        /// 框架信息.
        /// </summary>
        /// <returns></returns>
        public string MapFrame_Init()
        {
            MapFrame mf = new MapFrame();
            mf.FK_MapData = this.FK_MapData;

            if (this.MyPK == null)
            {
                mf.URL = "http://ccflow.org";
                mf.W = "100%";
                mf.H = "300";
                mf.Name = "我的框架.";
                mf.FK_MapData = this.FK_MapData;
                mf.MyPK = BP.DA.DBAccess.GenerGUID();
            }
            else
            {
                mf.MyPK = this.MyPK;
                mf.RetrieveFromDBSources();
            }
            return mf.ToJson();
        }
        /// <summary>
        /// 框架信息保存.
        /// </summary>
        /// <returns></returns>
        public string MapFrame_Save()
        {
            MapFrame mf = new MapFrame();
            mf = BP.Sys.PubClass.CopyFromRequestByPost(mf, context.Request) as MapFrame;
            mf.FK_MapData = this.FK_MapData;

            mf.Save(); //执行保存.
            return "保存成功..";
        }
        /// <summary>
        /// 框架信息删除.
        /// </summary>
        /// <returns></returns>
        public string MapFrame_Delete()
        {
            MapFrame dtl = new MapFrame();
            dtl.MyPK = this.MyPK;
            dtl.Delete();
            return "操作成功..." + this.MyPK;
        }
        /// <summary>
        /// 枚举值列表
        /// </summary>
        /// <returns></returns>
        public string EnumList()
        {
            SysEnumMains ses = new SysEnumMains();
            ses.RetrieveAll();
            return ses.ToJson();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string SFTableDelete()
        {
            SFTable sf = new SFTable(this.FK_SFTable);
            sf.Delete();
            return "删除成功...";
        }
        /// <summary>
        /// 字典表列表.
        /// </summary>
        /// <returns></returns>
        public string SFTableList()
        {
            SFTables ens = new SFTables();
            ens.RetrieveAll();
            return ens.ToJson();
        }
        /// <summary>
        /// 初始化表.
        /// </summary>
        /// <returns></returns>
        public string EditTableField_Init()
        {
            MapAttr attr = new MapAttr();
            attr.KeyOfEn = this.KeyOfEn;
            attr.FK_MapData = this.FK_MapData;

            if (string.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.MyPK = this.MyPK;
                attr.RetrieveFromDBSources();
            }
            else
            {
                SFTable sf = new SFTable(this.FK_SFTable);
                attr.Name = sf.Name;
                attr.KeyOfEn = sf.No;
            }

            //第1次加载.
            attr.UIContralType = UIContralType.DDL;

            attr.FK_MapData = this.FK_MapData;

            //字体大小.
            int size = attr.Para_FontSize;
            if (size == 0)
                attr.Para_FontSize = 12;

            //横跨的列数.
            if (attr.ColSpan == 0)
                attr.ColSpan = 1;

            return attr.ToJson();
        }
        /// <summary>
        /// 字段选择.
        /// </summary>
        /// <returns></returns>
        public string FieldTypeSelect_Create()
        {
            string no = this.GetRequestVal("KeyOfEn");
            string name = this.GetRequestVal("Name");
            string newNo = DataType.ParseStringForNo(no, 20);
            string newName = DataType.ParseStringForName(name, 20);
            int fType = int.Parse(this.context.Request.QueryString["FType"]);

            MapAttrs attrs = new MapAttrs();
            int i = attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, newNo);
            if (i != 0)
                return "err@字段名：" + newNo + "已经存在.";

            //求出选择的字段类型.
            MapAttr attr = new MapAttr();
            attr.Name = newName;
            attr.KeyOfEn = newNo;
            attr.FK_MapData = this.FK_MapData;
            attr.LGType = FieldTypeS.Normal;
            attr.MyPK = this.FK_MapData + "_" + newNo;
            attr.GroupID = this.GroupField;
            attr.MyDataType = fType;

            int colspan = attr.ColSpan;
            attr.Para_FontSize = 12;
            int rows = attr.UIRows;

            if (attr.MyDataType == DataType.AppString)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.Insert();
                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrStrings&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppInt)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrNums&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;

                // return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppMoney)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppMoney;
                attr.UIContralType = UIContralType.TB;
                attr.DefVal = "0.00";
                attr.Insert();
                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrNums&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                //return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppFloat)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppFloat;
                attr.UIContralType = UIContralType.TB;

                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrNums&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                //return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDouble)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppDouble;
                attr.UIContralType = UIContralType.TB;
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrNums&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                //return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDate)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.UIContralType = UIContralType.TB;
                attr.MyDataType = DataType.AppDate;
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrDTs&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                //return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDate + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDateTime)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.UIContralType = UIContralType.TB;
                attr.MyDataType = DataType.AppDateTime;
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrDTs&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                //return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDateTime + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppBoolean)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 1;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.UIContralType = UIContralType.CheckBok;
                attr.MyDataType = DataType.AppBoolean;
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrBoolens&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
                // return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppBoolean + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            return "err@没有判断的数据类型." + attr.MyDataTypeStr;
        }
        /// <summary>
        /// 字段初始化数据.
        /// </summary>
        /// <returns></returns>
        public string EditF_FieldInit()
        {
            MapAttr attr = new MapAttr();
            attr.KeyOfEn = this.KeyOfEn;
            attr.FK_MapData = this.FK_MapData;

            if (string.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.MyPK = this.MyPK;
                attr.RetrieveFromDBSources();
            }
            else
            {
                attr.GroupID = this.GroupField;
            }

            attr.FK_MapData = this.FK_MapData;

            //字体大小.
            int size = attr.Para_FontSize;
            if (size == 0)
                attr.Para_FontSize = 12;

            string field = attr.Para_SiganField;
            bool IsEnableJS = attr.IsEnableJS;
            bool IsSupperText = attr.IsSupperText; //是否是超大文本？
            bool isBigDoc = attr.IsBigDoc;

            //横跨的列数.
            if (attr.ColSpan == 0)
                attr.ColSpan = 1;

            return attr.ToJson();
        }
        public string FieldInitEnum()
        {
            MapAttr attr = new MapAttr();
            attr.KeyOfEn = this.KeyOfEn;
            attr.FK_MapData = this.FK_MapData;

            if (string.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.MyPK = this.MyPK;
                attr.RetrieveFromDBSources();
            }
            else
            {
                SysEnumMain sem = new SysEnumMain(this.EnumKey);
                attr.Name = sem.Name;
                attr.KeyOfEn = sem.No;
                attr.DefVal = "0";
            }

            //第1次加载.
            if (attr.UIContralType == UIContralType.TB)
                attr.UIContralType = UIContralType.DDL;

            attr.FK_MapData = this.FK_MapData;

            //字体大小.
            int size = attr.Para_FontSize;
            if (size == 0)
                attr.Para_FontSize = 12;

            //横跨的列数.
            if (attr.ColSpan == 0)
                attr.ColSpan = 1;

            var model = attr.RBShowModel;
            attr.RBShowModel = model;

            return attr.ToJson();
        }
        /// <summary>
        /// 转化成json
        /// </summary>
        /// <returns></returns>
        public string FieldInitGroupID()
        {
            GroupFields gfs = new GroupFields(this.FK_MapData);

            //转化成json输出.
            return gfs.ToJson();
        }
        /// <summary>
        /// 分组&枚举： 两个数据源.
        /// </summary>
        /// <returns></returns>
        public string FieldInitGroupAndSysEnum()
        {
            GroupFields gfs = new GroupFields(this.FK_MapData);

            //分组值.
            DataSet ds = new DataSet();
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupField"));

            //枚举值.
            string enumKey = this.EnumKey;
            if (enumKey == "" || enumKey == null || enumKey == "null")
            {
                MapAttr ma = new MapAttr(this.MyPK);
                enumKey = ma.UIBindKey;
            }

            SysEnums enums = new SysEnums(enumKey);
            ds.Tables.Add(enums.ToDataTableField("Sys_Enum"));

            //转化成json输出.
            string json = BP.Tools.Json.ToJson(ds);
            // BP.DA.DataType.WriteFile("c:\\FieldInitGroupAndSysEnum.json", json);
            return json;
        }

        /// <summary>
        /// 执行删除.
        /// </summary>
        /// <returns></returns>
        public string FieldDelete()
        {
            try
            {
                MapAttr attr = new MapAttr();
                attr.MyPK = this.MyPK;
                attr.RetrieveFromDBSources();
                attr.Delete();
                return "删除成功...";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存枚举值.
        /// </summary>
        /// <returns></returns>
        public string FieldSaveEnum()
        {
            try
            {
                //定义变量.
                if (this.EnumKey == null)
                    return "err@没有接收到EnumKey的值，无法进行保存操作。";

                //赋值.
                MapAttr attr = new MapAttr();
                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                if (string.IsNullOrEmpty(this.MyPK) == false)
                {
                    attr.MyPK = this.MyPK;
                    attr.RetrieveFromDBSources();
                }
                else
                {
                    /*判断字段是否存在？*/
                    if (attr.IsExit(MapAttrAttr.KeyOfEn, this.KeyOfEn, MapAttrAttr.FK_MapData, this.FK_MapData) == true)
                        return "err@字段名:" + this.KeyOfEn + "已经存在.";
                }

                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                attr.LGType = FieldTypeS.Enum;
                attr.UIBindKey = this.EnumKey;
                attr.MyDataType = DataType.AppInt;

                //控件类型.
                attr.UIContralType = UIContralType.DDL;

                attr.Name = this.GetValFromFrmByKey("TB_Name");
                attr.KeyOfEn = this.GetValFromFrmByKey("TB_KeyOfEn");
                attr.ColSpan = this.GetValIntFromFrmByKey("DDL_ColSpan");
                if (attr.ColSpan == 0)
                    attr.ColSpan = 1;

                //显示方式.
                attr.RBShowModel = this.GetValIntFromFrmByKey("DDL_RBShowModel");

                //控件类型.
                attr.UIContralType = (UIContralType)this.GetValIntFromFrmByKey("RB_CtrlType");

                attr.UIIsInput = this.GetValBoolenFromFrmByKey("CB_IsInput");   //是否是必填项.

                attr.IsEnableJS = this.GetValBoolenFromFrmByKey("CB_IsEnableJS");   //是否启用js设置？

                attr.Para_FontSize = this.GetValIntFromFrmByKey("TB_FontSize"); //字体大小.

                //默认值.
                attr.DefVal = this.GetValFromFrmByKey("TB_DefVal");

                try
                {
                    //分组.
                    if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                        attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？
                }
                catch
                {

                }

                //是否可用？所有类型的属性，都需要。
                int isEnable = this.GetValIntFromFrmByKey("RB_UIIsEnable");
                if (isEnable == 0)
                    attr.UIIsEnable = false;
                else
                    attr.UIIsEnable = true;

                //是否可见?
                int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
                if (visable == 0)
                    attr.UIVisible = false;
                else
                    attr.UIVisible = true;

                attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;

                attr.Save();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存外键表字段.
        /// </summary>
        /// <returns></returns>
        public string EditTableField_Save()
        {
            try
            {
                //定义变量.
                if (this.FK_SFTable == null)
                    return "err@没有接收到FK_SFTable的值，无法进行保存操作。";

                //赋值.
                MapAttr attr = new MapAttr();
                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                if (string.IsNullOrEmpty(this.MyPK) == false)
                {
                    attr.MyPK = this.MyPK;
                    attr.RetrieveFromDBSources();
                }
                else
                {
                    /*判断字段是否存在？*/
                    if (attr.IsExit(MapAttrAttr.KeyOfEn, this.KeyOfEn, MapAttrAttr.FK_MapData, this.FK_MapData) == true)
                        return "err@字段名:" + this.KeyOfEn + "已经存在.";
                }

                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                attr.LGType = FieldTypeS.FK;
                attr.UIBindKey = this.FK_SFTable;
                attr.MyDataType = DataType.AppString;

                //控件类型.
                attr.UIContralType = UIContralType.DDL;

                attr.Name = this.GetValFromFrmByKey("TB_Name");
                attr.KeyOfEn = this.GetValFromFrmByKey("TB_KeyOfEn");
                attr.ColSpan = this.GetValIntFromFrmByKey("DDL_ColSpan");
                if (attr.ColSpan == 0)
                    attr.ColSpan = 1;

                attr.UIIsInput = this.GetValBoolenFromFrmByKey("CB_IsInput");   //是否是必填项.

                attr.Para_FontSize = this.GetValIntFromFrmByKey("TB_FontSize"); //字体大小.

                //默认值.
                attr.DefVal = this.GetValFromFrmByKey("TB_DefVal");

                try
                {
                    //分组.
                    if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                        attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？
                }
                catch
                {

                }

                //是否可用？所有类型的属性，都需要。
                int isEnable = this.GetValIntFromFrmByKey("RB_UIIsEnable");
                if (isEnable == 0)
                    attr.UIIsEnable = false;
                else
                    attr.UIIsEnable = true;

                //是否可见?
                int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
                if (visable == 0)
                    attr.UIVisible = false;
                else
                    attr.UIVisible = true;

                attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
                attr.Save();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行保存.
        /// </summary>
        /// <returns></returns>
        public string EditF_Save()
        {
            try
            {
                //定义变量.
                int fType = int.Parse(context.Request.QueryString["FType"]);  //字段数据物理类型
                FieldTypeS lgType = (FieldTypeS)int.Parse(context.Request.QueryString["LGType"]); //逻辑类型.
                string uiBindKey = context.Request.QueryString["UIBindKey"];

                //赋值.
                MapAttr attr = new MapAttr();
                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                attr.LGType = lgType; //逻辑类型.
                attr.UIBindKey = uiBindKey; //绑定的枚举或者外键.
                attr.MyDataType = fType; //物理类型.

                if (string.IsNullOrEmpty(this.MyPK) == false)
                {
                    attr.MyPK = this.MyPK;
                    attr.RetrieveFromDBSources();
                }

                attr.FK_MapData = this.FK_MapData;
                attr.MyDataType = fType; //数据类型.
                attr.Name = this.GetValFromFrmByKey("TB_Name");

                attr.KeyOfEn = this.GetValFromFrmByKey("TB_KeyOfEn");
                attr.ColSpan = this.GetValIntFromFrmByKey("DDL_ColSpan");

                if (attr.ColSpan == 0)
                    attr.ColSpan = 1;

                attr.Para_FontSize = this.GetValIntFromFrmByKey("TB_FontSize"); //字体大小.
                attr.Para_Tip = this.GetValFromFrmByKey("TB_Tip"); //操作提示.

                //默认值.
                attr.DefVal = this.GetValFromFrmByKey("TB_DefVal");


                //对于明细表就可能没有值.
                try
                {
                    //分组.
                    if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                        attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？
                }
                catch
                {

                }


                //把必填项拿出来，所有字段都可以设置成必填项 杨玉慧
                attr.UIIsInput = this.GetValBoolenFromFrmByKey("CB_IsInput");   //是否是必填项.

                if (attr.MyDataType == BP.DA.DataType.AppString && lgType == FieldTypeS.Normal)
                {
                    attr.IsRichText = this.GetValBoolenFromFrmByKey("CB_IsRichText"); //是否是富文本？
                    attr.IsSupperText = this.GetValBoolenFromFrmByKey("CB_IsSupperText"); //是否是超大文本？

                    //高度.
                    attr.UIHeightInt = this.GetValIntFromFrmByKey("DDL_Rows") * 23;

                    //最大最小长度.
                    attr.MaxLen = this.GetValIntFromFrmByKey("TB_MaxLen");
                    attr.MinLen = this.GetValIntFromFrmByKey("TB_MinLen");

                    attr.UIWidth = this.GetValIntFromFrmByKey("TB_UIWidth"); //宽度.
                }

                switch (attr.MyDataType)
                {
                    case DataType.AppInt:
                    case DataType.AppFloat:
                    case DataType.AppDouble:
                    case DataType.AppMoney:
                        attr.IsSum = this.GetValBoolenFromFrmByKey("CB_IsSum");
                        break;
                }

                //获取宽度.
                try
                {
                    attr.UIWidth = this.GetValIntFromFrmByKey("TB_UIWidth"); //宽度.
                }
                catch
                {
                }


                //是否可用？所有类型的属性，都需要。
                int isEnable = this.GetValIntFromFrmByKey("RB_UIIsEnable");
                if (isEnable == 0)
                    attr.UIIsEnable = false;
                else
                    attr.UIIsEnable = true;

                //仅仅对普通类型的字段需要.
                if (lgType == FieldTypeS.Normal)
                {
                    //是否可见?
                    int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
                    if (visable == 0)
                        attr.UIVisible = false;
                    else
                        attr.UIVisible = true;
                }

                attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
                attr.Save();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获得从表的列.
        /// </summary>
        /// <returns></returns>
        public string DtlAttrs()
        {
            MapAttrs attrs = new MapAttrs(this.FK_MapDtl);
            return attrs.ToJson();
        }
        /// <summary>
        /// 该方法有2处调用。
        /// 1，修改字段。
        /// 2，编辑属性。
        /// </summary>
        /// <returns></returns>
        public string DtlInit()
        {
            MapDtl dtl = new MapDtl();
            dtl.No = this.FK_MapDtl;
            if (dtl.RetrieveFromDBSources() == 0)
            {
                dtl.FK_MapData = this.FK_MapData;
                dtl.Name = this.FK_MapData;
                dtl.Insert();
                dtl.IntMapAttrs();
            }

            if (this.FK_Node != 0)
            {
                /* 如果传递来了节点信息, 就是说明了独立表单的节点方案处理, 现在就要做如下判断.
                 * 1, 如果已经有了.
                 */
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                if (dtl.RetrieveFromDBSources() == 0)
                {

                    // 开始复制它的属性.
                    MapAttrs attrs = new MapAttrs(this.FK_MapDtl);

                    //让其直接保存.
                    dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                    dtl.FK_MapData = "Temp";
                    dtl.DirectInsert(); //生成一个明细表属性的主表.

                    //循环保存字段.
                    int idx = 0;
                    foreach (MapAttr item in attrs)
                    {
                        item.FK_MapData = this.FK_MapDtl + "_" + this.FK_Node;
                        item.MyPK = item.FK_MapData + "_" + item.KeyOfEn;
                        item.Save();
                        idx++;
                        item.Idx = idx;
                        item.DirectUpdate();
                    }

                    MapData md = new MapData();
                    md.No = "Temp";
                    if (md.IsExits == false)
                    {
                        md.Name = "为权限方案设置的临时的数据";
                        md.Insert();
                    }
                }
            }

            DataSet ds = new DataSet();
            DataTable dt = dtl.ToDataTableField("Main");
            ds.Tables.Add(dt);

            //获得字段列表.
            MapAttrs attrsDtl = new MapAttrs(this.FK_MapDtl);
            DataTable dtAttrs = attrsDtl.ToDataTableField("Ens");
            ds.Tables.Add(dtAttrs);

            //返回json配置信息.
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行保存.
        /// </summary>
        /// <returns></returns>
        public string DtlSave()
        {
            try
            {
                //复制.
                MapDtl dtl = new MapDtl(this.FK_MapDtl);

                //从request对象里复制数据,到entity.
                BP.Sys.PubClass.CopyFromRequest(dtl, context.Request);

                dtl.Update();

                return "保存成功...";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 下载表单.
        /// </summary>
        public void DownTempFrm()
        {
            string fileFullName = context.Request.PhysicalApplicationPath + "\\Temp\\" + context.Request.QueryString["FK_MapData"] + ".xml";
            FileInfo fileInfo = new FileInfo(fileFullName);
            if (fileInfo.Exists)
            {
                byte[] buffer = new byte[102400];
                context.Response.Clear();
                using (FileStream iStream = File.OpenRead(fileFullName))
                {
                    long dataLengthToRead = iStream.Length; //获取下载的文件总大小.

                    context.Response.ContentType = "application/octet-stream";
                    context.Response.AddHeader("Content-Disposition", "attachment;  filename=" +
                                       HttpUtility.UrlEncode(fileInfo.Name, System.Text.Encoding.UTF8));
                    while (dataLengthToRead > 0 && context.Response.IsClientConnected)
                    {
                        int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(102400));//'读取的大小

                        context.Response.OutputStream.Write(buffer, 0, lengthRead);
                        context.Response.Flush();
                        dataLengthToRead = dataLengthToRead - lengthRead;
                    }
                    context.Response.Close();
                    context.Response.End();
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        public bool IsNodeSheet
        {
            get
            {
                if (this.FK_MapData.StartsWith("ND") == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 字段属性编辑 初始化
        /// </summary>
        /// <returns></returns>
        public string Attachment_Init()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = this.FK_MapData;
            ath.NoOfObj = this.Ath;
            ath.FK_Node = this.FK_Node;
            if (this.MyPK == null)
            {
                if (this.FK_Node == 0)
                    ath.MyPK = this.FK_MapData + "_" + this.Ath;
                else
                    ath.MyPK = this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node;
            }
            else
            {
                ath.MyPK = this.MyPK;

            }
            int i = ath.RetrieveFromDBSources();
            if (i == 0)
            {
                /*初始化默认值.*/
                ath.NoOfObj = "Ath1";
                ath.Name = "我的附件";
                ath.SaveTo = SystemConfig.PathOfDataUser + "\\UploadFile\\" + this.FK_MapData + "\\";
                ath.W = 150;
                ath.H = 40;
                ath.Exts = "*.*";
            }

            if (i == 0 && this.FK_Node != 0)
            {
                /*这里处理 独立表单解决方案, 如果有FK_Node 就说明该节点需要单独控制该附件的属性. */
                MapData mapData = new MapData();
                mapData.RetrieveByAttr(MapDataAttr.No, this.FK_MapData);
                if (mapData.AppType == "0")
                {
                    FrmAttachment souceAthMent = new FrmAttachment();
                    // 查询出来原来的数据.
                    int rowCount = souceAthMent.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData, FrmAttachmentAttr.NoOfObj, this.Ath, FrmAttachmentAttr.FK_Node, "0");
                    if (rowCount > 0)
                    {
                        ath.Copy(souceAthMent);
                    }
                }
                if (this.FK_Node == 0)
                    ath.MyPK = this.FK_MapData + "_" + this.Ath;
                else
                    ath.MyPK = this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node;

                //插入一个新的.
                ath.FK_Node = this.FK_Node;
                ath.FK_MapData = this.FK_MapData;
                ath.NoOfObj = this.Ath;
                //  ath.DirectInsert();
            }

            return ath.ToJson();
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <returns></returns>
        public string Attachment_Save()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = this.FK_MapData;
            ath.NoOfObj = this.Ath;
            ath.FK_Node = this.FK_Node;
            ath.MyPK = this.FK_MapData + "_" + this.Ath;

            int i = ath.RetrieveFromDBSources();
            ath = BP.Sys.PubClass.CopyFromRequestByPost(ath, context.Request) as FrmAttachment;
            if (i == 0)
                ath.Save(); //执行保存.              
            else
                ath.Update();
            return "保存成功..";
        }
        public string Attachment_Delete()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.MyPK = this.MyPK;
            ath.Delete();
            return "删除成功.." + ath.MyPK;
        }
    }

    public class WSMethod
    {
        public string NO { get; set; }

        public string NAME { get; set; }

        public Dictionary<string, string> PARAMS { get; set; }

        public string RETURN { get; set; }
    }
}
