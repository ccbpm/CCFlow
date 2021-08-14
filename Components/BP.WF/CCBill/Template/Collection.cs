using System;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 集合方法属性
    /// </summary>
    public class CollectionAttr : EntityNoNameAttr
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string MethodID = "MethodID";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";
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
        public const string CollectionDoc_Url = "CollectionDoc_Url";
        /// <summary>
        /// 内容类型
        /// </summary>
        public const string CollectionDocTypeOfFunc = "CollectionDocTypeOfFunc";
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

        public const string IsEnable = "IsEnable";

        public const string FlowNo = "FlowNo";

    }
    /// <summary>
    /// 集合方法
    /// </summary>
    public class Collection : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.FrmID);
            }
            set
            {
                this.SetValByKey(CollectionAttr.FrmID, value);
            }
        }
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.Icon);
            }
            set
            {
                this.SetValByKey(CollectionAttr.Icon, value);
            }
        }

        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.MethodID);
            }
            set
            {
                this.SetValByKey(CollectionAttr.MethodID, value);
            }
        }

        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(CollectionAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 模式
        /// </summary>
        public string MethodModel
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.MethodModel);
            }
            set
            {
                this.SetValByKey(CollectionAttr.MethodModel, value);
            }
        }
        /// <summary>
        /// 标记
        /// </summary>
        public string Mark
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.Mark);
            }
            set
            {
                this.SetValByKey(CollectionAttr.Mark, value);
            }
        }
        /// <summary>
        /// tag1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.Tag1);
            }
            set
            {
                this.SetValByKey(CollectionAttr.Tag1, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 集合方法
        /// </summary>
        public Collection()
        {
        }
        /// <summary>
        /// 集合方法
        /// </summary>
        /// <param name="no"></param>
        public Collection(string no)
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

                Map map = new Map("Frm_Collection", "集合方法");

                //主键.
                map.AddTBStringPK(CollectionAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(CollectionAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(CollectionAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);

                //功能标记. 
                map.AddTBString(CollectionAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.Mark, null, "Mark", true, true, 0, 300, 10);

                map.AddTBString(CollectionAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.FlowNo, null, "流程编号", true, true, 0, 10, 10);

                map.AddTBString(CollectionAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                //临时存储.
                map.AddTBString(CollectionAttr.Docs, null, "方法内容", true, false, 0, 300, 10);

                #region 外观.
                map.AddTBInt(CollectionAttr.PopHeight, 0, "弹窗高度", true, false);
                map.AddTBInt(CollectionAttr.PopWidth, 0, "弹窗宽度", true, false);
                #endregion 外观.

                #region 对功能有效
                //对功能有效.
                map.AddTBString(CollectionAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10);
                map.AddTBString(CollectionAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(CollectionAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(CollectionAttr.WhatAreYouTodo, 0, "执行完毕后干啥？", true, true, CollectionAttr.WhatAreYouTodo,
                "@0=关闭提示窗口@1=关闭提示窗口并刷新@2=转入到Search.htm页面上去");
                #endregion 对功能有效

                //是否启用？
                map.AddBoolean(CollectionAttr.IsEnable, true, "是否启用？", true, true, true);
                map.AddTBInt(CollectionAttr.Idx, 0, "Idx", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 移动.
        public void DoUp()
        {
            this.DoOrderUp(CollectionAttr.FrmID, this.FrmID, CollectionAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(CollectionAttr.FrmID, this.FrmID, CollectionAttr.Idx);
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
    /// 集合方法
    /// </summary>
    public class Collections : EntitiesNoName
    {
        /// <summary>
        /// 集合方法
        /// </summary>
        public Collections() { }
        /// <summary>
        /// 集合方法
        /// </summary>
        /// <param name="nodeid">方法IDID</param>
        public Collections(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CollectionAttr.MethodID, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Collection();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Collection> ToJavaList()
        {
            return (System.Collections.Generic.IList<Collection>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Collection> Tolist()
        {
            System.Collections.Generic.List<Collection> list = new System.Collections.Generic.List<Collection>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Collection)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
