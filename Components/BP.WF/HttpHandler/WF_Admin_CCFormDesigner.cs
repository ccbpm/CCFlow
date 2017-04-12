using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner : BP.WF.HttpHandler.WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public string SaveForm()
        {
            BP.Sys.CCFormAPI.SaveFrm(this.FK_MapData, this.GetRequestVal("diagram"));
            return "保存成功.";
        }

        #region tables
        public string Tables_Init()
        {
            BP.Sys.SFTables tabs = new BP.Sys.SFTables();
            tabs.RetrieveAll();
            DataTable dt = tabs.ToDataTableField();
            dt.Columns.Add("RefNum", typeof(int));

            foreach (DataRow dr in dt.Rows)
            {
                //求引用数量.
                int refNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(KeyOfEn) FROM Sys_MapAttr WHERE UIBindKey='" + dr["No"] + "'", 0);
                dr["RefNum"] = refNum;
            }
            return BP.Tools.Json.ToJson(dt);
        }
        public string Tables_Delete()
        {
            try
            {
                BP.Sys.SFTable tab = new BP.Sys.SFTable();
                tab.No = this.No;
                tab.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string TableRef_Init()
        {
            BP.Sys.MapAttrs mapAttrs = new BP.Sys.MapAttrs();
            mapAttrs.RetrieveByAttr(BP.Sys.MapAttrAttr.UIBindKey, this.FK_SFTable);

            DataTable dt = mapAttrs.ToDataTableField();
            return BP.Tools.Json.ToJson(dt);
          }
        #endregion

        #region 方法 Home
        public string Home_Init()
        {
            string no = this.GetRequestVal("No");

            MapData md = new MapData(no);

            // 基础信息.
            Hashtable ht = new Hashtable();
            ht.Add("No", no);
            ht.Add("Name", md.Name);
            ht.Add("PTable", md.PTable);
            ht.Add("FrmTypeT", md.HisFrmTypeText);
            ht.Add("FrmTreeName", md.FK_FormTreeText);

            //统计信息.
            ht.Add("SumDataNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + md.PTable)); //数据量.
            ht.Add("SumAttrNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "'")); //字段数量.
            ht.Add("SumAttrFK", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "' AND LGType=2 ")); //外键.
            ht.Add("SumAttrEnum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='" + no + "' AND LGType=1 ")); //外键.

            ht.Add("MapFrmFrees", "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapFrmFrees&PK=" + no); //自由表单属性.
            ht.Add("MapFrmFools", "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapFrmFools&PK=" + no); //傻瓜表单属性.
            ht.Add("MapFrmExcels", "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapFrmExcels&PK=" + no); //Excel表单属性.
            ht.Add("MapDataURLs", "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataURLs&PK=" + no);  //嵌入式表单属性.

            return BP.DA.DataType.ToJsonEntityModel(ht);
        }
        #endregion 方法 Home

        #region 字段列表 的操作
        /// <summary>
        /// 初始化字段列表.
        /// </summary>
        /// <returns></returns>
        public string FiledsList_Init()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData);
            foreach (MapAttr item in attrs)
            {
                if (item.LGType == FieldTypeS.Enum)
                {
                    SysEnumMain se = new SysEnumMain(item.UIBindKey);
                    item.UIRefKey = se.CfgVal;
                    continue;
                }

                if (item.LGType == FieldTypeS.FK)
                {
                    item.UIRefKey = item.UIBindKey;
                    continue;
                }

                item.UIRefKey = "无";
            }
            return attrs.ToJson();
        }

        /// <summary>
        /// 删除字段
        /// </summary>
        /// <returns></returns>
        public string FiledsList_Delete()
        {
            MapAttr attr = new MapAttr(this.MyPK);
            if (attr.Delete() == 1)
            {
                return "删除成功！";
            }

            throw new Exception("删除失败！");
        }
        #endregion 字段列表 的操作
    }
}
