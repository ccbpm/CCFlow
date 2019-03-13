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
	/// 功能执行
	/// </summary>
    public class FrmMethodFunc : EntityMyPK
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
        public string MethodDoc
        {
            get
            {
                string s = this.GetValStringByKey(FrmMethodAttr.MethodDoc);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodDoc, value);
            }
        }
        /// <summary>
        /// 方法类型：@0=SQL@1=URL@2=JavaScript@3=业务单元
        /// </summary>
        public int MethodDocTypeOfFunc
        {
            get
            {
                return this.GetValIntByKey(FrmMethodAttr.MethodDocTypeOfFunc);
            }
            set
            {
                this.SetValByKey(FrmMethodAttr.MethodDocTypeOfFunc, value);
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
        public FrmMethodFunc()
        {
        }
        public FrmMethodFunc(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("WF_FrmMethod", "功能方法");
                map.AddMyPK();

                map.AddTBString(FrmMethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(FrmMethodAttr.MethodName, null, "方法名", true, false, 0, 300, 10, true);
                map.AddTBString(FrmMethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(FrmMethodAttr.WarningMsg, null, "功能执行警告信息", true, false, 0, 300, 10);
                map.AddDDLSysEnum(FrmMethodAttr.ShowModel, 0, "显示方式", true, true, FrmMethodAttr.ShowModel,
                  "@0=按钮@1=超链接");

                map.AddDDLSysEnum(FrmMethodAttr.MethodDocTypeOfFunc, 0, "内容类型", true, true, "MethodDocTypeOfFunc",
               "@0=SQL@1=URL@2=JavaScript@3=业务单元");

                map.AddTBStringDoc(FrmMethodAttr.MethodDoc, null, "执行内容", true, false);

                #region 显示位置控制.
                map.AddBoolean(FrmMethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true,true);
                map.AddBoolean(FrmMethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(FrmMethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.


                RefMethod rm = new RefMethod();
                rm.Title = "方法参数"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoParas";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
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

        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoParas()
        {
            return "../../CCBill/Admin/MethodParas.htm?MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 方法内容
        /// </summary>
        /// <returns></returns>
        public string DoDocs()
        {
            return "../../CCBill/Admin/MethodDoc.htm?MyPK=" + this.MyPK;
        }
    }
	/// <summary>
	/// 功能执行
	/// </summary>
    public class FrmMethodFuncs : EntitiesMyPK
    {
        /// <summary>
        /// 功能执行
        /// </summary>
        public FrmMethodFuncs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmMethodFunc();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmMethodFunc> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmMethodFunc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmMethodFunc> Tolist()
        {
            System.Collections.Generic.List<FrmMethodFunc> list = new System.Collections.Generic.List<FrmMethodFunc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmMethodFunc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
