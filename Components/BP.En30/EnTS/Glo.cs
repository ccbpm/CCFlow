using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.EnTS
{
    public class Glo
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="classID"></param>
        public static bool IsExitMap(string classID)
        {
            return BP.DA.Cash.IsExitMapTS(classID);
        }
        public static void SetMap(string classID, string mapData)
        {
            if (mapData == null)
                throw new Exception("err@classID=["+ classID + "]的mapData不能为空.");
            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(mapData);

            BP.En.Map myMap = new En.Map();

            #region 主表信息.
            myMap.PhysicsTable = json.GetValue("PhysicsTable").ToString();
            myMap.EnDesc = json.GetValue("EnDesc").ToString();
            myMap.CodeStruct = json.GetValue("CodeStruct").ToString();
            myMap.ParaFields = json.GetValue("ParaFields").ToString();
            if (DataType.IsNullOrEmpty(myMap.ParaFields) == true)
                myMap.ParaFields = null; //参数字段.
            #endregion 主表信息.

            #region 字段集合
            string attrs = json.GetValue("attrs").ToString();
            System.Data.DataTable dtAttrs = BP.Tools.Json.ToDataTable(attrs);
            foreach (System.Data.DataRow dr in dtAttrs.Rows)
            {
                int myDataType = int.Parse(dr["MyDataType"].ToString());
                FieldType myFieldType = (BP.En.FieldType)int.Parse(dr["MyFieldType"].ToString());

                Attr attr = new Attr();
                attr.Key = dr["Key"].ToString();
                attr.Desc = dr["Desc"].ToString();
                attr.Field = dr["Field"].ToString();
                attr.HelperUrl = dr["HelperUrl"].ToString();

                attr.UIBindKey = dr["UIBindKey"].ToString();
                attr.UIRefKeyText = dr["UIRefKeyText"].ToString();
                attr.UIRefKeyValue = dr["UIRefKeyValue"].ToString();

                attr.UITag = dr["UITag"].ToString(); //枚举字段.

                attr.IsSupperText = int.Parse(dr["IsSupperText"].ToString());
                //.最小长度
                attr.MinLength = int.Parse(dr["MinLength"].ToString());
                attr.MaxLength = int.Parse(dr["MaxLength"].ToString()); //.最大长度
                attr.UIWidth = int.Parse(dr["UIWidth"].ToString()); //.宽度.

                attr.DefaultVal = dr["_defaultVal"].ToString(); //.默认值.

                if (dr["UIIsLine"].ToString().Equals("false"))
                    attr.UIIsLine = false;
                else
                    attr.UIIsLine = true;

                attr.HelperUrl = dr["HelperUrl"].ToString();
                attr.MyDataType = myDataType; //类型.
                attr.MyFieldType = myFieldType;
                myMap.AddAttr(attr);
            }
            #endregion 字段集合

            #region 查询条件.

            //日期
            myMap.DTSearchKey = json.GetValue("DTSearchKey").ToString();
            myMap.DTSearchLabel = json.GetValue("DTSearchLabel").ToString();
            myMap.DTSearchWay = (DTSearchWay)int.Parse(json.GetValue("DTSearchWay").ToString());

            //数值类型的范围.
            myMap.SearchFieldsOfNum = json.GetValue("SearchFieldsOfNum").ToString();

            //查询条件 - 枚举外键的集合.
            string ass = json.GetValue("searchFKEnums").ToString();
            System.Data.DataTable dtAss = BP.Tools.Json.ToDataTable(ass);
            foreach (System.Data.DataRow dr in dtAss.Rows)
            {
                myMap.AddSearchAttr(dr["AttrKey"].ToString());
            }

            //查询条件 - 枚举外键的集合.
            ass = json.GetValue("searchNormals").ToString();
            System.Data.DataTable dtNor = BP.Tools.Json.ToDataTable(ass);
            foreach (System.Data.DataRow dr in dtNor.Rows)
            {
                SearchNormal sn = new SearchNormal();
                sn.Key = dr["Key"].ToString(); //  key || '';
                sn.Lab = dr["Lab"].ToString(); //  lab || '';
                sn.RefAttrKey = dr["RefAttrKey"].ToString(); //  refAttr || '';
                sn.DefaultSymbol = dr["DefaultSymbol"].ToString(); //  DefaultSymbol || '';
                sn.DefaultVal = dr["DefaultVal"].ToString(); // defaultValue || '';
                sn.TBWidth = int.Parse(dr["TBWidth"].ToString()); // tbwidth || 120;
                sn.IsHidden = Boolean.Parse(dr["IsHidden"].ToString()); // !!isHidden;
                myMap.SearchNormals.Add(sn);
            }
            #endregion 查询条件.

            BP.DA.Cash.SetMapTS(classID, myMap);

            //移除该en的缓存
            BP.DA.Cash.ClearSQL(classID);

        }
        /// <summary>
        /// 获得map的方法
        /// </summary>
        /// <param name="tsClassID"></param>
        /// <returns></returns>
        public static BP.En.Map GenerMap(string tsClassID)
        {
            if (tsClassID == null)
                throw new Exception("err@错误：tsClassID 不能为null. ");

            string caseTsClassID = tsClassID;

            var map = BP.DA.Cash.GetMapTS(caseTsClassID);
            if (map != null)
                return map;

            throw new Exception("err@没有找到 " + tsClassID + " 的map。");
        }

    }
}
