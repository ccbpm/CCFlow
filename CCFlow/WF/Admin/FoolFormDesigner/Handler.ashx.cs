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
            return int.Parse(this.GetValFromFrmByKey(key));
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
        #endregion 属性.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "DtlInit": //初始化明细表.
                        msg = this.DtlInit();
                        break;
                    case "DtlSave": //保存明细表.
                        msg = this.DtlSave();
                        break;
                    case "DtlAttrs":
                        msg = this.DtlAttrs();
                        break;
                    case "FieldSaveSFTable": //保存外键字段..
                        msg = this.FieldSaveSFTable();
                        break;
                    case "FieldInitSFTable": //初始化枚举信息.
                        msg = this.FieldInitSFTable();
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
                    case "FieldInit": //字段属性.
                        msg = this.FieldInit();
                        break;
                    case "FieldSave": //保存字段.
                        msg = this.FieldSave();
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
                        attrU.DoUp();
                        msg = "";
                        break;
                    case "DtlFieldDown": //字段下移.
                        MapAttr attrD = new MapAttr(this.MyPK);
                        attrD.DoDown();
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
        public string FieldInitSFTable()
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
            string groupIDStr = this.context.Request.QueryString["GroupID"];
            if (groupIDStr == null || groupIDStr == "")
                groupIDStr = "0";
            int groupID = int.Parse(groupIDStr);

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
            attr.GroupID = groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + attr.MyDataType + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDate + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDateTime + "&DoType=Edit&GroupID=" + groupID;
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
                return "url@EditF.htm?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppBoolean + "&DoType=Edit&GroupID=" + groupID;
            }

            return "err@没有判断的数据类型." + attr.MyDataTypeStr;
        }
        /// <summary>
        /// 字段初始化数据.
        /// </summary>
        /// <returns></returns>
        public string FieldInit()
        {
             MapAttr attr = new MapAttr();
            attr.KeyOfEn = this.KeyOfEn;
            attr.FK_MapData = this.FK_MapData;

            if (string.IsNullOrEmpty(this.MyPK) == false)
            {
                attr.MyPK = this.MyPK;
                attr.RetrieveFromDBSources();
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
            BP.DA.DataType.WriteFile("c:\\FieldInitGroupAndSysEnum.json", json);
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

                //分组.
                if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                    attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？

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
        /// 保存枚举
        /// </summary>
        /// <returns></returns>
        public string FieldSaveSFTable()
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

                //分组.
                if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                    attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？

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
        public string FieldSave()
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

                //默认值.
                attr.DefVal = this.GetValFromFrmByKey("TB_DefVal");

                //分组.
                if (this.GetValIntFromFrmByKey("DDL_GroupID") != 0)
                    attr.GroupID = this.GetValIntFromFrmByKey("DDL_GroupID"); //在那个分组里？

                if (attr.MyDataType == BP.DA.DataType.AppString && lgType == FieldTypeS.Normal)
                {
                    attr.UIIsInput = this.GetValBoolenFromFrmByKey("CB_IsInput");   //是否是必填项.
                    attr.IsRichText = this.GetValBoolenFromFrmByKey("CB_IsRichText"); //是否是富文本？
                    attr.IsSupperText = this.GetValBoolenFromFrmByKey("CB_IsSupperText"); //是否是超大文本？

                    //高度.
                    attr.UIHeightInt = this.GetValIntFromFrmByKey("DDL_Rows") * 23;

                    //最大最小长度.
                    attr.MaxLen = this.GetValIntFromFrmByKey("TB_MaxLen");
                    attr.MinLen = this.GetValIntFromFrmByKey("TB_MinLen");
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
    }
}