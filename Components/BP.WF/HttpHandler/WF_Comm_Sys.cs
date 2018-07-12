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
using System.Collections;
using System.Net;
using System.Xml.Schema;
using System.Web.Services.Description;
using System.Linq;
using System.IO;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Comm_Sys : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm_Sys(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Comm_Sys()
        {
        }
        /// <summary>
        /// 函数库
        /// </summary>
        /// <returns></returns>
        public string SystemClass_FuncLib()
        {
            string expFileName = "all-wcprops,dir-prop-base,entries";
            string expDirName = ".svn";

            string pathDir = BP.Sys.SystemConfig.PathOfData + "\\JSLib\\";

            string html = "";
            html += "<fieldset>";
            html += "<legend>" + "系统自定义函数. 位置:" + pathDir + "</legend>";


            //.AddFieldSet();
            DirectoryInfo dir = new DirectoryInfo(pathDir);
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo mydir in dirs)
            {
                if (expDirName.Contains(mydir.Name))
                    continue;

                html += "事件名称" + mydir.Name;
                html += "<ul>";
                FileInfo[] fls = mydir.GetFiles();
                foreach (FileInfo fl in fls)
                {
                    if (expFileName.Contains(fl.Name))
                        continue;

                    html += "<li>" + fl.Name + "</li>";
                }
                html += "</ul>";
            }
            html += "</fieldset>";

            pathDir = BP.Sys.SystemConfig.PathOfDataUser + "\\JSLib\\";
            html += "<fieldset>";
            html += "<legend>" + "用户自定义函数. 位置:" + pathDir + "</legend>";

            dir = new DirectoryInfo(pathDir);
            dirs = dir.GetDirectories();
            foreach (DirectoryInfo mydir in dirs)
            {
                if (expDirName.Contains(mydir.Name))
                    continue;

                html += "事件名称" + mydir.Name;
                html += "<ul>";
                FileInfo[] fls = mydir.GetFiles();
                foreach (FileInfo fl in fls)
                {
                    if (expFileName.Contains(fl.Name))
                        continue;
                    html += "<li>" + fl.Name + "</li>";
                }
                html += "</ul>";
            }
            html += "</fieldset>";
            return html;
        }

        #region 系统实体属性.
        public string SystemClass_EnsCheck()
        {
            try
            {
                BP.En.Entity en = BP.En.ClassFactory.GetEn(this.EnName);
                BP.En.Map map = en.EnMap;
                en.CheckPhysicsTable();
                string msg = "";
                // string msg = "";
                string table = "";
                string sql = "";
                string sql1 = "";
                string sql2 = "";
                int COUNT1 = 0;
                int COUNT2 = 0;

                DataTable dt = new DataTable();
                Entity refen = null;
                foreach (Attr attr in map.Attrs)
                {
                    /**/
                    if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                    {
                        refen = ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        table = refen.EnMap.PhysicsTable;
                        sql1 = "SELECT COUNT(*) FROM " + table;

                        Attr pkAttr = refen.EnMap.GetAttrByKey(refen.PK);
                        sql2 = "SELECT COUNT( distinct " + pkAttr.Field + ") FROM " + table;

                        COUNT1 = DBAccess.RunSQLReturnValInt(sql1);
                        COUNT2 = DBAccess.RunSQLReturnValInt(sql2);

                        if (COUNT1 != COUNT2)
                        {
                            msg += "<BR>@关联表(" + refen.EnMap.EnDesc + ")主键不唯一，它会造成数据查询不准确或者意向不到的错误：<BR>sql1=" + sql1 + " <BR>sql2=" + sql2;
                            msg += "@SQL= SELECT * FROM (  select " + refen.PK + ",  COUNT(*) AS NUM  from " + table + " GROUP BY " + refen.PK + " ) WHERE NUM!=1";
                        }

                        sql = "SELECT " + attr.Field + " FROM " + map.PhysicsTable + " WHERE " + attr.Field + " NOT IN (SELECT " + pkAttr.Field + " FROM " + table + " )";
                        dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                            continue;
                        else
                            msg += "<BR>:有" + dt.Rows.Count + "个错误。" + attr.Desc + " sql= " + sql;
                    }
                    if (attr.MyFieldType == FieldType.PKEnum || attr.MyFieldType == FieldType.Enum)
                    {
                        sql = "SELECT " + attr.Field + " FROM " + map.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( select Intkey from sys_enum WHERE ENUMKEY='" + attr.UIBindKey + "' )";
                        dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                            continue;
                        else
                            msg += "<BR>:有" + dt.Rows.Count + "个错误。" + attr.Desc + " sql= " + sql;
                    }
                }

                // 检查pk是否一致。
                if (en.PKs.Length == 1)
                {
                    sql1 = "SELECT COUNT(*) FROM " + map.PhysicsTable;
                    COUNT1 = DBAccess.RunSQLReturnValInt(sql1);

                    Attr attrMyPK = en.EnMap.GetAttrByKey(en.PK);
                    sql2 = "SELECT COUNT(DISTINCT " + attrMyPK.Field + ") FROM " + map.PhysicsTable;
                    COUNT2 = DBAccess.RunSQLReturnValInt(sql2);
                    if (COUNT1 != COUNT2)
                    {
                        msg += "@物理表(" + map.EnDesc + ")中主键不唯一;它会造成数据查询不准确或者意向不到的错误：<BR>sql1=" + sql1 + " <BR>sql2=" + sql2;
                        msg += "@SQL= SELECT * FROM (  select " + en.PK + ",  COUNT(*) AS NUM  from " + map.PhysicsTable + " GROUP BY " + en.PK + " ) WHERE NUM!=1";
                    }
                }

                if (msg == "")
                    return map.EnDesc + ":数据体检成功,完全正确.";

                string info = map.EnDesc + ":数据体检信息：体检失败" + msg;
                return info;

            }catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string SystemClass_Fields()
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;

            BP.En.Map map = en.EnMap;
            en.CheckPhysicsTable();

            string html = "<table>";

            html += "<caption>数据结构" + map.EnDesc + "," + map.PhysicsTable + "</caption>";

            html += "<tr>";
            html += "<th>序号</th>";
            html += "<th>描述</th>";
            html += "<th>属性</th>";
            html += "<th>物理字段</th>";
            html += "<th>数据类型</th>";
            html += "<th>关系类型</th>";
            html += "<th>长度</th>";
            html += "<th>对应</th>";
            html += "<th>默认值</th>";
            html += "</tr>";

            int i = 0;
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                i++;
                html += "<tr>";
                html += "<td>" + i + "</td>";
                html += "<td>" + attr.Desc + "</td>";
                html += "<td>" + attr.Key + "</td>";
                html += "<td>" + attr.Field + "</td>";
                html += "<td>" + attr.MyDataTypeStr + "</td>";
                html += "<td>" + attr.MyFieldType.ToString() + "</td>";

                if (attr.MyDataType == DataType.AppBoolean
                    || attr.MyDataType == DataType.AppDouble
                    || attr.MyDataType == DataType.AppFloat
                    || attr.MyDataType == DataType.AppInt
                    || attr.MyDataType == DataType.AppMoney
                    )
                    html += "<td>无</td>";
                else
                    html += "<td>" + attr.MaxLength + "</td>";


                switch (attr.MyFieldType)
                {
                    case FieldType.Enum:
                    case FieldType.PKEnum:
                        try
                        {
                            SysEnums ses = new SysEnums(attr.UIBindKey);
                            string str = "";
                            foreach (SysEnum se in ses)
                            {
                                str += se.IntKey + "&nbsp;" + se.Lab + ",";
                            }
                            html += "<td>" + str + "</td>";
                        }
                        catch
                        {
                            html += "<td>未使用</td>";

                        }
                        break;
                    case FieldType.FK:
                    case FieldType.PKFK:
                        Entities myens = ClassFactory.GetEns(attr.UIBindKey);
                        html += "<td>表/视图:" + myens.GetNewEntity.EnMap.PhysicsTable + " 关联字段:" + attr.UIRefKeyValue + "," + attr.UIRefKeyText+"</td>";
                        break;
                    default:
                        html += "<td>无</td>";
                        break;
                }

                html += "<td>" + attr.DefaultVal.ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }

        public string SystemClass_Init()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("EnsName");
            dt.Columns.Add("Name");
            dt.Columns.Add("PTable");

            ArrayList al = null;
            al = BP.En.ClassFactory.GetObjects("BP.En.Entity");
            foreach (Object obj in al)
            {
                Entity en = null;
                try
                {
                    en = obj as Entity;
                    string s = en.EnDesc;
                    if (en == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                if (en.ToString() == null)
                    continue;


                DataRow dr = dt.NewRow();
               
                dr["No"] = en.ToString();
                try
                {
                    dr["EnsName"] = en.GetNewEntities.ToString();
                }
                catch
                {
                    dr["EnsName"] = en.ToString()+"s";
                }
                dr["Name"] = en.EnMap.EnDesc;
                dr["PTable"] = en.EnMap.PhysicsTable;
                dt.Rows.Add(dr);
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion


        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string sfno = this.GetRequestVal("sfno") ;
            SFTable sftable = null;
            DataTable dt = null;
            StringBuilder s = null;

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
        
        #region 数据源管理
        public string SFDBSrcNewGuide_GetList()
        {
            //SysEnums enums = new SysEnums(SFDBSrcAttr.DBSrcType);
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();

            return srcs.ToJson();
        }

        public string SFDBSrcNewGuide_LoadSrc()
        {
            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc();
            if (!string.IsNullOrWhiteSpace(this.GetRequestVal("No")))
                src = new SFDBSrc(No);
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            SysEnums enums = new SysEnums();
            enums.Retrieve(SysEnumAttr.EnumKey, SFDBSrcAttr.DBSrcType, SysEnumAttr.IntKey);
            ds.Tables.Add(enums.ToDataTableField("DBSrcType"));

            return BP.Tools.Json.ToJson(ds);
        }

        public string SFDBSrcNewGuide_SaveSrc()
        {
            SFDBSrc src = new SFDBSrc();
            src.No = this.GetRequestVal("TB_No");
            if (src.RetrieveFromDBSources() > 0 && this.GetRequestVal("NewOrEdit") == "New")
            {
                return ("已经存在数据源编号为“" + src.No + "”的数据源，编号不能重复！");
            }
            src.Name = this.GetRequestVal("TB_Name");
            src.DBSrcType = (DBSrcType)this.GetRequestValInt("DDL_DBSrcType");
            switch (src.DBSrcType)
            {
                case DBSrcType.SQLServer:
                case DBSrcType.Oracle:
                case DBSrcType.MySQL:
                case DBSrcType.Informix:
                    if (src.DBSrcType != DBSrcType.Oracle)
                        src.DBName = this.GetRequestVal("TB_DBName");
                    else
                        src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = this.GetRequestVal("TB_UserID");
                    src.Password = this.GetRequestVal("TB_PWword");
                    break;
                case DBSrcType.WebServices:
                    src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = string.Empty;
                    src.Password = string.Empty;
                    break;
                default:
                    break;
            }
            //测试是否连接成功，如果连接不成功，则不允许保存。
            string testResult = src.DoConn();

            if (testResult.IndexOf("连接配置成功") == -1)
            {
                return (testResult + ".保存失败！");
            }

            src.Save();

            return "保存成功..";
        }

        public string SFDBSrcNewGuide_DelSrc()
        {
            string no = this.GetRequestVal("No");

            //检验要删除的数据源是否有引用
            SFTables sfs = new SFTables();
            sfs.Retrieve(SFTableAttr.FK_SFDBSrc, no);

            if (sfs.Count > 0)
            {
                //Alert("当前数据源已经使用，不能删除！");
                return "当前数据源已经使用，不能删除！";
                //return;
            }
            SFDBSrc src = new SFDBSrc(no);
            src.Delete();
            return "删除成功..";
        }

        //javaScript 脚本上传
        public string javaScriptImp_Done()
        {
            HttpFileCollection files = context.Request.Files;
            if (files.Count == 0)
                return "err@请选择要上传的流程模版。";
            string fileName = files[0].FileName;
            string savePath = BP.Sys.SystemConfig.PathOfDataUser + "JSLibData" + "\\" + fileName;

            //存在文件则删除
            if (System.IO.Directory.Exists(savePath) == true)
                System.IO.Directory.Delete(savePath);

            files[0].SaveAs(savePath);

            return "脚本" + fileName + "导入成功";
        }

        /**
         * 获取已知目录下的文件列表
         * @return
         */
        public string javaScriptFiles(){
		String savePath = BP.Sys.SystemConfig.PathOfDataUser+"JSLibData";

         DirectoryInfo di = new DirectoryInfo(savePath);
        //找到该目录下的文件 
        FileInfo[] fileList = di.GetFiles();

	    if(fileList==null||fileList.Length==0)
	    	return "";
	    DataTable dt = new DataTable();
	    dt.Columns.Add("FileName");
	    dt.Columns.Add("ChangeTime");
	    foreach(FileInfo file in fileList){
	    	DataRow dr = dt.NewRow();
            dr["FileName"] = file.Name;
            dr["ChangeTime"] = file.LastAccessTime.ToString();
			
			dt.Rows.Add(dr);
	    }
	    return BP.Tools.Json.ToJson(dt);
			
	}
        #endregion        
    }


   
}
