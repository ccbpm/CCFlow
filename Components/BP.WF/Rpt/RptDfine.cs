using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.WF;
using BP.Sys;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表定义
    /// </summary>
    public class RptDfineAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 查询的物理表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
    }
    /// <summary>
    /// 报表定义
    /// </summary>
    public class RptDfine : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(RptDfineAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(RptDfineAttr.PTable, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(RptDfineAttr.Note);
            }
            set
            {
                this.SetValByKey(RptDfineAttr.Note, value);
            }
        }
        public Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    _HisEns = BP.En.ClassFactory.GetEns(this.No);
                }
                return _HisEns;
            }
        }
        public Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 报表定义
        /// </summary>
        public RptDfine()
        {
        }
        /// <summary>
        /// 报表定义
        /// </summary>
        /// <param name="no">映射编号</param>
        public RptDfine(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "报表定义");

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("4"); ;

                #region 基本属性.
                map.AddTBStringPK(RptDfineAttr.No, null, "编号", true, false, 1, 200, 20);
                map.AddTBString(RptDfineAttr.Name, null, "描述", true, false, 0, 500, 20);
                map.AddTBString(RptDfineAttr.PTable, null, "物理表", true, false, 0, 500, 20);
                map.AddTBString(RptDfineAttr.Note, null, "备注", true, false, 0, 500, 20);
                #endregion 基本属性.

                #region 我发起的流程.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SelectCols.png";
                rm.ClassMethodName = this.ToString() + ".DoColsChoseOf_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示列次序";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Order.png";
                rm.ClassMethodName = this.ToString() + ".DoColsOrder_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置查询条件";
                rm.Icon = "../../WF/Admin/RptDfine/Img/SearchCond.png";
                rm.ClassMethodName = this.ToString() + ".DoSearchCond_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "设置导出模板";
                rm.Icon = "../../WF/Img/Guide.png";
                rm.ClassMethodName = this.ToString() + ".DoRptExportTemplate_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "执行查询";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoSearch_MyStartFlow()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "恢复设置";
                rm.Icon = "../../WF/Admin/RptDfine/Img/Reset.png";
                rm.Warning = "您确定要执行吗?";
                rm.ClassMethodName = this.ToString() + ".DoReset_MyStartFlow()";
                rm.RefMethodType = RefMethodType.Func;
                rm.GroupName = "我发起的流程";
                map.AddRefMethod(rm);

                #endregion 我发起的流程.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 我发起的流程
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoColsChoseOf_MyStartFlow()
        {
            string url = "../../Admin/RptDfine/S2ColsChose.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
            return url;
        }
        /// <summary>
        /// 列的次序
        /// </summary>
        /// <returns></returns>
        public string DoColsOrder_MyStartFlow()
        {
            string url = "../../Admin/RptDfine/S3ColsLabel.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
            return url;
        }       
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoSearchCond_MyStartFlow()
        {
            string url = "../../Admin/RptDfine/S5SearchCond.htm?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
            return url;
        }
        /// <summary>
        /// 导出模版.
        /// </summary>
        /// <returns></returns>
        public string DoRptExportTemplate_MyStartFlow()
        {
            string url = "../../Admin/RptDfine/S8_RptExportTemplate.aspx?FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
            return url;
        }
        /// <summary>
        /// 重置设置.
        /// </summary>
        public void DoReset_MyStartFlow()
        {
            MapData md = new MapData("ND" + int.Parse(this.No) + "MyRpt");
            md.RptIsSearchKey = true;
            md.RptDTSearchWay = DTSearchWay.None;
            md.RptDTSearchKey = "";
            md.RptSearchKeys = "*FK_Dept*WFSta*FK_NY*";

            Flow fl = new Flow(this.No);
            this.PTable = fl.PTable;
            this.Update();

            string keys = "'OID','FK_Dept','FlowStarter','WFState','Title','FlowStartRDT','FlowEmps','FlowDaySpan','FlowEnder','FlowEnderRDT','FK_NY','FlowEndNode','WFSta'";
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.No) + "Rpt");

            attrs.Delete(MapAttrAttr.FK_MapData, this.No); // 删除已经有的字段。
            foreach (MapAttr attr in attrs)
            {
                if (keys.Contains("'" + attr.KeyOfEn + "'") == false)
                    continue;
                attr.FK_MapData = this.No;
                attr.Insert();
            }
        }
        /// <summary>
        /// 设置选择的列
        /// </summary>
        /// <returns></returns>
        public string DoSearch_MyStartFlow()
        {
            return "../../Rpt/MyStartFlow.htm?FK_MapData=" + this.No + "&FK_Flow=" + this.No + "&RptNo=ND" + int.Parse(this.No) + "MyRpt";
        }
        #endregion

    }
    /// <summary>
    /// 报表定义s
    /// </summary>
    public class RptDfines : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 报表定义s
        /// </summary>
        public RptDfines()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new RptDfine();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<RptDfine> ToJavaList()
        {
            return (System.Collections.Generic.IList<RptDfine>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RptDfine> Tolist()
        {
            System.Collections.Generic.List<RptDfine> list = new System.Collections.Generic.List<RptDfine>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RptDfine)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
