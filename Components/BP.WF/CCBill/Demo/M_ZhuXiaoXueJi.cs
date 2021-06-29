using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace BP.CCBill.Demo
{
    /// <summary>
    /// 注销学籍 方法
    /// </summary>
    public class M_ZhuXiaoXueJi : BP.En.Method
    {
        public M_ZhuXiaoXueJi()
        {
            this.Title = "注销学籍.";
            this.Help = "执行学籍的注销的业务逻辑， 关于该学生的借书信息、食堂信息等资料都需要注销掉.";
            this.GroupName = "CCBill的是实体Demo";
        }
        public override object Do()
        {
            //0. 获得参数.
            Int64 workid = this.GetValIntByKey("WorkID"); //实体主键.
            string frmID = this.GetValStrByKey("FrmID"); //实体主键.

            //1. 检查是否有食堂欠费。

            //2. 检查图书馆借书是否归还？

            //3. 执行注销.(以下是采用ccbpm的语法法.)
            GEEntity en = new GEEntity(frmID, workid);
            en.SetValByKey("XSZT", 3); //修改字段值，
            en.Update();

            return "学籍已经注销了。";
        }

        #region 重写。
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.IsAdmin == true)
                    return true;
                return false;
            }
        }
        #endregion 重写。

    }
}
