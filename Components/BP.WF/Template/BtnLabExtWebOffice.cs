using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 公文属性控制
    /// </summary>
    public class BtnLabExtWebOffice : Entity
    {
        /// <summary>
        /// 访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                uac.IsDelete = false;
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// but
        /// </summary>
        public static string Btns
        {
            get
            {
                return "Send,Save,Thread,Return,CC,Shift,Del,Rpt,Ath,Track,Opt,EndFlow,SubFlow";
            }
        }
        /// <summary>
        /// PK
        /// </summary>
        public override string PK
        {
            get
            {
                return NodeAttr.NodeID;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NodeID);
            }
            set
            {
                this.SetValByKey(BtnAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 查询标签
        /// </summary>
        public string SearchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SearchLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchLab, value);
            }
        }
        /// <summary>
        /// 查询是否可用
        /// </summary>
        public bool SearchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SearchEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchEnable, value);
            }
        }
        /// <summary>
        /// 移交
        /// </summary>
        public string ShiftLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShiftLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftLab, value);
            }
        }
        /// <summary>
        /// 是否启用移交
        /// </summary>
        public bool ShiftEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShiftEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftEnable, value);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        public string SaveLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SaveLab);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool SaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SaveEnable);
            }
        }
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public string ThreadLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ThreadLab);
            }
        }
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public bool ThreadEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadEnable);
            }
        }

        /// <summary>
        /// 子流程按钮标签
        /// </summary>
        public string SubFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SubFlowLab);
            }
        }
       
        /// <summary>
        /// 跳转标签
        /// </summary>
        public string JumpWayLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.JumpWayLab);
            }
        }
        public JumpWay JumpWayEnum
        {
            get
            {
                return (JumpWay)this.GetValIntByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 是否启用跳转
        /// </summary>
        public bool JumpWayEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 退回标签
        /// </summary>
        public string ReturnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnLab);
            }
        }
        /// <summary>
        /// 退回字段
        /// </summary>
        public string ReturnField
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnField);
            }
        }
        /// <summary>
        /// 跳转是否启用
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        /// <summary>
        /// 挂起标签
        /// </summary>
        public string HungLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HungLab);
            }
        }
        /// <summary>
        /// 是否启用挂起
        /// </summary>
        public bool HungEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.HungEnable);
            }
        }
        /// <summary>
        /// 打印标签
        /// </summary>
        public string PrintDocLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintDocLab);
            }
        }
        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool PrintDocEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintDocEnable);
            }
        }
        /// <summary>
        /// 发送标签
        /// </summary>
        public string SendLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendLab);
            }
        }
        /// <summary>
        /// 是否启用发送?
        /// </summary>
        public bool SendEnable
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 发送的Js代码
        /// </summary>
        public string SendJS
        {
            get
            {
                string str = this.GetValStringByKey(BtnAttr.SendJS).Replace("~", "'");
                if (this.CCRole == BP.WF.CCRole.WhenSend)
                    str = str + "  if ( OpenCC()==false) return false;";
                return str;
            }
        }
        /// <summary>
        /// 轨迹标签
        /// </summary>
        public string TrackLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TrackLab);
            }
        }
        /// <summary>
        /// 是否启用轨迹
        /// </summary>
        public bool TrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TrackEnable);
            }
        }
        /// <summary>
        /// 抄送标签
        /// </summary>
        public string CCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CCLab);
            }
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        public CCRole CCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(BtnAttr.CCRole);
            }
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        public string DeleteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.DelLab);
            }
        }
        /// <summary>
        /// 删除类型
        /// </summary>
        public int DeleteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }
        /// <summary>
        /// 结束流程
        /// </summary>
        public string EndFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.EndFlowLab);
            }
        }
        /// <summary>
        /// 是否启用结束流程
        /// </summary>
        public bool EndFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.EndFlowEnable);
            }
        }
          /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public string TCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TCLab);
            }
        }
        /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public bool TCEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TCEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.TCEnable, value);
            }
        }
        
        /// <summary>
        /// 审核标签
        /// </summary>
        public string WorkCheckLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WorkCheckLab);
            }
        }
        /// <summary>
        /// 审核是否可用
        /// </summary>
        public bool WorkCheckEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.WorkCheckEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.WorkCheckEnable, value);
            }
        }
        /// <summary>
        /// 考核 是否可用
        /// </summary>
        public int CHRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.CHRole);
            }
        }
        /// <summary>
        /// 考核 标签
        /// </summary>
        public string CHLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CHLab);
            }
        }
        /// <summary>
        /// 重要性 是否可用
        /// </summary>
        public bool PRIEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PRIEnable);
            }
        }
        /// <summary>
        /// 重要性 标签
        /// </summary>
        public string PRILab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PRILab);
            }
        }
        /// <summary>
        /// 关注 是否可用
        /// </summary>
        public bool FocusEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.FocusEnable);
            }
        }
        /// <summary>
        /// 关注 标签
        /// </summary>
        public string FocusLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.FocusLab);
            }
        }
        /// <summary>
        /// 批量处理是否可用
        /// </summary>
        public bool BatchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.BatchEnable);
            }
        }
        /// <summary>
        /// 批处理标签
        /// </summary>
        public string BatchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.BatchLab);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public bool AskforEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AskforEnable);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public string AskforLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AskforLab);
            }
        }
        /// <summary>
        ///是否启用文档,@0=不启用@1=按钮方式@2=公文在前@3=表单在前
        /// </summary>
        private int WebOfficeEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.WebOfficeEnable);
            }
        }
        /// <summary>
        /// 公文的工作模式 @0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式
        /// </summary>
        public WebOfficeWorkModel WebOfficeWorkModel
        {
            get
            {
                return (WebOfficeWorkModel)this.WebOfficeEnable;
            }
            set
            {
                this.SetValByKey(BtnAttr.WebOfficeEnable, (int)value);
            }
        }
        /// <summary>
        /// 表单工作方式.
        /// </summary>
        public FrmType WebOfficeFrmModel
        {
            get
            {
                return (FrmType)this.GetValIntByKey(BtnAttr.WebOfficeFrmModel, (int)FrmType.FreeFrm);
            }
            set
            {
                this.SetValByKey(BtnAttr.WebOfficeFrmModel, (int)value);
            }
        }
        /// <summary>
        /// 文档按钮标签
        /// </summary>
        public string WebOfficeLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WebOfficeLab);
            }
        }
        /// <summary>
        /// 打开本地文件
        /// </summary>
        public bool OfficeOpenEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenEnable); }
        }
        /// <summary>
        /// 打开本地标签      
        /// </summary>
        public string OfficeOpenLab
        {
            get { return this.GetValStrByKey(BtnAttr.OfficeOpenLab); }
        }
        /// <summary>
        /// 打开模板
        /// </summary>
        public bool OfficeOpenTemplateEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenTemplateEnable); }
        }
        /// <summary>
        /// 打开模板标签
        /// </summary>
        public string OfficeOpenTemplateLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOpenTemplateLab); }
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        public bool OfficeSaveEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSaveEnable); }
        }
        /// <summary>
        /// 保存标签
        /// </summary>
        public string OfficeSaveLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSaveLab); }
        }
        /// <summary>
        /// 接受修订
        /// </summary>
        public bool OfficeAcceptEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeAcceptEnable); }
        }
        /// <summary>
        /// 接受修订标签
        /// </summary>
        public string OfficeAcceptLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeAcceptLab); }
        }
        /// <summary>
        /// 拒绝修订
        /// </summary>
        public bool OfficeRefuseEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeRefuseEnable); }
        }
        /// <summary>
        /// 拒绝修订标签
        /// </summary>
        public string OfficeRefuseLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeRefuseLab); }
        }
        public string OfficeOVerLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOverLab); }
        }
        /// <summary>
        /// 是否套红
        /// </summary>
        public bool OfficeOverEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOverEnable); }
        }
        /// <summary>
        /// 套红按钮标签
        /// </summary>
        public string OfficeOverLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOverLab); }
        }
        /// <summary>
        /// 是否打印
        /// </summary>
        public bool OfficePrintEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficePrintEnable); }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool OfficeMarksEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeMarksEnable); }
        }
        /// <summary>
        /// 打印按钮标签
        /// </summary>
        public string OfficePrintLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficePrintLab); }
        }
        /// <summary>
        /// 签章按钮
        /// </summary>
        public bool OfficeSealEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSealEnable); }
        }
        /// <summary>
        /// 签章标签
        /// </summary>
        public string OfficeSealLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSealLab); }
        }

        /// <summary>
        ///插入流程
        /// </summary>
        public bool OfficeInsertFlowEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeInsertFlowEnable); }
        }
        /// <summary>
        /// 流程标签
        /// </summary>
        public string OfficeInsertFlowLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeInsertFlowLab); }
        }

         
        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeNodeInfo
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeNodeInfo); }
        }

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeReSavePDF
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeReSavePDF); }
        }

        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public bool OfficeIsMarks
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsMarks); }
        }

        /// <summary>
        /// 风险点模板
        /// </summary>
        public String OfficeFengXianTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeFengXianTemplate); }
        }

        public bool OfficeReadOnly
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeReadOnly); }
        }

        /// <summary>
        /// 下载按钮标签
        /// </summary>
        public String OfficeDownLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeDownLab); }
        }
        /// <summary>
        /// 下载按钮标签
        /// </summary>
        public bool OfficeIsDown
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeDownEnable); }
        }

        /// <summary>
        /// 是否启用下载
        /// </summary>
        public bool OfficeDownEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeDownEnable); }
        }

        /// <summary>
        /// 指定文档模板
        /// </summary>
        public String OfficeTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTemplate); }
        }

 
        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public bool OfficeIsParent
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsParent); }
        }

        /// <summary>
        /// 是否自动套红
        /// </summary>
        public bool OfficeTHEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeTHEnable); }
        }
        /// <summary>
        /// 自动套红模板
        /// </summary>
        public string OfficeTHTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTHTemplate); }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLabExtWebOffice() { }
        /// <summary>
        /// 公文属性控制
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public BtnLabExtWebOffice(int nodeid)
        {
            this.NodeID = nodeid;
            this.RetrieveFromDBSources();
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

                Map map = new Map("WF_Node", "公文属性控制");

                map.Java_SetDepositaryOfEntity(Depositary.Application);

                map.AddTBIntPK(BtnAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(BtnAttr.Name, null, "节点名称", true, true, 0, 100, 10);


                #region 公文按钮
                map.AddTBString(BtnAttr.OfficeOpen, "打开本地", "打开本地标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeOpenTemplate, "打开模板", "打开模板标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSave, "保存", "保存标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeAccept, "接受修订", "接受修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeRefuse, "拒绝修订", "拒绝修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeOver, "套红按钮", "套红按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeMarks, true, "是否查看用户留痕", true, true);
                map.AddBoolean(BtnAttr.OfficeReadOnly, false, "是否只读", true, true);

                map.AddTBString(BtnAttr.OfficePrint, "打印按钮", "打印按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSeal, "签章按钮", "签章按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSealEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeInsertFlow, "插入流程", "插入流程标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "是否记录节点信息", true, true);
                map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "是否该自动保存为PDF", true, true);

                map.AddTBString(BtnAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeDownEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeIsMarks, true, "是否进入留痕模式", true, true);
                map.AddTBString(BtnAttr.OfficeTemplate, "", "指定文档模板", true, false, 0, 50, 10);

                map.AddBoolean(BtnAttr.OfficeIsParent, true, "是否使用父流程的文档", true, true);

             

                map.AddBoolean(BtnAttr.OfficeIsTrueTH, false, "是否自动套红", true, true);
                map.AddTBString(BtnAttr.OfficeTHTemplate, "", "自动套红模板", true, false, 0, 50, 10);

                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "公文标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                 "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");  //edited by liuxc,2016-01-18,from xc.

                map.AddDDLSysEnum(BtnAttr.WebOfficeFrmModel, 0, "表单工作方式", true, true, "FrmType");

                #endregion


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //同步更新表单的工作模式.
            MapData md = new MapData("ND" + this.NodeID);
            md.HisFrmType = this.WebOfficeFrmModel;
            md.Update();

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            fl.Update();

            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 公文属性控制s
    /// </summary>
    public class BtnLabExtWebOffices : Entities
    {
        /// <summary>
        /// 公文属性控制s
        /// </summary>
        public BtnLabExtWebOffices()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BtnLabExtWebOffice();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BtnLabExtWebOffice> ToJavaList()
        {
            return (System.Collections.Generic.IList<BtnLabExtWebOffice>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BtnLabExtWebOffice> Tolist()
        {
            System.Collections.Generic.List<BtnLabExtWebOffice> list = new System.Collections.Generic.List<BtnLabExtWebOffice>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BtnLabExtWebOffice)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
