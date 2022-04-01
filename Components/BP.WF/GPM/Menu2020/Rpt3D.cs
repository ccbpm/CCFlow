using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.Sys;

namespace BP.GPM.Menu2020
{
    /// <summary>
    /// 三维报表
    /// </summary>
    public class Rpt3D : EntityNoName
    {
        #region 属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = false;
                    return uac;
                }
                else
                {
                    uac.Readonly();
                }
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 三维报表
        /// </summary>
        public Rpt3D()
        {
        }
        protected override bool beforeDelete()
        {
            return base.beforeDelete();
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

                Map map = new Map("GPM_Menu", "三维报表");  // 类的基本属性.
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(MenuAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 50, 50);
                map.AddTBString(MenuAttr.Name, null, "菜单名称", true, false, 0, 200, 200);

                map.AddTBString(MenuAttr.Title, null, "报表标题", true, false, 0, 200, 200,true);
                map.AddTBString(MenuAttr.Tag4, null, "分析项目名称", true, false, 0, 200, 200);


                map.AddDDLSysEnum(MenuAttr.ListModel, 0, "维度显示格式", true, true, "RptModel", "@0=左边@1=顶部");

                map.AddDDLSysEnum(MenuAttr.TagInt1, 0, "合计位置?", true, true, "Rpt3SumModel", "@0=不显示@1=底部@2=头部");

                map.AddTBStringDoc(MenuAttr.Tag0, null, "数据源SQL", true, false, true);
                string msg = "编写说明";
                msg += "\t\n 1. 该数据源一般是一个分组统计语句, 比如： SELECT D1,D2,D3,SUM(XX) AS Num FROM MyTable WHERE 1=2 GROUP BY D1,D2,D3  ";
                msg += "\t\n 2. 对应的数据列分别是 如下数据源的列数据，列的顺序不要改变。 ";
                msg += "\t\n 3. 每个维度都是返回的No,Name两个列的数据。 ";
                map.SetHelperAlert(MenuAttr.Tag0, msg);

                map.AddTBStringDoc(MenuAttr.Tag1, null, "维度1SQL", true, false, true);
                map.AddTBStringDoc(MenuAttr.Tag2, null, "维度2SQL", true, false, true);
                map.AddTBStringDoc(MenuAttr.Tag3, null, "维度3SQL", true, false, true);


                //从表明细.
                map.AddDtl(new SearchAttrs(), SearchAttrAttr.RefMenuNo);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// 初始化数据.
        /// </summary>
        /// <returns></returns>
        public string Rpt3D_Init()
        {
            DataSet ds = new DataSet();
            //维度1
            DataTable src = DBAccess.RunSQLReturnTable(BP.WF.Glo.DealExp(this.GetValStrByKey(MenuAttr.Tag0), null, null));
            src.TableName = "Src";
            ds.Tables.Add(src);

            //维度1
            DataTable dt1 = DBAccess.RunSQLReturnTable(BP.WF.Glo.DealExp(this.GetValStrByKey(MenuAttr.Tag1), null, null));
            dt1.TableName = "D1";
            ds.Tables.Add(dt1);

            DataTable dt2 = DBAccess.RunSQLReturnTable(BP.WF.Glo.DealExp(this.GetValStrByKey(MenuAttr.Tag2), null, null));
            dt2.TableName = "D2";
            ds.Tables.Add(dt2);

            DataTable D3 = DBAccess.RunSQLReturnTable(BP.WF.Glo.DealExp(this.GetValStrByKey(MenuAttr.Tag3), null, null));
            D3.TableName = "D3";
            ds.Tables.Add(D3);

            return BP.Tools.Json.ToJson(ds);
        }

        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 三维报表s
    /// </summary>
    public class Rpt3Ds : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 三维报表s
        /// </summary>
        public Rpt3Ds()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Rpt3D();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Rpt3D> ToJavaList()
        {
            return (System.Collections.Generic.IList<Rpt3D>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Rpt3D> Tolist()
        {
            System.Collections.Generic.List<Rpt3D> list = new System.Collections.Generic.List<Rpt3D>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Rpt3D)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
