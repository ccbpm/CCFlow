using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF;
namespace BP.WF.Template
{
    /// <summary>
    /// 绑定表单的审核组件
    /// </summary>
    public class FrmWorkCheck : Entity
    {
        #region 属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public string No
        {
            get
            {
                return "ND" + this.NodeID;
            }
            set
            {
                string nodeID = value.Replace("ND", "");
                this.NodeID = int.Parse(nodeID);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public FrmWorkCheckSta HisFrmWorkCheckSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeWorkCheckAttr.FWCSta);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCSta, (int)value);
            }
        }
        /// <summary>
        /// 显示格式(0=表格,1=自由.)
        /// </summary>
        public FrmWorkShowModel HisFrmWorkShowModel
        {
            get
            {
                return (FrmWorkShowModel)this.GetValIntByKey(NodeWorkCheckAttr.FWCShowModel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCShowModel, (int)value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public FWCAth FWCAth
        {
            get
            {
                return (FWCAth)this.GetValIntByKey(NodeWorkCheckAttr.FWCAth);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCAth, (int)value);
            }
        }
        /// <summary>
        /// 组件类型
        /// </summary>
        public FWCType HisFrmWorkCheckType
        {
            get
            {
                return (FWCType)this.GetValIntByKey(NodeWorkCheckAttr.FWCType);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCType, (int)value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string FWCLab
        {
            get
            {
                return this.GetValStrByKey(NodeWorkCheckAttr.FWCLab);
            }
        }
        /// <summary>
        /// 组件类型名称
        /// </summary>
        public string FWCTypeT
        {
            get
            {
                return this.GetValRefTextByKey(NodeWorkCheckAttr.FWCType);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float FWC_Y
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_Y);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWC_Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float FWC_X
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_X);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWC_X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float FWC_W
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_W);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWC_W, value);
            }
        }
        public string FWC_Wstr
        {
            get
            {
                if (this.FWC_W == 0)
                    return "100%";
                return this.FWC_W + "px";
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float FWC_H
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_H);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWC_H, value);
            }
        }
        public string FWC_Hstr
        {
            get
            {
                if (this.FWC_H == 0)
                    return "100%";
                return this.FWC_H + "px";
            }
        }
        /// <summary>
        /// 轨迹图是否显示?
        /// </summary>
        public bool FWCTrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCTrackEnable);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCTrackEnable, value);
            }
        }
        /// <summary>
        /// 历史审核信息是否显示?
        /// </summary>
        public bool FWCListEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCListEnable);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCListEnable, value);
            }
        }
        /// <summary>
        /// 在轨迹表里是否显示所有的步骤？
        /// </summary>
        public bool FWCIsShowAllStep
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsShowAllStep);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowAllStep, value);
            }
        }
        /// <summary>
        /// 是否显示轨迹在没有走到的节点
        /// </summary>
        public bool FWCIsShowTruck
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsShowTruck);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowTruck, value);
            }
        }
        /// <summary>
        /// 是否显示退回信息？
        /// </summary>
        public bool FWCIsShowReturnMsg
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsShowReturnMsg);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowReturnMsg, value);
            }
        }
        /// <summary>
        /// 如果用户未审核是否按照默认意见填充?
        /// </summary>
        public bool FWCIsFullInfo
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsFullInfo);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsFullInfo, value);
            }
        }
        /// <summary>
        /// 默认审核信息
        /// </summary>
        public string FWCDefInfo
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCDefInfo);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCDefInfo, value);
            }
        }
        /// <summary>
        /// 节点名称.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        /// <summary>
        /// 节点意见名称，如果为空则取节点名称.
        /// </summary>
        public string FWCNodeName
        {
            get
            {
                string str = this.GetValStringByKey(NodeWorkCheckAttr.FWCNodeName);
                if (DataType.IsNullOrEmpty(str))
                    return this.Name;
                return str;
            }
        }
        /// <summary>
        /// 操作名词(审核，审定，审阅，批示)
        /// </summary>
        public string FWCOpLabel
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCOpLabel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCOpLabel, value);
            }
        }
        /// <summary>
        /// 操作字段
        /// </summary>
        public string FWCFields
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCFields);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCFields, value);
            }
        }
        /// <summary>
        /// 自定义常用短语
        /// </summary>
        public string FWCNewDuanYu
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCNewDuanYu);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCNewDuanYu, value);
            }
        }
        /// <summary>
        /// 是否显示数字签名？
        /// </summary>
        public bool SigantureEnabel
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.SigantureEnabel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.SigantureEnabel, value);
            }
        }

        /// <summary>
        /// 协作模式下操作员显示顺序
        /// </summary>
        public FWCOrderModel FWCOrderModel
        {
            get
            {
                return (FWCOrderModel)this.GetValIntByKey(NodeWorkCheckAttr.FWCOrderModel, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCOrderModel, (int)value);
            }
        }
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public FrmWorkCheckSta FWCSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeWorkCheckAttr.FWCSta, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCSta, (int)value);
            }
        }

        public int FWCVer
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCVer, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCVer,value);
            }
        }
        public string FWCView
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCView);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCView, value);
            }
        }

        public string CheckField
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.CheckField);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.CheckField, value);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                // uac.OpenForSysAdmin();
                uac.OpenForAdmin() ; //zsy2020.5.29修改
                
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        public FrmWorkCheck()
        {
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        /// <param name="no"></param>
        public FrmWorkCheck(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.HisFrmWorkCheckSta = FrmWorkCheckSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.HisFrmWorkCheckSta = FrmWorkCheckSta.Disable;
                return;
            }

            try
            {
                this.NodeID = int.Parse(mapdata);
            }
            catch
            {
                return;
            }
            this.Retrieve();
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        /// <param name="no"></param>
        public FrmWorkCheck(int nodeID)
        {
            this.NodeID = nodeID;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "审核组件");
                map.Java_SetEnType(EnType.Sys);

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);
                map.AddTBString(NodeWorkCheckAttr.FWCLab, "审核信息", "显示标签", true, false, 0, 100, 10, true);

                #region 此处变更了 NodeSheet类中的，map 描述该部分也要变更.
                //map.AddDDLSysEnum(NodeWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "审核组件状态",
                 //  true, true, NodeWorkCheckAttr.FWCSta, "@0=禁用@1=启用@2=只读");

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "显示方式",
                    true, true, NodeWorkCheckAttr.FWCShowModel, "@0=表格方式@1=自由模式"); //此属性暂时没有用.

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCType, (int)FWCType.Check, "审核组件", true, true,
                    NodeWorkCheckAttr.FWCType, "@0=审核组件@1=日志组件@2=周报组件@3=月报组件");

                map.AddTBString(NodeWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCAth, (int)FWCAth.None, "附件上传", true, true,
                   NodeWorkCheckAttr.FWCAth, "@0=不启用@1=多附件@2=单附件(暂不支持)@3=图片附件(暂不支持)");
                map.SetHelperAlert(NodeWorkCheckAttr.FWCAth,
                    "在审核期间，是否启用上传附件？启用什么样的附件？注意：附件的属性在节点表单里配置。"); //使用alert的方式显示帮助信息.

                map.AddBoolean(NodeWorkCheckAttr.FWCTrackEnable, true, "轨迹图是否显示？", true, true, false);

                map.AddBoolean(NodeWorkCheckAttr.FWCListEnable, true, "历史审核信息是否显示？(否,仅出现意见框)", true, true, true);
                map.AddBoolean(NodeWorkCheckAttr.FWCIsShowAllStep, false, "在轨迹表里是否显示所有的步骤？", true, true);

                map.AddTBString(NodeWorkCheckAttr.FWCOpLabel, "审核", "操作名词(审核/审阅/批示)", true, false, 0, 50, 10);
                map.AddTBString(NodeWorkCheckAttr.FWCDefInfo, "同意", "默认审核信息", true, false, 0, 50, 10);
                map.AddBoolean(NodeWorkCheckAttr.SigantureEnabel, false, "操作人是否显示为图片签名？", true, true);
                map.AddBoolean(NodeWorkCheckAttr.FWCIsFullInfo, true, "如果用户未审核是否按照默认意见填充？", true, true, true);


                map.AddTBFloat(NodeWorkCheckAttr.FWC_X, 300, "位置X", true, false);
                map.AddTBFloat(NodeWorkCheckAttr.FWC_Y, 500, "位置Y", true, false);

                map.AddTBFloat(NodeWorkCheckAttr.FWC_H, 300, "高度(0=100%)", true, false);
                map.AddTBFloat(NodeWorkCheckAttr.FWC_W, 400, "宽度(0=100%)", true, false);

                map.AddTBString(NodeWorkCheckAttr.FWCFields, null, "审批格式字段", true, false, 0, 50, 10, true);
                //map.AddTBString(NodeWorkCheckAttr.FWCNewDuanYu, null, "自定义常用短语(使用@分隔)", true, false, 0, 100, 10, true);

                map.AddBoolean(NodeWorkCheckAttr.FWCIsShowTruck, false, "是否显示未审核的轨迹？", true, true, true);
                map.AddBoolean(NodeWorkCheckAttr.FWCIsShowReturnMsg, false, "是否显示退回信息？", true, true, true);

                //增加如下字段是为了查询与排序的需要.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 0, 3, 10);
                map.AddTBInt(NodeAttr.Step, 0, "步骤", false, false);


                //协作模式下审核人显示顺序. add for yantai zongheng.
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCOrderModel, 0, "协作模式下操作员显示顺序", true, true,
                  NodeWorkCheckAttr.FWCOrderModel, "@0=按审批时间先后排序@1=按照接受人员列表先后顺序(官职大小)");

                //for tianye , 多人审核的时候，不让其看到其他人的意见.
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCMsgShow, 0, "审核意见显示方式", true, true,
                  NodeWorkCheckAttr.FWCMsgShow, "@0=都显示@1=仅显示自己的意见");

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCVer, 0, "审核意见版本号", true, true, NodeWorkCheckAttr.FWCVer,
                "@0=2018@1=2019");

             

                //map.AddTBString(NodeWorkCheckAttr.CheckField, null, "签批字段", true, false, 0, 50, 10, false);

                map.AddTBString(NodeWorkCheckAttr.FWCView, null, "审核意见立场", true, false, 20, 200, 200,true);

                #endregion 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (this.FWCAth == FWCAth.MinAth)
            {
                FrmAttachment workCheckAth = new FrmAttachment();
                bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK, this.NodeID + "_FrmWorkCheck");
                //不包含审核组件
                if (isHave == false)
                {
                    workCheckAth = new FrmAttachment();
                    /*如果没有查询到它,就有可能是没有创建.*/
                    workCheckAth.MyPK = "ND" + this.NodeID + "_FrmWorkCheck";
                    workCheckAth.FK_MapData = "ND" + this.NodeID.ToString();
                    workCheckAth.NoOfObj = "FrmWorkCheck";
                    workCheckAth.Exts = "*.*";

                    //存储路径.
                 //   workCheckAth.SaveTo = "/DataUser/UploadFile/";
                    workCheckAth.IsNote = false; //不显示note字段.
                    workCheckAth.IsVisable = false; // 让其在form 上不可见.

                    //位置.
                    workCheckAth.X = (float)94.09;
                    workCheckAth.Y = (float)333.18;
                    workCheckAth.W = (float)626.36;
                    workCheckAth.H = (float)150;

                    //多附件.
                    workCheckAth.UploadType = AttachmentUploadType.Multi;
                    workCheckAth.Name = "审核组件";
                    workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                    workCheckAth.Insert();
                }
            }
            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            if (this.FWCLab.Equals("审核组件") == false)
            {
                GroupFields groupFields = new GroupFields();
                groupFields.Retrieve(GroupFieldAttr.FrmID, "ND" + this.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
                if (groupFields.Count == 1)
                {
                    GroupField gf = groupFields[0] as GroupField;
                    if (gf.Lab.Equals(this.FWCLab) == false)
                    {
                        gf.Lab = this.FWCLab;
                        gf.Update();
                    }

                }
            }

            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 审核组件s
    /// </summary>
    public class FrmWorkChecks : Entities
    {
        #region 构造
        /// <summary>
        /// 审核组件s
        /// </summary>
        public FrmWorkChecks()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmWorkCheck();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmWorkCheck> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmWorkCheck>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmWorkCheck> Tolist()
        {
            System.Collections.Generic.List<FrmWorkCheck> list = new System.Collections.Generic.List<FrmWorkCheck>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmWorkCheck)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
