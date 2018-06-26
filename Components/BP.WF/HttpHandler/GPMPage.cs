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
    /// 页面功能实体
    /// </summary>
    public class GPMPage : DirectoryPageBase
    {
        #region 签名.
        public string Siganture_Save()
        {
            HttpPostedFile f = context.Request.Files[0];

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            }

            try
            {
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                f.SaveAs(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg");
            // f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
            return "上传成功！";
        }
        #endregion

        
        #region 组织结构维护.
        /// <summary>
        /// 初始化组织结构维护.
        /// </summary>
        /// <returns></returns>
        public string Organization_Init()
        {
            //BP.GPM.Depts depts = new GPM.Depts();
            //depts.RetrieveAll();

            //return depts.ToJsonOfTree("0");

            return "";
        }

        /// <summary>
        /// 获取该部门的所有人员
        /// </summary>
        /// <returns></returns>
        /// 
        public string LoadDatagridDeptEmp_Init()
        {
            string deptNo = this.GetRequestVal("deptNo");
            if (string.IsNullOrEmpty(deptNo))
            {
                return "{ total: 0, rows: [] }";
            }
            string orderBy = this.GetRequestVal("orderBy");


            string searchText = this.GetRequestVal("searchText");
            if (!DataType.IsNullOrEmpty(searchText))
            {
                searchText.Trim();
            }
            string addQue = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                addQue = "  AND (pe.No like '%" + searchText + "%' or pe.Name like '%" + searchText + "%') ";
            }

            string pageNumber = this.GetRequestVal("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = this.GetRequestVal("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = "(select pe.*,pd.name FK_DutyText from port_emp pe left join port_duty pd on pd.no = pe.fk_duty where pe.no in (select fk_emp from Port_DeptEmp where fk_dept='" + deptNo + "') "
                + addQue + " ) dbSo ";


            return DBPaging(sql, iPageNumber, iPageSize, "No", orderBy);

        }

        /// <summary>
        /// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        /// </summary>
        /// <param name="dataSource">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">当前页数据条数</param>
        /// <param name="key">计算总行数需要</param>
        /// <param name="orderKey">排序字段</param>
        /// <returns></returns>
        public string DBPaging(string dataSource, int pageNumber, int pageSize, string key, string orderKey)
        {
            string sql = "";
            string orderByStr = "";

            if (!string.IsNullOrEmpty(orderKey))
                orderByStr = " ORDER BY " + orderKey;

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    int beginIndex = (pageNumber - 1) * pageSize + 1;
                    int endIndex = pageNumber * pageSize;

                    sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
                        "FROM (SELECT * FROM  " + dataSource + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT TOP " + pageSize + " * FROM " + dataSource + " WHERE " + key + " NOT IN  ("
                    + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + dataSource + " )" + orderByStr;
                    break;
                case DBType.MySQL:
                    pageNumber -= 1;
                    sql = "select * from  " + dataSource + orderByStr + " limit " + pageNumber + "," + pageSize;
                    break;
                default:
                    throw new Exception("暂不支持您的数据库类型.");
            }

            DataTable DTable = DBAccess.RunSQLReturnTable(sql);

            int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + dataSource);

            return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        }
        #endregion




      //  #endregion 组织结构维护.



        public string StationToDeptEmp_Init()
        {
            return "";
        }

        /// <summary>
        /// 处理系统编辑菜单.
        /// </summary>
        /// <returns></returns>
        public string AppMenu_Init()
        {
            //BP.GPM.App app = new BP.GPM.App();
            //app.No = "CCFlowBPM";
            //if (app.RetrieveFromDBSources() == 0)
            //{
            //    BP.GPM.App.InitBPMMenu();
            //    app.Retrieve();
            //}
            return "";
        }

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public GPMPage(HttpContext mycontext)
        {
            this.context = mycontext;
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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region xxx 界面 .
        #endregion xxx 界面方法.

    }
}
