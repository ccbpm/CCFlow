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
	/// 单实体流程查询
	/// </summary>
    public class MethodSingleDictGenerWorkFlow : EntityNoName
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
        /// 单实体流程查询
        /// </summary>
        public MethodSingleDictGenerWorkFlow()
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

                Map map = new Map("Frm_Method", "单个实体流程查询");

                map.AddMyPK();

                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 200, 10);
                map.AddTBString(MethodAttr.GroupID, null, "GroupID", true, true, 0, 200, 10);

                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.Mark, null, "Mark", true, false, 0, 300, 10);


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
	/// 单实体流程查询
	/// </summary>
    public class MethodSingleDictGenerWorkFlows : EntitiesMyPK
    {
        /// <summary>
        /// 单实体流程查询
        /// </summary>
        public MethodSingleDictGenerWorkFlows() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodSingleDictGenerWorkFlow();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodSingleDictGenerWorkFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodSingleDictGenerWorkFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodSingleDictGenerWorkFlow> Tolist()
        {
            System.Collections.Generic.List<MethodSingleDictGenerWorkFlow> list = new System.Collections.Generic.List<MethodSingleDictGenerWorkFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodSingleDictGenerWorkFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
