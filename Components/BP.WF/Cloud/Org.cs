using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Port;

namespace BP.Cloud
{
    /// <summary>
    /// 组织 属性
    /// </summary>
    public class OrgAttr : BP.En.EntityNoNameAttr
    {
        /// <summary>
        /// 注册的IP
        /// </summary>
        public const string RegIP = "RegIP";
        /// <summary>
        /// 状态
        /// </summary>
        public const string OrgSta = "OrgSta";

        #region 基本属性
        /// <summary>
        /// 微信分配的永久的qiye ID.
        /// </summary>
        public const string CorpID = "CorpID";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_HY = "FK_HY";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// Addr
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 简称
        /// </summary>
        public const string NameFull = "NameFull";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Adminer = "Adminer";
        /// <summary>
        /// 管理员名称
        /// </summary>
        public const string AdminerName = "AdminerName";
        /// <summary>
        /// 注册来源
        /// </summary>
        public const string UrlFrom = "UrlFrom";
        public const string DB = "DB";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 拼音
        /// </summary>
        public const string PinYin = "PinYin";
        /// <summary>
        /// JSONOfTongJi
        /// </summary>
        public const string JSONOfTongJi = "JSONOfTongJi";
        /// <summary>
        /// QQ号
        /// </summary>
        public const string QQ = "QQ";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 注册来源
        /// </summary>
        public const string RegFrom = "RegFrom";
        /// <summary>
        /// 使用状态
        /// </summary>
        public const string UseSta = "UseSta";
        /// <summary>
        /// 停用日期
        /// </summary>
        public const string DTEnd = "DTEnd";
        /// <summary>
        /// 启用日期
        /// </summary>
        public const string DTReg = "DTReg";

        /// <summary>
        /// 
        /// </summary>
        public const string RDT = "RDT";


        /// <summary>
        /// 授权方（企业）access_token,最长为512字节
        /// </summary>
        public const string AccessToken = "AccessToken";
        /// <summary>
        /// 授权方（企业）access_token过期时间，有效期2小时
        /// </summary>
        public const string AccessTokenExpiresIn = "AccessTokenExpiresIn";
        /// <summary>
        /// 企业微信永久授权码
        /// </summary>
        public const string PermanentCode = "PermanentCode";

        /// <summary>
        /// 授权方企业方形头像
        /// </summary>
        public const string CorpSquareLogoUrl = "CorpSquareLogoUrl";
        /// <summary>
        /// 授权方企业圆形头像
        /// </summary>
        public const string CorpRoundLogoUrl = "CorpRoundLogoUrl";
        /// 授权方企业用户规模
        /// </summary>
        public const string CorpUserMax = "CorpUserMax";
        /// <summary>
        /// 授权方企业应用数上限
        /// </summary>
        public const string CorpAgentMax = "CorpAgentMax";
        /// <summary>
        /// 授权方企业的主体名称
        /// </summary>
        public const string CorpFullName = "CorpFullName";

        /// <summary>
        /// 企业类型，1. 企业; 2. 政府以及事业单位; 3. 其他组织, 4.团队号
        /// </summary>
        public const string SubjectType = "SubjectType";
        /// <summary>
        /// 认证到期时间
        /// </summary>
        public const string VerifiedEndTime = "VerifiedEndTime";
        /// <summary>
        /// 企业规模。当企业未设置该属性时，值为空
        /// </summary>
        public const string CorpScale = "CorpScale";
        /// <summary>
        /// 企业所属行业。当企业未设置该属性时，值为空
        /// </summary>
        public const string CorpIndustry = "CorpIndustry";
        /// <summary>
        /// 企业所属子行业。当企业未设置该属性时，值为空
        /// </summary>
        public const string CorpSubIndustry = "CorpSubIndustry";
        /// <summary>
        /// 企业所在地信息, 为空时表示未知
        /// </summary>
        public const string Location = "Location";
        /// <summary>
        /// 授权方应用方形头像
        /// </summary>
        public const string SquareLogoUrl = "SquareLogoUrl";
        /// <summary>
        /// 授权方应用圆形头像
        /// </summary>
        public const string RoundLogoUl = "RoundLogoUl";
        /// <summary>
        /// 授权方应用id
        /// </summary>
        public const string AgentId = "AgentId";
        /// <summary>
        /// 授权方应用名字
        /// </summary>
        public const string AgentName = "AgentName";

