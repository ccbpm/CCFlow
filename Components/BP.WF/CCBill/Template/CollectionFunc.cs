using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 功能执行
    /// </summary>
    public class CollectionFunc : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.FrmID);
            }
            set
            {
                this.SetValByKey(CollectionAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.MethodID);
            }
            set
            {
                this.SetValByKey(CollectionAttr.MethodID, value);
            }
        }

        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(CollectionAttr.MsgErr, value);
            }
        }
        public string MsgSuccess
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.MsgSuccess);
            }
            set
            {
                this.SetValByKey(CollectionAttr.MsgSuccess, value);
            }
        }
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(CollectionAttr.Tag1);
            }
            set
            {
                this.SetValByKey(CollectionAttr.Tag1, value);
            }
        }
       
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(MethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(MethodAttr.RefMethodType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 功能执行
        /// </summary>
        public CollectionFunc()
        {
        }
        public CollectionFunc(string mypk)
        {
            this.No = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Collection", "功能方法");


                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);

                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                map.AddTBString(MethodAttr.Mark, null, "功能说明", true, false, 0, 900, 10, true);
                map.SetHelperAlert(MethodAttr.Mark, "对于该功能的描述.");


                
                map.AddTBString(MethodAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10, true);
                //map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", true, true, MethodAttr.ShowModel,
                //  "@0=按钮@1=超链接");

                map.AddDDLSysEnum(MethodAttr.MethodDocTypeOfFunc, 0, "内容类型", true, false, "MethodDocTypeOfFunc",
               "@0=SQL@1=URL@2=JavaScript@3=业务单元");

                map.AddTBString(MethodAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);

               

                RefMethod rm = new RefMethod();
                
                rm.Title = "方法内容"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDocs";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoParas()
        {
         
            return "../../CCBill/Admin/MethodParas.htm?No=" + this.MethodID;
        }
        /// <summary>
        /// 方法内容
        /// </summary>
        /// <returns></returns>
        public string DoDocs()
        {
            return "../../CCBill/Admin/MethodDoc/Default.htm?No=" + this.No;
        }
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        #endregion 执行方法.

    }
    /// <summary>
    /// 功能执行
    /// </summary>
    public class CollectionFuncs : EntitiesNoName
    {
        /// <summary>
        /// 功能执行
        /// </summary>
        public CollectionFuncs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CollectionFunc();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CollectionFunc> ToJavaList()
        {
            return (System.Collections.Generic.IList<CollectionFunc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CollectionFunc> Tolist()
        {
            System.Collections.Generic.List<CollectionFunc> list = new System.Collections.Generic.List<CollectionFunc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CollectionFunc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
