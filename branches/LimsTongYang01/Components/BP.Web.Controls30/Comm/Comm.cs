using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace BP.Web.Comm
{
    public enum UIRowStyleGlo
    {
        /// <summary>
        /// 没有风格
        /// </summary>
        None,
        /// <summary>
        /// 交替风格
        /// </summary>
        Alternately,
        /// <summary>
        /// 鼠标移动
        /// </summary>
        Mouse,
        /// <summary>
        /// 鼠标移动并交替
        /// </summary>
        MouseAndAlternately
    }
    public enum ActionType
    {
        /// <summary>
        /// 不做任何事情
        /// </summary>
        None = 0,
        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteFile = 1,
        /// <summary>
        /// 打印只有一个实体的单据
        /// </summary>
        PrintEnBill = 2
    }

    public enum ShowWay
    {
        /// <summary>
        /// 缩落图
        /// </summary>
        Cards,
        /// <summary>
        /// 列表
        /// </summary>
        List,
        /// <summary>
        /// 详细信息
        /// </summary>
        Dtl
    }

}
 