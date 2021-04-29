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
    public class WF_Admin_FoolFormDesigner_StyletDfine : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner_StyletDfine()
        {
        }

        #region GloValStyles.htm
        public string GloValStyles_PinYin()
        {
            string name = this.GetRequestVal("TB_Name");

            //表单No长度最大100，因有前缀CCFrm_，因此此处设置最大94，added by liuxc,2017-9-25
            string str = BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true, true, 94);

            GloVar en = new GloVar();
            en.No = str;
            if (en.RetrieveFromDBSources() == 0)
                return str;

            return "err@标签:" + str + "已经被使用.";
        }
        public string GloValStyles_Init()
        {
            string val = this.GetRequestVal("CSS");
            if (DataType.IsNullOrEmpty(val) == true)
                return "";

            BP.Sys.GloVar en = new GloVar(val);

            //生成风格文件.
            string docs = GenerStyleDocs(en);

            //内容.
            docs = docs.Replace(en.No, "GloValsTemp");

            //保存一个临时文件,
            string path = SystemConfig.PathOfDataUser + "Style\\GloVarsCSSTemp.css";
            BP.DA.DataType.SaveAsFile(path, docs);

            return "风格文件已经生成:" + path;
        }
        /// <summary>
        /// 应用.
        /// </summary>
        /// <returns></returns>
        public string GloValStyles_App()
        {
            BP.Sys.GloVars ens = new GloVars();
            ens.Retrieve(GloVarAttr.GroupKey, "CSS");

            string html = "";
            foreach (BP.Sys.GloVar item in ens)
            {
                //生成风格文件.
                html += GenerStyleDocs(item);
            }

            //保存一个临时文件,
            string path = SystemConfig.PathOfDataUser + "Style\\GloVarsCSS.css";
            BP.DA.DataType.SaveAsFile(path, html);

            return "执行成功.";
        }
        #endregion

        #region Default.htm 风格设计页面..
        /// <summary>
        /// 保存为模版.
        /// </summary>
        /// <returns></returns>
        public string Default_SaveAsTemplate()
        {
            try
            {
                BP.Sys.GloVars ens = new GloVars();
                ens.Retrieve("GroupKey", "FoolFrmStyle", "Idx");

                string myName = this.GetRequestVal("TemplateName");

                if (DataType.IsNullOrEmpty(myName) == true)
                    myName = DateTime.Now.ToString("MM月dd日HH时mm分ss秒");

                string path = SystemConfig.PathOfDataUser + "\\Style\\TemplateFoolFrm\\" + myName + ".xml";
                ens.SaveToXml(path);

                return "模版创建成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 模版选择
        /// </summary>
        /// <returns></returns>
        public string Default_Template_Selected()
        {
            return Default_Selected_Ext(false);
        }
        /// <summary>
        /// 初始化表单风格
        /// </summary>
        /// <returns></returns>
        public string Default_GenerGloVars()
        {
            //获得标准的配置文件,用于比较缺少或者删除的标记.
            string path = SystemConfig.PathOfWebApp + "\\WF\\Admin\\FoolFormDesigner\\StyletDfine\\DefaultStyle.xml";
            System.Data.DataSet ds = new System.Data.DataSet();
            ds.ReadXml(path);
            DataTable dt = ds.Tables[0];

            GloVars ens = new GloVars();
            ens.Retrieve("GroupKey", "FoolFrmStyle", "Idx");

            #region  检查是否有新增的标签,如果有就 insert 一个。
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                string no = dr[0].ToString();
                string name = dr[1].ToString();
                string val = dr[2].ToString();
                if (ens.Contains(no) == false)
                {
                    GloVar myen = new GloVar();
                    myen.No = no;
                    myen.Name = name;
                    myen.Val = val;
                    myen.GroupKey = "FoolFrmStyle";
                    myen.Idx = idx;
                    myen.Insert();
                    ens.AddEntity(myen);
                }
            }
            #endregion  检查是否有新增的标签,如果有就insert一个。

            #region  检查是否有 多余 的标签,如果有就 Delete .
            bool isChange = false;
            foreach (GloVar item in ens)
            {
                bool isHave = false;
                foreach (DataRow dr in dt.Rows)
                {
                    string no = dr[0].ToString();

                    if (item.No.Equals(no) == false)
                        continue;

                    isHave = true;
                }

                if (isHave == false)
                {
                    item.Delete();
                    isChange = true;
                }
            }

            //如果发生了变化,就重新查询.
            if (isChange == true)
                ens.Retrieve("GroupKey", "FoolFrmStyle", "Idx");
            #endregion  检查是否有 多余 的标签,如果有就Delete 。

            Default_App_Ext(ens, false);

            return ens.ToJson();
        }
        /// <summary>
        /// 应用
        /// </summary>
        /// <returns></returns>
        public string Default_App()
        {
            string docs = "";

            //查询出来所有的.
            BP.Sys.GloVars ens = new GloVars();
            ens.Retrieve("GroupKey", "FoolFrmStyle", "Idx");
            return Default_App_Ext(ens, true);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="isApp"></param>
        /// <returns></returns>
        public string Default_App_Ext(GloVars ens, bool isApp = false)
        {
            string docs = "";

            foreach (GloVar en in ens)
            {
                docs += GenerStyleDocs(en);
            }

            //保存.
            if (isApp == true)
            {
                string pathDefault = SystemConfig.PathOfDataUser + "Style\\FoolFrmStyle\\Default.css";
                BP.DA.DataType.SaveAsFile(pathDefault, docs);
            }

            //保存一个临时文件,
            string path = SystemConfig.PathOfDataUser + "Style\\FoolFrmStyle\\DefaultPreview.css";
            BP.DA.DataType.SaveAsFile(path, docs);

            return "info@风格文件已经生成:" + path;
        }
        private string GenerStyleDocs(GloVar en)
        {
            string docs = "";
            docs += "\t\n/* " + en.Name + " */";

            docs += "\t\n." + en.No;
            docs += "\t\n{ ";

            AtPara ap = new AtPara(en.Val);
            foreach (string item in ap.HisHT.Keys)
            {
                //特殊标记.
                if (item.Contains("_Temp") == true)
                    continue;

                docs += "\t\n " + item + ":" + ap.GetValStrByKey(item).Trim().Replace(" ", "") + ";";
            }
            docs += "\t\n}";
            return docs;
        }
        #endregion 风格设计页面..

        #region Template.htm 模版页面.
        /// <summary>
        /// 应用
        /// </summary>
        /// <returns></returns>
        public string Template_App()
        {
            string str = Default_Selected_Ext(true);
            return str;
        }
        /// <summary>
        /// 删除文件.
        /// </summary>
        /// <returns></returns>
        public string Default_Template_Delete()
        {
            string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Style\\TemplateFoolFrm\\";
            System.IO.File.Delete(path + this.Name);
            return "删除成功.";
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string Default_GenerTemplate()
        {
            string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Style\\TemplateFoolFrm\\";
            string[] fls = BP.Tools.BaseFileUtils.getFiles(path);


            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");

            foreach (string item in fls)
            {
                System.IO.FileInfo info = new System.IO.FileInfo(item);

                DataRow dr = dt.NewRow();
                string name = info.Name;
                if (name.Contains("Default") == true)
                    continue;

                if (name.Contains("Sys.xml") == true)
                    name = info.Name.Replace("Sys.xml", "[系统]");
                else
                    name = info.Name.Replace(".xml", "[自定义]");


                dr[1] = name;

                dr[0] = info.Name;
                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 模版选择.
        /// </summary>
        /// <returns></returns>
        public string Default_Selected_Ext(bool isApp)
        {
            string filePath = BP.Sys.SystemConfig.PathOfDataUser + "\\Style\\TemplateFoolFrm\\" + this.Name;

            DataSet ds = new DataSet();
            ds.ReadXml(filePath);

            DataTable dt = ds.Tables[0];

            BP.Sys.GloVars ens = new GloVars();
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr["No"].ToString();
                string name = dr["Name"].ToString();
                string val = dr["Val"].ToString();

                idx++;
                BP.Sys.GloVar en = new GloVar();
                en.No = key;
                en.Name = name;
                en.GroupKey = "FoolFrmStyle";
                en.Val = val;
                en.Idx = idx;
                ens.AddEntity(en);
            }


            ens.Delete("GroupKey", "FoolFrmStyle");
            foreach (BP.Sys.GloVar en in ens)
                en.Insert();


            //生成临时文件.  如果是 isApp==true ,就生成正式的风格文件.
            Default_App_Ext(ens, isApp);

            return "执行成功.";
        }
        #endregion 模版页面.

    }
}
