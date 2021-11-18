using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class WF_WorkOpt_QRCode : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_QRCode()
        {

        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            NodeExt ne = new NodeExt(this.FK_Node);
            int val = ne.GetValIntByKey(BtnAttr.QRCodeRole);
            if (val == 0)
                return "err@流程表单扫描已经关闭不允许扫描.";

            //如果是：预览无需要权限， 没有这样的情况.
            if (val == 1)
            {
                if (WebUser.No == null)
                    BP.WF.Dev2Interface.Port_Login("Guest");

                return "../../MyView.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }


            //如果是：预览需要权限， 检查是否可以查看该流程？.
            if (val == 2)
            {
                //使用内部用户登录.
                BP.Port.Emp emp = new Emp();
                emp.No = this.No;
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@用户名账号错误.";

                if (emp.CheckPass(this.GetRequestVal("Pass")) == false)
                    return "err@s输入密码错误.";

                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    BP.WF.Dev2Interface.Port_Login(emp.No);
                else
                    BP.WF.Dev2Interface.Port_Login(emp.No, null, emp.OrgNo);

                return "../../MyFlowView.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            //如果是：外部用户？.
            if (val == 3)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.FK_Node != this.FK_Node)
                    return "err@二维码过期";

                //使用内部用户登录.
                BP.WF.Port.User user = new Port.User();
                user.No = this.No;
                if (user.RetrieveFromDBSources() == 0)
                    return "err@用户名账号错误.";

                if (user.CheckPass(this.GetRequestVal("Pass")) == false)
                    return "err@用户名账号错误.";

                //执行登录.
                BP.WF.Dev2InterfaceGuest.Port_Login(user.No, user.Name);

                HuiQian_AddGuest(this.WorkID, this.FK_Node);

                return "../../MyFlow.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            return "err@系统错误.";
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <returns></returns>
        public string GenerCode_Init()
        {
            string url = "";

            
            if (this.WorkID == 0)  //开始节点的时候.
                url = SystemConfig.HostURL + "/WF/WorkOpt/QRCode/ScanGuide.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow;
            else
                url = SystemConfig.HostURL + "/WF/WorkOpt/QRCode/ScanGuide.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow;

            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;//错误效验、错误更正(有4个等级)
            encoder.QRCodeBackgroundColor = Color.White;
            encoder.QRCodeForegroundColor = Color.Black;

            //生成临时文件.
            System.Drawing.Image image = encoder.Encode(url, Encoding.UTF8);

            string tempPath = "";

            if (this.WorkID == 0)
                tempPath = SystemConfig.PathOfTemp + this.FK_Flow + ".png";
            else
                tempPath = SystemConfig.PathOfTemp + this.WorkID + ".png";

            image.Save(tempPath, ImageFormat.Png);
            image.Dispose();

            //返回url.
            return url;
        }
        public string ScanGuide_Init()
        {
            NodeExt ne = new NodeExt(this.FK_Node);
            int val = ne.GetValIntByKey(BtnAttr.QRCodeRole);

            if (val == 0)
                return "err@流程表单扫描已经关闭不允许扫描.";

            // 如果不需要权限 就可以查看表单.
            if (val == 1)
            {
                if (WebUser.No == null)
                    BP.WF.Dev2Interface.Port_Login("Guest");

                return "../../MyView.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            // 如果需要权限才能查看表单.
            if (val == 2)
            {
                //判断是否登录?
                if (WebUser.No == null)
                    return "Login.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&QRCodeRole=2";

                return "../../MyView.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            //外部账户协作模式处理工作.
            if (val == 3)
            {
                if (GuestUser.No == null)
                    return "Login.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&QRCodeRole=2";

                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.FK_Node != this.FK_Node)
                    return "err@二维码过期";

                HuiQian_AddGuest(this.WorkID, this.FK_Node);


                return "../../MyFlowView.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
            }

            return "err@没有判断的模式.";

        }

        private void HuiQian_AddGuest(Int64 workid, int fk_node)
        {
            //判断是否存在该节点的待办
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.FK_Node = fk_node;
            gwl.FK_Emp = GuestUser.No;
            gwl.WorkID = workid;
            int num = gwl.RetrieveFromDBSources();
            //还没有待办，增加会签人信息
            if (num == 0)
            {
                Node nd = new Node(fk_node);
                GenerWorkerList gwlZCR = null;
                //获取会签组长的信息
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                if (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                {
                    gwlZCR = new GenerWorkerList();
                    num = gwlZCR.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, fk_node, GenerWorkerListAttr.IsPass, 0);
                }
                else
                {
                    gwlZCR = new GenerWorkerList();
                    num = gwlZCR.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, fk_node, GenerWorkerListAttr.FK_Emp, gwf.HuiQianZhuChiRen);
                }
                if (num == 0)
                    throw new Exception("err@发生不可预测的问题,组长协作模式下找不到组长信息");
                gwf.HuiQianZhuChiRen = gwlZCR.FK_Emp;
                gwf.HuiQianZhuChiRenName = gwlZCR.FK_EmpText;
                gwlZCR.SetPara("HuiQianType", "");
                gwlZCR.FK_Emp = GuestUser.No;
                gwlZCR.FK_EmpText = GuestUser.Name;
                gwlZCR.IsPassInt = 0; //设置不可以用.
                gwlZCR.FK_Dept = "";
                gwlZCR.FK_DeptT = ""; //部门名称.
                gwlZCR.IsRead = false;
                gwlZCR.GuestNo = GuestUser.No;
                gwlZCR.GuestName = GuestUser.Name;
                gwlZCR.SetPara("HuiQianZhuChiRen", gwlZCR.FK_Emp);

                #region 计算会签时间.
                if (nd.HisCHWay == CHWay.None)
                {
                    gwlZCR.SDT = "无";
                }
                else
                {
                    //给会签人设置应该完成日期. 考虑到了节假日.                
                    DateTime dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, nd.TimeLimit,
                         nd.TimeLimitHH, nd.TimeLimitMM, nd.TWay);
                    //应完成日期.
                    gwlZCR.SDT = dtOfShould.ToString(DataType.SysDataTimeFormat + ":ss");
                }

                //求警告日期.
                DateTime dtOfWarning = DateTime.Now;
                //计算警告日期。
                // 增加小时数. 考虑到了节假日.
                if (nd.WarningDay != 0)
                    dtOfWarning = Glo.AddDayHoursSpan(DateTime.Now, nd.WarningDay, 0, 0, nd.TWay);

                gwlZCR.DTOfWarning = dtOfWarning.ToString(DataType.SysDataTimeFormat);
                #endregion 计算会签时间.

                gwlZCR.Sender = gwlZCR.FK_Emp + "," + gwlZCR.FK_EmpText; //发送人为当前人.
                gwlZCR.IsHuiQian = true;
                gwlZCR.Insert(); //插入作为待办.

                //修改GenerWorkFlow的信息
                //gwf.TodoEmps += GuestUser.No + "," + GuestUser.Name + ";";
                gwf.HuiQianTaskSta = HuiQianTaskSta.HuiQianing;
                gwf.Update();
                //给组长发送消息
                BP.WF.Dev2Interface.Port_SendMsg(gwlZCR.FK_Emp,
                  "bpm会签工作参与", "HuiQian" + gwf.WorkID + "_" + gwf.FK_Node + "_" + GuestUser.No, GuestUser.Name + "参与了您对工作的｛" + gwf.Title + "｝邀请,请您及时关注工作进度.", "HuiQian", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

                //执行会签,写入日志.
                BP.WF.Dev2Interface.WriteTrack(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, gwf.WorkID, gwf.FID, GuestUser.No + "," + GuestUser.Name,
                    ActionType.HuiQian, "执行会签", null, null, null, GuestUser.No, GuestUser.Name, gwlZCR.FK_Emp, gwlZCR.FK_EmpText);
                return;
            }


        }

    }
}
