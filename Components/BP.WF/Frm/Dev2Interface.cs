using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using BP.En;
using BP.WF.Template;
using BP.WF.Data;

namespace BP.Frm
{
    /// <summary>
    /// 接口调用
    /// </summary>
    public class Dev2Interface
    {
        /// <summary>
        /// 创建工作ID
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="userNo">用户编号</param>
        /// <param name="htParas">参数</param>
        /// <returns>一个新的WorkID</returns>
        public static Int64 CreateBlankBillID(string frmID, string userNo, Hashtable htParas)
        {
            GenerBill gb = new GenerBill();
            int i = gb.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, userNo, GenerBillAttr.BillState, 0);
            if (i == 1)
                return gb.WorkID;

            FrmBill fb = new FrmBill(frmID);

            gb.WorkID = BP.DA.DBAccess.GenerOID("WorkID");
            gb.BillState = BillState.None; //初始化状态.
            gb.Starter = BP.Web.WebUser.No;
            gb.StarterName = BP.Web.WebUser.Name;
            gb.FrmName = fb.Name; //单据名称.
            gb.FrmID = fb.No; //单据ID

            gb.FK_FrmTree = fb.FK_FormTree; //单据类别.
            gb.RDT = BP.DA.DataType.CurrentDataTime;
            gb.NDStep = 1;
            gb.NDStepName = "启动";

            //创建rpt.
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt(frmID);

            //设置标题.
            if (fb.EntityType == EntityType.FrmBill)
            {
                gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
                gb.BillNo = BP.Frm.Dev2Interface.GenerBillNo(fb.BillNoFormat, gb.WorkID, null, frmID);
            }

            if (fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict)
            {
                rpt.EnMap.CodeStruct = fb.EnMap.CodeStruct;
                gb.BillNo = rpt.GenerNewNoByKey("BillNo");
                // BP.Frm.Dev2Interface.GenerBillNo(fb.BillNoFormat, gb.WorkID, null, frmID);
                gb.Title = "";
            }

            gb.DirectInsert(); //执行插入.

            //更新基础的数据到表单表.
            // rpt = new BP.WF.Data.GERpt(frmID);
            rpt.SetValByKey("BillState", (int)gb.BillState);
            rpt.SetValByKey("Starter", gb.Starter);
            rpt.SetValByKey("StarterName", gb.StarterName);
            rpt.SetValByKey("RDT", gb.RDT);
            rpt.SetValByKey("Title", gb.Title);
            rpt.SetValByKey("BillNo", gb.BillNo);
            rpt.OID = gb.WorkID;
            rpt.InsertAsOID(gb.WorkID);

