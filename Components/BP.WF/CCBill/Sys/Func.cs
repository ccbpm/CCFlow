using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCBill.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class FuncAttr : MethodAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 功能ID
        /// </summary>
        public const string FuncID = "FuncID";
        /// <summary>
        /// 功能来源
        /// </summary>
        public const string FuncSrc = "FuncSrc";
        /// <summary>
        /// 功能内容
        /// </summary>
        public const string DTSName = "DTSName";
    }
    /// <summary>
    /// 独立方法
    /// </summary>
    public class Func : EntityNoName
    {
        #region 基本属性

        /// <summary>
        /// 方法ID - 不是主键
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodID, value);
            }
        }

        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgErr, value);
            }
        }
        public string MsgSuccess
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgSuccess);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgSuccess, value);
            }
        }
        public string MethodDoc_Url
        {
            get
            {

                string s = this.GetValStringByKey(MethodAttr.MethodDoc_Url);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "http://192.168.0.100/MyPath/xxx.xx";
                return s;
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDoc_Url, value);
            }
        }
        /// <summary>
        /// 获得或者设置sql脚本.
        /// </summary>
        public string MethodDoc_SQL
        {
            get
            {
                string strs = this.GetBigTextFromDB("SQLScript");
                if (DataType.IsNullOrEmpty(strs) == true)
                    return this.MethodDoc_SQL_Demo; //返回默认信息.
                return strs;
            }
            set
            {
                this.SaveBigTxtToDB("SQLScript", value);
            }
        }
        /// <summary>
        /// 获得该实体的demo.
        /// </summary>
        public string MethodDoc_JavaScript_Demo
        {
            get
            {
                string file = SystemConfig.CCFlowAppPath + "WF/CCBill/Admin/MethodDoc/MethodDocDemoJS.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("/#", "+"); //为什么？
                doc = doc.Replace("/$", "-"); //为什么？

                //  doc = doc.Replace("@FrmID", this.FrmID);

                return doc;
            }
        }
        public string MethodDoc_SQL_Demo
        {
            get
            {
                string file = SystemConfig.CCFlowAppPath + "WF/CCBill/Admin/MethodDoc/MethodDocDemoSQL.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                                                          //  doc = doc.Replace("@FrmID", this.FrmID);
                return doc;
            }
        }
        /// <summary>
        /// 获得JS脚本.
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_JavaScript()
        {
            return this.MethodDoc_JavaScript;
        }

        public string Gener_MethodDoc_JavaScript_function()
        {
            string paras = "";
            MapAttrs mattrs = new MapAttrs(this.No);
            foreach (MapAttr item in mattrs)
            {
                paras += item.KeyOfEn + ",";
            }
            if (mattrs.Count > 1)
                paras = paras.Substring(0, paras.Length - 1);

            string strs = " function " + this.MethodID + "(" + paras + ") {";
            strs += this.MethodDoc_JavaScript;
            strs += "}";
            return strs;
        }
        /// <summary>
        /// 获得SQL脚本
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_SQL()
        {
            return this.MethodDoc_SQL;
        }
        /// <summary>
        /// 获得或者设置js脚本.
        /// </summary>
        public string MethodDoc_JavaScript
        {
            get
            {
                string strs = this.GetBigTextFromDB("JSScript");
                if (DataType.IsNullOrEmpty(strs) == true)
                    return this.MethodDoc_JavaScript_Demo;

                strs = strs.Replace("/#", "+");
                strs = strs.Replace("/$", "-");
                return strs;
            }
            set
            {

                this.SaveBigTxtToDB("JSScript", value);

            }
        }

        /// <summary>
        /// 方法类型：@0=SQL@1=URL@2=JavaScript@3=业务单元
        /// </summary>
        public int MethodDocTypeOfFunc
        {
            get
            {
                return this.GetValIntByKey(MethodAttr.MethodDocTypeOfFunc);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDocTypeOfFunc, value);
            }
        }
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(MethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(MethodAttr.RefMethodType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 独立方法
        /// </summary>
        public Func()
        {
        }
        public Func(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Func", "功能");

                map.AddTBStringPK(FuncAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(FuncAttr.Name, null, "方法名", true, false, 0, 300, 10, true);

                map.AddTBString(FuncAttr.FuncID, null, "方法ID", true, false, 0, 300, 10, true);
                map.AddTBString(FuncAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(FuncAttr.FuncSrc, 0, "功能来源", true, false, "FuncSrc",
              "@0=自定义@1=系统内置");
                map.AddTBString(FuncAttr.DTSName, null, "功能内容", true, false, 0, 300, 10, true);



                map.AddTBStringDoc(FuncAttr.Docs, null, "功能说明", true, false, true);
                map.SetHelperAlert(FuncAttr.Docs, "对于该功能的描述.");

                map.AddTBString(FuncAttr.WarningMsg, null, "独立方法警告信息", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(FuncAttr.MethodDocTypeOfFunc, 0, "内容类型", true, false, "MethodDocTypeOfFunc",
               "@0=SQL@1=URL@2=JavaScript@3=业务单元");

                map.AddTBString(FuncAttr.MethodDoc_Url, null, "URL执行内容", false, false, 0, 300, 10);
                map.AddTBString(FuncAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(FuncAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);
                map.AddTBInt(FuncAttr.IsHavePara, 0, "是否含有参数?", true, false);

                RefMethod rm = new RefMethod();
                //rm.Title = "方法参数"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoParas";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "方法内容"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDocs";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoParas()
        {
            return "../../CCBill/Admin/MethodParas.htm?No=" + this.No;
        }
        /// <summary>
        /// 方法内容
        /// </summary>
        /// <returns></returns>
        public string DoDocs()
        {
            return "../../CCBill/Admin/MethodDocSys/Default.htm?No=" + this.No;
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 独立方法s
    /// </summary>
    public class Funcs : EntitiesNoName
    {
        /// <summary>
        /// 独立方法
        /// </summary>
        public Funcs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Func();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Func> ToJavaList()
        {
            return (System.Collections.Generic.IList<Func>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Func> Tolist()
        {
            System.Collections.Generic.List<Func> list = new System.Collections.Generic.List<Func>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Func)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
