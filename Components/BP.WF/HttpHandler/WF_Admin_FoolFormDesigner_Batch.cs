using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.CCBill;
using System.IO;
using System.Text;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    public class WF_Admin_FoolFormDesigner_Batch : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 批量修改
        /// </summary>
        public WF_Admin_FoolFormDesigner_Batch()
        {
        }
        /// <summary>
        /// 批量修改字段Init.
        /// </summary>
        /// <returns></returns>
        public string KeyOfEn_Init()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve("FK_MapData", this.FrmID, "Idx");

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name")); //字段名
            dt.Columns.Add(new DataColumn("DBType")); //数据类型
            dt.Columns.Add(new DataColumn("GroupID")); //隶属分组.
            dt.Columns.Add(new DataColumn("KeyOfEn")); //字段ID
            dt.Columns.Add(new DataColumn("JianPin")); //简拼
            dt.Columns.Add(new DataColumn("QuanPin")); //全拼.
            dt.Columns.Add(new DataColumn("Etc")); //其他.

            foreach (MapAttr attr in attrs)
            {
                //生成字段名称.
                DataRow dr = dt.NewRow();
                dr["Name"] = attr.Name;
                dr["KeyOfEn"] = attr.KeyOfEn;
                dr["JianPin"] = BP.Sys.CCFormAPI.ParseStringToPinyinField(attr.Name, false);//简拼;
                dr["QuanPin"] = BP.Sys.CCFormAPI.ParseStringToPinyinField(attr.Name, true);//全拼;
                dr["GroupID"] = attr.GroupID;

                switch (attr.MyDataType)
                {
                    case BP.DA.DataType.AppString:
                        dr["DBType"] = "String";
                        break;
                    case BP.DA.DataType.AppBoolean:
                        dr["DBType"] = "AppBoolean";
                        break;
                    case BP.DA.DataType.AppFloat:
                        dr["DBType"] = "AppFloat";
                        break;
                    case BP.DA.DataType.AppInt:
                        dr["DBType"] = "Int";
                        break;
                    case BP.DA.DataType.AppMoney:
                        dr["DBType"] = "Money";
                        break;
                    case BP.DA.DataType.AppDate:
                        dr["DBType"] = "Date";
                        break;
                    case BP.DA.DataType.AppDateTime:
                        dr["DBType"] = "DateTime";
                        break;
                    default:
                        break;
                }
                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }
        public string KeyOfEn_Save()
        {
            string newName = this.KeyOfEn;
            BP.Sys.FrmUI.MapAttrString en = new Sys.FrmUI.MapAttrString(this.MyPK);
            return en.DoRenameField(newName);
        }
    }
}
