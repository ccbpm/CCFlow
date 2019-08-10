using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
//using System.Web.SessionState;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;
using System.IO;
using System.Text; 
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.Text.RegularExpressions;
using BP.Port;
 
namespace BP.En
{
	/// <summary>
	///  关于对Entity扩展，的方法。
	/// </summary>
    public class GloEntity
    {
        #region 用到ddl 的方法。
        public static string GetTextByValue(Entities ens, string no, string isNullAsVal)
        {
            try
            { 
                return GetTextByValue(ens, no);
            }
            catch
            {
                return isNullAsVal;
            }
        }
        public static string GetTextByValue(Entities ens, string no)
        {
            foreach (Entity en in ens)
            {
                if (en.GetValStringByKey("No") == no)
                    return en.GetValStringByKey("Name");
            }
            if (ens.Count == 0)
                throw new Exception("@实体集合里面没有数据.");
            else
                throw new Exception("@没有找到No=" + no + "在实体里面");
        }
        #endregion

        //public static string GetEnFilesUrl(Entity en)
        //{
        //    string str = null;
        //    SysFileManagers ens = null; // en.HisSysFileManagers;

        //    string path = BP.Sys.Glo.Request.ApplicationPath;
        //    foreach (SysFileManager file in ens)
        //    {
        //        str += "[<a href='" + path + file.MyFilePath + "' target='f" + file.OID + "' >" + file.MyFileName + "</a>]";
        //    }
        //    return str;
        //}

        #region 关于对entity 的处理

        #region 转换dataset
        /// <summary>
        /// 把指定的ens 转换为 dataset
        /// </summary>
        /// <param name="spen">指定的ens</param>
        /// <returns>返回关系dataset</returns>
        public static DataSet ToDataSet(Entities spens)
        {

            DataSet ds = new DataSet(spens.ToString());

            /* 把主表加入DataSet */
            Entity en = spens.GetNewEntity;
            DataTable dt = new DataTable();
            if (spens.Count == 0)
            {
                QueryObject qo = new QueryObject(spens);
                dt = qo.DoQueryToTable();
            }
            else
            {
                dt = spens.ToDataTableField();
            }
            dt.TableName = en.EnDesc; //设定主表的名称。
            dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);

            //dt.RowChanged+=new DataRowChangeEventHandler(dt_RowChanged);

            ds.Tables.Add(DealBoolTypeInDataTable(en, dt));


            foreach (EnDtl ed in en.EnMap.DtlsAll)
            {
                /* 循环主表的明细，编辑好关系并把他们放入 DataSet 里面。*/
                Entities edens = ed.Ens;
                Entity eden = edens.GetNewEntity;
                DataTable edtable = edens.RetrieveAllToTable();
                edtable.TableName = eden.EnDesc;
                ds.Tables.Add(DealBoolTypeInDataTable(eden, edtable));

                DataRelation r1 = new DataRelation(ed.Desc,
                    ds.Tables[dt.TableName].Columns[en.PK],
                    ds.Tables[edtable.TableName].Columns[ed.RefKey]);
                ds.Relations.Add(r1);


                //	int i = 0 ;

                foreach (EnDtl ed1 in eden.EnMap.DtlsAll)
                {
                    /* 主表的明细的明细。*/
                    Entities edlens1 = ed1.Ens;
                    Entity edlen1 = edlens1.GetNewEntity;

                    DataTable edlensTable1 = edlens1.RetrieveAllToTable();
                    edlensTable1.TableName = edlen1.EnDesc;
                    //edlensTable1.TableName =ed1.Desc ;


                    ds.Tables.Add(DealBoolTypeInDataTable(edlen1, edlensTable1));

                    DataRelation r2 = new DataRelation(ed1.Desc,
                        ds.Tables[edtable.TableName].Columns[eden.PK],
                        ds.Tables[edlensTable1.TableName].Columns[ed1.RefKey]);
                    ds.Relations.Add(r2);
                }

            }


