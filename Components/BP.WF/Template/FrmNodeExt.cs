using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmNodeExt : EntityMyPK
    {
        #region 属性.
        public string FK_Frm
        {
            get
            {
                return this.GetValStrByKey(FrmNodeAttr.FK_Frm);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
        }

        public int FK_Flow
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Flow);
            }
        }
        #endregion

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
                uac.IsInsert = false;
                return uac;
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeExt() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNodeExt(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("WF_FrmNode", "节点表单");

                map.AddMyPK();

                map.AddDDLEntities(FrmNodeAttr.FK_Frm, null, "表单", new MapDatas(), false);
                map.AddTBString(FrmNodeAttr.FK_Flow, null, "流程编号", true, true, 0, 4, 20);
                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "节点ID", true, true);

                map.AddBoolean(FrmNodeAttr.IsPrint, false, "是否可以打印", true, true);
                map.AddBoolean(FrmNodeAttr.IsEnableLoadData, false, "是否启用装载填充事件", true, true);
                map.AddDDLSysEnum(FrmNodeAttr.FrmSln, 0, "表单控制方案", true, true, FrmNodeAttr.FrmSln,
                    "@0=默认方案@1=只读方案@2=自定义方案");

                map.AddDDLSysEnum(FrmNodeAttr.WhoIsPK, 0, "谁是主键?", true, true);

                //显示的
                map.AddTBInt(FrmNodeAttr.Idx, 0, "顺序号(显示的顺序)", true, false);

                //add 2016.3.25.
                map.AddBoolean(FrmNodeAttr.Is1ToN, false, "是否1变N？(分流节点有效)", true, true, true);
                map.AddTBString(FrmNodeAttr.HuiZong, null, "子线程要汇总的数据表(子线程节点)", true, false, 0, 300, 20);

                //模版文件，对于office表单有效.
                map.AddTBString(FrmNodeAttr.TempleteFile, null, "模版文件", true, false, 0, 500, 20);

                //是否显示
                map.AddTBString(FrmNodeAttr.GuanJianZiDuan, null, "关键字段", true, false, 0, 20, 20);

                #region 表单启用规则.
                map.AddDDLSysEnum(FrmNodeAttr.FrmEnableRole, 0, "表单启用规则?", true, true);
                map.AddTBStringDoc(FrmNodeAttr.FrmEnableExp, null, "启用的表达式", true, false, true);
                #endregion 表单启用规则.

                RefMethod rm = new RefMethod();
                //rm.Title = "启用规则";
                //rm.ClassMethodName = this.ToString() + ".DoEnableRole()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段权限";
                rm.ClassMethodName = this.ToString() + ".DoFields()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "从表权限";
                rm.ClassMethodName = this.ToString() + ".DoDtls()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "附件权限";
                rm.ClassMethodName = this.ToString() + ".DoAths()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "从其他节点Copy权限设置";
                rm.ClassMethodName = this.ToString() + ".DoCopyFromNode()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 表单元素权限.
        public string DoDtls()
        {
            return "../../Admin/Sln/Dtls.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoFields()
        {
            return "../../Admin/Sln/Fields.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoAths()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }

        public string DoCopyFromNode()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoEnableRole()
        {
            return "../../Admin/AttrNode/BindFrmsFrmEnableRole.htm?MyPK=" + this.MyPK;
        }
        #endregion 表单元素权限.

    }
    /// <summary>
    /// 节点表单s
    /// </summary>
    public class FrmNodeExts : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeExts() { }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNodeExt();
            }
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNodeExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNodeExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNodeExt> Tolist()
        {
            System.Collections.Generic.List<FrmNodeExt> list = new System.Collections.Generic.List<FrmNodeExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNodeExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
