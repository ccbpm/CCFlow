using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.Frm
{
    
	/// <summary>
	/// 表单方法属性
	/// </summary>
    public class MethodAttr:EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string MethodID = "MethodID";
        /// <summary>
        /// 方法名
        /// </summary>
        public const string MethodName = "MethodName";
        /// <summary>
        /// 方法类型
        /// </summary>
        public const string RefMethodType = "RefMethodType";
        /// <summary>
        /// 显示方式.
        /// </summary>
        public const string ShowModel = "ShowModel";
        /// <summary>
        /// 处理内容
        /// </summary>
        public const string MethodDoc_Url = "MethodDoc_Url";
        /// <summary>
        /// 方法的内容类型
        /// </summary>
        public const string MethodDocTypeOfFunc = "MethodDocTypeOfFunc";
        /// <summary>
        /// 处理内容 tag.
        /// </summary>
        public const string Idx = "Idx";
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
    }
	/// <summary>
	/// 表单方法
	/// </summary>
    public class Method : EntityMyPK
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
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodName);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodName, value);
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
        /// 表单方法
        /// </summary>
        public Method()
        {
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

                Map map = new Map("Frm_Method", "表单方法");

                map.AddMyPK();

                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodName, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10);

                map.AddDDLSysEnum(MethodAttr.RefMethodType, 0, "方法类型", true, false, MethodAttr.RefMethodType,
                    "@0=功能@1=模态窗口打开@2=新窗口打开@3=右侧窗口打开@4=实体集合的功能");

                #region 显示位置控制.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.

                #region 外观.
                map.AddTBInt(MethodAttr.PopHeight, 0, "弹窗高度", true, false);
                map.AddTBInt(MethodAttr.PopWidth, 0, "弹窗宽度", true, false);
                #endregion 外观.


                //对功能有效.
                map.AddTBString(MethodAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(MethodAttr.WhatAreYouTodo, 0, "执行完毕后干啥？", true, true, MethodAttr.WhatAreYouTodo,
                "@0=关闭提示窗口@1=关闭提示窗口并刷新@2=转入到Search.htm页面上去");

                map.AddTBInt(MethodAttr.Idx, 0, "Idx", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public void DoUp()
        {
            this.DoOrderUp(MethodAttr.FrmID, this.FrmID, MethodAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(MethodAttr.FrmID, this.FrmID, MethodAttr.Idx);
        }

       
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
         
    }
	/// <summary>
	/// 表单方法
	/// </summary>
    public class Methods : EntitiesMyPK
    {
        /// <summary>
        /// 表单方法
        /// </summary>
        public Methods() { }
        /// <summary>
        /// 表单方法
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
