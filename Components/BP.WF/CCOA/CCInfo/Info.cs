using System;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;
using Org.BouncyCastle.Asn1.Mozilla;
using System.IO;

namespace BP.CCOA.CCInfo
{
    /// <summary>
    /// 信息 属性
    /// </summary>
    public class InfoAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 信息内容
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";

        public const string RecDeptNo = "RecDeptNo";
        public const string RecDeptName = "RecDeptName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        /// <summary>
        /// 读取人数
        /// </summary>
        public const string ReadTimes = "ReadTimes";
        /// <summary>
        /// 读取人
        /// </summary>
        public const string Reader = "Reader";
        /// <summary>
        /// 状态
        /// </summary>
        public const string InfoSta = "InfoSta";

        public const string RelerName = "RelerName";
        public const string RelDeptName = "RelDeptName";

        public const string InfoType = "InfoType";
        public const string InfoPRI = "InfoPRI";

    }
    /// <summary>
    /// 信息
    /// </summary>
    public class Info : EntityNoName
    {

        #region 基本属性
        public string Docs
        {
            get
            {
                return this.GetValStrByKey(InfoAttr.Docs);
            }
            set
            {
                this.SetValByKey(InfoAttr.Docs, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStrByKey(InfoAttr.Rec);
            }
            set
            {
                this.SetValByKey(InfoAttr.Rec, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStrByKey(InfoAttr.RecName);
            }
            set
            {
                this.SetValByKey(InfoAttr.RecName, value);
            }
        }
        public string RecDeptNo
        {
            get
            {
                return this.GetValStrByKey(InfoAttr.RecDeptNo);
            }
            set
            {
                this.SetValByKey(InfoAttr.RecDeptNo, value);
            }
        }
        public string RecDeptName
        {
            get
            {
                return this.GetValStrByKey(InfoAttr.RecDeptName);
            }
            set
            {
                this.SetValByKey(InfoAttr.RecDeptName, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;

                ////if (WebUser.IsAdmin)
                ////{
                ////    uac.IsUpdate = true;
                ////    return uac;
                ////}
                //return base.HisUAC;
            }
        }
        /// <summary>
        /// 信息
        /// </summary>
        public Info()
        {
        }
        public Info(string no)
        {
            this.SetValByKey(InfoAttr.No, no);
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_Info", "信息");

                map.AddTBStringPK(InfoAttr.No, null, "编号", false, true, 1, 59, 59);
                map.AddTBString(InfoAttr.Name, null, "标题", true, false, 0, 300, 10, true);

                map.AddTBStringDoc("Docs", "Docs", null, "内容", true, false, 0, 5000, 20, true, true);

                map.AddDDLSysEnum(InfoAttr.InfoPRI, 0, "重要性", true, true, "InfoPRI", "@0=普通@1=紧急@2=火急");
                map.AddDDLEntities(InfoAttr.InfoType, null, "类型", new InfoTypes(), true);
                map.AddDDLSysEnum(InfoAttr.InfoSta, 0, "状态", true, true, InfoAttr.InfoSta, "@0=发布中@1=禁用");


                map.AddTBString(InfoAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(InfoAttr.RecName, null, "记录人", true, true, 0, 100, 10, false);
                map.AddTBString(InfoAttr.RecDeptNo, null, "记录人部门", false, false, 0, 100, 10, false);


                map.AddTBString(InfoAttr.RelerName, null, "发布人", true, false, 0, 100, 10, false);
                map.AddTBString(InfoAttr.RelDeptName, null, "发布单位", true, false, 0, 100, 10, false);

                map.AddTBDateTime(InfoAttr.RDT, null, "发布日期", true, true);
                map.AddTBString(InfoAttr.NianYue, null, "隶属年月", false, false, 0, 10, 10);

                map.AddTBInt(InfoAttr.ReadTimes, 0, "读取次数", true, true);
                map.AddTBStringDoc(InfoAttr.Reader, null, "读取人", false, false, false);

                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddTBString(InfoAttr.OrgNo, null, "组织", true, true, 0, 100, 10);

                //增加附件.
                map.AddMyFile();


                #region 设置查询条件.
                map.DTSearchKey = InfoAttr.RDT;
                map.DTSearchWay = DTSearchWay.ByDate;
                map.DTSearchLabel = "发布日期";

                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddHidden("OrgNo", "=", BP.Web.WebUser.OrgNo);

                map.AddSearchAttr(InfoAttr.InfoSta);
                map.AddSearchAttr(InfoAttr.InfoType);
                #endregion 设置查询条件.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        public string DoRead()
        {
            string reader = this.GetValStrByKey("Reader");
            if (reader.Contains(WebUser.No + ",") == false)
                return "";

            reader += "@" + WebUser.Name + ",";
            this.SetValByKey("Reader", reader);
            int t = this.GetValIntByKey(InfoAttr.ReadTimes);
            this.SetValByKey("ReadTimes", t + 1);
            try
            {
                this.Update();
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteInfo("读取人数太多." + ex.Message);
            }
            return "";
        }
        protected override bool beforeInsert()
        {

            this.SetValByKey(InfoAttr.No, DBAccess.GenerGUID());

            this.SetValByKey(InfoAttr.Rec, BP.Web.WebUser.No);
            this.SetValByKey(InfoAttr.RecName, BP.Web.WebUser.Name);
            this.SetValByKey(InfoAttr.RecDeptNo, BP.Web.WebUser.FK_Dept);

            this.SetValByKey(InfoAttr.RDT, DataType.CurrentDateTime); //记录日期.
            this.SetValByKey(InfoAttr.NianYue, DataType.CurrentYearMonth);//隶属年月.

            this.SetValByKey(InfoAttr.RelerName, BP.Web.WebUser.Name);
            this.SetValByKey(InfoAttr.RelDeptName, BP.Web.WebUser.FK_DeptName);

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.SetValByKey(InfoAttr.OrgNo, BP.Web.WebUser.OrgNo);

            this.No = DBAccess.GenerGUID();


            return base.beforeInsert();
        }
        #endregion 执行方法.

        
    }
    /// <summary>
    /// 信息 s
    /// </summary>
    public class Infos : EntitiesNoName
    {
        #region 构造函数.
        /// <summary>
        /// 信息
        /// </summary>
        public Infos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Info();
            }
        }
        #endregion 构造函数.

        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }

        public string Init_Data()
        {
            string path = @"C:\\Users\\zhoup\\Documents\\WXWork\\1688851964626505\\WeDrive\\济南泉亿信息技术有限公司\\软文";
            string[] strs = System.IO.Directory.GetDirectories(path);
            foreach (string dirPath in strs)
            {
                string[] files = System.IO.Directory.GetFiles(dirPath);
                foreach (string filePath in files)
                {
                    System.IO.FileInfo finfo = new FileInfo(filePath);
                    if (finfo.Name.Contains("WeDrive") == true)
                        continue;

                    BP.CCOA.CCInfo.Info en = new BP.CCOA.CCInfo.Info();
                    en.Name = finfo.Name;
                    en.No = DBAccess.GenerGUID();
                    if (en.IsExit("Name", en.Name) == true)
                        continue;

                    en.Docs = finfo.Name;
                    en.SetValByKey("MyFileName", en.Name);
                    en.SetValByKey("MyFileExt", finfo.Extension.Replace(".", ""));

                    string file = "/DataUser/TS.CCOA.CCInfo.Info/" + en.No + "" + finfo.Extension;
                    en.SetValByKey("WebPath", file);
                    en.SetValByKey("MyFileSize", finfo.Length.ToString());
                    en.SetValByKey("InfoSrcType", 1);

                    if (DataType.IsImgExt(finfo.Extension) == true)
                    {
                        string html = "<p>";
                        html += "<img src='http://81.69.38.157:8090:" + file + "' alt='" + en.Name + "' />";
                        html += "</p>";
                        en.Docs = html;
                    }

                    //复制文件.
                    System.IO.File.Copy(filePath, BP.Difference.SystemConfig.PathOfDataUser + "/TS.CCOA.CCInfo.Info/" + en.No + "" + finfo.Extension);
                    en.Insert();
                }
            }
            return "info@执行成功.";
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Info> ToJavaList()
        {
            return (System.Collections.Generic.IList<Info>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Info> Tolist()
        {
            System.Collections.Generic.List<Info> list = new System.Collections.Generic.List<Info>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Info)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
