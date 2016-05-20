using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace BP.En
{
    public enum DBAChartType
    {
        Table,
        Column,
        Pie,
        Line
    }
    /// <summary>
    /// 柱状图显示类型
    /// </summary>
    public enum ColumnChartShowType
    {
        /// <summary>
        /// 不显示
        /// </summary>
        None,
        /// <summary>
        /// 横向
        /// </summary>
        HengXiang,
        /// <summary>
        /// 竖向
        /// </summary>
        ShuXiang,
        /// <summary>
        /// 横向叠加
        /// </summary>
        HengXiangAdd,
        /// <summary>
        /// 竖向叠加
        /// </summary>
        ShuXiangAdd
    }
    /// <summary>
    /// 折线图图显示类型
    /// </summary>
    public enum LineChartShowType
    {
        /// <summary>
        /// 不显示
        /// </summary>
        None,
        /// <summary>
        /// 横向
        /// </summary>
        HengXiang,
        /// <summary>
        /// 竖向
        /// </summary>
        ShuXiang
    }
    /// <summary>
    /// 数据报表
    /// </summary>
    public class Rpt2Attr
    {
        /// <summary>
        /// 数据报表
        /// </summary>
        public Rpt2Attr()
        {
        }
        private string _Title = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_Title))
                    return "";

                if (_Title.Contains("@") == false)
                    return _Title;

                string title = _Title.Clone() as string;

                title = title.Replace("@WebUser.No", BP.Web.WebUser.No);
                title = title.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                title = title.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                title = title.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                if (title.Contains("@") == false)
                    return title;

                foreach (string key in Glo.Request.QueryString)
                {
                    title = title.Replace("@" + key, Glo.Request.QueryString[key]);
                }

                if (title.Contains("@") == false)
                    return title;

                if (title.Contains("@") == false)
                    return title;

                BP.DA.AtPara ap = new AtPara(this.DefaultParas);
                foreach (string key in ap.HisHT.Keys)
                    title = title.Replace("@" + key, ap.GetValStrByKey(key));

                return title;
            }
            set
            {
                _Title = value;
            }
        }

        /// <summary>
        /// 左侧菜单标题
        /// </summary>
        public string LeftMenuTitle = "";
        /// <summary>
        /// 编码数据源
        /// </summary>
        public string DBSrc = "";
        /// <summary>
        /// 详细信息(可以为空)
        /// </summary>
        public string DBSrcOfDtl = "";
        /// <summary>
        /// 左边的菜单.
        /// </summary>
        public string LeftMenu = "";
        /// <summary>
        /// 高度.
        /// </summary>
        public int H = 420;
        /// <summary>
        /// 默认宽度
        /// </summary>
        public int W = 900;
        /// <summary>
        /// 底部文字.
        /// </summary>
        public string xAxisName = "";
        /// <summary>
        /// 右边文字
        /// </summary>
        public string yAxisName = "";
        /// <summary>
        /// 数值列的前缀
        /// </summary>
        public string numberPrefix = "";
        /// <summary>
        /// 图标的横向的参照线的条数.
        /// </summary>
        public int numDivLines = 8;
        /// <summary>
        /// 默认显示的数据图表.
        /// </summary>
        public DBAChartType DefaultShowChartType = DBAChartType.Column;
        /// <summary>
        /// 是否启用table.
        /// </summary>
        public bool IsEnableTable = true;

        /// <summary>
        /// 柱图显示类型.
        /// </summary>
        private ColumnChartShowType _ColumnChartShowType = ColumnChartShowType.ShuXiang;
        public ColumnChartShowType ColumnChartShowType
        {
            get
            {
                return _ColumnChartShowType;
            }
            set
            {
                _ColumnChartShowType = value;
            }
        }
        /// <summary>
        /// 是否显示饼图
        /// </summary>
        public bool IsEnablePie = true;
        /// <summary>
        /// 是否显示柱图
        /// </summary>
        public bool IsEnableColumn = true; 
        /// <summary>
        /// 是否显示折线图
        /// </summary>
        public bool IsEnableLine = true;
        /// <summary>
        /// 折线图显示类型.
        /// </summary>
        public LineChartShowType LineChartShowType = LineChartShowType.HengXiang;
        /// <summary>
        /// 默认参数.
        /// </summary>
        public string DefaultParas = "";
        /// <summary>
        /// y最大值.
        /// </summary>
        public double MaxValue = 0;
        /// <summary>
        /// y最小值.
        /// </summary>
        public double MinValue = 0;
        /// <summary>
        /// 最底部的信息制表等信息.
        /// </summary>
        public string ChartInfo = null;
        /// <summary>
        /// 字段关系表达式
        /// </summary>
        public string ColExp = "";
        /// <summary>
        /// 说明
        /// </summary>
        public string DESC = "";
        //---------------------------------------------------------------
       /// <summary>
        /// 设置图表背景的颜色
       /// </summary>
        public string canvasBgColor = "FF8E46";
        /// <summary>
        /// 设置图表基部的颜色
        /// </summary>
        public string canvasBaseColor = "008E8E";
        /// <summary>
        /// 设置图表基部的高度 
        /// </summary>
        public string canvasBaseDepth = "5";
        /// <summary>
        /// 设置图表背景的深度 
        /// </summary>
        public string canvasBgDepth = "5";
        /// <summary>
        /// 设置是否显示图表背景 
        /// </summary>
        public string showCanvasBg = "1";
        /// <summary>
        /// 设置是否显示图表基部
        /// </summary>
        public string showCanvasBase = "1";
        //---------------------------------------------------------------
        /// <summary>
        /// 数据源.
        /// </summary>
        public DataTable _DBDataTable = null;
        public DataTable DBDataTable
        {
            get
            {
                if (_DBDataTable == null)
                {
                    //获得数据表.
                    // 执行SQL.
                    string sql = this.DBSrc;
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    foreach (string k in System.Web.HttpContext.Current.Request.QueryString.Keys)
                        sql = sql.Replace("@" + k, System.Web.HttpContext.Current.Request.QueryString[k]);
                    if (sql.Contains("@") == true)
                    {
                        BP.DA.AtPara ap = new BP.DA.AtPara(this.DefaultParas);
                        foreach (string k in ap.HisHT.Keys)
                        {
                            sql = sql.Replace("@" + k, ap.HisHT[k].ToString());
                        }
                    }

                    _DBDataTable = BP.DA.DBAccess.RunSQLReturnTable(sql);
                }
                return _DBDataTable;
            }
            set
            {
                _DBDataTable = value;
            }
        }
    }
    public class Rpt2Attrs : System.Collections.CollectionBase
    {
        public void Add(Rpt2Attr en)
        {
            this.InnerList.Add(en);
        }
        public Rpt2Attr GetD2(int idx)
        {
            return (Rpt2Attr)this.InnerList[idx];
        }
    }
    /// <summary>
    /// 报表基类
    /// </summary>
    abstract public class Rpt2Base
    {
        #region 构造方法
        /// <summary>
        /// 报表基类
        /// </summary>
        public Rpt2Base()
        {
        }
        #endregion 构造方法

        #region 要求子类强制重写的属性.
        /// <summary>
        /// 显示的标题.
        /// </summary>
        abstract public string Title
        {
            get;
        }
        /// <summary>
        /// 默认选择的属性.
        /// </summary>
        abstract public int AttrDefSelected
        {
            get;
        }
        /// <summary>
        /// 分组显示属性, 多个属性用@符号隔开.
        /// </summary>
        abstract public Rpt2Attrs AttrsOfGroup
        {
            get;
        }
        #endregion 要求子类重写的属性.

        #region 提供给操作者的方法.
        #endregion 提供给操作者的方法
    }
}
