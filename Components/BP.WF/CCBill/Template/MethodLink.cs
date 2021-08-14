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
    /// 连接方法
    /// </summary>
    public class MethodLink : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodID, value);
            }
        }
        #endregion

        #region 构造方法
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
        /// 连接方法
        /// </summary>
        public MethodLink()
        {
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

                Map map = new Map("Frm_Method", "连接");

                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "链接标签", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", false, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", false, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", false, false, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "链接地址", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.Mark, null, "Mark", false, true, 0, 300, 10);

                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", false, false, MethodAttr.ShowModel, "@0=按钮@1=超链接");

                map.AddDDLSysEnum(MethodAttr.RefMethodType, 0, "页面打开方式", true, true,
                    "RefMethodTypeLink", "@0=模态窗口打开@1=新窗口打开@2=右侧窗口打开@4=转到新页面");
                //是否显示到列表.
                map.AddBoolean(MethodAttr.IsList, false, "是否显示在列表?", true, true);
                
                map.AddTBInt(MethodAttr.PopWidth, 500, "宽度", true, false);
                map.AddTBInt(MethodAttr.PopHeight, 700, "高度", true, false);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 连接方法
    /// </summary>
    public class MethodLinks : EntitiesNoName
    {
        /// <summary>
        /// 连接方法
        /// </summary>
        public MethodLinks() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodLink();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodLink> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodLink>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodLink> Tolist()
        {
            System.Collections.Generic.List<MethodLink> list = new System.Collections.Generic.List<MethodLink>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodLink)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
