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
    public class WF_CommTS : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 参数
        public string MyPK
        {
            get
            {
                return this.GetRequestVal("MyPK");
            }
        }
        public string No
        {
            get
            {
                return this.GetRequestVal("No");
            }
        }
        public string TSClassID
        {
            get
            {
                return this.GetRequestVal("TSClassID");
            }
        }
        public string KVs
        {
            get
            {
                return this.GetRequestVal("KVs");
            }
        }
        public string Map
        {
            get
            {
                return this.GetRequestVal("Map");
            }
        }
        #endregion 参数

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CommTS()
        {

        }
        public string EntityNoName_Init()
        {

            string classID = "";

            return "0";
        }
        public string EntityNoName_Insert()
        {
            TSEntityNoName en = new TSEntityNoName(this.TSClassID);

            // 获得数据.
            //en.Copy();

            var num = en.Insert();

            return num.ToString();
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <returns></returns>
        public string EntityNoName_Update()
        {
            TSEntityNoName en = new TSEntityNoName(this.TSClassID, this.No);
            //en.Copy(this.KVs);
            en.No = this.No;
            return en.Update().ToString();
        }
        public string EntityNoName_Delete()
        {
            TSEntityNoName en = new TSEntityNoName(this.TSClassID, this.No);
            //en.Copy(this.KVs);
            en.No = this.No;
            return en.Delete().ToString();
        }
    }
}
