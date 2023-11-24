using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Sys
{
    /// <summary>
    /// 文件保存方式
    /// </summary>
    public enum AthSaveWay
    {
        /// <summary>
        /// IIS服务器
        /// </summary>
        IISServer,
        /// <summary>
        /// 保存到数据库
        /// </summary>
        DB,
        /// <summary>
        /// ftp
        /// </summary>
        FTPServer,
        /// <summary>
        /// OSS
        /// </summary>
        OSS,
        /// <summary>
        /// 保存链接
        /// </summary>
        SaveLink
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum AthRunModel
    {
        /// <summary>
        /// 记录模式
        /// </summary>
        RecordModel,
        /// <summary>
        /// 固定模式
        /// </summary>
        FixModel
    }
    /// <summary>
    /// 上传校验,方式.
    /// </summary>
    public enum UploadFileNumCheck
    {
        /// <summary>
        /// 不校验
        /// </summary>
        None,
        /// <summary>
        /// 不能为空
        /// </summary>
        NotEmpty,
        /// <summary>
        /// 每个类别不能为空.
        /// </summary>
        EverySortNoteEmpty
    }
    public enum AthCtrlWay
    {
        /// <summary>
        /// 表单主键
        /// </summary>
        PK,
        /// <summary>
        /// FID
        /// </summary>
        FID,
        /// <summary>
        /// 父流程ID
        /// </summary>
        PWorkID,
        /// <summary>
        /// 仅仅查看自己的
        /// </summary>
        MySelfOnly,
        /// <summary>
        /// 工作ID,对流程有效.
        /// </summary>
        WorkID,
        /// <summary>
        /// P2流程
        /// </summary>
        P2WorkID,
        /// <summary>
        /// P3流程
        /// </summary>
        P3WorkID,
        /// <summary>
        /// 根流程的WorkID
        /// </summary>
        RootFlowWorkID


    }
    /// <summary>
    /// 附件上传类型
    /// </summary>
    public enum AttachmentUploadType
    {
        /// <summary>
        /// 单个的
        /// </summary>
        Single,
        /// <summary>
        /// 多个的
        /// </summary>
        Multi,
        /// <summary>
        /// 指定的
        /// </summary>
        Specifically
    }
    /// <summary>
    /// 附件上传方式
    /// </summary>
    public enum AthUploadWay
    {
        /// <summary>
        /// 继承模式
        /// </summary>
        Inherit,
        /// <summary>
        /// 协作模式
        /// </summary>
        Interwork
    }
    /// <summary>
    /// 文件展现方式
    /// </summary>
    public enum FileShowWay
    {
        /// <summary>
        /// 表格
        /// </summary>
        Table,
        /// <summary>
        /// 图片
        /// </summary>
        Pict,
        /// <summary>
        /// 自由模式
        /// </summary>
        Free
    }

    /// <summary>
    /// 附件删除规则
    /// </summary>
    public enum AthDeleteWay
    {
        /// <summary>
        /// 不删除 0
        /// </summary>
        None,
        /// <summary>
        /// 删除所有 1
        /// </summary>
        DelAll,
        /// <summary>
        /// 只删除自己上传 2
        /// </summary>
        DelSelf
    }

}
