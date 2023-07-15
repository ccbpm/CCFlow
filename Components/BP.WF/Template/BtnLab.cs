using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点按钮权限
    /// </summary>
    public class BtnLab : Entity
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
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.Name);
            }
            set
            {
                this.SetValByKey(BtnAttr.Name, value);
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
        /// 选择接受人
        /// </summary>
        public string SelectAccepterLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SelectAccepterLab);
            }
        }
        /// <summary>
        /// 选择接受人类型
        /// </summary>
        public int SelectAccepterEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.SelectAccepterEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SelectAccepterEnable, value);
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
        /// 是否可以删除（当前分流，分合流节点发送出去的）子线程.
        /// </summary>
        public bool ThreadIsCanDel
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanDel);
            }
        }
        /// <summary>
        /// 是否可以移交.
        /// </summary>
        public bool ThreadIsCanShift
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanShift);
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
        /// 退回是否启用
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
            set
            {
                this.SetValByKey(BtnAttr.SendLab, value);
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
                if (this.CCRole == BP.WF.CCRoleEnum.WhenSend)
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
        /// 查看父流程标签
        /// </summary>
        public string ShowParentFormLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShowParentFormLab);
            }
        }

        /// <summary>
        /// 是否启用查看父流程
        /// </summary>
        public bool ShowParentFormEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShowParentFormEnable);
            }
        }
        /// <summary>
        /// 数据批阅标签
        /// </summary>
        public string FrmDBRemarkLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.FrmDBRemarkLab);
            }
        }
        /// <summary>
        /// 数据批阅规则
        /// </summary>
        public int FrmDBRemarkEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.FrmDBRemarkEnable);
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
        public CCRoleEnum CCRole
        {
            get
            {
                return (CCRoleEnum)this.GetValIntByKey(BtnAttr.CCRole);
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

        public int HelpRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.HelpRole);
            }
        }

        public string HelpLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HelpLab);
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
        /// 标签是否启用？
        /// </summary>
        //public bool SubFlowEnable111
        //{
        //    get
        //    {
        //        return this.GetValBooleanByKey(BtnAttr.SubFlowEnable);
        //    }
        //    set
        //    {
        //        this.SetValByKey(BtnAttr.SubFlowEnable, value);
        //    }
        //}
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
        public int PRIEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.PRIEnable);
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
        /// 分配 是否可用
        /// </summary>
        public bool AllotEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AllotEnable);
            }
        }
        /// <summary>
        /// 分配 标签
        /// </summary>
        public string AllotLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AllotLab);
            }
        }

        /// <summary>
        /// 确认 是否可用
        /// </summary>
        public bool ConfirmEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ConfirmEnable);
            }
        }
        /// <summary>
        /// 确认标签
        /// </summary>
        public string ConfirmLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ConfirmLab);
            }
        }

        /// <summary>
        /// 打包下载 是否可用
        /// </summary>
        public bool PrintZipEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintZipEnable);
            }
        }
        public bool PrintZipMyCC
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintZipMyCC);
            }
        }
        public bool PrintZipMyView
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintZipMyView);
            }
        }
        /// <summary>
        /// 打包下载 标签
        /// </summary>
        public string PrintZipLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintZipLab);
            }
        }
        /// <summary>
        /// pdf 是否可用
        /// </summary>
        public bool PrintPDFEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintPDFEnable);
            }
        }
        public bool PrintPDFMyCC
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintPDFMyCC);
            }
        }
        public bool PrintPDFMyView
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintPDFMyView);
            }
        }
        /// <summary>
        /// 打包下载 标签
        /// </summary>
        public string PrintPDFLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintPDFLab);
            }
        }

        /// <summary>
        /// html 是否可用
        /// </summary>
        public bool PrintHtmlEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintHtmlEnable);
            }
        }

        public bool PrintHtmlMyCC
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintHtmlMyCC);
            }
        }

        public bool PrintHtmlMyView
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintHtmlMyView);
            }
        }
        /// <summary>
        /// html 标签
        /// </summary>
        public string PrintHtmlLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintHtmlLab);
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
        /// 会签规则
        /// </summary>
        public HuiQianRole HuiQianRole
        {
            get
            {
                return (HuiQianRole)this.GetValIntByKey(BtnAttr.HuiQianRole);
            }
        }
        /// <summary>
        /// 会签标签
        /// </summary>
        public string HuiQianLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HuiQianLab);
            }
        }
        
        // public int IsCanAddHuiQianer
        //{
        //    get
        //    {
        //        return this.GetValIntByKey(BtnAttr.IsCanAddHuiQianer);
        //    }
        //}
        public HuiQianLeaderRole HuiQianLeaderRole
        {
            get
            {
                return (HuiQianLeaderRole)this.GetValIntByKey(BtnAttr.HuiQianLeaderRole);
            }
        }

        public string AddLeaderLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AddLeaderLab);
            }
        }

        public bool AddLeaderEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AddLeaderEnable);
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
        /// 公文标签
        /// </summary>
        public string OfficeBtnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.OfficeBtnLab);
            }
        }
        /// <summary>
        /// 公文标签
        /// </summary>
        public bool OfficeBtnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.OfficeBtnEnable);
            }
        }
        public int OfficeBtnEnableInt
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.OfficeBtnEnable);
            }
        }
        /// <summary>
        /// 备注标签
        /// </summary>
        public string NoteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.NoteLab);
            }
        }
        /// <summary>
        ///备注标签
        /// </summary>
        public int NoteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NoteEnable);
            }
        }

        /// <summary>
        /// 公文显示的位置
        /// </summary>
        public int OfficeBtnLocal
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.OfficeBtnLocal);
            }
        }


        public string QRCodeLab
        {
            get
            {
                return this.GetValStrByKey(BtnAttr.QRCodeLab);
            }
        }
        /// <summary>
        /// 二维码规则
        /// </summary>
        public int QRCodeRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.QRCodeRole);
            }
        }

        public bool FrmDBVerEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.FrmDBVerEnable);
            }
        }
        /// <summary>
        /// 数据版本
        /// </summary>
        public bool FrmDBVerMyCC
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.FrmDBVerMyCC);
            }
        }
        public bool FrmDBVerMyView
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.FrmDBVerMyView);
            }
        }
        public string FrmDBVerLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.FrmDBVerLab);
            }
        }
        /// <summary>
        /// 小纸条
        /// </summary>
        public string ScripLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ScripLab);
            }
        }
        public int ScripRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.ScripRole);
            }
        }
        /// <summary>
        /// 评论
        /// </summary>
        public string FlowBBSLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.FlowBBSLab);
            }
        }
        public int FlowBBSRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.FlowBBSRole);
            }
        }

        public string IMLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.IMLab);
            }
        }
        public bool IMEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.IMEnable);
            }
        }

        public bool PressEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PressEnable);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLab() { }
        /// <summary>
        /// 节点按钮权限
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public BtnLab(int nodeid)
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

                Map map = new Map("WF_Node", "节点按钮标签");

                map.AddGroupAttr("处理器按钮权限");
                map.AddTBIntPK(BtnAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(BtnAttr.Name, null, "节点名称", true, true, 0, 200, 10);

                #region  功能按钮状态
                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SendLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3577079&doc_id=31094");
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 999, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow 发送前数据完整性判断"); //增加帮助.
                map.SetHelperUrl(BtnAttr.SendJS, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3577079&doc_id=31094");

                map.AddTBString(BtnAttr.DelayedSendLab, "延期发送", "延期发送按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.DelayedSendEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3577137&doc_id=31094"); //增加帮助


                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "抄送规则", true, true, NodeAttr.CCRole,
                    "@0=不能抄送@1=手工抄送@2=自动抄送@3=手工与自动@4=按表单SysCCEmps字段计算@5=在发送前打开抄送窗口");
                map.SetHelperUrl(BtnAttr.CCLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579867&doc_id=31094"); //增加帮助.

                map.AddTBString(BtnAttr.QRCodeLab, "二维码", "二维码标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.QRCodeLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=4275245&doc_id=31094"); //增加帮助.

                map.AddDDLSysEnum(BtnAttr.QRCodeRole, 0, "二维码规则", true, true, BtnAttr.QRCodeRole,
                    "@0=无@1=查看流程表单-无需权限@2=查看流程表单-需要登录@3=外部账户协作模式处理工作");

                // add 2014-04-05.
                //  map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送写入规则",
                //true, true, NodeAttr.CCWriteTo, "@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表", true);
                //  map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //增加帮助.

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "是否启用", true, true);

                map.SetHelperUrl(BtnAttr.ShiftLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979829&doc_id=31094");


                map.AddTBString(BtnAttr.DelLab, "删除", "删除按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "删除规则", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979834&doc_id=31094"); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979861&doc_id=31094"); //增加帮助

                map.AddTBString(BtnAttr.ShowParentFormLab, "查看父流程", "查看父流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShowParentFormEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ShowParentFormLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979871&doc_id=31094"); //增加帮助

              
                #region 公文相关.
                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "打开公文", "公文按钮标签", true, false, 0, 50, 10, false);
                map.SetHelperUrl(BtnAttr.OfficeBtnLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979875&doc_id=31094"); //增加帮助

                map.AddDDLSysEnum(BtnAttr.OfficeBtnEnable, 0, "文件状态", true, true, BtnAttr.OfficeBtnEnable,
                "@0=不可用@1=可编辑@2=不可编辑", false);

                map.AddDDLSysEnum(BtnAttr.OfficeFileType, 0, "文件类型", true, true, BtnAttr.OfficeFileType,
            "@0=word文件@1=WPS文件", false);

                map.AddDDLSysEnum(BtnAttr.OfficeBtnLocal, 0, "按钮位置", true, true, BtnAttr.OfficeBtnLocal,
         "@0=工具栏上@1=表单标签(divID=GovDocFile)", false);
                #endregion 公文相关.

                //map.AddTBString(BtnAttr.OfficeBtnLab, "公文主文件", "公文按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OfficeBtnEnable, false, "是否启用", true, true);

                //map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.TrackLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579860&doc_id=31094");//增加帮助

                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助
                //map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=5169441&doc_id=31094"); //增加帮助.

                //      map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                //      map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
                //true, true, BtnAttr.SelectAccepterEnable);
                //      map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10, false);
                map.AddBoolean(BtnAttr.SearchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助
                map.SetHelperUrl(BtnAttr.SearchLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979975&doc_id=31094");

                //map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //增加帮助

                //功能暂时取消
                //map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");

             
                // add by 周朋 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10, true);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true, true);
                map.SetHelperUrl(BtnAttr.TCLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3938664&doc_id=31094");

                map.AddTBString(BtnAttr.FrmDBVerLab, "数据版本", "数据版本", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FrmDBVerEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.FrmDBRemarkLab, "数据批阅", "数据批阅", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.FrmDBRemarkEnable, 0, "数据批阅", true, true, BtnAttr.FrmDBRemarkEnable, "@0=禁用@1=可编辑@2=不可编辑", false);


                //map.AddTBString(BtnAttr.AskforLab, "执行", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "加签"]); //增加帮助

                // 删除了这个模式,让表单方案进行控制了,保留这两个字段以兼容.
                //  map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", false, false, 0, 50, 10);
                // map.AddTBInt(BtnAttr.WebOfficeEnable, 0, "文档启用方式", false, false);

                //cut bye zhoupeng.
                //map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                //  "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");//edited by liuxc,2016-01-18,from xc
                //map.SetHelperUrl(BtnAttr.WebOfficeLab, "http://ccbpm.mydoc.io/?v=5404&t=17993");

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PRIEnable, 0, "重要性规则", true, true, BtnAttr.PRIEnable, @"0=不启用@1=只读@2=编辑");
                //map.AddBoolean(BtnAttr.PRIEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.PRILab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979978&doc_id=31094");

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CHRole, 0, "时限规则", true, true, BtnAttr.CHRole, @"0=禁用@1=启用@2=只读@3=启用并可以调整流程应完成时间");
                map.SetHelperUrl(BtnAttr.CHLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979979&doc_id=31094");

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分配", "分配按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.AllotLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979980&doc_id=31094");

                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "关注", "关注", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.FocusLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979981&doc_id=31094");

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "确认", "确认按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ConfirmLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979983&doc_id=31094");

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "列表", "列表按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ListLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979983&doc_id=31094");

                // 批量审核
                map.AddTBString(BtnAttr.BatchLab, "批量审核", "批量审核标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.BatchLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979986&doc_id=31094");

                //备注 流程不流转，设置备注信息提醒已处理人员当前流程运行情况
                map.AddTBString(BtnAttr.NoteLab, "备注", "备注标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NoteEnable, 0, "启用规则", true, true, BtnAttr.NoteEnable, @"0=禁用@1=启用@2=只读");
                map.SetHelperUrl(BtnAttr.NoteLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979987&doc_id=31094");

                //for 周大福.
                map.AddTBString(BtnAttr.HelpLab, "帮助", "帮助标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HelpRole, 0, "帮助显示规则", true, true, BtnAttr.HelpRole, @"0=禁用@1=启用@2=强制提示@3=选择性提示");
                map.SetHelperUrl(BtnAttr.HelpLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979989&doc_id=31094");

                //新增时间2022-03-14
                map.AddTBString(BtnAttr.ScripLab, "小纸条", "小纸条标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.ScripRole, 0, "小纸条显示规则", true, true, BtnAttr.ScripRole, @"0=禁用@1=按钮启用@2=发送启用");

                map.AddTBString(BtnAttr.FlowBBSLab, "评论", "评论标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.FlowBBSRole, 0, "评论规则", true, true, BtnAttr.FlowBBSRole, @"0=禁用@1=启用@2=只读");

                map.AddTBString(BtnAttr.IMLab, "即时通讯", "即时通讯标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.IMEnable, 0, "即时通讯规则", true, true, BtnAttr.IMEnable, @"0=禁用@1=启用");


                map.AddTBString(BtnAttr.NextLab, "下一条", "下一条", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NextRole, 0, "获得规则", true, true, BtnAttr.NextRole, @"0=禁用@1=相同节点@2=相同流程@3=相同的人@4=不限流程", true);
                map.SetHelperUrl(BtnAttr.NextLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979990&doc_id=31094");


                // 合流节点的：不常用的功能移动到这里.
                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3577139&doc_id=31094"); //增加帮助

                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "子线程删除方式", true, true,
           NodeAttr.ThreadKillRole, "@0=不能删除@1=手工删除@2=自动删除", true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579441&doc_id=31094"); //增加帮助

                //跳转.
                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.JumpWayLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3980001&doc_id=31094"); //增加帮助.

                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "跳转规则", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 200, 10, true);
                map.AddTBString(BtnAttr.ChangeDeptLab, "切换部门", "切换部门标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ChangeDeptEnable, false, "是否启用切换组织/部门", true, true);
                map.AddTBString("SubmitSF", "提交身份", "提交身份", true, false, 0, 50, 10);
                map.AddBoolean("SubmitSFEnable", false, "是否启用提交身份?", true, true);


                map.AddGroupAttr("查看器按钮权限");
                map.AddBoolean(BtnAttr.ShowParentFormEnableMyView, false, "查看父流程是否启用", true, true);
                map.AddBoolean(BtnAttr.TrackEnableMyView, true, "轨迹是否启用", true, true);
                map.AddBoolean(BtnAttr.FrmDBVerMyView, false, "数据版本是否启用", true, true);

                map.AddDDLSysEnum(BtnAttr.FrmDBRemarkEnableMyView, 0, "数据批阅", true, true, BtnAttr.FrmDBRemarkEnable, "@0=禁用@1=可编辑@2=不可编辑", false);
                // 催办
                map.AddTBString(BtnAttr.PressLab, "催办", "催办", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PressEnable, true, "是否启用", true, true);
                map.SetHelperAlert(BtnAttr.PressLab, "是否在在途的流程查看器上显示催办按钮？");

                map.AddTBString(BtnAttr.RollbackLab, "回滚", "回滚", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.RollbackEnable, true, "是否启用", true, true);
                map.SetHelperAlert(BtnAttr.RollbackLab, "流程结束后是否在查看器上显示回滚操作?");

                map.AddGroupAttr("抄送器按钮权限");
                map.AddBoolean(BtnAttr.ShowParentFormEnableMyCC, false, "查看父流程是否启用", true, true);
                map.AddBoolean(BtnAttr.TrackEnableMyCC, true, "轨迹是否启用", true, true);
                map.AddBoolean(BtnAttr.FrmDBVerMyCC, false, "数据版本是否启用", true, true);
                
                #endregion  功能按钮状态

                #region 退回处理.
                map.AddGroupAttr("退回规则");
                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);

                string returnRole = "@0=不能退回@1=只能退回上一个节点@2=可以退回任意节点@3=退回指定的节点";
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0, "退回规则", true, true, NodeAttr.ReturnRole, returnRole);

               // map.AddDDLSysEnum(NodeAttr.ReturnRole, 0, "退回规则", true, true, NodeAttr.ReturnRole);
                map.SetHelperUrl(NodeAttr.ReturnRole, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579467&doc_id=31094"); //增加帮助.
                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 999, 10, true);

                //  map.AddBoolean(NodeAttr.IsBackTracking, false, "是否可以原路返回(启用退回功能才有效)", true, true, false);

                map.AddDDLSysEnum(NodeAttr.IsBackTracking, 0, "是否可以原路返回", true, true, NodeAttr.IsBackTracking,
                 "@0=不允许原路返回@1=由退回人决定@2=强制原路返回");


                map.SetHelperUrl(NodeAttr.IsBackTracking, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579850&doc_id=31094"); //增加帮助.
                map.AddBoolean(NodeAttr.IsBackResetAccepter, false, "原路返回后是否自动计算接收人.", true, true, false);
                map.SetHelperAlert(NodeAttr.IsBackResetAccepter, "退回到此节点后，是否重新计算接受人，还是直接发送给退回人。");

                map.AddDDLSysEnum(NodeAttr.IsKillEtcThread, 0, "子线程退回,其它子线程删除规则", true, true, NodeAttr.IsKillEtcThread,
                    "@0=不删除其它的子线程@1=删除其它的子线程@2=由子线程退回人决定是否删除");

                //   map.SetHelperAlert(NodeAttr.IsKillEtcThread, "子线程退回到分流节点是，是否允许全部退回。");

                map.AddBoolean(NodeAttr.ReturnCHEnable, false, "是否启用退回考核规则", true, true);
                map.SetHelperUrl(NodeAttr.ReturnCHEnable, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3980009&doc_id=31094"); //增加帮助.

                map.AddDDLSysEnum(NodeAttr.ReturnOneNodeRole, 0, "单节点退回规则", true, true, NodeAttr.ReturnOneNodeRole,
                   "@0=不启用@1=按照[退回信息填写字段]作为退回意见直接退回@2=按照[审核组件]填写的信息作为退回意见直接退回", true);
                //map.AddTBString(NodeAttr.RetunFieldsLable, "退回扩展字段", "退回扩展字段", true, false, 0, 50, 20);

                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.ReturnField, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3579854&doc_id=31094"); //增加帮助.

                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 999, 10, true);
                #endregion 退回处理.

                #region 打印.
                map.AddGroupAttr("打印");
                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "打印Html", "打印Html标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "(打印Html)是否启用", true, true);
                map.SetHelperUrl(BtnAttr.PrintHtmlLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979893&doc_id=31094"); //增加帮助
                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintHtmlMyView, false, "(打印Html)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintHtmlMyCC, false, "(打印Html)显示在抄送工具栏?", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintPDFLab, "打印pdf", "打印pdf标签", true, false, 0, 50, 10, false);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "(打印pdf)是否启用", true, true);
                map.SetHelperUrl(BtnAttr.PrintPDFLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979894&doc_id=31094"); //增加帮助

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintPDFMyView, false, "(打印pdf)显示在查看器工具栏?", true, true, false);
                map.AddBoolean(BtnAttr.PrintPDFMyCC, false, "(打印pdf)显示在抄送工具栏?", true, true, false);

                map.AddDDLSysEnum(BtnAttr.PrintPDFModle, 0, "PDF打印规则", true, true, BtnAttr.PrintPDFModle,
                    "@0=全部打印@1=单个表单打印(针对树形表单)", false);
                map.AddTBString(BtnAttr.ShuiYinModle, null, "PDF水印内容", true, false, 20, 100, 100);
                map.SetHelperUrl(BtnAttr.ShuiYinModle, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=4055911&doc_id=31094"); //增加帮助

                map.AddTBString(BtnAttr.PrintZipLab, "打包下载", "打包下载zip标签", true, false, 0, 50, 10, false);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "(打包下载zip)是否启用", true, true);
                map.SetHelperUrl(BtnAttr.PrintZipLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979897&doc_id=31094"); //增加帮助

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintZipMyView, false, "(打包下载zip)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintZipMyCC, false, "(打包下载zip)显示在抄送工具栏?", true, true);

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印rtf按钮标签", true, false, 0, 50, 10);
                map.SetHelperAlert(BtnAttr.PrintDocLab, "请在节点表单里面配置要打印的模板，设置表单=》表单属性=》打印模板。"); //增加帮助
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //增加帮助
                //map.SetHelperUrl(BtnAttr.BatchLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979986&doc_id=31094");
                #endregion

                #region 会签按钮.
                map.AddGroupAttr("会签");
                map.AddTBString(BtnAttr.HuiQianLab, "会签", "按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole, "@0=不启用@1=协作(同事)模式@4=组长(领导)模式");
                map.SetHelperUrl(BtnAttr.HuiQianLab, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3979976&doc_id=31094");
                //map.AddDDLSysEnum(BtnAttr.IsCanAddHuiQianer, 0, "协作模式被加签的人处理规则", true, true, BtnAttr.IsCanAddHuiQianer,
                //   "0=不允许增加其他协作人@1=允许增加协作人", false);
                map.AddTBString(BtnAttr.AddLeaderLab, "加主持人", "加主持人", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AddLeaderEnable, false, "是否启用", true, true);
                map.AddDDLSysEnum(BtnAttr.HuiQianLeaderRole, 0, "组长会签规则", true, true, BtnAttr.HuiQianLeaderRole, "0=只有一个组长@1=最后一个组长发送@2=任意组长可以发送");
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            fl.Update();

            BtnLab btnLab = new BtnLab();
            btnLab.NodeID = this.NodeID;
            btnLab.RetrieveFromDBSources();
            btnLab.Update();

            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 节点按钮权限s
    /// </summary>
    public class BtnLabs : Entities
    {
        /// <summary>
        /// 节点按钮权限s
        /// </summary>
        public BtnLabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BtnLab();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BtnLab> ToJavaList()
        {
            return (System.Collections.Generic.IList<BtnLab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BtnLab> Tolist()
        {
            System.Collections.Generic.List<BtnLab> list = new System.Collections.Generic.List<BtnLab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BtnLab)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
