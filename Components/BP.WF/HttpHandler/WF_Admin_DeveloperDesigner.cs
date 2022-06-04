using System.IO;
using System.Data;
using System.Web;
using BP.Sys;
using BP.DA;
using BP.CCBill;
using System.Text;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    public class WF_Admin_DevelopDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 执行父类的重写方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_DevelopDesigner()
        {

        }
        /// <summary>
        /// 表单初始化
        /// </summary>
        /// <returns></returns>
        public string Designer_Init()
        {
            //获取htmlfrom 信息.
            string htmlCode = DBAccess.GetBigTextFromDB("Sys_MapData", "No", this.FK_MapData,
                "HtmlTemplateFile");

            if (DataType.IsNullOrEmpty(htmlCode) == true)
                htmlCode = "<h3>请插入表单模板.</h3>";

            //把数据同步到DataUser/CCForm/HtmlTemplateFile/文件夹下
            string filePath =  BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HtmlTemplateFile/";
            if (Directory.Exists(filePath) == false)
                Directory.CreateDirectory(filePath);
            filePath = filePath + this.FK_MapData + ".htm";

            //写入到html 中
            DataType.WriteFile(filePath, htmlCode);
            return htmlCode;
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public string SaveForm()
        {
            //获取html代码
            string htmlCode = this.GetRequestVal("HtmlCode");
            if (DataType.IsNullOrEmpty(htmlCode) == true)
                return "err@表单内容不能为空.";

            if (htmlCode.Contains("err@") == true)
                return "err@错误" + htmlCode;

            htmlCode = HttpUtility.UrlDecode(htmlCode, Encoding.UTF8);
            
            return BP.WF.Dev2Interface.SaveDevelopForm(htmlCode,this.FK_MapData);

        }
        #endregion

        #region 插入模版.
        /// <summary>
        /// 获取开发者表单模板目录
        /// </summary>
        /// <returns></returns>
        public string Template_Init()
        {
            DataSet ds = new DataSet();
            string path =  BP.Difference.SystemConfig.PathOfDataUser + "Style/TemplateFoolDevelopDesigner/";
            //var tmps = new DirectoryInfo(path).GetFiles("*.htm");
            string[] files = System.IO.Directory.GetDirectories(path);//获取子文件夹
            //模版类型
            DataTable dt = new DataTable();
            dt.TableName = "dirs";
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            DataRow dr = dt.NewRow();
            //模版信息
            DataTable filesDt = new DataTable();
            filesDt.TableName = "temps";
            filesDt.Columns.Add("No");
            filesDt.Columns.Add("Name");
            filesDt.Columns.Add("Dir");
            DataRow tempdr = filesDt.NewRow();

            foreach (string item in files)
            {
                //模版分类
                dr = dt.NewRow();
                dr["No"] = item;
                dr["Name"] = System.IO.Path.GetFileName(item); ;
                dt.Rows.Add(dr);

                //获取模版
                string[] temps = System.IO.Directory.GetFiles(item, "*.htm");
                foreach (string temp in temps)
                {
                    tempdr = filesDt.NewRow();
                    tempdr["No"] = temp;
                    tempdr["Name"] = System.IO.Path.GetFileName(temp);
                    tempdr["Dir"] = System.IO.Path.GetFileName(item);
                    filesDt.Rows.Add(tempdr);
                }
            }

            ds.Tables.Add(dt);
            ds.Tables.Add(filesDt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 根据名称获取开发者表单文件内容
        /// </summary>
        /// <returns></returns>
        public string Template_GenerHtml()
        {
            var fileName = this.GetRequestVal("DevTempName");
            var fielDir= this.GetRequestVal("DevTempDir");
            string path =  BP.Difference.SystemConfig.PathOfDataUser + "Style/TemplateFoolDevelopDesigner/"+ fielDir+"/";

            string filePath = path + fileName;

            Stream stream = new FileStream(filePath, FileMode.Open);
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            StreamReader reader = new StreamReader(stream, encode);
            string strHtml = reader.ReadToEnd();

            reader.Close();
            stream.Close();
            return strHtml;
        }

        public string Template_Imp()
        {
            var files = HttpContextHelper.RequestFiles();  //context.Request.Files;
            if (files.Count == 0)
                return "err@请选择要上传的流程模版。";

            //设置文件名
            string fileNewName = System.IO.Path.GetFileName(files[0].FileName);// DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);

            //文件存放路径
            string filePath =  BP.Difference.SystemConfig.PathOfDataUser + "Style/TemplateFoolDevelopDesigner/" + "" + fileNewName;
            HttpContextHelper.UploadFile(files[0], filePath);

            Stream stream = new FileStream(filePath, FileMode.Open);
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            StreamReader reader = new StreamReader(stream, encode);
            string strHtml = reader.ReadToEnd();

            reader.Close();
            stream.Close();
            return strHtml;
        }
   
        #endregion 插入模版.

        public string Fields_Init()
        {
            string html = DBAccess.GetBigTextFromDB("Sys_MapData", "No",
                this.FrmID, "HtmlTemplateFile");
            return html;
        }

        /// <summary>
        /// 格式化html的文档.
        /// </summary>
        /// <returns></returns>
        public string Designer_FormatHtml()
        {
            string html = DBAccess.GetBigTextFromDB("Sys_MapData", "No",
                this.FrmID, "HtmlTemplateFile");

            return "替换成功.";
            //return html;
        }

        /// <summary>
        /// 表单重置
        /// </summary>
        /// <returns></returns>
        public string ResetFrm_Init()
        {
            //删除html
            string filePath =  BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HtmlTemplateFile/" + this.FK_MapData + ".htm";
            if (File.Exists(filePath) == true)
                File.Delete(filePath);

            //删除存储的html代码
            string sql = "UPDATE Sys_MapData SET HtmlTemplateFile='' WHERE No='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);
            //删除MapAttr中的数据
            sql = "Delete Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);
            //删除MapExt中的数据
            sql = "Delete Sys_MapExt WHERE FK_MapData='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);

            return "重置成功";
        }

        #region 复制表单
        
        public void DoCopyFrm()
        {
            string fromFrmID = GetRequestVal("FromFrmID");
            string toFrmID = GetRequestVal("ToFrmID");
            string toFrmName = GetRequestVal("ToFrmName");
            #region 原表单信息
            //表单信息
            MapData fromMap = new MapData(fromFrmID);
            //单据信息
            FrmBill fromBill = new FrmBill();
            fromBill.No = fromFrmID;
            int billCount = fromBill.RetrieveFromDBSources();
            //实体单据
            FrmDict fromDict = new FrmDict();
            fromDict.No = fromFrmID;
            int DictCount = fromDict.RetrieveFromDBSources();
            #endregion 原表单信息

            #region 复制表单
            MapData toMapData = new MapData();
            toMapData = fromMap;
            toMapData.No = toFrmID;
            toMapData.Name = toFrmName;
            toMapData.Insert();
            if (billCount != 0)
            {
                FrmBill toBill = new FrmBill();
                toBill = fromBill;
                toBill.No = toFrmID;
                toBill.Name = toFrmName;
                toBill.EntityType = EntityType.FrmBill;
                toBill.Update();
            }
            if (DictCount != 0)
            {
                FrmDict toDict = new FrmDict();
                toDict = fromDict;
                toDict.No = toFrmID;
                toDict.Name = toFrmName;
                toDict.EntityType = EntityType.FrmDict;
                toDict.Update();
            }
            #endregion 复制表单

            MapData.ImpMapData(toFrmID, BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(fromFrmID));

            //清空缓存
            toMapData.RepairMap();
            SystemConfig.DoClearCash();


        }
        #endregion 复制表单

    }
}
