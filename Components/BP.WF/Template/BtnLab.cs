using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 公文工作模式
    /// </summary>
    public enum WebOfficeWorkModel
    {
        /// <summary>
        /// 不启用
        /// </summary>
        None,
        /// <summary>
        /// 按钮方式启用
        /// </summary>
        Button,
        /// <summary>
        /// 表单在前
        /// </summary>
        FrmFirst,
        /// <summary>
        /// 文件在前
        /// </summary>
        WordFirst
    }
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
        public bool CHEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.CHEnable);
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

                Map map = new Map("WF_Node", "节点标签");

                map.Java_SetDepositaryOfEntity(Depositary.Application);

                map.AddTBIntPK(BtnAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(BtnAttr.Name, null, "节点名称", true, true, 0, 200, 10);

                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "发送按钮JS函数", true, false, 0, 50, 10, true);

                //map.AddBoolean(BtnAttr.SendEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(NodeAttr.JumpWay, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);


                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.ThreadIsCanDel, false, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "是否可以移交子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);

              
                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "打开公文", "公文按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeBtnEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ReturnRole, true, "是否启用", true, true);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(NodeAttr.ReturnOneNodeRole, 0, "单节点退回规则", true, true, NodeAttr.ReturnOneNodeRole,
                "@@0=不启用@1=按照[退回信息填写字段]作为退回意见直接退回@2=按照[审核组件]填写的信息作为退回意见直接退回", true);


                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CCRole, 0, "抄送规则", true, true, BtnAttr.CCRole);

                //  map.AddBoolean(BtnAttr, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.DelLab, "删除流程", "删除流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.DelEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.ShowParentFormLab, "查看父流程", "查看父流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShowParentFormEnable, false, "是否启用", true, true);


                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "方式",
          true, true, BtnAttr.SelectAccepterEnable);

                // map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.OptLab, "选项", "选项按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OptEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, true, "是否启用", true, true);

                // 
                map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                // 
                map.AddTBString(BtnAttr.BatchLab, "批量审核", "批量审核标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.AskforLab, "加签", "加签标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole,
                    "@0=不启用@1=协作模式@4=组长模式");

                //map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole, "@0=不启用@1=组长模式@2=协作模式");

               // map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
              //  map.AddBoolean(BtnAttr.HuiQianRole, false, "是否启用", true, true);

                // add by stone 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "公文标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WebOfficeEnable, false, "是否启用", true, true);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                 "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");  //edited by liuxc,2016-01-18,from xc.

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PRIEnable, false, "是否启用", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.CHEnable, false, "是否启用", true, true);

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分配", "分配按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "是否启用", true, true);


                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "关注", "关注", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, true, "是否启用", true, true);

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "确认", "确认按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "是否启用", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "打印Html", "打印Html标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "是否启用", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintPDFLab, "打印pdf", "打印pdf标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "是否启用", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintZipLab, "打包下载", "打包下载zip按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "是否启用", true, true);

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "列表", "列表按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "是否启用", true, true);


                #region 公文按钮
                map.AddTBString(BtnAttr.OfficeOpenLab, "打开本地", "打开本地标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "打开模板", "打开模板标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "保存", "保存标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeAcceptLab, "接受修订", "接受修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeRefuseLab, "拒绝修订", "拒绝修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeOverLab, "套红", "套红标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "是否查看用户留痕", true, true, true);

                map.AddTBString(BtnAttr.OfficePrintLab, "打印", "打印标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSealLab, "签章", "签章标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSealEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeInsertFlowLab, "插入流程", "插入流程标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "是否记录节点信息", true, true);
                map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "是否该自动保存为PDF", true, true);


                map.AddTBString(BtnAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeDownEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeIsMarks, true, "是否进入留痕模式", true, true);
                map.AddTBString(BtnAttr.OfficeTemplate, "", "指定文档模板", true, false, 0, 100, 10);
                map.AddBoolean(BtnAttr.OfficeIsParent, true, "是否使用父流程的文档", true, true);

                map.AddBoolean(BtnAttr.OfficeTHEnable, false, "是否自动套红", true, true);
                map.AddTBString(BtnAttr.OfficeTHTemplate, "", "自动套红模板", true, false, 0, 200, 10);

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
