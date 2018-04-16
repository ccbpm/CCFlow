using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;


namespace BP.WF.HttpHandler
{
    abstract public class DirectoryPageBase
    {
        #region 执行方法.
        /// <summary>
        /// 获得Form数据.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>返回值</returns>
        public string GetValFromFrmByKey(string key, string isNullAsVal = null)
        {
            string val = context.Request.Form[key];
            if (val == null && key.Contains("DDL_") == false)
                val = context.Request.Form["DDL_" + key];

            if (val == null && key.Contains("TB_") == false)
                val = context.Request.Form["TB_" + key];

            if (val == null && key.Contains("CB_") == false)
                val = context.Request.Form["CB_" + key];

            if (val == null)
            {
                if (isNullAsVal != null)
                    return isNullAsVal;
                return "";
                //throw new Exception("@获取Form参数错误,参数集合不包含[" + key + "]");
            }

            val = val.Replace("'", "~");
            return val;
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">对象名</param>
        /// <param name="methodName">方法</param>
        /// <returns>返回执行的结果，执行错误抛出异常</returns>
        public string DoMethod(DirectoryPageBase myEn, string methodName)
        {
            try
            {
                Type tp = myEn.GetType();
                MethodInfo mp = tp.GetMethod(methodName);
                if (mp == null)
                {
                    /* 没有找到方法名字，就执行默认的方法. */
                    return myEn.DoDefaultMethod();
                }

                //执行该方法.
                object[] paras = null;
                return mp.Invoke(this, paras) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return "err@调用类:[" + myEn + "]方法:[" + methodName + "]出现错误:" + ex.InnerException;
                else
                    return "err@调用类:[" + myEn + "]方法:[" + methodName + "]出现错误:" + ex.Message;
            }
        }
        /// <summary>
        /// 执行默认的方法名称
        /// </summary>
        /// <returns>返回执行的结果</returns>
        protected virtual string DoDefaultMethod()
        {
            return "err@子类[" + this.ToString() + "]没有重写该[" + this.DoType + "]方法，请确认该方法是否缺少或者是非public类型的.";
        }
        #endregion 执行方法.

        #region 公共方法.
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string key)
        {
            string val = context.Request[key];
            if (val == null)
                val = context.Request.QueryString[key];
            if (val == null)
                val = context.Request.Form[key];

            if (val == null)
                return null;

            return HttpUtility.UrlDecode(val, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public int GetRequestValInt(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null" || str == "undefined")
                return 0;

            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public bool GetRequestValBoolen(string param)
        {
            if (this.GetRequestValInt(param) == 1)
                return true;
            return false;
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Int64 GetRequestValInt64(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return Int64.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public float GetRequestValFloat(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return float.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        public decimal GetRequestValDecimal(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return decimal.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获得参数.
        /// </summary>
        public string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = this.context.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;

                    if (para == "1=1")
                        continue;

                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
        #endregion

        #region 属性参数.
        /// <summary>
        /// 
        /// </summary>
        public string PKVal
        {
            get
            {
                string str = this.GetRequestVal("PKVal");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("OID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("No");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("MyPK");
                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("NodeID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("WorkID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("PK");

                if ("null".Equals(str) == true)
                    return null;

                return str;
            }
        }
        /// <summary>
        /// 是否是移动？
        /// </summary>
        public bool IsMobile
        {
            get
            {
                string v = this.GetRequestVal("IsMobile");
                if (v != null && v == "1")
                    return true;

                if (System.Web.HttpContext.Current.Request.RawUrl.Contains("/CCMobile/") == true)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = this.GetRequestVal("No"); // context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string Name
        {
            get
            {
                string str =this.GetRequestVal("Name");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }

        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                //获得执行的方法.
                string doType = "";

                doType = this.GetRequestVal("DoType");
                if (doType == null)
                    doType = this.GetRequestVal("Action");

                if (doType == null)
                    doType = this.GetRequestVal("action");

                if (doType == null)
                    doType = this.GetRequestVal("Method");

                return doType;
            }
        }
        public string EnName
        {
            get
            {
                string str = this.GetRequestVal("EnName");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FK_MapData");

                if (str == null || str == "" || str == "null")
                    return null;

                return str;
            }
        }
        public string EnsName
        {
            get
            {
                string str = this.GetRequestVal("EnsName");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FK_MapData");

                if (str == null || str == "" || str == "null")
                {
                    if (this.EnName == null)
                        return null;
                    return this.EnName + "s";
                }
                return str;
            }
        }
        public string FK_Dept
        {
            get
            {
                string str = this.GetRequestVal("FK_Dept");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string MyPK
        {
            get
            {
                string str = this.GetRequestVal("MyPK");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string FK_Event
        {
            get
            {
                string str = this.GetRequestVal("FK_Event");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 字典表
        /// </summary>
        public string FK_SFTable
        {
            get
            {
                string str = this.GetRequestVal("FK_SFTable");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string EnumKey
        {
            get
            {
                string str = this.GetRequestVal("EnumKey");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string KeyOfEn
        {
            get
            {
                string str = this.GetRequestVal("KeyOfEn");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = this.GetRequestVal("FK_MapData");
                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FrmID");
                return str;
            }
        }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public string FK_MapExt
        {
            get
            {
                string str = this.GetRequestVal("FK_MapExt");
                if (str == null || str == "" || str == "null")
                {
                    str = this.GetRequestVal("MyPK");
                    if (str == null || str == "" || str == "null")
                    {
                        return null;
                    }
                }


                return str;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string str = this.GetRequestVal("FK_Flow");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                string str = this.GetRequestVal("FK_Emp");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                string str = this.GetRequestVal("FrmID");
                if (str == null || str == "" || str == "null")
                    return this.GetRequestVal("FK_MapData");

                return str;
            }
        }
        public int GroupField
        {
            get
            {
                string str = this.GetRequestVal("GroupField");
                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                int nodeID = this.GetRequestValInt("FK_Node");
                if (nodeID == 0)
                    nodeID = this.GetRequestValInt("NodeID");
                return nodeID;
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetRequestValInt("FID");

                string str = this.GetRequestVal("FID");//  context.Request.QueryString["FID"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        public Int64 PWorkID
        {
            get
            {
                return this.GetRequestValInt("PWorkID");
 
            }
        }
        private Int64 _workID = 0;
        public Int64 WorkID
        {
            get
            {
                if (_workID != 0)
                    return _workID;

                string str = this.GetRequestVal("WorkID");
                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("PKVal");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("OID");

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
            set
            {
                _workID = value;
            }
        }
        public Int64 CWorkID
        {
            get
            {
                return this.GetRequestValInt("CWorkID");
            }
        }
        /// <summary>
        /// 框架ID
        /// </summary>
        public string FK_MapFrame
        {
            get
            {


                string str = this.GetRequestVal("FK_MapFrame");// context.Request.QueryString["FK_MapFrame"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// SID
        /// </summary>
        public string SID
        {
            get
            {
                string str = this.GetRequestVal("SID"); // context.Request.QueryString["SID"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        ///   RefOID
        /// </summary>
        public int RefOID
        {
            get
            {
                string str = this.GetRequestVal("RefOID"); //context.Request.QueryString["RefOID"];

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("OID"); //  context.Request.QueryString["OID"];

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
        }
        public int OID
        {
            get
            {
                string str = this.GetRequestVal("RefOID"); // context.Request.QueryString["RefOID"];
                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("OID");  //context.Request.QueryString["OID"];

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
        }
        /// <summary>
        /// 明细表
        /// </summary>
        public string FK_MapDtl
        {
            get
            {
                string str = this.GetRequestVal("FK_MapDtl"); //context.Request.QueryString["FK_MapDtl"];
                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("EnsName");// context.Request.QueryString["EnsName"];
                return str;
            }
        }
        /// <summary>
        /// 页面Index.
        /// </summary>
        public int PageIdx
        {
            get
            {
                return this.GetRequestValInt("PageIdx");
            }
        }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize
        {
            get
            {
                int i= this.GetRequestValInt("PageSize");
                if (i == 0)
                    return 10;
                return i;
            }
        }
        public int Index
        {
            get
            {
                return this.GetRequestValInt("Index");
            }
        }

        /// <summary>
        /// 字段属性编号
        /// </summary>
        public string Ath
        {
            get
            {
                string str = this.GetRequestVal("Ath");// context.Request.QueryString["Ath"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public HttpContext context = null;
        /// <summary>
        /// 获得Int数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetValIntFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return int.Parse(str);
        }

        public float GetValFloatFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return float.Parse(str);
        }
        public decimal GetValDecimalFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return decimal.Parse(str);
        }

        public bool GetValBoolenFromFrmByKey(string key)
        {
            string val = this.GetValFromFrmByKey(key, "0");
            if (val == "on" || val == "1")
                return true;
            if (val == null || val == "" || val == "0" || val == "off")
                return false;
            return true;
        }

        public new string RefPK
        {
            get
            {
                return this.GetRequestVal("RefPK");

                //string str = this.context.Request.QueryString["RefPK"];
                //return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.GetRequestVal("RefPKVal");
                if (str == null)
                    return "0";
                return str;
            }
        }
        #endregion 属性.

        protected string ExportDGToExcel(System.Data.DataTable dt, Entity en, string title)
        {
            string filename = title + "_" + BP.DA.DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";//"Ep" + this.Session.SessionID + ".xls";
            string file = filename;
            bool flag = true;
            string filepath = BP.Sys.SystemConfig.PathOfTemp;

            #region 参数及变量设置
            //			//参数校验
            //			if (dg == null || dg.Items.Count <=0 || filename == null || filename == "" || filepath == null || filepath == "")
            //				return null;

            //如果导出目录没有建立，则建立.
            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);

            filename = filepath + filename;

            if (File.Exists(filename))
                File.Delete(filename);

            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            try
            {
                Attrs attrs = en.EnMap.Attrs;
                Attrs selectedAttrs = null;
                BP.Sys.UIConfig cfg = new UIConfig(en);

                if (cfg.ShowColumns.Length == 0)
                    selectedAttrs = attrs;
                else
                {
                    selectedAttrs = new Attrs();

                    foreach (Attr attr in attrs)
                    {
                        bool contain = false;

                        foreach (string col in cfg.ShowColumns)
                        {
                            if (col == attr.Key)
                            {
                                contain = true;
                                break;
                            }
                        }

                        if (contain)
                            selectedAttrs.Add(attr);
                    }
                }

                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + title + Convert.ToChar(9));
                objStreamWriter.WriteLine();
                string strLine = "";

                //生成文件标题
                foreach (Attr attrT in selectedAttrs)
                {
                    if (attrT.UIVisible == false)
                        continue;

                    if (attrT.Key == "MyNum")
                        continue;

                    strLine = strLine + attrT.Desc + Convert.ToChar(9);
                }

                objStreamWriter.WriteLine(strLine);
                strLine = "";

                foreach (DataRow dr in dt.Rows)
                {
                    foreach (Attr attr in selectedAttrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.Key == "MyNum")
                            continue;

                        if (attr.MyDataType == DataType.AppBoolean)
                        {
                            strLine = strLine + (dr[attr.Key].Equals(1) ? "是" : "否") + Convert.ToChar(9);
                        }
                        else
                        {
                            strLine = strLine + dr[attr.IsFKorEnum ? (attr.Key + "Text") : attr.Key] + Convert.ToChar(9);
                        }
                    }

                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }


                objStreamWriter.WriteLine();
                objStreamWriter.WriteLine(Convert.ToChar(9) + " 制表人：" + Convert.ToChar(9) + WebUser.Name + Convert.ToChar(9) + "日期：" + Convert.ToChar(9) + DateTime.Now.ToShortDateString());

            }
            catch
            {
                flag = false;
            }
            finally
            {
                objStreamWriter.Close();
                objFileStream.Close();
            }
            #endregion

            #region 删除掉旧的文件
            //DelExportedTempFile(filepath);
            #endregion

            if (flag)
            {
               file ="/DataUser/Temp/" + file;
                //this.Write_Javascript(" window.open('"+ Request.ApplicationPath + @"/Report/Exported/" + filename +"'); " );
                //this.Write_Javascript(" window.open('"+Request.ApplicationPath+"/Temp/" + file +"'); " );
            }

            return file;
        }

        public static string DataTableToExcel(DataTable dt, string filename, string header = null,
            string creator = null, bool date = false, bool index = true, bool download = false)
        {

            string file = BP.Sys.SystemConfig.PathOfTemp + filename;
            bool flag = true;

            string dir = Path.GetDirectoryName(file);
            string name = Path.GetFileName(filename);
            long len = 0;
            IRow row = null, headerRow = null, dateRow = null, sumRow = null, creatorRow = null;
            ICell cell = null;
            int r = 0;
            int c = 0;
            int headerRowIndex = 0; //文件标题行序
            int dateRowIndex = 0;   //日期行序
            int titleRowIndex = 0;  //列标题行序
            int sumRowIndex = 0;    //合计行序
            int creatorRowIndex = 0;    //创建人行序
            float DEF_ROW_HEIGHT = 20;  //默认行高
            float charWidth = 0;    //单个字符宽度
            int columnWidth = 0;    //列宽，像素
            bool isDate;    //是否是日期格式，否则是日期时间格式
            int decimalPlaces = 2;  //小数位数
            bool qian;  //是否使用千位分隔符
            List<int> sumColumns = new List<int>();   //合计列序号集合

            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);
            

            //一个字符的像素宽度，以Arial，10磅，i进行测算
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    charWidth = g.MeasureString("i", new Font("Arial", 10)).Width;
                }
            }
            //序
            if (index && dt.Columns.Contains("序") == false)
            {
                dt.Columns.Add("序", typeof(int)).ExtendedProperties.Add("width", 50);
                dt.Columns["序"].SetOrdinal(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["序"] = i + 1;
            }
            //合计列
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ExtendedProperties.ContainsKey("sum") == false)
                    continue;

                sumColumns.Add(col.Ordinal);
            }

            headerRowIndex = string.IsNullOrWhiteSpace(header) ? -1 : 0;
            dateRowIndex = date ? (headerRowIndex + 1) : -1;
            titleRowIndex = date
                                        ? dateRowIndex + 1
                                        : headerRowIndex == -1 ? 0 : 1;
            sumRowIndex = sumColumns.Count == 0 ? -1 : titleRowIndex + dt.Rows.Count + 1;
            creatorRowIndex = string.IsNullOrWhiteSpace(creator)
                                  ? -1
                                  : sumRowIndex == -1 ? titleRowIndex + dt.Rows.Count + 1 : sumRowIndex + 1;

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                HSSFWorkbook wb = new HSSFWorkbook();
                ISheet sheet = wb.CreateSheet("Sheet1");
                sheet.DefaultRowHeightInPoints = DEF_ROW_HEIGHT;
                IFont font = null;
                IDataFormat fmt = wb.CreateDataFormat();

                if (headerRowIndex != -1)
                    headerRow = sheet.CreateRow(headerRowIndex);
                if (date)
                    dateRow = sheet.CreateRow(dateRowIndex);
                if (sumRowIndex != -1)
                    sumRow = sheet.CreateRow(sumRowIndex);
                if (creatorRowIndex != -1)
                    creatorRow = sheet.CreateRow(creatorRowIndex);

                #region 单元格样式定义
                //列标题单元格样式设定
                ICellStyle titleStyle = wb.CreateCellStyle();
                titleStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                titleStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                titleStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                titleStyle.VerticalAlignment = VerticalAlignment.Center;
                font = wb.CreateFont();
                font.IsBold = true;
                titleStyle.SetFont(font);

                //“序”列标题样式设定
                ICellStyle idxTitleStyle = wb.CreateCellStyle();
                idxTitleStyle.CloneStyleFrom(titleStyle);
                idxTitleStyle.Alignment = HorizontalAlignment.Center;

                //文件标题单元格样式设定
                ICellStyle headerStyle = wb.CreateCellStyle();
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.VerticalAlignment = VerticalAlignment.Center;
                font = wb.CreateFont();
                font.FontHeightInPoints = 12;
                font.IsBold = true;
                headerStyle.SetFont(font);

                //制表人单元格样式设定
                ICellStyle userStyle = wb.CreateCellStyle();
                userStyle.Alignment = HorizontalAlignment.Right;
                userStyle.VerticalAlignment = VerticalAlignment.Center;

                //单元格样式设定
                ICellStyle cellStyle = wb.CreateCellStyle();
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;

                //数字单元格样式设定
                ICellStyle numCellStyle = wb.CreateCellStyle();
                numCellStyle.CloneStyleFrom(cellStyle);
                numCellStyle.Alignment = HorizontalAlignment.Right;

                //“序”列单元格样式设定
                ICellStyle idxCellStyle = wb.CreateCellStyle();
                idxCellStyle.CloneStyleFrom(cellStyle);
                idxCellStyle.Alignment = HorizontalAlignment.Center;

                //日期单元格样式设定
                ICellStyle dateCellStyle = wb.CreateCellStyle();
                dateCellStyle.CloneStyleFrom(cellStyle);
                dateCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d;@");

                //日期时间单元格样式设定
                ICellStyle timeCellStyle = wb.CreateCellStyle();
                timeCellStyle.CloneStyleFrom(cellStyle);
                timeCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d h:mm;@");

                //千分位单元格样式设定
                ICellStyle qCellStyle = wb.CreateCellStyle();
                qCellStyle.CloneStyleFrom(cellStyle);
                qCellStyle.Alignment = HorizontalAlignment.Right;
                qCellStyle.DataFormat = fmt.GetFormat("#,##0_ ;@");

                //小数点、千分位单元格样式设定
                Dictionary<string, ICellStyle> cstyles = new Dictionary<string, ICellStyle>();
                ICellStyle cstyle = null;
                #endregion

                //输出列标题
                row = sheet.CreateRow(titleRowIndex);
                row.HeightInPoints = DEF_ROW_HEIGHT;

                foreach (DataColumn col in dt.Columns)
                {
                    cell = row.CreateCell(c++);
                    cell.SetCellValue(col.ColumnName);
                    cell.CellStyle = col.ColumnName == "序" ? idxTitleStyle : titleStyle;

                    columnWidth = col.ExtendedProperties.ContainsKey("width")
                                      ? (int)col.ExtendedProperties["width"]
                                      : 100;
                    sheet.SetColumnWidth(c - 1, (int)(Math.Ceiling(columnWidth / charWidth) + 0.72) * 256);

                    if (headerRow != null)
                        headerRow.CreateCell(c - 1);
                    if (dateRow != null)
                        dateRow.CreateCell(c - 1);
                    if (sumRow != null)
                        sumRow.CreateCell(c - 1);
                    if (creatorRow != null)
                        creatorRow.CreateCell(c - 1);

                    //定义数字列单元格样式
                    switch (col.DataType.Name)
                    {
                        case "Single":
                        case "Double":
                        case "Decimal":
                            decimalPlaces = col.ExtendedProperties.ContainsKey("dots")
                                                ? (int)col.ExtendedProperties["dots"]
                                                : 2;
                            qian = col.ExtendedProperties.ContainsKey("k")
                                       ? (bool)col.ExtendedProperties["k"]
                                       : false;

                            if (decimalPlaces > 0 && !qian)
                            {
                                cstyle = wb.CreateCellStyle();
                                cstyle.CloneStyleFrom(qCellStyle);
                                cstyle.DataFormat = fmt.GetFormat("0." + string.Empty.PadLeft(decimalPlaces, '0') + "_ ;@");
                            }
                            else if (decimalPlaces == 0 && qian)
                            {
                                cstyle = wb.CreateCellStyle();
                                cstyle.CloneStyleFrom(qCellStyle);
                            }
                            else if (decimalPlaces > 0 && qian)
                            {
                                cstyle = wb.CreateCellStyle();
                                cstyle.CloneStyleFrom(qCellStyle);
                                cstyle.DataFormat = fmt.GetFormat("#,##0." + string.Empty.PadLeft(decimalPlaces, '0') + "_ ;@");
                            }

                            cstyles.Add(col.ColumnName, cstyle);
                            break;
                        default:
                            break;
                    }
                }
                //输出文件标题
                if (headerRow != null)
                {
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(headerRowIndex, headerRowIndex, 0,
                                                                            dt.Columns.Count - 1));
                    cell = headerRow.GetCell(0);
                    cell.SetCellValue(header);
                    cell.CellStyle = headerStyle;
                    headerRow.HeightInPoints = 26;
                }
                //输出日期
                if (dateRow != null)
                {
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(dateRowIndex, dateRowIndex, 0,
                                                                            dt.Columns.Count - 1));
                    cell = dateRow.GetCell(0);
                    cell.SetCellValue("日期：" + DateTime.Today.ToString("yyyy-MM-dd"));
                    cell.CellStyle = userStyle;
                    dateRow.HeightInPoints = DEF_ROW_HEIGHT;
                }
                //输出制表人
                if (creatorRow != null)
                {
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(creatorRowIndex, creatorRowIndex, 0,
                                                                            dt.Columns.Count - 1));
                    cell = creatorRow.GetCell(0);
                    cell.SetCellValue("制表人：" + creator);
                    cell.CellStyle = userStyle;
                    creatorRow.HeightInPoints = DEF_ROW_HEIGHT;
                }

                r = titleRowIndex + 1;
                //输出查询结果
                foreach (DataRow dr in dt.Rows)
                {
                    row = sheet.CreateRow(r++);
                    row.HeightInPoints = DEF_ROW_HEIGHT;
                    c = 0;

                    foreach (DataColumn col in dt.Columns)
                    {
                        cell = row.CreateCell(c++);

                        switch (col.DataType.Name)
                        {
                            case "Boolean":
                                cell.CellStyle = cellStyle;
                                cell.SetCellValue(Equals(dr[col.ColumnName], true) ? "是" : "否");
                                break;
                            case "DateTime":
                                isDate = col.ExtendedProperties.ContainsKey("isdate")
                                             ? (bool)col.ExtendedProperties["isdate"]
                                             : false;

                                cell.CellStyle = isDate ? dateCellStyle : timeCellStyle;
                                cell.SetCellValue(dr[col.ColumnName] as string);
                                break;
                            case "Int16":
                            case "Int32":
                            case "Int64":
                                qian = col.ExtendedProperties.ContainsKey("k")
                                               ? (bool)col.ExtendedProperties["k"]
                                               : false;

                                cell.CellStyle = col.ColumnName == "序"
                                                     ? idxCellStyle
                                                     : qian ? qCellStyle : numCellStyle;
                                cell.SetCellValue(Convert.ToInt64(dr[col.ColumnName]));
                                break;
                            case "Single":
                            case "Double":
                            case "Decimal":
                                cell.CellStyle = cstyles[col.ColumnName];
                                cell.SetCellValue((Convert.ToDouble(dr[col.ColumnName])));
                                break;
                            default:
                                cell.CellStyle = cellStyle;
                                cell.SetCellValue(dr[col.ColumnName] as string);
                                break;
                        }
                    }
                }
                //合计
                if (sumRow != null)
                {
                    sumRow.HeightInPoints = DEF_ROW_HEIGHT;

                    for (c = 0; c < dt.Columns.Count; c++)
                    {
                        cell = sumRow.GetCell(c);
                        cell.CellStyle = cellStyle;

                        if (sumColumns.Contains(c) == false)
                            continue;

                        cell.SetCellFormula(string.Format("SUM({0}:{1})",
                                                          GetCellName(c, titleRowIndex + 1),
                                                          GetCellName(c, titleRowIndex + dt.Rows.Count)));
                    }
                }

                wb.Write(fs);
                len = fs.Length;
            }


            return null; 
        }

        /// <summary>
        /// 获取单元格的显示名称，格式如A1,B2
        /// </summary>
        /// <param name="columnIdx">单元格列号</param>
        /// <param name="rowIdx">单元格行号</param>
        /// <returns></returns>
        public static string GetCellName(int columnIdx, int rowIdx)
        {
            int[] maxs = new[] { 26, 26 * 26 + 26, 26 * 26 * 26 + (26 * 26 + 26) + 26 };
            int col = columnIdx + 1;
            int row = rowIdx + 1;

            if (col > maxs[2])
                throw new Exception("列序号不正确，超出最大值");

            int alphaCount = 1;

            foreach (int m in maxs)
            {
                if (m < col)
                    alphaCount++;
            }

            switch (alphaCount)
            {
                case 1:
                    return (char)(col + 64) + "" + row;
                case 2:
                    return (char)((col / 26) + 64) + "" + (char)((col % 26) + 64) + row;
                case 3:
                    return (char)((col / 26 / 26) + 64) + "" + (char)(((col - col / 26 / 26 * 26 * 26) / 26) + 64) + "" + (char)((col % 26) + 64) + row;
            }

            return "Unkown";
        }

    }


}
