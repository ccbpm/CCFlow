using BP.DA;
using BP.Web;
using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class MethodQRCode : EntityNoName
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
        /// 二维码
        /// </summary>
        public MethodQRCode()
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

                Map map = new Map("Frm_Method", "二维码");

                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Mark, null, "Mark", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", true, true, MethodAttr.ShowModel, "@0=按钮@1=超链接");
                 
                map.AddTBString(MethodAttr.MethodDoc_Url, null, "连接URL", true, false, 0, 300, 10);

                #region 工具栏.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 工具栏.

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
    /// 二维码
    /// </summary>
    public class MethodQRCodes : EntitiesNoName
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public MethodQRCodes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodQRCode();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodQRCode> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodQRCode>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodQRCode> Tolist()
        {
            System.Collections.Generic.List<MethodQRCode> list = new System.Collections.Generic.List<MethodQRCode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodQRCode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
