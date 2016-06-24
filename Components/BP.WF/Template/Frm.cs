using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// Frm属性
	/// </summary>
    public class FrmAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 运行类型
        /// </summary>
        public const string FormRunType = "FormRunType";
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
    public class Frm : EntityNoName
    {
        #region 基本属性
        public FrmNode HisFrmNode = null;
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.PTable);
            }
            set
            {
                this.SetValByKey(FrmAttr.PTable, value);
            }
        }
        public string FK_Flow11
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmAttr.FK_Flow, value);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.URL);
            }
            set
            {
                this.SetValByKey(FrmAttr.URL, value);
            }
        }
        public FormRunType HisFormRunType
        {
            get
            {
                return (FormRunType)this.GetValIntByKey(FrmAttr.FormRunType);
            }
            set
            {
                this.SetValByKey(FrmAttr.FormRunType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Frm
        /// </summary>
        public Frm()
        {
        }
        /// <summary>
        /// Frm
        /// </summary>
        /// <param name="no"></param>
        public Frm(string no)
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

                Map map = new Map("Sys_MapData", "表单库");
                map.Java_SetCodeStruct("4");

                map.AddTBStringPK(FrmAttr.No, null, null, true, true, 1, 200, 4);
                map.AddTBString(FrmAttr.Name, null, null, true, false, 0, 50, 10);
                map.AddTBString(FrmAttr.FK_Flow, null, "独立表单属性:FK_Flow", true, false, 0, 50, 10);
             //   map.AddDDLSysEnum(FrmAttr.FormRunType, 0, "独立表单属性:运行类型", true, false, FrmAttr.FormRunType);

                //表单的运行类型.
                map.AddDDLSysEnum(FrmAttr.FormRunType, (int)BP.WF.FormRunType.FreeForm, "运行类型",
                    true, false, FrmAttr.FormRunType, "@0=傻瓜表单@1=自由表单@2=嵌入式表单@4=Excel表单@5=Word表单");

                map.AddTBString(FrmAttr.PTable, null, "物理表", true, false, 0, 50, 10);
                map.AddTBInt(FrmAttr.DBURL, 0, "DBURL", true, false);

                // 如果是个嵌入式表单.
                map.AddTBString(FrmAttr.URL, null, "Url", true, false, 0, 50, 10);

                //表单类别.
                map.AddTBString(MapDataAttr.FK_FrmSort, "01", "表单类别", true, false, 0, 500, 20);

                map.AddTBInt(BP.Sys.MapDataAttr.FrmW, 900, "表单宽度", true, false);
                map.AddTBInt(BP.Sys.MapDataAttr.FrmH, 1200, "表单高度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        public int FrmW
        {
            get
            {
                return this.GetValIntByKey(Sys.MapDataAttr.FrmW);
            }
        }
        public int FrmH
        {
            get
            {
                return this.GetValIntByKey(BP.Sys.MapDataAttr.FrmH);
            }
        }
        
        #endregion
    }
	/// <summary>
    /// 表单s
	/// </summary>
    public class Frms : EntitiesNoName
    {
        /// <summary>
        /// Frm
        /// </summary>
        public Frms()
        {
        }
        /// <summary>
        /// Frm
        /// </summary>
        /// <param name="fk_flow"></param>
        public Frms(string fk_flow)
        {
            this.Retrieve(FrmAttr.FK_Flow, fk_flow);
        }
        public Frms(int fk_node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(FrmAttr.No, "SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + fk_node);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Frm();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Frm> ToJavaList()
        {
            return (System.Collections.Generic.IList<Frm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Frm> Tolist()
        {
            System.Collections.Generic.List<Frm> list = new System.Collections.Generic.List<Frm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Frm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
