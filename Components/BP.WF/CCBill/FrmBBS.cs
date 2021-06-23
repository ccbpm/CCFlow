using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    ///  评论组件-属性
    /// </summary>
    public class FrmBBSAttr : EntityTreeAttr
    {
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 活动类型
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// 活动类型名称
        /// </summary>
        public const string ActionTypeText = "ActionTypeText";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 部门No
        /// </summary>
        public const string DeptNo = "DeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 参数信息
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 表单数据
        /// </summary>
        public const string FrmDB = "FrmDB";
        /// <summary>
        /// 消息
        /// </summary>
        public const string Msg = "Msg";
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 表单名称
        /// </summary>
        public const string FrmName = "FrmName";

    }
    /// <summary>
    /// 评论组件
    /// </summary>
    public class FrmBBS : EntityNoName
    {
        #region 字段属性.
        /// <summary>
        /// 参数数据.
        /// </summary>
        public string Tag
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.Tag);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.Tag, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FrmName
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.FrmName);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.FrmName, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.RDT);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.RDT, value);
            }
        }

        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(FrmBBSAttr.WorkID);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 活动类型
        /// </summary>
        public BP.CCBill.FrmActionType FrmActionType
        {
            get
            {
                return (BP.CCBill.FrmActionType)this.GetValIntByKey(FrmBBSAttr.ActionType);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.ActionType, (int)value);
            }
        }
        /// <summary>
        /// 获取动作文本
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        public static string GetActionTypeT(BP.CCBill.FrmActionType at)
        {
            switch (at)
            {
                case BP.CCBill.FrmActionType.Save:
                    return "保存";
                case BP.CCBill.FrmActionType.Create:
                    return "提交";
                case BP.CCBill.FrmActionType.BBS:
                    return "评论";
                case BP.CCBill.FrmActionType.View:
                    return "打开";
                default:
                    return "信息" + at.ToString();
            }
        }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActionTypeText
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.ActionTypeText);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.ActionTypeText, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.Rec);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录人名字
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.RecName, value);
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.Msg);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.Msg, value);
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public string MsgHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(FrmBBSAttr.Msg);
            }
        }
        #endregion attrs

        #region 流程属性.
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.DeptNo);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.DeptNo, value);
            }
        }
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(FrmBBSAttr.DeptName);
            }
            set
            {
                this.SetValByKey(FrmBBSAttr.DeptName, value);
            }
        }
        #endregion 流程属性.

        #region 构造.
        /// <summary>
        /// 表单评论组件表
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_BBS", "表单评论组件表");

                #region 基本字段.

                map.AddTBStringPK(FrmBBSAttr.No, null, "No", true, false, 0, 50, 200);
                map.AddTBString(FrmBBSAttr.Name, null, "标题", true, false, 0, 4000, 200);
                map.AddTBString(FrmBBSAttr.ParentNo, null, "父节点", true, false, 0, 50, 200);
                map.AddTBInt(FrmBBSAttr.WorkID, 0, "工作ID/OID", true, false);

                //map.AddTBString(FrmBBSAttr.FrmID, null, "表单ID", true, false, 0, 50, 200);
                //map.AddTBString(FrmBBSAttr.FrmName, null, "表单名称(可以为空)", true, false, 0, 200, 200);
                //map.AddTBInt(FrmBBSAttr.ActionType, 0, "类型", true, false);
                // map.AddTBString(FrmBBSAttr.ActionTypeText, null, "类型(名称)", true, false, 0, 30, 100);

                map.AddTBString(FrmBBSAttr.Rec, null, "记录人", true, false, 0, 200, 100);
                map.AddTBString(FrmBBSAttr.RecName, null, "名称", true, false, 0, 200, 100);
                map.AddTBDateTime(FrmBBSAttr.RDT, null, "记录日期时间", true, false);

                map.AddTBString(FrmBBSAttr.DeptNo, null, "部门编号", true, false, 0, 200, 100);
                map.AddTBString(FrmBBSAttr.DeptName, null, "名称", true, false, 0, 200, 100);
                #endregion 基本字段

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 评论组件
        /// </summary>
        public FrmBBS()
        {
        }
        #endregion 构造.

        protected override bool beforeInsert()
        {

            this.No = DBAccess.GenerGUID();

            this.SetValByKey(FrmBBSAttr.Rec, BP.Web.WebUser.No);
            this.SetValByKey(FrmBBSAttr.RecName, BP.Web.WebUser.Name);
            this.SetValByKey(FrmBBSAttr.RDT,  DataType.CurrentDataTime);

            this.SetValByKey(FrmBBSAttr.DeptNo, BP.Web.WebUser.FK_Dept);
            this.SetValByKey(FrmBBSAttr.DeptName, BP.Web.WebUser.FK_DeptName);

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 评论组件集合s
    /// </summary>
    public class FrmBBSs : EntitiesNoName
    {
        #region 构造方法.
        /// <summary>
        /// 评论组件集合
        /// </summary>
        public FrmBBSs()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBBS();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBBS> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBBS>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBBS> Tolist()
        {
            System.Collections.Generic.List<FrmBBS> list = new System.Collections.Generic.List<FrmBBS>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBBS)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}