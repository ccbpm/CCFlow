using System;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 流程批量发起流程
    /// </summary>
    public class CollectionFlowBatch : EntityNoName
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
        /// 流程批量发起流程
        /// </summary>
        public CollectionFlowBatch()
        {
        }
        public CollectionFlowBatch(string no)
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

                Map map = new Map("Frm_Collection", "流程批量发起");

                //主键.
                map.AddTBStringPK(CollectionAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(CollectionAttr.Name, null, "方法名称", true, false, 0, 300, 10);
                map.AddTBString(CollectionAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);

                //功能标记. 
                map.AddTBString(CollectionAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.Mark, null, "Mark", true, true, 0, 300, 10);

                map.AddTBString(CollectionAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(CollectionAttr.FlowNo, null, "流程编号", true, true, 0, 10, 10);

                map.AddTBString(CollectionAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string CreateWorkID()
        {
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FlowNo);

            //给当前的流程实例做标记.
            BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);
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
    /// 流程批量发起流程
    /// </summary>
    public class CollectionFlowBatchs : EntitiesNoName
    {
        /// <summary>
        /// 流程批量发起流程
        /// </summary>
        public CollectionFlowBatchs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>45f55
        public override Entity GetNewEntity
        {
            get
            {
                return new CollectionFlowBatch();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CollectionFlowBatch> ToJavaList()
        {
            return (System.Collections.Generic.IList<CollectionFlowBatch>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CollectionFlowBatch> Tolist()
        {
            System.Collections.Generic.List<CollectionFlowBatch> list = new System.Collections.Generic.List<CollectionFlowBatch>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CollectionFlowBatch)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
