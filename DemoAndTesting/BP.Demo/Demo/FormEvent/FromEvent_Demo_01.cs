using System;
using System.Threading;
using System.Collections;
using BP.Web.Controls;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.Demo.FormEvent
{
    /// <summary>
    /// 表单事件demo.
    /// </summary>
    public class FromEvent_Demo_01 : BP.Sys.FormEventBase
    {
        /// <summary>
        /// 标记该表单事件实体是要注册到那个类上
        /// </summary>
        public override string FormMark
        {
            get { return "Demo_01"; }
        }

        public override string SaveBefore()
        {
            return base.SaveBefore();
        }

        public override string SaveAfter()
        {
            return base.SaveAfter();
        }

       
        public override string AthUploadeBefore()
        {
            //此时文件已经上传并保存到服务器，如果此方法返回非空信息，则文件会被删除，不记录附件信息，弹出上传附件失败，同时记录日志
            string athPK = this.SysPara["FK_FrmAttachment"] as string;  //附件
            string fileFullName = this.SysPara["FileFullName"] as string;   //附件文件存储路径

            //以下可写对文件的处理逻辑...
            //if (new System.IO.FileInfo(fileFullName).Length > 1024 * 1024)
            //    return "文件不能大于1M";

            return base.AthUploadeBefore();
        }

        public override string AthUploadeAfter()
        {
            //已经记录附件信息，如此方法返回非空信息，则文件不会被删除，只是记录日志
            string athPK = this.SysPara["FK_FrmAttachment"] as string;  //附件
            string athDBPK = this.SysPara["FK_FrmAttachmentDB"] as string;  //附件存储
            string fileFullName = this.SysPara["FileFullName"] as string;   //附件文件存储路径

            //以下可写对附件的处理逻辑...

            return base.AthUploadeAfter();
        }
    }
}
