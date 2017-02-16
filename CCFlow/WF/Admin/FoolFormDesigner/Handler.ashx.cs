using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using BP.Sys;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Tools;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 属性.
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string MyPK
        {
            get
            {
                string str= context.Request.QueryString["MyPK"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 字典表
        /// </summary>
        public string FK_SFTable
        {
            get
            {
                string str = context.Request.QueryString["FK_SFTable"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }
        public string EnumKey
        {
            get
            {
                string str = context.Request.QueryString["EnumKey"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
 
            }
        }
        public string KeyOfEn
        {
            get
            {
                string str = context.Request.QueryString["KeyOfEn"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string FK_MapData
        {
            get
            {
                string str = context.Request.QueryString["FK_MapData"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
           public int GroupField
        {
            get
            {
                string str = context.Request.QueryString["GroupField"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse( str);
            }
        }
        
        /// <summary>
        /// 框架ID
        /// </summary>
        public string FK_MapFrame
        {
            get
            {
                string str = context.Request.QueryString["FK_MapFrame"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        ///  节点ID.
        /// </summary>
        public int FK_Node
        {
            get
            {
                string str = context.Request.QueryString["FK_Node"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        ///   RefOID
        /// </summary>
        public int RefOID
        {
            get
            {
                string str = context.Request.QueryString["RefOID"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 明细表
        /// </summary>
        public string FK_MapDtl
        {
            get
            {
                string str = context.Request.QueryString["FK_MapDtl"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }

        /// <summary>
        /// 字段属性编号
        /// </summary>
        public string Ath
        {
            get
            {
                string str = context.Request.QueryString["Ath"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }

        public HttpContext context = null;
        /// <summary>
        /// 获得表单的属性.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValFromFrmByKey(string key)
        {
            string val = context.Request.Form[key];
            if (val == null)
                return null;
            val = val.Replace("'", "~");
            return val;
        }
        public int GetValIntFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:"+key+" 没有取到值.");

            return int.Parse(str);
        }
        public bool GetValBoolenFromFrmByKey(string key)
        {
            string val = this.GetValFromFrmByKey(key);
            if (val == null || val == "")
                return false;
            return true;
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public int GetRequestValInt(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "")
                return 0;
            return int.Parse(str);
        }
        #endregion 属性.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "Designer_NewMapDtl":
                        msg = this.Designer_NewMapDtl();
                        break;
                    case "Designer_NewFrame":
                        msg = this.Designer_NewFrame();
                        break;
                    case "Designer_AthNew":
                        msg = this.Designer_AthNew();
                        break;
                    case "GroupField_SaveCheck":  
                        msg = this.GroupField_SaveCheck();
                        break;
                    case "GroupField_DeleteCheck":  //删除分组
                        msg = this.GroupField_DeleteCheck();
                        break;
                    case "GroupField_DeleteAllCheck":  //删除并删除该分组下的字段
                        msg = this.GroupField_DeleteAllCheck();
                        break;
                    case "GroupField_Init"://保存空白.
                        msg = this.GroupField_Init();
                        break;
                    case "GroupField_SaveBlank"://保存空白.
                        msg = this.GroupField_SaveBlank();
                        break;
                    case "CheckGroup_Save":   //审核分组保存
                        msg = this.CheckGroup_Save();
                        break;
                    case "MapFrame_Save"://框架保存
                        msg = this.MapFrame_Save();
                        break;
                    case "MapFrame_Init": //框架初始化.
                        msg = this.MapFrame_Init();
                        break;
                    case "MapFrame_Delete": //框架初始化.
                        msg = this.MapFrame_Delete();
                        break;
                    case "DtlInit": //初始化明细表.
                        msg = this.DtlInit();
                        break;
                    case "DtlSave": //保存明细表.
                        msg = this.DtlSave();
                        break;
                    case "DtlAttrs":
                        msg = this.DtlAttrs();
                        break;
                    case "EditTableField_Save": //保存外键字段..
                        msg = this.EditTableField_Save();
                        break;
                    case "EditTableField_Init": //初始化外键表.
                        msg = this.EditTableField_Init();
                        break;
                    case "SFTableList":
                        msg = this.SFTableList();
                        break;
                    case "SFTableDelete": //删除.
                        msg = this.SFTableDelete();
                        break;
                    case "FieldSaveEnum": //保存字段.
                        msg = this.FieldSaveEnum();
                        break;
                    case "FieldInitEnum": //初始化枚举信息.
                        msg = this.FieldInitEnum();
                        break;
                    case "EnumList": //获得枚举列表.
                        msg = this.EnumList();
                        break;
                    case "FieldTypeSelect": //选择字段.
                        msg = this.FieldTypeSelect();
                        break;
                    case "EditF_FieldInit": //字段属性.
                        msg = this.EditF_FieldInit();
                        break;
                    case "EditF_Save": //保存字段.
                        msg = this.EditF_Save();
                        break;
                    case "FieldDelete": //执行删除..
                        msg = this.FieldDelete();
                        break;
                    case "FieldInitGroupID": //转化成json..
                        msg = this.FieldInitGroupID();
                        break;
                    case "FieldInitGroupAndSysEnum": //转化成json..
                        msg = this.FieldInitGroupAndSysEnum();
                        break;
                    case "DtlFieldUp": //字段上移
                        MapAttr attrU = new MapAttr(this.MyPK);
                        attrU.DoUpForMapDtl();
                        msg = "";
                        break;
                    case "DtlFieldDown": //字段下移.
                        MapAttr attrD = new MapAttr(this.MyPK);
                        attrD.DoDownForMapDtl();
                        msg = "";
                        return;
                    case "DownTempFrm":
                        this.DownTempFrm();
                        return;
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
                    case "Attachment_Init": //字段属性初始化
                        msg = this.Attachment_Init();
                        break;
                    case "Attachment_Save": //字段属性保存
                        msg = this.Attachment_Save();
                        break;
                    case "Attachment_Delete": //字段属性删除
                        msg = this.Attachment_Delete();
                        break;
                    case "EditFExtContral_Init": //字段属性删除
                        msg = this.EditFExtContral_Init();
                        break;
                    case "EditFExtContral_Save": //字段属性删除
                        msg = this.EditFExtContral_Save();
                        break;
                    case "GFDoUp":
                        GroupField gf = new GroupField(this.RefOID);
                        gf.DoUp();
                        gf.Retrieve();
                        if (gf.Idx == 0)
                            return;

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
                    //case "AthDoUp":
                    //    FrmAttachment frmAth = new FrmAttachment(this.MyPK);
                    //    if (frmAth.RowIdx > 0)
                    //    {
                    //        frmAth.RowIdx = frmAth.RowIdx - 1;
                    //        frmAth.Update();
                    //    }
                    //    break;
                    //case "AthDoDown":
                    //    FrmAttachment frmAthD = new FrmAttachment(this.MyPK);
                    //    if (frmAthD.RowIdx < 10)
                    //    {
                    //        frmAthD.RowIdx = frmAthD.RowIdx + 1;
                    //        frmAthD.Update();
                    //    }
                    //    break;
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
                        msg = "err@没有判断的执行类型：" + this.DoType;
                        break;
                }
                context.Response.Write(msg);
            }
            catch (Exception ex)
            {
                context.Response.Write("err@" + ex.Message);
            }
            //输出信息.
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
                return "err@附件ID:"+ath.NoOfObj+"已经存在.";
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
            gf.OID= this.GetRequestValInt("GroupField");
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
            gf.OID= this.GetRequestValInt("GroupField");
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
                return  "前缀已经使用：" + prx + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";
            }
            return "保存成功";
        }

        public string GroupField_SaveCheck()
        {
            string lab=this.GetValFromFrmByKey("TB_Check_Name");
            string prx=this.GetValFromFrmByKey("TB_Check_No");
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
                    BP.DA.DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE EnName='"+this.FK_MapData+"' AND CtrlID='"+en.AthRefObj+"'");

                    FrmAttachments aths = new FrmAttachments(this.FK_MapData);
                    foreach (FrmAttachment item in aths)
                    {
                        string sql = "SELECT count(*) FROM Sys_MapAttr WHERE AtPara LIKE '%"+item.MyPK+"@%' AND FK_MapData='"+this.FK_MapData+"'";
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
            return "操作成功..." + this.MyPK ;
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
        public string FieldTypeSelect()
        {
            string no = this.GetRequestVal("KeyOfEn");
            string name = this.GetRequestVal("Name");

            int fType = int.Parse(this.context.Request.QueryString["FType"]);

            MapAttrs attrs = new MapAttrs();
            int i = attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, no);
            if (i != 0)
                return "err@字段名：" + no + "已经存在.";

            //求出选择的字段类型.
            MapAttr attr = new MapAttr();
            attr.Name = name;
            attr.KeyOfEn = no;
            attr.FK_MapData = this.FK_MapData;
            attr.LGType = FieldTypeS.Normal;
            attr.MyPK = this.FK_MapData + "_" + no;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDate + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDateTime + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppBoolean + "&DoType=Edit&GroupField=" + this.GroupField;
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
                attr.ColSpan=1;

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
            if (enumKey == "" || enumKey == null || enumKey=="null")
            {
                MapAttr ma = new MapAttr(this.MyPK);
                enumKey = ma.UIBindKey;
            }

            SysEnums enums = new SysEnums(enumKey);
            ds.Tables.Add(enums.ToDataTableField("Sys_Enum"));

            //转化成json输出.
            string json=  BP.Tools.Json.ToJson(ds);
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
                return "err@"+ex.Message;
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
                return "err@"+ex.Message;
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
                FieldTypeS  lgType = (FieldTypeS)int.Parse(context.Request.QueryString["LGType"]); //逻辑类型.
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

		switch(attr.MyDataType)
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
            catch(Exception ex)
            {
                return "err@"+ex.Message;
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
                ath.SaveTo = SystemConfig.PathOfDataUser + "\\UploadFile\\"+this.FK_MapData+"\\";
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
            {
                ath.Save(); //执行保存.              
            }else {
                ath.Update();
            }
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
}