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
    /// 实体方法属性
    /// </summary>
    public class MethodAttr : EntityNoNameAttr
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";

        /// <summary>
        /// 分组ID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string MethodID = "MethodID";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// 方法类型
        /// </summary>
        public const string RefMethodType = "RefMethodType";
        /// <summary>
        /// 方法打开模式
        /// </summary>
        public const string MethodModel = "MethodModel";
        /// <summary>
        /// 标记
        /// </summary>
        public const string Mark = "Mark";
        /// <summary>
        /// tag
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 显示方式.
        /// </summary>
        public const string ShowModel = "ShowModel";
        /// <summary>
        /// 处理内容
        /// </summary>
        public const string MethodDoc_Url = "MethodDoc_Url";
        /// <summary>
        /// 内容类型
        /// </summary>
        public const string MethodDocTypeOfFunc = "MethodDocTypeOfFunc";
        /// <summary>
        /// 处理内容s
        /// </summary>
        public const string Docs = "Docs";

        /// <summary>
        /// 执行警告信息-对功能方法有效
        /// </summary>
        public const string WarningMsg = "WarningMsg";
        /// <summary>
        /// 成功提示信息
        /// </summary>
        public const string MsgSuccess = "MsgSuccess";
        /// <summary>
        /// 失败提示信息
        /// </summary>
        public const string MsgErr = "MsgErr";
        /// <summary>
        /// 执行完毕后干啥？
        /// </summary>
        public const string WhatAreYouTodo = "WhatAreYouTodo";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        #endregion 基本属性.

        #region 外观.
        /// <summary>
        /// 宽度.
        /// </summary>
        public const string PopWidth = "PopWidth";
        /// <summary>
        /// 高度
        /// </summary>
        public const string PopHeight = "PopHeight";
        #endregion 外观.

        #region 显示位置
        /// <summary>
        /// 是否显示myToolBar工具栏上.
        /// </summary>
        public const string IsMyBillToolBar = "IsMyBillToolBar";
        /// <summary>
        /// 显示在工具栏更多按钮里.
        /// </summary>
        public const string IsMyBillToolExt = "IsMyBillToolExt";
        /// <summary>
        /// 显示在查询列表工具栏目上，用于执行批处理.
        /// </summary>
        public const string IsSearchBar = "IsSearchBar";
        #endregion 显示位置

        #region 流程方法相关.
        /// <summary>
        /// 流程结束后是否同步字段?
        /// </summary>
        public const string DTSDataWay = "DTSDataWay";
        public const string DTSSpecFiels = "DTSSpecFiels";
        public const string DTSWhenFlowOver = "DTSWhenFlowOver";
        public const string DTSWhenNodeOver = "DTSWhenNodeOver";
        public const string FlowNo = "FlowNo";
        #endregion 流程方法相关.

        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 是否显示在列表？
        /// </summary>
        public const string IsList = "IsList";
        /// <summary>
        /// 是否含有参数
        /// </summary>
        public const string IsHavePara = "IsHavePara";
    }
    /// <summary>
    /// 实体方法
    /// </summary>
    public class Method : EntityNoName
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
        /// 方法分组ID
        /// </summary>
        public string GroupID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.GroupID);
            }
            set
            {
                this.SetValByKey(MethodAttr.GroupID, value);
            }
        }
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.Icon);
            }
            set
            {
                this.SetValByKey(MethodAttr.Icon, value);
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

        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(MethodAttr.FlowNo, value);
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
        /// <summary>
        /// 模式
        /// </summary>
        public string MethodModel
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodModel);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodModel, value);
            }
        }
        /// <summary>
        /// 标记
        /// </summary>
        public string Mark
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.Mark);
            }
            set
            {
                this.SetValByKey(MethodAttr.Mark, value);
            }
        }
        /// <summary>
        /// tag1
        /// </summary>
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
        #endregion

        #region 构造方法
        /// <summary>
        /// 实体方法
        /// </summary>
        public Method()
        {
        }
        /// <summary>
        /// 实体方法
        /// </summary>
        /// <param name="no"></param>
        public Method(string no)
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

                Map map = new Map("Frm_Method", "实体方法");

                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Mark, null, "Mark", true, true, 0, 300, 10);


                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.FlowNo, null, "流程编号", true, true, 0, 10, 10);

                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                ////批处理的方法，显示到集合上.
                //map.AddBoolean(MethodAttr.IsCanBatch, false, "是否可以批处理?", true, false);

                //临时存储.
                map.AddTBString(MethodAttr.Docs, null, "方法内容", true, false, 0, 300, 10);

                map.AddDDLSysEnum(MethodAttr.RefMethodType, 0, "方法类型", true, false, MethodAttr.RefMethodType,
                "@0=功能@1=模态窗口打开@2=新窗口打开@3=右侧窗口打开");

                #region 显示位置控制.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.

                #region 外观.
                map.AddTBInt(MethodAttr.PopHeight, 0, "弹窗高度", true, false);
                map.AddTBInt(MethodAttr.PopWidth, 0, "弹窗宽度", true, false);
                #endregion 外观.

                #region 对功能有效
                //对功能有效.
                map.AddTBString(MethodAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(MethodAttr.WhatAreYouTodo, 0, "执行完毕后干啥？", true, true, MethodAttr.WhatAreYouTodo,
                "@0=关闭提示窗口@1=关闭提示窗口并刷新@2=转入到Search.htm页面上去");
                map.AddDDLSysEnum(MethodAttr.MethodDocTypeOfFunc, 0, "内容类型", true, false, "MethodDocTypeOfFunc",
                "@0=SQL@1=URL@2=JavaScript@3=业务单元");
                #endregion 对功能有效

                #region (流程)相同字段数据同步方式.
                map.AddDDLSysEnum(MethodAttr.DTSDataWay, 0, "同步相同字段数据方式", true, true, MethodAttr.DTSDataWay,
               "@0=不同步@1=同步全部的相同字段的数据@2=同步指定字段的数据");

                map.AddTBString(MethodAttr.DTSSpecFiels, null, "要同步的字段", true, false, 0, 300, 10, true);

                map.AddBoolean(MethodAttr.DTSWhenFlowOver, false, "流程结束后同步？", true, true, true);
                map.AddBoolean(MethodAttr.DTSWhenNodeOver, false, "节点发送成功后同步？", true, true, true);
                #endregion (流程)相同字段数据同步方式.

                //是否启用？
                map.AddTBInt(MethodAttr.IsEnable, 1, "是否启用？", true, true);
                map.AddTBInt(MethodAttr.IsList, 0, "是否显示在列表?", true, false);
                map.AddTBInt(MethodAttr.IsHavePara, 0, "是否含有参数?", true, false);
                map.AddTBInt(MethodAttr.Idx, 0, "Idx", true, false);
                
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 移动.
        public void DoUp()
        {
            this.DoOrderUp(MethodAttr.FrmID, this.FrmID, MethodAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(MethodAttr.FrmID, this.FrmID, MethodAttr.Idx);
        }
        #endregion 移动.

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 实体方法
    /// </summary>
    public class Methods : EntitiesNoName
    {
        /// <summary>
        /// 实体方法
        /// </summary>
        public Methods() { }
        /// <summary>
        /// 实体方法
        /// </summary>
        /// <param name="nodeid">方法IDID</param>
        public Methods(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MethodAttr.MethodID, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Method();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Method> ToJavaList()
        {
            return (System.Collections.Generic.IList<Method>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Method> Tolist()
        {
            System.Collections.Generic.List<Method> list = new System.Collections.Generic.List<Method>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Method)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
