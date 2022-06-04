using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.En
{

    /// <summary>
    /// 编辑类型
    /// </summary>
    public enum EditType
    {
        /// <summary>
        /// 可编辑
        /// </summary>
        Edit = 0,
        /// <summary>
        /// 不可删除
        /// </summary>
        UnDel = 1,
        /// <summary>
        /// 只读,不可删除。
        /// </summary>
        Readonly = 2
    }
    /// <summary>
    ///  控件类型
    /// </summary>
    public enum UIContralType
    {
        /// <summary>
        /// 文本框
        /// </summary>
        TB = 0,
        /// <summary>
        /// 下拉框
        /// </summary>
        DDL = 1,
        /// <summary>
        /// CheckBok
        /// </summary>
        CheckBok = 2,
        /// <summary>
        /// 单选择按钮
        /// </summary>
        RadioBtn = 3,
        /// <summary>
        /// 地图定位
        /// </summary>
        MapPin = 4,
        /// <summary>
        /// 录音控件
        /// </summary>
        MicHot = 5,
        /// <summary>
        /// 附件展示控件
        /// </summary>
        AthShow = 6,
        /// <summary>
        /// 手机拍照控件
        /// </summary>
        MobilePhoto = 7,
        /// <summary>
        /// 手写签名版
        /// </summary>
        HandWriting = 8,
        /// <summary>
        /// 超链接
        /// </summary>
        HyperLink = 9,
        /// <summary>
        /// 文本
        /// </summary>
        Lab = 10,
        /// <summary>
        /// 图片
        /// </summary>
        FrmImg = 11,
        /// <summary>
        /// 图片附件
        /// </summary>
        FrmImgAth = 12,
        /// <summary>
        /// 身份证号
        /// </summary>
        IDCard = 13,
        /// <summary>
        /// 签批组件
        /// </summary>
        SignCheck = 14,
        /// <summary>
        /// 评论组件
        /// </summary>
        FlowBBS = 15,
        /// <summary>
        /// 系统定位
        /// </summary>
        Fixed = 16,
        /// <summary>
        /// 公文正文组件
        /// </summary>
        GovDocFile = 110,
        /// <summary>
        /// 发文字号
        /// </summary>
        DocWord = 17,
        /// <summary>
        /// 收文字号
        /// </summary>
        DocWordReceive = 170,
        /// <summary>
        /// 流程进度图
        /// </summary>
        JobSchedule = 50,
        /// <summary>
        /// 大块文本Html(说明性文字)
        /// </summary>
        BigText = 60,
        /// <summary>
        /// 评分
        /// </summary>
        Score = 101

    }
    /// <summary>
    /// 逻辑类型
    /// </summary>
    public enum FieldTypeS
    {
        /// <summary>
        /// 普通类型
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 枚举类型
        /// </summary>
        Enum = 1,
        /// <summary>
        /// 外键
        /// </summary>
        FK = 2
    }
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 正常的
        /// </summary>
        Normal,
        /// <summary>
        /// 主键
        /// </summary>
        PK,
        /// <summary>
        /// 外键
        /// </summary>
        FK,
        /// <summary>
        /// 枚举
        /// </summary>
        Enum,
        /// <summary>
        /// 既是主键又是外键
        /// </summary>
        PKFK,
        /// <summary>
        /// 既是主键又是枚举
        /// </summary>
        PKEnum,
        /// <summary>
        /// 关连的文本.
        /// </summary>
        RefText,
        /// <summary>
        /// 虚拟的
        /// </summary>
        NormalVirtual,
        /// <summary>
        /// 多值的
        /// </summary>
        MultiValues
    }
    /// <summary>
    /// 实体附件类型
    /// </summary>
    public enum BPEntityAthType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 单附件
        /// </summary>
        Single,
        /// <summary>
        /// 多附件
        /// </summary>
        Multi
    }
     
    /// <summary>
    /// 实体类型
    /// </summary>
    public enum EnType
    {
        /// <summary>
        /// 系统实体
        /// </summary>
        Sys,
        /// <summary>
        /// 管理员维护的实体
        /// </summary>
        Admin,
        /// <summary>
        /// 应用程序实体
        /// </summary>
        App,
        /// <summary>
        /// 第三方的实体（可以更新）
        /// </summary>
        ThirdPartApp,
        /// <summary>
        /// 视图(更新无效)
        /// </summary>
        View,
        /// <summary>
        /// 可以纳入权限管理
        /// </summary>
        PowerAble,
        /// <summary>
        /// 其他
        /// </summary>
        Etc,
        /// <summary>
        /// 明细或着点对点。
        /// </summary>
        Dtl,
        /// <summary>
        /// 点对点
        /// </summary>
        Dot2Dot,
        /// <summary>
        /// XML　类型
        /// </summary>
        XML,
        /// <summary>
        /// 扩展类型，它用于查询的需要。
        /// </summary>
        Ext
    }
    /// <summary>
    /// 移动到显示方式
    /// </summary>
    public enum MoveToShowWay
    {
        /// <summary>
        /// 不显示
        /// </summary>
        None,
        /// <summary>
        /// 下拉列表
        /// </summary>
        DDL,
        /// <summary>
        /// 平铺
        /// </summary>
        Panel
    }
}
