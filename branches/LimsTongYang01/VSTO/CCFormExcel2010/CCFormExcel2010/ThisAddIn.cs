using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using BP.VSTO;
using System.Management;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace CCFormExcel2010
{
    public partial class ThisAddIn
    {
        #region 全局变量
        private BP.VSTO.CCForm.CCFormAPISoapClient client;
        /// <summary>
        /// Excel表单基础功能类
        /// </summary>
        private ExcelFormBase _base;
        /// <summary>
        /// 原始数据(包括表单的描述、业务逻辑、主表子表数据。)
        /// </summary>
        private DataSet _originData;
        /// <summary>
        /// 子表（[{(string)DtlName:(SubTable)Dtl},...]）
        /// </summary>
        private Dictionary<string, SubTable> _dictSubTables = new Dictionary<string, SubTable>(); //excel中的子表信息
        private bool _ignoreOneTime = false; //用于【在代码中修改了值】时，忽略一次【SheetChange】事件.
        private bool IsDebug = false; //是否是调试模式.
        public string TestUrl = "excelform://-fromccflow,App=FrmExcel,DoType=Frm_Init,FK_MapData=FX_JNHBG_64_34A,IsEdit=1,IsPrint=0,WorkID=4415,FK_Flow=003,FK_Node=301,UserNo=huangwei,FID=0,SID=0dqhkuiavcbgygu5q0fa2d41,PWorkID=0,IsLoadData=1,PFlowNo=,Frms=FX_JNHBG_64_34A,IsCheckGuide=1,e1m=0.30355243844447577,WSUrl=http://localhost:8003/WF/CCForm/CCFormAPI.asmx";
        #endregion

        #region 测试用代码
        public Dictionary<string, string> InitTesterArgsString()
        {
            string argstr = TestUrl;
            string prefix = "-fromccflow,";
            int beginidx = -1;
            Dictionary<string, string> args = new Dictionary<string, string>();

            beginidx = argstr.IndexOf(prefix);
            if (beginidx == -1 || (beginidx + prefix.Length) == argstr.Length - 1)
            {
                args.Add("fromccflow", "false");
                return args;
            }

            beginidx = beginidx + prefix.Length;
            argstr = argstr.Substring(beginidx);

            if (argstr.IndexOf(' ') != -1)
                argstr = argstr.Substring(0, argstr.IndexOf(' '));

            Glo.AtParas = "@" + argstr.Replace(",", "@");
            string[] argsArr = argstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] ars = null;

            args.Add("fromccflow", "true");

            foreach (string arg in argsArr)
            {
                ars = arg.Split('=');

                if (ars.Length == 1)
                    continue;

                args.Add(ars[0], ars[1]);
            }

            return args;
        }
        #endregion

        /// <summary>
        /// 获得外部参数, 这是通过外部传递过来的参数.
        /// </summary>
        /// <returns></returns>
        private void GetArgs()
        {
            #region 获得外部参数, 这是通过外部传递过来的参数.
            try
            {
                Dictionary<string, string> args = null;

                //生成参数. 
                if (IsDebug == true)
                    args = InitTesterArgsString();
                else
                    args = Glo.GetArguments();

                //Dictionary<string, string> args = InitTesterArgsString();
                if (args.ContainsKey("fromccflow"))
                {
                    Glo.LoadSuccessful = args["fromccflow"] == "true";
                }

                //若插件没有加载成功：直接跳出
                if (Glo.LoadSuccessful == false)
                    return;

                if (args.ContainsKey("App"))
                    Glo.App = args["App"];
                else
                    throw new Exception("缺少参数: App");

                //是否只读：只加载Excel文件本身，不取数据不填充
                if (args.ContainsKey("IsReadonly"))
                    Glo.IsReadonly = args["IsReadonly"] == "1";
                else if (args.ContainsKey("IsEdit"))
                    Glo.IsReadonly = args["IsEdit"] == "0"; //Globals.Ribbons.RibbonCCFlow.btnSaveFrm.Enabled = true;

                //是否只保存文件：加载文件、填充数据，保存文件，但不保存表单数据
                if (args.ContainsKey("IsSaveFileOnly"))
                    Glo.IsSaveFileOnly = args["IsSaveFileOnly"] == "1";

                //身份认证相关
                if (args.ContainsKey("UserNo"))
                    Glo.UserNo = args["UserNo"];
                else
                    throw new Exception("缺少参数: UserNo");
                if (args.ContainsKey("SID"))
                    Glo.SID = args["SID"];
                else
                    throw new Exception("缺少参数: SID");

                if (args.ContainsKey("FK_MapData")) //若为表单
                {
                    Glo.eType = ExcelType.Form;
                    Glo.FrmID = args["FK_MapData"];//FrmID

                    if (args.ContainsKey("WorkID"))
                        //Glo.WorkID = int.Parse(args["WorkID"]);
                        Glo.pkValue = args["WorkID"];
                    else
                        throw new Exception("缺少参数: WorkID");

                    //2017-06-28：下面3个参数暂时没用到，不做验证
                    if (args.ContainsKey("FK_Flow"))
                        Glo.FK_Flow = args["FK_Flow"];
                    //else
                    //	throw new Exception("缺少参数: FK_Flow");

                    if (args.ContainsKey("FK_Node"))
                        Glo.FK_Node = int.Parse(args["FK_Node"]);
                    //else
                    //	throw new Exception("缺少参数: FK_Node");

                    if (args.ContainsKey("PWorkID"))
                        Glo.PWorkID = int.Parse(args["PWorkID"]);
                    //else
                    //	throw new Exception("缺少参数: PWorkID");
                }
                else if (args.ContainsKey("EnName")) //若为类
                {
                    Glo.eType = ExcelType.Entity;
                    Glo.FrmID = args["EnName"];

                    if (args.ContainsKey("MyPK"))
                        Glo.pkValue = args["MyPK"];
                    else
                        throw new Exception("缺少参数: MyPK");

                    if (args.ContainsKey("UseSheet"))
                        Glo.UseSheet = Uri.UnescapeDataString(args["UseSheet"]);
                }
                else
                    throw new Exception("缺少参数: FK_MapData/EnName");

                if (args.ContainsKey("IsAutoTesting"))
                    Glo.IsAutoTesting = string.IsNullOrWhiteSpace(args["IsAutoTesting"])
                                            ? false
                                            : args["IsAutoTesting"] == "1";

                if (args.ContainsKey("WSUrl"))
                    Glo.WSUrl = args["WSUrl"];
                else
                    throw new Exception("缺少参数: WSUrl");

                //Glo.LocalFile = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Glo.FrmID + "_" + Glo.WorkID + ".xlsx";
                Glo.LocalFile = System.Environment.GetEnvironmentVariable("TEMP") + "\\ccform.xlsx";
            }
            catch (Exception ex)
            {
                Glo.LoadSuccessful = false;
                MessageBox.Show("加载期间出现错误：\n" + ex.Message);
            }
            #endregion 获得外部参数, 这是通过外部传递过来的参数.
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
#if DEBUG
			this.IsDebug = true;
			//MessageBox.Show(Application.Version);//14.0
#endif

            //获得外部参数
            GetArgs();

            //若插件没有加载成功：直接跳出
            if (Glo.LoadSuccessful == false)
                return;

            //Office版本判断
            if (Application.Version == "12.0")
            {
                MessageBox.Show("@检测到您正在使用Excel 2007，该版本与本CCFlow插件存在兼容性问题，请升级至2010或更新版本！", "不兼容支持提示.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Glo.LoadSuccessful = false;
                return;
            }

            if (Application.Version == "15.0")
            {
                MessageBox.Show("@检测到您正在使用Excel 2013，但您当前安装的CCFlow插件为Excel 2010专版，继续使用可能会出现问题，推荐您安装Excel 2013专版插件。", "插件版本提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //// 测试当前数据.
            //this.InitTester();

            //init base class
            _base = new ExcelFormBase(Application);

            #region 校验用户安全与下载文件.
            try
            {
                client = Glo.GetCCFormAPISoapClient();

                //检查插件版本
                var serverVersion = client.GetVstoExtensionVersion();
                if (serverVersion != Glo.GetCurrentVersion())
                {
                    DialogResult result = MessageBox.Show("\t\n检测到有新版本插件！t\n当前版本为:[" + Glo.GetCurrentVersion() + "],最新版本为[" + serverVersion + "].  \t\n点击“确定”后自动下载新版安装包，请卸载旧版后安装新版插件。\t\n【详情参阅新版安装包中的操作手册】",
                        "更新提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Glo.WSUrl.Substring(0, Glo.WSUrl.IndexOf("/WF")) + "/DataUser/FrmOfficeTemplate/Excel表单插件安装程序.zip");
                        Glo.LoadSuccessful = false;
                        return;
                    }
                }

                byte[] bytes = null;
                var isExistDbFile = client.GenerExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.pkValue, ref bytes);

                // 把这个byt 保存到本地.
                if (System.IO.File.Exists(Glo.LocalFile) == true)
                    System.IO.File.Delete(Glo.LocalFile);
                //写入文件.
                Glo.WriteFile(Glo.LocalFile, bytes);

                //打开文件
                Globals.ThisAddIn.Application.Workbooks.Open(Glo.LocalFile, ReadOnly: Glo.IsReadonly);

                //如果是只读，则不再往下执行
                if (Glo.IsReadonly)
                    return;

                //获得该表单的，物理数据.
                _originData = client.GenerDBForVSTOExcelFrmModel(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.pkValue, Glo.AtParas);

                //如果打开的是模板，则还需填充数据
                if (isExistDbFile == false)
                {
                    //设置启用的Sheet页
                    SetUseSheet();

                    //加载外键枚举数据.
                    SetMetaData(_originData);
                    FillData(_originData);
                }
                else //xTODO: 如果打开的是DBFile二进制流，是否还执行填充操作？（表单数据是否有可能被修改？）//A:暂时不考虑这种情况，按数据库数据与Excel数据完全一致处理
                {
                    FillData(_originData);
                }

                //增加自动测试逻辑，2017-9-8
                if (Glo.IsAutoTesting)
                {
                    Globals.ThisAddIn.Application.ActiveWorkbook.Save();
                    Globals.ThisAddIn.Application.Quit();
                }
            }
            catch (Exception exp)
            {
                Glo.LoadSuccessful = false;
                if (exp.Message.IndexOf("another process") > -1)
                    MessageBox.Show("不允许同时打开多个表单（或重复打开表单）！\n若首次打开表单时遇此提示，请结束所有Excel进程后重试。\n\n后续不会记录任何操作，请关闭本文档。", "装载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("\n Excel表单出现错误，请联系您的系统管理员！\n错误信息：\n" + exp.Message + "\n\n后续不会记录任何操作，请关闭本文档。", "Excel表单配置错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion 校验用户安全与下载文件.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtlName"></param>
        /// <returns></returns>
        public string GetDtlNameByTableName(string dtlNo)
        {
            DataTable dtl = _originData.Tables["Sys_MapDtl"];

            foreach (DataRow dr in dtl.Rows)
            {
                if (dr["No"].ToString() == dtlNo)
                    return dr["Name"].ToString();
            }

            return dtlNo;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="originData">从服务端获取的数据</param>
        /// <returns></returns>
        private bool FillData(DataSet originData, bool isFirstFill = true)
        {
            //if(isFirstFill)
            foreach (DataTable dt in originData.Tables)
            {
                if (dt.TableName == "MainTable")
                {
                    //给主表赋值.
                    SetMainData(dt);
                    continue;
                }

                //给从表赋值.
                SetDtlData(dt);
            }
            //else
            //{
            //	foreach (KeyValuePair<string, SubTable> st in _dictSubTables)
            //	{
            //		var stnew = st.Value;
            //		if (_originData.Tables.Contains(stnew.Name))
            //		{
            //			stnew.OriginData = _originData.Tables[stnew.Name].Copy();
            //			stnew.Data = _originData.Tables[stnew.Name].Copy();
            //			stnew.InitConnection();
            //		}
            //	}
            //}

            return true;
        }

        /// <summary>
        /// 单元格内容变动监听事件
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="range"></param>
        void Application_SheetChange(object sh, Excel.Range range)
        {
            Show(range.Address);

            //若插件没有加载成功：直接跳出
            if (!Glo.LoadSuccessful) return;

            if (_ignoreOneTime)
            {
                _ignoreOneTime = false;
                return;
            }

            if (_base.IsSingle(range)) //是单个单元格
            {
                var strBelongDtlName = _base.GetBelongDtlName(range);
                if (strBelongDtlName == null) //单元格不属于某一命名区域
                {
                    try
                    {
                        if (range.Name == null) //也不属于主表
                            return;
                    }
                    catch
                    {
                        return;
                    }

                    //↓是主表字段
                    if (_base.IsValidList(range)) //是下拉
                    {
                        //?是否增加判断：值是否匹配数据有效性规则？
                        #region 设置级联的下拉
                        string listName;
                        var type = GetFieldType("MainTable", range.Name.Name, out listName);
                        if (type == FieldType.CascadeParentList) //是级联的父级字段
                        {
                            var drs = _originData.Tables["Sys_MapExt"].Select("AttrOfOper='" + range.Name.Name + "'");
                            foreach (DataRow dr in drs) //可能有多个子级？
                            {
                                //填充数据（序列）

                                //0.获取联动字段
                                if (!_base.IsExistsName(dr["AttrsOfActive"].ToString()))
                                    continue;
                                Excel.Range rangeAim = Application.Names.Item(dr["AttrsOfActive"].ToString()).RefersToRange;
                                if (!_base.IsValidList(rangeAim)) //如果联动目标单元格不是下拉列表类型
                                    continue;

                                //1.获取联动字段的序列区域的命名
                                var strListAreaName = rangeAim.Validation.Formula1.Replace("=", "");
                                if (!_base.IsExistsName(strListAreaName)) //若Workbook中没有该区域
                                    continue;
                                //x2.清除原序列 //用新序列覆盖即可

                                //3.填充新序列
                                var val = GetSaveValue("MainTable", range.Name.Name, range, fieldType: type, listName: listName); //获取当前单元格的值(key)
                                //获取新序列
                                var dt = client.MapExtGenerAcitviDDLDataTable(Glo.UserNo, Glo.SID, Glo.pkValue, dr["MyPK"].ToString(),
                                    val,
                                    GetMainData());
                                UpdateMetaList(strListAreaName, dt);

                                //清除关联的子级的值
                                IgnoreNextOperation();
                                rangeAim.Value2 = null;
                            }
                        }
                        #endregion
                    }
                    //不是下拉//不是级联-父级：终止执行
                }
                else //单元格在某区域内
                {
                    if (!_dictSubTables.ContainsKey(strBelongDtlName)) //单元格所在区域不是子表（可能是MetaData或？）
                        return;

                    //↓属于某子表时
                    var subTable = _dictSubTables[strBelongDtlName];
                    //操作的单元格位于子表表头：
                    if (range.Row >= subTable.Range.Row &&
                        range.Row <= subTable.Range.Row + subTable.TableHeadHeight - 1)
                    {
                        IgnoreNextOperation();
                        Application.Undo();
                        return;
                    }

                    //↓不是表头时：
                    var colName = subTable.GetColumnName(range);
                    if (colName == null) //单元格无绑定列
                        return;
                    //↓有绑定列时：
                    #region 设置级联的下拉 copy from
                    string listName;
                    var type = GetFieldType(strBelongDtlName, colName, out listName);
                    if (type == FieldType.CascadeParentList) //是级联的父级字段
                    {
                        var drs = _originData.Tables["Sys_MapExt_For_" + strBelongDtlName].Select("AttrOfOper='" + range.Name.Name + "'");
                        foreach (DataRow dr in drs) //可能有多个子级？
                        {
                            //填充数据（序列）

                            //0.获取联动字段
                            if (!_base.IsExistsName(dr["AttrsOfActive"].ToString()))
                                continue;
                            var sonColumnIdx = subTable.GetColumnCx(colName);
                            if (sonColumnIdx == -1) //不可能出现此情况
                                continue;
                            Excel.Range rangeAim = range.Worksheet.get_Range(_base.ConvertInt2Letter(sonColumnIdx) + range.Row, missing);
                            if (!_base.IsValidList(rangeAim)) //如果联动目标单元格不是下拉列表类型
                                continue;

                            //1.获取联动字段的序列区域的命名
                            var strListAreaName = rangeAim.Validation.Formula1.Replace("=", "");
                            if (!_base.IsExistsName(strListAreaName)) //若Workbook中没有该区域
                                continue;
                            //x2.清除原序列 //用新序列覆盖即可

                            //3.填充新序列
                            var val = GetSaveValue(strBelongDtlName, colName, range, fieldType: type, listName: listName); //获取当前单元格的值(key)
                            //获取新序列
                            var dt = client.MapExtGenerAcitviDDLDataTable(Glo.UserNo, Glo.SID, Glo.pkValue, dr["MyPK"].ToString(),
                                val,
                                GetMainData());
                            UpdateMetaList(strListAreaName, dt);

                            //清除关联的子级的值
                            IgnoreNextOperation();
                            rangeAim.Value2 = null;
                        }
                    }
                    #endregion
                }
            }
            else //不是单个单元格（是区域）
            {
                if (Regex.IsMatch(range.Address, _base.regexAddressRows)) //若是针对『整行』的操作
                {
                    //MessageBox.Show(range.ID);
                    //监听“插入行”“删除行”操作
                    //判断操作行数
                    //x判断各子表Range是否有变化:已保存到SubTable中的range会实时变化
                    //var haveChange = false;
                    //foreach (KeyValuePair<string, SubTable> st in _dictSubTables)
                    //{
                    //    if (_base.IsRowInRange(range.Row, range.Row + range.Rows.Count - 1, st.Value.Range.Row, st.Value.Range.Row + st.Value.RangeRows - 1)) //_base.IsIntersect(range, st.Value.Range)!若插入/删除的行在子表中，则此时SubTable.Range已经变化
                    //    {
                    //        if (range.Rows.Count > 1) //若是针对多行操作，不允许同时删除多行
                    //        {
                    //            IgnoreNextOperation();
                    //            Application.Undo();
                    //            return;
                    //        }

                    //        if (st.Value.IsInsertRow)//如果是插入行
                    //        {
                    //            st.Value.InsertRow(range.Row);
                    //            haveChange = true;
                    //        }
                    //        else if (st.Value.IsDeteteRow) //如果是删除行
                    //        {
                    //            st.Value.DeleteRow(range.Row);
                    //            haveChange = true;
                    //        }

                    //        //如果是插入行（删除行操作的撤销？）
                    //        //1.set Connection
                    //        //2.处理新行的下拉字段
                    //        //2.1级联子级字段
                    //        //2.1.1下方字段的 list area's name 更改 
                    //        //2.1.2下方字段的 Validation.f1 更改
                    //        //2.1.3本行字段的 list area 填充、命名
                    //        //2.1.4本行字段的 Validation 设置
                    //        //2.2不是级联子级的下拉类型字段
                    //        //2.2.1本行字段的 Validation 设置
                    //        //如果是删除行（插入行操作的撤销？）
                    //    }

                    //    st.Value.RefreshConnection();
                    //}

                    ////↓插入/删除的行不在任何子表中的情况
                    ////TODO: 受影响的子表行数顺延（）
                    ////if()

                    //if (!haveChange)
                    //{
                    //    IgnoreNextOperation();
                    //    Application.Undo();
                    //}

                    return;//暂时不做处理
                }
                else if (Regex.IsMatch(range.Address, _base.regexAddressColumns)) //若是对『整列』的操作
                {
                    //如果操作涉及到了某个子表，则Undo该操作
                    foreach (KeyValuePair<string, SubTable> st in _dictSubTables)
                    {
                        if (_base.IsIntersect(range, st.Value.Range)) //!若插入/删除的列在子表中，则此时SubTable.Range已经变化
                        {
                            IgnoreNextOperation();
                            Application.Undo();
                            return;
                        }
                    }
                }
                else
                {
                    //判断操作对象是否是某个子表的“整行”
                    //+无须对此情况做特殊处理：若是整行删除，保存时会作为删除行；若再填入值，则会作为修改行处理。

                    //TODO: 若为子表跨行合并的单元格，可能需要触发被合并单元格所在行的关联变更，例如下拉联动等
                }
            }
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="Wb"></param>
        /// <param name="Success"></param>
        void Application_WorkbookAfterSave(Excel.Workbook Wb, bool Success)
        {
            //若插件没有加载成功：直接跳出
            if (Glo.LoadSuccessful == false)
                return;

            //Excel文件流
            byte[] bytes = Glo.ReadFile(Glo.LocalFile);
            try
            {
                if (Glo.IsSaveFileOnly && Glo.IsAutoTesting == false)
                {
                    client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.pkValue, null, null, null, bytes);
                    MessageBox.Show("文件保存成功！", "ccform保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //主表字段
                //DataSet dsDtls = new DataSet();
                string mainTableAtParas = GetMainData();//ref dsDtls);
                if (string.IsNullOrEmpty(mainTableAtParas))
                    return;

                //子表字段
                DataSet dsDtlsOld = new DataSet();
                DataSet dsDtlsNew = GetDtls(dsOld: dsDtlsOld);
                if (dsDtlsNew == null)
                    return;

                //保存到服务器
                client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.pkValue, mainTableAtParas, dsDtlsNew, dsDtlsOld, bytes); //?能否返回保存结果（成功/失败）？A:暂不考虑@2017-03-01

                if (Glo.IsAutoTesting == false)
                    MessageBox.Show("保存成功！\n文档及表单数据已成功保存到服务器！", "ccform保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //获取新的子表数据，绑定行对应关系
                _originData = client.GenerDBForVSTOExcelFrmModel(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.pkValue, Glo.AtParas);
                ////保存成功后用『新数据』替换『旧数据』
                //foreach (KeyValuePair<string, SubTable> st in _dictSubTables)
                //{
                //	var stnew = st.Value;
                //	if (_originData.Tables.Contains(stnew.Name))
                //	{
                //		stnew.OriginData = _originData.Tables[stnew.Name].Copy();
                //		stnew.Data = _originData.Tables[stnew.Name].Copy();
                //		stnew.InitConnection();
                //	}
                //}
                FillData(_originData);
            }
            catch (Exception exp)
            {
                MessageBox.Show("保存失败！！\n错误信息：" + exp.Message + "\n请联系您的系统管理员！",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// 设置启用的Sheet页
        /// </summary>
        /// <returns></returns>
        public bool SetUseSheet()
        {
            if (Glo.UseSheet == null) //未收到“启用Sheet”参数
                return false;

            var ws = _base.GetWorksheet(Glo.UseSheet);
            if (ws == null) //excel中没有该Sheet页
            {
                throw new Exception("没有找到名为“" + Glo.UseSheet + "”的Sheet页，请检查【UseSheet】参数，或联系您的系统管理员！");
            }

            ws.Move(Before: _base.GetFirstWorkSheet());

            //遍历命名
            foreach (Excel.Name name in Application.Names)
            {
                if (name.NameLocal.IndexOf(Glo.UseSheet + ".") == 0)
                {
                    name.Name = name.NameLocal.Replace(Glo.UseSheet + ".", string.Empty);
                }
            }
            return true;
        }

        /// <summary>
        /// 忽略下次操作（不做用户操作限制）
        /// </summary>
        public void IgnoreNextOperation()
        {
            _ignoreOneTime = true;
        }

        /// <summary>
        /// 填充枚举、外键元数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public bool SetMetaData(DataSet ds)
        {
            var ws = _base.GetWorksheet("MetaData"); //只遍历Sheet(MetaData)内的名称
            if (ws == null)
            {
                //MessageBox.Show("不存在Sheet页：MetaData");
                //创建Sheet(MetaData):
                Excel.Worksheet wsheet = Application.Sheets.Add();
                wsheet.Name = "MetaData";
                //wsheet.Visible = _isDebug ? Excel.XlSheetVisibility.xlSheetVisible : Excel.XlSheetVisibility.xlSheetVeryHidden;
                wsheet.Visible = IsDebug ? Excel.XlSheetVisibility.xlSheetVisible : Excel.XlSheetVisibility.xlSheetHidden;//TODO：测试期间不强制隐藏metadata
                return false;
            }

            //ws.Visible = _isDebug ? Excel.XlSheetVisibility.xlSheetVisible : Excel.XlSheetVisibility.xlSheetVeryHidden;
            ws.Visible = IsDebug ? Excel.XlSheetVisibility.xlSheetVisible : Excel.XlSheetVisibility.xlSheetHidden;//TODO：测试期间不强制隐藏metadata

            //遍历命名区域.
            foreach (Excel.Name name in Application.Names)
            {
                this.UpdateMetaList(name.RefersToRange, name.NameLocal);
            }
            return true;
        }

        /// <summary>
        /// 填充表单数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool SetMainData(DataTable dt) //x尚未测试
        {
            //var r = false;
            foreach (DataColumn dc in dt.Columns)
            {
                if (dt.Rows[0][dc.ColumnName] != null && _base.IsExistsName(dc.ColumnName))//字段值不为空 且 Excel文档中存在此字段的命名
                {
                    var range = Application.Names.Item(dc.ColumnName).RefersToRange;
                    var location = Application.Names.Item(dc.ColumnName).RefersToLocal; //=Sheet1!$B$2
                    if (Regex.IsMatch(location, _base.regexRangeSingle)) //是单个单元格
                    {
                        IgnoreNextOperation();
                        range.Value2 = GetDisplayValue(tableName: "MainTable", keyOfEn: dc.ColumnName, value: dt.Rows[0][dc.ColumnName].ToString(), rangeCell: range);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 填充子表数据
        /// </summary>
        /// <param name="dt">确保TableName为子表表名</param>
        /// <returns>是否填充成功</returns>
        public bool SetDtlData(DataTable dt, bool isFill = true) //x尚未测试
        {
            #region 排除不是子表的情况

            if (_base.IsExistsName(dt.TableName) == false) //excel中不存在该子表区域时
                return false;
            var location = Application.Names.Item(dt.TableName).RefersToLocal;
            if (Regex.IsMatch(location, _base.regexRangeArea) == false) //excel中子表所在区域不是『区域』（是单个单元格）
                return false;
            if (location.IndexOf("=MetaData!") > -1) //若是元数据list区域.
                return false;

            #endregion

            var range = Application.Names.Item(dt.TableName).RefersToRange;
            //排序
            if (dt.Columns.Contains("Idx"))
            {
                dt.DefaultView.Sort = "Idx ASC";
                dt = dt.DefaultView.ToTable();
            }

            #region 获取子表

            SubTable st = null;
            if (_dictSubTables.ContainsKey(dt.TableName))
            {
                st = _dictSubTables[dt.TableName];
                if (st.Data != _originData.Tables[st.Name]) //用于【保存成功后，更新最新数据源】
                {
                    //st.OriginData = _originData.Tables[st.Name].Copy();
                    //st.Data = _originData.Tables[st.Name].Copy();
                    st.RefreshData(_originData.Tables[st.Name].Copy(), range);
                }
            }
            else
            {
                Dictionary<int, string> htColumns = new Dictionary<int, string>();
                int TableHeadHeight;
                htColumns = GetAreaColumns(range, out TableHeadHeight);

                string errInfs = "";
                foreach (KeyValuePair<int, string> col in htColumns)
                {
                    if (dt.Columns.Contains(col.Value) == false) //若数据源表不含此字段
                    {
                        //则添加
                        //dr.Table.Columns.Add((string)col.Value);
                        errInfs += col.Value + ",";
                        //throw new Exception("检测到字段绑定异常：子表“" + dt.TableName + "”绑定了『不存在于原始数据表』的字段“" + col.Value + "”！");
                    }
                }

                if (errInfs != "")
                {
                    //求出该子表的列名.
                    string attrs = "";
                    foreach (DataColumn dc in dt.Columns)
                        attrs += "," + dc.ColumnName;
                    attrs = attrs.Substring(1);

                    throw new Exception("@检测到字段绑定异常：子表名:" + this.GetDtlNameByTableName(dt.TableName) + ",ID=" + dt.TableName + ". 绑定了『不存在于原始数据表』的字段[" + errInfs + "]！\t\n@该子表的字段集合是[" + attrs + "]");
                }

                if (dt.Columns.Contains("OID") == true)
                {
                    st = new SubTable(range, dt, htColumns, TableHeadHeight, "OID"); //xTODO: 暂定所有子表的主键字段都是“OID”
                    _dictSubTables.Add(dt.TableName, st);
                }

                if (dt.Columns.Contains("MyPK") == true)
                {
                    st = new SubTable(range, dt, htColumns, TableHeadHeight, "MyPK");
                    _dictSubTables.Add(dt.TableName, st);
                }

                if (dt.Columns.Contains("No") == true)
                {
                    st = new SubTable(range, dt, htColumns, TableHeadHeight, "No");
                    _dictSubTables.Add(dt.TableName, st);
                }
            }

            #endregion

            #region 若子表区域行数不够，则插入行

            if (dt.Rows.Count > range.Rows.Count - st.TableHeadHeight) //表单实际数据中『子表行数』多于excel文档子表『区域行数』时
            {
                //插入行（在区域最后一行上方） //已验证：Q:插入行后range的范围是否扩大了？（后面要用到range）A:手动插入行时扩大了；
                int addRowsCount = dt.Rows.Count - (range.Rows.Count - st.TableHeadHeight);
                int currentRowIdx = range.Row + range.Rows.Count - 1;
                //var rangeLastRow = range.Worksheet.get_Range(_base.ConvertInt2Letter(range.Column) + (range.Row + range.Rows.Count - 1), missing);
                //var rangeLastRow = range.Worksheet.get_Range((range.Row + range.Rows.Count - 1), missing);//"$" + 
                var rangeLastRow = range.Worksheet.get_Range("$" + (range.Row + range.Rows.Count - 1).ToString() + ":$" + (range.Row + range.Rows.Count - 1).ToString(), missing);

                while (addRowsCount > 0)
                {
                    IgnoreNextOperation();
                    rangeLastRow.Insert();

                    //复制行格式
                    IgnoreNextOperation();
                    rangeLastRow.Copy();
                    IgnoreNextOperation();
                    var newRow = range.Worksheet.get_Range("$" + (rangeLastRow.Row - 1).ToString() + ":$" + (rangeLastRow.Row - 1).ToString(), missing);
                    newRow.PasteSpecial(Microsoft.Office.Interop.Excel.XlPasteType.xlPasteFormats,
                                Microsoft.Office.Interop.Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone,
                                missing, missing);

                    addRowsCount--;
                }
            }

            //清空明细表数据
            for (var r = range.Row + st.TableHeadHeight + dt.Rows.Count; r < range.Row + range.Rows.Count; r++)
            {
                for (var c = range.Column; c < range.Column + range.Columns.Count; c++)
                {
                    range.Worksheet.Cells[r, c].Value2 = string.Empty;
                }
                //如果区域内每行第一个单元格存在ID，则清空
                range.Worksheet.Cells[r, range.Column].ID = string.Empty;
            }

            Application.CutCopyMode = 0; //离开复制模式

            #endregion

            #region 填充数据
            if (isFill)
            {
                foreach (KeyValuePair<int, string> col in st.Columns)
                {
                    //xif (col.Key == "TableHeadHeight") continue;

                    if (!st.Data.Columns.Contains(col.Value)) continue; //DataTable中不含该字段时

                    #region 为子表的每一行的DropdownList类型的字段添加 Validation

                    var colL = _base.ConvertInt2Letter(col.Key);

                    //（分两种情况：作为级联的子级；有范围限制的外键/枚举） //另：验证插入行是否能自动添加上 Validation
                    var strListName = string.Empty;
                    FieldType ftype = GetFieldType(st.Data.TableName, col.Value, out strListName);
                    if (ftype == FieldType.CascadeSonList) //有范围限制的外键字段 且 为级联字段的子级（值需要动态获取）
                    {
                        //为每行的该字段声明一个区域并设置 Validation
                        for (var r = 0; r < st.Data.Rows.Count; r++)
                        {
                            var row = range.Row + st.TableHeadHeight + r;
                            var rangeCell = range.Worksheet.get_Range(colL + row, missing);
                            var tempListName = strListName + "_R" + row; //=数据有效性公式=序列命名={原序列名+_R+行号}
                            AddMetaList(tempListName, strListName); //与主表字段相似，初次加载时填充完整的list
                            rangeCell.Validation.Delete(); //若存在Validation，再执行Add()方法时会报错。
                            rangeCell.Validation.Add(Excel.XlDVType.xlValidateList, Formula1: "=" + tempListName);
                        }
                    }
                    else if (ftype == FieldType.CascadeParentList || ftype == FieldType.LimitedList || ftype == FieldType.List)
                    //枚举字段/无范围限制的外键字段/有范围限制但无级联关系的外键字段/有范围限制且有级联关系且不为级联的子级的外键字段
                    {
                        #region 设置下拉序列（枚举、外键）

                        if (_originData.Tables.Contains(strListName)) //原数据中包含
                        {
                            if (!_base.IsExistsName(strListName))
                            {
                                //若没有则新建一个
                                AddMetaList(strListName, _originData.Tables[strListName]);
                            }
                            else
                            {
                                UpdateMetaList(strListName, _originData.Tables[strListName]);
                            }
                            //设置数据有效性
                            var rangeColumn = range.Worksheet.get_Range(colL + (range.Row + st.TableHeadHeight),
                                colL + (range.Row + range.Rows.Count - 1));
                            rangeColumn.Validation.Delete(); //若存在Validation，再执行Add()方法时会报错。
                            rangeColumn.Validation.Add(Excel.XlDVType.xlValidateList, Formula1: "=" + strListName);
                        }

                        #endregion
                    }
                    else if (ftype == FieldType.TrueOrFalse)
                    {
                        #region 设置下拉序列（是否型）

                        if (!_base.IsExistsName(strListName)) //此时strListName="TrueOrFalse"
                        {
                            //若没有则新建一个
                            AddMetaList(strListName, new string[] { "是", "否" });
                        }

                        //设置数据有效性
                        var rangeColumn = range.Worksheet.get_Range(colL + (range.Row + st.TableHeadHeight),
                            colL + (range.Row + range.Rows.Count - 1));
                        rangeColumn.Validation.Delete();
                        rangeColumn.Validation.Add(Excel.XlDVType.xlValidateList, Formula1: "=" + strListName);

                        #endregion
                    }

                    #endregion

                    #region 填充行数据

                    for (var r = 0; r < st.Data.Rows.Count; r++)
                    {
                        var intRangeColumn = range.Row + st.TableHeadHeight + r; //当前行在excel中的行号
                        var rangeCell = range.Worksheet.get_Range(colL + intRangeColumn, missing);
                        this.GetDisplayValue(st.Data.TableName, col.Value, st.Data.Rows[r][col.Value].ToString(),
                            rangeCell, fieldType: ftype, listName: strListName);

                        //设置关联
                        //st.SetConnection(intRangeColumn, r);
                        //设置区域中每行的第一个单元格的ID，记录为主键值【OID/MyPK/No等】
                        var id = range.Worksheet.Cells[intRangeColumn, range.Column].ID;
                        if (string.IsNullOrWhiteSpace(id) || Equals(id, st.Data.Rows[r][st.PKColumnName].ToString()) == false)
                            range.Worksheet.Cells[intRangeColumn, range.Column].ID = st.Data.Rows[r][st.PKColumnName].ToString();
                    }

                    #endregion

                    //xTODO: 如果该字段是『级联的父级』，则需根据此字段的值来设置相应子级字段的“数据有效性” A:第一次加载时，子级是数据有效性全部，当“父级”改变时，B:再实时获取、设置子级的数据有效性
                }
            }
            #endregion

            _dictSubTables[dt.TableName] = st; //更新st
            return true;
        }

        /// <summary>
        /// 获取主~~、从~~表数据
        /// </summary>
        /// <param name="ds">ref子表数据</param>
        /// <returns>(AtParas)主表数据</returns>
        public string GetMainData()//ref DataSet ds) //x尚未测试
        {
            Hashtable htParas = new Hashtable();

            for (int i = 1; i <= Application.Names.Count; i++)
            {
                var name = Application.Names.Item(i).NameLocal; //name1
                var location = Application.Names.Item(i).RefersToLocal; //=Sheet1!$B$2
                if (location == "=#NAME?") //若单元格配置了公式（函数），则有可能被识别为NAME
                    continue;
                var range = Application.Names.Item(i).RefersToRange;
                //Excel.Range range;
                //try
                //{
                //	range = Application.Names.Item(i).RefersToRange;
                //}
                //catch
                //{
                //	continue;
                //}

                //var sheet = range.Worksheet.Name; //Sheet1
                //var col = Application.Names.Item(i).RefersToRange.Column; //2
                //var row = range.Row; //2
                //var val = range.Value2; //null/abc
                //range.Address //$B$2
                //range.AddressLocal //$B$2

                if (Regex.IsMatch(location, _base.regexRangeSingle)) //是单个单元格
                {
                    var strBelongDtl = _base.GetBelongDtlName(range);
                    if (strBelongDtl == null) //不属于某个子表
                    {
                        //htParas.Add(name, GetSaveValue("MainTable", name, range));
                        string val;
                        if (VaildData("MainTable", name, range, out val))
                            htParas.Add(name, val);
                        else
                            return null;
                    }
                    //else //属于子表
                    //{
                    //    if (!ds.Tables.Contains(strBelongDtl))
                    //    {
                    //        var dt = GetDtlData(strBelongDtl);
                    //        if (dt != null)
                    //            ds.Tables.Add(dt);
                    //    }
                    //}
                }
            }

            string r = string.Empty;
            ////把hashtable转换为@a=1形式的字符串
            //foreach (DictionaryEntry para in htParas)
            //{
            //    r += "@" + para.Key + "=" + para.Value;
            //}
            //!获取完整数据：
            var dt = _originData.Tables["MainTable"];//.Copy();
            foreach (DataColumn dc in dt.Columns)
            {
                if (htParas.Contains(dc.ColumnName))
                {
                    r += "@" + dc.ColumnName + "=" + htParas[dc.ColumnName];
                }
                else
                {
                    r += "@" + dc.ColumnName + "=" + dt.Rows[0][dc.ColumnName];
                }
            }
            return r;
        }

        /// <summary>
        /// 获取所有子表数据
        /// </summary>
        /// <param name="dsOld">(可选)所有子表原始数据</param>
        /// <returns></returns>
        public DataSet GetDtls(DataSet dsOld = null)
        {
            DataSet ds = new DataSet();
            foreach (KeyValuePair<string, SubTable> st in _dictSubTables)
            {
                //获取最新数据
                //0.获取Excel中的数据（Range->DataTable,with RowIdxInExcel）
                //1.根据st.Value.Connection更新newdata
                //?新数据中是否需要“未绑定到Excel表单的字段”？

                //ds.Tables.Add(GetDtl(st.Value));
                var dt = GetDtl(st.Value);
                if (dt == null)
                    return null;
                else
                    ds.Tables.Add(dt.Copy());

                //获取原始数据
                if (dsOld != null)
                    dsOld.Tables.Add(st.Value.OriginData.Copy());
            }
            return ds;
        }

        /// <summary>
        /// 获取子表数据
        /// </summary>
        /// <param name="st">子表对象</param>
        /// <returns>(DataTable)子表数据</returns>
        public DataTable GetDtl(SubTable st)
        {
            //删除『已标记为删除的行』
            //foreach (DataRow dr1 in st.Data.GetErrors())
            //{
            //    if (Equals(dr1.GetColumnError("Idx"), RowStatus.Deleted.ToString()))
            //        st.Data.Rows.Remove(dr1);
            //}

            //foreach (DataRow dr1 in st.Data.Rows)
            //{
            //    if (Equals(dr1.GetColumnError("Idx"), RowStatus.Deleted.ToString()))
            //        st.Data.Rows.Remove(dr1);
            //    //else if (dr["Idx"] == RowStatus.New.ToString()) //此时还没有dr["Idx"]="new"的行
            //    //	dr["OID"] = "0";
            //}

            //遍历单元格的行
            DataTable dt = st.Data.Clone();
            DataRow dr;
            int beginRowIdx = st.Range.Row + st.TableHeadHeight;

            for (var r = beginRowIdx; r < st.Range.Row + st.Range.Rows.Count; r++)
            {
                #region 判断是否是空行
                var rangeRow = st.Range.Worksheet.get_Range(_base.ConvertInt2Letter(st.Range.Column) + r,
                    _base.ConvertInt2Letter(st.Range.Column + st.Range.Columns.Count - 1) + r);
                if (_base.IsEmpty(rangeRow))
                {
                    continue;
                }
                #endregion

                var bindOid = st.GetOidByRowid(r);
                DataRow newRow = null;

                if (!string.IsNullOrEmpty(bindOid)) //『整行删除又填入数据』时作为修改处理
                {
                    var drs = st.Data.Select(string.Format("{1}='{0}'", bindOid, st.PKColumnName)); //!注意插入行时不能设置Idx="new"的关联信息
                    if (drs.Length == 1)
                    {
                        newRow = dt.Rows.Add(drs[0].ItemArray);
                        //此处要判断此行上面的行中，是否已经存在相同的OID主键值，此种情况是由于复制造成的，复制时，ID一同复制，造成有2个相同ID的单元格
                        //如果具有相同主键行，则标记此行为新建行
                        if (st.GetSameRowBeforeCurrentRowByPkValue(bindOid, r) > 0)
                            newRow[st.PKColumnName] = 0;
                    }
                    else
                    {
                        newRow = dt.NewRow();
                    }
                }
                else
                {
                    newRow = dt.NewRow();
                }

                //保存关联行号
                newRow["Idx"] = beginRowIdx++;
                //保存所有字段
                foreach (KeyValuePair<int, string> col in st.Columns)
                {
                    var rangeCell = st.Range.Worksheet.get_Range(_base.ConvertInt2Letter(col.Key) + r, missing);
                    string val;

                    if (VaildData(st.Data.TableName, col.Value, rangeCell, out val))
                    {
                        newRow[col.Value] = val;
                    }
                    else
                    {
                        //数据验证不通过时，删除所有新行，避免下一次保存时重复插入新行
                        var drs = st.Data.Select(string.Format("{0}='0'", st.PKColumnName));

                        foreach (DataRow dr1 in drs)
                        {
                            st.Data.Rows.Remove(dr1);
                        }

                        return null;
                    }
                }

                if (newRow[st.PKColumnName] == DBNull.Value)
                {
                    newRow[st.PKColumnName] = 0; //标记为新建行
                    dt.Rows.Add(newRow);
                }
            }

            return dt;
        }

        #region 表单数据相关方法

        /// <summary>
        /// 获取字段显示值（主要用于下拉/是否型字段No->Name）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyOfEn"></param>
        /// <param name="value"></param>
        /// <param name="rangeCell">(可选)单元格：若传入，则自动设置该单元格的值为计算后的“显示值”</param>
        /// <param name="fieldType">(可选)字段类型：若已获取了当前字段的FieldType,则可传入以节省计算步骤</param>
        /// <param name="listName">(可选)序列名称（序列区域命名）</param>
        /// <returns></returns>
        public string GetDisplayValue(string tableName, string keyOfEn, string value, Excel.Range rangeCell = null, FieldType fieldType = FieldType.Nothing, string listName = null)
        {
            string displayValue = null;
            if (fieldType == FieldType.Nothing || string.IsNullOrEmpty(listName))
                fieldType = GetFieldType(tableName, keyOfEn, out listName);
            if (fieldType == FieldType.CascadeParentList || fieldType == FieldType.CascadeSonList || fieldType == FieldType.LimitedList || fieldType == FieldType.List) //下拉类型的字段（枚举、外键）
            {
                //尝试读取Text字段
                var text = SelectDataTable(tableName, string.Format("{0}='{1}'", keyOfEn, value), keyOfEn + "Text");
                if (!string.IsNullOrEmpty(text))
                    displayValue = text;
                else
                    displayValue = Glo.GetNameByNo(_originData, listName, value);
            }
            else if (fieldType == FieldType.TrueOrFalse) //是否型字段
            {
                displayValue = value == "1" ? "是" : "否";
            }
            else
            {
                displayValue = value;
            }
            if (rangeCell != null)
            {
                IgnoreNextOperation();
                rangeCell.Value2 = displayValue; //TODO: 如果该单元格【是合并单元格】且【不是最小坐标的】，则此赋值方法无效，是否有必要调整？（若调整，则需要分主子表的不同情况来处理）
            }
            return displayValue;
        }

        /// <summary>
        /// 获取字段存储值（主要用于下拉/是否型字段Name->No）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyOfEn"></param>
        /// <param name="rangeCell"></param>
        /// <param name="fieldType">(可选)字段类型：若已获取了当前字段的FieldType,则可传入以节省计算步骤</param>
        /// <param name="listName">(可选)序列名称（序列区域命名）</param>
        /// <returns></returns>
        public string GetSaveValue(string tableName, string keyOfEn, Excel.Range rangeCell, FieldType fieldType = FieldType.Nothing, string listName = null)
        {
            if (_base.IsMerge(rangeCell))//2017-06-29：如果要取值的单元格已与其他单元格合并，则要从合并区域的第一个单元格坐标取值//不是合并时rangeCell==rangeCell.MergeArea
                rangeCell = rangeCell.MergeArea.Cells[1, 1];

            if (rangeCell == null || rangeCell.Value2 == null)
                return null;

            string text = rangeCell.Text;
            if (string.IsNullOrEmpty(text) || text.Trim().Length == 0)
                return null;

            //特殊类型的字段：日期
            DateTime dt;
            if (DateTime.TryParse(rangeCell.Text, out dt))
                return rangeCell.Text;

            if (fieldType == FieldType.Nothing || string.IsNullOrEmpty(listName))
                fieldType = GetFieldType(tableName, keyOfEn, out listName);
            //?是否需要判断单元格IsValidList？？
            if (_base.IsValidList(rangeCell) && (fieldType == FieldType.CascadeParentList || fieldType == FieldType.CascadeSonList || fieldType == FieldType.LimitedList || fieldType == FieldType.List))//下拉类型的字段（枚举、外键）
                return Glo.GetNoByName(_originData, listName, rangeCell.Value2);
            else if (_base.IsValidList(rangeCell) && fieldType == FieldType.TrueOrFalse) //是否型字段
                return rangeCell.Value2 == "是" ? "1" : "0";
            else
                return rangeCell.Value2.ToString();
        }

        public enum FieldType
        {
            /// <summary>
            /// 不是表单字段
            /// </summary>
            Nothing,
            /// <summary>
            /// 普通字段
            /// </summary>
            Normal,
            /// <summary>
            /// 是否型字段
            /// </summary>
            TrueOrFalse,
            /// <summary>
            /// 序列（下拉）
            /// </summary>
            List,
            /// <summary>
            /// 有限制的序列（下拉）
            /// </summary>
            LimitedList,
            /// <summary>
            /// 级联【子级】序列（下拉）
            /// </summary>
            CascadeSonList,
            /// <summary>
            /// 级联【父级】序列（下拉）
            /// </summary>
            CascadeParentList
        }

        /// <summary>
        /// 获取字段类型（并返回下拉类型的列表名(ListName(=ListTableName))）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strKeyOfEn"></param>
        /// <param name="strListName">下拉类型的列表名(ListName(=ListTableName))</param>
        /// <returns>enum FieldType</returns>
        public FieldType GetFieldType(string tableName, string strKeyOfEn, out string strListName)
        {
            var extTableName = tableName == "MainTable" ? "Sys_MapExt" : "Sys_MapExt_For_" + tableName;
            var attrTableName = tableName == "MainTable" ? "Sys_MapAttr" : "Sys_MapAttr_For_" + tableName;
            FieldType type = FieldType.Nothing;
            //是否型字段
            var drs1 = _originData.Tables[attrTableName].Select(string.Format("MyDataType='4' and KeyOfEn='{0}'", strKeyOfEn));
            if (drs1.Length > 0)
            {
                strListName = "TrueOrFalse";
                type = FieldType.TrueOrFalse;
            }
            else
            {
                //从『表单字段扩展信息』中获取相关数据
                if (_originData.Tables.Contains(extTableName))
                {
                    //作为级联的子级
                    var drs = _originData.Tables[extTableName].Select(string.Format("ExtType='ActiveDDL' and AttrsOfActive='{0}'", strKeyOfEn));
                    if (drs.Length > 0)
                    {
                        //listName = SelectDataTable(attrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "MyPk");
                        type = FieldType.CascadeSonList;
                    }
                    //作为级联的父级
                    drs = _originData.Tables[extTableName].Select(string.Format("ExtType='ActiveDDL' and AttrOfOper='{0}'", strKeyOfEn));
                    if (drs.Length > 0)
                    {
                        //listName = SelectDataTable(attrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "UIBindKey");
                        type = FieldType.CascadeParentList;
                    }
                    //外键（e.g. 本部门的人员）
                    drs = _originData.Tables[extTableName].Select(string.Format("ExtType='AutoFullDLL' and AttrOfOper='{0}'", strKeyOfEn));
                    if (drs.Length > 0)
                    {
                        //listName = SelectDataTable(attrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "MyPk");
                        type = FieldType.LimitedList;
                    }
                    //TODO: ExtType=TBFullCtrl?
                }
                var tempListName = SelectDataTable(attrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "MyPk");
                if (!string.IsNullOrEmpty(tempListName) && _originData.Tables.Contains(tempListName))
                {
                    if (type == FieldType.Nothing) //若不是级联、外键
                        type = FieldType.LimitedList;
                    strListName = tempListName;
                }
                else
                {
                    tempListName = SelectDataTable(attrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "UIBindKey");
                    if (!string.IsNullOrEmpty(tempListName) && _originData.Tables.Contains(tempListName))
                    {
                        if (type == FieldType.Nothing) //若不是级联、外键
                            type = FieldType.List;
                        strListName = tempListName;
                    }
                    else
                    {
                        type = FieldType.Normal;
                        strListName = null;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// 获取DataTable中的某一行某个字段的值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strWhere"></param>
        /// <param name="strSelectColumn"></param>
        /// <returns></returns>
        public string SelectDataTable(string strTableName, string strWhere, string strSelectColumn)
        {
            if (!_originData.Tables.Contains(strTableName))
                return null;
            if (!_originData.Tables[strTableName].Columns.Contains(strSelectColumn))
                return null;
            var drs = _originData.Tables[strTableName].Select(strWhere);
            if (drs.Length == 0)
                return null;
            return drs[0][strSelectColumn].ToString();
        }

        /// <summary>
        /// 【弃用】判断某个字段是否是枚举/外键类型
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strKeyOfEn"></param>
        /// <param name="UiBindKey"></param>
        /// <returns></returns>
        public bool __abandon_IsEnumOrFk(string tableName, string strKeyOfEn, ref string UiBindKey)
        {
            if (_originData.Tables.Contains(tableName))
            {
                foreach (DataRow dr in _originData.Tables[tableName].Rows)
                {
                    if (dr["KeyOfEn"].ToString() == strKeyOfEn && dr["LGType"].ToString() != "0")
                    {
                        UiBindKey = dr["UIBindKey"].ToString();
                        return !string.IsNullOrEmpty(UiBindKey);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 【弃用】判断某个字段是否是『有范围限制的外键』字段
        /// </summary>
        /// <param name="tableName">表名（MainTable/子表名）</param>
        /// <param name="strKeyOfEn">字段名（KeyOfEn）</param>
        /// <param name="UiBindKey">ref关联外键区域的命名（return false时不改变值，return true时返回该字段的MyPk）</param>
        /// <returns></returns>
        public bool __abandon_IsLimitedFk(string tableName, string strKeyOfEn, ref string UiBindKey)
        {
            var strMapExtTableName = tableName == "MainTable" ? "Sys_MapExt" : "Sys_MapExt_For_" + tableName;
            var strMapAttrTableName = tableName == "MainTable" ? "Sys_MapAttr" : "Sys_MapAttr_For_" + tableName;
            if (_originData.Tables.Contains(strMapExtTableName))
            {
                foreach (DataRow dr in _originData.Tables[strMapExtTableName].Rows)
                {
                    if ((dr["ExtType"].ToString() == "ActiveDDL" && dr["AttrsOfActive"].ToString() == strKeyOfEn) && //作为级联的下级
                        (dr["ExtType"].ToString() == "AutoFullDLL" && dr["AttrOfOper"].ToString() == strKeyOfEn) && //外键（e.g. 本部门的人员）
                        !string.IsNullOrEmpty(dr["Doc"].ToString()))
                    {
                        if (_originData.Tables.Contains(strMapAttrTableName))
                        {
                            var drs = _originData.Tables[strMapAttrTableName].Select("KeyOfEn='" + strKeyOfEn + "'");
                            if (drs.Length == 0)
                                UiBindKey = null;
                            else
                                UiBindKey = drs[0]["MyPK"].ToString();
                        }
                        else
                        {
                            UiBindKey = null;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 验证数据合法性（2017-03-23：目前仅判断必填项；2017-03-24：增加数据类型判断）
        /// </summary>
        /// <param name="tableName">MainTable/子表表名</param>
        /// <param name="keyOfEn">字段名</param>
        /// <param name="range">区域</param>
        /// <param name="val">输出值：该字段的保存值</param>
        /// <returns>是否验证通过</returns>
        public bool VaildData(string tableName, string keyOfEn, Excel.Range range, out string val)
        {
            var strMapAttrTableName = tableName == "MainTable" ? "Sys_MapAttr" : "Sys_MapAttr_For_" + tableName;
            val = GetSaveValue(tableName, keyOfEn, range);
            var dtAttr = _originData.Tables[strMapAttrTableName];
            var dr = dtAttr.Select("KeyOfEn='" + keyOfEn + "'");
            if (dr.Length != 1)
                return true;

            string name = dr[0]["Name"].ToString();

            //必填验证
            if (dr[0]["UIIsInput"].ToString() == "1")
            {
                if (string.IsNullOrEmpty(val))
                {
                    MessageBox.Show("从表[" + this.GetDtlNameByTableName(tableName) + "]字段:" + name + "(" + keyOfEn + ")”不能为空！\n单元格：" + range.Address,
                        "ccform输入提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    range.Activate();
                    return false;
                }
            }

            //数据类型验证
            if (string.IsNullOrEmpty(val) == false && dr[0]["LGType"].ToString() == "0")
            {
                //判断数据类型.
                int myDataType = int.Parse(dr[0]["MyDataType"].ToString());

                switch (myDataType)
                {
                    case BP.DA.DataType.AppInt: //整数类型.
                        int i;
                        if (int.TryParse(val, out i) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“整数”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    case BP.DA.DataType.AppFloat: //浮点型
                        float f;
                        if (float.TryParse(val, out f) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“数字（整数或小数）”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    case BP.DA.DataType.AppDouble: //双精度型
                        double d;
                        if (double.TryParse(val, out d) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“数字（整数或小数）”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    case BP.DA.DataType.AppDate: //日期型
                        DateTime date;
                        if (DateTime.TryParse(val, out date) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“日期”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    case BP.DA.DataType.AppDateTime: //时间型
                        DateTime time;
                        if (DateTime.TryParse(val, out time) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“时间”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    case BP.DA.DataType.AppMoney: //金额
                        float c;
                        if (float.TryParse(val, out c) == false)
                        {
                            MessageBox.Show("字段:" + name + "(" + keyOfEn + ") 只能填入“数字（金额）”！", "ccform输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            range.Activate();
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        /*
		//动态获取『有范围限制的外键字段』的区域填充数据
		public DataTable GetAreaList()
		{
			//!获取数据成功后更新_originData
			return null;
		}*/

        #endregion

        #region VSTO相关方法

        /// <summary>
        /// 获取子表区域中的表头（字段）信息
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetAreaColumns(Excel.Range range, out int intThHeight)
        {
            //获取字段信息，及表头所占子行数
            Dictionary<int, string> dictColumns = new Dictionary<int, string>();
            intThHeight = 1; //表头所占子行数
            for (var c = range.Column; c <= range.Column + range.Columns.Count - 1;)
            {
                string name = string.Empty;
                var currentThMergeColumnsCount = 1; //当前表头（单元格）所占列数
                for (var r = range.Row; r < range.Row + range.Rows.Count - 1;)
                {
                    var rangeTableHead = range.Worksheet.get_Range(_base.ConvertInt2Letter(c) + r, missing);

                    try
                    {
                        //若该单元格有命名
                        if (rangeTableHead.Name != null && rangeTableHead.Name.Name != null) //rangeTableHead.Name抛出异常
                        {
                            name = rangeTableHead.Name.Name;
                            var currentThMergeRowsCount = 1;
                            if (rangeTableHead.MergeCells)
                            {
                                currentThMergeColumnsCount = rangeTableHead.MergeArea.Columns.Count; //用于计算下一个要循环的列
                                currentThMergeRowsCount = rangeTableHead.MergeArea.Rows.Count; //用于计算当前表头所占行数
                            }
                            var currentThHeight = rangeTableHead.Row + currentThMergeRowsCount - range.Row; //当前表头占子表区域的高度（行数）
                            if (intThHeight < currentThHeight)
                                intThHeight = currentThHeight;
                            break; //发现表头即停止循环，不再往下寻找表头
                        }
                    }
                    catch
                    {
                    }

                    //!下一个要循环的行
                    //r += (rangeTableHead.MergeCells ? rangeTableHead.MergeArea.Rows.Count - 1 : 1);
                    if (rangeTableHead.MergeCells)
                        r += rangeTableHead.MergeArea.Rows.Count;
                    else
                        r++;
                }

                //若该列有绑定列
                if (!string.IsNullOrEmpty(name) && name.IndexOf(".") > -1)
                    dictColumns.Add(c, name.Substring(name.LastIndexOf('.') + 1));

                //!下一个要循环的列
                c += currentThMergeColumnsCount;
            }
            return dictColumns;
        }

        /// <summary>
        /// 新增一个元数据List区域
        /// </summary>
        /// <param name="strListName"></param>
        /// <returns></returns>
        public string AddMetaList(string strListName)
        {
            var count = _base.GetNamesCountInSheet("MetaData") + 1;
            Excel.Worksheet ws = _base.GetWorksheet("MetaData");
            var rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
            var strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            while (strBeloneListName != null)
            {
                count += 1;
                rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
                strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            }
            var colL = _base.ConvertInt2Letter(count);
            Application.Names.Add(strListName, string.Format("=MetaData!${0}$1:${0}$2", colL));
            return colL;
        }
        /// <summary>
        /// 新增一个元数据List区域，并填充List数据
        /// </summary>
        /// <param name="strListName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string AddMetaList(string strListName, string strTableName)
        {
            if (!_originData.Tables.Contains(strTableName))
                return null;
            return this.AddMetaList(strListName, _originData.Tables[strTableName]);
        }
        /// <summary>
        /// 新增一个元数据List区域，并填充List数据
        /// </summary>
        /// <param name="strListName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string AddMetaList(string strListName, DataTable dt)
        {
            if (!dt.Columns.Contains("Name"))//默认用“Name”列填充
                return null;

            //获取可用的列
            var count = _base.GetNamesCountInSheet("MetaData") + 1;
            Excel.Worksheet ws = _base.GetWorksheet("MetaData");
            var rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
            var strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            while (strBeloneListName != null)
            {
                count += 1;
                rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
                strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            }
            var colL = _base.ConvertInt2Letter(count);

            if (dt.Rows.Count == 0)//序列为空时先只占位
            {
                var rangeCcRx = ws.get_Range(colL + 1, missing);
                IgnoreNextOperation();
                rangeCcRx.Value2 = null;
                Application.Names.Add(strListName, "=MetaData!$" + colL + "$1"); // +":$c$1"?
            }
            else
            {
                var r = 1;
                for (; r <= dt.Rows.Count; r++)
                {
                    var rangeCcRx = ws.get_Range(colL + r, missing);
                    IgnoreNextOperation();
                    rangeCcRx.Value2 = dt.Rows[r - 1]["Name"].ToString();
                }
                Application.Names.Add(strListName, "=MetaData!$" + colL + "$1:$" + colL + "$" + (r - 1));
            }
            return colL;
        }
        public string AddMetaList(string strListName, string[] list)
        {
            //获取可用的列
            var count = _base.GetNamesCountInSheet("MetaData") + 1;
            Excel.Worksheet ws = _base.GetWorksheet("MetaData");
            var rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
            var strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            while (strBeloneListName != null)
            {
                count += 1;
                rangeCxR1 = ws.get_Range(_base.ConvertInt2Letter(count) + 1, missing);
                strBeloneListName = _base.GetBelongDtlName(rangeCxR1);
            }
            var colL = _base.ConvertInt2Letter(count);

            var r = 1;
            for (; r <= list.Length; r++)
            {
                var rangeCcRx = ws.get_Range(colL + r, missing);
                IgnoreNextOperation();
                rangeCcRx.Value2 = list[r - 1];
            }
            Application.Names.Add(strListName, "=MetaData!$" + colL + "$1:$" + colL + "$" + (r - 1));
            return colL;
        }
        /// <summary>
        /// 更新元数据List区域
        /// </summary>
        /// <param name="strListName"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool UpdateMetaList(string strListName, string strTableName)
        {
            if (!_originData.Tables.Contains(strTableName))
                return false;
            return this.UpdateMetaList(strListName, _originData.Tables[strTableName]);
        }
        /// <summary>
        /// 更新元数据List区域
        /// </summary>
        /// <param name="range"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool UpdateMetaList(Excel.Range range, string strTableName)
        {
            if (!_originData.Tables.Contains(strTableName))
                return false;
            return this.UpdateMetaList(range, _originData.Tables[strTableName]);
        }
        /// <summary>
        /// 更新元数据List区域
        /// </summary>
        /// <param name="strListName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool UpdateMetaList(string strListName, DataTable dt)
        {
            if (!_base.IsExistsName(strListName))
                return false;
            return this.UpdateMetaList(Application.Names.Item(strListName).RefersToRange, dt);
        }
        /// <summary>
        /// 更新元数据List区域
        /// </summary>
        /// <param name="range"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool UpdateMetaList(Excel.Range range, DataTable dt)
        {
            if (range.Worksheet.Name != "MetaData")
                return false;
            if (!dt.Columns.Contains("Name"))
                return false;
            string name;
            if (Regex.IsMatch(range.Address, _base.regexAddressCell))
                name = _base.GetNameFakeArea(range);
            else
                name = range.Name.Name;
            var colL = _base.ConvertInt2Letter(range.Column);
            if (dt.Rows.Count == 0)
            {
                var rangeCcRx = range.Worksheet.get_Range(colL + 1, missing);
                IgnoreNextOperation();
                rangeCcRx.Value2 = null;
                Application.Names.Add(name, "=MetaData!$" + colL + "$1"); // +":$c$1"?
                return true;
            }
            else
            {
                var r = 1;
                for (; r <= dt.Rows.Count; r++)
                {
                    var rangeCcRx = range.Worksheet.get_Range(colL + r, missing);
                    IgnoreNextOperation();
                    rangeCcRx.Value2 = dt.Rows[r - 1]["Name"].ToString();
                }
                Application.Names.Add(name, "=MetaData!$" + colL + "$1:$" + colL + "$" + (r - 1));
                return true;
            }
        }

        #endregion

        public void Show(string msg)
        {
            //if (_isDebug)
            //	MessageBox.Show(msg);
        }

        #region VSTO 生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);

            //单元格内容变动监听事件
            this.Application.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(Application_SheetChange);
            //保存事件
            this.Application.WorkbookAfterSave += new Excel.AppEvents_WorkbookAfterSaveEventHandler(Application_WorkbookAfterSave);

            //撤销事件
            //this.Application.OnUndo += new Excel.(Application_OnUndo); 
        }

        #endregion
    }
}
