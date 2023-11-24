using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 显示位置
    /// </summary>
    public enum ShowWhere
    {
        /// <summary>
        /// 树
        /// </summary>
        Tree,
        /// <summary>
        /// 工具栏
        /// </summary>
        Toolbar,
        /// <summary>
        /// 抄送
        /// </summary>
        CC
    }
    /// <summary>
    /// 工具栏属性
    /// </summary>
    public class NodeToolbarAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 到达目标
        /// </summary>
        public const string Target = "Target";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// url
        /// </summary>
        public const string UrlExt = "UrlExt";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        //public const string ShowWhere = "ShowWhere";
        /// <summary>
        /// 在工作处理器显示
        /// </summary>
        public const string IsMyFlow = "IsMyFlow";
        /// <summary>
        ///  在工作查看器显示
        /// </summary>
        public const string IsMyView = "IsMyView";
        /// <summary>
        ///  在树形表单显示
        /// </summary>
        public const string IsMyTree = "IsMyTree";
        /// <summary>
        ///  在抄送功能显示
        /// </summary>
        public const string IsMyCC = "IsMyCC";
        /// <summary>
        /// IconPath 图片附件
        /// </summary>
        public const string IconPath = "IconPath";
        /// 执行类型
        /// </summary>
        public const string ExcType = "ExcType";
        #endregion
    }
    /// <summary>
    /// 工具栏.
    /// </summary>
    public class NodeToolbar : EntityOID
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                //  uac.OpenForSysAdmin();
                uac.OpenForAdmin();// 2020.5.15zsy修改
                return uac;
            }
        }
        /// <summary>
        /// 工具栏的事务编号
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeToolbarAttr.FK_Node);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.FK_Node, value);
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey(NodeToolbarAttr.Title);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.Title, value);
            }
        }
        public string Url
        {
            get
            {
                string s= this.GetValStringByKey(NodeToolbarAttr.UrlExt);

                if (this.ExcType !=1 && s.Contains("?") == false && this.Target.ToLower() != "javascript")
                    s = s+"?1=2";
                return s;
            }
            set
            {
                SetValByKey(NodeToolbarAttr.UrlExt, value);
            }
        }
        public string Target
        {
            get
            {
                return this.GetValStringByKey(NodeToolbarAttr.Target);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.Target, value);
            }
        }
       
        /// <summary>
        /// 执行类型
        /// </summary>
        public int ExcType
        {
            get
            {
                return this.GetValIntByKey(NodeToolbarAttr.ExcType);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.ExcType, value);
            }
        }
        /// <summary>
        /// 显示在工具栏中
        /// </summary>
        public bool ItIsMyFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeToolbarAttr.IsMyFlow);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.IsMyFlow, value);
            }
        }
        //显示在流程树中
        public bool ItIsMyTree
        {
            get
            {
                return this.GetValBooleanByKey(NodeToolbarAttr.IsMyTree);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.IsMyTree, value);
            }
        }
        //显示在工作查看器
        public bool ItIsMyView
        {
            get
            {
                return this.GetValBooleanByKey(NodeToolbarAttr.IsMyView);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.IsMyView, value);
            }
        }
        
        //显示在抄送工具栏中
        public bool ItIsMyCC
        {
            get
            {
                return this.GetValBooleanByKey(NodeToolbarAttr.IsMyCC);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.IsMyCC, value);
            }
        }
        //图片附件路径
        public string IconPath
        {
            get
            {
                return this.GetValStringByKey(NodeToolbarAttr.IconPath);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.IconPath, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 工具栏
        /// </summary>
        public NodeToolbar() { }
        /// <summary>
        /// 工具栏
        /// </summary>
        /// <param name="_oid">工具栏ID</param>	
        public NodeToolbar(int oid)
        {
            this.OID = oid;
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

                Map map = new Map("WF_NodeToolbar", "自定义工具栏");

                map.AddTBIntPKOID();
                map.AddTBInt(NodeToolbarAttr.FK_Node, 0, "节点", false, true);
                map.AddTBString(NodeToolbarAttr.Title, null, "标题", true, false, 0, 100, 100, true);

                // 执行类型.
                map.AddDDLSysEnum(NodeToolbarAttr.ExcType, 0, "执行类型", true, true, "ToobarExcType",
                    "@0=超链接@1=函数");

                map.AddTBString(NodeToolbarAttr.UrlExt, null, "连接/函数", true, false, 0, 500, 300, true);
                map.AddTBString(NodeToolbarAttr.Target, null, "目标", true, false, 0, 100, 100, true);

                //显示位置.
                //map.AddDDLSysEnum(NodeToolbarAttr.ShowWhere, 1, "显示位置", false,true, NodeToolbarAttr.ShowWhere,
                 //   "@0=树形表单@1=工具栏@2=抄送工具栏");

                map.AddBoolean(NodeToolbarAttr.IsMyFlow, false, "工作处理器", true, true);
                map.AddBoolean(NodeToolbarAttr.IsMyTree, false, "流程树", true, true);
                map.AddBoolean(NodeToolbarAttr.IsMyView, false, "工作查看器", true, true);
                map.AddBoolean(NodeToolbarAttr.IsMyCC, false, "抄送工具栏", true, true);

                map.AddTBString(NodeToolbarAttr.IconPath, null, "ICON路径", true, false, 0, 100, 100, true);
                string msg = "提示：";
                msg += "\t\n 1. 给工具栏按钮设置图标两种方式,上传图标模式与设置指定的Icon的ID模式.";
                msg += "\t\n 2. 我们优先解决Icon的ID解析模式.";
                msg += "\t\n 3. 比如: ./Img/Btn/Save.png ";
                map.SetHelperAlert(NodeToolbarAttr.IconPath, msg);

                
                map.AddTBInt(NodeToolbarAttr.Idx, 0, "顺序", true, false);
                map.AddMyFile("图标");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 工具栏集合
    /// </summary>
    public class NodeToolbars : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeToolbar();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工具栏集合
        /// </summary>
        public NodeToolbars()
        {
        }
        /// <summary>
        /// 工具栏集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public NodeToolbars(string fk_node)
        {
            this.Retrieve(NodeToolbarAttr.FK_Node, fk_node);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List 
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeToolbar> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeToolbar>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeToolbar> Tolist()
        {
            System.Collections.Generic.List<NodeToolbar> list = new System.Collections.Generic.List<NodeToolbar>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeToolbar)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
