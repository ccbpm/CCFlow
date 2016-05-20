using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CCForm
{
    public class BtnFlow
    {

        #region attrs
        public const string Send = "Send";
        public const string Save = "Save";
        public const string Return = "Return";
        public const string CC = "CC";
        public const string Shift = "Shift";
        public const string Del = "Del";
        public const string EndFlow = "EndFlow";
        public const string Rpt = "Rpt";
        public const string Ath = "Ath";
        public const string Track = "Track";
        public const string Opt = "Opt";
        #endregion

        #region 属性
        private string _ID = null;
        private string _Lab = null;
        #endregion 属性


        #region 属性
        /// <summary>
        /// 图标名称
        /// </summary>
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        /// <summary>
        /// 图标文本
        /// </summary>
        public string Lab
        {
            get { return _Lab; }
            set { _Lab = value; }
        }
        #endregion

        #region 单一实例
        public static readonly BtnFlow instance = new BtnFlow();
        #endregion

        #region 公共方法

        public List<BtnFlow> getBtnFlowList()
        {
            List<BtnFlow> BtnFlowList = new List<BtnFlow>()
            {
                new BtnFlow(){ ID=BtnFlow.Send, Lab="发送" },
                new BtnFlow(){ ID=BtnFlow.Save, Lab="保存" },
                new BtnFlow(){ ID=BtnFlow.Return, Lab="退回" },
                new BtnFlow(){ ID=BtnFlow.Shift, Lab="移交" },
                new BtnFlow(){ ID=BtnFlow.Del, Lab="鼠标" },
                new BtnFlow(){ ID=BtnFlow.EndFlow, Lab="鼠标" },
                new BtnFlow(){ ID=BtnFlow.Ath, Lab="附件" },
                new BtnFlow(){ ID=BtnFlow.Track, Lab="轨迹" },
                new BtnFlow(){ ID=BtnFlow.Opt, Lab="选项" }
            };
            return BtnFlowList;
        }
        #endregion
    }
}
