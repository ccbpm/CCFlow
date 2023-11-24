using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.Cloud
{
    /// <summary>
    /// 组织 属性
    /// </summary>
    public class OrgExtAttr : OrgAttr
    {
        #region 基本属性
        #endregion
    }
    /// <summary>
    /// 组织 的摘要说明。
    /// </summary>
    public class OrgExt : EntityNoName
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

                string sql = "SELECT COUNT(FK_OrgExt) FROM Port_DeptOrgExtStation WHERE FK_OrgExt='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_OrgExt) FROM Port_DeptOrgExt WHERE FK_OrgExt='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                return true;
            }
        }
        public string Addr
        {
            get
            {
                
                return this.GetValStrByKey(OrgExtAttr.Addr);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.Addr, value);
            }
        }
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(OrgExtAttr.GUID);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.GUID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgExtAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string NameFull
        {
            get
            {
                return this.GetValStrByKey(OrgExtAttr.NameFull);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.NameFull, value);
            }
        }
        /// <summary>
        /// 统计用的JSON
        /// </summary>
        public string JSONOfTongJi
        {
            get
            {
                return this.GetValStrByKey(OrgExtAttr.JSONOfTongJi);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.JSONOfTongJi, value);
            }
        }
        /// <summary>
        /// 注册年月
        /// </summary>
        public string FK_HY
        {
            get
            {
                return this.GetValStrByKey(OrgExtAttr.FK_HY);
            }
            set
            {
                this.SetValByKey(OrgExtAttr.FK_HY, value);
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

        public string DTEnd
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.DTEnd);
            }
        }

        public int OrgSta
        {
            get
            {
                return this.GetValIntByKey(OrgAttr.OrgSta);
            }
            set
            {
                this.SetValByKey(OrgAttr.OrgSta, value);
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
        public OrgExt()
        {
        }
        public OrgExt(string  no)
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

                if (WebUser.No.Equals("admin")==true)
                {
                    uac.OpenAll();
                    uac.IsInsert = false;
                    uac.IsDelete = false;

                    return uac;
                }

                uac.IsInsert = false;
                uac.IsDelete = false;
                if (this.No.Equals(WebUser.OrgNo)==true)
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
                map.EnType = EnType.App;

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgExtAttr.No, null, "账号OrgNo", true, false, 1, 50, 90);
                map.AddTBString(OrgExtAttr.Name, null, "简称", true, false, 0, 200, 130);
                map.AddTBString(OrgExtAttr.NameFull, null, "全称", true, false, 0, 300, 400,true);
                map.AddTBString(OrgExtAttr.Adminer, null, "管理员帐号", true, false, 0, 300, 400);
                map.AddTBString(OrgExtAttr.AdminerName, null, "管理员名称", true, false, 0, 300, 400);

                map.AddTBString(OrgExtAttr.Addr, null, "地址", true, false, 0, 300, 36,true);
                map.AddTBString(OrgExtAttr.GUID, null, "GUID", true, true, 0, 32, 36);

                map.AddDDLSysEnum(OrgExtAttr.RegFrom, 0, "注册来源", true, false,
                    OrgExtAttr.RegFrom,"@0=网站注册@1=微信注册@2=钉钉注册");
                
                string msg = "注册来源";
                msg += "\t\n 1.只有网站注册的才可以维护组织结构";
                msg += "\t\n 2.非网站注册的组织结构的维护在钉钉或者微信里面，维护后系统自动会同步.";
                map.SetHelperAlert(OrgExtAttr.RegFrom, msg);

                map.AddTBDateTime(OrgAttr.DTReg, null, "注册日期", true, true);
                map.AddTBDateTime(OrgAttr.DTEnd, null, "停用日期", true, false);

                map.AddDDLSysEnum(OrgExtAttr.OrgSta, 1, "启用状态", true, false,
                OrgExtAttr.OrgSta, "@0=停用@1=运行中");
                #endregion 字段

                //查询条件.
                map.AddSearchAttr(OrgExtAttr.OrgSta);
                map.AddSearchAttr(OrgExtAttr.RegFrom);

                RefMethod rm = new RefMethod();
                rm.Title = "设置ICON";
                rm.ClassMethodName = this.ToString() + ".DoICON";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "清除组织";
                //rm.Warning = "您确定要删除该组织下的所有数据吗？包括：人员、部门、岗位、流程、表单等数据。<strong><font color='red'>清除后数据不可以恢复</font></strong>";
                //rm.ClassMethodName = this.ToString() + ".DoBDelete";
                //rm.IsCanBatch = true;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoBDelete()
        {
            if (WebUser.No.ToLower() != "admin")
                return "没有取得授权";

            BP.WF.Flows fls = new BP.WF.Flows();
            fls.Retrieve("OrgNo", this.No);
            foreach (BP.WF.Flow fl in fls)
            {
                fl.DoDelData();
                fl.DoDelete();
            }

            MapDatas mds = new MapDatas();
            mds.Retrieve("OrgNo", this.No);
            foreach (MapData md in mds)
            {
                md.Delete();
            }

            //删除目录树.
            DBAccess.RunSQL("DELETE WF_FlowSort WHERE OrgNo='"+this.No+"'");
            DBAccess.RunSQL("DELETE Sys_FormTree WHERE OrgNo='" + this.No + "'");

            DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE OrgNo='" + this.No + "'");


            this.Delete();
            return "删除成功";
        }

        public string DoICON()
        {
            return "/Admin/Setting/ICON.html?OrgNo=" + this.No;
        }

        protected override bool beforeInsert()
        {
            this.GUID = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        /// <summary>
        /// 更新前时间
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            if (DataType.IsNullOrEmpty(this.DTEnd) == true)
            {
                this.OrgSta = 1;
                return base.beforeUpdate();

            }

            //如果结束时间小于当前时间就停用，大于当前时间就启用
            if (this.DTEnd.CompareTo(DataType.CurrentDataTime) >= 0)
                this.OrgSta = 1;
            else
                this.OrgSta = 0;
            return base.beforeUpdate();
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

        protected override bool beforeDelete()
        {
            Org org = new Org(this.No);
            org.DoDelete();

            return base.beforeDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";
            return py;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new OrgExts(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 组织s
    // </summary>
    public class OrgExts : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgExt();
            }
        }
        /// <summary>
        /// 组织s
        /// </summary>
        public OrgExts()
        {
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgExt> Tolist()
        {
            System.Collections.Generic.List<OrgExt> list = new System.Collections.Generic.List<OrgExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
