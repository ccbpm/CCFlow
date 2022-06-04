using BP.DA;
using BP.WF.HttpHandler;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin_MethodDocSys : DirectoryPageBase
    {
        #region 属性.
        public string GroupID
        {
            get
            {
                string str = this.GetRequestVal("GroupID");
                return str;
            }
        }
        public string Name
        {
            get
            {
                string str = this.GetRequestVal("Name");
                return str;
            }
        }
        #endregion 属性.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin_MethodDocSys()
        {

        }
        /// <summary>
        /// 移动方法.
        /// </summary>
        /// <returns></returns>
        public string MethodDocSysParas_Mover()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                var enNo = ens[i];
                string sql = "UPDATE Sys_MapAttr SET Idx=" + i + " WHERE MyPK='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "方法顺序移动成功..";
        }

        /// <summary>
        /// 获得js,sql内容.
        /// </summary>
        /// <returns></returns>
        public string MethodDocSys_GetScript()
        {
            BP.CCBill.Sys.Func en = new BP.CCBill.Sys.Func(this.No);
            int type = this.GetRequestValInt("TypeOfFunc");
            if (type == 0)
                return en.MethodDoc_SQL;

            if (type == 1)
                return en.MethodDoc_JavaScript;

            if (type == 2)
                return en.MethodDoc_Url;

            return "err@没有判断的类型.";
        }
        /// <summary>
        /// 保存脚本
        /// </summary>
        /// <returns></returns>
        public string MethodDocSys_SaveScript()
        {
            BP.CCBill.Sys.Func en = new BP.CCBill.Sys.Func(this.No);

            int type = this.GetRequestValInt("TypeOfFunc");
            string doc = this.GetRequestVal("doc");
            string funcstr = this.GetRequestVal("funcstr");
            //sql模式.
            if (type == 0)
            {
                doc = doc.Replace("/#", "+");
                doc = doc.Replace("/$", "-");
                en.MethodDoc_SQL = doc;
            }

            //script.
            if (type == 1)
            {
                en.MethodDoc_JavaScript = doc;

                string path =  BP.Difference.SystemConfig.PathOfDataUser + "JSLibData/Method/";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);
                //写入文件.
                string file = path + en.No + ".js";
                funcstr = funcstr.Replace("/#", "+");
                funcstr = funcstr.Replace("/$", "-");
                DataType.WriteFile(file, funcstr);
            }

            //url. 、 methond.
            if (type == 2 || type == 3)
                en.MethodDoc_Url = this.GetRequestVal("Tag1");

            en.MethodDocTypeOfFunc = type;
            en.DirectUpdate();

            return "保存成功.";
        }


    }
}