            return gb.WorkID;
        }
        public static Int64 CreateBlankDictID(string frmID, string userNo, Hashtable htParas)
        {

            FrmBill fb = new FrmBill(frmID);

            //创建rpt.
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt(frmID);

            int i= rpt.Retrieve("Starter", WebUser.No, "BillState", 0);
            if (i >= 1)
            {
                rpt.SetValByKey("RDT", DataType.CurrentData);
                return rpt.OID;
            }

            //更新基础的数据到表单表.
            rpt.SetValByKey("BillState", 0);
            rpt.SetValByKey("Starter", WebUser.No);
            rpt.SetValByKey("StarterName", WebUser.Name);
            rpt.SetValByKey("RDT", DataType.CurrentData);

            rpt.EnMap.CodeStruct = fb.EnMap.CodeStruct;

            //rpt.SetValByKey("Title", gb.Title);
            rpt.SetValByKey("BillNo", rpt.GenerNewNoByKey("BillNo"));
            rpt.OID = DBAccess.GenerOID(frmID);
            rpt.InsertAsOID(rpt.OID);
            return rpt.OID;
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string SendWork(string frmID, Int64 workID, string sendToEmpID)
        {
            return "发送chenggong.";
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveWork(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);

            GenerBill gb = new GenerBill(workID);
            gb.BillState = BillState.Editing;

            //创建rpt.
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt(gb.FrmID, workID);

            if (fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict)
            {

                gb.Title = rpt.Title;
                gb.Update();
                return "保存成功...";
            }

            //单据编号.
            if (DataType.IsNullOrEmpty(gb.BillNo) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.BillNo = BP.Frm.Dev2Interface.GenerBillNo(fb.BillNoFormat, workID, null, fb.PTable);
                //更新单据里面的billNo字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "BillNo") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET BillNo='" + gb.BillNo + "' WHERE OID=" + workID);
            }

            //标题.
            if (DataType.IsNullOrEmpty(gb.Title) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
                //更新单据里面的 Title 字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "Title") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET Title='" + gb.Title + "' WHERE OID=" + workID);
            }

            gb.Update();

            //把通用的字段更新到数据库.
            rpt.Title = gb.Title;
            rpt.BillNo = gb.BillNo;
            rpt.Update();

            return "保存成功...";
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveAsDraft(string frmID, Int64 workID)
        {
            GenerBill gb = new GenerBill(workID);
            if (gb.BillState != BillState.None)
                return "err@只有在None的模式下才能保存草稿。";

            if (gb.BillState != BillState.Editing)
            {
                gb.BillState = BillState.Editing;
                gb.Update();
            }
            return "保存成功...";
        }
        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyBill_Delete(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);
            string sqls = "DELETE FROM Frm_GenerBill WHERE WorkID=" + workID;
            sqls += "@DELETE FROM " + fb.PTable + " WHERE OID=" + workID;
            DBAccess.RunSQLs(sqls);
            return "删除成功.";
        }

        /// <summary>
        /// 删除实体单据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyBill_DeleteDicts(string frmID, string workIds)
        {
            FrmBill fb = new FrmBill(frmID);
            string sql = "DELETE FROM " + fb.PTable + " WHERE OID in (" + workIds+")";
            DBAccess.RunSQLs(sql);
            return "删除成功.";
        }

        /// <summary>
        /// 工作退回
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="returnToEmpID"></param>
        /// <param name="returnMsg"></param>
        /// <returns></returns>
        public static string ReturnWork(string frmID, Int64 workID, string returnToEmpID, string returnMsg)
        {
            return "发送chenggong.";
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataSet DB_StartFlows(string empID)
        {
            //定义容器.
            DataSet ds = new DataSet();

            //单据类别.
            BP.Sys.FrmTrees ens = new BP.Sys.FrmTrees();
            ens.RetrieveAll();

            DataTable dtSort = ens.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //查询出来单据运行模式的.
            FrmBills bills = new FrmBills();
            bills.Retrieve(FrmBillAttr.EntityType, 0); //实体类型.

            DataTable dtStart = bills.ToDataTableField();
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);
            return ds;
        }
        /// <summary>
        /// 获得待办列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataTable DB_Todolist(string empID)
        {
            return new DataTable();
        }
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <param name="frmID">单据ID</param>
        /// <param name="empID">操作员</param>
        /// <returns></returns>
        public static DataTable DB_Draft(string frmID, string empID)
        {
            if (DataType.IsNullOrEmpty(empID) == true)
                empID = BP.Web.WebUser.No;

            GenerBills bills = new GenerBills();
            bills.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, empID);

            return bills.ToDataTableField();
        }

