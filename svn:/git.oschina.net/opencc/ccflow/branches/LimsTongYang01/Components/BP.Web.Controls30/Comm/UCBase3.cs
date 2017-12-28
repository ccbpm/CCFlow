using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.DA;

namespace BP.Web.UC
{
    /// <summary>
    /// Well 的摘要说明。
    /// </summary>
    public class UCBase3 : BP.Web.UC.UCBase2
    {
        #region 信息块- 套用
        public void DivInfoBlockBegin()
        {
            return;

            //this.Add("<style>");
            //this.Add(".xsnazzy span {width:20px; height:10px; w\\idth:0; hei\\ght:0;}");
            //this.Add(" .xb1, .xb2, .xb3, .xb4, .xb5, .xb6, .xb7 {display:block; overflow:hidden; font-size:0;}");
            //this.Add(".xb1, .xb2, .xb3, .xb4, .xb5, .xb6 {height:1px;}");
            //this.Add(".xb4, .xb5, .xb6, .xb7 {background:#ccc; border-left:1px solid #fff; border-right:1px solid #fff;}");
            //this.Add(".xb1 {margin:0 8px; background:#fff;}");
            //this.Add(".xb2 {margin:0 6px; background:#fff;}");
            //this.Add(".xb3 {margin:0 4px; background:#fff;}");
            //this.Add(".xb4 {margin:0 3px; background:#7f7f9c; border-width:0 5px;}");
            //this.Add(".xb5 {margin:0 2px; background:#7f7f9c; border-width:0 4px;}");
            //this.Add(".xb6 {margin:0 2px; background:#7f7f9c; border-width:0 3px;} ");
            //this.Add(".xb7 {margin:0 1px; background:#7f7f9c; border-width:0 3px; height:2px;} ");
            //this.Add(".xboxcontent {display:block; background:#7f7f9c; border:3px solid #fff; border-width:0 3px;}");
            //this.Add("</style>");

            //this.Add("<div class='container'>");
            //this.Add("<div class='xsnazzy' >");
            //this.Add("<b class='xb1'></b><b class=xb2></b><b class=xb3></b><b class=xb4></b><b class=xb5></b><b class=xb6></b><b class=xb7></b>");
            //this.Add("<div class='xboxcontent' >");
            //return;
        }

        public void DivInfoBlockEnd()
        {
            return;
            //this.AddDivEnd();
            //this.Add("<b class=xb7></b><b class=xb6></b><b class=xb5></b><b class=xb4></b><b class=xb3></b><b class=xb2></b><b class=xb1></b></div>");
            //return;

            //string path = this.Request.ApplicationPath;

            //this.Add("\n</td><td width=4 class='line_l' style='border-right:1px #ccc solid;background:#f9f9f9;'></td>");
            //this.Add("\n</tr>");
            //this.Add("\n<tr class='yj_style'>");
            //this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/bl_df.jpg'></td>");
            //this.Add("\n<td style='border-bottom:1px #ccc solid;background:#f9f9f9;'></td>");
            //this.Add("\n<td>");
            //this.Add("\n<img src='/WF/Img/Div/br_df.jpg'>");
            //this.Add("\n</td></tr>");
            //this.AddTableEnd();
        }

        #endregion 信息块- 套用


        #region AddMsgGreen
        public void AddMsgGreen(string title, string msg)
        {
            this.AddFieldSet(title);
            this.Add(msg);
            this.AddFieldSetEnd();
            return;


            //   this.DivInfoBlock(title, msg);
            //this.AddTableGreen();
            //this.AddTableBarGreen(title, 1);
            //if (msg != null)
            //{
            //    this.AddTR();
            //    this.Add("<TD class=BigDoc >" + msg + "</TD>");
            //    this.AddTREnd();
            //}
            //this.AddTableEnd();
        }

