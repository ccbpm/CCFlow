using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.CCFast.CCMenu;
using BP.WF.HttpHandler;
using BP.Difference;
using BP.CCBill.Template;
using LitJson;
using System.Text.RegularExpressions;

namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetRequestVal("MethodID");
            }
        }
        /// <summary>
        /// 方法编号
        /// </summary>
        public string MethodNo
        {
            get
            {
                return this.GetRequestVal("MethodNo");
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill()
        {
        }
        #endregion 构造方法.

        #region 方法处理.
        public string MyDict_DoBill_Start()
        {
            //创建单据
            Int64 workid = BP.CCBill.Dev2Interface.CreateBlankBillID(this.FrmID);

            string workids = GetRequestVal("WorkIDs");
            if (DataType.IsNullOrEmpty(workids) == true)
                return "err@请选择需要操作的行";
            string fromFrmID = GetRequestVal("FromFrmID");
            #region 把实体表单的数据集合拷贝到单据从表数据中
            GEEntitys ens = new GEEntitys(fromFrmID);
            QueryObject qo = new QueryObject(ens);
            qo.AddWhereIn("OID", "(" + workids + ")");
            qo.DoQuery();
            GEDtl gedtl = null;
            string mapdtlNo = this.FrmID + "Dtl1";
            GEDtls gedtls = new GEDtls(mapdtlNo);
            gedtls.Retrieve(GEDtlAttr.RefPK, workid);
            foreach (GEEntity en in ens)
            {
                //先判断从表中是不是存在该实体数据，存在continue;
                if (gedtls.IsExits("DictOID", en.OID) == true)
                    continue;
                gedtl = new GEDtl(mapdtlNo);
                gedtl.Copy(en);
                gedtl.RefPKInt64 = workid;
                gedtl.SetValByKey("DictOID", en.OID);
                gedtl.OID = 0;
                gedtl.Insert();
            }
            #endregion 把实体表单的数据集合拷贝到单据从表数据中

            return "./MyBill.htm?FrmID=" + this.FrmID + "&WorkID=" + workid;
        }
        public string MyDict_DoFlowBatchBaseData_StartFlow()
        {
            //创建工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FlowNo);

            string workids = GetRequestVal("WorkIDs");
            if (DataType.IsNullOrEmpty(workids) == true)
                return "err@请选择需要操作的行";
            string fromFrmID = GetRequestVal("FromFrmID");
            #region 把实体表单的数据集合拷贝到流程从表数据中
            GEEntitys ens = new GEEntitys(fromFrmID);
            QueryObject qo = new QueryObject(ens);
            qo.AddWhereIn("OID", "(" + workids + ")");
            qo.DoQuery();
            GEDtl gedtl = null;
            string mapdtlNo = "ND" + int.Parse(this.FlowNo) + "01" + "Dtl1";
            GEDtls gedtls = new GEDtls(mapdtlNo);
            gedtls.Retrieve(GEDtlAttr.RefPK, workid);
            foreach (GEEntity en in ens)
            {
                //先判断从表中是不是存在该实体数据，存在continue;
                if (gedtls.IsExits("DictOID", en.OID) == true)
                    continue;
                gedtl = new GEDtl(mapdtlNo);
                gedtl.Copy(en);
                gedtl.RefPKInt64 = workid;
                gedtl.SetValByKey("DictOID", en.OID);
                gedtl.OID = 0;
                gedtl.Insert();
            }
            #endregion 把实体表单的数据集合拷贝到单据从表数据中

            //更新标记, 表示:该流程被谁发起.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.PWorkID = this.WorkID;
            gwf.PFlowNo = fromFrmID;

            gwf.SetPara("FlowBaseData", "1"); //启动了修改基础资料流程..
            gwf.SetPara("MethodNo", this.MethodNo); //启动了修改基础资料流程..
            gwf.SetPara("DictFrmID", fromFrmID); //启动了修改基础资料流程..
            gwf.SetPara("DictWorkID", workids); //启动了修改基础资料流程..
            gwf.Update();

            //写日志.
            BP.CCBill.Dev2Interface.Dict_AddTrack(fromFrmID, "0", FrmActionType.StartFlow, "启动:" + gwf.FlowName + ",标题:" + gwf.Title);
            return "../MyFlow.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + workid;
        }
        /// <summary>
        /// 执行流程:变更基础资料
        /// </summary>
        /// <returns></returns>
        public string MyDict_DoFlowBaseData_StartFlow()
        {
            BP.CCBill.Template.Method md = new BP.CCBill.Template.Method(this.MethodNo);

            GEEntity en = new GEEntity(md.FrmID, this.WorkID);

            Hashtable ht = new Hashtable();

            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr item in attrs)
            {
                if (item.Key.Equals("BillNo") == false && BP.WF.Glo.FlowFields.Contains("," + item.Key + ",") == true)
                    continue;

                string val = en.GetValStrByKey(item.Key);
                ht.Add(item.Key, val);
                ht.Add("bak" + item.Key, val);
            }

            //创建工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(md.FlowNo, ht);

            //更新标记, 表示:该流程被谁发起.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.PWorkID = this.WorkID;
            gwf.PFlowNo = md.FrmID;

            gwf.SetPara("FlowBaseData", "1"); //启动了修改基础资料流程..
            gwf.SetPara("MethodNo", this.MethodNo); //启动了修改基础资料流程..
            gwf.SetPara("DictFrmID", md.FrmID); //启动了修改基础资料流程..
            gwf.SetPara("DictWorkID", this.WorkID); //启动了修改基础资料流程..
            gwf.Update();

            //写日志.
            BP.CCBill.Dev2Interface.Dict_AddTrack(md.FrmID, this.WorkID.ToString(), FrmActionType.StartFlow, "启动:" + gwf.FlowName + ",标题:" + gwf.Title, null, md.FlowNo, md.Name, int.Parse(md.FlowNo + "01"), workid);

            //   GEEntity frm=new GEEntity("ND"+int.Parse())
            return "../MyFlow.htm?FK_Flow=" + md.FlowNo + "&WorkID=" + workid;
        }
        /// <summary>
        /// 发起其他业务流程
        /// </summary>
        /// <returns></returns>
        public string MyDict_DoFlowEtc_StartFlow()
        {
            BP.CCBill.Template.Method md = new BP.CCBill.Template.Method(this.MethodNo);

            GEEntity en = new GEEntity(md.FrmID, this.WorkID);

            Hashtable ht = new Hashtable();

            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr item in attrs)
            {
                if (BP.WF.Glo.FlowFields.Contains("," + item.Key + ",") == true)
                    continue;

                string val = en.GetValStrByKey(item.Key);
                ht.Add(item.Key, val);
            }

            //创建工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(md.MethodID, ht);

            //更新标记, 表示:该流程被谁发起.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.PWorkID = this.WorkID;
            gwf.PFlowNo = this.FrmID;
            gwf.SetPara("DictFlowEtc", "1"); //启动了其他业务流程.
            gwf.Update();

            //写日志.
            BP.CCBill.Dev2Interface.Dict_AddTrack(md.FrmID, this.WorkID.ToString(), FrmActionType.StartFlow, "启动:" + gwf.FlowName + ",标题:" + gwf.Title,
                null, md.FlowNo, md.Name, int.Parse(md.FlowNo + "01"), workid);

            //GEEntity frm=new GEEntity("ND"+int.Parse())
            return "../MyFlow.htm?FK_Flow=" + md.FlowNo + "&WorkID=" + workid;
        }
        #endregion 

        /// <summary>
        /// 发起列表.
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.CCBill.Dev2Interface.DB_StartBills(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            //草稿列表.
            DataTable dt = BP.CCBill.Dev2Interface.DB_Draft(this.FrmID, BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        public string Recent_Init()
        {
            //草稿列表.
            DataTable dt = BP.CCBill.Dev2Interface.DB_Draft(this.FrmID, BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataTableToJson(dt, false);
        }

        /// <summary>
        /// 单据初始化
        /// </summary>
        /// <returns></returns>
        public string MyBill_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.CCBill.Dev2Interface.DB_StartBills(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public string DoMethod_ExeSQL()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            string doc = func.MethodDoc_SQL;
            string workID = this.WorkIDStr;
            if (DataType.IsNullOrEmpty(workID) == true)
            {
                //批量执行方法
                string workids = this.GetRequestVal("WorkIDs");
                if (DataType.IsNullOrEmpty(workids) == true)
                    throw new Exception("err@执行方法获取到的WorkID或者WorkIDs不能为空");
                string[] strs = workids.Split(',');
                workID = strs[0];
                doc = doc.Replace("@WorkIDs", workids);
            }
            GEEntity en = new GEEntity(func.FrmID, workID);

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.
            string sql = MidStrEx(doc, "/*", "*/");
            try
            {
                DBAccess.RunSQLs(sql);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, workID, "执行方法", func.Name);

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败(DoMethod_ExeSQL).";
                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
        }
        public string DoMethod_ExecFunc()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            string doc = func.Docs;
            BuessUnitBase en = BP.Sys.Base.Glo.GetBuessUnitEntityByEnName(doc);
            try
            {
                string workID = this.WorkIDStr;
                en.WorkID = Int64.Parse(workID);
                en.DoIt();

                BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, workID, "执行方法", func.Name);
                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败(DoMethod_ExecFunc).";
                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
        }
        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="sql">执行的SQL</param>
        /// <returns></returns>
        public string DealException(Exception ex, string sql)
        {
            string errmsg = "";
            // 使用正则表达式匹配字符串
            Regex regex = new Regex("");

            //字段不存在的异常
            regex = new Regex(@"unknown column '(.+)' in 'field list'");
            if (ex.Message != null && regex.IsMatch(ex.Message.ToLower()))
            {
                errmsg = "err@执行失败，物理表的字段不存在，请创建字段.";
            }
            //表不存在的异常
            regex = new Regex(@"table '(.+)' doesn't exist");
            if (ex.Message != null && regex.IsMatch(ex.Message.ToLower()))
            {
                errmsg = "err@执行失败，物理表不存在，请您创建物理表或者表名配置错误.";
            }

            if (DataType.IsNullOrEmpty(sql) == false)
            {
                errmsg += "SQL=" + sql;
            }
            return errmsg;
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <returns></returns>
        public string DoMethodPara_ExeSQL()
        {
            MethodFunc func = new MethodFunc(this.PKVal);
            string doc = func.MethodDoc_SQL;
            string workID = this.WorkIDStr;
            if (DataType.IsNullOrEmpty(workID) == true)
            {
                //批量执行方法
                string workids = this.GetRequestVal("WorkIDs");
                if (DataType.IsNullOrEmpty(workids) == true)
                    throw new Exception("err@执行方法获取到的WorkID或者WorkIDs不能为空");
                string[] strs = workids.Split(',');
                workID = strs[0];
                doc = doc.Replace("@WorkIDs", workids);
            }
            GEEntity en = new GEEntity(func.FrmID, workID);

            #region 替换参数变量.
            if (doc.Contains("@") == true)
            {
                MapAttrs mattrs = new MapAttrs();
                mattrs.Retrieve(MapAttrAttr.FK_MapData, this.PKVal);
                foreach (MapAttr item in mattrs)
                {
                    if (doc.Contains("@") == false)
                        break;
                    if (item.UIContralType == UIContralType.TB)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("TB_" + item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.DDL)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("DDL_" + item.KeyOfEn));
                        continue;
                    }


                    if (item.UIContralType == UIContralType.CheckBok)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("CB_" + item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.RadioBtn)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("RB_" + item.KeyOfEn));
                        continue;
                    }
                }
            }
            #endregion 替换参数变量.

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.
            string sql = MidStrEx(doc, "/*", "*/");
            #region 开始执行SQLs.
            try
            {
                DBAccess.RunSQLs(sql);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                string de = DealException(ex, sql);
                if (DataType.IsNullOrEmpty(de) == false)
                {
                    return de;
                }
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败.";

                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
            #endregion 开始执行SQLs.

            BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, workID, "执行方法", func.Name);

            return "err@" + func.MethodDocTypeOfFunc + ",执行的类型没有解析.";
        }

        public string DoMethodPara_ExeSQL_V3()
        {
            MethodFunc func = new MethodFunc(this.PKVal);
            string doc = func.MethodDoc_SQL;
            string workID = this.WorkIDStr;
            if (DataType.IsNullOrEmpty(workID) == true)
            {
                //批量执行方法
                string workids = this.GetRequestVal("WorkIDs");
                if (DataType.IsNullOrEmpty(workids) == true)
                    throw new Exception("err@执行方法获取到的WorkID或者WorkIDs不能为空");
                string[] strs = workids.Split(',');
                workID = strs[0];
                doc = doc.Replace("@WorkIDs", workids);
            }
            GEEntity en = new GEEntity(func.FrmID, workID);

            #region 替换参数变量.
            if (doc.Contains("@") == true)
            {
                MapAttrs mattrs = new MapAttrs();
                mattrs.Retrieve(MapAttrAttr.FK_MapData, this.PKVal);
                foreach (MapAttr item in mattrs)
                {
                    if (doc.Contains("@") == false)
                        break;
                    if (item.UIContralType == UIContralType.TB)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal(item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.DDL)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal(item.KeyOfEn));
                        continue;
                    }


                    if (item.UIContralType == UIContralType.CheckBok)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal(item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.RadioBtn)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal(item.KeyOfEn));
                        continue;
                    }
                }
            }
            #endregion 替换参数变量.

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.
            string sql = MidStrEx(doc, "/*", "*/");
            #region 开始执行SQLs.
            try
            {
                DBAccess.RunSQLs(sql);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                string de = DealException(ex, sql);
                if (DataType.IsNullOrEmpty(de) == false)
                {
                    return de;
                }
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败.";

                BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, workID, "执行方法失败", func.Name + func.MsgErr);

                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
            #endregion 开始执行SQLs.

            BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, workID, "执行方法", func.Name);

            return "err@" + func.MethodDocTypeOfFunc + ",执行的类型没有解析.";
        }
        /// <summary>
        /// 执行url.
        /// </summary>
        /// <returns></returns>
        public string DoMethodPara_ExeUrl()
        {
            MethodFunc func = new MethodFunc(this.PKVal);
            string doc = func.MethodDoc_Url;
            if (this.WorkID == 0)
            {
                //批量执行方法
                string workids = this.GetRequestVal("WorkIDs");
                if (DataType.IsNullOrEmpty(workids) == true)
                    throw new Exception("err@执行方法获取到的WorkID或者WorkIDs不能为空");
                string[] strs = workids.Split(',');
                this.WorkID = Int64.Parse(strs[0]);
                doc = doc.Replace("@WorkIDs", workids);
            }
            GEEntity en = new GEEntity(func.FrmID, this.WorkID);

            #region 替换参数变量.
            if (doc.Contains("@") == true)
            {
                MapAttrs mattrs = new MapAttrs();
                mattrs.Retrieve(MapAttrAttr.FK_MapData, this.PKVal);
                foreach (MapAttr item in mattrs)
                {
                    if (doc.Contains("@") == false)
                        break;
                    if (item.UIContralType == UIContralType.TB)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("TB_" + item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.DDL)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("DDL_" + item.KeyOfEn));
                        continue;
                    }


                    if (item.UIContralType == UIContralType.CheckBok)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("CB_" + item.KeyOfEn));
                        continue;
                    }

                    if (item.UIContralType == UIContralType.RadioBtn)
                    {
                        doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("RB_" + item.KeyOfEn));
                        continue;
                    }
                }
            }
            #endregion 替换参数变量.

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.

            #region 开始执行SQLs.
            try
            {
                doc += "&MethodName=" + func.MethodID;
                DataType.ReadURLContext(doc, 99999);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                string de = DealException(ex, doc);
                if (DataType.IsNullOrEmpty(de) == false)
                {
                    return de;
                }
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败.";

                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
            #endregion 开始执行SQLs.

            BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, this.WorkID.ToString(), "执行方法", func.Name);

            return "err@" + func.MethodDocTypeOfFunc + ",执行的类型没有解析.";
        }

        #region 单据处理.
        /// <summary>
        /// 创建空白的WorkID.
        /// </summary>
        /// <returns></returns>
        public string MyBill_CreateBlankBillID()
        {
            string PFrmID = this.GetRequestVal("PFrmID");
            string PWorkID = this.GetRequestVal("PWorkID");
            if (DataType.IsNullOrEmpty(PWorkID) == true)
                PWorkID = "0";
            return BP.CCBill.Dev2Interface.CreateBlankBillID(this.FrmID, BP.Web.WebUser.No, null, PFrmID, long.Parse(PWorkID)).ToString();
        }
        /// <summary>
        /// 创建空白的DictID.
        /// </summary>
        /// <returns></returns>
        public string MyDict_CreateBlankDictID()
        {
            return BP.CCBill.Dev2Interface.CreateBlankDictID(this.FrmID, null, null).ToString();
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyBill_SaveIt()
        {
            //创建entity 并执行copy方法.
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            Attrs attrs = rpt.EnMap.Attrs;

            //try
            //{
            //    //执行保存.
            //    rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            //}
            //catch (Exception ex)
            //{
            //    return "err@方法：MyBill_SaveIt错误，在执行 CopyFromRequest 期间" + ex.Message;
            //}
            //执行copy ，这部分与上一个方法重复了.
            try
            {
                Hashtable ht = this.GetMainTableHT();
                foreach (string item in ht.Keys)
                {
                    rpt.SetValByKey(item, ht[item]);
                }
            }
            catch (Exception ex)
            {
                return "err@方法：MyBill_SaveIt错误，在执行  GetMainTableHT 期间" + ex.Message;
            }
            //执行保存.
            try
            {
                rpt.OID = this.WorkID;
                rpt.SetValByKey("BillState", (int)BillState.Editing);
                rpt.Update();
                string str = BP.CCBill.Dev2Interface.SaveBillWork(this.FrmID, this.WorkID);
                return str;
            }
            catch (Exception ex)
            {
                return "err@方法：MyBill_SaveIt 错误，在执行 SaveWork 期间出现错误:" + ex.Message;
            }
        }
        public string MyBill_Submit()
        {
            //执行保存.
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Over);
            rpt.Update();

            string str = BP.CCBill.Dev2Interface.SubmitWork(this.FrmID, this.WorkID);
            return str;
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyDict_SaveIt()
        {
            //执行保存.
            MapData md = new MapData(this.FrmID);
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            //执行保存前事件
            ExecEvent.DoFrm(md, EventListFrm.SaveBefore, rpt, null);

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", 100);
            rpt.Update();


            //执行保存后事件
            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, rpt, null);
            return "保存成功.";
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyDict_Submit()
        {
            return "err@不在支持提交功能.";
            //  throw new Exception("dddssds");
            //执行保存.
            MapData md = new MapData(this.FrmID);
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;

            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            //执行保存前事件
            ExecEvent.DoFrm(md, EventListFrm.SaveBefore, rpt, null);

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Over);
            rpt.Update();

            //执行保存后事件
            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, rpt, null);
            return "提交";
        }

        public string GetFrmEntitys()
        {
            GEEntitys rpts = new GEEntitys(this.FrmID);
            QueryObject qo = new QueryObject(rpts);
            qo.AddWhere("BillState", " != ", 0);
            qo.DoQuery();
            return BP.Tools.Json.ToJson(rpts.ToDataTableField());
        }
        private Hashtable GetMainTableHT()
        {
            Hashtable htMain = new Hashtable();
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (key == null)
                    continue;

                string myKey = key;
                string val = HttpContextHelper.RequestParams(key);
                myKey = myKey.Replace("TB_", "");
                myKey = myKey.Replace("DDL_", "");
                myKey = myKey.Replace("CB_", "");
                myKey = myKey.Replace("RB_", "");
                val = HttpUtility.UrlDecode(val, Encoding.UTF8);

                if (htMain.ContainsKey(myKey) == true)
                    htMain[myKey] = val;
                else
                    htMain.Add(myKey, val);
            }

            return htMain;
        }

        public string MyBill_SaveAsDraft()
        {
            string str = BP.CCBill.Dev2Interface.SaveBillWork(this.FrmID, this.WorkID);
            return str;
        }
        //删除单据
        public string MyBill_Delete()
        {
            return BP.CCBill.Dev2Interface.MyBill_Delete(this.FrmID, this.WorkID);
        }
        public string MyBill_Deletes()
        {
            return BP.CCBill.Dev2Interface.MyBill_DeleteBills(this.FrmID, this.GetRequestVal("WorkIDs"));
        }


        //删除实体
        public string MyDict_Delete()
        {
            return BP.CCBill.Dev2Interface.MyDict_Delete(this.FrmID, this.WorkID);
        }
        /// <summary>
        /// 删除多个
        /// </summary>
        /// <returns></returns>
        public string MyDict_Deletes()
        {
            return BP.CCBill.Dev2Interface.MyDict_DeleteDicts(this.FrmID, this.GetRequestVal("WorkIDs"));
        }

        public string MyEntityTree_Delete()
        {
            return BP.CCBill.Dev2Interface.MyEntityTree_Delete(this.FrmID, this.GetRequestVal("BillNo"));
        }
        /// <summary>
        /// 复制单据数据
        /// </summary>
        /// <returns></returns>
        public string MyBill_Copy()
        {
            return BP.CCBill.Dev2Interface.MyBill_Copy(this.FrmID, this.WorkID);
        }
        #endregion 单据处理.

        #region 获取查询条件
        public string Search_ToolBar()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            //根据FrmID获取Mapdata
            MapData md = new MapData(this.FrmID);
            if (md.EntityType == EntityType.DBList)
            {
                DBListDBSrc dbList = new DBListDBSrc(this.FrmID);
                ds.Tables.Add(dbList.ToDataTableField("Sys_DBList"));
            }
            //如果设置按照时间字段的月度，季度，年度查询数据，需要查询数据显示的最小年份
            if (md.DTSearchWay != DTSearchWay.None && md.GetParaInt("DTShowWay") == 1)
            {
                GEEntity en = new GEEntity(this.FrmID);
                try
                {
                    string sql = "SELECT min(" + md.DTSearchKey + ") From " + en.EnMap.PhysicsTable;
                    md.SetPara("DateShowYear", DBAccess.RunSQLReturnStringIsNull(sql, ""));
                }
                catch (Exception e)
                {
                    GEEntity rpt = new GEEntity(this.FrmID);
                    rpt.CheckPhysicsTable();
                    string sql = "SELECT min(" + md.DTSearchKey + ") From " + en.EnMap.PhysicsTable;
                    md.SetPara("DateShowYear", DBAccess.RunSQLReturnStringIsNull(sql, ""));
                }

            }
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));
            //获取字段属性
            MapAttrs attrs = new MapAttrs(this.FrmID);

            #region //增加枚举/外键字段信息
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.TableName = "Attrs";
            dt.PrimaryKey = new DataColumn[] { dt.Columns["Field"] };
            ds.Tables.Add(dt);
            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            MapAttr mapattr;
            DataRow dr = null;
            MapExts mapExts = new MapExts();
            QueryObject qo = new QueryObject(mapExts);
            qo.AddWhere("FK_MapData", this.FrmID);
            qo.addAnd();
            qo.AddWhereIn("ExtType", "('ActiveDDLSearchCond','AutoFullDLLSearchCond')");
            qo.DoQuery();
            ds.Tables.Add(mapExts.ToDataTableField("Sys_MapExt"));
            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (DataType.IsNullOrEmpty(ctrl) || !DataType.IsNullOrEmpty(HttpContextHelper.RequestParams(ctrl)))
                    continue;

                mapattr = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                if (mapattr == null)
                    continue;

                dr = dt.NewRow();
                dr["Field"] = mapattr.KeyOfEn;
                dr["Name"] = mapattr.Name;
                dr["Width"] = mapattr.UIWidth;
                dt.Rows.Add(dr);

                Attr attr = mapattr.HisAttr;
                if (mapattr == null)
                    continue;

                if (attr.Key.Equals("FK_Dept"))
                    continue;

                if (attr.ItIsEnum == true)
                {
                    SysEnums ses = new SysEnums(mapattr.UIBindKey);
                    DataTable dtEnum = ses.ToDataTableField();
                    dtEnum.TableName = mapattr.KeyOfEn;
                    ds.Tables.Add(dtEnum);
                    continue;
                }
                if (attr.ItIsFK == true)
                {
                    Entities ensFK = attr.HisFKEns;
                    if (ensFK != null)
                    {
                        ensFK.RetrieveAll();
                        DataTable dtEn = ensFK.ToDataTableField();
                        dtEn.TableName = attr.Key;
                        ds.Tables.Add(dtEn);
                    }



                }
                //绑定SQL的外键
                if (ds.Tables.Contains(attr.Key) == false)
                {
                    DataTable dtSQl = null;
                    MapExt mapExt = mapExts.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLLSearchCond, MapExtAttr.AttrOfOper, attr.Key) as MapExt;
                    if (mapExt != null)
                    {
                        string fullSQL = mapExt.Doc.Clone() as string;
                        if (fullSQL == null)
                            throw new Exception("err@字段[" + attr.Key + "]下拉框AutoFullDLLSearchCond，没有配置SQL");

                        fullSQL = fullSQL.Replace("~", "'");
                        fullSQL = BP.WF.Glo.DealExp(fullSQL, null, null);
                        dtSQl = DBAccess.RunSQLReturnTable(fullSQL);
                    }
                    else if (DataType.IsNullOrEmpty(attr.UIBindKey) == false)
                    {
                        dtSQl = BP.Pub.PubClass.GetDataTableByUIBineKey(attr.UIBindKey);
                    }
                    if (dtSQl != null)
                    {
                        foreach (DataColumn col in dtSQl.Columns)
                        {
                            string colName = col.ColumnName.ToLower();
                            switch (colName)
                            {
                                case "no":
                                case "NO":
                                    col.ColumnName = "No";
                                    break;
                                case "name":
                                case "NAME":
                                    col.ColumnName = "Name";
                                    break;
                                case "parentno":
                                case "PARENTNO":
                                    col.ColumnName = "ParentNo";
                                    break;
                                default:
                                    break;
                            }
                        }
                        dtSQl.TableName = attr.Key;
                        ds.Tables.Add(dtSQl);
                    }
                }

            }




            //数据查询权限除只查看自己创建的数据外增加部门的查询条件
            SearchDataRole searchDataRole = (SearchDataRole)md.GetParaInt("SearchDataRole");
            if (searchDataRole != SearchDataRole.ByOnlySelf)
            {
                DataTable dd = GetDeptDataTable(searchDataRole, md);
                if (dd.Rows.Count == 0 && md.GetParaInt("SearchDataRoleByDeptStation") == 1)
                    dd = GetDeptAndSubLevel();
                if (dd.Rows.Count != 0)
                {
                    //增加部门的查询条件
                    if (dt.Rows.Contains("FK_Dept") == false)
                    {
                        dr = dt.NewRow();
                        dr["Field"] = "FK_Dept";
                        dr["Name"] = "部门";
                        dr["Width"] = 120;
                        dt.Rows.Add(dr);
                    }

                    dd.TableName = "FK_Dept";
                    ds.Tables.Add(dd);

                }
            }
            Methods methods = new Methods();
            //实体类方法
            try
            {
                methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsSearchBar, 1, MethodAttr.Idx);
            }
            catch (Exception e)
            {
                methods.GetNewEntity.CheckPhysicsTable();
                methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsSearchBar, 1, MethodAttr.IsEnable, 1, MethodAttr.Idx);

            }
            ds.Tables.Add(methods.ToDataTableField("Frm_Method"));


            Collections colls = Search_BtnPower();
            ds.Tables.Add(methods.ToDataTableField("ToolBtns"));
            ds.Tables.Add(colls.ToDataTableField("Frm_Collection"));

            return BP.Tools.Json.ToJson(ds);

        }
        #endregion 查询条件

        private DataTable GetDeptDataTable(SearchDataRole searchDataRole, MapData md)
        {
            //增加部门的外键
            DataTable dt = new DataTable();
            string sql = "";
            if (searchDataRole == SearchDataRole.ByDept)
            {
                sql = "SELECT D.No,D.Name From Port_Dept D,Port_DeptEmp E WHERE D.No=E.FK_Dept AND E.FK_Emp='" + WebUser.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            if (searchDataRole == SearchDataRole.ByDeptAndSSubLevel)
            {
                dt = GetDeptAndSubLevel();
            }
            if (searchDataRole == SearchDataRole.ByStationDept)
            {
                sql = "SELECT D.No,D.Name From Port_Dept D WHERE D.No IN(SELECT F.FK_Dept FROM Frm_StationDept F,Port_DeptEmpStation P Where F.FK_Station = P.FK_Station AND F.FK_Frm='" + md.No + "' AND P.FK_Emp='" + WebUser.UserID + "')";
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName.ToLower();
                switch (colName)
                {
                    case "no":
                        col.ColumnName = "No";
                        break;
                    case "name":
                        col.ColumnName = "Name";
                        break;

                    default:
                        break;
                }

            }
            return dt;
        }

        private DataTable GetDeptAndSubLevel()
        {
            //获取本部门和兼职部门
            string sql = "SELECT D.No,D.Name From Port_Dept D,Port_DeptEmp E WHERE D.No=E.FK_Dept AND E.FK_Emp='" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.PrimaryKey = new DataColumn[] { dt.Columns["No"] };
            DataTable dd = dt.Copy();
            foreach (DataRow dr in dd.Rows)
            {
                GetSubLevelDeptByParentNo(dt, dr[0].ToString());
            }
            return dt;
        }

        private void GetSubLevelDeptByParentNo(DataTable dt, string parentNo)
        {
            string sql = "SELECT No,Name FROM Port_Dept Where ParentNo='" + parentNo + "'";
            DataTable dd = DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow dr in dd.Rows)
            {
                if (dt.Rows.Contains(dr[0].ToString()) == true)
                    continue;
                dt.Rows.Add(dr.ItemArray);

                GetSubLevelDeptByParentNo(dt, dr[0].ToString());

            }
        }
        public string Search_TreeData()
        {
            MapData mapData = new MapData(this.FrmID);
            int listShowWay = mapData.GetParaInt("ListShowWay");
            string listShowWayKey = mapData.GetParaString("ListShowWayKey");
            if (DataType.IsNullOrEmpty(listShowWayKey) == true)
                return "err@树形结构展示的字段不存在，请检查查询条件设置中展示方式配置是否正确";
            MapAttr mapAttr = new MapAttr(this.FrmID + "_" + listShowWayKey);
            //获取绑定的数据源
            if (DataType.IsNullOrEmpty(mapAttr.UIBindKey) == true)
                return "err@字段" + mapAttr.Name + "绑定的外键或者外部数据源不存在,请检查字段属性[外键SFTable]是否为空";
            DataTable dt = BP.Pub.PubClass.GetDataTableByUIBineKey(mapAttr.UIBindKey);
            return BP.Tools.Json.ToJson(dt);

        }

        /// <summary>
        /// 实体、单据列表显示的字段
        /// </summary>
        /// <returns></returns>
        public string Search_MapAttr()
        {
            DBList dblist = new DBList(this.FrmID);
            if (dblist.EntityType == EntityType.DBList)
                return Search_MapAttrForDB();
            #region 查询显示的列
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);

            MapExts mapExts = new MapExts();
            QueryObject qo = new QueryObject(mapExts);
            qo.AddWhere(MapExtAttr.FK_MapData, this.FrmID);
            qo.addAnd();
            qo.AddWhereIn(MapExtAttr.ExtType, "('MultipleChoiceSmall','SingleChoiceSmall')");
            qo.DoQuery();
            foreach (MapExt mapExt in mapExts)
            {
                //获取mapAttr
                MapAttr mapAttr = mattrs.GetEntityByKey(this.FrmID + "_" + mapExt.AttrOfOper) as MapAttr;
                string searchVisable = mapAttr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                mapAttr.SetPara("SearchVisable", 0);
                mapAttr.Update();
                mapAttr = mattrs.GetEntityByKey(this.FrmID + "_" + mapExt.AttrOfOper + "T") as MapAttr;
                mapAttr.SetPara("SearchVisable", 1);
                mapAttr.Update();
            }
            DataRow row = null;
            DataTable dt = new DataTable("Attrs");
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            dt.Columns.Add("LGType", typeof(int));
            dt.Columns.Add("MyDataType", typeof(int));
            dt.Columns.Add("UIBindKey", typeof(string));
            dt.Columns.Add("AtPara", typeof(string));
            dt.Columns.Add("IsRichText", typeof(int));

            //设置标题、单据号位于开始位置
            foreach (MapAttr attr in mattrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if (DataType.IsNullOrEmpty(searchVisable) == true && attr.UIVisible == false)
                    continue;
                row = dt.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;
                row["LGType"] = attr.LGType;
                row["MyDataType"] = attr.MyDataType;
                row["UIBindKey"] = attr.UIBindKey;
                row["AtPara"] = attr.GetValStringByKey("AtPara");
                row["IsRichText"] = attr.TextModel == 3 ? 1 : 0;
                dt.Rows.Add(row);
            }

            #endregion 查询显示的列
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            //增加枚举
            MapData mapData = new MapData(this.FrmID);
            ds.Tables.Add(mapData.SysEnums.ToDataTableField("Sys_Enum"));
            //查询一行数据的操作
            Methods methods = new Methods();
            methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsList, 1, MethodAttr.Idx);

            ds.Tables.Add(methods.ToDataTableField("Frm_Method"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 获取查询列表的按钮权限
        /// </summary>
        /// <returns></returns>
        public Collections Search_BtnPower()
        {
            //获取该表单所有操作按钮的权限
            Collections colls = new Collections();
            colls.Retrieve(CollectionAttr.FrmID, this.FrmID, CollectionAttr.IsEnable, 1, "Idx");
            if (colls.Count == 0)
            {
                //查询
                Collection collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "Search";
                collection.Name = "查询";
                collection.MethodModel = "Search";
                collection.Mark = "Search";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 0);
                collection.Insert();

                //新建
                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "New";
                collection.Name = "新建";
                collection.MethodModel = "New";
                collection.Mark = "New";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 1);
                collection.Insert();

                //删除
                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "Delete";
                collection.Name = "删除";
                collection.MethodModel = "Delete";
                collection.Mark = "Delete";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 2);
                collection.Insert();

                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "Group";
                collection.Name = "分析";
                collection.MethodModel = "Group";
                collection.Mark = "Group";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 3);
                collection.SetValByKey("IsEnable", false);
                collection.Insert();

                //导出
                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "ExpExcel";
                collection.Name = "导出Excel";
                collection.MethodModel = "ExpExcel";
                collection.Mark = "ExpExcel";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 4);
                collection.Insert();

                //导入
                collection = new Collection();
                collection.FrmID = this.FrmID;
                collection.MethodID = "ImpExcel";
                collection.Name = "导入Excel";
                collection.MethodModel = "ImpExcel";
                collection.Mark = "ImpExcel";
                collection.No = collection.FrmID + "_" + collection.MethodID;
                collection.SetValByKey("Idx", 5);
                collection.Insert();
                colls.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
                return colls;


            }

            //获取针对按钮设置的操作权限
            PowerCenters pcs = new PowerCenters();
            pcs.Retrieve(PowerCenterAttr.CtrlObj, this.FrmID, PowerCenterAttr.CtrlGroup, "SearchBtn");

            string mydepts = "" + WebUser.DeptNo + ","; //我的部门.
            string mystas = ""; //我的角色.

            DataTable mydeptsDT = DBAccess.RunSQLReturnTable("SELECT FK_Dept,FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.UserID + "'");
            foreach (DataRow dr in mydeptsDT.Rows)
            {
                mydepts += dr[0].ToString() + ",";
                mystas += dr[1].ToString() + ",";
            }

            Collections newCollections = new Collections();
            string empIds = "";
            foreach (Collection collection in colls)
            {
                //找到关于系统的控制权限集合.
                PowerCenters mypcs = pcs.GetEntitiesByKey(PowerCenterAttr.CtrlPKVal, collection.No) as PowerCenters;
                //如果没有权限控制的描述，就默认有权限.
                if (mypcs == null)
                {
                    newCollections.AddEntity(collection);
                    continue;
                }

                //控制遍历权限.
                foreach (PowerCenter pc in mypcs)
                {
                    if (pc.CtrlModel.Equals("Anyone") == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }
                    if (pc.CtrlModel.Equals("Adminer") == true && BP.Web.WebUser.No.Equals("admin") == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }

                    if (pc.CtrlModel.Equals("AdminerAndAdmin2") == true && BP.Web.WebUser.IsAdmin == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }
                    empIds = "," + pc.IDs + ",";
                    if (pc.CtrlModel.Equals("Emps") == true && empIds.Contains("," + BP.Web.WebUser.No + ",") == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }

                    //是否包含部门？
                    if (pc.CtrlModel.Equals("Depts") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mydepts) == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }

                    //是否包含角色？
                    if (pc.CtrlModel.Equals("Stations") == true && BP.DA.DataType.IsHaveIt(pc.IDs, mystas) == true)
                    {
                        newCollections.AddEntity(collection);
                        break;
                    }

                    //SQL？
                    if (pc.CtrlModel.Equals("SQL") == true)
                    {
                        string sql = BP.WF.Glo.DealExp(pc.IDs, null, "");
                        if (DBAccess.RunSQLReturnValFloat(sql) > 0)
                        {
                            newCollections.AddEntity(collection);
                        }
                        break;
                    }
                }
            }
            return newCollections;
        }
        /// <summary>
        /// 获取数据源实体列表显示的列及操作列方法
        /// </summary>
        /// <returns></returns>
        public string Search_MapAttrForDB()
        {
            DBList dblist = new DBList(this.FrmID);
            #region 查询显示的列
            MapAttrs mapattrs = new MapAttrs();
            mapattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);

            //查询列表数据源显示的列
            if (DataType.IsNullOrEmpty(dblist.ExpList) == true)
                return "err@数据源实体的列表数据源不能为空，请联系设计人员，检查错误原因.";

            //查询结果集返回的字段列表
            DataTable listDT = null;
            string explist = dblist.ExpList;
            //替换
            if (dblist.DBType.Equals("local"))
            {

                if (explist.ToUpper().Contains("WHERE") == false)
                    explist += " WHERE 1=2";
                else
                    explist += " AND 1=2";
                if (DataType.IsNullOrEmpty(dblist.DBSrc) == true)
                    dblist.DBSrc = "local";
                explist = BP.WF.Glo.DealExp(explist, null);
                SFDBSrc dbSrc = new SFDBSrc(dblist.DBSrc);
                listDT = dbSrc.RunSQLReturnTable(explist);
            }


            DataRow row = null;
            DataTable dt = new DataTable("Attrs");
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            dt.Columns.Add("LGType", typeof(int));
            dt.Columns.Add("MyDataType", typeof(int));
            dt.Columns.Add("UIBindKey", typeof(string));
            dt.Columns.Add("AtPara", typeof(string));
            if (listDT == null)
            {
                foreach (MapAttr attr in mapattrs)
                {
                    string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                    if (searchVisable == "0")
                        continue;
                    if (DataType.IsNullOrEmpty(searchVisable) == true && attr.UIVisible == false)
                        continue;
                    row = dt.NewRow();
                    row["KeyOfEn"] = attr.KeyOfEn;
                    row["Name"] = attr.Name;
                    row["Width"] = attr.UIWidthInt;
                    row["UIContralType"] = attr.UIContralType;
                    row["LGType"] = attr.LGType;
                    row["MyDataType"] = attr.MyDataType;
                    row["UIBindKey"] = attr.UIBindKey;
                    row["AtPara"] = attr.GetValStringByKey("AtPara");
                    dt.Rows.Add(row);
                }
            }
            else
            {
                //设置标题、单据号位于开始位置
                foreach (DataColumn col in listDT.Columns)
                {
                    //获取key
                    string key = col.ColumnName;
                    if (DataType.IsNullOrEmpty(key) == true)
                        continue;
                    MapAttr attr = mapattrs.GetEntityByKey(this.FrmID + "_" + key) as MapAttr;
                    row = dt.NewRow();
                    if (attr == null)
                    {
                        row["KeyOfEn"] = key;
                        row["Name"] = key;
                        row["Width"] = 120;
                        row["UIContralType"] = UIContralType.TB;
                        row["LGType"] = FieldTypeS.Normal;
                        row["MyDataType"] = DataType.AppString;
                        row["UIBindKey"] = "";
                        row["AtPara"] = "";
                        dt.Rows.Add(row);
                        continue;
                    }
                    string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                    if (searchVisable == "0")
                        continue;
                    if (DataType.IsNullOrEmpty(searchVisable) == true && attr.UIVisible == false)
                        continue;
                    row["KeyOfEn"] = attr.KeyOfEn;
                    row["Name"] = attr.Name;
                    row["Width"] = attr.UIWidthInt;
                    row["UIContralType"] = attr.UIContralType;
                    row["LGType"] = attr.LGType;
                    row["MyDataType"] = attr.MyDataType;
                    row["UIBindKey"] = attr.UIBindKey;
                    row["AtPara"] = attr.GetValStringByKey("AtPara");
                    dt.Rows.Add(row);
                }

            }

            #endregion 查询显示的列
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            //增加枚举
            MapData mapData = new MapData(this.FrmID);
            ds.Tables.Add(mapData.SysEnums.ToDataTableField("Sys_Enum"));
            #region 把外键表加入 DataSet
            DataTable ddlTable = new DataTable();
            ddlTable.Columns.Add("No");

            foreach (MapAttr attr in mapattrs)
            {
                //为空、枚举值就continue.
                if (DataType.IsNullOrEmpty(attr.UIBindKey) == true || attr.LGType == FieldTypeS.Enum)
                    continue;
                DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(attr.UIBindKey);
                if (mydt == null)
                {
                    DataRow ddldr = ddlTable.NewRow();
                    ddldr["No"] = attr.UIBindKey;
                    ddlTable.Rows.Add(ddldr);
                }
                else
                {
                    ds.Tables.Add(mydt);
                }
            }
            ddlTable.TableName = "UIBindKey";
            ds.Tables.Add(ddlTable);
            #endregion End把外键表加入DataSet

            //查询一行数据的操作
            Methods methods = new Methods();
            methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsList, 1, MethodAttr.Idx);

            ds.Tables.Add(methods.ToDataTableField("Frm_Method"));

            return BP.Tools.Json.ToJson(ds);
        }

        public string SearchDB_UrlSearchData(string urlExt, string postData)
        {
            urlExt = BP.WF.Glo.DealExp(urlExt, null);
            if (urlExt.Contains("http") == false)
            {
                /*如果没有绝对路径 */
                if (BP.Difference.SystemConfig.isBSsystem)
                {
                    /*在cs模式下自动获取*/
                    string host = HttpContextHelper.RequestUrlHost;//BP.Sys.Base.Glo.Request.Url.Host;
                    if (urlExt.Contains("@AppPath"))
                        urlExt = urlExt.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);//BP.Sys.Base.Glo.Request.ApplicationPath
                    else
                        urlExt = "http://" + HttpContextHelper.RequestUrlAuthority + urlExt;
                }

                if (BP.Difference.SystemConfig.isBSsystem == false)
                {
                    /*在cs模式下它的baseurl 从web.config中获取.*/
                    string cfgBaseUrl = BP.Difference.SystemConfig.AppSettings["HostURL"];
                    if (DataType.IsNullOrEmpty(cfgBaseUrl))
                    {
                        string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                        BP.DA.Log.DebugWriteError(err);
                        throw new Exception(err);
                    }
                    urlExt = cfgBaseUrl + urlExt;
                }
            }

            string json = BP.Tools.PubGlo.HttpPostConnect(urlExt, postData, "POST", true);
            return json;
        }

        public string Search_Init()
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            try
            {
                #region 查询语句
                MapData md = new MapData(this.FrmID);

                //取出来查询条件.
                UserRegedit ur = new UserRegedit(WebUser.No, this.FrmID + "_SearchAttrs");

                GEEntitys rpts = new GEEntitys(this.FrmID);

                Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

                QueryObject qo = new QueryObject(rpts);
                bool isFirst = true; //是否第一次拼接SQL

                #region 关键字字段.
                string keyWord = ur.SearchKey;

                if (md.GetParaBoolen("IsSearchKey") && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
                {
                    Attr attrPK = new Attr();
                    foreach (Attr attr in attrs)
                    {
                        if (attr.ItIsPK)
                        {
                            attrPK = attr;
                            break;
                        }
                    }
                    int i = 0;
                    string enumKey = ","; //求出枚举值外键.
                    foreach (Attr attr in attrs)
                    {
                        switch (attr.MyFieldType)
                        {
                            case FieldType.Enum:
                                enumKey = "," + attr.Key + "Text,";
                                break;
                            case FieldType.FK:
                                continue;
                            default:
                                break;
                        }

                        if (attr.MyDataType != DataType.AppString)
                            continue;

                        //排除枚举值关联refText.
                        if (attr.MyFieldType == FieldType.RefText)
                        {
                            if (enumKey.Contains("," + attr.Key + ",") == true)
                                continue;
                        }

                        if (attr.Key == "FK_Dept")
                            continue;

                        i++;
                        if (i == 1)
                        {
                            isFirst = false;
                            /* 第一次进来。 */
                            qo.addLeftBracket();
                            if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                            else
                                qo.AddWhere(attr.Key, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                            continue;
                        }
                        qo.addOr();

                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");

                    }
                    qo.MyParas.Add("SKey", keyWord);
                    qo.addRightBracket();
                }
                else if (DataType.IsNullOrEmpty(md.GetParaString("StringSearchKeys")) == false)
                {
                    string field = "";//字段名
                    string fieldValue = "";//字段值
                    int idx = 0;

                    //获取查询的字段
                    string[] searchFields = md.GetParaString("StringSearchKeys").Split('*');
                    foreach (String str in searchFields)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        //字段名
                        string[] items = str.Split(',');
                        if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                            continue;
                        field = items[0];
                        //字段名对应的字段值
                        fieldValue = ur.GetParaString(field);
                        if (DataType.IsNullOrEmpty(fieldValue) == true)
                            continue;
                        idx++;
                        if (idx == 1)
                        {
                            isFirst = false;
                            /* 第一次进来。 */
                            qo.addLeftBracket();
                            if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                                qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                            else
                                qo.AddWhere(field, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                            qo.MyParas.Add(field, fieldValue);
                            continue;
                        }
                        qo.addAnd();

                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                        qo.MyParas.Add(field, fieldValue);


                    }
                    if (idx != 0)
                        qo.addRightBracket();
                }

                #endregion 关键字段查询

                #region 时间段的查询
                if (md.GetParaInt("DTSearchWay") != (int)DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
                {
                    string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                    string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                    //按日期查询
                    if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDate)
                    {
                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addLeftBracket();
                        dtTo += " 23:59:59";
                        qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                        qo.addAnd();
                        qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }

                    if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDateTime)
                    {
                        //取前一天的24：00
                        if (dtFrom.Trim().Length == 10) //2017-09-30
                            dtFrom += " 00:00:00";
                        if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                            dtFrom += ":00";

                        dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                        if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                            dtTo += " 24:00";

                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addLeftBracket();
                        qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                        qo.addAnd();
                        qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                        qo.addRightBracket();
                    }
                }
                #endregion 时间段的查询

                #region 外键或者枚举的查询
                //获得关键字.
                AtPara ap = new AtPara(ur.Vals);
                Attr ddattr = null;
                foreach (string str in ap.HisHT.Keys)
                {
                    string val = ap.GetValStrByKey(str);
                    if (val.Equals("all") || val.Equals("null") || val.Equals(""))
                        continue;
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;

                    qo.addLeftBracket();
                    ddattr = attrs.GetAttrByKeyOfEn(str);
                    if (val.IndexOf(",") != -1)
                    {
                        if (ddattr.ItIsNum == true)
                        {
                            qo.AddWhere(str, "IN", "(" + val + ")");
                            qo.addRightBracket();
                            continue;
                        }
                        val = "('" + val.Replace(",", "','") + "')";
                        qo.AddWhere(str, "IN", val);
                        qo.addRightBracket();
                        continue;
                    }
                    if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
                    {
                        object typeVal = BP.Sys.Base.Glo.GenerRealType(attrs, str, ap.GetValStrByKey(str));
                        qo.AddWhere(str, typeVal);

                    }
                    else
                    {
                        qo.AddWhere(str, ap.GetValStrByKey(str));
                    }

                    qo.addRightBracket();
                }
                #endregion 外键或者枚举的查询

                #region 设置隐藏字段的过滤查询

                FrmBill frmBill = new FrmBill(this.FrmID);
                string hidenField = frmBill.GetValStringByKey("HidenField");

                if (frmBill.GetParaInt("HidenWay") == 1)
                {
                    if (DataType.IsNullOrEmpty(hidenField) == false)
                    {
                        hidenField = hidenField.Replace("_WebUser.No", WebUser.No);
                        hidenField = hidenField.Replace("_WebUser.Name", WebUser.Name);
                        hidenField = hidenField.Replace("_WebUser.FK_DeptName", WebUser.DeptName);
                        hidenField = hidenField.Replace("_WebUser.FK_Dept", WebUser.DeptNo);
                        hidenField = hidenField.Replace("_WebUser.OrgNo", WebUser.OrgNo);
                        if (hidenField.IndexOf("_") != -1)
                            return "err@隐藏条件" + hidenField + "还有未替换的_符号";

                        if (isFirst == false)
                            qo.addAnd();
                        else
                            isFirst = false;
                        qo.addSQL(hidenField);
                    }
                }
                #endregion 设置隐藏字段的查询

                #endregion 查询语句

                if (isFirst == false)
                {
                    qo.addAnd();
                }

                qo.AddWhere("BillState", "!=", 0);
                isFirst = false;
                if ((SearchDataRole)md.GetParaInt("SearchDataRole") != SearchDataRole.SearchAll)
                {
                    //默认查询本部门的单据
                    if ((SearchDataRole)md.GetParaInt("SearchDataRole") == SearchDataRole.ByOnlySelf && DataType.IsNullOrEmpty(hidenField) == true
                        || (md.GetParaInt("SearchDataRoleByDeptStation") == 0 && DataType.IsNullOrEmpty(ap.GetValStrByKey("FK_Dept")) == true))
                    {
                        //qo.addAnd();
                        //qo.AddWhere("Starter", "=", WebUser.No);
                    }
                }

                //增加表单字段的查询
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key) || key.Equals("T") == true
                        || key.Equals("t") == true || key.Equals("HttpHandlerName") == true
                        || key.Equals("DoMethod") == true || key.Equals("DoType") == true)
                        continue;
                    if (attrs.Contains(key) == true)
                    {
                        if (isFirst == false)
                            qo.addAnd();
                        qo.AddWhere(key, HttpContextHelper.RequestParams(key));
                        continue;
                    }

                }

                //获得行数.
                ur.SetPara("RecCount", qo.GetCount());
                ur.Save();

                //获取配置信息
                string fieldSet = frmBill.FieldSet;
                string oper = "";
                if (DataType.IsNullOrEmpty(fieldSet) == false)
                {
                    string ptable = rpts.GetNewEntity.EnMap.PhysicsTable;
                    dt = new DataTable("Search_FieldSet");
                    dt.Columns.Add("Field");
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Value");
                    DataRow dr;
                    string[] strs = fieldSet.Split('@');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;
                        string[] item = str.Split('=');
                        if (item.Length == 2)
                        {
                            if (item[1].Contains(",") == true)
                            {
                                string[] ss = item[1].Split(',');
                                foreach (string s in ss)
                                {
                                    dr = dt.NewRow();
                                    dr["Field"] = attrs.GetAttrByKey(s).Desc;
                                    dr["Type"] = item[0];
                                    dt.Rows.Add(dr);

                                    oper += item[0] + "(" + ptable + "." + s + ")" + ",";
                                }
                            }
                            else
                            {
                                dr = dt.NewRow();
                                dr["Field"] = attrs.GetAttrByKey(item[1]).Desc;
                                dr["Type"] = item[0];
                                dt.Rows.Add(dr);

                                oper += item[0] + "(" + ptable + "." + item[1] + ")" + ",";
                            }
                        }
                    }
                    oper = oper.Substring(0, oper.Length - 1);
                    DataTable dd = qo.GetSumOrAvg(oper);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow ddr = dt.Rows[i];
                        ddr["Value"] = dd.Rows[0][i];
                    }
                    ds.Tables.Add(dt);
                }

                if (DataType.IsNullOrEmpty(ur.OrderBy) == false && DataType.IsNullOrEmpty(ur.OrderWay) == false)
                    qo.DoQuery("OID", this.PageSize, this.PageIdx, ur.OrderBy, ur.OrderWay);
                else
                    qo.DoQuery("OID", this.PageSize, this.PageIdx);

                DataTable mydt = rpts.ToDataTableField();
                mydt.TableName = "DT";

                ds.Tables.Add(mydt); //把数据加入里面.

            }
            catch (Exception ex)
            {
                if (ex.Message != null && (ex.Message.Contains("无效") == true || ex.Message.ToLower().Contains("unknown column") == true))
                {
                    GEEntity en = new GEEntity(this.FrmID);
                    en.CheckPhysicsTable();
                    return Search_Init();
                }
                return ex.Message;
            }


            return BP.Tools.Json.ToJson(ds);
        }
        public string SearchDB_Init()
        {

            DataSet ds = new DataSet();
            #region 查询语句
            DBList md = new DBList(this.FrmID);
            if (DataType.IsNullOrEmpty(md.ExpList) == true)
                return "err@列表数据源和的查询不能为空";

            string expList = md.ExpList;
            expList = BP.WF.Glo.DealExp(expList, null);
            //取出来查询条件.
            UserRegedit ur = new UserRegedit(WebUser.No, this.FrmID + "_SearchAttrs");

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;
            string systemKeys = "BillState,RDT,Starter,StarterName,OrgNo,AtPara,";//创建表单时的系统字段

            //获取查询条件
            DataTable whereDT = new DataTable();
            whereDT.Columns.Add("Key");
            whereDT.Columns.Add("Oper");
            whereDT.Columns.Add("Value");
            whereDT.Columns.Add("Type");
            DataRow dr;
            #region 关键字字段.
            string keyWord = ur.SearchKey;
            Hashtable ht = new Hashtable();

            if (md.GetParaBoolen("IsSearchKey") == true)
            {
                if (DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
                {
                    ht.Add("SearchKey", keyWord);
                }
                else
                {
                    Attr attrPK = new Attr();
                    foreach (Attr attr in attrs)
                    {
                        if (attr.ItIsPK)
                        {
                            attrPK = attr;
                            break;
                        }
                    }
                    int i = 0;
                    string enumKey = ","; //求出枚举值外键.
                    foreach (Attr attr in attrs)
                    {
                        if (systemKeys.IndexOf(attr.Key + ",") != -1)
                            continue;
                        switch (attr.MyFieldType)
                        {
                            case FieldType.Enum:
                                enumKey = "," + attr.Key + "Text,";
                                break;
                            case FieldType.FK:
                                continue;
                            default:
                                break;
                        }

                        if (attr.MyDataType != DataType.AppString)
                            continue;

                        //排除枚举值关联refText.
                        if (attr.MyFieldType == FieldType.RefText)
                        {
                            if (enumKey.Contains("," + attr.Key + ",") == true)
                                continue;
                        }

                        if (attr.Key == "FK_Dept")
                            continue;
                        i++;
                        dr = whereDT.NewRow();
                        dr["Key"] = attr.Key;
                        dr["Oper"] = "like";
                        dr["Value"] = keyWord;
                        dr["Type"] = "SearchKey";
                        whereDT.Rows.Add(dr);

                    }
                    ht.Add("SearchKey", keyWord);

                }

            }
            else if (DataType.IsNullOrEmpty(md.GetParaString("StringSearchKeys")) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值

                //获取查询的字段
                string[] searchFields = md.GetParaString("StringSearchKeys").Split('*');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    string[] items = str.Split(',');
                    if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                        continue;
                    field = items[0];
                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                    {
                        ht.Add(field, "");
                        continue;
                    }

                    dr = whereDT.NewRow();
                    dr["Key"] = field;
                    dr["Oper"] = "like";
                    dr["Value"] = fieldValue;
                    dr["Type"] = "StringKey";
                    whereDT.Rows.Add(dr);
                    ht.Add(field, fieldValue);
                }

            }

            #endregion 关键字段查询

            #region 时间段的查询
            if (md.GetParaInt("DTSearchWay") != (int)DTSearchWay.None)
            {
                if (DataType.IsNullOrEmpty(ur.DTFrom) == true)
                {
                    ht.Add("DTFrom", ur.DTFrom);
                    ht.Add("DTTo", ur.DTTo);

                }
                else
                {
                    string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                    string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                    //按日期查询
                    if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDate)
                    {
                        dr = whereDT.NewRow();
                        dr["Key"] = md.GetParaString("DTSearchKey");
                        dr["Oper"] = ">=";
                        dr["Value"] = dtFrom;
                        dr["Type"] = "Date";
                        whereDT.Rows.Add(dr);
                        dtTo += " 23:59:59";
                        dr = whereDT.NewRow();
                        dr["Key"] = md.GetParaString("DTSearchKey");
                        dr["Oper"] = "<=";
                        dr["Value"] = dtTo;
                        dr["Type"] = "Date";
                        whereDT.Rows.Add(dr);
                        ht.Add("DTFrom", dtFrom);
                        ht.Add("DTTo", dtTo);
                    }

                    if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDateTime)
                    {
                        //取前一天的24：00
                        if (dtFrom.Trim().Length == 10) //2017-09-30
                            dtFrom += " 00:00:00";
                        if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                            dtFrom += ":00";

                        dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                        if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                            dtTo += " 24:00";

                        dr = whereDT.NewRow();
                        dr["Key"] = md.GetParaString("DTSearchKey");
                        dr["Oper"] = ">=";
                        dr["Value"] = dtFrom;
                        dr["Type"] = "Date";
                        whereDT.Rows.Add(dr);
                        dr = whereDT.NewRow();
                        dr["Key"] = md.GetParaString("DTSearchKey");
                        dr["Oper"] = "<=";
                        dr["Value"] = dtTo;
                        dr["Type"] = "Date";
                        whereDT.Rows.Add(dr);
                        ht.Add("DTFrom", dtFrom);
                        ht.Add("DTTo", dtTo);
                    }
                }

            }
            #endregion 时间段的查询

            #region 外键或者枚举的查询

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);
            Attr ddattr = null;
            foreach (string str in ap.HisHT.Keys)
            {
                string val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                {
                    ht.Add(str, "");
                    continue;
                }

                dr = whereDT.NewRow();
                dr["Key"] = str;
                dr["Oper"] = "=";
                if (val.IndexOf(",") != -1)
                    dr["Oper"] = "IN";

                dr["Value"] = val;
                dr["Type"] = "Select";
                whereDT.Rows.Add(dr);
                ht.Add(str, ap.GetValStrByKey(str));
            }
            #endregion 外键或者枚举的查询


            //增加表单字段的查询
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (string.IsNullOrEmpty(key) || key.Equals("T") == true
                    || key.Equals("t") == true || key.Equals("HttpHandlerName") == true
                    || key.Equals("DoMethod") == true || key.Equals("DoType") == true)
                    continue;
                if (attrs.Contains(key) == true)
                {
                    dr = whereDT.NewRow();
                    dr["Key"] = key;
                    dr["Oper"] = "=";
                    dr["Value"] = HttpContextHelper.RequestParams(key);
                    dr["Type"] = "Normal";
                    whereDT.Rows.Add(dr);
                    ht.Add(key, HttpContextHelper.RequestParams(key));
                    continue;
                }

            }
            #endregion
            #region 数据源SQL
            if (md.DBType == 0)
            {

                string mainTable = "A.";
                string mainTablePK = md.MainTablePK;

                string whereSQL = "";
                bool isFirstSearchKey = true;
                bool isFirstDateKey = true;

                foreach (DataRow dataRow in whereDT.Rows)
                {
                    string key = dataRow["Key"].ToString();

                    if (expList.IndexOf("@" + Key) != -1)
                    {
                        expList = expList.Replace("@" + Key, dataRow["Value"].ToString());
                        continue;
                    }
                    string type = dataRow["Type"].ToString();
                    if (type.Equals("SearchKey") == true)
                    {
                        if (isFirstSearchKey)
                        {
                            isFirstSearchKey = false;
                            whereSQL += " AND (" + mainTable + key + " like '%" + dataRow["Value"].ToString() + "%' ";
                        }
                        else
                            whereSQL += " OR " + mainTable + key + " like '%" + dataRow["Value"].ToString() + "%' ";
                    }
                    if (isFirstSearchKey == false && type.Equals("SearchKey") == false)
                    {
                        whereSQL += ")";
                        isFirstSearchKey = true;
                    }

                    if (type.Equals("StringKey") == true)
                        whereSQL += " AND " + mainTable + key + " like '%" + dataRow["Value"].ToString() + "%' ";
                    //时间解析
                    if (type.Equals("Date") == true)
                    {

                        if (isFirstDateKey == true)
                        {
                            isFirstDateKey = false;
                            whereSQL += " AND (" + mainTable + key + " " + dataRow["Oper"].ToString() + " '" + dataRow["Value"].ToString() + "' ";
                            continue;
                        }
                        if (isFirstDateKey == false)
                            whereSQL += " AND " + mainTable + key + " " + dataRow["Oper"].ToString() + " '" + dataRow["Value"].ToString() + "')";
                    }
                    if (type.Equals("Select") == true || type.Equals("Normal") == true)
                    {
                        string oper = dataRow["Oper"].ToString();
                        string val = dataRow["Value"].ToString();
                        if (oper.Equals("IN") == true)
                        {
                            ddattr = attrs.GetAttrByKeyOfEn(key);
                            if (ddattr != null)
                            {
                                if (ddattr.ItIsNum)
                                    whereSQL += " AND " + mainTable + key + " " + oper + " (" + val + ") ";
                                else
                                {
                                    val = "('" + val.Replace(",", "','") + "')";
                                    whereSQL += " AND " + mainTable + key + " " + oper + val;
                                }
                            }

                        }
                        else
                            whereSQL += " AND " + mainTable + key + " " + oper + " '" + val + "'";

                    }
                }

                if (isFirstSearchKey == false)
                    whereSQL += ")";
                //expCount = expCount + whereSQL;
                //expList = expList + whereSQL;
                string hidenField = md.GetParaString("HidenField");

                if (DataType.IsNullOrEmpty(hidenField) == false)
                {
                    hidenField = hidenField.Replace("_WebUser.No", WebUser.No);
                    hidenField = hidenField.Replace("_WebUser.Name", WebUser.Name);
                    hidenField = hidenField.Replace("_WebUser.FK_DeptName", WebUser.DeptName);
                    hidenField = hidenField.Replace("_WebUser.FK_Dept", WebUser.DeptNo);
                    hidenField = hidenField.Replace("_WebUser.OrgNo", WebUser.OrgNo);
                    if (hidenField.IndexOf("@") != -1)
                        return "err@隐藏条件" + hidenField + "还有未替换的_符号";
                    whereSQL += " AND (" + hidenField + ")";
                }

                expList = "SELECT * From(" + expList + ") AS A WHERE 1=1 " + whereSQL;//查询列数的
                string expCount = "SELECT Count(*) From(" + expList + ") AS A WHERE 1=1 " + whereSQL;//查询总条数的
                string expPageSize = "SELECT A.OID  From(" + expList + ") AS A WHERE 1=1 " + whereSQL;//查询分页使用的SQL语句

                if (DataType.IsNullOrEmpty(md.DBSrc) == true)
                    md.DBSrc = "local";

                expCount = BP.WF.Glo.DealExp(expCount, null, null);
                expPageSize = BP.WF.Glo.DealExp(expPageSize, null, null);


                SFDBSrc dbsrc = new SFDBSrc(md.DBSrc);
                int count = dbsrc.RunSQLReturnInt(expCount, 0);


                dbsrc.DoQuery(rpts, expList, expPageSize, "OID", attrs, count, this.PageSize, this.PageIdx, ur.OrderBy);
                ur.SetPara("RecCount", count);
                ur.Save();
                DataTable dt = rpts.ToDataTableField("DT");
                ds.Tables.Add(dt); //把数据加入里面.

            }
            #endregion 数据源SQL
            #region URL请求数据
            if (md.DBType == 1)
            {
                ht.Add("PageSize", this.PageSize);
                ht.Add("PageIdx", this.PageIdx);
                // 请求的参数作为JSON字符串发送给列表URL
                string postData = BP.Tools.Json.ToJson(ht);
                if (DataType.IsNullOrEmpty(md.ExpList) == true)
                    return "err@根据URL请求数据的URL为空，请检查配置";
                string json = SearchDB_UrlSearchData(md.ExpList, postData);
                if (DataType.IsNullOrEmpty(json) == true)
                    return "err@根据URL请求数据列表数据为空";
                JsonData jd = JsonMapper.ToObject(json);
                string count = jd["count"].ToString();
                if (DataType.IsNullOrEmpty(count) == true)
                    ur.SetPara("RecCount", 0);
                else
                    ur.SetPara("RecCount", Int32.Parse(count));
                ur.Save();
                string data = jd["data"].ToJson();
                DataTable dt = new DataTable("DT");
                if (DataType.IsNullOrEmpty(data) == false)
                {
                    dt = BP.Tools.Json.ToDataTable(data);
                    dt.TableName = "DT";
                    ds.Tables.Add(dt); //把数据加入里面.
                }

            }
            #endregion URL请求数据

            #region 存储过程的查询
            if (md.DBType == 2)
            {
                string sql = md.ExpList;
                sql = sql.Replace("~", "'");
                Paras paras = new Paras();
                foreach (string key in ht.Keys)
                {
                    paras.Add(key, ht[key]);
                }
                string hidenField = md.GetParaString("HidenField");

                if (DataType.IsNullOrEmpty(hidenField) == false)
                {
                    hidenField = hidenField.Replace("_WebUser.No", WebUser.No);
                    hidenField = hidenField.Replace("_WebUser.Name", WebUser.Name);
                    hidenField = hidenField.Replace("_WebUser.FK_DeptName", WebUser.DeptName);
                    hidenField = hidenField.Replace("_WebUser.FK_Dept", WebUser.DeptNo);
                    hidenField = hidenField.Replace("_WebUser.OrgNo", WebUser.OrgNo);
                    if (hidenField.IndexOf("@") != -1)
                        return "err@隐藏条件" + hidenField + "还有未替换的_符号";
                    string[] strs = hidenField.Split(',');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;
                        string[] strVal = str.Split('=');
                        if (strVal.Length == 1)
                            paras.Add(strVal[0], "");
                        else
                            paras.Add(strVal[0], strVal[1]);
                    }

                }
                DataTable dt = DBAccess.RunProcReturnTable(sql, paras);
                dt.TableName = "DT";
                ds.Tables.Add(dt); //把数据加入里面.
            }
            #endregion 存储过程的查询

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string GenerBill_Init()
        {
            GenerBills bills = new GenerBills();
            bills.Retrieve(GenerBillAttr.Starter, WebUser.No);
            return bills.ToJson();
        }
        /// <summary>
        /// 查询初始化
        /// </summary>
        /// <returns></returns>
        public string SearchData_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");
            if (tSpan == "")
                tSpan = null;

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            GenerBill gb = new GenerBill();
            gb.CheckPhysicsTable();

            sql = "SELECT TSpan as No, COUNT(WorkID) as Num FROM Frm_GenerBill WHERE FrmID='" + this.FrmID + "'  AND Starter='" + WebUser.No + "' AND BillState >= 1 GROUP BY TSpan";

            DataTable dtTSpanNum = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            sql = " SELECT  A.BillState as No, B.Lab as Name, COUNT(WorkID) as Num FROM Frm_GenerBill A, " + BP.Sys.Base.Glo.SysEnum() + " B ";
            sql += " WHERE A.BillState=B.IntKey AND B.EnumKey='BillState' AND  A.Starter='" + WebUser.No + "' AND BillState >=1";
            if (tSpan.Equals("-1") == false)
                sql += "  AND A.TSpan=" + tSpan;

            sql += "  GROUP BY A.BillState, B.Lab  ";

            DataTable dtFlows = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            string sqlWhere = "";
            sqlWhere = "(1 = 1)AND Starter = '" + WebUser.No + "' AND BillState >= 1";
            if (tSpan.Equals("-1") == false)
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }

            if (this.FlowNo != null)
            {
                sqlWhere += "AND (FrmID = '" + this.FrmID + "')  ";
            }
            else
            {
                // sqlWhere += ")";
            }
            sqlWhere += "ORDER BY RDT DESC";

            string fields = " WorkID,FrmID,FrmName,Title,BillState,Starter,StarterName,Sender,RDT ";

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    sql = "SELECT  " + fields + " FROM Frm_GenerBill WHERE " + sqlWhere + " LIMIT 50";
                    break;
                case DBType.MSSQL:
                    sql = "SELECT  TOP 50 " + fields + " FROM Frm_GenerBill WHERE " + sqlWhere;
                    break;
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = "SELECT " + fields + " FROM (SELECT * FROM Frm_GenerBill WHERE " + sqlWhere + ") WHERE rownum <= 50";
                    break;
                default:
                    throw new Exception("err@没有判断的数据库类型.");
                    break;
            }



            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "FrmID";
                mydt.Columns[2].ColumnName = "FrmName";
                mydt.Columns[3].ColumnName = "Title";
                mydt.Columns[4].ColumnName = "BillState";
                mydt.Columns[5].ColumnName = "Starter";
                mydt.Columns[6].ColumnName = "StarterName";
                mydt.Columns[7].ColumnName = "Sender";
                mydt.Columns[8].ColumnName = "RDT";
            }

            mydt.TableName = "Frm_Bill";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    //   dr["TDTime"] =  GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion

            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 查询.

        #region 单据导出
        public string Search_Exp()
        {
            FrmBill frmBill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);

            string name = "数据导出";
            string filename = frmBill.Name + "_" + DataType.CurrentDateTimeCNOfLong + ".xls";
            string filePath = BP.Tools.ExportExcelUtil.ExportDGToExcel(Search_Data(), rpts.GetNewEntity, null, null, filename);
            return filePath;
        }

        public DataTable Search_Data()
        {
            DataSet ds = new DataSet();

            #region 查询语句

            MapData md = new MapData(this.FrmID);


            //取出来查询条件.
            UserRegedit ur = new UserRegedit(WebUser.No, this.FrmID + "_SearchAttrs");

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);

            #region 关键字字段.
            string keyWord = ur.SearchKey;
            bool isFirst = true; //是否第一次拼接SQL

            if (md.GetParaBoolen("IsSearchKey") && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
            {
                Attr attrPK = new Attr();
                foreach (Attr attr in attrs)
                {
                    if (attr.ItIsPK)
                    {
                        attrPK = attr;
                        break;
                    }
                }
                int i = 0;
                string enumKey = ","; //求出枚举值外键.
                foreach (Attr attr in attrs)
                {
                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                            enumKey = "," + attr.Key + "Text,";
                            break;
                        case FieldType.FK:
                            continue;
                        default:
                            break;
                    }

                    if (attr.MyDataType != DataType.AppString)
                        continue;

                    //排除枚举值关联refText.
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        if (enumKey.Contains("," + attr.Key + ",") == true)
                            continue;
                    }

                    if (attr.Key == "FK_Dept")
                        continue;

                    i++;
                    if (i == 1)
                    {
                        isFirst = false;
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }
                    qo.addOr();

                    if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(attr.Key, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                    else
                        qo.AddWhere(attr.Key, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SKey||'%'");

                }
                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();
            }
            else if (DataType.IsNullOrEmpty(md.GetParaString("StringSearchKeys")) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFields = md.GetParaString("StringSearchKeys").Split('*');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    string[] items = str.Split(',');
                    if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                        continue;
                    field = items[0];
                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    idx++;
                    if (idx == 1)
                    {
                        isFirst = false;
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                        qo.MyParas.Add(field, fieldValue);
                        continue;
                    }
                    qo.addAnd();

                    if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(field, " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + field + ",'%')") : ("'%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                    else
                        qo.AddWhere(field, " LIKE ", "'%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + field + "||'%'");
                    qo.MyParas.Add(field, fieldValue);


                }
                if (idx != 0)
                    qo.addRightBracket();
            }

            #endregion 关键字段查询

            #region 时间段的查询
            if (md.GetParaInt("DTSearchWay") != (int)DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                //按日期查询
                if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDate)
                {
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();
                    dtTo += " 23:59:59";
                    qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }

                if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDateTime)
                {
                    //取前一天的24：00
                    if (dtFrom.Trim().Length == 10) //2017-09-30
                        dtFrom += " 00:00:00";
                    if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                        dtFrom += ":00";

                    dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                    if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                        dtTo += " 24:00";

                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();
                    qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }
            }
            #endregion 时间段的查询

            #region 外键或者枚举的查询

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);
            foreach (string str in ap.HisHT.Keys)
            {

                string val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                    continue;
                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;

                qo.addLeftBracket();


                if (BP.Difference.SystemConfig.AppCenterDBFieldIsParaDBType == true)
                {
                    object typeVal = BP.Sys.Base.Glo.GenerRealType(attrs, str, ap.GetValStrByKey(str));
                    qo.AddWhere(str, typeVal);

                }
                else
                {
                    qo.AddWhere(str, ap.GetValStrByKey(str));
                }

                qo.addRightBracket();
            }
            #endregion 外键或者枚举的查询

            #region 设置隐藏字段的过滤查询
            FrmBill frmBill = new FrmBill(this.FrmID);
            string hidenField = frmBill.GetParaString("HidenField");

            if (DataType.IsNullOrEmpty(hidenField) == false)
            {
                hidenField = hidenField.Replace("_WebUser.No", WebUser.No);
                hidenField = hidenField.Replace("_WebUser.Name", WebUser.Name);
                hidenField = hidenField.Replace("_WebUser.FK_DeptName", WebUser.DeptName);
                hidenField = hidenField.Replace("_WebUser.FK_Dept", WebUser.DeptNo);
                hidenField = hidenField.Replace("_WebUser.OrgNo", WebUser.OrgNo);

                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;
                qo.addSQL(hidenField);
            }

            #endregion 设置隐藏字段的查询



            if (isFirst == false)
                qo.addAnd();

            qo.AddWhere("BillState", "!=", 0);

            if ((SearchDataRole)md.GetParaInt("SearchDataRole") != SearchDataRole.SearchAll)
            {
                //默认查询本部门的单据
                if ((SearchDataRole)md.GetParaInt("SearchDataRole") == SearchDataRole.ByOnlySelf && DataType.IsNullOrEmpty(hidenField) == true
                || (md.GetParaInt("SearchDataRoleByDeptStation") == 0 && DataType.IsNullOrEmpty(ap.GetValStrByKey("FK_Dept")) == true))
                {
                    //qo.addAnd();
                    //qo.AddWhere("Starter", "=", WebUser.No);
                }

            }

            #endregion 查询语句
            qo.addOrderBy("OID");
            qo.DoQuery();
            return rpts.ToDataTableField();

        }
        #endregion  执行导出

        #region 单据导入
        public string ImpData_Done()
        {
            if (BP.Difference.SystemConfig.CustomerNo.Equals("ASSET") == true)
                return ImpData_ASSETDone();
            //HttpFileCollection files = HttpContextHelper.RequestFiles();
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请选择要导入的数据信息。";

            string errInfo = "";

            string ext = ".xls";
            string fileName = System.IO.Path.GetFileName(HttpContextHelper.RequestFiles(0).FileName);
            if (fileName.Contains(".xlsx"))
                ext = ".xlsx";


            //设置文件名
            string fileNewName = DBAccess.GenerGUID() + ext;

            //文件存放路径
            string filePath = BP.Difference.SystemConfig.PathOfTemp + "/" + fileNewName;
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), filePath);

            //从excel里面获得数据表.
            DataTable dt = DBLoad.ReadExcelFileToDataTable(filePath);

            //删除临时文件
            System.IO.File.Delete(filePath);

            if (dt.Rows.Count == 0)
                return "err@无导入的数据";

            //获得entity.
            FrmBill bill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);
            GEEntity en = new GEEntity(this.FrmID);



            string noColName = ""; //编号(针对实体表单).
            string nameColName = ""; //名称(针对实体表单).

            BP.En.Map map = en.EnMap;
            Attr attr = map.GetAttrByKey("BillNo");
            noColName = attr.Desc; //
            String codeStruct = bill.EnMap.CodeStruct;
            attr = map.GetAttrByKey("Title");
            nameColName = attr.Desc; //

            //定义属性.
            Attrs attrs = map.Attrs;

            int impWay = this.GetRequestValInt("ImpWay");

            #region 清空方式导入.
            //清空方式导入.
            int count = 0;//导入的行数
            int changeCount = 0;//更新的行数
            String successInfo = "";
            if (impWay == 0)
            {
                rpts.ClearTable();
                GEEntity myen = new GEEntity(this.FrmID);

                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();
                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    myen.OID = 0;

                    //判断是否是自增序列，序列的格式
                    if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                        no = no.PadLeft(System.Int32.Parse(codeStruct), '0');

                    myen.SetValByKey("BillNo", no);
                    if (bill.EntityType == EntityType.FrmDict)
                    {
                        if (myen.Retrieve("BillNo", no) == 1)
                        {
                            errInfo += "err@编号[" + no + "][" + name + "]重复.";
                            continue;
                        }
                    }


                    //给实体赋值
                    errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }

            #endregion 清空方式导入.

            #region 更新方式导入
            if (impWay == 1 || impWay == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();

                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    //判断是否是自增序列，序列的格式
                    if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                    {
                        no = no.PadLeft(System.Int32.Parse(codeStruct), '0');
                    }
                    GEEntity myen = rpts.GetNewEntity as GEEntity;
                    myen.SetValByKey("BillNo", no);
                    if (myen.Retrieve("BillNo", no) == 1 && bill.EntityType == EntityType.FrmDict)
                    {
                        //给实体赋值
                        errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 1, bill);
                        changeCount++;
                        successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                        continue;
                    }


                    //给实体赋值
                    errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }
            #endregion

            return "errInfo=" + errInfo + "@Split" + "count=" + count + "@Split" + "successInfo=" + successInfo + "@Split" + "changeCount=" + changeCount;
        }

        private string SetEntityAttrVal(string no, DataRow dr, Attrs attrs, GEEntity en, DataTable dt, int saveType, FrmBill fbill)
        {

            //单据数据不存在
            if (saveType == 0)
            {
                Int64 oid = 0;
                if (fbill.EntityType == EntityType.FrmDict)
                    oid = BP.CCBill.Dev2Interface.CreateBlankDictID(fbill.No, WebUser.No, null);
                if (fbill.EntityType == EntityType.FrmBill)
                    oid = BP.CCBill.Dev2Interface.CreateBlankBillID(fbill.No, WebUser.No, null);
                en.OID = oid;
                en.RetrieveFromDBSources();
            }

            string errInfo = "";
            //按照属性赋值.
            foreach (Attr item in attrs)
            {
                if (item.Key.Equals("BillNo") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, no);
                    continue;
                }
                if (item.Key.Equals("Title") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, dr[item.Desc].ToString());
                    continue;
                }

                if (dt.Columns.Contains(item.Desc) == false)
                    continue;

                //枚举处理.
                if (item.MyFieldType == FieldType.Enum)
                {
                    string val = dr[item.Desc].ToString();

                    SysEnum se = new SysEnum();
                    int i = se.Retrieve(SysEnumAttr.EnumKey, item.UIBindKey, SysEnumAttr.Lab, val);

                    if (i == 0)
                    {
                        errInfo += "err@枚举[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    en.SetValByKey(item.Key, se.IntKey);
                    continue;
                }

                //外键处理.
                if (item.MyFieldType == FieldType.FK)
                {
                    string val = dr[item.Desc].ToString();
                    Entity attrEn = item.HisFKEn;
                    int i = attrEn.Retrieve("Name", val);
                    if (i == 0)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    if (i != 1)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]重复..";
                        continue;
                    }

                    //把编号值给他.
                    en.SetValByKey(item.Key, attrEn.GetValByKey("No"));
                    continue;
                }

                //boolen类型的处理..
                if (item.MyDataType == DataType.AppBoolean)
                {
                    string val = dr[item.Desc].ToString();
                    if (val == "是" || val == "有")
                        en.SetValByKey(item.Key, 1);
                    else
                        en.SetValByKey(item.Key, 0);
                    continue;
                }

                string myval = dr[item.Desc].ToString();
                en.SetValByKey(item.Key, myval);
            }
            if (DataType.IsNullOrEmpty(en.GetValStrByKey("BillNo")) == true && DataType.IsNullOrEmpty(fbill.BillNoFormat) == false)
                en.SetValByKey("BillNo", Dev2Interface.GenerBillNo(fbill.BillNoFormat, en.OID, en, fbill.No));

            if (DataType.IsNullOrEmpty(en.GetValStrByKey("Title")) == true && DataType.IsNullOrEmpty(fbill.TitleRole) == false)
                en.SetValByKey("Title", Dev2Interface.GenerTitle(fbill.TitleRole, en));

            en.SetValByKey("BillState", (int)BillState.Editing);
            en.Update();

            GenerBill gb = new GenerBill();
            gb.WorkID = en.OID;
            if (gb.RetrieveFromDBSources() == 0)
            {
                gb.BillState = BillState.Over; //初始化状态.
                gb.Starter = BP.Web.WebUser.No;
                gb.StarterName = BP.Web.WebUser.Name;
                gb.FrmName = fbill.Name; //单据名称.
                gb.FrmID = fbill.No; //单据ID
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.FrmTreeNo = fbill.FormTreeNo; //单据类别.
                gb.RDT = DataType.CurrentDateTime;
                gb.NDStep = 1;
                gb.NDStepName = "启动";
                gb.Insert();

            }
            else
            {
                gb.BillState = BillState.Editing;
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.Update();
            }

            return errInfo;
        }

        #endregion
        /**
         * 针对于北京农芯科技的单据导入的处理
         */
        public string ImpData_ASSETDone()
        {
            //HttpFileCollection files = HttpContextHelper.RequestFiles();
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请选择要导入的数据信息。";

            string errInfo = "";

            string ext = ".xls";
            string fileName = System.IO.Path.GetFileName(HttpContextHelper.RequestFiles(0).FileName);
            if (fileName.Contains(".xlsx"))
                ext = ".xlsx";


            //设置文件名
            string fileNewName = DBAccess.GenerGUID() + ext;

            //文件存放路径
            string filePath = BP.Difference.SystemConfig.PathOfTemp + "/" + fileNewName;
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), filePath);

            //从excel里面获得数据表.
            DataTable dt = DBLoad.ReadExcelFileToDataTable(filePath);

            //删除临时文件
            System.IO.File.Delete(filePath);

            if (dt.Rows.Count == 0)
                return "err@无导入的数据";

            //获得entity.
            FrmBill bill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);
            GEEntity en = new GEEntity(this.FrmID);


            string noColName = ""; //编号(唯一值)
            string nameColName = ""; //名称
            BP.En.Map map = en.EnMap;
            //获取表单的主键，合同类的(合同编号),人员信息类的(身份证号),其他(BillNo)
            bool isContractBill = false;
            bool isPersonBill = false;

            if (dt.Columns.Contains("合同编号") == true)
            {
                noColName = "合同编号";
                isContractBill = true;
            }

            else if (dt.Columns.Contains("身份证号") == true)
            {
                noColName = "身份证号";
                isPersonBill = true;
            }

            else
            {

                Attr attr = map.GetAttrByKey("BillNo");
                noColName = attr.Desc;
                attr = map.GetAttrByKey("Title");
                nameColName = attr.Desc;
            }


            string codeStruct = bill.EnMap.CodeStruct;


            //定义属性.
            Attrs attrs = map.Attrs;

            int impWay = this.GetRequestValInt("ImpWay");

            #region 清空方式导入.
            //清空方式导入.
            int count = 0;//导入的行数
            int changeCount = 0;//更新的行数
            String successInfo = "";
            if (impWay == 0)
            {
                rpts.ClearTable();
                GEEntity myen = new GEEntity(this.FrmID);

                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();
                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    myen.OID = 0;

                    if (isContractBill == false && isPersonBill == false)
                    {
                        //判断是否是自增序列，序列的格式
                        if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                            no = no.PadLeft(System.Int32.Parse(codeStruct), '0');

                        myen.SetValByKey("BillNo", no);
                        if (bill.EntityType == EntityType.FrmDict)
                        {
                            if (myen.Retrieve("BillNo", no) == 1)
                            {
                                errInfo += "err@编号[" + no + "][" + name + "]重复.";
                                continue;
                            }
                        }
                    }



                    //给实体赋值
                    errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }

            #endregion 清空方式导入.

            #region 更新方式导入
            if (impWay == 1 || impWay == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();

                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();

                    GEEntity myen = rpts.GetNewEntity as GEEntity;
                    //合同类
                    if (isContractBill == true || isPersonBill == true)
                    {
                        Attr attr = map.GetAttrByDesc(noColName);
                        myen.SetValByKey(attr.Key, no);
                        //存在就编辑修改数据
                        if (myen.Retrieve(attr.Key, no) == 1)
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 1, bill);
                            changeCount++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                            continue;
                        }
                        else
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                            count++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                            continue;
                        }

                    }
                    else
                    {
                        //判断是否是自增序列，序列的格式
                        if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                        {
                            no = no.PadLeft(System.Int32.Parse(codeStruct), '0');
                        }
                        myen.SetValByKey("BillNo", no);
                        if (myen.Retrieve("BillNo", no) == 1 && bill.EntityType == EntityType.FrmDict)
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 1, bill);
                            changeCount++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                            continue;
                        }
                    }

                    //给实体赋值
                    errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }
            #endregion

            return "errInfo=" + errInfo + "@Split" + "count=" + count + "@Split" + "successInfo=" + successInfo + "@Split" + "changeCount=" + changeCount;
        }
        private string SetEntityAttrValForASSET(string no, DataRow dr, Attrs attrs, GEEntity en, DataTable dt, int saveType, FrmBill fbill)
        {

            //单据数据不存在
            if (saveType == 0)
            {
                Int64 oid = 0;
                if (fbill.EntityType == EntityType.FrmDict)
                    oid = BP.CCBill.Dev2Interface.CreateBlankDictID(fbill.No, WebUser.No, null);
                if (fbill.EntityType == EntityType.FrmBill)
                    oid = BP.CCBill.Dev2Interface.CreateBlankBillID(fbill.No, WebUser.No, null);
                en.OID = oid;
                en.RetrieveFromDBSources();
            }

            string errInfo = "";
            //按照属性赋值.
            foreach (Attr item in attrs)
            {
                if (item.Key.Equals("BillNo") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, no);
                    continue;
                }
                if (item.Key.Equals("Title") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, dr[item.Desc].ToString());
                    continue;
                }

                if (dt.Columns.Contains(item.Desc) == false)
                    continue;
                string val = dr[item.Desc].ToString();
                //枚举处理.
                if (item.MyFieldType == FieldType.Enum)
                {
                    SysEnum se = new SysEnum();
                    int i = se.Retrieve(SysEnumAttr.EnumKey, item.UIBindKey, SysEnumAttr.Lab, val);

                    if (i == 0)
                    {
                        errInfo += "err@枚举[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    en.SetValByKey(item.Key, se.IntKey);
                    //en.SetValByKey(item.Key.Replace("Code",""), val);
                    continue;
                }


                //外键处理.
                if (item.MyFieldType == FieldType.FK)
                {
                    Entity attrEn = item.HisFKEn;
                    int i = attrEn.Retrieve("Name", val);
                    if (i == 0)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    if (i != 1)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]重复..";
                        continue;
                    }

                    //把编号值给他.
                    en.SetValByKey(item.Key, attrEn.GetValByKey("No"));
                    if (item.Key.EndsWith("BaseCode") == true)
                        en.SetValByKey(item.Key.Replace("BaseCode", "BaseName"), val);
                    else
                        en.SetValByKey(item.Key.Replace("Code", ""), val);
                    continue;
                }
                //外部数据源
                if (item.MyFieldType == FieldType.Normal && item.MyDataType == DataType.AppString && item.UIContralType == UIContralType.DDL)
                {
                    string uiBindKey = item.UIBindKey;
                    if (DataType.IsNullOrEmpty(uiBindKey) == true)
                        errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "]，绑定的外键为空";
                    DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                    if (mydt.Rows.Count == 0)
                        errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "],对应的外键没有获取到外键列表";
                    bool isHave = false;

                    //给赋值名称
                    if (item.Key.EndsWith("BaseCode") == true)
                        en.SetValByKey(item.Key.Replace("BaseCode", "BaseName"), val);
                    else
                        en.SetValByKey(item.Key.Replace("Code", ""), val);

                    en.SetValByKey(item.Key + "T", val);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        if (mydr["Name"].ToString().Equals(val) == true)
                        {
                            en.SetValByKey(item.Key, mydr["No"].ToString());
                            isHave = true;
                            break;
                        }
                    }

                    if (isHave == false)
                        errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "],没有获取到" + val + "对应的Code值";


                    continue;
                }

                //boolen类型的处理..
                if (item.MyDataType == DataType.AppBoolean)
                {
                    if (val == "是" || val == "有")
                        en.SetValByKey(item.Key, 1);
                    else
                        en.SetValByKey(item.Key, 0);
                    continue;
                }
                if (item.MyDataType == DataType.AppDate)
                {
                    if (DataType.IsNullOrEmpty(val) == false)
                    {

                    }

                }

                if (item.Key.EndsWith("BaseName") == true)
                {
                    BP.Port.Depts depts = new BP.Port.Depts();
                    depts.Retrieve(BP.Port.DeptAttr.Name, val);
                    if (depts.Count != 0)
                        en.SetValByKey(item.Key.Replace("BaseName", "BaseCode"), (depts[0] as BP.Port.Dept).No);
                    en.SetValByKey(item.Key, val);
                    continue;
                }
                else
                {
                    if (item.Key.Equals("CI_SmallBusinessFormatCode"))
                    {
                        string mypk = "MultipleChoiceSmall_" + fbill.No + "_" + item.Key;
                        MapExt mapExt = new MapExt();
                        mapExt.setMyPK(mypk);
                        if (mapExt.RetrieveFromDBSources() == 1 && mapExt.DoWay.Equals("3") && DataType.IsNullOrEmpty(mapExt.Tag3) == false)
                        {
                            string newVal = "," + val + ",";
                            string keyVal = "";
                            DataTable dataTable = BP.Pub.PubClass.GetDataTableByUIBineKey(mapExt.Tag3);
                            foreach (DataRow drr in dataTable.Rows)
                            {
                                if (drr["Name"] != null && newVal.Contains("," + drr["Name"].ToString() + ",") == true)
                                    keyVal += drr["No"].ToString() + ",";
                            }
                            keyVal = keyVal.Substring(0, keyVal.Length - 1);

                            en.SetValByKey(item.Key, keyVal);
                            en.SetValByKey(item.Key.Replace("Code", ""), val);
                            en.SetValByKey(item.Key + "T", val);
                        }
                        else
                        {
                            en.SetValByKey(item.Key, val);
                        }
                    }
                    else
                    {
                        if (item.ItIsNum)
                        {
                            if (DataType.IsNullOrEmpty(val) == true || val.Equals("null") == true)
                                val = "0";
                        }
                        en.SetValByKey(item.Key, val);
                    }


                }


            }
            if (DataType.IsNullOrEmpty(en.GetValStrByKey("BillNo")) == true && DataType.IsNullOrEmpty(fbill.BillNoFormat) == false)
                en.SetValByKey("BillNo", Dev2Interface.GenerBillNo(fbill.BillNoFormat, en.OID, en, fbill.No));

            if (DataType.IsNullOrEmpty(en.GetValStrByKey("Title")) == true && DataType.IsNullOrEmpty(fbill.TitleRole) == false)
                en.SetValByKey("Title", Dev2Interface.GenerTitle(fbill.TitleRole, en));
            en.SetValByKey("Rec", WebUser.No);
            en.SetValByKey("BillState", (int)BillState.Editing);
            en.SetValByKey("WFState", WFState.Complete);
            en.Update();

            GenerBill gb = new GenerBill();
            gb.WorkID = en.OID;
            if (gb.RetrieveFromDBSources() == 0)
            {
                gb.BillState = BillState.Over; //初始化状态.
                gb.Starter = BP.Web.WebUser.No;
                gb.StarterName = BP.Web.WebUser.Name;
                gb.FrmName = fbill.Name; //单据名称.
                gb.FrmID = fbill.No; //单据ID
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.FrmTreeNo = fbill.FormTreeNo; //单据类别.
                gb.RDT = DataType.CurrentDateTime;
                gb.NDStep = 1;
                gb.NDStepName = "启动";
                gb.Insert();

            }
            else
            {
                gb.BillState = BillState.Editing;
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.Update();
            }

            return errInfo;
        }

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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region 获得demo信息.
        public string MethodDocDemoJS_Init()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            return func.MethodDoc_JavaScript_Demo;
        }
        public string MethodDocDemoSQL_Init()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            return func.MethodDoc_SQL_Demo;
        }
        #endregion 获得demo信息.

        #region 处理SQL文中注释信息.
        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            int startindex, endindex;
            string tmpstr = string.Empty;
            string tmpstr2 = string.Empty;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return sourse;
                int i = 0;
                while (startindex != -1)
                {
                    if (i == 0)
                    {
                        endindex = sourse.IndexOf(endstr);
                        if (startindex != 0)
                        {
                            endindex = endindex - startindex;
                        }
                        tmpstr = sourse.Remove(startindex, endindex + endstr.Length);
                    }
                    else
                    {
                        endindex = tmpstr.IndexOf(endstr);
                        if (startindex != 0)
                        {
                            endindex = endindex - startindex;
                        }
                        tmpstr = tmpstr.Remove(startindex, endindex + endstr.Length);

                    }

                    if (endindex == -1)
                        return tmpstr;
                    // tmpstr = tmpstr.Substring(endindex + endstr.Length);
                    startindex = tmpstr.IndexOf(startstr);
                    i++;
                }
                //result = tmpstr.Remove(endindex);

            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("MidStrEx Err:" + ex.Message);
            }
            return tmpstr;
        }
        #endregion 处理SQL文中注释信息..

        #region 实体单据查询启动指定子流程显示的字段
        public string DictFlow_MapAttrs()
        {
            DataSet ds = new DataSet();
            string fk_mapData = "ND" + int.Parse(this.FlowNo) + "01";

            //查询出单流程的所有字段
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.FK_MapData, fk_mapData, MapAttrAttr.Idx);

            ds.Tables.Add(mattrs.ToDataTableField("Sys_MapAttr"));

            MapAttrs mattrsOfSystem1 = new MapAttrs();
            //判断表单中是否存在默认值@WebUser.No,@WebUser.FK_Dept,@RDT
            bool isHaveNo = false;
            bool isHaveRDT = false;
            bool isHaveTitle = false;

            //系统字段字符串
            string sysFields = "";
            foreach (MapAttr mapAttr in mattrs)
            {

                if (mapAttr.KeyOfEn.Equals(GERptAttr.Rec) || mapAttr.KeyOfEn.Equals(GERptAttr.RDT) || mapAttr.KeyOfEn.Equals(GERptAttr.CDT))
                    continue;
                if (mapAttr.KeyOfEn.Equals(GERptAttr.Title) == true)
                {
                    mattrsOfSystem1.AddEntity(mapAttr);
                    isHaveTitle = true;
                    continue;
                }

                switch (mapAttr.DefValReal)
                {

                    case "@WebUser.No":
                    case "@WebUser.Name":
                        sysFields += "," + mapAttr.KeyOfEn;
                        isHaveNo = true;
                        mattrsOfSystem1.AddEntity(mapAttr);

                        break;

                    case "@RDT":
                        mattrsOfSystem1.AddEntity(mapAttr);
                        isHaveRDT = true;
                        sysFields += "," + mapAttr.KeyOfEn;
                        break;
                    default: break;
                }
            }


            //默认显示的系统字段 标题、发起人、发起时间、当前所在节点、状态 , 系统字段需要在RPT中查找
            string fields = "(";
            if (isHaveTitle == false)
                fields += "'" + GERptAttr.Title + "',";
            if (isHaveNo == false)
                fields += "'" + GERptAttr.FlowStarter + "',";

            if (isHaveRDT == false)
                fields += "'" + GERptAttr.FlowStartRDT + "',";
            fields += "'" + GERptAttr.WFState + "','" + GERptAttr.FlowEndNode + "')";
            MapAttrs mattrsOfSystem = new MapAttrs();
            QueryObject qo = new QueryObject(mattrsOfSystem);
            qo.AddWhere(MapAttrAttr.FK_MapData, "ND" + int.Parse(this.FlowNo) + "Rpt");
            qo.addAnd();
            qo.AddWhereIn(MapAttrAttr.KeyOfEn, fields);
            //qo.addOrderBy(MapAttrAttr.Idx);
            //qo.addOrderByOfSelf("CHARINDEX(" + MapAttrAttr.KeyOfEn + ",'" + fields.Replace("'", "") + "')");
            qo.DoQuery();
            mattrsOfSystem.AddEntities(mattrsOfSystem1);

            ds.Tables.Add(mattrsOfSystem.ToDataTableField("Sys_MapAttrOfSystem"));

            //系统字段字符串
            fields = fields.Replace("(", "").Replace(")", "").Replace("'", "") + ",";
            sysFields += ",OID,FID,RDT,CDT,Rec,FK_Dept,MyNum,FK_NY,Emps,Title," + fields;
            DataTable dt = new DataTable();
            dt.Columns.Add("Field");
            dt.TableName = "Sys_Fields";
            DataRow dr = dt.NewRow();
            dr["Field"] = sysFields;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);

            //用户查询注册信息中记录使用到的流程业务表中的字段
            UserRegedit ur = new UserRegedit(WebUser.No, "ND" + int.Parse(this.FlowNo) + "Rpt_SearchAttrs");
            ur.SetPara("RptField", "," + fields);
            ur.Update();

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion

        #region 实体单据启动多个子流程的查询
        public string DictFlow_Search()
        {
            //实体单据的信息
            string frmID = this.GetRequestVal("FrmID");
            string frmOID = this.GetRequestVal("FrmOID");

            //表单编号
            string fk_mapData = "ND" + int.Parse(this.FlowNo) + "01";

            //当前用户查询信息表
            UserRegedit ur = new UserRegedit(WebUser.No, "ND" + int.Parse(this.FlowNo) + "Rpt_SearchAttrs");

            //表单属性
            MapData mapData = new MapData(fk_mapData);

            //流程的系统字段
            string rptFields = ur.GetParaString("RptField");
            rptFields = rptFields.Substring(1, rptFields.Length - 1);
            rptFields = "('" + rptFields.Replace(",", "','") + "'" + ",'" + GERptAttr.FlowStarter + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.PWorkID + "','" + GERptAttr.PFlowNo + "')";
            MapAttrs mattrsOfSystem = new MapAttrs();
            QueryObject qo = new QueryObject(mattrsOfSystem);
            qo.AddWhere(MapAttrAttr.FK_MapData, "ND" + int.Parse(this.FlowNo) + "Rpt");
            qo.addAnd();
            qo.AddWhereIn(MapAttrAttr.KeyOfEn, rptFields);
            qo.DoQuery();

            //流程表单对应的所有字段
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, fk_mapData, MapAttrAttr.Idx);
            attrs.AddEntities(mattrsOfSystem);

            //流程表单对应的流程数据
            GEEntitys ens = new GEEntitys(fk_mapData);
            GEEntity en = ens.GetNewEntity as GEEntity;
            foreach (MapAttr mapAttr in mattrsOfSystem)
                en.EnMap.AddAttr(mapAttr.HisAttr);
            Cache.SQL_Cache.Remove(fk_mapData);

            qo = new QueryObject(ens);
            qo.AddWhere(GERptAttr.PWorkID, frmOID);
            qo.addAnd();
            qo.AddWhere(GERptAttr.PFlowNo, frmID);
            qo.AddWhere(" AND  WFState > 1 ");
            //qo.addAnd();
            qo.AddWhere(" AND FID = 0 ");
            if (DataType.IsNullOrEmpty(ur.OrderBy) == false)
                if (ur.OrderWay.ToUpper().Equals("DESC") == true)
                    qo.addOrderByDesc(ur.OrderBy);
                else
                    qo.addOrderBy(ur.OrderBy);
            ur.Update();
            qo.DoQuery();

            return BP.Tools.Json.ToJson(ens.ToDataTableField("FlowSearch_Data"));
        }
        #endregion  实体单据启动多个子流程的查询

        public string RefDict_CreateBillWorkID()
        {
            Int64 refOID = GetRequestValInt64("RefOID");
            string refDict = GetRequestVal("RefDict");
            //获取关联实体表单的数据信息
            GERpt refRpt = new GERpt(refDict, refOID);
            string billNo = this.GetRequestVal("BillNo");
            Int64 workID = BP.CCBill.Dev2Interface.CreateBlankBillID(this.FrmID, BP.Web.WebUser.No, null, billNo);

            GenerBill gb = new GenerBill(workID);
            gb.BillState = BillState.Draft;
            gb.Update();
            //获取当前单据表单的数据信息
            GERpt rpt = new GERpt(this.FrmID, workID);
            rpt.Copy(refRpt);
            rpt.SetValByKey("BillState", (int)gb.BillState);
            rpt.Update();
            return workID.ToString();
        }

        #region 外部流程网页授权URL
        public string DictFlow_Qcode()
        {
            string state = "FlowNo_" + this.FlowNo + "|OrgNo_" + WebUser.OrgNo + "|FrmID_" + this.FrmID + "|FrmOID_" + this.GetRequestVal("FrmOID");
            //回调url
            string redirect_uri = HttpUtility.UrlEncode("http://www.ccbpm.cn/WF/CCBill/DictFlowStart.htm");
            //授权链接
            string oatuth2 = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + BP.Difference.SystemConfig.AppID + "&redirect_uri=" + redirect_uri + "&response_type=code&scope=snsapi_userinfo&&state=" + state + "#wechat_redirect";
            return oatuth2;
        }
        #endregion 外部流程网页授权URL
    }
}
