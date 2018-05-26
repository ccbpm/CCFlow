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

            // 检查组件的分组是否完整?
            foreach (GroupField item in gfs)
            {
                    bool isHave=false;
                if (item.CtrlType == "Dtl")
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dtl.No == item.CtrlID)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    //分组不存在了，就删除掉他.
                    if (isHave == false)
                        item.Delete();
                }
            }

            if (this.FK_MapData.IndexOf("ND") == 0)
            {
                string nodeStr = this.FK_MapData.Replace("ND", "");
                if (DataType.IsNumStr(nodeStr) == true)
                {
                    FrmNodeComponent fnc = new FrmNodeComponent(int.Parse(nodeStr));
                    //   var f = fnc.GetValFloatByKey("FWC_H");
                    ds.Tables.Add(fnc.ToDataTableField("WF_Node"));
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
            string isFor = this.GetRequestVal("For");
            if (isFor != "")
                return "sln@" + isFor;

            if (this.FK_MapDtl.Contains("_Ath") == true)
                return "info@附件扩展";


            MapDtl dtl = new MapDtl();

            //如果传递来了节点信息, 就是说明了独立表单的节点方案处理, 现在就要做如下判断
            if(this.FK_Node != 0)
            {
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;

                if (dtl.RetrieveFromDBSources() == 0)
                {
                    // 开始复制它的属性.
                    MapAttrs attrs = new MapAttrs(this.FK_MapDtl);
                    MapDtl odtl = new Sys.MapDtl();
                    odtl.No = this.FK_MapDtl;
                    int i= odtl.RetrieveFromDBSources();
                    if (i == 0)
                        return "info@字段列";


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
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, this.FK_MapDtl, 100, 200);
            else
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, dtl.Name, dtl.X, dtl.Y);

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
        /// 转拼音
        /// </summary>
        /// <returns></returns>
        public string ParseStringToPinyin()
        {
            string name = GetRequestVal("name");
            string flag = GetRequestVal("flag");
            //此处为字段中文转拼音，设置为最大20个字符，edited by liuxc,2017-9-25
            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(flag, "true"), true, 20);
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
                case "GFDoUp":
                    GroupField gf = new GroupField(this.RefOID);
                    gf.DoUp();
                    gf.Retrieve();
                    if (gf.Idx == 0)
                        return "";

                    int oidIdx = gf.Idx;
                    gf.Idx = gf.Idx - 1;
                    GroupField gfUp = new GroupField();
                    if (gfUp.Retrieve(GroupFieldAttr.FrmID, gf.FrmID, GroupFieldAttr.Idx, gf.Idx) == 1)
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
                    if (gfDown.Retrieve(GroupFieldAttr.FrmID, mygf.FrmID, GroupFieldAttr.Idx, mygf.Idx) == 1)
                    {
                        gfDown.Idx = oidIdx1;
                        gfDown.Update();
                    }
                    mygf.Update();
                    break;
                default:
                    throw new Exception("没有判断的执行类型：" + this.DoType);
                    break;
            }
            return msg;
        }

        /// <summary>
        /// 删除枚举值
        /// </summary>
        /// <returns></returns>
        public string SysEnumList_Del()
        {
            WF_Admin_CCFormDesigner_DialogCtr en = new WF_Admin_CCFormDesigner_DialogCtr(this.context);
            return en.FrmEnumeration_DelEnum();
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
                attr.Name = "枚举" + attr.UIBindKey;

            string sql = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID='" + this.FK_MapData + "' AND ( CtrlType='' OR CtrlType IS NULL ) ORDER BY OID DESC ";
            attr.GroupID = DBAccess.RunSQLReturnValInt(sql, 0);
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
                //en.Name = "从表" + en.No;
                en.Name = "从表";
                en.PTable = en.No;
                en.Insert();
                en.IntMapAttrs();
            }

            //返回字串.
            return en.No;
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
        public string GroupField_Save()
        {
            string lab = this.GetValFromFrmByKey("TB_Check_Name");
            if (lab.Length == 0)
                return "err@审核岗位不能为空";

            string prx = this.GetValFromFrmByKey("TB_Check_No");
            if (prx.Length == 0)
                prx = DataType.ParseStringToPinyin(lab);

            MapAttr attr = new MapAttr();
            int i = attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Note");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Checker");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_RDT");

            if (i > 0)
                return "err@前缀已经使用：" + prx + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。";

            BP.Sys.CCFormAPI.CreateCheckGroup(this.FK_MapData, lab, prx);

            return "保存成功";
        }
        /// <summary>
        /// 创建审核分组
        /// </summary>
        /// <returns></returns>
        public string GroupField_Create()
        {
            BP.Sys.GroupField gf = new GroupField();
            gf.FrmID = this.FK_MapData;
            gf.Lab = this.GetRequestVal("Lab");
            gf.EnName =this.FK_MapData;
            gf.Insert();
            return "创建成功..";
        }
        /// <summary>
        /// 保存分组
        /// </summary>
        /// <returns></returns>
        public string GroupField_SaveCheck()
        {
            string lab = this.GetRequestVal("TB_Check_Name");
            string prx = this.GetRequestVal("TB_Check_No");
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

        public string ImpTableField_Step1()
        {
            BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
            ens.RetrieveAll();
            DataSet ds = new DataSet();
            ds.Tables.Add(ens.ToDataTableField("SFDBSrcs"));
            return BP.Tools.Json.ToJson(ds);
        }

        public string FK_MapData
        {
            get
            {
                string str = this.GetRequestVal("FK_MapData");  //context.Request.QueryString["FK_MapData"];
                if (DataType.IsNullOrEmpty(str))
                    return "abc";
                return str;
            }
        }
        public string FK_SFDBSrc
        {
            get
            {
                return this.GetRequestVal("FK_SFDBSrc"); 
                //return context.Request.QueryString["FK_SFDBSrc"];
            }
        }
        private string _STable = null;
        public string STable
        {
            get
            {
                if (_STable == null)
                {
                    //return this.GetRequestVal("FK_SFDBSrc");

                    _STable = this.GetRequestVal("FK_SFDBSrc");// context.Request.QueryString["STable"];
                    if (_STable == null || "".Equals(_STable))
                    {
                        BP.En.Entity en = BP.En.ClassFactory.GetEn(this.FK_MapData);
                        if (en != null)
                            _STable = en.EnMap.PhysicsTable;
                        else
                        {
                            MapData md = new MapData(this.FK_MapData);
                            _STable = md.PTable;
                        }
                    }
                }

                if (_STable == null)
                    _STable = "";
                return _STable;
            }
        }

        public string ImpTableField_Step2()
        {

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);
            dictionary.Add("SFDBSrc", src.ToDataTableField());

            DataTable tables = src.GetTables();
            dictionary.Add("tables", tables);
            
            DataTable tableColumns = src.GetColumns(this.STable);
            dictionary.Add("columns", tableColumns);

            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            dictionary.Add("attrs", attrs.ToDataTableField("attrs"));
            dictionary.Add("STable", this.STable);

            return BP.Tools.Json.ToJson(dictionary);
        }

        private List<string> sCols = null;

        public List<string> SColumns
        {
            get
            {
                if (sCols != null)
                    return sCols;

                string cols = this.GetRequestVal("SColumns") ?? "";
                string[] arr = cols.Split(',');
                sCols = new List<string>();

                foreach (string s in arr)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;

                    sCols.Add(s);
                }

                return sCols;
            }
        }

        public string ImpTableField_Step3()
        {
            DataSet ds = new DataSet();
            SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);
            var tableColumns = src.GetColumns(this.STable);
            DataTable dt = tableColumns.Clone();
            dt.TableName = "selectedColumns";
            foreach (DataRow dr in tableColumns.Rows)
            {
                if (this.SColumns.Contains(dr["No"]))
                {
                    dt.Rows.Add(dr.ItemArray);
                }
            }
            ds.Tables.Add(dt);
            SysEnums ens = new SysEnums(MapAttrAttr.MyDataType);
            ds.Tables.Add(ens.ToDataTableField("MyDataType"));
            SysEnums ens1 = new SysEnums(MapAttrAttr.LGType);
            ds.Tables.Add(ens1.ToDataTableField("LGType"));
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 保存字段
        /// </summary>
        /// <returns></returns>
        public string ImpTableField_Save()
        {
            MapData md = new MapData();
            md.No = this.FK_MapData;
            md.RetrieveFromDBSources();

            string msg = md.Name+"导入字段信息:"+this.FK_MapData;
            bool isLeft = true;
            float maxEnd = md.MaxEnd;

            foreach (string name in this.context.Request.Form.Keys)
            {
                if (name.StartsWith("HID_Idx_") == false)
                    continue;

                string columnName = name.Substring("HID_Idx_".Length);

                MapAttr ma = new MapAttr();
                ma.KeyOfEn = columnName;
                ma.FK_MapData = this.FK_MapData;
                ma.MyPK = this.FK_MapData + "_" + ma.KeyOfEn;
                if (ma.IsExits)
                {
                    msg += "\t\n字段:" + ma.KeyOfEn + " - " + ma.Name + "已存在.";
                    continue;
                }

                ma.Name = this.GetValFromFrmByKey("TB_Desc_" + columnName);
                if (DataType.IsNullOrEmpty(ma.Name))
                    ma.Name = ma.KeyOfEn;

                ma.MyDataType =this.GetValIntFromFrmByKey("DDL_DBType_" + columnName);
                ma.MaxLen = this.GetValIntFromFrmByKey("TB_Len_" + columnName);
                ma.UIBindKey = this.GetValFromFrmByKey("TB_BindKey_" + columnName);
                ma.LGType = BP.En.FieldTypeS.Normal;

                //绑定了外键或者枚举.
                if (DataType.IsNullOrEmpty(ma.UIBindKey) == false)
                {
                    SysEnums se = new SysEnums();
                    se.Retrieve(SysEnumAttr.EnumKey, ma.UIBindKey);
                    if (se.Count > 0)
                    {
                        ma.MyDataType = BP.DA.DataType.AppInt;
                        ma.LGType = BP.En.FieldTypeS.Enum;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }
                    SFTable tb = new SFTable();
                    tb.No = ma.UIBindKey;
                    if (tb.IsExits == true)
                    {
                        ma.MyDataType = BP.DA.DataType.AppString;
                        ma.LGType = BP.En.FieldTypeS.FK;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }
                }

                if (ma.MyDataType == BP.DA.DataType.AppBoolean)
                    ma.UIContralType = BP.En.UIContralType.CheckBok;

                ma.Insert();

                msg += "\t\n字段:" + ma.KeyOfEn + " - " + ma.Name + "加入成功.";

                //生成lab.
                FrmLab lab = null;
                if (isLeft == true)
                {
                    maxEnd = maxEnd + 40;
                    /* 是否是左边 */
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 40;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                else
                {
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 350;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                isLeft = !isLeft;
            }

            //更新名称.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name=KeyOfEn WHERE Name=NULL OR Name='' ");

            md.ResetMaxMinXY();
            return msg;
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
                    BP.DA.DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE FrmID='" + this.FK_MapData + "' AND CtrlID='" + en.AthRefObj + "'");

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
        public string SysEnumList_Init()
        {
            SysEnumMains ses = new SysEnumMains();
            ses.RetrieveAll();

            //增加到列表里.
            DataSet ds = new DataSet();
            ds.Tables.Add(ses.ToDataTableField("SysEnumMains"));

            int pTableModel = 0;
            MapDtl dtl = new MapDtl();
            dtl.No = this.FK_MapData;
            if (dtl.RetrieveFromDBSources() == 1)
            {
                pTableModel = dtl.PTableModel;
            }
            else
            {
                MapData md = new MapData();
                md.No = this.FK_MapData;
                md.RetrieveFromDBSources();
                pTableModel = md.PTableModel;
            }

            if (pTableModel == 2)
            {
                DataTable dt = MapData.GetFieldsOfPTableMode2(this.FK_MapData);
                dt.TableName = "Fields";
                ds.Tables.Add(dt);
            }

            return BP.Tools.Json.ToJson(ds);
        }

        #region SFList 外键表列表.
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string SFList_Delete()
        {
            try
            {
                SFTable sf = new SFTable(this.FK_SFTable);
                sf.Delete();
                return "删除成功...";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 字典表列表.
        /// </summary>
        /// <returns></returns>
        public string SFList_Init()
        {
            DataSet ds = new DataSet();
             
            SFTables ens = new SFTables();
            ens.RetrieveAll();

            DataTable dt = ens.ToDataTableField("SFTables");
            ds.Tables.Add(dt);

            int pTableModel=0;
            if (this.GetRequestVal("PTableModel").Equals("2"))
                pTableModel = 2;

            //获得ptableModel.
            if (pTableModel == 0)
            {
                MapDtl dtl = new MapDtl();
                dtl.No = this.FK_MapData;
                if (dtl.RetrieveFromDBSources() == 1)
                {
                    pTableModel = dtl.PTableModel;
                }
                else
                {
                    MapData md = new MapData();
                    md.No = this.FK_MapData;
                    if(md.RetrieveFromDBSources() == 1)
                        pTableModel = md.PTableModel;
                }
            }

            if (pTableModel == 2)
            {
                DataTable mydt = MapData.GetFieldsOfPTableMode2(this.FK_MapData);
                mydt.TableName = "Fields";
                ds.Tables.Add(mydt);
            }

            return BP.Tools.Json.ToJson(ds);
        }
        public string SFList_SaveSFField()
        {
            MapAttr attr = new Sys.MapAttr();
            attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            if (attr.RetrieveFromDBSources() != 0)
                return "err@字段名[" + this.KeyOfEn + "]已经存在.";

            BP.Sys.CCFormAPI.SaveFieldSFTable(this.FK_MapData, this.KeyOfEn, null, this.GetRequestVal("SFTable"), 100, 100, 1);

            attr.Retrieve();
            string sql = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID='" + this.FK_MapData + "' AND (CtrlType='' OR CtrlType IS NULL) ORDER BY OID DESC ";
            attr.GroupID = DBAccess.RunSQLReturnValInt(sql, 0);
            attr.Update();

            SFTable sf = new SFTable(attr.UIBindKey);

            if (sf.SrcType == SrcType.TableOrView || sf.SrcType == SrcType.BPClass || sf.SrcType == SrcType.CreateTable)
                return "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=" + attr.MyPK;
            else
                return "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=" + attr.MyPK;
        }
        #endregion 外键表列表.

        /// <summary>
        /// 初始化表.
        /// </summary>
        /// <returns></returns>
        public string EditTableField_Init()
        {
            MapAttr attr = new MapAttr();
            attr.KeyOfEn = this.KeyOfEn;
            attr.FK_MapData = this.FK_MapData;

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
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
        /// 从表里选择字段.
        /// </summary>
        /// <returns></returns>
        public string FieldTypeListChoseOneField_Init()
        {
            string ptable = "";

            MapDtl dtl = new MapDtl();
            dtl.No = this.FK_MapData;
            if (dtl.RetrieveFromDBSources() == 1)
            {
                ptable = dtl.PTable;
            }
            else
            {
                MapData md = new MapData(this.FK_MapData);
                ptable = md.PTable;
            }

            //获得原始数据.
            DataTable dt = BP.DA.DBAccess.GetTableSchema(ptable, false);

            //创建样本.
            DataTable mydt = BP.DA.DBAccess.GetTableSchema(ptable, false);
            mydt.Rows.Clear();

            //获得现有的列..
            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            string flowFiels = ",GUID,PRI,PrjNo,PrjName,PEmp,AtPara,FlowNote,WFSta,PNodeID,FK_FlowSort,FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,MyNum,PWorkID,PFlowNo,BillNo,ProjNo,";

            //排除已经存在的列.
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr["FName"].ToString();
                if (attrs.Contains(MapAttrAttr.KeyOfEn, key) == true)
                    continue;

                if (flowFiels.Contains("," + key + ",") == true)
                    continue;

                DataRow mydr = mydt.NewRow();
                mydr["FName"] = dr["FName"];
                mydr["FType"] = dr["FType"];
                mydr["FLen"] = dr["FLen"];
                mydr["FDesc"] = dr["FDesc"];
                mydt.Rows.Add(mydr);
            }

            mydt.TableName = "dt";
            return BP.Tools.Json.ToJson(mydt);
        }
        public string FieldTypeListChoseOneField_Save()
        {
            int dataType = this.GetRequestValInt("DataType");
            string keyOfEn = this.GetRequestVal("KeyOfEn");
            string name = this.GetRequestVal("FDesc");
            string frmID = this.GetRequestVal("FK_MapData");

            MapAttr attr = new MapAttr();
            attr.FK_MapData = frmID;
            attr.KeyOfEn = keyOfEn;
            attr.MyPK = attr.FK_MapData + "_" + keyOfEn;
            if (attr.IsExits)
                return "err@该字段["+keyOfEn+"]已经加入里面了.";

            attr.Name = name;
            attr.MyDataType = dataType;

            if (BP.DA.DataType.AppBoolean == dataType)
                attr.UIContralType = UIContralType.CheckBok;
            else
                attr.UIContralType = UIContralType.TB;

            string sql = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID='" + this.FK_MapData + "' AND CtrlType='' OR CtrlType= NULL";
            attr.GroupID  = DBAccess.RunSQLReturnValInt(sql, 0);

            attr.Insert();

            return "保存成功.";
        }
        /// <summary>
        /// 字段选择.
        /// </summary>
        /// <returns></returns>
        public string FieldTypeSelect_Create()
        {
            string no = this.GetRequestVal("KeyOfEn");
            string name = this.GetRequestVal("name");
            string newNo = DataType.ParseStringForNo(no, 20);
            string newName = DataType.ParseStringForName(name, 20);
            int fType = int.Parse( this.GetRequestVal("FType"));

            MapAttrs attrs = new MapAttrs();
            int i = attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, newNo);
            if (i != 0)
                return "err@字段名：" + newNo + "已经存在.";

            #region 计算GroupID  需要翻译
            int iGroupID = this.GroupField;
            try
            {
                DataTable dt = DBAccess.RunSQLReturnTable("SELECT OID FROM Sys_GroupField WHERE FrmID='" + this.FK_MapData + "' and (CtrlID is null or ctrlid ='') ORDER BY OID DESC ");
                if (dt != null && dt.Rows.Count > 0)
                {
                    iGroupID = int.Parse(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
 
            }
            #endregion

            try
            {
                MapData md = new MapData();
                md.No = this.FK_MapData;
                if (md.RetrieveFromDBSources() != 0)
                    md.CheckPTableSaveModel(newNo);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            //求出选择的字段类型.
            MapAttr attr = new MapAttr();
            attr.Name = newName;
            attr.KeyOfEn = newNo;
            attr.FK_MapData = this.FK_MapData;
            attr.LGType = FieldTypeS.Normal;
            attr.MyPK = this.FK_MapData + "_" + newNo;
            attr.GroupID = iGroupID;
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
                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&MyPK=" + attr.MyPK;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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
                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
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

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
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

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
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


                //@周朋 , 判断数据模式，创建的字段是否符合要求.
                MapData md = new MapData(this.FK_MapData);
                md.CheckPTableSaveModel(this.KeyOfEn);
             


                //赋值.
                MapAttr attr = new MapAttr();
                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                if (DataType.IsNullOrEmpty(this.MyPK) == false)
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
                if (DataType.IsNullOrEmpty(this.MyPK) == false)
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
                int fType = int.Parse(  this.GetRequestVal("FType"));  //字段数据物理类型
                FieldTypeS lgType = (FieldTypeS)int.Parse( this.GetRequestVal("LGType") ); //逻辑类型.
                string uiBindKey = this.GetRequestVal("UIBindKey");// context.Request.QueryString["UIBindKey"];

                //赋值.
                MapAttr attr = new MapAttr();
                attr.KeyOfEn = this.KeyOfEn;
                attr.FK_MapData = this.FK_MapData;
                attr.LGType = lgType; //逻辑类型.
                attr.UIBindKey = uiBindKey; //绑定的枚举或者外键.
                attr.MyDataType = fType; //物理类型.

                if (DataType.IsNullOrEmpty(this.MyPK) == false)
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
            string fileFullName = context.Request.PhysicalApplicationPath + "\\Temp\\" + this.FK_MapData + ".xml";
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


        #region sfGuide
        /// <summary>
        /// 获取数据源字典表信息
        /// </summary>
        /// <returns></returns>
        public string SFGuide_GetInfo()
        {
            string sfno = this.GetRequestVal("sfno"); //context.Request.QueryString["sfno"];

            if (string.IsNullOrWhiteSpace(sfno))
                return "err@参数不正确";

            SFTable sftable = new SFTable(sfno);
            DataTable dt = sftable.ToDataTableField("info");

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            return BP.Tools.Json.ToJson(dt);
        }
        public string SFGuide_SaveInfo()
        {
            bool IsNew = this.GetRequestValBoolen("IsNew");
            string sfno = this.GetRequestVal("No"); 
            string myname = this.GetRequestVal("Name");

            int srctype = this.GetRequestValInt("SrcType");
            int codestruct = this.GetRequestValInt("CodeStruct");

            string defval = this.GetRequestVal("DefVal");
            string sfdbsrc = this.GetRequestVal("FK_SFDBSrc");
            string srctable = this.GetRequestVal("SrcTable");
            string columnvalue = this.GetRequestVal("ColumnValue");
            string columntext = this.GetRequestVal("ColumnText");

            string parentvalue = this.GetRequestVal("ParentValue");
            string tabledesc = this.GetRequestVal("TableDesc");
            string selectstatement = this.GetRequestVal("Selectstatement");

            //判断是否已经存在
            SFTable sftable = new SFTable();
            sftable.No = sfno;

            if (IsNew && sftable.RetrieveFromDBSources() > 0)
                return "err@字典编号" + sfno + "已经存在，不允许重复。";

            sftable.Name = myname;
            sftable.SrcType = (SrcType)srctype;
            sftable.CodeStruct = (CodeStruct)codestruct;
            sftable.DefVal = defval;
            sftable.FK_SFDBSrc = sfdbsrc;
            sftable.SrcTable = srctable;
            sftable.ColumnValue = columnvalue;
            sftable.ColumnText = columntext;
            sftable.ParentValue = parentvalue;
            sftable.TableDesc = tabledesc;
            sftable.SelectStatement = selectstatement;

            switch (sftable.SrcType)
            {
                case SrcType.BPClass:
                    string[] nos = sftable.No.Split('.');
                    sftable.FK_Val = "FK_" + nos[nos.Length - 1].TrimEnd('s');
                    sftable.FK_SFDBSrc = "local";
                    break;
                default:
                    sftable.FK_Val = "FK_" + sftable.No;
                    break;
            }

            sftable.Save();
            return "保存成功！";
        }
        public string SFGuide_Getmtds()
        {
            string src = this.GetRequestVal("src"); //context.Request.QueryString["src"];
            if (string.IsNullOrWhiteSpace(src))
                return "err@系统中没有webservices类型的数据源，该类型的外键表不能创建，请维护数据源.";

            SFDBSrc sr = new SFDBSrc(src);

            if (sr.DBSrcType != DBSrcType.WebServices)
                return "err@数据源“" + sr.Name + "”不是WebService数据源.";

            List<WSMethod> mtds = GetWebServiceMethods(sr);

            return LitJson.JsonMapper.ToJson(mtds);
        }
        public string SFGuide_GetCols()
        {
            string src = this.GetRequestVal("src"); //context.Request.QueryString["src"];
            string table = this.GetRequestVal("table"); //context.Request.QueryString["table"];

            if (string.IsNullOrWhiteSpace(src))
                throw new Exception("err@参数不正确");


            if (string.IsNullOrWhiteSpace(table))
            {
                return "[]";
            }

            SFDBSrc sr = new SFDBSrc(src);
            DataTable dt = sr.GetColumns(table);

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            foreach (DataRow r in dt.Rows)
            {
                r["Name"] = r["No"] +
                            (r["Name"] == null || r["Name"] == DBNull.Value ||
                             string.IsNullOrWhiteSpace(r["Name"].ToString())
                                 ? ""
                                 : string.Format("[{0}]", r["Name"]));
            }

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// //获取表/视图列表
        /// </summary>
        /// <returns></returns>
        public string SFGuide_GetTVs()
        {
            string src = this.GetRequestVal("src");// context.Request.QueryString["src"];

            SFDBSrc sr = new SFDBSrc(src);
            DataTable dt = sr.GetTables();

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得clsss列表.
        /// </summary>
        /// <returns></returns>
        public string SFGuide_GetClass()
        {
            string sfno = this.GetRequestVal("sfno");// context.Request.QueryString["sfno"];
            string stru = this.GetRequestVal("struct");  //context.Request.QueryString["struct"];
            int st = 0;

            if (string.IsNullOrWhiteSpace(stru) || !int.TryParse(stru, out st))
                throw new Exception("err@参数不正确.");

            string error = string.Empty;
            ArrayList arr = null;
            SFTables sfs = new SFTables();
            Entities ens = null;
            SFTable sf = null;
            sfs.Retrieve(SFTableAttr.SrcType, (int)SrcType.BPClass);

            switch (st)
            {
                case 0:
                    arr = ClassFactory.GetObjects("BP.En.EntityNoName");
                    break;
                case 1:
                    arr = ClassFactory.GetObjects("BP.En.EntitySimpleTree");
                    break;
                default:
                    arr = new ArrayList();
                    break;
            }

            StringBuilder s = new StringBuilder("[");
            foreach (BP.En.Entity en in arr)
            {
                try
                {
                    if (en == null)
                        continue;

                    ens = en.GetNewEntities;
                    if (ens == null)
                        continue;

                    sf = sfs.GetEntityByKey(ens.ToString()) as SFTable;

                    if ((sf != null && sf.No != sfno) ||
                        string.IsNullOrWhiteSpace(ens.ToString()))
                        continue;

                    s.Append(string.Format(
                        "{{\"NO\":\"{0}\",\"NAME\":\"{0}[{1}]\",\"DESC\":\"{1}\"}},", ens,
                        en.EnDesc));
                }
                catch
                {
                    continue;
                }
            }
            return s.ToString().TrimEnd(',') + "]";
        }
        /// <summary>
        /// 获取数据源列表
        /// </summary>
        /// <returns></returns>
        public string SFGuide_GetSrcs()
        {

            string type = this.GetRequestVal("type");
            int itype;
            bool onlyWS = false;

            SFDBSrcs srcs = new SFDBSrcs();
            if (!string.IsNullOrWhiteSpace(type) && int.TryParse(type, out itype))
            {
                onlyWS = true;
                srcs.Retrieve(SFDBSrcAttr.DBSrcType, itype);
            }
            else
            {
                srcs.RetrieveAll();
            }

            DataTable dt = srcs.ToDataTableField();

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            if (onlyWS == false)
            {
                List<DataRow> wsRows = new List<DataRow>();
                foreach (DataRow r in dt.Rows)
                {
                    if (Equals(r["DBSrcType"], (int)DBSrcType.WebServices))
                        wsRows.Add(r);
                }

                foreach (DataRow r in wsRows)
                    dt.Rows.Remove(r);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP))
                return new List<WSMethod>();

            var wsurl = dbsrc.IP.ToLower();
            if (!wsurl.EndsWith(".asmx") && !wsurl.EndsWith(".svc"))
                throw new Exception("@失败:" + dbsrc.No + " 中WebService地址不正确。");

            wsurl += wsurl.EndsWith(".asmx") ? "?wsdl" : "?singleWsdl";

            #region //解析WebService所有方法列表
            //var methods = new Dictionary<string, string>(); //名称Name，全称Text
            List<WSMethod> mtds = new List<WSMethod>();
            WSMethod mtd = null;
            var wc = new WebClient();
            var stream = wc.OpenRead(wsurl);
            var sd = ServiceDescription.Read(stream);
            var eles = sd.Types.Schemas[0].Elements.Values.Cast<XmlSchemaElement>();
            var s = new StringBuilder();
            XmlSchemaComplexType ctype = null;
            XmlSchemaSequence seq = null;
            XmlSchemaElement res = null;

            foreach (var ele in eles)
            {
                if (ele == null) continue;

                var resType = string.Empty;
                var mparams = string.Empty;

                //获取接口返回元素
                res = eles.FirstOrDefault(o => o.Name == (ele.Name + "Response"));

                if (res != null)
                {
                    mtd = new WSMethod();
                    //1.接口名称 ele.Name
                    mtd.No = ele.Name;
                    mtd.ParaMS = new Dictionary<string, string>();
                    //2.接口返回值类型
                    ctype = res.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                        mtd.Return = resType = (seq.Items[0] as XmlSchemaElement).SchemaTypeName.Name;
                    else
                        continue;// resType = "void";   //去除不返回结果的接口

                    //3.接口参数
                    ctype = ele.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                    {
                        foreach (XmlSchemaElement pe in seq.Items)
                        {
                            mparams += pe.SchemaTypeName.Name + " " + pe.Name + ", ";
                            mtd.ParaMS.Add(pe.Name, pe.SchemaTypeName.Name);
                        }

                        mparams = mparams.TrimEnd(", ".ToCharArray());
                    }

                    mtd.Name = string.Format("{0} {1}({2})", resType, ele.Name, mparams);
                    mtds.Add(mtd);
                    //methods.Add(ele.Name, string.Format("{0} {1}({2})", resType, ele.Name, mparams));
                }
            }

            stream.Close();
            stream.Dispose();
            wc.Dispose();
            #endregion

            return mtds;
        }
        #endregion


        #region  ImpTableFieldSelectBindKey 外键枚举
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string ImpTableFieldSelectBindKey_Init()
        {
            DataSet ds = new DataSet();

            BP.Sys.SysEnumMains ens = new BP.Sys.SysEnumMains();
            ens.RetrieveAll();
            ds.Tables.Add(ens.ToDataTableField("EnumMain"));

            BP.Sys.SFTables tabs = new BP.Sys.SFTables();
            tabs.RetrieveAll();
            ds.Tables.Add(tabs.ToDataTableField("SFTables"));

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion  ImpTableFieldSelectBindKey 外键枚举


    }

    public class WSMethod
    {
        public string No { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> ParaMS { get; set; }

        public string Return { get; set; }
    }
}
