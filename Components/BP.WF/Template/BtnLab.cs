﻿using System;
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
                map.AddDDLSysEnum(BtnAttr.OfficeBtnEnable, 0, "文件状态", true, true, BtnAttr.OfficeBtnEnable,
                "@@0=不可用@1=可编辑@2=不可编辑", false);


                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.ReturnRole, 1, "是否启用", true, true);
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
                map.SetHelperAlert(BtnAttr.ShowParentFormLab,"如果当前流程实例不是子流程，即时启用了，也不显示该按钮。");

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
                //map.AddDDLSysEnum(BtnAttr.IsCanAddHuiQianer, 0, "协作模式被加签的人处理规则", true, true, BtnAttr.IsCanAddHuiQianer,
                 //    "0=不允许增加其他协作人@1=允许增加协作人", false);


                map.AddDDLSysEnum(BtnAttr.HuiQianLeaderRole, 0, "会签组长规则", true, true, BtnAttr.HuiQianLeaderRole,
                     "0=只有一个组长@1=最后一个组长发送@2=任意组长发送",true);

                map.AddTBString(BtnAttr.AddLeaderLab, "加主持人", "加主持人", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AddLeaderEnable, false, "是否启用", true, true);

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
                map.AddDDLSysEnum(BtnAttr.CHRole, 0, "时限规则", true, true, BtnAttr.CHRole, @"0=禁用@1=启用@2=只读@3=启用并可以调整流程应完成时间");

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

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintHtmlMyView, false, "(打印Html)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintHtmlMyCC, false, "(打印Html)显示在抄送工具栏?", true, true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintPDFLab, "打印pdf", "打印pdf标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "是否启用", true, true);

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintPDFMyView, false, "(打印pdf)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintPDFMyCC, false, "(打印pdf)显示在抄送工具栏?", true, true, false);


                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintZipLab, "打包下载", "打包下载zip按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "是否启用", true, true);

                // add 2020.5.25 for 交投集团.
                map.AddBoolean(BtnAttr.PrintZipMyView, false, "(打包下载zip)显示在查看器工具栏?", true, true);
                map.AddBoolean(BtnAttr.PrintZipMyCC, false, "(打包下载zip)显示在抄送工具栏?", true, true, false);

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "列表", "列表按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "是否启用", true, true);

                //备注 流程不流转，设置备注信息提醒已处理人员当前流程运行情况
                map.AddTBString(BtnAttr.NoteLab, "备注", "备注标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NoteEnable, 0, "启用规则", true, true, BtnAttr.NoteEnable, @"0=禁用@1=启用@2=只读");

                //for 周大福.
                map.AddTBString(BtnAttr.HelpLab, "帮助", "帮助标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HelpRole, 0, "帮助显示规则", true, true, BtnAttr.HelpRole, @"0=禁用@1=启用@2=强制提示@3=选择性提示");


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
