using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.CCBill
{
	/// <summary>
	/// 连接方法
	/// </summary>
    public class FrmMethodLink : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodID, value);
            }
        }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.GetValStringByKey(FrmMethodAttr.MethodName);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodName, value);
            }
        }
        public string MethodDoc_Url
        {
            get
            {
                string s = this.GetValStringByKey(FrmMethodAttr.MethodDoc_Url);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "http://192.168.0.100/MyPath/xxx.xx";
                return s;
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodDoc_Url, value);
            }
        }
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(FrmMethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.RefMethodType, (int)value);
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
        public FrmMethodLink()
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


                Map map = new Map("WF_FrmMethod", "连接");

                map.AddMyPK();

                map.AddTBString(FrmMethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(FrmMethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddDDLSysEnum(FrmMethodAttr.RefMethodType, 0, "方法类型", true, true, "RefMethodTypeLink",
                  "@1=模态窗口打开@2=新窗口打开@3=右侧窗口打开");

                map.AddDDLSysEnum(FrmMethodAttr.ShowModel, 0, "显示方式", true, true, FrmMethodAttr.ShowModel,
                 "@0=按钮@1=超链接");

                map.AddTBString(FrmMethodAttr.MethodName, null, "方法名", true, false, 0, 300, 10, true);
                map.AddTBStringDoc(FrmMethodAttr.MethodDoc_Url, null, "连接URL", true, false);

                #region 工具栏.
                map.AddBoolean(FrmMethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                map.AddBoolean(FrmMethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(FrmMethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
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
    public class FrmMethodLinks : EntitiesMyPK
    {
        /// <summary>
        /// 连接方法
        /// </summary>
        public FrmMethodLinks() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmMethodLink();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmMethodLink> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmMethodLink>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmMethodLink> Tolist()
        {
            System.Collections.Generic.List<FrmMethodLink> list = new System.Collections.Generic.List<FrmMethodLink>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmMethodLink)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
