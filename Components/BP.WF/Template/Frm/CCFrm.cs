using BP.Sys;
using BP.En;

namespace BP.WF.Template.Frm
{
	/// <summary>
	/// CCFrm属性
	/// </summary>
    public class CCFrmAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 运行类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// URL
        /// </summary>
        public const string URL = "URL";
        /// <summary>
        /// 是否可以更新
        /// </summary>
        public const string IsUpdate = "IsUpdate";
        /// <summary>
        /// PTable
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// DBURL
        /// </summary>
        public const string DBURL = "DBURL";
    }
	/// <summary>
	/// 表单
	/// </summary>
    public class CCFrm : EntityNoName
    {
        #region 基本属性
        public FrmNode HisFrmNode = null;
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(CCFrmAttr.PTable);
            }
            set
            {
                this.SetValByKey(CCFrmAttr.PTable, value);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey(CCFrmAttr.URL);
            }
            set
            {
                this.SetValByKey(CCFrmAttr.URL, value);
            }
        }
        public FrmType HisFrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(CCFrmAttr.FrmType);
            }
            set
            {
                this.SetValByKey(CCFrmAttr.FrmType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// CCFrm
        /// </summary>
        public CCFrm()
        {
        }
        /// <summary>
        /// CCFrm
        /// </summary>
        /// <param name="no"></param>
        public CCFrm(string no)
            : base(no)
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

                Map map = new Map("Sys_MapData", "表单");
                map.CodeStruct = "4";

                map.AddTBStringPK(CCFrmAttr.No, null, null, true, true, 1, 200, 4);
                map.AddTBString(CCFrmAttr.Name, null, null, true, false, 0, 50, 10);
                map.AddTBString(CCFrmAttr.FK_Flow, null, "独立表单属性:FK_Flow", true, false, 0, 4, 10);
             //   map.AddDDLSysEnum(CCFrmAttr.CCFrmType, 0, "独立表单属性:运行类型", true, false, CCFrmAttr.CCFrmType);

                //表单的运行类型.
                map.AddDDLSysEnum(CCFrmAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型",true, false, CCFrmAttr.FrmType);

                map.AddTBString(CCFrmAttr.PTable, null, "物理表", true, false, 0, 50, 10);
                map.AddTBInt(CCFrmAttr.DBURL, 0, "DBURL", true, false);

                // 如果是个嵌入式表单.
                map.AddTBString(CCFrmAttr.URL, null, "Url", true, false, 0, 50, 10);

                //表单类别.
                map.AddTBString(MapDataAttr.FK_FormTree, "01", "表单类别", true, false, 0, 500, 20);

                map.AddTBInt(MapDataAttr.FrmW, 900, "表单宽度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        public int FrmW
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.FrmW);
            }
        }
      
        
        #endregion
    }
	/// <summary>
    /// 表单s
	/// </summary>
    public class CCFrms : EntitiesNoName
    {
        /// <summary>
        /// CCFrm
        /// </summary>
        public CCFrms()
        {
        }
        /// <summary>
        /// CCFrm
        /// </summary>
        /// <param name="fk_flow"></param>
        public CCFrms(string fk_flow)
        {
            this.Retrieve(CCFrmAttr.FK_Flow, fk_flow);
        }
        public CCFrms(int fk_node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(CCFrmAttr.No, "SELECT FK_CCFrm FROM WF_CCFrmNode WHERE FK_Node=" + fk_node);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCFrm();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCFrm> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCFrm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCFrm> Tolist()
        {
            System.Collections.Generic.List<CCFrm> list = new System.Collections.Generic.List<CCFrm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCFrm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