            return ds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DataTable DealBoolTypeInDataTable(Entity en, DataTable dt)
        {

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyDataType == DataType.AppBoolean)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "tmp" + attr.Key;
                    col.DataType = typeof(bool);
                    dt.Columns.Add(col);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[attr.Key].ToString() == "1")
                            dr["tmp" + attr.Key] = true;
                        else
                            dr["tmp" + attr.Key] = false;
                    }
                    dt.Columns.Remove(attr.Key);
                    dt.Columns["tmp" + attr.Key].ColumnName = attr.Key;
                    continue;
                }
                if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "tmp" + attr.Key;
                    col.DataType = typeof(DateTime);
                    dt.Columns.Add(col);
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            dr["tmp" + attr.Key] = DateTime.Parse(dr[attr.Key].ToString());
                        }
                        catch
                        {
                            if (attr.DefaultVal.ToString() == "")
                                dr["tmp" + attr.Key] = DateTime.Now;
                            else
                                dr["tmp" + attr.Key] = DateTime.Parse(attr.DefaultVal.ToString());

                        }

                    }
                    dt.Columns.Remove(attr.Key);
                    dt.Columns["tmp" + attr.Key].ColumnName = attr.Key;
                    continue;
                }
            }
            return dt;
        }
        /// <summary>
        /// DataRowChangeEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            throw new Exception(sender.ToString() + "  rows change ." + e.Row.ToString());
        }

        #endregion

        /// <summary>
        /// 把属性信息,与vlaue 转换为Table
        /// </summary>
        /// <param name="en">要转换的entity</param>
        /// <param name="editStyle">编辑风格</param>
        /// <returns>datatable</returns>
        public static DataTable ToTable(Entity en, int editStyle)
        {
            if (editStyle == 0)
                return GloEntity.ToTable0(en);
            else
                return GloEntity.ToTable1(en);
        }
        /// <summary>
        /// 用户风格0
        /// </summary>
        /// <returns></returns>
        private static DataTable ToTable0(Entity en)
        {
            string nameOfEnterInfo = en.EnDesc;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("输入项目", typeof(string)));
            dt.Columns.Add(new DataColumn(nameOfEnterInfo, typeof(string)));
            dt.Columns.Add(new DataColumn("信息输入要求", typeof(string)));

            foreach (Attr attr in en.EnMap.Attrs)
            {
                DataRow dr = dt.NewRow();
                dr["输入项目"] = attr.Desc;
                dr[nameOfEnterInfo] = en.GetValByKey(attr.Key);
                dr["信息输入要求"] = attr.EnterDesc;
                dt.Rows.Add(dr);
            }
            // 如果实体需要附件。
            if (en.EnMap.AdjunctType != AdjunctType.None)
            {
                // 加入附件信息。
                DataRow dr1 = dt.NewRow();
                dr1["输入项目"] = "附件";
                dr1[nameOfEnterInfo] = "";
                dr1["信息输入要求"] = "编辑附件";
                dt.Rows.Add(dr1);
            }
            // 明细
            foreach (EnDtl dtl in en.EnMap.Dtls)
            {
                DataRow dr = dt.NewRow();
                dr["输入项目"] = dtl.Desc;
                dr[nameOfEnterInfo] = "EnsName_" + dtl.Ens.ToString() + "_RefKey_" + dtl.RefKey;
                dr["信息输入要求"] = "请进入编辑明细";
                dt.Rows.Add(dr);
            }
            foreach (AttrOfOneVSM attr in en.EnMap.AttrsOfOneVSM)
            {
                DataRow dr = dt.NewRow();
                dr["输入项目"] = attr.Desc;
                dr[nameOfEnterInfo] = "OneVSM" + attr.EnsOfMM.ToString();
                dr["信息输入要求"] = "请进入编辑多选";
                dt.Rows.Add(dr);
            }
            return dt;

        }
        /// <summary>
        /// 用户风格1
        /// </summary>
        /// <returns></returns>
        private static DataTable ToTable1(Entity en)
        {

            string col1 = "字段名1";
            string col2 = "内容1";
            string col3 = "字段名2";
            string col4 = "内容2";

            //string enterNote=null;
            //			if (this.EnMap.Dtls.Count==0 || this.EnMap.AttrsOfOneVSM.Count==0)
            //				enterNote="内容1";
            //			else
            //				enterNote="保存后才能编辑关联信息";


            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(col1, typeof(string)));
            dt.Columns.Add(new DataColumn(col2, typeof(string)));
            dt.Columns.Add(new DataColumn(col3, typeof(string)));
            dt.Columns.Add(new DataColumn(col4, typeof(string)));


            for (int i = 0; i < en.EnMap.HisPhysicsAttrs.Count; i++)
            {
                DataRow dr = dt.NewRow();
                Attr attr = en.EnMap.HisPhysicsAttrs[i];
                dr[col1] = attr.Desc;
                dr[col2] = en.GetValByKey(attr.Key);

                i++;
                if (i == en.EnMap.HisPhysicsAttrs.Count)
                {
                    dt.Rows.Add(dr);
                    break;
                }
                attr = en.EnMap.HisPhysicsAttrs[i];
                dr[col3] = attr.Desc;
                dr[col4] = en.GetValByKey(attr.Key);
                dt.Rows.Add(dr);
            }


            // 如果实体需要附件。
            if (en.EnMap.AdjunctType != AdjunctType.None)
            {
                // 加入附件信息。
                DataRow dr1 = dt.NewRow();
                dr1[col1] = "附件";
                dr1[col2] = "编辑附件";
                //dr["输入项目2"]="附件信息";

                dt.Rows.Add(dr1);
            }
            // 明细
            foreach (EnDtl dtl in en.EnMap.Dtls)
            {
                DataRow dr = dt.NewRow();
                dr[col1] = dtl.Desc;
                dr[col2] = "EnsName_" + dtl.Ens.ToString() + "_RefKey_" + dtl.RefKey;
                //dr["输入项目2"]="明细信息";
                dt.Rows.Add(dr);
            }
            // 多对多的关系
            foreach (AttrOfOneVSM attr in en.EnMap.AttrsOfOneVSM)
            {
                DataRow dr = dt.NewRow();
                dr[col1] = attr.Desc;
                dr[col2] = "OneVSM" + attr.EnsOfMM.ToString();
                //dr["输入项目2"]="多选";
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion

        #region 张
        /// <summary>
        /// 通过一个集合，一个key，一个分割符号，获得这个属性的子串。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listspt"></param>
        /// <returns></returns>
        public static string GetEnsString(Entities ens, string key, string listspt)
        {
            string str = "";
            foreach (Entity en in ens)
            {
                str += en.GetValByKey(key) + listspt;
            }
            return str;
        }
        /// <summary>
        /// 通过一个集合，一个分割符号，获得这个属性的子串。
        /// </summary>		
        /// <param name="listspt"></param>
        /// <returns></returns>
        public static string GetEnsString(Entities ens, string listspt)
        {
            return GetEnsString(ens, ens.GetNewEntity.PK, listspt);
        }
        /// <summary>
        /// 通过一个集合获得这个属性的子串。
        /// </summary>		
        /// <param name="listspt"></param>
        /// <returns></returns>
        public static string GetEnsString(Entities ens)
        {
            return GetEnsString(ens, ens.GetNewEntity.PK, ";");
        }
        #endregion
    }
}
