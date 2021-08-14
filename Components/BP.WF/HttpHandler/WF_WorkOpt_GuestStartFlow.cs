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
    public class WF_WorkOpt_GuestStartFlow : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_GuestStartFlow()
        {
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <returns></returns>
        public string GenerCode_Init()
        {
            string url = "";

            url = SystemConfig.HostURL + "/WF/WorkOpt/GuestStartFlow/ScanGuide.htm?WorkID=0&FK_Flow=" + this.FK_Flow;

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
            tempPath = SystemConfig.PathOfTemp + "\\" + this.FK_Flow + ".png";

            image.Save(tempPath, ImageFormat.Png);
            image.Dispose();

            //返回url.
            return url;
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            BP.WF.Port.User usr = new Port.User(this.No);
            if (usr.CheckPass(this.GetRequestVal("Pass")) == true)
            {
                BP.WF.Dev2InterfaceGuest.Port_Login(usr.No, usr.Name);
                return "登录成功.";
            }

            return "err@密码或用户名错误.";
        }
        /// <summary>
        /// 一定是外部用户的扫描后的处理.
        /// </summary>
        /// <returns></returns>
        public string ScanGuide_Init()
        {
            //如果当前不是外部用户登录，就转到登录或者注册。
            if (BP.Web.WebUser.No == null || BP.Web.WebUser.No.Equals("Guest") == false)
                return "Login.htm?FK_Flow=" + this.FK_Flow;

            return "../../MyFlow.htm?FK_Flow=" + this.FK_Flow;
        }

    }
}
