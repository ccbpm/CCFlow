﻿using System;
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
                sql = "SELECT NodeID, Name  FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY NODEID ";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";

                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    dt.Columns["NODEID"].ColumnName = "NodeID";
                    dt.Columns["NAME"].ColumnName = "Name";
                }

                ds.Tables.Add(dt);
            }

            #region 加入表单库目录.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT NO as No ,Name,ParentNo FROM Sys_FormTree ORDER BY  PARENTNO, IDX ";
            else
                sql = "SELECT No,Name,ParentNo FROM Sys_FormTree ORDER BY  PARENTNO, IDX ";

            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FormTree";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
            ds.Tables.Add(dt);

            //加入表单
            sql = "SELECT A.No, A.Name, A.FK_FormTree  FROM Sys_MapData A, Sys_FormTree B WHERE A.FK_FormTree=B.No";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["FK_FORMTREE"].ColumnName = "FK_FormTree";
            }
            #endregion 加入表单库目录.

            #region 数据源
            BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
            ens.RetrieveAll();
            ds.Tables.Add(ens.ToDataTableField("SFDBSrcs"));
            #endregion

            //加入系统表.
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
            try
            {
                if (this.context.Request.Files.Count == 0)
                    return "err@请上传导入的模板文件.";

                string fk_mapData = this.FK_MapData;
            
                //读取上传的XML 文件.
                DataSet ds = new DataSet();
                //ds.ReadXml(path);
                ds.ReadXml(this.context.Request.Files[0].InputStream);

                //执行装载.
                MapData.ImpMapData(fk_mapData, ds);

                if (this.FK_Node != 0)
                {
                    Node nd = new Node(this.FK_Node);
                    nd.RepareMap( nd.HisFlow);
                }

                return "执行成功.";
            }
            catch (Exception ex)
            {
                return "err@导入失败:" + ex.Message;
            }
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
            try
            {
                string fromMapData = this.FromMapData;
                bool isClear = this.IsClear;

                MapData md = new MapData(fromMapData);

                MapData.ImpMapData(this.FK_MapData, BP.Sys.CCFormAPI.GenerHisDataSet(md.No));

                //设置为只读模式.
                if (this.IsSetReadonly == true)
                    MapData.SetFrmIsReadonly(this.FK_MapData);

                // 如果是节点表单，就要执行一次修复，以免漏掉应该有的系统字段。
                if (this.FK_MapData.Contains("ND") == true)
                {
                    string fk_node = this.FK_MapData.Replace("ND", "");
                    Node nd = new Node(int.Parse(fk_node));
                    nd.RepareMap(nd.HisFlow);
                }
                return "执行成功.";
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }

        }

        #region 04.从外部数据源导入
        /// <summary>
        /// 选择一个数据源，进入步骤2
        /// </summary>
        /// <returns></returns>
        public string Imp_Src_Step2_Init()
        {
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));

            //获取所有的表/视图
            DataTable dtTables = src.GetTables();

            return BP.Tools.FormatToJson.ToJson(dtTables);
        }

        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <returns></returns>
        public string Imp_Src_Step2_GetColumns()
        {
            DataSet ds = new DataSet();

            //01.当前节点表单已经存在的列
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            ds.Tables.Add(attrs.ToDataTableField("MapAttrs"));

            //02.数据源表中的列
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));
            DataTable tableColumns = src.GetColumns(this.GetRequestVal("STable"));
            tableColumns.TableName = "TableColumns";
            ds.Tables.Add(tableColumns);

            return BP.Tools.Json.ToJson(ds);
        }

        public string Imp_Src_Step3_Init()
        {
            DataSet ds = new DataSet();

            string SColumns = this.GetRequestVal("SColumns");
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));
            DataTable tableColumns = src.GetColumns(this.GetRequestVal("STable"));

            //01.添加列
            DataTable dt = tableColumns.Clone();
            foreach (DataRow dr in tableColumns.Rows)
            {
                if (SColumns.Contains(dr["no"].ToString()))
                    dt.ImportRow(dr);
            }
            dt.TableName = "Columns";
            ds.Tables.Add(dt);

            //02.添加枚举
            SysEnums ens = new SysEnums(MapAttrAttr.MyDataType);
            ds.Tables.Add(ens.ToDataTableField("EnumsDataType"));
            ens = new SysEnums(MapAttrAttr.LGType);
            ds.Tables.Add(ens.ToDataTableField("EnumsLGType"));

            return BP.Tools.FormatToJson.ToJson(ds);

        }

        public string Imp_Src_Step3_Save()
        {

            string hidImpFields = this.GetRequestVal("hidImpFields");
            string[] fields = hidImpFields.TrimEnd(',').Split(',');

            MapData md = new MapData();
            md.No = this.FK_MapData;
            md.RetrieveFromDBSources();


            string msg = "导入字段信息:"; 
            bool isLeft = true;
            float maxEnd = md.MaxEnd; //底部.
            for (int i = 0; i < fields.Length; i++)
            {
                string colname = fields[i];

                MapAttr ma = new MapAttr();
                ma.KeyOfEn = colname;
                ma.Name = this.GetRequestVal("TB_Desc_" + colname);
                ma.FK_MapData = this.FK_MapData;
                ma.MyDataType = int.Parse(this.GetRequestVal("DDL_DBType_" + colname));
                ma.MaxLen = int.Parse(this.GetRequestVal("TB_Len_" + colname));
                ma.UIBindKey = this.GetRequestVal("TB_BindKey_" + colname);
                ma.MyPK = this.FK_MapData + "_" + ma.KeyOfEn;
                ma.LGType = BP.En.FieldTypeS.Normal;

                if (ma.UIBindKey != "")
                {
                    SysEnums se = new SysEnums();
                    se.Retrieve(SysEnumAttr.EnumKey, ma.UIBindKey);
                    if (se.Count > 0)
                    {
                        ma.MyDataType = BP.DA.DataType.AppInt;
                        ma.LGType = BP.En.FieldTypeS.Enum;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }

                    SFTable tb = new SFTable();
                    tb.No = ma.UIBindKey;
                    if (tb.IsExits == true)
                    {
                        ma.MyDataType = BP.DA.DataType.AppString;
                        ma.LGType = BP.En.FieldTypeS.FK;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }
                }

                if (ma.MyDataType == BP.DA.DataType.AppBoolean)
                    ma.UIContralType = BP.En.UIContralType.CheckBok;
                if (ma.IsExits)
                    continue;
                ma.Insert();

                msg += "\t\n字段:" + ma.KeyOfEn + "" + ma.Name + "加入成功.";
                FrmLab lab = null;
                if (isLeft == true)
                {
                    maxEnd = maxEnd + 40;
                    /* 是否是左边 */
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 40;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                else
                {
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 350;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                isLeft = !isLeft;
            }
            
            //重新设置.
            md.ResetMaxMinXY();

            return msg;

        } 
        #endregion

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
