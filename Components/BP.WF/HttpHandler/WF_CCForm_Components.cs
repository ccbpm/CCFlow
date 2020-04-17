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
    public class WF_CCFormr_Components : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCFormr_Components()
        {

        }

        #region  公文文号 .
        /// <summary>
        /// 初始化字号编辑器
        /// </summary>
        /// <returns>当前的字号信息.</returns>
        public string DocWord_Init()
        {
            //创建实体.
            GEEntity en = new GEEntity(this.FrmID, this.OID);

            //查询字段.
            string ptable = en.EnMap.PhysicsTable; //获得存储表.

            //必须有4个列，分别是 DocWordKey字的外键,DocWordName字的名称,DocWordYear年度,DocWordLSH流水号,DocWord字号
            string sql = "SELECT DocWordKey,DocWordName,DocWordYear,DocWordLSH,DocWord FROM " + ptable + " WHERE OID=" + this.OID;
            DataTable dt = new DataTable();
            try
            {
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                string repairSQL = ""; //修复表结构的sql.

                //如果没有此列就检查增加此列.
                if (DBAccess.IsExitsTableCol(ptable, "DocWordKey") == false)
                    repairSQL += "@ALTER TABLE " + ptable + " ADD DocWordKey varchar(100) ";
                if (DBAccess.IsExitsTableCol(ptable, "DocWordName") == false)
                    repairSQL += "@ALTER TABLE " + ptable + " ADD  DocWordName varchar(100) ";
                if (DBAccess.IsExitsTableCol(ptable, "DocWordYear") == false)
                    repairSQL += "@ALTER TABLE " + ptable + " ADD DocWordYear nvarchar(100) ";
                if (DBAccess.IsExitsTableCol(ptable, "DocWordLSH") == false)
                    repairSQL += "@ALTER TABLE " + ptable + " ADD DocWordLSH nvarchar(100) ";
                if (DBAccess.IsExitsTableCol(ptable, "DocWord") == false)
                    repairSQL += "@ALTER TABLE " + ptable + " ADD DocWord nvarchar(100) ";

                if (DataType.IsNullOrEmpty(repairSQL) == false)
                    DBAccess.RunSQLs(repairSQL);

                dt = DBAccess.RunSQLReturnTable(sql);
            }

            //处理大小写.
            if (SystemConfig.AppCenterDBType== DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "DocWordKey";
                dt.Columns[1].ColumnName = "DocWordName";
                dt.Columns[2].ColumnName = "DocWordYear";
                dt.Columns[3].ColumnName = "DocWordLSH";
                dt.Columns[4].ColumnName = "DocWord";
            }

            // 判断流水号是否未空.
            string key = dt.Rows[0]["DocWordKey"].ToString();
            string name = dt.Rows[0]["DocWordName"].ToString();
            string year = dt.Rows[0]["DocWordYear"].ToString();
            string lsh = dt.Rows[0]["DocWordLSH"].ToString();
            string word = dt.Rows[0]["DocWord"].ToString();

            //如果year是空的就去当前年度.
            if (DataType.IsNullOrEmpty(year) == true)
                year = DataType.CurrentYear;

            //流水号为空，就取当前年度的最大流水号.
            if (DataType.IsNullOrEmpty(lsh) == true)
            {
                //生成一个新的流水号.
                sql = "SELECT MAX(DocWordLSH) AS No FROM "+ ptable + " WHERE DocWordKey='"+key+ "' AND DocWordYear='"+year+"' AND OID!="+this.OID;
                lsh = DBAccess.RunSQLReturnStringIsNull(sql,"");
                if (DataType.IsNullOrEmpty(lsh) == true)
                    lsh = "001";

                dt.Rows[0]["DocWordYear"] = year;
                dt.Rows[0]["DocWordLSH"] = lsh;
            }

            //初始化数据.
            sql = "UPDATE "+ptable+ " SET DocWordLSH='" + lsh + "', DocWordYear='"+year+"' WHERE OID="+this.OID;
            DBAccess.RunSQL(sql);

            //转成Json，返回出去.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 重新生成字号
        /// </summary>
        /// <returns></returns>
        public string DocWord_ReGenerDocWord()
        {
            //创建实体.
            GEEntity en = new GEEntity(this.FrmID, this.OID);

            //查询字段.
            string ptable = en.EnMap.PhysicsTable; //获得存储表.

            string word = this.GetRequestVal("DDL_Word"); //字号
            string ny = this.GetRequestVal("DDL_Year"); //年月. 

            //生成一个新的流水号.
           string sql = "SELECT MAX(DocWordLSH) AS No FROM " + ptable + " WHERE DocWordKey='" + word + "' AND DocWordYear='" + ny + "' AND OID!=" + this.OID;
           string lsh = DBAccess.RunSQLReturnStringIsNull(sql, "");
            if (DataType.IsNullOrEmpty(lsh) == true)
                lsh = "0001";
            string str = (int.Parse(lsh) + 1).ToString("0000");//将返回的数字加+1并格式化为0000;
            return str;
        }
  .
        /// <summary>
        /// 重新生成字号
        /// </summary>
        /// <returns></returns>
        public string DocWord_Save()
        {
            //创建实体.
            GEEntity en = new GEEntity(this.FrmID, this.OID);

            //查询字段.
            string ptable = en.EnMap.PhysicsTable; //获得存储表.

            string word = this.GetRequestVal("DDL_Word"); //字号
            string ny = this.GetRequestVal("DDL_Year"); //年份. 
            string lsh = this.GetRequestVal("TB_LSH"); //年份. 

            //生成一个新的流水号.
            string sql = "update FROM " + ptable + " set DocWordKey='" + word + "' ,DocWordYear='" + ny + "',DocWordLSH='"+lsh+"' WHERE OID=" + this.OID;
            DBAccess.RunSQL(sql);           
           
            return "保存成功";
        }
         #endregion 公文文号.

    }
}
