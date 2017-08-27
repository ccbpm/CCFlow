using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Rpt : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Rpt(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        /// <summary>
        /// 我的流程查询.
        /// </summary>
        /// <returns></returns>
        public string MyStartFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "MyRpt";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);


            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);
            QueryObject qo = new QueryObject(ges);
            DataTable dt = qo.DoQueryToTable();
            dt.TableName = "dt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

    }
}
