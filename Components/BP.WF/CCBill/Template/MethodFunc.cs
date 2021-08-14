using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 功能执行
    /// </summary>
    public class MethodFunc : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
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
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.Tag1);
            }
            set
            {
                this.SetValByKey(MethodAttr.Tag1, value);
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
                string file = SystemConfig.CCFlowAppPath + "WF\\CCBill\\Admin\\MethodDoc\\MethodDocDemoJS.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("/#", "+"); //为什么？
                doc = doc.Replace("/$", "-"); //为什么？

                doc = doc.Replace("@FrmID", this.FrmID);

                return doc;
            }
        }
        public string MethodDoc_SQL_Demo
        {
            get
            {
                string file = SystemConfig.CCFlowAppPath + "WF\\CCBill\\Admin\\MethodDoc\\MethodDocDemoSQL.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("@FrmID", this.FrmID);
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
            MapAttrs attrs = new MapAttrs(this.No);
            foreach (MapAttr item in attrs)
            {
                paras += item.KeyOfEn + ",";
            }
            if (attrs.Count > 1)
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
        /// 功能执行
        /// </summary>
        public MethodFunc()
        {
        }
        public MethodFunc(string mypk)
        {
            this.No = mypk;
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

                Map map = new Map("Frm_Method", "功能方法");


                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
               // map.AddTBString(MethodAttr.Mark, null, "Mark", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);

                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                map.AddTBString(MethodAttr.Mark, null, "功能说明", true, false, 0, 900, 10, true);
                map.SetHelperAlert(MethodAttr.Mark, "对于该功能的描述.");


                //是否显示到列表.
                map.AddBoolean(MethodAttr.IsList, false, "是否显示在列表?", true, true);


                // map.AddTBStringDoc(MethodAttr.Docs, null, "功能说明", true, false, true);

                //    map.AddDDLSysEnum(MethodAttr.WhatAreYouTodo, 0, "执行完毕后干啥？", true, true, MethodAttr.WhatAreYouTodo,
                //   "@0=关闭提示窗口@1=关闭提示窗口并刷新@2=转入到Search.htm页面上去");

                map.AddTBString(MethodAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10, true);
                //map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", true, true, MethodAttr.ShowModel,
                //  "@0=按钮@1=超链接");

                map.AddDDLSysEnum(MethodAttr.MethodDocTypeOfFunc, 0, "内容类型", true, false, "MethodDocTypeOfFunc",
               "@0=SQL@1=URL@2=JavaScript@3=业务单元");

                map.AddTBString(MethodAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);

                #region 外观.
                //  map.AddTBInt(MethodAttr.PopHeight, 100, "弹窗高度", true, false);
                //    map.AddTBInt(MethodAttr.PopWidth, 260, "弹窗宽度", true, false);
                #endregion 外观.

                #region 显示位置控制.
                // map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                //  map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                //  map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.

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
            return "../../CCBill/Admin/MethodDoc/Default.htm?No=" + this.No;
        }
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        #endregion 执行方法.

    }
    /// <summary>
    /// 功能执行
    /// </summary>
    public class MethodFuncs : EntitiesNoName
    {
        /// <summary>
        /// 功能执行
        /// </summary>
        public MethodFuncs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodFunc();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodFunc> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodFunc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodFunc> Tolist()
        {
            System.Collections.Generic.List<MethodFunc> list = new System.Collections.Generic.List<MethodFunc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodFunc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
