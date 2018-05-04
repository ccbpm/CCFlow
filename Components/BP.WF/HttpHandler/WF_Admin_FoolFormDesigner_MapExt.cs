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
    /// 初始化函数
    /// </summary>
    public class WF_Admin_FoolFormDesigner_MapExt : DirectoryPageBase
    {
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
            throw new Exception("@标记[" + this.DoType + "]，没有找到.");
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_FoolFormDesigner_MapExt(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        #endregion 执行父类的重写方法.

        #region AutoFullDtlField 自动计算 a*b  功能界面 .
        /// <summary>
        /// 保存(自动计算: @单价*@数量 模式.)
        /// </summary>
        /// <returns></returns>
        public string AutoFullDtlField_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.Doc = this.GetValFromFrmByKey("DDL_Dtl") + "." + this.GetValFromFrmByKey("DDL_Field") + "." + this.GetValFromFrmByKey("DDL_JSFS"); //要执行的表达式.

            me.ExtType = MapExtXmlList.AutoFullDtlField;

            //执行保存.
            me.MyPK = MapExtXmlList.AutoFullDtlField + "_" + me.FK_MapData + "_" + me.AttrOfOper;
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string AutoFullDtlField_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string AutoFullDtlField_Init()
        {
            DataSet ds = new DataSet();


         



            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            //把从表放入里面.
            MapDtls dtls = new MapDtls(this.FK_MapData);
            ds.Tables.Add(dtls.ToDataTableField("Dtls"));

            //把从表的字段放入.
            foreach (MapDtl dtl in dtls)
            {
                string sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='"+dtl.No+"' AND (MyDataType=2 OR MyDataType=3 OR MyDataType=5 OR MyDataType=8)  ";  
                sql+=" AND KeyOfEn !='OID' AND KeyOfEn!='FID' AND KeyOfEn!='RefPK' ";

                //把从表增加里面去.
                DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                mydt.TableName = dtl.No;
                ds.Tables.Add(mydt);
            }

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion AutoFullDtlField  功能界面.

        #region AutoFull 自动计算 a*b  功能界面 .
        /// <summary>
        /// 保存(自动计算: @单价*@数量 模式.)
        /// </summary>
        /// <returns></returns>
        public string AutoFull_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFull,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的表达式.

            me.ExtType = MapExtXmlList.AutoFull;

            //执行保存.
            me.MyPK = MapExtXmlList.AutoFull + "_" + me.FK_MapData + "_" + me.AttrOfOper ;
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string AutoFull_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.AutoFull,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string AutoFull_Init()
        {
            DataSet ds = new DataSet();

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFull,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion ActiveDDL 功能界面.

        #region TBFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string TBFullCtrl_Save()
        {
            try
            {
                MapExt me = new MapExt();
                int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                    MapExtAttr.FK_MapData, this.FK_MapData,
                    MapExtAttr.AttrOfOper, this.KeyOfEn);

                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = this.GetValFromFrmByKey("FK_DBSrc");
                me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

                me.ExtType = MapExtXmlList.TBFullCtrl;

                //执行保存.
                me.InitPK();

                if (me.Update() == 0)
                    me.Insert();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string TBFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string TBFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            //这个属性没有用.
            me.W = i;  //用于标记该数据是否保存?  从而不现实填充从表，填充下拉框.按钮是否可以用.
            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 填充从表
        /// </summary>
        /// <returns></returns>
        public string TBFullCtrlDtl_Init()
        {
            MapExt me = new MapExt(this.MyPK);

            string[] strs = me.Tag1.Split('$');
            // 格式为: $ND101Dtl2:SQL.

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, me.FK_MapData);
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) || str.Contains(":") == false)
                    continue;

                string[] kvs = str.Split(':');
                string fk_mapdtl = kvs[0];
                string sql = kvs[1];

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.No != fk_mapdtl)
                        continue;
                    dtl.MTR = sql.Trim();
                }
            }

            foreach (MapDtl dtl in dtls)
            {
                string cols = "";
                MapAttrs attrs = new MapAttrs(dtl.No);
                foreach (MapAttr item in attrs)
                {
                    if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal" || item.KeyOfEn == "RefPK")
                        continue;

                    cols += item.KeyOfEn + ",";
                }
                dtl.Alias = cols; //把ptable作为一个数据参数.
            }
            return dtls.ToJson();
        }

        public string TBFullCtrlDtl_Save()
        {
            MapDtls dtls = new MapDtls(this.FK_MapData);
            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapDtl dtl in dtls)
            {
                string sql =  this.GetRequestVal("TB_" + dtl.No);
                sql = sql.Trim();
                if (sql == "" || sql == null)
                    continue;

                if (sql.Contains("@Key")==false)
                    return "err@在配置从表:"+dtl.No+" sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + dtl.No + ":" +sql; 
            }
            me.Tag1 = str;
            me.Update();

            return "保存成功.";
        }

        public string TBFullCtrlDDL_Init()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            attrs.Retrieve(MapAttrAttr.FK_MapData, myme.FK_MapData,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            string[] strs = myme.Tag.Split('$');
            foreach (MapAttr attr in attrs)
            {
                foreach (string s in strs)
                {
                    if (s == null)
                        continue;
                    if (s.Contains(attr.KeyOfEn + ":") == false)
                        continue;

                    string[] ss = s.Split(':');
                    attr.DefVal = ss[1]; //使用这个字段作为对应设置的sql.
                }
            }

            return attrs.ToJson();
        }
        public string TBFullCtrlDDL_Save()
        {
            MapExt myme = new MapExt(this.MyPK);

            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            attrs.Retrieve(MapAttrAttr.FK_MapData, myme.FK_MapData,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapAttr attr in attrs)
            {

                string sql = this.GetRequestVal("TB_" + attr.KeyOfEn);
                sql = sql.Trim();
                if (sql == "" || sql == null)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + attr.KeyOfEn + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + attr.KeyOfEn + ":" + sql;
            }
            me.Tag = str;
            me.Update();

            return "保存成功.";
        }
        #endregion TBFullCtrl 功能界面.

        #region AutoFullDLL 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string AutoFullDLL_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.FK_DBSrc = this.GetValFromFrmByKey("FK_DBSrc");
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.
            me.ExtType = MapExtXmlList.AutoFullDLL;

            //执行保存.
            me.InitPK();

            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string AutoFullDLL_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string AutoFullDLL_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion AutoFullDLL 功能界面.

        #region DDLFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string DDLFullCtrl_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.FK_DBSrc = this.GetValFromFrmByKey("FK_DBSrc");
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

            me.ExtType = MapExtXmlList.DDLFullCtrl;

            //执行保存.
            me.InitPK();
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string DDLFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string DDLFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            me.W = i;

            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion DDLFullCtrl 功能界面.

        #region ActiveDDL 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string ActiveDDL_Save()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.AttrsOfActive = this.GetValFromFrmByKey("DDL_AttrsOfActive");
            me.FK_DBSrc = this.GetValFromFrmByKey("FK_DBSrc");
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.
            me.ExtType = MapExtXmlList.ActiveDDL;

            //执行保存.
            me.MyPK = MapExtXmlList.ActiveDDL + "_" + me.FK_MapData + "_" + me.AttrOfOper + "_" + me.AttrOfOper;
            me.Save();

            return "保存成功.";
        }
        public string ActiveDDL_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string ActiveDDL_Init()
        {
            DataSet ds = new DataSet();

            //加载外键字段.
            var sql = "SELECT KeyOfEn AS No, Name FROM Sys_MapAttr WHERE UIContralType=1 AND FK_MapData='" + this.FK_MapData + "' AND KeyOfEn!='" + this.KeyOfEn + "'";
            DataTable dt=BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            ds.Tables.Add(dt);

            if (dt.Rows.Count == 0)
                return "err@表单中没有要级联的下拉框.";
           
            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i= me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);
                 
            dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion ActiveDDL 功能界面.

        #region 单选按钮事件
        /// <summary>
        /// 返回信息。
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Init()
        {
            DataSet ds = new DataSet();

            //放入表单字段.
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            //属性.
            MapAttr attr = new MapAttr();
            attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            attr.Retrieve();

            //把分组加入里面.
            GroupFields gfs = new GroupFields(this.FK_MapData);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupFields"));

            //字段值.
            FrmRBs rbs = new FrmRBs();
            rbs.Retrieve(FrmRBAttr.FK_MapData, this.FK_MapData, FrmRBAttr.KeyOfEn, this.KeyOfEn);
            if (rbs.Count == 0)
            {
                /*初始枚举值变化.
                 */
                SysEnums ses = new SysEnums(attr.UIBindKey);
                foreach (SysEnum se in ses)
                {
                    FrmRB rb = new FrmRB();
                    rb.FK_MapData = this.FK_MapData;
                    rb.KeyOfEn = this.KeyOfEn;
                    rb.IntKey = se.IntKey;
                    rb.Lab = se.Lab;
                    rb.EnumKey = attr.UIBindKey;
                    rb.Insert(); //插入数据.
                }

                rbs.Retrieve(FrmRBAttr.FK_MapData, this.FK_MapData, FrmRBAttr.KeyOfEn, this.KeyOfEn);
            }

            //加入单选按钮.
            ds.Tables.Add(rbs.ToDataTableField("Sys_FrmRB"));
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Save()
        {
            string json = context.Request.Form["data"];
            DataTable dt = BP.Tools.Json.ToDataTable(json);

            foreach (DataRow dr in dt.Rows)
            {
                FrmRB rb = new FrmRB();
                rb.MyPK = dr["MyPK"].ToString();
                rb.Retrieve();

                rb.Script = dr["Script"].ToString();
                rb.FieldsCfg = dr["FieldsCfg"].ToString(); //格式为 @字段名1=1@字段名2=0
                rb.Tip = dr["Tip"].ToString(); //提示信息

                rb.SetVal = dr["SetVal"].ToString(); //设置值.

                rb.Update();
            }

            return "保存成功.";
        }
        #endregion

        #region xxx 界面
        /// <summary>
        /// 初始化正则表达式界面
        /// </summary>
        /// <returns></returns>
        public string RegularExpression_Init()
        {
            DataSet ds = new DataSet();
            string sql = "SELECT * FROM Sys_MapExt WHERE AttrOfOper='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            BP.Sys.XML.RegularExpressions res = new BP.Sys.XML.RegularExpressions();
            res.RetrieveAll();

            DataTable myDT = res.ToDataTable();
            myDT.TableName = "RE";
            ds.Tables.Add(myDT);


            BP.Sys.XML.RegularExpressionDtls dtls = new BP.Sys.XML.RegularExpressionDtls();
            dtls.RetrieveAll();
            DataTable myDTDtls = dtls.ToDataTable();
            myDTDtls.TableName = "REDtl";
            ds.Tables.Add(myDTDtls);

            return BP.Tools.Json.ToJson(ds);
        }
        private void RegularExpression_Save_Tag(string tagID)
        {
            string val = this.GetValFromFrmByKey("TB_Doc_" + tagID);
            if (DataType.IsNullOrEmpty(val))
                return;

            MapExt me = new MapExt();
            me.MyPK = MapExtXmlList.TBFullCtrl + "_" + this.FK_MapData + "_" + this.KeyOfEn + "_" + tagID;
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.KeyOfEn;
            me.ExtType = "RegularExpression";
            me.Tag = tagID;
            me.Doc = val;
            me.Tag1 = this.GetValFromFrmByKey("TB_Tag1_" + tagID);
            me.Save();
        }


        /// <summary>
        /// 执行 保存.
        /// </summary>
        /// <returns></returns>
        public string RegularExpression_Save()
        {
            //删除该字段的全部扩展设置. 
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.ExtType, MapExtXmlList.RegularExpression,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            //执行存盘.
            RegularExpression_Save_Tag("onblur");
            RegularExpression_Save_Tag("onchange");
            RegularExpression_Save_Tag("onclick");
            RegularExpression_Save_Tag("ondblclick");
            RegularExpression_Save_Tag("onkeypress");
            RegularExpression_Save_Tag("onkeyup");
            RegularExpression_Save_Tag("onsubmit");


            return "保存成功...";
        }


        string no;
        string name;
        string fk_dept;
        string oid;
        string kvs;
        public string DealSQL(string sql, string key)
        {
            sql = sql.Replace("@Key", key);
            sql = sql.Replace("@key", key);
            sql = sql.Replace("@Val", key);
            sql = sql.Replace("@val", key);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            if (oid != null)
                sql = sql.Replace("@OID", oid);

            if (DataType.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s)
                        || s.Contains("=") == false)
                        continue;
                    string[] mykv = s.Split('=');
                    sql = sql.Replace("@" + mykv[0], mykv[1]);

                    if (sql.Contains("@") == false)
                        break;
                }
            }
            return sql;
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string PopVal_Init()
        {
            MapExt ext = new MapExt();
            ext.MyPK = this.MyPK;
            if (ext.RetrieveFromDBSources() == 0)
            {
               // throw new Exception("err@主键=" + ext.MyPK + "的配置数据丢失");
                ext.PopValSelectModel = PopValSelectModel.One;
                ext.PopValWorkModel = PopValWorkModel.TableOnly;
            }

            // ext.SetValByKey
            return ext.PopValToJson();
        }
        /// <summary>
        /// 保存设置.
        /// </summary>
        /// <returns></returns>
        public string PopVal_Save()
        {
            try
            {
                MapExt me = new MapExt();
                me.MyPK = this.FK_MapExt;
                me.FK_MapData = this.FK_MapData;
                me.ExtType = "PopVal";
                me.AttrOfOper = this.KeyOfEn;
                me.RetrieveFromDBSources();

                string valWorkModel = this.GetValFromFrmByKey("Model");

                switch (valWorkModel)
                {
                    case "None":
                        me.PopValWorkModel = PopValWorkModel.None;
                        break;
                    case "SelfUrl": //URL模式.
                        me.PopValWorkModel = PopValWorkModel.SelfUrl;
                        me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "TableOnly": //表格模式.
                        me.PopValWorkModel = PopValWorkModel.TableOnly;
                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_Table_SQL");
                        break;
                    case "TablePage": //分页模式.
                        me.PopValWorkModel = PopValWorkModel.TablePage;
                        me.PopValTablePageSQL = this.GetValFromFrmByKey("TB_TablePage_SQL");
                        me.PopValTablePageSQLCount = this.GetValFromFrmByKey("TB_TablePage_SQLCount");
                        break;
                    case "Group": //分组模式.
                        me.PopValWorkModel = PopValWorkModel.Group;

                        me.PopValGroupSQL = this.GetValFromFrmByKey("TB_GroupModel_Group");
                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_GroupModel_Entity");

                        //me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "Tree": //单实体树.
                        me.PopValWorkModel = PopValWorkModel.Tree;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_TreeSQL");
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_TreeParentNo");
                        break;
                    case "TreeDouble": //双实体树.
                        me.PopValWorkModel = PopValWorkModel.TreeDouble;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_DoubleTreeSQL");// 树SQL
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_DoubleTreeParentNo");

                        me.PopValDoubleTreeEntitySQL = this.GetValFromFrmByKey("TB_DoubleTreeEntitySQL"); //实体SQL
                        break;
                    default:
                        break;
                }

                //高级属性.
                me.W = int.Parse(this.GetValFromFrmByKey("TB_Width"));
                me.H = int.Parse(this.GetValFromFrmByKey("TB_Height"));
                me.PopValColNames = this.GetValFromFrmByKey("TB_ColNames"); //中文列名的对应.
                me.PopValTitle = this.GetValFromFrmByKey("TB_Title"); //标题.
                me.PopValSearchTip = this.GetValFromFrmByKey("TB_PopValSearchTip"); //关键字提示.
                me.PopValSearchCond = this.GetValFromFrmByKey("TB_PopValSearchCond"); //查询条件.


                //数据返回格式.
                string popValFormat = this.GetValFromFrmByKey("PopValFormat");
                switch (popValFormat)
                {
                    case "OnlyNo":
                        me.PopValFormat = PopValFormat.OnlyNo;
                        break;
                    case "OnlyName":
                        me.PopValFormat = PopValFormat.OnlyName;
                        break;
                    case "NoName":
                        me.PopValFormat = PopValFormat.NoName;
                        break;
                    default:
                        break;
                }

                //选择模式.
                string seleModel = this.GetValFromFrmByKey("PopValSelectModel");
                if (seleModel == "One")
                    me.PopValSelectModel = PopValSelectModel.One;
                else
                    me.PopValSelectModel = PopValSelectModel.More;

                me.Save();
                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "@保存失败:" + ex.Message;
            }
        }
        #endregion xxx 界面方法.

        #region PopFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string PopFullCtrl_Save()
        {
            try
            {
                MapExt me = new MapExt();
                int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                    MapExtAttr.FK_MapData, this.FK_MapData,
                    MapExtAttr.AttrOfOper, this.KeyOfEn);

                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = this.GetValFromFrmByKey("FK_DBSrc");
                me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

                me.ExtType = MapExtXmlList.PopFullCtrl;

                //执行保存.
                me.InitPK();

                if (me.Update() == 0)
                    me.Insert();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string PopFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string PopFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = this.KeyOfEn;
                me.FK_DBSrc = "local";
            }

            //这个属性没有用.
            me.W = i;  //用于标记该数据是否保存?  从而不现实填充从表，填充下拉框.按钮是否可以用.
            if (me.FK_DBSrc == "")
                me.FK_DBSrc = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 填充从表
        /// </summary>
        /// <returns></returns>
        public string PopFullCtrlDtl_Init()
        {
            MapExt me = new MapExt(this.MyPK);

            string[] strs = me.Tag1.Split('$');
            // 格式为: $ND101Dtl2:SQL.

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, me.FK_MapData);
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) || str.Contains(":") == false)
                    continue;

                string[] kvs = str.Split(':');
                string fk_mapdtl = kvs[0];
                string sql = kvs[1];

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.No != fk_mapdtl)
                        continue;
                    dtl.MTR = sql.Trim();
                }
            }

            foreach (MapDtl dtl in dtls)
            {
                string cols = "";
                MapAttrs attrs = new MapAttrs(dtl.No);
                foreach (MapAttr item in attrs)
                {
                    if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal" || item.KeyOfEn == "RefPK")
                        continue;

                    cols += item.KeyOfEn + ",";
                }
                dtl.Alias = cols; //把ptable作为一个数据参数.
            }
            return dtls.ToJson();
        }

        public string PopFullCtrlDtl_Save()
        {
            MapDtls dtls = new MapDtls(this.FK_MapData);
            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapDtl dtl in dtls)
            {
                string sql = this.GetRequestVal("TB_" + dtl.No);
                sql = sql.Trim();
                if (sql == "" || sql == null)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + dtl.No + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + dtl.No + ":" + sql;
            }
            me.Tag1 = str;
            me.Update();

            return "保存成功.";
        }

        public string PopFullCtrlDDL_Init()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            attrs.Retrieve(MapAttrAttr.FK_MapData, myme.FK_MapData,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            string[] strs = myme.Tag.Split('$');
            foreach (MapAttr attr in attrs)
            {
                foreach (string s in strs)
                {
                    if (s == null)
                        continue;
                    if (s.Contains(attr.KeyOfEn + ":") == false)
                        continue;

                    string[] ss = s.Split(':');
                    attr.DefVal = ss[1]; //使用这个字段作为对应设置的sql.
                }
            }

            return attrs.ToJson();
        }
        public string PopFullCtrlDDL_Save()
        {
            MapExt myme = new MapExt(this.MyPK);

            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            attrs.Retrieve(MapAttrAttr.FK_MapData, myme.FK_MapData,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapAttr attr in attrs)
            {

                string sql = this.GetRequestVal("TB_" + attr.KeyOfEn);
                sql = sql.Trim();
                if (sql == "" || sql == null)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + attr.KeyOfEn + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + attr.KeyOfEn + ":" + sql;
            }
            me.Tag = str;
            me.Update();

            return "保存成功.";
        }
        #endregion PopFullCtrl 功能界面.

    }
}
