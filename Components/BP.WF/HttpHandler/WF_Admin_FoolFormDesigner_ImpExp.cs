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

namespace BP.WF.HttpHandler
{
    public class WF_Admin_FoolFormDesigner_ImpExp : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_FoolFormDesigner_ImpExp(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 导入
        /// <summary>
        /// 初始化 导入的界面 .
        /// </summary>
        /// <returns></returns>
        public string Imp_Init()
        {
            DataSet ds = new DataSet();

            string sql = "";
            System.Data.DataTable dt;

            if (this.FK_Flow != null)
            {
                //加入节点表单. 如果没有流程参数.
                sql = "SELECT NODEID, NAME  FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY NODEID ";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);
            }

            //加入表单库目录.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            sql = "SELECT 'ND'||NO as No ,NAME,PARENTNO FROM Sys_FormTree ORDER BY  PARENTNO, IDX ";
            else
                sql = "SELECT 'ND'+NO,NAME,PARENTNO FROM Sys_FormTree ORDER BY  PARENTNO, IDX ";

            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FormTree";
            ds.Tables.Add(dt);

            //加入表单
            sql = "SELECT A.No, A.Name, A.FK_FormTree  FROM Sys_MapData A, Sys_FormTree B WHERE A.FK_FormTree=B.No";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 从本机装载表单模版
        /// </summary>
        /// <param name="fileByte">文件流</param>
        /// <param name="fk_mapData">表单模版ID</param>
        /// <param name="isClear">是否清空？</param>
        /// <returns>执行结果</returns>
        public string Imp_LoadFrmTempleteFromLocalFile()
        {
            if (this.context.Request.Files.Count == 0) {
                return "请上传导入的模板文件.";
            }

            string fk_mapData = this.FK_MapData;
            //读取上传的XML 文件.
            DataSet ds = new DataSet();
            //ds.ReadXml(path);
            ds.ReadXml(this.context.Request.Files[0].InputStream);
            //执行装载.
            MapData.ImpMapData(fk_mapData, ds);

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
        public string Imp_CopyFrm()
        {
            string fromMapData = this.FromMapData;
            string fk_mapdata = this.FK_MapData;
            bool isClear = this.IsClear;
            bool isSetReadonly = this.IsSetReadonly;

            MapData md = new MapData(fromMapData);

            MapData.ImpMapData(fk_mapdata, BP.Sys.CCFormAPI.GenerHisDataSet(md.No));

            // 如果是节点表单，就要执行一次修复，以免漏掉应该有的系统字段。
            if (fk_mapdata.Contains("ND") == true)
            {
                string fk_node = fk_mapdata.Replace("ND", "");
                Node nd = new Node(int.Parse(fk_node));
                nd.RepareMap();
            }
            return "执行成功.";
        }

        #endregion


        public string FK_MapData
        {
            get
            {
                return context.Request.QueryString["FK_MapData"];
            }
        }
        public bool IsClear
        {
            get
            {
                string isClearStr = context.Request.QueryString["IsClear"];
                bool isClear = false;
                if (!string.IsNullOrEmpty(isClearStr) && isClearStr.ToString().ToLower() == "on")
                {
                    isClear = true;
                }
                return isClear;
            }
        }

        public bool IsSetReadonly
        {
            get
            {
                string isSetReadonlyStr = context.Request.QueryString["IsSetReadonly"];
                bool isSetReadonly = false;
                if (!string.IsNullOrEmpty(isSetReadonlyStr) && isSetReadonlyStr.ToString().ToLower() == "on")
                {
                    isSetReadonly = true;
                }
                return isSetReadonly;
            }
        }

        public string FromMapData
        {
            get
            {
                string fromMapData = context.Request.QueryString["FromMapData"];
                return fromMapData;
            }
        }
    }
}
