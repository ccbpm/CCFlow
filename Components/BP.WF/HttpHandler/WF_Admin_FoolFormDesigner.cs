using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Template.Frm;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 处理页面的业务逻辑
    /// </summary>
    public class WF_Admin_FoolFormDesigner : DirectoryPageBase
    {
        #region 表单设计器.
        public string Designer_Move()
        {
            //当前的MyPK.
            string mypk = this.MyPK; //当前的主键.

            //获得当前分组下的字段集合.
            string mypks = this.GetRequestVal("MyPKs"); //字段集合.
            int groupID = this.GetRequestValInt("GroupID");

            GroupField gf = new GroupField(groupID);
            if (gf.CtrlType.Equals("") == false)
                return "err@你不能移动到【" + gf.Lab + "】它是非字段分组，您不能移动。";
            string sql = "";
            string[] strs = mypks.Split(',');
            for (int i = 0; i < strs.Length; i++)
            {
                var str = strs[i];
                if (str.Equals(mypk))
                    sql = "UPDATE Sys_MapAttr SET GroupID=" + groupID + ", Idx=" + i + " WHERE MyPK='" + str + "'";
                else
                    sql = "UPDATE Sys_MapAttr SET Idx=" + i + " WHERE MyPK='" + str + "'";
                DBAccess.RunSQL(sql);
            }
            return "表单顺序移动成功..";
        }
        /// <summary>
        /// 字段分组移动.
        /// </summary>
        /// <returns></returns>
        public string DesignerVue_GF_Move()
        {
            string[] strs = this.GetRequestVal("Vals").Split(',');
            for (int i = 0; i < strs.Length; i++)
            {
                string str = strs[i];
                if (DataType.IsNullOrEmpty(str))
                    continue;

                string sql = "UPDATE Sys_GroupField SET Idx=" + i + " WHERE OID=" + str;
                DBAccess.RunSQL(sql);
            }
            return "分组顺序移动成功...";
        }
        /// <summary>
        ///  自动创建字段接口
        /// </summary>
        /// <returns></returns>
        public string DesignerVue_CreateField()
        {
            //获得类型.
            int dataType = int.Parse(this.GetRequestVal("DataType"));

            int fIdx = int.Parse(this.GetRequestVal("Idx"));

            MapAttr attr = new MapAttr();

            int idx = 0;
            while (true)
            {
                idx++;

                string field = "F" + idx.ToString();

                attr.MyPK = this.FrmID + "_" + field;
                if (attr.IsExits == true)
                    continue;

                //   if (attr.IsExit("KeyOfEn", field, "FK_MapData", this.FrmID, "DataType", dataType) == true)
                //     continue;

                if (dataType == BP.DA.DataType.AppString)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "文字" + field;
                    attr.MyDataType = dataType;
                    attr.setMaxLen(50);
                    attr.setMinLen(0);
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppInt)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "Int数值" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppBoolean)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "开关类型：" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppMoney)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "金额类型：" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppFloat)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "Float数值" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppDouble)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "Double数值" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                if (dataType == BP.DA.DataType.AppDate)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "Date" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }
                if (dataType == BP.DA.DataType.AppDateTime)
                {
                    attr.setKeyOfEn(field);
                    attr.Name = "DateTime" + field;
                    attr.MyDataType = dataType;
                    attr.setIdx(fIdx);
                    attr.Insert();
                    return attr.ToJson();
                }

                return "err@参数类型错误:dataType:" + dataType + ",没有判断.";
            }

            return "err@不应该运行到这里.";
        }
        /// <summary>
        /// 删除字段
        /// </summary>
        /// <returns></returns>
        public string DesignerVue_DeleteField()
        {
            MapAttr ma = new MapAttr(this.MyPK);
            ma.Delete();
            return "删除成功.";
        }

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
        /// 检查表单
        /// </summary>
        /// <returns></returns>
        public string Designer_CheckFrm()
        {
            #region  检查完整性.
            if (this.FK_MapData.ToUpper().Contains("BP.") == true)
            {
                /*如果是类的实体.*/
                Entities ens = ClassFactory.GetEns(this.FK_MapData);
                Entity en = ens.GetNewEntity;

                MapData mymd = new MapData();
                mymd.No = this.FK_MapData;
                mymd.ClearCash(); //清除缓存。

                int i = mymd.RetrieveFromDBSources();
                if (i == 0)
                    en.DTSMapToSys_MapData(this.FK_MapData); //调度数据到

                mymd.RetrieveFromDBSources();
                mymd.HisFrmType = FrmType.FoolForm;
                mymd.Update();
            }
            #endregion

         //   MapFrmFool cols = new MapFrmFool(this.FK_MapData);
          //  cols.DoCheckFixFrmForUpdateVer();
            return "url@Designer.htm?FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
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
                return Designer_CheckFrm();
            }

            //把表单属性放入里面去.
            MapData md = new MapData(this.FK_MapData);
            //清缓存
            md.ClearCash();
            ds.Tables.Add(md.ToDataTableField("Sys_MapData").Copy());

            // 字段属性.
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            foreach (MapAttr item in mattrs)
                item.DefVal = item.DefValReal;

            ds.Tables.Add(mattrs.ToDataTableField("Sys_MapAttr"));

            GroupFields gfs = new GroupFields(this.FK_MapData);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupField"));

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, this.FK_MapData, MapDtlAttr.FK_Node, 0);
            ds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));

            MapFrames frms = new MapFrames(this.FK_MapData);
            ds.Tables.Add(frms.ToDataTableField("Sys_MapFrame"));

            //附件表.
            FrmAttachments aths = new FrmAttachments(this.FK_MapData);
            ds.Tables.Add(aths.ToDataTableField("Sys_FrmAttachment"));

            //加入扩展属性.
            MapExts MapExts = new MapExts(this.FK_MapData);
            ds.Tables.Add(MapExts.ToDataTableField("Sys_MapExt"));

            // 检查组件的分组是否完整?
            foreach (GroupField item in gfs)
            {
                bool isHave = false;
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
                    ds.Tables.Add(fnc.ToDataTableField("WF_Node").Copy());
                }
            }

            //把dataet转化成json 对象.
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion

        public string Designer_AthNew()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.setFK_MapData(this.FK_MapData);
            ath.NoOfObj = this.GetRequestVal("AthNo");
            ath.setMyPK(ath.FK_MapData + "_" + ath.NoOfObj);
            if (ath.RetrieveFromDBSources() == 1)
                return "err@附件ID:" + ath.NoOfObj + "已经存在.";
            BP.Sys.CCFormAPI.CreateOrSaveAthMulti(this.FK_MapData, this.GetRequestVal("AthNo"), "我的附件");
            return ath.MyPK;
        }

        /// <summary>
        /// 生成随机的字段ID.
        /// </summary>
        /// <returns></returns>
        public string GenerRandomFieldID()
        {
            string sql = "SELECT count(MyPK) as MyNum FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "'";
            int idx = DBAccess.RunSQLReturnValInt(sql);
            for (int i = idx; i < 999; i++)
            {
                string str = "F" + i.ToString().PadLeft(3, '0');
                sql = "SELECT count(MyPK) as MyNum FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "' AND KeyOfEn='" + str + "'";
                var num = DBAccess.RunSQLReturnValInt(sql);
                if (num == 0)
                    return str;
            }
            return "err@系统错误.";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string MapDefDtlFreeFrm_Init()
        {
            string isFor = this.GetRequestVal("For");
            if (DataType.IsNullOrEmpty(isFor) == false)
                return "sln@" + isFor;

            if (this.FK_MapDtl.Contains("_Ath") == true)
                return "info@附件扩展";
            if (this.FK_MapDtl.ToUpper().Contains("BP.WF.RETURNWORKS") == true)
                return "info@退回字段扩展";


            MapDtl dtl = new MapDtl();

            //如果传递来了节点信息, 就是说明了独立表单的节点方案处理, 现在就要做如下判断
            if (this.FK_Node != 0)
            {
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;

                if (dtl.RetrieveFromDBSources() == 0)
                {
                    // 开始复制它的属性.
                    MapAttrs mattrs = new MapAttrs(this.FK_MapDtl);
                    MapDtl odtl = new Sys.MapDtl();
                    odtl.No = this.FK_MapDtl;
                    int i = odtl.RetrieveFromDBSources();
                    if (i == 0)
                        return "info@字段列";


                    //存储表要与原明细表一致
                    if (string.IsNullOrWhiteSpace(odtl.PTable))
                        dtl.PTable = odtl.No;
                    else
                        dtl.PTable = odtl.PTable;

                    //让其直接保存.
                    dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                    dtl.setFK_MapData("Temp");
                    dtl.DirectInsert(); //生成一个明细表属性的主表.

                    //字段的分组也要一同复制
                    Dictionary<int, int> groupids = new Dictionary<int, int>();

                    //循环保存字段.
                    int idx = 0;
                    foreach (MapAttr item in mattrs)
                    {
                        if (item.GroupID != 0)
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

                        item.setFK_MapData(this.FK_MapDtl + "_" + this.FK_Node);
                        item.setMyPK(item.FK_MapData + "_" + item.KeyOfEn);
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
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, this.FK_MapDtl);
            else
                BP.Sys.CCFormAPI.CreateOrSaveDtl(this.FK_MapData, this.FK_MapDtl, dtl.Name);

            return "创建成功.";
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner()
        {
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
        /// 增加一个枚举类型
        /// </summary>
        /// <returns></returns>
        public string SysEnumList_SaveEnumField()
        {
            string dtlKey = this.GetRequestVal("DtlKeyOfEn");
            MapAttr attr = new Sys.MapAttr();
            attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);
            if (attr.RetrieveFromDBSources() != 0)
                return "err@字段名[" + this.KeyOfEn + "]已经存在.";

            if (DataType.IsNullOrEmpty(dtlKey) == false)
            {
                attr = new Sys.MapAttr();
                attr.setMyPK(this.FK_MapData + "_" + dtlKey);
                if (attr.RetrieveFromDBSources() != 0)
                    return "err@字段名[" + dtlKey + "]已经存在.";
            }
            attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);
            attr.setFK_MapData(this.FK_MapData);
            attr.setKeyOfEn(this.KeyOfEn);
            attr.UIBindKey = this.GetRequestVal("EnumKey");

            attr.GroupID = this.GetRequestValInt("GroupFeid");
            int uiContralType = this.GetRequestValInt("UIContralType");

            if (uiContralType != 0)
            {
                attr.setUIContralType((UIContralType)uiContralType);
                if (attr.UIContralType == UIContralType.RadioBtn)
                    attr.SetPara("RBShowModel", 3); //设置模式.
            }
            else
                attr.setUIContralType(UIContralType.DDL);

            if (attr.UIContralType == UIContralType.CheckBok)
                attr.setMyDataType(DataType.AppString);
            else
                attr.setMyDataType(DataType.AppInt);
            attr.setLGType(FieldTypeS.Enum);


            SysEnumMain sem = new Sys.SysEnumMain();
            sem.No = attr.UIBindKey;
            if (sem.RetrieveFromDBSources() != 0)
                attr.Name = sem.Name;
            else
            {
                Int32 count = sem.Retrieve("EnumKey", attr.UIBindKey, "OrgNo", WebUser.OrgNo);
                if (count != 0)
                    attr.Name = sem.Name;
                else
                    attr.setName("枚举" + attr.UIBindKey);
            }


            //paras参数
            Paras ps = new Paras();
            ps.SQL = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FrmID AND ( CtrlType='' OR CtrlType IS NULL ) ORDER BY OID DESC ";
            ps.Add("FrmID", this.FK_MapData);
            attr.GroupID = DBAccess.RunSQLReturnValInt(ps, 0);
            attr.Insert();
            if (DataType.IsNullOrEmpty(dtlKey) == false)
            {
                attr.setMyPK(this.FK_MapData + "_" + dtlKey);
                attr.setKeyOfEn(dtlKey);
                string uiBindKey = sem.GetParaString("DtlEnumKey");
                attr.UIBindKey = uiBindKey;
                attr.Name = sem.GetParaString("DtlName");
                attr.Insert();
                //创建联动关系
                MapExt ext = new MapExt();
                string mypk = "ActiveDDL_" + this.FK_MapData + "_" + this.KeyOfEn;
                ext.setMyPK(mypk);
                ext.AttrsOfActive = dtlKey;
                ext.DBType = "0";
                ext.FK_DBSrc = "local";
                ext.Doc = "Select IntKey as No, Lab as Name From " + BP.Sys.Base.Glo.SysEnum() + " Where EnumKey='" + uiBindKey + "' AND IntKey>=@Key*100 AND IntKey< (@Key*100/#100)";
                ext.setFK_MapData(this.FK_MapData);
                ext.AttrOfOper = this.KeyOfEn;
                ext.ExtType = "ActiveDDL";
                ext.Insert();
            }
            return this.FK_MapData + "_" + this.KeyOfEn;
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
                return "err@审核角色不能为空";

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

            MapFrmFool md = new MapFrmFool(this.FK_MapData);
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
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.GroupID, this.GetRequestValInt("GroupField"));
            foreach (MapAttr attr in mattrs)
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
                    str = this.GetRequestVal("FrmID");
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

                    _STable = this.GetRequestVal("STable");// context.Request.QueryString["STable"];
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

            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            DataTable tables = src.GetTables();
            tables.TableName = "tables";
            ds.Tables.Add(tables);

            DataTable tableColumns = src.GetColumns(this.STable);
            tableColumns.TableName = "columns";
            ds.Tables.Add(tableColumns);

            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            ds.Tables.Add(attrs.ToDataTableField("attrs"));
            DataTable dt = new DataTable();
            dt.TableName = "STable";
            dt.Columns.Add("STable");
            DataRow dr = dt.NewRow();
            dr["STable"] = this.STable;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 列
        /// </summary>
        private List<string> sCols = null;
        /// <summary>
        /// 列
        /// </summary>
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
        /// <summary>
        /// 导入步骤
        /// </summary>
        /// <returns></returns>
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

            string msg = md.Name + "导入字段信息:" + this.FK_MapData;
            bool isLeft = true;
            // float maxEnd = md.MaxEnd;

            Int32 iGroupID = 0;

            Paras ps = new Paras();
            ps.SQL = "SELECT OID FROM Sys_GroupField WHERE FrmID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FrmID and (CtrlType is null or CtrlType ='') ORDER BY OID DESC ";
            ps.Add("FrmID", this.FK_MapData);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt != null && dt.Rows.Count > 0)
            {
                iGroupID = Int32.Parse(dt.Rows[0][0].ToString());
            }

            foreach (string name in HttpContextHelper.RequestParamKeys)
            {
                if (name.StartsWith("HID_Idx_") == false)
                    continue;

                string columnName = name.Substring("HID_Idx_".Length);

                MapAttr ma = new MapAttr();
                ma.setKeyOfEn(columnName);
                ma.setFK_MapData(this.FK_MapData);
                ma.setMyPK(this.FK_MapData + "_" + ma.KeyOfEn);
                if (ma.IsExits)
                {
                    msg += "\t\n字段:" + ma.KeyOfEn + " - " + ma.Name + "已存在.";
                    continue;
                }

                ma.Name = this.GetValFromFrmByKey("TB_Desc_" + columnName);
                if (DataType.IsNullOrEmpty(ma.Name))
                    ma.Name = ma.KeyOfEn;

                ma.MyDataType = this.GetValIntFromFrmByKey("DDL_DBType_" + columnName);

                //翻译过去.
                string len = this.GetValFromFrmByKey("TB_Len_" + columnName);
                if (len.Equals("null") || DataType.IsNullOrEmpty(len) == true)
                    len = "20";

                try
                {
                    int mylen = int.Parse(len);
                    if (mylen > 4000)
                    {
                        mylen = 0;
                        //ma.isu = true;
                    }


                    ma.setMaxLen(mylen);
                }
                catch (Exception)
                {
                    ma.setMaxLen(0);
                    //throw new Exception("err@转化为最大长度的时候错误:" + ma.KeyOfEn + " len:" + len);
                }

                ma.UIBindKey = this.GetValFromFrmByKey("TB_BindKey_" + columnName);
                ma.LGType = BP.En.FieldTypeS.Normal;
                ma.GroupID = iGroupID;

                //绑定了外键或者枚举.
                if (DataType.IsNullOrEmpty(ma.UIBindKey) == false)
                {
                    SysEnums se = new SysEnums();
                    se.Retrieve(SysEnumAttr.EnumKey, ma.UIBindKey);
                    if (se.Count > 0)
                    {
                        ma.setMyDataType(DataType.AppInt);
                        ma.LGType = BP.En.FieldTypeS.Enum;
                        ma.setUIContralType(BP.En.UIContralType.DDL);
                    }
                    SFTable tb = new SFTable();
                    tb.No = ma.UIBindKey;
                    if (tb.IsExits == true)
                    {
                        ma.setMyDataType(DataType.AppString);
                        ma.LGType = BP.En.FieldTypeS.FK;
                        ma.setUIContralType(BP.En.UIContralType.DDL);
                    }
                }

                if (ma.MyDataType == DataType.AppBoolean)
                    ma.UIContralType = BP.En.UIContralType.CheckBok;

                ma.Insert();

                msg += "\t\n字段:" + ma.KeyOfEn + " - " + ma.Name + "加入成功.";


                isLeft = !isLeft;
            }

            //更新名称.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name=KeyOfEn WHERE Name=NULL OR Name='' ");
            return msg;
        }
        /// <summary>
        /// 框架信息.
        /// </summary>
        /// <returns></returns>
        public string MapFrame_Init()
        {
            MapFrame mf = new MapFrame();
            mf.setFK_MapData(this.FK_MapData);

            if (this.MyPK == null)
            {
                mf.URL = "http://citydo.com.cn";
                mf.W = 400;
                mf.H = 300;
                mf.setName("我的框架.");
                mf.setFK_MapData(this.FK_MapData);
                mf.setMyPK(DBAccess.GenerGUID());
            }
            else
            {
                mf.setMyPK(this.MyPK);
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
            mf = BP.Pub.PubClass.CopyFromRequestByPost(mf) as MapFrame;
            mf.setFK_MapData(this.FK_MapData);

            mf.Save(); //执行保存.
            return "保存成功..";
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
            DataTable dt = null;
            SFTables ens = new SFTables();
            //获取关键字
            string Key = this.GetRequestVal("Key");
            //如果关键之不为空，直接查询
            if (string.IsNullOrWhiteSpace(Key))
            {
                ens.RetrieveAll();
                dt = ens.ToDataTableField("SFTables");
                ds.Tables.Add(dt);
            }//如果关键字为空，按条件查询
            else
            {
                QueryObject ob = new QueryObject(ens);
                // 查询条件示例：where No like '%Key%' or Name like '%Key%'
                ob.AddWhere(SFTableAttr.No, "like", "%" + Key + "%");
                ob.addOr();
                ob.AddWhere(SFTableAttr.Name, "like", "%" + Key + "%");
                //根据No 排序
                ob.addOrderBy(SFTableAttr.No);
                //返回对象为dt
                dt = ob.DoQueryToTable();
                dt.TableName = "SFTables";
                ds.Tables.Add(dt);
            }

            int pTableModel = 0;
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
                    if (md.RetrieveFromDBSources() == 1)
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
            attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);
            if (attr.RetrieveFromDBSources() != 0)
                return "err@字段名[" + this.KeyOfEn + "]已经存在.";

            BP.Sys.CCFormAPI.SaveFieldSFTable(this.FK_MapData, this.KeyOfEn, null, this.GetRequestVal("SFTable"), 100, 100, 1);

            attr.Retrieve();
            Paras ps = new Paras();
            ps.SQL = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FrmID AND (CtrlType='' OR CtrlType IS NULL) ORDER BY OID DESC ";
            ps.Add("FrmID", this.FK_MapData);
            attr.GroupID = DBAccess.RunSQLReturnValInt(ps, 0);
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
            attr.setKeyOfEn(this.KeyOfEn);
            attr.setFK_MapData(this.FK_MapData);

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.setMyPK(this.MyPK);
                attr.RetrieveFromDBSources();
            }
            else
            {
                SFTable sf = new SFTable(this.FK_SFTable);
                attr.Name = sf.Name;
                attr.setKeyOfEn(sf.No);
            }

            //第1次加载.
            attr.setUIContralType(UIContralType.DDL);

            attr.setFK_MapData(this.FK_MapData);

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
            DataTable dt = DBAccess.GetTableSchema(ptable, false);

            //创建样本.
            DataTable mydt = DBAccess.GetTableSchema(ptable, false);
            mydt.Rows.Clear();

            //获得现有的列..
            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            string flowFiels = ",GUID,PRI,PrjNo,PrjName,PEmp,AtPara,FlowNote,WFSta,PNodeID,FK_FlowSort,FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,PWorkID,PFlowNo,BillNo,ProjNo,";

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
            attr.setFK_MapData(frmID);
            attr.setKeyOfEn(keyOfEn);
            attr.setMyPK(attr.FK_MapData + "_" + keyOfEn);
            if (attr.IsExits)
                return "err@该字段[" + keyOfEn + "]已经加入里面了.";

            attr.Name = name;
            attr.MyDataType = dataType;

            if (DataType.AppBoolean == dataType)
                attr.setUIContralType(UIContralType.CheckBok);
            else
                attr.setUIContralType(UIContralType.TB);

            //Paras ps = new Paras();
            //ps.SQL = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FrmID AND  ( CtrlType='' OR CtrlType= NULL ) ";
            // ps.Add("FrmID", this.FK_MapData);
            string sql = "SELECT OID FROM Sys_GroupField A WHERE A.FrmID='" + this.FK_MapData + "' AND (CtrlType='' OR CtrlType= NULL) ";
            attr.GroupID = DBAccess.RunSQLReturnValInt(sql, 0);
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
            if (no == "No")
                no = "No1";

            string name = this.GetRequestVal("name");
            string newNo = DataType.ParseStringForNo(no, 0);
            string newName = DataType.ParseStringForName(name, 0);
            int fType = int.Parse(this.GetRequestVal("FType"));
            bool isSupperText = this.GetRequestValBoolen("IsSupperText");

            MapAttrs attrs = new MapAttrs();
            int i = attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, newNo);
            if (i != 0)
                return "err@字段名：" + newNo + "已经存在.";

            #region 计算GroupID  需要翻译
            int iGroupID = this.GroupField;
            try
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT OID FROM Sys_GroupField WHERE FrmID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FrmID and (CtrlType is null or CtrlType ='') ORDER BY OID DESC ";
                ps.Add("FrmID", this.FK_MapData);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
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
            attr.setKeyOfEn(newNo);
            attr.setFK_MapData(this.FK_MapData);
            attr.setLGType(FieldTypeS.Normal);
            attr.setMyPK(this.FK_MapData + "_" + newNo);
            attr.GroupID = iGroupID;
            attr.MyDataType = fType;

            if (DataType.IsNullOrEmpty(this.GetRequestVal("FK_Flow")) == false)
                attr.SetPara("FK_Flow", this.GetRequestVal("FK_Flow"));

            int colspan = attr.ColSpan;
            attr.Para_FontSize = 12;
            int rows = attr.UIRows;

            if (attr.MyDataType == DataType.AppString)
            {

                UIContralType uiContralType = (UIContralType)this.GetRequestValInt("UIContralType");

                attr.UIWidth = 100;
                attr.UIHeight = 23;
                if (uiContralType == UIContralType.SignCheck || uiContralType == UIContralType.FlowBBS)
                {
                    attr.setUIIsEnable(false);
                    attr.setUIVisible(true);
                }
                else
                {
                    attr.setUIVisible(true);
                    attr.setUIIsEnable(true);
                }

                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(uiContralType);
                attr.Insert();

                if (isSupperText == true)
                {
                    Sys.FrmUI.MapAttrString attrString = new Sys.FrmUI.MapAttrString(attr.MyPK);
                    attrString.IsSupperText = true;
                    attrString.Update();
                }
                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&MyPK=" + attr.MyPK;
            }

            if (attr.MyDataType == DataType.AppInt)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppMoney)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setMyDataType(DataType.AppMoney);
                attr.setUIContralType(UIContralType.TB);
                attr.DefVal = "0.00";
                attr.Insert();
                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppFloat)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setMyDataType(DataType.AppFloat);
                attr.setUIContralType(UIContralType.TB);

                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDouble)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.MyDataType = DataType.AppDouble;
                attr.setUIContralType(UIContralType.TB);
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDate)
            {

                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setUIContralType(UIContralType.TB);
                attr.setMyDataType(DataType.AppDate);
                attr.Insert();

                BP.Sys.FrmUI.MapAttrDT dt = new Sys.FrmUI.MapAttrDT();
                dt.setMyPK(attr.MyPK);
                dt.RetrieveFromDBSources();
                dt.Format = 0;
                dt.Update();


                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppDateTime)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setUIContralType(UIContralType.TB);
                attr.setMyDataType(DataType.AppDateTime);
                attr.Insert();

                BP.Sys.FrmUI.MapAttrDT dt = new Sys.FrmUI.MapAttrDT();
                dt.setMyPK(attr.MyPK);
                dt.RetrieveFromDBSources();
                dt.Format = 1;
                dt.Update();

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            if (attr.MyDataType == DataType.AppBoolean)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.ColSpan = 1;
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.setUIContralType(UIContralType.CheckBok);
                attr.setMyDataType(DataType.AppBoolean);
                attr.DefVal = "0";
                attr.Insert();

                return "url@../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + newNo + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupField=" + this.GroupField;
            }

            return "err@没有判断的字段数据类型." + attr.MyDataTypeStr;
        }
        /// <summary>
        /// 字段初始化数据.
        /// </summary>
        /// <returns></returns>
        public string EditF_FieldInit()
        {
            MapAttr attr = new MapAttr();
            attr.setKeyOfEn(this.KeyOfEn);
            attr.setFK_MapData(this.FK_MapData);

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.setMyPK(this.MyPK);
                attr.RetrieveFromDBSources();
            }
            else
            {
                attr.GroupID = this.GroupField;
            }

            attr.setFK_MapData(this.FK_MapData);

            //字体大小.
            int size = attr.Para_FontSize;
            if (size == 0)
                attr.Para_FontSize = 12;


            //横跨的列数.
            if (attr.ColSpan == 0)
                attr.ColSpan = 1;

            return attr.ToJson();
        }
        public string FieldInitEnum()
        {
            MapAttr attr = new MapAttr();
            attr.setKeyOfEn(this.KeyOfEn);
            attr.setFK_MapData(this.FK_MapData);

            if (DataType.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.setMyPK(this.MyPK);
                attr.RetrieveFromDBSources();
            }
            else
            {
                SysEnumMain sem = new SysEnumMain(this.EnumKey);
                attr.Name = sem.Name;
                attr.setKeyOfEn(sem.No);
                attr.DefVal = "0";
            }

            //第1次加载.
            if (attr.UIContralType == UIContralType.TB)
                attr.setUIContralType(UIContralType.DDL);

            attr.setFK_MapData(this.FK_MapData);

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
            if (DataType.IsNullOrEmpty(enumKey) == true || enumKey.Equals("null") == true)
            {
                MapAttr ma = new MapAttr(this.MyPK);
                enumKey = ma.UIBindKey;
            }

            SysEnums enums = new SysEnums(enumKey);
            ds.Tables.Add(enums.ToDataTableField("Sys_Enum"));

            //转化成json输出.
            string json = BP.Tools.Json.ToJson(ds);
            // DataType.WriteFile("c:\\FieldInitGroupAndSysEnum.json", json);
            return json;
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
                attr.setKeyOfEn(this.KeyOfEn);
                attr.setFK_MapData(this.FK_MapData);
                if (DataType.IsNullOrEmpty(this.MyPK) == false)
                {
                    attr.setMyPK(this.MyPK);
                    attr.RetrieveFromDBSources();
                }
                else
                {
                    /*判断字段是否存在？*/
                    if (attr.IsExit(MapAttrAttr.KeyOfEn, this.KeyOfEn, MapAttrAttr.FK_MapData, this.FK_MapData) == true)
                        return "err@字段名:" + this.KeyOfEn + "已经存在.";
                }

                attr.setKeyOfEn(this.KeyOfEn);
                attr.setFK_MapData(this.FK_MapData);
                attr.setLGType(FieldTypeS.Enum);
                attr.UIBindKey = this.EnumKey;
                attr.setMyDataType(DataType.AppInt);

                //控件类型.
                attr.setUIContralType(UIContralType.DDL);

                attr.Name = this.GetValFromFrmByKey("TB_Name");
                attr.setKeyOfEn(this.GetValFromFrmByKey("TB_KeyOfEn"));
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
                    attr.setUIIsEnable(false);
                else
                    attr.setUIIsEnable(true);

                //是否可见?
                int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
                if (visable == 0)
                    attr.setUIVisible(false);
                else
                    attr.setUIVisible(true);

                attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);

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
                attr.setKeyOfEn(this.KeyOfEn);
                attr.setFK_MapData(this.FK_MapData);
                if (DataType.IsNullOrEmpty(this.MyPK) == false)
                {
                    attr.setMyPK(this.MyPK);
                    attr.RetrieveFromDBSources();
                }
                else
                {
                    /*判断字段是否存在？*/
                    if (attr.IsExit(MapAttrAttr.KeyOfEn, this.KeyOfEn, MapAttrAttr.FK_MapData, this.FK_MapData) == true)
                        return "err@字段名:" + this.KeyOfEn + "已经存在.";
                }

                attr.setKeyOfEn(this.KeyOfEn);
                attr.setFK_MapData(this.FK_MapData);
                attr.setLGType(FieldTypeS.FK);
                attr.UIBindKey = this.FK_SFTable;
                attr.setMyDataType(DataType.AppString);

                //控件类型.
                attr.setUIContralType(UIContralType.DDL);

                attr.Name = this.GetValFromFrmByKey("TB_Name");
                attr.setKeyOfEn(this.GetValFromFrmByKey("TB_KeyOfEn"));
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
                    attr.setUIIsEnable(false);
                else
                    attr.setUIIsEnable(true);

                //是否可见?
                int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
                if (visable == 0)
                    attr.setUIVisible(false);
                else
                    attr.setUIVisible(true);

                attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);
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
        //public string EditF_Save()
        //{
        //    try
        //    {
        //        //定义变量.
        //        int fType = int.Parse(this.GetRequestVal("FType"));  //字段数据物理类型
        //        FieldTypeS lgType = (FieldTypeS)int.Parse(this.GetRequestVal("LGType")); //逻辑类型.
        //        string uiBindKey = this.GetRequestVal("UIBindKey");// context.Request.QueryString["UIBindKey"];

        //        //赋值.
        //        MapAttr attr = new MapAttr();
        //        attr.setKeyOfEn(this.KeyOfEn);
        //        attr.setFK_MapData(this.FK_MapData);
        //        attr.LGType = lgType; //逻辑类型.
        //        attr.UIBindKey = uiBindKey; //绑定的枚举或者外键.
        //        attr.MyDataType = fType; //物理类型.

        //        if (DataType.IsNullOrEmpty(this.MyPK) == false)
        //        {
        //            attr.setMyPK(this.MyPK);
        //            attr.RetrieveFromDBSources();
        //        }

        //        attr.setFK_MapData(this.FK_MapData);
        //        attr.MyDataType = fType; //数据类型.
        //        attr.Name = this.GetValFromFrmByKey("TB_Name");

        //        attr.setKeyOfEn(this.GetValFromFrmByKey("TB_KeyOfEn"));
        //        attr.ColSpan = this.GetValIntFromFrmByKey("DDL_ColSpan");

        //        if (attr.ColSpan == 0)
        //            attr.ColSpan = 1;

        //        attr.Para_FontSize = this.GetValIntFromFrmByKey("TB_FontSize"); //字体大小.
        //        attr.Para_Tip = this.GetValFromFrmByKey("TB_Tip"); //操作提示.

        //        //默认值.
        //        attr.DefVal = this.GetValFromFrmByKey("TB_DefVal");


        //        //对于明细表就可能没有值.
        //        try
        //        {
        //            //分组.
        //            if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
        //                attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？
        //        }
        //        catch
        //        {

        //        }


        //        //把必填项拿出来，所有字段都可以设置成必填项 杨玉慧
        //        attr.UIIsInput = this.GetValBoolenFromFrmByKey("CB_IsInput");   //是否是必填项.

        //        if (attr.MyDataType == DataType.AppString && lgType == FieldTypeS.Normal)
        //        {
        //            attr.IsRichText = this.GetValBoolenFromFrmByKey("CB_IsRichText"); //是否是富文本？
        //            attr.IsSupperText = this.GetValIntFromFrmByKey("CB_IsSupperText"); //是否是超大文本？


        //            //高度.
        //            attr.UIHeightInt = this.GetValIntFromFrmByKey("DDL_Rows") * 23;

        //            //最大最小长度.
        //            attr.setMaxLen(this.GetValIntFromFrmByKey("TB_MaxLen"));
        //            attr.setMinLen(this.GetValIntFromFrmByKey("TB_MinLen"));

        //            attr.UIWidth = this.GetValIntFromFrmByKey("TB_UIWidth"); //宽度.
        //        }

        //        switch (attr.MyDataType)
        //        {
        //            case DataType.AppInt:
        //            case DataType.AppFloat:
        //            case DataType.AppDouble:
        //            case DataType.AppMoney:
        //                attr.IsSum = this.GetValBoolenFromFrmByKey("CB_IsSum");
        //                break;
        //        }

        //        //获取宽度.
        //        try
        //        {
        //            attr.UIWidth = this.GetValIntFromFrmByKey("TB_UIWidth"); //宽度.
        //        }
        //        catch
        //        {
        //        }


        //        //是否可用？所有类型的属性，都需要。
        //        int isEnable = this.GetValIntFromFrmByKey("RB_UIIsEnable");
        //        if (isEnable == 0)
        //            attr.setUIIsEnable(false);
        //        else
        //            attr.setUIIsEnable(true);

        //        //仅仅对普通类型的字段需要.
        //        if (lgType == FieldTypeS.Normal)
        //        {
        //            //是否可见?
        //            int visable = this.GetValIntFromFrmByKey("RB_UIVisible");
        //            if (visable == 0)
        //                attr.setUIVisible(false);
        //            else
        //                attr.setUIVisible(true);
        //        }

        //        attr.setMyPK(this.FK_MapData + "_" + this.KeyOfEn);
        //        attr.Save();

        //        return "保存成功.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
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
                dtl.setFK_MapData(this.FK_MapData);
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
                    MapAttrs mattrs = new MapAttrs(this.FK_MapDtl);

                    //让其直接保存.
                    dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                    dtl.setFK_MapData("Temp");
                    dtl.DirectInsert(); //生成一个明细表属性的主表.

                    //循环保存字段.
                    int idx = 0;
                    foreach (MapAttr item in mattrs)
                    {
                        item.setFK_MapData(this.FK_MapDtl + "_" + this.FK_Node);
                        item.setMyPK(item.FK_MapData + "_" + item.KeyOfEn);
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
                BP.Pub.PubClass.CopyFromRequest(dtl);

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
            string fileFullName = BP.Difference.SystemConfig.PathOfWebApp + "Temp/" + this.FK_MapData + ".xml";

            HttpContextHelper.ResponseWriteFile(fileFullName, this.FK_MapData + ".xml");
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
            ath.setFK_MapData(this.FK_MapData);
            ath.NoOfObj = this.Ath;
            ath.FK_Node = this.FK_Node;
            if (this.MyPK == null)
            {
                if (this.FK_Node == 0)
                    ath.setMyPK(this.FK_MapData + "_" + this.Ath);
                else
                    ath.setMyPK(this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node);
            }
            else
            {
                ath.setMyPK(this.MyPK);

            }
            int i = ath.RetrieveFromDBSources();
            if (i == 0)
            {
                /*初始化默认值.*/
                ath.NoOfObj = "Ath1";
                ath.Name = "我的附件";
                // ath.SaveTo =  BP.Difference.SystemConfig.PathOfDataUser + "/UploadFile/" + this.FK_MapData + "/";
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
                    ath.setMyPK(this.FK_MapData + "_" + this.Ath);
                else
                    ath.setMyPK(this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node);

                //插入一个新的.
                ath.FK_Node = this.FK_Node;
                ath.setFK_MapData(this.FK_MapData);
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
            ath.setFK_MapData(this.FK_MapData);
            ath.NoOfObj = this.Ath;
            ath.FK_Node = this.FK_Node;
            ath.setMyPK(this.FK_MapData + "_" + this.Ath);

            int i = ath.RetrieveFromDBSources();
            ath = BP.Pub.PubClass.CopyFromRequestByPost(ath) as FrmAttachment;
            if (i == 0)
                ath.Save(); //执行保存.              
            else
                ath.Update();
            return "保存成功..";
        }
        public string Attachment_Delete()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.setMyPK(this.MyPK);
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
                    if (Equals(r["DBSrcType"], DBSrcType.WebServices))
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
            return BP.WF.Difference.WF_Admin_FoolFormDesigner.GetWebServiceMethods(dbsrc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string FrmView_Init()
        {
            string frmID = this.GetRequestVal("FrmID");

            MapData md = new MapData(frmID);

            //获得表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);


            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            GEEntity wk = new GEEntity(FrmID);

            DataTable mainTable = wk.ToDataTableField("MainTable");
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
            #endregion


            #region 增加附件信息.
            BP.Sys.FrmAttachments athDescs = new FrmAttachments();

            athDescs.Retrieve(FrmAttachmentAttr.FK_MapData, this.FrmID);
            if (athDescs.Count != 0)
            {
                FrmAttachment athDesc = athDescs[0] as FrmAttachment;

                //查询出来数据实体.
                BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
                {
                    Paras ps = new Paras();
                    ps.SQL = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add("WorkID", this.WorkID);
                    string pWorkID = DBAccess.RunSQLReturnValInt(ps, 0).ToString();
                    if (pWorkID == null || pWorkID == "0")
                    {
                        pWorkID = this.WorkID.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /* 继承模式 */
                        BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                        qo.addOr();
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                        qo.addOrderBy("RDT");
                        qo.DoQuery();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*共享模式*/
                        dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                    }
                }
                else if (athDesc.HisCtrlWay == AthCtrlWay.WorkID)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                }

                //增加一个数据源.
                myds.Tables.Add(dbs.ToDataTableField("Sys_FrmAttachmentDB").Copy());
            }
            #endregion

            #region 把外键表加入DataSet
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
            DataTable dt = new DataTable();
            MapExts mes = md.MapExts;
            MapExt me = new MapExt();
            DataTable ddlTable = new DataTable();
            ddlTable.Columns.Add("No");
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                string uiBindKey = dr["UIBindKey"].ToString();

                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue; //为空就continue.

                if (lgType.Equals("1") == true)
                    continue; //枚举值就continue;

                string uiIsEnable = dr["UIIsEnable"].ToString();
                if (uiIsEnable.Equals("0") == true && lgType.Equals("1") == true)
                    continue; //如果是外键，并且是不可以编辑的状态.

                if (uiIsEnable.Equals("1") == true && lgType.Equals("0") == true)
                    continue; //如果是外部数据源，并且是不可以编辑的状态.



                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                    dt = DBAccess.RunSQLReturnTable(fullSQL);
                    //重构新表
                    DataTable dt_FK_Dll = new DataTable();
                    dt_FK_Dll.TableName = keyOfEn;//可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                    dt_FK_Dll.Columns.Add("No", typeof(string));
                    dt_FK_Dll.Columns.Add("Name", typeof(string));
                    foreach (DataRow dllRow in dt.Rows)
                    {
                        DataRow drDll = dt_FK_Dll.NewRow();
                        drDll["No"] = dllRow["No"];
                        drDll["Name"] = dllRow["Name"];
                        dt_FK_Dll.Rows.Add(drDll);
                    }
                    myds.Tables.Add(dt_FK_Dll);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                {
                    continue;
                }

                DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                if (mydt == null)
                {
                    DataRow ddldr = ddlTable.NewRow();
                    ddldr["No"] = uiBindKey;
                    ddlTable.Rows.Add(ddldr);
                }
                else
                {
                    myds.Tables.Add(mydt);
                }
            }
            ddlTable.TableName = "UIBindKey";
            myds.Tables.Add(ddlTable);
            #endregion End把外键表加入DataSet

            #region 图片附件
            FrmImgAthDBs imgAthDBs = new FrmImgAthDBs(this.FrmID, this.WorkID.ToString());
            if (imgAthDBs != null && imgAthDBs.Count > 0)
            {
                DataTable dt_ImgAth = imgAthDBs.ToDataTableField("Sys_FrmImgAthDB");
                myds.Tables.Add(dt_ImgAth);
            }
            #endregion

            return BP.Tools.Json.ToJson(myds);
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

        public string MapDataVer_SetMainVer()
        {
            MapDataVer mdVer = new MapDataVer(this.MyPK);
            #region 1.变更主版本
            string sql = "UPDATE Sys_MapDataVer SET IsRel=0 WHERE FrmID='" + mdVer.FrmID + "'";
            DBAccess.RunSQL(sql);

            mdVer.SetValByKey("IsRel", 1);
            mdVer.Update(); //更新.
            #endregion 1.变更主版本

            #region 2. 覆盖之前的主版本数据
            MapData mainData = new MapData(mdVer.FrmID);
            string currVer = mainData.No + "." + mainData.Ver2022.ToString();
            MapData md = new MapData();
            md.No = currVer;
            if (md.RetrieveFromDBSources() == 1)
                md.Delete();
            //把表单属性的FK_FormTree清空
            BP.Sys.CCFormAPI.CopyFrm(mainData.No, currVer, mainData.Name + "(Ver" + currVer + ".0)", mainData.FK_FormTree);
            md.FK_FormTree = "";
            md.PTable = mainData.PTable;
            md.Update();
            //修改从表的存储表
            MapDtls dtls = md.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.PTable.Equals(dtl.No) == true)
                {
                    dtl.PTable = dtl.PTable.Replace(currVer, mainData.No);
                    dtl.Update();
                    continue;
                }
            }
            //把当前表单对应数据改成当前的版本
            DBAccess.RunSQL("UPDATE " + md.PTable + " SET AtPara=CONCAT(AtPara,'@FrmVer=" + mainData.Ver2022 + "') WHERE AtPara NOT LIKE '%@FrmVer=%'");
            #endregion 2. 覆盖之前的主版本数据


            #region 3.把设置的表单数据拷贝到FrmID所在的表单上去
            //FrmID的表单只有不存在其他版本的时候才可以删除，修改版本
            sql = "UPDATE Sys_MapDataVer SET FrmID='" + mdVer.MyPK + "'WHERE FrmID='" + mdVer.FrmID + "'";
            DBAccess.RunSQL(sql);
            //绑定的表单，表单方案的还原
            FrmNodes frmNodes = new FrmNodes();
            frmNodes.Retrieve(FrmNodeAttr.FK_Frm, mainData.No);

            FrmFields frmFields = new FrmFields();
            string whereFK_MapData = "'" + mainData.No + "'";
            dtls = new MapDtls(mainData.No);
            foreach (MapDtl dtl in dtls)
            {
                whereFK_MapData += ",'" + dtl.No + "' ";

            }
            frmFields.RetrieveIn(FrmFieldAttr.FK_MapData, whereFK_MapData);
            mainData.Delete();
            //把表单属性的FK_FormTree清空
            BP.Sys.CCFormAPI.CopyFrm(mainData.No + "." + mdVer.Ver, mainData.No, mainData.Name, mainData.FK_FormTree);
            mainData.FK_FormTree = mainData.FK_FormTree;
            mainData.PTable = mainData.PTable;
            mainData.Ver2022 = mdVer.Ver;
            mainData.Update();
            //修改从表的存储表
            dtls = mainData.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.PTable.Equals(dtl.No) == true)
                {
                    dtl.PTable = dtl.PTable.Replace(mainData.No + "." + mdVer.Ver, mainData.No);
                    dtl.Update();
                    continue;
                }
            }

            sql = "UPDATE Sys_MapDataVer SET FrmID='" + mainData.No + "' WHERE FrmID='" + this.MyPK + "'";
            DBAccess.RunSQL(sql);
            //还原表单方案
            foreach (FrmNode frmNode in frmNodes)
            {
                frmNode.DirectInsert();
            }
            foreach (FrmField frmField in frmFields)
            {
                frmField.DirectInsert();
            }
            #endregion 3.把设置的表单数据拷贝到FrmID所在的表单上去
            return "设置成功.";
        }

        public string SysEnumList_SelectEnum()
        {
            string sql = "";
            string EnumName = this.GetRequestVal("EnumName");
		   if (EnumName == "")
			   sql = "SELECT * FROM Sys_EnumMain";
		   else
			   sql = "SELECT * FROM Sys_EnumMain WHERE (No like '%" + EnumName + "%') OR (Name like '%" + EnumName + "%')";
		   DataTable dt = DBAccess.RunSQLReturnTable(sql);
		   return BP.Tools.Json.ToJson(dt);
	   }

        public string SysEnumList_MapAttrs()
        {
            string sql = "SELECT A.FK_MapData,A.KeyOfEn,A.Name From Sys_MapAttr A,Sys_MapData B Where A.FK_MapData=B.No " +
				       "AND A.UIBindKey='" + this.GetRequestVal("UIBindKey") + "' AND B.OrgNo='" + this.GetRequestVal("OrgNo") + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
		    return BP.Tools.Json.ToJson(dt);
        }

}

    public class WSMethod
    {
        public string No { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> ParaMS { get; set; }

        public string Return { get; set; }
    }
}