        public static string GenerTitle(string titleRole, Entity wk)
        {
            if (DataType.IsNullOrEmpty(titleRole))
            {
                // 为了保持与ccflow4.5的兼容,从开始节点属性里获取.
                Attr myattr = wk.EnMap.Attrs.GetAttrByKey("Title");
                if (myattr == null)
                    myattr = wk.EnMap.Attrs.GetAttrByKey("Title");

                if (myattr != null)
                    titleRole = myattr.DefaultVal.ToString();

                if (DataType.IsNullOrEmpty(titleRole) || titleRole.Contains("@") == false)
                    titleRole = "@WebUser.FK_DeptName-@WebUser.No,@WebUser.Name在@RDT发起.";
            }

            if (titleRole == "@OutPara" || DataType.IsNullOrEmpty(titleRole) == true)
                titleRole = "@WebUser.FK_DeptName-@WebUser.No,@WebUser.Name在@RDT发起.";


            titleRole = titleRole.Replace("@WebUser.No", WebUser.No);
            titleRole = titleRole.Replace("@WebUser.Name", WebUser.Name);
            titleRole = titleRole.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
            titleRole = titleRole.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            titleRole = titleRole.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            titleRole = titleRole.Replace("@RDT", DateTime.Now.ToString("yy年MM月dd日HH时mm分"));
            if (titleRole.Contains("@"))
            {
                Attrs attrs = wk.EnMap.Attrs;

                // 优先考虑外键的替换,因为外键文本的字段的长度相对较长。
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;
                    if (attr.IsRefAttr == false)
                        continue;
                    titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                }

                //在考虑其它的字段替换.
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;

                    if (attr.IsRefAttr == true)
                        continue;
                    titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                }
            }
            titleRole = titleRole.Replace('~', '-');
            titleRole = titleRole.Replace("'", "”");

            // 为当前的工作设置title.
            wk.SetValByKey("Title", titleRole);
            return titleRole;
        }
        /// <summary>
        /// 生成单据编号
        /// </summary>
        /// <param name="billNo">单据编号规则</param>
        /// <param name="workid">工作ID</param>
        /// <param name="en">实体类</param>
        /// <param name="frmID">表单ID</param>
        /// <returns>生成的单据编号</returns>
        public static string GenerBillNo(string billNo, Int64 workid, Entity en, string frmID)
        {
            if (DataType.IsNullOrEmpty(billNo))
                billNo = "3";

            //if (DataType.IsNumStr(billNo) == true)
            //{
            //    return  en.GenerNewNoByKey("BillNo");
            //}


            if (billNo.Contains("@"))
                billNo = BP.WF.Glo.DealExp(billNo, en, null);

            /*如果，Bill 有规则 */
            billNo = billNo.Replace("{YYYY}", DateTime.Now.ToString("yyyy"));
            billNo = billNo.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));

            billNo = billNo.Replace("{yy}", DateTime.Now.ToString("yy"));
            billNo = billNo.Replace("{YY}", DateTime.Now.ToString("yy"));

            billNo = billNo.Replace("{MM}", DateTime.Now.ToString("MM"));
            billNo = billNo.Replace("{mm}", DateTime.Now.ToString("MM"));

            billNo = billNo.Replace("{DD}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{dd}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{HH}", DateTime.Now.ToString("HH"));
            billNo = billNo.Replace("{hh}", DateTime.Now.ToString("HH"));

            billNo = billNo.Replace("{LSH}", workid.ToString());
            billNo = billNo.Replace("{WorkID}", workid.ToString());
            billNo = billNo.Replace("{OID}", workid.ToString());

            if (billNo.Contains("@WebUser.DeptZi"))
            {
                string val = DBAccess.RunSQLReturnStringIsNull("SELECT Zi FROM Port_Dept WHERE No='" + WebUser.FK_Dept + "'", "");
                billNo = billNo.Replace("@WebUser.DeptZi", val.ToString());
            }

            string sql = "";
            int num = 0;
            string supposeBillNo = billNo;  //假设单据号，长度与真实单据号一致
            List<KeyValuePair<int, int>> loc = new List<KeyValuePair<int, int>>();  //流水号位置，流水号位数
            string lsh; //流水号设置码
            int lshIdx = -1;    //流水号设置码所在位置

            for (int i = 2; i < 9; i++)
            {
                lsh = "{LSH" + i + "}";

                if (!supposeBillNo.Contains(lsh))
                    continue;

                while (supposeBillNo.Contains(lsh))
                {
                    //查找流水号所在位置
                    lshIdx = supposeBillNo.IndexOf(lsh);
                    //将找到的流水号码替换成假设的流水号
                    supposeBillNo = (lshIdx == 0 ? "" : supposeBillNo.Substring(0, lshIdx))
                                    + string.Empty.PadLeft(i, '_')
                                    +
                                    (lshIdx + 6 < supposeBillNo.Length
                                         ? supposeBillNo.Substring(lshIdx + 6)
                                         : "");
                    //保存当前流程号所处位置，及流程号长度，以便之后使用替换成正确的流水号
                    loc.Add(new KeyValuePair<int, int>(lshIdx, i));
                }
            }

            //数据库中查找符合的单据号集合,NOTE:此处需要注意，在LIKE中带有左广方括号时，要使用一对广播号将其转义
            sql = "SELECT BillNo FROM Frm_GenerBill WHERE BillNo LIKE '" + supposeBillNo.Replace("[", "[[]") + "'"
                + " AND WorkID <> " + workid
                + " AND FrmID ='" + frmID + "' "
                + " ORDER BY BillNo DESC ";

            string maxBillNo = DBAccess.RunSQLReturnString(sql);
            int ilsh = 0;

            if (string.IsNullOrWhiteSpace(maxBillNo))
            {
                //没有数据，则所有流水号都从1开始
                foreach (KeyValuePair<int, int> kv in loc)
                {
                    supposeBillNo = (kv.Key == 0 ? "" : supposeBillNo.Substring(0, kv.Key))
                                    + "1".PadLeft(kv.Value, '0')
                                    +
                                    (kv.Key + kv.Value < supposeBillNo.Length
                                         ? supposeBillNo.Substring(kv.Key + kv.Value)
                                         : "");
                }
            }
            else
            {
                //有数据，则从右向左开始判断流水号，当右侧的流水号达到最大值，则左侧的流水号自动加1
                Dictionary<int, int> mlsh = new Dictionary<int, int>();
                int plus1idx = -1;

                for (int i = loc.Count - 1; i >= 0; i--)
                {
                    //获取单据号中当前位的流水码数
                    ilsh = Convert.ToInt32(maxBillNo.Substring(loc[i].Key, loc[i].Value));

                    if (plus1idx >= 0)
                    {
                        //如果当前码位被置为+1，则+1，同时将标识置为-1
                        ilsh++;
                        plus1idx = -1;
                    }
                    else
                    {
                        mlsh.Add(loc[i].Key, i == loc.Count - 1 ? ilsh + 1 : ilsh);
                        continue;
                    }

                    if (ilsh >= Convert.ToInt32(string.Empty.PadLeft(loc[i].Value, '9')))
                    {
                        //右侧已经达到最大值
                        if (i > 0)
                        {
                            //记录前位的码
                            mlsh.Add(loc[i].Key, 1);
                        }
                        else
                        {
                            supposeBillNo = "单据号超出范围";
                            break;
                        }

                        //则将前一个流水码位，标记为+1
                        plus1idx = i - 1;
                    }
                    else
                    {
                        mlsh.Add(loc[i].Key, ilsh + 1);
                    }
                }

                if (supposeBillNo == "单据号超出范围")
                    return supposeBillNo;

                //拼接单据号
                foreach (KeyValuePair<int, int> kv in loc)
                {
                    supposeBillNo = (kv.Key == 0 ? "" : supposeBillNo.Substring(0, kv.Key))
                                    + mlsh[kv.Key].ToString().PadLeft(kv.Value, '0')
                                    +
                                    (kv.Key + kv.Value < supposeBillNo.Length
                                         ? supposeBillNo.Substring(kv.Key + kv.Value)
                                         : "");
                }
            }

            billNo = supposeBillNo;

            return billNo;
        }
    }
}
