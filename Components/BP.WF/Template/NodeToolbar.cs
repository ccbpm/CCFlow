﻿using System;
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
        Toolbar
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
        public const string Url = "Url";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        public const string ShowWhere = "ShowWhere";
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
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 工具栏的事务编号
        /// </summary>
        public int FK_Node
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
                string s= this.GetValStringByKey(NodeToolbarAttr.Url);
                if (s.Contains("?") == false && this.Target.ToLower() != "javascript")
                    s = s+"?1=2";
                return s;
            }
            set
            {
                SetValByKey(NodeToolbarAttr.Url, value);
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
        /// 显示在那里？
        /// </summary>
        public ShowWhere ShowWhere
        {
            get
            {
                return (ShowWhere)this.GetValIntByKey(NodeToolbarAttr.ShowWhere);
            }
            set
            {
                SetValByKey(NodeToolbarAttr.ShowWhere, (int)value);
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
                map.AddTBString(NodeToolbarAttr.Title, null, "标题", true, false, 0, 100, 100, true);
                map.AddTBString(NodeToolbarAttr.Target, null, "目标", true, false, 0, 100, 100, true);
                map.AddTBString(NodeToolbarAttr.Url, null, "连接", true, false, 0, 500, 300, true);


                // 显示位置.
                map.AddDDLSysEnum(NodeToolbarAttr.ShowWhere, 1, "显示位置", true,true, NodeToolbarAttr.ShowWhere,
                    "@0=树形表单@1=工具栏");

                //map.AddDDLEntities("FK_Dept", null, "部门", new BP.Port.Depts(), false);
                //map.AddDDLEntities("sta", null, "岗位", new BP.Port.Stations(), true);
                //map.AddBoolean("aass_true", false, "boolesn - true", true, true);
                //map.AddBoolean("sd2s_false", false, "boolesn-false", true, false);
                //map.AddTBDate("sssdt_false", null, "日期", true,false);
                //map.AddTBDate("sssdt_true", null, "日期-true", true, true);


                map.AddTBInt(NodeToolbarAttr.Idx, 0, "显示顺序", true, false);
                map.AddTBInt(NodeToolbarAttr.FK_Node, 0, "节点", false,true);
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

        #region 为了适应自动翻译成java的需要,把实体转换成List   /// <summary>
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
