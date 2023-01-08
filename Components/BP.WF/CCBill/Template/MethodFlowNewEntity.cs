using System;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 新建实体流程
    /// </summary>
    public class MethodFlowNewEntity : EntityNoName
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
        public string UrlExt
        {
            get
            {
                return this.GetValStringByKey("UrlExt");
            }
            set
            {
                this.SetValByKey("UrlExt", value);
            }
        }
        /// <summary>
        /// 是否在流程结束后同步？
        /// </summary>
        public bool DTSWhenFlowOver
        {
            get
            {
                return this.GetValBooleanByKey(MethodAttr.DTSWhenFlowOver);
            }
            set
            {
                this.SetValByKey(MethodAttr.DTSWhenFlowOver, value);
            }
        }
        /// <summary>
        /// 同步的方式
        /// </summary>
        public int DTSDataWay
        {
            get
            {
                return this.GetValIntByKey(MethodAttr.DTSDataWay);
            }
            set
            {
                this.SetValByKey(MethodAttr.DTSDataWay, value);
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
        /// 新建实体流程
        /// </summary>
        public MethodFlowNewEntity()
        {
        }
        public MethodFlowNewEntity(string no)
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

                Map map = new Map("GPM_Menu", "新建实体");

                map.AddTBStringPK(BP.CCFast.CCMenu.MenuAttr.No, null, "编号", true, true, 0, 150, 10);
                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString("FlowNo", null, "流程编号", true, true, 0, 300, 10, false);
                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                //
                map.AddTBString("UrlExt", null, "链接", false, false, 0, 300, 10, true);

                #region 显示位置控制.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyDict.htm工具栏上", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyDict.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在 SearchDict.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.

                #region 相同字段数据同步方式.
                map.AddDDLSysEnum(MethodAttr.DTSDataWay, 0, "同步相同字段数据方式", true, true, MethodAttr.DTSDataWay,
               "@0=不同步@1=同步全部的相同字段的数据@2=同步指定字段的数据");

                map.AddTBString(MethodAttr.DTSSpecFiels, null, "要同步的字段", true, false, 0, 300, 10, true);

                map.AddBoolean(MethodAttr.DTSWhenFlowOver, false, "流程结束后同步？", true, true, true);
                map.AddBoolean(MethodAttr.DTSWhenNodeOver, false, "节点发送成功后同步？", true, true, true);
                #endregion 相同字段数据同步方式.


                RefMethod rm = new RefMethod();
                rm.Title = "设计流程"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAlert";
                rm.Warning = "";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                //rm.GroupName = "开发接口";
                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重新导入实体字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".ReSetFrm";
                rm.Warning = "现有的表单字段将会被清除，重新导入的字段会被增加上去，数据不会变化，导入需慎重。";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string CreateWorkID()
        {
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FlowNo);

            //给当前的流程实例做标记.
            BP.WF.GenerWorkFlow gwf = new WF.GenerWorkFlow(workid);
            gwf.PFlowNo = this.FrmID;
            gwf.SetPara("FlowNewEntity", "1"); //设置标记，等到流程结束后，自动写入到Dict一笔记录.
            gwf.SetPara("MenuNo", this.No); //菜单编号.
            gwf.PWorkID = gwf.WorkID; //实体保存的ID 与 流程ID一致。
            gwf.Update();

            return workid.ToString();
        }

        #region 执行方法.
        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoAlert()
        {
            return "您需要转入流程设计器去设计流程.";
            // return "../../CCBill/Admin/MethodParas.htm?No=" + this.MyPK;
        }
        /// <summary>
        /// 重新导入实体字段
        /// </summary>
        /// <returns></returns>
        public string ReSetFrm()
        {
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handler = new BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            handler.Imp_CopyFrm("ND" + int.Parse(this.FlowNo + "01"), this.FrmID);
            return "执行成功，您需要转入流程设计器查看表单.";

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
    /// 新建实体流程
    /// </summary>
    public class MethodFlowNewEntitys : EntitiesNoName
    {
        /// <summary>
        /// 新建实体流程
        /// </summary>
        public MethodFlowNewEntitys() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodFlowNewEntity();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodFlowNewEntity> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodFlowNewEntity>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodFlowNewEntity> Tolist()
        {
            System.Collections.Generic.List<MethodFlowNewEntity> list = new System.Collections.Generic.List<MethodFlowNewEntity>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodFlowNewEntity)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