        /// <summary>
        /// 微信应用状态
        /// </summary>
        public const string WXUseSta = "WXUseSta";
        #endregion
    }
    /// <summary>
    /// 组织 的摘要说明。
    /// </summary>
    public class Org : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 该人员是否被禁用.
        /// </summary>
        public bool ItIsEnable
        {
            get
            {
                if (this.No == "admin")
                    return true;

                string sql = "SELECT COUNT(FK_Org) FROM Port_DeptOrgStation WHERE FK_Org='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_Org) FROM Port_DeptOrg WHERE FK_Org='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                return true;
            }
        }
        public string DB
        {
            get
            {
                return this.GetValStringByKey(OrgAttr.DB);
            }
            set
            {
                this.SetValByKey(OrgAttr.DB, value);
            }
        }
        /// <summary>
        /// 网站来源
        /// </summary>
        public string UrlFrom
        {
            get
            {
                return this.GetValStringByKey(OrgAttr.UrlFrom);
            }
            set
            {
                this.SetValByKey(OrgAttr.UrlFrom, value);
            }
        }
        /// <summary>
        /// 注册来源 0=网站， 1=微信, 2=钉钉.
        /// </summary>
        public int RegFrom
        {
            get
            {

                return this.GetValIntByKey(OrgAttr.RegFrom);
            }
            set
            {
                this.SetValByKey(OrgAttr.RegFrom, value);
            }
        }
        /// <summary>
        /// 注册的IP
        /// </summary>
        public string RegIP
        {
            get
            {

                return this.GetValStrByKey(OrgAttr.RegIP);
            }
            set
            {
                this.SetValByKey(OrgAttr.RegIP, value);
            }
        }
        public string DTReg
        {
            get
            {

                return this.GetValStrByKey(OrgAttr.DTReg);
            }
            set
            {
                this.SetValByKey(OrgAttr.DTReg, value);
            }
        }
        public string DTEnd
        {
            get
            {

                return this.GetValStrByKey(OrgAttr.DTEnd);
            }
            set
            {
                this.SetValByKey(OrgAttr.DTEnd, value);
            }
        }
        public string Addr
        {
            get
            {

                return this.GetValStrByKey(OrgAttr.Addr);
            }
            set
            {
                this.SetValByKey(OrgAttr.Addr, value);
            }
        }
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.GUID);
            }

        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgAttr.Adminer, value);
            }
        }
        public string AdminerName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AdminerName);
            }
            set
            {
                this.SetValByKey(OrgAttr.AdminerName, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string NameFull
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.NameFull);
            }
            set
            {
                this.SetValByKey(OrgAttr.NameFull, value);
            }
        }
        /// <summary>
        /// 统计用的JSON
        /// </summary>
        public string JSONOfTongJi
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.JSONOfTongJi);
            }
            set
            {
                this.SetValByKey(OrgAttr.JSONOfTongJi, value);
            }
        }
        /// <summary>
        /// 注册年月
        /// </summary>
        public string FK_HY
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.FK_HY);
            }
            set
            {
                this.SetValByKey(OrgAttr.FK_HY, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpAttr.PinYin, value);
            }
        }
        /// <summary>
        /// 使用状态0=未安装,1=使用中,2=卸载
        /// </summary>
        public int WXUseSta
        {
            get
            {
                return this.GetValIntByKey(OrgAttr.WXUseSta);
            }
            set
            {
                this.SetValByKey(OrgAttr.WXUseSta, value);
            }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string CorpID
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpID);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpID, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.RDT);
            }
            set
            {
                this.SetValByKey(OrgAttr.RDT, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AccessToken
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AccessToken);
            }
            set
            {
                this.SetValByKey(OrgAttr.AccessToken, value);
            }
        }
        public string AccessTokenExpiresIn
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AccessTokenExpiresIn);
            }
            set
            {
                this.SetValByKey(OrgAttr.AccessTokenExpiresIn, value);
            }
        }

        public string PermanentCode
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.PermanentCode);
            }
            set
            {
                this.SetValByKey(OrgAttr.PermanentCode, value);
            }
        }
        public string AgentId
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AgentId);
            }
            set
            {
                this.SetValByKey(OrgAttr.AgentId, value);
            }
        }
        public string AgentName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AgentName);
            }
            set
            {
                this.SetValByKey(OrgAttr.AgentName, value);
            }
        }

        public string CorpSquareLogoUrl
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpSquareLogoUrl);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpSquareLogoUrl, value);
            }
        }

        public string CorpRoundLogoUrl
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpRoundLogoUrl);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpRoundLogoUrl, value);
            }
        }

        public string CorpUserMax
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpUserMax);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpUserMax, value);
            }
        }

        public string CorpAgentMax
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpAgentMax);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpAgentMax, value);
            }
        }

        public string CorpFullName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpFullName);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpFullName, value);
            }
        }

        public string SubjectType
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.SubjectType);
            }
            set
            {
                this.SetValByKey(OrgAttr.SubjectType, value);
            }
        }

        public string VerifiedEndTime
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.VerifiedEndTime);
            }
            set
            {
                this.SetValByKey(OrgAttr.VerifiedEndTime, value);
            }
        }

        public string CorpScale
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpScale);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpScale, value);
            }
        }

        public string CorpIndustry
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpIndustry);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpIndustry, value);
            }
        }

        public string CorpSubIndustry
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.CorpSubIndustry);
            }
            set
            {
                this.SetValByKey(OrgAttr.CorpSubIndustry, value);
            }
        }

        public string Location
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.Location);
            }
            set
            {
                this.SetValByKey(OrgAttr.Location, value);
            }
        }

        public string SquareLogoUrl
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.SquareLogoUrl);
            }
            set
            {
                this.SetValByKey(OrgAttr.SquareLogoUrl, value);
            }
        }
        public string RoundLogoUl
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.RoundLogoUl);
            }
            set
            {
                this.SetValByKey(OrgAttr.RoundLogoUl, value);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            //if (this.Pass == pass)
            //    return true;
            return false;
        }
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 组织
        /// </summary>
        public Org()
        {
        }
        public Org(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
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

                Map map = new Map("Port_Org", "组织");
                map.setEnType(EnType.App);
                map.IndexField = OrgAttr.FK_HY;

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgAttr.No, null, "账号OrgNo", true, false, 1, 50, 90);

                map.AddTBString(OrgAttr.CorpID, null, "CorpID", true, false, 1, 50, 90);

                map.AddTBString(OrgAttr.Name, null, "简称", true, false, 0, 200, 130);
                map.AddTBString(OrgAttr.NameFull, null, "全称", true, false, 0, 300, 400);

                map.AddTBString(OrgAttr.Adminer, null, "管理员帐号", true, true, 0, 300, 400);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员", true, true, 0, 300, 400);

                map.AddTBString(OrgAttr.Addr, null, "地址", true, false, 0, 300, 36);
                map.AddTBString(OrgAttr.GUID, null, "GUID", true, false, 0, 32, 36);

                map.AddTBDateTime(OrgAttr.DTReg, null, "起始日期", true, false);
                map.AddTBDateTime(OrgAttr.DTEnd, null, "停用日期", true, false);
                map.AddTBString(OrgAttr.RegIP, null, "注册的IP", true, false, 0, 30, 20);
                map.AddTBDateTime(OrgAttr.RDT, null, "注册日期", true, false);

                //@0=网站注册@1=企业微信@2=钉钉@3=微信小程序
                map.AddTBInt(OrgAttr.RegFrom, 0, "注册来源", true, false);
                map.AddTBInt(OrgAttr.WXUseSta, 0, "状态@0=未安装1=使用中@2=卸载.", true, false);

                map.AddTBString(OrgAttr.UrlFrom, null, "网站来源", true, true, 0, 30, 20);
                map.AddTBString(OrgAttr.DB, null, "DB", true, false, 0, 32, 36);
                #endregion 字段

                #region 微信信息
                map.AddTBString(OrgAttr.CorpID, null, "授权方企业微信id", true, false, 0, 200, 36);
                map.AddTBString(OrgAttr.AgentName, null, "授权方企业名称，即企业简称", true, false, 0, 200, 36);
                map.AddTBString(OrgAttr.AgentId, null, "授权方应用id", true, false, 0, 200, 36);
                map.AddTBString(OrgAttr.PermanentCode, null, "企业微信永久授权码", true, false, 0, 200, 36);
                map.AddTBString(OrgAttr.AccessToken, null, "授权方（企业）access_token", true, false, 0, 200, 36);

                map.AddTBDateTime(OrgAttr.AccessTokenExpiresIn, null, "授权方（企业）access_token失效时间", true, false);

                //map.AddTBString(OrgAttr.CorpSquareLogoUrl, null, "授权方企业方形头像", true, false, 0, 4000, 36);
                //map.AddTBString(OrgAttr.CorpRoundLogoUrl, null, "授权方企业圆形头像", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpUserMax, null, "授权方企业用户规模", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpAgentMax, null, "授权方企业应用数上限", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpFullName, null, "授权方企业的主体名称", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.SubjectType, null, "企业类型，1. 企业; 2. 政府以及事业单位; 3. 其他组织, 4.团队号", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.VerifiedEndTime, null, "认证到期时间", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpScale, null, "企业规模。当企业未设置该属性时，值为空", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpIndustry, null, "企业所属行业。当企业未设置该属性时，值为空", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.CorpSubIndustry, null, "企业所属子行业。当企业未设置该属性时，值为空", true, false, 0, 4000, 36);
                map.AddTBString(OrgAttr.Location, null, "企业所在地信息, 为空时表示未知", true, false, 0, 4000, 36);
                // map.AddTBString(OrgAttr.SquareLogoUrl, null, "授权方应用方形头像", true, false, 0, 4000, 36);
                //map.AddTBString(OrgAttr.RoundLogoUl, null, "授权方应用圆形头像", true, false, 0, 4000, 36);
                #endregion 微信信息
                map.AddTBInt(OrgAttr.OrgSta, 1, "组织状态", true, false);
                RefMethod rm = new RefMethod();
                rm.Title = "设置图片签名";
                rm.ClassMethodName = this.ToString() + ".DoICON";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoICON()
        {
            return "../../../GPM/OrgDepts.htm?FK_Org=" + this.No;
        }

        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

            return base.beforeUpdateInsertAction();
        }
         
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Orgs(); }
        }
        #endregion 构造函数

        /// <summary>
        /// 初始化数据.
        /// </summary>
        public void Init_OrgDatas()
        {
            #region 初始化 - 流程树.
            BP.WF.Template.FlowSort fs = new BP.WF.Template.FlowSort();
            fs.No = this.No; //公司编号
            fs.Name = "流程树";
            fs.OrgNo = this.No;
            fs.ParentNo = "100"; //这里固定死了,必须是100.
            fs.DirectInsert();

            fs.No = DBAccess.GenerGUID(5, "Port_Dept", "No");
            fs.ParentNo = this.No; //帐号信息.
            fs.Name = "日常办公";
            fs.OrgNo = this.No;
            fs.DirectInsert();

            fs.No = DBAccess.GenerGUID();
            fs.ParentNo = this.No; //帐号信息.
            fs.Name = "财务类";
            fs.OrgNo = this.No;
            fs.DirectInsert();

            fs.No = DBAccess.GenerGUID();
            fs.ParentNo = this.No;
            fs.Name = "人力资源类";
            fs.OrgNo = this.No;
            fs.DirectInsert();
            #endregion 开始流程树.

            #region  加载流程模版.
            //类别.
            BP.WF.Template.FlowSorts fss = new BP.WF.Template.FlowSorts();
            fss.Retrieve("OrgNo", this.No);
            fs = fss[0] as BP.WF.Template.FlowSort;
            #endregion

            #region 表单树.
            BP.WF.Template.SysFormTree ft = new BP.WF.Template.SysFormTree();
            ft.No = this.No; //公司编号
            ft.Name = "表单树";
            ft.OrgNo = this.No;
            ft.ParentNo = "100"; //这里固定死了必须是100.
            ft.DirectInsert();

            ft.No = DBAccess.GenerGUID();
            ft.ParentNo = this.No; //帐号信息.
            ft.Name = "日常办公";
            ft.OrgNo = this.No;
            ft.DirectInsert();

            ft.No = DBAccess.GenerGUID();
            ft.ParentNo = this.No; //帐号信息.
            ft.Name = "财务类";
            ft.OrgNo = this.No;
            ft.DirectInsert();

            ft.No = DBAccess.GenerGUID();
            ft.ParentNo = this.No;
            ft.Name = "人力资源类";
            ft.OrgNo = this.No;
            ft.DirectInsert();
            #endregion 表单树.

            //如果是web注册 =0，就去掉. 
            if (this.RegFrom != 0)
                return;

            #region 初始化部门.
            //根目录.
            Dept dept = new Dept();
            dept.No = this.No;
            dept.ParentNo = "100";
            dept.Name = this.Name;
            dept.OrgNo = this.No;
            dept.Adminer = this.Adminer;
            dept.DirectInsert();

            Emp ep = new Emp(this.No + "_" + this.Adminer);
            ep.DeptNo = this.No;
            ep.OrgNo = this.No; //所在公司.
           
            ep.Update();

            //写入Port_OrgAdminer表中，供查询是否是管理员.
            BP.WF.Port.Admin2Group.OrgAdminer orgAdminer = new BP.WF.Port.Admin2Group.OrgAdminer();
            orgAdminer.EmpNo = ep.UserID;
            orgAdminer.OrgNo = this.No;
            orgAdminer.DirectInsert();

            //管理员.
            this.Adminer = ep.UserID;
            this.AdminerName = ep.Name;

            //把这个人员放入到根目录下.
            DeptEmp de = new DeptEmp();
            de.OrgNo = ep.OrgNo;
            de.DeptNo = this.No;
            de.EmpNo = ep.UserID;
            de.setMyPK(de.DeptNo + "_" + de.EmpNo);
            de.DirectInsert();

            #region 开始创建下级部门.

            dept.No = DBAccess.GenerGUID(); // (deptNoLen, "Port_Dept", "No");
            dept.ParentNo = this.No; //帐号信息.
            dept.Name = "总经理部";
            dept.OrgNo = this.No;
            dept.Adminer = null;
            dept.DirectInsert();

            dept.No = DBAccess.GenerGUID(); // (deptNoLen, "Port_Dept", "No");
            dept.ParentNo = this.No; //帐号信息.
            dept.Name = "信息部";
            dept.OrgNo = this.No;
            dept.Adminer = null;
            dept.DirectInsert();

            //把当前的人员放入到信息部里面去.
            DeptEmp myde = new DeptEmp();
            //设置主键.
            myde.setMyPK(dept.No + "_" + this.Adminer);

            myde.DeptNo = dept.No;
            myde.EmpNo = this.Adminer;

           // myde.EmpNo = this.No + "_" + this.Adminer;

            myde.OrgNo = this.No;
            myde.Insert();


            dept.No = DBAccess.GenerGUID(); // (deptNoLen, "Port_Dept", "No");
            dept.ParentNo = this.No; //帐号信息.
            dept.Name = "财务部";
            dept.OrgNo = this.No;
            dept.Adminer = null;
            dept.DirectInsert();

            dept.No = DBAccess.GenerGUID(); // (deptNoLen, "Port_Dept", "No");
            dept.ParentNo = this.No;
            dept.Name = "人力资源部";
            dept.Adminer = null;
            dept.OrgNo = this.No;
            dept.DirectInsert();
            #endregion 开始创建下级部门.

            #endregion 初始化部门.

            #region Init 角色类别.

            #region 高层角色.
            StationType st = new StationType();
            st.No = DBAccess.GenerGUID();
            st.Name = "高层";
            st.OrgNo = this.No;
            st.DirectInsert();

            Station sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "总经理岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "副总经理岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();
            #endregion 高层角色.

            #region 中层角色
            st = new StationType();
            st.No = DBAccess.GenerGUID();
            st.Name = "中层";
            st.OrgNo = this.No;
            st.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "信息部部长";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();

            //把当前的人员放入到信息部里面去.
            DeptEmpStation des = new DeptEmpStation();
            des.DeptNo = myde.DeptNo;
            des.EmpNo = this.Adminer;
            des.StationNo = sta.No; //信息部部长.
            des.OrgNo = this.No;
            des.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "财务部经理岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "人力资源部经理岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();
            #endregion 中层角色

            #region 基层
            st = new StationType();
            st.No = DBAccess.GenerGUID();
            st.Name = "基层";
            st.OrgNo = this.No;
            st.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "程序员岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "出纳岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();

            sta = new Station();
            sta.No = DBAccess.GenerGUID();
            sta.Name = "人力资源助理岗";
            sta.OrgNo = this.No;
            sta.FK_StationType = st.No;
            sta.DirectInsert();
            #endregion 基层

            #endregion Init 角色类别.

            //return "url@/Admin/Portal/Default.htm?Token=" + ep.SID + "&UserNo=" + ep.No;
        }
        /// <summary>
        /// 生成一个唯一的GUID》
        /// </summary>
        /// <returns></returns>
        public static string GenerNewOrgNo()
        {
            while (true)
            {
                string chars = DBAccess.GenerGUID().ToString().Substring(0, 4);
                string str = chars.Substring(0, 1);
                if (DataType.IsNumStr(str) == true)
                    continue;

                string sql = "SELECT count(No) as Num FROM Port_Org WHERE No='" + chars + "'";
                if (DBAccess.RunSQLReturnValInt(sql) >= 1)
                    continue;
                return chars;
            }
        }
        /// <summary>
        /// 执行删除.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            throw new Exception("不允许删除.");
        }
        public void DoDelete()
        {
            //删除流程，删除数据.
             BP.WF.Flows fls = new BP.WF.Flows();
             fls.Retrieve(BP.WF.Template.FlowAttr.OrgNo, this.No);
             foreach (BP.WF.Flow item in fls)
             {
                 item.DoDelData();
                 item.DoDelete();
             }

            //删除组织数据.
            DBAccess.RunSQL("DELETE FROM Port_Emp WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Port_Dept WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Port_DeptEmp WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation WHERE OrgNo='" + this.No + "'");

            //删除管理员.
            DBAccess.RunSQL("DELETE FROM Port_OrgAdminer WHERE OrgNo='" + this.No + "'");


            //删除类别.
            DBAccess.RunSQL("DELETE FROM WF_FlowSort WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Sys_FormTree WHERE OrgNo='" + this.No + "'");

            this.DirectDelete();
        }
        public void DoDeletePassPort()
        {
            //删除组织数据.
            DBAccess.RunSQL("DELETE FROM Port_Emp WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Port_Dept WHERE OrgNo='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM Port_DeptEmp WHERE OrgNo='" + this.No + "'");

            //删除管理员.
            DBAccess.RunSQL("DELETE FROM Port_OrgAdminer WHERE OrgNo='" + this.No + "'");

            this.DirectDelete();
        }
    }
   
    /// <summary>
    /// 组织s
    // </summary>
    public class Orgs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Org();
            }
        }
        /// <summary>
        /// 组织s
        /// </summary>
        public Orgs()
        {
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Org> ToJavaList()
        {
            return (System.Collections.Generic.IList<Org>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Org> Tolist()
        {
            System.Collections.Generic.List<Org> list = new System.Collections.Generic.List<Org>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Org)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