        public void AddMsgInfo(string title, string msg)
        {

            this.DivInfoBlockRed(title, msg);

            //this.AddTable();

            //this.AddTR();
            //this.Add("<TD class=TitleDef >" + title + "</TD>");
            //this.AddTREnd();

            //this.AddTR();
            //this.Add("<TD class=BigDoc >" + msg + "</TD>");
            //this.AddTREnd();

            //this.AddTableEnd();
        }
        #endregion

        #region 信息块- 函数
        /// <summary>
        /// 默认的信息块 
        /// </summary>
        /// <param name="html">html信息</param>
        public void DivInfoBlock(string html)
        {
            this.DivInfoBlockBegin();
            this.Add(html);
            this.DivInfoBlockEnd();
        }
        /// <summary>
        /// 默认的信息块
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="html">内容</param>
        public void DivInfoBlock(string title, string html)
        {
            this.DivInfoBlockBegin();
            this.Add("<b>" + title + "</b><br>");
            this.Add(html);
            this.DivInfoBlockEnd();
        }
        public void DivInfoBlockRed(string html)
        {
            DivInfoBlockRed(null, html);
        }
        /// <summary>
        /// 红色的信息块 
        /// </summary>
        /// <param name="html">html信息</param>
        public void DivInfoBlockRed(string title, string html)
        {
            string path = this.Request.ApplicationPath;
            this.Add("\n<table  cellspacing='0'>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/tl_red.jpg'></td>");
            this.Add("\n<td style='border-top:1px #ffb9b6 solid;background:#ffebea;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/tr_red.jpg'>");
            this.Add("\n</td></tr>");
            this.Add("\n<tr><td width=4 class='line_l' style='border-left:1px #ffb9b6 solid;background:#ffebea;'></td><td width='100%' style='background:#ffebea;'>");

            if (title != null)
                this.Add("<b>" + title + "</b><br>");

            this.Add(html);

            this.Add("\n</td><td width=4 class='line_l' style='border-right:1px #ffb9b6 solid;background:#ffebea;'></td>");
            this.Add("\n</tr>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/bl_red.jpg'></td>");
            this.Add("\n<td style='border-bottom:1px #ffb9b6 solid;background:#ffebea;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/br_red.jpg'>");
            this.Add("\n</td></tr>");
            this.AddTableEnd();
        }
        /// <summary>
        /// 绿色的信息块 
        /// </summary>
        /// <param name="html">html信息</param>
        public void DivInfoBlockGreen(string html)
        {
            string path = this.Request.ApplicationPath;
            this.Add("\n<table cellspacing='0'>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/tl_green.jpg'></td>");
            this.Add("\n<td style='border-top:1px #b5d95e solid;background:#efffc9;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/tr_green.jpg'>");
            this.Add("\n</td></tr>");
            this.Add("\n<tr><td width=4 class='line_l' style='border-left:1px #b5d95e solid;background:#efffc9;'></td><td width='100%' style='background:#efffc9;'>");
            this.Add(html);
            this.Add("\n</td><td width=4 class='line_l' style='border-right:1px #b5d95e solid;background:#efffc9;'></td>");
            this.Add("\n</tr>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/bl_green.jpg'></td>");
            this.Add("\n<td style='border-bottom:1px #b5d95e solid;background:#efffc9;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/br_green.jpg'>");
            this.Add("\n</td></tr>");
            this.AddTableEnd();
        }
        /// <summary>
        /// 蓝色的信息块 
        /// </summary>
        /// <param name="html">html信息</param>
        public void DivInfoBlockBlue(string html)
        {
            string path = this.Request.ApplicationPath;
            this.Add("\n<table  cellspacing='0'>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/tl_Blue.jpg'></td>");
            this.Add("\n<td style='border-top:1px #b5e8fa solid;background:#f0fbff;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/tr_Blue.jpg'>");
            this.Add("\n</td></tr>");
            this.Add("\n<tr><td width=4 class='line_l' style='border-left:1px #b5e8fa solid;background:#f0fbff;'></td><td width='100%' style='background:#f0fbff;'>");
            this.Add(html);
            this.Add("\n</td><td width=4 class='line_l' style='border-right:1px #b5e8fa solid;background:#f0fbff;'></td>");
            this.Add("\n</tr>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/bl_Blue.jpg'></td>");
            this.Add("\n<td style='border-bottom:1px #b5e8fa solid;background:#f0fbff;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/br_Blue.jpg'>");
            this.Add("\n</td></tr>");
            this.AddTableEnd();
        }
        /// <summary>
        /// 黄色的信息块 
        /// </summary>
        /// <param name="html">html信息</param>
        public void DivInfoBlockYellow(string html)
        {
            string path = this.Request.ApplicationPath;
            this.Add("\n<table  cellspacing='0'>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/tl_yellow.jpg'></td>");
            this.Add("\n<td style='border-top:1px #f1e167 solid;background:#fffce5;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/tr_yellow.jpg'>");
            this.Add("\n</td></tr>");
            this.Add("\n<tr><td width=4 class='line_l' style='border-left:1px #f1e167 solid;background:#fffce5;'></td><td width='100%' style='background:#fffce5;'>");
            this.Add(html);
            this.Add("\n</td><td width=4 class='line_l' style='border-right:1px #f1e167 solid;background:#fffce5;'></td>");
            this.Add("\n</tr>");
            this.Add("\n<tr>");
            this.Add("\n<td style='text-align:right'><img src='/WF/Img/Div/bl_yellow.jpg'></td>");
            this.Add("\n<td style='border-bottom:1px #f1e167 solid;background:#fffce5;'></td>");
            this.Add("\n<td>");
            this.Add("\n<img src='/WF/Img/Div/br_yellow.jpg'>");
            this.Add("\n</td></tr>");
            this.AddTableEnd();

        }
        #endregion 信息块

