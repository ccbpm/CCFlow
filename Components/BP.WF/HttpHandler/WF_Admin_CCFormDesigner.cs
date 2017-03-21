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
        /// 从本机装载表单模版
        /// </summary>
        /// <param name="fileByte">文件流</param>
        /// <param name="fk_mapData">表单模版ID</param>
        /// <param name="isClear">是否清空？</param>
        /// <returns>执行结果</returns>
        public string Imp_LoadFrmTempleteFromLocalFiel(byte[] fileByte, string fk_mapData, bool isClear)
        {
            //文件路径.
            string file = "\\Temp\\" + fk_mapData + ".xml";
            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file;

            //保存到服务器，指定的位置.
            BP.DA.DataType.WriteFile(path, fileByte);

            //读取文件.
            DataSet ds = new DataSet();
            ds.ReadXml(file);

            //执行装载.
            MapData.ImpMapData(fk_mapData, ds, true);

            if (fk_mapData.Contains("ND"))
            {
                /* 判断是否是节点表单 */
                int nodeID = 0;
                try
                {
                    nodeID = int.Parse(fk_mapData.Replace("ND", ""));
                }
                catch
                {
                    return "执行成功.";
                }

                Node nd = new Node(nodeID);
                nd.RepareMap();
            }
            return "执行成功.";
        }
        /// <summary>
        /// 从节点上Copy
        /// </summary>
        /// <param name="fromMapData">从表单ID</param>
        /// <param name="fk_mapdata">到表单ID</param>
        /// <param name="isClear">是否清楚现有的元素？</param>
        /// <param name="isSetReadonly">是否设置为只读？</param>
        /// <returns>执行结果</returns>
        public string Imp_CopyFrm(string fromMapData, string fk_mapdata, bool isClear, bool isSetReadonly)
        {
            MapData md = new MapData(fromMapData);

            MapData.ImpMapData(fk_mapdata, BP.Sys.CCFormAPI.GenerHisDataSet(md.No), isSetReadonly);

            // 如果是节点表单，就要执行一次修复，以免漏掉应该有的系统字段。
            if (fk_mapdata.Contains("ND") == true)
            {
                string fk_node = fk_mapdata.Replace("ND", "");
                Node nd = new Node(int.Parse(fk_node));
                nd.RepareMap();
            }
            return "执行成功.";
        }


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
