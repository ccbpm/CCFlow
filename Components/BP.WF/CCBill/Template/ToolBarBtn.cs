using BP.DA;
using BP.En;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 实体工具栏按钮属性
    /// </summary>
    public class ToolbarBtnAttr : EntityMyPKAttr
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 方法ID
        /// </summary>
        public const string BtnID = "BtnID";
        /// <summary>
        /// 按钮标签
        /// </summary>
        public const string BtnLab = "BtnLab";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        #endregion 基本属性.

        #region 外观.
        /// <summary>
        /// 宽度.
        /// </summary>
        public const string PopWidth = "PopWidth";
        /// <summary>
        /// 高度
        /// </summary>
        public const string PopHeight = "PopHeight";
        #endregion 外观.

        public const string IsEnable = "IsEnable";
    }
    /// <summary>
    /// 实体工具栏按钮
    /// </summary>
    public class ToolbarBtn : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(ToolbarBtnAttr.FrmID);
            }
            set
            {
                this.SetValByKey(ToolbarBtnAttr.FrmID, value);
            }
        }
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(ToolbarBtnAttr.Icon);
            }
            set
            {
                this.SetValByKey(ToolbarBtnAttr.Icon, value);
            }
        }

        /// <summary>
        /// 方法ID
        /// </summary>
        public string BtnID
        {
            get
            {
                return this.GetValStringByKey(ToolbarBtnAttr.BtnID);
            }
            set
            {
                this.SetValByKey(ToolbarBtnAttr.BtnID, value);
            }
        }
        public string BtnLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarBtnAttr.BtnLab);
            }
            set
            {
                this.SetValByKey(ToolbarBtnAttr.BtnLab, value);
            }
        }
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarBtnAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(ToolbarBtnAttr.IsEnable, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 实体工具栏按钮
        /// </summary>
        public ToolbarBtn()
        {
        }
        /// <summary>
        /// 实体工具栏按钮
        /// </summary>
        /// <param name="no"></param>
        public ToolbarBtn(string mypk)
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

                Map map = new Map("Frm_ToolbarBtn", "实体工具栏按钮");

                map.AddMyPK();

                //主键.
                map.AddTBString(ToolbarBtnAttr.BtnID, null, "按钮ID", true, true, 0, 300, 10);
                map.AddTBString(ToolbarBtnAttr.BtnLab, null, "标签", true, false, 0, 300, 10);


                //功能标记. 
                map.AddTBString(ToolbarBtnAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);

                map.AddTBString(ToolbarBtnAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                #region 外观.
                map.AddTBInt(ToolbarBtnAttr.PopHeight, 0, "弹窗高度", true, false);
                map.AddTBInt(ToolbarBtnAttr.PopWidth, 0, "弹窗宽度", true, false);
                #endregion 外观.

                //是否启用？
                map.AddBoolean(ToolbarBtnAttr.IsEnable, true, "是否启用？", true, true, true);
                map.AddTBInt(ToolbarBtnAttr.Idx, 0, "Idx", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion



        #region 移动.
        public void DoUp()
        {
            this.DoOrderUp(ToolbarBtnAttr.FrmID, this.FrmID, ToolbarBtnAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(ToolbarBtnAttr.FrmID, this.FrmID, ToolbarBtnAttr.Idx);
        }
        #endregion 移动.

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
                this.MyPK = DBAccess.GenerGUID();
            return base.beforeInsert();
        }

    }
    /// <summary>
    /// 实体工具栏按钮
    /// </summary>
    public class ToolbarBtns : EntitiesMyPK
    {
        /// <summary>
        /// 实体工具栏按钮
        /// </summary>
        public ToolbarBtns() { }
        /// <summary>
        /// 实体工具栏按钮
        /// </summary>
        /// <param name="nodeid">方法IDID</param>
        public ToolbarBtns(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(ToolbarBtnAttr.BtnID, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ToolbarBtn();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ToolbarBtn> ToJavaList()
        {
            return (System.Collections.Generic.IList<ToolbarBtn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ToolbarBtn> Tolist()
        {
            System.Collections.Generic.List<ToolbarBtn> list = new System.Collections.Generic.List<ToolbarBtn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ToolbarBtn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
