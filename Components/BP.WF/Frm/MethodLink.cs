using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.Frm
{
	/// <summary>
	/// 连接方法
	/// </summary>
    public class MethodLink : EntityMyPK
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
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodName);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodName, value);
            }
        }
        public string MethodDoc_Url
        {
            get
            {
                string s = this.GetValStringByKey(MethodAttr.MethodDoc_Url);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "http://192.168.0.100/MyPath/xxx.xx";
                return s;
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDoc_Url, value);
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

                map.AddMyPK();

                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddDDLSysEnum(MethodAttr.RefMethodType, 0, "方法类型", true, true, "RefMethodTypeLink",
                  "@1=模态窗口打开@2=新窗口打开@3=右侧窗口打开");

                map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", true, true, MethodAttr.ShowModel,
                 "@0=按钮@1=超链接");

                map.AddTBString(MethodAttr.MethodName, null, "方法名", true, false, 0, 300, 10, true);
                map.AddTBStringDoc(MethodAttr.MethodDoc_Url, null, "连接URL", true, false);

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
    }
	/// <summary>
	/// 连接方法
	/// </summary>
    public class MethodLinks : EntitiesMyPK
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
