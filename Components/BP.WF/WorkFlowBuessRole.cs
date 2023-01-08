using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using BP.En;
using BP.Port;
using BP.Web;
using BP.WF.Template.CCEn;


namespace BP.WF
{
    /// <summary>
    /// 工作流程业务规则
    /// </summary>
    public class WorkFlowBuessRole
    {
        #region 生成标题的方法.
        /// <summary>
        /// 生成标题
        /// </summary>
        /// <param name="wk">工作</param>
        /// <param name="emp">人员</param>
        /// <param name="rdt">日期</param>
        /// <returns>生成string.</returns>
        public static string GenerTitle(Flow fl, Work wk, Emp emp, string rdt)
        {
            string titleRole = fl.TitleRole.Clone() as string;
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


            titleRole = titleRole.Replace("@WebUser.No", emp.UserID);
            titleRole = titleRole.Replace("@WebUser.Name", emp.Name);
            titleRole = titleRole.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
            titleRole = titleRole.Replace("@WebUser.FK_DeptName", emp.FK_DeptText);
            titleRole = titleRole.Replace("@WebUser.FK_Dept", emp.FK_Dept);
            titleRole = titleRole.Replace("@RDT", rdt);
            if (titleRole.Contains("@") == true)
            {
                Attrs attrs = wk.EnMap.Attrs;

                // 优先考虑外键的替换。
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
                    if (attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL && attr.MyFieldType == FieldType.Normal)
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key + "T"));
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }
                    else
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }

                }
            }
            titleRole = titleRole.Replace('~', '-');
            titleRole = titleRole.Replace("'", "”");

            if (titleRole.Contains("@"))
            {
                /*如果没有替换干净，就考虑是用户字段拼写错误*/
                throw new Exception("@请检查是否是字段拼写错误，标题中有变量没有被替换下来. @" + titleRole);
            }

            if (titleRole.Contains("@"))
                titleRole = GenerTitleExt(fl, wk.NodeID, wk.OID, titleRole);

            wk.SetValByKey("Title", titleRole);
            return titleRole;
        }
        /// <summary>
        /// 生成标题
        /// </summary>
        /// <param name="wk"></param>
        /// <returns></returns>
        public static string GenerTitle(Flow fl, Work wk)
        {
            string titleRole = fl.TitleRole.Clone() as string;
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
            titleRole = titleRole.Replace("@RDT", DataType.CurrentDateTime);


            if (titleRole.Contains("@"))
            {
                Attrs attrs = wk.EnMap.Attrs;

                // 优先考虑外键的替换 , 因为外键文本的字段的长度相对较长。
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;
                    if (attr.IsRefAttr == false)
                        continue;

                    string temp = wk.GetValStrByKey(attr.Key);
                    if (DataType.IsNullOrEmpty(temp))
                    {
#warning 为什么，加这个代码？牺牲了很多效率，我注销了. by zhoupeng 2016.8.15
                        //  wk.DirectUpdate();
                        // wk.RetrieveFromDBSources();
                    }
                    titleRole = titleRole.Replace("@" + attr.Key, temp);
                }

                //在考虑其它的字段替换.
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;

                    if (attr.IsRefAttr == true)
                        continue;
                    if (attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL && attr.MyFieldType == FieldType.Normal)
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key + "T"));
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }
                    else
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }
                }
            }
            titleRole = titleRole.Replace('~', '-');
            titleRole = titleRole.Replace("'", "”");

            if (titleRole.Contains("@"))
                titleRole = GenerTitleExt(fl, wk.NodeID, wk.OID, titleRole);

            // 为当前的工作设置title.
            wk.SetValByKey("Title", titleRole);

            return titleRole;
        }
        /// <summary>
        /// 生成标题
        /// </summary>
        /// <param name="fl"></param>
        /// <param name="wk"></param>
        /// <returns></returns>
        public static string GenerTitle(Flow fl, GERpt wk)
        {
            string titleRole = fl.TitleRole.Clone() as string;
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


            titleRole = titleRole.Replace("@WebUser.No", wk.FlowStarter);
            titleRole = titleRole.Replace("@WebUser.Name", WebUser.Name);
            titleRole = titleRole.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
            titleRole = titleRole.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            titleRole = titleRole.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            titleRole = titleRole.Replace("@RDT", wk.FlowStartRDT);
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
                    if (attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL && attr.MyFieldType == FieldType.Normal)
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key + "T"));
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }
                    if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key + "Text"));
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }
                    else
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                    }


                }
            }
            titleRole = titleRole.Replace('~', '-');
            titleRole = titleRole.Replace("'", "”");

            if (titleRole.Contains("@"))
                titleRole = GenerTitleExt(fl, int.Parse(fl.No + "01"), wk.OID, titleRole);

            // 为当前的工作设置title.
            wk.SetValByKey("Title", titleRole);
            return titleRole;
        }
        /// <summary>
        /// 如果从节点表单上没有替换下来，就考虑独立表单的替换.
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="workid">工作ID</param>
        /// <returns>返回生成的标题</returns>
        private static string GenerTitleExt(Flow fl, int nodeId, Int64 workid, string titleRole)
        {
            FrmNodes nds = new FrmNodes(fl.No, nodeId);
            foreach (FrmNode item in nds)
            {
                GEEntity en = null;
                try
                {
                    en = new GEEntity(item.FK_Frm);
                    en.PKVal = workid;
                    if (en.RetrieveFromDBSources() == 0)
                        continue;
                }
                catch (Exception ex)
                {
                    continue;
                }

                Attrs attrs = en.EnMap.Attrs;
                // 优先考虑外键的替换,因为外键文本的字段的长度相对较长。
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;
                    if (attr.IsRefAttr == false)
                        continue;
                    titleRole = titleRole.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                }

                //在考虑其它的字段替换.
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;

                    if (attr.IsRefAttr == true)
                        continue;
                    if (attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL && attr.MyFieldType == FieldType.Normal)
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key + "T"));
                        titleRole = titleRole.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                    }
                    else
                    {
                        titleRole = titleRole.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                    }


                }

                //如果全部已经替换完成.
                if (titleRole.Contains("@") == false)
                    return titleRole;
            }
            return titleRole;
        }
        #endregion 生成标题的方法.

        #region 产生单据编号
        /// <summary>
        /// 产生单据编号
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="workid"></param>
        /// <param name="en"></param>
        /// <param name="flowPTable"></param>
        /// <returns></returns>
        public static string GenerBillNo(string billNo, Int64 workid, Entity en, string flowPTable)
        {
            if (DataType.IsNullOrEmpty(billNo))
                return "";

            if (billNo.Contains("@"))
                billNo = BP.WF.Glo.DealExp(billNo, en, null);

            DateTime dt = DateTime.Now;

            /*如果，Bill 有规则 */
            billNo = billNo.Replace("{YYYY}", dt.ToString("yyyy"));
            billNo = billNo.Replace("{yyyy}", dt.ToString("yyyy"));

            billNo = billNo.Replace("{yy}", dt.ToString("yy"));
            billNo = billNo.Replace("{YY}", dt.ToString("yy"));

            billNo = billNo.Replace("{MM}", dt.ToString("MM"));
            billNo = billNo.Replace("{mm}", dt.ToString("MM"));

            billNo = billNo.Replace("{DD}", dt.ToString("dd"));
            billNo = billNo.Replace("{dd}", dt.ToString("dd"));
            billNo = billNo.Replace("{HH}", dt.ToString("HH"));
            billNo = billNo.Replace("{hh}", dt.ToString("HH"));

            billNo = billNo.Replace("{LSH}", workid.ToString());
            billNo = billNo.Replace("{WorkID}", workid.ToString());
            billNo = billNo.Replace("{OID}", workid.ToString());

            if (billNo.Contains("@WebUser.DeptZi"))
            {
                string val = DBAccess.RunSQLReturnStringIsNull("SELECT Zi FROM Port_Dept WHERE No='" + WebUser.FK_Dept + "'", "");
                billNo = billNo.Replace("@WebUser.DeptZi", val.ToString());
            }
            int num = 0;
            string sql = "";
            if (billNo.Contains("{ParentBillNo}"))
            {
                string pWorkID = DBAccess.RunSQLReturnStringIsNull("SELECT PWorkID FROM " + flowPTable + " WHERE   WFState >1 AND  OID=" + workid, "0");
                string parentBillNo = DBAccess.RunSQLReturnStringIsNull("SELECT BillNo FROM WF_GenerWorkFlow WHERE WorkID=" + pWorkID, "");
                billNo = billNo.Replace("{ParentBillNo}", parentBillNo);

                sql = "";
                num = 0;
                for (int i = 2; i < 9; i++)
                {
                    if (billNo.Contains("{LSH" + i + "}") == false)
                        continue;

                    sql = "SELECT COUNT(OID) FROM " + flowPTable + " WHERE PWorkID =" + pWorkID + " AND WFState >1 ";
                    num = DBAccess.RunSQLReturnValInt(sql, 0);
                    billNo = billNo + num.ToString().PadLeft(i, '0');
                    billNo = billNo.Replace("{LSH" + i + "}", "");
                    break;
                }
                return billNo;
            }

            num = 0;
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
            supposeBillNo = supposeBillNo.Replace("[", "[[]");
            sql = "SELECT Max(BillNo) FROM " + flowPTable + " WHERE BillNo LIKE '" + supposeBillNo + "%'";
            if (flowPTable.ToLower() == "wf_generworkflow")
                sql += " AND WorkID <> " + workid + " AND WFState > 1 ";
            else
                sql += " AND OID <> " + workid + " ";
           // sql += " ORDER BY BillNo ";

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
            return supposeBillNo;
        }
        #endregion 产生单据编号

        #region 找到下一个节点的接受人员
        /// <summary>
        /// 找到下一个节点的接受人员
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="currNode">当前节点</param>
        /// <param name="toNode">到达节点</param>
        /// <returns>下一步工作人员No,Name格式的返回.</returns>
        public static DataTable RequetNextNodeWorkers(Flow fl, Node currNode, Node toNode, Entity enParas, Int64 workid)
        {
            if (toNode.IsGuestNode)
            {
                /*到达的节点是客户参与的节点. add by zhoupeng 2016.5.11*/
                DataTable mydt = new DataTable();
                mydt.Columns.Add("No", typeof(string));
                mydt.Columns.Add("Name", typeof(string));

                DataRow dr = mydt.NewRow();
                dr["No"] = "Guest";
                dr["Name"] = "外部用户";
                mydt.Rows.Add(dr);
                return mydt;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            //变量.
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            Paras ps = new Paras();
            // 按上一节点发送人处理。
            if (toNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeEmp)
            {
                DataRow dr = dt.NewRow();
                dr[0] = BP.Web.WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            #region 首先判断是否配置了获取下一步接受人员的sql.
            if (toNode.HisDeliveryWay == DeliveryWay.BySQL
                || toNode.HisDeliveryWay == DeliveryWay.BySQLTemplate
                || toNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {
                if (toNode.HisDeliveryWay == DeliveryWay.BySQLTemplate)
                {
                    SQLTemplate st = new SQLTemplate(toNode.DeliveryParas);
                    sql = st.Docs;
                }
                else
                {
                    if (toNode.DeliveryParas.Length < 4)
                        throw new Exception("@您设置的当前节点按照SQL，决定下一步的接受人员，但是你没有设置SQL.");
                    sql = toNode.DeliveryParas;
                    sql = sql.Clone().ToString();
                }

                //特殊的变量.
                sql = sql.Replace("@FK_Node", toNode.NodeID.ToString());
                sql = sql.Replace("@NodeID", toNode.NodeID.ToString());

                sql = Glo.DealExp(sql, enParas, null);


                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                    throw new Exception("@没有找到可接受的工作人员。@技术信息：执行的SQL没有发现人员:" + sql);
                return dt;
            }
            #endregion 首先判断是否配置了获取下一步接受人员的sql.

            #region 按绑定部门计算,该部门一人处理标识该工作结束(子线程)..
            if (toNode.HisDeliveryWay == DeliveryWay.BySetDeptAsSubthread)
            {
                if (toNode.IsSubThread == false)
                    throw new Exception("@您设置的节点接收人方式为：按绑定部门计算,该部门一人处理标识该工作结束(子线程)，但是当前节点非子线程节点。");

                sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ", Name,FK_Dept AS GroupMark FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + toNode.NodeID + ")";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                    throw new Exception("@没有找到可接受的工作人员,接受人方式为, ‘按绑定部门计算,该部门一人处理标识该工作结束(子线程)’ @技术信息：执行的SQL没有发现人员:" + sql);
                return dt;
            }
            #endregion 按绑定部门计算,该部门一人处理标识该工作结束(子线程)..

            #region 按照明细表,作为子线程的接收人.
            if (toNode.HisDeliveryWay == DeliveryWay.ByDtlAsSubThreadEmps)
            {
                if (toNode.IsSubThread == false)
                    throw new Exception("@您设置的节点接收人方式为：以分流点表单的明细表数据源确定子线程的接收人，但是当前节点非子线程节点。");

                currNode.WorkID = workid; //为获取表单ID ( NodeFrmID )提供参数.

                BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(currNode.NodeFrmID);
                string msg = null;
                foreach (BP.Sys.MapDtl dtl in dtls)
                {
                    try
                    {
                        string empFild = toNode.DeliveryParas;
                        if (DataType.IsNullOrEmpty(empFild))
                            empFild = " UserNo ";

                        ps = new Paras();
                        ps.SQL = "SELECT " + empFild + ", * FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "OID ORDER BY OID";
                        ps.Add("OID", workid);
                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                            throw new Exception("@流程设计错误，到达的节点（" + toNode.Name + "）在指定的节点中没有数据，无法找到子线程的工作人员。");
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        msg += ex.Message;
                        //if (dtls.Count == 1)
                        //    throw new Exception("@估计是流程设计错误,没有在分流节点的明细表中设置");
                    }
                }
                throw new Exception("@没有找到分流节点的明细表作为子线程的发起的数据源，流程设计错误，请确认分流节点表单中的明细表是否有UserNo约定的系统字段。" + msg);
            }
            #endregion 按照明细表,作为子线程的接收人.

            #region 按节点绑定的人员处理.
            if (toNode.HisDeliveryWay == DeliveryWay.ByBindEmp)
            {
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.SQL = "SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + dbStr + "FK_Node ORDER BY FK_Emp";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    throw new Exception("@流程设计错误:下一个节点(" + toNode.Name + ")没有绑定工作人员 . ");
                return dt;
            }
            #endregion 按节点绑定的人员处理.

            #region 按照选择的人员处理。
            if (toNode.HisDeliveryWay == DeliveryWay.BySelected
                || toNode.HisDeliveryWay == DeliveryWay.ByFEE)
            {
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", workid);
                ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    /*从上次发送设置的地方查询. */
                    SelectAccpers sas = new SelectAccpers();
                    int i = sas.QueryAccepterPriSetting(toNode.NodeID);
                    if (i == 0)
                    {
                        if (toNode.HisDeliveryWay == DeliveryWay.BySelected)
                        {
                            if (currNode.CondModel != DirCondModel.ByPopSelect)
                            {
                                // 2020.08.17 这里注释掉了， 有可能是到达的节点是，按照弹出窗体计算的. 
                                // 不做强制修改.
                                //currNode.CondModel = DirCondModel.SendButtonSileSelect;
                                //currNode.Update();
                                //throw new Exception("@下一个节点的接收人规则是按照上一步发送人员选择器选择的，但是在当前节点您没有启接收人选择器，系统已经自动做了设置，请关闭当前窗口重新打开重试。");
                            }

                            throw new Exception("@请选择下一步骤工作(" + toNode.Name + ")接受人员。");
                        }
                        else
                        {
                            throw new Exception("@流程设计错误，请重写FEE，然后为节点(" + toNode.Name + ")设置接受人员，详细请参考cc流程设计手册。");
                        }
                    }

                    //插入里面.
                    foreach (SelectAccper item in sas)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = item.FK_Emp;
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                return dt;
            }
            #endregion 按照选择的人员处理。

            #region 按照指定节点的处理人计算。
            if (toNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmp
                || toNode.HisDeliveryWay == DeliveryWay.ByStarter)
            {
                /* 按指定节点角色上的人员计算 */
                string strs = toNode.DeliveryParas;
                if (toNode.HisDeliveryWay == DeliveryWay.ByStarter)
                {
                    /*找开始节点的处理人员. */
                    strs = int.Parse(fl.No) + "01";
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                    ps.Add("FK_Node", int.Parse(strs));
                    ps.Add("OID", workid);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 1)
                        return dt;
                    else
                    {
                        /* 有可能当前节点就是第一个节点，那个时间还没有初始化数据，就返回当前人. */
                        DataRow dr = dt.NewRow();
                        dr[0] = BP.Web.WebUser.No;
                        dt.Rows.Add(dr);
                        return dt;
                    }
                }

                // 首先从本流程里去找。
                strs = strs.Replace(";", ",");
                string[] ndStrs = strs.Split(',');
                foreach (string nd in ndStrs)
                {
                    if (DataType.IsNullOrEmpty(nd))
                        continue;

                    if (DataType.IsNumStr(nd) == false)
                        throw new Exception("流程设计错误:您设置的节点(" + toNode.Name + ")的接收方式为按指定的节点角色投递，但是您没有在访问规则设置中设置节点编号。");

                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                    ps.Add("FK_Node", int.Parse(nd));
                    if (currNode.IsSubThread == true)
                        ps.Add("OID", workid);
                    else
                        ps.Add("OID", workid);

                    DataTable dt_ND = DBAccess.RunSQLReturnTable(ps);
                    //添加到结果表
                    if (dt_ND.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_ND.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = row[0].ToString();
                            dt.Rows.Add(dr);
                        }
                        //此节点已找到数据则不向下找，继续下个节点
                        continue;
                    }

                    //就要到轨迹表里查,因为有可能是跳过的节点.
                    ps = new Paras();
                    ps.SQL = "SELECT " + TrackAttr.EmpFrom + " FROM ND" + int.Parse(fl.No) + "Track WHERE (ActionType=" + dbStr + "ActionType1 OR ActionType=" + dbStr + "ActionType2 OR ActionType=" + dbStr + "ActionType3 OR ActionType=" + dbStr + "ActionType4 OR ActionType=" + dbStr + "ActionType5) AND NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID";
                    ps.Add("ActionType1", (int)ActionType.Skip);
                    ps.Add("ActionType2", (int)ActionType.Forward);
                    ps.Add("ActionType3", (int)ActionType.ForwardFL);
                    ps.Add("ActionType4", (int)ActionType.ForwardHL);
                    ps.Add("ActionType5", (int)ActionType.Start);

                    ps.Add("NDFrom", int.Parse(nd));
                    ps.Add("WorkID", workid);
                    dt_ND = DBAccess.RunSQLReturnTable(ps);
                    if (dt_ND.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_ND.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = row[0].ToString();
                            dt.Rows.Add(dr);
                        }
                    }
                }

                //本流程里没有有可能该节点是配置的父流程节点,也就是说子流程的一个节点与父流程指定的节点的工作人员一致.
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                if (gwf.PWorkID != 0)
                {
                    foreach (string pnodeiD in ndStrs)
                    {
                        if (DataType.IsNullOrEmpty(pnodeiD))
                            continue;

                        Node nd = new Node(int.Parse(pnodeiD));
                        if (nd.FK_Flow != gwf.PFlowNo)
                            continue; // 如果不是父流程的节点，就不执行.

                        ps = new Paras();
                        ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                        ps.Add("FK_Node", nd.NodeID);
                        if (currNode.IsSubThread == true)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        DataTable dt_PWork = DBAccess.RunSQLReturnTable(ps);
                        if (dt_PWork.Rows.Count != 0)
                        {
                            foreach (DataRow row in dt_PWork.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = row[0].ToString();
                                dt.Rows.Add(dr);
                            }
                            //此节点已找到数据则不向下找，继续下个节点
                            continue;
                        }

                        //就要到轨迹表里查,因为有可能是跳过的节点.
                        ps = new Paras();
                        ps.SQL = "SELECT " + TrackAttr.EmpFrom + " FROM ND" + int.Parse(fl.No) + "Track WHERE (ActionType=" + dbStr + "ActionType1 OR ActionType=" + dbStr + "ActionType2 OR ActionType=" + dbStr + "ActionType3 OR ActionType=" + dbStr + "ActionType4 OR ActionType=" + dbStr + "ActionType5) AND NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID";
                        ps.Add("ActionType1", (int)ActionType.Start);
                        ps.Add("ActionType2", (int)ActionType.Forward);
                        ps.Add("ActionType3", (int)ActionType.ForwardFL);
                        ps.Add("ActionType4", (int)ActionType.ForwardHL);
                        ps.Add("ActionType5", (int)ActionType.Skip);
                        ps.Add("NDFrom", nd.NodeID);

                        if (currNode.IsSubThread == true)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        dt_PWork = DBAccess.RunSQLReturnTable(ps);
                        if (dt_PWork.Rows.Count != 0)
                        {
                            foreach (DataRow row in dt_PWork.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = row[0].ToString();
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                //返回指定节点的处理人
                if (dt.Rows.Count != 0)
                    return dt;

                throw new Exception("@流程设计错误，到达的节点（" + toNode.Name + "）在指定的节点(" + strs + ")中没有数据，无法找到工作的人员。 @技术信息如下: 投递方式:BySpecNodeEmp sql=" + ps.SQLNoPara);
            }
            #endregion 按照节点绑定的人员处理。

            #region 按照上一个节点表单指定字段的人员处理。 
            if (toNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField)
            {
                // 检查接受人员规则,是否符合设计要求.
                string specEmpFields = toNode.DeliveryParas;
                if (DataType.IsNullOrEmpty(specEmpFields))
                    specEmpFields = "SysSendEmps";

                if (enParas.EnMap.Attrs.Contains(specEmpFields) == false)
                    throw new Exception("@您设置的接受人规则是按照表单指定的字段，决定下一步的接受人员，该字段{" + specEmpFields + "}已经删除或者丢失。");

                //获取接受人并格式化接受人, 
                string emps = enParas.GetValStringByKey(specEmpFields);
                emps = emps.Replace(" ", "");
                if (emps.Contains(",") && emps.Contains(";"))
                {
                    /*如果包含,; 例如 zhangsan,张三;lisi,李四;*/
                    string[] myemps1 = emps.Split(';');
                    foreach (string str in myemps1)
                    {
                        if (DataType.IsNullOrEmpty(str))
                            continue;

                        string[] ss = str.Split(',');
                        DataRow dr = dt.NewRow();
                        dr[0] = ss[0];
                        dt.Rows.Add(dr);
                    }
                    if (dt.Rows.Count == 0)
                        throw new Exception("@输入的接受人员信息错误;[" + emps + "]。");
                    else
                        return dt;
                }

                emps = emps.Replace(";", ",");
                emps = emps.Replace("；", ",");
                emps = emps.Replace("，", ",");
                emps = emps.Replace("、", ",");
                emps = emps.Replace("@", ",");

                if (DataType.IsNullOrEmpty(emps))
                    throw new Exception("@没有在字段[" + enParas.EnMap.Attrs.GetAttrByKey(specEmpFields).Desc + "]中指定接受人，工作无法向下发送。");

                // 把它加入接受人员列表中.
                string[] myemps = emps.Split(',');
                foreach (string s in myemps)
                {
                    if (DataType.IsNullOrEmpty(s))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = s;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            #endregion 按照上一个节点表单指定字段的人员处理。


            #region 按照上一个节点表单指定字段的【角色】处理。
            if (toNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormStationsAI
                || toNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormStationsOnly)
            {
                // 检查接受人员规则,是否符合设计要求.
                string specEmpFields = toNode.DeliveryParas;
                if (DataType.IsNullOrEmpty(specEmpFields))
                    specEmpFields = "SysSendEmps";

                if (enParas.EnMap.Attrs.Contains(specEmpFields) == false)
                    throw new Exception("@您设置的接受人规则是按照表单指定的角色字段，决定下一步的接受人员，该字段{" + specEmpFields + "}已经删除或者丢失。");

                string stas = "";
                //获取接受人并格式化接受人, 
                string emps = enParas.GetValStringByKey(specEmpFields);
                emps = emps.Replace(" ", "");
                if (emps.Contains(",") && emps.Contains(";"))
                {
                    /*如果包含,; 例如 xx,角色1;222,角色2;*/
                    string[] myemps1 = emps.Split(';');
                    foreach (string str in myemps1)
                    {
                        if (DataType.IsNullOrEmpty(str))
                            continue;

                        string[] ss = str.Split(',');
                        stas += "," + ss[0];
                    }
                    if (DataType.IsNullOrEmpty(stas))
                        throw new Exception("@输入的接受人员的角色信息错误;[" + emps + "]。");
                }
                else
                {
                    emps = emps.Replace(";", ",");
                    emps = emps.Replace("；", ",");
                    emps = emps.Replace("，", ",");
                    emps = emps.Replace("、", ",");
                    emps = emps.Replace("@", ",");

                    if (DataType.IsNullOrEmpty(emps))
                        throw new Exception("@没有在字段[" + enParas.EnMap.Attrs.GetAttrByKey(specEmpFields).Desc + "]中指定接受人的角色，工作无法向下发送。");

                    // 把它加入接受人员列表中.
                    string[] myemps = emps.Split(',');
                    foreach (string s in myemps)
                    {
                        if (DataType.IsNullOrEmpty(s))
                            continue;
                        stas += "," + s;
                    }
                }
                //根据角色:集合获取信息.
                stas = stas.Substring(1);

                // 仅按角色计算. 以下都有要重写.
                if (toNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormStationsOnly)
                {
                    dt = WorkFlowBuessRole.FindWorker_GetEmpsByStations(stas);
                    if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                        throw new Exception("err@按照字段角色(仅按角色计算)找接受人错误,当前部门下没有您选择的角色人员.");
                    return dt;
                }

                #region 按角色智能计算, 集合模式.
                if (toNode.DeliveryStationReqEmpsWay == 0)
                {
                    string deptNo = BP.Web.WebUser.FK_Dept;
                    dt = FindWorker_GetEmpsByDeptAI(stas, deptNo);
                    if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                        throw new Exception("err@按照字段角色(智能)找接受人错误,当前部门与父级部门下没有您选择的角色人员.");
                    return dt;
                }
                #endregion 按角色智能计算, 要判断切片模式,还是集合模式.

                #region 按角色智能计算, 切片模式. 需要对每个角色都要找到接受人，然后把这些接受人累加起来.
                if (toNode.DeliveryStationReqEmpsWay == 1 || toNode.DeliveryStationReqEmpsWay == 2)
                {
                    string deptNo = BP.Web.WebUser.FK_Dept;
                    string[] temps = stas.Split(',');
                    foreach (string str in temps)
                    {
                        //求一个角色下的人员.
                        DataTable mydt = FindWorker_GetEmpsByDeptAI(str, deptNo);

                        //如果是严谨模式.
                        if (toNode.DeliveryStationReqEmpsWay == 1 && mydt.Rows.Count == 0)
                        {
                            Station st = new Station(str);
                            throw new Exception("@角色["+st.Name+"]下，没有找到人不能发送下去，请检查组织结构是否完整。");
                        }

                        //累加起来.
                        foreach (DataRow dr in mydt.Rows)
                        {
                            DataRow mydr = dt.NewRow();
                            mydr[0] = dr[0].ToString();
                            dt.Rows.Add(mydr);
                        }
                    }

                    if (dt.Rows.Count == 0 && toNode.HisWhenNoWorker == false)
                        throw new Exception("err@按照字段角色(智能,切片)找接受人错误,当前部门与父级部门下没有您选择的角色人员.");

                    return dt;
                }
                #endregion 按角色智能计算, 切片模式.

                throw new Exception("err@没有判断的模式....");
            }
            #endregion 按照上一个节点表单指定字段的人员处理 【角色】。


            #region 按部门与角色的交集计算.
            if (toNode.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
            {

                sql = "SELECT pdes.FK_Emp AS No"
                      + " FROM   Port_DeptEmpStation pdes"
                      + " INNER JOIN WF_NodeDept wnd ON wnd.FK_Dept = pdes.FK_Dept"
                      + " AND wnd.FK_Node = " + toNode.NodeID
                      + " INNER JOIN WF_NodeStation wns ON  wns.FK_Station = pdes.FK_Station"
                      + " AND wns.FK_Node =" + toNode.NodeID
                      + " ORDER BY pdes.FK_Emp";

                dt = DBAccess.RunSQLReturnTable(sql);


                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则(" + toNode.HisDeliveryWay.ToString() + ")错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 按照角色与部门的交集确定接受人的范围错误，没有找到人员:SQL=" + sql);
            }
            #endregion 按部门与角色的交集计算.

            #region 判断节点部门里面是否设置了部门，如果设置了就按照它的部门处理。
            if (toNode.HisDeliveryWay == DeliveryWay.ByDept)
            {
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", workid);
                ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
            }
            #endregion 判断节点部门里面是否设置了部门，如果设置了，就按照它的部门处理。

            #region 仅按用户组计算 
            if (toNode.HisDeliveryWay == DeliveryWay.ByTeamOnly)
            {
                sql = "SELECT DISTINCT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B WHERE A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 仅按用户组计算，没有找到人员:SQL=" + ps.SQLNoPara);
            }
            #endregion

            #region 本集团组织 
            if (toNode.HisDeliveryWay == DeliveryWay.ByTeamOrgOnly)
            {
                sql = "SELECT DISTINCT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B, Port_Emp C WHERE A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node AND C.OrgNo=" + dbStr + "OrgNo  ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("OrgNo", BP.Web.WebUser.OrgNo);

                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 按用户组智能计算，没有找到人员:SQL=" + ps.SQLNoPara);
            }
            #endregion

            #region 本部门 
            if (toNode.HisDeliveryWay == DeliveryWay.ByTeamDeptOnly)
            {
                sql = "SELECT DISTINCT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B, Port_Emp C WHERE A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node AND C.FK_Dept=" + dbStr + "FK_Dept  ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("FK_Dept", BP.Web.WebUser.FK_Dept);

                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 按用户组智能计算，没有找到人员:SQL=" + ps.SQLNoPara);
            }
            #endregion

            #region 仅按角色计算
            if (toNode.HisDeliveryWay == DeliveryWay.ByStationOnly)
            {
                ps = new Paras();
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND A.OrgNo=" + dbStr + "OrgNo AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                    ps.Add("OrgNo", BP.Web.WebUser.OrgNo);
                    ps.Add("FK_Node", toNode.NodeID);
                    ps.SQL = sql;
                }
                else
                {
                    sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                    ps.Add("FK_Node", toNode.NodeID);
                    ps.SQL = sql;
                }

                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 仅按角色计算，没有找到人员。");
            }
            #endregion

            #region 按角色计算(以部门集合为纬度).
            if (toNode.HisDeliveryWay == DeliveryWay.ByStationAndEmpDept)
            {
                /* 考虑当前操作人员的部门, 如果本部门没有这个角色就不向上寻找. */

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    dt = DBAccess.RunSQLReturnTable("SELECT UserID as No, Name FROM Port_Emp WHERE UserID='" + BP.Web.WebUser.No + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'");
                }
                else
                {
                    ps = new Paras();
                    sql = "SELECT No,Name FROM Port_Emp WHERE No=" + dbStr + "FK_Emp ";
                    ps.Add("FK_Emp", WebUser.No);
                    dt = DBAccess.RunSQLReturnTable(ps);
                }



                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则(" + toNode.HisDeliveryWay.ToString() + ")错误:节点(" + toNode.NodeID + "," + toNode.Name + "), 按角色计算(以部门集合为纬度)。技术信息,执行的SQL=" + ps.SQLNoPara);
            }
            #endregion

            string empNo = WebUser.No;
            string empDept = WebUser.FK_Dept;

            #region 按指定的节点的人员角色，做为下一步骤的流程接受人。
            if (toNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmpStation)
            {
                /* 按指定的节点的人员角色 */
                string para = toNode.DeliveryParas;
                para = para.Replace("@", "");

                if (DataType.IsNumStr(para) == true)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node ";
                    ps.Add("OID", workid);
                    ps.Add("FK_Node", int.Parse(para));

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count != 1)
                        throw new Exception("@流程设计错误，到达的节点（" + toNode.Name + "）在指定的节点中没有数据，无法找到工作的人员。");

                    empNo = dt.Rows[0][0].ToString();
                    empDept = dt.Rows[0][1].ToString();
                }
                else
                {
                    if (enParas.Row.ContainsKey(para) == false)
                        throw new Exception("@在找人接收人的时候错误@字段{" + para + "}不包含在rpt里，流程设计错误。");

                    empNo = enParas.GetValStrByKey(para);
                    if (DataType.IsNullOrEmpty(empNo))
                        throw new Exception("@字段{" + para + "}不能为空，没有取出来处理人员。");

                    BP.Port.Emp em = new BP.Port.Emp(empNo);
                    empDept = em.FK_Dept;
                }
            }
            #endregion 按指定的节点人员，做为下一步骤的流程接受人。

            #region 最后判断 - 按照角色AI来执行。
            if (currNode.IsStartNode == false)
            {
                ps = new Paras();
                dt = DBAccess.RunSQLReturnTable(ps);
                // 如果能够找到.
                if (dt.Rows.Count >= 1)
                {
                    if (dt.Rows.Count == 1)
                    {
                        /*如果人员只有一个的情况，说明他可能要 */
                    }
                    return dt;
                }
            }

            /* 如果执行节点 与 接受节点角色集合一致 */
            if (currNode.GroupStaNDs == toNode.GroupStaNDs)
            {
                /* 说明，就把当前人员做为下一个节点处理人。*/
                DataRow dr = dt.NewRow();
                dr[0] = WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            /* 如果执行节点 与 接受节点角色集合不一致 */
            if (currNode.GroupStaNDs != toNode.GroupStaNDs)
            {
                /* 没有查询到的情况下, 先按照本部门计算。*/


                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B  WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept";
                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("FK_Dept", empDept);


                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    NodeStations nextStations = toNode.NodeStations;
                    if (nextStations.Count == 0)
                        throw new Exception("@节点没有角色:" + toNode.NodeID + "  " + toNode.Name);
                }
                else
                {
                    bool isInit = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0].ToString() == BP.Web.WebUser.No)
                        {
                            /* 如果角色分组不一样，并且结果集合里还有当前的人员，就说明了出现了当前操作员，拥有本节点上的角色也拥有下一个节点的工作角色
                             导致：节点的分组不同，传递到同一个人身上。 */
                            isInit = true;
                        }
                    }

#warning edit by zhoupeng, 用来确定不同角色集合的传递包含同一个人的处理方式。

                    //  if (isInit == false || isInit == true)
                    return dt;
                }
            }

            /*这里去掉了向下级别寻找的算法. */


            /* 没有查询到的情况下, 按照最大匹配数 提高一个级别计算，递归算法未完成。
             * 因为:以上已经做的角色的判断，就没有必要在判断其它类型的节点处理了。
             * */
            string nowDeptID = empDept.Clone() as string;
            while (true)
            {
                BP.Port.Dept myDept = new BP.Port.Dept(nowDeptID);
                nowDeptID = myDept.ParentNo;
                if (nowDeptID == "-1" || nowDeptID.ToString() == "0")
                {
                    break; /*一直找到了最高级仍然没有发现，就跳出来循环从当前操作员人部门向下找。*/
                    throw new Exception("@按角色计算没有找到(" + toNode.Name + ")接受人.");
                }

                //检查指定的部门下面是否有该人员.
                DataTable mydtTemp = RequetNextNodeWorkers_DiGui(nowDeptID, empNo, toNode);
                if (mydtTemp == null)
                {
                    /*如果父亲级没有，就找父级的平级. */
                    BP.Port.Depts myDepts = new BP.Port.Depts();
                    myDepts.Retrieve(BP.Port.DeptAttr.ParentNo, myDept.ParentNo);
                    foreach (BP.Port.Dept item in myDepts)
                    {
                        if (item.No == nowDeptID)
                            continue;
                        mydtTemp = RequetNextNodeWorkers_DiGui(item.No, empNo, toNode);
                        if (mydtTemp == null)
                            continue;
                        else
                            return mydtTemp;
                    }

                    continue; /*如果平级也没有，就continue.*/
                }
                else
                    return mydtTemp;
            }

            /* 如果向上找没有找到，就考虑从本级部门上向下找. */
            nowDeptID = empDept.Clone() as string;
            BP.Port.Depts subDepts = new BP.Port.Depts(nowDeptID);

            //递归出来子部门下有该角色的人员.
            DataTable mydt123 = RequetNextNodeWorkers_DiGui_ByDepts(subDepts, empNo, toNode);
            if (mydt123 == null)
                throw new Exception("@按角色计算没有找到(" + toNode.Name + ")接受人.");
            return mydt123;
            #endregion  按照角色来执行。
        }
        /// <summary>
        /// 按照部门编号，与角色集合智能计算接受人.
        /// </summary>
        /// <param name="stas">角色编号</param>
        /// <param name="deptNo">部门编号</param>
        /// <returns></returns>
        public static DataTable FindWorker_GetEmpsByDeptAI(string stas, string deptNo)
        {
            DataTable dt = WorkFlowBuessRole.FindWorker_GetEmpsByStationsAndDepts(stas, deptNo);
            if (dt.Rows.Count == 0)
            {
                //本部门的父级.
                Dept deptMy = new Dept(deptNo);
                dt = WorkFlowBuessRole.FindWorker_GetEmpsByStationsAndDepts(stas, deptMy.ParentNo);

                //本级部门的祖父级,不在向上判断了.
                if (dt.Rows.Count == 0 && deptMy.ParentNo.Equals("0") == false)
                {
                    Dept deptParent = new Dept(deptMy.ParentNo);
                    dt = WorkFlowBuessRole.FindWorker_GetEmpsByStationsAndDepts(stas, deptParent.ParentNo);
                }

                //扫描评级部门.
                if (dt.Rows.Count == 0)
                {
                    string deptNos = "";
                    Depts depts = new Depts();
                    depts.Retrieve(DeptAttr.ParentNo, deptMy.ParentNo);
                    foreach (Dept mydept in depts)
                        deptNos += "," + mydept.No;

                    dt = WorkFlowBuessRole.FindWorker_GetEmpsByStationsAndDepts(stas, deptNos);
                }
                return dt;
            }
            return dt;
        }

        public static DataTable FindWorker_GetEmpsByStations(string stas)
        {
            string sqlEnd = "";
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlEnd = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            //处理合法的 in 字段.
            if (stas.Contains("'") == false)
            {
                string[] temps = stas.Split(',');
                string mystrs = "";
                foreach (string temp in temps)
                    mystrs += ",'" + temp + "'";

                mystrs = mystrs.Substring(1);
                stas = mystrs;
            }

            string sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station IN (" + stas + ") " + sqlEnd;
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 获取部门与角色的交集.
        /// </summary>
        /// <param name="stas">角色集合s</param>
        /// <param name="depts">部门集合s</param>
        /// <returns></returns>
        public static DataTable FindWorker_GetEmpsByStationsAndDepts(string stas, string depts)
        {
            string sqlEnd = "";
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sqlEnd = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";


            //是单个的.
            if (stas.Contains(",") == false && depts.Contains(",") == false)
            {
                string sql1 = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station='" + stas + "' AND FK_Dept='" + depts + "' ";// + sqlEnd;
                return DBAccess.RunSQLReturnTable(sql1);
            }

            //处理合法的 in 字段.
            if (stas.Contains("'") == false)
            {
                string[] temps = stas.Split(',');
                string mystrs = "";
                foreach (string temp in temps)
                    mystrs += ",'" + temp + "'";

                mystrs = mystrs.Substring(1);
                stas = mystrs;
            }

            //处理合法的in 字段.
            if (depts.Contains("'") == false)
            {
                string[] temps = depts.Split(',');
                string mystrs = "";
                foreach (string temp in temps)
                    mystrs += ",'" + temp + "'";

                mystrs = mystrs.Substring(1);
                depts = mystrs;
            }

            string sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station IN(" + stas + ") AND FK_Dept IN (" + depts + ") ";// + sqlEnd;
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 递归出来子部门下有该角色的人员
        /// </summary>
        /// <param name="subDepts"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private static DataTable RequetNextNodeWorkers_DiGui_ByDepts(BP.Port.Depts subDepts, string empNo, Node toNode)
        {
            foreach (BP.Port.Dept item in subDepts)
            {
                DataTable dt = RequetNextNodeWorkers_DiGui(item.No, empNo, toNode);
                if (dt != null)
                    return dt;

                Depts MySubDepts = new Depts();
                MySubDepts.Retrieve(DeptAttr.ParentNo, item.No);

                dt = RequetNextNodeWorkers_DiGui_ByDepts(MySubDepts, empNo, toNode);
                if (dt != null)
                    return dt;
            }
            return null;
        }
        /// <summary>
        /// 根据部门获取下一步的操作员
        /// </summary>
        /// <param name="deptNo"></param>
        /// <param name="emp1"></param>
        /// <returns></returns>
        private static DataTable RequetNextNodeWorkers_DiGui(string deptNo, string empNo, Node toNode)
        {
            string sql;
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept AND A.FK_Emp!=" + dbStr + "FK_Emp";
            Paras ps = new Paras();
            ps.SQL = sql;
            ps.Add("FK_Node", toNode.NodeID);
            ps.Add("FK_Dept", deptNo);
            ps.Add("FK_Emp", empNo);

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                NodeStations nextStations = toNode.NodeStations;
                if (nextStations.Count == 0)
                    throw new Exception("@节点没有角色:" + toNode.NodeID + "  " + toNode.Name);

                sql = "SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE " + BP.Sys.Base.Glo.UserNoWhitOutAS + " IN ";
                sql += "(SELECT  FK_Emp  FROM Port_DeptEmpStation  WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) )";
                sql += " AND " + BP.Sys.Base.Glo.UserNoWhitOutAS + " IN ";

                if (deptNo == "1")
                {
                    sql += "(SELECT " + BP.Sys.Base.Glo.UserNoWhitOutAS + " as FK_Emp FROM Port_Emp WHERE " + BP.Sys.Base.Glo.UserNoWhitOutAS + "!=" + dbStr + "FK_Emp ) ";
                }
                else
                {
                    BP.Port.Dept deptP = new BP.Port.Dept(deptNo);
                    sql += "(SELECT " + BP.Sys.Base.Glo.UserNoWhitOutAS + " as FK_Emp FROM Port_Emp WHERE " + BP.Sys.Base.Glo.UserNoWhitOutAS + "!=" + dbStr + "FK_Emp AND FK_Dept = '" + deptP.ParentNo + "')";
                }

                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("FK_Emp", empNo);
                dt = DBAccess.RunSQLReturnTable(ps);

                if (dt.Rows.Count == 0)
                    return null;
                return dt;
            }
            else
            {
                return dt;
            }
            return null;
        }
        #endregion 找到下一个节点的接受人员

        #region 执行抄送.
        public static string DoCCAuto2022(Node node, GERpt rpt, Int64 workid, Int64 fid, CCRoles rls)
        {

            CC ccEn = new CC(node.NodeID);

            /*如果是自动抄送*/
            foreach (CCRole rl in rls)
            {
                //执行抄送.
                DataTable dt = rl.GenerCCers(rpt, workid);
                if (dt.Rows.Count == 0)
                    return "@设置的抄送规则，没有找到抄送人员。";

                string ccMsg = "@消息自动抄送给";
                string basePath = BP.WF.Glo.HostURL;

                foreach (DataRow dr in dt.Rows)
                {
                    string toUserNo = dr[0].ToString();
                    string toUserName = dr[1].ToString();

                    //生成标题与内容.
                    string ccTitle = ccEn.CCTitle.Clone() as string;
                    ccTitle = BP.WF.Glo.DealExp(ccTitle, rpt);

                    string ccDoc = ccEn.CCDoc.Clone() as string;
                    ccDoc = BP.WF.Glo.DealExp(ccDoc, rpt);

                    ccDoc = ccDoc.Replace("@Accepter", toUserNo);
                    ccTitle = ccTitle.Replace("@Accepter", toUserNo);

                    //抄送信息.
                    ccMsg += "(" + toUserNo + " - " + toUserName + ");";
                    CCList list = new CCList();
                    list.setMyPK(workid + "_" + node.NodeID + "_" + dr[0].ToString());
                    list.FK_Flow = node.FK_Flow;
                    list.FlowName = node.FlowName;
                    list.FK_Node = node.NodeID;
                    list.NodeName = node.Name;
                    list.Title = ccTitle;
                    list.Doc = ccDoc;
                    list.CCTo = dr[0].ToString();
                    list.CCToName = dr[1].ToString();
                    list.RDT = DataType.CurrentDateTime;
                    list.Rec = WebUser.No;
                    list.WorkID = workid;
                    list.FID = fid;

                    // if (this.HisNode.CCWriteTo == CCWriteTo.Todolist)
                    list.InEmpWorks = node.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6

                    //写入待办和写入待办与抄送列表,状态不同
                    if (node.CCWriteTo == CCWriteTo.All || node.CCWriteTo == CCWriteTo.Todolist)
                    {
                        //如果为写入待办则抄送列表中置为已读，原因：只为不提示有未读抄送。
                        //list.HisSta = node.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                        list.HisSta = CCSta.UnRead;
                    }
                    //结束节点只写入抄送列表
                    if (node.IsEndNode == true)
                    {
                        list.HisSta = CCSta.UnRead;
                        list.InEmpWorks = false;
                    }
                    try
                    {
                        list.Insert();
                    }
                    catch
                    {
                        list.Update();
                    }
                    PushMsgs pms = new PushMsgs();
                    pms.Retrieve(PushMsgAttr.FK_Node, node.NodeID, PushMsgAttr.FK_Event, EventListNode.CCAfter);

                    if (pms.Count > 0)
                    {
                        PushMsg pushMsg = pms[0] as PushMsg;
                        //     //写入消息提示.
                        //     ccMsg += list.CCTo + "(" + dr[1].ToString() + ");";
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(list.CCTo);

                        string title = string.Format("工作抄送:{0}.工作:{1},发送人:{2},需您查阅", node.FlowName, node.Name, WebUser.Name);
                        string mytemp = pushMsg.SMSDoc;
                        mytemp = mytemp.Replace("{Title}", title);
                        mytemp = mytemp.Replace("@WebUser.No", WebUser.No);
                        mytemp = mytemp.Replace("@WebUser.Name", WebUser.Name);
                        mytemp = mytemp.Replace("@WorkID", workid.ToString());
                        mytemp = mytemp.Replace("@OID", workid.ToString());

                        /*如果仍然有没有替换下来的变量.*/
                        if (mytemp.Contains("@") == true)
                            mytemp = BP.WF.Glo.DealExp(mytemp, rpt, null);
                        BP.WF.Dev2Interface.Port_SendMsg(wfemp.No, title, mytemp, null, BP.WF.SMSMsgType.CC, list.FK_Flow, list.FK_Node, list.WorkID, list.FID, pushMsg.SMSPushModel);
                    }
                }
            }

            return "抄送执行成功.";
        }
        #endregion 执行抄送.

        #region 执行抄送.
        /// <summary>
        /// 执行抄送.
        /// </summary>
        /// <param name="rpt"></param>
        /// <param name="workid"></param>
        /// <returns></returns>
        public static string DoCCAuto(Node node, GERpt rpt, Int64 workid, Int64 fid)
        {

            if (node.HisCCRole == CCRoleEnum.AutoCC
          || node.HisCCRole == CCRoleEnum.HandAndAuto)
            {

            }
            else
            {
                return "";
            }

            CC ccEn = new CC(node.NodeID);

            /*如果是自动抄送*/

            //执行抄送.
            DataTable dt = ccEn.GenerCCers(rpt, workid);
            if (dt.Rows.Count == 0)
                return "@设置的抄送规则，没有找到抄送人员。";

            string ccMsg = "@消息自动抄送给";
            string basePath = BP.WF.Glo.HostURL;

            foreach (DataRow dr in dt.Rows)
            {
                string toUserNo = dr[0].ToString();
                string toUserName = dr[1].ToString();

                //生成标题与内容.
                string ccTitle = ccEn.CCTitle.Clone() as string;
                ccTitle = BP.WF.Glo.DealExp(ccTitle, rpt);

                string ccDoc = ccEn.CCDoc.Clone() as string;
                ccDoc = BP.WF.Glo.DealExp(ccDoc, rpt);

                ccDoc = ccDoc.Replace("@Accepter", toUserNo);
                ccTitle = ccTitle.Replace("@Accepter", toUserNo);

                //抄送信息.
                ccMsg += "(" + toUserNo + " - " + toUserName + ");";
                CCList list = new CCList();
                list.setMyPK(workid + "_" + node.NodeID + "_" + dr[0].ToString());
                list.FK_Flow = node.FK_Flow;
                list.FlowName = node.FlowName;
                list.FK_Node = node.NodeID;
                list.NodeName = node.Name;
                list.Title = ccTitle;
                list.Doc = ccDoc;
                list.CCTo = dr[0].ToString();
                list.CCToName = dr[1].ToString();
                list.RDT = DataType.CurrentDateTime;
                list.Rec = WebUser.No;
                list.WorkID = workid;
                list.FID = fid;

                // if (this.HisNode.CCWriteTo == CCWriteTo.Todolist)
                list.InEmpWorks = node.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6

                //写入待办和写入待办与抄送列表,状态不同
                if (node.CCWriteTo == CCWriteTo.All || node.CCWriteTo == CCWriteTo.Todolist)
                {
                    //如果为写入待办则抄送列表中置为已读，原因：只为不提示有未读抄送。
                    //list.HisSta = node.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                    list.HisSta = CCSta.UnRead;
                }
                //结束节点只写入抄送列表
                if (node.IsEndNode == true)
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }
                try
                {
                    list.Insert();
                }
                catch
                {
                    list.Update();
                }
                PushMsgs pms = new PushMsgs();
                pms.Retrieve(PushMsgAttr.FK_Node, node.NodeID, PushMsgAttr.FK_Event, EventListNode.CCAfter);

                if (pms.Count > 0)
                {
                    PushMsg pushMsg = pms[0] as PushMsg;
                    //     //写入消息提示.
                    //     ccMsg += list.CCTo + "(" + dr[1].ToString() + ");";
                    BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(list.CCTo);

                    string title = string.Format("工作抄送:{0}.工作:{1},发送人:{2},需您查阅", node.FlowName, node.Name, WebUser.Name);
                    string mytemp = pushMsg.SMSDoc;
                    mytemp = mytemp.Replace("{Title}", title);
                    mytemp = mytemp.Replace("@WebUser.No", WebUser.No);
                    mytemp = mytemp.Replace("@WebUser.Name", WebUser.Name);
                    mytemp = mytemp.Replace("@WorkID", workid.ToString());
                    mytemp = mytemp.Replace("@OID", workid.ToString());

                    /*如果仍然有没有替换下来的变量.*/
                    if (mytemp.Contains("@") == true)
                        mytemp = BP.WF.Glo.DealExp(mytemp, rpt, null);
                    BP.WF.Dev2Interface.Port_SendMsg(wfemp.No, title, mytemp, null, BP.WF.SMSMsgType.CC, list.FK_Flow, list.FK_Node, list.WorkID, list.FID, pushMsg.SMSPushModel);
                }
            }


            //写入日志.

            return ccMsg;
        }
        /// <summary>
        /// 按照指定的字段执行抄送.
        /// </summary>
        /// <param name="nd"></param>
        /// <param name="rptGE"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public static string DoCCByEmps(Node nd, GERpt rptGE, Int64 workid, Int64 fid)
        {
            if (nd.HisCCRole != CCRoleEnum.BySysCCEmps)
                return "";

            CC cc = nd.HisCC;

            //生成标题与内容.
            string ccTitle = cc.CCTitle.Clone() as string;
            ccTitle = BP.WF.Glo.DealExp(ccTitle, rptGE, null);

            string ccDoc = cc.CCDoc.Clone() as string;
            ccDoc = BP.WF.Glo.DealExp(ccDoc, rptGE, null);

            //取出抄送人列表
            string ccers = rptGE.GetValStrByKey("SysCCEmps");
            if (DataType.IsNullOrEmpty(ccers) == true)
                return "";

            string[] cclist = ccers.Split('|');
            Hashtable ht = new Hashtable();
            foreach (string item in cclist)
            {
                string[] tmp = item.Split(',');
                ht.Add(tmp[0], tmp[1]);
            }
            string ccMsg = "@消息自动抄送给";
            string basePath = BP.WF.Glo.HostURL;

            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, nd.NodeID, PushMsgAttr.FK_Event, EventListNode.CCAfter);

            string mailTemp = DataType.ReadTextFile2Html(BP.Difference.SystemConfig.PathOfDataUser + "EmailTemplete/CC_" + WebUser.SysLang + ".txt");
            foreach (DictionaryEntry item in ht)
            {
                ccDoc = ccDoc.Replace("@Accepter", item.Value.ToString());
                ccTitle = ccTitle.Replace("@Accepter", item.Value.ToString());
                //抄送信息.
                ccMsg += "(" + item.Value.ToString() + " - " + item.Value.ToString() + ");";

                #region 如果是写入抄送列表.
                CCList list = new CCList();
                list.setMyPK(DBAccess.GenerGUID());  // workid + "_" + node.NodeID + "_" + item.Key.ToString();
                list.FK_Flow = nd.FK_Flow;
                list.FlowName = nd.FlowName;
                list.FK_Node = nd.NodeID;
                list.NodeName = nd.Name;
                list.Title = ccTitle;
                list.Doc = ccDoc;
                list.CCTo = item.Key.ToString();
                list.CCToName = item.Value.ToString();
                list.RDT = DataType.CurrentDateTime;
                list.Rec = WebUser.No;
                list.WorkID = workid;
                list.FID = fid;
                list.InEmpWorks = nd.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
                //写入待办和写入待办与抄送列表,状态不同
                if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                {
                    //如果为写入待办则抄送列表中置为已读，原因：只为不提示有未读抄送。
                    list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                }
                //如果为结束节点，只写入抄送列表
                if (nd.IsEndNode == true)
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }

                //执行保存或更新
                try
                {
                    list.Insert();
                }
                catch
                {
                    list.CheckPhysicsTable();
                    list.Update();
                }
                #endregion 如果要写入抄送

                #region 写入消息机制.


                if (pms.Count > 0)
                {
                    ccMsg += list.CCTo + "(" + item.Value.ToString() + ");";
                    BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(list.CCTo);

                    string sid = list.CCTo + "_" + list.WorkID + "_" + list.FK_Node + "_" + list.RDT;
                    string url = basePath + "WF/Do.htm?DoType=DoOpenCC&Token=" + sid;
                    url = url.Replace("//", "/");
                    url = url.Replace("//", "/");

                    string urlWap = basePath + "WF/Do.htm?DoType=DoOpenCC&Token=" + sid + "&IsWap=1";
                    urlWap = urlWap.Replace("//", "/");
                    urlWap = urlWap.Replace("//", "/");

                    string mytemp = mailTemp.Clone() as string;
                    mytemp = string.Format(mytemp, wfemp.Name, WebUser.Name, url, urlWap);

                    string title = string.Format("工作抄送:{0}.工作:{1},发送人:{2},需您查阅", nd.FlowName, nd.Name, WebUser.Name);

                    BP.WF.Dev2Interface.Port_SendMsg(wfemp.No, title, mytemp, null, BP.WF.SMSMsgType.CC, list.FK_Flow, list.FK_Node, list.WorkID, list.FID, ((PushMsg)pms[0]).SMSPushModel);
                }
                #endregion 写入消息机制.
            }

            return ccMsg;
        }
        #endregion 执行抄送.


    }

}
