using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.Cloud.OrgSetting
{
    /// <summary>
    /// 组织 属性
    /// </summary>
    public class OrgAttr : BP.Cloud.OrgAttr
    {
        #region 基本属性
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
        public bool IsEnable
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
            set
            {
                this.SetValByKey(OrgAttr.GUID, value);
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
        #endregion

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

                if (WebUser.No.Equals("admin") == true)
                {
                    uac.OpenAll();
                    uac.IsInsert = false;
                    uac.IsDelete = false;

                    return uac;
                }

                uac.IsInsert = false;
                uac.IsDelete = false;
                if (this.No.Equals(WebUser.OrgNo) == true)
                {
                    uac.IsUpdate = true;
                    return uac;
                }

                //删除.
                uac.IsInsert = false;
                uac.IsDelete = false;
                uac.IsView = false;

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

                #region 基本信息
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgAttr.No, null, "账号OrgNo", true, false, 1, 50, 90);
                map.AddTBString(OrgAttr.Name, null, "简称", true, false, 0, 200, 130);
                map.AddTBString(OrgAttr.NameFull, null, "全称", true, false, 0, 300, 400, true);
                map.AddTBString(OrgAttr.Adminer, null, "管理员帐号", true, true, 0, 300, 400);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 300, 400);
                map.AddTBString(OrgAttr.Addr, null, "地址", true, false, 0, 300, 36, true);
                map.AddDDLSysEnum(OrgAttr.RegFrom, 0, "注册来源", false, false,
                    OrgAttr.RegFrom, "@0=网站注册@1=微信注册@2=钉钉注册");

                string msg = "注册来源";
                msg += "\t\n 1.只有网站注册的才可以维护组织结构";
                msg += "\t\n 2.非网站注册的组织结构的维护在钉钉或者微信里面，维护后系统自动会同步.";
                map.SetHelperAlert(OrgAttr.RegFrom, msg);

                map.AddTBDateTime(OrgAttr.DTReg, null, "注册日期", true, true);
                map.AddTBDateTime(OrgAttr.DTEnd, null, "停用日期", true, true);
                //map.AddDDLSysEnum(OrgAttr.UseSta, 0, "启用状态", true, false,OrgAttr.UseSta,
                //    "@0=注册@1=使用中");
                #endregion 基本信息

                #region 一般配置
                // map.AddTBString(OrgAttr.HostURL, null, "服务器的URL", true, false, 0, 300, 36, true);
                //  map.SetHelperAlert();

                //map.AddTBString(OrgAttr.MobileURL, null, "移动端服务器地址", true, false, 0, 300, 36, true);
                #endregion 一般配置

                RefMethod rm = new RefMethod();
                rm.Title = "设置ICON";
                rm.ClassMethodName = this.ToString() + ".DoICON";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "消息设置";
                rm.ClassMethodName = this.ToString() + ".DoMsg";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "邮件设置";
                rm.ClassMethodName = this.ToString() + ".DoEmail";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "邮件发送测试";
                rm.ClassMethodName = this.ToString() + ".DoTestEmail";
                rm.HisAttrs.AddTBString("email", "ccbpm@ccbpm.cn", "邮件", true, false, 0, 1000, 100);
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "合并企业微信";
                rm.ClassMethodName = this.ToString() + ".DoHeBing";
                rm.HisAttrs.AddTBString("OrgNo", "", "企业组织编号", true, false, 0, 1000, 100);
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoHeBing(string orgNo)
        {
            #region  //检查部门.
            string sql = "SELECT * FROM Port_Dept WHERE OrgNo='" + orgNo + "' ORDER BY No,Idx";
            DataTable dtDeptWX = DBAccess.RunSQLReturnTable(sql);

            sql = "SELECT * FROM Port_Dept WHERE OrgNo='" + this.No + "' ORDER BY No,Idx";
            DataTable dtDept = DBAccess.RunSQLReturnTable(sql);
            if (dtDept.Rows.Count != dtDeptWX.Rows.Count)
                return "err@当前组织部门数量:【"+dtDept.Rows.Count+"】个，被合并的微信组织部门数量["+ dtDeptWX.Rows.Count+ "]个.";
            #endregion  //检查部门.

            #region  //检查人员.
            //检查人员.
            sql = "SELECT * FROM Port_Emp WHERE OrgNo='" + orgNo + "' ORDER BY FK_Dept,Idx ";
            DataTable dtEmpWX = DBAccess.RunSQLReturnTable(sql);

            sql = "SELECT * FROM Port_Emp WHERE OrgNo='" + this.No + "' ORDER BY FK_Dept,Idx ";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            if (dtEmp.Rows.Count != dtEmpWX.Rows.Count)
                return "err@当前组织人员数量:【" + dtDept.Rows.Count + "】个，被合并的微信数量[" + dtDeptWX.Rows.Count + "]个.";
            #endregion  //检查人员.


            //开始同步部门信息.


            //开始同步人员信息.


            //复制过来组织的weixin 信息.


            return "";
        }
        public string DoTestEmail(string emial)
        {
            try
            {
                OrgMail org = new OrgMail(this.No);
                org.SendEmail(emial, "科伦BPM-系统发送邮件测试", "系统发送测试,不需回复.");

                return "发送成功，请检查是否收到邮件[" + emial + "].";
            }
            catch (Exception ex)
            {
                return "err@测试错误:" + ex.Message;
            }
        }
        public string DoMsg()
        {
            return "/WF/Comm/RefFunc/EnOnly.htm?EnName=BP.Cloud.OrgSetting.OrgMsg&No=" + this.No;
        }
        public string DoEmail()
        {
            return "/WF/Comm/RefFunc/EnOnly.htm?EnName=BP.Cloud.OrgSetting.OrgMail&No=" + this.No;
        }
        public string DoICON()
        {
            return "/Admin/Setting/ICON.html?OrgNo=" + this.No;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Orgs(); }
        }
        #endregion 构造函数
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
