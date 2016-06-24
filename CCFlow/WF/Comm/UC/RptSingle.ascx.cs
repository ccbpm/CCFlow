using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime;
using System.Web.Caching;
using System.Web.SessionState;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime;
using System.Web;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime;
using System.Web.Configuration;
using System.Web.Routing;
using System.Web.UI.Adapters;


namespace CCFlow.WF.Comm.UC
{
    public partial class RptSingle : BP.Web.UC.UCBase
    {
        #region 属性.
        /// <summary>
        /// 类型
        /// </summary>
        [ParenthesizePropertyName(true)]
        [MergableProperty(false)]
        [Filterable(false)]
        [Themeable(false)]
        public virtual BP.En.DBAChartType ChartType { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [ParenthesizePropertyName(true)]
        [MergableProperty(false)]
        [Filterable(false)]
        [Themeable(false)]
        public virtual string Title { get; set; }

        /// <summary>
        /// Idx
        /// </summary>
        [ParenthesizePropertyName(true)]
        [MergableProperty(false)]
        [Filterable(false)]
        [Themeable(false)]
        public virtual string Idx { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        [ParenthesizePropertyName(true)]
        [MergableProperty(false)]
        [Filterable(false)]
        [Themeable(false)]
        public virtual string Rpt2Name { get; set; }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {


        }
    }
}