        #region 菜单
        /// <summary>
        /// 显示菜单
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void Menu(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table style='border-bottom:1px #96c1cc solid;border-collapse:collapse;' cellpadding='0' cellspacing='1' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='2%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuS><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=Menu ><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }

        /// <summary>
        /// 显示菜单Red
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void MenuRed(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table style='border-bottom:1px #75001b solid;' cellpadding='0' cellspacing='0' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='2%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuS><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=MenuRed><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD  ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }

        /// <summary>
        /// 显示菜单 Green
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void MenuGreen(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table width='100%' style='border-bottom:1px #5c8a0b solid;' cellpadding='0' cellspacing='0' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='24%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuS><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=MenuGreen><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD   ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }

        /// <summary>
        /// 显示菜单 Blue
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void MenuBlue(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table style='border-bottom:1px #4d71c3 solid;' cellpadding='0' cellspacing='0' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='2%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuS><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=MenuBlue><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }

        /// <summary>
        /// 显示菜单 Yellow
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void MenuYellow(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table style='border-bottom:1px #ffcc00 solid;' cellpadding='0' cellspacing='0' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='2%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuS><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=MenuYellow><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }



        /// <summary>
        /// 显示菜单 Win7
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selectVal"></param>
        public void MenuWin7(BP.Sys.XML.XmlMenus ens, string selectVal)
        {
            this.Add("\n<Table  cellpadding='0' cellspacing='0' >");
            this.Add("\n<TR>");
            this.Add("\n<TD width='2%' ></TD>");
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                if (selectVal == en.No)
                    this.Add("\n<TD class=MenuWin7S><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
                else
                    this.Add("\n<TD class=MenuWin7><a href='" + en.Url + "' target=" + en.Target + ">" + en.Name + "</a></TD>");
            }
            this.Add("\n<TD  ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }

        #endregion


        #region 生成菜单的方法 着的。
        public void MenuSelfVerticalBegin()
        {
            this.Add("<ul class=MenuSelfVertical >");
        }
        public void MenuSelfVerticalItem(string url, string lab, string target)
        {
            if (target == null)
                target = "_self";
            this.Add("\t\n<li class=MenuSelfVerticalItem ><a href=\"" + url + "\" target=" + target + " >" + lab + "</li>");
        }
        public void MenuSelfVerticalItemS(string url, string lab, string target)
        {
            if (target == null)
                target = "_self";
            this.Add("\t\n<li class=MenuSelfVerticalItemS ><a href=\"" + url + "\" target=" + target + " >" + lab + "</li>");
        }
        public void MenuSelfVerticalEnd()
        {
            this.Add("</ul>");
        }
        #endregion 生成菜单的方法 横着的。


        #region 生成菜单的方法 横着的。
        /// <summary>
        /// 开始增加菜单
        /// </summary>
        public void MenuSelfBegin()
        {
            this.Add("\n<Table style='width:100%;' cellpadding='5' cellspacing='5'>");
            this.Add("\n<TR>");
        }
        /// <summary>
        /// 增加一个lab
        /// </summary>
        /// <param name="attr">TD里面的属性</param>
        /// <param name="lab">标签</param>
        public void MenuSelfLab(string attr, string lab)
        {
            this.Add("\n<TD " + attr + ">" + lab + "</TD>");
        }

        public void MenuSelfItem(string url, string lab, string target)
        {
            this.Add("\n<TD class=Menu><a href=\"" + url + "\" target=" + target + ">" + lab + "</a></TD>");
        }

        public void MenuSelfItemLab(string lab)
        {
            this.Add("\n<TD class=Menu>" + lab + "</TD>");
        }

        public void MenuSelfItem(string url, string lab, string target, bool selected)
        {
            if (selected == false)
                MenuSelfItem(url, lab, target);
            else
                MenuSelfItemS(url, lab, target);
        }
        public void MenuSelfItemS(string url, string lab, string target)
        {
            this.Add("\n<TD class=MenuS >" + lab + "</TD>");
        }
        /// <summary>
        /// 结束菜单
        /// </summary>
        public void MenuSelfEnd(int perBlankLeft)
        {
            this.Add("\n<TD width='" + perBlankLeft + "%' ></TD>");
            this.Add("\n</TR>");
            this.AddTableEnd();
        }
        /// <summary>
        /// 结束菜单
        /// </summary>
        public void MenuSelfEnd()
        {
            this.Add("\n</TR>");
            this.AddTableEnd();
        }
        #endregion 菜单

        #region EasyUI样式的Panel信息展示方法 added by liuxc,2014-10-22,edited by liuxc,2014-11-28

        /// <summary>
        /// 增加一个EasyUi的Panel,展示一小段信息，带有标题
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">要展示的信息</param>
        /// <param name="iconCls">标题前面的图标，必须是EasyUi中icon.css中定义的类</param>
        /// <param name="padding">Panel内部边距(单位:px)</param>
        public void AddEasyUiPanelInfo(string title, string msg, string iconCls = "icon-tip", int padding = 10)
        {
            AddEasyUiPanelInfoBegin(title, iconCls, padding);
            Add(msg);
            AddEasyUiPanelInfoEnd();
        }

        /// <summary>
        /// 开始增加一个EasyUi的Panel，带有标题
        /// <remarks>
        /// <para>注意：用于AddEasyUiPanelInfoEnd方法之前，两者必须配合使用</para>
        /// </remarks>
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="iconCls">标题前面的图标，必须是EasyUi中icon.css中定义的类</param>
        /// <param name="padding">Panel内部边距(单位:px)</param>
        public void AddEasyUiPanelInfoBegin(string title, string iconCls = "icon-tip", int padding = 10)
        {
            Add("<div style='width:100%'>");
            Add(string.Format("<div class='easyui-panel' title='{0}' data-options=\"iconCls:'{1}',fit:true\" style='height:auto;padding:{2}px'>", title, iconCls, padding));
        }

        /// <summary>
        /// 结束增加一个EasyUi的Panel，带有标题
        /// <remarks>
        /// <para>注意：用于AddEasyUiPanelInfoBegin方法之后，两者必须配合使用</para>
        /// </remarks>
        /// </summary>
        public void AddEasyUiPanelInfoEnd()
        {
            Add("</div>");
            Add("</div>");
        }
        #endregion

      
    }
}
